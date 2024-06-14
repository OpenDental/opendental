using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary>This form is for a HQ only feature for tracking historical Commlog data.</summary>
	public partial class FrmCommItemHistory:FrmODBase {
		private List<CommlogHist> _listCommlogHists;
		private long _commlogNum;

		public FrmCommItemHistory(long commlogNum) {
			InitializeComponent();
			_commlogNum=commlogNum;
			Load+=FrmCommItemHistory_Load;
			gridCommlogHist.CellDoubleClick+=gridCommlogHist_CellDoubleClick;
			gridCommlogHist.SelectionCommitted+=gridCommlogHist_SelectionCommitted;
		}

		private void FrmCommItemHistory_Load(object sender,EventArgs e) {
			Lang.F(this);
			_listCommlogHists=CommlogHists.GetAllForCommlog(_commlogNum);
			FillGrid();
		}

		private void FillGrid() {
			gridCommlogHist.BeginUpdate();
			gridCommlogHist.Columns.Clear();
			GridColumn col=new GridColumn(Lang.g("TableCommlogHist","Date Time"),140,HorizontalAlignment.Center);
			gridCommlogHist.Columns.Add(col);
			col=new GridColumn(Lang.g("TableCommlogHist","Hist Source"),80);
			gridCommlogHist.Columns.Add(col);
			col=new GridColumn(Lang.g("TableCommlogHist","Cust. Phone Raw"),80);
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

		private void gridCommlogHist_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(gridCommlogHist.SelectedTag<CommlogHist>()==null) {
				return;
			}
			Commlog commlog=CommlogHist.ConvertToCommlog(gridCommlogHist.SelectedTag<CommlogHist>());
			FrmCommItem frmCommItem=new FrmCommItem(commlog);
			frmCommItem.IsHistoric=true;
			frmCommItem.ShowDialog();
		}

		private void gridCommlogHist_SelectionCommitted(object sender,EventArgs e) {
			textNote.textBox.Clear();
			CommlogHist commlogHist=gridCommlogHist.SelectedTag<CommlogHist>();
			if(commlogHist!=null) {
				textNote.Text=commlogHist.Note;
			}
		}

	}
}