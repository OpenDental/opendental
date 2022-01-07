using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormShutdown:FormODBase {
		///<summary>Set to true if part of the update process. Makes it behave more discretely to avoid worrying people.</summary>
		public bool IsUpdate;
		private List<ActiveInstance> _listActiveInstances;
		private List<Computer> _listComputers;
		private List<Userod> _listUsers;

		public FormShutdown() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormShutdown_Load(object sender,EventArgs e) {
			_listComputers=Computers.GetDeepCopy();
			_listUsers=Userods.GetDeepCopy();
			_listActiveInstances=ActiveInstances.GetAllResponsiveActiveInstances().OrderByDescending(x => x.DateTimeLastActive).ToList();
			if(ODBuild.IsWeb() && !Security.IsAuthorized(Permissions.CloseOtherSessions)) {
				butCloseSessions.Enabled=false;
			}
			if(IsUpdate) {
				butShutdown.Text=Lan.g(this,"Continue");
				butCloseSessions.Enabled=false;
			}
			FillGrid();
		}

		private void FillGrid() {
			gridActiveInstances.BeginUpdate();
			//Columns
			gridActiveInstances.ListGridColumns.Clear();
			gridActiveInstances.ListGridColumns.Add(new GridColumn(Lan.g(this,"Computer Name"),120));
			gridActiveInstances.ListGridColumns.Add(new GridColumn(Lan.g(this,"User"),110));
			gridActiveInstances.ListGridColumns.Add(new GridColumn(Lan.g(this,"Last Active"),140,GridSortingStrategy.DateParse));
			gridActiveInstances.ListGridColumns.Add(new GridColumn(Lan.g(this,"Connection Type"),100));
			//Rows
			gridActiveInstances.ListGridRows.Clear();
			GridRow row;
			foreach(ActiveInstance ai in _listActiveInstances) {
				row=new GridRow();
				row.Cells.Add(_listComputers.FirstOrDefault(x => x.ComputerNum==ai.ComputerNum)?.CompName??"UNKNOWN");
				row.Cells.Add(_listUsers.FirstOrDefault(x => x.UserNum==ai.UserNum)?.UserName??"");
				row.Cells.Add(ai.DateTimeLastActive.ToString());
				row.Cells.Add(ai.ConnectionType.ToString());
				if(ai.ActiveInstanceNum==ActiveInstances.CurrentActiveInstance.ActiveInstanceNum) {
					row.ColorBackG=Color.FromArgb(255,255,252,138);
				}
				row.Tag=ai;
				gridActiveInstances.ListGridRows.Add(row);
			}
			gridActiveInstances.EndUpdate();
		}


		private void butShutdown_Click(object sender,EventArgs e) {
			if(IsUpdate) {
				DialogResult=DialogResult.OK;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Shutdown this program on all workstations except this one? Users will be given a 15 second warning to save data.")) {
				return;
			}
			//happens outside this form
			DialogResult=DialogResult.OK;
		}

		private void butCloseSessions_Click(object sender,EventArgs e) {
			if(gridActiveInstances.SelectedTags<ActiveInstance>().Count==0) {
				MsgBox.Show(this,"Please select a session to close.");
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,Lan.g(this,"Shutdown selected session(s)? Users will be given a 15 second warning to save data."))) {
				return;
			}
			List<ActiveInstance> listInstances=gridActiveInstances.SelectedTags<ActiveInstance>()
				.Where(x => x.ActiveInstanceNum!=ActiveInstances.CurrentActiveInstance.ActiveInstanceNum).ToList();
			SecurityLogs.MakeLogEntry(Permissions.CloseOtherSessions,0,"User "+Security.CurUser.UserName
				+" ended "+listInstances.Count+" sessions via Shutdown Workstations.");
			ActiveInstances.CloseActiveInstances(listInstances);
			_listActiveInstances.RemoveAll(x => listInstances.Select(y => y.ActiveInstanceNum).Contains(x.ActiveInstanceNum));
			FillGrid();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}