using System;

namespace OpenDentBusiness {

	///<summary>Only used at HQ.  If a row is present in this table, then this customer is a reseller.  Also holds their credentials for the reseller portal.</summary>
	[Serializable]
	public class Reseller:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ResellerNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>User name used to log into the reseller portal with.</summary>
		public string UserName;
		///<summary>The password details in a "HashType$Salt$Hash" format, separating the different fields by '$'.
		///This is NOT the actual password but the encoded password hash.
		///If the contents of this variable are not in the aforementioned format, it is assumed to be a legacy password hash (MD5).</summary>
		public string ResellerPassword;
		/// <summary>FK to definition.DefNum.  An override for the default patient.BillingType for new reseller customers.</summary>
		public long BillingType;
		///<summary>An override for the default registrationkey.VotesAllotted for new reseller customers.</summary>
		public int VotesAllotted;
		///<summary>An override for the default registrationkey.Note for new reseller customers.</summary>
		public string Note;

		/// <summary>The getter will return a struct created from the database-ready password which is stored in the Password field.
		/// The setter will manipulate the Password variable to the string representation of this PasswordContainer object.</summary>
		public PasswordContainer LoginDetails {
			get {
				return Authentication.DecodePass(this.ResellerPassword);
			}
			set {
				this.ResellerPassword=value.ToString();
			}
		}

		///<summary>The password hash, not the actual password.  If no password has been entered, then this will be blank.</summary>
		public string PasswordHash {
			get {
				return LoginDetails.Hash;
			}
		}

		///<summary>Returns a copy of this Reseller.</summary>
		public Reseller Copy() {
			return (Reseller)this.MemberwiseClone();
		}
	}
}