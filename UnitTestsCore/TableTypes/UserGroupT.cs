using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class UserGroupT {

		///<summary>Inserts the new user group, refreshes the group cache and then returns UserGroupNum</summary>
		public static long CreateUserGroup(string description,bool isCEMT=false) {
			UserGroup newGroup=new UserGroup();
			newGroup.Description=description;
			newGroup.UserGroupNum=UserGroups.Insert(newGroup);
			if(isCEMT) {
				newGroup.UserGroupNumCEMT=newGroup.UserGroupNum;
			}
			UserGroups.Update(newGroup);
			UserGroups.RefreshCache();
			return newGroup.UserGroupNum;
		}

		public static void ClearUserGroupTable() {
			string command="DELETE FROM UserGroup";
			DataCore.NonQ(command);
		}
	}
}
