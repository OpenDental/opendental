using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Net;

namespace OpenDental {
	///<summary>Internet browser window for NewCrop.  This is essentially a Microsoft Internet Explorer control embedded into our form.
	///The browser.ScriptErrorsSuppressed is true in order to prevent javascript error popups from annoying the user.</summary>
	public class FormErx:FormWebBrowser {
		///<summary>The PatNum of the patient eRx was opened for.  The patient is tied to the window so that when the window is closed the Chart
		///knows which patient to refresh.  If the patient is different than the patient modified in the eRx window then the Chart does not need to 
		///refresh.</summary>
		public Patient PatCur=null;
		///<summary>This XML contains the patient information, provider information, employee information, practice information, etc...</summary>
		public byte[] PostDataBytes=new byte[1];
		public ErxOption ErxOptionCur;

		public FormErx(bool canWrapNewWindow = true) : base("","","","",canWrapNewWindow) { }

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(ErxOptionCur==ErxOption.Legacy) {
				ComposeNewRxLegacy();
			}
			else {
				ComposeNewRxDoseSpot();
			}
		}

		///<summary>Sends the ClickThroughXml to eRx and loads the result within the browser control.
		///Loads the compose tab in NewCrop's web interface.  Can be called externally to send provider information to eRx
		///without allowing the user to write any prescriptions.</summary>
		public void ComposeNewRxLegacy() {
			string additionalHeaders="Content-Type: application/x-www-form-urlencoded\r\n";
			string newCropUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.NewCropRxEntryURL,"https://secure.newcropaccounts.com/interfacev7/rxentry.aspx");
			if(ODBuild.IsDebug()) {
				newCropUrl="https://preproduction.newcropaccounts.com/interfaceV7/rxentry.aspx";
			}
			browser.Navigate(newCropUrl,"",PostDataBytes,additionalHeaders);
		}

		public void ComposeNewRxDoseSpot() {
			string additionalHeaders="Content-Type: application/x-www-form-urlencoded\r\n";
			string doseSpotUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.DoseSpotSingleSignOnURL,"https://my.dosespot.com/LoginSingleSignOn.aspx");
			if(ODBuild.IsDebug()) {
				doseSpotUrl="https://my.staging.dosespot.com/LoginSingleSignOn.aspx?b=2";
			}
			OIDExternal oidPatID=DoseSpot.GetDoseSpotPatID(PatCur.PatNum);
			if(oidPatID==null) {
				browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.GetPatientIdFromWebBrowser);
			}
			browser.Navigate(doseSpotUrl,"",PostDataBytes,additionalHeaders);
		}

		private void GetPatientIdFromWebBrowser(object sender,WebBrowserDocumentCompletedEventArgs e) {
			WebBrowser myWebBrowser=sender as WebBrowser;
			if(e.Url!=null && !string.IsNullOrEmpty(e.Url.Query)) {
				int doseSpotPatID;
				string patIdStr=DoseSpot.GetQueryParameterFromQueryString(e.Url.Query,"PatientId");
				if((!string.IsNullOrEmpty(patIdStr)) && int.TryParse(patIdStr.Trim(),out doseSpotPatID)) {
					if(doseSpotPatID != 0) {
						DoseSpot.CreateOIDForPatient(doseSpotPatID,PatCur.PatNum);
					}
				}
			}
			myWebBrowser.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.GetPatientIdFromWebBrowser);
		}

		protected override void SetTitle() {
			Text=Lan.g(this,"eRx");
			if(browser.DocumentTitle.Trim()!="") {
				Text+=" - "+browser.DocumentTitle;
			}
			if(PatCur!=null) {//Can only be null when a subwindow is opened by clicking on a link from inside another FormErx instance.
				Text+=" - "+PatCur.GetNameFL();
			}
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			ODEvent.Fire(ODEventType.ErxBrowserClosed,PatCur);
		}

	}
}