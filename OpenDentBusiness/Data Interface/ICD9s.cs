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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ICD9Cache.FillCacheFromTable(table);
				return table;
			}
			return _ICD9Cache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static List<ICD9> GetByCodeOrDescription(string searchTxt){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ICD9>>(MethodBase.GetCurrentMethod(),searchTxt);
			}
			string command="SELECT * FROM icd9 WHERE ICD9Code LIKE '%"+POut.String(searchTxt)+"%' "
				+"OR Description LIKE '%"+POut.String(searchTxt)+"%'";
			return Crud.ICD9Crud.SelectMany(command);
		}
		
		///<summary>Gets one ICD9 from the db.</summary>
		public static ICD9 GetOne(long iCD9Num){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ICD9>(MethodBase.GetCurrentMethod(),iCD9Num);
			}
			return Crud.ICD9Crud.SelectOne(iCD9Num);
		}

		///<summary></summary>
		public static List<ICD9> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ICD9>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM icd9";
			return Crud.ICD9Crud.SelectMany(command);
		}

		///<summary>Returns the total count of ICD9 codes.  ICD9 codes cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM icd9";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string iCD9Code) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				icd9.ICD9Num=Meth.GetLong(MethodBase.GetCurrentMethod(),icd9);
				return icd9.ICD9Num;
			}
			return Crud.ICD9Crud.Insert(icd9);
		}

		///<summary></summary>
		public static void Update(ICD9 icd9) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),icd9);
				return;
			}
			Crud.ICD9Crud.Update(icd9);
		}

		///<summary></summary>
		public static void Delete(long icd9Num) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(table.Rows.Count>0) {
				string s=Lans.g("ICD9","Not allowed to delete. Already in use by ")+table.Rows.Count.ToString()
					+" "+Lans.g("ICD9","patients, including")+" \r\n";
				for(int i=0;i<table.Rows.Count;i++) {
					if(i>5) {
						break;
					}
					s+=table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString()+" - "+table.Rows[i]["PatNum"].ToString()+"\r\n";
				}
				throw new ApplicationException(s);
			}
			command= "DELETE FROM icd9 WHERE ICD9Num = "+POut.Long(icd9Num);
			Db.NonQ(command);
		}

		///<summary>This method uploads only the ICD9s that are used by the disease table. This is to reduce upload time.</summary>
		public static List<long> GetChangedSinceICD9Nums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			//string command="SELECT ICD9Num FROM icd9 WHERE DateTStamp > "+POut.DateT(changedSince);//Dennis: delete this line later
			string command="SELECT ICD9Num FROM icd9 WHERE DateTStamp > "+POut.DateT(changedSince)
				+" AND ICD9Num in (SELECT ICD9Num FROM disease)";
			DataTable dt=Db.GetTable(command);
			List<long> icd9Nums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				icd9Nums.Add(PIn.Long(dt.Rows[i]["ICD9Num"].ToString()));
			}
			return icd9Nums;
		}

		///<summary>Used along with GetChangedSinceICD9Nums</summary>
		public static List<ICD9> GetMultICD9s(List<long> icd9Nums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ICD9>>(MethodBase.GetCurrentMethod(),icd9Nums);
			}
			string strICD9Nums="";
			DataTable table;
			if(icd9Nums.Count>0) {
				for(int i=0;i<icd9Nums.Count;i++) {
					if(i>0) {
						strICD9Nums+="OR ";
					}
					strICD9Nums+="ICD9Num='"+icd9Nums[i].ToString()+"' ";
				}
				string command="SELECT * FROM icd9 WHERE "+strICD9Nums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			ICD9[] multICD9s=Crud.ICD9Crud.TableToList(table).ToArray();
			List<ICD9> icd9List=new List<ICD9>(multICD9s);
			return icd9List;
		}

		///<summary>Returns the code and description of the icd9.</summary>
		public static string GetCodeAndDescription(string icd9Code) {
			if(string.IsNullOrEmpty(icd9Code)) {
				return "";
			}
			//No need to check RemotingRole; no call to db.
			ICD9 icd9=GetFirstOrDefault(x => x.ICD9Code==icd9Code);
			return (icd9==null ? "" : (icd9.ICD9Code+"-"+icd9.Description));
		}

		///<summary>Returns the ICD9 of the code passed in by looking in cache.  If code does not exist, returns null.</summary>
		public static ICD9 GetByCode(string icd9Code) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ICD9Code==icd9Code);
		}

		///<summary>Returns true if descriptions have not been updated to non-Caps Lock.  Always returns false if not MySQL.</summary>
		public static bool IsOldDescriptions() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT icd9code FROM icd9";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(table.Rows[i].ItemArray[0].ToString());
			}
			return retVal;
		}

		///<summary>Returns true if any of the procs have a ICD9 code.</summary>
		public static bool HasICD9Codes(List<Procedure> listProcs) {
			//No need to check RemotingRole; no call to db.
			List<string> listIcd9Codes=new List<string>();
			List<Procedure> listIcd9Procs=listProcs.FindAll(x => x.IcdVersion==9);
			listIcd9Codes.AddRange(listIcd9Procs.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode)).Select(x => x.DiagnosticCode));
			listIcd9Codes.AddRange(listIcd9Procs.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode2)).Select(x => x.DiagnosticCode2));
			listIcd9Codes.AddRange(listIcd9Procs.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode3)).Select(x => x.DiagnosticCode3));
			listIcd9Codes.AddRange(listIcd9Procs.Where(x => !string.IsNullOrEmpty(x.DiagnosticCode4)).Select(x => x.DiagnosticCode4));
			if(listIcd9Codes.Count!=0) {
				return true;
			}
			return false;
		}
	}
}