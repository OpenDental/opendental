using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class FamilyHealths {
		///<summary></summary>
		public static void Delete(long familyHealthNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),familyHealthNum);
				return;
			}
			string command= "DELETE FROM familyhealth WHERE FamilyHealthNum = "+POut.Long(familyHealthNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(FamilyHealth familyHealth) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				familyHealth.FamilyHealthNum=Meth.GetLong(MethodBase.GetCurrentMethod(),familyHealth);
				return familyHealth.FamilyHealthNum;
			}
			return Crud.FamilyHealthCrud.Insert(familyHealth);
		}

		///<summary></summary>
		public static void Update(FamilyHealth familyHealth) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),familyHealth);
				return;
			}
			Crud.FamilyHealthCrud.Update(familyHealth);
		}

		///<summary></summary>
		public static List<FamilyHealth> GetFamilyHealthForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<FamilyHealth>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM familyhealth WHERE PatNum = "+POut.Long(patNum);
			return Crud.FamilyHealthCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one FamilyHealth from the db.</summary>
		public static FamilyHealth GetOne(long familyHealthNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<FamilyHealth>(MethodBase.GetCurrentMethod(),familyHealthNum);
			}
			return Crud.FamilyHealthCrud.SelectOne(familyHealthNum);
		}
		*/
	}
}