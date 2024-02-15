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
using OpenDentBusiness;
using WpfControls.UI;
using System.Text.RegularExpressions;
using CodeBase;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Printing;
using OpenDental.Drawing;

namespace OpenDental {
	public partial class FrmMsgBoxCopyPaste:FrmODBase {
		private int _pagesPrinted;
		///<summary>This is how we carry over to subsequent pages.</summary>
		private int _idxCharsPrinted;

		///<summary>This presents a message box to the user, but is better because it allows user to copy the text and paste it into another program for testing.  Especially useful for queries.</summary>
		public FrmMsgBoxCopyPaste(string displayText){
			InitializeComponent();
			textMain.Text=displayText;
			Load+=FrmMsgBoxCopyPaste_Load;
			PreviewKeyDown+=FrmMsgBoxCopyPaste_PreviewKeyDown;
		}

		private void FrmMsgBoxCopyPaste_Load(object sender,EventArgs e) {
			Lang.F(this);
		}

		private void FrmMsgBoxCopyPaste_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
			if(butPrint.IsAltKey(Key.P,e)) {
				butPrint_Click(this,new EventArgs());
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_idxCharsPrinted=0;
			Printout printout=new Printout();
			printout.FuncPrintPage=pd_PrintPage;
			WpfControls.PrinterL.TryPrintOrDebugClassicPreview(printout);
		}

		///<summary>Called for each page to be printed. Returns true if has more pages.</summary>
		private bool pd_PrintPage(Graphics g){
			//Messages may span multiple pages. We print the header on each page as well as the page number.
			Font font=new Font();
			double yPos=0;
			string text="Page "+(_pagesPrinted+1);
			Size sizeText=g.MeasureString(text,font);
			g.DrawString(text,font,Colors.Black,g.Width-sizeText.Width,yPos);
			yPos+=sizeText.Height;
			text=Text;
			font.Size=10;
			font.IsBold=true;
			sizeText=g.MeasureString(text,font);
			g.DrawString(text,font,Colors.Black,(g.Width-sizeText.Width)/2,yPos);
			yPos+=sizeText.Height;
			text=DateTime.Now.ToString();
			sizeText=g.MeasureString(text,font);
			g.DrawString(text,font,Colors.Black,(g.Width-sizeText.Width)/2,yPos);
			yPos+=sizeText.Height+8;
			font=new Font();
			if(_pagesPrinted==0){
				text=textMain.Text;
			}
			else{
				text=textMain.Text.Substring(_idxCharsPrinted);
			}
			Size sizeAvail=new Size(g.Width,g.Height-yPos);
			int charactersFitted=g.MeasureCharactersFitted(text,font,sizeAvail);
			if(charactersFitted==0){
				//didn't run across this in testing, but I suppose it might happen
				_pagesPrinted++;
				return false;//no more pages
			}
			Rect rect=new Rect(new Point(0,yPos),sizeAvail);
			//We can't just let extra text spill off the bottom because it will show partial line
			g.DrawString(text.Substring(0,charactersFitted),font,Colors.Black,rect);
			if(charactersFitted==text.Length){
				_pagesPrinted++;
				return false;//no more pages
			}
			_idxCharsPrinted=_idxCharsPrinted+charactersFitted;			
			_pagesPrinted++;
			return true;//has more pages
		}

		private void butCopyAll_Click(object sender,EventArgs e) {
			try {
				ODClipboard.SetClipboard(textMain.Text);
			}
			catch(Exception ex) {
				MessageBox.Show("Could not copy contents to the clipboard. Please try again.");
				ex.DoNothing();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			IsDialogOK=true;
		}


	}
}
