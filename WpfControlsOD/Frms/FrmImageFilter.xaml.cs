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
	/// <summary></summary>
	public partial class FrmImageFilter : FrmODBase {
		///<summary>Does not control showing drawings for Pearl or any other external source.</summary>
		public bool ShowOD;
		///<summary></summary>
		public bool ShowPearlToothParts;
		///<summary></summary>
		public bool ShowPearlPolyAnnotations;
		///<summary></summary>
		public bool ShowPearlBoxAnnotations;
		///<summary></summary>
		public bool ShowPearlMeasurements;

		///<summary></summary>
		public FrmImageFilter(){
			InitializeComponent();
			Load+=FrmImageFilter_Load;
			PreviewKeyDown+=FrmImageFilter_PreviewKeyDown;
		}

		private void FrmImageFilter_Load(object sender, EventArgs e) {
			Lang.F(this);
			checkShowOD.Checked=ShowOD;
			if(Programs.IsEnabled(ProgramName.Pearl)){
				groupPearl.Visible=true;
				checkPearlToothParts.Checked=ShowPearlToothParts;
				checkPearlPolyAnnotations.Checked=ShowPearlPolyAnnotations;
				checkPearlBoxAnnotations.Checked=ShowPearlBoxAnnotations;
				checkPearlMeasurements.Checked=ShowPearlMeasurements;
			}
			else{
				groupPearl.Visible=false;
			}
		}

		private void FrmImageFilter_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			ShowOD=checkShowOD.Checked==true;
			if(Programs.IsEnabled(ProgramName.Pearl)){
				ShowPearlToothParts=checkPearlToothParts.Checked==true;
				ShowPearlPolyAnnotations=checkPearlPolyAnnotations.Checked==true;
				ShowPearlBoxAnnotations=checkPearlBoxAnnotations.Checked==true;
				ShowPearlMeasurements=checkPearlMeasurements.Checked==true;
			}
			IsDialogOK=true;
		}
	}
}