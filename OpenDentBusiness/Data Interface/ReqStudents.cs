using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
///<summary></summary>
	public class ReqStudents {
		///<summary>Returns an empty list if the aptNum passed in is 0.</summary>
		public static List<ReqStudent> GetForAppt(long aptNum) {
			if(aptNum==0) {
				return new List<ReqStudent>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ReqStudent>>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM reqstudent WHERE AptNum="+POut.Long(aptNum)+" ORDER BY ProvNum,Descript";
			return Crud.ReqStudentCrud.SelectMany(command);
		}

		public static ReqStudent GetOne(long reqStudentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ReqStudent>(MethodBase.GetCurrentMethod(),reqStudentNum);
			}
			string command="SELECT * FROM reqstudent WHERE ReqStudentNum="+POut.Long(reqStudentNum);
			return Crud.ReqStudentCrud.SelectOne(reqStudentNum);
		}

		///<summary></summary>
		public static void Update(ReqStudent reqStudent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reqStudent);
				return;
			}
			Crud.ReqStudentCrud.Update(reqStudent);
		}

		///<summary></summary>
		public static long Insert(ReqStudent reqStudent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				reqStudent.ReqStudentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),reqStudent);
				return reqStudent.ReqStudentNum;
			}
			return Crud.ReqStudentCrud.Insert(reqStudent);
		}

		///<summary>Surround with try/catch.</summary>
		public static void Delete(long reqStudentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reqStudentNum);
				return;
			}
			ReqStudent reqStudent=GetOne(reqStudentNum);
			//if a reqneeded exists, then disallow deletion.
			if(ReqNeededs.GetReq(reqStudent.ReqNeededNum)==null) {
				throw new Exception(Lans.g("ReqStudents","Cannot delete requirement.  Delete the requirement needed instead."));
			}
			string command= "DELETE FROM reqstudent WHERE ReqStudentNum = "+POut.Long(reqStudentNum);
			Db.NonQ(command);
		}

		public static DataTable RefreshOneStudent(long provNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),provNum);
			}
			DataTable table=new DataTable();
			DataRow dataRow;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("appointment");
			table.Columns.Add("course");
			table.Columns.Add("done");
			table.Columns.Add("patient");
			table.Columns.Add("ReqStudentNum");
			table.Columns.Add("requirement");
			string command="SELECT AptDateTime,CourseID,reqstudent.Descript ReqDescript,"
				+"schoolcourse.Descript CourseDescript,reqstudent.DateCompleted, "
				+"patient.LName,patient.FName,patient.MiddleI,patient.Preferred,ProcDescript,reqstudent.ReqStudentNum "
				+"FROM reqstudent "
				+"LEFT JOIN schoolcourse ON reqstudent.SchoolCourseNum=schoolcourse.SchoolCourseNum "
				+"LEFT JOIN patient ON reqstudent.PatNum=patient.PatNum "
				+"LEFT JOIN appointment ON reqstudent.AptNum=appointment.AptNum "
				+"WHERE reqstudent.ProvNum="+POut.Long(provNum)
				+" ORDER BY CourseID,ReqDescript";
			DataTable tableRaw=Db.GetTable(command);
			DateTime dateTimeApt;
			DateTime dateCompleted;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				dateTimeApt=PIn.DateT(tableRaw.Rows[i]["AptDateTime"].ToString());
				if(dateTimeApt.Year>1880){
					dataRow["appointment"]=dateTimeApt.ToShortDateString()+" "+dateTimeApt.ToShortTimeString()
						+" "+tableRaw.Rows[i]["ProcDescript"].ToString();
				}
				dataRow["course"]=tableRaw.Rows[i]["CourseID"].ToString();//+" "+raw.Rows[i]["CourseDescript"].ToString();
				dateCompleted=PIn.Date(tableRaw.Rows[i]["DateCompleted"].ToString());
				if(dateCompleted.Year>1880){
					dataRow["done"]="X";
				}
				dataRow["patient"]=PatientLogic.GetNameLF(tableRaw.Rows[i]["LName"].ToString(),tableRaw.Rows[i]["FName"].ToString(),
					tableRaw.Rows[i]["Preferred"].ToString(),tableRaw.Rows[i]["MiddleI"].ToString());
				dataRow["ReqStudentNum"]=tableRaw.Rows[i]["ReqStudentNum"].ToString();
				dataRow["requirement"]=tableRaw.Rows[i]["ReqDescript"].ToString();
				table.Rows.Add(dataRow);
			}
			return table;
		}

		public static DataTable RefreshManyStudents(long schoolClassNum,long schoolCourseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),schoolClassNum,schoolCourseNum);
			}
			DataTable table=new DataTable();
			DataRow dataRow;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("donereq");
			table.Columns.Add("FName");
			table.Columns.Add("LName");
			table.Columns.Add("studentNum");//ProvNum
			table.Columns.Add("totalreq");//not used yet.  It will be changed to be based upon reqneeded. Or not used at all.
			string command="SELECT COUNT(DISTINCT req2.ReqStudentNum) donereq,FName,LName,provider.ProvNum,"
				+"COUNT(DISTINCT req1.ReqStudentNum) totalreq "
				+"FROM provider "
				+"LEFT JOIN reqstudent req1 ON req1.ProvNum=provider.ProvNum AND req1.SchoolCourseNum="+POut.Long(schoolCourseNum)+" "
				+"LEFT JOIN reqstudent req2 ON req2.ProvNum=provider.ProvNum AND "+DbHelper.Year("req2.DateCompleted")+" > 1880 "
				+"AND req2.SchoolCourseNum="+POut.Long(schoolCourseNum)+" "
				+"WHERE provider.SchoolClassNum="+POut.Long(schoolClassNum)
				+" GROUP BY FName,LName,provider.ProvNum "
				+"ORDER BY LName,FName";
			DataTable tableRaw=Db.GetTable(command);
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				dataRow["donereq"]=tableRaw.Rows[i]["donereq"].ToString();
				dataRow["FName"]=tableRaw.Rows[i]["FName"].ToString();
				dataRow["LName"]=tableRaw.Rows[i]["LName"].ToString();
				dataRow["studentNum"]=tableRaw.Rows[i]["ProvNum"].ToString();
				dataRow["totalreq"]=tableRaw.Rows[i]["totalreq"].ToString();
				table.Rows.Add(dataRow);
			}
			return table;
		}

		public static List<Provider> GetStudents(long schoolClassNum) {
			//No need to check MiddleTierRole; no call to db.
			return Providers.GetWhere(x => x.SchoolClassNum==schoolClassNum,true);
		}

		///<summary>Provider(student) is required.</summary>
		public static DataTable GetForCourseClass(long schoolCourseNum,long schoolClassNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),schoolCourseNum,schoolClassNum);
			}
			string command="SELECT Descript,ReqNeededNum "
				+"FROM reqneeded ";
			//if(schoolCourse==0){
			//	command+="WHERE ProvNum="+POut.PInt(provNum);
			//}
			//else{
				command+="WHERE SchoolCourseNum="+POut.Long(schoolCourseNum)
					//+" AND ProvNum="+POut.PInt(provNum);
			//}
			+" AND SchoolClassNum="+POut.Long(schoolClassNum);
			command+=" ORDER BY Descript";
			return Db.GetTable(command);
		}

		
		///<summary>All fields for all reqs will have already been set.  All except for reqstudent.ReqStudentNum if new.  Now, they just have to be persisted to the database.</summary>
		public static void SynchApt(List<ReqStudent> listReqStudentsAttached,List<ReqStudent> listReqStudentsRemoved,long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listReqStudentsAttached,listReqStudentsRemoved,aptNum);
				return;
			}
			string command;
			//first, delete all that were removed from this appt
			if(listReqStudentsRemoved.Count(x => x.ReqStudentNum != 0) > 0) {
				command="DELETE FROM reqstudent WHERE ReqStudentNum IN("+string.Join(",",listReqStudentsRemoved.Where(x => x.ReqStudentNum != 0)
					.Select(x => x.ReqStudentNum))+")";
				Db.NonQ(command);
			}
			//second, detach all from this appt
			command="UPDATE reqstudent SET AptNum=0 WHERE AptNum="+POut.Long(aptNum);
			Db.NonQ(command);
			if(listReqStudentsAttached.Count==0) {
				return;
			}
			for(int i=0;i<listReqStudentsAttached.Count;i++){
				if(listReqStudentsAttached[i].ReqStudentNum==0){
					ReqStudents.Insert(listReqStudentsAttached[i]);
				}
				else{
					ReqStudents.Update(listReqStudentsAttached[i]);
				}
			}
		}

		///<summary>Before reqneeded.Delete, this checks to make sure that req is not in use by students.  Used to prompt user.</summary>
		public static string InUseBy(long reqNeededNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),reqNeededNum);
			}
			string command="SELECT LName,FName FROM provider,reqstudent "
				+"WHERE provider.ProvNum=reqstudent.ProvNum "
				+"AND reqstudent.ReqNeededNum="+POut.Long(reqNeededNum)
				+" AND reqstudent.DateCompleted > "+POut.Date(new DateTime(1880,1,1));
			DataTable table=Db.GetTable(command);
			string retVal="";
			for(int i=0;i<table.Rows.Count;i++){
				retVal+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString()+"\r\n";
			}
			return retVal;
		}

		/*
		///<summary>Attaches a req to an appointment.  Importantly, it also sets the patNum to match the apt.</summary>
		public static void AttachToApt(int reqStudentNum,int aptNum) {
			string command="SELECT PatNum FROM appointment WHERE AptNum="+POut.PInt(aptNum);
			string patNum=Db.GetCount(command);
			command="UPDATE reqstudent SET AptNum="+POut.PInt(aptNum)
				+", PatNum="+patNum
				+" WHERE ReqStudentNum="+POut.PInt(reqStudentNum);
			Db.NonQ(command);
		}*/
	}

}