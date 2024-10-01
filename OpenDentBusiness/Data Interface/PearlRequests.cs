using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PearlRequests{
		#region Methods - Get
		/*
		///<summary></summary>
		public static List<PearlRequest> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PearlRequest>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM pearlrequest WHERE PatNum = "+POut.Long(patNum);
			return Crud.PearlRequestCrud.SelectMany(command);
		}

		///<summary>Gets one PearlRequest from the db.</summary>
		public static PearlRequest GetOne(long pearlRequestNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<PearlRequest>(MethodBase.GetCurrentMethod(),pearlRequestNum);
			}
			return Crud.PearlRequestCrud.SelectOne(pearlRequestNum);
		}
		*/

		///<summary>Gets the most recently sent PearlRequest from the db that matches the given DocNum.</summary>
		public static PearlRequest GetOneByDocNum(long docNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<PearlRequest>(MethodBase.GetCurrentMethod(),docNum);
			}
			string command="SELECT * FROM pearlrequest WHERE DocNum="+POut.Long(docNum)+" ORDER BY DateTSent DESC";
			return Crud.PearlRequestCrud.SelectOne(command);
		}

		///<summary>Gets the most recently sent PearlRequest from the db that matches the given RequestId.</summary>
		public static PearlRequest GetOneByRequestId(string requestId){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<PearlRequest>(MethodBase.GetCurrentMethod(),requestId);
			}
			string command="SELECT * FROM pearlrequest WHERE RequestId=\'"+POut.String(requestId)+"\' ORDER BY DateTSent DESC";
			return Crud.PearlRequestCrud.SelectOne(command);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(PearlRequest pearlRequest){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				pearlRequest.PearlRequestNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pearlRequest);
				return pearlRequest.PearlRequestNum;
			}
			return Crud.PearlRequestCrud.Insert(pearlRequest);
		}

		///<summary></summary>
		public static void Update(PearlRequest pearlRequest){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pearlRequest);
				return;
			}
			Crud.PearlRequestCrud.Update(pearlRequest);
		}

		///<summary></summary>
		public static void UpdateStatusForRequests(List<PearlRequest> listPearlRequests,EnumPearlStatus pearlStatus){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPearlRequests,pearlStatus);
				return;
			}
			List<long> listPearlRequestNums=listPearlRequests.Select(x => x.PearlRequestNum).ToList();
			string command="UPDATE pearlrequest SET RequestStatus="+POut.Enum<EnumPearlStatus>(pearlStatus)
				+" WHERE PearlRequestNum IN ("+string.Join(",",listPearlRequestNums)+")";
			Db.NonQ(command);
		}

		///<summary>DEPRECATED. Sets RequestStatus to ServicePolling for each request matching a given DocNum.</summary>
		//public static void SwitchDocsToServerPolling(List<long> listDocNums){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),listDocNums);
		//		return;
		//	}
		//	string command="UPDATE pearlrequest SET RequestStatus="+POut.Enum<EnumPearlStatus>(EnumPearlStatus.ServicePolling)
		//		+" WHERE DocNum IN ("+string.Join(",",listDocNums.Select(x => POut.Long(x)))+")";
		//	Db.NonQ(command);
		//}

		/*
		///<summary></summary>
		public static void Delete(long pearlRequestNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pearlRequestNum);
				return;
			}
			Crud.PearlRequestCrud.Delete(pearlRequestNum);
		}
		*/
		#endregion Methods - Modify
		#region Methods - Misc
		///<summary>Returns whether the request has been processed by Pearl, either with success or error.</summary>
		public static bool IsRequestHandled(PearlRequest pearlRequest) {
			if(pearlRequest==null) {
				return false;
			}
			return pearlRequest.RequestStatus.In(EnumPearlStatus.Received,EnumPearlStatus.Error);
		}

		#endregion Methods - Misc

	}
}