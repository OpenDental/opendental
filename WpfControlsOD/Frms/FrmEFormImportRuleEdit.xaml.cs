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
	public partial class FrmEFormImportRuleEdit : FrmODBase {
		public EFormImportRule EFormImportRuleCur;
		public List<string> ListFieldsAvail;

		///<summary></summary>
		public FrmEFormImportRuleEdit() {
			InitializeComponent();
			Load+=Frm_Load;
		}

		private void Frm_Load(object sender, EventArgs e) {
			Lang.F(this);
			
			comboNameDb.Items.AddList(ListFieldsAvail);
			comboNameDb.SetSelected(0);
			if(EFormImportRuleCur.FieldName==""){
				radioNameGlobal.Checked=true;
			}
			else if(ListFieldsAvail.Contains(EFormImportRuleCur.FieldName)){
				radioNameDb.Checked=true;
				comboNameDb.SelectedItem=EFormImportRuleCur.FieldName;
			}
			else{
				radioNameNotDb.Checked=true;
				textNameNotDb.Text=EFormImportRuleCur.FieldName;
			}
			if(EFormImportRuleCur.Situation==EnumEFormImportSituation.New){
				radioSitNew.Checked=true;
			}
			if(EFormImportRuleCur.Situation==EnumEFormImportSituation.Changed){
				radioSitChanged.Checked=true;
			}
			if(EFormImportRuleCur.Situation==EnumEFormImportSituation.Deleted){
				radioSitDeleted.Checked=true;
			}
			if(EFormImportRuleCur.Situation==EnumEFormImportSituation.Invalid){
				radioSitInvalid.Checked=true;
			}
			if(EFormImportRuleCur.Action==EnumEFormImportAction.Overwrite){
				radioActionOverwrite.Checked=true;
			}
			if(EFormImportRuleCur.Action==EnumEFormImportAction.Review){
				radioActionReview.Checked=true;
			}
			if(EFormImportRuleCur.Action==EnumEFormImportAction.Ignore){
				radioActionIgnore.Checked=true;
			}
			if(EFormImportRuleCur.Action==EnumEFormImportAction.Fix){
				radioActionFix.Checked=true;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//The problem here is that IsNew can still be set when double clicking later on a new item.
			//So we need to tell the parent about the Delete button in both cases, but it will be handled differently.
			if(EFormImportRuleCur.IsNew){
				EFormImportRuleCur.IsDeleted=true;
				IsDialogCancel=true;
				return;
			}
			//no need
			//if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Delete this rule?")){
			//	return;
			//}
			EFormImportRuleCur.IsDeleted=true;
			IsDialogOK=true;
		}


		private void butSave_Click(object sender, EventArgs e) {
			if(radioNameDb.Checked==true
				&& comboNameDb.SelectedIndex==0)
			{
				MsgBox.Show(this,"Please specifiy a database field.");
				return;
			}
			if(radioNameNotDb.Checked==true
				&& textNameNotDb.Text=="")
			{
				MsgBox.Show(this,"Please specifiy a field name.");
				return;
			}
			string fieldName="";
			if(radioNameDb.Checked==true){
				fieldName=comboNameDb.GetSelected<string>();
			}
			if(radioNameNotDb.Checked==true){
				fieldName=textNameNotDb.Text;
			}
			EnumEFormImportSituation enumEFormImportSituation=EnumEFormImportSituation.New;
			if(radioSitChanged.Checked==true){
				enumEFormImportSituation=EnumEFormImportSituation.Changed;
			}
			if(radioSitDeleted.Checked==true){
				enumEFormImportSituation=EnumEFormImportSituation.Deleted;
			}
			if(radioSitInvalid.Checked==true){
				enumEFormImportSituation=EnumEFormImportSituation.Invalid;
			}
			if(!EFormImportRules.isAllowedSit(fieldName,enumEFormImportSituation)){
				MsgBox.Show(this,"That situation is not allowed for that field.");
				return;
			}
			EnumEFormImportAction enumEFormImportAction=EnumEFormImportAction.Overwrite;
			if(radioActionReview.Checked==true){
				enumEFormImportAction=EnumEFormImportAction.Review;
			}
			if(radioActionIgnore.Checked==true){
				enumEFormImportAction=EnumEFormImportAction.Ignore;
			}
			if(radioActionFix.Checked==true){
				enumEFormImportAction=EnumEFormImportAction.Fix;
			}
			if(!EFormImportRules.isAllowedAction(fieldName,enumEFormImportAction)){
				MsgBox.Show(this,"That action is not allowed for that field.");
				return;
			}
			//End of validation-------------------------------------------------------------------------------------------
			EFormImportRuleCur.FieldName=fieldName;
			EFormImportRuleCur.Situation=enumEFormImportSituation;
			EFormImportRuleCur.Action=enumEFormImportAction;
			//no db changes here
			IsDialogOK=true;
		}
	}
}