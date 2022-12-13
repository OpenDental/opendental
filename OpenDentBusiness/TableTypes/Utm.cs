using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>A UTM (urchin tracking module) code is a simple string that you can add to the end of a URL to track the performance of campaigns and content.</summary>
	[Serializable]
	public class Utm:TableBase {
		///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long UtmNum;
		///<summary>Text that identifies a specific campaign or promotion identifying why traffic is being directed to the users website.</summary>
		public string CampaignName;
		///<summary>Text that tracks how traffic is getting to the users website, such as email or social media.</summary>
		public string MediumInfo;
		///<summary>Text that identifies where traffic is originating from.</summary>
		public string SourceInfo;

		///<summary></summary>
		public Utm Copy() {
			return (Utm)MemberwiseClone();
		}
	}
}
