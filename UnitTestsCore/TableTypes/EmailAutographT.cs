using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EmailAutographT {

		///<summary>.</summary>
		public static EmailAutograph CreateEmailAutograph(string emailAddress,string autographText="",string description="") {
			EmailAutograph emailAutograph=new EmailAutograph();
			emailAutograph.EmailAddress=emailAddress;
			emailAutograph.AutographText=autographText;
			emailAutograph.Description=description;			
			EmailAutographs.Insert(emailAutograph);
			EmailAutographs.RefreshCache();
			return emailAutograph;
		}

		///<summary>Deletes everything from the table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEmailAutographTable() {
			string command="DELETE FROM emailautograph WHERE EmailAutographNum > 0";
			DataCore.NonQ(command);
		}
	}
}
