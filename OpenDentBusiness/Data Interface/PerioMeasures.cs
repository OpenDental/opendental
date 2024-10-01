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
		public static List<PerioMeasure> GetForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PerioMeasure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command =
				"SELECT periomeasure.*"
				+" FROM periomeasure,perioexam"
				+" WHERE periomeasure.PerioExamNum = perioexam.PerioExamNum"
				+" AND perioexam.PatNum = "+POut.Long(patNum);
			return Crud.PerioMeasureCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(PerioMeasure perioMeasure){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),perioMeasure);
				return;
			}
			Crud.PerioMeasureCrud.Update(perioMeasure);
			//3-10-10 A bug that only lasted for a few weeks has resulted in a number of duplicate entries for each tooth.
			//So we need to clean up duplicates as we go.  Might put in db maint later.
			string command="DELETE FROM periomeasure WHERE "
				+ "PerioExamNum = "+POut.Long(perioMeasure.PerioExamNum)
				+" AND SequenceType = "+POut.Long((int)perioMeasure.SequenceType)
				+" AND IntTooth = "+POut.Long(perioMeasure.IntTooth)
				+" AND PerioMeasureNum != "+POut.Long(perioMeasure.PerioMeasureNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(PerioMeasure perioMeasure) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				perioMeasure.PerioMeasureNum=Meth.GetLong(MethodBase.GetCurrentMethod(),perioMeasure);
				return perioMeasure.PerioMeasureNum;
			}
			return Crud.PerioMeasureCrud.Insert(perioMeasure);
		}

		///<summary></summary>
		public static void InsertMany(List<PerioMeasure> listPerioMeasures) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPerioMeasures);
				return;
			}
			Crud.PerioMeasureCrud.InsertMany(listPerioMeasures);
		}

		///<summary></summary>
		public static void Delete(PerioMeasure perioMeasure){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),perioMeasure);
				return;
			}
			string command= "DELETE from periomeasure WHERE PerioMeasureNum = '"
				+perioMeasure.PerioMeasureNum.ToString()+"'";
			Db.NonQ(command);
		}

		public static bool Sync(List<PerioMeasure> listPerioMeasuresNew,List<PerioMeasure> listPerioMeasuresOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listPerioMeasuresNew,listPerioMeasuresOld);
			}
			return Crud.PerioMeasureCrud.Sync(listPerioMeasuresNew,listPerioMeasuresOld);
		}

		///<summary>For the current exam, clears existing skipped teeth and resets them to the specified skipped teeth. The ArrayList valid values are 1-32 int.</summary>
		public static void SetSkipped(long perioExamNum,List<int> listSkippedTeeth) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),perioExamNum,listSkippedTeeth);
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
			PerioMeasure perioMeasure;
			//There should only be one periomeasure entry per skipped tooth.
			List<int> listDistinctTeeth=listSkippedTeeth.Distinct().ToList();
			for(int i=0;i<listDistinctTeeth.Count;i++) {
				perioMeasure=new PerioMeasure();
				perioMeasure.PerioExamNum=perioExamNum;
				perioMeasure.SequenceType=PerioSequenceType.SkipTooth;
				perioMeasure.IntTooth=listDistinctTeeth[i];
				perioMeasure.ToothValue=1;
				perioMeasure.MBvalue=-1;
				perioMeasure.Bvalue=-1;
				perioMeasure.DBvalue=-1;
				perioMeasure.MLvalue=-1;
				perioMeasure.Lvalue=-1;
				perioMeasure.DLvalue=-1;
				Insert(perioMeasure);
			}
		}

		///<summary>Used in FormPerio.Add_Click. For the specified exam, gets a list of all skipped teeth. The ArrayList valid values are 1-32 int.</summary>
		public static List<int> GetSkipped(long perioExamNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<int>>(MethodBase.GetCurrentMethod(),perioExamNum);
			}
			string command = "SELECT IntTooth FROM periomeasure WHERE "
				+"SequenceType = '"+POut.Int((int)PerioSequenceType.SkipTooth)+"' "
				+"AND PerioExamNum = '"+perioExamNum.ToString()+"' "
				+"AND ToothValue = '1'";
			DataTable tableSkippedTeeth=Db.GetTable(command);
			List<int> listSkippedTeeth=new List<int>();
			for(int i=0;i<tableSkippedTeeth.Rows.Count;i++){
				listSkippedTeeth.Add(PIn.Int(tableSkippedTeeth.Rows[i][0].ToString()));
			}
			return listSkippedTeeth;
		}

		///<summary>Get a list of periomeasures from the db.</summary>
		public static List<PerioMeasure> GetPerioMeasuresForApi(int limit,int offset,long perioExamNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PerioMeasure>>(MethodBase.GetCurrentMethod(),limit,offset,perioExamNum);
			}
			string command="SELECT * FROM periomeasure ";
			if(perioExamNum>0) {
				command+="WHERE PerioExamNum="+POut.Long(perioExamNum)+" ";
			}
			command+="ORDER BY PerioMeasureNum "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit)+" ";
			return Crud.PerioMeasureCrud.SelectMany(command);
		}

		///<summary>Gets a PerioMeasure by PerioMeasureNum from the database. Returns null if not found.</summary>
		public static PerioMeasure GetOne(long perioMeasureNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PerioMeasure>(MethodBase.GetCurrentMethod(),perioMeasureNum);
			}
			string command="SELECT * FROM periomeasure "
				+"WHERE PerioMeasureNum = "+POut.Long(perioMeasureNum);
			return Crud.PerioMeasureCrud.SelectOne(command);
		}
		
		public static List<PerioMeasure> GetAllForExam(long perioExamNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PerioMeasure>>(MethodBase.GetCurrentMethod(),perioExamNum);
			}
			string command ="SELECT * FROM periomeasure "
				+"WHERE PerioExamNum = "+POut.Long(perioExamNum);
			return Crud.PerioMeasureCrud.SelectMany(command);
		}

		///<summary>A -1 will be changed to a 0. Measures over 100 are changed to 100-measure. i.e. 100-104=-4 for hyperplastic GM.</summary>
		public static int AdjustGMVal(int measure) {
			Meth.NoCheckMiddleTierRole();
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















