using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormEmailAddresses:FormODBase {
		public bool IsSelectionMode;
		public long EmailAddressNum;
		///<summary>If true, a signal for invalid Email cache will be sent out upon closing.</summary>
		public bool IsChanged;
		///<summary>When in SelectionModeIf true, allows selecting an email address associated to a user.</summary>
		public bool DoAllowSelectingAddressWithUsers;

		public FormEmailAddresses() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailAddresses_Load(object sender,EventArgs e) {
			checkEmailDisclaimer.Checked=PrefC.GetBool(PrefName.EmailDisclaimerIsOn);
			if(IsSelectionMode) {
				labelInboxCheckInterval.Visible=false;
				textInboxCheckInterval.Visible=false;
				labelInboxCheckUnits.Visible=false;
				groupEmailPrefs.Visible=false;
				butAdd.Visible=false;
				checkEmailDisclaimer.Visible=false;
			}
			else {
				textInboxCheckInterval.Text=PrefC.GetInt(PrefName.EmailInboxCheckInterval).ToString();//Calls PIn() internally.
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EmailAddress selectedAddress=gridMain.SelectedTag<EmailAddress>();
			if(selectedAddress is null) {
				return;
			}
			if(IsSelectionMode) {
				if(!DoAllowSelectingAddressWithUsers && selectedAddress.UserNum!=0) {
					MsgBox.Show(this,"Please select an existing email address that is not associated with a user or clinic.");
					return;
				}
				EmailAddressNum=selectedAddress.EmailAddressNum;
				DialogResult=DialogResult.OK;
			}
			else {
				using FormEmailAddressEdit formEmailAddressEdit=new FormEmailAddressEdit(selectedAddress,isOpenedFromEmailSetup:true);
				formEmailAddressEdit.ShowDialog();
				if(formEmailAddressEdit.DialogResult==DialogResult.OK) {
					IsChanged=true;
					FillGrid();
				}
			}
		}

		private void FillGrid() {
			EmailAddresses.RefreshCache();
			List<EmailAddress> listEmailAddresses=EmailAddresses.GetDeepCopy()
				.OrderByDescending(x=>x.UserNum==Security.CurUser.UserNum) //grab current user
				.ThenByDescending(x=>x.UserNum!=0)//then any other user
			.ToList();
			//Removes any user specific email addresses to the list if the currenty user is not admin
			List<long> listUserodNums=new List<long>();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,suppressMessage:true) || IsSelectionMode) {
				listEmailAddresses.RemoveAll(x=>x.UserNum!=Security.CurUser.UserNum && x.UserNum!=0);
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"User Name"),240);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Sender Address"),270);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"User"),135);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Default"),50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Notify"),50,HorizontalAlignment.Center);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEmailAddresses.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEmailAddresses[i].EmailUsername);
				row.Cells.Add(listEmailAddresses[i].SenderAddress);
				row.Cells.Add(Userods.GetName(listEmailAddresses[i].UserNum));
				row.Cells.Add((listEmailAddresses[i].EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum))?"X":"");
				row.Cells.Add((listEmailAddresses[i].EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum))?"X":"");
				row.Tag=listEmailAddresses[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butSetDefault_Click(object sender,EventArgs e) {
			EmailAddress selectedAddress=gridMain.SelectedTag<EmailAddress>();
			if(selectedAddress is null) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(selectedAddress.UserNum>0) {
				MsgBox.Show(this,"User email address cannot be set as the default.");
				return;
			}
			if(Prefs.UpdateLong(PrefName.EmailDefaultAddressNum,selectedAddress.EmailAddressNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butWebMailNotify_Click(object sender,EventArgs e) {
			EmailAddress selectedAddress=gridMain.SelectedTag<EmailAddress>();
			if(selectedAddress is null) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(selectedAddress.UserNum>0) {
				MsgBox.Show(this,"User email address cannot be set as WebMail Notify.");
				return;
			}
			if(Prefs.UpdateLong(PrefName.EmailNotifyAddressNum,selectedAddress.EmailAddressNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEmailAddressEdit formEmailAddressEdit=new FormEmailAddressEdit(userNum:0,isOpenedFromEmailSetup:true);
			formEmailAddressEdit.ShowDialog();
			if(formEmailAddressEdit.DialogResult==DialogResult.OK) {
				FillGrid();
				IsChanged=true;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(IsSelectionMode) {
				if(gridMain.SelectedTag<EmailAddress>() is null || (!DoAllowSelectingAddressWithUsers && gridMain.SelectedTag<EmailAddress>().UserNum!=0)) {
					MsgBox.Show(this,"Please select an existing email address that is not associated with a user or clinic.");
					return;
				}
				EmailAddressNum=gridMain.SelectedTag<EmailAddress>().EmailAddressNum;
			}
			else {//The following fields are only visible when not in selection mode.
				int inboxCheckIntervalMinuteCount=0;
				try {
					inboxCheckIntervalMinuteCount=int.Parse(textInboxCheckInterval.Text);
				}
				catch {
					MsgBox.Show(this,"Inbox check interval must be between 1 and 60 inclusive.");
					return;
				}
				if(inboxCheckIntervalMinuteCount<1 || inboxCheckIntervalMinuteCount>60) {
					MsgBox.Show(this,"Inbox check interval must be between 1 and 60 inclusive.");
					return;
				}
				if(Prefs.UpdateInt(PrefName.EmailInboxCheckInterval,inboxCheckIntervalMinuteCount)
					| Prefs.UpdateBool(PrefName.EmailDisclaimerIsOn,checkEmailDisclaimer.Checked)) 
				{
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void FormEmailAddresses_FormClosing(object sender,FormClosingEventArgs e) {
			if(IsChanged) {
				DataValid.SetInvalid(InvalidType.Email);
			}
		}

		private void checkEmailDisclaimer_CheckedChanged(object sender,EventArgs e) {
			if(!checkEmailDisclaimer.Checked && CultureInfo.CurrentCulture.Name.EndsWith("US")) {
				MsgBox.Show(this,"An opt-out statement is legally required in the US. We do not recommend removing this from your outgoing emails.");
			}
		}

	}
}