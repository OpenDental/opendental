#if !DISABLE_WINDOWS_BRIDGES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VBbridges;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class DrCeph{
		
		/// <summary></summary>
		public DrCeph(){
			
		}

		///<summary>Uses a VB dll to launch.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null){
				MessageBox.Show("Please select a patient first.");
				return;
			}
			//js This was originally added on 9/2/09, probably due to race affecting ceph proportions.  But since we can't find any documentation, and since a customer is complaining, we are removing it for now.  If we add it back, we will document exactly why.
			//if(pat.Race==PatientRace.Unknown) {
			//  MessageBox.Show("Race must be entered first.");
			//  return;
			//}
			//Make sure the program is running
			if(Process.GetProcessesByName("DrCeph").Length==0) {
				try{
					ODFileUtils.ProcessStart(path);
				}
				catch{
					MsgBox.Show("DrCeph","Program path not set properly.");
					return;
				}
				Thread.Sleep(TimeSpan.FromSeconds(4));
			}
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);;
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
			string patID="";
			if(PPCur.PropertyValue=="0"){
				patID=pat.PatNum.ToString();
			}
			else{
				patID=pat.ChartNumber;
			}
			try{
				PatientRaceOld raceOld=PatientRaces.GetPatientRaceOldFromPatientRaces(pat.PatNum);
				List<RefAttach> referalList=RefAttaches.Refresh(pat.PatNum);
				Provider prov=Providers.GetProv(Patients.GetProvNum(pat));
				string provName=prov.FName+" "+prov.MI+" "+prov.LName+" "+prov.Suffix;
				Family fam=Patients.GetFamily(pat.PatNum);
				Patient guar=fam.ListPats[0];
				string relat="";
				if(guar.PatNum==pat.PatNum){
					relat="Self";
				}
				else if(guar.Gender==PatientGender.Male	&& pat.Position==PatientPosition.Child){
					relat="Father";
				}
				else if(guar.Gender==PatientGender.Female	&& pat.Position==PatientPosition.Child){
					relat="Mother";
				}
				else{
					relat="Unknown";
				}
				VBbridges.DrCephNew.Launch(patID,pat.FName,pat.MiddleI,pat.LName,pat.Address,pat.Address2,pat.City,
					pat.State,pat.Zip,pat.HmPhone,pat.SSN,pat.Gender.ToString(),raceOld.ToString(),"",pat.Birthdate.ToString(),
					DateTime.Today.ToShortDateString(),RefAttachL.GetReferringDr(referalList),provName,
					guar.GetNameFL(),guar.Address,guar.Address2,guar.City,guar.State,guar.Zip,guar.HmPhone,relat);
			}
			catch{
				MessageBox.Show("DrCeph not responding.  It might not be installed properly.");
			}
		}

		

	}
}
#endif