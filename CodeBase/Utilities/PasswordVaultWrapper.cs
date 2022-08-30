using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Security.Credentials;

namespace CodeBase {
	///<summary>This wrapper class protects Windows 7 users from a runtime error that is caused by the Windows.wnd reference. This reference only works on windows 8 and up. Any calling of 
	///the member's methods MUST be try-caught to avoid a runtime error.</summary>
	public class PasswordVaultWrapper {
		private const string _strMTResourcePrefix="OpenDental Middle Tier:";

		///<summary>Clears all OpenDental Middle Tier credentials from the PasswordVault for the currently signed in Windows User.</summary>
		public static void ClearCredentials(string uri) {
			IReadOnlyList<PasswordCredential> listCreds=new PasswordVault().FindAllByResource(_strMTResourcePrefix+uri);
			foreach(PasswordCredential passwordCredential in listCreds) {
				new PasswordVault().Remove(passwordCredential);
			}
		}
		
		///<summary>This method will throw an exception if you pass it a blank password. Windows cannot encrypt a blank password. 
		///Callers of this method should consider this scenario.  Throws exceptions.</summary>
		public static void WritePassword(string uri,string username,string password) {
			new PasswordVault().Add(new PasswordCredential(_strMTResourcePrefix+uri,username,password));//WCM encrypts the password
		}

		///<summary>Retrieves the first "OpenDental Middle Tier" username listed in the Password Vault for the URI passed in.</summary>
		public static bool TryRetrieveUserName(string uri,out string username) {
			username=string.Empty;
      IReadOnlyList<PasswordCredential> listCreds;
      try {
        listCreds=new PasswordVault().FindAllByResource(_strMTResourcePrefix+uri);
      }
      catch(Exception ex) {
				ex.DoNothing();
				return false;
      }
			if(listCreds.Count>0) {
				username=listCreds.FirstOrDefault().UserName;
				return true;//Found at least one PasswordCredential.  Use the first.
			}
			return false;//Couldn't find any PasswordCredentials.
		}

		///<summary>An exception will be thrown if the password cannot be found. Callers of this method should consider this scenario.</summary>
		public static string RetrievePassword(string uri,string username) {
			//This will only return the password if it has been saved under the current Windows user.
			PasswordCredential cred=new PasswordVault().Retrieve(_strMTResourcePrefix+uri,username);
			cred.RetrievePassword();
			return cred.Password;
		}
	}
}
