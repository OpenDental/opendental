using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormChartViewDateFilter:FormODBase {
		///<summary>Set this date before opening the form.  Also after OK, this date is available to the calling class.</summary>
		public DateTime DateStart;
		///<summary>Set this date before opening the form.  Also after OK, this date is available to the calling class.</summary>
		public DateTime DateEnd;

		public FormChartViewDateFilter() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormChartViewDateFilter_Load(object sender,EventArgs e) {
			for(int i=0;i<Enum.GetNames(typeof(ChartViewDates)).Length;i++) {
				listPresetDateRanges.Items.Add(Enum.GetNames(typeof(ChartViewDates))[i]);
			}
			FillDateTextBoxesHelper();
		}

		private void listPresetDateRanges_MouseClick(object sender,MouseEventArgs e) {
			int selectedI=listPresetDateRanges.IndexFromPoint(e.Location);
			switch((ChartViewDates)selectedI) {
				case ChartViewDates.All:
					DateStart=DateTime.MinValue;
					DateEnd=DateTime.MinValue;//interpreted as empty.  We want to show all future dates.
					break;
				case ChartViewDates.Today:
					DateStart=DateTime.Today;
					DateEnd=DateTime.Today;
					break;
				case ChartViewDates.Yesterday:
					DateStart=DateTime.Today.AddDays(-1);
					DateEnd=DateTime.Today.AddDays(-1);
					break;
				case ChartViewDates.ThisYear:
					DateStart=new DateTime(DateTime.Today.Year,1,1);
					DateEnd=new DateTime(DateTime.Today.Year,12,31);
					break;
				case ChartViewDates.LastYear:
					DateStart=new DateTime(DateTime.Today.Year-1,1,1);
					DateEnd=new DateTime(DateTime.Today.Year-1,12,31);
					break;
			}
			FillDateTextBoxesHelper();
		}

		private void FillDateTextBoxesHelper() {
			textDateStart.Text=DateStart.ToShortDateString();
			textDateEnd.Text=DateEnd.ToShortDateString();
			if(DateStart.Year < 1880) {
				textDateStart.Text=""; 
			}
			if(DateEnd.Year < 1880) { 
				textDateEnd.Text=""; 
			}
		}

		private void butNowStart_Click(object sender,EventArgs e) {
			DateStart=DateTime.Today;
			textDateStart.Text=DateStart.ToShortDateString();
		}

		private void butNowEnd_Click(object sender,EventArgs e) {
			DateEnd=DateTime.Today;
			textDateEnd.Text=DateEnd.ToShortDateString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {//validate the date boxes.
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateStart=PIn.Date(textDateStart.Text);
			DateEnd=PIn.Date(textDateEnd.Text);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
		

	
	}
}