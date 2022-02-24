/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Text;
using fyiReporting.RDL;

namespace fyiReporting.RdlViewer
{
	/// <summary>
	/// RdlViewer displays RDL files or syntax. 
	/// </summary>
	public class RdlViewer : System.Windows.Forms.Control
	{
		public NeedPassword GetDataSourceReferencePassword=null;
		bool _InPaint=false;
		bool _InLoading=false;
		private string _SourceFileName;		// file name to use
		private string _SourceRdl;			// source Rdl; if provided overrides filename
		private string _Parameters;			// parameters to run the report
		private Report _Report;				// the report
		private string _Folder;				// folder for DataSourceReference (if file name not provided)
		private Pages _pgs;					// the pages of the report to view
		//private PageDrawing _pd;			// draws the pages of a report
		private bool _loadFailed;			// last load of report failed
		private float _leftMargin;			// left margin; calculated based on size of window & scroll style
		// report information
		private float _PageWidth;			// width of page
		private float _PageHeight;			// height of page
		private string _ReportDescription;
		private string _ReportAuthor;
		private string _ReportName;
		private IList _errorMsgs;

		// Zoom
		private float _zoom;				// zoom factor
		private float DpiX;
		private float DpiY;
		private ZoomEnum _zoomMode=ZoomEnum.FitWidth;
		private float _leftGap=10;			// right margin: 10 points
		private float _rightGap=10;			// left margin: 10 points
		private float _pageGap=10;			// gap between pages: 10 points

		// printing 
		private int printEndPage;			// end page
		private int printCurrentPage;		// current page to print

		// Scrollbars
		private ScrollModeEnum _ScrollMode;
		private VScrollBar _vScroll;
		private ToolTip _vScrollToolTip;
		private HScrollBar _hScroll;

		private PageDrawing _DrawPanel;		// the main drawing panel
		private Button _RunButton;
		private PictureBox _WarningButton;
		private ScrollableControl _ParameterPanel;	// panel for specifying parameters
		private int _ParametersMaxHeight;			// max height of controls in _ParameterPanel

		private bool _ShowParameters=true;

		public RdlViewer()
		{
			_SourceFileName=null;
			_SourceRdl=null;
			_Parameters=null;				// parameters to run the report
			_pgs=null;						// the pages of the report to view
			_loadFailed=false;	
			_PageWidth=0;
			_PageHeight=0;
			_ReportDescription=null;
			_ReportAuthor=null;
			_ReportName=null;
			_zoom=-1;						// force zoom to be calculated

			// Get our graphics DPI					   
			Graphics g = null;
			try
			{
				g = this.CreateGraphics(); 
				DpiX = g.DpiX;
				DpiY = g.DpiY;
			}
			catch
			{
				DpiX = DpiY = 96;
			}
			finally
			{
				if (g != null)
					g.Dispose();
			}

			_ScrollMode = ScrollModeEnum.Continuous;

			// Handle the controls
			_vScroll = new VScrollBar();
			_vScroll.Scroll += new ScrollEventHandler(this.VerticalScroll);
			_vScroll.Enabled = false;

			// tooltip 
			_vScrollToolTip = new ToolTip();
			_vScrollToolTip.AutomaticDelay = 100;	// .1 seconds
			_vScrollToolTip.AutoPopDelay = 1000;	// 1 second
			_vScrollToolTip.ReshowDelay = 100;		// .1 seconds
			_vScrollToolTip.InitialDelay = 10;		// .01 seconds
			_vScrollToolTip.ShowAlways = false;
			_vScrollToolTip.SetToolTip(_vScroll, "");

			_hScroll = new HScrollBar();
			_hScroll.Scroll += new ScrollEventHandler(this.HorizontalScroll);
			_hScroll.Enabled = false;

			_DrawPanel = new PageDrawing(null);
			_DrawPanel.Paint += new PaintEventHandler(this.DrawPanelPaint);
			_DrawPanel.Resize += new EventHandler(this.DrawPanelResize); 
			_DrawPanel.MouseWheel +=new MouseEventHandler(DrawPanelMouseWheel);
			_DrawPanel.KeyDown += new KeyEventHandler(DrawPanelKeyDown);

			_RunButton = new Button();
			_RunButton.Parent = this;
			_RunButton.Text = "Run Report";
			_RunButton.Width = 90;
			_RunButton.FlatStyle = FlatStyle.Flat;
			_RunButton.Click += new System.EventHandler(ParametersViewClick);

			_WarningButton = new PictureBox();
			_WarningButton.Parent = this;
			_WarningButton.Width = 15;
			_WarningButton.Height = 15;
			_WarningButton.Paint +=new PaintEventHandler(_WarningButton_Paint);
			_WarningButton.Click += new System.EventHandler(WarningClick);
			ToolTip tip = new ToolTip();
			tip.AutomaticDelay = 500;
			tip.ShowAlways = true;
			tip.SetToolTip(_WarningButton, "Click to see Report Warnings");

			_ParameterPanel = new ScrollableControl();


			this.Layout +=new LayoutEventHandler(RdlViewer_Layout);
			this.SuspendLayout();		 

			// Must be added in this order for DockStyle to work correctly
			this.Controls.Add(_DrawPanel);
			this.Controls.Add(_vScroll);
			this.Controls.Add(_hScroll);
			this.Controls.Add(_ParameterPanel);

			this.ResumeLayout(false);
		}

		/// <summary>
		/// True if Parameter panel should be shown. 
		/// </summary>
		public bool ShowParameterPanel
		{
			get 
			{
				LoadPageIfNeeded();
				return _ShowParameters;
			}
			set 
			{
				_ShowParameters = value;
				RdlViewer_Layout(this, null);				// re layout based on new report
			}
		}
/// <summary>
/// Returns the number of pages in the report.  0 is returned if no report has been loaded.
/// </summary>
		public int PageCount
		{
			get 
			{
				LoadPageIfNeeded();
				if (_pgs == null) 
					return 0;
				else
					return _pgs.PageCount;
			}
		}

		/// <summary>
		/// Sets/Returns the page currently showing
		/// </summary>
		public int PageCurrent
		{
			get 
			{
                if (_pgs == null)
                    return 0;
				int pc = (int) (_pgs.PageCount * (long) _vScroll.Value / (double) _vScroll.Maximum)+1; 
				if (pc > _pgs.PageCount)
					pc = _pgs.PageCount;
				return pc;
			}
			set 
			{
                if (_pgs == null)
                    return;
				// Contributed by Henrique (h2a) 07/14/2006
				if(value <= _pgs.PageCount && value >= 1) 
				{ 
					_vScroll.Value = (int)(_vScroll.Maximum / _pgs.PageCount * (value -1)); 

					string tt = string.Format("Page {0} of {1}", 
						(int) (_pgs.PageCount * (long) _vScroll.Value / (double) _vScroll.Maximum)+1, 
						_pgs.PageCount); 

					_vScrollToolTip.SetToolTip(_vScroll, tt); 

					_DrawPanel.Invalidate(); 
				}
				else
					throw new ArgumentOutOfRangeException("PageCurrent", value, String.Format("Value must be between 1 and {0}.", _pgs.PageCount));
			}
		}

		/// <summary>
		/// Gets the report definition.
		/// </summary>
		public Report Report
		{
			get 
			{
				LoadPageIfNeeded();
				return _Report; 
			}
		}

		/// <summary>
		/// Forces the report to get rebuilt especially after changing parameters or data.
		/// </summary>
		public void Rebuild()
		{
			LoadPageIfNeeded();

			if (_Report == null)
				throw new Exception("Report must be loaded prior to Rebuild being called.");

			_pgs = GetPages(this._Report);
			_DrawPanel.Pgs = _pgs;
			_vScroll.Value = 0;
			CalcZoom(); 
			_DrawPanel.Invalidate();   
		}
		/// <summary>
		/// Gets/Sets the ScrollMode.  
		///		SinglePage: Shows a single page shows in pane.
		///		Continuous: Shows pages as a continuous vertical column.
		///		Facing: Shows first page on right side of pane, then alternating
		///				with single page scrolling.
		///		ContinuousFacing: Shows 1st page on right side of pane, then alternating 
		///				with continuous scrolling.
		/// </summary>
		public ScrollModeEnum ScrollMode
		{
			get { return _ScrollMode; }
			set 
			{ 
				_ScrollMode = value; 
				CalcZoom(); 
				this._DrawPanel.Invalidate();
			}
		}

		/// <summary>
		/// Holds a file name that contains the RDL (Report Specification Language).  Setting
		/// this field will cause a new report to be loaded into the viewer.
		/// SourceFile is mutually exclusive with SourceRdl.  Setting SourceFile will nullify SourceRdl.
		/// </summary>
		public string SourceFile
		{
			get 
			{
				return _SourceFileName;
			}
			set 
			{
				_SourceFileName=value;
				_SourceRdl = null;
				_vScroll.Value = _hScroll.Value = 0;
				_pgs = null;				// reset pages, only if SourceRdl is also unavailable
				_DrawPanel.Pgs = null;
				_loadFailed=false;			// attempt to load the report
				if (this.Visible)
				{
					LoadPageIfNeeded();			// force load of report
					this._DrawPanel.Invalidate();
				}
			}
		}

		/// <summary>
		/// Holds the XML source of the report in RDL (Report Specification Language).
		/// SourceRdl is mutually exclusive with SourceFile.  Setting SourceRdl will nullify SourceFile.
		/// </summary>
		public string SourceRdl
		{
			get {return _SourceRdl;}
			set 
			{
				_SourceRdl=value;
				_SourceFileName=null;
				_pgs = null;				// reset pages
				_DrawPanel.Pgs = null;
				_loadFailed=false;			// attempt to load the report	
				_vScroll.Value = _hScroll.Value = 0;
				if (this.Visible)
				{
					LoadPageIfNeeded();			// force load of report
					this._DrawPanel.Invalidate();
				}
			}
		}
		
		/// <summary>
		/// Holds the folder to data source reference files when SourceFileName not available.
		/// </summary>
		public string Folder
		{
			get {return _Folder;}
			set {_Folder = value;}
		}

		/// <summary>
		/// Parameters passed to report when run.  Parameters are separated by '&'.  For example,
		/// OrderID=10023&OrderDate=10/14/2002
		/// Note: these parameters will override the user specified ones.
		/// </summary>
		public string Parameters
		{
			get {return _Parameters;}
			set {_Parameters=value;}
		}

		/// <summary>
		/// The height of the report page (in points) as defined within the report.
		/// </summary>
		public float PageHeight
		{
			get 
			{
				LoadPageIfNeeded();
				return _PageHeight;
			}
		}

		/// <summary>
		/// The width of the report page (in points) as defined within the report.
		/// </summary>
		public float PageWidth
		{
			get 
			{
				LoadPageIfNeeded();
				return _PageWidth;
			}
		}

		/// <summary>
		/// Description of the report.
		/// </summary>
		public string ReportDescription
		{
			get 
			{
				LoadPageIfNeeded();
				return _ReportDescription;
			}
		}

		/// <summary>
		/// Author of the report.
		/// </summary>
		public string ReportAuthor
		{
			get 
			{
				LoadPageIfNeeded();
				return _ReportAuthor;
			}
		}

		/// <summary>
		/// Name of the report.
		/// </summary>
		public string ReportName
		{
			get 
			{
				return _ReportName;
			}
			set {_ReportName = value;}
		}

		/// <summary>
		/// Zoom factor.  For example, .5 is a 50% reduction, 2 is 200% increase.
		/// Setting this value will force ZoomMode to UseZoom.
		/// </summary>
		public float Zoom
		{
			get {return _zoom;}
			set 
			{
				_zoom = value;
				this._zoomMode = ZoomEnum.UseZoom;
				CalcZoom();			// this adjust any scrolling issues
				this._DrawPanel.Invalidate();
			}
		}

		/// <summary>
		/// ZoomMode.  Optionally, allows zoom to dynamically change depending on pane size.
		/// </summary>
		public ZoomEnum ZoomMode
		{
			get {return _zoomMode; }
			set 
			{
				_zoomMode = value; 
				CalcZoom();				// force zoom calculation
				this._DrawPanel.Invalidate();
			}
		}

		/// <summary>
		/// Print the report.
		/// </summary>
		public void Print(PrintDocument pd)
		{
			LoadPageIfNeeded();

			pd.PrintPage += new PrintPageEventHandler(PrintPage);
			printCurrentPage=-1;
			switch (pd.PrinterSettings.PrintRange)
			{
				case PrintRange.AllPages:
					printCurrentPage = 0;
					printEndPage = _pgs.PageCount - 1;
					break;
				case PrintRange.Selection:
					printCurrentPage = pd.PrinterSettings.FromPage - 1;
					printEndPage = pd.PrinterSettings.FromPage - 1;
					break;
				case PrintRange.SomePages:
					printCurrentPage = pd.PrinterSettings.FromPage - 1;
					if (printCurrentPage < 0)
						printCurrentPage = 0;
					printEndPage = pd.PrinterSettings.ToPage - 1;
					if (printEndPage >= _pgs.PageCount)
						printEndPage = _pgs.PageCount - 1;
					break;
			}
			pd.Print();
		}

		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			System.Drawing.Rectangle r = new System.Drawing.Rectangle(0, 0, int.MaxValue, int.MaxValue);
			_DrawPanel.Draw(e.Graphics, printCurrentPage, r, false);	 

			printCurrentPage++;
			if (printCurrentPage > printEndPage)
				e.HasMorePages = false;
			else
				e.HasMorePages = true;
		}

		/// <summary>
		/// Save the file.  The extension determines the type of file to save.
		/// </summary>
		/// <param name="FileName">Name of the file to be saved to.</param>
		/// <param name="ext">Type of file to save.  Should be "pdf", "xml", "html", "mhtml".</param>
		public void SaveAs(string FileName, string type)
		{
			LoadPageIfNeeded();

			string ext = type.ToLower();
			OneFileStreamGen sg = new OneFileStreamGen(FileName, true);	// overwrite with this name
			try
			{
				switch(ext)
				{
					case "pdf":	
						_Report.RunRenderPdf(sg, _pgs);
						break;
					case "xml": 
						_Report.RunRender(sg, OutputPresentationType.XML);
						break;																  
					case "html": case "htm":
						_Report.RunRender(sg, OutputPresentationType.HTML);
						break;
					case "mhtml": case "mht":
						_Report.RunRender(sg, OutputPresentationType.MHTML);
						break;
					default:
						throw new Exception("Unsupported file extension for SaveAs");
				}
			}
			finally
			{
				if (sg != null)
				{
					sg.CloseMainStream();
				}
			}
			return;
		}

		private void DrawPanelPaint(object sender, System.Windows.Forms.PaintEventArgs e)
			{
			// Only handle one paint at a time
			lock (this)
			{
				if (_InPaint)
					return;
				_InPaint=true;
			}

			Graphics g = e.Graphics;
			try			// never want to die in here
			{
				if (!_InLoading)				// If we're in the process of loading don't paint
				{
					LoadPageIfNeeded();				// make sure we have something to show
				
					if (_zoom < 0)
						CalcZoom();				// new report or resize client requires new zoom factor
				
					// Draw the page
					_DrawPanel.Draw(g,_zoom, _leftMargin, _pageGap, 
						PointsX(_hScroll.Value), PointsY(_vScroll.Value),	
						e.ClipRectangle);
				}
			}
			catch (Exception ex)
			{	// don't want to kill process if we die
				using (Font font = new Font("Arial", 8))	
					g.DrawString(ex.Message+"\r\n"+ex.StackTrace, font, Brushes.Black,0,0);
			}		
			
			lock (this)
			{
				_InPaint=false;
			}
		}

		private void DrawPanelResize(object sender, EventArgs e)
		{
			CalcZoom();							// calc zoom
			_DrawPanel.Refresh();
		}

		private float PointsX(float x)		// pixels to points
		{
			return x * 72f / DpiX;
		}

		private float PointsY(float y)
		{
			return y * 72f / DpiY;
		}

		private float POINTSIZEF = 72.27f;

		private int PixelsX(float x)		// points to pixels
		{
			return (int) (x * DpiX / POINTSIZEF);
		}

		private int PixelsY(float y)
		{
			return (int) (y * DpiY / POINTSIZEF);
		}

		private void CalcZoom()
		{
			switch (_zoomMode)
			{
				case ZoomEnum.UseZoom:
					if (_zoom <= 0)			// normalize invalid values
						_zoom = 1;
					break;					// nothing to calculate
				case ZoomEnum.FitWidth:
					CalcZoomFitWidth();
					break;
				case ZoomEnum.FitPage:
					CalcZoomFitPage();
					break;
			}
			if (_zoom <= 0)
				_zoom = 1;
			float w = PointsX(_DrawPanel.Width);	// convert to points

			if (w > (this._PageWidth + _leftGap + _rightGap)*_zoom)
				_leftMargin = ((w -(this._PageWidth + _leftGap + _rightGap)*_zoom)/2)/_zoom;
			else
				_leftMargin = _leftGap;
			if (_leftMargin < 0)
				_leftMargin = 0;
			SetScrollControls();			// zoom affects the scroll bars
			return;
		}
		
		private void CalcZoomFitPage()
		{
			try
			{
				float w = PointsX(_DrawPanel.Width);	// convert to points
				float h = PointsY(_DrawPanel.Height);
				float xratio = w / (this._PageWidth + _leftGap + _rightGap);
				float yratio = h / (this._PageHeight + this._pageGap + this._pageGap);	
				_zoom = Math.Min(xratio, yratio);
			}
			catch
			{
				_zoom = 1;			// shouldn't ever happen but this routine must never throw exception
			}
		}

		private void CalcZoomFitWidth()
		{
			try
			{
				float w = PointsX(_DrawPanel.Width);	// convert to points
				float h = PointsY(_DrawPanel.Height);
				_zoom = w / (this._PageWidth + _leftGap + _rightGap);

			}
			catch
			{
				_zoom = 1;			// shouldn't ever happen but this routine must never throw exception
			}
		}

		// Obtain the Pages by running the report
		private Report GetReport()
		{
			string prog;

			// Obtain the source
			if (_loadFailed)	
				prog = GetReportErrorMsg();
			else if (_SourceRdl != null)
				prog = _SourceRdl;
			else if (_SourceFileName != null)
				prog = GetRdlSource();
			else	
				prog = GetReportEmptyMsg();

			// Compile the report
			// Now parse the file
			RDLParser rdlp;
			Report r;
			try
			{
				_errorMsgs = null;
				rdlp =  new RDLParser(prog);
				rdlp.DataSourceReferencePassword = GetDataSourceReferencePassword;
				if (_SourceFileName != null)
					rdlp.Folder = Path.GetDirectoryName(_SourceFileName);
				else
					rdlp.Folder = this.Folder;

				r = rdlp.Parse();
				if (r.ErrorMaxSeverity > 0) 
				{
					_errorMsgs = r.ErrorItems;		// keep a copy of the errors

					int severity = r.ErrorMaxSeverity;
					r.ErrorReset();
					if (severity > 4)
					{
						r = null;			// don't return when severe errors
						_loadFailed=true;
					}
				}
				// If we've loaded the report; we should tell it where it got loaded from
				if (r != null && !_loadFailed)
				{	// Don't care much if this fails; and don't want to null out report if it does
					try 
					{
						if (_SourceFileName != null)
						{
							r.Name = Path.GetFileNameWithoutExtension(_SourceFileName);
							r.Folder = Path.GetDirectoryName(_SourceFileName);
						}
						else
						{
							r.Folder = this.Folder;
							r.Name = this.ReportName;
						}
					}
					catch {}
				}
			}
			catch (Exception ex)
			{
				_loadFailed=true;
				_errorMsgs = new List<string>();		// create new error list
				_errorMsgs.Add(ex.Message);			// put the message in it
				_errorMsgs.Add(ex.StackTrace);		//   and the stack trace
				r = null;
			}

			if (r != null)
			{
				_PageWidth = r.PageWidthPoints;
				_PageHeight = r.PageHeightPoints;
				_ReportDescription = r.Description;
				_ReportAuthor = r.Author;
				
				ParametersBuild(r);
			}
			else
			{
				_PageWidth = 0;
				_PageHeight = 0;
				_ReportDescription = null;
				_ReportAuthor = null;
				_ReportName = null;
			}
			return r;
		}

		private string GetReportEmptyMsg()
		{
			string prog = "<Report><Width>8.5in</Width><Body><Height>1in</Height><ReportItems><Textbox><Value></Value><Style><FontWeight>Bold</FontWeight></Style><Height>.3in</Height><Width>5 in</Width></Textbox></ReportItems></Body></Report>";
			return prog;
		}

		private string GetReportErrorMsg()
		{
			string data1 = @"<?xml version='1.0' encoding='UTF-8'?>
<Report> 
	<LeftMargin>.4in</LeftMargin><Width>8.5in</Width>
	<Author></Author>
	<DataSources>
		<DataSource Name='DS1'>
			<ConnectionProperties> 
				<DataProvider>xxx</DataProvider>
				<ConnectString></ConnectString>
			</ConnectionProperties>
		</DataSource>
	</DataSources>
	<DataSets>
		<DataSet Name='Data'>
			<Query>
				<DataSourceName>DS1</DataSourceName>
			</Query>
			<Fields>
				<Field Name='Error'> 
					<DataField>Error</DataField>
					<TypeName>String</TypeName>
				</Field>
			</Fields>";
			
			string data2 = @"
		</DataSet>
	</DataSets>
	<PageHeader>
		<Height>1 in</Height>
		<ReportItems>
			<Textbox><Top>.1in</Top><Value>fyiReporting Software, LLC</Value><Style><FontSize>18pt</FontSize><FontWeight>Bold</FontWeight></Style></Textbox>
			<Textbox><Top>.1in</Top><Left>4.25in</Left><Value>=Globals!ExecutionTime</Value><Style><Format>dddd, MMMM dd, yyyy hh:mm:ss tt</Format><FontSize>12pt</FontSize><FontWeight>Bold</FontWeight></Style></Textbox>
			<Textbox><Top>.5in</Top><Value>Errors processing report</Value><Style><FontSize>12pt</FontSize><FontWeight>Bold</FontWeight></Style></Textbox>
		</ReportItems>
	</PageHeader>
	<Body><Height>3 in</Height>
		<ReportItems>
			<Table>
				<Style><BorderStyle>Solid</BorderStyle></Style>
				<TableColumns>
					<TableColumn><Width>7 in</Width></TableColumn>
				</TableColumns>
				<Header>
					<TableRows>
						<TableRow>
							<Height>15 pt</Height>
							<TableCells>
								<TableCell>
									<ReportItems><Textbox><Value>Messages</Value><Style><FontWeight>Bold</FontWeight></Style></Textbox></ReportItems>
								</TableCell>
							</TableCells>
						</TableRow>
					</TableRows>
					<RepeatOnNewPage>true</RepeatOnNewPage>
				</Header>
				<Details>
					<TableRows>
						<TableRow>
							<Height>12 pt</Height>
							<TableCells>
								<TableCell>
									<ReportItems><Textbox Name='ErrorMsg'><Value>=Fields!Error.Value</Value><CanGrow>true</CanGrow></Textbox></ReportItems>
								</TableCell>
							</TableCells>
						</TableRow>
					</TableRows>
				</Details>
			</Table>
		</ReportItems>
	</Body>
</Report>";

			StringBuilder sb = new StringBuilder(data1, data1.Length + data2.Length + 1000);
			// Build out the error messages
			sb.Append("<Rows>");
			foreach (string msg in _errorMsgs)
			{
				sb.Append("<Row><Error>");
				string newmsg = msg.Replace("&", @"&amp;");
				newmsg = newmsg.Replace("<", @"&lt;");
				sb.Append(newmsg);
				sb.Append("</Error></Row>");
			}
			sb.Append("</Rows>");
			sb.Append(data2);
			return sb.ToString();
		}

		private Pages GetPages()
		{
			this._Report = GetReport();
			if (_loadFailed)			// retry on failure; this will get error report
				this._Report = GetReport();

			return GetPages(this._Report);
		}

		private Pages GetPages(Report report)
		{
			Pages pgs=null;

			ListDictionary ld = GetParameters();		// split parms into dictionary

			try
			{
				report.RunGetData(ld);

				pgs = report.BuildPages();

				if (report.ErrorMaxSeverity > 0) 
				{
					if (_errorMsgs == null)
						_errorMsgs = report.ErrorItems;		// keep a copy of the errors
					else
					{
						foreach (string err in report.ErrorItems)
							_errorMsgs.Add(err);
					}

					report.ErrorReset();
				}

			}
			catch (Exception e)
			{
				string msg = e.Message;
			}
			
			return pgs;
		}

		private ListDictionary GetParameters()
		{
			ListDictionary ld= new ListDictionary();
			if (_Parameters == null)
				return ld;				// dictionary will be empty in this case

			// parms are separated by &
			char[] breakChars = new char[] {'&'};
			string[] ps = _Parameters.Split(breakChars);
			foreach (string p in ps)
			{
				int iEq = p.IndexOf("=");
				if (iEq > 0)
				{
					string name = p.Substring(0, iEq);
					string val = p.Substring(iEq+1);
					ld.Add(name, val);	
				}
			}
			return ld;
		}

		private string GetRdlSource()
		{
			StreamReader fs=null;
			string prog=null;
			try
			{
				fs = new StreamReader(_SourceFileName);
				prog = fs.ReadToEnd();
			}
			finally
			{
				if (fs != null)
					fs.Close();
			}

			return prog;
		}

		/// <summary>
		/// Call LoadPageIfNeeded when a routine requires the report to be loaded in order
		/// to fulfill the request.
		/// </summary>
		private void LoadPageIfNeeded()
		{
			if (_pgs == null)
			{
				Cursor savec=null;
				try
				{
					_InLoading = true;
					savec = this.Cursor;				// this could take a while so put up wait cursor
					this.Cursor = Cursors.WaitCursor;
					_pgs = GetPages();
					_DrawPanel.Pgs = _pgs;
					CalcZoom();							// this could affect zoom
				}
				finally
				{
					_InLoading = false;
					if (savec != null)
						this.Cursor = savec;
				}
				RdlViewer_Layout(this, null);				// re layout based on new report
			}
		}

		private void ParametersBuild(Report r)
		{
			// Remove all previous controls
			_ParameterPanel.Controls.Clear();
			_ParameterPanel.AutoScroll = true;

			int yPos=10;
			foreach (UserReportParameter rp in r.UserReportParameters)
			{
				if (rp.Prompt == null)		// skip parameters that don't have a prompt
					continue;

				// Create a label
				Label label = new Label();
				label.Parent = _ParameterPanel;
				label.AutoSize = true;
				label.Text = rp.Prompt;
				label.Location = new Point(10, yPos);

				// Create a control
				Control v;
				int width = 90;
				if (rp.DisplayValues == null)
				{
					TextBox tb = new TextBox();
					v = tb;
					tb.Height = tb.PreferredHeight;
					tb.Validated += new System.EventHandler(ParametersTextValidated);
				}
				else
				{
					ComboBox cb = new ComboBox();
					// create a label to auto
					Label l = new Label();
					l.AutoSize = true;
					l.Visible = false;

					cb.Leave += new EventHandler(ParametersLeave);
					v = cb;
					width = 0;
					foreach (string s in rp.DisplayValues)
					{
						l.Text = s;
						if (width < l.Width)
							width = l.Width;
						cb.Items.Add(s);
					}																	   
					if (width > 0)
					{						   
						l.Text = "XX";
						width += l.Width;		// give some extra room for the drop down arrow
					}
					else
						width = 90;				// just force the default
				}
				v.Parent = _ParameterPanel;
				v.Width = width;
				v.Location = new Point(label.Location.X+label.Width+5, yPos);
				if (rp.DefaultValue != null)
				{
					StringBuilder sb = new StringBuilder();
					for (int i=0; i < rp.DefaultValue.Length; i++)
					{
						if (i > 0)
							sb.Append(", ");
						sb.Append(rp.DefaultValue[i].ToString());
					}
					v.Text = sb.ToString();
				}
				v.Tag = rp;
				v.Validated += new System.EventHandler(ParametersTextValidated);
								 
				yPos += Math.Max(label.Height, v.Height) + 5;
			}

			this._ParametersMaxHeight = yPos;
		}

		private void ParametersLeave(object sender, EventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb == null)
				return;

			UserReportParameter rp = cb.Tag as UserReportParameter;
			if (rp == null)
				return;

			try
			{
				rp.Value = cb.Text;			
			}
			catch (ArgumentException ae)
			{
				MessageBox.Show(ae.Message, "Invalid Report Parameter");
			}
		}

		private void ParametersTextValidated(object sender, System.EventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb == null)
				return;

			UserReportParameter rp = tb.Tag as UserReportParameter;
			if (rp == null)
				return;

			try
			{
				rp.Value = tb.Text;			
			}
			catch (ArgumentException ae)
			{
				MessageBox.Show(ae.Message, "Invalid Report Parameter");
			}
		}

		private void ParametersViewClick(object sender, System.EventArgs e)
		{
			_errorMsgs = null;			// reset the error message
			if (this._Report == null)
				return;

			bool bFail = false;
			foreach (UserReportParameter rp in _Report.UserReportParameters)
			{
				if (rp.Prompt == null)
					continue;
				if (rp.Value == null && !rp.Nullable)
				{
					MessageBox.Show(string.Format("Parameter '{0}' is required but not provided.",rp.Prompt), "Report Parameter Missing");
					bFail = true;
				}
			}
			if (bFail)
				return;

			_pgs = GetPages(this._Report);
			_DrawPanel.Pgs = _pgs;
			_vScroll.Value = 0;
			CalcZoom(); 
			_WarningButton.Visible = WarningVisible();
			_DrawPanel.Invalidate();   
		}
		
		private void WarningClick(object sender, System.EventArgs e)
		{
			if (_errorMsgs == null)
				return;						// shouldn't even be visible if no warnings

			DialogMessages dm = new DialogMessages(_errorMsgs);
			dm.ShowDialog();
			return;
		}

		private void SetScrollControls()
		{
			if (_pgs == null)		// nothing loaded; nothing to do
			{
				_vScroll.Enabled = _hScroll.Enabled = false;
				_vScroll.Value = _hScroll.Value = 0;
				return;
			}
			SetScrollControlsV();
			SetScrollControlsH();
		}

		private void SetScrollControlsV()
		{
			// calculate the vertical scroll needed
			float h = PointsY(_DrawPanel.Height);	// height of pane
			if (_zoom * ((this._PageHeight + this._pageGap) * _pgs.PageCount + this._pageGap) <= h)
			{
				_vScroll.Enabled = false;
				_vScroll.Value = 0;
				return;
			}
			_vScroll.Minimum = 0;
			_vScroll.Maximum = (int) (PixelsY((this._PageHeight + this._pageGap) * _pgs.PageCount + this._pageGap));
			_vScroll.Value = Math.Min(_vScroll.Value, _vScroll.Maximum);
			if (this._zoomMode == ZoomEnum.FitPage)
			{
				_vScroll.LargeChange = (int) (_vScroll.Maximum / _pgs.PageCount);
				_vScroll.SmallChange = _vScroll.LargeChange;
			}
			else
			{
				_vScroll.LargeChange = (int) (_DrawPanel.Height / _zoom);
				_vScroll.SmallChange = _vScroll.LargeChange / 5;
			}
			_vScroll.Enabled = true;
			string tt = string.Format("Page {0} of {1}", 
					(int) (_pgs.PageCount * (long) _vScroll.Value / (double) _vScroll.Maximum)+1, 
					_pgs.PageCount);

			_vScrollToolTip.SetToolTip(_vScroll, tt);
//			switch (_ScrollMode)
//			{
//				case ScrollModeEnum.SinglePage:
//					break;
//				case ScrollModeEnum.Continuous:
//				case ScrollModeEnum.ContinuousFacing:
//				case ScrollModeEnum.Facing:
//					break;
//			}
			return;
		}

		private void SetScrollControlsH()
			{
			// calculate the horizontal scroll needed
			float w = PointsX(_DrawPanel.Width);	// width of pane
			if (_zoomMode == ZoomEnum.FitPage || 
				_zoomMode == ZoomEnum.FitWidth ||
				_zoom * (this._PageWidth + this._leftGap + this._rightGap) <= w)
			{
				_hScroll.Enabled = false;
				_hScroll.Value = 0;
				return;
			}

			_hScroll.Minimum = 0;
			_hScroll.Maximum = (int) (PixelsX(this._PageWidth + this._leftGap + this._rightGap) );
			_hScroll.Value = Math.Min(_hScroll.Value, _hScroll.Maximum);
			_hScroll.LargeChange = (int) (_DrawPanel.Width / _zoom);
			_hScroll.SmallChange = _hScroll.LargeChange / 5;
			_hScroll.Enabled = true;

			return;
		}

		private void HorizontalScroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.NewValue == _hScroll.Value)	// don't need to scroll if already there
				return;

			_DrawPanel.Invalidate();   
		}

		private void VerticalScroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.NewValue == _vScroll.Value)	// don't need to scroll if already there
				return;

			string tt = string.Format("Page {0} of {1}", 
				(int) (_pgs.PageCount * (long) _vScroll.Value / (double) _vScroll.Maximum)+1, 
				_pgs.PageCount);
			
			_vScrollToolTip.SetToolTip(_vScroll, tt);

			_DrawPanel.Invalidate();   
		}

		private void DrawPanelMouseWheel(object sender, MouseEventArgs e)
		{
			int wvalue;
			if (e.Delta < 0)
			{
				if (_vScroll.Value < _vScroll.Maximum)
				{
					wvalue = _vScroll.Value + _vScroll.SmallChange;

					_vScroll.Value = Math.Min(_vScroll.Maximum - _DrawPanel.Height, wvalue);
					_DrawPanel.Refresh();
				}
			}
			else 
			{
				if (_vScroll.Value > _vScroll.Minimum)
				{
					wvalue = _vScroll.Value - _vScroll.SmallChange;

					_vScroll.Value = Math.Max(_vScroll.Minimum, wvalue);
					_DrawPanel.Refresh();
				}
			}
		}

		private void DrawPanelKeyDown(object sender, KeyEventArgs e)
		{
			// Force scroll up and down
			if (e.KeyCode == Keys.Down)
			{
				_vScroll.Value = Math.Min(_vScroll.Value + _vScroll.SmallChange, _vScroll.Maximum);
				_DrawPanel.Refresh();
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Up)
			{
				_vScroll.Value = Math.Max(_vScroll.Value - _vScroll.SmallChange, 0);
				_DrawPanel.Refresh();
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.PageDown)
			{
				_vScroll.Value = Math.Min(_vScroll.Value + _vScroll.LargeChange, _vScroll.Maximum);
				_DrawPanel.Refresh();
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.PageUp)
			{
				_vScroll.Value = Math.Max(_vScroll.Value - _vScroll.LargeChange, 0);
				_DrawPanel.Refresh();
				e.Handled = true;
			}
		}

		private bool WarningVisible()
		{
			if (!_ShowParameters)
				return false;

			return _errorMsgs != null;
		}

		private void RdlViewer_Layout(object sender, LayoutEventArgs e)
		{
			int pHeight;
			if (_ShowParameters)
			{	// Only the parameter panel is visible
				_ParameterPanel.Visible = true;
				_RunButton.Visible = true;
				
				_WarningButton.Visible = WarningVisible();

				_ParameterPanel.Location = new Point(0,0);
				_ParameterPanel.Width = this.Width - _RunButton.Width - _WarningButton.Width - 5;
				pHeight = this.Height / 3;
				if (pHeight > _ParametersMaxHeight)
					pHeight = _ParametersMaxHeight;
				if (pHeight < _RunButton.Height + 15)
					pHeight = _RunButton.Height + 15;
				_ParameterPanel.Height = pHeight;
			}
			else
			{
//				pHeight=_RunButton.Height + 15;
				pHeight=0;
				_RunButton.Visible = false;
				_WarningButton.Visible = false;
				_ParameterPanel.Visible = false;
			}
			_DrawPanel.Location = new Point(0, pHeight);
			_DrawPanel.Width = this.Width - _vScroll.Width;
			_DrawPanel.Height = this.Height - _hScroll.Height - pHeight;
			_hScroll.Location = new Point(0, this.Height - _hScroll.Height);
			_hScroll.Width = _DrawPanel.Width;
			_vScroll.Location = new Point(this.Width - _vScroll.Width, _DrawPanel.Location.Y);
			_vScroll.Height = _DrawPanel.Height;

			_RunButton.Location = new Point(this.Width - _RunButton.Width - 2 - _WarningButton.Width, 10);
			_WarningButton.Location = new Point(_RunButton.Location.X + _RunButton.Width + 2, 13);
		}

		private void _WarningButton_Paint(object sender, PaintEventArgs e)
		{
			int midPoint = _WarningButton.Width / 2;
			Graphics g = e.Graphics;
			
			Point[] triangle = new Point[5];
			triangle[0] = triangle[4] = new Point(midPoint-1, 0);
			triangle[1] = new Point(0, _WarningButton.Height-1);
			triangle[2] = new Point(_WarningButton.Width, _WarningButton.Height-1);
			triangle[3] = new Point(midPoint+1, 0);
			g.FillPolygon(Brushes.Yellow, triangle);
			g.DrawPolygon(Pens.Black, triangle);
			g.FillRectangle(Brushes.Red, midPoint - 1, 5, 2, 5);
			g.FillRectangle(Brushes.Red, midPoint - 1, 11, 2, 2);
		}
	}

	public enum ScrollModeEnum
	{
		SinglePage,
		Continuous,
		Facing,
		ContinuousFacing
	}

	public enum ZoomEnum
	{
		UseZoom,
		FitPage,
		FitWidth
	}
}
