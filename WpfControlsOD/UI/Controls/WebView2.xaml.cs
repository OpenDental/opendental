using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeBase;
using Microsoft.Web.WebView2.Core;

namespace WpfControls.UI {
/*
	-Use the below try/catch to intialize CoreWebView2
	-After this you can call Navigate()
	-Run in x86 for debug, otherwise an exception will occur
	private async void InitializeAndNavigate() {
		try {
			await webView2.Init();
		}
		catch(Exception ex) {
			FriendlyException.Show("Error loading window.",ex);
			Close();
			return;
		}
		webView2.Navigate("https://www.opendental.com/");
	}
	-WebView2 must be disposed correctly or it will throw a System.InvalidOperationException.
	-Call the webView2.Dispose() method on FormClosing:
	private void FrmWhatever_FormClosing(object sender,CancelEventArgs e) {
		webView2?.Dispose();
	}
*/

	public partial class WebView2:UserControl {

		public WebView2() {
			InitializeComponent();
			webView2.NavigationStarting+=WebView2_NavigationStarting;
			webView2.NavigationCompleted+=WebView2_NavigationCompleted;
		}

		public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;

		public async Task Init() {
			//Was like below by default, which didn't work because of file permissions in that folder.
			//string userDataFolder= "C:\\Program Files (x86)\\OpenDental\\OpenDental.exe.WebView2";
			//New location is like this:
			//C:\\Users\\User\\AppData\\Local\\Temp\\opendental
			string userDataFolder=OpenDentBusiness.PrefC.GetTempFolderPath();
			CoreWebView2Environment webView2Environment=await CoreWebView2Environment.CreateAsync(null,userDataFolder,null);
			await webView2.EnsureCoreWebView2Async(webView2Environment);
		}

		private void WebView2_NavigationStarting(object sender,CoreWebView2NavigationStartingEventArgs e) {
			if(!webView2.IsLoaded) {
				return;
			}
			if(webView2.IsEnabled) {
				return;
			}
			e.Cancel=true;
		}

		public bool CanGoBack() {
			return webView2.CanGoBack;
		}

		public bool CanGoForward() {
			return webView2.CanGoForward;
		}

		public Uri GetUri() {
			return webView2.Source;
		}

		public void GoBack() {
			webView2.GoBack();
		}

		public void GoForward() {
			webView2.GoForward();
		}

		public void Navigate(string url) {
			bool isEnabled=webView2.IsEnabled;
			if(!isEnabled) {
				webView2.IsEnabled=true;
			}
			webView2.CoreWebView2.Navigate(url);
			if(!isEnabled) {
				webView2.IsEnabled=false;
			}
		}

		public void NavigateToString(string text) {
			webView2.NavigateToString(text);
		}

		private void WebView2_NavigationCompleted(object sender,CoreWebView2NavigationCompletedEventArgs e) {
			NavigationCompleted?.Invoke(sender,e);
			Microsoft.Web.WebView2.Wpf.WebView2 webView2=sender as Microsoft.Web.WebView2.Wpf.WebView2;
			if(webView2!=null) {
				dynamic activeX=webView2.GetType().InvokeMember("ActiveXInstance",
						BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
						binder:null,webView2,new object[] { });
				activeX.Silent=true;
			}
		}

		public void Dispose() {
			webView2?.Dispose();
		}
	}
}

