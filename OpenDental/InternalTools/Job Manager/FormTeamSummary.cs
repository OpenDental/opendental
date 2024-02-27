using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using OpenDentBusiness.InternalTools.Job_Manager.HelperClasses;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTeamSummary:FormODBase {
		private const string _OTHER_NOTES_TAB_PAGE_TITLE="Other Notes";
		private List<TeamReportUser> _listTeamReportUsers=new List<TeamReportUser>();
		private string _subjectLine;
		private string _jobTeamName;

		private string _jobTeamAddress {
			get {
				return $"{_jobTeamName.Replace(" ","")}@opendental.com";
			}
		}

		public FormTeamSummary(string jobTeamName,DateTime dateFrom,DateTime dateTo,List<TeamReportUser> listTeamReportUsers) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_jobTeamName=jobTeamName;
			_listTeamReportUsers=listTeamReportUsers;
			_subjectLine=$"{_jobTeamName} Weekly Summary {dateFrom.ToShortDateString()}-{dateTo.ToShortDateString()}";
			this.Text=_subjectLine;//Window title
			InitializeTabs();
		}

		private void InitializeTabs() {
			foreach(TeamReportUser user in _listTeamReportUsers) {
				AddTab(user.UserName,user.GenerateSummary());
			}
			AddTab(_OTHER_NOTES_TAB_PAGE_TITLE,"");
		}

		public void AddTab(string title,string text) {
			UI.TabPage newTab=new UI.TabPage(title);
			LayoutManager.Add(newTab,tabControlSummary);
			RichTextBox richTextBox=CreateRichTextBox();
			AddAndFormatText(richTextBox,text);
			LayoutManager.Add(richTextBox,newTab);
		}

		public RichTextBox CreateRichTextBox() {
			RichTextBox richTextBox=new RichTextBox();
			richTextBox.Dock=DockStyle.Fill;
			richTextBox.BulletIndent=10;
			return richTextBox;
		}

		public void AddAndFormatText(RichTextBox richTextBox,string text) {
			richTextBox.Text=text;
			richTextBox.Select(0,richTextBox.TextLength);
			richTextBox.SelectionBullet=true;
			richTextBox.SelectionIndent=10;
			richTextBox.Select(0,0);
		}

		private void butDraftEmail_Click(object sender,EventArgs e) {
			EmailAddress fromAddress = EmailAddresses.GetFirstOrDefault(x => x.UserNum==Security.CurUser.UserNum);
			if(fromAddress==null) {
				MsgBox.Show(this,"You don't have an email address setup in Open Dental");
				return;
			}
			string autographText="";
			EmailAutograph emailAutograph=EmailAutographs.GetFirstOrDefaultForEmailAddress(fromAddress);
			if(emailAutograph!=null) {
				autographText=emailAutograph.AutographText;
			}
			//Style matches defaults for Desktop Outlook
			string emailBody="" +
				"<body style='font-family:Calibri;font-size:14.5px;'>"
					+"<div>Engineering Management,</div>"
					+"<br>"
					+ConvertTabPagesToHtml()
					+"<br>"
					+autographText
				+"</body>";
			EmailMessage emailMessage=new EmailMessage() { 
				FromAddress=fromAddress.GetFrom(),
				ToAddress="EngineeringManagement@opendental.com",
				CcAddress=_jobTeamAddress,
				Subject=_subjectLine,
				HtmlType=EmailType.RawHtml,
				HtmlText=emailBody,
				BodyText=emailBody,
				MsgType=EmailMessageSource.Manual,
				IsNew=true,
			};
			FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,fromAddress,true);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.FormClosed+=FormEmailMessageEdit_FormClosed;
			formEmailMessageEdit.Show();
		}

		private string ConvertTabPagesToHtml() {
			StringBuilder stringBuilder=new StringBuilder();
			foreach(UI.TabPage tabPage in tabControlSummary.TabPages) {
				stringBuilder.Append(ConvertTabPageToHtml(tabPage));
			}
			return stringBuilder.ToString();
		}

		private string ConvertTabPageToHtml(UI.TabPage tabPage) {
			StringBuilder stringBuilder = new StringBuilder();
			// Assuming the first control is always a RichTextBox. There should be no other controls.
			if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is RichTextBox richTextBox) {
				string richTextBoxText=richTextBox.Text.Trim();
				// Append the content if it's not empty, or the tab is not "Other Notes"
				if (!string.IsNullOrEmpty(richTextBoxText) || tabPage.Text != _OTHER_NOTES_TAB_PAGE_TITLE) {
					stringBuilder.Append($"<strong>{tabPage.Text}</strong>");
					stringBuilder.Append(ConvertStringToHtmlUnorderedList(richTextBoxText));
				}
			}
			return stringBuilder.ToString();
		}

		private string ConvertStringToHtmlUnorderedList(string text) {
			StringBuilder stringBuilder=new StringBuilder();
			string[] arrayLines=text.Split(new string[] {"\r\n","\r","\n"},StringSplitOptions.RemoveEmptyEntries);
			stringBuilder.Append("<ul>");
			foreach(string line in arrayLines) {
				stringBuilder.Append($"<li>{line.Trim()}</li>");
			}
			stringBuilder.Append("</ul>");
			return stringBuilder.ToString();
		}

		private void FormEmailMessageEdit_FormClosed(object sender, FormClosedEventArgs e)
		{
			var formEmailMessageEdit = sender as FormEmailMessageEdit;
			if (formEmailMessageEdit?.DialogResult == DialogResult.OK)
			{
				DialogResult = DialogResult.OK;
			}
		}
	}
}