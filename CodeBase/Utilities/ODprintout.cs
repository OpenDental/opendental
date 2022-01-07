using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace CodeBase {
	///<summary>A wrapper class for PrintDocument that has additional validation and error handling.  
	///Also acts like a vessel for helpful variables that are often used when printing.</summary>
	public class ODprintout {
		///<summary></summary>
		private PrintDocument _printDoc;
		///<summary>Used to determin the printiner that should be used.</summary>
		public PrintSituation Situation=PrintSituation.Default;
		///<summary></summary>
		public int TotalPages=1;
		///<summary>The description that is shown on the audi trails. If blank nothing will be entered.</summary>
		public string AuditDescription="";
		///<summary>Used when making audit trails.</summary>
		public long AuditPatNum=0;
		///<summary>Is set when an error occurs during loading and printing.</summary>
		public PrintoutErrorCode SettingsErrorCode=PrintoutErrorCode.Success;
		///<summary>Is set when an error occurs during loading and printing.</summary>
		public Exception ErrorEx=null;
		///<summary>Rarely used.  During construction, if the printer default page height is less than this value,
		///then will be considered to be invalid and will be automatically set to our usual default page size.
		///After used one time, is cleared back to the default value of 0.
		///This option is disabled when set to -1.</summary>
		public static int InvalidMinDefaultPageHeight=0;
		///<summary>Rarely used.  During construction, if the printer default page width is less than this value,
		///then will be considered to be invalid and will be automatically set to our usual default page size.
		///After used one time, is cleared back to the default value of -1.
		///This option is disabled when set to -1.</summary>
		public static int InvalidMinDefaultPageWidth=-1;
		/////<summary>Rarely used.  During construction, will first set the printer settings to the value in this variable,
		/////then overwrite with the settings specified in the construction parameters.
		/////After used one time, is cleared back to the default value of null.</summary>
		//public static PrinterSettings InitPrintSettings=null;
		///<summary>Is set to the last instantiated ODprintout for reference purposes.</summary>
		public static ODprintout CurPrintout=null;
		
		///<summary>Returns true if printer can be validated.
		///If false then SettingsErrorCode will contain more detailed information.</summary>
		public bool HasValidSettings {
			get {
				try {
					SettingsErrorCode=PrintoutErrorCode.Success;
					if(PrinterSettings.InstalledPrinters.Count==0) {
						SettingsErrorCode=PrintoutErrorCode.NoInstalledPrinter;
						return false;
					}
					if(_printDoc.PrinterSettings==null) {//Should not happen
						SettingsErrorCode=PrintoutErrorCode.PrinterSettingsNotFound;
						return false;
					}
					if(_printDoc.PrinterSettings.PrinterName==null) {
						SettingsErrorCode=PrintoutErrorCode.PrinterNameNotFound;
						return false;
					}
					if(!_printDoc.PrinterSettings.IsValid) {
						SettingsErrorCode=PrintoutErrorCode.InvalidPrinterSettings;
						return false;
					}
					//Try to find the printable area.  If there are no printers, it will throw an InvalidPrinterException.
					//We use the following pattern to deterimine which page size to use.
					//The values are not used here, we simply are accessing them the same way to see if an exception is thrown.
					//Therefore we will know when we check later the code will not fail.
					if(_printDoc.DefaultPageSettings.PrintableArea.Width==0 || _printDoc.DefaultPageSettings.PrintableArea.Height==0) {
						//At least one valid printer is installed.
					}
				}
				catch(InvalidPrinterException ex) {//No printers installed.
					ex.DoNothing();
					SettingsErrorCode=PrintoutErrorCode.InvalidPrinterSettings;
					ErrorEx=ex;
					return false;
				}
				catch(Win32Exception wex) {
					wex.DoNothing();
					SettingsErrorCode=PrintoutErrorCode.PrinterConnectionError;
					ErrorEx=wex;
					return false;
				}
				return true; 
			}
		}

		///<summary></summary>
		public PrintDocument PrintDoc {
			get {
				return _printDoc;
			}
		}

		///<summary>Creates an empty ODPrintDocuement to get the printiners default PaperSize.
		///If not printer is installed this will return a PaperSize with width of 850 and height of 1100.</summary>
		public static PaperSize DefaultPaperSize {
			get {
				try {
					return (new PrintDocument().DefaultPageSettings.PaperSize);
				}
				catch(Exception ex) {
					ex.DoNothing();
					return new PaperSize("default",850,1100);
				}
			}
		}

		///<summary>Gets a PrintDocument that has some added functionality.  All printing in Open Dental should use this method (or an ODprintout object) for printing.</summary>
		///<param name="printPageEventHandler">The handler that will get invoked when printing.  This defines how to draw each page.</param>
		///<param name="printSit">ODprintout does not do anything with this field. But when ValidationDelegate is invoked we will provide the information if needed.</param>
		///<param name="auditPatNum">ODprintout does not do anything with this field. But when ValidationDelegate is invoked we will provide the information if needed.</param>
		///<param name="auditDescription">ODprintout does not do anything with this field. But when ValidationDelegate is invoked we will provide the information if needed.</param>
		///<param name="margins">When set, this will override the default margins of "new Margins(25,25,40,40)".</param>
		///<param name="printoutOrigin">Defaults to printer default.  Set to AtMargin if the graphics origin starts at the page margins; AtZero if the graphics origin is at the top-left corner of the printable page.</param>
		///<param name="paperSize">When set, this will override the default paperSize of "new PaperSize("default",850,1100)".</param>
		///<param name="totalPages">When creating an ODprintout for print previewing, this defines the total number of pages.</param>
		///<param name="printoutOrientation">Defaults to printers default value.  Otherwise specify a value for either landscape or portrait.</param>
		///<param name="duplex">Typically set when performing double-sided printing.</param>
		///<param name="copies">Gets or sets the number of copies of the document to print.</param>
		///<param name="totalPages">ODprintout does not do anything with this field. But when ValidationDelegate is invoked we will provide the information if needed. Defaults to 1.</param>
		///<param name="printOrPreviewExceptionDelegate">Any custom delegate that the calling method wants to happen when printing or previewing throws an exception.</param>
		///<param name="tryPreviewDelegate">Required to be implemented if the calling method needs the ability to preview.</param>
		///<param name="tryPrintOrDebugPreviewDelegate">Same as tryPreviewDelegate, but defines isPreview based on if program is in DEBUG mode.</param>
		///<returns>A new ODprintout with the given args that serves as a conduit safe printing and previewing methods.</returns>
		public ODprintout(PrintPageEventHandler printPageEventHandler=null,PrintSituation printSit=PrintSituation.Default,long auditPatNum=0,string auditDescription=""
			,Margins margins=null,PrintoutOrigin printoutOrigin=PrintoutOrigin.Default,PaperSize paperSize=null,PrintoutOrientation printoutOrientation=PrintoutOrientation.Default
			,Duplex duplex=Duplex.Default,short copies=1,int totalPages=1) : base()
		{
			CurPrintout=this;
			_printDoc=new PrintDocument();
			//if(InitPrintSettings!=null) {
			//	_printDoc.PrinterSettings=InitPrintSettings;
			//	InitPrintSettings=null;
			//}
			Situation=printSit;
			AuditPatNum=auditPatNum;
			AuditDescription=auditDescription;
			TotalPages=totalPages;
			if(!HasValidSettings) {
				return;
			}
			_printDoc.PrintPage+=printPageEventHandler;
			if(printoutOrientation!=PrintoutOrientation.Default) {
				_printDoc.DefaultPageSettings.Landscape=(printoutOrientation==PrintoutOrientation.Landscape)?true:false;
			}
			if(printoutOrigin!=PrintoutOrigin.Default) {
				_printDoc.OriginAtMargins=(printoutOrigin==PrintoutOrigin.AtMargin)?true:false;
			}
			_printDoc.DefaultPageSettings.Margins=(margins??new Margins(25,25,40,40));
			if(paperSize==null) {
				//This prevents a bug caused by some printer drivers not reporting their papersize.
				//But remember that other countries use A4 paper instead of 8 1/2 x 11.
				if((InvalidMinDefaultPageWidth!=-1 && _printDoc.DefaultPageSettings.PrintableArea.Width<=InvalidMinDefaultPageWidth)
					|| (InvalidMinDefaultPageHeight!=-1 && _printDoc.DefaultPageSettings.PrintableArea.Height<=InvalidMinDefaultPageHeight))
				{
					_printDoc.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
				}
				InvalidMinDefaultPageHeight=0;
				InvalidMinDefaultPageWidth=-1;
			}
			else {
				_printDoc.DefaultPageSettings.PaperSize=paperSize;
			}
			_printDoc.PrinterSettings.Copies=copies;
			if(duplex!=Duplex.Default) {
				_printDoc.PrinterSettings.Duplex=duplex;
			}
			if(ODBuild.IsWeb()) {
				//https://referencesource.microsoft.com/#System.Drawing/commonui/System/Drawing/Printing/PrintDocument.cs,3f3c2622b65be86a
				//The PrintController property will create a PrintControllerWithStatusDialog if no print controller is explictly set.
				_printDoc.PrintController=new StandardPrintController();//Default PrintController shows a dialog that locks up web.
			}
		}

		///<summary>Invokes PrintDocument.Print() after validating printer settings and any custom validation specified within the ValidationDelegate.
		///Returns true if successfully printed, otherwise false if there are invalid printer settings or problems with printing.
		///PrintOrPreviewExceptionDelegate will get invoked with the specific exception that occurred while printing if the calling method cares to know what went wrong.</summary>
		public bool TryPrint() {
			if(SettingsErrorCode!=PrintoutErrorCode.Success) {
				return false;
			}
			try {
				Form activeForm=Form.ActiveForm;
				_printDoc.Print();
				ODException.SwallowAnyException(() => {
					//Sometimes after printing the application behind OD comes in front of OD. This is to help fix that.
					if(activeForm!=null && (Form.ActiveForm==null || Form.ActiveForm.Name=="")) {
						activeForm.Activate();
					}
				});
			}
			catch(Exception ex) {
				ErrorEx=ex;
				return false;
			}
			return true;
		}
		
	}	

	///<summary>Used to identify the printer to use for the given PrintSituation.</summary>
	public enum PrintSituation {
		///<summary>0- Covers any printing situation not listed separately.</summary>
		Default,
		///<summary>1</summary>
		Statement,
		///<summary>2</summary>
		LabelSingle,
		///<summary>3</summary>
		Claim,
		///<summary>4- TP and perio</summary>
		TPPerio,
		///<summary>5</summary>
		Rx,
		///<summary>6</summary>
		LabelSheet,
		///<summary>7</summary>
		Postcard,
		///<summary>8</summary>
		Appointments,
		///<summary>9</summary>
		RxControlled,
		///<summary>10</summary>
		Receipt,
		///<summary>11</summary>
		RxMulti
	}
	
	///<summary>Used to identify specific reasons that validaiton failed during construction.
	///Set to PrintoutErrorCode.Success when validation is passed.</summary>
	public enum PrintoutErrorCode {
		///<summary>0</summary>
		[Description("No error.")]
		Success,
		///<summary>1</summary>
		[Description("Error: No printers installed.")]
		NoInstalledPrinter,
		///<summary>2</summary>
		[Description("Error: Printers settings not found.")]
		PrinterSettingsNotFound,
		///<summary>3</summary>
		[Description("Error: Printer name not found.")]
		PrinterNameNotFound,
		///<summary>4</summary>
		[Description("Error: Printer settings found but are flagged as invalid.")]
		InvalidPrinterSettings,
		///<summary>5</summary>
		[Description("Error: An error occurred while attempting to connect to the printer.")]
		PrinterConnectionError,
	}
	
	///<summary></summary>
	public enum PrintoutOrigin {
		///<summary>0 - Use printer default.</summary>
		Default,
		///<summary>1 - Priting begins at (margin left,margin top).  Starts at the page margins.</summary>
		AtMargin,
		///<summary>2 - Printing begins at (0,0).  Top-left corner of the printable page.</summary>
		AtZero
	}
	
	///<summary></summary>
	public enum PrintoutOrientation {
		///<summary>0 - Use printer default.</summary>
		Default,
		///<summary>1</summary>
		Landscape,
		///<summary>2</summary>
		Portrait
	}

}
