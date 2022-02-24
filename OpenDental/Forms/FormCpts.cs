using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.IO;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCpts:FormODBase {
		public bool IsSelectionMode;
		public Cpt SelectedCpt;
		private List<Cpt> listCpts;

		public FormCpts() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSnomeds_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butClose.Text=Lan.g(this,"Cancel");
			}
			else {
				butOK.Visible=false;
			}
			ActiveControl=textCode;
		}
		
		private void butSearch_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("CPT Code",100);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn("Deprecated",75,HorizontalAlignment.Center);
			//gridMain.Columns.Add(col);
			col=new GridColumn("Description",500);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn("Used By CQM's",75);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			listCpts=Cpts.GetBySearchText(textCode.Text);
			//List<ODGridRow> listAll=new List<ODGridRow>();//for sorting grid after it has been filled.
			for(int i=0;i<listCpts.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listCpts[i].CptCode);
				row.Cells.Add(listCpts[i].Description);
				//row.Cells.Add(EhrCodes.GetMeasureIdsForCode(listCpts[i].SnomedCode,"SNOMEDCT"));
				row.Tag=listCpts[i];;
				//listAll.Add(row);
				gridMain.ListGridRows.Add(row);
			}
			//listAll.Sort(SortMeasuresMet);
			//for(int i=0;i<listAll.Count;i++) {
			//	gridMain.Rows.Add(listAll[i]);
			//}
			gridMain.EndUpdate();
		}

		///<summary>Sort function to put the codes that apply to the most number of CQM's at the top so the user can see which codes they should select.</summary>
		//private int SortMeasuresMet(ODGridRow row1,ODGridRow row2) {
		//	//First sort by the number of measures the codes apply to in a comma delimited list
		//	int diff=row2.Cells[2].Text.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries).Length-row1.Cells[2].Text.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries).Length;
		//	if(diff!=0) {
		//		return diff;
		//	}
		//	try {
		//		//if the codes apply to the same number of CQMs, order by the code values
		//		return PIn.Long(row1.Cells[0].Text).CompareTo(PIn.Long(row2.Cells[0].Text));
		//	}
		//	catch(Exception ex) {
		//		return 0;
		//	}
		//}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				SelectedCpt=(Cpt)gridMain.ListGridRows[e.Row].Tag;
				DialogResult=DialogResult.OK;
				return;
			}
			//changed=true;
			//FormSnomedEdit FormSE=new FormSnomedEdit((Snomed)gridMain.Rows[e.Row].Tag);
			//FormSE.ShowDialog();
			//if(FormSE.DialogResult!=DialogResult.OK) {
			//	return;
			//}
			//FillGrid();
		}

		/*private void butAdd_Click(object sender,EventArgs e) {
			//TODO: Either change to adding a snomed code instead of an ICD9 or don't allow users to add SNOMED codes other than importing.
			changed=true;
			Snomed snomed=new Snomed();
			FormSnomedEdit FormI=new FormSnomedEdit(snomed);
			FormI.IsNew=true;
			FormI.ShowDialog();
			FillGrid();
		}*/

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless IsSelectionMode
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SelectedCpt=(Cpt)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

	}
}