using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpHiddenPaySplits : FormODBase {
		private List<Provider> _listProviders;
		///<summary>List of all clinics the current user has access to, can include Unassigned/0 clinic.</summary>
		private List<Clinic> _listClinics;
		private List<Def> _listUnearnedTypes;

		///<summary></summary>
		public FormRpHiddenPaySplits(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpTpPreAllocation_Load(object sender,EventArgs e) {
			odDateRangePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
			odDateRangePicker.SetDateTimeTo(DateTime.Today);
			if(PrefC.HasClinicsEnabled) {
				labelClinic.Visible=true;
				checkAllClinics.Visible=true;
				checkAllClinics.Checked=true;
				listBoxClinic.Visible=true;
				listBoxClinic.SelectedIndices.Clear();
				_listClinics=Clinics.GetForUserod(Security.CurUser,(!Security.CurUser.ClinicIsRestricted),"Unassigned");
				foreach(Clinic clinic in _listClinics) {
					listBoxClinic.Items.Add(clinic.Abbr,clinic);
				}
			}
			else {
				_listClinics=new List<Clinic>();
			}
			_listProviders=Providers.GetListReports();
			_listProviders.Insert(0,Providers.GetUnearnedProv());
			checkAllProv.Checked=true;
			listBoxProv.Items.AddList(_listProviders,x => x.Abbr);
			_listUnearnedTypes=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).Where(x => !string.IsNullOrEmpty(x.ItemValue)).ToList();
			checkAllUnearnedTypes.Checked=true;
			listBoxUnearnedTypes.Items.AddList(_listUnearnedTypes,x => x.ItemName);
		}

		private void CheckAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listBoxProv.SelectedIndices.Clear();
			}
		}

		private void ListBoxProv_Click(object sender,EventArgs e) {
			if(listBoxProv.SelectedIndices.Count>0) {
				checkAllProv.Checked=false;
			}
		}

		private void CheckAllUnearnedTypes_Click(object sender,EventArgs e) {
			if(checkAllUnearnedTypes.Checked) {
				listBoxUnearnedTypes.SelectedIndices.Clear();
			}
		}

		private void ListBoxUnearnedTypes_Click(object sender,EventArgs e) {
			if(listBoxUnearnedTypes.SelectedIndices.Count>0) {
				checkAllUnearnedTypes.Checked=false;
			}
		}

		private void CheckAllClinics_Click(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				listBoxClinic.SelectedIndices.Clear();
			}
		}

		private void ListBoxClinic_Click(object sender,EventArgs e) {
			if(listBoxClinic.SelectedIndices.Count>0) {
				checkAllClinics.Checked=false;
			}
		}

		private bool IsValid() {
			if(!checkAllProv.Checked && listBoxProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return false;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClinics.Checked && listBoxClinic.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return false;
			}
			if(!checkAllUnearnedTypes.Checked && listBoxUnearnedTypes.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one unearned type must be selected.");
				return false;
			}
			return true;
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			List<long> listUnearnedTypeDefNums=new List<long>();
			string subtitleProvs="";
			string subtitleClinics="";
			string subtitleUnearned="";
			if(checkAllProv.Checked) {
				subtitleProvs="All Providers";
				listProvNums=_listProviders.Select(x => x.ProvNum).ToList();
			}
			else {
				subtitleProvs=string.Join(", ",listBoxProv.GetListSelected<Provider>().Select(x => x.Abbr));
				listProvNums=listBoxProv.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			}
			if(checkAllUnearnedTypes.Checked) {
				subtitleUnearned="All Hidden Unearned";
				listUnearnedTypeDefNums=_listUnearnedTypes.Select(x => x.DefNum).ToList();
			}
			else {
				subtitleUnearned=string.Join(", ",listBoxUnearnedTypes.GetListSelected<Def>().Select(x => x.ItemName));
				listUnearnedTypeDefNums=listBoxUnearnedTypes.GetListSelected<Def>().Select(x => x.DefNum).ToList();
			}
			if(PrefC.HasClinicsEnabled && checkAllClinics.Checked) {
				subtitleClinics="All Clinics";
				listClinicNums=listBoxClinic.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			else if(PrefC.HasClinicsEnabled && !checkAllClinics.Checked) {
				subtitleClinics=string.Join(", ",listBoxClinic.GetListSelected<Clinic>().Select(x => x.Abbr).ToList());
				listClinicNums=listBoxClinic.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			DataTable table=new DataTable();
			table=RpHiddenPaySplits.GetReportData(listProvNums,listUnearnedTypeDefNums,listClinicNums,PrefC.HasClinicsEnabled
				,odDateRangePicker.GetDateTimeFrom(),odDateRangePicker.GetDateTimeTo());
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Hidden Payment Splits");
			report.AddTitle("Title",Lan.g(this,"Hidden Payment Splits"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			report.AddSubTitle("UnearnedTypes",subtitleUnearned,fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			QueryObject query=report.AddQuery(table,"Hidden PaySplits");
			query.AddColumn("Date",90,FieldValueType.Date,font);
			query.AddColumn("Patient",140,FieldValueType.String,font);
			query.AddColumn("Provider",90,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",50,FieldValueType.String,font);
			}
			query.AddColumn("Code",65,FieldValueType.String,font);
			query.AddColumn("Description",220,FieldValueType.String,font);
			query.AddColumn("Amount",80,FieldValueType.Number,font);
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex formComplex=new FormReportComplex(report);
			formComplex.ShowDialog();
			DialogResult=DialogResult.OK;
		}
	}
}
