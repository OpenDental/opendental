using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using System.Threading;
using CodeBase;
using OpenDentBusiness.WebTypes.WebForms;

namespace OpenDental {
	public partial class FormWebForms:FormODBase {
		///<summary>List of all clinics for the user currently logged in.  Includes "Headquarters".  Filled on load, will be null if clinics turned off.</summary>
		private List<Clinic> _listClinics;
		private WebForms_Preference _webFormPref=new WebForms_Preference();

		/// <summary>
		/// This Form does 3 things: 
		/// 1) Retrieve data of filled out web forms from a web service and convert them into sheets and patients. Using the first name, last name and birth date it will check for existing patients. If an existing patient is found a new sheet is created. If no patient is found, a  patient and a sheet is created.
		/// 2) Send a list of the Sheets that have been created to the Server for deletion.
		/// 3)Show all the sheets that have been created in 1) using a date filter and a clinic filter.
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
			if(WebForms_Preferences.TryGetPreference(out _webFormPref)) {
				if(string.IsNullOrEmpty(_webFormPref.CultureName)){//Just in case.
					_webFormPref.CultureName=System.Globalization.CultureInfo.CurrentCulture.Name;
				}
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
			try {
				dateFrom=PIn.Date(textDateStart.Text);//handles blank
				if(textDateEnd.Text!=""){//if it is blank, default to today
					dateTo=PIn.Date(textDateEnd.Text);
				}
			}
			catch{
				MsgBox.Show(this,"Invalid date");
				return;
			}
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),42);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient Last Name"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient First Name"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),240);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Deleted"),40,HorizontalAlignment.Center){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			DataTable table=Sheets.GetWebFormSheetsTable(dateFrom,dateTo,comboClinics.ListSelectedClinicNums);
			for(int i=0;i<table.Rows.Count;i++) {
				long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				long sheetNum=PIn.Long(table.Rows[i]["SheetNum"].ToString());
				Patient pat=Patients.GetPat(patNum);
				if(pat!=null) {
					GridRow row=new GridRow();
					row.Cells.Add(table.Rows[i]["date"].ToString());
					row.Cells.Add(table.Rows[i]["time"].ToString());
					row.Cells.Add(pat.LName);
					row.Cells.Add(pat.FName);
					row.Cells.Add(table.Rows[i]["description"].ToString());
					row.Cells.Add(table.Rows[i]["IsDeleted"].ToString()=="0" ? "" : "X");
					row.Tag=sheetNum;
					gridMain.ListGridRows.Add(row);
				}
			} 
			gridMain.EndUpdate();
		}


		/// <summary>
		///  This method is used only for testing with security certificates that has problems.
		/// </summary>
		private void IgnoreCertificateErrors() {
			///the line below will allow the code to continue by not throwing an exception.
			///It will accept the security certificate if there is a problem with the security certificate.
			System.Net.ServicePointManager.ServerCertificateValidationCallback+=
			delegate(object sender,System.Security.Cryptography.X509Certificates.X509Certificate certificate,
									System.Security.Cryptography.X509Certificates.X509Chain chain,
									System.Net.Security.SslPolicyErrors sslPolicyErrors) {
				///do stuff here and return true or false accordingly.
				///In this particular case it always returns true i.e accepts any certificate.
				/* sample code 
				if(sslPolicyErrors==System.Net.Security.SslPolicyErrors.None) return true;
				// the sample below allows expired certificates
				foreach(X509ChainStatus s in chain.ChainStatus) {
					// allows expired certificates
					if(string.Equals(s.Status.ToString(),"NotTimeValid",
						StringComparison.OrdinalIgnoreCase)) {
						return true;
					}						
				}*/
				return true;
			};
		}

		/// <summary>
		/// </summary>
		private Sheet CreateSheet(long PatNum,WebForms_Sheet webFormSheet) {
			Sheet newSheet=null;
			try{
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
					foreach(WebForms_SheetField field in webFormSheet.SheetFields) {
						SheetField sheetfield=new SheetField();
						sheetfield.FieldName=field.FieldName;
						sheetfield.FieldType=field.FieldType;
						sheetfield.FontIsBold=field.FontIsBold;
						sheetfield.FontName=field.FontName;
						sheetfield.FontSize=field.FontSize;
						sheetfield.Height=field.Height;
						sheetfield.Width=field.Width;
						sheetfield.XPos=field.XPos;
						sheetfield.YPos=field.YPos;
						sheetfield.IsRequired=field.IsRequired;
						sheetfield.TabOrder=field.TabOrder;
						sheetfield.ReportableName=field.ReportableName;
						sheetfield.RadioButtonGroup=field.RadioButtonGroup;
						sheetfield.RadioButtonValue=field.RadioButtonValue;
						sheetfield.GrowthBehavior=field.GrowthBehavior;
						sheetfield.FieldValue=field.FieldValue;
						sheetfield.TextAlign=field.TextAlign;
						sheetfield.ItemColor=field.ItemColor;
						newSheet.SheetFields.Add(sheetfield);
					}// end of j loop
					Sheets.SaveNewSheet(newSheet);
					return newSheet;
			}
			catch(Exception e) {
				gridMain.EndUpdate();
				MessageBox.Show(e.Message);
			}
			return newSheet;
		}

		private void butRetrieve_Click(object sender,System.EventArgs e) {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			//this.backgroundWorker1.RunWorkerAsync(); call this  method if theread is to be used later.
			string strMsg=WebFormL.RetrieveAndSaveData(_webFormPref.CultureName); // if a thread is used this method will go into backgroundWorker1_DoWork
			if(!string.IsNullOrEmpty(strMsg)) {
				gridMain.EndUpdate();
				MsgBox.Show(strMsg);
			}
			FillGrid(); // if a thread is used this method will go into backgroundWorker1_RunWorkerCompleted
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
			try {
				using FormWebFormSetup formW=new FormWebFormSetup();
				formW.ShowDialog();
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Web Forms Setup");
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
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
			}
			else {
				menuItemViewAllSheets.Visible=true;
			}
		}

		private void menuItemViewAllSheets_Click(object sender,EventArgs e) {
			long sheetNum=(long)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag;
			Sheet sheet=Sheets.GetSheet(sheetNum);
			using FormPatientForms formP=new FormPatientForms();
			formP.PatNum=sheet.PatNum;
			formP.ShowDialog();
		}

		private void ComboClinics_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}