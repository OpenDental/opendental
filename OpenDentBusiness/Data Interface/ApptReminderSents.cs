using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ApptReminderSents{
		#region Get Methods

		public static List<ApptReminderSent> GetForApt(params long[] apptNumArray) {
			if(apptNumArray.IsNullOrEmpty()) {
				return new List<ApptReminderSent>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderSent>>(MethodBase.GetCurrentMethod(),apptNumArray);
			}
			string command="SELECT * FROM apptremindersent WHERE ApptNum IN ("+string.Join(",",apptNumArray.Select(x => POut.Long(x)))+") ";
			return Crud.ApptReminderSentCrud.SelectMany(command);
		}

		#endregion

		///<summary></summary>
		public static long Insert(ApptReminderSent apptReminderSent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				apptReminderSent.ApptReminderSentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptReminderSent);
				return apptReminderSent.ApptReminderSentNum;
			}
			return Crud.ApptReminderSentCrud.Insert(apptReminderSent);
		}

		///<summary></summary>
		public static void InsertMany(List<ApptReminderSent> listApptReminderSents) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listApptReminderSents);
				return;
			}
			Crud.ApptReminderSentCrud.InsertMany(listApptReminderSents);
		}

		///<summary></summary>
		public static void Update(ApptReminderSent apptReminderSent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderSent);
				return;
			}
			Crud.ApptReminderSentCrud.Update(apptReminderSent);
		}

		///<summary></summary>
		public static void Delete(long apptReminderSentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderSentNum);
				return;
			}
			Crud.ApptReminderSentCrud.Delete(apptReminderSentNum);
		}

		public static void Delete(params long[] apptReminderSentNumsArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderSentNumsArray);
				return;
			}
			if(apptReminderSentNumsArray.IsNullOrEmpty()) {
				return;
			}
			string command="DELETE FROM apptremindersent WHERE ApptReminderSentNum IN ("
				+string.Join(",",apptReminderSentNumsArray.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Delete the ApptReminderSent entries for rescheduled/cancelled appointments.  
		///AutoComm will resend these automatically where the appointment still exists.
		///HQ will increment the Sequence number of the .ics file such that the calendar entry on the patient's device is updated.
		///Remove those that the user specifically said to not resend</summary>
		public static void HandleApptChanged(Logger.IWriteLine log) {
			//No need to check MiddleTierRole; no call to db.
			List<ApptReminderSent> listApptRemindersSentsChanged=GetForApptChanged();
			log.WriteLine($"Deleting {listApptRemindersSentsChanged.Count} ApptReminderSent entries.",LogLevel.Information);
			string verboseLog=string.Join("\r\n\t\t",listApptRemindersSentsChanged
				.Select(x => $"ApptReminderSentNum: {x.ApptReminderSentNum}, PatNum: {x.PatNum}, ApptDateTime: {x.ApptDateTime}"));
			log.WriteLine($"Deleting \r\n\t\t{verboseLog}",LogLevel.Verbose);
			Delete(listApptRemindersSentsChanged.Select(x => x.ApptReminderSentNum).ToArray());
		}

		//<summary>Get the list of ApptReminderSents where the appointment was rescheduled or cancelled after sending the reminder.</summary>
		public static List<ApptReminderSent> GetForApptChanged() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderSent>>(MethodBase.GetCurrentMethod());
			}
			//Do not include UnscheduledList or Broken appointments
			List<ApptStatus> listApptStatuses=new List<ApptStatus>() { ApptStatus.UnschedList, ApptStatus.Broken };
			string command=@"SELECT apptremindersent.* 
				FROM apptremindersent
				LEFT JOIN appointment ON apptremindersent.ApptNum=appointment.AptNum
				WHERE (appointment.AptNum IS NULL OR appointment.AptDateTime!=apptremindersent.ApptDateTime
				OR appointment.AptStatus IN ("+string.Join(",",listApptStatuses.Select(x => POut.Int((int)x)))+"))";
			return Crud.ApptReminderSentCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ApptReminderSent> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderSent>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command = "SELECT * FROM apptremindersent WHERE PatNum = "+POut.Long(patNum);
			return Crud.ApptReminderSentCrud.SelectMany(command);
		}

		///<summary>Gets one ApptReminderSent from the db.</summary>
		public static ApptReminderSent GetOne(long apptReminderSentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ApptReminderSent>(MethodBase.GetCurrentMethod(),apptReminderSentNum);
			}
			return Crud.ApptReminderSentCrud.SelectOne(apptReminderSentNum);
		}

		*/
	}
}
 