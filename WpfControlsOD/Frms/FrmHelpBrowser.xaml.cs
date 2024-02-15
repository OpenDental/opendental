using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |Done
   Search for "progress". Any progress bars?                         |Done
   Anything in the Tray?                                             |Yes, ImageList
   Search for "filter". Any use of SetFilterControlsAndAction?       |Done
   If yes, then STOP here. Talk to Jordan for strategy               |
-Look in the code for any references to other Forms. If those forms  |Done
   have not been converted, then STOP.  Convert those forms first.   |
-Will we include TabIndexes?  If so, up to what index?  This applies |No
   even if one single control is set so that cursor will start there |
-Grids: get familiar with properties in bold and with events.        |n/a
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|Done
-Any conversion exceptions? Consider reverting.                      |No
-In WpfControlsOD/Frms, include the new files in the project.        |Done
-Switch to using this checklist in the new Frm. Delete the other one-|Done
-Do the red areas and issues at top look fixable? Consider reverting |No, lines:63,77,83,96,131,134,141,160,227,232,270,282,286,293,312
-Does convert script need any changes instead of fixing manually?    |No
-Fix all the red areas.                                              |The above lines still have problems, but are commented out
-Address all the issues at the top. Leave in place for review.       |n/a
-Verify that the Button click events converted automatically.        |n/a
-Attach all orphaned event handlers to events in constructor.        |Partially done. Can't do for WebBrowserNavigated events
-Possibly make some labels or other controls slightly bigger due to  |n/a
   font change.                                                      |
-Change OK button to Save and get rid of Cancel button (in Edit      |n/a
   windows). Put any old Cancel button functionality into a Close    |
   event handler.                                                    |
-Change all places where the form is called to now call the new Frm. |Done. The only place it is called is within itself
-Test thoroughly                                                     |Partially done
-Are behavior and look absolutely identical? List any variation.     |Trouble getting window to come up for Frms
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
End of Checklist=========================================================================================================================
*/

	public partial class FrmHelpBrowser:FrmODBase {
		private static string _stableVersion;

		///<summary>If enableUI is set to false, then it just shows them the Help Feature page and doesn't let them click.</summary>
		public void EnableUI(bool enableUI) {
			//only intended to run once.
			if(!enableUI) {
				//Locks web browser UI on the form.
				webBrowserManual.IsEnabled=false;
				webBrowserFAQ.IsEnabled=false;
				toolBarMain.IsEnabled=false;
			}
		}

		///<summary>Gets the latest stable version in the format of "XXX" (205,194,etc).</summary>
		private static string GetStableVersion() {
			if(_stableVersion==null) {
				_stableVersion=OpenDentalHelp.ODHelp.GetStableVersion();
			}
			return _stableVersion;
		}

		public FrmHelpBrowser() {
			InitializeComponent();
			webBrowserManual.Navigated+=WebBrowserManual_Navigated;
			webBrowserFAQ.Navigated+=WebBrowserFAQ_Navigated;
			//webBrowserManual.ScriptErrorsSuppressed=true;
			//webBrowserFAQ.ScriptErrorsSuppressed=true;
			Load+=FrmHelpBrowser_Load;
		}

		private void FrmHelpBrowser_Load(object sender,EventArgs e) {
			LayoutToolBar();
		}

		public void LayoutToolBar() {
			toolBarMain.Clear();
			if(PrefC.IsODHQ && Security.IsAuthorized(EnumPermType.FAQEdit,true)) {
				ToolBarButton toolBarButtonManageFaqs=new ToolBarButton();
				toolBarButtonManageFaqs.Text=Lans.g(this,"Manage FAQ's");
				toolBarButtonManageFaqs.Click+=ManageFAQs_Click;
				toolBarMain.Add(toolBarButtonManageFaqs);
				ToolBarButton toolBarButtonAddFaqs=new ToolBarButton();
				toolBarButtonAddFaqs.Text=Lans.g(this,"Add FAQ for Current Page");
				toolBarButtonAddFaqs.Click+=AddFAQ_Click;
				toolBarMain.Add(toolBarButtonAddFaqs);
			}
			ToolBarButton toolBarButtonBack=new ToolBarButton();
			toolBarButtonBack.Text=Lans.g(this,"Back");
			toolBarButtonBack.Icon=EnumIcons.ArrowLeft;
			toolBarButtonBack.Click+=Back_Click;
			toolBarMain.Add(toolBarButtonBack);
			ToolBarButton toolBarButtonForward=new ToolBarButton();
			toolBarButtonForward.Text=Lans.g(this,"Forward");
			toolBarButtonForward.Icon=EnumIcons.ArrowRight;
			toolBarButtonForward.Click+=Forward_Click;
			toolBarMain.Add(toolBarButtonForward);
		}

		public void GoToPage(string manualPageUrl) {
			webBrowserManual.Navigate(manualPageUrl);
		}

		///<summary>When the web browser navigates we attempt to determine if it has navigated to a manual page. If it has,
		///we parse it and send a new request for the associated faqs. If it navigates to a page not recognized, we hide
		///the faq browser pannel.</summary>
		private void WebBrowserManual_Navigated(object sender,NavigationEventArgs e) {
			ShowAndLoadFaq(e.Uri.ToString());
		}

		private void WebBrowserFAQ_Navigated(object sender,NavigationEventArgs e) {
			//This event gets fired for every iframe in a page to load.  
			//We only care about the webBrowserFAQ.Url because e.Url will be iframe urls in addition to the original one.
			if(webBrowserFAQ.GetUri().Query.Contains("results=empty")) {
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
			splitContainer.SetCollapsed(splitterPanel2,doCollapse:hideFaqPanel);
		}

		///<summary>A helper method that parses the manual page url into the associated faq page url. Should Jordan ever change
		///the manual publisher's routing pattern this will break.</summary>
		private string ManualUrlToFaqUrl(string manualPageUrl) {
			string version="";
			string page="";
			//The url params for the manual is /manualversion/pagename.html
			string[] urlParams=manualPageUrl.Replace("https://www.opendental.com/","").Split('/');
			if(urlParams[0]=="manual") {//"manual" signifies the stable version
				version=Faqs.GetStableManualVersion().ToString();
			}
			else {//for example '/manual183/'
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

		private void ManageFAQs_Click(object sender,EventArgs e) {
			string url="";
			string programVersion="";
			if(webBrowserManual!=null) {
				url=GetPageTopicFromUrl(webBrowserManual.GetUri().ToString());
			}
			if(url.Contains("preferences")) {
				MsgBox.Show(this,"You cannot alter any Preference faqs. Preferences have a custom information table instead of faqs.");
				return;
			}
			if(!string.IsNullOrWhiteSpace(url)) {
				programVersion=FormatVersionNumber(webBrowserManual.GetUri().ToString());
			}
			FrmFaqPicker frmFaqPicker=new FrmFaqPicker(url,programVersion);
			frmFaqPicker.ShowDialog();
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
		private void Back_Click(object sender,EventArgs e) {
			if(!webBrowserManual.CanGoBack()) {//If nothing to navigate back to in history, do nothing
				return;
			}
			Cursor=Cursors.Wait;//Is set back to default cursor after the document loads inside the browser.
			//Application.DoEvents();//To show cursor change.
			webBrowserManual.GoBack();
			ShowAndLoadFaq(webBrowserManual.GetUri().ToString());
			Cursor=Cursors.Arrow;//Default didn't exist
		}

		///<summary>Allows the user to go forward in navigation of web pages if possible.</summary>
		private void Forward_Click(object sender,EventArgs e) {
			if(!webBrowserManual.CanGoForward()) {//If nothing to navigate forward to in history, do nothing
				return;
			}
			Cursor=Cursors.Wait;//Is set back to default cursor after the document loads inside the browser.
			//Application.DoEvents();//To show cursor change.
			webBrowserManual.GoForward();
			ShowAndLoadFaq(webBrowserManual.GetUri().ToString());
			Cursor=Cursors.Arrow;//Default didn't exist
		}

		private void AddFAQ_Click(object sender,EventArgs e) {
			string url="";
			if(webBrowserManual!=null) {
				url=webBrowserManual.GetUri().ToString();
			}
			if(url.Contains("preferences")) {
				MsgBox.Show(this,"You cannot alter any Preference faqs. Preferences have a custom information table instead of faqs.");
				return;
			}
			FrmFaqEdit frmFaqEdit=new FrmFaqEdit();
			frmFaqEdit.FaqCur=new Faq();
			frmFaqEdit.FaqCur.IsNew=true;
			frmFaqEdit.IsQuickAdd=true;
			frmFaqEdit.ManualPage=GetPageTopicFromUrl(url);
			frmFaqEdit.Version=FormatVersionNumber(url);
			frmFaqEdit.ShowDialog();
		}
	}
}
