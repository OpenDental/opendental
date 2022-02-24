using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoSchedules{
		#region Get Methods
		///<summary>Gets one OrthoSchedule from the database.</summary>
		public static OrthoSchedule GetOne(long orthoScheduleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrthoSchedule>(MethodBase.GetCurrentMethod(),orthoScheduleNum);
			}
			return Crud.OrthoScheduleCrud.SelectOne(orthoScheduleNum);
		}

		///<summary>Gets all ortho schedules for a list of orthoschedulenums.</summary>
		public static List<OrthoSchedule> GetMany(List<long> listOrthoScheduleNums) {
			if(listOrthoScheduleNums.Count==0) {
				return new List<OrthoSchedule>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoSchedule>>(MethodBase.GetCurrentMethod(),listOrthoScheduleNums);
			}
			string command=$"SELECT * FROM orthoschedule WHERE orthoschedule.OrthoScheduleNum IN({string.Join(",",listOrthoScheduleNums)})";
			return Crud.OrthoScheduleCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Insert
		///<summary>Insert a OrthoSchedule into the database.</summary>
		public static long Insert(OrthoSchedule orthoSchedule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				orthoSchedule.OrthoScheduleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoSchedule);
				return orthoSchedule.OrthoScheduleNum;
			}
			return Crud.OrthoScheduleCrud.Insert(orthoSchedule);
		}
		#endregion Insert

		#region Update
		///<summary>Update only data that is different in newOrthoSchedule</summary>
		public static void Update(OrthoSchedule newOrthoSchedule,OrthoSchedule oldOrthoSchedule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),newOrthoSchedule,oldOrthoSchedule);
				return;
			}
			Crud.OrthoScheduleCrud.Update(newOrthoSchedule,oldOrthoSchedule);
		}
		#endregion Update

		#region Delete
		/////<summary>Delete a OrthoSchedule from the database.</summary>
		//public static void Delete(long orthoScheduleNum) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoScheduleNum);
		//		return;
		//	}
		//	Crud.OrthoScheduleCrud.Delete(orthoScheduleNum);
		//}
		#endregion Delete

		#region Misc Methods
		public static int CalculatePlannedVisitsCount(double bandingAmount,double debondAmount,double visitAmount,double totalFee) {
			//No need to check RemotingRole; no call to db.
			if(CompareDouble.IsZero(visitAmount)) {
				return 0;
			}
			double allVisitsAmount=Math.Round((totalFee-(bandingAmount+debondAmount))*100)/100;
			int plannedVisitsCount=(int)Math.Round(allVisitsAmount/visitAmount);
			if(CompareDouble.IsLessThan(plannedVisitsCount*visitAmount,allVisitsAmount)){
				plannedVisitsCount++;
			}
			return plannedVisitsCount;
		}
		#endregion Misc Methods

		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		///<summary></summary>
		public static List<OrthoSchedule> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoSchedule>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthoschedule WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrthoScheduleCrud.SelectMany(command);
		}
		///<summary></summary>
		public static long Insert(OrthoSchedule orthoSchedule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				orthoSchedule.OrthoScheduleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoSchedule);
				return orthoSchedule.OrthoScheduleNum;
			}
			return Crud.OrthoScheduleCrud.Insert(orthoSchedule);
		}
		///<summary></summary>
		public static void Update(OrthoSchedule orthoSchedule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoSchedule);
				return;
			}
			Crud.OrthoScheduleCrud.Update(orthoSchedule);
		}
		*/
	}
}