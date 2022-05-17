using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Keeps track of failed login attempts.</summary>
	[Serializable]
	public class LoginAttempt:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LoginAttemptNum;
		///<summary>The username that was attempted. May not be a username that exists.</summary>
		public string UserName;
		///<summary>Enum:UserWebFKeyType The part of the program where an attempt was made. If we want to use this for other parts of the program
		///that are do not use the userweb table, we can change this enum to a different one.</summary>
		public UserWebFKeyType LoginType;
		///<summary>When the failed attempt was attempted.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTFail;

		///<summary></summary>
		public LoginAttempt Copy() {
			return (LoginAttempt)this.MemberwiseClone();
		}
	}
}