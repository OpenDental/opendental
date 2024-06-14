using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmWikiListAdvancedSearch:FrmODBase {
		public int[] IntArrayColumnIndicesSelected=new int[0];
		private List<WikiListHeaderWidth> _listWikiListHeaderWidths;

		public FrmWikiListAdvancedSearch(List<WikiListHeaderWidth> listWikiListHeaderWidths):base() {
			InitializeComponent();
			_listWikiListHeaderWidths=listWikiListHeaderWidths;
			Load+=FrmWikiListAdvancedSearch_Load;
			PreviewKeyDown+=FrmWikiListAdvancedSearch_PreviewKeyDown;
		}

		private void FrmWikiListAdvancedSearch_Load(object sender,EventArgs e) {
			FillGrid();
		}

		/// <summary>Populates the grid with the current Wiki's column headers</summary>
		private void FillGrid() {
			Lang.F(this);
//Unable to select items in grid
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("",80);
			gridColumn.IsWidthDynamic=true;
			gridMain.Columns.Add(gridColumn);
			List<string> listColNames= _listWikiListHeaderWidths.Select(x => x.ColName).ToList();
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listWikiListHeaderWidths.Count;i++) {
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_listWikiListHeaderWidths[i].ColName);
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
			if(IntArrayColumnIndicesSelected.Length>0) {
				gridMain.SetSelected(IntArrayColumnIndicesSelected,true);
			}
		}

		private void FrmWikiListAdvancedSearch_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			IntArrayColumnIndicesSelected=gridMain.SelectedIndices;
			IsDialogOK=true;
		}

	}
}