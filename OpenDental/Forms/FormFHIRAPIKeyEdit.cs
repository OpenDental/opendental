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
		private APIKey _apiKeyCur;

		public bool HasChanged {
			get;
			private set;
		}

		public FormFHIRAPIKeyEdit(APIKey apiKey) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_apiKeyCur=apiKey;
		}

		private void FormFHIRAPIKeyEdit_Load(object sender,EventArgs e) {
			FillForm();
		}

		private void FillForm() {
			textKey.Text=_apiKeyCur.Key;
			textStatus.Text=Lan.g("enumFHIRAPIKeyStatus",_apiKeyCur.KeyStatus.GetDescription());
			textName.Text=_apiKeyCur.DeveloperName;
			textEmail.Text=_apiKeyCur.DeveloperEmail;
			textPhone.Text=_apiKeyCur.DeveloperPhone;
			if(_apiKeyCur.DateDisabled.Year>1880) {
				textDateDisabled.Text=_apiKeyCur.DateDisabled.ToShortDateString();
			}
			else {
				textDateDisabled.Text="";
			}
			if(!ListTools.In(_apiKeyCur.KeyStatus,FHIRKeyStatus.Enabled,FHIRKeyStatus.EnabledReadOnly,FHIRKeyStatus.DisabledByCustomer)) {
				butDisable.Enabled=false;
			}
			if(ListTools.In(_apiKeyCur.KeyStatus,FHIRKeyStatus.Enabled,FHIRKeyStatus.EnabledReadOnly)) {
				butDisable.Text=Lan.g(this,"&Disable Key");
			}
			if(_apiKeyCur.KeyStatus==FHIRKeyStatus.DisabledByCustomer) {
				butDisable.Text=Lan.g(this,"&Enable Key");
			}
		}

		private void ButDisable_Click(object sender,EventArgs e) {
			FHIRKeyStatus newStatus;
			DateTime dateDisabled;
			if(ListTools.In(_apiKeyCur.KeyStatus,FHIRKeyStatus.Enabled,FHIRKeyStatus.EnabledReadOnly)) {
				newStatus=FHIRKeyStatus.DisabledByCustomer;
				dateDisabled=DateTime.Now;
			}
			else {//DisabledByCustomer
				newStatus=FHIRKeyStatus.Enabled;
				dateDisabled=DateTime.MinValue;
			}
			string officeData=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(new List<PayloadItem> {
					new PayloadItem(textKey.Text,"APIKey"),
					new PayloadItem(newStatus.ToString(),"FHIRKeyStatus"),
				}),eServiceCode.FHIR);
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
			_apiKeyCur.KeyStatus=newStatus;
			_apiKeyCur.DateDisabled=dateDisabled;
			FillForm();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

	}
}