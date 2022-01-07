using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobLinks{		

		///<summary></summary>
		public static List<JobLink> GetByJobNum(long jobNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),jobNum);
			}
			string command="SELECT * FROM joblink WHERE JobNum = "+POut.Long(jobNum);
			return Crud.JobLinkCrud.SelectMany(command);
		}

		///<summary>Gets one JobLink from the db.</summary>
		public static JobLink GetOne(long jobLinknum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobLink>(MethodBase.GetCurrentMethod(),jobLinknum);
			}
			return Crud.JobLinkCrud.SelectOne(jobLinknum);
		}

		///<summary>Gets JobLinks for a specified JobNum. Only gets Bugs, Feature Requests, Quotes, and Tasks.</summary>
		public static List<JobLink> GetJobLinksForJobs(List<long> jobNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),jobNums);
			}
			if(jobNums==null || jobNums.Count==0) {
				return new List<JobLink>();
			}
			string command="SELECT * FROM joblink WHERE JobNum IN ("+string.Join(",",jobNums)+")";
			return Crud.JobLinkCrud.SelectMany(command);
		}

		///<summary>Gets JobLinks for a specified JobNum. Only gets Bugs, Feature Requests, Quotes, and Tasks.</summary>
		public static List<JobLink> GetForTask(long taskNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),taskNum);
			}
			if(taskNum==0) {
				return new List<JobLink>();
			}
			string command = "SELECT * FROM joblink WHERE FKey="+POut.Long(taskNum)+" AND LinkType="+POut.Int((int)JobLinkType.Task);
			return Crud.JobLinkCrud.SelectMany(command);
		}

		///<summary>Gets JobLinks from a list of TaskNums. Only gets tasks.</summary>
		public static List<JobLink> GetForTaskNums(List<long> listTaskNums) {
			if(listTaskNums==null || listTaskNums.Count==0) {
				return new List<JobLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),listTaskNums);
			}
			string command = "SELECT * FROM joblink WHERE FKey IN ("+string.Join(",",listTaskNums.Select(x => POut.Long(x)))+")"
				+" AND LinkType="+POut.Int((int)JobLinkType.Task);
			return Crud.JobLinkCrud.SelectMany(command);
		}

		///<summary>Gets JobLinks for a specified JobNum. Only gets Bugs, Feature Requests, Quotes, and Tasks.</summary>
		public static List<JobLink> GetForApptNum(long apptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),apptNum);
			}
			if(apptNum==0) {
				return new List<JobLink>();
			}
			string command = "SELECT * FROM joblink WHERE FKey="+POut.Long(apptNum)+" AND LinkType="+POut.Int((int)JobLinkType.Appointment);
			return Crud.JobLinkCrud.SelectMany(command);
		}

		///<summary>Gets JobLinks for a specified JobNum. Only gets Bugs, Feature Requests, and Tasks.</summary>
		public static List<JobLink> GetJobLinks(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),jobNum);
			}
			string command="SELECT * FROM joblink"
				+" WHERE JobNum="+POut.Long(jobNum)
				+" AND LinkType IN ("+(int)JobLinkType.Bug+","+(int)JobLinkType.Request+","+(int)JobLinkType.Task+")"
				+" ORDER BY LinkType";
			return Crud.JobLinkCrud.SelectMany(command);
		}

		public static List<JobLink> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM joblink";
			return Crud.JobLinkCrud.SelectMany(command);
		}

		public static List<JobLink> GetForType(JobLinkType linkType,long FKey) {
			return GetManyForType(linkType,new List<long>() { FKey });
		}

		public static List<JobLink> GetManyForType(JobLinkType linkType,List<long> listFkeys) {
			if(listFkeys==null || listFkeys.Count==0) {
				return new List<JobLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLink>>(MethodBase.GetCurrentMethod(),linkType,listFkeys);
			}
			string command="SELECT * FROM joblink WHERE LinkType="+POut.Int((int)linkType)+" "
				+"AND FKey IN ("+string.Join(",",listFkeys.Select(x => POut.Long(x)))+")";
			return Crud.JobLinkCrud.SelectMany(command);
		}

		///<summary>Returns task associated to the same job as the given bugIds job.</summary>
		public static List<long> GetTaskNumsForBug(long bugId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),bugId);
			}
			string command ="SELECT task.* "
				+"FROM task "
				+"INNER JOIN jobLink jLink1 ON task.TaskNum=jLink1.FKey "
				+"INNER JOIN joblink jLink2 ON jLink2.JobNum=jlInk1.JobNum "
				+"WHERE task.ObjectType=1 "//1 => patient
				+"AND jLink1.LinkType=0 "//0 => task
				+"AND jLink2.FKey="+POut.Long(bugId)+" "
				+"AND jLink2.LinkType=2";//2 => bug
			List<Task> listJobTasks=Crud.TaskCrud.SelectMany(command);
			List<Task> listBugTasks=listJobTasks.FindAll(x => x.Descript.Contains("Steps to duplicate:"));
			if(listBugTasks.Count>0){
				return listBugTasks.Select(x => x.TaskNum).ToList();
			}
			return listJobTasks.Select(x => x.TaskNum).ToList();
		}

		///<summary></summary>
		public static long Insert(JobLink jobLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobLink.JobLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobLink);
				return jobLink.JobLinkNum;
			}
			return Crud.JobLinkCrud.Insert(jobLink);
		}

		///<summary></summary>
		public static void Update(JobLink jobLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobLink);
				return;
			}
			Crud.JobLinkCrud.Update(jobLink);
		}

		///<summary></summary>
		public static void Delete(long jobLinknum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobLinknum);
				return;
			}
			Crud.JobLinkCrud.Delete(jobLinknum);
		}

		public static void DeleteForJob(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			string command="DELETE FROM joblink WHERE JobNum="+POut.Long(jobNum);
			Db.NonQ(command);
		}

		public static void DeleteForType(JobLinkType linkType,long fKey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),linkType,fKey);
				return;
			}
			string command="DELETE FROM joblink WHERE LinkType="+POut.Int((int)linkType)+" "
				+"AND FKey="+POut.Long(fKey);
			Db.NonQ(command);
		}

		public static void Sync(List<JobLink> listNew,long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,jobNum);
				return;
			}
			List<JobLink> listDB=GetByJobNum(jobNum);
			Crud.JobLinkCrud.Sync(listNew,listDB);
		}


	}
}