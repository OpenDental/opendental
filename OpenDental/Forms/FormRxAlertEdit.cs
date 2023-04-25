using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormRxAlertEdit:FormODBase {
		private RxAlert _rxAlert;
		private RxDef _rxDef;

		public FormRxAlertEdit(RxAlert rxAlert,RxDef rxDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_rxAlert=rxAlert;
			_rxDef=rxDef;
		}

		private void FormRxAlertEdit_Load(object sender,EventArgs e) {
			textRxName.Text=_rxDef.Drug;
			if(_rxAlert.DiseaseDefNum>0) {
				labelName.Text=Lan.g(this,"If the patient already has this Problem");
				textName.Text=DiseaseDefs.GetName(_rxAlert.DiseaseDefNum);
			}
			if(_rxAlert.AllergyDefNum>0) {
				labelName.Text=Lan.g(this,"If the patient already has this Allergy");
				textName.Text=AllergyDefs.GetOne(_rxAlert.AllergyDefNum).Description;
			}
			if(_rxAlert.MedicationNum>0) {
				labelName.Text=Lan.g(this,"If the patient is already taking this medication");
				textName.Text=Medications.GetMedicationFromDb(_rxAlert.MedicationNum).MedName;
			}
			textMessage.Text=_rxAlert.NotificationMsg;
			checkIsHighSignificance.Checked=_rxAlert.IsHighSignificance;
		}

		private void butOK_Click(object sender,EventArgs e) {
			_rxAlert.NotificationMsg=PIn.String(textMessage.Text);
			_rxAlert.IsHighSignificance=checkIsHighSignificance.Checked;
			RxAlerts.Update(_rxAlert);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			RxAlerts.Delete(_rxAlert);
			DialogResult=DialogResult.OK;
		}
	}
}