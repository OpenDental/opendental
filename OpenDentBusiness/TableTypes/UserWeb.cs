using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {    
	///<summary>Holds credentials for web applications.  Each userweb entry should be linked to a table type or entity of sorts.
	///E.g. Patient Portal credentials will have an FKey to patient.PatNum and an FKeyType linked to "UserWebFKeyType.PatientPortal".</summary>
  [Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class UserWeb:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long UserWebNum;
		///<summary>Foreign key to the table defined by the corresponding FKeyType.</summary>
		public long FKey;
		///<summary>Enum:UserWebFKeyType The type of row that identifies which table FKey links to.</summary>
		public UserWebFKeyType FKeyType;
		///<summary></summary>
		public string UserName;
		///<summary>The password details in a "HashType$Salt$Hash" format, separating the different fields by '$'.
		///This is NOT the actual password but the encoded password hash.
		///If the contents of this variable are not in the aforementioned format, it is assumed to be a legacy password hash (MD5).</summary>
		public string Password;
		///<summary>A randomly generated code that can be used to reset the password.</summary>
		public string PasswordResetCode;
		///<summary>Set to true to require a user to change their UserName.</summary>
		public bool RequireUserNameChange;
		///<summary>The last time when the user used their credentials to log in.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeLastLogin;
		///<summary>Set to true to require a user to change their Password.</summary>
		public bool RequirePasswordChange;
		
		/// <summary>The getter will return a struct created from the database-ready password which is stored in the Password field.
		/// The setter will manipulate the Password variable to the string representation of this PasswordContainer object.</summary>
		public PasswordContainer LoginDetails {
			get {
				return Authentication.DecodePass(this.Password);
			}
			set {
				this.Password=value.ToString();
			}
		}

		///<summary>The password hash, not the actual password.  If no password has been entered, then this will be blank.</summary>
		public string PasswordHash {
			get {
				return LoginDetails.Hash;
			}
		}

		///<summary></summary>
		public UserWeb Copy() {
			return (UserWeb)this.MemberwiseClone();
		}
	}
		
		///<summary>The type of row that identifies which table FKey links to.</summary>
		public enum UserWebFKeyType {
			///<summary>This is a default value that should never be saved into the table.</summary>
			Undefined,
			///<summary>FK to patient.PatNum</summary>
			PatientPortal
		}
}
