using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormEmailTemplateEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		public EmailTemplate ETcur;
		private List<EmailAttach> _listEmailAttachDisplayed;
		private List<EmailAttach> _listEmailAttachOld=new List<EmailAttach>();
		private bool _hasTextChanged;
		private bool _isRaw;
		private bool _isLoading;

		///<summary>The fully translated HTML text with the master template, as it would show in a web browser.</summary>
		private string _htmlDocument;

		///<summary></summary>
		public FormEmailTemplateEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailTemplateEdit_Load(object sender, System.EventArgs e) {
			_isLoading=true;
			gridAttachments.ContextMenu=contextMenuAttachments;
			textSubject.Text=ETcur.Subject;
			textDescription.Text=ETcur.Description;
			if(ETcur.EmailTemplateNum==0) {//New email template
				_listEmailAttachDisplayed=new List<EmailAttach>();
				ETcur.BodyText="";
			}
			else {
				_listEmailAttachDisplayed=EmailAttaches.GetForTemplate(ETcur.EmailTemplateNum); 
				foreach(EmailAttach attachment in _listEmailAttachDisplayed) {
					_listEmailAttachOld.Add(attachment.Copy());
				}
			}
			textBodyText.Text=ETcur.BodyText;
			if(EmailMessages.IsHtmlEmail(ETcur.TemplateType)) {
				_hasTextChanged=true;
				_isRaw=(ETcur.TemplateType==EmailType.RawHtml);
				ChangeView(true);
			}
			FillAttachments();
		}

		private void FillAttachments() {
			gridAttachments.BeginUpdate();
			gridAttachments.ListGridRows.Clear();
			gridAttachments.ListGridColumns.Clear();
			gridAttachments.ListGridColumns.Add(new GridColumn("",90){ IsWidthDynamic=true });//No name column, since there is only one column.
			foreach(EmailAttach attachment in _listEmailAttachDisplayed) {
				GridRow row=new GridRow();
				row.Cells.Add(attachment.DisplayedFileName);
				gridAttachments.ListGridRows.Add(row);
			}
			gridAttachments.EndUpdate();
			if(gridAttachments.ListGridRows.Count>0) {
				gridAttachments.SetSelected(0,true);
			}
		}
		
		private void butAttach_Click(object sender,EventArgs e) {
			_listEmailAttachDisplayed.AddRange(EmailAttachL.PickAttachments(null));
			FillAttachments();
		}

		private void gridAttachments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EmailAttach attach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),attach.ActualFileName),attach.DisplayedFileName);
		}

		private void butSubjectFields_Click(object sender,EventArgs e) {
			using FormMessageReplacements FormMR=new FormMessageReplacements(
				MessageReplaceType.Appointment | MessageReplaceType.Office | MessageReplaceType.Patient | MessageReplaceType.User | MessageReplaceType.Misc);
			FormMR.IsSelectionMode=true;
			FormMR.ShowDialog();
			if(FormMR.DialogResult==DialogResult.OK) {
				textSubject.SelectedText=FormMR.Replacement;
			}
		}

		private void butBodyFields_Click(object sender,EventArgs e) {
			using FormMessageReplacements FormMR=new FormMessageReplacements(
				MessageReplaceType.Appointment | MessageReplaceType.Office | MessageReplaceType.Patient | MessageReplaceType.User | MessageReplaceType.Misc);
			FormMR.IsSelectionMode=true;
			FormMR.ShowDialog();
			if(FormMR.DialogResult==DialogResult.OK) {
				textBodyText.SelectedText=FormMR.Replacement;
			}
		}

		private void gridAttachments_MouseDown(object sender,MouseEventArgs e) {
			//A right click also needs to select an items so that the context menu will work properly.
			if(e.Button==MouseButtons.Right) {
				int clickedIndex=gridAttachments.PointToRow(e.Y);
				if(clickedIndex!=-1) {
					gridAttachments.SetSelected(clickedIndex,true);
				}
			}
		}

		private void contextMenuAttachments_Popup(object sender,EventArgs e) {
			menuItemOpen.Enabled=false;
			menuItemRename.Enabled=false;
			menuItemRemove.Enabled=false;
			if(gridAttachments.SelectedIndices.Length > 0) {
				menuItemOpen.Enabled=true;
				menuItemRename.Enabled=true;
				menuItemRemove.Enabled=true;
			}
		}

		private void menuItemOpen_Click(object sender,EventArgs e) {
			EmailAttach attach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),attach.ActualFileName),attach.DisplayedFileName);
		}

		private void menuItemRename_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox(Lan.g(this,"Filename"));
			EmailAttach emailAttach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			input.textResult.Text=emailAttach.DisplayedFileName;
			input.ShowDialog();
			if(input.DialogResult!=DialogResult.OK) {
				return;
			}
			emailAttach.DisplayedFileName=input.textResult.Text;
			FillAttachments();
		}

		private void menuItemRemove_Click(object sender,EventArgs e) {
			EmailAttach emailAttach=_listEmailAttachDisplayed[gridAttachments.SelectedIndices[0]];
			_listEmailAttachDisplayed.Remove(emailAttach);
			FillAttachments();
		}

		private void butEditHtml_Click(object sender,EventArgs e) {
			//get the most current version of the "plain" text from the emailPreview text box.
			using FormEmailEdit formEE=new FormEmailEdit();
			formEE.MarkupText=textBodyText.Text;//Copy existing text in case user decided to compose HTML after starting their email.
			formEE.IsRaw=_isRaw;
			formEE.ShowDialog();
			if(formEE.DialogResult!=DialogResult.OK) {
				return;
			}
			textBodyText.Text=formEE.MarkupText;
			_hasTextChanged=true;
			_isRaw=formEE.IsRaw;
			ChangeView(true);
		}

		private void butEditText_Click(object sender,EventArgs e) {
			_isRaw=false;
			ChangeView(false);
		}

		private void textBodyText_KeyDown(object sender,KeyEventArgs e) {
			_hasTextChanged=true;
		}
		private void webBrowserHtml_Navigating(object sender,WebBrowserNavigatingEventArgs e) {
			if(_isLoading) {
				return;
			}
			e.Cancel=true;//Cancel browser navigation (for links clicked within the email message).
			if(e.Url.AbsoluteUri=="about:blank") {
				return;
			}
			//if user did not specify a valid url beginning with http:// then the event args would make the url start with "about:" 
			//ex: about:www.google.com and then would ask the user to get a separate app to open the link since it is unrecognized
			string url=e.Url.ToString();
			if(url.StartsWith("about")) {
				url=url.Replace("about:","http://");
			}
			Process.Start(url);//Instead launch the URL into a new default browser window.
		}

		private void webBrowserHtml_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			_isLoading=false;
		}

		///<summary>Refreshes the email preview pane to show the web browser when viewing HTML and the editable text if not.</summary>
		public void ChangeView(bool isHtml) {
			if(_hasTextChanged) {
				//plain text box changed, grab the new plain text string and translate into html, regardless of if this is currently an html message or not.
				try {
					string htmlText="";
					if(_isRaw) {
						htmlText=textBodyText.Text;
						_htmlDocument=htmlText;
					}
					else {
						//handle aggregation of the full document text with the template ourselves so we can display properly but only save the html string. 
						htmlText=MarkupEdit.TranslateToXhtml(textBodyText.Text,false,false,true,false);
						_htmlDocument=PrefC.GetString(PrefName.EmailMasterTemplate).Replace("@@@body@@@",htmlText);
					}
					_hasTextChanged=false;
				}
				catch(Exception ex) {
					FriendlyException.Show("Error loading HTML.",ex);
				}
			}
			if(isHtml || _isRaw) {
				try {
					textBodyText.Visible=false;
					webBrowserHtml.Visible=true;
					webBrowserHtml.DocumentText=_htmlDocument;
					webBrowserHtml.BringToFront();
				}
				catch(Exception ex) {
					ex.DoNothing();
					//invalid preview
				}
			}
			else {
				//load the plain text
				webBrowserHtml.Visible=false;
				textBodyText.Visible=true;
				textBodyText.BringToFront();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textSubject.Text=="" && textBodyText.Text==""){
				MsgBox.Show(this,"Both the subject and body of the template cannot be left blank.");
				return;
			}
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			ETcur.Subject=textSubject.Text;
			ETcur.BodyText=textBodyText.Text;//always save as plain text version. We will translate to html on loading. 
			ETcur.Description=textDescription.Text;
			if(!webBrowserHtml.Visible) {
				ETcur.TemplateType=EmailType.Regular;
			}
			else {
				ETcur.TemplateType=EmailType.Html;
				if(_isRaw) {
					ETcur.TemplateType=EmailType.RawHtml;
				}
			}
			if(IsNew){
				EmailTemplates.Insert(ETcur);
			}
			else{
				EmailTemplates.Update(ETcur);
			}
			foreach(EmailAttach attachment in _listEmailAttachDisplayed) {
				attachment.EmailTemplateNum=ETcur.EmailTemplateNum;
			}
			//Sync the email attachments and pass in an emailMessageNum of 0 because we will be providing _listEmailAttachOld.
			EmailAttaches.Sync(0,_listEmailAttachDisplayed,_listEmailAttachOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















