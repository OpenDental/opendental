using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;

namespace OpenDental {
	public partial class FormWiki:FormODBase {
		public WikiPage WikiPageCur;		
		private List<string> historyNav;
		///<summary>Number of pages back that you are browsing. Current page == 0, Oldest page == historyNav.Length. </summary>
		private int historyNavBack;
		const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
		const int SET_FEATURE_ON_PROCESS = 0x00000002;
		[DllImport("urlmon.dll")]
		[PreserveSig]
		[return: MarshalAs(UnmanagedType.Error)]
		static extern int CoInternetSetFeatureEnabled(int FeatureEntry,[MarshalAs(UnmanagedType.U4)] int dwFlags,bool fEnable);
		private FormWikiSearch FormWS;

		public FormWiki() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWiki_Load(object sender,EventArgs e) {
			//disable the annoying clicking sound when the browser navigates
			CoInternetSetFeatureEnabled(FEATURE_DISABLE_NAVIGATION_SOUNDS,SET_FEATURE_ON_PROCESS,true);
			webBrowserWiki.StatusTextChanged += new EventHandler(WebBrowserWiki_StatusTextChanged);
			Rectangle rectangleScreen=System.Windows.Forms.Screen.GetWorkingArea(this);
			Width=LayoutManager.Scale(960);
			Height=rectangleScreen.Height;
			Left=rectangleScreen.Left+((rectangleScreen.Width-Width)/2);
			Top=rectangleScreen.Top;
			LayoutToolBar();
			historyNav=new List<string>();
			historyNavBack=0;//This is the pointer that keeps track of our position in historyNav.  0 means this is the newest page in history, a positive number is the number of pages before the newest page.
			if(Plugins.HookMethod(this,"FormWiki.FormWiki_Load_beforeHomePageLoad")) {
				goto HookSkipHomePageLoad;
			}
			LoadWikiPageHome();
			HookSkipHomePageLoad: { }
			Plugins.HookAddCode(this,"FormWiki.FormWiki_Load_end");
		}

		///<summary>Loads the user's home page or the wiki page with the title of "Home" if a custom home page has not been set before.</summary>
		private void LoadWikiPageHome() {
			historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history or add to page history
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.WikiHomePage);
			if(listUserOdPrefs.Count > 0) {
				LoadWikiPage(listUserOdPrefs[0].ValueString);
			}
			else {
				LoadWikiPage("Home");
			}
		}

		/// <summary>Because FormWikiEdit is no longer modal, this is necessary to be able to tell FormWiki to refresh when saving an edited page.</summary>
		public void RefreshPage(string pageTitle) {
			historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history or add to page history
			LoadWikiPage(pageTitle);
		}

		private void WebBrowserWiki_StatusTextChanged(object sender,EventArgs e) {
			//if(webBrowserWiki.StatusText=="") {
			//  return;
			//}
			labelStatus.Text=webBrowserWiki.StatusText;
			if(labelStatus.Text=="Done") {
				labelStatus.Text="";
			}
		}

		public void LoadWikiPagePublic(string pageTitle) {
			Application.DoEvents();//allow initialization
			navPage(pageTitle);
		}

		///<summary>Before calling this, make sure to increment/decrement the historyNavBack index to keep track of the position in history.  If loading a new page, decrement historyNavBack before calling this function.  </summary>
		private void LoadWikiPage(string pageTitle) {
			//This is called from 11 different places, any time the program needs to refresh a page from the db.
			//It's also called from the browser_Navigating event when a "wiki:" link is clicked.
			WikiPage wpage=WikiPages.GetByTitle(pageTitle);
			if(wpage==null) {
				string errorMsg="";
				if(!WikiPages.IsWikiPageTitleValid(pageTitle,out errorMsg)) {
					MsgBox.Show(this,"That page does not exist and cannot be made because the page title contains invalid characters.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"That page does not exist. Would you like to create it?")) {
					return;
				}
				Action<string> onWikiSaved=new Action<string>((pageTitleNew) => {
					//Insert the pageTitleNew where the pageTitle exists in the existing WikiPageCur 
					//(guaranteed to be unique because all INVALID WIKIPAGE LINK's have an ID at the end)
					string pageContent=WikiPages.GetWikiPageContentWithWikiPageTitles(WikiPageCur.PageContent);
					WikiPageCur.PageContent=pageContent.Replace(pageTitle,pageTitleNew);
					WikiPageCur.PageContent=WikiPages.ConvertTitlesToPageNums(WikiPageCur.PageContent);
					if(!WikiPageCur.IsDraft) {
						WikiPageCur.PageContentPlainText=MarkupEdit.ConvertToPlainText(WikiPageCur.PageContent);
					}
					WikiPages.Update(WikiPageCur);
				});
				AddWikiPage(onWikiSaved);
				return;
			}
			WikiPageCur=wpage;
			try {
				float scale=LayoutManager.ScaleMy();//Include any Zoom adjustment to increase the font size of the Wiki
				webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(WikiPageCur.PageContent,false,scale:scale);
			}
			catch(Exception ex) {
				webBrowserWiki.DocumentText="";
				MessageBox.Show(this,Lan.g(this,"This page is broken and cannot be viewed.  Error message:")+" "+ex.Message);
			}
			Text="Wiki - "+WikiPageCur.PageTitle;
			#region historyMaint
			//This region is duplicated in webBrowserWiki_Navigating() for external links.  Modifications here will need to be reflected there.
			int indexInHistory=historyNav.Count-(1+historyNavBack);//historyNavBack number of pages before the last page in history.  This is the index of the page we are loading.
			if(historyNav.Count==0) {//empty history
				historyNavBack=0;
				historyNav.Add("wiki:"+pageTitle);
			}
			else if(historyNavBack<0) {//historyNavBack could be negative here.  This means before the action that caused this load, we were not navigating through history, simply set back to 0 and add to historyNav[] if necessary.
				historyNavBack=0;
				if(historyNav[historyNav.Count-1]!="wiki:"+pageTitle) {
					historyNav.Add("wiki:"+pageTitle);
				}
			}
			else if(historyNavBack>=0 && historyNav[indexInHistory]!="wiki:"+pageTitle) {//branching from page in history
				historyNav.RemoveRange(indexInHistory,historyNavBack+1);//remove "forward" history. branching off in a new direction
				historyNavBack=0;
				historyNav.Add("wiki:"+pageTitle);
			}
			#endregion
		}

		private void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Back"),0,"","Back"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Fwd"),1,"","Forward"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Setup"),2,Lan.g(this,"Setup master page and styles."),"Setup"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ODToolBarButton buttonHome=new ODToolBarButton(Lan.g(this,"Home"),3,"","Home");
			buttonHome.Style=ODToolBarButtonStyle.DropDownButton;
			buttonHome.DropDownMenu=menuHomeDropDown; 
			ToolBarMain.Buttons.Add(buttonHome);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Edit"),4,"","Edit"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),5,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Rename"),6,"","Rename"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Archive"),7,"","Archive"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Hist"),8,"","History"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Drafts"),14,"","Drafts"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"In-Links"),9,"","Inc Links"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),10,"","Add"));
			if(DataConnection.DBtype==DatabaseType.MySql) {//not supported in oracle.
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Lists"),13,"","Lists"));
			}
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"All Pages"),11,"","All Pages"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Search"),12,"","Search"));
		}

		private void ToolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Back":
					Back_Click();
					break;
				case "Forward":
					Forward_Click();
					break;
				case "Setup":
					Setup_Click();
					break;
				case "Home":
					Home_Click();
					break;
				case "Edit":
					if(IsWikiPageLocked()) {
						return;
					}
					Edit_Click();
					break;
				case "Print":
					Print_Click();
					break;
				case "Rename":
					if(IsWikiPageLocked()) {
						return;
					}
					Rename_Click();
					break;
				case "Archive":
					if(IsWikiPageLocked()) {
						return;
					}
					Archive_Click();
					break;
				case "History":
					History_Click();
					break;
				case "Drafts":
					if(IsWikiPageLocked()) {
						return;
					}
					Drafts_Click();
					break;
				case "Inc Links":
					Inc_Link_Click();
					break;
				case "Add":
					Add_Click();
					break;
				case "Lists":
					Lists_Click();
					break;
				case "Search":
					Search_Click();
					break;
			}
		}

		///<summary>Returns a boolean whether the current wiki page is locked or not.  
		///Will show a warning message to the user if it is locked and they do not have permission to access locked pages.
		///Always returns true if WikiPageCur is null.
		///Returns false if the user currently logged in has the WikiAdmin permission.</summary>
		private bool IsWikiPageLocked() {
			if(WikiPageCur==null) {
				return true;
			}
			if(Security.IsAuthorized(Permissions.WikiAdmin,true) || !WikiPageCur.IsLocked) {
				return false;
			}
			MsgBox.Show(this,"This wiki page is locked and cannot be edited without the Wiki Admin security permission.");
			return true;
		}

		private void menuItemHomePageSave_Click(object sender,EventArgs e) {
			if(WikiPageCur==null) {
				MsgBox.Show(this,"Invalid wiki page selected.");
				return;
			}
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.WikiHomePage);
			if(listUserOdPrefs.Count > 0) {
				//User is updating their current home page to a new one.
				listUserOdPrefs[0].ValueString=WikiPageCur.PageTitle;
				UserOdPrefs.Update(listUserOdPrefs[0]);
			}
			else {
				//User is saving a custom home page for the first time.
				UserOdPref userOdPref=new UserOdPref();
				userOdPref.UserNum=Security.CurUser.UserNum;
				userOdPref.ValueString=WikiPageCur.PageTitle;
				userOdPref.FkeyType=UserOdFkeyType.WikiHomePage;
				UserOdPrefs.Insert(userOdPref);
			}
			MsgBox.Show(this,"Home page saved.");
		}

		private void Back_Click() {
			if(historyNavBack<historyNav.Count-1) {
				historyNavBack++;
			}
			NavToHistory();
			//if(historyNav.Count<2) {//should always be 1 or greater
			//  MsgBox.Show(this,"No more history");
			//  return;
			//}
			//string pageName=historyNav[historyNav.Count-2];//-1 is the last/current page.
			//if(pageName.StartsWith("wiki:")) {
			//  pageName=pageName.Substring(5);
			//  WikiPage wpage=WikiPages.GetByTitle(pageName);
			//  if(wpage==null) {
			//    MessageBox.Show("'"+historyNav[historyNav.Count-2]+"' page does not exist.");//very rare
			//    return;
			//  }
			//  historyNav.RemoveAt(historyNav.Count-1);//remove the current page from history
			//  LoadWikiPage(pageName);//because it's a duplicate, it won't add it again to the list.
			//}
			//else if(pageName.StartsWith("http://")) {//www
			//  //historyNav.RemoveAt(historyNav.Count-1);//remove the current page from history
			//  //no need to set the text because the Navigating event will fire and take care of that.
			//  webBrowserWiki.Navigate(pageName);//adds new page to history
			//}
			//else {
			//  //?
			//}
		}

		private void Forward_Click() {
			if(historyNavBack>0) {
				historyNavBack--;
			}
			NavToHistory();
		}

		///<summary>Loads page from history based on historyCurIndex.</summary> 
		private void NavToHistory() {
			if(historyNavBack<0 || historyNavBack>historyNav.Count-1) {
				//This should never happen.
				MsgBox.Show(this,"Invalid history index.");
				return;
			}
			string pageName=historyNav[historyNav.Count-(1+historyNavBack)];//-1 is the last/current page.
			if(pageName.StartsWith("wiki:")) {
				pageName=pageName.Substring(5);
				WikiPage wpage=WikiPages.GetByTitle(pageName);
				if(wpage==null) {
					MessageBox.Show("'"+historyNav[historyNav.Count-(1+historyNavBack)]+"' page does not exist.");//very rare
					return;
				}
				//historyNavBack--;//no need to decrement since this is only called from Back_Click and Forward_Click and the appropriate adjustment to this index happens there
				LoadWikiPage(pageName);//because it's a duplicate, it won't add it again to the list.
			}
			else if(pageName.StartsWith("http://")) {//www
				//no need to set the text because the Navigating event will fire and take care of that.
				webBrowserWiki.Navigate(pageName);
			}
			else {
				//?
			}
		}

		private void Setup_Click() {
			using FormWikiSetup FormWS=new FormWikiSetup();
			FormWS.ShowDialog();
			if(FormWS.DialogResult!=DialogResult.OK) {
				return;
			}
			if(WikiPageCur==null) {//if browsing the WWW
				return;
			}
			historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history or add to page history
			LoadWikiPage(WikiPageCur.PageTitle);
		}

		private void Home_Click() {
			LoadWikiPageHome();
		}

		private void Edit_Click() {
			if(Plugins.HookMethod(this,"FormWiki.Edit_Click")) {
				return;
			}
			if(WikiPageCur==null) {
				return;
			}
			if(WikiPages.GetDraftsByTitle(WikiPageCur.PageTitle).Count > 0 && MsgBox.Show(this,MsgBoxButtons.YesNo,
				"This page has one or more drafts associated with it.  Would you like to open a draft instead of the current Wiki page?"))
			{
				using FormWikiDrafts FormWD=new FormWikiDrafts();
				FormWD.OwnerForm=this;
				FormWD.ShowDialog();
				return;
			}
			FormWikiEdit formWikiEdit=new FormWikiEdit();
			formWikiEdit.WikiPageCur=WikiPageCur.Copy();
			formWikiEdit.OwnerForm=this;
			formWikiEdit.Show();
			//Maximized non-modal forms pop up on the wrong screen, as documented in FormODBase.OnLoad, so:
			Rectangle rectangleScreen=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
			//Make it half size for when user un-maximizes.
			formWikiEdit.Bounds=new Rectangle(rectangleScreen.Left+rectangleScreen.Width/4,rectangleScreen.Top+rectangleScreen.Height/4,
				rectangleScreen.Width/2,rectangleScreen.Height/2);
			formWikiEdit.WindowState=FormWindowState.Maximized;
		}

		private void Print_Click() {
			if(WikiPageCur==null) {
				return;
			}
			webBrowserWiki.ShowPrintDialog();
		}

		private void Rename_Click() {
			if(WikiPageCur==null) {
				return;
			}
			using FormWikiRename FormWR=new FormWikiRename();
			FormWR.PageTitle=WikiPageCur.PageTitle;
			FormWR.ShowDialog();
			if(FormWR.DialogResult!=DialogResult.OK) {
				return;
			}
			WikiPages.Rename(WikiPageCur,FormWR.PageTitle);
			historyNav[historyNav.Count-(1+historyNavBack)]="wiki:"+FormWR.PageTitle;//keep history updated, do not decrement historyNavBack, stay at the same index in history
			//historyNavBack--;//no need to decrement history counter since we are loading the same page, just with a different name, historyNav was edited above with new name
			LoadWikiPage(FormWR.PageTitle);
		}

		private void Archive_Click() {
			if(WikiPageCur==null) {
				return;
			}
			if(WikiPageCur.PageTitle=="Home") {
				MsgBox.Show(this,"Cannot archive homepage."); 
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Archive this wiki page?  It will still be available from the Search window if needed.")) {
				return;
			}
			WikiPages.Archive(WikiPageCur.PageTitle,Security.CurUser.UserNum);
			//historyNavBack--;//do not decrement, load will consider this a branch and put "wiki:Home" in place of the deleted page and remove "forward" history.
			LoadWikiPage("Home");
		}

		private void History_Click() {
			if(WikiPageCur==null) {
				return;
			}
			using FormWikiHistory FormWH = new FormWikiHistory();
			FormWH.PageTitleCur=WikiPageCur.PageTitle;
			FormWH.IsLocked=WikiPageCur.IsLocked;
			FormWH.ShowDialog();
			//historyNavBack--;//no need to decrement since we are loading the same page, possibly a different version, but the same PageTitle
			LoadWikiPage(FormWH.PageTitleCur);
			//if(FormWH.DialogResult!=DialogResult.OK) {
			//	return;
			//}
			//Nothing to do here.
		}

		private void Drafts_Click() {
			if(WikiPageCur==null) {
				return;
			}
			if(WikiPages.GetDraftsByTitle(WikiPageCur.PageTitle).Count==0) {
				MsgBox.Show(this,"There are no drafts for this Wiki Page.");
				return;
			}
			using FormWikiDrafts FormWD=new FormWikiDrafts();
			FormWD.OwnerForm=this;
			FormWD.ShowDialog();
		}

		private void Inc_Link_Click() {
			if(WikiPageCur==null) {
				return;
			}
			using FormWikiIncomingLinks FormWIC = new FormWikiIncomingLinks();
			FormWIC.PageTitleCur=WikiPageCur.PageTitle;
			FormWIC.ShowDialog();
			if(FormWIC.DialogResult!=DialogResult.OK) {
				return;
			}
			historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history or add to page history
			LoadWikiPage(FormWIC.JumpToPage.PageTitle);
		}

		private void Add_Click() {
			AddWikiPage();
		}

		public void AddWikiPage(Action<string> onNewPageSaved=null) {
			using FormWikiRename FormWR=new FormWikiRename();
			FormWR.ShowDialog();
			if(FormWR.DialogResult!=DialogResult.OK) {
				return;
			}
			FormWikiEdit FormWE=new FormWikiEdit(onNewPageSaved);
			FormWE.WikiPageCur=new WikiPage();
			FormWE.WikiPageCur.IsNew=true;
			FormWE.WikiPageCur.PageTitle=FormWR.PageTitle;
			FormWE.WikiPageCur.PageContent="[["+WikiPageCur.PageTitle+"]]\r\n"//link back
				+"<h1>"+FormWR.PageTitle+"</h1>\r\n";//page title
			FormWE.OwnerForm=this;
			FormWE.Show();
		}

		/*No longer used
		private void All_Pages_Click() {
			using FormWikiAllPages FormWAP=new FormWikiAllPages();
			FormWAP.ShowDialog();
			if(FormWAP.DialogResult!=DialogResult.OK) {
			  return;
			}
			historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history or add to page history
			LoadWikiPage(FormWAP.SelectedWikiPage.PageTitle);
		}*/

		private void Lists_Click() {
			using FormWikiLists FormWL=new FormWikiLists();
			FormWL.ShowDialog();
		}

		private void Search_Click() {
			//Reselect existing window if available, if not create a new instance
			if(FormWS==null || FormWS.IsDisposed) {
				FormWS=new FormWikiSearch();
				FormWS.NavToPage=navPage;
			}
			FormWS.Show();
			if(FormWS.WindowState==FormWindowState.Minimized) {//only applicable if re-using an existing instance
				FormWS.WindowState=FormWindowState.Normal;
			}
			FormWS.BringToFront();
		}

		private void navPage(string pageTitle) {
			if(String.IsNullOrEmpty(pageTitle)) {
				return;
			}
			if(this==null || this.IsDisposed) {//when used as a deligate.
				return;
			}
			historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history
			LoadWikiPage(pageTitle);
		}

		private void webBrowserWiki_Navigating(object sender,WebBrowserNavigatingEventArgs e) {
			//For debugging, we need to remember that the following happens when you click on an internal link:
			//1. This method fires. url includes 'wiki:'
			//2. This causes LoadWikiPage method to fire.  It loads the document text.
			//3. Which causes this method to fire again.  url is "about:blank".
			//This doesn't seem to be a problem.  We wrote it so that it won't go into an infinite loop, but it's something to be aware of.
			if(e.Url.ToString()=="about:blank" || e.Url.ToString().StartsWith("about:blank#")) {
				//This is a typical wiki page OR this is an internal bookmark.
				//We want to let the browser control handle the "navigation" so that it correctly loads the page OR simply auto scrolls to the div.
			}
			else if(e.Url.ToString().StartsWith("about:")){
				//All other URLs that start with about: and do not have the required "blank#" are treated as malformed URLs.
				e.Cancel=true;
				return;
			}
			else if(e.Url.ToString().StartsWith("wiki:")) {//user clicked on an internal link
				//It is invalid to have more than one space in a row in URLs.
				//When there is more than one space in a row, WebBrowserNavigatingEventArgs will convert the spaces into '&nbsp'
				//In order for our internal wiki page links to work, we need to always replace the '&nbsp' chars with spaces again.
				string wikiPageTitle=Regex.Replace(e.Url.ToString(),@"\u00A0"," ").Substring(5);
				WikiPage wikiPageDeleted=WikiPages.GetByTitle(wikiPageTitle,isDeleted:true);//Should most likely be null.
				WikiPage wpExisting;
				if(wikiPageDeleted!=null && HasExistingWikiPage(wikiPageDeleted,out wpExisting)) {
					//Now replace any references to wikiPageDeleted with the non deleted wp(wpExisting).
					WikiPages.UpdateWikiPageReferences(WikiPageCur.WikiPageNum,wikiPageDeleted.WikiPageNum,wpExisting.WikiPageNum);
					//Continue to load the page.
				}
				else if(wikiPageDeleted!=null) {
					if(MessageBox.Show(Lan.g(this,"WikiPage '")+wikiPageTitle+Lan.g(this,"' is currently archived. Would you like to restore it?"),
							"",MessageBoxButtons.OKCancel)!=DialogResult.OK) 
					{
						e.Cancel=true;
						return;
					}
					else {
						//User wants to restore the WikiPage.
						WikiPages.WikiPageRestore(wikiPageDeleted,Security.CurUser.UserNum);
					}
				}
				historyNavBack--;//We have to decrement historyNavBack to tell whether or not we need to branch our page history or add to page history
				LoadWikiPage(wikiPageTitle);
				e.Cancel=true;
				return;
			}
			else if(e.Url.ToString().Contains("wikifile:") && !ODBuild.IsWeb()) {
				string fileName=e.Url.ToString().Substring(e.Url.ToString().LastIndexOf("wikifile:")+9).Replace("/","\\");
				if(!File.Exists(fileName)) {
					MessageBox.Show(Lan.g(this,"File does not exist: ")+fileName);
					e.Cancel=true;
					return;
				}
				try {
					System.Diagnostics.Process.Start(fileName);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				e.Cancel=true;
				return;
			}
			else if(e.Url.ToString().Contains("folder:") && !ODBuild.IsWeb()) {
				string folderName=e.Url.ToString().Substring(e.Url.ToString().LastIndexOf("folder:")+7).Replace("/","\\");
				if(!Directory.Exists(folderName)) {
					MessageBox.Show(Lan.g(this,"Folder does not exist: ")+folderName);
					e.Cancel=true;
					return;
				}
				try {
					System.Diagnostics.Process.Start(folderName);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				e.Cancel=true;
				return;
			}
			else if(e.Url.ToString().Contains("wikifilecloud:")) {
				string fileName=e.Url.ToString().Substring(e.Url.ToString().LastIndexOf("wikifilecloud:")+14);
				if(!FileAtoZ.Exists(fileName)) {
					MessageBox.Show(Lan.g(this,"File does not exist: ")+fileName);
					e.Cancel=true;
					return;
				}
				try {
					FileAtoZ.StartProcess(fileName);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				e.Cancel=true;
				return;
			}
			else if(e.Url.ToString().Contains("foldercloud:")) {
				string folderName=e.Url.ToString().Substring(e.Url.ToString().LastIndexOf("foldercloud:")+12);
				if(!FileAtoZ.DirectoryExists(folderName)) {
					MessageBox.Show(Lan.g(this,"Folder does not exist: ")+folderName);
					e.Cancel=true;
					return;
				}
				try {
					FileAtoZ.OpenDirectory(folderName);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				e.Cancel=true;
				return;
			}
			else if(e.Url.ToString().StartsWith("http")) {//navigating outside of wiki by clicking a link
				try {
					System.Diagnostics.Process.Start(e.Url.ToString());
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				e.Cancel=true;//Stops the page from loading in FormWiki.
				return;
			}
		}

		private bool HasExistingWikiPage(WikiPage wikiPage,out WikiPage wpExisting) {
			wpExisting=null;
			if(wikiPage==null) {
				return false;//This shouldn't happen.
			}
			wpExisting=WikiPages.GetByTitle(wikiPage.PageTitle);
			if(wpExisting==null) {
				return false;
			}
			return true;
		}

		private void webBrowserWiki_DocumentCompleted(object sender,WebBrowserDocumentCompletedEventArgs e) {			
		}

		private void FormWiki_ResizeEnd(object sender,EventArgs e) {
			Rectangle rectangleScreen=System.Windows.Forms.Screen.GetWorkingArea(this);
			if(Height>rectangleScreen.Height){
				Height=rectangleScreen.Height;
				Top=rectangleScreen.Top;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormWiki_FormClosing(object sender,FormClosingEventArgs e) {
			if(FormWS==null || FormWS.IsDisposed) {//Close any delinquent search window that may be open.
				return;
			}
			FormWS.Close();
			FormWS.Dispose();
		}

		
	}
}