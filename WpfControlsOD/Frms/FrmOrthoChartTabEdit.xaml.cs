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
			//Lan.F(this);
			_orthoChartTab=orthoChartTab;
		}

		private void FrmOrthoChartTabEdit_Loaded(object sender,RoutedEventArgs e) {
			textTabName.Text=_orthoChartTab.TabName;
			checkIsHidden.Checked=_orthoChartTab.IsHidden;
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