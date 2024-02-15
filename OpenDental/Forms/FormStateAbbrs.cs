using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormStateAbbrs:FormODBase {
		private bool _isChanged;
		public bool IsSelectionMode;
		public StateAbbr StateAbbrSelected;
		
		///<summary></summary>
		public FormStateAbbrs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormStateAbbrs_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode) {
				butAdd.Visible=false;
			}
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				this.Width+=100;//Also increases grid width due to anchoring.
			}
			FillGrid();
		}

		private void FillGrid(){
			long stateAbbrNumPreviousSelected=-1;
			int idxNewSelected=-1;
			if(gridMain.GetSelectedIndex()!=-1){
				stateAbbrNumPreviousSelected=((StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).StateAbbrNum;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormStateAbbrs","Description"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormStateAbbrs","Abbr"),70);
			gridMain.Columns.Add(col);
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				col=new GridColumn(Lan.g("FormStateAbbrs","Medicaid ID Length"),200);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<StateAbbr> listStateAbbrs=StateAbbrs.GetDeepCopy();
			for(int i=0;i<listStateAbbrs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listStateAbbrs[i].Description);
				row.Cells.Add(listStateAbbrs[i].Abbr);
				if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
					if(listStateAbbrs[i].MedicaidIDLength==0) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(listStateAbbrs[i].MedicaidIDLength.ToString());
					}
				}
				row.Tag=listStateAbbrs[i];
				gridMain.ListGridRows.Add(row);
				if(listStateAbbrs[i].StateAbbrNum==stateAbbrNumPreviousSelected) {
					idxNewSelected=i;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(idxNewSelected,true);
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"StateAbbrs");
			StateAbbr stateAbbr=new StateAbbr();
			stateAbbr.IsNew=true;
			using FormStateAbbrEdit formStateAbbrEdit=new FormStateAbbrEdit(stateAbbr);
			formStateAbbrEdit.ShowDialog();
			if(formStateAbbrEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_isChanged=true;
			Cache.Refresh(InvalidType.StateAbbrs);
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"StateAbbrs");
			if(IsSelectionMode) {
				StateAbbrSelected=(StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormStateAbbrEdit formStateAbbrEdit=new FormStateAbbrEdit((StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			formStateAbbrEdit.ShowDialog();
			if(formStateAbbrEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_isChanged=true;
			Cache.Refresh(InvalidType.StateAbbrs);
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsSelectionMode) {
				DialogResult=DialogResult.OK;
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a state.");
				return;
			}
			StateAbbrSelected=(StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void FormStateAbbrs_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.StateAbbrs);
			}
		}

	}
}