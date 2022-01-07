using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormRecallTypeEdit:FormODBase {
		public RecallType RecallTypeCur;
		private List<RecallTrigger> TriggerList;
		private Interval defaultIntervalOld;
		private int CountForType;

		///<summary></summary>
		public FormRecallTypeEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRecallTypeEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=RecallTypeCur.Description;
		 	defaultIntervalOld=RecallTypeCur.DefaultInterval;
			comboSpecial.Items.Add(Lan.g(this,"none"));
			comboSpecial.Items.Add(Lan.g(this,"Prophy"));
			comboSpecial.Items.Add(Lan.g(this,"ChildProphy"));
			comboSpecial.Items.Add(Lan.g(this,"Perio"));
			SetSpecialIdx();
			CountForType=Recalls.GetCountForType(RecallTypeCur.RecallTypeNum);
			TriggerList=RecallTriggers.GetForType(RecallTypeCur.RecallTypeNum);//works if 0, too.
			SetSpecialText();
			FillTriggers();
			if(PrefC.GetLong(PrefName.RecallTypeSpecialChildProphy)==RecallTypeCur.RecallTypeNum){
				textRecallAgeAdult.Text=PrefC.GetInt(PrefName.RecallAgeAdult).ToString();
			}
			textYears.Text=RecallTypeCur.DefaultInterval.Years.ToString();
			textMonths.Text=RecallTypeCur.DefaultInterval.Months.ToString();
			textWeeks.Text=RecallTypeCur.DefaultInterval.Weeks.ToString();
			textDays.Text=RecallTypeCur.DefaultInterval.Days.ToString();
			textPattern.Text=RecallTypeCur.TimePattern;
			checkAppendToSpecial.Checked=RecallTypeCur.AppendToSpecial;
			FillProcs();
		}

		private void SetSpecialIdx() {
			comboSpecial.SelectedIndex=0;
			if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==RecallTypeCur.RecallTypeNum){
				comboSpecial.SelectedIndex=1;
			}
			else if(PrefC.GetLong(PrefName.RecallTypeSpecialChildProphy)==RecallTypeCur.RecallTypeNum){
				comboSpecial.SelectedIndex=2;
			}
			else if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==RecallTypeCur.RecallTypeNum){
				comboSpecial.SelectedIndex=3;
			}
		}

		private void comboSpecial_SelectionChangeCommitted(object sender,EventArgs e) {
			if(CountForType>0){
				MessageBox.Show(Lan.g(this,"Cannot change Special Type. Patients using this recall type: ") + CountForType.ToString());
				SetSpecialIdx();//sets back to what it was when form opened
				return;
			}
			//cannot change a special type to one that is already in set for another recall type if that recall type is in use by patients
			long recallTypeNumPrev=0;
			switch(comboSpecial.SelectedIndex) {
				case 1:
					recallTypeNumPrev=PrefC.GetLong(PrefName.RecallTypeSpecialProphy);
					break;
				case 2:
					recallTypeNumPrev=PrefC.GetLong(PrefName.RecallTypeSpecialChildProphy);
					break;
				case 3:
					recallTypeNumPrev=PrefC.GetLong(PrefName.RecallTypeSpecialPerio);
					break;
			}
			int countForTypePrev=0;
			if(recallTypeNumPrev>0) {
				countForTypePrev=Recalls.GetCountForType(recallTypeNumPrev);
			}
			if(countForTypePrev>0) {
				MessageBox.Show(Lan.g(this,"Cannot change Special Type to one that is set for another recall type and in use by patients.  "
					+"Patients using the other recall type: ")+countForTypePrev.ToString());
				SetSpecialIdx();//sets back to what it was when form opened
				return;
			}
			SetSpecialText();
		}

		private void SetSpecialText(){
			if(comboSpecial.SelectedIndex==0){
				labelSpecial.Text="";
				listTriggers.Visible=true;
				labelTriggers.Visible=true;
				labelTriggerDisable.Visible=true;
				butAddTrigger.Visible=true;
				butRemoveTrigger.Visible=true;
				groupInterval.Visible=true;
				groupAgeLimit.Visible=false;
			}
			else if(comboSpecial.SelectedIndex==1){//prophy
				labelSpecial.Text="Should include triggers for ChildProphy.";
				listTriggers.Visible=true;
				labelTriggers.Visible=true;
				labelTriggerDisable.Visible=true;
				butAddTrigger.Visible=true;
				butRemoveTrigger.Visible=true;
				groupInterval.Visible=true;
				groupAgeLimit.Visible=false;
			}
			else if(comboSpecial.SelectedIndex==2){//childProphy
				labelSpecial.Text="";//the description is now inside a special group box.
				listTriggers.Visible=false;
				labelTriggers.Visible=false;
				labelTriggerDisable.Visible=false;
				butAddTrigger.Visible=false;
				butRemoveTrigger.Visible=false;
				groupInterval.Visible=false;
				groupAgeLimit.Visible=true;
			}
			else if(comboSpecial.SelectedIndex==3){//Perio
				labelSpecial.Text="Should include triggers.";
				listTriggers.Visible=true;
				labelTriggers.Visible=true;
				labelTriggerDisable.Visible=true;
				butAddTrigger.Visible=true;
				butRemoveTrigger.Visible=true;
				groupInterval.Visible=true;
				groupAgeLimit.Visible=false;
			}
		}

		private void FillTriggers(){
			listTriggers.Items.Clear();
			if(TriggerList.Count==0 || comboSpecial.SelectedIndex==2) {//child prophy special type has no triggers, triggers from Prophy type are used
				return;
			}
			string str;
			for(int i=0;i<TriggerList.Count;i++){
				str=ProcedureCodes.GetStringProcCode(TriggerList[i].CodeNum);
				str+="- "+ProcedureCodes.GetLaymanTerm(TriggerList[i].CodeNum);
				listTriggers.Items.Add(str);
			}
		}

		private void butAddTrigger_Click(object sender,EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			RecallTrigger trigger=new RecallTrigger();
			trigger.CodeNum=FormP.SelectedCodeNum;
			//RecallTypeNum handled during save.
			TriggerList.Add(trigger);
			FillTriggers();
		}

		private void butRemoveTrigger_Click(object sender,EventArgs e) {
			if(listTriggers.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a trigger code first.");
				return;
			}
			TriggerList.RemoveAt(listTriggers.SelectedIndex);
			FillTriggers();
		}

		private void FillProcs(){
			listProcs.Items.Clear();
			if(RecallTypeCur.Procedures==null || RecallTypeCur.Procedures==""){
				return;
			}
			string[] strArray=RecallTypeCur.Procedures.Split(',');
			string str;
			for(int i=0;i<strArray.Length;i++){
				str=strArray[i];
				str+="- "+ProcedureCodes.GetLaymanTerm(ProcedureCodes.GetProcCode(str).CodeNum);
				listProcs.Items.Add(str);
			}
		}

		private void butAddProc_Click(object sender,EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			if(!String.IsNullOrEmpty(RecallTypeCur.Procedures)){
				RecallTypeCur.Procedures+=",";
			}
			RecallTypeCur.Procedures+=ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum);
			FillProcs();
		}

		private void butRemoveProc_Click(object sender,EventArgs e) {
			if(listProcs.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a procedure first.");
				return;
			}
			string[] strArray=RecallTypeCur.Procedures.Split(',');
			List<string> strList=new List<string>(strArray);
			strList.RemoveAt(listProcs.SelectedIndex);
			RecallTypeCur.Procedures="";
			for(int i=0;i<strList.Count;i++){
				if(i>0){
					RecallTypeCur.Procedures+=",";
				}
				RecallTypeCur.Procedures+=strList[i];
			}
			FillProcs();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//Before deleting the actual type, we would need to check special types.
			/*if(RecallCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,true,"Delete this RecallType?")) {
				return;
			}
			try{
				Pharmacies.DeleteObject(PharmCur.PharmacyNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}*/
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			if(listTriggers.Items.Count>0) {
				MsgBox.Show(this,"All triggers must first be deleted.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you absolutely sure you want to delete all recalls of this type?")) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.RecallEdit,0,"Recall type deleted with description '"+RecallTypeCur.Description+"'");
			Recalls.DeleteAllOfType(RecallTypeCur.RecallTypeNum);
			CountForType=Recalls.GetCountForType(RecallTypeCur.RecallTypeNum);
			MsgBox.Show(this,"Done.");

		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			for(int i=0;i<textPattern.Text.Length;i++) {
				if(textPattern.Text[i]!='/' && textPattern.Text[i]!='X') {
					MsgBox.Show(this,"Time Pattern may only contain '/' and 'X'.  Please fix to continue.");
					return;
				}
			}
			if(!textYears.IsValid()
				|| !textMonths.IsValid()
				|| !textWeeks.IsValid()
				|| !textDays.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//if(RecallTypes.List comboSpecial.SelectedIndex
			/*
			if(listTriggers.Items.Count==0 && comboSpecial.SelectedIndex!=2) {//except child prophy
				if(!MsgBox.Show(this,true,"Warning! clearing all triggers for a recall type will cause all patient recalls of that type to be deleted, even those with notes.  Continue anyway?")){
					return;
				}
			}*/
			bool changed=false;
			if(comboSpecial.SelectedIndex==2) {//childProphy
				if(!textRecallAgeAdult.IsValid()) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				if(Prefs.UpdateInt(PrefName.RecallAgeAdult,PIn.Int(textRecallAgeAdult.Text))) {
					changed=true;
				}
				TriggerList.Clear();//triggers for child prophy special type are handled by the prophy special type
			}
			else {//for child prophy, interval will default to 0, since this special type uses the Prophy default interval
				Interval interval=new Interval(
					PIn.Int(textDays.Text),
					PIn.Int(textWeeks.Text),
					PIn.Int(textMonths.Text),
					PIn.Int(textYears.Text));
				RecallTypeCur.DefaultInterval=interval;
			}
			RecallTypeCur.Description=textDescription.Text;
			RecallTypeCur.TimePattern=textPattern.Text;
			RecallTypeCur.AppendToSpecial=checkAppendToSpecial.Checked;
			if(listProcs.Items.Count==0){
				RecallTypeCur.Procedures="";
			}
			//otherwise, already taken care of.
			try{
				if(RecallTypeCur.IsNew) {
					RecallTypes.Insert(RecallTypeCur);
					SecurityLogs.MakeLogEntry(Permissions.RecallEdit,0,"Recall type added '"+RecallTypeCur.Description+"'");
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.RecallEdit,0,"Recall type having description '"+RecallTypeCur.Description+"' edited");
					RecallTypes.Update(RecallTypeCur);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			RecallTriggers.SetForType(RecallTypeCur.RecallTypeNum,TriggerList);
			//The combo for special type is allowed to be changed by user.  But since the field is in the pref table instead of in the RecallType table, there's extra work involved in saving the selection.
			if(comboSpecial.SelectedIndex==0){//none:  If this recall type is now not any special type
				if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==RecallTypeCur.RecallTypeNum){//and it used to be the special prophy type
					Prefs.UpdateLong(PrefName.RecallTypeSpecialProphy,0);
					changed=true;
				}
				if(PrefC.GetLong(PrefName.RecallTypeSpecialChildProphy)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialChildProphy,0);
					changed=true;
				}
				if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialPerio,0);
					changed=true;
				}
			}
			else if(comboSpecial.SelectedIndex==1){//Prophy: If this recall type is now the prophy type.
				if(Prefs.UpdateLong(PrefName.RecallTypeSpecialProphy,RecallTypeCur.RecallTypeNum)){//and it was already the prophy type
					changed=true;
				}
				if(PrefC.GetLong(PrefName.RecallTypeSpecialChildProphy)==RecallTypeCur.RecallTypeNum){//but it used to be the childprophy type.
					Prefs.UpdateLong(PrefName.RecallTypeSpecialChildProphy,0);
					changed=true;
				}
				if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialPerio,0);
					changed=true;
				}
			}
			else if(comboSpecial.SelectedIndex==2){//ChildProphy
				if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialProphy,0);
					changed=true;
				}
				if(Prefs.UpdateLong(PrefName.RecallTypeSpecialChildProphy,RecallTypeCur.RecallTypeNum)){
					changed=true;
				}
				if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialPerio,0);
					changed=true;
				}
			}
			else if(comboSpecial.SelectedIndex==3){//Perio
				if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialProphy,0);
					changed=true;
				}
				if(PrefC.GetLong(PrefName.RecallTypeSpecialChildProphy)==RecallTypeCur.RecallTypeNum){
					Prefs.UpdateLong(PrefName.RecallTypeSpecialChildProphy,0);
					changed=true;
				}
				if(Prefs.UpdateLong(PrefName.RecallTypeSpecialPerio,RecallTypeCur.RecallTypeNum)){
					changed=true;
				}
			}
			DataValid.SetInvalid(InvalidType.RecallTypes);
			if(changed){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//Ask user to update recalls for patients if they changed the DefaultInterval.
			if(!RecallTypeCur.IsNew && defaultIntervalOld!=RecallTypeCur.DefaultInterval) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Default interval has been changed.  "
					+"Please note that this change will not affect patients with intervals that were set manually.\r\n"
					+"Reset all other current patient intervals of this type?")) {
					Recalls.UpdateDefaultIntervalForPatients(RecallTypeCur.RecallTypeNum,defaultIntervalOld,RecallTypeCur.DefaultInterval);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		

		

		

		

		

		


	}
}





















