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
		EClipboardSheetDef _eClipboardSheetDef;
		/// <summary></summary>
		public bool IsDeleted=false;
		public List<EClipboardSheetDef> ListEClipboardSheetDefs;
		///<summary></summary>
		private List<SheetDef> _listSheetDefs;

		public FormEClipboardSheetRule(EClipboardSheetDef eClipboardSheetDef,List<EClipboardSheetDef> listEClipboardSheetDefs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eClipboardSheetDef=eClipboardSheetDef;
			ListEClipboardSheetDefs=listEClipboardSheetDefs;
		}

		private void FormEClipboardSheetRule_Load(object sender,EventArgs e) {
			//Get the name of the sheet
			if(_eClipboardSheetDef.SheetDefNum!=0){
				List<long> listSheetDefNumsIgnored=EClipboardSheetDefs.GetListIgnoreSheetDefNums(_eClipboardSheetDef);
				_listSheetDefs=SheetDefs.GetWhere(x=>listSheetDefNumsIgnored.Contains(x.SheetDefNum));
				textSheet.Text=SheetDefs.GetDescription(_eClipboardSheetDef.SheetDefNum);
			}
			else{//EForm
				listSheetsToIgnore.Visible=false;
				butSelectSheetsToIgnore.Visible=false;
				labelSheetsToIgnore.Visible=false;
				EFormDef eFormDef=EFormDefs.GetFirstOrDefault(x=>x.EFormDefNum==_eClipboardSheetDef.EFormDefNum);
				if(eFormDef!=null){
					textSheet.Text=eFormDef.Description;
				}
			}
			comboBehavior.Items.AddEnums<PrefillStatuses>();
			comboBehavior.SetSelectedEnum(_eClipboardSheetDef.PrefillStatus);
			textFrequency.Text=_eClipboardSheetDef.ResubmitInterval.TotalDays.ToString();
			checkMinAge.Checked=false;
			textMinAge.Enabled=false;
			if(_eClipboardSheetDef.MinAge>0) {
				checkMinAge.Checked=true;
				textMinAge.Enabled=true;
				textMinAge.Text=_eClipboardSheetDef.MinAge.ToString();
			}
			checkMaxAge.Checked=false;
			textMaxAge.Enabled=false;
			if(_eClipboardSheetDef.MaxAge>0) {
				checkMaxAge.Checked=true;
				textMaxAge.Enabled=true;
				textMaxAge.Text=_eClipboardSheetDef.MaxAge.ToString();
			}
			if(_eClipboardSheetDef.EFormDefNum!=0){//if this eClipboardSheetDef is an eForm, skip setting up listSheetsToIgnore.
				return;
			}
			listSheetsToIgnore.Visible=_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once;
			listSheetsToIgnore.Items.Clear();
			listSheetsToIgnore.Items.AddStrings(_listSheetDefs.Select(x => x.Description).ToList());
			butSelectSheetsToIgnore.Visible=_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once;
			labelSheetsToIgnore.Visible=_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once;
		}

		///<summary>Only displays when the behavior Once is selected. When a patient fills out the selected sheet, they won't be prompted to fill out any Sheets to Ignore, even if these are in the Sheets in Use list.</summary>
		private void butSelectSheetsToIgnore_Click(object sender,EventArgs e) {
			if(_eClipboardSheetDef.EFormDefNum!=0){
				return;
			}
			FrmSheetPicker frmSheetPicker=new FrmSheetPicker();
			frmSheetPicker.AllowMultiSelect=true;
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x=>x.SheetDefNum==_eClipboardSheetDef.SheetDefNum);
			//Add any sheet defs that are ignoring this sheet def. We don't want to allow chaining ignores.
			frmSheetPicker.ListSheetDefNumsExclude=ListEClipboardSheetDefs
				.Where(x => x.IgnoreSheetDefNums!=null && x.IgnoreSheetDefNums.Contains(POut.Long(_eClipboardSheetDef.SheetDefNum)))
				.Select(x => x.SheetDefNum)
				.ToList();
			//Add this sheet def too, the rule shouldn't be able to ignore itself.
			frmSheetPicker.ListSheetDefNumsExclude.Add(_eClipboardSheetDef.SheetDefNum);
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

		private void checkMinAge_CheckedChanged(object sender,EventArgs e) {
			textMinAge.Enabled=checkMinAge.Checked;
			if(!checkMinAge.Checked) {
				textMinAge.Text="";
			}
		}

		private void checkMaxAge_CheckedChanged(object sender,EventArgs e) {
			textMaxAge.Enabled=checkMaxAge.Checked;
			if(!checkMaxAge.Checked) {
				textMaxAge.Text="";
			}
		}

		private void comboBehavior_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_eClipboardSheetDef.SheetDefNum!=0){//Sheets only, SheetsToIgnore doesn't apply to eForms.
				butSelectSheetsToIgnore.Visible=comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once;
				listSheetsToIgnore.Visible=comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once;
				labelSheetsToIgnore.Visible=comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once;
			}
			if(comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once) {
				textFrequency.Text="0";
			}
		}

		private void textFrequency_TextChanged(object sender,EventArgs e) {
			if(textFrequency.Text=="0") {
				comboBehavior.SetSelectedEnum(PrefillStatuses.Once);
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
			int minAge=-1;
			if(checkMinAge.Checked && !int.TryParse(textMinAge.Text,out minAge)) {
				MsgBox.Show(this,"The minimum age must be a valid whole number.");
				return;
			}
			if(checkMinAge.Checked && minAge<1) {
				MsgBox.Show(this,"The minimum age must be greater than 0.");
				return;
			}
			int maxAge=-1;
			if(checkMaxAge.Checked && !int.TryParse(textMaxAge.Text,out maxAge)) {
				MsgBox.Show(this,"The maximum age must be a valid whole number.");
				return;
			}
			if(checkMaxAge.Checked && maxAge<1) {
				MsgBox.Show(this,"The maximum age must be greater than 0.");
				return;
			}
			if(checkMaxAge.Checked && checkMinAge.Checked && maxAge<minAge) {
				MsgBox.Show(this,"The maximum age must be greater than the minimum age.");
				return;
			}
			_eClipboardSheetDef.PrefillStatus=comboBehavior.GetSelected<PrefillStatuses>();
			if(_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once) {
				days=0;
			}
			if(_eClipboardSheetDef.SheetDefNum!=0){
				_eClipboardSheetDef.IgnoreSheetDefNums=string.Join(",",_listSheetDefs.Select(x => POut.Long(x.SheetDefNum)));
			}
			_eClipboardSheetDef.MinAge=minAge;
			_eClipboardSheetDef.MaxAge=maxAge;
			_eClipboardSheetDef.ResubmitInterval=TimeSpan.FromDays(days);
			//EClipboardSheetDefs.Update(_eSheet);
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}
	}
}