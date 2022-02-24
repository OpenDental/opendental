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
		private DataTable RefTable;
		public List<CustReference> SelectedCustRefs;
		public long GotoPatNum;
		private List<Def> _listBillingTypeDefs;

		public FormReference() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReference_Load(object sender,EventArgs e) {
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillingType.Items.AddList(_listBillingTypeDefs,x => x.ItemName);
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
			long[] billingTypes=new long[listBillingType.SelectedIndices.Count];
			if(listBillingType.SelectedIndices.Count!=0){
				for(int i=0;i<listBillingType.SelectedIndices.Count;i++) {
					billingTypes[i]=_listBillingTypeDefs[listBillingType.SelectedIndices[i]].DefNum;
				}
			}
			RefTable=CustReferences.GetReferenceTable(limit,billingTypes,checkBadRefs.Checked,checkUsedRefs.Checked,checkGuarOnly.Checked,textCity.Text,textState.Text,
				textZip.Text,textAreaCode.Text,textSpecialty.Text,superFam,textLName.Text,textFName.Text,textPatNum.Text,age,textCountry.Text);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("PatNum",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("First Name",75);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Last Name",75);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("HmPhone",90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("State",45);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("City",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Zip Code",60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Country",90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Specialty",90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Age",40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Super",50);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Last Used",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Times Used",70);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col);
			if(checkBadRefs.Checked) {
				col=new GridColumn("Bad",50);
				col.TextAlign=HorizontalAlignment.Center;
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<RefTable.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(RefTable.Rows[i]["PatNum"].ToString());
				row.Cells.Add(RefTable.Rows[i]["FName"].ToString());
				row.Cells.Add(RefTable.Rows[i]["LName"].ToString());
				row.Cells.Add(RefTable.Rows[i]["HmPhone"].ToString());
				row.Cells.Add(RefTable.Rows[i]["State"].ToString());
				row.Cells.Add(RefTable.Rows[i]["City"].ToString());
				row.Cells.Add(RefTable.Rows[i]["Zip"].ToString());
				row.Cells.Add(RefTable.Rows[i]["Country"].ToString());
				row.Cells.Add(RefTable.Rows[i]["Specialty"].ToString());
				row.Cells.Add(RefTable.Rows[i]["age"].ToString());
				row.Cells.Add(RefTable.Rows[i]["SuperFamily"].ToString());
				row.Cells.Add(RefTable.Rows[i]["DateMostRecent"].ToString());
				row.Cells.Add(RefTable.Rows[i]["TimesUsed"].ToString());
				if(checkBadRefs.Checked) {
					row.Cells.Add(RefTable.Rows[i]["IsBadRef"].ToString());
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
			CustReference refCur=CustReferences.GetOne(PIn.Long(RefTable.Rows[e.Row]["CustReferenceNum"].ToString()));
			using FormReferenceEdit FormRE=new FormReferenceEdit(refCur);
			FormRE.ShowDialog();
			FillMain(true);
		}

		private void goToFamilyToolStripMenuItem_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				return;
			}
			GotoPatNum=PIn.Long(RefTable.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
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
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textState_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textZip_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textAreaCode_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textSpecialty_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textSuperFamily_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textLName_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textFName_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textPatNum_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void textAge_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				gridMain_KeyDown(sender,e);
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void gridMain_KeyDown(object sender,KeyEventArgs e) {

		}
		#endregion

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Select at least one reference.");
				return;
			}
			SelectedCustRefs=new List<CustReference>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				CustReference custRef=CustReferences.GetOne(PIn.Long(RefTable.Rows[gridMain.SelectedIndices[i]]["CustReferenceNum"].ToString()));
				custRef.DateMostRecent=DateTime.Now;
				CustReferences.Update(custRef);
				SelectedCustRefs.Add(custRef);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}