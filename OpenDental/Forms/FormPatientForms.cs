using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatientForms:FormODBase {
		DataTable table;
		public long PatNum;
		///<summary>Indicates the most recently selected Document.DocNum</summary>
		public long DocNum;

		public FormPatientForms() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			DocNum=0;
		}

		private void FormPatientForms_Load(object sender,EventArgs e) {
			Patient pat=Patients.GetLim(PatNum);
			Text=Lan.g(this,"Patient Forms for")+" "+pat.GetNameFL();
			LayoutMenu();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			MenuItemOD menuItemSetup=new MenuItemOD("Setup");
			menuMain.Add(menuItemSetup);
			menuItemSetup.Add("Sheets",menuItemSheets_Click);
			menuItemSetup.Add("Image Categories",menuItemImageCats_Click);
			menuItemSetup.Add("Options",menuItemOptions_Click);
			menuMain.EndUpdate();
		}

		private void FillGrid(){
			//if a sheet is selected, remember it
			long selectedSheetNum=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				selectedSheetNum=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["SheetNum"].ToString());
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),42);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Kiosk"),55,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),210);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Image Category"),120);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			table=Sheets.GetPatientFormsTable(PatNum);
			for(int i=0;i<table.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["date"].ToString());
				row.Cells.Add(table.Rows[i]["time"].ToString());
				row.Cells.Add(table.Rows[i]["showInTerminal"].ToString());
				row.Cells.Add(table.Rows[i]["description"].ToString());
				row.Cells.Add(table.Rows[i]["imageCat"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(selectedSheetNum!=0) {
				for(int i=0;i<table.Rows.Count;i++) {
					if(table.Rows[i]["SheetNum"].ToString()==selectedSheetNum.ToString()) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Images
			//Hold onto docNum so Image module refresh persists selection when closing FormPatientForms.
			DocNum=PIn.Long(table.Rows[e.Row]["DocNum"].ToString());//Set to 0 if not a Document, i.e. a Sheet.
			if(DocNum!=0) {
				GotoModule.GotoImage(PatNum,DocNum); 
				return;
			}
			//Sheets
			long sheetNum=PIn.Long(table.Rows[e.Row]["SheetNum"].ToString());
			Sheet sheet=Sheets.GetSheet(sheetNum);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void menuItemSheets_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetDefs FormSD=new FormSheetDefs();
			FormSD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sheets");
			FillGrid();
		}

		private void menuItemImageCats_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDefinitions formD=new FormDefinitions(DefCat.ImageCats);
			formD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Defs");
			FillGrid();
		}

		private void menuItemOptions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetSetup formSS=new FormSheetSetup();
			formSS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"ShowForms");
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormSheetPicker FormS=new FormSheetPicker();
			FormS.SheetType=SheetTypeEnum.PatientForm;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			SheetDef sheetDef;
			Sheet sheet=null;//only useful if not Terminal
			bool isPatUsingEClipboard=MobileAppDevices.PatientIsAlreadyUsingDevice(PatNum);
			for(int i=0;i<FormS.SelectedSheetDefs.Count;i++) {
				sheetDef=FormS.SelectedSheetDefs[i];
				if(FormS.TerminalSend && isPatUsingEClipboard && !sheetDef.HasMobileLayout) {
					if(!MsgBox.Show(MsgBoxButtons.YesNo,$"The patient is currently using an eClipboard to fill out forms, but the " +
						$"{sheetDef.Description} sheet does not have a mobile layout and cannot be used with eClipboard. " +
						$"If you add this form to the patient's list it will not be shown in eClipboard. Do you still want to add this form?")) {
						continue;
					}
				}
				sheet=SheetUtil.CreateSheet(sheetDef,PatNum);
				SheetParameter.SetParameter(sheet,"PatNum",PatNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
				if(FormS.TerminalSend) {
					sheet.InternalNote="";//because null not ok
					sheet.ShowInTerminal=(byte)(Sheets.GetBiggestShowInTerminal(PatNum)+1);
					Sheets.SaveNewSheet(sheet);//save each sheet.
					//Push new sheet to eClipboard.
					if(isPatUsingEClipboard && sheetDef.HasMobileLayout) {
						OpenDentBusiness.WebTypes.PushNotificationUtils.CI_AddSheet(sheet.PatNum,sheet.SheetNum);
					}
				}
			}
			if(FormS.TerminalSend) {
				//do not show a dialog now.  User will need to click the terminal button.
				FillGrid();
				Signalods.SetInvalid(InvalidType.Kiosk);
			}
			else if(sheet!=null) {
				FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
			}
		}

		private void butTerminal_Click(object sender,EventArgs e) {
			//<List>.All() returns true for an empty list.
			if(table.Select().All(x => x["showInTerminal"].ToString()=="")) {
				MsgBox.Show(this,"No forms for this patient are set to show in the kiosk.");
				return;
			}
			if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0) {
				MsgBox.Show(this,"Cannot open kiosk unless process signal interval is set. To set it, go to Setup > Miscellaneous.");
				return;
			}
			if(ODBuild.IsWeb()) {
				//Thinfinity messes up window ordering so sometimes FormOpenDental is visible in Kiosk mode.
				foreach(Form form in Application.OpenForms) {
					form.Visible=false;
				}
			}
			using FormTerminal formT=new FormTerminal();
			formT.IsSimpleMode=true;
			formT.PatNum=PatNum;
			formT.ShowDialog();
			if(ODBuild.IsWeb()) {
				foreach(Form form in Application.OpenForms) {
					form.Visible=true;
				}
			}
			FillGrid();
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length !=1) {
				MsgBox.Show(this,"Please select one completed sheet from the list above first.");
				return;
			}
			long sheetNum=PIn.Long(table.Rows[gridMain.SelectedIndices[0]]["SheetNum"].ToString());
			if(sheetNum==0) {
				MsgBox.Show(this,"Must select a sheet.");
				return;
			}
			Sheet sheet=Sheets.GetSheet(sheetNum);
			Sheet sheet2=sheet.Copy();
			sheet2.DateTimeSheet=DateTime.Now;
			sheet2.SheetFields=new List<SheetField>(sheet.SheetFields);
			for(int i=0;i<sheet2.SheetFields.Count;i++){
				sheet2.SheetFields[i].IsNew=true;
				if(sheet2.SheetFields[i].FieldType==SheetFieldType.SigBox){
					sheet2.SheetFields[i].FieldValue="";//clear signatures
				}
				//no need to set SheetNums here.  That's done from inside FormSheetFillEdit
			}
			sheet2.IsNew=true;
			using FormSheetFillEdit FormSF=new FormSheetFillEdit(sheet2);
			FormSF.ShowDialog();
			if(FormSF.DialogResult==DialogResult.OK || FormSF.DidChangeSheet) {
				FillGrid();
				for(int i=0;i<table.Rows.Count;i++){
					if(table.Rows[i]["SheetNum"].ToString()==sheet2.SheetNum.ToString()){
						gridMain.SetSelected(i,true);
					}
				}
				SecurityLogs.MakeLogEntry(Permissions.Copy,PatNum,"Patient form "+sheet.Description+" from "+sheet.DateTimeSheet.ToString()+" copied");
			}
		}
		
		private void butImport_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length !=1) {
				MsgBox.Show(this,"Please select one completed form from the list above first.");
				return;
			}
			long sheetNum=PIn.Long(table.Rows[gridMain.SelectedIndices[0]]["SheetNum"].ToString());
			long docNum=PIn.Long(table.Rows[gridMain.SelectedIndices[0]]["DocNum"].ToString());
			Document doc=null;
			if(docNum!=0) {
				doc=Documents.GetByNum(docNum);
				//Pdf importing broke with dot net 4.0 and was enver reimplemented.
				//See FormSheetImport.Load() region Acro 
				//string extens=Path.GetExtension(doc.FileName);
				//if(extens.ToLower()!=".pdf") {
				//	MsgBox.Show(this,"Only pdf's and sheets can be imported into the database.");
				//	return;
				//}
			}
			Sheet sheet=null;
			if(sheetNum!=0) {
				sheet=Sheets.GetSheet(sheetNum);
				if(!SheetDefs.IsWebFormAllowed(sheet.SheetType)) {
					MsgBox.Show(this,"For now, only sheets of type 'PatientForm' and 'MedicalHistory' can be imported.");
					return;
				}
			}
			if(sheet==null) {
				MsgBox.Show(this,"Only sheets can be imported into the database.");
				return;
			}
			using FormSheetImport formSI=new FormSheetImport();
			formSI.SheetCur=sheet;
			formSI.DocCur=doc;
			formSI.ShowDialog();
			//No need to refresh grid because no changes could have been made.
		}

		/// <summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(((FormSheetFillEdit)sender).DialogResult==DialogResult.OK || ((FormSheetFillEdit)sender).DidChangeSheet) {
				FillGrid();
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

		

		

		

		

		

		

		

		
	}
}