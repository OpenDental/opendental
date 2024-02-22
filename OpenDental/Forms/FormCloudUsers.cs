using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Amazon;
using Amazon.IdentityStore;
using Amazon.IdentityStore.Model;
using Amazon.Runtime;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCloudUsers:FormODBase {
		private const string IDENTITY_STORE_ID="d-92674b4f05";
		private List<CloudUser> _listUsers;
		private AmazonIdentityStoreClient _awsClient=new AmazonIdentityStoreClient(new AmazonIdentityStoreConfig() { Profile=new Profile("appstream_machine_role") });

		public FormCloudUsers() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_waitFilterMs=200;//because we are not doing any database calls, we want a lower time to make the application feel more responsive
		}

		private void FormCloudUsers_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGridUsers(false),textSearch);
			FillGridUsers(true);
		}

		private void FillGridUsers(bool doRefreshUserList=false) {
			if(doRefreshUserList) {
				ProgressWin progressOD = new ProgressWin {
					ActionMain=() => RefreshCloudUsers(),
					StartingMessage="Refreshing data..."
				};
				progressOD.ShowDialog();
				if(progressOD.IsCancelled){
					return;
				}
			}
			List<string> listCloudUserIdsSelected=gridUsers.SelectedTags<CloudUser>().Select(x => x.UserId).ToList();
			int indexSortCol=gridUsers.GetSortedByColumnIdx();
			bool isSortAsc=gridUsers.IsSortedAscending();
			int scroll=gridUsers.ScrollValue;
			gridUsers.BeginUpdate();
			gridUsers.Columns.Clear();
			gridUsers.Columns.Add(new GridColumn(Lan.g(this,"User Name"),150) { IsWidthDynamic=true,DynamicWeight=2f });
			gridUsers.Columns.Add(new GridColumn(Lan.g(this,"Display Name"),150) { IsWidthDynamic=true,DynamicWeight=2f });
			gridUsers.Columns.Add(new GridColumn(Lan.g(this,"First Name"),100) { IsWidthDynamic=true });
			gridUsers.Columns.Add(new GridColumn(Lan.g(this,"Last Name"),100) { IsWidthDynamic = true });
			gridUsers.Columns.Add(new GridColumn(Lan.g(this,"Email"),180) { IsWidthDynamic = true,DynamicWeight=2f });
			if(checkShowUserId.Checked) {
				gridUsers.Columns.Add(new GridColumn(Lan.g(this,"User ID"),230));
			}
			gridUsers.ListGridRows.Clear();
			GridRow row;
			List<string> listSearchWords=textSearch.Text.ToLower().Trim().Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
			for(int i=0;i<_listUsers.Count;i++) {
				CloudUser userCur=_listUsers[i];
				//Do not add the row if the user typed something into the search box and no cell contains the text that was typed in.
				List<string> listSearchFields=new[] { userCur.UserName,userCur.DisplayName,userCur.FName,userCur.LName,userCur.Email }
					.Select(x => x.ToLower()).ToList();
				if(listSearchWords.Count>0 && !listSearchWords.All(x => listSearchFields.Any(y => y.Contains(x)))) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(userCur.UserName);
				row.Cells.Add(userCur.DisplayName);
				row.Cells.Add(userCur.FName);
				row.Cells.Add(userCur.LName);
				row.Cells.Add(userCur.Email);
				if(checkShowUserId.Checked) {
					row.Cells.Add(userCur.UserId);
				}
				row.Tag=userCur;
				gridUsers.ListGridRows.Add(row);
			}
			gridUsers.EndUpdate();
			if(indexSortCol>-1 && indexSortCol<gridUsers.Columns.Count) {
				gridUsers.SortForced(indexSortCol,isSortAsc);
			}
			for(int i=0;i<gridUsers.ListGridRows.Count;i++) {
				string userIdCur=((CloudUser)gridUsers.ListGridRows[i].Tag).UserId;
				if(listCloudUserIdsSelected.Contains(userIdCur)) {
					gridUsers.SetSelected(i,true);
				}
			}
			gridUsers.ScrollValue=scroll;
		}

		private void RefreshCloudUsers() {
			_listUsers=new List<CloudUser>();
			ListGroupMembershipsForMemberRequest requestGroupsForUser=new ListGroupMembershipsForMemberRequest() {
				IdentityStoreId=IDENTITY_STORE_ID,
				MemberId=new MemberId() { UserId=Environment.GetEnvironmentVariable("AppStream_UserName",EnvironmentVariableTarget.User) }
			};
			if(!TryAwsAction(() => _awsClient.ListGroupMembershipsForMember(requestGroupsForUser),out ListGroupMembershipsForMemberResponse respGroupsForUser)
				|| respGroupsForUser?.GroupMemberships==null)
			{
				return;
			}
			List<string> listGroupIds=respGroupsForUser.GroupMemberships.Select(x => x.GroupId).Distinct().ToList();
			for(int i=0;i<listGroupIds.Count;i++) {//should be only 1 group
				string groupIdCur=listGroupIds[i];
				DescribeGroupRequest requestDescribeGroup=new DescribeGroupRequest() {
					IdentityStoreId=IDENTITY_STORE_ID,
					GroupId=groupIdCur
				};
				if(!TryAwsAction(() => _awsClient.DescribeGroup(requestDescribeGroup),out DescribeGroupResponse respDescribeGroup) || respDescribeGroup?.DisplayName==null) {
					return;
				}
				string groupDisplayName=respDescribeGroup.DisplayName;
				CloudGroup groupCur=new CloudGroup(groupIdCur,groupDisplayName);
				ListGroupMembershipsRequest requestUsersInGroup=new ListGroupMembershipsRequest() {
					IdentityStoreId=IDENTITY_STORE_ID,
					GroupId=groupIdCur
				};
				if(!TryAwsAction(() => _awsClient.ListGroupMemberships(requestUsersInGroup),out ListGroupMembershipsResponse respUsersInGroup) || respUsersInGroup?.GroupMemberships==null) {
					return;
				}
				for(int j=0;j<respUsersInGroup.GroupMemberships.Count;j++) {
					string userIdCur=respUsersInGroup.GroupMemberships[j].MemberId.UserId;
					CloudUser userCur=_listUsers.Find(x => x.UserId==userIdCur);
					if(userCur==null) {
						DescribeUserRequest requestUser=new DescribeUserRequest() {
							IdentityStoreId =IDENTITY_STORE_ID,
							UserId=userIdCur
						};
						if(!TryAwsAction(() => _awsClient.DescribeUser(requestUser),out DescribeUserResponse respUser) || respUser==null) {
							return;
						}
						string email=(respUser.Emails.Find(x => x.Primary)??respUser.Emails.First())?.Value??"";
						userCur=new CloudUser(IDENTITY_STORE_ID,respUser.UserId,respUser.UserName,respUser.DisplayName,respUser.Name.GivenName,respUser.Name.FamilyName,email);
						_listUsers.Add(userCur);
					}
					userCur.ListGroups.Add(groupCur);
				}
			}
		}

		private void gridUsers_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			List<CloudGroup> listGroups=_listUsers.SelectMany(x => x.ListGroups).DistinctBy(y => y.GroupId).ToList();
			FormCloudUserEdit formCloudUserEdit=new FormCloudUserEdit(listGroups,gridUsers.SelectedTag<CloudUser>());
			formCloudUserEdit.ShowDialog();
			FillGridUsers(true);
		}

		public static bool TryAwsAction<T>(Func<T> f,out T retVal) {
			retVal=default;
			string msgText="";
			try {
				retVal=f();
			}
			catch(Exception ex) {
				msgText="\r\n"+Lan.g("FormCloudUsers","Error returned")+": "+ex.Message;
			}
			if(retVal!=null && (retVal as AmazonWebServiceResponse).HttpStatusCode!=System.Net.HttpStatusCode.OK) {
				msgText="\r\n"+Lan.g("FormCloudUsers","Error code")+": "+(retVal as AmazonWebServiceResponse).HttpStatusCode+msgText;
			}
			if(!string.IsNullOrEmpty(msgText)) {
				msgText=Lan.g("FormCloudUsers","There was an error executing the command.")+msgText;
				MsgBox.Show(msgText);
				return false;
			}
			return true;
		}

		///<summary>Returns the list of GroupIds where this is the last cloud user in that group.  Used to determine if the user can be deleted.</summary>
		private bool TryGetListGroupIdsLastUser(CloudUser cloudUser,out List<string> listGroupIdsLastUser) {
			listGroupIdsLastUser=new List<string>();
			ListGroupMembershipsForMemberRequest requestListGroups=new ListGroupMembershipsForMemberRequest() {
				IdentityStoreId=cloudUser.IdentityStoreId,
				MemberId=new MemberId() { UserId=cloudUser.UserId }
			};
			if(!TryAwsAction(() => _awsClient.ListGroupMembershipsForMember(requestListGroups),out ListGroupMembershipsForMemberResponse responseListGroups)) {
				return false;
			}
			if(responseListGroups?.GroupMemberships!=null) {
				foreach(string groupId in responseListGroups.GroupMemberships.Select(x => x.GroupId)) {
					if(!CanRemoveUserFromGroup(cloudUser,groupId)) {
						listGroupIdsLastUser.Add(groupId);
					}
				}
			}
			return true;
		}

		///<summary>Returns true if the user is not the last user in a group.  False otherwise.  Used to determine if a user can be removed from a group.</summary>
		private bool CanRemoveUserFromGroup(CloudUser cloudUser,string groupId) {
			ListGroupMembershipsRequest requestUsersInGroup=new ListGroupMembershipsRequest() {
				IdentityStoreId=cloudUser.IdentityStoreId,
				GroupId=groupId
			};
			if(!TryAwsAction(() => _awsClient.ListGroupMemberships(requestUsersInGroup),out ListGroupMembershipsResponse respUsersInGroup)) {
				return false;
			}
			//If any of the UserIds listed for a group are not _cloudUserCur.UserId we can safely remove this user since it's not the last user in the group
			if(respUsersInGroup?.GroupMemberships!=null && respUsersInGroup.GroupMemberships.Any(x => x.MemberId.UserId!=cloudUser.UserId)) {
				return true;
			}
			return false;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			List<CloudGroup> listGroups=_listUsers.SelectMany(x => x.ListGroups).DistinctBy(y => y.GroupId).ToList();
			FormCloudUserEdit formCloudUserEdit=new FormCloudUserEdit(listGroups,new CloudUser(IDENTITY_STORE_ID));
			formCloudUserEdit.ShowDialog();
			FillGridUsers(true);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridUsers.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Select users to delete first");
				return;
			}
			string msgText="Deleting users will remove their access to the Open Dental Cloud application. This action cannot be undone. Continue?";
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
				return;
			}
			List<CloudUser> listDeleteUsers=gridUsers.SelectedTags<CloudUser>();
			List<CloudUser> listCannotDelete=new List<CloudUser>();
			for(int i=0;i<listDeleteUsers.Count;i++) {
				CloudUser userCur=listDeleteUsers[i];
				if(!TryGetListGroupIdsLastUser(userCur,out List<string> listGroupIdsLastUser)) {
					return;
				}
				if(listGroupIdsLastUser.Count>0) {
					listCannotDelete.Add(userCur);
					continue;
				}
				DeleteUserRequest requestDeleteUser=new DeleteUserRequest() { IdentityStoreId=userCur.IdentityStoreId,UserId=userCur.UserId };
				if(!TryAwsAction(() => _awsClient.DeleteUser(requestDeleteUser),out DeleteUserResponse responseUser)) {
					continue;
				}
			}
			if(listCannotDelete.Count>0) {
				msgText=Lan.g(this,"Some of the selected users cannot be deleted because they are the last user in one or more of their assigned groups.")+"\r\n"
					+Lan.g(this,"You cannot remove the last user from a group or you will lose all access to the Open Dental Cloud application assigned to that group.")+"\r\n"
					+Lan.g(this,"Users not deleted")+":\r\n\r\n"+string.Join("\r\n",listCannotDelete.Select(x => x.UserName));
				MsgBox.Show(msgText);
			}
			FillGridUsers(true);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridUsers(true);
		}

		private void checkShowUserId_CheckedChanged(object sender,EventArgs e) {
			FillGridUsers(false);
		}
	}

	public class CloudUser {
		public string IdentityStoreId;
		public string UserId;
		public string UserName;
		public string DisplayName;
		public string FName;
		public string LName;
		public string Email;
		public List<CloudGroup> ListGroups;

		public CloudUser(string identityStoreId="",string userId="",string userName="",string displayName="",string fName="",string lName="",string email="") {
			IdentityStoreId=identityStoreId;
			UserId=userId;
			UserName=userName;
			DisplayName=displayName;
			FName=fName;
			LName=lName;
			Email=email;
			ListGroups=new List<CloudGroup>();
		}
	}

	public class CloudGroup {
		public string GroupId;
		public string DisplayName;

		public CloudGroup(string groupId,string displayName) {
			GroupId=groupId;
			DisplayName=displayName;
		}
	}
}