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

namespace OpenDental {

	public partial class FormEServicesMobileWeb:FormODBase {
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		
		public FormEServicesMobileWeb(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesMobileWeb_Load(object sender,EventArgs e) {
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			string urlFromHQ=(
				WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut,eServiceCode.MobileWeb).FirstOrDefault()??
				new WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService() { HostedUrl="" }
			).HostedUrl;
			textHostedUrlMobileWeb.Text=urlFromHQ;
		}
	
		private void butSetupMobileWebUsers_Click(object sender,EventArgs e) {
			FormOpenDental.S_MenuItemSecurity_Click(sender,e);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}