using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Introspection_Tests {
	[TestClass]
	public class IntrospectionTests:TestBase {
		private readonly string _defaultUrl="thisIsADefaultURL";
		private readonly List<string> _wikiPrefs=new List<string>(){"DentalXChangeDwsURL","DentalXChangeDeaURL","DoseSpotURL","PayConnectRestURL","PDMPTestUrlIL","PaySimpleApiURL",
			"DoseSpotSingleSignOnURL","PayConnectWebServiceURL","CareCreditApiURL","NewCropRxEntryURL"};

		[ClassCleanup]
		public static void CleanupIntrospection() {
			IntrospectionT.DeletePref();
		}

		[TestMethod]
		public void Introspection_Preference_All_Valid_Keys() {
			List<Introspection.IntrospectionEntity> validKeys=Enum.GetValues(typeof(Introspection.IntrospectionEntity))
				.Cast<Introspection.IntrospectionEntity>().ToList();
			for(int i=0;i<_wikiPrefs.Count;i++) {
				try {
					string gottenVal=Introspection.GetOverride(validKeys[i],_defaultUrl);
				}
				catch(Exception ex) {
					Assert.Fail(ex.Message);
					return;
				}
			}
		}

		[TestMethod]
		public void Introspection_Preference_Invalid_Key() {
			string validKeyValue="blah";
			string invalidKey="DoesNotExist";
			string valueString=$"{{ \"{nameof(Introspection.IntrospectionEntity.DentalXChangeDwsURL)}\": \"{validKeyValue}\",\n \"{invalidKey}\":\"a\" }}";
			IntrospectionT.UpsertPref(valueString);
			string retVal=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL);
			//Should still read the correct value even if an invalid key was present
			Assert.AreEqual(retVal,validKeyValue);
		}

		[TestMethod]
		public void Introspection_Preference_Malformed() {
			IntrospectionT.UpsertPref("INVALID JSON ValueString");
			string retVal="";
			try {
				retVal=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,_defaultUrl);
			}
			catch(ApplicationException) {
				//GetOverride() should throw an application exception if the json is malformed and retVal should never be set.
				Assert.IsTrue(retVal=="");
				return;
			}
			Assert.Fail();
		}

		[TestMethod]
		public void Introspection_Preference_Missing() {
			IntrospectionT.DeletePref();
			string retVal=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,_defaultUrl);
			//Because there is no preference in the DB we should get back the default value we passed in.
			Assert.AreEqual(retVal,_defaultUrl);
		}

		[TestMethod]
		public void Introspection_Preference_Present() {
			IntrospectionT.DeletePref();
			string testUrl="https://opendental.com/";
			IntrospectionT.UpsertPref(new Dictionary<Introspection.IntrospectionEntity, string>() {
				{ Introspection.IntrospectionEntity.DentalXChangeDwsURL,testUrl }
			});
			string retVal=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,_defaultUrl);
			//The preference is present and valid. retVal should be overridden with the preference value.
			Assert.AreEqual(testUrl,retVal);
		}
	}
}
