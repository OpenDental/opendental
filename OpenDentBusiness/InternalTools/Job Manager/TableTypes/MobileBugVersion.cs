using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Tracks the bugs for the mobile apps.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral = true,CrudExcludePrefC = true)]
	public class MobileBugVersion {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileBugVersionNum;
		///<summary>The mobile bug this version information relates to.</summary>
		public long MobileBugNum;
		///<summary>The status of the bug (Same as MobileBug BugStatus).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public MobileBugStatus BugStatus;
		///<summary>The version as it shows on the app store that this bug was found in.</summary>
		public string MobileVersionFound;
		///<summary>The version as it shows on the app store that this bug is fixed in or will be fixed in.</summary>
		public string MobileVersionFixed;
		///<summary>The app that this mobile bug affects (Same as MobileRelease AppType).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public AppType AppType;

		///<summary></summary>
		public MobileBugVersion Copy() {
			return (MobileBugVersion)this.MemberwiseClone();
		}
	}
}
