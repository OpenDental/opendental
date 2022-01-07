using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class UserGroupT {

		///<summary>Inserts the new user group, refreshes the group attaches cache and then returns UserGroupNum</summary>
		public static long CreateUserGroup(string description) {
			UserGroup newGroup=new UserGroup();
			newGroup.Description=description;
			newGroup.UserGroupNum=UserGroups.Insert(newGroup);
			UserGroups.RefreshCache();
			return newGroup.UserGroupNum;
		}
	}
}
