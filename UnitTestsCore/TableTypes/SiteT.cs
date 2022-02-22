using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class SiteT {

		///<summary></summary>
		public static Site CreateSite(string description) {
			Site site=new Site() {
				Description=description
			};
			site.SiteNum=Sites.Insert(site);
			Sites.RefreshCache();
			return site;
		}

		///<summary>Deletes everything from the SiteLink table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearSiteTable() {
			string command="DELETE FROM site WHERE SiteNum > 0";
			DataCore.NonQ(command);
			Sites.RefreshCache();
		}
	}
}
