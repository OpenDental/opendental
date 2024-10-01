using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using DataConnectionBase;
using Newtonsoft.Json;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmSiteLinkEdit:FrmODBase {
		private SiteLink _siteLink;

		public FrmSiteLinkEdit(SiteLink siteLink) {
			InitializeComponent();
			_siteLink=siteLink;
			Load+=FrmSiteLinkEdit_Load;
			textOctet1.TextChanged+=textOctet1_TextChanged;
			textOctet2.TextChanged+=textOctet2_TextChanged;
			textOctet3.TextChanged+=textOctet3_TextChanged;
			PreviewKeyDown+=FrmSiteLinkEdit_PreviewKeyDown;
		}

		private void FrmSiteLinkEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			Site site=Sites.GetFirstOrDefault(x => x.SiteNum==_siteLink.SiteNum);
			if(_siteLink.SiteNum < 1 || site==null) {
				MsgBox.Show(this,"Invalid SiteNum set for the passed in siteLink.");
				IsDialogOK=false;
				Close();
				return;
			}
			textSite.Text=site.Description;
			//Octets
			if(!string.IsNullOrEmpty(_siteLink.OctetStart)) {
				string[] stringArrayOctets=_siteLink.OctetStart.Split('.');
				if(stringArrayOctets.Length > 0) {
					textOctet1.Text=stringArrayOctets[0];
				}
				if(stringArrayOctets.Length > 1) {
					textOctet2.Text=stringArrayOctets[1];
				}
				if(stringArrayOctets.Length > 2) {
					textOctet3.Text=stringArrayOctets[2];
				}
			}
			//Triage
			comboTriageCoordinator.Items.Clear();
			List<Employee> listEmployees=Employees.GetDeepCopy(true);
			for(int i=0;i<listEmployees.Count;i++) {
				comboTriageCoordinator.Items.Add(Employees.GetNameFL(listEmployees[i]),listEmployees[i]);
			}
			comboTriageCoordinator.SetSelectedKey<Employee>(_siteLink.EmployeeNum,x => x.EmployeeNum);
			//Colors
			panelSiteColor.ColorBack=ColorOD.ToWpf(_siteLink.SiteColor);
			panelForeColor.ColorBack=ColorOD.ToWpf(_siteLink.ForeColor);
			panelInnerColor.ColorBack=ColorOD.ToWpf(_siteLink.InnerColor);
			panelOuterColor.ColorBack=ColorOD.ToWpf(_siteLink.OuterColor);
			labelOpsCountdownPreview.ColorBack=panelOpsCountPreview.ColorBack;
			panelOpsCountPreview.ColorBack=panelInnerColor.ColorBack;
			panelOpsCountPreview.ColorBorder=panelOuterColor.ColorBack;
			labelOpsCountdownPreview.ColorText=panelForeColor.ColorBack;
			labelOpsCountdownPreview.ColorBack=panelOpsCountPreview.ColorBack;
			FillGridConnections();
		}

		private void FillGridConnections() {
			Dictionary<ConnectionNames,CentralConnection> dictConnDefaults=null;
			ODException.SwallowAnyException(() => { dictConnDefaults=DataAction.GetHqConnections(); });
			if(dictConnDefaults==null) {
				MsgBox.Show("Error loading default HQ connections.  Please restart Open Dental or log off and log back in.");
				return;
			}
			Dictionary<ConnectionNames,CentralConnection> dictConnOverrides=_siteLink.DictConnectionSettingsHQOverrides;
			gridConnections.BeginUpdate();
			if(gridConnections.Columns.Count < 1) {
				//The order of these columns matter.  See GetConnectionOverrides()
				GridColumn gridColumn=new GridColumn("Conn Name",60);
				gridColumn.IsWidthDynamic=true;
				gridConnections.Columns.Add(gridColumn);
				gridColumn=new GridColumn("Server",60);
				gridColumn.IsWidthDynamic=true;
				gridColumn.IsEditable=true;
				gridConnections.Columns.Add(gridColumn);
				gridColumn=new GridColumn("Database",60);
				gridColumn.IsWidthDynamic=true;
				gridColumn.IsEditable=true;
				gridConnections.Columns.Add(gridColumn);
				gridColumn=new GridColumn("User",60);
				gridColumn.IsWidthDynamic=true;
				gridColumn.IsEditable=true;
				gridConnections.Columns.Add(gridColumn);
				gridColumn=new GridColumn("Password",60);
				gridColumn.IsWidthDynamic=true;
				gridColumn.IsEditable=true;
				gridConnections.Columns.Add(gridColumn);
			}
			Action<ConnectionNames> actionAddConnection=(connName) => {
				GridRow row=new GridRow();
				CentralConnection centralConnection=dictConnDefaults[connName];
				if(dictConnOverrides!=null && dictConnOverrides.ContainsKey(connName)) {
					centralConnection=dictConnOverrides[connName];
					row.Bold=true;
				}
				row.Cells.Add(connName.ToString());
				row.Cells.Add(centralConnection.ServerName??"");
				row.Cells.Add(centralConnection.DatabaseName??"");
				row.Cells.Add(centralConnection.MySqlUser??"");
				row.Cells.Add(centralConnection.MySqlPassword??"");
				row.Tag=connName;
				gridConnections.ListGridRows.Add(row);
			};
			gridConnections.ListGridRows.Clear();
			actionAddConnection(ConnectionNames.BugsHQ);
			actionAddConnection(ConnectionNames.CustomersHQ);
			actionAddConnection(ConnectionNames.ManualPublisher);
			actionAddConnection(ConnectionNames.WebChat);
			actionAddConnection(ConnectionNames.RemoteSupport);
			gridConnections.EndUpdate();
		}

		private void textOctet1_TextChanged(object sender,EventArgs e) {
			if(textOctet1.Text.EndsWith(".")) {
				textOctet1.Text=textOctet1.Text.Trim('.');
				textOctet2.Focus();
				textOctet2.SelectAll();
			}
			textOctet1.Text=new string(textOctet1.Text.Where(x => char.IsDigit(x)).ToArray());
			if(textOctet1.Text.Length > 3) {
				textOctet1.Text=textOctet1.Text.Substring(0,3);
			}
			textOctet1.SelectionStart=textOctet1.Text.Length;
			if(textOctet1.Text.Length==3) {
				textOctet2.Focus();
				textOctet2.SelectAll();
			}
		}

		private void textOctet2_TextChanged(object sender,EventArgs e) {
			if(textOctet2.Text.EndsWith(".")) {
				textOctet2.Text=textOctet2.Text.Trim('.');
				textOctet3.Focus();
				textOctet3.SelectAll();
			}
			textOctet2.Text=new string(textOctet2.Text.Where(x => char.IsDigit(x)).ToArray());
			if(textOctet2.Text.Length > 3) {
				textOctet2.Text=textOctet2.Text.Substring(0,3);
			}
			textOctet2.SelectionStart=textOctet2.Text.Length;
			if(textOctet2.Text.Length==3) {
				textOctet3.Focus();
				textOctet3.SelectAll();
			}
		}

		private void textOctet3_TextChanged(object sender,EventArgs e) {
			textOctet3.Text=new string(textOctet3.Text.Where(x => char.IsDigit(x)).ToArray());
			if(textOctet3.Text.Length > 3) {
				textOctet3.Text=textOctet3.Text.Substring(0,3);
			}
			textOctet3.SelectionStart=textOctet3.Text.Length;
		}

		private Color PromptForColor(Color color) {
			Color colorRetVal=color;
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=color;
			frmColorDialog.ShowDialog();
			if(frmColorDialog.IsDialogOK) {
				colorRetVal=frmColorDialog.Color;
			}
			return colorRetVal;
		}

		private void butChangeSiteColor_Click(object sender,EventArgs e) {
			panelSiteColor.ColorBack=PromptForColor(panelSiteColor.ColorBack);
		}

		///<summary>Number color</summary>
		private void butChangeForeColor_Click(object sender,EventArgs e) {
			panelForeColor.ColorBack=PromptForColor(panelForeColor.ColorBack);
			labelOpsCountdownPreview.ColorText=panelForeColor.ColorBack;
		}

		///<summary>Background color</summary>
		private void butChangeInnerColor_Click(object sender,EventArgs e) {
			panelInnerColor.ColorBack=PromptForColor(panelInnerColor.ColorBack);
			panelOpsCountPreview.ColorBack=panelInnerColor.ColorBack;
			labelOpsCountdownPreview.ColorBack=panelOpsCountPreview.ColorBack;//Needed since the label background won't match the panel background otherwise
		}

		///<summary>Border color</summary>
		private void butChangeOuterColor_Click(object sender,EventArgs e) {
			panelOuterColor.ColorBack=PromptForColor(panelOuterColor.ColorBack);
			panelOpsCountPreview.ColorBorder=panelOuterColor.ColorBack;
		}

		private void ButDefault_Click(object sender,EventArgs e) {
			if(gridConnections.GetSelectedIndex()==-1) {
				MsgBox.Show("Please select a row.");
				return;
			}
			ConnectionNames connectionNames=gridConnections.SelectedTag<ConnectionNames>();
			Dictionary<ConnectionNames,CentralConnection> dictConnOverrides=_siteLink.DictConnectionSettingsHQOverrides;
			if(!dictConnOverrides.ContainsKey(connectionNames)) {
				return;
			}
			//Update the site link object so that it does not have an override anymore.
			dictConnOverrides.Remove(connectionNames);
			_siteLink.ConnectionSettingsHQOverrides=GetOverridesSerialized(dictConnOverrides);
			FillGridConnections();
		}

		///<summary>Returns a JSON serialized version of a connection override dictionary based off of gridConnection.
		///Users can edit cells of the grid to change the site overrides and this method returns what should be saved to the db.</summary>
		private string GetConnectionOverrides() {
			string retVal="";
			Dictionary<ConnectionNames,CentralConnection> dictConnDefaults=DataAction.GetHqConnections();
			Dictionary<ConnectionNames,CentralConnection> dictConnOverrides=new Dictionary<ConnectionNames,CentralConnection>();
			for(int i=0;i<gridConnections.ListGridRows.Count;i++) {
				ConnectionNames connectionNames=(ConnectionNames)gridConnections.ListGridRows[i].Tag;
				if(!dictConnDefaults.ContainsKey(connectionNames)) {
					continue;//Should never happen.
				}
				CentralConnection centralConnectionDefault=dictConnDefaults[connectionNames];
				//Read in the cell text and create an override if any of the values differ from the default.
				string serverName=gridConnections.ListGridRows[i].Cells[1].Text;
				string databaseName=gridConnections.ListGridRows[i].Cells[2].Text;
				string mySqlUser=gridConnections.ListGridRows[i].Cells[3].Text;
				string mySqlPassword=gridConnections.ListGridRows[i].Cells[4].Text;
				if(serverName.IsNullOrEmpty()
					&& databaseName.IsNullOrEmpty()
					&& mySqlUser.IsNullOrEmpty()
					&& mySqlPassword.IsNullOrEmpty())
				{
					continue;//Do not add an override, this will cause the connection to go back to using the default.
				}
				if(serverName!=centralConnectionDefault.ServerName
					|| databaseName!=centralConnectionDefault.DatabaseName
					|| mySqlUser!=centralConnectionDefault.MySqlUser
					|| mySqlPassword!=centralConnectionDefault.MySqlPassword)
				{
					dictConnOverrides[connectionNames]=new CentralConnection() {
						ServerName=serverName,
						DatabaseName=databaseName,
						MySqlUser=mySqlUser,
						MySqlPassword=mySqlPassword
					};
				}
			}
			if(dictConnOverrides.Count > 0) {
				retVal=GetOverridesSerialized(dictConnOverrides);
			}
			return retVal;
		}

		///<summary>Returns a JSON serialized version of the dictionary passed in.</summary>
		private string GetOverridesSerialized(Dictionary<ConnectionNames,CentralConnection> dictConnOverrides) {
			JsonSerializerSettings jsonSerializerSettings=new JsonSerializerSettings();
			jsonSerializerSettings.DefaultValueHandling=DefaultValueHandling.Ignore;
			return JsonConvert.SerializeObject(dictConnOverrides,jsonSerializerSettings);
		}

		private void FrmSiteLinkEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			_siteLink.OctetStart=PIn.Int(textOctet1.Text,false)
				+"."+PIn.Int(textOctet2.Text,false)
				+"."+PIn.Int(textOctet3.Text,false)
				+".";//End with a period so that the matching algorithm in other parts of the program are accurate.
			if(comboTriageCoordinator.SelectedIndex > -1) {
				_siteLink.EmployeeNum=comboTriageCoordinator.GetSelectedKey<Employee>(x => x.EmployeeNum);
			}
			_siteLink.SiteColor=ColorOD.FromWpf(panelSiteColor.ColorBack);
			_siteLink.ForeColor=ColorOD.FromWpf(panelForeColor.ColorBack);
			_siteLink.InnerColor=ColorOD.FromWpf(panelInnerColor.ColorBack);
			_siteLink.OuterColor=ColorOD.FromWpf(panelOuterColor.ColorBack);
			//There is no such thing as an empty grid of connection overrides (we show the defaults at minimum).
			//The only explanation for an empty grid is if the defaults could not be loaded correctly so do not save ConnectionSettingsHQOverrides.
			if(gridConnections.ListGridRows.Count > 0) {
				_siteLink.ConnectionSettingsHQOverrides=GetConnectionOverrides();
			}
			SiteLinks.Upsert(_siteLink);
			if(comboTriageCoordinator.SelectedIndex>-1) {
				SiteLinks.UpdateAllTriageCoordinators(_siteLink);
			}
			IsDialogOK=true;
		}

	}
}