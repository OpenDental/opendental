using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness.Mobile {
	///<summary></summary>
	public class Patientms {

		#region Only used on webserver for mobile web.
			///<summary>Gets one Patientm from the db.</summary>
			public static Patientm GetOne(long customerNum,long patNum) {
				return Crud.PatientmCrud.SelectOne(customerNum,patNum);
			}

			///<summary>Gets Patientms from the db as specified by the search string. Limit to 20 </summary>
			public static List<Patientm> GetPatientms(long customerNum,string searchterm) {
				string command="SELECT * FROM patientm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)+ " "
					+" AND LName like '"+POut.String(searchterm)+"%'"+" LIMIT 30";
				return Crud.PatientmCrud.SelectMany(command);
			}

			///<summary>Gets Family Members who are patients</summary>
			public static List<Patientm> GetPatientmsOfFamily(long customerNum,long patNum) {
				string command="SELECT * FROM patientm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)+ " "
					+"AND guarantor in "
					+"(SELECT guarantor FROM patientm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)+ " "
					+"AND PatNum ="+POut.Long(patNum)+ ")";
				return Crud.PatientmCrud.SelectMany(command);
			}

			///<summary>Converts a date to an age. If age is over 115, then returns 0.</summary>
			public static int DateToAge(DateTime date) {
				return Patients.DateToAge(date);
			}
		#endregion

		#region Used only on OD
			public static List<long> GetChangedSincePatNums(DateTime changedSince) {
				return Patients.GetChangedSincePatNums(changedSince);
			}

			///<summary>The values returned are sent to the webserver. Used if GetChanged returns large recordsets.</summary>
			public static List<Patientm> GetMultPats(List<long> patNums) {
				Patient[]  patientArray=Patients.GetMultPats(patNums);
				List<Patient> patientList=new List<Patient>(patientArray);
				List<Patientm> patientmList=ConvertListToM(patientList);
				return patientmList;
			}

			///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
			public static List<Patientm> ConvertListToM(List<Patient> list) {
				List<Patientm> retVal=new List<Patientm>();
				for(int i=0;i<list.Count;i++) {
					retVal.Add(Crud.PatientmCrud.ConvertToM(list[i]));
				}
				return retVal;
			}

			/// <summary>Gets PatNums of patients whose online password is blank</summary>
			public static List<long> GetPatNumsForDeletion() {
				return Patients.GetPatNumsForDeletion();
			}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
			///<summary>Takes the list of changes from the dental office and makes updates to those items in the mobile server db.</summary>
			public static void UpdateFromChangeList(List<Patientm> list,long customerNum) {
				for(int i=0;i<list.Count;i++) {
					list[i].CustomerNum=customerNum;
					Patientm patientm=Crud.PatientmCrud.SelectOne(customerNum,list[i].PatNum);
					if(patientm==null) {//not in db
						Crud.PatientmCrud.Insert(list[i],true);
					}
					else {
						Crud.PatientmCrud.Update(list[i]);
					}
				}
			}

			///<summary>used in tandem with Full synch</summary>
			public static void DeleteAll(long customerNum) {
				string command= "DELETE FROM patientm WHERE CustomerNum = "+POut.Long(customerNum); ;
				Db.NonQ(command);
			}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Patientm> Refresh(long patNum){
			string command="SELECT * FROM patientm WHERE PatNum = "+POut.Long(patNum);
			return Crud.PatientmCrud.SelectMany(command);
		}

		 ///<summary>This would be executed on the webserver only</summary>
		public static long Insert(Patientm patientm) {
			return Crud.PatientmCrud.Insert(patientm,true);
		}

		///<summary>This would be executed on the webserver only</summary>
		public static void Update(Patientm patientm) {
			Crud.PatientmCrud.Update(patientm);
		}

		///<summary>This would be executed on the webserver only</summary>
		public static void Delete(long customerNum,long patNum) {
			Crud.PatientmCrud.Delete(customerNum, patNum);
		}

	
		///<summary>The values returned are sent to the webserver.</summary>
		public static List<Patientm> GetChanged(DateTime changedSince) {
			List<Patient> ChangedPatientList=Patients.GetChangedSince(changedSince);
			List<Patientm> ChangedPatientmList=ConvertListToM(ChangedPatientList);
			return ChangedPatientmList;
		}

		*/



	}
}