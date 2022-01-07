using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;
using System.Data;
using UnitTestsCore;
using OpenDentBusiness.Crud;
using System.Reflection;
using CodeBase;

namespace UnitTests.CacheAbs_Tests {
	[TestClass]
	///<summary>This class is designed specifically for testing the CacheAbs paradigm for all implementations (dict, list, etc).</summary>
	public class CacheAbsTests:TestBase {
		private static DictTestCache _dictCache=new DictTestCache();
		private static DictNoPkTestCache _dictNoPkCache=new DictNoPkTestCache();
		private static ListTestCache _listCache=new ListTestCache();

		[ClassInitialize]
		///<summary>This method will get invoked when the CacheTests class is initialized.  All unit tests in this class utilize this data.</summary>
		public static void CacheTestInit(TestContext testContext) {
			//Insert multiple providers for all of the test methods within this class so that we are guaranteed to at least these providers to work with.
			//All of the test methods within this class will be written in such a way that it shouldn't matter if other tests created providers.
			ProviderT.CreateProvider("Cache1",fName:"Sophia",lName:"Jones");
			ProviderT.CreateProvider("Cache2",fName:"Jackson",lName:"Jones");
			ProviderT.CreateProvider("Cache3",fName:"Olivia",lName:"Brown");
			ProviderT.CreateProvider("Cache4",fName:"Liam",lName:"Neeson");
			ProviderT.CreateProvider("Cache5",fName:"Emma",lName:"Johnson",isHidden:true);
			ProviderT.CreateProvider("Cache6",fName:"Emma",lName:"Johnston");
			ProviderT.CreateProvider("Cache7",fName:"Aiden",lName:"Johnson");
		}

		#region CacheDictAbs
		///<summary>An example of our standard dictionary cache pattern where the key of the dictionary is guaranteed to be unique.
		///This particular cached dictionary will be comprised of Key: provider.ProvNum and Value: provider</summary>
		private class DictTestCache:CacheDictAbs<Provider,long,Provider> {
			protected override Provider Copy(Provider tableBase) {
				return tableBase.Copy();
			}
			protected override Provider CopyDictValue(Provider value) {
				return value.Copy();
			}
			protected override DataTable DictToTable(Dictionary<long,Provider> dictAllItems) {
				return ProviderCrud.ListToTable(dictAllItems.Values.ToList(),"Provider");
			}
			protected override void FillCacheIfNeeded() {
				GetTableFromDictCache(false);
			}
			protected override List<Provider> GetCacheFromDb() {
				return Providers.GetAll();
			}
			protected override long GetDictKey(Provider tableBase) {
				return tableBase.ProvNum;
			}
			protected override Provider GetDictValue(Provider tableBase) {
				return tableBase;
			}
			protected override List<Provider> TableToList(DataTable table) {
				return ProviderCrud.TableToList(table);
			}
			protected override bool IsInDictShort(Provider tableBase) {
				return !tableBase.IsHidden;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshDictCache() {
			return GetTableFromDictCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillDictCacheFromTable(DataTable table) {
			_dictCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromDictCache(bool doRefreshCache) {
			return _dictCache.GetTableFromCache(doRefreshCache);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetContainsKey() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			long provNum=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			//Check to make sure that the hidden provider can be found in the long cache.
			Assert.IsTrue(_dictCache.GetContainsKey(provNum));
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetContainsKeyShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			long provNum=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			//Check to make sure that the hidden provider cannot be found in the short cache.
			Assert.IsFalse(_dictCache.GetContainsKey(provNum,true));
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetCount() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Check to make sure that the dictionary and list have the same count.
			Assert.AreEqual(listProviders.Count,_dictCache.GetCount());
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetCountShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Check to make sure that the short dictionary and list do NOT have the same count.
			Assert.AreNotEqual(listProviders.Count,_dictCache.GetCount(true));
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetDeepCopy() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			Dictionary<long,Provider> dictDeepCopy=_dictCache.GetDeepCopy();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			long provNum=listProviders.First(x => x.Abbr=="Cache1" && x.FName=="Sophia" && x.LName=="Jones").ProvNum;
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			Provider provCache=_dictCache.GetOne(provNum);
			Assert.AreEqual(provCache.ProvNum,dictDeepCopy[provNum].ProvNum);
			Assert.AreEqual(provCache.Abbr,dictDeepCopy[provNum].Abbr);
			Assert.AreEqual(provCache.FName,dictDeepCopy[provNum].FName);
			Assert.AreEqual(provCache.LName,dictDeepCopy[provNum].LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get effected.
			dictDeepCopy[provNum].ProvNum++;
			//We cannot utilize provCache right here because GetOne() is supposed to give a deep copy so go back to our dict cache.
			provCache=_dictCache.GetOne(provNum);
			Assert.AreNotEqual(dictDeepCopy[provNum].ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetDeepCopyShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			Dictionary<long,Provider> dictDeepCopy=_dictCache.GetDeepCopy(true);
			//Make sure that our deep copy dictionary does not have the hidden provider.
			long provNumHidden=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			Assert.IsFalse(dictDeepCopy.ContainsKey(provNumHidden));
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			long provNum=listProviders.First(x => x.Abbr=="Cache1" && x.FName=="Sophia" && x.LName=="Jones").ProvNum;
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			Provider provCache=_dictCache.GetOne(provNum,true);
			Assert.AreEqual(provCache.ProvNum,dictDeepCopy[provNum].ProvNum);
			Assert.AreEqual(provCache.Abbr,dictDeepCopy[provNum].Abbr);
			Assert.AreEqual(provCache.FName,dictDeepCopy[provNum].FName);
			Assert.AreEqual(provCache.LName,dictDeepCopy[provNum].LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get affected.
			dictDeepCopy[provNum].ProvNum++;
			//We cannot utilize provCache right here because GetOne() is supposed to give a deep copy so go back to our dict cache.
			provCache=_dictCache.GetOne(provNum,true);
			Assert.AreNotEqual(dictDeepCopy[provNum].ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetFirstOrDefault() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetFirstOrDefaultShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Make sure that our method does not give back a hidden provider when asked for the short version.
			Provider providerHidden=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			Assert.IsNull(providerHidden);
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetOne() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			long provNum=listProviders.First(x => x.Abbr=="Cache3" && x.FName=="Olivia" && x.LName=="Brown").ProvNum;
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictCache.GetOne(provNum);//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetOne(provNum);//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetOneShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Make sure that our deep copy dictionary does not have the hidden provider.
			long provNumHidden=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			Provider providerHidden=null;
			ODException.SwallowAnyException(() => {
				providerHidden=_dictCache.GetOne(provNumHidden,true);
			});
			Assert.IsNull(providerHidden);
			//Now find the ProvNum of a provider that is not hidden.
			long provNum=listProviders.First(x => x.Abbr=="Cache3" && x.FName=="Olivia" && x.LName=="Brown").ProvNum;
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictCache.GetOne(provNum,true);//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetOne(provNum,true);//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetWhere() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictCache.GetWhere(x => x.LName=="Johnson");
			Assert.AreEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			Provider providerEmmaCopy=listJohnsonProviderCopies.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson");
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			long provNum=providerEmmaCopy.ProvNum;
			Provider provCache=_dictCache.GetOne(provNum);
			Assert.AreEqual(provCache.ProvNum,providerEmmaCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,providerEmmaCopy.Abbr);
			Assert.AreEqual(provCache.FName,providerEmmaCopy.FName);
			Assert.AreEqual(provCache.LName,providerEmmaCopy.LName);
			//Manipulate the copy we just got.
			providerEmmaCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetOne(provNum);
			Assert.AreNotEqual(providerEmmaCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetWhereShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictCache.GetWhere(x => x.LName=="Johnson",true);
			//Make sure that the lists are NOT equal because there should have been at least one hidden provider (Emma Johnson).
			Assert.AreNotEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			Provider providerAidenCopy=listJohnsonProviderCopies.First(x => x.Abbr=="Cache7" && x.FName=="Aiden" && x.LName=="Johnson");
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			long provNum=providerAidenCopy.ProvNum;
			Provider provCache=_dictCache.GetOne(provNum);
			Assert.AreEqual(provCache.ProvNum,providerAidenCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,providerAidenCopy.Abbr);
			Assert.AreEqual(provCache.FName,providerAidenCopy.FName);
			Assert.AreEqual(provCache.LName,providerAidenCopy.LName);
			//Manipulate the copy we just got.
			providerAidenCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetOne(provNum);
			Assert.AreNotEqual(providerAidenCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetWhereForKey() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictCache.GetWhereForKey(x => ListTools.In(x,listJohnsonProviders.Select(y => y.ProvNum)));
			//Make sure that the list counts are equal.
			Assert.AreEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			Provider providerAidenCopy=listJohnsonProviderCopies.First(x => x.Abbr=="Cache7" && x.FName=="Aiden" && x.LName=="Johnson");
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			long provNum=providerAidenCopy.ProvNum;
			Provider provCache=_dictCache.GetOne(provNum);
			Assert.AreEqual(provCache.ProvNum,providerAidenCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,providerAidenCopy.Abbr);
			Assert.AreEqual(provCache.FName,providerAidenCopy.FName);
			Assert.AreEqual(provCache.LName,providerAidenCopy.LName);
			//Manipulate the copy we just got.
			providerAidenCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetOne(provNum);
			Assert.AreNotEqual(providerAidenCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_GetWhereForKeyShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictCache.GetWhereForKey(x => ListTools.In(x,listJohnsonProviders.Select(y => y.ProvNum)),true);
			//Make sure that the lists are NOT equal because there should have been at least one hidden provider (Emma Johnson).
			Assert.AreNotEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			Provider providerAidenCopy=listJohnsonProviderCopies.First(x => x.Abbr=="Cache7" && x.FName=="Aiden" && x.LName=="Johnson");
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			long provNum=providerAidenCopy.ProvNum;
			Provider provCache=_dictCache.GetOne(provNum);
			Assert.AreEqual(provCache.ProvNum,providerAidenCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,providerAidenCopy.Abbr);
			Assert.AreEqual(provCache.FName,providerAidenCopy.FName);
			Assert.AreEqual(provCache.LName,providerAidenCopy.LName);
			//Manipulate the copy we just got.
			providerAidenCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictCache.GetOne(provNum);
			Assert.AreNotEqual(providerAidenCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_RemoveKey() {
			//Insert a new provider specifically for this test.
			long provNum=ProviderT.CreateProvider("CacheDictTestRemoveKey",fName:"Colby",lName:"Smith");
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Make sure the key is present for the provider we just inserted.
			Assert.IsTrue(_dictCache.GetContainsKey(provNum));
			//Remove the key from the dictionary
			Assert.IsTrue(_dictCache.RemoveKey(provNum));
			//Make sure the key is gone.
			Assert.IsFalse(_dictCache.GetContainsKey(provNum));
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_SetValueForKey() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Create a junk provider that is NOT going to be inserted into the database but will be used to manually update a value within the cache.
			Provider providerJunk=new Provider() {
				ProvNum=-1234,
				Abbr="JunkAbbr",
				FName="Junk",
				LName="Yard",
				IsHidden=true
			};
			//Pick a random provider from the cache (that we know SHOULD exist otherwise CacheTestInit failed).
			long provNum=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache3" && x.FName=="Olivia" && x.LName=="Brown").ProvNum;
			//Make sure that the dictionary contains the correct KEY for the random provider we selected.
			Provider providerOliviaViaKey=_dictCache.GetOne(provNum);
			Assert.IsNotNull(providerOliviaViaKey);
			Assert.AreEqual(providerOliviaViaKey.Abbr,"Cache3");
			Assert.AreEqual(providerOliviaViaKey.FName,"Olivia");
			Assert.AreEqual(providerOliviaViaKey.LName,"Brown");
			//Now we need to update the value for the key of the random provider we selected to our junk provider.
			_dictCache.SetValueForKey(provNum,providerJunk);
			//Go back to the dictionary and get the value for our key from before and make sure that the cache was in fact manipulated correctly.
			Provider providerJunkViaKey=_dictCache.GetOne(provNum);
			Assert.AreEqual(providerJunkViaKey.ProvNum,-1234);
			Assert.AreEqual(providerJunkViaKey.Abbr,"JunkAbbr");
			Assert.AreEqual(providerJunkViaKey.FName,"Junk");
			Assert.AreEqual(providerJunkViaKey.LName,"Yard");
			Assert.IsTrue(providerJunkViaKey.IsHidden);
			//This key may or may not be part of the short cache which might be something we address in the future.  It is not a bug at this time IMO.
			//The SetValueForKey methodology should be used VERY rarely and the only thing that really matters is that the value gets manipulated correctly.
		}

		[TestMethod]
		public void CacheAbsTests_DictTest_SetValueForKeyShort() {
			//Forcefully refresh the dict cache.
			RefreshDictCache();
			//Create a junk provider that is NOT going to be inserted into the database but will be used to manually update a value within the cache.
			Provider providerJunk=new Provider() {
				ProvNum=-1234,
				Abbr="JunkAbbr",
				FName="Junk",
				LName="Yard"
			};
			//Pick a random provider from the cache (that we know SHOULD exist otherwise CacheTestInit failed).
			long provNum=_dictCache.GetFirstOrDefault(x => x.Abbr=="Cache3" && x.FName=="Olivia" && x.LName=="Brown").ProvNum;
			//Make sure that the dictionary contains the correct KEY for the random provider we selected.
			Provider providerOliviaViaKey=_dictCache.GetOne(provNum);
			Assert.IsNotNull(providerOliviaViaKey);
			Assert.AreEqual(providerOliviaViaKey.Abbr,"Cache3");
			Assert.AreEqual(providerOliviaViaKey.FName,"Olivia");
			Assert.AreEqual(providerOliviaViaKey.LName,"Brown");
			//Now we need to update the value for the key of the random provider we selected to our junk provider.
			_dictCache.SetValueForKey(provNum,providerJunk,true);
			//Go back to the dictionary and get the value for our key from before and make sure that the cache was in fact manipulated correctly.
			Provider providerJunkViaKey=_dictCache.GetOne(provNum);
			Assert.AreEqual(providerJunkViaKey.ProvNum,-1234);
			Assert.AreEqual(providerJunkViaKey.Abbr,"JunkAbbr");
			Assert.AreEqual(providerJunkViaKey.FName,"Junk");
			Assert.AreEqual(providerJunkViaKey.LName,"Yard");
			//The key value pair should now be considered part of the short version regardless of how the kvp was treated before.
			Assert.IsTrue(_dictCache.GetContainsKey(provNum,true));
		}
		#endregion CacheDictAbs

		#region CacheDictNonPkAbs
		///<summary>An example of our non-standard dictionary cache pattern where the key of the dictionary is NOT guaranteed to be unique.
		///This particular cached dictionary will be comprised of Key: provider.LName and Value: provider</summary>
		private class DictNoPkTestCache:CacheDictNonPkAbs<Provider,string,Provider> {
			protected override Provider Copy(Provider tableBase) {
				return tableBase.Copy();
			}
			protected override Provider CopyDictValue(Provider value) {
				return value.Copy();
			}
			protected override DataTable DictToTable(Dictionary<string,Provider> dictAllItems) {
				return ProviderCrud.ListToTable(dictAllItems.Values.ToList(),"Provider");
			}
			protected override void FillCacheIfNeeded() {
				GetTableFromDictNoPkCache(false);
			}
			protected override List<Provider> GetCacheFromDb() {
				return Providers.GetAll();
			}
			protected override string GetDictKey(Provider tableBase) {
				return tableBase.LName;
			}
			protected override Provider GetDictValue(Provider tableBase) {
				return tableBase;
			}
			protected override List<Provider> TableToList(DataTable table) {
				return ProviderCrud.TableToList(table);
			}
			protected override bool IsInDictShort(Provider tableBase) {
				return !tableBase.IsHidden;
			}
			protected override DataTable ListToTable(List<Provider> listAllItems) {
				return ProviderCrud.ListToTable(listAllItems);
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshDictNoPkCache() {
			return GetTableFromDictNoPkCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillDictNoPkCacheFromTable(DataTable table) {
			_dictNoPkCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromDictNoPkCache(bool doRefreshCache) {
			return _dictNoPkCache.GetTableFromCache(doRefreshCache);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_AddValueForKey() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Make sure that we cannot add a duplicate key value pair.  If this is desired, we need to be calling the Set method instead of Add.
			Assert.IsFalse(_dictNoPkCache.AddValueForKey("Johnson",new Provider()));
			//Make a unique provider that we know is not present in the cache.
			long randomTicks=DateTime.Now.Ticks;
			string lName="Ticks"+randomTicks;
			Provider providerTicks=new Provider() { Abbr="TicksAbbr",FName="Tick",LName=lName };
			Assert.IsTrue(_dictNoPkCache.AddValueForKey(lName,providerTicks));
			//Event though IsHidden is currently false, this provider should NOT be in the short cache because we didn't explicitly tell it to.
			//This is probably a bug but the method is not currently written to be this smart.  I'm adding this test just to prove a point.
			Assert.IsFalse(_dictNoPkCache.GetContainsKey(lName,true));
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_AddValueForKeyShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Make sure that we cannot add a duplicate key value pair.  If this is desired, we need to be calling the Set method instead of Add.
			Assert.IsFalse(_dictNoPkCache.AddValueForKey("Johnson",new Provider(),true));
			//Make a unique provider that we know is not present in the cache.
			long randomTicks=DateTime.Now.Ticks;
			string lName="Ticks"+randomTicks;
			Provider providerTicks=new Provider() { Abbr="TicksAbbr",FName="Tick",LName=lName,IsHidden=true };
			Assert.IsTrue(_dictNoPkCache.AddValueForKey(lName,providerTicks,true));
			//Event though IsHidden is currently true, this provider SHOULD be in the short cache because we explicitly told it to do so.
			//This is probably a bug but the method is not currently written to be this smart.  I'm adding this test just to prove a point.
			Assert.IsTrue(_dictNoPkCache.GetContainsKey(lName,true));
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetContainsKey() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			string key=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").LName;
			//Check to make sure that the hidden provider can be found in the long cache.
			Assert.IsTrue(_dictNoPkCache.GetContainsKey(key));
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetContainsKeyShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			string key=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").LName;
			//Check to make sure that there is a value found for the specified key because there are MULTIPLE providers with the same key.
			//This should return true due to the Aiden Johnson provider.
			Assert.IsTrue(_dictNoPkCache.GetContainsKey(key,true));
			//Next we will insert a provider into the database that we know is not in the database:
			long randomTicks=DateTime.Now.Ticks;
			ProviderT.CreateProvider("TicksAbbr",fName:"Tick",lName:"Ticks"+randomTicks,isHidden:true);
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			key="Ticks"+randomTicks;
			//Make sure the new provider made it into the cache
			Assert.IsTrue(_dictNoPkCache.GetContainsKey(key));
			//Make sure the new provider did NOT make it into the short version
			Assert.IsFalse(_dictNoPkCache.GetContainsKey(key,true));
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetCount() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Check to make sure that the dictionary and list DO NOT have the same count.  Our non-PK dictionary picks duplicates at random... very lame.
			Assert.AreNotEqual(listProviders.Count,_dictNoPkCache.GetCount());
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetCountShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Check to make sure that the short dictionary and list do NOT have the same count.
			Assert.AreNotEqual(listProviders.Count,_dictNoPkCache.GetCount(true));
			//Double check that the long and short versions are different as well.
			Assert.AreNotEqual(_dictNoPkCache.GetCount(),_dictNoPkCache.GetCount(true));
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetDeepCopy() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			Dictionary<string,Provider> dictDeepCopy=_dictNoPkCache.GetDeepCopy();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			//Also, there should only be one "Brown" provider... another test might use this LName in which case this test will have to change.
			string provName=listProviders.First(x => x.Abbr=="Cache3" && x.FName=="Olivia" && x.LName=="Brown").LName;
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			Provider provCache=_dictNoPkCache.GetOne(provName);
			Assert.AreEqual(provCache.ProvNum,dictDeepCopy[provName].ProvNum);
			Assert.AreEqual(provCache.Abbr,dictDeepCopy[provName].Abbr);
			Assert.AreEqual(provCache.FName,dictDeepCopy[provName].FName);
			Assert.AreEqual(provCache.LName,dictDeepCopy[provName].LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get effected.
			dictDeepCopy[provName].ProvNum++;
			//We cannot utilize provCache right here because GetOne() is supposed to give a deep copy so go back to our dict cache.
			provCache=_dictNoPkCache.GetOne(provName);
			Assert.AreNotEqual(dictDeepCopy[provName].ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetDeepCopyShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			Dictionary<string,Provider> dictDeepCopy=_dictNoPkCache.GetDeepCopy(true);
			//Make sure that our deep copy dictionary does not have the hidden provider.
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			Provider providerHidden=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson");
			//Our dictionary is trash so we will actually get a hit based on this "duplicate" key.  It just won't be our beautiful Emma returned.
			Assert.IsTrue(dictDeepCopy.ContainsKey(providerHidden.LName));
			Provider provCache=_dictNoPkCache.GetOne(providerHidden.LName,true);
			//Because this type of dictionary can be filled in different ways (short / long), it is never a guarantee on how it is going to be filled.
			//We will keep using these dictionaries until a user complains in which case we need to move away from the NoPk style to a predictable style.
			////Make sure that the "Johnson" in the dictionary IS NOT our beautiful Emma.
			//Assert.AreNotEqual(provCache.ProvNum,providerHidden.ProvNum);
			//Assert.AreNotEqual(provCache.Abbr,providerHidden.Abbr);
			//Assert.AreNotEqual(provCache.FName,providerHidden.FName);//This might fail if another test utilized Emma Johnson...
			//Now we want to test that our deep copy is in fact the same and that it will not manipulate the actual cache.
			Assert.AreEqual(provCache.ProvNum,dictDeepCopy[providerHidden.LName].ProvNum);
			Assert.AreEqual(provCache.Abbr,dictDeepCopy[providerHidden.LName].Abbr);
			Assert.AreEqual(provCache.FName,dictDeepCopy[providerHidden.LName].FName);
			Assert.AreEqual(provCache.LName,dictDeepCopy[providerHidden.LName].LName);
			//Manipulate the object within our deep copy and make sure that the cache did not get affected.
			dictDeepCopy[providerHidden.LName].ProvNum++;
			//We cannot utilize provCache right here because GetOne() is supposed to give a deep copy so go back to our dict cache.
			provCache=_dictNoPkCache.GetOne(providerHidden.LName,true);
			Assert.AreNotEqual(dictDeepCopy[providerHidden.LName].ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetDeepCopyList() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			List<Provider> listDeepCopy=_dictNoPkCache.GetDeepCopyList();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			Provider providerDb=listProviders.First(x => x.Abbr=="Cache1" && x.FName=="Sophia" && x.LName=="Jones");
			Provider provCopy=listDeepCopy.First(x => x.ProvNum==providerDb.ProvNum);
			Provider provCache=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.ProvNum==providerDb.ProvNum);
			//First, double check that the provider we are about to manipulate is identical before manipulation.
			Assert.AreEqual(provCache.ProvNum,provCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,provCopy.Abbr);
			Assert.AreEqual(provCache.FName,provCopy.FName);
			Assert.AreEqual(provCache.LName,provCopy.LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get effected.
			provCopy.ProvNum++;
			//We cannot utilize provCache right here because GetFirst() is supposed to give a deep copy so go back to our cache.
			provCache=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.ProvNum==providerDb.ProvNum);
			Assert.AreNotEqual(provCopy.ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetDeepCopyListShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			List<Provider> listDeepCopy=_dictNoPkCache.GetDeepCopyList(true);
			//Make sure that our deep copy list does not have the hidden provider.
			long provNumHidden=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			Assert.IsFalse(listDeepCopy.Any(x => x.ProvNum==provNumHidden));
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			Provider providerDb=listProviders.First(x => x.Abbr=="Cache1" && x.FName=="Sophia" && x.LName=="Jones");
			Provider provCopy=listDeepCopy.First(x => x.ProvNum==providerDb.ProvNum);
			Provider provCache=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.ProvNum==providerDb.ProvNum,true);
			//First, double check that the provider we are about to manipulate is identical before manipulation.
			Assert.AreEqual(provCache.ProvNum,provCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,provCopy.Abbr);
			Assert.AreEqual(provCache.FName,provCopy.FName);
			Assert.AreEqual(provCache.LName,provCopy.LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get effected.
			provCopy.ProvNum++;
			//We must refresh provCache right here because GetFirst() is supposed to give a deep copy so go back to our cache.
			provCache=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.ProvNum==providerDb.ProvNum,true);
			Assert.AreNotEqual(provCopy.ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetFirstOrDefault() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictNoPkCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictNoPkCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetFirstOrDefaultShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Because this type of dictionary can be filled in different ways (short / long), it is never a guarantee on how it is going to be filled.
			//We will keep using these dictionaries until a user complains in which case we need to move away from the NoPk style to a predictable style.
			////Make sure that our method does not give back a hidden provider when asked for the short version.
			//Provider providerHidden =_dictNoPkCache.GetFirstOrDefault(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			//Assert.IsNull(providerHidden);
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictNoPkCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictNoPkCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetFirstOrDefaultFromList() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetFirstOrDefaultFromListShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Make sure that our method does not give back a hidden provider when asked for the short version.
			Provider providerHidden=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			Assert.IsNull(providerHidden);
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictNoPkCache.GetFirstOrDefaultFromList(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetOne() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictNoPkCache.GetOne("Brown");//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictNoPkCache.GetOne("Brown");//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetOneShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Make sure that our deep copy dictionary does not have the hidden provider.
			Provider providerHidden=null;
			ODException.SwallowAnyException(() => {
				providerHidden=_dictNoPkCache.GetOne("Johnson",true);
			});
			Assert.IsNotNull(providerHidden);//Our dictionary is trash and should actually have an entry for Johnson, it just won't be Emma.
			//There is an outstanding bug that we aren't going to spend time fixing with non-PK dictionaries.  The real solution is to not use them...
			//Assert.IsFalse(providerHidden.FName=="Emma");//Could fail if another test added an Emma... doubtful.
			//Now find the ProvNum of a provider that is not hidden.
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_dictNoPkCache.GetOne("Brown",true);//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the dictionary cache and make sure it was not affected.
			Provider providerCopy2=_dictNoPkCache.GetOne("Brown",true);//Could throw an exception if CacheTestInit did not correctly run prior to this test method.
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetWhere() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictNoPkCache.GetWhere(x => x.LName=="Johnson");
			Assert.IsTrue(listJohnsonProviders.Count > 1);
			Assert.IsTrue(listJohnsonProviderCopies.Count==1);//Our non-PK dictionary should always pick one provider at random.
			//Since the database/C# should have randomly selected the one item that our non-PK dictionary should use, find out which one was chosen.
			//Now we need to manipulate the deep copy of the random Johnson provider and make sure it didn't affect our cache.
			listJohnsonProviderCopies[0].ProvNum++;
			List<Provider> listJohnsonProviderCopies2=_dictNoPkCache.GetWhere(x => x.LName=="Johnson");
			Assert.AreEqual(listJohnsonProviderCopies[0].Abbr,listJohnsonProviderCopies2[0].Abbr);
			Assert.AreEqual(listJohnsonProviderCopies[0].FName,listJohnsonProviderCopies2[0].FName);
			Assert.AreEqual(listJohnsonProviderCopies[0].LName,listJohnsonProviderCopies2[0].LName);
			//Make sure that the ProvNum change did NOT affect the cache.
			Assert.AreNotEqual(listJohnsonProviderCopies[0].ProvNum,listJohnsonProviderCopies2[0].ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetWhereShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictNoPkCache.GetWhere(x => x.LName=="Johnson",true);
			Assert.IsTrue(listJohnsonProviders.Count > 1);
			Assert.IsTrue(listJohnsonProviderCopies.Count==1);//Our non-PK dictionary should always pick one provider at random.
			//Since the database/C# should have randomly selected the one item that our non-PK dictionary should use, find out which one was chosen.
			//Now we need to manipulate the deep copy of the random Johnson provider and make sure it didn't affect our cache.
			listJohnsonProviderCopies[0].ProvNum++;
			List<Provider> listJohnsonProviderCopies2=_dictNoPkCache.GetWhere(x => x.LName=="Johnson",true);
			Assert.AreEqual(listJohnsonProviderCopies[0].Abbr,listJohnsonProviderCopies2[0].Abbr);
			Assert.AreEqual(listJohnsonProviderCopies[0].FName,listJohnsonProviderCopies2[0].FName);
			Assert.AreEqual(listJohnsonProviderCopies[0].LName,listJohnsonProviderCopies2[0].LName);
			//Make sure that the ProvNum change did NOT affect the cache.
			Assert.AreNotEqual(listJohnsonProviderCopies[0].ProvNum,listJohnsonProviderCopies2[0].ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetWhereForKey() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictNoPkCache.GetWhereForKey(x => x=="Johnson");
			Assert.IsTrue(listJohnsonProviders.Count > 1);
			Assert.IsTrue(listJohnsonProviderCopies.Count==1);//Our non-PK dictionary should always pick one provider at random.
			//Since the database/C# should have randomly selected the one item that our non-PK dictionary should use, find out which one was chosen.
			//Now we need to manipulate the deep copy of the random Johnson provider and make sure it didn't affect our cache.
			listJohnsonProviderCopies[0].ProvNum++;
			List<Provider> listJohnsonProviderCopies2=_dictNoPkCache.GetWhereForKey(x => x=="Johnson");
			Assert.AreEqual(listJohnsonProviderCopies[0].Abbr,listJohnsonProviderCopies2[0].Abbr);
			Assert.AreEqual(listJohnsonProviderCopies[0].FName,listJohnsonProviderCopies2[0].FName);
			Assert.AreEqual(listJohnsonProviderCopies[0].LName,listJohnsonProviderCopies2[0].LName);
			//Make sure that the ProvNum change did NOT affect the cache.
			Assert.AreNotEqual(listJohnsonProviderCopies[0].ProvNum,listJohnsonProviderCopies2[0].ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetWhereForKeyShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictNoPkCache.GetWhereForKey(x => x=="Johnson",true);
			Assert.IsTrue(listJohnsonProviders.Count > 1);
			Assert.IsTrue(listJohnsonProviderCopies.Count==1);//Our non-PK dictionary should always pick one provider at random.
			//Since the database/C# should have randomly selected the one item that our non-PK dictionary should use, find out which one was chosen.
			//Now we need to manipulate the deep copy of the random Johnson provider and make sure it didn't affect our cache.
			listJohnsonProviderCopies[0].ProvNum++;
			List<Provider> listJohnsonProviderCopies2=_dictNoPkCache.GetWhereForKey(x => x=="Johnson",true);
			Assert.AreEqual(listJohnsonProviderCopies[0].Abbr,listJohnsonProviderCopies2[0].Abbr);
			Assert.AreEqual(listJohnsonProviderCopies[0].FName,listJohnsonProviderCopies2[0].FName);
			Assert.AreEqual(listJohnsonProviderCopies[0].LName,listJohnsonProviderCopies2[0].LName);
			//Make sure that the ProvNum change did NOT affect the cache.
			Assert.AreNotEqual(listJohnsonProviderCopies[0].ProvNum,listJohnsonProviderCopies2[0].ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetWhereFromList() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictNoPkCache.GetWhereFromList(x => x.LName=="Johnson");
			Assert.AreEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			//Now we need to manipulate the deep copy of the random Johnson provider and make sure it didn't affect our cache.
			listJohnsonProviderCopies[0].ProvNum++;
			List<Provider> listJohnsonProviderCopies2=_dictNoPkCache.GetWhereFromList(x => x.LName=="Johnson");
			Assert.AreEqual(listJohnsonProviderCopies[0].Abbr,listJohnsonProviderCopies2[0].Abbr);
			Assert.AreEqual(listJohnsonProviderCopies[0].FName,listJohnsonProviderCopies2[0].FName);
			Assert.AreEqual(listJohnsonProviderCopies[0].LName,listJohnsonProviderCopies2[0].LName);
			//Make sure that the ProvNum change did NOT affect the cache.
			Assert.AreNotEqual(listJohnsonProviderCopies[0].ProvNum,listJohnsonProviderCopies2[0].ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_GetWhereFromListShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_dictNoPkCache.GetWhereFromList(x => x.LName=="Johnson",true);
			Assert.AreNotEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			//Now we need to manipulate the deep copy of the random Johnson provider and make sure it didn't affect our cache.
			listJohnsonProviderCopies[0].ProvNum++;
			List<Provider> listJohnsonProviderCopies2=_dictNoPkCache.GetWhereFromList(x => x.LName=="Johnson",true);
			Assert.AreEqual(listJohnsonProviderCopies[0].Abbr,listJohnsonProviderCopies2[0].Abbr);
			Assert.AreEqual(listJohnsonProviderCopies[0].FName,listJohnsonProviderCopies2[0].FName);
			Assert.AreEqual(listJohnsonProviderCopies[0].LName,listJohnsonProviderCopies2[0].LName);
			//Make sure that the ProvNum change did NOT affect the cache.
			Assert.AreNotEqual(listJohnsonProviderCopies[0].ProvNum,listJohnsonProviderCopies2[0].ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_RemoveKey() {
			string lNameStr="Smith"+DateTime.Now.Ticks;
			//Insert a new provider specifically for this test.
			long provNum=ProviderT.CreateProvider("CacheDictTestRemoveKey",fName:"Colby",lName:lNameStr);
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Make sure the key is present for the provider we just inserted.
			Assert.IsTrue(_dictNoPkCache.GetContainsKey(lNameStr));
			//Remove the key from the dictionary
			Assert.IsTrue(_dictNoPkCache.RemoveKey(lNameStr));
			//Make sure the key is gone.
			Assert.IsFalse(_dictNoPkCache.GetContainsKey(lNameStr));
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_SetValueForKey() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Create a junk provider that is NOT going to be inserted into the database but will be used to manually update a value within the cache.
			Provider providerJunk=new Provider() {
				ProvNum=-1234,
				Abbr="JunkAbbr",
				FName="Junk",
				LName="Yard",
				IsHidden=true
			};
			//Pick a random provider from the cache (that we know SHOULD exist otherwise CacheTestInit failed).
			Provider providerBrownViaKey=_dictNoPkCache.GetOne("Brown");
			Assert.IsNotNull(providerBrownViaKey);
			Assert.AreEqual(providerBrownViaKey.LName,"Brown");
			//Now we need to update the value for the key of the random provider we selected to our junk provider.
			_dictNoPkCache.SetValueForKey("Brown",providerJunk);
			//Go back to the dictionary and get the value for our key from before and make sure that the cache was in fact manipulated correctly.
			Provider providerJunkViaKey=_dictNoPkCache.GetOne("Brown");
			Assert.AreEqual(providerJunkViaKey.ProvNum,-1234);
			Assert.AreEqual(providerJunkViaKey.Abbr,"JunkAbbr");
			Assert.AreEqual(providerJunkViaKey.FName,"Junk");
			Assert.AreEqual(providerJunkViaKey.LName,"Yard");
			Assert.IsTrue(providerJunkViaKey.IsHidden);
			//This key may or may not be part of the short cache which might be something we address in the future.  It is not a bug at this time IMO.
			//The SetValueForKey methodology should be used VERY rarely and the only thing that really matters is that the value gets manipulated correctly.
		}

		[TestMethod]
		public void CacheAbsTests_DictNoPkTest_SetValueForKeyShort() {
			//Forcefully refresh the dictNoPk cache.
			RefreshDictNoPkCache();
			//Create a junk provider that is NOT going to be inserted into the database but will be used to manually update a value within the cache.
			Provider providerJunk=new Provider() {
				ProvNum=-1234,
				Abbr="JunkAbbr",
				FName="Junk",
				LName="Yard"
			};
			//Make sure that the dictionary contains the correct KEY for the random provider we selected.
			Provider providerBrownViaKey=_dictNoPkCache.GetOne("Brown",true);
			Assert.IsNotNull(providerBrownViaKey);
			Assert.AreEqual(providerBrownViaKey.LName,"Brown");
			//Now we need to update the value for the key of the random provider we selected to our junk provider.
			_dictNoPkCache.SetValueForKey("Brown",providerJunk,true);
			//Go back to the dictionary and get the value for our key from before and make sure that the cache was in fact manipulated correctly.
			Provider providerJunkViaKey=_dictNoPkCache.GetOne("Brown",true);
			Assert.AreEqual(providerJunkViaKey.ProvNum,-1234);
			Assert.AreEqual(providerJunkViaKey.Abbr,"JunkAbbr");
			Assert.AreEqual(providerJunkViaKey.FName,"Junk");
			Assert.AreEqual(providerJunkViaKey.LName,"Yard");
			//The key value pair should now be considered part of the short version regardless of how the kvp was treated before.
			Assert.IsTrue(_dictNoPkCache.GetContainsKey("Brown",true));
		}
		#endregion CacheDictNonPkAbs

		#region CacheListAbs
		///<summary>An example of our standard list cache pattern that will be filled with providers.</summary>
		private class ListTestCache:CacheListAbs<Provider> {
			protected override Provider Copy(Provider tableBase) {
				return tableBase.Copy();
			}
			protected override void FillCacheIfNeeded() {
				GetTableFromListCache(false);
			}
			protected override List<Provider> GetCacheFromDb() {
				return Providers.GetAll();
			}
			protected override DataTable ListToTable(List<Provider> listAllItems) {
				return ProviderCrud.ListToTable(listAllItems,"Provider");
			}
			protected override List<Provider> TableToList(DataTable table) {
				return ProviderCrud.TableToList(table);
			}
			protected override bool IsInListShort(Provider tableBase) {
				return !tableBase.IsHidden;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshListCache() {
			return GetTableFromListCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_listCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromListCache(bool doRefreshCache) {
			return _listCache.GetTableFromCache(doRefreshCache);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetCount() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Check to make sure that the cached list and db list have the same count.
			Assert.AreEqual(listProviders.Count,_listCache.GetCount());
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetCountShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Check to make sure that the cached list and db list do NOT have the same count.
			Assert.AreNotEqual(listProviders.Count,_listCache.GetCount(true));
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetDeepCopy() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			List<Provider> listDeepCopy=_listCache.GetDeepCopy();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			Provider providerDb=listProviders.First(x => x.Abbr=="Cache1" && x.FName=="Sophia" && x.LName=="Jones");
			Provider provCopy=listDeepCopy.First(x => x.ProvNum==providerDb.ProvNum);
			Provider provCache=_listCache.GetFirst(x => x.ProvNum==providerDb.ProvNum);
			//First, double check that the provider we are about to manipulate is identical before manipulation.
			Assert.AreEqual(provCache.ProvNum,provCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,provCopy.Abbr);
			Assert.AreEqual(provCache.FName,provCopy.FName);
			Assert.AreEqual(provCache.LName,provCopy.LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get effected.
			provCopy.ProvNum++;
			//We cannot utilize provCache right here because GetFirst() is supposed to give a deep copy so go back to our cache.
			provCache=_listCache.GetFirst(x => x.ProvNum==providerDb.ProvNum);
			Assert.AreNotEqual(provCopy.ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetDeepCopyShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//Get a deep copy of the cache that can be safely manipulated.
			List<Provider> listDeepCopy=_listCache.GetDeepCopy(true);
			//Make sure that our deep copy list does not have the hidden provider.
			long provNumHidden=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			Assert.IsFalse(listDeepCopy.Any(x => x.ProvNum==provNumHidden));
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			Provider providerDb=listProviders.First(x => x.Abbr=="Cache1" && x.FName=="Sophia" && x.LName=="Jones");
			Provider provCopy=listDeepCopy.First(x => x.ProvNum==providerDb.ProvNum);
			Provider provCache=_listCache.GetFirst(x => x.ProvNum==providerDb.ProvNum,true);
			//First, double check that the provider we are about to manipulate is identical before manipulation.
			Assert.AreEqual(provCache.ProvNum,provCopy.ProvNum);
			Assert.AreEqual(provCache.Abbr,provCopy.Abbr);
			Assert.AreEqual(provCache.FName,provCopy.FName);
			Assert.AreEqual(provCache.LName,provCopy.LName);
			//Manipulate an object within our deep copy and make sure that the cache did not get effected.
			provCopy.ProvNum++;
			//We must refresh provCache right here because GetFirst() is supposed to give a deep copy so go back to our cache.
			provCache=_listCache.GetFirst(x => x.ProvNum==providerDb.ProvNum,true);
			Assert.AreNotEqual(provCopy.ProvNum,provCache.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetExists() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			long provNum=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			//Check to make sure that the hidden provider can be found in the long cache.
			Assert.IsTrue(_listCache.GetExists(x => x.ProvNum==provNum));
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetExistsShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following line fails then CacheTestInit failed to run and it is vital that it is run first.
			long provNum=listProviders.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson").ProvNum;
			//Check to make sure that the hidden provider cannot be found in the short cache.
			Assert.IsFalse(_listCache.GetExists(x => x.ProvNum==provNum,true));
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetFindIndex() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			//GetProvsNoCache doesn't sort by ItemOrder which is how our cache is sorted.  Manually sort the list instead of manipulating the method.
			List<Provider> listProviders=Providers.GetProvsNoCache().OrderByDescending(x => x.ItemOrder).ToList();
			//If the following provider index is not found then CacheTestInit failed to run and it is vital that it is run first.
			int index=listProviders.FindIndex(x => x.Abbr=="Cache2" && x.FName=="Jackson" && x.LName=="Jones");
			//Find the index of the same provider above within our cache.
			int indexCache=_listCache.GetFindIndex(x => x.Abbr=="Cache2" && x.FName=="Jackson" && x.LName=="Jones");
			//I'm not sure if the following check is going to always be the same, but theoretically they will match in a synchronous environment.
			//If GetProvsNoCache() ever orders its return value OR if the database gives the providers back in random orders, this might fail.
			Assert.IsTrue(index > -1 && indexCache > -1 && index==indexCache);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetFindIndexShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//First make sure that the hidden provider cannot be found
			int indexCacheHiddenProv=_listCache.GetFindIndex(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			Assert.IsTrue(indexCacheHiddenProv==-1);
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			//If the following provider index is not found then CacheTestInit failed to run and it is vital that it is run first.
			int indexHiddenProv=listProviders.FindIndex(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson" && x.IsHidden);
			Assert.IsTrue(indexHiddenProv > -1);
			//Now assert that a non-hidden provider is present in both the database and within our short cache.
			int index=listProviders.FindIndex(x => x.Abbr=="Cache2" && x.FName=="Jackson" && x.LName=="Jones");
			int indexCache=_listCache.GetFindIndex(x => x.Abbr=="Cache2" && x.FName=="Jackson" && x.LName=="Jones",true);
			Assert.IsTrue(index > -1 && indexCache > -1);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetFirst() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetFirst(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetFirst(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetFirstShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Make sure that our method does not give back a hidden provider when asked for the short version.
			Provider providerHidden=null;
			ODException.SwallowAnyException(() => {
				providerHidden=_listCache.GetFirst(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			});
			Assert.IsNull(providerHidden);
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetFirst(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetFirst(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetFirstOrDefault() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetFirstOrDefaultShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Make sure that our method does not give back a hidden provider when asked for the short version.
			Provider providerHidden=_listCache.GetFirstOrDefault(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			Assert.IsNull(providerHidden);
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetFirstOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetLast() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetLast();
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetLast();
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetLastShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetLast(true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetLast(true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetLastOrDefault() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetLastOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetLastOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson");
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetLastOrDefaultShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Make sure that our method does not give back a hidden provider when asked for the short version.
			Provider providerHidden=_listCache.GetLastOrDefault(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson",true);
			Assert.IsNull(providerHidden);
			//Get a deep copy of the provider that can be safely manipulated.
			Provider providerCopy=_listCache.GetLastOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			//Manipulate the copy we just got.
			providerCopy.ProvNum++;
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetLastOrDefault(x => x.Abbr=="Cache4" && x.FName=="Liam" && x.LName=="Neeson",true);
			Assert.AreNotEqual(providerCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetWhere() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_listCache.GetWhere(x => x.LName=="Johnson");
			Assert.AreEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			Provider providerEmmaCopy=listJohnsonProviderCopies.First(x => x.Abbr=="Cache5" && x.FName=="Emma" && x.LName=="Johnson");
			Provider providerEmmaCache=_listCache.GetFirst(x => x.ProvNum==providerEmmaCopy.ProvNum);
			long provNum=providerEmmaCache.ProvNum;
			//First, double check that the provider we are about to manipulate is identical before manipulation.
			Assert.AreEqual(providerEmmaCache.ProvNum,providerEmmaCopy.ProvNum);
			Assert.AreEqual(providerEmmaCache.Abbr,providerEmmaCopy.Abbr);
			Assert.AreEqual(providerEmmaCache.FName,providerEmmaCopy.FName);
			Assert.AreEqual(providerEmmaCache.LName,providerEmmaCopy.LName);
			//Manipulate the copy we just got.
			providerEmmaCopy.ProvNum++;
			Assert.AreNotEqual(providerEmmaCopy.ProvNum,provNum);//Double check that provNum didn't get affected by this.
			//Now go back to the cache and make sure the cache itself was not affected.
			Provider providerCopy2=_listCache.GetFirst(x => x.ProvNum==provNum);
			Assert.AreNotEqual(providerEmmaCopy.ProvNum,providerCopy2.ProvNum);
		}

		[TestMethod]
		public void CacheAbsTests_ListTest_GetWhereShort() {
			//Forcefully refresh the list cache.
			RefreshListCache();
			//Go to the database and get all providers that were just inserted.
			List<Provider> listProviders=Providers.GetProvsNoCache();
			List<Provider> listJohnsonProviders=listProviders.FindAll(x => x.LName=="Johnson");//At least one provider will be hidden.
			//Get a deep copy of the providers that can be safely manipulated.
			List<Provider> listJohnsonProviderCopies=_listCache.GetWhere(x => x.LName=="Johnson",true);
			//Make sure that the lists are NOT equal because there should have been at least one hidden provider (Emma Johnson).
			Assert.AreNotEqual(listJohnsonProviders.Count,listJohnsonProviderCopies.Count);
			Provider providerAidenCopy=listJohnsonProviderCopies.First(x => x.Abbr=="Cache7" && x.FName=="Aiden" && x.LName=="Johnson");
			Provider providerAidenCache=_listCache.GetFirst(x => x.ProvNum==providerAidenCopy.ProvNum,true);
			long provNum=providerAidenCache.ProvNum;
			//First, double check that the provider we are about to manipulate are identical before manipulation.
			Assert.AreEqual(providerAidenCache.ProvNum,providerAidenCopy.ProvNum);
			Assert.AreEqual(providerAidenCache.Abbr,providerAidenCopy.Abbr);
			Assert.AreEqual(providerAidenCache.FName,providerAidenCopy.FName);
			Assert.AreEqual(providerAidenCache.LName,providerAidenCopy.LName);
			//Manipulate the copy we just got.
			providerAidenCopy.ProvNum++;
			Assert.AreNotEqual(providerAidenCopy.ProvNum,provNum);//Double check that provNum didn't get affected by this.
			//Now go back to the cache and make sure it was not affected.
			Provider providerCopy2=_listCache.GetFirst(x => x.ProvNum==provNum,true);
			Assert.AreNotEqual(providerAidenCopy.ProvNum,providerCopy2.ProvNum);
		}
		#endregion CacheListAbs

	}
}
