using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RecallT {

		///<summary></summary>
		public static Recall CreateRecall(long patNum,long recallTypeNum,DateTime dateDue,Interval recallInterval,long recallStatus=0,
			DateTime dateScheduled=new DateTime(),DateTime dateDueCalc=new DateTime(),DateTime datePrevious=new DateTime(),string note="",
			bool isDisabled=false,double disableUntilBalance=0,DateTime disableUntilDate=new DateTime(),RecallPriority priority=RecallPriority.Normal,
			string timePatternOverride="")
		{
			Recall recall=new Recall();
			recall.PatNum=patNum;
			recall.DateDueCalc=dateDueCalc;
			recall.DateDue=dateDue;
			recall.DatePrevious=datePrevious;
			recall.RecallInterval=recallInterval;
			recall.RecallStatus=recallStatus;
			recall.Note=note;
			recall.IsDisabled=isDisabled;
			recall.RecallTypeNum=recallTypeNum;
			recall.DisableUntilBalance=disableUntilBalance;
			recall.DisableUntilDate=disableUntilDate;
			recall.DateScheduled=dateScheduled;
			recall.Priority=priority;
			recall.TimePatternOverride=timePatternOverride;
			Recalls.Insert(recall);
			return recall;
		}
		
		///<summary>Deletes everything from the recall table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRecallTable() {
			string command="DELETE FROM recall WHERE RecallNum > 0";
			DataCore.NonQ(command);
		}
	}
}
