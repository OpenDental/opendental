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
	public partial class FrmEFormImportRules : FrmODBase {
		///<summary>Pulled from cache when this window loads, and then saved to db if user clicks Save.</summary>
		private List<EFormImportRule> _listEFormImportRules;
		private List<EFormImportRule> _listEFormImportRulesDeleted;
		private List<string> _listFieldsAvail;

		///<summary></summary>
		public FrmEFormImportRules() {
			InitializeComponent();
			Load+=Frm_Load;
			//gridMain.CellDoubleClick+=GridMain_CellDoubleClick;
		}

		private void Frm_Load(object sender, EventArgs e) {
			Lang.F(this);
			_listFieldsAvail=new List<string>();
			//radio buttons that are relevant
			_listFieldsAvail.Add("Gender");
			_listFieldsAvail.Add("ins1Relat");
			_listFieldsAvail.Add("ins2Relat");
			_listFieldsAvail.Add("Position");
			_listFieldsAvail.Add("PreferConfirmMethod");
			_listFieldsAvail.Add("PreferContactMethod");
			_listFieldsAvail.Add("PreferRecallMethod");
			_listFieldsAvail.Add("StudentStatus");
			//only one date
			_listFieldsAvail.Add("Birthdate");
			List<string> listForTextbox=EFormFieldsAvailable.GetList_TextBox();
			//listForTextbox.Remove("None");//leaving this for UI purposes
			listForTextbox.Remove("allergiesOther");
			listForTextbox.Remove("problemsOther");
			_listFieldsAvail.AddRange(listForTextbox);
			_listFieldsAvail=_listFieldsAvail.OrderBy(x=>x!="None").ThenBy(x=>x).ToList();
			_listEFormImportRules=EFormImportRules.GetDeepCopy();
			_listEFormImportRulesDeleted=new List<EFormImportRule>();
			FillGrid();
		}

		private void butHuman_Click(object sender,EventArgs e) {
			for(int i=0;i<_listEFormImportRules.Count;i++){
				if(_listEFormImportRules[i].IsNew){
					continue;
				}
				_listEFormImportRulesDeleted.Add(_listEFormImportRules[i]);
			}
			_listEFormImportRules.Clear();
			//4 global rules---------------------------------------------------------------------------------
			//Global doesn't actually work very well because then you need to do algorithms in your head to figure out what's global and what's override.
			//It's going to be better to just list them all out individually and build tools to quickly change multiple entries.
			//AddRule("",EnumEFormImportSituation.New,EnumEFormImportAction.Overwrite);
			//AddRule("",EnumEFormImportSituation.Changed,EnumEFormImportAction.Review);
			//AddRule("",EnumEFormImportSituation.Deleted,EnumEFormImportAction.Review);
			//AddRule("",EnumEFormImportSituation.Invalid,EnumEFormImportAction.Review);
			//List<string> listTextboxes=new List<string>();//this is a list of textboxes that should be treated the same
			AddRule("Address",EnumEFormImportSituation.New,EnumEFormImportAction.Overwrite);
			AddRule("Address",EnumEFormImportSituation.Changed,EnumEFormImportAction.Overwrite);
			AddRule("Address",EnumEFormImportSituation.Deleted,EnumEFormImportAction.Review);
			AddRule("Address",EnumEFormImportSituation.Invalid,EnumEFormImportAction.Fix);
			AddRule("Address2",EnumEFormImportSituation.New,EnumEFormImportAction.Overwrite);
			AddRule("Address2",EnumEFormImportSituation.Changed,EnumEFormImportAction.Overwrite);
			AddRule("Address2",EnumEFormImportSituation.Deleted,EnumEFormImportAction.Overwrite);
			AddRule("Address2",EnumEFormImportSituation.Invalid,EnumEFormImportAction.Fix);
			AddRule("Birthdate",EnumEFormImportSituation.New,EnumEFormImportAction.Overwrite);
			AddRule("Birthdate",EnumEFormImportSituation.Changed,EnumEFormImportAction.Overwrite);
			AddRule("Birthdate",EnumEFormImportSituation.Deleted,EnumEFormImportAction.Review);
			AddRule("City",EnumEFormImportSituation.New,EnumEFormImportAction.Overwrite);
			AddRule("City",EnumEFormImportSituation.Changed,EnumEFormImportAction.Overwrite);
			AddRule("City",EnumEFormImportSituation.Deleted,EnumEFormImportAction.Review);
			AddRule("City",EnumEFormImportSituation.Invalid,EnumEFormImportAction.Fix);
			AddRule("Email",EnumEFormImportSituation.New,EnumEFormImportAction.Overwrite);
			AddRule("Email",EnumEFormImportSituation.Changed,EnumEFormImportAction.Overwrite);
			AddRule("Email",EnumEFormImportSituation.Deleted,EnumEFormImportAction.Overwrite);
			AddRule("Email",EnumEFormImportSituation.Invalid,EnumEFormImportAction.Fix);
/*
		
			

			listStrings.Add("Address");
			listStrings.Add("Address2");
			_listFieldsAvail.Add("Birthdate");
			listStrings.Add("City");
			listStrings.Add("Email");
			listStrings.Add("FName");
			_listFieldsAvail.Add("Gender");
			listStrings.Add("HmPhone");
			listStrings.Add("ICEName");
			listStrings.Add("ICEPhone");
			listStrings.Add("ins1CarrierName");
			listStrings.Add("ins1CarrierPhone");
			listStrings.Add("ins1EmployerName");
			listStrings.Add("ins1GroupName");
			listStrings.Add("ins1GroupNum");
			_listFieldsAvail.Add("ins1Relat");
			listStrings.Add("ins1SubscriberID");
			listStrings.Add("ins1SubscriberNameF");
			listStrings.Add("ins2CarrierName");
			listStrings.Add("ins2CarrierPhone");
			listStrings.Add("ins2EmployerName");
			listStrings.Add("ins2GroupName");
			listStrings.Add("ins2GroupNum");
			_listFieldsAvail.Add("ins2Relat");
			listStrings.Add("ins2SubscriberID");
			listStrings.Add("ins2SubscriberNameF");
			listStrings.Add("LName");
			//listStrings.Add("medsOther");
			listStrings.Add("MiddleI");
			_listFieldsAvail.Add("Position");
			_listFieldsAvail.Add("PreferConfirmMethod");
			_listFieldsAvail.Add("PreferContactMethod");
			listStrings.Add("Preferred");
			_listFieldsAvail.Add("PreferRecallMethod");
			listStrings.Add("referredFrom");
			listStrings.Add("SSN");
			listStrings.Add("State");
			listStrings.Add("StateNoValidation");//We assume this means that the UI won't enforce two uppercase letters.
			_listFieldsAvail.Add("StudentStatus");
			listStrings.Add("WirelessPhone");
			listStrings.Add("WkPhone");
			listStrings.Add("Zip");

*/



			FillGrid();
		}

		private void AddRules(List<string> listFieldNames,EnumEFormImportSituation enumEFormImportSituation,EnumEFormImportAction enumEFormImportAction){
			for(int i=0;i<listFieldNames.Count;i++){
				AddRule(listFieldNames[i],enumEFormImportSituation,enumEFormImportAction);
			}
		}

		private void AddRule(string fieldName,EnumEFormImportSituation enumEFormImportSituation,EnumEFormImportAction enumEFormImportAction){
			EFormImportRule eFormImportRule=new EFormImportRule();
			eFormImportRule.IsNew=true;
			eFormImportRule.FieldName=fieldName;
			eFormImportRule.Situation=enumEFormImportSituation;
			eFormImportRule.Action=enumEFormImportAction;
			_listEFormImportRules.Add(eFormImportRule);
		}

		private void FillGrid(){
			//_listEFormImportRules=_listEFormImportRules.OrderBy(x=>x.FieldName!="")//global first
			//	.ThenBy(x=>!_listFieldsAvail.Contains(x.FieldName))//then db fields
			//	.ThenBy(x=>x.FieldName)//alphabetical
			//	.ToList();
			//gridMain.BeginUpdate();
			//gridMain.Columns.Clear();
			//GridColumn gridColumn=new GridColumn(Lang.g("TableEFormImportRules","Field Name"),120);
			//gridMain.Columns.Add(gridColumn);
			//gridColumn=new GridColumn(Lans.g("TableEFormImportRules","Situation"),100);
			//gridMain.Columns.Add(gridColumn);
			//gridColumn=new GridColumn(Lans.g("TableEFormImportRules","Action"),100);
			//gridColumn.IsWidthDynamic=true;
			//gridMain.Columns.Add(gridColumn);
			//gridMain.ListGridRows.Clear();
			//for(int i=0;i<_listEFormImportRules.Count;i++){
			//	GridRow gridRow=new GridRow();
			//	if(_listEFormImportRules[i].FieldName==""){
			//		gridRow.Cells.Add("Global");
			//	}
			//	else{
			//		gridRow.Cells.Add(_listEFormImportRules[i].FieldName);
			//	}
			//	gridRow.Cells.Add(_listEFormImportRules[i].Situation.ToString());
			//	gridRow.Cells.Add(_listEFormImportRules[i].Action.ToString());
			//	gridMain.ListGridRows.Add(gridRow);
			//}
			//gridMain.EndUpdate();
		}

		private void GridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			FrmEFormImportRuleEdit frmEFormImportRuleEdit=new FrmEFormImportRuleEdit();
			frmEFormImportRuleEdit.EFormImportRuleCur=_listEFormImportRules[e.Row];
			frmEFormImportRuleEdit.ListFieldsAvail=_listFieldsAvail;
			frmEFormImportRuleEdit.ShowDialog();
			if(_listEFormImportRules[e.Row].IsDeleted){
				//regardless of dialog result
				if(_listEFormImportRules[e.Row].IsNew){//was added previously in this session
					//
				}
				else{
					//we don't actually delete yet, but we don't want it in the list
					_listEFormImportRulesDeleted.Add(_listEFormImportRules[e.Row]);
				}
				_listEFormImportRules.RemoveAt(e.Row);
			}
			else if(frmEFormImportRuleEdit.IsDialogCancel){
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmEFormImportRuleEdit frmEFormImportRuleEdit=new FrmEFormImportRuleEdit();
			//maybe if one is highlighted, we should copy those fields?
			frmEFormImportRuleEdit.EFormImportRuleCur=new EFormImportRule();
			frmEFormImportRuleEdit.EFormImportRuleCur.IsNew=true;
			frmEFormImportRuleEdit.EFormImportRuleCur.FieldName="";//global
			frmEFormImportRuleEdit.ListFieldsAvail=_listFieldsAvail;
			frmEFormImportRuleEdit.ShowDialog();
			if(frmEFormImportRuleEdit.IsDialogCancel){
				return;
			}
			_listEFormImportRules.Add(frmEFormImportRuleEdit.EFormImportRuleCur);
			FillGrid();
		}

		private void butSave_Click(object sender, EventArgs e) {
			//a quick validation, just in case something slipped through
			for(int i=0;i<_listEFormImportRules.Count;i++){
				if(!EFormImportRules.isAllowedSit(_listEFormImportRules[i].FieldName,_listEFormImportRules[i].Situation)){
					MsgBox.Show("Invalid situation is not allowed for "+_listEFormImportRules[i].FieldName);
					return;
				}
				if(!EFormImportRules.isAllowedAction(_listEFormImportRules[i].FieldName,_listEFormImportRules[i].Action)){
					MsgBox.Show("Fix action is not allowed for "+_listEFormImportRules[i].FieldName);
					return;
				}
			}
			for(int i=0;i<_listEFormImportRules.Count;i++){
				if(_listEFormImportRules[i].IsNew){
					EFormImportRules.Insert(_listEFormImportRules[i]);
					continue;
				}
				EFormImportRules.Update(_listEFormImportRules[i]);
			}
			for(int i=0;i<_listEFormImportRulesDeleted.Count;i++){
				EFormImportRules.Delete(_listEFormImportRulesDeleted[i].EFormImportRuleNum);
			}
			DataValid.SetInvalid(InvalidType.Sheets);
			IsDialogOK=true;
		}
	}
}