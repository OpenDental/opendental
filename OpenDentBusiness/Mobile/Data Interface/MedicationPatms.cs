using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class MedicationPatms{
		#region Only used for webserver for mobile.
		///<summary>Gets all MedicationPatm for a single patient </summary>
		public static List<MedicationPatm> GetMedicationPatms(long customerNum,long patNum) {
			string command=
					"SELECT * from medicationpatm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
			return Crud.MedicationPatmCrud.SelectMany(command);
		}

		
		public static DataTable GetMedicationmDetails(long customerNum,long patNum) {
			string command=
				"SELECT  medicationm.MedName from medicationpatm  LEFT JOIN medicationm on medicationpatm.MedicationNum=medicationm.MedicationNum "
				+"WHERE medicationpatm.CustomerNum = "+POut.Long(customerNum)
					+" AND medicationpatm.PatNum = "+POut.Long(patNum)
					+" AND medicationpatm.DateStop = "+POut.Date(DateTime.MinValue) // filter out discontinued medications.
					+" AND medicationm.CustomerNum = "+POut.Long(customerNum);
			return Db.GetTable(command);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceMedicationPatNums(DateTime changedSince,List<long> eligibleForUploadPatNumList) {
			return MedicationPats.GetChangedSinceMedicationPatNums(changedSince,eligibleForUploadPatNumList);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<MedicationPatm> GetMultMedicationPatms(List<long> rxNums) {
			List<MedicationPat> medicationPatList=MedicationPats.GetMultMedicationPats(rxNums);
			List<MedicationPatm> medicationPatmList=ConvertListToM(medicationPatList);
			return medicationPatmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<MedicationPatm> ConvertListToM(List<MedicationPat> list) {
			List<MedicationPatm> retVal=new List<MedicationPatm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.MedicationPatmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<MedicationPatm> list,long customerNum) {
			for(int i=0;i<list.Count;i++){
				list[i].CustomerNum=customerNum;
				MedicationPatm medicationPatm=Crud.MedicationPatmCrud.SelectOne(customerNum,list[i].MedicationPatNum);
				if(medicationPatm==null){//not in db
					Crud.MedicationPatmCrud.Insert(list[i],true);
				}
				else{
					Crud.MedicationPatmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM medicationpatm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}

		///<summary>Delete all medicationpats of a particular patient</summary>
		public static void Delete(long customerNum,long PatNum) {
			string command= "DELETE FROM medicationpatm WHERE CustomerNum = "+POut.Long(customerNum)+" AND PatNum = "+POut.Long(PatNum);
			Db.NonQ(command);
		}

		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<MedicationPatm> Refresh(long patNum){
			string command="SELECT * FROM medicationpatm WHERE PatNum = "+POut.Long(patNum);
			return Crud.MedicationPatmCrud.SelectMany(command);
		}

		///<summary>Gets one MedicationPatm from the db.</summary>
		public static MedicationPatm GetOne(long customerNum,long medicationPatNum){
			return Crud.MedicationPatmCrud.SelectOne(customerNum,medicationPatNum);
		}

		///<summary></summary>
		public static long Insert(MedicationPatm medicationPatm){
			return Crud.MedicationPatmCrud.Insert(medicationPatm,true);
		}

		///<summary></summary>
		public static void Update(MedicationPatm medicationPatm){
			Crud.MedicationPatmCrud.Update(medicationPatm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long medicationPatNum) {
			string command= "DELETE FROM medicationpatm WHERE CustomerNum = "+POut.Long(customerNum)+" AND MedicationPatNum = "+POut.Long(medicationPatNum);
			Db.NonQ(command);
		}


		*/



	}
}