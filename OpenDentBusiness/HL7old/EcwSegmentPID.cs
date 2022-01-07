using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.HL7 {
	/// <summary>(and GT1 and PV1)</summary>
	public class EcwSegmentPID {
		///<summary>PatNum will not be altered here.  The pat passed in must either have PatNum=0, or must have a PatNum matching the segment.  The reason that isStandalone is passed in is because if using tight integration mode (isStandalone=false), then we need to store the "alternate patient id" aka Account No. that comes in on PID.4 in the ChartNumber field so we can pass it back in PID.2 of the DFT charge message.  However, if not using tight integration (isStandalone=true), the ChartNumber field is already occupied by the eCW patient ID, and we do not want to overwrite it.</summary>
		public static void ProcessPID(Patient pat,SegmentHL7 seg,bool isStandalone,List<PatientRace> listPatRaces) {
			long patNum=PIn.Long(seg.GetFieldFullText(2));
			//Standalone mode may not have found a matching patient within the database and will be inserted later in the message processing.
			if(isStandalone && pat!=null) {
				patNum=pat.PatNum;//Standalone mode cannot always trust the PatNum in field 2.  Always use pat.PatNum because that will the OD PatNum.
			}
			//if(pat.PatNum==0) {
			//	pat.PatNum=patNum;
			//}
			//else 
			if(!isStandalone //in standalone, the patnums won't match, so don't check
				&& pat.PatNum!=0 && pat.PatNum!=patNum) 
			{
				throw new ApplicationException("Invalid patNum");
			}
			if(!isStandalone) {//when in tight integration mode
				pat.ChartNumber=seg.GetFieldFullText(4);
			}
			pat.LName=seg.GetFieldComponent(5,0);
			pat.FName=seg.GetFieldComponent(5,1);
			pat.MiddleI=seg.GetFieldComponent(5,2);
			pat.Birthdate=DateParse(seg.GetFieldFullText(7));
			pat.Gender=GenderParse(seg.GetFieldFullText(8));
			if(patNum > 0) {
				listPatRaces.Clear();
				listPatRaces.AddRange(RaceParse(seg.GetFieldFullText(10),patNum));
			}
			pat.Address=seg.GetFieldComponent(11,0);
			pat.Address2=seg.GetFieldComponent(11,1);
			pat.City=seg.GetFieldComponent(11,2);
			pat.State=seg.GetFieldComponent(11,3);
			pat.Zip=seg.GetFieldComponent(11,4);
			pat.HmPhone=PhoneParse(seg.GetFieldFullText(13));
			pat.WkPhone=PhoneParse(seg.GetFieldFullText(14));
			pat.Position=MaritalStatusParse(seg.GetFieldFullText(16));
			//pat.ChartNumber=seg.GetFieldFullText(18);//this is wrong.  Would also break standalone mode
			pat.SSN=seg.GetFieldFullText(19);
			if(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"FeeSchedulesSetManually")=="0") {//if !FeeSchedulesSetManually
				pat.FeeSched=FeeScheduleParse(seg.GetFieldFullText(22));
			}
		}

		///<summary>If relationship is self, this loop does nothing.  A new pat will later change guarantor to be same as patnum. </summary>
		public static void ProcessGT1(Patient pat,SegmentHL7 seg,bool useChartNumber) {
			long guarNum=PIn.Long(seg.GetFieldFullText(2));
			if(guarNum==0) {//because we have an example where they sent us this (position 2 is empty): GT1|1||^^||^^^^||||||||
				return;
			}
			if(seg.GetFieldFullText(11)=="1") {//if relationship is self (according to some of their documentation)
				return;
			}
			if(seg.GetFieldComponent(3,0)==""//lname
				|| seg.GetFieldComponent(3,1)=="")//fname
			{
				EventLog.WriteEntry("OpenDentHL7","Guarantor not processed due to missing first or last name. PatNum of patient:"+pat.PatNum.ToString()
					,EventLogEntryType.Information);
				return;
			}
			Patient guar=null;
			Patient guarOld=null;
			//So guarantor is someone else
			if(useChartNumber) {
				//try to find guarantor by using chartNumber
				guar=Patients.GetPatByChartNumber(guarNum.ToString());
				if(guar==null) {
					//try to find the guarantor by using name and birthdate
					string lName=seg.GetFieldComponent(3,0);
					string fName=seg.GetFieldComponent(3,1);
					DateTime birthdate=EcwSegmentPID.DateParse(seg.GetFieldFullText(8));
					long guarNumByName=Patients.GetPatNumByNameAndBirthday(lName,fName,birthdate);
					if(guarNumByName==0) {//guarantor does not exist in OD
						//so guar will still be null, triggering creation of new guarantor further down.
					}
					else {
						guar=Patients.GetPat(guarNumByName);
						guar.ChartNumber=guarNum.ToString();//from now on, we will be able to find guar by chartNumber
					}
				}
			}
			else {
				guar=Patients.GetPat(guarNum);
			}
			//we can't necessarily set pat.Guarantor yet, because in Standalone mode, we might not know it yet.
			bool isNewGuar= guar==null;
			if(isNewGuar) {//then we need to add guarantor to db
				guar=new Patient();
				if(useChartNumber) {
					guar.ChartNumber=guarNum.ToString();
				}
				else {
					guar.PatNum=guarNum;
				}
				guar.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
				guar.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
			}
			else {
				guarOld=guar.Copy();
			}
			//guar.Guarantor=guarNum;
			guar.LName=seg.GetFieldComponent(3,0);
			guar.FName=seg.GetFieldComponent(3,1);
			guar.MiddleI=seg.GetFieldComponent(3,2);
			guar.Address=seg.GetFieldComponent(5,0);
			guar.Address2=seg.GetFieldComponent(5,1);
			guar.City=seg.GetFieldComponent(5,2);
			guar.State=seg.GetFieldComponent(5,3);
			guar.Zip=seg.GetFieldComponent(5,4);
			guar.HmPhone=PhoneParse(seg.GetFieldFullText(6));
			guar.WkPhone=PhoneParse(seg.GetFieldFullText(7));
			guar.Birthdate=DateParse(seg.GetFieldFullText(8));
			guar.Gender=GenderParse(seg.GetFieldFullText(9));
			//11. Guarantor relationship to patient.  We can't really do anything with this value
			guar.SSN=seg.GetFieldFullText(12);
			if(isNewGuar) {
				Patients.Insert(guar,!useChartNumber);//if using chartnumber (standalone mode), then can't insert using existing PK
				SecurityLogs.MakeLogEntry(Permissions.PatientCreate,guar.PatNum,"Created from HL7 for eCW.",LogSources.HL7);
				guarOld=guar.Copy();
				guar.Guarantor=guar.PatNum;
				Patients.Update(guar,guarOld);
			}
			else {
				Patients.Update(guar,guarOld);
			}
			pat.Guarantor=guar.PatNum;
		}

		//public static void ProcessPV1(Patient pat,SegmentHL7 seg) {
		//	long provNum=ProvProcess(seg.GetField(7));
		//	if(provNum!=0) {
		//		pat.PriProv=provNum;
		//	}
		//}

		///<summary>yyyyMMdd.  If not in that format, it returns minVal.</summary>
		public static DateTime DateParse(string str) {
			if(str.Length != 8) {
				return DateTime.MinValue;
			}
			int year=PIn.Int(str.Substring(0,4));
			int month=PIn.Int(str.Substring(4,2));
			int day=PIn.Int(str.Substring(6));
			DateTime retVal=new DateTime(year,month,day);
			return retVal;
		}

		/// <summary>If it's exactly 10 digits, it will be formatted like this: (###)###-####.  Otherwise, no change.</summary>
		public static string PhoneParse(string str) {
			if(str.Length != 10) {
				return str;//no change
			}
			return "("+str.Substring(0,3)+")"+str.Substring(3,3)+"-"+str.Substring(6);
		}

		///<summary>M,F,U</summary>
		public static PatientGender GenderParse(string str) {
			if(str=="M" || str.ToLower()=="male") {
				return PatientGender.Male;
			}
			else if(str=="F" || str.ToLower()=="female") {
				return PatientGender.Female;
			}
			else {
				return PatientGender.Unknown;
			}
		}

		///<summary>Returns a list of PatientRaces for the passed in string.</summary>
		public static List<PatientRace> RaceParse(string str,long patNum) {
			List<PatientRace> listPatRaces=new List<PatientRace>();
			switch(str) {
				case "American Indian Or Alaska Native":
					listPatRaces.Add(new PatientRace(patNum,"1002-5"));
					break;
				case "Asian":
					listPatRaces.Add(new PatientRace(patNum,"2028-9"));
					break;
				case "Native Hawaiian or Other Pacific":
					listPatRaces.Add(new PatientRace(patNum,"2076-8"));
					break;
				case "Black or African American":
					listPatRaces.Add(new PatientRace(patNum,"2054-5"));
					break;
				case "White":
					listPatRaces.Add(new PatientRace(patNum,"2106-3"));
					break;
				case "Hispanic":
					listPatRaces.Add(new PatientRace(patNum,"2106-3"));//White
					listPatRaces.Add(new PatientRace(patNum,"2135-2"));//Hispanic
					break;
				case "Other Race":
				default:
					listPatRaces.Add(new PatientRace(patNum,"2131-1"));//Other race
					break;
			}
			return listPatRaces;
		}

		public static PatientPosition MaritalStatusParse(string str) {
			switch(str) {
				case "Single":
					return PatientPosition.Single;
				case "Married":
					return PatientPosition.Married;
				case "Divorced":
					return PatientPosition.Divorced;
				case "Widowed":
					return PatientPosition.Widowed;
				case "Legally Separated":
					return PatientPosition.Married;
				case "Unknown":
					return PatientPosition.Single;
				case "Partner":
					return PatientPosition.Single;
				default:
					return PatientPosition.Single;
			}
		}

		///<summary>Supply in format UPIN^LastName^FirstName^MI.  If UPIN(abbr) does not exist, provider gets created.  If name has changed, provider gets updated.  ProvNum is returned.  If blank, then returns 0.  If field is NULL, returns 0.</summary>
		public static long ProvProcess(FieldHL7 field) {
			if(field==null) {
				return 0;
			}
			string eID=field.GetComponentVal(0);
			eID=eID.Trim();
			if(eID=="") {
				return 0;
			}
			Provider prov=Providers.GetProvByEcwID(eID);
			bool isNewProv=false;
			bool provChanged=false;
			if(prov==null) {
				isNewProv=true;
				prov=new Provider();
				prov.Abbr=eID;//They can manually change this later.
				prov.EcwID=eID;
				prov.FeeSched=FeeScheds.GetFirst(true).FeeSchedNum;
			}
			if(prov.LName!=field.GetComponentVal(1)) {
				provChanged=true;
				prov.LName=field.GetComponentVal(1);
			}
			if(prov.FName!=field.GetComponentVal(2)) {
				provChanged=true;
				prov.FName=field.GetComponentVal(2);
			}
			if(prov.MI!=field.GetComponentVal(3)) {
				provChanged=true;
				prov.MI=field.GetComponentVal(3);
			}
			if(isNewProv) {
				Providers.Insert(prov);
				Providers.RefreshCache();
			}
			else if(provChanged) {
				Providers.Update(prov);
				Providers.RefreshCache();
			}
			return prov.ProvNum;
		}

		/// <summary>Will return 0 if string cannot be parsed to a number.  Will return 0 if the fee schedule passed in does not exactly match the description of a regular fee schedule.</summary>
		public static long FeeScheduleParse(string str) {
			if(str=="") {
				return 0;
			}
			FeeSched feeSched=FeeScheds.GetByExactName(str,FeeScheduleType.Normal);
			if(feeSched==null){
				return 0;
			}
			return feeSched.FeeSchedNum;
		}







	}
}
