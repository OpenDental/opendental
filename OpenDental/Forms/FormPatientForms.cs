using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.SheetFramework;

namespace OpenDental {
	public partial class FormPatientForms:FormODBase {
		private DataTable _table;
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
			Patient patient=Patients.GetLim(PatNum);
			Text=Lan.g(this,"Patient Forms for")+" "+patient.GetNameFL();
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
			long sheetNumSelected=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				sheetNumSelected=PIn.Long(_table.Rows[gridMain.GetSelectedIndex()]["SheetNum"].ToString());
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),42);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Kiosk"),55,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),210);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Image Category"),120);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_table=Sheets.GetPatientFormsTable(PatNum);
			for(int i=0;i<_table.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(_table.Rows[i]["date"].ToString());
				row.Cells.Add(_table.Rows[i]["time"].ToString());
				row.Cells.Add(_table.Rows[i]["showInTerminal"].ToString());
				row.Cells.Add(_table.Rows[i]["description"].ToString());
				row.Cells.Add(_table.Rows[i]["imageCat"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(sheetNumSelected!=0) {
				for(int i=0;i<_table.Rows.Count;i++) {
					if(_table.Rows[i]["SheetNum"].ToString()==sheetNumSelected.ToString()) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Images
			//Hold onto docNum so Image module refresh persists selection when closing FormPatientForms.
			DocNum=PIn.Long(_table.Rows[e.Row]["DocNum"].ToString());//Set to 0 if not a Document, i.e. a Sheet.
			if(DocNum!=0) {
				if(!Security.IsAuthorized(Permissions.ImagingModule)) {
					return;
				}
				GotoModule.GotoImage(PatNum,DocNum); 
				return;
			}
			//Sheets
			long sheetNum=PIn.Long(_table.Rows[e.Row]["SheetNum"].ToString());
			Sheet sheet=Sheets.GetSheet(sheetNum);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void menuItemSheets_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetDefs formSheetDefs=new FormSheetDefs();
			formSheetDefs.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sheets");
			FillGrid();
		}

		private void menuItemImageCats_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
				return;
			}
			using FormDefinitions formDefinitions=new FormDefinitions(DefCat.ImageCats);
			formDefinitions.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.DefEdit,0,"Defs");
			FillGrid();
		}

		private void menuItemOptions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetSetup formSheetSetup=new FormSheetSetup();
			formSheetSetup.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"ShowForms");
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormSheetPicker formSheetPicker=new FormSheetPicker();
			formSheetPicker.SheetType=SheetTypeEnum.PatientForm;
			formSheetPicker.ShowDialog();
			if(formSheetPicker.DialogResult!=DialogResult.OK) {
				return;
			}
			SheetDef sheetDef;
			Sheet sheet=null;//only useful if not Terminal
			bool isPatUsingEClipboard=MobileAppDevices.PatientIsAlreadyUsingDevice(PatNum);
			for(int i=0;i<formSheetPicker.ListSheetDefsSelected.Count;i++) {
				sheetDef=formSheetPicker.ListSheetDefsSelected[i];
				if(formSheetPicker.DoTerminalSend && isPatUsingEClipboard && !sheetDef.HasMobileLayout) {
					if(!MsgBox.Show(MsgBoxButtons.YesNo,$"The patient is currently using an eClipboard to fill out forms, but the " +
						$"{sheetDef.Description} sheet does not have a mobile layout and cannot be used with eClipboard. " +
						$"If you add this form to the patient's list it will not be shown in eClipboard. Do you still want to add this form?")) {
						continue;
					}
				}
				sheet=SheetUtil.CreateSheet(sheetDef,PatNum);
				//Will display FormApptsOther for the user to select an appointment or procedures to display on the sheet.
				if(!SheetUtilL.SetApptProcParamsForSheet(sheet,sheetDef,PatNum)) {
					return;
				}
				SheetParameter.SetParameter(sheet,"PatNum",PatNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet);
				if(formSheetPicker.DoTerminalSend) {
					sheet.InternalNote="";//because null not ok
					sheet.ShowInTerminal=(byte)(Sheets.GetBiggestShowInTerminal(PatNum)+1);
					Sheets.SaveNewSheet(sheet);//save each sheet.
					//Push new sheet to eClipboard.
					if(isPatUsingEClipboard && sheetDef.HasMobileLayout) {
						OpenDentBusiness.WebTypes.PushNotificationUtils.CI_AddSheet(sheet.PatNum,sheet.SheetNum);
					}
				}
			}
			if(formSheetPicker.DoTerminalSend) {
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
			if(_table.Select().All(x => x["showInTerminal"].ToString()=="")) {
				MsgBox.Show(this,"No forms for this patient are set to show in the kiosk.");
				return;
			}
			if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0) {
				MsgBox.Show(this,"Cannot open kiosk unless process signal interval is set. To set it, go to Setup > Miscellaneous.");
				return;
			}
			if(ODBuild.IsWeb()) {
				//Thinfinity messes up window ordering so sometimes FormOpenDental is visible in Kiosk mode.
				for(int i=0;i<Application.OpenForms.Count;i++) {
					Application.OpenForms[i].Visible=false;
				}
			}
			using FormTerminal formTerminal=new FormTerminal();
			formTerminal.IsSimpleMode=true;
			formTerminal.PatNum=PatNum;
			formTerminal.ShowDialog();
			if(ODBuild.IsWeb()) {
				for(int i=0;i<Application.OpenForms.Count;i++) {
					Application.OpenForms[i].Visible=true;
				}
			}
			FillGrid();
		}

		private void butPreFill_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length !=1) {
				MsgBox.Show(this,"Please select one completed sheet from the list above first.");
				return;
			}
			long sheetNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[0]]["SheetNum"].ToString());
			if(sheetNum==0) {
				MsgBox.Show(this,"Must select a sheet.");
				return;
			}
			Sheet sheet=Sheets.GetSheet(sheetNum);
			if(sheet==null) {
				MsgBox.Show(this,"The selected sheet has been deleted by another workstation.");
				return;
			}
			SheetDef sheetDefOriginal=SheetDefs.GetSheetDef(sheet.SheetDefNum,hasExceptions:false);
			if(sheetDefOriginal==null) {
				//We could not find the sheetDef which this sheet was based on.
				//Prompt the user, asking if they would like to manually select a sheet def.
				if(!MsgBox.Show(this, MsgBoxButtons.YesNo, "Sheet Def not found. Unable to pre-fill. Would you like to select the correct Sheet Def manually?")) {
					return;//If the user chose not to pick a sheetDef, return.
				}
				//Find SheetDefs of the same type as the sheet we are pre-filling, and allow the user to select one.
				List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(sheet.SheetType);
				using FormSheetPicker formSheetPicker=new FormSheetPicker();
				formSheetPicker.SheetType=sheet.SheetType;
				formSheetPicker.ListSheetDefs=listSheetDefs;
				formSheetPicker.IsPreFill=true;
				formSheetPicker.ShowDialog();
				if(formSheetPicker.DialogResult!=DialogResult.OK) {
					return;//If the user cancelled, return.
				}
				sheetDefOriginal=formSheetPicker.ListSheetDefsSelected.First();
				sheet.SheetDefNum=sheetDefOriginal.SheetDefNum;
			}
			Sheet sheetNew=Sheets.PreFillSheetFromPreviousAndDatabase(sheetDefOriginal,sheet);
			sheetNew.IsNew=true;
			using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheetNew);
			formSheetFillEdit.ShowDialog();
			//If they press ok, the form is inserted, so refresh the grid, make a security log, and select the new entry.
			if(formSheetFillEdit.DialogResult==DialogResult.OK) {
				SecurityLogs.MakeLogEntry(Permissions.Copy,PatNum,"Patient form "+sheet.Description+" from "+sheet.DateTimeSheet.ToString()+" copied via Pre-Fill");
				FillGrid();
				//Select the newly added sheet.
				for(int i=0;i<_table.Rows.Count;i++) {
					if(_table.Rows[i]["SheetNum"].ToString()==sheetNew.SheetNum.ToString()) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length !=1) {
				MsgBox.Show(this,"Please select one completed sheet from the list above first.");
				return;
			}
			long sheetNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[0]]["SheetNum"].ToString());
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
			using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheet2);
			formSheetFillEdit.ShowDialog();
			if(formSheetFillEdit.DialogResult==DialogResult.OK || formSheetFillEdit.DidChangeSheet) {
				FillGrid();
				for(int i=0;i<_table.Rows.Count;i++){
					if(_table.Rows[i]["SheetNum"].ToString()==sheet2.SheetNum.ToString()){
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
			long sheetNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[0]]["SheetNum"].ToString());
			long docNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[0]]["DocNum"].ToString());
			Document document=null;
			if(docNum!=0) {
				document=Documents.GetByNum(docNum);
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
			using FormSheetImport formSheetImport=new FormSheetImport();
			formSheetImport.SheetCur=sheet;
			formSheetImport.DocCur=document;
			formSheetImport.ShowDialog();
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