using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class FrmDatePicker:FrmODBase {

		public DateTime DateEntered;
		///<summary>Already converted to screen coords at the current scale.</summary>
		public System.Drawing.Point PointStartLocation;
		///<summary>Already converted to screen coords at the current scale.</summary>
		public int WidthForm=0;
		///<summary>Use this if calling from WPF. Math is different</summary>
		public int WidthFormWPF=0;

		public FrmDatePicker() {
			InitializeComponent();
			Lang.F(this);
			Load+=FrmDatePicker_Load;
			KeyDown+=Frm_KeyDown;
		}

		private void FrmDatePicker_Load(object sender,EventArgs e) {
			textDate.Text=DateEntered.ToShortDateString();
			_formFrame.Location=PointStartLocation;
			if(WidthForm!=0) {
				_formFrame.Width=WidthForm;
			}
			if(WidthFormWPF!=0) {
				_formFrame.Width=ScaleFormValue(WidthFormWPF);
			}
		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(textDate.Text)) {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			DateEntered=PIn.Date(textDate.Text);
			IsDialogOK=true;
		}
	}
}