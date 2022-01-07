using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PlannedAppts{
		///<summary>Gets all planned appt objects for a patient.</summary>
		public static List<PlannedAppt> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PlannedAppt>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM plannedappt WHERE PatNum="+POut.Long(patNum);
			return Crud.PlannedApptCrud.SelectMany(command);
		}

		///<Summary>Gets one plannedAppt from the database.</Summary>
		public static PlannedAppt GetOne(long plannedApptNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PlannedAppt>(MethodBase.GetCurrentMethod(),plannedApptNum);
			}
			return Crud.PlannedApptCrud.SelectOne(plannedApptNum);
		}

		///<summary>Gets one plannedAppt by patient, ordered by ItemOrder</summary>
		public static PlannedAppt GetOneOrderedByItemOrder(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PlannedAppt>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM plannedappt WHERE PatNum="+POut.Long(patNum)
				+" ORDER BY ItemOrder";
			command=DbHelper.LimitOrderBy(command,1);
			return Crud.PlannedApptCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(PlannedAppt plannedAppt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				plannedAppt.PlannedApptNum=Meth.GetLong(MethodBase.GetCurrentMethod(),plannedAppt);
				return plannedAppt.PlannedApptNum;
			}
			return Crud.PlannedApptCrud.Insert(plannedAppt);
		}

		///<summary></summary>
		public static void Update(PlannedAppt plannedAppt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),plannedAppt);
				return;
			}
			Crud.PlannedApptCrud.Update(plannedAppt);
		}

		///<summary></summary>
		public static void Update(PlannedAppt plannedAppt,PlannedAppt oldPlannedAppt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),plannedAppt,oldPlannedAppt);
				return;
			}
			Crud.PlannedApptCrud.Update(plannedAppt,oldPlannedAppt);
		}
	}
}