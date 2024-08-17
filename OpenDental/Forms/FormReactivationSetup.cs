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

namespace OpenDental {
///<summary></summary>
	public partial class FormReactivationSetup : FormODBase {

		///<summary></summary>
		public FormReactivationSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public void FormReactivationSetup_Load(object sender, System.EventArgs e) {
			checkGroupFamilies.Checked = PrefC.GetBool(PrefName.ReactivationGroupByFamily);
			textPostcardsPerSheet.Text=PrefC.GetLong(PrefName.ReactivationPostcardsPerSheet).ToString();
			if(PrefC.GetLong(PrefName.ReactivationDaysPast)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "" to display.
				textDaysPast.Text="";
			}
			else {
				textDaysPast.Text=PrefC.GetLong(PrefName.ReactivationDaysPast).ToString();
			}
			List<Def> listDefsRecallUnschedStatus=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			comboStatusMailedReactivation.Items.AddDefs(listDefsRecallUnschedStatus);
			comboStatusMailedReactivation.SetSelectedDefNum(PrefC.GetLong(PrefName.ReactivationStatusMailed));
			comboStatusEmailedReactivation.Items.AddDefs(listDefsRecallUnschedStatus);
			comboStatusEmailedReactivation.SetSelectedDefNum(PrefC.GetLong(PrefName.ReactivationStatusEmailed));
			if(PrefC.GetLong(PrefName.ReactivationContactInterval)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "0" to display.
				textDaysContactInterval.Text="0";
			}
			else {
				textDaysContactInterval.Text=PrefC.GetLong(PrefName.ReactivationContactInterval).ToString();
			}
			if(PrefC.GetLong(PrefName.ReactivationCountContactMax)==-1) {//For UI display when we store a zero/meaningless value as -1, for this field we want "" to display.
				textMaxReminders.Text="";
			}
			else {
				textMaxReminders.Text=PrefC.GetLong(PrefName.ReactivationCountContactMax).ToString();
			}
			FillGrid();
		}

		private void FillGrid(){
			string availableFields="[NameFL], [NameF], [ClinicName], [ClinicPhone], [PracticeName], [PracticePhone], [OfficePhone]";
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableReactivationMsgs","Mode"),61);
			gridMain.Columns.Add(col);
			col=new GridColumn("",300);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableReactivationMsgs","Message"),500);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			#region 1st Reminder
			//ReactivationEmailSubject
			GridRow row=new GridRow();
			row.Cells.Add(Lan.g(this,"E-mail"));
			row.Cells.Add(Lan.g(this,"Subject line"));
			row.Cells.Add(PrefC.GetString(PrefName.ReactivationEmailSubject));
			row.Tag=PrefName.ReactivationEmailSubject;
			gridMain.ListGridRows.Add(row);
			//ReactivationEmailMessage
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"E-mail"));
			row.Cells.Add(Lan.g(this,"Available variables")+": "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.ReactivationEmailMessage));
			row.Tag=PrefName.ReactivationEmailMessage;
			gridMain.ListGridRows.Add(row);
			//ReactivationEmailFamMsg
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"E-mail"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList] where the list of family members should show."));
			row.Cells.Add(PrefC.GetString(PrefName.ReactivationEmailFamMsg));
			row.Tag=PrefName.ReactivationEmailFamMsg;
			gridMain.ListGridRows.Add(row);
			//ReactivationPostcardMessage
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"Available variables")+": "+availableFields);
			row.Cells.Add(PrefC.GetString(PrefName.ReactivationPostcardMessage));
			row.Tag=PrefName.ReactivationPostcardMessage;
			gridMain.ListGridRows.Add(row);
			//ReactivationPostcardFamMsg
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family.  Use [FamilyList] where the list of family members should show."));
			row.Cells.Add(PrefC.GetString(PrefName.ReactivationPostcardFamMsg));
			row.Tag=PrefName.ReactivationPostcardFamMsg;
			gridMain.ListGridRows.Add(row);
			#endregion
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PrefName prefName=gridMain.SelectedTag<PrefName>();
			FrmRecallMessageEdit frmRecallMessageEdit = new FrmRecallMessageEdit(prefName);
			frmRecallMessageEdit.MessageVal=PrefC.GetString(prefName);
			frmRecallMessageEdit.ShowDialog();
			if(!frmRecallMessageEdit.IsDialogOK) {
				return;
			}
			if(Prefs.UpdateString(prefName,frmRecallMessageEdit.MessageVal)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDaysPast.IsValid()
				|| !textMaxReminders.IsValid()
				|| !textDaysContactInterval.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textPostcardsPerSheet.Text!="1"
				&& textPostcardsPerSheet.Text!="3"
				&& textPostcardsPerSheet.Text!="4") 
			{
				MsgBox.Show(this,"The value in postcards per sheet must be 1, 3, or 4");
				return;
			}
			if(comboStatusMailedReactivation.SelectedIndex==-1 || comboStatusEmailedReactivation.SelectedIndex==-1) {
				MsgBox.Show(this,"All status options on the left must be set.");
				return;
			}
			//End of Validation
			bool didChange=false;
			didChange |= Prefs.UpdateString(PrefName.ReactivationPostcardsPerSheet,textPostcardsPerSheet.Text);
			if(didChange) {
				if(textPostcardsPerSheet.Text=="1") {
					MsgBox.Show(this,"If using 1 postcard per sheet, you must adjust the position, and also the preview will not work");
				}
			}
			didChange |= Prefs.UpdateBool(PrefName.ReactivationGroupByFamily,checkGroupFamilies.Checked);
			if(textDaysPast.Value==0){//or empty
				didChange |= Prefs.UpdateLong(PrefName.ReactivationDaysPast,-1);
			}
			else{
				didChange |= Prefs.UpdateLong(PrefName.ReactivationDaysPast,textDaysPast.Value);
			}
			if(textDaysContactInterval.Value==0){
				didChange |= Prefs.UpdateLong(PrefName.ReactivationContactInterval,-1);
			}
			else{
				didChange |= Prefs.UpdateLong(PrefName.ReactivationContactInterval,textDaysContactInterval.Value);
			}
			if(string.IsNullOrEmpty(textMaxReminders.Text)) {//""= infinite, 0=disabled; 
				didChange |= Prefs.UpdateLong(PrefName.ReactivationCountContactMax,-1);
			}
			else {
				didChange |= Prefs.UpdateLong(PrefName.ReactivationCountContactMax,textMaxReminders.Value);
			}
			//combo boxes These have already been checked for -1
			didChange |= Prefs.UpdateLong(PrefName.ReactivationStatusEmailed,comboStatusEmailedReactivation.GetSelected<Def>().DefNum);
			didChange |= Prefs.UpdateLong(PrefName.ReactivationStatusMailed,comboStatusMailedReactivation.GetSelected<Def>().DefNum);
			if(didChange) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}

}
