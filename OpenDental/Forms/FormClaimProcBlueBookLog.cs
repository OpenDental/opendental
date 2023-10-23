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
			gridInsBlueBookLog.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date"),70,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridInsBlueBookLog.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time"),60,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridInsBlueBookLog.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Insurance\r\nEstimate"),70,HorizontalAlignment.Right);
			gridInsBlueBookLog.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),0);
			col.IsWidthDynamic=true;
			gridInsBlueBookLog.Columns.Add(col);
			gridInsBlueBookLog.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listInsBlueBookLogs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listInsBlueBookLogs[i].DateTEntry.ToShortDateString());
				row.Cells.Add(_listInsBlueBookLogs[i].DateTEntry.ToShortTimeString());
				row.Cells.Add(_listInsBlueBookLogs[i].AllowedFee.ToString("f"));
				row.Cells.Add(_listInsBlueBookLogs[i].Description);
				row.Tag=_listInsBlueBookLogs[i];
				gridInsBlueBookLog.ListGridRows.Add(row);
			}
			gridInsBlueBookLog.EndUpdate();
		}

	}
}