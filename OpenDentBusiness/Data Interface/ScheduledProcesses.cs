using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ScheduledProcesses{
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<ScheduledProcess> Refresh(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ScheduledProcess>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM scheduledprocess";
			return Crud.ScheduledProcessCrud.SelectMany(command);
		}

		#endregion Get Methods
		
		#region Insert
		///<summary></summary>
		public static long Insert(ScheduledProcess scheduledProcess){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				scheduledProcess.ScheduledProcessNum=Meth.GetLong(MethodBase.GetCurrentMethod(),scheduledProcess);
				return scheduledProcess.ScheduledProcessNum;
			}
			return Crud.ScheduledProcessCrud.Insert(scheduledProcess);
		}
		#endregion Insert
		
		#region Update
		///<summary></summary>
		public static void Update(ScheduledProcess scheduledProcess,ScheduledProcess oldScheduledProcess){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),scheduledProcess,oldScheduledProcess);
				return;
			}
			Crud.ScheduledProcessCrud.Update(scheduledProcess,oldScheduledProcess);
		}
		#endregion Update
		
		#region Delete
		///<summary></summary>
		public static void Delete(long scheduledProcessNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),scheduledProcessNum);
				return;
			}
			Crud.ScheduledProcessCrud.Delete(scheduledProcessNum);
		}
		#endregion Delete
	
		#region Misc Methods
		/// <summary>Returns a list of all scheduled actions with a matching Action type, Frequency to run, and TimeToRun as those passed in.</summary>
		public static List<ScheduledProcess> CheckAlreadyScheduled(ScheduledActionEnum schedActionEnum, FrequencyToRunEnum freqToRunEnum, 
			DateTime timeToRun) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ScheduledProcess>>(MethodBase.GetCurrentMethod(),schedActionEnum,freqToRunEnum,timeToRun);				
			}
			string command=$@"SELECT * FROM scheduledprocess 
				WHERE ScheduledAction='{POut.String(schedActionEnum.ToString())}' AND 
				FrequencyToRun='{POut.String(freqToRunEnum.ToString())}' AND 
				TIME(TimeToRun)=TIME({POut.DateT(timeToRun)}) ";
			return Crud.ScheduledProcessCrud.SelectMany(command);
		}

		#endregion Misc Methods

		/*
		///<summary>Gets one ScheduledProcess from the db.</summary>
		public static ScheduledProcess GetOne(long scheduledProcessNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ScheduledProcess>(MethodBase.GetCurrentMethod(),scheduledProcessNum);
			}
			return Crud.ScheduledProcessCrud.SelectOne(scheduledProcessNum);
		}
		*/
	}
}