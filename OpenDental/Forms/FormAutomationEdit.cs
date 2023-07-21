using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormAutomationEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Automation _automation;
		private List<AutomationCondition> _listAutomationConditions;
		///<summary>List of actions currently in the drop down.  Some actions are only available for specific triggers, so this is possibly a sub-set of
		///all AutomationAction enum values.</summary>
		private List<AutomationAction> _listAutomationActions;
		///<summary>Matches list of appointments in comboAppointmentType. Does not include hidden types unless current automation already has that type set.</summary>
		private List<AppointmentType> _listAppointmentTypes;
		private List<Def> _listDefs;

		///<summary></summary>
		public FormAutomationEdit(Automation automation)
		{
			//
			// Required for Windows Form Designer support
			//
			_automation=automation.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutomationEdit_Load(object sender, System.EventArgs e) {
			_listDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes,true);
			textDescription.Text=_automation.Description;
			_listAppointmentTypes=new List<AppointmentType>() { new AppointmentType() { AppointmentTypeName="none" } };
			AppointmentTypes.GetWhere(x => !x.IsHidden || x.AppointmentTypeNum==_automation.AppointmentTypeNum)
				.ForEach(x => _listAppointmentTypes.Add(x));
			_listAppointmentTypes=_listAppointmentTypes.OrderBy(x => x.AppointmentTypeNum>0).ThenBy(x => x.ItemOrder).ToList();
			Enum.GetNames(typeof(AutomationTrigger)).ToList().ForEach(x => comboTrigger.Items.Add(x));
			comboTrigger.SelectedIndex=(int)_automation.Autotrigger;
			textProcCodes.Text=_automation.ProcCodes;//although might not be visible.
			textMessage.Text=_automation.MessageContent;
			FillGrid();
		}

		private void FillGrid() {
			AutomationConditions.RefreshCache();
			_listAutomationConditions=AutomationConditions.GetListByAutomationNum(_automation.AutomationNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g("AutomationCondition","Field"),200));
			gridMain.Columns.Add(new GridColumn(Lan.g("AutomationCondition","Comparison"),75));
			gridMain.Columns.Add(new GridColumn(Lan.g("AutomationCondition","Text"),100));
			gridMain.ListGridRows.Clear();
			_listAutomationConditions.ForEach(x => gridMain.ListGridRows.Add(new GridRow(x.CompareField.ToString(),x.Comparison.ToString(),x.CompareString)));
			gridMain.EndUpdate();
		}

		private void comboTrigger_SelectedIndexChanged(object sender,EventArgs e) {
			comboAction.Items.Clear();
			_listAutomationActions=Enum.GetValues(typeof(AutomationAction)).OfType<AutomationAction>().ToList();
			//only add the SetApptASAP and SetApptType actions if the triggers CreateAppt or CreateApptNewPat are selected
			if(!comboTrigger.SelectedIndex.In((int)AutomationTrigger.CreateAppt,(int)AutomationTrigger.CreateApptNewPat)) {
				_listAutomationActions.Remove(AutomationAction.SetApptASAP);
				_listAutomationActions.Remove(AutomationAction.SetApptType);
			}
			//only add the PrintRxInstructions actions if the trigger RxCreate is selected
			if(!comboTrigger.SelectedIndex.In((int)AutomationTrigger.RxCreate)) {
				_listAutomationActions.Remove(AutomationAction.PrintRxInstruction);
			}
			_listAutomationActions.ForEach(x => comboAction.Items.Add(x.GetDescription()));
			if((int)_automation.Autotrigger==comboTrigger.SelectedIndex) {
				comboAction.SelectedIndex=_listAutomationActions.IndexOf(_automation.AutoAction);
			}
			else {
				comboAction.SelectedIndex=0;//default to first in the list
			}
			if(comboTrigger.SelectedIndex.In((int)AutomationTrigger.CompleteProcedure,(int)AutomationTrigger.ScheduleProcedure)) {
				labelProcCodes.Visible=true;
				textProcCodes.Visible=true;
				butProcCode.Visible=true;
			}
			else{
				labelProcCodes.Visible=false;
				textProcCodes.Visible=false;
				butProcCode.Visible=false;
			}
		}

		///<summary>Fills comboActionObject with the correct type of items based on the comboAction selection and sets labelActionObject text.
		///Also handles setting combos/labels/texts visibility based on selected action.</summary>
		private void comboAction_SelectedIndexChanged(object sender,EventArgs e) {
			labelActionObject.Text="Action Object";//user should never see this text, just to help with troubleshooting in case of bug
			labelActionObject.Visible=false;
			comboActionObject.Visible=false;
			labelMessage.Visible=false;
			textMessage.Visible=false;
			if(comboAction.SelectedIndex<0 || comboAction.SelectedIndex>=_listAutomationActions.Count) {
				return;
			}
			comboActionObject.Items.Clear();
			switch(_listAutomationActions[comboAction.SelectedIndex]) {
				case AutomationAction.CreateCommlog:
					labelActionObject.Visible=true;
					labelActionObject.Text=Lan.g(this,"Commlog Type");
					comboActionObject.Visible=true;
					_listDefs.ForEach(x => comboActionObject.Items.Add(x.ItemName));
					comboActionObject.SelectedIndex=_listDefs.FindIndex(x => x.DefNum==_automation.CommType);
					labelMessage.Visible=true;
					textMessage.Visible=true;
					return;
				case AutomationAction.PopUp:
				case AutomationAction.PopUpThenDisable10Min:
					labelMessage.Visible=true;
					textMessage.Visible=true;
					return;
				case AutomationAction.SetApptASAP:
					return;
				case AutomationAction.SetApptType:
					labelActionObject.Visible=true;
					labelActionObject.Text=Lan.g(this,"Appointment Type");
					comboActionObject.Visible=true;
					//_listAppointmentType contains 'none' with AppointmentTypeNum of 0 at index 0, just add list to combo and FindIndex will always be valid
					_listAppointmentTypes.ForEach(x => comboActionObject.Items.Add(x.AppointmentTypeName));
					comboActionObject.SelectedIndex=_listAppointmentTypes.FindIndex(x => _automation.AppointmentTypeNum==x.AppointmentTypeNum);//should always be >=0
					return;
				case AutomationAction.PrintPatientLetter:
				case AutomationAction.PrintReferralLetter:
				case AutomationAction.ShowConsentForm:
				case AutomationAction.ShowExamSheet:
				case AutomationAction.PrintRxInstruction:
					labelActionObject.Visible=true;
					labelActionObject.Text=Lan.g(this,"Sheet Definition");
					comboActionObject.Visible=true;
					List<SheetDef> listSheetDefs=SheetDefs.GetDeepCopy().FindAll(x => !SheetDefs.IsDashboardType(x));
					//Filter which items show based on SheetType
					for(int i=0;i<listSheetDefs.Count;i++) {
						if(listSheetDefs[i].SheetType==SheetTypeEnum.PatientLetter && _listAutomationActions[comboAction.SelectedIndex]==AutomationAction.PrintPatientLetter) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.ReferralLetter && _listAutomationActions[comboAction.SelectedIndex]==AutomationAction.PrintReferralLetter) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.Consent && _listAutomationActions[comboAction.SelectedIndex]==AutomationAction.ShowConsentForm) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.ExamSheet && _listAutomationActions[comboAction.SelectedIndex]==AutomationAction.ShowExamSheet) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.RxInstruction && _listAutomationActions[comboAction.SelectedIndex]==AutomationAction.PrintRxInstruction) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
					}
					comboActionObject.SetSelectedKey<long>(_automation.SheetDefNum,x => x);//can be -1
					return;
				case AutomationAction.ChangePatStatus:
					labelActionObject.Visible=true;
					labelActionObject.Text=Lan.g(this,"Patient Status");
					comboActionObject.Visible=true;
					//comboActionObject.Items.AddEnums<PatientStatus>();//can't use this because we are not including all the enums
					List<PatientStatus> listPatientStatuses=new List<PatientStatus>();
					listPatientStatuses.AddRange(Enum.GetValues(typeof(PatientStatus)).Cast<PatientStatus>());
					for(int i=0;i<listPatientStatuses.Count;i++) {
						if(listPatientStatuses[i]==PatientStatus.Deleted) {
							continue;//'Deleted' should not be automationAction
						}
						comboActionObject.Items.Add(Lan.g("enum"+nameof(PatientStatus),listPatientStatuses[i].GetDescription()),listPatientStatuses[i]);
					}
					comboActionObject.SetSelectedEnum(_automation.PatStatus);
					return;
			}
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormAutomationConditionEdit formAutmationConditionEdit=new FormAutomationConditionEdit();
			formAutmationConditionEdit.AutomationConditionCur=_listAutomationConditions[e.Row];
			formAutmationConditionEdit.ShowDialog();
			FillGrid();
		}

		private void butProcCode_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			textProcCodes.Text=string.Join(",",new[] { textProcCodes.Text,ProcedureCodes.GetStringProcCode(formProcCodes.CodeNumSelected) }.Where(x => !string.IsNullOrEmpty(x)));
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAutomationConditionEdit formAutomationConditionEdit=new FormAutomationConditionEdit();
			formAutomationConditionEdit.IsNew=true;
			formAutomationConditionEdit.AutomationConditionCur=new AutomationCondition();
			formAutomationConditionEdit.AutomationConditionCur.AutomationNum=_automation.AutomationNum;
			formAutomationConditionEdit.ShowDialog();
			if(formAutomationConditionEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;//delete takes place in FormClosing
			}
			else {
				AutomationConditions.DeleteByAutomationNum(_automation.AutomationNum);
				Automations.Delete(_automation);
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description not allowed to be blank.");
				return;
			}
			if(comboAction.SelectedIndex==-1) {
				MsgBox.Show(this,"Action not allowed to be blank.");
				return;
			}
			_automation.Description=textDescription.Text;
			_automation.Autotrigger=(AutomationTrigger)comboTrigger.SelectedIndex;//should never be <0
			#region ProcCodes
			_automation.ProcCodes="";//set to correct proc code string below if necessary
			if(new[] { AutomationTrigger.CompleteProcedure,AutomationTrigger.ScheduleProcedure }.Contains(_automation.Autotrigger)) {
				if(textProcCodes.Text.Contains(" ")){
					MsgBox.Show(this,"Procedure codes cannot contain any spaces.");
					return;
				}
				if(textProcCodes.Text=="") {
					MsgBox.Show(this,"Please enter valid procedure code(s) first.");
					return;
				}
				string strInvalidCodes=string.Join(", ",textProcCodes.Text.Split(',').Where(x => !ProcedureCodes.IsValidCode(x)));
				if(!string.IsNullOrEmpty(strInvalidCodes)) {
					MessageBox.Show(Lan.g(this,"The following procedure code(s) are not valid")+": "+strInvalidCodes);
					return;
				}
				_automation.ProcCodes=textProcCodes.Text;
			}
			#endregion ProcCodes
			#region Automation Action
			_automation.AutoAction=_listAutomationActions[comboAction.SelectedIndex];
			_automation.SheetDefNum=0;
			_automation.CommType=0;
			_automation.MessageContent="";
			_automation.AptStatus=ApptStatus.None;
			_automation.AppointmentTypeNum=0;
			switch(_automation.AutoAction) {
				case AutomationAction.CreateCommlog:
					if(comboActionObject.SelectedIndex==-1) {
						MsgBox.Show(this,"A commlog type must be selected.");
						return;
					}
					_automation.CommType=_listDefs[comboActionObject.SelectedIndex].DefNum;
					_automation.MessageContent=textMessage.Text;
					break;
				case AutomationAction.PopUp:
				case AutomationAction.PopUpThenDisable10Min:
					if(string.IsNullOrEmpty(textMessage.Text.Trim())) {
						MsgBox.Show(this,"The message cannot be blank.");
						return;
					}
					_automation.MessageContent=textMessage.Text;
					break;
				case AutomationAction.PrintPatientLetter:
				case AutomationAction.PrintReferralLetter:
				case AutomationAction.ShowExamSheet:
				case AutomationAction.ShowConsentForm:
				case AutomationAction.PrintRxInstruction:
					if(comboActionObject.SelectedIndex==-1) {
						MsgBox.Show(this,"A sheet definition must be selected.");
						return;
					}
					_automation.SheetDefNum=comboActionObject.GetSelected<long>();
					break;
				case AutomationAction.SetApptASAP:
					break;
				case AutomationAction.SetApptType:
					if(comboActionObject.SelectedIndex==-1) {
						MsgBox.Show(this,"An appointment type must be selected.");
						return;
					}
					_automation.AppointmentTypeNum=_listAppointmentTypes[comboActionObject.SelectedIndex].AppointmentTypeNum;
					break;
				case AutomationAction.ChangePatStatus:
					if(comboAction.SelectedIndex==-1) {
						MsgBox.Show(this,"A patient status must be selected.");
						return;
					}
					_automation.PatStatus=comboActionObject.GetSelected<PatientStatus>();
					break;
			}
			#endregion Automation Action
			Automations.Update(_automation);//Because always inserted before opening this form.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormAutomationEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			//this happens if cancel or if user deletes a new automation
			if(IsNew) {
				AutomationConditions.DeleteByAutomationNum(_automation.AutomationNum);
				Automations.Delete(_automation);
			}
		}

	}
}





















