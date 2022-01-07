using OpenDentBusiness;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormHelpBrowser:FormODBase {
		private static FormHelpBrowser _formHelpBrowser;
		private static string _stableVersion;
		///<summary>Singleton pattern.</summary>
		public static FormHelpBrowser GetFormHelpBrowser(){
			if(_formHelpBrowser==null || _formHelpBrowser.IsDisposed){
				_formHelpBrowser=new FormHelpBrowser();
			}
			return _formHelpBrowser;
		}

		///<summary>Temp workaround for dpi issue. See FormHelpBrowser_CloseXClicked().</summary>
		public static void InitializeIfNull(){
			if(_formHelpBrowser!=null && !_formHelpBrowser.IsDisposed){
				return;
			}
			FormHelpBrowser formHelpBrowser=GetFormHelpBrowser();
			formHelpBrowser.Show();//Needed for the Load() event only.
			formHelpBrowser.Hide();//Immediately hide the form from user.
		}

		///<summary>Gets the latest stable version in the format of "XXX" (205,194,etc).</summary>
		private static string GetStableVersion() {
			if(_stableVersion==null) {
				_stableVersion=OpenDentalHelp.ODHelp.GetStableVersion();
			}
			return _stableVersion;
		}

		public FormHelpBrowser() {
			InitializeComponent();
			InitializeLayoutManager();
			if(!PrefC.IsODHQ || !Security.IsAuthorized(Permissions.FAQEdit,true)) {
				faqToolStripMenuItem.Visible=false;
				versionReleaseToolStripMenuItem.Visible=false;
			}
			webBrowserManual.ScriptErrorsSuppressed=true;
			webBrowserFAQ.ScriptErrorsSuppressed=true;
		}

		private void FormHelpBrowser_Load(object sender,EventArgs e) {
		}

		public void GoToPage(string manualPageUrl) {
			webBrowserManual.Navigate(manualPageUrl);
		}

		///<summary>When the web browser navigates we attempt to determine if it has navigated to a manual page. If it has,
		///we parse it and send a new request for the associated faqs. If it navigates to a page not recognized, we hide
		///the faq browser pannel.</summary>
		private void WebBrowserManual_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			ShowAndLoadFaq(e.Url.ToString());
		}

		private void WebBrowserFAQ_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			//This event gets fired for every iframe in a page to load.  
			//We only care about the webBrowserFAQ.Url because e.Url will be iframe urls in addition to the original one.
			if(webBrowserFAQ.Url.Query.Contains("results=empty")) {
				ToggleFaqPanel(true);
			}
			else {
				ToggleFaqPanel(false);
			}
		}

		///<summary>Either shows or hides the FAQ panel depending on the URL passed in.
		///If the URL is a manual page, then the panel will navigate (load) the corresponding FAQ page.</summary>
		private void ShowAndLoadFaq(string url) {
			if(!IsManualPageUrl(url)) {
				ToggleFaqPanel(true);
				return;
			}
			ToggleFaqPanel(false);
			webBrowserFAQ.Navigate(ManualUrlToFaqUrl(url));
		}

		private void ToggleFaqPanel(bool hideFaqPanel) {
			if(hideFaqPanel) {
				splitContainer1.Panel2Collapsed=hideFaqPanel;
				LayoutManager.LayoutControlBoundsAndFonts(splitContainer1);
			}
			else {
				splitContainer1.Panel2Collapsed=hideFaqPanel;
				LayoutManager.LayoutControlBoundsAndFonts(splitContainer1);
			}
		}

		///<summary>A helper method that parses the manual page url into the associated faq page url. Should Jordan ever change
		///the manual publisher's routing pattern this will break.</summary>
		private string ManualUrlToFaqUrl(string manualPageUrl) {
			string version="";
			string page="";
			//The url params for the manual is /manualversion/pagename.html
			string[] urlParams=manualPageUrl.Replace("https://www.opendental.com/","").Split('/');
			if(urlParams[0]=="manual") {//Jordan uses "manual" to signify the stable version
				version=Faqs.GetStableManualVersion().ToString();
			}
			else {//Jordan appends the version to the manual page if it is not the stable version, for example '/manual183/'
				version=urlParams[0].Replace("manual","");
			}
			page=urlParams[1].Replace(".html","");
			return $"https://opendentalsoft.com:1943/ODFaq/{page}/{version}";
		}
		
		///<summary>Helper method that tries to determine if the navigated url is a manual page. To be honest,
		///this method is just hacking apart the url and returning false as soon as it finds something that doesn't fit
		///the manual page url pattern. This will definitely have to be added upon in the future.</summary>
		private bool IsManualPageUrl(string url) {
			if(!url.StartsWith("https://www.opendental.com/manual")) {
				return false;
			}
			if(url.EndsWith("searchmanual.html")) {//The user is most likely to hit 'search' from the help browser
				return false;
			}
			return true;
		}

		private void ManageToolStripMenuItem_Click(object sender,EventArgs e) {
			string url="";
			string programVersion="";
			if(webBrowserManual!=null) {
				url=GetPageTopicFromUrl(webBrowserManual.Url.ToString());
			}
			if(!string.IsNullOrWhiteSpace(url)) {
				programVersion=FormatVersionNumber(webBrowserManual.Url.ToString());
			}
			FormFaqPicker formFaqPicker=new FormFaqPicker(url,programVersion);
			formFaqPicker.Show(this);
		}

		///<summary>
		///	Returns an empty string if the url passed in is empty or is not a manual page or if the url is formatted in a way we don't expect. (See IsManualPageUrl)
		///	Otherwise the manual page subject will be returned.
		///</summary>
		private string GetPageTopicFromUrl(string url) {
			//The url is expected to in this format: https://opendental.com/manual205/claimedit.html. We would just want the "claimedit" piece.
			if(string.IsNullOrWhiteSpace(url) || !IsManualPageUrl(url)) {
				return "";
			}
			int startIndex=url.LastIndexOf('/')+1;//we want to exclude the '/' so we go one position past the value of startIndex
			int length=(url.LastIndexOf('.')-startIndex);
			if(length<0) {
				length=0;
			}
			string retVal=url.Substring(startIndex,length);
			return retVal;
		}

		///<summary>Parses the version from the manual page url. Checks to make sure we're on a legitimate manual topic page first and returns an empty string if not.</summary>
		private string FormatVersionNumber(string url) {
			if(string.IsNullOrWhiteSpace(url) || !IsManualPageUrl(url)) {
				return "";
			}
			string retVal=new string(url.Where(x=>Char.IsDigit(x)).ToArray());
			if(string.IsNullOrWhiteSpace(retVal)) {//We're on a manual page for stable so no version number will be present. In that case, grab the latest stable from the mp database.
				retVal=GetStableVersion();
			}
			return retVal;
		}

		///<summary>Allows the user to go back in navigation of web pages if possible.</summary>
		private void BackToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!webBrowserManual.CanGoBack) {//If nothing to navigate back to in history, do nothing
				return;
			}
			Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
			Application.DoEvents();//To show cursor change.
			webBrowserManual.GoBack();
			ShowAndLoadFaq(webBrowserManual.Url.ToString());
			Cursor=Cursors.Default;
		}

		///<summary>Allows the user to go forward in navigation of web pages if possible.</summary>
		private void ForwardToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!webBrowserManual.CanGoForward) {//If nothing to navigate forward to in history, do nothing
				return;
			}
			Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
			Application.DoEvents();//To show cursor change.
			webBrowserManual.GoForward();
			ShowAndLoadFaq(webBrowserManual.Url.ToString());
			Cursor=Cursors.Default;
		}

		///<summary>Opens a small form that takes the version being released. Once the user hits OK
		///all existing Faq's (and their links) will be copied forward into the 'new' version. This is how Jordan does
		///all of the manual publisher releases. This method removes the need to do it manually.</summary>
		private void VersionReleaseToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormFaqVersionRelease FormFVR=new FormFaqVersionRelease();
			FormFVR.ShowDialog();
		}

		public void FormHelpBrowser_CloseXClicked(object sender,CancelEventArgs e) {			
			Hide();
			e.Cancel=true;
			//Form stays open and not visible.  
			//FormSheetDefEdit does not support high dpi, including all child forms.
			//This causes FormHelpBrowser to malfunction if initialized from within FormSheetDefEdit or any child.
			//So, once they get into FormSheetDefEdit, they MUST reuse the existing FormHelpBrowser.
		}

		private void toolStripFaqQuickAdd_Click(object sender,EventArgs e) {
			using FormFaqEdit formFaqEdit=new FormFaqEdit();
			formFaqEdit.IsQuickAdd=true;
			formFaqEdit.ManualPage=GetPageTopicFromUrl(webBrowserManual.Url.ToString());
			formFaqEdit.Version=FormatVersionNumber(webBrowserManual.Url.ToString());
			formFaqEdit.ShowDialog();
		}
	}
}
