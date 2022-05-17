using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class CentralManagerT {
		///<summary>Returns a DisplayReport with the parameters passed in. Defaulted to a non-internal ProdInc report.</summary>
		public static DisplayReport CreateDisplayReport(long displayReportNum,string internalName="",string description="",DisplayReportCategory category=DisplayReportCategory.ProdInc) {
			return new DisplayReport() {
				DisplayReportNum=displayReportNum,
				InternalName=internalName,
				Description=description,
				Category=category
			};
		}

		///<summary>Returns a GroupPermission with the parameters passed in. GroupPermNum is omitted because CEMT sync should not care.</summary>
		public static GroupPermission CreateGroupPermission(long fKey,Permissions permType,long userGroupNum,DateTime newerDate=default(DateTime),int newerDays=0) {
			return new GroupPermission() {
				FKey=fKey,
				PermType=permType,
				UserGroupNum=userGroupNum,
				NewerDate=newerDate,
				NewerDays=newerDays
			};
		}

		///<summary>Returns the inserted GroupPermission. Currently only requires FKey, PermType, and UserGroupNum.</summary>
		public static GroupPermission InsertGroupPermissionNoCache(long FKey,Permissions permType,long userGroupNum) {
			GroupPermission groupPermission=CreateGroupPermission(FKey,permType,userGroupNum);
			GroupPermissions.InsertNoCache(groupPermission);
			return groupPermission;
		}
	}
}
