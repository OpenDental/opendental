using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml;
using CodeBase;
using OpenDentBusiness.WebTypes.AutoComm;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ConfirmationRequests{
		#region Get Methods

		///<summary>Gets the pending ConfirmationRequest row for the passed in patnum.  Will return null if the patnum with a pending row isn't found.</summary>
		public static List<ConfirmationRequest> GetPendingForPatNum(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM confirmationrequest WHERE PatNum="+POut.Long(patNum)+" AND RSVPStatus="+POut.Int((int)RSVPStatusCodes.PendingRsvp);
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		public static List<ConfirmationRequest> GetPendingForPatNumWithMessage(long patNum) {
			List<ConfirmationRequest> listConfRequests=GetPendingForPatNum(patNum);
			AutoCommSents.SetMessageBody(listConfRequests);
			return listConfRequests;
		}

		///<summary>Get the list of ConfirmationRequests for confirmation requests where the appointment was rescheduled or deleted after sending the 
		///request.</summary>
		public static List<ConfirmationRequest> GetForApptChanged() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod());
			}
			List<ApptStatus> listStatus=new List<ApptStatus>() { ApptStatus.UnschedList, ApptStatus.Broken };
			string command=@"SELECT confirmationrequest.*
				FROM confirmationrequest
				LEFT JOIN appointment ON confirmationrequest.ApptNum=appointment.AptNum
				WHERE appointment.AptNum IS NULL OR appointment.AptDateTime!=confirmationrequest.ApptDateTime
				OR appointment.AptStatus IN ("+string.Join(",",listStatus.Select(x => POut.Int((int)x)))+")";
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		///<summary>Gets all confirmation requests for the AptNums sent in.</summary>
		public static List<ConfirmationRequest> GetAllForAppts(List<long> listAptNums) {
			if(listAptNums.Count==0) {
				return new List<ConfirmationRequest>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod(),listAptNums);
			}
			string command="SELECT * FROM confirmationrequest WHERE ApptNum IN("+string.Join(",",listAptNums.Select(x => POut.Long(x)))+")";
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		#endregion

		///<summary>Get all rows where RSVPStatus==AwaitingTransmit.</summary>
		public static List<ConfirmationRequest> GetAllOutstandingForSend() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM confirmationrequest WHERE RSVPStatus = "+POut.Int((int)RSVPStatusCodes.AwaitingTransmit);
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		///<summary>Get all rows where RSVPStatus==PendingRsvp that match the apptReminderRule.</summary>
		public static List<ConfirmationRequest> GetPendingForRule(long apptReminderRule) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod(),apptReminderRule);
			}
			string command="SELECT * FROM confirmationrequest WHERE RSVPStatus = "+POut.Int((int)RSVPStatusCodes.PendingRsvp)
				+" AND ApptReminderRuleNum="+POut.Long(apptReminderRule);
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		///<summary>HQ only knows about the ShortGUID field. It treats both ShortGUIDEmail and ShortGuid as ShortGuid. 
		///Returns any client side ConfirmationRequest(s) where ShortGuid matches the server side ShortGuid field.</summary>
		public static List<ConfirmationRequest> GetConfirmationsByShortGuid(string shortGuid) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod(),shortGuid);
			}
			string command=$"SELECT * FROM confirmationrequest WHERE ShortGUID = '{POut.String(shortGuid)}'";
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		public static List<ConfirmationRequest> GetConfirmation(SmsToMobile sms) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod(),sms);
			}
			string command=$"SELECT * FROM confirmationrequest WHERE {nameof(ConfirmationRequest.MessageFk)} = {POut.Long(sms.SmsToMobileNum)} " +
				$"AND {nameof(ConfirmationRequest.MessageType)}={POut.Int((int)CommType.Text)}";
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}
		
		public static long Insert(ConfirmationRequest confirmationRequest) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				confirmationRequest.ConfirmationRequestNum=Meth.GetLong(MethodBase.GetCurrentMethod(),confirmationRequest);
				return confirmationRequest.ConfirmationRequestNum;
			}
			return Crud.ConfirmationRequestCrud.Insert(confirmationRequest);
		}

		public static void InsertMany(List<ConfirmationRequest> listConfirmationRequests) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listConfirmationRequests);
				return;
			}
			Crud.ConfirmationRequestCrud.InsertMany(listConfirmationRequests);
		}

		public static void Update(ConfirmationRequest confirmationRequest) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),confirmationRequest);
				return;
			}
			Crud.ConfirmationRequestCrud.Update(confirmationRequest);
		}

		///<summary>Expire confirmations for any appointments that have been rescheduled since sending out a confirmation request.</summary>
		public static void HandleConfirmationsApptChanged(Logger.IWriteLine log) {	
			//No remoting role check needed.		
			List<ConfirmationRequest> listChanged=GetForApptChanged();
			listChanged=listChanged.Where(x => !x.DoNotResend).ToList();//Remove those that the user specifically said to not resend
			List<string> listShortGuids=listChanged.Where(x => !string.IsNullOrWhiteSpace(x.ShortGUID)).Select(x => x.ShortGUID).ToList();
			if(listShortGuids.Count==0) {
				return;
			}
			string hqPayload=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(listShortGuids,"ListShortGuids"),
				eServiceCode.ConfirmationRequest);
			string result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().HandleConfirmationsApptChanged(hqPayload);
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				log.WriteLine(node.InnerText,LogLevel.Error);
				return;
			}
			log.WriteLine($"Deleting ConfirmationRequest entries for {listShortGuids.Count} ShortGuids.",LogLevel.Information);
			string verboseLog=string.Join("\r\n\t\t",listShortGuids.Select(x => x));
			log.WriteLine($"Deleting \r\n\t\t{verboseLog}",LogLevel.Verbose);
			//Deleting these will cause the AutoComm thread to resend where necessary.
			long countDeleted=DeleteShortGuids(listShortGuids);
			log.WriteLine($"Deleted {countDeleted} ConfirmationRequests.",LogLevel.Information);
		}

		///<summary>Update the RSVPStatusCode for the list of ConfirmationRequests matching the provided shortGuids</summary>
		public static long DeleteShortGuids(List<string> listShortGuids) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),listShortGuids);
			}
			listShortGuids=listShortGuids.FindAll(x => !string.IsNullOrWhiteSpace(x));
			if(listShortGuids.Count==0) {
				return 0;
			}
			string command="DELETE FROM confirmationrequest "
				+"WHERE ShortGUID IN('"+string.Join("','",listShortGuids.Select(x => POut.String(x)))+"')";			
			return DataCore.NonQ(command);
		}
		
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class ConfirmationRequestCache : CacheListAbs<ConfirmationRequest> {
			protected override List<ConfirmationRequest> GetCacheFromDb() {
				string command="SELECT * FROM ConfirmationRequest ORDER BY ItemOrder";
				return Crud.ConfirmationRequestCrud.SelectMany(command);
			}
			protected override List<ConfirmationRequest> TableToList(DataTable table) {
				return Crud.ConfirmationRequestCrud.TableToList(table);
			}
			protected override ConfirmationRequest Copy(ConfirmationRequest ConfirmationRequest) {
				return ConfirmationRequest.Clone();
			}
			protected override DataTable ListToTable(List<ConfirmationRequest> listConfirmationRequests) {
				return Crud.ConfirmationRequestCrud.ListToTable(listConfirmationRequests,"ConfirmationRequest");
			}
			protected override void FillCacheIfNeeded() {
				ConfirmationRequests.GetTableFromCache(false);
			}
			protected override bool IsInListShort(ConfirmationRequest ConfirmationRequest) {
				return !ConfirmationRequest.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ConfirmationRequestCache _ConfirmationRequestCache=new ConfirmationRequestCache();

		///<summary>A list of all ConfirmationRequests. Returns a deep copy.</summary>
		public static List<ConfirmationRequest> ListDeep {
			get {
				return _ConfirmationRequestCache.ListDeep;
			}
		}

		///<summary>A list of all visible ConfirmationRequests. Returns a deep copy.</summary>
		public static List<ConfirmationRequest> ListShortDeep {
			get {
				return _ConfirmationRequestCache.ListShortDeep;
			}
		}

		///<summary>A list of all ConfirmationRequests. Returns a shallow copy.</summary>
		public static List<ConfirmationRequest> ListShallow {
			get {
				return _ConfirmationRequestCache.ListShallow;
			}
		}

		///<summary>A list of all visible ConfirmationRequests. Returns a shallow copy.</summary>
		public static List<ConfirmationRequest> ListShort {
			get {
				return _ConfirmationRequestCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ConfirmationRequestCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ConfirmationRequestCache.FillCacheFromTable(table);
				return table;
			}
			return _ConfirmationRequestCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ConfirmationRequest> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ConfirmationRequest>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM confirmationrequest WHERE PatNum = "+POut.Long(patNum);
			return Crud.ConfirmationRequestCrud.SelectMany(command);
		}

		///<summary>Gets one ConfirmationRequest from the db.</summary>
		public static ConfirmationRequest GetOne(long confirmationRequestNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ConfirmationRequest>(MethodBase.GetCurrentMethod(),confirmationRequestNum);
			}
			return Crud.ConfirmationRequestCrud.SelectOne(confirmationRequestNum);
		}

		///<summary></summary>
		public static void Delete(long confirmationRequestNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),confirmationRequestNum);
				return;
			}
			Crud.ConfirmationRequestCrud.Delete(confirmationRequestNum);
		}

		

		
		*/
	}
}