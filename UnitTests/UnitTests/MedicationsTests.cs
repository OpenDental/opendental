
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Medications_Tests {
	[TestClass]


	public class MedicationsTests:TestBase {
		[TestInitialize]
		public void SetUpTest() {
			MedicationT.ClearMedicationTable();
		}
		[TestMethod]
		public void Medications_IsGenericInUse_No() {
			Medication genMed=MedicationT.CreateMedicationForMerge(true);
			Assert.IsTrue(genMed.MedicationNum==genMed.GenericNum);
			Assert.IsFalse(Medications.IsInUseAsGeneric(genMed));
		}

		[TestMethod]
		public void Medications_IsGenericInUse_Yes() {
			Medication genMed=MedicationT.CreateMedicationForMerge(true);
			MedicationT.CreateMedicationForMerge(false,genMed.MedicationNum);
			Assert.IsTrue(genMed.MedicationNum==genMed.GenericNum);
			Assert.IsTrue(Medications.IsInUseAsGeneric(genMed));
		}

		[TestMethod]
		public void Medications_Merge_RemovedFromDatabase() {
			Medication genMedToMerge=MedicationT.CreateMedicationForMerge(true,medName:"DeleteThisOne");
			Medication genMedNoMerge=MedicationT.CreateMedicationForMerge(true,medName:"KeepThisOne");
			Medication brandMed=MedicationT.CreateMedicationForMerge(false,genMedNoMerge.MedicationNum,"MergeIntoThisOne");
			Medications.RefreshCache();
			Assert.IsFalse(Medications.IsInUseAsGeneric(genMedToMerge));
			Medications.Merge(genMedToMerge.MedicationNum,brandMed.MedicationNum);
			Medications.RefreshCache();
			List<Medication> listMeds=Medications.GetList();
			Assert.IsFalse(listMeds.Contains(genMedToMerge));
		}
	}
}
