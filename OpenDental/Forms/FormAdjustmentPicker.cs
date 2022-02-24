using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormAdjustmentPicker:FormODBase {
		private bool _isUnattachedMode;
		private long _patNum;
		private List<Adjustment> _listAdjustments;
		private List<Adjustment> _listAdjustmentsFiltered;
		public Adjustment SelectedAdjustment;

		public FormAdjustmentPicker(long patNum,bool isUnattachedMode=false,List<Adjustment> listAdjustments=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
			_isUnattachedMode=isUnattachedMode;
			_listAdjustments=listAdjustments;
		}

		private void FormAdjustmentPicker_Load(object sender,EventArgs e) {
			if(_isUnattachedMode) {
				checkUnattached.Checked=true;
				checkUnattached.Enabled=false;
			}
			if(_listAdjustments==null) {
				_listAdjustments=Adjustments.Refresh(_patNum).ToList();
			}
			//Because this window is only opened when linking adjustments, and we cannot link those associated with a paysplit, we filter those out.
			List<PaySplit> listPaySplits=PaySplits.GetForAdjustments(_listAdjustments.Select(x => x.AdjNum).ToList());
			_listAdjustments.RemoveAll(x => listPaySplits.Exists(y => y.AdjNum==x.AdjNum));
			FillGrid();
		}

		private void FillGrid(){
			_listAdjustmentsFiltered=_listAdjustments;
			if(checkUnattached.Checked) {
				_listAdjustmentsFiltered=_listAdjustments.FindAll(x => x.ProcNum==0);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PatNum"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),70);
			gridMain.ListGridColumns.Add(col);
			if(!_isUnattachedMode) { 
				col=new GridColumn(Lan.g(this,"Has Proc"),0,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(Adjustment adjCur in _listAdjustmentsFiltered) {
				row=new GridRow();
				row.Cells.Add(adjCur.AdjDate.ToShortDateString());
				row.Cells.Add(adjCur.PatNum.ToString());
				row.Cells.Add(Defs.GetName(DefCat.AdjTypes,adjCur.AdjType));
				row.Cells.Add(adjCur.AdjAmt.ToString("F"));
				string attachedProc="";
				if(adjCur.ProcNum!=0){
					attachedProc="X";
				}
				if(!_isUnattachedMode) { 
					row.Cells.Add(attachedProc);
				}
				row.Tag=adjCur;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void checkUnattached_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectedAdjustment=_listAdjustmentsFiltered[gridMain.GetSelectedIndex()];
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			int index=gridMain.GetSelectedIndex();
			if(index<0) {
				MsgBox.Show("You must select an adjustment.");
				return;
			}
			SelectedAdjustment=_listAdjustmentsFiltered[index];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}