using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormExamSheets:FormODBase {
		//DataTable table;
		private List<Sheet> _listSheets;
		public long PatNum;

		public FormExamSheets() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormExamSheets_Load(object sender,EventArgs e) {
			Patient pat=Patients.GetLim(PatNum);
			Text=Lan.g(this,"Exam Sheets for")+" "+pat.GetNameFL();
			LayoutMenu();
			FillListExamTypes();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			MenuItemOD menuItemSetup=new MenuItemOD("Setup");
			menuMain.Add(menuItemSetup);
			menuItemSetup.Add("Sheets",menuItemSheets_Click);
			menuMain.EndUpdate();
		}

		private void FillListExamTypes(){
			listExamTypes.Items.Clear();
			List<SheetDef> sheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.ExamSheet);
			listExamTypes.Items.Add(Lan.g(this,"All"),new SheetDef() { SheetDefNum=-1 });//Option to filter for all exam types.
			for(int i=0;i<sheetDefs.Count;i++) {
				listExamTypes.Items.Add(sheetDefs[i].Description,sheetDefs[i]);
			}
			listExamTypes.SelectedIndex=0;//Default to "All".
		}

		private void listExamTypes_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			//if a sheet is selected, remember it
			long selectedSheetNum=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				selectedSheetNum=_listSheets[gridMain.GetSelectedIndex()].SheetNum;//PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["SheetNum"].ToString());
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),210);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),50){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			if(listExamTypes.SelectedIndex==-1) {
				return;//This will happen when resizing due to LayoutManager
			}
			long selectedDefNum=listExamTypes.GetSelected<SheetDef>().SheetDefNum;//-1 when 'All' is selected
			_listSheets=Sheets.GetExamSheetsTable(PatNum,DateTime.MinValue,DateTime.MaxValue,selectedDefNum);
			List<SheetDef> listExamSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.ExamSheet);
			for(int i=0;i<_listSheets.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listSheets[i].DateTimeSheet.ToShortDateString());// ["date"].ToString());
				row.Cells.Add(_listSheets[i].DateTimeSheet.ToShortTimeString());// ["time"].ToString());
				row.Cells.Add(_listSheets[i].Description);
				row.Cells.Add(listExamSheetDefs.FirstOrDefault(x => x.SheetDefNum==_listSheets[i].SheetDefNum)?.Description??"");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//reselect
			if(selectedSheetNum!=0) {
				for(int i=0;i<_listSheets.Count;i++) {
					if(_listSheets[i].SheetNum==selectedSheetNum){ //table.Rows[i]["SheetNum"].ToString()==selectedSheetNum.ToString()) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Sheets
			long sheetNum=_listSheets[e.Row].SheetNum;// PIn.Long(table.Rows[e.Row]["SheetNum"].ToString());
			Sheet sheet=Sheets.GetSheet(sheetNum);//must use this to get fields
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_Grid_FormClosing);
		}

		private void menuItemSheets_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetDefs FormSD=new FormSheetDefs();
			FormSD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sheets");
			FillListExamTypes();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormSheetPicker FormS=new FormSheetPicker();
			FormS.SheetType=SheetTypeEnum.ExamSheet;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			SheetDef sheetDef;
			Sheet sheet=null;//only useful if not Terminal
			for(int i=0;i<FormS.SelectedSheetDefs.Count;i++) {
				sheetDef=FormS.SelectedSheetDefs[i];
				sheet=SheetUtil.CreateSheet(sheetDef,PatNum);
				SheetParameter.SetParameter(sheet,"PatNum",PatNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
			}
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_Add_FormClosing);
		}

		/// <summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_Grid_FormClosing(object sender,FormClosingEventArgs e) {
			if(((FormSheetFillEdit)sender).DialogResult==DialogResult.OK || ((FormSheetFillEdit)sender).DidChangeSheet) {
				FillGrid();
			}
		}

		/// <summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_Add_FormClosing(object sender,FormClosingEventArgs e) {
			if(((FormSheetFillEdit)sender).DialogResult==DialogResult.OK || ((FormSheetFillEdit)sender).DidChangeSheet) {
				if(((FormSheetFillEdit)sender).SheetCur!=null && ((FormSheetFillEdit)sender).SheetCur.Description!=listExamTypes.GetSelected<SheetDef>().ToString()) {
					listExamTypes.SelectedIndex=0;//0 => All
				}
				FillGrid();
				gridMain.SetAll(false);//unselect all rows
				gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);//Select the newly added row. Always last, since ordered by date.
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}
		

		

		
	}
}