using System.Collections.Generic;
using Health.Direct.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class InsPlanSubstitutionTest:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		[ClassCleanup]
		public static void TearDownClass() {
		}


		[TestMethod]
		public void InsPlanSubstitution_AreEqual() {
			ProcedureCode proc1=ProcedureCodeT.CreateProcCode("1111");
			ProcedureCode proc2=ProcedureCodeT.CreateProcCode("2222");
			SubstitutionLink subLink1=SubstitutionLinkT.CreateSubstitutionLink(proc1.CodeNum,"0001",SubstitutionCondition.Always,0);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(proc2.CodeNum,"0002",SubstitutionCondition.Always,0);
			InsPlanSubstitution insSub1=new InsPlanSubstitution(proc1,subLink1);
			InsPlanSubstitution insSub2=new InsPlanSubstitution(proc2,subLink2);
			Assert.IsFalse(InsPlanSubstitution.AreEqual(insSub1,insSub2));
			InsPlanSubstitution insSub3=insSub1;
			Assert.IsTrue(InsPlanSubstitution.AreEqual(insSub1,insSub3));
			insSub3=new InsPlanSubstitution(proc1,subLink1);
			Assert.IsTrue(InsPlanSubstitution.AreEqual(insSub1,insSub3));
		}

		[TestMethod]
		public void InsPlanSubstitution_HasDuplicates() {
			ProcedureCode proc1=ProcedureCodeT.CreateProcCode("1111");
			ProcedureCode proc2=ProcedureCodeT.CreateProcCode("2222");
			SubstitutionLink subLink1=SubstitutionLinkT.CreateSubstitutionLink(proc1.CodeNum,"0001",SubstitutionCondition.Always,0);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(proc2.CodeNum,"0002",SubstitutionCondition.Always,0);
			InsPlanSubstitution insSub1=new InsPlanSubstitution(proc1,subLink1);
			InsPlanSubstitution insSub2=new InsPlanSubstitution(proc2,subLink2);
			List<InsPlanSubstitution> listInsPlanSubs=new List<InsPlanSubstitution>() {
				insSub1,insSub2
			};
			Assert.IsFalse(InsPlanSubstitution.HasDuplicates(listInsPlanSubs));
			InsPlanSubstitution insSub3=new InsPlanSubstitution(proc1,subLink1);
			listInsPlanSubs.Add(insSub3);
			Assert.IsTrue(InsPlanSubstitution.HasDuplicates(listInsPlanSubs));
		}
	}
}
