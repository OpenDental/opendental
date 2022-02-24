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
		long PatID;
		XmlDocument doc;
		DataSet DsEligResponse = new DataSet();
		DataTable DtResponse = new DataTable("EligibilityResponse");
		DataGridViewPrinter MyDataGridViewPrinter;

		///<summary></summary>
		public FormEligibilityResponseDisplay(XmlDocument Request,long PatientID) {
			doc = Request;
			PatID = PatientID;
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
			this.LblPatientName.Text=Patients.GetEligibilityDisplayName(PatID);
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
			DtResponse.Columns.Add(colBenefitInfo);
			DtResponse.Columns.Add(colCoverageLevelCode);
			DtResponse.Columns.Add(colPlanCoverageDesc);
			DtResponse.Columns.Add(colTimeQualifier);
			DtResponse.Columns.Add(colBenefitAmount);
			DtResponse.Columns.Add(colMessage);
			DtResponse.Columns.Add(colPercent);
			DtResponse.Columns.Add(colProcCode);
			DtResponse.Columns.Add(colDeliveryPattern);
			// Add Tabl to DataSet
			DsEligResponse.Tables.Add(DtResponse);
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
			PlanBenefits = doc.SelectNodes("EligBenefitResponse/BenefitResponse/PlanBenefit");
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
				DataRow DrResponse = DtResponse.NewRow();
				DrResponse["Benefit Info"] = BenefitInfo;
				DrResponse["Coverage Level"] = CoverageLevelCode;
				DrResponse["Plan Coverage"] = PlanCoverageDesc;
				DrResponse["Time Qualifier"] = TimeQualifier;
				DrResponse["Benefit Amount"] = BenefitAmount;
				DrResponse["Message"] = Message;
				DrResponse["Percent"] = Percent;
				DrResponse["ProcCode"] = ProcCode;
				DrResponse["Delivery Pattern"] = DeliveryPattern;
				DtResponse.Rows.Add(DrResponse);
			}
		}

		private void displayData() {
			this.dataGridView1.DataSource = DsEligResponse;
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
			MyDataGridViewPrinter = new DataGridViewPrinter(this.dataGridView1,MyPrintDocument,true,true,this.LblPatientName.Text,new Font("Tahoma",18,FontStyle.Bold,GraphicsUnit.Point),Color.Black,true);
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
			bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
			if(more == true){
				e.HasMorePages = true;
			}
		}

	}
}

class DataGridViewPrinter {
	private DataGridView TheDataGridView; // The DataGridView Control which will be printed
	private PrintDocument ThePrintDocument; // The PrintDocument to be used for printing
	private bool IsCenterOnPage; // Determine if the report will be printed in the Top-Center of the page
	private bool IsWithTitle; // Determine if the page contain title text
	private string TheTitleText; // The title text to be printed in each page (if IsWithTitle is set to true)
	private Font TheTitleFont; // The font to be used with the title text (if IsWithTitle is set to true)
	private Color TheTitleColor; // The color to be used with the title text (if IsWithTitle is set to true)
	private bool IsWithPaging; // Determine if paging is used
	static int CurrentRow; // A static parameter that keep track on which Row (in the DataGridView control) that should be printed
	static int PageNumber;
	private int PageWidth;
	private int PageHeight;
	private int LeftMargin;
	private int TopMargin;
	private int RightMargin;
	private int BottomMargin;
	private float CurrentY; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate
	private float RowHeaderHeight;
	private List<float> RowsHeight;
	private List<float> ColumnsWidth;
	private float TheDataGridViewWidth;
	// Maintain a generic list to hold start/stop points for the column printing
	// This will be used for wrapping in situations where the DataGridView will not fit on a single page
	private List<int[]> mColumnPoints;
	private List<float> mColumnPointsWidth;
	private int mColumnPoint;

	// The class constructor
	public DataGridViewPrinter(DataGridView aDataGridView,PrintDocument aPrintDocument,bool CenterOnPage,bool WithTitle,string aTitleText,Font aTitleFont,Color aTitleColor,bool WithPaging) {
		TheDataGridView = aDataGridView;
		ThePrintDocument = aPrintDocument;
		IsCenterOnPage = CenterOnPage;
		IsWithTitle = WithTitle;
		TheTitleText = aTitleText;
		TheTitleFont = aTitleFont;
		TheTitleColor = aTitleColor;
		IsWithPaging = WithPaging;
		PageNumber = 0;
		RowsHeight = new List<float>();
		ColumnsWidth = new List<float>();
		mColumnPoints = new List<int[]>();
		mColumnPointsWidth = new List<float>();
		// Claculating the PageWidth and the PageHeight
		if(!ThePrintDocument.DefaultPageSettings.Landscape) {
			PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
			PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
		}
		else {
			PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
			PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
		}
		// Claculating the page margins
		LeftMargin = ThePrintDocument.DefaultPageSettings.Margins.Left;
		TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
		RightMargin = ThePrintDocument.DefaultPageSettings.Margins.Right;
		BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;
		// First, the current row to be printed is the first row in the DataGridView control
		CurrentRow = 0;
	}

	// The function that calculate the height of each row (including the header row), the width of each column (according to the longest text in all its cells including the header cell), and the whole DataGridView width
	private void Calculate(Graphics g) {
		if(PageNumber == 0) // Just calculate once
		{
			SizeF tmpSize = new SizeF();
			Font tmpFont;
			float tmpWidth;
			TheDataGridViewWidth = 0;
			for(int i = 0;i < TheDataGridView.Columns.Count;i++) {
				tmpFont = TheDataGridView.ColumnHeadersDefaultCellStyle.Font;
				if(tmpFont == null){ // If there is no special HeaderFont style, then use the default DataGridView font style
					tmpFont = TheDataGridView.DefaultCellStyle.Font;
				}
				tmpSize = g.MeasureString(TheDataGridView.Columns[i].HeaderText,tmpFont);
				tmpWidth = tmpSize.Width;
				RowHeaderHeight = tmpSize.Height;
				for(int j = 0;j < TheDataGridView.Rows.Count;j++) {
					tmpFont = TheDataGridView.Rows[j].DefaultCellStyle.Font;
					if(tmpFont == null){ // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
						tmpFont = TheDataGridView.DefaultCellStyle.Font;
					}
					tmpSize = g.MeasureString("Anything",tmpFont);
					RowsHeight.Add(tmpSize.Height);
					tmpSize = g.MeasureString(TheDataGridView.Rows[j].Cells[i].EditedFormattedValue.ToString(),tmpFont);
					if(tmpSize.Width > tmpWidth){
						tmpWidth = tmpSize.Width;
					}
				}
				if(TheDataGridView.Columns[i].Visible){
					TheDataGridViewWidth += tmpWidth;
				}
				ColumnsWidth.Add(tmpWidth);
			}
			// Define the start/stop column points based on the page width and the DataGridView Width
			// We will use this to determine the columns which are drawn on each page and how wrapping will be handled
			// By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
			int k;
			int mStartPoint = 0;
			for(k = 0;k < TheDataGridView.Columns.Count;k++){
				if(TheDataGridView.Columns[k].Visible) {
					mStartPoint = k;
					break;
				}
			}
			int mEndPoint = TheDataGridView.Columns.Count;
			for(k = TheDataGridView.Columns.Count - 1;k >= 0;k--){
				if(TheDataGridView.Columns[k].Visible) {
					mEndPoint = k + 1;
					break;
				}
			}
			float mTempWidth = TheDataGridViewWidth;
			float mTempPrintArea = (float)PageWidth - (float)LeftMargin - (float)RightMargin;
			// We only care about handling where the total datagridview width is bigger then the print area
			if(TheDataGridViewWidth > mTempPrintArea) {
				mTempWidth = 0.0F;
				for(k = 0;k < TheDataGridView.Columns.Count;k++) {
					if(TheDataGridView.Columns[k].Visible) {
						mTempWidth += ColumnsWidth[k];
						// If the width is bigger than the page area, then define a new column print range
						if(mTempWidth > mTempPrintArea) {
							mTempWidth -= ColumnsWidth[k];
							mColumnPoints.Add(new int[] { mStartPoint,mEndPoint });
							mColumnPointsWidth.Add(mTempWidth);
							mStartPoint = k;
							mTempWidth = ColumnsWidth[k];
						}
					}
					// Our end point is actually one index above the current index
					mEndPoint = k + 1;
				}
			}
			// Add the last set of columns
			mColumnPoints.Add(new int[] { mStartPoint,mEndPoint });
			mColumnPointsWidth.Add(mTempWidth);
			mColumnPoint = 0;
		}
	}

	// The funtion that print the title, page number, and the header row
	private void DrawHeader(Graphics g) {
		CurrentY = (float)TopMargin;
		// Printing the page number (if isWithPaging is set to true)
		if(IsWithPaging) {
			PageNumber++;
			string PageString = "Page " + PageNumber.ToString();
			StringFormat PageStringFormat = new StringFormat();
			PageStringFormat.Trimming = StringTrimming.Word;
			PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			PageStringFormat.Alignment = StringAlignment.Far;
			Font PageStringFont = new Font("Tahoma",8,FontStyle.Regular,GraphicsUnit.Point);
			RectangleF PageStringRectangle = new RectangleF((float)LeftMargin,CurrentY,(float)PageWidth - (float)RightMargin - (float)LeftMargin,g.MeasureString(PageString,PageStringFont).Height);
			g.DrawString(PageString,PageStringFont,new SolidBrush(Color.Black),PageStringRectangle,PageStringFormat);
			CurrentY += g.MeasureString(PageString,PageStringFont).Height;
		}
		// Printing the title (if IsWithTitle is set to true)
		if(IsWithTitle) {
			StringFormat TitleFormat = new StringFormat();
			TitleFormat.Trimming = StringTrimming.Word;
			TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			if(IsCenterOnPage){
				TitleFormat.Alignment = StringAlignment.Center;
			}
			else{
				TitleFormat.Alignment = StringAlignment.Near;
			}
			RectangleF TitleRectangle = new RectangleF((float)LeftMargin,CurrentY,(float)PageWidth - (float)RightMargin - (float)LeftMargin,g.MeasureString(TheTitleText,TheTitleFont).Height);
			g.DrawString(TheTitleText,TheTitleFont,new SolidBrush(TheTitleColor),TitleRectangle,TitleFormat);
			CurrentY += g.MeasureString(TheTitleText,TheTitleFont).Height;
		}
		// Calculating the starting x coordinate that the printing process will start from
		float CurrentX = (float)LeftMargin;
		if(IsCenterOnPage){
			CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;
		}
		// Setting the HeaderFore style
		Color HeaderForeColor = TheDataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
		if(HeaderForeColor.IsEmpty){ // If there is no special HeaderFore style, then use the default DataGridView style
			HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
		}
		SolidBrush HeaderForeBrush = new SolidBrush(HeaderForeColor);
		// Setting the HeaderBack style
		Color HeaderBackColor = TheDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
		if(HeaderBackColor.IsEmpty){ // If there is no special HeaderBack style, then use the default DataGridView style
			HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor;
		}
		SolidBrush HeaderBackBrush = new SolidBrush(HeaderBackColor);
		// Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
		Pen TheLinePen = new Pen(TheDataGridView.GridColor,1);
		// Setting the HeaderFont style
		Font HeaderFont = TheDataGridView.ColumnHeadersDefaultCellStyle.Font;
		if(HeaderFont == null){ // If there is no special HeaderFont style, then use the default DataGridView font style
			HeaderFont = TheDataGridView.DefaultCellStyle.Font;
		}
		// Calculating and drawing the HeaderBounds        
		RectangleF HeaderBounds = new RectangleF(CurrentX,CurrentY,mColumnPointsWidth[mColumnPoint],RowHeaderHeight);
		g.FillRectangle(HeaderBackBrush,HeaderBounds);
		// Setting the format that will be used to print each cell of the header row
		StringFormat CellFormat = new StringFormat();
		CellFormat.Trimming = StringTrimming.Word;
		CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
		// Printing each visible cell of the header row
		RectangleF CellBounds;
		float ColumnWidth;
		for(int i = (int)mColumnPoints[mColumnPoint].GetValue(0);i < (int)mColumnPoints[mColumnPoint].GetValue(1);i++) {
			if(!TheDataGridView.Columns[i].Visible) continue; // If the column is not visible then ignore this iteration
			ColumnWidth = ColumnsWidth[i];
			// Check the CurrentCell alignment and apply it to the CellFormat
			if(TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
				CellFormat.Alignment = StringAlignment.Far;
			else if(TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
				CellFormat.Alignment = StringAlignment.Center;
			else
				CellFormat.Alignment = StringAlignment.Near;
			CellBounds = new RectangleF(CurrentX,CurrentY,ColumnWidth,RowHeaderHeight);
			// Printing the cell text
			g.DrawString(TheDataGridView.Columns[i].HeaderText,HeaderFont,HeaderForeBrush,CellBounds,CellFormat);
			// Drawing the cell bounds
			if(TheDataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None) // Draw the cell border only if the HeaderBorderStyle is not None
				g.DrawRectangle(TheLinePen,CurrentX,CurrentY,ColumnWidth,RowHeaderHeight);
			CurrentX += ColumnWidth;
		}
		CurrentY += RowHeaderHeight;
	}

	// The function that print a bunch of rows that fit in one page
	// When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
	// When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
	private bool DrawRows(Graphics g) {
		// Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
		Pen TheLinePen = new Pen(TheDataGridView.GridColor,1);
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
		while(CurrentRow < TheDataGridView.Rows.Count) {
			if(TheDataGridView.Rows[CurrentRow].Visible) // Print the cells of the CurrentRow only if that row is visible
						{
				// Setting the row font style
				RowFont = TheDataGridView.Rows[CurrentRow].DefaultCellStyle.Font;
				if(RowFont == null) // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
					RowFont = TheDataGridView.DefaultCellStyle.Font;

				// Setting the RowFore style
				RowForeColor = TheDataGridView.Rows[CurrentRow].DefaultCellStyle.ForeColor;
				if(RowForeColor.IsEmpty) // If the there is no special RowFore style of the CurrentRow, then use the default one associated with the DataGridView control
					RowForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
				RowForeBrush = new SolidBrush(RowForeColor);

				// Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
				RowBackColor = TheDataGridView.Rows[CurrentRow].DefaultCellStyle.BackColor;
				if(RowBackColor.IsEmpty) // If the there is no special RowBack style of the CurrentRow, then use the default one associated with the DataGridView control
								{
					RowBackBrush = new SolidBrush(TheDataGridView.DefaultCellStyle.BackColor);
					RowAlternatingBackBrush = new SolidBrush(TheDataGridView.AlternatingRowsDefaultCellStyle.BackColor);
				}
				else // If the there is a special RowBack style of the CurrentRow, then use it for both the RowBack and the RowAlternatingBack styles
								{
					RowBackBrush = new SolidBrush(RowBackColor);
					RowAlternatingBackBrush = new SolidBrush(RowBackColor);
				}

				// Calculating the starting x coordinate that the printing process will start from
				CurrentX = (float)LeftMargin;
				if(IsCenterOnPage)
					CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;

				// Calculating the entire CurrentRow bounds                
				RowBounds = new RectangleF(CurrentX,CurrentY,mColumnPointsWidth[mColumnPoint],RowsHeight[CurrentRow]);

				// Filling the back of the CurrentRow
				if(CurrentRow % 2 == 0)
					g.FillRectangle(RowBackBrush,RowBounds);
				else
					g.FillRectangle(RowAlternatingBackBrush,RowBounds);

				// Printing each visible cell of the CurrentRow                
				for(int CurrentCell = (int)mColumnPoints[mColumnPoint].GetValue(0);CurrentCell < (int)mColumnPoints[mColumnPoint].GetValue(1);CurrentCell++) {
					if(!TheDataGridView.Columns[CurrentCell].Visible) continue; // If the cell is belong to invisible column, then ignore this iteration

					// Check the CurrentCell alignment and apply it to the CellFormat
					if(TheDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Right"))
						CellFormat.Alignment = StringAlignment.Far;
					else if(TheDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Center"))
						CellFormat.Alignment = StringAlignment.Center;
					else
						CellFormat.Alignment = StringAlignment.Near;

					ColumnWidth = ColumnsWidth[CurrentCell];
					RectangleF CellBounds = new RectangleF(CurrentX,CurrentY,ColumnWidth,RowsHeight[CurrentRow]);

					// Printing the cell text
					g.DrawString(TheDataGridView.Rows[CurrentRow].Cells[CurrentCell].EditedFormattedValue.ToString(),RowFont,RowForeBrush,CellBounds,CellFormat);

					// Drawing the cell bounds
					if(TheDataGridView.CellBorderStyle != DataGridViewCellBorderStyle.None) // Draw the cell border only if the CellBorderStyle is not None
						g.DrawRectangle(TheLinePen,CurrentX,CurrentY,ColumnWidth,RowsHeight[CurrentRow]);

					CurrentX += ColumnWidth;
				}
				CurrentY += RowsHeight[CurrentRow];

				// Checking if the CurrentY is exceeds the page boundries
				// If so then exit the function and returning true meaning another PagePrint action is required
				if((int)CurrentY > (PageHeight - TopMargin - BottomMargin)) {
					CurrentRow++;
					return true;
				}
			}
			CurrentRow++;
		}
		CurrentRow = 0;
		mColumnPoint++; // Continue to print the next group of columns
		if(mColumnPoint == mColumnPoints.Count) // Which means all columns are printed
		{
			mColumnPoint = 0;
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
