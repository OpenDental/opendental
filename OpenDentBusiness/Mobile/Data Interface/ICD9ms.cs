using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class ICD9ms{
		#region Only used for webserver for Patient Portal.
		///<summary>Gets one Medicationm from the db.</summary>
		public static ICD9m GetOne(long customerNum,long icd9Num) {
			return Crud.ICD9mCrud.SelectOne(customerNum,icd9Num);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceICD9Nums(DateTime changedSince) {
			return ICD9s.GetChangedSinceICD9Nums(changedSince);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<ICD9m> GetMultICD9ms(List<long> icd9Nums) {
			List<ICD9> ICD9List=ICD9s.GetMultICD9s(icd9Nums);
			List<ICD9m> ICD9mList=ConvertListToM(ICD9List);
			return ICD9mList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<ICD9m> ConvertListToM(List<ICD9> list) {
			List<ICD9m> retVal=new List<ICD9m>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.ICD9mCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<ICD9m> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				ICD9m iCD9m=Crud.ICD9mCrud.SelectOne(customerNum,list[i].ICD9Num);
				if(iCD9m==null) {//not in db
					Crud.ICD9mCrud.Insert(list[i],true);
				}
				else {
					Crud.ICD9mCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM icd9m WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ICD9m> Refresh(long patNum){
			string command="SELECT * FROM icd9m WHERE PatNum = "+POut.Long(patNum);
			return Crud.ICD9mCrud.SelectMany(command);
		}

		///<summary>Gets one ICD9m from the db.</summary>
		public static ICD9m GetOne(long customerNum,long iCD9Num){
			return Crud.ICD9mCrud.SelectOne(customerNum,iCD9Num);
		}

		///<summary></summary>
		public static long Insert(ICD9m iCD9m){
			return Crud.ICD9mCrud.Insert(iCD9m,true);
		}

		///<summary></summary>
		public static void Update(ICD9m iCD9m){
			Crud.ICD9mCrud.Update(iCD9m);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long iCD9Num) {
			string command= "DELETE FROM icd9m WHERE CustomerNum = "+POut.Long(customerNum)+" AND ICD9Num = "+POut.Long(iCD9Num);
			Db.NonQ(command);
		}




		*/



	}
}