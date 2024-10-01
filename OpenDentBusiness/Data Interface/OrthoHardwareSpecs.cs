using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoHardwareSpecs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class OrthoHardwareSpecCache : CacheListAbs<OrthoHardwareSpec> {
			protected override List<OrthoHardwareSpec> GetCacheFromDb() {
				string command="SELECT * FROM orthohardwarespec ORDER BY OrthoHardwareType,ItemOrder";
				return Crud.OrthoHardwareSpecCrud.SelectMany(command);
			}
			protected override List<OrthoHardwareSpec> TableToList(DataTable table) {
				return Crud.OrthoHardwareSpecCrud.TableToList(table);
			}
			protected override OrthoHardwareSpec Copy(OrthoHardwareSpec orthoHardwareSpec) {
				return orthoHardwareSpec.Copy();
			}
			protected override DataTable ListToTable(List<OrthoHardwareSpec> listOrthoHardwareSpecs) {
				return Crud.OrthoHardwareSpecCrud.ListToTable(listOrthoHardwareSpecs,"OrthoHardwareSpec");
			}
			protected override void FillCacheIfNeeded() {
				OrthoHardwareSpecs.GetTableFromCache(false);
			}

			protected override bool IsInListShort(OrthoHardwareSpec orthoHardwareSpec) {
				if(orthoHardwareSpec.IsHidden){
					return false;
				}
				return true;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static OrthoHardwareSpecCache _orthoHardwareSpecCache=new OrthoHardwareSpecCache();

		public static List<OrthoHardwareSpec> GetDeepCopy(bool isShort=false) {
			return _orthoHardwareSpecCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _orthoHardwareSpecCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<OrthoHardwareSpec> match,bool isShort=false) {
			return _orthoHardwareSpecCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<OrthoHardwareSpec> match,bool isShort=false) {
			return _orthoHardwareSpecCache.GetFindIndex(match,isShort);
		}

		public static OrthoHardwareSpec GetFirst(bool isShort=false) {
			return _orthoHardwareSpecCache.GetFirst(isShort);
		}

		public static OrthoHardwareSpec GetFirst(Func<OrthoHardwareSpec,bool> match,bool isShort=false) {
			return _orthoHardwareSpecCache.GetFirst(match,isShort);
		}

		public static OrthoHardwareSpec GetFirstOrDefault(Func<OrthoHardwareSpec,bool> match,bool isShort=false) {
			return _orthoHardwareSpecCache.GetFirstOrDefault(match,isShort);
		}

		public static OrthoHardwareSpec GetLast(bool isShort=false) {
			return _orthoHardwareSpecCache.GetLast(isShort);
		}

		public static OrthoHardwareSpec GetLastOrDefault(Func<OrthoHardwareSpec,bool> match,bool isShort=false) {
			return _orthoHardwareSpecCache.GetLastOrDefault(match,isShort);
		}

		public static List<OrthoHardwareSpec> GetWhere(Predicate<OrthoHardwareSpec> match,bool isShort=false) {
			return _orthoHardwareSpecCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_orthoHardwareSpecCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_orthoHardwareSpecCache.FillCacheFromTable(table);
				return table;
			}
			return _orthoHardwareSpecCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_orthoHardwareSpecCache.ClearCache();
		}
		#endregion Cache Pattern
		
		/*
		#region Methods - Get
		///<summary></summary>
		public static List<OrthoHardwareSpec> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoHardwareSpec>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthohardwarespec WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrthoHardwareSpecCrud.SelectMany(command);
		}
		
		///<summary>Gets one OrthoHardwareSpec from the db.</summary>
		public static OrthoHardwareSpec GetOne(long orthoHardwareSpecNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<OrthoHardwareSpec>(MethodBase.GetCurrentMethod(),orthoHardwareSpecNum);
			}
			return Crud.OrthoHardwareSpecCrud.SelectOne(orthoHardwareSpecNum);
		}
		#endregion Methods - Get*/

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(OrthoHardwareSpec orthoHardwareSpec){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				orthoHardwareSpec.OrthoHardwareSpecNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoHardwareSpec);
				return orthoHardwareSpec.OrthoHardwareSpecNum;
			}
			return Crud.OrthoHardwareSpecCrud.Insert(orthoHardwareSpec);
		}

		///<summary></summary>
		public static void Update(OrthoHardwareSpec orthoHardwareSpec){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoHardwareSpec);
				return;
			}
			Crud.OrthoHardwareSpecCrud.Update(orthoHardwareSpec);
		}

		///<summary>Throws exception if in use by a patient or ortho prescription.</summary>
		public static void Delete(long orthoHardwareSpecNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoHardwareSpecNum);
				return;
			}
			string command="SELECT COUNT(*) FROM orthohardware WHERE OrthoHardwareSpecNum="+POut.Long(orthoHardwareSpecNum);
			string count=Db.GetCount(command);
			if(count!="0"){
				throw new Exception("Already in use by patients. Hide instead of Deleting.");
			}
			command="SELECT COUNT(*) FROM orthorx WHERE OrthoHardwareSpecNum="+POut.Long(orthoHardwareSpecNum);
			count=Db.GetCount(command);
			if(count!="0"){
				throw new Exception("Already in use by Ortho Prescription. Hide instead of Deleting or unlink this Hardware Spec from Ortho Prescriptions.");
			}
			Crud.OrthoHardwareSpecCrud.Delete(orthoHardwareSpecNum);
		}
		#endregion Methods - Modify
		



	}
}