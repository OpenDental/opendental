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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReqStudent>>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM reqstudent WHERE AptNum="+POut.Long(aptNum)+" ORDER BY ProvNum,Descript";
			return Crud.ReqStudentCrud.SelectMany(command);
		}

		public static ReqStudent GetOne(long ReqStudentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ReqStudent>(MethodBase.GetCurrentMethod(),ReqStudentNum);
			}
			string command="SELECT * FROM reqstudent WHERE ReqStudentNum="+POut.Long(ReqStudentNum);
			return Crud.ReqStudentCrud.SelectOne(ReqStudentNum);
		}

		///<summary></summary>
		public static void Update(ReqStudent req) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),req);
				return;
			}
			Crud.ReqStudentCrud.Update(req);
		}

		///<summary></summary>
		public static long Insert(ReqStudent req) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				req.ReqStudentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),req);
				return req.ReqStudentNum;
			}
			return Crud.ReqStudentCrud.Insert(req);
		}

		///<summary>Surround with try/catch.</summary>
		public static void Delete(long reqStudentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reqStudentNum);
				return;
			}
			ReqStudent req=GetOne(reqStudentNum);
			//if a reqneeded exists, then disallow deletion.
			if(ReqNeededs.GetReq(req.ReqNeededNum)==null) {
				throw new Exception(Lans.g("ReqStudents","Cannot delete requirement.  Delete the requirement needed instead."));
			}
			string command= "DELETE FROM reqstudent WHERE ReqStudentNum = "+POut.Long(reqStudentNum);
			Db.NonQ(command);
		}

		public static DataTable RefreshOneStudent(long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),provNum);
			}
			DataTable table=new DataTable();
			DataRow row;
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
			DataTable raw=Db.GetTable(command);
			DateTime AptDateTime;
			DateTime dateCompleted;
			for(int i=0;i<raw.Rows.Count;i++) {
				row=table.NewRow();
				AptDateTime=PIn.DateT(raw.Rows[i]["AptDateTime"].ToString());
				if(AptDateTime.Year>1880){
					row["appointment"]=AptDateTime.ToShortDateString()+" "+AptDateTime.ToShortTimeString()
						+" "+raw.Rows[i]["ProcDescript"].ToString();
				}
				row["course"]=raw.Rows[i]["CourseID"].ToString();//+" "+raw.Rows[i]["CourseDescript"].ToString();
				dateCompleted=PIn.Date(raw.Rows[i]["DateCompleted"].ToString());
				if(dateCompleted.Year>1880){
					row["done"]="X";
				}
				row["patient"]=PatientLogic.GetNameLF(raw.Rows[i]["LName"].ToString(),raw.Rows[i]["FName"].ToString(),
					raw.Rows[i]["Preferred"].ToString(),raw.Rows[i]["MiddleI"].ToString());
				row["ReqStudentNum"]=raw.Rows[i]["ReqStudentNum"].ToString();
				row["requirement"]=raw.Rows[i]["ReqDescript"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		public static DataTable RefreshManyStudents(long classNum,long courseNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),classNum,courseNum);
			}
			DataTable table=new DataTable();
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("donereq");
			table.Columns.Add("FName");
			table.Columns.Add("LName");
			table.Columns.Add("studentNum");//ProvNum
			table.Columns.Add("totalreq");//not used yet.  It will be changed to be based upon reqneeded. Or not used at all.
			string command="SELECT COUNT(DISTINCT req2.ReqStudentNum) donereq,FName,LName,provider.ProvNum,"
				+"COUNT(DISTINCT req1.ReqStudentNum) totalreq "
				+"FROM provider "
				+"LEFT JOIN reqstudent req1 ON req1.ProvNum=provider.ProvNum AND req1.SchoolCourseNum="+POut.Long(courseNum)+" "
				+"LEFT JOIN reqstudent req2 ON req2.ProvNum=provider.ProvNum AND "+DbHelper.Year("req2.DateCompleted")+" > 1880 "
				+"AND req2.SchoolCourseNum="+POut.Long(courseNum)+" "
				+"WHERE provider.SchoolClassNum="+POut.Long(classNum)
				+" GROUP BY FName,LName,provider.ProvNum "
				+"ORDER BY LName,FName";
			DataTable raw=Db.GetTable(command);
			for(int i=0;i<raw.Rows.Count;i++) {
				row=table.NewRow();
				row["donereq"]=raw.Rows[i]["donereq"].ToString();
				row["FName"]=raw.Rows[i]["FName"].ToString();
				row["LName"]=raw.Rows[i]["LName"].ToString();
				row["studentNum"]=raw.Rows[i]["ProvNum"].ToString();
				row["totalreq"]=raw.Rows[i]["totalreq"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		public static List<Provider> GetStudents(long classNum) {
			//No need to check RemotingRole; no call to db.
			return Providers.GetWhere(x => x.SchoolClassNum==classNum,true);
		}

		///<summary>Provider(student) is required.</summary>
		public static DataTable GetForCourseClass(long schoolCourse,long schoolClass) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),schoolCourse,schoolClass);
			}
			string command="SELECT Descript,ReqNeededNum "
				+"FROM reqneeded ";
			//if(schoolCourse==0){
			//	command+="WHERE ProvNum="+POut.PInt(provNum);
			//}
			//else{
				command+="WHERE SchoolCourseNum="+POut.Long(schoolCourse)
					//+" AND ProvNum="+POut.PInt(provNum);
			//}
			+" AND SchoolClassNum="+POut.Long(schoolClass);
			command+=" ORDER BY Descript";
			return Db.GetTable(command);
		}

		
		///<summary>All fields for all reqs will have already been set.  All except for reqstudent.ReqStudentNum if new.  Now, they just have to be persisted to the database.</summary>
		public static void SynchApt(List<ReqStudent> listReqsAttached,List<ReqStudent> listReqsRemoved,long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listReqsAttached,listReqsRemoved,aptNum);
				return;
			}
			string command;
			//first, delete all that were removed from this appt
			if(listReqsRemoved.Count(x => x.ReqStudentNum != 0) > 0) {
				command="DELETE FROM reqstudent WHERE ReqStudentNum IN("+string.Join(",",listReqsRemoved.Where(x => x.ReqStudentNum != 0)
					.Select(x => x.ReqStudentNum))+")";
				Db.NonQ(command);
			}
			//second, detach all from this appt
			command="UPDATE reqstudent SET AptNum=0 WHERE AptNum="+POut.Long(aptNum);
			Db.NonQ(command);
			if(listReqsAttached.Count==0) {
				return;
			}
			for(int i=0;i<listReqsAttached.Count;i++){
				if(listReqsAttached[i].ReqStudentNum==0){
					ReqStudents.Insert(listReqsAttached[i]);
				}
				else{
					ReqStudents.Update(listReqsAttached[i]);
				}
			}
		}

		///<summary>Before reqneeded.Delete, this checks to make sure that req is not in use by students.  Used to prompt user.</summary>
		public static string InUseBy(long reqNeededNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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













