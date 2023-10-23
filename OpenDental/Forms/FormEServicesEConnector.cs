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
		private Color _colorCritBackground=Color.OrangeRed;
		///<summary>The text color used when the OpenDentalCustListener service is down.
		private Color _colorCritText=Color.Yellow;
		///<summary>The background color used when the OpenDentalCustListener service has an error that has not be processed.
		private Color _colorErrBackground=Color.LightGoldenrodYellow;
		///<summary>The text color used when the OpenDentalCustListener service has an error that has not be processed.
		private Color _colorErrText=Color.OrangeRed;
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
			checkEnableEServicesListener.Checked=PrefC.GetBool(PrefName.EServiceListenerEnabled);
			//Disable certain buttons but let them continue to view.
			butListenerServiceAck.Enabled=Security.IsAuthorized(EnumPermType.EServicesSetup,suppressMessage:true);
		}

		///<summary>Updates the text box that is displaying the current status of the Listener Service.  Returns the status just in case other logic is needed outside of updating the status box.</summary>
		private eServiceSignalSeverity FillTextListenerServiceStatus() {
			eServiceSignalSeverity eserviceSignalSeverity=EServiceSignals.GetListenerServiceStatus();
			if(eserviceSignalSeverity==eServiceSignalSeverity.Critical) {
				textListenerServiceStatus.BackColor=_colorCritBackground;
				textListenerServiceStatus.ForeColor=_colorCritText;
				butStartListenerService.Enabled=true;
			}
			else if(eserviceSignalSeverity==eServiceSignalSeverity.Error) {
				textListenerServiceStatus.BackColor=_colorErrBackground;
				textListenerServiceStatus.ForeColor=_colorErrText;
				butStartListenerService.Enabled=true;
			}
			else {
				textListenerServiceStatus.BackColor=SystemColors.Control;
				textListenerServiceStatus.ForeColor=SystemColors.WindowText;
				butStartListenerService.Enabled=false;
			}
			textListenerServiceStatus.Text=eserviceSignalSeverity.ToString();
			return eserviceSignalSeverity;
		}

		private void butInstallEConnector_Click(object sender,EventArgs e) {
			DialogResult dialogResult;
			//Check to see if the update server preference is set.
			//If set, make sure that this is set to the computer currently logged on.
			string updateServerName=PrefC.GetString(PrefName.WebServiceServerName);
			//Only ask the user if they want to set the Update Server Name preference if it is not already set.
			bool doOverrideBlankUpdateServerName=false;
			bool isInvalidUpdateServerNameAllowed=false;
			if(string.IsNullOrWhiteSpace(updateServerName)) {
				dialogResult=MessageBox.Show(Lan.g(this,"The computer that has the eConnector service installed should be set as the Update Server.")+"\r\n"
					+Lan.g(this,"Would you like to make this computer the Update Server?"),"",MessageBoxButtons.YesNoCancel);
				if(dialogResult==DialogResult.Cancel) {
					return;
				}
				else if(dialogResult==DialogResult.Yes) {
					doOverrideBlankUpdateServerName=true;
				}
			}
			else if(!ODEnvironment.IdIsThisComputer(updateServerName)) {
				dialogResult=MessageBox.Show(Lan.g(this,"The eConnector service should be installed on the Update Server")+": "+updateServerName+"\r\n"
					+Lan.g(this,"Are you trying to install the eConnector on a different computer by accident?"),"",MessageBoxButtons.YesNoCancel);
				//Only saying No to this message box pop up will allow the user to continue (meaning they fully understand what they are getting into).
				if(dialogResult!=DialogResult.No) {
					return;
				}
				isInvalidUpdateServerNameAllowed=true;//Allow the user to do whatever they want I guess.
			}
			//At this point the user wants to install the eConnector service (or upgrade the old cust listener to the eConnector).
			if(!PrefL.UpgradeOrInstallEConnector(false,updateServerName:updateServerName,doOverrideBlankUpdateServerName:doOverrideBlankUpdateServerName,
				isInvalidUpdateServerNameAllowed:isInvalidUpdateServerNameAllowed))
			{
				return;//Warning messages would have already been shown to the user, simply return.
			}
			butInstallEConnector.Enabled=false;
			FillTextListenerServiceStatus();
			FillGridListenerService();
			MsgBox.Show(this,"eConnector successfully installed");
		}

		private void FillGridListenerService() {
			//Display some historical information for the last 30 days in this grid about the lifespan of the listener heartbeats.
			List<EServiceSignal> listEServiceSignals=EServiceSignals.GetServiceHistory(eServiceCode.ListenerService,DateTime.Today.AddDays(-30),DateTime.Today);
			gridListenerServiceStatusHistory.BeginUpdate();
			gridListenerServiceStatusHistory.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"DateTime"),120);
			gridListenerServiceStatusHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),90);
			gridListenerServiceStatusHistory.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Details"),80);
			col.IsWidthDynamic=true;
			gridListenerServiceStatusHistory.Columns.Add(col);
			gridListenerServiceStatusHistory.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<listEServiceSignals.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEServiceSignals[i].SigDateTime.ToString());
				row.Cells.Add(listEServiceSignals[i].Severity.ToString());
				row.Cells.Add(listEServiceSignals[i].Description.ToString());
				//Color the row if it is an error that has not been processed.
				if(listEServiceSignals[i].Severity==eServiceSignalSeverity.Error&&!listEServiceSignals[i].IsProcessed) {
					row.ColorBackG=_colorErrBackground;
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
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
				return;
			}
			//Insert a row into the eservicesignal table to indicate to all computers to stop monitoring.
			EServiceSignal eServiceSignal=new EServiceSignal();
			eServiceSignal.Description="Stop Monitoring clicked from setup window.";
			eServiceSignal.IsProcessed=true;
			eServiceSignal.ReasonCategory=0;
			eServiceSignal.ReasonCode=0;
			eServiceSignal.ServiceCode=(int)eServiceCode.ListenerService;
			eServiceSignal.Severity=eServiceSignalSeverity.NotEnabled;
			eServiceSignal.Tag="";
			eServiceSignal.SigDateTime=MiscData.GetNowDateTime();
			EServiceSignals.Insert(eServiceSignal);
			SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,0,"Listener Service monitoring manually stopped via eServices Setup window.");
			MsgBox.Show(this,"Monitoring shutdown signal sent.  This will take up to one minute.");
			FillGridListenerService();
			FillTextListenerServiceStatus();
		}

		private void butSave_Click(object sender,EventArgs e) {
			bool doRefreshCache=false;
			if(!textLogCleanupInterval.IsValid()) {
				MsgBox.Show("Invalid entry for 'Delete logs older than'. Please enter a value between 0 and 36500.");
				return;
			}
			int interval=PIn.Int(textLogCleanupInterval.Text);
			doRefreshCache|=Prefs.UpdateInt(PrefName.EConnectorCleanupLoggerIntervalDays,interval);
			doRefreshCache|=Prefs.UpdateBool(PrefName.EServiceListenerEnabled,checkEnableEServicesListener.Checked);
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

	}
}