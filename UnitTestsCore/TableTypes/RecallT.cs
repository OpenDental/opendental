using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RecallT {

		///<summary></summary>
		public static Recall CreateRecall(long patNum,long recallTypeNum,DateTime dateDue,Interval recallInterval,long recallStatus=0
			,DateTime dateScheduled=new DateTime(),DateTime dateDueCalc=new DateTime(),DateTime datePrevious=new DateTime())
		{
			Recall recall=new Recall();
			recall.DateDue=dateDue;
			recall.DateDueCalc=dateDueCalc;
			recall.DatePrevious=datePrevious;
			recall.DateScheduled=dateScheduled;
			recall.PatNum=patNum;
			recall.RecallInterval=recallInterval;
			recall.RecallStatus=recallStatus;
			recall.RecallTypeNum=recallTypeNum;
			Recalls.Insert(recall);
			return recall;
		}
		
		///<summary>Deletes everything from the recalltype table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRecallTable() {
			string command="DELETE FROM recall WHERE RecallNum > 0";
			DataCore.NonQ(command);
		}
	}
}
