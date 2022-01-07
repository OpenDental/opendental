using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LoginAttempts{
		///<summary>Returns the login attempts for the given user in the last X minutes.</summary>
		public static int CountForUser(string userName,UserWebFKeyType type,int lastXMinutes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<int>(MethodBase.GetCurrentMethod(),userName,type,lastXMinutes);
			}
			string command=$@"SELECT COUNT(*) FROM loginattempt WHERE UserName='{POut.String(userName)}' AND LoginType={POut.Int((int)type)}
				AND DateTFail >= {DbHelper.DateAddMinute("NOW()",POut.Int(-lastXMinutes))}";
			return PIn.Int(Db.GetCount(command));
		}

		///<summary></summary>
		public static long InsertFailed(string userName,UserWebFKeyType type) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),userName,type);
			}
			return Crud.LoginAttemptCrud.Insert(new LoginAttempt { UserName=userName,LoginType=type });
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<LoginAttempt> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<LoginAttempt>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM loginattempt WHERE PatNum = "+POut.Long(patNum);
			return Crud.LoginAttemptCrud.SelectMany(command);
		}
		
		///<summary>Gets one LoginAttempt from the db.</summary>
		public static LoginAttempt GetOne(long loginAttemptNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<LoginAttempt>(MethodBase.GetCurrentMethod(),loginAttemptNum);
			}
			return Crud.LoginAttemptCrud.SelectOne(loginAttemptNum);
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		#endregion Insert
		#region Update
		///<summary></summary>
		public static void Update(LoginAttempt loginAttempt){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),loginAttempt);
				return;
			}
			Crud.LoginAttemptCrud.Update(loginAttempt);
		}
		#endregion Update
		#region Delete
		///<summary></summary>
		public static void Delete(long loginAttemptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),loginAttemptNum);
				return;
			}
			Crud.LoginAttemptCrud.Delete(loginAttemptNum);
		}
		#endregion Delete
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/
	}
}