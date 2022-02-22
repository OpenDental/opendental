using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ionic.Zip;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RxNorms{
		/*
		#region CachePattern

		private class RxNormCache : CacheListAbs<RxNorm> {
			protected override List<RxNorm> GetCacheFromDb() {
				string command="SELECT * FROM RxNorm ORDER BY ItemOrder";
				return Crud.RxNormCrud.SelectMany(command);
			}
			protected override List<RxNorm> TableToList(DataTable table) {
				return Crud.RxNormCrud.TableToList(table);
			}
			protected override RxNorm Copy(RxNorm RxNorm) {
				return RxNorm.Clone();
			}
			protected override DataTable ListToTable(List<RxNorm> listRxNorms) {
				return Crud.RxNormCrud.ListToTable(listRxNorms,"RxNorm");
			}
			protected override void FillCacheIfNeeded() {
				RxNorms.GetTableFromCache(false);
			}
			protected override bool IsInListShort(RxNorm RxNorm) {
				return !RxNorm.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static RxNormCache _RxNormCache=new RxNormCache();

		///<summary>A list of all RxNorms. Returns a deep copy.</summary>
		public static List<RxNorm> ListDeep {
			get {
				return _RxNormCache.ListDeep;
			}
		}

		///<summary>A list of all visible RxNorms. Returns a deep copy.</summary>
		public static List<RxNorm> ListShortDeep {
			get {
				return _RxNormCache.ListShortDeep;
			}
		}

		///<summary>A list of all RxNorms. Returns a shallow copy.</summary>
		public static List<RxNorm> ListShallow {
			get {
				return _RxNormCache.ListShallow;
			}
		}

		///<summary>A list of all visible RxNorms. Returns a shallow copy.</summary>
		public static List<RxNorm> ListShort {
			get {
				return _RxNormCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_RxNormCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_RxNormCache.FillCacheFromTable(table);
				return table;
			}
			return _RxNormCache.GetTableFromCache(doRefreshCache);
		}

		#endregion*/

		///<summary>RxNorm table is considered to be too small if less than 50 RxNorms in table,
		///because our default medication list contains 50 items, implying that the user has not imported RxNorms.</summary>
		public static bool IsRxNormTableSmall() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM rxnorm";
			if(PIn.Int(Db.GetCount(command))<50) {
				return true;
			}
			return false;
		}

		public static RxNorm GetByRxCUI(string rxCui) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<RxNorm>(MethodBase.GetCurrentMethod(),rxCui);
			}
			string command="SELECT * FROM rxnorm WHERE RxCui='"+POut.String(rxCui)+"' AND MmslCode=''";
			return Crud.RxNormCrud.SelectOne(command);
		}

		///<summary>Never returns multums, only used for displaying after a search.</summary>
		public static List<RxNorm> GetListByCodeOrDesc(string codeOrDesc,bool isExact,bool ignoreNumbers) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<RxNorm>>(MethodBase.GetCurrentMethod(),codeOrDesc,isExact,ignoreNumbers);
			}
			string command="SELECT * FROM rxnorm WHERE MmslCode='' ";
			if(ignoreNumbers) {
				command+="AND Description NOT REGEXP '.*[0-9]+.*' ";
			}
			if(isExact) {
				command+="AND (RxCui LIKE '"+POut.String(codeOrDesc)+"' OR Description LIKE '"+POut.String(codeOrDesc)+"')";
			}
			else {//Similar matches
				string[] arraySearchWords=codeOrDesc.Split(new char[] { ' ','\t','\r','\n' },StringSplitOptions.RemoveEmptyEntries);
				if(arraySearchWords.Length > 0) {
					command+="AND ("
						+"RxCui LIKE '%"+POut.String(codeOrDesc)+"%' "
						+" OR "
						+"("+String.Join(" AND ",arraySearchWords.Select(x => "Description LIKE '%"+POut.String(x)+"%'"))+") "
						+")";
				}
			}
			command+=" ORDER BY Description";
			return Crud.RxNormCrud.SelectMany(command);
		}

		///<summary>Used to return the multum code based on RxCui.  If blank, use the Description instead.</summary>
		public static string GetMmslCodeByRxCui(string rxCui) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetString(MethodBase.GetCurrentMethod(),rxCui);
			}
			string command="SELECT MmslCode FROM rxnorm WHERE MmslCode!='' AND RxCui='"+rxCui+"'";
			return Db.GetScalar(command);
		}

		///<summary></summary>
		public static string GetDescByRxCui(string rxCui) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetString(MethodBase.GetCurrentMethod(),rxCui);
			}
			string command="SELECT Description FROM rxnorm WHERE MmslCode='' AND RxCui='"+rxCui+"'";
			return Db.GetScalar(command);
		}

		///<summary>Gets one RxNorm from the db.</summary>
		public static RxNorm GetOne(long rxNormNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<RxNorm>(MethodBase.GetCurrentMethod(),rxNormNum);
			}
			return Crud.RxNormCrud.SelectOne(rxNormNum);
		}

		///<summary></summary>
		public static long Insert(RxNorm rxNorm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				rxNorm.RxNormNum=Meth.GetLong(MethodBase.GetCurrentMethod(),rxNorm);
				return rxNorm.RxNormNum;
			}
			return Crud.RxNormCrud.Insert(rxNorm);
		}

		///<summary></summary>
		public static void Update(RxNorm rxNorm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rxNorm);
				return;
			}
			Crud.RxNormCrud.Update(rxNorm);
		}

		///<summary></summary>
		public static List<RxNorm> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxNorm>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM rxnorm";
			return Crud.RxNormCrud.SelectMany(command);
		}

		///<summary>Returns a list of just the codes for use in the codesystem import tool.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT RxCui FROM rxnorm";//will return some duplicates due to the nature of the data in the table. This is acceptable.
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				retVal.Add(table.Rows[i].ItemArray[0].ToString());
			}
			return retVal;
		}

		///<summary>Returns the count of all RxNorm codes in the database.  RxNorms cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM rxnorm";
			return PIn.Long(Db.GetCount(command));
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<RxNorm> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RxNorm>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM rxnorm WHERE PatNum = "+POut.Long(patNum);
			return Crud.RxNormCrud.SelectMany(command);
		}
		*/
	}
}