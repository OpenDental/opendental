using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TreatPlanAttaches{
		#region Update

		///<summary>Sets the priority for the procedures passed in that are associated to the designated treatment plan.</summary>
		public static void SetPriorityForTreatPlanProcs(long priority,long treatPlanNum,List<long> listProcNums) {
			if(listProcNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),priority,treatPlanNum,listProcNums);
				return;
			}
			Db.NonQ($@"UPDATE treatplanattach SET Priority = {POut.Long(priority)}
				WHERE TreatPlanNum = {POut.Long(treatPlanNum)}
				AND ProcNum IN({string.Join(",",listProcNums.Select(x => POut.Long(x)))})");
		}

		#endregion

		///<summary></summary>
		public static long Insert(TreatPlanAttach treatPlanAttach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				treatPlanAttach.TreatPlanAttachNum=Meth.GetLong(MethodBase.GetCurrentMethod(),treatPlanAttach);
				return treatPlanAttach.TreatPlanAttachNum;
			}
			return Crud.TreatPlanAttachCrud.Insert(treatPlanAttach);
		}

		///<summary></summary>
		public static void Update(TreatPlanAttach treatPlanAttach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanAttach);
				return;
			}
			Crud.TreatPlanAttachCrud.Update(treatPlanAttach);
		}

		///<summary></summary>
		public static void Delete(long treatPlanAttachNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanAttachNum);
				return;
			}
			Crud.TreatPlanAttachCrud.Delete(treatPlanAttachNum);
		}

		///<summary></summary>
		public static List<TreatPlanAttach> GetAllForPatNum(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TreatPlanAttach>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<TreatPlan> listTreatPlans=TreatPlans.GetAllForPat(patNum);
			if(listTreatPlans.Count==0) {
				return new List<TreatPlanAttach>();
			}
			return GetAllForTPs(listTreatPlans.Select(x => x.TreatPlanNum).Distinct().ToList());
		}

		///<summary>Gets all treatplanattaches with TreatPlanNum in listTpNums.</summary>
		public static List<TreatPlanAttach> GetAllForTPs(List<long> listTpNums) {
			if(listTpNums.Count==0) {
				return new List<TreatPlanAttach>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TreatPlanAttach>>(MethodBase.GetCurrentMethod(),listTpNums);
			}
			string command="SELECT * FROM treatplanattach WHERE TreatPlanNum IN ("+string.Join(",",listTpNums)+")";
			return Crud.TreatPlanAttachCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<TreatPlanAttach> GetAllForTreatPlan(long treatPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TreatPlanAttach>>(MethodBase.GetCurrentMethod(),treatPlanNum);
			}
			string command="SELECT * FROM treatplanattach WHERE TreatPlanNum="+POut.Long(treatPlanNum);
			return Crud.TreatPlanAttachCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void DeleteOrphaned() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//Orphaned TreatPlanAttaches due to missing treatment plans
			string command="DELETE FROM treatplanattach WHERE TreatPlanNum NOT IN (SELECT TreatPlanNum FROM treatplan)";
			Db.NonQ(command);
			//Orphaned TreatPlanAttaches due to missing procedures or procedures that are no longer TP or TPi status.
			command="DELETE FROM treatplanattach WHERE ProcNum NOT IN (SELECT ProcNum FROM procedurelog "+
				"WHERE ProcStatus IN ("+string.Join(",",new[]{(int)ProcStat.TP,(int)ProcStat.TPi})+"))";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Sync(List<TreatPlanAttach> listTreatPlanAttachNew,long treatPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTreatPlanAttachNew,treatPlanNum);
				return;
			}
			List<TreatPlanAttach> listTreatPlanAttachDB=TreatPlanAttaches.GetAllForTreatPlan(treatPlanNum);
			Crud.TreatPlanAttachCrud.Sync(listTreatPlanAttachNew,listTreatPlanAttachDB);
		}

		///<summary></summary>
		public static void DeleteMany(List<TreatPlanAttach> listTreatPlanAttaches) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTreatPlanAttaches);
				return;
			}
			foreach(TreatPlanAttach treatPlanAttach in listTreatPlanAttaches) {
				Crud.TreatPlanAttachCrud.Delete(treatPlanAttach.TreatPlanAttachNum);
			}
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<TreatPlanAttach> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TreatPlanAttach>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM treatplanattach WHERE PatNum = "+POut.Long(patNum);
			return Crud.TreatPlanAttachCrud.SelectMany(command);
		}

		///<summary>Gets one TreatPlanAttach from the db.</summary>
		public static TreatPlanAttach GetOne(long treatPlanAttachNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TreatPlanAttach>(MethodBase.GetCurrentMethod(),treatPlanAttachNum);
			}
			return Crud.TreatPlanAttachCrud.SelectOne(treatPlanAttachNum);
		}

		
		*/
	}
}