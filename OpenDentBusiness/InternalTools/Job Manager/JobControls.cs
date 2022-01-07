using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobControls{

		///<summary></summary>
		public static List<JobControl> Refresh(long jobControlNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobControl>>(MethodBase.GetCurrentMethod(),jobControlNum);
			}
			string command="SELECT * FROM jobcontrol WHERE JobControlNum = "+POut.Long(jobControlNum);
			return Crud.JobControlCrud.SelectMany(command);
		}

		///<summary>Gets one JobControl from the db.</summary>
		public static JobControl GetOne(long jobControlNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobControl>(MethodBase.GetCurrentMethod(),jobControlNum);
			}
			return Crud.JobControlCrud.SelectOne(jobControlNum);
		}

		///<summary></summary>
		public static long Insert(JobControl jobControl){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobControl.JobControlNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobControl);
				return jobControl.JobControlNum;
			}
			return Crud.JobControlCrud.Insert(jobControl);
		}

		///<summary></summary>
		public static void Update(JobControl jobControl){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobControl);
				return;
			}
			Crud.JobControlCrud.Update(jobControl);
		}

		///<summary></summary>
		public static void Delete(long jobControlNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobControlNum);
				return;
			}
			Crud.JobControlCrud.Delete(jobControlNum);
		}



	}
}