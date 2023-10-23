using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class PeerInfosTests:TestBase {

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
		public void PeerInfos_SetEmployeeNum_DuplicateEmployee() {
			EmployeeT.ClearEmployeeTable();
			Employee employeeFirst=EmployeeT.CreateEmployee("First","Employee",emailWork:"first@peerinfo.com");
			Employee employeeDuplicate1=EmployeeT.CreateEmployee("Duplicate1","Employee",emailWork:"duplicate@peerinfo.com");
			Employee employeeDuplicate2=EmployeeT.CreateEmployee("Duplicate2","Employee",emailWork:"duplicate@peerinfo.com");
			Employee employeeLast=EmployeeT.CreateEmployee("Last","Employee",emailWork:"last@peerinfo.com");
			List<PeerInfo> listPeerInfos=new List<PeerInfo>() {
				new PeerInfo() { UserName="first@peerinfo.com" },
				new PeerInfo() { UserName="duplicate@peerinfo.com" },
				new PeerInfo() { UserName="last@peerinfo.com" },
			};
			PeerInfos.SetEmployeeNum(ref listPeerInfos);
			Assert.AreEqual(employeeFirst.EmployeeNum,listPeerInfos[0].EmployeeNum);
			Assert.AreEqual(employeeDuplicate1.EmployeeNum,listPeerInfos[1].EmployeeNum);//FIFO
			Assert.AreEqual(employeeLast.EmployeeNum,listPeerInfos[2].EmployeeNum);
		}

		[TestMethod]
		public void PeerInfos_SetEmployeeNum_DuplicateEmployeeCaseSensitivity() {
			EmployeeT.ClearEmployeeTable();
			Employee employeeFirst=EmployeeT.CreateEmployee("First","Employee",emailWork:"first@peerinfo.com");
			Employee employeeDuplicate1=EmployeeT.CreateEmployee("Duplicate1","Employee",emailWork:"duplicate@peerinfo.com");
			Employee employeeDuplicate2=EmployeeT.CreateEmployee("Duplicate2","Employee",emailWork:"Duplicate@peerinfo.com");
			Employee employeeLast=EmployeeT.CreateEmployee("Last","Employee",emailWork:"last@peerinfo.com");
			List<PeerInfo> listPeerInfos=new List<PeerInfo>() {
				new PeerInfo() { UserName="first@peerinfo.com" },
				new PeerInfo() { UserName="duplicate@peerinfo.com" },
				new PeerInfo() { UserName="last@peerinfo.com" },
			};
			PeerInfos.SetEmployeeNum(ref listPeerInfos);
			Assert.AreEqual(employeeFirst.EmployeeNum,listPeerInfos[0].EmployeeNum);
			Assert.AreEqual(employeeDuplicate1.EmployeeNum,listPeerInfos[1].EmployeeNum);//FIFO
			Assert.AreEqual(employeeLast.EmployeeNum,listPeerInfos[2].EmployeeNum);
		}

		[TestMethod]
		public void PeerInfos_SetEmployeeNum_MissingEmployee() {
			EmployeeT.ClearEmployeeTable();
			Employee employeeFirst=EmployeeT.CreateEmployee("First","Employee",emailWork:"first@peerinfo.com");
			Employee employeeSecond=EmployeeT.CreateEmployee("Second","Employee",emailWork:"second@peerinfo.com");
			Employee employeeLast=EmployeeT.CreateEmployee("Last","Employee",emailWork:"last@peerinfo.com");
			List<PeerInfo> listPeerInfos=new List<PeerInfo>() {
				new PeerInfo() { UserName="first@peerinfo.com" },
				new PeerInfo() { UserName="unknown@peerinfo.com" },
				new PeerInfo() { UserName="last@peerinfo.com" },
			};
			PeerInfos.SetEmployeeNum(ref listPeerInfos);
			Assert.AreEqual(employeeFirst.EmployeeNum,listPeerInfos[0].EmployeeNum);
			Assert.AreEqual(0,listPeerInfos[1].EmployeeNum);//Unknown EmployeeNum should default to 0.
			Assert.AreEqual(employeeLast.EmployeeNum,listPeerInfos[2].EmployeeNum);
		}

	}
}
