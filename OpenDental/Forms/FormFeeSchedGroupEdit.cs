using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	///<summary>Clinics are assumed to be unrestricted by the time we get to this form.  If the user is restricted to a subset of clinics, bad things could occur.</summary>
	public partial class FormFeeSchedGroupEdit:FormODBase {
		///<summary>_feeSchedGroupCur is held by reference from the calling form when it is passed in.
		///All changes to this variable are immediately available to the calling form, so we should only write to this when we are hitting OK.</summary>
		private FeeSchedGroup _feeSchedGroup;
		private List<Clinic> _listClinicsInGroup;
		///<summary>Refresh list when the current group's fee schedule changes to quickly validate what clinics can be added to the current group.</summary>
		private List<FeeSchedGroup> _listFeeSchedGroupsOther=new List<FeeSchedGroup>();
		private List<FeeSched> _listFeeScheds;

		public FormFeeSchedGroupEdit(FeeSchedGroup feeSchedGroup) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_feeSchedGroup=feeSchedGroup;
			if(_feeSchedGroup.ListClinicNumsAll==null) {
				_listClinicsInGroup=Clinics.GetClinics(new List<long>());
			}
			else {
				_listClinicsInGroup=Clinics.GetClinics(_feeSchedGroup.ListClinicNumsAll);
			}
			if(_feeSchedGroup.FeeSchedNum>0) {
				_listFeeSchedGroupsOther=FeeSchedGroups.GetAllForFeeSched(_feeSchedGroup.FeeSchedNum)
					.FindAll(x => x.FeeSchedGroupNum!=_feeSchedGroup.FeeSchedGroupNum);
			}
			_listFeeScheds=FeeScheds.GetDeepCopy(true);
			//Global fee schedules cannot be localized, so there can't be clinic overrides for them. This block also exists in FormFeeSchedEdit.cs
			_listFeeScheds.RemoveAll(x => x.IsGlobal==true);
		}

		private void FormFeeSchedGroupEdit_Load(object sender,EventArgs e) {
			FillComboFeeScheds();
			RefreshAvailableClinics();
			textDescription.Text=_feeSchedGroup.Description.IsNullOrEmpty() ? "" : _feeSchedGroup.Description;
			if(_feeSchedGroup.IsNew) {
				butDelete.Visible=false;
			}
		}

		private void FillGridAvailable() {
			gridAvailable.BeginUpdate();
			gridAvailable.Columns.Clear();
			GridColumn col; 
			col=new GridColumn(Lan.g(this,"Abbr"),75);
			gridAvailable.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),200);
			gridAvailable.Columns.Add(col);
			gridAvailable.ListGridRows.Clear();
			GridRow row;
			List<Clinic> listClinicsAvailible=GetAvailableClinics();
			for(int i=0;i<listClinicsAvailible.Count();i++) {
				row=new GridRow();
				row.Cells.Add(listClinicsAvailible[i].Abbr);
				row.Cells.Add(listClinicsAvailible[i].Description+(listClinicsAvailible[i].IsHidden?" (Hidden)":""));
				row.Tag=listClinicsAvailible[i];
				gridAvailable.ListGridRows.Add(row);
			}
			gridAvailable.EndUpdate();
		}

		private void FillGridGroup() {
			gridGroup.BeginUpdate();
			gridGroup.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Abbr"),75);
			gridGroup.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),200);
			gridGroup.Columns.Add(col);
			gridGroup.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listClinicsInGroup.Count();i++) {
				row=new GridRow();
				row.Cells.Add(_listClinicsInGroup[i].Abbr);
				row.Cells.Add(_listClinicsInGroup[i].Description+(_listClinicsInGroup[i].IsHidden?" (Hidden)":""));
				row.Tag=_listClinicsInGroup[i];
				gridGroup.ListGridRows.Add(row);
			}
			gridGroup.EndUpdate();
		}

		private void FillComboFeeScheds() {
			comboFeeSched.Items.Clear();
			for(int i=0;i<_listFeeScheds.Count();i++) {
				comboFeeSched.Items.Add(_listFeeScheds[i].Description+(_listFeeScheds[i].IsHidden?" (Hidden)":""),_listFeeScheds[i]);
				if(_feeSchedGroup.FeeSchedNum==_listFeeScheds[i].FeeSchedNum) {
					comboFeeSched.SetSelectedKey<FeeSched>(_listFeeScheds[i].FeeSchedNum,x => x.FeeSchedNum);
				}
			}
		}

		private List<Clinic> GetAvailableClinics() {
			List<Clinic> listClinicsAvailable=new List<Clinic>();
			listClinicsAvailable=Clinics.GetForUserod(Security.CurUser);
			//Get list of Clinics in the current group.  Use in memory list to account for uncommitted changes.
			List<long> listClinicNumsInUse=_listClinicsInGroup.Select(x => x.ClinicNum).ToList();
			if(_listFeeSchedGroupsOther.Count>0) {
				for(int i=0;i<_listFeeSchedGroupsOther.Count();i++) {
					if(_listFeeSchedGroupsOther[i].FeeSchedGroupNum==_feeSchedGroup.FeeSchedGroupNum) {
						continue;
					}
					listClinicNumsInUse.AddRange(_listFeeSchedGroupsOther[i].ListClinicNumsAll);
				}
			}
			//Remove clinics currently in this group or in another group for the same schedule from the list of available clinics.
			listClinicsAvailable.RemoveAll(x => listClinicNumsInUse.Contains(x.ClinicNum));
			return listClinicsAvailable;
		}

		///<summary>Call this method after making changes to _listClinicsInGroup to update _listClinicsAvailable correctly.</summary>
		private void RefreshAvailableClinics() {
			//Fill the grids.
			FillGridAvailable();
			FillGridGroup();
		}

		///<summary>Checks that all information has been entered and checks if a clinic has been added to multiple groups for the same fee schedule.</summary>
		private bool ValidateUserInput() {
			string error="";
			if(String.IsNullOrWhiteSpace(textDescription.Text)) {
				error+=Lan.g(this,"Please enter a description.")+"\r\n";
			}
			if(comboFeeSched.SelectedIndex<0) {
				error+=Lan.g(this,"Please select a fee schedule.")+"\r\n";
			}
			string invalidClinics="";
			for(int i=0;i<_listClinicsInGroup.Count();i++) {
				//See if the current clinic is in another FeeSchedGroup's clinic list.
				//Ignore any matches for the FeeSchedGroup we are currently editing.
				if(_listFeeSchedGroupsOther.Any(x => x.ListClinicNumsAll.Contains(_listClinicsInGroup[i].ClinicNum))) { 
					//Insert a comma only if there are already clinics in the list.
					invalidClinics+="\r\n "+_listClinicsInGroup[i].Abbr;
				}
			}
			if(invalidClinics.Length>0) {
				error+=Lan.g(this,"The following clinic(s) already exist in a fee schedule group for the selected fee schedule:")+invalidClinics;
			}
			if(error.Length>0) {
				//Translation handled above due to variable nature of clinic.Abbr.
				MsgBox.Show(error);
				return false;
			}
			if(_listClinicsInGroup.IsNullOrEmpty()) {
				MsgBox.Show(this,"This group contains no clinics.  Clinics are required to make Fee Schedule Groups.");
				return false;
			}
			return true;
		}

		private void comboFeeSched_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboFeeSched.GetSelected<FeeSched>()!=null) {
				_listFeeSchedGroupsOther=FeeSchedGroups.GetAllForFeeSched(comboFeeSched.GetSelected<FeeSched>().FeeSchedNum)
				.FindAll(x => x.FeeSchedGroupNum!=_feeSchedGroup.FeeSchedGroupNum);
			}
			RefreshAvailableClinics();
		}

		private void butPickFeeSched_Click(object sender,EventArgs e) {
			List<GridColumn> listGridColumnHeaders=new List<GridColumn>(); 
			listGridColumnHeaders.Add(new GridColumn(Lan.g(this,"Description"),250));
			listGridColumnHeaders.Add(new GridColumn(Lan.g(this,"Hidden"),25));
			List<GridRow> listGridRowValues=new List<GridRow>();
			for(int i=0;i<_listFeeScheds.Count();i++) {
				GridRow row=new GridRow(_listFeeScheds[i].Description,_listFeeScheds[i].IsHidden?"X":"");
				row.Tag=_listFeeScheds[i];
				listGridRowValues.Add(row);
			}
			string formTitle=Lan.g(this,"Fee Schedule Picker");
			string gridTitle=Lan.g(this,"Fee Schedules");
			using FormGridSelection formGridSelection=new FormGridSelection(listGridColumnHeaders,listGridRowValues,formTitle,gridTitle);
			if(formGridSelection.ShowDialog()==DialogResult.OK) {
				comboFeeSched.SetSelectedKey<FeeSched>(((FeeSched)formGridSelection.ListSelectedTags[0]).FeeSchedNum, x => x.FeeSchedNum);
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(comboFeeSched.SelectedIndex<0) {
				MsgBox.Show(this,"Please select a fee schedule before selecting clinics.");
				return;
			}
			for(int i=0;i<gridAvailable.SelectedGridRows.Count();i++) {
				_listClinicsInGroup.Add((Clinic)gridAvailable.SelectedGridRows[i].Tag);
			}
			RefreshAvailableClinics();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(comboFeeSched.SelectedIndex<0) {
				MsgBox.Show(this,"Please select a fee schedule before selecting clinics.");
				return;
			}
			for(int i=0;i<gridGroup.SelectedGridRows.Count();i++)  {
				_listClinicsInGroup.Remove((Clinic)gridGroup.SelectedGridRows[i].Tag);
			}
			RefreshAvailableClinics();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//This button is hidden if the user is adding a new FeeSchedGroup.
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this Fee Schedule Group?")) {
				FeeSchedGroups.Delete(_feeSchedGroup.FeeSchedGroupNum);
			}
			DialogResult=DialogResult.Cancel;
			this.Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateUserInput()) {
				return;
			}
			FeeSched feeSched=comboFeeSched.GetSelected<FeeSched>();
			List<long> listClinicNumsNew=_listClinicsInGroup.Select(x => x.ClinicNum).ToList();
			//Initial fee sync for new groups or groups that were created without any clinics in the group, or if we just changed the Fee Schedule for the group.
			//If editing an existing fee schedule group that contains no clinic associations, treat it like a new group and set the initial fees.
			if(_feeSchedGroup.IsNew || _feeSchedGroup.ListClinicNumsAll.Count()<1 || _feeSchedGroup.FeeSchedNum!=feeSched.FeeSchedNum) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to set the initial group fees to a specific clinic's fees?"
					+"  Answering no will result in the default fees for the fee schedule being used."))
				{
					List<GridColumn> listGridColumnHeaders=new List<GridColumn>();
					listGridColumnHeaders.Add(new GridColumn(Lan.g(this,"Abbr"),75));
					listGridColumnHeaders.Add(new GridColumn(Lan.g(this,"Description"),200));
					List<GridRow> listGridRowValues=new List<GridRow>();
					for(int i=0;i<_listClinicsInGroup.Count();i++) {
						GridRow row=new GridRow(_listClinicsInGroup[i].Abbr,_listClinicsInGroup[i].Description);
						row.Tag=_listClinicsInGroup[i];
						listGridRowValues.Add(row);
					}
					string formTitle=Lan.g(this,"Clinic Picker");
					string gridTitle=Lan.g(this,"Clinics");
					using FormGridSelection formGridSelection=new FormGridSelection(listGridColumnHeaders,listGridRowValues,formTitle,gridTitle);
					if(formGridSelection.ShowDialog()!=DialogResult.OK) {
						MsgBox.Show(this,"A default clinic was not selected.");
						return;
					}
					long clinicNumMaster=((Clinic)formGridSelection.ListSelectedTags[0]).ClinicNum;//DialogResult.OK means a selection was made.
					//This list came from _listClinicsInGroup which was used to fill the grid picker that we get clinicNumMaster from.  We need to pop the master
					//clinic off the list while copying fee schedules or else it will be deleted before copying.
					//Give the user an out before potentially changing a lot of data in the db.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Fees are about to be updated.  Continue?")) {
						return;
					}
					ProgressOD progressOD=new ProgressOD();
					progressOD.ActionMain=() => {
						listClinicNumsNew.Remove(clinicNumMaster);
						FeeScheds.CopyFeeSchedule(feeSched,clinicNumMaster,0,feeSched,listClinicNumsNew,0);
						listClinicNumsNew.Add(clinicNumMaster);
					};
					progressOD.StartingMessage=Lans.g(this,"Creating Group, Please Wait...");
					progressOD.ShowDialogProgress();
					if(progressOD.IsCancelled){
						return;//fees could be partially copied which is no big deal.
					}
				}
				else {//Default fees.
					//Give the user an out before potentially changing a lot of data in the db.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Fees are about to be updated.  Continue?")) {
						return;
					}
					ProgressOD progressOD=new ProgressOD();
					progressOD.ActionMain=() => FeeScheds.CopyFeeSchedule(feeSched,0,0,feeSched,listClinicNumsNew,0);
					progressOD.StartingMessage=Lans.g(this,"Creating Group, Please Wait...");
					progressOD.ShowDialogProgress();
					if(progressOD.IsCancelled){
						return;
					}
				}
			}
			//Existing group, change the fees for all the new clinics in the group. Use the first clinic in the old clinic list as we already did an empty check
			//and the fees should have already been synched previously.
			else {
				if(listClinicNumsNew.Except(_feeSchedGroup.ListClinicNumsAll).Count()!=0 || _feeSchedGroup.ListClinicNumsAll.Except(listClinicNumsNew).Count()!=0) {
					//Give the user an out before potentially changing a lot of data in the db.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Clinics added to the group will have their fees updated to match the group.  Clinics removed from"+
						" the group will not have their fees changed.  Continue?")) {
						return;
					}
					//For an existing group, we can not guarantee that the ClinicNum we are using to copy fee schedules with is actually still in the group. It is ok to
					//use a ClinicNum that used to be in the group as the fees will still be in sync at this point.
					//Only update the fees for clinics that were just added to the group, don't attempt to update fess already in the group.
					List<long> listClinicNumsToSync=listClinicNumsNew.Where(x => !_feeSchedGroup.ListClinicNumsAll.Contains(x)).ToList();
					if(listClinicNumsToSync.Count>0) {
						FeeScheds.CopyFeeSchedule(feeSched,_feeSchedGroup.ListClinicNumsAll.First(),0,feeSched,listClinicNumsToSync,0);
					}
				}
			}
			_feeSchedGroup.Description=textDescription.Text;
			_feeSchedGroup.FeeSchedNum=feeSched.FeeSchedNum;
			_feeSchedGroup.ListClinicNumsAll=listClinicNumsNew;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}