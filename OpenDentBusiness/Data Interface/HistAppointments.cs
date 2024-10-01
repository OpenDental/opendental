using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class HistAppointments{
		#region Get Methods
		///<summary></summary>
		public static List<HistAppointment> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM histappointment WHERE PatNum = "+POut.Long(patNum);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		///<summary>Gets one HistAppointment from the db.</summary>
		public static HistAppointment GetOne(long histApptNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<HistAppointment>(MethodBase.GetCurrentMethod(),histApptNum);
			}
			return Crud.HistAppointmentCrud.SelectOne(histApptNum);
		}

		///<summary>Gets histappointments from database.</summary>
		public static List<HistAppointment> GetHistAppointmentsForApi(int limit,int offset,
			DateTime dateTStart,DateTime dateTEnd,long clinicNum,long patNum,int aptStatus,int histApptAction,long aptNum)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),limit,offset,
					dateTStart,dateTEnd,clinicNum,patNum,aptStatus,histApptAction,aptNum);
			}
			string command="SELECT * FROM histappointment "
				+"WHERE AptDateTime >= "+POut.DateT(dateTStart)+" "
				+"AND AptDateTime < "+POut.DateT(dateTEnd)+" ";
			if(clinicNum>-1) {
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(patNum>0) {
				command+="AND PatNum="+POut.Long(patNum)+" ";
			}
			if(aptStatus>-1) {
				command+="AND AptStatus="+POut.Int(aptStatus)+" ";
			}
			if(histApptAction>-1) {
				command+="AND HistApptAction="+POut.Int(histApptAction)+" ";
			}
			if(aptNum>0) {
				command+="AND AptNum="+POut.Long(aptNum)+" ";
			}
			command+="ORDER BY HistApptNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		public static List<HistAppointment> GetForApt(long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM histappointment WHERE AptNum="+POut.Long(aptNum);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		///<summary>Gets all HistAppointments that have a DateTStamp after dateTimeSince.</summary>
		public static List<HistAppointment> GetChangedSince(DateTime dateTimeSince) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),dateTimeSince);
			}
			string command="SELECT * FROM histappointment WHERE DateTStamp > "+POut.DateT(dateTimeSince);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		///<summary>Gets all AptNums for HistAppointments that have a DateTStamp after dateTimeSince.</summary>
		public static List<long> GetAptNumsChangedSince(DateTime dateTimeSince) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateTimeSince);
			}
			string command="SELECT AptNum FROM histappointment WHERE DateTStamp > "+POut.DateT(dateTimeSince);
			return Db.GetListLong(command);
		}

		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(HistAppointment histAppointment){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				histAppointment.HistApptNum=Meth.GetLong(MethodBase.GetCurrentMethod(),histAppointment);
				return histAppointment.HistApptNum;
			}
			return Crud.HistAppointmentCrud.Insert(histAppointment);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long histApptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),histApptNum);
				return;
			}
			Crud.HistAppointmentCrud.Delete(histApptNum);
		}
		#endregion

		#region Misc Methods
		///<summary>The other overload should be called when the action is Deleted so that the appt's fields will be recorded.</summary>
		public static void CreateHistoryEntry(long apptNum,HistAppointmentAction histAppointmentAction) {
			//No need for additional DB check when appt was already deleted.
			Appointment appointment=null;
			if(histAppointmentAction!=HistAppointmentAction.Deleted){
				appointment=Appointments.GetOneApt(apptNum);
			}
			CreateHistoryEntry(appointment,histAppointmentAction,apptNum);
		}
		
		///<summary>When appt is null you must provide aptNum and HistApptAction will be set to deleted.</summary>
		public static HistAppointment CreateHistoryEntry(Appointment appointment,HistAppointmentAction histAppointmentAction,long aptNum=0) {
			if(Security.CurUser==null) {
				return null;//There is no user currently logged on so do not create a HistAppointment.
			}
			HistAppointment histAppointment=new HistAppointment();
			histAppointment.HistUserNum=Security.CurUser.UserNum;
			histAppointment.ApptSource=Security.CurUser.EServiceType;
			histAppointment.HistApptAction=histAppointmentAction;
			if(appointment!=null) {//Null if deleted
				histAppointment.SetAppt(appointment);
			}
			else {
				histAppointment.AptNum=aptNum;
				histAppointment.HistApptAction=HistAppointmentAction.Deleted;
			}
			HistAppointments.Insert(histAppointment);
			return histAppointment;
		}
		#endregion
	}
}