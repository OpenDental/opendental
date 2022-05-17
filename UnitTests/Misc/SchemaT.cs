using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTests {
	public class SchemaT {
		//Step 1: Start in SchemaCrudProposedTest.  It is a series of proposed test methods (although not true crud).
		//   Manually build some functional C# code that will make changes to the db when called.
		//Step 2: In FormUnitTests, check the radiobutton for "Test proposed crud".  Click the Schema button.
		//   It then calls each of the methods in the proposed crud, making sure that they don't crash and that each method makes valid calls to db.
		//   There can still be errors if the crud queries don't crash, but still fail to perform the expected action.
		//   So the ending schema should be checked for expected indexes, triggers, etc.
		//   When the unit test Core Types button is clicked, some schema problems can be caught by failures at that point.
		//Step 3: In the CrudGenerator project, run with the Schema box checked in order to generate SchemaCrudTest.
		//   SchemaCrudTest will not actually be used.  It is simply to test the code generation ability. 
		//Step 4: In FormUnitTests, check the radiobutton for "Compare proposed to generated".  Click the Schema button.
		//   It will compare SchemaCrudProposedTest to the generated SchemaCrudTest, and then notify the user whether they match or not.
		//   If they don't match, alter CrudGenerator in order to produce the exact code needed.
		//Step 5: (Jordan only) Improve CrudGenerator to generate similar code on demand.  This happens in three different situations:
		//   a. Code is generated for the unit testing above.
		//   b. Code is generated when a missing table or column is detected.
		//   c. Code is also generated from the CrudGenerator interface to be copied and pasted into the ConvertDatabases class.
		//      This can be done through the UI, or for more complex situations, by setting up some C# in CrudGenerator.

		public static string TestProposedCrud(string serverAddr,string port,string userName,string password,bool isOracle) {
			string retVal="";
			UnitTestsCore.DatabaseTools.SetDbConnection(TestBase.UnitTestDbName,serverAddr,port,userName,password,isOracle);
			SchemaCrudProposedTest.AddTableTempcore();
			SchemaCrudProposedTest.AddColumnEndClob();
			SchemaCrudProposedTest.AddColumnEndInt();
			SchemaCrudProposedTest.AddColumnEndTimeStamp();
			SchemaCrudProposedTest.AddIndex();
			SchemaCrudProposedTest.DropColumn();
			retVal+="Proposed Crud passed test.\r\n";
			return retVal;
		}

		public static string CompareProposedToGenerated(bool isOracle) {
			return "CompareProposedToGenerated not implemented yet.\r\n";
		}

		


	}
}
