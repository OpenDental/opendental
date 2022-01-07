using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCloudManagement:FormODBase {
		private List<ActiveInstance> _listActiveInstances;
		private List<Computer> _listComputers;
		private List<Userod> _listUsers;
		private List<CloudAddress> _listCloudAddresses;

		public FormCloudManagement() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCloudManagement_Load(object sender,EventArgs e) {
			labelMaxSessionNum.Text=PrefC.GetString(PrefName.CloudSessionLimit);
			//TODO: Fetch the maximum storage size and display (leave this for a future job)
			_listComputers=Computers.GetDeepCopy();
			_listUsers=Userods.GetDeepCopy();
			_listActiveInstances=ActiveInstances.GetAllResponsiveActiveInstances()
				.Where(x => x.ConnectionType==ConnectionTypes.ODCloud)
				.ToList();
			_listCloudAddresses=CloudAddresses.GetAll();
			try {
				string filePath=FileAtoZ.GetFilesInDirectoryRelative("cloudbackupsize")
					.FirstOrDefault(x => !string.IsNullOrEmpty(Path.GetExtension(x)));
				labelCurAtoZSize.Text=(string.IsNullOrEmpty(filePath)?"":FileAtoZ.ReadAllText(filePath)).Trim()+" GB";
			}
			catch {
				labelCurAtoZSize.Text="Error";
			}
			FillGridInstances();
			FillGridAddresses();
		}

		private void FillGridInstances() {
			labelCurSessionNum.Text=_listActiveInstances.Count().ToString();
			gridActiveInstances.BeginUpdate();
			//Columns
			gridActiveInstances.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"User"),100);
			gridActiveInstances.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Computer Name"),200);
			gridActiveInstances.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Last Active"),200);
			gridActiveInstances.ListGridColumns.Add(col);
			//Rows
			gridActiveInstances.ListGridRows.Clear();
			GridRow row;
			foreach(ActiveInstance ai in _listActiveInstances) {
				row=new GridRow();
				row.Cells.Add(_listUsers.FirstOrDefault(x => x.UserNum==ai.UserNum).UserName);
				if (_listComputers.Exists(x => x.ComputerNum==ai.ComputerNum)) {
					row.Cells.Add(_listComputers.FirstOrDefault(x => x.ComputerNum==ai.ComputerNum).CompName);
				}
				else {
					row.Cells.Add("UNKNOWN");
				}				
				row.Cells.Add(ai.DateTimeLastActive.ToString());
				row.Tag=ai;
				gridActiveInstances.ListGridRows.Add(row);
			}
			gridActiveInstances.SortForced(1,true);
			gridActiveInstances.EndUpdate();
		}

		private void FillGridAddresses() {
			gridAllowedAddresses.BeginUpdate();
			gridAllowedAddresses.ListGridColumns.Clear();
			gridAllowedAddresses.ListGridColumns.Add(new GridColumn(Lan.g(this,"IP Address"), 95));
			gridAllowedAddresses.ListGridColumns.Add(new GridColumn(Lan.g(this,"Last User"), 140));
			gridAllowedAddresses.ListGridColumns.Add(new GridColumn(Lan.g(this,"Last Connect"), 140, GridSortingStrategy.DateParse));
			gridAllowedAddresses.ListGridRows.Clear();
			DateTime date;
			GridRow row;
			foreach(CloudAddress address in _listCloudAddresses) {
				date=address.DateTimeLastConnect;
				row=new GridRow();
				row.Cells.Add(address.IpAddress);
				row.Cells.Add(Userods.GetName(address.UserNumLastConnect));
				row.Cells.Add(date.Year<1880?"Never":date.ToString());
				row.Tag=address;
				gridAllowedAddresses.ListGridRows.Add(row);
			}
			gridAllowedAddresses.EndUpdate();
		}

		private void butAddSessions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormCloudSessionLimit formCloudSessionLimit=new FormCloudSessionLimit();
			if(formCloudSessionLimit.ShowDialog()==DialogResult.OK) {
				labelMaxSessionNum.Text=PrefC.GetString(PrefName.CloudSessionLimit);
			}
		}

		private void butCloseSession_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.CloseOtherSessions)){
				return;
			}
			ActiveInstance ai=gridActiveInstances.SelectedTag<ActiveInstance>();
			if(ai==null) {
				MsgBox.Show(this,"Please select a session to close.");
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,Lan.g(this,"Are you sure you want to close the selected session?"))) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.CloseOtherSessions,0,"Session for "+_listUsers.Find(x => x.UserNum==ai.UserNum).UserName
				+" on Computer "+_listComputers.Find(x => x.ComputerNum==ai.ComputerNum)?.CompName??"UNKNOWN"
				+" was ended via Cloud Management.");
			Signalods.SetInvalid(InvalidType.ActiveInstance,KeyType.Undefined,ai.ActiveInstanceNum);
			_listActiveInstances.Remove(ai);
			FillGridInstances();
		}

		private void butChangePassword_Click(object sender,EventArgs e) {
			using FormChangeCloudPassword formChangeCloudPassword=new FormChangeCloudPassword();
			formChangeCloudPassword.ShowDialog();
		}

		///<summary>Deletes all selected IP addresses from the list of allowed addresses.</summary>
		private void butDeleteAddress_Click(object sender,EventArgs e) {
			//Remove the selected rows from the grid
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			CloudAddresses.DeleteMany(_listCloudAddresses
				.Where(x => gridAllowedAddresses.SelectedTags<CloudAddress>().Any(y => y.IpAddress==x.IpAddress))
				.Select(x => x.CloudAddressNum).ToList());
			_listCloudAddresses.RemoveAll(x => gridAllowedAddresses.SelectedTags<CloudAddress>().Any(y => y.IpAddress==x.IpAddress));
			FillGridAddresses();
		}

		///<summary>Adds the current IP address to the list, sets the last user to the current user, and sets the time to the current time.</summary>
		private void butAddCurrent_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			string address=Browser.GetComputerIpAddress();
			if(_listCloudAddresses.Any(x => x.IpAddress==address)) {
				MsgBox.Show(Lan.g(this,"Address")+$" {address} "+Lan.g(this,"is already allowed."));
				return;
			}
			CloudAddress newAddress=new CloudAddress{
				IpAddress=address,
				UserNumLastConnect=Security.CurUser.UserNum,
				DateTimeLastConnect=DateTime.Now
			};
			CloudAddresses.Insert(newAddress);
			_listCloudAddresses.Add(newAddress);
			FillGridAddresses();
		}

		///<summary>Adds the current IP address to the list with UserNumLastConnect=0 and DateTimeLastConnect=DateTime.Min.</summary>
		private void butAllowAddress_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			string address=textAddress.Text.Trim();
			if(string.IsNullOrEmpty(address)) {
				return;
			}
			if(!IPAddress.TryParse(address,out IPAddress addr)) {
				MsgBox.Show(Lan.g(this,"Address")+$" {address} "+Lan.g(this,"is invalid."));
				return;
			}
			address=addr.ToString();
			if(_listCloudAddresses.Any(x => x.IpAddress==address)) {
				MsgBox.Show(Lan.g(this,"Address")+$" {address} "+Lan.g(this,"is already allowed."));
				return;
			}
			CloudAddress newAddress=new CloudAddress{
				IpAddress=address,
				UserNumLastConnect=0,
				DateTimeLastConnect=DateTime.MinValue
			};
			CloudAddresses.Insert(newAddress);
			_listCloudAddresses.Add(newAddress);
			FillGridAddresses();
		}

	}
}