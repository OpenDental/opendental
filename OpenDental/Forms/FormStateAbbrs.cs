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
				butClose.Text="Cancel";
			}
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				this.Width+=100;//Also increases grid width due to anchoring.
			}
			FillGrid();
		}

		private void FillGrid(){
			long previousSelectedStateAbbrNum=-1;
			int newSelectedIdx=-1;
			if(gridMain.GetSelectedIndex()!=-1){
				previousSelectedStateAbbrNum=((StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).StateAbbrNum;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormStateAbbrs","Description"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormStateAbbrs","Abbr"),70);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
				col=new GridColumn(Lan.g("FormStateAbbrs","Medicaid ID Length"),200);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<StateAbbr> stateAbbrs=StateAbbrs.GetDeepCopy();
			for(int i=0;i<stateAbbrs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(stateAbbrs[i].Description);
				row.Cells.Add(stateAbbrs[i].Abbr);
				if(PrefC.GetBool(PrefName.EnforceMedicaidIDLength)) {
					if(stateAbbrs[i].MedicaidIDLength==0) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(stateAbbrs[i].MedicaidIDLength.ToString());
					}
				}
				row.Tag=stateAbbrs[i];
				gridMain.ListGridRows.Add(row);
				if(stateAbbrs[i].StateAbbrNum==previousSelectedStateAbbrNum) {
					newSelectedIdx=i;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(newSelectedIdx,true);
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"StateAbbrs");
			StateAbbr stateAbbrCur=new StateAbbr();
			stateAbbrCur.IsNew=true;
			using FormStateAbbrEdit FormSAE=new FormStateAbbrEdit(stateAbbrCur);
			FormSAE.ShowDialog();
			if(FormSAE.DialogResult!=DialogResult.OK) {
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
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"StateAbbrs");
			if(IsSelectionMode) {
				StateAbbrSelected=(StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormStateAbbrEdit FormSAE=new FormStateAbbrEdit((StateAbbr)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			FormSAE.ShowDialog();
			if(FormSAE.DialogResult!=DialogResult.OK) {
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

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormStateAbbrs_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.StateAbbrs);
			}
		}

	}
}
