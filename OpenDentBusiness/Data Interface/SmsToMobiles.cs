using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using System.Globalization;
using CodeBase;
using WebServiceSerializer;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SmsToMobiles{
		///<summary>The amount that is charged per outgoing text. The actual charge may be higher if the message contains multiple pages.</summary>
		public const double CHARGE_PER_MSG=0.04;

		#region Insert

		public static void InsertMany(List<SmsToMobile> listSmsToMobiles) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSmsToMobiles);
				return;
			}
			Crud.SmsToMobileCrud.InsertMany(listSmsToMobiles);
		}

		#endregion
		
		///<summary></summary>
		public static void Update(SmsToMobile smsToMobile) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsToMobile);
				return;
			}
			Crud.SmsToMobileCrud.Update(smsToMobile);
		}

		///<summary>Gets one SmsToMobile from the db.</summary>
		public static SmsToMobile GetMessageByGuid(string guid) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SmsToMobile>(MethodBase.GetCurrentMethod(),guid);
			}
			string command="SELECT * FROM smstomobile WHERE GuidMessage='"+guid+"'";
			return Crud.SmsToMobileCrud.SelectOne(command);
		}

		public static List<SmsToMobile> GetMessagesByGuid(List<string> listGuids) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SmsToMobile>>(MethodBase.GetCurrentMethod(),listGuids);
			}
			if(listGuids.IsNullOrEmpty()) {
				return new List<SmsToMobile>();
			}
			string command=$"SELECT * FROM smstomobile WHERE GuidMessage in ({string.Join(",",listGuids.Select(x=>"'"+POut.String(x)+"'"))})";
			return Crud.SmsToMobileCrud.SelectMany(command);
		}

		public static List<SmsToMobile> GetMessagesByPk(List<long> listSmsToMobileNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SmsToMobile>>(MethodBase.GetCurrentMethod(),listSmsToMobileNums);
			}
			if(listSmsToMobileNums.IsNullOrEmpty()) {
				return new List<SmsToMobile>();
			}
			string command=$"SELECT * FROM smstomobile WHERE SmsToMobileNum IN ({string.Join(",",listSmsToMobileNums.Select(x=>POut.Long(x)))})";
			return Crud.SmsToMobileCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(SmsToMobile smsToMobile) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				smsToMobile.SmsToMobileNum=Meth.GetLong(MethodBase.GetCurrentMethod(),smsToMobile);
				return smsToMobile.SmsToMobileNum;
			}
			return Crud.SmsToMobileCrud.Insert(smsToMobile);
		}

		///<summary>Gets all SmsToMobile entries that have been inserted or updated since dateStart, which should be in server time.</summary>
		public static List<SmsToMobile> GetAllChangedSince(DateTime dateStart) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SmsToMobile>>(MethodBase.GetCurrentMethod(),dateStart);
			}
			string command="SELECT * from smstomobile WHERE SecDateTEdit >= "+POut.DateT(dateStart);
			return Crud.SmsToMobileCrud.SelectMany(command);
		}

		///<summary>Gets all SMS messages for the specified filters.</summary>
		///<param name="dateStart">If dateStart is 01/01/0001, then no start date will be used.</param>
		///<param name="dateEnd">If dateEnd is 01/01/0001, then no end date will be used.</param>
		///<param name="listClinicNums">Will filter by clinic only if not empty and patNum is -1.</param>
		///<param name="patNum">If patNum is not -1, then only the messages for the specified patient will be returned, otherwise messages for all 
		///patients will be returned.</param>
		///<param name="phoneNumber">The phone number to search by. Should be just the digits, no formatting.</param>
		public static List<SmsToMobile> GetMessages(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,long patNum=-1,string phoneNumber="") {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SmsToMobile>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,patNum,phoneNumber);
			}
			List<string> listCommandFilters=new List<string>();
			if(dateStart>DateTime.MinValue) {
				listCommandFilters.Add(DbHelper.DtimeToDate("DateTimeSent")+">="+POut.Date(dateStart));
			}
			if(dateEnd>DateTime.MinValue) {
				listCommandFilters.Add(DbHelper.DtimeToDate("DateTimeSent")+"<="+POut.Date(dateEnd));
			}
			if(patNum==-1) {
				//Only limit clinic if not searching for a particular PatNum.
				if(listClinicNums.Count>0) {
					listCommandFilters.Add("ClinicNum IN ("+String.Join(",",listClinicNums.Select(x => POut.Long(x)))+")");
				}
			}
			else {
				listCommandFilters.Add($"PatNum = {patNum}");
			}
			if(!string.IsNullOrEmpty(phoneNumber)) {
				listCommandFilters.Add($"MobilePhoneNumber = '{POut.String(phoneNumber)}'");
			}
			string command="SELECT * FROM smstomobile";
			if(listCommandFilters.Count>0) {
				command+=" WHERE "+String.Join(" AND ",listCommandFilters);
			}
			return Crud.SmsToMobileCrud.SelectMany(command);
		}

		///<summary>Convert a phone number to international format and remove all punctuation. Validates input number format. Throws exceptions.</summary>
		public static string ConvertPhoneToInternational(string phoneRaw,string countryCodeLocalMachine,string countryCodeSmsPhone) {
			Meth.NoCheckMiddleTierRole();
			if(string.IsNullOrWhiteSpace(phoneRaw)) {
				throw new Exception("Input phone number must be set");
			}
			bool isUSorCanada=(countryCodeLocalMachine.ToUpper().In("US","CA") || countryCodeSmsPhone.ToUpper().In("US","CA"));
			//Remove non-numeric.
			string phoneRetVal=new string(phoneRaw.Where(x => char.IsDigit(x)).ToArray());
			if(!isUSorCanada || IsShortCodeFormat(phoneRetVal)) {
				return phoneRetVal;
			}
			if(!phoneRetVal.StartsWith("1")) { //Add a "1" if US or Canada
				phoneRetVal="1"+phoneRetVal;
			}
			if(phoneRetVal.Length!=11) {
				throw new Exception("Input phone number cannot be properly formatted for country code: "+countryCodeLocalMachine);
			}
			return phoneRetVal;
		}

		///<summary>A 5 or 6 digit phone nubmer is likely a Short Code phone number.</summary>
		private static bool IsShortCodeFormat(string phoneRaw) {
			int length=phoneRaw.Length;
			return (length==5 || length==6);
		}
		
		///<summary>Surround with Try/Catch.  Sent as time sensitive message. Returns an instance of the new SmsToMobile row.</summary>
		public static SmsToMobile SendSmsSingle(long patNum,string wirelessPhone,string message,long clinicNum,SmsMessageSource smsMessageSource,
			bool makeCommLog=true,Userod userod=null,bool canCheckBal=true) 
		{
			Meth.NoCheckMiddleTierRole();
			if(Plugins.HookMethod(null,"SmsToMobiles.SendSmsSingle_start",patNum,wirelessPhone,message,clinicNum)) {
				return null;
			}
			double balance=SmsPhones.GetClinicBalance(clinicNum);
			if(balance-CHARGE_PER_MSG<0 && canCheckBal) { //ODException.ErrorCode 1 will be processed specially by caller.
				throw new ODException("To send this message first increase spending limit for integrated texting from eServices Setup.",1);
			}
			string countryCodeLocal=CultureInfo.CurrentCulture.Name.Substring(CultureInfo.CurrentCulture.Name.Length-2);//Example "en-US"="US"
			string countryCodePhone=SmsPhones.GetFirstOrDefault(x => x.ClinicNum==clinicNum)?.CountryCode??"";
			SmsToMobile smsToMobile=new SmsToMobile();
			smsToMobile.ClinicNum=clinicNum;
			smsToMobile.GuidMessage=Guid.NewGuid().ToString();
			smsToMobile.GuidBatch=smsToMobile.GuidMessage;
			smsToMobile.IsTimeSensitive=true;
			smsToMobile.MobilePhoneNumber=ConvertPhoneToInternational(wirelessPhone,countryCodeLocal,countryCodePhone);
			smsToMobile.PatNum=patNum;
			smsToMobile.MsgText=message;
			smsToMobile.MsgType=smsMessageSource;
			List<SmsToMobile> listSmsToMobiles=SmsToMobiles.SendSms(ListTools.FromSingle(smsToMobile));//Can throw if failed
			HandleSentSms(listSmsToMobiles,makeCommLog,userod);
			smsToMobile=listSmsToMobiles.FirstOrDefault(x => x.GuidMessage==smsToMobile.GuidMessage);
			if(smsToMobile !=null && smsToMobile.SmsStatus==SmsDeliveryStatus.FailNoCharge) {
				throw new ODException(smsToMobile.CustErrorText);
			}
			return smsToMobile;
		}

		///<summary>Surround with try/catch. Returns true if all messages succeded, throws exception if it failed.</summary>
		public static List<SmsToMobile> SendSmsMany(List<SmsToMobile> listSmsToMobilesMessages,bool makeCommLog=true,Userod userod=null,bool canCheckBal=true) {
			Meth.NoCheckMiddleTierRole();
			if(listSmsToMobilesMessages==null || listSmsToMobilesMessages.Count==0) {
				return new List<SmsToMobile>();
			}
			if(canCheckBal) {
				List<long> listClinicNums=listSmsToMobilesMessages.Select(x => x.ClinicNum).ToList();
				for(int i=0;i<listClinicNums.Count;i++) {
					double balance=SmsPhones.GetClinicBalance(listClinicNums[i]);
					if(balance-(CHARGE_PER_MSG*listSmsToMobilesMessages.Count(x => x.ClinicNum==listClinicNums[i])) < 0) { 
						//ODException.ErrorCode 1 will be processed specially by caller.
						throw new ODException("To send these messages first increase spending limit for integrated texting from eServices Setup.",1);
					}
				}
			}
			listSmsToMobilesMessages=SendSms(listSmsToMobilesMessages);
			HandleSentSms(listSmsToMobilesMessages,makeCommLog,userod);
			return listSmsToMobilesMessages;
		}

		///<summary>Inserts the SmsToMobile to the database and creates a commlog if necessary.</summary>
		private static void HandleSentSms(List<SmsToMobile> listSmsToMobiles,bool makeCommLog,Userod userod) {
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listSmsToMobiles.Count;i++) {
				listSmsToMobiles[i].DateTimeSent=DateTime.Now;
				if(listSmsToMobiles[i].PatNum==0 || !makeCommLog) {
					continue;
				}
				//Patient specified and calling code won't make commlog, make it here.
				long userNum=0;
				if(userod!=null) {
					userNum=userod.UserNum;
				}
				if(listSmsToMobiles[i].SmsStatus==SmsDeliveryStatus.FailNoCharge) {
					continue;
				}
				Commlog commlog=new Commlog();
				commlog.CommDateTime=listSmsToMobiles[i].DateTimeSent;
				commlog.Mode_=CommItemMode.Text;
				commlog.Note="Text message sent: "+listSmsToMobiles[i].MsgText;
				commlog.PatNum=listSmsToMobiles[i].PatNum;
				commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT);
				commlog.SentOrReceived=CommSentOrReceived.Sent;
				commlog.UserNum=userNum;
				Commlogs.Insert(commlog);
			}
			InsertMany(listSmsToMobiles);
		}

		///<summary></summary>
		public static void Update(SmsToMobile smsToMobile,SmsToMobile smsToMobileOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsToMobile,smsToMobileOld);
				return;
			}
			Crud.SmsToMobileCrud.Update(smsToMobile,smsToMobileOld);
		}

		///<summary>Surround with try/catch. Returns list of SmsToMobiles that were sent, (some may have failed), throws exception if no messages sent. 
		///All Integrated Texting should use this method, CallFire texting does not use this method.</summary>
		public static List<SmsToMobile> SendSms(List<SmsToMobile> listSmsToMobileMessages) {
			Meth.NoCheckMiddleTierRole();
			if(Plugins.HookMethod(null,"SmsToMobiles.SendSms_start",listSmsToMobileMessages)) {
				return new List<SmsToMobile>();
			}
			if(listSmsToMobileMessages==null || listSmsToMobileMessages.Count==0) {
				throw new Exception("No messages to send.");
			}
			System.Xml.Serialization.XmlSerializer xmlListSmsToMobileSerializer=new System.Xml.Serialization.XmlSerializer(typeof(List<SmsToMobile>));
			StringBuilder stringBuilder=new StringBuilder();
			using XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,WebSerializer.CreateXmlWriterSettings(true));
			xmlWriter.WriteStartElement("Payload");
			xmlWriter.WriteStartElement("ListSmsToMobile");
			xmlListSmsToMobileSerializer.Serialize(xmlWriter,listSmsToMobileMessages);
			xmlWriter.WriteEndElement(); //ListSmsToMobile
			xmlWriter.WriteEndElement(); //Payload
			xmlWriter.Close();
			string result = "";
			try {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.SmsSend(PayloadHelper.CreatePayload(stringBuilder.ToString(),eServiceCode.IntegratedTexting));
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw new Exception("Unable to send using web service.");
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			XmlNode xmlNodeError=xmlDocument.SelectSingleNode("//Error");
			XmlNode xmlNodeSmsToMobiles=xmlDocument.SelectSingleNode("//ListSmsToMobile");
			if(xmlNodeSmsToMobiles is null) {
				throw new Exception(xmlNodeError?.InnerText??"Output node not found: ListSmsToMobile");
			}
			using XmlReader xmlReader=XmlReader.Create(new System.IO.StringReader(xmlNodeSmsToMobiles.InnerXml));
			listSmsToMobileMessages=(List<SmsToMobile>)xmlListSmsToMobileSerializer.Deserialize(xmlReader);			
			if(listSmsToMobileMessages is null) { //List should always be there even if it's empty.
				throw new Exception(xmlNodeError?.InnerText??"Output node not found: Error");
			}
			return listSmsToMobileMessages;
		}
	}
}