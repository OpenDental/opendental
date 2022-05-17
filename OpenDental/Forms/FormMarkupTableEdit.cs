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
		private DataTable _table;
		private List<string> _listColNames;
		///<summary>Widths must always be specified.  Not optional.</summary>
		private List<int> _listColWidths;
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
			_table=new DataTable();
			_listColNames=new List<string>();
			_listColWidths=new List<int>();
			DataRow dataRow;
			string[] stringArrayCells;
			string[] stringArrayLines=Markup.Split(new string[] { "{|\r\n","{|\n","\r\n|-\r\n","\n|-\n","\r\n|}","\n|}" },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<stringArrayLines.Length;i++) {
				if(stringArrayLines[i].StartsWith("!")) {//header
					stringArrayLines[i]=stringArrayLines[i].Substring(1);//strips off the leading !
					stringArrayCells=stringArrayLines[i].Split(new string[] { "!!" },StringSplitOptions.None);
					for(int c=0;c<stringArrayCells.Length;c++) {
						string colName="";
						if(!Regex.IsMatch(stringArrayCells[c],@"^(Width="")\d+""\|")) {//e.g. Width="90"| 
							MessageBox.Show("Table is corrupt.  Each header must start with Width=\"#\"|.  Please manually edit the markup in the following window.");
							ManuallyEdit();
							return;
						}
						string width=stringArrayCells[c].Substring(7);//90"|Column Heading 1
						width=width.Substring(0,width.IndexOf("\""));//90
						_listColWidths.Add(PIn.Int(width));
						colName=stringArrayCells[c].Substring(stringArrayCells[c].IndexOf("|")+1);
						_listColNames.Add(colName);
						_table.Columns.Add("");//must be an empty string because Table object does not allow duplicate column names.
					}
					continue;
				}
				if(stringArrayLines[i].Trim()=="|-") {
					continue;//totally ignore these rows
				}
				//normal row.  Headers will have already been filled
				stringArrayLines[i]=stringArrayLines[i].Substring(1);//strips off the leading |
				stringArrayCells=stringArrayLines[i].Split(new string[] { "||" },StringSplitOptions.None);
				if(stringArrayCells.Length!=_listColNames.Count || stringArrayCells.Length!=_listColWidths.Count) {
					MessageBox.Show("Table is corrupt.  There are "+_listColNames.Count.ToString()+" columns, but row "+((i-1)/2).ToString()
						+" has "+stringArrayCells.Length.ToString()+" cells.  Please manually edit the markup in the following window.");
					ManuallyEdit();
					return;
				}
				dataRow=_table.NewRow();
				for(int c=0;c<stringArrayCells.Length;c++) {
					dataRow[c]=stringArrayCells[c];
				}
				_table.Rows.Add(dataRow);
			}
		}

		private void ManuallyEdit() {
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Markup);
			msgBoxCopyPaste.ShowDialog();
			if(msgBoxCopyPaste.DialogResult==DialogResult.OK) {
				Markup=msgBoxCopyPaste.textMain.Text;
				ParseMarkup();//try again
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			for(int c=0;c<_listColNames.Count;c++){
				col=new GridColumn(_listColNames[c],_listColWidths[c],true);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_table.Rows.Count;i++){
				row=new GridRow();
				for(int c=0;c<_listColNames.Count;c++) {
					row.Cells.Add(_table.Rows[i][c].ToString());
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
			_table.Rows[e.Row][e.Col]=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
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
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine("{|");
			stringBuilder.Append("!");
			for(int c=0;c<_listColWidths.Count;c++) {
				if(c>0) {
					stringBuilder.Append("!!");
				}
				if(_listColWidths[c]>0) {//otherwise, no width specified for this column.  Dynamic.
					stringBuilder.Append("Width=\""+_listColWidths[c].ToString()+"\"|");
				}
				stringBuilder.Append(_listColNames[c]);
			}
			stringBuilder.AppendLine();
			for(int i=0;i<_table.Rows.Count;i++) {
				stringBuilder.AppendLine("|-");
				stringBuilder.Append("|");
				for(int c=0;c<_listColWidths.Count;c++) {
					if(c>0) {
						stringBuilder.Append("||");
					}
					stringBuilder.Append(_table.Rows[i][c].ToString());
				}
				stringBuilder.AppendLine();
			}
			stringBuilder.Append("|}");
			return stringBuilder.ToString();
		}

		private void butManEdit_Click(object sender,EventArgs e) {
			//PumpGridIntoTable();
			Markup=GenerateMarkup();
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Markup);
			msgBoxCopyPaste.ShowDialog();
			if(msgBoxCopyPaste.DialogResult!=DialogResult.OK) {
				return;
			}
			Markup=msgBoxCopyPaste.textMain.Text;
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
			colName=_listColNames[gridMain.SelectedCell.X];
			_listColNames[gridMain.SelectedCell.X]=_listColNames[gridMain.SelectedCell.X-1];
			_listColNames[gridMain.SelectedCell.X-1]=colName;
			int width;
			width=_listColWidths[gridMain.SelectedCell.X];
			_listColWidths[gridMain.SelectedCell.X]=_listColWidths[gridMain.SelectedCell.X-1];
			_listColWidths[gridMain.SelectedCell.X-1]=width;
			string cellText;
			for(int i=0;i<_table.Rows.Count;i++) {
				cellText=_table.Rows[i][gridMain.SelectedCell.X].ToString();
				_table.Rows[i][gridMain.SelectedCell.X]=_table.Rows[i][gridMain.SelectedCell.X-1];
				_table.Rows[i][gridMain.SelectedCell.X-1]=cellText;
			}
			Point pointNewCellSelected=new Point(gridMain.SelectedCell.X-1,gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
		}

		private void butColumnRight_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a column first.");
				return;
			}
			if(gridMain.SelectedCell.X==_table.Columns.Count-1) {
				return;//Row is already on the right.
			}
			string colName;
			colName=_listColNames[gridMain.SelectedCell.X];
			_listColNames[gridMain.SelectedCell.X]=_listColNames[gridMain.SelectedCell.X+1];
			_listColNames[gridMain.SelectedCell.X+1]=colName;
			int width;
			width=_listColWidths[gridMain.SelectedCell.X];
			_listColWidths[gridMain.SelectedCell.X]=_listColWidths[gridMain.SelectedCell.X+1];
			_listColWidths[gridMain.SelectedCell.X+1]=width;
			string cellText;
			for(int i=0;i<_table.Rows.Count;i++) {
				cellText=_table.Rows[i][gridMain.SelectedCell.X].ToString();
				_table.Rows[i][gridMain.SelectedCell.X]=_table.Rows[i][gridMain.SelectedCell.X+1];
				_table.Rows[i][gridMain.SelectedCell.X+1]=cellText;
			}
			Point pointNewCellSelected=new Point(gridMain.SelectedCell.X+1,gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
		}

		private void butHeaders_Click(object sender,EventArgs e) {
			using FormWikiTableHeaders formWikiTableHeaders=new FormWikiTableHeaders();
			formWikiTableHeaders.ColNames=_listColNames;//Just passes the reference to the list in memory, so no need to "collect" the changes afterwords.
			formWikiTableHeaders.ColWidths=_listColWidths;//Just passes the reference to the list in memory, so no need to "collect" the changes afterwords.
			formWikiTableHeaders.ShowDialog();
			FillGrid();
		}

		private void butColumnAdd_Click(object sender,EventArgs e) {
			int index;
			if(gridMain.SelectedCell.X==-1) {
				index=_table.Columns.Count-1;
			}
			else {
				index=gridMain.SelectedCell.X;
			}
			_table.Columns.Add();
			_listColNames.Insert(index+1,"Header"+(_table.Columns.Count));
			_listColWidths.Insert(index+1,100);
			for(int i=0;i<_table.Rows.Count;i++) {
				for(int j=gridMain.Columns.Count-1;j>index;j--) {
					_table.Rows[i][j+1]=_table.Rows[i][j];
				}
				_table.Rows[i][index+1]="";
			}
			Point pointNewCellSelected=new Point(index,gridMain.SelectedCell.Y);
			if(gridMain.SelectedCell.X==gridMain.Columns.Count-1) {//only if this is the last column
				pointNewCellSelected=new Point(index+1,gridMain.SelectedCell.Y);//shift the selected column to the right
			}
			FillGrid();//gridMain.SelectedCell gets cleared.
			if(pointNewCellSelected.Y>-1) {
				gridMain.SetSelected(pointNewCellSelected);
			}
		}

		private void butColumnDelete_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a column first.");
				return;
			}
			if(gridMain.Columns.Count==1) {
				MsgBox.Show(this,"Cannot delete last column.");
				return;
			}
			_listColNames.RemoveAt(gridMain.SelectedCell.X);
			_listColWidths.RemoveAt(gridMain.SelectedCell.X);
			_table.Columns.RemoveAt(gridMain.SelectedCell.X);
			Point pointNewCellSelected=new Point(Math.Max(gridMain.SelectedCell.X-1,0),gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
		}

		private void butRowUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.Y==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.SelectedCell.Y==0) {
				return;//Row is already at the top.
			}
			DataRow dataRow=_table.NewRow();
			for(int i=0;i<_table.Columns.Count;i++) {
				dataRow[i]=_table.Rows[gridMain.SelectedCell.Y][i];
			}
			_table.Rows.InsertAt(dataRow,gridMain.SelectedCell.Y-1);
			_table.Rows.RemoveAt(gridMain.SelectedCell.Y+1);
			Point pointNewCellSelected=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y-1);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
		}

		private void butRowDown_Click(object sender,EventArgs e) {
			//Table.Rows.InsertAt
			//DataRow row=Table.Rows[i];
			//Table.Rows.RemoveAt
			if(gridMain.SelectedCell.Y==-1) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(gridMain.SelectedCell.Y==_table.Rows.Count-1) {
				return;//Row is already at the bottom.
			}
			DataRow dataRow=_table.NewRow();
			for(int i=0;i<_table.Columns.Count;i++) {
				dataRow[i]=_table.Rows[gridMain.SelectedCell.Y+1][i];
			}
			_table.Rows.InsertAt(dataRow,gridMain.SelectedCell.Y);
			_table.Rows.RemoveAt(gridMain.SelectedCell.Y+2);
			Point pointNewCellSelected=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y+1);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
		}

		private void butRowAdd_Click(object sender,EventArgs e) {
			//DataRow row=Table.NewRow();
			//Table.Rows.InsertAt(row,i);
			Point pointSelectedCell;
			if(gridMain.SelectedCell.Y==-1) {
				pointSelectedCell=new Point(0,_table.Rows.Count-1);
			}
			else {
				pointSelectedCell=gridMain.SelectedCell;
			}
			DataRow dataRow=_table.NewRow();
			_table.Rows.InsertAt(dataRow,pointSelectedCell.Y+1);
			Point pointNewCellSelected=new Point(pointSelectedCell.X,pointSelectedCell.Y+1);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
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
			_table.Rows.RemoveAt(gridMain.SelectedCell.Y);
			Point pointNewCellSelected=new Point(gridMain.SelectedCell.X,Math.Max(gridMain.SelectedCell.Y-1,0));
			FillGrid();//gridMain.SelectedCell gets cleared.
			if(pointNewCellSelected.X>-1 && pointNewCellSelected.Y >-1) {
				gridMain.SetSelected(pointNewCellSelected);
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
			string[] stringArrayTableRows = clipBoardText.Split(new string[] {"\r\n"},StringSplitOptions.None);
			rowsNeeded=stringArrayTableRows.Length;
			List<string> listCurrentRows;
			for(int i=0;i<stringArrayTableRows.Length;i++) {
				listCurrentRows = new List<string>();//currentRow[X] to access cell data
				string[] stringArrayRowCells = stringArrayTableRows[i].Split('\t');
				foreach(string cell in stringArrayRowCells) {
					listCurrentRows.Add(cell);
				}
				listTblBuilder.Add(listCurrentRows);
				colsNeeded=Math.Max(listCurrentRows.Count,colsNeeded);
			}
			//At this point:
			//colsNeeded = number of columns needed
			//rowsNeeded = number of rows needed
			//access data as arrayTblBuilder[Y][X], arrayTblBuilder contains all of the table data in a potentially uneven array (technically a list), 
			//Check for enough columns---------------------------------------------------------------------------------------------------------
			if(pointStarting.X + colsNeeded > _table.Columns.Count) {
				MessageBox.Show(this,Lan.g(this,"Additional columns required to paste")+": "+(pointStarting.X+colsNeeded-gridMain.Columns.Count));
				return;
			}
			//Check for Content----------------------------------------------------------------------------------------------------------------
			bool contentExists=false;
			for(int x=0;x+pointStarting.X<_table.Columns.Count;x++) {
				for(int y=0;y+pointStarting.Y<_table.Rows.Count;y++) {
					contentExists=contentExists||!_table.Rows[pointStarting.Y+y][pointStarting.X+x].ToString().Equals("");
				}
			}
			if(contentExists) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to continue and overwrite existing content?")){
					return;
				}
			}
			//Add New Rows---------------------------------------------------------------------------------------------------------------------
			//Must be after check for existing content, otherwise you will add rows when they are not necessary.
			if(_table.Rows.Count < pointStarting.Y + rowsNeeded) {
				int newRowsNeededCount=(pointStarting.Y+rowsNeeded)-_table.Rows.Count;
				for(int i=0;i<newRowsNeededCount;i++) {
					_table.Rows.Add(_table.NewRow());
				}
			}
			//Paste new data into data Table---------------------------------------------------------------------------------------------------
			for(int i=0;i<listTblBuilder.Count;i++) {
				for(int j=0;j<listTblBuilder[i].Count;j++) {
					//gridMain.Rows[startingPoint.Y+i].Cells[startingPoint.X+j].Text=tableBuilder[i][j];
					_table.Rows[pointStarting.Y+i][pointStarting.X+j]=listTblBuilder[i][j];
				}
			}
			//Redraw Grid
			Point pointNewCellSelected=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y);
			FillGrid();//gridMain.SelectedCell gets cleared.
			gridMain.SetSelected(pointNewCellSelected);
		}

		private void butOK_Click(object sender,EventArgs e) {
			//PumpGridIntoTable();
			for(int h=0;h<gridMain.Columns.Count;h++) {//loops through every header in the main grid
				string s=gridMain.Columns[h].Heading.ToString();
				s=s.Replace("&","&amp;");
				s=s.Replace("&amp;<","&lt;");//because "&" was changed to "&amp;" in the line above.
				s=s.Replace("&amp;>","&gt;");//because "&" was changed to "&amp;" in the line above.
				s="<body>"+s+"</body>";//Surround with body tags to make it valid xml
				XmlDocument xmlDocument=new XmlDocument();
				using(StringReader stringReader=new StringReader(s)) {
					try {
						xmlDocument.Load(stringReader);//Loading this document provides error checking
					}
					catch {
						MessageBox.Show(Lan.g(this,"The header for column "+(h+1)+" is invalid."));
						return;
					}
				}
			}
			for(int i=0;i<_table.Rows.Count;i++) {//loops through each row in the table
				for(int j=0;j<_table.Columns.Count;j++) {//loops through each column in the row
					XmlDocument xmlDocument=new XmlDocument();
					string s=_table.Rows[i][j].ToString();
					s=s.Replace("&","&amp;");
					s=s.Replace("&amp;<","&lt;");//because "&" was changed to "&amp;" in the line above.
					s=s.Replace("&amp;>","&gt;");//because "&" was changed to "&amp;" in the line above.
					s="<body>"+s+"</body>";
					using(StringReader stringReader=new StringReader(s))
					{
						try {
							xmlDocument.Load(stringReader);
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