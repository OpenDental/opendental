using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpPHRawScreen : FormODBase {
		private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpPHRawScreen(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpPHRawScreen_Load(object sender, System.EventArgs e) {
			DateTime today=DateTime.Today;
			//will start out 1st through 30th of previous month
			date1.SelectionStart=new DateTime(today.Year,today.Month,1).AddMonths(-1);
			date2.SelectionStart=new DateTime(today.Year,today.Month,1).AddDays(-1);
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=@"SELECT SGDate,ProvName,County,county.CountyCode,
				site.Description AS schoolName,site.Note AS schoolCode,site.PlaceService,screen.GradeLevel,Age,Birthdate,RaceOld,Gender,Urgency,";
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				report.Query+="HasCaries,EarlyChildCaries,CariesExperience,ExistingSealants,NeedsSealants,MissingAllTeeth,";
			}
			report.Query+=@"Comments 
				FROM screen
				LEFT JOIN screengroup ON screengroup.ScreenGroupNum=screen.ScreenGroupNum
				LEFT JOIN site ON screengroup.GradeSchool=site.Description
				LEFT JOIN county ON screengroup.County=county.CountyName
				WHERE SGDate >= "+POut.Date(date1.SelectionStart)+" "
				+"AND SGDate <= " +POut.Date(date2.SelectionStart);
			FormQuery2=new FormQuery(report);
			FormQuery2.textTitle.Text="RawScreeningData"+DateTime.Today.ToString("MMddyyyy");
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
