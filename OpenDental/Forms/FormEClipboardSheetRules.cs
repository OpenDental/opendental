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
	public partial class FormEClipboardSheetRules:FormODBase {
		EClipboardSheetDef _eClipboardSheetDef;
		public List<EClipboardSheetDef> ListEClipboardSheetDefs;
		///<summary></summary>
		private List<SheetDef> _listSheetDefs;

		public FormEClipboardSheetRules(EClipboardSheetDef eClipboardSheetDef,List<EClipboardSheetDef> listEClipboardSheetDefs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eClipboardSheetDef=eClipboardSheetDef;
			ListEClipboardSheetDefs=listEClipboardSheetDefs;
		}

		private void FormEClipboardSheetRules_Load(object sender,EventArgs e) {
			//Get the name of the sheet
			List<long> listSheetDefNumsIgnored=EClipboardSheetDefs.GetListIgnoreSheetDefNums(_eClipboardSheetDef);
			_listSheetDefs=SheetDefs.GetWhere(x=> listSheetDefNumsIgnored.Contains(x.SheetDefNum));
			textSheet.Text=SheetDefs.GetDescription(_eClipboardSheetDef.SheetDefNum);
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
			listSheetsToIgnore.Visible=_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once;
			listSheetsToIgnore.Items.Clear();
			listSheetsToIgnore.Items.AddStrings(_listSheetDefs.Select(x=>x.Description).ToList());
			butSelectSheetsToIgnore.Visible=_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once;
			labelSheetsToIgnore.Visible=_eClipboardSheetDef.PrefillStatus==PrefillStatuses.Once;
		}

		private void butSelectSheetsToIgnore_Click(object sender,EventArgs e) {
			using FormSheetPicker formSheetPicker=new FormSheetPicker();
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x=>x.SheetDefNum==_eClipboardSheetDef.SheetDefNum);
			//Add any sheet defs that are ignoring this sheet def. We don't want to allow chaining ignores.
			formSheetPicker.ListSheetDefNumsExclude=ListEClipboardSheetDefs
				.Where(x=>x.IgnoreSheetDefNums!=null && x.IgnoreSheetDefNums.Contains(POut.Long(_eClipboardSheetDef.SheetDefNum)))
				.Select(x=>x.SheetDefNum)
				.ToList();
			//Add this sheet def too, the rule shouldn't be able to ignore itself.
			formSheetPicker.ListSheetDefNumsExclude.Add(_eClipboardSheetDef.SheetDefNum);
			//Sets the list of sheets the sheet picker window will display.
			formSheetPicker.ListSheetDefs=SheetDefs.GetCustomForType(sheetDef.SheetType);
			formSheetPicker.ListSheetDefsSelected=formSheetPicker.ListSheetDefs.Where(x=>_listSheetDefs.Select(y=>y.SheetDefNum).Contains(x.SheetDefNum)).ToList();
			formSheetPicker.SheetType=sheetDef.SheetType;
			formSheetPicker.HideKioskButton=true;
			formSheetPicker.AllowMultiSelect=true;
			if(DialogResult.OK!=formSheetPicker.ShowDialog()) {
				return;
			}
			_listSheetDefs=formSheetPicker.ListSheetDefsSelected;
			listSheetsToIgnore.Items.Clear();
			listSheetsToIgnore.Items.AddStrings(_listSheetDefs.Select(x=>x.Description).ToList());
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
			butSelectSheetsToIgnore.Visible=comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once;
			listSheetsToIgnore.Visible=comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once;
			labelSheetsToIgnore.Visible=comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once;
			if(comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once) {
				textFrequency.Text="0";
			}
		}

		private void textFrequency_TextChanged(object sender,EventArgs e) {
			if(textFrequency.Text=="0") {
				comboBehavior.SetSelectedEnum(PrefillStatuses.Once);
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
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
			_eClipboardSheetDef.IgnoreSheetDefNums=string.Join(",",_listSheetDefs.Select(x=>POut.Long(x.SheetDefNum)));
			_eClipboardSheetDef.MinAge=minAge;
			_eClipboardSheetDef.MaxAge=maxAge;
			_eClipboardSheetDef.ResubmitInterval=TimeSpan.FromDays(days);
			//EClipboardSheetDefs.Update(_eSheet);
			DialogResult=DialogResult.OK;
		}

	}
}
