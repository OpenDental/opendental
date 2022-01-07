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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSmsToMobiles);
				return;
			}
			Crud.SmsToMobileCrud.InsertMany(listSmsToMobiles);
		}

		#endregion
		
		///<summary></summary>
		public static void Update(SmsToMobile smsToMobile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsToMobile);
				return;
			}
			Crud.SmsToMobileCrud.Update(smsToMobile);
		}

		///<summary>Gets one SmsToMobile from the db.</summary>
		public static SmsToMobile GetMessageByGuid(string guid) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SmsToMobile>(MethodBase.GetCurrentMethod(),guid);
			}
			string command="SELECT * FROM smstomobile WHERE GuidMessage='"+guid+"'";
			return Crud.SmsToMobileCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(SmsToMobile smsToMobile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				smsToMobile.SmsToMobileNum=Meth.GetLong(MethodBase.GetCurrentMethod(),smsToMobile);
				return smsToMobile.SmsToMobileNum;
			}
			return Crud.SmsToMobileCrud.Insert(smsToMobile);
		}

		///<summary>Gets all SmsToMobile entries that have been inserted or updated since dateStart, which should be in server time.</summary>
		public static List<SmsToMobile> GetAllChangedSince(DateTime dateStart) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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

		///<summary>Convert a phone number to internation format and remove all punctuation. Validates input number format. Throws exceptions.</summary>
		public static string ConvertPhoneToInternational(string phoneRaw,string countryCodeLocalMachine,string countryCodeSmsPhone) {
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrWhiteSpace(phoneRaw)) {
				throw new Exception("Input phone number must be set");
			}
			bool isUSorCanada=(ListTools.In(countryCodeLocalMachine.ToUpper(),"US","CA") || ListTools.In(countryCodeSmsPhone.ToUpper(),"US","CA"));
			//Remove non-numeric.
			string ret=new string(phoneRaw.Where(x => char.IsDigit(x)).ToArray());			
			if(isUSorCanada && !IsShortCodeFormat(ret)) {
				if(!ret.StartsWith("1")) { //Add a "1" if US or Canada
					ret="1"+ret;
				}
				if(ret.Length!=11) {
					throw new Exception("Input phone number cannot be properly formatted for country code: "+countryCodeLocalMachine);
				}
			}			
			return ret;
		}

		///<summary>A 5 or 6 digit phone nubmer is likely a Short Code phone number.</summary>
		private static bool IsShortCodeFormat(string phoneRaw) {
			int length=phoneRaw.Length;
			return (length==5 || length==6);
		}
		
		///<summary>Surround with Try/Catch.  Sent as time sensitive message. Returns an instance of the new SmsToMobile row.</summary>
		public static SmsToMobile SendSmsSingle(long patNum,string wirelessPhone,string message,long clinicNum,SmsMessageSource smsMessageSource,bool makeCommLog=true,Userod user=null,bool canCheckBal=true) {
			//No need to check RemotingRole; no call to db.
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
			HandleSentSms(listSmsToMobiles,makeCommLog,user);
			smsToMobile=listSmsToMobiles.FirstOrDefault(x => x.GuidMessage==smsToMobile.GuidMessage);
			if(smsToMobile !=null && smsToMobile.SmsStatus==SmsDeliveryStatus.FailNoCharge) {
				throw new ODException(smsToMobile.CustErrorText);
			}
			return smsToMobile;
		}

		///<summary>Surround with try/catch. Returns true if all messages succeded, throws exception if it failed.</summary>
		public static List<SmsToMobile> SendSmsMany(List<SmsToMobile> listMessages,bool makeCommLog=true,Userod user=null,bool canCheckBal=true) {
			//No need to check RemotingRole; no call to db.
			if(listMessages==null || listMessages.Count==0) {
				return new List<SmsToMobile>();
			}
			if(canCheckBal) {
				foreach(long clinicNum in listMessages.Select(x => x.ClinicNum)) {
					double balance=SmsPhones.GetClinicBalance(clinicNum);
					if(balance-(CHARGE_PER_MSG*listMessages.Count(x => x.ClinicNum==clinicNum)) < 0) { 
						//ODException.ErrorCode 1 will be processed specially by caller.
						throw new ODException("To send these messages first increase spending limit for integrated texting from eServices Setup.",1);
					}
				}
			}
			listMessages=SendSms(listMessages);
			HandleSentSms(listMessages,makeCommLog,user);
			return listMessages;
		}

		///<summary>Inserts the SmsToMobile to the database and creates a commlog if necessary.</summary>
		private static void HandleSentSms(List<SmsToMobile> listSmsToMobiles,bool makeCommLog,Userod user) {
			//No need to check RemotingRole; no call to db.
			foreach(SmsToMobile smsToMobile in listSmsToMobiles) {
				smsToMobile.DateTimeSent=DateTime.Now;
				if(smsToMobile.PatNum!=0 && makeCommLog) {  //Patient specified and calling code won't make commlog, make it here.
					long userNum=0;
					if(user!=null) {
						userNum=user.UserNum;
					}
					if(smsToMobile.SmsStatus==SmsDeliveryStatus.FailNoCharge) {
						continue;
					}
					Commlogs.Insert(new Commlog() {
						CommDateTime=smsToMobile.DateTimeSent,
						Mode_=CommItemMode.Text,
						Note="Text message sent: "+smsToMobile.MsgText,
						PatNum=smsToMobile.PatNum,
						CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT),
						SentOrReceived=CommSentOrReceived.Sent,
						UserNum=userNum
					});
				}
			}
			InsertMany(listSmsToMobiles);
		}

		///<summary></summary>
		public static void Update(SmsToMobile smsToMobile,SmsToMobile oldSmsToMobile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsToMobile,oldSmsToMobile);
				return;
			}
			Crud.SmsToMobileCrud.Update(smsToMobile,oldSmsToMobile);
		}

		///<summary>Surround with try/catch. Returns list of SmsToMobiles that were sent, (some may have failed), throws exception if no messages sent. 
		///All Integrated Texting should use this method, CallFire texting does not use this method.</summary>
		public static List<SmsToMobile> SendSms(List<SmsToMobile> listMessages) {
			//No need to check RemotingRole; no call to db.
			if(Plugins.HookMethod(null,"SmsToMobiles.SendSms_start",listMessages)) {
				return new List<SmsToMobile>();
			}
			if(listMessages==null || listMessages.Count==0) {
				throw new Exception("No messages to send.");
			}
			System.Xml.Serialization.XmlSerializer xmlListSmsToMobileSerializer=new System.Xml.Serialization.XmlSerializer(typeof(List<SmsToMobile>));
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(true))){
				writer.WriteStartElement("Payload");
				writer.WriteStartElement("ListSmsToMobile");
				xmlListSmsToMobileSerializer.Serialize(writer,listMessages);
				writer.WriteEndElement(); //ListSmsToMobile	
				writer.WriteEndElement(); //Payload	
			}	
			string result = "";
			try {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.SmsSend(PayloadHelper.CreatePayload(strbuild.ToString(),eServiceCode.IntegratedTexting));
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw new Exception("Unable to send using web service.");
			}
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			XmlNode nodeError=doc.SelectSingleNode("//Error");
			XmlNode nodeSmsToMobiles=doc.SelectSingleNode("//ListSmsToMobile");
			if(nodeSmsToMobiles is null) {
				throw new Exception(nodeError?.InnerText??"Output node not found: ListSmsToMobile");
			}
			using XmlReader reader=XmlReader.Create(new System.IO.StringReader(nodeSmsToMobiles.InnerXml));
			listMessages=(List<SmsToMobile>)xmlListSmsToMobileSerializer.Deserialize(reader);			
			if(listMessages is null) { //List should always be there even if it's empty.
				throw new Exception(nodeError?.InnerText??"Output node not found: Error");
			}
			return listMessages;
		}
	}
}