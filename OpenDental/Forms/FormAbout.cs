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
			BugSubmission.SubmissionInfo submissionInfo=new BugSubmission(new Exception(),patNum:FormOpenDental.CurPatNum).Info;
			StringBuilder stringBuilder=new StringBuilder();
			Action<Action> actionWithTryCatch=(action) => { 
				try {
					action();
				}
				catch (Exception ex){
					stringBuilder.AppendLine($"ERROR: {ex.Message}");
				}
			};
			stringBuilder.AppendLine("-------------");
			stringBuilder.AppendLine($"Connection settings");
			actionWithTryCatch(() => stringBuilder.AppendLine($"  Server Name: {DataConnection.GetServerName()}"));
			actionWithTryCatch(() => stringBuilder.AppendLine($"  Database Name: {DataConnection.GetDatabaseName()}"));
			actionWithTryCatch(() => stringBuilder.AppendLine($"  MySQL User: {DataConnection.GetMysqlUser()}"));
			//Servername, database name, msq user and password
			List<FieldInfo> listFieldInfos=submissionInfo.GetType().GetFields().ToList();
			for(int i=0;i<listFieldInfos.Count;i++) { 
				object value=listFieldInfos[i].GetValue(submissionInfo);
				if(ListTools.In(value,null,"")) {
					continue;
				}
				if(value is Dictionary<PrefName,string>) {//DictPrefValues
					Dictionary<PrefName,string> dictionaryPrefValues=value as Dictionary<PrefName,string>;
					if(dictionaryPrefValues.Keys.Count>0) {
						stringBuilder.AppendLine(listFieldInfos[i].Name+":");
						dictionaryPrefValues.ToList().ForEach(x => stringBuilder.AppendLine("  "+x.Key.ToString()+": "+x.Value));
						stringBuilder.AppendLine("-------------");
					}
				}
				else if(value is List<string>) {//EnabledPlugins
					List<string> listEnabledPlugins=value as List<string>;
					if(listEnabledPlugins.Count>0) { 
						stringBuilder.AppendLine(listFieldInfos[i].Name+":");
						listEnabledPlugins.ForEach(x => stringBuilder.AppendLine("  "+x));
						stringBuilder.AppendLine("-------------");
					}
				}
				else if (value is bool) {
					stringBuilder.AppendLine(listFieldInfos[i].Name+": "+(((bool)value)==true?"true":"false"));
				}
				else if(listFieldInfos[i].Name=="CountClinics") {
					int countTotalClinics=(int)value;
					int countHiddenClinics=countTotalClinics-Clinics.GetCount(true);
					stringBuilder.AppendLine($"{listFieldInfos[i].Name}: {countTotalClinics} ({countHiddenClinics} hidden)");
				}
				else {
					stringBuilder.AppendLine(listFieldInfos[i].Name+": "+value);
				}
			}
			//Display the current HQ connection information.
			if(PrefC.IsODHQ) {
				Action<ConnectionNames> action=(connName) => {
					actionWithTryCatch(() => stringBuilder.AppendLine($"  Server Name: {DataConnection.GetServerName()}"));
					actionWithTryCatch(() => stringBuilder.AppendLine($"  Database Name: {DataConnection.GetDatabaseName()}"));
					actionWithTryCatch(() => stringBuilder.AppendLine($"  MySQL User: {DataConnection.GetMysqlUser()}"));
				};
				stringBuilder.AppendLine("-------------");
				stringBuilder.AppendLine("HQ Connection Settings");
				stringBuilder.AppendLine($"{ConnectionNames.BugsHQ.ToString()}:");
				actionWithTryCatch(() => DataAction.RunBugsHQ(() => action(ConnectionNames.BugsHQ),useConnectionStore: false));
				stringBuilder.AppendLine($"{ConnectionNames.CustomersHQ.ToString()}:");
				actionWithTryCatch(() => DataAction.RunCustomers(() => action(ConnectionNames.CustomersHQ),useConnectionStore: false));
				stringBuilder.AppendLine($"{ConnectionNames.ManualPublisher.ToString()}:");
				actionWithTryCatch(() => DataAction.RunManualPublisherHQ(() => action(ConnectionNames.ManualPublisher)));
				stringBuilder.AppendLine($"{ConnectionNames.WebChat.ToString()}:");
				actionWithTryCatch(() => DataAction.RunWebChat(() => action(ConnectionNames.WebChat)));
				stringBuilder.AppendLine("-------------");
			}
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(stringBuilder.ToString());
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
