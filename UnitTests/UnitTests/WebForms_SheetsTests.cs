using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebForms;
using UnitTestsCore;

namespace UnitTests.WebForms_Sheets_Tests {
	[TestClass]
	public class WebForms_SheetsTests:TestBase {

		///<summary>Tests to see that the webforms matching function is working correctly. Tries multiple combinations to prove it is working.</summary>
		[TestMethod]
		public void WebForms_Sheets_FindSheetsForPat_Matching() {
			WebForms_Sheet sheetNoPhones=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,1),"bob@bobby.com",new List<string>());
			WebForms_Sheet sheetNoPhonesMatching=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,1),"bob@bobby.com",
				new List<string>());
			WebForms_Sheet sheetCloseMatch=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",new List<string>());
			WebForms_Sheet sheetPhoneNumber=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",
				new List<string> { "5555555555" });
			WebForms_Sheet sheetPhoneNumberMatching=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",
				new List<string> { "5555555555" });
			WebForms_Sheet sheetTooManyPhones=WebForms_SheetT.CreateWebFormSheet("Bob","Bobby",new DateTime(2018,1,2),"bob@bobby.com",
				new List<string> { "5555555555","4444444444" });
			WebForms_Sheet sheetDifferentName=WebForms_SheetT.CreateWebFormSheet("Bobby","Bobby",new DateTime(2018,1,1),"bob@bobby.com",
				new List<string>());
			//Exact Match.
			Assert.AreEqual(1,WebForms_Sheets.FindSheetsForPat(sheetNoPhones,new List<WebForms_Sheet> { sheetNoPhonesMatching },"en-us").Count);
			//Close except different birthday.
			Assert.AreEqual(0,WebForms_Sheets.FindSheetsForPat(sheetNoPhones,new List<WebForms_Sheet> { sheetCloseMatch },"en-us").Count);
			//Exact Match.
			Assert.AreEqual(1,WebForms_Sheets.FindSheetsForPat(sheetPhoneNumber,new List<WebForms_Sheet> { sheetPhoneNumberMatching },"en-us").Count);
			//One form has more phone numbers.
			Assert.AreEqual(0,WebForms_Sheets.FindSheetsForPat(sheetPhoneNumber,new List<WebForms_Sheet> { sheetTooManyPhones },"en-us").Count);
			//No match. Different name.
			Assert.AreEqual(0,WebForms_Sheets.FindSheetsForPat(sheetTooManyPhones,new List<WebForms_Sheet> { sheetDifferentName },"en-us").Count);
			//A sheet will always match on itself.
			Assert.AreEqual(1,WebForms_Sheets.FindSheetsForPat(sheetDifferentName,new List<WebForms_Sheet> { sheetDifferentName },"en-us").Count);
		}

		///<summary>Iterates through all available cultures and a date that would cause problems with mismatched month/date fields, ensuring that 
		///WebForms_Sheets.parseDateWebForms will return the expected DateTime.  Also uses a variety of different delimiters in between the month, day,
		///and year fields to ensure these are handled as best we can.</summary>
		[TestMethod]
		public void WebForms_Sheets_ParseDateWebForms_AllCulturesWithManyDelimiters() {
			StringBuilder	strBld=new StringBuilder();
			//These are currently unsupported cultures.  No customers currently have their webformspreference.CultureName set to these.
			List<string> listUnsupportedCultureNames=new List<string>() {
				"ar","ar-SA","fa","fa-IR","ku-Arab-IR","lrc","lrc-IR","mzn","mzn-IR","prs","prs-AF","ps","ps-AF","th","th-TH","uz-Arab","uz-Arab-AF" };
			foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures)
				.Where(x => !ListTools.In(x.Name,listUnsupportedCultureNames) && !string.IsNullOrWhiteSpace(x.Name)))
			{
				DateTime dateExpected=new DateTime(2019,12,31);//Use a date that would cause problems if MM and d are mismatched.
				string format=GetDateFormatLikeWebFormsDb(culture.Name,out string delimiterWeb);//The format WebForms will use, and its delimiter.
				foreach(string delimiter in new string[] { ".","/","\\"," ","-","random nonsense"}) {
					//Test this date, using the various delimiters patients might use, plus some that make no sense.
					string strDate=System.Text.RegularExpressions.Regex.Replace(dateExpected.ToString(format),delimiterWeb, delimiter);
					DateTime dateActual=WebForms_Sheets.ParseDateWebForms(strDate,culture.Name);
					Assert.AreEqual(dateExpected.ToShortDateString(),dateActual.ToShortDateString()
						,$"date: {strDate}, format: {format}, culture: {culture.Name}");
				}
			}
			//We don't support this format currently.  Make sure ParseDateWebForms returns MinValue.
			DateTime failDate=WebForms_Sheets.ParseDateWebForms("January 26,2006","en-US");
			Assert.AreEqual(DateTime.MinValue.ToShortDateString(),failDate.ToShortDateString());
			//If the user typed complete nonsense in a birthdate field.  Make sure ParseDateWebForms returns MinValue.
			failDate=WebForms_Sheets.ParseDateWebForms("chicken","en-US");
			Assert.AreEqual(DateTime.MinValue.ToShortDateString(),failDate.ToShortDateString());
			failDate=WebForms_Sheets.ParseDateWebForms("01/01/00","en-US");
			Assert.AreEqual(DateTime.MinValue.ToShortDateString(),failDate.ToShortDateString());
			Assert.IsTrue(string.IsNullOrWhiteSpace(strBld.ToString()),strBld.ToString());
		}

		///<summary>A copy of the code from ODDateHelper.java, which is the method that determines the format WebForms uses to save dates to the HQ WebForms db.
		///C:\Development\Shared Projects Subversion\OpenDentalWebApps\head\OpenDentalWeb\xCore\src\com\od\base\utilODDateHelper.java</summary>
		private static string GetDateFormatLikeWebFormsDb(string cultureName,out string delimiterWeb) {
			if(cultureName==null) {
				cultureName="";
			}
			String pattern="M/d/yyyy";//Default to en-US format just in case we don't currently support the culture passed in.
			delimiterWeb="/";
			if(cultureName.Equals("")
				|| cultureName.Equals("en-US")) 
			{
				pattern="M/d/yyyy";
			}
			else if(cultureName.Equals("ar-JO")
				|| cultureName.Equals("en-CA")
				|| cultureName.Equals("en-GB")
				|| cultureName.Equals("en-ES")
				|| cultureName.Equals("en-MX")
				|| cultureName.Equals("en-PR")) 
			{
				pattern="dd/MM/yyyy";
			}
			else if(cultureName.Equals("da-DK")
				|| cultureName.Equals("en-IN")) 
			{
				pattern="dd-MM-yyyy";
				delimiterWeb="-";
			}
			else if(cultureName.Equals("en-AU")
				|| cultureName.Equals("en-NZ")) 
			{
				pattern="d/MM/yyyy";
			}
			else if(cultureName.Equals("mn-MN")) {
				pattern="yy.MM.dd";
				delimiterWeb="\\.";
			}
			else if(cultureName.Equals("zh-CN")) {
				pattern="yyyy/M/d";
			}
			return pattern;
			}
	}
}
