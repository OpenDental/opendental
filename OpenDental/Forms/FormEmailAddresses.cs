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

namespace OpenDental {
	public partial class FormEmailAddresses:FormODBase {
		public bool IsSelectionMode;
		public long EmailAddressNum;
		///<summary>If true, a signal for invalid Email cache will be sent out upon closing.</summary>
		public bool IsChanged;
		private List<EmailAddress> _listEmailAddresses;

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
			if(IsSelectionMode) {
				EmailAddressNum=_listEmailAddresses[gridMain.GetSelectedIndex()].EmailAddressNum;
				DialogResult=DialogResult.OK;
			}
			else {
				using FormEmailAddressEdit FormEAE=new FormEmailAddressEdit(_listEmailAddresses[e.Row],true);
				FormEAE.ShowDialog();
				if(FormEAE.DialogResult==DialogResult.OK) {
					IsChanged=true;
					FillGrid();
				}
			}
		}

		private void FillGrid() {
			EmailAddresses.RefreshCache();
			_listEmailAddresses=EmailAddresses.GetDeepCopy();
			//Add user specific email addresses to the list
			List<Userod> listUsers=new List<Userod>();
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true) && !IsSelectionMode) {
				listUsers.AddRange(Userods.GetUsers());//If authorized, get all non-hidden users.
			}
			else {
				listUsers.Add(Security.CurUser);//Otherwise, just this user.
			}
			foreach(Userod user in listUsers) {
				EmailAddress userAddress=EmailAddresses.GetForUser(user.UserNum);
				if(userAddress!=null) {
					_listEmailAddresses.Insert(0,userAddress);
				}
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"User Name"),240);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Sender Address"),270);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"User"),135);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Default"),50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Notify"),50,HorizontalAlignment.Center) { IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(EmailAddress emailAddress in _listEmailAddresses) {
				row=new GridRow();
				row.Cells.Add(emailAddress.EmailUsername);
				row.Cells.Add(emailAddress.SenderAddress);
				row.Cells.Add(Userods.GetName(emailAddress.UserNum));
				row.Cells.Add((emailAddress.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum))?"X":"");
				row.Cells.Add((emailAddress.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum))?"X":"");
				row.Tag=emailAddress;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butSetDefault_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.SelectedTag<EmailAddress>().UserNum>0) {
				MsgBox.Show(this,"User email address cannot be set as the default.");
				return;
			}
			if(Prefs.UpdateLong(PrefName.EmailDefaultAddressNum,_listEmailAddresses[gridMain.GetSelectedIndex()].EmailAddressNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butWebMailNotify_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.SelectedTag<EmailAddress>().UserNum>0) {
				MsgBox.Show(this,"User email address cannot be set as WebMail Notify.");
				return;
			}
			if(Prefs.UpdateLong(PrefName.EmailNotifyAddressNum,_listEmailAddresses[gridMain.GetSelectedIndex()].EmailAddressNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEmailAddressEdit FormEAE=new FormEmailAddressEdit(0,true);
			FormEAE.ShowDialog();
			if(FormEAE.DialogResult==DialogResult.OK) {
				FillGrid();
				IsChanged=true;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(IsSelectionMode) {
				if(gridMain.GetSelectedIndex()==-1) {
					MsgBox.Show(this,"Please select an email address.");
					return;
				}
				EmailAddressNum=_listEmailAddresses[gridMain.GetSelectedIndex()].EmailAddressNum;
			}
			else {//The following fields are only visible when not in selection mode.
				int inboxCheckIntervalMinuteCount=0;
				try {
					inboxCheckIntervalMinuteCount=int.Parse(textInboxCheckInterval.Text);
					if(inboxCheckIntervalMinuteCount<1 || inboxCheckIntervalMinuteCount>60) {
						throw new ApplicationException("Invalid value.");//User never sees this message.
					}
				}
				catch {
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