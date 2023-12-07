using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailEdit:FormODBase {
		///<summary></summary>
		private bool _isInvalidPreview;
		///<summary>Used to handle the webBrowser navigation events in the case a user clicks the external link in the edit window. </summary>
		private bool _isLoading;
		///<summary>The message text that will be displayed in plain text on the left side.</summary>
		public string MarkupText;
		///<summary>The full HTML text of the email.</summary>
		public string HtmlText { get;private set; }
		///<summary>When true, the img tags will contain paths to local temp files where the images are stored. If false, the images
		///need to be downloaded from the cloud.</summary>
		public bool AreImagesDownloaded { get; private set; }
		///<summary>True if the text includes the entire HTML document and is not using the master template.</summary>
		public bool IsRaw;
		///<summary>If true, will not let the user click OK if the tag [EmailDisclaimer] is not present.</summary>
		public bool DoCheckForDisclaimer;
		///<summary>Defaults to true. If false, the user cannot save this as a raw email.</summary>
		public bool IsRawAllowed=true;
		///<summary>When true, the replacement button will be visible and allow users to select field replacements.</summary>
		public bool AreReplacementsAllowed=false;
		///<summary>When true, the caller of FormEmailEdit() is the FormMassEmail() window.</summary>
		public bool IsMassEmail=false;
		///<summary>A list of email image names and urls hosted at Email Hosting. Just for temporary caching so that we do not upload the same image
		///twice while in this window.</summary>
		private List<FileNameUrl> _listFileNameUrls=new List<FileNameUrl>();
		///<summary>When true, disables some buttons that are not allowed when EmailSignature.</summary>
		public bool IsEmailSignature=false;

		public FormEmailEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailEdit_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => RefreshHTML(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textContentEmail);
			LayoutToolBars();
			textContentEmail.Text=MarkupText;//display the message text in plain text on the left side. 
			textContentEmail.Focus();
			textContentEmail.SelectionStart=0;
			textContentEmail.SelectionLength=0;
			textContentEmail.ScrollToCaret();
			checkIsRaw.Checked=IsRaw;
			//Per Nathan, discussing I2339 and E19146, this checkbox and label should not show if we would block the user from interacting with it.
			checkIsRaw.Visible=IsRawAllowed;
			labelPlainText.Visible=IsRawAllowed;
			if(IsEmailSignature) {
				checkIsRaw.Checked=true;
				checkIsRaw.Enabled=false;
			}
			RefreshHTML();
			_isLoading=true;
		}

		private void FormEmailEdit_SizeChanged(object sender,EventArgs e) {
			
		}

		private void LayoutToolBars() {
			toolBarTop.Buttons.Clear();
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Setup"),19,Lan.g(this,"Setup master page and styles."),"Setup"));
			toolBarTop.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Ext Link"),8,"","Ext Link"));
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Heading1"),9,"","H1"));
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Heading2"),10,"","H2"));
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Heading3"),11,"","H3"));
			toolBarTop.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Table"),15,"","Table"));
			toolBarTop.Buttons.Add(new ODToolBarButton(Lan.g(this,"Image"),16,"","Image"));
			//The autograph button and drop down
			ODToolBarButton toolBarButton=new ODToolBarButton(Lan.g(this,"Autograph"),-1,"","Autograph");
			toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			FillAutographDropdown();
			toolBarButton.DropDownMenu=menuAutographDropdown;
			toolBarTop.Buttons.Add(toolBarButton);
			//Create the tool bar on the bottom
			toolBarBottom.Buttons.Clear();
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Cut"),3,"","Cut"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Copy"),4,"","Copy"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Paste"),5,"","Paste"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Undo"),6,"","Undo"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Bold"),12,"","Bold"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Italic"),13,"","Italic"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Color"),14,"","Color"));
			toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Font"),17,"","Font"));
			if(AreReplacementsAllowed) {
				toolBarBottom.Buttons.Add(new ODToolBarButton(Lan.g(this,"Body Fields"),-1,"","Body Fields"));
			}
			if(IsEmailSignature) {
				toolBarTop.Buttons["Setup"].Enabled=false;
				toolBarTop.Buttons["Autograph"].Enabled=false;
				toolBarTop.Buttons["Image"].Enabled=false;
			}
		}

		private void toolBarTop_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Setup":
					Setup_Click();
					break;
				case "Ext Link":
					Ext_Link_Click();
					break;
				case "H1": 
					H1_Click(); 
					break;
				case "H2": 
					H2_Click(); 
					break;
				case "H3": 
					H3_Click(); 
					break;
				case "Table": 
					Table_Click();
					break;
				case "Image":
					Image_Click();
					break;
				case "Autograph":
					Autograph_Click();
					break;
			}
		}

		private void toolBarBottom_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Cut":
					Cut_Click();
					break;
				case "Copy":
					Copy_Click();
					break;
				case "Paste":
					Paste_Click();
					break;
				case "Undo":
					Undo_Click();
					break;
				case "Bold":
					Bold_Click();
					break;
				case "Italic":
					Italic_Click();
					break;
				case "Color":
					Color_Click();
					break;
				case "Font":
					Font_Click();
					break;
				case "Body Fields":
					BodyFields_Click();
					break;
			}
		}

		private void toolBarAutograph_MenuItemClick(object sender,EventArgs e) {
			if(sender.GetType()!=typeof(MenuItem)) {
				return;
			}
			MenuItem menuItem=(MenuItem)sender;
			if(menuItem.Tag.GetType()==typeof(EmailAutograph)) {
				EmailAutograph emailAutograph=(EmailAutograph)menuItem.Tag;
				InsertAutograph(emailAutograph);
			}
		}

		private void FillAutographDropdown() {
			menuAutographDropdown.MenuItems.Clear();
			List<EmailAutograph> listEmailAutographs=EmailAutographs.GetDeepCopy();
			for(int i=0;i<listEmailAutographs.Count;i++) {
				MenuItem menuItem=new MenuItem();
				menuItem.Tag=listEmailAutographs[i];
				menuItem.Text=listEmailAutographs[i].Description;
				menuItem.Click+=toolBarAutograph_MenuItemClick;
				menuAutographDropdown.MenuItems.Add(menuItem);
			}
		}

		private void Setup_Click() {
			FrmEmailMasterTemplate frmEmailMasterTemplate=new FrmEmailMasterTemplate();
			frmEmailMasterTemplate.ShowDialog();
			if(!frmEmailMasterTemplate.IsDialogOK) {
				return;
			}
		}

		private void Ext_Link_Click() {
			FrmExternalLink frmExternalLink=new FrmExternalLink();
			frmExternalLink.ShowDialog();
			int startPoint=textContentEmail.SelectionStart;
			if(!frmExternalLink.IsDialogOK) {
				return;
			}
			textContentEmail.SelectedText="<a href=\""+frmExternalLink.URL+"\">"+frmExternalLink.DisplayText+"</a>";
			textContentEmail.SelectionLength=0;
			if(frmExternalLink.URL=="" && frmExternalLink.DisplayText=="") {
				textContentEmail.SelectionStart=startPoint+11;
			}
			textContentEmail.Focus();
		}

		private void Bold_Click() {
			MarkupL.AddTag("<b>","</b>",textContentEmail);
		}

		private void Italic_Click() {
			MarkupL.AddTag("<i>","</i>",textContentEmail);
		}

		private void Color_Click() {
			MarkupL.AddTag("[[color:red|","]]",textContentEmail);
		}

		private void Font_Click() {
			MarkupL.AddTag("[[font:courier|","]]",textContentEmail);
		}

		private void H1_Click() {
			MarkupL.AddTag("<h1>","</h1>",textContentEmail);
		}

		private void H2_Click() {
			MarkupL.AddTag("<h2>","</h2>",textContentEmail);
		}

		private void H3_Click() {
			MarkupL.AddTag("<h3>","</h3>",textContentEmail);
		}		

		private void Table_Click() {
			int idx=textContentEmail.SelectionStart;
			if(TableOrDoubleClick(idx)) {
				return;//so it was already handled with an edit table dialog
			}
			//User did not click inside a table, so they must want to add a new table.
			using FormMarkupTableEdit formMarkupTableEdit=new FormMarkupTableEdit();
			formMarkupTableEdit.Markup=@"{|
!Width=""100""|Heading1!!Width=""100""|Heading2!!Width=""100""|Heading3
|-
|||||
|-
|||||
|}";
			formMarkupTableEdit.IsNew=true;
			formMarkupTableEdit.ShowDialog();
			if(formMarkupTableEdit.DialogResult!=DialogResult.OK){
				return;
			}
			textContentEmail.SelectionLength=0;
			textContentEmail.SelectedText=formMarkupTableEdit.Markup;
			textContentEmail.SelectionLength=0;
			textContentEmail.Focus();
		}

		private void Image_Click() {
			//Store the images in their own a-z folder and retrieve them from there.
			string folder="";
			try {
				folder=ImageStore.GetEmailImagePath();//will create the folder if it doesn't exist already. 
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Unable to find Email Images folder"),ex);
				return;
			}
			using FormImagePicker formImagePicker=new FormImagePicker(folder); 
			formImagePicker.ShowDialog();
			if(formImagePicker.DialogResult!=DialogResult.OK) {
				return;
			}
			textContentEmail.SelectionLength=0;
			string imgName=formImagePicker.ImageNameSelected;
			if(IsMassEmail) {
				try {
					imgName=GetMassEmailImageLink(formImagePicker.ImageNameSelected);
				}
				catch(Exception e) {
					FriendlyException.Show($"An error occurred: {e.Message}",e);
					return;
				}
				if(string.IsNullOrEmpty(imgName)) {//if user cancelled
					return;
				}
			}
			if(checkIsRaw.Checked) {
				textContentEmail.SelectedText=$"<img src=\"{imgName}\" />";
			}
			else {
				textContentEmail.SelectedText=$"[[img:{imgName}]]";
			}
		}

		///<summary>Attempts to get the selected image name and upload the file to the EmailHosting api. May throw exception.</summary>
		private string GetMassEmailImageLink(string selectedImgName) {
			if(string.IsNullOrEmpty(selectedImgName)) {
				throw new ApplicationException("Please select an image");
			}
			FileNameUrl fileNameUrl=_listFileNameUrls.FirstOrDefault(x=>x.FileName==selectedImgName);
			if(fileNameUrl!=null){
				return fileNameUrl.Url;
			}
			string imagePath=ImageStore.GetEmailImagePath();
			string fullPath=FileAtoZ.CombinePaths(imagePath,POut.String(selectedImgName));
			byte[] byteArray;
			if(CloudStorage.IsCloudStorage) {
				//WebBrowser needs to have a local file to open, so we download the images to temp files.	
				OpenDentalCloud.Core.TaskStateDownload taskStateDownload=CloudStorage.Download(Path.GetDirectoryName(fullPath),Path.GetFileName(fullPath));
				byteArray=taskStateDownload.FileContent;
			}
			else {
				byteArray=File.ReadAllBytes(fullPath);
			}
			IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(Clinics.ClinicNum);
			UploadS3ObjectResponse uploadS3ObjectResponse=null;
			ProgressOD progress=new ProgressOD();
			UploadS3ObjectRequest uploadS3ObjectRequest=new UploadS3ObjectRequest { 
				FileName=Path.GetFileNameWithoutExtension(fullPath),
				Extension=Path.GetExtension(fullPath),
				ObjectBytesBase64=Convert.ToBase64String(byteArray),
				ObjectPurpose=S3ObjectPurpose.MassEmailImages,
				ObjectType=S3ObjectType.Image,
			};
			progress.ActionMain=() => uploadS3ObjectResponse=iAccountApi.UploadS3Object(uploadS3ObjectRequest);
			progress.ShowDialogProgress();
			if(progress.IsCancelled) {
				return "";
			}
			_listFileNameUrls.Add(new FileNameUrl(){FileName=selectedImgName,Url=uploadS3ObjectResponse.Url });
			return uploadS3ObjectResponse.Url;
		}

		private void Autograph_Click() {
			using FormEmailAutographEdit formEmailAutographEdit=new FormEmailAutographEdit(new EmailAutograph(),isNew:true);
			if(formEmailAutographEdit.ShowDialog()==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Email);
				FillAutographDropdown();
				InsertAutograph(formEmailAutographEdit.EmailAutographCur);
			}
		}

		private void Cut_Click() {
			textContentEmail.Cut();
			textContentEmail.Focus();
		}

		private void Copy_Click() {
			textContentEmail.Copy();
			textContentEmail.Focus();
		}

		private void Paste_Click() {
			textContentEmail.Paste();
			textContentEmail.Focus();
		}

		private void Undo_Click() {
			textContentEmail.Undo();
			textContentEmail.Focus();
		}

		private void BodyFields_Click() {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Appointment);
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.User);
			listMessageReplaceTypes.Add(MessageReplaceType.Misc);
			using FormMessageReplacements formMessageReplacements=new FormMessageReplacements(listMessageReplaceTypes);
			formMessageReplacements.IsSelectionMode=true;
			if(IsMassEmail) {
				formMessageReplacements.MessageReplacementSystemType=MessageReplacementSystemType.MassEmail;
			}
			formMessageReplacements.ShowDialog();
			if(formMessageReplacements.DialogResult==DialogResult.OK) {
				//assumed this is used for the email hosting project which requires this special replacement tag. 
				textContentEmail.SelectedText=formMessageReplacements.Replacement.Replace("[","[{[{ ").Replace("]"," }]}]");
			}
		}

		private void InsertAutograph(EmailAutograph emailAutograph) {
			textContentEmail.SelectionLength=0;
			textContentEmail.SelectedText=emailAutograph.AutographText;
		}

		///<summary>This is called both when a user double clicks anywhere in the edit box, or when the click the Table button in the toolbar.  This ONLY 
		///handles popping up an edit window for an existing table.  If the cursor was not in an existing table, then this returns false.  After that, 
		///the behavior in the two areas differs.  Returns true if it popped up.</summary>
		private bool TableOrDoubleClick(int idx){
			MatchCollection matchCollection=Regex.Matches(textContentEmail.Text,@"\{\|\n.+?\n\|\}",RegexOptions.Singleline);
			//Find the table match
			Match match=matchCollection.OfType<Match>().ToList().FirstOrDefault(x=>x.Index<=idx && x.Index+x.Length>=idx);
			//handle the clicks
			if(match==null) {
				return false;//did not click inside a table
			}
			bool isLastCharacter=match.Index+match.Length==textContentEmail.Text.Length;
			textContentEmail.SelectionLength=0;//otherwise we get an annoying highlight
			//==Travis 11/20/15:  If we want to fix wiki tables in the future so duplicate tables dont both get changed from a double click, we'll need to
			//   use a regular expression to find which match of strTableLoad the user clicked on, and only replace that match below.
			using FormMarkupTableEdit formMarkupTableEdit=new FormMarkupTableEdit();
			formMarkupTableEdit.Markup=match.Value;
			formMarkupTableEdit.CountTablesInPage=matchCollection.Count;
			formMarkupTableEdit.ShowDialog();
			if(formMarkupTableEdit.DialogResult!=DialogResult.OK) {
				return true;
			}
			if(formMarkupTableEdit.Markup==null) {//indicates delete
				textContentEmail.Text=textContentEmail.Text.Remove(match.Index,match.Length);
				textContentEmail.SelectionLength=0;
				return true;
			}
			textContentEmail.Text=textContentEmail.Text.Substring(0,match.Index)//beginning of markup
				+formMarkupTableEdit.Markup//replace the table
				+(isLastCharacter ? "" : textContentEmail.Text.Substring(match.Index+match.Length));//continue to end, if any text after table markup.
			textContentEmail.SelectionLength=0;
			return true;
		}

		private void textContentEmail_KeyPress(object sender,KeyPressEventArgs e) {
			//this doesn't always fire, which is good because the user can still use the arrow keys to move around.
			//look through all tables:
			MatchCollection matchCollection = Regex.Matches(textContentEmail.Text,@"\{\|\n.+?\n\|\}",RegexOptions.Singleline);
			//MatchCollection matches = Regex.Matches(textContent.Text,
			//	@"(?<=(?:\n|^))" //Checks for preceding newline or beggining of file
			//	+@"\{\|.+?\n\|\}" //Matches the table markup.
			//	+@"(?=(?:\n|$))" //Checks for following newline or end of file
			//	,RegexOptions.Singleline);
			for(int i=0;i<matchCollection.Count;i++) {
				if(textContentEmail.SelectionStart > matchCollection[i].Index
					&& textContentEmail.SelectionStart < matchCollection[i].Index+matchCollection[i].Length) 
				{
					e.Handled=true;
					MsgBox.Show(this,"Direct editing of tables is not allowed here.  Use the table button or double click to edit.");
					return;
				}
			}
			//Tab character isn't recognized by our custom markup.  Replace all tab chars with three spaces.
			if(e.KeyChar=='\t') {
				textContentEmail.SelectedText="   ";
				e.Handled=true;
			}
		}

		private void textContentEmail_MouseDoubleClick(object sender,MouseEventArgs e) {
			int idx=textContentEmail.GetCharIndexFromPosition(e.Location);
			TableOrDoubleClick(idx);//we don't care what this returns because we don't want to do anything else.
		}

		private void webBrowserEmail_Navigating(object sender,WebBrowserNavigatingEventArgs e) {
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

		private void webBrowserEmail_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			_isLoading=false;
		}

		private void checkIsRaw_CheckedChanged(object sender,EventArgs e) {
			if(!IsRawAllowed) {
				if(checkIsRaw.Checked) {
					checkIsRaw.Checked=false;
					MsgBox.Show(this,"This feature does not support raw HTML emails.");
				}
				return;
			}
			RefreshHTML();
			bool canEnableButtons=!checkIsRaw.Checked;//some buttons should be disabled when RAW is set. 
			SetWikiToolBarItems(canEnableButtons);
		}

		private void RefreshHTML() {
			webBrowserEmail.AllowNavigation=true;
			_isLoading=true;
			try {
				var text="";
				if(checkIsRaw.Checked) {
					text=textContentEmail.Text;
				}
				else {
					text=MarkupEdit.TranslateToXhtml(textContentEmail.Text,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true);
					AreImagesDownloaded=true;
				}
				webBrowserEmail.DocumentText=text;
				_isInvalidPreview=false;
			}
			catch(Exception ex) {
				ex.DoNothing();
				_isInvalidPreview=true;
			}
		}

		/// <summary>Some toolbar items use wiki markup instead of HTML markup. Quickly set wiki markup buttons to enabled/disabled from boolean.</summary>
		private void SetWikiToolBarItems(bool isWikiMarkupEnabled) {
			toolBarTop.Buttons["Table"].Enabled=isWikiMarkupEnabled;
			//Allow them to add images for mass email raw-html templates.
			toolBarTop.Buttons["Image"].Enabled=(isWikiMarkupEnabled || IsMassEmail) && !IsEmailSignature;
			toolBarBottom.Buttons["Color"].Enabled=isWikiMarkupEnabled;
			toolBarBottom.Buttons["Font"].Enabled=isWikiMarkupEnabled;
			toolBarTop.Refresh();
			toolBarBottom.Refresh();
		}

		private void butSave_Click(object sender,EventArgs e) {
			IsRaw=checkIsRaw.Checked;
			if(!IsRaw) {//do not validate for Raw emails. User is responsible for all validation themselves. 
				if(!MarkupL.ValidateMarkup(textContentEmail,isForSaving:true,showMsgBox:true,isEmail:true)) {
					_isInvalidPreview=true;
					return;
				}
				if(_isInvalidPreview) {
					MsgBox.Show(this,"This page is in an invalid state and cannot be saved.");
					return;
				}
				if(IsMassEmail) {
					//Check to make sure they have no regular image tags [[img:image.png]]. Only allowed to have http(s) image links for mass email.
					List<string> listImageNames=MarkupEdit.GetImageNames(textContentEmail.Text);
					if(listImageNames.Any(x => !MiscUtils.IsValidHttpUri(x))) {
						MsgBox.Show(this,"All images must be hosted externally. Use the Image button.");
						return;
					}
				}
			}
			if(DoCheckForDisclaimer && PrefC.GetBool(PrefName.EmailDisclaimerIsOn) && !textContentEmail.Text.ToLower().Contains("[emaildisclaimer]")) {
				MsgBox.Show(this,"Email must contain the \"[EmailDisclaimer]\" tag.");
				return;
			}
			string errorText=PrefC.GetFirstShortURL(textContentEmail.Text);
			if(!string.IsNullOrWhiteSpace(errorText)) {
				MsgBox.Show(this,Lan.g(this,"Message cannot contain the URL")+" "+errorText+" "+Lan.g(this,"as this is only allowed for eServices."));
				return;
			}
			HtmlText=webBrowserEmail.DocumentText;
			MarkupText=textContentEmail.Text;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void FormEmailEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Cancel) {
				return;
			}
			if(_isInvalidPreview) {
				e.Cancel=true;//don't close the form if there are errors (prevents OK click)
			}
		}

		private class FileNameUrl{
			public string FileName;
			public string Url;
		}

	}
}