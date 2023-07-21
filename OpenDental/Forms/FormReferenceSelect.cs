using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReference:FormODBase {
		private DataTable _tableRef;
		public List<CustReference> ListCustReferencesSelected;
		public long PatNumGoto;
		private List<Def> _listDefsBillingTypes;

		public FormReference() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReference_Load(object sender,EventArgs e) {
			_listDefsBillingTypes=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillingType.Items.AddList(_listDefsBillingTypes,x => x.ItemName);
			listBillingType.SetAll(true);
			FillMain(true);
		}

		private void FillMain(bool limit) {
			int age=0;
			try {
				age=PIn.Int(textAge.Text);
			}
			catch { }
			int superFam=0;
			try {
				superFam=PIn.Int(textSuperFamily.Text);
			}
			catch { }
			long[] longArrayBillingTypes=new long[listBillingType.SelectedIndices.Count];
			if(listBillingType.SelectedIndices.Count!=0){
				for(int i=0;i<listBillingType.SelectedIndices.Count;i++) {
					longArrayBillingTypes[i]=_listDefsBillingTypes[listBillingType.SelectedIndices[i]].DefNum;
				}
			}
			_tableRef=CustReferences.GetReferenceTable(limit,longArrayBillingTypes,checkBadRefs.Checked,checkUsedRefs.Checked,checkGuarOnly.Checked,textCity.Text,textState.Text,
				textZip.Text,textAreaCode.Text,textSpecialty.Text,superFam,textLName.Text,textFName.Text,textPatNum.Text,age,textCountry.Text);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("PatNum",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("First Name",75);
			gridMain.Columns.Add(col);
			col=new GridColumn("Last Name",75);
			gridMain.Columns.Add(col);
			col=new GridColumn("HmPhone",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("State",45);
			gridMain.Columns.Add(col);
			col=new GridColumn("City",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Zip Code",60);
			gridMain.Columns.Add(col);
			col=new GridColumn("Country",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("Specialty",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("Age",40);
			gridMain.Columns.Add(col);
			col=new GridColumn("Super",50);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			col=new GridColumn("Last Used",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			col=new GridColumn("Times Used",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col);
			if(checkBadRefs.Checked) {
				col=new GridColumn("Bad",50);
				col.TextAlign=HorizontalAlignment.Center;
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableRef.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableRef.Rows[i]["PatNum"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["FName"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["LName"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["HmPhone"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["State"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["City"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["Zip"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["Country"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["Specialty"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["age"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["SuperFamily"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["DateMostRecent"].ToString());
				row.Cells.Add(_tableRef.Rows[i]["TimesUsed"].ToString());
				if(checkBadRefs.Checked) {
					row.Cells.Add(_tableRef.Rows[i]["IsBadRef"].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(0,true);
		}

		private void OnDataEntered() {
			if(checkRefresh.Checked) {
				FillMain(true);
			}
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			CustReference custReference=CustReferences.GetOne(PIn.Long(_tableRef.Rows[e.Row]["CustReferenceNum"].ToString()));
			using FormReferenceEdit formReferenceEdit=new FormReferenceEdit(custReference);
			formReferenceEdit.ShowDialog();
			FillMain(true);
		}

		private void goToFamilyToolStripMenuItem_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				return;
			}
			PatNumGoto=PIn.Long(_tableRef.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
			Close();
		}

		private void listBillingType_SelectionChangeCommitted(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void checkUsedRefs_Click(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void checkBadRefs_Click(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void butSearch_Click(object sender,EventArgs e) {
			FillMain(true);
		}

		private void butGetAll_Click(object sender,EventArgs e) {
			Cursor.Current=Cursors.WaitCursor;
			FillMain(false);
			Cursor.Current=Cursors.Default;
		}

		#region TextChanged
		private void textCity_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textState_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textZip_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textAreaCode_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textSpecialty_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textSuperFamily_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textLName_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textFName_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textPatNum_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textAge_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}

		private void textCountry_TextChanged(object sender,EventArgs e) {
			OnDataEntered();
		}
		#endregion

		#region KeyDown
		private void textCity_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down) {
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textState_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down) {
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textZip_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textAreaCode_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down) {
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textSpecialty_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textSuperFamily_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textLName_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textFName_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textPatNum_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void textAge_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				return;
			}
			gridMain_KeyDown(sender,e);
			gridMain.Invalidate();
			e.Handled=true;
		}

		private void gridMain_KeyDown(object sender,KeyEventArgs e) {

		}
		#endregion

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Select at least one reference.");
				return;
			}
			ListCustReferencesSelected=new List<CustReference>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				CustReference custReference=CustReferences.GetOne(PIn.Long(_tableRef.Rows[gridMain.SelectedIndices[i]]["CustReferenceNum"].ToString()));
				custReference.DateMostRecent=DateTime.Now;
				CustReferences.Update(custReference);
				ListCustReferencesSelected.Add(custReference);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}