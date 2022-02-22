using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Xml;
using System.Xml.XPath;

namespace OpenDental {
	public partial class FormUpdateInProgress:FormODBase {
		private string _updateComputerName;
		private List<string> _listAdminCompNames;

		public FormUpdateInProgress(string updateComputerName) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_updateComputerName=updateComputerName;
			_listAdminCompNames=new List<string>();
			string xmlPath=ODFileUtils.CombinePaths(Application.StartupPath,"FreeDentalConfig.xml");
			XmlDocument document=new XmlDocument();
			try {
				document.Load(xmlPath);
				XPathNavigator Navigator=document.CreateNavigator();
				XPathNavigator nav;
				XPathNodeIterator navIterator;
				nav=Navigator.SelectSingleNode("//AdminCompNames");
				navIterator=nav.SelectChildren(XPathNodeType.All);
				for(int i = 0;i < navIterator.Count;i++) {
					navIterator.MoveNext();
					_listAdminCompNames.Add(navIterator.Current.Value);//Add this computer name to the list.
				}
			}
			catch(Exception) {
				//suppress. this just means that the FreeDentalConfig xml file doesn't have any Admin Computers listed.
			}
			_listAdminCompNames.Add(_updateComputerName);
		}

		private void FormUpdateInProgress_Load(object sender,System.EventArgs e) {
			string warningString=Lan.g("FormUpdateInProgress","An update is in progress on workstation")+": '"+_updateComputerName+"'.\r\n\r\n"
			+Lan.g(this,"Not allowed to start")+" "+PrefC.GetString(PrefName.SoftwareName)+" "+Lan.g(this,"while an update is in progress.")+"\r\n"
			+Lan.g(this,"Please wait and click 'Try Again'.");
			try {
				if(_listAdminCompNames.Contains(ComputerPrefs.LocalComputer.ComputerName)) {
					warningString+="\r\n\r\n"+Lan.g(this,"If you are the person who started the update and you wish to override"
						+" this message because an update is not in progress, click 'Override'.");
					butOverride.Visible=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();	//When updating to a version with a new computer pref column, the crud knows about the column before the database does.
			}
			labelWarning.Text=warningString;
		}

		private void butTryAgain_Click(object sender,EventArgs e) {
			Prefs.RefreshCache();
			if(PrefC.GetString(PrefName.UpdateInProgressOnComputerName)!="") {
				MessageBox.Show(Lan.g(this,"Workstation")+": '"+_updateComputerName+"' "+Lan.g(this,"is still updating.  Please wait and 'Try Again'"));
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOverride_Click(object sender,EventArgs e) {
			Prefs.UpdateString(PrefName.UpdateInProgressOnComputerName,"");
			MsgBox.Show(this,"You will be allowed access when you restart.");
			DialogResult=DialogResult.Cancel;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}