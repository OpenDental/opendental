using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class VaccinePats{		
		///<summary></summary>
		public static List<VaccinePat> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<VaccinePat>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM vaccinepat WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DateTimeStart";
			return Crud.VaccinePatCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(VaccinePat vaccinePat){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				vaccinePat.VaccinePatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),vaccinePat);
				return vaccinePat.VaccinePatNum;
			}
			return Crud.VaccinePatCrud.Insert(vaccinePat);
		}

		///<summary></summary>
		public static void Update(VaccinePat vaccinePat){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccinePat);
				return;
			}
			Crud.VaccinePatCrud.Update(vaccinePat);
		}

		///<summary></summary>
		public static void Delete(long vaccinePatNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccinePatNum);
				return;
			}
			string command= "DELETE FROM vaccinepat WHERE VaccinePatNum = "+POut.Long(vaccinePatNum);
			Db.NonQ(command);
			//Delete any attached observations.
			VaccineObses.DeleteForVaccinePat(vaccinePatNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		
		///<summary>Gets one VaccinePat from the db.</summary>
		public static VaccinePat GetOne(long vaccinePatNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<VaccinePat>(MethodBase.GetCurrentMethod(),vaccinePatNum);
			}
			return Crud.VaccinePatCrud.SelectOne(vaccinePatNum);
		}
		*/
	}
}