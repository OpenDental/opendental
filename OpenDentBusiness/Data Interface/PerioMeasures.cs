using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PerioMeasures{
		///<summary>Bad pattern.  List of all perio measures for the current patient. Dim 1 is exams. Dim 2 is Sequences. Dim 3 is Measurements, always 33 per sequence(0 is not used).  This public static variable is only used by the UI.  It's here because it would be complicated to put it in ContrPerio.</summary>
		public static PerioMeasure[,,] List;

		///<summary></summary>
		public static void Update(PerioMeasure Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.PerioMeasureCrud.Update(Cur);
			//3-10-10 A bug that only lasted for a few weeks has resulted in a number of duplicate entries for each tooth.
			//So we need to clean up duplicates as we go.  Might put in db maint later.
			string command="DELETE FROM periomeasure WHERE "
				+ "PerioExamNum = "+POut.Long(Cur.PerioExamNum)
				+" AND SequenceType = "+POut.Long((int)Cur.SequenceType)
				+" AND IntTooth = "+POut.Long(Cur.IntTooth)
				+" AND PerioMeasureNum != "+POut.Long(Cur.PerioMeasureNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(PerioMeasure Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.PerioMeasureNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.PerioMeasureNum;
			}
			return Crud.PerioMeasureCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Delete(PerioMeasure Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command= "DELETE from periomeasure WHERE PerioMeasureNum = '"
				+Cur.PerioMeasureNum.ToString()+"'";
			Db.NonQ(command);
		}

		///<summary>For the current exam, clears existing skipped teeth and resets them to the specified skipped teeth. The ArrayList valid values are 1-32 int.</summary>
		public static void SetSkipped(long perioExamNum,List<int> skippedTeeth) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),perioExamNum,skippedTeeth);
				return;
			}
			//for(int i=0;i<skippedTeeth.Count;i++){
			//MessageBox.Show(skippedTeeth[i].ToString());
			//}
			//first, delete all skipped teeth for this exam
			string command = "DELETE from periomeasure WHERE "
				+"PerioExamNum = "+perioExamNum.ToString()+" "
				+"AND SequenceType = '"+POut.Long((int)PerioSequenceType.SkipTooth)+"'";
			Db.NonQ(command);
			//then add the new ones in one at a time.
			PerioMeasure Cur;
			//There should only be one periomeasure entry per skipped tooth.
			List<int> listDistinctTeeth=skippedTeeth.Distinct().ToList();
			for(int i=0;i<listDistinctTeeth.Count;i++) {
				Cur=new PerioMeasure();
				Cur.PerioExamNum=perioExamNum;
				Cur.SequenceType=PerioSequenceType.SkipTooth;
				Cur.IntTooth=listDistinctTeeth[i];
				Cur.ToothValue=1;
				Cur.MBvalue=-1;
				Cur.Bvalue=-1;
				Cur.DBvalue=-1;
				Cur.MLvalue=-1;
				Cur.Lvalue=-1;
				Cur.DLvalue=-1;
				Insert(Cur);
			}
		}

		///<summary>Used in FormPerio.Add_Click. For the specified exam, gets a list of all skipped teeth. The ArrayList valid values are 1-32 int.</summary>
		public static List<int> GetSkipped(long perioExamNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<int>>(MethodBase.GetCurrentMethod(),perioExamNum);
			}
			string command = "SELECT IntTooth FROM periomeasure WHERE "
				+"SequenceType = '"+POut.Int((int)PerioSequenceType.SkipTooth)+"' "
				+"AND PerioExamNum = '"+perioExamNum.ToString()+"' "
				+"AND ToothValue = '1'";
			DataTable table=Db.GetTable(command);
			List<int> retVal=new List<int>();
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(PIn.Int(table.Rows[i][0].ToString()));
			}
			return retVal;
		}

		///<summary>Gets all measurements for the current patient, then organizes them by exam and sequence.</summary>
		public static void Refresh(long patNum,List<PerioExam> listPerioExams) {
			//No need to check RemotingRole; no call to db.
			DataTable table=GetMeasurementTable(patNum,listPerioExams);
			List=new PerioMeasure[listPerioExams.Count,Enum.GetNames(typeof(PerioSequenceType)).Length,33];
			int examIdx=0;
			//PerioMeasure pm;
			List<PerioMeasure> list=Crud.PerioMeasureCrud.TableToList(table);
			for(int i=0;i<list.Count;i++) {
				//the next statement can also handle exams with no measurements:
				if(i==0//if this is the first row
					|| list[i].PerioExamNum != list[i-1].PerioExamNum)//or examNum has changed
				{
					examIdx=PerioExams.GetExamIndex(listPerioExams,list[i].PerioExamNum);
				}
				List[examIdx,(int)list[i].SequenceType,list[i].IntTooth]=list[i];
			}
		}

		public static DataTable GetMeasurementTable(long patNum,List<PerioExam> listPerioExams) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,listPerioExams);
			}
			string command =
				"SELECT periomeasure.*,perioexam.ExamDate"
				+" FROM periomeasure,perioexam"
				+" WHERE periomeasure.PerioExamNum = perioexam.PerioExamNum"
				+" AND perioexam.PatNum = '"+patNum.ToString()+"'"
				+" ORDER BY perioexam.ExamDate";
			DataTable table=Db.GetTable(command);
			return table;
		}

		public static List<PerioMeasure> GetAllForExam(long perioExamNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PerioMeasure>>(MethodBase.GetCurrentMethod(),perioExamNum);
			}
			string command ="SELECT * FROM periomeasure "
				+"WHERE PerioExamNum = "+POut.Long(perioExamNum);
			return Crud.PerioMeasureCrud.SelectMany(command);
		}

		///<summary>A -1 will be changed to a 0. Measures over 100 are changed to 100-measure. i.e. 100-104=-4 for hyperplastic GM.</summary>
		public static int AdjustGMVal(int measure) {
			//No need to check RemotingRole; no call to db.
			if(measure==-1) {//-1 means no measurement, null.  In the places where this method is used, we have designed it to expect a 0 in those cases.
				return 0;
			}
			else if(measure>100) {
				return 100-measure;
			}
			return measure;//no adjustments needed.
		}		
	}
	
	

}















