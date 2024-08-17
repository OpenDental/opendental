using CodeBase;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Security{
		///<summary>The current user.  Might be null when first starting the program.  Otherwise, must contain valid user.</summary>
		private static Userod curUser;
		///<summary>The current user.  Might be null when first starting the program.  Otherwise, must contain valid user.</summary>
		[ThreadStatic]
		private static Userod _curUserT;
		///<Summary>Contains the value of Environment.MachineName.  If middle tier, it will be the name of the last computer to send 
		///a DataTransferObject to the server.</Summary>
		private static string _curComputerName;
		///<summary>This holds the value of Environment.MachineName.  When using middle tier, this thread static variable will hold the 
		///name of the computer making the request.  Different client connections are managed on different threads.  This is primarily used
		///to make sure securitylogs use the client's computer name instead of the middle tier server name.</summary>
		[ThreadStatic]
		private static string _curComputerNameT;
		///<summary>Remember the password that the user typed in. This is always saved on Log On. 
		///Do not store it in the database.  We will need it when connecting to the web service.
		///Needed for CEMT and reporting servers. If eCW, then this is already encrypted.</summary>
		private static string _passwordTyped;
		///<summary>The password that the user typed in.</summary>
		[ThreadStatic]
		private static string _passwordTypedT;
		///<summary>Tracks whether or not the user is logged in.  Security.CurUser==null usually is used for this purpose, 
		///but in Middle Tier we do not null out CurUser so that queries can continue to be run on the web service.</summary>
		public static bool IsUserLoggedIn;
		///<summary>The last local datetime that there was any mouse or keyboard activity.  Used for auto logoff comparison and for disabling signal 
		///processing due to inactivity.  Must be public so that it can be accessed from multiple application level classes.</summary>
		public static DateTime DateTimeLastActivity;

		public static Userod CurUser {
			get {
				if(_curUserT!=null) {
					return _curUserT;
				}
				if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
					throw new ApplicationException("Security.CurUser not accessible from RemotingRole.ServerWeb.");
				}
				return curUser;
			}
			set {
				if(_curUserT==value && curUser==value) {
					return;
				}
				_curUserT=value;
				curUser=value;
				UserodChangedEvent.Fire(ODEventType.Userod,_curUserT?.UserNum??curUser?.UserNum??0);
			}
		}

		public static string CurComputerName {
			get {
				if(_curComputerNameT!=null) {//Allows an empty string.
					return _curComputerNameT;
				}
				if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
					throw new ApplicationException("Security.CurComputerName not accessible from RemotingRole.ServerWeb.");
				}
				if(_curComputerName==null) {
					_curComputerName=ODEnvironment.MachineName;
				}
				return _curComputerName;
			}
			set {
				if(!ODBuild.IsWeb()) { 
					_curComputerNameT=value; 
				}
				_curComputerName=value;
			}
		}

		///<summary>Remember the password that the user typed in. This is always saved on Log On. 
		///Do not store it in the database.  We will need it when connecting to the web service.
		///Needed for CEMT and reporting servers. If eCW, then this is already encrypted.</summary>
		public static string PasswordTyped {
			get {
				if(_passwordTypedT!=null) {
					return _passwordTypedT;
				}
				if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
					throw new ApplicationException("Security.PasswordTyped not accessible from RemotingRole.ServerWeb.");
				}
				return _passwordTyped;
			}
			set {
				_passwordTypedT=value;
				_passwordTyped=value;
			}
		}

		///<summary></summary>
		public Security(){
			//No need to check MiddleTierRole; no call to db.
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.  If not authorized, it gives a Message box saying so and returns false.</summary>
		public static bool IsAuthorized(Permissions perm){
			//No need to check MiddleTierRole; no call to db.
			return IsAuthorized(perm,DateTime.MinValue,false);
		}

		///<summary>Checks to see if current user is authorized for the permission and corresponding FKey.  If not authorized, it gives a Message box 
		///saying so and returns false.</summary>
		public static bool IsAuthorized(Permissions perm,long fKey,bool suppressMessage) {
			//No need to check MiddleTierRole; no call to db.
			return IsAuthorized(perm,DateTime.MinValue,suppressMessage,true,0,-1,0,fKey);
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.  If not authorized, it gives a Message box saying so and returns false.</summary>
		public static bool IsAuthorized(Permissions perm,DateTime date){
			//No need to check MiddleTierRole; no call to db.
			return IsAuthorized(perm,date,false);
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.  If not authorized, it gives a Message box saying so and returns false.</summary>
		public static bool IsAuthorized(Permissions perm,bool suppressMessage){
			//No need to check MiddleTierRole; no call to db.
			return IsAuthorized(perm,DateTime.MinValue,suppressMessage);
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.  If not authorized, it gives a Message box saying so and returns false.</summary>
		public static bool IsAuthorized(Permissions perm,DateTime date,bool suppressMessage){
			//No need to check MiddleTierRole; no call to db.
			return IsAuthorized(perm,date,suppressMessage,false);
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.  If not authorized, it gives a Message box saying so and returns false.</summary>
		public static bool IsAuthorized(Permissions perm,DateTime date,bool suppressMessage,bool suppressLockDateMessage) {
			return IsAuthorized(perm,date,suppressMessage,suppressLockDateMessage,0,-1,0,0);
		}

		public static bool IsAuthorized(Permissions perm,DateTime date,long procCodeNum,double procCodeFee) {
			return IsAuthorized(perm,date,false,false,procCodeNum,procCodeFee,0,0);
		}

		public static bool IsAuthorized(Permissions perm,DateTime date,bool suppressMessage,bool suppressLockDateMessage,Userod curUser) {
			return IsAuthorized(perm,date,suppressMessage,suppressLockDateMessage,curUser,0,-1,0,0);
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.
		///If not authorized, actionOnUnauthorized is invoked with a pre-translated messaged and method returns false.</summary>
		public static bool IsAuthorized(Permissions perm,DateTime date,long procCodeNum=0,double procCodeFee=-1,Action<string> actionNotAuthorized=null) {
			//No need to check MiddleTierRole; no call to db.
			return IsAuthorized(perm,date,suppressMsgBox:true,suppressLockDateMessage:true,procCodeNum,procCodeFee,sheetDefNum:0,fKey:0,actionNotAuthorized);
		}

		///<summary>Checks to see if current user is authorized.  It also checks any date restrictions.  If not authorized, it gives a Message box saying so and returns false.
		///actionOfUnauthorized provides a translated not authorized message when set.</summary>
		public static bool IsAuthorized(Permissions perm,DateTime date,bool suppressMsgBox,bool suppressLockDateMessage,long procCodeNum,
			double procCodeFee,long sheetDefNum,long fKey,Action<string> actionNotAuthorized=null) 
		{
			//No need to check MiddleTierRole; no call to db.
			if(Security.CurUser==null) {
				string msg=Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(perm);
				if(!suppressMsgBox) {
					MessageBox.Show(msg);
				}
				actionNotAuthorized?.Invoke(msg);
				return false;
			}
			try {
				return IsAuthorized(perm,date,suppressMsgBox,suppressLockDateMessage,curUser,procCodeNum,procCodeFee,sheetDefNum,fKey,actionNotAuthorized);
			}
			catch(Exception ex) {
				if(!suppressMsgBox) {
					MessageBox.Show(ex.Message);
				}
				actionNotAuthorized?.Invoke(ex.Message);
				return false;
			}
		}

		///<summary>Will throw an error if not authorized and message not suppressed.</summary>
		public static bool IsAuthorized(Permissions perm,DateTime date,bool suppressException,bool suppressLockDateMessage,Userod curUser,
			long procCodeNum,double procFee,long sheetDefNum,long fKey,Action<string> actionNotAuthorized=null) 
		{
			//No need to check MiddleTierRole; no call to db.
			date=date.Date; //Remove the time portion of date so we can compare strictly as a date later.
			//Check eConnector permission first.
			if(IsValidEServicePermission(perm)) {
				return true;
			}
			string errorMsg;
			if(!GroupPermissions.HasPermission(curUser,perm,fKey)){
				errorMsg=Lans.g("Security","Not authorized.")+"\r\n"
						+Lans.g("Security","A user with the SecurityAdmin permission must grant you access for")+":\r\n"
						+GroupPermissions.GetDesc(perm);
				if(!suppressException){
					throw new Exception(errorMsg);
				}
				actionNotAuthorized?.Invoke(errorMsg);
				return false;
			}
			if(perm==Permissions.AccountingCreate || perm==Permissions.AccountingEdit){
				if(date <= PrefC.GetDate(PrefName.AccountingLockDate)){
					errorMsg=Lans.g("Security","Locked by Administrator.");
					if(!suppressException && !suppressLockDateMessage) {
						throw new Exception(errorMsg);
					}
					actionNotAuthorized?.Invoke(errorMsg);
					return false;	
				}
			}
			//Check the global security lock------------------------------------------------------------------------------------
			if(IsGlobalDateLock(perm,date,suppressException||suppressLockDateMessage,procCodeNum,procFee,sheetDefNum,actionNotAuthorized)) {
				return false;
			}
			//Check date/days limits on individual permission----------------------------------------------------------------
			if(!GroupPermissions.PermTakesDates(perm)){
				return true;
			}
			//Include CEMT users, as a CEMT user could be logged in when this is checked.
			DateTime dateLimit =GetDateLimit(perm,curUser.GetGroups(true).Select(x => x.UserGroupNum).ToList()); 
			if(date>dateLimit){//authorized
				return true;
			}
			//Prevents certain bugs when 1/1/1 dates are passed in and compared----------------------------------------------
			//Handling of min dates.  There might be others, but we have to handle them individually to avoid introduction of bugs.
			if(perm==Permissions.ClaimDelete//older versions did not have SecDateEntry
				|| perm==Permissions.ClaimSentEdit//no date sent was entered before setting claim received	
				|| perm==Permissions.ProcComplCreate
				|| perm==Permissions.ProcCompleteEdit
				|| perm==Permissions.ProcCompleteStatusEdit
				|| perm==Permissions.ProcCompleteNote
				|| perm==Permissions.ProcCompleteAddAdj
				|| perm==Permissions.ProcCompleteEditMisc
				|| perm==Permissions.ProcExistingEdit//a completed EO or EC procedure with a min date.
				|| perm==Permissions.InsPayEdit//a claim payment with no date.
				|| perm==Permissions.InsWriteOffEdit//older versions did not have SecDateEntry or DateEntryC
				|| perm==Permissions.TreatPlanEdit
				|| perm==Permissions.AdjustmentCreate
				|| perm==Permissions.AdjustmentEdit
				|| perm==Permissions.CommlogEdit//usually from a conversion
				|| perm==Permissions.ProcDelete//because older versions did not set the DateEntryC.
				|| perm==Permissions.ImageDelete//In case an image has a document.DateCreated date of DateTime.MinVal.
				|| perm==Permissions.PerioEdit//In case perio chart exam has a creation date of DateTime.MinValue.
				|| perm==Permissions.PreAuthSentEdit//older versions did not have SecDateEntry
				|| perm==Permissions.ClaimProcReceivedEdit//
				|| perm==Permissions.PaymentCreate//Older versions did not have a date limitation to PaymentCreate
				|| perm==Permissions.ImageEdit//In case an image has a document.DateCreated date of DateTime.MinVal.
				|| perm==Permissions.ImageExport//In case an image has a document.DateCreated date of DateTime.MinVal.
				|| perm==Permissions.SheetEdit)//In case a sheet has a sheet.DateTimeSheet date of DateTime.MinVal, will still allow the office to edit the sheet.
			{
				if(date.Year<1880	&& dateLimit.Year<1880) {
					return true;
				}
			}
			errorMsg=Lans.g("Security","Not authorized for")+"\r\n"
					+GroupPermissions.GetDesc(perm)+"\r\n"+Lans.g("Security","Date limitation");
			if(!suppressException){
				throw new Exception(errorMsg);
			}
			actionNotAuthorized?.Invoke(errorMsg);
			return false;		
		}

		///<summary>Surround with Try/Catch. Error messages will be thrown to caller.</summary>
		public static bool IsGlobalDateLock(Permissions perm,DateTime date,bool suppressMsgBox=false,long codeNum=0,double procFee=-1,long sheetDefNum=0,Action<string> actionNotAuthorized=null) {
			if(!(new[] {
				 Permissions.AdjustmentCreate
				,Permissions.AdjustmentEdit
				,Permissions.PaymentCreate
				,Permissions.PaymentEdit
				,Permissions.ProcComplCreate
				,Permissions.ProcCompleteEdit
				,Permissions.ProcCompleteStatusEdit
				//,Permissions.ProcComplNote (corresponds to obsolete ProcComplEditLimited)
				//,Permissions.ProcComplAddAdj (corresponds to obsolete ProcComplEditLimited)
				//,Permissions.ProcComplEditMisc (corresponds to obsolete ProcComplEditLimited)
				//,Permissions.ProcExistingEdit//per Allen 6/26/2020 this should not be affected by the global date lock
			//,Permissions.ImageDelete
				,Permissions.InsPayCreate
				,Permissions.InsPayEdit
			//,Permissions.InsWriteOffEdit//per Nathan 7/5/2016 this should not be affected by the global date lock
				,Permissions.SheetEdit
				,Permissions.SheetDelete
				,Permissions.CommlogEdit
			//,Permissions.ClaimDelete //per Nathan 01/18/2018 this should not be affected by the global date lock
				,Permissions.PayPlanEdit
			//,Permissions.ClaimHistoryEdit //per Nathan & Mark 03/01/2018 this should not be affected by the global lock date, not financial data.
			}).Contains(perm)) {
				return false;//permission being checked is not affected by global lock date.
			}
			if(date.Year==1) {
				return false;//Invalid or MinDate passed in.
			}
			if(!PrefC.GetBool(PrefName.SecurityLockIncludesAdmin) && GroupPermissions.HasPermission(Security.CurUser,Permissions.SecurityAdmin,0)) {
				return false;//admins are never affected by global date limitation when preference is false.
			}
			List<Permissions> listPermissionsCanBypassLockDate=new List<Permissions>() {
				Permissions.ProcCompleteEdit,Permissions.ProcCompleteAddAdj,Permissions.ProcCompleteEditMisc,Permissions.ProcCompleteStatusEdit,Permissions.ProcCompleteNote,
				Permissions.ProcComplCreate,Permissions.ProcExistingEdit
			};
			if(listPermissionsCanBypassLockDate.Contains(perm) && ProcedureCodes.CanBypassLockDate(codeNum,procFee)) {
				return false;
			}
			if(perm.In(Permissions.SheetEdit,Permissions.SheetDelete) && sheetDefNum > 0	&& SheetDefs.CanBypassLockDate(sheetDefNum)) {
				return false;
			}
			//If global lock is Date based.
			if(date <= PrefC.GetDate(PrefName.SecurityLockDate)) {
				string msg=Lans.g("Security", "Locked by Administrator before ") + PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
				if(!suppressMsgBox) {
					MessageBox.Show(msg);
				}
				actionNotAuthorized?.Invoke(msg);
				return true;
			}
			//If global lock is days based.
			int lockDays = PrefC.GetInt(PrefName.SecurityLockDays);
			if(lockDays>0 && date<=DateTime.Today.AddDays(-lockDays)) {
				string msg=Lans.g("Security", "Locked by Administrator before") + " " + lockDays.ToString() + " days.";
				if(!suppressMsgBox) {
					MessageBox.Show(msg);
				}
				actionNotAuthorized?.Invoke(msg);
				return true;
			}
			return false;
		}

		///<summary>Returns CurComputerName, ODEnvironment.MachineName, and Environment.MachineName in a comma seperated string. Replaces CurComputerName in some instances</summary>
		public static string GetComplexComputerName() {
			//If not RDP return CurComputerName for backwards compatibillity. Mimics ODEnvironment.MachineName
			if(typeof(SystemInformation).GetProperty("TerminalServerSession").GetValue(null).ToString()!="True"){
				return CurComputerName;
			}
			string[] arrayComputerNames=new string[] { CurComputerName,ODEnvironment.MachineName,Environment.MachineName };
			return string.Join(", ",arrayComputerNames.Where(x => !string.IsNullOrEmpty(x)).Distinct());
		}
		
		///<summary>Returns the Date that the user is restricted to for the passed-in PermType. 
		///Returns MinVal if the user is not restricted or does not have the permission.</summary>
		private static DateTime GetDateLimit(Permissions permType,List<long> listUserGroupNums){
			//No need to check MiddleTierRole; no call to db.
			return GroupPermissions.GetDateRestrictedForPermission(permType,listUserGroupNums);
		}

		///<summary>Gets a module that the user has permission to use.  Tries the suggestedI first.  If a -1 is supplied, it tries to find any authorized module.  If no authorization for any module, it returns a -1, causing no module to be selected.</summary>
		public static int GetModule(int suggestI){
			//No need to check MiddleTierRole; no call to db.
			if(suggestI!=-1 && IsAuthorized(PermofModule(suggestI),DateTime.MinValue,true)){
				return suggestI;
			}
			for(int i=0;i<7;i++){
				if(IsAuthorized(PermofModule(i),DateTime.MinValue,true)){
					return i;
				}
			}
			return -1;
		}

		private static Permissions PermofModule(int i){
			//No need to check MiddleTierRole; no call to db.
			switch(i){
				case 0:
					return Permissions.AppointmentsModule;
				case 1:
					return Permissions.FamilyModule;
				case 2:
					return Permissions.AccountModule;
				case 3:
					return Permissions.TPModule;
				case 4:
					return Permissions.ChartModule;
				case 5:
					return Permissions.ImagingModule;
				case 6:
					return Permissions.ManageModule;
			}
			return Permissions.None;
		}

		///<summary>Synchronizes CurUser with the corresponding userod object from the user cache.  Used to update CurUser with any changes from cache refreshes.
		///Throws an exception if CurUser was instantiated but is no longer in the user cache.  Does nothing if CurUser is null when invoked.</summary>
		public static void SyncCurUser() {
			if(CurUser==null || CurUser.UserNum==0) {//Usernum will be 0 for users instantiated for web. See InitWebcore.Init.
				return;
			}
			//Update CurUser with the user from the cache synchronizing any fields that could have been updated.  E.g. TaskListInBox
			CurUser=Userods.GetFirstOrDefault(x => x.UserNum==CurUser.UserNum);
			//The user could have been deleted and/or data loss could have occurred and the CurUser is no longer in the db.
			if(CurUser==null && !ODInitialize.IsRunningInUnitTest) {
				throw new ODException("The current user has been removed from the cache.");
			}
		}

		///<summary>Will throw an exception if server cannot validate username and password.
		///configPath will be empty from a workstation and filled from the server.  If Ecw, odpass will actually be the hash.</summary>
		public static Userod LogInWeb(string oduser,string odpass,string configPath,string clientVersionStr,bool usingEcw) {
			//Unusual remoting role check designed for first time logging in via middle tier.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
				Userod user=Userods.CheckUserAndPassword(oduser,odpass,usingEcw);
				if(string.IsNullOrEmpty(odpass)) {//All middle tier connections must pass in a password.
					throw new ODException("Invalid username or password.");
				}
				if(!string.IsNullOrEmpty(clientVersionStr)) {
					string serverVersionStr=Assembly.GetAssembly(typeof(Db)).GetName().Version.ToString(4);
					Version clientVersion=new Version(clientVersionStr);
					Version serverVersion=new Version(serverVersionStr);
					if(clientVersion > serverVersion) {
						throw new ODException("Version mismatch.  Client:"+clientVersionStr+"  Server:"+serverVersionStr);
					}
				}
				return user;
			}
			else {
				//Because RemotingRole has not been set, and because CurUser has not been set,
				//this particular method is more verbose than most and does not use Meth.
				//It's not a good example of the standard way of doing things.
				DtoGetObject dto=new DtoGetObject();
				dto.Credentials=new Credentials();
				dto.Credentials.Username=oduser;
				dto.Credentials.Password=odpass;//Userods.EncryptPassword(password);
				dto.ComputerName=Security.CurComputerName;
				dto.MethodName="OpenDentBusiness.Security.LogInWeb";
				dto.ObjectType=typeof(Userod).FullName;
				object[] parameters=new object[] { oduser,odpass,configPath,clientVersionStr,usingEcw };
				dto.Params=DtoObject.ConstructArray(MethodBase.GetCurrentMethod(),parameters);
				//Purposefully throws exceptions.  
				//If hasConnectionLost was set to true then the user would be stuck in an infinite loop of trying to connect to a potentially invalid Middle Tier URI.
				//Therefore, set hasConnectionLost false so that the user gets an error message immediately in the event that Middle Tier cannot be reached.
				return RemotingClient.ProcessGetObject<Userod>(dto,false);
			}
		}

		#region eServices

		///<summary>Returns false if the currently logged in user is not designated for the eConnector or if the user does not have permission.</summary>
		private static bool IsValidEServicePermission(Permissions perm) {
			//No need to check MiddleTierRole; no call to db.
			if(curUser==null) {
				return false;
			}
			//Run specific checks against certain types of eServices.
			switch(curUser.EServiceType) {
				case EServiceTypes.Broadcaster:
				case EServiceTypes.BroadcastMonitor:
				case EServiceTypes.ServiceMainHQ:
					return true;//These eServices are at HQ and we trust ourselves to have full permissions for any S class method.
				case EServiceTypes.EConnector:
					return IsPermAllowedEConnector(perm);
				case EServiceTypes.OpenDentalService:
					return IsPermAllowedOpenDentalService(perm);
				case EServiceTypes.None:
				default:
					return false;//Not an eService, let IsAuthorized handle the permission checking.
			}
		}

		///<summary>Returns true if the eConnector should be allowed to run methods with the passed in permission.</summary>
		private static bool IsPermAllowedEConnector(Permissions perm) {
			//We are typically on the customers eConnector and need to be careful when giving access to certain permission types.
			//Engineers must EXCPLICITLY add permissions to this switch statement as they need them.
			//Be very cautious when adding permissions because the flood gates for that permission will be opened once added.
			//E.g. we should never add a permission like Setup or SecurityAdmin.  If there is a need for such a thing, we need to rethink this paradigm.
			switch(perm) {
				//Add additional permissions to this case as needed to grant access.
				case Permissions.EmailSend:
					return true;
				default:
					return false;
			}
		}

		///<summary>Returns true if the OpenDentalService should be allowed to run methods with the passed in permission.</summary>
		private static bool IsPermAllowedOpenDentalService(Permissions perm) {
			//We need to be careful when giving access to certain permission types.
			//Engineers must EXCPLICITLY add permissions to this switch statement as they need them.
			//Be very cautious when adding permissions because the flood gates for that permission will be opened once added.
			//E.g. we should never add a permission like Setup or SecurityAdmin.  If there is a need for such a thing, we need to rethink this paradigm.
			switch(perm) {
				//Add additional permissions to this case as needed to grant access.
				case Permissions.EmailSend:
					return true;
				default:
					return false;
			}
		}

		#endregion

	}

}
