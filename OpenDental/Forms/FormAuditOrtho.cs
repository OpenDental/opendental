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
	public partial class FormAuditOrtho:FormODBase {
		///<summary>Should be passed in from calling function.</summary>
		public SortedDictionary<DateTime,List<SecurityLog>> DictDateOrthoLogs;
		public List<SecurityLog> ListSecurityLogs;

		public FormAuditOrtho() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			DictDateOrthoLogs=new SortedDictionary<DateTime,List<SecurityLog>>();
			ListSecurityLogs=new List<SecurityLog>();
		}

		private void FormAuditOrtho_Load(object sender,EventArgs e) {
			FillGridDates();
			FillGridMain();
		}

		private void FillGridDates() {
			gridHist.BeginUpdate();
			gridHist.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoAudit","Date"),70);
			gridHist.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","Entries"),50,HorizontalAlignment.Center);
			gridHist.Columns.Add(col);
			gridHist.ListGridRows.Clear();
			GridRow row;
			foreach(DateTime dt in DictDateOrthoLogs.Keys) {//must use foreach to enumerate through keys in the dictionary
				row=new GridRow();
				row.Cells.Add(dt.ToShortDateString());
				row.Cells.Add(DictDateOrthoLogs[dt].Count.ToString());
				row.Tag=dt;
				gridHist.ListGridRows.Add(row);
			}
			gridHist.EndUpdate();
			gridHist.ScrollToEnd();
			gridHist.SetAll(true);
		}

		private void FillGridMain() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoAudit","Date Time"),120,GridSortingStrategy.DateParse);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","User"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","Permission"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","Log Text"),569);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			Userod userod;
			//First Selected Ortho Chart Logs
			List<SecurityLog> listSecurityLogs=new List<SecurityLog>();
			foreach(int iDate in gridHist.SelectedIndices) {
				DateTime dateRow=(DateTime)gridHist.ListGridRows[iDate].Tag;
				if(!DictDateOrthoLogs.TryGetValue(dateRow,out List<SecurityLog> listSecurityLogsForDate)) {
					continue;
				}
				listSecurityLogs.AddRange(listSecurityLogsForDate);
			}
			listSecurityLogs=listSecurityLogs.OrderBy(x => x.LogDateTime).ToList();
			//Always add any patient field logs to the end of the list which needs to be odered by LogDateTime themselves.
			listSecurityLogs.AddRange(ListSecurityLogs.OrderBy(x => x.LogDateTime));
			for(int i=0;i<listSecurityLogs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listSecurityLogs[i].LogDateTime.ToShortDateString()+" "+listSecurityLogs[i].LogDateTime.ToShortTimeString());
				userod=Userods.GetUser(listSecurityLogs[i].UserNum);
				if(userod==null) {//Will be null for audit trails made by outside entities that do not require users to be logged in.  E.g. Web Sched.
					row.Cells.Add("unknown");
				}
				else {
					row.Cells.Add(userod.UserName);
				}
				row.Cells.Add(listSecurityLogs[i].PermType.ToString());
				row.Cells.Add(listSecurityLogs[i].LogText);
				row.Tag=listSecurityLogs[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollToEnd();
		}

		private void gridHist_CellClick(object sender,ODGridClickEventArgs e) {
			FillGridMain();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}