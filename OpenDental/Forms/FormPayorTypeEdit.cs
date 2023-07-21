using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPayorTypeEdit:FormODBase {
		public bool IsNew;
		private PayorType _payorType;
		private int _indexSelected;//used to keep track of the selected index in the comboSopCode drop down box since we are setting the text differently than the contents of the drop down list
		private List<Sop> _listSops;

		public FormPayorTypeEdit(PayorType payorType) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_payorType=payorType;
			_listSops=Sops.GetDeepCopy();
		}

		private void FormPayorTypeEdit_Load(object sender,EventArgs e) {
			_indexSelected=-1;
			for(int i=0;i<_listSops.Count;i++) {
				comboSopCode.Items.Add(_listSops[i].SopCode+" - "+_listSops[i].Description);
				if(_payorType.SopCode==_listSops[i].SopCode) {
					comboSopCode.SelectedIndex=i;
				}
			}
			_indexSelected=comboSopCode.SelectedIndex;
			textDate.Text=_payorType.DateStart.ToShortDateString();
			textNote.Text=_payorType.Note;
		}

		//Note: Switching to ComboBoxOD removed setting up hierarchy/"tree" for SOP codes on Dropdown and DropdownClosed events. Allen and Jordan doesn't think it is necessary now.
		private void comboSopCode_SelectionChangeCommitted(object sender,EventArgs e) {
			_indexSelected=comboSopCode.SelectedIndex;
			comboSopCode.Items.Clear();
			for(int i=0;i<_listSops.Count;i++) {
				comboSopCode.Items.Add(_listSops[i].SopCode+" - "+_listSops[i].Description);
			}
			comboSopCode.SelectedIndex=_indexSelected;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entry?")) {
				return;
			}
			PayorTypes.Delete(_payorType.PayorTypeNum);
			DialogResult=DialogResult.OK;//Causes grid to refresh in case this amendment is not new.
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(comboSopCode.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an Sop Code.");
				return;
			}
			//Make sure there is not already a payor type entered with the selected date.
			//If there is, they should edit the existing one for that date.  There should not be two payor types that start on the same date.
			List<PayorType> listPayorTypes=PayorTypes.GetPatientData(_payorType.PatNum);
			for(int i=0;i<listPayorTypes.Count;i++) {
				//if updating an existing payor type, move past the current one
				if(listPayorTypes[i].PayorTypeNum==_payorType.PayorTypeNum) {
					continue;
				}
				if(listPayorTypes[i].DateStart==PIn.Date(textDate.Text)) {
					MsgBox.Show(this,"There is already a payor type with the selected start date.  Either change the date of this payor type or edit the existing payor type with this date.");
					return;
				}
			}
			_payorType.SopCode=_listSops[comboSopCode.SelectedIndex].SopCode;
			_payorType.Note=textNote.Text;
			_payorType.DateStart=PIn.Date(textDate.Text);
			if(IsNew) {
				PayorTypes.Insert(_payorType);
			}
			else {
				PayorTypes.Update(_payorType);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
