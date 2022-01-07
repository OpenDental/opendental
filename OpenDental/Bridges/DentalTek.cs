using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO.Pipes;

namespace OpenDental.Bridges {
	///<summary></summary>
	public class DentalTek {

		///<summary></summary>
		public DentalTek() {
			
		}

		///<summary>Attempt to send a phone number to place a call using DentalTek.  Already surrounded in try-catch. 
		///Returns false if unsuccessful or a phone number wasn't passed in.</summary>
		public static bool PlaceCall(string phoneNumber) {
			if(!Programs.IsEnabledByHq(ProgramName.DentalTekSmartOfficePhone,out string error)) {
				MessageBox.Show(error);
				return false;
			}
			phoneNumber=new string(phoneNumber.Where(x => char.IsDigit(x)).ToArray());
			string apiToken=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.DentalTekSmartOfficePhone),"Enter your API Token");
			string request="";
			if(phoneNumber=="") {
				return false;
			}
			using(WebClient client = new WebClient()) {
				string response="";
				try {
					client.Headers[HttpRequestHeader.ContentType]="application/json";
					client.Encoding=UnicodeEncoding.UTF8;
					if(apiToken=="") {
						string domainUser=System.Security.Principal.WindowsIdentity.GetCurrent().Name;
						request="https://extapi.dentaltek.com/v1/pbx/rest/ClickToCall?domainUser="+domainUser+"&phoneNumber="+phoneNumber;
					}
					else {
						if(apiToken.ToLower()=="premise") {
							string token="";
							string domainUsername=System.Security.Principal.WindowsIdentity.GetCurrent().Name;
							domainUsername=domainUsername.Replace(@"\","-");
							string privateIdentifier=domainUsername+"-6a9631ab-be94-4bbe-8822-be68034f9009";
							try {
								using(NamedPipeClientStream clientPipes=new NamedPipeClientStream(privateIdentifier.ToString())) {
									StreamWriter writer=new StreamWriter(clientPipes);
									StreamReader reader=new StreamReader(clientPipes);
									clientPipes.Connect(1000);
									writer.WriteLine("!token!");
									writer.Flush();
									clientPipes.WaitForPipeDrain();
									token=reader.ReadLine();
								}
							}
							catch(Exception e) {
								MessageBox.Show(Lan.g("DentalTek","Error occurred:")+" "+e.Message+"\r\n"+Lan.g("DentalTek","Please login to your Xbeyon/DentalTek Application and try again."));
								return false;
							}
							request="https://extapi.dentaltek.com/v1/pbx/rest/ClickToCall?phoneNumber="+phoneNumber+"&token="+token+"&premise=true";
						}
						else {
							request="https://extapi.dentaltek.com/v1/pbx/rest/ClickToCall?phoneNumber="+phoneNumber+"&token="+apiToken;
						}
					}
					response=client.DownloadString(request);//GET
				}
				catch(Exception) {
					//Can't think of anything useful to tell them about why the call attempt failed.
				}
				return response.Contains("Success");
			}
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("DentalTek","Please select a patient first.");
				return;
			}
			string phoneNumber="";
			Dictionary<ContactMethod,string> dictPrefContactMethod=new Dictionary<ContactMethod,string>();
			dictPrefContactMethod.Add(ContactMethod.HmPhone,pat.HmPhone);
			dictPrefContactMethod.Add(ContactMethod.WkPhone,pat.WkPhone);
			dictPrefContactMethod.Add(ContactMethod.WirelessPh,pat.WirelessPhone);
			if(dictPrefContactMethod.ContainsKey(pat.PreferContactMethod)) {
				phoneNumber=new string(dictPrefContactMethod[pat.PreferContactMethod].Where(x => char.IsDigit(x)).ToArray());
				PlaceCall(phoneNumber);
			}
			else {
				List<string> listPhoneNumbers=new List<string>();
				listPhoneNumbers.Add("HmPhone: "+pat.HmPhone);
				listPhoneNumbers.Add("WkPhone: "+pat.WkPhone);
				listPhoneNumbers.Add("WirelessPhone: "+pat.WirelessPhone);
				using InputBox inputBox=new InputBox(Lan.g("DentalTek","Please select a phone number"),listPhoneNumbers);
				inputBox.comboSelection.SelectedIndex=0;//This could be set to always display the inputbox form to always allow users to choose the number.
				inputBox.ShowDialog();
				if(inputBox.DialogResult!=DialogResult.OK){
					return;
				}
				//Remove the titles that were added in addition to the phone numbers for UI purposes.
				phoneNumber=new string(listPhoneNumbers[inputBox.comboSelection.SelectedIndex].Where(x => char.IsDigit(x)).ToArray());
				if(!PlaceCall(phoneNumber)) {
					MsgBox.Show("DentalTek","Unable to place phone call.");
				}
			}

		}
	}
}