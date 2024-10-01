using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {

	public partial class FormWebBrowserPrefs:FormODBase {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public string HtmlContent;
		///<summary>Set this to the location where the window should start.  If this makes it fall outside the screen, it will be fixed automatically.</summary>
		public Point PointStart=new Point(0,0);
		///<summary>Default size is 591 x 363. If your info is too big and causes scrolling, then increase the size of the window.</summary>
		public Size SizeWindow=new Size(0,0);

		///<summary>Used when opening additional details within FormPreferences.  This form will also allow links clicked and opened within this form.</summary>
		public FormWebBrowserPrefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private async void FormWebBrowserPrefs_Load(object sender,EventArgs e) {
			if(SizeWindow!=new Size(0,0)) {
				Size=new Size(LayoutManager.Scale(SizeWindow.Width),LayoutManager.Scale(SizeWindow.Height));
			}
			if(PointStart==new Point(0,0)) {
				CenterToScreen();
			}
			else {
				Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;
				int x=PointStart.X;
				if(x+Width>rectangleWorkingArea.Right-10){
					x=rectangleWorkingArea.Right-Width-10;
				}
				int y=PointStart.Y;
				if(y+Height>rectangleWorkingArea.Bottom-10){
					y=rectangleWorkingArea.Bottom-Height-10;
				}
				Location=new Point(x,y);
			}
			if(!ODBuild.IsThinfinity()) {
				try {
					await webView.Init();
					webView.CoreWebView2.NewWindowRequested+=CoreWebView2_NewWindowRequested;
				}
				catch(Exception ex) {
					ex.DoNothing();
					DialogResult=DialogResult.Cancel;
					Close();
					return;
				}
			}
			if(string.IsNullOrEmpty(HtmlContent)) {
				return;
			}
			if(ODBuild.IsThinfinity()) {//Webview2 doesn't work with cloud, use a webBrowser object as a backup until Thinfinity supports webview2
				webBrowser.Visible=true;
				webBrowser.DocumentText=HtmlContent;
				return;
			}
			webView.Visible=true;
			ODException.SwallowAnyException(() => {
				webView.CoreWebView2.NavigateToString(HtmlContent);
			});
			
		}

		private void CoreWebView2_NewWindowRequested(object sender,Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e) {
			//Stop the default Microsoft Edge add-on from launching a new window when a user clicks on a hyperlink
			//The normal window from control CoreWebView2 does not allow the user to use their default browser and does not allow navigation
			e.Handled=true;
			try {
				Process.Start(e.Uri);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+e.Uri+"\r\n"+Lan.g(this,"Please set up a default web browser."));
			}
		}
	}
}