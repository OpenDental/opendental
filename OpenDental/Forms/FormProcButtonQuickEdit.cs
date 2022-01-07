using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcButtonQuickEdit:FormODBase {
		public ProcButtonQuick pbqCur;
		public bool IsNew;


		public FormProcButtonQuickEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcButtonQuickEdit_Load(object sender,EventArgs e) {
			textDescript.Text=pbqCur.Description;
			textProcedureCode.Text=pbqCur.CodeValue;
			textSurfaces.Text=pbqCur.Surf;
			checkIsLabel.Checked=pbqCur.IsLabel;
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
			using FormProcCodes FormPC=new FormProcCodes();
			FormPC.IsSelectionMode=true;
			FormPC.ShowDialog();
			if(FormPC.DialogResult!=DialogResult.OK) {
				return;
			}
			textProcedureCode.Text=ProcedureCodes.GetProcCode(FormPC.SelectedCodeNum).ProcCode;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				pbqCur=null;
				DialogResult=DialogResult.Cancel;
			}
			else {
				ProcButtonQuicks.Delete(pbqCur.ProcButtonQuickNum);
				pbqCur=null;
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			pbqCur.Description=textDescript.Text;
			pbqCur.CodeValue=textProcedureCode.Text;
			pbqCur.Surf=textSurfaces.Text;
			pbqCur.IsLabel=checkIsLabel.Checked;
			//TODO: Validation, if we need any.
			if(IsNew) {
				ProcButtonQuicks.Insert(pbqCur);
			}
			else {
				ProcButtonQuicks.Update(pbqCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}