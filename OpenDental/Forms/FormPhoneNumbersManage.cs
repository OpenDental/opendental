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
		private Patient Pat;

		public FormPhoneNumbersManage() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPhoneNumbersManage_Load(object sender,EventArgs e) {
			Pat=Patients.GetPat(PatNum);
			textName.Text=Pat.LName+", "+Pat.FName;
			textWkPhone.Text=Pat.WkPhone;
			textHmPhone.Text=Pat.HmPhone;
			textWirelessPhone.Text=Pat.WirelessPhone;
			textAddrNotes.Text=Pat.AddrNote;
			FillList();
		}

		private void FillList(){
			listOther.Items.Clear();
			listOther.Items.AddList(PhoneNumbers.GetPhoneNumbers(PatNum),x => x.PhoneNumberVal);
		}

		private void listOther_DoubleClick(object sender,EventArgs e) {
			if(listOther.SelectedIndex==-1) {
				return;
			}
			using InputBox input=new InputBox(null,new InputBoxParam(InputBoxType.ValidPhone,Lan.g(this,"Phone Number")));
			input.textResult.Text=listOther.Items.GetTextShowingAt(listOther.SelectedIndex);
			input.ShowDialog();
			if(input.DialogResult!=DialogResult.OK) {
				return;
			}
			PhoneNumbers.Update(listOther.GetSelected<PhoneNumber>());
			FillList();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox(null,new InputBoxParam(InputBoxType.ValidPhone,Lan.g(this,"Phone Number")));
			input.ShowDialog();
			if(input.DialogResult!=DialogResult.OK) {
				return;
			}
			PhoneNumber phoneNumber=new PhoneNumber();
			phoneNumber.PatNum=PatNum;
			phoneNumber.PhoneNumberVal=input.textResult.Text;
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

		private void butOK_Click(object sender,EventArgs e) {
			Patient PatOld=Pat.Copy();
			Pat.WkPhone=textWkPhone.Text;
			Pat.HmPhone=textHmPhone.Text;
			Pat.WirelessPhone=textWirelessPhone.Text;
			Pat.AddrNote=textAddrNotes.Text;
			Patients.Update(Pat,PatOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}