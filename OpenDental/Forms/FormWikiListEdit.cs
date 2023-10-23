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
		private WikiListHist _wikiListHistOld;
		private bool _isEdited;
		private int[] _intArrayIdxsSearchCol=new int[0];
		///<summary>A list of all possible column headers for the current wiki list.  Each header contains additional information (e.g. PickList) that can be useful.</summary>
		private List<WikiListHeaderWidth> _listWikiListHeaderWidths=new List<WikiListHeaderWidth>();
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
			_wikiListHistOld=WikiListHists.GenerateFromName(WikiListCurName,Security.CurUser.UserNum)??new WikiListHist();
			FillGrid();
			ActiveControl=textSearch;//start in search box.
		}

		///<summary>Fills the grid with the contents of the corresponding wiki list table in the database.
		///After filling the grid, FilterGrid() will get invoked to apply any advanced search options.</summary>
		private void FillGrid() {
			_listWikiListHeaderWidths=WikiListHeaderWidths.GetForList(WikiListCurName);
			_table=WikiLists.GetByName(WikiListCurName);
			if(_table.Rows.Count>0 && _listWikiListHeaderWidths.Count!=_table.Columns.Count) {//if these do not match, something happened to be desynched at the right moment.
				WikiListHeaderWidths.RefreshCache();
				_table=WikiLists.GetByName(WikiListCurName);
				_listWikiListHeaderWidths=WikiListHeaderWidths.GetForList(WikiListCurName);
				if(_listWikiListHeaderWidths.Count!=_table.Columns.Count) {//if they still do not match, one of them did not get synched correctly.
					MsgBox.Show(this,"Unable to open the wiki list.");
					return;
				}
			}
			_hasHiddenColumns=_listWikiListHeaderWidths.Any(x => x.IsHidden);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			//add all visible headers as grid columns
			for(int i=0;i<_listWikiListHeaderWidths.Count;i++) {
				if(checkBoxIncludeHiddenColumns.Checked || !_listWikiListHeaderWidths[i].IsHidden) {//only add columns that should be visible right now
					GridColumn col=new GridColumn(_listWikiListHeaderWidths[i].ColName,_listWikiListHeaderWidths[i].ColWidth);
					if(_listWikiListHeaderWidths[i].IsHidden) {
						col.Heading+=$" ({Lan.g(this,"hidden")})";
					}
					gridMain.Columns.Add(col);
				}
			}
			gridMain.ListGridRows.Clear();
			DataRow[] dataRowArray=_table.Select();
			for(int dataRowIndex=0;dataRowIndex<dataRowArray.Length;dataRowIndex++) {
				//only include currently-visible cells in this row
				string[] stringArrayVisibleCells=dataRowArray[dataRowIndex].ItemArray
					.Where((_,index) => checkBoxIncludeHiddenColumns.Checked || !_listWikiListHeaderWidths[index].IsHidden)
					.Select(x => x.ToString())
					.ToArray();
				gridMain.ListGridRows.Add(new GridRow(stringArrayVisibleCells) { Tag=dataRowIndex });
			}
			gridMain.Title=WikiListCurName;
			gridMain.EndUpdate();
			FilterGrid();
		}

		///<summary>Visually filters gridMain.  Tag is preserved so that double clicking and editing can still work.</summary>
		private void FilterGrid() {
			labelSearch.Text=Lan.g(this,"Search");
			labelSearch.ForeColor=Color.Black;
			List<string> listSearchTerms=textSearch.Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();
			if(!_intArrayIdxsSearchCol.IsNullOrEmpty()) {//adv search has been used, search specific columns selected
				if(!checkBoxIncludeHiddenColumns.Checked && _hasHiddenColumns) {//don't allow advanced search with hidden columns
					_intArrayIdxsSearchCol=new int[0];
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
				string[] stringArrayCellVals=_table.Rows[i].ItemArray
					.Where((_,index) => checkBoxIncludeHiddenColumns.Checked || !_listWikiListHeaderWidths[index].IsHidden)
					.Select(y => y.ToString()).ToArray();
				if((_intArrayIdxsSearchCol.IsNullOrEmpty()//not advanced search, so compare to all cell values
						&& !listSearchTerms.Any(x => stringArrayCellVals.Any(y => y.ToUpper().Contains(x.ToUpper()))))
					|| (!_intArrayIdxsSearchCol.IsNullOrEmpty()//advanced search, only compare to selected column cell values
						&& !listSearchTerms.Any(x => _intArrayIdxsSearchCol.Any(y => stringArrayCellVals[y].ToUpper().Contains(x.ToUpper())))))
				{
					continue;
				}
				//matching row
				if(radioButFilter.Checked) {
					gridMain.ListGridRows.Add(new GridRow(stringArrayCellVals) { Tag=i });
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
			using FormWikiListItemEdit formWikiListItemEdit=new FormWikiListItemEdit(); 
			formWikiListItemEdit.WikiListName=WikiListCurName;
			formWikiListItemEdit.ItemNum=PIn.Long(_table.Rows[(int)gridMain.ListGridRows[e.Row].Tag][0].ToString());
			formWikiListItemEdit.ListWikiListHeaderWidths=_listWikiListHeaderWidths;
			//saving occurs from within the form.
			if(formWikiListItemEdit.ShowDialog()!=DialogResult.OK) {
				return;
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
			if(!Security.IsAuthorized(EnumPermType.WikiListSetup)) {//gives a message box if no permission
				return false;
			}
			if(checkBoxIncludeHiddenColumns.Checked || !_hasHiddenColumns) {
				return true;
			}
			if(isDelete) {
				MsgBox.Show(this,"Include hidden columns before deleting one.");
				return false;
			}
			MsgBox.Show(this,"Include hidden columns before moving one.");
			return false;
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
			if(gridMain.SelectedCell.X>gridMain.Columns.Count-2) {//can't shift the last column right
				return;
			}
			SetIsEdited();
			Point pointNewSelectedCell=gridMain.SelectedCell;
			pointNewSelectedCell.X=Math.Min(gridMain.Columns.Count-1,pointNewSelectedCell.X+1);
			WikiLists.ShiftColumnRight(WikiListCurName,_table.Columns[gridMain.SelectedCell.X].ColumnName);
			FillGrid();
			gridMain.SetSelected(pointNewSelectedCell);
		}

		private void butColumnEdit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.WikiListSetup)) {//gives a message box if no permission
				return;
			}
			using FormWikiListHeaders formWikiListHeaders=new FormWikiListHeaders(WikiListCurName);
			if(formWikiListHeaders.ShowDialog()!=DialogResult.OK) {
				return;
			}
			SetIsEdited();
			FillGrid();
		}

		private void butColumnAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.WikiListSetup)) {//gives a message box if no permission
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
			using FormWikiListItemEdit formWikiListItemEdit=new FormWikiListItemEdit(); 
			formWikiListItemEdit.WikiListName=WikiListCurName;
			formWikiListItemEdit.ItemNum=WikiLists.AddItem(WikiListCurName);
			formWikiListItemEdit.ListWikiListHeaderWidths=_listWikiListHeaderWidths;
			if(formWikiListItemEdit.ShowDialog()!=DialogResult.OK) {
				//delete new item because dialog was not OK'ed.
				WikiLists.DeleteItem(formWikiListItemEdit.WikiListName,formWikiListItemEdit.ItemNum,formWikiListItemEdit.ListWikiListHeaderWidths.ElementAtOrDefault(0)?.ColName);
				return;
			}
			itemNum=formWikiListItemEdit.ItemNum;//capture itemNum to prevent marshall-by-reference warning
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
			using InputBox inputBox=new InputBox("New List Name"); 
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//Format input as it would be saved in the database--------------------------------------------
			newListName=inputBox.textResult.Text.ToLower().Replace(" ","");
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
			Cursor=Cursors.WaitCursor;
			try {
				WikiLists.Rename(WikiListCurName,newListName);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(this,ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			SetIsEdited();
			WikiListHists.Rename(WikiListCurName,newListName);
			WikiListCurName=newListName;
			FillGrid();
		}

		///<summary>Sets the _isEdited bool to true and saves a copy in the wikilisthist table. This only happens once to prevent spamming of updates.</summary>
		private void SetIsEdited() {
			if(_isEdited || IsNew) {//Dont save a wikiListHist entry if this is a new list, or we have already saved an entry prior to a previous edit.
				return;
			}
			_wikiListHistOld.WikiListHistNum=WikiListHists.Insert(_wikiListHistOld);
			_isEdited=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.WikiListSetup)) {//gives a message box if no permission
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
			using FormWikiListHistory formWikiListHistory=new FormWikiListHistory(); 
			formWikiListHistory.WikiListName=WikiListCurName;
			formWikiListHistory.ShowDialog();
			if(!formWikiListHistory.IsReverted) {
				return;
			}
			//Reversion has already saved a copy of the current revision.
			_wikiListHistOld=WikiListHists.GenerateFromName(WikiListCurName,Security.CurUser.UserNum);
			FillGrid();
			_isEdited=false;
			IsNew=false;
		}

		private void butAdvSearch_Click(object sender,EventArgs e) {
			if(!checkBoxIncludeHiddenColumns.Checked && _hasHiddenColumns) {
				MsgBox.Show(this,"Include hidden columns before using Advanced Search.");
				return;
			}
			FrmWikiListAdvancedSearch frmWikiListAdvancedSearch=new FrmWikiListAdvancedSearch(_listWikiListHeaderWidths);
			frmWikiListAdvancedSearch.IntArrayColumnIndicesSelected=_intArrayIdxsSearchCol;
			frmWikiListAdvancedSearch.ShowDialog();
			if(frmWikiListAdvancedSearch.IsDialogOK) {
				_intArrayIdxsSearchCol=frmWikiListAdvancedSearch.IntArrayColumnIndicesSelected;
				FillGrid();
			}
			ActiveControl=textSearch;
		}

		private void butClearAdvSearch_Click(object sender,EventArgs e) {
			_intArrayIdxsSearchCol=new int[0];
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