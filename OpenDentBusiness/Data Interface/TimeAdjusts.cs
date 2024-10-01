using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TimeAdjusts {
		#region Get Methods
		///<summary>Attempts to get one TimeAdjust based on a time.  Returns null if not found. </summary>
		public static TimeAdjust GetPayPeriodNote(long employeeNum,DateTime dateStart) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<TimeAdjust>(MethodBase.GetCurrentMethod(),employeeNum,dateStart);
			}
			string command="SELECT * FROM timeadjust WHERE EmployeeNum="+POut.Long(employeeNum)+" AND TimeEntry="+POut.DateT(dateStart)+" AND IsAuto=0 ";
			command+="AND RegHours='00:00:00' AND OTimeHours='00:00:00' AND PtoHours='00:00:00' ";
			return Crud.TimeAdjustCrud.SelectOne(command);
		}

		///<summary>Gets a list of payperiod notes.  Start Date should be the start date of the pay period trying to get notes for.</summary>
		public static List<TimeAdjust> GetNotesForPayPeriod(DateTime dateStart) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),dateStart);
			}
			string command="SELECT * FROM timeadjust WHERE TimeEntry="+POut.DateT(dateStart)+" AND isAuto=0 ";
			command+="AND RegHours='00:00:00' AND OTimeHours='00:00:00' AND PtoHours='00:00:00' ";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}
		#endregion

		///<summary></summary>
		public static List<TimeAdjust> Refresh(long employeeNum,DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),employeeNum,dateFrom,dateTo);
			}
			string command=
				"SELECT * FROM timeadjust WHERE "
				+"EmployeeNum = "+POut.Long(employeeNum)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(dateFrom)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" <= "+POut.Date(dateTo)+" "
				+"ORDER BY TimeEntry";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}

		///<summary>Validates and throws exceptions. Gets all time adjusts between date range and time adjusts made during the current work week. </summary>
		public static List<TimeAdjust> GetValidList(long employeeNum,DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),employeeNum,dateFrom,dateTo);
			}
			List<TimeAdjust> listTimeAdjusts=new List<TimeAdjust>();
			string command=
				"SELECT * FROM timeadjust WHERE "
				+"EmployeeNum = "+POut.Long(employeeNum)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(dateFrom)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" <= "+POut.Date(dateTo)+" "
				+"ORDER BY TimeEntry";
			listTimeAdjusts=Crud.TimeAdjustCrud.SelectMany(command);
			//Validate---------------------------------------------------------------------------------------------------------------
			//none necessary at this time.
			return listTimeAdjusts;
		}

		///<summary></summary>
		public static List<TimeAdjust> GetListForTimeCardManage(long employeeNum,long clinicNum,DateTime dateFrom,DateTime dateTo,bool isAll) {
			Meth.NoCheckMiddleTierRole();
			return GetListForTimeCardManage(new List<long>() { employeeNum },clinicNum,dateFrom,dateTo,isAll);
		}

		///<summary></summary>
		public static List<TimeAdjust> GetListForTimeCardManage(List<long> listEmployeeNums,long clinicNum,DateTime dateFrom,DateTime dateTo,bool isAll) {
			if(listEmployeeNums.IsNullOrEmpty()) {
				return new List<TimeAdjust>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),listEmployeeNums,clinicNum,dateFrom,dateTo,isAll);
			}
			string command="SELECT * FROM timeadjust WHERE "
				+"EmployeeNum IN ("+string.Join(",",listEmployeeNums.Select(x => POut.Long(x)))+") "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(dateFrom)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" <= "+POut.Date(dateTo)+" ";
			if(!isAll) {
				command+="AND ClinicNum = "+POut.Long(clinicNum)+" ";
			}
			command+="ORDER BY TimeEntry";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}

		///<summary>Dates are INCLUSIVE.</summary>
		public static List<TimeAdjust> GetAllForPeriod(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string command= "SELECT * FROM timeadjust "
				+"WHERE TimeEntry >= "+POut.Date(dateFrom)+" "
				+"AND TimeEntry < "+POut.Date(dateTo.AddDays(1))+" ";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(TimeAdjust timeAdjust) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				timeAdjust.TimeAdjustNum=Meth.GetLong(MethodBase.GetCurrentMethod(),timeAdjust);
				return timeAdjust.TimeAdjustNum;
			}
			return Crud.TimeAdjustCrud.Insert(timeAdjust);
		}

		///<summary></summary>
		public static void Update(TimeAdjust timeAdjust) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeAdjust);
				return;
			}
			Crud.TimeAdjustCrud.Update(timeAdjust);
		}

		///<summary></summary>
		public static void Update(TimeAdjust timeAdjust,TimeAdjust timeAdjustOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeAdjust,timeAdjustOld);
				return;
			}
			Crud.TimeAdjustCrud.Update(timeAdjust,timeAdjustOld);
		}

		///<summary></summary>
		public static void Delete(TimeAdjust timeAdjust) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeAdjust);
				return;
			}
			string command= "DELETE FROM timeadjust WHERE TimeAdjustNum = "+POut.Long(timeAdjust.TimeAdjustNum);
			Db.NonQ(command);
		}

		///<summary>Returns all automatically generated timeAdjusts for a given employee between the date range (inclusive).</summary>
		public static List<TimeAdjust> GetSimpleListAuto(long employeeNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),employeeNum,dateStart,dateStop);
			}
			List<TimeAdjust> listTimeAdjusts=new List<TimeAdjust>();
			//List<TimeAdjust> listTimeAdjusts=new List<TimeAdjust>();
			string command=
				"SELECT * FROM timeadjust WHERE "
				+"EmployeeNum = "+POut.Long(employeeNum)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(dateStart)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" < "+POut.Date(dateStop.AddDays(1))+" "//add one day to go the end of the specified date.
				+"AND IsAuto=1";
			//listTimeAdjusts=Crud.TimeAdjustCrud.SelectMany(command);
			return Crud.TimeAdjustCrud.SelectMany(command);
			
		}
	}

	
}




