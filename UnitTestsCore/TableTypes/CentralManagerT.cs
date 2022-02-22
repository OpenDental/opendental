using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class CentralManagerT {
		///<summary>Deletes everything from the grouppermission table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearGroupPermissionTable() {
			//The first ~300 group permissions are from the original SQL dump (and initialization) and need to stick around no matter what.
			string command="DELETE FROM grouppermission WHERE GroupPermNum > 293";
			DataCore.NonQ(command);
		}

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
