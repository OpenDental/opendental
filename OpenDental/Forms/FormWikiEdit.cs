using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Text.RegularExpressions;
using System.Linq;

namespace OpenDental {
	public partial class FormWikiEdit:FormODBase {
		public WikiPage WikiPageCur=new WikiPage();
		///<summary>Need a reference to the form where this was launched from so that we can tell it to refresh later.</summary>
		public FormWiki FormWiki_;
		public bool HasSaved;//used to differentiate what action caused the form to close.
		private int _scrollTop;
		private bool _isInvalidPreview;
		private Action<string> _actionWikiSaved;
		private string _exceptionMessage;

		public FormWikiEdit(Action<string> actionWikiSaved=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			WikiSaveEvent.Fired+=WikiSaveEvent_Fired;
			_actionWikiSaved=actionWikiSaved;
		}

		private void FormWikiEdit_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => RefreshHtml(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textContent);
			ResizeControls();
			//LayoutToolBar();
			Text = Lan.g(this,"Wiki Edit")+" - "+WikiPageCur.PageTitle;
			if(WikiPageCur.IsNew) {
				textContent.Text=WikiPageCur.PageContent;
			}
			else {
				textContent.Text=WikiPages.GetWikiPageContentWithWikiPageTitles(WikiPageCur.PageContent);
			}
			string[] strArray=new string[1];
			strArray[0]="\n";
			textContent.Focus();
			textContent.SelectionStart=0;
			textContent.SelectionLength=0;
			textContent.ScrollToCaret();
			RefreshHtml();
		}

		/// <summary>Because FormWikiAllPages is no longer modal, this is necessary to be able to tell FormWikiEdit to refresh with inserted data.</summary>
		public void RefreshPage(WikiPage wikiPage) {
			int tempStart=textContent.SelectionStart;
			if(wikiPage==null) {
				textContent.SelectionLength=0;
				textContent.SelectedText="[[]]";
				textContent.SelectionStart=tempStart+2;
			}
			else {
				textContent.SelectionLength=0;
				textContent.SelectedText="[["+wikiPage.PageTitle+"]]";
				textContent.SelectionStart=tempStart+wikiPage.PageTitle.Length+4;
			}
			textContent.Focus();
			textContent.SelectionLength=0;
		}

		private void RefreshHtml() {
			webBrowserWiki.AllowNavigation=true;
			//remember scroll
			if(webBrowserWiki.Document!=null) {
				_scrollTop=webBrowserWiki.Document.GetElementsByTagName("HTML")[0].ScrollTop;
			}
			try {
				webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(textContent.Text,true,true);
			}
			catch(Exception ex) {
				ex.DoNothing();
				_exceptionMessage=ex.Message;
				_isInvalidPreview=true;
				//don't refresh
				return;
			}
			_isInvalidPreview=false;
			//textContent.Focus();//this was causing a bug where it would re-highlight text after a backspace.
		}

		private void webBrowserWiki_DocumentCompleted(object sender,WebBrowserDocumentCompletedEventArgs e) {
			webBrowserWiki.Document.Body.Style = "zoom:"+(int)LayoutManager.ScaleFontODZoom(100)+"%";//100% seems to cut off a row at the bottom
			webBrowserWiki.Document.GetElementsByTagName("HTML")[0].ScrollTop=_scrollTop;
			textContent.Focus();
		}

		private void ResizeControls() {
			int top=LayoutManager.Scale(52);
			LayoutManager.Move(textContent,new Rectangle(0,top,ClientRectangle.Width/2-2,ClientRectangle.Height-top));
			LayoutManager.Move(webBrowserWiki,new Rectangle(ClientRectangle.Width/2,top,ClientRectangle.Width/2-1,ClientRectangle.Height-top));
			LayoutToolBars();
		}

		private void FormWikiEdit_SizeChanged(object sender,EventArgs e) {
			ResizeControls();
		}

		private void textContent_KeyPress(object sender,KeyPressEventArgs e) {
			//this doesn't always fire, which is good because the user can still use the arrow keys to move around.
			//look through all tables:
			MatchCollection matchCollection = Regex.Matches(textContent.Text,@"\{\|\n.+?\n\|\}",RegexOptions.Singleline);
			//MatchCollection matches = Regex.Matches(textContent.Text,
			//	@"(?<=(?:\n|^))" //Checks for preceding newline or beggining of file
			//	+@"\{\|.+?\n\|\}" //Matches the table markup.
			//	+@"(?=(?:\n|$))" //Checks for following newline or end of file
			//	,RegexOptions.Singleline);
			for(int i=0;i<matchCollection.Count;i++) {
				if(textContent.SelectionStart > matchCollection[i].Index
					&& textContent.SelectionStart < matchCollection[i].Index+matchCollection[i].Length) 
				{
					e.Handled=true;
					MsgBox.Show(this,"Direct editing of tables is not allowed here.  Use the table button or double click to edit.");
					return;
				}
			}
			//Tab character isn't recognized by our custom markup.  Replace all tab chars with three spaces.
			if(e.KeyChar=='\t') {
				textContent.SelectedText="   ";
				e.Handled=true;
			}
		}

		private void textContent_MouseDoubleClick(object sender,MouseEventArgs e) {
			int idx=textContent.GetCharIndexFromPosition(e.Location);
			TableOrDoubleClick(idx);//we don't care what this returns because we don't want to do anything else.
		}

		///<summary>This is called both when a user double clicks anywhere in the edit box, or when the click the Table button in the toolbar.  This ONLY handles popping up an edit window for an existing table.  If the cursor was not in an existing table, then this returns false.  After that, the behavior in the two areas differs.  Returns true if it popped up.</summary>
		private bool TableOrDoubleClick(int idxChar){
			//there is some code clutter in this method from when we used TableViews.  It seems harmless, but can be removed whenever.
			MatchCollection matchCollection=Regex.Matches(textContent.Text,@"\{\|\n.+?\n\|\}",RegexOptions.Singleline);
			//Tables-------------------------------------------------------------------------------
			Match match = matchCollection.OfType<Match>().ToList().FirstOrDefault(x=>x.Index<=idxChar && x.Index+x.Length>=idxChar);
			//handle the clicks----------------------------------------------------------------------------
			if(match==null) {
				return false;//did not click inside a table
			}
			bool isLastCharacter = match.Index+match.Length==textContent.Text.Length;
			textContent.SelectionLength=0;//otherwise we get an annoying highlight
			//==Travis 11/20/15:  If we want to fix wiki tables in the future so duplicate tables dont both get changed from a double click, we'll need to
			//   use a regular expression to find which match of strTableLoad the user clicked on, and only replace that match below.
			using FormMarkupTableEdit formMarkupTableEdit=new FormMarkupTableEdit();
			formMarkupTableEdit.Markup=match.Value;
			formMarkupTableEdit.CountTablesInPage=matchCollection.Count;
			formMarkupTableEdit.IsNew=false;
			formMarkupTableEdit.ShowDialog();
			if(formMarkupTableEdit.DialogResult!=DialogResult.OK) {
				return true;
			}
			if(formMarkupTableEdit.Markup==null) {//indicates delete
				textContent.Text=textContent.Text.Remove(match.Index,match.Length);
				textContent.SelectionLength=0;
				return true;
			}
			textContent.Text=textContent.Text.Substring(0,match.Index)//beginning of markup
				+formMarkupTableEdit.Markup//replace the table
				+(isLastCharacter ? "" : textContent.Text.Substring(match.Index+match.Length));//continue to end, if any text after table markup.
			textContent.SelectionLength=0;
			return true;
		}

		private void webBrowserWiki_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			webBrowserWiki.AllowNavigation=false;
		}

		private void LayoutToolBars() {
			ToolBarMain.Buttons.Clear();
			//Refresh no longer needed.
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Save"),1,"","Save"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Save as Draft"),18,"","SaveDraft"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Int Link"),7,"","Int Link"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Bookmark"),7,"","Bookmark"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"File"),7,"","File Link"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Folder"),7,"","Folder Link"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Ext Link"),8,"","Ext Link"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Heading1"),9,"","H1"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Heading2"),10,"","H2"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Heading3"),11,"","H3"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Table"),15,"","Table"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Image"),16,"","Image"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			if(Security.IsAuthorized(EnumPermType.WikiAdmin,true)) {
				if(WikiPageCur.IsLocked) {
					ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Unlock"),19,"","Lock"));
				}
				else {
					ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Lock"),20,"","Lock"));
				}
			}
			toolBar2.Buttons.Clear();
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Cut"),3,"","Cut"));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Copy"),4,"","Copy"));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Paste"),5,"","Paste"));
			toolBar2.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Undo"),6,"","Undo"));
			toolBar2.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Bold"),12,"","Bold"));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Italic"),13,"","Italic"));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Color"),14,"","Color"));
			toolBar2.Buttons.Add(new ODToolBarButton(Lan.g(this,"Font"),17,"","Font"));
			//There was a bug where if the window was resized from the bottom it would make the toolbars unclickable even though
			//Enabled for both was returning true.  By using Refresh() it has fixed the problem.
			ToolBarMain.Refresh();
			toolBar2.Refresh();
		}

		private void ToolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Save":
					Save_Click();
					break;
				case "SaveDraft":
					SaveDraft_Click();
					break;
				case "Int Link": 
					Int_Link_Click(); 
					break;
				case "Bookmark":
					Bookmark_Click();
					break;
				case "File Link":
					File_Link_Click();
					break;
				case "Folder Link":
					Folder_Link_Click();
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
				case "Lock":
					if(!Security.IsAuthorized(EnumPermType.WikiAdmin,true)) {
						return;
					}
					WikiPageCur.IsLocked=!WikiPageCur.IsLocked;
					if(WikiPageCur.IsLocked) {
						//The wiki page is was locked, switch the icon to the unlock symbol because they locked it.
						e.Button.Text=Lan.g(this,"Unlock");
						e.Button.ImageIndex=19;
					}
					else {
						//The wiki page is was unlocked, switch the icon to the lock symbol because they unlocked it.
						e.Button.Text=Lan.g(this,"Lock");
						e.Button.ImageIndex=20;
					}
					ToolBarMain.Invalidate();
					break;
			}
		}

		private void toolBar2_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
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
			}
		}

		private void menuItemCut_Click(object sender,EventArgs e) {
			Cut_Click();
		}

		private void menuItemCopy_Click(object sender,EventArgs e) {
			Copy_Click();
		}

		private void menuItemPaste_Click(object sender,EventArgs e) {
			Paste_Click();
		}

		private void menuItemUndo_Click(object sender,EventArgs e) {
			Undo_Click();
		}

		private void WikiSaveEvent_Fired(ODEventArgs e) {
			if(e.EventType==ODEventType.WikiSave) {
				if(WikiPageCur.IsNew) {
					Save_Click(showMsgBox:false);
					return;
				}
				SaveDraft_Click(showMsgBox:false);
			}
		}

		private void Save_Click(bool showMsgBox=true) {
			if(!MarkupL.ValidateMarkup(textContent,true)) {
				return;
			}
			if(_isInvalidPreview && showMsgBox) {
				MessageBox.Show(this,_exceptionMessage);
				return;
			}
			WikiPage wikiPageDb=WikiPages.GetByTitle(WikiPageCur.PageTitle);
			if(wikiPageDb!=null && WikiPageCur.DateTimeSaved<wikiPageDb.DateTimeSaved && showMsgBox) {
				if(WikiPageCur.IsDraft) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The wiki page has been edited since this draft was last saved.  Overwrite and continue?")) {
						return;
					}
				}
				else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This page has been modified and saved since it was opened on this computer.  Save anyway?")) {
					return;
				}
			}
			List<string> listInvalidWikiPages=WikiPages.GetMissingPageTitles(textContent.Text);
			string message=Lan.g(this,"This page has the following invalid wiki page link(s):")+"\r\n"+string.Join("\r\n",listInvalidWikiPages.Take(10))+"\r\n"
				+(listInvalidWikiPages.Count>10 ? "...\r\n\r\n" : "\r\n")
				+Lan.g(this,"Create the wikipage(s) automatically?");
			if(listInvalidWikiPages.Count!=0 && showMsgBox && MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
				for(int i=0;i<listInvalidWikiPages.Count;i++) {
					WikiPage wikiPage=new WikiPage();
					wikiPage.PageTitle=listInvalidWikiPages[i];
					wikiPage.UserNum=Security.CurUser.UserNum;
					wikiPage.PageContent="[["+WikiPageCur.WikiPageNum+"]]\r\n"//link back
						+"<h1>"+listInvalidWikiPages[i]+"</h1>\r\n";//page title
					WikiPages.InsertAndArchive(wikiPage);
				}
			}
			WikiPageCur.PageContent=WikiPages.ConvertTitlesToPageNums(textContent.Text);
			WikiPageCur.UserNum=Security.CurUser.UserNum;
			Regex regex=new Regex(@"\[\[(keywords:).+?\]\]");//only grab first match
			Match match=regex.Match(textContent.Text);
			WikiPageCur.KeyWords=match.Value.Replace("[[keywords:","").TrimEnd(']');//will be empty string if no match
			if(WikiPageCur.IsDraft) {
				WikiPages.DeleteDraft(WikiPageCur); //remove the draft from the database.
				WikiPageCur.IsDraft=false; //no longer a draft
				if(wikiPageDb!=null) {//wikiPageDb should never be null, otherwise there was no way to get to this draft.
					//We need to set the draft copy of the wiki page to the WikiPageNum of the real page.  
					//This is because the draft is the most up to date copy of the wiki page, and is about to be saved, however, we want to maintain any
					//links there may be to the real wiki page, since we link by WikiPageNum instead of PageTitle (see JobNum 4429 for more info)
					WikiPageCur.WikiPageNum=wikiPageDb.WikiPageNum;
				}
			}
			WikiPages.InsertAndArchive(WikiPageCur);
			//If the form was given an action, fire it.
			_actionWikiSaved?.Invoke(WikiPageCur.PageTitle);
			FormWiki formWiki=(FormWiki)this.FormWiki_;
			if(formWiki!=null && !formWiki.IsDisposed) {
				formWiki.RefreshPage(WikiPageCur.PageTitle);
			}
			HasSaved=true;
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Saves the the currently edited Wikipage as a draft. This method is copied from Save_Click with a few modifications.</summary>
		private void SaveDraft_Click(bool showMsgBox=true) {
			if(showMsgBox && WikiPageCur.IsNew) {
				MsgBox.Show(this,"You may not save a new Wiki page as a draft.  Save the Wiki page, then create a draft.");
				return;
			}
			if(!MarkupL.ValidateMarkup(textContent,true,showMsgBox)) {
				return;
			}
			if(showMsgBox && _isInvalidPreview) {
				MsgBox.Show(this,_exceptionMessage);
				return;
			}
			WikiPageCur.PageContent=WikiPages.ConvertTitlesToPageNums(textContent.Text);
			WikiPageCur.UserNum=Security.CurUser.UserNum;
			Regex regex=new Regex(@"\[\[(keywords:).+?\]\]");//only grab first match
			Match match=regex.Match(textContent.Text);
			WikiPageCur.KeyWords=match.Value.Replace("[[keywords:","").TrimEnd(']');//will be empty string if no match
			if(WikiPageCur.IsDraft) { //If it's already a draft, overwrite the current one.
				WikiPageCur.DateTimeSaved=DateTime.Now;
				try {
					WikiPages.UpdateDraft(WikiPageCur);
				}
				catch (Exception ex){
					//should never happen due to the if Draft check above.
					if(showMsgBox) {
						MessageBox.Show(ex.Message);
					}
					return;
				}
			}
			else { //otherwise, set it as a draft, then insert it.
				WikiPageCur.IsDraft=true;
				WikiPages.InsertAsDraft(WikiPageCur);
			}
			//HasSaved not set so that the user will stay in FormWikiDrafts when this window closes, causing the grid to update.
			DialogResult=DialogResult.OK;
			Close();
		}

		private void Cut_Click() {
			textContent.Cut();
			textContent.Focus();
			//RefreshHtml();
		}

		private void Copy_Click() {
			textContent.Copy();
			textContent.Focus();
			//RefreshHtml();
		}

		private void Paste_Click() {
			textContent.Paste();
			textContent.Focus();
			//RefreshHtml();
		}

		private void Undo_Click() {
			textContent.Undo();
			textContent.Focus();
			//RefreshHtml();
		}

		private void Int_Link_Click() {
			FormWikiAllPages formWikiAllPages = new FormWikiAllPages();
			formWikiAllPages.FormWikiEditOwner=this;
			formWikiAllPages.Show();
		}

		private void Bookmark_Click() {
			FrmExternalLink frmExternalLink=new FrmExternalLink(Lan.g(this,"Insert Internal Bookmark"),Lan.g(this,"ID"));
			frmExternalLink.ShowDialog();
			int tempStart=textContent.SelectionStart;
			if(!frmExternalLink.IsDialogOK) {
				return;
			}
			if(frmExternalLink.URL=="" && frmExternalLink.DisplayText=="") {
				textContent.SelectionLength=0;
				textContent.SelectedText="<a href=\"#\"></a>\n<div id=\"\"></div>";
				textContent.SelectionStart=tempStart+12;
				textContent.SelectionLength=0;
			}
			else {
				textContent.SelectionLength=0;
				textContent.SelectedText="<a href=\"#"+frmExternalLink.URL+"\">"+frmExternalLink.DisplayText+"</a>\n<div id=\""+frmExternalLink.URL+"\"></div>";
			}
			textContent.Focus();
		}

		private void File_Link_Click() {
			string fileLink;
			if(CloudStorage.IsCloudStorage) {
				using FormFilePicker formFilePicker=new FormFilePicker(WikiPages.GetWikiPath());
				formFilePicker.DoHideLocalButton=true;
				formFilePicker.ShowDialog();
				if(formFilePicker.DialogResult!=DialogResult.OK) {
					return;
				}
				fileLink=formFilePicker.ListSelectedFiles[0];
				textContent.SelectedText="[[filecloud:"+fileLink+"]]";
			}
			else {//Not cloud
				using FormWikiFileFolder formWikiFileFolder=new FormWikiFileFolder();
				formWikiFileFolder.ShowDialog();
				if(formWikiFileFolder.DialogResult!=DialogResult.OK) {
					return;
				}
				fileLink=formWikiFileFolder.LinkSelected;
				textContent.SelectedText="[[file:"+fileLink+"]]";
			}
			textContent.SelectionLength=0;
			//RefreshHtml();
		}

		private void Folder_Link_Click() {
			if(CloudStorage.IsCloudStorage) {
				using FormFilePicker formFilePicker=new FormFilePicker(CloudStorage.PathTidy(WikiPages.GetWikiPath()));
				formFilePicker.DoHideLocalButton=true;
				formFilePicker.ShowDialog();
				if(formFilePicker.DialogResult!=DialogResult.OK) {
					return;
				}
				textContent.SelectedText="[[foldercloud:"+formFilePicker.ListSelectedFiles[0]+"]]";
			}
			else {
				using FormWikiFileFolder formWikiFileFolder=new FormWikiFileFolder();
				formWikiFileFolder.IsFolderMode=true;
				formWikiFileFolder.ShowDialog();
				if(formWikiFileFolder.DialogResult!=DialogResult.OK) {
					return;
				}
				textContent.SelectedText="[[folder:"+formWikiFileFolder.LinkSelected+"]]";
			}
			textContent.SelectionLength=0;
			//RefreshHtml();
		}

		private void Ext_Link_Click() {
			FrmExternalLink frmExternalLink=new FrmExternalLink();
			frmExternalLink.ShowDialog();
			int tempStart=textContent.SelectionStart;
			if(!frmExternalLink.IsDialogOK) {
				return;
			}
			if(frmExternalLink.URL=="" && frmExternalLink.DisplayText=="") {
				textContent.SelectionLength=0;
				textContent.SelectedText="<a href=\"\"></a>";
				textContent.SelectionStart=tempStart+11;
				textContent.SelectionLength=0;
			}
			else {
				textContent.SelectionLength=0;
				textContent.SelectedText="<a href=\""+frmExternalLink.URL+"\">"+frmExternalLink.DisplayText+"</a>";
			}
			textContent.Focus();
		}

		private void H1_Click() {
			MarkupL.AddTag("<h1>","</h1>",textContent);
		}

		private void H2_Click() {
			MarkupL.AddTag("<h2>","</h2>",textContent);
		}

		private void H3_Click() {
			MarkupL.AddTag("<h3>","</h3>",textContent);
		}

		private void Bold_Click() {
			MarkupL.AddTag("<b>","</b>",textContent);
		}

		private void Italic_Click() {
			MarkupL.AddTag("<i>","</i>",textContent);
		}

		private void Color_Click() {
			MarkupL.AddTag("[[color:red|","]]",textContent);
		}

		private void Font_Click() {
			MarkupL.AddTag("[[font:courier|","]]",textContent);
		}

		///<summary>Works for a new table and for an existing table.</summary>
		private void Table_Click() {
			int idx=textContent.SelectionStart;
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
			textContent.SelectionLength=0;
			textContent.SelectedText=formMarkupTableEdit.Markup;
			textContent.SelectionLength=0;
			textContent.Focus();
		}

		private void Image_Click() {
			//if storing images in database, GetWikiPath will throw an exception, cannot be storing in database to use images in the wiki
			string wikiPath="";
			try {
				wikiPath=WikiPages.GetWikiPath();
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
				return;
			}
			using FormImagePicker formImagePicker=new FormImagePicker(wikiPath);
			formImagePicker.ShowDialog();
			if(formImagePicker.DialogResult!=DialogResult.OK) {
				return;
			}
			textContent.SelectionLength=0;
			textContent.SelectedText="[[img:"+formImagePicker.ImageNameSelected+"]]";
			//webBrowserWiki.AllowNavigation=true;
			//RefreshHtml();
		}

		private void FormWikiEdit_FormClosing(object sender,FormClosingEventArgs e) {
			//handles both the Cancel button and the user clicking on the x, and also the save button.
			WikiSaveEvent.Fired-=WikiSaveEvent_Fired;
			if(HasSaved) {
				return;
			}
			if(!WikiPageCur.IsNew && textContent.Text!=WikiPages.GetWikiPageContentWithWikiPageTitles(WikiPageCur.PageContent)){
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Unsaved changes will be lost. Would you like to continue?")) {
					e.Cancel=true;
				}
			}
		}
	}
}