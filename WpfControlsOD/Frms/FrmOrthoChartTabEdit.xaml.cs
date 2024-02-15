using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmOrthoChartTabEdit:FrmODBase {

		private OrthoChartTab _orthoChartTab=null;

		public FrmOrthoChartTabEdit(OrthoChartTab orthoChartTab) {
			InitializeComponent();
			_orthoChartTab=orthoChartTab;
			Load+=FrmOrthoChartTabEdit_Load;
			PreviewKeyDown+=FrmOrthoChartTabEdit_PreviewKeyDown;
		}

		private void FrmOrthoChartTabEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			textTabName.Text=_orthoChartTab.TabName;
			textTabName.SelectAll();
			checkIsHidden.Checked=_orthoChartTab.IsHidden;
		}

		private void FrmOrthoChartTabEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(textTabName.Text.Trim()=="") {
				MsgBox.Show(this,"Tab Name cannot be blank.");
				return;
			}
			_orthoChartTab.TabName=textTabName.Text;
			_orthoChartTab.IsHidden=(bool)checkIsHidden.Checked;
			IsDialogOK=true;
		}

	}
}