using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiListEdit:FormODBase {
		///<summary>Name of the wiki list being manipulated. This does not include the "wikilist" prefix. i.e. "networkdevices" not "wikilistnetworkdevices"</summary>
		public string WikiListCurName;
		public bool IsNew;
		private DataTable _table;
		private WikiListHist _wikiListOld;
		private bool _isEdited;
		private int[] _arraySearchColIdxs=new int[0];
		///<summary>A list of all possible column headers for the current wiki list.  Each header contains additional information (e.g. PickList) that can be useful.</summary>
		private List<WikiListHeaderWidth> _listColumnHeaders=new List<WikiListHeaderWidth>();
		///<summary>Whether or not this list has at least one hidden column</summary>
		private bool _hasHiddenColumns;


		public FormWikiListEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiListEdit_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textSearch);
			if(!WikiLists.CheckExists(WikiListCurName)) {
				IsNew=true;
				WikiLists.CreateNewWikiList(WikiListCurName);
			}
			_wikiListOld=WikiListHists.GenerateFromName(WikiListCurName,Security.CurUser.UserNum)??new WikiListHist();
			FillGrid();
			ActiveControl=textSearch;//start in search box.
		}

		///<summary>Fills the grid with the contents of the corresponding wiki list table in the database.
		///After filling the grid, FilterGrid() will get invoked to apply any advanced search options.</summary>
		private void FillGrid() {
			_listColumnHeaders=WikiListHeaderWidths.GetForList(WikiListCurName);
			_table=WikiLists.GetByName(WikiListCurName);
			if(_table.Rows.Count>0 && _listColumnHeaders.Count!=_table.Columns.Count) {//if these do not match, something happened to be desynched at the right moment.
				WikiListHeaderWidths.RefreshCache();
				_table=WikiLists.GetByName(WikiListCurName);
				_listColumnHeaders=WikiListHeaderWidths.GetForList(WikiListCurName);
				if(_listColumnHeaders.Count!=_table.Columns.Count) {//if they still do not match, one of them did not get synched correctly.
					MsgBox.Show(this,"Unable to open the wiki list.");
					return;
				}
			}
			_hasHiddenColumns=_listColumnHeaders.Any(x => x.IsHidden);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			//add all visible headers as grid columns
			for(int i=0;i<_listColumnHeaders.Count;i++) {
				if(checkBoxIncludeHiddenColumns.Checked || !_listColumnHeaders[i].IsHidden) {//only add columns that should be visible right now
					GridColumn newCol=new GridColumn(_listColumnHeaders[i].ColName,_listColumnHeaders[i].ColWidth);
					if(_listColumnHeaders[i].IsHidden) {
						newCol.Heading+=$" ({Lan.g(this,"hidden")})";
					}
					gridMain.ListGridColumns.Add(newCol);
				}
			}
			gridMain.ListGridRows.Clear();
			DataRow[] dataRowArray=_table.Select();
			for(int dataRowIndex=0;dataRowIndex<dataRowArray.Length;dataRowIndex++) {
				//only include currently-visible cells in this row
				string[] arrayVisibleCells=dataRowArray[dataRowIndex].ItemArray
					.Where((_,index) => checkBoxIncludeHiddenColumns.Checked || !_listColumnHeaders[index].IsHidden)
					.Select(x => x.ToString())
					.ToArray();
				gridMain.ListGridRows.Add(new GridRow(arrayVisibleCells) { Tag=dataRowIndex });
			}
			gridMain.Title=WikiListCurName;
			gridMain.EndUpdate();
			FilterGrid();
		}

		///<summary>Visually filters gridMain.  Tag is preserved so that double clicking and editing can still work.</summary>
		private void FilterGrid() {
			labelSearch.Text=Lan.g(this,"Search");
			labelSearch.ForeColor=Color.Black;
			List<string> searchTerms=textSearch.Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();
			if(!_arraySearchColIdxs.IsNullOrEmpty()) {//adv search has been used, search specific columns selected
				if(!checkBoxIncludeHiddenColumns.Checked && _hasHiddenColumns) {//don't allow advanced search with hidden columns
					_arraySearchColIdxs=new int[0];
					if(!string.IsNullOrEmpty(textSearch.Text)) {
						textSearch.Clear();
					}
					return;
				}
				labelSearch.Text=Lan.g(this,"Advanced Search");
				labelSearch.ForeColor=Color.Red;
			}
			if(string.IsNullOrEmpty(textSearch.Text)) {
				return;
			}
			if(radioButFilter.Checked) {
				gridMain.BeginUpdate();
				gridMain.ListGridRows.Clear();
			}
			bool isScrollSet=false;
			for(int i=0;i<_table.Rows.Count;i++) {
				if(radioButHighlight.Checked) {
					gridMain.ListGridRows[i].ColorBackG=Color.White;
				}
				//For regular search, only search visible columns
				string[] listCellVals=_table.Rows[i].ItemArray
					.Where((_,index) => checkBoxIncludeHiddenColumns.Checked || !_listColumnHeaders[index].IsHidden)
					.Select(y => y.ToString()).ToArray();
				if((_arraySearchColIdxs.IsNullOrEmpty()//not advanced search, so compare to all cell values
						&& !searchTerms.Any(x => listCellVals.Any(y => y.ToUpper().Contains(x.ToUpper()))))
					|| (!_arraySearchColIdxs.IsNullOrEmpty()//advanced search, only compare to selected column cell values
						&& !searchTerms.Any(x => _arraySearchColIdxs.Any(y => listCellVals[y].ToUpper().Contains(x.ToUpper())))))
				{
					continue;
				}
				//matching row
				if(radioButFilter.Checked) {
					gridMain.ListGridRows.Add(new GridRow(listCellVals) { Tag=i });
				}
				else {
					gridMain.ListGridRows[i].ColorBackG=Color.Yellow;
					if(!isScrollSet) {
						gridMain.ScrollToIndex(i);
						isScrollSet=true;
					}
				}
			}
			if(radioButFilter.Checked) {
				gridMain.EndUpdate();
			}
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using(FormWikiListItemEdit FormWLIE=new FormWikiListItemEdit()) {
				FormWLIE.WikiListCurName=WikiListCurName;
				FormWLIE.ItemNum=PIn.Long(_table.Rows[(int)gridMain.ListGridRows[e.Row].Tag][0].ToString());
				FormWLIE.ListColumnHeaders=_listColumnHeaders;
				//saving occurs from within the form.
				if(FormWLIE.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			SetIsEdited();
			FillGrid();
		}

		private bool CanMoveOrDeleteColumn(bool isDelete=false) {
			if(gridMain.SelectedCell.X==-1) {//a cell must be selected
				if(isDelete) {
					MsgBox.Show(this,"Select cell in column to be deleted first.");
				}
				else {
					MsgBox.Show(this,"Select cell in column to be moved first.");
				}
				return false;
			}
			if(!Security.IsAuthorized(Permissions.WikiListSetup)) {//gives a message box if no permission
				return false;
			}
			if(!checkBoxIncludeHiddenColumns.Checked && _hasHiddenColumns) {
				if(isDelete) {
					MsgBox.Show(this,"Include hidden columns before deleting one.");
				}
				else {
					MsgBox.Show(this,"Include hidden columns before moving one.");
				}
				return false;
			}
			return true;
		}

		private void butColumnLeft_Click(object sender,EventArgs e) {
			if(!CanMoveOrDeleteColumn()) {
				return;
			}
			if(gridMain.SelectedCell.X<2) {//can't shift first col nor the 2nd col left since, the first col is the PK and can't be moved right
				return;
			}
			SetIsEdited();
			Point pointNewSelectedCell=gridMain.SelectedCell;
			pointNewSelectedCell.X=Math.Max(1,pointNewSelectedCell.X-1);
			WikiLists.ShiftColumnLeft(WikiListCurName,_table.Columns[gridMain.SelectedCell.X].ColumnName);
			FillGrid();
			gridMain.SetSelected(pointNewSelectedCell);
		}

		private void butColumnRight_Click(object sender,EventArgs e) {
			if(!CanMoveOrDeleteColumn()) {
				return;
			}
			if(gridMain.SelectedCell.X>gridMain.ListGridColumns.Count-2) {//can't shift the last column right
				return;
			}
			SetIsEdited();
			Point pointNewSelectedCell=gridMain.SelectedCell;
			pointNewSelectedCell.X=Math.Min(gridMain.ListGridColumns.Count-1,pointNewSelectedCell.X+1);
			WikiLists.ShiftColumnRight(WikiListCurName,_table.Columns[gridMain.SelectedCell.X].ColumnName);
			FillGrid();
			gridMain.SetSelected(pointNewSelectedCell);
		}

		private void butColumnEdit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.WikiListSetup)) {//gives a message box if no permission
				return;
			}
			using(FormWikiListHeaders FormWLH=new FormWikiListHeaders(WikiListCurName)) {
				if(FormWLH.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			SetIsEdited();
			FillGrid();
		}

		private void butColumnAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.WikiListSetup)) {//gives a message box if no permission
				return;
			}
			SetIsEdited();
			WikiLists.AddColumn(WikiListCurName);
			FillGrid();
		}

		private void butColumnDelete_Click(object sender,EventArgs e) {
			if(!CanMoveOrDeleteColumn(isDelete:true)) {
				return;
			}
			if(!WikiLists.CheckColumnEmpty(WikiListCurName,_table.Columns[gridMain.SelectedCell.X].ColumnName)) {
				MsgBox.Show(this,"Column cannot be deleted because it contains data.");
				return;
			}
			SetIsEdited();
			WikiLists.DeleteColumn(WikiListCurName,_table.Columns[gridMain.SelectedCell.X].ColumnName);
			FillGrid();
		}

		private void butAddItem_Click(object sender,EventArgs e) {
			long itemNum=0;
			using(FormWikiListItemEdit FormW=new FormWikiListItemEdit()) {
				FormW.WikiListCurName=WikiListCurName;
				FormW.ItemNum=WikiLists.AddItem(WikiListCurName);
				FormW.ListColumnHeaders=_listColumnHeaders;
				if(FormW.ShowDialog()!=DialogResult.OK) {
					//delete new item because dialog was not OK'ed.
					WikiLists.DeleteItem(FormW.WikiListCurName,FormW.ItemNum,FormW.ListColumnHeaders.ElementAtOrDefault(0)?.ColName);
					return;
				}
				itemNum=FormW.ItemNum;//capture itemNum to prevent marshall-by-reference warning
			}
			SetIsEdited();
			FillGrid();
			for(int i = 0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Cells[0].Text==itemNum.ToString()) {
					gridMain.ListGridRows[i].ColorBackG=Color.FromArgb(255,255,128);
					gridMain.ScrollToIndex(i);
				}
			}
		}

		private void butRenameList_Click(object sender,EventArgs e) {
			//Logic copied from FormWikiLists.butAdd_Click()---------------------
			string newListName;
			using(InputBox inputListName=new InputBox("New List Name")) {
				if(inputListName.ShowDialog()!=DialogResult.OK) {
					return;
				}
				//Format input as it would be saved in the database--------------------------------------------
				newListName=inputListName.textResult.Text.ToLower().Replace(" ","");
			}
			//Validate list name---------------------------------------------------------------------------
			if(string.IsNullOrEmpty(newListName)) {
				MsgBox.Show(this,"List name cannot be blank.");
				return;
			}
			if(DbHelper.isMySQLReservedWord(newListName)) {
				//Can become an issue when retrieving column header names.
				MsgBox.Show(this,"List name is a MySQL reserved word.");
				return;
			}
			if(WikiLists.CheckExists(newListName)) {
				MsgBox.Show(this,"List name already exists.");
				return;
			}
			try {
				Cursor=Cursors.WaitCursor;
				WikiLists.Rename(WikiListCurName,newListName);
				SetIsEdited();
				WikiListHists.Rename(WikiListCurName,newListName);
				WikiListCurName=newListName;
				FillGrid();
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			}
			finally {
				Cursor=Cursors.Default;
			}
		}

		///<summary>Sets the _isEdited bool to true and saves a copy in the wikilisthist table. This only happens once to prevent spamming of updates.</summary>
		private void SetIsEdited() {
			if(_isEdited || IsNew) {//Dont save a wikiListHist entry if this is a new list, or we have already saved an entry prior to a previous edit.
				return;
			}
			_wikiListOld.WikiListHistNum=WikiListHists.Insert(_wikiListOld);
			_isEdited=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.WikiListSetup)) {//gives a message box if no permission
				return;
			}
			if(gridMain.ListGridRows.Count>0) {
				MsgBox.Show(this,"Cannot delete a non-empty list.  Remove all items first and try again.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this entire list and all references to it?")) {
				return;
			}
			SetIsEdited();
			WikiLists.DeleteList(WikiListCurName);
			//Someday, if we have links to lists, then this is where we would update all the wikipages containing those links.  Remove links to data that was contained in the table.
			DialogResult=DialogResult.OK;
		}

		private void butHistory_Click(object sender,EventArgs e) {
			using(FormWikiListHistory FormWLH=new FormWikiListHistory()) {
				FormWLH.ListNameCur=WikiListCurName;
				FormWLH.ShowDialog();
				if(!FormWLH.IsReverted) {
					return;
				}
			}
			//Reversion has already saved a copy of the current revision.
			_wikiListOld=WikiListHists.GenerateFromName(WikiListCurName,Security.CurUser.UserNum);
			FillGrid();
			_isEdited=false;
			IsNew=false;
		}

		private void butAdvSearch_Click(object sender,EventArgs e) {
			if(!checkBoxIncludeHiddenColumns.Checked && _hasHiddenColumns) {
				MsgBox.Show(this,"Include hidden columns before using Advanced Search.");
				return;
			}
			using(FormWikiListAdvancedSearch FormWLAS=new FormWikiListAdvancedSearch(_listColumnHeaders)) {
				FormWLAS.SelectedColumnIndices=_arraySearchColIdxs;
				if(FormWLAS.ShowDialog()==DialogResult.OK) {
					_arraySearchColIdxs=FormWLAS.SelectedColumnIndices;
					FillGrid();
				}
			}
			ActiveControl=textSearch;
		}

		private void butClearAdvSearch_Click(object sender,EventArgs e) {
			_arraySearchColIdxs=new int[0];
			if(string.IsNullOrEmpty(textSearch.Text)) {
				FillGrid();//if no search text to clear, just re-fill grid
			}
			else {
				textSearch.Clear();//will trigger FillGrid if text changes, so no need to call again
			}
			ActiveControl=textSearch;
		}

		private void radioButHighlight_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioButFilter_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkBoxIncludeHiddenColumns_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}
	}
}