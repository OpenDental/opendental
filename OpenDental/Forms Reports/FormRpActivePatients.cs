using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using System.Linq;

namespace OpenDental {
	public partial class FormRpActivePatients:FormODBase {
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private List<Def> _listBillingTypeDefs;
		private List<PatientStatus> _listPatientStatuses = new List<PatientStatus>();

		public FormRpActivePatients() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpActivePatients_Load(object sender,EventArgs e) {
			dateStart.SelectionStart=DateTime.Today;
			dateEnd.SelectionStart=DateTime.Today;
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillingTypes.Items.AddList(_listBillingTypeDefs,x => x.ItemName);
			_listProviders=Providers.GetListReports();
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			foreach(PatientStatus patientStatus in Enum.GetValues(typeof(PatientStatus))) {
				if(patientStatus==PatientStatus.Deleted) {
					continue;
				}
				_listPatientStatuses.Add(patientStatus);
				listPatientStatuses.Items.Add(Lan.g("enumPatientStatus",patientStatus.ToString()));
			}
			checkAllPatStatus.Checked=true;
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
						checkAllClin.Checked=true;
					}
					if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
						listClin.SelectedIndices.Clear();
						listClin.SetSelected(listClin.Items.Count-1);
					}
				}
			}
		}

		private void checkAllProv_CheckedChanged(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listProv.ClearSelected();
			}
		}

		private void listProv_Click(object sender,EventArgs e) {
			if(listProv.SelectedIndices.Count>0) {
				checkAllProv.Checked=false;
			}
		}

		private void checkAllPatStatus_CheckedChanged(object sender,EventArgs e) {
			if(checkAllPatStatus.Checked) {
				listPatientStatuses.ClearSelected();
			}
		}

		private void listPatientStatuses_Click(object sender,EventArgs e) {
			if(listPatientStatuses.SelectedIndices.Count>0) {
				checkAllPatStatus.Checked=false;
			}
		}

		private void checkAllClin_CheckedChanged(object sender,EventArgs e) {
			if(checkAllClin.Checked) {
				listClin.ClearSelected();
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void checkAllBilling_CheckedChanged(object sender,EventArgs e) {
			if(checkAllBilling.Checked) {
				listBillingTypes.ClearSelected();
			}
		}

		private void listBillingTypes_Click(object sender,EventArgs e) {
			if(listBillingTypes.SelectedIndices.Count>0) {
				checkAllBilling.Checked=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!checkAllBilling.Checked && listBillingTypes.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one Billing Type must be selected.");
				return;
			}
			if(!checkAllPatStatus.Checked && listPatientStatuses.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one Patient Status must be selected.");
				return;
			}
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one Provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one Clinic must be selected.");
					return;
				}
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			List<long> listBillingTypeDefNums=new List<long>();
			List<long> listPatientStatusEnums=new List<long>();
			if(checkAllProv.Checked) {
				for(int i=0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
				}
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClin.Checked) {
					if(!Security.CurUser.ClinicIsRestricted) {
						listClinicNums.Add(0);
					}
					for(int i=0;i<_listClinics.Count;i++) {
						listClinicNums.Add(_listClinics[i].ClinicNum);
					}
					// Add hidden unrestricted clinics to the list
					listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
				}
				else {
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
				}
			}
			if(checkAllBilling.Checked) {
				for(int i=0;i<_listBillingTypeDefs.Count;i++) {
					listBillingTypeDefNums.Add(_listBillingTypeDefs[i].DefNum);
				}
			}
			else {
				for(int i=0;i<listBillingTypes.SelectedIndices.Count;i++) {
					listBillingTypeDefNums.Add(_listBillingTypeDefs[listBillingTypes.SelectedIndices[i]].DefNum);
				}
			}
			if(checkAllPatStatus.Checked) {
				for(int i = 0;i<_listPatientStatuses.Count;++i) {
					listPatientStatusEnums.Add((long)_listPatientStatuses[i]);
				}
			}
			else {
				for(int i = 0;i<listPatientStatuses.SelectedIndices.Count;++i) {
					listPatientStatusEnums.Add((long)_listPatientStatuses[listPatientStatuses.SelectedIndices[i]]);
				}
			}
			DataTable tablePats=RpActivePatients.GetActivePatientTable(dateStart.SelectionStart,dateEnd.SelectionStart,listProvNums,listClinicNums
				,listBillingTypeDefNums,listPatientStatusEnums,checkAllProv.Checked,checkAllClin.Checked,checkAllBilling.Checked);
			string subtitleProvs="";
			string subtitleClinics="";
			string subtitleBilling="";
			string subtitlePatStatus="";
			if(checkAllProv.Checked) {
				subtitleProvs=Lan.g(this,"All Providers");
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					if(i>0) {
						subtitleProvs+=", ";
					}
					subtitleProvs+=_listProviders[listProv.SelectedIndices[i]].Abbr;
				}
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClin.Checked) {
					subtitleClinics=Lan.g(this,"All Clinics");
				}
				else {
					for(int i=0;i<listClin.SelectedIndices.Count;i++) {
						if(i>0) {
							subtitleClinics+=", ";
						}
						if(Security.CurUser.ClinicIsRestricted) {
							subtitleClinics+=_listClinics[listClin.SelectedIndices[i]].Abbr;
						}
						else {
							if(listClin.SelectedIndices[i]==0) {
								subtitleClinics+=Lan.g(this,"Unassigned");
							}
							else {
								subtitleClinics+=_listClinics[listClin.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index
							}
						}
					}
				}
			}
			if(checkAllBilling.Checked) {
				subtitleBilling=Lan.g(this,"All Billing Types");
			}
			else {
				for(int i=0;i<listBillingTypes.SelectedIndices.Count;i++) {
					if(i>0) {
						subtitleBilling+=", ";
					}
					subtitleBilling+=Defs.GetValue(DefCat.BillingTypes,_listBillingTypeDefs[listBillingTypes.SelectedIndices[i]].DefNum);
				}
			}
			if(checkAllPatStatus.Checked) {
				subtitlePatStatus=Lan.g(this,"All Patient Statuses");
			}
			else {
				for(int i = 0;i<listPatientStatuses.SelectedIndices.Count;++i) {
					if(i>0) {
						subtitlePatStatus+=", ";
					}
					subtitlePatStatus+=_listPatientStatuses[listPatientStatuses.SelectedIndices[i]].ToString();
				}
			}
			report.ReportName=Lan.g(this,"Active Patients");
			report.AddTitle("Title",Lan.g(this,"Active Patients"));
			report.AddSubTitle("Date",dateStart.SelectionStart.ToShortDateString()+" - "+dateEnd.SelectionStart.ToShortDateString());
			report.AddSubTitle("Providers",subtitleProvs);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics);
			}
			report.AddSubTitle("Billing",subtitleBilling);
			report.AddSubTitle("Patient Status",subtitlePatStatus);
			QueryObject query;
			if(PrefC.HasClinicsEnabled) {
				query=report.AddQuery(tablePats,"","clinic",SplitByKind.Value,0);
			}
			else {
				query=report.AddQuery(tablePats,"","",SplitByKind.None,0);
			}
			query.AddColumn("Name",150,FieldValueType.String);
			query.AddColumn("Provider",80,FieldValueType.String);
			query.AddColumn("Address",150,FieldValueType.String);
			query.AddColumn("Address2",90,FieldValueType.String);
			query.AddColumn("City",100,FieldValueType.String);
			query.AddColumn("State",40,FieldValueType.String);
			query.AddColumn("Zip",70,FieldValueType.String);
			query.AddColumn("Carrier",120,FieldValueType.String);
			query.AddGroupSummaryField("Patient Count:","Carrier","Name",SummaryOperation.Count);
			report.AddPageNum();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			//DialogResult=DialogResult.OK;		
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}