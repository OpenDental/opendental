using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using OpenDental.UI;
using Newtonsoft.Json;
using DataConnectionBase;

namespace OpenDental {
	public partial class FormSiteLinkEdit:FormODBase {
		private SiteLink _siteLink;

		public FormSiteLinkEdit(SiteLink siteLink) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_siteLink=siteLink;
		}

		private void FormSiteLinkEdit_Load(object sender,EventArgs e) {
			Site site=Sites.GetFirstOrDefault(x => x.SiteNum==_siteLink.SiteNum);
			if(_siteLink.SiteNum < 1 || site==null) {
				MsgBox.Show(this,"Invalid SiteNum set for the passed in siteLink.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			textSite.Text=site.Description;
			//Octets
			if(!string.IsNullOrEmpty(_siteLink.OctetStart)) {
				string[] arrayOctets=_siteLink.OctetStart.Split('.');
				if(arrayOctets.Length > 0) {
					textOctet1.Text=arrayOctets[0];
				}
				if(arrayOctets.Length > 1) {
					textOctet2.Text=arrayOctets[1];
				}
				if(arrayOctets.Length > 2) {
					textOctet3.Text=arrayOctets[2];
				}
			}
			//Triage
			comboTriageCoordinator.Items.Clear();
			foreach(Employee employee in Employees.GetDeepCopy(true)) {
				comboTriageCoordinator.Items.Add(Employees.GetNameFL(employee),employee);
			}
			comboTriageCoordinator.SetSelectedKey<Employee>(_siteLink.EmployeeNum,x => x.EmployeeNum);
			//Colors
			panelSiteColor.BackColor=_siteLink.SiteColor;
			panelForeColor.BackColor=_siteLink.ForeColor;
			panelInnerColor.BackColor=_siteLink.InnerColor;
			panelOuterColor.BackColor=_siteLink.OuterColor;
			labelOpsCountPreview.SetColors(panelForeColor.BackColor,panelOuterColor.BackColor,panelInnerColor.BackColor);
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
			if(gridConnections.ListGridColumns.Count < 1) {
				//The order of these columns matter.  See GetConnectionOverrides()
				gridConnections.ListGridColumns.Add(new GridColumn("Conn Name",60){ IsWidthDynamic=true });
				gridConnections.ListGridColumns.Add(new GridColumn("Server",60,true){ IsWidthDynamic=true });
				gridConnections.ListGridColumns.Add(new GridColumn("Database",60,true){ IsWidthDynamic=true });
				gridConnections.ListGridColumns.Add(new GridColumn("User",60,true){ IsWidthDynamic=true });
				gridConnections.ListGridColumns.Add(new GridColumn("Password",60,true){ IsWidthDynamic=true });
			}
			Action<ConnectionNames> actionAddConnection=(connName) => {
				GridRow row=new GridRow();
				CentralConnection connection=dictConnDefaults[connName];
				if(dictConnOverrides!=null && dictConnOverrides.ContainsKey(connName)) {
					connection=dictConnOverrides[connName];
					row.Bold=true;
				}
				row.Cells.Add(connName.ToString());
				row.Cells.Add(connection.ServerName??"");
				row.Cells.Add(connection.DatabaseName??"");
				row.Cells.Add(connection.MySqlUser??"");
				row.Cells.Add(connection.MySqlPassword??"");
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
			if(textOctet1.TextLength > 3) {
				textOctet1.Text=textOctet1.Text.Substring(0,3);
			}
			textOctet1.SelectionStart=textOctet1.TextLength;
			if(textOctet1.TextLength==3) {
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
			if(textOctet2.TextLength > 3) {
				textOctet2.Text=textOctet2.Text.Substring(0,3);
			}
			textOctet2.SelectionStart=textOctet2.TextLength;
			if(textOctet2.TextLength==3) {
				textOctet3.Focus();
				textOctet3.SelectAll();
			}
		}

		private void textOctet3_TextChanged(object sender,EventArgs e) {
			textOctet3.Text=new string(textOctet3.Text.Where(x => char.IsDigit(x)).ToArray());
			if(textOctet3.TextLength > 3) {
				textOctet3.Text=textOctet3.Text.Substring(0,3);
			}
			textOctet3.SelectionStart=textOctet3.TextLength;
		}

		private Color GetColor(Color colorCur) {
			Color retVal=colorCur;
			using ColorDialog colorDlg=new ColorDialog();
			colorDlg.AllowFullOpen=true;
			colorDlg.AnyColor=true;
			colorDlg.SolidColorOnly=false;
			colorDlg.Color=colorCur;
			if(colorDlg.ShowDialog()==DialogResult.OK) {
				retVal=colorDlg.Color;
			}
			return retVal;
		}

		private void butChangeSiteColor_Click(object sender,EventArgs e) {
			panelSiteColor.BackColor=GetColor(panelSiteColor.BackColor);
		}

		private void butChangeForeColor_Click(object sender,EventArgs e) {
			panelForeColor.BackColor=GetColor(panelForeColor.BackColor);
			labelOpsCountPreview.SetColors(panelForeColor.BackColor,panelOuterColor.BackColor,panelInnerColor.BackColor);
		}

		private void butChangeInnerColor_Click(object sender,EventArgs e) {
			panelInnerColor.BackColor=GetColor(panelInnerColor.BackColor);
			labelOpsCountPreview.SetColors(panelForeColor.BackColor,panelOuterColor.BackColor,panelInnerColor.BackColor);
		}

		private void butChangeOuterColor_Click(object sender,EventArgs e) {
			panelOuterColor.BackColor=GetColor(panelOuterColor.BackColor);
			labelOpsCountPreview.SetColors(panelForeColor.BackColor,panelOuterColor.BackColor,panelInnerColor.BackColor);
		}

		private void ButDefault_Click(object sender,EventArgs e) {
			if(gridConnections.GetSelectedIndex()==-1) {
				MsgBox.Show("Please select a row.");
				return;
			}
			ConnectionNames connName=gridConnections.SelectedTag<ConnectionNames>();
			Dictionary<ConnectionNames,CentralConnection> dictConnOverrides=_siteLink.DictConnectionSettingsHQOverrides;
			if(!dictConnOverrides.ContainsKey(connName)) {
				return;
			}
			//Update the site link object so that it does not have an override anymore.
			dictConnOverrides.Remove(connName);
			_siteLink.ConnectionSettingsHQOverrides=GetOverridesSerialized(dictConnOverrides);
			FillGridConnections();
		}

		///<summary>Returns a JSON serialized version of a connection override dictionary based off of gridConnection.
		///Users can edit cells of the grid to change the site overrides and this method returns what should be saved to the db.</summary>
		private string GetConnectionOverrides() {
			string retVal="";
			Dictionary<ConnectionNames,CentralConnection> dictConnDefaults=DataAction.GetHqConnections();
			Dictionary<ConnectionNames,CentralConnection> dictConnOverrides=new Dictionary<ConnectionNames,CentralConnection>();
			foreach(GridRow row in gridConnections.ListGridRows) {
				ConnectionNames connName=(ConnectionNames)row.Tag;
				if(!dictConnDefaults.ContainsKey(connName)) {
					continue;//Should never happen.
				}
				CentralConnection connectionDefault=dictConnDefaults[connName];
				//Read in the cell text and create an override if any of the values differ from the default.
				string serverName=row.Cells[1].Text;
				string databaseName=row.Cells[2].Text;
				string mySqlUser=row.Cells[3].Text;
				string mySqlPassword=row.Cells[4].Text;
				if(serverName.IsNullOrEmpty()
					&& databaseName.IsNullOrEmpty()
					&& mySqlUser.IsNullOrEmpty()
					&& mySqlPassword.IsNullOrEmpty())
				{
					continue;//Do not add an override, this will cause the connection to go back to using the default.
				}
				if(serverName!=connectionDefault.ServerName
					|| databaseName!=connectionDefault.DatabaseName
					|| mySqlUser!=connectionDefault.MySqlUser
					|| mySqlPassword!=connectionDefault.MySqlPassword)
				{
					dictConnOverrides[connName]=new CentralConnection() {
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
			JsonSerializerSettings settings=new JsonSerializerSettings();
			settings.DefaultValueHandling=DefaultValueHandling.Ignore;
			return JsonConvert.SerializeObject(dictConnOverrides,settings);
		}

		private void butOK_Click(object sender,EventArgs e) {
			_siteLink.OctetStart=PIn.Int(textOctet1.Text,false)
				+"."+PIn.Int(textOctet2.Text,false)
				+"."+PIn.Int(textOctet3.Text,false)
				+".";//End with a period so that the matching algorithm in other parts of the program are accurate.
			if(comboTriageCoordinator.SelectedIndex > -1) {
				_siteLink.EmployeeNum=comboTriageCoordinator.GetSelectedKey<Employee>(x => x.EmployeeNum);
			}
			_siteLink.SiteColor=panelSiteColor.BackColor;
			_siteLink.ForeColor=panelForeColor.BackColor;
			_siteLink.InnerColor=panelInnerColor.BackColor;
			_siteLink.OuterColor=panelOuterColor.BackColor;
			//There is no such thing as an empty grid of connection overrides (we show the defaults at minimum).
			//The only explanation for an empty grid is if the defaults could not be loaded correctly so do not save ConnectionSettingsHQOverrides.
			if(gridConnections.ListGridRows.Count > 0) {
				_siteLink.ConnectionSettingsHQOverrides=GetConnectionOverrides();
			}
			SiteLinks.Upsert(_siteLink);
			if(comboTriageCoordinator.SelectedIndex>-1) {
				SiteLinks.UpdateAllTriageCoordinators(_siteLink);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}