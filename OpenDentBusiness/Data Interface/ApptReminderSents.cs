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

		public static List<ApptReminderSent> GetForApt(params long[] arrApptNums) {
			if(arrApptNums.IsNullOrEmpty()) {
				return new List<ApptReminderSent>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptReminderSent>>(MethodBase.GetCurrentMethod(),arrApptNums);
			}
			string command="SELECT * FROM apptremindersent WHERE ApptNum IN ("+string.Join(",",arrApptNums.Select(x => POut.Long(x)))+") ";
			return Crud.ApptReminderSentCrud.SelectMany(command);
		}

		#endregion

		///<summary></summary>
		public static long Insert(ApptReminderSent apptReminderSent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				apptReminderSent.ApptReminderSentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptReminderSent);
				return apptReminderSent.ApptReminderSentNum;
			}
			return Crud.ApptReminderSentCrud.Insert(apptReminderSent);
		}

		///<summary></summary>
		public static void InsertMany(List<ApptReminderSent> listApptReminderSents) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listApptReminderSents);
				return;
			}
			Crud.ApptReminderSentCrud.InsertMany(listApptReminderSents);
		}

		///<summary></summary>
		public static void Update(ApptReminderSent apptReminderSent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderSent);
				return;
			}
			Crud.ApptReminderSentCrud.Update(apptReminderSent);
		}

		///<summary></summary>
		public static void Delete(long apptReminderSentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderSentNum);
				return;
			}
			Crud.ApptReminderSentCrud.Delete(apptReminderSentNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ApptReminderSent> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptReminderSent>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command = "SELECT * FROM apptremindersent WHERE PatNum = "+POut.Long(patNum);
			return Crud.ApptReminderSentCrud.SelectMany(command);
		}

		///<summary>Gets one ApptReminderSent from the db.</summary>
		public static ApptReminderSent GetOne(long apptReminderSentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ApptReminderSent>(MethodBase.GetCurrentMethod(),apptReminderSentNum);
			}
			return Crud.ApptReminderSentCrud.SelectOne(apptReminderSentNum);
		}

		*/
	}
}
 