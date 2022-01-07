using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class RxPatms {

		#region Only used for webserver for mobile.
			public static RxPatm GetOne(long customerNum,long rxNum) {
				return Crud.RxPatmCrud.SelectOne(customerNum,rxNum);
			}

			///<summary>Gets all RxPatm for a single patient </summary>
			public static List<RxPatm> GetRxPatms(long customerNum,long patNum) {
				string command=
					"SELECT * from rxpatm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
				return Crud.RxPatmCrud.SelectMany(command);
			}
		#endregion

		#region Used only on OD
			///<summary>The values returned are sent to the webserver.</summary>
			public static List<long> GetChangedSinceRxNums(DateTime changedSince) {
				return RxPats.GetChangedSinceRxNums(changedSince);
			}

			///<summary>The values returned are sent to the webserver.</summary>
			public static List<RxPatm> GetMultRxPats(List<long> rxNums) {
				List<RxPat> rxList=RxPats.GetMultRxPats(rxNums);
				List<RxPatm> RxmList=ConvertListToM(rxList);
				return RxmList;
			}

			///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
			public static List<RxPatm> ConvertListToM(List<RxPat> list) {
				List<RxPatm> retVal=new List<RxPatm>();
				for(int i=0;i<list.Count;i++) {
					retVal.Add(Crud.RxPatmCrud.ConvertToM(list[i]));
				}
				return retVal;
			}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
			///<summary>Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
			public static void UpdateFromChangeList(List<RxPatm> list,long customerNum) {
				for(int i=0;i<list.Count;i++) {
					list[i].CustomerNum=customerNum;
					RxPatm rxPatm=Crud.RxPatmCrud.SelectOne(customerNum,list[i].RxNum);
					if(rxPatm==null) {//not in db
						Crud.RxPatmCrud.Insert(list[i],true);
					}
					else {
						Crud.RxPatmCrud.Update(list[i]);
					}
				}
			}

			///<summary>used in tandem with Full synch</summary>
			public static void DeleteAll(long customerNum) {
				string command= "DELETE FROM rxpatm WHERE CustomerNum = "+POut.Long(customerNum); ;
				Db.NonQ(command);
			}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<RxPatm> Refresh(long patNum){
			string command="SELECT * FROM rxpatm WHERE PatNum = "+POut.Long(patNum);
			return Crud.RxPatmCrud.SelectMany(command);
		}

		///<summary>Gets one RxPatm from the db.</summary>


		///<summary></summary>
		public static long Insert(RxPatm rxPatm){
			return Crud.RxPatmCrud.Insert(rxPatm,true);
		}

		///<summary></summary>
		public static void Update(RxPatm rxPatm){
			Crud.RxPatmCrud.Update(rxPatm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long rxNum) {
			string command= "DELETE FROM rxpatm WHERE CustomerNum = "+POut.Long(customerNum)+" AND RxNum = "+POut.Long(rxNum);
			Db.NonQ(command);
		}


		*/



	}
}