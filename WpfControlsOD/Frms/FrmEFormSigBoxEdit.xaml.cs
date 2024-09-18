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
		public List<EFormField> ListEFormFields;
		///<summary>Set this before opening this window. It's the current language being used in the parent form. Format is the text that's showing in the comboBox. Will be empty string if languages are not set up in pref LanguagesUsedByPatients or if the default language is being used in the parent FrmEFormDefs.</summary>
		public string LanguageShowing="";
		///<summary>We don't fire off a signal to update the language cache on other computers until we hit Save in the form window. So each edit window has this variable to keep track of whether there are any new translations. This bubbles up to the parent.</summary>
		public bool IsChangedLanCache;

		///<summary></summary>
		public FrmEFormSigBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsSigBoxEdit_Load;
			PreviewKeyDown+=FrmEFormSigBoxEdit_PreviewKeyDown;
		}

		private void FrmEFormsSigBoxEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(LanguageShowing==""){
				groupLanguage.Visible=false;
			}
			else{
				textLanguage.Text=LanguageShowing;
				textLabelTranslated.Text=LanguagePats.TranslateEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,EFormFieldCur.ValueLabel);
			}
			textLabel.Text=EFormFieldCur.ValueLabel;
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			checkBorder.Checked=EFormFieldCur.Border==EnumEFormBorder.ThreeD;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			int spaceBelowDefault=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
			labelSpaceDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceBelowDefault.ToString();
			if(EFormFieldCur.SpaceBelow==-1){
				textSpaceBelow.Text="";
			}
			else{
				textSpaceBelow.Text=EFormFieldCur.SpaceBelow.ToString();
			}
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(ListEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			EFormFieldCur.IsDeleted=true;
			//No need to check stacking for next field, SigBox is not a h-stackable type.
			IsDialogOK=true;
		}

		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=ListEFormFields;
			int idx=ListEFormFields.IndexOf(EFormFieldCur);
			frmEFormFieldPicker.ListSelectedIndices.Add(idx);//Prevents self selection as parent
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			textCondParent.Text=frmEFormFieldPicker.ParentSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			textCondValue.Text=EFormL.PickCondValue(ListEFormFields,textCondParent.Text,textCondValue.Text);
		}

		private void FrmEFormSigBoxEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntFontScale.IsValid()) {
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			int spaceBelow=-1;
			if(textSpaceBelow.Text!=""){
				try{
					spaceBelow=Convert.ToInt32(textSpaceBelow.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix error in Space Below first.");
					return;
				}
				if(spaceBelow<0 || spaceBelow>200){
					MsgBox.Show(this,"Space Below value is invalid.");
					return;
				}
			}
			//end of validation
			if(LanguageShowing!=""){
				IsChangedLanCache=LanguagePats.SaveTranslationEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,textLabelTranslated.Text);
				if(IsChangedLanCache){
					LanguagePats.RefreshCache();
				}
			}
			EFormFieldCur.ValueLabel=textLabel.Text;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			if(checkBorder.Checked==true){
				EFormFieldCur.Border=EnumEFormBorder.ThreeD;
			}
			else{
				EFormFieldCur.Border=EnumEFormBorder.None;
			}
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.SpaceBelow=spaceBelow;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(ListEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}