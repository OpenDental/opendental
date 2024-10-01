using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using OpenDentBusiness.InternalTools.Job_Manager.HelperClasses;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormTeamSummary:FormODBase {
		private const string _OTHER_NOTES_TAB_PAGE_TITLE="Other Notes";
		private const string _RICH_TEXT_BOX_DISCUSSION="richTextBoxDiscussion";
		private const string _RICH_TEXT_BOX_JOBS="richTextBoxJobs";
		private List<TeamReportUser> _listTeamReportUsers=new List<TeamReportUser>();
		private string _subjectLine;
		private string _jobTeamName;

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
			float fontSize=LayoutManager.ScaleF(8.25f);
			//Add discussion label
			Label labelDiscussion=new Label();
			labelDiscussion.Text="Discussion Notes";
			labelDiscussion.Size=new Size(LayoutManager.Scale(100),LayoutManager.Scale(18));
			labelDiscussion.Location=new Point(LayoutManager.Scale(2),LayoutManager.Scale(5));
			labelDiscussion.Font=new Font("Microsoft Sans Serif",fontSize);
			LayoutManager.Add(labelDiscussion,newTab);
			//Add discussion textrich
			RichTextBox richTextBoxDiscussion=CreateRichTextBox();
			richTextBoxDiscussion.Name=_RICH_TEXT_BOX_DISCUSSION;
			richTextBoxDiscussion.SelectionBullet=true;
			richTextBoxDiscussion.Size=new Size(LayoutManager.Scale(newTab.Width-10),LayoutManager.Scale(280));
			if(title==_OTHER_NOTES_TAB_PAGE_TITLE) {
				richTextBoxDiscussion.Size=new Size(LayoutManager.Scale(newTab.Width-10),LayoutManager.Scale(580));
			}
			richTextBoxDiscussion.Location=new Point(LayoutManager.Scale(5),LayoutManager.Scale(25));
			richTextBoxDiscussion.Font=new Font("Microsoft Sans Serif",fontSize);
			LayoutManager.Add(richTextBoxDiscussion,newTab);
			if(title!=_OTHER_NOTES_TAB_PAGE_TITLE) {
				//Add job label
				Label labelJobs=new Label();
				labelJobs.Text="Jobs";
				labelJobs.Size=new Size(LayoutManager.Scale(40),LayoutManager.Scale(18));
				labelJobs.Location=new Point(LayoutManager.Scale(2),LayoutManager.Scale(315));
				labelJobs.Font=new Font("Microsoft Sans Serif",fontSize);
				LayoutManager.Add(labelJobs,newTab);
				//Add job textrich
				RichTextBox richTextBox=CreateRichTextBox();
				richTextBox.Name=_RICH_TEXT_BOX_JOBS;
				richTextBox.Size=new Size(LayoutManager.Scale(newTab.Width-10),LayoutManager.Scale(280));
				richTextBox.Location=new Point(LayoutManager.Scale(5),LayoutManager.Scale(335));
				richTextBox.Font=new Font("Microsoft Sans Serif",fontSize);
				AddAndFormatText(richTextBox,text);
				LayoutManager.Add(richTextBox,newTab);
			}
		}

		public RichTextBox CreateRichTextBox() {
			RichTextBox richTextBox=new RichTextBox();
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

		private void buttonCopy_Click(object sender,EventArgs e) {
			string copyText="";
			for(int i=0;i<tabControlSummary.TabPages.Count;i++) {
				UI.TabPage tabPage=tabControlSummary.TabPages[i];
				if(tabPage.Controls.Count==0) {
					continue;
				}
				//Discussion Notes
				RichTextBox richTextBoxDiscussion=GetRichTextBoxFromTab(tabPage,_RICH_TEXT_BOX_DISCUSSION);
				if(richTextBoxDiscussion==null 
					|| (tabPage.Text==_OTHER_NOTES_TAB_PAGE_TITLE && string.IsNullOrEmpty(richTextBoxDiscussion.Text))) 
				{
					continue;//Kick out if other notes tab is empty
				}
				copyText+=tabPage.Text+"\r\n";//Tabpage title
				copyText+="Discussion Notes:\r\n";
				if(string.IsNullOrEmpty(richTextBoxDiscussion.Text)) {
					copyText+="* No Discussion Notes\r\n";
				}
				else {
					string[] arrayLinesDiscussion=richTextBoxDiscussion.Text.Split(new string[] {"\r\n","\r","\n"},StringSplitOptions.RemoveEmptyEntries);
					for(int j=0;j<arrayLinesDiscussion.Length;j++) {
						copyText+="* "+arrayLinesDiscussion[j]+"\r\n";
					}
				}
				//Job Notes
				RichTextBox richTextBoxJobs=GetRichTextBoxFromTab(tabPage,_RICH_TEXT_BOX_JOBS);
				if(richTextBoxJobs==null || string.IsNullOrEmpty(richTextBoxJobs.Text) || tabPage.Text==_OTHER_NOTES_TAB_PAGE_TITLE) {
					copyText+="\r\n";
					continue;
				}
				copyText+="Jobs:\r\n";
				string[] arrayLinesJobs=richTextBoxJobs.Text.Split(new string[] {"\r\n","\r","\n"},StringSplitOptions.RemoveEmptyEntries);
				for(int j=0;j<arrayLinesJobs.Length;j++) {
					copyText+="* "+arrayLinesJobs[j]+"\r\n";
				}
				copyText+="\r\n";
			}
			ODClipboard.Clear();
			ODClipboard.SetClipboard(copyText);
		}

		private RichTextBox GetRichTextBoxFromTab(UI.TabPage tabPage,string name) {
			Control[] controls=tabPage.Controls.Find(name,false);
			if(controls.Length==0) {
				return null;
			}
			RichTextBox richTextBox=controls[0] as RichTextBox;
			return richTextBox;
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
			StringBuilder stringBuilder=new StringBuilder();
			RichTextBox richTextBoxDiscussion=GetRichTextBoxFromTab(tabPage,_RICH_TEXT_BOX_DISCUSSION);
			if(richTextBoxDiscussion==null) {
				return stringBuilder.ToString();
			}
			string textRichTextBox=richTextBoxDiscussion.Text.Trim();
			//Append the content if it's not empty, or the tab is not "Other Notes"
			if(!string.IsNullOrEmpty(textRichTextBox) || tabPage.Text!=_OTHER_NOTES_TAB_PAGE_TITLE) {
				stringBuilder.Append($"<strong>{tabPage.Text}</strong>");
				stringBuilder.Append("<p>Discussion Notes:</p>");
				stringBuilder.Append(ConvertStringToHtmlUnorderedList(textRichTextBox));
				if(string.IsNullOrEmpty(textRichTextBox)) {
					stringBuilder.Append($"<ul><li>No Discussion Notes</li></ul>");//Consistent message with the copy button
				}
			}
			RichTextBox richTextBoxJobs=GetRichTextBoxFromTab(tabPage,_RICH_TEXT_BOX_JOBS);
			if(richTextBoxJobs==null) {
				return stringBuilder.ToString();
			}
			string richTextBoxText=richTextBoxJobs.Text.Trim();
			//Append the content if it's not empty, or the tab is not "Other Notes"
			if(string.IsNullOrEmpty(richTextBoxText) || tabPage.Text==_OTHER_NOTES_TAB_PAGE_TITLE) {
				return stringBuilder.ToString();
			}
			stringBuilder.Append("<p>Jobs:</p>");
			stringBuilder.Append(ConvertStringToHtmlUnorderedList(richTextBoxText));
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