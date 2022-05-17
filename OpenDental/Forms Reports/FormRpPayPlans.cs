using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental
{
	/// <summary>
	/// Summary description for FormRpApptWithPhones.
	/// </summary>
	public partial class FormRpPayPlans:FormODBase {
		private List<Clinic> _listClinics;
		//private int pagesPrinted;
		private ErrorProvider errorProvider1=new ErrorProvider();
		//private DataTable BirthdayTable;
		//private int patientsPrinted;
		//private PrintDocument pd;
		//private OpenDental.UI.PrintPreview printPreview;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpPayPlans()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpPayPlans_Load(object sender, System.EventArgs e){
			dateStart.Value=DateTime.Today;
			dateEnd.Value=DateTime.Today;
			checkHideCompletePlans.Checked=true;
			_listProviders=Providers.GetListReports();
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			listProv.SetAll(true);
			checkAllProv.Checked=true;
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
			}
			else {
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				if(!Security.CurUser.ClinicIsRestricted) {
					listClin.Items.Add(Lan.g(this,"Unassigned"));
					listClin.SetSelected(0);
				}
				for(int i=0;i<_listClinics.Count;i++) {
					listClin.Items.Add(_listClinics[i].Abbr);
					if(Clinics.ClinicNum==0) {
						listClin.SetSelected(listClin.Items.Count-1);
						checkAllClin.Checked=true;
					}
					if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
						listClin.SelectedIndices.Clear();
						listClin.SetSelected(listClin.Items.Count-1);
					}
				}
			}
		}

		private void checkAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			else {
				listProv.ClearSelected();
			}
		}

		private void listProv_Click(object sender,EventArgs e) {
			if(listProv.SelectedIndices.Count>0) {
				checkAllProv.Checked=false;
			}
		}

		private void checkAllClin_Click(object sender,EventArgs e) {
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			else {
				listClin.ClearSelected();
			}
		}

		private void checkHasDateRange_Click(object sender,EventArgs e) {
			if(checkHasDateRange.Checked) {
				dateStart.Enabled=true;
				dateEnd.Enabled=true;
			}
			else {
				dateStart.Enabled=false;
				dateEnd.Enabled=false;
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one provider.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {//Using clinics
				if(listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"Please select at least one clinic.");
					return;
				}
			}
			if(dateStart.Value>dateEnd.Value) {
				MsgBox.Show(this,"Start date cannot be greater than the end date.");
				return;
			}
			ReportComplex report=new ReportComplex(true,true);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
			}
			if(checkAllProv.Checked) {
				for(int i=0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
			}
			if(PrefC.HasClinicsEnabled) {
				for(int i=0;i<listClin.SelectedIndices.Count;i++) {
					if(Security.CurUser.ClinicIsRestricted) {
						listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]].ClinicNum);//we know that the list is a 1:1 to _listClinics
					}
					else {
						if(listClin.SelectedIndices[i]==0) {
							listClinicNums.Add(0);
						}
						else {
							listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]-1].ClinicNum);//Minus 1 from the selected index
						}
					}
				}
				if(checkAllClin.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
					listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
				}
			}
			DisplayPayPlanType displayPayPlanType;
			if(radioInsurance.Checked) {
				displayPayPlanType=DisplayPayPlanType.Insurance;
			}
			else if(radioPatient.Checked) {
				displayPayPlanType=DisplayPayPlanType.Patient;
			}
			else {
				displayPayPlanType=DisplayPayPlanType.Both;
			}
			bool isPayPlanV2=(PrefC.GetInt(PrefName.PayPlansVersion)==2);
			DataSet ds=RpPayPlan.GetPayPlanTable(dateStart.Value,dateEnd.Value,listProvNums,listClinicNums,checkAllProv.Checked
					,displayPayPlanType,checkHideCompletePlans.Checked,checkShowFamilyBalance.Checked,checkHasDateRange.Checked,isPayPlanV2);
			DataTable table=ds.Tables["Clinic"];
			DataTable tableTotal=ds.Tables["Total"];
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"PaymentPlans");
			report.AddTitle("Title",Lan.g(this,"Payment Plans"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			if(checkHasDateRange.Checked) {
				report.AddSubTitle("Date SubTitle",dateStart.Value.ToShortDateString()+" - "+dateEnd.Value.ToShortDateString(),fontSubTitle);
			}
			else{
				report.AddSubTitle("Date SubTitle",DateTime.Today.ToShortDateString(),fontSubTitle);
			}
			QueryObject query;
			if(PrefC.HasClinicsEnabled) {
				query=report.AddQuery(table,"","clinicName",SplitByKind.Value,1,true);
			}
			else {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			}
			query.AddColumn("Provider",160,FieldValueType.String,font);
			query.AddColumn("Guarantor",160,FieldValueType.String,font);
			query.AddColumn("Ins",40,FieldValueType.String,font);
			query.GetColumnHeader("Ins").ContentAlignment=ContentAlignment.MiddleCenter;
			query.GetColumnDetail("Ins").ContentAlignment=ContentAlignment.MiddleCenter;
			query.AddColumn("Princ",100,FieldValueType.Number,font);
			query.GetColumnHeader("Princ").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Princ").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Accum Int",100,FieldValueType.Number,font);
			query.GetColumnHeader("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Paid",100,FieldValueType.Number,font);
			query.GetColumnHeader("Paid").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Paid").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Balance",100,FieldValueType.Number,font);
			query.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Due Now",100,FieldValueType.Number,font);
			query.GetColumnHeader("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
			if(isPayPlanV2) {
				query.AddColumn("Bal Not Due",100,FieldValueType.Number,font);
				query.GetColumnHeader("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
			}
			if(checkShowFamilyBalance.Checked) {
				query.AddColumn("Fam Balance",100,FieldValueType.String,font);
				query.GetColumnHeader("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Fam Balance").SuppressIfDuplicate=true;
			}
			if(PrefC.HasClinicsEnabled) {
				QueryObject queryTotals=report.AddQuery(tableTotal,"Totals");
				queryTotals.AddColumn("Clinic",360,FieldValueType.String,font);
				queryTotals.AddColumn("Princ",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Princ").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Princ").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Accum Int",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Paid",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Paid").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Paid").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Balance",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Due Now",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
				if(isPayPlanV2) {
					queryTotals.AddColumn("Bal Not Due",100,FieldValueType.Number,font);
					queryTotals.GetColumnHeader("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
					queryTotals.GetColumnDetail("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
				}
				if(checkShowFamilyBalance.Checked) {
					queryTotals.AddColumn("Fam Balance",100,FieldValueType.String,font);
					queryTotals.GetColumnHeader("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
					queryTotals.GetColumnDetail("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
					queryTotals.GetColumnDetail("Fam Balance").SuppressIfDuplicate=true;
				}
			}
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		













		

		

		
	}
}
