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
		private FeeSchedGroup _feeSchedGroupCur;
		private List<Clinic> _listClinicsInGroup;
		///<summary>Refresh list when the current group's fee schedule changes to quickly validate what clinics can be added to the current group.</summary>
		private List<FeeSchedGroup> _listOtherGroupsWithFeeSched=new List<FeeSchedGroup>();
		private List<FeeSched> _listFeeScheds;

		public FormFeeSchedGroupEdit(FeeSchedGroup feeSchedGroupCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_feeSchedGroupCur=feeSchedGroupCur;
			_listClinicsInGroup=Clinics.GetClinics(_feeSchedGroupCur.ListClinicNumsAll??new List<long>());
			if(_feeSchedGroupCur.FeeSchedNum>0) {
				_listOtherGroupsWithFeeSched=FeeSchedGroups.GetAllForFeeSched(_feeSchedGroupCur.FeeSchedNum)
					.FindAll(x => x.FeeSchedGroupNum!=_feeSchedGroupCur.FeeSchedGroupNum);
			}
			_listFeeScheds=FeeScheds.GetDeepCopy(true);
			//Global fee schedules cannot be localized, so there can't be clinic overrides for them. This block also exists in FormFeeSchedEdit.cs
			_listFeeScheds.RemoveAll(x => x.IsGlobal==true);
		}

		private void FormFeeSchedGroupEdit_Load(object sender,EventArgs e) {
			FillComboFeeScheds();
			RefreshAvailableClinics();
			textDescription.Text=_feeSchedGroupCur.Description.IsNullOrEmpty() ? "" : _feeSchedGroupCur.Description;
			if(_feeSchedGroupCur.IsNew) {
				butDelete.Visible=false;
			}
		}

		private void FillGridAvailable() {
			gridAvailable.BeginUpdate();
			gridAvailable.ListGridColumns.Clear();
			GridColumn col; 
			col=new GridColumn(Lan.g(this,"Abbr"),75);
			gridAvailable.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),200);
			gridAvailable.ListGridColumns.Add(col);
			gridAvailable.ListGridRows.Clear();
			GridRow row;
			foreach(Clinic clinicAvailable in GetAvailableClinics()) {
				row=new GridRow();
				row.Cells.Add(clinicAvailable.Abbr);
				row.Cells.Add(clinicAvailable.Description+(clinicAvailable.IsHidden?" (Hidden)":""));
				row.Tag=clinicAvailable;
				gridAvailable.ListGridRows.Add(row);
			}
			gridAvailable.EndUpdate();
		}

		private void FillGridGroup() {
			gridGroup.BeginUpdate();
			gridGroup.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Abbr"),75);
			gridGroup.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),200);
			gridGroup.ListGridColumns.Add(col);
			gridGroup.ListGridRows.Clear();
			GridRow row;
			foreach(Clinic clinicGrouped in _listClinicsInGroup) {
				row=new GridRow();
				row.Cells.Add(clinicGrouped.Abbr);
				row.Cells.Add(clinicGrouped.Description+(clinicGrouped.IsHidden?" (Hidden)":""));
				row.Tag=clinicGrouped;
				gridGroup.ListGridRows.Add(row);
			}
			gridGroup.EndUpdate();
		}

		private void FillComboFeeScheds() {
			comboFeeSched.Items.Clear();
			foreach(FeeSched sched in _listFeeScheds) {
				comboFeeSched.Items.Add(sched.Description+(sched.IsHidden?" (Hidden)":""),sched);
				if(_feeSchedGroupCur.FeeSchedNum==sched.FeeSchedNum) {
					comboFeeSched.SetSelectedKey<FeeSched>(sched.FeeSchedNum,x => x.FeeSchedNum);
				}
			}
		}

		private List<Clinic> GetAvailableClinics() {
			List<Clinic> listAvailableClinics=new List<Clinic>();
			listAvailableClinics=Clinics.GetForUserod(Security.CurUser);
			//Get list of Clinics in the current group.  Use in memory list to account for uncommitted changes.
			List<long> listClinicNumsInUse=_listClinicsInGroup.Select(x => x.ClinicNum).ToList();
			if(_listOtherGroupsWithFeeSched.Count>0) {
				foreach(FeeSchedGroup feeGroup in _listOtherGroupsWithFeeSched) {
					if(feeGroup.FeeSchedGroupNum==_feeSchedGroupCur.FeeSchedGroupNum) {
						continue;
					}
					listClinicNumsInUse.AddRange(feeGroup.ListClinicNumsAll);
				}
			}
			//Remove clinics currently in this group or in another group for the same schedule from the list of available clinics.
			listAvailableClinics.RemoveAll(x => listClinicNumsInUse.Contains(x.ClinicNum));
			return listAvailableClinics;
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
			foreach(Clinic clinicCur in _listClinicsInGroup) {
				//See if the current clinic is in another FeeSchedGroup's clinic list.
				//Ignore any matches for the FeeSchedGroup we are currently editing.
				if(_listOtherGroupsWithFeeSched.Any(x => x.ListClinicNumsAll.Contains(clinicCur.ClinicNum))) { 
					//Insert a comma only if there are already clinics in the list.
					invalidClinics+="\r\n "+clinicCur.Abbr;
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
				_listOtherGroupsWithFeeSched=FeeSchedGroups.GetAllForFeeSched(comboFeeSched.GetSelected<FeeSched>().FeeSchedNum)
				.FindAll(x => x.FeeSchedGroupNum!=_feeSchedGroupCur.FeeSchedGroupNum);
			}
			RefreshAvailableClinics();
		}

		private void butPickFeeSched_Click(object sender,EventArgs e) {
			List<GridColumn> listColumnHeaders=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Description"),250),
				new GridColumn(Lan.g(this,"Hidden"),25)
			};
			List<GridRow> listRowValues=new List<GridRow>();
			_listFeeScheds.ForEach(x => {
				GridRow row=new GridRow(x.Description,x.IsHidden?"X":"");
				row.Tag=x;
				listRowValues.Add(row);
			});
			string formTitle=Lan.g(this,"Fee Schedule Picker");
			string gridTitle=Lan.g(this,"Fee Schedules");
			using FormGridSelection form=new FormGridSelection(listColumnHeaders,listRowValues,formTitle,gridTitle);
			if(form.ShowDialog()==DialogResult.OK) {
				comboFeeSched.SetSelectedKey<FeeSched>(((FeeSched)form.ListSelectedTags[0]).FeeSchedNum, x => x.FeeSchedNum);
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(comboFeeSched.SelectedIndex<0) {
				MsgBox.Show(this,"Please select a fee schedule before selecting clinics.");
				return;
			}
			foreach(GridRow row in gridAvailable.SelectedGridRows) {
				_listClinicsInGroup.Add((Clinic)row.Tag);
			}
			RefreshAvailableClinics();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(comboFeeSched.SelectedIndex<0) {
				MsgBox.Show(this,"Please select a fee schedule before selecting clinics.");
				return;
			}
			foreach(GridRow row in gridGroup.SelectedGridRows) {
				_listClinicsInGroup.Remove((Clinic)row.Tag);
			}
			RefreshAvailableClinics();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//This button is hidden if the user is adding a new FeeSchedGroup.
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this Fee Schedule Group?")) {
				FeeSchedGroups.Delete(_feeSchedGroupCur.FeeSchedGroupNum);
			}
			DialogResult=DialogResult.Cancel;
			this.Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateUserInput()) {
				return;
			}
			FeeSched feeSchedCur=comboFeeSched.GetSelected<FeeSched>();
			List<long> listClinicNumsNew=_listClinicsInGroup.Select(x => x.ClinicNum).ToList();
			//Initial fee sync for new groups or groups that were created without any clinics in the group, or if we just changed the Fee Schedule for the group.
			//If editing an existing fee schedule group that contains no clinic associations, treat it like a new group and set the initial fees.
			if(_feeSchedGroupCur.IsNew || _feeSchedGroupCur.ListClinicNumsAll.Count()<1 || _feeSchedGroupCur.FeeSchedNum!=feeSchedCur.FeeSchedNum) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to set the initial group fees to a specific clinic's fees?"
					+"  Answering no will result in the default fees for the fee schedule being used."))
				{
					List<GridColumn> listColumnHeaders=new List<GridColumn>() {
						new GridColumn(Lan.g(this,"Abbr"),75),
						new GridColumn(Lan.g(this,"Description"),200)
					};
					List<GridRow> listRowValues=new List<GridRow>();
					_listClinicsInGroup.ForEach(x => {
						GridRow row=new GridRow(x.Abbr,x.Description);
						row.Tag=x;
						listRowValues.Add(row);
					});
					string formTitle=Lan.g(this,"Clinic Picker");
					string gridTitle=Lan.g(this,"Clinics");
					using FormGridSelection form=new FormGridSelection(listColumnHeaders,listRowValues,formTitle,gridTitle);
					if(form.ShowDialog()!=DialogResult.OK) {
						MsgBox.Show(this,"A default clinic was not selected.");
						return;
					}
					long clinicNumMaster=((Clinic)form.ListSelectedTags[0]).ClinicNum;//DialogResult.OK means a selection was made.
					//This list came from _listClinicsInGroup which was used to fill the grid picker that we get clinicNumMaster from.  We need to pop the master
					//clinic off the list while copying fee schedules or else it will be deleted before copying.
					//Give the user an out before potentially changing a lot of data in the db.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Fees are about to be updated.  Continue?")) {
						return;
					}
					ProgressOD progressOD=new ProgressOD();
					progressOD.ActionMain=() => {
						listClinicNumsNew.Remove(clinicNumMaster);
						FeeScheds.CopyFeeSchedule(feeSchedCur,clinicNumMaster,0,feeSchedCur,listClinicNumsNew,0);
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
					progressOD.ActionMain=() => FeeScheds.CopyFeeSchedule(feeSchedCur,0,0,feeSchedCur,listClinicNumsNew,0);
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
				if(listClinicNumsNew.Except(_feeSchedGroupCur.ListClinicNumsAll).Count()!=0 || _feeSchedGroupCur.ListClinicNumsAll.Except(listClinicNumsNew).Count()!=0) {
					//Give the user an out before potentially changing a lot of data in the db.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Clinics added to the group will have their fees updated to match the group.  Clinics removed from"+
						" the group will not have their fees changed.  Continue?")) {
						return;
					}
					//For an existing group, we can not guarantee that the ClinicNum we are using to copy fee schedules with is actually still in the group. It is ok to
					//use a ClinicNum that used to be in the group as the fees will still be in sync at this point.
					//Only update the fees for clinics that were just added to the group, don't attempt to update fess already in the group.
					List<long> listClinicNumsToSync=listClinicNumsNew.Where(x => !_feeSchedGroupCur.ListClinicNumsAll.Contains(x)).ToList();
					if(listClinicNumsToSync.Count>0) {
						FeeScheds.CopyFeeSchedule(feeSchedCur,_feeSchedGroupCur.ListClinicNumsAll.First(),0,feeSchedCur,listClinicNumsToSync,0);
					}

				}
			}
			_feeSchedGroupCur.Description=textDescription.Text;
			_feeSchedGroupCur.FeeSchedNum=feeSchedCur.FeeSchedNum;
			_feeSchedGroupCur.ListClinicNumsAll=listClinicNumsNew;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}