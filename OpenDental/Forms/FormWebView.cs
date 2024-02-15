using CodeBase;
using OpenDental.UI;
using System;
using System.Windows.Forms;
using System.Text;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace OpenDental {
	///<summary>A wrapper form to display an Internet browser within an Open Dental window.
	///This is essentially a Microsoft Internet Explorer control embedded into our form.
	///The browser.ScriptErrorsSuppressed is true in order to prevent javascript error popups from annoying the user.
	///When links are clicked inside this browser, the link is followed directly inside the browser instead of opening a new window.</summary>
	public partial class FormWebView:FormODBase {
		///<summary>Set to the value passed into the constructor if one was passed in.  Navigates the browser control to this url on load.</summary>
		public string UrlBrowseTo="";
		///<summary>Set to the post data to be sent to the browser.</summary>
		public string PostData=null;
		///<summary>Any additional headers will be sent to the browser. Will only be sent if postData is set as well.</summary>
		public string AdditionalHeaders="";
		///<summary>Decides whether or not to wrap new windows in FormWebBrowser controls, or just open them in their regular browser instance.</summary>
		public bool CanWrapNewWindow;
		///<summary>Default to false. Determines if url is single use or not. When this is set to true, then the refresh button is not added. See LayoutToolBar() 
		///Appriss's API returns a single use URL for PHI security reasons. Hitting refresh would cause an unauthorized message to show from their server which would force offices to close and relaunch the bridge.
		///</summary>
		public bool IsUrlSingleUse;
		/// <summary>This will be a static form title that overrides the dynamic website title.</summary>
		public string Title="";
		public string HtmlContent;

		///<summary>Used when opening a new browser window via a link or to display html content.
		///If html content and url specified, url will be used and given content ignored. postData and additionalHeaders will only be used if url is 
		///passed in.</summary>
		public FormWebView(){//string url="",string htmlContent="",string postData=null,string additionalHeaders="",bool canWrapNewWindow=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//_postData=postData;
			//_additionalHeaders=additionalHeaders;
			//if(string.IsNullOrEmpty(url)) {
			//	if(!string.IsNullOrEmpty(htmlContent)) {
			//		webViewMain.NavigateToString(htmlContent);
			//	}
			//}
			//else {
			//	_urlBrowseTo=url;
			//}
			//_canWrapNewWindow=canWrapNewWindow;
		}

		private async void FormWebView_Load(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(HtmlContent)) {
				//this was moved from the ctor
				webViewMain.NavigateToString(HtmlContent);
			}
			await webViewMain.Init();
			webViewMain.Source=new Uri("https://opendental.com/");
			webViewMain.CoreWebView2InitializationCompleted+=WebViewMain_CoreWebView2InitializationCompleted;
			Cursor=Cursors.WaitCursor;//Is set back to default cursor after the document loads inside the browser.
			Application.DoEvents();//To show cursor change.
			Text=Lan.g(this,"Loading")+"...";
			LayoutToolBars();
			//throw new Exception("test");
			if(UrlBrowseTo=="") {
				Text="";//Since we are not browsing, we need to clear the title stored within the Text.
			}
			else { //Use the window as a simple web browswer when a URL is passed in.
						 //This will also change the Text to the title of the URL page browsed to.
				try {
					if(PostData==null) {
						webViewMain.CoreWebView2.Navigate(UrlBrowseTo);
					}
					else {
						byte[] byteArrayPostData=Encoding.ASCII.GetBytes(PostData);
						using MemoryStream memoryStream = new MemoryStream(byteArrayPostData);
						CoreWebView2WebResourceRequest coreWebView2WebResourceRequest=webViewMain.CoreWebView2.Environment.CreateWebResourceRequest(UrlBrowseTo,
							"POST",memoryStream,AdditionalHeaders);
						webViewMain.CoreWebView2.NavigateWithWebResourceRequest(coreWebView2WebResourceRequest);
					}
					Cursor=Cursors.Default;
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					FriendlyException.Show("Error loading window.",ex);
					return;
				}
			}
			Cursor=Cursors.Default;
		}

		private void WebViewMain_CoreWebView2InitializationCompleted(object sender,CoreWebView2InitializationCompletedEventArgs e) {
			webViewMain.CoreWebView2.DocumentTitleChanged+=WebViewMain_DocumentTitleChanged;
		}

		///<summary></summary>
		public void LayoutToolBars() {
			ToolBarMain.Buttons.Clear();
			ContextMenu contextMenuSettings = new ContextMenu();
			MenuItem menuItemClearCache = new MenuItem();
			menuItemClearCache.Name="ClearCache";
			menuItemClearCache.Text="Clear Browser Cache";
			menuItemClearCache.Click+=new System.EventHandler(this.menuItemClearCache_Click);
			contextMenuSettings.MenuItems.Add(menuItemClearCache);
			ODToolBarButton toolBarButtonSettings = new ODToolBarButton(Lan.g(this,"Settings"),-1,"","Settings");
			toolBarButtonSettings.Style=ODToolBarButtonStyle.DropDownButton;
			toolBarButtonSettings.DropDownMenu=contextMenuSettings;
			ToolBarMain.Buttons.Add(toolBarButtonSettings);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Back"),0,"","Back"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Forward"),1,"","Forward"));
			if(!IsUrlSingleUse) {
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Refresh"),-1,"","Refresh"));
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),-1,"","Print"));
			ToolBarMain.Invalidate();
		}

		private void WebViewMain_DocumentTitleChanged(object sender,object e) {
			SetTitle();
		}

		///<summary>Sets the text of the form to the browser's DocumentTitle.</summary>
		protected virtual void SetTitle() {
			Text=webViewMain.CoreWebView2.DocumentTitle;
			if(!string.IsNullOrWhiteSpace(Title)) {
				Text=Title;
			}
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
			}
		}

		private void menuItemClearCache_Click(object sender,EventArgs e) {
			webViewMain.ClearCache();
		}

	}
}