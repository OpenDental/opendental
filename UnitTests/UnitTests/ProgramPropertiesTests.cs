using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ProgramProperties_Tests {
	[TestClass]
	public class ProgramPropertiess:TestBase {
		private static List<ProgramProperty> _listProgramProperties=new List<ProgramProperty>();
		private IWebServiceMainHQ _webInstanceOld;

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//There are many program properties that are expected to be within the database.
			//Make a deep copy of the entire program property cache and keep it around so that we can reinsert them after these tests run.
			_listProgramProperties=ProgramProperties.GetWhere(x => true);
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			ProgramPropertyT.ClearProgamPropertyTable();
			HqProgram.ClearCaches();
			Programs.RefreshCache();
			_webInstanceOld=WebServiceMainHQProxy.GetWebServiceMainHQInstance();
			WebServiceMainHQProxy.MockWebServiceMainHQ=new WebServiceMainHQMockDemo() {
				EnableAdditionalFeaturesDelegate=(officeData) => {
					List<HqProgram> listHqProgsForBiz=new List<HqProgram> {
						new HqProgram() {
							ProgramNameAsString=ProgramName.PDMP.ToString(),
							IsEnabled=true,
							ListProperties=new List<HqProgramProperty>() {
								new HqProgramProperty() {
									PropertyDesc="California "+PdmpProperty.PdmpUrl,
									PropertyValue="www.testpdmp.com/ca",
								},
							}
						},
					};
					foreach(HqProgram prog in listHqProgsForBiz) {
						if(!prog.IsEnabled) {
							prog.ListProperties.Where(x =>x.PropertyDesc!="Disable Advertising HQ").ForEach(x => x.PropertyValue="");//When program is disabled by HQ, clear out any property values at Dental Office.
						}
					}
					return PayloadHelper.CreateSuccessResponse(new List<PayloadItem> {
						new PayloadItem(listHqProgsForBiz,"ListHqPrograms"),
						new PayloadItem((long)TimeSpan.FromHours(12).TotalHours,"IntervalHours"),
					});
				},
			};
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			ProgramPropertyT.ClearProgamPropertyTable();
			ProgramProperties.InsertMany(_listProgramProperties);
			ProgramProperties.RefreshCache();
			_webInstanceOld=null;
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
			IntrospectionT.DeletePref();

		}

		[TestMethod]
		///<summary>Tests the Delete method in ProgramProperties to ensure it will delete when the PropertyDesc is one of those in 
		///the GetDeletablePropertyDescriptions() list, in this case ProgramProperties.PropertyDescs.ClinicHideButton.</summary>
		public void ProgramProperties_Delete_DeletesWhenDescriptionInGetDeletablePropertyDescriptions() {
			ProgramProperty prop=ProgramPropertyT.CreateProgramProperty(10,ProgramProperties.PropertyDescs.ClinicHideButton,1);
			ProgramProperty getPropBefore=ProgramProperties.GetPropForProgByDesc(prop.ProgramNum,prop.PropertyDesc);
			Assert.IsNotNull(getPropBefore);
			Assert.AreEqual(prop.ProgramPropertyNum,getPropBefore.ProgramPropertyNum);
			try {
				ProgramProperties.Delete(prop);
			}
			catch (Exception ex) {
				ex.DoNothing();
			}
			ProgramProperties.RefreshCache();//Make sure data is as current as it can be.
			//Ensure it was deleted.
			ProgramProperty getPropAfter=ProgramProperties.GetPropForProgByDesc(prop.ProgramNum,prop.PropertyDesc);
			Assert.IsNull(getPropAfter);//No longer in DB
		}

		[TestMethod]
		///<summary>Tests the Delete method in ProgramProperties to ensure it does not delete a ProgramProperty when the PropertyDesc is outside of those
		///in GetDeletablePropertyDescriptions(), instead it throws an exception (which we catch here) and then remains in the db.</summary>
		public void ProgramProperties_Delete_DoesNotDeleteWhenDescriptionIsNotInGetDeletablePropertyDescriptions() {
			ProgramProperty prop=ProgramPropertyT.CreateProgramProperty(20,"Stuff",2);
			ProgramProperty getPropBefore=ProgramProperties.GetPropForProgByDesc(prop.ProgramNum,prop.PropertyDesc);
			Assert.IsNotNull(getPropBefore);
			Assert.AreEqual(prop.ProgramPropertyNum,getPropBefore.ProgramPropertyNum);
			try {
				ProgramProperties.Delete(prop);
			}
			catch (Exception ex) {
				ex.DoNothing();
			}
			ProgramProperties.RefreshCache();//Make sure data is as current as it can be.
			//Ensure it was NOT deleted since the description is most certainly not in GetDeletablePropertyDescriptions().
			ProgramProperty getPropAfter=ProgramProperties.GetPropForProgByDesc(prop.ProgramNum,prop.PropertyDesc);
			Assert.IsNotNull(getPropAfter);//Still in DB
			Assert.AreEqual(prop.ProgramPropertyNum,getPropAfter.ProgramPropertyNum);//And we know its the ProgramProperty we put in.
		}

		[TestMethod]
		public void PDMP_GetPropertyFromSource_UseBusinessValueNoDownload() {
			OpenDentBusiness.Program prog=Programs.GetCur(ProgramName.PDMP);
			string propDesc="California "+PdmpProperty.PdmpProvLicenseField;
			ProgramPropertyT.UpdateProgramProperty(ProgramName.PDMP,propDesc,"ExpectedVal");
			ProgramProperties.RefreshCache();
			ProgramProperty prop=ProgramProperties.GetPropForProgByDesc(prog.ProgramNum,propDesc);
			string actualValue=ProgramProperties.GetFirstOrDefault(x=>x.ProgramNum==prog.ProgramNum && x.PropertyDesc==propDesc).PropertyValue;
			Assert.AreEqual(actualValue,prop.PropertyValue);
		}

		[TestMethod]
		public void PDMP_GetPropertyFromSource_UseBusinessValueWithDownload() {
			OpenDentBusiness.Program prog=Programs.GetCur(ProgramName.PDMP);
			string propDesc="Not Hq property";
			string propVal="dbVal";
			ProgramProperty notHqProp=ProgramProperties.GetPropForProgByDesc(prog.ProgramNum,propDesc);
			if(notHqProp!=null) {
				ProgramProperties.UpdateProgramPropertyWithValue(notHqProp,propVal);
			}
			else {
				ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,propDesc,0,propertyValue:propVal);
			}
			ProgramProperties.RefreshCache();
			HqProgram.Download();
			string actualValue=ProgramProperties.GetFirstOrDefault(x=>x.ProgramNum==prog.ProgramNum && x.PropertyDesc==propDesc).PropertyValue;
			Assert.AreEqual(propVal,actualValue);
		}

		[TestMethod]
		public void PDMP_GetPropertyFromSource_UseHQ() {
			string propDesc="California "+PdmpProperty.PdmpUrl;
			OpenDentBusiness.Program prog=Programs.GetCur(ProgramName.PDMP);
			ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,propDesc,0);
			ProgramProperty propertyFromCache=ProgramProperties.GetPropForProgByDesc(prog.ProgramNum,propDesc);
			Assert.AreEqual("",propertyFromCache.PropertyValue);
			if(ProgramProperties.UpdateProgramPropertyWithValue(propertyFromCache,"www.fakeurl.com")) {
				ProgramProperties.RefreshCache();
			}
			propertyFromCache=ProgramProperties.GetPropForProgByDesc(Programs.GetCur(ProgramName.PDMP).ProgramNum,propDesc);
			Assert.AreNotEqual("",propertyFromCache.PropertyValue);
			Assert.AreEqual("www.fakeurl.com",propertyFromCache.PropertyValue);
			HqProgram.Download();
			ProgramProperty propertyFromHq=ProgramProperties.GetPropForProgByDesc(Programs.GetCur(ProgramName.PDMP).ProgramNum,propDesc);
			Assert.AreNotEqual(propertyFromHq.PropertyValue,propertyFromCache.PropertyValue);		
		}
	}
}
