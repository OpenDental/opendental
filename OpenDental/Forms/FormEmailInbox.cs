using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEmailInbox:FormODBase {
		///<summary>Do not access directly.  Instead use AddressInbox.</summary>
		private EmailAddress _emailAddress=null;
		///<summary>Should always match the combobox 1:1.</summary>
		///<summary>Unfiltered list of emails showing in the Inbox.</summary>
		private List<EmailMessage> _listEmailMessagesInbox;
		///<summary>Unfiltered list of emails showing in the Sent Messages box.</summary>
		private List<EmailMessage> _listEmailMessagesSent;
		///<summary>Gets refreshed as needed.</summary>
		private List<Patient> _listPatients=new List<Patient>();
		///<summary>Indicates whether or not the Sent Messages box needs to be refreshed.</summary>
		private bool _isRefreshSent=true;
		///<summary>Indicates whether or not the Inbox needs to be refreshed.</summary>
		private bool _isRefreshInbox=true;
		///<summary>The patnum of the patient chosen by the user by clicking the Patient Pick button when searching.</summary>
		private long _patNum;
		///<summary>True when the user is search mode.</summary>
		private bool _isSearching=false;
		///<summary>When _isSearching is true, the following list is used instead if _listInboxEmails.
		///At the point that _isSearching is true, it should always be filled.</summary>
		private List<EmailMessage> _listEmailMessagesInboxSearched;
		///<summary>When _isSearching is true, the following list is used instead of _listSentEmails. 
		///At the point that _isSearching is true, it should always be filled.</summary>
		private List<EmailMessage> _listEmailMessagesSentSearched;
		///<summary>True when the form has been closed.</summary>
		private bool _hasClosed;
		///<summary>List of all HideInFlags, except None.</summary>
		private List<HideInFlags> _listHideInFlags;
		///<summary>Used to set the inbox that will be shown once the form is loaded.
		///String matches the _listEmailAddresses.EmailUsername property</summary>
		private string _emailUsername="";

		/// <summary>Attempts to set the selected inbox based on email username.  Used to show a specific inbox when opening the form.</summary>
		public FormEmailInbox(string emailUsername) : this() {
			_emailUsername=emailUsername;
		}

		public FormEmailInbox() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailInbox_Resize(object sender,EventArgs e) {
			if(_hasClosed) {
				return;
			}
			if(!comboEmailAddress.Items.GetAll<EmailAddress>().IsNullOrEmpty()) {//Since Resize() is called once before Load(), we must ensure FillComboEmail() has been called at least once.
				FillInboxOrSent();
			}
		}

		private void FormEmailInbox_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			//default dates to the last month
			textDateFrom.Text=DateTime.Now.AddDays(-31).ToShortDateString();
			textDateTo.Text=DateTime.Now.ToShortDateString();
			LayoutMenu();
			InitEmailHideInFlags();
			FillComboEmail();
			SetButtonsEnabled();
			if(comboEmailAddress.Items is null || comboEmailAddress.Items.Count==0) {
				MsgBox.Show(this,"No email addresses available for current user.  Edit email address info in the File menu or Setup menu.");
				this.Close();
				return;
			}
			//If a email username is passed in, we will try to set the form to that inbox.
			if(!string.IsNullOrWhiteSpace(_emailUsername)) {
				comboEmailAddress.SelectedItem=_emailUsername;
			}
			listManualMessageSource.Items.Clear();
			listManualMessageSource.Items.AddList(EmailMessage.GetMsgTypesForEmailClient(),x => Lan.g("enum"+typeof(EmailMessageSource).Name,x.GetDescription()));
			if(!IsWebMail()) {
				listManualMessageSource.SetAll(true);
			}
			listAutomatedMessageSource.Items.Clear();
			listAutomatedMessageSource.Items.AddList(EmailMessage.GetMsgTypesForEmailClient(isAutomated:true),x => Lan.g("enum"+typeof(EmailMessageSource).Name,x.GetDescription()));
			groupSentMessageSource.Enabled=GetActiveMailbox()==MailboxType.Sent;
			Application.DoEvents();//Show the form contents before loading email into the grid.
			try {
				GetMessages();//If no new messages, then the user will know based on what shows in the inbox grid.
				FillGridSent();
			}
			catch (ODException ex) {
				if(ex.ErrorCodeAsEnum == ODException.ErrorCodes.FormClosed) {
					return;
				}
				throw ex;
			}
			//Manually scale the hieight and width since we launch this form in maximized mode, the form's Restore.bounds property will not automatically scale to account for any potential zoom setting.
			Height=LayoutManager.Scale(735);//Maximum allowed height
			Width=LayoutManager.Scale(1246);//Maximum allowed width
			Cursor=Cursors.Default;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		///<summary>This function is called on load and also when signals are received indicating a refresh is needed for email addresses.</summary>
		private void FillComboEmail() {
			//Emails to show:
			//Me
			//Default Practice
			//All email addresses for my clinics
			//All other email addresses not tied to a user or clinic
			long emailAddressNumPreviouslySelected=-1;
			if(GetSelectedAddress()!=null) {
				emailAddressNumPreviouslySelected=GetSelectedAddress().EmailAddressNum;
			}
			long curUserNum=Security.CurUser.UserNum;
			long defaultEmailAddressNum=PrefC.GetLong(PrefName.EmailDefaultAddressNum);
			//Get all addresses that are tied to either the current user or no user at all
			List<long> listEmailAddressNumsForUserClinics=new List<long>();
			List<long> listEmailAddressNumsAllClinics=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				listEmailAddressNumsForUserClinics=Clinics.GetForUserod(Security.CurUser).Select(x=>x.EmailAddressNum).ToList();
				listEmailAddressNumsAllClinics=Clinics.GetDeepCopy().Select(x => x.EmailAddressNum).ToList();
			}
			List<EmailAddress> listEmailAddresses=EmailAddresses.GetDeepCopy().Where(x=>
				(x.UserNum==curUserNum || //associated with current user  
				x.EmailAddressNum==defaultEmailAddressNum || //or is the practice default
				(x.UserNum==0 && listEmailAddressNumsForUserClinics.Contains(x.EmailAddressNum)) ||//or has no user and is associated with any clinics this user has access to
				(x.UserNum==0 && !listEmailAddressNumsAllClinics.Contains(x.EmailAddressNum))) //or has no user and is not associated to any clinic
			)
				.OrderByDescending(x=>x.UserNum==curUserNum) //place user email first
				.ThenByDescending(x=>x.EmailAddressNum==defaultEmailAddressNum)//then practice default
				.ToList();
			if(Security.CurUser.ProvNum!=0) {//Only providers have access to see Webmail messages.
				EmailAddress webMail=new EmailAddress() {
					EmailUsername="WebMail",
					WebmailProvNum=Security.CurUser.ProvNum,
				};
				listEmailAddresses.Add(webMail);
			}
			comboEmailAddress.Items.Clear();//Clear out email addresses currently in combobox as to not duplicate them
			comboEmailAddress.Items.AddList(listEmailAddresses,x=>EmailAddresses.GetDisplayStringForComboBox(x,curUserNum,defaultEmailAddressNum));
			bool isPreviousAddressAvailable=listEmailAddresses.Any(x => x.EmailAddressNum==emailAddressNumPreviouslySelected);//Checks if previously selected email address still available in comboBox
			if(emailAddressNumPreviouslySelected<0 || !isPreviousAddressAvailable) {
				comboEmailAddress.SelectedIndex=0;//select the first one, will happen on load
			}
			else {
				comboEmailAddress.SetSelectedKey<EmailAddress>(emailAddressNumPreviouslySelected,x=>x.EmailAddressNum);
			}
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormEmailAddresses formEmailAddresses=new FormEmailAddresses();
			formEmailAddresses.ShowDialog();
			FillComboEmail();
		}

		///<summary>The currently showing mailbox.</summary>
		private MailboxType GetActiveMailbox() {
			switch(tabControl.SelectedIndex) {
				case 0:
					return MailboxType.Inbox;
				case 1:
					return MailboxType.Sent;
				default:
					throw new Exception("Unknown Mailbox");
			}
		}

		///<summary>The currently showing grid. Based off ActiveMailbox above.</summary>
		private GridOD GetActiveGrid() {
			switch(GetActiveMailbox()) {
				case MailboxType.Inbox:
					return gridInbox;
				case MailboxType.Sent:
					return gridSent;
				default:
					throw new Exception("Unknown Mailbox");
			}
		}

		///<summary>The currently selected email address.</summary>
		private EmailAddress GetSelectedAddress() {
			if(comboEmailAddress.GetSelected<EmailAddress>() is null) {
				_emailAddress=null;
			}
			else {
				_emailAddress=comboEmailAddress.GetSelected<EmailAddress>();
			}
			return _emailAddress;
		}

		///<summary>Gets messages from the db and fills the grid. Also, resends previously failed acknowledgements when the current email address is set up to receive email.S</summary>
		private void GetMessages() {
			_isRefreshInbox=true;
			FillGridInbox();
			if(IsWebMail()) { //WebMail is selected
				return;//Webmail messages are in the database, so we will not need to receive email from the email server.
			}
			if(GetSelectedAddress().EmailUsername=="" || GetSelectedAddress().Pop3ServerIncoming=="") {//Email address not setup.
				return;
			}
			Text="Email Client for "+GetSelectedAddress().EmailUsername+" - Resending any acknowledgments which previously failed...";
			EmailMessages.SendOldestUnsentAck(GetSelectedAddress());
			Text="Email Client for "+GetSelectedAddress().EmailUsername;
		}

		///<summary>Gets new emails and also shows older emails from the database.</summary>
		private void FillGridInbox() {
			//Remember current email selections and sorting column.
			List<long> listEmailMessageNumsSelected=new List<long>();
			for(int i=0;i<gridInbox.SelectedIndices.Length;i++) {
				EmailMessage emailMessage=(EmailMessage)gridInbox.ListGridRows[gridInbox.SelectedIndices[i]].Tag;
				listEmailMessageNumsSelected.Add(emailMessage.EmailMessageNum);
			}
			int sortByColIdx=gridInbox.GetSortedByColumnIdx();
			bool isSortAsc=gridInbox.IsSortedAscending();
			if(sortByColIdx==-1) {
				//Default to sorting by Date Received descending.
				sortByColIdx=2;
				isSortAsc=false;
			}
			if(_listEmailMessagesInbox==null || _isRefreshInbox) {
				RefreshInboxEmailList();
			}
			gridInbox.BeginUpdate();
			gridInbox.ListGridRows.Clear();
			gridInbox.Columns.Clear();
			int widthColDateReceived=140;
			int widthColMessageType=120;
			int widthColFrom=200;
			int widthColSig=40;
			int widthColPatient=140;
			gridInbox.Columns.Add(new UI.GridColumn(Lan.g(this,"From"),widthColFrom,HorizontalAlignment.Left));//0
			gridInbox.Columns[gridInbox.Columns.Count-1].SortingStrategy=UI.GridSortingStrategy.StringCompare;
			//Make the grid column dynamic so that it always adjusts the width correctly when using the Zoom feature.
			gridInbox.Columns.Add(new UI.GridColumn(Lan.g(this,"Subject"),50,HorizontalAlignment.Left){IsWidthDynamic=true});//1
			gridInbox.Columns[gridInbox.Columns.Count-1].SortingStrategy=UI.GridSortingStrategy.StringCompare;
			gridInbox.Columns.Add(new UI.GridColumn(Lan.g(this,"Date Received"),widthColDateReceived,HorizontalAlignment.Left));//2
			gridInbox.Columns[gridInbox.Columns.Count-1].SortingStrategy=UI.GridSortingStrategy.DateParse;
			gridInbox.Columns.Add(new UI.GridColumn(Lan.g(this,"MessageType"),widthColMessageType,HorizontalAlignment.Left));//3
			gridInbox.Columns[gridInbox.Columns.Count-1].SortingStrategy=UI.GridSortingStrategy.StringCompare;
			gridInbox.Columns.Add(new UI.GridColumn(Lan.g(this,"Sig"),widthColSig,HorizontalAlignment.Center));//4
			gridInbox.Columns[gridInbox.Columns.Count-1].SortingStrategy=UI.GridSortingStrategy.StringCompare;
			gridInbox.Columns.Add(new UI.GridColumn(Lan.g(this,"Patient"),widthColPatient,HorizontalAlignment.Left));//5
			gridInbox.Columns[gridInbox.Columns.Count-1].SortingStrategy=UI.GridSortingStrategy.StringCompare;
			List<EmailMessage> listEmailMessagesFiltered;
			if(_isSearching) { //if searching, use the search list. Should already be filled.
				listEmailMessagesFiltered=_listEmailMessagesInboxSearched.Where(x => EmailMessages.GetAddressSimple(x.RecipientAddress).ToLower().Contains(GetSelectedAddress().EmailUsername.ToLower())).ToList();
			}
			else {
				if(IsWebMail()) {
					listEmailMessagesFiltered=_listEmailMessagesInbox.Where(x => x.ProvNumWebMail==Security.CurUser.ProvNum).ToList();
				}
				else {
					listEmailMessagesFiltered=_listEmailMessagesInbox.Where(x => EmailMessages.GetAddressSimple(x.RecipientAddress).ToLower().Contains(GetSelectedAddress().EmailUsername.ToLower())).ToList();
				}
			}
			//Refresh the local list of patient names with all of the patients in the filtered email list.
			if(!listEmailMessagesFiltered.IsNullOrEmpty()) {
				List<long> listPatNums=listEmailMessagesFiltered.Select(x => x.PatNum).ToList();
				listPatNums.AddRange(listEmailMessagesFiltered.Select(x => x.PatNumSubj).ToList());
				RefreshPatientsList(listPatNums.Distinct().ToList());
			}
			for(int i=0;i<listEmailMessagesFiltered.Count;i++) {
				EmailMessage emailMessage=listEmailMessagesFiltered[i];
				if(!checkShowHiddenEmails.Checked && emailMessage.HideIn.HasFlag(HideInFlags.EmailInbox)) {
					continue;
				}
				UI.GridRow row=new UI.GridRow();
				row.Tag=emailMessage;//Used to locate the correct email message if the user decides to sort the grid.
				if(EmailMessages.IsUnread(emailMessage.SentOrReceived)) {
					row.Bold=true;//unread
									  //row.ColorText=UI.ODPaintTools.ColorNotify;
				}
				row.Cells.Add(new UI.GridCell(emailMessage.FromAddress));//0 From
				row.Cells.Add(new UI.GridCell(emailMessage.Subject));//1 Subject
				row.Cells.Add(new UI.GridCell(emailMessage.MsgDateTime.ToString()));//2 ReceivedDate
				row.Cells.Add(new UI.GridCell(EmailMessages.GetEmailSentOrReceivedDescript(emailMessage.SentOrReceived)));//3 MessageType
				string sigTrust="";//Blank for no signature, N for untrusted signature, Y for trusted signature.
				for(int j=0;j<emailMessage.Attachments.Count;j++) {
					if(emailMessage.Attachments[j].DisplayedFileName.ToLower()!="smime.p7s") {
						continue;//Not a digital signature.
					}
					sigTrust="N";
					//A more accurate way to test for trust would be to read the subject name from the certificate, then check the trust for the subject name instead of the from address.
					//We use the more accurate way inside FormEmailDigitalSignature.  However, we cannot use the accurate way inside the inbox because it would cause the inbox to load very slowly.
					if(EmailMessages.GetReceiverUntrustedCount(emailMessage.FromAddress)==-1) {
						sigTrust="Y";
					}
					break;
				}
				row.Cells.Add(new UI.GridCell(sigTrust));//4 Sig
				long patNumRegardingPatient=emailMessage.PatNum;
				//Webmail messages should list the patient as the PatNumSubj, which means "the patient whom this message is regarding".
				if(EmailMessages.IsSecureWebMail(emailMessage.SentOrReceived)) {
					patNumRegardingPatient=emailMessage.PatNumSubj;
				}
				row.Cells.Add(GetPatientName(patNumRegardingPatient));//5 Patient
				gridInbox.ListGridRows.Add(row);
			}
			gridInbox.EndUpdate();
			//sorting/reselcting previously selected rows
			gridInbox.SortForced(sortByColIdx,isSortAsc);
			//Selection must occur after EndUpdate().
			for(int i=0;i<gridInbox.ListGridRows.Count;i++) {
				EmailMessage emailMessage=(EmailMessage)gridInbox.ListGridRows[i].Tag;
				if(listEmailMessageNumsSelected.Contains(emailMessage.EmailMessageNum)) {
					gridInbox.SetSelected(i,true);
				}
			}
			//Do not show email preview panel until an email has been clicked on.
			if(gridInbox.SelectedIndices.Length!=1) { //collapse the panel if they don't have exactly one email selected.
				splitContainerInbox.Panel2Collapsed=true;
			}
			RefreshShowIn();
		}

		///<summary>Fills the Sent grid with emails. 
		///Set _refreshSent to true to refresh the local list of emails from the db before calling this method.</summary>
		private void FillGridSent() {
			//remember emails selected
			List<long> listEmailMessageNumsSelected=new List<long>();
			for(int i=0;i<gridSent.SelectedIndices.Length;i++) {
				EmailMessage emailMessage=(EmailMessage)gridSent.ListGridRows[gridSent.SelectedIndices[i]].Tag;
				listEmailMessageNumsSelected.Add(emailMessage.EmailMessageNum);
			}
			//sorting
			int sortByColIdx=gridSent.GetSortedByColumnIdx();
			bool isSortAsc=gridSent.IsSortedAscending();
			if(sortByColIdx==-1) {
				//Default to sorting by Date Received descending.
				sortByColIdx=3;
				isSortAsc=false;
			}
			if(_listEmailMessagesSent==null || _isRefreshSent==true) {
				RefreshSentEmailList();
			}
			//calculate column widths
			int widthColSent=180;
			int widthColDateSent=120;
			int widthColMsg=95;
			int widthColAutomatedSig=40;
			int widthColPatient=140;
			gridSent.BeginUpdate();
			gridSent.Columns.Clear();
			//add columns
			GridColumn col=new GridColumn("Sent From",widthColSent,HorizontalAlignment.Left);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			col=new GridColumn("Sent To",widthColSent,HorizontalAlignment.Left);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			//Make the grid column dynamic so that it always adjusts the width correctly when using the Zoom feature.
			col=new GridColumn("Subject",50,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			col=new GridColumn("Date Sent",widthColDateSent,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridSent.Columns.Add(col);
			col=new GridColumn("MsgType",widthColMsg,HorizontalAlignment.Left);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			col=new GridColumn("MsgSource",widthColMsg,HorizontalAlignment.Left);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			col=new GridColumn("IsAutomated",widthColAutomatedSig,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			col=new GridColumn("Sig",widthColAutomatedSig,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			col=new GridColumn("Patient",widthColPatient,HorizontalAlignment.Left);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridSent.Columns.Add(col);
			if(checkShowFailedSent.Checked) {
				col=new GridColumn("FailReason",150,HorizontalAlignment.Left);
				col.SortingStrategy=GridSortingStrategy.StringCompare;
				gridSent.Columns.Add(col);
			}
			gridSent.ListGridRows.Clear();
			List<EmailMessage> listEmailsFiltered;
			EmailAddress emailAddressSelected=GetSelectedAddress();
			string[] arrayAddresses={ emailAddressSelected.EmailUsername.ToLower() };
			if(!string.IsNullOrEmpty(emailAddressSelected.SenderAddress)) {
				arrayAddresses=arrayAddresses.Append(emailAddressSelected.SenderAddress.ToLower()).ToArray();
			}
			if(_isSearching) { //if searching, use the search list. Should be prefilled.
				listEmailsFiltered=_listEmailMessagesSentSearched.FindAll(x => EmailMessages.GetAddressSimple(x.FromAddress).ToLower().In(arrayAddresses));
			}
			else {
				if(IsWebMail()) {
					listEmailsFiltered=_listEmailMessagesSent.Where(x => x.ProvNumWebMail==Security.CurUser.ProvNum).ToList();
				}
				else {
					listEmailsFiltered=_listEmailMessagesSent.FindAll(x => EmailMessages.GetAddressSimple(x.FromAddress).ToLower().In(arrayAddresses));
				}
			}
			//Refresh the local list of patient names with all of the patients in the filtered email list.
			if(!listEmailsFiltered.IsNullOrEmpty()) {
				List<long> listPatNums=listEmailsFiltered.Select(x => x.PatNum).ToList();
				listPatNums.AddRange(listEmailsFiltered.Select(x => x.PatNumSubj).ToList());
				RefreshPatientsList(listPatNums.Distinct().ToList());
			}
			//add rows
			for(int i=0;i<listEmailsFiltered.Count;i++) {
				if(!checkShowHiddenEmails.Checked && listEmailsFiltered[i].HideIn.HasFlag(HideInFlags.EmailInbox)) {//We might need a separate HideInFlags.EmailSent option later.
					continue;
				}
				if(IsWebMail()) {
					if(!EmailMessages.GetSentTypes(EmailPlatform.WebMail).Contains(listEmailsFiltered[i].SentOrReceived)) {
						continue;
					}
				}
				else {
					if(!listManualMessageSource.GetListSelected<EmailMessageSource>().Contains(listEmailsFiltered[i].MsgType)
						&& !listAutomatedMessageSource.GetListSelected<EmailMessageSource>().Contains(listEmailsFiltered[i].MsgType))
					{
						continue;
					}
				}
				if(!checkShowFailedSent.Checked && listEmailsFiltered[i].SentOrReceived==EmailSentOrReceived.SendFailed) {
					//Only show failed messages when show failed sent checkbox is checked
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(listEmailsFiltered[i].FromAddress);
				row.Cells.Add(listEmailsFiltered[i].ToAddress);
				row.Cells.Add(listEmailsFiltered[i].Subject);
				row.Cells.Add(listEmailsFiltered[i].MsgDateTime.ToShortDateString()+" "+listEmailsFiltered[i].MsgDateTime.ToShortTimeString());
				row.Cells.Add(listEmailsFiltered[i].SentOrReceived.ToString());
				row.Cells.Add(listEmailsFiltered[i].MsgType.GetDescription());
				string strIsAutomated="";
				if(listEmailsFiltered[i].IsAutomated) {
					strIsAutomated="X";
				}
				row.Cells.Add(strIsAutomated);
				string sigTrust="";
				for(int j=0;j<listEmailsFiltered[i].Attachments.Count;j++) {
					if(listEmailsFiltered[i].Attachments[j].DisplayedFileName.ToLower()!="smime.p7s") {
						continue;//Not a digital signature.
					}
					sigTrust="N";
					//A more accurate way to test for trust would be to read the subject name from the certificate, then check the trust for the subject name instead of the from address.
					//We use the more accurate way inside FormEmailDigitalSignature.  However, we cannot use the accurate way inside the inbox because it would cause the inbox to load very slowly.
					if(EmailMessages.IsSenderTrusted(listEmailsFiltered[i].FromAddress)) {
						sigTrust="Y";
					}
					break;
				}
				row.Cells.Add(sigTrust);
				long patNum=listEmailsFiltered[i].PatNum;
				//Webmail messages should list the patient as the PatNumSubj, which means "the patient whom this message is regarding".
				if(EmailMessages.IsSecureWebMail(listEmailsFiltered[i].SentOrReceived)) {
					patNum=listEmailsFiltered[i].PatNumSubj;
				}
				row.Cells.Add(GetPatientName(patNum));//5 Patient
				if(checkShowFailedSent.Checked) {
					row.Cells.Add(listEmailsFiltered[i].FailReason);
				}
				row.Tag=listEmailsFiltered[i];
				gridSent.ListGridRows.Add(row);
			}
			gridSent.EndUpdate();
			//sorting/reselcting previously selected rows
			gridSent.SortForced(sortByColIdx,isSortAsc);
			//Select the previously selected emails
			for(int i=0;i<gridSent.ListGridRows.Count;i++) {
				EmailMessage emailMessage=(EmailMessage)gridSent.ListGridRows[i].Tag;
				if(listEmailMessageNumsSelected.Contains(emailMessage.EmailMessageNum)) {
					gridSent.SetSelected(i,true);
				}
			}
			//Do not show email preview panel until an email has been clicked on.
			if(gridSent.SelectedIndices.Length!=1) { //collapse the panel if they don't have exactly one email selected.
				splitContainerSent.Panel2Collapsed=true;
				LayoutManager.LayoutControlBoundsAndFonts(splitContainerSent);
			}
			RefreshShowIn();
		}

		///<summary>Loads HideInFlags into listEmailHideInFlags and adds them listHideInFlags.</summary>
		private void InitEmailHideInFlags() {
			_listHideInFlags=Enum.GetValues(typeof(HideInFlags)).Cast<HideInFlags>().Where(x => x!=HideInFlags.None).ToList();
			for(int i=0;i<_listHideInFlags.Count;i++) {
				listShowIn.Items.Add(Lan.g("enumHideInFlags",(_listHideInFlags[i]).GetDescription()));
			}
		}

		///<summary>Returns true if the currently selected email client is WebMail.</summary>
		private bool IsWebMail() {
			return GetSelectedAddress().EmailUsername=="WebMail";
		}

		///<summary>Updates listShowIn according to current selections in GridSent/GridInbox</summary>
		private void RefreshShowIn() {
			EmailMessage emailMessageCur=null;
			if(GetActiveGrid().SelectedGridRows.Count!=0) { //at least one selected email
				emailMessageCur=(EmailMessage)(GetActiveGrid().SelectedGridRows[0].Tag);
			}
			//set all listShowIn Items to match selected email, default to all items not selected if no email selected
			for(int i=0;i<_listHideInFlags.Count;i++) {
				listShowIn.Items.SetValue(i,Lan.g("enumHideInFlags",_listHideInFlags[i].GetDescription()));
				if(emailMessageCur==null) {
					listShowIn.SetSelected(i,false);//no selected email
				}
				else {
					bool isShowing=!emailMessageCur.HideIn.HasFlag(_listHideInFlags[i]);//HideInFlags logic inverted for "showing"
					listShowIn.SetSelected(i,isShowing);
				}
			}
			if(emailMessageCur==null) {
				listShowIn.Enabled=false;
				return;
			}
			//compare current email HideInFlags to other selected emails and modify listShowIn accordingly
			for(int r=0;r<GetActiveGrid().SelectedGridRows.Count;r++) {
				EmailMessage emailMessage=(EmailMessage)GetActiveGrid().SelectedGridRows[r].Tag;
				if(emailMessage.HideIn==emailMessageCur.HideIn) {
					continue;
				}
				//mismatched flags across multiple selected emails should display as "showing"+"Settings Vary"
				for(int i=0;i<_listHideInFlags.Count;i++) {
					if(emailMessage.HideIn.HasFlag(_listHideInFlags[i])!=emailMessageCur.HideIn.HasFlag(_listHideInFlags[i])) {
						listShowIn.Items.SetValue(i,Lan.g("enumHideInFlags",_listHideInFlags[i].GetDescription()+" *Settings Vary"));
						listShowIn.SetSelected(i);
					}
				}
			}
		}

		///<summary>Fills the two classwide search lists and calls the FillGrid methods to filter them.
		///By the time this is called, the _isSearching variable should already be set to true.</summary>
		private void FillSearch() {
			string searchBody=textSearchBody.Text;
			string searchEmail=textSearchEmail.Text;
			DateTime dateSearchFrom=PIn.Date(textDateFrom.Text); //returns MinVal if empty or invalid.
			DateTime dateSearchTo=PIn.Date(textDateTo.Text); //returns MinVal if empty or invalid.
			List<EmailMessage> listEmailMessagesInboxSearched=new List<EmailMessage>();
			List<EmailMessage> listEmailMessagesSentSearched=new List<EmailMessage>();
			if(searchBody!="") {
				//We have to run a query here to search the database, since our cache only includes the first 50 characters of the body text for preview.
				List<EmailMessage> listEmailMessagesSearched=EmailMessages.GetBySearch(_patNum,searchEmail,dateSearchFrom,dateSearchTo,searchBody,checkSearchAttach.Checked);
				//inbox emails
				listEmailMessagesInboxSearched=listEmailMessagesSearched.Where(x => EmailMessages.IsReceived(x.SentOrReceived)).ToList();
				//sent messages
				listEmailMessagesSentSearched=listEmailMessagesSearched.Where(x => EmailMessages.IsSent(x.SentOrReceived)).ToList();
			}
			else {
				//if not filtering by subject/body, then don't look through the db.
				//Filter Inbox Emails
				for(int i=0;i<_listEmailMessagesInbox.Count;i++) {
					if(_patNum!=0) {
						if(_listEmailMessagesInbox[i].PatNum!=_patNum) {
							continue;
						}
					}
					if(!string.IsNullOrEmpty(searchEmail)) {
						if(!CheckForAddress(_listEmailMessagesInbox[i],searchEmail)) {
							continue;
						}
					}
					if(checkSearchAttach.Checked) {
						if(_listEmailMessagesInbox[i].Attachments.Count<1) {
							continue;
						}
					}
					if(dateSearchFrom!=DateTime.MinValue) {
						if(_listEmailMessagesInbox[i].MsgDateTime.Date<dateSearchFrom.Date) {
							continue;
						}
					}
					if(dateSearchTo!=DateTime.MinValue) {
						if(_listEmailMessagesInbox[i].MsgDateTime.Date>dateSearchTo.Date) {
							continue;
						}
					}
					listEmailMessagesInboxSearched.Add(_listEmailMessagesInbox[i]); //only happens if all the criteria are filled.
				}
				//Filter Sent Emails
				for(int i=0;i<_listEmailMessagesSent.Count;i++) {
					if(_patNum!=0) {
						if(_listEmailMessagesSent[i].PatNum!=_patNum) {
							continue;
						}
					}
					if(!string.IsNullOrEmpty(searchEmail)) {
						if(!CheckForAddress(_listEmailMessagesSent[i],searchEmail)) {
							continue;
						}
					}
					if(checkSearchAttach.Checked) {
						if(_listEmailMessagesSent[i].Attachments.Count<1) {
							continue;
						}
					}
					if(dateSearchFrom!=DateTime.MinValue) {
						if(_listEmailMessagesSent[i].MsgDateTime.Date<dateSearchFrom.Date) {
							continue;
						}
					}
					if(dateSearchTo!=DateTime.MinValue) {
						if(_listEmailMessagesSent[i].MsgDateTime.Date>dateSearchTo.Date) {
							continue;
						}
					}
					listEmailMessagesSentSearched.Add(_listEmailMessagesSent[i]); //only happens if all the criteria are filled.
				}
			}
			_listEmailMessagesInboxSearched=listEmailMessagesInboxSearched;
			_listEmailMessagesSentSearched=listEmailMessagesSentSearched;
			FillGridInbox();
			FillGridSent();
		}

		/// <summary>Searches for a given email address in the passed in message's From,To,Recipient,CC, and BCC addresses.
		/// The message addresses and the search term are both converted to lowercase for comparison.</summary>
		private bool CheckForAddress(EmailMessage emailMessage,string searchTerm) {
			string normalizedTerm=searchTerm.ToLower();
			if(!emailMessage.FromAddress.ToLower().Contains(normalizedTerm)
				&& !emailMessage.ToAddress.ToLower().Contains(normalizedTerm)
				&& !emailMessage.RecipientAddress.ToLower().Contains(normalizedTerm)
				&& !emailMessage.CcAddress.ToLower().Contains(normalizedTerm)
				&& !emailMessage.BccAddress.ToLower().Contains(normalizedTerm))
			{
				return false;
			}
			return true;
		}

		private void gridInboxSent_CellClick(object sender,UI.ODGridClickEventArgs e) {
			UI.SplitContainer splitContainerActive;
			if(GetActiveMailbox()==MailboxType.Inbox){
				splitContainerActive=splitContainerInbox;
			}
			else{
				splitContainerActive=splitContainerSent;
			}
			EmailPreviewControl emailPreviewControlActive=(GetActiveMailbox()==MailboxType.Inbox)?emailPreviewControlInbox:emailPreviewControlSent;
			SetButtonsEnabled();
			RefreshShowIn();
			if(GetActiveGrid().SelectedIndices.Length>=2) {
				splitContainerActive.Panel2Collapsed=true;//Do not show preview if there are more than one emails selected.
				return;
			}
			EmailMessage emailMessage=(EmailMessage)GetActiveGrid().ListGridRows[e.Row].Tag;
			if(EmailMessages.IsSecureWebMail(emailMessage.SentOrReceived)) {
				//We do not yet have a preview for secure web mail messages.
				splitContainerActive.Panel2Collapsed=true;
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(GetActiveMailbox()==MailboxType.Inbox) {
				emailMessage.SentOrReceived=EmailMessages.UpdateSentOrReceivedRead(emailMessage);
			}
			emailMessage=EmailMessages.GetOne(emailMessage.EmailMessageNum);//Refresh from the database to get the full body text.
			if(emailMessage==null) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Email has been deleted.");
				FillInboxOrSent(true);
				return;
			}
			splitContainerActive.Panel2Collapsed=false;
			emailPreviewControlActive.EmailAddressPreview=GetSelectedAddress();
			emailPreviewControlActive.LoadEmailMessage(emailMessage);
			Cursor=Cursors.Default;
			//Handle Sig column clicks.
			if(e.Col!=4) {
				return;//Not the Sig column.
			}
			for(int i=0;i<emailMessage.Attachments.Count;i++) {
				if(emailMessage.Attachments[i].DisplayedFileName.ToLower()!="smime.p7s") {
					continue;
				}
				string smimeP7sFilePath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailMessage.Attachments[i].ActualFileName);
				string localFile=PrefC.GetRandomTempFile(".p7s");
				FileAtoZ.Copy(smimeP7sFilePath,localFile,FileAtoZSourceDestination.AtoZToLocal);
				X509Certificate2 x509Certificate2=EmailMessages.GetEmailSignatureFromSmimeP7sFile(localFile);
				using FormEmailDigitalSignature formEmailDigitalSignature=new FormEmailDigitalSignature(x509Certificate2);
				if(formEmailDigitalSignature.ShowDialog()==DialogResult.OK) {
					Cursor=Cursors.WaitCursor;
					//If the user just added trust, then refresh to pull the newly added certificate into the memory cache.
					EmailMessages.RefreshCertStoreExternal(GetSelectedAddress());
					//Refresh the entire inbox, because there may be multiple email messages from the same address that the user just added trust for.
					//The Sig column may need to be updated on multiple rows.
					try {
						GetMessages();
					}
					catch(ODException ex) {
						if(ex.ErrorCodeAsEnum == ODException.ErrorCodes.FormClosed) {
							return;
						}
						throw ex;
					}
					Cursor=Cursors.Default;
				}
				break;
			}
		}

		private void gridInboxSent_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			EmailMessage emailMessage=(EmailMessage)GetActiveGrid().ListGridRows[e.Row].Tag;
			//When an email is read from the database for display in the inbox, the BodyText is limited to 50 characters and the RawEmailIn is blank.
			emailMessage=EmailMessages.GetOne(emailMessage.EmailMessageNum);//Refresh the email from the database to include the full BodyText and RawEmailIn.
			if(EmailMessages.IsSecureWebMail(emailMessage.SentOrReceived)) {
				//web mail uses special secure messaging portal
				using FormWebMailMessageEdit formWebMailMessageEdit=new FormWebMailMessageEdit(emailMessage.PatNum,emailMessage);
				//Will return Abort if validation fails on load or message was deleted, in which case do not set email as read.
				if(formWebMailMessageEdit.ShowDialog() != DialogResult.Abort) {
					emailMessage.SentOrReceived=EmailMessages.UpdateSentOrReceivedRead(emailMessage);//Mark the message read.
				}
			}
			else {
				emailMessage.SentOrReceived=EmailMessages.UpdateSentOrReceivedRead(emailMessage); //mark read
				FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,GetSelectedAddress(),false);
				formEmailMessageEdit.FormClosed+=FormEmailMessage_Closed; //takes care of updating email message grid (Inbox/Sent) when edit window closes
				formEmailMessageEdit.Show(); //any changes are taken care of in the edit window by signals.
			}
		}

		private void butMarkUnread_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			for(int i=0;i<gridInbox.SelectedIndices.Length;i++) {
				EmailMessage emailMessage=(EmailMessage)gridInbox.ListGridRows[gridInbox.SelectedIndices[i]].Tag;
				emailMessage.SentOrReceived=EmailMessages.UpdateSentOrReceivedUnread(emailMessage);
			}
			FillGridInbox();
			Cursor=Cursors.Default;
		}

		private void butMarkRead_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			for(int i=0;i<gridInbox.SelectedIndices.Length;i++) {
				EmailMessage emailMessage=(EmailMessage)gridInbox.ListGridRows[gridInbox.SelectedIndices[i]].Tag;
				emailMessage.SentOrReceived=EmailMessages.UpdateSentOrReceivedRead(emailMessage);
			}
			FillGridInbox();
			Cursor=Cursors.Default;
		}

		private void butChangePat_Click(object sender,EventArgs e) {
			if(GetActiveGrid().SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an email message.");
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			for(int i=0;i<GetActiveGrid().SelectedIndices.Length;i++) {
				EmailMessage emailMessage=(EmailMessage)GetActiveGrid().ListGridRows[GetActiveGrid().SelectedIndices[i]].Tag;
				emailMessage.PatNum=formPatientSelect.PatNumSelected;
				EmailMessages.UpdatePatNum(emailMessage);
			}
			int countMsgsMoved=GetActiveGrid().SelectedIndices.Length;
			FillInboxOrSent();
			Signalods.SetInvalid(InvalidType.EmailMessages); //will refresh for other users.
			Cursor=Cursors.Default;
			MessageBox.Show(Lan.g(this,"Email messages moved successfully")+": "+countMsgsMoved);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(GetActiveMailbox()==MailboxType.Inbox) {
				try {
					if(string.IsNullOrWhiteSpace(_emailAddress.Pop3ServerIncoming)) {
						GetMessages();
					}
					else {//Insert a signal that will cause the Open Dental Service to retrieve emails when using the inbox feature.
						if(!AlertItems.IsODServiceRunning()) {
							MessageBox.Show(Lans.g("Alerts","No instance of Open Dental Service is running."));
							return;
						}
						Text="Email Client for "+GetSelectedAddress().EmailUsername+" - Receiving new email...";
						Signalod newSignal=new Signalod() {
							FKey=GetSelectedAddress().EmailAddressNum,
							FKeyType=KeyType.EmailAddress,
							IType=InvalidType.EmailInboxRetrieve
						};
						Signalods.Insert(newSignal);
					}
				}
				catch(ODException ex) {
					if(ex.ErrorCodeAsEnum == ODException.ErrorCodes.FormClosed) {
						return;
					}
					throw ex;
				}
			}
			else if(GetActiveMailbox()==MailboxType.Sent) {
				_isRefreshSent=true;
				FillGridSent();
			}
			Cursor=Cursors.Default;
		}

		///<summary>Refreshes the local list of inbox emails.</summary>
		private void RefreshInboxEmailList() {
			DateTime dateFrom=PIn.Date(textDateFrom.Text); //returns MinVal if empty or invalid.
			DateTime dateTo=PIn.Date(textDateTo.Text); //returns MinVal if empty or invalid.
			_listEmailMessagesInbox=EmailMessages.GetMailboxForAddress(GetSelectedAddress(),dateFrom,dateTo,MailboxType.Inbox);
			_isRefreshInbox=false;
		}

		///<summary>Refreshes the local list of sent emails.</summary>
		private void RefreshSentEmailList() {
			DateTime dateFrom=PIn.Date(textDateFrom.Text); //returns MinVal if empty or invalid.
			DateTime dateTo=PIn.Date(textDateTo.Text); //returns MinVal if empty or invalid.
			_listEmailMessagesSent=EmailMessages.GetMailboxForAddress(GetSelectedAddress(),dateFrom,dateTo,MailboxType.Sent);
			_isRefreshSent=false;
		}

		private void RefreshLists() {
			DateTime dateFrom=PIn.Date(textDateFrom.Text); //returns MinVal if empty or invalid.
			DateTime dateTo=PIn.Date(textDateTo.Text); //returns MinVal if empty or invalid.
			List<EmailMessage> listEmailsForSelection=EmailMessages.GetMailboxForAddress(GetSelectedAddress(),dateFrom,dateTo,MailboxType.Inbox,MailboxType.Sent);
			_listEmailMessagesInbox=listEmailsForSelection.Where(x => EmailMessages.IsReceived(x.SentOrReceived)).ToList();
			_listEmailMessagesSent=listEmailsForSelection.Where(x => !EmailMessages.IsReceived(x.SentOrReceived)).ToList();
			_isRefreshSent=false;
			_isRefreshInbox=false;
		}

		///<summary>Adds patients for the PatNums passed in that the list doesn't already contain.</summary>
		private void RefreshPatientsList(List<long> listPatNums) {
			List<long> listPatNumsNew=listPatNums.Except(_listPatients.Select(x=>x.PatNum)).ToList();//not already in list
			List<Patient> listPatientsRefresh=Patients.GetLimForPats(listPatNumsNew);
			for(int i=0;i<listPatientsRefresh.Count;i++) {
				_listPatients.Add(listPatientsRefresh[i]);
			}
		}

		///<summary>Gets the patient name from the local list.  Queries the database and adds to the list if not found.
		///Returns an empty string if the patient couldn't be found in list or in the database.</summary>
		private string GetPatientName(long patNum) {
			if(patNum==0) {
				return "";
			}
			Patient patient=_listPatients.Find(x=>x.PatNum==patNum);
			if(patient!=null) {
				return patient.GetNameLF();
			}
			List<long> listPatNumsNew=new List<long>();
			listPatNumsNew.Add(patNum);
			RefreshPatientsList(listPatNumsNew);
			patient=_listPatients.Find(x=>x.PatNum==patNum);
			if(patient is null) {
				return "";
			}
			return patient.GetNameLF();
		}

		///<summary>Sets the sidebar buttons enabled or disabled based on the currently selected Mailbox.</summary>
		private void SetButtonsEnabled() {
			if(GetActiveGrid().SelectedIndices.Length==0) {
				listShowIn.Enabled=false;
			}
			else {
				listShowIn.Enabled=true;
			}
			if(GetActiveMailbox()==MailboxType.Inbox) {
				if(gridInbox.SelectedIndices.Length>1) {
					butReply.Enabled=false;
					butReplyAll.Enabled=false;
					butForward.Enabled=false;
				}
				else {
					butReply.Enabled=true;
					butReplyAll.Enabled=true;
					butForward.Enabled=true;
				}
				butMarkRead.Enabled=true;
				butMarkUnread.Enabled=true;
			}
			else if(GetActiveMailbox()==MailboxType.Sent) {
				butReply.Enabled=false;
				butReplyAll.Enabled=false;
				butForward.Enabled=false;
				butMarkRead.Enabled=false;
				butMarkUnread.Enabled=false;
			}
		}

		///<summary>For searching.</summary>
		private void butPickPat_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult==DialogResult.OK) {
				_patNum=formPatientSelect.PatNumSelected;
				textSearchPat.Text=GetPatientName(formPatientSelect.PatNumSelected);
			}
		}

		private void butSearch_Click(object sender,EventArgs e) {
			if(textSearchBody.Text=="" && textDateFrom.Text=="" && textDateTo.Text==""&& textSearchEmail.Text=="" && textSearchPat.Text=="" && !checkSearchAttach.Checked) {
				MsgBox.Show(this,"Please specify some search criteria before searching.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			_isSearching=true;
			gridInbox.Title=Lan.g(this,"Currently Searching - Inbox");
			gridSent.Title=Lan.g(this,"Currently Searching - Sent Messages");
			butClear.Enabled=true;
			groupSearch.BackColor=Color.FromArgb(255,255,192); //same as the color of the Appointment Scheduler
																				//if the user typed something into the Subject/Body textbox, then we need to go to the database.
			FillSearch();
			FillInboxOrSent();
			Cursor=Cursors.Default;
		}

		///<summary>Clears search fields.</summary>
		private void butClear_Click(object sender,EventArgs e) {
			textSearchBody.Text="";
			textDateFrom.Text="";
			textDateTo.Text="";
			textSearchEmail.Text="";
			textSearchPat.Text="";
			checkSearchAttach.Checked=false;
			_patNum=0;
			_isSearching=false;
			butClear.Enabled=false;
			gridInbox.Title=Lan.g(this,"Inbox");
			gridSent.Title=Lan.g(this,"Sent Messages");
			groupSearch.BackColor=SystemColors.Control;
			FillInboxOrSent();
		}

		private void butCompose_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.EmailSend)){
				return;
			}
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.FromAddress=GetSelectedAddress().GetFrom();
			emailMessage.MsgType=EmailMessageSource.Manual;
			FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,GetSelectedAddress(),true,null,_listEmailMessagesInbox,_listEmailMessagesSent);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.FormClosed+=FormEmailMessage_Closed; //takes care of updating gridSent
			formEmailMessageEdit.Show();
		}

		private void butReply_Click(object sender,EventArgs e) {
			ReplyClickHelper();
		}

		private void butReplyAll_Click(object sender,EventArgs e) {
			ReplyClickHelper(true);
		}

		private void ReplyClickHelper(bool isReplyAll=false) {
			if(!Security.IsAuthorized(EnumPermType.EmailSend)) {
				return;
			}
			if(gridInbox.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"You must select an email before replying.");
				return;
			}
			EmailMessage emailMessageSelected=EmailMessages.GetOne(((EmailMessage)gridInbox
			  .ListGridRows[gridInbox.GetSelectedIndex()].Tag).EmailMessageNum);//Refresh from the database to get the full body text.
			bool isSecureEmail=EmailMessages.IsSecureEmail(emailMessageSelected.SentOrReceived);
			isReplyAll|=isSecureEmail;//Secure Email is ALWAYS 'Reply All'
			FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(EmailMessages.CreateReply(emailMessageSelected,GetSelectedAddress(),isReplyAll)
				,GetSelectedAddress(),true,emailMessageSelected,_listEmailMessagesInbox,_listEmailMessagesSent);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.FormClosed+=FormEmailMessage_Closed;//takes care of updating the SentMessages grid.
			formEmailMessageEdit.Show();
		}

		private void butForward_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.EmailSend)) {
				return;
			}
			if(gridInbox.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"You must select an email to forward.");
				return;
			}
			EmailMessage emailMessageSelected=EmailMessages.GetOne(((EmailMessage)gridInbox
						.ListGridRows[gridInbox.GetSelectedIndex()].Tag).EmailMessageNum);//Refresh from the database to get the full body text.
			FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(EmailMessages.CreateForward(emailMessageSelected,GetSelectedAddress()),GetSelectedAddress(),true
				,null,_listEmailMessagesInbox,_listEmailMessagesSent);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.FormClosed+=FormEmailMessage_Closed;//takes care of updating the SentMessages grid.
			formEmailMessageEdit.Show();
		}

		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			SetButtonsEnabled();
			FillInboxOrSent();
			groupSentMessageSource.Enabled=false;
			if(GetActiveMailbox()==MailboxType.Sent && !IsWebMail()) {
				groupSentMessageSource.Enabled=true;
			}
			Cursor=Cursors.Default;
		}

		private void comboEmailAddress_SelectionChangeCommitted(object sender,EventArgs e) {
			Text="Email Client for "+GetSelectedAddress().EmailUsername;
			groupSentMessageSource.Enabled=GetActiveMailbox()==MailboxType.Sent;
			if(GetSelectedAddress().EmailUsername==""||GetSelectedAddress().Pop3ServerIncoming=="") {//Email address not setup.
				Text="Email Client - The currently selected email address is not setup to receive email.";
			}
			if(IsWebMail()) { //WebMail is selected
				Text="Email Client for "+GetSelectedAddress().EmailUsername;
				groupSentMessageSource.Enabled=false;
				listAutomatedMessageSource.SetAll(false);
				listManualMessageSource.SetAll(false);
				checkShowFailedSent.Checked=false;
			}
			Cursor=Cursors.WaitCursor;
			RefreshLists();
			if(GetActiveMailbox()==MailboxType.Inbox) {
				try {
					GetMessages();
				}
				catch(ODException ex) {
					if(ex.ErrorCodeAsEnum == ODException.ErrorCodes.FormClosed) {
						return;
					}
					throw ex;
				}
			}
			else {
				FillGridSent();
			}
			Cursor=Cursors.Default;
		}

		///<summary>If someone else is sending emails on another workstation, this will update this form to reflect that.</summary>
		public override void ProcessSignalODs(List<Signalod> listSignalods) {
			if(listSignalods.Exists(x => x.IType==InvalidType.Email)) {
				Cursor=Cursors.WaitCursor;
				//an address may have changed. refill the combobox
				FillComboEmail();
			}
			if(listSignalods.Exists(x => x.IType==InvalidType.EmailMessages && x.FKeyType==KeyType.EmailAddress && x.FKey==GetSelectedAddress().EmailAddressNum)) {
				Text="Email Client for "+GetSelectedAddress().EmailUsername;
				_isRefreshInbox=true;
				FillGridInbox();
			}
			if(listSignalods.Exists(x => x.IType==InvalidType.EmailMessages && x.FKeyType==KeyType.Undefined)) {
				Cursor=Cursors.WaitCursor;
				_isRefreshInbox=true;
				_isRefreshSent=true;
				FillGridInbox();
				FillGridSent();
			}
			Cursor=Cursors.Default;
		}

		///<summary>Refreshes Inbox or Sent depending on currently active mailbox</summary>
		private void FillInboxOrSent(bool isRefreshNeeded=false) {
			//Refresh mailbox with/without Hidden emails
			_isRefreshSent=isRefreshNeeded;
			if(GetActiveMailbox()==MailboxType.Inbox) {
				FillGridInbox();
			}
			else if(GetActiveMailbox()==MailboxType.Sent) {
				FillGridSent();
			}
		}

		///<summary>Builds a HideInFlags flag from listShowIn</summary>
		private HideInFlags GetHideInFlagsFromListBox() {
			HideInFlags hideInFlags=HideInFlags.None;
			for(int i=0;i<listShowIn.SelectedIndices.Count;i++) {
				hideInFlags|=_listHideInFlags[listShowIn.SelectedIndices[i]];
			}
			return (HideInFlags)((HideInFlags)_listHideInFlags.Sum(x => (uint)x)-hideInFlags); //UI and backend logic are flipped (show vs hide).
		}

		private void butAllManual_Click(object sender,EventArgs e) {
			SetSelectedMessageSource(isManualMessageSource:true,setAll:true);
		}

		private void butManualNone_Click(object sender,EventArgs e) {
			SetSelectedMessageSource(isManualMessageSource:true,setAll:false);
		}

		private void butAutomatedNone_Click(object sender,EventArgs e) {
			SetSelectedMessageSource(isManualMessageSource:false,setAll:false);
		}

		private void butAutomatedAll_Click(object sender,EventArgs e) {
			SetSelectedMessageSource(isManualMessageSource:false,setAll:true);
		}

		///<summary>Sets the message sources listbox selected items to all or none.</summary>
		private void SetSelectedMessageSource(bool isManualMessageSource,bool setAll) {
			if(isManualMessageSource) {
				listManualMessageSource.SetAll(setAll);
			}
			else {//automated source
				listAutomatedMessageSource.SetAll(setAll);
			}
			FillGridSent();
		}

		///<summary>Causes ActiveMailbox to refill based on checkShowHiddenEmails status.</summary>
		private void checkShowHiddenEmails_CheckedChanged(object sender,EventArgs e) {
			FillInboxOrSent();
		}

		private void checkShowFailedSent_CheckedChanged(object sender,EventArgs e) {
			FillGridSent();
		}

		private void listMessageSource_MouseCaptureChanged(object sender,EventArgs e) {
			FillGridSent();
		}

		private void listAutomatedMessageSource_MouseCaptureChanged(object sender,EventArgs e) {
			FillGridSent();
		}

		///<summary>Updates email objects,DB, and UI in response to HideInFlags setting selections.</summary>
		private void listHideInFlags_MouseClick(object sender,MouseEventArgs e) {
			bool isGridRefreshRequired=false;
			EmailMessage emailMessage;
			HideInFlags hideInFlags=GetHideInFlagsFromListBox();
			for(int i=0;i<GetActiveGrid().SelectedGridRows.Count;i++) {
				emailMessage=(EmailMessage)GetActiveGrid().SelectedGridRows[i].Tag;
				if(emailMessage.HideIn==hideInFlags) {
					continue;
				}
				if(hideInFlags.HasFlag(HideInFlags.EmailInbox)) {
					isGridRefreshRequired=true;
				}
				EmailMessage emailMessageOld=emailMessage.Copy();
				emailMessage.HideIn=hideInFlags;
				EmailMessages.Update(emailMessage,emailMessageOld,isAttachmentSyncNeeded:false);
			}
			Signalods.SetInvalid(InvalidType.EmailMessages);
			if(isGridRefreshRequired) {
				FillInboxOrSent();
			}
			else {
				RefreshShowIn();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(GetActiveGrid().SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select email to delete or hide.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Permanently delete or hide selected email(s)?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			for(int i=0;i<GetActiveGrid().SelectedIndices.Length;i++) {
				EmailMessage emailMessage=(EmailMessage)GetActiveGrid().ListGridRows[GetActiveGrid().SelectedIndices[i]].Tag;
				//If attached to a patient, simply hide the email message instead of deleting it so that it still shows in other parts of the program.
				if(emailMessage.PatNum!=0) {
					//Create a deep copy of the emailMessage object from the grid which is an altered version of the email message from the database (e.g. BodyText is only up to 50 chars).
					EmailMessage emailMessageOld=emailMessage.Copy();
					//Only manipulate the HideIn column and preserve everything else about the email so that it can be shown in other parts of the program if desired.
					emailMessage.HideIn=(HideInFlags)_listHideInFlags
							  .FindAll(x => !x.In(HideInFlags.AccountCommLog,HideInFlags.AccountProgNotes,HideInFlags.ChartProgNotes)).Sum(x => (int)x);
					EmailMessages.Update(emailMessage,messageOld:emailMessageOld);
					continue;
				}
				if(EmailMessages.IsSecureWebMail(emailMessage.SentOrReceived)) {
					EmailMessages.Delete(emailMessage);
					string logText="";
					logText+="\r\n"+Lan.g(this,"From")+": "+emailMessage.FromAddress+". ";
					logText+="\r\n"+Lan.g(this,"To")+": "+emailMessage.ToAddress+". ";
					if(!String.IsNullOrEmpty(emailMessage.Subject)) {
						logText+="\r\n"+Lan.g(this,"Subject")+": "+emailMessage.Subject+". ";
					}
					if(!String.IsNullOrEmpty(emailMessage.BodyText)) {
						if(emailMessage.BodyText.Length > 50) {
							logText+="\r\n"+Lan.g(this,"Body Text")+": "+emailMessage.BodyText.Substring(0,49)+"... ";
						}
						else {
							logText+="\r\n"+Lan.g(this,"Body Text")+": "+emailMessage.BodyText;
						}
					}
					if(emailMessage.MsgDateTime != DateTime.MinValue) {
						logText+="\r\n"+Lan.g(this,"Date")+": "+emailMessage.MsgDateTime.ToShortDateString()+". ";
					}
					SecurityLogs.MakeLogEntry(EnumPermType.WebMailDelete,emailMessage.PatNum,Lan.g(this,"Webmail deleted.")+" "+logText);
				}
				else {//Not a web mail message.
					EmailMessages.Delete(emailMessage);
				}
			}
			if(GetActiveMailbox()==MailboxType.Inbox) {
				_isRefreshInbox=true;
				FillGridInbox();
			}
			else {
				_isRefreshSent=true;
				FillGridSent();
			}
			Signalods.SetInvalid(InvalidType.EmailMessages);
			Cursor=Cursors.Default;
		}

		///<summary>Since FormEmailMessageEdit is now modeless, this registers for events that 
		///would cause changes to what the current form needs to display so they can be shows immediately.</summary>
		private void FormEmailMessage_Closed(object sender,EventArgs e) {
			if(sender.GetType()!=typeof(FormEmailMessageEdit)) {
				return; //this should never happen
			}
			if(!((FormEmailMessageEdit)sender).DidEmailChange) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			FillInboxOrSent(isRefreshNeeded:true);
			Cursor=Cursors.Default;
		}

		private void FormEmailInbox_FormClosing(object sender,FormClosingEventArgs e) {
			_hasClosed=true;
		}

	}
}