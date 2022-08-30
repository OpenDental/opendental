using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.EmailAttach_Tests {
	[TestClass]
	public class EmailAttachTests:TestBase {
		private const string _attachmentRaw=@"R01GOD1hJQA1AKIAAP/////78P/omn19fQAAAAAAAAAAAAAAACwAAAAAJQA1AAAD7Qi63P5w
wEmjBCLrnQnhYCgM1wh+pkgqqeC9XrutmBm7hAK3tP31gFcAiFKVQrGFR6kscnonTe7FAAad
GugmRu3CmiBt57fsVq3Y0VFKnpYdxPC6M7Ze4crnnHum4oN6LFJ1bn5NXTN7OF5fQkN5WYow
BEN2dkGQGWJtSzqGTICJgnQuTJN/WJsojad9qXMuhIWdjXKjY4tenjo6tjVssk2gaWq3uGNX
U6ZGxseyk8SasGw3J9GRzdTQky1iHNvcPNNI4TLeKdfMvy0vMqLrItvuxfDW8ubjueDtJufz
7itICBxISKDBgwgTKjyYAAA7";

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

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void EmailAttaches_CreateAttach_Inbound() {
			EmailAttach emailAttach=EmailAttaches.CreateAttach("map_of_argentina.gif","",Encoding.UTF8.GetBytes(_attachmentRaw),false);
			Assert.IsTrue(File.Exists(ODFileUtils.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName)));
		}

		[TestMethod]
		public void EmailAttaches_CreateAttach_Outbound() {
			EmailAttach emailAttach=EmailAttaches.CreateAttach("map_of_argentina.gif","",Encoding.UTF8.GetBytes(_attachmentRaw),true);
			Assert.IsTrue(File.Exists(ODFileUtils.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName)));
		}

		[TestMethod]
		public void EmailAttaches_CreateAttach_InvalidDisaplyName() {
			string displayName="invalid*display?Name.gif";
			EmailAttach emailAttachFirst=EmailAttaches.CreateAttach(displayName,"",Encoding.UTF8.GetBytes(_attachmentRaw),false);
			EmailAttach emailAttachSecond=EmailAttaches.CreateAttach(displayName,"",Encoding.UTF8.GetBytes(_attachmentRaw),false);
			Assert.IsTrue(File.Exists(ODFileUtils.CombinePaths(EmailAttaches.GetAttachPath(),emailAttachFirst.ActualFileName)));
			Assert.IsTrue(File.Exists(ODFileUtils.CombinePaths(EmailAttaches.GetAttachPath(),emailAttachSecond.ActualFileName)));
			Assert.AreNotEqual(emailAttachFirst.ActualFileName,emailAttachSecond.ActualFileName);
		}

	}
}
