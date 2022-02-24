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
	public class MobileReleaseBuild {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileReleaseBuildNum;
		///<summary>The mobile release number.</summary>
		public long MobileReleaseNum;
		///<summary>The build version for this app store release (in iOS it's a version, in Android it's an int).</summary>
		public string BuildVersion;
		///<summary>The app type being built (AppType - Unknown, eClipboard, ODMobile).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public AppType AppType;
		///<summary>The app store being released to (AppStore - Unknown, AppleAppStore, GooglePlayStore).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public AppStore AppStore;
		///<summary>The status of this release (StoreStatus - Unknown, InReview, Rejected, Released).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public StoreStatus StoreStatus;
		///<summary>The last time the status of this release changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStatusChanged;

		///<summary>Parses BuildVersion to a sortable Version according to the app store.</summary>
		public Version AppVersion {
			get {
				Version retVal=new Version();
				if(AppStore==AppStore.AppleAppStore || AppStore==AppStore.WindowsStore) {
					Version.TryParse(BuildVersion,out retVal);
				}
				else if(AppStore==AppStore.GooglePlayStore) {
					int buildInt;
					if(int.TryParse(BuildVersion,out buildInt)) {
						retVal=new Version("0.0.0."+buildInt);
					}
				}
				return retVal;
			}
		}

		///<summary></summary>
		public MobileReleaseBuild Copy() {
			return (MobileReleaseBuild)this.MemberwiseClone();
		}
	}

	/// <summary></summary>
	public enum AppStore {
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		AppleAppStore,
		///<summary>2</summary>
		GooglePlayStore,
		///<summary>3</summary>
		WindowsStore,
	}

	/// <summary></summary>
	public enum StoreStatus {
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		InReview,
		///<summary>2</summary>
		Rejected,
		///<summary>3</summary>
		Released,
	}
}