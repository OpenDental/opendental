using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiListHeaders : FormODBase {
		private string _wikiListCurName;
		///<summary>Widths must always be specified.  Not optional.</summary>
		private List<WikiListHeaderWidth> _listTableHeaders;
		///<summary>All possible "options" for the currently selected Wiki List Header.</summary>
		private List<string> _listStringPick=new List<string>();
		private Point _gridCurCell=new Point(-1,-1);
		private int _pickListIndex=-1;

		public FormWikiListHeaders(string wikiListCurName) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_wikiListCurName=wikiListCurName;
		}

		private void FormWikiListHeaders_Load(object sender,EventArgs e) {
			_listTableHeaders=WikiListHeaderWidths.GetForList(_wikiListCurName).Select(x => x.Copy()).ToList();
			FillGrid();
		}

		///<summary>Each item defines a column in the wiki list grid.</summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableWikiListHeaders","Column Name"),100,true) { IsWidthDynamic=true,DynamicWeight=3 });
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableWikiListHeaders","Width"),100,true) { IsWidthDynamic=true,DynamicWeight=1 });
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableWikiListHeaders","Is Hidden"),0,HorizontalAlignment.Center,false) { IsWidthDynamic=true,DynamicWeight=1 });
			gridMain.ListGridRows.Clear();
			gridMain.ListGridRows.AddRange(_listTableHeaders.Select(x => new GridRow(x.ColName,x.ColWidth.ToString(),x.IsHidden?"X":"")));
			gridMain.EndUpdate();
		}

		///<summary>Filled when Add, Remove, or change main header idx.</summary>
		private void FillGridPickList() {
			gridPickList.BeginUpdate();
			gridPickList.ListGridColumns.Clear();
			gridPickList.ListGridColumns.Add(new GridColumn(Lan.g(gridPickList.TranslationName,"Input Text"),100,true));
			gridPickList.ListGridRows.Clear();
			if(gridMain.SelectedCell.Y!=-1 && _listStringPick.Count>0) {
				gridPickList.ListGridRows.AddRange(_listStringPick.Select(x => new GridRow(x)));
			}
			gridPickList.EndUpdate();
			if(_pickListIndex > -1 && _listStringPick.Count > _pickListIndex) {
				gridPickList.SetSelected(_pickListIndex,true);
			}
		}

		private void UpdateShowHideColumnButtonText(int columnIndex) {
			if(_listTableHeaders[columnIndex].IsHidden) {
				butHideColumn.Text=Lan.g(this,"Show Column");
			}
			else {
				butHideColumn.Text=Lan.g(this,"Hide Column");
			}
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==0 && e.Col==0){
				MsgBox.Show("First column name cannot be edited.");
				_gridCurCell=new Point(1,e.Row);
				gridMain.SetSelected(new Point(1,e.Row));
			}
			UpdateShowHideColumnButtonText(e.Row);
			if(_gridCurCell.Y==e.Row && _gridCurCell.X==e.Col){
				return;
			}
			if(_gridCurCell.Y>-1){//it will be -1 on startup
				//"save" the picklist back to an \r\n delimited string
				_listTableHeaders[_gridCurCell.Y].PickList=string.Join("\r\n",_listStringPick);
			}
			_listStringPick=_listTableHeaders[e.Row].PickList.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries).ToList();
			_pickListIndex=-1;
			FillGridPickList();
			_gridCurCell=new Point(e.Col,e.Row);
			butHideColumn.Enabled=true;
		}

		private void butHideColumn_Click(object sender,EventArgs e) {
			_listTableHeaders[_gridCurCell.Y].IsHidden=!_listTableHeaders[_gridCurCell.Y].IsHidden;
			if(_listTableHeaders.All(x => x.IsHidden)) {
				MsgBox.Show(this,"At least one column must be visible.");
				_listTableHeaders[_gridCurCell.Y].IsHidden=false;
				return;
			}
			UpdateShowHideColumnButtonText(_gridCurCell.Y);
			FillGrid();
			gridMain.SetSelected(_gridCurCell);
		}

		///<summary>Used to store the currently selected cell in the secondary grid.  
		///SelectedCell and Selected don't behave correctly when you click away or the cell is editable.</summary>
		private void gridPickList_CellEnter(object sender,ODGridClickEventArgs e) {
			_pickListIndex=e.Row;
		}

		private void gridPickList_CellLeave(object sender,ODGridClickEventArgs e) {
			string addedListItem=gridPickList.ListGridRows[e.Row].Cells[0].Text;
			if(e.Row < _listStringPick.Count) {
				_listStringPick[e.Row]=addedListItem;
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.Y==0){
				MsgBox.Show("Pick list cannot be added to first column.");
				return;
			}
			//Get input from user
			using InputBox newOption=new InputBox(Lan.g(this,"New Pick List Option"));
			if(newOption.ShowDialog()==DialogResult.OK) {
				_listStringPick.Add(newOption.textResult.Text);
				_pickListIndex=_listStringPick.Count-1;
				FillGridPickList();
			}
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(_pickListIndex==-1) {
				return;
			}
			_listStringPick.RemoveAt(_pickListIndex);
			_pickListIndex=Math.Max(_pickListIndex-1,0);
			FillGridPickList();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_gridCurCell.Y>0 && _gridCurCell.Y<_listTableHeaders.Count) {//no pick list option for the PK col
				_listTableHeaders[_gridCurCell.Y].PickList=string.Join("\r\n",_listStringPick);
			}
			#region Validation
			List<string> listColNames=new List<string>();
			for(int i=1;i<gridMain.ListGridRows.Count;i++) {//start with index 1, first col is PK
				string colName=gridMain.ListGridRows[i].Cells[0].Text;
				string colWidth=gridMain.ListGridRows[i].Cells[1].Text;
				#region Validate Column Widths
				if(Regex.IsMatch(colWidth,@"\D")) {// "\D" matches any non-decimal character
					MsgBox.Show(this,"Column widths must only contain positive integers.");
					return;
				}
				//inlcude the comma for international support. For instance Pi = 3.1415 or 3,1415 depending on your region
				if(new[] { '-','.',',' }.Any(x => colWidth.Contains(x))) {
					MsgBox.Show(this,"Column widths must only contain positive integers.");
					return;
				}
				#endregion Validate Column Widths
				#region Validate Column Names
				if(listColNames.Contains(colName)) {
					MessageBox.Show(Lan.g(this,$"Duplicate column name detected")+": "+colName);
					return;
				}
				listColNames.Add(colName);
				if(i==0) {//don't check PK column name, since it can't be changed
					continue;
				}
				if(Regex.IsMatch(colName,@"^\d")) {
					MsgBox.Show(this,"Column cannot start with numbers.");
					return;
				}
				if(Regex.IsMatch(colName,@"\s")) {
					MsgBox.Show(this,"Column names cannot contain spaces.");
					return;
				}
				if(Regex.IsMatch(colName,@"\W")) {//W=non-word chars
					MsgBox.Show(this,"Column names cannot contain special characters.");
					return;
				}
				//Check for reserved words--------------------------------------------------------------------------------
				if(DbHelper.isMySQLReservedWord(colName)) {
					MessageBox.Show(Lan.g(this,"Column name is a reserved word in MySQL")+": "+colName);
					return;
				}
				#endregion Validate Column Names
			}
			#endregion Validation
			#region Update _listTableHeaders
			for(int i=0;i<_listTableHeaders.Count;i++) {
				if(i>0) {//don't allow renaming the first column, it's the PK
					_listTableHeaders[i].ColName=PIn.String(gridMain.ListGridRows[i].Cells[0].Text);
				}
				_listTableHeaders[i].ColWidth=PIn.Int(gridMain.ListGridRows[i].Cells[1].Text);
			}
			#endregion Update _listTableHeaders
			#region Try Update DB
			try {
				WikiListHeaderWidths.UpdateNamesAndWidths(_wikiListCurName,_listTableHeaders);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//will throw exception if table schema has changed since the window was opened.
				DialogResult=DialogResult.Cancel;
			}
			#endregion Try Update DB
			DataValid.SetInvalid(InvalidType.Wiki);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}