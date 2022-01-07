using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Xml;
//using mshtml;

namespace OpenDental {
	///<summary></summary>
	public partial class FormEligibilityResponseDisplay:FormODBase {
		private long _patNum;
		private XmlDocument _xmlDocument;
		private DataSet _dataSet = new DataSet();
		private DataTable _table = new DataTable("EligibilityResponse");
		private DataGridViewPrinter _dataGridViewPrinter;

		///<summary></summary>
		public FormEligibilityResponseDisplay(XmlDocument Request,long PatientID) {
			_xmlDocument = Request;
			_patNum = PatientID;
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormDisplayEligibilityResponse_Load(object sender,System.EventArgs e) {
			displayPatientName();
			prepareDataSetAndTable();
			populateDataTable();
			setDataColumn();
			displayData();
		}

		private void setDataColumn() {
			// Setting the style of the DataGridView control
			this.dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma",9,FontStyle.Bold,GraphicsUnit.Point);
			this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.ControlDark;
			this.dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.dataGridView1.DefaultCellStyle.Font = new Font("Tahoma",8,FontStyle.Regular,GraphicsUnit.Point);
			this.dataGridView1.DefaultCellStyle.BackColor = Color.Empty;
			this.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.ControlLight;
			this.dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
			this.dataGridView1.GridColor = SystemColors.ControlDarkDark;
		}

		private void displayPatientName() {
			this.LblPatientName.Text=Patients.GetEligibilityDisplayName(_patNum);
		}

		private void prepareDataSetAndTable() {
			// Define DataSet and DataTable
			// Define DataColumns
			DataColumn colBenefitInfo = new DataColumn("Benefit Info",System.Type.GetType("System.String"));
			DataColumn colCoverageLevelCode = new DataColumn("Coverage Level",System.Type.GetType("System.String"));
			DataColumn colPlanCoverageDesc = new DataColumn("Plan Coverage",System.Type.GetType("System.String"));
			DataColumn colTimeQualifier = new DataColumn("Time Qualifier",System.Type.GetType("System.String"));
			DataColumn colBenefitAmount = new DataColumn("Benefit Amount",System.Type.GetType("System.String"));
			DataColumn colMessage = new DataColumn("Message",System.Type.GetType("System.String"));
			DataColumn colPercent = new DataColumn("Percent",System.Type.GetType("System.String"));
			DataColumn colProcCode = new DataColumn("ProcCode",System.Type.GetType("System.String"));
			DataColumn colDeliveryPattern = new DataColumn("Delivery Pattern",System.Type.GetType("System.String"));
			// Add Columns to Data Table
			_table.Columns.Add(colBenefitInfo);
			_table.Columns.Add(colCoverageLevelCode);
			_table.Columns.Add(colPlanCoverageDesc);
			_table.Columns.Add(colTimeQualifier);
			_table.Columns.Add(colBenefitAmount);
			_table.Columns.Add(colMessage);
			_table.Columns.Add(colPercent);
			_table.Columns.Add(colProcCode);
			_table.Columns.Add(colDeliveryPattern);
			// Add Tabl to DataSet
			_dataSet.Tables.Add(_table);
		}

		private void populateDataTable() {
			XmlNodeList PlanBenefits;
			XmlNodeList ChildNodes;
			// Declare Variable for each table column
			string BenefitInfo;
			string CoverageLevelCode;
			string PlanCoverageDesc;
			string TimeQualifier;
			string BenefitAmount;
			string Message;
			string Percent;
			string ProcCode;
			string DeliveryPattern;
			// select all Plan Benefit Nodes
			PlanBenefits = _xmlDocument.SelectNodes("EligBenefitResponse/BenefitResponse/PlanBenefit");
			foreach(XmlNode Node in PlanBenefits) {
				ChildNodes = Node.ChildNodes;
				// reset All Prior Column Values
				BenefitInfo = string.Empty;
				CoverageLevelCode = string.Empty;
				PlanCoverageDesc = string.Empty;
				TimeQualifier = string.Empty;
				BenefitAmount = string.Empty;
				Message = string.Empty;
				Percent = string.Empty;
				ProcCode = string.Empty;
				DeliveryPattern = string.Empty;
				foreach(XmlNode ChildNode in ChildNodes) {
					// Process Each Child Node and Get the values back
					switch(ChildNode.Name) {
						case "BenefitInfo":
							BenefitInfo = ChildNode.InnerText;
							break;
						case "CoverageLevelCode":
							CoverageLevelCode = ChildNode.InnerText;
							break;
						case "PlanCoverageDesc":
							PlanCoverageDesc = ChildNode.InnerText;
							break;
						case "TimeQualifier":
							TimeQualifier = ChildNode.InnerText;
							break;
						case "BenefitAmount":
							BenefitAmount = "$" + ChildNode.InnerText;
							break;
						case "Message":
							if(ChildNode.InnerText.Substring(0,3) != "URL")
								Message = ChildNode.InnerText;
							break;
						case "Percent":
							try {
								Percent = System.Convert.ToString(System.Convert.ToDecimal(ChildNode.InnerText) * 100) + "%";
							}
							catch(Exception) {
								Percent = "??";
							}
							break;
						case "Procedure":
							ProcCode = ChildNode.ChildNodes.Item(1).InnerText;
							break;
						case "DeliveryPattern":
							DeliveryPattern = ChildNode.ChildNodes.Item(0).InnerText + "-" + ChildNode.ChildNodes.Item(1).InnerText;
							break;
					}
				}
				// Insert Record Into DataTable
				DataRow DrResponse = _table.NewRow();
				DrResponse["Benefit Info"] = BenefitInfo;
				DrResponse["Coverage Level"] = CoverageLevelCode;
				DrResponse["Plan Coverage"] = PlanCoverageDesc;
				DrResponse["Time Qualifier"] = TimeQualifier;
				DrResponse["Benefit Amount"] = BenefitAmount;
				DrResponse["Message"] = Message;
				DrResponse["Percent"] = Percent;
				DrResponse["ProcCode"] = ProcCode;
				DrResponse["Delivery Pattern"] = DeliveryPattern;
				_table.Rows.Add(DrResponse);
			}
		}

		private void displayData() {
			this.dataGridView1.DataSource = _dataSet;
			this.dataGridView1.DataMember = "EligibilityResponse";
			// Changing the last column alignment to be in the Right alignment            
			this.dataGridView1.Columns[this.dataGridView1.Columns.Count - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			// Adjusting each column to be fit as the content of all its cells, including the header cell
			this.dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

			//this.dataGridView1.AutoResizeColumns();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		private bool SetupThePrinting() {//TODO: Implement ODprintout pattern
			PrintDialog MyPrintDialog = new PrintDialog();
			MyPrintDialog.AllowCurrentPage = false;
			MyPrintDialog.AllowPrintToFile = false;
			MyPrintDialog.AllowSelection = false;
			MyPrintDialog.AllowSomePages = false;
			MyPrintDialog.PrintToFile = false;
			MyPrintDialog.ShowHelp = false;
			MyPrintDialog.ShowNetwork = false;
			MyPrintDialog.UseEXDialog=true;
			if(MyPrintDialog.ShowDialog() != DialogResult.OK){
				return false;
			}
			MyPrintDocument.DocumentName = "Patient Eligibility Response";
			MyPrintDocument.PrinterSettings = MyPrintDialog.PrinterSettings;
			MyPrintDocument.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
			MyPrintDocument.DefaultPageSettings.Margins = new Margins(40,40,40,40);
			_dataGridViewPrinter = new DataGridViewPrinter(this.dataGridView1,MyPrintDocument,true,true,this.LblPatientName.Text,new Font("Tahoma",18,FontStyle.Bold,GraphicsUnit.Point),Color.Black,true);
			return true;
		}

		private void btnPrintPreview_Click(object sender,EventArgs e) {
			if(SetupThePrinting()) {
				PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
				MyPrintPreviewDialog.Document = MyPrintDocument;
				MyPrintPreviewDialog.PrintPreviewControl.Zoom = 1.0;
				((Form)MyPrintPreviewDialog).WindowState = FormWindowState.Maximized;
				MyPrintPreviewDialog.ShowDialog();
			}
		}

		private void MyPrintDocument_PrintPage(object sender,PrintPageEventArgs e) {
			bool more = _dataGridViewPrinter.DrawDataGridView(e.Graphics);
			if(more == true){
				e.HasMorePages = true;
			}
		}

	}
}

class DataGridViewPrinter {
	private DataGridView _dataGridView; // The DataGridView Control which will be printed
	private PrintDocument _printDocument; // The PrintDocument to be used for printing
	private bool _isCenterOnPage; // Determine if the report will be printed in the Top-Center of the page
	private bool _doesContainTitle; // Determine if the page contain title text
	private string _title; // The title text to be printed in each page (if IsWithTitle is set to true)
	private Font _fontTitle; // The font to be used with the title text (if IsWithTitle is set to true)
	private Color _colorTitle; // The color to be used with the title text (if IsWithTitle is set to true)
	private bool _doesUsePaging; // Determine if paging is used
	static int _currentRow; // A static parameter that keep track on which Row (in the DataGridView control) that should be printed
	static int _pageNumber;
	private int _widthPage;
	private int _heightPage;
	private int _leftMargin;
	private int _topMargin;
	private int _rightMargin;
	private int _bottomMargin;
	private float _yCoord; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate
	private float _heightRowHeader;
	private List<float> _listHeightRows;
	private List<float> _listWidthCols;
	private float _widthDataGridView;
	// Maintain a generic list to hold start/stop points for the column printing
	// This will be used for wrapping in situations where the DataGridView will not fit on a single page
	private List<int[]> _listIntArraysColumnPoints;
	private List<float> _listWidthColumnPoints;
	private int _columnPoint;

	// The class constructor
	public DataGridViewPrinter(DataGridView aDataGridView,PrintDocument aPrintDocument,bool CenterOnPage,bool WithTitle,string aTitleText,Font aTitleFont,Color aTitleColor,bool WithPaging) {
		_dataGridView = aDataGridView;
		_printDocument = aPrintDocument;
		_isCenterOnPage = CenterOnPage;
		_doesContainTitle = WithTitle;
		_title = aTitleText;
		_fontTitle = aTitleFont;
		_colorTitle = aTitleColor;
		_doesUsePaging = WithPaging;
		_pageNumber = 0;
		_listHeightRows = new List<float>();
		_listWidthCols = new List<float>();
		_listIntArraysColumnPoints = new List<int[]>();
		_listWidthColumnPoints = new List<float>();
		// Claculating the PageWidth and the PageHeight
		if(!_printDocument.DefaultPageSettings.Landscape) {
			_widthPage = _printDocument.DefaultPageSettings.PaperSize.Width;
			_heightPage = _printDocument.DefaultPageSettings.PaperSize.Height;
		}
		else {
			_heightPage = _printDocument.DefaultPageSettings.PaperSize.Width;
			_widthPage = _printDocument.DefaultPageSettings.PaperSize.Height;
		}
		// Claculating the page margins
		_leftMargin = _printDocument.DefaultPageSettings.Margins.Left;
		_topMargin = _printDocument.DefaultPageSettings.Margins.Top;
		_rightMargin = _printDocument.DefaultPageSettings.Margins.Right;
		_bottomMargin = _printDocument.DefaultPageSettings.Margins.Bottom;
		// First, the current row to be printed is the first row in the DataGridView control
		_currentRow = 0;
	}

	// The function that calculate the height of each row (including the header row), the width of each column (according to the longest text in all its cells including the header cell), and the whole DataGridView width
	private void Calculate(Graphics g) {
		if(_pageNumber == 0) // Just calculate once
		{
			SizeF tmpSize = new SizeF();
			Font tmpFont;
			float tmpWidth;
			_widthDataGridView = 0;
			for(int i = 0;i < _dataGridView.Columns.Count;i++) {
				tmpFont = _dataGridView.ColumnHeadersDefaultCellStyle.Font;
				if(tmpFont == null){ // If there is no special HeaderFont style, then use the default DataGridView font style
					tmpFont = _dataGridView.DefaultCellStyle.Font;
				}
				tmpSize = g.MeasureString(_dataGridView.Columns[i].HeaderText,tmpFont);
				tmpWidth = tmpSize.Width;
				_heightRowHeader = tmpSize.Height;
				for(int j = 0;j < _dataGridView.Rows.Count;j++) {
					tmpFont = _dataGridView.Rows[j].DefaultCellStyle.Font;
					if(tmpFont == null){ // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
						tmpFont = _dataGridView.DefaultCellStyle.Font;
					}
					tmpSize = g.MeasureString("Anything",tmpFont);
					_listHeightRows.Add(tmpSize.Height);
					tmpSize = g.MeasureString(_dataGridView.Rows[j].Cells[i].EditedFormattedValue.ToString(),tmpFont);
					if(tmpSize.Width > tmpWidth){
						tmpWidth = tmpSize.Width;
					}
				}
				if(_dataGridView.Columns[i].Visible){
					_widthDataGridView += tmpWidth;
				}
				_listWidthCols.Add(tmpWidth);
			}
			// Define the start/stop column points based on the page width and the DataGridView Width
			// We will use this to determine the columns which are drawn on each page and how wrapping will be handled
			// By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
			int k;
			int mStartPoint = 0;
			for(k = 0;k < _dataGridView.Columns.Count;k++){
				if(_dataGridView.Columns[k].Visible) {
					mStartPoint = k;
					break;
				}
			}
			int mEndPoint = _dataGridView.Columns.Count;
			for(k = _dataGridView.Columns.Count - 1;k >= 0;k--){
				if(_dataGridView.Columns[k].Visible) {
					mEndPoint = k + 1;
					break;
				}
			}
			float mTempWidth = _widthDataGridView;
			float mTempPrintArea = (float)_widthPage - (float)_leftMargin - (float)_rightMargin;
			// We only care about handling where the total datagridview width is bigger then the print area
			if(_widthDataGridView > mTempPrintArea) {
				mTempWidth = 0.0F;
				for(k = 0;k < _dataGridView.Columns.Count;k++) {
					if(_dataGridView.Columns[k].Visible) {
						mTempWidth += _listWidthCols[k];
						// If the width is bigger than the page area, then define a new column print range
						if(mTempWidth > mTempPrintArea) {
							mTempWidth -= _listWidthCols[k];
							_listIntArraysColumnPoints.Add(new int[] { mStartPoint,mEndPoint });
							_listWidthColumnPoints.Add(mTempWidth);
							mStartPoint = k;
							mTempWidth = _listWidthCols[k];
						}
					}
					// Our end point is actually one index above the current index
					mEndPoint = k + 1;
				}
			}
			// Add the last set of columns
			_listIntArraysColumnPoints.Add(new int[] { mStartPoint,mEndPoint });
			_listWidthColumnPoints.Add(mTempWidth);
			_columnPoint = 0;
		}
	}

	// The funtion that print the title, page number, and the header row
	private void DrawHeader(Graphics g) {
		_yCoord = (float)_topMargin;
		// Printing the page number (if isWithPaging is set to true)
		if(_doesUsePaging) {
			_pageNumber++;
			string PageString = "Page " + _pageNumber.ToString();
			StringFormat PageStringFormat = new StringFormat();
			PageStringFormat.Trimming = StringTrimming.Word;
			PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			PageStringFormat.Alignment = StringAlignment.Far;
			Font PageStringFont = new Font("Tahoma",8,FontStyle.Regular,GraphicsUnit.Point);
			RectangleF PageStringRectangle = new RectangleF((float)_leftMargin,_yCoord,(float)_widthPage - (float)_rightMargin - (float)_leftMargin,g.MeasureString(PageString,PageStringFont).Height);
			g.DrawString(PageString,PageStringFont,new SolidBrush(Color.Black),PageStringRectangle,PageStringFormat);
			_yCoord += g.MeasureString(PageString,PageStringFont).Height;
		}
		// Printing the title (if IsWithTitle is set to true)
		if(_doesContainTitle) {
			StringFormat TitleFormat = new StringFormat();
			TitleFormat.Trimming = StringTrimming.Word;
			TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			if(_isCenterOnPage){
				TitleFormat.Alignment = StringAlignment.Center;
			}
			else{
				TitleFormat.Alignment = StringAlignment.Near;
			}
			RectangleF TitleRectangle = new RectangleF((float)_leftMargin,_yCoord,(float)_widthPage - (float)_rightMargin - (float)_leftMargin,g.MeasureString(_title,_fontTitle).Height);
			g.DrawString(_title,_fontTitle,new SolidBrush(_colorTitle),TitleRectangle,TitleFormat);
			_yCoord += g.MeasureString(_title,_fontTitle).Height;
		}
		// Calculating the starting x coordinate that the printing process will start from
		float CurrentX = (float)_leftMargin;
		if(_isCenterOnPage){
			CurrentX += (((float)_widthPage - (float)_rightMargin - (float)_leftMargin) - _listWidthColumnPoints[_columnPoint]) / 2.0F;
		}
		// Setting the HeaderFore style
		Color HeaderForeColor = _dataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
		if(HeaderForeColor.IsEmpty){ // If there is no special HeaderFore style, then use the default DataGridView style
			HeaderForeColor = _dataGridView.DefaultCellStyle.ForeColor;
		}
		SolidBrush HeaderForeBrush = new SolidBrush(HeaderForeColor);
		// Setting the HeaderBack style
		Color HeaderBackColor = _dataGridView.ColumnHeadersDefaultCellStyle.BackColor;
		if(HeaderBackColor.IsEmpty){ // If there is no special HeaderBack style, then use the default DataGridView style
			HeaderBackColor = _dataGridView.DefaultCellStyle.BackColor;
		}
		SolidBrush HeaderBackBrush = new SolidBrush(HeaderBackColor);
		// Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
		Pen TheLinePen = new Pen(_dataGridView.GridColor,1);
		// Setting the HeaderFont style
		Font HeaderFont = _dataGridView.ColumnHeadersDefaultCellStyle.Font;
		if(HeaderFont == null){ // If there is no special HeaderFont style, then use the default DataGridView font style
			HeaderFont = _dataGridView.DefaultCellStyle.Font;
		}
		// Calculating and drawing the HeaderBounds        
		RectangleF HeaderBounds = new RectangleF(CurrentX,_yCoord,_listWidthColumnPoints[_columnPoint],_heightRowHeader);
		g.FillRectangle(HeaderBackBrush,HeaderBounds);
		// Setting the format that will be used to print each cell of the header row
		StringFormat CellFormat = new StringFormat();
		CellFormat.Trimming = StringTrimming.Word;
		CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
		// Printing each visible cell of the header row
		RectangleF CellBounds;
		float ColumnWidth;
		for(int i = (int)_listIntArraysColumnPoints[_columnPoint].GetValue(0);i < (int)_listIntArraysColumnPoints[_columnPoint].GetValue(1);i++) {
			if(!_dataGridView.Columns[i].Visible) continue; // If the column is not visible then ignore this iteration
			ColumnWidth = _listWidthCols[i];
			// Check the CurrentCell alignment and apply it to the CellFormat
			if(_dataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
				CellFormat.Alignment = StringAlignment.Far;
			else if(_dataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
				CellFormat.Alignment = StringAlignment.Center;
			else
				CellFormat.Alignment = StringAlignment.Near;
			CellBounds = new RectangleF(CurrentX,_yCoord,ColumnWidth,_heightRowHeader);
			// Printing the cell text
			g.DrawString(_dataGridView.Columns[i].HeaderText,HeaderFont,HeaderForeBrush,CellBounds,CellFormat);
			// Drawing the cell bounds
			if(_dataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None) // Draw the cell border only if the HeaderBorderStyle is not None
				g.DrawRectangle(TheLinePen,CurrentX,_yCoord,ColumnWidth,_heightRowHeader);
			CurrentX += ColumnWidth;
		}
		_yCoord += _heightRowHeader;
	}

	// The function that print a bunch of rows that fit in one page
	// When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
	// When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
	private bool DrawRows(Graphics g) {
		// Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
		Pen TheLinePen = new Pen(_dataGridView.GridColor,1);
		// The style paramters that will be used to print each cell
		Font RowFont;
		Color RowForeColor;
		Color RowBackColor;
		SolidBrush RowForeBrush;
		SolidBrush RowBackBrush;
		SolidBrush RowAlternatingBackBrush;
		// Setting the format that will be used to print each cell
		StringFormat CellFormat = new StringFormat();
		CellFormat.Trimming = StringTrimming.Word;
		CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
		// Printing each visible cell
		RectangleF RowBounds;
		float CurrentX;
		float ColumnWidth;
		while(_currentRow < _dataGridView.Rows.Count) {
			if(_dataGridView.Rows[_currentRow].Visible) // Print the cells of the CurrentRow only if that row is visible
						{
				// Setting the row font style
				RowFont = _dataGridView.Rows[_currentRow].DefaultCellStyle.Font;
				if(RowFont == null) // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
					RowFont = _dataGridView.DefaultCellStyle.Font;

				// Setting the RowFore style
				RowForeColor = _dataGridView.Rows[_currentRow].DefaultCellStyle.ForeColor;
				if(RowForeColor.IsEmpty) // If the there is no special RowFore style of the CurrentRow, then use the default one associated with the DataGridView control
					RowForeColor = _dataGridView.DefaultCellStyle.ForeColor;
				RowForeBrush = new SolidBrush(RowForeColor);

				// Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
				RowBackColor = _dataGridView.Rows[_currentRow].DefaultCellStyle.BackColor;
				if(RowBackColor.IsEmpty) // If the there is no special RowBack style of the CurrentRow, then use the default one associated with the DataGridView control
								{
					RowBackBrush = new SolidBrush(_dataGridView.DefaultCellStyle.BackColor);
					RowAlternatingBackBrush = new SolidBrush(_dataGridView.AlternatingRowsDefaultCellStyle.BackColor);
				}
				else // If the there is a special RowBack style of the CurrentRow, then use it for both the RowBack and the RowAlternatingBack styles
								{
					RowBackBrush = new SolidBrush(RowBackColor);
					RowAlternatingBackBrush = new SolidBrush(RowBackColor);
				}

				// Calculating the starting x coordinate that the printing process will start from
				CurrentX = (float)_leftMargin;
				if(_isCenterOnPage)
					CurrentX += (((float)_widthPage - (float)_rightMargin - (float)_leftMargin) - _listWidthColumnPoints[_columnPoint]) / 2.0F;

				// Calculating the entire CurrentRow bounds                
				RowBounds = new RectangleF(CurrentX,_yCoord,_listWidthColumnPoints[_columnPoint],_listHeightRows[_currentRow]);

				// Filling the back of the CurrentRow
				if(_currentRow % 2 == 0)
					g.FillRectangle(RowBackBrush,RowBounds);
				else
					g.FillRectangle(RowAlternatingBackBrush,RowBounds);

				// Printing each visible cell of the CurrentRow                
				for(int CurrentCell = (int)_listIntArraysColumnPoints[_columnPoint].GetValue(0);CurrentCell < (int)_listIntArraysColumnPoints[_columnPoint].GetValue(1);CurrentCell++) {
					if(!_dataGridView.Columns[CurrentCell].Visible) continue; // If the cell is belong to invisible column, then ignore this iteration

					// Check the CurrentCell alignment and apply it to the CellFormat
					if(_dataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Right"))
						CellFormat.Alignment = StringAlignment.Far;
					else if(_dataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Center"))
						CellFormat.Alignment = StringAlignment.Center;
					else
						CellFormat.Alignment = StringAlignment.Near;

					ColumnWidth = _listWidthCols[CurrentCell];
					RectangleF CellBounds = new RectangleF(CurrentX,_yCoord,ColumnWidth,_listHeightRows[_currentRow]);

					// Printing the cell text
					g.DrawString(_dataGridView.Rows[_currentRow].Cells[CurrentCell].EditedFormattedValue.ToString(),RowFont,RowForeBrush,CellBounds,CellFormat);

					// Drawing the cell bounds
					if(_dataGridView.CellBorderStyle != DataGridViewCellBorderStyle.None) // Draw the cell border only if the CellBorderStyle is not None
						g.DrawRectangle(TheLinePen,CurrentX,_yCoord,ColumnWidth,_listHeightRows[_currentRow]);

					CurrentX += ColumnWidth;
				}
				_yCoord += _listHeightRows[_currentRow];

				// Checking if the CurrentY is exceeds the page boundries
				// If so then exit the function and returning true meaning another PagePrint action is required
				if((int)_yCoord > (_heightPage - _topMargin - _bottomMargin)) {
					_currentRow++;
					return true;
				}
			}
			_currentRow++;
		}
		_currentRow = 0;
		_columnPoint++; // Continue to print the next group of columns
		if(_columnPoint == _listIntArraysColumnPoints.Count) // Which means all columns are printed
		{
			_columnPoint = 0;
			return false;
		}
		else
			return true;
	}

	// The method that calls all other functions
	public bool DrawDataGridView(Graphics g) {
		try {
			Calculate(g);
			DrawHeader(g);
			bool bContinue = DrawRows(g);
			return bContinue;
		}
		catch(Exception ex) {
			MessageBox.Show("Operation failed: " + ex.Message.ToString(),Application.ProductName + " - Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			return false;
		}
	}
}
