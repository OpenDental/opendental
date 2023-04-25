using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebForms;

namespace OpenDental {
	public partial class FormWebForms:FormODBase {
		private WebForms_Preference _webForms_Preference=new WebForms_Preference();

		/// <summary>
		/// This Form does 3 things: 
		/// 1) Retrieve data of filled out web forms from a web service and convert them into sheets and patients. Using the first name, last name and birth date it will check for existing patients. If an existing patient is found a new sheet is created. If no patient is found, a  patient and a sheet is created.
		/// 2) Send a list of the Sheets that have been created to the Server for deletion.
		/// 3) Show all the sheets that have been created in 1) using a date filter and a clinic filter.
		/// </summary>
		public FormWebForms() {
			InitializeComponent();
			InitializeLayoutManager();
			gridMain.ContextMenu=menuWebFormsRight;
			Lan.F(this);
		}

		private void FormWebForms_Load(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			if(!PrefC.HasClinicsEnabled) {
				LayoutManager.MoveWidth(groupFilters,LayoutManager.Scale(240));//Shrink to better fit just the date range pickers.
			}
			else if(Clinics.ClinicNum==0) {
				comboClinics.IsAllSelected=true;
			}
			if(WebForms_Preferences.TryGetPreference(out _webForms_Preference)) {
				if(string.IsNullOrEmpty(_webForms_Preference.CultureName)){//Just in case.
					_webForms_Preference.CultureName=System.Globalization.CultureInfo.CurrentCulture.Name;
				}
			}
			if(PrefC.GetBool(PrefName.WebFormsDownloadAutomcatically)) {
				label1.Text=Lan.g(this,"Most forms are now automatically downloaded and attached."
					+"\r\nThis button is just for forms that couldn't be matched to an existing patient.");
			}
			LayoutMenu();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		/// <summary>
		/// </summary>
		private void FillGrid() {
			DateTime dateFrom=DateTime.Today;
			DateTime dateTo=DateTime.Today;
			dateFrom=PIn.Date(textDateStart.Text);//handles blank or invalid
			if(textDateEnd.Text!="") {//if it is blank, default to today
				dateTo=PIn.Date(textDateEnd.Text);
			}
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),42);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient Last Name"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient First Name"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),240);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Deleted"),40,HorizontalAlignment.Center){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			DataTable table=Sheets.GetWebFormSheetsTable(dateFrom,dateTo,comboClinics.ListSelectedClinicNums);
			for(int i=0;i<table.Rows.Count;i++) {
				long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				long sheetNum=PIn.Long(table.Rows[i]["SheetNum"].ToString());
				Patient patient=Patients.GetPat(patNum);
				if(patient==null) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(table.Rows[i]["date"].ToString());
				row.Cells.Add(table.Rows[i]["time"].ToString());
				row.Cells.Add(patient.LName);
				row.Cells.Add(patient.FName);
				row.Cells.Add(table.Rows[i]["description"].ToString());
				row.Cells.Add(table.Rows[i]["IsDeleted"].ToString()=="0" ? "" : "X");
				row.Tag=sheetNum;
				gridMain.ListGridRows.Add(row);
			} 
			gridMain.EndUpdate();
		}


		/// <summary>
		///  This method is used only for testing with security certificates that has problems.
		/// </summary>
		private void IgnoreCertificateErrors() {
			///the line below will allow the code to continue by not throwing an exception.
			///It will accept the security certificate if there is a problem with the security certificate.
			ServicePointManager.ServerCertificateValidationCallback+=CertCallBack;
		}

		private bool CertCallBack(object sender,X509Certificate x509Certificate,X509Chain x509Chain,SslPolicyErrors sslPolicyErrors) {
			///do stuff here and return true or false accordingly.
			///In this particular case it always returns true i.e accepts any certificate.
			if(sslPolicyErrors==SslPolicyErrors.None) {
				return true;
			}
			// the sample below allows expired certificates
			for(int i = 0;i<x509Chain.ChainStatus.Length;i++) {
				// allows expired certificates
				if(x509Chain.ChainStatus[i].ToString().ToLower()=="NotTimeValid".ToLower()) {
					return true;
				}
			}
			return true;
		}

		/*
		/// <summary>
		/// </summary>
		private Sheet CreateSheet(long PatNum,WebForms_Sheet webFormSheet) {
			Sheet newSheet=null;
			SheetDef sheetDef=new SheetDef((SheetTypeEnum)webFormSheet.SheetType);
			newSheet=SheetUtil.CreateSheet(sheetDef,PatNum);
			SheetParameter.SetParameter(newSheet,"PatNum",PatNum);
			newSheet.DateTimeSheet=webFormSheet.DateTimeSheet;
			newSheet.Description=webFormSheet.Description;
			newSheet.Height=webFormSheet.Height;
			newSheet.Width=webFormSheet.Width;
			newSheet.FontName=webFormSheet.FontName;
			newSheet.FontSize=webFormSheet.FontSize;
			newSheet.SheetType=(SheetTypeEnum)webFormSheet.SheetType;
			newSheet.IsLandscape=webFormSheet.IsLandscape;
			newSheet.InternalNote="";
			newSheet.IsWebForm=true;
			//loop through each variable in a single sheetfield
			for(int i = 0;i<webFormSheet.SheetFields.Count;i++) {
				SheetField sheetfield=new SheetField();
				sheetfield.FieldName=webFormSheet.SheetFields[i].FieldName;
				sheetfield.FieldType=webFormSheet.SheetFields[i].FieldType;
				sheetfield.FontIsBold=webFormSheet.SheetFields[i].FontIsBold;
				sheetfield.FontName=webFormSheet.SheetFields[i].FontName;
				sheetfield.FontSize=webFormSheet.SheetFields[i].FontSize;
				sheetfield.Height=webFormSheet.SheetFields[i].Height;
				sheetfield.Width=webFormSheet.SheetFields[i].Width;
				sheetfield.XPos=webFormSheet.SheetFields[i].XPos;
				sheetfield.YPos=webFormSheet.SheetFields[i].YPos;
				sheetfield.IsRequired=webFormSheet.SheetFields[i].IsRequired;
				sheetfield.TabOrder=webFormSheet.SheetFields[i].TabOrder;
				sheetfield.ReportableName=webFormSheet.SheetFields[i].ReportableName;
				sheetfield.RadioButtonGroup=webFormSheet.SheetFields[i].RadioButtonGroup;
				sheetfield.RadioButtonValue=webFormSheet.SheetFields[i].RadioButtonValue;
				sheetfield.GrowthBehavior=webFormSheet.SheetFields[i].GrowthBehavior;
				sheetfield.FieldValue=webFormSheet.SheetFields[i].FieldValue;
				sheetfield.TextAlign=webFormSheet.SheetFields[i].TextAlign;
				sheetfield.ItemColor=webFormSheet.SheetFields[i].ItemColor;
				newSheet.SheetFields.Add(sheetfield);
			}
			try {
				Sheets.SaveNewSheet(newSheet);
			}
			catch(Exception e) {
				gridMain.EndUpdate();
				MessageBox.Show(e.Message);
			}
			return newSheet;
		}*/

		private void butRetrieve_Click(object sender,System.EventArgs e) {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid() || !textBatchSize.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string strMessage=Lan.g(this,"Done");
			if(!WebFormL.TryRetrieveAndSaveData(_webForms_Preference.CultureName,comboClinics.ListSelectedClinicNums,textBatchSize.Value,
				out List<string> listMsgsCEMT,out List<string> listMsgsWebForms,out List<String> listMsgsDownloadedWebForms))
			{
				strMessage=Lan.g(this,"Retrieval process exited prematurely.");
			}
			if(!listMsgsCEMT.IsNullOrEmpty()) {
				strMessage+="\r\n\r\n"+string.Join("\r\n",listMsgsCEMT);
			}
			if(!listMsgsWebForms.IsNullOrEmpty()) {
				strMessage+="\r\n\r\n"+string.Join("\r\n",listMsgsWebForms);
			}
			if(strMessage.Length <= 50) {
				MsgBox.Show(strMessage);
				FillGrid();
				Cursor=Cursors.Default;
				return;
			}
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(strMessage);
			msgBoxCopyPaste.ShowDialog();
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void butToday_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormWebFormSetup formWebFormSetup=new FormWebFormSetup();
			formWebFormSetup.ShowDialog();
			if(PrefC.GetBool(PrefName.WebFormsDownloadAutomcatically)) {
				label1.Text=Lan.g(this,"Most forms are now automatically downloaded and attached."
					+"\r\nThis button is just for forms that couldn't be matched to an existing patient.");
			}
			else {
				label1.Text=Lan.g(this,"Retrieved forms are automatically attached to the correct patient if they are a match.");
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Web Forms Setup");
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			long sheetNum=(long)gridMain.ListGridRows[e.Row].Tag;
			Sheet sheet=Sheets.GetSheet(sheetNum);
			GotoModule.GotoFamily(sheet.PatNum);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long sheetNum=(long)gridMain.ListGridRows[e.Row].Tag;
			Sheet sheet=Sheets.GetSheet(sheetNum);
			FormSheetFillEdit.ShowForm(sheet,delegate { FillGrid(); }); //We must refresh the grid because the web form clicked might have been deleted by the user.
		}

		/*
		private void menuItemViewSheet_Click(object sender,EventArgs e) {
			long sheetNum=(long)gridMain.Rows[gridMain.SelectedIndices[0]].Tag;
			Sheet sheet=Sheets.GetSheet(sheetNum);
			using FormSheetFillEdit FormSF=new FormSheetFillEdit(sheet);
			FormSF.ShowDialog();
		}

		private void menuItemImportSheet_Click(object sender,EventArgs e) {
			long sheetNum=(long)gridMain.Rows[gridMain.SelectedIndices[0]].Tag;
			Sheet sheet=Sheets.GetSheet(sheetNum);
			usning FormSheetImport formSI=new FormSheetImport();
			formSI.SheetCur=sheet;
			formSI.ShowDialog();
		}*/

		private void menuWebFormsRight_Popup(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				menuItemViewAllSheets.Visible=false;
				return;
			}
			menuItemViewAllSheets.Visible=true;
		}

		private void menuItemViewAllSheets_Click(object sender,EventArgs e) {
			long sheetNum=(long)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			Sheet sheet=Sheets.GetSheet(sheetNum);
			using FormPatientForms formPatientForms=new FormPatientForms();
			formPatientForms.PatNum=sheet.PatNum;
			formPatientForms.ShowDialog();
		}

		private void ComboClinics_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}