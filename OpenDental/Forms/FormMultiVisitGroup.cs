using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMultiVisitGroup:FormODBase {

		/// <summary> If true, then this form removed one or more ProcMultiVisit items from the db </summary>
		public bool Changed=false;
		public List<ProcMultiVisit> ListChartModulePMV;
		public List<DataRow> ListDataRows;
		/// <summary> The function that builds a grid row for display, given an input DataRow </summary>
		public Func<DataRow,GridRow> FuncBuildGridRow;
		public ListGridColumns GridColumns;
		public List<DisplayField> ListGridDisplayFields;
		public string GridTitle;

		public FormMultiVisitGroup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMultiVisitGroup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridGroupedProcs.BeginUpdate();
			gridGroupedProcs.ListGridRows.Clear();
			gridGroupedProcs.ListGridColumns.Clear();
			gridGroupedProcs.Title=GridTitle;
			for(int i=0;i<GridColumns.Count;i++) {
				gridGroupedProcs.ListGridColumns.Add(GridColumns[i]);
			}
			for(int i=0;i<ListDataRows.Count;i++) {
				GridRow gridRowFromDataRow=FuncBuildGridRow(ListDataRows[i]);
				gridGroupedProcs.ListGridRows.Add(gridRowFromDataRow);
			}
			gridGroupedProcs.EndUpdate();
			gridGroupedProcs.SetAll(true);//So the user can ungroup all when opening the form
		}

		private void gridGroupedProcs_Click(object sender,EventArgs e) {
			if(gridGroupedProcs.SelectedIndices.Length==0) {
				butUngroup.Enabled=false;
			}
			else {
				butUngroup.Enabled=true;
			}
		}

		private void butUngroup_Click(object sender,EventArgs e) {
			Changed=true;
			if(gridGroupedProcs.ListGridRows.Count-gridGroupedProcs.SelectedIndices.Length==1) {//One is not selected
				//Select all procedures in the form
				for(int i=0;i<gridGroupedProcs.ListGridRows.Count;i++) {
					gridGroupedProcs.SetSelected(i,true);
				}
			}
			//Get the ProcNum of each selected procedure
			long[] arraySelectedProcNums=new long[gridGroupedProcs.SelectedIndices.Length];
			for(int i=0;i<gridGroupedProcs.SelectedIndices.Length;i++) {
				DataRow row=(DataRow)gridGroupedProcs.ListGridRows[gridGroupedProcs.SelectedIndices[i]].Tag;
				arraySelectedProcNums[i]=PIn.Long(row["ProcNum"].ToString());
			}
			bool isInvalid=false;
			//Get the ProcMultiVisit associated with each procedure
			List<ProcMultiVisit> listProcMVs=ProcMultiVisits.GetGroupsForProcsFromDb(arraySelectedProcNums);
			for(int i=0;i<arraySelectedProcNums.Length;i++) {
				ProcMultiVisit pmv=listProcMVs.FirstOrDefault(x => x.ProcNum==arraySelectedProcNums[i]);
				if(pmv!=null) {//Could have been already ungrouped in another window/pc, possibly
					ProcMultiVisits.Delete(pmv.ProcMultiVisitNum);
					ListChartModulePMV.RemoveAll(x => x.ProcMultiVisitNum==pmv.ProcMultiVisitNum);
					isInvalid=true;
				}
				//Remove the procedure rows from this form
				ListDataRows.RemoveAll(row => PIn.Long(row["ProcNum"].ToString())==arraySelectedProcNums[i]);
			}
			//Signal that one or more ProcMultiVisit objects were removed from the DB
			if(isInvalid) {
				Signalods.SetInvalid(InvalidType.ProcMultiVisits);
			}
			ProcMultiVisits.RefreshCache();
			if(ListDataRows.Count==0) {
				MsgBox.Show(this,"All procedures removed from group.");
				Close();
			}
			else {
				MsgBox.Show(this,"Procedure(s) removed from group.");
				FillGrid();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}