using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormOperatories : FormODBase {
		///<summary>A list of all operatories used to sync at the end.</summary>
		private List<Operatory> _listOperatories;
		///<summary>Stale deep copy of _listOps to use with sync.</summary>
		private List<Operatory> _listOperatoriesOld;
		///<summary>List of conflict appointments to show the user.
		///Only used for the combine operatories tool</summary>
		public List<Appointment> ListAppointmentsConflicting=new List<Appointment>();
		///<summary>This reference is passed in because it's needed for the "Update Provs on Future Appts" tool.</summary>
		public ControlAppt ControlApptRef;

		///<summary></summary>
		public FormOperatories() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOperatories_Load(object sender,System.EventArgs e) {
			RefreshList();
		}

		private void RefreshList() {
			Cache.Refresh(InvalidType.Operatories);
			_listOperatories=Operatories.GetDeepCopy();//Already ordered by ItemOrder
			_listOperatoriesOld=_listOperatories.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void FillGrid(){
			List<long> listOpNumsSelected=gridMain.SelectedTags<Operatory>().Select(x => x.OperatoryNum).ToList();
			int scrollValueCur=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			int widthOpName=180;
			int widthClinic=85;
			if(!PrefC.HasClinicsEnabled) {
				//Clinics are hidden so add the width of the clinic column to the Op Name column because the clinic column will not show.
				widthOpName+=widthClinic;
			}
			GridColumn col=new GridColumn(Lan.g("TableOperatories","Op Name"),widthOpName);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Abbrev"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHidden"),64,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableOperatories","Clinic"),widthClinic);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableOperatories","Provider"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Hygienist"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHygiene"),64,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsWebSched"),74,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsNewPat"),75,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsExistPat"),75,HorizontalAlignment.Center) { IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			for(int i=0;i<_listOperatories.Count;i++){
				if(PrefC.HasClinicsEnabled 
					&& !comboClinic.IsAllSelected
					&& _listOperatories[i].ClinicNum!=comboClinic.ClinicNumSelected) 
				{
					continue;
				}
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listOperatories[i].OpName);
				row.Cells.Add(_listOperatories[i].Abbrev);
				if(_listOperatories[i].IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listOperatories[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(_listOperatories[i].ProvDentist));
				row.Cells.Add(Providers.GetAbbr(_listOperatories[i].ProvHygienist));
				if(_listOperatories[i].IsHygiene){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(_listOperatories[i].IsWebSched?"X":"");
				row.Cells.Add((_listOperatories[i].ListWSNPAOperatoryDefNums!=null && _listOperatories[i].ListWSNPAOperatoryDefNums.Count > 0) ? "X" : "");
				row.Cells.Add((_listOperatories[i].ListWSEPOperatoryDefNums!=null && _listOperatories[i].ListWSEPOperatoryDefNums.Count > 0) ? "X" : "");
				row.Tag=_listOperatories[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				Operatory operatory=(Operatory)gridMain.ListGridRows[i].Tag;
				if(listOpNumsSelected.Contains(operatory.OperatoryNum)) {
					gridMain.SetSelected(i,true);
				}
			}
			gridMain.ScrollValue=scrollValueCur;
		}

		private void gridMain_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormOperatoryEdit formOperatoryEdit=new FormOperatoryEdit((Operatory)gridMain.ListGridRows[e.Row].Tag);
			formOperatoryEdit.ListOperatories=_listOperatories;
			formOperatoryEdit.ControlApptRef=ControlApptRef;
			formOperatoryEdit.ShowDialog();
			FillGrid();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPickClinic_Click(object sender,EventArgs e) {
			using FormClinics formClinics=new FormClinics();
			formClinics.IsSelectionMode=true;
			formClinics.ShowDialog();
			if(formClinics.DialogResult!=DialogResult.OK) {
				return;
			}
			comboClinic.ClinicNumSelected=formClinics.ClinicNumSelected;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Operatory operatory=new Operatory();
			operatory.IsNew=true;
			if(PrefC.HasClinicsEnabled && !comboClinic.IsAllSelected && !comboClinic.IsNothingSelected) {
				operatory.ClinicNum=comboClinic.ClinicNumSelected;
				operatory.IsInHQView=false;
			}
			if(gridMain.SelectedIndices.Length>0){//a row is selected
				operatory.ItemOrder=gridMain.SelectedIndices[0];
			}
			else{
				operatory.ItemOrder=_listOperatories.Count;//goes at end of list
			}
			using FormOperatoryEdit formOperatoryEdit=new FormOperatoryEdit(operatory);
			formOperatoryEdit.ListOperatories=_listOperatories;
			formOperatoryEdit.IsNew=true;
			formOperatoryEdit.ShowDialog();
			if(formOperatoryEdit.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}
		
		private void butCombine_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			if(gridMain.SelectedIndices.Length<2) {
				MsgBox.Show(this,"Please select multiple items first while holding down the control key.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,
				"Combine all selected operatories into a single operatory?\r\n\r\n"
				+"This will affect all appointments set in these operatories and could take a while to run.  "
				+"The next window will let you select which operatory to keep when combining."))
			{
				return;
			}
			#region Get selected OperatoryNums
			bool hasNewOp=gridMain.SelectedIndices.ToList().Exists(x => ((Operatory)gridMain.ListGridRows[x].Tag).IsNew);
			if(hasNewOp) {
				//This is needed due to the user adding an operatory then clicking combine.
				//The newly added operatory does not hava and OperatoryNum , so sync.
				ReorderAndSync();
				DataValid.SetInvalid(InvalidType.Operatories);//With sync we don't know if anything changed.
				RefreshList();//Without this the user could cancel out of a prompt below and quickly close FormOperatories, causing a duplicate op from sync.
			}
			List<long> listOpNumsSelected=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listOpNumsSelected.Add(((Operatory)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag).OperatoryNum);
			}
			#endregion
			#region Determine what Op to keep as the 'master'
			using FormOperatoryPick formOperatoryPick=new FormOperatoryPick(_listOperatories.FindAll(x => listOpNumsSelected.Contains(x.OperatoryNum)));
			formOperatoryPick.ShowDialog();
			if(formOperatoryPick.DialogResult!=DialogResult.OK) {
				return;
			}
			long opNumMaster=formOperatoryPick.OperatoryNumSelected;
			#endregion
			#region Determine if any appts conflict exist and potentially show them
			//List of all appointments for all child ops. If conflict was detected the appointments OperatoryNum will bet set to -1
			List<Appointment> listAppointmentsToMerge=Operatories.MergeApptCheck(opNumMaster,listOpNumsSelected.FindAll(x => x!=opNumMaster));
			if(!PrefC.GetBool(PrefName.ApptsAllowOverlap)) { //We only want to check appt overlap and return if the customer has the relevant pref turned off
				ListAppointmentsConflicting=listAppointmentsToMerge.FindAll(x => x.Op!=opNumMaster && Operatories.HasConflict(x,listAppointmentsToMerge));
				if(ListAppointmentsConflicting.Count > 0) {//Appointments conflicts exist, can not merge
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Cannot merge operatories due to appointment conflicts.\r\n\r\n"
						+"These conflicts need to be resolved before combining can occur.\r\n"
						+"Click OK to view the conflicting appointments.")) 
					{
						ListAppointmentsConflicting.Clear();
						return;
					}
					Close();//Having ListConflictingAppts filled with appointments will cause outside windows that care to show the corresponding window.
					return;
				}
			}
			#endregion
			#region Final prompt, displays number of appts to move and the 'master' ops abbr.
			int apptCount=listAppointmentsToMerge.FindAll(x => x.Op!=opNumMaster).Count;
			if(apptCount>0) {
				string selectedOpName=_listOperatories.First(x => x.OperatoryNum==opNumMaster).Abbrev;//Safe
				if(MessageBox.Show(Lan.g(this,"Would you like to move")+" "+apptCount+" "
					+Lan.g(this,"appointments from their current operatories to")+" "+selectedOpName+"?\r\n\r\n"
					+Lan.g(this,"You cannot undo this!")
					,"WARNING"
					,MessageBoxButtons.YesNo)!=DialogResult.Yes) 
				{
					return;
				}
			}
			#endregion
			//Point of no return.
			if(!hasNewOp) {//Avoids running ReorderAndSync() twice.
				//The user could have made other changes within this window, we need to make sure to save those changes before merging.
				ReorderAndSync();
				DataValid.SetInvalid(InvalidType.Operatories);//With sync we don't know if anything changed.
			}
			try {
				Operatories.MergeOperatoriesIntoMaster(opNumMaster,listOpNumsSelected,listAppointmentsToMerge);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			MessageBox.Show(Lan.g("Operatories","The following operatories and all of their appointments were merged into the")
					+" "+_listOperatories.FirstOrDefault(x => x.OperatoryNum==opNumMaster).Abbrev+" "+Lan.g("Operatories","operatory:")+"\r\n"
					+string.Join(", ",_listOperatories.FindAll(x => x.OperatoryNum!=opNumMaster && listOpNumsSelected.Contains(x.OperatoryNum)).Select(x => x.Abbrev)));
			RefreshList();
			FillGrid();
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"You must first select a row.");
				return;
			}
			int indexSelected = gridMain.GetSelectedIndex();
			if(indexSelected==0){
				return;//already at the top
			}
			Operatory operatorySelected = (Operatory)gridMain.ListGridRows[indexSelected].Tag;
			Operatory operatoryAboveSelected = (Operatory)gridMain.ListGridRows[indexSelected - 1].Tag;
			if(!CanReorderOps(operatorySelected,operatoryAboveSelected)) {
				return;
			}
			int selectedItemOrder=operatorySelected.ItemOrder;
			//move selected item up
			operatorySelected.ItemOrder=operatoryAboveSelected.ItemOrder;
			//move the one above it down
			operatoryAboveSelected.ItemOrder=selectedItemOrder;
			//Swap positions
			_listOperatories = _listOperatories.OrderBy(x => x.ItemOrder).ToList();
			//FillGrid();  //We don't fill grid anymore because it takes too long and we dont need to pull any new data into the DB.
			SwapGridMainLocations(indexSelected,indexSelected-1);
			gridMain.SetSelected(indexSelected-1,true);
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"You must first select a row.");
				return;
			}
			int indexSelected = gridMain.GetSelectedIndex();
			if(indexSelected == gridMain.ListGridRows.Count - 1) {
				return;//already at the bottom
			}
			Operatory operatorySelected = (Operatory)gridMain.ListGridRows[indexSelected].Tag;
			Operatory operatoryBelowSelected = (Operatory)gridMain.ListGridRows[indexSelected + 1].Tag;
			if(!CanReorderOps(operatorySelected,operatoryBelowSelected)) {
				return;
			}
			int selectedItemOrder = operatorySelected.ItemOrder;
			//move selected item down
			operatorySelected.ItemOrder=operatoryBelowSelected.ItemOrder;
			//move the one below it up
			operatoryBelowSelected.ItemOrder=selectedItemOrder;
			//Swap positions
			_listOperatories = _listOperatories.OrderBy(x => x.ItemOrder).ToList();
			//FillGrid();  //We don't fill grid anymore because it takes too long and we dont need to pull any new data into the DB.
			SwapGridMainLocations(indexSelected,indexSelected+1);
			gridMain.SetSelected(indexSelected+1,true);
		}

		///<summary>Returns true if the two operatories can be reordered. Returns false and fills in the error string if they cannot.</summary>
		private bool CanReorderOps(Operatory operatory1,Operatory operatory2) {
			string strErr="";
			if(!PrefC.HasClinicsEnabled || comboClinic.IsAllSelected) {
				return true;
			}
			if(!operatory1.IsInHQView && !operatory2.IsInHQView) {
				return true;
			}
			strErr=Lan.g(this,"You cannot change the order of the Operatories")+" '"+operatory1.Abbrev+"' "+Lan.g(this,"and")+" '"+operatory2.Abbrev+"' "
				+Lan.g(this,"with Clinic")+" '"+comboClinic.GetSelectedAbbr()+"' "
				+Lan.g(this,"selected because it is also a member of a Headquarters Appointment View.") +" "
				+Lan.g(this,"You must set your clinic selection to 'All' to reorder these operatories.");
			MessageBox.Show(strErr); //already translated
			return false;
		}

		///<summary>Swaps two rows in the grid for use with the Up and Down buttons.  Does not edit lists or do any calls to cache or DB to refresh the grid.</summary>
		private void SwapGridMainLocations(int indxMoveFrom, int indxMoveTo) {
			gridMain.BeginUpdate();
			GridRow dataRow=gridMain.ListGridRows[indxMoveFrom];
			gridMain.ListGridRows.RemoveAt(indxMoveFrom);
			gridMain.ListGridRows.Insert(indxMoveTo,dataRow);
			gridMain.EndUpdate();		
		}

		///<summary>Syncs _listOps and _listOpsOld after correcting the order of _listOps.</summary>
		private void ReorderAndSync() {
			//Renumber the itemorders to match the grid.  In most cases this will not do anything, but will fix any duplicate itemorders.
			for(int i=0;i<_listOperatories.Count;i++) {
				_listOperatories[i].ItemOrder=i;
			}
			Operatories.Sync(_listOperatories,_listOperatoriesOld);
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormOperatories_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			ReorderAndSync();
			DataValid.SetInvalid(InvalidType.Operatories);//With sync we don't know if anything changed.
		}
	}
}





















