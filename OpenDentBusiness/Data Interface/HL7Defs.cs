using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using OpenDentBusiness.HL7;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class HL7Defs{
		#region CachePattern

		private class HL7DefCache : CacheListAbs<HL7Def> {
			protected override List<HL7Def> GetCacheFromDb() {
				string command="SELECT * FROM hl7def ORDER BY Description";
				return Crud.HL7DefCrud.SelectMany(command);
			}
			protected override List<HL7Def> TableToList(DataTable table) {
				return Crud.HL7DefCrud.TableToList(table);
			}
			protected override HL7Def Copy(HL7Def HL7Def) {
				return HL7Def.Clone();
			}
			protected override DataTable ListToTable(List<HL7Def> listHL7Defs) {
				return Crud.HL7DefCrud.ListToTable(listHL7Defs,"HL7Def");
			}
			protected override void FillCacheIfNeeded() {
				HL7Defs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static HL7DefCache _HL7DefCache=new HL7DefCache();

		public static HL7Def GetFirstOrDefault(Func<HL7Def,bool> match,bool isShort=false) {
			return _HL7DefCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_HL7DefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_HL7DefCache.FillCacheFromTable(table);
				return table;
			}
			return _HL7DefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_HL7DefCache.ClearCache();
		}
		#endregion

		///<summary>Gets an internal HL7Def from the database of the specified type.</summary>
		public static HL7Def GetInternalFromDb(HL7InternalType hL7InternalType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<HL7Def>(MethodBase.GetCurrentMethod(),hL7InternalType);
			}
			string command="SELECT * FROM hl7def WHERE IsInternal=1 "
				+"AND InternalType='"+POut.String(hL7InternalType.ToString())+"'";
			return Crud.HL7DefCrud.SelectOne(command);
		}

		public static List<HL7Def> GetListInternalFromDb() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HL7Def>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM hl7def WHERE IsInternal=1";
			return Crud.HL7DefCrud.SelectMany(command);
		}

		///<summary>Gets from cache.  Will get all enabled defs that are not InternalType HL7InternalType.MedLabv2_3.
		///Only one def that is not MedLabv2_3 can be enabled so this is guaranteed to return only one def.</summary>
		public static HL7Def GetOneDeepEnabled() {
			return GetOneDeepEnabled(false);
		}

		///<summary>Gets from cache.  If isMedLabHL7 is true, this will only return the enabled def if it is HL7InternalType.MedLabv2_3.
		///If false, then only those defs not of that type.  This will return null if no HL7defs are enabled.  Since only one can be enabled,
		///this will return only one.  No need to check MiddleTierRole, cache is filled by calling GetTableRemotelyIfNeeded.</summary>
		public static HL7Def GetOneDeepEnabled(bool isMedLabHL7) {
			HL7Def hL7DefRet=GetFirstOrDefault(x => x.IsEnabled && isMedLabHL7==(x.InternalType==HL7InternalType.MedLabv2_3));
			if(hL7DefRet==null) {
				return null;
			}
			if(hL7DefRet.IsInternal) {//if internal, messages, segments, and fields will not be in the database
				GetDeepForInternal(hL7DefRet);
			}
			else {
				hL7DefRet.hl7DefMessages=HL7DefMessages.GetDeepFromCache(hL7DefRet.HL7DefNum);
			}
			return hL7DefRet;
		}

		///<summary>Gets a full deep list of all internal defs.  If one is enabled, then it might be in database.</summary>
		public static List<HL7Def> GetDeepInternalList() {
			Meth.NoCheckMiddleTierRole();
			List<HL7Def> listHL7DefsInternalDb=GetListInternalFromDb();
			List<HL7Def> listHL7DefsRet=new List<HL7Def>();
			HL7Def hL7Def;
			//Whether or not the def was in the db, internal def messages, segments, and fields will not be in the db.  GetDeep from C# code
			List<HL7InternalType> listHL7InternalTypes=Enum.GetValues(typeof(HL7InternalType)).Cast<HL7InternalType>().ToList();
			for(int i=0;i<listHL7InternalTypes.Count;i++){
				hL7Def=listHL7DefsInternalDb.Find(x => x.InternalType==listHL7InternalTypes[i]);//might be null
				switch(listHL7InternalTypes[i]) {
					case HL7InternalType.eCWFull:
						listHL7DefsRet.Add(InternalEcwFull.GetDeepInternal(hL7Def));
						continue;
					case HL7InternalType.eCWStandalone:
						listHL7DefsRet.Add(InternalEcwStandalone.GetDeepInternal(hL7Def));
						continue;
					case HL7InternalType.eCWTight:
						listHL7DefsRet.Add(InternalEcwTight.GetDeepInternal(hL7Def));
						continue;
					case HL7InternalType.Centricity:
						listHL7DefsRet.Add(InternalCentricity.GetDeepInternal(hL7Def));
						continue;
					case HL7InternalType.HL7v2_6:
						listHL7DefsRet.Add(InternalHL7v2_6.GetDeepInternal(hL7Def));
						continue;
					case HL7InternalType.MedLabv2_3:
						listHL7DefsRet.Add(MedLabv2_3.GetDeepInternal(hL7Def));
						continue;
					default:
						continue;
				}
			}
			return listHL7DefsRet;
		}

		///<summary>Gets from C# internal code rather than db</summary>
		private static void GetDeepForInternal(HL7Def hL7Def) {
			Meth.NoCheckMiddleTierRole();
			if(hL7Def.InternalType==HL7InternalType.eCWFull) {
				hL7Def=InternalEcwFull.GetDeepInternal(hL7Def);//def that we're passing in is guaranteed to not be null
			}
			else if(hL7Def.InternalType==HL7InternalType.eCWStandalone) {
				hL7Def=InternalEcwStandalone.GetDeepInternal(hL7Def);
			}
			else if(hL7Def.InternalType==HL7InternalType.eCWTight) {
				hL7Def=InternalEcwTight.GetDeepInternal(hL7Def);
			}
			else if(hL7Def.InternalType==HL7InternalType.Centricity) {
				hL7Def=InternalCentricity.GetDeepInternal(hL7Def);
			}
			else if(hL7Def.InternalType==HL7InternalType.HL7v2_6) {
				hL7Def=InternalHL7v2_6.GetDeepInternal(hL7Def);
			}
			else if(hL7Def.InternalType==HL7InternalType.MedLabv2_3) {
				hL7Def=MedLabv2_3.GetDeepInternal(hL7Def);
			}
			//no need to return a def because the original reference won't have been lost.
		}

		///<summary>Tells us whether there is an existing enabled HL7Def, excluding the def with excludeHL7DefNum.
		///If isMedLabHL7 is true, this will only check to see if a def of type HL7InternalType.MedLabv2_3 is enabled.
		///Otherwise, only defs not of that type will be checked.</summary>
		public static bool IsExistingHL7Enabled(long hL7DefNumExclude,bool isMedLabHL7) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),hL7DefNumExclude,isMedLabHL7);
			}
			string command="SELECT COUNT(*) FROM hl7def WHERE IsEnabled=1 AND HL7DefNum != "+POut.Long(hL7DefNumExclude);
			if(isMedLabHL7) {
				command+=" AND InternalType='"+POut.String(HL7InternalType.MedLabv2_3.ToString())+"'";
			}
			else {
				command+=" AND InternalType!='"+POut.String(HL7InternalType.MedLabv2_3.ToString())+"'";
			}
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Tells us whether there is an existing enabled HL7Def that is not HL7InternalType.MedLabv2_3.</summary>
		public static bool IsExistingHL7Enabled() {
			return _HL7DefCache.GetWhere(x => x.IsEnabled && x.InternalType!=HL7InternalType.MedLabv2_3).Count > 0;
		}

		///<summary>Gets a full deep list of all defs that are not internal from the database.</summary>
		public static List<HL7Def> GetDeepCustomList() {
			List<HL7Def> listHL7Defs=GetShallowFromDb();
			for(int d=0;d<listHL7Defs.Count;d++) {
				listHL7Defs[d].hl7DefMessages=HL7DefMessages.GetDeepFromDb(listHL7Defs[d].HL7DefNum);
			}
			return listHL7Defs;
		}

		///<summary>Gets shallow list of all defs that are not internal from the database</summary>
		public static List<HL7Def> GetShallowFromDb() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HL7Def>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM hl7def WHERE IsInternal=0";
			return Crud.HL7DefCrud.SelectMany(command);
		}

		///<summary>Only used from Unit Tests.  Since we clear the db of hl7Defs we have to insert this internal def not update it.</summary>
		public static void EnableInternalForTests(HL7InternalType hL7InternalType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7InternalType);
				return;
			}
			HL7Def hl7Def=null;
			List<HL7Def> listHL7Defs=GetDeepInternalList();
			for(int i=0;i<listHL7Defs.Count;i++){
				if(listHL7Defs[i].InternalType==hL7InternalType){
					hl7Def=listHL7Defs[i];
					break;
				}
			}
			if(hl7Def==null) {
				return;
			}
			hl7Def.IsEnabled=true;
			Insert(hl7Def);
		}

		///<summary></summary>
		public static long Insert(HL7Def hL7Def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				hL7Def.HL7DefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hL7Def);
				return hL7Def.HL7DefNum;
			}
			return Crud.HL7DefCrud.Insert(hL7Def);
		}

		///<summary></summary>
		public static void Update(HL7Def hL7Def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7Def);
				return;
			}
			Crud.HL7DefCrud.Update(hL7Def);
		}

		///<summary></summary>
		public static void Delete(long hL7DefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7DefNum);
				return;
			}
			string command= "DELETE FROM hl7def WHERE HL7DefNum = "+POut.Long(hL7DefNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<HL7Def> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<HL7Def>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM hl7def WHERE PatNum = "+POut.Long(patNum);
			return Crud.HL7DefCrud.SelectMany(command);
		}

		///<summary>Gets one HL7Def from the db.</summary>
		public static HL7Def GetOne(long hL7DefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<HL7Def>(MethodBase.GetCurrentMethod(),hL7DefNum);
			}
			return Crud.HL7DefCrud.SelectOne(hL7DefNum);
		}

		

		
		*/
	}
}
