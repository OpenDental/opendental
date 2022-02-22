using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class Diseasems{
		#region Only used for webserver for mobile.
		///<summary>Gets all Diseasem for a single patient </summary>
		public static List<Diseasem> GetDiseasems(long customerNum,long patNum) {
			string command=
					"SELECT * from diseasem "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
			return Crud.DiseasemCrud.SelectMany(command);
		}

		public static DataTable GetDiseasemDetails(long customerNum,long patNum) {
			string command=
				"SELECT  icd9m.Description from diseasem  LEFT JOIN icd9m on icd9m.ICD9Num=diseasem.ICD9Num "
				+"WHERE diseasem.CustomerNum = "+POut.Long(customerNum)
					+" AND diseasem.PatNum = "+POut.Long(patNum)
					+" AND diseasem.ProbStatus = "+POut.Int((int)ProblemStatus.Active) // get only active diseases
					+" AND diseasem.ICD9Num !=0" //get only ICD9NUM which are not zero. ICD9NUM and DiseaseDefNum are mutually exculsive. If one is zero the other is not.
					+" AND icd9m.CustomerNum = "+POut.Long(customerNum);
			return Db.GetTable(command);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceDiseaseNums(DateTime changedSince,List<long> eligibleForUploadPatNumList) {
			return Diseases.GetChangedSinceDiseaseNums(changedSince,eligibleForUploadPatNumList);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<Diseasem> GetMultDiseasems(List<long> diseaseNums) {
			List<Disease> diseaseList=Diseases.GetMultDiseases(diseaseNums);
			List<Diseasem> diseasemList=ConvertListToM(diseaseList);
			return diseasemList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<Diseasem> ConvertListToM(List<Disease> list) {
			List<Diseasem> retVal=new List<Diseasem>();
			for(int i=0;i<list.Count;i++){
				retVal.Add(Crud.DiseasemCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<Diseasem> list,long customerNum) {
			for(int i=0;i<list.Count;i++){
				list[i].CustomerNum=customerNum;
				Diseasem diseasem=Crud.DiseasemCrud.SelectOne(customerNum,list[i].DiseaseNum);
				if(diseasem==null){//not in db
					Crud.DiseasemCrud.Insert(list[i],true);
				}
				else{
					Crud.DiseasemCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM diseasem WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}

		///<summary>Delete all diseases of a particular patient</summary>
		public static void Delete(long customerNum,long PatNum) {
			string command= "DELETE FROM diseasem WHERE CustomerNum = "+POut.Long(customerNum)+" AND PatNum = "+POut.Long(PatNum);
			Db.NonQ(command);
		}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Diseasem> Refresh(long patNum){
			string command="SELECT * FROM diseasem WHERE PatNum = "+POut.Long(patNum);
			return Crud.DiseasemCrud.SelectMany(command);
		}

		///<summary>Gets one Diseasem from the db.</summary>
		public static Diseasem GetOne(long customerNum,long diseaseNum){
			return Crud.DiseasemCrud.SelectOne(customerNum,diseaseNum);
		}

		///<summary></summary>
		public static long Insert(Diseasem diseasem){
			return Crud.DiseasemCrud.Insert(diseasem,true);
		}

		///<summary></summary>
		public static void Update(Diseasem diseasem){
			Crud.DiseasemCrud.Update(diseasem);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long diseaseNum) {
			string command= "DELETE FROM diseasem WHERE CustomerNum = "+POut.Long(customerNum)+" AND DiseaseNum = "+POut.Long(diseaseNum);
			Db.NonQ(command);
		}




		*/



	}
}