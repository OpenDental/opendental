using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ScheduleOps {
		#region Get Methods
		///<summary></summary>
		public static List<ScheduleOp> GetForSched(long scheduleNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ScheduleOp>>(MethodBase.GetCurrentMethod(), scheduleNum);
			}
			string command="SELECT * FROM scheduleop ";
			command+="WHERE schedulenum = "+scheduleNum;
			return Crud.ScheduleOpCrud.SelectMany(command);
		}

		///<summary>Returns a list of ScheduleOps filtered by either scheduleNum or operatoryNum. Supplying both returns an empty list.</summary>
		public static List<ScheduleOp> GetScheduleOpsForApi(int limit,int offset,long scheduleNum,long operatoryNum) {
			if(scheduleNum>0 && operatoryNum>0) {//Shouldn't be possible, but just in case.
				return new List<ScheduleOp>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ScheduleOp>>(MethodBase.GetCurrentMethod(),limit,offset,scheduleNum,operatoryNum);
			}
			string command="SELECT * FROM scheduleop ";
			if(scheduleNum>0) {
				command+="WHERE scheduleNum="+POut.Long(scheduleNum)+" ";
			}
			if(operatoryNum>0) {
				command+="WHERE operatoryNum="+POut.Long(operatoryNum)+" ";
			}
			command+="ORDER BY ScheduleOpNum "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.ScheduleOpCrud.SelectMany(command);
		}

		///<summary>Gets all the ScheduleOps for the list of schedules.</summary>
		public static List<ScheduleOp> GetForSchedList(List<Schedule> schedules) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ScheduleOp>>(MethodBase.GetCurrentMethod(),schedules);
			}
			if(schedules==null || schedules.Count==0) {
				return new List<ScheduleOp>();
			}
			string command="SELECT * FROM scheduleop WHERE ScheduleNum IN ("+string.Join(",",schedules.Select(x=>x.ScheduleNum))+")";
			return Crud.ScheduleOpCrud.SelectMany(command);
		}

		///<summary>Gets all the ScheduleOps for the list of schedules.  Only returns ScheduleOps for the list of operatories passed in.
		///Necessary in the situation that a provider has two operatories but only one schedule that is assigned to both operatories.</summary>
		public static List<ScheduleOp> GetForSchedList(List<Schedule> schedules,List<long> listOpNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ScheduleOp>>(MethodBase.GetCurrentMethod(),schedules,listOpNums);
			}
			if(schedules==null || schedules.Count==0 || listOpNums==null || listOpNums.Count==0) {
				return new List<ScheduleOp>();
			}
			string command="SELECT * FROM scheduleop "
				+"WHERE ScheduleNum IN ("+string.Join(",",schedules.Select(x => POut.Long(x.ScheduleNum)))+") "
				+"AND OperatoryNum IN ("+string.Join(",",listOpNums.Select(x => POut.Long(x)))+")";
			return Crud.ScheduleOpCrud.SelectMany(command);
		}
		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(ScheduleOp op) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				op.ScheduleOpNum=Meth.GetLong(MethodBase.GetCurrentMethod(),op);
				return op.ScheduleOpNum;
			}
			return Crud.ScheduleOpCrud.Insert(op);
		}
		#endregion

		#region Delete
		public static void DeleteBatch(List<long> listSchedOpNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSchedOpNums);
				return;
			}
			if(listSchedOpNums==null || listSchedOpNums.Count==0) {
				return;
			}
			string command = "DELETE FROM scheduleop WHERE ScheduleOpNum IN ("+string.Join(",",listSchedOpNums)+")";
			Db.NonQ(command);
		}
		#endregion
	}

}













