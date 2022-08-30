using System;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	///<summary></summary>
	public class OrthoInsight3d {

		///<summary></summary>
		public OrthoInsight3d(){
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("OrthoInsight3D","Please select a patient first.");
				return;
			}
			string args=" ";
			//-TOOLBAR (Required)
			args+="-TOOLBAR ";
			//PatNum (Required)
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				args+="\""+pat.PatNum.ToString()+"\" ";
			}
			else{
				args+="\""+Tidy(pat.ChartNumber)+"\" ";
			}
			//-DateNeutralFormat (Required)
			args+="\"-DateNeutralFormat=mm/dd/yyyy\" ";
			//-FirstName (Required)
			args+="\"-FirstName="+Tidy(pat.FName)+"\" ";
			//-LastName (Required)
			args+="\"-LastName="+Tidy(pat.LName)+"\" ";
			//-MiddleName
			if(!string.IsNullOrEmpty(pat.MiddleI)) {
				args+="\"-MiddleName="+Tidy(pat.MiddleI)+"\" ";
			}
			//-Title
			if(!string.IsNullOrEmpty(pat.Title)) {
				args+="\"-Title="+Tidy(pat.Title)+"\" ";
			}
			//-PreferredName
			if(!string.IsNullOrEmpty(pat.Preferred)) {
				args+="\"-PreferredName="+Tidy(pat.Preferred)+"\" ";
			}
			//-SocialSecurityNumber
			if(!string.IsNullOrEmpty(pat.SSN) && (pat.SSN.Replace("0","").Trim()!="")) {//An SSN which is all zeros will be treated as a blank SSN.  
				args+="\"-SocialSecurityNumber="+pat.SSN+"\" ";
			}
			//-BirthDateNeutral (Required)
			args+="\"-BirthDateNeutral="+pat.Birthdate.ToString("MM/dd/yyyy")+"\" ";
			//-InitialVisitNeutral
			if(pat.DateFirstVisit.Year>1880) {
				args+="\"-InitialVisitNeutral="+pat.DateFirstVisit.ToString("MM/dd/yyyy")+"\" ";
			}
			//-Gender
			if(pat.Gender==PatientGender.Female) {
				args+="\"-Gender=1"+"\" ";//1=Female
			}
			else if(pat.Gender==PatientGender.Male) {
				args+="\"-Gender=0"+"\" ";//0=Male
			}
			//-Address1
			if(!string.IsNullOrEmpty(pat.Address)) {
				args+="\"-Address1="+pat.Address+"\" ";//Do not tidy, leave spaces.
			}
			//-Address2
			if(!string.IsNullOrEmpty(pat.Address2)) {
				args+="\"-Address2="+pat.Address2+"\" ";//Do not tidy, leave spaces.
			}
			//-City
			if(!string.IsNullOrEmpty(pat.City)) {
				args+="\"-City="+Tidy(pat.City)+"\" ";
			}
			//-StateProvince
			if(!string.IsNullOrEmpty(pat.State)) {
				args+="\"-StateProvince="+Tidy(pat.State)+"\" ";
			}
			//-ZipPostalCode
			if(!string.IsNullOrEmpty(pat.Zip)) {
				args+="\"-ZipPostalCode="+Tidy(pat.Zip)+"\" ";
			}
			try {
				ODFileUtils.ProcessStart(path,args);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");
			return retVal;
		}

	}
}
