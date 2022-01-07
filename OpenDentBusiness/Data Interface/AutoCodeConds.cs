using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
  ///<summary></summary>
	public class AutoCodeConds{
		#region Cache Pattern

		private class AutoCodeCondCache : CacheListAbs<AutoCodeCond> {
			protected override List<AutoCodeCond> GetCacheFromDb() {
				string command="SELECT * from autocodecond ORDER BY Cond";
				return Crud.AutoCodeCondCrud.SelectMany(command);
			}
			protected override List<AutoCodeCond> TableToList(DataTable table) {
				return Crud.AutoCodeCondCrud.TableToList(table);
			}
			protected override AutoCodeCond Copy(AutoCodeCond AutoCodeCond) {
				return AutoCodeCond.Copy();
			}
			protected override DataTable ListToTable(List<AutoCodeCond> listAutoCodeConds) {
				return Crud.AutoCodeCondCrud.ListToTable(listAutoCodeConds,"AutoCodeCond");
			}
			protected override void FillCacheIfNeeded() {
				AutoCodeConds.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutoCodeCondCache _autoCodeCondCache=new AutoCodeCondCache();

		public static List<AutoCodeCond> GetDeepCopy(bool isShort=false) {
			return _autoCodeCondCache.GetDeepCopy(isShort);
		}

		public static List<AutoCodeCond> GetWhere(Predicate<AutoCodeCond> match,bool isShort=false) {
			return _autoCodeCondCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_autoCodeCondCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_autoCodeCondCache.FillCacheFromTable(table);
				return table;
			}
			return _autoCodeCondCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(AutoCodeCond Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.AutoCodeCondNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.AutoCodeCondNum;
			}
			return Crud.AutoCodeCondCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Update(AutoCodeCond Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.AutoCodeCondCrud.Update(Cur);
		}

		///<summary></summary>
		public static void Delete(AutoCodeCond Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command="DELETE from autocodecond WHERE autocodecondnum = '"+POut.Long(Cur.AutoCodeCondNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForItemNum(long itemNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),itemNum);
				return;
			}
			string command= "DELETE from autocodecond WHERE autocodeitemnum = '"
				+POut.Long(itemNum)+"'";//AutoCodeItems.Cur.AutoCodeItemNum)
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<AutoCodeCond> GetListForItem(long autoCodeItemNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.AutoCodeItemNum==autoCodeItemNum);
		}

		///<summary></summary>
		public static bool IsSurf(AutoCondition myAutoCondition){
			//No need to check RemotingRole; no call to db.
			switch(myAutoCondition){
				case AutoCondition.One_Surf:
				case AutoCondition.Two_Surf:
				case AutoCondition.Three_Surf:
				case AutoCondition.Four_Surf:
				case AutoCondition.Five_Surf:
					return true;
				default:
					return false;
			}
		}

		///<summary></summary>
		public static bool ConditionIsMet(AutoCondition myAutoCondition, string toothNum,string surf,bool isAdditional,bool willBeMissing,int age){
			//No need to check RemotingRole; no call to db.
			switch(myAutoCondition){
				case AutoCondition.Anterior:
					return Tooth.IsAnterior(toothNum);
				case AutoCondition.Posterior:
					return Tooth.IsPosterior(toothNum);
				case AutoCondition.Premolar:
					return Tooth.IsPreMolar(toothNum);
				case AutoCondition.Molar:
					return Tooth.IsMolar(toothNum);
				case AutoCondition.One_Surf:
					return surf.Length==1;
				case AutoCondition.Two_Surf:
					return surf.Length==2;
				case AutoCondition.Three_Surf:
					return surf.Length==3;
				case AutoCondition.Four_Surf:
					return surf.Length==4;
				case AutoCondition.Five_Surf:
					return surf.Length==5;
				case AutoCondition.First:
					return !isAdditional;
				case AutoCondition.EachAdditional:
					return isAdditional;
				case AutoCondition.Maxillary:
					return Tooth.IsMaxillary(toothNum);
				case AutoCondition.Mandibular:
					return !Tooth.IsMaxillary(toothNum);
				case AutoCondition.Primary:
					return Tooth.IsPrimary(toothNum);
				case AutoCondition.Permanent:
					return !Tooth.IsPrimary(toothNum);
				case AutoCondition.Pontic:
					return willBeMissing;
				case AutoCondition.Retainer:
					return !willBeMissing;
				case AutoCondition.AgeOver18:
					return age>18;
				default:
					return false;
			}
		}
	}

	

	


}









