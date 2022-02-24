using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.SheetFramework;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace OpenDental {
	class SheetPrintingJob {
		#region Fields
		///<summary>Used when printing statements that use the Statements use Sheets feature.  Pdf printing does not use this variable.</summary>
		private DataSet _dataSet;
		///<summary>Used when printing ERAs to speed up the printing of the EraClaimsPaid grid.</summary>
		private int _idxPreClaimPaidPrinted=0;
		private bool _isPrinting=false;
		///<summary>If not a batch, then there will just be one sheet in the list.</summary>
		private List<Sheet> _listSheets;
		private MedLab _medLab;
		///<summary>Pages printed on current sheet.</summary>
		private int _pagesPrinted;
		#if DEBUG
		private bool _printCalibration=false;
		#endif
		///<summary>Print margin of the default printer. only used in page break calulations, and only top and bottom are used.</summary>
		private static Margins _printMargin=new Margins(0,0,40,60);//jordan static only because it's an unchanging val.
		///<summary>If there is only one sheet, then this will stay 0.</Summary>
		private int _sheetsPrinted;
		private Statement _stmt;
		private int _yPosPrevious;
		///<summary>Used for determining page breaks. When moving to next page, use this Y value to determine the next field to print.</summary>
		private int _yPosPrint;
		#endregion Fields

		#region Properties
		///<summary>The treatment finder needs to be able to clear out the pages printed variable before it prints a batch.</summary>
		public int PagesPrinted {
			get {
				return _pagesPrinted;
			}
			set {
				_pagesPrinted=value;
			}
		}
		#endregion Properties

		#region Public Methods - Print
		///<summary>If printing a statement, use the polymorphism that takes a DataSet otherwise this method will make another call to the db.</summary>
		public void Print(Sheet sheet,int copies=1,bool isRxControlled=false,Statement stmt=null,MedLab medLab=null,bool isPrintDocument=true,bool isPreviewMode=false) {
			if(sheet.SheetType==SheetTypeEnum.Statement && stmt!=null) {
				//This should never get hit.  This line of code is here just in case I forgot to update a random spot in our code.
				//Worst case scenario we will end up calling the database a few extra times for the same data set.
				//It use to call this method many, many times so anything is an improvement at this point.
				if(stmt.SuperFamily!=0) {
					_dataSet=AccountModules.GetSuperFamAccount(stmt,doShowHiddenPaySplits:stmt.IsReceipt);
				}
				else {
					_dataSet=AccountModules.GetAccount(stmt.PatNum,stmt,doShowHiddenPaySplits:stmt.IsReceipt);
				}
			}
			Print(sheet,_dataSet,copies,isRxControlled,stmt,medLab,isPrintDocument,isPreviewMode);
		}

		///<Summary>DataSet should be prefilled with AccountModules.GetAccount() before calling this method if printing a statement.  Returns null if document failed to print.</Summary>
		public void Print(Sheet sheet,DataSet dataSet,int copies=1,bool isRxControlled=false,Statement stmt=null,MedLab medLab=null,
			bool isPrintDocument=true,bool isPreviewMode=false)
		{
			try {
				//If we were unable to create the printdocument this will throw an exception.
				TryPrint(sheet,dataSet,copies,isRxControlled,stmt,medLab,isPrintDocument,isPreviewMode);
			}
			catch(InvalidPrinterException ex) {//Dont catch other exceptions so that customer calls and we can be aware.
				ex.DoNothing();//Error msg shown, see TryPrint(...)
			}
		}

		///<summary>Surround with try/catch.</summary>
		public void PrintBatch(List<Sheet> sheetBatch){
			//currently no validation for parameters in a batch because of the way it was created.
			//could validate field names here later.
			_listSheets=sheetBatch;
			_sheetsPrinted=0;
			_pagesPrinted=0;
			_yPosPrint=0;
			PrintSituation sit=PrintSituation.Default;
			switch(sheetBatch[0].SheetType){
				case SheetTypeEnum.LabelPatient:
				case SheetTypeEnum.LabelCarrier:
				case SheetTypeEnum.LabelReferral:
					sit=PrintSituation.LabelSingle;
					break;
				case SheetTypeEnum.ReferralSlip:
					sit=PrintSituation.Default;
					break;
				case SheetTypeEnum.RxMulti:
					sit=PrintSituation.RxMulti;
					break;
			}
			//Moved Calculate heights here because we need to caluclate height before printing, not while we are printing.
			foreach(Sheet s in _listSheets) {
				SheetUtil.CalculateHeights(s,null,null,_isPrinting,_printMargin.Top,_printMargin.Bottom);
			}
			Margins margins=new Margins(20,20,0,0);
			int pageCount=0;
			if(ODBuild.IsDebug()) {
				foreach(Sheet s in _listSheets) {
					//SetForceSinglePage(s);
					SheetUtil.CalculateHeights(s,null,null,_isPrinting,_printMargin.Top,_printMargin.Bottom);
					pageCount+=Sheets.CalculatePageCount(s,_printMargin);//(_forceSinglePage?1:Sheets.CalculatePageCount(s,_printMargin));
				}
			}
			else {
				foreach(Sheet s in _listSheets) {
					s.SheetFields.Sort(OpenDentBusiness.SheetFields.SortDrawingOrderLayers);
				}
				margins=new Margins(0,0,0,0);
			}
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,
				Lan.g(nameof(PrinterL),"Batch of")+" "+sheetBatch[0].Description+" "+Lan.g(nameof(PrinterL),"printed"),
				margins,
				pageCount,
				sit,
				PrintoutOrigin.AtMargin,
				(sheetBatch[0].IsLandscape?PrintoutOrientation.Landscape:PrintoutOrientation.Portrait)
			);
		}

		///<summary>Performs validation on each Rx before printing.  If one of the given Rx does not pass validation, then the entire batch is aborted, because it is easy for user to fix errors.</summary>
		public void PrintMultiRx(List<RxPat> listRxs) {
			SheetDef sheetDef=SheetDefs.GetInternalOrCustom(SheetInternalType.RxMulti);
			List<int> rxSheetCountList=GetSheetRxCount(sheetDef);//gets the number of rx available in the sheet
			if(sheetDef.Parameters.Count==0) {//adds parameters if internal sheet
				sheetDef.Parameters.Add(new SheetParameter(true,"ListRxNums"));
				sheetDef.Parameters.Add(new SheetParameter(true,"ListRxSheet"));
			}
			List<Sheet> batchRxList=new List<Sheet>();//list of sheets to be batch printed
			if(rxSheetCountList.Count==0) {
				MessageBox.Show(Lan.g("Sheets","MuitiRx sheet is invalid."
					+"Please visit the manual to see what output fields must be added to the MultiRx Sheet."));
				return;
			}
			//Sort RxPats into batches. rxSheetCount is most rx's we can print on one sheet.
			int batchSize=rxSheetCountList.Count;
			int batchIdx=0;
			List<List<RxPat>> batches=new List<List<RxPat>>();
			for(int i = 0;i<listRxs.Count;i++) {
				if(SheetPrinting.ValidateRxForSheet(listRxs[i])!="") {
					MsgBox.Show("Sheets","One or more of the selected prescriptions is missing information.\r\nPlease fix and try again.");
					return;
				}
				if(i>0 && i%batchSize==0) {
					batchIdx++;
				}
				if(i%batchSize==0) {
					batches.Add(new List<RxPat>());
				}
				batches[batchIdx].Add(listRxs[i]);
			}
			//Fill and add sheets to batchRxList to be printed
			foreach(List<RxPat> listBatch in batches) {
				Sheet sheet=SheetUtil.CreateSheet(sheetDef,listRxs[0].PatNum);
				SheetParameter.SetParameter(sheet,"ListRxNums",listBatch);
				SheetParameter.SetParameter(sheet,"ListRxSheet",rxSheetCountList);
				SheetFiller.FillFields(sheet);
				batchRxList.Add(sheet);
			}
			PrintBatch(batchRxList);
		}

		///<summary>Performs validation on the given Rx before printing.  Returns false if validation failed.</summary>
		public bool PrintRx(Sheet sheet,RxPat rx){
			string validationErrors=SheetPrinting.ValidateRxForSheet(rx);
			if(validationErrors!="") {
				MessageBox.Show(Lan.g("Sheets","Cannot print until missing info is fixed: ")+validationErrors);
				return false;
			}
			Print(sheet,1,rx.IsControlled);
			return true;
		}

		///<summary></summary>
		public void TryPrint(Sheet sheet, DataSet dataSet, int copies=1, bool isRxControlled=false,Statement stmt=null,MedLab medLab=null,bool isPrintDocument=true,bool isPreviewMode = false) {
			_dataSet=dataSet;
			//parameter null check moved to SheetFiller.
			//could validate field names here later.
			_stmt=stmt;
			_medLab=medLab;
			_isPrinting=true;
			_sheetsPrinted=0;
			_yPosPrint=0;// _printMargin.Top;
			PaperSize paperSize=null;//Default for ODprintout.
			if(sheet.SheetType==SheetTypeEnum.LabelPatient
				|| sheet.SheetType==SheetTypeEnum.LabelCarrier
				|| sheet.SheetType==SheetTypeEnum.LabelAppointment
				|| sheet.SheetType==SheetTypeEnum.LabelReferral)
				{//I think this causes problems for non-label sheet types.
				if(sheet.Width>0 && sheet.Height>0) {
					paperSize=new PaperSize("Default",sheet.Width,sheet.Height);
				}
			}
			PrintSituation sit=PrintSituation.Default;
			switch(sheet.SheetType){
				case SheetTypeEnum.LabelPatient:
				case SheetTypeEnum.LabelCarrier:
				case SheetTypeEnum.LabelReferral:
				case SheetTypeEnum.LabelAppointment:
					sit=PrintSituation.LabelSingle;
					break;
				case SheetTypeEnum.ReferralSlip:
					sit=PrintSituation.Default;
					break;
				case SheetTypeEnum.Rx:
					if(isRxControlled){
						sit=PrintSituation.RxControlled;
					}
					else{
						sit=PrintSituation.Rx;
					}
					break;
				case SheetTypeEnum.Statement:
					sit= PrintSituation.Statement;
					break;
				case SheetTypeEnum.TreatmentPlan:
					sit=PrintSituation.TPPerio;
					break;
			}
			Sheets.SetPageMargin(sheet,_printMargin);
			foreach(SheetField field in sheet.SheetFields) {//validate all signatures before modifying any of the text fields.
				if(!ListTools.In(field.FieldType,SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)) {
					continue;
				}
				field.SigKey=Sheets.GetSignatureKey(sheet);
			}
			if(stmt!=null) {//Because most of the time statements do not display the Fill Sheet window, so there is no opportunity to select a combobox.
				SheetUtil.SetDefaultValueForComboBoxes(sheet);
			}
			SheetUtil.CalculateHeights(sheet,_dataSet,_stmt,_isPrinting,_printMargin.Top,_printMargin.Bottom,_medLab);
			_listSheets=new List<Sheet>();
			for(int i=0;i<copies;i++) {
				_listSheets.Add(sheet.Copy());
			}
			if(ODBuild.IsDebug()) {
				isPreviewMode=true;
			}
			int pageCount=0;//Default for ODprintout.
			if(isPreviewMode) {//pageCount only shows in preview mode.
				foreach(Sheet s in _listSheets) {
					pageCount+=Sheets.CalculatePageCount(s,_printMargin);
				}
			}
			ODprintout.InvalidMinDefaultPageWidth=0;
			ODprintout.InvalidMinDefaultPageHeight=-1;//Disable page height test.
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,
				sheet.Description+" "+Lans.g(nameof(PrinterL),"sheet from")+" "+sheet.DateTimeSheet.ToShortDateString()+" "+Lans.g(nameof(PrinterL),"printed"),
				printSit:sit,
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin,
				printoutOrientation:(sheet.IsLandscape?PrintoutOrientation.Landscape:PrintoutOrientation.Portrait),
				totalPages:pageCount,
				isForcedPreview:isPreviewMode,//Ignored when running in DEBUG, will show preview.
				auditPatNum:sheet.PatNum,
				paperSize:paperSize
			);
			_isPrinting=false;
			GC.Collect();//We are done with printing so we can forcefully clean up all the objects and controls that were used in printing.
		}
		#endregion Public Methods - Print

		#region Private Methods
		///<summary>Returns a list of the integer values corresponding to the Rx multi defs (1 through 6) found on the MultiRx sheet.
		///An rx is considered valid if it has ProvNameFL,PatNameFL,PatBirthdate, and Drug</summary>
		private List<int> GetSheetRxCount(SheetDef sheetDef) {
			List<int> rxFieldList=new List<int>();
			bool hasProvNameFL=false;
			bool hasProvNameFL2=false;
			bool hasProvNameFL3=false;
			bool hasProvNameFL4=false;
			bool hasProvNameFL5=false;
			bool hasProvNameFL6=false;
			bool hasPatNameFL=false;
			bool hasPatNameFL2=false;
			bool hasPatNameFL3=false;
			bool hasPatNameFL4=false;
			bool hasPatNameFL5=false;
			bool hasPatNameFL6=false;
			bool hasPatBirthdate=false;
			bool hasPatBirthdate2=false;
			bool hasPatBirthdate3=false;
			bool hasPatBirthdate4=false;
			bool hasPatBirthdate5=false;
			bool hasPatBirthdate6=false;
			bool hasDrug=false;
			bool hasDrug2=false;
			bool hasDrug3=false;
			bool hasDrug4=false;
			bool hasDrug5=false;
			bool hasDrug6=false;
			foreach(SheetFieldDef field in sheetDef.SheetFieldDefs) {
				if(field.FieldName=="prov.nameFL") {
					hasProvNameFL=true;
				}
				if(field.FieldName=="prov.nameFL2") {
					hasProvNameFL2=true;
				}
				if(field.FieldName=="prov.nameFL3") {
					hasProvNameFL3=true;
				}
				if(field.FieldName=="prov.nameFL4") {
					hasProvNameFL4=true;
				}
				if(field.FieldName=="prov.nameFL5") {
					hasProvNameFL5=true;
				}
				if(field.FieldName=="prov.nameFL6") {
					hasProvNameFL6=true;
				}
				if(field.FieldName=="pat.nameFL") {
					hasPatNameFL=true;
				}
				if(field.FieldName=="pat.nameFL2") {
					hasPatNameFL2=true;
				}
				if(field.FieldName=="pat.nameFL3") {
					hasPatNameFL3=true;
				}
				if(field.FieldName=="pat.nameFL4") {
					hasPatNameFL4=true;
				}
				if(field.FieldName=="pat.nameFL5") {
					hasPatNameFL5=true;
				}
				if(field.FieldName=="pat.nameFL6") {
					hasPatNameFL6=true;
				}
				if(field.FieldName=="pat.Birthdate") {
					hasPatBirthdate=true;
				}
				if(field.FieldName=="pat.Birthdate2") {
					hasPatBirthdate2=true;
				}
				if(field.FieldName=="pat.Birthdate3") {
					hasPatBirthdate3=true;
				}
				if(field.FieldName=="pat.Birthdate4") {
					hasPatBirthdate4=true;
				}
				if(field.FieldName=="pat.Birthdate5") {
					hasPatBirthdate5=true;
				}
				if(field.FieldName=="pat.Birthdate6") {
					hasPatBirthdate6=true;
				}
				if(field.FieldName=="Drug") {
					hasDrug=true;
				}
				if(field.FieldName=="Drug2") {
					hasDrug2=true;
				}
				if(field.FieldName=="Drug3") {
					hasDrug3=true;
				}
				if(field.FieldName=="Drug4") {
					hasDrug4=true;
				}
				if(field.FieldName=="Drug5") {
					hasDrug5=true;
				}
				if(field.FieldName=="Drug6") {
					hasDrug6=true;
				}
			}	
			if(hasProvNameFL && hasPatNameFL && hasPatBirthdate && hasDrug) {
				rxFieldList.Add(1);
			}
			if(hasProvNameFL2 && hasPatNameFL2 && hasPatBirthdate2 && hasDrug2) {
				rxFieldList.Add(2);
			}
			if(hasProvNameFL3 && hasPatNameFL3 && hasPatBirthdate3 && hasDrug3) {
				rxFieldList.Add(3);
			}
			if(hasProvNameFL4 && hasPatNameFL4 && hasPatBirthdate4 && hasDrug4) {
				rxFieldList.Add(4);
			}
			if(hasProvNameFL5 && hasPatNameFL5 && hasPatBirthdate5 && hasDrug5) {
				rxFieldList.Add(5);
			}
			if(hasProvNameFL6 && hasPatNameFL6 && hasPatBirthdate6 && hasDrug6) {
				rxFieldList.Add(6);
			}
			return rxFieldList;
		}

		///<summary>Surround with try/catch.
		///Set parentSheet when working with a sheet withing a sheet (ERA).</summary>
		private void pd_DrawFieldsHelper(Sheet sheet,Graphics g,XGraphics gx,Sheet parentSheet=null) {
			//Begin drawing.
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob(_dataSet,_medLab,_pagesPrinted,_yPosPrint,_yPosPrevious,_idxPreClaimPaidPrinted);
			foreach(SheetField field in sheet.SheetFields) {
				if(parentSheet!=null && !SheetDrawingJob.FieldOnCurPageHelper(field,parentSheet,_printMargin,_yPosPrint,_pagesPrinted)) {
					continue;
				}
				else if(parentSheet==null && !SheetDrawingJob.FieldOnCurPageHelper(field,sheet,_printMargin,_yPosPrint,_pagesPrinted)) { 
					continue; 
				}
				switch(field.FieldType) {
					case SheetFieldType.Image:
					case SheetFieldType.PatImage:
						sheetDrawingJob.DrawFieldImage(field,g,gx);
						break;
					case SheetFieldType.Drawing:
						sheetDrawingJob.DrawFieldDrawing(field,g,gx);
						break;
					case SheetFieldType.Rectangle:
						SheetDrawingJob.DrawFieldRectangle(field,g,gx,_yPosPrint);
						break;
					case SheetFieldType.Line:
						sheetDrawingJob.DrawFieldLine(field,g,gx);
						break;
					case SheetFieldType.Special:
						OpenDentBusiness.SheetPrinting.DrawFieldSpecial(sheet,field,g,gx,_yPosPrint);
						break;
					case SheetFieldType.Grid:
						sheetDrawingJob.DrawFieldGrid(field,sheet,g,gx,_dataSet,_stmt,_medLab,true);
						_yPosPrevious=sheetDrawingJob.YPosPrevious;//used for helping print ERAs
						_idxPreClaimPaidPrinted=sheetDrawingJob.IdxPreClaimPaidPrinted;//used for helping print ERAs
						break;
					case SheetFieldType.InputField:
					case SheetFieldType.OutputText:
					case SheetFieldType.StaticText:
						SheetDrawingJob.DrawFieldText(field,sheet,g,gx,_yPosPrint);
						break;
					case SheetFieldType.CheckBox:
						sheetDrawingJob.DrawFieldCheckBox(field,g,gx);
						break;
					case SheetFieldType.ComboBox:
						sheetDrawingJob.DrawFieldComboBox(field,sheet,g,gx);
						break;
					case SheetFieldType.ScreenChart:
						sheetDrawingJob.DrawFieldScreenChart(field,sheet,g,gx);
						break;
					case SheetFieldType.SigBox:
					case SheetFieldType.SigBoxPractice:
						sheetDrawingJob.DrawFieldSigBox(field,sheet,g,gx);
						break;
					default:
						//Parameter or possibly new field type.
						break;
				}
			}//end foreach SheetField
		}

		///<summary>This gets called for every page to be printed when sending to a printer.  Will stop printing when e.HasMorePages==false.  See also CreatePdfPage.</summary>
		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.InterpolationMode=InterpolationMode.HighQualityBicubic;//Necessary for very large images that need to be scaled down.
			Sheet sheet=_listSheets[_sheetsPrinted];
			Sheets.SetPageMargin(sheet,_printMargin);
			if(PagesPrinted==0 && sheet.SheetType==SheetTypeEnum.ERA) {
				_idxPreClaimPaidPrinted=0;
				_yPosPrevious=0;
			}
			try {
				pd_DrawFieldsHelper(sheet,g,null);
			}
			catch(OutOfMemoryException ex) {
				//Cancel the print job because there is a static image on this sheet which is to big for the printer to handle.
				MessageBox.Show(ex.Message);//Custom message that is already translated.
				e.Cancel=true;
				return;
			}
			OpenDentBusiness.SheetDrawingJob.DrawHeader(sheet,g,null,_pagesPrinted,_yPosPrint);
			OpenDentBusiness.SheetDrawingJob.DrawFooter(sheet,g,null,_pagesPrinted,_yPosPrint,_medLab);
			#if DEBUG
				if(_printCalibration) {
					OpenDentBusiness.SheetDrawingJob.DrawCalibration(sheet,g,e,null,null);
				}
			#endif
			g.Dispose();
			g=null;
			#region Set variables for next page to be printed
			_yPosPrint+=sheet.HeightPage-_printMargin.Bottom-_printMargin.Top;//move _yPosPrint down equal to the amount of printable area per page.
			_pagesPrinted++;
			if(_pagesPrinted<Sheets.CalculatePageCount(sheet,_printMargin)) {
				e.HasMorePages=true;
			}
			else {//we are printing the last page of the current sheet.
				_yPosPrint=0;
				_pagesPrinted=0;
				_sheetsPrinted++;
				if(_sheetsPrinted<_listSheets.Count){
					e.HasMorePages=true;
				}
				else{
					e.HasMorePages=false;
					_sheetsPrinted=0;
				}
			}
			#endregion
		}
		#endregion Private Methods	
	}
}
