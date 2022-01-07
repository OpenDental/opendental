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
		private PayorType PayorTypeCur;
		private int _selectedIndex;//used to keep track of the selected index in the comboSopCode drop down box since we are setting the text differently than the contents of the drop down list
		private List<Sop> _listSops;

		public FormPayorTypeEdit(PayorType payorType) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PayorTypeCur=payorType;
			_listSops=Sops.GetDeepCopy();
		}

		private void FormPayorTypeEdit_Load(object sender,EventArgs e) {
			_selectedIndex=-1;
			for(int i=0;i<_listSops.Count;i++) {
				comboSopCode.Items.Add(_listSops[i].SopCode+" - "+_listSops[i].Description);
				if(PayorTypeCur.SopCode==_listSops[i].SopCode) {
					comboSopCode.SelectedIndex=i;
				}
			}
			_selectedIndex=comboSopCode.SelectedIndex;
			textDate.Text=PayorTypeCur.DateStart.ToShortDateString();
			textNote.Text=PayorTypeCur.Note;
		}

		private void comboSopCode_DropDown(object sender,EventArgs e) {
			comboSopCode.Items.Clear();
			for(int i=0;i<_listSops.Count;i++) {
				comboSopCode.Items.Add("".PadRight(_listSops[i].SopCode.Length*4-4,' ')+_listSops[i].SopCode+" - "+_listSops[i].Description);
			}
			comboSopCode.SelectedIndex=_selectedIndex;
		}

		private void comboSopCode_SelectionChangeCommitted(object sender,EventArgs e) {
			_selectedIndex=comboSopCode.SelectedIndex;
			comboSopCode.Items.Clear();
			for(int i=0;i<_listSops.Count;i++) {
				comboSopCode.Items.Add(_listSops[i].SopCode+" - "+_listSops[i].Description);
			}
			comboSopCode.SelectedIndex=_selectedIndex;
		}

		private void comboSopCode_DropDownClosed(object sender,EventArgs e) {
			_selectedIndex=comboSopCode.SelectedIndex;
			//if they expanded the drop down and then collapsed it without changing their selection, re-fill combo box without spaces for heirarchy
			//we don't want to do this every time because you can see the text changing and it is an annoyance, better to do in SelectionChangeCommitted
			if(comboSopCode.Items.Count>2 && comboSopCode.Items[1].ToString().Length>0 && comboSopCode.Items[1].ToString().Substring(0,1)==" ") {
				comboSopCode.Items.Clear();
				for(int i=0;i<_listSops.Count;i++) {
					comboSopCode.Items.Add(_listSops[i].SopCode+" - "+_listSops[i].Description);
				}
			}
			comboSopCode.SelectedIndex=_selectedIndex;
		}
		
		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entry?")) {
				return;
			}
			PayorTypes.Delete(PayorTypeCur.PayorTypeNum);
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
			List<PayorType> listPayorTypes=PayorTypes.Refresh(PayorTypeCur.PatNum);
			for(int i=0;i<listPayorTypes.Count;i++) {
				//if updating an existing payor type, move past the current one
				if(listPayorTypes[i].PayorTypeNum==PayorTypeCur.PayorTypeNum) {
					continue;
				}
				if(listPayorTypes[i].DateStart==PIn.Date(textDate.Text)) {
					MsgBox.Show(this,"There is already a payor type with the selected start date.  Either change the date of this payor type or edit the existing payor type with this date.");
					return;
				}
			}
			PayorTypeCur.SopCode=_listSops[comboSopCode.SelectedIndex].SopCode;
			PayorTypeCur.Note=textNote.Text;
			PayorTypeCur.DateStart=PIn.Date(textDate.Text);
			if(IsNew) {
				PayorTypes.Insert(PayorTypeCur);
			}
			else {
				PayorTypes.Update(PayorTypeCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
