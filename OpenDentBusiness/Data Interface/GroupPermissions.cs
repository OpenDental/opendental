using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using CodeBase;
namespace OpenDentBusiness{
	///<summary></summary>
	public class GroupPermissions {
		///<summary>The maximum number of days allowed for the NewerDays column.
		///Setting a NewerDays to a value higher than this will cause an exception to be thrown in the program.
		///There is a DBM that will correct invalid NewerDays in the database.</summary>
		public const double NewerDaysMax=3000;

		#region Misc Methods
		///<summary>Returns the Date that the user is restricted to for the passed-in permission. 
		///Returns MinVal if the user is not restricted or does not have the permission.</summary>
		public static DateTime GetDateRestrictedForPermission(EnumPermType enumPermType,List<long> listUserGroupNums) {
			Meth.NoCheckMiddleTierRole();
			DateTime nowDate=DateTime.MinValue;
			Func<DateTime> getNowDate=new Func<DateTime>(() => {
				if(nowDate.Year < 1880) {
					nowDate=MiscData.GetNowDateTime().Date;
				}
				return nowDate;
			});
			DateTime dateTimeRet=DateTime.MinValue;
			List<GroupPermission> listGroupPermissions=GetForUserGroups(listUserGroupNums,enumPermType);
			//get the permission that applies
			GroupPermission groupPermission=listGroupPermissions.OrderBy((GroupPermission y) => {
				if(y.NewerDays==0 && y.NewerDate==DateTime.MinValue) {
					return DateTime.MinValue;
				}
				if(y.NewerDays==0) {
					return y.NewerDate;
				}
				return getNowDate().AddDays(-y.NewerDays);
			}).FirstOrDefault();
			if(groupPermission==null) {
				//do not change retVal. The user does not have the permission.
			}
			else if(groupPermission.NewerDate.Year < 1880 && groupPermission.NewerDays == 0) {
				//do not change retVal. The user is not restricted by date.
			}
			else if(groupPermission.NewerDate.Year > 1880) {
				dateTimeRet=groupPermission.NewerDate;
			}
			else if(getNowDate().AddDays(-groupPermission.NewerDays)>dateTimeRet) {
				dateTimeRet=getNowDate().AddDays(-groupPermission.NewerDays);
			}
			return dateTimeRet;
		}

		///<summary>Used for procedures with status EO, EC, or C. Returns Permissions.ProcExistingEdit for EO/EC</summary>
		public static EnumPermType SwitchExistingPermissionIfNeeded(EnumPermType enumPermType,Procedure procedure) {
			if(procedure.ProcStatus.In(ProcStat.EO,ProcStat.EC)) {
				return EnumPermType.ProcExistingEdit;
			}
			return enumPermType;
		}
		#endregion

		#region CachePattern

		private class GroupPermissionCache : CacheListAbs<GroupPermission> {
			protected override List<GroupPermission> GetCacheFromDb() {
				string command="SELECT * FROM grouppermission";
				return Crud.GroupPermissionCrud.SelectMany(command);
			}
			protected override List<GroupPermission> TableToList(DataTable table) {
				return Crud.GroupPermissionCrud.TableToList(table);
			}
			protected override GroupPermission Copy(GroupPermission GroupPermission) {
				return GroupPermission.Copy();
			}
			protected override DataTable ListToTable(List<GroupPermission> listGroupPermissions) {
				return Crud.GroupPermissionCrud.ListToTable(listGroupPermissions,"GroupPermission");
			}
			protected override void FillCacheIfNeeded() {
				GroupPermissions.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static GroupPermissionCache _GroupPermissionCache=new GroupPermissionCache();

		public static GroupPermission GetFirstOrDefault(Func<GroupPermission,bool> match,bool isShort=false) {
			return _GroupPermissionCache.GetFirstOrDefault(match,isShort);
		}

		public static List<GroupPermission> GetWhere(Predicate<GroupPermission> match,bool isShort=false) {
			return _GroupPermissionCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_GroupPermissionCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_GroupPermissionCache.FillCacheFromTable(table);
				return table;
			}
			return _GroupPermissionCache.GetTableFromCache(doRefreshCache);
		}

		///<summary>Clears the cache.</summary>
		public static void ClearCache() {
			_GroupPermissionCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static void Update(GroupPermission groupPermission){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupPermission);
				return;
			}
			if(groupPermission.NewerDate.Year>1880 && groupPermission.NewerDays>0) {
				throw new Exception(Lans.g("GroupPermissions","Date or days can be set, but not both."));
			}
			if(!GroupPermissions.PermTakesDates(groupPermission.PermType)) {
				if(groupPermission.NewerDate.Year>1880 || groupPermission.NewerDays>0) {
					throw new Exception(Lans.g("GroupPermissions","This type of permission may not have a date or days set."));
				}
			}
			Crud.GroupPermissionCrud.Update(groupPermission);
		}

		///<summary>Update that doesn't use the local cache or validation. Useful for multiple database connections.</summary>
		public static void UpdateNoCache(GroupPermission groupPermission) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupPermission);
				return;
			}
			Crud.GroupPermissionCrud.Update(groupPermission);
		}

		///<summary>Deletes GroupPermissions based on primary key.  Do not call this method unless you have checked specific dependencies first.  E.g. after deleting this permission, there will still be a security admin user.  This method is only called from the CEMT sync.  RemovePermission should probably be used instead.</summary>
		public static void Delete(GroupPermission groupPermission) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupPermission);
				return;
			}
			string command="DELETE FROM grouppermission WHERE GroupPermNum = "+POut.Long(groupPermission.GroupPermNum);
			Db.NonQ(command);
		}

		///<summary>Deletes without using the cache. Cannot trust GroupPermNum when dealing with remote DB so we rely on every other field to check.</summary>
		public static void DeleteNoCache(GroupPermission groupPermission) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupPermission);
				return;
			}
			string command=$@"DELETE FROM grouppermission 
				WHERE NewerDate={POut.Date(groupPermission.NewerDate)} 
				AND NewerDays={POut.Int(groupPermission.NewerDays)} 
				AND UserGroupNum={POut.Long(groupPermission.UserGroupNum)} 
				AND PermType={POut.Int((int)groupPermission.PermType)} 
				AND FKey={POut.Long(groupPermission.FKey)}";
			Db.NonQ(command);
		}

		///<summary>Delete all GroupPermissions for the specified PermType and UserGroupNum.</summary>
		public static void DeleteForPermTypeAndUserGroup(EnumPermType enumPermType,long userGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),enumPermType,userGroupNum);
				return;
			}
			string command="DELETE FROM grouppermission WHERE PermType="+POut.Enum<EnumPermType>(enumPermType)+" AND UserGroupNum="+POut.Long(userGroupNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(GroupPermission groupPermission){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				groupPermission.GroupPermNum=Meth.GetLong(MethodBase.GetCurrentMethod(),groupPermission);
				return groupPermission.GroupPermNum;
			}
			if(groupPermission.NewerDate.Year>1880 && groupPermission.NewerDays>0) {
				throw new Exception(Lans.g("GroupPermissions","Date or days can be set, but not both."));
			}
			if(!GroupPermissions.PermTakesDates(groupPermission.PermType)) {
				if(groupPermission.NewerDate.Year>1880 || groupPermission.NewerDays>0) {
					throw new Exception(Lans.g("GroupPermissions","This type of permission may not have a date or days set."));
				}
			}
			if(groupPermission.PermType!=EnumPermType.SecurityAdmin){
				return Crud.GroupPermissionCrud.Insert(groupPermission);
			}
			//Make sure there are no hidden users in the group that is about to get the Security Admin permission.
			string command="SELECT COUNT(*) FROM userod "
				+"INNER JOIN usergroupattach ON usergroupattach.UserNum=userod.UserNum "
				+"WHERE userod.IsHidden=1 "
				+"AND usergroupattach.UserGroupNum="+groupPermission.UserGroupNum;
			int count=PIn.Int(Db.GetCount(command));
			if(count!=0) {//there are hidden users in this group
				throw new Exception(Lans.g("FormSecurity","The Security Admin permission cannot be given to a user group with hidden users."));
			}
			return Crud.GroupPermissionCrud.Insert(groupPermission);
		}

		///<summary>Insertion logic that doesn't use the cache. Always ignores the PK and relies on auto-increment.</summary>
		public static long InsertNoCache(GroupPermission groupPermission) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),groupPermission);
			}
			string command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) VALUES (" 
				+POut.Date(groupPermission.NewerDate)+","
				+POut.Int(groupPermission.NewerDays)+","
				+POut.Long(groupPermission.UserGroupNum)+","
				+POut.Int((int)groupPermission.PermType)+","
				+POut.Long(groupPermission.FKey)+")";
			return Db.GetLong(command);
		}

		///<summary></summary>
		public static void RemovePermission(long userGroupNum,EnumPermType enumPermType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroupNum,enumPermType);
				return;
			}
			string command;
			if(enumPermType==EnumPermType.SecurityAdmin){
				//need to make sure that at least one other user has this permission
				command="SELECT COUNT(*) FROM (SELECT DISTINCT grouppermission.UserGroupNum "
					+"FROM grouppermission "
					+"INNER JOIN usergroupattach ON usergroupattach.UserGroupNum=grouppermission.UserGroupNum "
					+"INNER JOIN userod ON userod.UserNum=usergroupattach.UserNum AND userod.IsHidden=0 "
					+"WHERE grouppermission.PermType='"+POut.Long((int)enumPermType)+"' "
					+"AND grouppermission.UserGroupNum!="+POut.Long(userGroupNum)+") t";//This query is Oracle compatable
				if(Db.GetScalar(command)=="0") {//no other users outside of this group have SecurityAdmin
					throw new Exception(Lans.g("FormSecurity","There must always be at least one user in a user group that has the Security Admin permission."));
				}
			}
			command="DELETE FROM grouppermission WHERE UserGroupNum="+POut.Long(userGroupNum)+" "
				+"AND PermType="+POut.Long((int)enumPermType);
 			Db.NonQ(command);
		}

		public static bool Sync(List<GroupPermission> listGroupPermissionsNew,List<GroupPermission> listGroupPermissionsOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listGroupPermissionsNew,listGroupPermissionsOld);
			}
			return Crud.GroupPermissionCrud.Sync(listGroupPermissionsNew,listGroupPermissionsOld);
		}

		///<summary>Gets a GroupPermission based on the supplied userGroupNum and permType.  If not found, then it returns null.  Used in FormSecurity when double clicking on a dated permission or when clicking the all button.</summary>
		public static GroupPermission GetPerm(long userGroupNum,EnumPermType enumPermType) {
			Meth.NoCheckMiddleTierRole();
			return GetFirstOrDefault(x => x.UserGroupNum==userGroupNum && x.PermType==enumPermType);
		}

		///<summary>Gets a list of GroupPermissions for the supplied UserGroupNum.</summary>
		public static List<GroupPermission> GetPerms(long userGroupNum) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => x.UserGroupNum==userGroupNum);
		}

		///<summary>Gets a list of GroupPermissions for the supplied UserGroupNum without using the local cache.  Useful for multithreaded connections.</summary>
		public static List<GroupPermission> GetPermsNoCache(long userGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<GroupPermission>>(MethodBase.GetCurrentMethod(),userGroupNum);
			}
			List<GroupPermission> listGroupPermissions=new List<GroupPermission>();
			string command="SELECT * FROM grouppermission WHERE UserGroupNum="+POut.Long(userGroupNum);
			DataTable tableGroupPerms=Db.GetTable(command);
			listGroupPermissions=Crud.GroupPermissionCrud.TableToList(tableGroupPerms);
			return listGroupPermissions;
		}

		///<summary>Gets a list of GroupPermissions that are associated with reports. Uses Reports (22) permission.</summary>
		public static List<GroupPermission> GetPermsForReports(long userGroupNum=0) {
			Meth.NoCheckMiddleTierRole();
			List<GroupPermission> listGroupPermissions=GetWhere(x => x.PermType==EnumPermType.Reports);
			if(userGroupNum > 0) {
				listGroupPermissions.RemoveAll(x => x.UserGroupNum!=userGroupNum);
			}
			return listGroupPermissions;
		}

		///<summary>Gets a list of AdjustmentTypeDeny perms for a user group. Having an AdjustmentTypeDeny perm indicates the user group does not have 
		///permission to access (create,edit,edit zero) the adjustmenttype that has a defnum==fkey. Pattern approved by Jordan.</summary>
		public static List<GroupPermission> GetAdjustmentTypeDenyPermsForUserGroup(long userGroupNum) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => x.PermType==EnumPermType.AdjustmentTypeDeny && x.UserGroupNum==userGroupNum);
		}

		///<summary>Gets a list of GroupPermissions that are associated with reports and the user groups that the passed in user. Uses Reports (22) permission.</summary>
		public static List<GroupPermission> GetPermsForReports(Userod user) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => x.PermType==EnumPermType.Reports && user.IsInUserGroup(x.UserGroupNum));
		}

		///<summary>Used to check if user has permission to access the report. Pass in a list of DisplayReports to avoid a call to the db.</summary>
		public static bool HasReportPermission(string reportName,Userod user,List<DisplayReport> listDisplayReports=null) {
			Meth.NoCheckMiddleTierRole();
			DisplayReport displayReport;
			if(listDisplayReports==null){
				displayReport=DisplayReports.GetAll(showHidden: false).Find(x => x.InternalName==reportName);
			}
			else{
				displayReport=listDisplayReports.Find(x => x.InternalName==reportName);
			}	
			if(displayReport==null) {//Report is probably hidden.
				return false;
			}
			List<GroupPermission> listReportPermissions=GroupPermissions.GetPermsForReports(user);
			return listReportPermissions.Any(x => x.FKey.In(0,displayReport.DisplayReportNum));//Zero FKey means access to every report.
		}

		///<summary>Determines whether a single userGroup contains a specific permission.</summary>
		public static bool HasPermission(long userGroupNum,EnumPermType enumPermType,long fKey,List<GroupPermission> listGroupPermissions=null) {
			Meth.NoCheckMiddleTierRole();
			List<GroupPermission> listGroupPermissionsCopy;
			if(listGroupPermissions==null) {
				listGroupPermissionsCopy=GetWhere(x => x.UserGroupNum==userGroupNum && x.PermType==enumPermType);
			}
			else {
				listGroupPermissionsCopy=new List<GroupPermission>(listGroupPermissions);
				listGroupPermissionsCopy.RemoveAll(x => x.UserGroupNum!=userGroupNum || x.PermType!=enumPermType);
			}
			if(DoesPermissionTreatZeroFKeyAsAll(enumPermType) && listGroupPermissionsCopy.Any(x => x.FKey==0)) {//Access to everything.
				return true;
			}
			return listGroupPermissionsCopy.Any(x => x.FKey==fKey);
		}

		///<summary>Determines whether an individual user has a specific permission.</summary>
		public static bool HasPermission(Userod user,EnumPermType enumPermType,long fKey,List<GroupPermission> listGroupPermissions=null) {
			Meth.NoCheckMiddleTierRole();
			if(listGroupPermissions==null) {
				listGroupPermissions=GetWhere(x => x.PermType==enumPermType && user.IsInUserGroup(x.UserGroupNum));
			}
			else {
				listGroupPermissions.RemoveAll(x => x.PermType!=enumPermType && !user.IsInUserGroup(x.UserGroupNum));
			}
			if(DoesPermissionTreatZeroFKeyAsAll(enumPermType) && listGroupPermissions.Any(x => x.FKey==0)) {//Access to everything.
				return true;
			}
			return listGroupPermissions.Any(x => x.FKey==fKey);
		}

		///<summary>Checks if user has permission to access the passed-in adjustment type. 
		///Unlike other permissions, if this permission node isn't checked then a user is not barred from creating this specific adjustment type</summary>
		public static bool HasPermissionForAdjType(Def defAdjType,bool suppressMessage = true) {
			Meth.NoCheckMiddleTierRole();
			List<UserGroup> listUserGroupsAdjTypeDeny=UserGroups.GetForPermission(EnumPermType.AdjustmentTypeDeny);
			List<UserGroup> listUserGroupsForUser=UserGroups.GetForUser(Security.CurUser.UserNum, (Security.CurUser.UserNumCEMT!=0));
			List<UserGroup> listUserGroupsForUserWithAdjTypeDeny=listUserGroupsForUser.FindAll(x => listUserGroupsAdjTypeDeny.Any(y => y.UserGroupNum==x.UserGroupNum));
			List<long> listUserGroupNums=listUserGroupsForUserWithAdjTypeDeny.Select(x => x.UserGroupNum).ToList();
			List<GroupPermission> listGroupPermissions=GetForUserGroups(listUserGroupNums, EnumPermType.AdjustmentTypeDeny)
				.FindAll(x => x.FKey==defAdjType.DefNum || x.FKey==0);// Fkey of 0 means all adjTypeDefs were selected
			//Return true when not all the user's groups with AdjustmentTypeDeny have the adjTypeDef.DefNum checked or have the Fkey value of 0 so the adjustment is not blocked.
			if(listGroupPermissions.IsNullOrEmpty() || listGroupPermissions.Count!=listUserGroupsForUser.Count ) {
        return true;
			}
			if(suppressMessage) {
				return false;
			}
			string unauthorizedMessage=Lans.g("Security","Not authorized.")+"\r\n"
				+Lans.g("Security","A user with the SecurityAdmin permission must grant you access for adjustment type")+":\r\n"+defAdjType.ItemName;
      MessageBox.Show(unauthorizedMessage);
			return false;
		}

		///<summary>Checks if user has permission to access the passed-in adjustment type then checks if the user has the passed-in permission as well.</summary>
		public static bool HasPermissionForAdjType(EnumPermType enumPermType,Def defAdjType,bool supressMessage=true) {
			Meth.NoCheckMiddleTierRole();
			return HasPermissionForAdjType(enumPermType,defAdjType,DateTime.MinValue,supressMessage);
		}

		///<summary>Checks if user has permission to access the passed-in adjustment type then checks if the user has the passed-in permission as well. Use this method if the permission
		///also takes in a date.</summary>
		public static bool HasPermissionForAdjType(EnumPermType enumPermType,Def defAdjType,DateTime dateTime,bool suppressMessage=true) {
			Meth.NoCheckMiddleTierRole();
			bool canEdit=HasPermissionForAdjType(defAdjType,suppressMessage);
			if(!canEdit) {
				return false;
			}
			return Security.IsAuthorized(enumPermType,dateTime,suppressMessage);
		}

		public static bool DoesPermissionTreatZeroFKeyAsAll(EnumPermType enumPermType) {
			Meth.NoCheckMiddleTierRole();
			return enumPermType.In(EnumPermType.AdjustmentTypeDeny,EnumPermType.DashboardWidget,EnumPermType.Reports);
		}

		///<summary>Returns permissions associated to the passed-in usergroups. 
		///Pass in a specific permType to only return GroupPermissions of that type.
		///Otherwise, will return all GroupPermissions for the UserGroups.</summary>
		public static List<GroupPermission> GetForUserGroups(List<long> listUserGroupNums, EnumPermType enumPermType=EnumPermType.None) {
			Meth.NoCheckMiddleTierRole();
			if(enumPermType==EnumPermType.None) {
				return GetWhere(x => listUserGroupNums.Contains(x.UserGroupNum));
			}
			return GetWhere(x => x.PermType == enumPermType && listUserGroupNums.Contains(x.UserGroupNum));
		}

		///<summary>Gets permissions that actually generate audit trail entries. Returns false for HQ-only preferences if not at HQ.</summary>
		public static bool HasAuditTrail(EnumPermType enumPermType) {
			Meth.NoCheckMiddleTierRole();
			switch(enumPermType) {//If commented, has an audit trail. In the order they appear in Permissions enumeration
				//Normal pattern is to comment out the FALSE cases. 
				//This is the opposite so that the default behavior for new security permissions to be to show in the audit trail. In case it wasn't added to this function.
				case EnumPermType.None:
				case EnumPermType.AppointmentsModule:
				//case Permissions.FamilyModule:
				//case Permissions.AccountModule:
				//case Permissions.TPModule:
				//case Permissions.ChartModule:
				//case Permissions.ImagesModule:
				case EnumPermType.ManageModule:
				//case Permissions.Setup:
				//case Permissions.RxCreate:
				//case Permissions.ChooseDatabase:
				//case Permissions.Schedules:
				//case Permissions.Blockouts:
				//case Permissions.ClaimSentEdit:
				//case Permissions.PaymentCreate:
				//case Permissions.PaymentEdit:
				//case Permissions.AdjustmentCreate:
				//case Permissions.AdjustmentEdit:
				//case Permissions.UserQuery:
				case EnumPermType.StartupSingleUserOld:
				case EnumPermType.StartupMultiUserOld:
				//case Permissions.Reports:
				//case Permissions.ProcComplCreate:
				//case Permissions.SecurityAdmin:
				//case Permissions.AppointmentCreate:
				//case Permissions.AppointmentMove:
				//case Permissions.AppointmentEdit:
				//case Permissions.AppointmentCompleteEdit:
				//case Permissions.Backup:
				case EnumPermType.TimecardsEditAll:
				//case Permissions.DepositSlips:
				//case Permissions.AccountingEdit:
				//case Permissions.AccountingCreate:
				//case Permissions.Accounting:
				case EnumPermType.AnesthesiaIntakeMeds:
				case EnumPermType.AnesthesiaControlMeds:
				//case Permissions.InsPayCreate:
				//case Permissions.InsPayEdit:
				//case Permissions.TreatPlanEdit:
				//case Permissions.ReportProdInc:
				//case Permissions.TimecardDeleteEntry:
				case EnumPermType.EquipmentDelete:
				//case Permissions.SheetEdit:
				//case Permissions.CommlogEdit:
				//case Permissions.ImageDelete:
				//case Permissions.PerioEdit:
				case EnumPermType.ProcEditShowFee:
				case EnumPermType.AdjustmentEditZero:
				case EnumPermType.EhrEmergencyAccess:
				//case Permissions.ProcDelete:
				//case EnumPermType.EhrKeyAdd:
				//case Permissions.ProviderEdit:
				case EnumPermType.EcwAppointmentRevise:
				case EnumPermType.ProcedureNoteFull:
				case EnumPermType.ProcedureNoteUser:
				//case Permissions.ReferralAdd:
				//case Permissions.InsPlanChangeSubsc:
				//case Permissions.RefAttachAdd:
				//case Permissions.RefAttachDelete:
				//case Permissions.CarrierCreate:
				//case Permissions.CarrierEdit:
				case EnumPermType.GraphicalReports:
				//case Permissions.AutoNoteQuickNoteEdit:
				case EnumPermType.EquipmentSetup:
				//case Permissions.Billing:
				//case Permissions.ProblemDefEdit:
				//case Permissions.ProcFeeEdit:
				//case Permissions.InsPlanChangeCarrierName:
				//case Permissions.TaskNoteEdit:
				case EnumPermType.WikiListSetup:
				case EnumPermType.Copy:
				//case Permissions.Printing:
				//case Permissions.MedicalInfoViewed:
				//case Permissions.PatProblemListEdit:
				//case Permissions.PatMedicationListEdit:
				//case Permissions.PatAllergyListEdit:
				case EnumPermType.PatFamilyHealthEdit:
				case EnumPermType.PatientPortal:
				//case Permissions.RxEdit:
				case EnumPermType.AdminDentalStudents:
				case EnumPermType.AdminDentalInstructors:
				//case Permissions.OrthoChartEditFull:
				case EnumPermType.OrthoChartEditUser://We only ever use OrthoChartEditFull when audit trailing.
				//case Permissions.PatientFieldEdit:
				case EnumPermType.AdminDentalEvaluations:
				//case Permissions.TreatPlanDiscountEdit:
				//case Permissions.UserLogOnOff:
				//case Permissions.TaskEdit:
				//case Permissions.EmailSend:
				//case Permissions.WebmailSend:
				case EnumPermType.UserQueryAdmin:
				//case Permissions.InsPlanChangeAssign:
				//case Permissions.ImageEdit:
				//case Permissions.EhrMeasureEventEdit:
				//case Permissions.EServicesSetup:
				//case Permissions.FeeSchedEdit:
				//case Permissions.PatientBillingEdit:
				case EnumPermType.ProviderFeeEdit:
				case EnumPermType.ClaimHistoryEdit:
				//case Permissions.FeatureRequestEdit:
				//case Permissions.QueryRequestEdit:
				//case Permissions.JobApproval:
				//case Permissions.JobDocumentation:
				//case Permissions.JobEdit:
				//case Permissions.JobManager:
				//case Permissions.JobReview:
				//case Permissions.WebmailDelete:
				//case Permissions.MissingRequiredField:
				//case Permissions.ReferralMerge:
				//case Permissions.ProcEdit:
				//case Permissions.ProviderMerge:
				//case Permissions.MedicationMerge:
				//case Permissions.AccountQuickCharge:
				//case Permissions.ClaimSend:
				//case Permissions.TaskListCreate:
				//case Permissions.PatientCreate:
				//case Permissions.GraphicalReportSetup:
				case EnumPermType.PreAuthSentEdit:
				//case Permissions.PatientEdit:
				//case Permissions.InsPlanCreate:
				//case Permissions.InsPlanEdit:
				//case Permissions.InsPlanCreateSub:
				//case Permissions.InsPlanEditSub:
				//case Permissions.InsPlanAddPat:
				//case Permissions.InsPlanDropPat:
				case EnumPermType.InsPlanVerifyList:
				//case Permissions.SheetEdit:
				//case Permissions.SplitCreatePastLockDate:
				//case Permissions.ClaimDelete:
				//case Permissions.InsWriteOffEdit:
				case EnumPermType.ProviderAlphabetize:
				//case Permissions.ApptConfirmStatusEdit:
				//case Permissions.GraphicsRemoteEdit:
				//case Permissions.AuditTrail:
				//case Permissions.TreatPlanPresenterEdit:
				case EnumPermType.ClaimProcReceivedEdit:
				//case Permissions.MobileWeb:
				//case Permissions.StatementPatNumMismatch:
				//case Permissions.PatPriProvEdit:
				//case Permissions.ReferralEdit:
				//case Permissions.ReplicationSetup:
				case EnumPermType.ReportProdIncAllProviders:
				//case Permissions.ReportDaily:
				case EnumPermType.ReportDailyAllProviders:
				case EnumPermType.SheetDelete:
				case EnumPermType.UpdateCustomTracking:
				//case Permissions.GraphicsEdit:
				case EnumPermType.InsPlanOrthoEdit:
				//case Permissions.ClaimProcClaimAttachedProvEdit:
				//case Permissions.InsPlanMerge:
				//case Permissions.InsuranceCarrierCombine:
				case EnumPermType.PopupEdit://Popups are archived, so they don't need to show in the audit trail.
				case EnumPermType.InsPlanPickListExisting:
				case EnumPermType.GroupNoteEditSigned:
				case EnumPermType.WikiAdmin:
				//case Permissions.PayPlanEdit:
				//case Permissions.ClaimEdit:
				//case Permissions.LogFeeEdit:
				//case Permissions.LogSubscriberEdit:
				//case Permissions.RecallEdit:
				//case Permissions.ProcCodeEdit:
				//case Permissions.AddNewUser:
				case EnumPermType.ClaimView:
				//case Permissions.RepeatChargeTool:
				//case Permissions.DiscountPlanAddDrop:
				case EnumPermType.TreatPlanSign:
				case EnumPermType.UnrestrictedSearch:
				case EnumPermType.ArchivedPatientEdit:
				//case EnumPermType.CommlogPersistent:
				//case Permissions.VerifyPhoneOwnership
				//case Permissions.SalesTaxAdjEdit://All other adjustment operations are already audited.
				//case Permissions.AgingRan:
				case EnumPermType.InsuranceVerification:
				//case Permissions.CreditCardMove:
				//case Permissions.HeadmasterSetup
				case EnumPermType.NewClaimsProcNotBilled:
				//case Permissions.PatientPortalLogin:
				//case Permissions.FAQEdit:
				//case EnumPermType.FeatureRequestEdit:
				//case Permissions.SupplementalBackup:
				//case Permissions.WebSchedRecallManualSend:
				//case Permissions.PatientSSNView:
				//case Permissions.PatientDOBView:
				//case Permissions.FamAgingTruncate:
				//case Permissions.DiscountPlanMerge:
				//case Permissions.ProcCompleteEditMisc:
				//case Permissions.ProcCompleteAddAdj:
				//case Permissions.ProcCompletetStatusEdit:
				//case Permissions.ProcCompleteNote:
				//case Permissions.ProcCompleteEdit:
				//case Permissions.ProtectedLeaveAdjustmentEdit:
				//case Permissions.TimeAdjustEdit:
				//case Permissions.QueryMonitor:
				//case Permissions.CommlogCreate:
				//case Permissions.RepeatChargeCreate:
				//case Permissions.RepeatChargeUpdate:
				//case Permissions.RepeatChargeDelete:
				case EnumPermType.WebFormAccess:
				//case Permissions.CloseOtherSessions:
				case EnumPermType.Zoom:
				//case Permissions.ImageExport:
				//case Permissions.ImageCreate:
				case EnumPermType.CertificationEmployee:
				case EnumPermType.CertificationSetup:
				//case Permissions.AllowLoginFromAnywhere:
				//case Permissions.PayPlanChargeDateEdit;
				//case Permissions.DiscountPlanAdd:
				//case Permissions.DiscountPlanEdit:
				//case Permissions.AllowFeeEditWhileReceivingClaim:
				//case Permissions.ManageHighSecurityProgProperties:
				//case Permissions.CreditCardEdit:
				//case Permissions.Advertising:
				//case Permissions.RxMerge:
				case EnumPermType.MedicationDefEdit:
				case EnumPermType.AllergyDefEdit:
				//case Permissions.TextMessageView:
				case EnumPermType.TextMessageSend:
				//case Permissions.DefEdit:
				//case Permissions.UpdateInstall;
				case EnumPermType.AdjustmentTypeDeny:
				//case Permissions.StatementCSV:
				//case Permissions.SecurityGlobal:
				//case Permissions.TaskDelete:
				case EnumPermType.SetupWizard:
				//case Permissions.ShowFeatures:
				//case Permissions.PrinterSetup:
				//case Permissions.ProviderAdd:
				//case Permissions.ClinicEdit:
				case EnumPermType.ApiAccountEdit:
				//case Permissions.RegistrationKeyCreate:
				//case Permissions.RegistrationKeyEdit:
				//case Permissions.AppointmentDelete:
				//case Permissions.AppointmentCompleteDelete:
				//case Permissions.AppointmentTypeEdit:
				//case Permissions.TextingAccountEdit:
				//case Permissions.WebChatEdit:
				case EnumPermType.SupplierEdit:
				//case Permissions.SupplyPurchases:
				//case Permissions.PreferenceEditBroadcastMonitor:
				case EnumPermType.AppointmentResize:
				//case Permissions.CreditCardTerminal:
				case EnumPermType.ViewAppointmentAuditTrail:
				//case Permissions.PayPlanChargeEdit:
				case EnumPermType.ArchivedPatientSelect:
				//case EnumPermType.CloudCustomerEdit:
				//case Permissions.ChanSpy
				case EnumPermType.ClaimProcFeeBilledToInsEdit:
				//case Permissions.AllergyMerge
				//case Permissions.AiChatSession:
				//case Permissions.BadgeIdEdit
				case EnumPermType.ChildDaycareEdit:
				case EnumPermType.PerioEditCopy:
				return false;//Does not have audit Trail if uncommented.
			}
			if(!PrefC.IsODHQ && enumPermType.In(
					//These permissions are only used at OD HQ
					EnumPermType.EhrKeyAdd,
					EnumPermType.CommlogPersistent,
					EnumPermType.VerifyPhoneOwnership,
					EnumPermType.SalesTaxAdjEdit,
					EnumPermType.HeadmasterSetup,
					EnumPermType.FAQEdit,
					EnumPermType.FeatureRequestEdit,
					EnumPermType.EditReadOnlyTasks,
					EnumPermType.TextingAccountEdit,
					EnumPermType.PreferenceEditBroadcastMonitor,
					EnumPermType.CloudCustomerEdit,
					EnumPermType.ChanSpy,
					EnumPermType.AiChatSession
				)) 
			{
				return false;
			}
			return true;
		}		

		///<summary>Removes all FKey specific permissions and gives the user group a single 'zero FKey' permission for the type passed in.</summary>
		public static void GiveUserGroupPermissionAll(long userGroupNum,EnumPermType enumPermType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroupNum,enumPermType);
				return;
			}
			//Remove all permissions for the user group and perm type.
			string command=$"DELETE FROM grouppermission WHERE UserGroupNum={POut.Long(userGroupNum)} AND PermType={POut.Enum(enumPermType)}";
			Db.NonQ(command);
			//AdjustmentTypeDeny is a permission that denies access to a usergroup when they have this permission. When a user clicks 'Set All', they want the user group to have every permission.
			//This means they want the user group to have access to every adjustment type. So we need to delete all adjustment type deny permissions for this user group, which we do above. 
			//But we do NOT want to create a 0 FKey perm because that will indicate the user group does not have access to any adjusment type, so we return early.
			if(enumPermType==EnumPermType.AdjustmentTypeDeny) {
				return;
			}
			//Insert a new permission with a zero FKey.
			GroupPermission groupPermission=new GroupPermission();
			groupPermission.NewerDate=DateTime.MinValue;
			groupPermission.NewerDays=0;
			groupPermission.PermType=enumPermType;
			groupPermission.UserGroupNum=userGroupNum;
			groupPermission.FKey=0;
			Crud.GroupPermissionCrud.Insert(groupPermission);
		}

		///<summary>Gets the description for the specified permisssion.  Already translated.</summary>
		public static string GetDesc(EnumPermType enumPermType){
			Meth.NoCheckMiddleTierRole();
			return Lans.g("enumPermissions",enumPermType.GetDescription());//If Description attribute is not defined, will default to perm.ToString()
		}

		///<summary></summary>
		public static bool PermTakesDates(EnumPermType enumPermType){
			Meth.NoCheckMiddleTierRole();
			if(enumPermType==EnumPermType.AccountingCreate//prevents backdating
				|| enumPermType==EnumPermType.AccountingEdit
				|| enumPermType==EnumPermType.AdjustmentCreate
				|| enumPermType==EnumPermType.AdjustmentEdit
				|| enumPermType==EnumPermType.ClaimDelete
				|| enumPermType==EnumPermType.ClaimHistoryEdit
				|| enumPermType==EnumPermType.ClaimProcReceivedEdit
				|| enumPermType==EnumPermType.ClaimSentEdit
				|| enumPermType==EnumPermType.CommlogEdit
				|| enumPermType==EnumPermType.DepositSlips//prevents backdating
				|| enumPermType==EnumPermType.EquipmentDelete
				|| enumPermType==EnumPermType.ImageDelete
				|| enumPermType==EnumPermType.InsPayEdit
				|| enumPermType==EnumPermType.InsWriteOffEdit
				|| enumPermType==EnumPermType.NewClaimsProcNotBilled
				|| enumPermType==EnumPermType.OrthoChartEditFull
				|| enumPermType==EnumPermType.OrthoChartEditUser
				|| enumPermType==EnumPermType.PaymentEdit
				|| enumPermType==EnumPermType.PerioEdit
				|| enumPermType==EnumPermType.PreAuthSentEdit
				|| enumPermType==EnumPermType.ProcComplCreate
				|| enumPermType==EnumPermType.ProcCompleteEdit
				|| enumPermType==EnumPermType.ProcCompleteNote
				|| enumPermType==EnumPermType.ProcCompleteEditMisc
				|| enumPermType==EnumPermType.ProcCompleteStatusEdit
				|| enumPermType==EnumPermType.ProcCompleteAddAdj
				|| enumPermType==EnumPermType.ProcExistingEdit
				|| enumPermType==EnumPermType.ProcDelete
				|| enumPermType==EnumPermType.SheetEdit
				|| enumPermType==EnumPermType.TimecardDeleteEntry
				|| enumPermType==EnumPermType.TreatPlanEdit
				|| enumPermType==EnumPermType.TreatPlanSign
				|| enumPermType==EnumPermType.PaymentCreate//to prevent backdating of newly created payments
				|| enumPermType==EnumPermType.ImageEdit
				|| enumPermType==EnumPermType.ImageExport
				)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns a list of permissions that are included in the bitwise enum crudSLFKeyPerms passed in.
		///Used in DBM and the crud generator.  Needs to be updated every time a new CrudAuditPerm is added.</summary>
		public static List<EnumPermType> GetPermsFromCrudAuditPerm(CrudAuditPerm crudAuditPerm) {
			List<EnumPermType> listPerms=new List<EnumPermType>();
			//No check for none.
			if(crudAuditPerm.HasFlag(CrudAuditPerm.AppointmentCompleteEdit)) { //b01
				listPerms.Add(EnumPermType.AppointmentCompleteEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.AppointmentCreate)) { //b010
				listPerms.Add(EnumPermType.AppointmentCreate);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.AppointmentEdit)) { //b0100
				listPerms.Add(EnumPermType.AppointmentEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.AppointmentMove)) { //b01000
				listPerms.Add(EnumPermType.AppointmentMove);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.ClaimHistoryEdit)) { //b010000
				listPerms.Add(EnumPermType.ClaimHistoryEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.ImageDelete)) { //b0100000
				listPerms.Add(EnumPermType.ImageDelete);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.ImageEdit)) { //b01000000
				listPerms.Add(EnumPermType.ImageEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.InsPlanChangeCarrierName)) { //b010000000
				listPerms.Add(EnumPermType.InsPlanChangeCarrierName);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.RxCreate)) { //b0100000000
				listPerms.Add(EnumPermType.RxCreate);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.RxEdit)) { //b01000000000
				listPerms.Add(EnumPermType.RxEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.TaskNoteEdit)) { //b010000000000
				listPerms.Add(EnumPermType.TaskNoteEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.PatientPortal)) { //b0100000000000
				listPerms.Add(EnumPermType.PatientPortal);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.ProcFeeEdit)) { //b01000000000000
				listPerms.Add(EnumPermType.ProcFeeEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.LogFeeEdit)) { //b010000000000000
				listPerms.Add(EnumPermType.LogFeeEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.LogSubscriberEdit)) { //b0100000000000000
				listPerms.Add(EnumPermType.LogSubscriberEdit);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.AppointmentDelete)) { //b01000000000000000
				listPerms.Add(EnumPermType.AppointmentDelete);
			}
			if(crudAuditPerm.HasFlag(CrudAuditPerm.AppointmentCompleteDelete)) { //b010000000000000000
				listPerms.Add(EnumPermType.AppointmentCompleteDelete);
			}
			return listPerms;
		}
	}
 



}













