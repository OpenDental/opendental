using Newtonsoft.Json;
using OpenDentBusiness.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class MsgToPaySents {
		public const string MSG_TO_PAY_TAG="[MsgToPayURL]";
		public const string STATEMENT_URL_TAG="[StatementURL]";
		public const string STATEMENT_BALANCE_TAG="[StatementBalance]";

		public static long Insert(MsgToPaySent msgToPaySent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				msgToPaySent.MsgToPaySentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),msgToPaySent);
				return msgToPaySent.MsgToPaySentNum;
			}
			return MsgToPaySentCrud.Insert(msgToPaySent);
		}

		///<summary>Creates a MsgToPaySent using the Patient's PatNum and ClinicNum. DOES NOT INSERT.</summary>
		public static MsgToPaySent CreateFromPat(Patient pat) {
			MsgToPaySent msgToPaySent=new MsgToPaySent();
			msgToPaySent.PatNum=pat.PatNum;
			msgToPaySent.ClinicNum=pat.ClinicNum;
			msgToPaySent.SendStatus=AutoCommStatus.SendNotAttempted;
			msgToPaySent.Source=MsgToPaySource.Manual;
			return msgToPaySent;
		}

		public static bool Update(MsgToPaySent msgToPay,MsgToPaySent msgToPayOld=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),msgToPay,msgToPayOld);
			}
			if(msgToPayOld==null) {
				MsgToPaySentCrud.Update(msgToPay);
				return true;
			}
			return MsgToPaySentCrud.Update(msgToPay,msgToPayOld);
		}

		///<summay>Gets and returns all unsent (SendNotAttempted) MsgToPaySents from the database.</summay>
		public static List<MsgToPaySent> GetAllUnsent(long clinicNum,DateTime dateNow,CommType commType=CommType.Invalid) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MsgToPaySent>>(MethodBase.GetCurrentMethod(),clinicNum,dateNow,commType);
			}
			//SendNotAttempted means this has been queued to be sent
			string command=$"SELECT * FROM msgtopaysent WHERE Source={POut.Enum(MsgToPaySource.Manual)} AND ((DateTimeSent < '1880-01-01' AND SendStatus={POut.Enum(AutoCommStatus.SendNotAttempted)}) "
				+ $"OR (SendStatus={POut.Enum(AutoCommStatus.SendFailed)} AND DATE(DateTimeSendFailed)<DATE({POut.Date(dateNow,encapsulate:true)})))";//Or the attempted send failed and its been at least a day.
			if(clinicNum>=0) {
				//Query clinicNum if needed
				command+=$" AND ClinicNum={POut.Long(clinicNum)}";
			}
			if(commType!=CommType.Invalid) {
				command+=$" AND MessageType={POut.Enum(commType)}";
			}
			return MsgToPaySentCrud.SelectMany(command);
		}
	}

	///<summary>Backing class for PrefName.PaymentPortalMsgToPayEmailMessageTemplate. Holds the email template and type (html, raw html, etc, etc)</summary>
	public class MsgToPayEmailTemplate {
		public string Template;
		public EmailType EmailType;
	}
}