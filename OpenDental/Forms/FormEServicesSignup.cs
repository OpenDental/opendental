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
				await webViewMain.Init();
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