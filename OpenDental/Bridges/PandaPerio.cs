using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	///<summary>This class is just an example template that we use when we build a new bridge.  Start with a copy of this.</summary>
	public class PandaPerio {

		///<summary></summary>
		public PandaPerio() {

		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
			else {
				string cmdline="OD14";
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
					cmdline+=" "+pat.PatNum.ToString();
				}
				else {
					cmdline+=" "+Tidy(pat.ChartNumber);
				}

				if(pat.FName!="") {
					cmdline+=" "+Tidy(pat.FName);
				}
				else {
					cmdline+=" NA";
				}
				cmdline+=" "+Tidy(pat.LName);
				if(pat.Birthdate.Year>1880) {
					cmdline+=" "+pat.Birthdate.ToString("MM-dd-yyyy");
				}
				else {
					cmdline+=" NA";
				}
				//SSNs which are all zeros will be treated as blank.  Needed for eCW, since eCW sets SSN to 000-00-0000 if the patient does not have an SSN.
				if(pat.SSN.Replace("0","").Trim()!="") {
					//Otherwise, output raw SSN:
					cmdline+=" "+pat.SSN;
				}
				else {
					cmdline+=" NA";
				}
				if(pat.HmPhone!="") {
					cmdline+=" "+Tidy(pat.HmPhone);
				}
				else {
					cmdline+=" NA";
				}
				if(pat.WkPhone!="") {
					cmdline+=" "+Tidy(pat.WkPhone);
				}
				else {
					cmdline+=" NA";
				}
				Referral referral = Referrals.GetReferralForPat(pat.PatNum);
				if(referral!=null && referral.IsDoctor) {
					cmdline+=" "+referral.ReferralNum.ToString();
					if(referral.FName!="") {
						cmdline+=" "+Tidy(referral.FName);
					}
					else {
						cmdline+=" NA";
					}
					if(referral.LName!="") {
						cmdline+=" "+Tidy(referral.LName);
					}
					else {
						cmdline+=" NA";
					}
					if(referral.Address!="") {
						cmdline+=" "+Tidy(referral.Address);
					}
					else {
						cmdline+=" NA";
					}
					if(referral.City!="") {
						cmdline+=" "+Tidy(referral.City);
					}
					else {
						cmdline+=" NA";
					}
					if(referral.ST!="") {
						cmdline+=" "+Tidy(referral.ST);
					}
					else {
						cmdline+=" NA";
					}
					if(referral.Zip!="") {
						cmdline+=" "+Tidy(referral.Zip);
					}
					else {
						cmdline+=" NA";
					}
				}
				else {
					cmdline+=" NA NA NA NA NA NA NA";
				}

				try {
					ODFileUtils.ProcessStart(path,cmdline);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
		}

		///<summary>Removes semicolons and replaces spaces with underscores.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","_");
			//for changing phone number format from (XXX)XXX-XXXX to XXX-XXX-XXXX
			retVal=retVal.Replace("(","");
			retVal=retVal.Replace(")","-");
			return retVal;
		}

	}
}







