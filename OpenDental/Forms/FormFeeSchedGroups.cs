using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormFeeSchedGroups:FormODBase {
		///<summary>All clinics in the cache.</summary>
		private List<Clinic> _listAllClinics;
		///<summary>List of all clinics for the selected FeeSchedGroup in the grid.  Used to fill gridClinics.</summary>
		private List<Clinic> _listClinicsForGroup=new List<Clinic>();
		///<summary>List of all FeeSchedGroups in db.</summary>
		private List<FeeSchedGroup> _listFeeSchedGroups;
		///<summary>List of all FeeSchedGroups that will populate the grid. This is a filtered version of the list retrieved from the cache.</summary>
		private List<FeeSchedGroup> _listFeeSchedGroupsFiltered;

		public FormFeeSchedGroups() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFeeSchedGroups_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FilterFeeSchedGroups(),textFeeSched);
			//No restricting clinics because this window assumes that the user is an admin without restricted clinics
			_listAllClinics=Clinics.GetWhere(x => x.ClinicNum > -1 && x.IsHidden==false).OrderBy(x => x.Abbr).ToList(); //Get all Clinics from cache that are not hidden
			_listFeeSchedGroups=FeeSchedGroups.GetAll().OrderBy(x => x.Description).ToList();
			_listFeeSchedGroupsFiltered=ListTools.DeepCopy<FeeSchedGroup,FeeSchedGroup>(_listFeeSchedGroups);
			FillClinicCombo();
			FilterFeeSchedGroups();
		}

		private void FillClinicCombo() {
			comboClinic.Items.Clear();
			comboClinic.Items.Add("All");
			foreach(Clinic clinic in _listAllClinics){
				comboClinic.Items.Add(clinic.Abbr);
			}
			comboClinic.SelectedIndex=0;
		}

		//Used by comboClinic to filter the list of FeeSchedGroups
		private void comboClinic_SelectionChanged(object sender,EventArgs e) {
			FilterFeeSchedGroups();
		}

		private void FilterFeeSchedGroups() {
			List<FeeSched> listFilteredFeeScheds=FeeScheds.GetWhere(x => x.Description.ToLower().Contains(textFeeSched.Text.ToLower()));
			//Clinic filter will be either a list of all clinics or a list containing only the selected clinic
			List<Clinic> listFilteredClinics=(comboClinic.SelectedIndex==0) ? _listAllClinics : ListTools.FromSingle(_listAllClinics[comboClinic.SelectedIndex-1]);
			//This filter should return everything if both filters are empty.
			_listFeeSchedGroupsFiltered=_listFeeSchedGroups
				.Where(x => ListTools.In(x.FeeSchedNum,listFilteredFeeScheds.Select(y => y.FeeSchedNum)))
				.Where(x => x.ListClinicNumsAll.Any(y => ListTools.In(y,listFilteredClinics.Select(z => z.ClinicNum))))
				.ToList();
			FillGridGroups();
			FillGridClinics();
		}

		private void FillGridGroups() {
			gridGroups.BeginUpdate();
			gridGroups.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Group Name"),200);
			gridGroups.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Fee Schedule"),75);
			gridGroups.ListGridColumns.Add(col);
			gridGroups.ListGridRows.Clear();
			GridRow row;
			foreach(FeeSchedGroup feeSchedGroupCur in _listFeeSchedGroupsFiltered) {
				row=new GridRow();
				row.Cells.Add(feeSchedGroupCur.Description);
				row.Cells.Add(FeeScheds.GetDescription(feeSchedGroupCur.FeeSchedNum));//Returns empty string if the FeeSched couldn't be found.
				row.Tag=feeSchedGroupCur;
				gridGroups.ListGridRows.Add(row);
			} 
			gridGroups.EndUpdate();
		}

		private void FillGridClinics() {
			_listClinicsForGroup.Clear();
			if(gridGroups.GetSelectedIndex()>=0) {
				_listClinicsForGroup=Clinics.GetClinics(gridGroups.SelectedTag<FeeSchedGroup>().ListClinicNumsAll).OrderBy(x => x.Abbr).ToList();
			}
			gridClinics.BeginUpdate();
			gridClinics.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Abbr"),100){ IsWidthDynamic=true };
			gridClinics.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100){ IsWidthDynamic=true,DynamicWeight=2 };
			gridClinics.ListGridColumns.Add(col);
			gridClinics.ListGridRows.Clear();
			GridRow row;
			foreach(Clinic clinicCur in _listClinicsForGroup) {
				row=new GridRow();
				row.Cells.Add(clinicCur.Abbr);
				row.Cells.Add(clinicCur.Description+(clinicCur.IsHidden?" (Hidden)":""));
				row.Tag=clinicCur;
				gridClinics.ListGridRows.Add(row);
			}
			gridClinics.EndUpdate();
		}

		private void gridGroups_CellClick(object sender,ODGridClickEventArgs e) {
			FillGridClinics();
		}

		private void gridGroups_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			FeeSchedGroup feeSchedGroupCur=(FeeSchedGroup)gridGroups.ListGridRows[e.Row].Tag;
			using FormFeeSchedGroupEdit formFG=new FormFeeSchedGroupEdit(feeSchedGroupCur);
			formFG.ShowDialog();
			if(formFG.DialogResult==DialogResult.OK) {
				FeeSchedGroups.Update(feeSchedGroupCur);
			}
			//Still need to refresh incase the user deleted the FeeSchedGroup, since it returns DialogResult.Cancel.
			FilterFeeSchedGroups();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FeeSchedGroup feeSchedGroupNew=new FeeSchedGroup(){ ListClinicNumsAll=new List<long>(), IsNew=true };
			using FormFeeSchedGroupEdit formFG=new FormFeeSchedGroupEdit(feeSchedGroupNew);
			formFG.ShowDialog();
			if(formFG.DialogResult==DialogResult.OK) {
				FeeSchedGroups.Insert(feeSchedGroupNew);
				_listFeeSchedGroups.Add(feeSchedGroupNew);
				_listFeeSchedGroups=_listFeeSchedGroups.OrderBy(x => x.Description).ToList();
				FilterFeeSchedGroups();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}