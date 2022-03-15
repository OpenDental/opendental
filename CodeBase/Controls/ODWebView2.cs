using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

//This class wraps webview2 in order for the use to be easier, hides all of the setup. 
//This can throw exceptions if WebView2 runtime is not installed, needs to be wrapped in a try-catch for this reason.
namespace CodeBase.Controls {
	public partial class ODWebView2:WebView2 {
		public ODWebView2() {
			InitializeComponent();
		}

		public async Task Init() {
			bool doShowError=false;
			try {
				if(CoreWebView2Environment.GetAvailableBrowserVersionString().IsNullOrEmpty()) {
					doShowError=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				doShowError=true;
			}
			if(doShowError) {
				string warning ="Microsoft WebView2 is not available on this device." +
					"To use this feature, the MicroSoft WebView2 Runtime needs to be downloaded and installed on this machine.\r\n" +
					"Would you like to download the WebView2 Runtime now?";
				if(ODBuild.IsDebug()) {
					warning+="\r\nIf you are in debug, move the \"WebView2Loader.dll\" from \"RequiredDLLs\" to \"OpenDental\\Bin\\Debug\"";
				}
					if(MessageBox.Show(warning,"Error",MessageBoxButtons.YesNo)==DialogResult.Yes)
				{
						ODException.SwallowAnyException(() => Process.Start("https://go.microsoft.com/fwlink/p/?LinkId=2124703"));
				}
				throw new ODException();
			}
			// Create the cache directory 
			string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string cacheFolder = ODFileUtils.CombinePaths(localAppData, "WindowsFormsWebView2");
			CoreWebView2Environment environment=await CoreWebView2Environment.CreateAsync(null,cacheFolder);
			await this.EnsureCoreWebView2Async(environment);
		}

		public void ClearCache(bool doDisplayError = true) {
			//Fire and forget.
			_=ClearCache();
			if(doDisplayError) {
				MessageBox.Show("Browser cache cleared. You may need to close or refresh this window. If you are still experiencing problems, run the program as an Admin and try again.");
			}
		}

		///<summary>Makes a call to dev tools to clear WebView2 browser cache. Use a fire and forget pattern as you shouldn't care about the result of this method.</summary>
		private async Task ClearCache() {
			await this.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache","{}");
		}
	}
}
