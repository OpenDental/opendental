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
	/// <summary>The editor is for the EFormField even though we're really editing the EFormFieldDef. This editor is not patient facing.</summary>
	public partial class FrmEFormSigBoxEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>We need access to a few other fields of the EFormDef.</summary>
		public EFormDef EFormDefCur;

		///<summary></summary>
		public FrmEFormSigBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsTextBoxEdit_Load;
			PreviewKeyDown+=FrmEFormTextBoxEdit_PreviewKeyDown;
		}

		private void FrmEFormsTextBoxEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textLabel.Text=EFormFieldCur.ValueLabel;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur=null;
			IsDialogOK=true;
		}

		private void FrmEFormTextBoxEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntFontScale.IsValid()) {
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			//end of validation
			EFormFieldCur.ValueLabel=textLabel.Text;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}