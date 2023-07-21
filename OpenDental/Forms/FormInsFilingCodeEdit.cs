using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormInsFilingCodeEdit:FormODBase {
		public InsFilingCode InsFilingCodeCur;
		private List<InsFilingCodeSubtype> _listInsFilingCodeSubtypes;

		///<summary></summary>
		public FormInsFilingCodeEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsFilingCodeEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=InsFilingCodeCur.Descript;
			textEclaimCode.Text=InsFilingCodeCur.EclaimCode;
			comboGroup.Items.AddDefNone();
			comboGroup.Items.AddDefs(Defs.GetDefsForCategory(DefCat.InsuranceFilingCodeGroup,true));
			comboGroup.SetSelectedDefNum(InsFilingCodeCur.GroupType); 
			checkExcludeOtherCoverage.Checked=InsFilingCodeCur.ExcludeOtherCoverageOnPriClaims;
			FillGrid();
		}

		private void FillGrid() {
			InsFilingCodeSubtypes.RefreshCache();
			_listInsFilingCodeSubtypes=InsFilingCodeSubtypes.GetForInsFilingCode(InsFilingCodeCur.InsFilingCodeNum);
			gridInsFilingCodeSubtypes.BeginUpdate();
			gridInsFilingCodeSubtypes.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInsFilingCodes","Description"),100);
			gridInsFilingCodeSubtypes.Columns.Add(col);
			gridInsFilingCodeSubtypes.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listInsFilingCodeSubtypes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listInsFilingCodeSubtypes[i].Descript);
				gridInsFilingCodeSubtypes.ListGridRows.Add(row);
			}
			gridInsFilingCodeSubtypes.EndUpdate();
		}

		///<summary>Returns without doing anything if InsFilingCodeCur hasn't been inserted.</summary>
		private void DeleteInsFilingCodeAndSubTypes() {
			if(InsFilingCodeCur.InsFilingCodeNum<=0) {
				return;//This code hasn't been inserted so nothing to delete.
			}
			try {
				InsFilingCodes.Delete(InsFilingCodeCur.InsFilingCodeNum);
				InsFilingCodeSubtypes.DeleteForInsFilingCode(InsFilingCodeCur.InsFilingCodeNum);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return;
		}

		private void gridInsFilingCodeSubtypes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FrmInsFilingCodeSubtypeEdit frmInsFilingCodeSubtypeEdit=new FrmInsFilingCodeSubtypeEdit();
			frmInsFilingCodeSubtypeEdit.InsFilingCodeSubtypeCur=_listInsFilingCodeSubtypes[e.Row].Clone();
			frmInsFilingCodeSubtypeEdit.ShowDialog();
			if(!frmInsFilingCodeSubtypeEdit.IsDialogOK){
				FillGrid();
				return;
			}
			try{
				InsFilingCodeSubtypes.Update(frmInsFilingCodeSubtypeEdit.InsFilingCodeSubtypeCur);
			} 
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmInsFilingCodeSubtypeEdit frmInsFilingCodeSubtypeEdit=new FrmInsFilingCodeSubtypeEdit();
			frmInsFilingCodeSubtypeEdit.InsFilingCodeSubtypeCur=new InsFilingCodeSubtype();
			frmInsFilingCodeSubtypeEdit.InsFilingCodeSubtypeCur.IsNew=true;
			frmInsFilingCodeSubtypeEdit.ShowDialog();
			if(!frmInsFilingCodeSubtypeEdit.IsDialogOK){
				return;
			}
			if(InsFilingCodeCur.InsFilingCodeNum<=0) {
				//If we are adding a subtype to a new filing code, then we need to
				//save the filing code to the database to generate the InsFilingCodeNum,
				//so that we can then save the InsFilingCodeSubtype record with the correct
				//foreign key.
				SaveFilingCode();
			}
			frmInsFilingCodeSubtypeEdit.InsFilingCodeSubtypeCur.InsFilingCodeNum=InsFilingCodeCur.InsFilingCodeNum;
			try {
				InsFilingCodeSubtypes.Insert(frmInsFilingCodeSubtypeEdit.InsFilingCodeSubtypeCur);
			} 
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(InsFilingCodeCur.InsFilingCodeNum<=0){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this code?")) {
				return;
			}
			DeleteInsFilingCodeAndSubTypes();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(this.textDescription.Text==""){
				MessageBox.Show(Lan.g(this,"Please enter a description."));
				return;
			}
			if(this.textEclaimCode.Text==""){
				MessageBox.Show(Lan.g(this,"Please enter an electronic claim code."));
				return;
			}
			SaveFilingCode();
			DialogResult=DialogResult.OK;
		}

		private void SaveFilingCode(){
			InsFilingCodeCur.Descript=textDescription.Text;
			InsFilingCodeCur.EclaimCode=textEclaimCode.Text;
			InsFilingCodeCur.GroupType=comboGroup.GetSelectedDefNum();
			InsFilingCodeCur.ExcludeOtherCoverageOnPriClaims=checkExcludeOtherCoverage.Checked;
			if(InsFilingCodeCur.InsFilingCodeNum<=0) {
				InsFilingCodeCur.InsFilingCodeNum=InsFilingCodes.Insert(InsFilingCodeCur);
				return;
			}
			InsFilingCodes.Update(InsFilingCodeCur);
		}

		private void CheckSubtypeButtonEnabled(){
			this.butAdd.Enabled=(this.textDescription.Text!="" && this.textEclaimCode.Text!="");
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			CheckSubtypeButtonEnabled();
		}

		private void textEclaimCode_TextChanged(object sender,EventArgs e) {
			CheckSubtypeButtonEnabled();
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(InsFilingCodeCur.IsNew) {//Set outside this form.
				DeleteInsFilingCodeAndSubTypes();
			}
			DialogResult=DialogResult.Cancel;
		}

	}
}





















