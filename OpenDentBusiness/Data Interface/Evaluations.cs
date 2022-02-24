using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Evaluations{
		/*
		///<summary></summary>
		public static List<Evaluation> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Evaluation>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM evaluation WHERE PatNum = "+POut.Long(patNum);
			return Crud.EvaluationCrud.SelectMany(command);
		}
		*/
		///<summary>Gets one Evaluation from the db.</summary>
		public static Evaluation GetOne(long evaluationNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Evaluation>(MethodBase.GetCurrentMethod(),evaluationNum);
			}
			return Crud.EvaluationCrud.SelectOne(evaluationNum);
		}

		///<summary>Gets all Evaluations from the DB.  Multiple filters are available.  Dates must be valid before calling this.</summary>
		public static DataTable GetFilteredList(DateTime dateStart,DateTime dateEnd,string lastName,string firstName,long uniqueID,long courseNum,long instructorNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,lastName,firstName,uniqueID,courseNum,instructorNum);
			}
			string command="SELECT evaluation.EvaluationNum,evaluation.EvalTitle,evaluation.DateEval,evaluation.StudentNum,evaluation.InstructNum,"
			+"stu.LName,stu.FName,schoolcourse.CourseID,gradingscale.Description,evaluation.OverallGradeShowing FROM evaluation "
				+"INNER JOIN provider ins ON ins.ProvNum=evaluation.InstructNum "
				+"INNER JOIN provider stu ON stu.ProvNum=evaluation.StudentNum "
				+"INNER JOIN schoolcourse ON schoolcourse.SchoolCourseNum=evaluation.SchoolCourseNum "
				+"INNER JOIN gradingscale ON gradingscale.GradingScaleNum=evaluation.GradingScaleNum "
				+"WHERE TRUE";
			if(!String.IsNullOrWhiteSpace(lastName)) {
				command+=" AND stu.LName LIKE '%"+POut.String(lastName)+"%'";
			}
			if(!String.IsNullOrWhiteSpace(firstName)) {
				command+=" AND stu.FName LIKE '%"+POut.String(firstName)+"%'";
			}
			if(uniqueID!=0) {
				command+=" AND evaluation.StudentNum = '"+POut.Long(uniqueID)+"'";
			}
			if(courseNum!=0) {
				command+=" AND schoolcourse.SchoolCourseNum = '"+POut.Long(courseNum)+"'";
			}
			if(instructorNum!=0) {
				command+=" AND evaluation.InstructNum = '"+POut.Long(instructorNum)+"'";
			}
			command+=" AND evaluation.DateEval BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
			command+=" ORDER BY DateEval,LName";
			return Db.GetTable(command);
		}

		///<summary>Gets all Evaluations from the DB.  List filters are available.</summary>
		public static DataTable GetFilteredList(List<long> courseNums,List<long> instructorNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),courseNums,instructorNums);
			}
			string command="SELECT DISTINCT evaluation.StudentNum,stu.LName,stu.FName FROM evaluation "
				+"INNER JOIN provider ins ON ins.ProvNum=evaluation.InstructNum "
				+"INNER JOIN provider stu ON stu.ProvNum=evaluation.StudentNum "
				+"INNER JOIN schoolcourse ON schoolcourse.SchoolCourseNum=evaluation.SchoolCourseNum "
				+"WHERE TRUE";
			if(courseNums!=null && courseNums.Count!=0) {
				command+=" AND schoolcourse.SchoolCourseNum IN (";
				for(int i=0;i<courseNums.Count;i++) {
					command+="'"+POut.Long(courseNums[i])+"'";
					if(i!=courseNums.Count-1) {
						command+=",";
						continue;
					}
					command+=")";
				}
			}
			if(instructorNums!=null && instructorNums.Count!=0) {
				command+=" AND ins.ProvNum IN (";
				for(int i=0;i<instructorNums.Count;i++) {
					command+="'"+POut.Long(instructorNums[i])+"'";
					if(i!=instructorNums.Count-1) {
						command+=",";
						continue;
					}
					command+=")";
				}
			}
			command+=" ORDER BY LName,FName";
			return Db.GetTable(command);
		}

		///<summary></summary>
		public static long Insert(Evaluation evaluation){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				evaluation.EvaluationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),evaluation);
				return evaluation.EvaluationNum;
			}
			return Crud.EvaluationCrud.Insert(evaluation);
		}

		///<summary></summary>
		public static void Update(Evaluation evaluation){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),evaluation);
				return;
			}
			Crud.EvaluationCrud.Update(evaluation);
		}

		///<summary></summary>
		public static void Delete(long evaluationNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),evaluationNum);
				return;
			}
			string command= "DELETE FROM evaluation WHERE EvaluationNum = "+POut.Long(evaluationNum);
			Db.NonQ(command);
		}
	}
}