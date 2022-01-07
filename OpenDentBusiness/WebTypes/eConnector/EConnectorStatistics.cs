using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness.WebTypes {
	///<summary>This class is used to hold interesting pieces of information regarding a customer's eConnector.</summary>
	public class EConnectorStatistics : WebBase {
		///<summary>The computer where the eConnector is running.</summary>
		public string EConnectorComputerName;
		///<summary>The user under which the eConnector is running.</summary>
		public string EConnectorDomainUserName;
		///<summary>An IP address of the computer where the eConnector is running.</summary>
		public string EConnectorIP;
		///<summary>The preference in the database.</summary>
		public bool HasClinicsEnabled;
		///<summary>The number of clinics that are currently in use.</summary>
		public int CountActiveClinics;
		///<summary>The number of clinics that are not currently in use.</summary>
		public int CountInactiveClinics;
		///<summary>The number of patients who have a completed procedure in the last two years.</summary>
		public int CountActivePatients;
		///<summary>The number of patients who have not had a completed procedure in the last two years.</summary>
		public int CountNonactivePatients;
		///<summary>The signals of type ListenerService from the last 30 days. Equivalent to what displays in the History grid in the EConnector tab
		///in the eServices Setup window.</summary>
		public List<EServiceSignal> ListEServiceSignals;
		///<summary>The time that these statistics were generated. Will be in the time zone of eConnector.</summary>
		public DateTime DateTimeNow;
		///<summary>A few choice prefs.</summary>
		public List<Pref> ListEServicePrefs;
		///<summary>Public IP Address of the network hosting the eConnector</summary>
		public string PublicIP;

		///<summary>Send a summary of eConnector statistics to OD HQ. This should only be called from the eConnector.</summary>
		public static void UpdateEConnectorStats() {
			EConnectorStatistics eConnStats=new EConnectorStatistics() {
				ListEServiceSignals=new List<EServiceSignal>(),
				ListEServicePrefs=new List<Pref>(),
			};
			eConnStats.EConnectorComputerName=Environment.MachineName;
			eConnStats.EConnectorDomainUserName=Environment.UserName;
			eConnStats.EConnectorIP=ODEnvironment.GetLocalIPAddress();
			eConnStats.HasClinicsEnabled=PrefC.HasClinicsEnabled;
			if(PrefC.HasClinicsEnabled) {
				eConnStats.CountActiveClinics=OpenDentBusiness.Clinics.GetCount();
				eConnStats.CountInactiveClinics=OpenDentBusiness.Clinics.GetCount()-eConnStats.CountActiveClinics;
			}
			else {
				eConnStats.CountActiveClinics=0;
				eConnStats.CountInactiveClinics=OpenDentBusiness.Clinics.GetCount();
			}
			if(DateTime.Now.Hour==0) { //These are heavy queries so only run them once a day around midnight.
				eConnStats.CountActivePatients=OpenDentBusiness.Procedures.GetCountPatsComplete(DateTime.Today.AddYears(-2),DateTime.Today);
				eConnStats.CountNonactivePatients=OpenDentBusiness.Patients.GetPatCountAll()-eConnStats.CountActivePatients;
				eConnStats.ListEServiceSignals=OpenDentBusiness.EServiceSignals.GetServiceHistory(eServiceCode.ListenerService,DateTime.Today.AddDays(-30),
					DateTime.Today,30);
			}
			eConnStats.DateTimeNow=DateTime.Now;
			foreach(PrefName prefName in Enum.GetValues(typeof(PrefName))) {
				if(ListTools.In(prefName,
					PrefName.RegistrationKey,
					PrefName.ProgramVersion,
					PrefName.DataBaseVersion,
					PrefName.TextingDefaultClinicNum,
					PrefName.WebServiceServerName,
					PrefName.SendEmailsInDiffProcess,
					PrefName.EmailAlertMaxConsecutiveFails,
					PrefName.AutoCommNumClinicsParallel,
					PrefName.AutomaticCommunicationTimeStart,
					PrefName.AutomaticCommunicationTimeEnd)
					|| prefName.ToString().StartsWith("WebSched")
					|| prefName.ToString().StartsWith("ApptConfirm")
					|| prefName.ToString().StartsWith("ApptRemind")
					|| prefName.ToString().StartsWith("ApptEConfirm")
					|| prefName.ToString().StartsWith("Recall")
					|| prefName.ToString().StartsWith("PatientPortal")
					|| prefName.ToString().StartsWith("Sms")) 
				{
					try {
						eConnStats.ListEServicePrefs.Add(Prefs.GetPref(prefName.ToString()));
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
				}
			}
			List<EConnectorStatistics> listStatsToSend=new List<EConnectorStatistics> { eConnStats };
			string dbStats=PrefC.GetString(PrefName.EConnectorStatistics);
			List<EConnectorStatistics> listDbStats=DeserializeListFromJson(dbStats)??new List<EConnectorStatistics>();
			bool doCreateAlert=false;
			foreach(EConnectorStatistics stats in listDbStats) {
				//If a different eConnector is saving stats, add that one to the list to be sent to HQ.
				if(!AreSameEConnector(eConnStats,stats) && (eConnStats.DateTimeNow-stats.DateTimeNow).TotalHours < 23) {
					stats.ListEServicePrefs=new List<Pref>();//To save on bandwidth
					stats.ListEServiceSignals=new List<EServiceSignal>();
					listStatsToSend.Add(stats);
					if((eConnStats.DateTimeNow-stats.DateTimeNow).TotalHours < 3) {
						doCreateAlert=true;
					}
				}
			}
			if(doCreateAlert && AlertItems.RefreshForType(AlertType.MultipleEConnectors).Count==0) {
				AlertItem alert=new AlertItem {
					Actions=ActionType.MarkAsRead | ActionType.Delete,
					Description=Lans.g("EConnectorStats","eConnector services are being run on these computers:")+" "
						+string.Join(", ",listStatsToSend.Select(x => x.EConnectorComputerName)),
					Severity=SeverityType.High,
					Type=AlertType.MultipleEConnectors,
				};
				AlertItems.Insert(alert);
			}
			string statsStr=SerializeToJson(listStatsToSend);
			OpenDentBusiness.Prefs.UpdateString(PrefName.EConnectorStatistics,statsStr);
			string payload=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(statsStr,"EConnectorStatsStr"),
				eServiceCode.ListenerService);
			WebServiceMainHQProxy.GetWebServiceMainHQInstance().SetEConnectorStatsAsync(payload);
		}
		
		///<summary>Returns an empty list if deserialization fails.</summary>
		public static List<EConnectorStatistics> DeserializeListFromJson(string jsonString) {
			try {
				return JsonConvert.DeserializeObject<List<EConnectorStatistics>>(jsonString)??new List<EConnectorStatistics>();
			}
			catch(Exception ex) {
				ex.DoNothing();
				try {
					//Previous to 17.3.21ish, eConnectors sent over a single EConnectorStats instead of a list.
					return new List<EConnectorStatistics> { JsonConvert.DeserializeObject<EConnectorStatistics>(jsonString)??new EConnectorStatistics() };
				}
				catch(Exception e) {
					e.DoNothing();
				}
				return new List<EConnectorStatistics>();
			}
		}

		public static string SerializeToJson(List<EConnectorStatistics> listStats) {
			return JsonConvert.SerializeObject(listStats);
		}

		///<summary>Get the most recent entry for each computer.</summary>
		public static List<EConnectorStatistics> GroupByComputerNameAndPublicIp(List<EConnectorStatistics> listIn) {
			try {
				List<EConnectorStatistics> listOut=listIn.OrderByDescending(x => x.DateTimeNow)
					.ThenBy(x => x.PublicIP)//Tie Breaker
					.ThenBy(x => x.EConnectorComputerName)//Tie Breaker
					.GroupBy(x => new { x.PublicIP,x.EConnectorComputerName })
					.Select(x => x.First()).ToList();
				return listOut;
			}
			catch(Exception e) {
				e.DoNothing();
			}
			return listIn;
		}

		///<summary>Compares two EConnectorStatistics to determine if they refer to the same eConnector</summary>
		public static bool AreSameEConnector(EConnectorStatistics eConnA,EConnectorStatistics eConnB) {
			if(eConnA==null || eConnB==null) {
				return false;
			}
			string nameA=eConnA.EConnectorComputerName.ToLower().Trim();
			string ipA=(eConnA.PublicIP??"").ToLower().Trim();
			string nameB=eConnB.EConnectorComputerName.ToLower().Trim();
			string ipB=(eConnB.PublicIP??"").ToLower().Trim();
			return (nameA==nameB && ipA==ipB);
		}
	}
}
