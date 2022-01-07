using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	///<summary>This is the engine that will parse our incoming HL7 messages.</summary>
	public class MessageParser {
		private static bool _isNewPat;
		private static bool _isVerboseLogging;
		private static HL7Msg _hl7MsgCur;
		private static bool _isEcwHL7Def=false;//set to true if enabled def is internal type eCWTight, eCWFull, or eCWStandalone
		private static Appointment _aptProcessed;//return value for ProcessSeg if an appointment was processed
		//Open \\SERVERFILES\storage\OPEN DENTAL\Programmers Documents\Standards (X12, ADA, etc)\HL7\Version2.6\V26_CH02_Control_M4_JAN2007.doc
		//At the top of page 33, there are rules for the recipient.
		//Basically, they state that parsing should not fail just because there are extra unexpected items.
		//And parsing should also not fail if expected items are missing.

		public static void Process(MessageHL7 msg,bool isVerboseLogging) {
			_hl7MsgCur=new HL7Msg();
			_hl7MsgCur.HL7Status=HL7MessageStatus.InFailed;//it will be marked InProcessed once data is inserted.
			_hl7MsgCur.MsgText=msg.ToString();
			_hl7MsgCur.PatNum=0;
			_hl7MsgCur.AptNum=0;
			List<HL7Msg> listHL7Existing=HL7Msgs.GetOneExisting(_hl7MsgCur);
			if(listHL7Existing.Count>0) {//This message is already in the db
				_hl7MsgCur.HL7MsgNum=listHL7Existing[0].HL7MsgNum;
				HL7Msgs.UpdateDateTStamp(_hl7MsgCur);
				msg.ControlId=HL7Msgs.GetControlId(_hl7MsgCur);
				return;
			}
			else {
				//Insert as InFailed until processing is complete.  Update once complete, PatNum will have correct value, AptNum will have correct value if SIU message or 0 if ADT, and status changed to InProcessed
				HL7Msgs.Insert(_hl7MsgCur);
			}
			_isVerboseLogging=isVerboseLogging;
			_isNewPat=false;
			#region Get and Validate Message Definition for This Message
			HL7Def def=HL7Defs.GetOneDeepEnabled();
			if(def==null) {
				_hl7MsgCur.Note="Could not process HL7 message.  No HL7 definition is enabled.";
				HL7Msgs.Update(_hl7MsgCur);
				throw new Exception("Could not process HL7 message.  No HL7 definition is enabled.");
			}
			if(def.InternalType==HL7InternalType.eCWFull
				|| def.InternalType==HL7InternalType.eCWTight
				|| def.InternalType==HL7InternalType.eCWStandalone)
			{
				_isEcwHL7Def=true;
			}
			HL7DefMessage hl7defmsg=null;
			for(int i=0;i<def.hl7DefMessages.Count;i++) {
				if(def.hl7DefMessages[i].MessageType==msg.MsgType && def.hl7DefMessages[i].InOrOut==InOutHL7.Incoming) {//Ignoring event type for now, we will treat all ADT's and SIU's the same
					hl7defmsg=def.hl7DefMessages[i];
					break;
				}
			}
			if(hl7defmsg==null) {//No message definition matches this message's MessageType and is Incoming
				_hl7MsgCur.Note="Could not process HL7 message.  No definition for this type of message in the enabled HL7Def.";
				HL7Msgs.Update(_hl7MsgCur);
				throw new Exception("Could not process HL7 message.  No definition for this type of message in the enabled HL7Def.");
			}
			#endregion Get and Validate Message Definition for This Message
			string chartNum=null;
			long patNum=0;
			long patNumFromIds=0;
			//if we cannot locate the patient by the supplied patnum, either from the PatNum field or the list of IDs in PID.3, then we will use the external IDs to attempt to locate the pat
			//we will only trust these external IDs to return the correct patient if they all refer to a single patient
			//we may want to do some other checking to ensure that the patient referred to is the right patient?
			List<OIDExternal> listOids=new List<OIDExternal>();
			DateTime birthdate=DateTime.MinValue;
			string patLName=null;
			string patFName=null;
			#region GetPatientIDs
			#region PID segmentsLoop
			//Identify the location of the PID segment based on the message definition
			//Get patient in question, incoming messages must have a PID segment so use that to find the pat in question
			if(!msg.TryGetSegmentOrder(SegmentNameHL7.PID,hl7defmsg,out int pidOrder,out int pidDefOrder)) {
				_hl7MsgCur.Note="Could not process the HL7 message due to missing PID segment.";
				HL7Msgs.Update(_hl7MsgCur);
				throw new Exception("Could not process HL7 message.  Could not process the HL7 message due to missing PID segment.");
			}
			#endregion PID segmentsLoop
			#region PID fieldsLoop
			//Using the identified location of the PID segment, loop through the fields and find each identifier defined
			for(int f=0;f<hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields.Count;f++) {//Go through fields of PID segment and get patnum, chartnum, patient name, and/or birthdate to locate patient
				if(hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].FieldName=="pat.ChartNumber") {
					int chartNumOrdinal=hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].OrdinalPos;
					chartNum=msg.Segments[pidOrder].GetField(chartNumOrdinal).ToString();
				}
				else if(hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].FieldName=="pat.PatNum") {
					int patNumOrdinal=hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].OrdinalPos;
					try {
						patNum=PIn.Long(msg.Segments[pidOrder].GetField(patNumOrdinal).ToString());
					}
					catch(Exception ex) {
						ex.DoNothing();
						//do nothing, patNum will remain 0
					}
				}
				else if(hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].FieldName=="pat.birthdateTime") {
					int patBdayOrdinal=hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].OrdinalPos;
					birthdate=FieldParser.DateTimeParse(msg.Segments[pidOrder].GetField(patBdayOrdinal).ToString());
				}
				else if(hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].FieldName=="pat.nameLFM") {
					int patNameOrdinal=hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].OrdinalPos;
					patLName=msg.Segments[pidOrder].GetFieldComponent(patNameOrdinal,0);
					patFName=msg.Segments[pidOrder].GetFieldComponent(patNameOrdinal,1);
				}
				#region patientIdList
				else if(hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].FieldName=="patientIds") {
					//get the id with the assigning authority equal to the internal OID root stored in their database for a patient object
					string patOIDRoot=OIDInternals.GetForType(IdentifierType.Patient).IDRoot;
					if(patOIDRoot=="") {
						//if they have not set their internal OID root, we cannot identify which repetition in this field contains the OD PatNum
						continue;
					}
					int patIdListOrdinal=hl7defmsg.hl7DefSegments[pidDefOrder].hl7DefFields[f].OrdinalPos;
					FieldHL7 fieldPatIds=msg.Segments[pidOrder].GetField(patIdListOrdinal);
					//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&OIDType^MR|
					//field component values will be the first repetition, repeats will be in field.ListRepeatFields
					//field 4 is an HD data type, which is composed of 3 subcomponents separated by the "&".
					//subcomponent 1 is the universal ID for the assigning authority
					//subcomponent 2 is the universal ID type for the assigning authority
					char subcompSeparator='&';
					if(msg.Delimiters.Length>3) {
						subcompSeparator=msg.Delimiters[3];
					}
					string[] arrayPatIdSubComps=fieldPatIds.GetComponentVal(3).Split(new char[] { subcompSeparator },StringSplitOptions.None);
					if(arrayPatIdSubComps.Length>1//there must be at least 2 sub-components in the assigning authority component so we can identify whose ID this is
						&& fieldPatIds.GetComponentVal(4).ToLower()=="pi")//PI=patient internal identifier; a number that is unique to a patient within an assigning authority
					{
						int checkDigit=-1;
						try {
							checkDigit=PIn.Int(fieldPatIds.GetComponentVal(1));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//checkDigit will remain -1
						}
						//if using the M10 or M11 check digit algorithm and it passes the respective test, or not using either algorithm, then use the ID
						if((fieldPatIds.GetComponentVal(2).ToLower()=="m10"
								&& checkDigit!=-1
								&& M10CheckDigit(fieldPatIds.GetComponentVal(0))==checkDigit)//using M10 scheme and the check digit is valid and matches calc
							|| (fieldPatIds.GetComponentVal(2).ToLower()=="m11"
								&& checkDigit!=-1
								&& M11CheckDigit(fieldPatIds.GetComponentVal(0))==checkDigit)//using M11 scheme and the check digit is valid and matches calc
							|| (fieldPatIds.GetComponentVal(2).ToLower()!="m10"
								&& fieldPatIds.GetComponentVal(2).ToLower()!="m11"))//not using either check digit scheme
						{
							if(arrayPatIdSubComps[1].ToLower()==patOIDRoot.ToLower()) {
								try {
									patNumFromIds=PIn.Long(fieldPatIds.GetComponentVal(0));
								}
								catch(Exception ex) {
									ex.DoNothing();
									//do nothing, patNumFromList will remain 0
								}
							}
							else {
								OIDExternal oidCur=new OIDExternal();
								oidCur.IDType=IdentifierType.Patient;
								oidCur.IDExternal=fieldPatIds.GetComponentVal(0);
								oidCur.rootExternal=arrayPatIdSubComps[1];
								listOids.Add(oidCur);
							}
						}
					}
					//patNumFromList will be 0 if the first repetition is not the OD patient id or if the check digit is incorrect based on the specified algorithm
					if(patNumFromIds!=0) {
						continue;
					}
					for(int r=0;r<fieldPatIds.ListRepeatFields.Count;r++) {
						arrayPatIdSubComps=fieldPatIds.ListRepeatFields[r].GetComponentVal(3).ToLower().Split(new char[] { subcompSeparator },StringSplitOptions.None);
						if(arrayPatIdSubComps.Length<2) {//there must be at least 2 sub-components in the assigning authority component so we can identify whose ID this is
							continue;
						}
						if(fieldPatIds.ListRepeatFields[r].GetComponentVal(4).ToLower()!="pi")
						{
							continue;
						}
						int checkDigit=-1;
						try {
							checkDigit=PIn.Int(fieldPatIds.ListRepeatFields[r].GetComponentVal(1));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//checkDigit will remain -1
						}
						if(fieldPatIds.ListRepeatFields[r].GetComponentVal(2).ToLower()=="m10"
						&& (checkDigit==-1 || M10CheckDigit(fieldPatIds.ListRepeatFields[r].GetComponentVal(0))!=checkDigit))//using M10 scheme and either invalid check digit or doesn't match calc
						{
							continue;
						}
						if(fieldPatIds.ListRepeatFields[r].GetComponentVal(2).ToLower()=="m11"
						&& (checkDigit==-1 || M11CheckDigit(fieldPatIds.ListRepeatFields[r].GetComponentVal(0))!=checkDigit))//using M11 scheme and either invalid check digit or doesn't match calc
						{
							continue;
						}
						//if not using the M10 or M11 check digit scheme or if the check digit is good, trust the ID in component 0 to be valid and attempt to use
						if(arrayPatIdSubComps[1]==patOIDRoot.ToLower()) {
							if(patNumFromIds==0) {
								try {
									patNumFromIds=PIn.Long(fieldPatIds.ListRepeatFields[r].GetComponentVal(0));
								}
								catch(Exception ex) {
									ex.DoNothing();
									//do nothing, patNumFromList will remain 0
								}
							}
						}
						else {
							OIDExternal oidCur=new OIDExternal();
							oidCur.IDType=IdentifierType.Patient;
							oidCur.IDExternal=fieldPatIds.ListRepeatFields[r].GetComponentVal(0);
							oidCur.rootExternal=arrayPatIdSubComps[1];
							listOids.Add(oidCur);
						}
					}
				}
				#endregion patientIdList
			}
			if(_isEcwHL7Def &&  (string.IsNullOrEmpty(patLName) || string.IsNullOrEmpty(patFName))) {
				EventLog.WriteEntry("OpenDentHL7","Message not processed due to missing first or last name. PatNum:"+patNum.ToString()
					,EventLogEntryType.Information);
				_hl7MsgCur.Note="Message not processed due to missing first or last name. PatNum:"+patNum.ToString();
				HL7Msgs.Update(_hl7MsgCur);
				return;
			}
			#endregion PID fieldsLoop
			#endregion GetPatientIDs
			#region Locate Patient Using PID Segment Details
			#region Locate Patient Using PatNum, ChartNumber, or Name and Birthday
			//We now have patnum, chartnum, patname, and/or birthdate so locate pat
			Patient pat=null;
			Patient patOld=null;
			//pat will be null if patNum==0
			pat=Patients.GetPat(patNum);
			if(pat==null && patNumFromIds>0) {
				pat=Patients.GetPat(patNumFromIds);
			}
			//If ChartNumber is a field in their defined PID segment, and they are not using eCWTight or Full internal type
			//Use the ChartNumber followed by Name and Birthdate to try to locate the patient for this message if chartNum is not null
			if(def.InternalType!=HL7InternalType.eCWFull && def.InternalType!=HL7InternalType.eCWTight && pat==null && chartNum!=null) {
				pat=Patients.GetPatByChartNumber(chartNum);
				//If not using eCWTight or Full integration, if pat is still null we need to try to locate patient by name and birthdate
				if(pat==null) {
					long patNumByName=Patients.GetPatNumByNameAndBirthday(patLName,patFName,birthdate);
					if(patNumByName>0) {
						pat=Patients.GetPat(patNumByName);
					}
				}
				if(pat!=null) {
					patOld=pat.Copy();
					pat.ChartNumber=chartNum;//from now on, we will be able to find pat by chartNumber
					Patients.Update(pat,patOld);
					if(_isVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Updated patient "+pat.GetNameFLnoPref()+" to include ChartNumber.",EventLogEntryType.Information);
					}
				}
			}
			#endregion Locate Patient Using PatNum, ChartNumber, or Name and Birthday
			#region Locate Patient Using External OIDs
			//Use the external OIDs stored in the oidexternal table to find the patient
			//Only trust the external IDs if all OIDs refer to the same patient
			long patNumFromExtIds=0;
			if(pat==null) {
				for(int i=0;i<listOids.Count;i++) {
					OIDExternal oidExternalCur=OIDExternals.GetByRootAndExtension(listOids[i].rootExternal,listOids[i].IDExternal);
					if(oidExternalCur==null || oidExternalCur.IDInternal==0 || oidExternalCur.IDType!=IdentifierType.Patient) {//must have an IDType of patient
						continue;
					}
					if(patNumFromExtIds==0) {
						patNumFromExtIds=oidExternalCur.IDInternal;
					}
					else if(patNumFromExtIds!=oidExternalCur.IDInternal) {//the current ID refers to a different patient than a previously found ID, don't trust the external IDs
						patNumFromExtIds=0;
						break;
					}
				}
				if(patNumFromExtIds>0) {//will be 0 if not in the OIDExternal table or no external IDs supplied or more than one supplied and they point to different pats (ambiguous)
					pat=Patients.GetPat(patNumFromExtIds);
				}
			}
			#endregion Locate Patient Using External OIDs
			_isNewPat=pat==null;
			#endregion Locate Patient Using PID Segment Details
			#region SRM Segment for OD as Filler Application
			if(!_isEcwHL7Def && msg.MsgType==MessageTypeHL7.SRM && _isNewPat) {//SRM messages must refer to existing appointments, so there must be an existing patient as well
				MessageHL7 hl7SRR=MessageConstructor.GenerateSRR(pat,null,msg.EventType,msg.ControlId,false,msg.AckEvent);//use false to indicate AE - Application Error in SRR.MSA segment
				HL7Msg hl7Msg=new HL7Msg();
				hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
				hl7Msg.MsgText=hl7SRR.ToString();
				hl7Msg.Note="Could not process HL7 SRM message due to an invalid or missing patient ID in the PID segment.";
				HL7Msgs.Insert(hl7Msg);
				throw new Exception("Could not process HL7 SRM message due to an invalid or missing patient ID in the PID segment.");
			}
			#endregion SRM Segment for OD as Filler Application
			#region Set pat for New Patient or patOld for Existing Patient
			if(_isNewPat) {
				pat=new Patient();
				if(chartNum!=null) {
					pat.ChartNumber=chartNum;
				}
				if(patNum!=0) {
					pat.PatNum=patNum;
					pat.Guarantor=patNum;
				}
				else if(patNumFromIds!=0) {
					pat.PatNum=patNumFromIds;
					pat.Guarantor=patNumFromIds;
				}
				else if(patNumFromExtIds!=0) {
					pat.PatNum=patNumFromExtIds;
					pat.Guarantor=patNumFromExtIds;
				}
				pat.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
				pat.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
			}
			else {
				patOld=pat.Copy();
			}
			#endregion Set pat for New Patient or patOld for Existing Patient
			#region Get Appointment if Necessary
			long aptNum=0;
			//If this is a message that contains an ARQ or SCH segment, loop through the fields to find the AptNum.  Pass it to other segment parsing methods that require it.
			//If this is an SRM message, and an AptNum is not included or no appointment with this AptNum is in the OD db, do not process the message.
			//We only accept SRM messages for appointments that already exist in the OD db.
			for(int i=0;i<hl7defmsg.hl7DefSegments.Count;i++) {
				if(hl7defmsg.hl7DefSegments[i].SegmentName!=SegmentNameHL7.SCH//SIU messages will have the SCH segment, used for eCW or other interfaces where OD is an auxiliary application
					&& hl7defmsg.hl7DefSegments[i].SegmentName!=SegmentNameHL7.ARQ)//SRM messages will have the ARQ segment, used for interfaces where OD is the filler application
				{
					continue;
				}
				//we found the SCH or ARQ segment
				int segOrder=hl7defmsg.hl7DefSegments[i].ItemOrder;
				for(int j=0;j<hl7defmsg.hl7DefSegments[i].hl7DefFields.Count;j++) {//Go through fields of SCH or ARQ segment and get AptNum
					if(hl7defmsg.hl7DefSegments[i].hl7DefFields[j].FieldName=="apt.AptNum") {
						int aptNumOrdinal=hl7defmsg.hl7DefSegments[i].hl7DefFields[j].OrdinalPos;
						try {
							aptNum=PIn.Long(msg.Segments[segOrder].GetFieldComponent(aptNumOrdinal,0).ToString());
						}
						catch(Exception ex) {//PIn.Long will throw an exception if a value is not able to be parsed into a long
							ex.DoNothing();
							//do nothing, aptNum will remain 0
						}
						break;
					}
				}
				if(aptNum>0) {
					break;
				}
			}
			Appointment aptCur=Appointments.GetOneApt(aptNum);//if aptNum=0, aptCur will be null
			#endregion Get Appointment if Necessary
			#region SRM Segment for OD as Filler Application
			//SRM messages are only for interfaces where OD is considered the 'filler' application
			//Not valid for eCW where OD is considered an 'auxiliary' application and we receive SIU messages instead.
			if(!_isEcwHL7Def && msg.MsgType==MessageTypeHL7.SRM) {
				MessageHL7 hl7Srr=null;
				HL7Msg hl7Msg=null;
				if(aptCur==null) {
					hl7Srr=MessageConstructor.GenerateSRR(pat,aptCur,msg.EventType,msg.ControlId,false,msg.AckEvent);//use false to indicate AE - Application Error in SRR.MSA segment
					hl7Msg=new HL7Msg();
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=hl7Srr.ToString();
					hl7Msg.PatNum=pat.PatNum;
					hl7Msg.Note="Could not process HL7 SRM message due to an invalid or missing appointment ID in the ARQ segment.  Appointment ID attempted: "+aptNum.ToString();
					HL7Msgs.Insert(hl7Msg);
					throw new Exception("Could not process HL7 SRM message due to an invalid or missing appointment ID in the ARQ segment.  Appointment ID attempted: "+aptNum.ToString());
				}
				if(pat.PatNum!=aptCur.PatNum) {//an SRM must refer to a valid appt, therefore the patient cannot be new and must have a PatNum
					hl7Srr=MessageConstructor.GenerateSRR(pat,aptCur,msg.EventType,msg.ControlId,false,msg.AckEvent);//use false to indicate AE - Application Error in SRR.MSA segment
					hl7Msg=new HL7Msg();
					hl7Msg.AptNum=aptCur.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=hl7Srr.ToString();
					hl7Msg.PatNum=pat.PatNum;
					hl7Msg.Note="Could not process HL7 SRM message.\r\n"
						+"The patient identified in the PID segment is not the same as the patient on the appointment identified in the ARQ segment.\r\n"
						+"Appointment PatNum: "+aptCur.PatNum.ToString()+".  PID segment PatNum: "+pat.PatNum.ToString()+".";
					HL7Msgs.Insert(hl7Msg);
					throw new Exception("Could not process HL7 SRM message.\r\n"
						+"The patient identified in the PID segment is not the same as the patient on the appointment identified in the ARQ segment.\r\n"
						+"Appointment PatNum: "+aptCur.PatNum.ToString()+".  PID segment PatNum: "+pat.PatNum.ToString()+".");
				}
			}
			#endregion SRM Segment for OD as Filler Application
			if(aptCur!=null && pat.PatNum!=aptCur.PatNum) {
				throw new Exception("Appointment does not match patient "+pat.GetNameFLnoPref()+", apt.PatNum: "+aptCur.PatNum.ToString()+", pat.PatNum: "+pat.PatNum.ToString());
			}
			#region Insert New Patient
			//We now have a patient object , either loaded from the db or new, and an appointment (could be null) so process this message for this patient
			//We need to insert the pat to get a patnum so we can compare to guar patnum to see if relationship to guar is self
			if(_isNewPat) {
				if(pat.PatNum==0) {//Only eCWTight or eCWFull internal types will allow the HL7 message to dictate our PatNums.
					pat.PatNum=Patients.Insert(pat,false);
					SecurityLogs.MakeLogEntry(Permissions.PatientCreate,pat.PatNum,"Created from HL7.",LogSources.HL7);
				}
				else {
					pat.PatNum=Patients.Insert(pat,true);
					SecurityLogs.MakeLogEntry(Permissions.PatientCreate,pat.PatNum,"Created from HL7.",LogSources.HL7);
				}
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted patient with PatNum="+pat.PatNum,EventLogEntryType.Information);
				}
				patOld=pat.Copy();
				if(pat.Guarantor==0) {
					pat.Guarantor=pat.PatNum;
					Patients.Update(pat,patOld);
					if(_isVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Updated patient with PatNum="+pat.PatNum+", updated Guarantor from 0 to "+pat.Guarantor+".",
							EventLogEntryType.Information);
					}
					patOld=pat.Copy();
				}
			}
			#endregion Insert New Patient
			//Update hl7msg table with correct PatNum for this message
			_hl7MsgCur.PatNum=pat.PatNum;
			HL7Msgs.Update(_hl7MsgCur);
			#region Process Segments
			for(int i=0;i<hl7defmsg.hl7DefSegments.Count;i++) {
				try {
					List<SegmentHL7> listSegments=new List<SegmentHL7>();
					if(hl7defmsg.hl7DefSegments[i].CanRepeat) {
						listSegments=msg.GetSegments(hl7defmsg.hl7DefSegments[i].SegmentName,!hl7defmsg.hl7DefSegments[i].IsOptional);
					}
					else {
						SegmentHL7 seg=msg.GetSegment(hl7defmsg.hl7DefSegments[i].SegmentName,!hl7defmsg.hl7DefSegments[i].IsOptional);
						if(seg==null) {//null if segment was not found but is optional
							continue;
						}
						listSegments.Add(seg);
					}
					for(int j=0;j<listSegments.Count;j++) {//normally only 1 or 0 in the list, but if it is a repeatable segment the list may contain multiple segments to process
						if(_isVerboseLogging) {
							EventLog.WriteEntry("OpenDentHL7","Process segment "+hl7defmsg.hl7DefSegments[i].SegmentName.ToString(),EventLogEntryType.Information);
						}
						ProcessSeg(pat,aptCur,hl7defmsg.hl7DefSegments[i],listSegments[j],msg);
						if(hl7defmsg.hl7DefSegments[i].SegmentName==SegmentNameHL7.SCH) {
							aptCur=_aptProcessed;
						}
					}
				}
				catch(ApplicationException ex) {//Required segment was missing, or other error.
					_hl7MsgCur.Note="Could not process this HL7 message.  "+ex;
					HL7Msgs.Update(_hl7MsgCur);
					if(!_isEcwHL7Def && msg.MsgType==MessageTypeHL7.SRM) {//SRM messages require sending an SRR response, this will be with Ack Code AE - Application Error
						MessageHL7 hl7Srr=MessageConstructor.GenerateSRR(pat,aptCur,msg.EventType,msg.ControlId,false,msg.AckEvent);//use false to indicate AE - Application Error in SRR.MSA segment
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=aptCur.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=hl7Srr.ToString();
						hl7Msg.Note="Could not process an HL7 SRM message.  Send SRR for the request.  "+ex;
						hl7Msg.PatNum=pat.PatNum;
						HL7Msgs.Insert(hl7Msg);
					}
					throw new Exception("Could not process an HL7 message.  "+ex);
				}
			}
			#endregion Process Segments
			//We have processed the message so now update the patient
			if(pat.FName=="" || pat.LName=="") {
				EventLog.WriteEntry("OpenDentHL7","Patient demographics not processed due to missing first or last name. PatNum:"+pat.PatNum.ToString()
					,EventLogEntryType.Information);
				_hl7MsgCur.Note="Patient demographics not processed due to missing first or last name. PatNum:"+pat.PatNum.ToString();
				HL7Msgs.Update(_hl7MsgCur);
			}
			else {
				Patients.Update(pat,patOld);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated patient "+pat.GetNameFLnoPref(),EventLogEntryType.Information);
				}
				_hl7MsgCur.HL7Status=HL7MessageStatus.InProcessed;
				HL7Msgs.Update(_hl7MsgCur);
			}
			#region SRM Segment for OD as Filler Application
			//Schedule Request Messages require a Schedule Request Response if the data requested to be changed was successful
			//We only allow changing the appt note, setting the dentist and hygienist, updating the confirmation status, and changing the ClinicNum.
			//We also allow setting the appt status to broken if the EventType is S04 - Request Appointment Cancellation.
			//We will generate the SRR if we get here, since we will have processed the message properly.
			//The SRM will be ACK'd after returning from this Process method in ServiceHL7.
			//Our SRR will also be ACK'd by the receiving software.
			if(!_isEcwHL7Def && msg.MsgType==MessageTypeHL7.SRM) {
				if(msg.AckEvent=="S04") {
					Appointment aptOld=aptCur.Copy();
					aptCur.AptStatus=ApptStatus.Broken;
					Appointments.Update(aptCur,aptOld);
					if(_isVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Appointment broken due to inbound SRM message with event type S04 for patient "+pat.GetNameFLnoPref(),EventLogEntryType.Information);
					}
				}
				MessageHL7 hl7Srr=MessageConstructor.GenerateSRR(pat,aptCur,msg.EventType,msg.ControlId,true,msg.AckEvent);
				HL7Msg hl7Msg=new HL7Msg();
				hl7Msg.AptNum=aptCur.AptNum;
				hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
				hl7Msg.MsgText=hl7Srr.ToString();
				hl7Msg.PatNum=pat.PatNum;
				HL7Msgs.Insert(hl7Msg);
			}
			#endregion SRM Segment for OD as Filler Application
		}

		public static void ProcessAck(MessageHL7 msg,bool isVerboseLogging) {
			_isVerboseLogging=isVerboseLogging;
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def==null) {
				throw new Exception("Could not process ACK.  No HL7 definition is enabled.");
			}
			HL7DefMessage hl7DefMsg=null;
			for(int i=0;i<hl7Def.hl7DefMessages.Count;i++) {
				if(hl7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.ACK && hl7Def.hl7DefMessages[i].InOrOut==InOutHL7.Incoming) {
					hl7DefMsg=hl7Def.hl7DefMessages[i];
					break;
				}
			}
			if(hl7DefMsg==null) {//No incoming ACK defined, do nothing with it
				throw new Exception("Could not process HL7 ACK message.  No definition for this type of message in the enabled HL7Def.");
			}
			for(int i=0;i<hl7DefMsg.hl7DefSegments.Count;i++) {
				try {
					SegmentHL7 seg=msg.GetSegment(hl7DefMsg.hl7DefSegments[i].SegmentName,!hl7DefMsg.hl7DefSegments[i].IsOptional);
					if(seg!=null) {//null if segment was not found but is optional
						ProcessSeg(null,null,hl7DefMsg.hl7DefSegments[i],seg,msg);
					}
				}
				catch(ApplicationException ex) {//Required segment was missing, or other error.
					throw new Exception("Could not process HL7 message.  "+ex);
				}
			}
		}

		public static void ProcessSeg(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			switch(segDef.SegmentName) {
				case SegmentNameHL7.AIG:
				case SegmentNameHL7.AIP:
					ProcessAIGorAIP(pat,apt,segDef,seg);//segDef.SegmentName will be used by this function to parse the provider as ProvNum^LName^Fname^MI^^Abbr or ProvNum^LName, FName^^Abbr
					return;
				case SegmentNameHL7.AIL:
					ProcessAIL(pat,apt,segDef,seg);
					return;
				case SegmentNameHL7.AL1:
					ProcessAL1(pat,segDef,seg);
					return;
				case SegmentNameHL7.ARQ:
					ProcessARQ(pat,apt,segDef,seg,msg);
					return;
				case SegmentNameHL7.GT1:
					ProcessGT1(pat,segDef,seg,msg);
					return;
				case SegmentNameHL7.IN1:
					//ProcessIN1();
					return;
				case SegmentNameHL7.MSA:
					ProcessMSA(segDef,seg,msg);
					return;
				case SegmentNameHL7.MSH:
					ProcessMSH(segDef,seg,msg);
					return;
				case SegmentNameHL7.NTE:
					ProcessNTE(pat,apt,segDef,seg,msg);
					return;
				case SegmentNameHL7.OBX:
					ProcessOBX(pat,segDef,seg);
					return;
				case SegmentNameHL7.PD1:
					//ProcessPD1();
					return;
				case SegmentNameHL7.PID:
					ProcessPID(pat,segDef,seg,msg);
					return;
				case SegmentNameHL7.PR1:
					ProcessPR1(pat,segDef,seg,msg);
					return;
				case SegmentNameHL7.PRB:
					ProcessPRB(pat,segDef,seg,msg);
					return;
				case SegmentNameHL7.PV1:
					ProcessPV1(pat,apt,segDef,seg);
					return;
				case SegmentNameHL7.SCH:
					ProcessSCH(pat,apt,segDef,seg,msg);
					return;
				default:
					return;
			}
		}

		///<summary>This is used to set the provider (either dentist or hygienist depending on provType) and the confirmation status of the appointment.  If the apt is null, does nothing.  If this is an AIG, the provider field will be in the format ProvNum^LName, FName^^Abbr.  AIP and PV1 have the provider field in the format ProvNum^LName^FName^^^Abbr.  segDef.SegmentName will be used to determine how to parse the field.</summary>
		public static void ProcessAIGorAIP(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg) {
			if(apt==null) {//We have to have an appt to process the AIG or AIP segment
				return;
			}
			Appointment aptOld=apt.Copy();
			long provNum=0;
			string strProvType="";
			Def[] listConfirmStats=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true).ToArray();
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				if(itemOrder<1) {
					continue;
				}
				switch(segDef.hl7DefFields[i].FieldName) {
					case "prov.provIdName":
					case "prov.provIdName_V2_3":
					case "prov.provIdNameLFM":
					case "prov.provIdNameLFM_V2_3":
						if(_isEcwHL7Def) {
							//for eCW interfaces, we allow inserting a new provider from the AIG or AIP segment and store the provider ID they supply in the prov.EcwId column
							provNum=FieldParser.ProvProcessEcw(seg.GetField(itemOrder));
						}
						else {
							//for all other interfaces, we will only accept provider ID's that match the OD ProvNum
							//if not found by ProvNum, we will attempt to match LName, FName, and Abbr
							//if still not found, we will not set the provider on the appt
							provNum=FieldParser.ProvParse(seg.GetField(itemOrder),segDef.SegmentName,_isVerboseLogging,segDef.hl7DefFields[i].FieldName.EndsWith("_V2_3"));
						}
						//provNum may still be 0
						continue;
					case "prov.provType":
						strProvType=seg.GetFieldComponent(itemOrder);
						//provType could still be an empty string, which is treated the same as 'd' or 'D' for dentist
						continue;
					case "apt.confirmStatus":
						long aptConfirmDefNum=0;
						for(int j=0;j<listConfirmStats.Length;j++) {
							if(seg.GetFieldComponent(itemOrder,1).ToLower()==listConfirmStats[j].ItemName.ToLower()//ItemName is the confirmation name
								|| (listConfirmStats[j].ItemValue!="" && seg.GetFieldComponent(itemOrder,1).ToLower()==listConfirmStats[j].ItemValue.ToLower()))//ItemValue is the confirmation abbreviation, which may be blank
							{
								aptConfirmDefNum=listConfirmStats[j].DefNum;
								break;
							}
						}
						if(aptConfirmDefNum>0) {
							apt.Confirmed=aptConfirmDefNum;
						}
						continue;
					default:
						continue;
				}
			}
			if(provNum>0) {
				if(strProvType.ToLower()=="h") {//only set ProvHyg if they specifically send 'h' or 'H' as the provType
					apt.ProvHyg=provNum;
				}
				else {//if 'd' or 'D' or not specified or invalid provType, default to setting the ProvNum
					apt.ProvNum=provNum;
				}
				if(_isEcwHL7Def) {
					if(pat.PriProv!=provNum) {
						pat.PriProv=provNum;
						SecurityLogs.MakeLogEntry(Permissions.PatPriProvEdit,pat.PatNum,
							"Primary provider changed automatically from "+(pat.PriProv==0?"'blank'":Providers.GetLongDesc(pat.PriProv))+" to "
							+Providers.GetLongDesc(provNum)+" due to an indound HL7 message "+segDef.SegmentName.ToString()+" segment.");
					}
				}
			}
			Appointments.Update(apt,aptOld);
			if(apt.Confirmed!=aptOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,apt.PatNum,
					Lans.g("MessageParser","Appointment confirmation status automatically changed from")+" "
					+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" "+Lans.g("MessageParser","to")+" "+Defs.GetName(DefCat.ApptConfirmed,apt.Confirmed)
					+" "+Lans.g("MessageParser","due to an indound HL7 message")+".",apt.AptNum,LogSources.HL7,aptOld.DateTStamp);
			}
			_aptProcessed=apt;
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated appointment for patient "+pat.GetNameFLnoPref()+" due to an incoming AIG or AIP segment.",EventLogEntryType.Information);
			}
			return;
		}

		///<summary>This will be used to set the clinic and confirmation status of the appointment.  If apt is null, does nothing.</summary>
		public static void ProcessAIL(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg) {
			if(apt==null) {//We have to have an appt to process the AIL segment
				return;
			}
			Def[] listConfirmStats=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true).ToArray();
			Appointment aptOld=apt.Copy();	
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int intItemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "apt.confirmStatus":
						long aptConfirmDefNum=0;
						for(int j=0;j<listConfirmStats.Length;j++) {
							if(seg.GetFieldComponent(intItemOrder,1).ToLower()==listConfirmStats[j].ItemName.ToLower()//ItemName is the confirmation name
								|| (listConfirmStats[j].ItemValue!="" && seg.GetFieldComponent(intItemOrder,1).ToLower()==listConfirmStats[j].ItemValue.ToLower()))//ItemValue is the confirmation abbreviation, which may be blank
							{
								aptConfirmDefNum=listConfirmStats[j].DefNum;
								break;
							}
						}
						if(aptConfirmDefNum>0) {
							apt.Confirmed=aptConfirmDefNum;
						}
						continue;
					case "apt.location":
						//Example: ClinicDescript^OpName^^PracticeTitle^^c  (c for clinic)
						long clinicNum=Clinics.GetByDesc(seg.GetFieldComponent(intItemOrder,0));//0 if field is blank or description doesn't match clinic description in the db
						if(clinicNum>0) {
							apt.ClinicNum=clinicNum;//the pat.ClinicNum may be set based on a different segment, like the PV1 segment
						}
						continue;
					default:
						continue;
				}
			}
			Appointments.Update(apt,aptOld);
			if(apt.Confirmed!=aptOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,apt.PatNum,
					Lans.g("MessageParser","Appointment confirmation status automatically changed from")+" "
					+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" "+Lans.g("MessageParser","to")+" "+Defs.GetName(DefCat.ApptConfirmed,apt.Confirmed)
					+" "+Lans.g("MessageParser","due to an indound HL7 message")+".",apt.AptNum,LogSources.HL7,aptOld.DateTStamp);
			}
			_aptProcessed=apt;
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated appointment for patient "+pat.GetNameFLnoPref()+" due to an incoming AIL segment.",EventLogEntryType.Information);
			}
			return;
		}

		public static void ProcessAL1(Patient pat,HL7DefSegment segDef,SegmentHL7 seg) {
			//If there is an allergydef with SnomedType 2 or 3 (DrugAllergy or DrugIntolerance) with attached medication with RxCui=rxnorm supplied
			//And if there is not already an active allergy with that AllergyDefNum for the patient
			//Then insert an allergy with that AllergyDefNum for this patient
			long rxnorm=0;
			string strAllergenType="";
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int intItemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "allergenType":
						strAllergenType=seg.GetFieldComponent(intItemOrder).ToLower();
						continue;
					case "allergenRxNorm":
						if(seg.GetFieldComponent(intItemOrder,2).ToLower()!="rxnorm") {
							//if not an RXNORM code, do nothing.  Only RXNORM codes are currently supported
							EventLog.WriteEntry("OpenDentHL7","A drug allergy was not added for patient "+pat.GetNameFLnoPref()
								+".  Only RxNorm codes are currently allowed in the AL1 segment.  The code system name provided was "
								+seg.GetFieldComponent(intItemOrder,2)+".",EventLogEntryType.Information);
							return;
						}
						try {
							rxnorm=PIn.Long(seg.GetFieldComponent(intItemOrder,0));
						}
						catch(Exception ex) {//PIn.Long throws an exception if converting to an Int64 fails
							ex.DoNothing();
							//do nothing, rxnorm will remain 0
						}
						continue;
					default:
						continue;
				}
			}
			if(rxnorm==0 || strAllergenType!="da") {
				EventLog.WriteEntry("OpenDentHL7","A drug allergy was not added for patient "+pat.GetNameFLnoPref()
					+".  Only drug allergies, identified by type 'DA', are currently allowed in the AL1 segment.  The allergen type received was '"
					+strAllergenType+"'.",EventLogEntryType.Information);
				return;//not able to assign drug allergy if not given a valid rxnorm or allergen type is not a drug allergy
			}
			AllergyDef allergyDefCur=AllergyDefs.GetAllergyDefFromRxnorm(rxnorm);
			if(allergyDefCur==null) {
				EventLog.WriteEntry("OpenDentHL7","A drug allergy was not added for patient "+pat.GetNameFLnoPref()+
					".  There is not a drug allergy in the database with an RxNorm code of "+rxnorm.ToString()+".",EventLogEntryType.Information);
				return;//no allergydef for this rxnorm exists
			}
			//see if there is already an active allergy with this AllergyDefNum for this patient
			List<Allergy> listAllergForPat=Allergies.GetAll(pat.PatNum,false);
			for(int i=0;i<listAllergForPat.Count;i++) {
				if(listAllergForPat[i].AllergyDefNum==allergyDefCur.AllergyDefNum) {
					return;//already an active allergy with this AllergyDefNum
				}
			}
			Allergy allergyCur=new Allergy();
			allergyCur.AllergyDefNum=allergyDefCur.AllergyDefNum;
			allergyCur.PatNum=pat.PatNum;
			allergyCur.StatusIsActive=true;
			Allergies.Insert(allergyCur);
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Inserted a new allergy for patient "+pat.GetNameFLnoPref()+" due to an incoming AL1 segment.",EventLogEntryType.Information);
			}
			return;
		}

		///<summary>Appointment request segment.  Included in inbound SRM, Schedule Request Messages.  When OD is the filler application, this will identify the appointment that the placer or auxiliary aplication is trying to update.  We only support event S03 - Appt Modification requests or event S04 - Appt Cancellation requests for now.</summary>
		public static void ProcessARQ(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			long aptNum=0;
			string externAptId="";
			string externRoot="";
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int intItemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "apt.externalAptID":
						externAptId=seg.GetFieldComponent(intItemOrder,0);
						externRoot=seg.GetFieldComponent(intItemOrder,2);
						continue;
					case "apt.AptNum":
						try {
							aptNum=PIn.Long(seg.GetFieldComponent(intItemOrder,0));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, aptNum will remain 0
						}
						if(apt!=null && apt.AptNum!=aptNum) {
							//an appointment was located from the inbound message, but the AptNum on the appointment is not the same as the AptNum in this ARQ segment (should never happen)
							throw new Exception("Invalid appointment number.");
						}
						continue;
					default:
						continue;
				}
			}
			if(externAptId!="" && externRoot!="" && OIDExternals.GetByRootAndExtension(externRoot,externAptId)==null) {
				OIDExternal oidExtCur=new OIDExternal();
				oidExtCur.IDType=IdentifierType.Appointment;
				oidExtCur.IDInternal=apt.AptNum;
				oidExtCur.IDExternal=externAptId;
				oidExtCur.rootExternal=externRoot;
				OIDExternals.Insert(oidExtCur);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Added an external appointment ID to the oidexternals table due to an incoming ARQ segment.\r\n"
						+"AptNum: "+apt.AptNum.ToString()+", External AptID: "+externAptId+", External root: "+externRoot+".",EventLogEntryType.Information);
				}
			}
			_hl7MsgCur.AptNum=apt.AptNum;
			HL7Msgs.Update(_hl7MsgCur);
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated hl7msg to include AptNum for "+pat.GetNameFLnoPref()+" due to an incoming SCH segment.",EventLogEntryType.Information);
			}
			_aptProcessed=apt;
			return;
		}

		///<summary>If relationship is self, this method does nothing.  A new pat will later change guarantor to be same as patnum. </summary>
		public static void ProcessGT1(Patient pat,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			char escapeChar='\\';
			if(msg.Delimiters.Length>2) {//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default \
				escapeChar=msg.Delimiters[2];
			}
			char subcompChar='&';
			if(msg.Delimiters.Length>3) {
				subcompChar=msg.Delimiters[3];
			}
			#region Get And Validate Definition Field Ordinals
			//Find the position of the guarNum, guarChartNum, guarName, and guarBirthdate in this HL7 segment based on the definition of a GT1
			int guarPatNumOrdinal=-1;
			int guarChartNumOrdinal=-1;
			int guarNameOrdinal=-1;
			int guarBirthdateOrdinal=-1;
			int guarIdsOrdinal=-1;
			string patOidRoot="";
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				switch(segDef.hl7DefFields[i].FieldName) {
					case "guar.PatNum":
						guarPatNumOrdinal=segDef.hl7DefFields[i].OrdinalPos;
						continue;
					case "guar.ChartNumber":
						guarChartNumOrdinal=segDef.hl7DefFields[i].OrdinalPos;
						continue;
					case "guar.nameLFM":
						guarNameOrdinal=segDef.hl7DefFields[i].OrdinalPos;
						continue;
					case "guar.birthdateTime":
						guarBirthdateOrdinal=segDef.hl7DefFields[i].OrdinalPos;
						continue;
					case "guarIdList":
						//get the id with the assigning authority equal to the internal OID root stored in their database for a patient object
						patOidRoot=OIDInternals.GetForType(IdentifierType.Patient).IDRoot;
						if(patOidRoot=="") {
							//if they have not set their internal OID root, we cannot identify which repetition in this field contains the OD PatNum, guarIdListOrdinal will remain -1
							continue;
						}
						guarIdsOrdinal=segDef.hl7DefFields[i].OrdinalPos;
						continue;
					default://not supported
						continue;
				}
			}
			//If neither guar.PatNum nor guar.ChartNumber are included in this GT1 definition log a message in the event log and return
			if(guarPatNumOrdinal==-1 && guarChartNumOrdinal==-1 && guarIdsOrdinal==-1) {
				_hl7MsgCur.Note="Guarantor not processed.  guar.PatNum, guar.ChartNumber, or guarIdList must be included in the GT1 definition.  PatNum of patient: "+pat.PatNum.ToString();
				HL7Msgs.Update(_hl7MsgCur);
				EventLog.WriteEntry("OpenDentHL7","Guarantor not processed.  guar.PatNum, guar.ChartNumber, or guarIdList must be included in the GT1 definition.  "
					+"PatNum of patient: "+pat.PatNum.ToString(),EventLogEntryType.Information);
				return;
			}
			//If guar.nameLFM is not included in this GT1 definition log a message in the event log and return
			if(guarNameOrdinal==-1) {
				_hl7MsgCur.Note="Guarantor not processed due to guar.nameLFM not included in the GT1 definition.  Patnum of patient: "+pat.PatNum.ToString();
				HL7Msgs.Update(_hl7MsgCur);
				EventLog.WriteEntry("OpenDentHL7","Guarantor not processed due to guar.nameLFM not included in the GT1 definition.  Patnum of patient: "
					+pat.PatNum.ToString(),EventLogEntryType.Information);
				return;
			}
			//If the first or last name are not included in this GT1 segment, log a message in the event log and return
			if(seg.GetFieldComponent(guarNameOrdinal,0)=="" || seg.GetFieldComponent(guarNameOrdinal,1)=="") {
				_hl7MsgCur.Note="Guarantor not processed due to missing first or last name.  PatNum of patient: "+pat.PatNum.ToString();
				HL7Msgs.Update(_hl7MsgCur);
				EventLog.WriteEntry("OpenDentHL7","Guarantor not processed due to missing first or last name.  PatNum of patient: "
					+pat.PatNum.ToString(),EventLogEntryType.Information);
				return;
			}
			#endregion Get And Validate Definition Field Ordinals
			#region Get guar.PatNum, guar.ChartNumber, and guarIdList IDs
			//Only process GT1 if guar.PatNum, guar.ChartNumber, or guarIdList is included and both guar.LName and guar.FName are included
			long guarPatNum=0;
			string guarChartNum="";
			long guarPatNumFromIds=0;
			List<OIDExternal> listOids=new List<OIDExternal>();
			if(guarPatNumOrdinal!=-1) {
				try {
					guarPatNum=PIn.Long(seg.GetFieldFullText(guarPatNumOrdinal));
				}
				catch(Exception ex) {
					ex.DoNothing();
					//do nothing, guarPatNum will remain 0
				}
			}
			if(guarChartNumOrdinal!=-1) {
				guarChartNum=seg.GetFieldComponent(guarChartNumOrdinal);
			}
			if(guarIdsOrdinal!=-1) {//if OIDInternal root for a patient object is not defined, guarIdListOrdinal will be -1
				FieldHL7 fieldGuarIds=seg.GetField(guarIdsOrdinal);
				//Example field: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&OIDType^PI|
				//field component values will be the first repetition, repeats will be in field.ListRepeatFields
				string[] arrayGuarIdSubComps=fieldGuarIds.GetComponentVal(3).Split(new char[] { subcompChar },StringSplitOptions.None);
				if(arrayGuarIdSubComps.Length>1 //must be at least two subcomponents in the assigning authority component so we can identify whose ID this is
					&& fieldGuarIds.GetComponentVal(4).ToLower()=="pi") //PI=patient internal identifier; a number that is unique to a patient within an assigning authority
				{
					int intCheckDigit=-1;
					try {
						intCheckDigit=PIn.Int(fieldGuarIds.GetComponentVal(1));
					}
					catch(Exception ex) {
						ex.DoNothing();
						//checkDigit will remain -1
					}
					//if using the M10 or M11 check digit algorithm and it passes the respective test, or not using either algorithm
					//and only if there's at least 2 components in field 3 for the assigning authority so we can identify whose ID we are dealing with, then use the ID
					if((fieldGuarIds.GetComponentVal(2).ToLower()=="m10"
							&& intCheckDigit!=-1
							&& M10CheckDigit(fieldGuarIds.GetComponentVal(0))==intCheckDigit)//using M10 scheme and the check digit is valid and matches calc
						|| (fieldGuarIds.GetComponentVal(2).ToLower()=="m11"
							&& intCheckDigit!=-1
							&& M11CheckDigit(fieldGuarIds.GetComponentVal(0))==intCheckDigit)//using M11 scheme and the check digit is valid and matches calc
						|| (fieldGuarIds.GetComponentVal(2).ToLower()!="m10"
							&& fieldGuarIds.GetComponentVal(2).ToLower()!="m11"))//not using either check digit scheme
					{
						if(arrayGuarIdSubComps[1].ToLower()==patOidRoot.ToLower()) {
							try {
								guarPatNumFromIds=PIn.Long(fieldGuarIds.GetComponentVal(0));
							}
							catch(Exception ex) {
								ex.DoNothing();
								//do nothing, guarPatNumFromList will remain 0
							}
						}
						else {
							OIDExternal oidCur=new OIDExternal();
							oidCur.IDType=IdentifierType.Patient;
							oidCur.IDExternal=fieldGuarIds.GetComponentVal(0);
							oidCur.rootExternal=arrayGuarIdSubComps[1];
							listOids.Add(oidCur);
						}
					}
				}
				for(int r=0;r<fieldGuarIds.ListRepeatFields.Count;r++) {
					arrayGuarIdSubComps=fieldGuarIds.ListRepeatFields[r].GetComponentVal(3).Split(new char[] { subcompChar },StringSplitOptions.None);
					if(arrayGuarIdSubComps.Length<2) {//must be at least two sub-components to the assigning authority component so we can identify whose ID this is
						continue;
					}
					if(fieldGuarIds.ListRepeatFields[r].GetComponentVal(4).ToLower()!="pi")
					{
						continue;
					}
					int intCheckDigit=-1;
					try {
						intCheckDigit=PIn.Int(fieldGuarIds.ListRepeatFields[r].GetComponentVal(1));
					}
					catch(Exception ex) {
						ex.DoNothing();
						//checkDigit will remain -1
					}
					if(fieldGuarIds.ListRepeatFields[r].GetComponentVal(2).ToLower()=="m10"
						&& (intCheckDigit==-1 || M10CheckDigit(fieldGuarIds.ListRepeatFields[r].GetComponentVal(0))!=intCheckDigit))//using M10 scheme and either invalid check digit or doesn't match calc
					{
						continue;
					}
					if(fieldGuarIds.ListRepeatFields[r].GetComponentVal(2).ToLower()=="m11"
						&& (intCheckDigit==-1 || M11CheckDigit(fieldGuarIds.ListRepeatFields[r].GetComponentVal(0))!=intCheckDigit))//using M11 scheme and either invalid check digit or doesn't match calc
					{
						continue;
					}
					//if not using the M10 or M11 check digit scheme or if the check digit is good, trust the ID in component 0 to be valid and attempt to use
					if(arrayGuarIdSubComps[1].ToLower()==patOidRoot.ToLower()) {
						try {
							guarPatNumFromIds=PIn.Long(fieldGuarIds.ListRepeatFields[r].GetComponentVal(0));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, guarPatNumFromList will remain 0
						}
					}
					else {
						OIDExternal oidCur=new OIDExternal();
						oidCur.IDType=IdentifierType.Patient;
						oidCur.IDExternal=fieldGuarIds.ListRepeatFields[r].GetComponentVal(0);
						oidCur.rootExternal=arrayGuarIdSubComps[1];
						listOids.Add(oidCur);
					}
				}
			}
			if(guarPatNum==0 && guarChartNum=="" && guarPatNumFromIds==0 && listOids.Count==0) {//because we have an example where they sent us this (position 2 (guarPatNumOrder or guarChartNumOrder for eCW) is empty): GT1|1||^^||^^^^||||||||
				_hl7MsgCur.Note="Guarantor not processed due to missing guar.PatNum, guar.ChartNumber, and guarIdList.  One of those numbers must be included.  "
					+"PatNum of patient: "+pat.PatNum.ToString();
				HL7Msgs.Update(_hl7MsgCur);
				EventLog.WriteEntry("OpenDentHL7","Guarantor not processed due to missing guar.PatNum, guar.ChartNumber, and guarIdList.  "
					+"One of those numbers must be included.  PatNum of patient: "+pat.PatNum.ToString(),EventLogEntryType.Information);
				return;
			}
			#endregion Get guar.PatNum, guar.ChartNumber, and guarIdList IDs
			if(guarPatNum==pat.PatNum
				|| (guarChartNum!="" && guarChartNum==pat.ChartNumber)
				|| guarPatNumFromIds==pat.PatNum)
			{//if relationship is self
				return;
			}
			#region Get Patient from Ids
			//Guar must be someone else
			Patient guar=null;
			Patient guarOld=null;
			//Find guarantor by guar.PatNum if defined and in this segment
			if(guarPatNum!=0) {
				guar=Patients.GetPat(guarPatNum);
			}
			else if(guarPatNumFromIds!=0) {
				guar=Patients.GetPat(guarPatNumFromIds);
			}
			else if(guarChartNum!="") {//guarPatNum and guarPatNumFromList was 0 so try to get guar by guar.ChartNumber or name and birthdate
				//try to find guarantor using chartNumber
				guar=Patients.GetPatByChartNumber(guarChartNum);
				if(guar==null) {
					//try to find the guarantor by using name and birthdate
					string strGuarLName=seg.GetFieldComponent(guarNameOrdinal,0);
					string strGuarFName=seg.GetFieldComponent(guarNameOrdinal,1);
					DateTime guarBirthdate=FieldParser.DateTimeParse(seg.GetFieldFullText(guarBirthdateOrdinal));
					long guarNamePatNum=Patients.GetPatNumByNameAndBirthday(strGuarLName,strGuarFName,guarBirthdate);
					if(guarNamePatNum==0) {//guarantor does not exist in OD
						//so guar will still be null, triggering creation of new guarantor further down.
					}
					else {
						guar=Patients.GetPat(guarNamePatNum);
						guarOld=guar.Copy();
						guar.ChartNumber=guarChartNum;//from now on, we will be able to find guar by chartNumber
						Patients.Update(guar,guarOld);
					}
				}
			}
			long guarExtPatNum=0;
			if(guar==null) {//As a last resort, use the external IDs for this patient in the oidexternal to find the guar
				//Use the external OIDs stored in the oidexternal table to find the patient
				//Only trust the external IDs if all OIDs refer to the same patient
				for(int i=0;i<listOids.Count;i++) {
					OIDExternal oidExternalCur=OIDExternals.GetByRootAndExtension(listOids[i].rootExternal,listOids[i].IDExternal);
					if(oidExternalCur==null || oidExternalCur.IDInternal==0 || oidExternalCur.IDType!=IdentifierType.Patient) {//must have an IDType of Patient
						continue;
					}
					if(guarExtPatNum==0) {
						guarExtPatNum=oidExternalCur.IDInternal;
					}
					else if(guarExtPatNum!=oidExternalCur.IDInternal) {//the current ID refers to a different patient than a previously found ID, don't trust the external IDs
						guarExtPatNum=0;
						break;
					}
				}
				if(guarExtPatNum>0) {//will be 0 if not in the OIDExternal table or no external IDs supplied or more than one supplied and they point to different pats (ambiguous)
					guar=Patients.GetPat(guarExtPatNum);
				}
			}
			//At this point we have a guarantor located in OD or guar=null so guar is new patient
			bool isNewGuar=(guar==null);
			if(isNewGuar) {//then we need to add guarantor to db
				guar=new Patient();
				if(guarChartNum!="") {
					guar.ChartNumber=guarChartNum;
				}
				if(guarPatNum!=0) {
					guar.PatNum=guarPatNum;
					guar.Guarantor=guarPatNum;
				}
				else if(guarPatNumFromIds!=0) {
					guar.PatNum=guarPatNumFromIds;
					guar.Guarantor=guarPatNumFromIds;
				}
				else if(guarExtPatNum!=0) {
					guar.PatNum=guarExtPatNum;
					guar.Guarantor=guarExtPatNum;
				}
				guar.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
				guar.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
			}
			else {
				guarOld=guar.Copy();
			}
			#endregion Get Patient from Ids
			#region Update Guarantor Data
			//Now that we have our guarantor, process the GT1 segment
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "guar.addressCityStateZip":
						guar.Address=seg.GetFieldComponent(itemOrder,0);
						guar.Address2=seg.GetFieldComponent(itemOrder,1);
						guar.City=seg.GetFieldComponent(itemOrder,2);
						guar.State=seg.GetFieldComponent(itemOrder,3);
						guar.Zip=seg.GetFieldComponent(itemOrder,4);
						guar.AddrNote=FieldParser.StringNewLineParse(seg.GetFieldComponent(itemOrder,19),escapeChar);
						continue;
					case "guar.birthdateTime":
						guar.Birthdate=FieldParser.DateTimeParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "guar.ChartNumber":
						guar.ChartNumber=seg.GetFieldComponent(itemOrder);
						continue;
					case "guar.Gender":
						guar.Gender=FieldParser.GenderParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "guar.HmPhone":
						if(seg.GetFieldComponent(itemOrder,2)=="" && seg.GetField(itemOrder).ListRepeatFields.Count==0) {
							//either no component 2 or left blank, and there are no repetitions, process the old way
							//the first repetition is considered the primary number, but if the primary number is not sent then it will be blank followed by the list of other numbers
							guar.HmPhone=FieldParser.PhoneParse(seg.GetFieldComponent(itemOrder));
						}
						else {
							//XTN data type, repeatable.
							//Component 2 values: PH-Phone, FX-Fax, MD-Modem, CP-Cell Phone, Internet-Internet Address, X.400-X.400 email address, TDD-Tel Device for the Deaf, TTY-Teletypewriter.
							//We will support PH for pat.HmPhone, CP for pat.WirelessPhone, and Internet for pat.Email
							//Component 5 is area code, 6 is number
							//Component 3 will be Email if the type is Internet
							//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com
							FieldHL7 fieldPh=seg.GetField(itemOrder);
							if(fieldPh==null) {
								continue;
							}
							string strPhType=seg.GetFieldComponent(itemOrder,2);
							string strPh=seg.GetFieldComponent(itemOrder,5)+seg.GetFieldComponent(itemOrder,6);
							string strEmail=seg.GetFieldComponent(itemOrder,3);
							for(int p=-1;p<fieldPh.ListRepeatFields.Count;p++) {
								if(p>=0) {
									strPhType=fieldPh.ListRepeatFields[p].GetComponentVal(2);
									strPh=fieldPh.ListRepeatFields[p].GetComponentVal(5)+fieldPh.ListRepeatFields[p].GetComponentVal(6);
									strEmail=fieldPh.ListRepeatFields[p].GetComponentVal(3);
								}
								switch(strPhType) {
									case "PH":
										guar.HmPhone=FieldParser.PhoneParse(strPh);
										continue;
									case "CP":
										guar.WirelessPhone=FieldParser.PhoneParse(strPh);
										continue;
									case "Internet":
										guar.Email=strEmail;
										continue;
									default:
										continue;
								}
							}
						}
						continue;
					case "guar.nameLFM":
						guar.LName=seg.GetFieldComponent(itemOrder,0);
						guar.FName=seg.GetFieldComponent(itemOrder,1);
						guar.MiddleI=seg.GetFieldComponent(itemOrder,2);
						guar.Title=seg.GetFieldComponent(itemOrder,4);
						continue;
					//case "guar.PatNum": Maybe do nothing??
					case "guar.SSN":
						guar.SSN=seg.GetFieldComponent(itemOrder);
						continue;
					case "guar.WkPhone":
						if(seg.GetFieldComponent(itemOrder,2)=="PH") {
							//Component 2 value: PH-Phone
							//Component 5 is area code, 6 is number
							//Example: ^WPN^PH^^^503^3635432
							guar.WkPhone=FieldParser.PhoneParse(seg.GetFieldComponent(itemOrder,5)+seg.GetFieldComponent(itemOrder,6));
							continue;
						}
						//either no component 2 or left blank, process the old way
						guar.WkPhone=FieldParser.PhoneParse(seg.GetFieldComponent(itemOrder));
						continue;
					default:
						continue;
				}
			}
			if(isNewGuar) {
				if(guar.PatNum==0) {
					guarOld=guar.Copy();
					guar.PatNum=Patients.Insert(guar,false);
					SecurityLogs.MakeLogEntry(Permissions.PatientCreate,guar.PatNum,"Created from HL7.",LogSources.HL7);
					guar.Guarantor=guar.PatNum;
					Patients.Update(guar,guarOld);
				}
				else {
					guar.PatNum=Patients.Insert(guar,true);
					SecurityLogs.MakeLogEntry(Permissions.PatientCreate,guar.PatNum,"Created from HL7.",LogSources.HL7);
				}
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted patient "+guar.GetNameFLnoPref()+" when processing a GT1 segment.",EventLogEntryType.Information);
				}
			}
			else {
				Patients.Update(guar,guarOld);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated patient "+guar.GetNameFLnoPref()+" when processing a GT1 segment.",EventLogEntryType.Information);
				}
			}
			#endregion Update Guarantor Data
			pat.Guarantor=guar.PatNum;
			//store external IDs in the oidexternal table if they do not already exist in the table
			string strVerboseMsg="";
			for(int i=0;i<listOids.Count;i++) {
				if(listOids[i].IDExternal==""//not a good external ID OR
					|| listOids[i].rootExternal==""//not a good external root OR
					|| OIDExternals.GetByRootAndExtension(listOids[i].rootExternal,listOids[i].IDExternal)!=null)//already exists in the oidexternal table
				{
					continue;
				}
				listOids[i].IDInternal=guar.PatNum;
				OIDExternals.Insert(listOids[i]);
				strVerboseMsg+="\r\nExternal patient ID: "+listOids[i].IDExternal+", External root: "+listOids[i].rootExternal;
			}
			if(_isVerboseLogging && strVerboseMsg.Length>0) {
				EventLog.WriteEntry("OpenDentHL7","Added the following external patient ID(s) to the oidexternals table due to an incoming GT1 segment for PatNum: "+guar.PatNum+"."
					+strVerboseMsg+".",EventLogEntryType.Information);
			}
			return;
		}

		//public static void ProcessIN1() {
		//	return;
		//}

		public static void ProcessMSA(HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			int ackCodeOrder=0;
			int msgControlIdOrder=0;
			//find position of AckCode in segDef for MSA seg
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				if(segDef.hl7DefFields[i].FieldName=="ackCode") {
					ackCodeOrder=segDef.hl7DefFields[i].OrdinalPos;
				}
				if(segDef.hl7DefFields[i].FieldName=="messageControlId") {
					msgControlIdOrder=segDef.hl7DefFields[i].OrdinalPos;
				}
			}
			if(ackCodeOrder==0) {//no ackCode defined for this def of MSA, do nothing with it?
				return;
			}
			if(msgControlIdOrder==0) {//no messageControlId defined for this def of MSA, do nothing with it?
				return;
			}
			//set msg.AckCode to value in position located in def of ackcode in seg
			msg.AckCode=seg.GetFieldComponent(ackCodeOrder).ToString();
			msg.ControlId=seg.GetFieldComponent(msgControlIdOrder).ToString();
		}

		public static void ProcessMSH(HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			int msgControlIdOrder=0;
			//find position of messageControlId in segDef for MSH seg
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				if(segDef.hl7DefFields[i].FieldName=="messageControlId") {
					msgControlIdOrder=segDef.hl7DefFields[i].OrdinalPos;
					continue;
				}
			}
			if(msgControlIdOrder==0) {
				return;
			}
			msg.ControlId=seg.GetFieldComponent(msgControlIdOrder).ToString();
		}
		
		///<summary>So far this is only used in SRM messages and saves data to the appointment note field.  If apt is null this does nothing.  The note in the NTE segment will be appended to the existing appointment note unless the existing note already contains the exact note we are attempting to append.</summary>
		public static void ProcessNTE(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			char escapeChar='\\';
			if(msg.Delimiters.Length>2) {//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default \
				escapeChar=msg.Delimiters[2];
			}
			if(apt==null) {
				return;
			}
			if(apt.PatNum!=pat.PatNum) {
				throw new Exception("Appointment does not match patient "+pat.GetNameFLnoPref()+", apt.PatNum: "+apt.PatNum.ToString()+", pat.PatNum: "+pat.PatNum.ToString());
			}
			string strAptNote="";
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int intItemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "apt.Note":
						strAptNote=FieldParser.StringNewLineParse(seg.GetFieldComponent(intItemOrder),escapeChar);
						continue;
					default:
						continue;
				}
			}
			//if the existing note contains the exact text of the note in the NTE segment, don't append it again
			//replace the \r's and then \n's with a blank string before comparing to eliminate inconsistencies in new line characters
			if(apt.Note==null) {//just in case a new appointment gets here and has not been inserted yet, should never happen
				apt.Note="";
			}
			if(apt.Note.Replace("\r\n","\n").Replace("\r","\n").Contains(strAptNote.Replace("\r\n","\n").Replace("\r","\n"))) {
				return;
			}
			Appointment aptOld=apt.Copy();
			if(apt.Note.Length>0) {//if the existing note already contains some text, add a new line before appending the note from the NTE segment
				apt.Note+="\r\n";
			}
			apt.Note+=strAptNote;
			Appointments.Update(apt,aptOld);
			_aptProcessed=apt;
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated appointment for patient "+pat.GetNameFLnoPref()+" due to an incoming NTE segment.",EventLogEntryType.Information);
			}
			return;
		}

		public static void ProcessOBX(Patient pat,HL7DefSegment segDef,SegmentHL7 seg) {
			long rxnorm=0;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int intItemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "medicationRxNorm":
						if(seg.GetFieldComponent(intItemOrder,2).ToLower()!="rxnorm") {
							//if not an RXNORM code, do nothing.  Only RXNORM codes are currently supported
							EventLog.WriteEntry("OpenDentHL7","A medication was not added for patient "+pat.GetNameFLnoPref()
								+".  Only RxNorm codes are currently allowed in the OBX segment.  The code system name provided was "
								+seg.GetFieldComponent(intItemOrder,2)+".",EventLogEntryType.Information);
							return;
						}
						try {
							rxnorm=PIn.Long(seg.GetFieldComponent(intItemOrder,0));
						}
						catch(Exception ex) {//PIn.Long throws an exception if converting to an Int64 fails
							ex.DoNothing();
							//do nothing, rxnorm will remain 0
						}
						continue;
					default:
						continue;
				}
			}
			if(rxnorm==0) {
				EventLog.WriteEntry("OpenDentHL7","A medication was not added for patient "
					+pat.GetNameFLnoPref()+".  The RxNorm code supplied in the OBX segment was invalid.",EventLogEntryType.Information);
				return;//not able to enter this medication if not given a valid rxnorm
			}
			Medication medCur=Medications.GetMedicationFromDbByRxCui(rxnorm);//an RxNorm could be attached to multiple medications, we will just add the first one we come to
			if(medCur==null) {
				EventLog.WriteEntry("OpenDentHL7","A medication was not added for patient "+pat.GetNameFLnoPref()
					+".  There is not a medication in the database with the RxNorm code of "+rxnorm.ToString()+".",EventLogEntryType.Information);
				return;
			}
			List<MedicationPat> listMedPatsCur=MedicationPats.Refresh(pat.PatNum,false);
			for(int i=0;i<listMedPatsCur.Count;i++) {
				if(listMedPatsCur[i].MedicationNum==medCur.MedicationNum) {
					return;//this patient already has this medication recorded and active
				}
			}
			MedicationPat medpatCur=new MedicationPat();
			medpatCur.PatNum=pat.PatNum;
			medpatCur.MedicationNum=medCur.MedicationNum;
			medpatCur.ProvNum=pat.PriProv;
			medpatCur.RxCui=medCur.RxCui;
			MedicationPats.Insert(medpatCur);
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Inserted a new medication for patient "+pat.GetNameFLnoPref()+" due to an incoming OBX segment.",EventLogEntryType.Information);
			}
			return;
		}

		//public static void ProcessPD1() {
		//	return;
		//}

		public static void ProcessPID(Patient pat,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			Patient patOld=pat.Copy();
			char escapeChar='\\';
			if(msg.Delimiters.Length>2) {//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default \
				escapeChar=msg.Delimiters[2];
			}
			char subcompChar='&';
			if(msg.Delimiters.Length>3) {//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default &
				subcompChar=msg.Delimiters[3];
			}
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "pat.addressCityStateZip":
						pat.Address=seg.GetFieldComponent(itemOrder,0);
						pat.Address2=seg.GetFieldComponent(itemOrder,1);
						pat.City=seg.GetFieldComponent(itemOrder,2);
						pat.State=seg.GetFieldComponent(itemOrder,3);
						pat.Zip=seg.GetFieldComponent(itemOrder,4);
						string strAddrNote=FieldParser.StringNewLineParse(seg.GetFieldComponent(itemOrder,19),escapeChar);
						if(strAddrNote!="") {
							pat.AddrNote=strAddrNote;
						}						
						continue;
					case "pat.birthdateTime":
						pat.Birthdate=FieldParser.DateTimeParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "pat.ChartNumber":
						pat.ChartNumber=seg.GetFieldComponent(itemOrder);
						continue;
					case "pat.FeeSched":
						if(Programs.IsEnabled(ProgramName.eClinicalWorks) && ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"FeeSchedulesSetManually")=="1") {
							//if using eCW and FeeSchedulesSetManually
							continue;//do not process fee sched field, manually set by user
						}
						else {
							pat.FeeSched=FieldParser.FeeScheduleParse(seg.GetFieldComponent(itemOrder));
						}
						continue;
					case "pat.Gender":
						pat.Gender=FieldParser.GenderParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "pat.HmPhone":
					case "pat.HmPhone_V2_3":
						if(seg.GetFieldComponent(itemOrder,2)=="" && seg.GetField(itemOrder).ListRepeatFields.Count==0) {//must be eCW, process the old way
							//either no component 2 or left blank, and there are no repetitions, process the old way
							//the first repetition is considered the primary number, but if the primary number is not sent then it will be blank followed by the list of other numbers
							pat.HmPhone=FieldParser.PhoneParse(seg.GetFieldComponent(itemOrder));
						}
						else {
							//XTN data type, repeatable.
							//Component 2 values: PH-Phone, FX-Fax, MD-Modem, CP-Cell Phone, Internet-Internet Address, X.400-X.400 email address, TDD-Tel Device for the Deaf, TTY-Teletypewriter.
							//We will support PH for pat.HmPhone, CP for pat.WirelessPhone, and Internet for pat.Email
							//Component 5 is area code, 6 is number
							//Component 3 will be Email if the type is Internet
							//For v2_3:
								//If component 1 is WPN, skip since that would be the WkPhone and will also be in PID-14.
								//If component 2 is NET, email address will be in component 3 without requiring "Internet" to be in component 2
							//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com
							//Example V2_3: ^WPN^PH^^^503^3635432~^PRN^PH^^^503^6891234~^NET^^someone@somewhere.com (this would set the HmPhone to 5036891234 and the email address to someone@somewhere.com and would ignore the WPN repeat field
							FieldHL7 phField=seg.GetField(itemOrder);
							if(phField==null) {
								continue;
							}
							string strPhUseCode=seg.GetFieldComponent(itemOrder,1);
							string strPhType=seg.GetFieldComponent(itemOrder,2);
							string strPh=seg.GetFieldComponent(itemOrder,5)+seg.GetFieldComponent(itemOrder,6);
							string strEmail=seg.GetFieldComponent(itemOrder,3);
							for(int p=-1;p<phField.ListRepeatFields.Count;p++) {
								if(p>=0) {
									strPhUseCode=phField.ListRepeatFields[p].GetComponentVal(1);
									strPhType=phField.ListRepeatFields[p].GetComponentVal(2);
									strPh=phField.ListRepeatFields[p].GetComponentVal(5)+phField.ListRepeatFields[p].GetComponentVal(6);
									strEmail=phField.ListRepeatFields[p].GetComponentVal(3);
								}
								if(segDef.hl7DefFields[i].FieldName=="pat.HmPhone_V2_3") {
									if(strPhUseCode.ToLower()=="wpn") {//work phone processed in different field
										continue;
									}
									if(strPhUseCode.ToLower()=="net") {//set strPhType to Internet if use code is "NET" for V2_3
										strPhType="Internet";
									}
								}
								switch(strPhType) {
									case "PH":
										pat.HmPhone=FieldParser.PhoneParse(strPh);
										continue;
									case "CP":
										pat.WirelessPhone=FieldParser.PhoneParse(strPh);
										continue;
									case "Internet":
										pat.Email=strEmail;
										continue;
									default:
										continue;
								}
							}
						}
						continue;
					case "pat.nameLFM":
						pat.LName=seg.GetFieldComponent(itemOrder,0);
						pat.FName=seg.GetFieldComponent(itemOrder,1);
						pat.MiddleI=seg.GetFieldComponent(itemOrder,2);
						pat.Title=seg.GetFieldComponent(itemOrder,4);
						continue;
					case "pat.PatNum":
						//pat.PatNum guaranteed to not be 0, a new patient will be inserted if the field component for PatNum is not an int, is 0, or is blank
						//if(pat.PatNum!=0 && pat.PatNum!=PIn.Long(seg.GetFieldComponent(intItemOrder))) {
						//if field component is a valid number and the patient located is not the same as the patient with the PatNum in the segment, then throw the exception, message will fail.
						long patNumFromPid=0;
						try {
							patNumFromPid=PIn.Long(seg.GetFieldComponent(itemOrder));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, patNumFromPID will remain 0
						}
						if(patNumFromPid!=0 && pat.PatNum!=patNumFromPid) {
							throw new Exception("Invalid PatNum");
						}
						continue;
					case "pat.Position":
						pat.Position=FieldParser.MaritalStatusParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "pat.Race":
					case "pat.Race_V2_3":
						//PID.10 is a CWE data type, with 9 defined components.  The first three are CodeValue^CodeDescription^CodeSystem
						//This field can repeat, so we will need to add an entry in the patientrace table for each repetition
						//The race code system is going to be CDCREC and the values match our enum.
						//The code description is not saved, we only require CodeValue and CodeSystem=CDCREC.  Descriptions come from the CDCREC table for display in OD.
						//For V2_3, the CodeSystem is not required.
						//Examples: |2106-3^White^CDCREC~2186-5^Not Hispanic or Latino^CDCREC|, for _V2_3: |2106-3^White~2186-5^Not Hispanic or Latino|
						string strRaceCode=seg.GetFieldComponent(itemOrder,0);
						string strRaceCodeSystem=seg.GetFieldComponent(itemOrder,2);
						//if there aren't 3 components to the race field or if the code system is not CDCREC, then parse the old way by matching to string name, i.e. white=PatientRaceOld.White
						if(strRaceCodeSystem!="CDCREC" && segDef.hl7DefFields[i].FieldName!="pat.Race_V2_3") {//must be eCW, process the old way
							PatientRaceOld patientRaceOld=FieldParser.RaceParseOld(strRaceCode);
							//Converts deprecated PatientRaceOld enum to list of PatRaces, and adds them to the PatientRaces table for the patient.
							PatientRaces.Reconcile(pat.PatNum,PatientRaces.GetPatRacesFromPatientRaceOld(patientRaceOld,pat.PatNum));
						}
						else {
							FieldHL7 fieldRace=seg.GetField(itemOrder);
							if(fieldRace==null) {
								continue;
							}
							List<PatientRace> listPatRaces=new List<PatientRace>();
							PatientRace patRace=new PatientRace(pat.PatNum,strRaceCode);
							listPatRaces.Add(patRace);
							for(int r=0;r<fieldRace.ListRepeatFields.Count;r++) {
								strRaceCode=fieldRace.ListRepeatFields[r].GetComponentVal(0);
								strRaceCodeSystem=fieldRace.ListRepeatFields[r].GetComponentVal(2);
								if(strRaceCodeSystem=="CDCREC" || segDef.hl7DefFields[i].FieldName=="pat.Race_V2_3") {//CDCREC for CodeSystem is only required if not V2_3
									patRace=new PatientRace(pat.PatNum,strRaceCode);
									listPatRaces.Add(patRace);
								}
							}
							PatientRaces.Reconcile(pat.PatNum,listPatRaces);
						}
						continue;
					case "pat.SSN":
						pat.SSN=seg.GetFieldComponent(itemOrder);
						continue;
					case "pat.WkPhone":
						if(seg.GetFieldComponent(itemOrder,2)=="PH") {
							//Component 2 value: PH-Phone
							//Component 5 is area code, 6 is number
							//Example: ^WPN^PH^^^503^3635432
							pat.WkPhone=FieldParser.PhoneParse(seg.GetFieldComponent(itemOrder,5)+seg.GetFieldComponent(itemOrder,6));
							continue;
						}
						//either no component 2 or left blank, process the old way
						pat.WkPhone=FieldParser.PhoneParse(seg.GetFieldComponent(itemOrder));//must be eCW
						continue;
					case "patientIds"://Not used by eCW
						//We will store the ID's from other software in the oidexternal table.  These ID's will be sent in outbound msgs and used to locate a patient for subsequent inbound msgs.
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&OIDType^PI|
						//field component values will be the first repetition, repeats will be in field.ListRepeatFields
						//1234=PatNum, 3=check digit, M11=using check digit scheme, the assigning authority universal ID is the offices oidinternal.IDRoot for IDType Patient,
						//HL7=ID type, PI=identifier type code for: Patient internal identifier (a number that is unique to a patient within an assigning authority).
						FieldHL7 fieldCur=seg.GetField(itemOrder);
						//get the id with the assigning authority equal to the internal OID root stored in their database for a patient object
						string strPatOIDRoot=OIDInternals.GetForType(IdentifierType.Patient).IDRoot;
						if(strPatOIDRoot=="") {
							//if they have not set their internal OID root, we cannot identify which repetition in this field contains the OD PatNum
							EventLog.WriteEntry("OpenDentHL7","Could not process the patientIdList field due to the internal OID for the patient object missing.",EventLogEntryType.Information);
							continue;
						}
						string strPatIdCur="";
						string strPatIdRootCur="";
						string[] arrayPatIdSubComps=fieldCur.GetComponentVal(3).Split(new char[] { subcompChar },StringSplitOptions.None);
						if(arrayPatIdSubComps.Length>1 //so the next line doesn't throw an exception
							&& arrayPatIdSubComps[1].ToLower()!=strPatOIDRoot.ToLower()//not our PatNum
							&& fieldCur.GetComponentVal(4).ToLower()=="pi")//PI=patient internal identifier; a number that is unique to a patient within an assigning authority
						{
							int intCheckDigit=-1;
							try {
								intCheckDigit=PIn.Int(fieldCur.GetComponentVal(1));
							}
							catch(Exception ex) {
								ex.DoNothing();
								//checkDigit will remain -1
							}
							//if using the M10 or M11 check digit algorithm and it passes the respective test, or not using either algorithm, then use the ID
							if((fieldCur.GetComponentVal(2).ToLower()=="m10"
									&& intCheckDigit!=-1
									&& M10CheckDigit(fieldCur.GetComponentVal(0))==intCheckDigit)//using M10 scheme and the check digit is valid and matches calc
								|| (fieldCur.GetComponentVal(2).ToLower()=="m11"
									&& intCheckDigit!=-1
									&& M11CheckDigit(fieldCur.GetComponentVal(0))==intCheckDigit)//using M11 scheme and the check digit is valid and matches calc
								|| (fieldCur.GetComponentVal(2).ToLower()!="m10"
									&& fieldCur.GetComponentVal(2).ToLower()!="m11"))//not using either check digit scheme
							{
								//Either passed the check digit test or not using the M10 or M11 check digit scheme, so trust the ID to be valid
								strPatIdCur=fieldCur.GetComponentVal(0);
								strPatIdRootCur=arrayPatIdSubComps[1];
							}
						}
						OIDExternal oidExtCur=new OIDExternal();
						string verboseMsg="";
						if(strPatIdCur!="" && strPatIdRootCur!="" && OIDExternals.GetByRootAndExtension(strPatIdRootCur,strPatIdCur)==null) {
							oidExtCur.IDType=IdentifierType.Patient;
							oidExtCur.IDInternal=pat.PatNum;
							oidExtCur.IDExternal=strPatIdCur;
							oidExtCur.rootExternal=strPatIdRootCur;
							OIDExternals.Insert(oidExtCur);
							verboseMsg+="\r\nExternal patient ID: "+oidExtCur.IDExternal+", External root: "+oidExtCur.rootExternal;
						}
						for(int r=0;r<fieldCur.ListRepeatFields.Count;r++) {
							arrayPatIdSubComps=fieldCur.ListRepeatFields[r].GetComponentVal(3).Split(new char[] { subcompChar },StringSplitOptions.None);
							if(arrayPatIdSubComps.Length<2) {//there must be at least 2 sub-components in the assigning authority component so we can identify whose ID this is
								continue;
							}
							if(arrayPatIdSubComps[1].ToLower()==strPatOIDRoot.ToLower() //if this is the OD patient OID root
								|| fieldCur.ListRepeatFields[r].GetComponentVal(4).ToLower()!="pi")//or not a patient identifier, then skip this repetition
							{
								continue;
							}
							int intCheckDigit=-1;
							try {
								intCheckDigit=PIn.Int(fieldCur.ListRepeatFields[r].GetComponentVal(1));
							}
							catch(Exception ex) {
								ex.DoNothing();
								//checkDigit will remain -1
							}
							if(fieldCur.ListRepeatFields[r].GetComponentVal(2).ToLower()=="m10"
								&& (intCheckDigit==-1 || M10CheckDigit(fieldCur.ListRepeatFields[r].GetComponentVal(0))!=intCheckDigit))//using M11 scheme and either an invalid check digit was received or it doesn't match our calc
							{
								continue;
							}
							if(fieldCur.ListRepeatFields[r].GetComponentVal(2).ToLower()=="m11"
								&& (intCheckDigit==-1 || M11CheckDigit(fieldCur.ListRepeatFields[r].GetComponentVal(0))!=intCheckDigit))//using M11 scheme and either an invalid check digit was received or it doesn't match our calc
							{
								continue;
							}
							//if not using the M10 or M11 check digit scheme or if the check digit is good, trust the ID in component 0 to be valid and store as IDExternal
							strPatIdCur=fieldCur.ListRepeatFields[r].GetComponentVal(0);
							strPatIdRootCur=arrayPatIdSubComps[1];
							if(strPatIdCur!="" && strPatIdRootCur!="" && OIDExternals.GetByRootAndExtension(strPatIdRootCur,strPatIdCur)==null) {
								oidExtCur=new OIDExternal();
								oidExtCur.IDType=IdentifierType.Patient;
								oidExtCur.IDInternal=pat.PatNum;
								oidExtCur.IDExternal=strPatIdCur;
								oidExtCur.rootExternal=strPatIdRootCur;
								OIDExternals.Insert(oidExtCur);
								verboseMsg+="\r\nExternal patient ID: "+oidExtCur.IDExternal+", External root: "+oidExtCur.rootExternal;
							}
						}
						if(_isVerboseLogging && verboseMsg.Length>0) {
							EventLog.WriteEntry("OpenDentHL7","Added the following external patient ID(s) to the oidexternals table due to an incoming PID segment for PatNum: "
								+pat.PatNum+"."+verboseMsg+".",EventLogEntryType.Information);
						}
						continue;
					default:
						continue;
				}
			}
			Patients.Update(pat,patOld);
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated patient "+pat.GetNameFLnoPref()+" due to an incoming PID segment.",EventLogEntryType.Information);
			}
			return;
		}

		public static void ProcessPR1(Patient pat,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			char escapeChar='\\';
			if(msg.Delimiters.Length>2) {//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default \
				escapeChar=msg.Delimiters[2];
			}
			string strProcNote="";
			string strToothNum="";//could contain the tooth range in comma-delimited list
			string strSurf="";//used for surf/quad/sext/arch
			string strOidExt="";
			string strOidExtRoot="";
			ProcStat procStatus=ProcStat.TP;
			ProcedureCode procCode=null;
			DateTime dateProc=DateTime.MinValue;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "proccode.ProcCode":
						//ProcCode^descript (ignored)^CD2^^^^2014^^Layman Term (ignored)
						//Example: D1351^^CD2^^^^2014
						//must be CDT code (CD2 is code system abbr according to HL7 documentation) and must be version 2014
						if(seg.GetFieldComponent(itemOrder,2).ToLower()!="cd2" || seg.GetFieldComponent(itemOrder,6)!="2014") {
							EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()
								+".  Only CDT codes from code system version 2014 are currently allowed in the PR1 segment.  The code system name provided was "
								+seg.GetFieldComponent(itemOrder,2)+" and the code system version provided was "+seg.GetFieldComponent(itemOrder,6)+".",EventLogEntryType.Information);
							return;
						}
						string strProcCode=seg.GetFieldComponent(itemOrder,0);
						if(!ProcedureCodes.GetContainsKey(strProcCode)) {//code does not exist in proc code list, write entry in event log an return
							EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()
								+".  The code supplied in the PR1 segment does not exist in the database.  The code provided was "+strProcCode+".",EventLogEntryType.Information);
							return;
						}
						procCode=ProcedureCodes.GetOne(strProcCode);
						continue;
					case "proc.Note":
						strProcNote=FieldParser.StringNewLineParse(seg.GetFieldComponent(itemOrder),escapeChar);
						continue;
					case "proc.procDateTime":
						dateProc=FieldParser.DateTimeParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "proc.Status":
						procStatus=FieldParser.ProcStatusParse(seg.GetFieldComponent(itemOrder));
						continue;
					case "proc.toothSurfRange":
						char subcompSeparator='&';
						if(msg.Delimiters.Length>3) {
							subcompSeparator=msg.Delimiters[3];
						}
						string[] listSubComponents=seg.GetFieldComponent(itemOrder).Split(new char[] { subcompSeparator },StringSplitOptions.None);
						if(listSubComponents.Length>0) {
							strToothNum=listSubComponents[0];
						}
						if(listSubComponents.Length>1) {
							strSurf=listSubComponents[1];
						}
						continue;
					case "proc.uniqueId":
						//Id^^UniversalId
						strOidExt=seg.GetFieldComponent(itemOrder,0);
						strOidExtRoot=seg.GetFieldComponent(itemOrder,2);
						if(strOidExt!="" && strOidExtRoot!="" && OIDExternals.GetByRootAndExtension(strOidExtRoot,strOidExt)!=null) {
							//the universal ID and root are already in the oidexternals table, do not insert another procedure, must be a duplicate
							EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()
								+".  The universal ID and root supplied in the PR1 segment refers to an existing procedure."
								+"  Inserting another would result in a duplicate procedure.  The ID provided was "
								+strOidExt+", the root was "+strOidExtRoot+".",EventLogEntryType.Information);
							return;
						}
						continue;
					default:
						continue;
				}
			}
			if(procCode==null) {
				EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()
					+".  No procedure code was defined for a PR1 segment or was missing.",EventLogEntryType.Information);
				return;
			}
			Procedure procCur=new Procedure();
			#region Validate/Convert/Set Treatment Area
			switch(procCode.TreatArea) {
				case TreatmentArea.Arch:
					if(strSurf!="L" && strSurf!="U") {
						EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
							+procCode.ProcCode+" is arch but the arch of "+strSurf+" is invalid.",EventLogEntryType.Information);
						return;
					}
					procCur.Surf=strSurf;
					break;
				case TreatmentArea.Quad:
					if(strSurf!="UL" && strSurf!="UR" && strSurf!="LL" && strSurf!="LR") {
						EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
							+procCode.ProcCode+" is quadrant but the quadrant of "+strSurf+" is invalid.",EventLogEntryType.Information);
						return;
					}
					procCur.Surf=strSurf;
					break;
				case TreatmentArea.Sextant:
					bool isValidSextant=false;
					if(strSurf=="1" || strSurf=="2" || strSurf=="3" || strSurf=="4" || strSurf=="5" || strSurf=="6") {
						isValidSextant=true;
					}
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						if(strSurf=="03" || strSurf=="04" || strSurf=="05" || strSurf=="06" || strSurf=="07" || strSurf=="08") {
							isValidSextant=true;
						}
					}
					if(!isValidSextant) {
						EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
							+procCode.ProcCode+" is sextant but the sextant of "+strSurf+" is invalid.",EventLogEntryType.Information);
						return;
					}
					procCur.Surf=strSurf;
					break;
				case TreatmentArea.Surf:
					if(!Tooth.IsValidEntry(strToothNum)) {
						EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
							+procCode.ProcCode+" is surface but the tooth number of "+strToothNum+" is invalid.",EventLogEntryType.Information);
					}
					procCur.ToothNum=Tooth.FromInternat(strToothNum);
					string strSurfTidy=Tooth.SurfTidyFromDisplayToDb(strSurf,procCur.ToothNum);
					if(strSurfTidy=="" || strSurf=="") {
						EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
							+procCode.ProcCode+" is surface but the surface of "+strSurf+" is invalid.",EventLogEntryType.Information);
						return;
					}
					procCur.Surf=strSurfTidy;
					break;
				case TreatmentArea.Tooth:
					if(!Tooth.IsValidEntry(strToothNum)) {
						EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
							+procCode.ProcCode+" is tooth but the tooth number of "+strToothNum+" is invalid.",EventLogEntryType.Information);
						return;
					}
					procCur.ToothNum=Tooth.FromInternat(strToothNum);
					break;
				case TreatmentArea.ToothRange:
					//break up the list of tooth numbers supplied and validate and convert them into universal tooth numbers for inserting into the db
					string[] listToothNums=strToothNum.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
					for(int i=0;i<listToothNums.Length;i++) {
						if(!Tooth.IsValidEntry(listToothNums[i])) {
							EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "
								+procCode.ProcCode+" is tooth range but the tooth number of "+listToothNums[i]+" is invalid.",EventLogEntryType.Information);
							return;
						}
						if(Tooth.IsPrimary(Tooth.FromInternat(listToothNums[i]))) {
							EventLog.WriteEntry("OpenDentHL7","A procedure was not added for patient "+pat.GetNameFLnoPref()+".  The treatment area for the code "+procCode.ProcCode
								+" is tooth range but the tooth number of "+listToothNums[i]+" is a primary tooth number and therefore not allowed.",EventLogEntryType.Information);
						}
						listToothNums[i]=Tooth.FromInternat(listToothNums[i]);
					}
					procCur.ToothNum=string.Join(",",listToothNums);
					break;
				//We won't validate or use the tooth number or surface fields if the treatment are of the proccode is mouth or none
				case TreatmentArea.None:
				case TreatmentArea.Mouth:
				default:
					break;
			}
			if(procCur.ProcStatus==ProcStat.C && dateProc.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				EventLog.WriteEntry("OpenDentalHL7", "A procedure was not added for patient "+pat.GetNameFLnoPref()+". Procedures are not allowed for future dates.",EventLogEntryType.Information);
				return;
			}
			#endregion Validate/Convert/Set Treatment Area
			//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
			if(procCode.ProcCode!="D9986" && procCode.ProcCode!="D9987") {
				Procedures.SetDateFirstVisit(dateProc,1,pat);//wait until after validating, might not insert the proc, don't return after this point
			}
			procCur.PatNum=pat.PatNum;
			procCur.CodeNum=procCode.CodeNum;
			procCur.ProcDate=dateProc;
			procCur.DateTP=dateProc;
			procCur.ProcStatus=procStatus;//defaults to TP status if not included in msg or not a valid entry
			procCur.ProvNum=procCode.ProvNumDefault;//use default prov if one is set
			if(procCur.ProvNum==0) {//if no proc default prov, use pat's pri or sec prov
				procCur.ProvNum=pat.PriProv;
				if(procCode.IsHygiene && pat.SecProv!=0) {
					procCur.ProvNum=pat.SecProv;
				}
			}
			procCur.Note=strProcNote;
			procCur.ClinicNum=pat.ClinicNum;
			procCur.BaseUnits=procCode.BaseUnits;
			procCur.SiteNum=pat.SiteNum;
			procCur.RevCode=procCode.RevenueCodeDefault;
			procCur.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
			procCur.PlaceService=(PlaceOfService)PrefC.GetInt(PrefName.DefaultProcedurePlaceService);//Default Proc Place of Service for the Practice is used.
			List<PatPlan> listPatPlan=PatPlans.Refresh(pat.PatNum);
			List<InsSub> listSubs=InsSubs.RefreshForFam(Patients.GetFamily(pat.PatNum));
			List<InsPlan> insPlanList=InsPlans.RefreshForSubList(listSubs);
			List<Benefit> listBen=Benefits.Refresh(listPatPlan,listSubs);
			procCur.MedicalCode=procCode.MedicalCode;
			procCur.ProcFee=Procedures.GetProcFee(pat,listPatPlan,listSubs,insPlanList,procCur.CodeNum,procCur.ProvNum,procCur.ClinicNum,
					procCur.MedicalCode);
			procCur.ProcNum=Procedures.Insert(procCur);
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Inserted a new procedure for patient "+pat.GetNameFLnoPref()+" due to an incoming PR1 segment.",EventLogEntryType.Information);
			}
			if(procStatus==ProcStat.C) {
				//Auto-insert default encounter -------------------------------------------------------------------------------------------------------------
				Encounters.InsertDefaultEncounter(procCur.PatNum,procCur.ProvNum,procCur.ProcDate);
				//OrthoProcedures ---------------------------------------------------------------------------------------------------------------------------
				Procedures.SetOrthoProcComplete(procCur,procCode);
				string logText=procCode.ProcCode+", ";
				if(!string.IsNullOrWhiteSpace(procCur.ToothNum)) {
					logText+=Lans.g("Procedures","Teeth")+": "+procCur.ToothNum+", ";
				}
				logText+=Lans.g("Procedures","Fee")+": "+procCur.ProcFee.ToString("F")+", "+procCode.Descript;
				SecurityLogs.MakeLogEntry(Permissions.ProcComplCreate,procCur.PatNum,logText,LogSources.HL7);
			}
			if(ListTools.In(procStatus,ProcStat.C,ProcStat.EC,ProcStat.EO) && procCode.PaintType==ToothPaintingType.Extraction) {
				ToothInitials.SetValue(procCur.PatNum,procCur.ToothNum,ToothInitialType.Missing);
			}
			Procedures.ComputeEstimates(procCur,pat.PatNum,new List<ClaimProc>(),true,new List<InsPlan>(),listPatPlan,listBen,pat.Age,new List<InsSub>());
			if(strOidExt!="" && strOidExtRoot!="") {
				OIDExternal procOidExt=new OIDExternal();
				procOidExt.IDType=IdentifierType.Procedure;
				procOidExt.IDInternal=procCur.ProcNum;
				procOidExt.IDExternal=strOidExt;
				procOidExt.rootExternal=strOidExtRoot;
				OIDExternals.Insert(procOidExt);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Added an external procedure ID to the oidexternals table due to an incoming PR1 segment.\r\nProcNum: "
						+procCur.ProcNum.ToString()+", External problem ID: "+procOidExt.IDExternal+", External root: "+procOidExt.rootExternal+".",EventLogEntryType.Information);
				}
			}
			return;
		}

		public static void ProcessPRB(Patient pat,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			int probActionOrder=-1;
			int probCodeOrder=-1;
			int probStartDateOrder=-1;
			int probStopDateOrder=-1;
			int probUniqueIdOrder=-1;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "problemAction":
						probActionOrder=itemOrder;
						continue;
					case "problemCode":
						probCodeOrder=itemOrder;
						continue;
					case "problemStartDate":
						probStartDateOrder=itemOrder;
						continue;
					case "problemStopDate":
						probStopDateOrder=itemOrder;
						continue;
					case "problemUniqueId":
						probUniqueIdOrder=itemOrder;
						continue;
					default:
						continue;
				}
			}
			//we need these 3 items defined in order to process the problem
			if(probActionOrder<0 || probCodeOrder<0 || probUniqueIdOrder<0) {
				EventLog.WriteEntry("OpenDentHL7","The PRB segment was not processed.  The segment must have an action, code, and unique ID defined.",EventLogEntryType.Information);
				return;
			}
			//get values from defined locations within the segment and validate info
			//We only add or update problems.  Other actions like delete or correct are not yet supported
			if(seg.GetFieldComponent(probActionOrder).ToLower()!="ad" && seg.GetFieldComponent(probActionOrder).ToLower()!="up") {
				EventLog.WriteEntry("OpenDentHL7","The PRB segment was not processed.  The action codes supported are 'AD' for add or 'UP' for update.",EventLogEntryType.Information);
				return;
			}
			long probDefNum=DiseaseDefs.GetNumFromSnomed(PIn.String(seg.GetFieldComponent(probCodeOrder,0)));
			//The problem must be a SNOMEDCT code, identified by the coding system table 0396 value "SNM" in component 3 of the CWE problem code field
			//There must be a disease def setup with the SNOMEDCT code in the problem list or we will ignore this problem
			if(seg.GetFieldComponent(probCodeOrder,2).ToLower()!="snm" || probDefNum==0) {
				EventLog.WriteEntry("OpenDentHL7","The PRB segment was not processed.  "
					+"The code is not attached to an existing problem definition or is not a SNOMEDCT code.",EventLogEntryType.Information);
				return;
			}
			string probIdExternal=seg.GetFieldComponent(probUniqueIdOrder,0);
			string probRootExternal=seg.GetFieldComponent(probUniqueIdOrder,2);
			if(probIdExternal=="" || probRootExternal=="") {
				EventLog.WriteEntry("OpenDentHL7","The PRB segment was not processed. "
					+" The problem does not have a unique ID with assigning authority root.",EventLogEntryType.Information);
				return;
			}
			//If problem external ID and root is in the database, but is used to identify an object other than a problem, do not process the segment
			OIDExternal probOidExt=OIDExternals.GetByRootAndExtension(probRootExternal,probIdExternal);
			if(probOidExt!=null && probOidExt.IDType!=IdentifierType.Problem) {
				EventLog.WriteEntry("OpenDentHL7","The PRB segment was not processed.  "
					+"The problem has a unique ID with assigning authority root that has already been used to identify an object of type "
					+probOidExt.IDType.ToString()+".",EventLogEntryType.Information);
				return;
			}
			long diseaseNum=0;
			if(probOidExt!=null) {//exists in oidexternal table and is of type Problem, so IDInternal is a DiseaseNum
				diseaseNum=probOidExt.IDInternal;					
			}
			Disease probCur=new Disease();
			probCur.DiseaseNum=diseaseNum;//probNum could be 0 if new
			//The problem referenced by the external root and ID is already linked in the oidexternal table, get the problem to update
			//Also make sure the problem linked by oidexternal table is for the patient identified in the PID segment
			if(diseaseNum!=0) {
				probCur=Diseases.GetOne(diseaseNum);
				if(probCur==null || probCur.PatNum!=pat.PatNum) {//should never be null if in the oidexternal table
					EventLog.WriteEntry("OpenDentHL7","The PRB segment was not processed.  "
						+"The problem referenced and the patient in the PID segment do not match.",EventLogEntryType.Information);
					return;
				}
			}
			DateTime dateProbStart=DateTime.MinValue;
			if(probStartDateOrder>-1) {
				dateProbStart=FieldParser.DateTimeParse(seg.GetFieldComponent(probStartDateOrder));
			}
			DateTime dateProbStop=DateTime.MinValue;
			if(probStopDateOrder>-1) {
				dateProbStop=FieldParser.DateTimeParse(seg.GetFieldComponent(probStopDateOrder));
			}
			//The patient may already have an active problem with this DiseaseDefNum, but it is not referenced by this problem GUID
			//Mark the existing problem inactive and add a new one with StartDate of today
			//Add an entry in the oidexternal table that will point the problem GUID to this new problem.
			List<Disease> listProbsForPat=Diseases.GetDiseasesForPatient(pat.PatNum,probDefNum,true);
			int markedInactiveCount=0;
			for(int p=0;p<listProbsForPat.Count;p++) {
				if(listProbsForPat[p].DiseaseNum==diseaseNum) {//probNum may be 0 if there was not an existing problem referenced by the GUID in the message
					continue;
				}
				listProbsForPat[p].ProbStatus=ProblemStatus.Inactive;
				Diseases.Update(listProbsForPat[p]);
				markedInactiveCount++;
			}
			if(_isVerboseLogging && markedInactiveCount>0) {
				EventLog.WriteEntry("OpenDentHL7","Updated "+markedInactiveCount.ToString()+" problems to a status of inactive due to an incoming PRB segment.",EventLogEntryType.Information);
			}
			Disease probOld=probCur.Copy();
			probCur.PatNum=pat.PatNum;
			probCur.DiseaseDefNum=probDefNum;
			probCur.ProbStatus=ProblemStatus.Active;
			probCur.DateStart=dateProbStart;//could be '0001-01-01' if not present or not the correct format, handled by FieldParser.DateTimeParse
			probCur.DateStop=dateProbStop;//could be '0001-01-01' if not present or not the correct format, handled by FieldParser.DateTimeParse
			if(probCur.DiseaseNum==0) {//new problem
				//insert new problem
				probCur.DiseaseNum=Diseases.Insert(probCur);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted a new problem for patient "+pat.GetNameFLnoPref()+" due to an incoming PRB segment.",EventLogEntryType.Information);
				}
				//using DiseaseNum from newly inserted problem, link to the external GUID and root in the oidexternals table
				probOidExt=new OIDExternal();
				probOidExt.IDType=IdentifierType.Problem;
				probOidExt.IDInternal=probCur.DiseaseNum;
				probOidExt.IDExternal=probIdExternal;
				probOidExt.rootExternal=probRootExternal;
				OIDExternals.Insert(probOidExt);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Added an external problem ID to the oidexternals table due to an incoming PRB segment.\r\nDiseaesNum: "
						+probCur.DiseaseNum.ToString()+", External problem ID: "+probOidExt.IDExternal+", External root: "+probOidExt.rootExternal+".",EventLogEntryType.Information);
				}
			}
			else {//the segment is for an existing problem, update fields if necessary
				Diseases.Update(probCur,probOld);
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated an existing problem for patient "+pat.GetNameFLnoPref()+" due to an incoming PRB segment.",EventLogEntryType.Information);
				}
			}
			return;
		}

		///<summary>apt could be null if the PV1 segment is from a message that does not refer to an appointment like an ADT or PPR message or if it is from an SIU message and is for a new appointment.  If apt is null, do not update any of the apt fields, like clinic or provider.</summary>
		public static void ProcessPV1(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg) {
			Appointment aptOld=null;
			if(apt!=null) {
				aptOld=apt.Copy();
			}
			Patient patOld=pat.Copy();
			string securitylogText="";
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "pat.GradeLevel":
						int intGradeLevel=0;
						try {
							intGradeLevel=PIn.Int(seg.GetFieldComponent(itemOrder));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//if parsing field to int fails, do nothing
							continue;
						}
						if(intGradeLevel<Enum.GetNames(typeof(PatientGrade)).Length) {//if parsed value is outside the range of enum, do nothing with the data
							pat.GradeLevel=(PatientGrade)intGradeLevel;//0=Unknown,1-12=first-twelfth, 13=PrenatalWIC, 14=PreK, 15=Kindergarten, 16=Other
						}
						continue;
					case "pat.location":
					case "pat.location_V2_3":
						//Example: ClinicDescript^OpName^^PracticeTitle^^c  (c for clinic), for V2_3: ^ClinicDescript^^PracticeTitle^^c (ClinicDescript in component 1 instead of 0)
						int componentIndex=0;
						if(segDef.hl7DefFields[i].FieldName=="pat.location_V2_3") {
							componentIndex++;
						}
						long clinicNum=Clinics.GetByDesc(seg.GetFieldComponent(itemOrder,componentIndex));//0 if field is blank or description doesn't match clinic description in the db
						if(clinicNum==0) {
							continue;//do nothing, either no clinic description in the message or no matching clinic found
						}
						pat.ClinicNum=clinicNum;
						if(apt!=null) {
							apt.ClinicNum=clinicNum;//the apt.ClinicNum may be set based on a different segment, like the AIL segment
						}
						continue;
					case "prov.provIdName":
					case "prov.provIdName_V2_3":
					case "prov.provIdNameLFM":
					case "prov.provIdNameLFM_V2_3":
						long provNum=0;
						if(_isEcwHL7Def) {//uses prov.EcwID
							provNum=FieldParser.ProvProcessEcw(seg.GetField(itemOrder));
						}
						else {
							provNum=FieldParser.ProvParse(seg.GetField(itemOrder),SegmentNameHL7.PV1,_isVerboseLogging,segDef.hl7DefFields[i].FieldName.EndsWith("_V2_3"));
						}
						if(provNum==0) {//This segment didn't have valid provider information in it, so do nothing
							continue;
						}
						if(apt!=null) {//We will set the appt.ProvNum (dentist) to the provider located in the PV1 segment, but this may be changed if the AIG or AIP segments are included
							apt.ProvNum=provNum;
						}
						if(pat.PriProv!=provNum) {
							pat.PriProv=provNum;
							securitylogText="Primary provider changed automatically from "+(pat.PriProv==0?"'blank'":Providers.GetLongDesc(pat.PriProv))+" to "
								+Providers.GetLongDesc(provNum)+" due to an indound HL7 message PV1 segment.";
						}
						continue;
					case "pat.site":
						//Example: |West Salem Elementary^^^^^s| ('s' for site)
						if(seg.GetFieldComponent(itemOrder,5).ToLower()!="s") {//LocationType is 's' for site
							continue;//not a site description, do nothing
						}
						long siteNum=Sites.FindMatchSiteNum(seg.GetFieldComponent(itemOrder,0));//0 if component is blank, -1 if no matching site description in the db
						if(siteNum>0) {
							pat.SiteNum=siteNum;
						}
						continue;
					case "pat.Urgency":
						int intPatUrgency=-1;
						try {
							intPatUrgency=PIn.Int(seg.GetFieldComponent(itemOrder));//if field is empty, PIn.Int will return 0 which will be the Unknown default urgency
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, patUrgency will remain -1
						}
						if(intPatUrgency>-1 && intPatUrgency<4) {//only cast to enum if 0-3
							pat.Urgency=(TreatmentUrgency)intPatUrgency;
						}
						continue;
					default:
						continue;
				}
			}
			Patients.Update(pat,patOld);
			if(!string.IsNullOrEmpty(securitylogText)) {
				SecurityLogs.MakeLogEntry(Permissions.PatPriProvEdit,pat.PatNum,securitylogText);
			}
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated patient "+pat.GetNameFLnoPref()+" due to an incoming PV1 segment.",EventLogEntryType.Information);
			}
			if(apt!=null) {
				Appointments.Update(apt,aptOld);
				_aptProcessed=apt;
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated appointment for patient "+pat.GetNameFLnoPref()+" due to an incoming PV1 segment.",
						EventLogEntryType.Information);
				}
			}
			return;
		}

		///<summary>Returns AptNum of the incoming appointment.  apt was found using the apt.AptNum field of the SCH segment, but can be null if it's a new appointment.  Used for eCW and other interfaces where OD is not the filler application.  When OD is not the filler application, we allow appointments to be created by the interfaced software and communicated to OD with an SIU message.</summary>
		public static long ProcessSCH(Patient pat,Appointment apt,HL7DefSegment segDef,SegmentHL7 seg,MessageHL7 msg) {
			if(pat.FName=="" || pat.LName=="") {
				throw new Exception("Appointment not processed due to missing patient first or last name. PatNum:"+pat.PatNum.ToString());
			}
			char escapeChar='\\';
			if(msg.Delimiters.Length>2) {//it is possible they did not send all 4 of the delimiter chars, in which case we will use the default \
				escapeChar=msg.Delimiters[2];
			}
			string strAptNote="";
			double aptLength=0;
			long aptNum=0;
			DateTime aptStart=DateTime.MinValue;
			DateTime aptStop=DateTime.MinValue;
			for(int i=0;i<segDef.hl7DefFields.Count;i++) {
				int itemOrder=segDef.hl7DefFields[i].OrdinalPos;
				switch(segDef.hl7DefFields[i].FieldName) {
					case "apt.AptNum":
						try {
							aptNum=PIn.Long(seg.GetFieldComponent(itemOrder));
						}
						catch(Exception ex) {
							ex.DoNothing();
							//do nothing, aptNum will remain 0
						}
						if(apt!=null && apt.AptNum!=aptNum) {
							//an appointment was located from the inbound message, but the AptNum on the appointment is not the same as the AptNum in this SCH segment (should never happen)
							throw new Exception("Invalid appointment number.");
						}
						continue;
					case "apt.lengthStartEnd":
						aptLength=FieldParser.SecondsToMinutes(seg.GetFieldComponent(itemOrder,2));
						aptStart=FieldParser.DateTimeParse(seg.GetFieldComponent(itemOrder,3));
						aptStop=FieldParser.DateTimeParse(seg.GetFieldComponent(itemOrder,4));
						continue;
					case "apt.Note":
						strAptNote=FieldParser.StringNewLineParse(seg.GetFieldComponent(itemOrder),escapeChar);
						continue;
					default:
						continue;
				}
			}
			Appointment aptOld=null;
			bool isNewApt=(apt==null);
			if(isNewApt) {
				apt=new Appointment();
				apt.AptNum=aptNum;
				apt.PatNum=pat.PatNum;
				apt.AptStatus=ApptStatus.Scheduled;
				apt.Note="";
			}
			else {
				aptOld=apt.Copy();
			}
			if(apt.PatNum!=pat.PatNum) {//we can't process this message because wrong patnum.
				throw new Exception("Appointment does not match patient "+pat.GetNameFLnoPref()+", apt.PatNum: "+apt.PatNum.ToString()+", pat.PatNum: "+pat.PatNum.ToString());
			}
			bool isBreakApt=false;
			if(msg.EventType==EventTypeHL7.S15) { //insert with broken status, this is an appointment cancellation message
				apt.AptStatus=ApptStatus.Broken;
				string aptNoteBroken="Appointment set broken via HL7.";
				if(!apt.Note.Contains(aptNoteBroken)) {
					isBreakApt=true;
					apt.Note+=(!string.IsNullOrEmpty(apt.Note)?"\r\n":"")+aptNoteBroken;
				}
			}
			//if the existing note contains the exact text of the note in the SCH segment, don't append it again
			//replace the \r's and then \n's with a blank string before comparing to eliminate inconsistencies in new line characters
			if(!apt.Note.Replace("\r","").Replace("\n","").Contains(strAptNote.Replace("\r","").Replace("\n",""))) {
				if(apt.Note.Length>0) {//if the existing note already contains some text, add a new line before appending the note from the SCH segment
					apt.Note+="\r\n";
				}
				apt.Note+=strAptNote;
			}
			string pattern;
			//If aptStop is MinValue we know that stop time was not sent or was not in the correct format so try to use the duration field.
			if(aptStop==DateTime.MinValue && aptLength!=0) {//Stop time is optional.  If not included we will use the duration field to calculate pattern.
				pattern=FieldParser.ProcessPattern(aptStart,aptStart.AddMinutes(aptLength));
			}
			else {//We received a good stop time or stop time is MinValue but we don't have a good aptLength so ProcessPattern will return the apt length or the default 5 minutes
				pattern=FieldParser.ProcessPattern(aptStart,aptStop);
			}
			apt.AptDateTime=aptStart;
			apt.Pattern=pattern;
			apt.ProvNum=pat.PriProv;//Set apt.ProvNum to the patient's primary provider.  This may change after processing the AIG or PV1 segments, but set here in case those segs are missing.
			if(isNewApt) {
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted appointment "+(isBreakApt?"with broken status ":"")+"for "+pat.GetNameFLnoPref()+" due to an incoming SCH segment.",EventLogEntryType.Information);
				}
				Appointments.InsertIncludeAptNum(apt,true);
			}
			else {
				if(_isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated appointment for "+pat.GetNameFLnoPref()+" due to an incoming SCH segment.",EventLogEntryType.Information);
				}
				Appointments.Update(apt,aptOld);
			}
			_hl7MsgCur.AptNum=apt.AptNum;
			HL7Msgs.Update(_hl7MsgCur);
			if(_isVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Updated hl7msg to include AptNum for "+pat.GetNameFLnoPref()+" due to an incoming SCH segment.",EventLogEntryType.Information);
			}
			_aptProcessed=apt;
			return aptNum;
		}

		///<summary>This uses the Mod11 check digit algorithm to calculate and return the checkDigit for the supplied objectID.
		///<para>(see \\SERVERFILES\storage\OPEN DENTAL\Programmers Documents\Standards (X12, ADA, etc)\HL7\Version2.6\V26_CH02A_DataTypes_M4_JAN2007.doc page 23)</para>
		///<para>If objectId is an empty string, this will return -1.</para>
		///<para>Returns the calculated check digit to compare to check digit received or use in constructing a message.</para>
		///<para>M11 algorithm: d	=	digit of number starting from units digit, followed by 10’s position, followed by 100’s position, etc.</para>
		///<para>w	=	weight of digit position starting with the units position, followed by 10’s position, followed by 100’s position etc.</para>
		///<para>Values for w = 2,3,4,5,6,7,2,3,4,5,6,7,etc. (repeats for each group of 6 digits)</para>
		///<para>c	=	check digit</para>
		///<para>(Step 1) m = sum of (d*w) starting at units position for d=digit value starting with units position to highest order,</para>
		///<para>for w=weight value from 2 to 7 for every 6 positions starting with units digit</para>
		///<para>(Step 2) c1 = m mod 11</para>
		///<para>(Step 3) if c1 = 0 then c1 = 1</para>
		///<para>(Step 4) c = (11 - c1) mod 10</para>
		///<para>Example: 1234567, check digit is 4; m=(7*2)+(6*3)+(5*4)+(4*5)+(3*6)+(2*7)+(1*2)=106; 106 mod 11=7; (11-7) mod 10=4.</para></summary>
		public static int M11CheckDigit(string objectId) {
			if(objectId=="") {
				return -1;
			}
			//Contains the characters of the patId, converted to integers, in reverse order
			int[] arrayIds=new int[objectId.Length];
			for(int i=0;i<objectId.Length;i++) {
				try {
					arrayIds[objectId.Length-(1+i)]=PIn.Int(objectId[i].ToString());
				}
				catch {
					//if a character in the patId is not an integer, return false
					return -1;
				}
			}
			int checkDigitCalc=0;
			int[] arrayWeights=new int[6] { 2,3,4,5,6,7 };
			//Step 1
			for(int i=0;i<arrayIds.Length;i++) {
				checkDigitCalc+=arrayIds[i]*arrayWeights[i%6];
			}
			//Step 2
			checkDigitCalc=checkDigitCalc%11;
			//Step 3
			if(checkDigitCalc==0) {
				checkDigitCalc=1;
			}
			//Step 4
			checkDigitCalc=(11-checkDigitCalc)%10;
			return checkDigitCalc;
		}

		///<summary>This uses the Mod10 check digit algorithm to calculate and return the checkDigit for the supplied objectId.
		///<para>(see \\SERVERFILES\storage\OPEN DENTAL\Programmers Documents\Standards (X12, ADA, etc)\HL7\Version2.6\V26_CH02A_DataTypes_M4_JAN2007.doc page 22)</para>
		///<para>If objectId is an empty string, this will return -1.</para>
		///<para>Returns the calculated check digit to compare to check digit received or use in constructing a message.</para>
		///<para>M10 algorithm: (Step 1) Take the odd positioned digits starting from the right and append them into a single number.</para>
		///<para>(Step 2) Multiply value from step 1 by 2.</para>
		///<para>(Step 3) Take the even digit positions starting from the right (of the original number) and prepend these to the value from step 2.</para>
		///<para>(Step 4) Add all of the digits together and subtract the total from the next highest multiple of 10.  If it is a multiple of 10, check digit is 0.</para>
		///<para>Example: 12345, check digit is 5; Step 1: 531; Step 2: 531*2=1062; Step 3: 421062; Step 4: 4+2+1+0+6+2=15; 20-15=5.</para></summary>
		public static int M10CheckDigit(string objectId) {
			if(objectId=="") {
				return -1;
			}
			//idIntsOddPos will contain the odd positioned chars of patId, converted to ints, the right-most positioned char (i.e. the 1's digit) will be in pos 0 of the array
			//char[] idIntsOddPos=new char[patId.Length/2+(patId.Length%2)];
			string idIntsOddPos="";
			int step4Tot=0;
			char[] arrayPatIdCharsReversed=objectId.ToCharArray();
			Array.Reverse(arrayPatIdCharsReversed);
			for(int i=0;i<arrayPatIdCharsReversed.Length;i++) {
				try {
					if(i%2==0) {//if i is even then odd positioned digit. e.g. patId=1234, patIdCharsReversed=4321, if i=0, digit=4 which is the first digit from the right, so odd positioned
						idIntsOddPos+=arrayPatIdCharsReversed[i];
					}
					else {//if i is odd then even positioned digit. e.g. patId=1234, patIdCharsReversed=4321, if i=1, digit=3 which is the second digit from the right, so even positioned
						step4Tot+=PIn.Int(arrayPatIdCharsReversed[i].ToString());
					}
				}
				catch {
					return -1;
				}
			}
			string oddDigitsDoubled;
			try {
				oddDigitsDoubled=(PIn.Long(idIntsOddPos)*2).ToString();
			}
			catch {
				//if unable to convert the odd positioned chars to a long or any other error in above calculation and translation, return false.
				return -1;
			}
			for(int i=0;i<oddDigitsDoubled.Length;i++) {
				try {
					step4Tot+=PIn.Int(oddDigitsDoubled[i].ToString());
				}
				catch {
					//if any of the chars can't be converted to integers (shouldn't be possible) return false.
					return -1;
				}
			}
			int checkDigitCalc=0;
			if(step4Tot%10!=0) {
				checkDigitCalc=10-step4Tot%10;
			}
			return checkDigitCalc;
		}
	}
}
