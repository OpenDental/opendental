using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoRxEdit:FormODBase {
		public OrthoRx OrthoRxCur;

		public FormOrthoRxEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoRxEdit_Load(object sender,EventArgs e) {
			
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(OrthoRxCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			try{
				//OrthoHardwareSpecs.Delete(OrthoRxCur.);
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(OrthoRxCur.IsNew){
				//OrthoRxs.Insert(OrthoRxCur);
			}
			else{
				//OrthoRxs.Update(OrthoRxCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}