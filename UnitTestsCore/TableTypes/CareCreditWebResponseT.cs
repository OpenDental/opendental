using System;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CareCreditWebResponseT {
		///<summary></summary>
		public static CareCreditWebResponse CreateCareCreditWebResponse(long patNum,long clinicNum,long payNum=0,double amt=0,
			CareCreditWebStatus processingStatus=CareCreditWebStatus.Pending,DateTime dateTimeStatus=default(DateTime),string lastResponseError="",
			CareCreditServiceType serviceType=CareCreditServiceType.Prefill,CareCreditTransType transType=CareCreditTransType.None,DateTime dateTimeEntry=default(DateTime))
		{
			CareCreditWebResponse ccWebResponse=new CareCreditWebResponse();
			ccWebResponse.PatNum=patNum;
			ccWebResponse.PayNum=payNum;
			ccWebResponse.RefNumber="";
			ccWebResponse.Amount=amt;
			ccWebResponse.WebToken="";
			ccWebResponse.ProcessingStatus=processingStatus;
			DateTime dateTime=dateTimeStatus;
			if(dateTimeStatus==default(DateTime)) {
				dateTime=DateTime.Now;
			}
			if(ListTools.In(ccWebResponse.ProcessingStatus,CareCreditWebResponses.ListWebStatusPending)) {
				ccWebResponse.DateTimePending=dateTime;
			}
			else if(ListTools.In(ccWebResponse.ProcessingStatus,CareCreditWebResponses.ListWebStatusCompleted)) {
				ccWebResponse.DateTimeCompleted=dateTime;
			}
			else if(ListTools.In(ccWebResponse.ProcessingStatus,CareCreditWebResponses.ListWebStatusExpired)) {
				ccWebResponse.DateTimeExpired=dateTime;
			}
			else {//all other status
				ccWebResponse.DateTimeLastError=dateTime;
			}
			ccWebResponse.LastResponseStr=lastResponseError;
			ccWebResponse.ClinicNum=clinicNum;
			ccWebResponse.ServiceType=serviceType;
			ccWebResponse.TransType=transType;
			CareCreditWebResponses.Insert(ccWebResponse);
			if(dateTimeEntry!=default(DateTime)) {
				UpdateDateTimeEntry(ccWebResponse.CareCreditWebResponseNum,dateTimeEntry,status:ccWebResponse.ProcessingStatus);
			}
			return ccWebResponse;
		}

		///<summary>Deletes everything from the webresponse table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearCareCreditWebResponseTable() {
			string command="DELETE FROM carecreditwebresponse WHERE CareCreditWebResponseNum > 0";
			DataCore.NonQ(command);
		}

		public static void UpdateDateTimeEntry(long careCreditWebResponseNum,DateTime dateTimeEntry,CareCreditWebStatus status=CareCreditWebStatus.PreApproved) {
			string command=$"UPDATE carecreditwebresponse SET DateTimeEntry={POut.DateT(dateTimeEntry)},ProcessingStatus='{POut.String(status.ToString())}' "
				+$"WHERE CareCreditWebResponseNum={POut.Long(careCreditWebResponseNum)} ";
			DataCore.NonQ(command);
		}
	}
}
