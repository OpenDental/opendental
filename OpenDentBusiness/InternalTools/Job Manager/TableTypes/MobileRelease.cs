using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This schema is copied directly from JRMT. Do not rename columns here.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true)]
	public class MobileRelease {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileReleaseNum;
		///<summary>Major release number.</summary>
		public int MajorNum;
		///<summary>Minor release number.</summary>
		public int MinorNum;
		///<summary>Build number.</summary>
		public int BuildNum;
		///<summary>The app being released (AppType - Unknown, eClipboard, ODMobile).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public AppType AppType;
		///<summary>The release notes associated to the build.</summary>
		public string ReleaseNote;
		///<summary>The date/time this row was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeCreated;

		///<summary></summary>
		public MobileRelease Copy() {
			return (MobileRelease)this.MemberwiseClone();
		}
	}

	/// <summary></summary>
	public enum AppType {
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		eClipboard,
		///<summary>2</summary>
		ODMobile,
	}
}
