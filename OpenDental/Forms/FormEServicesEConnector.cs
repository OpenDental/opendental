using CodeBase;
using Microsoft.Win32;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Linq;
using System.IO;
using WebServiceSerializer;
using OpenDentBusiness.WebServiceMainHQ;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace OpenDental {

	public partial class FormEServicesEConnector:FormODBase {
		///<summary>The background color used when the OpenDentalCustListener service is down.  Using Red was deemed too harsh.
		///This variable should be treated as a constant which is why it is in all caps.  The type 'System.Drawing.Color' cannot be declared const.</summary>
		private Color COLOR_ESERVICE_CRITICAL_BACKGROUND=Color.OrangeRed;
		///<summary>The text color used when the OpenDentalCustListener service is down.
		///This variable should be treated as a constant which is why it is in all caps.  The type 'System.Drawing.Color' cannot be declared const.</summary>
		private Color COLOR_ESERVICE_CRITICAL_TEXT=Color.Yellow;
		///<summary>The background color used when the OpenDentalCustListener service has an error that has not be processed.
		///This variable should be treated as a constant which is why it is in all caps.  The type 'System.Drawing.Color' cannot be declared const.</summary>
		private Color COLOR_ESERVICE_ERROR_BACKGROUND=Color.LightGoldenrodYellow;
		///<summary>The text color used when the OpenDentalCustListener service has an error that has not be processed.
		///This variable should be treated as a constant which is why it is in all caps.  The type 'System.Drawing.Color' cannot be declared const.</summary>
		private Color COLOR_ESERVICE_ERROR_TEXT=Color.OrangeRed;
		///<summary>Output from HQ initialized in FillForm().</summary>
		
		public FormEServicesEConnector() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEServicesEConnector_Load(object sender,EventArgs e) {
			//Check to see if the eConnector service is already installed.  If it is, disable the install button.
			//Users who want to install multiple on one computer can use the Service Manager instead.
			//Do nothing on error.  The Install button will simply be visible.
			ODException.SwallowAnyException(() => {
				if(PrefC.IsCloudMode || ServicesHelper.GetServicesByExe("OpenDentalEConnector.exe").Count>0) {
					butInstallEConnector.Enabled=false;
				}
			});
			FillTextListenerServiceStatus();
			FillGridListenerService();
			//Default to not checked if it is unknown.
			checkEmailsWithDiffProcess.Checked=((YN)PrefC.GetInt(PrefName.SendEmailsInDiffProcess)==YN.Yes);
			textLogCleanupInterval.Text=PrefC.GetLong(PrefName.EConnectorCleanupLoggerIntervalDays).ToString();
			//Disable certain buttons but let them continue to view.
			butListenerServiceAck.Enabled=Security.IsAuthorized(Permissions.EServicesSetup,true);
		}

		///<summary>Updates the text box that is displaying the current status of the Listener Service.  Returns the status just in case other logic is needed outside of updating the status box.</summary>
		private eServiceSignalSeverity FillTextListenerServiceStatus() {
			eServiceSignalSeverity eServiceStatus=EServiceSignals.GetListenerServiceStatus();
			if(eServiceStatus==eServiceSignalSeverity.Critical) {
				textListenerServiceStatus.BackColor=COLOR_ESERVICE_CRITICAL_BACKGROUND;
				textListenerServiceStatus.ForeColor=COLOR_ESERVICE_CRITICAL_TEXT;
				butStartListenerService.Enabled=true;
			}
			else if(eServiceStatus==eServiceSignalSeverity.Error) {
				textListenerServiceStatus.BackColor=COLOR_ESERVICE_ERROR_BACKGROUND;
				textListenerServiceStatus.ForeColor=COLOR_ESERVICE_ERROR_TEXT;
				butStartListenerService.Enabled=true;
			}
			else {
				textListenerServiceStatus.BackColor=SystemColors.Control;
				textListenerServiceStatus.ForeColor=SystemColors.WindowText;
				butStartListenerService.Enabled=false;
			}
			textListenerServiceStatus.Text=eServiceStatus.ToString();
			return eServiceStatus;
		}

		private void butInstallEConnector_Click(object sender,EventArgs e) {
			DialogResult dialogResult;
			//Check to see if the update server preference is set.
			//If set, make sure that this is set to the computer currently logged on.
			string updateServerName=PrefC.GetString(PrefName.WebServiceServerName);
			if(!string.IsNullOrEmpty(updateServerName)&&!ODEnvironment.IdIsThisComputer(updateServerName.ToLower())) {
				dialogResult=MessageBox.Show(Lan.g(this,"The eConnector service should be installed on the Update Server")+": "+updateServerName+"\r\n"
					+Lan.g(this,"Are you trying to install the eConnector on a different computer by accident?"),"",MessageBoxButtons.YesNoCancel);
				//Only saying No to this message box pop up will allow the user to continue (meaning they fully understand what they are getting into).
				if(dialogResult!=DialogResult.No) {
					return;
				}
			}
			//Only ask the user if they want to set the Update Server Name preference if it is not already set.
			if(string.IsNullOrEmpty(updateServerName)) {
				dialogResult=MessageBox.Show(Lan.g(this,"The computer that has the eConnector service installed should be set as the Update Server.")+"\r\n"
					+Lan.g(this,"Would you like to make this computer the Update Server?"),"",MessageBoxButtons.YesNoCancel);
				if(dialogResult==DialogResult.Cancel) {
					return;
				}
				else if(dialogResult==DialogResult.Yes) {
					Prefs.UpdateString(PrefName.WebServiceServerName,Dns.GetHostName());
				}
				try {
					WebServiceMainHQProxy.SetEConnectorOn();
				}
				catch {
					MsgBox.Show(this,"The eConnector was disabled.  Please contact support.");
					return;
				}
			}
			//At this point the user wants to install the eConnector service (or upgrade the old cust listener to the eConnector).
			bool isListening;
			if(!PrefL.UpgradeOrInstallEConnector(false,out isListening)) {
				//Warning messages would have already been shown to the user, simply return.
				return;
			}
			//The eConnector service was successfully installed and is running, set the EConnectorEnabled flag true if false.
			Prefs.UpdateBool(PrefName.EConnectorEnabled,true);
			MsgBox.Show(this,"eConnector successfully installed");
			butInstallEConnector.Enabled=false;
			FillTextListenerServiceStatus();
			FillGridListenerService();
		}

		private void FillGridListenerService() {
			//Display some historical information for the last 30 days in this grid about the lifespan of the listener heartbeats.
			List<EServiceSignal> listESignals=EServiceSignals.GetServiceHistory(eServiceCode.ListenerService,DateTime.Today.AddDays(-30),DateTime.Today);
			gridListenerServiceStatusHistory.BeginUpdate();
			gridListenerServiceStatusHistory.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"DateTime"),120);
			gridListenerServiceStatusHistory.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),90);
			gridListenerServiceStatusHistory.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Details"),80){ IsWidthDynamic=true };
			gridListenerServiceStatusHistory.ListGridColumns.Add(col);
			gridListenerServiceStatusHistory.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<listESignals.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listESignals[i].SigDateTime.ToString());
				row.Cells.Add(listESignals[i].Severity.ToString());
				row.Cells.Add(listESignals[i].Description.ToString());
				//Color the row if it is an error that has not been processed.
				if(listESignals[i].Severity==eServiceSignalSeverity.Error&&!listESignals[i].IsProcessed) {
					row.ColorBackG=COLOR_ESERVICE_ERROR_BACKGROUND;
				}
				gridListenerServiceStatusHistory.ListGridRows.Add(row);
			}
			gridListenerServiceStatusHistory.EndUpdate();
		}

		private void butStartListenerService_Click(object sender,EventArgs e) {
			//No setup permission check here so that anyone can hopefully get the service back up and running.
			//Check to see if the service started up on its own while we were in this window.
			if(FillTextListenerServiceStatus()==eServiceSignalSeverity.Working) {
				//Use a slightly different message than below so that we can easily tell which part of this method customers reached.
				MsgBox.Show(this,"Listener Service already started.  Please call us for support if eServices are still not working.");
				return;
			}
			//Check to see if the listener service is installed on this computer.
			List<ServiceController> listServiceControllers;
			try {
				listServiceControllers=ServicesHelper.GetServicesByRegistryImagePath("OpenDentalEConnector.exe");
			}
			catch(Exception ex) {
				string error=Lan.g(this,"There was an problem starting eConnector Services.  Try running eConnector in administrator "
					+"mode or manually start the services.");
				FriendlyException.Show(error,ex);
				return;
			}
			if(listServiceControllers.Count==0) {
				MsgBox.Show(this,"eConnector Service not found on this computer.  The service can only be started from the computer that is hosting eServices.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string serviceErrors=ServicesHelper.StartServices(listServiceControllers);
			Cursor=Cursors.Default;
			if(!string.IsNullOrEmpty(serviceErrors)) {
				string error=Lan.g(this,"There was a problem starting eConnector Services.  Please go manually start the following eConnector Services")
					+":\r\n"+serviceErrors;
				MessageBox.Show(error);
			}
			else {
				MsgBox.Show(this,"eConnector Services Started.");
			}
			FillTextListenerServiceStatus();
			FillGridListenerService();
		}

		private void butListenerServiceHistoryRefresh_Click(object sender,EventArgs e) {
			FillTextListenerServiceStatus();
			FillGridListenerService();
		}

		private void butListenerServiceAck_Click(object sender,EventArgs e) {
			EServiceSignals.ProcessSignalsForSeverity(eServiceSignalSeverity.Error);
			FillTextListenerServiceStatus();
			FillGridListenerService();
			MsgBox.Show(this,"Errors successfully acknowledged.");
		}

		private void butListenerAlertsOff_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			//Insert a row into the eservicesignal table to indicate to all computers to stop monitoring.
			EServiceSignal signalDisable=new EServiceSignal();
			signalDisable.Description="Stop Monitoring clicked from setup window.";
			signalDisable.IsProcessed=true;
			signalDisable.ReasonCategory=0;
			signalDisable.ReasonCode=0;
			signalDisable.ServiceCode=(int)eServiceCode.ListenerService;
			signalDisable.Severity=eServiceSignalSeverity.NotEnabled;
			signalDisable.Tag="";
			signalDisable.SigDateTime=MiscData.GetNowDateTime();
			EServiceSignals.Insert(signalDisable);
			SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Listener Service monitoring manually stopped via eServices Setup window.");
			MsgBox.Show(this,"Monitoring shutdown signal sent.  This will take up to one minute.");
			FillGridListenerService();
			FillTextListenerServiceStatus();
		}

		private void butOk_Click(object sender,EventArgs e) {
			bool doRefreshCache=false;
			if(!textLogCleanupInterval.IsValid()) {
				MsgBox.Show("Invalid entry for 'Delete logs older than'. Please enter a value between 0 and 36500.");
				return;
			}
			int intervalNew=PIn.Int(textLogCleanupInterval.Text);
			doRefreshCache|=Prefs.UpdateInt(PrefName.EConnectorCleanupLoggerIntervalDays,intervalNew);
			if(checkEmailsWithDiffProcess.Checked) {
				doRefreshCache|=Prefs.UpdateInt(PrefName.SendEmailsInDiffProcess,(int)YN.Yes);
			}
			else {
				//Not checked.
				if((YN)PrefC.GetInt(PrefName.SendEmailsInDiffProcess)==YN.Yes) {
					//If it was yes before, switch it to no.
					doRefreshCache|=Prefs.UpdateInt(PrefName.SendEmailsInDiffProcess,(int)YN.No);
				}
				//Otherwise do not update it. It is either no or unknown.
			}
			if(doRefreshCache) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;	
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}