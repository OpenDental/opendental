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
		public EmailTemplate EmailTemplateCur;
		private List<EmailAttach> _listEmailAttachesDisplayed;
		private List<EmailAttach> _listEmailAttachesOld=new List<EmailAttach>();
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
			textSubject.Text=EmailTemplateCur.Subject;
			textDescription.Text=EmailTemplateCur.Description;
			if(EmailTemplateCur.EmailTemplateNum==0) {//New email template
				_listEmailAttachesDisplayed=new List<EmailAttach>();
				EmailTemplateCur.BodyText="";
			}
			else {
				_listEmailAttachesDisplayed=EmailAttaches.GetForTemplate(EmailTemplateCur.EmailTemplateNum); 
				for(int i=0;i<_listEmailAttachesDisplayed.Count;i++) {
					_listEmailAttachesOld.Add(_listEmailAttachesDisplayed[i].Copy());
				}
			}
			textBodyText.Text=EmailTemplateCur.BodyText;
			if(EmailMessages.IsHtmlEmail(EmailTemplateCur.TemplateType)) {
				_hasTextChanged=true;
				_isRaw=(EmailTemplateCur.TemplateType==EmailType.RawHtml);
				ChangeView(isHtml:true);
			}
			FillAttachments();
		}

		private void FillAttachments() {
			gridAttachments.BeginUpdate();
			gridAttachments.ListGridRows.Clear();
			gridAttachments.Columns.Clear();
			GridColumn col=new GridColumn("",90); //No name column, since there is only one column.
			col.IsWidthDynamic=true;
			gridAttachments.Columns.Add(col);
			for(int i=0;i<_listEmailAttachesDisplayed.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(_listEmailAttachesDisplayed[i].DisplayedFileName);
				gridAttachments.ListGridRows.Add(row);
			}
			gridAttachments.EndUpdate();
			if(gridAttachments.ListGridRows.Count>0) {
				gridAttachments.SetSelected(0,true);
			}
		}
		
		private void butAttach_Click(object sender,EventArgs e) {
			_listEmailAttachesDisplayed.AddRange(EmailAttachL.PickAttachments(null));
			FillAttachments();
		}

		private void gridAttachments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EmailAttach emailAttach=_listEmailAttachesDisplayed[gridAttachments.SelectedIndices[0]];
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName),emailAttach.DisplayedFileName);
		}

		private void butSubjectFields_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Appointment);
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.User);
			listMessageReplaceTypes.Add(MessageReplaceType.Misc);
			using FormMessageReplacements formMessageReplacements=new FormMessageReplacements(listMessageReplaceTypes);
			formMessageReplacements.IsSelectionMode=true;
			formMessageReplacements.ShowDialog();
			if(formMessageReplacements.DialogResult==DialogResult.OK) {
				textSubject.SelectedText=formMessageReplacements.Replacement;
			}
		}

		private void butBodyFields_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Appointment);
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.User);
			listMessageReplaceTypes.Add(MessageReplaceType.Misc);
			using FormMessageReplacements formMessageReplacements=new FormMessageReplacements(listMessageReplaceTypes);
			formMessageReplacements.IsSelectionMode=true;
			formMessageReplacements.ShowDialog();
			if(formMessageReplacements.DialogResult==DialogResult.OK) {
				textBodyText.SelectedText=formMessageReplacements.Replacement;
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
			EmailAttach emailAttach=_listEmailAttachesDisplayed[gridAttachments.SelectedIndices[0]];
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName),emailAttach.DisplayedFileName);
		}

		private void menuItemRename_Click(object sender,EventArgs e) {
			using InputBox inputBox=new InputBox(Lan.g(this,"Filename"));
			EmailAttach emailAttach=_listEmailAttachesDisplayed[gridAttachments.SelectedIndices[0]];
			inputBox.textResult.Text=emailAttach.DisplayedFileName;
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK) {
				return;
			}
			emailAttach.DisplayedFileName=inputBox.textResult.Text;
			FillAttachments();
		}

		private void menuItemRemove_Click(object sender,EventArgs e) {
			EmailAttach emailAttach=_listEmailAttachesDisplayed[gridAttachments.SelectedIndices[0]];
			_listEmailAttachesDisplayed.Remove(emailAttach);
			FillAttachments();
		}

		private void butEditHtml_Click(object sender,EventArgs e) {
			//get the most current version of the "plain" text from the emailPreview text box.
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			formEmailEdit.MarkupText=textBodyText.Text;//Copy existing text in case user decided to compose HTML after starting their email.
			formEmailEdit.IsRaw=_isRaw;
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			textBodyText.Text=formEmailEdit.MarkupText;
			_hasTextChanged=true;
			_isRaw=formEmailEdit.IsRaw;
			ChangeView(isHtml:true);
		}

		private void butEditText_Click(object sender,EventArgs e) {
			_isRaw=false;
			ChangeView(isHtml:false);
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
						htmlText=MarkupEdit.TranslateToXhtml(textBodyText.Text,isPreviewOnly:false,hasWikiPageTitles:false,isEmail:true,canAggregate:false);
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
			EmailTemplateCur.Subject=textSubject.Text;
			EmailTemplateCur.BodyText=textBodyText.Text;//always save as plain text version. We will translate to html on loading. 
			EmailTemplateCur.Description=textDescription.Text;
			if(!webBrowserHtml.Visible) {
				EmailTemplateCur.TemplateType=EmailType.Regular;
			}
			else {
				EmailTemplateCur.TemplateType=EmailType.Html;
				if(_isRaw) {
					EmailTemplateCur.TemplateType=EmailType.RawHtml;
				}
			}
			if(IsNew){
				EmailTemplates.Insert(EmailTemplateCur);
			}
			else{
				EmailTemplates.Update(EmailTemplateCur);
			}
			for(int i=0;i<_listEmailAttachesDisplayed.Count;i++) {
				_listEmailAttachesDisplayed[i].EmailTemplateNum=EmailTemplateCur.EmailTemplateNum;
			}
			//Sync the email attachments and pass in an emailMessageNum of 0 because we will be providing _listEmailAttachOld.
			EmailAttaches.Sync(0,_listEmailAttachesDisplayed,_listEmailAttachesOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















