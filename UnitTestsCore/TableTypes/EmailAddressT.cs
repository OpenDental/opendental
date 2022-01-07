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
			EmailAddresses.Insert(email);
			EmailAddresses.RefreshCache();
			return email;
		}
		
	}
}
