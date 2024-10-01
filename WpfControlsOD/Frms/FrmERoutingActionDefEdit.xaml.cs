using System;
using System.Collections.Generic;
using System.Linq;
using OpenDentBusiness;

namespace OpenDental {

	/// <summary> Handles editing and deleting eRoutingAction. </summary>
	public partial class FrmERoutingActionDefEdit : FrmODBase {

		public ERoutingActionDef ERoutingActionDefCur;
		private List<SheetDef> _listSheetDef;

		///<summary> Pass in the action to be edited. </summary>
		public FrmERoutingActionDefEdit(ERoutingActionDef eRoutingActionDef) {
			ERoutingActionDefCur=eRoutingActionDef;
			InitializeComponent();
			//Event Handlers
			Load+=FrmERoutingActionDefEdit_Load;
			comboActionType.SelectionChangeCommitted+=comboActionType_SelectionChangeCommitted;
		}

		private void FrmERoutingActionDefEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			//First set action type and other simple fields
			comboActionType.Items.AddEnums<EnumERoutingActionType>();
			comboActionType.SetSelected((int)ERoutingActionDefCur.ERoutingActionType);
			textLabelOverride.Text=ERoutingActionDefCur.LabelOverride;
			//Then fetch the defaults for the chosen Action Type
			GetDefaultOptionsForERoutingActionType();
			//If FK is set, select it in the UI
			if(ERoutingActionDefCur.ForeignKey!=0){
				SheetDef sheetDefAttached=_listSheetDef.FirstOrDefault( x=> x.SheetDefNum==ERoutingActionDefCur.ForeignKey);
				if(sheetDefAttached!=null){
					comboDefaultToLoad.SetSelectedKey<SheetDef>(sheetDefAttached.SheetDefNum, (x) => {return x.SheetDefNum;});
				}
			}
			else{
				//Select "None"
				comboDefaultToLoad.SelectedIndex=0;
			}
		}

		/// <summary> Gets default options for the selected EnumERoutingActionType, and enablesComboDefaultToLoad if appropriate. </summary>
		public void GetDefaultOptionsForERoutingActionType(){
			//Disable and clear comboDefaultToLoad, and then fill and enable it when appropriate.
			comboDefaultToLoad.IsEnabled=false;
			comboDefaultToLoad.Items.Clear();
			if(comboActionType.GetSelected<EnumERoutingActionType>()==EnumERoutingActionType.ExamSheet) {
				//Exam sheet eRouting action defs can have a sheet def from the Exam Sheet type associated with them.
				//Fetch the list to pick from, and enable the combobox.
				comboDefaultToLoad.Items.AddNone<SheetDef>();
				_listSheetDef=SheetDefs.GetWhere(x=>x.SheetType==SheetTypeEnum.ExamSheet && x.HasMobileLayout);
				comboDefaultToLoad.Items.AddList(_listSheetDef, x=>x.Description);
				comboDefaultToLoad.IsEnabled=true;
			}
			else if(comboActionType.GetSelected<EnumERoutingActionType>()==EnumERoutingActionType.ConsentForm) {
				//Consent Form eRouting action defs can have a sheet def from the Consent Form type associated with them.
				//Fetch the list to pick from, and enable the combobox.
				comboDefaultToLoad.Items.AddNone<SheetDef>();
				_listSheetDef=SheetDefs.GetWhere(x=>x.SheetType==SheetTypeEnum.Consent && x.HasMobileLayout);
				comboDefaultToLoad.Items.AddList(_listSheetDef, x=>x.Description);
				comboDefaultToLoad.IsEnabled=true;
			}
		}

		/// <summary> Fetch the defaults for the selected category, and clear the selection. </summary>
		private void comboActionType_SelectionChangeCommitted(object sender,EventArgs e) {
			GetDefaultOptionsForERoutingActionType();
			comboDefaultToLoad.SelectedIndex=0;
		}

		private void butSave_Click(object sender, EventArgs e) {
			//Validate
			if(comboActionType.GetSelected<EnumERoutingActionType>()==EnumERoutingActionType.None) {
				MsgBox.Show(this, "Please select an Action Type.");
				return;
			}
			//Set Fields
			ERoutingActionDefCur.LabelOverride=textLabelOverride.Text;
			ERoutingActionDefCur.ERoutingActionType=(EnumERoutingActionType)comboActionType.SelectedItem;
			SheetDef SheetDefAttached=comboDefaultToLoad.GetSelected<SheetDef>();
			if(comboDefaultToLoad.GetSelected<SheetDef>()!=null && comboDefaultToLoad.GetSelected<SheetDef>().SheetDefNum!=0){
				//Attach Default
				ERoutingActionDefCur.ForeignKey=SheetDefAttached.SheetDefNum;
				ERoutingActionDefCur.ForeignKeyType=EnumERoutingDefFKType.SheetDef;
			}
			else{
				//Clear Default
				ERoutingActionDefCur.ForeignKey=0;
				ERoutingActionDefCur.ForeignKeyType=EnumERoutingDefFKType.None;
			}
			IsDialogOK=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this, buttons:MsgBoxButtons.YesNo,"Delete?", "Delete?")){
				return;
			}
			ERoutingActionDefCur=null;
			IsDialogOK=false;
			//Set it null, and handle removing it from the UI and database on the parent form.
		}
	}
}