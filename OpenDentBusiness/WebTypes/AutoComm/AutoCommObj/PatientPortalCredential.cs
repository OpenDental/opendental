using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>An object to track credentials for PatientPortal.</summary>
	public class PatientPortalCredential {
		public const string STRING_OBFUSCATION="*******";

		public readonly UserWeb UserWeb;
		public readonly string PlainTextPassword;
		public readonly PasswordContainer PasswordContainer;
		///<summary>A user who has logged in before will have a blank PlainTextPassword.</summary>
		public readonly bool HasAccessedPatientPortal;

		public PatientPortalCredential() { }

		public PatientPortalCredential(UserWeb userWeb,string plainTextPassword,PasswordContainer passwordContainer) {
			UserWeb=userWeb;
			HasAccessedPatientPortal=string.IsNullOrEmpty(plainTextPassword);
			PasswordContainer=passwordContainer;
			PlainTextPassword=HasAccessedPatientPortal ? STRING_OBFUSCATION : plainTextPassword;
		}

		public string GetUserName() {
			if(UserWeb.RequireUserNameChange) {
				return UserWeb.UserName;
			}
			return UserWeb.UserName.First()+STRING_OBFUSCATION+UserWeb.UserName.Last();
		}
	}
}
