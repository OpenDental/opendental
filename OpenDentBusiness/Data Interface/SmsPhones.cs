using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SmsPhones{
		///<summary>Used to display "SHORTCODE" on the customer side as the phone number for SMS sent via Short Code.  End user does not need to see the 
		///specific short code number we use at HQ.  This ensures we do not recored this communication on a different valid SmsPhone/VLN that it didn't 
		///truly take place on.  However, on the HQ side, we want records of this communication to be listed as having taken place on the actual Short 
		///Code number.</summary>
		public const string SHORTCODE="SHORTCODE";

		#region Cache Pattern

		private class SmsPhoneCache : CacheListAbs<SmsPhone> {
			protected override List<SmsPhone> GetCacheFromDb() {
				string command="SELECT * FROM smsphone";
				return Crud.SmsPhoneCrud.SelectMany(command);
			}
			protected override List<SmsPhone> TableToList(DataTable table) {
				return Crud.SmsPhoneCrud.TableToList(table);
			}
			protected override SmsPhone Copy(SmsPhone smsPhone) {
				return smsPhone.Copy();
			}
			protected override DataTable ListToTable(List<SmsPhone> listSmsPhones) {
				return Crud.SmsPhoneCrud.ListToTable(listSmsPhones,"SmsPhone");
			}
			protected override void FillCacheIfNeeded() {
				SmsPhones.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SmsPhoneCache _smsPhoneCache=new SmsPhoneCache();

		public static List<SmsPhone> GetDeepCopy(bool isShort=false) {
			return _smsPhoneCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _smsPhoneCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<SmsPhone> match,bool isShort=false) {
			return _smsPhoneCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<SmsPhone> match,bool isShort=false) {
			return _smsPhoneCache.GetFindIndex(match,isShort);
		}

		public static SmsPhone GetFirst(bool isShort=false) {
			return _smsPhoneCache.GetFirst(isShort);
		}

		public static SmsPhone GetFirst(Func<SmsPhone,bool> match,bool isShort=false) {
			return _smsPhoneCache.GetFirst(match,isShort);
		}

		public static SmsPhone GetFirstOrDefault(Func<SmsPhone,bool> match,bool isShort=false) {
			return _smsPhoneCache.GetFirstOrDefault(match,isShort);
		}

		public static SmsPhone GetLast(bool isShort=false) {
			return _smsPhoneCache.GetLast(isShort);
		}

		public static SmsPhone GetLastOrDefault(Func<SmsPhone,bool> match,bool isShort=false) {
			return _smsPhoneCache.GetLastOrDefault(match,isShort);
		}

		public static List<SmsPhone> GetWhere(Predicate<SmsPhone> match,bool isShort=false) {
			return _smsPhoneCache.GetWhere(match,isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_smsPhoneCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_smsPhoneCache.FillCacheFromTable(table);
				return table;
			}
			return _smsPhoneCache.GetTableFromCache(doRefreshCache);
		}

		public static void RefreshCache() {
			GetTableFromCache(true);
		}

		#endregion Cache Pattern

		///<summary>Gets one SmsPhone from the db. Returns null if not found.</summary>
		public static SmsPhone GetByPhone(string phoneNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SmsPhone>(MethodBase.GetCurrentMethod(),phoneNumber);
			}
			string command="SELECT * FROM smsphone WHERE PhoneNumber='"+POut.String(phoneNumber)+"'";
			return Crud.SmsPhoneCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(SmsPhone smsPhone) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				smsPhone.SmsPhoneNum=Meth.GetLong(MethodBase.GetCurrentMethod(),smsPhone);
				return smsPhone.SmsPhoneNum;
			}
			return Crud.SmsPhoneCrud.Insert(smsPhone);
		}

		///<summary></summary>
		public static void Update(SmsPhone smsPhone) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsPhone);
				return;
			}
			Crud.SmsPhoneCrud.Update(smsPhone);
		}

		///<summary>This will only be called by HQ via the listener in the event that this number has been cancelled.</summary>
		public static void UpdateToInactive(string phoneNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneNumber);
				return;
			}
			SmsPhone smsPhone=GetByPhone(phoneNumber);
			if(smsPhone==null) {
				return;
			}
			smsPhone.DateTimeInactive=DateTime.Now;
			Crud.SmsPhoneCrud.Update(smsPhone);
		}

		///<summary>Gets sms phones when not using clinics.</summary>
		public static List<SmsPhone> GetForPractice() {
			//No need to check RemotingRole; no call to db.
			//Get for practice is just getting for clinic num 0
			return GetForClinics(new List<long>() { 0 });//clinic num 0
		}

		public static List<SmsPhone> GetForClinics(List<long> listClinicNums) {
			//No need to check RemotingRole; no call to db.
			if(listClinicNums.Count==0) {
				return new List<SmsPhone>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SmsPhone>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command= "SELECT * FROM smsphone WHERE ClinicNum IN ("+String.Join(",",listClinicNums)+")";
			return Crud.SmsPhoneCrud.SelectMany(command);
		}

		public static List<SmsPhone> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SmsPhone>>(MethodBase.GetCurrentMethod());
			}
			string command= "SELECT * FROM smsphone";
			return Crud.SmsPhoneCrud.SelectMany(command);
		}

		public static DataTable GetSmsUsageLocal(List<long> listClinicNums, DateTime dateMonth,List<SmsPhone> listPhones) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listClinicNums,dateMonth,listPhones);
			}
			#region Initialize retVal DataTable
			string strNoActivePhones="No Active Phones";
			List<SmsPhone> listSmsPhones=listPhones.Where(x => ListTools.In(x.ClinicNum,listClinicNums)).ToList();
			DateTime dateStart=dateMonth.Date.AddDays(1-dateMonth.Day);//remove time portion and day of month portion. Remainder should be midnight of the first of the month
			DateTime dateEnd=dateStart.AddMonths(1);//This should be midnight of the first of the following month.
			//This query builds the data table that will be filled from several other queries, instead of writing one large complex query.
			//It is written this way so that the queries are simple to write and understand, and makes Oracle compatibility easier to maintain.
			string command=@"SELECT 
							  0 ClinicNum,
							  ' ' PhoneNumber,
							  ' ' CountryCode,
							  0 SentMonth,
							  0.0 SentCharge,
								0.0 SentDiscount,
								0.0 SentPreDiscount,
							  0 ReceivedMonth,
							  0.0 ReceivedCharge 
							FROM
							  DUAL";//this is a cute way to get a data table with the correct layout without having to query any real data.
			DataTable retVal=Db.GetTable(command).Clone();//use .Clone() to get schema only, with no rows.
			retVal.TableName="SmsUsageLocal";
			for(int i=0;i<listClinicNums.Count;i++) {
				DataRow row=retVal.NewRow();
				row["ClinicNum"]=listClinicNums[i];
				row["PhoneNumber"]=strNoActivePhones;
				SmsPhone firstActivePhone=listSmsPhones
					.Where(x => x.ClinicNum==listClinicNums[i])//phones for this clinic
					.Where(x => x.DateTimeInactive.Year<1880)//that are active
					.OrderByDescending(x => x.IsPrimary)
					.ThenBy(x => x.DateTimeActive)
					.FirstOrDefault();
				if(firstActivePhone!=null) {
					row["PhoneNumber"]=firstActivePhone.PhoneNumber;
					row["CountryCode"]=firstActivePhone.CountryCode;
				}
				row["SentMonth"]=0;
				row["SentCharge"]=0.0;
				row["SentDiscount"]=0.0;
				row["SentPreDiscount"]=0.0;
				row["ReceivedMonth"]=0;
				row["ReceivedCharge"]=0.0;
				retVal.Rows.Add(row);
			}
			#endregion
			#region Fill retVal DataTable
			//Sent Last Month
			command="SELECT ClinicNum, COUNT(*), ROUND(SUM(MsgChargeUSD),2),ROUND(SUM(MsgDiscountUSD),2)"
				+",SUM(CASE SmsPhoneNumber WHEN '"+POut.String(SmsPhones.SHORTCODE)+"' THEN 1 ELSE 0 END) FROM smstomobile "
				+"WHERE DateTimeSent >="+POut.Date(dateStart)+" "
				+"AND DateTimeSent<"+POut.Date(dateEnd)+" "
				+"AND MsgChargeUSD>0 GROUP BY ClinicNum";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				for(int j=0;j<retVal.Rows.Count;j++) {
					if(retVal.Rows[j]["ClinicNum"].ToString()!=table.Rows[i]["ClinicNum"].ToString()) {
						continue;
					}
					retVal.Rows[j]["SentMonth"]=table.Rows[i][1];//.ToString();
					retVal.Rows[j]["SentCharge"]=table.Rows[i][2];//.ToString();
					retVal.Rows[j]["SentDiscount"]=table.Rows[i][3];
					retVal.Rows[j]["SentPreDiscount"]=PIn.Double(retVal.Rows[j]["SentCharge"].ToString())+PIn.Double(retVal.Rows[j]["SentDiscount"].ToString());
					//No active phone but at least one of these messages sent from Short Code
					if(retVal.Rows[j]["PhoneNumber"].ToString()==strNoActivePhones && PIn.Long(table.Rows[i][4].ToString())>0) {
						retVal.Rows[j]["PhoneNumber"]=POut.String(SmsPhones.SHORTCODE);//display "SHORTCODE" as primary number.
					}
					break;
				}
			}
			//Received Month
			command="SELECT ClinicNum, COUNT(*),SUM(CASE SmsPhoneNumber WHEN '"+POut.String(SmsPhones.SHORTCODE)+"' THEN 1 ELSE 0 END) FROM smsfrommobile "
				+"WHERE DateTimeReceived >="+POut.Date(dateStart)+" "
				+"AND DateTimeReceived<"+POut.Date(dateEnd)+" "
				+"GROUP BY ClinicNum";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				for(int j=0;j<retVal.Rows.Count;j++) {
					if(retVal.Rows[j]["ClinicNum"].ToString()!=table.Rows[i]["ClinicNum"].ToString()) {
						continue;
					}
					retVal.Rows[j]["ReceivedMonth"]=table.Rows[i][1].ToString();
					retVal.Rows[j]["ReceivedCharge"]="0";
					//No active phone but at least one of these messages sent from Short Code
					if(retVal.Rows[j]["PhoneNumber"].ToString()==strNoActivePhones && PIn.Long(table.Rows[i][2].ToString())>0) {
						retVal.Rows[j]["PhoneNumber"]=POut.String(SmsPhones.SHORTCODE);//display "SHORTCODE" as primary number.
					}
					break;
				}
			}
			#endregion
			return retVal;
		}
		
		///<summary>Find all phones in the db (by PhoneNumber) and sync with listPhonesSync. If a given PhoneNumber does not already exist then insert the SmsPhone.
		///If a given PhoneNumber exists in the local db but does not exist in the HQ-provided listPhoneSync, then deacitvate that phone locallly.
		///Return true if a change has been made to the database.</summary>
		public static bool UpdateOrInsertFromList(List<SmsPhone> listPhonesSync) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listPhonesSync);
			}
			//Get all phones so we can filter as needed below.
			string command="SELECT * FROM smsphone";
			List<SmsPhone> listPhonesDb=Crud.SmsPhoneCrud.SelectMany(command);
			bool isChanged=false;
			//Deal with phones that occur in the HQ-supplied list.
			foreach(SmsPhone phoneSync in listPhonesSync) {
				SmsPhone phoneOld=listPhonesDb.FirstOrDefault(x => x.PhoneNumber==phoneSync.PhoneNumber);
				//Upsert.
				if(phoneOld!=null) { //This phone already exists. Update it to look like the phone we are trying to insert.
					phoneOld.ClinicNum=phoneSync.ClinicNum; //The clinic may have changed so set it to the new clinic.
					phoneOld.CountryCode=phoneSync.CountryCode;
					phoneOld.DateTimeActive=phoneSync.DateTimeActive;
					phoneOld.DateTimeInactive=phoneSync.DateTimeInactive;
					phoneOld.InactiveCode=phoneSync.InactiveCode;
					Update(phoneOld);
				}
				else { //This phone is new so insert it.
					Insert(phoneSync);
				}
				isChanged=true;
			}
			//Deal with phones which are in the local db but that do not occur in the HQ-supplied list.
			foreach(SmsPhone phoneNotFound in listPhonesDb.FindAll(x => !listPhonesSync.Any(y => y.PhoneNumber==x.PhoneNumber))) {
				//This phone not found at HQ so deactivate it.
				phoneNotFound.DateTimeInactive=DateTime.Now;
				phoneNotFound.InactiveCode="Phone not found at HQ";
				Update(phoneNotFound);
				isChanged=true;
			}
			return isChanged;
		}

		///<summary>Returns current clinic limit minus message usage for current calendar month.</summary>
		public static double GetClinicBalance(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),clinicNum);
			}
			double limit=0;
			if(!PrefC.HasClinicsEnabled) {
				if(PrefC.GetDate(PrefName.SmsContractDate).Year>1880) {
					limit=PrefC.GetDouble(PrefName.SmsMonthlyLimit);
				}
			}
			else { 
				if(clinicNum==0 && Clinics.GetCount(true) > 0) {//Sending text for "Unassigned" patient.  Use the first non-hidden clinic. (for now)
					clinicNum=Clinics.GetFirst(true).ClinicNum;
				}
				Clinic clinicCur=Clinics.GetClinic(clinicNum);
				if(clinicCur!=null && clinicCur.SmsContractDate.Year>1880) {
					limit=clinicCur.SmsMonthlyLimit;
				}
			}
			DateTime dtStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
			DateTime dtEnd=dtStart.AddMonths(1);
			string command="SELECT SUM(MsgChargeUSD) FROM smstomobile WHERE ClinicNum="+POut.Long(clinicNum)+" "
				+"AND DateTimeSent>="+POut.Date(dtStart)+" AND DateTimeSent<"+POut.Date(dtEnd);
			limit-=PIn.Double(Db.GetScalar(command));
			return limit;
		}

		///<summary>Returns true if texting is enabled for any clinics (including hidden), or if not using clinics, if it is enabled for the practice.</summary>
		public static bool IsIntegratedTextingEnabled() {
			//No need to check RemotingRole; no call to db.
			if(Plugins.HookMethod(null,"SmsPhones.IsIntegratedTextingEnabled_start")) {
				return true;
			}
			if(!PrefC.HasClinicsEnabled) {
				return PrefC.GetDateT(PrefName.SmsContractDate).Year>1880;
			}
			return (Clinics.GetFirstOrDefault(x => x.SmsContractDate.Year > 1880)!=null);
		}

		///<summary>Returns 0 if clinics not in use, or patient.ClinicNum if assigned to a clinic, or ClinicNum of the default texting clinic.</summary>
		public static long GetClinicNumForTexting(long patNum) {
			//No need to check RemotingRole; no call to db.
			if(!PrefC.HasClinicsEnabled || Clinics.GetCount()==0) {
				return 0;//0 used for no clinics
			}
			Clinic clinic=Clinics.GetClinic(Patients.GetPat(patNum).ClinicNum);//if patnum invalid will throw unhandled exception.
			if(clinic!=null) {//if pat assigned to invalid clinic or clinic num 0
				return clinic.ClinicNum;
			}
			return PrefC.GetLong(PrefName.TextingDefaultClinicNum);
		}

		///<summary>Returns true if there is an active phone for the country code.</summary>
		public static bool IsTextingForCountry(params string[] countryCodes) {
			if(countryCodes==null || countryCodes.Length==0) {
				return false;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),countryCodes);
			}
			string command = "SELECT COUNT(*) FROM smsphone WHERE CountryCode IN ("+string.Join(",",countryCodes.Select(x=>"'"+POut.String(x)+"'"))+") AND "+DbHelper.Year("DateTimeInactive")+"<1880";
			return Db.GetScalar(command)!="0";
		}

		/// <summary>This method calculates the number of messages will be sent. Given string of text to determine how many with a standard charset or unicode text.</summary>
		public static int CalculateMessagePartsNumber(string text)  {
			int countHeaderBytes=PrefC.GetInt(PrefName.BytesPerSmsHeader);
			string gsmChars=PrefC.GetString(PrefName.GsmCharSet);
			string gsmExtendedChars=PrefC.GetString(PrefName.GsmExtendedCharSet);
			double bytesPerMessagePart=PrefC.GetInt(PrefName.BytesPerSmsMessagePart);
			return CalculateMessageParts(text,countHeaderBytes,gsmChars,gsmExtendedChars,bytesPerMessagePart);
		}

		public static int CalculateMessageParts(string text,int countHeaderBytes,string gsmChars,string extendedChars,double countBytesPerMessagePart) {
			if(text.Length==0) {
				return 0;
			}
			double countBytesForWholeMessage=GetCountBytesForMessage(text,gsmChars,extendedChars);
			if(countBytesForWholeMessage<=countBytesPerMessagePart) {
				return 1;
			}
			else {
				countBytesPerMessagePart-=countHeaderBytes;
				if(countBytesPerMessagePart<=0) {//safe guard in case we try to divide by 0
					return 1;
				}
				double value=countBytesForWholeMessage/countBytesPerMessagePart;
				return (int)Math.Ceiling(value);
			}
		}

		public static double GetCountBytesForMessage(string text, string charsGsm, string charsGsmExtended) {
			MessageCharSet msgType=GetMessageCharSet(text,charsGsm,charsGsmExtended);
			double countBytesForMessage;
			if(msgType==MessageCharSet.Unicode) {
				countBytesForMessage=text.Length*2;
			}
			else {
				double bitsForMessage=0;
				for(int i=0;i<text.Length;i++) {
					if(charsGsm.Contains(text[i])) {
						bitsForMessage+=7;//standard gsm char set uses 7 bits
					}
					else if(charsGsmExtended.Contains(text[i])) {
						bitsForMessage+=14;//extended gsm char set uses 14 bits
					}
					else {
						bitsForMessage+=7;//Couldn't find the correct value but as this method is an estimate, we want to at least add one more character
					}
				}
				countBytesForMessage=Math.Ceiling(bitsForMessage/8.0);
			}
			
			return countBytesForMessage;
		}

		public static MessageCharSet GetMessageCharSet(string text,string charsGsm,string charsGsmExtended) {
			//text.length should already have been verified in CalculateMessageParts(...)
			for(int i=0;i<text.Length;i++) {
				if(!charsGsm.Contains(text[i]) && !charsGsmExtended.Contains(text[i])) {//we are not supported by GSM
					return MessageCharSet.Unicode;
				}
			}
			return MessageCharSet.Text;
		}

		public enum MessageCharSet {
			Text,//7-bit char set used in text messaging which represents the gsm char set
			Unicode,//16-bit char set used in text messaging
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<SmsPhone> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SmsPhone>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM smsvln WHERE PatNum = "+POut.Long(patNum);
			return Crud.SmsVlnCrud.SelectMany(command);
		}

		///<summary>Gets one SmsPhone from the db.</summary>
		public static SmsPhone GetOne(long smsVlnNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<SmsPhone>(MethodBase.GetCurrentMethod(),smsVlnNum);
			}
			return Crud.SmsVlnCrud.SelectOne(smsVlnNum);
		}

		///<summary></summary>
		public static void Delete(long smsVlnNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsVlnNum);
				return;
			}
			string command= "DELETE FROM smsvln WHERE SmsVlnNum = "+POut.Long(smsVlnNum);
			Db.NonQ(command);
		}
		*/
	}
}