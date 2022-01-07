using CodeBase;
using OpenDental.UI;
using OpenDentBusiness.SheetFramework;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDentBusiness {
	public class SheetDrawingJob {
		//private fields are essentially the same as in SheetPrintingJob
		#region Fields
		///<summary>Used when printing statements that use the Statements use Sheets feature.  Pdf printing does not use this variable.</summary>
		private DataSet _dataSet;
		///<summary>Used when printing ERAs to speed up the printing of the EraClaimsPaid grid.</summary>
		private int _idxPreClaimPaidPrinted=0;
		private bool _isPrinting=false;
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

		#region Constructors
		public SheetDrawingJob() {}

		//add constructor to set the private fields
    //Make a series of instances of this SheetDrawingJob from within SheetPrintingJob
    //Or, for pdf, just make a SheetDrawingJob.
		public SheetDrawingJob(DataSet dataSet,MedLab medLab,int pagesPrinted,int yPosPrint,int yPosPrev,int idxPrinted) {
			_dataSet=dataSet;
			_medLab=medLab;
			_pagesPrinted=pagesPrinted;
			_yPosPrint=yPosPrint;
			_yPosPrevious=yPosPrev;
			_idxPreClaimPaidPrinted=idxPrinted;
		}
		#endregion Constructors

		#region Properties
		public int YPosPrevious {
			get { return _yPosPrevious; }
		}

		public int IdxPreClaimPaidPrinted {
			get { return _idxPreClaimPaidPrinted; }
		}
		#endregion Properties

		#region Public Methods - CreatePdf
		///<summary>Creates a PDF in memory and returns it, does not save to disk.</summary>
		public PdfDocument CreatePdf(Sheet sheet) {
			return CreatePdf(sheet,"",null,null,null,null,null,false);
		}

		///<summary>If making a statement, use the overload that takes a DataSet otherwise this method will make another call to the db.  If fullFileName is in the A to Z folder, pass in fullFileName as a temp file and then upload that file to the cloud.</summary>
		public void CreatePdf(Sheet sheet,string fullFileName,Statement stmt,MedLab medLab=null) {
			DataSet dataSet=null;
			if(sheet.SheetType==SheetTypeEnum.Statement && stmt!=null) {
				//This should never get hit.  This line of code is here just in case I forgot to update a random spot in our code.
				//Worst case scenario we will end up calling the database a few extra times for the same data set.
				//It use to call this method many, many times so anything is an improvement at this point.
				if(stmt.SuperFamily!=0) {
					dataSet=AccountModules.GetSuperFamAccount(stmt,doShowHiddenPaySplits:stmt.IsReceipt);
				}
				else {
					dataSet=AccountModules.GetAccount(stmt.PatNum,stmt,doShowHiddenPaySplits:stmt.IsReceipt);
				}
			}
			CreatePdf(sheet,fullFileName,stmt,dataSet,medLab);
		}

		///<summary>Creates a file where fullFileName is a local path. If fullFileName is in the A to Z folder, pass in fullFileName as a temp file and then upload that file to the cloud.</summary>
		public PdfDocument CreatePdf(Sheet sheet,string fullFileName,Statement stmt,DataSet dataSet,MedLab medLab,Patient pat=null,Patient patGuar=null,bool doSave=true) {
			Sheets.SetPageMargin(sheet,_printMargin);
			_stmt=stmt;
			_medLab=medLab;
			_isPrinting=true;
			_yPosPrint=0;
			PdfDocument document=new PdfDocument();
			foreach(SheetField field in sheet.SheetFields) {//validate all signatures before modifying any of the text fields.
				if(!ListTools.In(field.FieldType,SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)) {
					continue;
				}
				field.SigKey=Sheets.GetSignatureKey(sheet);
			}
			if(stmt!=null) {//Because most of the time statements do not display the Fill Sheet window, so there is no opportunity to select a combobox.
				SheetUtil.SetDefaultValueForComboBoxes(sheet);
			}
			//this will set the page breaks as well as adjust for growth behavior
			SheetUtil.CalculateHeights(sheet,dataSet,_stmt,_isPrinting,_printMargin.Top,_printMargin.Bottom,_medLab,pat,patGuar);
			int pageCount=Sheets.CalculatePageCount(sheet,_printMargin);
			for(int i=0;i<pageCount;i++) {
				_pagesPrinted=i;
				PdfPage page=document.AddPage();
				_yPosPrint=CreatePdfPage(sheet,page,dataSet,pat: pat,patGuar: patGuar,_yPosPrint);
			}
			if(doSave) {
				try {
					document.Save(fullFileName);
				}
				catch (Exception ex) {
					MessageBox.Show(Lans.g(this,"An error has occurred while trying to save this document:")+"\n"+ex.Message);
				}
			}
			_isPrinting=false;
			GC.Collect();//We are done creating the pdf so we can forcefully clean up all the objects and controls that were used.
			return document;
		}

		///<summary>Called for every page that is generated for a PDF document. Pages and yPos must be tracked outside of this function. See also pd_PrintPage.  DataSet should be prefilled with AccountModules.GetAccount() before calling this method if making a statement.</Summary>
		public int CreatePdfPage(Sheet sheet,PdfPage page,DataSet dataSet,Patient pat,Patient patGuar,int yPos,int pagesPrinted=-1) {
			_yPosPrint=yPos;
			if(pagesPrinted > -1) {
				_pagesPrinted=pagesPrinted;
			}
			page.Width=p(sheet.Width);//XUnit.FromInch((double)sheet.Width/100);  //new XUnit((double)sheet.Width/100,XGraphicsUnit.Inch);
			page.Height=p(sheet.Height);//new XUnit((double)sheet.Height/100,XGraphicsUnit.Inch);
			if(sheet.IsLandscape){
				page.Orientation=PageOrientation.Landscape;
			}
			Sheets.SetPageMargin(sheet,_printMargin);
			XGraphics gx=XGraphics.FromPdfPage(page);
			gx.SmoothingMode=XSmoothingMode.HighQuality;
			foreach(SheetField field in sheet.SheetFields) {
				if(!FieldOnCurPageHelper(field,sheet,_printMargin,yPos,_pagesPrinted)) { 
					continue; 
				}
				switch(field.FieldType) {
					case SheetFieldType.Image:
					case SheetFieldType.PatImage:
						DrawFieldImage(field,null,gx);
						break;
					case SheetFieldType.Drawing:
						DrawFieldDrawing(field,null,gx);
						break;
					case SheetFieldType.Rectangle:
						DrawFieldRectangle(field,null,gx,_yPosPrint);
						break;
					case SheetFieldType.Line:
						DrawFieldLine(field,null,gx);
						break;
					case SheetFieldType.Special:
						OpenDentBusiness.SheetPrinting.DrawFieldSpecial(sheet,field,null,gx,yPos);
						break;
					case SheetFieldType.Grid:
						DrawFieldGrid(field,sheet,null,gx,dataSet,_stmt,_medLab,pat: pat,patGuar: patGuar);
						break;
					case SheetFieldType.InputField:
					case SheetFieldType.OutputText:
					case SheetFieldType.StaticText:
						DrawFieldText(field,sheet,null,gx,_yPosPrint);
						break;
					case SheetFieldType.CheckBox:
						DrawFieldCheckBox(field,null,gx);
						break;
					case SheetFieldType.ComboBox:
						DrawFieldComboBox(field,sheet,null,gx);
						break;
					case SheetFieldType.ScreenChart:
						DrawFieldScreenChart(field,sheet,null,gx);
						break;
					case SheetFieldType.SigBox:
					case SheetFieldType.SigBoxPractice:
						DrawFieldSigBox(field,sheet,null,gx);
						break;
					case SheetFieldType.Parameter:
					default:
						//Parameter or possibly new field type.
						break;
				}
			}//end foreach SheetField
			DrawHeader(sheet,null,gx,_pagesPrinted,_yPosPrint);
			DrawFooter(sheet,null,gx,_pagesPrinted,_yPosPrint,_medLab);
			#if DEBUG
				if(_printCalibration) {
					DrawCalibration(sheet,null,null,gx,page);
				}
			#endif
			gx.Dispose();
			gx=null;
			#region Set variables for next page to be printed
			yPos+=sheet.HeightPage-(_printMargin.Bottom+_printMargin.Top);//move _yPosPrint down equal to the amount of printable area per page.
			_pagesPrinted++;
			if(_pagesPrinted<Sheets.CalculatePageCount(sheet,_printMargin)) {
				//More pages need to be created for this pdf.  Do not manipulate _yPosPrint and simply continue.
			}
			else {//we are printing the last page of the current sheet.
				yPos=0;
				_pagesPrinted=0;
				_sheetsPrinted++;
			}
			return yPos;
			#endregion
		}
		#endregion Public Methods - CreatePdf

		#region Public Methods - Draw
		public void DrawFieldCheckBox(SheetField field,Graphics g,XGraphics gx) {
			if(field.FieldValue!="X") {
				return;
			}
			if(gx==null) {
				Pen pen3=new Pen(Brushes.Black,1.6f);
				g.DrawLine(pen3,field.XPos,field.YPos-_yPosPrint,field.XPos+field.Width,field.YPos-_yPosPrint+field.Height);
				g.DrawLine(pen3,field.XPos+field.Width,field.YPos-_yPosPrint,field.XPos,field.YPos-_yPosPrint+field.Height);
				pen3.Dispose();
				pen3=null;
			}
			else {
				XPen pen3=new XPen(XColors.Black,p(1.6f));
				gx.DrawLine(pen3,p(field.XPos),p(field.YPos-_yPosPrint),p(field.XPos+field.Width),p(field.YPos-_yPosPrint+field.Height));
				gx.DrawLine(pen3,p(field.XPos+field.Width),p(field.YPos-_yPosPrint),p(field.XPos),p(field.YPos-_yPosPrint+field.Height));
				pen3=null;
			}
		}

		public void DrawFieldComboBox(SheetField field,Sheet sheet,Graphics g,XGraphics gx) {
			string comboChoice=field.FieldValue.Split(';')[0];
			string fontName=(string.IsNullOrEmpty(sheet.FontName) ? FontFamily.GenericMonospace.ToString() : sheet.FontName);
			if(gx==null){
				Font font=new Font(fontName,field.Height-7,FontStyle.Regular);
				Rectangle bounds=new Rectangle(field.XPos,field.YPos-_yPosPrint,field.Width,field.Height);
				GraphicsHelper.DrawString(g,comboChoice,font,Brushes.Black,bounds,HorizontalAlignment.Left);
				font.Dispose();
				font=null;
			}
			else{
				XFont xfont=new XFont(fontName,field.Height-10,XFontStyle.Regular);
				RectangleF rect=new RectangleF(field.XPos,field.YPos-_yPosPrint,field.Width,field.Height);
				GraphicsHelper.DrawStringX(gx,comboChoice,xfont,XBrushes.Black,rect,HorizontalAlignment.Left);
				xfont=null;
			}
		}

		public void DrawFieldDrawing(SheetField field,Graphics g,XGraphics gx) {
			if(gx==null) {
				Pen pen=new Pen(Brushes.Black,2f);
				List<Point> points=new List<Point>();
				string[] pairs=field.FieldValue.Split(new string[] { ";" },StringSplitOptions.RemoveEmptyEntries);
				foreach(string p in pairs) {
					points.Add(new Point(PIn.Int(p.Split(',')[0]),PIn.Int(p.Split(',')[1])));
				}
				for(int i=1;i<points.Count;i++) {
					g.DrawLine(pen,points[i-1].X,points[i-1].Y-_yPosPrint,points[i].X,points[i].Y-_yPosPrint);
				}
				pen.Dispose();
				pen=null;
			}
			else {
				XPen pen=new XPen(XColors.Black,p(2));
				List<Point> points=new List<Point>();
				string[] pairs=field.FieldValue.Split(new string[] { ";" },StringSplitOptions.RemoveEmptyEntries);
				foreach(string p2 in pairs) {
					points.Add(new Point(PIn.Int(p2.Split(',')[0]),PIn.Int(p2.Split(',')[1])));
				}
				for(int i=1;i<points.Count;i++) {
					gx.DrawLine(pen,p(points[i-1].X),p(points[i-1].Y-_yPosPrint),p(points[i].X),p(points[i].Y-_yPosPrint));
				}
				pen=null;
			}
		}

		///<Summary>DataSet should be prefilled with AccountModules.GetAccount() before calling this method if printing a statement.</Summary>
		public void DrawFieldGrid(SheetField field,Sheet sheet,Graphics g,XGraphics gx,DataSet dataSet,Statement stmt,MedLab medLab,
			bool isPrinting=false,Patient pat=null,Patient patGuar=null) 
		{
			if(sheet.SheetType!=SheetTypeEnum.ERA) {
				DrawFieldGridHelper(field,sheet,g,gx,dataSet,stmt,medLab,isPrinting,pat: pat,patGuar: patGuar);
				return;
			}
			//ERA sheets can only have grid field such that field.FieldName=="EraClaimsPaid".
			SheetParameter param=SheetParameter.GetParamByName(sheet.Parameters,"IsSingleClaimPaid");
			bool isSingleClaim=(param.ParamValue==null)?false:true;//param is only set when true
			//This logic mimics SheetUtil.CalculateHeights(...)
			bool isOneClaimPerPage=PrefC.GetBool(PrefName.EraPrintOneClaimPerPage);
			if(isSingleClaim) {
				//When printing a single claim we do not want to print the claim on the next page like we would when printing every claim.
				isOneClaimPerPage=false;
			}
			if(isOneClaimPerPage && _pagesPrinted==0) {
				//First page is the ERA fields page.  Continue to next page unless this is a single claim being printed.
				return;
			}
			SheetDef gridHeaderSheetDef=SheetDefs.GetInternalOrCustom(SheetInternalType.ERAGridHeader);
			X835 era=(X835)SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue;//Required param
			DataTable tablePaidProcs=SheetDataTableUtil.GetDataTableForGridType(sheet,dataSet,field.FieldName,stmt,medLab);
			DataTable tableRemarks=SheetDataTableUtil.GetDataTableForGridType(sheet,dataSet,"EraClaimsPaidProcRemarks",stmt,medLab);
			Sheet sheetGridHeader=SheetUtil.CreateSheet(gridHeaderSheetDef);
			int yPosStep=_yPosPrevious;
			int printableHeight=sheet.Height-_printMargin.Top-_printMargin.Bottom;
			int pageMinY=(_pagesPrinted*printableHeight);
			int pageMaxY=((_pagesPrinted+1)*printableHeight);
			for(int i=_idxPreClaimPaidPrinted; i<era.ListClaimsPaid.Count; i++) {
				Hx835_Claim claim=era.ListClaimsPaid[i];
				//Work with copy so that original position values do not change per iteration.
				SheetField fieldCopy=field.Copy();
				Sheet sheetGridHeaderCopy=sheetGridHeader.Copy();
				if(isOneClaimPerPage) {
					fieldCopy.YPos=_printMargin.Top+1;//Set field to top of page.  Plus 1 for padding.
					yPosStep+=printableHeight;
				}
				fieldCopy.YPos+=yPosStep;
				if(fieldCopy.YPos>=pageMaxY//Field is on the next page.
					|| fieldCopy.YPos<=pageMaxY && fieldCopy.YPos+sheetGridHeader.Height>pageMaxY)//Header is split between 2 pages.  Let Next page handle.
				{
					break;//Done with this page.
				}
				//At this point claim has not been drawn on any previous page.
				if(fieldCopy.YPos<=pageMinY) { //Claim is not drawn and YPos is above the current page, so we must adjust it.
					yPosStep+=((pageMinY+_printMargin.Top+1)-fieldCopy.YPos);//The difference of what we are about to add the YPos.
					fieldCopy.YPos=pageMinY+_printMargin.Top+1;//Set to top of page, plus 1 for padding.
				}
				sheetGridHeaderCopy.SheetFields.ForEach(x => {
					//Make header sheet possitions relative to parent sheet and grid field location.
					x.XPos+=fieldCopy.XPos;
					x.YPos+=fieldCopy.YPos;
				});
				SheetParameter.GetParamByName(sheetGridHeaderCopy.Parameters,"EraClaimPaid").ParamValue=claim;//Required param
				SheetParameter.GetParamByName(sheetGridHeaderCopy.Parameters,"ClaimIndexNum").ParamValue=(isSingleClaim?0:era.ListClaimsPaid.IndexOf(claim)+1);//0 index so +1
				SheetFiller.FillFields(sheetGridHeaderCopy);
				pd_DrawFieldsHelper(sheetGridHeaderCopy,g,gx,sheet);
				fieldCopy.YPos+=sheetGridHeaderCopy.Height;
				int procGridHeight=DrawFieldGridHelper(fieldCopy,sheet,g,gx,dataSet,stmt,medLab,isPrinting,claim.ClpSegmentIndex,tablePaidProcs);
				SheetField fieldRemarks=fieldCopy.Copy();
				fieldRemarks.FieldName="EraClaimsPaidProcRemarks";
				fieldRemarks.YPos=fieldCopy.YPos+procGridHeight;
				int remarkGridHeight=DrawFieldGridHelper(fieldRemarks,sheet,g,gx,dataSet,stmt,medLab,isPrinting,claim.ClpSegmentIndex,tableRemarks);
				int subFieldHeight=sheetGridHeaderCopy.Height+procGridHeight+remarkGridHeight;
				if(isOneClaimPerPage) {
					if(subFieldHeight>printableHeight) {//Grid wraps onto a second page.
						yPosStep+=printableHeight*(subFieldHeight/printableHeight);//Truncation intended.  Add extra pages when needed.
					}
				}
				else {
					subFieldHeight+=25;
					yPosStep+=subFieldHeight;
				}
				_idxPreClaimPaidPrinted++;
				_yPosPrevious=yPosStep;
			}
		}

		public int DrawFieldGridHelper(SheetField field,Sheet sheet,Graphics g,XGraphics gx,DataSet dataSet,Statement stmt,MedLab medLab,
			bool isPrinting=false,long eraClaimSegmentIdx=-1,DataTable table=null,Patient pat=null,Patient patGuar=null) 
		{
			if(stmt!=null && stmt.StatementType==StmtType.LimitedStatement && field.FieldName.StartsWith("StatementAging")) {
				return 0;
			}
			Sheets.SetPageMargin(sheet,_printMargin);
			OpenDental.UI.GridOD odGrid=new OpenDental.UI.GridOD();//Only used for measurements, also contains printing/drawing logic.
			odGrid.BeginUpdate();
			odGrid.IsForSheets=true;
			odGrid.VScrollVisible=false;
			if(!string.IsNullOrEmpty(field.FontName)) {
				odGrid.FontForSheets=new Font(field.FontName,field.FontSize,field.FontIsBold ? FontStyle.Bold : FontStyle.Regular);
			}
			int _yAdjCurRow=0;//used to adjust for Titles, Headers, Rows, and footers (all considered part of the same row).
			List<DisplayField> listDisplayFields=SheetUtil.GetGridColumnsAvailable(field.FieldName);
			if(sheet.SheetType==SheetTypeEnum.PaymentPlan) {
				PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(sheet.Parameters,"payplan").ParamValue;
				if(payPlan.IsDynamic) {
					listDisplayFields.RemoveAll(x => x.InternalName=="Adjustment");
				}
			}
			if(table==null) {
				table=SheetDataTableUtil.GetDataTableForGridType(sheet,dataSet,field.FieldName,stmt,medLab,patGuar: patGuar);
			}
			if(field.FieldName=="TreatPlanMain") {
				TreatPlanType tpType=(TreatPlanType)PIn.Int(table.Rows[0]["paramTreatPlanType"].ToString());
				switch(tpType) {
					case TreatPlanType.Discount:
						listDisplayFields.RemoveAll(x => x.InternalName=="Pri Ins" || x.InternalName=="Sec Ins" || x.InternalName=="Allowed");
						break;
					case TreatPlanType.Insurance:
						listDisplayFields.RemoveAll(x => x.InternalName=="DPlan");
						break;
				}
				listDisplayFields.RemoveAll(x => x.InternalName==DisplayFields.InternalNames.TreatmentPlanModule.Appt);
			}
			FilterColumnsHelper(sheet,field,listDisplayFields);
			odGrid.Width=listDisplayFields.Sum(x => x.ColumnWidth);//sum of empty list will return 0
			odGrid.Height=field.Height;
			odGrid.SheetYPos=field.YPos;
			odGrid.Title=field.FieldName;
			if(stmt!=null) {
				odGrid.Title+=((stmt.Intermingled || stmt.SinglePatient)?".Intermingled":".NotIntermingled");//Important for calculating heights.
			}
			odGrid.SheetTopMargin=_printMargin.Top;
			odGrid.SheetBottomMargin=_printMargin.Bottom;
			odGrid.SheetPageHeight=sheet.HeightPage;
			#region  Fill Grid, Set Text Alignment
			odGrid.BeginUpdate();
			odGrid.ListGridColumns.Clear();
			GridColumn col;
			foreach(DisplayField colCur in listDisplayFields) {
				if(string.IsNullOrEmpty(colCur.Description)) {
					col=new GridColumn(colCur.InternalName,colCur.ColumnWidth);
				}
				else {
					 col=new GridColumn(colCur.Description,colCur.ColumnWidth);
				}
				switch(field.FieldName+"."+colCur.InternalName) {//Unusual switch statement to differentiate similar column names in different grids.
					case "StatementMain.charges":
					case "StatementMain.credits":
					case "StatementMain.balance":
					case "StatementPayPlan.charges":
					case "StatementPayPlan.credits":
					case "StatementPayPlan.balance":
					case "StatementDynamicPayPlan.charges":
					case "StatementDynamicPayPlan.credits":
					case "StatementDynamicPayPlan.balance":
					case "StatementInvoicePayment.amt":
					case "TreatPlanMain.Fee":
					case "TreatPlanMain.Pri Ins":
					case "TreatPlanMain.Sec Ins":
					case "TreatPlanMain.DPlan":
					case "TreatPlanMain.Discount":
					case "TreatPlanMain.Pat":
					case "TreatPlanMain.Tax Est":
					case "TreatPlanMain."+DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR:
					case "TreatPlanBenefitsFamily.Primary":
					case "TreatPlanBenefitsFamily.Secondary":
					case "TreatPlanBenefitsIndividual.Primary":
					case "TreatPlanBenefitsIndividual.Secondary":
					case "PayPlanMain.Principal":
					case "PayPlanMain.Interest":
					case "PayPlanMain.due":
					case "PayPlanMain.payment":
					case "PayPlanMain.balance":
					case "PayPlanMain.Adjustment":
					case "EraClaimsPaid.FeeBilled":
					case "EraClaimsPaid.PatResp":
					case "EraClaimsPaid.Contractual":
					case "EraClaimsPaid.PayorReduct":
					case "EraClaimsPaid.OtherAdjust":
					case "EraClaimsPaid.InsPaid":
					case "EraClaimsPaid.RemarkCodes":
						col.TextAlign=HorizontalAlignment.Right;
						break;
					case "StatementAging.Age00to30":
					case "StatementAging.Age31to60":
					case "StatementAging.Age61to90":
					case "StatementAging.Age90plus":
					case "StatementAging.AcctTotal":
						if(sheet.SheetType==SheetTypeEnum.Statement && stmt!=null && stmt.SuperFamily!=0) {
							col.TextAlign=HorizontalAlignment.Right;
						}
						else {
							col.TextAlign=HorizontalAlignment.Center;
						}
						break;
					case "StatementEnclosed.AmountDue":
					case "StatementEnclosed.DateDue":
					case "EraClaimsPaid.ProcCode":
					case "EraClaimsPaidProcRemarks.RemarkCode":
						col.TextAlign=HorizontalAlignment.Center;
						break;
					default:
						col.TextAlign=HorizontalAlignment.Left;
						break;
				}
				odGrid.ListGridColumns.Add(col);
			}
			GridRow row;
			foreach(DataRow rowCur in table.Rows) {
				if(eraClaimSegmentIdx!=-1 && rowCur["ClpSegmentIndex"].ToString()!=eraClaimSegmentIdx.ToString()) {
					continue;
				}
				row=new GridRow();
				foreach(DisplayField colCur in listDisplayFields) {
					//Some DisplayFields require additional formatting and/or logic to display properly in a grid.
					string value=SheetUtil.GetStringFromInternalName(field,colCur,rowCur);
					row.Cells.Add(value);
				}
				if(table.Columns.Contains("PatNum")) {//Used for statments to determine account splitting.
					row.Tag=rowCur["PatNum"].ToString();
				}
				//Colored Text
				if(table.Columns.Contains("paramTextColor") && !string.IsNullOrEmpty(rowCur["paramTextColor"].ToString())) {
					Color cRowText=Color.FromArgb(PIn.Int(rowCur["paramTextColor"].ToString()));
					if(!cRowText.IsEmpty) {
						row.ColorText=cRowText;
					}
				}
				//Bold Text
				if(table.Columns.Contains("paramIsBold")) {
					row.Bold=(bool)rowCur["paramIsBold"];
				}
				if(table.Columns.Contains("paramIsBorderBoldBottom")) {
					if((bool)rowCur["paramIsBorderBoldBottom"]) {
						row.ColorLborder=Color.Black;
					}
				}
				odGrid.ListGridRows.Add(row);
			}
			odGrid.EndUpdate();//Calls ComputeRows and ComputeColumns, meaning the RowHeights int[] has been filled.
			#endregion
			for(int i=0;i<odGrid.ListGridRows.Count;i++) {
				GridSheetRow gridSheetRow=odGrid.ListGridSheetRows[i];
				if(_isPrinting
					&& (gridSheetRow.YPos-_printMargin.Top<_yPosPrint //rows at the end of previous page
						|| gridSheetRow.YPos-sheet.HeightPage+_printMargin.Bottom>_yPosPrint)) 
				{
					continue;//continue because we do not want to draw rows from other pages.
				}
				_yAdjCurRow=0;
				//if(printRowCur.YPos<_yPosPrint
				//	|| printRowCur.YPos-_yPosPrint>sheet.HeightPage) {
				//	continue;//skip rows on previous page and rows on next page.
				//}
				#region Draw Title
				int heightGridTitle=18;
				if(gridSheetRow.IsTitleRow) {
					switch(field.FieldName) {//Draw titles differently for different grids.
						case "StatementMain":
							long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
							Patient patient=(pat==null || pat.PatNum!=patNum ? Patients.GetPat(patNum) : pat);
							string patName="";
							if(patient!=null) {//should always be true
								patName=patient.GetNameFLnoPref()+(stmt.SuperFamily!=0 ? " - "+patient.PatNum : "");//Append patnum only if super statement
							}
							if(gx==null) {
								g.FillRectangle(Brushes.White,field.XPos-10,gridSheetRow.YPos-_yPosPrint,odGrid.Width+10,heightGridTitle);
								g.DrawString(patName,new Font("Arial",10,FontStyle.Bold),new SolidBrush(Color.Black),field.XPos-10,gridSheetRow.YPos-_yPosPrint);
							}
							else {
								gx.DrawRectangle(Brushes.White,p(field.XPos-10),p(gridSheetRow.YPos-_yPosPrint-1),p(odGrid.Width+10),p(heightGridTitle));
								using(Font _font=new Font("Arial",10,FontStyle.Bold)) {
									GraphicsHelper.DrawStringX(gx,patName,
										new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,
										new RectangleF(field.XPos-10,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width+10,heightGridTitle),HorizontalAlignment.Left);
									//gx.DrawString(patName,new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),new SolidBrush(Color.Black),field.XPos-10,yPosGrid);
								}
							}
							break;
						case "StatementDynamicPayPlan":
						case "StatementPayPlan":
							string text="Payment Plans";
							if(field.FieldName=="StatementDynamicPayPlan") {
								text="Dynamic Payment Plans";
							}
							SizeF sSize=TextRenderer.MeasureText(text,new Font("Arial",10,FontStyle.Bold));
							if(gx==null) {
								g.FillRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint,odGrid.Width,heightGridTitle);
								g.DrawString(text,new Font("Arial",10,FontStyle.Bold),new SolidBrush(Color.Black),field.XPos+(field.Width-sSize.Width)/2,gridSheetRow.YPos-_yPosPrint);
							}
							else {
								gx.DrawRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle);
								using(Font _font=new Font("Arial",10,FontStyle.Bold)) {
									GraphicsHelper.DrawStringX(gx,text,
										new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,
										new RectangleF(field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle),HorizontalAlignment.Center);
									//gx.DrawString("Payment Plans",new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),new SolidBrush(Color.Black),field.XPos+(field.Width-sSize.Width)/2,yPosGrid);
								}
							}
							break;
							//grid that shows all payments made on the day of the sheet or attached to procedures on the invoice. 
							//only for invoices.
						case "StatementInvoicePayment":
							//drawing logic is mostly copied from PayPlan grid logic above.
							sSize=TextRenderer.MeasureText("Payments & WriteOffs",new Font("Arial",10,FontStyle.Bold));
							if(gx==null) {
								g.FillRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint,odGrid.Width,heightGridTitle);
								g.DrawString("Payments & WriteOffs",new Font("Arial",10,FontStyle.Bold),new SolidBrush(Color.Black),field.XPos,gridSheetRow.YPos-_yPosPrint);
							}
							else {
								gx.DrawRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle);
								using(Font _font = new Font("Arial",10,FontStyle.Bold)) {
									GraphicsHelper.DrawStringX(gx,"Payments",
										new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,
										new RectangleF(field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle),HorizontalAlignment.Center);
								}
							}
							break;
						case "TreatPlanBenefitsFamily":
							sSize=TextRenderer.MeasureText("Family Insurance Benefits",new Font("Arial",10,FontStyle.Bold));
							if(gx==null) {
								g.FillRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint,odGrid.Width,heightGridTitle);
								g.DrawString("Family Insurance Benefits",new Font("Arial",10,FontStyle.Bold),new SolidBrush(Color.Black),field.XPos+(field.Width-sSize.Width)/2,gridSheetRow.YPos-_yPosPrint);
							}
							else {
								gx.DrawRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle);
								using(Font _font=new Font("Arial",10,FontStyle.Bold)) {
									GraphicsHelper.DrawStringX(gx,"Family Insurance Benefits",
										new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,
										new RectangleF(field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle),HorizontalAlignment.Center);
									//gx.DrawString("Payment Plans",new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),new SolidBrush(Color.Black),field.XPos+(field.Width-sSize.Width)/2,yPosGrid);
								}
							}
							break;
						case "TreatPlanBenefitsIndividual":
							sSize=TextRenderer.MeasureText("Individual Insurance Benefits",new Font("Arial",10,FontStyle.Bold));
							if(gx==null) {
								g.FillRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint,odGrid.Width,heightGridTitle);
								g.DrawString("Individual Insurance Benefits",new Font("Arial",10,FontStyle.Bold),new SolidBrush(Color.Black),field.XPos+(field.Width-sSize.Width)/2,gridSheetRow.YPos-_yPosPrint);
							}
							else {
								gx.DrawRectangle(Brushes.White,field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle);
								using(Font _font=new Font("Arial",10,FontStyle.Bold)) {
									GraphicsHelper.DrawStringX(gx,"Individual Insurance Benefits",
										new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,
										new RectangleF(field.XPos,gridSheetRow.YPos-_yPosPrint-1,odGrid.Width,heightGridTitle),HorizontalAlignment.Center);
									//gx.DrawString("Payment Plans",new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),new SolidBrush(Color.Black),field.XPos+(field.Width-sSize.Width)/2,yPosGrid);
								}
							}
							break;
						default:
							if(gx==null) {
								odGrid.SheetDrawTitle(g,field.XPos,gridSheetRow.YPos-_yPosPrint);
							}
							else {
								odGrid.SheetDrawTitleX(gx,field.XPos,gridSheetRow.YPos-_yPosPrint);
							}
							break;
					}
					_yAdjCurRow+=heightGridTitle;
				}
				#endregion
				#region Draw Header
				if(gridSheetRow.IsHeaderRow) {
					if(gx==null) {
						odGrid.SheetDrawHeader(g,field.XPos,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow);
					}
					else {
						odGrid.SheetDrawHeaderX(gx,field.XPos,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow);
					}
					_yAdjCurRow+=15;
				}
				#endregion
				#region Draw Row
				if(gx==null) {
					odGrid.SheetDrawRow(i,g,field.XPos,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,gridSheetRow.IsBottomRow,true,isPrinting);
				}
				else {
					odGrid.SheetDrawRowX(i,gx,field.XPos,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,gridSheetRow.IsBottomRow,true);
				}
				_yAdjCurRow+=odGrid.ListGridRows[i].State.HeightMain;
				#endregion
				#region Draw Footer (rare)
				if(gridSheetRow.IsFooterRow) {
					_yAdjCurRow+=2;
					switch(field.FieldName) {
						case "StatementDynamicPayPlan":
						case "StatementPayPlan":
							string descript="patientPayPlanDue";
							string textAmountDue="Payment Plan Amount Due: ";
							if(field.FieldName=="StatementDynamicPayPlan") {
								descript="dynamicPayPlanDue";
								textAmountDue="Dynamic Payment Plan Amount Due:";
							}
							DataTable tableMisc=dataSet.Tables["misc"];
							if(tableMisc==null) {
								tableMisc=new DataTable();
							}
							double payPlanDue=tableMisc.Rows.OfType<DataRow>().Where(x => x["descript"].ToString()==descript).Sum(x => PIn.Double(x["value"].ToString()));
							if(gx==null) {
								RectangleF rf=new RectangleF(sheet.Width-60-field.Width,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,field.Width,heightGridTitle);
								g.FillRectangle(Brushes.White,rf);
								StringFormat sf=new StringFormat();
								sf.Alignment=StringAlignment.Far;
								g.DrawString(textAmountDue+payPlanDue.ToString("c"),new Font("Arial",9,FontStyle.Bold),new SolidBrush(Color.Black),rf,sf);
							}
							else {
								gx.DrawRectangle(Brushes.White,p(sheet.Width-field.Width-60),p(gridSheetRow.YPos-_yPosPrint+_yAdjCurRow),p(field.Width),p(heightGridTitle));
								using(Font _font=new Font("Arial",9,FontStyle.Bold)) {
									GraphicsHelper.DrawStringX(gx,textAmountDue+payPlanDue.ToString("c"),new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,new RectangleF(sheet.Width-field.Width-60,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,field.Width,heightGridTitle),HorizontalAlignment.Right);
								}
							}
							break;
						case "StatementInvoicePayment":
							//Table should be filled with payments for today & attached to procs on the invoice.
							//drawing logic copied from payplan logic above.
							if(table==null) {
								tableMisc=new DataTable();
							}
							double totalPayments=table.Select().Sum(x => PIn.Double(x["amt"].ToString()));
							if(gx==null) {
								RectangleF rf=new RectangleF(sheet.Width-60-field.Width,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,field.Width,heightGridTitle);
								g.FillRectangle(Brushes.White,rf);
								StringFormat sf=new StringFormat();
								sf.Alignment=StringAlignment.Far;
								if(PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd)) {
									g.DrawString("Total Payments & WriteOffs: "+totalPayments.ToString("c"),new Font("Arial",9,FontStyle.Bold),new SolidBrush(Color.Black),rf,sf);
								}
								else {
									g.DrawString("Total Payments: "+totalPayments.ToString("c"),new Font("Arial",9,FontStyle.Bold),new SolidBrush(Color.Black),rf,sf);
								}
							}
							else {
								gx.DrawRectangle(Brushes.White,p(sheet.Width-field.Width-60),p(gridSheetRow.YPos-_yPosPrint+_yAdjCurRow),p(field.Width),p(heightGridTitle));
								using(Font _font = new Font("Arial",9,FontStyle.Bold)) {
									if(PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd)) {
										GraphicsHelper.DrawStringX(gx,"Total Payments & WriteOffs: "+totalPayments.ToString("c"),new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,new RectangleF(sheet.Width-field.Width-60,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,field.Width,heightGridTitle),HorizontalAlignment.Right);
									}
									else {
										GraphicsHelper.DrawStringX(gx,"Total Payments: "+totalPayments.ToString("c"),new XFont(_font.FontFamily.ToString(),_font.Size,XFontStyle.Bold),XBrushes.Black,new RectangleF(sheet.Width-field.Width-60,gridSheetRow.YPos-_yPosPrint+_yAdjCurRow,field.Width,heightGridTitle),HorizontalAlignment.Right);
									}
								}
							}
							break;
					}
				}
				#endregion
			}
			odGrid.Dispose();
			return odGrid.SheetPrintHeight;
		}

		///<summary>Draws the image to the graphics object passed in.  Can throw an OutOfMemoryException when printing that will have a message that should be displayed and the print job should be cancelled.</summary>
		public void DrawFieldImage(SheetField field,Graphics g,XGraphics gx,Bitmap image=null) {
			Bitmap bmpOriginal=null;
			ImageFormat bmpOriginalFormat=null;
			Document patDoc=null;
			string filePathAndName="";
			if(image!=null) {
				bmpOriginal=image;
				filePathAndName="image Parameter";
			}
			else {
				#region Get the path for the image
				switch(field.FieldType) {
					case SheetFieldType.Image:
						filePathAndName=ODFileUtils.CombinePaths(SheetUtil.GetClinicImagePath(field.FieldName),field.FieldName);
						break;
					case SheetFieldType.PatImage:
						if(field.FieldValue=="") {
							//There is no document object to use for display, but there may be a baked in image and that situation is dealt with below.
							filePathAndName="";
							break;
						}
						patDoc=Documents.GetByNum(PIn.Long(field.FieldValue));
						List<string> paths=Documents.GetPaths(new List<long> { patDoc.DocNum },ImageStore.GetPreferredAtoZpath());
						if(paths.Count < 1) {//No path was found so we cannot draw the image.
							return;
						}
						filePathAndName=paths[0];
						break;
					default:
						//not an image field
						return;
				}
				#endregion
				#region Load the image into bmpOriginal
				if(field.FieldName=="Patient Info.gif") {
					bmpOriginal=OpenDentBusiness.Properties.Resources.Patient_Info;
					bmpOriginalFormat=ImageFormat.Gif;
				}
				else if(CloudStorage.IsCloudStorage) {
					//FormProgress FormP=new FormProgress();
					//FormP.DisplayText=Lan.g(CloudStorage.LanThis,"Downloading...");
					//FormP.NumberFormat="F";
					//FormP.NumberMultiplication=1;
					//FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					//FormP.TickMS=1000;
					OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(SheetUtil.GetImagePath(),field.FieldName);
					//if(FormP.ShowDialog()==DialogResult.Cancel) {
					//	state.DoCancel=true;
					//	return;
					//}
					if(state==null || state.FileContent==null) {
						return;//Unable to download the image
					}
					else {
						using(MemoryStream stream=new MemoryStream(state.FileContent)) {
							try {
								bmpOriginal=new Bitmap(Image.FromStream(stream));
								bmpOriginalFormat=ImageFormat.Bmp;
							}
							catch(Exception ex) {
								ex.DoNothing();
								return;//If the image is not an actual image file, leave the image field blank.
							}
						}
					}
				}
				else if(File.Exists(filePathAndName)) {//Local AtoZ
					try {
						bmpOriginal=new Bitmap(filePathAndName);
						bmpOriginalFormat=bmpOriginal.RawFormat;
					}
					catch {
						return;//If the image is not an actual image file, leave the image field blank.
					}
				}
				else {
					return;
				}
				if(field.FieldType==SheetFieldType.PatImage && patDoc.DocNum!=0) {
					Bitmap bmpCopy=ImageHelper.ApplyDocumentSettingsToImage(patDoc,bmpOriginal,ImageSettingFlags.ALL);
					bmpOriginal.Dispose();
					bmpOriginal=bmpCopy;
				}
				#endregion
			}
			#region Calculate the image ratio and location, set values for imgDrawWidth and imgDrawHeight
			//inscribe image in field while maintaining aspect ratio.
			float imgRatio=(float)bmpOriginal.Width/(float)bmpOriginal.Height;
			float fieldRatio=(float)field.Width/(float)field.Height;
			float imgDrawHeight=field.Height;//drawn size of image
			float imgDrawWidth=field.Width;//drawn size of image
			int adjustY=0;//added to YPos
			int adjustX=0;//added to XPos
			//For patient images, we need to make sure the images will fit and can maintain aspect ratio.
			if(field.FieldType==SheetFieldType.PatImage && imgRatio>fieldRatio) {//image is too wide
				//X pos and width of field remain unchanged
				//Y pos and height must change
				imgDrawHeight=(float)bmpOriginal.Height*((float)field.Width/(float)bmpOriginal.Width);//img.Height*(width based scale) This also handles images that are too small.
				adjustY=(int)((field.Height-imgDrawHeight)/2f);//adjustY= half of the unused vertical field space
			}
			else if(field.FieldType==SheetFieldType.PatImage && imgRatio<fieldRatio) {//image is too tall
				//X pos and width must change
				//Y pos and height remain unchanged
				imgDrawWidth=(float)bmpOriginal.Width*((float)field.Height/(float)bmpOriginal.Height);//img.Height*(width based scale) This also handles images that are too small.
				adjustX=(int)((field.Width-imgDrawWidth)/2f);//adjustY= half of the unused horizontal field space
			}
			else {//image ratio == field ratio
				//do nothing
			}
			#endregion
			//We used to scale down bmpOriginal here to avoid memory exceptions.
			//Doing so was causing significant quality loss when printing or creating pdfs with very large images.
			if(gx==null) {
				try {
					//Always use the original BMP so that very large images can be scaled by the graphics class thus keeping a high quality image by using interpolation.
					g.DrawImage(bmpOriginal,
						new Rectangle(field.XPos+adjustX,field.YPos+adjustY-_yPosPrint,(int)imgDrawWidth,(int)imgDrawHeight),
						new Rectangle(0,0,bmpOriginal.Width,bmpOriginal.Height),
						GraphicsUnit.Pixel);
				}
				catch(OutOfMemoryException) {
					throw new OutOfMemoryException(Lans.g("Sheets","A static image on this sheet is too high in quality and cannot be printed.")+"\r\n"
						+Lans.g("Sheets","Try printing to a different printer or lower the quality of the static image")+":\r\n"
						+filePathAndName);
				}
			}
			else {
				MemoryStream ms=null;
				//For some reason PdfSharp's XImage cannot handle TIFF images.
				if(filePathAndName.ToLower().EndsWith(".tif") || filePathAndName.ToLower().EndsWith(".tiff")) {
					//Trick PdfSharp when we get a TIFF image into thinking it is a different image type.
					//Saving to BMP format will sometimes increase the file size dramatically.  E.g. an 11MB JPG turned into a 240MB BMP.
					//Instead of using BMP, we will use JPG which should have little to no quality loss and should be more compressed than BMP.
					ms=new MemoryStream();
					bmpOriginal.Save(ms,ImageFormat.Jpeg);
					bmpOriginal.Dispose();
					bmpOriginal=new Bitmap(ms);
				}
				// 07/24/2015 Task created by Brian for a customer stated that when creating a PDF of a sheet that is generated using an image as a background 
				// the PDF will sometimes distort the image beyond recognition.  This didn't happen in 14.3 and began happening in 15.1.  In versions 14.3 and 
				// earlier this section of code would resize the image gotten from the specified file and put it into a new Bitmap object.  It was discovered 
				// that the act of using the Bitmap created directly from the file would lead to the garbled image, so we decided to put the image Bitmap into
				// a new Bitmap without resizing which fixes the issue without the quality loss that was present when we resized in 14.3.
				//Bitmap bmpDraw=new Bitmap(bmpOriginal);
				//XImage xI=XImage.FromGdiPlusImage(bmpDraw);
				// 10/07/2015 We cannot do the above fix for the distorted images because it forcefully changes all images to be BMPs.  
				// Instead, we need to resave the image but in it's native "RawFormat" in order to preserve it's original format.
				// Forcefully making all images on PDFs use BMP causes the PDFs to bloat in size significantly (harder to email).  E.g. 2MB turns into 22MBs.
				Bitmap bmpQualityCopy=new Bitmap(bmpOriginal);//Convert to a BMP to preserve the quality of the original image.
				ms=new MemoryStream();
				bmpQualityCopy.Save(ms,bmpOriginalFormat);//Convert the BMP back to the original image format which will typically cut down the image size.
				bmpOriginal.Dispose();
				bmpOriginal=new Bitmap(ms);//Override the original image with the re-saved imaged in the memory stream (fixes corrupt images).
				XImage xI=XImage.FromGdiPlusImage(bmpOriginal);
				gx.DrawImage(xI,p(field.XPos+adjustX),p(field.YPos-_yPosPrint+adjustY),p(imgDrawWidth),p(imgDrawHeight));
				xI.Dispose();
				xI=null;
				if(ms!=null) {
					ms.Dispose();
					ms=null;
				}
				if(bmpQualityCopy!=null) {
					bmpQualityCopy.Dispose();
					bmpQualityCopy=null;
				}
			}
			if(bmpOriginal!=null) {
				bmpOriginal.Dispose();
				bmpOriginal=null;
			}
		}

		public void DrawFieldLine(SheetField field,Graphics g,XGraphics gx) {
			if(gx==null) {
				g.DrawLine((field.ItemColor.ToArgb()==Color.FromArgb(0).ToArgb() ? Pens.Black : new Pen(field.ItemColor,1)),
					field.XPos,field.YPos-_yPosPrint,
					field.XPos+field.Width,
					field.YPos-_yPosPrint+field.Height);
			}
			else {
				gx.DrawLine((field.ItemColor.ToArgb()==Color.FromArgb(0).ToArgb() ? XPens.Black : new XPen(field.ItemColor,1)),
					p(field.XPos),p(field.YPos-_yPosPrint),
					p(field.XPos+field.Width),
					p(field.YPos-_yPosPrint+field.Height));
			}
		}

		public static void DrawFieldRectangle(SheetField field,Graphics g,XGraphics gx,int yPosPrint) {
			if(gx==null) {
				g.DrawRectangle(Pens.Black,field.XPos,field.YPos-yPosPrint,field.Width,field.Height);
			}
			else {
				gx.DrawRectangle(XPens.Black,p(field.XPos),p(field.YPos-yPosPrint),p(field.Width),p(field.Height));
			}
		}

		public void DrawFieldScreenChart(SheetField field,Sheet sheet,Graphics g,XGraphics gx) {
			//FieldValue is a semicolon delimited list of a single number followed by groups of comma separated surfaces.
			//The first digit represents what type of ScreenChart it is.  0 = Permanent, 1 = Primary
			//Always ignore the first two chars which should be a number followed by a semicolon to get at the surfaces.
			string[] toothValues=field.FieldValue.Substring(2).Split(';');
			//The PDF and Print versions of the drawing are nearly identical, one uses PDF elements and the other uses general Graphics elements.
		  //The sizing is a bit different because PDF's take pixels so we have to convert the sizes.  
			//Additionally the inner boxes are done differently.  For printing it's one horizontal and one vertical line.  For PDF's they're all rectangles.
			if(gx==null) {
				int xPos=field.XPos;//Start positions.
				int yPos=field.YPos-_yPosPrint-15;
				int toothWidth=field.Width/8;
				int toothHeight=(field.Height/2)-10;
				Pen surfacePen=new Pen(Color.Black,0.5f);
				StringFormat stringFormat=new StringFormat();
				stringFormat.Alignment=StringAlignment.Center;
				stringFormat.LineAlignment=StringAlignment.Center;
				RectangleF rectFormat=new RectangleF();
				Font font=new Font(FontFamily.GenericMonospace.ToString(),10,FontStyle.Regular);
				for(int i=0;i<8;i++) {
					int topToothNum=0;
					int botToothNum=0;
					if(i<4) {
						topToothNum=i+2;//2, 3, 4, 5
						botToothNum=31-i;//31, 30, 29, 28
					}
					else {
						topToothNum=i+8;//12, 13, 14, 15
						botToothNum=25-i;//21, 20, 19, 18
					}
					string[] topToothSurfaces=toothValues[i].Split(',');//Getting teeth from indexes 0 to 7
					string[] botToothSurfaces=toothValues[toothValues.Length-(i+1)].Split(',');//Getting teeth from indexes 15 to 8
					rectFormat.Location=new PointF((float)xPos,(float)yPos);
					rectFormat.Size=new Size((int)toothWidth,15);
					g.DrawString(topToothNum.ToString(),font,Brushes.Black,rectFormat,stringFormat);
					rectFormat.Location=new PointF((float)xPos,(float)(yPos+2*toothHeight+15));//20 for top tooth, 2xtoothHeight to get to the bottom of the two teeth rectangles.
					g.DrawString(botToothNum.ToString(),font,Brushes.Black,rectFormat,stringFormat);
					g.DrawRectangle(Pens.Black,xPos,yPos+15,toothWidth,toothHeight);//Draw top tooth box
					g.DrawRectangle(Pens.Black,xPos,yPos+15+toothHeight,toothWidth,toothHeight);//Draw bottom tooth box
					if(i<2 || i>5) { //Only molar teeth have multiple surfaces.
						g.DrawLine(surfacePen,xPos,yPos+15+(toothHeight/2),xPos+toothWidth,yPos+15+(toothHeight/2));//Draw top tooth horizontal line
						g.DrawLine(surfacePen,xPos+(toothWidth/2),yPos+15,xPos+(toothWidth/2),yPos+15+(toothHeight/2));//Draw top tooth vertical line
						g.DrawLine(surfacePen,xPos,yPos+15+(toothHeight/2)+toothHeight,xPos+toothWidth,yPos+15+(toothHeight/2)+toothHeight);//Draw bottom tooth horizontal line
						g.DrawLine(surfacePen,xPos+(toothWidth/2),yPos+15+toothHeight,xPos+(toothWidth/2),yPos+15+(toothHeight/2)+toothHeight);//Draw bottom tooth vertical line
						rectFormat.Location=new PointF((float)xPos,(float)yPos+15);
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						g.DrawString(topToothSurfaces[0],font,(topToothSurfaces[0]=="d" || topToothSurfaces[0]=="m")?Brushes.LightGray : Brushes.Black,rectFormat,stringFormat);//Surface information
						rectFormat.Location=new PointF((float)(xPos+(toothWidth/2)),(float)yPos+15);
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						g.DrawString(topToothSurfaces[1],font,(topToothSurfaces[1]=="d" || topToothSurfaces[1]=="m")?Brushes.LightGray : Brushes.Black,rectFormat,stringFormat);
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+(toothHeight/2)+15));
						rectFormat.Size=new Size((int)toothWidth,(int)(toothHeight/2));
						g.DrawString(topToothSurfaces[2],font,(topToothSurfaces[2]=="ling" || topToothSurfaces[2]=="buc")?Brushes.LightGray : Brushes.Black,rectFormat,stringFormat);
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+15+toothHeight));
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						g.DrawString(botToothSurfaces[0],font,(botToothSurfaces[0]=="d" || botToothSurfaces[0]=="m")?Brushes.LightGray : Brushes.Black,rectFormat,stringFormat);//Surface information
						rectFormat.Location=new PointF((float)(xPos+(toothWidth/2)),(float)(yPos+15+toothHeight));
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						g.DrawString(botToothSurfaces[1],font,(botToothSurfaces[1]=="d" || botToothSurfaces[1]=="m")?Brushes.LightGray : Brushes.Black,rectFormat,stringFormat);
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+15+toothHeight+(toothHeight/2)));
						rectFormat.Size=new Size((int)toothWidth,(int)(toothHeight/2));
						g.DrawString(botToothSurfaces[2],font,(botToothSurfaces[2]=="ling" || botToothSurfaces[2]=="buc")?Brushes.LightGray : Brushes.Black,rectFormat,stringFormat);
					}
					else {
						rectFormat.Location=new PointF((float)xPos,(float)yPos+15);
						rectFormat.Size=new Size((int)toothWidth,(int)toothHeight);
						g.DrawString(topToothSurfaces[2],font,Brushes.Black,rectFormat,stringFormat);//Will be empty or a selected option
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+toothHeight+15));
						rectFormat.Size=new Size((int)toothWidth,(int)toothHeight);
						g.DrawString(botToothSurfaces[2],font,Brushes.Black,rectFormat,stringFormat);//Will be empty or a selected option
					}
					xPos+=toothWidth;
				}
			}
			else {
				double xPos=p(field.XPos);
				double yPos=p(field.YPos-_yPosPrint-15);//Start section for tooth chart control
				double toothWidth=p(field.Width/8);
				double toothHeight=p((field.Height/2)-20);
				XPen surfacePen=new XPen(Color.Black,0.5);
				XStringFormat stringFormat=new XStringFormat();
				stringFormat.Alignment=XStringAlignment.Center;	
				stringFormat.LineAlignment=XLineAlignment.Center;
				RectangleF rectFormat=new RectangleF();
				XFont xfont=new XFont(FontFamily.GenericMonospace.ToString(),toothHeight-20,XFontStyle.Regular);
				for(int i=0;i<8;i++) {
					int topToothNum=0;
					int botToothNum=0;
					if(i<4) {
						topToothNum=i+2;//2, 3, 4, 5
						botToothNum=31-i;//31, 30, 29, 28
					}
					else {
						topToothNum=i+8;//12, 13, 14, 15
						botToothNum=25-i;//21, 20, 19, 18
					}
					string[] topToothSurfaces=toothValues[i].Split(',');//Getting teeth from indexes 0 to 7
					string[] botToothSurfaces=toothValues[toothValues.Length-(i+1)].Split(',');//Getting teeth from indexes 15 to 8
					rectFormat.Location=new PointF((float)xPos,(float)yPos);
					rectFormat.Size=new Size((int)toothWidth,15);
					gx.DrawString(topToothNum.ToString(),xfont,XBrushes.Black,rectFormat,stringFormat);
					rectFormat.Location=new PointF((float)xPos,(float)(yPos+2*toothHeight+15));//20 for top tooth, 2xtoothHeight to get to the bottom of the two teeth rectangles.
					gx.DrawString(botToothNum.ToString(),xfont,XBrushes.Black,rectFormat,stringFormat);
					gx.DrawRectangle(XPens.Black,xPos,yPos+15,toothWidth,toothHeight);//Draw top tooth box
					gx.DrawRectangle(XPens.Black,xPos,yPos+15+toothHeight,toothWidth,toothHeight);//Draw bottom tooth box
					if(i<2 || i>5) { //Only molar teeth have multiple surfaces.
						gx.DrawRectangle(surfacePen,xPos,yPos+15,toothWidth/2,toothHeight/2);//Draw one top tooth surface
						gx.DrawRectangle(surfacePen,xPos+(toothWidth/2),yPos+15,toothWidth/2,toothHeight/2);//Draw second top tooth surface.
						gx.DrawRectangle(surfacePen,xPos,yPos+toothHeight+15,toothWidth/2,toothHeight/2);//Draw one bottom tooth surface
						gx.DrawRectangle(surfacePen,xPos+(toothWidth/2),yPos+toothHeight+15,toothWidth/2,toothHeight/2);//Draw second bottom tooth surface.
						rectFormat.Location=new PointF((float)xPos,(float)yPos+15);
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						gx.DrawString(topToothSurfaces[0],xfont,(topToothSurfaces[0]=="d" || topToothSurfaces[0]=="m")?XBrushes.LightGray : XBrushes.Black,rectFormat,stringFormat);//Surface information
						rectFormat.Location=new PointF((float)(xPos+(toothWidth/2)),(float)yPos+15);
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						gx.DrawString(topToothSurfaces[1],xfont,(topToothSurfaces[1]=="d" || topToothSurfaces[1]=="m")?XBrushes.LightGray : XBrushes.Black,rectFormat,stringFormat);
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+(toothHeight/2)+15));
						rectFormat.Size=new Size((int)toothWidth,(int)(toothHeight/2));
						gx.DrawString(topToothSurfaces[2],xfont,(topToothSurfaces[2]=="ling" || topToothSurfaces[2]=="buc")?XBrushes.LightGray : XBrushes.Black,rectFormat,stringFormat);
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+15+toothHeight));
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						gx.DrawString(botToothSurfaces[0],xfont,(botToothSurfaces[0]=="d" || botToothSurfaces[0]=="m")?XBrushes.LightGray : XBrushes.Black,rectFormat,stringFormat);//Surface information
						rectFormat.Location=new PointF((float)(xPos+(toothWidth/2)),(float)(yPos+15+toothHeight));
						rectFormat.Size=new Size((int)toothWidth/2,(int)(toothHeight/2));
						gx.DrawString(botToothSurfaces[1],xfont,(botToothSurfaces[1]=="d" || botToothSurfaces[1]=="m")?XBrushes.LightGray : XBrushes.Black,rectFormat,stringFormat);
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+15+toothHeight+(toothHeight/2)));
						rectFormat.Size=new Size((int)toothWidth,(int)(toothHeight/2));
						gx.DrawString(botToothSurfaces[2],xfont,(botToothSurfaces[2]=="ling" || botToothSurfaces[2]=="buc")?XBrushes.LightGray : XBrushes.Black,rectFormat,stringFormat);
					}
					else {
						rectFormat.Location=new PointF((float)xPos,(float)yPos+15);
						rectFormat.Size=new Size((int)toothWidth,(int)toothHeight);
						gx.DrawString(topToothSurfaces[2],xfont,XBrushes.Black,rectFormat,stringFormat);//Will be empty or a selected option
						rectFormat.Location=new PointF((float)xPos,(float)(yPos+toothHeight+15));
						rectFormat.Size=new Size((int)toothWidth,(int)toothHeight);
						gx.DrawString(botToothSurfaces[2],xfont,XBrushes.Black,rectFormat,stringFormat);//Will be empty or a selected option
					}
					xPos+=toothWidth;
				}
			}
		}

		public void DrawFieldSigBox(SheetField field,Sheet sheet,Graphics g,XGraphics gx) {
			Bitmap sigImage=new Bitmap(field.Width,field.Height);
			string strSigned=null;//will be set if SheetType is not TreatmentPlan or PaymentPlan, FieldValue.Length>0, and DateTimeSig.Year>1880
			if(sheet.SheetType==SheetTypeEnum.TreatmentPlan) {
				sigImage=GetSigTPHelper(sheet,field);
			}
			else if(sheet.SheetType==SheetTypeEnum.PaymentPlan) {
				sigImage=GetSigPPHelper(sheet,field);
			}
			else {
				SignatureBoxWrapper wrapper=new SignatureBoxWrapper();
				wrapper.Width=field.Width;
				wrapper.Height=field.Height;
				if(field.FieldValue.Length>0) {//a signature is present
					bool sigIsTopaz=false;
					if(field.FieldValue[0]=='1') {
						sigIsTopaz=true;
					}
					string signature="";
					if(field.FieldValue.Length>1) {
						signature=field.FieldValue.Substring(1);
					}
					//string keyData=Sheets.GetSignatureKey(sheet);//can't do this because some of the fields might have different new line characters. Sig will be invalid.
					wrapper.FillSignature(sigIsTopaz,field.SigKey,signature);
					sigImage=wrapper.GetSigImage();
					if(field.DateTimeSig.Year>1880) {
						strSigned=(wrapper.GetIsTypedFromWebForms() ? Lans.g("","Typed Signature in Webforms: ") : Lans.g("","Signed: "))+field.DateTimeSig.ToString();
					}
				}
			}
			string fontName=(string.IsNullOrEmpty(sheet.FontName) ? FontFamily.GenericMonospace.ToString() : sheet.FontName);
			double fontSizeSigned=8.25;//Mimics FormSheetFillEdit.LayoutFields
			int sigWidth=field.Width-2;
			int sigHeight=field.Height-2;
			if(g!=null) {
				Rectangle bounds=new Rectangle(field.XPos,field.YPos-_yPosPrint,sigWidth,sigHeight);
				g.DrawImage(sigImage,bounds);
				if(!string.IsNullOrEmpty(strSigned)) {//not a TreatmentPlan or PaymentPlan, FielValue.Length>0, and DateTimeSig.Year>1880
					Rectangle boundsSigned=new Rectangle(bounds.X+1,bounds.Y+bounds.Height-15,sigWidth-2,14);//Height 14 taken from FormSheetFillEdit
					Font font=new Font(fontName,(float)fontSizeSigned,FontStyle.Regular);
					GraphicsHelper.DrawString(g,strSigned,font,Brushes.Black,boundsSigned,HorizontalAlignment.Left);
					font.Dispose();
				}
			}
			else {
				gx.DrawImage(XImage.FromGdiPlusImage(sigImage),p(field.XPos),p(field.YPos-_yPosPrint),p(sigWidth),p(sigHeight));
				if(!string.IsNullOrEmpty(strSigned)) {//not a TreatmentPlan or PaymentPlan, FielValue.Length>0, and DateTimeSig.Year>1880
					RectangleF boundsSigned=new RectangleF(field.XPos+1,field.YPos-_yPosPrint+field.Height-15,sigWidth-2,14);//Height 14 taken from FormSheetFillEdit
					XFont xfont=new XFont(fontName,fontSizeSigned,XFontStyle.Regular);
					GraphicsHelper.DrawStringX(gx,strSigned,xfont,Brushes.Black,boundsSigned,HorizontalAlignment.Left);
				}
			}
			sigImage.Dispose();
		}

		public static void DrawFieldText(SheetField field,Sheet sheet,Graphics g,XGraphics gx,int yPosPrint) {
			Plugins.HookAddCode(null,"pd_PrintPage_drawFieldLoop",field);
			RectangleF boundsActual;
			if(gx==null){
				FontStyle fontstyle=(field.FontIsBold?FontStyle.Bold:FontStyle.Regular);
				Font font=new Font(field.FontName,field.FontSize,fontstyle);
				Rectangle bounds=new Rectangle(field.XPos,field.YPos-yPosPrint,field.Width,field.Height);//Math.Min(field.Height,_yPosPrint+sheet.HeightPage-_printMargin.Bottom-field.YPos));
				boundsActual=GraphicsHelper.DrawString(g,field.FieldValue,font,
					field.ItemColor.ToArgb()==Color.FromArgb(0).ToArgb() ? Brushes.Black : new SolidBrush(field.ItemColor),
					bounds,field.TextAlign);
				font.Dispose();
				font=null;
			}
			else{
				XFontStyle xfontstyle=(field.FontIsBold?XFontStyle.Bold:XFontStyle.Regular);
				XFont xfont;
				try {
					xfont=new XFont(field.FontName,field.FontSize,xfontstyle);
				}
				catch(Exception) {
					//There are some fonts that PdfSharp does not support.  Instead of showing an error, use our default font.
					xfont=new XFont("Courier New",field.FontSize,xfontstyle);
				}
				//Subtract 5 from YPos to compensate for downward shift of text fields that occurs when creating a PDF from sheet. We don't want to shift
				//sheetdefs created before this bug fix. They will have a DateTCreated of 0001-01-01 by default. Internal sheets and deleted sheetdefs will
				//return null. We still want to shift text fields in these cases.
				SheetDef sheetDef=SheetDefs.GetSheetDef(sheet.SheetDefNum,false);
				if(sheetDef==null || sheetDef.DateTCreated.Year > 1880) {
					field.YPos-=5;
				}
				RectangleF rect=new RectangleF(field.XPos,field.YPos-yPosPrint,field.Width,field.Height);
				boundsActual=GraphicsHelper.DrawStringX(gx,field.FieldValue,xfont,
					field.ItemColor.ToArgb()==Color.FromArgb(0).ToArgb() ? XBrushes.Black : new XSolidBrush(field.ItemColor),
					rect,field.TextAlign);
				//xfont.Dispose();
				xfont=null;
			}
			if(field.FieldType==SheetFieldType.OutputText) {
				switch(sheet.SheetType.ToString()+"."+field.FieldName) {
					case "TreatmentPlan.Note":
						//Add plus 4 to width and height to allow for border thickness.
						if(gx==null) {
							g.DrawRectangle(Pens.DarkGray,
								new Rectangle((int)boundsActual.X,(int)boundsActual.Y,(int)Math.Ceiling(boundsActual.Width+4),(int)Math.Ceiling(boundsActual.Height+4)));
						}
						else {
							gx.DrawRectangle(XPens.DarkGray,
								new XRect(p(boundsActual.X),p(boundsActual.Y),p(boundsActual.Width+4),p(boundsActual.Height+4)));
						}
						break;
				}
			}
		}
		#endregion Public Methods - Draw

		#region Private Methods - Draw
		public static void DrawHeader(Sheet sheet,Graphics g,XGraphics gx,int pagesPrinted,int yPosPrint) {
			if(pagesPrinted==0) {
				return;//Never draw header on first page
			}
			//white-out the header.
			if(gx==null) {
				g.FillRectangle(Brushes.White,0,0,sheet.WidthPage,_printMargin.Top);
			}
			else {
				gx.DrawRectangle(XPens.White,Brushes.White,p(0),p(0),p(sheet.WidthPage),p(_printMargin.Top));
			}
			if(sheet.SheetType==SheetTypeEnum.MedLabResults) {
				DrawMedLabHeader(sheet,g,gx,yPosPrint);
			}
		}

		public static void DrawFooter(Sheet sheet,Graphics g,XGraphics gx,int pagesPrinted,int yPosPrint,MedLab medLab) {
			if(Sheets.CalculatePageCount(sheet,_printMargin)==1 && sheet.SheetType!=SheetTypeEnum.MedLabResults) {
				return;//Never draw footers on single page sheets.
			}
			//whiteout footer.
			if(gx==null) {
				g.FillRectangle(Brushes.White,0,sheet.HeightPage-_printMargin.Bottom,sheet.WidthPage,sheet.HeightPage);
			}
			else {
				gx.DrawRectangle(XPens.White,Brushes.White,p(0),p(sheet.HeightPage-_printMargin.Bottom),p(sheet.WidthPage),p(sheet.HeightPage));
			}
			if(sheet.SheetType==SheetTypeEnum.MedLabResults) {
				DrawMedLabFooter(sheet,g,gx,pagesPrinted,yPosPrint,medLab);
			}
		}

		private static void DrawMedLabFooter(Sheet sheet,Graphics g,XGraphics gx,int pagesPrinted,int yPosPrint, MedLab medLab) {
			SheetField fieldCur=new SheetField();
			fieldCur.XPos=50;
			int pageCount;
			fieldCur.YPos=OpenDentBusiness.SheetPrinting.BottomCurPage(yPosPrint+_printMargin.Bottom+_printMargin.Top+1,sheet,out pageCount)+1;
			fieldCur.Width=625;
			fieldCur.Height=20;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=675;
			fieldCur.Width=125;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			string patLName="";
			string patFName="";
			string patMiddleI="";
			string specNum="";
			foreach(SheetField sf in sheet.SheetFields) {
				switch(sf.FieldName) {
					case "patient.LName":
						patLName=sf.FieldValue;
						continue;
					case "patient.FName":
						patFName=sf.FieldValue;
						continue;
					case "patient.MiddleI":
						patMiddleI=sf.FieldValue;
						continue;
					case "medlab.PatIDLab":
						specNum=sf.FieldValue;
						continue;
					default:
						continue;
				}
			}
			fieldCur.FieldValue=patLName;
			if(patLName!="" && (patFName!="" || patMiddleI!="")) {
				fieldCur.FieldValue+=", ";
			}
			fieldCur.FieldValue+=patFName;
			if(fieldCur.FieldValue!="" && patMiddleI!="") {
				fieldCur.FieldValue+=" ";
			}
			fieldCur.FieldValue+=patMiddleI;
			fieldCur.FontSize=9;
			fieldCur.FontName="Arial";
			fieldCur.FontIsBold=false;
			fieldCur.XPos=53;
			fieldCur.YPos+=1;
			fieldCur.Width=245;
			fieldCur.Height=17;
			fieldCur.TextAlign=HorizontalAlignment.Left;
			fieldCur.ItemColor=Color.FromKnownColor(KnownColor.Black);
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=specNum;
			fieldCur.XPos=678;
			fieldCur.Width=120;
			fieldCur.TextAlign=HorizontalAlignment.Center;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
			fieldCur.FontSize=8.5f;
			fieldCur.FontName=sheet.FontName;
			fieldCur.XPos=50;//position the field at 50 for left margin
			fieldCur.YPos+=19;//drop down 19 pixels from the top of the text in the rect (17 pixel height of text box + 1 pixel to bottom of rect + 1 pixel)
			fieldCur.Width=150;
			fieldCur.TextAlign=HorizontalAlignment.Left;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=String.Format("Page {0} of {1}",pagesPrinted+1,Sheets.CalculatePageCount(sheet,_printMargin));
			fieldCur.XPos=sheet.Width-200;//width of field is 150, with a right margin of 50 xPos is sheet width-150-50=width-200
			fieldCur.TextAlign=HorizontalAlignment.Right;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			if(medLab.IsPreliminaryResult) {
				fieldCur.FieldValue="Preliminary Report";
			}
			else {
				fieldCur.FieldValue="Final Report";
			}
			fieldCur.FontSize=10.0f;
			fieldCur.FontIsBold=true;
			//field will be centered on page, since page count is taking up 150 pixels plus page right margin of 50 pixels on the right side of page
			//and date printed is taking up 50 pixel left margin plus 150 pixel field width on the left side of page
			//field width will be sheet.Width-400 and XPos will be 200
			fieldCur.XPos=200;
			fieldCur.YPos+=2;
			fieldCur.Width=sheet.Width-400;//sheet width-150 (date field width)-150 (page count field width)-50 (left margin)-50 (right margin)
			fieldCur.TextAlign=HorizontalAlignment.Center;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
		}

		private static void DrawMedLabHeader(Sheet sheet,Graphics g,XGraphics gx,int yPosPrint) {
			SheetField fieldCur=new SheetField();
			fieldCur.XPos=50;
			fieldCur.YPos=yPosPrint+40;//top of the top rectangle
			fieldCur.Width=529;
			fieldCur.Height=40;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=579;
			fieldCur.Width=221;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=50;
			fieldCur.YPos+=40;//drop down an additional 40 pixels for second row of rectangles
			fieldCur.Width=100;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=150;
			fieldCur.Width=140;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=290;
			fieldCur.Width=100;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=390;
			fieldCur.Width=145;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=535;
			fieldCur.Width=100;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=635;
			fieldCur.Width=65;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.XPos=700;
			fieldCur.Width=100;
			DrawFieldRectangle(fieldCur,g,gx,yPosPrint);
			fieldCur.FieldValue="Patient Name";
			fieldCur.FontSize=8.5f;
			fieldCur.FontName="Arial";
			fieldCur.FontIsBold=false;
			fieldCur.XPos=54;
			fieldCur.YPos=yPosPrint+44;//4 pixels down from the rectangle top for static text descriptions of text boxes in header
			fieldCur.Width=522;
			fieldCur.Height=15;
			fieldCur.TextAlign=HorizontalAlignment.Left;
			fieldCur.ItemColor=Color.FromKnownColor(KnownColor.GrayText);
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Specimen Number";
			fieldCur.XPos=583;
			fieldCur.Width=214;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Account Number";
			fieldCur.XPos=54;
			fieldCur.YPos+=40;//drop down an additional 40 pixels for second row of static text descriptions
			fieldCur.Width=93;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Patient ID";
			fieldCur.XPos=154;
			fieldCur.Width=133;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Control Number";
			fieldCur.XPos=294;
			fieldCur.Width=93;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Date & Time Collected";
			fieldCur.XPos=394;
			fieldCur.Width=138;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Date Reported";
			fieldCur.XPos=539;
			fieldCur.Width=93;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Gender";
			fieldCur.XPos=639;
			fieldCur.Width=58;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue="Date of Birth";
			fieldCur.XPos=704;
			fieldCur.Width=93;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			string patLName="";
			string patFName="";
			string patMiddleI="";
			string specNum="";
			string acctNum="";
			string patId="";
			string ctrlNum="";
			string dateTCollected="";
			string dateReported="";
			string gender="";
			string birthdate="";
			foreach(SheetField sf in sheet.SheetFields) {
				switch(sf.FieldName) {
					case "patient.LName":
						patLName=sf.FieldValue;
						continue;
					case "patient.FName":
						patFName=sf.FieldValue;
						continue;
					case "patient.MiddleI":
						patMiddleI=sf.FieldValue;
						continue;
					case "medlab.PatIDLab":
						specNum=sf.FieldValue;
						continue;
					case "medlab.PatAccountNum":
						acctNum=sf.FieldValue;
						continue;
					case "medlab.PatIDAlt":
						patId=sf.FieldValue;
						continue;
					case "medlab.SpecimenIDAlt":
						ctrlNum=sf.FieldValue;
						continue;
					case "medlab.DateTimeCollected":
						dateTCollected=sf.FieldValue;
						continue;
					case "medlab.DateTimeReported":
						dateReported=PIn.DateT(sf.FieldValue).ToShortDateString();
						if(dateReported==DateTime.MinValue.ToShortDateString()) {
							dateReported="";
						}
						continue;
					case "patient.Gender":
						gender=sf.FieldValue;
						continue;
					case "patient.Birthdate":
						birthdate=sf.FieldValue;
						continue;
				}
			}
			fieldCur.FieldValue=patLName+", "+patFName+" "+patMiddleI;
			fieldCur.FontSize=9;
			fieldCur.FontName="Arial";
			fieldCur.FontIsBold=false;
			fieldCur.XPos=53;
			fieldCur.YPos=yPosPrint+62;//22 pixels down from the rectangle top (second row of text is 20 pixels below static text descriptions)
			fieldCur.Width=524;
			fieldCur.Height=17;
			fieldCur.TextAlign=HorizontalAlignment.Left;
			fieldCur.ItemColor=Color.FromKnownColor(KnownColor.Black);
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=specNum;
			fieldCur.XPos=582;
			fieldCur.Width=216;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=acctNum;
			fieldCur.XPos=53;
			fieldCur.YPos+=40;//drop down an additional 40 pixels for second row
			fieldCur.Width=95;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=patId;
			fieldCur.XPos=153;
			fieldCur.Width=135;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=ctrlNum;
			fieldCur.XPos=293;
			fieldCur.Width=95;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=dateTCollected;
			fieldCur.XPos=393;
			fieldCur.Width=140;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=dateReported;
			fieldCur.XPos=538;
			fieldCur.Width=95;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=gender;
			fieldCur.XPos=638;
			fieldCur.Width=60;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
			fieldCur.FieldValue=birthdate;
			fieldCur.XPos=703;
			fieldCur.Width=95;
			DrawFieldText(fieldCur,sheet,g,gx,yPosPrint);
		}

		public static void DrawCalibration(Sheet sheet,Graphics g,PrintPageEventArgs e,XGraphics gx, PdfPage page) {
			Font font=new Font("Calibri",10f,FontStyle.Regular);
			XFont xfont=new XFont("Calibri",p(10f),XFontStyle.Regular);
			int sLineSize=15;
			int mLineSize=45;
			int lLineSize=90;
			for(int pass=0;pass<3;pass++) {
				int xO=0;//xOrigin
				int yO=0;//yOrigin
				switch(pass) {
					case 0: xO=yO=0; break;
					case 1: xO=sheet.WidthPage/2; yO=sheet.HeightPage/2; break;
					case 2: xO=sheet.WidthPage; yO=sheet.HeightPage; break;
				}
				for(int i=-100;i<2000;i++) {
					if(i%100==0 && pass==0) {
						//label Axis
						if(g!=null) {
							if(i==0) {
								g.DrawString(i.ToString(),font,Brushes.Black,new PointF(4,4));//label 0
							}//don't draw the zero twice
							else {
								g.DrawString(i.ToString(),font,Brushes.Black,new PointF(xO+75,i+2));//label Y-axis
								g.DrawString(i.ToString(),font,Brushes.Black,new PointF(i+2,yO+75));//label X-axis
							}
						}
						else {
							if(i==0) {

							}//don't draw the zero twice
							else {
								gx.DrawString(i.ToString(),xfont,XBrushes.Black,p(xO+75),p(i+2));//label Y-axis
								gx.DrawString(i.ToString(),xfont,XBrushes.Black,p(i+2),p(yO+75));//label X-axis
							}
						}
					}
					if(i%100==0) {
						//draw large lines and label txt
						if(g!=null) {
							g.DrawLine(Pens.Black,new Point(-lLineSize+xO,i),new Point(+lLineSize+xO,i));//Allong Y-axis
							g.DrawLine(Pens.Black,new Point(i,-lLineSize+yO),new Point(i,+lLineSize+yO));//Allong X-axis
						}
						else {
							gx.DrawLine(XPens.Black,p(-lLineSize+xO),p(i),p(+lLineSize+xO),p(i));//Allong Y-axis
							gx.DrawLine(XPens.Black,p(i),p(-lLineSize+yO),p(i),p(+lLineSize+yO));//Allong X-axis
						}
					}
					else if(i%50==0) {
						//draw 50px lines
						if(g!=null) {
							g.DrawLine(Pens.Black,new Point(-mLineSize+xO,i),new Point(+mLineSize+xO,i));//Allong Y-axis
							g.DrawLine(Pens.Black,new Point(i,-mLineSize+yO),new Point(i,+mLineSize+yO));//Allong X-axis
						}
						else {
							gx.DrawLine(XPens.Black,p(-mLineSize+xO),p(i),p(+mLineSize+xO),p(i));//Allong Y-axis
							gx.DrawLine(XPens.Black,p(i),p(-mLineSize+yO),p(i),p(+mLineSize+yO));//Allong X-axis
						}
					}
					else if(i%10==0) {
						//draw small lines
						if(g!=null) {
							g.DrawLine(Pens.Black,new Point(-sLineSize+xO,i),new Point(+sLineSize+xO,i));//Allong Y-axis
							g.DrawLine(Pens.Black,new Point(i,-sLineSize+yO),new Point(i,+sLineSize+yO));//Allong X-axis
						}
						else {
							gx.DrawLine(XPens.Black,new Point(-sLineSize+xO,i),new Point(+sLineSize+xO,i));//Allong Y-axis
							gx.DrawLine(XPens.Black,p(i),p(-sLineSize+yO),p(i),p(+sLineSize+yO));//Allong X-axis
						}
					}
					else if(i%2==0) {
						//draw dots
						if(g!=null) {
							g.DrawLine(Pens.Black,new Point(-1+xO,i),new Point(+1+xO,i));//Allong Y-axis
							g.DrawLine(Pens.Black,new Point(i,-1+yO),new Point(i,+1+yO));//Allong X-axis
						}
						else {
							gx.DrawLine(XPens.Black,p(-1+xO),p(i),p(+1+xO),p(i));//Allong Y-axis
							gx.DrawLine(XPens.Black,p(i),p(-1+yO),p(i),p(+1+yO));//Allong X-axis
						}
					}
				}//end i -100=>2000
			}//end pass
			//infoBlock
			PrinterSettings settings = new PrinterSettings();
			if(g!=null) {
				g.FillRectangle(Brushes.White,110,110,480,100);
				g.DrawRectangle(Pens.Black,110,110,480,100);
				g.DrawString("Sheet Height = "+sheet.HeightPage.ToString(),font,Brushes.Black,112,112);
				g.DrawString("Sheet Width = "+sheet.WidthPage.ToString(),font,Brushes.Black,112,124);//12px per line
				g.DrawString("DefaultPrinter = "+settings.PrinterName,font,Brushes.Black,112,136);
				g.DrawString("HardMarginX = "+e.PageSettings.HardMarginX,font,Brushes.Black,112,148);
				g.DrawString("HardMarginY = "+e.PageSettings.HardMarginY,font,Brushes.Black,112,160);
			}
			else {
				gx.DrawRectangle(XPens.Black,Brushes.White,p(110),p(110),p(480),p(100));
				gx.DrawRectangle(XPens.Black,p(110),p(110),p(480),p(100));
				gx.DrawString("Sheet Height = "+sheet.HeightPage.ToString(),xfont,XBrushes.Black,p(112),p(112));
				gx.DrawString("Sheet Width = "+sheet.WidthPage.ToString(),xfont,XBrushes.Black,p(112),p(124));//12px per line
				gx.DrawString("DefaultPrinter = "+settings.PrinterName,xfont,XBrushes.Black,p(112),p(136));
				gx.DrawString("HardMarginX = "+settings.DefaultPageSettings.HardMarginX,xfont,XBrushes.Black,p(112),p(148));
				gx.DrawString("HardMarginY = "+settings.DefaultPageSettings.HardMarginY,xfont,XBrushes.Black,p(112),p(160));
				gx.DrawString("PDF TrimMargins ^v<> = "+page.TrimMargins.Top+","+page.TrimMargins.Bottom+","+page.TrimMargins.Left+","+page.TrimMargins.Right,xfont,XBrushes.Black,p(112),p(172));
			}
			font.Dispose();
			font=null;
			xfont=null;
		}

		///<summary>Surround with try/catch.
		///Set parentSheet when working with a sheet withing a sheet (ERA).</summary>
		//Similar method in both OD.SheetPrintingJob and ODB.SheetDrawingJob
		private void pd_DrawFieldsHelper(Sheet sheet,Graphics g,XGraphics gx,Sheet parentSheet=null) {
			//Begin drawing.
			foreach(SheetField field in sheet.SheetFields) {
				if(parentSheet!=null && !FieldOnCurPageHelper(field,parentSheet,_printMargin,_yPosPrint,_pagesPrinted)) {
					continue;
				}
				else if(parentSheet==null && !FieldOnCurPageHelper(field,sheet,_printMargin,_yPosPrint,_pagesPrinted)) { 
					continue; 
				}
				switch(field.FieldType) {
					case SheetFieldType.Image:
					case SheetFieldType.PatImage:
						DrawFieldImage(field,g,gx);
						break;
					case SheetFieldType.Drawing:
						DrawFieldDrawing(field,g,gx);
						break;
					case SheetFieldType.Rectangle:
						DrawFieldRectangle(field,g,gx,_yPosPrint);
						break;
					case SheetFieldType.Line:
						DrawFieldLine(field,g,gx);
						break;
					case SheetFieldType.Special:
						SheetPrinting.DrawFieldSpecial(sheet,field,g,gx,_yPosPrint);
						break;
					case SheetFieldType.Grid:
						DrawFieldGrid(field,sheet,g,gx,_dataSet,_stmt,_medLab,true);
						break;
					case SheetFieldType.InputField:
					case SheetFieldType.OutputText:
					case SheetFieldType.StaticText:
						DrawFieldText(field,sheet,g,gx,_yPosPrint);
						break;
					case SheetFieldType.CheckBox:
						DrawFieldCheckBox(field,g,gx);
						break;
					case SheetFieldType.ComboBox:
						DrawFieldComboBox(field,sheet,g,gx);
						break;
					case SheetFieldType.ScreenChart:
						DrawFieldScreenChart(field,sheet,g,gx);
						break;
					case SheetFieldType.SigBox:
					case SheetFieldType.SigBoxPractice:
						DrawFieldSigBox(field,sheet,g,gx);
						break;
					default:
						//Parameter or possibly new field type.
						break;
				}
			}//end foreach SheetField
		}
		#endregion Private Methods - Draw

		#region Private Methods 
		public static bool FieldOnCurPageHelper(SheetField field,Sheet sheet,Margins printMargin,int yPosPrint,int pagesPrinted) {
			//Even though _printMargins and _yPosPrint are available in this context they are passed in so for future compatibility with webforms.
			if(field.YPos>(yPosPrint+sheet.HeightPage)){
				return false;//field is entirely on one of the next pages.
			}
			if(field.Bounds.Bottom<yPosPrint && pagesPrinted>0) {
				return false;//field is entirely on one of the previous pages. Unless we are on the first page, then it is in the top margin.
			}
			return true;//field is all or partially on current page.
		}

		private void FilterColumnsHelper(Sheet sheet,SheetField field,List<DisplayField> Columns) {
			switch(sheet.SheetType+"."+field.FieldName) {
				case "TreatmentPlan.TreatPlanMain":
					bool checkShowDiscount;
					bool checkShowFees;
					bool checkShowIns;
					bool hasSalesTax;
					try {
						checkShowDiscount=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowDiscount").ParamValue;
						checkShowFees=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowFees").ParamValue;
						checkShowIns=(bool)SheetParameter.GetParamByName(sheet.Parameters,"checkShowIns").ParamValue;
						//this is taken from HasSalesTax in the ContrTreat module
						hasSalesTax=AvaTax.IsTaxable(sheet.PatNum) && Columns.Any(x => x.InternalName=="Tax Est");
					}
					catch {
						//if unable to find any assume default values of true
						checkShowDiscount=true;
						checkShowFees=true;
						checkShowIns=true;
						hasSalesTax=true;
					}
					if(!checkShowFees) {
						Columns.RemoveAll(x => x.InternalName=="Fee");
					}
					if(!checkShowIns) {
						Columns.RemoveAll(x => ListTools.In(x.InternalName,"Pri Ins","Sec Ins","DPlan","Allowed"));
					}
					if(!checkShowDiscount) {
						Columns.RemoveAll(x => x.InternalName=="Discount");
					}
					if(!checkShowIns && !checkShowDiscount && !hasSalesTax) {
						Columns.RemoveAll(x => x.InternalName=="Pat");
					}
					//recenters the GridColumnStylesCollection on the page.
					field.XPos=(sheet.WidthPage-Columns.Sum(x => x.ColumnWidth))/2;
					break;
				case "Statement.StatementAging":
					Statement stmt=null;
					try {
						stmt=(Statement)SheetParameter.GetParamByName(sheet.Parameters,"Statement").ParamValue;
					}
					catch(Exception) {}
					if(sheet.SheetType==SheetTypeEnum.Statement && stmt!=null &&  stmt.SuperFamily==0) {
						Columns.RemoveAll(x => x.InternalName=="AcctTotal" || x.InternalName=="Account");
						Columns.RemoveAll(x => x.InternalName=="PatNum");
					}
					break;
			}
		}

		private static Bitmap GetSigPPHelper(Sheet sheet,SheetField field) {
			PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(sheet.Parameters,"payplan").ParamValue;
			string keyData=(string)SheetParameter.GetParamByName(sheet.Parameters,"keyData").ParamValue;
			if(payPlan.Signature!="") {
				SignatureBoxWrapper sigBoxWrapper=new SignatureBoxWrapper();
				sigBoxWrapper.FillSignature(payPlan.SigIsTopaz,keyData,payPlan.Signature);
				if(sigBoxWrapper.GetNumberOfTabletPoints(payPlan.SigIsTopaz)!=0) {
					return sigBoxWrapper.GetSigImage();
				}
			}
			return new Bitmap(field.Width,field.Height); //return blank image if sig invalid.
		}

		private static Bitmap GetSigTPHelper(Sheet sheet,SheetField field) {
			TreatPlan treatPlan=(TreatPlan)SheetParameter.GetParamByName(sheet.Parameters,"TreatPlan").ParamValue;
			if(field.FieldType==SheetFieldType.SigBox && treatPlan.Signature!="") {
				SignatureBoxWrapper sigBoxWrapper=new SignatureBoxWrapper();
				sigBoxWrapper.SignatureMode=SignatureBoxWrapper.SigMode.TreatPlan;
				string keyData=TreatPlans.GetKeyDataForSignatureHash(treatPlan,treatPlan.ListProcTPs);
				sigBoxWrapper.FillSignature(treatPlan.SigIsTopaz,keyData,treatPlan.Signature);
				if(sigBoxWrapper.GetNumberOfTabletPoints(treatPlan.SigIsTopaz)!=0) {
					return sigBoxWrapper.GetSigImage();
				}
			}
			else if(field.FieldType==SheetFieldType.SigBoxPractice && treatPlan.SignaturePractice!="") {
				SignatureBoxWrapper sigBoxWrapper=new SignatureBoxWrapper();
				sigBoxWrapper.SignatureMode=SignatureBoxWrapper.SigMode.TreatPlan;
				string keyData=TreatPlans.GetKeyDataForSignatureHash(treatPlan,treatPlan.ListProcTPs);
				sigBoxWrapper.FillSignature(treatPlan.SigIsTopaz,keyData,treatPlan.SignaturePractice);
				if(sigBoxWrapper.GetNumberOfTabletPoints(treatPlan.SigIsTopaz)!=0) {
					return sigBoxWrapper.GetSigImage();
				}
			}
			return new Bitmap(field.Width,field.Height);
		}

		///<summary>Deprecated: See GraphicsHelper.PixelsToPoints().  Converts pixels used by us to points used by PdfSharp.</summary>
		private static double p(int pixels){
			XUnit xunit=XUnit.FromInch((double)pixels/100d);//100 ppi
			return xunit.Point;
			//XUnit.FromInch((double)pixels/100);
		}

		///<summary>Deprecated: See GraphicsHelper.PixelsToPoints().  Converts pixels used by us to points used by PdfSharp.</summary>
		private static double p(float pixels){
			XUnit xunit=XUnit.FromInch((double)pixels/100d);//100 ppi
			return xunit.Point;
		}
		#endregion Private Methods
	}
}
