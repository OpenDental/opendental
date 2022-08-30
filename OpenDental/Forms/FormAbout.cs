using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;
//using mshtml;

namespace OpenDental{
	///<summary>Form can be found at Help Tab->About</summary>
	public partial class FormAbout : FormODBase{

		///<summary></summary>
		public FormAbout(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAbout_Load(object sender, System.EventArgs e) {
			string softwareName=PrefC.GetString(PrefName.SoftwareName);
			if(Programs.GetCur(ProgramName.BencoPracticeManagement).Enabled) {
				pictureOpenDental.Image=Properties.Resources.bencoLogo;
			}
			if(softwareName!="Open Dental Software" && !Programs.GetCur(ProgramName.BencoPracticeManagement).Enabled) {
				pictureOpenDental.Visible=false;
			}
			labelVersion.Text=Lan.g(this,"Version:")+" "+Application.ProductVersion;
			UpdateHistory updateHistory=UpdateHistories.GetForVersion(Application.ProductVersion);
			if(updateHistory!=null) {
				labelVersion.Text+="  "+Lan.g(this,"Since:")+" "+updateHistory.DateTimeUpdated.ToShortDateString();
			}
			//keeps the trailing year up to date
			labelCopyright.Text=softwareName+" "+Lan.g(this,"Copyright 2003-")+DateTime.Now.ToString("yyyy")+", Jordan Sparks, D.M.D.";
			labelMySQLCopyright.Text=Lan.g(this,"MySQL - Copyright 1995-")+DateTime.Now.ToString("yyyy")+Lan.g(this,", www.mysql.com");
			labelMariaDBCopyright.Text=Lan.g(this,"MariaDB - Copyright 2009-")+DateTime.Now.ToString("yyyy")+Lan.g(this,", www.mariadb.com");
			//Database Server----------------------------------------------------------		
			List<string> listServiceInfo=Computers.GetServiceInfo();
			labelName.Text+=listServiceInfo[2].ToString();//MiscData.GetODServer();//server name
			labelService.Text+=listServiceInfo[0].ToString();//service name
			labelMySqlVersion.Text+=listServiceInfo[3].ToString();//service version
			labelServComment.Text+=listServiceInfo[1].ToString();//service comment
			labelMachineName.Text+=Environment.MachineName.ToUpper();//current client or remote application machine name
			labelDatabase.Text+=listServiceInfo[4].ToString();//database name
			Plugins.HookAddCode(this,"FormAbout.Load_end");
		}

		private void butDiagnostics_Click(object sender,EventArgs e) {
			string diagnostics = BugSubmissions.GetDiagnostics(FormOpenDental.PatNumCur);
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(diagnostics);
			msgBoxCopyPaste.Text=Lans.g(this,"Diagnostics");
			msgBoxCopyPaste.ShowDialog();
		}

		private void butLicense_Click(object sender,EventArgs e) {
			using FormLicense formLicense=new FormLicense();
			formLicense.ShowDialog();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
