using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobSprintLinks{
		#region Get Methods

		///<summary>Gets a list of JobSprintLinks according to the passed in JobSprintNum.</summary>
		public static List<JobSprintLink> GetForSprint(long jobSprintNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<JobSprintLink>>(MethodBase.GetCurrentMethod(),jobSprintNum);
			}
			string command="SELECT * FROM jobsprintlink WHERE jobsprintnum="+POut.Long(jobSprintNum);
			return Crud.JobSprintLinkCrud.SelectMany(command);
		}
		
		///<summary>Gets one JobSprintLink from the db.</summary>
		public static JobSprintLink GetOne(long jobSprintLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobSprintLink>(MethodBase.GetCurrentMethod(),jobSprintLinkNum);
			}
			return Crud.JobSprintLinkCrud.SelectOne(jobSprintLinkNum);
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		///<summary></summary>
		public static long Insert(JobSprintLink jobSprintLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobSprintLink.JobSprintLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobSprintLink);
				return jobSprintLink.JobSprintLinkNum;
			}
			return Crud.JobSprintLinkCrud.Insert(jobSprintLink);
		}
		#endregion Insert
		#region Update
		///<summary></summary>
		public static void Update(JobSprintLink jobSprintLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobSprintLink);
				return;
			}
			Crud.JobSprintLinkCrud.Update(jobSprintLink);
		}
		#endregion Update
		#region Delete
		///<summary></summary>
		public static void Delete(long jobSprintLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobSprintLinkNum);
				return;
			}
			Crud.JobSprintLinkCrud.Delete(jobSprintLinkNum);
		}

		///<summary>Deletes all JobSprintLinks for the JobSprintNum.</summary>
		public static void DeleteForSprint(long jobSprintNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobSprintNum);
			}
			string command="DELETE FROM jobsprintlink "
				+"WHERE JobSprintNum = "+POut.Long(jobSprintNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all JobSprintLinks for the JobNum.</summary>
		public static void DeleteForJob(long jobNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
			}
			string command="DELETE FROM jobsprintlink "
				+"WHERE JobNum = "+POut.Long(jobNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all JobSprintLinks for the JobNum and JobSprintNum.</summary>
		public static void DeleteForJobAndSprint(long jobNum,long jobSprintNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum,jobSprintNum);
			}
			string command=$@"DELETE FROM jobsprintlink 
					WHERE JobNum = {POut.Long(jobNum)} 
					AND JobSprintNum = {POut.Long(jobSprintNum)}";
			Db.NonQ(command);
		}
		#endregion Delete
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods



	}
}