using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class HL7DefFields{
		#region CachePattern

		private class HL7DefFieldCache : CacheListAbs<HL7DefField> {
			protected override List<HL7DefField> GetCacheFromDb() {
				string command="SELECT * FROM hl7deffield ORDER BY OrdinalPos";
				return Crud.HL7DefFieldCrud.SelectMany(command);
			}
			protected override List<HL7DefField> TableToList(DataTable table) {
				return Crud.HL7DefFieldCrud.TableToList(table);
			}
			protected override HL7DefField Copy(HL7DefField HL7DefField) {
				return HL7DefField.Clone();
			}
			protected override DataTable ListToTable(List<HL7DefField> listHL7DefFields) {
				return Crud.HL7DefFieldCrud.ListToTable(listHL7DefFields,"HL7DefField");
			}
			protected override void FillCacheIfNeeded() {
				HL7DefFields.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static HL7DefFieldCache _HL7DefFieldCache=new HL7DefFieldCache();

		public static List<HL7DefField> GetWhere(Predicate<HL7DefField> match,bool isShort=false) {
			return _HL7DefFieldCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_HL7DefFieldCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_HL7DefFieldCache.FillCacheFromTable(table);
				return table;
			}
			return _HL7DefFieldCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		/// <summary>Gets it straight from the database instead of from cache.</summary>
		public static List<HL7DefField> GetFromDb(long hl7DefSegmentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7DefField>>(MethodBase.GetCurrentMethod(),hl7DefSegmentNum);
			}
			string command="SELECT * FROM hl7deffield WHERE HL7DefSegmentNum='"+POut.Long(hl7DefSegmentNum)+"' ORDER BY OrdinalPos";
			return Crud.HL7DefFieldCrud.SelectMany(command);
		}

		/// <summary>Gets the field list from the cache.</summary>
		public static List<HL7DefField> GetFromCache(long hl7DefSegmentNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.HL7DefSegmentNum==hl7DefSegmentNum);
		}

		///<summary></summary>
		public static long Insert(HL7DefField hL7DefField) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				hL7DefField.HL7DefFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hL7DefField);
				return hL7DefField.HL7DefFieldNum;
			}
			return Crud.HL7DefFieldCrud.Insert(hL7DefField);
		}

		///<summary></summary>
		public static void Update(HL7DefField hL7DefField) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7DefField);
				return;
			}
			Crud.HL7DefFieldCrud.Update(hL7DefField);
		}

		///<summary></summary>
		public static void Delete(long hL7DefFieldNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7DefFieldNum);
				return;
			}
			string command= "DELETE FROM hl7deffield WHERE HL7DefFieldNum = "+POut.Long(hL7DefFieldNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<HL7DefField> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7DefField>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM hl7deffield WHERE PatNum = "+POut.Long(patNum);
			return Crud.HL7DefFieldCrud.SelectMany(command);
		}

		///<summary>Gets one HL7DefField from the db.</summary>
		public static HL7DefField GetOne(long hL7DefFieldNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<HL7DefField>(MethodBase.GetCurrentMethod(),hL7DefFieldNum);
			}
			return Crud.HL7DefFieldCrud.SelectOne(hL7DefFieldNum);
		}

		*/
	}
}