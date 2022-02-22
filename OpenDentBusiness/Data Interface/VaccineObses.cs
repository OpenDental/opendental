using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class VaccineObses{
		///<summary></summary>
		public static long Insert(VaccineObs vaccineObs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				vaccineObs.VaccineObsNum=Meth.GetLong(MethodBase.GetCurrentMethod(),vaccineObs);
				return vaccineObs.VaccineObsNum;
			}
			return Crud.VaccineObsCrud.Insert(vaccineObs);
		}

		///<summary>Gets one VaccineObs from the db.</summary>
		public static List<VaccineObs> GetForVaccine(long vaccinePatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VaccineObs>>(MethodBase.GetCurrentMethod(),vaccinePatNum);
			}
			string command="SELECT * FROM vaccineobs WHERE VaccinePatNum="+POut.Long(vaccinePatNum)+" ORDER BY VaccineObsNumGroup";
			return Crud.VaccineObsCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(VaccineObs vaccineObs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccineObs);
				return;
			}
			Crud.VaccineObsCrud.Update(vaccineObs);
		}

		///<summary></summary>
		public static void Delete(long vaccineObsNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccineObsNum);
				return;
			}
			string command= "DELETE FROM vaccineobs WHERE VaccineObsNum = "+POut.Long(vaccineObsNum);
			Db.NonQ(command);
		}

		public static void DeleteForVaccinePat(long vaccinePatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccinePatNum);
				return;
			}
			string command="DELETE FROM vaccineobs WHERE VaccinePatNum="+POut.Long(vaccinePatNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<VaccineObs> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VaccineObs>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM vaccineobs WHERE PatNum = "+POut.Long(patNum);
			return Crud.VaccineObsCrud.SelectMany(command);
		}

		///<summary>Gets one VaccineObs from the db.</summary>
		public static VaccineObs GetOne(long vaccineObsNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<VaccineObs>(MethodBase.GetCurrentMethod(),vaccineObsNum);
			}
			return Crud.VaccineObsCrud.SelectOne(vaccineObsNum);
		}
		*/
	}
}