using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobQuotes{

		///<summary></summary>
		public static List<JobQuote> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobQuote>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM jobquote WHERE PatNum = "+POut.Long(patNum);
			return Crud.JobQuoteCrud.SelectMany(command);
		}

		public static List<JobQuote> GetForJob(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobQuote>>(MethodBase.GetCurrentMethod(),jobNum);
			}
			string command="SELECT * FROM jobquote WHERE JobNum = "+POut.Long(jobNum);
			return Crud.JobQuoteCrud.SelectMany(command);
		}

		public static List<JobQuote> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobQuote>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM jobquote ";
			return Crud.JobQuoteCrud.SelectMany(command);
		}

		///<summary>Gets one JobQuote from the db.</summary>
		public static JobQuote GetOne(long jobQuoteNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobQuote>(MethodBase.GetCurrentMethod(),jobQuoteNum);
			}
			return Crud.JobQuoteCrud.SelectOne(jobQuoteNum);
		}

		///<summary></summary>
		public static long Insert(JobQuote jobQuote){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobQuote.JobQuoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobQuote);
				return jobQuote.JobQuoteNum;
			}
			return Crud.JobQuoteCrud.Insert(jobQuote);
		}

		///<summary></summary>
		public static void Update(JobQuote jobQuote){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobQuote);
				return;
			}
			Crud.JobQuoteCrud.Update(jobQuote);
		}

		///<summary></summary>
		public static void Delete(long jobQuoteNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobQuoteNum);
				return;
			}
			Crud.JobQuoteCrud.Delete(jobQuoteNum);
		}

		public static void DeleteForJob(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			string command="DELETE FROM jobquote WHERE JobNum="+POut.Long(jobNum);
			Db.NonQ(command);
		}

		public static void Sync(List<JobQuote> listNew,long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,jobNum);
				return;
			}
			List<JobQuote> listDB=GetForJob(jobNum);
			Crud.JobQuoteCrud.Sync(listNew,listDB);
		}

		public static List<JobQuote> GetJobQuotesForJobs(List<long> jobNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobQuote>>(MethodBase.GetCurrentMethod(),jobNums);
			}
			if(jobNums==null || jobNums.Count==0) {
				return new List<JobQuote>();
			}
			string command="SELECT * FROM jobquote WHERE JobNum IN ("+string.Join(",",jobNums)+")";
			return Crud.JobQuoteCrud.SelectMany(command);
		}

		public static List<JobQuote> GetUnfinishedJobQuotes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobQuote>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT jq.* FROM JobQuote jq INNER JOIN Job j ON j.JobNum=jq.JobNum AND j.PhaseCur!='Complete'";
			return Crud.JobQuoteCrud.SelectMany(command);
		}

	}
}

