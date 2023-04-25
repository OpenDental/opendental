using System;
using System.Collections.Generic;
using System.Linq;
using OpenDentBusiness.WebTypes.WebForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using OpenDentBusiness.WebTypes;

namespace UnitTests {
	[TestClass]
	public class SheetsTests:TestBase {

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

		/// <summary>
		/// Creates a Sheet from a SheetDef that has no translations, for a patient with None for language.  Default language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_NoTranslations_PatientDefaultLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string>(),patLanguage:"",expectedLanguage:"");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has no translations, for a patient with DeclinedToSpecify for language.  Default language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_NoTranslations_PatientDeclineToSpecifyLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string>(),patLanguage:Patients.LANGUAGE_DECLINED_TO_SPECIFY,expectedLanguage:"");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has no translations, for a patient with Spanish for language.  Default language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_NoTranslations_PatientSpanLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string>(),patLanguage:"spa",expectedLanguage:"");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has spanish translations, for a patient with None for language.  Default language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_SpanishTranslations_PatientDefaultLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string> { "spa" },patLanguage:"",expectedLanguage:"");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has spanish translations, for a patient with DeclineToSpecify for language.  Default language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_SpanishTranslations_PatientDeclineToSpecifyLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string> { "spa" },patLanguage:Patients.LANGUAGE_DECLINED_TO_SPECIFY,expectedLanguage:"");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has spanish translations, for a patient with Spanish for language.  Spanish language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_SpanishTranslations_PatientSpanishLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string> { "spa" },patLanguage:"spa",expectedLanguage:"spa");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has spanish translations, for a patient with German for language.  Default language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_SpanishTranslations_PatientGermanLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string> { "spa" },patLanguage:"ger",expectedLanguage:"");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has spanish and german translations, for a patient with German for language.  German language should be used.
		/// </summary>
		[TestMethod]
		public void Sheets_CreateSheetFromSheetDef_SpanishGermanTranslations_PatientGermanLang() {
			TestSheetDefLanguages(listSheetDefLanguages:new List<string> { "spa","ger" },patLanguage:"ger",expectedLanguage:"ger");
		}

		/// <summary>
		/// Creates a Sheet from a SheetDef that has given translations, for a patient with German for language.  German language should be used.
		/// </summary>
		private void TestSheetDefLanguages(List<string> listSheetDefLanguages,string patLanguage,string expectedLanguage) {
			//Arrange
			Patient pat=PatientT.CreatePatient(language:patLanguage);
			SheetDef sheetDef=CreateSheetDefWithLanguages(listSheetDefLanguages.ToArray());
			List<SheetFieldDef> listExpected=sheetDef.SheetFieldDefs.Where(x => x.Language==expectedLanguage).ToList();
			//Act
			Sheet sheet=Sheets.CreateSheetFromSheetDef(sheetDef,pat.PatNum);
			List<SheetField> listActual=sheet.SheetFields;
			//Assert
			Assert.AreEqual(listExpected.Count,listActual.Count);
			foreach(SheetFieldDef sheetFieldDef in listExpected) {
				//Sadly, a SheetField does not have a SheetFieldDefNum it was sourced from, so we have to get as close as we can.
				SheetField sheetField=listActual.First(x => 
					x.FieldName==sheetFieldDef.FieldName && 
					x.XPos==sheetFieldDef.XPos && 
					x.YPos==sheetFieldDef.YPos &&
					x.FieldType==sheetFieldDef.FieldType);
				Assert.IsNotNull(sheetField);
			}
		}

		private SheetDef CreateSheetDefWithLanguages(params string[] languages) {
			SheetDefT.ClearSheetDefAndSheetFieldDefTable();
			SheetDefs.RefreshCache();
			SheetFieldDefs.RefreshCache();
			SheetDefT.CreateCustomSheet(SheetInternalType.PatientLetter);
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x => x.SheetType==SheetTypeEnum.PatientLetter);
			SheetDefs.GetFieldsAndParameters(sheetDef);
			foreach(string language in languages) {
				List<SheetFieldDef> listSheetFieldDefs=sheetDef.SheetFieldDefs.Where(x => string.IsNullOrWhiteSpace(x.Language)).Select(x => x.Copy()).ToList();
				listSheetFieldDefs.ForEach(x => {
					x.Language=language;
					x.SheetDefNum=sheetDef.SheetDefNum;
				});
				sheetDef.SheetFieldDefs.AddRange(listSheetFieldDefs);
			}
			SheetFieldDefs.Sync(sheetDef.SheetFieldDefs,sheetDef.SheetDefNum);
			SheetFieldDefs.RefreshCache();
			return SheetDefs.GetSheetDef(sheetDef.SheetDefNum);
		}

		/// <summary>
		/// creates a medical history sheet for a patient, who has a variety of diseases, medications, and allergies.
		/// Then creates two sheets from that sheet def, and prefills it with values from the database and from a previously filled out sheet.
		/// Then we make sure that fields from the db are filled with values from the db, and other fields have their values from the previous sheet passed over.
		/// </summary>
		[TestMethod]
		public void Sheets_PrefillSheetFromPreviousAndDatabase_PrefillSheetTest() {
			Patient pat=PatientT.CreatePatient(fName:"dave",lName:"davis",birthDate:new DateTime(2001,2,3), address:"123 nw marietta dr", city:"Salem",state:"Oregon");
			DiseaseDef diseaseDef=DiseaseDefT.CreateDiseaseDef("Heart Murmur");
			Disease disease=DiseaseT.CreateDisease(pat.PatNum, diseaseDefNum:diseaseDef.DiseaseDefNum);
			//Allergies
			AllergyDef allergyDef=AllergyDefT.CreateAllergyDef("Ibuprofen");
			Allergy allergy=new Allergy();
			allergy.PatNum=pat.PatNum;
			allergy.AllergyDefNum=allergyDef.AllergyDefNum;
			allergyDef.Description="Ibuprofen";
			AllergyDefs.Update(allergyDef);
			allergy.AllergyDefNum=allergyDef.AllergyDefNum;
			allergy.StatusIsActive=true;
			Allergies.Insert(allergy);
			var listallergies=Medications.GetListFromDb();
			//Medications
			var medications=Medications.GetList("");
			var medication=MedicationT.CreateMedication("Happy pills");			
			allergyDef.MedicationNum=medication.MedicationNum;
			MedicationPat medpat=MedicationPatT.CreateMedicationPat(pat.PatNum,medicationNum:medication.MedicationNum,dateStop:DateTime.MaxValue,dateStart:new DateTime(2000,1,2));
			medpat.MedDescript="Happy pills";
			MedicationPats.Update(medpat);
			Medications.RefreshCache();
			//Sheets
			SheetDef sheetDef=SheetDefT.CreateCustomSheet(SheetInternalType.MedicalHistNewPat);
			SheetDefs.GetFieldsAndParameters(sheetDef);
			Sheet sheet=Sheets.CreateSheetFromSheetDef(sheetDef, patNum:pat.PatNum);
			Sheet sheetNew;
			List<SheetField> sheetfields=sheet.SheetFields.FindAll(x=>x.FieldName!="" && x.FieldType!=SheetFieldType.CheckBox && x.FieldType!=SheetFieldType.ComboBox);
			int countValue=0; 
			sheetfields.ForEach(x=> {
				x.FieldValue+="value";
				countValue++;
			});
			sheetNew=Sheets.PreFillSheetFromPreviousAndDatabase(sheetDef, sheet);
			List<SheetField> newFields=sheetNew.SheetFields.FindAll(x => x.FieldName!="" && x.FieldType!=SheetFieldType.CheckBox && x.FieldType!=SheetFieldType.ComboBox);
			List<SheetField> allergyFields=sheetNew.SheetFields.FindAll(x => x.FieldName.ToLower().Contains("allergy") 
				|| (x.FieldValue!=null && x.FieldValue.ToLower().Contains("allergy")));
			//assert that these values are pulled from the db as expected
			//lName, fName, birthDate, address, city, state
			string fname=newFields.FirstOrDefault(x => x.FieldName=="FName").FieldValue;
			string lname=newFields.FirstOrDefault(x => x.FieldName=="LName").FieldValue;
			string bday=sheetNew.SheetFields.LastOrDefault(x => x.FieldName=="Birthdate").FieldValue;
			//From pat table
			Assert.AreEqual(fname, pat.FName);
			Assert.AreEqual(lname, pat.LName);
			Assert.AreEqual(bday, pat.Birthdate.ToShortDateString());
			//assert that values are pulled from the existing sheet as expected. (not the DB fields, date, or combo or check boxes, but everything else should match.)
			for(int i = 0;i<sheet.SheetFields.Count;i++) {
				if(sheet.SheetFields[i].FieldName!="FName"
					&&sheet.SheetFields[i].FieldName!="LName"
					&&sheet.SheetFields[i].FieldName!="Birthdate"
					&&sheet.SheetFields[i].FieldName!="Address"
					&&sheet.SheetFields[i].FieldName!="City"
					&&sheet.SheetFields[i].FieldName!="State"
					&&!sheet.SheetFields[i].FieldValue.Contains("Date")
					&&sheet.SheetFields[i].FieldType!=SheetFieldType.CheckBox					
					&&sheet.SheetFields[i].FieldType!=SheetFieldType.ComboBox
					&&!sheet.SheetFields[i].FieldName.Contains("inputMed"))
				Assert.AreEqual(sheet.SheetFields[i].FieldValue, sheetNew.SheetFields[i].FieldValue);
			}
			//Assert that allergies, medications and problems are present from DB
			List<SheetField> listHeartMurumr=sheetNew.SheetFields.FindAll(x => x.FieldName=="problem:Heart Murmur" && x.FieldValue=="X" && x.RadioButtonValue=="Y");
			Assert.AreEqual(1, listHeartMurumr.Count());
			List<SheetField> listMedication=sheetNew.SheetFields.FindAll(x => x.FieldValue!=null && x.FieldValue.Contains("Happy pills") && x.FieldName=="inputMed1");
			Assert.AreEqual(1, listMedication.Count());
			List<SheetField> listAllergies= sheetNew.SheetFields.FindAll(x => x.FieldName.Contains("Ibuprofen") && x.FieldValue=="X" && x.RadioButtonValue=="Y");
			Assert.AreEqual(1, listAllergies.Count);
		}

		///<summary>Turn a WebForms_Sheet into a sheet and check that the sheetFieldDefNums on the WebFormsSheetFieldDefs match.</summary>
		[TestMethod]
		public void SheetUtil_CreateSheetFromWebSheet_SheetFieldDefNums() {
			//Convert a WebForms_sheet into a Sheet, and assert that its sheetFieldDefNums are retained.
			WebForms_Sheet webSheet=WebForms_SheetT.CreateWebFormSheet("Jones", "Dougie",DateTime.Now.AddYears(-20), "email@gmail.com", new List<string> {"1231231234","4564564567","6786786789"});
			Sheet sheetFromWebSheet=SheetUtil.CreateSheetFromWebSheet(0,webSheet);
			for(int i = 0;i<webSheet.SheetFields.Count;i++) {
				Assert.AreEqual(webSheet.SheetFields[i].SheetFieldDefNum,sheetFromWebSheet.SheetFields[i].SheetFieldDefNum);
				Assert.AreEqual(webSheet.SheetFields[i].SheetFieldDefNum,sheetFromWebSheet.SheetFields[i].SheetFieldDefNum);
			}
		}
	}
}
