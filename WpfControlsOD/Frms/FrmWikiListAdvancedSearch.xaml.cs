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
			//Lan.F(this);
		}

		private void FrmWikiListAdvancedSearch_Loaded(object sender,RoutedEventArgs e) {
			FillGrid();
		}

		/// <summary>Populates the grid with the current Wiki's column headers</summary>
		private void FillGrid() {
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

		private void butOK_Click(object sender,EventArgs e) {
			IntArrayColumnIndicesSelected=gridMain.SelectedIndices;
			IsDialogOK=true;
		}

	}
}
