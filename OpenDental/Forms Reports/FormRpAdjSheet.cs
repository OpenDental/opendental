using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.ReportingComplex;
using CodeBase;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpAdjSheet : FormODBase {
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private bool _hasClinicsEnabled;
		///<summary>Holds all adjustment types, does NOT include hidden.</summary>
		private List<Def> _listAdjTypeDefs;

		///<summary></summary>
		public FormRpAdjSheet(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDailyAdjustment_Load(object sender, System.EventArgs e) {
			date1.SelectionStart=DateTime.Today;
			date2.SelectionStart=DateTime.Today;
			_listProviders=Providers.GetListReports();
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(checkAllProv.Enabled==false && _listProviders.Count>0) {
				listProv.SetSelected(0,true);
			}
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
				_hasClinicsEnabled=false;
			}
			else {
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				_hasClinicsEnabled=true;
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
			_listAdjTypeDefs=Defs.GetDefsForCategory(DefCat.AdjTypes,true);//Exclude hidden.
			checkAllAdjs.Checked=true;
			listType.Items.AddList(_listAdjTypeDefs,x => x.ItemName);
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

		private void listType_Click(object sender,EventArgs e) {
			if(listType.SelectedIndices.Count>0) {
				checkAllAdjs.Checked=false;
			}
		}

		private void checkAllAdjs_Click(object sender,EventArgs e) {
			if(checkAllAdjs.Checked) {
				listType.ClearSelected();
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(_hasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			if(!checkAllAdjs.Checked && listType.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one type must be selected.");
				return;
			}
			List<long> listClinicNums=new List<long>();
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
			List<long> listProvNums=new List<long>();
			if(checkAllProv.Checked) {
				for(int i = 0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
				}
			}
			List<string> listAdjType=new List<string>();
			if(checkAllAdjs.Checked) {
				//add all adjustment types, including hidden
				listAdjType=Defs.GetDefsForCategory(DefCat.AdjTypes).Select(x => POut.Long(x.DefNum)).ToList();
			}
			else {
				for(int i=0;i<listType.SelectedIndices.Count;i++) {//1:1
					listAdjType.Add(POut.Long(_listAdjTypeDefs[listType.SelectedIndices[i]].DefNum));
				}
			}
			ReportComplex report=new ReportComplex(true,false);	 
			DataTable table=RpAdjSheet.GetAdjTable(date1.SelectionStart,date2.SelectionStart,listProvNums,
				listClinicNums,listAdjType,checkAllClin.Checked,_hasClinicsEnabled);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Daily Adjustments");
			report.AddTitle("Title",Lan.g(this,"Daily Adjustments"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",date1.SelectionStart.ToString("d")+" - "+date2.SelectionStart.ToString("d"),fontSubTitle);
			if(checkAllProv.Checked) {
				report.AddSubTitle("Provider SubTitle",Lan.g(this,"All Providers"));
			}
			else {
				string provNames="";
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					if(i>0) {
						provNames+=", ";
					}
					provNames+=_listProviders[listProv.SelectedIndices[i]].Abbr;
				}
				report.AddSubTitle("Provider SubTitle",provNames);
			}
			if(_hasClinicsEnabled) {
				if(checkAllClin.Checked) {
					report.AddSubTitle("Clinic SubTitle",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames="";
					for(int i=0;i<listClin.SelectedIndices.Count;i++) {
						if(i>0) {
							clinNames+=", ";
						}
						if(Security.CurUser.ClinicIsRestricted) {
							clinNames+=_listClinics[listClin.SelectedIndices[i]].Abbr;
						}
						else {
							if(listClin.SelectedIndices[i]==0) {
								clinNames+=Lan.g(this,"Unassigned");
							}
							else {
								clinNames+=_listClinics[listClin.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index
							}
						}
					}
					report.AddSubTitle("Clinic SubTitle",clinNames);
				}
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Date",90,FieldValueType.Date);
			query.AddColumn("Patient Name",130,FieldValueType.String);
			query.AddColumn("Prov",60,FieldValueType.String);
			if(_hasClinicsEnabled) {
				query.AddColumn("Clinic",70,FieldValueType.String);
			}
			query.AddColumn("AdjustmentType",150,FieldValueType.String);
			query.AddColumn("Note",180,FieldValueType.String);
			query.AddColumn("Amount",75,FieldValueType.Number);
			query.GetColumnDetail("Amount").ContentAlignment=ContentAlignment.MiddleRight;
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}
		

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
