using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Provides a cache pattern for S-classes that use a Dictionary to store data instead of a list.</summary>
	public abstract class CacheDictAbs<T,KEY_TYPE,VALUE_TYPE> : CacheAbs<T> where T : TableBase {
		protected abstract DataTable DictToTable(Dictionary<KEY_TYPE,VALUE_TYPE> dictAllItems);
		///<summary>Return the expected key for the dictionary (typically the primary key).</summary>
		protected abstract KEY_TYPE GetDictKey(T tableBase);
		///<summary>Return the expected key for the dictionary (typically the tablebase).</summary>
		protected abstract VALUE_TYPE GetDictValue(T tableBase);
		///<summary>Return a deep copy of the expected value for the dictionary (typically the tablebase).</summary>
		protected abstract VALUE_TYPE CopyDictValue(VALUE_TYPE value);

		///<summary>A lock object that allows multiple threads to obtain a read lock but allows only one thread to obtain a write lock.</summary>
		private ReaderWriterLockSlim _lock=new ReaderWriterLockSlim();
		///<summary>The one and only stored copy of the cache items.</summary>
		private Dictionary<KEY_TYPE,VALUE_TYPE> _dictAllItems;
		///<summary>Keeps track of all dictionary keys that represent key value pairs that belong to the short version of this cache.
		///This is so that we don't have to store two entire copies of the cache. Will be kept in sync with _dictAllItems.</summary>
		private List<KEY_TYPE> _listShortKeys;

		///<summary>Return true if the object should be included in the short version of this cache and return false if it shouldn't be included.
		///If no short version of the given cache exists then no need to override this method.
		///This should be something very simple, e.g. !isHidden, do not make complicated logic or database calls.</summary>
		protected virtual bool IsInDictShort(T tableBase) {
			return true;
		}

		///<summary>Returns the short version of the VALUE_TYPE.
		///Only useful for caches that have complicated value types that change between short and long versions.
		///If no short version of the given cache exists then no need to override this method.
		///This should be something very simple, e.g. !isHidden, do not modify value, make database calls, or perform resource heavy computations.
		///This method is invoked in the context of a cache read or write lock and should be used sparingly.</summary>
		protected virtual VALUE_TYPE GetShortValue(VALUE_TYPE value) {
			return value;
		}

		///<summary>CacheDictNonPkAbs needs to know when a new cache is received.</summary>
		protected virtual void GotNewCache(List<T> listAllItems) {

		}

		///<summary>Returns true if _dictAllItems is null; otherwise, false.</summary>
		public bool DictIsNull() {
			return IsCacheNull();
		}

		///<summary>Safe to call anytime and will ensure that _dictAllItems gets filled if needed.</summary>
		protected void FillDictIfNull() {
			if(IsCacheNull()) {
				FillCacheIfNeeded();
			}
		}

		///<summary>This method can be overridden when the extending class needs a more complicated dictionary.
		///Returns a dictionary that is comprised of all items that are passed in.
		///If not overridden, returns a shallow dictionary that will be comprised via invoking GetDictKey() and GetDictValue().
		///E.g. The Defs cache cannot create a correct dictionary by having GetDictValue(T) implemented because it needs more than one T.</summary>
		protected virtual Dictionary<KEY_TYPE,VALUE_TYPE> GetDictFromList(List<T> listAllItems) {
			return listAllItems.ToDictionary(x => GetDictKey(x),x => GetDictValue(x));
		}

		///<summary>This method can be overridden when the extending class needs a more complicated dictionary.
		///Returns a shallow list that represents all dictionary keys that comprise the short version of the cache.
		///If not overridden, returns a shallow list that will be comprised via invoking IsInDictShort() and GetDictKey().</summary>
		protected virtual List<KEY_TYPE> GetDictShortKeys(List<T> listAllItems) {
			//Set the short keys.
			List<KEY_TYPE> listShortKeys=listAllItems
				//Only the keys that meet the criteria to be considered in the short list.
				.FindAll(x => IsInDictShort(x))
				//Get the key from the type (T).
				.Select(x => GetDictKey(x)).ToList();
			return listShortKeys;
		}

		///<summary>Several methods that manipulate _dictAllItems need the "short" version sometimes.  This helper method simply saves code.
		///Calling method must already be in a Read or Write lock. If not then this method will throw an exception.</summary>
		private Dictionary<KEY_TYPE,VALUE_TYPE> GetShallowHelper(bool isShort=false) {
			if(!_lock.IsReadLockHeld && !_lock.IsWriteLockHeld) {
				throw new ODException("GetShallowHelper() requires a lock to be present before invoking the method.");
			}
			Dictionary<KEY_TYPE,VALUE_TYPE> dict=new Dictionary<KEY_TYPE,VALUE_TYPE>();
			if(isShort) {
				foreach(KEY_TYPE key in _listShortKeys) {
					dict[key]=GetShortValue(_dictAllItems[key]);
				}
			}
			else {
				dict=_dictAllItems;
			}
			return dict;
		}

		///<summary>Returns true if given key exists in cache.  Optionally set isShort true to check short version of cache only.</summary>
		public bool GetContainsKey(KEY_TYPE key,bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				if(isShort) {
					return _listShortKeys.Contains(key);
				}
				else {
					return _dictAllItems.ContainsKey(key);
				}
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Optionally set isShort true to check short version of cache only.</summary>
		public int GetCount(bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				if(isShort) {
					return _listShortKeys.Count;
				}
				else {
					return _dictAllItems.Keys.Count;
				}
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Returns a deep copy of the entire dictionary for looping purposes.
		///Optionally set isShort true to check short version of cache only.</summary>
		public Dictionary<KEY_TYPE,VALUE_TYPE> GetDeepCopy(bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				Dictionary<KEY_TYPE,VALUE_TYPE> dict=GetShallowHelper(isShort);
				Dictionary<KEY_TYPE,VALUE_TYPE> dictDeepCopy=new Dictionary<KEY_TYPE, VALUE_TYPE>();
				foreach(KeyValuePair<KEY_TYPE,VALUE_TYPE> kvp in dict) {
					dictDeepCopy[kvp.Key]=CopyDictValue(kvp.Value);
				}
				return dictDeepCopy;
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Returns a deep copy of the dictionary's value for the corresponding key.  Throws KeyNotFoundException if no match is found.</summary>
		public VALUE_TYPE GetOne(KEY_TYPE key,bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				if(isShort) {
					//The key passed in may in fact be present within _dictAllItems so check _listShortKeys instead.
					if(!_listShortKeys.Contains(key)) {
						throw new KeyNotFoundException();
					}
					//Now that we know the key passed in is in fact supposed to be within the short version of the cache, get the short values from it.
					return CopyDictValue(GetShortValue(_dictAllItems[key]));
				}
				return CopyDictValue(_dictAllItems[key]);
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Gets a deep copy of the first value in the dictionary that match the predicate passed in.
		///Optionally set isShort true to check short version of cache only.  If value not found then returns null.</summary>
		public VALUE_TYPE GetFirstOrDefault(Func<VALUE_TYPE,bool> match,bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				Dictionary<KEY_TYPE,VALUE_TYPE> dict=GetShallowHelper(isShort);
				VALUE_TYPE value=dict.Values.FirstOrDefault(match);
				if(value==null) {
					return value;
				}
				else {
					return CopyDictValue(value);
				}
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Gets a deep copy of all values in the dictionary that match the predicate passed in.
		///Optionally set isShort true to check short version of cache only.  If value not found then returns empty list.</summary>
		public List<VALUE_TYPE> GetWhere(Func<VALUE_TYPE,bool> match,bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				Dictionary<KEY_TYPE,VALUE_TYPE> dict=GetShallowHelper(isShort);
				return dict.Values.Where(match).Select(x => CopyDictValue(x)).ToList();
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Gets a deep copy of all values in the dictionary that have a KEY that matches the predicate passed in.
		///Optionally set isShort true to check short version of cache only.  If value not found then returns empty list.</summary>
		public List<VALUE_TYPE> GetWhereForKey(Func<KEY_TYPE,bool> match,bool isShort=false) {
			FillDictIfNull();
			_lock.EnterReadLock();
			try {
				Dictionary<KEY_TYPE,VALUE_TYPE> dict=GetShallowHelper(isShort);
				List<KEY_TYPE> listKeys=dict.Keys.Where(match).ToList();
				List<VALUE_TYPE> listValues=new List<VALUE_TYPE>();
				foreach(KEY_TYPE key in listKeys) {
					listValues.Add(CopyDictValue(dict[key]));
				}
				return listValues;
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>Returns true if the key is successfully found and removed; otherwise, false.
		///Removes from dictionary and short list if applicable.</summary>
		public bool RemoveKey(KEY_TYPE key) {
			FillDictIfNull();
			//There is no such thing as removing a dictionary entry from one dictionary and not the other.  Always remove from both.
			_lock.EnterWriteLock();
			try {
				_listShortKeys.Remove(key);
				return _dictAllItems.Remove(key);
			}
			finally {
				_lock.ExitWriteLock();
			}
		}

		///<summary>Tries to add the key value pair to the dictionary.  Returns true if successfully added; otherwise, false.
		///Optionally set isShort true to add to short version of cache only.</summary>
		public bool AddValueForKey(KEY_TYPE key,VALUE_TYPE value,bool isShort=false) {
			FillDictIfNull();
			bool wasKeyAdded=false;
			_lock.EnterWriteLock();
			try {
				if(isShort) {
					if(!_listShortKeys.Contains(key)) {
						wasKeyAdded=true;//Technically we "added" this key to the short list even if it is already within _dictAllItems.
						_listShortKeys.Add(key);
					}
				}
				if(!_dictAllItems.ContainsKey(key)) {
					wasKeyAdded=true;
					_dictAllItems.Add(key,CopyDictValue(value));
				}
			}
			finally {
				_lock.ExitWriteLock();
			}
			return wasKeyAdded;
		}

		///<summary>Forces the key to be set to the value passed in.  Optionally set isShort true to add to short version of cache as well.</summary>
		public void SetValueForKey(KEY_TYPE key,VALUE_TYPE value,bool isShort=false) {
			FillDictIfNull();
			_lock.EnterWriteLock();
			try {
				if(isShort && !_listShortKeys.Contains(key)) {
					_listShortKeys.Add(key);
				}
				_dictAllItems[key]=CopyDictValue(value);
			}
			finally {
				_lock.ExitWriteLock();
			}
		}

		#region Sealed methods
		///<summary>Fill the dictionary with the data from the base List. 
		///This has been sealed at this level on purpose so that no concreted implementers can override its behavior.</summary>
		protected sealed override void OnNewCacheReceived(List<T> listAllItems) {
			//As of right now, we require T to be a long or a string.  A long conversation needs to take place if another type needs to be introduced.
			//The bottom line is that T cannot be a type where a shallow copy is not equivalent to a deep copy.
			//This allows us to use the accessor of the dictionary.  E.g. _dictAllItems[key]
			if(typeof(KEY_TYPE)!=typeof(string) && typeof(KEY_TYPE)!=typeof(long) && !typeof(KEY_TYPE).IsEnum) {
				throw new ODException("CacheDictAbs requires KEY_TYPE to be of type string or long.");
			}
			//Let anyone that cares know we just got a new cache.
			GotNewCache(listAllItems);
			//New instance will be filled and then become _dictAllItems. No read/write lock necessary in this context.
			Dictionary<KEY_TYPE,VALUE_TYPE> dict=GetDictFromList(listAllItems);
			List<KEY_TYPE> listShortKeys=GetDictShortKeys(listAllItems);
			//Setters will correctly handle any necessary locking.
			_lock.EnterWriteLock();
			try {
				//Set the short keys.
				_listShortKeys=listShortKeys;
				//Set the dictionary items within the same lock.
				_dictAllItems=dict;
			}
			finally {
				_lock.ExitWriteLock();
			}
		}

		///<summary>Returns if the dictionary is null; otherwise, false.</summary>
		protected sealed override bool IsCacheNull() {
			_lock.EnterReadLock();
			try {
				return _dictAllItems==null;
			}
			finally {
				_lock.ExitReadLock();
			}
		}

		///<summary>No longer sealed so that CacheDictNonPkAbs can override.  Once we get rid of the Non PK version this can be resealed.</summary>
		protected /* sealed */ override DataTable CacheToTable() {
			//If _dictAllItems is null, make sure it gets filled before we continue.
			FillDictIfNull();
			Dictionary<KEY_TYPE,VALUE_TYPE> dict=new Dictionary<KEY_TYPE,VALUE_TYPE>();
			//Deep copy both the key and the value so either could be modified without modifying the cache.
			_lock.EnterReadLock();
			try {
				foreach(KEY_TYPE key in _dictAllItems.Keys) {
					dict.Add(key,CopyDictValue(_dictAllItems[key]));
				}
			}
			finally {
				_lock.ExitReadLock();
			}
			//Convert our new deep copy to a DataTable.
			return DictToTable(dict);
		}
		#endregion
	}
}
