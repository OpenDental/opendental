using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class Medicationms {

		#region Only used for webserver for Patient Portal.
		///<summary>Gets one Medicationm from the db.</summary>
		public static Medicationm GetOne(long customerNum,long medicationNum) {
			return Crud.MedicationmCrud.SelectOne(customerNum,medicationNum);
		}
		#endregion

		#region Used only on OD
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<long> GetChangedSinceMedicationNums(DateTime changedSince) {
			return Medications.GetChangedSinceMedicationNums(changedSince);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<Medicationm> GetMultMedicationms(List<long> medicationNums) {
			List<Medication> MedicationList=Medications.GetMultMedications(medicationNums);
			List<Medicationm> MedicationmList=ConvertListToM(MedicationList);
			return MedicationmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<Medicationm> ConvertListToM(List<Medication> list) {
			List<Medicationm> retVal=new List<Medicationm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.MedicationmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		public static void UpdateFromChangeList(List<Medicationm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				Medicationm medicationm=Crud.MedicationmCrud.SelectOne(customerNum,list[i].MedicationNum);
				if(medicationm==null) {//not in db
					Crud.MedicationmCrud.Insert(list[i],true);
				}
				else {
					Crud.MedicationmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM medicationm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Medicationm> Refresh(long patNum){
			string command="SELECT * FROM medicationm WHERE PatNum = "+POut.Long(patNum);
			return Crud.MedicationmCrud.SelectMany(command);
		}



		///<summary></summary>
		public static long Insert(Medicationm medicationm){
			return Crud.MedicationmCrud.Insert(medicationm,true);
		}

		///<summary></summary>
		public static void Update(Medicationm medicationm){
			Crud.MedicationmCrud.Update(medicationm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long medicationNum) {
			string command= "DELETE FROM medicationm WHERE CustomerNum = "+POut.Long(customerNum)+" AND MedicationNum = "+POut.Long(medicationNum);
			Db.NonQ(command);
		}




		*/



	}
}