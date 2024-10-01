using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ICD9s{
		#region CachePattern

		private class ICD9Cache : CacheListAbs<ICD9> {
			protected override List<ICD9> GetCacheFromDb() {
				string command="SELECT * FROM icd9 ORDER BY ICD9Code";
				return Crud.ICD9Crud.SelectMany(command);
			}
			protected override List<ICD9> TableToList(DataTable table) {
				return Crud.ICD9Crud.TableToList(table);
			}
			protected override ICD9 Copy(ICD9 ICD9) {
				return ICD9.Copy();
			}
			protected override DataTable ListToTable(List<ICD9> listICD9s) {
				return Crud.ICD9Crud.ListToTable(listICD9s,"ICD9");
			}
			protected override void FillCacheIfNeeded() {
				ICD9s.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ICD9Cache _ICD9Cache=new ICD9Cache();

		public static ICD9 GetFirstOrDefault(Func<ICD9,bool> match,bool isShort=false) {
			return _ICD9Cache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ICD9Cache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ICD9Cache.FillCacheFromTable(table);
				return table;
			}
			return _ICD9Cache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_ICD9Cache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static List<ICD9> GetByCodeOrDescription(string searchTxt){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ICD9>>(MethodBase.GetCurrentMethod(),searchTxt);
			}
			string command="SELECT * FROM icd9 WHERE ICD9Code LIKE '%"+POut.String(searchTxt)+"%' "
				+"OR Description LIKE '%"+POut.String(searchTxt)+"%'";
			return Crud.ICD9Crud.SelectMany(command);
		}
		
		///<summary>Gets one ICD9 from the db.</summary>
		public static ICD9 GetOne(long iCD9Num){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ICD9>(MethodBase.GetCurrentMethod(),iCD9Num);
			}
			return Crud.ICD9Crud.SelectOne(iCD9Num);
		}

		///<summary></summary>
		public static List<ICD9> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ICD9>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM icd9";
			return Crud.ICD9Crud.SelectMany(command);
		}

		///<summary>Returns the total count of ICD9 codes.  ICD9 codes cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM icd9";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string iCD9Code) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),iCD9Code);
			}
			string command="SELECT COUNT(*) FROM icd9 WHERE ICD9Code = '"+POut.String(iCD9Code)+"'";
			string count=Db.GetCount(command);
			if(count=="0") {
				return false;
			}
			return true;
		}

		///<summary></summary>
		public static long Insert(ICD9 icd9){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				icd9.ICD9Num=Meth.GetLong(MethodBase.GetCurrentMethod(),icd9);
				return icd9.ICD9Num;
			}
			return Crud.ICD9Crud.Insert(icd9);
		}

		///<summary></summary>
		public static void Update(ICD9 icd9) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),icd9);
				return;
			}
			Crud.ICD9Crud.Update(icd9);
		}

		///<summary></summary>
		public static void Delete(long icd9Num) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),icd9Num);
				return;
			}
			string command="SELECT LName,FName,patient.PatNum FROM patient,disease,diseasedef,icd9 WHERE "
				+"patient.PatNum=disease.PatNum "
				+"AND disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
				+"AND diseasedef.ICD9Code=icd9.ICD9Code "
				+"AND icd9.ICD9Num='"+POut.Long(icd9Num)+"' "
				+"GROUP BY patient.PatNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				command= "DELETE FROM icd9 WHERE ICD9Num = "+POut.Long(icd9Num);
				Db.NonQ(command);
				return;
			}
			string exceptionMsg=Lans.g("ICD9","Not allowed to delete. Already in use by ")+table.Rows.Count.ToString()
				+" "+Lans.g("ICD9","patients, including")+" \r\n";
			for(int i=0;i<table.Rows.Count;i++) {
				if(i>5) {
					break;
				}
				exceptionMsg+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString()+" - "+table.Rows[i]["PatNum"].ToString()+"\r\n";
			}
			throw new ApplicationException(exceptionMsg);
		}

		///<summary>This method uploads only the ICD9s that are used by the disease table. This is to reduce upload time.</summary>
		public static List<long> GetChangedSinceICD9Nums(DateTime dateTChangedSince) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateTChangedSince);
			}
			//string command="SELECT ICD9Num FROM icd9 WHERE DateTStamp > "+POut.DateT(changedSince);//Dennis: delete this line later
			string command="SELECT ICD9Num FROM icd9 WHERE DateTStamp > "+POut.DateT(dateTChangedSince)
				+" AND ICD9Num in (SELECT ICD9Num FROM disease)";
			DataTable table=Db.GetTable(command);
			List<long> listIcd9Nums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				listIcd9Nums.Add(PIn.Long(table.Rows[i]["ICD9Num"].ToString()));
			}
			return listIcd9Nums;
		}

		///<summary>Used along with GetChangedSinceICD9Nums</summary>
		public static List<ICD9> GetMultICD9s(List<long> listICD9Nums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ICD9>>(MethodBase.GetCurrentMethod(),listICD9Nums);
			}
			string strICD9Nums="";
			DataTable table=new DataTable();
			if(listICD9Nums.Count>0) {
				for(int i=0;i<listICD9Nums.Count;i++) {
					if(i>0) {
						strICD9Nums+="OR ";
					}
					strICD9Nums+="ICD9Num='"+listICD9Nums[i].ToString()+"' ";
				}
				string command="SELECT * FROM icd9 WHERE "+strICD9Nums;
				table=Db.GetTable(command);
			}
			return Crud.ICD9Crud.TableToList(table);
		}

		///<summary>Returns the code and description of the icd9.</summary>
		public static string GetCodeAndDescription(string iCD9Code) {
			if(string.IsNullOrEmpty(iCD9Code)) {
				return "";
			}
			Meth.NoCheckMiddleTierRole();
			ICD9 iCD9=GetFirstOrDefault(x => x.ICD9Code==iCD9Code);
			if(iCD9==null) {
				return "";
			}
			return iCD9.ICD9Code+"-"+iCD9.Description;
		}

		///<summary>Returns the ICD9 of the code passed in by looking in cache.  If code does not exist, returns null.</summary>
		public static ICD9 GetByCode(string iCD9Code) {
			Meth.NoCheckMiddleTierRole();
			return GetFirstOrDefault(x => x.ICD9Code==iCD9Code);
		}

		///<summary>Returns true if descriptions have not been updated to non-Caps Lock.  Always returns false if not MySQL.</summary>
		public static bool IsOldDescriptions() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				return false;
			}
			string command=@"SELECT COUNT(*) FROM icd9 WHERE BINARY description = UPPER(description)";//count rows that are all caps
			if(PIn.Int(Db.GetScalar(command))>10000) {//"Normal" DB should have 4, might be more if hand entered, over 10k means it is the old import.
				return true;
			}
			return false;
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> listICD9Codes=new List<string>();
			string command="SELECT icd9code FROM icd9";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				listICD9Codes.Add(table.Rows[i].ItemArray[0].ToString());
			}
			return listICD9Codes;
		}

		///<summary>Returns true if any of the procs have a ICD9 code.</summary>
		public static bool HasICD9Codes(List<Procedure> listProcedures) {
			Meth.NoCheckMiddleTierRole();
			List<string> listICD9Codes=new List<string>();
			List<Procedure> listProceduresICD9=listProcedures.FindAll(x => x.IcdVersion==9);
			listICD9Codes.AddRange(listProceduresICD9.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode)).Select(x => x.DiagnosticCode));
			listICD9Codes.AddRange(listProceduresICD9.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode2)).Select(x => x.DiagnosticCode2));
			listICD9Codes.AddRange(listProceduresICD9.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode3)).Select(x => x.DiagnosticCode3));
			listICD9Codes.AddRange(listProceduresICD9.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode4)).Select(x => x.DiagnosticCode4));
			if(listICD9Codes.Count!=0) {
				return true;
			}
			return false;
		}
	}
}