using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormRepeatChargesUpdate :FormODBase{
		// ReSharper disable once InconsistentNaming
		// ReSharper disable once InconsistentNaming

		///<summary></summary>
		public FormRepeatChargesUpdate()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRepeatChargesUpdate_Load(object sender, EventArgs e) {
			checkRunAging.Checked=PrefC.GetBool(PrefName.RepeatingChargesRunAging);//this will cause the label text to be updated
		}

		///<summary>Do not use this method in release code. This is only to be used for Unit Tests 53-56.</summary>
		public void RunRepeatingChargesForUnitTests(DateTime dateRun) {
			RepeatCharges.RunRepeatingCharges(dateRun);
		}

		private void checkRunAging_CheckedChanged(object sender,EventArgs e) {
			labelDescription.Text=Lan.g(this,"This will add completed procedures to each account that has a repeating charge set up.   The date of the "
				+"procedure will be based on the settings in the repeating charge.")+"\r\n\r\n"
				+Lan.g(this,"This tool must be run at least every month for the repeating charges to be added.  The procedure will be backdated up "
				+"to one month and 20 days.")+"\r\n\r\n"
				+Lan.g(this,"If the repeating charge 'Creates Claim' check box is selected, and the patient has insurance, and the procedure added "
				+"is not marked as 'Do not usually bill to Ins', then a claim will be created for the procedure. If the patient has a secondary "
				+"insurance plan, a secondary claim will be created with a hold status.");
			if(!checkRunAging.Checked) {
				labelDescription.Text=labelDescription.Text+"\r\n\r\n"+Lan.g(this,"You should run aging when you are done.");
			}
		}

		///<summary>Checks if Repeating Charges are already running on another workstation or by the OpenDental Service.  If less than 24 hours have 
		///passed since the tool was started, user will be blocked from running Repeating Charges.  Otherwise, SecurityAdmin users can restart the tool.
		///</summary>
		private bool CheckBeginDateTime() {
			Prefs.RefreshCache();//Just to be sure we don't miss someone who has just started running repeating charges.
			if(PrefC.GetString(PrefName.RepeatingChargesBeginDateTime)=="") {
				return true;
			}
			DateTime repeatingChargesBeginDateTime=PrefC.GetDateT(PrefName.RepeatingChargesBeginDateTime);
			if((MiscData.GetNowDateTime()-repeatingChargesBeginDateTime).TotalHours < 24) {
				MsgBox.Show(this,"Repeating charges already running on another workstation, you must wait for them to finish before continuing.");
				return false;
			}
			//It's been more than 24 hours since repeat charges started.
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				string message=Lans.g(this,"Repeating Charges last started on")+" "+repeatingChargesBeginDateTime.ToString()
					+Lans.g(this,".  Restart repeating charges?");
				if(MsgBox.Show(MsgBoxButtons.OKCancel,message)) {
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Restarted repeating charges. Previous Repeating Charges Begin DateTime was "
						+repeatingChargesBeginDateTime.ToString()+".");
					return true;
				}
				return false;//Security admin doesn't want to restart repeat charges.
			}
			//User isn't a security admin.
			MsgBox.Show(Lans.g(this,"Repeating Charges last started on")+" "+repeatingChargesBeginDateTime.ToString()
				+Lans.g(this,".  Contact a user with SecurityAdmin permission to restart repeating charges."));
			return false;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!CheckBeginDateTime()) {
				return;
			}
			//If the AvaTax API is not available at HQ show popup and return.
			if(AvaTax.IsEnabled() && !AvaTax.PingAvaTax()) {
				MsgBox.Show(this,"Unable to connect to AvaTax API.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			RepeatChargeResult result=RepeatCharges.RunRepeatingCharges(MiscData.GetNowDateTime(),checkRunAging.Checked);
			string metrics=result.ProceduresAddedCount+" "+Lan.g(this,"procedures added.")+"\r\n"+result.ClaimsAddedCount+" "
				+Lan.g(this,"claims added.");
			SecurityLogs.MakeLogEntry(Permissions.RepeatChargeTool,0,"Repeat Charge Tool ran.\r\n"+metrics);
			Cursor=Cursors.Default;
			MessageBox.Show(metrics);
			if(!string.IsNullOrEmpty(result.ErrorMsg)) {
				SecurityLogs.MakeLogEntry(Permissions.RepeatChargeTool,0,"Repeat Charge Tool Error: "+result.ErrorMsg);
				MessageBox.Show(result.ErrorMsg);
			}
			DialogResult=DialogResult.OK;
		}	

		private void butCancel_Click(object sender, EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















