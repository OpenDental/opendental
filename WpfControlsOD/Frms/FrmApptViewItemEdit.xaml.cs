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
	public partial class FrmApptViewItemEdit:FrmODBase {
		public ApptViewItem ApptViewItemCur;
		private bool _hasColorChanged;

		public FrmApptViewItemEdit() {
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmApptViewItemEdit_Loaded(object sender,RoutedEventArgs e) {
			if(ApptViewItemCur.ApptFieldDefNum>0) {
				textDesc.Text=ApptFieldDefs.GetFieldName(ApptViewItemCur.ApptFieldDefNum);
			}
			else {
				textDesc.Text=ApptViewItemCur.ElementDesc;
			}
			panelColor.ColorBack=ColorOD.ToWpf(ApptViewItemCur.ElementColor);
			for(int i=0;i<Enum.GetNames(typeof(ApptViewAlignment)).Length;i++) {
				listAlignment.Items.Add(Enum.GetNames(typeof(ApptViewAlignment))[i]);
			}
			listAlignment.SelectedIndex=(int)ApptViewItemCur.ElementAlignment;
			if(textDesc.Text=="ProcsColored") {
				//This is the one field where setting the color would be meaningless.
				labelBeforeTime.Visible=false;
				panelColor.Visible=false;
				butColor.Visible=false;
			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=panelColor.ColorBack;
			frmColorDialog.ShowDialog();
			if(!frmColorDialog.IsDialogOK) {
				return;
			}
			panelColor.ColorBack=frmColorDialog.Color;
			_hasColorChanged=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_hasColorChanged) {
				ApptViewItemCur.ElementColor=ColorOD.FromWpf(panelColor.ColorBack);
			}
			ApptViewAlignment apptViewAlignment=(ApptViewAlignment)listAlignment.SelectedIndex;
			if(apptViewAlignment!=ApptViewAlignment.Main && textDesc.Text=="ProcsColored") {
				MsgBox.Show(this,"ProcsColored is not intended to be used outside of Main and will not display in the upper or lower right.");
			}
			ApptViewItemCur.ElementAlignment=(ApptViewAlignment)listAlignment.SelectedIndex;
			IsDialogOK=true;
		}


	}
}