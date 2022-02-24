using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Health.Direct.Common.Extensions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EmailSecures {
		///<summary>Gets all emailsecure rows by primary key.</summary>
		public static List<EmailSecure> GetMany(List<long> listEmailSecureNums,bool doIncludePending=false) {
			if(listEmailSecureNums.IsNullOrEmpty()) {
				return new List<EmailSecure>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),listEmailSecureNums,doIncludePending);
			}
			string command=$"SELECT * FROM emailsecure WHERE emailsecure.EmailSecureNum IN ({string.Join(",",listEmailSecureNums.Select(x => POut.Long(x)))}) ";
			if(!doIncludePending) {
				command+="AND emailsecure.EmailMessageNum > 0";
			}
			return Crud.EmailSecureCrud.SelectMany(command);
		}

		///<summary>Gets all emailsecure rows that have not yet downloaded the email from the EmailHosting API.  Pass empty list to get for all clinics.</summary>
		public static List<EmailSecure> GetOutstanding(List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT * FROM emailsecure WHERE emailsecure.EmailMessageNum=0 ";
			if(!listClinicNums.IsNullOrEmpty()) {
				command+="AND emailsecure.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+")";
			}
			return Crud.EmailSecureCrud.SelectMany(command);
		}

		public static SerializableDictionary<long,List<EmailSecure>> GetEmailChains(List<long> listEmailChainFKs) {
			if(listEmailChainFKs.Count==0) {
				return new SerializableDictionary<long, List<EmailSecure>>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetSerializableDictionary<long,List<EmailSecure>>(MethodBase.GetCurrentMethod(),listEmailChainFKs);
			}
			string command="SELECT * FROM emailsecure WHERE emailsecure.EmailChainFK IN ("
				+string.Join(",",listEmailChainFKs.Distinct().Select(x => POut.Long(x)))+")";
			return Crud.EmailSecureCrud.SelectMany(command).GroupBy(x => x.EmailChainFK).ToSerializableDictionary(x => x.Key,x => x.ToList());
		}

		///<summary></summary>
		public static long Insert(EmailSecure emailSecure){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				emailSecure.EmailSecureNum=Meth.GetLong(MethodBase.GetCurrentMethod(),emailSecure);
				return emailSecure.EmailSecureNum;
			}
			return Crud.EmailSecureCrud.Insert(emailSecure);
		}

		///<summary></summary>
		public static void InsertMany(List<EmailSecure> listEmailSecures) {
			if(listEmailSecures.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEmailSecures);
				return;
			}
			Crud.EmailSecureCrud.InsertMany(listEmailSecures);
		}

		///<summary></summary>
		public static void Update(EmailSecure emailSecure){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailSecure);
				return;
			}
			Crud.EmailSecureCrud.Update(emailSecure);
		}
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<EmailSecure> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM emailsecure WHERE PatNum = "+POut.Long(patNum);
			return Crud.EmailSecureCrud.SelectMany(command);
		}
		
		///<summary>Gets one EmailSecure from the db.</summary>
		public static EmailSecure GetOne(long emailSecureNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EmailSecure>(MethodBase.GetCurrentMethod(),emailSecureNum);
			}
			return Crud.EmailSecureCrud.SelectOne(emailSecureNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Delete(long emailSecureNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailSecureNum);
				return;
			}
			Crud.EmailSecureCrud.Delete(emailSecureNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/



	}
}