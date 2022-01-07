using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

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

	}
}
