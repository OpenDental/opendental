using System;
using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class StatementT {

		public static Statement CreateStatement(long patNum,StatementMode mode_=StatementMode.InPerson,bool isSent=false,DateTime dateSent=default,
			AutoCommStatus smsSendStatus=AutoCommStatus.Undefined,DateTime dateRangeFrom=default,DateTime dateRangeTo=default,
			string note="",string noteBold="",bool hidePayment=false,bool singlePatient=false,bool intermingled=false,long docNum=0,
			DateTime dateTStamp=default,bool isReceipt=false,bool isInvoice=false,bool isInvoiceCopy=false,long superFamily=0,bool isBalValid=false,
			double insEst=0,double balTotal=0,StmtType statementType=StmtType.NotSet)
		{
			Statement statement=new Statement() {
				PatNum=patNum,
				DateSent=dateSent,
				SmsSendStatus=smsSendStatus,
				DateRangeFrom=dateRangeFrom,
				DateRangeTo=dateRangeTo,
				Note=note,
				NoteBold=noteBold,
				Mode_=mode_,
				HidePayment=hidePayment,
				SinglePatient=singlePatient,
				Intermingled=intermingled,
				IsSent=isSent,
				DocNum=docNum,
				DateTStamp=dateTStamp,
				IsReceipt=isReceipt,
				IsInvoice=isInvoice,
				IsInvoiceCopy=isInvoiceCopy,
				SuperFamily=superFamily,
				IsBalValid=isBalValid,
				InsEst=insEst,
				BalTotal=balTotal,
				StatementType=statementType,
			};
			DataAction.RunPractice(() => Statements.Insert(statement));
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

		public static List<Statement> GetAllStatements() {
			string command=$"SELECT * FROM statement";
			return OpenDentBusiness.Crud.StatementCrud.SelectMany(command);
		}
	}
}
