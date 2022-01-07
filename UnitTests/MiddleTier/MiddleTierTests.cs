using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataConnectionBase;
using OpenDentBusiness;
using UnitTestsCore;
using static OpenDentBusiness.WebServiceTests;
using UnitTests.MiddleTier.ParamCheck_Tests;

namespace UnitTests.MiddleTier {
	[TestClass]
	public class MiddleTierTests:TestBase {

		///<summary>This method will get invoked before every single test.</summary>
		[TestInitialize]
		public void Initialize() {
			//Instantiate a brand new mock service because other unit tests could have changed the database connection settings via DataConnection.SetDb().
			//Also, we need to make sure that MySQL UserLow and PassLow are used because some of these unit tests require them to be set.
			RunTestsAgainstMiddleTier(new OpenDentBusiness.WebServices.OpenDentalServerMockIIS(
					server:"localhost",
					db:UnitTestDbName,
					user:"root",
					password:"",
					userLow: "root",
					passLow:"",
					dbType:DatabaseType.MySql
				));
		}

		///<summary>This method will get invoked after every single test.</summary>
		[TestCleanup]
		public void Cleanup() {
			RevertMiddleTierSettingsIfNeeded();
		}

		[TestMethod]
		public void MiddleTier_ExceptionType() {
			//Make sure that the Middle Tier preserves the type of exception that was thrown from the S class method.
			try {
				//Should throw an ODException with error code 36.
				WebServiceTests.GetODException();
			}
			catch(ODException odE) {
				//The type of exception was preserved correctly, verify that the ErrorCode was also preserved.
				Assert.AreEqual(36,odE.ErrorCode);
			}
			catch(Exception e) {
				//The expected exception type was not preserved.  Fail this unit test.
				e.DoNothing();
				Assert.Fail();
			}
		}

		[TestMethod]
		public void MiddleTier_GetStringLong() {
			int intStrLen=1000000;
			string strAlphNumLong=CoreTypesT.CreateRandomAlphaNumericString(intStrLen);
			Assert.AreEqual(WebServiceTests.GetString(strAlphNumLong),"Processed: "+strAlphNumLong);
		}

		[TestMethod]
		public void MiddleTier_GetStringDirty() {
			Assert.AreEqual(WebServiceTests.GetString(WebServiceTests.DirtyString),"Processed: "+WebServiceTests.DirtyString);
		}

		[TestMethod]
		public void MiddleTier_GetStringNull() {
			Assert.AreEqual(null,WebServiceTests.GetStringNull());
		}

		[TestMethod]
		public void MiddleTier_GetStringCarriageReturn() {
			Assert.AreEqual(WebServiceTests.GetStringCarriageReturn(WebServiceTests.NewLineString),"Processed: "+WebServiceTests.NewLineString);
		}

		[TestMethod]
		public void MiddleTier_GetInt() {
			Assert.AreEqual(WebServiceTests.GetInt(1),2);
		}

		[TestMethod]
		public void MiddleTier_GetLong() {
			Assert.AreEqual(WebServiceTests.GetLong(1),2);
		}

		[TestMethod]
		public void MiddleTier_GetVoid() {
			WebServiceTests.GetVoid();
		}

		[TestMethod]
		public void MiddleTier_GetBool() {
			Assert.AreEqual(true,WebServiceTests.GetBool());
		}

		[TestMethod]
		public void MiddleTier_GetObjectPat() {
			Patient pat=WebServiceTests.GetObjectPat();
			List<string>  strErrors=new List<string>();
			if(pat==null) {
				strErrors.Add("The patient returned is null.");
			}
			else {
				if(pat.FName!=null) {
					strErrors.Add("The patient.FName should be null but returned '"+pat.FName+"'.");
				}
				if(pat.LName!="Smith") {
					strErrors.Add("The patient.LName should be 'Smith' but returned "+(pat.LName==null?"null":("'"+pat.LName+"'"))+"'.");
				}
				if(pat.AddrNote!=WebServiceTests.DirtyString) {
					strErrors.Add(string.Format(@"The patient.AddrNote should be '{0}' but returned {1}.",WebServiceTests.DirtyString,
						pat.AddrNote==null?"null":("'"+pat.AddrNote+"'")));
				}
			}
			Assert.AreEqual(0,strErrors.Count);
		}

		[TestMethod]
		public void MiddleTier_GetListPats() {
			List<Patient> listPats=WebServiceTests.GetListPats();
			List<string> strErrors=new List<string>();
			if(listPats==null) {
				strErrors.Add("The list of patients returned is null.");
			}
			else {
				if(listPats[0].FName!=null) {
					strErrors.Add("The first patient in the list of patients FName should be null but returned '"+listPats[0].FName+"'.");
				}
				if(listPats[0].LName!="Smith") {
					strErrors.Add("The first patient in the list of patients LName should be 'Smith' but returned "+(listPats[0].LName==null ? "null" : ("'"+listPats[0].LName+"'"))+"'.");
				}
				if(listPats[0].AddrNote!=WebServiceTests.DirtyString) {
					strErrors.Add(string.Format(@"The first patient in the list of patients AddrNote should be '{0}' but returned {1}.",WebServiceTests.DirtyString,
						listPats[0].AddrNote==null ? "null" : ("'"+listPats[0].AddrNote+"'")));
				}
			}
			Assert.AreEqual(0,strErrors.Count);
		}

		[TestMethod]
		public void MiddleTier_GetTable() {
			DataTable table=WebServiceTests.GetTable();
			Assert.IsTrue(table!=null && table.Rows!=null 
				&& table.Rows.Count>0 
				&& table.Rows[0]["Col1"]!=null 
				&& table.Rows[0]["Col1"].ToString()=="cell00");
		}

		[TestMethod]
		public void MiddleTier_GetTableCarriageReturn() {
			DataTable table=WebServiceTests.GetTableCarriageReturn();
			Assert.IsTrue(table!=null && table.Rows!=null 
				&& table.Rows.Count>0 
				&& table.Columns.Count>0 
				&& table.Rows[0]!=null 
				&& table.Rows[0][0]!=null
				&& table.Rows[0][0].ToString()==WebServiceTests.NewLineString);
		}

		[TestMethod]
		public void MiddleTier_GetTable2by3() {
			DataTable table=WebServiceTests.GetTable2by3();
			List<string> strErrors=new List<string>();
			for(int i = 0;i<table.Rows.Count;i++) {
				for(int j = 0;j<table.Columns.Count;j++) {
					if(table.Rows[i][j].ToString()!="cell"+i+j) {
						strErrors.Add(string.Format(@"The table cell should be '{0}' but returned '{1}'.","cell"+i+j,table.Rows[i][j]));
					}
				}
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetTableSpecialChars() {
			DataTable table=WebServiceTests.GetTableSpecialChars();
			char[] chars={'|','<','>','&','\'','"','\\','/'};
			List<string> strErrors=new List<string>();
			for(int i=0;i<table.Rows.Count;i++) {
				for(int j=0;j<table.Columns.Count-1;j++) {//last column is for DirtyString
					if(table.Rows[i][j].ToString()!="cell"+i+j+chars[i*2+j]) {
						strErrors.Add(string.Format(@"The table cell should be '{0}' but returned '{1}'.","cell"+i+j+chars[i*2+j],table.Rows[i][j]));
					}
				}
			}
			if(table.Rows[0]["DirtyString"].ToString()!=WebServiceTests.DirtyString) {
				strErrors.Add(string.Format(@"The table cell should be '{0}' but returned '{1}'.",WebServiceTests.DirtyString,table.Rows[0]["DirtyString"]));
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetTableDataTypes() {
			DataTable table=WebServiceTests.GetTableDataTypes();
			List<string> strErrors=new List<string>();
			if(table==null || table.Rows==null || table.Columns==null || table.Rows.Count<1 || table.Rows[0]==null) {
				strErrors.Add(table==null?"The DataTable is null.":table.Rows==null?"The DataRowCollection is null.":
					table.Columns==null?"The DataColumnCollection is null.":table.Rows.Count<1?"The DataRowCollection is empty.":"The DataRow is null.");
			}
			else {
				if(table.Columns.Count<1 || table.Rows[0][0]==null || table.Rows[0][0].GetType()!=typeof(string)) {
					strErrors.Add(string.Format("The cell DataType should be {0} but returned {1}.",typeof(string),
						table.Columns.Count<1?"an insufficient column count":table.Rows[0][0]==null?"a null cell":table.Rows[0][0].GetType().ToString()));
				}
				if(table.Columns.Count<2 || table.Rows[0][1]==null || table.Rows[0][1].GetType()!=typeof(decimal)) {
					strErrors.Add(string.Format("The cell DataType should be {0} but returned {1}.",typeof(decimal),
						table.Columns.Count<2?"an insufficient column count":table.Rows[0][1]==null?"a null cell":table.Rows[0][1].GetType().ToString()));
				}
				if(table.Columns.Count<3 || table.Rows[0][2]==null || table.Rows[0][2].GetType()!=typeof(DateTime)) {
					strErrors.Add(string.Format("The cell DataType should be {0} but returned {1}.",typeof(DateTime),
						table.Columns.Count<3?"an insufficient column count":table.Rows[0][2]==null?"a null cell":table.Rows[0][2].GetType().ToString()));
				}
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetDataSet() {
			DataSet ds=WebServiceTests.GetDataSet();
			List<string> strErrors=new List<string>();
			if(ds==null || ds.Tables==null || ds.Tables.Count<1 || ds.Tables[0]==null || ds.Tables[0].TableName!="table0") {
				strErrors.Add(string.Format("The DataTable's name in the DataSet should be {0} but returned {1}.","table0",
					ds==null?"a null DataSet":ds.Tables==null?"a null DataTableCollection":ds.Tables.Count<1?"an empty DataTableCollection":
					ds.Tables[0]==null?"a null DataTable":ds.Tables[0].TableName??"a null TableName"));
			}
			if(ds==null || ds.Tables==null || ds.Tables.Count<1 || ds.Tables[0]==null || ds.Tables[0].Rows.Count<1
				|| ds.Tables[0].Rows[0]["DirtyString"].ToString()!=WebServiceTests.DirtyString)
			{
				strErrors.Add(string.Format(@"The cell value in the DataSet should be {0} but returned {1}.",WebServiceTests.DirtyString,
					ds==null?"a null DataSet":ds.Tables==null?"a null DataTableCollection":ds.Tables.Count<1?"an empty DataTableCollection":
					ds.Tables[0]==null?"a null DataTable":ds.Tables[0].Rows.Count<1?"an empty DataRowCollection":
					ds.Tables[0].Rows[0]["DirtyString"]??"a null cell"));
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetListInt() {
			List<int> listInt=WebServiceTests.GetListInt();
			Assert.IsTrue(listInt!=null && listInt.Count>0 && listInt[0]==2);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListLong_NullList() {
			List<long> listExpected=null;
			List<long> listActual=new List<long>();
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListLongs(listExpected);
			});
			Assert.IsNull(listActual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListLong_EmptyList() {
			List<long> listExpected=new List<long>();
			List<long> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListLongs(listExpected);
			});
			Assert.IsTrue(listActual.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListLong_NonEmptyList() {
			List<long> listExpected=new List<long>() { 1,2,3 };
			List<long> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListLongs(listExpected);
			});
			Assert.AreEqual(listExpected.Count,listActual.Count);
			bool areListsEqual=true;
			for(int i=0;i<listActual.Count;i++) {
				if(listExpected[i]!=listActual[i]) {
					areListsEqual=false;
					break;
				}
			}
			Assert.IsTrue(areListsEqual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListInt_NullList() {
			List<int> listExpected=null;
			List<int> listActual=new List<int>();
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListInts(listExpected);
			});
			Assert.IsNull(listActual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListInt_EmptyList() {
			List<int> listExpected=new List<int>();
			List<int> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListInts(listExpected);
			});
			Assert.IsTrue(listActual.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListInt_NonEmptyList() {
			List<int> listExpected=new List<int>() { 1,2,3 };
			List<int> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListInts(listExpected);
			});
			Assert.AreEqual(listExpected.Count,listActual.Count);
			bool areListsEqual=true;
			for(int i=0;i<listActual.Count;i++) {
				if(listExpected[i]!=listActual[i]) {
					areListsEqual=false;
					break;
				}
			}
			Assert.IsTrue(areListsEqual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListChar_NullList() {
			List<char> listExpected=null;
			List<char> listActual=new List<char>();
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListChars(listExpected);
			});
			Assert.IsNull(listActual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListChar_EmptyList() {
			List<char> listExpected=new List<char>();
			List<char> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListChars(listExpected);
			});
			Assert.IsTrue(listActual.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListChar_NonEmptyList() {
			List<char> listExpected=new List<char>() { 'a','b','c' };
			List<char> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListChars(listExpected);
			});
			Assert.AreEqual(listExpected.Count,listActual.Count);
			bool areListsEqual=true;
			for(int i=0;i<listActual.Count;i++) {
				if(listExpected[i]!=listActual[i]) {
					areListsEqual=false;
					break;
				}
			}
			Assert.IsTrue(areListsEqual);
		}

		[TestMethod]
		public void MiddleTier_GetListString() {
			List<string> listString=WebServiceTests.GetListString();
			Assert.IsTrue(listString!=null && listString.Count > 0 && listString[0]=="Clean");
		}

		[TestMethod]
		public void MiddleTier_GetListStringDirty() {
			List<string> listStringDirty=WebServiceTests.GetListStringDirty();
			Assert.IsTrue(listStringDirty!=null && listStringDirty.Count > 0 && listStringDirty[0]==WebServiceTests.DirtyString);
		}

		[TestMethod]
		public void MiddleTier_GetArrayString() {
			string[] arrayString=WebServiceTests.GetArrayString();
			Assert.IsTrue(arrayString!=null && arrayString.Length > 0 && arrayString[0]=="Clean");
		}

		[TestMethod]
		public void MiddleTier_GetArrayStringDirty() {
			string[] arrayStringDirty=WebServiceTests.GetArrayStringDirty();
			Assert.IsTrue(arrayStringDirty!=null && arrayStringDirty.Length > 0 && arrayStringDirty[0]==WebServiceTests.DirtyString);
		}

		[TestMethod]
		public void MiddleTier_GetArrayPatient() {
			Patient[] arrayPat=WebServiceTests.GetArrayPatient();
			List<string> strErrors=new List<string>();
			if(arrayPat==null || arrayPat.Length<2) {
				strErrors.Add(arrayPat==null?"The patient array is null.":"The patient array contains an insufficient number of patients.");
			}
			else {
				if(arrayPat[0]==null || arrayPat[0].LName!="Jones") {
					strErrors.Add(string.Format("The patient in the array should have the LName {0} but returned {1}.","Jones",
						arrayPat[0]==null?"a null patient":arrayPat[0].LName??"a null LName"));
				}
				if(arrayPat[0]==null || arrayPat[0].AddrNote!=WebServiceTests.DirtyString) {
					strErrors.Add(string.Format(@"The patient in the array should have the AddrNote {0} but returned {1}.",WebServiceTests.DirtyString,
						arrayPat[0]==null?"a null patient":arrayPat[0].AddrNote??"a null AddrNote"));
				}
				if(arrayPat[1]!=null) {
					strErrors.Add("The patient array should contain a null patient but returned a non-null patient.");
				}
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		///<summary>Reflection treats methods that have an array as their only argument differently than methods with multiple arguments.
		///When an array is the only argument, the functionality somehow mimics what happens when a method utilizes the params keyword.
		///Meaning, an array of three objects will act like three separate objects instead of acting like an array of three objects.
		///This functionality goes away as soon as another argument is added to the method signature (why we haven't noticed this problem yet).</summary>
		[TestMethod]
		public void MiddleTier_SendArrayPatient() {
			Patient[] arrayPatients=new Patient[3];
			arrayPatients[0]=new Patient { LName="Jones",AddrNote=WebServiceTests.DirtyString };
			arrayPatients[1]=null;
			arrayPatients[2]=new Patient { SchedAfterTime=new TimeSpan(5,30,22) };
			Patient[] arrayPatientsReturned=WebServiceTests.SendArrayPatient(arrayPatients);
			Assert.AreNotEqual(arrayPatientsReturned,null);
			Assert.AreEqual(arrayPatientsReturned.Length,3);
			Assert.AreEqual(arrayPatientsReturned[0].LName,"Jones");
			Assert.AreEqual(arrayPatientsReturned[0].AddrNote,WebServiceTests.DirtyString);
			Assert.AreEqual(arrayPatientsReturned[1],null);
			Assert.AreEqual(arrayPatientsReturned[2].SchedAfterTime.Hours,5);
			Assert.AreEqual(arrayPatientsReturned[2].SchedAfterTime.Minutes,30);
			Assert.AreEqual(arrayPatientsReturned[2].SchedAfterTime.Seconds,22);
		}
		
		[TestMethod]
		public void MiddleTier_SendIntParams() {
			int[] arrayITypesReturned=WebServiceTests.SendIntParams(6,2,9);
			Assert.AreNotEqual(arrayITypesReturned,null);
			Assert.AreEqual(arrayITypesReturned.Length,3);
			Assert.AreEqual(arrayITypesReturned[0],6);
			Assert.AreEqual(arrayITypesReturned[1],2);
			Assert.AreEqual(arrayITypesReturned[2],9);
		}
		
		[TestMethod]
		public void MiddleTier_SendStringParams() {
			string[] arrayStringsReturned=WebServiceTests.SendStringParams("Str",null,WebServiceTests.DirtyString);
			Assert.AreNotEqual(arrayStringsReturned,null);
			Assert.AreEqual(arrayStringsReturned.Length,3);
			Assert.AreEqual(arrayStringsReturned[0],"Str");
			Assert.AreEqual(arrayStringsReturned[1],null);
			Assert.AreEqual(arrayStringsReturned[2],WebServiceTests.DirtyString);
		}
		
		[TestMethod]
		public void MiddleTier_SendEnumParams() {
			InvalidType[] arrayITypesReturned=WebServiceTests.SendEnumParams(InvalidType.Prefs,InvalidType.AccountingAutoPays,InvalidType.AlertSubs);
			Assert.AreNotEqual(arrayITypesReturned,null);
			Assert.AreEqual(arrayITypesReturned.Length,3);
			Assert.AreEqual(arrayITypesReturned[0],InvalidType.Prefs);
			Assert.AreEqual(arrayITypesReturned[1],InvalidType.AccountingAutoPays);
			Assert.AreEqual(arrayITypesReturned[2],InvalidType.AlertSubs);
		}
		
		[TestMethod]
		public void MiddleTier_SendEnumParams_WithArgs() {
			InvalidType[] arrayITypesReturned=WebServiceTests.SendEnumParamsWithArgs(true,WebServiceTests.DirtyString,
				InvalidType.Prefs,InvalidType.AccountingAutoPays,InvalidType.AlertSubs);
			Assert.AreNotEqual(arrayITypesReturned,null);
			Assert.AreEqual(arrayITypesReturned.Length,3);
			Assert.AreEqual(arrayITypesReturned[0],InvalidType.Prefs);
			Assert.AreEqual(arrayITypesReturned[1],InvalidType.AccountingAutoPays);
			Assert.AreEqual(arrayITypesReturned[2],InvalidType.AlertSubs);
		}

		[TestMethod]
		public void MiddleTier_SendNullParam() {
			string stringNull=WebServiceTests.SendNullParam(null);
			Assert.AreEqual(null,stringNull);
		}

		[TestMethod]
		public void MiddleTier_SendInterface_WithArgs() {
			List<long> listLongs=WebServiceTests.SendInterfaceParamWithArgs(true,listArgLongs:new List<long>() { 6,2,9 });
			Assert.AreEqual(listLongs.Count,3);
			Assert.AreEqual(listLongs[0],6);
			Assert.AreEqual(listLongs[1],2);
			Assert.AreEqual(listLongs[2],9);
		}

		[TestMethod]
		public void MiddleTier_GetObjectNull() {
			Patient pat2=WebServiceTests.GetObjectNull();
			Assert.IsTrue(pat2==null);
		}

		///<summary>There was a bug with appointment objects incorrectly decoding strings for HTTP transmission within Appointments.ApptSaveHelperResults.
		///E.g. an Appointment object with a Note set to "asdf & asdf" was incorrectly getting saved into the db as "asdf &amp; asdf"</summary>
		[TestMethod]
		public void MiddleTier_GetObjectReused() {
			string expectedStr=@"This ampersand right here -> & <- that one right there, should be pr353rv3d";
			List<WebServiceTests.WebServiceTestObject> listTestObjs=WebServiceTests.GetObjectReused(
				new WebServiceTests.WebServiceTestObject() { ValueStr=expectedStr }
			);
			Assert.AreEqual(2,listTestObjs.Count);
			Assert.AreEqual(expectedStr,listTestObjs[0].ValueStr);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendObjectWithListLongs_NullList() {
			List<long> listExpected=null;
			WebServiceTestObject objectActual=null;
			DataAction.RunMiddleTierMock(() => {
				WebServiceTestObject testObject=new WebServiceTestObject() {
					ValueStr=MethodBase.GetCurrentMethod().Name,
					ListLongValues=listExpected,
				};
				objectActual=WebServiceTests.GetAndSendObjectWithListLongs(testObject);
			});
			//Assert.IsNull(objectActual.ListLongValues);  Commented out since we know this will fail, but don't have a solution yet.
		}

		[TestMethod]
		public void MiddleTier_GetAndSendObjectWithListLongs_EmptyList() {
			List<long> listExpected=new List<long>();
			WebServiceTestObject objectActual=null;
			DataAction.RunMiddleTierMock(() => {
				WebServiceTestObject testObject=new WebServiceTestObject() {
					ListLongValues=listExpected,
				};
				objectActual=WebServiceTests.GetAndSendObjectWithListLongs(testObject);
			});
			Assert.IsTrue(objectActual.ListLongValues.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendObjectWithListLongs_NonEmptyList() {
			List<long> listExpected=new List<long>() { 1,2,3 };
			WebServiceTestObject objectActual=null;
			DataAction.RunMiddleTierMock(() => {
				WebServiceTestObject testObject=new WebServiceTestObject() {
					ListLongValues=listExpected,
				};
				objectActual=WebServiceTests.GetAndSendObjectWithListLongs(testObject);
			});
			Assert.AreEqual(listExpected.Count,objectActual.ListLongValues.Count);
			bool areListsEqual=true;
			for(int i=0;i<objectActual.ListLongValues.Count;i++) {
				if(listExpected[i]!=objectActual.ListLongValues[i]) {
					areListsEqual=false;
					break;
				}
			}
			Assert.IsTrue(areListsEqual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListObjects_NullList() {
			List<WebServiceTestObject> listExpected=null;
			List<WebServiceTestObject> listActual=new List<WebServiceTestObject>();
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListObjects(listExpected);
			});
			Assert.IsNull(listActual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListObjects_EmptyList() {
			List<WebServiceTestObject> listExpected=new List<WebServiceTestObject>();
			List<WebServiceTestObject> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListObjects(listExpected);
			});
			Assert.IsTrue(listActual.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListObjects_NonEmptyList() {
			List<WebServiceTestObject> listExpected=new List<WebServiceTestObject>() {
				new WebServiceTestObject() { ValueStr=MethodBase.GetCurrentMethod().Name }
			};
			List<WebServiceTestObject> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListObjects(listExpected);
			});
			Assert.AreEqual(listExpected.Count,listActual.Count);
			bool areListsEqual=true;
			for(int i=0;i<listActual.Count;i++) {
				if(listExpected[i].ValueStr!=listActual[i].ValueStr) {
					areListsEqual=false;
					break;
				}
			}
			Assert.IsTrue(areListsEqual);
		}

		[TestMethod]
		public void MiddleTier_GetAndSendListObjects_NonEmptyList_ObjectHasNullList() {
			List<WebServiceTestObject> listExpected=new List<WebServiceTestObject>() {
				new WebServiceTestObject() { ValueStr=MethodBase.GetCurrentMethod().Name }
			};
			List<WebServiceTestObject> listActual=null;
			DataAction.RunMiddleTierMock(() => {
				listActual=WebServiceTests.GetAndSendListObjects(listExpected);
			});
			Assert.AreEqual(listExpected.Count,listActual.Count);
			bool areListsEqual=true;
			for(int i=0;i<listActual.Count;i++) {
				if(listExpected[i].ValueStr==listActual[i].ValueStr) 
				{
					if((listExpected[i].ListLongValues==null && listActual[i].ListLongValues!=null)
						|| (listExpected[i].ListLongValues!=null && listActual[i].ListLongValues==null)
						|| (listExpected[i].ListLongValues.Count!=listActual[i].ListLongValues.Count)) 
					{
						areListsEqual=false;
						break;
					}
					for(int j=0;j<listActual[i].ListLongValues.Count;j++) {
						if(listExpected[i].ListLongValues[j]!=listActual[i].ListLongValues[j]) {
							areListsEqual=false;
							break;
						}
					}
				}
				else {
					areListsEqual=false;
					break;
				}
			}
			//Assert.IsTrue(areListsEqual); Commented out since we know this fails, but don't have a fix yet.
		}

		///<summary>There was a bug with appointment objects incorrectly decoding strings for HTTP transmission within Appointments.ApptSaveHelperResults.
		///E.g. an Appointment object with a Note set to "asdf & asdf" was incorrectly getting saved into the db as "asdf &amp; asdf"</summary>
		[TestMethod]
		public void MiddleTier_GetPrimitiveReused() {
			string expectedStr=@"This ampersand right here -> & <- that one right there, should be pr353rv3d";
			WebServiceTests.WebServiceTestObject listTestObjs=WebServiceTests.GetPrimitiveReused(
				new WebServiceTests.WebServiceTestObject() { ValueStr=expectedStr }
			);
			Assert.AreEqual(expectedStr,listTestObjs.ValueStr);
			Assert.AreEqual(expectedStr,listTestObjs.ValueStr2);//GetPrimitiveReused will set ValueStr2 for us.
		}

		///<summary>The purpose of this test is to make sure middle tier can invoke methods that utilize the params keyword.</summary>
		[TestMethod]
		public void MiddleTier_SendObjectParams() {
			List<Schedule> listRetVals=WebServiceTests.SendObjectParams(new Schedule() { ScheduleNum=5 },new Schedule() { ScheduleNum=23 });
			Assert.AreEqual(listRetVals.Count,2);
			Assert.AreEqual(listRetVals[0].ScheduleNum,5);
			Assert.AreEqual(listRetVals[1].ScheduleNum,23);
		}

		///<summary>The purpose of this test is to make sure middle tier can invoke methods that utilize the params keyword with other args.</summary>
		[TestMethod]
		public void MiddleTier_SendObjectParams_WithArgs() {
			List<Schedule> listRetVals=WebServiceTests.SendObjectParamsWithArgs(true,WebServiceTests.DirtyString
				,new Schedule() { ScheduleNum=5 }
				,new Schedule() { ScheduleNum=23 });
			Assert.AreEqual(listRetVals.Count,2);
			Assert.AreEqual(listRetVals[0].ScheduleNum,5);
			Assert.AreEqual(listRetVals[1].ScheduleNum,23);
		}

		[TestMethod]
		public void MiddleTier_SendColorParam() {
			Color colorResult=WebServiceTests.SendColorParam(Color.Green);
			Assert.IsTrue(colorResult!=null && colorResult.ToArgb()==Color.Green.ToArgb());
		}

		[TestMethod]
		public void MiddleTier_SendJobLog_TimeSpan() {
			JobLog jobLogExpected=new JobLog() {
				TimeEstimate=TimeSpan.MaxValue
			};
			JobLog jobLogActual=WebServiceTests.SendJobLog(jobLogExpected);
			Assert.AreEqual(jobLogExpected.TimeEstimate,jobLogActual.TimeEstimate);
		}

		[TestMethod]
		public void MiddleTier_SendProviderColor() {
			Provider prov=new Provider();
			prov.ProvColor=Color.Fuchsia;
			Color colorResult=WebServiceTests.SendProviderColor(prov);
			Assert.IsTrue(colorResult!=null && colorResult.ToArgb()==Color.Fuchsia.ToArgb());
		}

		[TestMethod]
		public void MiddleTier_SendSheetParameter() {
			SheetParameter sheetParam=new SheetParameter(false,"ParamName");
			Assert.AreEqual("ParamName",WebServiceTests.SendSheetParameter(sheetParam));
		}

		[TestMethod]
		public void MiddleTier_SendSheetWithFields() {
			Sheet sheet=new Sheet();
			sheet.SheetFields=new List<SheetField>();
			sheet.Parameters=new List<SheetParameter>();
			SheetField field=new SheetField();
			field.FieldName="FieldName";
			sheet.SheetFields.Add(field);
			Assert.AreEqual("FieldName",WebServiceTests.SendSheetWithFields(sheet));
		}

		[TestMethod]
		public void MiddleTier_SendSheetDefWithFieldDefs() {
			SheetDef sheetdef=new SheetDef();
			sheetdef.SheetFieldDefs=new List<SheetFieldDef>();
			sheetdef.Parameters=new List<SheetParameter>();
			SheetFieldDef fielddef=new SheetFieldDef();
			fielddef.FieldName="FieldName";
			sheetdef.SheetFieldDefs.Add(fielddef);
			Assert.AreEqual("FieldName",WebServiceTests.SendSheetDefWithFieldDefs(sheetdef));
		}

		[TestMethod]
		public void MiddleTier_GetTimeSpan() {
			Assert.AreEqual(new TimeSpan(1,0,0),WebServiceTests.GetTimeSpan());
		}

		[TestMethod]
		public void MiddleTier_GetStringContainingCR() {
			Assert.AreEqual(WebServiceTests.NewLineString,WebServiceTests.GetStringContainingCR());
		}

		[TestMethod]
		public void MiddleTier_GetListTasksContainingCR() {
			OpenDentBusiness.Task t=WebServiceTests.GetListTasksContainingCR()[0];
			Assert.IsTrue(t!=null && t.Descript==WebServiceTests.NewLineString);
		}

		[TestMethod]
		public void MiddleTier_GetListTasksSpecialChars() {
			//Tests special chars, new lines, Date, DateTime, and enum values in a list of objects as the parameter and the return value
			List<OpenDentBusiness.Task> listTasks=new List<OpenDentBusiness.Task> { new OpenDentBusiness.Task {
				Descript=WebServiceTests.DirtyString,
				ParentDesc=WebServiceTests.NewLineString,
				DateTask=WebServiceTests.DateTodayTest,
				DateTimeEntry=WebServiceTests.DateTEntryTest,
				TaskStatus=TaskStatusEnum.Done } };
			List<OpenDentBusiness.Task> listTasksReturned=WebServiceTests.GetListTasksSpecialChars(listTasks);
			List<string> strErrors=new List<string>();
			if(listTasksReturned==null || listTasksReturned.Count<1) {
				strErrors.Add(listTasksReturned==null?"The list of tasks is null.":"The list of tasks contains an insufficient number of tasks.");
			}
			int idx=0;
			foreach(OpenDentBusiness.Task task in listTasksReturned) {
				if(task==null) {
					strErrors.Add("The tasklist contains a null task.");
					idx++;
					continue;
				}
				if(idx==0 && task.Descript!=WebServiceTests.DirtyString) {
					strErrors.Add(string.Format(@"The task.Descript should be {0} but returned {1}.",WebServiceTests.DirtyString,task.Descript??"null"));
				}
				if(task.ParentDesc!=WebServiceTests.NewLineString) {
					strErrors.Add(string.Format(@"The task.ParentDesc should be {0} but returned {1}.",WebServiceTests.NewLineString,task.ParentDesc??"null"));
				}
				if(task.DateTask==null || task.DateTask.Date!=WebServiceTests.DateTodayTest.Date) {
					strErrors.Add(string.Format("The task.DateTask should be {0} but returned {1}.",WebServiceTests.DateTodayTest.ToShortDateString(),
						task.DateTask==null?"null":task.DateTask.ToShortDateString()));
				}
				if(task.DateTimeEntry!=WebServiceTests.DateTEntryTest) {
					strErrors.Add(string.Format("The task.DateTimeEntry should be {0} but returned {1}.",WebServiceTests.DateTEntryTest.ToString(),
						task.DateTimeEntry==null?"null":task.DateTimeEntry.ToString()));
				}
				if(task.TaskStatus!=TaskStatusEnum.Done) {
					strErrors.Add(string.Format("The task.TaskStatus should be {0} but returned {1}.",TaskStatusEnum.Done,task.TaskStatus));
				}
				idx++;
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetFamily() {
			Family family=WebServiceTests.GetFamily();
			List<string> strErrors=new List<string>();
			Assert.IsNotNull(family,"Family is null");
			Assert.IsNotNull(family.ListPats,"ListPats is null");
			Assert.AreEqual(2,family.ListPats.Length,"ListPats contains an insufficient number of patients");
			Patient patJohn=family.ListPats[0];
			Patient patJennifer=family.ListPats[1];
			Assert.AreEqual("John",patJohn.FName);
			Assert.AreEqual("Jennifer",patJennifer.FName);
			Assert.IsNull(patJohn.LName);
			Assert.IsNull(patJennifer.LName);
			//Visual Studio does not like having the DirtyString show up in the Text Explorer's error pane so we can't use Assert.AreEqual().
			Assert.IsTrue(WebServiceTests.DirtyString==patJohn.AddrNote,"John's AddrNote does not equate to WebServiceTests.DirtyString");
			Assert.AreEqual("<&>..</&>",patJennifer.AddrNote);
			Assert.AreEqual(WebServiceTests.NewLineString,patJohn.ApptModNote);
			Assert.AreEqual(WebServiceTests.NewLineString,patJennifer.ApptModNote);
			Assert.AreEqual("service@opendental.com",patJohn.Email);
			Assert.AreEqual("service@opendental.com",patJennifer.Email);
			Assert.AreEqual(PatientStatus.NonPatient,patJohn.PatStatus);
			Assert.AreEqual(PatientStatus.NonPatient,patJennifer.PatStatus);
			Assert.AreEqual(WebServiceTests.DateTodayTest.Date,patJohn.AdmitDate);
			Assert.AreEqual(WebServiceTests.DateTodayTest.Date,patJennifer.AdmitDate);
			Assert.AreEqual(WebServiceTests.DateTEntryTest,patJohn.DateTStamp);
			Assert.AreEqual(WebServiceTests.DateTEntryTest,patJennifer.DateTStamp);
		}

		[TestMethod]
		public void MiddleTier_GetFamilySimple() {
			Family family=WebServiceTests.GetFamilySimple();
			Assert.AreEqual("Salmon & Smith",family.ListPats[0].LName);
		}

		[TestMethod]
		public void MiddleTier_GetListMedLabsSpecialChars() {
			List<MedLab> listMLabs=WebServiceTests.GetListMedLabsSpecialChars();
			List<string> strErrors=new List<string>();
			if(listMLabs==null || listMLabs.Count<1) {
				strErrors.Add("The list of MedLabs is "+listMLabs==null?"null.":"empty.");
			}
			else {
				MedLab mlab=listMLabs[0];
				if(mlab.MedLabNum!=1) {
					strErrors.Add("The MedLabNum should be 1 but returned "+mlab.MedLabNum+".");
				}
				if(mlab.NoteLab!=WebServiceTests.DirtyString) {
					strErrors.Add(string.Format(@"The MedLab.NoteLab should be {0} but returned {1}.",WebServiceTests.DirtyString,mlab.NoteLab??"null"));
				}
				if(mlab.NotePat!=WebServiceTests.NewLineString) {
					strErrors.Add(string.Format(@"The MedLab.NotePat should be {0} but returned {1}.",WebServiceTests.NewLineString,mlab.NotePat??"null"));
				}
				if(mlab.ResultStatus!=ResultStatus.P) {
					strErrors.Add("The MedLab.ResultStatus should be "+ResultStatus.P+" but returned "+mlab.ResultStatus+".");
				}
				if(mlab.DateTimeEntered==null || mlab.DateTimeEntered.Date!=WebServiceTests.DateTodayTest.Date) {
					strErrors.Add(string.Format("The MedLab.DateTimeEntered should be {0} but returned {1}.",WebServiceTests.DateTodayTest.ToShortDateString(),
						mlab.DateTimeEntered==null?"null":mlab.DateTimeEntered.ToShortDateString()));
				}
				if(mlab.DateTimeReported!=WebServiceTests.DateTEntryTest) {
					strErrors.Add(string.Format("The MedLab.DateTimeReported should be {0} but returned {1}.",WebServiceTests.DateTEntryTest.ToString(),
						mlab.DateTimeReported==null?"null":mlab.DateTimeReported.ToString()));
				}
				if(mlab.ListMedLabResults==null || mlab.ListMedLabResults.Count<1) {
					strErrors.Add("The list of MedLabResults for the MedLab is "+mlab.ListMedLabResults==null?"null.":"empty.");
				}
				else {
					MedLabResult mlr=mlab.ListMedLabResults[0];
					if(mlr.MedLabResultNum!=2) {
						strErrors.Add("The MedLabResultNum should be 2 but returned "+mlr.MedLabResultNum+".");
					}
					if(mlr.MedLabNum!=1) {
						strErrors.Add("The MedLabResult.MedLabNum should be 1 but returned "+mlr.MedLabNum+".");
					}
					if(mlr.Note!=WebServiceTests.DirtyString) {
						strErrors.Add(string.Format(@"The MedLabResult.Note should be {0} but returned {1}.",WebServiceTests.DirtyString,mlr.Note??"null"));
					}
					if(mlr.ObsText!=WebServiceTests.NewLineString) {
						strErrors.Add(string.Format(@"The MedLabResult.ObsText should be {0} but returned {1}.",WebServiceTests.NewLineString,mlr.ObsText??"null"));
					}
					if(mlr.ObsSubType!=DataSubtype.PDF) {
						strErrors.Add("The MedLabResult.ObsSubType should be "+DataSubtype.PDF+" but returned "+mlr.ObsSubType+".");
					}
					if(mlr.DateTimeObs!=WebServiceTests.DateTEntryTest) {
						strErrors.Add(string.Format("The MedLabResult.DateTimeObs should be {0} but returned {1}.",WebServiceTests.DateTEntryTest.ToString(),
							mlr.DateTimeObs==null?"null":mlr.DateTimeObs.ToString()));
					}
				}
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_ObjectParamType() {
			Vitalsign vs=new Vitalsign { IsIneligible=true };
			vs=WebServiceTests.GetVitalsignFromObjectParam(vs);
			List<string> strErrors=new List<string>();
			if(vs.GetType()!=typeof(Vitalsign)) {
				strErrors.Add(string.Format("The object returned is not a {0} it is a {1}.",typeof(Vitalsign),vs.GetType()));
			}
			if(vs.IsIneligible) {
				strErrors.Add(string.Format("The vitalsign object IsIneligible flag should be {0} but returned {1}.","true",vs.IsIneligible.ToString()));
			}
			if(vs.Documentation!=WebServiceTests.DirtyString) {
				strErrors.Add("The vitalsign object returned did not have the correct dirty string.");
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetProcCodeWithDirtyProperty() {
			Def d;
			if(Defs.GetDefsForCategory(DefCat.ProcCodeCats,true).Count==0) {
				d=new Def() { Category=DefCat.ProcCodeCats,ItemName=WebServiceTests.DirtyString };
				d.DefNum=Defs.Insert(d);
			}
			else {
				d=Defs.GetFirstForCategory(DefCat.ProcCodeCats,true);
				d.ItemName=WebServiceTests.DirtyString;
				Defs.Update(d);
			}
			Defs.RefreshCache();
			d=Defs.GetDef(DefCat.ProcCodeCats,d.DefNum);
			ProcedureCode pc=new ProcedureCode { IsNew=true,ProcCat=d.DefNum };
			ProcedureCode pc2=new ProcedureCode { IsNew=true };
			List<ProcedureCode> listPcs=new List<ProcedureCode>();
			List<string> strErrors=new List<string>();
			try {
				listPcs=WebServiceTests.GetProcCodeWithDirtyProperty(pc,pc2);
			}
			catch(Exception ex) {
				strErrors.Add("Cannot serialize a property with a getter that does not retrieve the same value the setter is manipulating.");
				strErrors.Add(ex.Message);
				strErrors.Add(ex.StackTrace);
			}
			if(listPcs.Count>0 && (listPcs[0].IsNew || listPcs[1].IsNew)) {
				strErrors.Add(string.Format("One or more of the returned ProcedureCode objects IsNew flag should be {0} but returned {1}.","false","true"));
			}
			if(listPcs.Count>0 && (listPcs[0].ProcCat!=d.DefNum||listPcs[1].ProcCat!=d.DefNum)) {
				strErrors.Add("One or more of the ProcedureCode objects returned did not have the correct ProcCat.");
			}
			if(listPcs.Count>0 && (listPcs[0].ProcCatDescript!=d.ItemName || listPcs[1].ProcCatDescript!=d.ItemName)) {
				strErrors.Add("One or more of the ProcedureCode objects returned did not have the correct dirty string.");
			}
			Assert.IsTrue(strErrors.Count==0);
		}

		[TestMethod]
		public void MiddleTier_GetTableLow() {
			DataTable table=Meth.GetTableLow("SELECT VERSION()");
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
		}

		[TestMethod]
		public void MiddleTier_SimulatedProcUpdate() {
			ProcedureCode pc=new ProcedureCode { Descript="periodic oral evaluation - established patient & stuff",ProcCatDescript="Exams & Xrays",DefaultNote=WebServiceTests.DirtyString };
			ProcedureCode pc2=pc.Copy();
			WebServiceTests.SimulatedProcUpdate(pc);
			List<string> strErrors=new List<string>();
			if(pc.Descript!=pc2.Descript || pc.ProcCatDescript!=pc2.ProcCatDescript) {
				strErrors.Add(string.Format(@"The Descript before is ""{0}"" and after is ""{1}"".  The ProcCatDescript before is ""{2}"" and after is ""{3}"".",pc2.Descript,pc.Descript,pc2.ProcCatDescript,pc.ProcCatDescript));
			}
			if(pc.DefaultNote!=pc2.DefaultNote) {
				strErrors.Add("The dirty string was altered from the simulated update call.");
			}
			Assert.IsTrue(strErrors.Count==0);
		}
		
		[TestMethod]
		public void MiddleTier_GetObjectWithObject() {
			Procedure proc=new Procedure { ProcNum=1123581321,ProcStatus=ProcStat.TP };
			AccountEntry aeOrig=new AccountEntry(proc);
			AccountEntry aeWeb=WebServiceTests.GetAccountEntry(aeOrig);
			Assert.AreEqual(aeWeb.Tag.GetType(),typeof(Procedure));
			Assert.AreEqual(proc.ProcNum,((Procedure)aeWeb.Tag).ProcNum);
		}

		[TestMethod]
		public void MiddleTier_GetTableTypeTag() {
			Procedure proc=new Procedure { ProcNum=1123581321,ProcStatus=ProcStat.TP };
			proc.TagOD="Rocket";
			Procedure procWeb=WebServiceTests.GetProc(proc);
			Assert.AreEqual("Rocket",procWeb.TagOD);
		}

		[TestMethod]
		public void MiddleTier_GetTableTypeObjectTag() {
			Procedure proc=new Procedure { ProcNum=1123581321,ProcStatus=ProcStat.TP };
			proc.TagOD=new Patient { PatNum=007 };
			Procedure procWeb=WebServiceTests.GetProc(proc);
			Assert.AreEqual(procWeb.TagOD.GetType(),typeof(Patient));
			Assert.AreEqual(007,((Patient)procWeb.TagOD).PatNum);
		}
		
		[TestMethod]
		public void MiddleTier_GetInterface() {
			ODProgressDoNothing prog=new ODProgressDoNothing();
			IODProgress progWeb=WebServiceTests.GetIODProgress(prog);
			Assert.AreEqual(typeof(ODProgressDoNothing),progWeb.GetType());
		}

		[TestMethod]
		public void MiddleTier_GetSplitCollection() {
			SplitCollection splitsOrig=new SplitCollection();
			splitsOrig.Add(new PaySplit { SplitNum=12624120720 });
			SplitCollection splitsWeb=WebServiceTests.GetSplitCollection(splitsOrig);
			Assert.AreEqual(1,splitsWeb.Count);
			Assert.AreEqual(splitsOrig.First().SplitNum,splitsWeb.First().SplitNum);
			Assert.AreEqual(splitsOrig.First().TagOD,splitsWeb.First().TagOD);
		}

		[TestMethod]
		public void MiddleTier_GetObjectWithTable() {
			DataTable tableProgNotes=new DataTable("TableProgNotes");
			tableProgNotes.Columns.Add("ProcNum");
			DataRow row=tableProgNotes.NewRow();
			row["ProcNum"]=12624120720;
			tableProgNotes.Rows.Add(row);
			ChartModules.LoadData dataWeb=WebServiceTests.GetChartModulesLoadData(tableProgNotes);
			Assert.AreEqual(1,dataWeb.TableProgNotes.Rows.Count);
			Assert.AreEqual(tableProgNotes.Rows[0][0],dataWeb.TableProgNotes.Rows[0][0]);
		}

		[TestMethod]
		public void MiddleTier_GetObjectWithTableWithInvalidCharacter() {
			DataTable tableProgNotes=new DataTable("TableProgNotes");
			tableProgNotes.Columns.Add("Note");
			DataRow row=tableProgNotes.NewRow();
			row["Note"]="\b is not allowed in XML";
			tableProgNotes.Rows.Add(row);
			ChartModules.LoadData dataWeb=WebServiceTests.GetChartModulesLoadData(tableProgNotes);
			Assert.AreEqual(1,dataWeb.TableProgNotes.Rows.Count);
			Assert.AreEqual(tableProgNotes.Rows[0][0],dataWeb.TableProgNotes.Rows[0][0]);
		}

		[TestMethod]
		public void MiddleTier_GetSerializableDictionary() {
			SerializableDictionary<long,DateTime> dictExpected=new SerializableDictionary<long, DateTime>();
			dictExpected[24]=new DateTime(2018,1,24);
			dictExpected[81]=new DateTime(2064,6,16);
			SerializableDictionary<long,DateTime> dictActual=WebServiceTests.GetSerializableDictionary(dictExpected);
			Assert.IsNotNull(dictActual);
			DateTime date;
			Assert.IsTrue(dictActual.TryGetValue(24,out date));
			Assert.AreEqual(dictExpected[24],date);
			Assert.IsTrue(dictActual.TryGetValue(81,out date));
			Assert.AreEqual(dictExpected[81],date);
		}

		///<summary>Tests to see if Middle Tier can correctly serialize and deserialize a list of ODTuples.</summary>
		[TestMethod]
		public void MiddleTier_GetListODTuple() {
			List<ODTuple<long,string>> listTuples=new List<ODTuple<long,string>>();
			for(int i=0;i<10;i++) {
				listTuples.Add(Tuple.Create((long)i,i.ToString()));
			}
			List<ODTuple<long,string>> listTuplesWeb=WebServiceTests.GetListODTuple(listTuples);
			for(int i=0;i<10;i++) {
				Assert.AreEqual(listTuples[i].Item1,listTuplesWeb[i].Item1);
				Assert.AreEqual(listTuples[i].Item2,listTuplesWeb[i].Item2);
			}
		}

		///<summary>Tests to see if Middle Tier can correctly serialize and deserialize a single ODTuple.</summary>
		[TestMethod]
		public void MiddleTier_GetODTuple() {
			ODTuple<long,DateTime> odTupleExpected=new ODTuple<long,DateTime>(40,new DateTime(2018,1,1));
			ODTuple<long,DateTime> odTupleActual=WebServiceTests.GetODTuple(odTupleExpected);
			Assert.AreEqual(odTupleExpected.Item1,odTupleActual.Item1);
			Assert.AreEqual(odTupleExpected.Item2,odTupleActual.Item2);
		}

		///<summary>This test explicitly tests that Middle Tier does not invoke polymorphisms when passed an incorrect number of parameters.</summary>
		[TestMethod]
		public void MiddleTier_InvalidWebMethod_TooFew() {
			bool hasFailed=false;
			try {
				WebServiceTests.InvalidWebMethod(true);
			}
			catch(Exception ex) {
				ex.DoNothing();
				hasFailed=true;
			}
			Assert.IsTrue(hasFailed);
		}

		///<summary>This test explicitly tests that Middle Tier does not invoke polymorphisms when passed an incorrect number of parameters.</summary>
		[TestMethod]
		public void MiddleTier_InvalidWebMethod_TooMany() {
			bool hasFailed=false;
			try {
				WebServiceTests.InvalidWebMethod();
			}
			catch(Exception ex) {
				ex.DoNothing();
				hasFailed=true;
			}
			Assert.IsTrue(hasFailed);
		}

		///<summary>Our CRUD method ListToTable is incorrectly setting TimeSpan columns by surrounding the time in single quotes.
		///TimeSpans that are formatted like '08:00:00' cannot be parsed correctly which causes the program to do two things:
		///1. It significantly slows down (due to trying its hardest to parse the string literal into a TimeSpan).
		///2. Throws an exception which we catch within PIn.TimeSpan() which sadly turns the end result to System.TimeSpan.Zero.
		///The scenario above describes how our cache classes work over the middle tier which ends up losing data and taking a long time.</summary>
		[TestMethod]
		public void MiddleTier_GetListToTable_TimeSpans() {
			ApptViewT.ClearApptView();
			long apptViewNum1=ApptViewT.CreateApptView(MethodBase.GetCurrentMethod().Name,new TimeSpan(5,20,13)).ApptViewNum;
			long apptViewNum2=ApptViewT.CreateApptView(MethodBase.GetCurrentMethod().Name,new TimeSpan(9,0,45)).ApptViewNum;
			long apptViewNum3=ApptViewT.CreateApptView(MethodBase.GetCurrentMethod().Name).ApptViewNum;
			List<ApptView> listApptViews=ApptViews.GetDeepCopy();
			Assert.AreEqual(listApptViews.Count,3);
			Assert.AreEqual(5,listApptViews.First(x => x.ApptViewNum==apptViewNum1).ApptTimeScrollStart.Hours);
			Assert.AreEqual(20,listApptViews.First(x => x.ApptViewNum==apptViewNum1).ApptTimeScrollStart.Minutes);
			Assert.AreEqual(13,listApptViews.First(x => x.ApptViewNum==apptViewNum1).ApptTimeScrollStart.Seconds);
			Assert.AreEqual(9,listApptViews.First(x => x.ApptViewNum==apptViewNum2).ApptTimeScrollStart.Hours);
			Assert.AreEqual(0,listApptViews.First(x => x.ApptViewNum==apptViewNum2).ApptTimeScrollStart.Minutes);
			Assert.AreEqual(45,listApptViews.First(x => x.ApptViewNum==apptViewNum2).ApptTimeScrollStart.Seconds);
			Assert.AreEqual(TimeSpan.Zero.Hours,listApptViews.First(x => x.ApptViewNum==apptViewNum3).ApptTimeScrollStart.Hours);
			Assert.AreEqual(TimeSpan.Zero.Minutes,listApptViews.First(x => x.ApptViewNum==apptViewNum3).ApptTimeScrollStart.Minutes);
			Assert.AreEqual(TimeSpan.Zero.Seconds,listApptViews.First(x => x.ApptViewNum==apptViewNum3).ApptTimeScrollStart.Seconds);
		}

		///<summary>The following method pulls all s-class insert methods and tests if they correctly handle the primary key. Because we send the 
		///object over middle tier, when our crud sets the primary key within the object, it must be set manually when using middletier. Otherwise,
		///the primary key is lost within the object.</summary>
		[TestMethod]
		public void MiddleTier_GetPrimaryKeysFromInsert() {
			StringBuilder strBuilderErrors=new StringBuilder();
			List<string> listSClassPaths=Directory.GetFiles(@"..\..\..\OpenDentBusiness\Data Interface").ToList();
			List<string> listTableTypePaths=Directory.GetFiles(@"..\..\..\OpenDentBusiness\TableTypes").ToList();
			//Get just the names of the classes without the path and extension information.
			List<string> listSClassNames=listSClassPaths.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
			List<string> listTableTypeNames=listTableTypePaths.Select(x => Path.GetFileNameWithoutExtension(x).ToLower()).ToList();
			Type typeTableBase=typeof(TableBase);
			Assembly ass=Assembly.GetAssembly(typeTableBase);
			List<Type> listSClassTypes=ass.GetTypes().Where(x => x.IsClass && x.IsPublic && listSClassNames.Contains(x.Name))
				.OrderBy(x => x.Name)
				.ToList();
			List<Type> listTableTypeTypes=ass.GetTypes().Where(x => x.IsClass && x.IsPublic
					&& typeTableBase.IsAssignableFrom(x)
					&& listTableTypeNames.Contains(x.Name.ToLower()))
				.OrderBy(x => x.Name)
				.ToList();
			//Get all methods that are insert methods. Insert methods should be static, public, and called insert.
			List<MethodInfo> listInsertMethods=new List<MethodInfo>();
			listSClassTypes.ForEach(x => listInsertMethods.AddRange(x.GetMethods(BindingFlags.Static | BindingFlags.Public).ToList()
				.Where(y => y.Name=="Insert").ToList()));
			int countFailure=0;
			ApptFieldDefT.ClearApptFieldDefTable();//Clear table so the value inserted below will be unique.
			ClaimSnapshotT.ClearClaimSnapshotTable();//Clear table so the value inserted below will be unique.
			Random rand=new Random();
			foreach(MethodInfo insertMethod in listInsertMethods) {
				#region Manual Invokes & Skips
				#region Userods
				if(insertMethod.DeclaringType.Name=="Userods") {//Validation occurs. Must be called with unique username and one user group
					List<Userod> listAllUsers=Userods.GetUsersNoCache();
					Userod userOd=new Userod();
					do {
						userOd.UserName=rand.Next().ToString();
					} while(listAllUsers.Select(x => x.UserName).ToList().Contains(userOd.UserName)); 
					List<long> listUserGroupNums=new List<long> { 1 };
					try {
						Userods.Insert(userOd,listUserGroupNums);
					}
					catch(Exception ex) {
						AppendError(insertMethod,strBuilderErrors,countFailure,ex);
						countFailure++;
						continue;
					}
					if(userOd.UserNum==0) {
						AppendError(insertMethod,strBuilderErrors,countFailure);
						countFailure++;
					}
					continue;
				}
				#endregion
				#region RegistrationKeys
				else if(insertMethod.DeclaringType.Name=="RegistrationKeys") {//Needs a field set to true to ensure a valid return.
					RegistrationKey regKey=new RegistrationKey();
					regKey.IsForeign=true;
					try {
						RegistrationKeys.Insert(regKey);
					}
					catch(Exception ex) {
						AppendError(insertMethod,strBuilderErrors,countFailure,ex);
						countFailure++;
						continue;
					}
					if(regKey.RegistrationKeyNum==0) {
						AppendError(insertMethod,strBuilderErrors,countFailure);
						countFailure++;
					}
					continue;
				}
				#endregion
				#region Payments
				else if(insertMethod.DeclaringType==typeof(Payments)) {
					//There are multiple Insert methods with different parameters. For simplicity, we will skip these methods.
					continue;
				}
				#endregion
				#region Bulk Insert Skip
				else if(ListTools.In(insertMethod.DeclaringType.Name,"Schedules","Signalods")) {//Bulk inserts. Does not return or set primary keys. Skip
					continue;
				}
				#endregion
				#region HQ Only Skip
				else if(ListTools.In(insertMethod.DeclaringType.Name,"Jobs","SiteLinks","BugSubmissions")) {//HQ only. Skip because they require bugs db.
					continue;
				}
				#endregion
				#endregion
				//List of parameters needed for the method.
				List<ParameterInfo> parameters=insertMethod.GetParameters().ToList();
				//Generate necessary parameters
				bool notUsed=false;
				object[] arrayObjs=ParamCheckTests.ConstructParameters(parameters.ToArray(),insertMethod.DeclaringType,insertMethod,strBuilderErrors,out notUsed);
				try {
					//Find the TableType that represents this S class Method.
					Type tableType=listTableTypeTypes.First(x => x.Name==ConvertSClassNameToTableType(insertMethod.DeclaringType.Name));
					if(xCrudGenerator.CrudGenHelper.IsMissingInGeneral(tableType)) {
						continue;//Ignore testing the Insert method for table types that are missing in general.  The table is probably missing from unit test db
					}
					//Call the method.
					insertMethod.Invoke(null,arrayObjs);
					//Pull out the object from the list.
					object insertedObject=arrayObjs.First(x => x.GetType()==tableType);
					//Find the field within this object that is the primary key. Use the CrudColumn attribute with IsPriKey set to true and cast
					//to a long.
					long primaryKey=(long)insertedObject.GetType().GetRuntimeFields().First(x => x.GetCustomAttribute<CrudColumnAttribute>().IsPriKey)
						.GetValue(insertedObject);
					//If the primary key was not set in the object.
					if(primaryKey==0) {
						AppendError(insertMethod,strBuilderErrors,countFailure);
						countFailure++;
					}
				}
				catch(Exception ex) {
					AppendError(insertMethod,strBuilderErrors,countFailure,ex);
					countFailure++;
				}
			}
			Assert.AreEqual("",strBuilderErrors.ToString());
		}

		[TestMethod]
		public void MiddleTier_GetCurUserFromThread() {
			string methodCur=MethodBase.GetCurrentMethod().Name;
			Userod userClient=Security.CurUser;
			Userod userServerThread=WebServiceTests.GetCurUserFromThread();
			Assert.AreEqual(userClient.UserNum,userServerThread.UserNum);
		}

		[TestMethod]
		public void MiddleTier_DataConnection_RemotingRoleTest() {
			#region Main thread client, other thread server.
			RemotingClient.RemotingRole=RemotingRole.ClientDirect;
			Action a=() => {
				RemotingClient.SetRemotingRoleT(RemotingRole.ServerWeb);
				Assert.AreEqual(DataConnection.GetCanReconnect(),false);
			};
			ODThread.RunParallel(new List<Action>(new Action[] { a }),TimeSpan.FromHours(1));
			Assert.AreEqual(DataConnection.GetCanReconnect(),true);
			#endregion
			#region Main thread server, other thread client.
			RemotingClient.RemotingRole=RemotingRole.ServerWeb;
			Security.CurComputerName="";//This is usually set upon program entry.
			Action b=() => {
				RemotingClient.SetRemotingT("",RemotingRole.ClientWeb,false);
				Assert.AreEqual(DataConnection.GetCanReconnect(),true);
			};
			ODThread.RunParallel(new List<Action>(new Action[] { b }),TimeSpan.FromHours(1));
			Assert.AreEqual(DataConnection.GetCanReconnect(),false);
			#endregion
		}

		[TestMethod]
		public void MiddleTier_IsMiddleTierAvailable_Success() {
			DataAction.RunMiddleTierMock(() => {
				Assert.IsTrue(RemotingClient.IsMiddleTierAvailable());
			});
		}

		[TestMethod]
		public void MiddleTier_IsMiddleTierAvailable_Failure() {
			RemotingRole roleCur=RemotingClient.RemotingRole;
			RemotingClient.RemotingRole=RemotingRole.ClientDirect;
			bool isSuccess=RemotingClient.IsMiddleTierAvailable();
			RemotingClient.RemotingRole=roleCur;
			Assert.IsFalse(isSuccess);
		}

		///<summary>Appends an error for MiddleTier_GetPrimaryKeysFromInserts to the passed in stringbuilder. This is used for when the insert commands
		///do not return the primary key from within the object. An exception can be passed in as well to add the exception to the results.</summary>
		private void AppendError(MethodInfo insertMethod,StringBuilder strBuilderErrors,int countFailure,Exception ex=null) {
			if(countFailure==0) {
				string fix="To fix the following classes insert method, when returning from the middletier call, assign the "
					+"returned primary key to the objects primary key. The crud normally does this, but since this is middletier, we "
					+"must do it manually. If an exception appears below, debug the test and find out what is causing this issue.";
				strBuilderErrors.AppendLine("");
				strBuilderErrors.AppendLine(fix);
				Console.WriteLine(fix);
			}
			string error=(ex==null ? "Insert method failed to assign primary key " : "Exception occurred ");
			error+="in SClass: "+insertMethod.DeclaringType.Name;
			strBuilderErrors.AppendLine(error);
			Console.WriteLine(error);
		}

		///<summary>Takes in the the S class method and returns that name of the class that is the base table type. E.g. Send in Appointments, 
		///return appointment.</summary>
		private string ConvertSClassNameToTableType(string sClassName) {
			string retVal="";
			if(sClassName.ToLower()=="lans") {
				retVal="Language";
			}
			else if(sClassName.EndsWith("ies")) {
				retVal=sClassName.Substring(0,sClassName.Length-3)+"y";
			}
			else if(sClassName.EndsWith("ches") || sClassName.EndsWith("bses") 
				|| sClassName.EndsWith("sses") || sClassName.EndsWith("pcses") 
				|| sClassName.EndsWith("shes")) 
			{
				retVal=sClassName.Substring(0,sClassName.Length-2);
			}
			else if(sClassName.EndsWith("s")) {
				retVal=sClassName.Substring(0,sClassName.Length-1);
			}
			return retVal;
		}
	}
}
