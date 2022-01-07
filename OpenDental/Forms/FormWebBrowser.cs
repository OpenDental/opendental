using CodeBase;
using OpenDental.UI;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

namespace OpenDental {
	///<summary>A wrapper form to display an Internet browser within an Open Dental window.
	///This is essentially a Microsoft Internet Explorer control embedded into our form.
	///The browser.ScriptErrorsSuppressed is true in order to prevent javascript error popups from annoying the user.
	///When links are clicked inside this browser, the link is followed directly inside the browser instead of opening a new window.</summary>
	public partial class FormWebBrowser:FormODBase {
		///<summary>Set to the value passed into the constructor if one was passed in.  Navigates the browser control to this url on load.</summary>
		private string _urlBrowseTo="";
		///<summary>Set to the post data to be sent to the browser.</summary>
		private string _postData=null;
		///<summary>Any additional headers will be sent to the browser. Will only be sent if postData is set as well.</summary>
		private string _additionalHeaders="";
		///<summary>Decides whether or not to wrap new windows in FormWebBrowser controls, or just open them in their regular browser instance.</summary>
		private bool _canWrapNewWindow;
		///<summary>Default to false. Determines if url is single use or not. When this is set to true, then the refresh button is not added. See LayoutToolBar() 
		///Appriss's API returns a single use URL for PHI security reasons. Hitting refresh would cause an unauthorized message to show from their server which would force offices to close and relaunch the bridge.
		///</summary>
		public bool IsUrlSingleUse;

		///<summary> DO NOT USE. Use FormWebView.cs instead. Used when opening a new browser window via a link or to display html content.
		///If html content and url specified, url will be used and given content ignored. postData and additionalHeaders will only be used if url is 
		///passed in.</summary>
		public FormWebBrowser(string url="",string htmlContent="",string postData=null,string additionalHeaders="",bool canWrapNewWindow=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			SHDocVw.WebBrowser axBrowser=(SHDocVw.WebBrowser)browser.ActiveXInstance;
			if(axBrowser!=null) {//This was null once during testing.  Not sure when null can happen.  Not sure if we should allow the user to continue.
				axBrowser.NewWindow2+=axBrowser_NewWindow2;
				axBrowser.NewWindow3+=axBrowser_NewWindow3;
			}
			_postData=postData;
			_additionalHeaders=additionalHeaders;
			browser.DocumentTitleChanged+=browser_DocumentTitleChanged;
			if(string.IsNullOrEmpty(url)) {
				if(!string.IsNullOrEmpty(htmlContent)) {
					browser.DocumentText=htmlContent;
				}
			}
			else {
				_urlBrowseTo=url;
			}
			_canWrapNewWindow=canWrapNewWindow;
		}

		[Obsolete("Designer Only",true)]
		private FormWebBrowser() { }

		private void FormWebBrowser_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
			Application.DoEvents();//To show cursor change.
			Text=Lan.g(this,"Loading")+"...";
			LayoutToolBars();
			if(_urlBrowseTo=="") {
				Text="";//Since we are not browsing, we need to clear the title stored within the Text.
			}
			else { //Use the window as a simple web browswer when a URL is passed in.
				//This will also change the Text to the title of the URL page browsed to.
				if(_postData==null) {
					browser.Navigate(_urlBrowseTo);
				}
				else {
					browser.Navigate(_urlBrowseTo,"",Encoding.ASCII.GetBytes(_postData),_additionalHeaders);
				}
			}
			Cursor=Cursors.Default;
		}

		///<summary></summary>
		public void LayoutToolBars() {
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Back"),0,"","Back"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Forward"),1,"","Forward"));
			if(!IsUrlSingleUse) {
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Refresh"),-1,"","Refresh"));
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),-1,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"","Close"));
			ToolBarMain.Invalidate();
		}

		///<summary>Linked up to the browser in the designer.
		///This event fires when a link is clicked within the webbrowser control which opens in a new window.
		///The browser.IsWebBrowserContextMenuEnabled is set to false to disable the popup menu that shows up when right clicking on links or images,
		///because right clicking a link and choosing to open in a new window causes this function to fire but the destination URL is unknown and thus
		///we cannot handle that situation.  Best to hide the context menu since there is little or no need for it.</summary>
		private void browser_NewWindow(object sender,CancelEventArgs e) {
			if(_canWrapNewWindow) {
				CreateNewWindow(browser.StatusText);//This is the URL of the page that is supposed to open in a new window.
				e.Cancel=true;//Cancel Internet Explorer from launching.
			}
		}

		///<summary>This event fires when a javascript snippet calls window.open() to open a URL in a new	browser window.
		///When window.open() is called, our browser_NewWindow() event function does not fire.</summary>
		private void axBrowser_NewWindow2(ref object ppDisp,ref bool Cancel) {
			if(_canWrapNewWindow) {
				//We could not get this event to fire in testing.  Here just in case we need it.
				CreateNewWindow(browser.StatusText);//This is the URL of the page that is supposed to open in a new window.
				Cancel=true;//Cancel Internet Explorer from launching.
			}
		}

		///<summary>We are not sure when this event function fires, but we implemented it just in case.</summary>
		void axBrowser_NewWindow3(ref object ppDisp,ref bool Cancel,uint dwFlags,string bstrUrlContext,string bstrUrl) {
			if(_canWrapNewWindow) {
				//We could not get this event to fire in testing.  Here just in case we need it.
				CreateNewWindow(bstrUrl);
				Cancel=true;//Cancel Internet Explorer from launching.
			}
		}

		///<summary>This helper function is called any time a new browser window needs to be opened.  By default, new windows launched by clicking a link
		///from within the webbrowser control will open in Internet Explorer, even if the system default is another web browser such as Mozilla.  We had a
		///problem with cookies not being carried over from our webbrowser control into Internet Explorer when a link is clicked.  To preserve cookies, we
		///intercept the new window creation, cancel it, then launch the destination URL in a new OD browser window.  Cancel the new window creation
		///inside the calling event.</summary>
		private void CreateNewWindow(string url) {
			//For example, the "ScureScripts Drug History" link within the "Compose Rx" tab for NewCrop.
			if(Regex.IsMatch(url,"^.*javascript\\:.*$",RegexOptions.IgnoreCase)) {//Ignore tab clicks because the user is not navigating to a new page.
				return;
			}
			FormWebBrowser formNew=new FormWebBrowser(url);//Open the page in a new window, but stay inside of OD.
			formNew.WindowState=FormWindowState.Normal;
			LayoutManager.Add(formNew.PanelBorders,this);
			formNew.Show();//Non-modal, so that we get the effect of opening in an independent window.
		}

		///<summary>Called after a document has finished loading, including initial page load and when Back and Forward buttons are pressed.</summary>
		public void browser_DocumentCompleted(object sender,WebBrowserDocumentCompletedEventArgs e) {
			Cursor=Cursors.Default;
			SetTitle();
		}

		private void browser_DocumentTitleChanged(object sender,EventArgs e) {
			SetTitle();
		}

		///<summary>Sets the text of the form to the browser's DocumentTitle.</summary>
		protected virtual void SetTitle() {
			Text=browser.DocumentTitle;
		}

		private void ToolBarMain_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Back":
					if(browser.CanGoBack) {
						Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
						Application.DoEvents();//To show cursor change.
						Text=Lan.g(this,"Loading")+"...";
						browser.GoBack();
					}
					break;
				case "Forward":
					if(browser.CanGoForward) {
						Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
						Application.DoEvents();//To show cursor change.
						Text=Lan.g(this,"Loading")+"...";
						browser.GoForward();
					}
					break;
				case "Refresh":
					browser.Refresh();
					break;
				case "Print":
					browser.ShowPrintDialog();
					break;
				case "Close":
					DialogResult=DialogResult.Cancel;
					Close();//For when we launch the window in a non-modal manner.
					break;
			}
		}

	}
}