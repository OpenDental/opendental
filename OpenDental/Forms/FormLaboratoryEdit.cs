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
		public Laboratory LaboratoryCur;
		private List<LabTurnaround> _listLabTurnarounds;
		private List<SheetDef> _listSheetDefs;

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
			textDescription.Text=LaboratoryCur.Description;
			textPhone.Text=LaboratoryCur.Phone;
			textWirelessPhone.Text=LaboratoryCur.WirelessPhone;
			textAddress.Text=LaboratoryCur.Address;
			textCity.Text=LaboratoryCur.City;
			textState.Text=LaboratoryCur.State;
			textZip.Text=LaboratoryCur.Zip;
			textEmail.Text=LaboratoryCur.Email;
			textNotes.Text=LaboratoryCur.Notes;
			_listLabTurnarounds=LabTurnarounds.GetForLab(LaboratoryCur.LaboratoryNum);
			comboSlip.Items.Add(Lan.g(this,"Default"));
			comboSlip.SelectedIndex=0;
			_listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.LabSlip);
			for(int i=0;i<_listSheetDefs.Count;i++) {
				comboSlip.Items.Add(_listSheetDefs[i].Description);
				if(LaboratoryCur.Slip==_listSheetDefs[i].SheetDefNum) {
					comboSlip.SelectedIndex=i+1;
				}
			}
			checkIsHidden.Checked=LaboratoryCur.IsHidden;
			FillGrid();
		}

		private void FillGrid(){
			//does not refresh from database.
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableLabTurnaround","Service Description"),300);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabTurnaround","Days Published"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabTurnaround","Actual Days"),120);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listLabTurnarounds.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listLabTurnarounds[i].Description);
				if(_listLabTurnarounds[i].DaysPublished==0){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listLabTurnarounds[i].DaysPublished.ToString());
				}
				row.Cells.Add(_listLabTurnarounds[i].DaysActual.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormLabTurnaroundEdit formLabTurnaroundEdit=new FormLabTurnaroundEdit();
			formLabTurnaroundEdit.LabTurnaroundCur=new LabTurnaround();
			formLabTurnaroundEdit.ShowDialog();
			if(formLabTurnaroundEdit.DialogResult==DialogResult.OK){
				_listLabTurnarounds.Add(formLabTurnaroundEdit.LabTurnaroundCur);
				FillGrid(); 
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormLabTurnaroundEdit formLabTurnaroundEdit=new FormLabTurnaroundEdit();
			formLabTurnaroundEdit.LabTurnaroundCur=_listLabTurnarounds[e.Row];
			formLabTurnaroundEdit.ShowDialog();
			if(formLabTurnaroundEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butDeleteTurnaround_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			_listLabTurnarounds.RemoveAt(gridMain.GetSelectedIndex());
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
				Laboratories.Delete(LaboratoryCur.LaboratoryNum);
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
			LaboratoryCur.Description=textDescription.Text;
			LaboratoryCur.Phone=textPhone.Text;
			LaboratoryCur.WirelessPhone=textWirelessPhone.Text;
			LaboratoryCur.Address=textAddress.Text;
			LaboratoryCur.City=textCity.Text;
			LaboratoryCur.State=textState.Text;
			LaboratoryCur.Zip=textZip.Text;
			LaboratoryCur.Email=textEmail.Text;
			LaboratoryCur.Notes=textNotes.Text;
			LaboratoryCur.Slip=0;
			LaboratoryCur.IsHidden=checkIsHidden.Checked;
			if(comboSlip.SelectedIndex>0) {
				LaboratoryCur.Slip=_listSheetDefs[comboSlip.SelectedIndex-1].SheetDefNum;
			}
			try{
				if(IsNew){
					Laboratories.Insert(LaboratoryCur);
				}
				else{
					Laboratories.Update(LaboratoryCur);
				}
				LabTurnarounds.SetForLab(LaboratoryCur.LaboratoryNum,_listLabTurnarounds);
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





















