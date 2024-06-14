using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using OpenDental;
using OpenDentBusiness;
using OpenDental.Drawing;
using CodeBase;//for PrintoutErrorCode

namespace WpfControls {
//Jordan is the only one allowed to edit this file.
/*
Also see drawing instructions in GraphicsWPF.
How to use:
This should allow you to use existing WinForms code with very little modification.
Margins
	Margins will tend to be an issue. 
	In the old code we sometimes treated 0,0 as the UL of the inset margin, 
	and we sometimes treated 0,0 as the UL of the actual page.
	But WPF has much better margin functionality baked into everything.
	Also, we've designed our framework with solid margin handling.
	Here, it's always UL of the inset margin. Specify margin separately.
	To treat 0,0 as UL of page, simply set margins to 0.
	But only do this in areas where the user would not expect you to use margins.
	Example of zero margins might be the ADA claim form.
Documents in WPF are a new concept. We can choose between FlowDocuments and FixedDocuments.
	In old WinForms, we directly controlled all layout, so it was very similar to a FixedDocument.
	This PrinterL can internally only handle FixedDocuments.
	FlowDocuments are very different and would need to use a totally different class and a different kind of print preview.

Example:
using OpenDental.Drawing;

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			Printout printout=new Printout();
			printout.FuncPrintPage=pd_PrintPage;
			printout.thicknessMarginInches=new Thickness(0.5,0.4,0.25,0.4);//optional
			WpfControls.PrinterL.TryPreview(printout);//or TryPrint, etc

then change:
		private void pd_PrintPage(object sender,PrintPageEventArgs e){
	to:
		private bool pd_PrintPage(Graphics g){
*/
	public class PrinterL {
		private static void CreateFixedDocument(Printout printout){
			printout.FixedDocument_=new FixedDocument();
			Thickness thicknessMarginInches=new Thickness(left:0.25,top:0.4,right:0.25,bottom:0.4);
			if(printout.thicknessMarginInches!=new Thickness()){
				thicknessMarginInches=printout.thicknessMarginInches;
			}
			Thickness thicknessMargin=new Thickness(
				thicknessMarginInches.Left*96,
				thicknessMarginInches.Top*96,
				thicknessMarginInches.Right*96,
				thicknessMarginInches.Bottom*96);
			while(true){
				PageContent pageContent=new PageContent();
				printout.FixedDocument_.Pages.Add(pageContent);
				FixedPage fixedPage=new FixedPage();
				pageContent.Child=fixedPage;
				fixedPage.Width=printout.FixedDocument_.DocumentPaginator.PageSize.Width;
				fixedPage.Height=printout.FixedDocument_.DocumentPaginator.PageSize.Height;
				//fixedPage.Margin=thicknessMargin;//this was suggested but seems wrong
				Canvas canvas=new Canvas();//each page gets its own canvas
				//We must respect margins, so this is how we do it:
				//So canvas is our area where we can draw, and the drawing logic does not get access to margins.
				FixedPage.SetLeft(canvas,thicknessMargin.Left);
				FixedPage.SetTop(canvas,thicknessMargin.Top);
				canvas.Width=fixedPage.Width-thicknessMargin.Left-thicknessMargin.Right;
				canvas.Height=fixedPage.Height-thicknessMargin.Top-thicknessMargin.Bottom;
				Graphics g=Graphics.PrinterInit(canvas);
				fixedPage.Children.Add(canvas);
				bool hasMorePages=printout.FuncPrintPage(g);
				if(!hasMorePages){
					break;
				}
			}
		}

		///<summary>Returns a translated error code description.</summary>
		public static string GetErrorStringFromCode(PrintoutErrorCode printoutErrorCode) {
			string message=Lang.g(nameof(PrinterL),printoutErrorCode.GetDescription());
			if(printoutErrorCode!=PrintoutErrorCode.Success) {
				message+="\r\n"+Lang.g(nameof(PrinterL),"If you do have a printer installed, restarting the workstation may solve the problem.");
			}
			return message;
		}

		///<summary>Launches FormPrintPreview for the given printDoc.  Returns true if dialog result was OK; Otherwise false.</returns>
		private static bool PreviewClassic(Printout printout) {
//todo: build robust margin functionality
			//MakeMarginsFitWithinHardMargins(printout.PrintDoc);
			FrmPrintPreview frmPrintPreview=new FrmPrintPreview();
			CreateFixedDocument(printout);
			frmPrintPreview.FixedDocumentCur=printout.FixedDocument_;
			frmPrintPreview.ShowDialog();
			return frmPrintPreview.IsDialogOK;
		}

		private static bool SetPrinter(PrinterSettings printerSettings,PrintSituation printSituation,long patNum,string auditDescription) {
			#region 1 - Set default printer if available from this computer.
			//printerSettings will always be new when this function is called.
			//Get the name of the Windows default printer.
			//This method only works when the printerSettings is still new.
			//string winDefault=printerSettings.PrinterName;
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
			if(showPrompt && !ODBuild.IsThinfinity()) {
				PrintDialog printDialog=new PrintDialog();
				//printDialog.AllowSomePages=true;
//todo:
				//printDialog.PrinterSettings=printerSettings;
				bool? dialogResult=printDialog.ShowDialog();
				printerSettings.Collate=true;
				if(dialogResult!=true){ 
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
				SecurityLogs.MakeLogEntry(EnumPermType.Printing,patNum,auditDescription);
			}
			return true;
		}
		
		///<summary>Shows a translated error message in a MessageBox for the given printDoc.</summary>
		public static void ShowError(Printout printout) {
			ShowError(GetErrorStringFromCode(printout.SettingsErrorCode),printout.ErrorEx);
		}

		///<summary>Helper method that shows a generic, translated printing error message or the msgOverride passed in.
		///If using msgOverride, must be translated before passing in.
		///Optionally pass an exception to include the exception.Message text at the end of the pop up.</summary>
		private static void ShowError(string msgOverride="",Exception ex=null) {
			string message=Lang.g(nameof(PrinterL),"There was an error while trying to print.");
			if(!string.IsNullOrEmpty(msgOverride)) {
				message=msgOverride;
			}
			if(ex!=null) {
				message+="\r\n"+ex.Message;
			}
			MessageBox.Show(message);//Message is translated above.
		}

		///<summary>Returns true if preview is shown and then printed.</summary>
		public static bool TryPreview(Printout printout){
			return PreviewClassic(printout);
		}

		///<summary>Whenever we are printing we should eventually go through this method.</summary>
		public static bool TryPrint(Printout printout) {
			if(!TrySetPrinter(printout)) {
				return false;
			}
			PrintDialog printDialog=new PrintDialog();
			CreateFixedDocument(printout);
			printDialog.PrintDocument(printout.FixedDocument_.DocumentPaginator,printout.Descript);
			return true;
		}

		///<summary>Attempts to print if in RELEASE mode or if in DEBUG mode will open ODprintout in FormPrintPreview. Returns true if succesfully printed, or if preview is shown and OK is clicked.</summary>
		public static bool TryPrintOrDebugClassicPreview(Printout printout){
			if(ODBuild.IsDebug() || printout.IsForcedPreview) {
				return PreviewClassic(printout);
			}
			return TryPrint(printout);
		}

		///<summary>This method is designed to be called every single time we print.  It helps figure out which printer to use, handles displaying dialogs if necessary,
		///and tests to see if the selected printer is valid, and if not then it gives user the option to print to an available printer.  
		///Also creates an audit trail entry with the AuditDescription text that is set within printDocument.
		///Debug mode will not display the print dialog but will instead prefer the default printer.</summary>
		public static bool TrySetPrinter(Printout printout) {
			if(printout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				ShowError(printout);
				return false;
			}
			return true;
//todo:
			//return SetPrinter(printout.PrintDoc.PrinterSettings,printout.Situation,printout.AuditPatNum,printout.AuditDescription);
		}

		


		
	}
}
