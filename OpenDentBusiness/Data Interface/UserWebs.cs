using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using ODCrypt;

namespace OpenDentBusiness{
	///<summary></summary>
	public class UserWebs {
		#region Get Methods

		///<summary>Gets all UserWebs from the db for the passed in type.</summary>
		public static List<UserWeb> GetAllByType(UserWebFKeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserWeb>>(MethodBase.GetCurrentMethod(),fkeyType);
			}
			string command=$"SELECT * FROM userweb WHERE FKeyType={POut.Int((int)fkeyType)}";
			return Crud.UserWebCrud.SelectMany(command);
		}

		///<summary>Gets one UserWeb from the db.</summary>
		public static UserWeb GetOne(long userWebNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<UserWeb>(MethodBase.GetCurrentMethod(),userWebNum);
			}
			return Crud.UserWebCrud.SelectOne(userWebNum);
		}

		///<summary>Gets the UserWeb associated to the passed in username and hashed password.  Must provide the FKeyType.  Returns null if not found.</summary>
		public static UserWeb GetByUserNameAndPassword(string userName,string passwordHashed,UserWebFKeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<UserWeb>(MethodBase.GetCurrentMethod(),userName,passwordHashed,fkeyType);
			}
			string command="SELECT * "
				+"FROM userweb "
				+"WHERE Password='"+passwordHashed+"' "
				+"AND UserName='"+OpenDentBusiness.POut.String(userName)+"' "
				+"AND FKeyType="+OpenDentBusiness.POut.Int((int)fkeyType)+"";
			return Crud.UserWebCrud.SelectOne(command);
		}

		///<summary>Gets the UserWeb associated to the passed in username.  Must provide the FKeyType.  Returns null if not found.</summary>
		public static UserWeb GetByUserName(string userName,UserWebFKeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<UserWeb>(MethodBase.GetCurrentMethod(),userName,fkeyType);
			}
			string command="SELECT * "
				+"FROM userweb "
				+"WHERE UserName='"+OpenDentBusiness.POut.String(userName)+"' "
				+"AND FKeyType="+OpenDentBusiness.POut.Int((int)fkeyType)+"";
			return Crud.UserWebCrud.SelectOne(command);
		}

		///<summary>Gets the UserWeb associated to the passed in username and reset code.  Must provide the FKeyType.  Returns null if not found.</summary>
		public static UserWeb GetByUserNameAndResetCode(string userName,string resetCode,UserWebFKeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<UserWeb>(MethodBase.GetCurrentMethod(),userName,resetCode,fkeyType);
			}
			string command="SELECT * "
				+"FROM userweb "
				+"WHERE userweb.FKeyType="+POut.Int((int)UserWebFKeyType.PatientPortal)+" "
				+"AND userweb.UserName='"+POut.String(userName)+"' "
				+"AND userweb.PasswordResetCode='"+POut.String(resetCode)+"' ";
			return Crud.UserWebCrud.SelectOne(command);
		}

		public static UserWeb GetByFKeyAndType(long fkey,UserWebFKeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<UserWeb>(MethodBase.GetCurrentMethod(),fkey,fkeyType);
			}
			string command="SELECT * FROM userweb WHERE FKey="+POut.Long(fkey)+" AND FKeyType="+POut.Int((int)fkeyType)+" ";
			return Crud.UserWebCrud.SelectOne(command);
		}
		
		#endregion

		#region Insert

		///<summary></summary>
		public static long Insert(UserWeb userWeb) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				userWeb.UserWebNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userWeb);
				return userWeb.UserWebNum;
			}
			return Crud.UserWebCrud.Insert(userWeb);
		}

		///<summary></summary>
		public static void InsertMany(List<UserWeb> listUserWebs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listUserWebs);
				return;
			}
			Crud.UserWebCrud.InsertMany(listUserWebs);
		}

		#endregion

		#region Update

		///<summary></summary>
		public static void Update(UserWeb userWeb) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userWeb);
				return;
			}
			Crud.UserWebCrud.Update(userWeb);
		}

		///<summary></summary>
		public static void Update(UserWeb userWeb,UserWeb userWebOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userWeb,userWebOld);
				return;
			}
			Crud.UserWebCrud.Update(userWeb,userWebOld);
		}

		#endregion

		#region Delete

		///<summary></summary>
		public static void Delete(long userWebNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userWebNum);
				return;
			}
			Crud.UserWebCrud.Delete(userWebNum);
		}

		#endregion

		#region Misc Methods

		///<summary>Creates a username that is not yet in use. Should typically call UserWebs.GetNewPatientPortalCredentials() instead.
		///If you are not inserting the name into UserWeb immediately then listExcludedNames should persist across multiple calls.</summary>
		public static string CreateUserNameFromPat(Patient pat,UserWebFKeyType fkeyType,List<string> listExcludedNames) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			bool isUserNameOk=false;
			int i=0;
			while(!isUserNameOk) {
				retVal=pat.FName+ODRandom.Next(100,100000);
				if(!UserWebs.UserNameExists(retVal,fkeyType)) {
					if(!listExcludedNames.Contains(retVal)) {
						isUserNameOk=true;
					}
				}
				if(i>1000) {
					throw new CodeBase.ODException(Lans.g("UserWebs","Unable to create username for patient."));
				}
				i++;
			}
			return retVal;
		}

		///<summary>Generates a random password 8 char long containing at least one uppercase, one lowercase, and one number.</summary>
		public static string PassGen(int len) {
			if(len<0) {
				len=0;
			}
			//Leave out characters that can cause confusion (o,O,0,l,1,I).
			string lowerCase="abcdefgijkmnopqrstwxyz";
			string upperCase="ABCDEFGHJKLMNPQRSTWXYZ";
			string numbers="23456789";
			string allChars=lowerCase+upperCase+numbers;
			string passChars="";
			//Grab a letter from each so know we have one of each.
			foreach(string s in new string[] { lowerCase,upperCase,numbers }) {
				passChars+=s[CryptUtil.Random<int>()%s.Length];
			}
			//Start at 3 because we already added 3 characters
			for(int i=3;i<len;i++) {
				passChars+=allChars[ODCrypt.CryptUtil.Random<int>()%allChars.Length];
			}
			//Now that we have our character set, now we do a Fisher-Yates shuffle.
			char[] chars=passChars.ToCharArray();
			int arraysize = chars.Length;
			int random;
			char temp;
			for (int i = 0; i < arraysize; i++) {
				random = i + (int)(CryptUtil.Random<int>()%(arraysize - i));
				temp = chars[random];
				chars[random] = chars[i];
				chars[i] = temp;
			}
			//Take a substring in case the requested length is 1 or 2 characters.
			return new string(chars).Substring(0,len);
		}

		///<summary>Generates a random password 8 char long containing at least one uppercase, one lowercase, and one number.</summary>
		public static string GenerateRandomPassword(int length) {
			//No need to check RemotingRole; no call to db.
			//Chracters like o(letter O), 0 (Zero), l (letter l), 1 (one) etc are avoided because they can be ambigious.
			string PASSWORD_CHARS_LCASE="abcdefgijkmnopqrstwxyz";
			string PASSWORD_CHARS_UCASE="ABCDEFGHJKLMNPQRSTWXYZ";
			string PASSWORD_CHARS_NUMERIC="23456789";
			//Create a local array containing supported password characters grouped by types.
			char[][] charGroups=new char[][]{
						PASSWORD_CHARS_LCASE.ToCharArray(),
						PASSWORD_CHARS_UCASE.ToCharArray(),
						PASSWORD_CHARS_NUMERIC.ToCharArray(),};
			//Use this array to track the number of unused characters in each character group.
			int[] charsLeftInGroup=new int[charGroups.Length];
			//Initially, all characters in each group are not used.
			for(int i = 0;i<charsLeftInGroup.Length;i++) {
				charsLeftInGroup[i]=charGroups[i].Length;
			}
			//Use this array to track (iterate through) unused character groups.
			int[] leftGroupsOrder=new int[charGroups.Length];
			//Initially, all character groups are not used.
			for(int i = 0;i<leftGroupsOrder.Length;i++) {
				leftGroupsOrder[i]=i;
			}
			//This array will hold password characters.
			char[] password=new char[length];
			//Index of the next character to be added to password.
			int nextCharIdx;
			//Index of the next character group to be processed.
			int nextGroupIdx;
			//Index which will be used to track not processed character groups.
			int nextLeftGroupsOrderIdx;
			//Index of the last non-processed character in a group.
			int lastCharIdx;
			//Index of the last non-processed group.
			int lastLeftGroupsOrderIdx=leftGroupsOrder.Length - 1;
			//Generate password characters one at a time.
			for(int i = 0;i<password.Length;i++) {
				//If only one character group remained unprocessed, process it;
				//otherwise, pick a random character group from the unprocessed
				//group list. To allow a special character to appear in the
				//first position, increment the second parameter of the Next
				//function call by one, i.e. lastLeftGroupsOrderIdx + 1.
				if(lastLeftGroupsOrderIdx==0) {
					nextLeftGroupsOrderIdx=0;
				}
				else {
					nextLeftGroupsOrderIdx=ODRandom.Next(0,lastLeftGroupsOrderIdx);
				}
				//Get the actual index of the character group, from which we will
				//pick the next character.
				nextGroupIdx=leftGroupsOrder[nextLeftGroupsOrderIdx];
				//Get the index of the last unprocessed characters in this group.
				lastCharIdx=charsLeftInGroup[nextGroupIdx] - 1;
				//If only one unprocessed character is left, pick it; otherwise,
				//get a random character from the unused character list.
				if(lastCharIdx==0) {
					nextCharIdx=0;
				}
				else {
					nextCharIdx=ODRandom.Next(0,lastCharIdx+1);
				}
				//Add this character to the password.
				password[i]=charGroups[nextGroupIdx][nextCharIdx];
				//If we processed the last character in this group, start over.
				if(lastCharIdx==0) {
					charsLeftInGroup[nextGroupIdx]=charGroups[nextGroupIdx].Length;
					//There are more unprocessed characters left.
				}
				else {
					//Swap processed character with the last unprocessed character
					//so that we don't pick it until we process all characters in
					//this group.
					if(lastCharIdx !=nextCharIdx) {
						char temp=charGroups[nextGroupIdx][lastCharIdx];
						charGroups[nextGroupIdx][lastCharIdx]=charGroups[nextGroupIdx][nextCharIdx];
						charGroups[nextGroupIdx][nextCharIdx]=temp;
					}
					//Decrement the number of unprocessed characters in
					//this group.
					charsLeftInGroup[nextGroupIdx]--;
				}
				//If we processed the last group, start all over.
				if(lastLeftGroupsOrderIdx==0) {
					lastLeftGroupsOrderIdx=leftGroupsOrder.Length - 1;
					//There are more unprocessed groups left.
				}
				else {
					//Swap processed group with the last unprocessed group
					//so that we don't pick it until we process all groups.
					if(lastLeftGroupsOrderIdx !=nextLeftGroupsOrderIdx) {
						int temp=leftGroupsOrder[lastLeftGroupsOrderIdx];
						leftGroupsOrder[lastLeftGroupsOrderIdx]=
																leftGroupsOrder[nextLeftGroupsOrderIdx];
						leftGroupsOrder[nextLeftGroupsOrderIdx]=temp;
					}
					//Decrement the number of unprocessed groups.
					lastLeftGroupsOrderIdx--;
				}
			}
			//Convert password characters into a string and return the result.
			return new string(password);
		}

		public static bool ValidatePatientAccess(Patient pat,out string strErrors) {
			//No need to check RemotingRole; no call to db.
			StringBuilder strbErrors=new StringBuilder();
			if(pat.FName.Trim()=="") {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient first name."));
			}
			if(pat.LName.Trim()=="") {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient last name."));
			}
			if(pat.Address.Trim()=="") {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient address line 1."));
			}
			if(pat.City.Trim()=="") {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient city."));
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("US") && pat.State.Trim().Length!=2) {
				strbErrors.AppendLine(Lans.g("PatientPortal","Invalid patient state.  Must be two letters."));
			}
			if(pat.Birthdate.Year<1880) {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient birth date."));
			}
			if(pat.HmPhone.Trim()=="" && pat.WirelessPhone.Trim()=="" && pat.WkPhone.Trim()=="") {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient phone;  Must have home, wireless, or work phone."));
			}
			if(pat.Email.Trim()=="") {
				strbErrors.AppendLine(Lans.g("PatientPortal","Missing patient email."));
			}
			strErrors=strbErrors.ToString();
			return strErrors=="";
		}

		///<summary>Updates password info in db for given inputs if PlainTextPassword (Item2) is not empty.
		///Insert EhrMeasureEvent OnlineAccessProvided if previous db version of UserWeb had no PasswordHash (access has now been granted to portal).
		///Returns true if UserWeb row was updated. Otherwise returns false.</summary>
		public static bool UpdateNewPatientPortalCredentials(Tuple<UserWeb,string,PasswordContainer> args) {
			//No need to check RemotingRole; no call to db.
			if(args==null) {
				return false;
			}
			UserWeb userWeb=args.Item1; string passwordPlainText=args.Item2; PasswordContainer loginDetails=args.Item3;
			if(userWeb!=null && !string.IsNullOrEmpty(passwordPlainText)) {
				//Only insert an EHR event if the password was previously blank (meaning they don't currently have access).
				if(string.IsNullOrEmpty(userWeb.PasswordHash)) {
					EhrMeasureEvents.Insert(new EhrMeasureEvent() {
						DateTEvent=DateTime.Now,
						EventType=EhrMeasureEventType.OnlineAccessProvided,
						PatNum=userWeb.FKey, //PatNum.
						MoreInfo="",
					});
				}
				//New password was created so set the flag for the user to change on next login and update the db accordingly.
				userWeb.RequirePasswordChange=true;
				userWeb.LoginDetails=loginDetails;
				UserWebs.Update(userWeb);
				return true;
			}
			return false;
		}

		///<summary>Generates a username and password if necessary for this patient. If the patient is not eligible to be given access, this will return null. 
		///Otherwise returns the UserWeb (Item1), PlainTextPassword (Item2), PasswordContainer (Item3). 
		///If PlainTextPassword (Item2) is empty then assume new password generation was not necessary.
		///Will insert a new UserWeb if none found for this Patient. Will leave UserWeb.PasswordHash blank. 
		///Call UpdateNewPatientPortalCredentials() using results of this method if you want to save password to db.</summary>
		///<param name="passwordOverride">If a password has already been generated for this patient, pass it in here so that the password returned
		///will match.</param>
		public static Tuple<UserWeb,string,PasswordContainer> GetNewPatientPortalCredentials(Patient pat,string passwordOverride="") {
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrEmpty(PrefC.GetString(PrefName.PatientPortalURL))) {
				return null;//Haven't set up patient portal yet.
			}
			string errors;
			if(!UserWebs.ValidatePatientAccess(pat,out errors)) {
				return null;//Patient is missing necessary fields.
			}
			UserWeb userWeb=UserWebs.GetByFKeyAndType(pat.PatNum,UserWebFKeyType.PatientPortal);			
			if(userWeb==null) {
				userWeb=new UserWeb() {
					UserName=UserWebs.CreateUserNameFromPat(pat,UserWebFKeyType.PatientPortal,new List<string>()),
					FKey=pat.PatNum,
					FKeyType=UserWebFKeyType.PatientPortal,
					RequireUserNameChange=true,
					LoginDetails=new PasswordContainer(HashTypes.None,"",""),
					IsNew=true,
				};				
				//Always insert here. We may not ever end up updating UserWeb.PasswordHash if an email is not sent to this patient.
				//This will leave a UserWeb row with a UserName (for next time) but no password. This row will be updated with a password at the appropriate time.
				UserWebs.Insert(userWeb);
			}
			bool isNewPasswordRequired=false;			
			if(string.IsNullOrEmpty(userWeb.UserName)) { //Fixing B11013 so new UserName and Password should be generated.
				userWeb.UserName=UserWebs.CreateUserNameFromPat(pat,UserWebFKeyType.PatientPortal,new List<string>());
				userWeb.RequireUserNameChange=true;
				//UserName fields have been changed so update db.
				UserWebs.Update(userWeb);
				isNewPasswordRequired=true;
			}
			if(userWeb.RequirePasswordChange) { //Could be a new UserWeb or a subsequent invite being generated for the same patient (had a second appointment).
				isNewPasswordRequired=true;
			}
			if(string.IsNullOrEmpty(userWeb.PasswordHash)) { //Patient has no password so portal access has not been previously granted.
				isNewPasswordRequired=true;
			}
			string passwordPlainText="";
			PasswordContainer loginDetails=userWeb.LoginDetails;
			if(isNewPasswordRequired) {
				//PP invites will often times call this method and get this far but not actually want to save the new creds to the db.
				//For that reason we won't actually update the db with the new password here. 
				//The caller of this method will need to call ProcessNewPatientPortalCredentialsOut() if they really want this new password to persist to the db.
				passwordPlainText=passwordOverride=="" ? UserWebs.GenerateRandomPassword(8) : passwordOverride;
				loginDetails=Authentication.GenerateLoginDetails(passwordPlainText,HashTypes.SHA3_512);
			}			
			return new Tuple<UserWeb, string, PasswordContainer>(userWeb,passwordPlainText,loginDetails);
		}

		public static bool UserNameExists(string userName,UserWebFKeyType fkeyType) {
			//No need to check RemotingRole; no call to db.
			if(GetUserNameCount(userName,fkeyType)!=0) {
				return true;
			}
			return false;
		}

		public static int GetUserNameCount(string userName,UserWebFKeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),userName,fkeyType);
			}
			string command="SELECT COUNT(*) FROM userweb WHERE UserName='"+POut.String(userName)+"' AND FKeyType="+POut.Int((int)fkeyType);
			return PIn.Int(Db.GetCount(command));
		}

		#endregion
	}
}