using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class ApptGeneralMessageSents{
		#region Methods - Get
		///<summary></summary>
		public static List<ApptGeneralMessageSent> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptGeneralMessageSent>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM apptgeneralmessagesent WHERE PatNum="+POut.Long(patNum);
			return Crud.ApptGeneralMessageSentCrud.SelectMany(command);
		}

		public static List<ApptGeneralMessageSent> GetForAppointment(long apptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptGeneralMessageSent>>(MethodBase.GetCurrentMethod(),apptNum);
			}
			string command="SELECT * FROM apptgeneralmessagesent WHERE ApptNum="+POut.Long(apptNum);
			return Crud.ApptGeneralMessageSentCrud.SelectMany(command);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ApptGeneralMessageSent apptGeneralMessageSent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				apptGeneralMessageSent.ApptGeneralMessageSentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptGeneralMessageSent);
				return apptGeneralMessageSent.ApptGeneralMessageSentNum;
			}
			return Crud.ApptGeneralMessageSentCrud.Insert(apptGeneralMessageSent);
		}

		///<summary></summary>
		public static void InsertMany(List<ApptGeneralMessageSent> listapptGeneralMessageSents) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listapptGeneralMessageSents);
				return;
			}
			Crud.ApptGeneralMessageSentCrud.InsertMany(listapptGeneralMessageSents);
		}
		#endregion Methods - Modify
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary>Gets one ApptGeneralMessageSent from the db.</summary>
		public static ApptGeneralMessageSent GetOne(long apptGeneralMessageSentNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ApptGeneralMessageSent>(MethodBase.GetCurrentMethod(),apptGeneralMessageSentNum);
			}
			return Crud.ApptGeneralMessageSentCrud.SelectOne(apptGeneralMessageSentNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static void Update(ApptGeneralMessageSent apptGeneralMessageSent){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptGeneralMessageSent);
				return;
			}
			Crud.ApptGeneralMessageSentCrud.Update(apptGeneralMessageSent);
		}
		///<summary></summary>
		public static void Delete(long apptGeneralMessageSentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptGeneralMessageSentNum);
				return;
			}
			Crud.ApptGeneralMessageSentCrud.Delete(apptGeneralMessageSentNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		#endregion Methods - Misc
		*/
	}
}