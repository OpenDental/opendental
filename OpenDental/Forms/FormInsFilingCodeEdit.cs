using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormInsFilingCodeEdit:FormODBase {
		public InsFilingCode InsFilingCodeCur;
		private List <InsFilingCodeSubtype> insFilingCodeSubtypes;

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
			FillGrid();
		}

		private void FillGrid() {
			InsFilingCodeSubtypes.RefreshCache();
			insFilingCodeSubtypes=InsFilingCodeSubtypes.GetForInsFilingCode(InsFilingCodeCur.InsFilingCodeNum);
			gridInsFilingCodeSubtypes.BeginUpdate();
			gridInsFilingCodeSubtypes.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInsFilingCodes","Description"),100);
			gridInsFilingCodeSubtypes.ListGridColumns.Add(col);
			gridInsFilingCodeSubtypes.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<insFilingCodeSubtypes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(insFilingCodeSubtypes[i].Descript);
				gridInsFilingCodeSubtypes.ListGridRows.Add(row);
			}
			gridInsFilingCodeSubtypes.EndUpdate();
		}

		private void gridInsFilingCodeSubtypes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormInsFilingCodeSubtypeEdit FormI=new FormInsFilingCodeSubtypeEdit();
			FormI.InsFilingCodeSubtypeCur=insFilingCodeSubtypes[e.Row].Clone();
			FormI.ShowDialog();
			if(FormI.DialogResult==DialogResult.OK){
				try {
					InsFilingCodeSubtypes.Update(FormI.InsFilingCodeSubtypeCur);
				} 
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormInsFilingCodeSubtypeEdit FormI=new FormInsFilingCodeSubtypeEdit();
			FormI.InsFilingCodeSubtypeCur=new InsFilingCodeSubtype();
			FormI.InsFilingCodeSubtypeCur.IsNew=true;
			FormI.ShowDialog();
			if(FormI.DialogResult==DialogResult.OK) {
				if(InsFilingCodeCur.IsNew){
					//If we are adding a subtype to a new filing code, then we need to
					//save the filing code to the database to generate the InsFilingCodeNum,
					//so that we can then save teh InsFilingCodeSubtype record with the correct
					//foreign key.
					SaveFilingCode();
					InsFilingCodeCur.IsNew=false;
				}
				FormI.InsFilingCodeSubtypeCur.InsFilingCodeNum=InsFilingCodeCur.InsFilingCodeNum;
				try {
					InsFilingCodeSubtypes.Insert(FormI.InsFilingCodeSubtypeCur);
				} 
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				FillGrid();
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(InsFilingCodeCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this code?")) {
				return;
			}
			try{
				InsFilingCodes.Delete(InsFilingCodeCur.InsFilingCodeNum);
				InsFilingCodeSubtypes.DeleteForInsFilingCode(InsFilingCodeCur.InsFilingCodeNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
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
			if(comboGroup.GetSelectedDefNum()==0) {
				MsgBox.Show(this,"Please select a group.");
				return;
			}
			SaveFilingCode();
			DialogResult=DialogResult.OK;
		}

		private void SaveFilingCode(){
			InsFilingCodeCur.Descript=textDescription.Text;
			InsFilingCodeCur.EclaimCode=textEclaimCode.Text;
			InsFilingCodeCur.GroupType=comboGroup.GetSelectedDefNum();
			try {
				if(InsFilingCodeCur.IsNew) {
					InsFilingCodes.Insert(InsFilingCodeCur);
				}
				else {
					InsFilingCodes.Update(InsFilingCodeCur);
				}
			} 
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
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
			DialogResult=DialogResult.Cancel;
		}

	}
}





















