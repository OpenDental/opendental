using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DrugManufacturers{
		#region CachePattern

		private class DrugManufacturerCache : CacheListAbs<DrugManufacturer> {
			protected override List<DrugManufacturer> GetCacheFromDb() {
				string command="SELECT * FROM drugmanufacturer ORDER BY ManufacturerCode";
				return Crud.DrugManufacturerCrud.SelectMany(command);
			}
			protected override List<DrugManufacturer> TableToList(DataTable table) {
				return Crud.DrugManufacturerCrud.TableToList(table);
			}
			protected override DrugManufacturer Copy(DrugManufacturer drugManufacturer) {
				return drugManufacturer.Copy();
			}
			protected override DataTable ListToTable(List<DrugManufacturer> listDrugManufacturers) {
				return Crud.DrugManufacturerCrud.ListToTable(listDrugManufacturers,"DrugManufacturer");
			}
			protected override void FillCacheIfNeeded() {
				DrugManufacturers.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DrugManufacturerCache _drugManufacturerCache=new DrugManufacturerCache();

		public static bool GetExists(Predicate<DrugManufacturer> match,bool isShort=false) {
			return _drugManufacturerCache.GetExists(match,isShort);
		}

		public static List<DrugManufacturer> GetDeepCopy(bool isShort=false) {
			return _drugManufacturerCache.GetDeepCopy(isShort);
		}

		public static DrugManufacturer GetFirstOrDefault(Func<DrugManufacturer,bool> match,bool isShort=false) {
			return _drugManufacturerCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_drugManufacturerCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_drugManufacturerCache.FillCacheFromTable(table);
				return table;
			}
			return _drugManufacturerCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Gets one DrugManufacturer from the db.</summary>
		public static DrugManufacturer GetOne(long drugManufacturerNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<DrugManufacturer>(MethodBase.GetCurrentMethod(),drugManufacturerNum);
			}
			return Crud.DrugManufacturerCrud.SelectOne(drugManufacturerNum);
		}

		///<summary></summary>
		public static long Insert(DrugManufacturer drugManufacturer){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				drugManufacturer.DrugManufacturerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),drugManufacturer);
				return drugManufacturer.DrugManufacturerNum;
			}
			return Crud.DrugManufacturerCrud.Insert(drugManufacturer);
		}

		///<summary></summary>
		public static void Update(DrugManufacturer drugManufacturer){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),drugManufacturer);
				return;
			}
			Crud.DrugManufacturerCrud.Update(drugManufacturer);
		}

		///<summary></summary>
		public static void Delete(long drugManufacturerNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),drugManufacturerNum);
				return;
			}
			//validation
			string command;
			command="SELECT COUNT(*) FROM VaccineDef WHERE drugManufacturerNum="+POut.Long(drugManufacturerNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("FormDrugUnitEdit","Cannot delete: DrugManufacturer is in use by VaccineDef."));
			}
			command= "DELETE FROM drugmanufacturer WHERE DrugManufacturerNum = "+POut.Long(drugManufacturerNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<DrugManufacturer> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DrugManufacturer>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM drugmanufacturer WHERE PatNum = "+POut.Long(patNum);
			return Crud.DrugManufacturerCrud.SelectMany(command);
		}
		*/
	}
}