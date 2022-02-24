using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormMarkupTableEdit:FormODBase {
		///<summary>Both an incoming and outgoing parameter.</summary>
		public string Markup;
		private DataTable Table;
		private List<string> ColNames;
		///<summary>Widths must always be specified.  Not optional.</summary>
		private List<int> ColWidths;
		///<summary>This is passed in from the calling form.  It is used when deciding whether to allow user to add tableviews.  Blocks them if more than one table in page.</summary>
		public int CountTablesInPage;
		public bool IsNew;

		public FormMarkupTableEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMarkupTableEdit_Load(object sender,EventArgs e) {
			//strategy: when form loads, process incoming markup into column names, column widths, and a data table.
			//grid will need to be filled repeatedly, and it will pull from the above objects rather than the original markup
			//When user clicks ok, the objects are transformed back into markup.
			ParseMarkup();
			FillGrid();
		}

		///<summary>Happens on Load, and will also happen when user manually edits markup.  Recursive.</summary>
		private void ParseMarkup(){
			//{|
			//!Width="100"|Column Heading 1!!Width="150"|Column Heading 2!!Width="75"|Column Heading 3
			//|- 
			//|Cell 1||Cell 2||Cell 3 
			//|-
			//|Cell A||Cell B||Cell C 
			//|}
			Table=new DataTable();
			ColNames=new List<string>();
			ColWidths=new List<int>();
			DataRow row;
			string[] cells;
			string[] lines=Markup.Split(new string[] { "{|\r\n","{|\n","\r\n|-\r\n","\n|-\n","\r\n|}","\n|}" },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<lines.Length;i++) {
				if(lines[i].StartsWith("!")) {//header
					lines[i]=lines[i].Substring(1);//strips off the leading !
					cells=lines[i].Split(new string[] { "!!" },StringSplitOptions.None);
					for(int c=0;c<cells.Length;c++) {
						string colName="";
						if(!Regex.IsMatch(cells[c],@"^(Width="")\d+""\|")) {//e.g. Width="90"| 
							MessageBox.Show("Table is corrupt.  Each header must start with Width=\"#\"|.  Please manually edit the markup in the following window.");
							ManuallyEdit();
							return;
						}
						string width=cells[c].Substring(7);//90"|Column Heading 1
						width=width.Substring(0,width.IndexOf("\""));//90
						ColWidths.Add(PIn.Int(width));
						colName=cells[c].Substring(cells[c].IndexOf("|")+1);
						ColNames.Add(colName);
						Table.Columns.Add("");//must be an empty string because Table object does not allow duplicate column names.
					}
					continue;
				}
				if(lines[i].Trim()=="|-") {
					continue;//totally ignore these rows
				}
				//normal row.  Headers will have already been filled
				lines[i]=lines[i].Substring(1);//strips off the leading |
				cells=lines[i].Split(new string[] { "||" },StringSplitOptions.None);
				if(cells.Length!=ColNames.Count || cells.Length!=ColWidths.Count) {
					MessageBox.Show("Table is corrupt.  There are "+ColNames.Count.ToString()+" columns, but row "+((i-1)/2).ToString()
						+" has "+cells.Length.ToString()+" cells.  Please manually edit the markup in the following window.");
					ManuallyEdit();
					return;
				}
				row=Table.NewRow();
				for(int c=0;c<cells.Length;c++) {
					row[c]=cells[c];
				}
				Table.Rows.Add(row);
			}
		}

		private void ManuallyEdit() {
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(Markup);
			msgbox.ShowDialog();
			if(msgbox.DialogResult==DialogResult.OK) {
				Markup=msgbox.textMain.Text;
				ParseMarkup();//try again
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			for(int c=0;c<ColNames.Count;c++){
				col=new GridColumn(ColNames[c],ColWidths[c],true);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<Table.Rows.Count;i++){
				row=new GridRow();
				for(int c=0;c<ColNames.Count;c++) {
					row.Cells.Add(Table.Rows[i][c].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			//new big window to edit row
		}

		private void gridMain_CellTextChanged(object sender,EventArgs e) {
			
		}

		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			Table.Rows[e.Row][e.Col]=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
		}

		/*No longer necessary because gridMain_CellLeave does this as text is changed.
		///<summary>This is done before generating markup, when adding or removing rows or columns, and when changing from "none" view to another view.  FillGrid can't be done until this is done.</summary>
		private void PumpGridIntoTable() {
			//table and grid will only have the same numbers of rows and columns if the view is none.
			//Otherwise, table may have more columns
			//So this is only allowed when switching from the none view to some other view.
			if(ViewShowing!=0) {
				return;
			}
			for(int i=0;i<Table.Rows.Count;i++) {
				for(int c=0;c<Table.Columns.Count;c++) {
					Table.Rows[i][c]=gridMain.Rows[i].Cells[c].Text;
				}
			}
		}*/

		///<summary>Happens when user clicks OK.  Also happens when user wants to manually edit markup.</summary>
		private string GenerateMarkup() {
			StringBuilder strb=new StringBuilder();
			strb.AppendLine("{|");
			strb.Append("!");
			for(int c=0;c<ColWidths.Count;c++) {
				if(c>0) {
					strb.Append("!!");
				}
				if(ColWidths[c]>0) {//otherwise, no width specified for this column.  Dynamic.
					strb.Append("Width=\""+ColWidths[c].ToString()+"\"|");
				}
				strb.Append(ColNames[c]);
			}
			strb.AppendLine();
			for(int i=0;i<Table.Rows.Count;i++) {
				strb.AppendLine("|-");
				strb.Append("|");
				for(int c=0;c<ColWidths.Count;c++) {
					if(c>0) {
						strb.Append("||");
					}
					strb.Append(Table.Rows[i][c].ToString());
				}
				strb.AppendLine();
			}
			strb.Append("|}");
			return strb.ToString();
		}

		private void butManEdit_Click(object sender,EventArgs e) {
			//PumpGridIntoTable();
			Markup=GenerateMarkup();
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(Markup);
			msgbox.ShowDialog();
			if(msgbox.DialogResult!=DialogResult.OK) {
				return;
			}
			Markup=msgbox.textMain.Text;
			ParseMarkup();
			FillGrid();
		}

		private void butColumnLeft_Click(object sender,EventArgs e) {
			//check to make sure you're not already at the left
			//we are guaranteed to have the Table and the gridMain synched.  Same # of rows and columns.
			//swap ColNames
			//swap ColWidths
			//Loop through table rows.
			//  Swap 2 cells.  Remember one of the first as part of the swap.
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a column first.");
				return;
			}
			if(gridMain.SelectedCell.X==0) {
				return;//Row is already on the left.
			}
			string colName;
			colName=ColNames[gridMain.SelectedCell.X];
			ColNames[gridMain.SelectedCell.X]=ColNames[gridMain.SelectedCell.X-1];
			ColNames[gridMain.SelectedCell.X-1]=colName;
			int width;
			width=ColWidths[gridMain.SelectedCell.X];
			ColWidths[gridMain.SelectedCell.X]=ColWidths[gridMain.SelectedCell.X-1];
			ColWidths[gridMain.SelectedCell.X-1]=width;
			string cellText;
			for(int i=0;i<Table.Rows.Count;i++) {
				cellText=Table.Rows[i][gridMain.SelectedCell.X].ToString();
				Table.Rows[i][gridMain.SelectedCell.X]=Table.Rows[i][gridMain.SelectedCell.X-1];
				Table.Rows[i][gridMain.SelectedCell.X-1]=cellText;
			}
			Point newCellSelected=new Point(gridMain.SelectedCell.X-1,gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butColumnRight_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a column first.");
				return;
			}
			if(gridMain.SelectedCell.X==Table.Columns.Count-1) {
				return;//Row is already on the right.
			}
			string colName;
			colName=ColNames[gridMain.SelectedCell.X];
			ColNames[gridMain.SelectedCell.X]=ColNames[gridMain.SelectedCell.X+1];
			ColNames[gridMain.SelectedCell.X+1]=colName;
			int width;
			width=ColWidths[gridMain.SelectedCell.X];
			ColWidths[gridMain.SelectedCell.X]=ColWidths[gridMain.SelectedCell.X+1];
			ColWidths[gridMain.SelectedCell.X+1]=width;
			string cellText;
			for(int i=0;i<Table.Rows.Count;i++) {
				cellText=Table.Rows[i][gridMain.SelectedCell.X].ToString();
				Table.Rows[i][gridMain.SelectedCell.X]=Table.Rows[i][gridMain.SelectedCell.X+1];
				Table.Rows[i][gridMain.SelectedCell.X+1]=cellText;
			}
			Point newCellSelected=new Point(gridMain.SelectedCell.X+1,gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butHeaders_Click(object sender,EventArgs e) {
			using FormWikiTableHeaders FormWTH=new FormWikiTableHeaders();
			FormWTH.ColNames=ColNames;//Just passes the reference to the list in memory, so no need to "collect" the changes afterwords.
			FormWTH.ColWidths=ColWidths;//Just passes the reference to the list in memory, so no need to "collect" the changes afterwords.
			FormWTH.ShowDialog();
			FillGrid();
		}

		private void butColumnAdd_Click(object sender,EventArgs e) {
			int index;
			if(gridMain.SelectedCell.X==-1) {
				index=Table.Columns.Count-1;
			}
			else {
				index=gridMain.SelectedCell.X;
			}
			Table.Columns.Add();
			ColNames.Insert(index+1,"Header"+(Table.Columns.Count));
			ColWidths.Insert(index+1,100);
			for(int i=0;i<Table.Rows.Count;i++) {
				for(int j=gridMain.ListGridColumns.Count-1;j>index;j--) {
					Table.Rows[i][j+1]=Table.Rows[i][j];
				}
				Table.Rows[i][index+1]="";
			}
			Point newCellSelected=new Point(index,gridMain.SelectedCell.Y);
			if(gridMain.SelectedCell.X==gridMain.ListGridColumns.Count-1) {//only if this is the last column
				newCellSelected=new Point(index+1,gridMain.SelectedCell.Y);//shift the selected column to the right
			}
			FillGrid();//gridMain.SelectedCell gets cleared.
			if(newCellSelected.Y>-1) {
				gridMain.SetSelected(newCellSelected);
			}
		}

		private void butColumnDelete_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a column first.");
				return;
			}
			if(gridMain.ListGridColumns.Count==1) {
				MsgBox.Show(this,"Cannot delete last column.");
				return;
			}
			ColNames.RemoveAt(gridMain.SelectedCell.X);
			ColWidths.RemoveAt(gridMain.SelectedCell.X);
			Table.Columns.RemoveAt(gridMain.SelectedCell.X);
			Point newCellSelected=new Point(Math.Max(gridMain.SelectedCell.X-1,0),gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butRowUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.Y==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.SelectedCell.Y==0) {
				return;//Row is already at the top.
			}
			DataRow row=Table.NewRow();
			for(int i=0;i<Table.Columns.Count;i++) {
				row[i]=Table.Rows[gridMain.SelectedCell.Y][i];
			}
			Table.Rows.InsertAt(row,gridMain.SelectedCell.Y-1);
			Table.Rows.RemoveAt(gridMain.SelectedCell.Y+1);
			Point newCellSelected=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y-1);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butRowDown_Click(object sender,EventArgs e) {
			//Table.Rows.InsertAt
			//DataRow row=Table.Rows[i];
			//Table.Rows.RemoveAt
			if(gridMain.SelectedCell.Y==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.SelectedCell.Y==Table.Rows.Count-1) {
				return;//Row is already at the bottom.
			}
			DataRow row=Table.NewRow();
			for(int i=0;i<Table.Columns.Count;i++) {
				row[i]=Table.Rows[gridMain.SelectedCell.Y+1][i];
			}
			Table.Rows.InsertAt(row,gridMain.SelectedCell.Y);
			Table.Rows.RemoveAt(gridMain.SelectedCell.Y+2);
			Point newCellSelected=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y+1);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butRowAdd_Click(object sender,EventArgs e) {
			//DataRow row=Table.NewRow();
			//Table.Rows.InsertAt(row,i);
			Point selectedCell;
			if(gridMain.SelectedCell.Y==-1) {
				selectedCell=new Point(0,Table.Rows.Count-1);
			}
			else {
				selectedCell=gridMain.SelectedCell;
			}
			DataRow row=Table.NewRow();
			Table.Rows.InsertAt(row,selectedCell.Y+1);
			Point newCellSelected=new Point(selectedCell.X,selectedCell.Y+1);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butRowDelete_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.Y==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.ListGridRows.Count==1) {
				MsgBox.Show(this,"Cannot delete last row.");
				return;
			}
			Table.Rows.RemoveAt(gridMain.SelectedCell.Y);
			Point newCellSelected=new Point(gridMain.SelectedCell.X,Math.Max(gridMain.SelectedCell.Y-1,0));
			FillGrid();//gridMain.SelectedCell gets cleared.
			if(newCellSelected.X>-1 && newCellSelected.Y >-1) {
				gridMain.SetSelected(newCellSelected);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this entire table?")) {
				return;
			}
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				Markup=null;
				DialogResult=DialogResult.OK;
			}
		}

		private void butPaste_Click(object sender,EventArgs e) {
			Point pointStarting=gridMain.SelectedCell;
			//The point will be set to (-1,-1) if no cell is currently selected.
			if(pointStarting.X < 0 || pointStarting.Y < 0) {
				MsgBox.Show(this,"Select a cell to paste clipboard contents into.");
				return;
			}
			int colsNeeded=0;
			int rowsNeeded=0;
			//the incoming text is not necessarily rectangular
			List<List<string>> listTblBuilder = new List<List<string>>();//tableBuilder[Y][X] to access cell.
			string clipBoardText;
			try {
				clipBoardText=ODClipboard.GetText();
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not paste contents from the clipboard.  Please try again.");
				ex.DoNothing();
				return;
			}
			string[] arrayTableRows = clipBoardText.Split(new string[] {"\r\n"},StringSplitOptions.None);
			rowsNeeded=arrayTableRows.Length;
			List<string> listCurrentRow;
			for(int i=0;i<arrayTableRows.Length;i++) {
				listCurrentRow = new List<string>();//currentRow[X] to access cell data
				string[] rowCells = arrayTableRows[i].Split('\t');
				foreach(string cell in rowCells) {
					listCurrentRow.Add(cell);
				}
				listTblBuilder.Add(listCurrentRow);
				colsNeeded=Math.Max(listCurrentRow.Count,colsNeeded);
			}
			//At this point:
			//colsNeeded = number of columns needed
			//rowsNeeded = number of rows needed
			//access data as arrayTblBuilder[Y][X], arrayTblBuilder contains all of the table data in a potentially uneven array (technically a list), 
			//Check for enough columns---------------------------------------------------------------------------------------------------------
			if(pointStarting.X + colsNeeded > Table.Columns.Count) {
				MessageBox.Show(this,Lan.g(this,"Additional columns required to paste")+": "+(pointStarting.X+colsNeeded-gridMain.ListGridColumns.Count));
				return;
			}
			//Check for Content----------------------------------------------------------------------------------------------------------------
			bool contentExists=false;
			for(int x=0;x+pointStarting.X<Table.Columns.Count;x++) {
				for(int y=0;y+pointStarting.Y<Table.Rows.Count;y++) {
					contentExists=contentExists||!Table.Rows[pointStarting.Y+y][pointStarting.X+x].ToString().Equals("");
				}
			}
			if(contentExists) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to continue and overwrite existing content?")){
					return;
				}
			}
			//Add New Rows---------------------------------------------------------------------------------------------------------------------
			//Must be after check for existing content, otherwise you will add rows when they are not necessary.
			if(Table.Rows.Count < pointStarting.Y + rowsNeeded) {
				int newRowsNeededCount=(pointStarting.Y+rowsNeeded)-Table.Rows.Count;
				for(int i=0;i<newRowsNeededCount;i++) {
					Table.Rows.Add(Table.NewRow());
				}
			}
			//Paste new data into data Table---------------------------------------------------------------------------------------------------
			for(int i=0;i<listTblBuilder.Count;i++) {
				for(int j=0;j<listTblBuilder[i].Count;j++) {
					//gridMain.Rows[startingPoint.Y+i].Cells[startingPoint.X+j].Text=tableBuilder[i][j];
					Table.Rows[pointStarting.Y+i][pointStarting.X+j]=listTblBuilder[i][j];
				}
			}
			//Redraw Grid
			Point newCellSelected=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(newCellSelected);
		}

		private void butOK_Click(object sender,EventArgs e) {
			//PumpGridIntoTable();
			for(int h=0;h<gridMain.ListGridColumns.Count;h++) {//loops through every header in the main grid
				string s=gridMain.ListGridColumns[h].Heading.ToString();
				s=s.Replace("&","&amp;");
				s=s.Replace("&amp;<","&lt;");//because "&" was changed to "&amp;" in the line above.
				s=s.Replace("&amp;>","&gt;");//because "&" was changed to "&amp;" in the line above.
				s="<body>"+s+"</body>";//Surround with body tags to make it valid xml
				XmlDocument doc=new XmlDocument();
				using(StringReader reader=new StringReader(s)) {
					try {
						doc.Load(reader);//Loading this document provides error checking
					}
					catch {
						MessageBox.Show(Lan.g(this,"The header for column "+(h+1)+" is invalid."));
						return;
					}
				}
			}
			for(int i=0;i<Table.Rows.Count;i++) {//loops through each row in the table
				for(int j=0;j<Table.Columns.Count;j++) {//loops through each column in the row
					XmlDocument doc=new XmlDocument();
					string s=Table.Rows[i][j].ToString();
					s=s.Replace("&","&amp;");
					s=s.Replace("&amp;<","&lt;");//because "&" was changed to "&amp;" in the line above.
					s=s.Replace("&amp;>","&gt;");//because "&" was changed to "&amp;" in the line above.
					s="<body>"+s+"</body>";
					using(StringReader reader=new StringReader(s))
					{
						try {
							doc.Load(reader);
						}
						catch {
							MessageBox.Show(Lan.g(this,"The cell at column "+(j+1)+", row "+(i+1)+" is invalid"));
							return;
						}
					}
				}
			}
			Markup=GenerateMarkup();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		
	

	

		

		

		

	

	}
}