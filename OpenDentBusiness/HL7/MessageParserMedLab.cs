using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	///<summary>This is the engine that will parse our incoming HL7 messages for MedLab interfaces.</summary>
	public class MessageParserMedLab {
		private static bool _isVerboseLogging;
		private static MedLab _medLabCur;
		private static MedLabResult _medLabResultCur;
		private static Patient _patCur;
		private static HL7Def _defCur;
		private static string _msgArchiveFilePath;
		private static List<long> _medLabNumList;
		//key is facility footnote ID linked to value List<MedLabFacilityNum>.  Each key should only link to a single MedLabFacility, but if a result came
		//from more than one facility, the same footnote ID could point to more than one facility and result in multiple MedLabAttach rows to link the
		//result to multiple facilities
		private static Dictionary<string,List<long>> _dictFacilityCodeNum;
		private static string _medicaidIdCur;
		private static bool _isFirstObr;
		private static bool _isObsValueNte;

		///<summary>Processes the msg and creates the MedLab, MedLabResult, MedLabSpecimen, MedLabFacility, and MedLabAttach objects.
		///Stores the msgArchiveFileName for each MedLab object created from the inbound message.
		///Each message will result in one MedLab object for each repitition of the ORC/OBR observation group in the message.
		///Each of the MedLab objects created will be linked to the msgArchiveFileName supplied.
		///Each repetition of the OBX result group will result in a MedLabResult object.
		///This returns a list of the MedLab.MedLabNums for all MedLab objects created from this message.
		///Use selectedPat to manually specify the patient to attach the objects and embedded files to.  Used when a patient could not be located using
		///the original message PID segment info and the user has now manually selected a patient.  The ZEF segments would not have been processed if a
		///patient could not be located and once the user selects the patient we will need to re-process the message to create the embedded PDFs.</summary>
		public static List<long> Process(MessageHL7 msg,string msgArchiveFilePath,bool isVerboseLogging,Patient selectedPat=null) {
			_isVerboseLogging=isVerboseLogging;
			_msgArchiveFilePath=msgArchiveFilePath;
			_medLabNumList=new List<long>();
			_medLabCur=null;//make sure the last medlab object is cleared out
			_medLabResultCur=null;
			_patCur=null;
			HL7Def def=HL7Defs.GetOneDeepEnabled(true);
			if(def==null) {
				throw new Exception("Could not process the MedLab HL7 message.  No MedLab HL7 definition is enabled.");
			}
			_defCur=def;
			HL7DefMessage hl7defmsg=null;
			for(int i=0;i<def.hl7DefMessages.Count;i++) {
				//for now there are only incoming ORU messages supported, so there should only be one defined message type and it should be inbound
				if(def.hl7DefMessages[i].MessageType==msg.MsgType && def.hl7DefMessages[i].InOrOut==InOutHL7.Incoming) {
					hl7defmsg=def.hl7DefMessages[i];
					break;
				}
			}
			if(hl7defmsg==null) {//No message definition matches this message's MessageType and is Incoming
				throw new Exception("Could not process the MedLab HL7 message.  There is no definition for this type of message in the enabled MedLab HL7Def.");
			}
			#region Locate Patient
			//for MedLab interfaces, we limit the ability to rearrange the message structure, so the PID segment is always in position 1
			if(hl7defmsg.hl7DefSegments.Count<2 || msg.Segments.Count<2) {
				throw new Exception("Could not process the MedLab HL7 message.  "
					+"The message or message definition for this type of message does not contain the correct number of segments.");
			}
			HL7DefSegment pidSegDef=hl7defmsg.hl7DefSegments[1];
			SegmentHL7 pidSegCur=msg.Segments[1];
			if(pidSegDef.SegmentName!=SegmentNameHL7.PID || pidSegCur.Name!=SegmentNameHL7.PID) {
				throw new Exception("Could not process the MedLab HL7 message.  "
					+"The second segment in the message or message definition is not the PID segment.");
			}
			if(selectedPat==null) {
				//get the patient from the PID segment
				_patCur=GetPatFromPID(pidSegDef,pidSegCur);
			}
			if(_patCur==null) {//if no patient is located using PID segment or if selectedPat is not null, use selectedPat
				_patCur=selectedPat;//selectedPat could be null as well, but null _patCur is handled
			}
			#endregion Locate Patient
			#region Validate Message Structure
			if(hl7defmsg.hl7DefSegments.Count<1 || msg.Segments.Count<1) {
				throw new Exception("Could not process the MedLab HL7 message.  "
					+"The message or message definition for this type of message does not contain the correct number of segments.");
			}
			SegmentHL7 mshSegCur=msg.Segments[0];
			if(hl7defmsg.hl7DefSegments[0].SegmentName!=SegmentNameHL7.MSH || mshSegCur.Name!=SegmentNameHL7.MSH) {
				throw new Exception("Could not process the MedLab HL7 message.  "
					+"The first segment in the message or the message definition is not the MSH segment.");
			}
			SegmentHL7 nk1SegCur=null;
			List<SegmentHL7> listNteSegs=new List<SegmentHL7>();
			List<SegmentHL7> listZpsSegs=new List<SegmentHL7>();
			int indexFirstOrc=0;
			int indexFirstZps=0;
			//the third item could be the optional NK1 segment, followed by an optional and repeatable NTE segment, so the first ORC segment
			//could be anywhere after the PID segment in position 2.  This will tell us where the first ORC (order) repeatable group begins.
			for(int i=2;i<msg.Segments.Count;i++) {
				if(indexFirstZps>0 && msg.Segments[i].Name!=SegmentNameHL7.ZPS) {
					throw new Exception("Could not process the MedLab HL7 message.  There is a "+msg.Segments[i].Name.ToString()
						+" segment after a ZPS segment.  Incorrect message structure.");
				}
				if(msg.Segments[i].Name==SegmentNameHL7.NK1) {
					//the NK1 segment must come after the PID segment
					if(msg.Segments[i-1].Name!=SegmentNameHL7.PID) {
						throw new Exception("Could not process the MedLab HL7 message.  The NK1 segment was in the wrong position.  Incorrect message structure.");
					}
					nk1SegCur=msg.Segments[i];
					continue;
				}
				if(indexFirstOrc==0 && msg.Segments[i].Name==SegmentNameHL7.NTE) {//if we find an ORC segment, the NTEs that follow won't be at the PID level
					//the PID level NTE segments can follow after the PID segment, the optional NK1 segment, or other repetitions of the NTE segment
					if(msg.Segments[i-1].Name!=SegmentNameHL7.PID
						&& msg.Segments[i-1].Name!=SegmentNameHL7.NK1
						&& msg.Segments[i-1].Name!=SegmentNameHL7.NTE) 
					{
						throw new Exception("Could not process the MedLab HL7 message.  Found a NTE segment before an ORC segment but after a "
							+msg.Segments[i-1].Name.ToString()+" segment.  Incorrect message structure.");
					}
					listNteSegs.Add(msg.Segments[i]);
					continue;
				}
				if(msg.Segments[i].Name==SegmentNameHL7.ZPS) {
					if(indexFirstZps==0) {//this is the first ZPS segment we've encountered, set the index
						indexFirstZps=i;
					}
					listZpsSegs.Add(msg.Segments[i]);
					continue;
				}
				if(indexFirstOrc==0 && msg.Segments[i].Name==SegmentNameHL7.ORC) {//this is the first ORC segment we've encountered, set the index
					indexFirstOrc=i;
				}
			}
			#endregion Validate Message Structure
			#region Process Order Observations
			//We can process the ZPS segments before any of the other segments and then link each of the
			//MedLab objects created to each of the MedLabFacilities referenced by the ZPSs
			ProcessSeg(hl7defmsg,listZpsSegs,msg);
			List<SegmentHL7> listRepeatSegs=new List<SegmentHL7>();
			_isFirstObr=true;
			for(int i=indexFirstOrc;i<indexFirstZps;i++) {
				//for every repetition of the order observation, process the MSH, PID, NK1 and PID level NTE segments for a new MedLab object
				if(msg.Segments[i].Name==SegmentNameHL7.ORC) {
					ProcessSeg(hl7defmsg,new List<SegmentHL7> { mshSegCur },msg);//instatiates a new MedLab object _medLabCur and inserts to get PK
					ProcessSeg(hl7defmsg,new List<SegmentHL7> { pidSegCur },msg);
					//ProcessSeg(hl7defmsg,new List<SegmentHL7> { nk1SegCur },msg);
					if(listNteSegs.Count>0) {
						ProcessSeg(hl7defmsg,listNteSegs,msg,SegmentNameHL7.PID);
					}
					listRepeatSegs=new List<SegmentHL7>();
				}
				if(msg.Segments[i].Name==SegmentNameHL7.NTE
					|| msg.Segments[i].Name==SegmentNameHL7.ZEF
					|| msg.Segments[i].Name==SegmentNameHL7.SPM)
				{
					SegmentNameHL7 prevSegName=msg.Segments[i-1].Name;
					listRepeatSegs.Add(msg.Segments[i]);
					for(int j=i+1;j<indexFirstZps;j++) {
						if(msg.Segments[j].Name!=msg.Segments[i].Name) {
							i=j-1;
							break;
						}
						listRepeatSegs.Add(msg.Segments[j]);
					}
					ProcessSeg(hl7defmsg,listRepeatSegs,msg,prevSegName);
					listRepeatSegs=new List<SegmentHL7>();//clear out list for next repeatable segment
					continue;
				}
				//if the segment is an OBX, ProcessOBX will instantiate a new MedLabResult object, one for each repetition of the OBX, ZEF, NTE group
				ProcessSeg(hl7defmsg,new List<SegmentHL7> { msg.Segments[i] },msg);
			}
			MedLabs.Update(_medLabCur);
			MedLabResults.Update(_medLabResultCur);
			#endregion Process Order Observations
			return _medLabNumList;
		}

		///<summary>Finds a patient from the information in the PID segment, using the PID segment definition in the enabled HL7 def.
		///Will return null if a patient cannot be found using the information in the PID segment.  If an alternate patient ID is
		///provided and the name and birthdate in the PID segment match the name and birthdate of the patient located,
		///it will be stored in the oidexternal table linked to the patient's PatNum.</summary>
		public static Patient GetPatFromPID(HL7DefSegment pidSegDef,SegmentHL7 pidSeg) {
			//PID.2 should contain the OpenDental PatNum.
			//If there is no patient with PatNum=PID.2, we will attempt to find the value in PID.4 in the oidexternals table IDExternal column
			//with root MedLabv2_3.Patient and type IdentifierType.Patient.
			//The PatNum found in the oidexternals table will only be trusted if the name and birthdate of the pat match the name and birthdate in the msg.
			//If there is no entry in the oidexternals table, we will attempt to match by name and birthdate alone.
			//If a patient is found, an entry will be made in the oidexternals table with IDExternal=PID.4, rootExternal=MedLabv2_3.Patient,
			//IDType=IdentifierType.Patient, and IDInternal=PatNum if one does not already exist.
			long patNum=0;
			long patNumFromAlt=0;
			string altPatID="";
			string patLName="";
			string patFName="";
			DateTime birthdate=DateTime.MinValue;
			//Go through fields of PID segment and get patnum, altPatNum, patient name, and birthdate to locate patient
			for(int i=0;i<pidSegDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=pidSegDef.hl7DefFields[i];
				OIDExternal oidCur=null;
				switch(fieldDefCur.FieldName) {
					case "altPatID":
						altPatID=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						if(altPatID=="") {
							continue;
						}
						oidCur=OIDExternals.GetByRootAndExtension(HL7InternalType.MedLabv2_3.ToString()+".Patient",altPatID);
						if(oidCur==null || oidCur.IDType!=IdentifierType.Patient) {
							//not in the oidexternals table or the oidexternal located is not for a patient object, patNumFromAlt will remain 0
							continue;
						}
						patNumFromAlt=oidCur.IDInternal;
						continue;
					case "patBirthdateAge":
						//LabCorp sends the birthdate and age in years, months, and days like yyyyMMdd^YYY^MM^DD
						birthdate=FieldParser.DateTimeParse(pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					default:
						continue;
					case "pat.nameLFM":
						patLName=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos,0);
						patFName=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1);
						continue;
					case "pat.PatNum":
						try {
							patNum=PIn.Long(pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, patNum will remain 0
						}
						continue;
				}
			}
			//We now have patNum, patNumFromAlt, patLName, patFName, and birthdate so locate pat
			Patient patCur=Patients.GetPat(patNum);//will be null if not found or patNum=0
			#region Upsert the oidexternals Table
			//insert/update the altPatID and labPatID in the oidexternals table if a patient was found and the patient's name and birthdate match the message
			if(patCur!=null && IsMatchNameBirthdate(patCur,patLName,patFName,birthdate)) {
				//the altPatID field was populated in the PID segment (usually PID.4) and the altPatID isn't stored in the oidexternals table as IDExternal
				if(altPatID!="") {
					if(patNumFromAlt==0) { //if the altPatID isn't stored in the oidexternals table as IDExternal, insert
						OIDExternal oidCur=new OIDExternal();
						oidCur.IDType=IdentifierType.Patient;
						oidCur.IDInternal=patCur.PatNum;
						oidCur.rootExternal=HL7InternalType.MedLabv2_3.ToString()+".Patient";
						oidCur.IDExternal=altPatID;
						OIDExternals.Insert(oidCur);
					}
					else if(patCur.PatNum!=patNumFromAlt) { //else if patCur.PatNum is different than the IDInternal stored in the oidexternals table, update
						OIDExternal oidCur=OIDExternals.GetByRootAndExtension(HL7InternalType.MedLabv2_3.ToString()+".Patient",altPatID);
						oidCur.IDInternal=patCur.PatNum;
						OIDExternals.Update(oidCur);
					}
				}
			}
			#endregion Upsert the oidexternals Table
			//patCur is null so try to find a patient from the altPatID (PID.4)
			if(patCur==null && patNumFromAlt>0) {
				patCur=Patients.GetPat(patNumFromAlt);
				//We will only trust the altPatID if the name and birthdate of the patient match the name and birthdate in the message.
				if(!IsMatchNameBirthdate(patCur,patLName,patFName,birthdate)) {
					patCur=null;
				}
			}
			//if we haven't located a patient yet, attempt with name and birthdate
			if(patCur==null && patLName!="" && patFName!="" && birthdate>DateTime.MinValue) {
				long patNumByName=Patients.GetPatNumByNameAndBirthday(patLName,patFName,birthdate);
				if(patNumByName>0) {
					patCur=Patients.GetPat(patNumByName);
				}
			}
			return patCur;
		}

		///<summary>Compare the first and last name of the patient to the string values.  The string value for lname cannot exceed 25 chars,
		///so we only compare the first 25 chars.  The string value for fname cannot exceed 15 chars, so we only compare the first 15 chars.
		///These limitations are set by LabCorp in their HL7 documentation.  If name and birthdate match, this returns true.  Otherwise false.
		///If patCur is null, returns false.</summary>
		public static bool IsMatchNameBirthdate(Patient patCur,string lname,string fname,DateTime birthdate) {
			if(patCur!=null
				&& patCur.Birthdate.Date==birthdate.Date
				&& patCur.LName.ToLower().PadRight(25).Substring(0,25)==lname.ToLower().PadRight(25).Substring(0,25)
				&& patCur.FName.ToLower().PadRight(15).Substring(0,15)==fname.ToLower().PadRight(15).Substring(0,15)) 
			{
				return true;
			}
			return false;
		}

		///<summary>Returns the segment definion based on the name supplied, and only if it follows the previous segment name supplied.  If prevSegName
		///is Unknown, we'll return the first match by segName only.  If a segment can appear in a message definition more than once and in multiple
		///locations, send in the preceding segment name.  For example, the NTE seg can follow the PID (or optional NK1), OBR, and OBX (or optional ZEF)
		///segments. Returns the definition that immediately follows the previous segment name given. If the segment def follows an optional
		///segment, the prevSegName could be any preceding segment up to and including the first preceding required segment. We'll decrement backward
		///through the segments looking for a predecessor with the prevSegName until we get to the first required (IsOptional=false) segment.</summary>
		public static HL7DefSegment GetSegDefByName(HL7DefMessage msgDef,SegmentNameHL7 segName,SegmentNameHL7 prevSegName) {
			HL7DefSegment segDef=null;
			for(int i=0;i<msgDef.hl7DefSegments.Count;i++) {//start with the second segment in the def since the first segment won't have a predecessor
				if(msgDef.hl7DefSegments[i].SegmentName!=segName) {
					continue;
				}
				//if prevSegName is Unkown, we don't care about the preceding segment so this segment must appear at most once in the message
				if(prevSegName==SegmentNameHL7.Unknown) {
					segDef=msgDef.hl7DefSegments[i];
					break;
				}
				//move backward in the list of segments looking for the nearest preceding, required segment
				for(int j=1;j<i+1;j++) {
					if(msgDef.hl7DefSegments[i-j].SegmentName==prevSegName) {
						segDef=msgDef.hl7DefSegments[i];
						break;
					}
					//if the nearest preceding, required (IsOptional=false) segment isn't prevSegName, this is not the repetition we're looking for, continue
					if(!msgDef.hl7DefSegments[i-j].IsOptional) {
						break;
					}
				}
				if(segDef!=null) {
					break;
				}
			}
			return segDef;
		}

		///<summary>listSegs will only contain more than one segment if the segment repeats.
		///prevSegName is only required when processing NTE segment(s) to determine which level NTE we are processing,
		///either a PID NTE, a OBR NTE, or a OBX NTE.</summary>
		public static void ProcessSeg(HL7DefMessage msgDef,List<SegmentHL7> listSegs,MessageHL7 msg,SegmentNameHL7 prevSegName=SegmentNameHL7.Unknown) {
			//if prevSegName=Unknown, segDefCur will be the first segDef with matching name
			HL7DefSegment segDefCur=GetSegDefByName(msgDef,listSegs[0].Name,prevSegName);
			switch(listSegs[0].Name) {
				case SegmentNameHL7.MSH://required segment
					if(segDefCur==null) {
						throw new Exception("Could not process the MedLab HL7 message.  "
							+"The message definition for this type of message did not contain a MSH segment definition.");
					}
					ProcessMSH(segDefCur,listSegs[0],msg);//listSegs will only contain one segment, the MSH segment
					return;
				case SegmentNameHL7.NK1://optional segment
					//NK1 is currently not processed
					if(segDefCur==null) {//do not process the NK1 segment if it's not defined
						return;
					}
					ProcessNK1(segDefCur,listSegs);//Currently does nothing
					return;
				case SegmentNameHL7.NTE://optional segment
					if(segDefCur==null) {//do not process the NTE segment if it's not defined
						return;
					}
					ProcessNTE(segDefCur,listSegs,msg);
					return;
				case SegmentNameHL7.OBR://required segment
					if(segDefCur==null) {
						throw new Exception("Could not process the MedLab HL7 message.  "
							+"The message definition for this type of message did not contain a OBR segment definition.");
					}
					ProcessOBR(segDefCur,listSegs[0],msg);
					return;
				case SegmentNameHL7.OBX://required segment
					if(segDefCur==null) {
						throw new Exception("Could not process the MedLab HL7 message.  "
							+"The message definition for this type of message did not contain a OBX segment definition.");
					}
					//instantiates a new MedLabResult object and sets the MedLabNum FK to the _medLabCur.MedLabNum
					ProcessOBX(segDefCur,listSegs[0]);//listSegs will only contain one OBX segment, not repeatable and not optional
					return;
				case SegmentNameHL7.ORC://required segment
					if(segDefCur==null) {
						throw new Exception("Could not process the MedLab HL7 message.  "
							+"The message definition for this type of message did not contain a ORC segment definition.");
					}
					ProcessORC(segDefCur,listSegs[0]);
					return;
				case SegmentNameHL7.PID://required segment
					if(segDefCur==null) {
						throw new Exception("Could not process the MedLab HL7 message.  "
							+"The message definition for this type of message did not contain a PID segment definition.");
					}
					ProcessPID(segDefCur,listSegs[0]);//listSegs will only contain one segment, the PID segment
					return;
				case SegmentNameHL7.SPM://optional segment
					if(segDefCur==null) {//do not process the SPM segment if it's not defined
						return;
					}
					ProcessSPM(segDefCur,listSegs);
					return;
				case SegmentNameHL7.ZEF://optional segment, LabCorp proprietary segment, not offical HL7 spec segment
					if(segDefCur==null) {//do not process the ZEF segment if it's not defined
						return;
					}
					ProcessZEF(segDefCur,listSegs);
					return;
				case SegmentNameHL7.ZPS://required segment
					if(segDefCur==null) {
						throw new Exception("Could not process the MedLab HL7 message.  "
							+"The message definition for this type of message did not contain a ZPS segment definition.");
					}
					ProcessZPS(segDefCur,listSegs,msg);
					return;
				default:
					return;
			}
		}

		///<summary>This will insert a new MedLab object and set _medLabCur.</summary>
		public static void ProcessMSH(HL7DefSegment segDef,SegmentHL7 mshSeg,MessageHL7 msg) {
			string firstObrClinicalInfo="";
			if(_medLabCur!=null) {
				//if _medLabCur already exists and we are about to start another ORC/OBR group, save the changes to _medLabCur before instantiating a new one
				MedLabs.Update(_medLabCur);
				firstObrClinicalInfo=_medLabCur.ClinicalInfo;//this is populated with processing the OBR segment of the first OBR in the message
			}
			_medLabCur=new MedLab();
			_medLabCur.FileName=_msgArchiveFilePath;
			_medLabCur.ClinicalInfo=firstObrClinicalInfo;//the clinical information from the first OBR, will be on each report generated by this message
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "messageControlId":
						msg.ControlId=mshSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "sendingApp":
						_medLabCur.SendingApp=mshSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "sendingFacility":
						_medLabCur.SendingFacility=mshSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					default:
						continue;
				}
			}
			_medLabCur.MedLabNum=MedLabs.Insert(_medLabCur);
			_medLabNumList.Add(_medLabCur.MedLabNum);
		}

		///<summary>Not currently processing the NK1 segment.</summary>
		public static void ProcessNK1(HL7DefSegment segDef,List<SegmentHL7> listSegs) {
			return;
		}

		///<summary>The segDef contains the field name that will identify which NTE segment we're processing.  It could be a PID note, an OBR/ORC note,
		///or an OBX note.  The segDef field is named accordingly.</summary>
		public static void ProcessNTE(HL7DefSegment segDef,List<SegmentHL7> listSegs,MessageHL7 msg) {
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "patNote":
						for(int j=0;j<listSegs.Count;j++) {
							if(!string.IsNullOrEmpty(_medLabCur.NotePat)) {
								_medLabCur.NotePat+="\r\n";
							}
							_medLabCur.NotePat+=listSegs[j].GetFieldComponent(fieldDefCur.OrdinalPos);
						}
						continue;
					case "labNote":
						for(int j=0;j<listSegs.Count;j++) {
							if(!string.IsNullOrEmpty(_medLabCur.NoteLab)) {
								_medLabCur.NoteLab+="\r\n";
							}
							_medLabCur.NoteLab+=listSegs[j].GetFieldComponent(fieldDefCur.OrdinalPos);
						}
						continue;
					case "obsNote":
						for(int j=0;j<listSegs.Count;j++) {
							if(!string.IsNullOrEmpty(_medLabResultCur.Note)) {
								_medLabResultCur.Note+="\r\n";
							}
							_medLabResultCur.Note+=listSegs[j].GetFieldComponent(fieldDefCur.OrdinalPos);
						}
						if(_isObsValueNte) {
							_medLabResultCur.ObsValue+=_medLabResultCur.Note;
							_medLabResultCur.Note="";
						}
						continue;
					default:
						continue;
				}
			}
		}

		///<summary></summary>
		public static void ProcessOBR(HL7DefSegment segDef,SegmentHL7 obrSeg,MessageHL7 obrMsg) {
			char subcompChar='&';
			//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default &
			if(obrMsg.Delimiters!=null && obrMsg.Delimiters.Length>3) {
				subcompChar=obrMsg.Delimiters[3];
			}
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				FieldHL7 fieldCur=obrSeg.GetField(fieldDefCur.OrdinalPos);//fieldCur can be null
				if(fieldCur==null) {
					continue;
				}
				switch(fieldDefCur.FieldName) {
					case "clinicalInfo":
						if(_isFirstObr) {
							_medLabCur.ClinicalInfo=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos).Trim();
						}
						continue;
					case "dateTimeCollected":
						_medLabCur.DateTimeCollected=FieldParserMedLab.DateTimeParse(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "dateTimeEntered":
						_medLabCur.DateTimeEntered=FieldParserMedLab.DateTimeParse(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "dateTimeReported":
						_medLabCur.DateTimeReported=FieldParserMedLab.DateTimeParse(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "facilityID":
						string facIdCur=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						if(facIdCur=="") {
							continue;
						}
						if(!_dictFacilityCodeNum.ContainsKey(facIdCur)) {//no ZPS segments with this footnote ID
							continue;
						}
						List<long> facNumList=_dictFacilityCodeNum[facIdCur];//should only be one facility with this footnote ID in this message
						for(int j=0;j<facNumList.Count;j++) {
							MedLabFacAttach medLabAttachCur=new MedLabFacAttach();
							medLabAttachCur.MedLabFacilityNum=facNumList[j];
							medLabAttachCur.MedLabNum=_medLabCur.MedLabNum;
							MedLabFacAttaches.Insert(medLabAttachCur);
						}
						continue;
					case "obsTestID":
						if(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos,2).ToLower()=="l") {
							_medLabCur.ObsTestID=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
							_medLabCur.ObsTestDescript=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1);
						}
						if(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos,5).ToLower()=="ln") {
							_medLabCur.ObsTestLoinc=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos,3);
							_medLabCur.ObsTestLoincText=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos,4);
						}
						continue;
					case "orderingProv":
						if(_medLabCur.OrderingProvNPI==null || _medLabCur.OrderingProvNPI=="") {//may be filled from the ORC segment
							_medLabCur.OrderingProvNPI=FieldParserMedLab.OrderingProvIDParse(fieldCur,"N");
						}
						if(_medLabCur.OrderingProvLocalID==null || _medLabCur.OrderingProvLocalID=="") {//may be filled from the ORC segment
							_medLabCur.OrderingProvLocalID=FieldParserMedLab.OrderingProvIDParse(fieldCur,"L");
						}
						int k=0;
						if(_medLabCur.OrderingProvLName==null || _medLabCur.OrderingProvLName=="") {//may be filled from the ORC segment
							_medLabCur.OrderingProvLName=fieldCur.GetComponentVal(1).Trim();
							while(_medLabCur.OrderingProvLName=="" && k<fieldCur.ListRepeatFields.Count) {//if LName is not present in first repetition check others
								_medLabCur.OrderingProvLName=fieldCur.ListRepeatFields[k].GetComponentVal(1).Trim();
								k++;
							}
						}
						k=0;
						if(_medLabCur.OrderingProvFName==null || _medLabCur.OrderingProvFName=="") {//may be filled from the ORC segment
							_medLabCur.OrderingProvFName=fieldCur.GetComponentVal(2).Trim();
							while(_medLabCur.OrderingProvFName=="" && k<fieldCur.ListRepeatFields.Count) {//if FName is not present in first repetition check others
								_medLabCur.OrderingProvFName=fieldCur.ListRepeatFields[k].GetComponentVal(2).Trim();
								k++;
							}
						}
						#region Locate Provider
						if(_medLabCur.ProvNum!=0) {//may have located the provider from the ORC segment, nothing left to do
							continue;
						}
						_medicaidIdCur=FieldParserMedLab.OrderingProvIDParse(fieldCur,"P");
						List<Provider> listProvs=Providers.GetProvsByNpiOrMedicaidId(_medLabCur.OrderingProvNPI,_medicaidIdCur);
						listProvs.Sort(SortByNpiMedicaidIdMatch);
						if(listProvs.Count>0) {//if a provider with either a matching NPI or Medicaid ID is found, use the first matching prov
							_medLabCur.ProvNum=listProvs[0].ProvNum;
						}
						else {//no provider match based on NPI or MedicaidID
							listProvs=Providers.GetProvsByFLName(_medLabCur.OrderingProvLName,_medLabCur.OrderingProvFName);//must have both LName and FName
							if(listProvs.Count>0) {//use the first provider found with matching LName and FName
								_medLabCur.ProvNum=listProvs[0].ProvNum;
							}
						}
						#endregion Locate Provider
						continue;
					case "parentObsID":
						_medLabCur.ParentObsID=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "parentObsTestID":
						_medLabCur.ParentObsTestID=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "resultStatus":
						_medLabCur.ResultStatus=FieldParserMedLab.ResultStatusParse(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "specimenAction":
						_medLabCur.ActionCode=FieldParserMedLab.ResultActionParse(obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "specimenID":
						if(_medLabCur.SpecimenID!=null && _medLabCur.SpecimenID!="") {//could be filled by value in ORC
							continue;
						}
						_medLabCur.SpecimenID=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "specimenIDAlt":
						_medLabCur.SpecimenIDAlt=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "specimenIDFiller":
						if(_medLabCur.SpecimenIDFiller!=null && _medLabCur.SpecimenIDFiller!="") {//could be filled by value in ORC
							continue;
						}
						_medLabCur.SpecimenIDFiller=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "totalVolume":
						_medLabCur.TotalVolume=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						if(_medLabCur.TotalVolume=="") {
							continue;
						}
						string[] unitsSubComp=obrSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1).Split(new char[] { subcompChar },StringSplitOptions.None);
						if(unitsSubComp==null || unitsSubComp.Length==0 || unitsSubComp[0]=="") {
							_medLabCur.TotalVolume+=" ml";
							continue;
						}
						_medLabCur.TotalVolume+=" "+unitsSubComp[0];
						continue;
					default:
						continue;
				}
			}
			_isFirstObr=false;
			return;
		}

		///<summary>This will insert a new MedLabResult object and set _medLabResultCur.</summary>
		public static void ProcessOBX(HL7DefSegment segDef,SegmentHL7 obxSeg) {
			if(_medLabResultCur!=null) {
				//if _medLabResultCur already exists and we're about to start another OBX, save the changes to _medLabResultCur before instantiating a new one
				MedLabResults.Update(_medLabResultCur);
			}
			_medLabResultCur=new MedLabResult();
			_medLabResultCur.MedLabNum=_medLabCur.MedLabNum;
			_isObsValueNte=false;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "obsID":
						_medLabResultCur.ObsID=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						_medLabResultCur.ObsText=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1);
						_medLabResultCur.ObsLoinc=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos,3);
						_medLabResultCur.ObsLoincText=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos,4);
						continue;
					case "obsIDSub":
						_medLabResultCur.ObsIDSub=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "obsValue":
						_medLabResultCur.ObsValue=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						if(_medLabResultCur.ObsValue.ToLower()=="tnp") {
							_medLabResultCur.ObsValue="Test Not Performed";
						}
						DataSubtype dataSubType;
						try {
							dataSubType=(DataSubtype)Enum.Parse(typeof(DataSubtype),obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos,2),true);
						}
						catch(Exception ex){
							ex.DoNothing();
							dataSubType=DataSubtype.Unknown;
						}
						_medLabResultCur.ObsSubType=dataSubType;
						continue;
					case "obsValueType":
						if(obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos).ToLower()=="tx") {//if value type is text (TX) the value will be in attached NTEs
							_isObsValueNte=true;
						}
						continue;
					case "obsUnits":
						if(_medLabResultCur.ObsValue.ToLower()=="test not performed") {//if TNP then we won't bother saving the units
							continue;
						}
						_medLabResultCur.ObsUnits=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						if(obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1).Length>0) {
							if(_medLabResultCur.Note!=null && _medLabResultCur.Note.Length>0) {
								_medLabResultCur.Note+="\r\n";
							}
							_medLabResultCur.Note+="Units full text: "+obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1);
						}
						continue;
					case "obsRefRange":
						_medLabResultCur.ReferenceRange=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "obsAbnormalFlag":
						_medLabResultCur.AbnormalFlag=FieldParserMedLab.AbnormalFlagParse(obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "resultStatus":
						_medLabResultCur.ResultStatus=FieldParserMedLab.ResultStatusParse(obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "dateTimeObs":
						_medLabResultCur.DateTimeObs=FieldParserMedLab.DateTimeParse(obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos));
						continue;
					case "facilityID":
						_medLabResultCur.FacilityID=obxSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					default:
						continue;
				}
			}
			_medLabResultCur.MedLabResultNum=MedLabResults.Insert(_medLabResultCur);
			if(_medLabResultCur.FacilityID=="") {//no facility ID in the OBX segment, can't link to facilities in the ZPS segments
				return;
			}
			if(!_dictFacilityCodeNum.ContainsKey(_medLabResultCur.FacilityID)) {//no facility in the ZPS segments with this facility ID
				return;
			}
			List<long> facNumList=_dictFacilityCodeNum[_medLabResultCur.FacilityID];//should only be one facility in the ZPS segments with this ID
			for(int i=0;i<facNumList.Count;i++) {
				MedLabFacAttach medLabAttachCur=new MedLabFacAttach();
				medLabAttachCur.MedLabFacilityNum=facNumList[i];
				medLabAttachCur.MedLabResultNum=_medLabResultCur.MedLabResultNum;
				MedLabFacAttaches.Insert(medLabAttachCur);
			}
		}

		///<summary></summary>
		public static void ProcessORC(HL7DefSegment segDef,SegmentHL7 orcSeg) {
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "orderingProv":
						FieldHL7 fieldCur=orcSeg.GetField(fieldDefCur.OrdinalPos);//fieldCur can be null
						if(fieldCur==null) {
							continue;
						}
						_medLabCur.OrderingProvNPI=FieldParserMedLab.OrderingProvIDParse(fieldCur,"N");
						_medLabCur.OrderingProvLocalID=FieldParserMedLab.OrderingProvIDParse(fieldCur,"L");
						_medLabCur.OrderingProvLName=fieldCur.GetComponentVal(1).Trim();
						_medLabCur.OrderingProvFName=fieldCur.GetComponentVal(2).Trim();
						int j=0;
						while(_medLabCur.OrderingProvLName=="" && j<fieldCur.ListRepeatFields.Count) {//if LName is not present in first repetition, check others
							_medLabCur.OrderingProvLName=fieldCur.ListRepeatFields[j].GetComponentVal(1).Trim();
							j++;
						}
						j=0;
						while(_medLabCur.OrderingProvFName=="" && j<fieldCur.ListRepeatFields.Count) {//if FName is not present in first repetition, check others
							_medLabCur.OrderingProvLName=fieldCur.ListRepeatFields[j].GetComponentVal(2).Trim();
							j++;
						}
						#region Locate Provider
						_medicaidIdCur=FieldParserMedLab.OrderingProvIDParse(fieldCur,"P");
						List<Provider> listProvs=Providers.GetProvsByNpiOrMedicaidId(_medLabCur.OrderingProvNPI,_medicaidIdCur);
						listProvs.Sort(SortByNpiMedicaidIdMatch);
						if(listProvs.Count>0) {//if a provider with either a matching NPI or Medicaid ID is found, use the first matching prov
							_medLabCur.ProvNum=listProvs[0].ProvNum;
							continue;
						}
						//no provider match based on NPI or MedicaidID
						listProvs=Providers.GetProvsByFLName(_medLabCur.OrderingProvLName,_medLabCur.OrderingProvFName);//must have both LName and FName
						if(listProvs.Count>0) {//use the first provider found with matching LName and FName
							_medLabCur.ProvNum=listProvs[0].ProvNum;
						}
						#endregion Locate Provider
						continue;
					case "specimenID":
						if(_medLabCur.SpecimenID!=null && _medLabCur.SpecimenID!="") {//could be filled in by value in OBR
							continue;
						}
						_medLabCur.SpecimenID=orcSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "specimenIDFiller":
						if(_medLabCur.SpecimenIDFiller!=null && _medLabCur.SpecimenIDFiller!="") {//could be filled in by value in OBR
							continue;
						}
						_medLabCur.SpecimenIDFiller=orcSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					default:
						continue;
				}
			}
			return;
		}

		///<summary>Sets values on _medLabCur based on the PID segment fields.  Also sets medlab.OriginalPIDSegment to the full PID segment.</summary>
		public static void ProcessPID(HL7DefSegment segDef,SegmentHL7 pidSeg) {
			_medLabCur.OriginalPIDSegment=pidSeg.ToString();
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "accountNum":
						_medLabCur.PatAccountNum=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						string fastingStr=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos,6);
						//if the component is blank or has a value other than "Y" or "N", the PatFasting field will be 0 - Unknown
						if(!string.IsNullOrEmpty(fastingStr)) {
							fastingStr=fastingStr.ToLower().Substring(0,1);
							if(fastingStr=="y") {
								_medLabCur.PatFasting=YN.Yes;
							}
							else if(fastingStr=="n") {
								_medLabCur.PatFasting=YN.No;
							}
						}
						continue;
					case "altPatID":
						_medLabCur.PatIDAlt=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "labPatID":
						_medLabCur.PatIDLab=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos);
						continue;
					case "pat.nameLFM":
						//not currently using this to update any db fields, only used to validate the correct patient was selected
						continue;
					case "pat.PatNum":
						if(_patCur!=null) {
							//If a patient was not located, the MedLab object will have a 0 for PatNum.
							//We will be able to generate a list of all 'unattached' MedLab objects so the user can manually select the correct patient.
							_medLabCur.PatNum=_patCur.PatNum;
						}
						continue;
					case "patBirthdateAge":
						//the birthday field for MedLab is birthdate^age in years^age months^age days.
						//The year component is left padded with 0's to three digits, the month and day components are left padded with 0's to two digits.
						//If the year, age, or month components don't exist or are blank, the default will be to 
						//Example: 19811213^033^02^19
						FieldHL7 fieldCur=pidSeg.GetField(fieldDefCur.OrdinalPos);
						if(fieldCur==null || fieldCur.Components.Count<4) {//if there aren't even 4 components (3 ^'s) in the field, don't set the age
							continue;
						}
						_medLabCur.PatAge=pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos,1).PadLeft(3,'0')+"/"
							+pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos,2).PadLeft(2,'0')+"/"
							+pidSeg.GetFieldComponent(fieldDefCur.OrdinalPos,3).PadLeft(2,'0');
						continue;
					default:
						continue;
				}
			}
			return;
		}

		///<summary></summary>
		public static void ProcessSPM(HL7DefSegment segDef,List<SegmentHL7> listSegs) {
			int specIdIndex=-1;
			int specDescIndex=-1;
			int dateTSpecIndex=-1;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {//get indexes for fields from the segment def
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "specimenID":
						specIdIndex=fieldDefCur.OrdinalPos;
						continue;
					case "specimenDescript":
						specDescIndex=fieldDefCur.OrdinalPos;
						continue;
					case "dateTimeSpecimen":
						dateTSpecIndex=fieldDefCur.OrdinalPos;
						continue;
					default:
						continue;
				}
			}
			for(int i=0;i<listSegs.Count;i++) {
				MedLabSpecimen specimenCur=new MedLabSpecimen();
				specimenCur.MedLabNum=_medLabCur.MedLabNum;
				if(specIdIndex>=0) {
					specimenCur.SpecimenID=listSegs[i].GetFieldComponent(specIdIndex);
				}
				if(specDescIndex>=0) {
					specimenCur.SpecimenDescript=listSegs[i].GetFieldComponent(specDescIndex);
				}
				if(dateTSpecIndex>=0) {
					specimenCur.DateTimeCollected=FieldParserMedLab.DateTimeParse(listSegs[i].GetFieldComponent(dateTSpecIndex));
				}
				MedLabSpecimens.Insert(specimenCur);
			}
			return;
		}

		///<summary>Appends all base64 fields from all repetitions of the ZEF segments in this repetition of the observation result group into a single
		///base64 text version of the PDF.  Then converts the base64 string into a PDF file and stores it in the patients image folder.  This will use
		///the image category stored in the def, or if not set in the def it will use the first image category in the list.  An entry is then made in
		///the document table and the DocNum for the imported PDF is stored in the MedLabResult.DocNum field.</summary>
		public static void ProcessZEF(HL7DefSegment segDef,List<SegmentHL7> listSegs) {
			//first make sure the list of segments is complete by ordering them by sequence number, ZEF.1, and making sure there are no gaps in the sequence
			int sequenceNumIndex=-1;
			int base64TextIndex=-1;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				if(segDef.hl7DefFields[i].FieldName=="sequenceNum") {
					sequenceNumIndex=segDef.hl7DefFields[i].OrdinalPos;
					continue;
				}
				if(segDef.hl7DefFields[i].FieldName=="base64File") {
					base64TextIndex=segDef.hl7DefFields[i].OrdinalPos;
					continue;
				}
			}
			if(listSegs.Count>1 && sequenceNumIndex<0) {//cannot determine the order of the segs without a sequenceNum field, so don't process the ZEFs
				EventLog.WriteEntry("MedLabHL7","The ZEF segment definition in the enabled MedLab HL7 definition does not contain a sequenceNum field.  "
					+"Since the segments cannot be ordered without the sequenceNum field, the ZEF segments were not processed.",EventLogEntryType.Information);
				return;
			}
			if(base64TextIndex<0) {//cannot process the ZEF segs if there is no base64File field to process
				EventLog.WriteEntry("MedLabHL7","The ZEF segment definition in the enabled MedLab HL7 definition does not contain a base64File field.  "
					+"Since the PDF file is generated from the base64 text, the ZEF segments were not processed.",EventLogEntryType.Information);
				return;
			}
			foreach(SegmentHL7 seg in listSegs) {
				seg.SequenceNumIndex=sequenceNumIndex;
				//the seq num is a field in the seg that we attempt to parse to an int, but parsing may fail and the seq num will be -1
				if(listSegs.Count>1 && seg.SequenceNum<0) {
					EventLog.WriteEntry("MedLabHL7","A ZEF segment had a sequence number that was invalid or not present.  "
						+"The ZEF segments were not processed and the embedded PDF was not created.");
					return;
				}
			}
			if(listSegs.Count>1) {//only sort the segs and validate the sequential order of the sequence numbers if there are more than one ZEF segment
				//sort the segments by sequence number
				try {
					listSegs.Sort(SortSegsBySeqNum);
				}
				catch(Exception ex) {
					EventLog.WriteEntry("MedLabHL7",ex.Message,EventLogEntryType.Information);
					return;
				}
			}
			//Loop through ZEF segments and make sure the sequence numbers are sequential, no segments are missing
			//Append all repetitions together into a single base64 text string to generate the PDF from
			StringBuilder sb=new StringBuilder();
			for(int i=0;i<listSegs.Count;i++) {
				sb.Append(listSegs[i].GetFieldComponent(base64TextIndex));
				if(i==0) {
					continue;
				}
				//If only one ZEF segment the following if statement won't run
				if(listSegs[i].SequenceNum!=listSegs[i-1].SequenceNum+1) {//i>=1, so index i-1 is guaranteed to exist
					//if the current sequence number is not the previous sequence number plus 1, there must be a missing ZEF segment
					EventLog.WriteEntry("MedLabHL7","The MedLab HL7 message was missing ZEF segment(s) based on missing sequence numbers.  "
						+"The ZEF segments were not processed and the embedded PDF was not created.");
					return;
				}
			}
			//If we reach this point, the segments were ordered and no sequence numbers were missing and the base64 text has been appended
			//to the string builder sb, now convert to a PDF and save.
			//If storing images in the db, save the base64 PDF in the document table with PatNum=0 if no pat was found or PatNum=_patCur.PatNum if found.
			//Save PDF in the image folder in the MedLabEmbeddedFiles directory or in the document table if storing images in the db
			//If a pat has been located, move the embedded PDF into the pat's image folder or assign document.PatNum=PatCur.PatNum if storing images in db
			//If a pat has not been located, move the PDF into the MedLabEmbeddedFiles\Unassigned dir or leave document.PatNum=0 if storing images in db
			Document doc=new Document();
			doc.FileName=".pdf";
			doc.ImgType=ImageType.Document;
			doc.DateCreated=DateTime.Now;
			doc.DocCategory=Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum;//put it in the first category
			if(_defCur.LabResultImageCat>0) {//if category is set for the def, use that image category
				doc.DocCategory=_defCur.LabResultImageCat;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {//saving to db
				doc.RawBase64=sb.ToString();
				if(_patCur==null) {
					//doc.PatNum will be 0 until the user manually attaches to a patient
					doc.DocNum=Documents.Insert(doc);//filename will remain ".pdf" until the user manually attaches to a patient
				}
				else {
					doc.PatNum=_patCur.PatNum;
					doc.DocNum=Documents.Insert(doc,_patCur);//this assigns a filename and saves to db
				}
			}
			else {//Using AtoZ folder (or Cloud)--------------------------------------------------------------------
				string embeddedFile="";
				try {
					string embeddedFilePath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"MedLabEmbeddedFiles");
					if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(embeddedFilePath)) {
						Directory.CreateDirectory(embeddedFilePath);
					}
					else {//Cloud, create random temp folder
						embeddedFilePath=PrefC.GetTempFolderPath();
					}
					embeddedFile=ODFileUtils.CreateRandomFile(embeddedFilePath,".pdf");
					byte[] byteArray=Convert.FromBase64String(sb.ToString());
					File.WriteAllBytes(embeddedFile,byteArray);
					if(_patCur==null) {
						doc.FileName=Path.GetFileName(embeddedFile);
						doc.DocNum=Documents.Insert(doc);//PatNum will be 0, this will indicate that we should look in the MedLabEmbeddedFiles folder for the PDF
					}
					else {
						//This will upload to Dropbox from the temp file created above if using dropbox.
						doc=ImageStore.Import(embeddedFile,doc.DocCategory,_patCur);
						try {
							File.Delete(embeddedFile);//Clean up the temp file, only one copy will exist in the patient's image folder
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, file could be in use or there is not sufficient permissions, just leave it in the image folder as unassigned
						}
					}
				}
				catch(Exception ex) {
					Documents.Delete(doc);//delete the doc since the MedLabResult will not be pointing to it
					//The file may exist in the MedLabEmbeddedFiles directory, but no OD object will be referencing it.  It would have to be recreated from the
					//HL7 message or manually imported into the patient's image folder.
					EventLog.WriteEntry("MedLabHL7","Error saving the embedded PDF when processing a MedLab HL7 message.\r\nThe PDF located here "+embeddedFile
						+" will need to be manually imported into the correct patient's image folder.\r\n"+ex.Message,EventLogEntryType.Information);
					return;
				}
			}
			_medLabResultCur.DocNum=doc.DocNum;
		}

		///<summary>Inserts any MedLabFacility objects not in the database.  Creates a dictionary linking the facilityIDs to a list of MedLabFacilityNums.
		///For each MedLab and MedLabResult in the message, OBR.24 and OBX.15, respectively, will contain the facilityID for the facility where the lab
		///was performed.  Using the facilityID, the orders and results will be attached to facilities via entries in the MedLabAttach table.</summary>
		public static void ProcessZPS(HL7DefSegment segDef,List<SegmentHL7> listSegs,MessageHL7 msg) {
			_dictFacilityCodeNum=new Dictionary<string,List<long>>();
			int facIdIndex=-1;
			int facNameIndex=-1;
			int facAddrIndex=-1;
			int facPhIndex=-1;
			int facDirIndex=-1;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {//get indexes for fields from the segment def
				HL7DefField fieldDefCur=segDef.hl7DefFields[i];
				switch(fieldDefCur.FieldName) {
					case "facilityID":
						facIdIndex=fieldDefCur.OrdinalPos;
						continue;
					case "facilityName":
						facNameIndex=fieldDefCur.OrdinalPos;
						continue;
					case "facilityAddress":
						facAddrIndex=fieldDefCur.OrdinalPos;
						continue;
					case "facilityPhone":
						facPhIndex=fieldDefCur.OrdinalPos;
						continue;
					case "facilityDirector":
						facDirIndex=fieldDefCur.OrdinalPos;
						continue;
					default:
						continue;
				}
			}
			if(facIdIndex<0 || facNameIndex<0 || facAddrIndex<0) {
				EventLog.WriteEntry("MessageParserMedLab","The MedLab HL7 definition does not contain a field definition for facilityID, facilityName, or "
					+"facilityAddress with a valid item order.  There will not be any MedLabFacility objects created and linked to the MedLabResult objects.",
					EventLogEntryType.Warning);
				return;
			}
			for(int i=0;i<listSegs.Count;i++) {
				MedLabFacility facilityCur=new MedLabFacility();
				facilityCur.FacilityName=listSegs[i].GetFieldComponent(facNameIndex);
				if(facAddrIndex>=0) {
					facilityCur.Address=listSegs[i].GetFieldComponent(facAddrIndex,0);
					facilityCur.City=listSegs[i].GetFieldComponent(facAddrIndex,2);
					facilityCur.State=listSegs[i].GetFieldComponent(facAddrIndex,3);
					facilityCur.Zip=listSegs[i].GetFieldComponent(facAddrIndex,4);
				}
				if(facPhIndex>=0) {
					facilityCur.Phone=listSegs[i].GetFieldComponent(facPhIndex);
				}
				if(facDirIndex>=0) {
					facilityCur.DirectorTitle=listSegs[i].GetFieldComponent(facDirIndex,0);
					facilityCur.DirectorLName=listSegs[i].GetFieldComponent(facDirIndex,1);
					facilityCur.DirectorFName=listSegs[i].GetFieldComponent(facDirIndex,2);
				}
				facilityCur.MedLabFacilityNum=MedLabFacilities.InsertIfNotInDb(facilityCur);
				string segFacId=listSegs[i].GetFieldComponent(facIdIndex);
				if(!_dictFacilityCodeNum.ContainsKey(segFacId)) {//if the footnote id doesn't exist in the dictionary, add it and the MedLabFacilityNum value
					_dictFacilityCodeNum.Add(segFacId,new List<long>() { facilityCur.MedLabFacilityNum });
					continue;
				}
				if(!_dictFacilityCodeNum[segFacId].Contains(facilityCur.MedLabFacilityNum)) {//if the id is not linked to this facilitynum, add it to the list
					_dictFacilityCodeNum[segFacId].Add(facilityCur.MedLabFacilityNum);
				}
				//dictionary contains the footnote id key and the list value for that key contains this MedLabFacilityNum
			}
			return;
		}

		///<summary>Sort by the sequence num located in the field at index _sequenceNumIndex.</summary>
		public static int SortSegsBySeqNum(SegmentHL7 segX,SegmentHL7 segY) {
			return segX.SequenceNum.CompareTo(segY.SequenceNum);
		}

		///<summary>A prov with both NPI and Medicaid ID matching those in the message will come before a prov with only one ID matching.  Those with only
		///NPI matching will come before those with only Medicaid ID matching.  The provs compared should have at least one matching ID.  If there are
		///multiple provs with the same matching IDs, those with matching LName and FName will come before those with matching LName only which will come
		///before those with no name match.</summary>
		public static int SortByNpiMedicaidIdMatch(Provider provX,Provider provY) {
			int pXVal=0;
			int pYVal=0;
			//matching NPI is +2, matching MedicaidId is +1, if both match the value will be 3
			if(provX.NationalProvID!="" && provX.NationalProvID.Trim().ToLower()==_medLabCur.OrderingProvNPI.ToLower()) {
				pXVal+=2;
			}
			if(provX.MedicaidID!="" && provX.MedicaidID.Trim().ToLower()==_medicaidIdCur.ToLower()) {
				pXVal+=1;
			}
			if(provY.NationalProvID!="" && provY.NationalProvID.Trim().ToLower()==_medLabCur.OrderingProvNPI.ToLower()) {
				pYVal+=2;
			}
			if(provY.MedicaidID!="" && provY.MedicaidID.Trim().ToLower()==_medicaidIdCur.ToLower()) {
				pYVal+=1;
			}
			//prov with the greatest value comes first
			if(pXVal>pYVal) {
				return -1;
			}
			if(pYVal>pXVal) {
				return 1;
			}
			//if values are the same based on matching NPI and Medicaid ID, compare LName and FName
			//Matching LName and FName +2, Matching LName only +1, LName is not a match +0 (FName match only +0)
			if(provX.LName.Trim().ToLower()==_medLabCur.OrderingProvLName.ToLower()) {
				pXVal+=1;
				if(provX.FName.Trim().ToLower()==_medLabCur.OrderingProvFName.ToLower()) {
					pXVal+=1;
				}
			}
			if(provY.LName.Trim().ToLower()==_medLabCur.OrderingProvLName.ToLower()) {
				pYVal+=1;
				if(provY.FName.Trim().ToLower()==_medLabCur.OrderingProvFName.ToLower()) {
					pYVal+=1;
				}
			}
			//prov with the greatest value comes first
			if(pXVal>pYVal) {
				return -1;
			}
			if(pYVal>pXVal) {
				return 1;
			}
			//values must be the same
			return 0;
		}

	}
}
