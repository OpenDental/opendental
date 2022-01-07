using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>HQ only. This is NOT a table type. It is a class that is populated by the Broadcaster at a fairly frequenty interval (30 seconds or so).
	///It is then serialized and saved as a ESerivceSignal via upsert. Each HQ workstation will then select that EServiceSignal very frequently and display the results.</summary>
	[Serializable()]
	public class EServiceMetrics {
		///<summary>Time at which this data was generated. It past a certain threshold in the past then consider the data invalid.</summary>
		public DateTime Timestamp=DateTime.MinValue;
		///<summary>True if all Broadcaster heartbeats are current and not critical; otherwise false.</summary>
		public bool IsBroadcasterHeartbeatOk;
		///<summary>Retreived from NexmoAPI.GetAccountBalance().</summary>
		public float AccountBalanceEuro;
		///<summary>If true then this data is valid and came from the Broadcaster AccountMaintThread; otherwise this data is not accurate.
		///Will be set after deserialization to indicate that the data was found and deserialized correctly.</summary>
		public bool IsValid;
		///<summary>If this string is not empty then assume a critical server is unavailable. This is considered a critical error.</summary>
		public string WebsitesDown="";
		///<summary>Any error message from not being able to read the eService metrics.</summary>
		public string ErrorMessage="";

		///<summary>This is derived property. Do not serialize.</summary>
		[XmlIgnore]
		public eServiceSignalSeverity Severity {
			get {
				if(!string.IsNullOrEmpty(CriticalStatus)) {
					return eServiceSignalSeverity.Critical;
				}
				return eServiceSignalSeverity.Working;
			}
		}

		///<summary>This is derived property. Do not serialize. If returns non-empty string then assume EServices is in a critical state.</summary>
		[XmlIgnore]
		public string CriticalStatus {
			get {
				try {
					if(!IsValid) {
						if(!string.IsNullOrEmpty(ErrorMessage)) {
							throw new Exception(ErrorMessage);
						}
						throw new Exception("EServiceMetrics.IsValid=false. Deserialization failed.");
					}
					if(DateTime.Now.Subtract(Timestamp)>TimeSpan.FromMinutes(5)) {
						throw new Exception("EServiceMetrics object is more than 5 minutes stale. "+ErrorMessage);
					}
					if(!IsBroadcasterHeartbeatOk) {
						throw new Exception("Broadcaster or Proxy thread heartbeat is invalid. "+ErrorMessage);
					}
					if(!string.IsNullOrEmpty(WebsitesDown)) {
						throw new Exception(WebsitesDown+ErrorMessage);
					}
					return "";
				}
				catch(Exception e) {
					return "EService Critical Error: "+e.Message;
				}
			}
		}
		
		public delegate void EServiceMetricsArgs(EServiceMetrics eServiceMetrics);

		///<summary>Gets one EServiceSignalHQ from the serviceshq db located on SERVER184. Returns null in case of failure.</summary>
		public static EServiceMetrics GetEServiceMetricsFromSignalHQ() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EServiceMetrics>(MethodBase.GetCurrentMethod());
			}
			EServiceMetrics eServiceMetric=new EServiceMetrics();
			if(PrefC.ContainsKey("ServicesHqDoNotConnect") && PrefC.GetBool(PrefName.ServicesHqDoNotConnect)) {
				eServiceMetric.ErrorMessage="Not allowed to connect to the serviceshq database.";
				return eServiceMetric;
			}
			string dbPassword;
			if(!CDT.Class1.Decrypt(PrefC.GetString(PrefName.ServicesHqMySqpPasswordObf),out dbPassword)) {
				eServiceMetric.ErrorMessage="Unable to decrypt serviceshq password";
				return eServiceMetric;
			}
			try {
				DataAction.Run(() => {
						//See EServiceSignalHQs.GetEServiceMetrics() for details.
						string command=@"SELECT 0 EServiceSignalNum, h.* FROM eservicesignalhq h 
							WHERE h.ReasonCode=1024
								AND h.ReasonCategory=1
								AND h.ServiceCode=2
								AND h.RegistrationKeyNum=-1
							ORDER BY h.SigDateTime DESC 
							LIMIT 1";
						EServiceSignal eServiceSignal=Crud.EServiceSignalCrud.SelectOne(command);
						if(eServiceSignal!=null) {
							using(XmlReader reader=XmlReader.Create(new System.IO.StringReader(eServiceSignal.Tag))) {
								eServiceMetric=(EServiceMetrics)new XmlSerializer(typeof(EServiceMetrics)).Deserialize(reader);
							}
							eServiceMetric.IsValid=true;
						}
					},
					PrefC.GetString(PrefName.ServicesHqServer),PrefC.GetString(PrefName.ServicesHqDatabase),PrefC.GetString(PrefName.ServicesHqMySqlUser),
					dbPassword,"","",DatabaseType.MySql);
			}
			catch(Exception ex) {
				eServiceMetric.ErrorMessage=ex.Message;
			}
			return eServiceMetric;
		}

		#region Calculate metrics

		private static DataTable GetSmsInbound(DateTime dateTimeStart,DateTime dateTimeEnd) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTimeStart,dateTimeEnd);
			}
			//-- Returns Count of outbound messages and total customer charges accrued.
			string command=@"
				SELECT 
				  COUNT(*) NumMessages
				FROM 
				  smsmoterminated t
				WHERE
				  t.DateTimeODRcv>="+POut.DateT(dateTimeStart,true)+@"
				  AND t.DateTimeODRcv <"+POut.DateT(dateTimeEnd,true)+";";
			return Db.GetTable(command);
		}

		private static DataTable GetSmsOutbound(DateTime dateTimeStart,DateTime dateTimeEnd) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTimeStart,dateTimeEnd);
			}
			//-- Returns Count of outbound messages and total customer charges accrued.
			string command=@"
				SELECT 
				  COUNT(*) NumMessages,
				  SUM(t.MsgChargeUSD) MsgChargeUSDTotal
				FROM 
				  smsmtterminated t
				WHERE
				  t.MsgStatusCust IN(1,2,3,4)
				  AND t.DateTimeTerminated>="+POut.DateT(dateTimeStart,true)+@"
				  AND t.DateTimeTerminated <"+POut.DateT(dateTimeEnd,true)+";";
			return Db.GetTable(command);
		}

		private static DataTable GetBroadcastersErrors() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			//-- Returns Count of all unprocessed rows which have severity of Warning or Error.
			string command=@"
				SELECT e.Severity,COUNT(*) CountOf
				  FROM eservicesignalhq e
				WHERE
				  e.RegistrationKeyNum=-1  -- HQ
				  AND e.IsProcessed=0 -- NOT processed
				  AND 
				  (
					e.ReasonCode<>1004 -- NOT Heartbeat
					OR e.ReasonCode<>1005 -- NOT ThreadExit
				  ) 
				  AND 
				  (
					e.Severity=3 -- Warning
					OR e.Severity=4 -- Error
				  )
				GROUP BY
				  e.Severity
				;";
			return Db.GetTable(command);
		}

		#endregion
	}	
}
