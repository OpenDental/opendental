using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Patients_Tests {
	[TestClass]
	public class PatientsTests:TestBase {

		[TestInitialize] 
		public void RunBeforeEachTest() {
			PatientT.ClearPatientTable();
			PhoneNumberT.ClearPhoneNumberTable();
			DiscountPlanSubT.ClearDiscountPlanSubTable();
		}

		#region GetPatientsByPartialName

		[TestMethod]
		public void Patients_GetPatientsByPartialName_LastAndFirst() {
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Owre",fName: "Sarah");
			List<Patient> listPats=Patients.GetPatientsByPartialName("sam owre");
			Assert.AreEqual(1,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_MatchTwoLastAndFirst() {
			PatientT.CreatePatient(lName: "OWRE",fName: "sam");
			PatientT.CreatePatient(lName: "Owre",fName: "sarah");
			List<Patient> listPats=Patients.GetPatientsByPartialName("owre s");
			Assert.AreEqual(2,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_JustFirst() {
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Owre",fName: "Sarah");
			List<Patient> listPats=Patients.GetPatientsByPartialName("SaRah");
			Assert.AreEqual(1,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_JustLast() {
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Owre",fName: "Sarah");
			List<Patient> listPats=Patients.GetPatientsByPartialName("OWRE");
			Assert.AreEqual(2,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_FirstNotPreferred() {
			PatientT.CreatePatient(lName: "Brock",fName: "Taylor");
			PatientT.CreatePatient(lName: "McGehee",fName: "Christopher",preferredName: "Chris");
			List<Patient> listPats=Patients.GetPatientsByPartialName("chris owre");
			Assert.AreEqual(0,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_LastAndPreferred() {
			PatientT.CreatePatient(lName: "Jansen",fName: "Andrew");
			PatientT.CreatePatient(lName: "Montano",fName: "Joseph",preferredName: "Joe");
			List<Patient> listPats=Patients.GetPatientsByPartialName("Joe Montano");
			Assert.AreEqual(1,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_TwoWordLastName() {
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Van Damme",fName: "Jean-Claude");
			List<Patient> listPats=Patients.GetPatientsByPartialName("van damme");
			Assert.AreEqual(1,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_TwoWordLastNamePlusFirst() {
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Van Damme",fName: "Jean-Claude");
			List<Patient> listPats=Patients.GetPatientsByPartialName("sam van damme");
			Assert.AreEqual(0,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_LotsOfNames() {
			PatientT.CreatePatient(lName: "Salmon",fName: "Jason");
			PatientT.CreatePatient(lName: "Jansen",fName: "Andrew");
			List<Patient> listPats=Patients.GetPatientsByPartialName("andrew jansen thinks programming is fun");
			Assert.AreEqual(0,listPats.Count);
		}

		[TestMethod]
		public void Patients_GetPatientsByPartialName_SameNames() {
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Owre",fName: "Sarah");
			List<Patient> listPats=Patients.GetPatientsByPartialName("sam sam");
			Assert.AreEqual(1,listPats.Count);
		}


		[TestMethod]
		public void Patients_GetPatientsByPartialName_AllCaps() {
			PatientT.CreatePatient(lName: "Buchanan",fName: "Cameron");
			PatientT.CreatePatient(lName: "Montano",fName: "Joseph");
			List<Patient> listPats=Patients.GetPatientsByPartialName("CAMERON BUCHANAN");
			Assert.AreEqual(1,listPats.Count);
		}


		[TestMethod]
		public void Patients_GetPatientsByPartialName_Everybody() {
			PatientT.CreatePatient(lName: "Buchanan",fName: "Cameron");
			PatientT.CreatePatient(lName: "Montano",fName: "Joseph");
			PatientT.CreatePatient(lName: "Owre",fName: "Sam");
			PatientT.CreatePatient(lName: "Owre",fName: "Sarah");
			PatientT.CreatePatient(lName: "Salmon",fName: "Jason");
			PatientT.CreatePatient(lName: "Jansen",fName: "Andrew");
			PatientT.CreatePatient(lName: "Van Damme",fName: "Jean-Claude");
			PatientT.CreatePatient(lName: "Brock",fName: "Taylor");
			PatientT.CreatePatient(lName: "McGehee",fName: "Christopher",preferredName: "Chris");
			List<Patient> listPats=Patients.GetPatientsByPartialName("");
			Assert.AreEqual(9,listPats.Count);
		}
		#endregion GetPatientsByPartialName

		#region GetAgingList
		[TestMethod]
		public void Patients_GetAgingList_Patients_GetAgingList_FamilyGuarantorPlanExcludableNonGuarantorNoInsurance() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: " ",guarantor: patientA.PatNum,balTotal: 5);
			PatPlan patPlan=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientA.PatNum,subNum: 1);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientA.PatNum);
			List<PatAging> listPatAgingExpected=new List<PatAging>();
			PatAging patAgingExpected=new PatAging();
			patAgingExpected.PatNum=patientA.PatNum;
			patAgingExpected.PatName="Dalton, Andy";
			patAgingExpected.BalTotal=5;
			patAgingExpected.AmountDue=5;
			patAgingExpected.PriProv=1;
			patAgingExpected.BillingType=40;
			patAgingExpected.Zip="";
			listPatAgingExpected.Add(patAgingExpected);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsFalse(listPatAgingActual.IsNullOrEmpty());
			Assert.AreEqual(listPatAgingActual.Count(),listPatAgingExpected.Count());
			Assert.IsTrue(listPatAgingActual.All(x=>PatientT.GetArePatAgingsEqual(patAgingExpected,x)));
		}

		[TestMethod]
		public void Patients_GetAgingList_FamilyGuarantorPlanExcludableNonGuarantorPlanIsNot() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientA.PatNum,subNum: 1);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientB.PatNum,subNum: 2);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientA.PatNum);
			List<PatAging> listPatAgingExpected=new List<PatAging>();
			PatAging patAgingExpected=new PatAging();
			patAgingExpected.PatNum=patientA.PatNum;
			patAgingExpected.PatName="Dalton, Andy";
			patAgingExpected.BalTotal=5;
			patAgingExpected.AmountDue=5;
			patAgingExpected.PriProv=1;
			patAgingExpected.BillingType=40;
			patAgingExpected.Zip="";
			listPatAgingExpected.Add(patAgingExpected);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsFalse(listPatAgingActual.IsNullOrEmpty());
			Assert.AreEqual(listPatAgingActual.Count(),listPatAgingExpected.Count());
			Assert.IsTrue(listPatAgingActual.All(x=>PatientT.GetArePatAgingsEqual(patAgingExpected,x)));
		}

		[TestMethod]
		public void Patients_GetAgingList_FamilyGuarantorNoInsuranceNonGuarantorPlanExcludable() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: " ",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			PatPlan patPlan=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientB.PatNum,subNum: 1);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientB.PatNum);
			List<PatAging> listPatAgingExpected=new List<PatAging>();
			PatAging patAgingExpected=new PatAging();
			patAgingExpected.PatNum=patientA.PatNum;
			patAgingExpected.PatName="Dalton, Andy";
			patAgingExpected.BalTotal=5;
			patAgingExpected.AmountDue=5;
			patAgingExpected.PriProv=1;
			patAgingExpected.BillingType=40;
			patAgingExpected.Zip="";
			listPatAgingExpected.Add(patAgingExpected);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsFalse(listPatAgingActual.IsNullOrEmpty());
			Assert.AreEqual(listPatAgingActual.Count(),listPatAgingExpected.Count());
			Assert.IsTrue(listPatAgingActual.All(x=>PatientT.GetArePatAgingsEqual(patAgingExpected,x)));
		}

		[TestMethod]
		public void Patients_GetAgingList_FamilyGuarantorInsurancePlanNotExcludableNonGuarantorPlanExcludable() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientA.PatNum,subNum: 1);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientB.PatNum,subNum: 2);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientB.PatNum);
			List<PatAging> listPatAgingExpected=new List<PatAging>();
			PatAging patAgingExpected=new PatAging();
			patAgingExpected.PatNum=patientA.PatNum;
			patAgingExpected.PatName="Dalton, Andy";
			patAgingExpected.BalTotal=5;
			patAgingExpected.AmountDue=5;
			patAgingExpected.PriProv=1;
			patAgingExpected.BillingType=40;
			patAgingExpected.Zip="";
			listPatAgingExpected.Add(patAgingExpected);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsFalse(listPatAgingActual.IsNullOrEmpty());
			Assert.AreEqual(listPatAgingActual.Count(),listPatAgingExpected.Count());
			Assert.IsTrue(listPatAgingActual.All(x=>PatientT.GetArePatAgingsEqual(patAgingExpected,x)));
		}

		/// <summary>This is tested for if a non-guarantor drops a not excludable plan and joins the guarantor's plan that is excludable.</summary>
		[TestMethod]
		public void Patients_GetAgingList_FamilyGuarantorPlanExcludableNonGuarentorChangedToGuarantorPlan() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			InsSub insSubA=InsSubT.CreateInsSub(subscriberNum: patientA.PatNum,planNum: 1);
			InsSub insSubB=InsSubT.CreateInsSub(subscriberNum: patientB.PatNum,planNum: 2);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientA.PatNum,subNum: insSubA.InsSubNum);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientB.PatNum,subNum: insSubA.InsSubNum);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientA.PatNum);
			listPatNums.Add(patientB.PatNum);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsTrue(listPatAgingActual.IsNullOrEmpty());
		}

		/// <summary>This is tested for if a guarantor drops a not excludable plan and joins a non-guarantor's plan that is excludable.</summary>
		[TestMethod]
		public void Patients_GetAgingList_GuarantorChangedToNonGuarantorExcludablePlan() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			InsSub insSubA=InsSubT.CreateInsSub(subscriberNum: patientA.PatNum,planNum: 1);
			InsSub insSubB=InsSubT.CreateInsSub(subscriberNum: patientB.PatNum,planNum: 2);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientA.PatNum,subNum: insSubB.InsSubNum);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientB.PatNum,subNum: insSubB.InsSubNum);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientA.PatNum);
			listPatNums.Add(patientB.PatNum);
			PatAging patAgingExpected=new PatAging();
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsTrue(listPatAgingActual.IsNullOrEmpty());
		}

		[TestMethod]
		public void Patients_GetAgingList_FamilyGuarantorInsurancePlanExcludableNonGuarantorPlanExcludable() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientA.PatNum,subNum: 1);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patientB.PatNum,subNum: 2);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientA.PatNum);
			listPatNums.Add(patientB.PatNum);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsTrue(listPatAgingActual.IsNullOrEmpty());
		}

		[TestMethod]
		public void Patients_GetAgingList_ExcludesFamilyWhereGuarantorsPlanIsExcludableAndNonGuarantorOnSamePlan() {
			//Arrange
			Patient patientA=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			Patient patientB=PatientT.CreatePatient(lName: "Green",fName: "A.J.",hasIns: "I",guarantor: patientA.PatNum,balTotal: 5);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum:patientA.PatNum,subNum: 1);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 2,patNum:patientB.PatNum,subNum: 1);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patientA.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patientA.PatNum);
			listPatNums.Add(patientB.PatNum);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsTrue(listPatAgingActual.IsNullOrEmpty());
		}

		[TestMethod]
		public void Patients_GetAgingList_SinglePatientPlanExcludable() {
			//Arrange
			Patient patient=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			PatPlan patPlan=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patient.PatNum,subNum: 1);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patient.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patient.PatNum);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsTrue(listPatAgingActual.IsNullOrEmpty());
		}

		[TestMethod]
		public void Patients_GetAgingList_PrimaryPlanExcludableSecondaryIsNot() {
			//Arrange
			Patient patient=PatientT.CreatePatient(lName: "Dalton",fName: "Andy", hasIns: "I",balTotal: 5);
			PatPlan patPlanA=PatPlanT.CreatePatPlan(ordinal: 1,patNum: patient.PatNum,subNum: 1);
			PatPlan patPlanB=PatPlanT.CreatePatPlan(ordinal: 2,patNum: patient.PatNum,subNum: 2);
			PatAgingTransaction patAgingTransaction=new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure, DateTime.Today);
			List<PatAgingTransaction> listPatAgingTransactions=new List<PatAgingTransaction>();
			listPatAgingTransactions.Add(patAgingTransaction);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTransactions=new SerializableDictionary<long, List<PatAgingTransaction>>();
			dictPatAgingTransactions[patient.PatNum]=listPatAgingTransactions;
			//This list determines what PatPlanNums to exclude
			List<long> listPatNums=new List<long>();
			listPatNums.Add(patient.PatNum);
			//Act
			List<PatAging> listPatAgingActual=Patients.GetAgingList(age:"",lastStatement: DateTime.MinValue,new List<long>(),excludeAddr: false,excludeNeg: false,
			excludeLessThan: 0,excludeInactive: false,ignoreInPerson: false,new List<long>(),isSuperStatements: false,isSinglePatient:false,
			new List<long>(),new List<long>(),dictPatAgingTransactions,listPatNumsToExclude: listPatNums);
			//Assert
			Assert.IsTrue(listPatAgingActual.IsNullOrEmpty());
		}
		#endregion GetAgingList

		#region GetPatNumsByNameBirthdayEmailAndPhone

		[TestMethod]
		public void Patients_GetPatNumsByNameBirthdayEmailAndPhone_SingleEmailMatch() {
			Patient patA,patB,patA2;
			GenerateThreePatients(out patA,out patB,out patA2);
			//Match using email on family member's account
			List<long> listMatchingPatNums=GetPatNumsByNameBirthdayEmailAndPhone(patA.LName,patA.FName,patA.Birthdate,patB.Email,
				new List<string>());
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}
		
		[TestMethod]
		public void Patients_GetPatNumsByNameBirthdayEmailAndPhone_SameNameAndBirthdate() {
			Patient patA,patB,patA2;
			GenerateThreePatients(out patA,out patB,out patA2);
			//Match two patients based on name and birthdate
			List<long> listMatchingPatNums=GetPatNumsByNameBirthdayEmailAndPhone(patA.LName,patA.FName,patA.Birthdate,"",new List<string>());
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patA2.PatNum));
		}
		
		[TestMethod]
		public void Patients_GetPatNumsByNameBirthdayEmailAndPhone_SameEmail() {
			Patient patA,patB,patA2;
			GenerateThreePatients(out patA,out patB,out patA2);
			//Match two patients based on email
			List<long> listMatchingPatNums=GetPatNumsByNameBirthdayEmailAndPhone(patA.LName,patA.FName,patA.Birthdate,patA.Email,new List<string>());
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patA2.PatNum));
		}
		
		[TestMethod]
		public void Patients_GetPatNumsByNameBirthdayEmailAndPhone_SamePhoneNumbers() {
			Patient patA,patB,patA2;
			GenerateThreePatients(out patA,out patB,out patA2);
			//Match two patients based on phone numbers
			List<long> listMatchingPatNums=GetPatNumsByNameBirthdayEmailAndPhone(patA.LName,patA.FName,patA.Birthdate,"",
				new List<string> { patB.HmPhone,patA2.WirelessPhone });
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patA2.PatNum));
		}
		
		[TestMethod]
		public void Patients_GetPatNumsByNameBirthdayEmailAndPhone_SinglePhoneNumber() {
			Patient patA,patB,patA2;
			GenerateThreePatients(out patA,out patB,out patA2);
			//Match one patient based on phone numbers
			List<long> listMatchingPatNums=GetPatNumsByNameBirthdayEmailAndPhone(patA.LName,patA.FName,patA.Birthdate,"",
				new List<string> { patA2.WirelessPhone });
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA2.PatNum));
		}
		
		[TestMethod]
		public void Patients_GetPatNumsByNameBirthdayEmailAndPhone_SingleNameAndBirthdate() {
			Patient patA,patB,patA2;
			GenerateThreePatients(out patA,out patB,out patA2);
			//Match one patient based on name and birthdate
			List<long> listMatchingPatNums=GetPatNumsByNameBirthdayEmailAndPhone(patB.LName,patB.FName,patB.Birthdate,"",new List<string>());
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patB.PatNum));
		}

		#endregion

		#region GetListPatNumsByNameAndBirthday
		#region Legacy Behavior, prior to N23472
		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchOnePat() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}
		
		///<summary>Does not match any patients, due to LName not matching.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchLName() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Robert",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Does not match any patients, due to FName not matching.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchFName() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"John",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Does not match any patients, due to Birthdate not matching.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchBirthday() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(1900,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Does not match any patients, due to Archived status.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchArchived() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Archived
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Does not match any patients, due to Deleted status.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchDeleted() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Deleted
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchTwoPats() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			Patient patB=PatientT.CreatePatient(
				lName:"Bob",
				fName:"John",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches two patients.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_TwoMatchTowPats() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			Patient patB=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Billy",
				birthdate:new DateTime(2000,1,14)
			);
			//Assert
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patB.PatNum));
		}
		#endregion
		#region Preferred Name
		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPreferred() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				preferredName:"Earl",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Earl",
				birthdate:new DateTime(2000,1,14),
				isPreferredMatch:true
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}
		///<summary>Matches two patients, one from FName and one from Preferred.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_TwoMatchesPreferred() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Billy",
				preferredName:"Earl",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			Patient patB=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Earl",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Earl",
				birthdate:new DateTime(2000,1,14),
				isPreferredMatch:true
			);
			//Assert
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patB.PatNum));
		}
		#endregion
		#region Partial Match
		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPartialStart() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Benjamin",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Ben",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPartialMiddle() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Liz",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPartialEnd() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Beth",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches no patients.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchPartialLName() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bobby",
				fName:"Beth",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPartialEndLowercase() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"beth",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches no patients.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_NoMatchesPartial() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Beth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Elizabeth",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(0,listMatchingPatNums.Count);
		}

		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPartialTwoPats() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Benjamin",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			Patient patB=PatientT.CreatePatient(
				lName:"Bob",
				fName:"John",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Ben",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches two patients.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_TwoMatchesPartial() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Benjamin",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);Patient patB=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Ben",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Ben",
				birthdate:new DateTime(2000,1,14),
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patB.PatNum));
		}
		#endregion
		#region Preferred and Partial Match
		///<summary>Matches one patient exactly.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_OneMatchPreferredPartial() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Benjamin",
				preferredName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Liz",
				birthdate:new DateTime(2000,1,14),
				isPreferredMatch:true,
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(1,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
		}

		///<summary>Matches two patients.</summary>
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_TwoMatchesPreferredPartial() {
			//Arrange
			Patient patA=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Benjamin",
				preferredName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			Patient patB=PatientT.CreatePatient(
				lName:"Bob",
				fName:"Elizabeth",
				birthDate:new DateTime(2000,1,14),
				patStatus:PatientStatus.Patient
			);
			//Act
			List<long> listMatchingPatNums=Patients.GetListPatNumsByNameAndBirthday(
				lName:"Bob",
				fName:"Liz",
				birthdate:new DateTime(2000,1,14),
				isPreferredMatch:true,
				isExactMatch:false
			);
			//Assert
			Assert.AreEqual(2,listMatchingPatNums.Count);
			Assert.IsTrue(listMatchingPatNums.Contains(patA.PatNum));
			Assert.IsTrue(listMatchingPatNums.Contains(patB.PatNum));
		}

		///<summary>B47528, starting in iOS 11, the iOS keyboard has the Smart Punctuation feature.
		///It enters a curly single quote when the single quote key is pressed.
		///This is counter to the majority of other operating systems that use a straight single quote.
		///So, when an iOS user enters "O’Brien", it will fail to match "O'Brien" in the DB.
		///It is also possible for the name in the DB to contain a curly quote.
		///To avoid both problems, we replace all curly single quotes with straight quotes for both sides of the comparison.///</summary>	
		[TestMethod]
		public void Patients_GetListPatNumsByNameAndBirthday_FindPatsWithDifferentSingleQuoteTypes() {
			string lName="O’Brien";
			DateTime birthDate=new DateTime(2000,1,14);
			List<string> listExactMatchFNames=new List<string>(){ "D'Wayne","D‘Wayne","D’Wayne" };
			List<string> listPartialMatchFNames=new List<string>() { "abcD'Wayne","D‘Waynexyz","abcD’Waynexyz" };
			List<string> listAllFNames=listExactMatchFNames.Concat(listPartialMatchFNames).ToList();
			List<Patient> listPatients=new List<Patient>();
			//Add six patients to the patient table, all with the same last name and birthdate, but with different variations of the first name D'Wayne.
			for(int i=0;i<listAllFNames.Count;i++) {
				Patient patient=PatientT.CreatePatient(lName:lName,fName:listAllFNames[i],birthDate:birthDate,clinicNum:1);
				listPatients.Add(patient);
			}
			List<long>listPatNumsExactMatch=listPatients
				.Where(x => listExactMatchFNames.Contains(x.FName))
				.Select(x => x.PatNum)
				.OrderBy(x => x)
				.ToList();
			List<long>listAllPatNums=listPatients
				.Select(x => x.PatNum)
				.OrderBy(x => x)
				.ToList();
			//Performing each variation of query to make sure there are no typos in combining WHERE and AND clauses when single quotes are involved.
			for(int i=0;i<listExactMatchFNames.Count;i++) {
				List<long> listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:true,isPreferredMatch:false,clinicNum:-1).OrderBy(x => x).ToList();
				//Assert searching for exact match with name D'Wayne, D‘Wayne, or D’Wayne returns all three of those patients but no partial matches.
				CollectionAssert.AreEqual(listPatNumsExactMatch,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:true,isPreferredMatch:true,clinicNum:-1).OrderBy(x => x).ToList();
				CollectionAssert.AreEqual(listPatNumsExactMatch,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:true,isPreferredMatch:false,clinicNum:1).OrderBy(x => x).ToList();
				CollectionAssert.AreEqual(listPatNumsExactMatch,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:true,isPreferredMatch:true,clinicNum:1).OrderBy(x => x).ToList();
				CollectionAssert.AreEqual(listPatNumsExactMatch,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:false,isPreferredMatch:false,clinicNum:-1).OrderBy(x => x).ToList();
				//Assert searching for partial match with name D'Wayne, D‘Wayne, or D’Wayne returns all six patients (exact and partial matches).
				CollectionAssert.AreEqual(listAllPatNums,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:false,isPreferredMatch:true,clinicNum:-1).OrderBy(x => x).ToList();
				CollectionAssert.AreEqual(listAllPatNums,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:false,isPreferredMatch:false,clinicNum:1).OrderBy(x => x).ToList();
				CollectionAssert.AreEqual(listAllPatNums,listPatNums);
				listPatNums=Patients.GetListPatNumsByNameAndBirthday(lName,listExactMatchFNames[i],birthDate,isExactMatch:false,isPreferredMatch:true,clinicNum:1).OrderBy(x => x).ToList();
			}
		}
		#endregion
		#endregion

		#region GetPatientsByPhone
		#region PatientTable		
		///<summary>Searches all three phone fields.  Finds a patient because searched number is in the WirelessPhone field.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_WirelessPhone_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wirelessPhone:phoneNumber);
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(pat1.PatNum,listPats.First().PatNum);
		}

		///<summary>Searches all three phone fields.  Finds a patient because searched number is in the HmPhone field.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_HmPhone_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(pat1.PatNum,listPats.First().PatNum);
		}

		///<summary>Searches all three phone fields.  Finds a patient because searched number is in the WkPhone field.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_WkPhone_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(pat1.PatNum,listPats.First().PatNum);
		}

		///<summary>Searches all three phone fields. Failes to find a patient because searched number is not any of the fields.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_Failure() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(phone:"1234567890");
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(0,listPats.Count);
		}

		
		///<summary>Searches all three phone fields.  Finds two patients because searched number is in a phone field for both patients.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_MultipleMatches_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			Patient pat2=PatientT.CreatePatient(wkPhone:"1234567890",wirelessPhone:phoneNumber);
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(2,listPats.Count);
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat1.PatNum));
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat2.PatNum));
		}

		///<summary>Searches all three phone fields.  Finds one patient even with a 1 at the beginning and US country code.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_CountryCode_Success() {
			string phoneNumber="15033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"US");
			Assert.AreEqual(1,listPats.Count);
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat1.PatNum));
		}

		///<summary>Searches all three phone fields.  Finds one patient even with a 1 at the beginning and no country code.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_NoCountryCode_Success() {
			string phoneNumber="15033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat1.PatNum));
		}
		#endregion
		#region PhoneNumber table		
		///<summary>Searches all three phone fields.  Finds a patient because searched number is in the WirelessPhone field.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_WirelessPhone_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wirelessPhone:phoneNumber);
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(pat1.PatNum,listPats.First().PatNum);
		}

		///<summary>Searches all three phone fields.  Finds a patient because searched number is in the HmPhone field.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_HmPhone_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(pat1.PatNum,listPats.First().PatNum);
		}

		///<summary>Searches all three phone fields.  Finds a patient because searched number is in the WkPhone field.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_WkPhone_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(pat1.PatNum,listPats.First().PatNum);
		}

		///<summary>Searches all three phone fields. Failes to find a patient because searched number is not any of the fields.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_Failure() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(phone:"1234567890");
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(0,listPats.Count);
		}

		///<summary>Searches all three phone fields.  Finds two patients because searched number is in a phone field for both patients.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_MultipleMatches_Success() {
			string phoneNumber="5033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			Patient pat2=PatientT.CreatePatient(wkPhone:"1234567890",wirelessPhone:phoneNumber);
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(2,listPats.Count);
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat1.PatNum));
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat2.PatNum));
		}

		///<summary>Searches all three phone fields.  Finds one patient even with a 1 at the beginning and US country code.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_CountryCode_Success() {
			string phoneNumber="15033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"US");
			Assert.AreEqual(1,listPats.Count);
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat1.PatNum));
		}

		///<summary>Searches all three phone fields.  Finds one patient even with a 1 at the beginning and no country code.</summary>
		[TestMethod]
		public void Patients_GetPatientsByPhone_PhoneNumberTable_NoCountryCode_Success() {
			string phoneNumber="15033635432";
			Patient pat1=PatientT.CreatePatient(wkPhone:phoneNumber,wirelessPhone:"1234567890");
			PrefT.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,true);
			PhoneNumbers.SyncAllPats();
			List<Patient> listPats=Patients.GetPatientsByPhone(phoneNumber,"");
			Assert.AreEqual(1,listPats.Count);
			Assert.IsTrue(listPats.Any(x => x.PatNum==pat1.PatNum));
		}

		///<summary>Creates many phone number fields, and syncs the PhoneNumberTable.</summary>
		[TestMethod]
		public void Patients_PhoneNumberTable_SyncAllPats() {
			//Arrange
			for(int i=0;i<10;i++) {
				//A few random orphaned PhoneNumbers.
				string phoneNumberVal=PhoneNumberT.GetRandomPhoneNumber();
				PhoneNumbers.Insert(new PhoneNumber {
					PatNum=1,
					PhoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(phoneNumberVal),
					PhoneNumberVal=phoneNumberVal,
					PhoneType=PhoneType.HmPhone,
				});
			}
			PhoneNumbers.SyncBatchSize=10;
			List<Patient> listPats=new List<Patient>();
			for(int i=0;i<5;i++) {
				Clinics.Insert(new Clinic());
			}
			Clinics.RefreshCache();
			foreach(Clinic clinic in Clinics.GetWhere(x => true).Concat(new List<Clinic> { Clinics.GetPracticeAsClinicZero() })) {
				for(int i=0;i<(PhoneNumbers.SyncBatchSize*3)+1;i++) {
					string hmPhone=PhoneNumberT.GetRandomPhoneNumber();
					string wkPhone=PhoneNumberT.GetRandomPhoneNumber();
					string wirelessPhone=PhoneNumberT.GetRandomPhoneNumber();
					listPats.Add(PatientT.CreatePatient(clinicNum:clinic.ClinicNum,phone:hmPhone,wkPhone:wkPhone,wirelessPhone:wirelessPhone));
				}
			}
			//Act
			PhoneNumbers.SyncAllPats();
			//Assert
			List<PhoneNumber> listPhoneNumbersAll=PhoneNumberT.GetAll();
			Assert.AreEqual(3*listPats.Count,listPhoneNumbersAll.Count);//3 phonenumbers per patient, with old PhoneNumbers removed.
			foreach(Patient pat in listPats) {
				string hmPhoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(pat.HmPhone);
				string wkPhoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(pat.WkPhone);
				string wirelessPhoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(pat.WirelessPhone);
				List<PhoneNumber> listPhoneNumbers=PhoneNumbers.GetPhoneNumbers(pat.PatNum);
				Assert.AreEqual(3,listPhoneNumbers.Count);
				Assert.IsTrue(listPhoneNumbers.Any(x => x.PhoneNumberDigits==hmPhoneNumberDigits && x.PhoneType==PhoneType.HmPhone));
				Assert.IsTrue(listPhoneNumbers.Any(x => x.PhoneNumberDigits==wkPhoneNumberDigits && x.PhoneType==PhoneType.WkPhone));
				Assert.IsTrue(listPhoneNumbers.Any(x => x.PhoneNumberDigits==wirelessPhoneNumberDigits && x.PhoneType==PhoneType.WirelessPhone));
			}
		}
		#endregion
		#endregion

		#region MergeTwoPatientPointOfNoReturn
		///<summary>Tests that HasIns column updates if we merge insurance into a patient.</summary>
		[TestMethod]
		public void Patients_MergeTwoPatients_NewInsuranceUpdatesHasIns() {
			Patient patTo=PatientT.CreatePatient(lName: "Jonas",fName: "TO");
			Patient patFrom=PatientT.CreatePatientWithInsurance(lName: "Jonas",fName: "FROM");
			Patients.MergeTwoPatients(patTo.PatNum,patFrom.PatNum);
			Assert.AreEqual("I",Patients.GetPat(patTo.PatNum).HasIns);
		}

		///<summary>Tests that when merging two patients with discount plans, it keeps the plan
		///from the original patient.</summary>
		[TestMethod]
		public void Patients_MergeTwoPatients_PatIntoKeepsOriginalDiscountPlan() {
			Patient patTo=PatientT.CreatePatientWithDiscountPlan(lName: "Jonas",fName: "TO");
			long discountPlanNumTo=DiscountPlanSubs.GetDiscountPlanNumForPat(patTo.PatNum);
			Patient patFrom=PatientT.CreatePatientWithDiscountPlan(lName: "Jonas",fName: "FROM");
			Patients.MergeTwoPatients(patTo.PatNum,patFrom.PatNum);
			Assert.AreEqual(discountPlanNumTo,DiscountPlanSubs.GetDiscountPlanNumForPat(patTo.PatNum));
		}

		///<summary>Tests that when merging a patient with a discount plan into a patient
		///with insurance, the patTo keeps insurance and does not add a discount plan.</summary>
		[TestMethod]
		public void Patients_MergeTwoPatients_DiscountPlanDoesNotUpdateIfInsuranceExists() {
			Patient patTo=PatientT.CreatePatientWithInsurance(lName: "Jonas",fName: "TO");
			Patient patFrom=PatientT.CreatePatientWithDiscountPlan(lName: "Jonas",fName: "FROM");
			Patients.MergeTwoPatients(patTo.PatNum,patFrom.PatNum);
			Assert.AreEqual(false,DiscountPlanSubs.HasDiscountPlan(patTo.PatNum));
		}

		///<summary>Tests that when merging a patient with a discount plan into a patient
		///without a discount plan or insurance, the patTo gets the patFrom discount plan.</summary>
		[TestMethod]
		public void Patients_MergeTwoPatients_DiscountPlanUpdatesIfNoExistingPlanOrInsurance() {
			Patient patTo=PatientT.CreatePatient(lName: "Jonas",fName: "TO");
			Patient patFrom=PatientT.CreatePatientWithDiscountPlan(lName: "Jonas",fName: "FROM");
			long discountPlanNumFrom=DiscountPlanSubs.GetDiscountPlanNumForPat(patFrom.PatNum);
			Patients.MergeTwoPatients(patTo.PatNum,patFrom.PatNum);
			Assert.AreEqual(discountPlanNumFrom,DiscountPlanSubs.GetDiscountPlanNumForPat(patTo.PatNum));
		}
		#endregion

		#region GetPatsToChangeStatus
		///<summary>Tests that active patients without TP procedures since date will change to inactive/archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ActivePatientWOTPProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patActive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a TP procedure a day before dateSince. 
			ProcedureT.CreateProcedure(patActive,"D1050",ProcStat.TP,"",50,procDate:dateSince.AddDays(-1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Patient,dateSince,true,false,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patActive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that active patients with TP procedures since date will not change to inactive/archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ActivePatientWTPProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patActive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			Patient patActiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a TP procedure a day after dateSince. 
			ProcedureT.CreateProcedure(patActive,"D1050",ProcStat.TP,"",50,procDate:dateSince.AddDays(1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Patient,dateSince,true,false,false,null);
			//Only blank patient should show
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patActiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that active patients without C procedures since date will change to inactive/archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ActivePatientWOCProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patActive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a C procedure a day before dateSince. 
			ProcedureT.CreateProcedure(patActive,"D1050",ProcStat.C,"",50,procDate:dateSince.AddDays(-1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Patient,dateSince,false,true,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patActive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that active patients with C procedures since date will not change to inactive/archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ActivePatientWCProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patActive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			Patient patActiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a C procedure a day after dateSince. 
			ProcedureT.CreateProcedure(patActive,"D1050",ProcStat.C,"",50,procDate:dateSince.AddDays(1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Patient,dateSince,false,true,false,null);
			//Only the blank patient should be returned
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patActiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that active patients without an appointment since date will change to inactive/archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ActivePatientWOApptSince() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			long provNum=ProviderT.GetFirstProvNum();
			Operatory op=OperatoryT.CreateOperatory();
			Patient patActive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			Appointment apt=AppointmentT.CreateAppointment(patActive.PatNum,dateSince.AddDays(-1),op.OperatoryNum,provNum);
			//Add an appointment a day before dateSince. 
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Patient,dateSince,false,false,true,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patActive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that Active patients with an appointment since date will not change to inactive/archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ActivePatientWApptSince() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			long provNum=ProviderT.GetFirstProvNum();
			Operatory op=OperatoryT.CreateOperatory();
			Patient patActive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			Patient patActiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add an appointment a day after dateSince. 
			Appointment apt=AppointmentT.CreateAppointment(patActive.PatNum,dateSince.AddDays(1),op.OperatoryNum,provNum);
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Patient,dateSince,false,false,true,null);
			//only the blank pat should be returned
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patActiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Inactive patients with TP procedures since date will not change to Active/Archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_InactivePatientWTPProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patInactive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Inactive);
			Patient patInactiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum,patStatus:PatientStatus.Inactive);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a TP procedure a day after dateSince. 
			ProcedureT.CreateProcedure(patInactive,"D1050",ProcStat.TP,"",50,procDate:dateSince.AddDays(1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Inactive,dateSince,true,false,false,null);
			//Only the blank patient should show
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patInactiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Inactive patients without TP procedures since date will change to Active/Archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_InactivePatientWOTPProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patInactive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Inactive);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a TP procedure a day before dateSince. 
			ProcedureT.CreateProcedure(patInactive,"D1050",ProcStat.TP,"",50,procDate: dateSince.AddDays(-1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Inactive,dateSince,true,false,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patInactive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that Inactive patients with C procedures since date will not change to Active/Archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_InactivePatientWCProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patInactive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Inactive);
			Patient patInactiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum,patStatus:PatientStatus.Inactive);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a C procedure a day after dateSince. 
			ProcedureT.CreateProcedure(patInactive,"D1050",ProcStat.C,"",50,procDate: dateSince.AddDays(1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Inactive,dateSince,false,true,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patInactiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Inactive patients without C procedures since date will change to Active/Archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_InactivePatientWOCProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patInactive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Inactive);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a C procedure a day before dateSince. 
			ProcedureT.CreateProcedure(patInactive,"D1050",ProcStat.C,"",50,procDate: dateSince.AddDays(-1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Inactive,dateSince,false,true,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patInactive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that Inactive patients with an appointment since date will not change to Active/Archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_InactivePatientWApptSince() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			long provNum=ProviderT.GetFirstProvNum();
			Operatory op=OperatoryT.CreateOperatory();
			Patient patInactive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Inactive);
			Patient patInactiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum,patStatus:PatientStatus.Inactive);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add an appointment a day after dateSince. 
			Appointment apt=AppointmentT.CreateAppointment(patInactive.PatNum,dateSince.AddDays(1),op.OperatoryNum,provNum);
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Inactive,dateSince,false,false,true,null);
			//Only the blank inactive patient should show
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patInactiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Inactive patients without an appointment since date will change to Active/Archive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_InactivePatientWOApptSince() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			long provNum=ProviderT.GetFirstProvNum();
			Operatory op=OperatoryT.CreateOperatory();
			Patient patInactive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Inactive);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			Appointment apt=AppointmentT.CreateAppointment(patInactive.PatNum,dateSince.AddDays(-1),op.OperatoryNum,provNum);
			//Add an appointment a day before dateSince. 
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Inactive,dateSince,false,false,true,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patInactive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that Archive patients with TP procedures since date will not change to Active/Inactive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ArchivePatientWTPProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patArchive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Archived);
			Patient patArchiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum,patStatus:PatientStatus.Archived);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a TP procedure a day after dateSince. 
			ProcedureT.CreateProcedure(patArchive,"D1050",ProcStat.TP,"",50,procDate: dateSince.AddDays(1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Archived,dateSince,true,false,false,null);
			//Only the blank patient should show
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patArchiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Archive patients without TP procedures since date will change to Active/Inactive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ArchivePatientWOTPProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patArchive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Archived);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a TP procedure a day before dateSince. 
			ProcedureT.CreateProcedure(patArchive,"D1050",ProcStat.TP,"",50,procDate: dateSince.AddDays(-1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Archived,dateSince,true,false,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patArchive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that Archive patients with C procedures since date will not change to Active/Inactive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ArchivePatientWCProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patArchive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Archived);
			Patient patArchiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum,patStatus:PatientStatus.Archived);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a C procedure a day after dateSince. 
			ProcedureT.CreateProcedure(patArchive,"D1050",ProcStat.C,"",50,procDate: dateSince.AddDays(1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Archived,dateSince,false,true,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patArchiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Archive patients without C procedures since date will change to Active/Inactive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ArchivePatientWOCProcsSince() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			long provNum=ProviderT.GetFirstProvNum();
			Patient patArchive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Archived);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add a C procedure a day before dateSince. 
			ProcedureT.CreateProcedure(patArchive,"D1050",ProcStat.C,"",50,procDate: dateSince.AddDays(-1));
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Archived,dateSince,false,true,false,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patArchive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that Archive patients with an appointment since date will not change to Active/Inactive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ArchivePatientWApptSince() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			long provNum=ProviderT.GetFirstProvNum();
			Operatory op=OperatoryT.CreateOperatory();
			Patient patArchive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Archived);
			Patient patArchiveBlank=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_blank",provNum,patStatus:PatientStatus.Archived);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			//Add an appointment a day after dateSince. 
			Appointment apt=AppointmentT.CreateAppointment(patArchive.PatNum,dateSince.AddDays(1),op.OperatoryNum,provNum);
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Archived,dateSince,false,false,true,null);
			//Only the blank inactive patient should show
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patArchiveBlank.PatNum,listPats.First().PatNum);
		}

		///<summary>Tests that Archive patients without an appointment since date will change to Active/Inactive patient.</summary>
		[TestMethod]
		public void Patient_GetPatsToChangeStatus_ArchivePatientWOApptSince() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			long provNum=ProviderT.GetFirstProvNum();
			Operatory op=OperatoryT.CreateOperatory();
			Patient patArchive=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum,patStatus:PatientStatus.Archived);
			DateTime dateSince=DateTime.Now.AddDays(-100);
			Appointment apt=AppointmentT.CreateAppointment(patArchive.PatNum,dateSince.AddDays(-1),op.OperatoryNum,provNum);
			//Add an appointment a day before dateSince. 
			List<Patient> listPats=Patients.GetPatsToChangeStatus(PatientStatus.Archived,dateSince,false,false,true,null);
			Assert.AreEqual(1,listPats.Count);
			Assert.AreEqual(patArchive.PatNum,listPats.FirstOrDefault().PatNum);
		}

		///<summary>Tests that foreign keys were added to the method after adding a new patnum field.</summary>
		[TestMethod]
		public void Patient_MergeTwoPatientPointOfNoReturn_ForeignKeyCheck() {
			//This array contains commented out tables from Patients.StringArrayPatNumForeignKeys array.
			string[] stringArrayPatNumForeignKeysToExclude=new string[] {
				"custreference.PatNum", 
				"discountplansub.PatNum",
				"document.PatNum", 
				"ehrpatient.PatNum",
				"famaging.PatNum",
				"erouting.PatNum",
				"formpat.FormPatNum",
				"medicationpat.MedicationPatNum",
				"oidexternal.IDInternal",
				"orthochart.PatNum",
				"orthochartlog.PatNum",
				"patfield.PatNum",
				"patient.PatNum",
				"patient.Guarantor",
				"patient.PatNumCloneFrom",
				"patientlink.PatNumFrom",
				"patientlink.PatNumTo",
				"patientnote.PatNum",
				"patientrace.PatNum",
				"procmultivisit.PatNum",
				"question.FormPatNum",
				"recall.PatNum",
				"referral.PatNum",
				"screenpat.ScreenPatNum",
				"screen.ScreenPatNum",
				"securitylog.FKey",
				"securitylog.PatNum",
				"task.KeyNum",
				"taskhist.KeyNum",
				"tsitranslog.PatNum",
				"vaccineobs.VaccinePatNum",
				"vaccinepat.VaccinePatNum",
			};
			string dbName=LargeTableHelper.GetCurrentDatabase();
			string command="SELECT CONCAT(TABLE_NAME,'.',COLUMN_NAME) " 
				+"FROM INFORMATION_SCHEMA.columns "
				+$"WHERE table_schema='{dbName}' AND COLUMN_NAME LIKE '%patnum%' "
				+"ORDER BY TABLE_NAME";
			List<string> listTablePatNumFromDb=DataCore.GetList<string>(command,(x) => x.GetString(0));
			List<string> listTablePatNumMissing=new List<string>();
			for(int i=0;i<listTablePatNumFromDb.Count;i++) {
				string tablePatNumCur=listTablePatNumFromDb[i];
				if(Patients.StringArrayPatNumForeignKeys.Contains(tablePatNumCur)
					|| stringArrayPatNumForeignKeysToExclude.Contains(tablePatNumCur)) 
				{
					continue;
				}
				listTablePatNumMissing.Add(tablePatNumCur);
			}
			string message=string.Join($", ",listTablePatNumMissing);
			Assert.AreEqual(0,listTablePatNumMissing.Count,message);
		}
		#endregion

		#region HelperMethods

		///<summary>Calls the Patient S class method.</summary>
		private List<long> GetPatNumsByNameBirthdayEmailAndPhone(string lName,string fName,DateTime birthDate,string email,List<string> listPhones) {
			return Patients.GetPatNumsByNameBirthdayEmailAndPhone(lName,fName,birthDate,email,listPhones);
		}

		///<summary>This method generates three patients and returns them as out variables. The patients have overlapping names, birthdates, emails, 
		///and phones.</summary>
		private void GenerateThreePatients(out Patient patA,out Patient patB,out Patient patA2) {
			patA=PatientT.CreatePatient(fName:"Billy",lName:"Bob");
			Patient oldPat=patA.Copy();
			patA.Birthdate=new DateTime(1971,6,28);
			patA.WirelessPhone="5413635432";
			patA.Email="joe@schmoe.com";
			patA.ClinicNum=1;
			Patients.Update(patA,oldPat);
			patB=PatientT.CreatePatient(fName:"Joe",lName:"Schmoe");
			oldPat=patB.Copy();
			patB.Birthdate=new DateTime(2000,6,28);
			patB.HmPhone="5413631111";
			patB.Email="chuck@schmuck.com";
			patB.Guarantor=patA.PatNum;
			patB.ClinicNum=1;
			Patients.Update(patB,oldPat);
			patA2=PatientT.CreatePatient(fName:"Billy",lName:"Bob");
			oldPat=patA2.Copy();
			patA2.Birthdate=new DateTime(1971,6,28);
			patA2.WirelessPhone="5478982525";
			patA2.Email=patA.Email;
			patA2.ClinicNum=0;
			Patients.Update(patA2,oldPat);
		}
		#endregion

	}
}
