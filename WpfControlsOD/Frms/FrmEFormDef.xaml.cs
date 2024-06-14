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
	public partial class FrmEFormDef : FrmODBase {
		public EFormDef EFormDefCur;

		///<summary></summary>
		public FrmEFormDef() {
			InitializeComponent();
			Load+=FrmEFormDef_Load;
		}

		private void FrmEFormDef_Load(object sender, EventArgs e) {
			Lang.F(this);
			textDescription.Text=EFormDefCur.Description;
			listBoxType.Items.AddEnums<EnumEFormType>();
			listBoxType.SetSelectedEnum(EFormDefCur.FormType);
		}

		private void butSave_Click(object sender, EventArgs e) {
			EFormDefCur.Description=textDescription.Text;
			EFormDefCur.FormType=listBoxType.GetSelected<EnumEFormType>();
			IsDialogOK=true;
		}
	}
}