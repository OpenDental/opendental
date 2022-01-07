using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenDentBusiness.HL7 {
	public class EcwSIU {
		public static void ProcessMessage(MessageHL7 message,bool isVerboseLogging) {
			SegmentHL7 seg=message.GetSegment(SegmentNameHL7.PID,true);
			long patNum=PIn.Long(seg.GetFieldFullText(2));
			Patient pat=Patients.GetPat(patNum);
			Patient patOld=null;
			bool isNewPat = pat==null;
			if(isNewPat) {
				pat=new Patient();
				pat.PatNum=patNum;
				pat.Guarantor=patNum;
				pat.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
				pat.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,Clinics.ClinicNum));
			}
			else {
				patOld=pat.Copy();
			}
			List<PatientRace> listPatRaces=new List<PatientRace>();
			EcwSegmentPID.ProcessPID(pat,seg,false,listPatRaces);//IsStandalone=false because should never make it this far.
			//PV1-patient visit---------------------------
			//seg=message.GetSegment(SegmentName.PV1,false);
			//if(seg!=null) {
			//	SegmentPID.ProcessPV1(pat,seg);
			//}
			//SCH- Schedule Activity Information
			seg=message.GetSegment(SegmentNameHL7.SCH,true);
			//The documentation is wrong.  SCH.01 is not the appointment ID, but is instead a sequence# (always 1)
			long aptNum=PIn.Long(seg.GetFieldFullText(2));
			Appointment apt=Appointments.GetOneApt(aptNum);
			Appointment aptOld=null;
			bool isNewApt = apt==null;
			if(isNewApt) {
				apt=new Appointment();
				apt.AptNum=aptNum;
				apt.PatNum=pat.PatNum;
				apt.AptStatus=ApptStatus.Scheduled;
			}
			else{
				aptOld=apt.Copy();
			}
			if(apt.PatNum != pat.PatNum) {
				EventLog.WriteEntry("OpenDentHL7","Appointment does not match patient: "+pat.FName+" "+pat.LName
					+", apt.PatNum:"+apt.PatNum.ToString()+", pat.PatNum:"+pat.PatNum.ToString()
					,EventLogEntryType.Error);
				return;//we can't process this message because wrong patnum.
			}
			apt.Note=seg.GetFieldFullText(7);
			//apt.Pattern=ProcessDuration(seg.GetFieldFullText(9));
			//9 and 10 are not actually available, in spite of the documentation.
			//11-We need start time and stop time
			apt.AptDateTime=DateTimeParse(seg.GetFieldComponent(11,3));
			DateTime stopTime=DateTimeParse(seg.GetFieldComponent(11,4));
			apt.Pattern=ProcessPattern(apt.AptDateTime,stopTime);
			apt.ProvNum=pat.PriProv;//just in case there's not AIG segment.
			//AIG is optional, but looks like the only way to get provider for the appt-----------
			//PV1 seems to frequently be sent instead of AIG.
			SegmentHL7 segAIG=message.GetSegment(SegmentNameHL7.AIG,false);
			SegmentHL7 segPV=message.GetSegment(SegmentNameHL7.PV1,false);
			if(segAIG!=null) {
				long provNum=EcwSegmentPID.ProvProcess(segAIG.GetField(3));
				if(provNum!=0) {
					apt.ProvNum=provNum;
					pat.PriProv=provNum;
				}
			}
			else if(segPV!=null) {
				long provNum=EcwSegmentPID.ProvProcess(segPV.GetField(7));
				if(provNum!=0) {
					apt.ProvNum=provNum;
					pat.PriProv=provNum;
				}
			}
			//AIL,AIP seem to be optional, and I'm going to ignore them for now.
			if(pat.FName=="" || pat.LName=="") {
				EventLog.WriteEntry("OpenDentHL7","Appointment not processed due to missing patient first or last name. PatNum:"+pat.PatNum.ToString()
					,EventLogEntryType.Information);
				return;//this will also skip the appt insert.
			}
			if(isNewPat) {
				if(isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted patient: "+pat.FName+" "+pat.LName+", PatNum:"+pat.PatNum.ToString()
						,EventLogEntryType.Information);
				}
				Patients.Insert(pat,true);
				SecurityLogs.MakeLogEntry(Permissions.PatientCreate,pat.PatNum,"Created from HL7 for eCW.",LogSources.HL7);
			}
			else {
				if(isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated patient: "+pat.FName+" "+pat.LName,EventLogEntryType.Information);
				}
				Patients.Update(pat,patOld);
			}
			//had to move this reconcile here since we might not have a PatNum for new patients until after the insert
			PatientRaces.Reconcile(pat.PatNum,listPatRaces);
			if(isNewApt) {
				if(isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Inserted appointment for: "+pat.FName+" "+pat.LName,EventLogEntryType.Information);
				}
				Appointments.InsertIncludeAptNum(apt,true);
			}
			else {
				if(isVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Updated appointment for: "+pat.FName+" "+pat.LName,EventLogEntryType.Information);
				}
				Appointments.Update(apt,aptOld);
			}
		}

		private static string ProcessPattern(DateTime startTime,DateTime stopTime) {
			int minutes=(int)((stopTime-startTime).TotalMinutes);
			if(minutes==0){
				return "//";//we don't want it to be zero minutes
			}
			int increments5=minutes/5;
			StringBuilder pattern=new StringBuilder();
			for(int i=0;i<increments5;i++) {
				pattern.Append("X");//make it all provider time, I guess.
			}
			return pattern.ToString();
		}

		///<summary>yyyyMMddHHmmss.  If not in that format, it returns minVal.</summary>
		public static DateTime DateTimeParse(string str) {
			if(str.Length != 14) {
				return DateTime.MinValue;
			}
			int year=PIn.Int(str.Substring(0,4));
			int month=PIn.Int(str.Substring(4,2));
			int day=PIn.Int(str.Substring(6,2));
			int hour=PIn.Int(str.Substring(8,2));
			int minute=PIn.Int(str.Substring(10,2));
			//skip seconds
			DateTime retVal=new DateTime(year,month,day,hour,minute,0);
			return retVal;
		}


	}
}
