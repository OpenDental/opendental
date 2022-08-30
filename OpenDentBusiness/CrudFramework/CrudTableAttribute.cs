using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Crud table attributes cannot be used by inherited classes because some properties would not work if they were inherited.
	///Simply add the desired attributes to the "inheriting" class which will effectively override the attribute.</summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=false)]
	public class CrudTableAttribute : Attribute {
		public CrudTableAttribute() {
			this.tableName="";
			this.isDeleteForbidden=false;
			this.isMissingInGeneral=false;
			this.isMobile=false;
			this.isSynchable=false;
			this.IsSynchableBatchWriteMethods=false;
			this._auditPerms=CrudAuditPerm.None;
			this._isSecurityStamped=false;
			this._hasBatchWriteMethods=false;
			this._isTableHist=false;
		}

		private string tableName;
		///<summary>If tablename is different than the lowercase class name.</summary>
		public string TableName {
			get { return tableName; }
			set { tableName=value; }
		}

		private bool isDeleteForbidden;
		///<summary>Set to true for tables where rows are not deleted.</summary>
		public bool IsDeleteForbidden {
			get { return isDeleteForbidden; }
			set { isDeleteForbidden=value; }
		}

		private bool isMissingInGeneral;
		///<summary>Set to true for tables that are part of internal tools and not found in the general release.  The Crud generator will gracefully skip these tables if missing from the database that it's running against.  It also won't try to generate a dataInterface s class.</summary>
		public bool IsMissingInGeneral {
			get { return isMissingInGeneral; }
			set { isMissingInGeneral=value; }
		}

		private bool isMobile;
		///<summary>Set to true for tables that are used on server for mobile services.  These are 'lite' versions of the main tables, and end with m.  A composite primary key will be expected.  The Crud generator will generate these crud files in a different place than the other crud files.  It will also generate the dataInterface 'ms' class to a different location.  It also won't validate that the table exists in the test database.</summary>
		public bool IsMobile {
			get { return isMobile; }
			set { isMobile=value; }
		}

		private bool isSynchable;
		public bool IsSynchable {
			get { return isSynchable; }
			set { isSynchable=value; }
		}

		///<summary>This attribute, used with or without IsSynchable, will result in a sync method being added to the crud file and if the attribute
		///HasBatchWriteMethods is true the sync will use InsertMany instead of inserting objects one at a time.  This is more efficient but has the
		///drawback of the objects inserted not being updated with their primary keys.  So only use this attribute if you do not require the inserted
		///object's in the new list to have primary keys when the sync is done.  e.g. when a form is closing and you don't use the new list outside the
		///cosing form.</summary>
		public bool IsSynchableBatchWriteMethods { get; set; }

		private CrudAuditPerm _auditPerms;
		///<summary>Enum containing all permissions used as an FKey entry for the Securitylog table.
		///The Crud generator uses these to add an additional function call to Delete(), and a new function ClearFkey() to ensure that securitylog FKeys 
		///  are not orphaned.</summary>
		public CrudAuditPerm AuditPerms {
			get { return _auditPerms; }
			set { _auditPerms=value; }
		}

		private bool _isSecurityStamped;
		///<summary>If IsSecurityStamped is true, the table must include the field SecUserNumEntry.
		///<para>If IsSynchable and IsSecurityStamped are BOTH true, the Crud generator will create a Sync function that takes userNum and sets the
		///SecUserNumEntry field before inserting.  Security.CurUser isn't accessible from the Crud due to remoting role, must be passed in.</para>
		///<para>IsSecurityStamped is ignored if IsSynchable is false.</para></summary>
		public bool IsSecurityStamped {
			get { return _isSecurityStamped; }
			set { _isSecurityStamped=value; }
		}

		private bool _hasBatchWriteMethods;
		public bool HasBatchWriteMethods {
			get { return _hasBatchWriteMethods; }
			set { _hasBatchWriteMethods=value; }
		}

		private string _crudLocationOverride;
		///<summary>If crud file location is different than .../OpenDentBusiness/Crud.</summary>
		public string CrudLocationOverride {
			get { return _crudLocationOverride; }
			set { _crudLocationOverride=value; }
		}

		private string _namespaceOverride;
		///<summary>If namespace is different than OpenDentBusiness.Crud.</summary>
		public string NamespaceOverride {
			get { return _namespaceOverride; }
			set { _namespaceOverride=value; }
		}
		
		/// <summary>True if PrefC should not be used in CRUD file.</summary>
		private bool _crudExcludePrefC;
		public bool CrudExcludePrefC {
			get { return _crudExcludePrefC; }
			set { _crudExcludePrefC=value; }
		}

		///<summary>No not use for OD proper. True if the table is a archive/historical copy from another table and as such requires inherited DateTime columns (those from the other table) to be inserted as they are and not to replace them with NOW() or the current timestamp.</summary>
		private bool _isTableHist;
		public bool IsTableHist {
			get { return _isTableHist; }
			set { _isTableHist=value; }
		}

		///<summary>True if the table is marked as a large table. These are tables that are a minimum of 500MB-1GB in size. When adding a column 
		///to the tables with this attribute set to true, the LargeTableHelper will be used in order to break large alter table statements into smaller
		///transactions in order to adhere to the Galera cluster transaction size limit of 2 GB.  This strategy is NOT a faster way to alter tables and
		///is strictly due to the transaction size limit imposed by Galera cluster environments for node syncing purposes.</summary>
		private bool _isLargeTable;
		public bool IsLargeTable {
			get { return _isLargeTable; }
			set { _isLargeTable=value; }
		}

		///<summary>True if the CrudGenerator should produce a RowToObj method for use as an alternative to the TableToList(GetTable(command)) pattern.
		///The TableToList(GetTable) pattern causes two copies of the data to be held in memory, one copy as a DataTable and one as the list of objects.
		///Tables with UsesDataReader=true will have the option of calling Db.GetList(command,RowToObj) that uses a DataReader and converts DataRows
		///(IDataRecords) into objects one at a time to reduce the memory required to get a list of objects.</summary>
		public bool UsesDataReader { get; set; } = false;

		///<summary>Returns a bitwise enum that represents all permissions used by the security log FKey column for the class passed in.</summary>
		public static CrudAuditPerm GetCrudAuditPermForClass(Type typeClass) {
			object[] attributes=typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return CrudAuditPerm.None;
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).AuditPerms!=CrudAuditPerm.None) {
					return ((CrudTableAttribute)attributes[i]).AuditPerms;
				}
			}
			//couldn't find any.
			return CrudAuditPerm.None;
		}

		///<summary>The name of the table in the database.  By default, the lowercase name of the class type.</summary>
		public static string GetTableName(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return typeClass.Name.ToLower();
			}
			for(int i=0;i<attributes.Length;i++) {
				if(attributes[i].GetType()!=typeof(CrudTableAttribute)) {
					continue;
				}
				if(((CrudTableAttribute)attributes[i]).TableName!="") {
					return((CrudTableAttribute)attributes[i]).TableName;
				}
			}
			//couldn't find any override.
			return typeClass.Name.ToLower();
		}

	}

	///<summary>Hard coded list of all permission names that are used for securitylog.FKey.  Uses 2^n values for use in bitwise operations.
	///This enum can only hold 31 permissions (and none) before we will need to create a new one.  Instead of creating a new enum, we could instead
	///create a new table to hold a composite key between the permission type the table name and foreign key.</summary>
	[Flags]
	public enum CrudAuditPerm {
		///<summary>Perm#:0 - Value:0</summary>
		None=0,
		///<summary>Perm#:1 - Value:1</summary>
		AppointmentCompleteEdit=1,
		///<summary>Perm#:2 - Value:2</summary>
		AppointmentCreate=2,
		///<summary>Perm#:3 - Value:4</summary>
		AppointmentEdit=4,
		///<summary>Perm#:4 - Value:8</summary>
		AppointmentMove=8,
		///<summary>Perm#:5 - Value:16</summary>
		ClaimHistoryEdit=16,
		///<summary>Perm#:6 - Value:32</summary>
		ImageDelete=32,
		///<summary>Perm#:7 - Value:64</summary>
		ImageEdit=64,
		///<summary>Perm#:8 - Value:128</summary>
		InsPlanChangeCarrierName=128,
		///<summary>Perm#:9 - Value:256</summary>
		RxCreate=256,
		///<summary>Perm#:10 - Value:512</summary>
		RxEdit=512,
		///<summary>Perm#:11 - Value:1024</summary>
		TaskNoteEdit=1024,
		///<summary>Perm#:12 - Value:2048</summary>
		PatientPortal=2048,
		///<summary>Perm#:13 - Value:4096</summary>
		ProcFeeEdit=4096,
		///<summary>Perm#:14 - Value:8192</summary>
		LogFeeEdit=8192,
		///<summary>Perm#15 - Value:16384</summary>
		LogSubscriberEdit=16384
	}

}
