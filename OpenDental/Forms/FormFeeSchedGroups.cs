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
		private List<Clinic> _listClinicsAll;
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
			_listClinicsAll=Clinics.GetWhere(x => x.ClinicNum > -1 && x.IsHidden==false).OrderBy(x => x.Abbr).ToList(); //Get all Clinics from cache that are not hidden
			_listFeeSchedGroups=FeeSchedGroups.GetAll().OrderBy(x => x.Description).ToList();
			_listFeeSchedGroupsFiltered=ListTools.DeepCopy<FeeSchedGroup,FeeSchedGroup>(_listFeeSchedGroups);
			FillClinicCombo();
			FilterFeeSchedGroups();
		}

		private void FillClinicCombo() {
			comboClinic.Items.Clear();
			comboClinic.Items.Add("All");
			for(int i=0;i<_listClinicsAll.Count();i++) {
				comboClinic.Items.Add(_listClinicsAll[i].Abbr);
			}
			comboClinic.SelectedIndex=0;
		}

		//Used by comboClinic to filter the list of FeeSchedGroups
		private void comboClinic_SelectionChanged(object sender,EventArgs e) {
			FilterFeeSchedGroups();
		}

		private void FilterFeeSchedGroups() {
			List<FeeSched> listFeeSchedsFiltered=FeeScheds.GetWhere(x => x.Description.ToLower().Contains(textFeeSched.Text.ToLower()));
			//Clinic filter will be either a list of all clinics or a list containing only the selected clinic
			List<Clinic> listClinicsFiltered;
			if(comboClinic.SelectedIndex==0) {
				listClinicsFiltered=_listClinicsAll;
			}
			else {
				listClinicsFiltered=ListTools.FromSingle(_listClinicsAll[comboClinic.SelectedIndex-1]);
			}
			//This filter should return everything if both filters are empty.
			_listFeeSchedGroupsFiltered=_listFeeSchedGroups
				.Where(x => listFeeSchedsFiltered.Select(y => y.FeeSchedNum).Contains(x.FeeSchedNum))
				.Where(x => x.ListClinicNumsAll.Any(y => listClinicsFiltered.Select(z => z.ClinicNum).Contains(y)))
				.ToList();
			FillGridGroups();
			FillGridClinics();
		}

		private void FillGridGroups() {
			gridGroups.BeginUpdate();
			gridGroups.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Group Name"),200);
			gridGroups.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Fee Schedule"),75);
			gridGroups.Columns.Add(col);
			gridGroups.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listFeeSchedGroups.Count();i++) {
				row=new GridRow();
				row.Cells.Add(_listFeeSchedGroups[i].Description);
				row.Cells.Add(FeeScheds.GetDescription(_listFeeSchedGroups[i].FeeSchedNum));//Returns empty string if the FeeSched couldn't be found.
				row.Tag=_listFeeSchedGroups[i];
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
			gridClinics.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Abbr"),100);
			col.IsWidthDynamic=true;
			gridClinics.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),100);
			col.IsWidthDynamic=true;
			col.DynamicWeight=2;
			gridClinics.Columns.Add(col);
			gridClinics.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listClinicsForGroup.Count();i++) {
				row=new GridRow();
				row.Cells.Add(_listClinicsForGroup[i].Abbr);
				row.Cells.Add(_listClinicsForGroup[i].Description+(_listClinicsForGroup[i].IsHidden?" (Hidden)":""));
				row.Tag=_listClinicsForGroup[i];
				gridClinics.ListGridRows.Add(row);
			}
			gridClinics.EndUpdate();
		}

		private void gridGroups_CellClick(object sender,ODGridClickEventArgs e) {
			FillGridClinics();
		}

		private void gridGroups_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			FeeSchedGroup feeSchedGroup=(FeeSchedGroup)gridGroups.ListGridRows[e.Row].Tag;
			using FormFeeSchedGroupEdit formFeeSchedGroupEdit=new FormFeeSchedGroupEdit(feeSchedGroup);
			formFeeSchedGroupEdit.ShowDialog();
			if(formFeeSchedGroupEdit.DialogResult==DialogResult.OK) {
				FeeSchedGroups.Update(feeSchedGroup);
			}
			//Still need to refresh incase the user deleted the FeeSchedGroup, since it returns DialogResult.Cancel.
			FilterFeeSchedGroups();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FeeSchedGroup feeSchedGroup=new FeeSchedGroup();
			feeSchedGroup.ListClinicNumsAll=new List<long>();
			feeSchedGroup.IsNew=true;
			using FormFeeSchedGroupEdit formFeeSchedGroupEdit=new FormFeeSchedGroupEdit(feeSchedGroup);
			formFeeSchedGroupEdit.ShowDialog();
			if(formFeeSchedGroupEdit.DialogResult==DialogResult.OK) {
				FeeSchedGroups.Insert(feeSchedGroup);
				_listFeeSchedGroups.Add(feeSchedGroup);
				_listFeeSchedGroups=_listFeeSchedGroups.OrderBy(x => x.Description).ToList();
				FilterFeeSchedGroups();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}