using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Linq;
using CodeBase;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary>RESTful bridge to podium service. Without using REST Sharp or JSON libraries this code might not work properly.</summary>
	public class Podium {

		public static DateTime DateTimeLastRan=DateTime.MinValue;
		///<summary>Amount of time to wait inbetween trying to send Podium review invitations.</summary>
		public static int PodiumThreadIntervalMS=(int)TimeSpan.FromMinutes(5).TotalMilliseconds;
		///<summary>"Podium"</summary>
		public static string LOG_DIRECTORY_PODIUM="Podium";

		///<summary></summary>
		public Podium() {

		}

		///<summary>The main logic that sends Podium invitations.  Set isService true only when the calling method is the Open Dental Service.</summary>
		public static void ThreadPodiumSendInvitations(bool isService) {
			Program prog=Programs.GetCur(ProgramName.Podium);
			long programNum=prog.ProgramNum;
			//Consider blocking re-entrance if this hasn't finished.
			//Only send invitations if the program link is enabled here and at HQ, the computer name is set to this computer, and eConnector is not set to send invitations 
			if(!Programs.IsEnabledByHq(prog,out string _)
				|| !Programs.IsEnabled(ProgramName.Podium) 
				|| !ODEnvironment.IdIsThisComputer(ProgramProperties.GetPropVal(programNum,PropertyDescs.ComputerNameOrIP)) 
				|| ProgramProperties.GetPropVal(programNum,PropertyDescs.UseService)!=POut.Bool(isService)) 
			{ 
				return;
			}
			//Keep a consistant "Now" timestamp throughout this method.
			DateTime nowDT=MiscData.GetNowDateTime();
			if(Podium.DateTimeLastRan==DateTime.MinValue) {//First time running the thread.
				Podium.DateTimeLastRan=nowDT.AddMilliseconds(-PodiumThreadIntervalMS);
			}
			ReviewInvitationTrigger newPatTrigger=PIn.Enum<ReviewInvitationTrigger>(ProgramProperties.GetPropVal(programNum,PropertyDescs.NewPatientTriggerType));
			ReviewInvitationTrigger existingPatTrigger=PIn.Enum<ReviewInvitationTrigger>(ProgramProperties.GetPropVal(programNum,PropertyDescs.ExistingPatientTriggerType));
			List<Appointment> listNewPatAppts=GetAppointmentsToSendReview(newPatTrigger,programNum,true);
			foreach(Appointment apptCur in listNewPatAppts) {
				Podium.SendData(Patients.GetPat(apptCur.PatNum),apptCur.ClinicNum);
			}
			List<Appointment> listExistingPatAppts=GetAppointmentsToSendReview(existingPatTrigger,programNum,false);
			foreach(Appointment apptCur in listExistingPatAppts) {
				Podium.SendData(Patients.GetPat(apptCur.PatNum),apptCur.ClinicNum);
			}
			Podium.DateTimeLastRan=nowDT;
		}

		private static List<Appointment> GetAppointmentsToSendReview(ReviewInvitationTrigger trigger,long programNum,bool isNewPatient) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),trigger,programNum,isNewPatient);
			}
			string minsWaitComplete=ProgramProperties.GetPropVal(programNum,PropertyDescs.ApptSetCompletedMinutes);
			string minsWaitArriveOrDismiss=ProgramProperties.GetPropVal(programNum,PropertyDescs.ApptTimeDismissedMinutes);
			bool isArriveTrigger=false;
			if(trigger==ReviewInvitationTrigger.AppointmentTimeArrived) {
				minsWaitArriveOrDismiss=ProgramProperties.GetPropVal(programNum,PropertyDescs.ApptTimeArrivedMinutes);
				isArriveTrigger=true;
			}
			string command=$@"SELECT appointment.*
				FROM appointment
				LEFT JOIN commlog ON commlog.PatNum=appointment.PatNum
					AND commlog.CommSource={POut.Int((int)CommItemSource.ProgramLink)}
					AND DATE(commlog.DateTimeEnd)={DbHelper.Curdate()}
					AND commlog.ProgramNum={POut.Long(programNum)}
				WHERE ISNULL(commlog.PatNum)
				AND appointment.IsNewPatient={POut.Bool(isNewPatient)}
				AND appointment.AptDateTime BETWEEN {DbHelper.Curdate()} AND {DbHelper.Now()} + INTERVAL 1 HOUR";//Hard coded 1 hour to allow for appts that have an early DateTimeArrived
			if(trigger==ReviewInvitationTrigger.AppointmentCompleted) {
				command+=$@"
				AND appointment.AptStatus={POut.Int((int)ApptStatus.Complete)}
				AND EXISTS (
					SELECT 1 FROM histappointment
					WHERE histappointment.AptNum=appointment.AptNum
					AND histappointment.AptStatus={POut.Int((int)ApptStatus.Complete)}
					AND NOT EXISTS (
						SELECT 1 FROM histappointment h2
						WHERE h2.AptNum=histappointment.AptNum
						AND h2.AptStatus!={POut.Int((int)ApptStatus.Complete)}
						AND h2.HistDateTStamp>histappointment.HistDateTStamp
					)
					HAVING MIN(histappointment.HistDateTStamp)<={DbHelper.Now()} - INTERVAL {minsWaitComplete} MINUTE
				)";
			}
			else {//trigger==AppointmentTimeArrived or AppointmentTimeDismissed
				command+=$@"
				AND appointment.AptStatus IN ({POut.Int((int)ApptStatus.Scheduled)},{POut.Int((int)ApptStatus.Complete)})
				AND (
					(
						appointment.AptStatus={POut.Int((int)ApptStatus.Complete)}
						AND EXISTS (
							SELECT 1 FROM histappointment
							WHERE histappointment.AptNum=appointment.AptNum
							AND histappointment.AptStatus={POut.Int((int)ApptStatus.Complete)}
							AND NOT EXISTS (
								SELECT 1 FROM histappointment h2
								WHERE h2.AptNum=histappointment.AptNum
								AND h2.AptStatus!={POut.Int((int)ApptStatus.Complete)}
								AND h2.HistDateTStamp>histappointment.HistDateTStamp
							)
							HAVING MIN(histappointment.HistDateTStamp)<={DbHelper.Now()} - INTERVAL {(isArriveTrigger?minsWaitComplete:"90")} MINUTE
						)
					)
					OR (
						appointment.{(isArriveTrigger?"DateTimeArrived":"DateTimeDismissed")}>{DbHelper.Curdate()}
						AND appointment.{(isArriveTrigger?"DateTimeArrived":"DateTimeDismissed")}<={DbHelper.Now()} - INTERVAL {minsWaitArriveOrDismiss} MINUTE
					)
				)";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Throws exceptions.</summary>
		public static void ShowPage() {
			try {
				if(Programs.IsEnabled(ProgramName.Podium)) {
					Process.Start("http://www.opendental.com/resources/redirects/redirectpodiumdashboard.html");
				}
				else {
					Process.Start("http://www.opendental.com/resources/redirects/redirectpodiumod.html");
				}
			}
			catch {
				throw new Exception(Lans.g("Podium","Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet and then try again."));
			}
		}

		///<summary>Tries each of the phone numbers provided in the list one at a time until it succeeds.</summary>
		public static bool SendData(Patient pat,long clinicNum) {
			string locationId=ProgramProperties.GetPropValForClinicOrDefault(Programs.GetProgramNum(ProgramName.Podium),PropertyDescs.LocationID,clinicNum);
			if(string.IsNullOrEmpty(locationId)) {
				return false;
			}
			List<string> listPhoneNumbers=new List<string>() { pat.WirelessPhone,pat.HmPhone };
			int statusCode=-100;	//Set default to a failure, negative because http status codes are 1xx-5xx
			string apiUrl="https://api.podium.com/api/v3/switchboard_invitations";
			string apiToken=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.Podium),PropertyDescs.APIToken);
			if(pat.TxtMsgOk==YN.No || (pat.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo))) {//Don't text
				//Try to use email
				statusCode=MakeWebCall(apiToken,apiUrl,locationId,"",pat);
				MakeCommlog(pat,"",statusCode);
				return statusCode.Between(200,299);
			}
			//Try all phone numbers for this patient since they allow texting
			for(int i=0;i<listPhoneNumbers.Count;i++) {
				string phoneNumber=new string(listPhoneNumbers[i].Where(x => char.IsDigit(x)).ToArray());
				if(phoneNumber=="") {
					continue;
				}
				statusCode=MakeWebCall(apiToken,apiUrl,locationId,phoneNumber,pat);
				if(statusCode.Between(200,299)) {
					MakeCommlog(pat,phoneNumber,statusCode);
					return true;
				}
				else {
					//If the status code was an error for the current number, we will attempt to use other numbers the patient may have entered.
					//We will only make an error commlog and return false if all numbers tried returned an error.
				}
			}
			MakeCommlog(pat,"",statusCode);
			//explicitly failed or did not succeed.
			return false;
		}

		///<summary>Returns the status code of the web call made to Podium.  
		///200-299 indicates a successful response, >=300 indicates a Podium error response, negative status codes indicate an Open Dental error.</summary>
		private static int MakeWebCall(string apiToken,string apiUrl,string locationId,string phoneNumber,Patient pat) 
		{
			int retVal=-100;
			if(pat.TxtMsgOk==YN.No || (pat.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo))) {//Use email.
				retVal=-200;//Our code for 'no email address'
				phoneNumber="";//Ensure that there is no way we send a phone number to Podium if the patient doesn't approve text messages.
				if(string.IsNullOrEmpty(pat.Email)) {
					return retVal;//Will be -200, meaning no email and can't text.
				}
			}
			if(string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrWhiteSpace(pat.Email)) {
				return retVal;
			}
			//We either have a phoneNumber or email (or both), so send it to Podium.
			string isTestString="false";
			if(ODBuild.IsDebug()) {
				isTestString="true";
			}
			try {
				using(WebClientEx client=new WebClientEx()) {
					client.Headers[HttpRequestHeader.Accept]="application/json";
					client.Headers[HttpRequestHeader.ContentType]="application/json";
					client.Headers[HttpRequestHeader.Authorization]="Token token=\""+apiToken+"\"";
					client.Encoding=UnicodeEncoding.UTF8;
					string bodyJson=string.Format(@"
							{{
								""locationId"": ""{0}"",
								""lastName"": ""{3}"",
								""firstName"": ""{2}"",
								""email"": ""{4}"",
								""phoneNumber"": ""{1}"",
								""integrationName"": ""opendental"",
								""test"": {5}
							}}",locationId,phoneNumber,pat.FName,pat.LName,pat.Email,isTestString);
					//Post with Authorization headers and a body comprised of a JSON serialized anonymous type.
					client.UploadString(apiUrl,"POST",bodyJson);
					retVal=(int)(client.StatusCode);
				}
			}
			catch(WebException we) {
				if(we.Response.GetType()==typeof(HttpWebResponse)) {
					retVal=(int)((HttpWebResponse)we.Response).StatusCode;
				}
			}
			catch(Exception) {
				//Do nothing because a verbose commlog will be made below if all phone numbers fail.
			}
			return retVal;
			//Sample Request:

			//Accept: 'application/json's
			//Content-Type: 'application/json'
			//Authorization: 'Token token="my_dummy_token"'
			//Body:
			//{
			//	"location_id": "54321",
			//	"phone_number": "1234567890",
			//	"customer": {
			//		"first_name": "Johnny",
			//		"last_name": "Appleseed",
			//		"email": "johnny.appleseed@gmail.com"
			//	},
			//	"test": true
			//}
			//NOTE:  There will never be a value after "customer": although it was initially interpreted that there would be a "new" flag there.
		}

		private static void MakeCommlog(Patient pat,string phoneNumber,int statusCode) {
			string commText="";
			//Status code meanings:
			//		-100: Patient had no phone number
			//		-200: Patient can't text and had no email
			//		2XX: Successfully sent message
			//		422: Message has already been sent for patient
			//		Anything else: Failure of some sort.
			switch(statusCode/100) {	//Get general http status codes e.g. -100=-1, 203=2
				case -1:	//Failure, no phone number
					commText=Lans.g("Podium","Podium review invitation request failed because there was no phone number.  Error code:")+" "+statusCode;
					break;
				case -2:	//Failure, no email
					commText=Lans.g("Podium","Podium review invitation request failed because the patient doesn't accept texts "
						+"and there was no email address.  Error code:")+" "+statusCode;
					break;
				case 2:	//Success https://httpstatusdogs.com/200-ok
					commText=Lans.g("Podium","Podium review invitation request successfully sent.");
					break;
				case 4:	//Client side communication failure https://httpstatusdogs.com/400-bad-request
					if(statusCode==422) {//422 is Unprocessable Entity, which is sent in this case when a phone number has received an invite already.
						commText=Lans.g("Podium","The request failed because an identical request was previously sent.");
					}
					else {
						commText=Lans.g("Podium","The request failed to reach Podium with error code:")+" "+statusCode;
					}
					break;
				case 5:	//Server side internal failure. https://httpstatusdogs.com/500-internal-server-error
					commText=Lans.g("Podium","The request was rejected by the Podium server with error code:")+" "+statusCode;
					break;
				default:	//General Failure 
					commText=Lans.g("Podium","The request failed to send with error code:")+" "+statusCode;
					break;
			}
			if(!string.IsNullOrEmpty(commText)) {
				commText+="\r\n";
			}
			commText+=Lans.g("Podium","The information sent in the request was")+": \r\n"
				+Lans.g("Podium","First name")+": \""+pat.FName+"\", "
				+Lans.g("Podium","Last name")+": \""+pat.LName+"\", "
				+Lans.g("Podium","Email")+": \""+pat.Email+"\"";
			if(phoneNumber!="") {//If "successful".
				commText+=", "+Lans.g("Podium","Phone number")+": \""+phoneNumber+"\"";
			}
			else {
				string wirelessPhone=new string(pat.WirelessPhone.Where(x => char.IsDigit(x)).ToArray());
				string homePhone=new string(pat.HmPhone.Where(x => char.IsDigit(x)).ToArray());
				List<string> phonesTried=new List<string> { wirelessPhone,homePhone }.FindAll(x => x!="");
				string phoneNumbersTried=", "+Lans.g("Podium","No valid phone number found.");
				if(pat.TxtMsgOk==YN.No || (pat.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo))) {//Used email
					phoneNumbersTried="";
				}
				else if(phonesTried.Count>0) {
					phoneNumbersTried=", "+Lans.g("Podium","Phone numbers tried")+": "+string.Join(", ",phonesTried);
				}
				commText+=phoneNumbersTried;
			}
			long programNum=Programs.GetProgramNum(ProgramName.Podium);
			Commlog commlogCur=new Commlog();
			commlogCur.CommDateTime=DateTime.Now;
			commlogCur.DateTimeEnd=DateTime.Now;
			commlogCur.PatNum=pat.PatNum;
			commlogCur.UserNum=0;//run from server, no valid CurUser
			commlogCur.CommSource=CommItemSource.ProgramLink;
			commlogCur.ProgramNum=programNum;
			commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			commlogCur.Note=commText;
			commlogCur.Mode_=CommItemMode.Text;
			commlogCur.SentOrReceived=CommSentOrReceived.Sent;
			Commlogs.Insert(commlogCur);
		}

		private class WebClientEx:WebClient {
			//http://stackoverflow.com/questions/3574659/how-to-get-status-code-from-webclient
			private WebResponse _mResp = null;

			protected override WebResponse GetWebResponse(WebRequest req,IAsyncResult ar) {
				return _mResp = base.GetWebResponse(req,ar);
			}

			public HttpStatusCode StatusCode {
				get {
					HttpWebResponse httpWebResponse=_mResp as HttpWebResponse;
					return httpWebResponse!=null?httpWebResponse.StatusCode:HttpStatusCode.OK;
				}
			}
		}

		public class PropertyDescs {
			public static string ComputerNameOrIP="Enter your computer name or IP (required)";
			public static string APIToken="Enter your API Token (required)";
			public static string LocationID="Enter your Location ID (required)";
			public static string UseService="Enter 0 to use Open Dental for sending review invitations, or 1 to use the Service";
			public static string ApptSetCompletedMinutes="Send after appointment completed (minutes)";
			public static string ApptTimeArrivedMinutes="Send after appointment time arrived (minutes)";
			public static string ApptTimeDismissedMinutes="Send after appointment time dismissed (minutes)";
			public static string ExistingPatientTriggerType="Existing patient trigger type";
			public static string NewPatientTriggerType="New patient trigger type";
			public static string DisableAdvertising="Disable Advertising";
			///<summary>This is used in the Chart and the Account. 
			///The string is an identifier for the database and should not be changed</summary>
			public static string ShowCommlogsInChartAndAccount="Show Commlogs In Chart";
		}
	}

	public enum ReviewInvitationTrigger {
		AppointmentCompleted,
		AppointmentTimeArrived,
		AppointmentTimeDismissed
	}
}







