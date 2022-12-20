using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {

	public partial class FormWebBrowserPrefs:FormODBase {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public string HtmlContent;
		public Point PointStart=new Point(0,0);
		public Size SizeWindow=new Size(0,0);

		///<summary>Used when opening additional details within FormPreferences.  This form will also allow links clicked and opened within this form.</summary>
		public FormWebBrowserPrefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private async void FormWebBrowserPrefs_Load(object sender,EventArgs e) {
			if(SizeWindow!=new Size(0,0)) {
				Size=new Size(LayoutManager.Scale(SizeWindow.Width),LayoutManager.Scale(SizeWindow.Height));
			}
			if(PointStart==new Point(0,0)) {
				CenterToScreen();
			}
			else {
				Location=PointStart;
			}
			if(!ODBuild.IsWeb()) {
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
			if(!string.IsNullOrEmpty(HtmlContent)) {
				int fontSize=LayoutManager.Scale(11);//Font in pixels that matches our current font size.
				string html=HtmlContent;//The content passed in.
				string documentText="<p style='font-family:Microsoft Sans Serif;font-size:"+fontSize+"px'>"+html+"</p>";
				if(!ODBuild.IsWeb()) {//Webview2 doesn't work with cloud, use a webBrowser object as a backup until Thinfinity supports webview2
					webView.Visible=true;
					ODException.SwallowAnyException(() => {
						webView.CoreWebView2.NavigateToString(documentText);
					});
				}
				else {
					webBrowser.Visible=true;
					webBrowser.DocumentText=documentText;
				}
			}
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