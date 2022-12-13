using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using CodeBase;

namespace OpenDental {
	public class ODcodeBox:RichTextBox {
		ContextMenu contextMenu = new ContextMenu();
		private int _lastLineWidth=0;
		///<summary>Set to -1 when the line number width needs to be recalculated.</summary>
		private int _lineNumberWidth=-1;
		///<summary></summary>
		private SizeF _lineNumberSize=new SizeF();
		///<summary>Holds the indexes of each newline char within _stringTextMain.  Set to null when needed to recalculate.</summary>
		private List<int> _listIndicesOfNewLines;
		///<summary>The string format that keeps the numbers on the left centered and aligned with the adjacent text.</summary>
		private StringFormat _stringFormatForDrawingNumbers;
		///<summary>The actual format that goes into the parenthesis of the ToString() to format the string. E.g. {0:D3}</summary>
		private string _stringFormatForNumbers;
		private Font _lineNumberFont=new Font("Courier New",9F,FontStyle.Regular);
		private Brush _brushOlive=new SolidBrush(Color.Olive);

		public ODcodeBox() : this(false) {
			//Parameterless constructor is required for controls that can display within the Visual Studio designer.
		}

		public ODcodeBox(bool isQueryText) {
			_stringFormatForDrawingNumbers=new StringFormat();
			//Center the text horizontally
			_stringFormatForDrawingNumbers.Alignment=StringAlignment.Center;
			//Shove the number to the top (so that long sentences that span multiple "lines" but have no newline chars only show as one line number.
			//This makes it so that a large blank space will show underneath any additional space needed between line numbers.
			//This looks really good so don't mess with this setting.
			_stringFormatForDrawingNumbers.LineAlignment=StringAlignment.Near;
			_stringFormatForDrawingNumbers.Trimming=StringTrimming.None;
			//setup context menu
			EventHandler onClickCut=new EventHandler(menuItemCut_Click);
			EventHandler onClickCopy=new EventHandler(menuItemCopy_Click);
			EventHandler onClickPaste=new EventHandler(menuItemPaste_Click);
			EventHandler onClickSelectAll=new EventHandler(menuItemSelectAll_Click);
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Cut"),onClickCut,Shortcut.CtrlX));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Copy"),onClickCopy,Shortcut.CtrlC));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Paste"),onClickPaste,Shortcut.CtrlV));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Select All"),onClickSelectAll,Shortcut.CtrlA));
			if(isQueryText) {
				EventHandler onClickCommentLine=new EventHandler(menuItemAddComment_Click);
				EventHandler onClickUnCommentLine=new EventHandler(menuItemRemoveComment_Click);
				contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Comment Selection"),onClickCommentLine,Shortcut.CtrlShiftC));
				contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Remove Comment From Selection"),onClickUnCommentLine,Shortcut.CtrlShiftR));
			}
			this.ContextMenu = contextMenu;
		}

		private List<int> GetListIndicesOfNewLines() {
			if(_listIndicesOfNewLines!=null) {
				return _listIndicesOfNewLines;
			}
			_listIndicesOfNewLines=new List<int>();
			int index=0;
			foreach(char character in this.Text) {
				if(character=='\n') {
					_listIndicesOfNewLines.Add(index);
				}
				index++;
			}
			//Always add a new line for the very last line.
			_listIndicesOfNewLines.Add(this.Text.Length-1);
			return _listIndicesOfNewLines;
		}
		
		private int GetLineNumberWidth() {
			if(_lineNumberWidth > 0) {
				return _lineNumberWidth;
			}
			int numberOfDigits=1;
			if(GetListIndicesOfNewLines().Count>0) {
				numberOfDigits=(int)(1+Math.Log((double)GetListIndicesOfNewLines().Count,10));
			}
			string widthForDigits=new string('Z',numberOfDigits);
			using(Bitmap bmp=new Bitmap(1,1))
			using(Graphics g=Graphics.FromImage(bmp)) 
			{
				_lineNumberSize=g.MeasureString(widthForDigits,_lineNumberFont);
				_lineNumberWidth=(int)Math.Ceiling(_lineNumberSize.Width+10);
				_stringFormatForNumbers="{0:D"+numberOfDigits+"}";
				return _lineNumberWidth;
			}
		}

		///<summary>Recalculates the line numbers on every text change.</summary>
		protected override void OnTextChanged(EventArgs e) {
			LineNumberRecalculate();
			base.OnTextChanged(e);
		}

		///<summary>Nulls out variables so that the text boxes recalculate their contents on paint.</summary>
		private void LineNumberRecalculate() {
			_listIndicesOfNewLines=null;
			_lineNumberWidth=-1;
			WindowsApiWrapper.SendMessage(this.Handle,(int)WindowsApiWrapper.WinMessagesOther.WM_PAINT,0,0);
		}

		protected override void WndProc(ref Message m) {
			if(m.Msg==(int)WindowsApiWrapper.WinMessagesOther.WM_CHAR){
				LineNumberRecalculate();
				base.WndProc(ref m);
				return;
			}
			if(m.Msg==(int)WindowsApiWrapper.WinMessagesOther.WM_PAINT){
				base.WndProc(ref m);
				using(Graphics g=Graphics.FromHwnd(Handle)){
					PaintLineNumbers(g);
				}
				return;
			}
			base.WndProc(ref m);
		}

		private void PaintLineNumbers(Graphics g) {
			int lineNumberWidth=GetLineNumberWidth();
			//If the line number width has changed, we need to grow or shrink the left margin and repaint.
			if(lineNumberWidth!=_lastLineWidth) {
				SetLeftMargin(lineNumberWidth+Margin.Left);
				_lastLineWidth=lineNumberWidth;
				g.FillRectangle(Brushes.White,ClientRectangle);
				WindowsApiWrapper.SendMessage(Handle,(int)WindowsApiWrapper.WinMessagesOther.WM_PAINT,0,0);
				return;
			}
			//Figure out how many total lines can be shown on the screen.
			int visibleLineCount=(int)Math.Ceiling((double)Height / _lineNumberSize.ToSize().Height);
			int minLineCount=Math.Min(visibleLineCount,GetListIndicesOfNewLines().Count);
			int firstLineNumber=GetListIndicesOfNewLines().Count;
			int firstCharIndex=this.GetCharIndexFromPosition(new Point(1,1));
			int idx=0;
			foreach(int index in GetListIndicesOfNewLines()) {
				idx++;
				if(firstCharIndex<=index) {
					firstLineNumber=idx;
					break;
				}
			}
			using(Bitmap doubleBufferer=new Bitmap(lineNumberWidth,Height))
			using(Graphics gDoubleBuffer=Graphics.FromImage(doubleBufferer))
			{
				Rectangle rect=new Rectangle(0,0,lineNumberWidth,Height);
				gDoubleBuffer.FillRectangle(SystemBrushes.ControlLight,rect);
				int curNumberWidth=lineNumberWidth;
				List<int> listIndicesNewLines=GetListIndicesOfNewLines();
				for(int i=firstLineNumber;i<=listIndicesNewLines.Count;i++) {
					int curPointY=0;
					if(firstLineNumber!=1) {
						//Get the Y position of the character after the new line character of the previous line.
						curPointY=GetPosFromCharIndex(listIndicesNewLines[i-2]+1).Y;//2 because we want the previous line and the list is 0 based
					}
					if(Height<=curPointY) {
						break;
					}
					Rectangle rectDraw=new Rectangle(0,curPointY,curNumberWidth,_lineNumberSize.ToSize().Height);
					gDoubleBuffer.DrawString(firstLineNumber.ToString(),_lineNumberFont,_brushOlive,rectDraw,_stringFormatForDrawingNumbers);
					firstLineNumber++;
				}
				g.DrawImage(doubleBufferer,new Point(0,0));
			}
		}

		///<summary>Fills a memory address to quickly get the point of the char index within the RichTextBox from the Windows API wrapper.  This is MUCH faster than using RichTextBox.GetPositionFromCharIndex().</summary>
		private Point GetPosFromCharIndex(int index) {
			int rawPointSize=Marshal.SizeOf(typeof(Point));
			IntPtr wParam=Marshal.AllocHGlobal(rawPointSize);
			WindowsApiWrapper.SendMessage(this.Handle,(int)WindowsApiWrapper.EM_Rich.EM_POSFROMCHAR,wParam,index);
			Point point=(Point)Marshal.PtrToStructure(wParam,typeof(Point));
			Marshal.FreeHGlobal(wParam);
			return point;
		}

		private void SetLeftMargin(int widthInPixels) {
			WindowsApiWrapper.SendMessage(this.Handle,(int)WindowsApiWrapper.EM_Rich.EM_SETMARGINS,WindowsApiWrapper.EC_LEFTMARGIN,
				widthInPixels);
		}

		public void SetScroll(int delta) {
			WindowsApiWrapper.SendMessage(this.Handle,(int)WindowsApiWrapper.EM_Rich.EM_LINESCROLL,0,delta);
		}

		#region context menu events
		private void menuItemCopy_Click(object sender, EventArgs e) {
			this.Copy();
		}

		private void menuItemPaste_Click(object sender,EventArgs e) {
			this.Paste();
		}

		private void menuItemCut_Click(object sender,EventArgs e) {
			this.Cut();
		}

		private void menuItemSelectAll_Click(object sender, EventArgs e) {
			this.SelectAll();
		}

		private void menuItemRemoveComment_Click(object sender, EventArgs e) {
			//remove block comment, only remove if opening and closing is selected.
			if(this.SelectedText.Contains("/*") && this.SelectedText.Contains("*/")) {
				string temp = this.SelectedText;
				temp=temp.Replace("/*","");
				temp=temp.Replace("*/","");
				this.SelectedText=temp;
				return;
			}
			//no text selected, remove comment at beginning of line if present, otherwise don't change the string
			int newLineIndex = this.Text.Substring(0,this.SelectionStart).LastIndexOf("\n");
			if(newLineIndex == -1) {
				if(this.Text.Substring(0,3) == "-- ") {
					int saveIndex = this.SelectionStart;
					this.Select(0,3);
					this.SelectedText=this.SelectedText.Replace("-- ","");
					this.Select(saveIndex-3,0);
					return;
				}
			}
			else {
				if(this.Text.Substring(newLineIndex+1,3) =="-- ") {
					int saveIndex = this.SelectionStart;
					this.Select(newLineIndex+1,3);
					this.SelectedText=this.SelectedText.Replace("-- ","");
					this.Select(saveIndex-3,0);
					return;
				}
			}
		}

		private void menuItemAddComment_Click(object sender,EventArgs e) {
			string selectedComment = this.SelectedText;
			//add block comment around selected text
			if(!string.IsNullOrEmpty(selectedComment)) {
				this.SelectedText=$"/*{selectedComment}*/";
				return;
			}
			//no text is selected, comment out a line
			//finding if cursor is on the first line or after first line
			int newLineIndex = this.Text.Substring(0,this.SelectionStart).LastIndexOf("\n");
			if(newLineIndex == -1) {
				//no new line, we are on the first line, add comment to beginning of first line
				this.Select(0,0);
				this.SelectedText=$"-- {this.SelectedText}";
			}
			else {
				//we are on some other line, add comment to the beginning of this line
				this.Select(newLineIndex+1,0);
				this.SelectedText=$"-- {this.SelectedText}";
			}
		}
		#endregion



	}
}