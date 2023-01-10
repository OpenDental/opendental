using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormMapAreaContainers:FormODBase {
		private List<MapAreaContainer> _listMapAreaContainers;

		public FormMapAreaContainers() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMapAreaContainers_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			_listMapAreaContainers=MapAreaContainers.Refresh();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Description",120);
			gridMain.Columns.Add(col);
			col=new GridColumn("Site",120);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listMapAreaContainers.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_listMapAreaContainers[i].Description);
				row.Cells.Add(Sites.GetDescription(_listMapAreaContainers[i].SiteNum));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMapAreaContainerEdit formMapAreaContainerEdit=new FormMapAreaContainerEdit();
			formMapAreaContainerEdit.MapAreaContainerCur=_listMapAreaContainers[e.Row];
			formMapAreaContainerEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			MapAreaContainer mapAreaContainer=new MapAreaContainer();
			mapAreaContainer.Description="new";
			mapAreaContainer.FloorWidthFeet=100;
			mapAreaContainer.FloorHeightFeet=70;
			MapAreaContainers.Insert(mapAreaContainer);
			mapAreaContainer.IsNew=true;//so we can delete if they hit cancel
			using FormMapAreaContainerEdit formMapAreaContainerEdit=new FormMapAreaContainerEdit();
			formMapAreaContainerEdit.MapAreaContainerCur=mapAreaContainer;
			formMapAreaContainerEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}