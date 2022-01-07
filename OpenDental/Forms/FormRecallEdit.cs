using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary>Summary description for FormRecallEdit.</summary>
	public partial class FormRecallEdit : FormODBase {
		///<summary>The recall object to edit.</summary>
		public Recall RecallCur;
		///<summary></summary>
		public bool IsNew;
		private List<RecallType> _listRecallTypes;
		private List<Def> _listRecallUnschedStatusDefs;

		//private Patient PatCur;

		///<summary>Don't forget to set the RecallCur before opening this form.</summary>
		public FormRecallEdit(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRecallEdit_Load(object sender, System.EventArgs e) {
			_listRecallTypes=RecallTypes.GetDeepCopy();
			for(int i=0;i<_listRecallTypes.Count;i++){
				comboType.Items.Add(_listRecallTypes[i].Description);
				if(RecallCur.RecallTypeNum==_listRecallTypes[i].RecallTypeNum){
					comboType.SelectedIndex=i;
				}
			}
			if(!IsNew){
				comboType.Enabled=false;
			}
			checkASAP.Checked=RecallCur.Priority==RecallPriority.ASAP;
			checkIsDisabled.Checked=RecallCur.IsDisabled;
			if(checkIsDisabled.Checked){
				textDateDue.ReadOnly=true;
			}
			if(RecallCur.DisableUntilBalance==0) {
				textBalance.Text="";
			}
			else {
				textBalance.Text=RecallCur.DisableUntilBalance.ToString("f");
			}
			if(RecallCur.DisableUntilDate.Year<1880) {
				textDisableDate.Text="";
			}
			else {
				textDisableDate.Text=RecallCur.DisableUntilDate.ToShortDateString();
			}
			if(RecallCur.DatePrevious.Year>1880){
				textDatePrevious.Text=RecallCur.DatePrevious.ToShortDateString();
			}
			if(RecallCur.DateDueCalc.Year>1880){
				textDateDueCalc.Text=RecallCur.DateDueCalc.ToShortDateString();
			}
			if(RecallCur.DateDue.Year>1880){
				textDateDue.Text=RecallCur.DateDue.ToShortDateString();
			}
			if(RecallCur.DateScheduled.Year>1880) {
				textScheduledDate.Text=RecallCur.DateScheduled.ToShortDateString();
			}
			textYears.Text=RecallCur.RecallInterval.Years.ToString();
			textMonths.Text=RecallCur.RecallInterval.Months.ToString();
			textWeeks.Text=RecallCur.RecallInterval.Weeks.ToString();
			textDays.Text=RecallCur.RecallInterval.Days.ToString();
			textPattern.Text=RecallCur.TimePatternOverride;
			comboStatus.Items.Add(Lan.g(this,"None"));
			comboStatus.SelectedIndex=0;
			_listRecallUnschedStatusDefs=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			for(int i=0;i<_listRecallUnschedStatusDefs.Count;i++){
				comboStatus.Items.Add(_listRecallUnschedStatusDefs[i].ItemName);
				if(_listRecallUnschedStatusDefs[i].DefNum==RecallCur.RecallStatus)
					comboStatus.SelectedIndex=i+1;
			}
			textNote.Text=RecallCur.Note;
		}

		private void comboType_SelectionChangeCommitted(object sender,EventArgs e) {
			//not possible unless new recall manually being entered
			Interval iv=_listRecallTypes[comboType.SelectedIndex].DefaultInterval;
			textYears.Text=iv.Years.ToString();
			textMonths.Text=iv.Months.ToString();
			textWeeks.Text=iv.Weeks.ToString();
			textDays.Text=iv.Days.ToString();
			List<RecallTrigger> triggerList=RecallTriggers.GetForType(_listRecallTypes[comboType.SelectedIndex].RecallTypeNum);
			if(triggerList.Count==0) {//if no triggers, then it's a manual type
				RecallCur.DatePrevious=DateTime.Today;
				//textDatePrevious.Text=DateTime.Today.ToShortDateString();
				DateTime dueDate=DateTime.Today+iv;
				textDateDue.Text=dueDate.ToShortDateString();
			}
		}

		private void checkIsDisabled_Click(object sender, System.EventArgs e) {
			if(checkIsDisabled.Checked){
				textDateDue.Text="";
				textDateDue.ReadOnly=true;
			}
			else{
				textDateDue.Text=textDateDueCalc.Text;
				textDateDue.ReadOnly=false;
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(RecallCur.DatePrevious.Year>1880){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This recall should not normally be deleted because the Previous Date has a value.  You should use the Disabled checkBox instead.  But if you are just deleting a duplicate, it's ok to continue.  Continue?")) {
					return;
				}
			}
			else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this recall?")) {
				return;
			}
			Recalls.Delete(RecallCur);
			SecurityLogs.MakeLogEntry(Permissions.RecallEdit,RecallCur.PatNum
				,"Recall deleted with type '"+RecallTypes.GetSpecialTypeStr(RecallCur.RecallTypeNum)+"' and interval '"+RecallCur.RecallInterval+"'");
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(comboType.SelectedIndex==-1){
				MsgBox.Show(this,"Please pick a type first.");
				return;
			}
			for(int i=0;i<textPattern.Text.Length;i++) {
				if(textPattern.Text[i]!='/' && textPattern.Text[i]!='X') {
					MsgBox.Show(this,"Time Pattern may only contain '/' and 'X'.  Please fix to continue.");
					return;
				}
			}
			if(!textDateDue.IsValid()
				|| !textYears.IsValid()
				|| !textMonths.IsValid()
				|| !textWeeks.IsValid()
				|| !textDays.IsValid()
				|| !textBalance.IsValid()
				|| !textDisableDate.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			double disableUntilBalance=PIn.Double(textBalance.Text);
			if(disableUntilBalance<0){
				MsgBox.Show(this,"Disabled balance must be greater than zero.");
				return;
			}
			RecallCur.RecallTypeNum=_listRecallTypes[comboType.SelectedIndex].RecallTypeNum;
			RecallCur.IsDisabled=checkIsDisabled.Checked;
			RecallCur.DisableUntilBalance=disableUntilBalance;
			RecallCur.DisableUntilDate=PIn.Date(textDisableDate.Text);
			RecallCur.DateDue=PIn.Date(textDateDue.Text);
			RecallCur.RecallInterval.Years=PIn.Int(textYears.Text);
			RecallCur.RecallInterval.Months=PIn.Int(textMonths.Text);
			RecallCur.RecallInterval.Weeks=PIn.Int(textWeeks.Text);
			RecallCur.RecallInterval.Days=PIn.Int(textDays.Text);
			RecallCur.TimePatternOverride=PIn.String(textPattern.Text);
      if(comboStatus.SelectedIndex==0){
				RecallCur.RecallStatus=0;
			}
			else{
				RecallCur.RecallStatus
					=_listRecallUnschedStatusDefs[comboStatus.SelectedIndex-1].DefNum;
			}
			RecallCur.Note=textNote.Text;
			RecallCur.Priority=(checkASAP.Checked ? RecallPriority.ASAP : RecallPriority.Normal);
			if(IsNew){
				//if(Recalls.IsAllDefault(RecallCur)){//only save if something meaningful
				//	MsgBox.Show(this,"Recall cannot be saved if all values are still default.");
				//	return;
				//}
				Recalls.Insert(RecallCur);
				SecurityLogs.MakeLogEntry(Permissions.RecallEdit,RecallCur.PatNum,"Recall added from the Edit Recall window.");
			}
			else{
				/*if(Recalls.IsAllDefault(RecallCur)){
					if(!MsgBox.Show(this,true,"All values are default.  This recall will be deleted.  Continue?")){
						return;
					}
					Recalls.Delete(RecallCur);
					DialogResult=DialogResult.OK;
					return;
				}
				else{*/
				Recalls.Update(RecallCur);
				SecurityLogs.MakeLogEntry(Permissions.RecallEdit,RecallCur.PatNum,"Recall edited from the Edit Recall window.");
				//}
			}
			//Recalls.Synch(PatCur.PatNum,RecallCur);//This was moved up into FormRecallsPat.FillGrid.  This is the only way to access a recall.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















