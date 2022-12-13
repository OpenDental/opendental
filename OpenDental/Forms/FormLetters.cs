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
		private Patient _patient;
		private Letter _letter;
		///<summary>If this is not null, then this letter will be addressed to the referral rather than the patient.</summary>
		public Referral ReferralCur;
		//<summary>Only used if FuchsOptionsOn</summary>
		//private string ExtraImageToPrint;

		///<summary></summary>
		public FormLetters(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_patient=new Patient();
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
			_letter=listLetters.GetSelected<Letter>();
			StringBuilder stringBuilder=new StringBuilder();
			//return address
			//if (checkIncludeRet.Checked) {
				stringBuilder.Append(PrefC.GetString(PrefName.PracticeTitle) + "\r\n");
				stringBuilder.Append(PrefC.GetString(PrefName.PracticeAddress) + "\r\n");
				if (PrefC.GetString(PrefName.PracticeAddress2) != "")
					stringBuilder.Append(PrefC.GetString(PrefName.PracticeAddress2) + "\r\n");
				stringBuilder.Append(PrefC.GetString(PrefName.PracticeCity) + ", ");
				stringBuilder.Append(PrefC.GetString(PrefName.PracticeST) + "  ");
				stringBuilder.Append(PrefC.GetString(PrefName.PracticeZip) + "\r\n");
			//}
			//else {
			//	str.Append("\r\n\r\n\r\n\r\n");
			//}
			stringBuilder.Append("\r\n\r\n");
			//address
			if (ReferralCur == null) {
				stringBuilder.Append(_patient.FName + " " + _patient.MiddleI + " " + _patient.LName + "\r\n");
				stringBuilder.Append(_patient.Address + "\r\n");
				if (_patient.Address2 != "")
					stringBuilder.Append(_patient.Address2 + "\r\n");
				stringBuilder.Append(_patient.City + ", " + _patient.State + "  " + _patient.Zip);
			}
			else {
				stringBuilder.Append(Referrals.GetNameFL(ReferralCur.ReferralNum) + "\r\n");
				stringBuilder.Append(ReferralCur.Address + "\r\n");
				if (ReferralCur.Address2 != "")
					stringBuilder.Append(ReferralCur.Address2 + "\r\n");
				stringBuilder.Append(ReferralCur.City + ", " + ReferralCur.ST + "  " + ReferralCur.Zip);
			}
			stringBuilder.Append("\r\n\r\n\r\n\r\n");
			//date
			stringBuilder.Append(DateTime.Today.ToLongDateString() + "\r\n");
			//referral RE
			if (ReferralCur != null) {
				stringBuilder.Append(Lan.g(this, "RE Patient: ") + _patient.GetNameFL() + "\r\n");
			}
			stringBuilder.Append("\r\n");
			//greeting
			stringBuilder.Append(Lan.g(this, "Dear "));
			if (ReferralCur == null) {
				if (CultureInfo.CurrentCulture.Name == "en-GB") {
					if (_patient.Salutation != "")
						stringBuilder.Append(_patient.Salutation);
					else {
						if (_patient.Gender == PatientGender.Female) {
							stringBuilder.Append("Ms. " + _patient.LName);
						}
						else {
							stringBuilder.Append("Mr. " + _patient.LName);
						}
					}
				}
				else {
					if (_patient.Salutation != "")
						stringBuilder.Append(_patient.Salutation);
					else if (_patient.Preferred != "")
						stringBuilder.Append(_patient.Preferred);
					else
						stringBuilder.Append(_patient.FName);
				}
			}
			else {//referral
				stringBuilder.Append(ReferralCur.FName);
			}
			stringBuilder.Append(",\r\n\r\n");
			//body text
			stringBuilder.Append(_letter.BodyText);
			//closing
			if (CultureInfo.CurrentCulture.Name == "en-GB") {
				stringBuilder.Append("\r\n\r\nYours sincerely,\r\n\r\n\r\n\r\n");
			}
			else {
				stringBuilder.Append("\r\n\r\n" + Lan.g(this, "Sincerely,") + "\r\n\r\n\r\n\r\n");
			}
			stringBuilder.Append(PrefC.GetString(PrefName.PracticeTitle));
			textBody.Text = stringBuilder.ToString();
			//bodyChanged = false;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!WarnOK())
				return;
			using FormLetterEdit formLetterEdit=new FormLetterEdit();
			formLetterEdit.LetterCur=new Letter();
			formLetterEdit.IsNew=true;
			formLetterEdit.ShowDialog();
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
			using FormLetterEdit formLetterEdit=new FormLetterEdit();
			formLetterEdit.LetterCur=listLetters.GetSelected<Letter>();//just in case
			formLetterEdit.ShowDialog();
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





















