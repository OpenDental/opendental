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
		public static TimeAdjust GetPayPeriodNote(long empNum,DateTime startDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<TimeAdjust>(MethodBase.GetCurrentMethod(),empNum,startDate);
			}
			string command="SELECT * FROM timeadjust WHERE EmployeeNum="+POut.Long(empNum)+" AND TimeEntry="+POut.DateT(startDate)+" AND IsAuto=0 ";
			command+="AND RegHours='00:00:00' AND OTimeHours='00:00:00' AND PtoHours='00:00:00' ";
			return Crud.TimeAdjustCrud.SelectOne(command);
		}

		///<summary>Gets a list of payperiod notes.  Start Date should be the start date of the pay period trying to get notes for.</summary>
		public static List<TimeAdjust> GetNotesForPayPeriod(DateTime startDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),startDate);
			}
			string command="SELECT * FROM timeadjust WHERE TimeEntry="+POut.DateT(startDate)+" AND isAuto=0 ";
			command+="AND RegHours='00:00:00' AND OTimeHours='00:00:00' AND PtoHours='00:00:00' ";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}
		#endregion

		///<summary></summary>
		public static List<TimeAdjust> Refresh(long empNum,DateTime fromDate,DateTime toDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),empNum,fromDate,toDate);
			}
			string command=
				"SELECT * FROM timeadjust WHERE "
				+"EmployeeNum = "+POut.Long(empNum)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(fromDate)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" <= "+POut.Date(toDate)+" "
				+"ORDER BY TimeEntry";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}

		///<summary>Validates and throws exceptions. Gets all time adjusts between date range and time adjusts made during the current work week. </summary>
		public static List<TimeAdjust> GetValidList(long empNum,DateTime fromDate,DateTime toDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),empNum,fromDate,toDate);
			}
			List<TimeAdjust> retVal=new List<TimeAdjust>();
			string command=
				"SELECT * FROM timeadjust WHERE "
				+"EmployeeNum = "+POut.Long(empNum)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(fromDate)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" <= "+POut.Date(toDate)+" "
				+"ORDER BY TimeEntry";
			retVal=Crud.TimeAdjustCrud.SelectMany(command);
			//Validate---------------------------------------------------------------------------------------------------------------
			//none necessary at this time.
			return retVal;
		}

		///<summary></summary>
		public static List<TimeAdjust> GetListForTimeCardManage(long empNum,long clinicNum,DateTime fromDate,DateTime toDate,bool isAll) {
			//No need to check RemotingRole; no call to db.
			return GetListForTimeCardManage(new List<long>() { empNum },clinicNum,fromDate,toDate,isAll);
		}

		///<summary></summary>
		public static List<TimeAdjust> GetListForTimeCardManage(List<long> listEmpNums,long clinicNum,DateTime fromDate,DateTime toDate,bool isAll) {
			if(listEmpNums.IsNullOrEmpty()) {
				return new List<TimeAdjust>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),listEmpNums,clinicNum,fromDate,toDate,isAll);
			}
			string command="SELECT * FROM timeadjust WHERE "
				+"EmployeeNum IN ("+string.Join(",",listEmpNums.Select(x => POut.Long(x)))+") "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(fromDate)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" <= "+POut.Date(toDate)+" ";
			if(!isAll) {
				command+="AND ClinicNum = "+POut.Long(clinicNum)+" ";
			}
			command+="ORDER BY TimeEntry";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}

		///<summary>Dates are INCLUSIVE.</summary>
		public static List<TimeAdjust> GetAllForPeriod(DateTime fromDate,DateTime toDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),fromDate,toDate);
			}
			string command= "SELECT * FROM timeadjust "
				+"WHERE TimeEntry >= "+POut.Date(fromDate)+" "
				+"AND TimeEntry < "+POut.Date(toDate.AddDays(1))+" ";
			return Crud.TimeAdjustCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(TimeAdjust timeAdjust) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				timeAdjust.TimeAdjustNum=Meth.GetLong(MethodBase.GetCurrentMethod(),timeAdjust);
				return timeAdjust.TimeAdjustNum;
			}
			return Crud.TimeAdjustCrud.Insert(timeAdjust);
		}

		///<summary></summary>
		public static void Update(TimeAdjust timeAdjust) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeAdjust);
				return;
			}
			Crud.TimeAdjustCrud.Update(timeAdjust);
		}

		///<summary></summary>
		public static void Update(TimeAdjust timeAdjust,TimeAdjust oldTimeAdjust) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeAdjust,oldTimeAdjust);
				return;
			}
			Crud.TimeAdjustCrud.Update(timeAdjust,oldTimeAdjust);
		}

		///<summary></summary>
		public static void Delete(TimeAdjust adj) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),adj);
				return;
			}
			string command= "DELETE FROM timeadjust WHERE TimeAdjustNum = "+POut.Long(adj.TimeAdjustNum);
			Db.NonQ(command);
		}

		///<summary>Returns all automatically generated timeAdjusts for a given employee between the date range (inclusive).</summary>
		public static List<TimeAdjust> GetSimpleListAuto(long employeeNum,DateTime startDate,DateTime stopDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeAdjust>>(MethodBase.GetCurrentMethod(),employeeNum,startDate,stopDate);
			}
			List<TimeAdjust> retVal=new List<TimeAdjust>();
			//List<TimeAdjust> listTimeAdjusts=new List<TimeAdjust>();
			string command=
				"SELECT * FROM timeadjust WHERE "
				+"EmployeeNum = "+POut.Long(employeeNum)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" >= "+POut.Date(startDate)+" "
				+"AND "+DbHelper.DtimeToDate("TimeEntry")+" < "+POut.Date(stopDate.AddDays(1))+" "//add one day to go the end of the specified date.
				+"AND IsAuto=1";
			//listTimeAdjusts=Crud.TimeAdjustCrud.SelectMany(command);
			return Crud.TimeAdjustCrud.SelectMany(command);
			
		}
	}

	
}




