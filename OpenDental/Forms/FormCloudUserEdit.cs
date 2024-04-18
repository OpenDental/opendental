using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Amazon;
using Amazon.IdentityStore;
using Amazon.IdentityStore.Model;
using Amazon.Runtime.Documents;
using CodeBase;


namespace OpenDental {
	public partial class FormCloudUserEdit:FormODBase {
		private CloudUser _cloudUserCur;
		private List<CloudGroup> _listGroups;
		private AmazonIdentityStoreClient _awsClient=new AmazonIdentityStoreClient(new AmazonIdentityStoreConfig() { Profile = new Profile("appstream_machine_role") });
		private List<TmZoneInfo> _listTimeZones=new List<TmZoneInfo>();

		public FormCloudUserEdit(List<CloudGroup> listGroups,CloudUser cloudUserCur,List<TmZoneInfo> listTimeZones) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listGroups=listGroups;
			_cloudUserCur=cloudUserCur;
			_listTimeZones=listTimeZones;
		}

		private void FormCloudUserEdit_Load(object sender,EventArgs e) {
			textUserName.Text=_cloudUserCur.UserName;
			textFirstName.Text=_cloudUserCur.FName;
			textLastName.Text=_cloudUserCur.LName;
			textDisplayName.Text=_cloudUserCur.DisplayName;
			textEmail.Text=_cloudUserCur.Email;
			for(int i=0;i<_listGroups.Count;i++) {
				CloudGroup groupCur=_listGroups[i];
				listGroups.Items.Add(groupCur.DisplayName,groupCur);
				listGroups.SetSelected(i,_cloudUserCur.ListGroups.Any(x => x.GroupId==groupCur.GroupId));
			}
			comboTimezone.Items.AddList(_listTimeZones,x => x.TZInfo.DisplayName,x => x.TZInfo.Id);
			comboTimezone.SetSelected(0);
			if(!_cloudUserCur.TimeZone.IsNullOrEmpty()) {
				int timeZoneIndex=_listTimeZones.FindIndex(x => _cloudUserCur.TimeZone.ToLower().In(x.AwsTZoneName.ToLower(),x.TZInfo.Id.ToLower()));
				if(timeZoneIndex!=-1) {
					comboTimezone.SetSelected(timeZoneIndex);
				}
			}
		}

		private void RefreshCurUser() {
			if(string.IsNullOrEmpty(_cloudUserCur.UserId)) {
				_cloudUserCur=new CloudUser(_cloudUserCur.IdentityStoreId);
				return;
			}
			DescribeUserRequest requestUser=new DescribeUserRequest() {
				IdentityStoreId =_cloudUserCur.IdentityStoreId,
				UserId=_cloudUserCur.UserId
			};
			if(!FormCloudUsers.TryAwsAction(() => _awsClient.DescribeUser(requestUser),out DescribeUserResponse responseUser) || responseUser==null) {
				return;
			}
			string email=(responseUser.Emails.Find(x => x.Primary)??responseUser.Emails.First())?.Value??"";
			_cloudUserCur=new CloudUser(responseUser.IdentityStoreId,responseUser.UserId,responseUser.UserName,responseUser.DisplayName,responseUser.Name.GivenName,
				responseUser.Name.FamilyName,email,responseUser.Timezone);
			ListGroupMembershipsForMemberRequest requestListGroups=new ListGroupMembershipsForMemberRequest() {
				IdentityStoreId=_cloudUserCur.IdentityStoreId,
				MemberId=new MemberId() { UserId=_cloudUserCur.UserId }
			};
			if(!FormCloudUsers.TryAwsAction(() => _awsClient.ListGroupMembershipsForMember(requestListGroups),out ListGroupMembershipsForMemberResponse responseListGroups)
				|| responseListGroups==null)
			{
				return;
			}
			_cloudUserCur.ListGroups.AddRange(_listGroups.FindAll(x => responseListGroups.GroupMemberships.Any(y => y.GroupId==x.GroupId)));
		}

		private bool ValidateFields() {
			List<string> listErrors=new List<string>();
			if(textUserName.Text.Length>128) {
				listErrors.Add(Lan.g(this,"User Name must be 128 characters or less"));
			}
			if(textUserName.Text.Any(x => !char.IsLetterOrDigit(x) && !x.In("+=,.@-_".ToCharArray()))) {
				listErrors.Add(Lan.g(this,"User Name can only contain alphanumeric characters or any of the following")+": +=,.@-_");
			}
			if(listGroups.SelectedIndices.Count==0) {
				listErrors.Add(Lan.g(this,"User must be assigned to at least one group"));
			}
			if(listErrors.Count>0) {
				MsgBox.Show(Lan.g(this,"Please correct the following error(s) first")+":\r\n\r\n"+string.Join("\r\n",listErrors));
				return false;
			}
			return true;
		}

		private bool SyncGroups(List<CloudGroup> listNew,List<CloudGroup> listOld) {
			List<CloudGroup> listAdd=new List<CloudGroup>();
			List<CloudGroup> listRemove=new List<CloudGroup>();
			listNew.Sort((CloudGroup x,CloudGroup y) => x.GroupId.CompareTo(y.GroupId));
			listOld.Sort((CloudGroup x,CloudGroup y) => x.GroupId.CompareTo(y.GroupId));
			int idxNew=0;
			int idxOld=0;
			CloudGroup groupNew;
			CloudGroup groupOld;
			//Both lists sorted by the same criteria, so walk the lists and find elements in one and not the other. If both lists have the CloudGroup, no update necessary.
			//If listNew contains the group but not listOld, create group membership. If listOld contains the group but not listNew, remove the group membership.
			while(idxNew<listNew.Count || idxOld<listOld.Count) {
				groupNew=null;
				if(idxNew<listNew.Count) {
					groupNew=listNew[idxNew];
				}
				groupOld=null;
				if(idxOld<listOld.Count) {
					groupOld=listOld[idxOld];
				}
				if(groupNew!=null && groupOld==null) {//listNew has more items, listOld does not
					listAdd.Add(groupNew);
					idxNew++;
					continue;
				}
				else if(groupNew==null && groupOld!=null) {//listOld has more items, listNew does not
					listRemove.Add(groupOld);
					idxOld++;
					continue;
				}
				else if(groupNew.GroupId.CompareTo(groupOld.GroupId)<0) {//groupNew.GroupId comes before groupOld, groupNew is 'next'
					listAdd.Add(groupNew);
					idxNew++;
					continue;
				}
				else if(groupNew.GroupId.CompareTo(groupOld.GroupId)>0) {//groupOld.GroupId comes before groupNew, groupOld is 'next'
					listRemove.Add(groupOld);
					idxOld++;
					continue;
				}
				//both lists contain the 'next' item, no update necessary
				idxNew++;
				idxOld++;
			}
			for(int i=0;i<listAdd.Count;i++) {
				CreateGroupMembershipRequest requestCreate=new CreateGroupMembershipRequest() {
					GroupId=listAdd[i].GroupId,
					IdentityStoreId=_cloudUserCur.IdentityStoreId,
					MemberId=new MemberId() { UserId=_cloudUserCur.UserId }
				};
				if(!FormCloudUsers.TryAwsAction(() => _awsClient.CreateGroupMembership(requestCreate),out CreateGroupMembershipResponse responseCreate)) {
					return false;
				}
			}
			for(int i=0;i<listRemove.Count;i++) {
				if(!CanRemoveUserFromGroup(listRemove[i].GroupId)) {
					MsgBox.Show(Lan.g(this,"This user is the last user in the group")+" "+listRemove[i].DisplayName+". "+Lan.g(this,"You cannot remove the last user from a group or you will lose all access to the Open Dental Cloud application assigned to that group."));
					return false;
				}
				GetGroupMembershipIdRequest requestGetId=new GetGroupMembershipIdRequest() {
					GroupId=listRemove[i].GroupId,
					IdentityStoreId=_cloudUserCur.IdentityStoreId,
					MemberId=new MemberId() { UserId=_cloudUserCur.UserId }
				};
				if(!FormCloudUsers.TryAwsAction(() => _awsClient.GetGroupMembershipId(requestGetId),out GetGroupMembershipIdResponse responseGetId) || responseGetId==null) {
					return false;
				}
				DeleteGroupMembershipRequest requestDelete=new DeleteGroupMembershipRequest() {
					IdentityStoreId=_cloudUserCur.IdentityStoreId,
					MembershipId=responseGetId.MembershipId
				};
				if(!FormCloudUsers.TryAwsAction(() => _awsClient.DeleteGroupMembership(requestDelete),out DeleteGroupMembershipResponse responseDelete)) {
					return false;
				}
			}
			return true;
		}

		///<summary>Returns the list of GroupIds where this is the last cloud user in that group.  Used to determine if the user can be deleted.</summary>
		private bool TryGetListGroupIdsLastUser(out List<string> listGroupIdsLastUser) {
			listGroupIdsLastUser=new List<string>();
			ListGroupMembershipsForMemberRequest requestListGroups=new ListGroupMembershipsForMemberRequest() {
				IdentityStoreId=_cloudUserCur.IdentityStoreId,
				MemberId=new MemberId() { UserId=_cloudUserCur.UserId }
			};
			if(!FormCloudUsers.TryAwsAction(() => _awsClient.ListGroupMembershipsForMember(requestListGroups),out ListGroupMembershipsForMemberResponse responseListGroups)) {
				return false;
			}
			if(responseListGroups?.GroupMemberships!=null) {
				foreach(string groupId in responseListGroups.GroupMemberships.Select(x => x.GroupId)) {
					if(!CanRemoveUserFromGroup(groupId)) {
						listGroupIdsLastUser.Add(groupId);
					}
				}
			}
			return true;
		}

		///<summary>Returns true if the user is not the last user in a group.  False otherwise.  Used to determine if a user can be removed from a group.</summary>
		private bool CanRemoveUserFromGroup(string groupId) {
			ListGroupMembershipsRequest requestUsersInGroup=new ListGroupMembershipsRequest() {
				IdentityStoreId=_cloudUserCur.IdentityStoreId,
				GroupId=groupId
			};
			if(!FormCloudUsers.TryAwsAction(() => _awsClient.ListGroupMemberships(requestUsersInGroup),out ListGroupMembershipsResponse respUsersInGroup)) {
				return false;
			}
			//If any of the UserIds listed for a group are not _cloudUserCur.UserId we can safely remove this user since it's not the last user in the group
			if(respUsersInGroup?.GroupMemberships!=null && respUsersInGroup.GroupMemberships.Any(x => x.MemberId.UserId!=_cloudUserCur.UserId)) {
				return true;
			}
			return false;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			if(textDisplayName.Text.Length==0) {
				textDisplayName.Text=textFirstName.Text+" "+textLastName.Text;
			}
			if(string.IsNullOrEmpty(_cloudUserCur.UserId)) {//new user, send create user request
				CreateUserRequest requestCreateUser=new CreateUserRequest() {
					IdentityStoreId=_cloudUserCur.IdentityStoreId,
					UserName=textUserName.Text,
					Name=new Name() { GivenName=textFirstName.Text,FamilyName=textLastName.Text },
					DisplayName=textDisplayName.Text,
					Emails=new List<Email>() { new Email() { Primary=true,Value=textEmail.Text } },
					Timezone=comboTimezone.GetSelected<TmZoneInfo>().AwsTZoneName
				};
				if(!FormCloudUsers.TryAwsAction(() => _awsClient.CreateUser(requestCreateUser),out CreateUserResponse responseCreateUser)) {
					RefreshCurUser();
					return;
				}
				_cloudUserCur.UserId=responseCreateUser.UserId;
			}
			else {//existing user, update with form data
				List<AttributeOperation> listOperations=new List<AttributeOperation>();
				if(textUserName.Text!=_cloudUserCur.UserName) {
					listOperations.Add(new AttributeOperation() { AttributePath="userName",AttributeValue=new Document(textUserName.Text) });
				}
				if(textFirstName.Text!=_cloudUserCur.FName) {
					listOperations.Add(new AttributeOperation() { AttributePath="name.givenName",AttributeValue=new Document(textFirstName.Text) });
				}
				if(textLastName.Text!=_cloudUserCur.LName) {
					listOperations.Add(new AttributeOperation() { AttributePath="name.familyName",AttributeValue=new Document(textLastName.Text) });
				}
				if(textDisplayName.Text!=_cloudUserCur.DisplayName) {
					listOperations.Add(new AttributeOperation() { AttributePath="displayName",AttributeValue=new Document(textDisplayName.Text) });
				}
				if(textEmail.Text!=_cloudUserCur.Email) {
					listOperations.Add(new AttributeOperation() { AttributePath="emails",AttributeValue=new Document(Document.FromObject(new Email() { Primary=true,Value=textEmail.Text,Type="work" })) });
				}
				if(_cloudUserCur.TimeZone.IsNullOrEmpty() || comboTimezone.GetSelected<TmZoneInfo>().AwsTZoneName.ToLower()!=_cloudUserCur.TimeZone.ToLower()) {
					listOperations.Add(new AttributeOperation() { AttributePath="timezone",AttributeValue=new Document(comboTimezone.GetSelected<TmZoneInfo>().AwsTZoneName) });
				}
				if(listOperations.Count>0) {
					UpdateUserRequest requestUpdateUser=new UpdateUserRequest() {
						IdentityStoreId=_cloudUserCur.IdentityStoreId,
						UserId=_cloudUserCur.UserId,
						Operations=listOperations
					};
					if(!FormCloudUsers.TryAwsAction(() => _awsClient.UpdateUser(requestUpdateUser),out UpdateUserResponse responseUpdateUser)) {
						RefreshCurUser();
						return;
					}
				}
			}
			bool hasSynchedGroups=SyncGroups(listGroups.GetListSelected<CloudGroup>(),_cloudUserCur.ListGroups);//if new user, _cloudUserCur.ListGroups can be empty
			if(!hasSynchedGroups) {
				RefreshCurUser();
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(_cloudUserCur.UserId)) {//new user, just return
				DialogResult=DialogResult.OK;
			}
			if(!TryGetListGroupIdsLastUser(out List<string> listGroupIdsLastUser)) {
				return;
			}
			string msgText;
			if(listGroupIdsLastUser.Count>0) {
				List<string> listGroupDisplayNames=listGroupIdsLastUser
					.Select(x => _listGroups.Find(y => y.GroupId==x)?.DisplayName)
					.Where(x => !string.IsNullOrEmpty(x)).ToList();
				msgText=Lan.g(this,"This user is the last user in the group(s)")+":\r\n"+string.Join("\r\n",listGroupDisplayNames)+"\r\n"
					+Lan.g(this,"You cannot remove the last user from a group or you will lose all access to the Open Dental Cloud application assigned to that group.");
				MsgBox.Show(msgText);
				return;
			}
			msgText="Deleting this user will remove their access to the Open Dental Cloud application. This action cannot be undone. Continue?";
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
				return;
			}
			DeleteUserRequest deleteUserRequest = new DeleteUserRequest() { IdentityStoreId=_cloudUserCur.IdentityStoreId,UserId=_cloudUserCur.UserId };
			if(!FormCloudUsers.TryAwsAction(() => _awsClient.DeleteUser(deleteUserRequest),out DeleteUserResponse responseDeleteUser)) {
				RefreshCurUser();
				return;
			}
			DialogResult=DialogResult.OK;
		}
	}
}
