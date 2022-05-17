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
		public FormEligibilityResponseDisplay(XmlDocument xmlDocument,long patNum) {
			_xmlDocument = xmlDocument;
			_patNum = patNum;
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
			DataColumn dataColumnBenefitInfo = new DataColumn("Benefit Info",System.Type.GetType("System.String"));
			DataColumn dataColumnCoverageLevel = new DataColumn("Coverage Level",System.Type.GetType("System.String"));
			DataColumn dataColumnPlanCoverage = new DataColumn("Plan Coverage",System.Type.GetType("System.String"));
			DataColumn dataColumnTimeQualifier = new DataColumn("Time Qualifier",System.Type.GetType("System.String"));
			DataColumn dataColumnBenefitAmt = new DataColumn("Benefit Amount",System.Type.GetType("System.String"));
			DataColumn dataColumnMsg = new DataColumn("Message",System.Type.GetType("System.String"));
			DataColumn dataColumnPercent = new DataColumn("Percent",System.Type.GetType("System.String"));
			DataColumn dataColumnProcCode = new DataColumn("ProcCode",System.Type.GetType("System.String"));
			DataColumn dataColumnDeliveryPattern = new DataColumn("Delivery Pattern",System.Type.GetType("System.String"));
			// Add Columns to Data Table
			_table.Columns.Add(dataColumnBenefitInfo);
			_table.Columns.Add(dataColumnCoverageLevel);
			_table.Columns.Add(dataColumnPlanCoverage);
			_table.Columns.Add(dataColumnTimeQualifier);
			_table.Columns.Add(dataColumnBenefitAmt);
			_table.Columns.Add(dataColumnMsg);
			_table.Columns.Add(dataColumnPercent);
			_table.Columns.Add(dataColumnProcCode);
			_table.Columns.Add(dataColumnDeliveryPattern);
			// Add Tabl to DataSet
			_dataSet.Tables.Add(_table);
		}

		private void populateDataTable() {
			XmlNodeList xmlNodeListBenefits;
			XmlNodeList xmlNodeListChildNodes;
			// Declare Variable for each table column
			string benefitInfo;
			string coverageLevelCode;
			string planCoverageDesc;
			string timeQualifier;
			string benefitAmount;
			string message;
			string percent;
			string procCode;
			string deliveryPattern;
			// select all Plan Benefit Nodes
			xmlNodeListBenefits = _xmlDocument.SelectNodes("EligBenefitResponse/BenefitResponse/PlanBenefit");
			for(int i=0;i<xmlNodeListBenefits.Count;i++) {
				xmlNodeListChildNodes=xmlNodeListBenefits[i].ChildNodes;
				// reset All Prior Column Values
				benefitInfo = string.Empty;
				coverageLevelCode = string.Empty;
				planCoverageDesc = string.Empty;
				timeQualifier = string.Empty;
				benefitAmount = string.Empty;
				message = string.Empty;
				percent = string.Empty;
				procCode = string.Empty;
				deliveryPattern = string.Empty;
				for(int c=0;c<xmlNodeListChildNodes.Count;c++) {
					// Process Each Child Node and Get the values back
					switch(xmlNodeListChildNodes[c].Name) {
						case "BenefitInfo":
							benefitInfo = xmlNodeListChildNodes[c].InnerText;
							break;
						case "CoverageLevelCode":
							coverageLevelCode = xmlNodeListChildNodes[c].InnerText;
							break;
						case "PlanCoverageDesc":
							planCoverageDesc = xmlNodeListChildNodes[c].InnerText;
							break;
						case "TimeQualifier":
							timeQualifier = xmlNodeListChildNodes[c].InnerText;
							break;
						case "BenefitAmount":
							benefitAmount = "$" + xmlNodeListChildNodes[c].InnerText;
							break;
						case "Message":
							if(xmlNodeListChildNodes[c].InnerText.Substring(0,3) != "URL") {
								message = xmlNodeListChildNodes[c].InnerText;
							}
							break;
						case "Percent":
							try {
								percent = System.Convert.ToString(System.Convert.ToDecimal(xmlNodeListChildNodes[c].InnerText) * 100) + "%";
							}
							catch(Exception) {
								percent = "??";
							}
							break;
						case "Procedure":
							procCode = xmlNodeListChildNodes[c].ChildNodes.Item(1).InnerText;
							break;
						case "DeliveryPattern":
							deliveryPattern = xmlNodeListChildNodes[c].ChildNodes.Item(0).InnerText + "-" + xmlNodeListChildNodes[c].ChildNodes.Item(1).InnerText;
							break;
					}
				}
				// Insert Record Into DataTable
				DataRow dataRowResponse = _table.NewRow();
				dataRowResponse["Benefit Info"] = benefitInfo;
				dataRowResponse["Coverage Level"] = coverageLevelCode;
				dataRowResponse["Plan Coverage"] = planCoverageDesc;
				dataRowResponse["Time Qualifier"] = timeQualifier;
				dataRowResponse["Benefit Amount"] = benefitAmount;
				dataRowResponse["Message"] = message;
				dataRowResponse["Percent"] = percent;
				dataRowResponse["ProcCode"] = procCode;
				dataRowResponse["Delivery Pattern"] = deliveryPattern;
				_table.Rows.Add(dataRowResponse);
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
			PrintDialog printDialog = new PrintDialog();
			printDialog.AllowCurrentPage = false;
			printDialog.AllowPrintToFile = false;
			printDialog.AllowSelection = false;
			printDialog.AllowSomePages = false;
			printDialog.PrintToFile = false;
			printDialog.ShowHelp = false;
			printDialog.ShowNetwork = false;
			printDialog.UseEXDialog=true;
			if(printDialog.ShowDialog() != DialogResult.OK){
				return false;
			}
			MyPrintDocument.DocumentName = "Patient Eligibility Response";
			MyPrintDocument.PrinterSettings = printDialog.PrinterSettings;
			MyPrintDocument.DefaultPageSettings = printDialog.PrinterSettings.DefaultPageSettings;
			MyPrintDocument.DefaultPageSettings.Margins = new Margins(40,40,40,40);
			using Font font = new Font("Tahoma",18,FontStyle.Bold,GraphicsUnit.Point);
			_dataGridViewPrinter = new DataGridViewPrinter(this.dataGridView1,MyPrintDocument,isCentered:true,hasTitle:true,this.LblPatientName.Text,font,Color.Black,doesUsePaging:true);
			return true;
		}

		private void btnPrintPreview_Click(object sender,EventArgs e) {
			if(!SetupThePrinting()) { 
				return; 
			}
			PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
			printPreviewDialog.Document = MyPrintDocument;
			printPreviewDialog.PrintPreviewControl.Zoom = 1.0;
			((Form)printPreviewDialog).WindowState = FormWindowState.Maximized;
			printPreviewDialog.ShowDialog();
		}

		private void MyPrintDocument_PrintPage(object sender,PrintPageEventArgs e) {
			bool hasMore = _dataGridViewPrinter.DrawDataGridView(e.Graphics);
			if(hasMore == true){
				e.HasMorePages = true;
			}
		}

	}
}

class DataGridViewPrinter {
	private DataGridView _dataGridView; // The DataGridView Control which will be printed
	private PrintDocument _printDocument; // The PrintDocument to be used for printing
	private bool _isCentered; // Determine if the report will be printed in the Top-Center of the page
	private bool _hasTitle; // Determine if the page contain title text
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
	public DataGridViewPrinter(DataGridView dataGridView,PrintDocument printDocument,bool isCentered,bool hasTitle,string title,Font fontTitle,Color colorTitle,bool doesUsePaging) {
		_dataGridView = dataGridView;
		_printDocument = printDocument;
		_isCentered = isCentered;
		_hasTitle = hasTitle;
		_title = title;
		_fontTitle = fontTitle;
		_colorTitle = colorTitle;
		_doesUsePaging = doesUsePaging;
		_pageNumber = 0;
		_listHeightRows = new List<float>();
		_listWidthCols = new List<float>();
		_listIntArraysColumnPoints = new List<int[]>();
		_listWidthColumnPoints = new List<float>();
		// Calculating the PageWidth and the PageHeight
		if(_printDocument.DefaultPageSettings.Landscape) {
			_heightPage = _printDocument.DefaultPageSettings.PaperSize.Width;
			_widthPage = _printDocument.DefaultPageSettings.PaperSize.Height;
		}
		else {
			_widthPage = _printDocument.DefaultPageSettings.PaperSize.Width;
			_heightPage = _printDocument.DefaultPageSettings.PaperSize.Height;
		}
		// Calculating the page margins
		_leftMargin = _printDocument.DefaultPageSettings.Margins.Left;
		_topMargin = _printDocument.DefaultPageSettings.Margins.Top;
		_rightMargin = _printDocument.DefaultPageSettings.Margins.Right;
		_bottomMargin = _printDocument.DefaultPageSettings.Margins.Bottom;
		// First, the current row to be printed is the first row in the DataGridView control
		_currentRow = 0;
	}

	// The function that calculate the height of each row (including the header row), the width of each column (according to the longest text in all its cells including the header cell), and the whole DataGridView width
	private void Calculate(Graphics g) {
		if(_pageNumber != 0) // Just calculate once
		{
			return;
		}
		SizeF sizeF = new SizeF();
		Font font;
		float width;
		_widthDataGridView = 0;
		for(int i = 0;i < _dataGridView.Columns.Count;i++) {
			font = _dataGridView.ColumnHeadersDefaultCellStyle.Font;
			if(font == null){ // If there is no special HeaderFont style, then use the default DataGridView font style
				font = _dataGridView.DefaultCellStyle.Font;
			}
			sizeF = g.MeasureString(_dataGridView.Columns[i].HeaderText,font);
			width = sizeF.Width;
			_heightRowHeader = sizeF.Height;
			for(int j = 0;j < _dataGridView.Rows.Count;j++) {
				font = _dataGridView.Rows[j].DefaultCellStyle.Font;
				if(font == null){ // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
					font = _dataGridView.DefaultCellStyle.Font;
				}
				sizeF = g.MeasureString("Anything",font);
				_listHeightRows.Add(sizeF.Height);
				sizeF = g.MeasureString(_dataGridView.Rows[j].Cells[i].EditedFormattedValue.ToString(),font);
				if(sizeF.Width > width){
					width = sizeF.Width;
				}
			}
			if(_dataGridView.Columns[i].Visible){
				_widthDataGridView += width;
			}
			_listWidthCols.Add(width);
		}
		// Define the start/stop column points based on the page width and the DataGridView Width
		// We will use this to determine the columns which are drawn on each page and how wrapping will be handled
		// By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
		int idx;
		int startPoint = 0;
		for(idx = 0;idx < _dataGridView.Columns.Count;idx++){
			if(_dataGridView.Columns[idx].Visible) {
				startPoint = idx;
				break;
			}
		}
		int endPoint = _dataGridView.Columns.Count;
		for(idx = _dataGridView.Columns.Count - 1;idx >= 0;idx--){
			if(_dataGridView.Columns[idx].Visible) {
				endPoint = idx + 1;
				break;
			}
		}
		float widthTemp = _widthDataGridView;
		float printArea = (float)_widthPage - (float)_leftMargin - (float)_rightMargin;
		// We only care about handling where the total datagridview width is bigger then the print area
		if(_widthDataGridView > printArea) {
			widthTemp = 0.0F;
			for(idx = 0;idx < _dataGridView.Columns.Count;idx++) {
				if(_dataGridView.Columns[idx].Visible) {
					widthTemp += _listWidthCols[idx];
					// If the width is bigger than the page area, then define a new column print range
					if(widthTemp > printArea) {
						widthTemp -= _listWidthCols[idx];
						_listIntArraysColumnPoints.Add(new int[] { startPoint,endPoint });
						_listWidthColumnPoints.Add(widthTemp);
						startPoint = idx;
						widthTemp = _listWidthCols[idx];
					}
				}
				// Our end point is actually one index above the current index
				endPoint = idx + 1;
			}
		}
		// Add the last set of columns
		_listIntArraysColumnPoints.Add(new int[] { startPoint,endPoint });
		_listWidthColumnPoints.Add(widthTemp);
		_columnPoint = 0;
	}

	// The funtion that print the title, page number, and the header row
	private void DrawHeader(Graphics g) {
		_yCoord = (float)_topMargin;
		// Printing the page number (if isWithPaging is set to true)
		if(_doesUsePaging) {
			_pageNumber++;
			string pageNumber = "Page " + _pageNumber.ToString();
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.Word;
			stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			stringFormat.Alignment = StringAlignment.Far;
			using Font font = new Font("Tahoma",8,FontStyle.Regular,GraphicsUnit.Point);
			RectangleF rectangle = new RectangleF((float)_leftMargin,_yCoord,(float)_widthPage - (float)_rightMargin - (float)_leftMargin,g.MeasureString(pageNumber,font).Height);
			g.DrawString(pageNumber,font,new SolidBrush(Color.Black),rectangle,stringFormat);
			_yCoord += g.MeasureString(pageNumber,font).Height;
		}
		// Printing the title (if IsWithTitle is set to true)
		if(_hasTitle) {
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.Word;
			stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			if(_isCentered){
				stringFormat.Alignment = StringAlignment.Center;
			}
			else{
				stringFormat.Alignment = StringAlignment.Near;
			}
			RectangleF rectangle = new RectangleF((float)_leftMargin,_yCoord,(float)_widthPage - (float)_rightMargin - (float)_leftMargin,g.MeasureString(_title,_fontTitle).Height);
			g.DrawString(_title,_fontTitle,new SolidBrush(_colorTitle),rectangle,stringFormat);
			_yCoord += g.MeasureString(_title,_fontTitle).Height;
		}
		// Calculating the starting x coordinate that the printing process will start from
		float xCoord = (float)_leftMargin;
		if(_isCentered){
			xCoord += (((float)_widthPage - (float)_rightMargin - (float)_leftMargin) - _listWidthColumnPoints[_columnPoint]) / 2.0F;
		}
		// Setting the HeaderFore style
		Color colorForeground = _dataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
		if(colorForeground.IsEmpty){ // If there is no special HeaderFore style, then use the default DataGridView style
			colorForeground = _dataGridView.DefaultCellStyle.ForeColor;
		}
		SolidBrush solidBrushFore = new SolidBrush(colorForeground);
		// Setting the HeaderBack style
		Color colorBackground = _dataGridView.ColumnHeadersDefaultCellStyle.BackColor;
		if(colorBackground.IsEmpty){ // If there is no special HeaderBack style, then use the default DataGridView style
			colorBackground = _dataGridView.DefaultCellStyle.BackColor;
		}
		SolidBrush solidBrushBackground = new SolidBrush(colorBackground);
		// Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
		using Pen pen = new Pen(_dataGridView.GridColor,1);
		// Setting the HeaderFont style
		Font fontHeader = _dataGridView.ColumnHeadersDefaultCellStyle.Font;
		if(fontHeader == null){ // If there is no special HeaderFont style, then use the default DataGridView font style
			fontHeader = _dataGridView.DefaultCellStyle.Font;
		}
		// Calculating and drawing the HeaderBounds        
		RectangleF rectangleF = new RectangleF(xCoord,_yCoord,_listWidthColumnPoints[_columnPoint],_heightRowHeader);
		g.FillRectangle(solidBrushBackground,rectangleF);
		// Setting the format that will be used to print each cell of the header row
		StringFormat stringFormatCell = new StringFormat();
		stringFormatCell.Trimming = StringTrimming.Word;
		stringFormatCell.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
		// Printing each visible cell of the header row
		RectangleF rectangleFCell;
		float width;
		for(int i = (int)_listIntArraysColumnPoints[_columnPoint].GetValue(0);i < (int)_listIntArraysColumnPoints[_columnPoint].GetValue(1);i++) {
			if(!_dataGridView.Columns[i].Visible) {
				continue; // If the column is not visible then ignore this iteration
			}
			width = _listWidthCols[i];
			// Check the CurrentCell alignment and apply it to the CellFormat
			if(_dataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right")) {
				stringFormatCell.Alignment = StringAlignment.Far;
			}
			else if(_dataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center")) {
				stringFormatCell.Alignment = StringAlignment.Center;
			}
			else {
				stringFormatCell.Alignment = StringAlignment.Near;
			}
			rectangleFCell = new RectangleF(xCoord,_yCoord,width,_heightRowHeader);
			// Printing the cell text
			g.DrawString(_dataGridView.Columns[i].HeaderText,fontHeader,solidBrushFore,rectangleFCell,stringFormatCell);
			// Drawing the cell bounds
			if(_dataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None) { // Draw the cell border only if the HeaderBorderStyle is not None
				g.DrawRectangle(pen,xCoord,_yCoord,width,_heightRowHeader);
			}
			xCoord += width;
		}
		_yCoord += _heightRowHeader;
	}

	// The function that print a bunch of rows that fit in one page
	// When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
	// When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
	private bool DrawRows(Graphics g) {
		// Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
		using Pen pen = new Pen(_dataGridView.GridColor,1);
		// The style paramters that will be used to print each cell
		Font fontRow;
		Color colorForeground;
		Color colorBackground;
		SolidBrush solidBrushForeground;
		SolidBrush solidBrushBackground;
		SolidBrush solidBrushAltBackground;
		// Setting the format that will be used to print each cell
		StringFormat stringFormat = new StringFormat();
		stringFormat.Trimming = StringTrimming.Word;
		stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
		// Printing each visible cell
		RectangleF rectangleF;
		float xCoord;
		float width;
		while(_currentRow < _dataGridView.Rows.Count) {
			if(_dataGridView.Rows[_currentRow].Visible) { // Print the cells of the CurrentRow only if that row is visible
				fontRow = _dataGridView.Rows[_currentRow].DefaultCellStyle.Font; // Setting the row font style
				if(fontRow == null) { // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
					fontRow = _dataGridView.DefaultCellStyle.Font;
				}
				// Setting the RowFore style
				colorForeground = _dataGridView.Rows[_currentRow].DefaultCellStyle.ForeColor;
				if(colorForeground.IsEmpty) { // If the there is no special RowFore style of the CurrentRow, then use the default one associated with the DataGridView control
					colorForeground = _dataGridView.DefaultCellStyle.ForeColor;
				}
				solidBrushForeground = new SolidBrush(colorForeground);
				// Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
				colorBackground = _dataGridView.Rows[_currentRow].DefaultCellStyle.BackColor;
				if(colorBackground.IsEmpty) { // If the there is no special RowBack style of the CurrentRow, then use the default one associated with the DataGridView control
					solidBrushBackground = new SolidBrush(_dataGridView.DefaultCellStyle.BackColor);
					solidBrushAltBackground = new SolidBrush(_dataGridView.AlternatingRowsDefaultCellStyle.BackColor);
				}
				else { // If the there is a special RowBack style of the CurrentRow, then use it for both the RowBack and the RowAlternatingBack styles
					solidBrushBackground = new SolidBrush(colorBackground);
					solidBrushAltBackground = new SolidBrush(colorBackground);
				}
				// Calculating the starting x coordinate that the printing process will start from
				xCoord = (float)_leftMargin;
				if(_isCentered) {
					xCoord += (((float)_widthPage - (float)_rightMargin - (float)_leftMargin) - _listWidthColumnPoints[_columnPoint]) / 2.0F;
				}
				// Calculating the entire CurrentRow bounds                
				rectangleF = new RectangleF(xCoord,_yCoord,_listWidthColumnPoints[_columnPoint],_listHeightRows[_currentRow]);
				// Filling the back of the CurrentRow
				if(_currentRow % 2 == 0) {
					g.FillRectangle(solidBrushBackground,rectangleF);
				} 
				else {
					g.FillRectangle(solidBrushAltBackground,rectangleF);
				}
				// Printing each visible cell of the CurrentRow                
				for(int c = (int)_listIntArraysColumnPoints[_columnPoint].GetValue(0);c < (int)_listIntArraysColumnPoints[_columnPoint].GetValue(1);c++) {
					if(!_dataGridView.Columns[c].Visible) { // If the cell is belong to invisible column, then ignore this iteration
						continue;
					}
					// Check the CurrentCell alignment and apply it to the CellFormat
					if(_dataGridView.Columns[c].DefaultCellStyle.Alignment.ToString().Contains("Right")) {
						stringFormat.Alignment = StringAlignment.Far;
					}
					else if(_dataGridView.Columns[c].DefaultCellStyle.Alignment.ToString().Contains("Center")) {
						stringFormat.Alignment = StringAlignment.Center;
					}
					else {
						stringFormat.Alignment = StringAlignment.Near;
					}
					width = _listWidthCols[c];
					RectangleF rectangleFCell = new RectangleF(xCoord,_yCoord,width,_listHeightRows[_currentRow]);
					// Printing the cell text
					g.DrawString(_dataGridView.Rows[_currentRow].Cells[c].EditedFormattedValue.ToString(),fontRow,solidBrushForeground,rectangleFCell,stringFormat);
					// Drawing the cell bounds
					if(_dataGridView.CellBorderStyle != DataGridViewCellBorderStyle.None) { // Draw the cell border only if the CellBorderStyle is not None
						g.DrawRectangle(pen,xCoord,_yCoord,width,_listHeightRows[_currentRow]);
					}
					xCoord += width;
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
		if(_columnPoint == _listIntArraysColumnPoints.Count) { // Which means all columns are printed
			_columnPoint = 0;
			return false;
		}
		else {
			return true;
		}
	}

	// The method that calls all other functions
	public bool DrawDataGridView(Graphics g) {
		Calculate(g);
		bool doContinue;
		try {
			DrawHeader(g);
			doContinue = DrawRows(g);
		}
		catch(Exception ex) {
			MessageBox.Show("Operation failed: " + ex.Message.ToString(),Application.ProductName + " - Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			return false;
		}
		return doContinue;
	}
}
