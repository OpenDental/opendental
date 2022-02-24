using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Stores all the connection info for one email address.  Linked to clinic by clinic.EmailAddressNum.  Sends email based on patient's clinic.</summary>
	[Serializable()]
	public class EmailAddress:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailAddressNum;
		///<summary>For example smtp.gmail.com</summary>
		public string SMTPserver;
		///<summary>.</summary>
		public string EmailUsername;
		///<summary>Password associated with this email address.  Encrypted when stored in the database and decrypted before using.</summary>
		public string EmailPassword;
		///<summary>Usually 587, sometimes 25 or 465.</summary>
		public int ServerPort;
		///<summary>.</summary>
		public bool UseSSL;
		///<summary>The email address of the sender as it should appear to the recipient.</summary>
		public string SenderAddress;
		///<summary>For example pop.gmail.com</summary>
		public string Pop3ServerIncoming;
		///<summary>Usually 110, sometimes 995.</summary>
		public int ServerPortIncoming;
    ///<summary>FK to userod.UserNum.  Associates a user with this email address.  A user may only have one email address associated with them.
    ///Can be 0 if no user is associated with this email address.</summary>
    public long UserNum;
    ///<summary>Webmail ProvNum.  Just makes it easier to know what email address the user picked in the inbox. Not a DB column.</summary>
    [CrudColumn(IsNotDbColumn=true)]
    public long WebmailProvNum;
		///<summary>Needed for OAuth.</summary>
		public string AccessToken;
		///<summary>Needed for OAuth.</summary>
		public string RefreshToken;
		///<summary>When true, this will allow the user to download emails to their inbox.</summary>
		public bool DownloadInbox;

    ///<summary>We assume the email settings are implicit if the server port is 465.</summary>
    public bool IsImplicitSsl {
			get {
				if(ServerPort==465) {
					return true;
				}
				return false;
			}
		}

		///<summary></summary>
		public EmailAddress Clone() {
			return (EmailAddress)this.MemberwiseClone();
		}

		///<summary>Returns the SenderAddress if it is not blank, otherwise returns the EmailUsername.</summary>
		public string GetFrom() {
			return string.IsNullOrEmpty(SenderAddress) ? EmailUsername : SenderAddress;
		}

	}


}