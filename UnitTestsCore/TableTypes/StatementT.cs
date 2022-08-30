using System;
using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class StatementT {

		public static Statement CreateStatement(long patNum,StatementMode mode_=StatementMode.InPerson,bool isSent=false,DateTime dateSent=default) {
			Statement statement=new Statement() {
				PatNum=patNum,
				Mode_=mode_,
				IsSent=isSent,
				DateSent=dateSent,
			};
			Statements.Insert(statement);
			return statement;
		}

		///<summary>Deletes everything from the statement table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearStatementTable() {
			string command="DELETE FROM statement WHERE StatementNum > 0";
			DataCore.NonQ(command);
		}

		public static List<Statement> GetStatementsForPat(long patNum) {
			string command=$"SELECT * FROM statement WHERE PatNum={patNum}";
			return OpenDentBusiness.Crud.StatementCrud.SelectMany(command);
		}
	}
}
