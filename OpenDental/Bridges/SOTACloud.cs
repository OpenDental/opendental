using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	///<summary>Loads the website in a WebView2 control.</summary>
	public class SOTACloud {

		///<summary></summary>
		public SOTACloud(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			if(pat==null) {
				MsgBox.Show("SOTACloud","Please select a patient first.");
				return;
			}
			//Get the base URL from program properties.
			string practiceInstanceName=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Practice Instance Name");
			if(string.IsNullOrWhiteSpace(practiceInstanceName)) {
				MsgBox.Show("SOTACloud","Practice Instance Name was not set. Please contact support.");
				return;
			}
			bool doUseChartNum=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="1";
			//Add in the various query params that are available (validate/format as needed).
			string error=ValidatePatientFields(pat,doUseChartNum);
			if(!string.IsNullOrEmpty(error)) {
				MsgBox.Show(error);
				return;
			}
			string patientPhoneNumber="";
			if(!pat.HmPhone.IsNullOrEmpty()) {
				patientPhoneNumber=pat.HmPhone;
			}
			else if (!pat.WirelessPhone.IsNullOrEmpty()) {
				patientPhoneNumber=pat.WirelessPhone;
			}
			else if(!pat.WkPhone.IsNullOrEmpty()) {
				patientPhoneNumber=pat.WkPhone;
			}
			string bridgeId=pat.PatNum.ToString();
			if(doUseChartNum) {
				bridgeId=pat.ChartNumber;
			}
			string baseUrl=$"https://{practiceInstanceName}.sotadevices.com/select-patients?" +
				$"nameFirst={CleanPatientName(pat.FName)}" +
				$"&nameLast={CleanPatientName(pat.LName)}" +
				$"&nameMiddle={CleanPatientName(pat.MiddleI)}" +
				$"&dateBirth={pat.Birthdate.ToString("MM-dd-yyyy")}" +
				$"&bridgeId={bridgeId}" +
				$"&email={pat.Email}" +
				$"&phone={FormatPhoneNumber(patientPhoneNumber)}" +
				$"&forceUpdate=0";
			if(!Uri.IsWellFormedUriString(baseUrl,UriKind.Absolute)) {
				string message=Lans.g("SOTACloud","The following URL is not valid:");
				MsgBox.Show(message+"\r\n"+baseUrl);
				return;
			}
			//Open Webview2 to force the user to use a Chromium browser since that is the only browser that SOTA Cloud supports.
			using FormWebView formWebView=new FormWebView(url:baseUrl);
			try {
				formWebView.ShowDialog();
			}
			catch {
				//If the user does not have WebView2 installed and clicks 'No' when prompted to install it, and exception will bubble up through FormWebView from it's ODWebView2 control. This exception has no useful information in it since it is only being thrown because the user decided not to download WebView2, so nothing to do here.
			}
		}

		///<summary>Strips out characters that SOTACloud considers invalid for patient names.</summary>
		private static string CleanPatientName(string name) {
			string retVal=name.Replace("&","");
			retVal=retVal.Replace("!","");
			retVal=retVal.Replace("#","");
			retVal=retVal.Replace("+","");
			retVal=retVal.Replace("=","");
			retVal=retVal.Replace("?","");
			return retVal;
		}

		/// <summary>Validates all the fields that are required by SOTACloud to access patient records. returns an error string detailing fields that need to be changes, otherwise an empty string means all fields are valid.</summary>
		private static string ValidatePatientFields(Patient patient,bool doUseChartNum) {
			if(patient==null) {
				return Lans.g("SOTACloud","Invalid patient");
			}
			StringBuilder stringBuilder=new StringBuilder();
			if(string.IsNullOrWhiteSpace(CleanPatientName(patient.FName))) {
				stringBuilder.AppendLine(Lans.g("SOTACloud","Invalid first name."));
			}
			if(string.IsNullOrWhiteSpace(CleanPatientName(patient.LName))) {
				stringBuilder.AppendLine(Lans.g("SOTACloud","Invalid last name."));
			}
			if(string.IsNullOrWhiteSpace(CleanPatientName(patient.MiddleI))) {
				stringBuilder.AppendLine(Lans.g("SOTACloud","Invalid middle name."));
			}
			if(string.IsNullOrWhiteSpace(patient.Email) || !EmailAddresses.IsValidEmail(patient.Email)) {
				stringBuilder.AppendLine(Lans.g("SOTACloud","Invalid email address."));
			}
			if(TelephoneNumbers.FormatNumbersExactTen(patient.HmPhone).IsNullOrEmpty() && 
				TelephoneNumbers.FormatNumbersExactTen(patient.WirelessPhone).IsNullOrEmpty() && 
				TelephoneNumbers.FormatNumbersExactTen(patient.WkPhone).IsNullOrEmpty()) 
			{
				stringBuilder.AppendLine(Lans.g("SOTACloud","No valid phone number(s). Must be 10 digits."));
			}
			if(doUseChartNum && patient.ChartNumber.Trim().IsNullOrEmpty()) {
				stringBuilder.AppendLine(Lans.g("SOTACloud","Invalid ChartNum."));
			}
			if(stringBuilder.Length>0) {
				return Lans.g("SOTACloud","The selected patient has the following invalid fields:")+"\r\n"+stringBuilder.ToString();
			}
			return "";//All the patients fields are valid.
		}

		/// <summary>Takes in a 10 digit phone number and returns a formatted string of the form NNN-NNN-NNNN where N is a number. Returns an empty string if phoneNumber is not 10 digits.</summary>
		private static string FormatPhoneNumber(string phoneNumber) {
			string formattedNumber=TelephoneNumbers.FormatNumbersExactTen(phoneNumber);
			if(formattedNumber.IsNullOrEmpty()) {
				return formattedNumber;//Cannot format an empty number
			}
			return formattedNumber.Substring(0,3)+"-"+formattedNumber.Substring(3,3)+"-"+formattedNumber.Substring(6);
		}

	}
}
