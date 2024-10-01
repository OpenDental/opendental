using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	public class ProcedureLogic{

		///<summary>The supplied DataRows must include the following columns: ProcStatus(optional),Priority(optional),ToothRange,ToothNum,ProcCode.  This sorts procedures based on priority, then tooth number, then procCode.  It does not care about dates or status.  Currently used in Account module, appointments, and Chart module sorting.  TP uses Procedures.ProcedureComparer.</summary>
		public static int CompareProcedures(DataRow dataRowX,DataRow dataRowY) {
			//first, by status
			if(dataRowX.Table.Columns.Contains("ProcStatus") && dataRowY.Table.Columns.Contains("ProcStatus")) {
				if(dataRowX["ProcStatus"].ToString()!=dataRowY["ProcStatus"].ToString()) {
					//Cn,TP,R,EO,C,EC,D
					//EC procs will draw on top of C procs of same date in the 3D tooth chart, 
					//but this is not a problem since C procs should always have a later date than EC procs.
					//EC must come after C so that group notes will come after their procedures in Progress Notes.
					int idxX=0;
					switch(dataRowX["ProcStatus"].ToString()) {
						case "8"://TPi
							idxX=0;
							break;
						case "7"://Cn
							idxX=1;
							break;
						case "1"://TP
							idxX=2;
							break;
						case "5"://R
							idxX=3;
							break;
						case "4"://EO
							idxX=4;
							break;
						case "2"://C
							idxX=5;
							break;
						case "3"://EC
							idxX=6;
							break;
						case "6"://D
							idxX=7;
							break;
					}
					int idxY=0;
					switch(dataRowY["ProcStatus"].ToString()) {
						case "8"://TPi
							idxY=0;
							break;
						case "7"://Cn
							idxY=1;
							break;
						case "1"://TP
							idxY=2;
							break;
						case "5"://R
							idxY=3;
							break;
						case "4"://EO
							idxY=4;
							break;
						case "2"://C
							idxY=5;
							break;
						case "3"://EC
							idxY=6;
							break;
						case "6"://D
							idxY=7;
							break;
					}
					return idxX.CompareTo(idxY);
				}
			}
			//by priority
			if(dataRowX.Table.Columns.Contains("Priority") && dataRowY.Table.Columns.Contains("Priority")){
				if(dataRowX["Priority"].ToString()!=dataRowY["Priority"].ToString()) {//if priorities are different
					if(dataRowX["Priority"].ToString()=="0") {
						return 1;//x is greater than y. Priorities always come first.
					}
					if(dataRowY["Priority"].ToString()=="0") {
						return -1;//x is less than y. Priorities always come first.
					}
					int defOrderX=Defs.GetOrder(DefCat.TxPriorities,PIn.Long(dataRowX["Priority"].ToString()));
					int defOrderY=Defs.GetOrder(DefCat.TxPriorities,PIn.Long(dataRowY["Priority"].ToString()));
					return defOrderX.CompareTo(defOrderY);
				}
			}
			//priorities are the same, so sort by toothrange
			if(dataRowX["ToothRange"].ToString()!=dataRowY["ToothRange"].ToString()) {
				//empty toothranges come before filled toothrange values
				return dataRowX["ToothRange"].ToString().CompareTo(dataRowY["ToothRange"].ToString());
			}
			//toothranges are the same (usually empty), so compare toothnumbers
			if(dataRowX["ToothNum"].ToString()!=dataRowY["ToothNum"].ToString()) {
				//this also puts invalid or empty toothnumbers before the others.
				return Tooth.ToInt(dataRowX["ToothNum"].ToString()).CompareTo(Tooth.ToInt(dataRowY["ToothNum"].ToString()));
			}
			if(dataRowX["ProcCode"].ToString()!=dataRowY["ProcCode"].ToString()) {
			//priority and toothnums are the same, so sort by proccode if different.
				return dataRowX["ProcCode"].ToString().CompareTo(dataRowY["ProcCode"].ToString());
			}
			//priority, tooth number, and proccode are all the same.  Sort by ProcNum so we have a determinate order if everything else is the same.
			return dataRowX["ProcNum"].ToString().CompareTo(dataRowY["ProcNum"].ToString());
		}

		///<summary>Compares two procedures and returns the order they should appear based on status, priority, toothrange, toothnum, then proccode.
		///Uses the same logic as the other CompareProcedures but takes Procedure objects instead of DataRows.
		///Does not sort Canadian labs correctly.  Make sure there are no Canadian labs present prior to comparing.</summary>
		public static int CompareProcedures(Procedure procedureX,Procedure procedureY) {
			//We cannot sort Canadian labs within this comparer because there can be multiple labs associated to one procedure.
			//This comparer doesn't have enough information in order to sort a procedure and correctly move the corresponding lab(s) with it.
			//Therefore, Canadian labs need to be sorted as an additional step after this comparer has been invoked.
			//=========================================================================================================================
			//if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && x.ProcNumLab!=y.ProcNumLab) {//This code should not impact USA users
			//	int retVal=CanadianLabSortHelper(x,y);
			//	if(retVal!=0) {
			//		return retVal;
			//	}
			//}
			//=========================================================================================================================
			//first by status
			if(procedureX.ProcStatus!=procedureY.ProcStatus) {
				//Cn,TP,R,EO,C,EC,D
				//EC procs will draw on top of C procs of same date in the 3D tooth chart, 
				//but this is not a problem since C procs should always have a later date than EC procs.
				//EC must come after C so that group notes will come after their procedures in Progress Notes.
				int idxX, idxY;
				List<ProcStat> sortOrder=new List<ProcStat> {
					//The order of statuses in this list is very important and determines the sort order for procedures.
					ProcStat.TPi,
					ProcStat.Cn,
					ProcStat.TP,
					ProcStat.R,
					ProcStat.EO,
					ProcStat.C,
					ProcStat.EC,
					ProcStat.D
				};
				idxX=sortOrder.IndexOf(procedureX.ProcStatus);
				idxY=sortOrder.IndexOf(procedureY.ProcStatus);
				return idxX.CompareTo(idxY);
			}
			//by priority
			if(procedureX.Priority!=procedureY.Priority) {//if priorities are different
					if(procedureX.Priority==0) {
						return 1;//x is greater than y. Priorities always come first.
					}
					if(procedureY.Priority==0) {
						return -1;//x is less than y. Priorities always come first.
					}
					return Defs.GetOrder(DefCat.TxPriorities,procedureX.Priority).CompareTo(Defs.GetOrder(DefCat.TxPriorities,procedureY.Priority));
			}
			//priorities are the same, so sort by toothrange
			if(procedureX.ToothRange!=procedureY.ToothRange) {
				//empty toothranges come before filled toothrange values
				return procedureX.ToothRange.CompareTo(procedureY.ToothRange);
			}
			//toothranges are the same (usually empty), so compare toothnumbers
			if(procedureX.ToothNum!=procedureY.ToothNum) {
				//this also puts invalid or empty toothnumbers before the others.
				return Tooth.ToInt(procedureX.ToothNum).CompareTo(Tooth.ToInt(procedureY.ToothNum));
			}
			//priority and toothnums are the same, so sort by proccode.
			if(procedureX.CodeNum!=procedureY.CodeNum) {
				//GetProcCode(...).ProcCode can be null.
				//We do not protect the second call because comparing any string to null doesn't cause an error.
				string procCode=ProcedureCodes.GetProcCode(procedureX.CodeNum).ProcCode??"";
				return procCode.CompareTo(ProcedureCodes.GetProcCode(procedureY.CodeNum).ProcCode);
			}
			//if everything else is the same, sort by ProcNum so sort is deterministic
			return procedureX.ProcNum.CompareTo(procedureY.ProcNum);
		}

		///<summary>Sorts the list of procedures passed in via CompareProcedures.  Correctly sorts Canadian labs if needed.</summary>
		public static void SortProcedures(List<Procedure> listProcedures) {
			//Keep track of all Canadian labs which will be manually re-inserted underneath their corresponding parent procedure.
			List<Procedure> listProceduresLab=listProcedures.FindAll(x => x.ProcNumLab!=0);
			//Remove all labs from the list prior to sorting.  Labs are always below their parent proc.
			listProcedures.RemoveAll(x => x.ProcNumLab!=0);
			//Sort the list of procedures that are missing Canadian labs.
			listProcedures.Sort(CompareProcedures);
			//Loop backward so we can insert as we go without affecting the index.
			for(int i=listProcedures.Count-1;i>=0;i--) {
				List<Procedure> listProceduresChildren=listProceduresLab.FindAll(x => x.ProcNumLab==listProcedures[i].ProcNum);
				listProcedures.InsertRange(i+1,listProceduresChildren);//Insert labs below parent proc.
			}
		}
	}
}
