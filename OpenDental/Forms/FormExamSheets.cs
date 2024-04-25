using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes;
using PdfSharp.Pdf;

namespace OpenDental {
	public partial class FormExamSheets:FormODBase {
		private List<Sheet> _listSheets;
		public long PatNum;

		public FormExamSheets() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormExamSheets_Load(object sender,EventArgs e) {
			Patient patient=Patients.GetLim(PatNum);
			Text=Lan.g(this,"Exam Sheets for")+" "+patient.GetNameFL();
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
			List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.ExamSheet);
			SheetDef sheetDefFilter=new SheetDef();
			sheetDefFilter.SheetDefNum=-1;
			listExamTypes.Items.Add(Lan.g(this,"All"),sheetDefFilter);//Option to filter for all exam types.
			for(int i=0;i<listSheetDefs.Count;i++) {
				listExamTypes.Items.Add(listSheetDefs[i].Description,listSheetDefs[i]);
			}
			listExamTypes.SelectedIndex=0;//Default to "All".
		}

		private void listExamTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			//if a sheet is selected, remember it
			long selectedSheetNum=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				selectedSheetNum=gridMain.SelectedTag<Sheet>().SheetNum;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),54);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),210);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),75);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			SheetDef sheetDefSelected=listExamTypes.GetSelected<SheetDef>();
			if(sheetDefSelected==null) {
				gridMain.EndUpdate();
				panelSheetPreview.Invalidate();
				return;
			}
			_listSheets=Sheets.GetExamSheetsTable(PatNum,DateTime.MinValue,DateTime.MaxValue,sheetDefSelected.SheetDefNum);//SheetDefNum is -1 when 'All' is selected
			Sheets.SetSheetFieldsForSheets(_listSheets);
			List<SheetDef> listSheetDefsExam=SheetDefs.GetCustomForType(SheetTypeEnum.ExamSheet);
			int indexToSelect=0;
			for(int i=0;i<_listSheets.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=_listSheets[i];
				row.Cells.Add(_listSheets[i].DateTimeSheet.ToShortDateString());
				row.Cells.Add(_listSheets[i].DateTimeSheet.ToShortTimeString());
				row.Cells.Add(_listSheets[i].Description);
				string descSheetDef="";
				SheetDef sheetDef=listSheetDefsExam.FirstOrDefault(x=>x.SheetDefNum==_listSheets[i].SheetDefNum);
				if(sheetDef!=null) {
					descSheetDef=sheetDef.Description;
				}
				row.Cells.Add(descSheetDef);
				gridMain.ListGridRows.Add(row);
				if(_listSheets[i].SheetNum==selectedSheetNum) {
					indexToSelect=i;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(indexToSelect);
			panelSheetPreview.Invalidate();
		}

		private void gridMain_SelectionCommitted(object sender,EventArgs e) {
			panelSheetPreview.Invalidate();
			if(gridMain.GetSelectedIndex()==-1) {
				butSendToDevice.Enabled=false;
			}
			else {
				butSendToDevice.Enabled=true;
			}
		}

		private void panelSheetPreview_Paint(object sender,PaintEventArgs e) {
			Graphics g=e.Graphics;
			g.Clear(Color.FromArgb(252,253,254));
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			Sheet sheetSelected=gridMain.SelectedTag<Sheet>();
			//Subtracting 1 from panel dimensions in ratio calculations
			//so that the rectangle we draw for the sheet border does not fall outside the panel.
			float heightRatio=(panelSheetPreview.Height-1)/(float)sheetSelected.HeightPage;
			float widthRatio=(panelSheetPreview.Width-1)/(float)sheetSelected.WidthPage;
			float scalingRatio=heightRatio;
			if(widthRatio<heightRatio) {
				scalingRatio=widthRatio;
			}
			//Passing in 0 causes g.ScaleTransform to throw an error.
			if(scalingRatio==0) {
				return;
			}
			g.ScaleTransform(scalingRatio,scalingRatio);
			Rectangle rectangle=new Rectangle(0,0,sheetSelected.WidthPage,sheetSelected.HeightPage);
			g.FillRectangle(Brushes.White,rectangle);
			SheetPrintingJob sheetPrintingJob=new SheetPrintingJob();
			sheetPrintingJob.DrawSheetFirstPage(g,sheetSelected);
			g.DrawRectangle(Pens.Gray,rectangle);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Sheet sheet=(Sheet)gridMain.ListGridRows[e.Row].Tag;
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_Grid_FormClosing);
		}

		private void menuItemSheets_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormSheetDefs formSheetDefs=new FormSheetDefs();
			formSheetDefs.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Sheets");
			FillListExamTypes();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmSheetPicker frmSheetPicker=new FrmSheetPicker();
			frmSheetPicker.AllowMultiSelect=true;
			frmSheetPicker.SheetType=SheetTypeEnum.ExamSheet;
			frmSheetPicker.ShowDialog();
			if(!frmSheetPicker.IsDialogOK) {
				return;
			}
			SheetDef sheetDef;
			Sheet sheet=null;//only useful if not Terminal
			for(int i=0;i<frmSheetPicker.ListSheetDefsSelected.Count;i++) {
				sheetDef=frmSheetPicker.ListSheetDefsSelected[i];
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
				panelSheetPreview.Invalidate();//The sheet may have changed, so we refresh the preview.
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
				gridMain.SetSelected(gridMain.ListGridRows.Count-1,setValue:true);//Select the newly added row. Always last, since ordered by date.
				panelSheetPreview.Invalidate();//The new sheet is selected, so we refresh the preview.
			}
		}

		///<summary>Tries to send a single exam sheet. It attempts to do this by looking for a mobile device that has a logged in user that has also selected a patient.</summary>
		private void butSendToDevice_Click(object sender,EventArgs e) {
			if(PatNum==0){
				MsgBox.Show("Please select a patient first.");
				return;
			}
			if(_listSheets.IsNullOrEmpty()) {
				MsgBox.Show("The patient doesn't have any exam sheets. Please add one.");
				return;
			}
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetAll();
			//Get the device that we know has a clinical user logged in, plus they will have to have selected a patient
			MobileAppDevice mobileAppDevice=listMobileAppDevices.Where(x => x.UserNum!=0 && x.PatNum==PatNum).FirstOrDefault();
			if(mobileAppDevice==null || gridMain.SelectedIndices.Count()==0) {
				return;//Nothing is selected, but the button was some how still enabled
			}
			Sheet sheetSelected=gridMain.SelectedTag<Sheet>();
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
			if(MobileNotifications.ODT_ExamSheet(PatNum,sheetSelected.SheetNum,mobileAppDevice.MobileAppDeviceNum,
				out string errorMessge)) 
			{
				MsgBox.Show(this,$"Exam sheet sent to device: {mobileAppDevice.DeviceName}");
				return;
			}
			MsgBox.Show($"Error sending the exam sheet: {errorMessge}");
		}

	}
}