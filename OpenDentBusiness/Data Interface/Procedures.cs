using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.SheetFramework;

namespace OpenDentBusiness {
	public class Procedures {
		public const string AutoNotePromptRegex=@"\[Prompt:""[a-zA-Z_0-9 ]+""\]";
		#region Global Update Fees Variables
		///<summary>Queue to hold batches for FIFO processing.  A batch is a DataTable of TPd procs.  One thread fills the queue with db data while the
		///main thread processes the batches of data.  Make sure to use _lockObjQueueBatchData when manipulating this queue.</summary>
		private static Queue<DataTable> _queueDataTables;
		///<summary>Lock object to keep the queue thread safe.</summary>
		private static object _lockObjQueueThread=new object();
		///<summary>False until the filling thread has added the last batch of data to the queue.  Once true AND the queue is empty, the main thread is
		///finished as well.</summary>
		private static bool _isQueueDone;
		///<summary>10,000. Number of ProcNums the filling thread uses for each batch of data.  The processing takes longer than filling, so we can keep this
		///number relatively small to reduce total program memory consumption.</summary>
		private const int ROWS_BATCH_MAX_SIZE=10000;
		///<summary>1000. The number of procnums to update at one time with the same fee.  This gets around transaction size limits.</summary>
		private const int UPDATE_PROCNUM_IN_MAX_SIZE=1000;
		///<summary>If this thread is not null then GlobalUpdateFees is in the middle of running.</summary>
		private static ODThread _odThreadQueueData;
		///<summary></summary>
		private static List<long> _listProcNumsMaxForGroups;
		private static int _totCount;
		#endregion Global Update Fees Variables

		#region Get Methods

		///<summary>Gets all procedures for a single planned appointment.  Does not include deleted procedures.</summary>
		public static List<Procedure> GetForPlanned(long patNum,long plannedAptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum,plannedAptNum);
			}
			if(patNum==0 || plannedAptNum==0) {
				return new List<Procedure>();
			}
			string command="SELECT * FROM procedurelog WHERE PatNum="+POut.Long(patNum)
				+" AND PlannedAptNum="+POut.Long(plannedAptNum)
				+" AND ProcStatus !="+POut.Int((int)ProcStat.D);//don't include deleted
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets a list of all tp'd procedures for a specified clinic.  Uses a MySqlDataReader to get the by converting one
		///row to a procedure object at a time to reduce memory load.  Only used in the global update writeoff estimates fee .</summary>
		public static List<Procedure> GetAllTp(long clinicNum=-1) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM procedurelog WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP);
			if(clinicNum >= 0) {
				command+=" AND procedurelog.ClinicNum="+clinicNum;
			}
			return Db.GetList(command,Crud.ProcedureCrud.RowToObj);
		}

		public static List<Procedure> GetAllForPatsAndStatuses(List<long> listPatNums,params ProcStat[] arrayProcStats) {
			if(listPatNums.IsNullOrEmpty() || arrayProcStats.IsNullOrEmpty()) {
				return new List<Procedure>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listPatNums,arrayProcStats);
			}
			string command="SELECT * FROM procedurelog "
				+"WHERE procedurelog.PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND procedurelog.ProcStatus IN ("+string.Join(",",arrayProcStats.Select(x => POut.Enum<ProcStat>(x)))+") ";
			return Db.GetList(command,Crud.ProcedureCrud.RowToObj);
		}

		///<summary>Gets all procedures for a single patient, without notes.  Does not include deleted procedures.</summary>
		public static List<Procedure> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM procedurelog WHERE PatNum="+POut.Long(patNum)
				+" AND ProcStatus !="+POut.Int((int)ProcStat.D)//don't include deleted
				+" ORDER BY ProcDate";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets all procedures for a single patient, without notes.  Does not include deleted procedures.</summary>
		public static List<Procedure> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM procedurelog WHERE PatNum="+POut.Long(patNum)
				+" AND ProcStatus !="+POut.Int((int)ProcStat.D)//don't include deleted
				+" ORDER BY ProcDate";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets all procedures for a single patient, without notes.  Does not include deleted procedures.</summary>
		public static List<Procedure> RefreshForStatus(long patNum,ProcStat procStatus,bool isNotOnApt=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum,procStatus,isNotOnApt);
			}
			string command="SELECT * FROM procedurelog WHERE PatNum="+POut.Long(patNum)+" "
				+"AND ProcStatus ="+POut.Int((int)procStatus)+" "
				+(isNotOnApt?"AND AptNum=0":"");
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets all procedures with a code num in listProcCodeNums for a single patient, without notes.  Does not include deleted procedures.</summary>
		public static List<Procedure> RefreshForProcCodeNums(long patNum,List<long> listProcCodeNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum,listProcCodeNums);
			}
			if(listProcCodeNums==null || listProcCodeNums.Count==0) {
				return new List<Procedure>();
			}
			string command="SELECT * FROM procedurelog WHERE PatNum="+POut.Long(patNum)+" "+
				"AND CodeNum IN ("+String.Join(",",listProcCodeNums)+") "+
				"AND ProcStatus !="+POut.Int((int)ProcStat.D)+" "+//don't include deleted
				"ORDER BY ProcDate";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets all completed procedures without notes for a list of patients. Used when making auto splits. Also returns any procedures attached to payplans that the current patient is responsible for.</summary>
		public static List<Procedure> GetCompleteForPats(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums==null || listPatNums.Count < 1) {
				return new List<Procedure>();
			}
			return GetProceduresForStatusHelper(ProcStat.C,listPatNums);
		}

		///<summary>Gets all treatment planned procedures without notes for a list of patients. Used when making auto splits. Also returns any procedures attached to payplans that the current patient is responsible for.</summary>
		public static List<Procedure> GetTpForPats(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums==null || listPatNums.Count<1) {
				return new List<Procedure>();
			}
			return GetProceduresForStatusHelper(ProcStat.TP,listPatNums);
		}

		///<summary>Gets procedures of a specified status for the patients passed in. Also returns any procedures attached to payplans that the patients are responsible for.</summary>
		private static List<Procedure> GetProceduresForStatusHelper(ProcStat stat,List<long> listPatNums) {
			//No RemotingRole check; private method
			string strQueryFilter=$@"procedurelog.PatNum IN ({string.Join(",",listPatNums)})";
			string command=$"SELECT PayPlanNum FROM payplan WHERE Guarantor IN ({string.Join(",",listPatNums)})";
			List<long> listGuarPayPlanNums=Db.GetListLong(command);
			if(!listGuarPayPlanNums.IsNullOrEmpty()) {
				//Get all procedures from both patient and dynamic payment plans where the given patients are the guarantor.
				//Purposefully includes payment plans that are inside or outside of the family.
				command=$@"SELECT ProcNum FROM payplancharge 
					WHERE PayPlanNum IN ({string.Join(",",listGuarPayPlanNums)}) 
					AND ProcNum!=0
						UNION 
					SELECT FKey ProcNum FROM payplanlink 
					WHERE PayPlanNum IN ({string.Join(",",listGuarPayPlanNums)}) 
					AND LinkType={POut.Int((int)PayPlanLinkType.Procedure)}
					AND FKey!=0";
				List<long> listProcNums=Db.GetListLong(command);
				if(!listProcNums.IsNullOrEmpty()) {
					strQueryFilter+=$" OR procedurelog.ProcNum IN({string.Join(",",listProcNums)})";
				}
			}
			command=$@" SELECT procedurelog.*
				FROM procedurelog
				WHERE ProcStatus = {POut.Int((int)stat)}
				AND ({strQueryFilter})
				ORDER BY ProcDate ";
			List<Procedure> listProcedures=Db.GetList(command,Crud.ProcedureCrud.RowToObj);
			listProcedures.Sort(ProcedureLogic.CompareProcedures);
			return listProcedures;
		}

		///<summary>Gets a limited procedure list.</summary>
		public static List<Procedure> GetForProcTPs(List<ProcTP> listProcTP,params ProcStat[] procStats) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listProcTP,procStats);
			}
			if(listProcTP.Count == 0) {
				return new List<Procedure>();
			}
			string command = "SELECT ProcNum,CodeNum,AptNum,ProcDate,ClinicNum,ProcStatus,ProcFee,BaseUnits,UnitQty FROM procedurelog "
				+"WHERE procedurelog.ProcNum IN ("+string.Join(",",listProcTP.Select(x => x.ProcNumOrig).ToList())+") "
				+"AND procedurelog.ProcStatus IN (" +string.Join(",",procStats.Select(x => (int)x))+ ")";
			DataTable table = Db.GetTable(command);
			List<Procedure> listProcs = new List<Procedure>();
			foreach(DataRow row in table.Rows) {
				Procedure proc = new Procedure();
				proc.ProcNum=PIn.Long(row["ProcNum"].ToString());
				proc.CodeNum=PIn.Long(row["CodeNum"].ToString());
				proc.AptNum=PIn.Long(row["AptNum"].ToString());
				proc.ProcDate=PIn.Date(row["ProcDate"].ToString());
				proc.ClinicNum=PIn.Long(row["ClinicNum"].ToString());
				proc.ProcStatus=(ProcStat)PIn.Int(row["ProcStatus"].ToString());
				proc.ProcFee=PIn.Double(row["ProcFee"].ToString());
				proc.BaseUnits=PIn.Int(row["BaseUnits"].ToString());
				proc.UnitQty=PIn.Int(row["UnitQty"].ToString());
				listProcs.Add(proc);
			}
			return listProcs;
		}

		///<summary>Pass in a list of guarantors. 
		///Gets all procedures that have a remaining balance on them for any member of the guarantor's family.</summary>
		public static List<RpUnearnedIncome.UnearnedProc> GetRemainingProcsForFamilies(List<long> listGuarantorNums) {
			if(listGuarantorNums.Count == 0) {
				return new List<RpUnearnedIncome.UnearnedProc>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<RpUnearnedIncome.UnearnedProc>>(MethodBase.GetCurrentMethod(),listGuarantorNums);
			}
			List<long> listAllFamilyPatNums = Patients.GetAllFamilyPatNums(listGuarantorNums);
			/*given a list of families, get all procedures with a remaining pat port for those families.*/
			string command = @"
			SELECT patient.Guarantor,
			(procedurelog.ProcFee *(procedurelog.BaseUnits + procedurelog.UnitQty)) + COALESCE(adj.AdjAmt,0)
				- (COALESCE(cp.WriteOff,0) + COALESCE(cp.InsPay,0) + COALESCE(cp.InsEst,0) + COALESCE(patpay.Amt,0)) RemAmt,
			procedurelog.* 
			FROM procedurelog
			LEFT JOIN (
				SELECT claimproc.ProcNum, 
				SUM(CASE WHEN claimproc.Status IN ("
					+POut.Int((int)ClaimProcStatus.NotReceived)+","
					+POut.Int((int)ClaimProcStatus.Received)+","
					+POut.Int((int)ClaimProcStatus.Supplemental)+","
					+POut.Int((int)ClaimProcStatus.CapComplete)
				+@") THEN claimproc.WriteOff END) AS WriteOff,
				SUM(CASE WHEN claimproc.Status IN ("
					+POut.Int((int)ClaimProcStatus.Received)+","
					+POut.Int((int)ClaimProcStatus.Supplemental)
				+@") THEN claimproc.InsPayAmt END) AS InsPay,
				SUM(CASE WHEN claimproc.Status = "+POut.Int((int)ClaimProcStatus.NotReceived)+@" THEN claimproc.InsPayEst END) AS InsEst
				FROM claimproc
				WHERE claimproc.Status IN ("
					+POut.Int((int)ClaimProcStatus.NotReceived)+","
					+POut.Int((int)ClaimProcStatus.Received)+","
					+POut.Int((int)ClaimProcStatus.Supplemental)+","
					+POut.Int((int)ClaimProcStatus.CapComplete)
				+@")
				AND claimproc.PatNum IN ("+string.Join(",",listAllFamilyPatNums.Select(x => POut.Long(x)))+@")
				AND claimproc.ProcNum != 0
				GROUP BY claimproc.ProcNum
			)cp ON cp.ProcNum = procedurelog.ProcNum
			LEFT JOIN (
				SELECT adjustment.ProcNum, SUM(adjustment.AdjAmt) AdjAmt
				FROM adjustment
				WHERE adjustment.PatNum IN ("+string.Join(",",listAllFamilyPatNums.Select(x => POut.Long(x)))+@")
				AND adjustment.ProcNum != 0
				GROUP BY adjustment.ProcNum
			)adj ON adj.ProcNum = procedurelog.ProcNum
			LEFT JOIN (
				SELECT paysplit.ProcNum, SUM(paysplit.SplitAmt) Amt
				FROM paysplit
				WHERE paysplit.PatNum IN ("+string.Join(",",listAllFamilyPatNums.Select(x => POut.Long(x)))+@")
				AND paysplit.ProcNum != 0
				GROUP BY paysplit.ProcNum
			)patpay ON patpay.ProcNum = procedurelog.ProcNum
			INNER JOIN patient ON patient.PatNum = procedurelog.PatNum
			WHERE procedurelog.ProcStatus = "+POut.Int((int)ProcStat.C)+@"
			AND procedurelog.PatNum IN ("+string.Join(",",listAllFamilyPatNums.Select(x => POut.Long(x)))+@")
			AND (procedurelog.ProcFee *(procedurelog.BaseUnits + procedurelog.UnitQty)) + COALESCE(adj.AdjAmt,0)
				- (COALESCE(cp.WriteOff,0) + COALESCE(cp.InsPay,0) + COALESCE(cp.InsEst,0) + COALESCE(patpay.Amt,0)) > 0.005";
			DataTable table = Db.GetTable(command);
			List<RpUnearnedIncome.UnearnedProc> retVal = new List<RpUnearnedIncome.UnearnedProc>();
			List<Procedure> listProcs = Crud.ProcedureCrud.TableToList(table);
			for(int i = 0;i < listProcs.Count;i++) {
				retVal.Add(new RpUnearnedIncome.UnearnedProc(listProcs[i],PIn.Long(table.Rows[i]["Guarantor"].ToString())
					,PIn.Decimal(table.Rows[i]["RemAmt"].ToString())));
			}
			return retVal;
		}

		///<summary>Gets all completed and TP procedures for a family.</summary>
		public static List<Procedure> GetCompAndTpForPats(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * from procedurelog WHERE PatNum IN("+String.Join(", ",listPatNums)+") "
				+"AND ProcStatus IN("+(int)ProcStat.C+","+(int)ProcStat.TP+") "
				+"ORDER BY ProcDate";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets one procedure directly from the db. Option to include the note.
		///If the procNum is 0 or if the procNum does not exist in the database, this will return a new Procedure object with uninitialized fields.  
		///If a new Procedure object is sent through the middle tier with an uninitialized ProcStatus=0, this will fail validation since the ProcStatus 
		///enum starts with 1.  Make sure to handle a new Procedure object with uninitialized fields.</summary>
		public static Procedure GetOneProc(long procNum,bool includeNote) {
			//Doing this before remoting role check because Middle Tier can't serialize a Procedure with ProcStatus=0.
			if(procNum==0) {
				return new Procedure();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Procedure>(MethodBase.GetCurrentMethod(),procNum,includeNote);
			}
			Procedure proc=Crud.ProcedureCrud.SelectOne(procNum);
			if(proc==null) {
				return new Procedure();//This will throw if Middle Tier. Haven't come up with a good solution yet.
			}
			if(!includeNote) {
				return proc;
			}
			string command="SELECT * FROM procnote WHERE ProcNum="+POut.Long(procNum)+" ORDER BY EntryDateTime DESC";
			DbHelper.LimitOrderBy(command,1);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return proc;
			}
			proc.UserNum   =PIn.Long(table.Rows[0]["UserNum"].ToString());
			proc.Note      =PIn.String(table.Rows[0]["Note"].ToString());
			proc.SigIsTopaz=PIn.Bool(table.Rows[0]["SigIsTopaz"].ToString());
			proc.Signature =PIn.String(table.Rows[0]["Signature"].ToString());
			return proc;
		}

		///<summary>Gets many procedures directly from the db.  Option to include the note.  If the procNum is 0 or if the procNum does not exist in 
		///the database, this will return a new Procedure object with uninitialized fields.  If, for example, a new Procedure object is sent through 
		///the middle tier with an uninitialized ProcStatus=0, this will fail validation since the ProcStatus enum starts with 1.  Make sure to handle a 
		///new Procedure object with uninitialized fields.</summary>
		public static List<Procedure> GetManyProc(List<long> listProcNums,bool includeNote) {
			if(listProcNums==null || listProcNums.Count==0) {
				return new List<Procedure>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listProcNums,includeNote);
			}
			string command="";
			if(!includeNote) {
				command="SELECT * FROM procedurelog WHERE ProcNum IN ("+string.Join(",",listProcNums)+")";
				return Crud.ProcedureCrud.SelectMany(command);
			}
			command="SELECT procedurelog.*"
					+",procnoterow.UserNum NoteUserNum,procnoterow.Note NoteNote,procnoterow.SigIsTopaz NoteSigIsTopaz,procnoterow.Signature NoteSignature "
				+"FROM procedurelog "
				+"LEFT JOIN (SELECT ProcNum,MAX(EntryDateTime) EntryDateTime FROM procnote WHERE procnote.ProcNum IN ("+string.Join(",",listProcNums)+") GROUP BY ProcNum) procnotemax ON procnotemax.ProcNum=procedurelog.ProcNum "
				+"LEFT JOIN procnote procnoterow ON procnoterow.ProcNum=procedurelog.ProcNum AND procnoterow.EntryDateTime=procnotemax.EntryDateTime "
				+"WHERE procedurelog.ProcNum IN ("+string.Join(",",listProcNums)+")";
			//ProcNote stuff
			DataTable table=Db.GetTable(command);
			List<Procedure> listProcs=Crud.ProcedureCrud.TableToList(table);
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["NoteNote"].ToString()=="") {
					continue;
				}
				listProcs[i].UserNum=PIn.Long(table.Rows[i]["NoteUserNum"].ToString());
				listProcs[i].Note=PIn.String(table.Rows[i]["NoteNote"].ToString());
				listProcs[i].SigIsTopaz=PIn.Bool(table.Rows[i]["NoteSigIsTopaz"].ToString());
				listProcs[i].Signature=PIn.String(table.Rows[i]["NoteSignature"].ToString());
			}
			return listProcs;
		}

		///<summary>Gets Procedures for a single appointment directly from the database</summary>
		public static List<Procedure> GetProcsForSingle(long aptNum,bool isPlanned) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),aptNum,isPlanned);
			}
			string command;
			if(isPlanned) {
				command = "SELECT * from procedurelog WHERE PlannedAptNum = '"+POut.Long(aptNum)+"'";
			}
			else {
				command = "SELECT * from procedurelog WHERE AptNum = '"+POut.Long(aptNum)+"'";
			}
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets all Procedures that need to be displayed in FormApptEdit.</summary>
		public static List<Procedure> GetProcsForApptEdit(Appointment appt) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),appt);
			}
			string command="SELECT procedurelog.* FROM procedurelog "
				+"WHERE procedurelog.PatNum="+POut.Long(appt.PatNum)+" "
				+"AND (procedurelog.ProcStatus="+POut.Long((int)ProcStat.TP)+" ";
			if(appt.AptNum!=0) {//Filling grid for a new appt
				command+="OR ";
				if(appt.AptStatus==ApptStatus.Planned) {
					command+="procedurelog.PlannedAptNum="+POut.Long(appt.AptNum)+" ";
				}
				else {//Scheduled
					command+="procedurelog.AptNum="+POut.Long(appt.AptNum)+" ";
				}
			}
			if(appt.AptStatus==ApptStatus.Scheduled || appt.AptStatus==ApptStatus.Complete 
				|| appt.AptStatus==ApptStatus.Broken)
			{
					command+="OR (procedurelog.AptNum=0 AND procedurelog.ProcStatus="+POut.Long((int)ProcStat.C)+" AND "
						+DbHelper.DtimeToDate("procedurelog.ProcDate")+"="+POut.Date(appt.AptDateTime)+") ";
			}
			command+=") AND procedurelog.ProcStatus != "+POut.Long((int)ProcStat.D);
			List<Procedure> result=Crud.ProcedureCrud.SelectMany(command);
			for(int i=0;i<result.Count;i++){
				command="SELECT * FROM procnote WHERE ProcNum="+POut.Long(result[i].ProcNum)+" ORDER BY EntryDateTime DESC";
				command=DbHelper.LimitOrderBy(command,1);
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count==0) {
					continue;
				}
				result[i].UserNum   =PIn.Long(table.Rows[0]["UserNum"].ToString());
				result[i].Note      =PIn.String(table.Rows[0]["Note"].ToString());
				result[i].SigIsTopaz=PIn.Bool(table.Rows[0]["SigIsTopaz"].ToString());
				result[i].Signature =PIn.String(table.Rows[0]["Signature"].ToString());
			}
			ProcedureLogic.SortProcedures(ref result);
			return result;
		}

		///<summary>Gets all Procedures for a single date for the specified patient directly from the database.  Excludes deleted procs.</summary>
		public static List<Procedure> GetProcsForPatByDate(long patNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum,date);
			}
			string command="SELECT * FROM procedurelog "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND (ProcDate="+POut.Date(date)+" OR DateEntryC="+POut.Date(date)+") "
				+"AND ProcStatus!="+POut.Int((int)ProcStat.D);//exclude deleted procs
			List<Procedure> result=Crud.ProcedureCrud.SelectMany(command);
			for(int i=0;i<result.Count;i++){
				command="SELECT * FROM procnote WHERE ProcNum="+POut.Long(result[i].ProcNum)+" ORDER BY EntryDateTime DESC";
				command=DbHelper.LimitOrderBy(command,1);
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count==0) {
					continue;
				}
				result[i].UserNum   =PIn.Long(table.Rows[0]["UserNum"].ToString());
				result[i].Note      =PIn.String(table.Rows[0]["Note"].ToString());
				result[i].SigIsTopaz=PIn.Bool(table.Rows[0]["SigIsTopaz"].ToString());
				result[i].Signature =PIn.String(table.Rows[0]["Signature"].ToString());
			}
			return result;
		}

		///<summary>Gets all procedures associated with corresponding claimprocs. Returns empty procedure list if an empty list was passed in.</summary>
		public static List<Procedure> GetProcsFromClaimProcs(List<ClaimProc> listClaimProc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listClaimProc);
			}
			if(listClaimProc.Count==0) {
				return new List<Procedure>();
			}
			string command="SELECT * FROM procedurelog WHERE ProcNum IN (";
			for(int i=0;i<listClaimProc.Count;i++) {
				if(i>0) {
					command+=",";
				}
				command+=listClaimProc[i].ProcNum;
			}
			command+=")";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets a list of TP procedures that are attached to scheduled appointments that are not flagged as CPOE.</summary>
		public static List<Procedure> GetProcsNonCpoeAttachedToApptsForProv(long provNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),provNum);
			}
			if(provNum==0) {
				return new List<Procedure>();
			}
			string command="SELECT procedurelog.* "
				+"FROM procedurelog "
				+"INNER JOIN appointment ON procedurelog.AptNum=appointment.AptNum "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"WHERE procedurecode.IsRadiology=1 "
				+"AND appointment.AptStatus="+POut.Int((int)ApptStatus.Scheduled)+" "
				+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
				+"AND procedurelog.IsCpoe=0 "
				+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
				+"AND "+DbHelper.DtimeToDate("appointment.AptDateTime")+" >= "+DbHelper.Curdate()+" "
				+"ORDER BY appointment.AptDateTime";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets count of non-CPOE radiology procedures that are TP'd and attached to scheduled appointments for every provider who has ever had
		///an ehrprovkey.  Only used by the OpenDentalService AlertRadiologyProceduresThread.</summary>
		public static SerializableDictionary<long,long> GetCountNonCpoeProcsAttachedToAppts() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,long>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT procedurelog.ProvNum,COUNT(*) procCount "
				+"FROM procedurelog USE INDEX (RadiologyProcs) "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum AND procedurecode.IsRadiology=1 "
				+"INNER JOIN appointment ON appointment.AptNum=procedurelog.AptNum AND appointment.AptStatus="+POut.Int((int)ApptStatus.Scheduled)+" AND appointment.AptDateTime>=CURDATE() "
				+"WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
				+"AND procedurelog.IsCpoe=0 "
				+"AND procedurelog.ProvNum IN("
					+"SELECT ProvNum FROM provider "
					+"WHERE provider.LName!='' "//SQL standard says ''=='  ', an empty string is equal to a string composed entirely of any number of spaces
					+"AND provider.FName!='' "//so no need to trim LName or FName
					+"AND EXISTS("
						+"SELECT * FROM ehrprovkey "
						+"WHERE provider.LName=ehrprovkey.LName "
						+"AND provider.FName=ehrprovkey.FName "
					+")"
				+") "
				+"GROUP BY procedurelog.ProvNum";
			return Db.GetTable(command).Select().ToSerializableDictionary(x => PIn.Long(x["ProvNum"].ToString()),x => PIn.Long(x["procCount"].ToString()));
		}

		///<summary>Gets a list of TP or C procedures starting a year into the past that are flagged as IsRadiology and IsCpoe for the specified patient.
		///Primarily used for showing patient specific MU data in the EHR dashboard.</summary>
		public static List<Procedure> GetProcsRadiologyCpoeForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			//Since this is used for the dashboard and not directly used in any reporting calculations, we do not need to worry about the date that the
			// office updated past v15.4.1.
			DateTime dateStart=new DateTime(DateTime.Now.Year,1,1);//January first of this year.
			DateTime dateEnd=dateStart.AddYears(1).AddDays(-1);//Last day in December of this year.
			string command="SELECT procedurelog.* "
				+"FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum AND procedurecode.IsRadiology=1 "
				+"WHERE procedurelog.ProcStatus IN ("+POut.Int((int)ProcStat.C)+","+POut.Int((int)ProcStat.TP)+") "
				+"AND procedurelog.PatNum="+POut.Long(patNum)+" "
				+"AND procedurelog.IsCpoe=1 "
				+"AND procedurelog.DateEntryC BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
			return Crud.ProcedureCrud.SelectMany(command);
		}

		public static List<Procedure> GetProcsByStatusForPat(long patNum,params ProcStat[] procStatuses) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum,procStatuses);
			}
			if(procStatuses==null || procStatuses.Length==0) {
				return new List<Procedure>();
			}
			string command="SELECT * FROM procedurelog WHERE PatNum="+POut.Long(patNum)+" AND ProcStatus IN ("+string.Join(",",procStatuses.Select(x => (int)x))+")";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets a string in M/yy format for the most recent completed procedure in the specified code range.  Gets directly from the database.</summary>
		public static string GetRecentProcDateString(long patNum,DateTime aptDate,string procCodeRange) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),patNum,aptDate,procCodeRange);
			}
			if(aptDate.Year<1880) {
				aptDate=DateTime.Today;
			}
			string code1;
			string code2;
			if(procCodeRange.Contains("-")) {
				string[] codeSplit=procCodeRange.Split('-');
				code1=codeSplit[0].Trim();
				code2=codeSplit[1].Trim();
			}
			else {
				code1=procCodeRange.Trim();
				code2=procCodeRange.Trim();
			}
			string command="SELECT MAX(ProcDate) FROM procedurelog "
				+"LEFT JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				//+"AND CodeNum="+POut.Long(codeNum)+" "
				+"AND ProcDate < "+POut.Date(aptDate)+" "
				+"AND (ProcStatus ="+POut.Int((int)ProcStat.C)+" "
				+"OR ProcStatus ="+POut.Int((int)ProcStat.EC)+" "
				+"OR ProcStatus ="+POut.Int((int)ProcStat.EO)+") "
				+"AND procedurecode.ProcCode >= '"+POut.String(code1)+"' "
				+"AND procedurecode.ProcCode <= '"+POut.String(code2)+"' ";
			DateTime date=PIn.Date(Db.GetScalar(command));
			if(date.Year<1880) {
				return "";
			}
			return date.ToString("M/yy");
		}

		///<summary>Gets the first completed procedure within the family.  Used to determine the earliest date the family became a customer.</summary>
		public static Procedure GetFirstCompletedProcForFamily(long guarantor) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Procedure>(MethodBase.GetCurrentMethod(),guarantor);
			}
			string command="SELECT procedurelog.* FROM procedurelog "
				+"LEFT JOIN patient ON procedurelog.PatNum=patient.PatNum AND patient.Guarantor="+POut.Long(guarantor)+" "
				+"WHERE "+DbHelper.Year("procedurelog.ProcDate")+">1 "
				+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"ORDER BY procedurelog.ProcDate";
			command=DbHelper.LimitOrderBy(command,1);
			return Crud.ProcedureCrud.SelectOne(command);
		}

		///<summary>Gets a list of all the procedures attached to the specified appointments.  Then, use GetProcsOneApt to pull procedures for one appointment from this list or GetProductionOneApt.  This process requires only one call to the database.  "myAptNums" is the list of appointments to get procedures for.  isForNext gets procedures for a list of next appointments rather than regular appointments.</summary>
		public static List<Procedure> GetProcsMultApts(List<long> myAptNums,bool isForPlanned=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),myAptNums,isForPlanned);
			}
			if(myAptNums.Count==0) {
				return new List<Procedure>();
			}
			string strAptNums="";
			for(int i=0;i<myAptNums.Count;i++) {
				if(i>0) {
					strAptNums+=" OR";
				}
				if(isForPlanned) {
					strAptNums+=" PlannedAptNum='"+POut.Long(myAptNums[i])+"'";
				}
				else {
					strAptNums+=" AptNum='"+POut.Long(myAptNums[i])+"'";
				}
			}
			string command = "SELECT * FROM procedurelog WHERE"+strAptNums;
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets procedures for one appointment by looping through the procsMultApts which was filled previously from GetProcsMultApts.</summary>
		public static Procedure[] GetProcsOneApt(long myAptNum,List<Procedure> procsMultApts) {
			//No need to check MiddleTierRole; no call to db.
			ArrayList al=new ArrayList();
			for(int i=0;i<procsMultApts.Count;i++) {
				if(procsMultApts[i].AptNum==myAptNum) {
					al.Add(procsMultApts[i].Copy());
				}
			}
			Procedure[] retVal=new Procedure[al.Count];
			al.CopyTo(retVal);
			return retVal;
		}

		///<summary>Gets procedures for one appointment for use in the API direct from the DB. If none are found, this returns null.</summary>
		public static List<ProcedureForApi> GetProcsOneAptForApi(long aptNum,DateTime dateTStamp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ProcedureForApi>>(MethodBase.GetCurrentMethod(),aptNum,dateTStamp);
			}
			List<ProcedureForApi> listProcedureForApis=new List<ProcedureForApi>();
			string command="SELECT * FROM procedurelog "
				+"WHERE DateTStamp>="+POut.DateT(dateTStamp)+" ";
			if(aptNum!=0) {
				command+="AND AptNum='"+POut.Long(aptNum)+"'";
			}
			command+=" ORDER BY ProcNum DESC";
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before procedures for rigorous inclusion of procedures
			List<Procedure> listProcedures=Crud.ProcedureCrud.SelectMany(command);
			for(int i=0;i<listProcedures.Count;i++) {
				ProcedureForApi procedureForApi=new ProcedureForApi();
				procedureForApi.ProcedureCur=listProcedures[i];
				procedureForApi.DateTimeServer=dateTimeServer;
				listProcedureForApis.Add(procedureForApi);
			}
			return listProcedureForApis;
		}

		///<summary>Gets list of TP procedures for patient with codes included in Ortho Setup's Banding procedure list.</summary>
		public static List<Procedure> GetProcsForFormProcBandingSelect(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			if(PrefC.GetString(PrefName.OrthoBandingCodes)=="") {
				return new List<Procedure>();
			}
			List<string> listBandingProcedures=OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoBandingCodes);
			string command=$@"SELECT procedurelog.* FROM procedurelog
				JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum
				WHERE procedurelog.PatNum={POut.Long(patNum)}
				AND procedurelog.ProcStatus={POut.Int((int)ProcStat.TP)}
				AND procedurecode.ProcCode IN({string.Join(",",listBandingProcedures.Select(x => "'"+POut.String(x)+"'").ToList())})";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Used in FormClaimEdit,FormClaimPrint,FormClaimPayTotal,ContrAccount etc to get description of procedure.  Procedure list needs to
		///include the procedure we are looking for.  If procNum could be 0 (e.g. total payment claimprocs) or if the list does not contain the procNum,
		///this will return a new Procedure with uninitialized fields.
		///If, for example, a new Procedure object is sent through the middle tier with an uninitialized ProcStatus=0, this will fail validation since the
		///ProcStatus enum starts with 1.  Make sure to handle a new Procedure object with uninitialized fields.</summary>
		public static Procedure GetProcFromList(List<Procedure> listProcs,long procNum) {
			//No need to check MiddleTierRole; no call to db.
			return listProcs.FirstOrDefault(x => x.ProcNum==procNum)??new Procedure();
		}

		///<summary>Gets all completed procedures within a date range with optional ProcCodeNum and PatientNum filters. Date range is inclusive.  
		///If including GroupNotes, make sure to include the GroupNote code in the list of ProcCodeNums when explicitly specifying code nums.</summary>
		public static List<Procedure> GetCompletedForDateRange(DateTime dateStart,DateTime dateStop,List<long> listProcCodeNums=null
			,List<long> listPatNums=null,bool includeNote=false,bool includeGroupNote=false) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),dateStart,dateStop,listProcCodeNums,listPatNums,includeNote
					,includeGroupNote);
			}
			string command="";
			string whereClause="WHERE procedurelog.ProcStatus IN("+POut.Int((int)ProcStat.C);
			if(includeGroupNote) {
				whereClause+=","+POut.Int((int)ProcStat.EC);
			}
			whereClause+=") AND procedurelog.ProcDate>="+POut.Date(dateStart)+" AND procedurelog.ProcDate<="+POut.Date(dateStop);
			if (listProcCodeNums!=null && listProcCodeNums.Count > 0) {
				whereClause+=" AND procedurelog.CodeNum IN ("+string.Join(",", listProcCodeNums)+")";
			}
			if(listPatNums!=null && listPatNums.Count > 0) {
				whereClause+=" AND procedurelog.PatNum IN ("+string.Join(",",listPatNums)+")";
			}
			command="SELECT * FROM procedurelog "+whereClause;
			List<Procedure> listProcs=Crud.ProcedureCrud.SelectMany(command);
			if(!includeNote || listProcs.Count==0) {
				return listProcs;
			}
			//ProcNote stuff
			command="SELECT procnote.ProcNum,procnote.UserNum,procnote.Note,procnote.SigIsTopaz,procnote.Signature "
				+"FROM procnote "
				+"INNER JOIN ("
					+"SELECT procnote.ProcNum,MAX(procnote.EntryDateTime) EntryDateTime "
					+"FROM procnote "
					+"WHERE procnote.ProcNum IN("+string.Join(",",listProcs.Select(x => x.ProcNum))+") "
					+"GROUP BY procnote.ProcNum"
				+") procnotemax ON procnote.ProcNum=procnotemax.ProcNum AND procnote.EntryDateTime=procnotemax.EntryDateTime";
			Dictionary<long,DataRow> dictProcNoteRows=Db.GetTable(command).Select().ToDictionary(x => PIn.Long(x["ProcNum"].ToString()));
			if(dictProcNoteRows.Count==0) {//no notes for the procs, just return the list of procs
				return listProcs;
			}
			foreach(Procedure proc in listProcs) {
				DataRow row;
				if(!dictProcNoteRows.TryGetValue(proc.ProcNum,out row) || string.IsNullOrEmpty(row["Note"].ToString())) {
					continue;
				}
				proc.UserNum=PIn.Long(row["UserNum"].ToString());
				proc.Note=PIn.String(row["Note"].ToString());
				proc.SigIsTopaz=PIn.Bool(row["SigIsTopaz"].ToString());
				proc.Signature=PIn.String(row["Signature"].ToString());
			}
			return listProcs;
		}

		///<summary>Gets all completed procedures having procedurelog.DateComplete within the date range. Date range is inclusive.</summary>
		public static List<Procedure> GetCompletedByDateCompleteForDateRange(DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),dateStart,dateStop);
			}
			string command="SELECT * FROM procedurelog WHERE ProcStatus="+POut.Int((int)ProcStat.C)
				+" AND DateComplete>="+POut.Date(dateStart)
				+" AND DateComplete<="+POut.Date(dateStop);
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Determines what the ProcFee should be based on the given inputs.  If listFees is not null, it must include include fees for medical codes, which are needed here.</summary>
		public static double GetProcFee(Patient pat,List<PatPlan> listPatPlans,List<InsSub> listInsSubs,List<InsPlan> listInsPlans,
			long procCodeNum,long procProvNum,long procClinicNum,string procMedicalCode,List<Benefit> listBenefits=null,List<Fee> listFees=null)
		{
			//No need to check MiddleTierRole; no call to db.
			double procFeeRet;
			InsPlan insPlanPrimary=null;
			PatPlan patPlanPrimary=listPatPlans.Find(x => x.Ordinal==1);
			if(patPlanPrimary!=null) {
				//This will return a new InsSub if one is not found in the list or the DB.
				InsSub insSubPrimary=InsSubs.GetSub(patPlanPrimary.InsSubNum,listInsSubs);
				//This will be null if insSubPrimary.PlanNum is 0 due to a insSubPrimary being a new InsSub.
				insPlanPrimary=InsPlans.GetPlan(insSubPrimary.PlanNum,listInsPlans);
			}
			//Get fee schedule and fee amount for medical or dental.
			if(PrefC.GetBool(PrefName.MedicalFeeUsedForNewProcs) && !string.IsNullOrEmpty(procMedicalCode)) {
				long feeSch=FeeScheds.GetMedFeeSched(pat,listInsPlans,listPatPlans,listInsSubs,procProvNum);
				procFeeRet=Fees.GetAmount0(ProcedureCodes.GetProcCode(procMedicalCode).CodeNum,feeSch,procClinicNum,procProvNum,listFees);
			}
			else {
				long feeSch=FeeScheds.GetFeeSched(pat,listInsPlans,listPatPlans,listInsSubs,procProvNum);
				procFeeRet=Fees.GetAmount0(procCodeNum,feeSch,procClinicNum,procProvNum,listFees);
			}
			if(insPlanPrimary!=null && insPlanPrimary.PlanType=="p") {//PPO
				double ucrFee=Fees.GetAmount0(procCodeNum,Providers.GetProv(Patients.GetProvNum(pat)).FeeSched,procClinicNum,procProvNum,listFees);
				if(procFeeRet < ucrFee || PrefC.GetBool(PrefName.InsPpoAlwaysUseUcrFee)) {
					procFeeRet=ucrFee;
				}
			}
			PatPlan priPatPlan=listPatPlans.Find(x => x.Ordinal==1);
			if(priPatPlan==null) {
				return procFeeRet;
			}
			InsPlan priInsPlan=listInsPlans.Find(x => x.PlanNum==listInsSubs.Find(y => y.InsSubNum==priPatPlan.InsSubNum)?.PlanNum);
			if(priInsPlan!=null && InsPlans.UsesUcrFeeForExclusions(priInsPlan)) {
				//Getting benefits here may cause slowness in places that get the fee many times 
				//(i.e. the Update Fees button in the TP module, or the Quick Add Procs button in FormApptEdit).
				//In places that are slow because of this (loops, etc) we should pass in listBenefits to avoid the chatty database calls.
				if(listBenefits==null) {
					listBenefits=Benefits.RefreshForPlan(priInsPlan.PlanNum,priPatPlan.PatPlanNum);
				}
				if(Benefits.IsExcluded(ProcedureCodes.GetStringProcCode(procCodeNum),listBenefits,priInsPlan.PlanNum,priPatPlan.PatPlanNum)
					|| Benefits.GetPercent(ProcedureCodes.GetStringProcCode(procCodeNum),priInsPlan.PlanType,priInsPlan.PlanNum,priPatPlan.PatPlanNum,
							listBenefits)==0) 
				{
					//Get the fee from the provider's fee schedule (ucr fee)
					procFeeRet=Fees.GetAmount0(procCodeNum,Providers.GetProv(Patients.GetProvNum(pat)).FeeSched,procClinicNum,procProvNum,listFees);
				}
			}
			return procFeeRet;
		}

		public static Procedure GetProcForPatByToothSurfStat(long patNum,int toothNum,string surf,ProcStat procStat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Procedure>(MethodBase.GetCurrentMethod(),patNum,toothNum,surf,procStat);
			}
			string command="SELECT * FROM procedurelog "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND Surf='"+POut.String(surf)+"' "
				+"AND ToothNum='"+POut.Int(toothNum)+"' "
				+"AND ProcStatus="+POut.Int((int)procStat);
			return Crud.ProcedureCrud.SelectOne(command);
		}

		///<summary>Returns table of patients with completed procedure count and most recent completed procedure ProcDate for each provider.  Used for reassigning patients PriProv to their most used provider with highest procedure count.  Ordered by most procedures starting from highest.  The return list will be huge, with some patients having multiple rows. If a patient has no procedures, then this returns zero rows.</summary>
		public static DataTable GetTablePatProvUsed(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT PatNum,procedurelog.ProvNum,COUNT(ProcNum) procCount "
				+"FROM procedurelog "
				+"INNER JOIN provider ON procedurelog.ProvNum=provider.ProvNum AND provider.IsHidden=0 "
				+"WHERE PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"GROUP BY procedurelog.ProvNum,PatNum" +" "
				+"ORDER BY procCount DESC";
			return Db.GetTable(command);
		}

		///<summary>Gets the ProvNum from the appointment that will be used on the procedure passed in.</summary>
		public static long GetProvNumFromAppointment(Appointment apt,Procedure proc,ProcedureCode procCode) {
			//No need to check MiddleTierRole; no call to db.
			long provNum;
			if(procCode.ProvNumDefault!=0) {//Override provider for procedures with a default provider
				provNum=procCode.ProvNumDefault;
			}
			else if(apt.ProvHyg==0 || !procCode.IsHygiene) {//either no hygiene prov on the appt or the proc is not a hygiene proc
				provNum=apt.ProvNum;
			}
			else {//appointment has a hygiene prov and the proc IsHygiene
				provNum=apt.ProvHyg;
			}
			return provNum;
		}

		///<summary>Gets a list of procedures representing extracted teeth.  Status of C,EC,orEO. Includes procs with toothNum "1"-"32".  Will not include procs with procdate before 1880.  Used for Canadian e-claims instead of the usual ToothInitials.GetMissingOrHiddenTeeth, because Canada requires dates on the extracted teeth.  Supply all procedures for the patient.</summary>
		public static List<Procedure> GetCanadianExtractedTeeth(List<Procedure> procList) {
			//No need to check MiddleTierRole; no call to db.
			List<Procedure> extracted=new List<Procedure>();
			ProcedureCode procCode;
			foreach(Procedure proc in procList) {
				if(!proc.ProcStatus.In(ProcStat.C,ProcStat.EC,ProcStat.EO)) {
					continue;
				}
				if(!Tooth.IsValidDB(proc.ToothNum)) {
					continue;
				}
				if(Tooth.IsSuperNum(proc.ToothNum)) {
					continue;
				}
				if(Tooth.IsPrimary(proc.ToothNum)) {
					continue;
				}
				if(proc.ProcDate.Year<1880) {
					continue;
				}
				procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				if(procCode.TreatArea!=TreatmentArea.Tooth) {
					continue;
				}
				if(procCode.PaintType!=ToothPaintingType.Extraction) {
					continue;
				}
				//Canadian claims only allow 1 extraction procedure per tooth.
				//If there are duplicates found, the CDA only wants the most recent extraction procedure.
				Procedure procedureInList=extracted.FirstOrDefault(x => x.ToothNum==proc.ToothNum);
				if(procedureInList!=null) {
					if(procedureInList.DateTStamp>proc.DateTStamp) {
						continue;//The current proc is not the most recent extraction for the toothnum.
					}
					extracted.Remove(procedureInList);
				}
				extracted.Add(proc.Copy());
			}
			return extracted.OrderByDescending(x => x.DateTStamp).ToList();
		}

		///<summary>Takes the list of all procedures for the patient, and finds any that are attached as lab procs to that proc.</summary>
		public static List<Procedure> GetCanadianLabFees(long procNumLab,List<Procedure> procList){
			//No need to check MiddleTierRole; no call to db.
			List<Procedure> retVal=new List<Procedure>();
			if(procNumLab==0) {//Ignore regular procedures.
				return retVal;
			}
			for(int i=0;i<procList.Count;i++) {
				if(procList[i].ProcNumLab==procNumLab) {
					retVal.Add(procList[i]);
				}
			}
			return retVal;
		}

		///<summary>Pulls the lab fees for the given procnums directly from the database.</summary>
		public static List<Procedure> GetCanadianLabFees(List<long> listProcNums){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			if(listProcNums.Count==0) {
				return new List<Procedure>();
			}
			return Crud.ProcedureCrud.SelectMany("SELECT * FROM procedurelog WHERE ProcStatus<>"+POut.Int((int)ProcStat.D)+" AND ProcNumLab IN ("+string.Join(",",listProcNums)+")");
		}

		///<summary>Pulls the lab fees for the given procnum directly from the database.</summary>
		public static List<Procedure> GetCanadianLabFees(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),procNum);
			}
			if(procNum==0) {//By Total payment rows do not have labs.
				return new List<Procedure>();
			}
			string command="SELECT * FROM procedurelog WHERE ProcStatus<>"+POut.Int((int)ProcStat.D)+" AND ProcNumLab="+POut.Long(procNum);
			return Crud.ProcedureCrud.SelectMany(command);
		}
		
		///<summary>Uses similar logic to ComputeEstimates() to find old estimates which need to be recomputed.</summary>
		public static List<Procedure> GetProcsWithOldEstimates() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod());
			}
			//only claimprocs which are estimate or capestimate for all procedures which are not Canadian labs.
			string command=@"SELECT procedurelog.*
				FROM procedurelog
				INNER JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum
					AND claimproc.PlanNum!=0
					AND claimproc.Status IN (6,8)
					AND (claimproc.InsSubNum,claimproc.PlanNum) NOT IN (SELECT patplan.InsSubNum,inssub.PlanNum FROM patplan INNER JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum WHERE patplan.PatNum=claimproc.PatNum)
				WHERE procedurelog.ProcNumLab=0
				GROUP BY procedurelog.ProcNum";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets patients treatment planned procedures associated to future scheduled appointments including today.
		///Returns an empty list if listPatNum or listCodeNums is empty.</summary>
		public static List<Procedure> GetProcsAttachedToFutureAppt(List<long> listPatNums,List<long> listCodeNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),listPatNums,listCodeNums);
			}
			if(listPatNums.Count==0 || listCodeNums.Count==0) {
				return new List<Procedure>();
			}
			string command="SELECT procedurelog.* "
				+"FROM procedurelog "
				+"INNER JOIN appointment ON appointment.AptNum=procedurelog.AptNum "
				+"WHERE appointment.PatNum IN ("+String.Join(",",listPatNums)+") "
				+"AND procedurelog.CodeNum IN ("+String.Join(",",listCodeNums)+") "
				+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
				//All appts today or later
				+"AND "+DbHelper.DateTConditionColumn("appointment.AptDateTime",ConditionOperator.GreaterThanOrEqual,MiscData.GetNowDateTime())+" "
				+"AND appointment.AptStatus="+POut.Int((int)ApptStatus.Scheduled);
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Returns list of ProcNums such that each ProcNum is the max ProcNum in it's group of numPerGroup ProcNums.
		///Example: If there are 1000 procedures in the db with sequential ProcNums and each ProcStatus is in the list of ProcStatuses and the numPerGroup
		///is 500, the returned list would have 2 values in it, 500 and 1000. Each number is the max ProcNum such that if you selected the procedures with
		///ProcNum greater than the previous entry (or greater than 0 if it is the first entry) and less than or equal to the current entry you would get
		///at most numPerGroup procedures (the last group could, of course, have fewer in it).</summary>
		public static List<long> GetProcNumMaxForGroups(int numPerGroup,List<ProcStat> listProcStatuses,long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),numPerGroup,listProcStatuses,clinicNum);
			}
			_totCount=0;
			List<long> retval=new List<long>();
			if(numPerGroup<1) {
				return retval;
			}
			List<string> listWhereClauses=new List<string>();
			if(listProcStatuses!=null && listProcStatuses.Count>0) {
				listWhereClauses.Add("ProcStatus IN("+string.Join(",",listProcStatuses.Select(x => POut.Int((int)x)))+")");
			}
			if(PrefC.HasClinicsEnabled && clinicNum>-1) {
				listWhereClauses.Add("ClinicNum="+POut.Long(clinicNum));
			}
			string whereClause="";
			if(listWhereClauses.Count>0) {
				whereClause="WHERE "+string.Join(" AND ",listWhereClauses)+" ";
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				string command="SET @row=0,@maxProcNum=0;"
					+"SELECT procNum,@row totalCount FROM ("
						+"SELECT @row:=@row+1 rowNum,@maxProcNum:=ProcNum procNum FROM ("
							+"SELECT ProcNum FROM procedurelog "+whereClause+"ORDER BY ProcNum"
						+") a"
					+") b "
					+"WHERE procNum=@maxProcNum OR rowNum%"+numPerGroup+"=0";
				DataTable tableCur=Db.GetTable(command);
				if(tableCur.Rows.Count>0) {
					_totCount=PIn.Int(tableCur.Rows[0]["totalCount"].ToString());
					retval=tableCur.Select().Select(x => PIn.Long(x["procNum"].ToString())).ToList();
				}
			}
			else {//oracle
				//different strategy for oracle, but not used for MySQL because it's much slower
				long groupMaxPatNum;
				int groupNum=0;
				do {
					groupMaxPatNum=0;
					string command="SELECT MAX(ProcNum) procNumMax,COUNT(*) groupCount FROM ("
						+DbHelper.LimitOrderByOffset("SELECT ProcNum FROM procedurelog "+whereClause+"ORDER BY ProcNum",numPerGroup,groupNum)+") patNumGroup";
					DataTable tableCur=Db.GetTable(command);//increase timeout to 5 minutes for this query
					if(tableCur.Rows.Count==0) {
						break;
					}
					groupMaxPatNum=PIn.Long(tableCur.Rows[0]["procNumMax"].ToString());
					if(groupMaxPatNum>0) {
						retval.Add(groupMaxPatNum);
					}
					_totCount+=PIn.Int(tableCur.Rows[0]["groupCount"].ToString());
					groupNum+=numPerGroup;
				} while(groupMaxPatNum>0);
			}
			return retval;
		}

		///<summary>Used from TP to get a list of all TP procs, ordered by their treatment plan's priority, (conditionally) toothnum.
		///Uses the preference 'TreatPlanSortByTooth' to determine if procedures should be sorted by tooth order.</summary>
		public static List<Procedure> GetListTPandTPi(List<Procedure> procList,List<TreatPlanAttach> listTreatPlanAttaches=null) {
			//No need to check MiddleTierRole; no call to db.
			return SortListByTreatPlanPriority(procList.FindAll(x => x.ProcStatus==ProcStat.TP || x.ProcStatus==ProcStat.TPi),listTreatPlanAttaches);
		}

		///<summary></summary>
		public static long GetClinicNum(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT ClinicNum FROM procedurelog WHERE ProcNum="+POut.Long(procNum);
			return PIn.Long(Db.GetScalar(command));
		}

		///<summary>Returns all the unique diagnostic codes in the list.  If there is less than 12 unique codes then it will pad the list with empty
		///entries if isPadded is true.  Will always place the principal diagnosis as the first item in the list.</summary>
		public static List<string> GetUniqueDiagnosticCodes(List<Procedure> listProcs,bool isPadded) {
			return GetUniqueDiagnosticCodes(listProcs,isPadded,new List<byte>());
		}

		///<summary>Returns all the unique diagnostic codes in the list.  If there is less than 12 unique codes then it will pad the list with empty
		///entries if isPadded is true.  Will always place the principal diagnosis as the first item in the list.  The returned list and
		///listDiagnosticVersions will be the same length upon return.  When returning, listDiagnosticVersions will contain the diagnostic code versions
		///of each code in the returned list, used for allowing the user to mix diagnostic code versions on a single claim.  The listDiagnosticVersions
		///must be a valid list (not null).</summary>
		public static List<string> GetUniqueDiagnosticCodes(List<Procedure> listProcs,bool isPadded,List<byte> listDiagnosticVersions) {
			//No need to check MiddleTierRole; no call to db.
			List<string> listDiagnosticCodes=new List<string>();
			listDiagnosticVersions.Clear();
			for(int i=0;i<listProcs.Count;i++) {//Ensure that the principal diagnosis is first in the list.
				Procedure proc=listProcs[i];
				if(proc.IcdVersion==9) {
					continue;
				}
				if(proc.IsPrincDiag && proc.DiagnosticCode!="") {
					listDiagnosticCodes.Add(proc.DiagnosticCode);
					listDiagnosticVersions.Add(proc.IcdVersion);
					break;
				}
			}
			for(int i=0;i<listProcs.Count;i++) {
				Procedure proc=listProcs[i];
				if(proc.IcdVersion==9) {//don't return icd9 codes.
					continue;
				}
				if(proc.DiagnosticCode!="" && !ExistsDiagnosticCode(listDiagnosticCodes,listDiagnosticVersions,proc.DiagnosticCode,proc.IcdVersion)) {
					listDiagnosticCodes.Add(proc.DiagnosticCode);
					listDiagnosticVersions.Add(proc.IcdVersion);
				}
				if(proc.DiagnosticCode2!="" && !ExistsDiagnosticCode(listDiagnosticCodes,listDiagnosticVersions,proc.DiagnosticCode2,proc.IcdVersion)) {
					listDiagnosticCodes.Add(proc.DiagnosticCode2);
					listDiagnosticVersions.Add(proc.IcdVersion);
				}
				if(proc.DiagnosticCode3!="" && !ExistsDiagnosticCode(listDiagnosticCodes,listDiagnosticVersions,proc.DiagnosticCode3,proc.IcdVersion)) {
					listDiagnosticCodes.Add(proc.DiagnosticCode3);
					listDiagnosticVersions.Add(proc.IcdVersion);
				}
				if(proc.DiagnosticCode4!="" && !ExistsDiagnosticCode(listDiagnosticCodes,listDiagnosticVersions,proc.DiagnosticCode4,proc.IcdVersion)) {
					listDiagnosticCodes.Add(proc.DiagnosticCode4);
					listDiagnosticVersions.Add(proc.IcdVersion);
				}
			}
			while(isPadded && listDiagnosticCodes.Count<12) {//Pad to at least 12 items.  Simplifies claim printing logic.
				listDiagnosticCodes.Add("");
				listDiagnosticVersions.Add(0);
			}
			return listDiagnosticCodes;
		}

		///<summary>Gets a list of procedures for </summary>
		public static DataTable GetReferred(DateTime dateFrom, DateTime dateTo, bool complete) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,complete);
			}
			string command=
				"SELECT procedurelog.CodeNum,procedurelog.PatNum,LName,FName,MName,RefDate,DateProcComplete,refattach.Note,RefToStatus "
				+"FROM procedurelog "
				+"JOIN refattach ON procedurelog.ProcNum=refattach.ProcNum "
				+"JOIN referral ON refattach.ReferralNum=referral.ReferralNum "
				+"WHERE RefDate>="+POut.Date(dateFrom)+" "
				+"AND RefDate<="+POut.Date(dateTo)+" ";
			if(!complete) {
				command+="AND DateProcComplete="+POut.Date(DateTime.MinValue)+" ";
			}
			command+="ORDER BY RefDate";
			return Db.GetTable(command);
		}

		///<summary>Gets all completed procedures within a date range with optional ProcCodeNum and PatientNum filters. Date range is inclusive.</summary>
		public static List<Procedure> GetCompletedForDateRangeLimited(DateTime dateStart,DateTime dateStop,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),dateStart,dateStop,listClinicNums);
			}
			string command = "SELECT ProcNum,ProcFee,UnitQty,BaseUnits,ClinicNum,CodeNum,ProcDate "
				+"FROM procedurelog WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"AND ProcDate>="+POut.Date(dateStart)+" AND ProcDate<="+POut.Date(dateStop);
			if(listClinicNums != null && listClinicNums.Count > 0) {
				command+=" AND ClinicNum IN ("+string.Join(",",listClinicNums)+")";
			}
			DataTable table = Db.GetTable(command);
			List<Procedure> listProcsLim = new List<Procedure>();
			foreach(DataRow row in table.Rows) {
				Procedure proc = new Procedure();
				proc.ProcNum=PIn.Long(row["ProcNum"].ToString());
				proc.ProcFee=PIn.Double(row["ProcFee"].ToString());
				proc.UnitQty=PIn.Int(row["UnitQty"].ToString());
				proc.BaseUnits=PIn.Int(row["BaseUnits"].ToString());
				proc.ClinicNum=PIn.Long(row["ClinicNum"].ToString());
				proc.CodeNum=PIn.Long(row["CodeNum"].ToString());
				proc.ProcDate=PIn.Date(row["ProcDate"].ToString());
				listProcsLim.Add(proc);
			}
			return listProcsLim;
		}

		///<summary>Helper method that returns a list of helper ProcExtended objects that will aggregate and sum up things based on the lists passed in.</summary>
		public static List<ProcExtended> GetProcExtendedEntriesFromProcedures(List<Procedure> listProcs,List<Adjustment> listAdjs,
			List<PaySplit> listPaySplits,List<ClaimProc> listClaimProcs,List<PayPlanCharge> listPayPlanCharges,List<PaySplit> listSplitsCur=null, 
			params ProcAttachTypes[] excludedTypes) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ProcExtended> retVal = new List<ProcExtended>();
			foreach(Procedure proc in listProcs) {
				ProcExtended procE = new ProcExtended() {
					Proc=proc,
					Adjustments=listAdjs.Where(x => x.ProcNum == proc.ProcNum).ToList(),
					PaySplits=listPaySplits.Where(x => x.ProcNum == proc.ProcNum).ToList(),
					ClaimProcs=listClaimProcs.Where(x => x.ProcNum == proc.ProcNum).ToList(),
					PayPlanCredits=listPayPlanCharges.Where(x => x.ProcNum == proc.ProcNum).ToList(),
					ExcludedTypes=excludedTypes.ToList(),
					SplitsCur=listSplitsCur.Where(x => x.ProcNum == proc.ProcNum).ToList(),
				};
				retVal.Add(procE);
			}
			return retVal;
		}

		///<summary>Gets a list of ProcedureForApi from db. Returns an empty list if not found.</summary>
		public static List<ProcedureForApi> GetProceduresForApi(int limit,int offset,long patNum,DateTime dateTStamp,long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ProcedureForApi>>(MethodBase.GetCurrentMethod(),limit,offset,patNum,dateTStamp,aptNum);
			}
			List<ProcedureForApi> listProcedureForApis=new List<ProcedureForApi>();
			string command="SELECT * FROM procedurelog "
				+"WHERE DateTStamp>="+POut.DateT(dateTStamp)+" ";
			if(patNum!=0) {
				command+="AND PatNum='"+POut.Long(patNum)+"' ";
			}
			if(aptNum!=0) {
				command+="AND AptNum='"+POut.Long(aptNum)+"' ";
			}
			command+="ORDER BY ProcNum DESC "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before procedures for rigorous inclusion of procedures
			List<Procedure> listProcedures=Crud.ProcedureCrud.SelectMany(command);
			for(int i=0;i<listProcedures.Count;i++) {
				ProcedureForApi procedureForApi=new ProcedureForApi();
				procedureForApi.ProcedureCur=listProcedures[i];
				procedureForApi.DateTimeServer=dateTimeServer;
				listProcedureForApis.Add(procedureForApi);
			}
			return listProcedureForApis;
		}

		///<summary>Gets a list of treatment planned (TP) procedures for a planned appointment. Returns an empty list if not found.</summary>
		public static List<Procedure> GetProceduresPlannedForApi(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT procedurelog.* FROM procedurelog "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND ProcStatus="+POut.Long((int)ProcStat.TP)+" ";
			return Crud.ProcedureCrud.SelectMany(command);
		}

		///<summary>Gets a list of a patient's procedures (including notes). Filters by ProcStatus of (C), (EC), or (EO).</summary>
		public static List<Procedure> GetProceduresWithNotesForApi(long patNum,List<long> listProcNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),patNum,listProcNums);
			}
			string command="SELECT * FROM procedurelog "
				+$"WHERE PatNum="+POut.Long(patNum)+" "
				+$"AND ProcNum IN ({string.Join(",",listProcNums)}) "
				+$"AND (ProcStatus="+POut.Int((int)ProcStat.C)+//Group Notes can only be ProcStatus of (C), (EC), or (EO).
					" OR ProcStatus="+POut.Int((int)ProcStat.EC)+
					" OR ProcStatus="+POut.Int((int)ProcStat.EO)+")";
			List<Procedure> listProcedures=Crud.ProcedureCrud.SelectMany(command);
			for(int i=0;i<listProcedures.Count;i++) {
				command="SELECT * FROM procnote WHERE ProcNum="+POut.Long(listProcedures[i].ProcNum)+" ORDER BY EntryDateTime DESC";
				command=DbHelper.LimitOrderBy(command,1);//Get the most recent procnote.
				DataTable dataTable=Db.GetTable(command);
				if(dataTable.Rows.Count==0) {
					continue;
				}
				listProcedures[i].Note=PIn.String(dataTable.Rows[0]["Note"].ToString());
				listProcedures[i].Signature=PIn.String(dataTable.Rows[0]["Signature"].ToString());
			}
			return listProcedures;
		}
		#endregion

		#region Insert
		///<summary>Inserts a procedure into the db, including adjustments automatically created for sales tax or discount plans.  (HQ only) If a sales tax was already calculated (pre-payments only), set doCalcTax to false to avoid making an extra AvaTax call.  isSilent indicates if the procedure is being inserted by the repeat charge tool, currently only used to supress error messages in the Avatax API. =jordan 2020-05-29- This is a bad pattern.  Do not do anything like this in other S classes.  Insert and Update methods are sacred and should never have additional logic in them.  We will need to untangle and remove all this code when we have time.</summary>
		public static long Insert(Procedure procedure,bool doCalcTax=true,bool isRepeatCharge=false,bool skipDiscountPlanAdjustment=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				procedure.ProcNum=Meth.GetLong(MethodBase.GetCurrentMethod(),procedure,doCalcTax,isRepeatCharge,skipDiscountPlanAdjustment);
				return procedure.ProcNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			procedure.SecUserNumEntry=Security.CurUser.UserNum;
			procedure.DiscountPlanAmt=GetDiscountAmountForDiscountPlan(procedure,out _, out _);
			if(procedure.ProcStatus==ProcStat.C) {
				procedure.DateComplete=DateTime.Today;
			}
			else {//In case someone tried to programmatically set the DateComplete when they shouldn't have.
				procedure.DateComplete=DateTime.MinValue;
			}
			if(procedure.ProcStatus==ProcStat.TP && AvaTax.DoSendProcToAvalara(procedure)) {
				procedure.TaxAmt=(double)AvaTax.GetEstimate(procedure.CodeNum,procedure.PatNum,procedure.ProcFee);
			}
			Crud.ProcedureCrud.Insert(procedure);
			//Do the sales tax adjustments first because we might be changing the proc note if an error occurs.
			if(procedure.ProcStatus==ProcStat.C && !skipDiscountPlanAdjustment) {
				Adjustments.CreateAdjustmentForDiscountPlan(procedure);//Inserting completed procedure.
				try {
					Adjustments.CreateOrUpdateSalesTaxIfNeeded(procedure,doCalcTax:doCalcTax,isRepeatCharge:isRepeatCharge);
				}
				catch(Exception ex) {
					if(isRepeatCharge) {//should only throw an exception if isRepeatCharge is true.
						throw new ODException("Failed to create sales tax adjustment.",ex);
					}
				}
			}
			if(procedure.Note!="") {
				ProcNote note=new ProcNote();
				note.PatNum=procedure.PatNum;
				note.ProcNum=procedure.ProcNum;
				note.UserNum=procedure.UserNum;
				note.Note=procedure.Note;
				ProcNotes.Insert(note);
			}
			return procedure.ProcNum;
		}

		///<summary>Returns 0 if the procedure will exceed AnnualMax or FreqLimitations. If the patient attached to the procedure doesn't have a discountplan will return 0.
		///Will also return 0 if there is no associated Fee with the procedure. Otherwise returns the expected DiscountPlanAmt.
		///freqLimitationMessage and annualMaxMessage will be blank if no limitations have been met.</summary>
		public static double GetDiscountAmountForDiscountPlan(Procedure procedure,out string freqLimitationMessage,out string annualMaxMessage, 
			DiscountPlanSub discountPlanSub=null,DiscountPlan discountPlan=null,double runningTotal=0,List<Procedure> listAddHistProcs=null)
		{
			//No remoting role check; uses out parameters
			freqLimitationMessage="";
			annualMaxMessage="";
			if(discountPlanSub==null) {
				discountPlanSub=DiscountPlanSubs.GetSubForPat(procedure.PatNum);
				if(discountPlanSub==null) { //Patient isn't a discountPlanSub
					return 0;
				}
			}
			if(discountPlan==null) {
				discountPlan=DiscountPlans.GetPlan(discountPlanSub.DiscountPlanNum);
				if(discountPlan==null) { //Patient doesn't have a discountPlan
					return 0;
				}
			}
			if(!DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,procedure.ProcDate,out DateTime startDate,out DateTime stopDate)) {
				return 0;
			}
			Dictionary<long,long> dictDiscountFees=DiscountPlans.GetFeeSchedNumsByPatNums(ListTools.FromSingle(procedure.PatNum));
			if(dictDiscountFees.Count==0) {//Only do this if the patient has a discount plan.
				return 0;
			}
			Fee procFee=Fees.GetFee(procedure.CodeNum,dictDiscountFees.First().Value,procedure.ClinicNum,procedure.ProvNum);
			if(procFee==null) {//No fee for discount plan's feesched
				Provider procProv=Providers.GetProv(procedure.ProvNum);
				procFee=Fees.GetFee(procedure.CodeNum,procProv.FeeSched,procedure.ClinicNum,procProv.ProvNum);
				if(procFee==null) {//No fee for discount plan's feesched and proc's provider
					Patient pat=Patients.GetPat(procedure.PatNum);
					Provider patProv=Providers.GetProv(pat.PriProv);
					procFee=Fees.GetFee(procedure.CodeNum,patProv.FeeSched,procedure.ClinicNum,patProv.ProvNum);
					if(procFee==null) {//No fee for pat's pri prov feesched and pat's pri prov
						patProv=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
						procFee=Fees.GetFee(procedure.CodeNum,patProv.FeeSched,procedure.ClinicNum,patProv.ProvNum);
					}
				}
			}
			//double procFeeAmt=(procFee == null) ? procedure.ProcFeeTotal : procFee.Amount;
			if(procFee==null || procFee.Amount==-1) { //If there is no procfee from the fee sched, there will be no discount
				return 0;
			}
			//Check if the frequency limitation is exceeded
			string discountPlanFrequencyLimits="";
			try {
				if(discountPlanSub!=null) {
					discountPlanFrequencyLimits=DiscountPlans.CheckDiscountFrequency(ListTools.FromSingle(procedure),procedure.PatNum,
						procedure.ProcDate,discountPlanSub:discountPlanSub,listAddHistProcs:listAddHistProcs);
				}
			} 
			catch(Exception e) {//Just Swollow it, the following if statement actually will handle things
				discountPlanFrequencyLimits=e.Message;//Set this to something so at least we don't try applying a Fee.
			}
			if(!string.IsNullOrEmpty(discountPlanFrequencyLimits)) {
				freqLimitationMessage=discountPlanFrequencyLimits;
				return 0;
			}
			//Check if the annual max is exceeded
			double procFeeAmt=procFee.Amount;
			double estimatedDiscountAmt=procedure.ProcFeeTotal-(procFeeAmt*procedure.Quantity);
			if(estimatedDiscountAmt<=0) { //ProcFeeAmount would be issuing a refund
				return 0;
			}
			if(runningTotal==0) {
				runningTotal=Adjustments.GetTotForPatByType(discountPlanSub.PatNum,discountPlan.DefNum,startDate,stopDate,procNumExclude:procedure.ProcNum);
			}
			if(discountPlan.AnnualMax==-1) {
				return estimatedDiscountAmt;
			}
			if(runningTotal>discountPlan.AnnualMax) { //Discount would exceed annual limit
				annualMaxMessage=Lans.g("Procedures","Exceeds Annual Max.");
				return 0;
			} 
			else if(runningTotal+estimatedDiscountAmt>discountPlan.AnnualMax) {  //Discount is partially applied up to annualMax
				annualMaxMessage=Lans.g("Procedures","Partially exceeds Annual Max. Applying:")+$" {discountPlan.AnnualMax-runningTotal:C}";
				return discountPlan.AnnualMax-runningTotal;
			}
			return estimatedDiscountAmt;
		}
		#endregion

		#region Update
		///<summary>A centralized form of Update combined with logic that was in FormProcEdit.  This should be called when you wish to matain the same logic as FormProcEdit AFTER changes are validated.</summary>
		public static void FormProcEditUpdate(Procedure procNew,Procedure procOld,ProcedureCode procCode,bool isProcLinkedToOrthoCase,bool isNew=false
			,string procTeethStr="") 
		{
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA") || procOld.ProcNumLab==0) {//Canadian. en-CA or fr-CA
				Update(procNew,procOld,false,isProcLinkedToOrthoCase:isProcLinkedToOrthoCase);//Do not update Canadian labs here, because they are handled in SetCanadianLabFeesCompleteForProc below.
			}
			//No need to check MiddleTierRole; no call to db.
			Appointments.UpdateProcDescriptionForAppt(procNew,procOld);//Does nothing if procNew is not associated to an appt or planned appt.
			DiscountChangeSecLogEntry(procNew,procOld);//Does nothing if procNew.Discount is the same as procOld.Discount.
			UpdateTpProcPriority(procNew);//Only updates/goes to DB if procNew.ProcStatus is TP.
			if(!ProcWasSetComplete(procNew,procOld,procCode)) {//Does nothing if procOld.ProcStatus is C or if procNew.ProcStatus is not C.
				if(procNew.ProcStatus==ProcStat.C && isNew) {
					//if new procedure is complete
					Procedures.LogProcComplCreate(procNew.PatNum,procNew,procNew.ToothNum);
				}
				else if(procOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC) && !isNew) {
					#region SecurityLog for editing a previously completed proc
					string logText=procCode.ProcCode+" ("+procOld.ProcStatus+"), ";
					if(procOld.ProcStatus!=procNew.ProcStatus) {//Status changed.
						string statusText=logText+Lans.g("Procedures"," changed from ")+procOld.ProcStatus+Lans.g("Procedures"," to ")+procNew.ProcStatus;
						SecurityLogs.MakeLogEntry(Permissions.ProcCompleteStatusEdit,procNew.PatNum,statusText);
					}
					if(!string.IsNullOrEmpty(procTeethStr)) {
						logText+=Lans.g("Procedures","Teeth")+": "+procTeethStr+", ";
					}
					logText+=Lans.g("Procedures","Fee")+": "+procNew.ProcFee.ToString("F")+", "+procCode.Descript+". ";
					if(procNew.ProvNum!=procOld.ProvNum && procOld.ProcStatus==ProcStat.C) {
						logText+=Lans.g("Procedures","Provider was changed from")+" "+Providers.GetAbbr(procOld.ProvNum)+" "+Lans.g("Procedures","to")+" "+Providers.GetAbbr(procNew.ProvNum)+".";
					}
					if(procOld.ProcStatus.In(ProcStat.EO,ProcStat.EC)) {
						SecurityLogs.MakeLogEntry(Permissions.ProcExistingEdit,procNew.PatNum,logText);
					}
					else {  //completed procedures, adds changes to securitylog if certain fields edited
						logText+=SecurityLogs.AppendProcCompleteEditSecurityLog(procNew,procOld);
						SecurityLogs.MakeLogEntry(Permissions.ProcCompleteEdit,procNew.PatNum,logText);
					}
				}
				#endregion
			}
			SetToothInitialForCompExtraction(procNew);
			#region Canadian lab helpers
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada
				if(procNew.ProcStatus==ProcStat.C) {//Canadian lab fees complete
					Procedures.SetCanadianLabFeesCompleteForProc(procNew);
				}
				else {//Canadian lab fees not complete
					Procedures.SetCanadianLabFeesStatusForProc(procNew);
				}
			}
			#endregion
			return;
		}

		///<summary>Updates only the changed columns.  Called from 44 places.  Also creates adjustments for discounts and sales tax, and updates payment plan charges.  =jordan 2020-05-29- This is a bad pattern.  Do not do anything like this in other S classes.  Insert and Update methods are sacred and should never have additional logic in them.  We will need to untangle and remove all this code when we have time.</summary>
		public static bool Update(Procedure procedure,Procedure oldProcedure,bool isPaySplit=false,bool isSilent=false
			,bool isProcLinkedToOrthoCase=false) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				bool retVal=Meth.GetBool(MethodBase.GetCurrentMethod(),procedure,oldProcedure,isPaySplit,isSilent,isProcLinkedToOrthoCase);
				if(procedure.ProcStatus!=oldProcedure.ProcStatus) {//This conditional must match below where we call ProcMultiVisits.UpdateGroupForProc().
					ProcMultiVisits.RefreshCache();//This refresh must be run on the client to be useful.
				}
				return retVal;
			}
			//Setting any procedure to TP, get a tax estimate only if the procedure amount is changing and the procedure is taxable
			if(procedure.ProcStatus==ProcStat.TP) {
				//Status changed and is attached to appointment (do not care about planned appointments since that is what treatment planning is for).
				if(procedure.ProcStatus!=oldProcedure.ProcStatus && procedure.AptNum!=0) {
					foreach(Appointment appt in Appointments.GetAppointmentsForProcs(new List<Procedure>() { procedure })) { //only 0,1, or 2 of these
						//Detach the procedure from completed appointment.
						if(appt.AptStatus==ApptStatus.Complete && procedure.AptNum==appt.AptNum) {
							procedure.AptNum=0;
							break;//If there is another appointment in the loop, then it must be a planned appointment which we do not care about here.
						}
					}
				}
				if(AvaTax.DoSendProcToAvalara(procedure,isSilent)) {//checks isHQ
					if(procedure.ProcFee!=oldProcedure.ProcFee || procedure.Discount!=oldProcedure.Discount) {
						double procFee=procedure.ProcFee;
						if(procedure.ProcStatus==ProcStat.TP) {
							procFee-=procedure.Discount;
						}
						procedure.TaxAmt=(double)AvaTax.GetEstimate(procedure.CodeNum,procedure.PatNum,procFee);
					}
				}
			}
			//Setting a procedure to complete
			if(oldProcedure.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C && !isProcLinkedToOrthoCase) {
				procedure.DiscountPlanAmt=Adjustments.CreateAdjustmentForDiscountPlan(procedure);
				if(!CompareDouble.IsZero(procedure.Discount)) { //Discounted Procedure
					Adjustments.CreateAdjustmentForDiscount(procedure);
				}
			}
			//If this is a complete procedure but either the date or the fee is changing, we need to try to update the transaction in AvaTax
			if(procedure.ProcStatus==ProcStat.C && 
				(oldProcedure.ProcFee!=procedure.ProcFee || oldProcedure.ProcDate.Date!=procedure.ProcDate.Date || oldProcedure.ProcStatus!=ProcStat.C)) {
				Adjustments.CreateOrUpdateSalesTaxIfNeeded(procedure);
			}
			//Setting a completed procedure to TP.
			if(oldProcedure.ProcStatus==ProcStat.C && procedure.ProcStatus!=ProcStat.C) {
				Adjustments.DeleteForProcedure(procedure.ProcNum);
			}
			if(procedure.ProcStatus==ProcStat.C && procedure.DateComplete.Year<1880) {
				procedure.DateComplete=DateTime.Today;
			}
			else if(procedure.ProcStatus!=ProcStat.C && procedure.DateComplete.Date==DateTime.Today.Date) {
				procedure.DateComplete=DateTime.MinValue;//db only field used by one customer and this is how they requested it.  PatNum #19191
			}
			if(isPaySplit) {
				PaySplits.UpdateAttachedPaySplits(procedure);
			}
			bool hasChanged=Crud.ProcedureCrud.Update(procedure,oldProcedure);
			//Setting the procedure to complete OR treatment planned. Alternatively, if setting a proc TP / Completed to NOT be TP / Completed.
			if((oldProcedure.ProcStatus!=ProcStat.C && new List<ProcStat> { ProcStat.C, ProcStat.TP }.Contains(procedure.ProcStatus))
				|| (procedure.ProcStatus!=ProcStat.C && new List<ProcStat> { ProcStat.C, ProcStat.TP }.Contains(oldProcedure.ProcStatus)))
			{
				PayPlanCharges.UpdateAttachedPayPlanCharges(procedure);//does nothing if there are none.
			}
			if(hasChanged && oldProcedure.ProcStatus==ProcStat.TP && procedure.ProcStatus==ProcStat.C) {
				//check for any tp prepayments and make transfer if valid
				List<PaySplit> listUnearnedSplitsForProc=PaySplits.GetPaySplitsFromProc(procedure.ProcNum,onlyUnearned:true);
				Payments.CreateTransferForTpProcs(procedure,listUnearnedSplitsForProc);
			}
			if(procedure.ProcStatus!=oldProcedure.ProcStatus) {
				ProcMultiVisits.UpdateGroupForProc(procedure.ProcNum,procedure.ProcStatus);
			}
			if(procedure.Note!=oldProcedure.Note
				|| procedure.UserNum!=oldProcedure.UserNum
				|| procedure.SigIsTopaz!=oldProcedure.SigIsTopaz
				|| procedure.Signature!=oldProcedure.Signature) 
			{
				ProcNote note=new ProcNote();
				note.PatNum=procedure.PatNum;
				note.ProcNum=procedure.ProcNum;
				note.UserNum=procedure.UserNum;
				note.Note=procedure.Note;
				note.SigIsTopaz=procedure.SigIsTopaz;
				note.Signature=procedure.Signature;
				ProcNotes.Insert(note);
			}
			return hasChanged;
		}

		public static void UpdateAptNum(long procNum,long newAptNum) {
			UpdateAptNums(new List<long>() { procNum },newAptNum);
		}

		public static void UpdateAptNums(List<long> listProcNums,long newAptNum,bool isPlannedAptNum=false) {
			if(listProcNums==null || listProcNums.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listProcNums,newAptNum,isPlannedAptNum);
				return;
			}
			string command="UPDATE procedurelog "
				+"SET "+(isPlannedAptNum?"PlannedAptNum =":"AptNum =")+POut.Long(newAptNum)+" "
				+"WHERE ProcNum IN ("+string.Join(",",listProcNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		public static void UpdatePriority(long procNum,long newPriority) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum,newPriority);
				return;
			}
			string command="UPDATE procedurelog SET Priority = "+POut.Long(newPriority)
				+" WHERE ProcNum = "+POut.Long(procNum);
			Db.NonQ(command);
		}

		///<summary>Updates IsCpoe column in the procedurelog table with the passed in value for the corresponding procedure.
		///This method explicitly used instead of the generic Update method because this (and only this) field can get updated when a user cancels out
		///of the Procedure Edit window and no other changes should accidentally make their way to the database.</summary>
		public static void UpdateCpoeForProc(long procNum,bool isCpoe) {
			//No need to check MiddleTierRole; no call to db.
			UpdateCpoeForProcs(new List<long>() { procNum },isCpoe);
		}

		///<summary>Updates IsCpoe column in the procedurelog table with the passed in value for the corresponding procedures.
		///This method explicitly used instead of the generic Update method because this (and only this) field can get updated when a user cancels out
		///of the Procedure Edit window and no other changes should accidentally make their way to the database.</summary>
		public static void UpdateCpoeForProcs(List<long> listProcNums,bool isCpoe) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listProcNums,isCpoe);
				return;
			}
			if(listProcNums==null || listProcNums.Count < 1) {
				return;
			}
			string command="UPDATE procedurelog SET IsCpoe = "+POut.Bool(isCpoe)
				+" WHERE ProcNum IN ("+string.Join(",",listProcNums)+")";
			Db.NonQ(command);
		}

		///<summary>Sets the patient.DateFirstVisit if necessary. A visitDate is required to be passed in because it may not be today's date. This is triggered by:
		///1. When any procedure is inserted regardless of status. From Chart or appointment. If no C procs and date blank, changes date.
		///2. When updating a procedure to status C. If no C procs, update visit date. Ask user first?
		///  #2 was recently changed to only happen if date is blank or less than 7 days old.
		///3. When an appointment is deleted. If no C procs, clear visit date.
		///  #3 was recently changed to not occur at all unless appt is of type IsNewPatient
		///4. Changing an appt date of type IsNewPatient. If no C procs, change visit date.
		///Old: when setting a procedure complete in the Chart module or the ProcEdit window.  Also when saving an appointment that is marked IsNewPat.</summary>
		public static void SetDateFirstVisit(DateTime visitDate,int situation,Patient pat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),visitDate,situation,pat);
				return;
			}
			if(situation==1) {
				if(pat.DateFirstVisit.Year>1880) {
					return;//a date has already been set.
				}
			}
			if(situation==2) {
				if(pat.DateFirstVisit.Year>1880 && pat.DateFirstVisit<DateTime.Now.AddDays(-7)) {
					return;//a date has already been set.
				}
			}
			string command="SELECT COUNT(*) from procedurelog "
				+"INNER JOIN procedurecode on procedurecode.CodeNum = procedurelog.CodeNum "
					+"AND procedurecode.ProcCode NOT IN ('D9986','D9987') "
				+"WHERE PatNum = '"+POut.Long(pat.PatNum)+"' "
				+"AND ProcStatus = '2'";
			DataTable table=Db.GetTable(command);
			if(PIn.Long(table.Rows[0][0].ToString())>0) {
				return;//there are already completed procs (for all situations)
			}
			if(situation==2) {
				//ask user first?
			}
			if(situation==3) {
				command="UPDATE patient SET DateFirstVisit ="+POut.Date(new DateTime(0001,01,01))
					+" WHERE PatNum ='"
					+POut.Long(pat.PatNum)+"'";
			}
			else {
				command="UPDATE patient SET DateFirstVisit ="
					+POut.Date(visitDate)+" WHERE PatNum ='"
					+POut.Long(pat.PatNum)+"'";
			}
			//MessageBox.Show(cmd.CommandText);
			//dcon.NonQ(command);
			Db.NonQ(command);
		}

		public static void AttachToApt(long procNum,long aptNum,bool isPlanned) {
			//No need to check MiddleTierRole; no call to db.
			List<long> procNums=new List<long>();
			procNums.Add(procNum);
			AttachToApt(procNums,aptNum,isPlanned);
		}

		public static void AttachToApt(List<long> procNums,long aptNum,bool isPlanned) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNums,aptNum,isPlanned);
				return;
			}
			if(procNums.Count==0) {
				return;
			}
			string command="UPDATE procedurelog SET ";
			if(isPlanned) {
				command+="PlannedAptNum";
			}
			else {
				command+="AptNum";
			}
			command+="="+POut.Long(aptNum)+" WHERE ";
			for(int i=0;i<procNums.Count;i++) {
				if(i>0) {
					command+=" OR ";
				}
				command+="ProcNum="+POut.Long(procNums[i]);
			}
			Db.NonQ(command);
		}

		public static void DetachFromApt(List<long> procNums,bool isPlanned) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNums,isPlanned);
				return;
			}
			if(procNums.Count==0) {
				return;
			}
			string command="UPDATE procedurelog SET ";
			if(isPlanned) {
				command+="PlannedAptNum";
			}
			else {
				command+="AptNum";
			}
			command+="=0 WHERE ";
			for(int i=0;i<procNums.Count;i++) {
				if(i>0) {
					command+=" OR ";
				}
				command+="ProcNum="+POut.Long(procNums[i]);
			}
			Db.NonQ(command);
		}

		public static void DetachFromInvoice(long statementNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum);
				return;
			}
			string command="UPDATE procedurelog SET StatementNum=0 WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}

		public static void DetachAllFromInvoices(List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatementNums);
				return;
			}
			if(listStatementNums==null || listStatementNums.Count==0) {
				return;
			}
			string command="UPDATE procedurelog SET StatementNum=0 WHERE StatementNum IN ("+string.Join(",",listStatementNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<Summary>Supply the list of procedures attached to the appointment.  It will loop through each and assign the correct provider.
		///Also sets clinic.  Also sets procDate for TP procs.  js 7/24/12 This is not supposed to be called if the appointment is complete.
		///When isUpdatingFees is true, we also update the ProcFee based on PrefName.ProcFeeUpdatePrompt</Summary>
		public static void SetProvidersInAppointment(Appointment apt,List<Procedure> listProcOrig,bool isUpdatingFees,ProcFeeHelper procFeeHelper) {
			//No need to check MiddleTierRole; no call to db.
			List<Procedure> listProcNew=new List<Procedure>();
			Procedure changedProc;
			OrthoProcLink orthoProcLink;
			Dictionary<long,OrthoProcLink> dictOrthoProcLinks=
				OrthoProcLinks.GetManyForProcs(listProcOrig.Select(x => x.ProcNum).ToList()).ToDictionary(y => y.ProcNum,y => y);
			for(int i=0;i<listProcOrig.Count;i++) {
				changedProc=listProcOrig[i].Copy();
				listProcNew.Add(changedProc);
				if(!IsProcComplEditAuthorized(changedProc)) {
					continue;
				}
				changedProc=ChangeProcInAppointment(apt,changedProc);
				if(listProcOrig[i].ProcDate.Date!=changedProc.ProcDate.Date && changedProc.ProcStatus==ProcStat.TP) {
					dictOrthoProcLinks.TryGetValue(listProcOrig[i].ProcNum,out orthoProcLink);
					if(orthoProcLink!=null && orthoProcLink.ProcLinkType==OrthoProcType.Banding) {
						OrthoCases.UpdateDatesByLinkedProc(orthoProcLink,changedProc);
					}
				}
			}
			if(isUpdatingFees) {
				foreach(Procedure procCur in listProcNew) {
					if(!IsProcComplEditAuthorized(procCur)) {
						continue;
					}
					procFeeHelper.FillData();
					procCur.ProcFee=GetProcFee(procFeeHelper.Pat,procFeeHelper.ListPatPlans,procFeeHelper.ListInsSubs,procFeeHelper.ListInsPlans,
						procCur.CodeNum,procCur.ProvNum,procCur.ClinicNum,procCur.MedicalCode,procFeeHelper.ListBenefitsPrimary,procFeeHelper.ListFees);
				}
			}
			Sync(listProcNew,listProcOrig);
		}

		public static void SetCanadianLabFeesCompleteForProc(Procedure proc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			//If this gets run on a lab fee itself, nothing will happen because result will be zero procs.
			string command="SELECT * FROM procedurelog WHERE ProcNumLab="+proc.ProcNum+" AND ProcStatus!="+POut.Int((int)ProcStat.D);
			List<Procedure> labFeesForProc=Crud.ProcedureCrud.SelectMany(command);
			if(proc.ProcNumLab==0) {//Regular procedure, not a lab.
				for(int i=0;i<labFeesForProc.Count;i++) {
					Procedure labFeeNew=labFeesForProc[i];
					Procedure labFeeOld=labFeeNew.Copy();
					labFeeNew.AptNum=proc.AptNum;
					labFeeNew.CanadianTypeCodes=proc.CanadianTypeCodes;
					labFeeNew.ClinicNum=proc.ClinicNum;
					labFeeNew.DateEntryC=proc.DateEntryC;
					labFeeNew.PlaceService=proc.PlaceService;
					labFeeNew.ProcDate=proc.ProcDate;
					labFeeNew.ProcStatus=ProcStat.C;
					if(labFeeNew.ProvNum==0) {//Shouldn't happen
						labFeeNew.ProvNum=proc.ProvNum;
					}
					labFeeNew.SiteNum=proc.SiteNum;
					labFeeNew.UserNum=proc.UserNum;
					Update(labFeeNew,labFeeOld);
				}
			}
			else {//Lab fee.  Set complete, set the parent procedure as well as any other lab fees complete.
				command="SELECT * FROM procedurelog WHERE ProcNum="+proc.ProcNumLab+" AND ProcStatus!="+POut.Int((int)ProcStat.D);
				Procedure procParent=Crud.ProcedureCrud.SelectOne(command);
				SetCanadianLabFeesCompleteForProc(procParent);
				Procedure parentProcNew=procParent;
				Procedure parentProcOld=procParent.Copy();
				parentProcNew.ProcStatus=ProcStat.C;
				Update(parentProcNew,parentProcOld);
			}
		}

		public static void SetCanadianLabFeesStatusForProc(Procedure proc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),proc);
				return;
			}
			//If this gets run on a lab fee itself, nothing will happen because result will be zero procs.
			string command="SELECT * FROM procedurelog WHERE ProcNumLab="+proc.ProcNum+" AND ProcStatus!="+POut.Int((int)ProcStat.D);
			List<Procedure> labFeesForProc=Crud.ProcedureCrud.SelectMany(command);
			if(proc.ProcNumLab==0) {//Regular procedure, not a lab.
				for(int i=0;i<labFeesForProc.Count;i++) {
					Procedure labFeeNew=labFeesForProc[i];
					Procedure labFeeOld=labFeeNew.Copy();
					labFeeNew.ProcStatus=proc.ProcStatus;
					Update(labFeeNew,labFeeOld);
				}
			}
			else {//Lab fee.  If lab is set back to any status other than complete, set the parent procedure as well as any other lab fees back to that status.
				command="SELECT * FROM procedurelog WHERE ProcNum="+proc.ProcNumLab+" AND ProcStatus!="+POut.Int((int)ProcStat.D);
				Procedure procParent=Crud.ProcedureCrud.SelectOne(command);
				Procedure parentProcNew=procParent;
				Procedure parentProcOld=procParent.Copy();
				parentProcNew.ProcStatus=proc.ProcStatus;
				SetCanadianLabFeesStatusForProc(parentProcNew);
				Update(parentProcNew,parentProcOld);
			}
		}

		///<summary></summary>
		public static void Lock(DateTime date1, DateTime date2) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),date1,date2);
				return;
			}
			string command="UPDATE procedurelog SET IsLocked=1 "
				+"WHERE (ProcStatus="+POut.Int((int)ProcStat.C)+" "//completed
				+"OR CodeNum="+POut.Long(ProcedureCodes.GetCodeNum(ProcedureCodes.GroupProcCode))+") "//or group note
				+"AND ProcDate >= "+POut.Date(date1)+" "
				+"AND ProcDate <= "+POut.Date(date2);
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Must always pass in two lists.</summary>
		public static void Sync(List<Procedure> listNew,List<Procedure> listOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,listOld);
				return;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			Crud.ProcedureCrud.Sync(listNew,listOld,Security.CurUser.UserNum);
		}

		public static void SetTPActive(long patNum,List<long> listProcNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,listProcNums); //Never pass DB list through the web service (Note: Why?  Our proc list is special, it doesn't contain all procs so we shouldn't code this method to always use our limited list of procs........)
				return;
			}
			string command="UPDATE procedurelog SET ProcStatus="+POut.Int((int)ProcStat.TPi)+" WHERE PatNum="+POut.Long(patNum)+" "+
				"AND ProcStatus="+POut.Int((int)ProcStat.TP)+" ";
			if(listProcNums.Count==0) {
				Db.NonQ(command);
				return; //no procedures left on active plan
			}
			command+="AND ProcNum NOT IN ("+string.Join(",",listProcNums)+") ";
			Db.NonQ(command);
			command="UPDATE procedurelog SET ProcStatus="+POut.Int((int)ProcStat.TP)+" WHERE PatNum="+POut.Long(patNum)+" "+
				"AND ProcStatus="+POut.Int((int)ProcStat.TPi)+" AND ProcNum IN ("+string.Join(",",listProcNums)+") ";
			Db.NonQ(command);
		}

		///<summary>Returns true when automation needed.
		///Sets the provider and clinic for a proc based on the appt to which it is attached.
		///Also sets ProcDate for TP procs. Will automatically set procs in listProcs to complete and make securitylogs.</summary>
		public static bool UpdateProcsInApptHelper(List<Procedure> listProcsForAppt,Patient pat,Appointment AptCur,Appointment AptOld,
			List<InsPlan> PlanList,List<InsSub> SubList,List<int> listProcSelectedIndices,bool removeCompletedProcs,bool doUpdateProcFees=false,
			LogSources logSource=LogSources.None)
		{ 
			if(AptCur.AptStatus!=ApptStatus.Complete) {//appt is not set complete, just update necessary fields like ProvNum, ProcDate, and ClinicNum
				ProcFeeHelper procFeeHelper=null;
				OrthoProcLink orthoProcLink;
				Dictionary<long,OrthoProcLink> dictOrthoProcLinks=
				OrthoProcLinks.GetManyForProcs(listProcsForAppt.Select(x => x.ProcNum).ToList()).ToDictionary(y => y.ProcNum,y => y);
				foreach(int index in listProcSelectedIndices) {
					//We only want to change the fields that just changed.  We don't want to undo any changes that are being made outside this window.  Note
					//that if we make any other changes to this proc that are not in this section we should consolidate update statements.
					Procedure procCur=listProcsForAppt[index];
					Procedure procOld=procCur.Copy();
					Permissions perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit,procOld);
					DateTime dateForPerm=GetDateForPermCheck(procOld);
					if(procOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC) && !Security.IsAuthorized(perm,dateForPerm,true)) {
						continue;
					}
					ChangeProcInAppointment(AptCur,procCur);//Doesn't update DB
					dictOrthoProcLinks.TryGetValue(procCur.ProcNum,out orthoProcLink);
					if(doUpdateProcFees && orthoProcLink==null) {//Can't update fees for procedures linked to orthocases.
						procFeeHelper=procFeeHelper??new ProcFeeHelper(pat,null,null,SubList,PlanList,null);
						procFeeHelper.FillData();
						procCur.ProcFee=GetProcFee(procFeeHelper.Pat,procFeeHelper.ListPatPlans,procFeeHelper.ListInsSubs,procFeeHelper.ListInsPlans,
							procCur.CodeNum,procCur.ProvNum,procCur.ClinicNum,procCur.MedicalCode,procFeeHelper.ListBenefitsPrimary,procFeeHelper.ListFees);
					}
					Update(procCur,procOld);//Update fields if needed.
					//If proc is linked as a TP'd banding to an OrthoCase, update the OrthoCase.
					if(procCur.ProcStatus==ProcStat.TP && orthoProcLink!=null && orthoProcLink.ProcLinkType==OrthoProcType.Banding) {
							OrthoCases.UpdateDatesByLinkedProc(orthoProcLink,procCur);
					}
				}
			}
			else if(listProcSelectedIndices.Any(x => listProcsForAppt[x].ProcStatus!=ProcStat.C)) {
				//if appointment is marked complete and any procedures are not, then set the remaining procedures complete.
				List<PatPlan> listPatPlans=PatPlans.Refresh(AptCur.PatNum);
				SetCompleteInAppt(AptCur,PlanList,listPatPlans,pat,SubList,removeCompletedProcs);
				if(AptOld.AptStatus==ApptStatus.Complete) {//seperate log entry for completed appointments
					SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,pat.PatNum,AptCur.AptDateTime.ToShortDateString()
						+", "+AptCur.ProcDescript+", Procedures automatically set complete due to appt being set complete",AptCur.AptNum,logSource,
						AptOld.DateTStamp);
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,AptCur.AptDateTime.ToShortDateString()
						+", "+AptCur.ProcDescript+", Procedures automatically set complete due to appt being set complete",AptCur.AptNum,logSource,
						AptOld.DateTStamp);
				}
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static void UpdateDiscountPlanAmt(long procNum,double newDiscountPlanAmt) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum,newDiscountPlanAmt);
				return;
			}
			string command="UPDATE procedurelog SET DiscountPlanAmt = "+POut.Double(newDiscountPlanAmt)
				+" WHERE ProcNum = "+POut.Long(procNum);
			Db.NonQ(command);
		}

		public static void UpdateDiscountPlanAmts(List<Procedure> listProcs) {
			//No need to check MiddleTierRole; no call to db.
			UpdateDiscountPlanAmts(listProcs.Select(x => new DiscountPlanProc(x)).ToList());
		}

		public static void UpdateDiscountPlanAmts(List<DiscountPlanProc> listProcs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listProcs);
				return;
			}
			listProcs.ForEach(x=>UpdateDiscountPlanAmt(x.ProcNum,x.DiscountPlanAmt));
		}
		#endregion

		#region Delete
		///<summary>If not allowed to delete, then it throws an exception, so surround it with a try catch. 
		///Also deletes any claimProcs, adjustments, and payplancharge credits.  
		///This does not actually delete the procedure, but just changes the status to deleted.</summary>
		///<param name="forceDelete">If true, forcefully deletes all objects attached to the procedure.</param>
		///<param name="hideGraphics">If true, sets the procedure's "HideGraphics" db field to true so that it will not show up on the chart.</param>
		public static void Delete(long procNum,bool forceDelete=false,bool hideGraphics=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum,forceDelete,hideGraphics);
				ProcMultiVisits.RefreshCache();//This refresh must be run on the client to be useful.
				return;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				DeleteCanadianLabFeesForProcCode(procNum);//Deletes lab fees attached to current procedures.
			}
			string command;
			if(forceDelete) {
				//Delete referral attaches
				command="DELETE FROM refattach WHERE ProcNum="+POut.Long(procNum);
				Db.NonQ(command);
				//Remove the procedure from the pay split
				command="UPDATE paysplit SET ProcNum=0 WHERE ProcNum="+POut.Long(procNum);
				Db.NonQ(command);
				//Claimprocs deleted below
			}
			else {
				ValidateDelete(procNum);
			}
			//delete adjustments, audit logs added from Adjustments.DeleteForProcedure()
			Adjustments.DeleteForProcedure(procNum);
			//delete claimprocs
			command="DELETE from claimproc WHERE ProcNum = '"+POut.Long(procNum)+"'";
			Db.NonQ(command);
			//detach procedure labs
			command="UPDATE procedurelog SET ProcNumLab=0 WHERE ProcNumLab='"+POut.Long(procNum)+"'";
			Db.NonQ(command);
			PayPlanCharges.DeleteForProc(procNum);
			//delete and update procmultivisits
			ProcMultiVisits.UpdateGroupForProc(procNum,ProcStat.D);
			command="SELECT AptNum,PlannedAptNum,DateComplete FROM procedurelog WHERE ProcNum = "+POut.Long(procNum);
			DataTable table = Db.GetTable(command);
			DateTime dateComplete = PIn.Date(table.Rows[0]["DateComplete"].ToString());
			long aptNum = PIn.Long(table.Rows[0]["AptNum"].ToString());
			long plannedAptNum = PIn.Long(table.Rows[0]["PlannedAptNum"].ToString());
			//set the procedure deleted-----------------------------------------------------------------------------------------
			command="UPDATE procedurelog SET ProcStatus = "+POut.Int((int)ProcStat.D)+", "
				+"AptNum=0, "
				+"PlannedAptNum=0";
			if(dateComplete.Date==DateTime.Today.Date) {
				command+=", DateComplete="+POut.Date(DateTime.MinValue);
			}
			if(hideGraphics) {
				command+=", HideGraphics=1";
			}
			command+=" WHERE ProcNum="+POut.Long(procNum);
			Db.NonQ(command);
			//resynch appointment description-------------------------------------------------------------------------------------
			if(aptNum != 0) {
				Appointment apt = Appointments.GetOneApt(aptNum);
				Appointment aptOld = apt.Copy();
				Appointments.SetProcDescript(apt);
				Appointments.Update(apt,aptOld);
			}
			if(plannedAptNum != 0) {
				Appointment plannedApt = Appointments.GetOneApt(plannedAptNum);
				Appointment plannedAptOld = plannedApt.Copy();
				Appointments.SetProcDescript(plannedApt);
				Appointments.Update(plannedApt,plannedAptOld);
			}
		}

		///<summary>Deletes the lab procedure if it's not attached to anything important. Updates the fee to $0 if unable to delete.</summary>
		public static void TryDeleteLab(Procedure procLab) {
			//No need to check MiddleTierRole; no call to db.
			try {
				Procedures.Delete(procLab.ProcNum);
			}
			catch(Exception ex) {
				//The lab procedure could be attached to a claim or adjustment. If so, it seems harmless to keep the procedure around with a $0 fee.
				ex.DoNothing();
				if(CompareDouble.IsEqual(procLab.ProcFee,0)) {
					return;
				}
				Procedure procOld=procLab.Copy();
				procLab.ProcFee=0;
				Procedures.Update(procLab,procOld);
			}
		}

		public static void DeleteCanadianLabFeesForProcCode(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum);
				return;
			}
			string command="SELECT * FROM procedurelog WHERE ProcNumLab="+procNum+" AND ProcStatus!="+POut.Int((int)ProcStat.D);
			List<Procedure> labFeeProcs=Crud.ProcedureCrud.SelectMany(command);
			for(int i=0;i<labFeeProcs.Count;i++) {
				Delete(labFeeProcs[i].ProcNum);
			}
		}
		#endregion

		#region Misc Methods
		///<summary></summary>
		public static void DiscountChangeSecLogEntry(Procedure procNew,Procedure procOld) {
			//No need to check MiddleTierRole; no call to db.
			if(procNew.Discount==procOld.Discount) {
				return;
			}
			string message=Lans.g("Procedures","Discount created or changed from Proc Edit window for procedure")
					+": "+ProcedureCodes.GetProcCode(procNew.CodeNum).ProcCode+"  "+Lans.g("Procedures","Dated")
					+": "+procNew.ProcDate.ToShortDateString()+"  "+Lans.g("Procedures","With a Fee of")+": "+procNew.ProcFee.ToString("c")+".  "
					+Lans.g("Procedures","Changed the discount value from")+" "+procOld.Discount.ToString("c")+" "+Lans.g("Procedures","to")+" "
					+procNew.Discount.ToString("c");
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanDiscountEdit,procNew.PatNum,message);
		}

		///<summary></summary>
		public static void UpdateTpProcPriority(Procedure proc) {
			//No need to check MiddleTierRole; no call to db.
			if(proc.ProcStatus!=ProcStat.TP) {
				return;
			}
			//if proc is TP status, update priority on any TreatPlanAttach objects if they are attaching this proc to the active TP
			TreatPlan activePlan=TreatPlans.GetActiveForPat(proc.PatNum);
			if(activePlan==null) {
				return;
			}
			List<TreatPlanAttach> listTpAttaches=TreatPlanAttaches.GetAllForTreatPlan(activePlan.TreatPlanNum);
			//should only be 0 or one TPAttach on this TP with this ProcNum
			listTpAttaches.FindAll(x => x.ProcNum==proc.ProcNum).ForEach(x => x.Priority=proc.Priority);
			TreatPlanAttaches.Sync(listTpAttaches,activePlan.TreatPlanNum);
		}

		///<summary>Called when setting a proc that was not complete to complete.</summary>
		public static bool ProcWasSetComplete(Procedure procNew,Procedure procOld,ProcedureCode procCode) {
			//No need to check MiddleTierRole; no call to db.
			if(procOld.ProcStatus==ProcStat.C || procNew.ProcStatus!=ProcStat.C) {
				return false;
			}
			//Auto-insert default encounter -------------------------------------------------------------------------------------------------------------
			Encounters.InsertDefaultEncounter(procNew.PatNum,procNew.ProvNum,procNew.ProcDate);
			//OrthoProcedures ---------------------------------------------------------------------------------------------------------------------------
			Procedures.SetOrthoProcComplete(procNew,procCode);
			//if status was changed to complete
			Procedures.LogProcComplCreate(procNew.PatNum,procNew,procNew.ToothNum);
			return true;
		}

		///<summary>When proc.ProcStatus is C,EC or EO and the associated proc code paint type is Extraction
		///this updates the tooth paint type to missing.</summary>
		public static void SetToothInitialForCompExtraction(Procedure proc) {
			//No need to check MiddleTierRole; no call to db.
			if(!proc.ProcStatus.In(ProcStat.C,ProcStat.EC,ProcStat.EO)
				|| ProcedureCodes.GetProcCode(proc.CodeNum).PaintType!=ToothPaintingType.Extraction) {
				return;
			}
			ToothInitials.SetValue(proc.PatNum,proc.ToothNum,ToothInitialType.Missing);
		}

		///<summary>Counts the number of patients who have had completed procedures in the date range. D9986 and D9987 are not counted in this query.
		///</summary>
		public static int GetCountPatsComplete(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command=@"SELECT COUNT(DISTINCT PatNum) 
				FROM procedurelog 
				INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum
					AND procedurecode.ProcCode NOT IN('D9986','D9987')
				WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+@"
				AND procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Gets all procedures with a specific StatementNum.  Currently, procedurelog.StatementNum is only used for invoices.</summary>
		public static List<long> GetForInvoice(long statementNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),statementNum);
			}
			if(statementNum==0) {
				return new List<long>();
			}
			string command="SELECT ProcNum FROM procedurelog WHERE procedurelog.StatementNum = "+POut.Long(statementNum);
			return Db.GetListLong(command);
		}

		///<summary>Throws an exception if the given procedure cannot be deleted safely.</summary>
		public static void ValidateDelete(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procNum);
				return;
			}
			//Test to see if the procedure is attached to a claim (excluding pre-auths)
			string command="SELECT COUNT(*) FROM claimproc WHERE ProcNum="+POut.Long(procNum)
				+" AND ClaimNum > 0 AND Status!="+POut.Int((int)ClaimProcStatus.Preauth);
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to a claim."));
			}
			//Test to see if any payment at all has been received for this proc
			command="SELECT COUNT(*) FROM claimproc WHERE ProcNum="+POut.Long(procNum)
				+" AND InsPayAmt > 0 AND Status IN ("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+","
					+POut.Int((int)ClaimProcStatus.CapClaim)+","+POut.Int((int)ClaimProcStatus.CapComplete)+")";
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to an insurance payment."));
			}
			//Test to see if any referrals exist for this proc
			command="SELECT COUNT(*) FROM refattach WHERE ProcNum="+POut.Long(procNum);
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure with referrals attached."));
			}
			//Test to see if any paysplits are attached to this proc
			command="SELECT COUNT(*) FROM paysplit WHERE ProcNum="+POut.Long(procNum);
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to a patient payment."));
			}
			command="SELECT COUNT(*) FROM adjustment WHERE ProcNum="+POut.Long(procNum);
			//While using AvaTax we want procedures with auto-generated sales tax adjustments to be deleted, so that Procedures.Delete can clean up
			//the adjustment and send a call to Avalara, rather than having the user manually delete the adjustment and then the procedure, this is the best
			//way to ensure that our Avalara transaction data is synced with what we have in our program.
			if(AvaTax.IsEnabled()) {
				command+=" AND AdjType<>"+POut.Long(AvaTax.GetSalesTaxAdjType());
			}
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to an adjustment."));
			}
			command="SELECT COUNT(*) FROM rxpat WHERE ProcNum="+POut.Long(procNum);
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to a prescription."));
			}
			command=$"SELECT COUNT(*) FROM payplancharge WHERE payplancharge.ProcNum={POut.Long(procNum)}";
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to a payment plan."));
			}
			command=$"SELECT COUNT(*) FROM payplanlink WHERE payplanlink.FKey={POut.Long(procNum)} " +
				$"AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Procedure)}";
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("Procedures","Not allowed to delete a procedure that is attached to a dynamic payment plan."));
			}
		}

		///<summary>Creates a new procedure with the patient, surface, toothnum, and status for the specified procedure code.
		///Make sure to make a security log after calling this method.  This method requires that Security.CurUser be set prior to invoking.
		///Returns null procedure if one was not created for the patient.</summary>
		public static Procedure CreateProcForPatNum(long patNum,long codeNum,string surf,string toothNum,ProcStat procStatus,long provNum) {
			//No need to check MiddleTierRole; no call to db.
			Patient pat=Patients.GetPat(patNum);
			return CreateProcForPat(pat,codeNum,surf,toothNum,procStatus,provNum);
		}

		///<summary>Creates a new procedure with the patient, surface, toothnum, and status for the specified procedure code.
		///Make sure to make a security log after calling this method.  This method requires that Security.CurUser be set prior to invoking.
		///Returns null procedure if one was not created for the patient.</summary>
		public static Procedure CreateProcForPat(Patient pat,long codeNum,string surf,string toothNum,ProcStat procStatus,long provNum,long aptNum=0
			,List<InsSub> subList=null,List<InsPlan> insPlanList=null,List<PatPlan> patPlanList=null,List<Benefit> benefitList=null,long clinicNum=-1) 
		{
			//No need to check MiddleTierRole; no call to db.
			if(codeNum < 1) {
				return null;
			}
			if(provNum==0) {
				provNum=Patients.GetProvNum(pat);
			}
			Procedure proc=new Procedure();
			proc.PatNum=pat.PatNum;
			if(clinicNum==-1) {
				proc.ClinicNum=Clinics.ClinicNum;
			}
			else {
				proc.ClinicNum=clinicNum;
			}
			proc.ProcStatus=procStatus;
			proc.ProvNum=provNum;
			proc.AptNum=aptNum;
			if(surf=="") {
				proc.Surf="";
			}
			else { 
				proc.Surf=surf; //Note: Sealant code D1351 is not a surface code by default, but can be manually set.  For screens they will be surface specific.
			}
			if(toothNum=="") {
				proc.ToothNum="";
			}
			else {
				proc.ToothNum=toothNum;
			}
			proc.UserNum=Security.CurUser.UserNum;
			proc.CodeNum=codeNum;
			proc.ProcDate=DateTime.Today;
			proc.DateTP=DateTime.Today;
			//The below logic is a trimmed down version of the code existing in ContrChart.AddQuick()
			InsPlan insPlanPrimary=null;
			InsSub insSubPrimary=null;
			if(subList==null) {
				subList=InsSubs.RefreshForFam(Patients.GetFamily(pat.PatNum));
			}
			if(insPlanList==null) {
				insPlanList=InsPlans.RefreshForSubList(subList);
			}
			if(patPlanList==null) {
				patPlanList=PatPlans.Refresh(pat.PatNum);
			}
			if(benefitList==null) {
				benefitList=Benefits.Refresh(patPlanList,subList);
			}
			if(patPlanList.Count>0) {
				insSubPrimary=InsSubs.GetSub(patPlanList[0].InsSubNum,subList);
				insPlanPrimary=InsPlans.GetPlan(insSubPrimary.PlanNum,insPlanList);
			}
			//Get fee schedule and fee amount for dental or medical.
			long feeSch=FeeScheds.GetFeeSched(pat,insPlanList,patPlanList,subList,provNum);
			proc.ProcFee=Fees.GetAmount0(proc.CodeNum,feeSch,proc.ClinicNum,provNum);
			if(insPlanPrimary!=null && insPlanPrimary.PlanType=="p") {//PPO
				double provFee=Fees.GetAmount0(proc.CodeNum,Providers.GetProv(provNum).FeeSched,proc.ClinicNum,provNum);
				proc.ProcFee=Math.Max(proc.ProcFee,provFee);//use greater of standard fee or ins fee
			}
			ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(proc.CodeNum);
			proc.BaseUnits=procCodeCur.BaseUnits;
			proc.SiteNum=pat.SiteNum;
			proc.RevCode=procCodeCur.RevenueCodeDefault;
			proc.DateEntryC=DateTime.Now;
			proc.PlaceService=Clinics.GetPlaceService(proc.ClinicNum);
			proc.ProcNum=Procedures.Insert(proc);
			Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),true,insPlanList,patPlanList,benefitList,pat.Age,subList);
			return proc;
		}

		///<summary>Used by WebSched to create a new procedure for every proc code passed in.  Make sure to make a security log after calling this method.
		///This method requires that Security.CurUser be set prior to invoking.  Returns an empty list if none were created for the patient.</summary>
		public static List<Procedure> CreateProcsForPat(long patNum,List<long> listProcCodeNums,string surf,string toothNum,ProcStat procStatus
			,long provNum,long aptNum,long clinicNum=-1) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<Procedure> listProcedures=new List<Procedure>();
			Patient patient=Patients.GetPat(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(Patients.GetFamily(patNum));
			List<InsPlan> insPlanList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlanList=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlanList,subList);
			foreach(long codeNum in listProcCodeNums) {
				Procedure proc=CreateProcForPat(patient,codeNum,surf,toothNum,procStatus,provNum,aptNum,subList,insPlanList,patPlanList,benefitList,clinicNum);
				if(proc!=null) {
					listProcedures.Add(proc);
				}
			}
			return listProcedures;
		}

		///<summary>Creates the auto ortho procedure for the passed-in patient.</summary>
		public static Procedure CreateOrthoAutoProcsForPat(long patNum,long codeNum,long provNum,long clinicNum, DateTime procDate) {
			//No need to check MiddleTierRole; no call to db.
			Procedure procedure=new Procedure();
			procedure.PatNum=patNum;
			procedure.CodeNum=codeNum;
			procedure.ProvNum=provNum;
			procedure.ClinicNum=clinicNum;
			procedure.ProcStatus=ProcStat.C;
			procedure.Surf="";
			procedure.ToothNum="";
			procedure.UserNum=Security.CurUser.UserNum;
			procedure.ProcDate=procDate;
			procedure.DateEntryC=DateTime.Today;
			procedure.SecDateEntry=DateTime.Today;
			procedure.ProcFee=0;
			procedure.PlaceService=Clinics.GetPlaceService(clinicNum);
			procedure.ProcNum = Procedures.Insert(procedure);
			return procedure;
		}

		///<summary>Called from FormApptsOther when creating a new appointment.  Returns true if there are any procedures marked complete for this patient.  The result is that the NewPt box on the appointment won't be checked.</summary>
		public static bool AreAnyComplete(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM procedurelog "
				+"INNER JOIN procedurecode on procedurecode.CodeNum = procedurelog.CodeNum "
					+"AND procedurecode.ProcCode NOT IN ('D9986','D9987') "
				+"WHERE PatNum="+patNum.ToString()
				+" AND ProcStatus=2";
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()=="0") {
				return false;
			}
			else return true;
		}

		///<summary>Called from AutoCodeItems.  Makes a call to the database to determine whether the specified tooth has been extracted or will be extracted. This could then trigger a pontic code.</summary>
		public static bool WillBeMissing(string toothNum,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),toothNum,patNum);
			}
			//first, check for missing teeth
			string command="SELECT COUNT(*) FROM toothinitial "
				+"WHERE ToothNum='"+toothNum+"' "
				+"AND PatNum="+POut.Long(patNum)
				+" AND InitialType=0";//missing
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="0") {
				return true;
			}
			//then, check for a planned extraction
			command="SELECT COUNT(*) FROM procedurelog,procedurecode "
				+"WHERE procedurelog.CodeNum=procedurecode.CodeNum "
				+"AND procedurelog.ToothNum='"+toothNum+"' "
				+"AND procedurelog.PatNum="+patNum.ToString()+" "
				+"AND procedurelog.ProcStatus <> "+POut.Int((int)ProcStat.D)+" "//Not deleted procedures
				+"AND procedurelog.ProcStatus <> "+POut.Int((int)ProcStat.TPi)+" "//Not inactive treatment planned procedures
				+"AND procedurecode.PaintType=1";//extraction
			table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="0") {
				return true;
			}
			return false;
		}

		///<summary>Used in ContrAccount.CreateClaim when validating selected procedures. Returns true if there is any claimproc for this procedure and plan which is marked NoBillIns.  The claimProcList can be all claimProcs for the patient or only those attached to this proc. Will be true if any claimProcs attached to this procedure are set NoBillIns.</summary>
		public static bool NoBillIns(Procedure proc,List<ClaimProc> claimProcList,long planNum) {
			//No need to check MiddleTierRole; no call to db.
			if(proc==null) {
				return false;
			}
			for(int i=0;i<claimProcList.Count;i++) {
				if(claimProcList[i].ProcNum==proc.ProcNum
					&& claimProcList[i].PlanNum==planNum
					&& claimProcList[i].NoBillIns) {
					return true;
				}
			}
			return false;
		}

		///<summary>Called from FormProcEdit to signal when to disable much of the editing in that form.  If the procedure is 'AttachedToClaim' then user
		///should not change it very much.  Also prevents user from Invalidating a locked procedure if attached to a claim.  The claimProcList can be all
		///claimProcs for the patient or only those attached to this proc.  Ignore preauth claims by setting isPreauthIncluded to false.</summary>
		public static bool IsAttachedToClaim(Procedure proc,List<ClaimProc> claimProcList,bool isPreauthIncluded=true) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<claimProcList.Count;i++) {
				if(claimProcList[i].ProcNum==proc.ProcNum
					&& claimProcList[i].ClaimNum>0
					&& (claimProcList[i].Status==ClaimProcStatus.CapClaim
					|| claimProcList[i].Status==ClaimProcStatus.NotReceived
					|| (claimProcList[i].Status==ClaimProcStatus.Preauth && isPreauthIncluded)
					|| claimProcList[i].Status==ClaimProcStatus.Received
					|| claimProcList[i].Status==ClaimProcStatus.Supplemental
					)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Only called from FormProcEdit.  When attached  to a claim and user clicks Edit Anyway, we need to know the oldest claim date for security reasons.  The claimProcsForProc should only be claimprocs for this procedure.</summary>
		public static DateTime GetOldestClaimDate(List<ClaimProc> claimProcsForProc,bool includePreAuth=true) {
			//No need to check MiddleTierRole; no call to db.
			Claim claim;
			DateTime retVal=DateTime.Today;
			for(int i=0;i<claimProcsForProc.Count;i++) {
				if(claimProcsForProc[i].ClaimNum==0){
					continue;
				}
				if(claimProcsForProc[i].Status==ClaimProcStatus.CapClaim
					|| claimProcsForProc[i].Status==ClaimProcStatus.NotReceived
					|| (claimProcsForProc[i].Status==ClaimProcStatus.Preauth && includePreAuth)
					|| claimProcsForProc[i].Status==ClaimProcStatus.Received
					|| claimProcsForProc[i].Status==ClaimProcStatus.Supplemental
					) 
				{
					claim=Claims.GetClaim(claimProcsForProc[i].ClaimNum);
					if(claim==null) {//Another user probably deleted this claim just now.
						continue;
					}
					if(claim.DateSent<retVal){
						retVal=claim.DateSent;
					}
				}
			}
			return retVal;
		}

		///<summary>Used to check any date restrictions in FormProcEdit for a procedure attached to a sent or recevied preauthorization.</summary>
		public static DateTime GetOldestPreAuth(List<ClaimProc> claimProcsForProc) {
			//No need to check MiddleTierRole; no call to db.
			Claim claim;
			DateTime retVal=DateTime.Today;
			for(int i=0;i<claimProcsForProc.Count;i++) {
				if(claimProcsForProc[i].ClaimNum!=0 && claimProcsForProc[i].Status==ClaimProcStatus.Preauth) {
					claim=Claims.GetClaim(claimProcsForProc[i].ClaimNum);
					if(claim!=null && claim.DateSent<retVal) {
						retVal=claim.DateSent;
					}
				}
			}
			return retVal;
		}

		///<summary>
		///Takes in a procedure and returns proc.DateEntryC if procedure is EO,EC, TP, or TPi. Default parameter used where a textbox is providing the date that is being
		///checked for a permission. See ProcedureL.CheckPermissionsAndGlobalLockDate for example
		///</summary>
		public static DateTime GetDateForPermCheck(Procedure proc,DateTime dateOverride=default) {
			DateTime date=dateOverride==DateTime.MinValue ? proc.ProcDate : dateOverride;
			if(proc.ProcStatus.In(ProcStat.EO,ProcStat.EC,ProcStat.TP,ProcStat.TPi)) {
				date=proc.DateEntryC;
			}
			return date;
		}

		///<summary>Only called from FormProcEditAll to signal when to disable much of the editing in that form. If the procedure is 'AttachedToClaim' then user should not change it very much.  The claimProcList can be all claimProcs for the patient or only those attached to this proc.</summary>
		public static bool IsAttachedToClaim(List<Procedure> procList,List<ClaimProc> claimprocList) {
			//No need to check MiddleTierRole; no call to db.
			for(int j=0;j<procList.Count;j++) {
				if(IsAttachedToClaim(procList[j],claimprocList)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Queries the database to determine if this procedure is attached to a claim already.</summary>
		public static bool IsAttachedToClaim(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT COUNT(*) FROM claimproc "
				+"WHERE ProcNum="+POut.Long(procNum)+" "
				+"AND ClaimNum>0";
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()=="0") {
				return false;
			}
			return true;
		}

		///<summary>Used in ContrAccount.CreateClaim to validate that procedure is not already attached to a claim for this specific insPlan.  The claimProcList can be all claimProcs for the patient or only those attached to this proc.</summary>
		public static bool IsAlreadyAttachedToClaim(Procedure proc,List<ClaimProc> claimProcList,long insSubNum) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<claimProcList.Count;i++) {
				if(claimProcList[i].ProcNum==proc.ProcNum
					&& claimProcList[i].InsSubNum==insSubNum
					&& claimProcList[i].ClaimNum>0
					&& claimProcList[i].Status!=ClaimProcStatus.Preauth) {
					return true;
				}
			}
			return false;
		}

		public static bool IsReferralAttached(long referralNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),referralNum);
			}
			string command="SELECT COUNT(*) FROM procedurelog WHERE OrderingReferralNum="+POut.Long(referralNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Returns true if this procedure needs to be sent to insurance.
		///This happens if there is at least one claimproc attached for this inssub that is an estimate, and it is not set to NoBillIns.
		///The list can be all ClaimProcs for patient or just those for this procedure.</summary>
		public static bool NeedsSent(long procNum,long insSubNum,List<ClaimProc> listClaimProcs) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listClaimProcs.Count;i++) {
				if(listClaimProcs[i].ProcNum==procNum
					&& !listClaimProcs[i].NoBillIns
					&& listClaimProcs[i].InsSubNum==insSubNum
					&& listClaimProcs[i].Status==ClaimProcStatus.Estimate) 
				{
					return true;
				}
			}
			return false;
		}

		///<summary>Only used in ContrAccount.CreateClaim and FormRepeatChargeUpdate.CreateClaim to decide whether a given procedure has an estimate that can be used to attach to a claim for the specified plan.  Returns a valid claimProc if this procedure has an estimate attached that is not set to NoBillIns.  The list can be all ClaimProcs for patient, or just those for this procedure. Returns null if there are no claimprocs that would work.</summary>
		public static ClaimProc GetClaimProcEstimate(long procNum,List<ClaimProc> claimProcList,InsPlan plan,long insSubNum) {
			//No need to check MiddleTierRole; no call to db.
			//bool matchOfWrongType=false;
			for(int i=0;i<claimProcList.Count;i++) {
				if(claimProcList[i].ProcNum==procNum
					&& !claimProcList[i].NoBillIns
					&& claimProcList[i].PlanNum==plan.PlanNum
					&& claimProcList[i].InsSubNum==insSubNum) 
				{
					if(plan.PlanType=="c") {
						if(claimProcList[i].Status==ClaimProcStatus.CapComplete) {
							return claimProcList[i];
						}
					}
					else {//any type except capitation
						if(claimProcList[i].Status==ClaimProcStatus.Estimate) {
							return claimProcList[i];
						}
					}
				}
			}
			return null;
		}

		/// <summary>Used by GetProcsForSingle and GetProcsMultApts to generate a short string description of a procedure.</summary>
		public static string ConvertProcToString(long codeNum,string surf,string toothNum,bool forAccount) {
			//No need to check MiddleTierRole; no call to db.
			ProcedureCode code=ProcedureCodes.GetProcCode(codeNum);
			string strLine=GetToothAndSurfForCodeNum(codeNum,surf,toothNum,!forAccount);
			if(!forAccount) {
				strLine+=" "+code.AbbrDesc;
			}
			else if(code.LaymanTerm!="") {
				strLine+=" "+code.LaymanTerm;
			}
			else {
				strLine+=" "+code.Descript;
			}
			return strLine;
		}

		///<summary></summary>
		private static string GetToothAndSurfForCodeNum(long codeNum,string surf,string toothNum,bool hasToothNum) {
			//No need to check MiddleTierRole; no call to db.
			string strLine="";
			ProcedureCode code=ProcedureCodes.GetProcCode(codeNum);
			switch(code.TreatArea) {
				case TreatmentArea.Surf:
					if(hasToothNum) {
						strLine+="#"+Tooth.Display(toothNum)+"-";//"#12-"
					}
					strLine+=Tooth.SurfTidyFromDbToDisplay(surf,toothNum);//"MOD-"
					break;
				case TreatmentArea.Tooth:
					if(hasToothNum) {
						strLine+="#"+Tooth.Display(toothNum)+"-";//"#12-"
					}
					break;
				default://area 3 or 0 (mouth)
					break;
				case TreatmentArea.Quad:
					strLine+=surf+"-";//"UL-"
					break;
				case TreatmentArea.Sextant:
					strLine+="S"+Tooth.GetSextant(surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers))+"-";//"S2-"
					break;
				case TreatmentArea.Arch:
					strLine+=surf+"-";//"U-"
					break;
				case TreatmentArea.ToothRange:
					//strLine+=table.Rows[j][13].ToString()+" ";//don't show range
					break;
			}//end switch
			return strLine;
		}

		///<summary>Used to display procedure descriptions on appointments. The returned string also includes surf and toothNum.</summary>
		public static string GetDescription(Procedure proc) {
			//No need to check MiddleTierRole; no call to db.
			return ConvertProcToString(proc.CodeNum,proc.Surf,proc.ToothNum,false);
		}

		///<summary>Used to display procedure descriptions on letters. The returned string also includes surf and toothNum.</summary>
		public static string GetDescriptionForLetter(Procedure proc) {
			//No need to check MiddleTierRole; no call to db.
			ProcedureCode code=ProcedureCodes.GetProcCode(proc.CodeNum);
			string retVal;
			if(code.LaymanTerm!="") {
				retVal=code.LaymanTerm;
			}
			else {
				retVal=code.Descript;
			}
			string toothSurf=GetToothAndSurfForCodeNum(code.CodeNum,proc.Surf,proc.ToothNum,true).TrimEnd('-');
			if(!string.IsNullOrWhiteSpace(toothSurf)) {
				retVal+=" "+toothSurf;
			}
			return retVal;
		}

		///<summary>Sets the provider and clinic for a proc based on the appt to which it is attached.  Also sets ProcDate for TP procs.  Changes are reflected in proc returned, but not saved to the db (for synch later).</summary>
		public static Procedure ChangeProcInAppointment(Appointment apt,Procedure proc) {
			//No need to check MiddleTierRole; no call to db.
			if(!IsProcComplEditAuthorized(proc)) {
				//This check is redundant but helps future calls to not miss the security check.
				return proc;//Don't make any changes to procedure.
			}
			if(proc.ProcStatus!=ProcStat.C) {
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				proc.ProvNum=GetProvNumFromAppointment(apt,proc,procCode);
			}
			proc.ClinicNum=apt.ClinicNum;
			if(proc.ProcStatus==ProcStat.TP && apt.AptDateTime!=DateTime.MinValue) {
				proc.ProcDate=apt.AptDateTime;
			}
			return proc;
		}

		private static bool IsProcComplAuthorized(Permissions perm,Procedure proc,bool includeCodeNumAndFee=false) {
			//No need to check MiddleTierRole; no call to db.
			if(!proc.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				return true;//Don't check security if the procedure isn't completed (or EO/EC).
			}
			DateTime dateForPerm=Procedures.GetDateForPermCheck(proc);
			perm=GroupPermissions.SwitchExistingPermissionIfNeeded(perm,proc);
			if(includeCodeNumAndFee) {
				return Security.IsAuthorized(perm,dateForPerm,proc.CodeNum,proc.ProcFee);
			}
			else {
				return Security.IsAuthorized(perm,dateForPerm,true);
			}
		}

		public static bool IsProcComplDeleteAuthorized(Procedure proc,bool includeCodeNumAndFee=false) {
			//No need to check MiddleTierRole; no call to db.
			return IsProcComplAuthorized(Permissions.ProcCompleteStatusEdit,proc,includeCodeNumAndFee);
		}

		public static bool IsProcComplEditAuthorized(Procedure proc,bool includeCodeNumAndFee=false) {
			//No need to check MiddleTierRole; no call to db.
			return IsProcComplAuthorized(Permissions.ProcCompleteEdit,proc,includeCodeNumAndFee);
		}

		///<summary>Returns true when we want to allow procedure fees to be changed.
		///Depending on PrefName.ProcFeeUpdatePrompt, may prompt user for input. We will need to show a MsgBox to the user when promptText is not empty after returing true.  Not translated here. Should only be called after identifying a procedurelog or appointment provider change.</summary>
		public static bool ShouldFeesChange(List<Procedure> listNewProcs,List<Procedure> listOldProcs,ref string promptText,ProcFeeHelper procFeeHelper) {
			//this method was called FeeUpdatePromptHelper
			//No need to check MiddleTierRole; no call to db.
			switch(PrefC.GetInt(PrefName.ProcFeeUpdatePrompt)) {
				case 0://No prompt, don't change fee
					return false;
				case 1://No prompt, always change fee
					//No prompt or check required. Equivilent to clicking "Yes" in some sense. Returns true.
					return true;
				case 2://Prompt if patient portion would be different
					List<ClaimProc> listClaimProcs=ClaimProcs.GetForProcs(listNewProcs.Select(x => x.ProcNum).ToList());
					List<Adjustment> listAdjustments=Adjustments.GetForProcs(listNewProcs.Select(x => x.ProcNum).ToList());
					foreach(Procedure proc in listNewProcs) {
						Procedure procOld=listOldProcs.FirstOrDefault(x => x.ProcNum==proc.ProcNum);
						if(procOld==null) {
							continue;
						}
						procFeeHelper.FillData();
						proc.ProcFee=GetProcFee(procFeeHelper.Pat,procFeeHelper.ListPatPlans,procFeeHelper.ListInsSubs,procFeeHelper.ListInsPlans,proc.CodeNum,
							proc.ProvNum,proc.ClinicNum,proc.MedicalCode,procFeeHelper.ListBenefitsPrimary,procFeeHelper.ListFees);
						decimal procCurPatPortion=ClaimProcs.GetPatPortion(proc,listClaimProcs,listAdjustments);
						decimal procOldPatPortion=ClaimProcs.GetPatPortion(procOld,listClaimProcs,listAdjustments);
						if(procCurPatPortion!=procOldPatPortion) {
							promptText=Lans.g(nameof(Procedures),"The procedure's newly selected provider will change the fee.  Would you like to update the procedure's fee to the newly selected provider's fee?");
							return true;
						}
					}
					return false;
				case 3://Prompt if procedure fee amount is different per our manual
					for(int i=0;i<listNewProcs.Count;i++) {
						Procedure proc=listNewProcs[i];
						Procedure procOld=listOldProcs.FirstOrDefault(x => x.ProcNum==proc.ProcNum);
						if(procOld==null) { // New procedure, so fee can't be changed since it didn't exist before
							continue;
						}
						procFeeHelper.FillData();
						proc.ProcFee=GetProcFee(procFeeHelper.Pat,procFeeHelper.ListPatPlans,procFeeHelper.ListInsSubs,procFeeHelper.ListInsPlans,proc.CodeNum,
							proc.ProvNum,proc.ClinicNum,proc.MedicalCode,procFeeHelper.ListBenefitsPrimary,procFeeHelper.ListFees);
						if(proc.ProcFee!=procOld.ProcFee) { 
							promptText=Lans.g(nameof(Procedures),"Would you like to update procedure fee amounts to the newly selected provider's fees?");
							return true;							
						}
					}
					return false;
			}
			return false;
		}

		///<summary>Goes through the database looking for TP procedures that might need their proc fee updated.
		///Only updates the TP proc fee, does not compute estimates.  Returns number of fees changed.
		///Pass in a valid clinic num, or zero for HQ.  Pass in listFees for the HQ fees, and get individual clinic fees inside here.</summary>
		public static long GlobalUpdateFees(List<Fee> listFeesHQ,long clinicNumGlobal,string progressText) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),listFeesHQ,clinicNumGlobal,progressText);
			}
			//There are three parts to this:
			//1. Spawn multiple threads to queue up DataTables, each with 10,000 procedures.
			//2. In the main thread, process the DataTables and calculate new fees.
						//This requires going to the db for fees. The old way was to use the global fee cache.
						//The new way does something very similar, but with local lists instead of global fee cache.
						//This isn't better/worse/faster/slower than the old way.  It's just part of removing global fee cache. 
						//At the beginning, we get all fees with ClinicNum of zero.
						//Then, with each new clinic, we get all fees with matching ClinicNum.  Takes a few seconds.
						//This may fail (and always did fail) above a certain number (thousands?) of fee schedules because local machine will run out of memory, etc.
						//We will need a copy of such a database to determine new strategy.
			//3. On multiple threads, update the fees on procedures in blocks of 1000 identical fees.
			if(clinicNumGlobal==-1){
				throw new ApplicationException("Must specify ClinicNum or 0 for HQ.");
			}
			if(_odThreadQueueData!=null) {
				throw new ApplicationException("Global update fees tool is already running.");
			}
			FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(("Getting table of fees to update...")
				,progressBarEventType:ProgBarEventType.TextMsg));
			Stopwatch s=new Stopwatch();
			if(ODBuild.IsDebug()) {
				s.Start();
			}
			#region Create Thread Queue Data
			_odThreadQueueData=new ODThread(QueueDataBatches,clinicNumGlobal);
			_odThreadQueueData.Name="GlobalUpdateFeesQueueDataThread";
			_odThreadQueueData.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { _isQueueDone=true; }));
			_isQueueDone=false;
			lock(_lockObjQueueThread) {
				_queueDataTables=new Queue<DataTable>();
			}
			_listProcNumsMaxForGroups=GetProcNumMaxForGroups(ROWS_BATCH_MAX_SIZE,new List<ProcStat>() { ProcStat.TP },clinicNumGlobal);
			if(_totCount==0 || _listProcNumsMaxForGroups.Count==0) {//not likely to happen, this would mean there are 0 TP procedures in the db, nothing to do
				_odThreadQueueData=null;
				return 0;
			}
			_odThreadQueueData.Start(true);
			#endregion Create Queue Batch Data Thread
			#region Get Medical Fee Sched Dict
			bool isMedFeeUsedForNewProcs=PrefC.GetBool(PrefName.MedicalFeeUsedForNewProcs);
			Dictionary<long,long> dictPatNumMedFeeSchedNum=new Dictionary<long,long>();
			if(isMedFeeUsedForNewProcs) {
				string command="SELECT patplan.PatNum,MAX(insplan.FeeSched) medFeeSched "
					+"FROM patplan "
					+"INNER JOIN ("
						+"SELECT patplan.PatNum,MIN(patplan.Ordinal) Ordinal "
						+"FROM patplan "
						+"INNER JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum "
						+"INNER JOIN insplan ON insplan.PlanNum=inssub.PlanNum AND insplan.IsMedical "
						+"GROUP BY patplan.PatNum ";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command+="ORDER BY NULL";
				}
				command+=") medplan ON medplan.PatNum=patplan.PatNum AND medplan.Ordinal=patplan.Ordinal "
					+"INNER JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum "
					+"INNER JOIN insplan ON insplan.PlanNum=inssub.PlanNum "
					+"GROUP BY patplan.PatNum ";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command+="ORDER BY NULL";
				}
				dictPatNumMedFeeSchedNum=Db.GetTable(command).Select()
					.ToDictionary(x => PIn.Long(x["PatNum"].ToString()),x => PIn.Long(x["medFeeSched"].ToString()));
			}
			#endregion Get Medical Fee Sched Dict
			#region Get Variables Used By All Batches
			int rowSkippedCount=0;//used to update progress bar
			int procFeesUpdatedCount=0;//used to report number of fees updated to calling form
			bool isInsPpoAlwaysUseUcrFee=PrefC.GetBool(PrefName.InsPpoAlwaysUseUcrFee);
			Lookup<FeeKey2,Fee> lookupFeesByCodeAndSched=null;
			List<Fee> listFeesHQandClinic=Fees.GetByClinicNum(clinicNumGlobal);//could be empty for some clinics that don't use overrides
			listFeesHQandClinic.AddRange(listFeesHQ);
			lookupFeesByCodeAndSched=(Lookup<FeeKey2,Fee>)listFeesHQandClinic.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
			//lookup will make it very fast to look up the fees we need.
			long practDefaultProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			long practDefaultProvFeeSched=Providers.GetFirstOrDefault(x => x.ProvNum==practDefaultProvNum)?.FeeSched??0;//default to 0 if prov is not found
			long firstNonHiddenProvFeeSched=Providers.GetFirstOrDefault(x => !x.IsHidden)?.FeeSched??0;//default to 0 if all provs hidden (not likely to happen)
			Dictionary<long,long> dictProvFeeSched=Providers.GetDeepCopy().ToDictionary(x => x.ProvNum,x => x.FeeSched);
			//dictionary of fee key linked to a list of lists of longs in order to keep each update statement limited to updating 1000 procedures per query
			Dictionary<double,List<List<long>>> dictFeeListCodes=new Dictionary<double,List<List<long>>>();
			DataTable table=new DataTable();
			long batchNumber=0;
			#endregion Get Variables Used By All Batches
			try {
				while(!_isQueueDone || _queueDataTables.Count>0) {//if batch thread is done and queue is empty, loop is finished
					if(_queueDataTables.Count==0) {
						//queueBatchThread must not be finished gathering batches but the queue is empty, give the batch thread time to catch up
						continue;
					}
					try {
						lock(_lockObjQueueThread) {
							table=_queueDataTables.Dequeue();
							if(ODBuild.IsDebug()) {
								Console.WriteLine("Main thread, dequeue batch, queue count: "+_queueDataTables.Count);
							}
						}
					}
					catch(Exception ex) {//queue must be empty even though we just checked it before entering the while loop, just loop again and wait if necessary
						ex.DoNothing();
						continue;
					}
					batchNumber++;
					double currentRowCount=0; //keeps track of both rows skipped and unskipped
					foreach(DataRow rowCur in table.Rows) {
						#region Get Variables from DataRow
						long codeNum=PIn.Long(rowCur["CodeNum"].ToString());
						long clinicNum=PIn.Long(rowCur["ClinicNum"].ToString());
						long procProvNum=PIn.Long(rowCur["ProvNum"].ToString());
						long patPriProv=PIn.Long(rowCur["PriProv"].ToString());
						long patFeeSched=PIn.Long(rowCur["patFeeSched"].ToString());
						long medCodeNum=PIn.Long(rowCur["medCodeNum"].ToString());
						long procNum=PIn.Long(rowCur["ProcNum"].ToString());
						long procNumLab=PIn.Long(rowCur["ProcNumLab"].ToString());
						double procFeeCur=PIn.Double(rowCur["ProcFee"].ToString());
						long patNum=PIn.Long(rowCur["PatNum"].ToString());
						long patPriPlanFeeSchedNum=PIn.Long(rowCur["planFeeSched"].ToString());
						double percentComplete=Math.Ceiling(((double)currentRowCount/table.Rows.Count)*100);
						long feeSchedCur=0;
						double newFee;
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")
							&& procNumLab!=0) 
						{
							currentRowCount++;
							FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(progressText,(int)percentComplete+"%"
								,(int)percentComplete,100,ProgBarStyle.Blocks,"Clinic"
								,labelTop: "Batch "+batchNumber+"/"+_listProcNumsMaxForGroups.Count));
							continue;//The proc fee for a lab is derived from the lab fee on the parent procedure.
						}
						#endregion Get Variables from DataRow
						#region Med Fee Used and Proc Has Med CodeNum
						if(isMedFeeUsedForNewProcs && medCodeNum>0) {
							if(dictPatNumMedFeeSchedNum.TryGetValue(patNum,out feeSchedCur)) {//use med plan fee sched first
								if(feeSchedCur==0 && patFeeSched>0) {//if med plan fee sched is 0, use pat fee sched second
									feeSchedCur=patFeeSched;
								}
								if(feeSchedCur==0 && patPriProv>0) {//if no pat fee sched, use pat pri prov fee sched third
									dictProvFeeSched.TryGetValue(patPriProv,out feeSchedCur);
								}
								if(feeSchedCur==0) {//if no pat pri prov fee sched, use first non-hidden prov fee sched last
									feeSchedCur=firstNonHiddenProvFeeSched;
								}
							}
						}
						#endregion Med Fee Used and Proc Has Med CodeNum
						#region Dental Fee Sched
						if(feeSchedCur==0) {//not using med plan fees or no med plan for pat or no med codeNum for proc, basically no fee sched found yet
							feeSchedCur=patPriPlanFeeSchedNum;//use pri plan fee sched first
						}
						if(feeSchedCur==0 && patFeeSched>0) {//no pri plan fee sched, use pat fee sched second
							feeSchedCur=patFeeSched;
						}
						if(feeSchedCur==0 && procProvNum>0) {//no pat fee sched, use proc prov fee sched third
							dictProvFeeSched.TryGetValue(procProvNum,out feeSchedCur);
						}
						if(feeSchedCur==0 && patPriProv>0) {//no proc prov fee sched, use pat pri prov fee sched last
							dictProvFeeSched.TryGetValue(patPriProv,out feeSchedCur);
						}
						#endregion Dental Fee Sched
						if(isMedFeeUsedForNewProcs && medCodeNum>0) {
							List<Fee> listFeesForCodeAndSched=lookupFeesByCodeAndSched[new FeeKey2(medCodeNum,feeSchedCur)].ToList();
							newFee=Fees.GetAmount0(medCodeNum,feeSchedCur,clinicNum,procProvNum,listFeesForCodeAndSched);
						}
						else {
							List<Fee> listFeesForCodeAndSched=lookupFeesByCodeAndSched[new FeeKey2(codeNum,feeSchedCur)].ToList();
							newFee=Fees.GetAmount0(codeNum,feeSchedCur,clinicNum,procProvNum,listFeesForCodeAndSched);
						}
						#region PPO Plan, Might Use UCR Fee
						if(rowCur["PlanType"].ToString()=="p") {//PPO plan, might use UCR fee
							feeSchedCur=0;
							if(patPriProv>0) {//use pat pri prov fee sched first
								dictProvFeeSched.TryGetValue(patPriProv,out feeSchedCur);
							}
							if(feeSchedCur==0 && practDefaultProvFeeSched>0) {//no pat pri prov fee sched, use practice default prov fee sched second
								feeSchedCur=practDefaultProvFeeSched;
							}
							if(feeSchedCur==0) {//no practice default prov fee sched, use first non-hidden prov fee sched last
								feeSchedCur=firstNonHiddenProvFeeSched;
							}
							double ucrFee=0;
							List<Fee> listFeesForCodeAndSched=lookupFeesByCodeAndSched[new FeeKey2(codeNum,feeSchedCur)].ToList();
							ucrFee=Fees.GetAmount0(codeNum,feeSchedCur,clinicNum,procProvNum,listFeesForCodeAndSched);
							if(newFee<ucrFee || isInsPpoAlwaysUseUcrFee) {
								newFee=ucrFee;
							}
						}
						#endregion PPO Plan, Might Use UCR Fee
						if(CompareDouble.IsEqual(newFee,procFeeCur)) {
							rowSkippedCount++;
							currentRowCount++;
							FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(progressText,(int)percentComplete+"%"
								,(int)percentComplete,100,ProgBarStyle.Blocks,"Clinic"
								,labelTop:"Batch "+batchNumber+"/"+_listProcNumsMaxForGroups.Count));
							continue;
						}
						if(!dictFeeListCodes.ContainsKey(newFee)) {
							dictFeeListCodes[newFee]=new List<List<long>>() { new List<long>(UPDATE_PROCNUM_IN_MAX_SIZE) };
						}
						if(dictFeeListCodes[newFee].Last().Count>=UPDATE_PROCNUM_IN_MAX_SIZE) {
							dictFeeListCodes[newFee].Add(new List<long>(UPDATE_PROCNUM_IN_MAX_SIZE));
						}
						dictFeeListCodes[newFee].Last().Add(procNum);
						currentRowCount++;
						//update batch label for progress bar
						FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(progressText,(int)percentComplete+"%"
							,(int)percentComplete,100,ProgBarStyle.Blocks,"Clinic",
							labelTop:"Batch "+batchNumber+"/"+_listProcNumsMaxForGroups.Count));
					}//end of foreach loop. Done with one batch of procedures.
					FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(
						"Batch "+batchNumber+"/"+_listProcNumsMaxForGroups.Count+" fee process completed"
						,progressBarEventType:ProgBarEventType.TextMsg));
				}//end of while loop. Done with all batches of procedures.
				FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(progressText,100+"%"
						,100,100,ProgBarStyle.Blocks,"Clinic",labelTop:"Batch "+batchNumber+"/"+_listProcNumsMaxForGroups.Count));
				FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(
					"Procedure fees processed "+progressText+" "+_totCount+"/"+_totCount
					,progressBarEventType:ProgBarEventType.TextMsg));
				if(dictFeeListCodes.Count==0) {
					return 0;//no procedure fees updated, all skipped
				}
				FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(
					"Updating fees...",progressBarEventType:ProgBarEventType.TextMsg));
				#region Create List of Actions			
				List<Action> listActions=dictFeeListCodes.SelectMany(x => x.Value.Select(y => new Action(() => {
					string command="UPDATE procedurelog SET ProcFee="+POut.Double(x.Key)+" WHERE ProcNum IN ("+string.Join(",",y)+")";
					Stopwatch s1=new Stopwatch();
					if(ODBuild.IsDebug()) {
						s1.Start();
					}
					Db.NonQ(command);
					if(ODBuild.IsDebug()) {
						s1.Stop();
						Console.WriteLine("Updated "+y.Count+" procedures, runtime: "+s1.Elapsed.TotalSeconds+" sec");
					}
					procFeesUpdatedCount+=y.Count;
				}))).ToList();
				#endregion Create List of Actions
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(30)
					,onException:new ODThread.ExceptionDelegate((ex) => {
						//Notify the user what went wrong via the text box.
						FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper("Error updating ProcFee: "+ex.Message
							,progressBarEventType:ProgBarEventType.TextMsg));
				}));//each group of actions gets X minutes.
				FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper("Fees Updated Successfully",
						progressBarEventType:ProgBarEventType.TextMsg));
			}//end of try
			catch(Exception ex) {
				ex.DoNothing();
			}
			finally {
				_odThreadQueueData?.QuitAsync();
				_odThreadQueueData=null;
			}
			if(ODBuild.IsDebug()) {
				s.Stop();
				Console.WriteLine("Runtime: "+s.Elapsed.Minutes+" min "+(s.Elapsed.TotalSeconds-(s.Elapsed.Minutes*60))+" sec");
			}
			return procFeesUpdatedCount;
		}

		///<summary>Thread that gets batches of data to put into a queue for another thread to process.</summary>
		private static void QueueDataBatches(ODThread odThread) {
			//No need to check MiddleTierRole; private method.
			Stopwatch s=new Stopwatch();
			if(ODBuild.IsDebug()) {
				s.Start();
			}
			try {
				bool isMedFeeUsedForNewProcs=PrefC.GetBool(PrefName.MedicalFeeUsedForNewProcs);
				long clinicNumGlobal=(long)odThread.Parameters[0];
				List<string> listQueries=new List<string>();
				for(int i=0;i<_listProcNumsMaxForGroups.Count;i++) {
					#region Get ProcNum Range and ClinicNum Where Clauses
					List<string> listWhereAnds=new List<string>();
					if(i>0) {
						listWhereAnds.Add("procedurelog.ProcNum>"+_listProcNumsMaxForGroups[i-1]);
					}
					if(i<_listProcNumsMaxForGroups.Count-1) {
						listWhereAnds.Add("procedurelog.ProcNum<="+_listProcNumsMaxForGroups[i]);
					}
					if(PrefC.HasClinicsEnabled && clinicNumGlobal>-1) { //only add clinic restriction if a clinic was passed in. Defaults to -1.
						listWhereAnds.Add("procedurelog.ClinicNum="+clinicNumGlobal);
					}
					#endregion Get ProcNum Range and ClinicNum Where Clauses
					#region Get Query String
					//Get all TP procedures in this batch's ProcNum range.
					string command="SELECT procedurelog.PatNum,procedurelog.ProcNum,procedurelog.CodeNum,procedurelog.ClinicNum,procedurelog.ProvNum,"
						+"procedurelog.ProcFee,patient.PriProv,patient.FeeSched patFeeSched,procedurelog.ProcNumLab,"
						+(isMedFeeUsedForNewProcs?"COALESCE(procedurecode.CodeNum,0) medCodeNum,":"0 medCodeNum,")
						+"IFNULL(MAX(CASE WHEN p.PatNum IS NOT NULL THEN insplan.PlanType ELSE '' END),'') PlanType,"//greater of "",c,f,or p. p is what we care about.
						+"IFNULL(MAX(CASE WHEN patplan.Ordinal=1 THEN insplan.FeeSched ELSE 0 END),0) planFeeSched "//handles multiple primary patplans
						+"FROM procedurelog "
						+"INNER JOIN patient ON patient.PatNum=procedurelog.PatNum "
						+"LEFT JOIN ("
							+"SELECT patplan.PatNum,MIN(patplan.Ordinal) minOrdinal "
							+"FROM patplan "
							+"INNER JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum "
							+"INNER JOIN insplan ON insplan.PlanNum=inssub.PlanNum AND !insplan.IsMedical "
							+"GROUP BY patplan.PatNum"
						+") p ON patient.PatNum=p.PatNum "
						+"LEFT JOIN patplan ON patplan.PatNum=patient.PatNum AND (patplan.Ordinal=p.MinOrdinal OR patplan.Ordinal=1) "
						+"LEFT JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum "
						+"LEFT JOIN insplan ON insplan.PlanNum=inssub.PlanNum "
						+(isMedFeeUsedForNewProcs?"LEFT JOIN procedurecode ON procedurecode.ProcCode=procedurelog.MedicalCode ":"")
						+"WHERE procedurelog.ProcStatus IN ("+POut.Int((int)ProcStat.TP)+","+POut.Int((int)ProcStat.TPi)+") "
						+(listWhereAnds.Count>0?("AND "+string.Join(" AND ",listWhereAnds)+" "):"")
						+"GROUP BY procedurelog.ProcNum "//because sometimes a pat can have more than one primary patplan and DBM only has "manual fix needed" for this problem
						+"ORDER BY NULL";//an old mysql trick to improve performance. Trick is obsolete as of 5.6.
					listQueries.Add(command);
					#endregion Get Query String
				}
				#region Create List of Actions
				List<Action> listActions=listQueries.Select(x => new Action(() => {
					Stopwatch s1=new Stopwatch();
					if(ODBuild.IsDebug()) {
						s1.Start();
					}
					DataTable table=Db.GetTable(x);
					if(ODBuild.IsDebug()) {
						s1.Stop();
					}
					if(table.Rows.Count>0) {
						while(_queueDataTables.Count>5) {
							//wait until queue is at reasonable size before queueing more. We don't want to hold more than 5 datatables in memory at once. 
							Thread.Sleep(1);
						}
						lock(_lockObjQueueThread) {
							_queueDataTables.Enqueue(table);
							if(ODBuild.IsDebug()) {
								Console.WriteLine(odThread.Name+" - enqueue batch, queue count: "+_queueDataTables.Count+", runtime: "+s1.Elapsed.TotalSeconds+" sec");
							}
						}
					}
				})).ToList();
				#endregion Create List of Actions
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(30),onException:new ODThread.ExceptionDelegate((ex) => {
						//Notify the user what went wrong via the text box.
						FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper("Error getting TP procedures batch: "+ex.Message
							,progressBarEventType:ProgBarEventType.TextMsg));
				}));
			}
			catch(Exception ex) {
				ex.DoNothing();//if error happens, just swallow the error and kill the thread
			}
			finally {//always make sure to notify the main thread that the thread is done so the main thread doesn't wait for eternity
				_isQueueDone=true;
				if(ODBuild.IsDebug()) {
					s.Stop();
					Console.WriteLine(odThread.Name+" - Done, enqueue total count: "+_listProcNumsMaxForGroups.Count
						+", thread runtime: "+s.Elapsed.Minutes+" min "+(s.Elapsed.TotalSeconds-(s.Elapsed.Minutes*60))+" sec");
				}
			}
		}

		///<summary>Sorts the given list based on the procedure's priority, tooth, date, and procnum.
		///SortListByTreatPlanPriority() should be the only method of sorting procedures that need to emulate the treatment plan module.
		///This is to prevent recurring bugs due to different sort methodology.</summary>
		public static List<Procedure> SortListByTreatPlanPriority(List<Procedure> listProcs,List<TreatPlanAttach> listTreatPlanAttaches=null) {
			return SortListByTreatPlanPriority(listProcs,PrefC.GetBool(PrefName.TreatPlanSortByTooth),listTreatPlanAttaches);
		}

		///<summary>Sorts the given list based on the procedure's priority, tooth, date, and procnum.
		///isTreatPlanSortByTooth is generally either PrefC.IsTreatPlanSortByTooth or PrefName.TreatPlanSortByTooth.
		///SortListByTreatPlanPriority() should be the only method of sorting procedures that need to emulate the treatment plan module.
		///This is to prevent recurring bugs due to different sort methodology.</summary>
		public static List<Procedure> SortListByTreatPlanPriority(List<Procedure> listProcs,bool isTreatPlanSortByTooth,List<TreatPlanAttach> listTreatPlanAttaches=null) {
			//No need to check MiddleTierRole; no call to db.
			Dictionary<long,int> dictPriorities=new Dictionary<long, int>();
			Dictionary<long,int> dictProcNumPriority=new Dictionary<long, int>();
			bool hasTreatPlanAttaches=false;
			//Check to see if a list of TreatPlanAttaches was passed in.  If so, use the treatplanattach priorities instead of the priority of the procedure that's in the database.
			//This is used for multiple active treatment plans where we need to display a sorted priority even though that procedure might not have the same priority in another
			//saved treatment plan and thus would have a different priority in the database.
			if(listTreatPlanAttaches!=null) {
				hasTreatPlanAttaches=true;
				dictPriorities=Defs.GetDefsForCategory(DefCat.TxPriorities).ToDictionary(x=>x.DefNum,x=>x.ItemOrder);
				listTreatPlanAttaches.ForEach(x => dictProcNumPriority[x.ProcNum]=(x.Priority==0 ? -1 : dictPriorities[x.Priority]));
			}
			List<Procedure> listLabProcs=listProcs.Where(x => x.ProcNumLab!=0).Select(x => x.Copy()).ToList();//Canadian Lab Procs
			List<long> listLabProcNums=listLabProcs.Select(x => x.ProcNum).ToList();
			listProcs.RemoveAll(x => listLabProcNums.Contains(x.ProcNum));//Remove all labs from this list if any.  Labs are always below their parent proc.
			//Procedure code is purposefully not included in the sorting of this list.  It will ruin PrefName.TreatPlanSortByTooth sorting.
			List<Procedure> listOrderedProcs=listProcs
				.OrderBy(x => (hasTreatPlanAttaches ? dictProcNumPriority[x.ProcNum] : x.PriorityOrder)<0)
				.ThenBy(x => (hasTreatPlanAttaches ? dictProcNumPriority[x.ProcNum] : x.PriorityOrder))
				//.ThenBy(x => isTreatPlanSortByTooth ? x.ToothRange : 0)
				.ThenBy(x => isTreatPlanSortByTooth ? Tooth.ToInt(x.ToothNum) : 0)//Sorting by a constant causes nothing to happen.
				.ThenBy(x => x.ProcDate)
				.ThenBy(x => x.ProcNum)//This is necessary in case isTreatPlanSortByTooth is false, and to break any ties between the other sorting methods.
				.ToList();
			List<long> listParentProcNums=listLabProcs.Select(x => x.ProcNumLab).Distinct().ToList();//There can be up to 2 lab procs for each parent proc.
			for(int i=listOrderedProcs.Count-1;i>=0;i--) {//Loop backward so we can insert as we go without affecting the index.
				if(!listParentProcNums.Contains(listOrderedProcs[i].ProcNum)) {
					continue;//Not a parent proc.
				}
				listOrderedProcs.InsertRange(i+1,listLabProcs.Where(x => x.ProcNumLab==listOrderedProcs[i].ProcNum).ToList());//Insert labs below parent proc.
			}
			return listOrderedProcs;
		}

		///<summary>Checks for frequency conflicts with the passed-in list of procedures.
		///Returns empty string if there are no conflicts, new line delimited list of proc codes if there are.  Throws exceptions.</summary>
		public static string CheckFrequency(List<Procedure> procList,long patNum,DateTime aptDateTime) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),procList,patNum,aptDateTime);
			}
			if(procList==null) {
				throw new ArgumentException("Invalid procedure list passed in.","procList");
			}
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				throw new ArgumentException("Patient not found in database.","patNum");
			}
			if(aptDateTime==null) {
				throw new ArgumentException("Appointment Date not present.","aptDateTime");
			}
			List<Procedure> procListNew=procList.Select(x => x.Copy()).ToList();//Because we're modifying the procedures in this method
			string frequencyConflicts="";
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(patNum);
			if(!PatPlans.IsPatPlanListValid(listPatPlans)) {
				//need to validate due to call to GetHistList below
				listPatPlans=PatPlans.Refresh(patNum);
			}
			if(listPatPlans.Count<1) {
				return "";
			}
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			List<InsPlan> listInsPlans=InsPlans.GetByInsSubs(listInsSubs.Select(x => x.InsSubNum).ToList());
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			listBenefits.AddRange(Benefits.GetForPatPlansAndProcs(listPatPlans.Select(x => x.PatPlanNum).ToList(),procListNew.Select(x => x.CodeNum).ToList()));
			List<ClaimProcHist> listClaimProcsHist=ClaimProcs.GetHistList(patNum,listBenefits,listPatPlans,listInsPlans,DateTime.Now,listInsSubs);
			List<ClaimProc> listClaimProcsForProcs=ClaimProcs.GetForProcs(procListNew.Select(x => x.ProcNum).ToList());
			if(aptDateTime!=DateTime.MinValue) {
				foreach(Procedure proc in procListNew) {
					proc.ProcDate=aptDateTime.Date;
				}
				foreach(ClaimProc claimProc in listClaimProcsForProcs) {
					claimProc.ProcDate=aptDateTime.Date;
				}
			}
			//Get data for any OrthoCases that may be linked to procs in procListNew
			List<OrthoCase> listOrthoCases=OrthoCases.Refresh(patNum);
			List<OrthoProcLink> listOrthoProcLinksAll=OrthoProcLinks.GetManyByOrthoCases(listOrthoCases.Select(x=>x.OrthoCaseNum).ToList());
			List<long> listProcNums=procListNew.Select(x=>x.ProcNum).ToList();
			List<OrthoProcLink> listOrthoProcLinks=listOrthoProcLinksAll.FindAll(x=>listProcNums.Contains(x.ProcNum));
			List<OrthoSchedule> listOrthoSchedules=new List<OrthoSchedule>();
			if(listOrthoProcLinks.Count>0) {
				List<long> listSchedulePlanLinksFKey=OrthoPlanLinks.GetAllForOrthoCasesByType(listOrthoCases.Select(x=>x.OrthoCaseNum).ToList(),OrthoPlanLinkType.OrthoSchedule).Select(x=>x.FKey).ToList();
				listOrthoSchedules=OrthoSchedules.GetMany(listSchedulePlanLinksFKey);
			}
			List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
			BlueBookEstimateData blueBookEstimateData=new BlueBookEstimateData(listInsPlans,listInsSubs,listPatPlans,procListNew,listSubstLinks);
			for(int i=0;i<procListNew.Count;i++) {
				OrthoCase orthoCase=null;
				OrthoSchedule orthoSchedule=null;
				List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
				OrthoProcLink orthoProcLink=listOrthoProcLinks.Find(x => x.ProcNum==procListNew[i].ProcNum);
				if(orthoProcLink!=null) {
					long orthoCaseNum=orthoProcLink.OrthoCaseNum;
					orthoCase=listOrthoCases.Find(x=>x.OrthoCaseNum==orthoCaseNum);
					orthoSchedule=listOrthoSchedules.Find(x=>x.OrthoScheduleNum==orthoCaseNum);
					listOrthoProcLinksForOrthoCase=listOrthoProcLinksAll.FindAll(x=>x.OrthoCaseNum==orthoCaseNum);
				}
				ComputeEstimates(procListNew[i],pat.PatNum,ref listClaimProcsForProcs,false,listInsPlans,listPatPlans,listBenefits,listClaimProcsHist,null,false,pat.Age
					,listInsSubs,listSubstLinks:listSubstLinks,orthoProcLink:orthoProcLink,orthoCase:orthoCase,orthoSchedule:orthoSchedule
					,listOrthoProcLinksForOrthoCase:listOrthoProcLinksForOrthoCase,blueBookEstimateData:blueBookEstimateData);
				ClaimProc claimProc=listClaimProcsForProcs.Find(x => x.ProcNum==procListNew[i].ProcNum);
				if(claimProc!=null && !string.IsNullOrEmpty(claimProc.EstimateNote) && claimProc.EstimateNote.Contains("Frequency Limitation")) {
					if(frequencyConflicts!="") {
						frequencyConflicts+="\r\n";
					}
					frequencyConflicts+=ProcedureCodes.GetStringProcCode(procListNew[i].CodeNum);
				}
			}
			return frequencyConflicts;
		}

		///<summary></summary>
		public static void ComputeEstimates(Procedure proc,long patNum,List<ClaimProc> claimProcs,bool isInitialEntry,List<InsPlan> planList,
			List<PatPlan> patPlans,List<Benefit> benefitList,
			//missing histList,loopList,saveToDb
			int patientAge,List<InsSub> subList,
			OrthoProcLink orthoProcLink=null,OrthoCase orthoCase=null,OrthoSchedule orthoSchedule=null,List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null,
			//missing listClaimProcsAll,isClaimProcRemoveNeeded,useProcDateOnProc,listSubLinks,isForOrtho
			List<Fee> listFees=null,BlueBookEstimateData blueBookEstimateData=null) 
		{
			//No need to check MiddleTierRole; no call to db.
			ComputeEstimates(proc,patNum,ref claimProcs,isInitialEntry,planList,patPlans,benefitList,
				null,null,true,
				patientAge,subList,
				listClaimProcsAll:null,isClaimProcRemoveNeeded:false,useProcDateOnProc:false,listSubstLinks:null,isForOrtho:false,//null,false,false,ListSubstLinks,false
				listFees:listFees,lookupFees:null,orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCase,blueBookEstimateData);
		}

		///<summary>Used whenever a procedure changes or a plan changes.  All estimates for a given procedure must be updated. This frequently includes 
		///adding claimprocs, but can also just edit the appropriate existing claimprocs. Skips status=Adjustment,CapClaim,Preauth,Supplemental.  
		///Also fixes date,status,and provnum if appropriate.  The claimProc list only needs to include claimprocs for this proc, although it can 
		///include more.</summary>
		///<param name="claimProcs">claimProcs is the list of claimprocs for the patNum, sometimes only estimate cp's or only those for non-completed and
		///non-deleted procs and other times all claimprocs for the patNum or all claimprocs for the procedure, modified by this method if necessary.</param>
		///<param name="isInitialEntry">Set this to true when this is the first time computing estimates for this procedure. It is only used for 
		///capitation plans. If true, a claim proc that is CapComplete will still calculate estimate.</param>
		///<param name="planList">planList is insplans for the subList, so all insplans for the family of patNum.</param>
		///<param name="patPlans">patPlans is patplans for the patNum (NOT for entire family).</param>
		///<param name="benefitList">benefitList is benefits with PatPlanNum in patPlans or PlanNum in subList.PlanNums where subList[i].InsSubNum is in
		///the patPlans.InsSubNums.  So benefitList is only benefits affecting patPlans for patNum (NOT for entire family).</param>
		///<param name="histList">histList contains claimprochists for the pat or family (depending on coverage level, individual or family) where the
		///claimproc.InsSubNum is in patPlans.InsSubNums and there is a benefit in benefitList that applies to this claimproc.  Used for determining
		///annual max, deductibles, and other limitations/frequency limits.</param>
		///<param name="loopList">loopList contains all claimprocs in the current list (TP or claim) that come before this procedure.  Used in the TP
		///module and in claims to determine (pending) benefits used prior to this one.  loopList can be null, i.e. FormProcEdit or checking frequency
		///limits for appt scheduling, since there is no list of pending benefits used prior to this one.</param>
		///<param name="subList">subList is the list of inssubs for the family of patNum.</param>
		///<param name="orthoProcLink">The link between proc and an ortho case.</param>
		///<param name="orthoCase">The ortho case the proc is linked to.</param>
		///<param name="orthoSchedule">Contains fee information regarding an ortho case.</param>
		///<param name="listOrthoProcLinksForOrthoCase">All proc links for an ortho case.</param>
		///<param name="listClaimProcsAll">listClaimProcsAll holds all claimprocs for the patNum, even ones that are received. The purpose of this list
		///is to hold all claim procs for reference. This list will not be modified.  Only sent from PatPlans.Delete, otherwise it's set to claimProcs.</param>
		///<param name="listSubstLinks">If null, gets list from db, so don't use this in a loop with listSubstLinks null.</param>
		///<param name="listFees">Normally, pass in a short listFees or short lookupFees. Ok to leave null if not much looping, and can get fee directly from db.  listFees gets converted to lookupFees before being passed off to Procedures.ComputerForOrdinal</param>
		///<param name="lookupFees">In the case of GlobalUpdateWriteoffs, pass in this huge lookup instead of listFees.</param>
		///<param name="blueBookEstimateData">If null, we will create it before calling ComputeForOrdinal(). Data needed to produce estimates using the Blue Book feature.</param>
		public static void ComputeEstimates(
			Procedure proc,
			long patNum,
			ref List<ClaimProc> claimProcs,
			bool isInitialEntry,
			List<InsPlan> planList,
			List<PatPlan> patPlans,
			List<Benefit> benefitList,
			List<ClaimProcHist> histList,
			List<ClaimProcHist> loopList,
			bool saveToDb,//can default to null,null,true
			int patientAge,
			List<InsSub> subList,
			List<ClaimProc> listClaimProcsAll=null,
			bool isClaimProcRemoveNeeded=false,
			bool useProcDateOnProc=false,
			List<SubstitutionLink> listSubstLinks=null,
			bool isForOrtho=false,
			List<Fee> listFees=null,
			Lookup<FeeKey2,Fee> lookupFees=null,
			OrthoProcLink orthoProcLink=null,
			OrthoCase orthoCase=null,
			OrthoSchedule orthoSchedule=null,
			List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null,
			BlueBookEstimateData blueBookEstimateData=null,
			List<long> listApptNums=null) 
		{
			if(PrefC.GetBool(PrefName.EnterpriseHygProcUsePriProvFee) && ProcedureCodes.GetProcCode(proc.CodeNum).IsHygiene) {
				if(listApptNums==null) {
					listApptNums=new List<long>();
					listApptNums.Add(proc.AptNum);
					listApptNums.Add(proc.PlannedAptNum);
					listApptNums=listApptNums.FindAll(x=>x!=0).Distinct().ToList();
				}
				proc=proc.Copy();
				Appointment appointment=null;
				if(listApptNums.Contains(proc.AptNum)) {
					appointment=Appointments.GetOneApt(proc.AptNum);
					if(appointment!=null) {
						proc.ProvNum=appointment.ProvNum;
					}
				}else if(listApptNums.Contains(proc.PlannedAptNum)) {
					appointment=Appointments.GetOneApt(proc.PlannedAptNum);
					if(appointment!=null) {
						proc.ProvNum=appointment.ProvNum;
					}
				}
			}
			//No need to check MiddleTierRole; no call to db.
			//If an orthoCase or an orthProcLink was passed in with the other orthocase related data left null, we must get that data.
			if(orthoProcLink!=null || orthoCase!=null) {
				long orthoCaseNum=0;
				if(orthoProcLink!=null) {
					orthoCaseNum=orthoProcLink.OrthoCaseNum;
					if(orthoCase==null) {
						orthoCase=OrthoCases.GetOne(orthoProcLink.OrthoCaseNum);
					}
				}
				else if(orthoCase!=null) {
					orthoCaseNum=orthoCase.OrthoCaseNum;
					if(orthoProcLink==null) {
						orthoProcLink=OrthoProcLinks.GetByProcNum(proc.ProcNum);
					}
				}
				if(orthoSchedule==null) {
					OrthoPlanLink orthoSchedulePlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
					orthoSchedule=OrthoSchedules.GetOne(orthoSchedulePlanLink.FKey);
				}
				if(listOrthoProcLinksForOrthoCase==null) {
					listOrthoProcLinksForOrthoCase=OrthoProcLinks.GetManyByOrthoCase(orthoCaseNum);
				}
			}
			if(listClaimProcsAll==null){
				listClaimProcsAll=claimProcs;
			}
			bool isHistorical=false;
			if(proc.ProcDate<DateTime.Today && proc.ProcStatus==ProcStat.C) {
				isHistorical=true;//Don't automatically create an estimate for completed procedures, especially if they are older than today.  Very important after a conversion from another software.
				//Special logic in place only for capitation plans:
				if(planList.Any(x => x.PlanType=="c") //11/19/2012 js We had a specific complaint where changing plan type to capitation automatically added WOs to historical procs.
					&& !listClaimProcsAll.Any(x => x.ProcNum==proc.ProcNum 
						&& x.Status.In(ClaimProcStatus.CapClaim,ClaimProcStatus.CapComplete,ClaimProcStatus.CapEstimate))) 
				{
					//If there are any capitation plans but no capitation claimproc.statuses then return.
					//04/02/2013 Jason- To relax this filter for offices that enter treatment a few days after it's done, we will see if any capitation statuses exist.
					return;//There are no capitation claimprocs for this procedure, therefore we don't want to touch/damage this proc.
				}
			}
			//first test to see if each estimate matches an existing patPlan (current coverage),
			//delete any other estimates
			List<long> listDeletedClaimProcNums=new List<long>();
			for(int i=0;i<claimProcs.Count;i++) {
				if(claimProcs[i].ProcNum!=proc.ProcNum) {
					continue;
				}
				if(claimProcs[i].PlanNum==0) {
					continue;
				}
				if(claimProcs[i].Status!=ClaimProcStatus.Estimate && claimProcs[i].Status!=ClaimProcStatus.CapEstimate) {
					continue;
				}
				bool planIsCurrent=false;
				for(int p=0;p<patPlans.Count;p++) {
					if(patPlans[p].InsSubNum==claimProcs[i].InsSubNum
						&& InsSubs.GetSub(patPlans[p].InsSubNum,subList).PlanNum==claimProcs[i].PlanNum) 
					{
						planIsCurrent=true;
						break;
					}
				}
				//If claimProc estimate is for a plan that is not current, delete it
				if(!planIsCurrent) {
					if(saveToDb) {
						ClaimProcs.Delete(claimProcs[i]);
					}
					else {
						claimProcs[i].DoDelete=true;
					}
					listDeletedClaimProcNums.Add(claimProcs[i].ClaimProcNum);
				}
			}
			if(isClaimProcRemoveNeeded) {
				//Remove all claimProcs which were deleted.
				//This prevents Canadian lab procedures from generating claimProcs for deleted parent claimProcs. 
				claimProcs.RemoveAll(x => listDeletedClaimProcNums.Contains(x.ClaimProcNum));
			}
			InsPlan planCur;
			InsSub subCur;
			//bool estExists;
			bool cpAdded=false;
			//loop through all patPlans (current coverage), and add any missing estimates
			for(int p=0;p<patPlans.Count;p++) {//typically, loop will only have length of 1 or 2
				//Don't automatically create an estimate for completed procedures, especially if they are older than today.
				//However, we have an optional preference for users that knowingly accept this danger and have a workflow that requires this.
				if(isHistorical && !PrefC.GetBool(PrefName.ClaimProcsAllowedToBackdate) && !isForOrtho) {
					break;
				}
				else if(proc.ProcNumLab!=0) {
					break;//Do not add estimates for labs, they are handeld by their parent procedure.
				}
				PatPlan patPlanCur=patPlans[p];
				//test to see if estimate exists
				if(listClaimProcsAll.Any(x => x.ProcNum==proc.ProcNum
					&& x.PlanNum!=0
					&& x.InsSubNum==patPlanCur.InsSubNum
					&& !x.Status.In(ClaimProcStatus.CapClaim,ClaimProcStatus.Preauth,ClaimProcStatus.Supplemental)))
				{
					continue;//estimate exists
				}
				//estimate is missing, so add it.
				subCur=InsSubs.GetSub(patPlanCur.InsSubNum,subList);
				planCur=InsPlans.GetPlan(subCur.PlanNum,planList);
				if(planCur==null){//subCur can never be null) {//??
					continue;//??
				}
				ClaimProc cp=new ClaimProc();
				cp.ProcNum=proc.ProcNum;
				cp.PatNum=patNum;
				cp.ProvNum=proc.ProvNum;
				if(planCur.PlanType=="c") {
					if(proc.ProcStatus==ProcStat.C) {
						cp.Status=ClaimProcStatus.CapComplete;
					}
					else {
						cp.Status=ClaimProcStatus.CapEstimate;//this may be changed below
					}
				}
				else {
					cp.Status=ClaimProcStatus.Estimate;
				}
				cp.PlanNum=planCur.PlanNum;
				cp.InsSubNum=subCur.InsSubNum;
				//Capitation procedures are not usually attached to a claim.
				//In order for Aging to calculate properly the ProcDate (Date Completed) and DateCP (Payment Date) must be the same.
				//If the following line of code changes, then we need to preserve this existing behavior specifically for CapComplete.
				cp.DateCP=proc.ProcDate;
				cp.AllowedOverride=-1;
				cp.PercentOverride=-1;
				cp.NoBillIns=InsPlanPreferences.NoBillIns(ProcedureCodes.GetProcCode(proc.CodeNum),planCur);
				cp.PaidOtherIns=-1;
				cp.CopayOverride=-1;
				cp.ProcDate=proc.ProcDate;
				cp.BaseEst=0;
				cp.InsEstTotal=0;
				cp.InsEstTotalOverride=-1;
				cp.DedEst=-1;
				cp.DedEstOverride=-1;
				cp.PaidOtherInsOverride=-1;
				cp.WriteOffEst=-1;
				cp.WriteOffEstOverride=-1;
				//ComputeBaseEst will fill AllowedOverride,Percentage,CopayAmt,BaseEst
				if(saveToDb) {
					ClaimProcs.Insert(cp);
				}
				else {
					claimProcs.Add(cp);//this newly added cp has no ClaimProcNum and is not yet in the db.
				}
				cpAdded=true;
			}
			//if any were added, refresh the list
			if(cpAdded && saveToDb) {//no need to refresh the list if !saveToDb, because list already made current.
				claimProcs=ClaimProcs.Refresh(patNum);
			}
			if(orthoProcLink!=null) {//For procs linked to Orthocases,estimates are calculated based off of orthocase info, not insurance info.
				//Need all claimprocs for pat, including any that were added but not inserted yet.
				if(cpAdded && saveToDb) {
					listClaimProcsAll=claimProcs;
				}
				else {
					listClaimProcsAll=ClaimProcs.Refresh(patNum);
					listClaimProcsAll.AddRange(claimProcs.Where(x => x.ClaimProcNum==0).ToList());//Add any that are made but not in DB yet.
				}
				ClaimProcs.ComputeEstimatesByOrthoCase(proc,orthoProcLink,orthoCase,orthoSchedule,saveToDb,listClaimProcsAll,claimProcs,patPlans
					,listOrthoProcLinksForOrthoCase);
				return;
			}
			double paidOtherInsEstTotal=0;
			double paidOtherInsBaseEst=0;
			double writeOffEstOtherIns=0;
			if(listSubstLinks==null){
				listSubstLinks=SubstitutionLinks.GetAllForPlans(planList);
			}
			if(listFees!=null && lookupFees==null){
				//we are just dealing with a very short list of fees, so this doesn't cost anything.
				lookupFees=(Lookup<FeeKey2,Fee>)listFees.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
			}
			if(blueBookEstimateData==null) {
				blueBookEstimateData=new BlueBookEstimateData(planList,subList,patPlans,new List<Procedure>() { proc },listSubstLinks);
			}
			//because secondary claimproc might come before primary claimproc in the list, we cannot simply loop through the claimprocs
			ComputeForOrdinal(1,claimProcs,proc,planList,isInitialEntry,ref paidOtherInsEstTotal,ref paidOtherInsBaseEst,ref writeOffEstOtherIns,
				patPlans,benefitList,histList,loopList,saveToDb,patientAge,subList,listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData);
			ComputeForOrdinal(2,claimProcs,proc,planList,isInitialEntry,ref paidOtherInsEstTotal,ref paidOtherInsBaseEst,ref writeOffEstOtherIns,
				patPlans,benefitList,histList,loopList,saveToDb,patientAge,subList,listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData);
			ComputeForOrdinal(3,claimProcs,proc,planList,isInitialEntry,ref paidOtherInsEstTotal,ref paidOtherInsBaseEst,ref writeOffEstOtherIns,
				patPlans,benefitList,histList,loopList,saveToDb,patientAge,subList,listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData);
			ComputeForOrdinal(4,claimProcs,proc,planList,isInitialEntry,ref paidOtherInsEstTotal,ref paidOtherInsBaseEst,ref writeOffEstOtherIns,
				patPlans,benefitList,histList,loopList,saveToDb,patientAge,subList,listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData);	
			//At this point, for a PPO with secondary, the sum of all estimates plus primary writeoff might be greater than fee.
			if(patPlans.Count>1){
				subCur=InsSubs.GetSub(patPlans[0].InsSubNum,subList);
				planCur=InsPlans.GetPlan(subCur.PlanNum,planList);
				if(planCur.PlanType=="p") {
					//claimProcs=ClaimProcs.Refresh(patNum);
					//ClaimProc priClaimProc=null;
					int priClaimProcIdx=-1;
					double sumPay=0;//Either actual or estimate
					for(int i=0;i<claimProcs.Count;i++){
						if(claimProcs[i].ProcNum!=proc.ProcNum){
							continue;
						}
						if(claimProcs[i].Status==ClaimProcStatus.Received && !PrefC.GetBool(PrefName.InsEstRecalcReceived)) {
							continue;
						}
						if(claimProcs[i].Status.In(ClaimProcStatus.Adjustment,ClaimProcStatus.CapClaim,ClaimProcStatus.CapComplete,ClaimProcStatus.CapEstimate,
							ClaimProcStatus.Preauth,ClaimProcStatus.InsHist))
						{
							continue;
						}
						if(claimProcs[i].PlanNum==planCur.PlanNum && claimProcs[i].WriteOffEst>0){
							priClaimProcIdx=i;
						}
						if(claimProcs[i].Status==ClaimProcStatus.Received
							|| claimProcs[i].Status==ClaimProcStatus.Supplemental ){
							sumPay+=claimProcs[i].InsPayAmt;
						}
						if(claimProcs[i].Status==ClaimProcStatus.Estimate){
							if(!CompareDouble.IsEqual(claimProcs[i].InsEstTotalOverride,-1)){
								sumPay+=claimProcs[i].InsEstTotalOverride;
							}
							else{
								sumPay+=claimProcs[i].InsEstTotal;
							}
						}
						if(claimProcs[i].Status==ClaimProcStatus.NotReceived){
							sumPay+=claimProcs[i].InsPayEst;
						}
					}
					//Alter primary WO if needed.
					if(priClaimProcIdx!=-1){
						double procFee=proc.ProcFeeTotal;
						if(sumPay+claimProcs[priClaimProcIdx].WriteOffEst > procFee) {
							double writeOffEst=procFee-sumPay;
							if(writeOffEst<0) {
								writeOffEst=0;
							}
							claimProcs[priClaimProcIdx].WriteOffEst=writeOffEst;
							if(saveToDb){
								ClaimProcs.Update(claimProcs[priClaimProcIdx]);
							}
						}
					}
				}
			}
			#region Sales Tax
			//At the end of recalculating writeoffs, we need to update sales tax estimate for treatment planned procs.
			if(proc.ProcStatus==ProcStat.TP && !PrefC.IsODHQ) {
				proc.TaxAmt=ClaimProcs.ComputeSalesTax(proc,claimProcs,true);
			}
			#endregion
		}

		///<summary>Passing in ordinal 4 will compute for 4 as well as any other situation such as dropped plan.
		///For Canada, a lab estimate will be created and added to list claimProcs if any of the procs have labs without estimates.</summary>
		private static void ComputeForOrdinal(int ordinal,List<ClaimProc> claimProcs,Procedure proc,List<InsPlan> planList,bool isInitialEntry,
			ref double paidOtherInsEstTotal,ref double paidOtherInsBaseEst,ref double writeOffEstOtherIns,
			List<PatPlan> patPlans,List<Benefit> benefitList,List<ClaimProcHist> histList,List<ClaimProcHist> loopList,bool saveToDb,int patientAge,
			List<InsSub> listInsSubs,List<SubstitutionLink> listSubstLinks,bool useProcDateOnProc,Lookup<FeeKey2,Fee> lookupFees,BlueBookEstimateData blueBookEstimateData)
			//lookupFees passed in will also contain all possible alternate codes and medical codes
		{
			//No need to check MiddleTierRole; no call to db.
			InsPlan PlanCur;
			PatPlan patplan;
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
			for(int i=0;i<claimProcs.Count;i++) {
				bool hasEstimateCalculation=true;
				if(claimProcs[i].Status==ClaimProcStatus.Received && !PrefC.GetBool(PrefName.InsEstRecalcReceived)) {
					//Do not recalculate insurance estimates on received claimprocs.  However, we still need to include the insurance payments in the 
					//calculation of any secondary, tertiary, etc insurance plans for each procedure.  In this case, we want to make sure writeOffEstOtherIns,
					//paidOtherInsEstTotal, and paidOtherInsBaseEst include the Received claimproc estimates/payments, while still not changing the received 
					//claimproc estimates due to Frequency Limitations or Waiting Periods (calculated in this method, rather than ClaimProcs.ComputeBaseEst()).
					hasEstimateCalculation=false;
				}
				if(claimProcs[i].ProcNum!=proc.ProcNum) {
					continue;
				}
				PlanCur=InsPlans.GetPlan(claimProcs[i].PlanNum,planList);
				if(PlanCur==null) {
					continue;//in older versions it still did a couple of small things even if plan was null, but don't know why
					//example:cap estimate changed to cap complete, and if estimate, then provnum set
					//but I don't see how PlanCur could ever be null
				}
				patplan=PatPlans.GetFromList(patPlans,claimProcs[i].InsSubNum);
				//capitation estimates are always forced to follow the status of the procedure
				if(PlanCur.PlanType=="c"
					&& (claimProcs[i].Status==ClaimProcStatus.CapComplete	|| claimProcs[i].Status==ClaimProcStatus.CapEstimate)) 
				{
					if(isInitialEntry) {
						//this will be switched to CapComplete further down if applicable.
						//This makes ComputeBaseEst work properly on new cap procs w status Complete
						claimProcs[i].Status=ClaimProcStatus.CapEstimate;
					}
					else if(proc.ProcStatus==ProcStat.C) {
						claimProcs[i].Status=ClaimProcStatus.CapComplete;
						//Capitation procedures are not usually attached to a claim.
						//In order for Aging to calculate properly the ProcDate (Date Completed) and DateCP (Payment Date) must be the same.
						claimProcs[i].DateCP=proc.ProcDate;
					}
					else {
						claimProcs[i].Status=ClaimProcStatus.CapEstimate;
					}
				}
				//ignored: adjustment,InsHist
				//ComputeBaseEst automatically skips: capComplete,Preauth,capClaim,Supplemental
				//does recalc est on: CapEstimate,Estimate,NotReceived,Received
				//the cp is altered within ComputeBaseEst, but not saved unless cp is associated
				//to a lab then the sibling labs claimProcs are updated in CanadianLabBaseEstHelper(...)
				if(patplan==null) {//the plan for this claimproc was dropped 
					if(ordinal!=4) {//only process on the fourth round
						continue;
					}
					ClaimProcs.ComputeBaseEst(claimProcs[i],proc,PlanCur,0,
						benefitList,histList,loopList,patPlans,0,0,patientAge,0,planList,listInsSubs,listSubstLinks,useProcDateOnProc,lookupFees,
						blueBookEstimateData:blueBookEstimateData);
				}
				else if(patplan.Ordinal==1){
					if(ordinal!=1) {
						continue;
					}
					ClaimProcs.ComputeBaseEst(claimProcs[i],proc,PlanCur,patplan.PatPlanNum,
						benefitList,histList,loopList,patPlans,paidOtherInsEstTotal,paidOtherInsBaseEst,patientAge,writeOffEstOtherIns,planList,listInsSubs,
						listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData:blueBookEstimateData);
				}
				else if(patplan.Ordinal==2){
					if(ordinal!=2) {
						continue;
					}
					ClaimProcs.ComputeBaseEst(claimProcs[i],proc,PlanCur,patplan.PatPlanNum,
						benefitList,histList,loopList,patPlans,paidOtherInsEstTotal,paidOtherInsBaseEst,patientAge,writeOffEstOtherIns,planList,listInsSubs,
						listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData:blueBookEstimateData);
				}
				else if(patplan.Ordinal==3) {
					if(ordinal!=3) {
						continue;
					}
					ClaimProcs.ComputeBaseEst(claimProcs[i],proc,PlanCur,patplan.PatPlanNum,
						benefitList,histList,loopList,patPlans,paidOtherInsEstTotal,paidOtherInsBaseEst,patientAge,writeOffEstOtherIns,planList,listInsSubs,
						listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData:blueBookEstimateData);
				}
				else{//patplan.Ordinal is 4 or greater.  Estimate won't be accurate if more than 4 insurances.
					if(ordinal!=4) {
						continue;
					}
					ClaimProcs.ComputeBaseEst(claimProcs[i],proc,PlanCur,patplan.PatPlanNum,
						benefitList,histList,loopList,patPlans,paidOtherInsEstTotal,paidOtherInsBaseEst,patientAge,writeOffEstOtherIns,planList,listInsSubs,
						listSubstLinks,useProcDateOnProc,lookupFees,blueBookEstimateData:blueBookEstimateData);
				}
				bool hasMetFrequencyLimit=HasMetFrequencyLimitation(claimProcs[i],histList,benefitList,proc,procCode,PlanCur,planList,patplan,loopList,listInsSubs:listInsSubs);
				//Only sync the ProcDate if attached to a claim.
				//We know that all procedures associated to a claim or multi-visit group are complete when a claim has been created.
				//Once attached, date cannot be edited from procedure edit window, unless user clicks "Edit Anyway".
				if(claimProcs[i].ClaimNum==0) {
					claimProcs[i].ProcDate=proc.ProcDate;
				}
				claimProcs[i].ClinicNum=proc.ClinicNum;
				//Wish we could do this, but it might change history.  It's needed when changing a completed proc to a different provider.
				//Can't do it here, though, because some people intentionally set provider different on claimprocs.
				//claimProcs[i].ProvNum=proc.ProvNum;
				if(isInitialEntry
					&&claimProcs[i].Status==ClaimProcStatus.CapEstimate
					&&proc.ProcStatus==ProcStat.C) 
				{
					claimProcs[i].Status=ClaimProcStatus.CapComplete;
					//Capitation procedures are not usually attached to a claim.
					//In order for Aging to calculate properly the ProcDate (Date Completed) and DateCP (Payment Date) must be the same.
					claimProcs[i].DateCP=proc.ProcDate;
				}
				//prov only updated if still an estimate
				if(claimProcs[i].Status==ClaimProcStatus.Estimate
					||claimProcs[i].Status==ClaimProcStatus.CapEstimate) {
					claimProcs[i].ProvNum=proc.ProvNum;
				}
				if(hasMetFrequencyLimit && hasEstimateCalculation) {
					claimProcs[i].BaseEst=0;
					claimProcs[i].InsEstTotal=0;
					claimProcs[i].DedEst=0;
					claimProcs[i].DedEstOverride=-1;
					claimProcs[i].DedApplied=0;
					if(claimProcs[i].EstimateNote!="") {
						claimProcs[i].EstimateNote+=", ";
					}
					claimProcs[i].EstimateNote+=Lans.g("Procedures","Frequency Limitation");
				}				
				#region Waiting Periods
				//Determine if there is a waiting period associated to this procedure, if there is and it falls within the bounds of the waiting
				//period zero out the claimproc. Otherwise, continue through
				//get all WaitingPeriod Benefits
				List<Benefit> listWaitBenefits=benefitList.FindAll(x => x.BenefitType==InsBenefitType.WaitingPeriod 
					&& (x.PlanNum==PlanCur.PlanNum || (patplan!=null && x.PatPlanNum==patplan.PatPlanNum))
					&& (procCode.CodeNum==x.CodeNum || CovSpans.IsCodeInSpans(procCode.ProcCode,CovSpans.GetForCat(x.CovCatNum))));
				//Check to see if we have any WaitingPeriod Benefits
				if(listWaitBenefits.Count>0 && hasEstimateCalculation) {
					//WaitingPeriods were found, loop through them to see if they apply
					bool hasProcCodeWaitPeriod = false;
					DateTime dateClaimProc=claimProcs[i].ProcDate;
					if(claimProcs[i].ProcDate<DateTime.Today) {
						//If claimproc.procdate is in the past then use today's date instead.
						dateClaimProc=DateTime.Today;
					}
					//first loop through to see if we have a matching proccode wait period. Apply it, then skip checking for category wait period
					foreach(Benefit ben in listWaitBenefits) {
						if(procCode.CodeNum == ben.CodeNum) {
							hasProcCodeWaitPeriod=true;
							DateTime dateCutoff=new DateTime();
							InsSub inssubCur=InsSubs.GetSub(claimProcs[i].InsSubNum,listInsSubs);
							if(ben.QuantityQualifier==BenefitQuantity.Months) {
								dateCutoff=inssubCur.DateEffective.AddMonths(ben.Quantity);
							}
							else if(ben.QuantityQualifier==BenefitQuantity.Years) {//Years
								dateCutoff=inssubCur.DateEffective.AddYears(ben.Quantity);
							}
							//If still within waiting period, zero out the claim and add a note
							if(dateClaimProc<dateCutoff.Date && dateClaimProc>=inssubCur.DateEffective.Date) {
								claimProcs[i].BaseEst=0;
								claimProcs[i].InsEstTotal=0;
								claimProcs[i].DedEst=0;
								claimProcs[i].DedEstOverride=-1;
								claimProcs[i].DedApplied=0;
								if(claimProcs[i].EstimateNote!="") {
									claimProcs[i].EstimateNote+=", ";
								}
								claimProcs[i].EstimateNote+=Lans.g("Procedures","Waiting Period");
								break;
							}
						}
					}
					if(!hasProcCodeWaitPeriod) {
						foreach(Benefit ben in listWaitBenefits) {
							DateTime dateCutoff=new DateTime();
							InsSub inssubCur=InsSubs.GetSub(claimProcs[i].InsSubNum,listInsSubs);
							if(ben.QuantityQualifier==BenefitQuantity.Months) {
								dateCutoff=inssubCur.DateEffective.AddMonths(ben.Quantity);
							}
							else if(ben.QuantityQualifier==BenefitQuantity.Years) {//Years
								dateCutoff=inssubCur.DateEffective.AddYears(ben.Quantity);
							}
							//If still within waiting period, zero out the claim and add a note
							if(dateClaimProc<dateCutoff.Date && dateClaimProc>=inssubCur.DateEffective.Date) {
								claimProcs[i].BaseEst=0;
								claimProcs[i].InsEstTotal=0;
								claimProcs[i].DedEst=0;
								claimProcs[i].DedEstOverride=-1;
								claimProcs[i].DedApplied=0;
								if(claimProcs[i].EstimateNote!="") {
									claimProcs[i].EstimateNote+=", ";
								}
								claimProcs[i].EstimateNote+=Lans.g("Procedures","Waiting Period");
								break;
							}
						}
					}
				}
				#endregion
				if(hasMetFrequencyLimit && InsPlans.DoZeroOutWriteOffOnOtherLimitation(PlanCur) && ClaimProcs.GetEstimatedStatuses().Contains(claimProcs[i].Status)) {
					claimProcs[i].WriteOffEst=0;
				}
				//If patplan.Ordinal is 1, 2, or 3 and the claim proc in question is not a preauth.
				//There is no such thing as having "paid by other ins" when dealing with preauths.
				if(patplan!=null && new[] { 1,2,3 }.Contains(patplan.Ordinal) && claimProcs[i].Status!=ClaimProcStatus.Preauth) {
					if(claimProcs[i].Status==ClaimProcStatus.Received || claimProcs[i].Status==ClaimProcStatus.Supplemental) {
						paidOtherInsEstTotal+=claimProcs[i].InsPayAmt;
						paidOtherInsBaseEst+=claimProcs[i].InsPayAmt;
						writeOffEstOtherIns+=claimProcs[i].WriteOff;
					}
					else {
						if(claimProcs[i].InsEstTotalOverride!=-1) {
							paidOtherInsEstTotal+=claimProcs[i].InsEstTotalOverride;
						}
						else {
							paidOtherInsEstTotal+=claimProcs[i].InsEstTotal;
						}
						paidOtherInsBaseEst+=claimProcs[i].BaseEst;
						writeOffEstOtherIns+=ClaimProcs.GetWriteOffEstimate(claimProcs[i]);
					}
				}
				//Calculations done, copy over estimates from InsEstTotal into InsPayEst.  
				//This was already done in ComputeBaseEst but frequencies could have changed it
				//This could potentially be limited to claimprocs status Recieved or NotReceived, but there likely is no harm in doing it for all claimprocs.
				if(!CompareDouble.IsEqual(claimProcs[i].InsEstTotalOverride,-1)) {
					claimProcs[i].InsPayEst=claimProcs[i].InsEstTotalOverride;
				}
				else {
					claimProcs[i].InsPayEst=claimProcs[i].InsEstTotal;
				}
				if(saveToDb) {
					//Only creates a log if blue book was used to calculate the estimate for this claimproc and the estimate has changed since last log.
					InsBlueBookLog blueBookLog=blueBookEstimateData.CreateInsBlueBookLog(claimProcs[i]);
					if(blueBookLog!=null) {
						InsBlueBookLogs.Insert(blueBookLog);
					}
					ClaimProcs.Update(claimProcs[i]);
				}
			}
		}

		///<summary>Returns true if a frequency benefit has been met this time period.</summary>
		public static bool HasMetFrequencyLimitation(ClaimProc claimProc,List<ClaimProcHist> histList,List<Benefit> benefitList,Procedure proc,
			ProcedureCode procCode,InsPlan planCur,List<InsPlan> listInsPlans,PatPlan patPlan=null,List<ClaimProcHist> loopList=null,List<InsSub> listInsSubs=null) 
		{
			//No need to check MiddleTierRole; no call to db.
			if(histList==null || benefitList==null || !PrefC.GetBool(PrefName.InsChecksFrequency) || proc.ProcDate.Year<1880 || claimProc.NoBillIns) {
				return false;
			}
			List<ClaimProcHist> listAll=new List<ClaimProcHist>(histList);
			if(loopList!=null) {
				listAll.AddRange(loopList);
			}
			//Procedures not billed to insurance do not affect frequency limitations, because the carrier is the entity which would enforce the limit.
			listAll=listAll.FindAll(x => !x.NoBillIns);
			//In case we are recalculating the estimate for a procedure already attached to a claim, we need to make sure the histList does not include
			//the claim proc we are currently recalculating.
			listAll=listAll.FindAll(x => x.ProcNum!=claimProc.ProcNum || x.ClaimNum!=claimProc.ClaimNum);
			//The frequency will be compared to the histList of ClaimProcs. Use it to check if any procedures have been completed for the patient within 
			//the frequency period. If this would put them over the limit then continue past the zero out the claimproc and continue. 
			//If no frequency information has been set then skip this logic for the current claimproc.
			//It is assumed for now that all benefits for a group will have the same quantity and quantityqualifier
			long patPlanNum=0;
			if(patPlan!=null) {
				patPlanNum=patPlan.PatPlanNum;
			}
			List<Benefit> listBenefitsLimitations=new List<Benefit>();
			for(int i=0;i<benefitList.Count;i++) {
				if(benefitList[i].PlanNum==0 && benefitList[i].PatPlanNum!=patPlanNum) {
					continue;
				}
				if(benefitList[i].PatPlanNum==0 && benefitList[i].PlanNum!=planCur.PlanNum) {
					continue;
				}
				if(benefitList[i].BenefitType==InsBenefitType.Limitations //BW, Pano/FW, Exam, and Custom category frequency limitations
					&& benefitList[i].MonetaryAmt==-1
					&& benefitList[i].Percent==-1
					&& (benefitList[i].QuantityQualifier==BenefitQuantity.Months
					|| benefitList[i].QuantityQualifier==BenefitQuantity.Years
					|| benefitList[i].QuantityQualifier==BenefitQuantity.NumberOfServices)) 
				{
					listBenefitsLimitations.Add(benefitList[i]);
				}
			}
			List<Benefit> listBensForGroup=GetAllLimitationsForGroups(listBenefitsLimitations,planCur,patPlanNum,procCode);
			ClaimProcStatus[] arrayClaimProcStatuses=new ClaimProcStatus[] {
				ClaimProcStatus.Received,ClaimProcStatus.InsHist,ClaimProcStatus.NotReceived,ClaimProcStatus.Estimate
			};
			//Filter the list of benefits down to the most pertinent benefit for each unique procedure code.
			listBensForGroup=listBensForGroup.GroupBy(x => x.CodeNum).Select(x => x.First()).ToList();
			foreach(Benefit ben in listBensForGroup) {
				if(ben.PlanNum!=planCur.PlanNum && ben.PatPlanNum!=patPlanNum && patPlanNum!=0) {
					continue;//Only look at frequency limitation benefits for the current plan, not all plans.
				}
				if(ben.QuantityQualifier==BenefitQuantity.Months || ben.QuantityQualifier==BenefitQuantity.Years) {
					DateTime datePast=new DateTime();
					if(ben.QuantityQualifier==BenefitQuantity.Months) {
						datePast=DateTimeOD.CalculateForEndOfMonthOffset(proc.ProcDate,ben.Quantity);
					}
					else {//Years
						datePast=DateTimeOD.CalculateForEndOfMonthOffset(proc.ProcDate,ben.Quantity*12);
					}
					//Find received or InsHist claimproc for the code/s wanted that's >= datePast
					string benCodeStr=ProcedureCodes.GetStringProcCode(ben.CodeNum);
					ClaimProcHist claimProcHist=listAll.Find(x => x.StrProcCode==benCodeStr 
						&& x.Status.In(arrayClaimProcStatuses)
						&& x.ProcDate>=datePast 
						&& x.PatNum==proc.PatNum
						&& x.InsSubNum==claimProc.InsSubNum
						&& IsSameProcedureArea(x.ToothNum,proc.ToothNum,x.ToothRange,proc.ToothRange,x.Surf,proc.Surf,procCode.TreatArea));
					if(claimProcHist!=null) {
						return true;
					}
				}
				if(ben.QuantityQualifier==BenefitQuantity.NumberOfServices) {
					DateTime datePast=new DateTime();
					List<ClaimProcHist> claimProcHistList=listAll.FindAll(x => x.PatNum==proc.PatNum
						&& x.InsSubNum==claimProc.InsSubNum
						&& listBensForGroup.Exists(y => ProcedureCodes.GetStringProcCode(y.CodeNum)==x.StrProcCode)
						&& IsSameProcedureArea(x.ToothNum,proc.ToothNum,x.ToothRange,proc.ToothRange,x.Surf,proc.Surf,procCode.TreatArea));
					if(ben.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
						datePast=DateTimeOD.CalculateForEndOfMonthOffset(proc.ProcDate,12);//Exactly 12 months in the past.
						claimProcHistList=claimProcHistList
							.Where(x => x.Status.In(arrayClaimProcStatuses)
							//For NumberInLast12Months, we do not include procedure exactly 1 year ago. This is why our lower bound is not inclusive.
							&& x.ProcDate.Between(datePast,datePast.AddYears(1),isLowerBoundInclusive:false)).ToList();
					}
					else {
						if(ben.TimePeriod==BenefitTimePeriod.CalendarYear) {
							datePast=new DateTime(proc.ProcDate.Year,1,1);
						}
						else if(ben.TimePeriod==BenefitTimePeriod.ServiceYear) {
							InsPlan insPlan=listInsPlans.Find(x => x.PlanNum==ben.PlanNum);
							//If benefit is an override, use PatPlanNum to get the PlanNum to match to.
							if(ben.PatPlanNum==patPlanNum && patPlanNum!=0 && !listInsSubs.IsNullOrEmpty()) {
								if(!listInsSubs.Exists(x => x.InsSubNum==patPlan.InsSubNum)) {
									continue;
								}
								InsSub insSub=listInsSubs.Find(x => x.InsSubNum==patPlan.InsSubNum);
								insPlan=listInsPlans.Find(x => x.PlanNum==insSub.PlanNum);
							}
							if(insPlan==null) {
								continue;
							}
							datePast=new DateTime(proc.ProcDate.Year,Math.Max(insPlan.MonthRenew,(byte)1),1);
							if(proc.ProcDate.Date<=datePast && datePast>DateTime.Today) {
								datePast=datePast.AddYears(-1);
							}
						}
						//We are assuming all benefits for the group have the same number of services.
						claimProcHistList=claimProcHistList.FindAll(x => x.Status.In(arrayClaimProcStatuses)
							&& x.ProcDate>=datePast 
							&& x.ProcDate<=datePast.AddYears(1));
					}
					//Sometimes, we look at supplemental payments. Also, if there happen to be two received claimprocs for the same procedure, we only
					//want to count these as one toward the frequency count if they are for the same procedure.
					if(claimProcHistList.DistinctBy(x => x.ProcNum).Count()>=ben.Quantity) {
						return true;
					}
				}
			}
			return false;
		}

		///<summary>Gets frequency limitation benefits and adds benefits for procedure codes in the same group.
		///E.g., if the benefit list includes a frequency limitation for D0274 (this code designates the bitewing group), this will include a benefit 
		///for D0274 and another benefit for D0272.</summary>
		///<param name="procCode">Include a procedure code to limit the benefits returned to only procedures in the group.</param>
		public static List<Benefit> GetAllLimitationsForGroups(List<Benefit> benefitList,InsPlan plan,long patPlanNum=0,ProcedureCode procCode=null) {
			List<Benefit> listBenefits=new List<Benefit>(benefitList).OrderByDescending(x => x.PatPlanNum).ToList();
			List<Benefit> listBensForGroup=new List<Benefit>();
			Benefit bwBenefit=listBenefits.Find(x => Benefits.IsBitewingFrequency(x,patPlanNum));
			Benefit panoBenefit=listBenefits.Find(x => Benefits.IsPanoFrequency(x,patPlanNum));
			Benefit examBenefit=listBenefits.Find(x => Benefits.IsExamFrequency(x,patPlanNum));
			listBensForGroup.AddRange(GetBenefitsForGroup(bwBenefit,
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenBWCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistBWCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(panoBenefit,
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenPanoCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistPanoCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(examBenefit,
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenExamCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistExamCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsCancerScreeningFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenCancerScreeningCodes),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsProphyFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenProphyCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistProphyCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsFlourideFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenFlourideCodes),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsSealantFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenSealantCodes),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsCrownFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenCrownCodes),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsSRPFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenSRPCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistPerioLLCodes))
					.Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistPerioLRCodes))
					.Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistPerioULCodes))
					.Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistPerioURCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsFullDebridementFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenFullDebridementCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenFullDebridementCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsPerioMaintFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenPerioMaintCodes).Union(ProcedureCodes.GetCodeNumsForPref(PrefName.InsHistPerioMaintCodes)).ToList(),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsDenturesFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenDenturesCodes),procCode));
			listBensForGroup.AddRange(GetBenefitsForGroup(listBenefits.Find(x => Benefits.IsImplantFrequency(x,patPlanNum)),
				ProcedureCodes.GetCodeNumsForPref(PrefName.InsBenImplantCodes),procCode));
			if(listBensForGroup.Count==0) {
				listBensForGroup=listBenefits.FindAll(x => x.BenefitType==InsBenefitType.Limitations
					&& (procCode==null || x.CodeNum==procCode.CodeNum)
					&& ((x.PatPlanNum==patPlanNum && patPlanNum!=0) || x.PlanNum==plan.PlanNum)
					&& (panoBenefit==null || x.BenefitNum!=panoBenefit.BenefitNum)
					&& (bwBenefit==null || x.BenefitNum!=bwBenefit.BenefitNum));//Takes care of Canadian codes (should only find one benefit generally)
			}
			if(listBensForGroup.Count==0) {//Benefit not found for above categories, look to see if it's covered by a span
				listBensForGroup=listBenefits.FindAll(x => x.BenefitType==InsBenefitType.Limitations
					&& x.CovCatNum!=0
					&& ((x.PatPlanNum==patPlanNum && patPlanNum!=0) || x.PlanNum==plan.PlanNum)
					&& (examBenefit==null || x.BenefitNum!=examBenefit.BenefitNum)
					&& CovSpans.IsCodeInSpans(procCode.ProcCode,CovSpans.GetForCat(x.CovCatNum)));
			}
			return listBensForGroup;
		}

		///<summary>Creates one benefit for each code num in the list if the given procedure is in the list of code nums.</summary>
		private static List<Benefit> GetBenefitsForGroup(Benefit ben,List<long> listCodeNums,ProcedureCode procCode) {
			//No need to check MiddleTierRole; no call to db.
			List<Benefit> listBensForGroup=new List<Benefit>();
			if((procCode==null ||  listCodeNums.Contains(procCode.CodeNum)) && ben!=null) {//The proc is not included or is part of the group
				foreach(long codeNum in listCodeNums) {
					Benefit benefit=ben.Copy();
					benefit.CodeNum=codeNum;
					listBensForGroup.Add(benefit);
				}
			}
			return listBensForGroup;
		}

		///<summary>This method is a helper for checking frequency limitations in ComputeForOrdinal(). The below booleans pain stakingly vet the passed in nums/ranges for handling empty strings.
		///Because we store toothrange,toothnum, and surf as strings they default to be empty which results in false positives in our logic. Checking for empty strings whilst doing our logic
		///in ComputeForOrdinal() results in a very ugly and unreadable linq statement. This method simply pulls out that logic and simplifies the linq statement above.</summary>
		public static bool IsSameProcedureArea(string histToothNum,string procCurToothNum,string histToothRangeStr,string procCurToothRangeStr,
			string histSurf,string procCurSurf,TreatmentArea procCurTreatArea) 
		{
			//Procedures like exams and BW's do not ever specify a toothnum, toothrange, or surface.
			if(string.IsNullOrEmpty(histToothNum) 
				&& string.IsNullOrEmpty(procCurToothNum)
				&& string.IsNullOrEmpty(histToothRangeStr)
				&& string.IsNullOrEmpty(procCurToothRangeStr) 
				&& string.IsNullOrEmpty(histSurf)
				&& string.IsNullOrEmpty(procCurSurf)) 
			{
				return true;
			}
			//No need to check MiddleTierRole; no call to db.
			string[] histToothRange=histToothRangeStr?.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)??Array.Empty<string>();
			string[] procCurToothRange=procCurToothRangeStr?.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)??Array.Empty<string>();
			bool hasToothRangeOverlap=histToothRange.Intersect(procCurToothRange).Count() > 0;
			bool hasCurToothInHistRange=histToothRange.Any(y => y!="" && y==procCurToothNum);
			bool hasHistToothInCurRange=!string.IsNullOrWhiteSpace(histToothNum) && procCurToothRange.Contains(histToothNum);
			bool hasSameToothNum=!string.IsNullOrWhiteSpace(procCurToothNum) && histToothNum==procCurToothNum;
			bool hasSameSurface=histSurf==procCurSurf;
			if(hasSameSurface
					&& (hasSameToothNum
						|| hasHistToothInCurRange
						|| hasCurToothInHistRange
						|| hasToothRangeOverlap
						|| procCurTreatArea.In(TreatmentArea.Arch,TreatmentArea.Quad,TreatmentArea.Sextant)))
			{
				return true;
			}
			return false;
		}

		///<summary>After changing important coverage plan info, this is called to recompute estimates for all procedures for this patient.</summary>
		public static void ComputeEstimatesForAll(long patNum,List<ClaimProc> claimProcs,List<Procedure> procs,List<InsPlan> planList,
			List<PatPlan> patPlans,List<Benefit> benefitList,int patientAge,List<InsSub> subList,List<ClaimProc> listClaimProcsAll=null,bool isClaimProcRemoveNeeded=false,
			List<SubstitutionLink> listSubstLinks=null,List<Fee> listFees=null) 
		{
			//No need to check MiddleTierRole; no call to db.
			//Get data for any OrthoCases that may be linked to procs in procs list
			List<long> listApptNums=null;
			if(PrefC.GetBool(PrefName.EnterpriseHygProcUsePriProvFee)) {
				listApptNums=new List<long>();
				for(int i=0;i<procs.Count;i++) {
					listApptNums.Add(procs[i].AptNum);
					listApptNums.Add(procs[i].PlannedAptNum);
				}
				listApptNums=listApptNums.FindAll(x=>x!=0).Distinct().ToList();
			}
			List<OrthoCase> listOrthoCases=OrthoCases.Refresh(patNum);
			List<OrthoProcLink> listOrthoProcLinksAllForPat=OrthoProcLinks.GetManyByOrthoCases(listOrthoCases.Select(x=>x.OrthoCaseNum).ToList());
			List<long> listProcNums=procs.Select(x=>x.ProcNum).ToList();
			List<OrthoProcLink> listOrthoProcLinks=listOrthoProcLinksAllForPat.FindAll(x=>listProcNums.Contains(x.ProcNum));
			List<OrthoSchedule> listOrthoSchedules=new List<OrthoSchedule>();
			if(listOrthoProcLinks.Count>0) {
				List<long> listSchedulePlanLinksFKey=OrthoPlanLinks.GetAllForOrthoCasesByType(listOrthoCases.Select(x=>x.OrthoCaseNum).ToList(),OrthoPlanLinkType.OrthoSchedule).Select(x=>x.FKey).ToList();
				listOrthoSchedules=OrthoSchedules.GetMany(listSchedulePlanLinksFKey);
			}
			BlueBookEstimateData blueBookEstimateData=new BlueBookEstimateData(planList,subList,patPlans,procs,listSubstLinks);
			for(int i=0;i<procs.Count;i++) {
				if(procs[i].ProcNumLab!=0){
					//Labs have their estimates calculated through their parents. Without this claimProcs have duplicated for a lab in the past.
					continue;
				}
				OrthoProcLink orthoProcLink=listOrthoProcLinks.Find(x=>x.ProcNum==procs[i].ProcNum);
				OrthoCase orthoCase=null;
				OrthoSchedule orthoSchedule=null;
				List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
				if(orthoProcLink!=null) {
					long orthoCaseNum=orthoProcLink.OrthoCaseNum;
					orthoCase=listOrthoCases.Find(x=>x.OrthoCaseNum==orthoCaseNum);
					orthoSchedule=listOrthoSchedules.Find(x=>x.OrthoScheduleNum==orthoCaseNum);
					listOrthoProcLinksForOrthoCase=listOrthoProcLinksAllForPat.FindAll(x=>x.OrthoCaseNum==orthoCaseNum);
				}

				ComputeEstimates(procs[i],patNum,ref claimProcs,false,planList,patPlans,benefitList,
					null,null,true,
					patientAge,subList,
					listClaimProcsAll,isClaimProcRemoveNeeded,false,listSubstLinks,false,
					listFees,null,
					orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCase,blueBookEstimateData,listApptNums:listApptNums);
			}
		}

		///<summary>Only called from one place in this same class.  Loops through list of procs for appt.  
		///Does not add notes to a procedure that already has notes. Only called from ProcedureL.SetCompleteInAppt, security checked
		///before calling this.  Also sets provider for each proc and claimproc.</summary>
		public static List<Procedure> SetCompleteInApptInList(Appointment apt,List<InsPlan> planList,List<PatPlan> patPlans,Patient patient,
			List<Procedure> listProcsForAppt,List<InsSub> subList,Userod curUser)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),apt,planList,patPlans,patient,listProcsForAppt,subList,curUser);
			}
			if(listProcsForAppt.Count==0) {
				return listProcsForAppt;//Nothing to do.
			}
			List<ClaimProc> claimProcList=ClaimProcs.Refresh(apt.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(apt.PatNum,benefitList,patPlans,planList,-1,apt.AptDateTime,subList);
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//most recent note will be first in list.
			string command="SELECT * FROM procnote "
				+"WHERE ProcNum IN("+string.Join(",",listProcsForAppt.Select(x => x.ProcNum))+") "
				+"ORDER BY EntryDateTime DESC";
			DataTable rawNotes=Db.GetTable(command);
			List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(planList);
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			for(int i=0;i<listProcsForAppt.Count;i++){
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcsForAppt[i].CodeNum);
				listProcedureCodes.Add(procedureCode);//null and dups ok
			}
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(patient.PatNum,apt.AptDateTime);
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,listProcsForAppt.Select(x=>x.MedicalCode).ToList(),
				listProcsForAppt.Select(x=>x.ProvNum).ToList(),patient.PriProv,patient.SecProv,patient.FeeSched,planList,
				listProcsForAppt.Select(x=>x.ClinicNum).ToList(),new List<Appointment>(){apt },listSubstLinks,discountPlanNum);
			ProcedureCode procCode;
			Procedure procOld;
			List<long> encounterProvNums=new List<long>();//for auto-inserting default encounters
			//Get a list of any claimnums associated to procedures in the appoitments
			List<long> listClaimNumsForApptProcs=claimProcList.Where(x => x.ClaimNum>0 && listProcsForAppt.Select(y => y.ProcNum).Contains(x.ProcNum))
				.Select(x => x.ClaimNum).ToList();
			List<ClaimProcHist> histListForApptProcs=histList.Where(x => listClaimNumsForApptProcs.Contains(x.ClaimNum)).ToList();
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(patient.PatNum);
			BlueBookEstimateData blueBookEstimateData=new BlueBookEstimateData(planList,subList,patPlans,listProcsForAppt,listSubstLinks);
			foreach(Procedure procCur in listProcsForAppt) {
				//Should only be procs for this appointment
				//attach the note, if it exists.
				foreach(DataRow row in rawNotes.Rows) {
					if(procCur.ProcNum.ToString()!=row["ProcNum"].ToString()) {
						continue;
					}
					procCur.UserNum=PIn.Long(row["UserNum"].ToString());
					procCur.Note=PIn.String(row["Note"].ToString());
					procCur.SigIsTopaz=PIn.Bool(row["SigIsTopaz"].ToString());
					procCur.Signature=PIn.String(row["Signature"].ToString());
					break;//out of note loop.
				}
				procOld=procCur.Copy();
				procCode=ProcedureCodes.GetProcCode(procCur.CodeNum);
				if(procCode.PaintType==ToothPaintingType.Extraction) {//if an extraction, then mark previous procs hidden
					//SetHideGraphical(procCur);//might not matter anymore
					ToothInitials.SetValue(apt.PatNum,procCur.ToothNum,ToothInitialType.Missing);
				}
				procCur.ProcStatus=ProcStat.C;
				if(procOld.ProcStatus!=ProcStat.C) {
					procCur.ProcDate=apt.AptDateTime.Date;//only change date to match appt if not already complete.
					if(procCur.ProcDate.Year<1880) {
						procCur.ProcDate=MiscData.GetNowDateTime().Date;//Change procdate to today if the appointment date was invalid
					}
					procCur.DateEntryC=DateTime.Now;//this triggers it to set to server time NOW().
					if(procCur.DiagnosticCode=="") {
						procCur.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
						procCur.IcdVersion=PrefC.GetByte(PrefName.DxIcdVersion);
					}
				}
				procCur.ClinicNum=apt.ClinicNum;
				procCur.SiteNum=patient.SiteNum;
				procCur.PlaceService=Clinics.GetPlaceService(procCur.ClinicNum);
				procCur.ProvNum=GetProvNumFromAppointment(apt,procCur,procCode);
				//if procedure was already complete, then don't add more notes.
				if(procOld.ProcStatus!=ProcStat.C) {
					string procNoteDefault = ProcCodeNotes.GetNote(procCur.ProvNum,procCur.CodeNum,procCur.ProcStatus);
					if(procCur.Note!="" && procNoteDefault!="") {
						procCur.Note+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
					}
					procCur.Note+=procNoteDefault;
				}
				if(Userods.IsUserCpoe(curUser)) {
					//Only change the status of IsCpoe to true.  Never set it back to false for any reason.  Once true, always true.
					procCur.IsCpoe=true;
				}
				OrthoProcLink orthoProcLink=OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,procCur);
				SetOrthoProcComplete(procCur,procCode);
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada
					SetCanadianLabFeesCompleteForProc(procCur);
				}
				Plugins.HookAddCode(null,"Procedures.SetCompleteInAppt_procLoop",procCur,procOld);
				bool isProcLinkedToOrthoCase=orthoProcLink!=null;
				if(procCur.ProcNumLab==0) {//skip because SetCanadianLabFeesCompleteForProc() called update already
					Update(procCur,procOld,isProcLinkedToOrthoCase:isProcLinkedToOrthoCase);//Updates payplan charges for the procedure if it went from any status to complete.
				}
				if(isProcLinkedToOrthoCase) {//If proc was linked to orthoCase, Pass ortho case objects to ComputeEstimates.
					ComputeEstimates(procCur,apt.PatNum,ref claimProcList,false,planList,patPlans,benefitList,
						histList,loopList,true,
						patient.Age,subList,
						null,false,false,listSubstLinks,false,
						listFees,null,
						orthoProcLink,orthoCaseProcLinkingData.ActiveOrthoCase,orthoCaseProcLinkingData.OrthoSchedule,orthoCaseProcLinkingData.ListProcLinksForCase,blueBookEstimateData);
				}
				else {//Otherwise, call without orthocase objects.
					ComputeEstimates(procCur,apt.PatNum,ref claimProcList,false,planList,patPlans,benefitList,
						histList,loopList,true,
						patient.Age,subList,
						null,false,false,listSubstLinks,false,
						listFees,null,blueBookEstimateData:blueBookEstimateData);
				}
				List<ClaimProcHist> listClaimProcHistToAdd=ClaimProcs.GetHistForProc(claimProcList,procCur,procCur.CodeNum);
				loopList.AddRange(listClaimProcHistToAdd);
				//Remove any from the histList that were just added to the loopList. This is needed so we don't count the estimated amounts twice (histList and loopList)
				histList.RemoveAll(x => x.ProcNum==procCur.ProcNum
					&& listClaimProcHistToAdd.Any(y => y.PatNum==x.PatNum && y.PlanNum==x.PlanNum && y.InsSubNum==x.InsSubNum));
				ClaimProcs.SetProvForProc(procCur,claimProcList);
				//Add provnum to list to create an encounter later. Done to limit calls to DB from Encounters.InsertDefaultEncounter().
				if(procOld.ProcStatus!=ProcStat.C) {//check for distinct later.
					encounterProvNums.Add(procCur.ProvNum);
				}
			}
			//Auto-insert default encounters for the providers that did work on this appointment
			encounterProvNums.Distinct().ToList().ForEach(x => Encounters.InsertDefaultEncounter(apt.PatNum,x,apt.AptDateTime));
			Recalls.Synch(apt.PatNum);
			return listProcsForAppt;
			//Patient pt=Patients.GetPat(apt.PatNum);
			//jsparks-See notes within this method:
			//Reporting.Allocators.AllocatorCollection.CallAll_Allocators(pt.Guarantor);
		}

		///<summary>Both listDiagCodes and listDiagVersions must be the same length and not null.</summary>
		private static bool ExistsDiagnosticCode(List<string> listDiagCodes,List<byte> listDiagVersions,string diagnosticCode,byte diagnosticVersion)	{
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<listDiagCodes.Count;i++) {
				if(listDiagCodes[i]==diagnosticCode && listDiagVersions[i]==diagnosticVersion) {
					return true;
				}
			}
			return false;
		}

		///<summary>Gets the placement date for ortho patients.
		///Takes the patient's patientNote.DateOrthoPlacementOverride and preference.OrthoPlacementProcsList into account.
		///Uses the first D8* procedure if neither of the above are found.  Returns DateTime.MinValue if no proc found.</summary>
		public static DateTime GetFirstOrthoProcDate(PatientNote patNoteCur) {
			//No need to check MiddleTierRole; no call to db.
			if(patNoteCur.DateOrthoPlacementOverride != DateTime.MinValue) {
				//if an override is set, use that.
				return patNoteCur.DateOrthoPlacementOverride;
			}
			DateTime firstOrthoProcDate = DateTime.MinValue;
			List<long> listOrthoProcNums = ProcedureCodes.GetOrthoBandingCodeNums();
			//otherwise, use the proc of one of the codes specified in the pref.
			Procedure proc=Procedures.GetProcsByStatusForPat(patNoteCur.PatNum,ProcStat.C)
				.Where(x => listOrthoProcNums.Contains(x.CodeNum))
				.OrderBy(x => x.ProcDate)//Earliest ortho placement first.
				.FirstOrDefault();
			if(proc!=null) {
				firstOrthoProcDate=proc.ProcDate;
			}
			return firstOrthoProcDate;
		}

		///<summary>Does nothing if not an ortho proc (dictated by the pref OrthoPlacementProcsList or a code starting with D8 if pref not set).
		///PatPlan table needs to be updated when an ortho placement procedure is set complete.
		///Only updates the date if no OrthoAutoNextClaimDate is set on the corresponding PatPlan. Updates PatientNote.OrthoMonthsTreatOverride
		///if this is the first Ortho placement proc set complete.</summary>
		public static void SetOrthoProcComplete(Procedure procCur,ProcedureCode procCode) {
			if(procCode == null || procCur == null) { //this should never happen unless they have some corruption
				return;
			}
			if(PrefC.GetString(PrefName.OrthoDebondCodes).Contains(procCode.ProcCode) && PrefC.GetBool(PrefName.OrthoDebondProcCompletedSetsMonthsTreat)) {
				SetAutoOrthoMonthsTreat(procCur,procCode.ProcCode);//Check if completed code is debond and needs to update Months Treatment.
				return;
			}
			List<long> listOrthoPlacementCodeNums = ProcedureCodes.GetOrthoBandingCodeNums();
			if(listOrthoPlacementCodeNums.Count > 0) {
				if(!listOrthoPlacementCodeNums.Contains(procCode.CodeNum)) {
					return;
				}
			}
			else if(!procCode.ProcCode.StartsWith("D8080") && !procCode.ProcCode.StartsWith("D8090")) {
				return;
			}
			List<PatPlan> listPatPlans = PatPlans.GetPatPlansForPat(procCur.PatNum);
			foreach(PatPlan patPlanCur in listPatPlans) {
				if(patPlanCur.OrthoAutoNextClaimDate.Date != DateTime.MinValue.Date) {
					continue;
				}
				InsPlan insPlanCur = InsPlans.GetByInsSubs(new List<long> { patPlanCur.InsSubNum }).FirstOrDefault();
				if(insPlanCur == null || insPlanCur.OrthoType != OrthoClaimType.InitialPlusPeriodic) {
					continue;
				}
				TimeSpan waitDays = TimeSpan.FromDays(0);
				waitDays = TimeSpan.FromDays(insPlanCur.OrthoAutoClaimDaysWait);
				DateTime procWaitDays = procCur.ProcDate + waitDays;
				procWaitDays=procWaitDays.AddMonths(1);//Always push the next claim date out a month, even after the "wait days" preference.
				patPlanCur.OrthoAutoNextClaimDate = new DateTime(procWaitDays.Year,procWaitDays.Month,1);
				PatPlans.Update(patPlanCur);
			}
			if(procCur.ProcStatus==ProcStat.C) {//Only make this change if Complete procedure.(Currently, will always be true, but for safety).
				Patient patCur=Patients.GetLim(procCur.PatNum);//GetLim because we just need pat.Guarantor.
				PatientNote patNoteCur=PatientNotes.Refresh(patCur.PatNum,patCur.Guarantor);//Inserts PatientNote rows if one does not exists for PatNum AND Guarantor.
				//First time completing an Ortho placement procedure, so we don't have an override in place yet. Any subsequent Ortho procs will use the same
				//override as the first Ortho proc.
				Byte defaultMonths=PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat);
				//Only set the override if one has not already been set.
				if(patNoteCur.OrthoMonthsTreatOverride==-1) {
					//Set OrthoMonthsTreatOverride to PrefName.OrthoDefaultMonthsTreat, so we don't overwrite it if the practice default changes later.
					patNoteCur.OrthoMonthsTreatOverride=defaultMonths;//Use current practice default.
					PatientNotes.Update(patNoteCur,patCur.Guarantor);
				}
			}
		}

		///<summary>This method will set their auto proc's Months Treatment to the number of months from the start date to the completed procedure.</summary>
		private static void SetAutoOrthoMonthsTreat(Procedure procCur,string procCode) {
			Patient patCur=Patients.GetLim(procCur.PatNum);
			PatientNote patNoteCur=PatientNotes.Refresh(patCur.PatNum,patCur.Guarantor);
			DateTime firstOrthoProcDate=GetFirstOrthoProcDate(patNoteCur);
			DateSpan dateSpan=new DateSpan(firstOrthoProcDate,procCur.ProcDate);
			int totalMonthsDiff=(dateSpan.YearsDiff*12)+dateSpan.MonthsDiff+(dateSpan.DaysDiff<15?0:1);
			patNoteCur.OrthoMonthsTreatOverride=totalMonthsDiff; //Setting the patient notes OrthoMonthsTreatOverride will set the Tx Total Months.
			PatientNotes.Update(patNoteCur,patCur.Guarantor);
		}

		///<summary>Decides the base description to send to insurance based off of the claimproc.
		///This function mimics logic from ContrAccount.CreateClaim() when setting CodeSent.
		///If the logic in this function changes, then consider changing ContrAccount.CreateClaim() as well.</summary>
		public static string GetClaimDescript(ClaimProc claimProcCur,ProcedureCode procCodeSent,Procedure procCur,ProcedureCode procCodeCur,InsPlan planCur=null) {
			//No need to check MiddleTierRole; no call to db.
			string descript=procCodeSent.Descript;
			if(PrefC.GetBool(PrefName.ClaimPrintProcChartedDesc)) {
				if(planCur==null) {
					planCur=InsPlans.GetPlan(claimProcCur.PlanNum,null);
				}
				//If the proccode was not overridden by a alternate code or a medical code,
				//then use the orignal procedure code description instead of the codesent description.
				if((procCodeCur.AlternateCode1=="" || !planCur.UseAltCode) 
					&& (procCodeCur.MedicalCode=="" || !planCur.IsMedical))
				{
					descript=procCodeCur.Descript;
				}
			}
			return descript;
		}

		///<summary>Sets either the AptNum or Planned AptNum for given procs.
		///Uses listSelectedRows and listProcNumsAttachedStart to determine if procs are attaching to or detaching from AptCur.
		///When moving proc from another appt, other appt descriptions are updated.</summary>
		public static void ProcsAptNumHelper(List<Procedure> listProcs,Appointment AptCur,List<Appointment> listAppointments,
			List<int> listSelectedRows,List<long> listProcNumsAttachedStart,bool isAptPlanned=false,LogSources logSource=LogSources.None)
		{
			//No need to check MiddleTierRole; no call to db.
			if(listProcs==null || AptCur==null || listAppointments==null || listSelectedRows==null || listProcNumsAttachedStart==null) {
				return;
			}
			for(int i=0;i<listProcs.Count;i++) {
				Procedure proc=listProcs[i];
				Procedure procOld=proc.Copy();
				bool isAttaching=listSelectedRows.Contains(i);
				bool isDetaching=(!isAttaching);
				bool isAttachedStart=listProcNumsAttachedStart.Contains(proc.ProcNum);
				bool isDetachedStart=(!isAttachedStart);
				if(isDetaching && isAptPlanned && proc.PlannedAptNum==AptCur.AptNum) {//Detatching from this planned appointment.
					proc.PlannedAptNum=0;
				}
				else if(isDetaching && !isAptPlanned && proc.AptNum==AptCur.AptNum) {//Detatching from this appointment.
					proc.AptNum=0;
				}
				else if(isDetachedStart && isAttaching && isAptPlanned) {//Attaching to this planned appointment.
					if(proc.PlannedAptNum!=0 && proc.PlannedAptNum != AptCur.AptNum) {//Currently attached to another planned appointment.
						Appointment apptOldPlanned=listAppointments.FirstOrDefault(x => x.AptNum==proc.PlannedAptNum && x.AptStatus==ApptStatus.Planned);
						string apptOldPlannedDateStr=(apptOldPlanned==null ? "[INVALID #"+proc.PlannedAptNum+"]" : apptOldPlanned.AptDateTime.ToShortDateString());
						//Add securityLog to planned appointment.
						SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,AptCur.PatNum,Lans.g("AppointmentEdit","Procedure")+" "
							+ProcedureCodes.GetProcCode(proc.CodeNum).AbbrDesc+" "+Lans.g("AppointmentEdit","moved from planned appointment created on")+" "
							+apptOldPlannedDateStr+" "+Lans.g("AppointmentEdit","to planned appointment created on")+" "
							+AptCur.AptDateTime.ToShortDateString(),AptCur.AptNum,logSource,AptCur.DateTStamp);
						//Add securityLog to previously planned appointment.
						SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,apptOldPlanned?.PatNum??AptCur.PatNum,Lans.g("AppointmentEdit","Procedure")+" "
							+ProcedureCodes.GetProcCode(proc.CodeNum).AbbrDesc+" "+Lans.g("AppointmentEdit","moved from planned appointment created on")+" "
							+apptOldPlannedDateStr+" "+Lans.g("AppointmentEdit","to planned appointment created on")+" "
							+apptOldPlannedDateStr,apptOldPlanned?.AptNum??0,logSource,apptOldPlanned?.DateTStamp??default);
						UpdateOtherApptDesc(proc,AptCur,isAptPlanned,listAppointments,listProcs);
					}
					proc.PlannedAptNum=AptCur.AptNum;
				}
				else if(isDetachedStart && isAttaching && !isAptPlanned) {//Attaching to this appointment.
					if(proc.AptNum!=0 && proc.AptNum != AptCur.AptNum) {//Currently attached to another appointment.
						Appointment apptOld=listAppointments.FirstOrDefault(x => x.AptNum==proc.AptNum);
						string apptOldDateStr=(apptOld==null ? "[INVALID #"+proc.AptNum+"]" : apptOld.AptDateTime.ToShortDateString());
						//Add securityLog to appointment.
						SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,AptCur.PatNum,Lans.g("AppointmentEdit","Procedure")+" "
							+ProcedureCodes.GetProcCode(proc.CodeNum).AbbrDesc+" "+Lans.g("AppointmentEdit","moved from appointment on")+" "+apptOldDateStr
							+" "+Lans.g("AppointmentEdit","to appointment on")+" "+AptCur.AptDateTime,AptCur.AptNum,logSource,AptCur.DateTStamp);
						//Add securityLog to previous appointment.
						SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,apptOld?.PatNum??AptCur.PatNum,Lans.g("AppointmentEdit","Procedure")+" "
							+ProcedureCodes.GetProcCode(proc.CodeNum).AbbrDesc+" "+Lans.g("AppointmentEdit","moved from appointment on")+" "+apptOldDateStr
							+" "+Lans.g("AppointmentEdit","to appointment on")+" "+apptOldDateStr,apptOld?.AptNum??0,logSource,apptOld?.DateTStamp??default);
						UpdateOtherApptDesc(proc,AptCur,isAptPlanned,listAppointments,listProcs);
					}
					proc.AptNum=AptCur.AptNum;
				}
				else {
					continue;//No changes were made to the current procedure.
				}
				Procedures.Update(proc,procOld);//Update above changes to db.
			}
		}

		///<summary>Updates the procs description and AptNum of the both the old appointment and appt.
		///listAppts should contain the procs previous appt.
		///ListProcsForAppt and listProcCodes can be null.</summary>
		public static void UpdateOtherApptDesc(Procedure proc,Appointment appt,bool isApptPlanned,List<Appointment> listAppts,List<Procedure> listProcsForAppt) {
			Appointment apptPrevious;
			if(isApptPlanned) {
				apptPrevious=listAppts.FirstOrDefault(x => x.AptNum==proc.PlannedAptNum);
				proc.PlannedAptNum=appt.AptNum;
			}
			else {
				apptPrevious=listAppts.FirstOrDefault(x => x.AptNum==proc.AptNum);
				proc.AptNum=appt.AptNum;
			}
			if(apptPrevious!=null) {
				//apptPrevious gets updated in memory which causes listAppts to contain the changes.
				Appointments.SetProcDescript(apptPrevious,listProcsForAppt);
			}
		}

		///<summary>Creates securitylog entry for a completed procedure.  Set toothNum to empty string and it will be omitted from the log entry. toothNums can be null or empty.</summary>
		public static void LogProcComplCreate(long patNum,Procedure procCur,string toothNums) {
			//No need to check MiddleTierRole; no call to db.
			if(procCur==null) {
				return;//Nothing to do.  Should never happen.
			}
			ProcedureCode procCode=ProcedureCodes.GetProcCode(procCur.CodeNum);
			string logText=procCode.ProcCode+", ";
			if(toothNums!=null && toothNums.Trim()!="") {
				logText+=Lans.g("Procedures","Teeth")+": "+toothNums+", ";
			}
			logText+=Lans.g("Procedures","Fee")+": "+procCur.ProcFee.ToString("F")+", "+procCode.Descript;
			SecurityLogs.MakeLogEntry(Permissions.ProcComplCreate,patNum,logText);
		}

		///<summary>Creates securitylog entry for completed procedure where appointment ProvNum is different than the procedures provnum.</summary>
		private static void LogProcComplEdit(Procedure proc,Procedure procOld,List<ProcedureCode> listProcedureCodes=null) {
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum,listProcedureCodes);
			string logText=Lans.g("Procedures","Completed procedure")+" "+procCode.ProcCode.ToString()+" "
				+Lans.g("Procedures","edited by setting appointment complete.");
			if(proc.ProvNum!=procOld.ProvNum) {
				logText+=" "+Lans.g("Procedures","Provider was changed from")+" "+Providers.GetAbbr(procOld.ProvNum)+" "+Lans.g("Procedures","to")+" "+
					Providers.GetAbbr(proc.ProvNum)+".";
			}
			SecurityLogs.MakeLogEntry(Permissions.ProcCompleteEdit,proc.PatNum,logText,proc.ProcNum,LogSources.None,procOld.DateTStamp);
		}

		///<summary>Sets all procedures for apt complete.  Flags procedures as CPOE as needed (when prov logged in).  Makes a log entry for each completed proc.</summary>
		public static List<Procedure> SetCompleteInAppt(Appointment apt,List<InsPlan> PlanList,List<PatPlan> patPlans,Patient patient,List<InsSub> subList,
			bool removeCompletedProcs) 
		{
			//Get all procs attached to the appointment and go through the set complete logic.
			//We must go through all procedures. Remove completed procs if removeCompletedProcs is set to true. We don't want to change completed procedures 
			//unless user wants/has permissions.The permission check should be done before calling this method.
			List<Procedure> listProcsInAppt=GetProcsForSingle(apt.AptNum,false);
			if(removeCompletedProcs) {
				//Remove already completed procedures.
				listProcsInAppt.RemoveAll(x => x.ProcStatus==ProcStat.C);
			}
			if(listProcsInAppt.Count==0) {
				return listProcsInAppt;//Nothing to do.
			}
			List<Procedure> listProcsOld=listProcsInAppt.Select(x => x.Copy()).ToList();
			listProcsInAppt=SetCompleteInApptInList(apt,PlanList,patPlans,patient,listProcsInAppt,subList,Security.CurUser);
			List<Procedure> listProcsCompleted=listProcsInAppt.FindAll(x => listProcsOld.Any(y => y.ProcNum==x.ProcNum && y.ProcStatus!=ProcStat.C));
			listProcsCompleted.ForEach(x => LogProcComplCreate(apt.PatNum,x,x.ToothNum));
			List<Procedure> listProcsAlreadyComplete=listProcsInAppt.FindAll(x => listProcsOld.Any(y => y.ProcNum==x.ProcNum && y.ProcStatus==ProcStat.C));
			foreach(Procedure proc in listProcsAlreadyComplete) {
				Procedure procOld=listProcsOld.FirstOrDefault(x => x.ProcNum==proc.ProcNum);
				LogProcComplEdit(proc,procOld);
			}
			if(Programs.UsingOrion) {
				OrionProcs.SetCompleteInAppt(listProcsInAppt);
			}
			return listProcsInAppt;
		}

		///<summary>Constructs a procedure from a passed-in codenum. Does not prompt you to fill in info like toothNum, etc. 
		///Does NOT insert the procedure into the DB, just returns it.</summary>
		public static Procedure ConstructProcedureForAppt(long codeNum,Appointment appt,Patient pat,List<PatPlan> listPatPlans,
			List<InsPlan> listInsPlans,List<InsSub> listInsSubs,List<Fee> listFees=null)
		{
			//No need to check MiddleTierRole; no call to db.
			Procedure proc=new Procedure();
			proc.CodeNum=codeNum;
			proc.PatNum=appt.PatNum;
			proc.ProcDate=DateTime.Today;
			proc.DateTP=proc.ProcDate;
			proc.ToothRange="";
			//surf
			proc.Priority=0;
			proc.ProcStatus=ProcStat.TP;
			ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(proc.CodeNum);
			#region ProvNum
			proc.ProvNum=appt.ProvNum;
			if(procCodeCur.ProvNumDefault!=0) {//Override provider for procedures with a default provider
				//This provider might be restricted to a different clinic than this user.
				proc.ProvNum=procCodeCur.ProvNumDefault;
			}
			else if(procCodeCur.IsHygiene && appt.ProvHyg!=0) {
				proc.ProvNum=appt.ProvHyg;
			}
			#endregion ProvNum
			proc.ClinicNum=appt.ClinicNum;
			proc.MedicalCode=procCodeCur.MedicalCode;
			proc.ProcFee=Procedures.GetProcFee(pat,listPatPlans,listInsSubs,listInsPlans,proc.CodeNum,proc.ProvNum,proc.ClinicNum,proc.MedicalCode,listFees:listFees);
			proc.Note=ProcCodeNotes.GetNote(proc.ProvNum,proc.CodeNum,proc.ProcStatus); //get the TP note.
			//dx
			//nextaptnum
			proc.DateEntryC=DateTime.Now;
			proc.BaseUnits=procCodeCur.BaseUnits;
			proc.SiteNum=pat.SiteNum;
			proc.RevCode=procCodeCur.RevenueCodeDefault;
			proc.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
			proc.PlaceService=Clinics.GetPlaceService(proc.ClinicNum); 
			if(Userods.IsUserCpoe(Security.CurUser)) {
				//This procedure is considered CPOE because the provider is the one that has added it.
				proc.IsCpoe=true;
			}
			return proc;
		}

		///<summary>Procedure comparer to sort procedures specifically like how they display in the Account Module.  DataRows should be sorted via ProcDate first.</summary>
		private static int ProcedureComparer(DataRow x,DataRow y) {
			return ProcedureLogic.CompareProcedures(x,y);
		}

		///<summary>Returns true if the procedure's ProcCode is a banding, debond, or visit code</summary>
		public static bool HasAnOrthoCaseProcCode(Procedure proc) {
			//No remoting role check; no call to db
			List<string>orthoProcCodes=OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoBandingCodes);
			orthoProcCodes.AddRange(OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoVisitCodes));
			orthoProcCodes.AddRange(OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoDebondCodes));
			string procCode=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
			if(orthoProcCodes.Contains(procCode)) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if given values are valid for editing a procedure, otherwisw false.
		///Values passed in by ref can be modified in this method.
		///Set actionOnFailure to handle a message that explains the reasons that this method returned false. Error messages are pre-translated.
		///Set funcYesNoPrompt to handle situations where the user needs to confirm or block behavior. Message prompts are pre-translated.
		///Set actionOnProcedureCodeFailure to handle when ProcedureCodes.GetStringProcCode(...) throws an exception.</summary>
		public static bool EntriesAreValid(string textNotes,bool isSigChangedAndNotBlank,string textTimeStart,string textTimeEnd,int unityQty,long provNumSelected,
			string textMedicalCode,bool isTextDrugNdcNotBlank,string textDrugQty,ref DateTime textDate,bool isNew,double procFee,bool isQuickAdd,
			bool isCheckTypeCodeNonXChecked,bool isCheckTypeCodeXChecked,int listProsthSelectedIndex,bool isQuadrantSelected,
			bool hasOrthoProcLink,bool isTextDateOriginalProsthBlank,int comboDrugUnitSelectedIndex,
			string textSurfaces,string textTooth,bool hasSextantSelection,bool isArchSelected,
			Procedure procedure,Procedure procedureOld,ref ProcedureCode procedureCode,string translationSource,Userod user,
			Action<string> actionOnFailure,Func<string,bool> funcYesNoPrompt,Action actionOnProcedureCodeFailure,
			ref List<ClaimProc> listClaimProcsForProc)
		{
			//No need to check MiddleTierRole; no call to db.
			#region Note
			bool hasAutoNotePrompt=Regex.IsMatch(textNotes,AutoNotePromptRegex);
			//If ProcNoteSigsBlockedAutoNoteIncomplete is true, do not allow the user to save a changed signature if there are still autonote prompts.
			if(isSigChangedAndNotBlank && hasAutoNotePrompt && PrefC.GetBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete)) {
				actionOnFailure.Invoke(Lans.g(translationSource,"Remaining auto note prompts must be completed to sign this note. Use Edit Auto Note to resume."));
				return false;
			}
			//There have been 2 or 3 cases where a customer entered a note with thousands of new lines and when OD tries to display such a note in the chart, a GDI exception occurs because the progress notes grid is very tall and takes up too much video memory. To help prevent this issue, we block the user from entering any note where there are 50 or more consecutive new lines anywhere in the note. Any number of new lines less than 50 are considered to be intentional.
			StringBuilder tooManyNewLines=new StringBuilder();
			for(int i=0;i<50;i++) {
				tooManyNewLines.Append("\r\n");
			}
			if(textNotes.Contains(tooManyNewLines.ToString())) {
				actionOnFailure.Invoke(Lans.g(translationSource,"The notes contain 50 or more consecutive blank lines. Probably unintentional and must be fixed."));
				return false;
			}
			#endregion
			#region textTimeStart, textTimeEnd validation
			if(!AreTimesValid(textTimeStart,textTimeEnd,actionOnFailure)) {
				return false;
			}
			#endregion
			#region textUnitQty validation
			if(!IsQuantityValid(unityQty,actionOnFailure)) {
				return false;
			}
			#endregion
			#region Provider UI
			if(provNumSelected==0){
				actionOnFailure.Invoke(Lans.g(translationSource,"You must select a provider first."));
				return false;
			}
			#endregion
			if(procedureCode.TreatArea==TreatmentArea.Surf && (textSurfaces=="" || !ValidateToothValue(textTooth,ref textSurfaces))) {
				actionOnFailure.Invoke(Lans.g(translationSource,"Please fix tooth number or surfaces first."));
				return false;
			}
			if(procedureCode.TreatArea==TreatmentArea.Sextant && !ValidateSextant(hasSextantSelection)) {
				actionOnFailure.Invoke(Lans.g(translationSource,"Please fix sextant first."));
				return false;
			}
			if(procedureCode.TreatArea==TreatmentArea.Arch && !ValidateArch(isArchSelected)) {
				actionOnFailure.Invoke(Lans.g(translationSource, "Please fix arch first."));
				return false;
			}
			if(procedureCode.TreatArea==TreatmentArea.Tooth && !Tooth.IsValidEntry(textTooth)) {
				actionOnFailure.Invoke(Lans.g(translationSource, "Please fix tooth number first."));
				return false;
			}
			#region Medical Code
			if(textMedicalCode!="" && !ProcedureCodes.GetContainsKey(textMedicalCode)){
				actionOnFailure.Invoke(Lans.g(translationSource,"Invalid medical code.  It must refer to an existing procedure code."));
				return false;
			}
			#endregion
			#region Drug UI
			if(isTextDrugNdcNotBlank){
				if(comboDrugUnitSelectedIndex==(int)EnumProcDrugUnit.None || textDrugQty==""){
					if(!funcYesNoPrompt.Invoke(Lans.g(translationSource,"Drug quantity and unit are not entered.  Continue anyway?"))){
						return false;
					}
				}
			}
			if(textDrugQty!=""){
				try{
					float.Parse(textDrugQty);
				}
				catch{
					actionOnFailure.Invoke(Lans.g(translationSource,"Please fix drug qty first."));
					return false;
				}
			}
			#endregion
			#region Procedure Status
			//If user is trying to change status to complete and using eCW.
			if(procedure.ProcStatus==ProcStat.C && (isNew || procedureOld.ProcStatus!=ProcStat.C) && Programs.UsingEcwTightOrFullMode()) {
				actionOnFailure.Invoke(Lans.g(translationSource,"Procedures cannot be set complete in this window.  Set the procedure complete by setting the appointment complete."));
				return false;
			}
			if(procedure.ProcStatus==ProcStat.C && textDate.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				actionOnFailure.Invoke(Lans.g(translationSource,"Completed procedures cannot have future dates."));
				return false;
			}
			if(procedureOld.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C){//if status was changed to complete
				if(procedure.AptNum!=0) {//if attached to an appointment
					Appointment apt=Appointments.GetOneApt(procedure.AptNum);
					if(apt.AptDateTime.Date > MiscData.GetNowDateTime().Date) {//if appointment is in the future
						actionOnFailure.Invoke(Lans.g(translationSource,"Not allowed because procedure is attached to a future appointment with a date of ")
							+apt.AptDateTime.ToShortDateString());
						return false;
					}
					if(apt.AptDateTime.Year>=1880) {
						textDate=apt.AptDateTime;
					}
				}
				if(!isQuickAdd && !Security.IsAuthorized(Permissions.ProcComplCreate,textDate,true,true, user,0,-1,0,0,actionNotAuthorized: actionOnFailure)){//use the new date
					return false;
				}
			}
			else if(!isQuickAdd && isNew && procedure.ProcStatus==ProcStat.C) {//if new procedure is complete
				if(!Security.IsAuthorized(Permissions.ProcComplCreate,textDate,true,true,user,procedure.CodeNum,procFee,0,0,actionNotAuthorized: actionOnFailure)){
					return false;
				}
			}
			else if(!isNew) {//an old procedure
				if(procedureOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {//that was already complete
					if(!CheckPermissionsAndGlobalLockDate(procedureOld,procedure,textDate,user,procFee,actionOnFailure)) {
						return false;
					}
				}
			}
			#endregion
			#region Canada and Prosthesis
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(isCheckTypeCodeXChecked) {
					if(isCheckTypeCodeNonXChecked) {
						actionOnFailure.Invoke(Lans.g(translationSource,"If type code 'none' is checked, no other type codes may be checked."));
						return false;
					}
				}
				if(procedureCode.IsProsth && !isCheckTypeCodeNonXChecked && !isCheckTypeCodeXChecked) {
					if(!funcYesNoPrompt.Invoke(Lans.g(translationSource,"At least one type code should be checked for prosthesis.  Continue anyway?"))) {
						return false;
					}
				}
			}
			else {
				if(procedureCode.IsProsth) {
					if(listProsthSelectedIndex == 0 || (listProsthSelectedIndex == 2 && isTextDateOriginalProsthBlank)) {
						if(!funcYesNoPrompt.Invoke(Lans.g(translationSource,"Prosthesis date not entered. Continue anyway?"))){
							return false;
						}
					}
				}
			}
			#endregion
			#region Quadrant
			if(procedureCode.TreatArea==TreatmentArea.Quad) {
				if(!isQuadrantSelected) {
					actionOnFailure.Invoke(Lans.g(translationSource,"Please select a quadrant."));
					return false;
				}
			}
			#endregion
			#region Provider
			listClaimProcsForProc=ClaimProcs.GetForProc(ClaimProcs.Refresh(procedure.PatNum),procedure.ProcNum);//update for accuracy
			if(!ValidateProvider(listClaimProcsForProc,provNumSelected, procedureOld.ProvNum,actionOnFailure)) {
				return false;
			}
			#endregion
			//Block if proc is linked to ortho case and user tries to set status from complete to any other status.
			if(hasOrthoProcLink && (procedureOld.ProcStatus==ProcStat.C && procedure.ProcStatus!=ProcStat.C)) {
				actionOnFailure.Invoke(Lans.g(translationSource,"The status of a completed procedure that is attached to an ortho case cannot be changed. " +
					"Detach the procedure from the ortho case or delete the ortho case first."));
				return false;
			}
			//Once upon a time we used to use the textProc.Text field to validate that the procedure was valid. Sometimes, that text field would not have a value, or have some strange messed up value that did not coorespond to a procedure code. We no longer need to check that, so we are checking the procedure object itself. All this to say, this check will likely never return false, but just in case, here it is. 
			try {
				ProcedureCodes.GetStringProcCode(procedure.CodeNum,doThrowIfMissing: true);
			}
			catch(ApplicationException ae) {
				actionOnProcedureCodeFailure.Invoke();
				return false;
			}
			//double check that nothing has happed with the _procedureCode object and that it still reflects a valid code. If it doesn't use the CodeNum from _procedure to get a valid procedureCode object.
			try {
				ProcedureCodes.GetStringProcCode(procedureCode.CodeNum,doThrowIfMissing: true);
				if(procedure.CodeNum != procedureCode.CodeNum) {
					procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum); //_procedureCode is valid, but for some reason has fallen out of sync with the _procedure object. Force them back in sync before saving.
				}
			}
			catch(Exception e) {
				e.DoNothing();
				procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
				//no need to return false here since we fixed the issue.
			}
			return true;
		}

		///<summary>Returns true if quantity is valid, otherwise false.
		///Optionally calls actionOnFailure with a translated message if validation failed.</summary>
		public static bool IsQuantityValid(int quantity,Action<string> actionOnFailure=null) {
			//No need to check MiddleTierRole; no call to db.
			if(quantity<1) {
				actionOnFailure?.Invoke(Lans.g("Procedures", "Qty not valid.  Typical value is 1."));
				return false;
			}
			return true;
		}

		///<summary>Returns true if timeStart and timeEnd are valid, otherwise false.
		///Optionally calls actionOnFailure with a translated message if validation failed.</summary>
		public static bool AreTimesValid(string timeStart,string timeEnd,Action<string> actionOnFailure=null) {
			//No need to check MiddleTierRole; no call to db.
			if(Programs.UsingOrion || PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				if(!ValidateTime(timeStart)) {
					actionOnFailure?.Invoke(Lans.g("Procedures", "Start time is invalid."));
					return false;
				}
				if(!ValidateTime(timeEnd)) {
					actionOnFailure?.Invoke(Lans.g("Procedures", "End time is invalid."));
					return false;
				}
			}
			else {
				if(timeStart != "") {
					try {
						DateTime.Parse(timeStart);
					}
					catch {
						actionOnFailure?.Invoke(Lans.g("Procedures", "Start time is invalid."));
						return false;
					}
				}
			}
			return true;
		}

		///<summary>Returns true if given time value is a valid time, otherwise false. Empty string is considered valid.</summary>
		public static bool ValidateTime(string time) {
			//No need to check MiddleTierRole; no call to db.
			string militaryTime=time;
			if(militaryTime=="") {
				return true;
			}
			if(militaryTime.Length<4) {
				militaryTime=militaryTime.PadLeft(4, '0');
			}
			//Test if user typed in military time. Ex: 0830 or 1536
			try {
				int hour=PIn.Int(militaryTime.Substring(0, 2));
				int minute=PIn.Int(militaryTime.Substring(2, 2));
				if(hour>23) {
					return false;
				}
				if(minute>59) {
					return false;
				}
				return true;
			}
			catch { }
			//Test typical DateTime format. Ex: 1:00 PM
			try {
				DateTime.Parse(time);
				return true;
			}
			catch {
				return false;
			}
		}

		///<summary>Returns true if provider change is valid, otherwise false.
		///Optionally calls actionOnFailure with a translated message if validation failed.</summary>
		public static bool ValidateProvider(List<ClaimProc> listClaimProcsForProc,long selectedProvNum,long provNumForProc,Action<string> actionOnFailure=null) {
			//No need to check MiddleTierRole; no call to db.
			//validate for provider change
			if(provNumForProc!=selectedProvNum && PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim)) {
				//if selected prov is null (no selection made), no change will happen to the provider
				if(listClaimProcsForProc.Any(x => x.Status.In(ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim))) {
					actionOnFailure?.Invoke(Lans.g("Procedures","The provider cannot be changed when this procedure is attached to a claim."));
					return false;
				}
			}
			return true;
		}

		///<summary>Only needs to be called when procOld.ProcStatus is C, EO or EC.</summary>
		public static bool CheckPermissionsAndGlobalLockDate(Procedure procOld,Procedure procNew,DateTime procDate,Userod user,
			double procFeeOverride=double.MinValue,Action<string> actionNotAuthorized=null)
		{
			//No need to check MiddleTierRole; no call to db.
			if(!procOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {//that was already complete
				return true;
			}
			Permissions perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteStatusEdit,procOld);
			DateTime dateToUseProcOld=Procedures.GetDateForPermCheck(procOld);
			if(procOld.ProcStatus!=procNew.ProcStatus 
				&& !Security.IsAuthorized(perm,dateToUseProcOld,true,true,user,0,-1,0,0,actionNotAuthorized:actionNotAuthorized)) 
			{//block old date
				return false;
			}
			if(procNew.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				DateTime dateToUseProcCur=Procedures.GetDateForPermCheck(procNew,procDate);
				if(procOld.ProcStatus!=procNew.ProcStatus) {
					if(procOld.ProcStatus==ProcStat.C) {
						perm=Permissions.ProcCompleteStatusEdit;
					}
					else if(procNew.ProcStatus==ProcStat.C) {
						perm=Permissions.ProcComplCreate;
					}
					else {
						perm=Permissions.ProcExistingEdit;
					}
					if(!Security.IsAuthorized(perm,dateToUseProcCur,true,true,user,0,-1,0,0,actionNotAuthorized:actionNotAuthorized)) {//block new date, too
						return false;
					}
				}
				double procFee=procNew.ProcFee;
				if(procFeeOverride!=double.MinValue) {
					procFee=procFeeOverride;
				}
				if(procOld.ProcDate!=procDate //If user changed the procedure date
					|| !CompareDouble.IsEqual(procOld.ProcFee,procFee) //If user changed the procedure fee
					|| procOld.CodeNum != procNew.CodeNum) //If user changed the procedure code
				{
					perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit,procNew);
					if(!Security.IsAuthorized(perm,dateToUseProcCur,true,true,user,procNew.CodeNum,procFee,0,0,actionNotAuthorized:actionNotAuthorized)) {
						return false;
					}
				}
			}
			return true;
		}

		///<summary>Returns true if given values are valid for a tooth number, otherwise false.
		///toothSurface is a ref because the value can run through some tidy functions for display.</summary>
		public static bool ValidateToothValue(string toothNumLabel,ref string toothSurface,Action<string> actionOnFailure=null) {
			//No need to check MiddleTierRole; no call to db.
			if(Tooth.IsValidEntry(toothNumLabel)){
				toothSurface=Tooth.SurfTidyForDisplay(toothSurface,Tooth.Parse(toothNumLabel));
			}
			else{
				toothSurface=Tooth.SurfTidyForDisplay(toothSurface, "");
			}
			if(toothSurface=="") { 
				actionOnFailure?.Invoke(Lans.g(nameof(Procedures), "No surfaces selected."));
				return false;
			}
			return true;
		}

		///<summary>Returns true if hasSextantSelection is true, otherwise false.
		///This method exist so that it can serve both error providers in FormProcEdit and centralized Procedures.cs validation.</summary>
		public static bool ValidateSextant(bool hasSextantSelection,Action<string> actionOnFailure=null) {
			//No need to check MiddleTierRole; no call to db.
			if(hasSextantSelection) {
				return true;
			}
			actionOnFailure?.Invoke(Lans.g(nameof(Procedures),"Please select a sextant treatment area."));
			return false;
		}

		///<summary>Returns true if isArchSelected is true, otherwise false.
		///This method exist so that it can serve both error providers in FormProcEdit and centralized Procedures.cs validation.</summary>
		public static bool ValidateArch(bool isArchSelected, Action<string> actionOnFailure=null) {
			//No need to check MiddleTierRole; no call to db.
			if(isArchSelected) {
				return true;
			}
			actionOnFailure?.Invoke(Lans.g(nameof(Procedures),"Please select a arch treatment area."));
			return false;
		}

		///<summary>A helper method that updates various fields for the given procedure.</summary>
		public static void UpdateProcedureFields(Procedure procedure,Patient patient,string textMedicalCodeText,double discount,Snomed snomedBodySite,
			bool checkIcdVersionChecked, List<string> diagnosticCodes,bool checkIsPrincDiagChecked,long selectedProvOrderNum,Referral referralOrdering,
			string textCodeMod1,string textCodeMod2,string textCodeMod3,string textCodeMod4,int unitQty,ProcUnitQtyType unitQtyType,string revCode,EnumProcDrugUnit drugUnit,
			float drugQty,ProcUrgency procUrgency,long selectedProvNum,long clinicNum)
		{
			procedure.PatNum=patient.PatNum;
			procedure.MedicalCode=textMedicalCodeText;
			procedure.Discount=discount;
			if(snomedBodySite==null) {
				procedure.SnomedBodySite="";
			}
			else {
				procedure.SnomedBodySite=snomedBodySite.SnomedCode;
			}
			procedure.IcdVersion=9;
			if(checkIcdVersionChecked) {
				procedure.IcdVersion=10;
			}
			procedure.DiagnosticCode="";
			procedure.DiagnosticCode2="";
			procedure.DiagnosticCode3="";
			procedure.DiagnosticCode4="";
			if(diagnosticCodes.Count>0) {
				procedure.DiagnosticCode=diagnosticCodes[0];
			}
			if(diagnosticCodes.Count>1) {
				procedure.DiagnosticCode2=diagnosticCodes[1];
			}
			if(diagnosticCodes.Count>2) {
				procedure.DiagnosticCode3=diagnosticCodes[2];
			}
			if(diagnosticCodes.Count>3) {
				procedure.DiagnosticCode4=diagnosticCodes[3];
			}
			procedure.IsPrincDiag=checkIsPrincDiagChecked;
			procedure.ProvOrderOverride=selectedProvOrderNum;
			if(referralOrdering==null) {
				procedure.OrderingReferralNum=0;
			}
			else {
				procedure.OrderingReferralNum=referralOrdering.ReferralNum;
			}
			procedure.CodeMod1=textCodeMod1;
			procedure.CodeMod2=textCodeMod2;
			procedure.CodeMod3=textCodeMod3;
			procedure.CodeMod4=textCodeMod4;
			procedure.UnitQty=unitQty;
			procedure.UnitQtyType=unitQtyType;
			procedure.RevCode=revCode;
			procedure.DrugUnit=drugUnit;
			procedure.DrugQty=drugQty;
			procedure.Urgency=procUrgency;
			procedure.ProvNum=selectedProvNum;
			procedure.ClinicNum=clinicNum;
		}

		///<summary>Returns false if;
		///<para>1) Given proc was previously compete but was changed and either the user declines adjustment changes due to status change or there are paysplits assocaited to the proc.</para>
		///<para>2) Proc was set complete when it previously wasn't and there are hidden proc codes.</para>
		///Otherwise returns true. This method can also update various properties for given procedure.
		///</summary>
		public static bool VerifyCompletedProcStatusChange(List<PaySplit> listPaySplitsForProc,Procedure procedure,Procedure procedureOld,
			string translationSource,Func<string,bool> funcYesNoPrompt,Action<string> actionOnFailure=null)
		{
			double sumPaySplits=0;
			for(int i=0;i<listPaySplitsForProc.Count;i++) {
				sumPaySplits+=listPaySplitsForProc[i].SplitAmt;
			}
			if(procedureOld.ProcStatus==ProcStat.C && procedure.ProcStatus!=ProcStat.C) {//Proc was complete but was changed.
				if(Adjustments.GetForProc(procedure.ProcNum,Adjustments.Refresh(procedure.PatNum)).Count!=0
				&& !funcYesNoPrompt.Invoke(Lans.g(translationSource,"This procedure has adjustments attached to it. Changing the status from completed will delete any adjustments for the procedure. Continue?"))) {
					return false;
				}
				if(sumPaySplits!=0) {
					actionOnFailure?.Invoke(Lans.g(translationSource, "Not allowed to modify the status of a procedure that has payments attached to it. Detach payments from the procedure first."));
					return false;
				}
			}
			else if(procedureOld.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C) {//Proc set complete.
				if(ProcedureCodes.AreAnyProcCodesHidden(procedure.CodeNum)) {
					actionOnFailure?.Invoke($"{Lans.g(translationSource, "Procedure cannot be set complete because it is in a hidden category")}: {ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode}");
					return false;
				}
				procedure.DateEntryC=DateTime.Now;//this triggers it to set to server time NOW().
				if(procedure.DiagnosticCode=="") {
					procedure.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
					procedure.IcdVersion=PrefC.GetByte(PrefName.DxIcdVersion);
				}
			}
			return true;
		}

		///<summary>Returns true if provider was not changed or if the provider was changed and there are no pay splits or adustments associated to the procedure or 
		///if user confirms pay split and/or adjustment changes. Otherwise false.</summary>
		public static bool VerifyProviderChange(Procedure procedure, Procedure procedureOld,List<Adjustment> listAdjusts,out bool hasSplitProvChanged,out bool hasAdjProvChanged,
			string translationSource,Func<string,bool> funcYesNoPrompt,Action<string> actionNotAuthorized=null)
		{
			hasSplitProvChanged=false;
			hasAdjProvChanged=false;
			if(procedure.ProvNum!=procedureOld.ProvNum) {
				if(PaySplits.IsPaySplitAttached(procedure.ProcNum)) {
					List<PaySplit> listPaySplit=PaySplits.GetPaySplitsFromProc(procedure.ProcNum);
					foreach(PaySplit paySplit in listPaySplit) {
						if(!Security.IsAuthorized(Permissions.PaymentEdit,Payments.GetPayment(paySplit.PayNum).PayDate,actionNotAuthorized: actionNotAuthorized)) {
							return false;
						}
						if(procedure.ProvNum!=paySplit.ProvNum) {
							hasSplitProvChanged=true;
						}
					}
					if(hasSplitProvChanged
						&& !funcYesNoPrompt.Invoke(Lans.g(translationSource,"The provider for the associated payment splits will be changed to match the provider on the procedure. Continue?"))) {
						return false;
					}
				}
				foreach(Adjustment adjust in listAdjusts) {
					if(!Security.IsAuthorized(Permissions.AdjustmentEdit,adjust.AdjDate,actionNotAuthorized: actionNotAuthorized)) {
						return false;
					}
					if(procedure.ProvNum!=adjust.ProvNum && PrefC.GetInt(PrefName.RigorousAdjustments)==(int)RigorousAdjustments.EnforceFully) {
						hasAdjProvChanged=true;
					}
				}
				if(hasAdjProvChanged
					&& !funcYesNoPrompt.Invoke(Lans.g(translationSource, "The provider for the associated adjustments will be changed to match the provider on the procedure. Continue?"))) {
					return false;
				}
			}
			return true;
		}

		///<summary>Helper method that sets various datetime fields on the procedure.</summary>
		public static void SetMiscDateAndTimeEditFields(Procedure procedure,string textDateTP,DateTime procDate,string textTimeStart,string textTimeEnd) {
			// textDateTP.Text is blank upon load if date in DB is before 1/1/1880. We don't want to update this if the DateTP box is left blank.
			if(procedure.DateTP.Year>1880 || textDateTP!="") {
				procedure.DateTP=PIn.Date(textDateTP);
			}
			procedure.ProcDate=procDate;
			DateTime dateT=PIn.DateT(textTimeStart);
			procedure.ProcTime=new TimeSpan(dateT.Hour,dateT.Minute,0);
			if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				dateT=ParseTime(textTimeStart);
				procedure.ProcTime=new TimeSpan(dateT.Hour,dateT.Minute,0);
				dateT=ParseTime(textTimeEnd);
				procedure.ProcTimeEnd=new TimeSpan(dateT.Hour,dateT.Minute,0);
			}
		}

		///<summary>Returns min value if blank or invalid string passed in.</summary>
		public static DateTime ParseTime(string time) {
			string militaryTime=time;
			DateTime dTime=DateTime.MinValue;
			if(militaryTime=="") {
				return dTime;
			}
			if(militaryTime.Length<4) {
				militaryTime=militaryTime.PadLeft(4,'0');
			}
			//Test if user typed in military time. Ex: 0830 or 1536
			try {
				int hour=PIn.Int(militaryTime.Substring(0,2));
				int minute=PIn.Int(militaryTime.Substring(2,2));
				dTime=new DateTime(1,1,1,hour,minute,0);
				return dTime;
			}
			catch { }
			//Test if user typed in a typical DateTime format. Ex: 1:00 PM
			try { 
				return DateTime.Parse(time);
			}
			catch { }
			return dTime;
		}

		///<summary>Return true if given procedureCode TreatArea is NOT associated to a tooth range (TreatArea or AreaAlsoToothRange).
		///Also returns true when given procedureCode IS associated to a tooth range and tooth selection can be validated.
		///This method can modify procedure; ToothRange, ToothNum and Surf.</summary>
		public static bool SetAndValidateToothData(ProcedureCode procedureCode, Procedure procedure,string textTooth,string textSurfaces,
			List<int> listBoxTeethSelectedIndices,List<int> listBoxTeeth2SelectedIndices,List<string> listPriTeeth,
			string translationSource,Action<string> actionOnFailure=null)
		{
			if(procedureCode.TreatArea==TreatmentArea.None
				|| procedureCode.TreatArea==TreatmentArea.Mouth)
			{
				procedure.Surf="";
				procedure.ToothNum="";	
			}
			if(procedureCode.TreatArea==TreatmentArea.Surf){
				procedure.ToothNum=Tooth.Parse(textTooth);
				procedure.Surf=Tooth.SurfTidyFromDisplayToDb(textSurfaces,procedure.ToothNum);
			}
			if(procedureCode.TreatArea==TreatmentArea.Tooth){
				procedure.Surf="";
				procedure.ToothNum=Tooth.Parse(textTooth);
			}
			if(procedureCode.TreatArea==TreatmentArea.Quad){
				//surf set when radio pushed
				procedure.ToothNum="";	
			}
			if(procedureCode.TreatArea==TreatmentArea.Sextant){
				//surf taken care of when radio pushed
				procedure.ToothNum="";	
			}
			if(procedureCode.TreatArea==TreatmentArea.Arch){
				//taken care of when radio pushed
				procedure.ToothNum="";	
			}
			if(procedureCode.TreatArea==TreatmentArea.ToothRange
				|| procedureCode.AreaAlsoToothRange)
			{
				if(listBoxTeethSelectedIndices.Count<1 && listBoxTeeth2SelectedIndices.Count<1) {
					actionOnFailure?.Invoke(Lans.g(translationSource, "Must pick at least 1 tooth"));
					return false;
				}
				List <string> listSelectedToothNums=new List<string>();
				//Store selected teeth in Maxillary/Upper Arch.
				foreach(int index in listBoxTeethSelectedIndices) {
					listSelectedToothNums.Add((index+1).ToString());
				}
				//Store selected teeth in Mandibular/Lower Arch.
				foreach(int index in listBoxTeeth2SelectedIndices) {
					listSelectedToothNums.Add((32-index).ToString());
				}
				//Identify selected teeth which are primary and convert from permanent tooth num to primary tooth num for storage into database.
				for(int j=0;j<listSelectedToothNums.Count;j++) {
					if(listPriTeeth.Contains(listSelectedToothNums[j])) {
						listSelectedToothNums[j]=Tooth.PermToPri(listSelectedToothNums[j]);
					}
				}
				procedure.ToothRange=String.Join(",",listSelectedToothNums);
				procedure.ToothNum="";	
				if(procedureCode.AreaAlsoToothRange){
					//arch or quad stored in surf
				}
				else{
					procedure.Surf="";
				}
			}
			return true;
		}

		///<summary>Helper method that sets procedure Note field..</summary>
		public static void SetNote(Procedure procedure,Procedure procedureOld,string textNotes) {
			//Status taken care of when list pushed
			procedure.Note=textNotes;
			//Larger offices have trouble with doctors editing specific procedure notes at the same time.
			//One of our customers paid for custom programming that will merge the two notes together in a specific fashion if there was concurrency issues.
			//A specific preference was added because this functionality is so custom.  Typical users can just use the Chart View Audit mode for this info.
			if(procedureOld.ProcNum > 0 && PrefC.GetBool(PrefName.ProcNoteConcurrencyMerge)) {
				//Go to the database to get the most recent version of the current procedure's note and check it against ProcOld.Note to see if they differ.
				List<ProcNote> listProcNotes=ProcNotes.GetProcNotesForProc(procedureOld.ProcNum)
					.OrderByDescending(x => x.EntryDateTime)
					.ThenBy(x => x.ProcNoteNum)//Just in case two notes were entered at the "same time" (current version of MySQL can't handle milliseconds)
					.ToList();
				//If there are notes for the current procedure, get the most recent note and compare it to ProcOld.Note.
				//If the current database note differs from the ProcOld.Note then there was a concurrency issue and we have to merge the db note.
				if(listProcNotes.Count > 0 && procedureOld.Note!=listProcNotes[0].Note) {
					//Manipulate ProcCur.Note to include the most recent note in its entirety with some custom information required by job #2484
					//Use DateTime.Now because the ProcNote won't get inserted until farther down in this method but we have to do this manipulation before sig.
					procedure.Note=DateTime.Now.ToString()+"  "+Userods.GetName(procedure.UserNum)+"\r\n"+procedure.Note;
					//Now we need to append the old note from the database in the same format.
					procedure.Note+="\r\n------------------------------------------------------\r\n"
						+listProcNotes[0].EntryDateTime.ToString()+"  "+Userods.GetName(listProcNotes[0].UserNum)
						+"\r\n"+listProcNotes[0].Note;
				}
			}
		}

		///<summary>Does NOT check region, verify region is in CA prior to calling this method.</summary>
		public static void SetCanadianEditFields(Procedure procedure,bool checkTypeCodeAChecked,bool checkTypeCodeBChecked,bool checkTypeCodeCChecked,bool checkTypeCodeEChecked,
			bool checkTypeCodeLChecked,bool checkTypeCodeSChecked,bool checkTypeCodeXChecked)
		{
			procedure.CanadianTypeCodes="";
			if(checkTypeCodeAChecked) {
				procedure.CanadianTypeCodes+="A";
			}
			if(checkTypeCodeBChecked) {
				procedure.CanadianTypeCodes+="B";
			}
			if(checkTypeCodeCChecked) {
				procedure.CanadianTypeCodes+="C";
			}
			if(checkTypeCodeEChecked) {
				procedure.CanadianTypeCodes+="E";
			}
			if(checkTypeCodeLChecked) {
				procedure.CanadianTypeCodes+="L";
			}
			if(checkTypeCodeSChecked) {
				procedure.CanadianTypeCodes+="S";
			}
			if(checkTypeCodeXChecked) {
				procedure.CanadianTypeCodes+="X";
			}
		}

		public static void CanadianLabHelper(string textCanadaLabFee1Text,bool isCanadianLab,List<Procedure> listCanadaLabFees,Procedure procedure,string textCanadaLabFee2Text) {
			double canadaLabFee1=0;
			if(textCanadaLabFee1Text!="") {
				canadaLabFee1=PIn.Double(textCanadaLabFee1Text);
			}
			if(canadaLabFee1==0) {
				if(!isCanadianLab && listCanadaLabFees.Count>0) { //Don't worry about deleting child lab fees if we are editing a lab fee. No such concept.
					TryDeleteLab(listCanadaLabFees[0]);
				}
			}
			else { //canadaLabFee1!=0
				if(listCanadaLabFees.Count>0) { //Retain the old lab code if present.
					Procedure labFee1Old=listCanadaLabFees[0].Copy();
					listCanadaLabFees[0].ProcFee=canadaLabFee1;
					Update(listCanadaLabFees[0],labFee1Old);
				}
				else {
					CanadaLabProcEditHelper(procedure,canadaLabFee1);
				}
			}
			double canadaLabFee2=0;
			if(textCanadaLabFee2Text!="") {
				canadaLabFee2=PIn.Double(textCanadaLabFee2Text);
			}
			if(canadaLabFee2==0) {
				if(!isCanadianLab && listCanadaLabFees.Count>1) { //Don't worry about deleting child lab fees if we are editing a lab fee. No such concept.
					TryDeleteLab(listCanadaLabFees[1]);
				}
			}
			else { //canadaLabFee2!=0
				if(listCanadaLabFees.Count>1) { //Retain the old lab code if present.
					Procedure labFee2Old=listCanadaLabFees[1].Copy();
					listCanadaLabFees[1].ProcFee=canadaLabFee2;
					Update(listCanadaLabFees[1],labFee2Old);
				}
				else {
					CanadaLabProcEditHelper(procedure,canadaLabFee2);
				}
			}
		}

		///<summary>Centralizes the code to create a lab fee. Should only be called in Canada region but we return if we're not to protect against labs procedures 
		///being created in non-Canadian offices.</summary>
		public static void CanadaLabProcEditHelper(Procedure procedure,double labFeeAmt) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")){
				return;//We don't care if we're not Canadian.
			}
			ProcedureCode labCode=ProcedureCodes.GetProcCode("99111");
			Procedure labFee=new Procedure();
			labFee.PatNum=procedure.PatNum;
			labFee.ProcDate=procedure.ProcDate;
			labFee.ProcFee=labFeeAmt;
			labFee.ProcStatus=procedure.ProcStatus;
			labFee.DateEntryC=DateTime.Now;
			labFee.ClinicNum=procedure.ClinicNum;
			labFee.ProcNumLab=procedure.ProcNum;
			labFee.CodeNum=labCode.CodeNum;
			//Not sure if Place of Service is required for canadian labs. (I don't see any reason why this would/could/should break anything.)
			labFee.PlaceService=Clinics.GetPlaceService(procedure.ClinicNum);
			if(labFee.CodeNum==0) { //Code does not exist.
				ProcedureCode code99111=new ProcedureCode();
				code99111.IsCanadianLab=true;
				code99111.ProcCode="99111";
				code99111.Descript="+L Commercial Laboratory Procedures";
				code99111.AbbrDesc="Lab Fee";
				code99111.ProcCat=Defs.GetByExactNameNeverZero(DefCat.ProcCodeCats,"Adjunctive General Services");
				ProcedureCodes.Insert(code99111);
				labFee.CodeNum=code99111.CodeNum;
				ProcedureCodes.RefreshCache();
			}
			labFee.ProvNum=procedure.ProvNum;
			if(labCode.ProvNumDefault!=0) {
				labFee.ProvNum=labCode.ProvNumDefault;
			}
			Procedures.Insert(labFee);
		}

		///<summary>Currently is only called by FormProcEdit.cs when NOT running in Cananda.</summary>
		public static void SetProsthEditFields(ProcedureCode procedureCode,Procedure procedure,int listProsthSelectedIndex,DateTime dateOriginalProsth,bool checkIsDateProsthEstChecked) {
			if(procedureCode.IsProsth) {
				switch(listProsthSelectedIndex) {
					case 0:
						procedure.Prosthesis="";
						break;
					case 1:
						procedure.Prosthesis="I";
						break;
					case 2:
						procedure.Prosthesis="R";
						break;
				}
				procedure.DateOriginalProsth=dateOriginalProsth;
				procedure.IsDateProsthEst=checkIsDateProsthEstChecked;
			}
			else {
				procedure.Prosthesis="";
				procedure.DateOriginalProsth=DateTime.MinValue;
				procedure.IsDateProsthEst=false;
			}
		}

		///<summary>Checks auto codes if the procedure is not complete or the user has permission to edit completed procedures, can prompt user.</summary>
		public static void TryAutoCodesPrompt(ref Procedure procedure,Procedure procedureOld,ProcedureCode procedureCode,bool isMandibular,Patient patient,ref List<ClaimProc> listClaimProcsForProc,
			Func<long,Procedure> funcPromptFormACLI)
		{
			Permissions perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit,procedure);
			DateTime dateForPerm=GetDateForPermCheck(procedure);
			if(!procedureOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC) || Security.IsAuthorized(perm,dateForPerm,true)) {
				//Only check auto codes if the procedure is not complete or the user has permission to edit completed procedures.
				if(AutoCodeItems.ShouldPromptForCodeChange(procedure,procedureCode,patient,isMandibular,listClaimProcsForProc,out long verifyCode)) {
					Procedure proc=funcPromptFormACLI(verifyCode);
					if(proc==null){
						return;
					}
					procedure=proc;
					listClaimProcsForProc=ClaimProcs.RefreshForProc(procedure.ProcNum);//funPromptFormACLI may have added claimprocs.
				}
			}
		}

		///<summary>Called in our mobile applications, mimics FormProcEdit logic.</summary>
		public static void TryValidateProcFee(Procedure procedure,Procedure procedureOld,Patient patient,Func<string,bool> funcYesNoPrompt,
			out List<PatPlan> listPatPlans,out List<InsSub> listInsSubs,out List<InsPlan> listInsPlans,out List<SubstitutionLink> listSubstitutionLinks,
			out List<Benefit> listBenefits,out List<Fee> listFees)
		{
			//Mimics FormProcEdit.cs
			listPatPlans=PatPlans.Refresh(patient.PatNum);
			listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(patient.PatNum));
			listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>(){ ProcedureCodes.GetProcCode(procedure.CodeNum) };
			List<Procedure> listProcedures=new List<Procedure>(){ procedure };
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(patient.PatNum,procedure.ProcDate);
			listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
			listFees=Fees.GetListFromObjects(listProcedureCodes,listProcedures.Select(x=>x.MedicalCode).ToList(),listProcedures.Select(x=>x.ProvNum).ToList(),
				patient.PriProv,patient.SecProv,patient.FeeSched,listInsPlans,listProcedures.Select(x=>x.ClinicNum).ToList(),null,//appts not needed
				listSubstitutionLinks,discountPlanNum
			);
			listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			TryValidateProcFee(procedure,procedureOld,patient,listFees,listPatPlans,listInsSubs,listInsPlans,listBenefits,funcYesNoPrompt);
		}

		///<summary>If fees should change, will prompt the user and update procedure.ProcFee.</summary>
		public static void TryValidateProcFee(Procedure procedure,Procedure procedureOld,Patient patient,List<Fee> listFees,List<PatPlan> listPatPlans,
			List<InsSub> listInsSubs,List<InsPlan> listInsPlans,List<Benefit> listBenefits,Func<string,bool> funcYesNoPrompt)
		{
			if(procedure.ProvNum!=procedureOld.ProvNum 
				&& procedure.ProcFee==procedureOld.ProcFee)
			{
				string promptText="";
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(patient,listFees,listPatPlans,listInsSubs,listInsPlans,listBenefits);
				bool isUpdatingFee=ShouldFeesChange(new List<Procedure>() { procedure.Copy() },new List<Procedure>() { procedureOld.Copy() },
					ref promptText,procFeeHelper
				);
				if(isUpdatingFee) {//Made it past the pref check.
					if(promptText!="" && !funcYesNoPrompt.Invoke(promptText)) {
							isUpdatingFee=false;
					}
					if(isUpdatingFee) {
						procedure.ProcFee=GetProcFee(patient,listPatPlans,listInsSubs,listInsPlans,procedure.CodeNum,procedure.ProvNum,
							procedure.ClinicNum,procedure.MedicalCode,listBenefits,listFees:listFees
						);
					}
				}
			}
		}

		///<summary>Returns true if procedureOld was not set complete and procedure is set complete and if the proc can be linked to actie ortho case./summary>
		public static bool IsProcLinkedToOrthoCase(Procedure procedureOld,Procedure procedure,ref OrthoProcLink orthoProcLink) {
			//If proc is set complete and orthocases are on, check if we need to link it to an ortho case.
			if(procedureOld.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C) {
				OrthoCaseProcLinkingData linkingData=new OrthoCaseProcLinkingData(procedure.PatNum);
				orthoProcLink=OrthoProcLinks.TryLinkProcForActiveOrthoCase(linkingData,procedure);//Updates the _procCur.ProcFee for in memory object.
			}
			else if(orthoProcLink!=null && procedureOld.ProcDate!=procedure.ProcDate) {
				OrthoCases.UpdateDatesByLinkedProc(orthoProcLink,procedure);
			}
			return orthoProcLink!=null;
		}

		///<summary>Used by mobile apps to trigger automation when a procedure is set complete.</summary>
		public static void CompleteProcedureAutomation(Procedure procedure,Procedure procedureOld,
			Dictionary<long, Dictionary<long,DateTime>> dicBlockedAutomations,Action<string> actionOnShowMsg,
			Func<string,bool> funcYesNoPrompt,Action<Commlog> onShowCommLog,Action<Sheet> onShowSheetFillEdit)
		{
			if(procedureOld.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C) {
				List<string> procCodeList=new List<string>() {
					ProcedureCodes.GetStringProcCode(procedure.CodeNum)
				};
				Func<List<Procedure>,Image> funcCreateToothChartImage=(listProcs) => {
					return ToothChartHelper.GetImage(procedure.PatNum,false,listProceduresFilteredOverride:listProcs);
				};
				Automations.Trigger<object>(AutomationTrigger.CompleteProcedure,procCodeList,procedure.PatNum,
					dicBlockedAutomations,actionOnShowMsg,(msg,caption) => funcYesNoPrompt.Invoke(msg),onShowCommLog,onShowSheetFillEdit,funcCreateToothChartImage
				);
				AfterProcsSetComplete(new List<Procedure>() { procedure });
			}
		}

		///<summary>Should be run after procedures are set complete, automates logic such as adding sales tax.</summary>
		public static void AfterProcsSetComplete(List<Procedure> listProcedures) {
			if(PrefC.GetBool(PrefName.SalesTaxDoAutomate) && !PrefC.IsODHQ) { //HQ shouldn't be doing this, it's taken care of elsewhere by AvaTax.
				for(int i=0; i<listProcedures.Count; i++) {
					Adjustments.AddSalesTaxIfNoneExists(listProcedures[i]);
				}
			}
		}

		///<summary>Used in mobile applications.</summary>
		public static string GetSignatureKeyData(Procedure procedure, string updatedNote=null) {
			string keyData=procedure.Note;
			if (updatedNote!=null) {
				keyData=updatedNote;
			}
			keyData+=procedure.UserNum.ToString();
			keyData=keyData.Replace("\r\n","\n");//We need all newlines to be the same, a mix of /r/n and /n can invalidate the procedure signature.
			return keyData;
		}

		///<summary>Used in mobile applications.</summary>
		public static bool TryValidateSignatures(Procedure procedure, string signature, out string signatureResult, out string error) {
			error=null;
			string keyData=GetSignatureKeyData(procedure);
			//331 and 79 are the width and height of the signature box in FormTPsign.cs
			signatureResult=UI.SigBox.EncryptSigString(ODCrypt.MD5.Hash(Encoding.UTF8.GetBytes(keyData)), TreatPlans.GetScaledSignature(signature));
			if(signatureResult.IsNullOrEmpty()) {
				error="Error occurred when encrypting the patient signature.";
			}
			return (error.IsNullOrEmpty());
		}
		#endregion

		//--------------------Taken from Procedure class--------------------------------------------------
		/*
		///<summary>Gets allowedOverride for this procedure based on supplied claimprocs. Includes all claimproc types.  Only used in main TP module when calculating PPOs. The claimProc array typically includes all claimProcs for the patient, but must at least include all claimprocs for this proc.</summary>
		public static double GetAllowedOverride(Procedure proc,ClaimProc[] claimProcs,int priPlanNum) {
			//double retVal=0;
			for(int i=0;i<claimProcs.Length;i++) {
				if(claimProcs[i].ProcNum==proc.ProcNum && claimProcs[i].PlanNum==priPlanNum) {
					return claimProcs[i].AllowedOverride;
					//retVal+=claimProcs[i].WriteOff;
				}
			}
			return 0;//retVal;
		}*/

		/*
		///<summary>Gets total writeoff for this procedure based on supplied claimprocs. Includes all claimproc types.  Only used in main TP module. The claimProc array typically includes all claimProcs for the patient, but must at least include all claimprocs for this proc.</summary>
		public static double GetWriteOff(Procedure proc,List<ClaimProc> claimProcs) {
			//No need to check MiddleTierRole; no call to db.
			double retVal=0;
			for(int i=0;i<claimProcs.Count;i++) {
				if(claimProcs[i].ProcNum==proc.ProcNum) {
					retVal+=claimProcs[i].WriteOff;
				}
			}
			return retVal;
		}*/

		#region InsHist Preference
		///<summary>Returns a procedure with the date passed in. New procedure defaults to the patient's clinic, primary provider and uses the 
		///first code in the InsHistPref passed in. </summary>
		private static Procedure CreateProcedureForInsHist(Patient patient,DateTime date,PrefName prefName) {
			//No need to check MiddleTierRole; no call to db.
			//Create new EO procedure. Default to the patient's clinic, primary provider, and the first code in the InsHistPref
			ProcedureCode procedureCode=ProcedureCodes.GetByInsHistPref(prefName);
			Procedure procedure=new Procedure();
			procedure.CodeNum=procedureCode.CodeNum;
			procedure.PatNum=patient.PatNum;
			procedure.ProcDate=date;
			procedure.DateTP=date;
			procedure.ProcStatus=ProcStat.EO;
			procedure.ProvNum=patient.PriProv;
			procedure.BaseUnits=procedureCode.BaseUnits;
			procedure.RevCode=procedureCode.RevenueCodeDefault;
			if(procedure.ProvNum==0) {
				procedure.ProvNum=patient.SecProv;
			}
			if(procedure.ProvNum==0) {
				procedure.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			procedure.ClinicNum=patient.ClinicNum;
			procedure.SiteNum=patient.SiteNum;
			procedure.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
			procedure.PlaceService=Clinics.GetPlaceService(procedure.ClinicNum);
			procedure.Surf="";
			if(prefName==PrefName.InsHistPerioLLCodes) {
				procedure.Surf="LL";
			}
			else if(prefName==PrefName.InsHistPerioLRCodes) {
				procedure.Surf="LR";
			}
			else if(prefName==PrefName.InsHistPerioURCodes) {
				procedure.Surf="UR";
			}
			else if(prefName==PrefName.InsHistPerioULCodes) {
				procedure.Surf="UL";
			}
			return procedure;
		}

		///<summary>Returns the most recent EO procedure for the InsHist preference CodeNums. Returns null if no precedure is found.</summary>
		public static Procedure GetMostRecentInsHistProc(List<Procedure> listProceduresPatEo,List<long> listCodeNumsInsHist,PrefName prefName) {
			//No need to check MiddleTierRole; no call to db.
			List<Procedure> listProceduresHistCodeNum=listProceduresPatEo.FindAll(x => listCodeNumsInsHist.Contains(x.CodeNum));
			List<Procedure> listProceduresFiltered;
			//For Perio procs, we also need to look at the surface a procedure was done on.
			switch(prefName) {
				case PrefName.InsHistPerioURCodes:
					listProceduresFiltered=listProceduresHistCodeNum.FindAll(x => x.Surf.Contains("UR"));
					break;
				case PrefName.InsHistPerioULCodes:
					listProceduresFiltered=listProceduresHistCodeNum.FindAll(x => x.Surf.Contains("UL"));
					break;
				case PrefName.InsHistPerioLRCodes:
					listProceduresFiltered=listProceduresHistCodeNum.FindAll(x => x.Surf.Contains("LR"));
					break;
				case PrefName.InsHistPerioLLCodes:
					listProceduresFiltered=listProceduresHistCodeNum.FindAll(x => x.Surf.Contains("LL"));
					break;
				default:  //No change in the list.
					listProceduresFiltered=listProceduresHistCodeNum;
					break;
			}
			if(listProceduresFiltered.Count==0) {
				return null;
			}
			//Returns the most recent procedure
			return listProceduresFiltered.OrderBy(x => x.ProcDate).LastOrDefault();
		}

		///<summary>Returns a dictionary for each InsHist pref. Value is the most recent EO or C procedures each InsHist preference. The out param will 
		///have all ClaimProcs associated to the EO and C procedures in the dictionary.</summary>
		public static Dictionary<PrefName,Procedure> GetDictInsHistProcs(long patNum,long insSubNum,out List<ClaimProc> listClaimProcs) {
			//No need to check MiddleTierRole; no call to db.
			Dictionary<PrefName,Procedure> dictionary=new Dictionary<PrefName,Procedure>();
			List<Procedure> listProceduresEoAndC=GetProcsByStatusForPat(patNum,new [] { ProcStat.EO,ProcStat.C });
			//Get claimprocs for EO and C procedures with the plannum passed in. 
			List<ClaimProc> listClaimProcsForEoAndCProcs=ClaimProcs.GetForProcs(listProceduresEoAndC.Select(x=>x.ProcNum).ToList())
				.FindAll(y => y.InsSubNum==insSubNum && y.Status.In(ClaimProcStatus.InsHist,ClaimProcStatus.Received));
			//There could be more than one procedure for each of the InsHist category. For each of the InsHist pref, get the most recent procedure
			listProceduresEoAndC.RemoveAll(x => listClaimProcsForEoAndCProcs.All(y => y.ProcNum!=x.ProcNum));
			List<PrefName> listPrefNames=Prefs.GetInsHistPrefNames();
			for(int i=0;i<listPrefNames.Count;i++) { 
				//get the most recent procedure for the inshist prefs.
				Procedure procedure=GetMostRecentInsHistProc(listProceduresEoAndC,ProcedureCodes.GetCodeNumsForInsHistPref(listPrefNames[i]),listPrefNames[i]);
				dictionary.Add(listPrefNames[i],procedure);
			}
			List<long> listProcNums=dictionary.Values.Where(x => x!=null).Select(x => x.ProcNum).ToList();
			listClaimProcs=listClaimProcsForEoAndCProcs.FindAll(x => listProcNums.Contains(x.ProcNum));//Find all claimprocs for procs in dictionary (used later)
			return dictionary;
		}

		///<summary>Add new/update EO procedure for the patient. New procedures will use the patient's default clinic and provider using the date specified.
		///New procedure will use the first code in the category for the preference passed in.</summary>
		public static void InsertOrUpdateInsHistProcedure(Patient patient,PrefName prefName,DateTime date,long planNum,long insSubNum,Procedure procedure, List<ClaimProc> listClaimProcsForProc)
		{
			//No need to check MiddleTierRole; no call to db.
			//Add a new EO procedure if the proc is null or proc does not have a Status of EO and the new date is greater. 
			if(procedure==null || (procedure.ProcStatus!=ProcStat.EO && date.Date>procedure.ProcDate.Date)) {
				procedure=Procedures.CreateProcedureForInsHist(patient,date,prefName);
				Procedures.Insert(procedure);
				ClaimProcs.InsertClaimProcForInsHist(procedure,planNum,insSubNum);
				Recalls.Synch(patient.PatNum);//A new EO procedure was added, run recall sync.
			}
			//Only Update the date if new date is different than the ProcDate and proc has a Status of EO.
			else if(procedure.ProcStatus==ProcStat.EO && date.Date!=procedure.ProcDate.Date) {
				Procedure procedureOld=procedure.Copy();
				procedure.ProcDate=date;
				Procedures.Update(procedure,procedureOld);
				//We need to update claimprocs or create new claimprocs if we made changes to the EO procedure in this form.
				if(listClaimProcsForProc.Count>0) {
					ClaimProcs.UpdateClaimProcForInsHist(listClaimProcsForProc.FindAll(x => x.ProcDate!=date || x.Status!=ClaimProcStatus.InsHist),date,insSubNum);
				}
				else {
					ClaimProcs.InsertClaimProcForInsHist(procedure,planNum,insSubNum);
				}
			}
		}

		///<summary>Adds and returns a new EO procedure for the patient. New procedures will use the patient's default clinic and provider using the date specified.
		///New procedure will use the first code in the category for the preference passed in.</summary>
		public static Procedure InsertInsHistProcedureForApi(Patient patient,PrefName prefName,DateTime date,long planNum,long insSubNum) {
			Procedure procedure=Procedures.CreateProcedureForInsHist(patient,date,prefName);
			Procedures.Insert(procedure);
			ClaimProcs.InsertClaimProcForInsHist(procedure,planNum,insSubNum);
			Recalls.Synch(patient.PatNum);//A new EO procedure was added, run recall sync.
			return procedure;
		}
		#endregion

		//public static ProcExtended GetProcExtendedEntry(Procedure proc,params ProcAttachTypes[] excludedTypes) {
		//	//No need to check MiddleTierRole; no call to db.
		//	ProcExtended procE = new ProcExtended() {
		//		Proc=proc,
		//		Adjustments=Adjustments.GetForProc(proc.ProcNum),
		//		PaySplits=PaySplits.GetPaySplitsFromProc(proc.ProcNum),
		//		ClaimProcs=ClaimProcs.RefreshForProc(proc.ProcNum),
		//		PayPlanCredits=PayPlanCharges.GetFromProc(proc.ProcNum),
		//		AmountOriginal=proc.ProcFee * (proc.UnitQty + proc.BaseUnits),
		//		ExcludedTypes = excludedTypes.ToList()
		//	};
		//	procE.AmountEnd=procE.AmountStart;
		//	return procE;
		//}

	}

	/*================================================================================================================
	=========================================== class ProcedureComparer =============================================*/

	///<summary>This sorts procedures based on priority, then tooth number, then code (but if Canadian lab code, uses proc code here instead of lab code).  Finally, if comparing a proc and its Canadian lab code, it puts the lab code after the proc.  It does not care about dates or status.  Currently used in TP module only.  The Chart module, Account module, and appointments use Procedurelog.CompareProcedures().</summary>
	public class ProcedureComparer:IComparer {
		///<summary>This sorts procedures based on priority, then tooth number.  It does not care about dates or status.  Currently used in TP module and Chart module sorting.</summary>
		int IComparer.Compare(Object objx,Object objy) {
			Procedure x=(Procedure)objx;
			Procedure y=(Procedure)objy;
			//first, by priority
			if(x.Priority!=y.Priority) {//if priorities are different
				if(x.Priority==0) {
					return 1;//x is greater than y. Priorities always come first.
				}
				if(y.Priority==0) {
					return -1;//x is less than y. Priorities always come first.
				}
				return Defs.GetOrder(DefCat.TxPriorities,x.Priority).CompareTo(Defs.GetOrder(DefCat.TxPriorities,y.Priority));
			}
			//priorities are the same, so sort by toothrange
			if(x.ToothRange!=y.ToothRange) {
				//empty toothranges come before filled toothrange values
				return x.ToothRange.CompareTo(y.ToothRange);
			}
			//toothranges are the same (usually empty), so compare toothnumbers
			if(x.ToothNum!=y.ToothNum) {
				//this also puts invalid or empty toothnumbers before the others.
				return Tooth.ToInt(x.ToothNum).CompareTo(Tooth.ToInt(y.ToothNum));
			}
			//priority and toothnums are the same, so sort by code.
			/*string adaX=x.Code;
			if(x.ProcNumLab !=0){//if x is a Canadian lab proc
				//then use the Code of the procedure instead of the lab code
				adaX=Procedures.GetOneProc(
			}
			string adaY=y.Code;*/
			return ProcedureCodes.GetStringProcCode(x.CodeNum).CompareTo(ProcedureCodes.GetStringProcCode(y.CodeNum));
			//return x.Code.CompareTo(y.Code);
			//return 0;//priority, tooth number, and code are all the same
		}
	}

	///<summary>Helper class that contains properties that give specific results based on data that is currently set.</summary>
	public class ProcExtended {
		private static long _procExtendedAutoIncrementValue = 1;
		///<summary>No matter which constructor is used, the AccountEntryNum will be unique and automatically assigned.</summary>
		public long ProcExtendedEntryNum = (_procExtendedAutoIncrementValue++);
		//Read only data.  Do not modify.
		public Procedure Proc=null;
		//Variables below will be changed as needed.
		public List<Adjustment> Adjustments=new List<Adjustment>();
		public List<PaySplit> PaySplits= new List<PaySplit>();
		public List<ClaimProc> ClaimProcs = new List<ClaimProc>();
		public List<PayPlanCharge> PayPlanCredits = new List<PayPlanCharge>();
		public List<ProcAttachTypes> ExcludedTypes = new List<ProcAttachTypes>();
		public List<PaySplit> SplitsCur = new List<PaySplit>();

		public double NegativeAdjTotals {
			get { return Adjustments.Where(x => x.AdjAmt < 0).Sum(x => x.AdjAmt); }
		}

		public double PositiveAdjTotal {
			get { return Adjustments.Where(x => x.AdjAmt > 0).Sum(x => x.AdjAmt); }
		}

		public double InsPayTotal {
			get { return ClaimProcs.Where(x => OpenDentBusiness.ClaimProcs.GetInsPaidStatuses().Contains(x.Status)).Sum(x => x.InsPayAmt); }
		}

		public double WriteOffTotal {
			get { return ClaimProcs.Where(x => OpenDentBusiness.ClaimProcs.GetInsPaidStatuses().Contains(x.Status)).Sum(x => x.WriteOff); }
		}

		public double WriteOffEstTotal {
			get {
				return ClaimProcs.Where(x => OpenDentBusiness.ClaimProcs.GetEstimatedStatuses().Contains(x.Status))
			.Sum(x => (x.WriteOffEstOverride == -1 ? (x.WriteOffEst == -1 ? 0 : x.WriteOffEst) : x.WriteOffEstOverride));
			}
		}

		public double InsEstTotal {
			get {
				return ClaimProcs.Where(x => OpenDentBusiness.ClaimProcs.GetEstimatedStatuses().Contains(x.Status))
			.Sum(x => (x.InsEstTotalOverride == -1 ? (x.InsEstTotal == -1 ? 0 : x.InsEstTotal) : x.InsEstTotalOverride));
			}
		}

		public double PaySplitTotal {
			//If a split exists for a payment plan procedure, do not count it, the credit is what pays off the procedure. 
			get { return PaySplits.FindAll(x => x.PayPlanNum==0).Sum(x => x.SplitAmt); }
		}

		public double SplitsCurTotal {
			//If a split exists for a payment plan procedure, do not count it, the credit is what pays off the procedure. 
			get { return SplitsCur.FindAll(x => x.PayPlanNum==0).Sum(x => x.SplitAmt); }
		}

		public double PayPlanCreditTotal {
			get { return PayPlanCredits.Sum(x => x.Principal); }
		}

		public double AmountOriginal {
			get {
				return Proc.ProcFeeTotal;
			}
		}

		public double AmountStart {
			get {
				double amt = AmountOriginal;
				if(!ExcludedTypes.Contains(ProcAttachTypes.Adjustments)) {
					amt+=PositiveAdjTotal + NegativeAdjTotals;
				}
				if(!ExcludedTypes.Contains(ProcAttachTypes.InsEsts)) {
					amt-=InsEstTotal;
				}
				if(!ExcludedTypes.Contains(ProcAttachTypes.InsPays)) {
					amt-=InsPayTotal;
				}
				if(!ExcludedTypes.Contains(ProcAttachTypes.PayPlanCredits)) {
					amt-=PayPlanCreditTotal;
				}
				if(!ExcludedTypes.Contains(ProcAttachTypes.PaySplits)) {
					amt-=(PaySplitTotal-SplitsCurTotal);
				}
				if(!ExcludedTypes.Contains(ProcAttachTypes.WriteOffEsts)) {
					amt-=WriteOffEstTotal;
				}
				if(!ExcludedTypes.Contains(ProcAttachTypes.WriteOffs)) {
					amt-=WriteOffTotal;
				}
				return amt;
			}
		}

		public double AmountEnd {
			get {
				return AmountStart - SplitsCurTotal;
			}
		}
	}

	public enum ProcAttachTypes {
		PaySplits,
		Adjustments,
		InsPays,
		InsEsts,
		WriteOffs,
		WriteOffEsts,
		PayPlanCredits,
	}

	public enum CreditCalcType {
		///<summary>Used to be called 'FIFO'.</summary>
		IncludeAll,
		///<summary>Used to be called 'ExplicitOnly'.</summary>
		AllocatedOnly,
		ExcludeAll
	}

	public class ProcedureForApi {
		public Procedure ProcedureCur;
		public DateTime DateTimeServer;
	}

}
