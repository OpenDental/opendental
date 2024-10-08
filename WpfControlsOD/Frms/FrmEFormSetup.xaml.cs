using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using Microsoft.Win32;
using OpenDental.Thinfinity;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmEFormSetup : FrmODBase {
		///<summary></summary>
		public FrmEFormSetup() {
			InitializeComponent();
			Load+=FrmEformSetup_Load;
		}

		///<summary></summary>
		private void FrmEformSetup_Load(object sender, EventArgs e) {
			Lang.F(this);
			textVIntSpaceBelowEachField.Value=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
			textVIntSpaceToRightEachField.Value=PrefC.GetInt(PrefName.EformsSpaceToRightEachField);
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!textVIntSpaceBelowEachField.IsValid()
				|| !textVIntSpaceToRightEachField.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			bool changed=false;
			changed|=Prefs.UpdateInt(PrefName.EformsSpaceBelowEachField,textVIntSpaceBelowEachField.Value);
			changed|=Prefs.UpdateInt(PrefName.EformsSpaceToRightEachField,textVIntSpaceToRightEachField.Value);
			if(changed){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			IsDialogOK=true;
		}
	}
}