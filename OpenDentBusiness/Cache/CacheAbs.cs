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
using DataConnectionBase;

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
		///<summary>Clears out this cache by setting the cache entity to null. IsCacheNull() should yield true after this method is invoked which will cause the cache to automatically refresh the next time it is used.</summary>
		public abstract void ClearCache();

		public CacheAbs() {
			Cache.TrackCacheObject(this); //So we can keep track of all cache objects in one place
		}

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
			//Always go to the database in order to initialize this cache if it is null.
			if(IsCacheNull()) {
				//Throw an exception when there is no database connection which is required in order to fill this cache.
				//Open Dental has custom controls that ask the preference cache questions in event handlers. E.g. ContentsResized() for spell checking within our custom text box control.
				//These types of events will get fired prior to the database connection getting set.
				if(!DataConnection.HasDatabaseConnection) {
					throw new Exception("No database context has been set.");
				}
				//Force this cache to be refreshed from the database since it is null and there is a database connection.
				doRefreshCache=true;
			}
			//Conditionally refresh this cache from the database before returning it in DataTable format.
			if(doRefreshCache) {
				//Prefer running cache queries against the Read-Only server if one is set up and the current application is ready to invoke ReportsComplex.RunFuncOnReadOnlyServer().
				if(PrefC.HasReadOnlyServer()) {
					try {
						ReportsComplex.RunFuncOnReadOnlyServer(() => {
							FillCache(FillCacheSource.Database,null);
							return true;
						});
						return CacheToTable();
					}
					catch(Exception ex) {
						//This catch will prevent the situation where the Read-Only server was set up incorrectly.
						//Do not crash the program here so that the user is given a chance to fix the problem within the UI.
						ex.DoNothing();
					}
				}
				//Either no Read-Only server is set up or there was a problem making a connecting to the Read-Only database.
				//Fall back to filling this cache from the local database connection since cache is so important for Open Dental to function properly.
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
