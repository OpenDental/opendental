using System;
using System.Drawing;
using System.Drawing.Printing;
using CodeBase;

namespace OpenDental {
	public partial class FormFriendlyException:FormODBase {
		private string _friendlyMessage;
		private Exception _exception;
		private int _defaultDetailsHeight;
		string _query;
		private int _pagesPrinted;
		private bool _isUnhandledException;

		/// <summary>Don't use this.  Use FriendlyException.Show().  The 5 places where this is called directly for Chart eRx might need to be refactored.</summary>
		public FormFriendlyException(string friendlyMessage,Exception ex,bool isUnhandledException) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_friendlyMessage=friendlyMessage;
			_exception=ex;
			_isUnhandledException=isUnhandledException;
			this.Text+=" - "+DateTime.Now.ToString();//Append to title of form.
		}

		private void FormFriendlyException_Load(object sender,EventArgs e) {
			labelFriendlyMessage.Text=_friendlyMessage;
			textDetails.Text=MiscUtils.GetExceptionText(_exception,isUnhandledException:_isUnhandledException);
			_query=((_exception as ODException)?.Query??"");
			_defaultDetailsHeight=tabControl.Height;
			if(_isUnhandledException) {
				FormFriendlyException_SetupUE();
			} 
			else {
				FormFriendlyException_SetupFriendlyException();
			}
			//textDetails is visible by default so that it actually has height.
			ResizeDetails();//Invoke the ResizeDetails method so that the details are hidden when the window initially loads for the user.
			if(string.IsNullOrEmpty(_query)) {
				tabControl.TabPages.RemoveByKey("tabQuery");
			}
			else {
				textQuery.Text=_query;
			}
		}

		private void FormFriendlyException_SetupUE() {
			butClose.Visible=true;
			butCloseFriendly.Visible=false;
			tabControl.Visible=false;//This will get switched to Visible=true when ResizeDetails() is called.
			labelDetails.Visible=false;
			butCopyAll.Visible=true;
			butCopyAllFriendly.Visible=false;
			butPrint.Visible=true;
			butPrintFriendly.Visible=false;
			Height-=_defaultDetailsHeight; //Stops height from being doubled when ResizeDetails() is called on UE.
		}

		//Our goal is to make FormFriendlyException look as similar to the windows msgbox.
		private void FormFriendlyException_SetupFriendlyException() { 
			butClose.Visible=false;
			butCloseFriendly.Visible=true;
			MinimizeBox=false;
			MaximizeBox=false;
			this.Text="";
			this.BackColor=Color.White;
		}

		private void labelDetails_Click(object sender,EventArgs e) {
			ResizeDetails();
		}

		///<summary>A helper method that toggles visibility of the details text box and adjusts the size of the form to accomodate the UI change.</summary>
		private void ResizeDetails() {
			if(tabControl.Visible) {
				tabControl.Visible=false;
				butCopyAll.Visible=false;
				butPrint.Visible=false;
				butCopyAllFriendly.Visible=false;
				butPrintFriendly.Visible=false;
				Height-=tabControl.Height;
			}
			else {
				tabControl.Visible=true;
				Height+=_defaultDetailsHeight;
				tabControl.Height=_defaultDetailsHeight;
				if(!_isUnhandledException) {
					butCopyAll.Visible=false;
					butPrint.Visible=false;
					butCopyAllFriendly.Visible=true;
					butPrintFriendly.Visible=true;
				}
			}
		}

		private void butCopyAll_Click(object sender,EventArgs e) {
			try {
				string content=this.Text+"\r\n"+textDetails.Text+GetQueryText();
				ODClipboard.SetClipboard(content);
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard. Please try again.");
				ex.DoNothing();
			}
		}

		private string GetQueryText() {
			return (string.IsNullOrEmpty(_query)?"":"\r\n-------------------------------------------\r\n"+_query);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			ODprintout.InvalidMinDefaultPageWidth=0;
			//No print previews, since this form is in and of itself a print preview.
			PrinterL.TryPrint(pd_PrintPage,margins:new Margins(50,50,50,50),duplex:Duplex.Horizontal);
		}
		
		///<summary>Called for each page to be printed.</summary>
		private void pd_PrintPage(object sender,PrintPageEventArgs e){
			e.HasMorePages=!Print(e.Graphics,_pagesPrinted++,e.MarginBounds);
		}

		///<summary>Prints one page. Returns true if pageToPrint is the last page in this print job.</summary>
		private bool Print(Graphics g,int pageToPrint,Rectangle margins){
			//Messages may span multiple pages. We print the header on each page as well as the page number.
			float baseY=margins.Top;
			string text="Page "+(pageToPrint+1);
			Font font=Font;
			SizeF textSize=g.MeasureString(text,font);
			g.DrawString(text,font,Brushes.Black,margins.Right-textSize.Width,baseY);
			baseY+=textSize.Height;
			text=Text;
			font=new Font(Font.FontFamily,16,FontStyle.Bold);
			textSize=g.MeasureString(text,font);
			g.DrawString(text,font,Brushes.Black,(margins.Width-textSize.Width)/2,baseY);
			baseY+=textSize.Height;
			font.Dispose();
			string[] messageLines=(textDetails.Text+GetQueryText())
				.Split(new string[] {Environment.NewLine},StringSplitOptions.None);
			font=Font;
			bool isLastPage=false;
			float y=0;
			for(int curPage=0,msgLine=0;curPage<=pageToPrint;curPage++){
				//Set y to its initial value for the current page (right after the header).
				y=curPage*(margins.Bottom-baseY);
				while(msgLine<messageLines.Length){
					//If a line is blank, we need to make sure that is counts for some vertical space.
					if(messageLines[msgLine]==""){
						textSize=g.MeasureString("A",font);
					}
					else{
						textSize=g.MeasureString(messageLines[msgLine],font);
					}
					//Would the current text line go past the bottom margin?
					if(y+textSize.Height>(curPage+1)*(margins.Bottom-baseY)){
						break;
					}
					if(curPage==pageToPrint){
						g.DrawString(messageLines[msgLine],font,Brushes.Black,margins.Left,baseY+y-curPage*(margins.Bottom-baseY));
						if(msgLine==messageLines.Length-1){
							isLastPage=true;
						}
					}
					y+=textSize.Height;
					msgLine++;
				}
			}
			return isLastPage;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}

	public class FriendlyException {
		///<summary>Assumes friendlyMessage is already translated.  Will throw an exception when running UnitTests.</summary>
		public static void Show(string friendlyMessage,Exception ex,bool isUnhandledException=false) {
			if(ODInitialize.IsRunningInUnitTest) {
				throw new Exception(friendlyMessage,ex);
			}
			try {
				using FormFriendlyException formFriendlyException=new FormFriendlyException(friendlyMessage,ex,isUnhandledException);
				formFriendlyException.ShowDialog();
			}
			catch(Exception e) {
				MessageBox.Show($"Error encountered: {ex.Message}\r\n\r\nAdditional error: {e.Message}");
			}			
		}
	}
}