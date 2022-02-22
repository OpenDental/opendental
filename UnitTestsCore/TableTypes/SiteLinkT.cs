using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class SiteLinkT {

		///<summary></summary>
		public static SiteLink CreateSiteLink(long siteNum,string octetStart="",string connectionSettingsHQOverrides="") {
			SiteLink siteLink=new SiteLink() {
				SiteNum=siteNum,
				OctetStart=octetStart,
				ConnectionSettingsHQOverrides=connectionSettingsHQOverrides
			};
			siteLink.SiteLinkNum=SiteLinks.Insert(siteLink);
			SiteLinks.RefreshCache();
			return siteLink;
		}

		///<summary>Deletes everything from the SiteLink table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearSiteLinkTable() {
			string command="DELETE FROM sitelink WHERE SiteLinkNum > 0";
			DataCore.NonQ(command);
			SiteLinks.RefreshCache();
		}

		public static string GetCreateTableStatement() {
			return @"CREATE TABLE sitelink (
				SiteLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				SiteNum bigint NOT NULL,
				OctetStart varchar(255) NOT NULL,
				EmployeeNum bigint NOT NULL,
				SiteColor int NOT NULL,
				ForeColor int NOT NULL,
				InnerColor int NOT NULL,
				OuterColor int NOT NULL,
				ConnectionSettingsHQOverrides text NOT NULL,
				INDEX(SiteNum),
				INDEX(EmployeeNum)
				) DEFAULT CHARSET=utf8";
		}
	}
}
