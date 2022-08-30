using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpPHRawPop : FormODBase {
		private FormQuery FormQuery2;
		private List<Def> _listAdjTypeDefs;

		///<summary></summary>
		public FormRpPHRawPop(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpPHRawPop_Load(object sender, System.EventArgs e) {
			DateTime today=DateTime.Today;
			//will start out 1st through 30th of previous month
			date1.SelectionStart=new DateTime(today.Year,today.Month,1).AddMonths(-1);
			date2.SelectionStart=new DateTime(today.Year,today.Month,1).AddDays(-1);
			_listAdjTypeDefs=Defs.GetDefsForCategory(DefCat.AdjTypes,true);
			listAdjType.Items.AddList(_listAdjTypeDefs,x => x.ItemName);
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(listAdjType.SelectedIndices.Count==0){
				MessageBox.Show("At least one adjustment type must be selected.");
				return;
			}
			ReportSimpleGrid report=new ReportSimpleGrid();
			string types="";
			for(int i=0;i<listAdjType.SelectedIndices.Count;i++){
				if(i==0){
					types+="(";
				}
				else{
					types+="OR ";
				}
				types+="AdjType='"
					+_listAdjTypeDefs[listAdjType.SelectedIndices[i]].DefNum.ToString()
					+"' ";
			}
			types+=")";
			report.Query=@"SELECT patient.PatNum,MIN(procedurelog.ProcDate) AS ProcDate,
				CONCAT(CONCAT(provider.LName,', '),provider.FName) as ProvName,
				patient.County,county.CountyCode,
				site.Description AS gradeschool,site.Note AS schoolCode,patient.GradeLevel,patient.Birthdate,"
				+DbHelper.GroupConcat("patientrace.Race",true)//distinct races from the patient race table in a comma delimited list of ints
				+@" Race,patient.Gender,patient.Urgency,patient.BillingType,
				patient.PlannedIsDone,broken.NumberBroken
				FROM patient
				LEFT JOIN patientrace ON patient.PatNum=patientrace.PatNum
				LEFT JOIN procedurelog ON procedurelog.PatNum=patient.PatNum
				LEFT JOIN provider ON procedurelog.ProvNum=provider.ProvNum
				LEFT JOIN site ON patient.SiteNum=site.SiteNum
				LEFT JOIN county ON patient.County=county.CountyName
				LEFT JOIN (
						SELECT PatNum,COUNT(*) NumberBroken
						FROM adjustment WHERE "+types
						+"AND adjustment.AdjDate >= "+POut.Date(date1.SelectionStart)+" "
						+"AND adjustment.AdjDate <= " +POut.Date(date2.SelectionStart)+" "
						+@"GROUP BY adjustment.PatNum
				) broken ON broken.PatNum=patient.PatNum
				WHERE	(procedurelog.ProcStatus='2'
				AND procedurelog.ProcDate >= "+POut.Date(date1.SelectionStart)+" "
				+"AND procedurelog.ProcDate <= " +POut.Date(date2.SelectionStart)+" )"
				+"OR broken.NumberBroken>0 "
				+@"GROUP BY patient.PatNum
				ORDER By procedurelog.ProcDate;";
			FormQuery2=new FormQuery(report);
			FormQuery2.textTitle.Text="RawPopulationData"+DateTime.Today.ToString("MMddyyyy");
			//FormQuery2.IsReport=true;
			//FormQuery2.SubmitReportQuery();			
			FormQuery2.SubmitQuery();
			FormQuery2.ShowDialog();
			FormQuery2.Dispose();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}
