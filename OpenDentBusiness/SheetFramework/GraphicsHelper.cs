using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
//using System.Windows;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using OpenDentBusiness;
using System.Drawing.Drawing2D;
using CodeBase;
using Google.Apis.Util;

namespace OpenDentBusiness {
	public class GraphicsHelper {
		///<summary>Creates a textbox from the text sheetfield provided.  Used for display and for text wrapping calculations. Calling methods are responsible for explicitly disposing of the returned text box itself and it's Font property.</summary>
		public static RichTextBox CreateTextBoxForSheetDisplay(SheetField sheetField,bool isClipText=true,RichTextBox richTextBox=null) {
			if(richTextBox==null) {
				richTextBox=new RichTextBox();
			}
			richTextBox.BorderStyle=BorderStyle.None;
			richTextBox.TabStop=false;//Only input fields allow tab stop (set below for input text).
			//Input fields need to have a yellow background so that they stand out to the user.
			if(sheetField.FieldType==SheetFieldType.InputField) {
				richTextBox.BackColor=Color.FromArgb(245,245,200);
				richTextBox.TabStop=(sheetField.TabOrder>0);
				richTextBox.TabIndex=sheetField.TabOrder;
			}
			richTextBox.Location=new Point(sheetField.XPos,sheetField.YPos);
			richTextBox.Width=sheetField.Width;
			richTextBox.ScrollBars=RichTextBoxScrollBars.None;
			if(sheetField.ItemColor!=Color.FromArgb(0)){
				richTextBox.ForeColor=sheetField.ItemColor;
			}
			richTextBox.SelectionAlignment=sheetField.TextAlign;
			richTextBox.Multiline=IsTextBoxMultiline(richTextBox,sheetField:sheetField);//Needs to be set before we set the 'SelectedText' field or else the 'Text' field could be incorrectly truncated.
			richTextBox.SelectedText=sheetField.FieldValue;
			FontStyle style=FontStyle.Regular;
			if(sheetField.FontIsBold) {
				style=FontStyle.Bold;
			}
			richTextBox.Font=new Font(sheetField.FontName,sheetField.FontSize,style);
			richTextBox.Height=sheetField.Height;
			richTextBox.ScrollBars=RichTextBoxScrollBars.None;
			richTextBox.Tag=sheetField;
			#region Text Clipping
			//For multi-line textboxes, clip vertically to remove lines which extend beyond the bottom of the field.
			if(isClipText && richTextBox.Text.Length > 0 && richTextBox.Multiline) {
				List<RichTextLineInfo> listLines=GetTextSheetDisplayLines(richTextBox);
				//Only clip text from a multiline text box when there is more than one line of text.
				if(listLines.Count > 1) {
					int fitLineIndex=listLines.Count-1;//Line numbers are zero-based.
					while(fitLineIndex >= 0	&& listLines[fitLineIndex].Top+richTextBox.Font.Height-1 > richTextBox.Height) {//if bottom of line is past bottom of textbox
						fitLineIndex--;
					}
					if(fitLineIndex < 0) {//Every single line of text falls below the bottom of the field (the sheet field is too small for the font).
						//Always show the first line of text regardless of the bounds of the control. A user will never purposefully make a sheet def with an invisible text box.
						//Also, we know that there are at least 2 lines of text in the text box so hard code the Text property to the very first line of text.
						richTextBox.Text=richTextBox.Text.Substring(0,listLines[1].FirstCharIndex);//Truncate text after the very first line of text.
					}
					else if(fitLineIndex < listLines.Count-1) {//If at least one line was truncated from the bottom.
						richTextBox.Text=richTextBox.Text.Substring(0,listLines[fitLineIndex+1].FirstCharIndex);//Truncate text to the end of the fit line.
					}
				}
			}
			//For single-line textboxes, clip the text to the width of the textbox.  This forces the display to look the same on screen as when printed.
			if(isClipText && richTextBox.Text.Length > 0 && !richTextBox.Multiline) {
				int indexRight=richTextBox.GetCharIndexFromPosition(new Point(richTextBox.Width,0));
				if(indexRight < richTextBox.Text.Length-1) {//If the string is too wide for the textbox.
					richTextBox.Text=richTextBox.Text.Substring(0,indexRight+1);//Truncate the text to fit the textbox width.
				}
			}
			#endregion Text Clipping
			//Take over the ability to paste formatted text into the RTB.
			richTextBox.KeyDown+=(o,e) => {
				//Two different ways to paste are "Ctrl + V" and "Shift + Insert".
				if((e.Control && e.KeyCode==Keys.V) || (e.Shift && e.KeyCode==Keys.Insert)) {
					try {
						if(System.Windows.Clipboard.ContainsText()) {//System.Windows.Forms.Clipboard fails for Thinfinity
							//Paste just the plain text value from the clipboard.
							richTextBox.Paste(DataFormats.GetFormat(DataFormats.Text));
						}
					}
					catch(Exception ex) {
						//Several of the methods above can fail for different reasons. E.g. "Requested Clipboard operation did not succeed"
						ex.DoNothing();
					}
					e.Handled=true;//Do not let the base Paste function to be invoked because it allows pasting formatted text, pictures, etc.
				}
			};
			return richTextBox;
		}

		///<summary>Creates a textbox from the text sheetfield provided.  Used for display and for text wrapping calculations. Calling methods are responsible for explicitly disposing of the returned text box itself and it's Font property.</summary>
		public static RichTextBox CreateTextBoxForSheetDisplay(string text,Font font,int width,int height,HorizontalAlignment align,bool isClipText=true){
			SheetField field=new SheetField();
			field.FieldType=SheetFieldType.StaticText;
			field.TabOrder=0;
			field.XPos=0;
			field.YPos=0;
			field.Width=width;
			field.ItemColor=Color.Black;
			field.TextAlign=align;
			field.FieldValue=text;
			field.FontIsBold=font.Bold;	
			field.FontName=font.Name;
			field.FontSize=font.Size;
			field.Height=height;
			return CreateTextBoxForSheetDisplay(field,isClipText);
		}

		///<summary>gdi+ has had a bug since the beginning: All pen widths between 1 and 1.5 show as 1. This helper method fixes this limitation.</summary>
		public static void DrawLine(Graphics g,Pen pen,float x1,float y1,float x2,float y2){
			if(pen.Width>1.5f || pen.Width==1){
				g.DrawLine(pen,x1,y1,x2,y2);
				return;
			}
			System.Windows.Vector vector=new System.Windows.Vector(x2-x1,y2-y1);//connect the dots
			System.Windows.Vector vector12oclock=new System.Windows.Vector(0,1);
			double angle=System.Windows.Vector.AngleBetween(vector12oclock,vector);
			PointF pointCenter=new PointF(x1+(x2-x1)/2f,y1+(y2-y1)/2f);
			GraphicsState graphicsState=g.Save();
			g.TranslateTransform(pointCenter.X,pointCenter.Y);
			g.RotateTransform((float)angle);
			using(SolidBrush brush=new SolidBrush(pen.Color)){
				g.FillRectangle(brush,-pen.Width/2,-(float)vector.Length/2,pen.Width,(float)vector.Length);
			}
			g.Restore(graphicsState);
			//the result of the above is unexpectedly and stunningly an exactly perfect match for GDI+ line drawing.
		}

		///<summary>Draws a rectangle with rounded edges. Since you probably also want to do a fill, GetRoundedPath is better.  Remember to dispose of the GraphicsPath.</summary>
		public static void DrawRoundedRectangle(Graphics g,Pen pen,RectangleF rectangleF,float radiusCorner) {
			GraphicsState graphicsState=g.Save();
			g.SmoothingMode=SmoothingMode.AntiAlias;
			//top,right
			g.DrawLine(pen,rectangleF.Left+radiusCorner,rectangleF.Top,rectangleF.Right-radiusCorner,rectangleF.Top);
			g.DrawArc(pen,rectangleF.Right-radiusCorner*2,rectangleF.Top,radiusCorner*2,radiusCorner*2,270,90);
			//right,bottom
			g.DrawLine(pen,rectangleF.Right,rectangleF.Top+radiusCorner,rectangleF.Right,rectangleF.Bottom-radiusCorner);
			g.DrawArc(pen,rectangleF.Right-radiusCorner*2,rectangleF.Bottom-radiusCorner*2,radiusCorner*2,radiusCorner*2,0,90);
			//bottom,left
			g.DrawLine(pen,rectangleF.Right-radiusCorner,rectangleF.Bottom,rectangleF.Left+radiusCorner,rectangleF.Bottom);
			g.DrawArc(pen,rectangleF.Left,rectangleF.Bottom-radiusCorner*2,radiusCorner*2,radiusCorner*2,90,90);
			//left,top
			g.DrawLine(pen,rectangleF.Left,rectangleF.Bottom-radiusCorner,rectangleF.Left,rectangleF.Top+radiusCorner);
			g.DrawArc(pen,rectangleF.Left,rectangleF.Top,radiusCorner*2,radiusCorner*2,180,90);
			g.Restore(graphicsState);
		}

		///<summary></summary>
		public static RectangleF DrawString(Graphics g,string str,Font font,Brush brush,Rectangle rectangle,HorizontalAlignment align) {
			if(str.Trim()=="") {
				return rectangle;//Nothing to draw.
			}
			//Screen is at 96 dpi (DIPs actually) and printer is at 100 dpi (device units actually)
			//Font follows dpi, so fonts always draw 4% bigger on printers.
			//We will increase the text width and height by 4% (by dividing by .96) to make wrapping identical.
			//But this still doesn't make everything perfect.
			//To be perfect, we would need to adjust positions of all elements by 4%.
			//That would be a massive undertaking, affecting the drawing logic at all levels.
			//Not practical right now, and only a tiny benefit for a huge cost.
			//We can't do g.ScaleTransform() because that would also scale the font size, so it wouldn't fix the wrap.
			//Shrinking the font size by 4% wouldn't work because fonts scale incrementally instead of smoothly.
			//The remaining imperfection will only be noticeable when a tall section of text spills down too close to the next element.
			RectangleF rectangleActual=new RectangleF(rectangle.X,rectangle.Y,(rectangle.Width/0.96f),(rectangle.Height/0.96f));
			StringFormat stringFormat=new StringFormat();
			//The overload for DrawString that takes a StringFormat will cause the tabs '\t' to be ignored.
			//In order for the tabs to not get ignored, we have to tell StringFormat how many pixels each tab should be.
			//50.0f is the closest to our Fill Sheet Edit preview.
			stringFormat.SetTabStops(0.0f,new float[] { 50.0f });
			stringFormat.Alignment=StringAlignment.Near;
			if(align==HorizontalAlignment.Center){
				stringFormat.Alignment=StringAlignment.Center;
			}
			if(align==HorizontalAlignment.Right){
				stringFormat.Alignment=StringAlignment.Far;
			}
			g.DrawString(str,font,brush,rectangleActual,stringFormat);
			stringFormat?.Dispose();
			return rectangleActual;
		}

		///<summary></summary>
		public static RectangleF DrawStringX(XGraphics xg,string str,XFont xfont,XBrush xbrush,RectangleF rectangleF,HorizontalAlignment horizontalAlignment) {
			if(str.Trim()=="") {
				return rectangleF;//Nothing to draw.
			}
			Bitmap bitmap=new Bitmap(100,100);//only used for measurements.
			bitmap.SetResolution(96,96);
			Graphics g=Graphics.FromImage(bitmap);
			//There are two coordinate systems here: pixels (used by us) and points (used by PdfSharp)..
			//PDFSharp is not capable of wrapping, so we need to do that manually.
			//Our incoming rectangleF is already in pixels.
			//It does not need to be adjusted by 4% like for the printer.
			//We need to do all our measurement using Graphics, not XGraphics.
			FontStyle fontstyle=FontStyle.Regular;
			if(xfont.Style==XFontStyle.Bold) {
				fontstyle=FontStyle.Bold;
			}
			Font font=new Font(xfont.Name,(float)xfont.Size,fontstyle);
			bool hasNonAscii=str.Any(x => x > 127);
			if(hasNonAscii) {
				XPdfFontOptions options=new XPdfFontOptions(PdfFontEncoding.Unicode,PdfFontEmbedding.Always);
				xfont=new XFont(xfont.Name,xfont.Size,xfont.Style,options);
			}
			else {
				xfont=new XFont(xfont.Name,xfont.Size,xfont.Style);
			}
			SizeF sizeLayout=new SizeF(rectangleF.Width,font.Height);
			StringFormat stringFormat=new StringFormat();
			stringFormat.SetTabStops(0.0f, new float[] {50.0f});//helps with measurement further down.
			stringFormat.Trimming=StringTrimming.Word;
			float pixelsPerLine=font.GetHeight();
			XStringFormat xStringFormat=new XStringFormat();//or maybe XStringFormats.Default
			xStringFormat.Alignment=XStringAlignment.Near;
			if(horizontalAlignment==HorizontalAlignment.Center){
				xStringFormat.Alignment=XStringAlignment.Center;
			}
			if(horizontalAlignment==HorizontalAlignment.Right){
				xStringFormat.Alignment=XStringAlignment.Far;
			}
			float lineIdx=0;
			int chars;
			//this loop adds chars each time, which is one line's worth of text.
			for(int i=0;i<str.Length;i+=chars) {
				//Example. We have drawn 1 line and we are getting ready to draw the second line.
				//For this example, with text wrap, this text will only be two lines high, so this is also the last line.
				//lineIdx=1. if(rect.Y+(heightRow*2)>rect.Bottom) then no room so kick out.
				if(rectangleF.Y+pixelsPerLine*(lineIdx+1)>rectangleF.Bottom) {//Check if rectangleF is tall enough to show next line.
					break;
				}
				//pixels:
				//TextRenderer.MeasureText(str.Substring(i),font, //no overload for measuring line by line
				//sizeLayout is a rectangle one line high, so we are measuring how much will fit in one line.
				//_lines variable below is thrown away.
				g.MeasureString(str.Substring(i),font,sizeLayout,stringFormat,out chars,out int _lines);
				//Newline characters \r\n, \r, and \n will not be recognized in Unicode PDF and will create rectangles on the screen, so since g.MeasureString has
				//already calculated the next new line that will appear on the screen, we can remove the unneeded newline characters from the current substring.
				string substring=str.Substring(i,chars);
				substring=substring.Replace("\r\n","");
				substring=substring.Replace("\r","");
				substring=substring.Replace("\n","");
				substring=substring.Replace("\t","    ");
				//use points here:
				double x=PixelsToPoints(rectangleF.X);
				if(horizontalAlignment==HorizontalAlignment.Right){
					x=PixelsToPoints(rectangleF.Right);
				}
				if(horizontalAlignment==HorizontalAlignment.Center){
					x=PixelsToPoints(rectangleF.X+rectangleF.Width/2f);
				}
				double y=PixelsToPoints(rectangleF.Y+pixelsPerLine*lineIdx);
				xg.DrawString(substring,xfont,xbrush,x,y,xStringFormat);
				lineIdx+=1;
			}
			g.Dispose();
			stringFormat?.Dispose();
			font?.Dispose();
			//xStringFormat?.Dispose();//Does not exist
			return rectangleF;
		}

		///<summary>If lineIndex is past the last line, then the information of the last line will be returned. </summary>
		public static RichTextLineInfo GetOneTextLine(RichTextBox richTextbox,int lineIndex) {
			if(lineIndex<0 || GetTextLineCount(richTextbox)==0) {
				return new RichTextLineInfo();
			}
			RichTextLineInfo lineInfo=new RichTextLineInfo();
			//GetFirstCharIndexFromLine() returns -1 if lineIndex is past the last line.
			lineInfo.FirstCharIndex=richTextbox.GetFirstCharIndexFromLine(lineIndex);//-1 if lineIndex >= count of lines in rtb.
			if(lineInfo.FirstCharIndex==-1) {//Return the last line's information.
				lineIndex=GetTextLineCount(richTextbox)-1;
				lineInfo.FirstCharIndex=richTextbox.GetFirstCharIndexFromLine(lineIndex);//First character of last line.
			}
			Point posThisLine=richTextbox.GetPositionFromCharIndex(lineInfo.FirstCharIndex);
			Point posNextLine;
			if(lineIndex!=GetTextLineCount(richTextbox)-1) {//This is not the last line.
				posNextLine=richTextbox.GetPositionFromCharIndex(richTextbox.GetFirstCharIndexFromLine(lineIndex+1));//Top of next line=bottom of this line.
			}
			else { //Only do this "phony last line" logic when we are looking at the actual last line.  Trying to copy all the content/formatting of an 
				//entire RichTextBox causes all kinds of printing/Pdf/alignment/spacing issues in Sheets and Forms.  
				using(RichTextBox rtb=new RichTextBox()) {
					//Copy values to new RichTextBox because we might modify text below.
					rtb.Rtf=richTextbox.Rtf;
					//Setting the rtb.Font property in this RichTextBox causes an Out of Memory Exception when the Treatment Finder report attempted to run for 100+ 
					//pdfs. Attempts were made to employ a "using" to dispose of the font, as well as to ensure the textbox passing it in was disposed of, 
					//however neither method succeeded. Since the only importance of the Font in this context is getting the position of the Next Line, it is also 
					//not necessary here because we obtain that from the Rtf and postNextLine. It's removal should not change the user experience. However, it will
					//remain here, commented out, for future information so that others do not attempt to employ a font here.
					//rtb.Font=textbox.Font;
					rtb.Size=richTextbox.Size;
					//According to MSDN, "If no paragraph is selected in the control, setting this property applies the alignment setting to the paragraph in
					//which the insertion point appears as well as to paragraphs created after the paragraph that has the alignment property setting."
					//https://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.selectionalignment(v=vs.110).aspx
					//When copying the RTF from the given textbox it also passes the selection to the new RichTextBox which is needed for the SelectionAlignment.
					rtb.SelectionAlignment=richTextbox.SelectionAlignment;
					rtb.AppendText("\r\n\t");//Add a phony line.
					posNextLine=rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(lineIndex+1));//Top of next line=bottom of this line.
				}
			}
			lineInfo.Left=posThisLine.X;
			lineInfo.Top=posThisLine.Y;
			lineInfo.Bottom=posNextLine.Y;
			return lineInfo;
		}

		public static GraphicsPath GetRoundedPath(RectangleF rectangleF,float radiusCorner){
			//There is a similar copy in ContrApptPanel.
			GraphicsPath graphicsPath=new GraphicsPath();
			graphicsPath.AddLine(rectangleF.Left+radiusCorner,rectangleF.Top,rectangleF.Right-radiusCorner,rectangleF.Top);//top
			graphicsPath.AddArc(rectangleF.Right-radiusCorner*2,rectangleF.Top,radiusCorner*2,radiusCorner*2,270,90);//UR
			graphicsPath.AddLine(rectangleF.Right,rectangleF.Top+radiusCorner,rectangleF.Right,rectangleF.Bottom-radiusCorner);//right
			graphicsPath.AddArc(rectangleF.Right-radiusCorner*2,rectangleF.Bottom-radiusCorner*2,radiusCorner*2,radiusCorner*2,0,90);//LR
			graphicsPath.AddLine(rectangleF.Right-radiusCorner,rectangleF.Bottom,rectangleF.Left+radiusCorner,rectangleF.Bottom);//bottom
			graphicsPath.AddArc(rectangleF.Left,rectangleF.Bottom-radiusCorner*2,radiusCorner*2,radiusCorner*2,90,90);//LL
			graphicsPath.AddLine(rectangleF.Left,rectangleF.Bottom-radiusCorner,rectangleF.Left,rectangleF.Top+radiusCorner);//left
			graphicsPath.AddArc(rectangleF.Left,rectangleF.Top,radiusCorner*2,radiusCorner*2,180,90);//UL
			return graphicsPath;
		}

		///<summary>Only rounds some corners.</summary>
		public static GraphicsPath GetRoundedPathPartial(RectangleF rectangleF,float radiusCorner,bool roundUL=false,bool roundUR=false,bool roundLL=false,bool roundLR=false)
		{
			GraphicsPath graphicsPath=new GraphicsPath();
			float radiusUR=0;
			if(roundUR){
				radiusUR=radiusCorner;
			}
			float radiusUL=0;
			if(roundUL){
				radiusUL=radiusCorner;
			}
			float radiusLR=0;
			if(roundLR){
				radiusLR=radiusCorner;
			}
			float radiusLL=0;
			if(roundLL){
				radiusLL=radiusCorner;
			}
			graphicsPath.AddLine(rectangleF.Left+radiusUL,rectangleF.Top,rectangleF.Right-radiusUR,rectangleF.Top);//top
			if(roundUR){
				graphicsPath.AddArc(rectangleF.Right-radiusUR*2,rectangleF.Top,radiusUR*2,radiusUR*2,270,90);//UR
			}
			graphicsPath.AddLine(rectangleF.Right,rectangleF.Top+radiusUR,rectangleF.Right,rectangleF.Bottom-radiusLR);//right
			if(roundLR){
				graphicsPath.AddArc(rectangleF.Right-radiusLR*2,rectangleF.Bottom-radiusLR*2,radiusLR*2,radiusLR*2,0,90);//LR
			}
			graphicsPath.AddLine(rectangleF.Right-radiusLR,rectangleF.Bottom,rectangleF.Left+radiusLL,rectangleF.Bottom);//bottom
			if(roundLL){
				graphicsPath.AddArc(rectangleF.Left,rectangleF.Bottom-radiusLL*2,radiusLL*2,radiusLL*2,90,90);//LL
			}
			graphicsPath.AddLine(rectangleF.Left,rectangleF.Bottom-radiusLL,rectangleF.Left,rectangleF.Top+radiusUL);//left
			if(roundUL){
				graphicsPath.AddArc(rectangleF.Left,rectangleF.Top,radiusUL*2,radiusUL*2,180,90);//UL
			}
			return graphicsPath;
		}

		public static int GetTextLineCount(RichTextBox richTextbox) {
			if(richTextbox.Text.Length==0) {
				return 0;
			}
			return richTextbox.GetLineFromCharIndex(richTextbox.Text.Length-1)+1;//GetLineFromCharIndex() returns a zero-based index.
		}

		///<summary>Returns a list of strings representing the lines of sheet text which will display on screen when viewing the specified text.</summary>
		public static List<RichTextLineInfo> GetTextSheetDisplayLines(RichTextBox richTextbox) {
			return GetTextSheetDisplayLines(richTextbox,-1,-1);
		}

		///<summary>Set start index or end index to -1 to return information for all lines. Returns a list of strings representing the lines of sheet text which will display on screen when viewing the specified text.</summary>
		public static List<RichTextLineInfo> GetTextSheetDisplayLines(RichTextBox richTextbox,int startCharIndex,int endCharIndex) {
			List <RichTextLineInfo> listLines=new List<RichTextLineInfo>();
			int startLineIndex=0;
			int endLineIndex=0;
			if(startCharIndex==-1 || endCharIndex==-1) {//All lines.
				startLineIndex=0;
				endLineIndex=GetTextLineCount(richTextbox)-1;
			}
			else {
				startLineIndex=richTextbox.GetLineFromCharIndex(startCharIndex);
				endLineIndex=richTextbox.GetLineFromCharIndex(endCharIndex);
			}
			for(int i=startLineIndex;i<=endLineIndex;i++) {
				listLines.Add(GetOneTextLine(richTextbox,i));
			}
			return listLines;
		}

		///<summary>This function is critical for measuring dynamic text fields on sheets when displaying or printing. Measures the size of a text string when displayed on screen.  This also differs from the regular MeasureString in that it will correctly measure trailing carriage returns as requiring additional lines.</summary>
		public static HeightAndChars MeasureStringH(string text,Font font,int width,int heightAvail,HorizontalAlignment align) {
			Bitmap bitmap=new Bitmap(100,100);//only used for measurements.
			bitmap.SetResolution(96,96);
			Graphics g=Graphics.FromImage(bitmap);
			SizeF sizeF=new SizeF(width,heightAvail);
			StringFormat stringFormat=new StringFormat();
			stringFormat.Trimming=StringTrimming.Word;
			stringFormat.Alignment=StringAlignment.Near;
			if(align==HorizontalAlignment.Center) {
				stringFormat.Alignment=StringAlignment.Center;
			}
			else if(align==HorizontalAlignment.Right) {
				stringFormat.Alignment=StringAlignment.Far;
			}
			SizeF sizeFFit=g.MeasureString(text,font,sizeF,stringFormat,out int charactersFitted,out int linesFilled);//don't care about linesFilled
			bitmap.Dispose();
			g.Dispose();
			HeightAndChars heightAndChars= new HeightAndChars();
			heightAndChars.Chars=charactersFitted;
			heightAndChars.Height=(int)Math.Floor(sizeFFit.Height);
			return heightAndChars;
		}

		/// <summary>Returns whether a given RichTextBox is multiline or not. Considers single line textBoxes with GrowthBehavior to be multiline. Expects either a SheetField or a SheetDef, not both. </summary>
		public static bool IsTextBoxMultiline(RichTextBox richTextBox,SheetField sheetField=null,SheetFieldDef sheetFieldDef=null) {
			if((sheetField==null && sheetFieldDef==null) || richTextBox==null || (sheetField != null && sheetFieldDef != null)) {
				return false;
			}
			bool isSingleLineTextBox;
			if(sheetField==null) {
				isSingleLineTextBox=(sheetFieldDef.Height<=richTextBox.Font.Height+2);
				if(sheetFieldDef.FieldType==SheetFieldType.InputField && isSingleLineTextBox && sheetFieldDef.GrowthBehavior==GrowthBehaviorEnum.None) {
					//There is a chance that data can get truncated when loading text into a text box that has Multiline set to false.
					//E.g. the text "line 1\r\nline 2\r\nline 3" will get truncated to be "line 1"
					//This causes a nasty bug where the user could have filled out the sheet as a Web Form (which does not truncate the text) and signed the sheet.
					//The signature would invalidate if the office were to open the downloaded web form within Open Dental proper and simply click OK.
					//Therefore, only allow InputField text boxes to have Multiline set to false (which will stop users from making newlines) when they also do not
					//have any GrowthBehavior specified.
					return false;
				}
				else {//InputField that is too tall to be considered a "single line" text box or not an InputField or has some GrowthBehavior set.
					//An office can set up a sheet def with a static text field that has newlines but is only sized (height) for a single line of text.
					//Always display output to the user in a text box that has the capabilities of preserving anything the user wants to display.
					return true;
				}
			}
			else {
				isSingleLineTextBox=(sheetField.Height<=richTextBox.Font.Height+2);
				if(sheetField.FieldType==SheetFieldType.InputField && isSingleLineTextBox && sheetField.GrowthBehavior==GrowthBehaviorEnum.None) {
					return false;
				}
				else {
					return true;
				}
			}
		}

		#region Methods - Scaling DPI
		///<summary>Converts pixels used by us (100dpi) to points used by PdfSharp.</summary>
		public static float PixelsToPoints(float pixels) {
			double inches=pixels/100d;//100 ppi
			XUnit xunit=XUnit.FromInch(inches);
			return (float)xunit.Point;
		}

		///<summary>Converts points used by PdfSharp to pixels used by us (100dpi).</summary>
		public static float PointsToPixels(float points) {
			double pointsPerInch=XUnit.FromInch(1).Point;
			double inches=points/pointsPerInch;
			return (float)(inches*100d);//100dpi
		}
		#endregion Methods - Scaling DPI
	}

	///<summary>Used to collect information about a single line of text within a RichTextBox.</summary>
	public class RichTextLineInfo {
		///<summary>The index of the first character on the line within the RichTextBox.</summary>
		public int FirstCharIndex=0;
		///<summary>The horizontral location in pixels where the text begins on the line.</summary>
		public int Left=0;
		///<summary>The vertical location in pixels where the text begins on the line.</summary>
		public int Top=0;
		///<summary>The vertical location in pixels where the text ends on the line.</summary>
		public int Bottom=0;

		///<summary>We get text in this manner so that we can wait as long as possible in the process before converting strings. This lazy loading approach increases efficiency.</summary>
		public static string GetTextForLine(string text,List<RichTextLineInfo> listLines,int lineIndex) {
			if(lineIndex==listLines.Count-1) {//Last line
				return text.Substring(listLines[lineIndex].FirstCharIndex);
			}
			return text.Substring(listLines[lineIndex].FirstCharIndex,listLines[lineIndex+1].FirstCharIndex-listLines[lineIndex].FirstCharIndex);
		}

	}

	/// <summary>Container class used to encapsulate the results of Graphics.MeasureString.</summary>
	public class HeightAndChars{
		/// <summary>The height of the text that will fit in the SizeF used by Graphics.MeasureString.</summary>
		public int Height;
		/// <summary>The number of characters that fit in the SizeF used by Graphics.MeasureString.</summary>
		public int Chars;
	}

}
