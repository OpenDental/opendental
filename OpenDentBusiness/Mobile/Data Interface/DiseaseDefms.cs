using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class DiseaseDefms{

		#region Only used for webserver for Patient Portal.
		///<summary>Gets one Medicationm from the db.</summary>
		public static DiseaseDefm GetOne(long customerNum,long diseaseNum) {
			return Crud.DiseaseDefmCrud.SelectOne(customerNum,diseaseNum);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceDiseaseDefNums(DateTime changedSince) {
			return DiseaseDefs.GetChangedSinceDiseaseDefNums(changedSince);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<DiseaseDefm> GetMultDiseaseDefms(List<long> diseaseDefNums) {
			List<DiseaseDef> DiseaseDefList=DiseaseDefs.GetMultDiseaseDefs(diseaseDefNums);
			List<DiseaseDefm> DiseaseDefmList=ConvertListToM(DiseaseDefList);
			return DiseaseDefmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<DiseaseDefm> ConvertListToM(List<DiseaseDef> list) {
			List<DiseaseDefm> retVal=new List<DiseaseDefm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.DiseaseDefmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<DiseaseDefm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				DiseaseDefm diseaseDefm=Crud.DiseaseDefmCrud.SelectOne(customerNum,list[i].DiseaseDefNum);
				if(diseaseDefm==null) {//not in db
					Crud.DiseaseDefmCrud.Insert(list[i],true);
				}
				else {
					Crud.DiseaseDefmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM diseasedefm WHERE CustomerNum = "+POut.Long(customerNum);
			Db.NonQ(command);
		}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<DiseaseDefm> Refresh(long patNum){
			string command="SELECT * FROM diseasedefm WHERE PatNum = "+POut.Long(patNum);
			return Crud.DiseaseDefmCrud.SelectMany(command);
		}

		///<summary>Gets one DiseaseDefm from the db.</summary>
		public static DiseaseDefm GetOne(long customerNum,long diseaseDefNum){
			return Crud.DiseaseDefmCrud.SelectOne(customerNum,diseaseDefNum);
		}

		///<summary></summary>
		public static long Insert(DiseaseDefm diseaseDefm){
			return Crud.DiseaseDefmCrud.Insert(diseaseDefm,true);
		}

		///<summary></summary>
		public static void Update(DiseaseDefm diseaseDefm){
			Crud.DiseaseDefmCrud.Update(diseaseDefm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long diseaseDefNum) {
			string command= "DELETE FROM diseasedefm WHERE CustomerNum = "+POut.Long(customerNum)+" AND DiseaseDefNum = "+POut.Long(diseaseDefNum);
			Db.NonQ(command);
		}




		*/



	}
}