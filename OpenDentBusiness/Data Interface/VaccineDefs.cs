using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class VaccineDefs{
		#region CachePattern

		private class VaccineDefCache : CacheListAbs<VaccineDef> {
			protected override List<VaccineDef> GetCacheFromDb() {
				string command="SELECT * FROM vaccinedef ORDER BY CVXCode";
				return Crud.VaccineDefCrud.SelectMany(command);
			}
			protected override List<VaccineDef> TableToList(DataTable table) {
				return Crud.VaccineDefCrud.TableToList(table);
			}
			protected override VaccineDef Copy(VaccineDef vaccineDef) {
				return vaccineDef.Copy();
			}
			protected override DataTable ListToTable(List<VaccineDef> listVaccineDefs) {
				return Crud.VaccineDefCrud.ListToTable(listVaccineDefs,"VaccineDef");
			}
			protected override void FillCacheIfNeeded() {
				VaccineDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static VaccineDefCache _vaccineDefCache=new VaccineDefCache();

		public static bool GetExists(Predicate<VaccineDef> match,bool isShort=false) {
			return _vaccineDefCache.GetExists(match,isShort);
		}

		public static VaccineDef GetFirstOrDefault(Func<VaccineDef,bool> match,bool isShort=false) {
			return _vaccineDefCache.GetFirstOrDefault(match,isShort);
		}

		public static List<VaccineDef> GetDeepCopy(bool isShort=false) {
			return _vaccineDefCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_vaccineDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_vaccineDefCache.FillCacheFromTable(table);
				return table;
			}
			return _vaccineDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Gets one VaccineDef from the db.</summary>
		public static VaccineDef GetOne(long vaccineDefNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<VaccineDef>(MethodBase.GetCurrentMethod(),vaccineDefNum);
			}
			return Crud.VaccineDefCrud.SelectOne(vaccineDefNum);
		}

		///<summary></summary>
		public static long Insert(VaccineDef vaccineDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				vaccineDef.VaccineDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),vaccineDef);
				return vaccineDef.VaccineDefNum;
			}
			return Crud.VaccineDefCrud.Insert(vaccineDef);
		}

		///<summary></summary>
		public static void Update(VaccineDef vaccineDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccineDef);
				return;
			}
			Crud.VaccineDefCrud.Update(vaccineDef);
		}

		///<summary></summary>
		public static void Delete(long vaccineDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vaccineDefNum);
				return;
			}
			//validation
			string command;
			command="SELECT COUNT(*) FROM VaccinePat WHERE VaccineDefNum="+POut.Long(vaccineDefNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("FormDrugUnitEdit","Cannot delete: VaccineDef is in use by VaccinePat."));
			}
			command= "DELETE FROM vaccinedef WHERE VaccineDefNum = "+POut.Long(vaccineDefNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<VaccineDef> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VaccineDef>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM vaccinedef WHERE PatNum = "+POut.Long(patNum);
			return Crud.VaccineDefCrud.SelectMany(command);
		}
		*/
	}
}