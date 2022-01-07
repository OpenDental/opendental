using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SessionTokens {

		///<summary></summary>
		public static long Insert(SessionToken sessionToken) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				sessionToken.SessionTokenNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sessionToken);
				return sessionToken.SessionTokenNum;
			}
			return Crud.SessionTokenCrud.Insert(sessionToken);
		}

		///<summary>Generates a new token and inserts it into the database.</summary>
		public static SessionToken GenerateToken(SessionTokenType tokenType,long fkey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SessionToken>(MethodBase.GetCurrentMethod(),tokenType,fkey);
			}
			SessionToken token=new SessionToken {
				FKey=fkey,
				TokenType=tokenType,
				Expiration=MiscData.GetNowDateTime().AddHours(24),
				TokenPlainText=ODCrypt.CryptUtil.RandomString(128),//Using 128 bits because it would be impossible to brute force using today's techniques
			};
			token.SessionTokenHash=GetHash(token.TokenPlainText);
			Insert(token);
			//These fields don't need to be sent to the client.
			token.SessionTokenHash="";
			token.SessionTokenNum=0;
			return token;
		}

		///<summary>Hashes the session token using SHA3.</summary>
		private static string GetHash(string sessionToken) {
			//Salt not necessary because the token is almost certainly unique.
			PasswordContainer passwordContainer=new PasswordContainer(HashTypes.SHA3_512,"",Authentication.HashPasswordSHA512(sessionToken));
			return passwordContainer.ToString();
		}

		///<summary>Checks that the passed in token is valid. Throws if not. Returns the matching token from the database.</summary>
		public static SessionToken CheckToken(string sessionToken,SessionTokenType tokenType,long fkey) {
			return CheckToken(sessionToken,new List<SessionTokenType> { tokenType },fkey);
		}

		///<summary>Checks that the passed in token is valid. Throws if not. Returns the matching token from the database.</summary>
		public static SessionToken CheckToken(string sessionToken,List<SessionTokenType> listTokenTypes,long fkey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SessionToken>(MethodBase.GetCurrentMethod(),sessionToken,listTokenTypes,fkey);
			}
			if(sessionToken.IsNullOrEmpty()) {
				throw new ODException("Invalid credentials");
			}
			if(listTokenTypes.Count==0) {
				throw new Exception("Token type required");
			}
			string tokenHash=GetHash(sessionToken);
			string command=$"SELECT *,NOW() DatabaseTime FROM sessiontoken WHERE SessionTokenHash='{POut.String(tokenHash)}' ";
			if(listTokenTypes.Any(x => x!=SessionTokenType.Undefined)) {
				command+=$"AND TokenType IN({string.Join(",",listTokenTypes.Select(x =>  POut.Int((int)x)))}) ";
			}
			DataTable table=Db.GetTable(command);
			List<SessionToken> listTokens=Crud.SessionTokenCrud.TableToList(table);
			if(listTokens.Count==0) {
				throw new ODException("Invalid credentials");
			}
			SessionToken token=listTokens[0];
			DateTime dbTime=PIn.DateT(table.Rows[0]["DatabaseTime"].ToString());
			if(token.Expiration < dbTime) {
				//Normally won't get here because apps will request a new token before it expires.
				throw new ODException("Session has expired.",ODException.ErrorCodes.SessionExpired);
			}
			bool isAuthorized=false;
			if(fkey==0 || fkey==token.FKey 
				//Check if the FKey for the token is allowed to view the patient for the request
				|| (listTokenTypes.Contains(SessionTokenType.PatientPortal) && Patients.GetPatNumsForPhi(token.FKey).Contains(fkey))) 
			{
				isAuthorized=true;
			}
			if(!isAuthorized) {
				//For example, the token is for a different patient than the patient for which information is being requested.
				throw new ODException("Invalid credentials");
			}
			return token;
		}
		///<summary>Deletes the token if it is present in the database.</summary>
		public static void DeleteToken(string sessionToken) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sessionToken);
				return;
			}
			if(sessionToken.IsNullOrEmpty()) {
				throw new ODException("Invalid credentials");
			}
			string tokenHash=GetHash(sessionToken);
			string command=$"DELETE FROM sessiontoken WHERE SessionTokenHash='{POut.String(tokenHash)}'";
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<SessionToken> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SessionToken>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM sessiontoken WHERE PatNum = "+POut.Long(patNum);
			return Crud.SessionTokenCrud.SelectMany(command);
		}
		
		///<summary>Gets one SessionToken from the db.</summary>
		public static SessionToken GetOne(long sessionTokenNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<SessionToken>(MethodBase.GetCurrentMethod(),sessionTokenNum);
			}
			return Crud.SessionTokenCrud.SelectOne(sessionTokenNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Update(SessionToken sessionToken){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sessionToken);
				return;
			}
			Crud.SessionTokenCrud.Update(sessionToken);
		}
		///<summary></summary>
		public static void Delete(long sessionTokenNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sessionTokenNum);
				return;
			}
			Crud.SessionTokenCrud.Delete(sessionTokenNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/

	}
}