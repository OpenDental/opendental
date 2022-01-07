using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	///<summary>This is the engine that will construct our outgoing HL7 message fields.</summary>
	public class FieldConstructor {
		private static bool _isEcwDef;
		
		///<summary>Sends null values in for objects not required.  GenerateField will return an empty string if a field requires an object and that
		///object is null.</summary>
		public static string GenerateFieldADT(HL7Def def,string fieldName,Patient pat,Provider prov,Patient guar,int sequenceNum,EventTypeHL7 eventType,
			SegmentNameHL7 segName)
		{
			return GenerateField(def,fieldName,MessageTypeHL7.ADT,pat,prov,null,guar,null,sequenceNum,eventType,null,null,MessageStructureHL7.ADT_A01,segName);
		}
		
		///<summary>Sends null values in for objects not required.  GenerateField will return an empty string if a field requires an object and that
		///object is null.</summary>
		public static string GenerateFieldSIU(HL7Def def,string fieldName,Patient pat,Provider prov,Patient guar,Appointment apt,int sequenceNum,
			EventTypeHL7 eventType,SegmentNameHL7 segName)
		{
			return GenerateField(def,fieldName,MessageTypeHL7.SIU,pat,prov,null,guar,apt,sequenceNum,eventType,null,null,MessageStructureHL7.SIU_S12,segName);
		}
		
		///<summary>Sends null values in for objects not required.  GenerateField will return an empty string if a field requires an object and that
		///object is null.</summary>
		public static string GenerateFieldSRR(HL7Def def,string fieldName,Patient pat,Provider prov,Appointment apt,int sequenceNum,
			EventTypeHL7 eventType,SegmentNameHL7 segName)
		{
			return GenerateField(def,fieldName,MessageTypeHL7.SRR,pat,prov,null,null,apt,sequenceNum,eventType,null,null,MessageStructureHL7.SRR_S01,segName);
		}

		///<summary>apt, guar, proc, prov, pdfDescription, pdfDataString, patplanCur, inssubCur, insplanCur, carrierCur, and patSub can be null and will
		///return an empty string if a field requires that object</summary>
		public static string GenerateField(HL7Def def,string fieldName,MessageTypeHL7 msgType,Patient pat,Provider prov,Procedure proc,Patient guar,
			Appointment apt,int sequenceNum,EventTypeHL7 eventType,string pdfDescription,string pdfDataString,MessageStructureHL7 msgStructure,
			SegmentNameHL7 segName)
		{
			string retval="";
			if(def.InternalType==HL7InternalType.eCWFull
				|| def.InternalType==HL7InternalType.eCWTight
				|| def.InternalType==HL7InternalType.eCWStandalone)
			{
				_isEcwDef=true;
			}
			//big long list of fieldnames that we support
			switch(fieldName) {
				#region Appointment
				case "apt.AptDateTime":
					if(apt==null) {
						return "";
					}
					return gDTM(apt.AptDateTime,14);
				case "apt.AptNum":
					if(apt==null) {
						return "";
					}
					if(_isEcwDef) {
						return apt.AptNum.ToString();
					}
					//We will use AptNum, with the OIDRoot for an appt object (set in oidinternal), and ID Type 'HL7'
					OIDInternal aptOid=OIDInternals.GetForType(IdentifierType.Appointment);
					string aptOidRoot="";
					if(aptOid!=null) {
						aptOidRoot=aptOid.IDRoot;
					}
					//For PV1 segments, the data type is a CX, which has the AptNum, check digit, check digit scheme, assigning authority - &universalID&universalIDType, IDType
					//We will use the check digit scheme M11, their appt OID with "HL7" as the type for assigning authority, and the ID type is VN - Visit Number
					//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.6&HL7^VN|
					if(segName==SegmentNameHL7.PV1) {
						string strCheckDigit=MessageParser.M11CheckDigit(apt.AptNum.ToString()).ToString();
						string strCheckDigitScheme="M11";
						if(strCheckDigit=="-1") {
							strCheckDigit="";
							strCheckDigitScheme="";
						}
						return gConcat(def.ComponentSeparator,apt.AptNum.ToString(),strCheckDigit,strCheckDigitScheme,gConcat(def.SubcomponentSeparator,"",aptOidRoot,"HL7"),"VN");
					}
					//For segments other than PV1 (currently SCH or ARQ segments) the data type is EI
					//Example: |1234^^2.16.840.1.113883.3.4337.1486.6566.6^HL7|
					return gConcat(def.ComponentSeparator,apt.AptNum.ToString(),"",aptOidRoot,"HL7");
				case "apt.aptStatus":
					if(apt==null) {
						return "";
					}
					if(apt.AptStatus==ApptStatus.Complete) {
						return "Complete";
					}
					if(apt.AptStatus==ApptStatus.UnschedList || apt.AptStatus==ApptStatus.Broken) {
						return "Cancelled";
					}
					if(eventType==EventTypeHL7.S17) {//S17 is event type for outbound SIU messages when we delete an appointment
						return "Deleted";
					}
					//apt.AptStatus==ApptStatus.Scheduled or apt.AptStatus==ApptStatus.ASAP or other status that triggers an SCH segment
					return "Booked";
				case "apt.confirmStatus":
					if(apt==null) {
						return "";
					}
					//Example: |^Appointment Confirmed|
					return gConcat(def.ComponentSeparator,"",Defs.GetName(DefCat.ApptConfirmed,apt.Confirmed));//this will return an empty string if apt.Confirmed is 0 or invalid
				case "apt.endAptDateTime":
					if(apt==null) {
						return "";
					}
					return gDTM(apt.AptDateTime.AddMinutes(5*apt.Pattern.Length),14);
				case "apt.externalAptID":
					//EntityID^NamespaceID^UniversalID^UniversalIDType
					//Example: |12345^^OtherSoftware.Root^|
					if(apt==null) {
						return "";
					}
					List<OIDExternal> listAptOidsExt=OIDExternals.GetByInternalIDAndType(apt.AptNum,IdentifierType.Appointment);
					if(listAptOidsExt.Count==0) {
						return "";
					}
					return gConcat(def.ComponentSeparator,listAptOidsExt[0].IDExternal,"",listAptOidsExt[0].rootExternal,"");
				case "apt.location":
					//Point of Care^Room^^Facility^^Person Location Type
					//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
					//use operatory and clinic from appt
					if(apt==null) {
						return "";
					}
					string aptClinicDescript=Clinics.GetDesc(apt.ClinicNum);
					Operatory opCur=Operatories.GetOperatory(apt.Op);
					string opName="";
					if(opCur!=null) {
						opName=opCur.OpName;
					}
					string practiceName=PrefC.GetString(PrefName.PracticeTitle);
					return gConcat(def.ComponentSeparator,aptClinicDescript,opName,"",def.SubcomponentSeparator+practiceName,"","C");//all of these could be empty strings and it works fine
				case "apt.length":
					//Example: 60^min&&ANS+, ANS+ is the name of the coding system
					if(apt==null) {
						return "";
					}
					return gConcat(def.ComponentSeparator,(5*apt.Pattern.Length).ToString(),gConcat(def.SubcomponentSeparator,"min","","ANS+"));
				case "apt.Note":
					//As in the address note field (see PID.11) we will send '\.br\' (where the '\' is the defined escape char, \ by default) to signal a new line.
					if(apt==null) {
						return "";
					}
					return gNewLines(def.EscapeCharacter,apt.Note);
				case "apt.operatory":
					if(apt==null) {
						return "";
					}
					opCur=Operatories.GetOperatory(apt.Op);
					if(opCur==null) {
						return "";
					}
					return opCur.OpName;
				case "apt.type":
					//Suggested values are Normal - Routine schedule request type, Tentative, or Complete - Request to add a completed appt
					//We will send Normal for all appointment statuses except complete.
					if(apt==null) {
						return "";
					}
					if(apt.AptStatus==ApptStatus.Complete) {
						return "Complete";
					}
					return "Normal";
				case "apt.userOD":
					//The OD user who created/modified the appointment
					if(apt==null) {
						return "";
					}
					//SRR messages are generated from the service in response to SRM messages, therefore we do not have a user logged in.
					if(msgType==MessageTypeHL7.SRR) {
						return "OpenDentalHL7";
					}
					if(Security.CurUser!=null) {
						return Security.CurUser.UserName;
					}
					return "";
				#endregion Appointment
				case "dateTime.Now":
					return gDTM(DateTime.Now,14);
				case "eventType":
					return eventType.ToString();
				#region Guarantor
				case "guar.addressCityStateZip":
					if(guar==null) {
						return "";
					}
					retval=gConcat(def.ComponentSeparator,guar.Address,guar.Address2,guar.City,guar.State,guar.Zip);
					if(!_isEcwDef) {
						//Example: 123 Main St^Apt 1^Dallas^OR^97338^^^^^^^^^^^^^^^Emergency Contact: Mom Test1\.br\Mother\.br\(503)623-3072
						retval=gConcat(def.ComponentSeparator,retval,"","","","","","","","","","","","","","",gNewLines(def.EscapeCharacter,guar.AddrNote));
					}
					return retval;
				case "guar.birthdateTime":
					if(guar==null) {
						return "";
					}
					return gDTM(guar.Birthdate,8);
				case "guar.Gender":
					if(guar==null) {
						return "";
					}
					return gIS(guar);
				case "guar.HmPhone":
					if(guar==null) {
						return "";
					}
					string hmPh=gXTN(guar.HmPhone,10);
					string cPh=gXTN(guar.WirelessPhone,10);
					if(_isEcwDef) {
						return hmPh;
					}
					//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
					//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
					if(hmPh!="") {
						retval=gConcat(def.ComponentSeparator,"","PRN","PH","","",hmPh.Substring(0,3),hmPh.Substring(3));//hmPh guaranteed to be 10 digits if not blank
					}
					if(cPh!="") {
						if(retval!="") {
							retval+=def.RepetitionSeparator;
						}
						retval+=gConcat(def.ComponentSeparator,"","PRN","CP","","",cPh.Substring(0,3),cPh.Substring(3));//cPh guaranteed to be 10 digits if not blank
					}
					if(guar.Email!="") {
						if(retval!="") {
							retval+=def.RepetitionSeparator;
						}
						retval+=gConcat(def.ComponentSeparator,"","PRN","Internet",guar.Email);
					}
					return retval;
				case "guar.nameLFM":
					if(guar==null) {
						return "";
					}
					return gConcat(def.ComponentSeparator,guar.LName,guar.FName,guar.MiddleI);
				case "guar.PatNum":
					if(guar==null) {
						return "";
					}
					return guar.PatNum.ToString();
				case "guar.SSN":
					if(guar==null) {
						return "";
					}
					return guar.SSN;
				case "guar.WkPhone":
					if(guar==null) {
						return "";
					}
					string wkPh=gXTN(guar.WkPhone,10);
					if(_isEcwDef) {
						return wkPh;
					}
					//WPN stands for Work Number, equipment type: PH is Telephone
					//Example: ^WPN^PH^^^503^3635432
					if(wkPh=="") {
						return "";
					}
					return gConcat(def.ComponentSeparator,"","WPN","PH","","",wkPh.Substring(0,3),wkPh.Substring(3));//wkPh guaranteed to be 10 digits if not blank
				case "guarIdList":
					if(guar==null) {
						return "";
					}
					//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&^PI|
					OIDInternal guarOid=OIDInternals.GetForType(IdentifierType.Patient);
					string guarOidRoot="";
					if(guarOid!=null) {
						guarOidRoot=guarOid.IDRoot;
					}
					string guarIdCheckDigitStr=MessageParser.M11CheckDigit(guar.PatNum.ToString()).ToString();
					retval=gConcat(def.ComponentSeparator,guar.PatNum.ToString(),guarIdCheckDigitStr,"M11",def.SubcomponentSeparator+guarOidRoot+def.SubcomponentSeparator+"HL7","PI");
					List<OIDExternal> listGuarOidsExt=OIDExternals.GetByInternalIDAndType(guar.PatNum,IdentifierType.Patient);
					for(int i=0;i<listGuarOidsExt.Count;i++) {
						guarIdCheckDigitStr=MessageParser.M11CheckDigit(listGuarOidsExt[i].IDExternal).ToString();
						if(guarIdCheckDigitStr=="-1") {//could not get a check digit from the external ID, could contain characters that are not numbers
							retval+=def.RepetitionSeparator+gConcat(def.ComponentSeparator,listGuarOidsExt[i].IDExternal,"","",
								def.SubcomponentSeparator+listGuarOidsExt[i].rootExternal+def.SubcomponentSeparator,"PI");
							continue;
						}
						retval+=def.RepetitionSeparator+gConcat(def.ComponentSeparator,listGuarOidsExt[i].IDExternal,guarIdCheckDigitStr,"M11",
							def.SubcomponentSeparator+listGuarOidsExt[i].rootExternal+def.SubcomponentSeparator,"PI");
					}
					return retval;
				#endregion Guarantor
				case "messageControlId":
					return Guid.NewGuid().ToString("N");
				case "messageType":
					return gConcat(def.ComponentSeparator,msgType.ToString(),eventType.ToString(),msgStructure.ToString());
				case "messageTypeNoStruct":
					return gConcat(def.ComponentSeparator,msgType.ToString(),eventType.ToString());
				#region Patient
				case "pat.addressCityStateZip":
					retval=gConcat(def.ComponentSeparator,pat.Address,pat.Address2,pat.City,pat.State,pat.Zip);
					if(!_isEcwDef) {
						//Example: 123 Main St^Apt 1^Dallas^OR^97338^^^^^^^^^^^^^^^Emergency Contact: Mom Test1\.br\Mother\.br\(503)623-3072
						retval=gConcat(def.ComponentSeparator,retval,"","","","","","","","","","","","","","",gNewLines(def.EscapeCharacter,pat.AddrNote));
					}
					return retval;
				case "pat.birthdateTime":
					return gDTM(pat.Birthdate,8);
				case "pat.ChartNumber":
					return pat.ChartNumber;
				case "pat.Gender":
					return gIS(pat);
				case "pat.HmPhone":
				case "pat.HmPhone_V2_3":
					hmPh=gXTN(pat.HmPhone,10);
					cPh=gXTN(pat.WirelessPhone,10);
					if(_isEcwDef) {
						return hmPh;
					}
					//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
					//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
					//For V2_3, email address repeat field will have NET in field 1 and field 2 will be blank instead of Internet
					//V2_3 example: ^PRN^PH^^^503^3635432~^NET^^someone@somewhere.com
					List<string> listReps=new List<string>();
					if(!string.IsNullOrEmpty(hmPh)) {
						listReps.Add(gConcat(def.ComponentSeparator,"","PRN","PH","","",hmPh.Substring(0,3),hmPh.Substring(3)));//hmPh guaranteed to be 10 digits if not blank
					}
					if(!string.IsNullOrEmpty(cPh)) {
						listReps.Add(gConcat(def.ComponentSeparator,"","PRN","CP","","",cPh.Substring(0,3),cPh.Substring(3)));//cPh guaranteed to be 10 digits if not blank
					}
					if(!string.IsNullOrEmpty(pat.Email)) {
						listReps.Add(gConcat(def.ComponentSeparator,"",(fieldName.EndsWith("V2_3")?"NET":"PRN"),(fieldName.EndsWith("V2_3")?"":"Internet"),pat.Email));
					}
					if(listReps.Count>0) {
						return string.Join(def.RepetitionSeparator,listReps);
					}
					return "";
				case "pat.location":
				case "pat.location_V2_3":
					//Point of Care^Room^^Facility^^Person Location Type
					//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic), for V2_3: ^ClinicDescript^^&PracticeTitle^^C
					if(pat.ClinicNum==0) {
						return "";
					}
					string patClinicDescript=Clinics.GetDesc(pat.ClinicNum);
					practiceName=PrefC.GetString(PrefName.PracticeTitle);
					if(fieldName.EndsWith("V2_3")) {
						return gConcat(def.ComponentSeparator,"",patClinicDescript,"",def.SubcomponentSeparator+practiceName,"","C");
					}
					return gConcat(def.ComponentSeparator,patClinicDescript,"","",def.SubcomponentSeparator+practiceName,"","C");
				case "pat.nameLFM":
					return gConcat(def.ComponentSeparator,pat.LName,pat.FName,pat.MiddleI);
				case "pat.PatNum":
					return pat.PatNum.ToString();
				case "pat.Position":
					if(_isEcwDef) {
						return gPos(pat);
					}
					return gPos(pat).Substring(0,1);//S-Single, M-Married, D-Divorced, W-Widowed
				case "pat.Race":
				case "pat.Race_V2_3":
					if(_isEcwDef) {
						return gRaceOld(pat);
					}
					return gRace(pat,def,fieldName=="pat.Race_V2_3");
				case "pat.site":
					//Example: |West Salem Elementary^^^^^S| ('S' for site)
					if(pat.SiteNum==0) {
						return "";
					}
					string patSiteDescript=Sites.GetDescription(pat.SiteNum);
					if(patSiteDescript=="") {
						return "";
					}
					return gConcat(def.ComponentSeparator,patSiteDescript,"","","","","","S");
				case "pat.SSN":
					return pat.SSN??"";
				case "pat.WkPhone":
					if(_isEcwDef) {
						return gXTN(pat.WkPhone,10);
					}
					//WPN stands for Work Number, equipment type: PH is Telephone
					//Example: ^WPN^PH^^^503^3635432
					wkPh=gXTN(pat.WkPhone,10);
					if(wkPh=="") {
						return "";
					}
					return gConcat(def.ComponentSeparator,"","WPN","PH","","",wkPh.Substring(0,3),wkPh.Substring(3));//wkPh guaranteed to be 10 digits if not blank
				case "pat.Urgency":
					//We will send one of the following values retrieved from the patient.Urgency field for treatment urgency: 0-Unknown, 1-NoProblems, 2-NeedsCare, 3-Urgent
					return ((int)pat.Urgency).ToString();
				case "patientIds":
					//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&^PI|
					OIDInternal patOid=OIDInternals.GetForType(IdentifierType.Patient);
					string patOidRoot="";
					if(patOid!=null) {
						patOidRoot=patOid.IDRoot;
					}
					string patIdCheckDigitStr=MessageParser.M11CheckDigit(pat.PatNum.ToString()).ToString();
					retval=gConcat(def.ComponentSeparator,pat.PatNum.ToString(),patIdCheckDigitStr,"M11",def.SubcomponentSeparator+patOidRoot+def.SubcomponentSeparator+"HL7","PI");
					List<OIDExternal> listPatOidsExt=OIDExternals.GetByInternalIDAndType(pat.PatNum,IdentifierType.Patient);
					for(int i=0;i<listPatOidsExt.Count;i++) {
						patIdCheckDigitStr=MessageParser.M11CheckDigit(listPatOidsExt[i].IDExternal).ToString();
						if(patIdCheckDigitStr=="-1") {//could not get a check digit from the external ID, could contain characters that are not numbers
							retval+=def.RepetitionSeparator+gConcat(def.ComponentSeparator,listPatOidsExt[i].IDExternal,"","",
								def.SubcomponentSeparator+listPatOidsExt[i].rootExternal+def.SubcomponentSeparator,"PI");
							continue;
						}
						retval+=def.RepetitionSeparator+gConcat(def.ComponentSeparator,listPatOidsExt[i].IDExternal,patIdCheckDigitStr,"M11",
							def.SubcomponentSeparator+listPatOidsExt[i].rootExternal+def.SubcomponentSeparator,"PI");
					}
					return retval;
				#endregion Patient
				case "pdfDescription":
					if(pdfDescription==null) {
						return "";
					}
					return pdfDescription;
				case "pdfDataAsBase64":
					if(pdfDataString==null) {
						return "";
					}
					else {
						return pdfDataString;
					}
				#region Procedure
				case "proc.DiagnosticCode":
					if(proc==null) {
						return "";
					}
					List<string> listDiagCodes=new List<string>();
					if(proc.DiagnosticCode!=null && proc.DiagnosticCode!="") {
						listDiagCodes.Add(proc.DiagnosticCode);
					}
					if(proc.DiagnosticCode2!=null && proc.DiagnosticCode2!="") {
						listDiagCodes.Add(proc.DiagnosticCode2);
					}
					if(proc.DiagnosticCode3!=null && proc.DiagnosticCode3!="") {
						listDiagCodes.Add(proc.DiagnosticCode3);
					}
					if(proc.DiagnosticCode4!=null && proc.DiagnosticCode4!="") {
						listDiagCodes.Add(proc.DiagnosticCode4);
					}
					for(int i=0;i<listDiagCodes.Count;i++) {
						if(retval!="") {
							retval+=def.RepetitionSeparator;
						}
						ICD9 icd9Cur=ICD9s.GetByCode(listDiagCodes[i]);
						Icd10 icd10Cur=Icd10s.GetByCode(listDiagCodes[i]);
						if(icd9Cur!=null && icd10Cur==null) {
							retval+=gConcat(def.ComponentSeparator,listDiagCodes[i],icd9Cur.Description,"I9C","","","","31");//See HL7 v2.6 Ch 6.5.2.3
							continue;
						}
						if(icd10Cur!=null && icd9Cur==null) {
							retval+=gConcat(def.ComponentSeparator,listDiagCodes[i],icd10Cur.Description,"I10","","","","2013");//See HL7 v2.6 Ch 6.5.2.3
							continue;
						}
						//either the code exists in both lists or neither list, just stick in the code they have entered into OD
						retval+=listDiagCodes[i];
					}
					return retval;
				case "proc.location":
					//Point of Care^Room^^Facility^^Person Location Type
					//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
					if(proc==null || (proc.ClinicNum==0 && pat.ClinicNum==0)) {//if proc is null and both pat.ClinicNum and proc.ClinicNum are 0, return empty string
						return "";
					}
					string procClinicDescript=Clinics.GetDesc(proc.ClinicNum);//could be blank if proc.ClinicNum is invalid
					if(procClinicDescript=="") {
						procClinicDescript=Clinics.GetDesc(pat.ClinicNum);//could be blank if pat.ClinicNum is invalid
					}
					string procOpName="";
					if(apt!=null) {
						Operatory procOp=Operatories.GetOperatory(apt.Op);
						if(procOp!=null) {
							procOpName=procOp.OpName;
						}
					}
					practiceName=PrefC.GetString(PrefName.PracticeTitle);
					return gConcat(def.ComponentSeparator,procClinicDescript,procOpName,"",def.SubcomponentSeparator+practiceName,"","C");
				case "proc.procDateTime":
					if(proc==null) {
						return "";
					}
					return gDTM(proc.ProcDate,8);
				case "proc.ProcFee":
					if(proc==null) {
						return "";
					}
					return proc.ProcFee.ToString("F2");
				case "proc.ProcNum":
					if(proc==null) {
						return "";
					}
					return proc.ProcNum.ToString();
				case "proc.toothSurfRange":
					if(proc==null) {
						return "";
					}
					if(_isEcwDef) {
						return gTreatArea(def.ComponentSeparator,proc,def.IsQuadAsToothNum);
					}
					else {
						return gTreatArea(def.SubcomponentSeparator,proc,def.IsQuadAsToothNum);
					}
				case "proccode.ProcCode":
					if(proc==null) {
						return "";
					}
					if(_isEcwDef) {
						return gProcCodeOld(proc,def.HasLongDCodes);
					}
					//ProcNum^Descript^CD2^^^^2014^^LaymanTerm
					//Example: D0150^comprehensive oral evaluation - new or established patient^CD2^^^^2014^^Comprehensive Exam
					return gProcCode(proc,def);
				#endregion Procedure
				#region Provider
				case "prov.provIdName":
				case "prov.provIdName_V2_3":
				case "prov.provIdNameLFM":
				case "prov.provIdNameLFM_V2_3":
					if(prov==null) {
						return "";
					}
					if(_isEcwDef) {
						return gConcat(def.ComponentSeparator,prov.EcwID,prov.LName,prov.FName,prov.MI);
					}
					//Will return all provider IDs in the oidexternals table linked to this provider as repetitions
					//For an AIG, the provider name is one component in the form LName, FName MI and the fourth component is the provider abbreviation
					//For a PV1 or AIP, the provider name is separated into three components like LName^FName^MI and the sixth component is the provider abbreviation
					//AIG Example: |2.16.840.1.113883.3.4337.1486.6566.3.1^Abbott, Sarah L, DMD^^DrAbbott~OtherSoftware.Root.Provider.ProvID^Abbott, Sarah L, DMD^^DrAbbott|
					//PV1 or AIP Example: 2.16.840.1.113883.3.4337.1486.6566.3.1^Abbott^Sarah^L^DMD^DrAbbott~OtherSoftware.Root.Provider.ProvID^Abbott^Sarah^L^DMD^DrAbbott
					//For the '_V2_3' field names, the abbreviation field will not be populated
					List<OIDExternal> listProvOidExt=OIDExternals.GetByInternalIDAndType(prov.ProvNum,IdentifierType.Provider);
					string provName;
					string provAbbr=fieldName.EndsWith("_V2_3")?"":prov.Abbr;
					if(ListTools.In(fieldName,"prov.provIdName","prov.provIdName_V2_3")) {
						provName=gConcat(def.ComponentSeparator,prov.LName+", "+prov.FName+" "+prov.MI+", "+prov.Suffix,"");
					}
					else {
						provName=gConcat(def.ComponentSeparator,prov.LName,prov.FName,prov.MI,prov.Suffix);
					}
					retval=gConcat(def.ComponentSeparator,OIDInternals.GetForType(IdentifierType.Provider).IDRoot+"."+prov.ProvNum,provName,provAbbr);
					foreach(OIDExternal oidExt in listProvOidExt) {
						retval+=def.RepetitionSeparator+gConcat(def.ComponentSeparator,oidExt.rootExternal+"."+oidExt.IDExternal,provName,provAbbr);
					}
					return retval;
				case "prov.provType":
					if(prov==null) {
						return "";
					}
					if(apt==null) {
						if(prov.IsSecondary) {
							return "H";
						}
						return "D";
					}
					//if we have an appt, return 'D' if prov is the dentist and 'H' if prov is the hygienist, regardless of whether they are marked secondary or not
					if(prov.ProvNum==apt.ProvHyg) {
						return "H";
					}
					return "D";//default to 'D' - dentist
				#endregion Provider
				case "segmentAction":
					//This is currently only supported for SIU and SRR messages in the RSG, AIL, and AIP segments
					//A-Add/Insert, D-Delete, U-Update, X-No Change
					//SIU.S12 - Create Appt, S13 - Appt Rescheduling, S14 - Appt Modification, S15 - Appt Cancellation, S17 - Appt Deletion
					//SRR.S03 - Request Appointment Modification, S04 - Request Appointment Cancellation
					if(msgType==MessageTypeHL7.SIU && eventType==EventTypeHL7.S12) {
						return "A";
					}
					if(msgType==MessageTypeHL7.SRR //all SRR messages are for updating existing appts, 'U'
						|| (msgType==MessageTypeHL7.SIU
						&& (eventType==EventTypeHL7.S13 || eventType==EventTypeHL7.S14 || eventType==EventTypeHL7.S15))) //SIU's with event type S13, S14, or S15 are for updating existing appts
					{
						return "U";
					}
					if(msgType==MessageTypeHL7.SIU && eventType==EventTypeHL7.S17) {
						return "D";
					}
					return "";//if not an SIU or SRR or if it is not one of these event types, return empty string
				case "sendingApp":
					//HD data type, Namespace ID^UniversalID^UniversalIDType
					//UniversalID=oidinternal.IDRoot for IDType of Root, UniversalIDType=HL7
					//If no value in oidinternal table, then revert to 'OD'
					OIDInternal oidRoot=OIDInternals.GetForType(IdentifierType.Root);
					if(oidRoot==null) {
						return "OD";
					}
					return gConcat(def.ComponentSeparator,"",oidRoot.IDRoot,"HL7");
				case "separators^~\\&":
					return gSep(def);
				case "sequenceNum":
					return sequenceNum.ToString();
				default:
					return "";
			}
		}

		///<summary>Only for creating the IN1 segment(s).</summary>
		public static string GenerateFieldIN1(HL7Def def,string fieldName,int sequenceNum,PatPlan patplanCur,InsSub inssubCur,InsPlan insplanCur,
			Carrier carrierCur,int patplanCount,Patient patSub)
		{
			switch(fieldName) {
				#region Carrier
				case "carrier.addressCityStateZip":
					if(carrierCur==null) {
						return "";
					}
					return gConcat(def.ComponentSeparator,carrierCur.Address,carrierCur.Address2,carrierCur.City,carrierCur.State,carrierCur.Zip);
				case "carrier.CarrierName":
					if(carrierCur==null) {
						return "";
					}
					return carrierCur.CarrierName;
				case "carrier.ElectID":
					if(carrierCur==null) {
						return "";
					}
					return carrierCur.ElectID;
				case "carrier.Phone":
					//Example: ^WPN^PH^^^800^3635432
					if(carrierCur==null) {
						return "";
					}
					string carrierPh=gXTN(carrierCur.Phone,10);
					if(carrierPh=="") {
						return "";
					}
					return gConcat(def.ComponentSeparator,"","WPN","PH","","",carrierPh.Substring(0,3),carrierPh.Substring(3));//carrierPh guaranteed to be 10 digits or blank
				#endregion Carrier
				#region InsPlan
				case "insplan.cob":
					if(insplanCur==null) {
						return "";
					}
					if(patplanCount>1) {
						return "CO";
					}
					return "IN";
				case "insplan.coverageType":
					if(insplanCur==null) {
						return "";
					}
					if(insplanCur.IsMedical) {
						return "M";
					}
					return "D";
				case "insplan.empName":
					if(insplanCur==null) {
						return "";
					}
					return Employers.GetName(insplanCur.EmployerNum);//will return empty string if EmployerNum=0 or not found
				case "insplan.GroupName":
					if(insplanCur==null) {
						return "";
					}
					return insplanCur.GroupName;
				case "insplan.GroupNum":
					if(insplanCur==null) {
						return "";
					}
					return insplanCur.GroupNum;
				case "insplan.planNum":
					//Example: 2.16.840.1.113883.3.4337.1486.6566.7.1234
					//If OID for insplan is not set, then it will be ODInsPlan.1234 (where 1234=PlanNum)
					if(insplanCur==null) {
						return "";
					}
					OIDInternal insplanOid=OIDInternals.GetForType(IdentifierType.InsPlan);//returns root+".7" or whatever they have set in their oidinternal table for type InsPlan
					string insplanOidRoot="ODInsPlan";
					if(insplanOid!=null && insplanOid.IDRoot!="") {
						insplanOidRoot=insplanOid.IDRoot;
					}
					return insplanOidRoot+"."+insplanCur.PlanNum;//tack on "."+PlanNum for extension
				case "insplan.PlanType":
					if(insplanCur==null) {
						return "";
					}
					if(insplanCur.PlanType=="p") {
						return "PPO Percentage";
					}
					if(insplanCur.PlanType=="f") {
						return "Medicaid or Flat Copay";
					}
					if(insplanCur.PlanType=="c") {
						return "Capitation";
					}
					return "Category Percentage";
				#endregion InsPlan
				#region InsSub
				case "inssub.AssignBen":
					if(inssubCur==null) {
						return "";
					}
					if(inssubCur.AssignBen) {
						return "Y";
					}
					return "N";
				case "inssub.DateEffective":
					if(inssubCur==null || inssubCur.DateEffective==DateTime.MinValue) {
						return "";
					}
					return gDTM(inssubCur.DateEffective,8);
				case "inssub.DateTerm":
					if(inssubCur==null || inssubCur.DateTerm==DateTime.MinValue) {
						return "";
					}
					return gDTM(inssubCur.DateTerm,8);
				case "inssub.ReleaseInfo":
					if(inssubCur==null) {
						return "";
					}
					if(inssubCur.ReleaseInfo) {
						return "Y";
					}
					return "N";
				case "inssub.subAddrCityStateZip":
					if(patSub==null) {
						return "";
					}
					return gConcat(def.ComponentSeparator,patSub.Address,patSub.Address2,patSub.City,patSub.State,patSub.Zip);
				case "inssub.subBirthdate":
					if(patSub==null) {
						return "";
					}
					return gDTM(patSub.Birthdate,8);
				case "inssub.SubscriberID":
					if(inssubCur==null) {
						return "";
					}
					return inssubCur.SubscriberID;
				case "inssub.subscriberName":
					if(patSub==null) {
						return "";
					}
					return gConcat(def.ComponentSeparator,patSub.LName,patSub.FName,patSub.MiddleI);
				#endregion InsSub
				#region PatPlan
				case "patplan.Ordinal":
					if(patplanCur==null) {
						return "";
					}
					return patplanCur.Ordinal.ToString();
				case "patplan.policyNum":
					if(patplanCur==null || inssubCur==null) {
						return "";
					}
					if(patplanCur.PatID!="") {
						return patplanCur.PatID;
					}
					return inssubCur.SubscriberID;
				case "patplan.subRelationToPat":
					//SEL-Self, SPO-Spouse, DOM-LifePartner, CHD-Child (PAR-Parent), EME-Employee (EMR-Employer)
					//DEP-HandicapDep (GRD-Guardian), DEP-Dependent, OTH-SignifOther (OTH-Other), OTH-InjuredPlantiff
					//We store relationship to subscriber and they want subscriber's relationship to patient, therefore
					//Relat.Child will return "PAR" for Parent, Relat.Employee will return "EMR" for Employer, and Relat.HandicapDep and Relat.Dependent will return "GRD" for Guardian
					//Example: |PAR^Parent|
					if(patplanCur==null) {
						return "";
					}
					if(patplanCur.Relationship==Relat.Self) {
						return gConcat(def.ComponentSeparator,"SEL","Self");
					}
					if(patplanCur.Relationship==Relat.Spouse) {
						return gConcat(def.ComponentSeparator,"SPO","Spouse");
					}
					if(patplanCur.Relationship==Relat.LifePartner) {
						return gConcat(def.ComponentSeparator,"DOM","Life Partner");
					}
					if(patplanCur.Relationship==Relat.Child) {
						return gConcat(def.ComponentSeparator,"PAR","Parent");
					}
					if(patplanCur.Relationship==Relat.Employee) {
						return gConcat(def.ComponentSeparator,"EMR","Employer");
					}
					if(patplanCur.Relationship==Relat.Dependent || patplanCur.Relationship==Relat.HandicapDep) {
						return gConcat(def.ComponentSeparator,"GRD","Guardian");
					}
					//if Relat.SignifOther or Relat.InjuredPlaintiff or any others
					return gConcat(def.ComponentSeparator,"OTH","Other");
				#endregion PatPlan
				case "sequenceNum":
					return sequenceNum.ToString();
				default:
					return "";
			}
		}

		public static string GenerateFieldACK(HL7Def def,string fieldName,string controlId,bool isAck,string ackEvent) {
			switch(fieldName) {
				case "ackCode":
					return gAck(isAck);
				case "dateTime.Now":
					return gDTM(DateTime.Now,14);
				case "messageControlId":
					return controlId;
				case "messageType":
					return gConcat(def.ComponentSeparator,MessageTypeHL7.ACK.ToString(),ackEvent,MessageTypeHL7.ACK.ToString());
				case "separators^~\\&":
					return gSep(def);
				default:
					return "";
			}
		}

		private static string gAck(bool isAck) {
			if(isAck) {
				return "AA";//Acknowledgment accept
			}
			else {
				return "AE";//Acknowledgment error
			}
			//Ack reject is a third possible response that we don't currently support
		}

		///<summary>Send in component separator for this def and the values in the order they should be in the message.</summary>
		private static string gConcat(string componentSep,params string[] vals) {
			string retVal="";
			if(vals.Length==1) {
				return retVal=vals[0];//this allows us to pass in all components for the field as one long string: comp1^comp2^comp3
			}
			for(int i=0;i<vals.Length;i++) {
				if(i>0) {
					retVal+=componentSep;
				}
				retVal+=vals[i];
			}
			return retVal;
		}

		private static string gSep(HL7Def def) {
			return def.ComponentSeparator+def.RepetitionSeparator+def.EscapeCharacter+def.SubcomponentSeparator;
		}

		private static string gDTM(DateTime dt,int precisionDigits) {
			switch(precisionDigits) {
				case 8:
					return dt.ToString("yyyyMMdd");
				case 14:
					return dt.ToString("yyyyMMddHHmmss");
				default:
					return "";
			}
		}

		private static string gIS(Patient pat) {
			if(pat.Gender==PatientGender.Female) {
				return "F";
			}
			if(pat.Gender==PatientGender.Male) {
				return "M";
			}
			return "U";
		}

		private static string gNewLines(string escapeChar,string note) {
			if(string.IsNullOrEmpty(note)) {
				return "";
			}
			return note.Replace("\r\n","\n").Replace("\r","\n").Replace("\n",escapeChar+".br"+escapeChar);
		}

		private static string gPos(Patient pat) {
			switch(pat.Position) {
				case PatientPosition.Single:
					return "Single";
				case PatientPosition.Married:
					return "Married";
				case PatientPosition.Divorced:
					return "Divorced";
				case PatientPosition.Widowed:
					return "Widowed";
				case PatientPosition.Child:
					return "Single";
				default:
					return "Single";
			}
		}

		private static string gProcCodeOld(Procedure proc,bool hasLongDCodes) {
			if(proc==null) {
				return "";
			}
			string retVal="";
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
			if(procCode.ProcCode.Length>5
				&& procCode.ProcCode.StartsWith("D")
				&& !hasLongDCodes)//truncate only if HasLongDCodes is false
			{
				retVal=procCode.ProcCode.Substring(0,5);//Remove suffix from all D codes.
			}
			else {
				retVal=procCode.ProcCode;
			}
			return retVal;
		}

		private static string gProcCode(Procedure proc,HL7Def def) {
			//CNE data type, ProcNum^Descript^CD2^^^^2014^^LaymanTerm
			//Example: D0150^comprehensive oral evaluation - new or established patient^CD2^^^^2014^^Comprehensive Exam
			if(proc==null) {
				return "";
			}
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
			return gConcat(def.ComponentSeparator,procCode.ProcCode,procCode.Descript,"CD2","","","","2014","",procCode.LaymanTerm);			
		}

		private static string gRaceOld(Patient pat) {
			switch(PatientRaces.GetPatientRaceOldFromPatientRaces(pat.PatNum)) {//Uses the PatientRaceOld enum converted from the patient's current list of PatientRaces
				case PatientRaceOld.AmericanIndian:
					return "American Indian Or Alaska Native";
				case PatientRaceOld.Asian:
					return "Asian";
				case PatientRaceOld.HawaiiOrPacIsland:
					return "Native Hawaiian or Other Pacific";
				case PatientRaceOld.AfricanAmerican:
					return "Black or African American";
				case PatientRaceOld.White:
					return "White";
				case PatientRaceOld.HispanicLatino:
					return "Hispanic";
				case PatientRaceOld.Other:
					return "Other Race";
				default:
					return "Other Race";
			}
		}

		private static string gRace(Patient pat,HL7Def def,bool isV2_3) {
			//Example: 2106-3^White^CDCREC^^^^1~2186-5^NotHispanic^CDCREC^^^^1, for V2_3: 2106-3^White~2186-5^NotHispanic
			List<PatientRace> listPatRaces=PatientRaces.GetForPatient(pat.PatNum).FindAll(x => !string.IsNullOrEmpty(x.CdcrecCode));
			if(listPatRaces.Count==0) {
				return "";
			}
			List<string> listRepetitions=listPatRaces.Select(x => gConcat(def.ComponentSeparator,x.CdcrecCode,x.Description)).ToList();
			if(!isV2_3) {
				listRepetitions.ForEach(x => x=gConcat(def.ComponentSeparator,x,"CDCREC","","","","1"));
			}
			return string.Join(def.RepetitionSeparator,listRepetitions);
		}

		///<summary>The value for the parameter componentSep is def.ComponentSeparator (default ^) for eCW bridges.  But for all other bridges it will be def.SubcomponentSeparator (default &amp;)., we will place the "procedure code modifier" all in the first component, the "Identifier" component.  The second component is "Text" so it doesn't really make sense to put the tooth or range of teeth in the "Identifier" component and the surface/quad/sextant/arch in the "Text" component.  Now the whole tooth/surf/range/quad/sextant/arch modifier will be in the "Identifier" component with 0, 1, or 2 subcomponents.  Examples for eCW: |4^MODL|, |^UL|, |1,2,3|, |^3|.  Examples for everyone else: |4&amp;MODL|, |&amp;UL|, |1,2,3|, |&amp;3|</summary>
		private static string gTreatArea(string componentSep,Procedure proc,bool isQuadAsToothNum) {
			if(proc==null) {
				return "";
			}
			string retVal="";
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
			if(procCode.TreatArea==TreatmentArea.ToothRange) {
				retVal=gConcat(componentSep,proc.ToothRange,"");
			}
			else if(procCode.TreatArea==TreatmentArea.Surf) {//probably not necessary
				retVal=gConcat(componentSep,Tooth.ToInternat(proc.ToothNum),Tooth.SurfTidyForClaims(proc.Surf,proc.ToothNum));
			}
			//requested change to have the UL, UR, LL, LR appear in the tooth number component of this field instead of the surface component
			else if(procCode.TreatArea==TreatmentArea.Quad && isQuadAsToothNum) {
				retVal=gConcat(componentSep,proc.Surf,"");
			}
			else {
				retVal=gConcat(componentSep,Tooth.ToInternat(proc.ToothNum),proc.Surf);
			}
			return retVal;
		}

		///<summary>XTN is a phone number.</summary>
		private static string gXTN(string phone,int numDigits) {
			if(string.IsNullOrEmpty(phone)) {
				return "";
			}
			string retVal="";
			for(int i=0;i<phone.Length;i++) {
				if(!Char.IsNumber(phone,i)) {
					continue;
				}
				if(retVal=="" && phone.Substring(i,1)=="1") {
					continue;//skip leading 1.
				}
				retVal+=phone.Substring(i,1);
			}
			if(retVal.Length==numDigits) {
				return retVal;
			}
			//not a 10 digit number
			return "";
		}

	}
}
