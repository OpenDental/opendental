using OpenDentBusiness;
using OpenDentBusiness.Crud;
using System;
using System.Collections.Generic;

namespace UnitTestsCore {
	public class MsgToPaySentT {
		public static void ClearMsgToPaySentTable() {
			string command="DELETE FROM msgtopaysent";
			DataCore.NonQ(command);
		}

		public static MsgToPaySent CreateOne(long clinicNum=0,AutoCommStatus status=AutoCommStatus.SendNotAttempted,DateTime dateFailed=default,long patNum=0,string shortGuid="",string message="",CommType commType=CommType.Text,MsgToPaySource source=MsgToPaySource.Manual,long statementNum=0) {
			MsgToPaySent msgToPaySent=new MsgToPaySent();
			msgToPaySent.ClinicNum=clinicNum;
			msgToPaySent.SendStatus=status;
			msgToPaySent.DateTimeSendFailed=dateFailed==default ? DateTime.MinValue : dateFailed;
			msgToPaySent.PatNum=patNum;
			msgToPaySent.ShortGUID=shortGuid;
			msgToPaySent.Message=message;
			msgToPaySent.MessageType=commType;
			msgToPaySent.Source=source;
			msgToPaySent.StatementNum=statementNum;
			DataAction.RunPractice(() => msgToPaySent.MsgToPaySentNum=MsgToPaySentCrud.Insert(msgToPaySent));
			return msgToPaySent;
		}

		public static List<MsgToPaySent> GetAll() {
			string command="SELECT * FROM msgtopaysent";
			return DataAction.GetPractice(() => MsgToPaySentCrud.SelectMany(command));
		}
	}
}
