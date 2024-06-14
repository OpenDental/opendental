using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class CodeSystemsTests:TestBase {

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			SnomedT.ClearSnomedTable();
		}

		#region SNOMEDCT
		/// <summary>Asserts that unique codes are inserted when there are duplicates in the file being imported from.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_NothingInDB_DuplicatesInSnomedFile() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis");
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,false);
			List<Snomed> listSnomeds=Snomeds.GetAll();
			Assert.AreEqual(3,listSnomeds.Count);
		}

		/// <summary>Asserts that unique codes are inserted when all entries in the file being imported from are unique.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_NothingInDB_NoDuplicatesInSnomedFile() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,false);
			List<Snomed> listSnomeds=Snomeds.GetAll();
			Assert.AreEqual(3,listSnomeds.Count);
		}

		/// <summary>Asserts that the ImportSnomed method does not break when there are duplicate entries in the DB.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_CodesInDBSameAsInSnomedFile_DuplicatesInDB() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			Snomed snomed = new Snomed();
			snomed.SnomedCode="126813005";
			snomed.Description="Neoplasm of anterior aspect of epiglottis";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126813005";
			snomed.Description="Neoplasm of anterior aspect of epiglottis";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126814004";
			snomed.Description="Neoplasm of junctional region of epiglottis";
			Snomeds.Insert(snomed);
			snomed=new Snomed();
			snomed.SnomedCode="126814004";
			snomed.Description="Neoplasm of junctional region of epiglottis";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126815003";
			snomed.Description="Neoplasm of lateral wall of oropharynx";
			Snomeds.Insert(snomed);
			snomed=new Snomed();
			snomed.SnomedCode="126815003";
			snomed.Description="Neoplasm of lateral wall of oropharynx";
			Snomeds.Insert(snomed);
			
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis");
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,false);
			List<Snomed> listSnomedsNew=Snomeds.GetAll();
			Assert.AreEqual(6,listSnomedsNew.Count);
		}
		
		/// <summary>Happy Path.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_CodesInDBSameAsInSnomedFile_NoDuplicatesInDB() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			Snomed snomed = new Snomed();
			snomed.SnomedCode="126813005";
			snomed.Description="Neoplasm of anterior aspect of epiglottis";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126814004";
			snomed.Description="Neoplasm of junctional region of epiglottis";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126815003";
			snomed.Description="Neoplasm of lateral wall of oropharynx";
			Snomeds.Insert(snomed);
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,false);
			List<Snomed> listSnomedsNew=Snomeds.GetAll();
			Assert.AreEqual(listSnomedsNew.Count,listSnomedsNew.Count);
		}
		
		/// <summary>Mimics the behavior of the user importing codes with the 'Keep Old Description' checkbox checked when the codes being imported have different descriptions. Asserts that the code descriptions are unchanged.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_CodesInDBDifferFromSnomedFile_KeepOldDescription() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			Snomed snomed = new Snomed();
			snomed.SnomedCode="126813005";
			snomed.Description="Neoplasm of anterior aspect of epiglottis OLD";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126814004";
			snomed.Description="Neoplasm of junctional region of epiglottis OLD";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126815003";
			snomed.Description="Neoplasm of lateral wall of oropharynx OLD";
			Snomeds.Insert(snomed);
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis NEW");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis NEW");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx NEW");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,false);
			List<Snomed> listSnomedsNew=Snomeds.GetAll();
			Assert.AreEqual(3,listSnomedsNew.Count);
			Assert.IsTrue(listSnomedsNew.All(x => x.Description.Contains("OLD")));
		}
		
		/// <summary>Mimics the behavior of the user importing codes with the 'Keep Old Description' checkbox unchecked when the codes being imported have different descriptions. Asserts that the code descriptions are changed to the descriptions of the codes being imported.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_CodesInDBDifferFromSnomedFile_DoNotKeepOldDescription() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			Snomed snomed = new Snomed();
			snomed.SnomedCode="126813005";
			snomed.Description="Neoplasm of anterior aspect of epiglottis OLD";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126814004";
			snomed.Description="Neoplasm of junctional region of epiglottis OLD";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126815003";
			snomed.Description="Neoplasm of lateral wall of oropharynx OLD";
			Snomeds.Insert(snomed);
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005\tNeoplasm of anterior aspect of epiglottis NEW");
			listSnomedCodeStrs.Add("126814004\tNeoplasm of junctional region of epiglottis NEW");
			listSnomedCodeStrs.Add("126815003\tNeoplasm of lateral wall of oropharynx NEW");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,true);
			List<Snomed> listSnomedsNew=Snomeds.GetAll();
			Assert.AreEqual(3,listSnomedsNew.Count);
			Assert.IsTrue(listSnomedsNew.All(x => x.Description.Contains("NEW")));
		}
		
		/// <summary>Mimics the behavior of the user importing codes with the 'Keep Old Description' checkbox unchecked when the codes being imported have different descriptions. However, the file being imported from has no tabs in it, so none of the new codes should be imported. Asserts that the code descriptions are unchanged.</summary>
		[TestMethod]
		public void CodeSystems_ImportSnomed_CodesInDBDifferFromSnomedFile_DoNotKeepOldDescription_FileHasNoTabs() {
			bool quit=false;
			int numCodesImported=0;
			int numCodesUpdated=0;
			Snomed snomed = new Snomed();
			snomed.SnomedCode="126813005";
			snomed.Description="Neoplasm of anterior aspect of epiglottis OLD";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126814004";
			snomed.Description="Neoplasm of junctional region of epiglottis OLD";
			Snomeds.Insert(snomed);
			snomed = new Snomed();
			snomed.SnomedCode="126815003";
			snomed.Description="Neoplasm of lateral wall of oropharynx OLD";
			Snomeds.Insert(snomed);
			List<string> listSnomedCodeStrs=new List<string>();
			listSnomedCodeStrs.Add("126813005 Neoplasm of anterior aspect of epiglottis NEW");
			listSnomedCodeStrs.Add("126814004 Neoplasm of junctional region of epiglottis NEW");
			listSnomedCodeStrs.Add("126815003 Neoplasm of lateral wall of oropharynx NEW");
			File.WriteAllLines("C:\\Temp\\SNOMED_TEST.txt",listSnomedCodeStrs.ToArray());
			CodeSystems.ImportSnomed("C:\\Temp\\SNOMED_TEST.txt",new CodeSystems.ProgressArgs((i,j) =>{ }),ref quit,ref numCodesImported,ref numCodesUpdated,true);
			List<Snomed> listSnomedsNew=Snomeds.GetAll();
			Assert.AreEqual(3,listSnomedsNew.Count);
			Assert.IsTrue(listSnomedsNew.All(x => x.Description.Contains("OLD")));
		}

		#endregion
	}
}