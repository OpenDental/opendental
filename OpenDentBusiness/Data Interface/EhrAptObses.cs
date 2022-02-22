using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrAptObses{
		///<summary></summary>
		public static List<EhrAptObs> Refresh(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrAptObs>>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM ehraptobs WHERE AptNum = "+POut.Long(aptNum);
			return Crud.EhrAptObsCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EhrAptObs ehrAptObs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrAptObs.EhrAptObsNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrAptObs);
				return ehrAptObs.EhrAptObsNum;
			}
			return Crud.EhrAptObsCrud.Insert(ehrAptObs);
		}

		///<summary></summary>
		public static void Update(EhrAptObs ehrAptObs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrAptObs);
				return;
			}
			Crud.EhrAptObsCrud.Update(ehrAptObs);
		}

		///<summary></summary>
		public static void Delete(long ehrAptObsNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrAptObsNum);
				return;
			}
			string command= "DELETE FROM ehraptobs WHERE EhrAptObsNum = "+POut.Long(ehrAptObsNum);
			Db.NonQ(command);
		}
		
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one EhrAptObs from the db.</summary>
		public static EhrAptObs GetOne(long ehrAptObsNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrAptObs>(MethodBase.GetCurrentMethod(),ehrAptObsNum);
			}
			return Crud.EhrAptObsCrud.SelectOne(ehrAptObsNum);
		}

		
		*/
	}
}