using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI{
	///<summary></summary>
	public partial class FormPrintPreview:FormODBase {
		private ODprintout _printout;

		///<summary>Uses the following ODprintout fields: Situation, TotalPages, AuditPatNum, AuditDescription.</summary>
		public FormPrintPreview(ODprintout printout) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_printout=printout;
			_printout.PrintDoc.EndPrint+=(sender,e)=>SetPageNumText();
		}

		private void FormPrintPreview_Load(object sender, System.EventArgs e) {
			LayoutToolBar();
			if(_printout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				PrinterL.ShowError(_printout);
				DialogResult=DialogResult.Cancel;
				return;
			}
			LoadPrintDocument(_printout.PrintDoc);
		}

		private void LoadPrintDocument(PrintDocument printdoc) {
			//if document fits within window, then don't zoom it bigger; leave it at 100%
			if(printdoc.DefaultPageSettings.PaperSize.Height<printPreviewControl2.ClientSize.Height
				&& printdoc.DefaultPageSettings.PaperSize.Width<printPreviewControl2.ClientSize.Width)
			{
				printPreviewControl2.Zoom=1;
			}
			//if document ratio is taller than screen ratio, shrink by height.
			else if(printdoc.DefaultPageSettings.PaperSize.Height
				/printdoc.DefaultPageSettings.PaperSize.Width
				> printPreviewControl2.ClientSize.Height / printPreviewControl2.ClientSize.Width)
			{
				printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Height
					/(double)printdoc.DefaultPageSettings.PaperSize.Height);
			}
			//otherwise, shrink by width
			else{
				printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Width
					/(double)printdoc.DefaultPageSettings.PaperSize.Width);
			}
			printPreviewControl2.Document=printdoc;
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",1,"Go Back One Page","Back"));
			ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			button.Style=ODToolBarButtonStyle.Label;
			ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
		}

		private delegate void PrintClick();

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			//MessageBox.Show(e.Button.Tag.ToString());
			switch(e.Button.Tag.ToString()){
				case "Print":
					//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
					//when it comes from a toolbar click.
					//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
					PrintClick printClick=OnPrint_Click;
					this.BeginInvoke(printClick);
					break;
				case "Back":
					OnBack_Click();
					break;
				case "Fwd":
					OnFwd_Click();
					break;
				case "Close":
					OnClose_Click();
					break;
			}
		}

		private void OnPrint_Click() {
			if(_printout.PrintDoc.OriginAtMargins) {
				//In the sheets framework,we had to set margins to 20 because of a bug in their preview control.
				//We now need to set it back to 0 for the actual printing.
				//Hopefully, this doesn't break anything else.
				_printout.PrintDoc.DefaultPageSettings.Margins=new Margins(0,0,0,0);
			}
			if(!PrinterL.TryPrint(_printout)) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void OnClose_Click() {
			this.Close();
		}

		private void OnBack_Click(){
			if(printPreviewControl2.StartPage==0) return;
			printPreviewControl2.StartPage--;
			SetPageNumText();
			ToolBarMain.Invalidate();
		}

		private void OnFwd_Click(){
			//if(printPreviewControl2.StartPage==totalPages-1) return;
			printPreviewControl2.StartPage++;
			SetPageNumText();
			ToolBarMain.Invalidate();
		}

		///<summary>Sets the toolbar's pagenum text based on the total pages. If 0 total pages, only shows the current pagenum.</summary>
		private void SetPageNumText() {
			if(_printout.TotalPages==0) {
				ToolBarMain.Buttons["PageNum"].Text=(printPreviewControl2.StartPage+1).ToString();
			}
			else {
				ToolBarMain.Buttons["PageNum"].Text=(printPreviewControl2.StartPage+1).ToString()
								+" / "+_printout.TotalPages.ToString();
			}
		}
	
	

		

		

		

		


	}
}
