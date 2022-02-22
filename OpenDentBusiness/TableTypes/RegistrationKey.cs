using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Keeps track of which product keys have been assigned to which customers.
	///This datatype is only used if the program is being run from a distributor installation.
	///A single customer is allowed to have more than one key, to accommodate for various circumstances, including having multiple physical business locations.</summary>
	[Serializable]
	public class RegistrationKey:TableBase {
		///<summary>Primary Key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RegistrationKeyNum;
		///<summary>FK to patient.PatNum. The customer to which this registration key applies.</summary>
		public long PatNum;
		///<summary>The registration key as stored in the customer database.</summary>
		public string RegKey;
		///<summary>Db note about the registration key. Specifically, the note must include information about the location to which this key pertains, since once at least one key must be assigned to each location to be legal.</summary>
		public string Note;
		///<summary>This will help later with tracking for licensing.</summary>
		public DateTime DateStarted;
		///<summary>This is used to completely disable a key.  Might possibly even cripple the user's program.  Usually only used if reassigning another key due to abuse or error.  If no date specified, then this key is still valid.</summary>
		public DateTime DateDisabled;
		///<summary>This is used when the customer cancels monthly support.  This still allows the customer to get downloads for bug fixes, but only up through a certain version.  Our web server program will use this date to deduce which version they are allowed to have.  Any version that was released as a beta before this date is allowed to be downloaded.</summary>
		public DateTime DateEnded;
		///<summary>This is assigned automatically based on whether the registration key is a US version vs. a foreign version.  The foreign version is not able to unlock the procedure codes.  There are muliple layers of safeguards in place.</summary>
		public bool IsForeign;
		///<summary>Deprecated.</summary>
		public bool UsesServerVersion;
		///<summary>We have given this customer a free version.  Typically in India.</summary>
		public bool IsFreeVersion;
		///<summary>This customer is not using the software with live patient data, but only for testing and development purposes.</summary>
		public bool IsOnlyForTesting;
		///<summary>Typically 100, although it can be more for multilocation offices.</summary>
		public int VotesAllotted;
		///<summary>This is a customer of a reseller, so this customer will not have full access to all our services.</summary>
		public bool IsResellerCustomer;
		///<summary>This is a customer that is allowed early access to certain features.  E.g. downloading the Alpha version of the software.</summary>
		public bool HasEarlyAccess;
		///<summary>Next date and time of supplemental backup for the customer who owns this registration key.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTBackupScheduled;
		///<summary>Pass code for next supplemental backup expected from this customer.</summary>
		public string BackupPassCode;

		public RegistrationKey Copy(){
			return (RegistrationKey)this.MemberwiseClone();
		}
	}
}
