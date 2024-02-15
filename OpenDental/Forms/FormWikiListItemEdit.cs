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

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Column"),200));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Value"),400,true));
			gridMain.ListGridRows.Clear();
			gridMain.ListGridRows.AddRange(_tableWikiList.Columns.OfType<DataColumn>().Skip(1).Select(x => new GridRow(x.ColumnName,_tableWikiList.Rows[0][x].ToString())));
			gridMain.EndUpdate();
			gridMain.Title=Lan.g(this,"Edit List Item");
		}

		private void gridMain_CellEnter(object sender,ODGridClickEventArgs e) {
			if(ListWikiListHeaderWidths==null || ListWikiListHeaderWidths.Count<=e.Row+1 || string.IsNullOrEmpty(ListWikiListHeaderWidths[e.Row+1].PickList)) {
				comboEntry.Visible=false;
				return;
			}
			comboEntry.Items.Clear();
			//add 1 to e.Row because the primary key value isn't drawn.
			List<string> listComboOptions=ListWikiListHeaderWidths[e.Row+1].PickList.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries).ToList();
			for(int i=0;i<listComboOptions.Count;i++) {
				comboEntry.Items.Add(listComboOptions[i]);
			}
			comboEntry.SelectedIndex=listComboOptions.FindIndex(x => x==_tableWikiList.Rows[0][e.Row+1].ToString());
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
			comboEntry.Tag=(int)e.Row+1;
		}

		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			//Save data from grid into table. No call to DB, so this should be safe.
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				_tableWikiList.Rows[0][i+1]=gridMain.ListGridRows[i].Cells[1].Text.Replace("\r\n","\n").Replace("\n","\r\n");//Column 0 of TableItems.Rows[0] is in the title bar, so it is off from the grid by 1.
			}
		}

		private void comboEntry_Leave(object sender,EventArgs e) {
			comboEntry.Visible=false;
			//Save any previous combo box values if present.
			if(comboEntry.Tag!=null && comboEntry.SelectedItem!=null) {
				//We display this information to the user as if the columns of _tableWikiList are actually the rows.
				_tableWikiList.Rows[0][(int)comboEntry.Tag]=comboEntry.SelectedItem;
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
			WikiLists.UpdateItem(WikiListName,_tableWikiList);
			DialogResult=DialogResult.OK;
		}

	}
}
