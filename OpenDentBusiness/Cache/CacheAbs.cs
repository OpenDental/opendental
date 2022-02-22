using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>The purpose of this class is to provide a shared read lock and an exclusive write lock on a cache.</summary>
	public abstract class CacheAbs<T> where T : TableBase {
		///<summary>Called at the end of FillCache which tells all implementors to refresh their caches with the new items within listItemsAll.</summary>
		protected abstract void OnNewCacheReceived(List<T> listItemsAll);
		///<summary>This method queries the database for the list of objects and return it. Remoting role does not need to be checked.</summary>
		protected abstract List<T> GetCacheFromDb();
		///<summary>This method takes in a DataTable and turns it into a list of objects.</summary>
		protected abstract List<T> TableToList(DataTable table);
		///<summary></summary>
		protected abstract T Copy(T tableBase);
		///<summary>Returns the main cache entity as a DataTable.  This is for middle tier purposes, see Cache.GetCacheDs() for details.</summary>
		protected abstract DataTable CacheToTable();
		///<summary>After this method has run, both the client's and the server's cache should be initialized.</summary>
		protected abstract void FillCacheIfNeeded();
		///<summary>Return true if the cache entity is null.</summary>
		protected abstract bool IsCacheNull();

		///<summary>Fills the cache using the specified source. If source is Database, then table can be null.</summary>
		private void FillCache(FillCacheSource source,DataTable table) {
			//Get a list that can be used later to quickly set the cache.
			List<T> listItemsAll=new List<T>();
			Logger.LogToPath(""+typeof(T).Name,LogPath.Signals,LogPhase.Start);
			if(source==FillCacheSource.Database) {
				listItemsAll=GetCacheFromDb();
			}
			else if(source==FillCacheSource.DataTable) {
				listItemsAll=TableToList(table);
			}
			OnNewCacheReceived(listItemsAll);
			Logger.LogToPath(""+typeof(T).Name,LogPath.Signals,LogPhase.End,"Got "+listItemsAll.Count.ToString()+" items");
		}
		
		///<summary>Fills the cache using the provided DataTable. Thread safe. This can be called from ClientWeb.</summary>
		public void FillCacheFromTable(DataTable table) {
			FillCache(FillCacheSource.DataTable,table);
		}

		///<summary>Gets the table from the cache. This should not be called from ClientWeb.</summary>
		public DataTable GetTableFromCache(bool doRefreshCache) {
			if(IsCacheNull() || doRefreshCache) {
				FillCache(FillCacheSource.Database,null);
			}
			return CacheToTable();
		}

		///<summary>The source that the cache will be filled from.</summary>
		private enum FillCacheSource {
			///<summary>Cache is to be filled from the database.</summary>
			Database,
			///<summary>Cache is to be filled using the provided DataTable.</summary>
			DataTable
		}
	}

}
