using CodeBase;
using OpenDental.UI;
using System;
using System.Windows.Forms;
using System.Text;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Threading.Tasks;

namespace OpenDental {
	///<summary>A wrapper form to display an Internet browser within an Open Dental window.
	///This is essentially a Microsoft Internet Explorer control embedded into our form.
	///The browser.ScriptErrorsSuppressed is true in order to prevent javascript error popups from annoying the user.
	///When links are clicked inside this browser, the link is followed directly inside the browser instead of opening a new window.</summary>
	public partial class FormWebView:FormODBase {
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

		///<summary>Used when opening a new browser window via a link or to display html content.
		///If html content and url specified, url will be used and given content ignored. postData and additionalHeaders will only be used if url is 
		///passed in.</summary>
		public FormWebView(string url="",string htmlContent="",string postData=null,string additionalHeaders="",bool canWrapNewWindow=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_postData=postData;
			_additionalHeaders=additionalHeaders;
			if(string.IsNullOrEmpty(url)) {
				if(!string.IsNullOrEmpty(htmlContent)) {
					webViewMain.NavigateToString(htmlContent);
				}
			}
			else {
				_urlBrowseTo=url;
			}
			_canWrapNewWindow=canWrapNewWindow;
		}

		public async Task InitCoreWebView() {
			 await webViewMain.EnsureCoreWebView2Async();
		}

		[Obsolete("Designer Only",true)]
		private FormWebView() { }

		private async void FormWebView_Load(object sender,EventArgs e) {
			try {
				webViewMain.Source=new Uri("https://opendental.com/");
				webViewMain.CoreWebView2InitializationCompleted+=WebViewMain_CoreWebView2InitializationCompleted;
				await InitCoreWebView();
				//throw new Exception("test");
				string version=CoreWebView2Environment.GetAvailableBrowserVersionString();
				if(version.IsNullOrEmpty()) {
					throw new Exception("WebView2 not available on device.");
				}
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
						webViewMain.CoreWebView2.Navigate(_urlBrowseTo);
					}
					else {
						byte[] byteArrayPostData=Encoding.ASCII.GetBytes(_postData);
						using MemoryStream stream = new MemoryStream(byteArrayPostData);
						CoreWebView2WebResourceRequest coreWebView2WebResourceRequest=webViewMain.CoreWebView2.Environment.CreateWebResourceRequest(_urlBrowseTo,
						"POST",
						stream,
						_additionalHeaders
					);
						webViewMain.CoreWebView2.NavigateWithWebResourceRequest(coreWebView2WebResourceRequest);
					}
				}
				Cursor=Cursors.Default;
			}
			catch {
				MsgBox.Show("error loading window");
				return;
			}
		}

		private void WebViewMain_CoreWebView2InitializationCompleted(object sender,CoreWebView2InitializationCompletedEventArgs e) {
			webViewMain.CoreWebView2.DocumentTitleChanged+=WebViewMain_DocumentTitleChanged;
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

		private void WebViewMain_DocumentTitleChanged(object sender,object e) {
			SetTitle();
		}

		///<summary>Sets the text of the form to the browser's DocumentTitle.</summary>
		protected virtual void SetTitle() {
			Text=webViewMain.CoreWebView2.DocumentTitle;
		}

		private void ToolBarMain_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Back":
					if(webViewMain.CanGoBack) {
						Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
						Application.DoEvents();//To show cursor change.
						Text=Lan.g(this,"Loading")+"...";
						webViewMain.GoBack();
					}
					break;
				case "Forward":
					if(webViewMain.CanGoForward) {
						Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
						Application.DoEvents();//To show cursor change.
						Text=Lan.g(this,"Loading")+"...";
						webViewMain.GoForward();
					}
					break;
				case "Refresh":
					webViewMain.Refresh();
					break;
				case "Close":
					DialogResult=DialogResult.Cancel;
					Close();//For when we launch the window in a non-modal manner.
					break;
			}
		}
	}
}