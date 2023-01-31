using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class EmailMessageUids{
		///<summary>Gets all unique email ids for the given recipient email address.  The result is used to determine which emails to download for a particular inbox address.</summary>
		public static List<string> GetMsgIdsRecipientAddress(string strRecipientAddress) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),strRecipientAddress);
			}
			string command="SELECT DISTINCT BINARY MsgId FROM emailmessageuid WHERE RecipientAddress='"+POut.String(strRecipientAddress)+"'";
			return Db.GetListString(command);
		}

		///<summary></summary>
		public static long Insert(EmailMessageUid emailMessageUid) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				emailMessageUid.EmailMessageUidNum=Meth.GetLong(MethodBase.GetCurrentMethod(),emailMessageUid);
				return emailMessageUid.EmailMessageUidNum;
			}
			return Crud.EmailMessageUidCrud.Insert(emailMessageUid);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one EmailMessageUid from the db.</summary>
		public static EmailMessageUid GetOne(long emailMessageUidNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EmailMessageUid>(MethodBase.GetCurrentMethod(),emailMessageUidNum);
			}
			return Crud.EmailMessageUidCrud.SelectOne(emailMessageUidNum);
		}

		///<summary></summary>
		public static void Update(EmailMessageUid emailMessageUid){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailMessageUid);
				return;
			}
			Crud.EmailMessageUidCrud.Update(emailMessageUid);
		}

		///<summary></summary>
		public static void Delete(long emailMessageUidNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailMessageUidNum);
				return;
			}
			string command= "DELETE FROM emailmessageuid WHERE EmailMessageUidNum = "+POut.Long(emailMessageUidNum);
			Db.NonQ(command);
		}
		*/
	}
}