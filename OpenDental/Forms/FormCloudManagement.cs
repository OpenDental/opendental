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
using System.Xml.Serialization;
using WebServiceSerializer;

namespace OpenDental {
	public partial class FormCloudManagement:FormODBase {
		private List<ActiveInstance> _listActiveInstances;
		private List<Computer> _listComputers;
		private List<Userod> _listUserods;
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
			_listUserods=Userods.GetDeepCopy();
			_listActiveInstances=ActiveInstances.GetAllResponsiveActiveInstances()
				.Where(x => x.ConnectionType==ConnectionTypes.ODCloud)
				.ToList();
			_listCloudAddresses=CloudAddresses.GetAll();
			string officeData=PayloadHelper.CreatePayload("",eServiceCode.Undefined);
			Cursor=Cursors.WaitCursor;
			try {
				string maxSize=WebSerializer.DeserializePrimitive<int>(WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetMaxAtoZSizeGB(officeData)).ToString();
				string filePath=FileAtoZ.GetFilesInDirectoryRelative("cloudbackupsize")
					.FirstOrDefault(x => !string.IsNullOrEmpty(Path.GetExtension(x)));
				labelCurAtoZSize.Text=(string.IsNullOrEmpty(filePath)?"":FileAtoZ.ReadAllText(filePath)).Trim()+$"/{maxSize}GB";
			}
			catch(Exception ex) {
				ex.DoNothing();
				labelCurAtoZSize.Text="Error";
			}
			finally {
				Cursor=Cursors.Default;
			}
			FillGridInstances();
			FillGridAddresses();
		}

		private void FillGridInstances() {
			labelCurSessionNum.Text=_listActiveInstances.Count().ToString();
			gridActiveInstances.BeginUpdate();
			//Columns
			gridActiveInstances.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"User"),100);
			gridActiveInstances.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Computer Name"),200);
			gridActiveInstances.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Last Active"),200);
			gridActiveInstances.Columns.Add(col);
			//Rows
			gridActiveInstances.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listActiveInstances.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listUserods.FirstOrDefault(x => x.UserNum==_listActiveInstances[i].UserNum).UserName);
				if (_listComputers.Exists(x => x.ComputerNum==_listActiveInstances[i].ComputerNum)) {
					row.Cells.Add(_listComputers.FirstOrDefault(x => x.ComputerNum==_listActiveInstances[i].ComputerNum).CompName);
				}
				else {
					row.Cells.Add("UNKNOWN");
				}				
				row.Cells.Add(_listActiveInstances[i].DateTimeLastActive.ToString());
				row.Tag=_listActiveInstances[i];
				gridActiveInstances.ListGridRows.Add(row);
			}
			gridActiveInstances.SortForced(1,true);
			gridActiveInstances.EndUpdate();
		}

		private void FillGridAddresses() {
			gridAllowedAddresses.BeginUpdate();
			gridAllowedAddresses.Columns.Clear();
			gridAllowedAddresses.Columns.Add(new GridColumn(Lan.g(this,"IP Address"), 95));
			gridAllowedAddresses.Columns.Add(new GridColumn(Lan.g(this,"Last User"), 140));
			gridAllowedAddresses.Columns.Add(new GridColumn(Lan.g(this,"Last Connect"), 140, GridSortingStrategy.DateParse));
			gridAllowedAddresses.ListGridRows.Clear();
			DateTime date;
			GridRow row;
			for(int i=0;i<_listCloudAddresses.Count;i++) {
				date=_listCloudAddresses[i].DateTimeLastConnect;
				row=new GridRow();
				row.Cells.Add(_listCloudAddresses[i].IpAddress);
				row.Cells.Add(Userods.GetName(_listCloudAddresses[i].UserNumLastConnect));
				row.Cells.Add(date.Year<1880?"Never":date.ToString());
				row.Tag=_listCloudAddresses[i];
				gridAllowedAddresses.ListGridRows.Add(row);
			}
			gridAllowedAddresses.EndUpdate();
		}

		private void butAddSessions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
				return;
			}
			using FormCloudSessionLimit formCloudSessionLimit=new FormCloudSessionLimit();
			if(formCloudSessionLimit.ShowDialog()==DialogResult.OK) {
				labelMaxSessionNum.Text=PrefC.GetString(PrefName.CloudSessionLimit);
			}
		}

		private void butCloseSession_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.CloseOtherSessions)){
				return;
			}
			ActiveInstance activeInstance=gridActiveInstances.SelectedTag<ActiveInstance>();
			if(activeInstance==null) {
				MsgBox.Show(this,"Please select a session to close.");
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,Lan.g(this,"Are you sure you want to close the selected session?"))) {
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.CloseOtherSessions,0,"Session for "+_listUserods.Find(x => x.UserNum==activeInstance.UserNum).UserName
				+" on Computer "+_listComputers.Find(x => x.ComputerNum==activeInstance.ComputerNum)?.CompName??"UNKNOWN"
				+" was ended via Cloud Management.");
			Signalods.SetInvalid(InvalidType.ActiveInstance,KeyType.Undefined,activeInstance.ActiveInstanceNum);
			_listActiveInstances.Remove(activeInstance);
			FillGridInstances();
		}

		private void butChangePassword_Click(object sender,EventArgs e) {
			using FormChangeCloudPassword formChangeCloudPassword=new FormChangeCloudPassword();
			formChangeCloudPassword.ShowDialog();
		}

		///<summary>Deletes all selected IP addresses from the list of allowed addresses.</summary>
		private void butDeleteAddress_Click(object sender,EventArgs e) {
			//Remove the selected rows from the grid
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
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
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
				return;
			}
			string address=Browser.GetComputerIpAddress();
			if(_listCloudAddresses.Any(x => x.IpAddress==address)) {
				MsgBox.Show(Lan.g(this,"Address")+$" {address} "+Lan.g(this,"is already allowed."));
				return;
			}
			CloudAddress cloudAddressNew=new CloudAddress{
				IpAddress=address,
				UserNumLastConnect=Security.CurUser.UserNum,
				DateTimeLastConnect=DateTime.Now
			};
			CloudAddresses.Insert(cloudAddressNew);
			_listCloudAddresses.Add(cloudAddressNew);
			FillGridAddresses();
		}

		///<summary>Adds the current IP address to the list with UserNumLastConnect=0 and DateTimeLastConnect=DateTime.Min.</summary>
		private void butAllowAddress_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
				return;
			}
			string address=textAddress.Text.Trim();
			if(string.IsNullOrEmpty(address)) {
				return;
			}
			if(!IPAddress.TryParse(address,out IPAddress iPAddress)) {
				MsgBox.Show(Lan.g(this,"Address")+$" {address} "+Lan.g(this,"is invalid."));
				return;
			}
			address=iPAddress.ToString();
			if(_listCloudAddresses.Any(x => x.IpAddress==address)) {
				MsgBox.Show(Lan.g(this,"Address")+$" {address} "+Lan.g(this,"is already allowed."));
				return;
			}
			CloudAddress cloudAddressNew=new CloudAddress{
				IpAddress=address,
				UserNumLastConnect=0,
				DateTimeLastConnect=DateTime.MinValue
			};
			CloudAddresses.Insert(cloudAddressNew);
			_listCloudAddresses.Add(cloudAddressNew);
			FillGridAddresses();
		}

	}
}