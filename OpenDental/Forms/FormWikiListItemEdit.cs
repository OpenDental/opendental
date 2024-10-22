using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiListItemEdit:FormODBase {
		///<summary>Name of the wiki list.</summary>
		public string WikiListName;
		public long ItemNum;
		public bool IsNew;
		public bool ShowHidden;
		///<summary>A list of all possible column headers for the current wiki list.  Each header contains additional information (e.g. PickList) that can be useful.</summary>
		public List<WikiListHeaderWidth> ListWikiListHeaderWidths;
		///<summary>Creating a data table containing only one item allows us to use column names.</summary>
		private DataTable _tableWikiList;

		public FormWikiListItemEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiListEdit_Load(object sender,EventArgs e) {
			_tableWikiList = WikiLists.GetItem(WikiListName,ItemNum,ListWikiListHeaderWidths.ElementAtOrDefault(0)?.ColName);
			//Show the PK in the title bar for informational purposes.  We don't put it in the grid because user can't change it.
			this.Text=this.Text+" - "+_tableWikiList.Columns[0]+" "+_tableWikiList.Rows[0][0].ToString();//OK to use 0 indices here. If this fails something else is wrong.
			FillGrid();
		}

		///<summary>Returns a lits of helper objects to fill the grid. Considers the ShowHidden boolean.</summary>
		private List<WikiListColumnComponent> GetWikiListGridRowComponents() {
			List<WikiListColumnComponent> listWikiListColumnComponents=new List<WikiListColumnComponent>();
			//The first WikiListHeaderWidth in the list will always be the PK Name information.
			//We only want to return the column headers that the user will be interacting with.
			//Also, the single row inside of the corresponding wikilist table will have columns that match 1:1 with these rows.
			//Therefore, we need to skip the first column in said table just like we skipped the first row in the header width table.
			/*  ===========================
				* wikilistheaderwidth.ColName
				* ===========================
				* ...
				* |---------------------------------------|
				* | wikilist_(User Defined Table Name)Num | <- ColumnPK to skip
				* |---------------------------------------|
				* | (User Defined Column #1)              |
				* |---------------------------------------|
				* | (User Defined Column #2)              |
				* |---------------------------------------|
				* | (User Defined Column #3)              |
				* |---------------------------------------|
				* ...
				* ====================================================================
				* Row of column values for "wikilist_(User Defined Table Name)" table.
				* ====================================================================
				* |----------|--------------------------------|--------------------------------|--------------------------------|
				* | ColumnPK | (User Defined Column #1) Value | (User Defined Column #2) Value | (User Defined Column #3) Value |
				* |----------|--------------------------------|--------------------------------|--------------------------------|
				*  ^Skip ColumnPK.
				*/
			//Start i at 1 to skip ColumnPK.
			for(int i=1;i<ListWikiListHeaderWidths.Count;i++) {
				if(!ShowHidden && ListWikiListHeaderWidths[i].IsHidden) {
					continue;//Do not show hidden headers.
				}
				//There is only one row in the corresponding wiki list table.
				//The columns in said table will always match the header width rows that we are looping through.
				//Thus, the wiki list table will always have a corresponding 'i' column and that is how these two are linked.
				string columnValue=_tableWikiList.Rows[0][i].ToString();
				//Create a new helper class that duct tapes these two entities together.
				listWikiListColumnComponents.Add(new WikiListColumnComponent(ListWikiListHeaderWidths[i],columnValue));
			}
			return listWikiListColumnComponents;
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			UI.GridColumn col;
			col=new UI.GridColumn(Lan.g(this,"Column"),200);
			gridMain.Columns.Add(col);
			col=new UI.GridColumn(Lan.g(this,"Value"),400,true);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			List<WikiListColumnComponent> listWikiListColumnComponents=GetWikiListGridRowComponents();
			for(int i=0;i<listWikiListColumnComponents.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(listWikiListColumnComponents[i].WikiListHeaderWidth.ColName);
				row.Cells.Add(listWikiListColumnComponents[i].ColumnValue);
				row.Tag=listWikiListColumnComponents[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.Title=Lan.g(this,"Edit List Item");
		}

		private void gridMain_CellEnter(object sender,ODGridClickEventArgs e) {
			WikiListColumnComponent wikiListColumnComponent=gridMain.SelectedTag<WikiListColumnComponent>();
			if(wikiListColumnComponent==null || string.IsNullOrEmpty(wikiListColumnComponent.WikiListHeaderWidth.PickList)) {
				comboEntry.Visible=false;
				return;
			}
			comboEntry.Items.Clear();
			List<string> listComboOptions=wikiListColumnComponent.WikiListHeaderWidth.PickList.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries).ToList();
			for(int i=0;i<listComboOptions.Count;i++) {
				comboEntry.Items.Add(listComboOptions[i]);
			}
			comboEntry.SelectedIndex=listComboOptions.FindIndex(x => x==wikiListColumnComponent.ColumnValue);
			//Hack together the location to put the combo box
			Point drawLoc=new Point();
			drawLoc.Y=gridMain.ListGridRows[e.Row].State.YPos+gridMain.Location.Y
				+15//gridMain.HeaderHeight
				+18//gridMain.TitleHeight
				+1;
			drawLoc.X=gridMain.Columns[0].ColWidth+gridMain.Location.X+1;
			comboEntry.Location=drawLoc;
			//Get the size to set the combo box to cover the item
			comboEntry.Width=gridMain.Columns[1].ColWidth+1;
			comboEntry.Height=(gridMain.ListGridRows[e.Row].State.HeightMain-1);
			comboEntry.Visible=true;
			comboEntry.Focus();
			comboEntry.DroppedDown=true;
			//Keep track of the ColName that this combo box represents. Used when updating _tableWikiList values.
			comboEntry.Tag=wikiListColumnComponent.WikiListHeaderWidth.ColName;
		}

		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			//We display information to the user as if the columns of _tableWikiList are actually the rows.
			//The first column in _tableWikiList is reserved for the title bar text for the window.
			//Therefore, the rows in the grid that the user is interacting with represent the columns following said title bar column.
			//The ODGridClickEventArgs passed in represent the cell that was just edited by the user. We need to update the table with that information.
			//Figure out which column in the table needs to be synchronized (see GetWikiListGridRowComponents() for details).
			string columnName=gridMain.ListGridRows[e.Row].Cells[0].Text;//Get the columnName for the gridMain row, so we can use it to identify the corresponding column in _tableWikiList.
			//Normalize invisible newline characters from the grid cell. All content following the first newline would be lost without normalizing.
			string gridCellValue=gridMain.ListGridRows[e.Row].Cells[1].Text.Replace("\r\n","\n").Replace("\n","\r\n");
			//Synchronize the table with the grid.
			_tableWikiList.Rows[0][columnName]=gridCellValue;
		}

		private void comboEntry_Leave(object sender,EventArgs e) {
			comboEntry.Visible=false;
			//Synchronize the table with the combobox selection.
			if(comboEntry.Tag!=null && comboEntry.SelectedItem!=null) {
				//We display this information to the user as if the columns of _tableWikiList are actually the rows.
				string columnName=comboEntry.Tag.ToString();
				_tableWikiList.Rows[0][columnName]=comboEntry.SelectedItem;
				comboEntry.Tag=null;
				comboEntry.SelectedItem=null;
				FillGrid();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.WikiListSetup)) {//might want to implement a new security permission.
				return;
			}
			//maybe require all empty or admin priv
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this list item and all references to it?")) {
				return;
			}
			WikiLists.DeleteItem(WikiListName,ItemNum,ListWikiListHeaderWidths.ElementAtOrDefault(0)?.ColName);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			List<string> listColumnNames=gridMain.GetTags<WikiListColumnComponent>().Select(x => x.WikiListHeaderWidth.ColName).ToList();
			if(listColumnNames.Count==0) {
				DialogResult=DialogResult.OK;
				return;
			}
			for(int i=0;i<listColumnNames.Count;i++) {
				//Normalize invisible newline characters. All content following the first newline would be lost without normalizing.
				_tableWikiList.Rows[0][listColumnNames[i]]=gridMain.ListGridRows[i].Cells[1].Text
					.Replace("\r\n","\n").Replace("\n","\r\n");
			}
			//Save data from grid into table.
			WikiLists.UpdateItem(WikiListName,_tableWikiList);
			DialogResult=DialogResult.OK;
		}

		///<summary>A helper class that keeps track of a wiki list column (header width) and it's corresponding value.</summary>
		private class WikiListColumnComponent {
			public WikiListHeaderWidth WikiListHeaderWidth;
			public string ColumnValue;

			public WikiListColumnComponent(WikiListHeaderWidth wikiListHeaderWidth,string columnValue) {
				WikiListHeaderWidth=wikiListHeaderWidth;
				ColumnValue=columnValue;
			}
		}
	}
}
