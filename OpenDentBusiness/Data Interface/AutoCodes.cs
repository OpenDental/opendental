using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AutoCodes{
		#region Cache Pattern

		private class AutoCodeCache : CacheDictAbs<AutoCode,long,AutoCode> {
			protected override List<AutoCode> GetCacheFromDb() {
				string command="SELECT * from autocode";
				return Crud.AutoCodeCrud.SelectMany(command);
			}
			protected override List<AutoCode> TableToList(DataTable table) {
				return Crud.AutoCodeCrud.TableToList(table);
			}
			protected override AutoCode Copy(AutoCode autoCode) {
				return autoCode.Copy();
			}
			protected override DataTable DictToTable(Dictionary<long,AutoCode> dictAutoCodes) {
				return Crud.AutoCodeCrud.ListToTable(dictAutoCodes.Values.Cast<AutoCode>().ToList(),"AutoCode");
			}
			protected override void FillCacheIfNeeded() {
				AutoCodes.GetTableFromCache(false);
			}
			protected override bool IsInDictShort(AutoCode autoCode) {
				return !autoCode.IsHidden;
			}
			protected override long GetDictKey(AutoCode autoCode) {
				return autoCode.AutoCodeNum;
			}
			protected override AutoCode GetDictValue(AutoCode autoCode) {
				return autoCode;
			}
			protected override AutoCode CopyDictValue(AutoCode autoCode) {
				return autoCode.Copy();
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutoCodeCache _autoCodeCache=new AutoCodeCache();

		public static List<AutoCode> GetListDeep(bool isShort=false) {
			return _autoCodeCache.GetDeepCopy(isShort).Values.ToList();
		}

		public static AutoCode GetOne(long codeNum) {
			return _autoCodeCache.GetOne(codeNum);
		}

		public static bool GetContainsKey(long codeNum) {
			return _autoCodeCache.GetContainsKey(codeNum);
		}

		public static int GetCount(bool isShort=false) {
			return _autoCodeCache.GetCount(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_autoCodeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_autoCodeCache.FillCacheFromTable(table);
				return table;
			}
			return _autoCodeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(AutoCode Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.AutoCodeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.AutoCodeNum;
			}
			return Crud.AutoCodeCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Update(AutoCode Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.AutoCodeCrud.Update(Cur);
		}

		///<summary>Surround with try/catch.  Currently only called from FormAutoCode and FormAutoCodeEdit.</summary>
		public static void Delete(AutoCode autoCodeCur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),autoCodeCur);
				return;
			}
			//look for dependencies in ProcButton table.
			string strInUse="";
			List<ProcButton> listProcButtons=ProcButtons.GetDeepCopy();
			List<ProcButtonItem> listProcButtonItems=ProcButtonItems.GetDeepCopy();
			for(int i=0;i<listProcButtons.Count;i++) {
				for(int j=0;j<listProcButtonItems.Count;j++) {
					if(listProcButtonItems[j].ProcButtonNum==listProcButtons[i].ProcButtonNum 
						&& listProcButtonItems[j].AutoCodeNum==autoCodeCur.AutoCodeNum) 
					{
						if(strInUse!="") {
							strInUse+="; ";
						}
						//Add the procedure button description to the list for display.
						strInUse+=listProcButtons[i].Description;
						break;//Button already added to the description, check the other buttons in the list.
					}
				}
			}
			if(strInUse!="") {
				throw new ApplicationException(Lans.g("AutoCodes","Not allowed to delete autocode because it is in use.  Procedure buttons using this autocode include ")+strInUse);
			}
			List<AutoCodeItem> listAutoCodeItems=AutoCodeItems.GetListForCode(autoCodeCur.AutoCodeNum);
			for(int i=0;i<listAutoCodeItems.Count;i++) {
				AutoCodeItem AutoCodeItemCur=listAutoCodeItems[i];
        AutoCodeConds.DeleteForItemNum(AutoCodeItemCur.AutoCodeItemNum);
        AutoCodeItems.Delete(AutoCodeItemCur);
      }
			Crud.AutoCodeCrud.Delete(autoCodeCur.AutoCodeNum);
		}

		///<summary>Used in ProcButtons.SetToDefault.  Returns 0 if the given autocode does not exist.</summary>
		public static long GetNumFromDescript(string descript) {
			//No need to check RemotingRole; no call to db.
			AutoCode autoCode=_autoCodeCache.GetFirstOrDefault(x => x.Description==descript,true);
			return (autoCode==null) ? 0 : autoCode.AutoCodeNum;
		}

		//------

		///<summary>Deletes all current autocodes and then adds the default autocodes.  Procedure codes must have already been entered or they cannot be added as an autocode.</summary>
		public static void SetToDefault() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="DELETE FROM autocode";
			Db.NonQ(command);
			command="DELETE FROM autocodecond";
			Db.NonQ(command);
			command="DELETE FROM autocodeitem";
			Db.NonQ(command);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				SetToDefaultCanada();
				return;
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				SetToDefaultMySQL();
			}
			else {//Oracle
				SetToDefaultOracle();
			}
		}

		private static void SetToDefaultMySQL() {
			//No need to check RemotingRole; Private method.
			long autoCodeNum;
			long autoCodeItemNum;
			//Amalgam-------------------------------------------------------------------------------------------------------
			string command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Amalgam',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//1Surf
			if(ProcedureCodes.IsValidCode("D2140")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2140")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
				+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
			}
			//2Surf
			if(ProcedureCodes.IsValidCode("D2150")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2150")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
			}
			//3Surf
			if(ProcedureCodes.IsValidCode("D2160")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2160")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
			}
			//4Surf
			if(ProcedureCodes.IsValidCode("D2161")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2161")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
			}
			//5Surf
			if(ProcedureCodes.IsValidCode("D2161")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2161")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
			}
			//Composite-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Composite',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//1SurfAnt
			if(ProcedureCodes.IsValidCode("D2330")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2330")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//2SurfAnt
			if(ProcedureCodes.IsValidCode("D2331")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2331")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//3SurfAnt
			if(ProcedureCodes.IsValidCode("D2332")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2332")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//4SurfAnt
			if(ProcedureCodes.IsValidCode("D2335")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2335")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//5SurfAnt
			if(ProcedureCodes.IsValidCode("D2335")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2335")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Posterior Composite----------------------------------------------------------------------------------------------
			//1SurfPost
			if(ProcedureCodes.IsValidCode("D2391")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2391")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//2SurfPost
			if(ProcedureCodes.IsValidCode("D2392")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2392")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//3SurfPost
			if(ProcedureCodes.IsValidCode("D2393")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2393")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//4SurfPost
			if(ProcedureCodes.IsValidCode("D2394")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2394")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//5SurfPost
			if(ProcedureCodes.IsValidCode("D2394")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2394")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//Root Canal-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Root Canal',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Ant
			if(ProcedureCodes.IsValidCode("D3310")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3310")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Premolar
			if(ProcedureCodes.IsValidCode("D3320")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3320")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			//Molar
			if(ProcedureCodes.IsValidCode("D3330")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3330")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
			//PFM Bridge-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('PFM Bridge',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Pontic
			if(ProcedureCodes.IsValidCode("D6242")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6242")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Pontic)+")";
				Db.NonQ(command);
			}
			//Retainer
			if(ProcedureCodes.IsValidCode("D6752")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6752")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Retainer)+")";
				Db.NonQ(command);
			}
			//Ceramic Bridge-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Ceramic Bridge',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Pontic
			if(ProcedureCodes.IsValidCode("D6245")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6245")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Pontic)+")";
				Db.NonQ(command);
			}
			//Retainer
			if(ProcedureCodes.IsValidCode("D6740")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6740")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Retainer)+")";
				Db.NonQ(command);
			}
			//Denture-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Denture',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Max
			if(ProcedureCodes.IsValidCode("D5110")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D5110")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Maxillary)+")";
				Db.NonQ(command);
			}
			//Mand
			if(ProcedureCodes.IsValidCode("D5120")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D5120")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Mandibular)+")";
				Db.NonQ(command);
			}
			//BU/P&C-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('BU/P&C',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//BU
			if(ProcedureCodes.IsValidCode("D2950")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2950")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//P&C
			if(ProcedureCodes.IsValidCode("D2954")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2954")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Root Canal Retreat--------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('RCT Retreat',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Ant
			if(ProcedureCodes.IsValidCode("D3346")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3346")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Premolar
			if(ProcedureCodes.IsValidCode("D3347")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3347")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			//Molar
			if(ProcedureCodes.IsValidCode("D3348")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3348")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
		}

		private static void SetToDefaultOracle() {
			//No need to check RemotingRole; Private method.
			long autoCodeNum;
			long autoCodeItemNum;
			#region Amalgam
			//Amalgam-------------------------------------------------------------------------------------------------------
			string command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'Amalgam',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//1Surf
			if(ProcedureCodes.IsValidCode("D2140")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2140")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
				+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
			}
			//2Surf
			if(ProcedureCodes.IsValidCode("D2150")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2150")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
			}
			//3Surf
			if(ProcedureCodes.IsValidCode("D2160")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2160")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
			}
			//4Surf
			if(ProcedureCodes.IsValidCode("D2161")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2161")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
			}
			//5Surf
			if(ProcedureCodes.IsValidCode("D2161")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2161")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
			}
			#endregion
			#region Composite
			//Composite-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'Composite',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//1SurfAnt
			if(ProcedureCodes.IsValidCode("D2330")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2330")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
				+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//2SurfAnt
			if(ProcedureCodes.IsValidCode("D2331")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2331")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//3SurfAnt
			if(ProcedureCodes.IsValidCode("D2332")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2332")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//4SurfAnt
			if(ProcedureCodes.IsValidCode("D2335")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2335")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//5SurfAnt
			if(ProcedureCodes.IsValidCode("D2335")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2335")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			#endregion
			#region Posterior Composite
			//Posterior Composite----------------------------------------------------------------------------------------------
			//1SurfPost
			if(ProcedureCodes.IsValidCode("D2391")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2391")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
				+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//2SurfPost
			if(ProcedureCodes.IsValidCode("D2392")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2392")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//3SurfPost
			if(ProcedureCodes.IsValidCode("D2393")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2393")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//4SurfPost
			if(ProcedureCodes.IsValidCode("D2394")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2394")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//5SurfPost
			if(ProcedureCodes.IsValidCode("D2394")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2394")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			#endregion
			#region Root Canal
			//Root Canal-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'Root Canal',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//Ant
			if(ProcedureCodes.IsValidCode("D3310")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3310")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Premolar
			if(ProcedureCodes.IsValidCode("D3320")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3320")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			//Molar
			if(ProcedureCodes.IsValidCode("D3330")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3330")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
			#endregion
			#region PFM Bridge
			//PFM Bridge-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'PFM Bridge',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//Pontic
			if(ProcedureCodes.IsValidCode("D6242")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6242")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Pontic)+")";
				Db.NonQ(command);
			}
			//Retainer
			if(ProcedureCodes.IsValidCode("D6752")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6752")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Retainer)+")";
				Db.NonQ(command);
			}
			#endregion
			#region Ceramic Bridge
			//Ceramic Bridge-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'Ceramic Bridge',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//Pontic
			if(ProcedureCodes.IsValidCode("D6245")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6245")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Pontic)+")";
				Db.NonQ(command);
			}
			//Retainer
			if(ProcedureCodes.IsValidCode("D6740")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D6740")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Retainer)+")";
				Db.NonQ(command);
			}
			#endregion
			#region Denture
			//Denture-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'Denture',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//Max
			if(ProcedureCodes.IsValidCode("D5110")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D5110")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Maxillary)+")";
			}
			//Mand
			if(ProcedureCodes.IsValidCode("D5120")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D5120")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Mandibular)+")";
				Db.NonQ(command);
			}
			#endregion
			#region BU/P&C
			//BU/P&C-------------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'BU/P&C',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//BU
			if(ProcedureCodes.IsValidCode("D2950")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2950")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Posterior)+")";
				Db.NonQ(command);
			}
			//P&C
			if(ProcedureCodes.IsValidCode("D2954")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D2954")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			#endregion
			#region Root Canal Retreat
			//Root Canal Retreat--------------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (AutoCodeNum,Description,IsHidden,LessIntrusive) "
				+"VALUES ((SELECT COALESCE(MAX(AutoCodeNum),0)+1 AutoCodeNum FROM autocode),'RCT Retreat',0,0)";
			autoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autocode");
			//Ant
			if(ProcedureCodes.IsValidCode("D3346")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3346")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Premolar
			if(ProcedureCodes.IsValidCode("D3347")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3347")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			//Molar
			if(ProcedureCodes.IsValidCode("D3348")) {
				command="INSERT INTO autocodeitem (AutoCodeItemNum,AutoCodeNum,CodeNum) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeItemNum),0)+1 AutoCodeItemNum FROM autocodeitem),"+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("D3348")+")";
				autoCodeItemNum=Db.NonQ(command,true,"AutoCodeItemNum","autocodeitem");
				command="INSERT INTO autocodecond (AutoCodeCondNum,AutoCodeItemNum,Cond) "
					+"VALUES ((SELECT COALESCE(MAX(AutoCodeCondNum),0)+1 AutoCodeCondNum FROM autocodecond),"+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
			#endregion
		}

		///<summary>Deletes all current autocodes and then adds the default autocodes.  Procedure codes must have already been entered or they cannot be added as an autocode.</summary>
		public static void SetToDefaultCanada() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				throw new ApplicationException("SetToDefaultCanada is not Oracle compatible.  Please call support.");
			}
			string command;
			long autoCodeNum;
			long autoCodeItemNum;
			//Amalgam-Bonded--------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Amalgam-Bonded',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//1SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21121")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21121")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//1SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21121")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21121")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//2SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21122")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21122")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//2SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21122")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21122")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//3SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21123")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21123")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//3SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21123")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21123")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//4SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21124")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21124")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//4SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21124")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21124")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//5SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21125")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21125")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//5SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21125")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21125")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21231")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21231")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21231")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21231")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21232")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21232")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21232")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21232")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21233")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21233")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21233")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21233")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21234")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21234")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21234")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21234")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21235")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21235")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21235")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21235")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21241")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21241")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21242")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21242")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21243")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21243")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21244")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21244")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21245")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21245")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//Amalgam Non-Bonded----------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Amalgam Non-Bonded',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//1SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21111")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21111")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//1SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21111")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21111")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//2SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21112")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21112")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//2SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21112")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21112")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//3SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21113")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21113")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//3SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21113")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21113")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//4SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21114")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21114")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//4SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21114")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21114")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//5SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("21115")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21115")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//5SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("21115")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21115")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21211")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21211")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21211")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21211")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21212")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21212")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21212")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21212")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21213")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21213")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21213")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21213")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21214")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21214")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21214")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21214")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("21215")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21215")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("21215")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21215")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21221")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21221")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21222")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21222")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21223")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21223")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21224")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21224")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("21225")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("21225")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//Composite-------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Composite',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//1SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("23111")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23111")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("23112")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23112")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("23113")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23113")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("23114")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23114")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentAnterior
			if(ProcedureCodes.IsValidCode("23115")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23115")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("23311")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23311")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("23312")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23312")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("23313")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23313")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("23314")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23314")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentPremolar
			if(ProcedureCodes.IsValidCode("23315")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23315")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("23321")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23321")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//2SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("23322")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23322")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//3SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("23323")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23323")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//4SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("23324")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23324")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//5SurfPermanentMolar
			if(ProcedureCodes.IsValidCode("23325")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23325")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Permanent)+")";
				Db.NonQ(command);
			}
			//1SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("23411")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23411")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//2SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("23412")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23412")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//3SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("23413")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23413")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//4SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("23414")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23414")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//5SurfPrimaryAnterior
			if(ProcedureCodes.IsValidCode("23415")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23415")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//1SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("23511")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23511")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//2SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("23512")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23512")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//3SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("23513")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23513")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//4SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("23514")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23514")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//5SurfPrimaryMolar
			if(ProcedureCodes.IsValidCode("23515")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("23515")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Primary)+")";
				Db.NonQ(command);
			}
			//Gold Inlay----------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Gold Inlay',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			if(ProcedureCodes.IsValidCode("25113")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25113")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25112")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25112")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25111")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25111")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25114")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25114")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25114")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25114")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
			}
			//Open&Drain----------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Open&Drain',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			if(ProcedureCodes.IsValidCode("39201")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("39201")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("39201")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("39201")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("39202")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("39202")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
			//OpenThruCrown-------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('OpenThruCrown',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			if(ProcedureCodes.IsValidCode("39212")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("39212")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("39211")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("39211")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("39211")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("39211")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			//PFM Bridge----------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('PFM Bridge',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Pontic
			if(ProcedureCodes.IsValidCode("62501")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("62501")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Pontic)+")";
				Db.NonQ(command);
			}
			//Retainer
			if(ProcedureCodes.IsValidCode("67211")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("67211")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Retainer)+")";
				Db.NonQ(command);
			}
			//Porcelain Inlay-----------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('Porcelain Inlay',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			if(ProcedureCodes.IsValidCode("25141")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25141")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.One_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25142")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25142")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Two_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25143")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25143")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Three_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25144")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25144")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Four_Surf)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("25144")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("25144")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Five_Surf)+")";
				Db.NonQ(command);
			}
			//RCTDifficult--------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('RCTDifficult',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			if(ProcedureCodes.IsValidCode("33122")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("33122")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("33112")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("33112")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			if(ProcedureCodes.IsValidCode("33132")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("33112")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
			//RCTSimple-------------------------------------------------------------------------------------------
			command="INSERT INTO autocode (Description,IsHidden,LessIntrusive) VALUES ('RCTSimple',0,0)";
			autoCodeNum=Db.NonQ(command,true);
			//Anterior
			if(ProcedureCodes.IsValidCode("33111")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("33111")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Anterior)+")";
				Db.NonQ(command);
			}
			//Premolar
			if(ProcedureCodes.IsValidCode("33121")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("33121")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Premolar)+")";
				Db.NonQ(command);
			}
			//Molar
			if(ProcedureCodes.IsValidCode("33131")) {
				command="INSERT INTO autocodeitem (AutoCodeNum,CodeNum) VALUES ("+POut.Long(autoCodeNum)+","
					+ProcedureCodes.GetCodeNum("33131")+")";
				autoCodeItemNum=Db.NonQ(command,true);
				command="INSERT INTO autocodecond (AutoCodeItemNum,Cond) VALUES ("+POut.Long(autoCodeItemNum)+","
					+POut.Long((int)AutoCondition.Molar)+")";
				Db.NonQ(command);
			}
		}
	}

	


}









