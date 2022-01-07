using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Allergies{
		///<summary></summary>
		public static List<Allergy> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Allergy>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM allergy WHERE PatNum = "+POut.Long(patNum);
			return Crud.AllergyCrud.SelectMany(command);
		}

		///<summary>Gets one Allergy from the db.</summary>
		public static Allergy GetOne(long allergyNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Allergy>(MethodBase.GetCurrentMethod(),allergyNum);
			}
			return Crud.AllergyCrud.SelectOne(allergyNum);
		}

		///<summary></summary>
		public static long Insert(Allergy allergy){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				allergy.AllergyNum=Meth.GetLong(MethodBase.GetCurrentMethod(),allergy);
				return allergy.AllergyNum;
			}
			return Crud.AllergyCrud.Insert(allergy);
		}

		///<summary></summary>
		public static void Update(Allergy allergy){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),allergy);
				return;
			}
			Crud.AllergyCrud.Update(allergy);
		}

		///<summary></summary>
		public static void Delete(long allergyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),allergyNum);
				return;
			}
			string command= "DELETE FROM allergy WHERE AllergyNum = "+POut.Long(allergyNum);
			Db.NonQ(command);
		}

		///<summary>Gets all allergies for patient whether active or not.</summary>
		public static List<Allergy> GetAll(long patNum,bool showInactive) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Allergy>>(MethodBase.GetCurrentMethod(),patNum,showInactive);
			}
			string command="SELECT * FROM allergy WHERE PatNum = "+POut.Long(patNum);
			if(!showInactive) {
				command+=" AND StatusIsActive<>0";
			}
			return Crud.AllergyCrud.SelectMany(command);
		}

		public static List<long> GetChangedSinceAllergyNums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT AllergyNum FROM allergy WHERE DateTStamp > "+POut.DateT(changedSince);
			DataTable dt=Db.GetTable(command);
			List<long> allergynums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				allergynums.Add(PIn.Long(dt.Rows[i]["AllergyNum"].ToString()));
			}
			return allergynums;
		}

		///<summary>Used along with GetChangedSinceAllergyNums</summary>
		public static List<Allergy> GetMultAllergies(List<long> allergyNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Allergy>>(MethodBase.GetCurrentMethod(),allergyNums);
			}
			string strAllergyNums="";
			DataTable table;
			if(allergyNums.Count>0) {
				for(int i=0;i<allergyNums.Count;i++) {
					if(i>0) {
						strAllergyNums+="OR ";
					}
					strAllergyNums+="AllergyNum='"+allergyNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM allergy WHERE "+strAllergyNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			Allergy[] multAllergies=Crud.AllergyCrud.TableToList(table).ToArray();
			List<Allergy> allergyList=new List<Allergy>(multAllergies);
			return allergyList;
		}

		///<summary>Returns an array of all patient names who are using this allergy.</summary>
		public static string[] GetPatNamesForAllergy(long allergyDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<string[]>(MethodBase.GetCurrentMethod(),allergyDefNum);
			}
			string command="SELECT CONCAT(CONCAT(CONCAT(CONCAT(LName,', '),FName),' '),Preferred) FROM allergy,patient "
				+"WHERE allergy.PatNum=patient.PatNum "
				+"AND allergy.AllergyDefNum="+POut.Long(allergyDefNum);
			DataTable table=Db.GetTable(command);
			string[] retVal=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++) {
				retVal[i]=PIn.String(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Returns a list of PatNums that have an allergy from the PatNums that are passed in.</summary>
		public static List<long> GetPatientsWithAllergy(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums.Count==0) {
				return new List<long>();
			}
			string command="SELECT DISTINCT PatNum FROM allergy WHERE PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND allergy.AllergyDefNum != "+POut.Long(PrefC.GetLong(PrefName.AllergiesIndicateNone));
			return Db.GetListLong(command);
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all allergies of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE allergy SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all allergies of a patient that are the status specified</summary>
		public static void ResetTimeStamps(long patNum, bool onlyActive) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,onlyActive);
				return;
			}
			string command="UPDATE allergy SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			if(onlyActive) {
				command+=" AND StatusIsActive = "+POut.Bool(onlyActive);
			}
			Db.NonQ(command);
		}
	}
}