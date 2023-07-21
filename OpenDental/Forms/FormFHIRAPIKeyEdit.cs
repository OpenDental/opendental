using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFHIRAPIKeyEdit:FormODBase {
		private ApiKeyVisibleInfo _apiKeyPair;
		public bool HasChanged;

		public FormFHIRAPIKeyEdit(ApiKeyVisibleInfo apiKeyPair) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_apiKeyPair=apiKeyPair;
		}

		private void FormFHIRAPIKeyEdit_Load(object sender,EventArgs e) {
			FillForm();
		}

		private void FillForm() {
			textKey.Text=_apiKeyPair.CustomerKey;
			textStatus.Text=Lan.g("enumFHIRAPIKeyStatus",_apiKeyPair.FHIRKeyStatusCur.GetDescription());
			textName.Text=_apiKeyPair.DeveloperName;
			textEmail.Text=_apiKeyPair.DeveloperEmail;
			textPhone.Text=_apiKeyPair.DeveloperPhone;
			if(_apiKeyPair.DateDisabled.Year>1880) {
				textDateDisabled.Text=_apiKeyPair.DateDisabled.ToShortDateString();
			}
			else {
				textDateDisabled.Text="";
			}
			if(!_apiKeyPair.FHIRKeyStatusCur.In(FHIRKeyStatus.Enabled,FHIRKeyStatus.EnabledReadOnly,FHIRKeyStatus.DisabledByCustomer)) {
				butDisable.Enabled=false;
			}
			if(_apiKeyPair.FHIRKeyStatusCur.In(FHIRKeyStatus.Enabled,FHIRKeyStatus.EnabledReadOnly)) {
				butDisable.Text=Lan.g(this,"&Disable Key");
			}
			if(_apiKeyPair.FHIRKeyStatusCur==FHIRKeyStatus.DisabledByCustomer) {
				butDisable.Text=Lan.g(this,"&Enable Key");
			}
		}

		private void ButDisable_Click(object sender,EventArgs e) {
			FHIRKeyStatus fhirKeyStatusNew;
			DateTime dateDisabled;
			if(_apiKeyPair.FHIRKeyStatusCur.In(FHIRKeyStatus.Enabled, FHIRKeyStatus.EnabledReadOnly)) {
				fhirKeyStatusNew=FHIRKeyStatus.DisabledByCustomer;
				dateDisabled=DateTime.Now;
			}
			else {//DisabledByCustomer
				fhirKeyStatusNew=FHIRKeyStatus.Enabled;
				dateDisabled=DateTime.MinValue;
			}
			List<PayloadItem> listPayloadItems=new List<PayloadItem>();
			PayloadItem payloadItem=new PayloadItem(textKey.Text,"APIKey");
			listPayloadItems.Add(payloadItem);
			payloadItem=new PayloadItem(fhirKeyStatusNew.ToString(),"FHIRKeyStatus");
			listPayloadItems.Add(payloadItem);
			string officeData = PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(listPayloadItems),eServiceCode.FHIR);
			string result;
			try {
				Cursor=Cursors.WaitCursor;
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().UpdateFHIRKeyStatus(officeData);
				PayloadHelper.CheckForError(result);
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				Cursor=Cursors.Default;
				return;
			}
			HasChanged=true;
			Cursor=Cursors.Default;
			_apiKeyPair.FHIRKeyStatusCur=fhirKeyStatusNew;
			_apiKeyPair.DateDisabled=dateDisabled;
			FillForm();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

	}
}