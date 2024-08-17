using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public class PrinterL {
		
		///<summary>Rarely used.  When previewing, defines what control to show the printdoc in after validation.
		///After used one time, is cleared back to the default value of null.
		///This option is disabled when set to null.</summary>
		public static PrintPreviewControl ControlPreviewOverride=null;

		///<summary>Gets a PrintDocument that has some added functionality.  All printing in Open Dental should use this method (or an ODprintout object) for printing.</summary>
		///<param name="printPageEventHandler">The handler that will get invoked when printing.  This defines how to draw each page.</param>
		///<param name="printSituation">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="auditPatNum">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="auditDescription">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="margins">When set, this will override the default margins of "new Margins(25,25,40,40)".</param>
		///<param name="printoutOrigin">Defaults to printer default.  Set to AtMargin if the graphics origin starts at the page margins; AtZero if the graphics origin is at the top-left corner of the printable page.</param>
		///<param name="paperSize">When set, this will override the default paperSize of "new PaperSize("default",850,1100)".</param>
		///<param name="totalPages">When creating an ODprintout for print previewing, this defines the total number of pages.  Required if multiple pages needed when using Classic printing in FormPrintPreview.</param>
		///<param name="printoutOrientation">Defaults to printers default value.  Otherwise specify a value for either landscape or portrait.</param>
		///<param name="duplex">Use default unless double-sided printing is required.</param>
		///<param name="copies">Gets or sets the number of copies of the document to print.</param>
		///<returns>A new ODprintout with the given args that serves as a conduit for centralized printing and previewing methods with nice error messages.</returns>
		public static ODprintout CreateODprintout(PrintPageEventHandler printPageEventHandler=null,string auditDescription=""
			,PrintSituation printSituation=PrintSituation.Default,long auditPatNum=0,Margins margins=null,PrintoutOrigin printoutOrigin=PrintoutOrigin.Default
			,PaperSize paperSize=null,int totalPages=1,PrintoutOrientation printoutOrientation=PrintoutOrientation.Default,Duplex duplex=Duplex.Default,short copies=1
			,bool isErrorSuppressed=false)
		{
			ODprintout printout=new ODprintout(
				printPageEventHandler,
				printSituation,
				auditPatNum,
				auditDescription,
				margins,
				printoutOrigin,
				paperSize,
				printoutOrientation,
				duplex,
				copies,
				totalPages
			);
			if(!isErrorSuppressed && printout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				ShowError(printout);
			}
			return printout;
		}

		///<summary>Attempts to print if in RELEASE mode or if in DEBUG mode will open ODprintout in FormRpPrintPreview.</summary>
		///<param name="printPageEventHandler">The handler that will get invoked when printing.  This defines how to draw each page.</param>
		///<param name="auditDescription">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="printSituation">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="auditPatNum">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="margins">When set, this will override the default margins of "new Margins(25,25,40,40)".</param>
		///<param name="printoutOrigin">Defaults to printer default.  Set to AtMargin if the graphics origin starts at the page margins; AtZero if the graphics origin is at the top-left corner of the printable page.</param>
		///<param name="printoutOrientation">Defaults to printers default value.  Otherwise specify a value for either landscape or portrait.</param>
		///<returns>Returns true if succesfully printed, or if preview is shown and OK is clicked.</returns>
		public static bool TryPrintOrDebugRpPreview(PrintPageEventHandler printPageEventHandler,string auditDescription
			,PrintoutOrientation printoutOrientation=PrintoutOrientation.Default,PrintSituation printSituation=PrintSituation.Default,
			long auditPatNum=0,Margins margins=null,PrintoutOrigin printoutOrigin=PrintoutOrigin.Default,bool isForcedPreview=false)
		{
			ODprintout printout=new ODprintout(
				printPageEventHandler,
				printSituation,
				auditPatNum,
				auditDescription,
				margins,
				printoutOrigin,
				printoutOrientation:printoutOrientation,
				duplex:Duplex.Default
			);
			if(ODBuild.IsDebug() || isForcedPreview) {
				return RpPreview(printout);
			}
			return TryPrint(printout);
		}

		///<summary>Attempts to print if in RELEASE mode or if in DEBUG mode will open ODprintout in FormPrintPreview.</summary>
		///<param name="printPageEventHandler">The handler that will get invoked when printing.  This defines how to draw each page.</param>
		///<param name="auditDescription">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="printSituation">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="margins">When set, this will override the default margins of "new Margins(25,25,40,40)".</param>
		///<param name="printoutOrigin">Defaults to printer default.  Set to AtMargin if the graphics origin starts at the page margins; AtZero if the graphics origin is at the top-left corner of the printable page.</param>
		///<param name="printoutOrientation">Defaults to printers default value.  Otherwise specify a value for either landscape or portrait.</param>
		///<returns>Returns true if succesfully printed, or if preview is shown and OK is clicked.</returns>
		public static bool TryPrintOrDebugClassicPreview(PrintPageEventHandler printPageEventHandler,string auditDescription,Margins margins=null
			,int totalPages=1,PrintSituation printSituation=PrintSituation.Default,PrintoutOrigin printoutOrigin=PrintoutOrigin.Default
			,PrintoutOrientation printoutOrientation=PrintoutOrientation.Default,bool isForcedPreview=false,long auditPatNum=0,PaperSize paperSize=null)
		{
			ODprintout printout=new ODprintout(
				printPageEventHandler,
				printSituation,
				auditPatNum,
				auditDescription,
				margins,
				printoutOrigin,
				paperSize,
				printoutOrientation,
				totalPages:totalPages
			);
			if(ODBuild.IsDebug() || isForcedPreview) {
				return PreviewClassic(printout);
			}
			return TryPrint(printout);
		}

		///<summary>Attempts to print a PrintDocument that has some added functionality.  All printing in Open Dental should use this method (or an ODprintout object) for printing.</summary>
		///<param name="printPageEventHandler">The handler that will get invoked when printing.  This defines how to draw each page.</param>
		///<param name="printSituation">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="auditPatNum">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="auditDescription">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="margins">When set, this will override the default margins of "new Margins(25,25,40,40)".</param>
		///<param name="printoutOrigin">Defaults to printer default.  Set to AtMargin if the graphics origin starts at the page margins; AtZero if the graphics origin is at the top-left corner of the printable page.</param>
		///<param name="printoutOrientation">Defaults to printers default value.  Otherwise specify a value for either landscape or portrait.</param>
		///<param name="duplex">Use default unless double-sided printing is required.</param>
		///<returns>Returns true if succesfully printed.</returns>
		public static bool TryPrint(PrintPageEventHandler printPageEventHandler,string auditDescription="",long auditPatNum=0
			,PrintSituation printSituation=PrintSituation.Default,Margins margins=null,PrintoutOrigin printoutOrigin=PrintoutOrigin.Default
			,PrintoutOrientation printoutOrientation=PrintoutOrientation.Default,Duplex duplex=Duplex.Default)
		{
			ODprintout printout=new ODprintout(
				printPageEventHandler,
				printSituation,
				auditPatNum,
				auditDescription,
				margins,
				printoutOrigin,
				printoutOrientation:printoutOrientation,
				duplex:duplex
			);
			return TryPrint(printout);
		}

		///<summary>Whenever we are printing we should eventually go through this method.</summary>
		public static bool TryPrint(ODprintout printout) {
			if(!TrySetPrinter(printout)) {
				return false;
			}
			return printout.TryPrint();
		}
		
		///<summary>Attempts to preview (FormPrintPreview) a PrintDocument that has some added functionality.  All printing in Open Dental should use this method (or an ODprintout object) for printing.</summary>
		///<param name="printPageEventHandler">The handler that will get invoked when printing.  This defines how to draw each page.</param>
		///<param name="auditDescription">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="printSituation">ODprintout does not do anything with this field.  But when PrinterL.TrySetPrinter() is invoked we will provide the information if needed.</param>
		///<param name="margins">When set, this will override the default margins of "new Margins(25,25,40,40)".</param>
		///<param name="printoutOrigin">Defaults to printer default.  Set to AtMargin if the graphics origin starts at the page margins; AtZero if the graphics origin is at the top-left corner of the printable page.</param>
		///<param name="paperSize">When set, this will override the default paperSize of "new PaperSize("default",850,1100)".</param>
		///<param name="totalPages">When creating an ODprintout for print previewing, this defines the total number of pages.</param>
		///<param name="printoutOrientation">Defaults to printers default value.  Otherwise specify a value for either landscape or portrait.</param>
		///<returns>Returns true if preview is shown and OK is clicked.</returns>
		public static bool TryPreview(PrintPageEventHandler printPageEventHandler,string auditDescription,PrintSituation printSituation=PrintSituation.Default
			,Margins margins=null,PrintoutOrigin printoutOrigin=PrintoutOrigin.Default,PaperSize paperSize=null,PrintoutOrientation printoutOrientation=PrintoutOrientation.Default
			,int totalPages=1,bool doCalculateTotalPages=false)
		{
			ODprintout printout=new ODprintout(
				printPageEventHandler,
				printSituation,
				auditDescription:auditDescription,
				margins:margins,
				printoutOrigin:printoutOrigin,
				paperSize:paperSize,
				printoutOrientation:printoutOrientation,
				totalPages:totalPages
			);
			if(doCalculateTotalPages) {
				printout.TotalPages=0;
				printout.PrintDoc.PrintPage+=(sender,e)=>printout.TotalPages++;
			}
			return PreviewClassic(printout);
		}

		///<summary>This method is designed to be called every single time we print.  It helps figure out which printer to use, handles displaying dialogs if necessary,
		///and tests to see if the selected printer is valid, and if not then it gives user the option to print to an available printer.  
		///Also creates an audit trail entry with the AuditDescription text that is set within printDocument.
		///Debug mode will not display the print dialog but will instead prefer the default printer.</summary>
		public static bool TrySetPrinter(ODprintout printout) {
			if(printout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				ShowError(printout);
				return false;
			}
			return SetPrinter(printout.PrintDoc.PrinterSettings,printout.Situation,printout.AuditPatNum,printout.AuditDescription);
		}
		
		///<summary>DEPRECATED.</summary>
		public static bool SetPrinter(PrintDocument printDocument,PrintSituation printSituation,long patNum,string auditDescription) {
			return SetPrinter(printDocument.PrinterSettings,printSituation,patNum,auditDescription);
		}

		private static bool SetPrinter(PrinterSettings printerSettings,PrintSituation printSituation,long patNum,string auditDescription) {
			#region 1 - Set default printer if available from this computer.
			//pSet will always be new when this function is called.
			//Get the name of the Windows default printer.
			//This method only works when the pSet is still new.
			//string winDefault=pSet.PrinterName;
			//1. If a default printer is set in OD,
			//and it is in the list of installed printers, use it.
			bool showPrompt=false;
			Printer printerForSit=Printers.GetForSit(PrintSituation.Default);//warning: this changes
			string printerName="";
			if(printerForSit!=null){
				printerName=printerForSit.PrinterName;
				showPrompt=printerForSit.DisplayPrompt;
				if(Printers.PrinterIsInstalled(printerName)) {
					printerSettings.PrinterName=printerName;
				}
			}
			#endregion 1
			#region 2 - If a printer is set for this situation, and it is in the list of installed printers, use it.
			if(printSituation!=PrintSituation.Default){
				printerForSit=Printers.GetForSit(printSituation);
				printerName="";
				if(printerForSit!=null){
					printerName=printerForSit.PrinterName;
					showPrompt=printerForSit.DisplayPrompt;
					if(Printers.PrinterIsInstalled(printerName)) {
						printerSettings.PrinterName=printerName;
					}
				}
			}
			#endregion 2
			#region 3 - Present the dialog
			if(showPrompt && !ODBuild.IsWeb()) {
				PrintDialog printDialog=new PrintDialog();
				printDialog.AllowSomePages=true;
				printDialog.PrinterSettings=printerSettings;
				DialogResult dialogResult=printDialog.ShowDialog();
				printerSettings.Collate=true;
				if(dialogResult!=DialogResult.OK){
					return false;
				}
				if(printerSettings.PrintRange!=PrintRange.AllPages && printerSettings.ToPage<1) {
					//User set the range to not print any pages.
					return false;
				}
			}
			#endregion 3
			//Create audit log entry for printing.  PatNum can be 0.
			if(!string.IsNullOrEmpty(auditDescription)){
				SecurityLogs.MakeLogEntry(Permissions.Printing,patNum,auditDescription);
			}
			return true;
		}

		///<summary>Returns a translated error code description.</summary>
		public static string GetErrorStringFromCode(PrintoutErrorCode printoutErrorCode) {
			string message=Lan.g(nameof(PrinterL),printoutErrorCode.GetDescription());
			if(printoutErrorCode!=PrintoutErrorCode.Success) {
				message+="\r\n"+Lan.g(nameof(PrinterL),"If you do have a printer installed, restarting the workstation may solve the problem.");
			}
			return message;
		}

		///<summary>Shows a translated error message in a MessageBox for the given printDoc.</summary>
		public static void ShowError(ODprintout printout) {
			ShowError(GetErrorStringFromCode(printout.SettingsErrorCode),printout.ErrorEx);
		}

		///<summary>Helper method that shows a generic, translated printing error message or the msgOverride passed in.
		///If using msgOverride, must be translated before passing in.
		///Optionally pass an exception to include the exception.Message text at the end of the pop up.</summary>
		private static void ShowError(string msgOverride="",Exception ex=null) {
			string message=Lan.g(nameof(PrinterL),"There was an error while trying to print.");
			if(!string.IsNullOrEmpty(msgOverride)) {
				message=msgOverride;
			}
			if(ex!=null) {
				message+="\r\n"+ex.Message;
			}
			MessageBox.Show(message);//Message is translated above.
		}

		private static bool IsControlPreviewOverrideValid(ODprintout printout) {
			if(printout.SettingsErrorCode!=PrintoutErrorCode.Success) {//Mimics how preview form handles errors.
				ShowError(printout);
				return false;
			}
			ControlPreviewOverride.Document=printout.PrintDoc;
			ControlPreviewOverride=null;
			return true;
		}

		///<summary>Launches FormRpPrintPreview for the given printDoc.  Returns true if dialog result was OK; Otherwise false.</returns>
		private static bool RpPreview(ODprintout printout) {
			if(ControlPreviewOverride!=null) {
				return IsControlPreviewOverrideValid(printout);
			}
			FormRpPrintPreview formRpPrintPreview=new FormRpPrintPreview(printout);
			formRpPrintPreview.ShowDialog();
			formRpPrintPreview.BringToFront();
			return (formRpPrintPreview.DialogResult==DialogResult.OK);
		}
		
		///<summary>Launches FormPrintPreview for the given printDoc.  Returns true if dialog result was OK; Otherwise false.</returns>
		private static bool PreviewClassic(ODprintout printout) {
			if(ControlPreviewOverride!=null) {
				return IsControlPreviewOverrideValid(printout);
			}
			MakeMarginsFitWithinHardMargins(printout.PrintDoc);
			using FormPrintPreview formPrintPreview=new FormPrintPreview(printout);//This form is self validating.
			formPrintPreview.ShowDialog();
			formPrintPreview.BringToFront();
			return (formPrintPreview.DialogResult==DialogResult.OK);
		}

		///<summary>Ensures that the margins set on the current printdoc are within the hard margin values.
		///The hard margin represents the physical margin set by the printer.</summary>
		private static void MakeMarginsFitWithinHardMargins(PrintDocument printDocument) {
			Margins marginsDefault = printDocument.DefaultPageSettings.Margins;
			int hardMarginX = (int)printDocument.DefaultPageSettings.HardMarginX;
			int hardMarginY = (int)printDocument.DefaultPageSettings.HardMarginY;
			Margins marginsHard=new Margins(hardMarginX,hardMarginX,hardMarginY,hardMarginY);
			if(marginsDefault.Bottom < marginsHard.Bottom) {
				marginsDefault.Bottom=marginsHard.Bottom;
			}
			if(marginsDefault.Top < marginsHard.Top) {
				marginsDefault.Top=marginsHard.Top;
			}
			if(marginsDefault.Left < marginsHard.Left) {
				marginsDefault.Left=marginsHard.Left;
			}
			if(marginsDefault.Right < marginsHard.Right) {
				marginsDefault.Right=marginsHard.Right;
			}
		}
	}
}