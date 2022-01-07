using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class LabResultms{
		#region Only used for webserver for Patient Portal.
		///<summary>Gets one LabResultm from the db.</summary>
		public static LabResultm GetOne(long customerNum,long labResultNum) {
			return Crud.LabResultmCrud.SelectOne(customerNum,labResultNum);
		}
		///<summary>Gets one LabResultm from the db.</summary>
		public static List<LabResultm> GetLabResultms(long customerNum,long labPanelNum) {
			string command=
					"SELECT * from labresultm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND LabPanelNum = "+POut.Long(labPanelNum);
			return Crud.LabResultmCrud.SelectMany(command);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceLabResultNums(DateTime changedSince) {
			return LabResults.GetChangedSinceLabResultNums(changedSince);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<LabResultm> GetMultLabResultms(List<long> labResultNums) {
			List<LabResult> LabResultList=LabResults.GetMultLabResults(labResultNums);
			List<LabResultm> LabResultmList=ConvertListToM(LabResultList);
			return LabResultmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<LabResultm> ConvertListToM(List<LabResult> list) {
			List<LabResultm> retVal=new List<LabResultm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.LabResultmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		public static void UpdateFromChangeList(List<LabResultm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				LabResultm labresultm=Crud.LabResultmCrud.SelectOne(customerNum,list[i].LabResultNum);
				if(labresultm==null) {//not in db
					Crud.LabResultmCrud.Insert(list[i],true);
				}
				else {
					Crud.LabResultmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM labresultm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<LabResultm> Refresh(long patNum){
			string command="SELECT * FROM labresultm WHERE PatNum = "+POut.Long(patNum);
			return Crud.LabResultmCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(LabResultm labResultm){
			return Crud.LabResultmCrud.Insert(labResultm,true);
		}

		///<summary></summary>
		public static void Update(LabResultm labResultm){
			Crud.LabResultmCrud.Update(labResultm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long labResultNum) {
			string command= "DELETE FROM labresultm WHERE CustomerNum = "+POut.Long(customerNum)+" AND LabResultNum = "+POut.Long(labResultNum);
			Db.NonQ(command);
		}


		*/



	}
}