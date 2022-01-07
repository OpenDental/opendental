using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness {
	///<summary></summary>
	public class SigElementDefs {
		#region CachePattern

		private class SigElementDefCache : CacheListAbs<SigElementDef> {
			protected override List<SigElementDef> GetCacheFromDb() {
				string command="SELECT * FROM sigelementdef ORDER BY ItemOrder";
				return Crud.SigElementDefCrud.SelectMany(command);
			}
			protected override List<SigElementDef> TableToList(DataTable table) {
				return Crud.SigElementDefCrud.TableToList(table);
			}
			protected override SigElementDef Copy(SigElementDef sigElementDef) {
				return sigElementDef.Copy();
			}
			protected override DataTable ListToTable(List<SigElementDef> listSigElementDefs) {
				return Crud.SigElementDefCrud.ListToTable(listSigElementDefs,"SigElementDef");
			}
			protected override void FillCacheIfNeeded() {
				SigElementDefs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SigElementDefCache _sigElementDefCache=new SigElementDefCache();

		public static List<SigElementDef> GetWhere(Predicate<SigElementDef> match,bool isShort=false) {
			return _sigElementDefCache.GetWhere(match,isShort);
		}

		public static SigElementDef GetFirstOrDefault(Func<SigElementDef,bool> match,bool isShort=false) {
			return _sigElementDefCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_sigElementDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_sigElementDefCache.FillCacheFromTable(table);
				return table;
			}
			return _sigElementDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(SigElementDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.SigElementDefCrud.Update(def);
		}

		///<summary></summary>
		public static long Insert(SigElementDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				def.SigElementDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.SigElementDefNum;
			}
			return Crud.SigElementDefCrud.Insert(def);
		}

		///<summary></summary>
		public static void Delete(SigElementDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			string command="DELETE FROM sigelementdef WHERE SigElementDefNum ="+POut.Long(def.SigElementDefNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static SigElementDef[] GetSubList(SignalElementType sigElementType){
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.SigElementType==sigElementType).ToArray();
		}

		///<summary>Moves the selected item up in the supplied sub list.</summary>
		public static void MoveUp(int selected,SigElementDef[] subList){
			//No need to check RemotingRole; no call to db.
			if(selected<0) {
				throw new ApplicationException(Lans.g("SigElementDefs","Please select an item first."));
			}
			if(selected==0) {//already at top
				return;
			}
			if(selected>subList.Length-1){
				throw new ApplicationException(Lans.g("SigElementDefs","Invalid selection."));
			}
			SetOrder(selected-1,subList[selected].ItemOrder,subList);
			SetOrder(selected,subList[selected].ItemOrder-1,subList);
			//Selected-=1;
		}

		///<summary></summary>
		public static void MoveDown(int selected,SigElementDef[] subList) {
			//No need to check RemotingRole; no call to db.
			if(selected<0) {
				throw new ApplicationException(Lans.g("SigElementDefs","Please select an item first."));
			}
			if(selected==subList.Length-1){//already at bottom
				return;
			}
			if(selected>subList.Length-1) {
				throw new ApplicationException(Lans.g("SigElementDefs","Invalid selection."));
			}
			SetOrder(selected+1,subList[selected].ItemOrder,subList);
			SetOrder(selected,subList[selected].ItemOrder+1,subList);
			//selected+=1;
		}

		///<summary>Used by MoveUp and MoveDown.</summary>
		private static void SetOrder(int mySelNum,int myItemOrder,SigElementDef[] subList) {
			//No need to check RemotingRole; no call to db.
			SigElementDef temp=subList[mySelNum];
			temp.ItemOrder=myItemOrder;
			Update(temp);
		}

		///<summary>Returns the SigElementDef with the specified num from the cache.</summary>
		public static SigElementDef GetElementDef(long SigElementDefNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.SigElementDefNum==SigElementDefNum);
		}

		///<summary>Gets all sigelementdefs for the sigbutdef passed in.  Includes user, extra, and message element defs.</summary>
		public static List<SigElementDef> GetElementsForButDef(SigButDef sigButDef) {
			//No need to check RemotingRole; no call to db.
			List<SigElementDef> listSigElementDefs=new List<SigElementDef>();
			listSigElementDefs.AddRange(SigElementDefs.GetWhere(x => x.SigElementDefNum==sigButDef.SigElementDefNumUser));
			listSigElementDefs.AddRange(SigElementDefs.GetWhere(x => x.SigElementDefNum==sigButDef.SigElementDefNumExtra));
			listSigElementDefs.AddRange(SigElementDefs.GetWhere(x => x.SigElementDefNum==sigButDef.SigElementDefNumMsg));
			return listSigElementDefs;
		}

		///<summary>Gets all sigelementdefs for the sigmessage passed in.  Includes user, extra, and message element defs.</summary>
		public static List<SigElementDef> GetDefsForSigMessage(SigMessage sigMessage) {
			//No need to check RemotingRole; no call to db.
			List<SigElementDef> listSigElementDefs=new List<SigElementDef>();
			listSigElementDefs.AddRange(SigElementDefs.GetWhere(x => x.SigElementDefNum==sigMessage.SigElementDefNumUser));
			listSigElementDefs.AddRange(SigElementDefs.GetWhere(x => x.SigElementDefNum==sigMessage.SigElementDefNumExtra));
			listSigElementDefs.AddRange(SigElementDefs.GetWhere(x => x.SigElementDefNum==sigMessage.SigElementDefNumMsg));
			return listSigElementDefs;
		}
	}

		



		
	

	

	


}










