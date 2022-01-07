using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpFinanceCharge : FormODBase {
		private List<Provider> _listProviders=new List<Provider>();
		private List<Def> _listBillingTypeDefs=new List<Def>();

		///<summary></summary>
		public FormRpFinanceCharge(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpFinanceCharge_Load(object sender, System.EventArgs e) {
			textDateFrom.Text=PrefC.GetDate(PrefName.FinanceChargeLastRun).ToShortDateString();
			textDateTo.Text=PrefC.GetDate(PrefName.FinanceChargeLastRun).ToShortDateString();
			_listProviders=Providers.GetListReports();
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillingType.Items.AddList(_listBillingTypeDefs,x => x.ItemName);
			if(listBillingType.Items.Count>0) {
				listBillingType.SelectedIndex=0;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc(),x => x.Abbr);
			if(listProv.Items.Count>0) {
				listProv.SelectedIndex=0; 
			}
			checkAllProv.Checked=true;
			checkAllBilling.Checked=true;
			listProv.Visible=false;
			listBillingType.Visible=false;
		}

		private void checkAllProv_Click(object sender,EventArgs e) {
			listProv.Visible=!checkAllProv.Checked;
		}

		private void checkAllBilling_Click(object sender,EventArgs e) {
			listBillingType.Visible=!checkAllBilling.Checked;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			if(dateTo<dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listProvNums = new List<long>();
			if(!checkAllProv.Checked) {
				listProvNums.AddRange(listProv.SelectedIndices.Select(x => _listProviders[x].ProvNum).ToList());
			}
			List<long> listBillingDefNums= new List<long>();
			if(!checkAllBilling.Checked) {
				listBillingDefNums.AddRange(listBillingType.SelectedIndices.Select(x => _listBillingTypeDefs[x].DefNum).ToList());
			}
			DataTable table=RpFinanceCharge.GetFinanceChargeTable(dateFrom,dateTo,PrefC.GetLong(PrefName.FinanceChargeAdjustmentType),listProvNums,listBillingDefNums);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Finance Charge Report");
			report.AddTitle("Title",Lan.g(this,"Finance Charge Report"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",dateFrom.ToString("d")+" - "+dateTo.ToString("d"),fontSubTitle);
			string subtitleProvs="";
			if(listProvNums.Count>0) {
				subtitleProvs+=listProv.GetStringSelectedItems(true);
			}
			else {
				subtitleProvs=Lan.g(this,"All Providers");
			}
			report.AddSubTitle("Provider Subtitle",subtitleProvs);
			string subtBillingTypes="";
			if(listBillingDefNums.Count>0) {
				subtBillingTypes+=listBillingType.GetStringSelectedItems();
			}
			else {
				subtBillingTypes=Lan.g(this,"All Billing Types");
			}
			report.AddSubTitle("Billing Subtitle",subtBillingTypes);
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("PatNum",75);
			query.AddColumn("Patient Name",180);
			query.AddColumn("Preferred Name",130);
			query.AddColumn("Amount",100,FieldValueType.Number);
			query.GetColumnDetail("Amount").ContentAlignment=ContentAlignment.MiddleRight;
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();		
			DialogResult=DialogResult.OK;
		}

	}
}
