using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	public partial class FormReport_ProviderIncomeReport :FormODBase
	{
		private DateTime _FromDate;
		private DateTime _ToDate;
		public FormReport_ProviderIncomeReport(DateTime fromdate, DateTime todate)
		{
			InitializeComponent();
			_FromDate = fromdate;
			_ToDate = todate;
		}

		private void FormReport_ProviderIncomeReport_Load(object sender, EventArgs e)
		{
			this.Text = "Provider Income Report From " + _FromDate.ToShortDateString()
				+ " To " + _ToDate.ToShortDateString();
			this.lblDgvHeader.Text = this.Text;

			this.dataGridView1.DataSource = SupportingCode.Report1_GuarantorAllocation.GenerateSummaryTable(-1,_FromDate,_ToDate);
			ReportUI.ReportStyles.Set_DefaultDataGridViewStyle(this.dataGridView1);

		}
		

	}
}