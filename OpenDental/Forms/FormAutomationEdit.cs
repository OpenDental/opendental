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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAutomationEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Automation AutoCur;
		private List<AutomationCondition> autoList;
		///<summary>List of actions currently in the drop down.  Some actions are only available for specific triggers, so this is possibly a sub-set of
		///all AutomationAction enum values.</summary>
		private List<AutomationAction> _listAutoActions;
		///<summary>Matches list of appointments in comboAppointmentType. Does not include hidden types unless current automation already has that type set.</summary>
		private List<AppointmentType> _listAptTypes;
		private List<Def> _listCommLogTypeDefs;

		///<summary></summary>
		public FormAutomationEdit(Automation autoCur)
		{
			//
			// Required for Windows Form Designer support
			//
			AutoCur=autoCur.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutomationEdit_Load(object sender, System.EventArgs e) {
			_listCommLogTypeDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes,true);
			textDescription.Text=AutoCur.Description;
			_listAptTypes=new List<AppointmentType>() { new AppointmentType() { AppointmentTypeName="none" } };
			AppointmentTypes.GetWhere(x => !x.IsHidden || x.AppointmentTypeNum==AutoCur.AppointmentTypeNum)
				.ForEach(x => _listAptTypes.Add(x));
			_listAptTypes=_listAptTypes.OrderBy(x => x.AppointmentTypeNum>0).ThenBy(x => x.ItemOrder).ToList();
			Enum.GetNames(typeof(AutomationTrigger)).ToList().ForEach(x => comboTrigger.Items.Add(x));
			comboTrigger.SelectedIndex=(int)AutoCur.Autotrigger;
			textProcCodes.Text=AutoCur.ProcCodes;//although might not be visible.
			textMessage.Text=AutoCur.MessageContent;
			FillGrid();
		}

		private void FillGrid() {
			AutomationConditions.RefreshCache();
			autoList=AutomationConditions.GetListByAutomationNum(AutoCur.AutomationNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("AutomationCondition","Field"),200));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("AutomationCondition","Comparison"),75));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("AutomationCondition","Text"),100));
			gridMain.ListGridRows.Clear();
			autoList.ForEach(x => gridMain.ListGridRows.Add(new GridRow(x.CompareField.ToString(),x.Comparison.ToString(),x.CompareString)));
			gridMain.EndUpdate();
		}

		private void comboTrigger_SelectedIndexChanged(object sender,EventArgs e) {
			comboAction.Items.Clear();
			_listAutoActions=Enum.GetValues(typeof(AutomationAction)).OfType<AutomationAction>().ToList();
			//only add the SetApptASAP and SetApptType actions if the triggers CreateAppt or CreateApptNewPat are selected
			if(!new[] { (int)AutomationTrigger.CreateAppt,(int)AutomationTrigger.CreateApptNewPat }.Contains(comboTrigger.SelectedIndex)) {
				_listAutoActions.Remove(AutomationAction.SetApptASAP);
				_listAutoActions.Remove(AutomationAction.SetApptType);
			}
			//only add the PrintRxInstructions actions if the trigger RxCreate is selected
			if(!new[] { (int)AutomationTrigger.RxCreate}.Contains(comboTrigger.SelectedIndex)) {
				_listAutoActions.Remove(AutomationAction.PrintRxInstruction);
			}
			_listAutoActions.ForEach(x => comboAction.Items.Add(x.GetDescription()));
			if((int)AutoCur.Autotrigger==comboTrigger.SelectedIndex) {
				comboAction.SelectedIndex=_listAutoActions.IndexOf(AutoCur.AutoAction);
			}
			else {
				comboAction.SelectedIndex=0;//default to first in the list
			}
			if(new[] { (int)AutomationTrigger.CompleteProcedure,(int)AutomationTrigger.ScheduleProcedure }.Contains(comboTrigger.SelectedIndex)) {
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
			if(comboAction.SelectedIndex<0 || comboAction.SelectedIndex>=_listAutoActions.Count) {
				return;
			}
			comboActionObject.Items.Clear();
			switch(_listAutoActions[comboAction.SelectedIndex]) {
				case AutomationAction.CreateCommlog:
					labelActionObject.Visible=true;
					labelActionObject.Text=Lan.g(this,"Commlog Type");
					comboActionObject.Visible=true;
					_listCommLogTypeDefs.ForEach(x => comboActionObject.Items.Add(x.ItemName));
					comboActionObject.SelectedIndex=_listCommLogTypeDefs.FindIndex(x => x.DefNum==AutoCur.CommType);
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
					_listAptTypes.ForEach(x => comboActionObject.Items.Add(x.AppointmentTypeName));
					comboActionObject.SelectedIndex=_listAptTypes.FindIndex(x => AutoCur.AppointmentTypeNum==x.AppointmentTypeNum);//should always be >=0
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
						if(listSheetDefs[i].SheetType==SheetTypeEnum.PatientLetter && _listAutoActions[comboAction.SelectedIndex]==AutomationAction.PrintPatientLetter) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.ReferralLetter && _listAutoActions[comboAction.SelectedIndex]==AutomationAction.PrintReferralLetter) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.Consent && _listAutoActions[comboAction.SelectedIndex]==AutomationAction.ShowConsentForm) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.ExamSheet && _listAutoActions[comboAction.SelectedIndex]==AutomationAction.ShowExamSheet) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
						else if(listSheetDefs[i].SheetType==SheetTypeEnum.RxInstruction && _listAutoActions[comboAction.SelectedIndex]==AutomationAction.PrintRxInstruction) {
							comboActionObject.Items.Add(listSheetDefs[i].Description,listSheetDefs[i].SheetDefNum);
						}
					}
					comboActionObject.SelectedIndex=listSheetDefs.FindIndex(x => AutoCur.SheetDefNum==x.SheetDefNum);//can be -1
					return;
				case AutomationAction.ChangePatStatus:
					labelActionObject.Visible=true;
					labelActionObject.Text=Lan.g(this,"Patient Status");
					comboActionObject.Visible=true;
					//comboActionObject.Items.AddEnums<PatientStatus>();//can't use this because we are not including all the enums
					List<PatientStatus> listPatStatus=new List<PatientStatus>();
					listPatStatus.AddRange(Enum.GetValues(typeof(PatientStatus)).Cast<PatientStatus>());
					foreach(PatientStatus patStatus in listPatStatus) {
						if(patStatus==PatientStatus.Deleted) {
							continue;//'Deleted' should not be automationAction
						}
						comboActionObject.Items.Add(Lan.g("enum"+nameof(PatientStatus),patStatus.GetDescription()),patStatus);
					}
					comboActionObject.SetSelectedEnum(AutoCur.PatStatus);
					return;
			}
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormAutomationConditionEdit FormACE=new FormAutomationConditionEdit();
			FormACE.ConditionCur=autoList[e.Row];
			FormACE.ShowDialog();
			FillGrid();
		}

		private void butProcCode_Click(object sender,EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			textProcCodes.Text=string.Join(",",new[] { textProcCodes.Text,ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum) }.Where(x => !string.IsNullOrEmpty(x)));
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAutomationConditionEdit FormACE=new FormAutomationConditionEdit();
			FormACE.IsNew=true;
			FormACE.ConditionCur=new AutomationCondition();
			FormACE.ConditionCur.AutomationNum=AutoCur.AutomationNum;
			FormACE.ShowDialog();
			if(FormACE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;//delete takes place in FormClosing
			}
			else {
				AutomationConditions.DeleteByAutomationNum(AutoCur.AutomationNum);
				Automations.Delete(AutoCur);
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
			AutoCur.Description=textDescription.Text;
			AutoCur.Autotrigger=(AutomationTrigger)comboTrigger.SelectedIndex;//should never be <0
			#region ProcCodes
			AutoCur.ProcCodes="";//set to correct proc code string below if necessary
			if(new[] { AutomationTrigger.CompleteProcedure,AutomationTrigger.ScheduleProcedure }.Contains(AutoCur.Autotrigger)) {
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
				AutoCur.ProcCodes=textProcCodes.Text;
			}
			#endregion ProcCodes
			#region Automation Action
			AutoCur.AutoAction=_listAutoActions[comboAction.SelectedIndex];
			AutoCur.SheetDefNum=0;
			AutoCur.CommType=0;
			AutoCur.MessageContent="";
			AutoCur.AptStatus=ApptStatus.None;
			AutoCur.AppointmentTypeNum=0;
			switch(AutoCur.AutoAction) {
				case AutomationAction.CreateCommlog:
					if(comboActionObject.SelectedIndex==-1) {
						MsgBox.Show(this,"A commlog type must be selected.");
						return;
					}
					AutoCur.CommType=_listCommLogTypeDefs[comboActionObject.SelectedIndex].DefNum;
					AutoCur.MessageContent=textMessage.Text;
					break;
				case AutomationAction.PopUp:
				case AutomationAction.PopUpThenDisable10Min:
					if(string.IsNullOrEmpty(textMessage.Text.Trim())) {
						MsgBox.Show(this,"The message cannot be blank.");
						return;
					}
					AutoCur.MessageContent=textMessage.Text;
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
					AutoCur.SheetDefNum=comboActionObject.GetSelected<long>();
					break;
				case AutomationAction.SetApptASAP:
					break;
				case AutomationAction.SetApptType:
					if(comboActionObject.SelectedIndex==-1) {
						MsgBox.Show(this,"An appointment type must be selected.");
						return;
					}
					AutoCur.AppointmentTypeNum=_listAptTypes[comboActionObject.SelectedIndex].AppointmentTypeNum;
					break;
				case AutomationAction.ChangePatStatus:
					if(comboAction.SelectedIndex==-1) {
						MsgBox.Show(this,"A patient status must be selected.");
						return;
					}
					AutoCur.PatStatus=comboActionObject.GetSelected<PatientStatus>();
					break;
			}
			#endregion Automation Action
			Automations.Update(AutoCur);//Because always inserted before opening this form.
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
				AutomationConditions.DeleteByAutomationNum(AutoCur.AutomationNum);
				Automations.Delete(AutoCur);
			}
		}

	}
}





















