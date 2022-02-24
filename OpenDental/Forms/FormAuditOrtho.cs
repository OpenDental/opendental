using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormAuditOrtho:FormODBase {
		///<summary>Should be passed in from calling function.</summary>
		public SortedDictionary<DateTime,List<SecurityLog>> DictDateOrthoLogs;
		public List<SecurityLog> PatientFieldLogs;

		public FormAuditOrtho() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			DictDateOrthoLogs=new SortedDictionary<DateTime,List<SecurityLog>>();
			PatientFieldLogs=new List<SecurityLog>();
		}

		private void FormAuditOrtho_Load(object sender,EventArgs e) {
			FillGridDates();
			FillGridMain();
		}

		private void FillGridDates() {
			gridHist.BeginUpdate();
			gridHist.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoAudit","Date"),70);
			gridHist.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","Entries"),50,HorizontalAlignment.Center);
			gridHist.ListGridColumns.Add(col);
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
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoAudit","Date Time"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","User"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","Permission"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoAudit","Log Text"),569);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			Userod user;
			//First Selected Ortho Chart Logs
			foreach(int iDate in gridHist.SelectedIndices) {
				DateTime dateRow=(DateTime)gridHist.ListGridRows[iDate].Tag;
				if(!DictDateOrthoLogs.ContainsKey(dateRow)){
					continue;
				}
				for(int i=0;i<DictDateOrthoLogs[dateRow].Count;i++) {
					row=new GridRow();
					row.Cells.Add(DictDateOrthoLogs[dateRow][i].LogDateTime.ToShortDateString()+" "+DictDateOrthoLogs[dateRow][i].LogDateTime.ToShortTimeString());
					user=Userods.GetUser(DictDateOrthoLogs[dateRow][i].UserNum);
					if(user==null) {//Will be null for audit trails made by outside entities that do not require users to be logged in.  E.g. Web Sched.
						row.Cells.Add("unknown");
					}
					else {
						row.Cells.Add(user.UserName);
					}
					row.Cells.Add(DictDateOrthoLogs[dateRow][i].PermType.ToString());
					row.Cells.Add(DictDateOrthoLogs[dateRow][i].LogText);
					gridMain.ListGridRows.Add(row);
				}
			}
			//Then any applicable patient field logs.
			for(int i=0;i<PatientFieldLogs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(PatientFieldLogs[i].LogDateTime.ToShortDateString()+" "+PatientFieldLogs[i].LogDateTime.ToShortTimeString());
				row.Cells.Add(Userods.GetUser(PatientFieldLogs[i].UserNum).UserName);
				row.Cells.Add(PatientFieldLogs[i].PermType.ToString());
				row.Cells.Add(PatientFieldLogs[i].LogText);
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