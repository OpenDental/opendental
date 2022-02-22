using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormRpPresentedTreatmentProduction:FormODBase {
		private List<Userod> _listUsers;
		private List<Clinic> _listClinics;
		public FormRpPresentedTreatmentProduction() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpTreatPlanPresenter_Load(object sender,EventArgs e) {
			date1.SelectionStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(-1);
			date2.SelectionStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddDays(-1);
			_listUsers=Userods.GetDeepCopy(true);
			listUser.Items.AddList(_listUsers,x => x.UserName);
			checkAllUsers.Checked=true;
			if(PrefC.HasClinicsEnabled) {
				if(!Security.CurUser.ClinicIsRestricted) {
					listClin.Items.Add(Lan.g(this,"Unassigned"));
				}
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				listClin.Items.AddList(_listClinics,x => x.Abbr);
				checkAllClinics.Checked=true;
			}
			else {
				listClin.Visible=false;
				checkAllClinics.Visible=false;
				labelClin.Visible=false;
				groupType.Location=new Point(185,225);
				groupOrder.Location=new Point(185,295);
				groupUser.Location=new Point(185,365);
				listUser.Width+=30;
			}
		}

		private void RunTotals(List<long> listUserNums,List<long> listClinicsNums) {
			ReportComplex report=new ReportComplex(true,false);
			report.AddTitle("Title",Lan.g(this,"Presented Treatment Production"));
			report.AddSubTitle("SubTitle","Totals Report");
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("Date",date1.SelectionStart.ToShortDateString()+" - "+date2.SelectionStart.ToShortDateString());
			if(checkAllUsers.Checked) {
				report.AddSubTitle("Users",Lan.g(this,"All Users"));
			}
			else {
				string strUsers="";
				for(int i=0;i<listUser.SelectedIndices.Count;i++) {
					if(i==0) {
						strUsers=_listUsers[listUser.SelectedIndices[i]].UserName;
					}
					else {
						strUsers+=", "+_listUsers[listUser.SelectedIndices[i]].UserName;
					}
				}
				report.AddSubTitle("Users",strUsers);
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClinics.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
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
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			DataTable table=RpPresentedTreatmentProduction.GetPresentedTreatmentProductionTable(date1.SelectionStart,date2.SelectionStart,listClinicsNums,checkAllClinics.Checked
				,PrefC.HasClinicsEnabled,radioPresenter.Checked,radioFirstPresented.Checked,listUserNums,false);
			QueryObject query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			query.AddColumn(Lan.g(this,"Presenter"),100,FieldValueType.String);
			query.AddColumn(Lan.g(this,"# of Procs"),70,FieldValueType.Integer);
			query.AddColumn(Lan.g(this,"GrossProd"),100,FieldValueType.Number);
			query.AddColumn(Lan.g(this,"WriteOffs"),100,FieldValueType.Number);
			query.AddColumn(Lan.g(this,"Adjustments"),100,FieldValueType.Number);
			query.AddColumn(Lan.g(this,"NetProduction"),100,FieldValueType.Number);
			if(!report.SubmitQueries()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void RunDetailed(List<long> listUserNums,List<long> listClinicsNums) {
			ReportComplex report=new ReportComplex(true,false);
			report.AddTitle("Title",Lan.g(this,"Presented Treatment Production"));
			report.AddSubTitle("SubTitle", "Detailed Report");
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("Date",date1.SelectionStart.ToShortDateString()+" - "+date2.SelectionStart.ToShortDateString());
			if(checkAllUsers.Checked) {
				report.AddSubTitle("Users",Lan.g(this,"All Users"));
			}
			else {
				string strUsers="";
				for(int i=0;i<listUser.SelectedIndices.Count;i++) {
					if(i==0) {
						strUsers=_listUsers[listUser.SelectedIndices[i]].UserName;
					}
					else {
						strUsers+=", "+_listUsers[listUser.SelectedIndices[i]].UserName;
					}
				}
				report.AddSubTitle("Users",strUsers);
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClinics.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
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
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			DataTable tableReport=RpPresentedTreatmentProduction.GetPresentedTreatmentProductionTable(date1.SelectionStart,date2.SelectionStart,listClinicsNums,checkAllClinics.Checked
				,PrefC.HasClinicsEnabled,radioPresenter.Checked,radioFirstPresented.Checked,listUserNums,true);
			QueryObject query=report.AddQuery(tableReport,"","",SplitByKind.None,1,true);
			query.AddColumn("\r\n"+Lan.g(this,"Presenter"),90,FieldValueType.String);
			query.AddColumn(Lan.g(this,"Date")+"\r\n"+Lan.g(this,"Presented"),75,FieldValueType.Date);
			query.AddColumn(Lan.g(this,"Date")+"\r\n"+Lan.g(this,"Completed"),75,FieldValueType.Date);
			query.AddColumn("\r\n"+Lan.g(this,"Descript"),200,FieldValueType.String);
			query.AddColumn("\r\n"+Lan.g(this,"GrossProd"),90,FieldValueType.Number);
			query.AddColumn("\r\n"+Lan.g(this,"WriteOffs"),90,FieldValueType.Number);
			query.AddColumn("\r\n"+Lan.g(this,"Adjustments"),90,FieldValueType.Number);
			query.AddColumn("\r\n"+Lan.g(this,"NetProduction"),90,FieldValueType.Number);
			if(!report.SubmitQueries()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void checkAllUsers_Click(object sender,EventArgs e) {
			if(checkAllUsers.Checked) {
				listUser.ClearSelected();
			}
		}

		private void listUser_Click(object sender,EventArgs e) {
			if(listUser.SelectedIndices.Count>0) {
				checkAllUsers.Checked=false;
			}
		}

		private void checkAllClinics_Click(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				listClin.ClearSelected();
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClinics.Checked=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(!checkAllUsers.Checked && listUser.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one user.");
				return;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClinics.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one clinic.");
				return;
			}
			List<long> listUserNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			if(checkAllUsers.Checked) {
				listUserNums=_listUsers.Select(x => x.UserNum).ToList();
			}
			else {
				listUserNums=listUser.SelectedIndices.Select(x => _listUsers[x].UserNum).ToList();
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClinics.Checked) {
					listClinicNums=_listClinics.Select(x => x.ClinicNum).ToList();
				}
				else {
					for(int i = 0;i<listClin.SelectedIndices.Count;i++) {
						if(Security.CurUser.ClinicIsRestricted) {
							listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]].ClinicNum);
						}
						else if(listClin.SelectedIndices[i]!=0) {
							listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]-1].ClinicNum);
						}
					}
				}
				if(!Security.CurUser.ClinicIsRestricted && (listClin.SelectedIndices.Contains(0) || checkAllClinics.Checked)) {
					listClinicNums.Add(0);
				}
				if(checkAllClinics.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
					listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
				}
			}
			if(radioDetailed.Checked) {
				RunDetailed(listUserNums,listClinicNums);
			}
			else {
				RunTotals(listUserNums,listClinicNums);
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}

}