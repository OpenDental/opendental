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
		public static DateTime GetDateRestrictedForPermission(Permissions permission,List<long> listUserGroupNums) {
			//No need to check RemotingRole; no call to db.
			DateTime nowDate=DateTime.MinValue;
			Func<DateTime> getNowDate=new Func<DateTime>(() => {
				if(nowDate.Year < 1880) {
					nowDate=MiscData.GetNowDateTime().Date;
				}
				return nowDate;
			});
			DateTime retVal=DateTime.MinValue;
			List<GroupPermission> listGroupPerms=GetForUserGroups(listUserGroupNums,permission);
			//get the permission that applies
			GroupPermission perm=listGroupPerms.OrderBy((GroupPermission y) => {
				if(y.NewerDays==0 && y.NewerDate==DateTime.MinValue) {
					return DateTime.MinValue;
				}
				if(y.NewerDays==0) {
					return y.NewerDate;
				}
				return getNowDate().AddDays(-y.NewerDays);
			}).FirstOrDefault();
			if(perm==null) {
				//do not change retVal. The user does not have the permission.
			}
			else if(perm.NewerDate.Year < 1880 && perm.NewerDays == 0) {
				//do not change retVal. The user is not restricted by date.
			}
			else if(perm.NewerDate.Year > 1880) {
				retVal=perm.NewerDate;
			}
			else if(getNowDate().AddDays(-perm.NewerDays)>retVal) {
				retVal=getNowDate().AddDays(-perm.NewerDays);
			}
			return retVal;
		}

		///<summary>Used for procedures with status EO, EC, or C. Returns Permissions.ProcExistingEdit for EO/EC</summary>
		public static Permissions SwitchExistingPermissionIfNeeded(Permissions perm,Procedure proc) {
			if(ListTools.In(proc.ProcStatus,ProcStat.EO,ProcStat.EC)) {
				return Permissions.ProcExistingEdit;
			}
			return perm;
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_GroupPermissionCache.FillCacheFromTable(table);
				return table;
			}
			return _GroupPermissionCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(GroupPermission gp){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			if(gp.NewerDate.Year>1880 && gp.NewerDays>0) {
				throw new Exception(Lans.g("GroupPermissions","Date or days can be set, but not both."));
			}
			if(!GroupPermissions.PermTakesDates(gp.PermType)) {
				if(gp.NewerDate.Year>1880 || gp.NewerDays>0) {
					throw new Exception(Lans.g("GroupPermissions","This type of permission may not have a date or days set."));
				}
			}
			Crud.GroupPermissionCrud.Update(gp);
		}

		///<summary>Update that doesnt use the local cache.  Useful for multithreaded connections.</summary>
		public static void UpdateNoCache(GroupPermission gp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			string command="UPDATE grouppermission SET "
				+"NewerDate   =  "+POut.Date  (gp.NewerDate)+", "
				+"NewerDays   =  "+POut.Int   (gp.NewerDays)+", "
				+"UserGroupNum=  "+POut.Long  (gp.UserGroupNum)+", "
				+"PermType    =  "+POut.Int   ((int)gp.PermType)+" "
				+"WHERE GroupPermNum = "+POut.Long(gp.GroupPermNum);
			Db.NonQ(command);
		}

		///<summary>Deletes GroupPermissions based on primary key.  Do not call this method unless you have checked specific dependencies first.  E.g. after deleting this permission, there will still be a security admin user.  This method is only called from the CEMT sync.  RemovePermission should probably be used instead.</summary>
		public static void Delete(GroupPermission gp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			string command="DELETE FROM grouppermission WHERE GroupPermNum = "+POut.Long(gp.GroupPermNum);
			Db.NonQ(command);
		}

		///<summary>Deletes without using the cache.  Useful for multithreaded connections.</summary>
		public static void DeleteNoCache(GroupPermission gp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			string command="DELETE FROM grouppermission WHERE GroupPermNum="+POut.Long(gp.GroupPermNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(GroupPermission gp){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				gp.GroupPermNum=Meth.GetLong(MethodBase.GetCurrentMethod(),gp);
				return gp.GroupPermNum;
			}
			if(gp.NewerDate.Year>1880 && gp.NewerDays>0) {
				throw new Exception(Lans.g("GroupPermissions","Date or days can be set, but not both."));
			}
			if(!GroupPermissions.PermTakesDates(gp.PermType)) {
				if(gp.NewerDate.Year>1880 || gp.NewerDays>0) {
					throw new Exception(Lans.g("GroupPermissions","This type of permission may not have a date or days set."));
				}
			}
			if(gp.PermType==Permissions.SecurityAdmin) {
				//Make sure there are no hidden users in the group that is about to get the Security Admin permission.
				string command="SELECT COUNT(*) FROM userod "
					+"INNER JOIN usergroupattach ON usergroupattach.UserNum=userod.UserNum "
					+"WHERE userod.IsHidden=1 "
					+"AND usergroupattach.UserGroupNum="+gp.UserGroupNum;
				int count=PIn.Int(Db.GetCount(command));
				if(count!=0) {//there are hidden users in this group
					throw new Exception(Lans.g("FormSecurity","The Security Admin permission cannot be given to a user group with hidden users."));
				}
			}
			return Crud.GroupPermissionCrud.Insert(gp);
		}

		///<summary>Insertion logic that doesn't use the cache. Has special cases for generating random PK's and handling Oracle insertions.</summary>
		public static long InsertNoCache(GroupPermission gp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),gp);
			}
			return Crud.GroupPermissionCrud.InsertNoCache(gp);
		}

		///<summary></summary>
		public static void RemovePermission(long groupNum,Permissions permType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupNum,permType);
				return;
			}
			string command;
			if(permType==Permissions.SecurityAdmin){
				//need to make sure that at least one other user has this permission
				command="SELECT COUNT(*) FROM (SELECT DISTINCT grouppermission.UserGroupNum "
					+"FROM grouppermission "
					+"INNER JOIN usergroupattach ON usergroupattach.UserGroupNum=grouppermission.UserGroupNum "
					+"INNER JOIN userod ON userod.UserNum=usergroupattach.UserNum AND userod.IsHidden=0 "
					+"WHERE grouppermission.PermType='"+POut.Long((int)permType)+"' "
					+"AND grouppermission.UserGroupNum!="+POut.Long(groupNum)+") t";//This query is Oracle compatable
				if(Db.GetScalar(command)=="0") {//no other users outside of this group have SecurityAdmin
					throw new Exception(Lans.g("FormSecurity","There must always be at least one user in a user group that has the Security Admin permission."));
				}
			}
			if(permType==Permissions.Reports) {
				//Special case.  For Reports permission type we want to delete the "base" Reports permission but not any Reports permissions with FKey
				//When they re-enable the Reports permission we want to remember all individual reports permissions for that UserGroup
				command="DELETE from grouppermission WHERE UserGroupNum='"+POut.Long(groupNum)+"' "
					+"AND PermType='"+POut.Long((int)permType)+"' AND FKey=0";
			}
			else {
				command="DELETE from grouppermission WHERE UserGroupNum='"+POut.Long(groupNum)+"' "
					+"AND PermType='"+POut.Long((int)permType)+"'";
			}
 			Db.NonQ(command);
		}

		public static bool Sync(List<GroupPermission> listNew,List<GroupPermission> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.GroupPermissionCrud.Sync(listNew,listOld);
		}

		///<summary>Gets a GroupPermission based on the supplied userGroupNum and permType.  If not found, then it returns null.  Used in FormSecurity when double clicking on a dated permission or when clicking the all button.</summary>
		public static GroupPermission GetPerm(long userGroupNum,Permissions permType) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.UserGroupNum==userGroupNum && x.PermType==permType);
		}

		///<summary>Gets a list of GroupPermissions for the supplied UserGroupNum.</summary>
		public static List<GroupPermission> GetPerms(long userGroupNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.UserGroupNum==userGroupNum);
		}

		///<summary>Gets a list of GroupPermissions for the supplied UserGroupNum without using the local cache.  Useful for multithreaded connections.</summary>
		public static List<GroupPermission> GetPermsNoCache(long userGroupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<GroupPermission>>(MethodBase.GetCurrentMethod(),userGroupNum);
			}
			List<GroupPermission> retVal=new List<GroupPermission>();
			string command="SELECT * FROM grouppermission WHERE UserGroupNum="+POut.Long(userGroupNum);
			DataTable tableGroupPerms=Db.GetTable(command);
			retVal=Crud.GroupPermissionCrud.TableToList(tableGroupPerms);
			return retVal;
		}

		///<summary>Gets a list of GroupPermissions that are associated with reports.  Uses Reports (22) permission.</summary>
		public static List<GroupPermission> GetPermsForReports() {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.PermType==Permissions.Reports && x.FKey!=0);
		}

		///<summary>Used to check if user has permission to access the report. Pass in a list of DisplayReports to avoid a call to the db.</summary>
		public static bool HasReportPermission(string reportName,Userod user,List<DisplayReport> listReports=null) {
			//No need to check RemotingRole; no call to db.
			if(!Security.IsAuthorized(Permissions.Reports,true)) {
				return false;
			}
			DisplayReport report=(listReports??DisplayReports.GetAll(false)).FirstOrDefault(x=>x.InternalName==reportName);
			if(report==null) {//Report is probably hidden.
				return false;
			}
			List<GroupPermission> listReportPermissions=GroupPermissions.GetPermsForReports();
			if(listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum && Userods.IsInUserGroup(user.UserNum,x.UserGroupNum))) {
				return true;
			}
			return false;
		}

		///<summary>Determines whether a single userGroup contains a specific permission.</summary>
		public static bool HasPermission(long userGroupNum,Permissions permType,long fKey){
			//No need to check RemotingRole; no call to db.
			GroupPermission groupPermission=GetFirstOrDefault(x => x.UserGroupNum==userGroupNum && x.PermType==permType && x.FKey==fKey);
			return (groupPermission!=null);
		}

		///<summary>Determines whether an individual user has a specific permission.</summary>
		public static bool HasPermission(Userod user,Permissions permType,long fKey) {
			//No need to check RemotingRole; no call to db.
			GroupPermission groupPermission=GetFirstOrDefault(x => x.PermType==permType && x.FKey==fKey && user.IsInUserGroup(x.UserGroupNum));
			return (groupPermission!=null);
		}

		///<summary>Returns permissions associated to the passed-in usergroups. 
		///Pass in a specific permType to only return GroupPermissions of that type.
		///Otherwise, will return all GroupPermissions for the UserGroups.</summary>
		public static List<GroupPermission> GetForUserGroups(List<long> listUserGroupNums, Permissions permType=Permissions.None) {
			//No need to check RemotingRole; no call to db.
			if(permType==Permissions.None) {
				return GetWhere(x => listUserGroupNums.Contains(x.UserGroupNum));
			}
			return GetWhere(x => x.PermType == permType && listUserGroupNums.Contains(x.UserGroupNum));
		}

		///<summary>Gets permissions that actually generate audit trail entries.</summary>
		public static bool HasAuditTrail(Permissions permType) {
			//No need to check RemotingRole; no call to db.
			switch(permType) {//If commented, has an audit trail. In the order they appear in Permissions enumeration
				//Normal pattern is to comment out the FALSE cases. 
				//This is the opposite so that the default behavior for new security permissions to be to show in the audit trail. In case it wasn't added to this function.
				case Permissions.None:
				case Permissions.AppointmentsModule:
				//case Permissions.FamilyModule:
				//case Permissions.AccountModule:
				//case Permissions.TPModule:
				//case Permissions.ChartModule:
				//case Permissions.ImagesModule:
				case Permissions.ManageModule:
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
				case Permissions.StartupSingleUserOld:
				case Permissions.StartupMultiUserOld:
				//case Permissions.Reports:
				//case Permissions.ProcComplCreate:
				//case Permissions.SecurityAdmin:
				//case Permissions.AppointmentCreate:
				//case Permissions.AppointmentMove:
				//case Permissions.AppointmentEdit:
				//case Permissions.AppointmentCompleteEdit:
				//case Permissions.Backup:
				case Permissions.TimecardsEditAll:
				//case Permissions.DepositSlips:
				//case Permissions.AccountingEdit:
				//case Permissions.AccountingCreate:
				//case Permissions.Accounting:
				case Permissions.AnesthesiaIntakeMeds:
				case Permissions.AnesthesiaControlMeds:
				//case Permissions.InsPayCreate:
				//case Permissions.InsPayEdit:
				//case Permissions.TreatPlanEdit:
				//case Permissions.ReportProdInc:
				//case Permissions.TimecardDeleteEntry:
				case Permissions.EquipmentDelete:
				//case Permissions.SheetEdit:
				//case Permissions.CommlogEdit:
				//case Permissions.ImageDelete:
				//case Permissions.PerioEdit:
				case Permissions.ProcEditShowFee:
				case Permissions.AdjustmentEditZero:
				case Permissions.EhrEmergencyAccess:
				//case Permissions.ProcDelete:
				case Permissions.EhrKeyAdd:
				case Permissions.Providers:
				case Permissions.EcwAppointmentRevise:
				case Permissions.ProcedureNoteFull:
				case Permissions.ProcedureNoteUser:
				//case Permissions.ReferralAdd:
				//case Permissions.InsPlanChangeSubsc:
				//case Permissions.RefAttachAdd:
				//case Permissions.RefAttachDelete:
				//case Permissions.CarrierCreate:
				case Permissions.GraphicalReports:
				//case Permissions.AutoNoteQuickNoteEdit:
				case Permissions.EquipmentSetup:
				//case Permissions.Billing:
				//case Permissions.ProblemEdit:
				//case Permissions.ProcFeeEdit:
				//case Permissions.InsPlanChangeCarrierName:
				//case Permissions.TaskNoteEdit:
				case Permissions.WikiListSetup:
				case Permissions.Copy:
				//case Permissions.Printing:
				//case Permissions.MedicalInfoViewed:
				//case Permissions.PatProblemListEdit:
				//case Permissions.PatMedicationListEdit:
				//case Permissions.PatAllergyListEdit:
				case Permissions.PatFamilyHealthEdit:
				case Permissions.PatientPortal:
				//case Permissions.RxEdit:
				case Permissions.AdminDentalStudents:
				case Permissions.AdminDentalInstructors:
				//case Permissions.OrthoChartEditFull:
				case Permissions.OrthoChartEditUser://We only ever use OrthoChartEditFull when audit trailing.
				//case Permissions.PatientFieldEdit:
				case Permissions.AdminDentalEvaluations:
				//case Permissions.TreatPlanDiscountEdit:
				//case Permissions.UserLogOnOff:
				//case Permissions.TaskEdit:
				//case Permissions.EmailSend:
				//case Permissions.WebmailSend:
				case Permissions.UserQueryAdmin:
				//case Permissions.InsPlanChangeAssign:
				//case Permissions.ImageEdit:
				//case Permissions.EhrMeasureEventEdit:
				//case Permissions.EServicesSetup:
				//case Permissions.FeeSchedEdit:
				//case Permissions.PatientBillingEdit:
				case Permissions.ProviderFeeEdit:
				case Permissions.ClaimHistoryEdit:
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
				case Permissions.PreAuthSentEdit:
				//case Permissions.PatientEdit:
				//case Permissions.InsPlanCreate:
				//case Permissions.InsPlanEdit:
				//case Permissions.InsPlanCreateSub:
				//case Permissions.InsPlanEditSub:
				//case Permissions.InsPlanAddPat:
				//case Permissions.InsPlanDropPat:
				case Permissions.InsPlanVerifyList:
				//case Permissions.SheetEdit:
				//case Permissions.SplitCreatePastLockDate:
				//case Permissions.ClaimDelete:
				//case Permissions.InsWriteOffEdit:
				case Permissions.ProviderAlphabetize:
				//case Permissions.ApptConfirmStatusEdit:
				//case Permissions.GraphicsRemoteEdit:
				//case Permissions.AuditTrail:
				//case Permissions.TreatPlanPresenterEdit:
				case Permissions.ClaimProcReceivedEdit:
				//case Permissions.MobileWeb:
				//case Permissions.StatementPatNumMismatch:
				//case Permissions.PatPriProvEdit:
				//case Permissions.ReferralEdit:
				//case Permissions.ReplicationSetup:
				case Permissions.ReportProdIncAllProviders:
				//case Permissions.ReportDaily:
				case Permissions.ReportDailyAllProviders:
				case Permissions.SheetDelete:
				case Permissions.UpdateCustomTracking:
				//case Permissions.GraphicsEdit:
				case Permissions.InsPlanOrthoEdit:
				//case Permissions.ClaimProcClaimAttachedProvEdit:
				//case Permissions.InsPlanMerge:
				//case Permissions.InsuranceCarrierCombine:
				case Permissions.PopupEdit://Popups are archived, so they don't need to show in the audit trail.
				case Permissions.InsPlanPickListExisting:
				//case Permissions.GroupNoteEditSigned:
				case Permissions.WikiAdmin:
				//case Permissions.PayPlanEdit:
				//case Permissions.ClaimEdit:
				//case Permissions.LogFeeEdit:
				//case Permissions.LogSubscriberEdit:
				//case Permissions.RecallEdit:
				//case Permissions.ProcCodeEdit:
				//case Permissions.AddNewUser:
				case Permissions.ClaimView:
				//case Permissions.RepeatChargeTool:
				//case Permissions.DiscountPlanAddDrop:
				case Permissions.TreatPlanSign:
				case Permissions.UnrestrictedSearch:
				case Permissions.ArchivedPatientEdit:
				case Permissions.CommlogPersistent:
				//case Permissions.VerifyPhoneOwnership
				//case Permissions.SalesTaxAdjEdit://All other adjustment operations are already audited.
				//case Permissions.AgingRan:
				case Permissions.InsuranceVerification:
				//case Permissions.CreditCardMove:
				//case Permissions.HeadmasterSetup
				case Permissions.NewClaimsProcNotBilled:
				//case Permissions.PatientPortalLogin:
				//case Permissions.FAQEdit:
				case Permissions.FeatureRequestEdit:
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
				case Permissions.WebFormAccess:
				//case Permissions.CloseOtherSessions:
				//case Permissions.ImageExport:
				//case Permissions.ImageCreate:
				case Permissions.CertificationEmployee:
				case Permissions.CertificationSetup:
				//case Permissions.AllowLoginFromAnywhere:
				//case Permissions.PayPlanChargeDateEdit;
				//case Permissions.DiscountPlanAdd:
				//case Permissions.DiscountPlanEdit:
				//case Permissions.AllowFeeEditWhileReceivingClaim:
				return false;//Does not have audit Trail if uncommented.
			}
			if(!PrefC.IsODHQ && ListTools.In(permType,
					//These permissions are only used at OD HQ
					Permissions.VerifyPhoneOwnership,
					Permissions.HeadmasterSetup,
					Permissions.FAQEdit
				)) 
			{
				return false;
			}
			return true;
		}

		///<summary>Gets the description for the specified permisssion.  Already translated.</summary>
		public static string GetDesc(Permissions perm){
			//No need to check RemotingRole; no call to db.
			return Lans.g("enumPermissions",perm.GetDescription());//If Description attribute is not defined, will default to perm.ToString()
		}

		///<summary></summary>
		public static bool PermTakesDates(Permissions permType){
			//No need to check RemotingRole; no call to db.
			if(permType==Permissions.AccountingCreate//prevents backdating
				|| permType==Permissions.AccountingEdit
				|| permType==Permissions.AdjustmentEdit
				|| permType==Permissions.ClaimDelete
				|| permType==Permissions.ClaimHistoryEdit
				|| permType==Permissions.ClaimProcReceivedEdit
				|| permType==Permissions.ClaimSentEdit
				|| permType==Permissions.CommlogEdit
				|| permType==Permissions.DepositSlips//prevents backdating
				|| permType==Permissions.EquipmentDelete
				|| permType==Permissions.ImageDelete
				|| permType==Permissions.InsPayEdit
				|| permType==Permissions.InsWriteOffEdit
				|| permType==Permissions.NewClaimsProcNotBilled
				|| permType==Permissions.OrthoChartEditFull
				|| permType==Permissions.OrthoChartEditUser
				|| permType==Permissions.PaymentEdit
				|| permType==Permissions.PerioEdit
				|| permType==Permissions.PreAuthSentEdit
				|| permType==Permissions.ProcCompleteEdit
				|| permType==Permissions.ProcCompleteNote
				|| permType==Permissions.ProcCompleteEditMisc
				|| permType==Permissions.ProcCompleteStatusEdit
				|| permType==Permissions.ProcCompleteAddAdj
				|| permType==Permissions.ProcExistingEdit
				|| permType==Permissions.ProcDelete
				|| permType==Permissions.SheetEdit
				|| permType==Permissions.TimecardDeleteEntry
				|| permType==Permissions.TreatPlanEdit
				|| permType==Permissions.TreatPlanSign
				|| permType==Permissions.PaymentCreate//to prevent backdating of newly created payments
				|| permType==Permissions.ImageEdit
				|| permType==Permissions.ImageExport
				)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns a list of permissions that are included in the bitwise enum crudSLFKeyPerms passed in.
		///Used in DBM and the crud generator.  Needs to be updated every time a new CrudAuditPerm is added.</summary>
		public static List<Permissions> GetPermsFromCrudAuditPerm(CrudAuditPerm crudSLFKeyPerms) {
			List<Permissions> listPerms=new List<Permissions>();
			//No check for none.
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentCompleteEdit)) { //b01
				listPerms.Add(Permissions.AppointmentCompleteEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentCreate)) { //b010
				listPerms.Add(Permissions.AppointmentCreate);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentEdit)) { //b0100
				listPerms.Add(Permissions.AppointmentEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentMove)) { //b01000
				listPerms.Add(Permissions.AppointmentMove);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ClaimHistoryEdit)) { //b010000
				listPerms.Add(Permissions.ClaimHistoryEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ImageDelete)) { //b0100000
				listPerms.Add(Permissions.ImageDelete);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ImageEdit)) { //b01000000
				listPerms.Add(Permissions.ImageEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.InsPlanChangeCarrierName)) { //b010000000
				listPerms.Add(Permissions.InsPlanChangeCarrierName);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.RxCreate)) { //b0100000000
				listPerms.Add(Permissions.RxCreate);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.RxEdit)) { //b01000000000
				listPerms.Add(Permissions.RxEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.TaskNoteEdit)) { //b010000000000
				listPerms.Add(Permissions.TaskNoteEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.PatientPortal)) { //b0100000000000
				listPerms.Add(Permissions.PatientPortal);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ProcFeeEdit)) { //b01000000000000
				listPerms.Add(Permissions.ProcFeeEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.LogFeeEdit)) { //b010000000000000
				listPerms.Add(Permissions.LogFeeEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.LogSubscriberEdit)) { //b0100000000000000
				listPerms.Add(Permissions.LogSubscriberEdit);
			}
			return listPerms;
		}
	}
 
	

	
}













