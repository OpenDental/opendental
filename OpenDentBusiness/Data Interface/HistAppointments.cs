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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM histappointment WHERE PatNum = "+POut.Long(patNum);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		///<summary>Gets one HistAppointment from the db.</summary>
		public static HistAppointment GetOne(long histApptNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<HistAppointment>(MethodBase.GetCurrentMethod(),histApptNum);
			}
			return Crud.HistAppointmentCrud.SelectOne(histApptNum);
		}

		public static List<HistAppointment> GetForApt(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM histappointment WHERE AptNum="+POut.Long(aptNum);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		///<summary>Gets all HistAppointments that have a DateTStamp after dateTimeSince.</summary>
		public static List<HistAppointment> GetChangedSince(DateTime dateTimeSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HistAppointment>>(MethodBase.GetCurrentMethod(),dateTimeSince);
			}
			string command="SELECT * FROM histappointment WHERE DateTStamp > "+POut.DateT(dateTimeSince);
			return Crud.HistAppointmentCrud.SelectMany(command);
		}

		///<summary>Gets all AptNums for HistAppointments that have a DateTStamp after dateTimeSince.</summary>
		public static List<long> GetAptNumsChangedSince(DateTime dateTimeSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateTimeSince);
			}
			string command="SELECT AptNum FROM histappointment WHERE DateTStamp > "+POut.DateT(dateTimeSince);
			return Db.GetListLong(command);
		}

		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(HistAppointment histAppointment){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				histAppointment.HistApptNum=Meth.GetLong(MethodBase.GetCurrentMethod(),histAppointment);
				return histAppointment.HistApptNum;
			}
			return Crud.HistAppointmentCrud.Insert(histAppointment);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long histApptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),histApptNum);
				return;
			}
			Crud.HistAppointmentCrud.Delete(histApptNum);
		}
		#endregion

		#region Misc Methods
		///<summary>The other overload should be called when the action is Deleted so that the appt's fields will be recorded.</summary>
		public static void CreateHistoryEntry(long apptNum,HistAppointmentAction action) {
			//No need for additional DB check when appt was already deleted.
			Appointment appt=(action==HistAppointmentAction.Deleted?null:Appointments.GetOneApt(apptNum));
			CreateHistoryEntry(appt,action,apptNum);
		}
		
		///<summary>When appt is null you must provide aptNum and HistApptAction will be set to deleted.</summary>
		public static HistAppointment CreateHistoryEntry(Appointment appt,HistAppointmentAction action,long aptNum=0) {
			if(Security.CurUser==null) {
				return null;//There is no user currently logged on so do not create a HistAppointment.
			}
			HistAppointment hist=new HistAppointment();
			hist.HistUserNum=Security.CurUser.UserNum;
			hist.ApptSource=Security.CurUser.EServiceType;
			hist.HistApptAction=action;
			if(appt!=null) {//Null if deleted
				hist.SetAppt(appt);
			}
			else {
				hist.AptNum=aptNum;
				hist.HistApptAction=HistAppointmentAction.Deleted;
			}
			HistAppointments.Insert(hist);
			return hist;
		}
		#endregion
	}
}