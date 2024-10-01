using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class MountDefs {
		#region CachePattern

		private class MountDefCache : CacheListAbs<MountDef> {
			protected override List<MountDef> GetCacheFromDb() {
				string command="SELECT * FROM mountdef ORDER BY ItemOrder";
				return Crud.MountDefCrud.SelectMany(command);
			}
			protected override List<MountDef> TableToList(DataTable table) {
				return Crud.MountDefCrud.TableToList(table);
			}
			protected override MountDef Copy(MountDef mountDef) {
				return mountDef.Copy();
			}
			protected override DataTable ListToTable(List<MountDef> listMountDefs) {
				return Crud.MountDefCrud.ListToTable(listMountDefs,"MountDef");
			}
			protected override void FillCacheIfNeeded() {
				MountDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static MountDefCache _mountDefCache=new MountDefCache();

		public static List<MountDef> GetDeepCopy(bool isShort=false) {
			return _mountDefCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			Meth.NoCheckMiddleTierRole();
			_mountDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_mountDefCache.FillCacheFromTable(table);
				return table;
			}
			return _mountDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_mountDefCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static void Update(MountDef mountDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountDef);
				return;
			}
			Crud.MountDefCrud.Update(mountDef);
		}

		///<summary></summary>
		public static long Insert(MountDef mountDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				mountDef.MountDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mountDef);
				return mountDef.MountDefNum;
			}
			return Crud.MountDefCrud.Insert(mountDef);
		}

		///<summary>No need to surround with try/catch, because all deletions are allowed.</summary>
		public static void Delete(long mountDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountDefNum);
				return;
			}
			string command="DELETE FROM mountdef WHERE MountDefNum="+POut.Long(mountDefNum);
			Db.NonQ(command);
			command="DELETE FROM mountitemdef WHERE MountDefNum ="+POut.Long(mountDefNum);
			Db.NonQ(command);
		}

		public static string SetScale(float scale,int decimals,string units){
			Meth.NoCheckMiddleTierRole();
			string retVal=scale.ToString()+" "+decimals.ToString();
			if(units!=null && units!=""){
				retVal+=" "+units;
			}
			return retVal;
		}

		public static float GetScale(string scaleValue){
			Meth.NoCheckMiddleTierRole();
			if(scaleValue is null){
				return 0;
			}
			List<string> listStrings=scaleValue.Split(' ').ToList();
			if(listStrings.Count>0){
				return PIn.Float(listStrings[0]);
			}
			return 0;
		}

		public static int GetDecimals(string scaleValue){
			Meth.NoCheckMiddleTierRole();
			if(scaleValue is null){
				return 0;
			}
			List<string> listStrings=scaleValue.Split(' ').ToList();
			if(listStrings.Count>1){
				return PIn.Int(listStrings[1]);
			}
			return 0;
		}

		public static string GetScaleUnits(string scaleValue){
			Meth.NoCheckMiddleTierRole();
			if(scaleValue is null){
				return "";
			}
			List<string> listStrings=scaleValue.Split(' ').ToList();
			if(listStrings.Count==3){
				return listStrings[2];
			}
			return "";
		}


	}

		



		
	

	

	


}










