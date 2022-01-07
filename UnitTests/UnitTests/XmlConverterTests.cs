using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.XmlConverter_Tests {
	[TestClass]
	public class XmlConverterTests:TestBase {

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		///<summary>Strange classes (like Family) were escaping strings too much for HTTP transmission sometimes due to properties and such.</summary>
		[TestMethod]
		public void XmlConverter_XmlEscapeRecursion_Family() {
			Patient pat=new Patient {
				FName="Jason",
				LName="Salmon & Smith",
			};
			Family family=new Family { ListPats=new[] { pat, } };
			//Regardless if this is the correct way to create a Family object, the fact remains that the pat object is used within another object (ListPats)
			//This was causing our recursive method to escape the pat object too many times.
			object familyEscaped=XmlConverter.XmlEscapeRecursion(family.GetType(),family);
			Assert.AreEqual("Salmon &amp; Smith",((Family)familyEscaped).ListPats[0].LName);
		}

		///<summary></summary>
		[TestMethod]
		public void XmlConverter_XmlEscapeRecursion_NewLines() {
			WebServiceTests.WebServiceTestObject testObj=new WebServiceTests.WebServiceTestObject() {
				ValueStr=WebServiceTests.NewLineString,
				ValueStr2=WebServiceTests.NewLineString,
			};
			object testObjEscaped=XmlConverter.XmlEscapeRecursion(testObj.GetType(),testObj);
			string expectedStr="Line1§#13;Line2§#10;Line3§#13;§#10;Line4";
			Assert.AreEqual(expectedStr,((WebServiceTests.WebServiceTestObject)testObjEscaped).ValueStr);
			Assert.AreEqual(expectedStr,((WebServiceTests.WebServiceTestObject)testObjEscaped).ValueStr2);
		}

	}
}
