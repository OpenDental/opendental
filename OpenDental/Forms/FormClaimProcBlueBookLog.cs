using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormClaimProcBlueBookLog:FormODBase {
		///<summary>List of InsBlueBookLogs for a ClaimProc.</summary>
		private List<InsBlueBookLog> _listInsBlueBookLogs=new List<InsBlueBookLog>();

		public FormClaimProcBlueBookLog(List<InsBlueBookLog> listInsBlueBookLogs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listInsBlueBookLogs=listInsBlueBookLogs;
		}

		private void FormClaimProcBlueBookLog_Load(object sender,EventArgs e) {
			_listInsBlueBookLogs=_listInsBlueBookLogs.OrderBy(x => x.DateTEntry).ToList();
			if(PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook) {
				labelBlueBookOff.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid() {
			gridInsBlueBookLog.BeginUpdate();
			gridInsBlueBookLog.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date"),70,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridInsBlueBookLog.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),60,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridInsBlueBookLog.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Insurance\r\nEstimate"),70,HorizontalAlignment.Right);
			gridInsBlueBookLog.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),0);
			col.IsWidthDynamic=true;
			gridInsBlueBookLog.ListGridColumns.Add(col);
			gridInsBlueBookLog.ListGridRows.Clear();
			GridRow row;
			foreach(InsBlueBookLog log in _listInsBlueBookLogs) {
				row=new GridRow();
				row.Cells.Add(log.DateTEntry.ToShortDateString());
				row.Cells.Add(log.DateTEntry.ToShortTimeString());
				row.Cells.Add(log.AllowedFee.ToString("f"));
				row.Cells.Add(log.Description);
				row.Tag=log;
				gridInsBlueBookLog.ListGridRows.Add(row);
			}
			gridInsBlueBookLog.EndUpdate();
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}