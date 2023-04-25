using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This form is for a HQ only feature for tracking historical Commlog data.</summary>
	public partial class FormCommItemHistory:FormODBase {

		private List<CommlogHist> _listCommlogHists;
		private long _commlogNum;

		public FormCommItemHistory(long commlogNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_commlogNum=commlogNum;
		}

		private void FormCommItemHistory_Load(object sender,EventArgs e) {
			_listCommlogHists=CommlogHists.GetAllForCommlog(_commlogNum);
			FillGrid();
		}

		private void FillGrid() {
			gridCommlogHist.BeginUpdate();
			gridCommlogHist.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableCommlogHist","Date Time"),140,HorizontalAlignment.Center);
			gridCommlogHist.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommlogHist","Hist Source"),80);
			gridCommlogHist.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommlogHist","Cust. Phone Raw"),80);
			gridCommlogHist.Columns.Add(col);
			gridCommlogHist.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listCommlogHists.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listCommlogHists[i].DateTStamp.ToString());//We want the time this CommlogHist 'Snapshot' was created
				row.Cells.Add(_listCommlogHists[i].HistSource.ToString());
				row.Cells.Add(_listCommlogHists[i].CustomerNumberRaw);
				row.Tag=_listCommlogHists[i];
				gridCommlogHist.ListGridRows.Add(row);
			}
			gridCommlogHist.EndUpdate();
		}

		private void gridCommlogHist_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(gridCommlogHist.SelectedTag<CommlogHist>()==null) {
				return;
			}
			Commlog commlog=CommlogHist.ConvertToCommlog(gridCommlogHist.SelectedTag<CommlogHist>());
			using FormCommItem formCommItem=new FormCommItem(commlog);
			formCommItem.IsHistoric=true;
			formCommItem.ShowDialog();
		}

		private void gridCommlogHist_SelectionCommitted(object sender,EventArgs e) {
			textNote.Clear();
			if(gridCommlogHist.SelectedTag<CommlogHist>()!=null) {
				textNote.Text=gridCommlogHist.SelectedTag<CommlogHist>().Note;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}