using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace OpenDental{
///<summary></summary>
	public partial class FormRecallSetup : FormODBase {
		private List<RecallType> listRecallCache;

		///<summary></summary>
		public FormRecallSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//Lan.C(this, new System.Windows.Forms.Control[] {
				//textBox1,
				//textBox6
			//});
		}

		public void FormRecallSetup_Load(object sender, System.EventArgs e) {
			FillManualRecall();
		}

		///<summary>Called on load to initially load the recall window with values from the database.  Calls FillGrid at the end.</summary>
		private void FillManualRecall() {
			checkGroupFamilies.Checked = PrefC.GetBool(PrefName.RecallGroupByFamily);
			textPostcardsPerSheet.Text=PrefC.GetLong(PrefName.RecallPostcardsPerSheet).ToString();
			checkReturnAdd.Checked=PrefC.GetBool(PrefName.RecallCardsShowReturnAdd);
			checkGroupFamilies.Checked=PrefC.GetBool(PrefName.RecallGroupByFamily);
			if(PrefC.GetLong(PrefName.RecallDaysPast)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "".
				textDaysPast.Text="";
			}
			else {
				textDaysPast.Text=PrefC.GetLong(PrefName.RecallDaysPast).ToString();
			}
			if(PrefC.GetLong(PrefName.RecallDaysFuture)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "".
				textDaysFuture.Text="";
			}
			else {
				textDaysFuture.Text=PrefC.GetLong(PrefName.RecallDaysFuture).ToString();
			}
			if(PrefC.GetBool(PrefName.RecallExcludeIfAnyFutureAppt)) {
				radioExcludeFutureYes.Checked=true;
			}
			else {
				radioExcludeFutureNo.Checked=true;
			}
			textRight.Text=PrefC.GetDouble(PrefName.RecallAdjustRight).ToString();
			textDown.Text=PrefC.GetDouble(PrefName.RecallAdjustDown).ToString();
			List<Def> listUnschedStatusDefs=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			comboStatusMailedRecall.Items.Clear();
			comboStatusMailedRecall.Items.AddDefs(listUnschedStatusDefs);
			comboStatusMailedRecall.SetSelectedDefNum(PrefC.GetLong(PrefName.RecallStatusMailed));
			comboStatusEmailedRecall.Items.Clear();
			comboStatusEmailedRecall.Items.AddDefs(listUnschedStatusDefs);
			comboStatusEmailedRecall.SetSelectedDefNum(PrefC.GetLong(PrefName.RecallStatusEmailed));
			comboStatusTextedRecall.Items.Clear();
			comboStatusTextedRecall.Items.AddDefs(listUnschedStatusDefs);
			comboStatusTextedRecall.SetSelectedDefNum(PrefC.GetLong(PrefName.RecallStatusTexted));
			comboStatusEmailTextRecall.Items.Clear();
			comboStatusEmailTextRecall.Items.AddDefs(listUnschedStatusDefs);
			comboStatusEmailTextRecall.SetSelectedDefNum(PrefC.GetLong(PrefName.RecallStatusEmailedTexted));
			List<long> recalltypes=new List<long>();
			string[] typearray=PrefC.GetString(PrefName.RecallTypesShowingInList).Split(',');
			if(typearray.Length>0) {
				for(int i=0;i<typearray.Length;i++) {
					recalltypes.Add(PIn.Long(typearray[i]));
				}
			}
			listRecallCache=RecallTypes.GetWhere(x => x.Description!="Child Prophy");
			for(int i=0;i<listRecallCache.Count;i++) {
				listTypes.Items.Add(listRecallCache[i].Description);
				if(recalltypes.Contains(listRecallCache[i].RecallTypeNum)) {
					listTypes.SetSelected(i);
				}
			}
			if(PrefC.GetLong(PrefName.RecallShowIfDaysFirstReminder)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "" to display.
				textDaysFirstReminder.Text="";
			}
			else {
				textDaysFirstReminder.Text=PrefC.GetLong(PrefName.RecallShowIfDaysFirstReminder).ToString();
			}
			if(PrefC.GetLong(PrefName.RecallShowIfDaysSecondReminder)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "" to display.
				textDaysSecondReminder.Text="";
			}
			else {
				textDaysSecondReminder.Text=PrefC.GetLong(PrefName.RecallShowIfDaysSecondReminder).ToString();
			}
			if(PrefC.GetLong(PrefName.RecallMaxNumberReminders)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "" to display.
				textMaxReminders.Text="";
			}
			else {
				textMaxReminders.Text=PrefC.GetLong(PrefName.RecallMaxNumberReminders).ToString();
			}
			if(PrefC.GetBool(PrefName.RecallUseEmailIfHasEmailAddress)) {
				radioUseEmailTrue.Checked=true;
			}
			else {
				radioUseEmailFalse.Checked=true;
			}
			FillGrid();
		}

		private void FillGrid(){
			string availableFields="[NameF], [DueDate], [ClinicName], [ClinicPhone], [PracticeName], [PracticePhone], [OfficePhone]";
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableRecallMsgs","Remind#"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallMsgs","Mode"),61);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("",300);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallMsgs","Message"),500);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			#region 1st Reminder
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"Subject line"));
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailSubject));//old
			row.Tag="RecallEmailSubject";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [NameFL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailMessage));
			row.Tag="RecallEmailMessage";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList] where the list of family members should show."));
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailFamMsg));
			row.Tag="RecallEmailFamMsg";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [NameFL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.RecallPostcardMessage));//old
			row.Tag="RecallPostcardMessage";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList] where the list of family members should show."));
			row.Cells.Add(PrefC.GetString(PrefName.RecallPostcardFamMsg));//old
			row.Tag="RecallPostcardFamMsg";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"WebSched Email"));
			row.Cells.Add(Lan.g(this,"Subject line.  Available variables")+": [NameF]");
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedSubject));
			row.Tag="WebSchedSubject";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"WebSched Email"));
			row.Cells.Add(Lan.g(this,"Email body.  Available variables")+": [URL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedMessage));
			row.Tag="WebSchedMessage";
			gridMain.ListGridRows.Add(row);
			AddRow(Lans.g(this,"All"),Lans.g(this,"WebSched Email Aggregate"),Lans.g(this,"Subject Line.  Available variables")+": [NameF], "
				+"[ClinicName], [PracticeName], [OfficeName]",PrefName.WebSchedAggregatedEmailSubject);
			AddRow(Lans.g(this,"All"),Lans.g(this,"WebSched Email Aggregate"),Lans.g(this,"Email body.  Available variables")+": [FamilyListURLs], [NameF],"
				+" [ClinicName], [ClinicPhone], [PracticeName], [PracticePhone], [OfficeName], [OfficePhone]",PrefName.WebSchedAggregatedEmailBody);
			//
			row=new GridRow();
			row.Cells.Add("1");
			row.Cells.Add(Lan.g(this,"WebSched Text"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [URL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedMessageText));
			row.Tag=PrefName.WebSchedMessageText.ToString();
			gridMain.ListGridRows.Add(row);
			AddRow(Lans.g(this,"All"),Lans.g(this,"WebSched Text Aggregate"),Lans.g(this,"Available variables")+": [FamilyListURLs], [NameF], [ClinicName],"
				+" [ClinicPhone], [PracticeName], [PracticePhone], [OfficeName], [OfficePhone]",PrefName.WebSchedAggregatedTextMessage);
			#endregion
			#region 2nd Reminder
			//2---------------------------------------------------------------------------------------------
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"Subject line"));
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailSubject2));
			row.Tag="RecallEmailSubject2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"Available variables")+": "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailMessage2));
			row.Tag="RecallEmailMessage2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList]."));
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailFamMsg2));
			row.Tag="RecallEmailFamMsg2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [NameFL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.RecallPostcardMessage2));
			row.Tag="RecallPostcardMessage2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList]."));
			row.Cells.Add(PrefC.GetString(PrefName.RecallPostcardFamMsg2));
			row.Tag="RecallPostcardFamMsg2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"WebSched Email"));
			row.Cells.Add(Lan.g(this,"Subject line.  Available variables")+": [NameF]");
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedSubject2));
			row.Tag="WebSchedSubject2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"WebSched Email"));
			row.Cells.Add(Lan.g(this,"Email body.  Available variables")+": [URL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedMessage2));
			row.Tag="WebSchedMessage2";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("2");
			row.Cells.Add(Lan.g(this,"WebSched Text"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [URL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedMessageText2));
			row.Tag=PrefName.WebSchedMessageText2;
			gridMain.ListGridRows.Add(row);
			#endregion
			#region 3rd Reminder
			//3---------------------------------------------------------------------------------------------
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"Subject line"));
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailSubject3));
			row.Tag="RecallEmailSubject3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"Available variables")+": "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailMessage3));
			row.Tag="RecallEmailMessage3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"Email"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList]."));
			row.Cells.Add(PrefC.GetString(PrefName.RecallEmailFamMsg3));
			row.Tag="RecallEmailFamMsg3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [NameFL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.RecallPostcardMessage3));
			row.Tag="RecallPostcardMessage3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList]."));
			row.Cells.Add(PrefC.GetString(PrefName.RecallPostcardFamMsg3));
			row.Tag="RecallPostcardFamMsg3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"WebSched Email"));
			row.Cells.Add(Lan.g(this,"Subject line.  Available variables")+": [NameF]");
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedSubject3));
			row.Tag="WebSchedSubject3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"WebSched Email"));
			row.Cells.Add(Lan.g(this,"Email body.  Available variables")+": [URL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedMessage3));
			row.Tag="WebSchedMessage3";
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add("3");
			row.Cells.Add(Lan.g(this,"WebSched Text"));
			row.Cells.Add(Lan.g(this,"Available variables")+": [URL], "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.WebSchedMessageText3));
			row.Tag=PrefName.WebSchedMessageText3.ToString();
			gridMain.ListGridRows.Add(row);
			#endregion
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PrefName prefName=(PrefName)Enum.Parse(typeof(PrefName),gridMain.ListGridRows[e.Row].Tag.ToString());
			string newPrefValue;
			bool isHtmlTemplate=ListTools.In(prefName,PrefName.WebSchedMessage,PrefName.WebSchedMessage2,PrefName.WebSchedMessage3,
				PrefName.WebSchedAggregatedEmailBody);
			if(isHtmlTemplate) {
				PrefName emailTypePref=PrefName.NotApplicable;
				switch(prefName) {
					case PrefName.WebSchedMessage:
						emailTypePref=PrefName.WebSchedRecallEmailTemplateType;
						break;
					case PrefName.WebSchedMessage2:
						emailTypePref=PrefName.WebSchedRecallEmailTemplateType2;
						break;
					case PrefName.WebSchedMessage3:
						emailTypePref=PrefName.WebSchedRecallEmailTemplateType3;
						break;
					case PrefName.WebSchedAggregatedEmailBody:
						emailTypePref=PrefName.WebSchedRecallEmailTemplateTypeAgg;
						break;
					default:
						break;
				}
				using FormEmailEdit formEmailEdit=new FormEmailEdit {
					MarkupText=PrefC.GetString(prefName),
					DoCheckForDisclaimer=true,
					IsRawAllowed=true,
					IsRaw=PrefC.GetEnum<EmailType>(emailTypePref)==EmailType.RawHtml,
				};
				formEmailEdit.ShowDialog();
				if(formEmailEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				Prefs.UpdateInt(emailTypePref,(formEmailEdit.IsRaw?(int)EmailType.RawHtml:(int)EmailType.Html));
				newPrefValue=formEmailEdit.MarkupText;
			}
			else {
				using FormRecallMessageEdit FormR=new FormRecallMessageEdit(prefName) {
					MessageVal=PrefC.GetString(prefName),
				};
				FormR.ShowDialog();
				if(FormR.DialogResult!=DialogResult.OK) {
					return;
				}
				newPrefValue=FormR.MessageVal;
			}
			Prefs.UpdateString(prefName,newPrefValue);
			FillGrid();
		}		

		private void AddRow(string reminderNum,string title,string availableVariables,PrefName pref) {
			GridRow row=new GridRow();
			row.Cells.Add(reminderNum);
			row.Cells.Add(title);
			row.Cells.Add(availableVariables);
			row.Cells.Add(PrefC.GetString(pref));
			row.Tag=pref.ToString();
			gridMain.ListGridRows.Add(row);
		}

		private void butSetup_Click(object sender,EventArgs e) {
			using FormEServicesWebSchedRecall formEServicesWebSchedRecall=new FormEServicesWebSchedRecall();
			formEServicesWebSchedRecall.ShowDialog();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textRight.IsValid()
				|| !textDown.IsValid()
				|| !textDaysPast.IsValid()
				|| !textDaysFuture.IsValid()
				|| !textDaysFirstReminder.IsValid()
				|| !textDaysSecondReminder.IsValid()
				|| !textMaxReminders.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//We changed the text box to a ValidNum, which prevents it from ever being blank, so this message will never fire.
			//if(textDaysFirstReminder.Text=="") {
			//	if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Initial Reminder box should not be blank, or the recall list will be blank.")) {
			//		return;
			//	}
			//}
			if(textPostcardsPerSheet.Text!="1"
				&& textPostcardsPerSheet.Text!="3"
				&& textPostcardsPerSheet.Text!="4") {
				MsgBox.Show(this,"The value in postcards per sheet must be 1, 3, or 4");
				return;
			}
			if(comboStatusMailedRecall.SelectedIndex==-1
				|| comboStatusEmailedRecall.SelectedIndex==-1
				|| comboStatusTextedRecall.SelectedIndex==-1
				|| comboStatusEmailTextRecall.SelectedIndex==-1) 
			{
				MsgBox.Show(this,"All status options on the left must be set.");
				return;
			}
			//End of Validation
			if(Prefs.UpdateString(PrefName.RecallPostcardsPerSheet,textPostcardsPerSheet.Text)) {
				if(textPostcardsPerSheet.Text=="1") {
					MsgBox.Show(this,"If using 1 postcard per sheet, you must adjust the position, and also the preview will not work");
				}
			}
			Prefs.UpdateBool(PrefName.RecallCardsShowReturnAdd,checkReturnAdd.Checked);
			Prefs.UpdateBool(PrefName.RecallGroupByFamily,checkGroupFamilies.Checked);
			if(string.IsNullOrEmpty(textDaysPast.Text)) {
				Prefs.UpdateLong(PrefName.RecallDaysPast,-1);
			}
			else {
				Prefs.UpdateLong(PrefName.RecallDaysPast,textDaysPast.Value);
			}
			if(string.IsNullOrEmpty(textDaysFuture.Text)) {
				Prefs.UpdateLong(PrefName.RecallDaysFuture,-1);
			}
			else {
				Prefs.UpdateLong(PrefName.RecallDaysFuture,textDaysFuture.Value);
			}
			Prefs.UpdateBool(PrefName.RecallExcludeIfAnyFutureAppt,radioExcludeFutureYes.Checked);
			Prefs.UpdateDouble(PrefName.RecallAdjustRight,PIn.Double(textRight.Text));
			Prefs.UpdateDouble(PrefName.RecallAdjustDown,PIn.Double(textDown.Text));
			//combo boxes These have already been checked for -1
			Prefs.UpdateLong(PrefName.RecallStatusEmailed,comboStatusEmailedRecall.GetSelected<Def>().DefNum);
			Prefs.UpdateLong(PrefName.RecallStatusMailed,comboStatusMailedRecall.GetSelected<Def>().DefNum);
			Prefs.UpdateLong(PrefName.RecallStatusTexted,comboStatusTextedRecall.GetSelected<Def>().DefNum);
			Prefs.UpdateLong(PrefName.RecallStatusEmailedTexted,comboStatusEmailTextRecall.GetSelected<Def>().DefNum);
			string recalltypes = string.Join(",",listTypes.SelectedIndices.OfType<int>().Select(x => listRecallCache[x].RecallTypeNum));
			Prefs.UpdateString(PrefName.RecallTypesShowingInList,recalltypes);
			if(string.IsNullOrEmpty(textDaysFirstReminder.Text)) {
				Prefs.UpdateLong(PrefName.RecallShowIfDaysFirstReminder,-1);
			}
			else {
				Prefs.UpdateLong(PrefName.RecallShowIfDaysFirstReminder,textDaysFirstReminder.Value);
			}
			if(string.IsNullOrEmpty(textDaysSecondReminder.Text)) {
				Prefs.UpdateLong(PrefName.RecallShowIfDaysSecondReminder,-1);
			}
			else {
				Prefs.UpdateLong(PrefName.RecallShowIfDaysSecondReminder,textDaysSecondReminder.Value);
			}
			if(string.IsNullOrEmpty(textMaxReminders.Text)) {//""= infinite, 0=disabled;
				Prefs.UpdateLong(PrefName.RecallMaxNumberReminders,-1);
			}
			else {
				Prefs.UpdateLong(PrefName.RecallMaxNumberReminders,textMaxReminders.Value);
			}
			Prefs.UpdateBool(PrefName.RecallUseEmailIfHasEmailAddress,radioUseEmailTrue.Checked);
			//If we want to take the time to check every Update and see if something changed 
			//then we could move this to a FormClosing event later.
			DataValid.SetInvalid(InvalidType.Prefs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}

}
