using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{
///<summary></summary>
	public class ReqNeededs{
		public static DataTable Refresh(long schoolClassNum,long schoolCourseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),schoolClassNum,schoolCourseNum);
			}
			string command="SELECT * FROM reqneeded WHERE SchoolClassNum="+POut.Long(schoolClassNum)
				+" AND SchoolCourseNum="+POut.Long(schoolCourseNum)
				+" ORDER BY Descript";
			return Db.GetTable(command);
		}

		public static ReqNeeded GetReq(long reqNeededNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ReqNeeded>(MethodBase.GetCurrentMethod(),reqNeededNum);
			}
			string command="SELECT * FROM reqneeded WHERE ReqNeededNum="+POut.Long(reqNeededNum);
			return Crud.ReqNeededCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(ReqNeeded reqNeeded) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reqNeeded);
				return;
			}
			Crud.ReqNeededCrud.Update(reqNeeded);
		}

		///<summary></summary>
		public static long Insert(ReqNeeded reqNeeded) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				reqNeeded.ReqNeededNum=Meth.GetLong(MethodBase.GetCurrentMethod(),reqNeeded);
				return reqNeeded.ReqNeededNum;
			}
			return Crud.ReqNeededCrud.Insert(reqNeeded);
		}

		///<summary>Surround with try/catch.</summary>
		public static void Delete(long reqNeededNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reqNeededNum);
				return;
			}
			//still need to validate
			string command= "DELETE FROM reqneeded "
				+"WHERE ReqNeededNum = "+POut.Long(reqNeededNum);
			Db.NonQ(command);
		}

		///<summary>Returns a list with all reqneeded entries in the database.</summary>
		public static List<ReqNeeded> GetListFromDb() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ReqNeeded>>(MethodBase.GetCurrentMethod());
			}
			string command ="SELECT * FROM reqneeded ORDER BY Descript";
			return Crud.ReqNeededCrud.SelectMany(command);
		}

		///<summary>Inserts, updates, or deletes rows to reflect changes between listNew and stale listOld.</summary>
		public static void Sync(List<ReqNeeded> listReqNeededsNew,List<ReqNeeded> listReqNeededsOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listReqNeededsNew,listReqNeededsOld);//never pass DB list through the web service
				return;
			}
			Crud.ReqNeededCrud.Sync(listReqNeededsNew,listReqNeededsOld);
		}
	}
}