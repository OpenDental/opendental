using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcTPs {
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
			Db.NonQ($@"UPDATE proctp SET Priority = {POut.Long(priority)}
				WHERE TreatPlanNum = {POut.Long(treatPlanNum)}
				AND ProcNumOrig IN({string.Join(",",listProcNums.Select(x => POut.Long(x)))})");
		}

		#endregion

		///<summary>Gets all ProcTPs for a given Patient ordered by ItemOrder.</summary>
		public static ProcTP[] Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ProcTP[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM proctp "
				+"WHERE PatNum="+POut.Long(patNum)
				+" ORDER BY ItemOrder";
			return Crud.ProcTPCrud.SelectMany(command).ToArray();
		}

		///<summary>Ordered by ItemOrder.</summary>
		public static List<ProcTP> RefreshForTP(long tpNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcTP>>(MethodBase.GetCurrentMethod(),tpNum);
			}
			string command="SELECT * FROM proctp "
				+"WHERE TreatPlanNum="+POut.Long(tpNum)
				+" ORDER BY ItemOrder";
			DataTable table=Db.GetTable(command);
			return Crud.ProcTPCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(ProcTP proc){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			Crud.ProcTPCrud.Update(proc);
		}

		///<summary></summary>
		public static long Insert(ProcTP proc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				proc.ProcTPNum=Meth.GetLong(MethodBase.GetCurrentMethod(),proc);
				return proc.ProcTPNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			proc.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.ProcTPCrud.Insert(proc);
		}

		///<summary></summary>
		public static void InsertOrUpdate(ProcTP proc, bool isNew){
			//No need to check RemotingRole; no call to db.
			if(isNew){
				Insert(proc);
			}
			else{
				Update(proc);
			}
		}

		///<summary>There are no dependencies.</summary>
		public static void Delete(ProcTP proc){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			string command= "DELETE from proctp WHERE ProcTPNum = '"+POut.Long(proc.ProcTPNum)+"'";
 			Db.NonQ(command);
		}

		///<summary>Gets a list for just one tp.  Used in TP module.  Supply a list of all ProcTPs for pt.</summary>
		public static ProcTP[] GetListForTP(long treatPlanNum,ProcTP[] listAll) {
			//No need to check RemotingRole; no call to db.
			ArrayList AL=new ArrayList();
			for(int i=0;i<listAll.Length;i++){
				if(listAll[i].TreatPlanNum!=treatPlanNum){
					continue;
				}
				AL.Add(listAll[i]);
			}
			ProcTP[] retVal=new ProcTP[AL.Count];
			AL.CopyTo(retVal);
			return retVal;
		}

		///<summary>No dependencies to worry about.</summary>
		public static void DeleteForTP(long treatPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanNum);
				return;
			}
			string command="DELETE FROM proctp "
				+"WHERE TreatPlanNum="+POut.Long(treatPlanNum);
			Db.NonQ(command);
		}

		public static List<ProcTP> GetForProcs(List<long> listProcNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcTP>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			if(listProcNums.Count==0) {
				return new List<ProcTP>();
			}
			string command = "SELECT * FROM proctp "
				+"WHERE proctp.ProcNumOrig IN ("+ string.Join(",",listProcNums) +")";
			return Crud.ProcTPCrud.SelectMany(command);
		}

		///<summary>Returns only three columns from all ProcTPs -- TreatPlanNum, PatNum, and ProcNumOrig.</summary>
		public static List<ProcTP> GetAllLim() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProcTP>>(MethodBase.GetCurrentMethod());
			}
			string command = "SELECT TreatPlanNum,PatNum,ProcNumOrig FROM proctp";
			DataTable table = Db.GetTable(command);
			List<ProcTP> listProcTpsLim = new List<ProcTP>();
			foreach(DataRow row in table.Rows) {
				ProcTP procTp = new ProcTP();
				procTp.TreatPlanNum=PIn.Long(row["TreatPlanNum"].ToString());
				procTp.PatNum=PIn.Long(row["PatNum"].ToString());
				procTp.ProcNumOrig=PIn.Long(row["ProcNumOrig"].ToString());
				listProcTpsLim.Add(procTp);
			}
			return listProcTpsLim;
		}
	}

}




















