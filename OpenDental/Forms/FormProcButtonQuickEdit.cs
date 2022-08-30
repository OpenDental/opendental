using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcButtonQuickEdit:FormODBase {
		public ProcButtonQuick ProcButtonQuickCur;
		public bool IsNew;


		public FormProcButtonQuickEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcButtonQuickEdit_Load(object sender,EventArgs e) {
			textDescript.Text=ProcButtonQuickCur.Description;
			textProcedureCode.Text=ProcButtonQuickCur.CodeValue;
			textSurfaces.Text=ProcButtonQuickCur.Surf;
			checkIsLabel.Checked=ProcButtonQuickCur.IsLabel;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelSurfaces.Visible=false;
				textSurfaces.Visible=false;
			}
		}

		private void checkIsLabel_CheckedChanged(object sender,EventArgs e) {
			textProcedureCode.Enabled=!checkIsLabel.Checked;
			textSurfaces.Enabled=!checkIsLabel.Checked;
			butPickProc.Enabled=!checkIsLabel.Checked;
		}

		private void butPickProc_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			textProcedureCode.Text=ProcedureCodes.GetProcCode(formProcCodes.SelectedCodeNum).ProcCode;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				ProcButtonQuickCur=null;
				DialogResult=DialogResult.Cancel;
				return;
			}
			ProcButtonQuicks.Delete(ProcButtonQuickCur.ProcButtonQuickNum);
			ProcButtonQuickCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			ProcButtonQuickCur.Description=textDescript.Text;
			ProcButtonQuickCur.CodeValue=textProcedureCode.Text;
			ProcButtonQuickCur.Surf=textSurfaces.Text;
			ProcButtonQuickCur.IsLabel=checkIsLabel.Checked;
			//TODO: Validation, if we need any.
			if(IsNew) {
				ProcButtonQuicks.Insert(ProcButtonQuickCur);
			}
			else {
				ProcButtonQuicks.Update(ProcButtonQuickCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}