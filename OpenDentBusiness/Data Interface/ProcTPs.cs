using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),priority,treatPlanNum,listProcNums);
				return;
			}
			Db.NonQ($@"UPDATE proctp SET Priority = {POut.Long(priority)}
				WHERE TreatPlanNum = {POut.Long(treatPlanNum)}
				AND ProcNumOrig IN({string.Join(",",listProcNums.Select(x => POut.Long(x)))})");
		}

		#endregion

		///<summary>Gets all ProcTPs for a given Patient ordered by ItemOrder.</summary>
		public static List<ProcTP> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ProcTP>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM proctp "
				+"WHERE PatNum="+POut.Long(patNum)
				+" ORDER BY ItemOrder";
			return Crud.ProcTPCrud.SelectMany(command);
		}

		///<summary>Ordered by ItemOrder.</summary>
		public static List<ProcTP> RefreshForTP(long tpNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			Crud.ProcTPCrud.Update(proc);
		}

		///<summary></summary>
		public static long Insert(ProcTP proc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				proc.ProcTPNum=Meth.GetLong(MethodBase.GetCurrentMethod(),proc);
				return proc.ProcTPNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			proc.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.ProcTPCrud.Insert(proc);
		}

		///<summary></summary>
		public static void InsertOrUpdate(ProcTP proc, bool isNew){
			Meth.NoCheckMiddleTierRole();
			if(isNew){
				Insert(proc);
			}
			else{
				Update(proc);
			}
		}

		///<summary>There are no dependencies.</summary>
		public static void Delete(ProcTP proc){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			string command= "DELETE from proctp WHERE ProcTPNum = '"+POut.Long(proc.ProcTPNum)+"'";
 			Db.NonQ(command);
		}

		///<summary>No dependencies to worry about.</summary>
		public static void DeleteForTP(long treatPlanNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),treatPlanNum);
				return;
			}
			string command="DELETE FROM proctp "
				+"WHERE TreatPlanNum="+POut.Long(treatPlanNum);
			Db.NonQ(command);
		}

		public static List<ProcTP> GetForProcs(List<long> listProcNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static List<ProcTP> GetAllLim(List<long> listTreatPlanNums) {
			if(listTreatPlanNums.IsNullOrEmpty()) {//No need to go through middletier if we know listTreatPlanNums is empty. Return early.
				return new List<ProcTP>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ProcTP>>(MethodBase.GetCurrentMethod(),listTreatPlanNums);
			}
			string command = "SELECT TreatPlanNum,PatNum,ProcNumOrig FROM proctp "
				+"WHERE proctp.TreatPlanNum IN ("+ string.Join(",",listTreatPlanNums) +")";
			DataTable table=Db.GetTable(command);
			List<ProcTP> listProcTpsLim=new List<ProcTP>();
			foreach(DataRow row in table.Rows) {
				ProcTP procTp = new ProcTP();
				procTp.TreatPlanNum=PIn.Long(row["TreatPlanNum"].ToString());
				procTp.PatNum=PIn.Long(row["PatNum"].ToString());
				procTp.ProcNumOrig=PIn.Long(row["ProcNumOrig"].ToString());
				listProcTpsLim.Add(procTp);
			}
			return listProcTpsLim;
		}

		public static List<ProcTP> GetProcTPsFromTpRows(long patNum,List<TpRow> listTpRows,List<Procedure> listProcedures,List<TreatPlanAttach> listTreatPlanAttaches) {
			List<ProcTP> listProcTPs=new List<ProcTP>();
			for(int i = 0;i<listTpRows.Count;i++) {
				Procedure procedure=listProcedures[i].Copy();
				//procList.Add(proc);
				ProcTP procTP=new ProcTP();
				//procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=patNum;
				procTP.ProcNumOrig=procedure.ProcNum;
				procTP.ItemOrder=i;
				procTP.Priority=listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==procedure.ProcNum).Priority;//proc.Priority;
				procTP.ToothNumTP=Tooth.Display(procedure.ToothNum);
				if(ProcedureCodes.GetProcCode(procedure.CodeNum).TreatArea==TreatmentArea.Surf) {
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(procedure.Surf,procedure.ToothNum);
				}
				else {
					procTP.Surf=procedure.Surf;//for UR, L, etc.
				}
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(procedure.CodeNum);
				procTP.Descript=listTpRows[i].Description;
				procTP.FeeAmt=PIn.Double(listTpRows[i].Fee.ToString());
				procTP.PriInsAmt=PIn.Double(listTpRows[i].PriIns.ToString());
				procTP.SecInsAmt=PIn.Double(listTpRows[i].SecIns.ToString());
				procTP.Discount=PIn.Double(listTpRows[i].Discount.ToString());
				procTP.PatAmt=PIn.Double(listTpRows[i].Pat.ToString());
				procTP.Prognosis=listTpRows[i].Prognosis;
				procTP.Dx=listTpRows[i].Dx;
				procTP.ProcAbbr=listTpRows[i].ProcAbbr;
				procTP.FeeAllowed=PIn.Double(listTpRows[i].FeeAllowed.ToString());
				procTP.TaxAmt=PIn.Double(listTpRows[i].TaxEst.ToString());
				procTP.ProvNum=listTpRows[i].ProvNum;
				procTP.DateTP=PIn.Date(listTpRows[i].DateTP.ToString());
				procTP.ClinicNum=listTpRows[i].ClinicNum;
				procTP.CatPercUCR=(double)listTpRows[i].CatPercUCR;
				procTP.TagOD=procedure.ProcNumLab; //Used for selection logic. See ControlTreat.gridMain_CellClick(...).
				listProcTPs.Add(procTP);
				listTpRows[i].Tag=procTP;
			}
			return listProcTPs;
		}

		///<summary>Gets one ProcTP object from the database using the primary key. Returns null if not found.</summary>
		public static ProcTP GetOneyByProcTPNum(long procTPNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ProcTP>(MethodBase.GetCurrentMethod(),procTPNum);
			}
			return Crud.ProcTPCrud.SelectOne(procTPNum);
		}
	}

}




















