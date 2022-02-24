using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLaboratoryEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public Laboratory LabCur;
		private List<LabTurnaround> turnaroundList;
		private List<SheetDef> SlipList;

		///<summary></summary>
		public FormLaboratoryEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLaboratoryEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=LabCur.Description;
			textPhone.Text=LabCur.Phone;
			textWirelessPhone.Text=LabCur.WirelessPhone;
			textAddress.Text=LabCur.Address;
			textCity.Text=LabCur.City;
			textState.Text=LabCur.State;
			textZip.Text=LabCur.Zip;
			textEmail.Text=LabCur.Email;
			textNotes.Text=LabCur.Notes;
			turnaroundList=LabTurnarounds.GetForLab(LabCur.LaboratoryNum);
			comboSlip.Items.Add(Lan.g(this,"Default"));
			comboSlip.SelectedIndex=0;
			SlipList=SheetDefs.GetCustomForType(SheetTypeEnum.LabSlip);
			for(int i=0;i<SlipList.Count;i++) {
				comboSlip.Items.Add(SlipList[i].Description);
				if(LabCur.Slip==SlipList[i].SheetDefNum) {
					comboSlip.SelectedIndex=i+1;
				}
			}
			checkIsHidden.Checked=LabCur.IsHidden;
			FillGrid();
		}

		private void FillGrid(){
			//does not refresh from database.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableLabTurnaround","Service Description"),300);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabTurnaround","Days Published"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabTurnaround","Actual Days"),120);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<turnaroundList.Count;i++){
				row=new GridRow();
				row.Cells.Add(turnaroundList[i].Description);
				if(turnaroundList[i].DaysPublished==0){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(turnaroundList[i].DaysPublished.ToString());
				}
				row.Cells.Add(turnaroundList[i].DaysActual.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormLabTurnaroundEdit FormL=new FormLabTurnaroundEdit();
			FormL.LabTurnaroundCur=new LabTurnaround();
			FormL.ShowDialog();
			if(FormL.DialogResult==DialogResult.OK){
				turnaroundList.Add(FormL.LabTurnaroundCur);
				FillGrid(); 
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormLabTurnaroundEdit FormL=new FormLabTurnaroundEdit();
			FormL.LabTurnaroundCur=turnaroundList[e.Row];
			FormL.ShowDialog();
			if(FormL.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butDeleteTurnaround_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			turnaroundList.RemoveAt(gridMain.GetSelectedIndex());
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this entire Laboratory?")){
				return;
			}
			try{
				Laboratories.Delete(LabCur.LaboratoryNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			LabCur.Description=textDescription.Text;
			LabCur.Phone=textPhone.Text;
			LabCur.WirelessPhone=textWirelessPhone.Text;
			LabCur.Address=textAddress.Text;
			LabCur.City=textCity.Text;
			LabCur.State=textState.Text;
			LabCur.Zip=textZip.Text;
			LabCur.Email=textEmail.Text;
			LabCur.Notes=textNotes.Text;
			LabCur.Slip=0;
			LabCur.IsHidden=checkIsHidden.Checked;
			if(comboSlip.SelectedIndex>0) {
				LabCur.Slip=SlipList[comboSlip.SelectedIndex-1].SheetDefNum;
			}
			try{
				if(IsNew){
					Laboratories.Insert(LabCur);
				}
				else{
					Laboratories.Update(LabCur);
				}
				LabTurnarounds.SetForLab(LabCur.LaboratoryNum,turnaroundList);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















