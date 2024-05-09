using OpenDental.UI;
using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Bridges;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary>Internet browser window for CareCredit. This is essentially a Microsoft Internet Explorer control embedded into our form.</summary>
	public partial class FormCareCreditWeb:FormODBase {
		///<summary>Set to the value passed into the constructor if one was passed in.  Navigates the browser control to this url on load.</summary>
		private string _urlBrowseTo="";
		///<summary>The patient CareCredit was opened for.</summary>
		private Patient _patient;
		///<summary>Default to false. Determines if url is single use or not. When this is set to true, then the refresh button is not added. See LayoutToolBar() 
		///Appriss's API returns a single use URL for PHI security reasons. Hitting refresh would cause an unauthorized message to show from their server which would force offices to close and relaunch the bridge.
		///</summary>
		public bool IsUrlSingleUse;
		///<summary>The session ID of the CareCredit prefill response.</summary>
		public string SessionId;


		///<summary>Used when opening a new browser window via a link.</summary>
		public FormCareCreditWeb(string url,Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(ODBuild.IsWeb()) {
				cloudIframe.Initialize(url);
			}
			_urlBrowseTo=url;
			_patient=patient;
			SessionId=CareCredit.GetSessionIDFromUrl(url);
		}

		private async void FormCareCreditWeb_Load(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(_urlBrowseTo) || string.IsNullOrEmpty(SessionId)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(ODBuild.IsWeb()) {
				return;//Don't use WebView2 or the toolbar in cloud
			}
			try {
				await webViewMain.Init();
			}
			catch(Exception ex){
				FriendlyException.Show("Error loading window.",ex);
				Close();
				return;
			}
			Text=Lan.g(this,"Loading")+"...";
			LayoutToolBars();
			webViewMain.CoreWebView2.Navigate(_urlBrowseTo);
		}

		///<summary></summary>
		public void LayoutToolBars() {
			ToolBarMain.Buttons.Clear();
			ContextMenu contextMenuSettings=new ContextMenu();
			MenuItem menuItemClearCache=new MenuItem();
			menuItemClearCache.Name="ClearCache";
			menuItemClearCache.Text="Clear Browser Cache";
			menuItemClearCache.Click+=new System.EventHandler(this.menuItemClearCache_Click);
			contextMenuSettings.MenuItems.Add(menuItemClearCache);
			ODToolBarButton buttonSettings=new ODToolBarButton(Lan.g(this,"Settings"),-1,"","Settings");
			buttonSettings.Style=ODToolBarButtonStyle.DropDownButton;
			buttonSettings.DropDownMenu=contextMenuSettings;
			ToolBarMain.Buttons.Add(buttonSettings);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Back"),0,"","Back"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Forward"),1,"","Forward"));
			if(!IsUrlSingleUse) {
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Refresh"),-1,"","Refresh"));
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),-1,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"","Close"));
			ToolBarMain.Invalidate();
		}

		private void webViewMain_NavigationCompleted(object sender,CoreWebView2NavigationCompletedEventArgs e) {
			SetTitle();
		}

		///<summary>Sets the text of the form to the browser's DocumentTitle.</summary>
		private void SetTitle() {
			Text=Lan.g(this,"CareCredit");
			if(_patient!=null) {//Can only be null when a subwindow is opened by clicking on a link from inside another CareCredit instance.
				Text+=" - "+_patient.GetNameFL();
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
				case "Close":
					DialogResult=DialogResult.Cancel;
					Close();//For when we launch the window in a non-modal manner.
					break;
			}
		}

		private void menuItemClearCache_Click(object sender,EventArgs e) {
			webViewMain.ClearCache();
		}

		protected override void OnClosed(EventArgs e) {
			webViewMain.Dispose();
			base.OnClosed(e);
		}
	}
}