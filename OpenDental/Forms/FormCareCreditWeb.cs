using System;
using System.Windows.Forms;
using Bridges;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCareCreditWeb:FormWebBrowser {
		private Patient _patient;
		private string _url;
		public string RefID;

		public FormCareCreditWeb(Patient patient,string url) : base() {
			_patient=patient;
			_url=url;
			RefID=CareCredit.GetRefIDFromUrl(url);
		}

		protected override void OnLoad(EventArgs e) {
			if(string.IsNullOrEmpty(_url) || string.IsNullOrEmpty(RefID)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			base.OnLoad(e);
			browser.Navigate(_url);
		}

		protected override void SetTitle() {
			Text=Lan.g(this,"CareCredit");
			if(browser.DocumentTitle.Trim()!="") {
				Text+=" - "+browser.DocumentTitle;
			}
			if(_patient!=null) {//Can only be null when a subwindow is opened by clicking on a link from inside another FormErx instance.
				Text+=" - "+_patient.GetNameFL();
			}
		}

	}
}