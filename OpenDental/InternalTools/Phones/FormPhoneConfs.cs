using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormPhoneConfs:FormODBase {

		public FormPhoneConfs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPhoneConf_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup,true)) {
				//Dwayne explicitly requested allowing users to click the Kick button and then to warn them of which permission is required.
				this.DisableAllExcept(butRefresh,checkHideEmpty,butKick,butClose);
			}
			FillGrid();
		}

		private void FillGrid() {
			List<PhoneConf> listPhoneConfs=PhoneConfs.GetAll();
			listPhoneConfs=listPhoneConfs.OrderBy(x => x.Extension).ToList();
			if(checkHideEmpty.Checked) {
				listPhoneConfs.RemoveAll(x => x.Occupants==0);
			}
			List<Site> listSites=Sites.GetDeepCopy();
			List<Userod> listUserods=Userods.GetDeepCopy();
			gridConfRoom.BeginUpdate();
			gridConfRoom.ListGridColumns.Clear();
			//No translations due to HQ only.
			gridConfRoom.ListGridColumns.Add(new GridColumn("Extension",70,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridConfRoom.ListGridColumns.Add(new GridColumn("MsgBtn",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridConfRoom.ListGridColumns.Add(new GridColumn("Site",100));
			gridConfRoom.ListGridColumns.Add(new GridColumn("UserReserved",100));
			gridConfRoom.ListGridColumns.Add(new GridColumn("DateTimeReserved",110,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
			gridConfRoom.ListGridColumns.Add(new GridColumn("Occupants",70,HorizontalAlignment.Center,GridSortingStrategy.AmountParse){ IsWidthDynamic=true });
			gridConfRoom.ListGridRows.Clear();
			GridRow row;
			foreach(PhoneConf conf in listPhoneConfs) {
				row=new GridRow();
				Site site=listSites.FirstOrDefault(x => x.SiteNum==conf.SiteNum);
				Userod user=listUserods.FirstOrDefault(x => x.UserNum==conf.UserNum);
				row.Cells.Add(conf.Extension.ToString());
				row.Cells.Add((conf.ButtonIndex < 0) ? "" : conf.ButtonIndex.ToString());
				row.Cells.Add((site==null) ? "" : site.Description);
				row.Cells.Add((user==null) ? "" : user.UserName);
				row.Cells.Add((conf.DateTimeReserved.Year < 1880) ? "" : conf.DateTimeReserved.ToShortDateString()+" "+conf.DateTimeReserved.ToShortTimeString());
				row.Cells.Add((conf.Occupants < 1) ? "" : conf.Occupants.ToString());
				row.Tag=conf;
				gridConfRoom.ListGridRows.Add(row);
			}
			gridConfRoom.EndUpdate();
		}

		private void gridConfRoom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormPhoneConfEdit FormPCE=new FormPhoneConfEdit((PhoneConf)gridConfRoom.ListGridRows[e.Row].Tag);
			FormPCE.ShowDialog();
			if(FormPCE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			PhoneConf phoneConf=new PhoneConf();
			phoneConf.ButtonIndex=-1;
			phoneConf.IsNew=true;
			using FormPhoneConfEdit FormPCE=new FormPhoneConfEdit(phoneConf);
			FormPCE.ShowDialog();
			if(FormPCE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridConfRoom.SelectedIndices.Length < 1) {
				MsgBox.Show(this,"Select conference rooms to delete.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete all selected conference rooms?")) {
				return;
			}
			List<long> listPhoneConfNums=new List<long>();
			foreach(int index in gridConfRoom.SelectedIndices) {
				listPhoneConfNums.Add(((PhoneConf)gridConfRoom.ListGridRows[index].Tag).PhoneConfNum);
			}
			PhoneConfs.DeleteMany(listPhoneConfNums);
			FillGrid();
		}

		private void butKick_Click(object sender,EventArgs e) {
			//Dwayne explicitly requested allowing users to click the Kick button and then to warn them of which permission is required.
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			if(gridConfRoom.SelectedIndices.Length < 1) {
				MsgBox.Show(this,"Select conference rooms to kick occupants from.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Kick occupants from all selected conference rooms?")) {
				return;
			}
			List<long> listPhoneConfNums=new List<long>();
			foreach(int index in gridConfRoom.SelectedIndices) {
				listPhoneConfNums.Add(((PhoneConf)gridConfRoom.ListGridRows[index].Tag).Extension);
			}
			PhoneConfs.KickConfRooms(listPhoneConfNums);
			MsgBox.Show(this,"Signals have been sent to kick occupants from the selected conference rooms.\r\n"
				+"Wait a few seconds for Asterisk to process the signals and then click Refresh.");
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}