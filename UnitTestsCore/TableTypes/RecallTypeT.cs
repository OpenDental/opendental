using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RecallTypeT {

		///<summary></summary>
		public static RecallType CreateRecallType(string description="Cleaning",string procedures="D1110,D0150",string timePattern="///X///",
			Interval defaultInterval=default(Interval)) 
		{
			RecallType recallType=new RecallType();
			if(defaultInterval==default(Interval)) {
				recallType.DefaultInterval=new Interval(1,0,6,0);
			}
			else {
				recallType.DefaultInterval=defaultInterval;
			}
			recallType.Description=description;
			recallType.Procedures=procedures;
			recallType.TimePattern=timePattern;
			RecallTypes.Insert(recallType);
			RecallTypes.RefreshCache();
			foreach(string procStr in procedures.Split(',')) {
				ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
				RecallTriggers.Insert(new RecallTrigger {
					CodeNum=procCode.CodeNum,
					RecallTypeNum=recallType.RecallTypeNum
				});
			}
			return recallType;
		}

		///<summary>Deletes everything from the recalltype table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRecallTypeTable() {
			string command="DELETE FROM recalltype WHERE RecallTypeNum > 0";
			DataCore.NonQ(command);
		}
	}
}
