using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DefLinks{
		#region Get Methods
		///<summary>Gets list of all DefLinks by defLinkType .</summary>
		public static List<DefLink> GetDefLinksByType(DefLinkType defType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DefLink>>(MethodBase.GetCurrentMethod(),defType);
			}
			string command="SELECT * FROM deflink WHERE LinkType="+POut.Int((int)defType);
			return Crud.DefLinkCrud.SelectMany(command);
		}

		///<summary>Gets list of all DefLinks for the definition and defLinkType passed in.</summary>
		public static List<DefLink> GetDefLinksByType(DefLinkType defType,long defNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DefLink>>(MethodBase.GetCurrentMethod(),defType,defNum);
			}
			string command="SELECT * FROM deflink "
				+"WHERE LinkType="+POut.Int((int)defType)+" "
				+"AND DefNum="+POut.Long(defNum);
			return Crud.DefLinkCrud.SelectMany(command);
		}

		///<summary>Gets list of all DefLinks for the definitions and defLinkType passed in.</summary>
		public static List<DefLink> GetDefLinksByTypeAndDefs(DefLinkType defType,List<long> listDefNums) {
			if(listDefNums.IsNullOrEmpty()) {
				return new List<DefLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DefLink>>(MethodBase.GetCurrentMethod(),defType,listDefNums);
			}
			string command="SELECT * FROM deflink "
				+"WHERE LinkType="+POut.Int((int)defType)+" "
				+"AND DefNum IN("+string.Join(",",listDefNums.Select(x => POut.Long(x)))+")";
			return Crud.DefLinkCrud.SelectMany(command);
		}

		///<summary>Gets list of all operatory specific DefLinks associated to any definitions for the category provided.
		///Set isShort true if hidden definitions need to be considered.</summary>
		public static List<DefLink> GetOperatoryDefLinksForCategory(DefCat defCat,bool isShort=false) {
			//No need to check RemotingRole; no call to db.
			//Get all definitions that are associated to the category.
			List<Def> listDefs=Defs.GetDefsForCategory(defCat,isShort);
			//Return all of the deflinks that are of type Operatory in order to get the operatory specific deflinks.
			return DefLinks.GetDefLinksByTypeAndDefs(DefLinkType.Operatory,listDefs.Select(x => x.DefNum).ToList());
		}

		///<summary>Gets list of all appointment type specific DefLinks associated to the WebSchedNewPatApptTypes definition category.</summary>
		public static List<DefLink> GetDefLinksForWebSchedNewPatApptApptTypes() {
			//No need to check RemotingRole; no call to db.
			//Get all definitions that are associated to the WebSchedNewPatApptTypes category that are linked to an operatory.
			List<Def> listWSNPAATDefs=Defs.GetDefsForCategory(DefCat.WebSchedNewPatApptTypes);//Cannot hide defs of this category at this time.
			//Return all of the deflinks that are of type Operatory in order to get the operatory specific deflinks.
			return DefLinks.GetDefLinksByTypeAndDefs(DefLinkType.AppointmentType,listWSNPAATDefs.Select(x => x.DefNum).ToList());
		}

		///<summary>Gets one DefLinks by FKey. Must provide DefLinkType.  Returns null if not found.</summary>
		public static DefLink GetOneByFKey(long fKey,DefLinkType defType) {
			//No need to check RemotingRole; no call to db.
			return GetListByFKeys(new List<long>() { fKey },defType).FirstOrDefault();
		}

		///<summary>Gets list of DefLinks by FKey. Must provide DefLinkType.</summary>
		public static List<DefLink> GetListByFKey(long fKey,DefLinkType defType) {
			//No need to check RemotingRole; no call to db.
			return GetListByFKeys(new List<long>() { fKey },defType);
		}

		///<summary>Gets list of DefLinks by FKeys. Must provide DefLinkType.</summary>
		public static List<DefLink> GetListByFKeys(List<long> listFKeys,DefLinkType defType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DefLink>>(MethodBase.GetCurrentMethod(),listFKeys,defType);
			}
			if(listFKeys.Count==0) {
				return new List<DefLink>();
			}
			string command="SELECT * FROM deflink WHERE FKey IN("+string.Join(",",listFKeys.Select(x => POut.Long(x)))+")"
				+" AND LinkType ="+POut.Int((int)defType);
			return Crud.DefLinkCrud.SelectMany(command);
		}

		///<summary>Gets one DefLink from the db.</summary>
		public static DefLink GetOne(long defLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DefLink>(MethodBase.GetCurrentMethod(),defLinkNum);
			}
			return Crud.DefLinkCrud.SelectOne(defLinkNum);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(DefLink defLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				defLink.DefLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),defLink);
				return defLink.DefLinkNum;
			}
			return Crud.DefLinkCrud.Insert(defLink);
		}

		///<summary>Inserts or updates the FKey entry for the corresponding definition passed in.
		///This is a helper method that should only be used when there can only be a one to one relationship between DefNum and FKey.</summary>
		public static void SetFKeyForDef(long defNum,long fKey,DefLinkType linkType) {
			//No need to check RemotingRole; no call to db.
			//Look for the def link first to decide if we need to run an update or an insert statement.
			List<DefLink> listDefLinks=GetDefLinksByType(linkType,defNum);
			if(listDefLinks.Count > 0) {
				UpdateDefWithFKey(defNum,fKey,linkType);
			}
			else {
				Insert(new DefLink() {
					DefNum=defNum,
					FKey=fKey,
					LinkType=linkType,
				});
			}
		}

		///<summary>Sync method for inserting all necessary DefNums associated to the operatory passed in.
		///Does nothing if operatory.ListWSNPAOperatoryDefNums/ListWSEPOperatoryDefNums is null.  Will delete
		///all deflinks if the list is empty. Optionally pass in the list of deflinks to consider in order to 
		///save database calls.</summary>
		public static void SyncWebSchedOpLinks(Operatory operatory,DefCat defCat,List<DefLink> listOpDefLinks=null) {
			//No need to check RemotingRole; no call to db.
			if((defCat==DefCat.WebSchedNewPatApptTypes && operatory.ListWSNPAOperatoryDefNums==null)
				|| (defCat==DefCat.WebSchedExistingApptTypes && operatory.ListWSEPOperatoryDefNums==null))
			{
				return;//null means that this column was never even considered.  Save time by simply returning.
			}
			List<long> _listWSOpDefNums=(defCat==DefCat.WebSchedNewPatApptTypes) ? operatory.ListWSNPAOperatoryDefNums : operatory.ListWSEPOperatoryDefNums;
			//Get all operatory deflinks from the database if a specific list was not passed in.
			listOpDefLinks=listOpDefLinks??GetOperatoryDefLinksForCategory(defCat);
			//Filter the deflinks down in order to get the current DefNums that are linked to the operatory passed in.
			listOpDefLinks=listOpDefLinks.Where(x => x.FKey==operatory.OperatoryNum).ToList();
			//Delete all def links that are associated to DefNums that are not in listDefNums.
			List<DefLink> listDefLinksToDelete=listOpDefLinks.Where(x => !_listWSOpDefNums.Contains(x.DefNum)).ToList();
			DeleteDefLinks(listDefLinksToDelete.Select(x => x.DefLinkNum).ToList());
			//Insert new DefLinks for all DefNums that were passed in that are not in listOpDefLinks.
			List<long> listDefNumsToInsert=_listWSOpDefNums.Where(x => !listOpDefLinks.Select(y => y.DefNum).Contains(x)).ToList();
			InsertDefLinksForDefs(listDefNumsToInsert,operatory.OperatoryNum,DefLinkType.Operatory);
			//There is no reason to "update" deflinks so there is nothing else to do.
		}

		public static void InsertDefLinksForDefs(List<long> listDefNums,long fKey,DefLinkType linkType) {
			if(listDefNums==null || listDefNums.Count < 1) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Remoting role check to save on middle tier calls due to loop.
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDefNums,fKey,linkType);
				return;
			}
			foreach(long defNum in listDefNums) {
				Insert(new DefLink() {
					DefNum=defNum,
					FKey=fKey,
					LinkType=linkType,
				});
			}
		}
		
		///<summary>Creates multiple rows from a list of foreign keys using a single defnum and link type.</summary>
		public static void InsertDefLinksForFKeys(long defNum,List<long> listFKeys,DefLinkType linkType) {
			if(listFKeys.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defNum,listFKeys,linkType);
				return;
			}
			Crud.DefLinkCrud.InsertMany(listFKeys.Select(x => new DefLink() { DefNum=defNum,FKey=x,LinkType=linkType }).ToList());
		}

		#endregion

		#region Update
		///<summary></summary>
		public static void Update(DefLink defLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defLink);
				return;
			}
			Crud.DefLinkCrud.Update(defLink);
		}

		///<summary>Updates the FKey column on all deflink rows for the corresponding definition and type.</summary>
		public static void UpdateDefWithFKey(long defNum,long fKey,DefLinkType defType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defNum,fKey,defType);
				return;
			}
			string command="UPDATE deflink SET FKey="+POut.Long(fKey)+" "
				+"WHERE LinkType="+POut.Int((int)defType)+" "
				+"AND DefNum="+POut.Long(defNum);
			Db.NonQ(command);
		}

		///<summary>Syncs two supplied lists of DefLink.</summary>
		public static bool Sync(List<DefLink> listNew,List<DefLink> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.DefLinkCrud.Sync(listNew,listOld);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long defLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defLinkNum);
				return;
			}
			Crud.DefLinkCrud.Delete(defLinkNum);
		}

		///<summary>Deletes all links for the specified FKey and link type.</summary>
		public static void DeleteAllForFKeys(List<long> listFKeys,DefLinkType defType) {
			if(listFKeys==null || listFKeys.Count < 1) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listFKeys,defType);
				return;
			}
			string command="DELETE FROM deflink "
				+"WHERE LinkType="+POut.Int((int)defType)+" "
				+"AND FKey IN("+string.Join(",",listFKeys.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		/// <summary>Deletes all deflinks for a given defnum. This also handles the cases where the DefNum is used in the FKey field.</summary>
		public static void DeleteAllForDef(long defNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defNum);
				return;
			}
			string command="DELETE FROM deflink "
				+"WHERE DefNum="+POut.Long(defNum)+" "
				+"OR ("
					+"LinkType="+POut.Int((int)DefLinkType.BlockoutType)+" "
					+"AND FKey="+POut.Long(defNum)+" "
				+")";
			Db.NonQ(command);
		}

		///<summary>Deletes all links for the specified definition and link type.</summary>
		public static void DeleteAllForDef(long defNum,DefLinkType defType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),defNum,defType);
				return;
			}
			string command="DELETE FROM deflink "
				+"WHERE LinkType="+POut.Int((int)defType)+" "
				+"AND DefNum="+POut.Long(defNum);
			Db.NonQ(command);
		}

		public static void DeleteDefLinks(List<long> listDefLinkNums) {
			if(listDefLinkNums==null || listDefLinkNums.Count < 1) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDefLinkNums);
				return;
			}
			string command="DELETE FROM deflink WHERE DefLinkNum IN ("+string.Join(",",listDefLinkNums)+")";
			Db.NonQ(command);
		}
		#endregion
	}
}




//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
/*
#region Cache Pattern
//This region can be eliminated if this is not a table type with cached data.
//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

private class DefLinkCache : CacheListAbs<DefLink> {
	protected override List<DefLink> GetCacheFromDb() {
		string command="SELECT * FROM deflink";
		return Crud.DefLinkCrud.SelectMany(command);
	}
	protected override List<DefLink> TableToList(DataTable table) {
		return Crud.DefLinkCrud.TableToList(table);
	}
	protected override DefLink Copy(DefLink defLink) {
		return defLink.Copy();
	}
	protected override DataTable ListToTable(List<DefLink> listDefLinks) {
		return Crud.DefLinkCrud.ListToTable(listDefLinks,"DefLink");
	}
	protected override void FillCacheIfNeeded() {
		DefLinks.GetTableFromCache(false);
	}
	protected override bool IsInListShort(DefLink defLink) {
		return true;//Either change this method or delete it.
	}
}

///<summary>The object that accesses the cache in a thread-safe manner.</summary>
private static DefLinkCache _defLinkCache=new DefLinkCache();

///<summary>A list of all DefLinks. Returns a deep copy.</summary>
public static List<DefLink> ListDeep {
	get {
		return _defLinkCache.ListDeep;
	}
}

///<summary>A list of all non-hidden DefLinks. Returns a deep copy.</summary>
public static List<DefLink> ListShortDeep {
	get {
		return _defLinkCache.ListShortDeep;
	}
}

///<summary>A list of all DefLinks. Returns a shallow copy.</summary>
public static List<DefLink> ListShallow {
	get {
		return _defLinkCache.ListShallow;
	}
}

///<summary>A list of all non-hidden DefLinks. Returns a shallow copy.</summary>
public static List<DefLink> ListShortShallow {
	get {
		return _defLinkCache.ListShallowShort;
	}
}

///<summary>Fills the local cache with the passed in DataTable.</summary>
public static void FillCacheFromTable(DataTable table) {
	_defLinkCache.FillCacheFromTable(table);
}

///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
public static DataTable GetTableFromCache(bool doRefreshCache) {
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
		_defLinkCache.FillCacheFromTable(table);
		return table;
	}
	return _defLinkCache.GetTableFromCache(doRefreshCache);
}
#endregion Cache Pattern
*/
