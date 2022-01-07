//using Microsoft.Vsa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental.ReportingComplex {
	///<summary></summary>
	public partial class FormReportComplex : FormODBase {
		private ODprintout _printout;
		///<summary>The report to display.</summary>
		private ReportComplex _myReport;
		///<summary>The name of the last section printed. It really only keeps track of whether the details section and the reportfooter have finished printing. This variable will be refined when groups are implemented.</summary>
		private AreaSectionType _lastSectionPrinted;
		private int _rowsPrinted;
		private int _totalRowsPrinted;
		private int _totalPages;
		///<summary>Records the number of pages that are printed, including the pages that are "fake" printed as described in the summary comment of the pagePrinter() method.</summary>
		private int _pagesProcessed;
		///<summary>Records the number of pages that are actually printed</summary>
		private int _totalNumberPagesPrinted;
		private int _heightRemaining=0;
		private bool _isWrappingText;
		///<summary>An arbitrary buffer amount for AreaSectionType.GroupFooter added to give a buffer between split tables.</summary>
		private const int GROUP_FOOTER_BUFFER=20;

		///<summary>The location of the current page being printed, used to increment through the pages.</summary>
		private int _currentPage {
			get {
				return _pagesProcessed+1;
			}
		}

		///<summary></summary>
		public FormReportComplex(ReportComplex myReport){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_myReport=myReport;
		}

		private void FormReport_Load(object sender, System.EventArgs e) {
      _isWrappingText=PrefC.GetBool(PrefName.ReportsWrapColumns);
      RefreshWindow();
		}

    ///<summary>Used to refresh the print window when something changes.</summary>
    public void RefreshWindow() {
      LayoutToolBar();
			if(ResetODprintout()) {
				SetDefaultZoom();
				printPreviewControl2.Document=_printout.PrintDoc;
			}
			LayoutManager.Move(printPreviewControl2,new Rectangle(0,ToolBarMain.Bottom,ClientSize.Width,ClientSize.Height-ToolBarMain.Bottom-LayoutManager.Scale(28)));
			//Otherwise, it will not lay out properly when primary monitor is not at 100%
    }

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",1,Lan.g(this,"Go Back One Page"),"Back"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.PageNav) {Text="/",Tag="PageNum" });
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,Lan.g(this,"Go Forward One Page"),"Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",4,"","ZoomIn"));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",5,"","ZoomOut"));
			ToolBarMain.Buttons.Add(new ODToolBarButton("100",-1,"","ZoomReset"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
      ODToolBarButton butWrapText=new ODToolBarButton(Lan.g(this,"Wrap Text"),-1,Lan.g(this,"Wrap Text In Columns"),"WrapText");
      butWrapText.Style=ODToolBarButtonStyle.ToggleButton;
      butWrapText.Pushed=_isWrappingText;
      ToolBarMain.Buttons.Add(butWrapText);
      ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Export"),3,"","Export"));
      ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,Lan.g(this,"Close This Window"),"Close"));
			//ToolBarMain.Invalidate();
		}

		///<summary>Sets the default zoom factor based on the reports orientation.</summary>
		private void SetDefaultZoom() {
			if(_myReport.IsLandscape) {
				printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Height
					/(double)_printout.PrintDoc.DefaultPageSettings.PaperSize.Width);
			}
			else {
				printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Height
					/(double)_printout.PrintDoc.DefaultPageSettings.PaperSize.Height);
			}
		}

		private void FormReportComplex_SizeChanged(object sender, EventArgs e){
			LayoutManager.Move(printPreviewControl2,new Rectangle(
				0,ToolBarMain.Bottom,ClientSize.Width,ClientSize.Height-ToolBarMain.Bottom-LayoutManager.Scale(28)));
		}
		
		private bool ResetODprintout(){
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Retrieving Printer Settings")+"...");
			_printout=PrinterL.CreateODprintout(
				pd2_PrintPage,
				auditDescription:Lan.g(this,"Report printed ")+_myReport.ReportName,
				printoutOrientation:(_myReport.IsLandscape?PrintoutOrientation.Landscape:PrintoutOrientation.Default),
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin,
				isErrorSuppressed:true//The error is handled by firing error event below.
			);
			if(_printout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				MsgBox.Show(PrinterL.GetErrorStringFromCode(_printout.SettingsErrorCode));
				_myReport.CloseProgressBar();
				this.Close();//Close form because print preview would only show a gray box otherwise.
				return false;
			}
			_printout.PrintDoc.EndPrint += new PrintEventHandler(this.pd2_EndPrint);
			_lastSectionPrinted=AreaSectionType.None;
			_rowsPrinted=0;
			_totalRowsPrinted=0;
			_pagesProcessed=0;
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Calculating Row Heights")+"...");
			foreach(ReportObject reportObject in _myReport.ReportObjects) {
				if(reportObject.ObjectType==ReportObjectType.QueryObject) {
					QueryObject queryObject=(QueryObject)reportObject;
          queryObject.CalculateRowHeights(_isWrappingText);
					if(queryObject.IsPrinted==true) {
						queryObject.IsPrinted=false;
					}
				}
			}
			return true;
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			//MessageBox.Show(e.Button.Tag.ToString());
			switch(e.Button.Tag.ToString()){
				case "Print":
					OnPrint_Click();
					break;
				case "Back":
					OnBack_Click();
					break;
				case "Fwd":
					OnFwd_Click();
					break;
				case "ZoomIn":
					OnZoomIn_Click();
					break;
				case "ZoomOut":
					OnZoomOut_Click();
					break;
				case "ZoomReset":
					OnZoomReset_Click();
					break;
				case "Export":
					OnExport_Click();
					break;
        case "WrapText":
					OnWrapText_Click();
					break;
				}
		}

		private void ToolBarMain_PageNav(object sender,ODToolBarButtonPageNavEventArgs e) {
			if(e.NavValue==0) {
				return;
			}
			printPreviewControl2.StartPage=e.NavValue-1;
			SetPageNavString();
		}

		///<summary>raised for each page to be printed.</summary>
		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){
			do {
				pagePrinter(ev);
			} while(ev.PageSettings.PrinterSettings.PrintRange!=PrintRange.AllPages && ev.PageSettings.PrinterSettings.FromPage>=_currentPage);
			//if the reportfooter or the last configured print page has printed, then there are no more pages to print.
			if(_lastSectionPrinted==AreaSectionType.ReportFooter
				|| (ev.PageSettings.PrinterSettings.PrintRange!=PrintRange.AllPages 
				&& ev.PageSettings.PrinterSettings.ToPage<_currentPage)) 
			{ 
				ev.HasMorePages=false;
			}
			else {//If we reach the end of the document, OR the end of the specificed range, stop printed. Otherwise, continue.
				ev.HasMorePages=true;
				ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+" - "
					+Lan.g("ReportComplex","Page Printed. Preparing Next Page")+"...");
			}
			_totalNumberPagesPrinted+=1;
		}

		///<summary>This method contains the logic to print a given page using the grfx Graphics object. It tracks the yPos to extend the data over pages
		///as necessary using a standard paperSize and predefined currentMargins. It will only print if isPrintablePage is true, otherwise it will "fake"
		///print the pages for which it is false to allow users to print over a range.</summary>
		private void pagePrinter(PrintPageEventArgs ev) {
			//Is a printable page if we're printing (or viewing) all pages, or only printing the pages between the FromPage and ToPage designated by the user.
			bool isPrintablePage=ev.PageSettings.PrinterSettings.PrintRange==PrintRange.AllPages
				|| (ev.PageSettings.PrinterSettings.FromPage<=_currentPage && ev.PageSettings.PrinterSettings.ToPage>=_currentPage);
			ReportComplexEvent.Fire(ODEventType.ReportComplex,new ProgressBarHelper(Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+"..."
				,"",_totalRowsPrinted,_myReport.TotalRows,ProgBarStyle.Blocks));
			//Note that the locations of the reportObjects are not absolute.  They depend entirely upon the margins.  When the report is initially created, it is pushed up against the upper and the left.
			Graphics grfx=ev.Graphics;
			//xPos and yPos represent the upper left of current section after margins are accounted for.
			//All reportObjects are then placed relative to this origin.
			Size paperSize;
			if(_myReport.IsLandscape) {
				paperSize=new Size(1100,850);
			}
			else {
				paperSize=new Size(850,1100);
			}
			int xPos=_myReport.PrintMargins.Left;
			int yPos=_myReport.PrintMargins.Top;
			int printableHeight=paperSize.Height-_myReport.PrintMargins.Top-_myReport.PrintMargins.Bottom;
			int yLimit=paperSize.Height-_myReport.PrintMargins.Bottom;//the largest yPos allowed
			//Now calculate and layout each section in sequence.
			Section section;
			//Technically the ReportFooter should only be subtracted from the printableHeight of the last page, but we have no way to know how many pages
			//the report will end up taking so we will subtract it from the printable height of all pages.
			//Used to determine the max height of a single grid cell.
			int maxGridCellHeight=printableHeight-_myReport.GetSectionHeight(AreaSectionType.PageHeader)
				-_myReport.GetSectionHeight(AreaSectionType.GroupFooter)-_myReport.GetSectionHeight(AreaSectionType.GroupTitle)
				-_myReport.GetSectionHeight(AreaSectionType.GroupHeader)-_myReport.GetSectionHeight(AreaSectionType.ReportFooter);
			if(_pagesProcessed==0) {
				maxGridCellHeight-=_myReport.GetSectionHeight(AreaSectionType.ReportHeader);
			}
			foreach(ReportObject reportObject in _myReport.ReportObjects) {
				if(reportObject.ObjectType!=ReportObjectType.QueryObject) {
					continue;
				}
				QueryObject queryObject=(QueryObject)reportObject;
				for(int i=0;i<queryObject.RowHeightValues.Count;i++) {
					queryObject.RowHeightValues[i]=Math.Min(queryObject.RowHeightValues[i],maxGridCellHeight);
				}
				foreach(ReportObject rObject in queryObject.ReportObjects) {
					if(rObject.SectionType!=AreaSectionType.Detail && rObject.SectionType!=AreaSectionType.GroupFooter) {
						rObject.ContentAlignment=ContentAlignment.TopCenter;
						continue;
					}
					if(rObject.ObjectType==ReportObjectType.FieldObject && rObject.FieldValueType==FieldValueType.Number) {
						rObject.ContentAlignment=ContentAlignment.TopRight;
					}
				}
			}
			while(true){//will break out if no more room on page
				//if no sections have been printed yet, print a report header.
				if(_lastSectionPrinted==AreaSectionType.None) {
					if(_myReport.Sections.Contains(AreaSectionType.ReportHeader)) {
						ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+" - "
							+Lan.g("ReportComplex","Printing Report Header")+"...");
						section=_myReport.Sections[AreaSectionType.ReportHeader];
						PrintSection(grfx,section,xPos,yPos,isPrintablePage);
						yPos+=section.Height;
						if(section.Height>printableHeight){//this can happen if the reportHeader takes up the full page
							//if there are no other sections to print
							//this will keep the second page from printing:
							_lastSectionPrinted=AreaSectionType.ReportFooter;
							break;
						}
					}
					else{//no report header
						//it will still be marked as printed on the next line
					}
					_lastSectionPrinted=AreaSectionType.ReportHeader;
				}
				//always print a page header if it exists
				if(_myReport.Sections.Contains(AreaSectionType.PageHeader)) {
					ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+" - "
						+Lan.g("ReportComplex","Printing Page Header")+"...");
					section=_myReport.Sections[AreaSectionType.PageHeader];
					PrintSection(grfx,section,xPos,yPos,isPrintablePage);
					yPos+=section.Height;
				}
				_heightRemaining=yLimit-yPos-_myReport.GetSectionHeight(AreaSectionType.PageFooter);
				section=_myReport.Sections[AreaSectionType.Query];
				PrintQuerySection(grfx,section,xPos,yPos,isPrintablePage);
				yPos+=section.Height;
				bool isRoomForReportFooter=true;
				if(_heightRemaining-_myReport.GetSectionHeight(AreaSectionType.ReportFooter)<=0) {
					isRoomForReportFooter=false;
				}
				//print the reportfooter section if there is room
				if(isRoomForReportFooter){
					if(_myReport.Sections.Contains(AreaSectionType.ReportFooter)) {
						ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+" - "
							+Lan.g("ReportComplex","Printing Report Footer")+"...");
						section=_myReport.Sections[AreaSectionType.ReportFooter];
						PrintSection(grfx,section,xPos,yPos,isPrintablePage);
						yPos+=section.Height;
					}
					//mark the reportfooter as printed. This will prevent another loop.
					_lastSectionPrinted=AreaSectionType.ReportFooter;
				}
				//print the pagefooter
				if(_myReport.Sections.Contains(AreaSectionType.PageFooter)) {
					ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+" - "
						+Lan.g("ReportComplex","Printing Page Footer")+"...");
					section=_myReport.Sections[AreaSectionType.PageFooter];
					yPos=yLimit-section.Height;
					PrintSection(grfx,section,xPos,yPos,isPrintablePage);
					yPos+=section.Height;
				}
				break;
			}//while		
			_pagesProcessed++;
		}

		///<summary>Either the report finished printing OR the user canceled out of the print job.</summary>
		private void pd2_EndPrint(object sender,PrintEventArgs ev) {
			_totalPages=_pagesProcessed;
			SetPageNavString();
			_myReport.CloseProgressBar();
		}

		///<summary>Prints one section other than details at the specified x and y position on the page.  The math to decide whether it will fit on the 
		///current page is done ahead of time.  Will print a blank section if isPrintablePage is false.</summary>
		private void PrintSection(Graphics g,Section section,int xPos,int yPos,bool isPrintablePage){
			ReportObject textObject;
			ReportObject fieldObject;
			ReportObject lineObject;
			ReportObject boxObject;
			foreach(ReportObject reportObject in _myReport.ReportObjects){
				if(reportObject.SectionType!=section.SectionType){
					continue;
				}
				using(StringFormat strFormat=ReportObject.GetStringFormatAlignment(reportObject.ContentAlignment)) {
					#region TextObject
					if(reportObject.ObjectType==ReportObjectType.TextObject){
						textObject=reportObject;
						Font newFont=new Font(textObject.Font,textObject.Font.Style|(textObject.IsUnderlined?FontStyle.Underline:FontStyle.Regular));
						RectangleF layoutRect=new RectangleF(xPos+textObject.Location.X
							,yPos+textObject.Location.Y
							,textObject.Size.Width,textObject.Size.Height);
						#region ReportFooter
						if(section.SectionType==AreaSectionType.ReportFooter) {
							if(textObject.Name=="ReportSummaryText") {
								xPos+=_myReport.ReportObjects["ReportSummaryLabel"].Size.Width;
								newFont=new Font(textObject.Font,textObject.Font.Style|FontStyle.Bold);
								SizeF size=g.MeasureString(textObject.StaticText,newFont);
								textObject.Size=new Size((int)size.Width+1,(int)size.Height+1);
							}
							layoutRect=new RectangleF(xPos+textObject.Location.X+textObject.OffSetX
								,yPos+textObject.Location.Y+textObject.OffSetY
								,textObject.Size.Width,textObject.Size.Height);
						}
						#endregion ReportFooter
						if(isPrintablePage) {
							g.DrawString(textObject.StaticText,newFont,Brushes.Black,layoutRect,strFormat);
						}
						newFont.Dispose();
					}
					#endregion TextObject
					#region FieldObject
					else if(reportObject.ObjectType==ReportObjectType.FieldObject) {
						if(reportObject.FieldDefKind!=FieldDefKind.SpecialField
							|| reportObject.SpecialFieldType!=SpecialFieldType.PageNumber
							|| !isPrintablePage)
						{
							continue;
						}
						fieldObject=reportObject;
						RectangleF layoutRect=new RectangleF(xPos+fieldObject.Location.X
							,yPos+fieldObject.Location.Y
							,fieldObject.Size.Width,fieldObject.Size.Height);
						g.DrawString(Lan.g(this,"Page")+" "+_currentPage,fieldObject.Font,Brushes.Black,layoutRect,strFormat);
					}
					#endregion FieldObject
					#region BoxObject
					else if(reportObject.ObjectType==ReportObjectType.BoxObject) {
						if(!isPrintablePage) {
							continue;
						}
						boxObject=reportObject;
						int x1=xPos+boxObject.OffSetX;
						int x2=xPos-boxObject.OffSetX;
						int y1=yPos+boxObject.OffSetY;
						int y2=yPos-boxObject.OffSetY;
						int maxHorizontalLength=1100;
						if(!_myReport.IsLandscape) {
							maxHorizontalLength=850;
						}
						x2+=maxHorizontalLength;
						y2+=_myReport.GetSectionHeight(boxObject.SectionType);
						g.DrawRectangle(new Pen(boxObject.ForeColor,boxObject.FloatLineThickness),x1,y1,x2-x1,y2-y1);
					}
					#endregion BoxObject
					#region LineObject
					else if(reportObject.ObjectType==ReportObjectType.LineObject) {
						if(!isPrintablePage) {
							continue;
						}
						lineObject=reportObject;
						int length;
						int x=lineObject.OffSetX;
						int y=yPos+lineObject.OffSetY;
						int maxHorizontalLength=1100;
						if(!_myReport.IsLandscape) {
							maxHorizontalLength=850;
						}
						#region Horizontal Line
						if(lineObject.LineOrientation==LineOrientation.Horizontal) {
							length=maxHorizontalLength*lineObject.IntLinePercent/100;
							if(lineObject.LinePosition==LinePosition.South) {
								y+=_myReport.GetSectionHeight(lineObject.SectionType);
							}
							else if(lineObject.LinePosition==LinePosition.North) {
								//Do Nothing Here
							}
							else if(lineObject.LinePosition==LinePosition.Center) {
								y+=(_myReport.GetSectionHeight(lineObject.SectionType)/2);
							}
							else {
								continue;
							}
							x+=(maxHorizontalLength/2)-(length/2);
							g.DrawLine(new Pen(reportObject.ForeColor,reportObject.FloatLineThickness),x,y,x+length,y);
						}
						#endregion Horizontal Line
						#region Vertical Line
						else if(lineObject.LineOrientation==LineOrientation.Vertical) {
							length=_myReport.GetSectionHeight(lineObject.SectionType)*lineObject.IntLinePercent/100;
							if(lineObject.LinePosition==LinePosition.West) {
								//Do Nothing Here
							}
							else if(lineObject.LinePosition==LinePosition.East) {
								x+=maxHorizontalLength;
							}
							else if(lineObject.LinePosition==LinePosition.Center) {
								x+=maxHorizontalLength/2;
							}
							else {
								continue;
							}
							y+=(_myReport.GetSectionHeight(lineObject.SectionType)/2)-(length/2);
							g.DrawLine(new Pen(reportObject.ForeColor,reportObject.FloatLineThickness),x,y,x,y+length);
						}
						#endregion Vertical Line
					}
					#endregion LineObject
				}
			}
		}

		///<summary>Prints some rows of the details section at the specified x and y position on the page.  The math to decide how many rows to print is 
		///done ahead of time.  The number of rows printed so far is kept global so that it can be used in calculating the layout of this section.
		///Prints a blank section if isPrintablePage=false.</summary>
		private void PrintQuerySection(Graphics g,Section section,int xPos,int yPos,bool isPrintablePage) {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Printing Page")+" "+(_currentPage)+" - "
				+Lan.g("ReportComplex","Printing Query Section")+"...");
			section.Height=0;
			ReportObject textObject;
			ReportObject lineObject;
			ReportObject boxObject;
			QueryObject queryObject;
			StringFormat strFormat;//used each time text is drawn to handle alignment issues
			#region Lines And Boxes
			foreach(ReportObject reportObject in _myReport.ReportObjects) {
				if(reportObject.SectionType!=section.SectionType) {
					//skip any reportObjects that are not in this section
					continue;
				}
				if(reportObject.ObjectType==ReportObjectType.BoxObject) {
					boxObject=reportObject;
					int x1=xPos+boxObject.OffSetX;
					int x2=xPos-boxObject.OffSetX;
					int y1=yPos+boxObject.OffSetY;
					int y2=yPos-boxObject.OffSetY;
					int maxHorizontalLength=1100;
					if(!_myReport.IsLandscape) {
						maxHorizontalLength=850;
					}
					x2+=maxHorizontalLength-xPos;
					y2+=_heightRemaining*_myReport.GetSectionHeight(boxObject.SectionType);
					if(isPrintablePage) {
						g.DrawRectangle(new Pen(boxObject.ForeColor,boxObject.FloatLineThickness),x1,y1,x2-x1,y2-y1);
					}
				}
				else if(reportObject.ObjectType==ReportObjectType.LineObject) {
					lineObject=reportObject;
					int length;
					int x=lineObject.OffSetX;
					int y=yPos+lineObject.OffSetY;
					int maxHorizontalLength=1100;
					if(!_myReport.IsLandscape) {
						maxHorizontalLength=850;
					}
					if(lineObject.LineOrientation==LineOrientation.Horizontal) {
						length=maxHorizontalLength*lineObject.IntLinePercent/100;
						if(lineObject.LinePosition==LinePosition.South) {
							y+=_myReport.GetSectionHeight(lineObject.SectionType);
						}
						else if(lineObject.LinePosition==LinePosition.North) {
							//Do Nothing Here
						}
						else if(lineObject.LinePosition==LinePosition.Center) {
							y+=(_myReport.GetSectionHeight(lineObject.SectionType)/2);
						}
						else {
							continue;
						}
						x+=(maxHorizontalLength/2)-(length/2);
						if(isPrintablePage) {
							g.DrawLine(new Pen(reportObject.ForeColor,reportObject.FloatLineThickness),x,y,x+length,y);
						}
					}
					else if(lineObject.LineOrientation==LineOrientation.Vertical) {
						length=_myReport.GetSectionHeight(lineObject.SectionType)*lineObject.IntLinePercent/100;
						if(lineObject.LinePosition==LinePosition.West) {
							//Do Nothing Here
						}
						else if(lineObject.LinePosition==LinePosition.East) {
							x=maxHorizontalLength;
						}
						else if(lineObject.LinePosition==LinePosition.Center) {
							x=maxHorizontalLength/2;
						}
						else {
							continue;
						}
						y=y+(_myReport.GetSectionHeight(lineObject.SectionType)/2)-(length/2);
						if(isPrintablePage) {
							g.DrawLine(new Pen(reportObject.ForeColor,reportObject.FloatLineThickness),x,y,x,y+length);
						}
					}
					else {
						//Do nothing since it has already been done for each row.
					}
				}
			}
			#endregion
			foreach(ReportObject reportObject in _myReport.ReportObjects) {
				if(reportObject.SectionType!=section.SectionType) {
					//skip any reportObjects that are not in this section
					continue;
				}
				if(reportObject.ObjectType==ReportObjectType.TextObject) {
					//not typical to print textobject in details section, but allowed
					textObject=reportObject;
					strFormat=ReportObject.GetStringFormatAlignment(textObject.ContentAlignment);
					RectangleF layoutRect=new RectangleF(xPos+textObject.Location.X
						,yPos+textObject.Location.Y
						,textObject.Size.Width,textObject.Size.Height);
					if(isPrintablePage) {
						g.DrawString(textObject.StaticText,textObject.Font
							,new SolidBrush(textObject.ForeColor),layoutRect,strFormat);
						if(textObject.IsUnderlined) {
							g.DrawLine(new Pen(textObject.ForeColor),xPos+textObject.Location.X,yPos+textObject.Location.Y+textObject.Size.Height,xPos+textObject.Location.X+textObject.Size.Width,yPos+textObject.Location.Y+textObject.Size.Height);
						}
					}
				}
				else if(reportObject.ObjectType==ReportObjectType.QueryObject) {
					queryObject=(QueryObject)reportObject;
					if(queryObject.IsPrinted==true) {
						continue;
					}
					if(queryObject.IsCentered) {
						if(_myReport.IsLandscape) {
							xPos=1100/2-(queryObject.QueryWidth/2);
						}
						else {
							xPos=850/2-(queryObject.QueryWidth/2);
						}
					}
					if(_heightRemaining>0) {
						PrintQueryObjectSection(queryObject,g,queryObject.Sections[AreaSectionType.GroupTitle],xPos,yPos,isPrintablePage);
						yPos+=queryObject.Sections[AreaSectionType.GroupTitle].Height;
						section.Height+=queryObject.Sections[AreaSectionType.GroupTitle].Height;
					}
					if(_heightRemaining>0) {
						PrintQueryObjectSection(queryObject,g,queryObject.Sections[AreaSectionType.GroupHeader],xPos,yPos,isPrintablePage);
						yPos+=queryObject.Sections[AreaSectionType.GroupHeader].Height;
						section.Height+=queryObject.Sections[AreaSectionType.GroupHeader].Height;
					}
					if(_heightRemaining>0) {
						PrintQueryObjectSection(queryObject,g,queryObject.Sections[AreaSectionType.Detail],xPos,yPos,isPrintablePage);
						yPos+=queryObject.Sections[AreaSectionType.Detail].Height;
						section.Height+=queryObject.Sections[AreaSectionType.Detail].Height;
					}
					if(_heightRemaining>0) {
						PrintQueryObjectSection(queryObject,g,queryObject.Sections[AreaSectionType.GroupFooter],xPos,yPos,isPrintablePage);
						yPos+=queryObject.Sections[AreaSectionType.GroupFooter].Height;
						section.Height+=queryObject.Sections[AreaSectionType.GroupFooter].Height;
					}
					if(_heightRemaining<=0) {
						return;
					}
				}
			}
		}

		///<summary>Prints sections inside a QueryObject</summary>
		private void PrintQueryObjectSection(QueryObject queryObj,Graphics g,Section section,int xPos,int yPos,bool isPrintablePage) {
			section.Height=0;
			ReportObject textObject;
			ReportObject fieldObject;
			ReportObject lineObject;
			ReportObject boxObject;
			string displayText="";//The formatted text to print
			string prevDisplayText="";//The formatted text of the previous row. Used to test suppress dupl.	
			StringFormat strFormat;//used each time text is drawn to handle alignment issues
			int yPosAdd=0;
			if(queryObj.SuppressIfDuplicate
				&& section.SectionType==AreaSectionType.GroupTitle && _rowsPrinted>0) 
			{
				return;//Only print the group title for each query object once.
			}
			//loop through each row in the table and make sure that the row can fit.  If it can fit, print it.  Otherwise go to next page.
			for(int i=_rowsPrinted;i<queryObj.ReportTable.Rows.Count;i++) {
				//Figure out the current row height
				if(section.SectionType==AreaSectionType.Detail && queryObj.RowHeightValues[i]>_heightRemaining) {
					_heightRemaining=0;
					return;
				}
				//Find the Group Header height to see if printing at least one row is possible.
				if(section.SectionType==AreaSectionType.GroupTitle) {
					int titleHeight=0;
					int headerHeight=0;
					foreach(ReportObject reportObject in queryObj.ReportObjects) {
						if(reportObject.SectionType==AreaSectionType.GroupTitle) {
							titleHeight+=reportObject.Size.Height;
						}
						else if(reportObject.SectionType==AreaSectionType.GroupHeader && reportObject.Size.Height>headerHeight) {
							headerHeight=reportObject.Size.Height;
						}
					}
					//This is a new table and we want to know if we can print the first row
					if(titleHeight+headerHeight+queryObj.RowHeightValues[0]>_heightRemaining) {
						_heightRemaining=0;
						return;
					}
				}
				//Find the Group Footer height to see if printing the last row should happen on another page.
				if(section.SectionType==AreaSectionType.Detail && _rowsPrinted==queryObj.ReportTable.Rows.Count-1) {
					int groupSummaryLabelHeight=0;
					int tallestTotalSummaryHeight=0;
					foreach(ReportObject reportObject in queryObj.ReportObjects) {
						if(reportObject.SectionType==AreaSectionType.GroupFooter
							&& !reportObject.Name.Contains("GroupSummaryLabel")
							&& !reportObject.Name.Contains("GroupSummaryText")
							&& tallestTotalSummaryHeight<reportObject.Size.Height+reportObject.OffSetY)
						{
							tallestTotalSummaryHeight=reportObject.Size.Height+reportObject.OffSetY;
						}
						//Find the height of the group footer using GroupSummaryLabel because GroupSummaryText has not been filled yet.
						if(reportObject.SectionType==AreaSectionType.GroupFooter && reportObject.Name.Contains("GroupSummaryLabel")) {
							groupSummaryLabelHeight+=reportObject.Size.Height+reportObject.OffSetY;
							//If it is North or South then we need to add its height a second time because the GroupSummaryLabel is located above or below the text.
							if(reportObject.SummaryOrientation==SummaryOrientation.North || reportObject.SummaryOrientation==SummaryOrientation.South) {
								groupSummaryLabelHeight+=reportObject.Size.Height;
							}
						}
					}
					int groupFooterHeight=groupSummaryLabelHeight+tallestTotalSummaryHeight+GROUP_FOOTER_BUFFER;
					//For reports without group footers, check to see if we can print the last row. 
					if(groupFooterHeight==GROUP_FOOTER_BUFFER && queryObj.RowHeightValues[queryObj.ReportTable.Rows.Count-1]>_heightRemaining) {
						_heightRemaining=0;
						return;
					}
					//See if we can print the Group Footer and the Last row
					else if(groupFooterHeight+queryObj.RowHeightValues[queryObj.ReportTable.Rows.Count-1]>_heightRemaining) {
						_heightRemaining=0;
						return;
					}
				}
				int greatestObjectHeight=0;
				int groupTitleHeight=0;
				//Now figure out if anything in the header, footer, or title sections can still fit on the page
				foreach(ReportObject reportObject in queryObj.ReportObjects) {
					if(reportObject.SectionType!=section.SectionType) {
						continue;
					}
					if(reportObject.ObjectType!=ReportObjectType.FieldObject && reportObject.Size.Height>_heightRemaining) {
						_heightRemaining=0;
						return;
					}
					if(reportObject.SectionType==AreaSectionType.GroupFooter && reportObject.Name.Contains("GroupSummary")) {
						if(!queryObj.IsLastSplit) {
							continue;
						}
						if(reportObject.Name.Contains("GroupSummaryText")) {
							if(reportObject.SummaryOperation==SummaryOperation.Sum) {
								if(reportObject.StringFormat=="") {
									reportObject.StaticText=GetGroupSummaryValue(reportObject.DataField,reportObject.SummaryGroups,reportObject.SummaryOperation).ToString("c");
								}
								else{ 
									reportObject.StaticText=GetGroupSummaryValue(reportObject.DataField,reportObject.SummaryGroups,reportObject.SummaryOperation)
									.ToString(reportObject.StringFormat);
								}
							}
							else if(reportObject.SummaryOperation==SummaryOperation.Count) {
								reportObject.StaticText=GetGroupSummaryValue(reportObject.DataField,reportObject.SummaryGroups,reportObject.SummaryOperation).ToString();
							}
							int width=(int)g.MeasureString(reportObject.StaticText,reportObject.Font).Width+2;
							int height=(int)g.MeasureString(reportObject.StaticText,reportObject.Font).Height+2;
							if(width<queryObj.GetObjectByName(reportObject.SummarizedField+"Header").Size.Width) {
								width=queryObj.GetObjectByName(reportObject.SummarizedField+"Header").Size.Width;
							}
							reportObject.Size=new Size(width,height);
						}
					}
					if(section.SectionType==AreaSectionType.GroupTitle && _rowsPrinted>0 && reportObject.Name=="Initial Group Title") {
						continue;
					}
					if(section.SectionType==AreaSectionType.GroupFooter && reportObject.SummaryOrientation==SummaryOrientation.South) {
						ReportObject summaryField=queryObj.GetObjectByName(reportObject.DataField+"Footer");
						yPos+=summaryField.Size.Height;
					}
					if(reportObject.ObjectType==ReportObjectType.TextObject) {
						textObject=reportObject;
						strFormat=ReportObject.GetStringFormatAlignment(textObject.ContentAlignment);
						RectangleF layoutRect=new RectangleF(xPos+textObject.Location.X+textObject.OffSetX
							,yPos+textObject.Location.Y+textObject.OffSetY
							,textObject.Size.Width,textObject.Size.Height);
						if(isPrintablePage) {
							if(textObject.IsUnderlined) {
								g.DrawString(textObject.StaticText,new Font(textObject.Font.FontFamily,textObject.Font.Size,textObject.Font.Style|FontStyle.Underline),Brushes.Black,layoutRect,strFormat);
							}
							else {
								g.DrawString(textObject.StaticText,textObject.Font,Brushes.Black,layoutRect,strFormat);
							}
						}
						if(section.SectionType==AreaSectionType.GroupHeader) {
							greatestObjectHeight=Math.Max(greatestObjectHeight,textObject.Size.Height);
						}
						if(section.SectionType==AreaSectionType.GroupTitle) {
							groupTitleHeight+=textObject.Size.Height;
							yPos+=textObject.Size.Height;
						}
						if(section.SectionType==AreaSectionType.GroupFooter 
							&& ((reportObject.SummaryOrientation==SummaryOrientation.North || reportObject.SummaryOrientation==SummaryOrientation.South)
								|| (reportObject.Name.Contains("GroupSummaryText")))) 
						{
							yPosAdd+=textObject.Size.Height;
							yPos+=textObject.Size.Height+textObject.OffSetY;
						}
					}
					else if(reportObject.ObjectType==ReportObjectType.BoxObject) {
						boxObject=reportObject;
						int x1=xPos+boxObject.OffSetX;
						int x2=xPos-boxObject.OffSetX;
						int y1=yPos+boxObject.OffSetY;
						int y2=yPos-boxObject.OffSetY;
						int maxHorizontalLength=1100;
						if(!_myReport.IsLandscape) {
							maxHorizontalLength=850;
						}
						x2+=maxHorizontalLength;
						y2+=queryObj.GetSectionHeight(boxObject.SectionType);
						if(isPrintablePage) {
							g.DrawRectangle(new Pen(boxObject.ForeColor,boxObject.FloatLineThickness),x1,y1,x2-x1,y2-y1);
						}
						if(section.SectionType==AreaSectionType.GroupHeader) {
							greatestObjectHeight=Math.Max(greatestObjectHeight,boxObject.Size.Height);
						}
						if(section.SectionType==AreaSectionType.GroupTitle) {
							groupTitleHeight+=boxObject.Size.Height;
						}
					}
					else if(reportObject.ObjectType==ReportObjectType.LineObject) {
						lineObject=reportObject;
						int length;
						int x=lineObject.OffSetX;
						int y=yPos+lineObject.OffSetY;
						int maxHorizontalLength=1100;
						if(!_myReport.IsLandscape) {
							maxHorizontalLength=850;
						}
						if(lineObject.LineOrientation==LineOrientation.Horizontal) {
							length=maxHorizontalLength*lineObject.IntLinePercent/100;
							if(lineObject.LinePosition==LinePosition.South) {
								y+=queryObj.GetSectionHeight(lineObject.SectionType);
							}
							else if(lineObject.LinePosition==LinePosition.North) {
								//Do Nothing Here
							}
							else if(lineObject.LinePosition==LinePosition.Center) {
								y+=(queryObj.GetSectionHeight(lineObject.SectionType)/2);
							}
							else {
								continue;
							}
							x+=(maxHorizontalLength/2)-(length/2);
							if(isPrintablePage) {
								g.DrawLine(new Pen(reportObject.ForeColor,reportObject.FloatLineThickness),x,y,x+length,y);
							}
						}
						else if(lineObject.LineOrientation==LineOrientation.Vertical) {
							length=queryObj.GetSectionHeight(lineObject.SectionType)*lineObject.IntLinePercent/100;
							if(lineObject.LinePosition==LinePosition.West) {
								//Do Nothing Here
							}
							else if(lineObject.LinePosition==LinePosition.East) {
								x+=maxHorizontalLength;
							}
							else if(lineObject.LinePosition==LinePosition.Center) {
								x+=maxHorizontalLength/2;
							}
							else {
								continue;
							}
							y+=(queryObj.GetSectionHeight(lineObject.SectionType)/2)-(length/2);
							if(isPrintablePage) {
								g.DrawLine(new Pen(reportObject.ForeColor,reportObject.FloatLineThickness),x,y,x,y+length);
							}
						}
						if(section.SectionType==AreaSectionType.GroupHeader) {
							greatestObjectHeight=Math.Max(greatestObjectHeight,lineObject.Size.Height);
						}
						if(section.SectionType==AreaSectionType.GroupTitle) {
							groupTitleHeight+=lineObject.Size.Height;
						}
					}
					else if(reportObject.ObjectType==ReportObjectType.FieldObject) {
						fieldObject=reportObject;
						RectangleF layoutRect;
						strFormat=ReportObject.GetStringFormatAlignment(fieldObject.ContentAlignment);
						if(fieldObject.FieldDefKind==FieldDefKind.DataTableField) {
							layoutRect=new RectangleF(xPos+fieldObject.Location.X,yPos+fieldObject.Location.Y,fieldObject.Size.Width,queryObj.RowHeightValues[i]);
							if(_myReport.HasGridLines() && isPrintablePage) {
								g.DrawRectangle(new Pen(Brushes.LightGray),Rectangle.Round(layoutRect));
							}
							displayText=queryObj.ReportTable.Rows[i][queryObj.ArrDataFields.IndexOf(fieldObject.DataField)].ToString();
							List<string> listString=GetDisplayString(displayText,prevDisplayText,fieldObject,i,queryObj);
							displayText=listString[0];
							prevDisplayText=listString[1];
							//suppress if duplicate:
							if(i>0 && fieldObject.SuppressIfDuplicate && displayText==prevDisplayText) {
								displayText="";
							}
						}
						else {
							displayText=fieldObject.GetSummaryValue(queryObj.ReportTable,queryObj.ArrDataFields.IndexOf(fieldObject.SummarizedField)).ToString(fieldObject.StringFormat);
							using(Font fontBold=new Font(fieldObject.Font.FontFamily,fieldObject.Font.Size,FontStyle.Bold)) {
								layoutRect=new RectangleF(xPos+fieldObject.Location.X,yPos+fieldObject.Location.Y,fieldObject.Size.Width,
									g.MeasureString(displayText,fontBold,fieldObject.Size.Width).Height);
							}
						}
						if(isPrintablePage) {
							g.DrawString(displayText,fieldObject.Font
							,new SolidBrush(fieldObject.ForeColor),new RectangleF(layoutRect.X+1,layoutRect.Y+1,layoutRect.Width,layoutRect.Height-1),strFormat);
						}
						yPosAdd=(int)layoutRect.Height;
					}
				}
				if(section.SectionType==AreaSectionType.GroupFooter) {
					yPosAdd+=GROUP_FOOTER_BUFFER;//Added to give a buffer between split tables.
					section.Height+=yPosAdd;
					_heightRemaining-=section.Height;
					break;
				}
				else if(section.SectionType==AreaSectionType.GroupTitle) {
					section.Height+=groupTitleHeight;
					_heightRemaining-=section.Height;
					break;
				}
				else if(section.SectionType==AreaSectionType.GroupHeader) {
					section.Height=greatestObjectHeight;
					_heightRemaining-=section.Height;
					break;
				}
				else if(section.SectionType==AreaSectionType.Detail) {
					_rowsPrinted++;
					_totalRowsPrinted++;
					yPos+=yPosAdd;
					_heightRemaining-=yPosAdd;
					section.Height+=yPosAdd;
					yPosAdd=0;//reset for next loop
				}
			}
			if(_rowsPrinted==queryObj.ReportTable.Rows.Count) {
				_rowsPrinted=0;
				queryObj.IsPrinted=true;
			}
		}

		private double GetGroupSummaryValue(string columnName,List<int> summaryGroups,SummaryOperation operation) {
			double retVal=0;
			for(int i=0;i<_myReport.ReportObjects.Count;i++) {
				if(_myReport.ReportObjects[i].ObjectType!=ReportObjectType.QueryObject) {
					continue;
				}
				QueryObject queryObj=(QueryObject)_myReport.ReportObjects[i];
				if(!summaryGroups.Contains(queryObj.QueryGroupValue)) {
					continue;
				}
				for(int j=0;j<queryObj.ReportTable.Rows.Count;j++) {
					if(operation==SummaryOperation.Sum) {
						//This could be enhanced in the future to only sum up the cells that match the column name within the current query group.
						//Right now, if multiple query groups share the same column name that is being summed, the total will include both sets.
						if(queryObj.IsNegativeSummary) {
							retVal-=PIn.Double(queryObj.ReportTable.Rows[j][queryObj.ReportTable.Columns.IndexOf(columnName)].ToString());
						}
						else {
							retVal+=PIn.Double(queryObj.ReportTable.Rows[j][queryObj.ReportTable.Columns.IndexOf(columnName)].ToString());
						}
					}
					else if(operation==SummaryOperation.Count) {
						retVal++;
					}
				}
			}
			return retVal;
		}

		private List<string> GetDisplayString(string rawText,string prevDisplayText,ReportObject reportObject,int i,QueryObject queryObj) {
			return GetDisplayString(rawText,prevDisplayText,reportObject,i,queryObj,false);
		}

		///<summary>This takes in a raw report object and returns the display text and previous display text. For dates, it will return a blank string if the year
		///is less than 1880.</summary>
		private List<string> GetDisplayString(string rawText,string prevDisplayText,ReportObject reportObject,int i,QueryObject queryObj,bool isExport) {
			string displayText="";
			List<string> retVals=new List<string>();
			DataTable dt=queryObj.ReportTable;
			//For exporting, we need to use the ExportTable which is the data that is visible to the user.  Using ReportTable would show raw query data (potentially different than what the user sees).
			if(isExport) {
				dt=queryObj.ExportTable;
			}
			if(reportObject.FieldValueType==FieldValueType.Age) {
				displayText=Patients.AgeToString(Patients.DateToAge(PIn.Date(rawText)));//(fieldObject.FormatString);
			}
			else if(reportObject.FieldValueType==FieldValueType.Boolean) {
				if(PIn.Bool(dt.Rows[i][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString())) {
					displayText="X";
				}
				else {
					displayText="";
				}
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=PIn.Bool(dt.Rows[i-1][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString();
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.Date) {
				DateTime rowDateTime=PIn.DateT(dt.Rows[i][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString());
				if(rowDateTime.Year>1880) {
					displayText=rowDateTime.ToString(reportObject.StringFormat);
				}
				if(i>0 && reportObject.SuppressIfDuplicate) {
					rowDateTime=PIn.DateT(dt.Rows[i-1][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString());
					prevDisplayText="";
					if(rowDateTime.Year>1880) {
						prevDisplayText=rowDateTime.ToString(reportObject.StringFormat);
					}
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.Integer) {
				displayText=PIn.Long(dt.Rows[i][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=PIn.Long(dt.Rows[i-1][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.Number) {
				displayText=PIn.Double(dt.Rows[i][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=PIn.Double(dt.Rows[i-1][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.String) {
				displayText=rawText;
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=dt.Rows[i-1][queryObj.ArrDataFields.IndexOf(reportObject.DataField)].ToString();
				}
			}
			retVals.Add(displayText);
			retVals.Add(prevDisplayText);
			return retVals;
		}

		private void OnPrint_Click() {
			_totalNumberPagesPrinted=0;
			int totalPages=_totalPages;//_totalPages gets set to _pagesProcessed in the RestedODPrintout function. 
			if(ResetODprintout() && PrinterL.TryPrint(_printout) && _totalNumberPagesPrinted!=totalPages) {
				//We must refresh the window here or it will display the printout as missing any pages beyond ex.PageSettings.PrinterSettings.ToPage.
				RefreshWindow();
			}
		}

		private void OnBack_Click(){
			PrevPage();
		}

		private void OnFwd_Click(){
			NextPage();
		}

		private void OnZoomIn_Click() {
			printPreviewControl2.Zoom=printPreviewControl2.Zoom*2;
		}

		private void OnZoomOut_Click() {
			printPreviewControl2.Zoom=printPreviewControl2.Zoom/2;
		}

		private void OnZoomReset_Click() {
			SetDefaultZoom();
		}

		private void PrevPage() {
			if(printPreviewControl2.StartPage==0) {
				return;
			}
			printPreviewControl2.StartPage--;
			SetPageNavString();
		}

		private void NextPage() {
			if(printPreviewControl2.StartPage==_totalPages-1) {
				return;
			}
			printPreviewControl2.StartPage++;
			SetPageNavString();
		}

		public void SetPageNavString() {
			ToolBarMain.Buttons["PageNum"].PageValue=(printPreviewControl2.StartPage+1);
			ToolBarMain.Buttons["PageNum"].PageMax=_totalPages;
			ToolBarMain.Invalidate();
		}

		private void OnExport_Click(){
			string filePath;
			if(ODBuild.IsWeb()) {
				string fileName=_myReport.ReportName+".txt";
				filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			}
			else {
				SaveFileDialog saveFileDialog2=new SaveFileDialog();
				saveFileDialog2.AddExtension=true;
				//saveFileDialog2.Title=Lan.g(this,"Select Folder to Save File To");
				saveFileDialog2.FileName=_myReport.ReportName+".txt";
				if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					try {
						Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
						saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
					catch {
						//initialDirectory will be blank
					}
				}
				else {
					saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				//saveFileDialog2.DefaultExt="txt";
				saveFileDialog2.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
				saveFileDialog2.FilterIndex=0;
				if(saveFileDialog2.ShowDialog()!=DialogResult.OK) {
					return;
				}
				filePath=saveFileDialog2.FileName;
			}
			try {
				using(StreamWriter sw=new StreamWriter(filePath,false)) {
					String line="";
					foreach(ReportObject reportObject in _myReport.ReportObjects) {
						if(reportObject.ObjectType==ReportObjectType.QueryObject) {
							QueryObject query=(QueryObject)reportObject;
							line=query.GetGroupTitle().StaticText;
							sw.WriteLine(line);
							line="";
							for(int i=0;i<query.ExportTable.Columns.Count;i++) {
								DataColumn col=query.ExportTable.Columns[i];
								if(Regex.IsMatch(col.Caption,@"^[@\-+=']{1}")) {
									line+="'";//start with ' to escape one of the special characters @, -, +, =, or '
								}
								line+=col.Caption;
								if(i<query.ExportTable.Columns.Count-1) {
									line+="\t";
								}
							}
							//To preserve forms with multi line headers, we are just stripping any carriage returns and newlines on exporting a report
							line=line.Replace("\r\n"," ");
							sw.WriteLine(line);
							string cell;
							for(int i=0;i<query.ExportTable.Rows.Count;i++) {
								line="";
								string displayText="";
								foreach(ReportObject reportObj in query.ReportObjects) {
									if(reportObj.SectionType!=AreaSectionType.Detail) {
										continue;
									}
									string rawText="";
									if(reportObj.ObjectType==ReportObjectType.FieldObject) {
										rawText=query.ExportTable.Rows[i][query.ArrDataFields.IndexOf(reportObj.DataField)].ToString();
										if(String.IsNullOrWhiteSpace(rawText)) {
											line+="\t";
											continue;
										}
										List<string> listString=GetDisplayString(rawText,"",reportObj,i,query,true);
										displayText=listString[0];
									}
									cell=displayText;
									cell=cell.Replace("\r","");
									cell=cell.Replace("\n","");
									cell=cell.Replace("\t","");
									cell=cell.Replace("\"","");
									line+=cell;
									if(query.ArrDataFields.IndexOf(reportObj.DataField)<query.ArrDataFields.Count-1) {
										line+="\t";
									}
								}
								sw.WriteLine(line);
							}
							int columnValue=-1;
							line="";
							foreach(ReportObject reportObjQuery in query.ReportObjects) {
								if(reportObjQuery.SectionType==AreaSectionType.GroupFooter && reportObjQuery.Name.Contains("Footer")) {
									if(columnValue==-1) {
										columnValue=query.ArrDataFields.IndexOf(reportObjQuery.SummarizedField);
										for(int i=0;i<columnValue;i++) {
											line+=" \t";
										}
									}
									line+=reportObjQuery.GetSummaryValue(query.ExportTable,query.ArrDataFields.IndexOf(reportObjQuery.SummarizedField)).ToString(reportObjQuery.StringFormat)+"\t";
								}
							}
							sw.WriteLine(line);
						}
					}
				}//using
			}
			catch {
				MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MessageBox.Show(Lan.g(this,"File created successfully"));
			}
		}

    private void OnWrapText_Click() {
      _isWrappingText=!_isWrappingText;
			RefreshWindow();
		}

		private void button1_Click(object sender,System.EventArgs e) {
			//ScriptEngine.FormulaCode = 
			/*string functionCode=
			@"using System.Windows.Forms;
				using System;
				public class Test{
					public static void Main(){
						MessageBox.Show(""This is a test"");
						Test2 two = new Test2();
						two.Stuff();
					}
				}
				public class Test2{
					public void Stuff(){

					}
				}";
			CodeDomProvider codeProvider=new CSharpCodeProvider();
			ICodeCompiler compiler = codeProvider.CreateCompiler();
			CompilerParameters compilerParams = new CompilerParameters();
			compilerParams.CompilerOptions = "/target:library /optimize";
			compilerParams.GenerateExecutable = false;
			compilerParams.GenerateInMemory = true;
			compilerParams.IncludeDebugInformation = false;
			compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
			compilerParams.ReferencedAssemblies.Add("System.dll");
			compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
			CompilerResults results = compiler.CompileAssemblyFromSource(
                             compilerParams,functionCode);
			if (results.Errors.Count > 0){
				MessageBox.Show(results.Errors[0].ErrorText);
				//foreach (CompilerError error in results.Errors)
				//	DotNetScriptEngine.LogAllErrMsgs("Compine Error:"+error.ErrorText); 
				return;
			}
			Assembly assembly = results.CompiledAssembly;	
			//Use reflection to call the Main function in the assembly
			ScriptEngine.RunScript(assembly, "Main");		
			*/

		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

		
	}
}
