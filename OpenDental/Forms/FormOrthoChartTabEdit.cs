using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoChartTabEdit:FormODBase {

		OrthoChartTab _orthoChartTab=null;

		public FormOrthoChartTabEdit(OrthoChartTab orthoChartTab) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_orthoChartTab=orthoChartTab;
		}

		private void FormOrthoChartTabEdit_Load(object sender,EventArgs e) {
			textTabName.Text=_orthoChartTab.TabName;
			checkIsHidden.Checked=_orthoChartTab.IsHidden;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textTabName.Text.Trim()=="") {
				MsgBox.Show(this,"Tab Name cannot be blank.");
				return;
			}
			_orthoChartTab.TabName=textTabName.Text;
			_orthoChartTab.IsHidden=checkIsHidden.Checked;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}