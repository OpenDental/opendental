using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenDentBusiness.HL7 {
	///<summary>ADT messages are known as Patient Administration messages.  There are around 60 different kinds of ADT messages.  ADT messages are the most common message type, and I always think of them as "demographics" messages.  Not sure what ADT stands for; probably "admit/discharge/transfer" since many of the kinds of ADTs have to do with handling incoming and outgoing patients.</summary>
	public class EcwADT {
		public static void ProcessMessage(MessageHL7 message,bool isStandalone,bool isVerboseLogging) {
			/*string triggerevent=message.Segments[0].GetFieldComponent(8,1);
			switch(triggerevent) {
				case "A01"://Admit/Visit Information

					break;
				case "A04"://New Patient Information
					ProcessNewPatient(message);
					break;
				case "A08"://Update Patient Information

					break;
				case "A28"://Add Patient Information

					break;
				case "A31"://Update Patient Information

					break;
			}*/
			//MSH-Ignore
			//EVN-Ignore
			//PID-------------------------------------
			SegmentHL7 seg=message.GetSegment(SegmentNameHL7.PID,true);
			long patNum=PIn.Long(seg.GetFieldFullText(2));
			Patient pat=null;
			if(isStandalone) {
				pat=Patients.GetPatByChartNumber(patNum.ToString());
				if(pat==null) {
					//try to find the patient in question by using name and birthdate
					string lName=seg.GetFieldComponent(5,0);
					string fName=seg.GetFieldComponent(5,1);
					DateTime birthdate=EcwSegmentPID.DateParse(seg.GetFieldFullText(7));
					long patNumByName=Patients.GetPatNumByNameAndBirthday(lName,fName,birthdate);
					if(patNumByName==0) {//patient does not exist in OD
						//so pat will still be null, triggering creation of new patient further down.
					}
					else {
						pat=Patients.GetPat(patNumByName);
						pat.ChartNumber=patNum.ToString();//from now on, we will be able to find pat by chartNumber
					}
				}
			}
			else {
				pat=Patients.GetPat(patNum);
			}
			Patient patOld=null;
			bool isNewPat = pat==null;
			if(isNewPat) {
				pat=new Patient();
				if(isStandalone) {
					pat.ChartNumber=patNum.ToString();
					//this line does not work if isStandalone, so moved to end
					//pat.Guarantor=patNum;
				}
				else {
					pat.PatNum=patNum;
					pat.Guarantor=patNum;
				}
				pat.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
				pat.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
			}
			else{
				patOld=pat.Copy();
			}
			List<PatientRace> listPatRaces=new List<PatientRace>();
			bool hasNoRaceInStandalone=(isStandalone && (pat==null || pat.PatNum==0));
			EcwSegmentPID.ProcessPID(pat,seg,isStandalone,listPatRaces);
			//PV1-patient visit---------------------------
			//seg=message.GetSegment(SegmentName.PV1,false);
			//if(seg!=null) {//this seg is optional
			//	SegmentPID.ProcessPV1(pat,seg);
			//}
			//PD1-additional patient demographics------------
			//seg=message.GetSegment(SegmentName.PD1,false);
			//if(seg!=null) {//this seg is optional
			//	ProcessPD1(pat,seg);
			//}
			//GT1-Guarantor-------------------------------------
			seg=message.GetSegment(SegmentNameHL7.GT1,true);
			EcwSegmentPID.ProcessGT1(pat,seg,isStandalone);
			//IN1-Insurance-------------------------------------
			//List<SegmentHL7> segments=message.GetSegments(SegmentName.IN1);
			//for(int i=0;i<segments.Count;i++) {
			//	ProcessIN1(pat,seg);
			//}
			if(pat.FName=="" || pat.LName=="") {
				EventLog.WriteEntry("OpenDentHL7","Patient demographics not processed due to missing first or last name. PatNum:"+pat.PatNum.ToString()
					,EventLogEntryType.Information);
				return;
			}
			if(isNewPat) {
				if(isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted patient: "+pat.FName+" "+pat.LName,EventLogEntryType.Information);
				}
				pat.PatNum=Patients.Insert(pat,!isStandalone);//use existing PK if not standalone, standalone will have PatNum=0, so set PatNum here
				SecurityLogs.MakeLogEntry(Permissions.PatientCreate,pat.PatNum,"Created from HL7 for eCW.",LogSources.HL7);
				if(hasNoRaceInStandalone) {
					Patient patientRaceTemp=pat.Copy();//Make a deep copy so that we do not accidentally override something.
					seg=message.GetSegment(SegmentNameHL7.PID,true);
					//We have to process the PID again in order to correct the patient race.  Patient race(s) will automatically get inserted if needed.
					EcwSegmentPID.ProcessPID(patientRaceTemp,seg,isStandalone,listPatRaces);
				}
				if(pat.Guarantor==0) {
					patOld=pat.Copy();
					pat.Guarantor=pat.PatNum;
					Patients.Update(pat,patOld);
				}
			}
			else {
				if(isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated patient: "+pat.FName+" "+pat.LName,EventLogEntryType.Information);
				}
				Patients.Update(pat,patOld);
			}
			//had to move this reconcile here since we might not have a PatNum for new patients until after the insert
			PatientRaces.Reconcile(pat.PatNum,listPatRaces);
		}

		//public static void ProcessPD1(Patient pat,SegmentHL7 seg) {
		//	long provNum=EcwSegmentPID.ProvProcess(seg.GetField(4));//don't know why both
		//	if(provNum!=0 && provNum!=pat.PriProv) {
		//		SecurityLogs.MakeLogEntry(Permissions.PatPriProvEdit,pat.PatNum,"Primary provider changed from "+Providers.GetLongDesc(pat.PriProv)+" to "
		//			+Providers.GetLongDesc(provNum)+".");
		//		pat.PriProv=provNum;

		//	}
		//}

		//public static void ProcessIN1(Patient pat,SegmentHL7 seg) {
		//	//as a general strategy, if certain things are the same, like subscriber and carrier,
		//	//then we change the existing plan.
		//	//However, if basics change at all, then we drop the old plan and create a new one
		//	int ordinal=PIn.Int(seg.GetFieldFullText(1));
		//	PatPlan oldPatPlan=PatPlans.GetPatPlan(pat.PatNum,ordinal);
		//	if(oldPatPlan==null) {
		//		//create a new plan and a new patplan
		//	}
		//	//InsPlan oldPlan=InsPlans.g
		//	//we'll have to get back to this.  This is lower priority than appointments.
		//}

		


	}


}
