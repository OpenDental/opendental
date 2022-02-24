using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness{

	///<summary></summary>
	public class Medications{
		#region Cache Pattern

		private class MedicationCache : CacheDictAbs<Medication,long,Medication> {
			protected override List<Medication> GetCacheFromDb() {
				string command="SELECT * FROM medication ORDER BY MedName";
				return Crud.MedicationCrud.SelectMany(command);
			}
			protected override List<Medication> TableToList(DataTable table) {
				return Crud.MedicationCrud.TableToList(table);
			}
			protected override Medication Copy(Medication medication) {
				return medication.Copy();
			}
			protected override DataTable DictToTable(Dictionary<long,Medication> dictMedications) {
				return Crud.MedicationCrud.ListToTable(dictMedications.Values.ToList(),"Medication");
			}
			protected override void FillCacheIfNeeded() {
				Medications.GetTableFromCache(false);
			}
			protected override long GetDictKey(Medication medication) {
				return medication.MedicationNum;
			}
			protected override Medication GetDictValue(Medication medication) {
				return medication;
			}
			protected override Medication CopyDictValue(Medication medication) {
				return medication.Copy();
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static MedicationCache _medicationCache=new MedicationCache();

		public static Medication GetOne(long medicationNum) {
			return _medicationCache.GetOne(medicationNum);
		}

		public static List<Medication> GetWhere(Func<Medication,bool> match,bool isShort=false) {
			return _medicationCache.GetWhere(match,isShort);
		}

		public static bool GetContainsKey(long medicationNum) {
			return _medicationCache.GetContainsKey(medicationNum);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_medicationCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_medicationCache.FillCacheFromTable(table);
				return table;
			}
			return _medicationCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary>Checks to see if the medication exists in the current cache.  If not, the local cache will get refreshed and then searched again.  If med is still not found, false is returned because the med does not exist.</summary>
		private static bool HasMedicationInCache(long medicationNum) {
			//Check if the medication exists in the cache.
			return GetContainsKey(medicationNum);
		}

		///<summary>Only public so that the remoting works.  Do not call this from anywhere except in this class.</summary>
		public static List<Medication> GetListFromDb() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Medication>>(MethodBase.GetCurrentMethod());
			}
			string command ="SELECT * FROM medication ORDER BY MedName";// WHERE MedName LIKE '%"+POut.String(str)+"%' ORDER BY MedName";
			return Crud.MedicationCrud.SelectMany(command);
		}

		public static List<Medication> TableToList(DataTable table) {
			//No need to check RemotingRole; no call to db.
			return Crud.MedicationCrud.TableToList(table);
		}

		///<summary>Returns medications that contain the passed in string.  Blank for all.</summary>
		public static List<Medication> GetList(string str="") {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => str=="" || x.MedName.ToUpper().Contains(str.ToUpper()));
		}

		///<summary></summary>
		public static void Update(Medication Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.MedicationCrud.Update(Cur);
		}

		///<summary></summary>
		public static long Insert(Medication Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.MedicationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.MedicationNum;
			}
			return Crud.MedicationCrud.Insert(Cur);
		}

		///<summary>Dependent brands and patients will already be checked.  Be sure to surround with try-catch.</summary>
		public static void Delete(Medication Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string s=IsInUse(Cur);
			if(s!="") {
				throw new ApplicationException(Lans.g("Medications",s));
			}
			string command = "DELETE from medication WHERE medicationNum = '"+Cur.MedicationNum.ToString()+"'";
			Db.NonQ(command);
		}

		///<summary>Refreshes cache and then checks if any medication's generic num is a foreign key to med.MedicationNum aside from itself</summary>
		public static bool IsInUseAsGeneric(Medication med) {
			RefreshCache();
			//Any other medications using the given medication as a generic
			return _medicationCache.GetWhere(x=>x.MedicationNum!=med.MedicationNum && x.GenericNum==med.MedicationNum).FirstOrDefault()!=null;
		}

		///<summary>Returns a string if medication is in use in medicationpat, allergydef, eduresources, or preference.MedicationsIndicateNone. The string will explain where the medication is in use.</summary>
		public static string IsInUse(Medication med) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),med);
			}
			string[] brands;
			if(med.MedicationNum==med.GenericNum) {
				brands=GetBrands(med.MedicationNum);
			}
			else {
				brands=new string[0];
			}
			if(brands.Length>0) {
				return "You can not delete a medication that has brand names attached.";
			}
			string command="SELECT COUNT(*) FROM medicationpat WHERE MedicationNum="+POut.Long(med.MedicationNum);
			if(PIn.Int(Db.GetCount(command))!=0) {
				return "Not allowed to delete medication because it is in use by a patient";
			}
			command="SELECT COUNT(*) FROM allergydef WHERE MedicationNum="+POut.Long(med.MedicationNum);
			if(PIn.Int(Db.GetCount(command))!=0) {
				return "Not allowed to delete medication because it is in use by an allergy";
			}
			command="SELECT COUNT(*) FROM eduresource WHERE MedicationNum="+POut.Long(med.MedicationNum);
			if(PIn.Int(Db.GetCount(command))!=0) {
				return "Not allowed to delete medication because it is in use by an education resource";
			}
			command="SELECT COUNT(*) FROM rxalert WHERE MedicationNum="+POut.Long(med.MedicationNum);
			if(PIn.Int(Db.GetCount(command))!=0) {
				return "Not allowed to delete medication because it is in use by an Rx alert";
			}
			//If any more tables are added here in the future, then also update GetAllInUseMedicationNums() to include the new table.
			if(PrefC.GetLong(PrefName.MedicationsIndicateNone)==med.MedicationNum) {
				return "Not allowed to delete medication because it is in use by a medication";
			}
			return "";
		}

		public static List<long> GetAllInUseMedicationNums() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			//If any more tables are added here in the future, then also update IsInUse() to include the new table.
			string command="SELECT MedicationNum FROM medicationpat WHERE MedicationNum!=0 "
				+"UNION SELECT MedicationNum FROM allergydef WHERE MedicationNum!=0 "
				+"UNION SELECT MedicationNum FROM eduresource WHERE MedicationNum!=0 "
				+"GROUP BY MedicationNum";
			List <long> listMedicationNums=Db.GetListLong(command);
			if(PrefC.GetLong(PrefName.MedicationsIndicateNone)!=0) {
				listMedicationNums.Add(PrefC.GetLong(PrefName.MedicationsIndicateNone));
			}
			return listMedicationNums;
		}

		///<summary>Returns an array of all patient names who are using this medication.</summary>
		public static string[] GetPatNamesForMed(long medicationNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<string[]>(MethodBase.GetCurrentMethod(),medicationNum);
			}
			string command =
				"SELECT CONCAT(CONCAT(CONCAT(CONCAT(LName,', '),FName),' '),Preferred) FROM medicationpat,patient "
				+"WHERE medicationpat.PatNum=patient.PatNum "
				+"AND medicationpat.MedicationNum="+POut.Long(medicationNum);
			DataTable table=Db.GetTable(command);
			string[] retVal=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++){
				retVal[i]=PIn.String(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Returns a list of all brands dependend on this generic. Only gets run if this is a generic.</summary>
		public static string[] GetBrands(long medicationNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<string[]>(MethodBase.GetCurrentMethod(),medicationNum);
			}
			string command =
				"SELECT MedName FROM medication "
				+"WHERE GenericNum="+medicationNum.ToString()
				+" AND MedicationNum !="+medicationNum.ToString();//except this med
			DataTable table=Db.GetTable(command);
			string[] retVal=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++){
				retVal[i]=PIn.String(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Returns null if not found.</summary>
		public static Medication GetMedication(long medNum) {
			//No need to check RemotingRole; no call to db.
			if(!HasMedicationInCache(medNum)) {
				return null;//Should never happen.
			}
			return GetOne(medNum);
		}

		///<summary>Deprecated.  Use GetMedication instead.</summary>
		public static Medication GetMedicationFromDb(long medicationNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Medication>(MethodBase.GetCurrentMethod(),medicationNum);
			}
			string command="SELECT * FROM medication WHERE MedicationNum="+POut.Long(medicationNum);
			return Crud.MedicationCrud.SelectOne(command);
		}

		///<summary>//Returns first medication with matching MedName, if not found returns null.</summary>
		public static Medication GetMedicationFromDbByName(string medicationName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Medication>(MethodBase.GetCurrentMethod(),medicationName);
			}
			string command="SELECT * FROM medication WHERE MedName='"+POut.String(medicationName)+"' ORDER BY MedicationNum";
			List<Medication> retVal=Crud.MedicationCrud.SelectMany(command);
			if(retVal.Count>0) {
				return retVal[0];
			}
			return null;
		}

		///<summary>Gets the generic medication for the specified medication Num. Returns null if not found.</summary>
		public static Medication GetGeneric(long medNum) {
			//No need to check RemotingRole; no call to db.
			if(!HasMedicationInCache(medNum)) {
				return null;
			}
			return GetOne((GetOne(medNum)).GenericNum);
		}

		///<summary>Gets the medication name.  Also, generic in () if applicable.  Returns empty string if not found.</summary>
		public static string GetDescription(long medNum) {
			//No need to check RemotingRole; no call to db.
			if(!HasMedicationInCache(medNum)) {
				return "";
			}
			Medication med=GetOne(medNum);
			string retVal=med.MedName;
			if(med.GenericNum==med.MedicationNum){//this is generic
				return retVal;
			}
			if(!GetContainsKey(med.GenericNum)) {
				return retVal;
			}
			Medication generic=GetOne(med.GenericNum);
			return retVal+"("+generic.MedName+")";
		}

		///<summary>Gets the medication name. Copied from GetDescription.</summary>
		public static string GetNameOnly(long medNum) {
			//No need to check RemotingRole; no call to db.
			if(!HasMedicationInCache(medNum)) {
				return "";
			}
			return GetOne(medNum).MedName;
		}

		///<summary>Gets the generic medication name, given it's generic Num.</summary>
		public static string GetGenericName(long genericNum) {
			//No need to check RemotingRole; no call to db.
			if(!HasMedicationInCache(genericNum)) {
				return "";
			}
			return GetOne(genericNum).MedName;
		}

		///<summary>Gets the generic medication name, given it's generic Num.  Will search through the passed in list before resorting to cache.</summary>
		public static string GetGenericName(long genericNum,Hashtable hlist) {
			//No need to check RemotingRole; no call to db.
			if(!hlist.ContainsKey(genericNum)) {
				//Medication not found.  Refresh the cache and check again.
				return GetGenericName(genericNum);
			}
			return ((Medication)hlist[genericNum]).MedName;
		}

		public static List<long> GetChangedSinceMedicationNums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT MedicationNum FROM medication WHERE DateTStamp > "+POut.DateT(changedSince);
			DataTable dt=Db.GetTable(command);
			List<long> medicationNums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				medicationNums.Add(PIn.Long(dt.Rows[i]["MedicationNum"].ToString()));
			}
			return medicationNums;
		}

		///<summary>Used along with GetChangedSinceMedicationNums</summary>
		public static List<Medication> GetMultMedications(List<long> medicationNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Medication>>(MethodBase.GetCurrentMethod(),medicationNums);
			}
			string strMedicationNums="";
			DataTable table;
			if(medicationNums.Count>0) {
				for(int i=0;i<medicationNums.Count;i++) {
					if(i>0) {
						strMedicationNums+="OR ";
					}
					strMedicationNums+="MedicationNum='"+medicationNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM medication WHERE "+strMedicationNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			Medication[] multMedications=Crud.MedicationCrud.TableToList(table).ToArray();
			List<Medication> MedicationList=new List<Medication>(multMedications);
			return MedicationList;
		}
		
		///<summary>Deprecated.  Use MedicationPat.Refresh() instead.  Returns medication list for a specific patient.</summary>
		public static List<Medication> GetMedicationsByPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Medication>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT medication.* "
				+"FROM medication, medicationpat "
				+"WHERE medication.MedicationNum=medicationpat.MedicationNum "
				+"AND medicationpat.PatNum="+POut.Long(patNum);
			return Crud.MedicationCrud.SelectMany(command);
		}

		public static Medication GetMedicationFromDbByRxCui(long rxcui) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Medication>(MethodBase.GetCurrentMethod(),rxcui);
			}
			//an RxCui could be linked to multiple medications, the ORDER BY ensures we get the same medication every time we call this function
			string command="SELECT * FROM medication WHERE RxCui="+POut.Long(rxcui)+" ORDER BY MedicationNum";
			return Crud.MedicationCrud.SelectOne(command);
		}

		public static List<Medication> GetAllMedsByRxCui(long rxCui) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.RxCui==rxCui).OrderBy(x => x.MedicationNum).ToList();
		}

		public static bool AreMedicationsEqual(Medication medication,Medication medicationOld) {
			//No need to check RemotingRole; no call to db.
			if((medicationOld==null || medication==null)
				|| medicationOld.MedicationNum!=medication.MedicationNum
				|| medicationOld.MedName!=medication.MedName
				|| medicationOld.GenericNum!=medication.GenericNum
				|| medicationOld.Notes!=medication.Notes
				|| medicationOld.RxCui!=medication.RxCui) 
			{
				return false;
			}
			return true;
		}

		///<summary>Returns the number of patients associated with the passed-in medicationNum.</summary>
		public static long CountPats(long medNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),medNum);
			}
			string command="SELECT COUNT(DISTINCT medicationpat.PatNum) FROM medicationpat WHERE MedicationNum="+POut.Long(medNum);
			return PIn.Long(Db.GetScalar(command));
		}

		///<summary>Medication merge tool.  Returns the number of rows changed.  Deletes the medication associated with medNumInto.</summary>
		public static long Merge(long medNumFrom,long medNumInto) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),medNumFrom,medNumInto);
			}
			string[] medNumForeignKeys=new string[] { //add any new FKs to this list.
				"allergydef.MedicationNum",
				"eduresource.MedicationNum",
				"medication.GenericNum",
				"medicationpat.MedicationNum",
				"rxalert.MedicationNum"
			};
			string command="";
			long rowsChanged=0;
			for(int i=0;i<medNumForeignKeys.Length;i++) { //actually change all of the FKs in the above tables.
				string[] tableAndKeyName=medNumForeignKeys[i].Split(new char[] { '.' });
				command="UPDATE "+tableAndKeyName[0]
					+" SET "+tableAndKeyName[1]+"="+POut.Long(medNumInto)
					+" WHERE "+tableAndKeyName[1]+"="+POut.Long(medNumFrom);
				rowsChanged+=Db.NonQ(command);
			}
			command="SELECT medication.RxCui FROM medication WHERE MedicationNum="+medNumInto; //update medicationpat's RxNorms to match medication.
			string rxNorm=Db.GetScalar(command);
			command="UPDATE medicationpat SET RxCui="+rxNorm+" WHERE MedicationNum="+medNumInto;
			Db.NonQ(command);
			command="SELECT * FROM ehrtrigger WHERE MedicationNumList LIKE '% "+POut.Long(medNumFrom)+" %'";
			List<EhrTrigger> ListEhrTrigger=Crud.EhrTriggerCrud.SelectMany(command); //get all ehr triggers with matching mednum in mednumlist
			for(int i=0;i<ListEhrTrigger.Count;i++) {//for each trigger...
				string[] arrayMedNums=ListEhrTrigger[i].MedicationNumList.Split(new char[] { ' ' }); //get an array of their medicationNums.
				bool containsMedNumInto=arrayMedNums.Any(x => x==POut.Long(medNumInto));
				string strMedNumList="";
				foreach(string medNumStr in arrayMedNums) { //for each mednum in the MedicationList for the current trigger.
					string medNumCur=medNumStr.Trim();
					if(medNumCur=="") {	//because we use spaces as a buffer before and after mednums, this prevents empty spaces from being considered.
						continue;
					}
					if(containsMedNumInto) { //if the list already contains medNumInto, 
						if(medNumCur==POut.Long(medNumFrom)) {
							continue;	//skip medNumFrom (remove it from the list)
						}
						else {
							strMedNumList+=" "+medNumCur+" ";
						}
					}
					else { //if the list doesn't contain medNumInto
						if(medNumCur==POut.Long(medNumFrom)) {
							strMedNumList+=" "+medNumInto+" "; //replace medNumFrom with medNumInto
						}
						else {
							strMedNumList+=" "+medNumCur+" ";
						}
					}
				}//end for each mednum
				ListEhrTrigger[i].MedicationNumList=strMedNumList;
				EhrTriggers.Update(ListEhrTrigger[i]); //update the ehrtrigger list.
			}//end for each trigger
			Crud.MedicationCrud.Delete(medNumFrom); //finally, delete the mednum.
			return rowsChanged;
		}
	}

	




}










