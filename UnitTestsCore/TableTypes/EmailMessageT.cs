using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class EmailMessageT {

		public static EmailMessage CreateEmailMessage(long patNum,long patNumSubj=0,string fromAddress="",string toAddress="",string recipientAddress="",
			string subject="",string bodyText="",EmailSentOrReceived sentOrReceived=EmailSentOrReceived.Neither,DateTime msgDateTime=default)
		{
			if(patNumSubj==0) {
				patNumSubj=patNum;
			}
			EmailMessage email=new EmailMessage() {
				FromAddress=fromAddress,
				ToAddress=toAddress,
				RecipientAddress=recipientAddress,
				PatNum=patNum,
				PatNumSubj=patNumSubj,
				Subject=subject,
				BodyText=bodyText,
				SentOrReceived=sentOrReceived,
				MsgDateTime=msgDateTime
			};
			EmailMessages.Insert(email);
			return email;
		}

		public static EmailMessage CreateWebMail(long provnum,long patNum,long patNumSubj=0,string fromAddress="",string toAddress="",
			string recipientAddress="",string subject="",string bodyText="") {
			if(patNumSubj==0) {
				patNumSubj=patNum;
			}
			EmailMessage email=new EmailMessage() {
				FromAddress=fromAddress,
				ToAddress=toAddress,
				RecipientAddress=recipientAddress,
				PatNum=patNum,
				PatNumSubj=patNumSubj,
				Subject=subject,
				BodyText=bodyText,
				SentOrReceived=EmailSentOrReceived.WebMailReceived,
				ProvNumWebMail=provnum,
			};
			EmailMessages.Insert(email);
			return email;
		}

		///<summary>Deletes everything from the table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEmailMessageTable() {
			string command="DELETE FROM emailmessage WHERE EmailMessageNum > 0";
			DataCore.NonQ(command);
		}

		///<summary>Gets all messages for the patient.</summary>
		public static List<EmailMessage> GetForPatient(long patNum) {
			string command=$"SELECT * FROM emailmessage WHERE PatNum={POut.Long(patNum)}";
			return OpenDentBusiness.Crud.EmailMessageCrud.SelectMany(command);
		}

		///<summary>Gets all messages for the given recipient email address.</summary>
		public static List<EmailMessage> GetForToAddress(string toAddress) {
			string command=$"SELECT * FROM emailmessage WHERE ToAddress='{POut.String(toAddress)}'";
			return OpenDentBusiness.Crud.EmailMessageCrud.SelectMany(command);
		}
	}
}
