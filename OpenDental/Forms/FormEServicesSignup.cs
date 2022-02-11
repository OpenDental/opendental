using CodeBase;
using Microsoft.Win32;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Linq;
using System.IO;
using WebServiceSerializer;
using OpenDentBusiness.WebServiceMainHQ;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;
using Microsoft.Web.WebView2.Core;

namespace OpenDental {

	public partial class FormEServicesSignup:FormODBase {
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		
		public FormEServicesSignup(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private async void FormEServicesSignup_Load(object sender,EventArgs e) {
			try {
				await Init();
			}
			catch(Exception ex) {
				ex.DoNothing();
				DialogResult=DialogResult.Cancel;
				Close();
			}
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			if(ODBuild.IsWeb()) {
				UIHelper.ForceBringToFront(this);
				Process.Start(_signupOut.SignupPortalUrl);
				DialogResult=DialogResult.Abort;
				return;
			}
			ODException.SwallowAnyException(() => {
			if(ODBuild.IsDebug()) {
				_signupOut.SignupPortalUrl=_signupOut.SignupPortalUrl.Replace("https://www.patientviewer.com/SignupPortal/GWT/SignupPortal/SignupPortal.html","http://127.0.0.1:8888/SignupPortal.html");
			}
				webViewMain.CoreWebView2.Navigate(_signupOut.SignupPortalUrl);
			});
		}

		private async System.Threading.Tasks.Task Init() {
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
			await webViewMain.EnsureCoreWebView2Async(environment);
		}

		private void webViewMain_NavigationCompleted(object sender,CoreWebView2NavigationCompletedEventArgs e) {
			SetTitle();
		}

		private void SetTitle() {
			Text=Lan.g(this,"eServices");
			if(webViewMain.CoreWebView2!=null && !string.IsNullOrWhiteSpace(webViewMain.CoreWebView2.DocumentTitle)) {
				Text+=" - "+webViewMain.CoreWebView2.DocumentTitle;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}