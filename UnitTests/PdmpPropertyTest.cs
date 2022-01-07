using System;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class PdmpPropertyTest:TestBase {

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

		#region Database Value Tests
		[TestMethod]
		public void PDMP_TryValidateValues_Fail() {
			bool didThrow=false;
			OpenDentBusiness.Program progCur = Programs.GetCur(ProgramName.PDMP);
			long provNum = ProviderT.CreateProvider("invalid-doc");
			Provider prov = Providers.GetProv(provNum);
			Patient pat = null;
			string state = "MD";
			PdmpProperty properties = new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr=state,
			};
			properties.PdmpPat=PatientT.CreatePatient();
			properties.PdmpProv.FName="Jerry";
			properties.PdmpProv.LName="Atric";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.ProvLicenseNum="A1-SteakSauce";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.PdmpPat.Birthdate=DateTime.Today;
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.PdmpPat.Address="123 Tuhehl Rd";
			properties.PdmpPat.City="Rack City";
			properties.PdmpPat.State="MD";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.StateAbbr="KY";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.PdmpPat.SSN="555555555";
			properties.Dea="789456123";
			properties.Url="www.fakeurl.com/test";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsFalse(didThrow);
		}

		[TestMethod]
		public void PDMP_TryValidateValues_HappyPath() {
			OpenDentBusiness.Program progCur = Programs.GetCur(ProgramName.PDMP);
			string state = "MD";
			long provNum = ProviderT.CreateProvider("Valid-Prov",fName: "Jerry",lName: "Atric",nationalProvID: "B012");
			Provider prov = Providers.GetProv(provNum);
			Patient pat = new Patient() {
				Address="555 Haight St",
				City="San Ramon",
				State="MD",
				Birthdate=DateTime.Today,
			};
			PdmpProperty props = new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr=state,
				ProvLicenseNum="A1-SteakSauce",
				Url="www.fakesite.com",
				Dea="123456789",
			};
			try {
				props.Validate(progCur);
			}
			catch {
				Assert.Fail();
			}
		}

		[TestMethod]
		public void Appriss_TryValidateValues_Fail() {
			bool didThrow=false;
			OpenDentBusiness.Program progCur = Programs.GetCur(ProgramName.Appriss);
			long provNum = ProviderT.CreateProvider("invalid-doc");
			Provider prov = Providers.GetProv(provNum);
			Patient pat = null;
			string state = "Ma";
			PdmpProperty properties = new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr=state,
			};
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.PdmpPat=PatientT.CreatePatient(lName: "Abbot",fName: "Hannah",zip: "43215");
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;;
			properties.PdmpPat.Birthdate=new DateTime(2001,1,1);
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.Dea="123456789";
			properties.ProvLicenseNum="8675309";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.PdmpProv.FName="George";
			properties.PdmpProv.LName="Harris";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.Url="www.fakeapprisswebsite.eu";
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			didThrow=false;
			properties.StateAbbr="MA";
			CDT.Class1.Encrypt("F@k3C3Rt10010010011011",out properties.ApprissClientKey);
			CDT.Class1.Encrypt("f@k3p@$$w0rd",out properties.ApprissClientPassword);
			try {
			properties.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsFalse(didThrow);
		}

		[TestMethod]
		public void Appriss_TryValidateValues_HappyPath() {
			OpenDentBusiness.Program progCur = Programs.GetCur(ProgramName.Appriss);
			string state = "MD";
			long provNum = ProviderT.CreateProvider("Valid-Prov",fName: "Jerry",lName: "Atric",nationalProvID: "B012");
			Provider prov = Providers.GetProv(provNum);
			Patient pat = new Patient() {
				Zip="12345",
				Birthdate=DateTime.Today,
			};
			CDT.Class1.Encrypt("F@k3C3Rt10010010011011",out string encryptedKey);
			CDT.Class1.Encrypt("f@k3p@$$w0rd",out string encryptedPassword);
			PdmpProperty props = new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr=state,
				ProvLicenseNum="A1-SteakSauce",
				Url="www.fakesite.com",
				Dea="123456789",
				ApprissClientKey=encryptedKey,
				ApprissClientPassword=encryptedPassword
			};
			bool didThrow=false;
			try {
			props.Validate(progCur);
			}
			catch (Exception ex) {
				ex.DoNothing();
				didThrow=true;
			}
			Assert.IsFalse(didThrow);
		}
		#endregion

	}
}
