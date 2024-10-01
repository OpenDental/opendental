using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPhoneNumbersManage:FormODBase {
		public long PatNum;
		private Patient _patient;

		public FormPhoneNumbersManage() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPhoneNumbersManage_Load(object sender,EventArgs e) {
			_patient=Patients.GetPat(PatNum);
			textName.Text=_patient.LName+", "+_patient.FName;
			textWkPhone.Text=_patient.WkPhone;
			textHmPhone.Text=_patient.HmPhone;
			textWirelessPhone.Text=_patient.WirelessPhone;
			textAddrNotes.Text=_patient.AddrNote;
			FillList();
		}

		private void FillList(){
			listOther.Items.Clear();
			List<PhoneNumber> listPhoneNumbers=PhoneNumbers.GetPhoneNumbers(PatNum).FindAll(x => x.PhoneType==PhoneType.Other);
			listOther.Items.AddList(listPhoneNumbers,x => x.PhoneNumberVal);
		}

		private void listOther_DoubleClick(object sender,EventArgs e) {
			if(listOther.SelectedIndex==-1) {
				return;
			}
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.ValidPhone;
			inputBoxParam.LabelText=Lan.g(this,"Phone Number");
			inputBoxParam.Text=listOther.Items.GetTextShowingAt(listOther.SelectedIndex);
			InputBox inputBox=new InputBox(inputBoxParam);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return;
			}
			PhoneNumber phoneNumber=listOther.GetSelected<PhoneNumber>();
			phoneNumber.PhoneNumberVal=inputBox.StringResult;
			phoneNumber.PhoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(phoneNumber.PhoneNumberVal);
			PhoneNumbers.Update(phoneNumber);
			FillList();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.ValidPhone;
			inputBoxParam.LabelText=Lan.g(this,"Phone Number");
			InputBox inputBox=new InputBox(inputBoxParam);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return;
			}
			PhoneNumber phoneNumber=new PhoneNumber();
			phoneNumber.PatNum=PatNum;
			phoneNumber.PhoneNumberVal=inputBox.StringResult;
			phoneNumber.PhoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(phoneNumber.PhoneNumberVal);
			PhoneNumbers.Insert(phoneNumber);
			FillList();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(listOther.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a phone number first.");
				return;
			}
			PhoneNumbers.DeleteObject(listOther.GetSelected<PhoneNumber>().PhoneNumberNum);
			FillList();
		}

		private void butSave_Click(object sender,EventArgs e) {
			Patient patientOld=_patient.Copy();
			_patient.WkPhone=textWkPhone.Text;
			_patient.HmPhone=textHmPhone.Text;
			_patient.WirelessPhone=textWirelessPhone.Text;
			_patient.AddrNote=textAddrNotes.Text;
			Patients.Update(_patient,patientOld);
			DialogResult=DialogResult.OK;
		}

	}
}