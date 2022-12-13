using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDental {
	public partial class FormFHIRAssignKey:FormODBase {

		public FormFHIRAssignKey() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(textKey.Text=="") {
				MsgBox.Show(this,"Please enter an API key.");
				return;
			}
			string officeData=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(textKey.Text,"APIKey"),eServiceCode.FHIR);
			string result;
			Cursor=Cursors.WaitCursor;
			try {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().AssignFHIRAPIKey(officeData);
				PayloadHelper.CheckForError(result);
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				Cursor=Cursors.Default;
				return;
			}
			string message=WebSerializer.DeserializeTag<string>(result,"Response");
			MsgBox.Show(this,message);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}

}