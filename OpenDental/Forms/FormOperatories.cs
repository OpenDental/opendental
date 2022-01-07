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
		private List<Operatory> _listOps;
		///<summary>Stale deep copy of _listOps to use with sync.</summary>
		private List<Operatory> _listOpsOld;
		///<summary>List of conflict appointments to show the user.
		///Only used for the combine operatories tool</summary>
		public List<Appointment> ListConflictingAppts=new List<Appointment>();
		///<summary>This reference is passed in because it's needed for the "Update Provs on Future Appts" tool.</summary>
		public ControlAppt ContrApptRef;

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
			_listOps=Operatories.GetDeepCopy();//Already ordered by ItemOrder
			_listOpsOld=_listOps.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void FillGrid(){
			List<long> listSelectedOpNums=gridMain.SelectedTags<Operatory>().Select(x => x.OperatoryNum).ToList();
			int scrollValueCur=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			int opNameWidth=180;
			int clinicWidth=85;
			if(!PrefC.HasClinicsEnabled) {
				//Clinics are hidden so add the width of the clinic column to the Op Name column because the clinic column will not show.
				opNameWidth+=clinicWidth;
			}
			GridColumn col=new GridColumn(Lan.g("TableOperatories","Op Name"),opNameWidth);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Abbrev"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHidden"),64,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableOperatories","Clinic"),clinicWidth);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableOperatories","Provider"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Hygienist"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHygiene"),64,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsWebSched"),74,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsNewPat"),75,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsExistPat"),75,HorizontalAlignment.Center) { IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			for(int i=0;i<_listOps.Count;i++){
				if(PrefC.HasClinicsEnabled 
					&& !comboClinic.IsAllSelected
					&& _listOps[i].ClinicNum!=comboClinic.SelectedClinicNum) 
				{
					continue;
				}
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listOps[i].OpName);
				row.Cells.Add(_listOps[i].Abbrev);
				if(_listOps[i].IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listOps[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(_listOps[i].ProvDentist));
				row.Cells.Add(Providers.GetAbbr(_listOps[i].ProvHygienist));
				if(_listOps[i].IsHygiene){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(_listOps[i].IsWebSched?"X":"");
				row.Cells.Add((_listOps[i].ListWSNPAOperatoryDefNums!=null && _listOps[i].ListWSNPAOperatoryDefNums.Count > 0) ? "X" : "");
				row.Cells.Add((_listOps[i].ListWSEPOperatoryDefNums!=null && _listOps[i].ListWSEPOperatoryDefNums.Count > 0) ? "X" : "");
				row.Tag=_listOps[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				Operatory op=(Operatory)gridMain.ListGridRows[i].Tag;
				if(ListTools.In(op.OperatoryNum,listSelectedOpNums)) {
					gridMain.SetSelected(i,true);
				}
			}
			gridMain.ScrollValue=scrollValueCur;
		}

		private void gridMain_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormOperatoryEdit FormOE=new FormOperatoryEdit((Operatory)gridMain.ListGridRows[e.Row].Tag);
			FormOE.ListOps=_listOps;
			FormOE.ContrApptRef=ContrApptRef;
			FormOE.ShowDialog();
			FillGrid();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPickClinic_Click(object sender,EventArgs e) {
			using FormClinics FormC=new FormClinics();
			FormC.IsSelectionMode=true;
			FormC.ShowDialog();
			if(FormC.DialogResult!=DialogResult.OK) {
				return;
			}
			comboClinic.SelectedClinicNum=FormC.SelectedClinicNum;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Operatory opCur=new Operatory();
			opCur.IsNew=true;
			if(PrefC.HasClinicsEnabled && !comboClinic.IsAllSelected && !comboClinic.IsNothingSelected) {
				opCur.ClinicNum=comboClinic.SelectedClinicNum;
			}
			if(gridMain.SelectedIndices.Length>0){//a row is selected
				opCur.ItemOrder=gridMain.SelectedIndices[0];
			}
			else{
				opCur.ItemOrder=_listOps.Count;//goes at end of list
			}
			using FormOperatoryEdit FormE=new FormOperatoryEdit(opCur);
			FormE.ListOps=_listOps;
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}
		
		private void butCombine_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
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
			List<long> listSelectedOpNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listSelectedOpNums.Add(((Operatory)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag).OperatoryNum);
			}
			#endregion
			#region Determine what Op to keep as the 'master'
			using FormOperatoryPick FormOP=new FormOperatoryPick(_listOps.FindAll(x => listSelectedOpNums.Contains(x.OperatoryNum)));
			FormOP.ShowDialog();
			if(FormOP.DialogResult!=DialogResult.OK) {
				return;
			}
			long masterOpNum=FormOP.SelectedOperatoryNum;
			#endregion
			#region Determine if any appts conflict exist and potentially show them
			//List of all appointments for all child ops. If conflict was detected the appointments OperatoryNum will bet set to -1
			List<ODTuple<Appointment,bool>> listTupleApptsToMerge=Operatories.MergeApptCheck(masterOpNum,listSelectedOpNums.FindAll(x => x!=masterOpNum));
			if(!PrefC.GetYN(PrefName.ApptsAllowOverlap)) { //We only want to check appt overlap and return if the customer has the relevant pref turned off
				ListConflictingAppts=listTupleApptsToMerge.Where(x => x.Item2).Select(x => x.Item1).ToList();
				if(ListConflictingAppts.Count > 0) {//Appointments conflicts exist, can not merge
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Cannot merge operatories due to appointment conflicts.\r\n\r\n"
						+"These conflicts need to be resolved before combining can occur.\r\n"
						+"Click OK to view the conflicting appointments.")) 
					{
						ListConflictingAppts.Clear();
						return;
					}
					Close();//Having ListConflictingAppts filled with appointments will cause outside windows that care to show the corresponding window.
					return;
				}
			}
			List<Appointment> listApptsToMerge=listTupleApptsToMerge.Select(x => x.Item1).ToList();
			#endregion
			#region Final prompt, displays number of appts to move and the 'master' ops abbr.
			int apptCount=listApptsToMerge.FindAll(x => x.Op!=masterOpNum).Count;
			if(apptCount > 0) {
				string selectedOpName=_listOps.First(x => x.OperatoryNum==masterOpNum).Abbrev;//Safe
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
				Operatories.MergeOperatoriesIntoMaster(masterOpNum,listSelectedOpNums,listApptsToMerge);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			MessageBox.Show(Lan.g("Operatories","The following operatories and all of their appointments were merged into the")
					+" "+_listOps.FirstOrDefault(x => x.OperatoryNum==masterOpNum).Abbrev+" "+Lan.g("Operatories","operatory:")+"\r\n"
					+string.Join(", ",_listOps.FindAll(x => x.OperatoryNum!=masterOpNum && listSelectedOpNums.Contains(x.OperatoryNum)).Select(x => x.Abbrev)));
			RefreshList();
			FillGrid();
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"You must first select a row.");
				return;
			}
			int selected = gridMain.GetSelectedIndex();
			if(selected==0){
				return;//already at the top
			}
			Operatory selectedOp = (Operatory)gridMain.ListGridRows[selected].Tag;
			Operatory aboveSelectedOp = (Operatory)gridMain.ListGridRows[selected - 1].Tag;
			string strErr;
			if(!CanReorderOps(selectedOp,aboveSelectedOp,out strErr)) {
				MessageBox.Show(strErr); //already translated
				return;
			}
			int selectedItemOrder=selectedOp.ItemOrder;
			//move selected item up
			selectedOp.ItemOrder=aboveSelectedOp.ItemOrder;
			//move the one above it down
			aboveSelectedOp.ItemOrder=selectedItemOrder;
			//Swap positions
			_listOps = _listOps.OrderBy(x => x.ItemOrder).ToList();
			//FillGrid();  //We don't fill grid anymore because it takes too long and we dont need to pull any new data into the DB.
			SwapGridMainLocations(selected,selected-1);
			gridMain.SetSelected(selected-1,true);
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"You must first select a row.");
				return;
			}
			int selected = gridMain.GetSelectedIndex();
			if(selected == gridMain.ListGridRows.Count - 1) {
				return;//already at the bottom
			}
			Operatory selectedOp = (Operatory)gridMain.ListGridRows[selected].Tag;
			Operatory belowSelectedOp = (Operatory)gridMain.ListGridRows[selected + 1].Tag;
			string strErr;
			if(!CanReorderOps(selectedOp,belowSelectedOp,out strErr)) {
				MessageBox.Show(strErr); //already translated
				return;
			}
			int selectedItemOrder = selectedOp.ItemOrder;
			//move selected item down
			selectedOp.ItemOrder=belowSelectedOp.ItemOrder;
			//move the one below it up
			belowSelectedOp.ItemOrder=selectedItemOrder;
			//Swap positions
			_listOps = _listOps.OrderBy(x => x.ItemOrder).ToList();
			//FillGrid();  //We don't fill grid anymore because it takes too long and we dont need to pull any new data into the DB.
			SwapGridMainLocations(selected,selected+1);
			gridMain.SetSelected(selected+1,true);
		}

		///<summary>Returns true if the two operatories can be reordered. Returns false and fills in the error string if they cannot.</summary>
		private bool CanReorderOps(Operatory op1,Operatory op2,out string strErrorMsg) {
			strErrorMsg="";
			if(!PrefC.HasClinicsEnabled || comboClinic.IsAllSelected) {
				return true;
			}
			if(!op1.IsInHQView && !op2.IsInHQView) {
				return true;
			}
			strErrorMsg=Lan.g(this,"You cannot change the order of the Operatories")+" '"+op1.Abbrev+"' "+Lan.g(this,"and")+" '"+op2.Abbrev+"' "
				+Lan.g(this,"with Clinic")+" '"+comboClinic.GetSelectedAbbr()+"' "
				+Lan.g(this,"selected because it is also a member of a Headquarters Appointment View.") +" "
				+Lan.g(this,"You must set your clinic selection to 'All' to reorder these operatories.");
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
			for(int i=0;i<_listOps.Count;i++) {
				_listOps[i].ItemOrder=i;
			}
			Operatories.Sync(_listOps,_listOpsOld);
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





















