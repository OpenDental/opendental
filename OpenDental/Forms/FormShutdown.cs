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
		private List<Userod> _listUserods;

		public FormShutdown() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormShutdown_Load(object sender,EventArgs e) {
			_listComputers=Computers.GetDeepCopy();
			_listUserods=Userods.GetDeepCopy();
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
			gridActiveInstances.Columns.Clear();
			gridActiveInstances.Columns.Add(new GridColumn(Lan.g(this,"Computer Name"),120));
			gridActiveInstances.Columns.Add(new GridColumn(Lan.g(this,"User"),110));
			gridActiveInstances.Columns.Add(new GridColumn(Lan.g(this,"Last Active"),140,GridSortingStrategy.DateParse));
			gridActiveInstances.Columns.Add(new GridColumn(Lan.g(this,"Connection Type"),100));
			//Rows
			gridActiveInstances.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listActiveInstances.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listComputers.FirstOrDefault(x => x.ComputerNum==_listActiveInstances[i].ComputerNum)?.CompName??"UNKNOWN");
				row.Cells.Add(_listUserods.FirstOrDefault(x => x.UserNum==_listActiveInstances[i].UserNum)?.UserName??"");
				row.Cells.Add(_listActiveInstances[i].DateTimeLastActive.ToString());
				row.Cells.Add(_listActiveInstances[i].ConnectionType.ToString());
				if(_listActiveInstances[i].ActiveInstanceNum==ActiveInstances.GetActiveInstance().ActiveInstanceNum) {
					row.ColorBackG=Color.FromArgb(255,255,252,138);
				}
				row.Tag=_listActiveInstances[i];
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
			List<ActiveInstance> listActiveInstances=gridActiveInstances.SelectedTags<ActiveInstance>()
				.Where(x => x.ActiveInstanceNum!=ActiveInstances.GetActiveInstance().ActiveInstanceNum).ToList();
			SecurityLogs.MakeLogEntry(Permissions.CloseOtherSessions,0,"User "+Security.CurUser.UserName
				+" ended "+listActiveInstances.Count+" sessions via Shutdown Workstations.");
			ActiveInstances.CloseActiveInstances(listActiveInstances);
			_listActiveInstances.RemoveAll(x => listActiveInstances.Select(y => y.ActiveInstanceNum).Contains(x.ActiveInstanceNum));
			FillGrid();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}