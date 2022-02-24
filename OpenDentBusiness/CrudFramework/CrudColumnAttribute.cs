using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	[AttributeUsage(AttributeTargets.Field,AllowMultiple=false)]
	public class CrudColumnAttribute : Attribute {
		public CrudColumnAttribute() {
			this.isPriKey=false;
			this.specialType=CrudSpecialColType.None;
			this.isNotDbColumn=false;
			this.isPriKeyMobile1=false;
			this.isPriKeyMobile2=false;
			this.isNotCemtColumn=false;
			this.isCemtSyncKey=false;
			this.defaultEnumAsString=0;
		}

		private bool isPriKey;
		public bool IsPriKey {
			get { return isPriKey; }
			set { isPriKey=value; }
		}

		private CrudSpecialColType specialType;
		public CrudSpecialColType SpecialType {
			get { return specialType; }
			set { specialType=value; }
		}

		private bool isNotDbColumn;
		public bool IsNotDbColumn {
			get { return isNotDbColumn; }
			set { isNotDbColumn=value; }
		}

		private bool isPriKeyMobile1;
		///<summary>Always present in a mobile table.  Always CustomerNum, FK to PatNum.</summary>
		public bool IsPriKeyMobile1 {
			get { return isPriKeyMobile1; }
			set { isPriKeyMobile1=value; }
		}

		private bool isPriKeyMobile2;
		///<summary>Always present in a mobile table.  Always the ordinary priKey of the table, used together with CustomerNum.</summary>
		public bool IsPriKeyMobile2 {
			get { return isPriKeyMobile2; }
			set { isPriKeyMobile2=value; }
		}

		private bool isNotCemtColumn;
		///<summary>A userod column that is not synced to remote databases for CEMT.</summary>
		public bool IsNotCemtColumn {
			get { return isNotCemtColumn; }
			set { isNotCemtColumn=value; }
		}

		private bool isCemtSyncKey;
		///<summary>A field that is used instead of the PK to update remote databases for CEMT. Required when there is at least one field flagged as IsNotCemtColumn.</summary>
		public bool IsCemtSyncKey {
			get { return isCemtSyncKey; }
			set { isCemtSyncKey=value; }
		}

		private int defaultEnumAsString;
		///<summary>For use with EnumAsString. Used to set the default enum if one can't be found / parsed.</summary>
		public int DefaultEnumAsString {
			get { return defaultEnumAsString; }
			set { defaultEnumAsString=value; }
		}
	}

///<summary>There are also some patterns we follow that do not require special types.  For enums, crud automatically generates tinyint.  For itemorders, we manually change mysql type to smallint?.  </summary>
[Flags]
	public enum CrudSpecialColType {
		None=0,
		///<summary>User not allowed to change.  Insert uses NOW(), Update exludes this column, Select treats this like a date.</summary>
		DateEntry=1,
		///<summary>Insert uses NOW(), Update and Select treat this like a Date.</summary>
		DateEntryEditable=2,
		///<summary>Is set and updated by MySQL.  Leave these columns completely out of Insert and Update statements.  The name of the column must be exactly: DateTStamp for the crud schema logic to work.</summary>
		TimeStamp=4,
		///<summary>Same C# type as Date, but the MySQL database uses datetime instead of date.</summary>
		DateT=8,
		///<summary>User not allowed to change.  Insert uses NOW(), Update exludes this column, Select treats this like a DateT.</summary>
		DateTEntry=16,
		///<summary>Insert uses NOW(), Update and Select treat this like a DateT.</summary>
		DateTEntryEditable=32,
		///<summary>Database type is tinyint signed.  C# type is int.  Range -128 to 127.  The validation does not check to make sure the db is signed.  The programmer must do that.  So far, only used for percent fields because -1 is required to be accepted.  For most other fields, such as enums and itemorders, the solution is to change the field in C# to a byte, indicating a range of 0-255.  It usually doesn't matter whether the database accepts values to 255 or only to 127.  The validation does not check that.</summary>
		TinyIntSigned=64,
		///<summary>We do not want this column updated except as part of a separate routine.  Warning! The logic fails if this is used on the last column in a table.</summary>
		ExcludeFromUpdate=128,
		///<summary>Instead of storing this enum as an int in the db, it is stored as a string.  Very rarely used.</summary>
		EnumAsString=256,
		///<summary>DEPRECATED. See TimeSpanLong. For most C# TimeSpans, the default db type is TimeOfDay.  But for the few that need to use negative values or values greater than 24 hours, they get marked as this special type.  Handled differently in MySQL vs Oracle.</summary>
		TimeSpanNeg=512,
		///<summary>Many C# strings are varchar(255).  Most longer ones are mysql text or oracle varchar2.  But if they might go over 4000 char, then in oracle, they must be clob.  Clobs are handled significantly differently in oracle, so we are tracking those.  There is also a consideration for columns in mysql that might go over 65,000 char, but we do not need to track those in C#.</summary>
		TextIsClob=1024,
		///<summary>If the text contains 50 or more consecutive new line characters, the insert and update crud calls will replace them with a single 
		///new line.  If the control tries to display a very large number of new line characters, graphics memory errors may occur when trying to measure 
		///the height of the string.  This attribute should only be used for large note fields that the user is allowed to type into manually.  Not for 
		///base64 text or other programatically generated text. Additionally null characters will be removed from the text.</summary>
		CleanText=2048,
		/// <summary>MySQL 5.5 limits TimeSpan to 838:59:59 which is 34 days 6 hours. Use this tag to store the timespan as a bigint/long in the DB as a number of ticks. 1 Tick = 1/10 ms.</summary>
		TimeSpanLong=4096,
	}
}
