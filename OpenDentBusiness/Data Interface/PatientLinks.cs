using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PatientLinks{
		#region Get Methods
		///<summary>Gets all of the 'PatNumTo's linked to the passed-in patNumFrom. 
		///Does not recursively look up additional patient links.
		///The list returned will NOT include the patient passed in unless there is an entry in the database linking them to... themselves.</summary>
		public static List<long> GetPatNumsLinkedFrom(long patNumFrom,PatientLinkType patLinkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patNumFrom,patLinkType);
			}
			string command="SELECT PatNumTo FROM patientlink "
				+"WHERE PatNumFrom="+POut.Long(patNumFrom)+" "
				+"AND LinkType="+POut.Int((int)patLinkType);
			return Db.GetListLong(command);
		}

		///<summary>Gets all of the 'PatNumFroms's linked to the passed-in patNumTo.   Does not recursively look up additional patient links.
		///The list returned will NOT include the patient passed in unless there is an entry in the database linking them to... themselves.</summary>
		public static List<long> GetPatNumsLinkedTo(long patNumTo,PatientLinkType patLinkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patNumTo,patLinkType);
			}
			string command="SELECT PatNumFrom FROM patientlink "
				+"WHERE PatNumTo="+POut.Long(patNumTo)+" "
				+"AND LinkType="+POut.Int((int)patLinkType);
			return Db.GetListLong(command);
		}

		///<summary>Gets links to and from the patient passed in. Not recursive.</summary>
		public static List<PatientLink> GetLinks(long patNum,PatientLinkType patLinkType) {
			return GetLinks(new List<long> { patNum },patLinkType);
		}

		///<summary>Gets links to and from the patients passed in. Not recursive.</summary>
		public static List<PatientLink> GetLinks(List<long> listPatNums,PatientLinkType patLinkType) {
			if(listPatNums.Count==0) {
				return new List<PatientLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatientLink>>(MethodBase.GetCurrentMethod(),listPatNums,patLinkType);
			}
			string command="SELECT * FROM patientlink "
				+"WHERE (PatNumTo IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+") "
					+"OR PatNumFrom IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+")) "
				+"AND LinkType="+POut.Int((int)patLinkType);
			return Crud.PatientLinkCrud.SelectMany(command);
		}

		///<summary>Gets all the PatNums that are linked to this PatNum and all the PatNums that are linked to those PatNums, etc. Always returns a list
		///that contains at least the PatNum passed in.</summary>
		public static List<long> GetPatNumsLinkedFromRecursive(long patNumFrom,PatientLinkType patLinkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patNumFrom,patLinkType);
			}
			List<long> listPatNums=new List<long> { patNumFrom };
			AddPatNumsLinkedFromRecursive(patNumFrom,patLinkType,listPatNums);
			return listPatNums;
		}

		///<summary>Gets all the PatNums that are recursively linked to this PatNum and adds them to the list passed in.</summary>
		private static void AddPatNumsLinkedFromRecursive(long patNumFrom,PatientLinkType patLinkType,List<long> listPatNums) {
			//No need to check RemotingRole; private method.
			string command="SELECT PatNumTo FROM patientlink "
				+"WHERE PatNumFrom="+POut.Long(patNumFrom)+" "
				+"AND LinkType="+POut.Int((int)patLinkType);
			List<long> listPatNumTos=Db.GetListLong(command);
			if(listPatNumTos.Count==0) {
				return;//Base case
			}
			foreach(long patNumTo in listPatNumTos) {
				if(listPatNums.Contains(patNumTo)) {
					continue;//So that a patient that links to itself does not cause an infinite circle of recursion.
				}
				listPatNums.Add(patNumTo);
				AddPatNumsLinkedFromRecursive(patNumTo,patLinkType,listPatNums);//Find all the patients that are linked to the "To" patient.
			}
		}

		///<summary>Gets all the PatNums that are linked to this PatNum and all the PatNums that are linked to those PatNums, etc. Always returns a list
		///that contains at least the PatNum passed in.</summary>
		public static List<long> GetPatNumsLinkedToRecursive(long patNumTo,PatientLinkType patLinkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patNumTo,patLinkType);
			}
			List<long> listPatNums=new List<long> { patNumTo };
			AddPatNumsLinkedToRecursive(patNumTo,patLinkType,listPatNums);
			return listPatNums;
		}

		///<summary>Gets all the PatNums that are recursively linked to this PatNum and adds them to the list passed in.</summary>
		private static void AddPatNumsLinkedToRecursive(long patNumTo,PatientLinkType patLinkType,List<long> listPatNums) {
			//No need to check RemotingRole; private method.
			string command="SELECT PatNumFrom FROM patientlink "
				+"WHERE PatNumTo="+POut.Long(patNumTo)+" "
				+"AND LinkType="+POut.Int((int)patLinkType);
			List<long> listPatNumFroms=Db.GetListLong(command);
			if(listPatNumFroms.Count==0) {
				return;//Base case
			}
			foreach(long patNumFrom in listPatNumFroms) {
				if(listPatNums.Contains(patNumFrom)) {
					continue;//So that a patient that links to itself does not cause an infinite circle of recursion.
				}
				listPatNums.Add(patNumFrom);
				AddPatNumsLinkedToRecursive(patNumFrom,patLinkType,listPatNums);//Find all the patients that are linked to the from patient.
			}
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(PatientLink patientLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				patientLink.PatientLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),patientLink);
				return patientLink.PatientLinkNum;
			}
			return Crud.PatientLinkCrud.Insert(patientLink);
		}
		#endregion

		#region Delete
		///<summary>Deletes all of the entries for the patNumFrom passed in of the specified type.</summary>
		public static void DeletePatNumFroms(long patNumFrom,PatientLinkType patLinkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNumFrom,patLinkType);
				return;
			}
			string command="DELETE FROM patientlink "
				+"WHERE PatNumFrom="+POut.Long(patNumFrom)+" "
				+"AND LinkType="+POut.Int((int)patLinkType);
			Db.NonQ(command);
		}

		///<summary>Deletes all of the entries for the patNumTo passed in of the specified type.</summary>
		public static void DeletePatNumTos(long patNumTo,PatientLinkType patLinkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNumTo,patLinkType);
				return;
			}
			string command="DELETE FROM patientlink "
				+"WHERE PatNumTo="+POut.Long(patNumTo)+" "
				+"AND LinkType="+POut.Int((int)patLinkType);
			Db.NonQ(command);
		}
		#endregion

		#region Clone Methods
		///<summary>Returns the original patient's PatNum for the clone passed in.  Returns the patNum passed in if it is not a clone.
		///Otherwise returns 0 if the master could not be found.</summary>
		public static long GetOriginalPatNumFromClone(long patNum) {
			//No need to check RemotingRole; no call to db.
			if(!IsPatientAClone(patNum)) {
				return patNum;//Not a clone so this patient must be the original.
			}
			long patNumOriginal=0;
			List<long> listMatchingClonePatNums=GetPatNumsLinkedTo(patNum,PatientLinkType.Clone);
			if(listMatchingClonePatNums!=null && listMatchingClonePatNums.Count > 0) {
				patNumOriginal=listMatchingClonePatNums[0];
			}
			return patNumOriginal;
		}

		///<summary>Returns true if the patient passed in is a clone otherwise false.</summary>
		public static bool IsPatientAClone(long patNum) {
			//No need to check RemotingRole; no call to db.
			List<long> listMatchingClonePatNums=GetPatNumsLinkedTo(patNum,PatientLinkType.Clone);
			return (listMatchingClonePatNums.Count > 0);
		}

		///<summary>Returns true if the patient passed in is a clone or the original patient of clones, otherwise false.
		///This method is helpful when trying to determine if the patient passed in is related in any way to the clone system.</summary>
		public static bool IsPatientACloneOrOriginal(long patNum) {
			//No need to check RemotingRole; no call to db.
			List<long> listMatchingClonePatNums=GetPatNumsLinkedTo(patNum,PatientLinkType.Clone);
			List<long> listMatchingMasterPatNums=GetPatNumsLinkedFrom(patNum,PatientLinkType.Clone);
			return (listMatchingClonePatNums.Count > 0 || listMatchingMasterPatNums.Count > 0);
		}

		///<summary>Returns true if one patient is a clone of the other or if both are clones of the same master, otherwise false.
		///Always returns false if patNum1 and patNum2 are the same PatNum.</summary>
		public static bool ArePatientsClonesOfEachOther(long patNum1,long patNum2) {
			//No need to check RemotingRole; no call to db.
			if(patNum1==patNum2) {//A patient is not considered a clone of themselves.  Even if the database has this scenario we do not honor it.
				return false;
			}
			//First check if patNum1 is a master of patNum2
			List<long> listPatNums=GetPatNumsLinkedFrom(patNum1,PatientLinkType.Clone);
			if(listPatNums.Contains(patNum2)) {
				return true;
			}
			//Then check if patNum2 is a master of patNum1
			listPatNums=GetPatNumsLinkedFrom(patNum2,PatientLinkType.Clone);
			if(listPatNums.Contains(patNum1)) {
				return true;
			}
			//Finally, check to see if both patients passed in are clones and if they are, check if they share the same master.
			if(IsPatientAClone(patNum1) && IsPatientAClone(patNum2)) {
				long patNum1Master=GetOriginalPatNumFromClone(patNum1);
				long patNum2Master=GetOriginalPatNumFromClone(patNum2);
				return (patNum1Master==patNum2Master);
			}
			return false;//The two patients are not clones and / or are not clones of eachother.
		}

		///<summary>Returns true if this patient has been merged into another patient, and no other patient has merged into this patient after the
		///original merge.</summary>
		public static bool WasPatientMerged(long patNum,List<PatientLink> listMergeLinks=null) {
			//No need to check RemotingRole; no call to db.
			listMergeLinks=listMergeLinks??GetLinks(patNum,PatientLinkType.Merge);
			PatientLink mergeLink=listMergeLinks.OrderBy(x => x.DateTimeLink).LastOrDefault(x => x.PatNumFrom==patNum);
			if(mergeLink==null) {
				return false;//This patient has never been merged into another patient.
			}
			if(listMergeLinks.Any(x => x.PatNumTo==patNum && x.DateTimeLink > mergeLink.DateTimeLink)) {
				return false;//After our patient was merged into another patient, a different patient was merged into our patient. Our patient is no longer
										 //considered a "merged" patient.
			}
			return true;//This patient has been merged into another patient.
		}

		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<PatientLink> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatientLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM patientlink WHERE PatNum = "+POut.Long(patNum);
			return Crud.PatientLinkCrud.SelectMany(command);
		}

		///<summary>Gets one PatientLink from the db.</summary>
		public static PatientLink GetOne(long patientLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<PatientLink>(MethodBase.GetCurrentMethod(),patientLinkNum);
			}
			return Crud.PatientLinkCrud.SelectOne(patientLinkNum);
		}

		///<summary></summary>
		public static void Update(PatientLink patientLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patientLink);
				return;
			}
			Crud.PatientLinkCrud.Update(patientLink);
		}

		///<summary></summary>
		public static void Delete(long patientLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patientLinkNum);
				return;
			}
			Crud.PatientLinkCrud.Delete(patientLinkNum);
		}

		

		
		*/
	}
}