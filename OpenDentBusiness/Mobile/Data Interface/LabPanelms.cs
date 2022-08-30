using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class LabPanelms{
		#region Only used for webserver for Patient Portal.
		///<summary>Gets all LabPanelm for a single patient </summary>
		public static List<LabPanelm> GetLabPanelms(long customerNum,long patNum) {
			string command=
					"SELECT * from labpanelm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
			return Crud.LabPanelmCrud.SelectMany(command);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceLabPanelNums(DateTime changedSince,List<long> eligibleForUploadPatNumList) {
			return LabPanels.GetChangedSinceLabPanelNums(changedSince,eligibleForUploadPatNumList);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<LabPanelm> GetMultLabPanelms(List<long> labPanelNums) {
			List<LabPanel> LabPanelList=LabPanels.GetMultLabPanels(labPanelNums);
			List<LabPanelm> LabPanelmList=ConvertListToM(LabPanelList);
			return LabPanelmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<LabPanelm> ConvertListToM(List<LabPanel> list) {
			List<LabPanelm> retVal=new List<LabPanelm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.LabPanelmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		public static void UpdateFromChangeList(List<LabPanelm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				LabPanelm labPanelm=Crud.LabPanelmCrud.SelectOne(customerNum,list[i].LabPanelNum);
				if(labPanelm==null) {//not in db
					Crud.LabPanelmCrud.Insert(list[i],true);
				}
				else {
					Crud.LabPanelmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM labpanelm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}

		///<summary>Delete all labpanels of a particular patient</summary>
		public static void Delete(long customerNum,long PatNum) {
			string command= "DELETE FROM labpanelm WHERE CustomerNum = "+POut.Long(customerNum)+" AND PatNum = "+POut.Long(PatNum);
			Db.NonQ(command);
		}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<LabPanelm> Refresh(long patNum){
			string command="SELECT * FROM labpanelm WHERE PatNum = "+POut.Long(patNum);
			return Crud.LabPanelmCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(LabPanelm labPanelm){
			return Crud.LabPanelmCrud.Insert(labPanelm,true);
		}

		///<summary></summary>
		public static void Update(LabPanelm labPanelm){
			Crud.LabPanelmCrud.Update(labPanelm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long labPanelNum) {
			string command= "DELETE FROM labpanelm WHERE CustomerNum = "+POut.Long(customerNum)+" AND LabPanelNum = "+POut.Long(labPanelNum);
			Db.NonQ(command);
		}


		*/



	}
}