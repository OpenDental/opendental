using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEClipboardSheetRule:FormODBase {
		///<summary>List of the selected SheetDefs to ignore. Sheets only.</summary>
		private List<SheetDef> _listSheetDefs;
		///<summary>The eClipboardSheetDef we are currently editing. Must be set before opening this form.</summary>
		public EClipboardSheetDef EClipboardSheetDefCur;
		///<summary>List of the currently in use EClipboardSheetDefs from the parent form. Used for listSheetsToIgnore, not needed for eForms.</summary>
		public List<EClipboardSheetDef> ListEClipboardSheetDefs;
		///<summary>Gets set to true if an eClipboardSheetDef has been marked for deletion. Deletion occurs in parent form.</summary>
		public bool IsDeleted=false;

		public FormEClipboardSheetRule() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClipboardSheetRule_Load(object sender,EventArgs e) {
			//Get the name of the sheet
			if(EClipboardSheetDefCur.SheetDefNum!=0){
				List<long> listSheetDefNumsIgnored=EClipboardSheetDefs.GetListIgnoreSheetDefNums(EClipboardSheetDefCur);
				_listSheetDefs=SheetDefs.GetWhere(x=>listSheetDefNumsIgnored.Contains(x.SheetDefNum));
				textSheet.Text=SheetDefs.GetDescription(EClipboardSheetDefCur.SheetDefNum);
			}
			else{//EForm
				listSheetsToIgnore.Visible=false;
				butSelectSheetsToIgnore.Visible=false;
				labelSheetsToIgnore.Visible=false;
				EFormDef eFormDef=EFormDefs.GetFirstOrDefault(x=>x.EFormDefNum==EClipboardSheetDefCur.EFormDefNum);
				if(eFormDef!=null){
					textSheet.Text=eFormDef.Description;
				}
			}
			radioBehaviorNew.Checked=EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.New;
			radioBehaviorPreFill.Checked=EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.PreFill;
			radioBehaviorOnce.Checked=EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.Once;
			textFrequency.Text=EClipboardSheetDefCur.ResubmitInterval.TotalDays.ToString();
			if(EClipboardSheetDefCur.MinAge>0) {
				textMinAge.Text=EClipboardSheetDefCur.MinAge.ToString();
			}
			if(EClipboardSheetDefCur.MaxAge>0) {
				textMaxAge.Text=EClipboardSheetDefCur.MaxAge.ToString();
			}
			if(EClipboardSheetDefCur.EFormDefNum!=0){//if this eClipboardSheetDef is an eForm, skip setting up listSheetsToIgnore.
				return;
			}
			listSheetsToIgnore.Visible=EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.Once;
			listSheetsToIgnore.Items.Clear();
			listSheetsToIgnore.Items.AddStrings(_listSheetDefs.Select(x => x.Description).ToList());
			butSelectSheetsToIgnore.Visible=EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.Once;
			labelSheetsToIgnore.Visible=EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.Once;
		}

		///<summary>Only displays when the behavior Once is selected. When a patient fills out the selected sheet, they won't be prompted to fill out any Sheets to Ignore, even if these are in the Sheets in Use list.</summary>
		private void butSelectSheetsToIgnore_Click(object sender,EventArgs e) {
			if(EClipboardSheetDefCur.SheetDefNum==0){
				return;
			}
			FrmSheetPicker frmSheetPicker=new FrmSheetPicker();
			frmSheetPicker.AllowMultiSelect=true;
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x=>x.SheetDefNum==EClipboardSheetDefCur.SheetDefNum);
			//Add any sheet defs that are ignoring this sheet def. We don't want to allow chaining ignores.
			frmSheetPicker.ListSheetDefNumsExclude=ListEClipboardSheetDefs
				.Where(x => x.IgnoreSheetDefNums!=null && x.IgnoreSheetDefNums.Contains(POut.Long(EClipboardSheetDefCur.SheetDefNum)))
				.Select(x => x.SheetDefNum)
				.ToList();
			//Add this sheet def too, the rule shouldn't be able to ignore itself.
			frmSheetPicker.ListSheetDefNumsExclude.Add(EClipboardSheetDefCur.SheetDefNum);
			//Sets the list of sheets the sheet picker window will display.
			frmSheetPicker.ListSheetDefs=SheetDefs.GetCustomForType(sheetDef.SheetType);
			frmSheetPicker.ListSheetDefsSelected=frmSheetPicker.ListSheetDefs.Where(x => _listSheetDefs.Select(y => y.SheetDefNum).Contains(x.SheetDefNum)).ToList();
			frmSheetPicker.SheetType=sheetDef.SheetType;
			frmSheetPicker.HideKioskButton=true;
			frmSheetPicker.AllowMultiSelect=true;
			frmSheetPicker.RequireSelection=false;
			frmSheetPicker.ShowDialog();
			if(!frmSheetPicker.IsDialogOK) {
				return;
			}
			_listSheetDefs=frmSheetPicker.ListSheetDefsSelected;
			listSheetsToIgnore.Items.Clear();
			listSheetsToIgnore.Items.AddStrings(_listSheetDefs.Select(x => x.Description).ToList());
		}

		private void radioBehaviorOnce_CheckedChanged(object sender,EventArgs e) {
			if(EClipboardSheetDefCur.SheetDefNum!=0){//Sheets only, SheetsToIgnore doesn't apply to eForms.
				butSelectSheetsToIgnore.Visible=radioBehaviorOnce.Checked;
				listSheetsToIgnore.Visible=radioBehaviorOnce.Checked;
				labelSheetsToIgnore.Visible=radioBehaviorOnce.Checked;
			}
			if(radioBehaviorOnce.Checked) {
				textFrequency.Text="0";
			}
			else{
				textFrequency.Text="30";//Remove 0 from frequency. Leaving the 0 will cause some UI bugs if the user switched from 'Once' to a different behavior and saves.
				if(EClipboardSheetDefCur.ResubmitInterval.TotalDays.ToString()!="0"){//This is fluff. If user clicks 'Once' and then clicks back to 'PreFill', it will set the frequency back to what it was from the last save.
					textFrequency.Text=EClipboardSheetDefCur.ResubmitInterval.TotalDays.ToString();//Change frequency to previous setting if it wasn't 0.
				}
			}
		}

		private void textFrequency_TextChanged(object sender,EventArgs e) {
			if(textFrequency.Text=="0") {
				radioBehaviorNew.Checked=false;
				radioBehaviorPreFill.Checked=false;
				radioBehaviorOnce.Checked=true;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			int days;
			if(!int.TryParse(textFrequency.Text,out days)) {
				MsgBox.Show(this,"Frequency (days) must be a valid whole number.");
				return;
			}
			if(days<0) {
				MsgBox.Show(this,"Frequency (days) must be greater than -1.");
				return;
			}
			bool isMinAgeBlank=textMinAge.Text.IsNullOrEmpty();
			int minAge=-1;
			if(!isMinAgeBlank && !int.TryParse(textMinAge.Text,out minAge)) {
				MsgBox.Show(this,"The minimum age must be a valid whole number.");
				return;
			}
			if(!isMinAgeBlank && minAge<1) {
				MsgBox.Show(this,"The minimum age must be greater than 0.");
				return;
			}
			bool isMaxAgeBlank=textMaxAge.Text.IsNullOrEmpty();
			int maxAge=-1;
			if(!isMaxAgeBlank && !int.TryParse(textMaxAge.Text,out maxAge)) {
				MsgBox.Show(this,"The maximum age must be a valid whole number.");
				return;
			}
			if(!isMaxAgeBlank && maxAge<1) {
				MsgBox.Show(this,"The maximum age must be greater than 0.");
				return;
			}
			if(!isMaxAgeBlank && !isMinAgeBlank && maxAge<minAge) {
				MsgBox.Show(this,"The maximum age must be greater than the minimum age.");
				return;
			}
			PrefillStatuses prefillStatuses=new PrefillStatuses();
			if(radioBehaviorNew.Checked){
				prefillStatuses=PrefillStatuses.New;
			}
			else if(radioBehaviorPreFill.Checked){
				prefillStatuses=PrefillStatuses.PreFill;
			}
			else if(radioBehaviorOnce.Checked){
				prefillStatuses=PrefillStatuses.Once;
			}
			EClipboardSheetDefCur.PrefillStatus=prefillStatuses;
			if(EClipboardSheetDefCur.PrefillStatus==PrefillStatuses.Once) {
				days=0;
			}
			if(EClipboardSheetDefCur.SheetDefNum!=0){
				EClipboardSheetDefCur.IgnoreSheetDefNums=string.Join(",",_listSheetDefs.Select(x => POut.Long(x.SheetDefNum)));
			}
			EClipboardSheetDefCur.MinAge=minAge;
			EClipboardSheetDefCur.MaxAge=maxAge;
			EClipboardSheetDefCur.ResubmitInterval=TimeSpan.FromDays(days);
			//EClipboardSheetDefs.Update(_eSheet);
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}
	}
}