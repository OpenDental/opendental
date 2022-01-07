using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//using System.Windows;
using PdfSharp.Drawing;
using OpenDentBusiness;
using System.Drawing.Drawing2D;
using CodeBase;
using Google.Apis.Util;

namespace OpenDentBusiness {
	public class GraphicsHelper {
		///<summary>Creates a textbox from the text sheetfield provided.  Used for display and for text wrapping calculations. Calling methods are responsible for explicitly disposing of the returned text box itself and it's Font property.</summary>
		public static RichTextBox CreateTextBoxForSheetDisplay(SheetField sheetField,bool isClipText=true) {
			RichTextBox textbox=new RichTextBox();
			textbox.BorderStyle=BorderStyle.None;
			textbox.TabStop=false;//Only input fields allow tab stop (set below for input text).
			//Input fields need to have a yellow background so that they stand out to the user.
			if(sheetField.FieldType==SheetFieldType.InputField) {
				textbox.BackColor=Color.FromArgb(245,245,200);
				textbox.TabStop=(sheetField.TabOrder>0);
				textbox.TabIndex=sheetField.TabOrder;
			}
			textbox.Location=new Point(sheetField.XPos,sheetField.YPos);
			textbox.Width=sheetField.Width;
			textbox.ScrollBars=RichTextBoxScrollBars.None;
			if(sheetField.ItemColor!=Color.FromArgb(0)){
				textbox.ForeColor=sheetField.ItemColor;
			}
			textbox.SelectionAlignment=sheetField.TextAlign;
			textbox.SelectedText=sheetField.FieldValue;
			FontStyle style=FontStyle.Regular;
			if(sheetField.FontIsBold) {
				style=FontStyle.Bold;
			}
			textbox.Font=new Font(sheetField.FontName,sheetField.FontSize,style);
			bool isSingleLineTextBox=(sheetField.Height < textbox.Font.Height+2);
			//There is a chance that data can get truncated when loading text into a text box that has Multiline set to false.
			//E.g. the text "line 1\r\nline 2\r\nline 3" will get truncated to be "line 1"
			//This causes a nasty bug where the user could have filled out the sheet as a Web Form (which does not truncate the text) and signed the sheet.
			//The signature would invalidate if the office were to open the downloaded web form within Open Dental proper and simply click OK.
			//Therefore, only allow InputField text boxes to have Multiline set to false (which will stop users from making newlines).
			if(sheetField.FieldType==SheetFieldType.InputField && isSingleLineTextBox) {
				textbox.Multiline=false;
			}
			else {//InputField that is too tall to be considered a "single line" text box or not an InputField.
				//An office can set up a sheet def with a static text field that has newlines but is only sized (height) for a single line of text.
				//Always display output to the user in a text box that has the capabilities of preserving anything the user wants to display.
				textbox.Multiline=true;
			}
			textbox.Height=sheetField.Height;
			textbox.ScrollBars=RichTextBoxScrollBars.None;
			textbox.Tag=sheetField;
			#region Text Clipping
			//For multi-line textboxes, clip vertically to remove lines which extend beyond the bottom of the field.
			if(isClipText && textbox.Text.Length > 0 && !isSingleLineTextBox) {
				List <RichTextLineInfo> listLines=GetTextSheetDisplayLines(textbox);
				int fitLineIndex=listLines.Count-1;//Line numbers are zero-based.
				while(fitLineIndex >= 0	&& listLines[fitLineIndex].Top+textbox.Font.Height-1 > textbox.Height) {//if bottom of line is past bottom of textbox
					fitLineIndex--;
				}
				if(fitLineIndex < listLines.Count-1) {//If at least one line was truncated from the bottom.
					textbox.Text=textbox.Text.Substring(0,listLines[fitLineIndex+1].FirstCharIndex);//Truncate text to the end of the fit line.
				}
			}
			//For single-line textboxes, clip the text to the width of the textbox.  This forces the display to look the same on screen as when printed.
			if(isClipText && textbox.Text.Length > 0 && isSingleLineTextBox) {
				int indexRight=textbox.GetCharIndexFromPosition(new Point(textbox.Width,0));
				if(indexRight < textbox.Text.Length-1) {//If the string is too wide for the textbox.
					textbox.Text=textbox.Text.Substring(0,indexRight+1);//Truncate the text to fit the textbox width.
				}
			}
			#endregion Text Clipping
			//Take over the ability to paste formatted text into the RTB.
			textbox.KeyDown+=(o,e) => {
				//Two different ways to paste are "Ctrl + V" and "Shift + Insert".
				if((e.Control && e.KeyCode==Keys.V) || (e.Shift && e.KeyCode==Keys.Insert)) {
					try {
						if(Clipboard.ContainsText()) {
							//Paste just the plain text value from the clipboard.
							textbox.Paste(DataFormats.GetFormat(DataFormats.Text));
						}
					}
					catch(Exception ex) {
						//Several of the methods above can fail for different reasons. E.g. "Requested Clipboard operation did not succeed"
						ex.DoNothing();
					}
					e.Handled=true;//Do not let the base Paste function to be invoked because it allows pasting formatted text, pictures, etc.
				}
			};
			return textbox;
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

		///<summary>Since Graphics doesn't have a line height property. Our rendering algorithm focuses on wrapping the text on the same character on each line for all printing options (screen, printer, pdf), as well as making the text exactly fix vertically within the bounds given in order to properly support sheet vertical growth behavior. Since all printing options have slightly different implementations within their drivers, the same font when used in each option is slightly different.  As a result, the printed text width will vary depending on which printing option is used.  We return a rectangle representing the actual drawn area of the output string for use in special cases such as the TreatmentPlan.Note, which has a border drawn around it always.</summary>
		public static RectangleF DrawString(Graphics g,string str,Font font,Brush brush,Rectangle bounds,HorizontalAlignment align) {
			if(str.Trim()=="") {
				return bounds;//Nothing to draw.
			}
			StringFormat sf=StringFormat.GenericTypographic;
			//The overload for DrawString that takes a StringFormat will cause the tabs '\t' to be ignored.
			//In order for the tabs to not get ignored, we have to tell StringFormat how many pixels each tab should be.  
			//50.0f is the closest to our Fill Sheet Edit preview.
			sf.SetTabStops(0.0f,new float[1] { 50.0f });
			RichTextBox textbox=CreateTextBoxForSheetDisplay(str,font,bounds.Width,bounds.Height,align);
			List<RichTextLineInfo> listTextLines=GetTextSheetDisplayLines(textbox);
			float deviceLineHeight=g.MeasureString(str.Replace("\r","").Replace("\n",""),font,int.MaxValue,sf).Height;
			float scale=deviceLineHeight/textbox.Font.Height;//(size when printed)/(size on screen)
			font=new Font(font.FontFamily,font.Size*scale,font.Style);
			float maxLineWidth=0;
			for(int i=0;i<listTextLines.Count;i++) {
				string line=RichTextLineInfo.GetTextForLine(textbox.Text,listTextLines,i);
				if(line.Trim().Length > 0) {
					float textWidth=g.MeasureString(line,font,int.MaxValue,sf).Width;
					float x;
					if(align==HorizontalAlignment.Left) {
						x=bounds.X + listTextLines[i].Left * scale;
					}
					else if(align==HorizontalAlignment.Center) {
						x=bounds.X + ((bounds.Width - (textWidth * scale)) / 2);
					}
					else {//Right
						x=bounds.X + bounds.Width - (textWidth * scale);
					}
					float y=bounds.Y+listTextLines[i].Top*scale;
					g.DrawString(line,font,brush,x,y,sf);
					maxLineWidth=Math.Max(maxLineWidth,(listTextLines[i].Left*scale)+textWidth);
				}
			}
			textbox.Font.Dispose();//This font was dynamically created when the textbox was created.
			textbox.Dispose();
			sf.Dispose();
			font.Dispose();
			return new RectangleF(bounds.X,bounds.Y,maxLineWidth,bounds.Height);
		}

		///<summary>The PdfSharp version of Graphics.DrawString().  scaleToPix scales xObjects to pixels. Our rendering algorithm focuses on wrapping the text on the same character on each line for all printing options (screen, printer, pdf), as well as making the text exactly fix vertically within the bounds given in order to properly support sheet vertical growth behavior. Since all printing options have slightly different implementations within their drivers, the same font when used in each option is slightly different.  As a result, the printed text width will vary depending on which printing option is used.  We return a rectangle representing the actual drawn area of the output string for use in special cases such as the TreatmentPlan.Note, which has a border drawn around it always.</summary>
		public static RectangleF DrawStringX(XGraphics xg,string str,XFont xfont,XBrush xbrush,RectangleF bounds,HorizontalAlignment align) {
			if(str.Trim()=="") {
				return bounds;//Nothing to draw.
			}
			XStringFormat sf=XStringFormats.Default;
			//There are two coordinate systems here: pixels (used by us) and points (used by PdfSharp).
			//MeasureString and ALL related measurement functions must use pixels.
			//DrawString is the ONLY function that uses points.
			//pixels:
			FontStyle fontstyle=FontStyle.Regular;
			if(xfont.Style==XFontStyle.Bold) {
				fontstyle=FontStyle.Bold;
			}
			//pixels: (except Size is em-size)
			Font font=new Font(xfont.Name,(float)xfont.Size,fontstyle);
			RichTextBox textbox=CreateTextBoxForSheetDisplay(str,font,(int)Math.Ceiling(bounds.Width),(int)Math.Ceiling(bounds.Height),align);
			List<RichTextLineInfo> listTextLines=GetTextSheetDisplayLines(textbox);
			float deviceLineHeight=PointsToPixels((float)xg.MeasureString(str.Replace("\r","").Replace("\n",""),xfont,sf).Height);
			float scale=deviceLineHeight/textbox.Font.Height;//(size when printed)/(size on screen)
			font.Dispose();
			xfont=new XFont(xfont.Name,xfont.Size*scale,xfont.Style);
			double maxLineWidth=0;
			for(int i=0;i<listTextLines.Count;i++) {
				string line=RichTextLineInfo.GetTextForLine(textbox.Text,listTextLines,i);
				if(line.Trim().Length > 0) {
					double textWidth=xg.MeasureString(line,xfont,sf).Width;
					float x;
					if(align==HorizontalAlignment.Left) {
						x=PixelsToPoints(bounds.X + listTextLines[i].Left * scale);
					}
					else if(align==HorizontalAlignment.Center) {
						x=PixelsToPoints(bounds.X) + ((PixelsToPoints(bounds.Width) - (float)textWidth) / 2);
					}
					else {//Right
						x=PixelsToPoints(bounds.X + bounds.Width) - (float)textWidth;
					}
					float y=PixelsToPoints(bounds.Y+listTextLines[i].Bottom*scale);
					//There is currently a problem with printing the tab character '\t' when using XStringFormat.
					//C#'s StringFormat has a method called SetTabStops() which can be used to get the tabs to be drawn (see regular printing above).
					//We're doing nothing for now because the current complaint is only for printing, not PDF creation.  
					//A workaround is to not use tabs and to instead use separate static text fields that are spaced out as desired.
					xg.DrawString(line,xfont,xbrush,x,y,sf);
					maxLineWidth=Math.Max(maxLineWidth,PixelsToPoints(listTextLines[i].Left*scale)+textWidth);
				}
			}
			textbox.Font.Dispose();//This font was dynamically created when the textbox was created.
			textbox.Dispose();
			//sf.Dispose();//Does not exist for PDF.
			//xfont.Dispose();//Does not exist for PDF fonts.
			return new RectangleF(bounds.X,bounds.Y,PointsToPixels((float)maxLineWidth),bounds.Height);
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
		public static int MeasureStringH(string text,Font font,int width,HorizontalAlignment align) {
			if(!text.EndsWith("\n")) {
				//Add an extra line of text so that we can calculate the top of that last fake line to figure out where the bottom of the last real line is.
				text+="\n";
			}
			text+="*";
			//Assume height of font.Height+2 to force multi-line.
			RichTextBox textbox=CreateTextBoxForSheetDisplay(text,font,width,font.Height+2,align,false);
			int h=textbox.GetPositionFromCharIndex(textbox.Text.Length-1).Y;//The top of the fake line is the bottom of the last real line.
			textbox.Font.Dispose();//This font was dynamically created when the textbox was created.
			textbox.Dispose();
			return h;
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

}
