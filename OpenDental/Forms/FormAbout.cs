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
	///<summary></summary>
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
			//Database Server----------------------------------------------------------		
			List<string> serviceList=Computers.GetServiceInfo();
			labelName.Text+=serviceList[2].ToString();//MiscData.GetODServer();//server name
			labelService.Text+=serviceList[0].ToString();//service name
			labelMySqlVersion.Text+=serviceList[3].ToString();//service version
			labelServComment.Text+=serviceList[1].ToString();//service comment
			labelMachineName.Text+=Environment.MachineName.ToUpper();//current client or remote application machine name
			labelDatabase.Text+=serviceList[4].ToString();//database name
			Plugins.HookAddCode(this,"FormAbout.Load_end");
		}

		private void butDiagnostics_Click(object sender,EventArgs e) {
			BugSubmission.SubmissionInfo subInfo=new BugSubmission(new Exception(),patNum:FormOpenDental.CurPatNum).Info;
			StringBuilder strBuilder=new StringBuilder();
			Action<Action> tryRunAction=(act) => { 
				try {
					act();
				}
				catch (Exception ex){
					strBuilder.AppendLine($"ERROR: {ex.Message}");
				}
			};
			strBuilder.AppendLine("-------------");
			strBuilder.AppendLine($"Connection settings");
			tryRunAction(() => strBuilder.AppendLine($"  Server Name: {DataConnection.GetServerName()}"));
			tryRunAction(() => strBuilder.AppendLine($"  Database Name: {DataConnection.GetDatabaseName()}"));
			tryRunAction(() => strBuilder.AppendLine($"  MySQL User: {DataConnection.GetMysqlUser()}"));
			//Servername, database name, msq user and password
			foreach(FieldInfo field in subInfo.GetType().GetFields()) {
				object value=field.GetValue(subInfo);
				if(ListTools.In(value,null,"")) {
					continue;
				}
				if(value is Dictionary<PrefName,string>) {//DictPrefValues
					Dictionary<PrefName,string> dictPrefValues=value as Dictionary<PrefName,string>;
					if(dictPrefValues.Keys.Count>0) {
						strBuilder.AppendLine(field.Name+":");
						dictPrefValues.ToList().ForEach(x => strBuilder.AppendLine("  "+x.Key.ToString()+": "+x.Value));
						strBuilder.AppendLine("-------------");
					}
				}
				else if(value is List<string>) {//EnabledPlugins
					List<string> enabledPlugins=value as List<string>;
					if(enabledPlugins.Count>0) { 
						strBuilder.AppendLine(field.Name+":");
						enabledPlugins.ForEach(x => strBuilder.AppendLine("  "+x));
						strBuilder.AppendLine("-------------");
					}
				}
				else if (value is bool) {
					strBuilder.AppendLine(field.Name+": "+(((bool)value)==true?"true":"false"));
				}
				else if(field.Name=="CountClinics") {
					int numTotalClinics=(int)value;
					int numHiddenClinics=numTotalClinics-Clinics.GetCount(true);
					strBuilder.AppendLine($"{field.Name}: {numTotalClinics} ({numHiddenClinics} hidden)");
				}
				else {
					strBuilder.AppendLine(field.Name+": "+value);
				}
			}
			//Display the current HQ connection information.
			if(PrefC.IsODHQ) {
				Action<ConnectionNames> action=(connName) => {
					tryRunAction(() => strBuilder.AppendLine($"  Server Name: {DataConnection.GetServerName()}"));
					tryRunAction(() => strBuilder.AppendLine($"  Database Name: {DataConnection.GetDatabaseName()}"));
					tryRunAction(() => strBuilder.AppendLine($"  MySQL User: {DataConnection.GetMysqlUser()}"));
				};
				strBuilder.AppendLine("-------------");
				strBuilder.AppendLine("HQ Connection Settings");
				strBuilder.AppendLine($"{ConnectionNames.BugsHQ.ToString()}:");
				tryRunAction(() => DataAction.RunBugsHQ(() => action(ConnectionNames.BugsHQ),useConnectionStore: false));
				strBuilder.AppendLine($"{ConnectionNames.CustomersHQ.ToString()}:");
				tryRunAction(() => DataAction.RunCustomers(() => action(ConnectionNames.CustomersHQ),useConnectionStore: false));
				strBuilder.AppendLine($"{ConnectionNames.ManualPublisher.ToString()}:");
				tryRunAction(() => DataAction.RunManualPublisherHQ(() => action(ConnectionNames.ManualPublisher)));
				strBuilder.AppendLine($"{ConnectionNames.WebChat.ToString()}:");
				tryRunAction(() => DataAction.RunWebChat(() => action(ConnectionNames.WebChat)));
				strBuilder.AppendLine("-------------");
			}
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(strBuilder.ToString());
			msgbox.Text=Lans.g(this,"Diagnostics");
			msgbox.ShowDialog();
		}

		private void butLicense_Click(object sender,EventArgs e) {
			using FormLicense FormL=new FormLicense();
			FormL.ShowDialog();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
