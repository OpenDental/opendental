using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.IO;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLetters : FormODBase {
		//private bool localChanged;
		//private bool bodyChanged;
		//private int pagesPrinted=0;
		private Patient PatCur;
		private Letter LetterCur;
		///<summary>If this is not null, then this letter will be addressed to the referral rather than the patient.</summary>
		public Referral ReferralCur;
		//<summary>Only used if FuchsOptionsOn</summary>
		//private string ExtraImageToPrint;

		///<summary></summary>
		public FormLetters(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			PatCur=new Patient();
			Lan.F(this);
		}

		private void FormLetterSetup_Load(object sender,System.EventArgs e) {
			//if(PrefC.GetBool(PrefName.LettersIncludeReturnAddress")){
			//	checkIncludeRet.Checked=true;
			//}
			//if(PrefC.GetBool(PrefName.FuchsOptionsOn")) {
				//buttonTYDMF.Visible = true;
				//buttonTYREF.Visible = true;
			//}
			FillList();
		}

		private void FillList(){
			Letters.RefreshCache();
			listLetters.Items.Clear();
			listLetters.Items.AddList(Letters.GetDeepCopy(),x => x.Description);
			//no items are initially selected
		}

		private void listLetters_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			if (listLetters.SelectedIndex==-1) {
				return;
			}
			if (!WarnOK()) {
				return;
			}
			LetterCur=listLetters.GetSelected<Letter>();
			StringBuilder str=new StringBuilder();
			//return address
			//if (checkIncludeRet.Checked) {
				str.Append(PrefC.GetString(PrefName.PracticeTitle) + "\r\n");
				str.Append(PrefC.GetString(PrefName.PracticeAddress) + "\r\n");
				if (PrefC.GetString(PrefName.PracticeAddress2) != "")
					str.Append(PrefC.GetString(PrefName.PracticeAddress2) + "\r\n");
				str.Append(PrefC.GetString(PrefName.PracticeCity) + ", ");
				str.Append(PrefC.GetString(PrefName.PracticeST) + "  ");
				str.Append(PrefC.GetString(PrefName.PracticeZip) + "\r\n");
			//}
			//else {
			//	str.Append("\r\n\r\n\r\n\r\n");
			//}
			str.Append("\r\n\r\n");
			//address
			if (ReferralCur == null) {
				str.Append(PatCur.FName + " " + PatCur.MiddleI + " " + PatCur.LName + "\r\n");
				str.Append(PatCur.Address + "\r\n");
				if (PatCur.Address2 != "")
					str.Append(PatCur.Address2 + "\r\n");
				str.Append(PatCur.City + ", " + PatCur.State + "  " + PatCur.Zip);
			}
			else {
				str.Append(Referrals.GetNameFL(ReferralCur.ReferralNum) + "\r\n");
				str.Append(ReferralCur.Address + "\r\n");
				if (ReferralCur.Address2 != "")
					str.Append(ReferralCur.Address2 + "\r\n");
				str.Append(ReferralCur.City + ", " + ReferralCur.ST + "  " + ReferralCur.Zip);
			}
			str.Append("\r\n\r\n\r\n\r\n");
			//date
			str.Append(DateTime.Today.ToLongDateString() + "\r\n");
			//referral RE
			if (ReferralCur != null) {
				str.Append(Lan.g(this, "RE Patient: ") + PatCur.GetNameFL() + "\r\n");
			}
			str.Append("\r\n");
			//greeting
			str.Append(Lan.g(this, "Dear "));
			if (ReferralCur == null) {
				if (CultureInfo.CurrentCulture.Name == "en-GB") {
					if (PatCur.Salutation != "")
						str.Append(PatCur.Salutation);
					else {
						if (PatCur.Gender == PatientGender.Female) {
							str.Append("Ms. " + PatCur.LName);
						}
						else {
							str.Append("Mr. " + PatCur.LName);
						}
					}
				}
				else {
					if (PatCur.Salutation != "")
						str.Append(PatCur.Salutation);
					else if (PatCur.Preferred != "")
						str.Append(PatCur.Preferred);
					else
						str.Append(PatCur.FName);
				}
			}
			else {//referral
				str.Append(ReferralCur.FName);
			}
			str.Append(",\r\n\r\n");
			//body text
			str.Append(LetterCur.BodyText);
			//closing
			if (CultureInfo.CurrentCulture.Name == "en-GB") {
				str.Append("\r\n\r\nYours sincerely,\r\n\r\n\r\n\r\n");
			}
			else {
				str.Append("\r\n\r\n" + Lan.g(this, "Sincerely,") + "\r\n\r\n\r\n\r\n");
			}
			str.Append(PrefC.GetString(PrefName.PracticeTitle));
			textBody.Text = str.ToString();
			//bodyChanged = false;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!WarnOK())
				return;
			using FormLetterEdit FormLE=new FormLetterEdit();
			FormLE.LetterCur=new Letter();
			FormLE.IsNew=true;
			FormLE.ShowDialog();
			FillList();
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			if(!WarnOK()) {
				return;
			}
			if(listLetters.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
      using FormLetterEdit FormLE=new FormLetterEdit();
			FormLE.LetterCur=listLetters.GetSelected<Letter>();//just in case
			FormLE.ShowDialog();
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listLetters.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete letter permanently for all patients?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			Letters.Delete(listLetters.GetSelected<Letter>());
			FillList();
		}

		//private void checkIncludeRet_Click(object sender, System.EventArgs e) {	
		//	Prefs.UpdateBool(PrefName.LettersIncludeReturnAddress",checkIncludeRet.Checked);
		//	localChanged=true;
		//	CacheL.Refresh(InvalidType.Prefs);
		//}

		///<summary>If the user has selected a letter, and then edited it in the main textbox, this warns them before continuing.</summary>
		private bool WarnOK(){
			/*if(bodyChanged){
				if(!MsgBox.Show(this,true,
					"Any changes you made to the letter you were working on will be lost.  Do you wish to continue?"))
				{
					return false;
				}
			}*/
			return true;
		}

		private void textBody_TextChanged(object sender, System.EventArgs e) {
			//bodyChanged=true;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormLetters_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			//if(localChanged){
			//	DataValid.SetInvalid(InvalidType.Letters);
			//}
		}

		


		

		

		

		

		

		

		

		


	}
}





















