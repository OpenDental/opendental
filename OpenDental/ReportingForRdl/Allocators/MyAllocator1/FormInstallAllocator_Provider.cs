using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.Reporting.Allocators.MyAllocator1 {
	public partial class FormInstallAllocator_Provider:FormODBase {
		//private static string _InitialMessage = "Welcome to the Provider Allocation Setup\r\nPlease read the following:";

		public FormInstallAllocator_Provider() {
			InitializeComponent();

		}




		private void FormInstallAllocator_Provider_Load(object sender,EventArgs e) {
			RefreshForm();
		}
		private void RefreshForm() {
			Cache.Refresh(InvalidType.Prefs);
			bool toolRan = PrefC.ContainsKey(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_ToolHasRun);
			bool isUsing = PrefC.ContainsKey(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_Use);
			this.lblAllocatorStatus.Text = "Current Tool Status: "
				+ (toolRan ? "Tool has been run and " + (isUsing ? "\nsettings indicate that allocation is occuring." : "\nsettings indicate that allocation is not occuring.")
						: "Tool has not been run yet.");

			this.butGuarnDetailReport.Enabled = toolRan & isUsing;
			this.butProviderIncomeReport.Enabled = toolRan & isUsing;
			this.butUneardedIncomeReport.Enabled = toolRan & isUsing;
		}
		#region Report Buttons on Right of Form
		private void butProviderIncomeReport_Click(object sender,EventArgs e) {
			1.ToString();
			//DateTime dtFrom = new DateTime(2007,1,1);
			DateTime dtFrom = new DateTime(2007,12,31);
			DateTime dtTo = new DateTime(2007,12,31);
			//DateTime dtNow = DateTime.Now;
			//DateTime dtTo	 = new DateTime(dtNow.Year,dtNow.Month,dtNow.Day);
			FormReport_ProviderIncomeReport fpir = new FormReport_ProviderIncomeReport(dtFrom,dtTo);
			fpir.ShowDialog();
		}

		private void butUneardedIncomeReport_Click(object sender,EventArgs e) {

		}
		private void butGuarantorDetailReport_Click(object sender,EventArgs e) {
			FormReport_GuarantorAllocationCheck fgac = new FormReport_GuarantorAllocationCheck();
			fgac.ShowDialog();
		}

		#endregion
		private void butRunAllocatorTool_Click(object sender,EventArgs e) {
			if(!rbutIHaveRead.Checked) {
				PU.MB = Lan.g(this,"You must indicate that you have read the text in the box!");
				return;
			}

			if(MessageBox.Show("Do you want to run the batch allocation?","Please Respond",MessageBoxButtons.YesNo) == DialogResult.Yes) {
				FormWarnToCloseComputers fwc = new FormWarnToCloseComputers();
				if(fwc.ShowDialog() == DialogResult.Yes) {
					Reporting.Allocators.MyAllocator1_ProviderPayment allocator1 = new OpenDental.Reporting.Allocators.MyAllocator1_ProviderPayment();
					SecurityLogs.MakeLogEntry(OpenDentBusiness.Permissions.Setup,0,"Started Batch Allocation For Provider Allocation Tool");
					allocator1.StartBatchAllocation();
					SecurityLogs.MakeLogEntry(OpenDentBusiness.Permissions.Setup,0,"Finished Batch Allocation For Provider Allocation Tool");

					List<string> commands = new List<string>();
					if(!PrefC.ContainsKey(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_ToolHasRun)) {
						commands.Add("INSERT INTO preference VALUES ('"
							+ MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_ToolHasRun + "','0')");
					}
					if(!PrefC.ContainsKey(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_Use)) {
						commands.Add("INSERT INTO preference VALUES ('"
							+ MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_Use + "','0')");
					}
					if(commands.Count != 0) {
						Db.NonQOld(commands.ToArray());
						Cache.Refresh(InvalidType.Prefs);
					}
					Prefs.UpdateRaw(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_ToolHasRun,"1");
					Prefs.UpdateRaw(MyAllocator1_ProviderPayment.Pref_AllocatorProvider1_Use,"1");
				}
			}
			RefreshForm();

		}

		private void rbutIHaveRead_CheckedChanged(object sender,EventArgs e) {
			this.butRunAllocatorTool.Enabled = this.rbutIHaveRead.Checked;
			if(this.butRunAllocatorTool.Enabled)
				this.butRunAllocatorTool.BackColor = Color.White;
			else
				this.butRunAllocatorTool.BackColor = this.BackColor;
		}


	}
}