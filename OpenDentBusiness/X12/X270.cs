using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace OpenDentBusiness
{
	///<summary></summary>
	public class X270:X12object{

		public X270(string messageText):base(messageText){
		
		}

		///<summary>In progress.  Probably needs a different name.  Info must be validated first.
		///Set dependent to the currently selected patient, compares dependent.PatNum to subscriber.PatNum to either contruct a subscriber based or dependent based benefit request.</summary>
		public static string GenerateMessageText(Clearinghouse clearinghouseClin,Carrier carrier,Provider billProv,Clinic clinic,InsPlan insPlan,Patient subscriber,InsSub insSub,Patient patForRequest) {
			bool isSubscriberRequest=(subscriber.PatNum==patForRequest.PatNum);
			int batchNum=Clearinghouses.GetNextBatchNumber(clearinghouseClin);
			string groupControlNumber=batchNum.ToString();//Must be unique within file.  We will use batchNum
			int transactionNum=1;
			StringBuilder strb=new StringBuilder();
			//Interchange Control Header
			strb.Append("ISA*00*          *");//ISA01,ISA02: 00 + 10 spaces
			if(IsEmdeonDental(clearinghouseClin)) {
				strb.Append("00*"+Sout(clearinghouseClin.Password,10,10)+"*"//ISA03,ISA04: 00 + emdeon password padded to 10 characters
					+clearinghouseClin.ISA05+"*"//ISA05: Sender ID type: ZZ=mutually defined. 30=TIN. Validated
					+"316:"+Sout(clearinghouseClin.LoginID,11,11)+"*"//ISA06: Emdeon vendor number + username
					+clearinghouseClin.ISA07+"*"//ISA07: Receiver ID type: ZZ=mutually defined. 30=TIN. Validated
					+Sout("EMDEONDENTAL",15,15)+"*");//ISA08: Receiver ID. Validated to make sure length is at least 2.
			}
			else{
				strb.Append("00*          *"//ISA03,ISA04: 00 + 10 spaces
					+clearinghouseClin.ISA05+"*"//ISA05: Sender ID type: ZZ=mutually defined. 30=TIN. Validated
					+X12Generator.GetISA06(clearinghouseClin)+"*"//ISA06: Sender ID(TIN). Or might be TIN of Open Dental
					+clearinghouseClin.ISA07+"*"//ISA07: Receiver ID type: ZZ=mutually defined. 30=TIN. Validated
					+Sout(clearinghouseClin.ISA08,15,15)+"*");//ISA08: Receiver ID. Validated to make sure length is at least 2.
			}
			strb.AppendLine(DateTime.Today.ToString("yyMMdd")+"*"//ISA09: today's date
				+DateTime.Now.ToString("HHmm")+"*"//ISA10: current time
				+"U*00401*"//ISA11 and ISA12. 
				//ISA13: interchange control number, right aligned:
				+batchNum.ToString().PadLeft(9,'0')+"*"
				+"0*"//ISA14: no acknowledgment requested
				+clearinghouseClin.ISA15+"*"//ISA15: T=Test P=Production. Validated.
				+":~");//ISA16: use ':'
			//Functional Group Header
			if(IsEmdeonDental(clearinghouseClin)) {
				strb.Append("GS*HS*"//GS01: HS for 270 benefit inquiry
					+X12Generator.GetGS02(clearinghouseClin)+"*"//GS02: Senders Code. Sometimes Jordan Sparks.  Sometimes the sending clinic.
					+Sout("EMDEONDENTAL",15,15)+"*");//GS03: Application Receiver's Code
			}
			else{
				strb.Append("GS*HS*"//GS01: HS for 270 benefit inquiry
					+X12Generator.GetGS02(clearinghouseClin)+"*"//GS02: Senders Code. Sometimes Jordan Sparks.  Sometimes the sending clinic.
					+Sout(clearinghouseClin.GS03,15,2)+"*");//GS03: Application Receiver's Code
			}
			strb.AppendLine(DateTime.Today.ToString("yyyyMMdd")+"*"//GS04: today's date
				+DateTime.Now.ToString("HHmm")+"*"//GS05: current time
				+groupControlNumber+"*"//GS06: Group control number. Max length 9. No padding necessary.
				+"X*"//GS07: X
				+"004010X092~");//GS08: Version
			//Beginning of transaction--------------------------------------------------------------------------------
			int seg=0;//count segments for the ST-SE transaction
			//Transaction Set Header
			//ST02 Transact. control #. Must be unique within ISA
			seg++;
			strb.AppendLine("ST*270*"//ST01
				+transactionNum.ToString().PadLeft(4,'0')+"~");//ST02
			seg++;
			strb.AppendLine("BHT*0022*13*"//BHT02: 13=request
				+transactionNum.ToString().PadLeft(4,'0')+"*"//BHT03. Can be same as ST02
				+DateTime.Now.ToString("yyyyMMdd")+"*"//BHT04: Date
				+DateTime.Now.ToString("HHmmss")+"~");//BHT05: Time, BHT06: not used
			//HL Loops-----------------------------------------------------------------------------------------------
			int HLcount=1;
			//2000A HL: Information Source--------------------------------------------------------------------------
			seg++;
			strb.AppendLine("HL*"+HLcount.ToString()+"*"//HL01: Heirarchical ID.  Here, it's always 1.
				+"*"//HL02: No parent. Not used
				+"20*"//HL03: Heirarchical level code. 20=Information source
				+"1~");//HL04: Heirarchical child code. 1=child HL present
			//2100A NM1
			seg++;
			strb.AppendLine("NM1*PR*"//NM101: PR=Payer
				+"2*"//NM102: 2=Non person
				+Sout(carrier.CarrierName,35)+"*"//NM103: Name Last.
				+"****"//NM104-07 not used
				+"PI*"//NM108: PI=PayorID
				+Sout(carrier.ElectID,80,2)+"~");//NM109: PayorID. Validated to be at least length of 2.
			HLcount++;
			//2000B HL: Information Receiver------------------------------------------------------------------------
			seg++;
			strb.AppendLine("HL*"+HLcount.ToString()+"*"//HL01: Heirarchical ID.  Here, it's always 2.
				+"1*"//HL02: Heirarchical parent id number.  1 in this simple message.
				+"21*"//HL03: Heirarchical level code. 21=Information receiver
				+"1~");//HL04: Heirarchical child code. 1=child HL present
			seg++;
			//2100B NM1: Information Receiver Name
			strb.AppendLine("NM1*1P*"//NM101: 1P=Provider
				+(billProv.IsNotPerson?"2":"1")+"*"//NM102: 1=person,2=non-person
				+Sout(billProv.LName,35)+"*"//NM103: Last name
				+Sout(billProv.FName,25)+"*"//NM104: First name
				+Sout(billProv.MI,25,1)+"*"//NM105: Middle name
				+"*"//NM106: not used
				+"*"//NM107: Name suffix. not used
				+"XX*"//NM108: ID code qualifier. 24=EIN. 34=SSN, XX=NPI
				+Sout(billProv.NationalProvID,80)+"~");//NM109: ID code. NPI validated
			//2100B REF: Information Receiver ID
			if(IsEmdeonDental(clearinghouseClin) && IsDentiCalCarrier(carrier)) {
				string ref4aSegment="";
				Clearinghouse clearinghouseDentiCalHQ=Clearinghouses.GetFirstOrDefault(x => IsDentiCalClearinghouse(x),true);
				if(clearinghouseDentiCalHQ!=null) {
					Clearinghouse clearinghouseDentiCalClin=Clearinghouses.OverrideFields(clearinghouseDentiCalHQ,clearinghouseClin.ClinicNum);
					if(clearinghouseDentiCalClin!=null){
						ref4aSegment=clearinghouseDentiCalClin.Password;
					}
				}
				seg++;
				strb.Append("REF*4A*"+ref4aSegment+"~");
			}
			seg++;
			strb.Append("REF*");
			if(billProv.UsingTIN) {
				strb.Append("TJ*");//REF01: qualifier. TJ=Federal TIN
			}
			else {//SSN
				strb.Append("SY*");//REF01: qualifier. SY=SSN
			}
			strb.AppendLine(Sout(billProv.SSN,30)+"~");//REF02: ID 
			//2100B N3: Information Receiver Address
			seg++;
			if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
				strb.Append("N3*"+Sout(PrefC.GetString(PrefName.PracticeBillingAddress),55));//N301: Address
			}
			else if(clinic==null) {
				strb.Append("N3*"+Sout(PrefC.GetString(PrefName.PracticeAddress),55));//N301: Address
			}
			else {
				strb.Append("N3*"+Sout(clinic.Address,55));//N301: Address
			}
			if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
				if(PrefC.GetString(PrefName.PracticeBillingAddress2)=="") {
					strb.AppendLine("~");
				}
				else {
					//N302: Address2. Optional.
					strb.AppendLine("*"+Sout(PrefC.GetString(PrefName.PracticeBillingAddress2),55)+"~");
				}
			}
			else if(clinic==null) {
				if(PrefC.GetString(PrefName.PracticeAddress2)=="") {
					strb.AppendLine("~");
				}
				else {
					//N302: Address2. Optional.
					strb.AppendLine("*"+Sout(PrefC.GetString(PrefName.PracticeAddress2),55)+"~");
				}
			}
			else {
				if(clinic.Address2=="") {
					strb.AppendLine("~");
				}
				else {
					//N302: Address2. Optional.
					strb.AppendLine("*"+Sout(clinic.Address2,55)+"~");
				}
			}
			//2100B N4: Information Receiver City/State/Zip
			seg++;
			if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
				strb.AppendLine("N4*"+Sout(PrefC.GetString(PrefName.PracticeBillingCity),30)+"*"//N401: City
					+Sout(PrefC.GetString(PrefName.PracticeBillingST),2)+"*"//N402: State
					+Sout(PrefC.GetString(PrefName.PracticeBillingZip).Replace("-",""),15)+"~");//N403: Zip
			}
			else if(clinic==null) {
				strb.AppendLine("N4*"+Sout(PrefC.GetString(PrefName.PracticeCity),30)+"*"//N401: City
					+Sout(PrefC.GetString(PrefName.PracticeST),2)+"*"//N402: State
					+Sout(PrefC.GetString(PrefName.PracticeZip).Replace("-",""),15)+"~");//N403: Zip
			}
			else {
				strb.AppendLine("N4*"+Sout(clinic.City,30)+"*"//N401: City
					+Sout(clinic.State,2)+"*"//N402: State
					+Sout(clinic.Zip.Replace("-",""),15)+"~");//N403: Zip
			}
			//2100B PRV: Information Receiver Provider Info
			seg++;
			//PRV*PE*ZZ*1223G0001X~
			strb.AppendLine("PRV*PE*"//PRV01: Provider Code. PE=Performing.  There are many other choices.
				+"ZZ*"//PRV02: ZZ=Mutually defined = health care provider taxonomy code
				+X12Generator.GetTaxonomy(billProv)+"~");//PRV03: Specialty code
			HLcount++;
			//2000C HL: Subscriber-----------------------------------------------------------------------------------
			seg++;
			strb.AppendLine("HL*"+HLcount.ToString()+"*"//HL01: Heirarchical ID.  Here, it's always 3.
				+"2*"//HL02: Heirarchical parent id number.  2 in this simple message.
				+"22*"//HL03: Heirarchical level code. 22=Subscriber
				+(isSubscriberRequest?"0~":"1~"));//HL04: Heirarchical child code. 0=no child HL present (no dependent) else 1
			//2000C TRN: Subscriber Trace Number
			seg++;
			strb.AppendLine("TRN*1*"//TRN01: Trace Type Code.  1=Current Transaction Trace Numbers
				+"1*"//TRN02: Trace Number.  We don't really have a good primary key yet.  Keep it simple. Use 1.
				+"1"+billProv.SSN+"~");//TRN03: Entity Identifier. First digit is 1=EIN.  Next 9 digits are EIN.  Length validated.
			//2100C NM1: Subscriber Name
			seg++;
			strb.AppendLine("NM1*IL*"//NM101: IL=Insured or Subscriber
				+"1*"//NM102: 1=Person
				+Sout(subscriber.LName,35)+"*"//NM103: LName
				+Sout(subscriber.FName,25)+"*"//NM104: FName
				+Sout(subscriber.MiddleI,25)+"*"//NM105: MiddleName
				+"*"//NM106: not used
				+"*"//NM107: suffix. Not present in Open Dental yet.
				+"MI*"//NM108: MI=MemberID
				+Sout(insSub.SubscriberID.Replace("-",""),80)+"~");//NM109: Subscriber ID. Validated to be L>2.
			if(!String.IsNullOrEmpty(insPlan.GroupNum)) {//Is allowed to be blank for Medicaid.
				//Medicaid uses the patient First/Last/DOB to get benefits, not group number.
				//However, the Edit Electronic Benefit Request window (FormEtrans270Edit) will show a "Mismatched group number" warning
				//if Medicaid returns a Group Num in the response, so we send the GroupNum out if specified to prevent the warning.
				//2100C REF: Subscriber Additional Information.  Without this, old plans seem to be frequently returned.
				seg++;
				strb.AppendLine("REF*6P*"//REF01: 6P=GroupNumber
					+Sout(insPlan.GroupNum,30)+"~");//REF02: Supplemental ID. Validated.
			}
			//2100C DMG: Subscriber Demographic Information
			seg++;
			strb.AppendLine("DMG*D8*"//DMG01: Date Time Period Qualifier.  D8=CCYYMMDD
				+subscriber.Birthdate.ToString("yyyyMMdd")+"~");//DMG02: Subscriber birthdate.  Validated
				//DMG03: Gender code.  Situational.  F or M.  Since this was left out in the example,
				//and since we don't want to send the wrong gender, we will not send this element.
			//2100C DTP: Subscriber Date.  Deduced through trial and error that this is required by EHG even though not by X12 specs.
			seg++;
			strb.AppendLine("DTP*307*"//DTP01: Qualifier.  307=Eligibility
				+"D8*"//DTP02: Format Qualifier.
				+DateTime.Today.ToString("yyyyMMdd")+"~");//DTP03: Date
			//2000D HL: Dependent Level Hierarchical Level
			if(isSubscriberRequest) {
				//2110C EQ: Subscriber Eligibility or Benefit Enquiry Information
				//X12 documentation seems to say that we can loop this 99 times to request very specific benefits.
				//ClaimConnect wants to see either an EQ*30 for "an eligibility request", or an EQ*35 for "a general benefits request".
				//The director of vendor implementation at ClaimConnect has informed us that we should send an EQ*35 to get the full set of benefits.
				seg++;
				strb.AppendLine("EQ*35~");//Dental Care
			}
			else {//Dependent based request.
				HLcount++;
				seg++;
				strb.AppendLine("HL*"+HLcount.ToString()+"*"//HL01: Heirarchical ID. 
					+(HLcount-1).ToString()+"*"//HL02: Heirarchical parent id number.
					+"23*"//HL03: Heirarchical level code. 23=Dependent
					+"0~");//HL04: Heirarchical child code. 0=no child HL present (no dependent)
				//2000D TRN: Dependent Trace Number
				seg++;
				strb.AppendLine("TRN*1*"//TRN01: Trace Type Code.  1=Current Transaction Trace Numbers
					+"1*"//TRN02: Trace Number.  We don't really have a good primary key yet.  Keep it simple. Use 1.
					+"1"+billProv.SSN+"~");//TRN03: Entity Identifier. First digit is 1=EIN.  Next 9 digits are EIN.  Length validated.
				//2100D NM1: Dependent Name
				seg++;
				strb.AppendLine("NM1*03*"//NM101: 03=Dependent
					+"1*"//NM102: 1=Person
					+Sout(patForRequest.LName,35)+"*"//NM103: Name Last or Organization Name
					+Sout(patForRequest.FName,25)+"*"//NM104: Name First
					+Sout(patForRequest.MiddleI,25)+"~");//NM105: Name Middle
				//2100D REF: Dependent Additional Identification
				seg++;
				strb.AppendLine("REF*6P*"//REF01: 6P=GroupNumber
					+Sout(insPlan.GroupNum,30)+"~");//REF02: Supplemental ID. Validated.
				//2100D DMG: Dependent Demographic Information
				seg++;
				strb.AppendLine("DMG*D8*"//DMG01: Date Time Period Qualifier.  D8=CCYYMMDD
					+patForRequest.Birthdate.ToString("yyyyMMdd")+"~");//DMG02: Dependent birthdate.  Validated
				//DMG03: Gender code.  Situational.  F or M.  Since this was left out in the example,
				//and since we don't want to send the wrong gender, we will not send this element.
				//2100D DTP: DEPENDENT Date.  Deduced through trial and error that this is required by EHG even though not by X12 specs.
				seg++;
				strb.AppendLine("DTP*307*"//DTP01: Qualifier.  307=Eligibility
					+"D8*"//DTP02: Format Qualifier.
					+DateTime.Today.ToString("yyyyMMdd")+"~");//DTP03: Date
				seg++;
				strb.AppendLine("EQ*35~");//Dental Care
			}
			//Transaction Trailer
			seg++;
			strb.AppendLine("SE*"
				+seg.ToString()+"*"//SE01: Total segments, including ST & SE
				+transactionNum.ToString().PadLeft(4,'0')+"~");
			//End of transaction--------------------------------------------------------------------------------------
			//Functional Group Trailer
			strb.AppendLine("GE*"+transactionNum.ToString()+"*"//GE01: Number of transaction sets included
				+groupControlNumber+"~");//GE02: Group Control number. Must be identical to GS06
			//Interchange Control Trailer
			strb.AppendLine("IEA*1*"//IEA01: number of functional groups
				+batchNum.ToString().PadLeft(9,'0')+"~");//IEA02: Interchange control number
			object[] parameters={clearinghouseClin,strb,carrier,billProv,clinic,insPlan,subscriber,insSub,patForRequest};
			Plugins.HookAddCode(null,"X270.GenerateMessateText_end",parameters);
			return strb.ToString();
			/*
			return @"
ISA*00*          *00*          *30*AA0989922      *30*330989922      *030519*1608*U*00401*000012145*1*T*:~
GS*HS*AA0989922*330989922*20030519*1608*12145*X*004010X092~
ST*270*0001~
BHT*0022*13*ASX012145WEB*20030519*1608~
HL*1**20*1~
NM1*PR*2*Metlife*****PI*65978~
HL*2*1*21*1~
NM1*1P*1*PROVLAST*PROVFIRST****XX*1234567893~
REF*TJ*200384584~
N3*JUNIT ROAD~
N4*CHICAGO*IL*60602~
PRV*PE*ZZ*1223G0001X~
HL*3*2*22*0~
TRN*1*12145*1AA0989922~
NM1*IL*1*SUBLASTNAME*SUBFIRSTNAME****MI*123456789~
DMG*D8*19750323~
DTP*307*D8*20030519~
EQ*30~
SE*17*0001~
GE*1*12145~
IEA*1*000012145~";
			*/

			//return "ISA*00*          *00*          *30*AA0989922      *30*330989922      *030519*1608*U*00401*000012145*1*T*:~GS*HS*AA0989922*330989922*20030519*1608*12145*X*004010X092~ST*270*0001~BHT*0022*13*ASX012145WEB*20030519*1608~HL*1**20*1~NM1*PR*2*Metlife*****PI*65978~HL*2*1*21*1~NM1*1P*1*PROVLAST*PROVFIRST****XX*1234567893~REF*TJ*200384584~N3*JUNIT ROAD~N4*CHICAGO*IL*60602~PRV*PE*ZZ*1223G0001X~HL*3*2*22*0~TRN*1*12145*1AA0989922~NM1*IL*1*SUBLASTNAME*SUBFIRSTNAME****MI*123456789~DMG*D8*19750323~DTP*307*D8*20030519~EQ*30~SE*17*0001~GE*1*12145~IEA*1*000012145~";
		}

		

		///<summary>Converts any string to an acceptable format for X12. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		private static string Sout(string inputStr,int maxL,int minL) {
			return X12Generator.Sout(inputStr,maxL,minL);
		}

		///<summary>Converts any string to an acceptable format for X12. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		private static string Sout(string str,int maxL) {
			return X12Generator.Sout(str,maxL,-1);
		}

		///<summary>Converts any string to an acceptable format for X12. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		private static string Sout(string str) {
			return X12Generator.Sout(str,-1,-1);
		}

		public static string Validate(Clearinghouse clearinghouseClin,Carrier carrier,Provider billProv,Clinic clinic,InsPlan insPlan,Patient subscriber,InsSub insSub,Patient patForRequest) {
			StringBuilder strb=new StringBuilder();
			X12Validate.ISA(clearinghouseClin,strb);
			X12Validate.Carrier(carrier,strb);
			if(carrier.ElectID.Length<2) {
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("Electronic ID");
			}
			if(billProv.SSN.Length!=9) {
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("Prov TIN 9 digits");
			}
			X12Validate.BillProv(billProv,strb);
			if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
				X12Validate.BillingAddress(strb);
			}
			else if(clinic==null) {
				X12Validate.PracticeAddress(strb);
			}
			else {
				X12Validate.Clinic(clinic,strb);
			}
			if(insSub.SubscriberID.Length<2) {
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("SubscriberID");
			}
			if(subscriber.Birthdate.Year<1880) {
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("Subscriber Birthdate");
			}
			if(patForRequest.PatNum!=subscriber.PatNum) {//Dependent validation.
				if(patForRequest.Birthdate.Year<1880) {//Dependent level validation.
					if(strb.Length!=0) {
						strb.Append(",");
					}
					strb.Append("Dependent Birthdate");
				}
			}
			ElectID eID=ElectIDs.GetID(carrier.ElectID);
			//Medicaid uses the patient First/Last/DOB to get benefits, not group number, so skip this validation if this is medicaid
			bool isMedicaid=(eID!=null && eID.IsMedicaid) || carrier.CarrierName.ToLower().Contains("medicaid");
			if(!isMedicaid) {
				if(insPlan.GroupNum=="") {
					if(strb.Length!=0) {
						strb.Append(",");
					}
					strb.Append("Group Number");
				}
			}
			//Darien at Dentalxchange helped us resolve one issue where the Group Number included a dash and was failing.
			//The fix suggested by Darien was to only include the numbers before the dash...  See task #705773
			if(IsClaimConnect(clearinghouseClin) && insPlan.GroupNum.Contains("-")) {
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("Group Number: Only include the group number prior to the '-'");
			}
			return strb.ToString();
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsClaimConnect(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="330989922");
		}

		///<summary>Checks carrier ElectIDs to match to Denti-Cal's unique ElectID.</summary>
		private static bool IsDentiCalCarrier(Carrier carrier) {
			return (carrier.ElectID=="94146");
		}

		///<summary>DentiCal is a carrier.  Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsDentiCalClearinghouse(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="DENTICAL");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsEmdeonDental(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="0135WCH00");
		}


	}
}
