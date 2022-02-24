using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness{
		///<summary>(User OD since "user" is a reserved word) Users are a completely separate entity from Providers and Employees even though they can be linked.  A usernumber can never be changed, ensuring a permanent way to record database entries and leave an audit trail.  A user can be a provider, employee, or neither.</summary>
	[Serializable()]
	public class Userod:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long UserNum;
		///<summary>.</summary>
		public string UserName;
		///<summary>The password details in a "HashType$Salt$Hash" format, separating the different fields by '$'.
		///This is NOT the actual password but the encoded password hash.
		///If the contents of this variable are not in the aforementioned format, it is assumed to be a legacy password hash (MD5).</summary>
		public string Password;
		///<summary>Deprecated. Use UserGroupAttaches to link Userods to UserGroups.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public long UserGroupNum;
		///<summary>FK to employee.EmployeeNum. Cannot be used if provnum is used. Used for timecards to block access by other users.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public long EmployeeNum;
		///<summary>FK to clinic.ClinicNum.  Default clinic for this user.  It causes new patients to default to this clinic when entered by this user.  
		///If 0, then user has no default clinic or default clinic is HQ if clinics are enabled.</summary> 		
		public long ClinicNum;
		///<summary>FK to provider.ProvNum.  Cannot be used if EmployeeNum is used.  It is possible to have multiple userods attached to a single provider.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public long ProvNum;
		///<summary>Set true to hide user from login list.</summary>
		public bool IsHidden;
		///<summary>FK to tasklist.TaskListNum.  0 if no inbox setup yet.  It is assumed that the TaskList is in the main trunk, but this is not strictly enforced.  User can't delete an attached TaskList, but they could move it.</summary>
		public long TaskListInBox;
		/// <summary> Defaults to 3 (regular user) unless specified. Helps populates the Anesthetist, Surgeon, Assistant and Circulator dropdowns properly on FormAnestheticRecord/// </summary>
		public int AnesthProvType;
		///<summary>If set to true, the BlockSubsc button will start out pressed for this user.</summary>
		public bool DefaultHidePopups;
		///<summary>Gets set to true if strong passwords are turned on, and this user changes their password to a strong password.  We don't store actual passwords, so this flag is the only way to tell.</summary>
		public bool PasswordIsStrong;
		///<summary>When true, prevents user from having access to clinics that are not in the corresponding userclinic table.
		///Many places throughout the program will optionally remove the 'All' option from this user when true.</summary>
		public bool ClinicIsRestricted;
		///<summary>If set to true, the BlockInbox button will start out pressed for this user.</summary>
		public bool InboxHidePopups;
		///<summary>FK to userod.UserNum.  The user num within the Central Manager database.  Only editable via CEMT.  Can change when CEMT syncs.</summary>
		[CrudColumn(IsNotCemtColumn=true,IsCemtSyncKey=true)]
		public long UserNumCEMT;
		///<summary>The date and time of the most recent log in failure for this user.  Set to MinValue after user logs in successfully.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT,IsNotCemtColumn=true)]
		public DateTime DateTFail;
		///<summary>The number of times this user has failed to log into their account.  Set to 0 after user logs in successfully.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public byte FailedAttempts;
		/// <summary>The username for the ActiveDirectory user to link the account to.</summary>
		public string DomainUser;
		///<summary>Boolean.  If true, the user's password needs to be reset on next login.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public bool IsPasswordResetRequired;
		///<summary>A hashed pin that is used for mobile web validation on eClipboard. Not used in OD proper.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public string MobileWebPin;
		///<summary>The number of attempts the mobile web pin has failed. Reset on successful attempt.</summary>
		[CrudColumn(IsNotCemtColumn=true)]
		public byte MobileWebPinFailedAttempts;
		///<summary>Minimum date if last login date and time is unknown.
		///Otherwise contians the last date and time this user successfully logged in.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTLastLogin;

		///<summary>The getter will return a struct created from the database-ready password which is stored in the Password field.
		/// The setter will manipulate the Password variable to the string representation of this PasswordContainer object.</summary>
		[XmlIgnore]
		public PasswordContainer LoginDetails {
			get {
				return Authentication.DecodePass(this.Password);
			}
			set {
				this.Password=value.ToString();
			}
		}

		///<summary>The password hash, not the actual password.  If no password has been entered, then this will be blank.</summary>
		[XmlIgnore]
		public string PasswordHash {
			get {
				return LoginDetails.Hash;
			}
		}

		///<summary>Enum representing what eService this user is a "phantom" user for.
		///This variable is to be ignored for serialization and was made private to emphasize the fact that it should not be a db column.
		///Mainly used to not have Security.IsAuthorized() checks throw exceptions due to a null Userod when the eServices are calling S class methods.
		///Currently only used to grant eServices access to certain methods that would otherwise reject it due to permissions.</summary>
		private EServiceTypes _eServiceType;
		///<summary>All valid users should NOT set this value to anything other than None otherwise permission checking will act unexpectedly.
		///Programmatically set this value from the init method of the corresponding eService.  Helps prevent unhandled exceptions.
		///Custom property only meant to be used via eServices.  Not a column in db.  Not to be used in middle tier environment.</summary>
		[XmlIgnore]
		public EServiceTypes EServiceType {
			get {
				return _eServiceType;
			}
			set {
				_eServiceType=value;
			}
		}

		public Userod(){

		}
		
		///<summary></summary>
		public Userod Copy(){
			return (Userod)this.MemberwiseClone();
		}

		public override string ToString(){
			return UserName;
		}

		public bool IsInUserGroup(long userGroupNum) {
			return Userods.IsInUserGroup(this.UserNum,userGroupNum);
		}

		///<summary>Gets all of the usergroups attached to this user.</summary>
		public List<UserGroup> GetGroups(bool includeCEMT = false) {
			return UserGroups.GetForUser(UserNum, includeCEMT);
		}

	}

	///<summary></summary>
	public enum EServiceTypes {
		///<summmary>Not an eService user.  All valid users should be this type otherwise permission checking will act differently.</summmary>
		None,
		///<summary></summary>
		EConnector,
		///<summary></summary>
		Broadcaster,
		///<summary></summary>
		BroadcastMonitor,
		///<summary></summary>
		ServiceMainHQ,
	}

	//public class DtoUserodRefresh:DtoQueryBase {
	//}

}
