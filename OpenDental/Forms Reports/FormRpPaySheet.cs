using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpPaySheet : FormODBase{
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private List<Def> _listInsDefs;
		private List<Def> _listPayDefs;
		private List<Def> _listClaimPayGroupDefs;

		///<summary></summary>
		public FormRpPaySheet(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormPaymentSheet_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			_listProviders.Insert(0,Providers.GetUnearnedProv());
			date1.SelectionStart=DateTime.Today;
			date2.SelectionStart=DateTime.Today;
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				Provider prov=_listProviders.FirstOrDefault();
				if(prov!=null) {
					_listProviders.AddRange(Providers.GetWhere(x => x.FName==prov.FName && x.LName==prov.LName && x.ProvNum!=prov.ProvNum));
				}
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			//If the user is not allowed to run the report for all providers, default the selection to the first in the list box.
			if(checkAllProv.Enabled==false && listProv.Items.Count > 0) {
				listProv.SetSelected(0);
			}
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true) && listProv.Items.Count>0) {
				listProv.SetAll(true);
			}
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
			checkReportDisplayUnearnedTP.Checked=PrefC.GetBool(PrefName.ReportsDoShowHiddenTPPrepayments);
			_listPayDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			_listPayDefs.Add(new Def {
				ItemName="Income Transfer",
				DefNum=0,//Income Transfer's PayType is 0.
			});
			_listInsDefs=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
			_listClaimPayGroupDefs=Defs.GetDefsForCategory(DefCat.ClaimPaymentGroups,true);
			listPatientTypes.Items.AddList(_listPayDefs,x => x.ItemName);
			listPatientTypes.Visible=false;
			listInsuranceTypes.Items.AddList(_listInsDefs,x => x.ItemName);
			listInsuranceTypes.Visible=false;
			listClaimPayGroups.Items.AddList(_listClaimPayGroupDefs,x => x.ItemName);
			listClaimPayGroups.Visible=false;
			Plugins.HookAddCode(this,"FormPaymentSheet_Load_end");
		}

		private void checkAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
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

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void checkAllClaimPayGroups_Click(object sender,EventArgs e) {
			if(checkAllClaimPayGroups.Checked) {
				listClaimPayGroups.Visible=false;
			}
			else {
				listClaimPayGroups.Visible=true;
			}
		}

		private void checkAllTypes_Click(object sender,EventArgs e) {
			if(checkPatientTypes.Checked){
				listPatientTypes.Visible=false;
			}
			else{
				listPatientTypes.Visible=true;
			}
		}

		private void checkIns_Click(object sender,EventArgs e) {
			if(checkInsuranceTypes.Checked) {
				listInsuranceTypes.Visible=false;
			}
			else {
				listInsuranceTypes.Visible=true;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return;
			}
			if(!checkPatientTypes.Checked && listPatientTypes.SelectedIndices.Count==0
				&& !checkInsuranceTypes.Checked && listInsuranceTypes.SelectedIndices.Count==0)
			{
				MsgBox.Show(this,"At least one type must be selected.");
				return;
			}
			if(!checkAllClaimPayGroups.Checked && listClaimPayGroups.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one claim payment group must be selected.");
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			List<long> listInsTypes=new List<long>();
			List<long> listPatTypes=new List<long>();
			List<long> listSelectedClaimPayGroupNums=new List<long>();
			if(checkAllProv.Checked) {
				listProvNums=_listProviders.Select(x => x.ProvNum).ToList();
			}
			else {
				listProvNums=listProv.SelectedIndices.Select(x => _listProviders[x].ProvNum).ToList();
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
			if(checkInsuranceTypes.Checked) {
				listInsTypes=_listInsDefs.Select(x => x.DefNum).ToList();
			}
			else {
				listInsTypes=listInsuranceTypes.SelectedIndices.Select(x => _listInsDefs[x].DefNum).ToList();
			}
			if(checkPatientTypes.Checked) {
				listPatTypes=_listPayDefs.Select(x => x.DefNum).ToList();
			}
			else {
				listPatTypes=listPatientTypes.SelectedIndices.Select(x => _listPayDefs[x].DefNum).ToList();
			}
			if(checkAllClaimPayGroups.Checked) {
				listSelectedClaimPayGroupNums=_listClaimPayGroupDefs.Select(x => x.DefNum).ToList();
			}
			else {
				listSelectedClaimPayGroupNums=listClaimPayGroups.SelectedIndices.Select(x => _listClaimPayGroupDefs[x].DefNum).ToList();
			}
			DataTable tableIns=new DataTable();
			if(listProvNums.Count!=0) {
				tableIns=RpPaySheet.GetInsTable(date1.SelectionStart,date2.SelectionStart,listProvNums,listClinicNums,listInsTypes,
					listSelectedClaimPayGroupNums,checkAllProv.Checked,checkAllClin.Checked,checkInsuranceTypes.Checked,radioPatient.Checked,
					checkAllClaimPayGroups.Checked,checkShowProvSeparate.Checked);
			}
			DataTable tablePat=RpPaySheet.GetPatTable(date1.SelectionStart,date2.SelectionStart,listProvNums,listClinicNums,listPatTypes,
				checkAllProv.Checked,checkAllClin.Checked,checkPatientTypes.Checked,radioPatient.Checked,checkUnearned.Checked,checkShowProvSeparate.Checked,
				checkReportDisplayUnearnedTP.Checked);
			string subtitleProvs="";
			string subtitleClinics="";
			if(checkAllProv.Checked) {
				subtitleProvs=Lan.g(this,"All Providers");
			}
			else {
				subtitleProvs+=string.Join(", ",listProv.SelectedIndices.Select(x => _listProviders[x].Abbr));
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClin.Checked && !Security.CurUser.ClinicIsRestricted) {
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
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Daily Payments");
			report.AddTitle("Title",Lan.g(this,"Daily Payments"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			Dictionary<long,string> dictInsDefNames=new Dictionary<long,string>();
			Dictionary<long,string> dictPatDefNames=new Dictionary<long,string>();
			List<Def> insDefs=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
			List<Def> patDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			for(int i=0;i<insDefs.Count;i++) {
				dictInsDefNames.Add(insDefs[i].DefNum,insDefs[i].ItemName);
			}
			for(int i=0;i<patDefs.Count;i++) {
				dictPatDefNames.Add(patDefs[i].DefNum,patDefs[i].ItemName);
			}
			dictPatDefNames.Add(0,"Income Transfer");//Otherwise income transfers show up with a payment type of "Undefined"
			int[] summaryGroups1= { 1 };
			int[] summaryGroups2= { 2 };
			int[] summaryGroups3= { 1,2 };
			//Insurance Payments Query-------------------------------------
			QueryObject query=report.AddQuery(tableIns,"Insurance Payments","PayType",SplitByKind.Definition,1,true,dictInsDefNames,fontSubTitle);
			query.AddColumn("Date",90,FieldValueType.Date,font);
			//query.GetColumnDetail("Date").SuppressIfDuplicate = true;
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Carrier",150,FieldValueType.String,font);
			query.AddColumn("Patient Name",150,FieldValueType.String,font);
			query.AddColumn("Provider",90,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",120,FieldValueType.String,font);
			}
			query.AddColumn("Check#",75,FieldValueType.String,font);
			query.AddColumn("Amount",90,FieldValueType.Number,font);
			query.AddGroupSummaryField("Total Insurance Payments:","Amount","amt",SummaryOperation.Sum,new List<int>(summaryGroups1),Color.Black,fontBold,0,20);
			//Patient Payments Query---------------------------------------
			query=report.AddQuery(tablePat,"Patient Payments","PayType",SplitByKind.Definition,2,true,dictPatDefNames,fontSubTitle);
			query.AddColumn("Date",90,FieldValueType.Date,font);
			//query.GetColumnDetail("Date").SuppressIfDuplicate = true;
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Paying Patient",270,FieldValueType.String,font);
			query.AddColumn("Provider",90,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",120,FieldValueType.String,font);
			}
			query.AddColumn("Check#",75,FieldValueType.String,font);
			query.AddColumn("Amount",120,FieldValueType.Number,font);
			query.AddGroupSummaryField("Total Patient Payments:","Amount","amt",SummaryOperation.Sum,new List<int>(summaryGroups2),Color.Black,fontBold,0,20);
			query.AddGroupSummaryField("Total All Payments:","Amount","amt",SummaryOperation.Sum,new List<int>(summaryGroups3),Color.Black,fontBold,0,4);
			report.AddPageNum(font);
			report.AddGridLines();
			report.AddPageFooterText("PageFooter","*Part of a bulk check, which can be located by going to the listed patient's account",font,0,
				ContentAlignment.MiddleLeft);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}
	}
}
