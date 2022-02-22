using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.ReportingComplex;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpProcCodes : FormODBase {

		///<summary></summary>
		public FormRpProcCodes(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpProcCodes_Load(object sender, System.EventArgs e) {
			listBoxFeeSched.Items.AddList(FeeScheds.GetDeepCopy(true),x => x.Description);
			listBoxFeeSched.SelectedIndex=0;	
			listBoxClinics.Items.Add(Lan.g(this,"Default"));
			if(PrefC.HasClinicsEnabled) {
				listBoxClinics.Items.AddList(Clinics.GetDeepCopy(true),x =>x.Abbr);
			}
			listBoxClinics.SelectedIndex=0;
			listBoxProviders.Items.Add(Lan.g(this,"Default"));
			listBoxProviders.Items.AddList(Providers.GetListReports(),x => x.Abbr);
			listBoxProviders.SelectedIndex=0;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(listBoxFeeSched.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a fee schedule.");
				return;
			}
			if(PrefC.HasClinicsEnabled && listBoxClinics.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a clinic.");
				return;
			}
			if(listBoxProviders.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a provider.");
				return;
			}
			ReportComplex report=new ReportComplex(true,true);
			FeeSched feeSched=listBoxFeeSched.GetSelected<FeeSched>();
			long clinicNum=0;
			if(listBoxClinics.SelectedIndex>0){
				clinicNum=listBoxClinics.GetSelected<Clinic>().ClinicNum;
			}
			long provNum=0;
			if(listBoxProviders.SelectedIndex>0){
				provNum=listBoxProviders.GetSelected<Provider>().ProvNum;
			}
			DataTable dataTable=RpProcCodes.GetData(feeSched.FeeSchedNum,clinicNum,provNum,radioCategories.Checked,checkShowBlankFees.Checked);
			report.ReportName="Procedure Codes - Fee Schedules";
			report.AddTitle("Title",Lan.g(this,"Procedure Codes - Fee Schedules"));
			report.AddSubTitle("Fee Schedule",feeSched.Description);
			report.AddSubTitle("Clinic",listBoxClinics.Items.GetTextShowingAt(listBoxClinics.SelectedIndex));
			report.AddSubTitle("Provider",listBoxProviders.Items.GetTextShowingAt(listBoxProviders.SelectedIndex));
			report.AddSubTitle("Date",DateTime.Now.ToShortDateString());
			QueryObject queryObject=new QueryObject();
			queryObject=report.AddQuery(dataTable,"","",SplitByKind.None,1,true);
			if(radioCategories.Checked) {
				queryObject.AddColumn("Category",100,FieldValueType.String);
				queryObject.GetColumnDetail("Category").SuppressIfDuplicate=true;
			}
			queryObject.AddColumn("Code",100,FieldValueType.String);
			queryObject.AddColumn("Desc",600,FieldValueType.String);
			queryObject.AddColumn("Abbr",100,FieldValueType.String);
			queryObject.AddColumn("Fee",100,FieldValueType.String);
			queryObject.GetColumnDetail("Fee").ContentAlignment=ContentAlignment.MiddleRight;
			queryObject.GetColumnDetail("Fee").StringFormat="C"; //This isn't working...
			report.AddPageNum();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}
	}
}
