using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class PDMPTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
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
			IntrospectionT.DeletePref();
		}
		private PdmpProperty CreateFakeProps() {
			long provNum=ProviderT.CreateProvider("appriss-doc",fName:"Paul",lName:"Doctor");
			Provider prov=Providers.GetProv(provNum);
			prov.NationalProvID="1023011178";
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,lName:"Married",fName:"Sherri",birthDate:new DateTime(1950,1,1));
			pat.MiddleI="Dylan";
			pat.Gender=PatientGender.Male;
			pat.State="KS";
			pat.Address="555 Fake Dr.";
			pat.City="Wichita";
			pat.Zip="67203";
			pat.HmPhone="7770000000";
			prov.StateLicense="A1231xbzy-bx";
			ProviderClinic provClinic=ProviderClinicT.CreateProviderClinic(prov,stateWhereLicensed:"KS",stateLicense:"A1231xbzy-bx");
			ProviderClinicT.Update(provClinic);
			return new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr="KS",
				Dea="AD1111119",
				FacilityId="VAMC Division XYZ",
				ProvLicenseType="StateLicense",
				ProvLicenseNum="A1231xbzy-bx",
				Url="apprissTest.com"
			};
		}

		#region Logicoy Tests
		[TestMethod]
		public void PDMPLogicoy_MakeRequester() {
			PdmpProperty props=CreateFakeProps();
			PDMPLogicoy pdmp=new PDMPLogicoy(props);
			PDMPScript.RequesterType pdmpReq=pdmp.MakeRequester();
			Assert.AreEqual(props.FacilityId,pdmpReq.Location.Name);
			Assert.AreEqual(props.Dea,pdmpReq.Provider.DEANumber.Value);
			Assert.AreEqual(props.ProvLicenseNum,pdmpReq.Provider.ProfessionalLicenseNumber.Value.Value);
		}

		[TestMethod]
		public void PDMPLogicoy_MakePrescriptionRequester() {
			PdmpProperty props=CreateFakeProps();
			PDMPLogicoy pdmp=new PDMPLogicoy(props);
			PDMPScript.PrescriptionSummaryRequestTypePrescriptionRequest prescReq=pdmp.MakePrescriptionRequest();
			Assert.AreEqual(props.PdmpPat.FName,prescReq.Patient.Name.First.Value);
			Assert.AreEqual(props.PdmpPat.Gender.ToString().First(),prescReq.Patient.SexCode.Value.ToString().First());
			Assert.AreEqual(props.PdmpPat.Birthdate,prescReq.Patient.Birthdate);
		}
		#endregion

		#region Appriss Tests

		[TestMethod]
		public void PDMPAppriss_MakeRequester() {
			PdmpProperty props=CreateFakeProps();
			PDMPAppriss pdmp=new PDMPAppriss(props);
			pdmp.Url=props.Url;
			var req=pdmp.MakeRequester();
			Assert.AreEqual(props.PdmpProv.LName,req.Requester.Provider.LastName);
			Assert.AreEqual(props.ProvLicenseNum,req.Requester.Provider.ProfessionalLicenseNumber.Value);
			Assert.AreEqual(props.StateAbbr,req.Requester.Location.Address.StateCode.ToString());
		}

		[TestMethod]
		public void PDMPAppriss_MakePrescriptionRequest() {
			long provNum=ProviderT.CreateProvider("appriss-doc",fName:"Paul",lName:"Doctor");
			Provider prov=Providers.GetProv(provNum);
			prov.NationalProvID="1023011178";
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,lName:"Married",fName:"Sherri",birthDate:new DateTime(1950,1,1));
			pat.MiddleI="Dylan";
			pat.Gender=PatientGender.Male;
			pat.State="KS";
			pat.Address="555 Fake Dr.";
			pat.City="Wichita";
			pat.Zip="67203";
			pat.HmPhone="7770000000";
			prov.StateLicense="A1231xbzy-bx";
			ProviderClinic provClinic=ProviderClinicT.CreateProviderClinic(prov,stateWhereLicensed:"KS",stateLicense:"A1231xbzy-bx");
			ProviderClinicT.Update(provClinic);
			PdmpProperty props=new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr="KS",
				Dea="AD1111119",
				FacilityId="VAMC Division XYZ",
				ProvLicenseType="StateLicense",
				ProvLicenseNum="A1231xbzy-bx",
				Url="apprissTest.com"
			};
			PDMPAppriss pdmp=new PDMPAppriss(props);
			var req=pdmp.MakePrescriptionRequest();
			Assert.AreEqual(props.PdmpPat.FName,req.Patient.Name.First);
			Assert.AreEqual(props.PdmpPat.Gender.ToString().First(),req.Patient.SexCode.ToString().First());
			Assert.AreEqual(props.PdmpPat.Birthdate,req.Patient.Birthdate);
		}

		[TestMethod]
		public void PDMPAppriss_GetResponse_HasScores() {
			PdmpProperty props=CreateFakeProps();
			ApprissScript.ReportType mockReport=new ApprissScript.ReportType();
			mockReport.Message="";//Messages are blank when a match is found
			ApprissScript.NarxScoreType score1=new ApprissScript.NarxScoreType();
			score1.ScoreType="Presc";
			score1.ScoreValue="100";
			ApprissScript.NarxScoreType score2=new ApprissScript.NarxScoreType();
			score1.ScoreType="Narco";
			score1.ScoreValue="200";
			mockReport.NarxScores=new ApprissScript.NarxScoreType[] {score1,score2};
			PDMPAppriss pdmp=new PDMPAppriss(props);
			string retVal=pdmp.GetResponse(mockReport,"fakesite.com");
			Assert.IsTrue(retVal.Contains("Some states do not require the full PDMP report to be viewed."));
			mockReport.Message="New message";
			retVal=pdmp.GetResponse(mockReport,"fdafd");
			//if a report has a message then we don't bother adding this message
			Assert.IsFalse(retVal.Contains("Some states do not require the full PDMP report to be viewed."));
		}

		[TestMethod]
		public void PDMPAppriss_GetResponse_NoScores() {
			PdmpProperty props=CreateFakeProps();
			PDMPAppriss pdmp=new PDMPAppriss(props);
			ApprissScript.ReportType mockReport=new ApprissScript.ReportType();
			mockReport.Message="Null scores result.";
			string retVal=pdmp.GetResponse(mockReport,"www.fakesite.com");
			Assert.IsTrue(retVal.Contains(mockReport.Message));
		}

		[TestMethod]
		public void PDMPAppriss_PasswordDigest() {
			string nonce="648db976-6428-459d-873e-891169e5a4e2";
			long epoch=1516658372;
			ApprissAuth apps=new ApprissAuth(epoch,nonce);
			string retVal=apps.GetPasswordDigest("1!Password");
			string expected="1a6bb6be5ecd6c440ea84a0e7a5c5ea742d203c92e1c37e4962dedb49fa72c47";//value from documentation
			Assert.AreEqual(expected,retVal);
		}

		[TestMethod]
		public void PDMPAppriss_EpochTime() {
			long expectedVal=1601281320;//time in epoch based on https://www.freeformatter.com/epoch-timestamp-to-date-converter.html
			long actualVal=ApprissAuth.GetEpoch(new DateTime(2020,9,28,1,22,0));
			Assert.AreEqual(expectedVal,actualVal);
		}

		#endregion

	}
}
