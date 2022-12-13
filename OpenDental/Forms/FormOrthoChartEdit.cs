using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoChartEdit:FormODBase {
		public OrthoChart OrthoChartCur;
		public bool IsNew;

		public FormOrthoChartEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			OrthoChartCur = new OrthoChart();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormOrthoChartEdit_Load(object sender,EventArgs e) {
			textDateService.Text=OrthoChartCur.DateService.ToShortDateString();
			textFieldName.Text=OrthoChartCur.FieldName;
			textFieldValue.Text=OrthoChartCur.FieldValue;
		}
	}
}