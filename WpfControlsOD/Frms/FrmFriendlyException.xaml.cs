using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDental.Drawing;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmFriendlyException:FrmODBase {
		private string _friendlyMessage;
		private Exception _exception;
		private int _defaultDetailsHeight;
		private string _query;
		private int _pagesPrinted;
		private bool _isUnhandledException;
		private bool _showDetails;
		private int _widthWithoutTextQuery;
		///<summary>This is how we carry over to subsequent pages for printing.</summary>
		private int _idxCharsPrinted;
		/// <summary>Stores combination of text from both textBoxes for printing.</summary>
		private string _textToPrint;

		/// <summary>Don't use this.  Use FriendlyException.Show().  The 5 places where this is called directly for Chart eRx might need to be refactored.</summary>
		public FrmFriendlyException(string friendlyMessage,Exception ex,bool isUnhandledException) {
			InitializeComponent();
			_friendlyMessage=friendlyMessage;
			_exception=ex;
			_isUnhandledException=isUnhandledException;
			this.Text+=" - "+DateTime.Now.ToString();//Append to title of form.
			Load+=FrmFriendlyException_Load;
			PreviewKeyDown+=FrmFriendlyexception_PreviewKeyDown;
		}

		private void FrmFriendlyException_Load(object sender,EventArgs e) {
			Lang.F(this);
			_showDetails=true;
			labelFriendlyMessage.Text=_friendlyMessage;
			textStackTrace.Text=MiscUtils.GetExceptionText(_exception,isUnhandledException:_isUnhandledException);
			_query="";
			if((_exception as ODException)!=null) {
				_query=(_exception as ODException).Query;
			}
			_widthWithoutTextQuery=630;
			_defaultDetailsHeight=150;
			if(_isUnhandledException) {
				FormFriendlyException_SetupUE();
			} 
			else {
				FormFriendlyException_SetupFriendlyException();
			}
			//textDetails is visible by default so that it actually has height.
			ResizeDetails();//Invoke the ResizeDetails method so that the details are hidden when the window initially loads for the user.
			textQuery.Text=_query;
		}

		private void FormFriendlyException_SetupUE() {
			_showDetails=false;
			textStackTrace.Visible=false;//This will get switched to Visible=true when ResizeDetails() is called.
			textQuery.Visible=false;
			labelQuery.Visible=false;
			labelStackTrace.Visible=false;
			labelDetails.Visible=false;
			butCopyAll.Visible=false;
			butPrintOD.Visible=false;
			_formFrame.Size=new System.Drawing.Size(ScaleFormValue(1262),ScaleFormValue(_defaultDetailsHeight+16));//Stops height from being doubled when ResizeDetails() is called on UE.
			_formFrame.CenterFormOnMonitor();
		}

		//Our goal is to make FormFriendlyException look as similar to the windows msgbox.
		private void FormFriendlyException_SetupFriendlyException() { 
			this.MinMaxBoxes=false;
			this.Text="";
			this.Background=Brushes.White;
		}

		private void labelDetails_Click(object sender,MouseButtonEventArgs e) {
			ResizeDetails();
		}

		///<summary>A helper method that toggles visibility of the details text box and adjusts the size of the form to accomodate the UI change.</summary>
		private void ResizeDetails() {
			if(_showDetails) {
				textStackTrace.Visible=false;
				textQuery.Visible=false;
				labelQuery.Visible=false;
				labelStackTrace.Visible=false;
				butCopyAll.Visible=false;
				butPrintOD.Visible=false;
				_formFrame.Size=new System.Drawing.Size(ScaleFormValue(_widthWithoutTextQuery),ScaleFormValue(_defaultDetailsHeight));
				_showDetails=false;
			}
			else {
				_formFrame.Size=new System.Drawing.Size(ScaleFormValue(_widthWithoutTextQuery),ScaleFormValue(769));//Without query box.
				textStackTrace.Visible=true;
				labelStackTrace.Visible=true;
				if(!string.IsNullOrEmpty(_query)){
					_formFrame.Size=new System.Drawing.Size(ScaleFormValue(1262),ScaleFormValue(769));//With query box.
					textQuery.Visible=true;
					labelQuery.Visible=true;
				}
				if(!_isUnhandledException) {
					butCopyAll.Visible=true;
					butPrintOD.Visible=true;
				}
				_showDetails=true;
			}
			_formFrame.CenterFormOnMonitor();
		}

		private void butCopyAll_Click(object sender,EventArgs e) {
			string content=this.Text+"\r\n"+textStackTrace.Text+GetQueryText();
			try {
				ODClipboard.SetClipboard(content);
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard. Please try again.");
				ex.DoNothing();
			}
		}

		private string GetQueryText() {
			if(string.IsNullOrEmpty(_query)) {
				return "";
			}
			return"\r\n-------------------------------------------\r\n"+_query;
		}

		private void FrmFriendlyexception_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butPrintOD.IsAltKey(Key.P,e)) {
				butPrint_Click(this,new EventArgs());
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_textToPrint=textStackTrace.Text+GetQueryText();
			_pagesPrinted=0;
			_idxCharsPrinted=0;
			Printout printout=new Printout();
			printout.FuncPrintPage=pd_PrintPage;
			printout.thicknessMarginInches=new Thickness(0.5,0.4,0.25,0.4);
			WpfControls.PrinterL.TryPrintOrDebugClassicPreview(printout);
		}
		
		///<summary>Called for each page to be printed.</summary>
		private bool pd_PrintPage(Graphics g){
			//Messages may span multiple pages. We print the header on each page as well as the page number.
			Font font=new Font();
			double yPos=0;
			string text="Page "+(_pagesPrinted+1);//Page number heading
			Size sizeText=g.MeasureString(text,font);
			g.DrawString(text,font,Colors.Black,g.Width-sizeText.Width,yPos);
			yPos+=sizeText.Height+8;
			font=new Font();
			if(_pagesPrinted==0){
				text=_textToPrint;
			}
			else{
				text=_textToPrint.Substring(_idxCharsPrinted);
			}
			Size sizeAvail=new Size(g.Width,g.Height-yPos);
			int charactersFitted=g.MeasureCharactersFitted(text,font,sizeAvail);
			if(charactersFitted==0){
				//didn't run across this in testing, but I suppose it might happen
				_pagesPrinted++;
				return false;//no more pages
			}
			Rect rect=new Rect(new Point(0,yPos),sizeAvail);
			g.DrawString(text.Substring(0,charactersFitted),font,Colors.Black,rect);
			if(charactersFitted==text.Length){
				_pagesPrinted++;
				return false;//no more pages
			}
			_idxCharsPrinted=_idxCharsPrinted+charactersFitted;
			_pagesPrinted++;
			return true;//has more pages
		}
	}

	public class FriendlyException {
		///<summary>Assumes friendlyMessage is already translated.  Will throw an exception when running UnitTests.</summary>
		public static void Show(string friendlyMessage,Exception ex,bool isUnhandledException=false) {
			if(ODBuild.IsUnitTest) {
				throw new Exception(friendlyMessage,ex);
			}
			FrmFriendlyException frmFriendlyException=new FrmFriendlyException(friendlyMessage,ex,isUnhandledException);
			try {
				frmFriendlyException.ShowDialog();
			}
			catch(Exception e) {
				MessageBox.Show($"Error encountered: {ex.Message}\r\n\r\nAdditional error: {e.Message}");
			}
		}

	}
}