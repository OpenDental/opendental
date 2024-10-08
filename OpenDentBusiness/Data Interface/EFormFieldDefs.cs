using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EFormFieldDefs{
		#region Cache Pattern
		//Uses InvalidType.Sheets
		private class EFormFieldDefCache : CacheListAbs<EFormFieldDef> {
			protected override List<EFormFieldDef> GetCacheFromDb() {
				string command="SELECT * FROM eformfielddef ORDER BY ItemOrder";
				return Crud.EFormFieldDefCrud.SelectMany(command);
			}
			protected override List<EFormFieldDef> TableToList(DataTable table) {
				return Crud.EFormFieldDefCrud.TableToList(table);
			}
			protected override EFormFieldDef Copy(EFormFieldDef eFormFieldDef) {
				return eFormFieldDef.Copy();
			}
			protected override DataTable ListToTable(List<EFormFieldDef> listEFormFieldDefs) {
				return Crud.EFormFieldDefCrud.ListToTable(listEFormFieldDefs,"EFormFieldDef");
			}
			protected override void FillCacheIfNeeded() {
				EFormFieldDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EFormFieldDefCache _eFormFieldDefCache=new EFormFieldDefCache();

		public static void ClearCache() {
			_eFormFieldDefCache.ClearCache();
		}

		public static List<EFormFieldDef> GetDeepCopy(bool isShort=false) {
			return _eFormFieldDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _eFormFieldDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<EFormFieldDef> match,bool isShort=false) {
			return _eFormFieldDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<EFormFieldDef> match,bool isShort=false) {
			return _eFormFieldDefCache.GetFindIndex(match,isShort);
		}

		public static EFormFieldDef GetFirst(bool isShort=false) {
			return _eFormFieldDefCache.GetFirst(isShort);
		}

		public static EFormFieldDef GetFirst(Func<EFormFieldDef,bool> match,bool isShort=false) {
			return _eFormFieldDefCache.GetFirst(match,isShort);
		}

		public static EFormFieldDef GetFirstOrDefault(Func<EFormFieldDef,bool> match,bool isShort=false) {
			return _eFormFieldDefCache.GetFirstOrDefault(match,isShort);
		}

		public static EFormFieldDef GetLast(bool isShort=false) {
			return _eFormFieldDefCache.GetLast(isShort);
		}

		public static EFormFieldDef GetLastOrDefault(Func<EFormFieldDef,bool> match,bool isShort=false) {
			return _eFormFieldDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<EFormFieldDef> GetWhere(Predicate<EFormFieldDef> match,bool isShort=false) {
			return _eFormFieldDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eFormFieldDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eFormFieldDefCache.FillCacheFromTable(table);
				return table;
			}
			return _eFormFieldDefCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(EFormFieldDef eFormFieldDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eFormFieldDef.EFormFieldDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eFormFieldDef);
				return eFormFieldDef.EFormFieldDefNum;
			}
			return Crud.EFormFieldDefCrud.Insert(eFormFieldDef);
		}

		///<summary></summary>
		public static void Update(EFormFieldDef eFormFieldDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormFieldDef);
				return;
			}
			Crud.EFormFieldDefCrud.Update(eFormFieldDef);
		}

		///<summary>This also deletes all LanguagePats which are linked to it.</summary>
		public static void Delete(long eFormFieldDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormFieldDefNum);
				return;
			}
			LanguagePats.DeleteForEFormFieldDef(eFormFieldDefNum);
			Crud.EFormFieldDefCrud.Delete(eFormFieldDefNum);
		}

		//We can't do this. We must instead loop through them because we need to delete any LanguagePats
		//<summary></summary>
		//public static void DeleteForForm(long eFormDefNum) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormDefNum);
		//		return;
		//	}
		//	string command="DELETE FROM eformfielddef WHERE EFormDefNum = "+POut.Long(eFormDefNum);
		//	Db.NonQ(command);
		//}

		//public static List<EFormFieldDef> FromList(List<EFormField> listEFormFields){
		//	List<EFormFieldDef> listEFormFieldDefs=new List<EFormFieldDef>();
		//	for(int i=0;i<listEFormFields.Count;i++) {
		//		EFormFieldDef eFormFieldDef=EFormFields.ToDef(listEFormFields[i]);
		//		listEFormFieldDefs.Add(eFormFieldDef);
		//	}
		//	return listEFormFieldDefs;
		//}

		///<summary>True for CheckBox, DateField, Label, RadioButtons, TextField</summary>
		public static bool IsHorizStackableType(EnumEFormFieldType enumEFormFieldType){
			if(enumEFormFieldType.In(
				EnumEFormFieldType.CheckBox, 
				EnumEFormFieldType.DateField, 
				EnumEFormFieldType.Label, 
				EnumEFormFieldType.TextField,
				EnumEFormFieldType.RadioButtons))
			{
				return true;//those types are allowed to stack
			}
			if(enumEFormFieldType.In(
				EnumEFormFieldType.SigBox, 
				EnumEFormFieldType.PageBreak, 
				EnumEFormFieldType.MedicationList))
			{
				return false;//those types are not allowed to stack
			}
			throw new Exception("Missing a type.");
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		
		///<summary></summary>
		public static List<EFormFieldDef> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EFormFieldDef>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eformfielddef WHERE PatNum = "+POut.Long(patNum);
			return Crud.EFormFieldDefCrud.SelectMany(command);
		}
		
		///<summary>Gets one EFormFieldDef from the db.</summary>
		public static EFormFieldDef GetOne(long eFormFieldDefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EFormFieldDef>(MethodBase.GetCurrentMethod(),eFormFieldDefNum);
			}
			return Crud.EFormFieldDefCrud.SelectOne(eFormFieldDefNum);
		}
		
		
		*/



	}
}