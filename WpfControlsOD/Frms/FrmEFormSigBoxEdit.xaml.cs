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
using WpfControls;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>The editor is for the EFormField even though we're really editing the EFormFieldDef. This editor is not patient facing.</summary>
	public partial class FrmEFormSigBoxEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>We need access to a few other fields of the EFormDef.</summary>
		public EFormDef EFormDefCur;
		///<summary>All the siblings</summary>
		public List<EFormField> _listEFormFields;

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
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.CondValueStrConverter(_listEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);//This is used to make checkbox values, "X" and "", more user readable by converting them to "Checked" and "Unchecked".
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur=null;
			IsDialogOK=true;
		}

		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=_listEFormFields;
			int idx=_listEFormFields.IndexOf(EFormFieldCur);
			frmEFormFieldPicker.ListSelectedIndices.Add(idx);//Prevents self selection as parent
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			textCondParent.Text=frmEFormFieldPicker.LabelSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			if(textCondParent.Text==""){
				MsgBox.Show("Please enter a name in the Parent field first.");
				return;
			}
			EFormConditionValueSetter conditionValueSetter=EFormL.SetCondValue(_listEFormFields,textCondParent.Text,textCondValue.Text);
			if(conditionValueSetter.ErrorMsg!="") {
				MsgBox.Show(conditionValueSetter.ErrorMsg);
				return;
			}
			textCondValue.Text=conditionValueSetter.SelectedValue;
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
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.CondValueStrConverter(_listEFormFields,textCondParent.Text,textCondValue.Text);//This is used to convert the user readable checkbox values, "Checked" and "Unchecked", into "X" and "" which are what we store in the database. 
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}