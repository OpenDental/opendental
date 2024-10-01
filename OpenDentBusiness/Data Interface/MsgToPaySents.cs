using CodeBase;
using OpenDentBusiness.Crud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class MsgToPaySents {
		public static long Insert(MsgToPaySent msgToPaySent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				msgToPaySent.MsgToPaySentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),msgToPaySent);
				return msgToPaySent.MsgToPaySentNum;
			}
			return MsgToPaySentCrud.Insert(msgToPaySent);
		}

		///<summary>Creates and Inserts a MsgToPaySent and attaches a StmtLink. EmailType not used for Texts. This table is linked to AutoComm and is only used for appointment-related messages. Inserts into this table
		///do not require a corresponding Text or Email to be sent as the AutoCommProcessor handles the creation and sending of those message. Regular M2Ps are sent in real time and are not inserted into this table.
		///AutoComm treats M2Ps inserted with a Source of Manual as an outbound queue to handle.</summary>
		public static MsgToPaySent CreateFromPat(Patient patient,CommType commType,string message,Appointment appt,string subject="",EmailType emailType=EmailType.Regular,long statementNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<MsgToPaySent>(MethodBase.GetCurrentMethod(),patient,commType,message,appt,subject,emailType,statementNum);
			}
			MsgToPaySent msgToPaySent=new MsgToPaySent();
			msgToPaySent.PatNum=patient.PatNum;
			msgToPaySent.ClinicNum=patient.ClinicNum;
			msgToPaySent.StatementNum=statementNum;
			msgToPaySent.SendStatus=AutoCommStatus.SendNotAttempted;
			msgToPaySent.Source=MsgToPaySource.Manual;
			msgToPaySent.Message=message;
			msgToPaySent.MessageType=commType;
			msgToPaySent.Subject=subject;
			msgToPaySent.EmailType=emailType;
			msgToPaySent.ApptNum=appt.AptNum;
			msgToPaySent.ApptDateTime=appt.AptDateTime;
			msgToPaySent.StatementNum=Insert(msgToPaySent);
			//Bind the Statement to the MsgToPaySent
			StmtLinks.AttachMsgToPayToStatement(statementNum,ListTools.FromSingle(msgToPaySent.MsgToPaySentNum));
			return msgToPaySent;
		}

		public static bool Update(MsgToPaySent msgToPaySent,MsgToPaySent msgToPaySentOld=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),msgToPaySent,msgToPaySentOld);
			}
			if(msgToPaySentOld==null) {
				MsgToPaySentCrud.Update(msgToPaySent);
				return true;
			}
			return MsgToPaySentCrud.Update(msgToPaySent,msgToPaySentOld);
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

	///<summary>Backing class for PrefName.PaymentPortalMsgToPayEmailMessageTemplate and PrefName.PaymentPortalMsgToPayEmailMessageTemplateAppt. Holds the email template and type (html, raw html, etc, etc)</summary>
	public class MsgToPayEmailTemplate {
		public string Template;
		public EmailType EmailType;
	}
}