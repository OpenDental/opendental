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
		public OrthoChart OrthoCur;
		public bool IsNew;

		public FormOrthoChartEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			OrthoCur = new OrthoChart();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormOrthoChartEdit_Load(object sender,EventArgs e) {
			textDateService.Text=OrthoCur.DateService.ToShortDateString();
			textFieldName.Text=OrthoCur.FieldName;
			textFieldValue.Text=OrthoCur.FieldValue;
		}
	}
}