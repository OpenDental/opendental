using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EmailAddressT {

		///<summary>Inserts the new email address and returns it.</summary>
		public static EmailAddress CreateEmailAddress(string senderAddress="",string emailUserName="") {
			EmailAddress email=new EmailAddress();
			email.SenderAddress=senderAddress;
			if(senderAddress=="") {
				email.SenderAddress="Email"+MiscUtils.CreateRandomAlphaNumericString(4);
			}
			email.EmailUsername=emailUserName;
			email.SMTPserver="xxxSMTPxxx";
			EmailAddresses.Insert(email);
			EmailAddresses.RefreshCache();
			return email;
		}

		///<summary>Deletes everything from the table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEmailAddressTable() {
			string command="DELETE FROM emailaddress WHERE EmailAddressNum > 0";
			DataCore.NonQ(command);
		}
	}
}
