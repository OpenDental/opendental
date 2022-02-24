using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobSprints{
		#region Get Methods
		///<summary></summary>
		public static List<JobSprint> GetAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobSprint>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM jobsprint";
			return Crud.JobSprintCrud.SelectMany(command);
		}
		
		///<summary>Gets one JobSprint from the db.</summary>
		public static JobSprint GetOne(long jobSprintNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobSprint>(MethodBase.GetCurrentMethod(),jobSprintNum);
			}
			return Crud.JobSprintCrud.SelectOne(jobSprintNum);
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		///<summary></summary>
		public static long Insert(JobSprint jobSprint){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobSprint.JobSprintNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobSprint);
				return jobSprint.JobSprintNum;
			}
			return Crud.JobSprintCrud.Insert(jobSprint);
		}
		#endregion Insert
		#region Update
		///<summary></summary>
		public static void Update(JobSprint jobSprint){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobSprint);
				return;
			}
			Crud.JobSprintCrud.Update(jobSprint);
		}
		#endregion Update
		#region Delete
		///<summary></summary>
		public static void Delete(long jobSprintNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobSprintNum);
				return;
			}
			JobSprintLinks.DeleteForSprint(jobSprintNum);
			Crud.JobSprintCrud.Delete(jobSprintNum);
		}
		#endregion Delete
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods



	}
}