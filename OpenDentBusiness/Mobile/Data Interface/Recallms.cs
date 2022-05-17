using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile {
	///<summary></summary>
	public class Recallms {
	
		#region Only used for webserver for mobile.
		///<summary>Gets all Recallm for a single patient </summary>
		public static List<Recallm> GetRecallms(long customerNum,long patNum) {
			string command=
					"SELECT * from recallm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
			return Crud.RecallmCrud.SelectMany(command);
		}
	
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceRecallNums(DateTime changedSince) {
			return Recalls.GetChangedSinceRecallNums(changedSince);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<Recallm> GetMultRecallms(List<long> recallNums) {
			List<Recall> recallList=Recalls.GetMultRecalls(recallNums);
			List<Recallm> recallmList=ConvertListToM(recallList);
			return recallmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<Recallm> ConvertListToM(List<Recall> list) {
			List<Recallm> retVal=new List<Recallm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.RecallmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<Recallm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				Recallm recallm=Crud.RecallmCrud.SelectOne(customerNum,list[i].RecallNum);
				if(recallm==null) {//not in db
					Crud.RecallmCrud.Insert(list[i],true);
				}
				else {
					Crud.RecallmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM recallm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}

		#endregion




		 
		 



	}
}