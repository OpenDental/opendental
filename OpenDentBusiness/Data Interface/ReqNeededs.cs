using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{
///<summary></summary>
	public class ReqNeededs{
		public static DataTable Refresh(long schoolClass,long schoolCourse) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),schoolClass,schoolCourse);
			}
			string command="SELECT * FROM reqneeded WHERE SchoolClassNum="+POut.Long(schoolClass)
				+" AND SchoolCourseNum="+POut.Long(schoolCourse)
				+" ORDER BY Descript";
			return Db.GetTable(command);
		}

		public static ReqNeeded GetReq(long reqNeededNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ReqNeeded>(MethodBase.GetCurrentMethod(),reqNeededNum);
			}
			string command="SELECT * FROM reqneeded WHERE ReqNeededNum="+POut.Long(reqNeededNum);
			return Crud.ReqNeededCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(ReqNeeded req) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),req);
				return;
			}
			Crud.ReqNeededCrud.Update(req);
		}

		///<summary></summary>
		public static long Insert(ReqNeeded req) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				req.ReqNeededNum=Meth.GetLong(MethodBase.GetCurrentMethod(),req);
				return req.ReqNeededNum;
			}
			return Crud.ReqNeededCrud.Insert(req);
		}

		///<summary>Surround with try/catch.</summary>
		public static void Delete(long reqNeededNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReqNeeded>>(MethodBase.GetCurrentMethod());
			}
			string command ="SELECT * FROM reqneeded ORDER BY Descript";
			return Crud.ReqNeededCrud.SelectMany(command);
		}

		///<summary>Inserts, updates, or deletes rows to reflect changes between listNew and stale listOld.</summary>
		public static void Sync(List<ReqNeeded> listNew,List<ReqNeeded> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,listOld);//never pass DB list through the web service
				return;
			}
			Crud.ReqNeededCrud.Sync(listNew,listOld);
		}
	}


}













