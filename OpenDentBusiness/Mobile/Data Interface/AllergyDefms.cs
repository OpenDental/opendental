using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class AllergyDefms{

		#region Only used for webserver for Patient Portal.
		///<summary>Gets one Medicationm from the db.</summary>
		public static AllergyDefm GetOne(long customerNum,long allergyNum) {
			return Crud.AllergyDefmCrud.SelectOne(customerNum,allergyNum);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceAllergyDefNums(DateTime changedSince) {
			return AllergyDefs.GetChangedSinceAllergyDefNums(changedSince);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<AllergyDefm> GetMultAllergyDefms(List<long> allergyDefNums) {
			List<AllergyDef> AllergyDefList=AllergyDefs.GetMultAllergyDefs(allergyDefNums);
			List<AllergyDefm> AllergyDefmList=ConvertListToM(AllergyDefList);
			return AllergyDefmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<AllergyDefm> ConvertListToM(List<AllergyDef> list) {
			List<AllergyDefm> retVal=new List<AllergyDefm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.AllergyDefmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<AllergyDefm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				AllergyDefm allergyDefm=Crud.AllergyDefmCrud.SelectOne(customerNum,list[i].AllergyDefNum);
				if(allergyDefm==null) {//not in db
					Crud.AllergyDefmCrud.Insert(list[i],true);
				}
				else {
					Crud.AllergyDefmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM allergydefm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<AllergyDefm> Refresh(long patNum){
			string command="SELECT * FROM allergydefm WHERE PatNum = "+POut.Long(patNum);
			return Crud.AllergyDefmCrud.SelectMany(command);
		}

		///<summary>Gets one AllergyDefm from the db.</summary>
		public static AllergyDefm GetOne(long customerNum,long allergyDefNum){
			return Crud.AllergyDefmCrud.SelectOne(customerNum,allergyDefNum);
		}

		///<summary></summary>
		public static long Insert(AllergyDefm allergyDefm){
			return Crud.AllergyDefmCrud.Insert(allergyDefm,true);
		}

		///<summary></summary>
		public static void Update(AllergyDefm allergyDefm){
			Crud.AllergyDefmCrud.Update(allergyDefm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long allergyDefNum) {
			string command= "DELETE FROM allergydefm WHERE CustomerNum = "+POut.Long(customerNum)+" AND AllergyDefNum = "+POut.Long(allergyDefNum);
			Db.NonQ(command);
		}




		*/



	}
}