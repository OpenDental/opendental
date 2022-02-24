using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Tracks the bugs for the mobile apps.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral = true,CrudExcludePrefC = true)]
	public class MobileBug {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileBugNum;
		///<summary>The date/time this row was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeCreated;
		///<summary>The status of the bug (MobileBugStatus - Unknown, Found, Fixed)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public MobileBugStatus BugStatus;
		///<summary>The description of the bug.</summary>
		public string Description;
		///<summary>The version the bug was found. May be blank. Is the same across all different apps.</summary>
		public string ODVersionsFound;
		///<summary>The versions of OD that this bug is fixed in. May be blank. Is the same across all different apps.</summary>
		public string ODVersionsFixed;
		///<summary>The app being released (AppType - Unknown, eClipboard, ODMobile).</summary>
		public Platforms Platforms;
		///<summary>The submitter. Same as Bug.Submitter.</summary>
		public int Submitter;

		///<summary></summary>
		public MobileBug Copy() {
			return (MobileBug)this.MemberwiseClone();
		}
	}

	/// <summary></summary>
	public enum MobileBugStatus {
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		Found,
		///<summary>2</summary>
		Fixed,
	}

	///<summary>Bitwise</summary>
	[Flags]
	public enum Platforms {
		///<summary>0</summary>
		Unknown=0,
		///<summary>1</summary>
		iOS=1,
		///<summary>2</summary>
		Android=2,
		///<summary>3</summary>
		UWP=4,
	}
}
