using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpInsOverpaid:FormODBase {
		private List<Clinic> _listClinics;

		///<summary></summary>
		public FormRpInsOverpaid() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpInsOverpaid_Load(object sender, System.EventArgs e) {
			dateStart.SelectionStart=DateTime.Today.AddMonths(-1);
			dateStart.SelectionEnd=DateTime.Today.AddMonths(-1);
			dateEnd.SelectionStart=DateTime.Today;
			dateEnd.SelectionEnd=DateTime.Today;
			if(PrefC.HasClinicsEnabled) {
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
						listClin.ClearSelected();
						listClin.SetSelected(listClin.Items.Count-1);
					}
				}
			}
			else {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
				//Adjust the location of the window size to make up for the clinic list being invisible
				this.Height=412;
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

		private void butOK_Click(object sender, System.EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			ReportComplex report=new ReportComplex(true,false);
			Cursor=Cursors.WaitCursor;
			List<long> listClinicNums=new List<long>();
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
			DataTable tableOverpaid=RpInsOverpaid.GetInsuranceOverpaid(dateStart.SelectionStart,dateEnd.SelectionStart,listClinicNums,
				radioGroupByProc.Checked);
			Cursor=Cursors.Default;
			string subtitleClinics="";
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
			report.ReportName=Lan.g(this,"Insurance Overpaid");
			report.AddTitle("Title",Lan.g(this,"Insurance Overpaid"));
			report.AddSubTitle("Practice Name",PrefC.GetString(PrefName.PracticeTitle));
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics);
			}
			QueryObject query=report.AddQuery(tableOverpaid,DateTime.Today.ToShortDateString());
			query.AddColumn("Pat Name",200,FieldValueType.String);
			query.AddColumn("Date",90,FieldValueType.Date);
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Fee",100,FieldValueType.Number);
			query.AddColumn("InsPaid+W/O",120,FieldValueType.Number);
			report.AddPageNum();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

	}
}
