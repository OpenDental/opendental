using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrLabResultsCopyTos {
		///<summary></summary>
		public static List<EhrLabResultsCopyTo> GetForLab(long ehrLabNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EhrLabResultsCopyTo>>(MethodBase.GetCurrentMethod(),ehrLabNum);
			}
			string command="SELECT * FROM ehrlabresultscopyto WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			return Crud.EhrLabResultsCopyToCrud.SelectMany(command);
		}

		///<summary>Deletes notes for lab results too.</summary>
		public static void DeleteForLab(long ehrLabNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNum);
				return;
			}
			string command="DELETE FROM ehrlabresultscopyto WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(EhrLabResultsCopyTo ehrLabResultsCopyTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				ehrLabResultsCopyTo.EhrLabResultsCopyToNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabResultsCopyTo);
				return ehrLabResultsCopyTo.EhrLabResultsCopyToNum;
			}
			return Crud.EhrLabResultsCopyToCrud.Insert(ehrLabResultsCopyTo);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrLabResultsCopyTo> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EhrLabResultsCopyTo>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabresultscopyto WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrLabResultsCopyToCrud.SelectMany(command);
		}

		///<summary>Gets one EhrLabResultsCopyTo from the db.</summary>
		public static EhrLabResultsCopyTo GetOne(long ehrLabResultsCopyToNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EhrLabResultsCopyTo>(MethodBase.GetCurrentMethod(),ehrLabResultsCopyToNum);
			}
			return Crud.EhrLabResultsCopyToCrud.SelectOne(ehrLabResultsCopyToNum);
		}

		///<summary></summary>
		public static void Update(EhrLabResultsCopyTo ehrLabResultsCopyTo){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabResultsCopyTo);
				return;
			}
			Crud.EhrLabResultsCopyToCrud.Update(ehrLabResultsCopyTo);
		}

		///<summary></summary>
		public static void Delete(long ehrLabResultsCopyToNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabResultsCopyToNum);
				return;
			}
			string command= "DELETE FROM ehrlabresultscopyto WHERE EhrLabResultsCopyToNum = "+POut.Long(ehrLabResultsCopyToNum);
			Db.NonQ(command);
		}
		*/
	}
}