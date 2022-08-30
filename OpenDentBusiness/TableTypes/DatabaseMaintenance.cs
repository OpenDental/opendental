using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable()]
	public class DatabaseMaintenance:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DatabaseMaintenanceNum;
		///<summary>The name of the databasemaintenance name.</summary>
		public string MethodName;
		///<summary>Set to true to indicate that the method is hidden.</summary>
		public bool IsHidden;
		///<summary>Set to true to indicate that the method is old.</summary>
		public bool IsOld;
		///<summary>Updates the date and time they run the method.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateLastRun;

		///<summary></summary>
		public DatabaseMaintenance Copy() {
			return (DatabaseMaintenance)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum DbmMode {
		///<summary></summary>
		Check = 0,
		///<summary></summary>
		Breakdown = 1,
		///<summary></summary>
		Fix = 2
	}

	///<summary>An attribute that should get applied to any method that needs to show up in the main grid of FormDatabaseMaintenance.
	///Also, an attribute that identifies methods that require a userNum parameter for sending the current user through the middle tier to set the
	///SecUserNumEntry field.</summary>
	[System.AttributeUsage(System.AttributeTargets.Method,AllowMultiple = false)]
	public class DbmMethodAttr:System.Attribute {
		private bool _hasBreakDown;
		private bool _hasPatNum;
		private bool _isCanada;
		private bool _isPatDependent;
		private bool _isOneOff;
		private bool _isReplicationUnsafe;

		///<summary>Set to true if this dbm method needs to be able to show the user a list or break down of items that need manual attention.</summary>
		public bool HasBreakDown {
			get {
				return _hasBreakDown;
			}
			set {
				_hasBreakDown=value;
			}
		}

		/////<summary>Not needed anymore. The usernum can be set from Security.CurUser.UserNum on both Middle Tier client and server (and direct connection).</summary>
		//public bool HasUserNum {
		//	get { return _hasUserNum; }
		//	set { _hasUserNum=value; }
		//}

		///<summary>Set to true if this dbm method needs to be able to run for a specific patient.</summary>
		public bool HasPatNum {
			get {
				return _hasPatNum;
			}
			set {
				_hasPatNum=value;
			}
		}

		///<summary>Set to true if this DBM is only for Canadian customers.</summary>
		public bool IsCanada {
			get {
				return _isCanada;
			}
			set {
				_isCanada=value;
			}
		}

		///<summary>Set to true if this DBM is only meant to be ran once.</summary>
		public bool IsOneOff {
			get {
				return _isOneOff;
			}
			set {
				_isOneOff=value;
			}
		}

		///<summary>Set to true if this DBM method is only for the patient specific DBM tool. This method will not be available to the global DBM tool.</summary>
		public bool IsPatDependent {
			get {
				return _isPatDependent;
			}
			set {
				_isPatDependent=value;
			}
		}

		///<summary>Set to true if this DBM method is not safe to be run on replication servers (Usually if there is an insert or calls an S-Class with an insert).</summary>
		public bool IsReplicationUnsafe {
			get {
				return _isReplicationUnsafe;
			}
			set {
				_isReplicationUnsafe=value;
			}
		}

		public DbmMethodAttr() {
			this._hasBreakDown=false;
			this._hasPatNum=false;
			this._isCanada=false;
			this._isOneOff=false;
			this._isReplicationUnsafe=false;
		}

	}

	///<summary>Sorting class used to sort a MethodInfo list by Name.</summary>
	public class MethodInfoComparer:IComparer<MethodInfo> {

		public MethodInfoComparer() {
		}

		public int Compare(MethodInfo x,MethodInfo y) {
			return x.Name.CompareTo(y.Name);
		}
	}
}
