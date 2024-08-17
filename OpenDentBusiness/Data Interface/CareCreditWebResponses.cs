using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Bridges;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CareCreditWebResponses {
		public const string MERCHANT_ID_PROG_PROPERTY="MerchantIds";
		#region Get Methods
		///<summary></summary>
		public static CareCreditWebResponse GetOne(long ccWebResponse) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CareCreditWebResponse>(MethodBase.GetCurrentMethod(),ccWebResponse);
			}
			return Crud.CareCreditWebResponseCrud.SelectOne(ccWebResponse);
		}

		///<summary></summary>
		public static CareCreditWebResponse GetOneByPayNum(long payNum) {
			if(payNum==0) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CareCreditWebResponse>(MethodBase.GetCurrentMethod(),payNum);
			}
			string command=$"SELECT * FROM carecreditwebresponse WHERE PayNum={POut.Long(payNum)}";
			return Crud.CareCreditWebResponseCrud.SelectOne(command);
		}

		///<summary></summary>
		public static CareCreditWebResponse GetByRefId(string refNum) {
			if(string.IsNullOrEmpty(refNum)) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CareCreditWebResponse>(MethodBase.GetCurrentMethod(),refNum);
			}
			string command=$"SELECT * FROM carecreditwebresponse WHERE RefNumber='{POut.String(refNum)}'";
			return Crud.CareCreditWebResponseCrud.SelectOne(command);
		}

		public static CareCreditWebResponse GetBySessionId(string sessionId) {
			if(string.IsNullOrEmpty(sessionId)) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CareCreditWebResponse>(MethodBase.GetCurrentMethod(),sessionId);
			}
			//Reusing WebToken column since web tokens no longer exist but sessionId is similar in that it is no longer useful after the user is finished with the response.
			string command=$"SELECT * FROM carecreditwebresponse WHERE WebToken='{POut.String(sessionId)}'";
			return Crud.CareCreditWebResponseCrud.SelectOne(command);
		}

		///<summary>Gets all responses with Pending or Created ProcessingStatus from the db.</summary>
		public static List<CareCreditWebResponse> GetAllPending() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CareCreditWebResponse>>(MethodBase.GetCurrentMethod());
			}
			return Crud.CareCreditWebResponseCrud.SelectMany("SELECT * FROM carecreditwebresponse "
				+$"WHERE ProcessingStatus IN ('{CareCreditWebStatus.Pending}','{CareCreditWebStatus.Created}')");
		}

		///<summary>Sets all expired batch requests to a pending status.</summary>
		public static void SetAllExpiredBatchesToPending() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command=$"UPDATE carecreditwebresponse SET ProcessingStatus='{CareCreditWebStatus.Pending}' "
				+$"WHERE ProcessingStatus='{CareCreditWebStatus.ExpiredBatch}' " +
				$"AND ServiceType='{CareCreditServiceType.Batch}'";
			Db.NonQ(command);
		}

		///<summary>Returns all responses with completed ProcessingStatus.</summary>
		public static List<CareCreditWebResponse> GetApprovedTransactions(List<long> listClinicNums,DateTime dateFrom,DateTime dateTo,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CareCreditWebResponse>>(MethodBase.GetCurrentMethod(),listClinicNums,dateFrom,dateTo,patNum);
			}
			string clinicFilter="";
			string patNumFilter="";
			if(!listClinicNums.IsNullOrEmpty()) {
				clinicFilter=$"AND ClinicNum IN({string.Join(",",listClinicNums.Select(x => POut.Long(x)))})";
			}
			if(patNum>0) {
				patNumFilter=$"AND PatNum={POut.Long(patNum)}";
			}
			string command="SELECT * FROM carecreditwebresponse "
				+$"WHERE {DbHelper.BetweenDates("DateTimeCompleted",dateFrom,dateTo)} "
				+"AND ProcessingStatus IN('"+CareCreditWebStatus.Completed.ToString()+"') "
				+$"AND ServiceType='{CareCreditServiceType.Prefill.ToString()}' "
				+$"AND PayNum!=0 "
				+$"{clinicFilter} "
				+$"{patNumFilter} ";
			return Crud.CareCreditWebResponseCrud.SelectMany(command);
		}

		///<summary>Returns all responses with completed ProcessingStatus.</summary>
		public static List<CareCreditWebResponse> GetBatchQSTransactions(List<long> listClinicNums,List<CareCreditWebStatus> listStatuses,DateTime dateFrom,
			DateTime dateTo,long patNum) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CareCreditWebResponse>>(MethodBase.GetCurrentMethod(),listClinicNums,listStatuses,dateFrom,dateTo,patNum);
			}
			string clinicFilter="";
			string patNumFilter="";
			if(!listClinicNums.IsNullOrEmpty()) {
				clinicFilter=$"AND ClinicNum IN({string.Join(",",listClinicNums.Select(x => POut.Long(x)))})";
			}
			if(patNum>0) {
				patNumFilter=$"AND PatNum={POut.Long(patNum)}";
			}
			string processStatusStr="";
			if(listStatuses.IsNullOrEmpty()) {
				listStatuses=ListQSBatchTransactions;
			}
			for(int i=0;i<listStatuses.Count;i++) {
				if(i > 0) {
					processStatusStr+=" OR ";
				}
				CareCreditWebStatus processingStatus=listStatuses[i];
				processStatusStr+=$"(ProcessingStatus='{processingStatus}' AND {DbHelper.BetweenDates(GetDateTimeColumnForStatus(processingStatus),dateFrom,dateTo)}) ";
			}
			string command="SELECT * FROM carecreditwebresponse "
				+$"WHERE ServiceType='{CareCreditServiceType.Batch}' "
				+$"{clinicFilter} "
				+$"{patNumFilter} "
				+$"AND ({processStatusStr}) ";
			return Crud.CareCreditWebResponseCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<CareCreditWebResponse> GetBatchErrors(List<long> listClinicNums,DateTime dateFrom,DateTime dateTo,bool doIncludeAckErrors,
			long patNum=0) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CareCreditWebResponse>>(MethodBase.GetCurrentMethod(),listClinicNums,dateFrom,dateTo,doIncludeAckErrors,patNum);
			}
			string clinicFilter="";
			string patNumFilter="";
			string includeAckError="";
			if(!listClinicNums.IsNullOrEmpty()) {
				clinicFilter=$"AND ClinicNum IN({string.Join(",",listClinicNums.Select(x => POut.Long(x)))}) ";
			}
			if(patNum>0) {
				patNumFilter=$"AND PatNum={POut.Long(patNum)} ";
			}
			if(doIncludeAckErrors) {
				includeAckError=$"UNION ALL SELECT * FROM carecreditwebresponse WHERE ProcessingStatus='{CareCreditWebStatus.ErrorAcknowledged}' " 
					+$"AND {DbHelper.BetweenDates("DateTimeCompleted",dateFrom,dateTo)}  {clinicFilter}";
			}
			string command="SELECT ccw.* " 
				+"FROM carecreditwebresponse ccw "
				+"INNER JOIN ("
					+"SELECT PatNum,MAX(DateTimeLastError) DateTimeLastError "
					+"FROM carecreditwebresponse "
					+$"WHERE ServiceType='{CareCreditServiceType.Batch}' "
					+$"AND {DbHelper.BetweenDates("DateTimeLastError",dateFrom,dateTo)} "
					+$"AND ProcessingStatus='{CareCreditWebStatus.BatchError}' "
					+$"{clinicFilter} "
					+$"{patNumFilter} "
				+$"GROUP BY PatNum) groupccw ON groupccw.PatNum=ccw.PatNum AND groupccw.DateTimeLastError=ccw.DateTimeLastError "
				+$"AND ccw.ProcessingStatus='{CareCreditWebStatus.BatchError}' {includeAckError} ";
			List<CareCreditWebResponse> listWebResponses=Crud.CareCreditWebResponseCrud.SelectMany(command);
			command="SELECT PatNum " 
				+"FROM carecreditwebresponse "
				+$"WHERE ProcessingStatus IN({string.Join(",",ListWebStatusCompleted.Select(x => "'"+x.ToString()+"'").ToList())}) "
				+$"AND {DbHelper.BetweenDates("DateTimeCompleted",dateFrom,dateTo)} {clinicFilter} {patNumFilter}";
			List<long> listPatNumToExclude=Db.GetListLong(command);
			return listWebResponses.Where(x => !listPatNumToExclude.Contains(x.PatNum)).ToList();
		}

		///<summary></summary>
		public static List<CareCreditWebResponse> GetAllPatNum(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CareCreditWebResponse>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT * FROM carecreditwebresponse WHERE PatNum={POut.Long(patNum)}";
			return Crud.CareCreditWebResponseCrud.SelectMany(command);
		}

		///<summary>Returns a distinct list of PatNums that should be excluded from running a batch request. Patient that should be excluded have
		///some sort of pending transaction or have been confirmed a CareCredit cardholder or declinded as a cardholder. Uses DateTimeEntry.</summary>
		public static List<long> GetPatNumsForDateRangeToExclude(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string command="SELECT PatNum FROM carecreditwebresponse "
				+$"WHERE (ProcessingStatus IN({string.Join(",",GetExclusionStatuses().Select(x => "'"+x.ToString()+"'").ToList())}) "
				+$"OR (ProcessingStatus IN('{CareCreditWebStatus.Created.ToString()}','{CareCreditWebStatus.Pending.ToString()}') AND ServiceType='{CareCreditServiceType.Batch}')) "
				+$"AND {DbHelper.BetweenDates("DateTimeEntry",dateFrom,dateTo)}";
			List<long> listPatNumsToExclude=Db.GetListLong(command);
			//Exclude Batch errors quickscreens that occurred during date range passed in.
			//Also exclude Invalid Input Errors and responses with closed merchant nums that match any in the DB.
			//We have to run a second query to get Batch Errors in the date range so we can use c# to evalute the LastResponseStr column 
			//The LastResponseStr contains a  JSON object and we only want to exclude patients with Invalid Input errors.
			List<CareCreditWebResponse> listBatchErrors=GetBatchErrors(null,dateFrom,dateTo,false);
			List<string> listMerchantNumsAllDb=CareCredit.GetMerchantNumsAll();
			for(int i=0;i<listBatchErrors.Count;i++) {
				CareCreditQSBatchResponse careCreditQSBatch=CareCreditQSBatchResponse.From(listBatchErrors[i]);
				if(careCreditQSBatch==null) {
					continue;
				}
				if(careCreditQSBatch.IsInvalidInput 
					|| CareCredit.IsMerchantNumClosed(listBatchErrors[i].MerchantNumber,listMerchantNumsAllDb))
				{
					listPatNumsToExclude.Add(listBatchErrors[i].PatNum);
				}
			}
			return listPatNumsToExclude.Distinct().ToList();
		}

		///<summary>Returns the original completed or expired batch CareCredtWebResonse for the patnum in the last 60 Days. If the patient is a minor, it will use the patient's guarantor.</summary>
		public static CareCreditWebResponse GetOriginalCompletedBatchForPatNum(Patient pat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CareCreditWebResponse>(MethodBase.GetCurrentMethod(),pat);
			}
			long patNum=pat.PatNum;
			if(CareCredit.IsPatAMinor(pat)) {
				//If the patient is a minor, look for the guarantor. 
				patNum=pat.Guarantor;
			}
			List<CareCreditWebStatus> listCompletedStatuses=ListWebStatusCompleted;
			listCompletedStatuses.RemoveAll(x => x==CareCreditWebStatus.DupQS);
			listCompletedStatuses.Add(CareCreditWebStatus.ExpiredBatch);
			string command="SELECT * FROM carecreditwebresponse "
				+$"WHERE PatNum={POut.Long(patNum)} "
				+$"AND ServiceType='{CareCreditServiceType.Batch}' "
				+$"AND ProcessingStatus IN({string.Join(",",listCompletedStatuses.Select(x => $"'{x}'"))}) "
				+"ORDER BY DateTimeEntry DESC "
				+"LIMIT 1" ;
			return Crud.CareCreditWebResponseCrud.SelectOne(command);
		}

		///<summary>Returns up to 50 items that need to be logged to ODHQ.</summary>
		public static List<CareCreditWebResponse> GetPendingForLogging() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CareCreditWebResponse>>(MethodBase.GetCurrentMethod());
			}
			List<CareCreditWebStatus> listCareCreditWebStatusToLog=ListWebStatusCompleted;
			listCareCreditWebStatusToLog.Add(CareCreditWebStatus.ExpiredBatch);
			string command=$"SELECT * "
				+$"FROM carecreditwebresponse "
				+$"WHERE HasLogged={POut.Bool(false)} "
				+$"AND ProcessingStatus IN({string.Join(",",listCareCreditWebStatusToLog.Select(x => "'"+x.ToString()+"'").ToList())}) "
				+$"LIMIT 50";
			return Crud.CareCreditWebResponseCrud.SelectMany(command);
		}

		///<summary>Gets the DateTime column name for the status passed in.</summary>
		private static string GetDateTimeColumnForStatus(CareCreditWebStatus status) {
			//No remoting role check; no call to db
			if(ListWebStatusAcknowledge.Contains(status) || ListWebStatusCompleted.Contains(status)) {
				return nameof(CareCreditWebResponse.DateTimeCompleted);
			}
			else if(ListWebStatusError.Contains(status)) {
				return nameof(CareCreditWebResponse.DateTimeLastError);
			}
			else if(ListWebStatusExpired.Contains(status)) {
				return nameof(CareCreditWebResponse.DateTimeExpired);
			}
			else {
				return nameof(CareCreditWebResponse.DateTimePending);
			}
		}
		#endregion Get Methods

		#region Insert
		///<summary></summary>
		public static long Insert(CareCreditWebResponse ccWebResponse) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				ccWebResponse.CareCreditWebResponseNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ccWebResponse);
				return ccWebResponse.CareCreditWebResponseNum;
			}
			return Crud.CareCreditWebResponseCrud.Insert(ccWebResponse);
		}
		#endregion Insert

		#region Update
		///<summary></summary>
		public static void Update(CareCreditWebResponse ccWebResponse) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ccWebResponse);
				return;
			}
			Crud.CareCreditWebResponseCrud.Update(ccWebResponse);
		}

		public static bool Update(CareCreditWebResponse careCreditWebResponse,CareCreditWebResponse careCreditWebResponseOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),careCreditWebResponse,careCreditWebResponseOld);
			}
			return Crud.CareCreditWebResponseCrud.Update(careCreditWebResponse,careCreditWebResponseOld);
		}

		///<summary>Updates the HasLogged column to true for all CareCreditWebResponseNums passed in.</summary>
		public static void UpdateHasLoggedToTrue(List<long> listCareCreditWebResponseNums) {
			if(listCareCreditWebResponseNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCareCreditWebResponseNums);
				return;
			}
			string command=$"UPDATE carecreditwebresponse SET HasLogged={POut.Bool(true)} "
				+$"WHERE CareCreditWebResponseNum IN ({string.Join(",",listCareCreditWebResponseNums.Select(x => POut.Long(x)).ToList())})";
			Db.NonQ(command);
		}

		public static void UpdateProcessingWebStatus(CareCreditWebResponse webResponse,CareCreditWebStatus status) {
			//No remoting role check; no call to db
			UpdateProcessingWebStatus(new List<CareCreditWebResponse>() { webResponse },status);
		}

		///<summary>Updates the CareCreditWebStatus to expired CareCreditWebResponses passed in.</summary>
		public static void UpdateProcessingWebStatus(List<CareCreditWebResponse> listCareCreditWebResponses,CareCreditWebStatus status) {
			//No remoting role check; no call to db
			if(listCareCreditWebResponses.IsNullOrEmpty()) {
				return;
			}
			foreach(CareCreditWebResponse webRes in listCareCreditWebResponses) {
				webRes.ProcessingStatus=status;
				DateTime dateTimeNow=MiscData.GetNowDateTime();
				if(ListWebStatusAcknowledge.Contains(status) || ListWebStatusCompleted.Contains(status)) {
					webRes.DateTimeCompleted=dateTimeNow;
				}
				else if(ListWebStatusError.Contains(status)) {
					webRes.DateTimeLastError=dateTimeNow;
				}
				else if(ListWebStatusExpired.Contains(status)) {
					webRes.DateTimeExpired=dateTimeNow;
				}
				else {
					webRes.DateTimePending=dateTimeNow;
				}
				Update(webRes);
			}
		}
		#endregion Update

		#region Misc Methods
		///<summary>Sets the PayNum=0 for the CareCreditWebResponseNum.</summary>
		public static void ClearPayment(long careCreditWebResponseNum) {
			if(careCreditWebResponseNum==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),careCreditWebResponseNum);
				return;
			}
			string command=$"UPDATE carecreditwebresponse SET PayNum=0 WHERE CareCreditWebResponseNum={POut.Long(careCreditWebResponseNum)}";
			Db.NonQ(command);
		}

		///<summary>Returns a list of CareCreditWebStatuses that we consider acknowledge or unknown. We update their DateTimeCompleted field when acknowledge.</summary>
		public static List<CareCreditWebStatus> ListWebStatusAcknowledge => new List<CareCreditWebStatus>() { CareCreditWebStatus.ErrorAcknowledged,
			CareCreditWebStatus.Unknown };

		///<summary>Returns a list of CareCreditWebStatuses that we consider completed. We update their DateTimeCompleted field when completed.</summary>
		public static List<CareCreditWebStatus> ListWebStatusCompleted => new List<CareCreditWebStatus>() { CareCreditWebStatus.AccountFound,
			CareCreditWebStatus.CallForAuth,CareCreditWebStatus.Completed,CareCreditWebStatus.Declined,CareCreditWebStatus.DupQS,
			CareCreditWebStatus.PreApproved };

		///<summary>Returns a list of CareCreditWebStatuses that we consider had errors at some point. We update their DateTimeLastError field when failure.</summary>
		public static List<CareCreditWebStatus> ListWebStatusError => new List<CareCreditWebStatus>() {   CareCreditWebStatus.BatchError,
			CareCreditWebStatus.Cancelled,CareCreditWebStatus.CreatedError,CareCreditWebStatus.PendingError, CareCreditWebStatus.UnknownError };

		///<summary>Returns a list of CareCreditWebStatuses that we consider expired. We update their DateTimeExpired field when expired.</summary>
		public static List<CareCreditWebStatus> ListWebStatusExpired => new List<CareCreditWebStatus>() { CareCreditWebStatus.Expired,
			CareCreditWebStatus.ExpiredBatch };

		///<summary>Returns a list of CareCreditWebStatuses that we consider pending. We updated their DateTimePending field.</summary>
		public static List<CareCreditWebStatus> ListWebStatusPending => new List<CareCreditWebStatus>() { CareCreditWebStatus.Created,
			CareCreditWebStatus.Pending };

		///<summary>Returns a list of CareCreditWebStatuses that we consider quickscreen batch transactions (with the exception of MerchantClosed). We updated their DateTimePending field.</summary>
		public static List<CareCreditWebStatus> ListQSBatchTransactions => new List<CareCreditWebStatus>() { CareCreditWebStatus.Created,
			CareCreditWebStatus.Pending,CareCreditWebStatus.AccountFound,CareCreditWebStatus.PreApproved,CareCreditWebStatus.Declined,CareCreditWebStatus.ExpiredBatch, };

		///<summary></summary>
		public static List<CareCreditWebStatus> GetExclusionStatuses() {
			//No remoting role check; no call to db
			List<CareCreditWebStatus> listExclusionStatuses=ListWebStatusCompleted;
			//Expired batches means we already sent a request to CC. Exclude these patients.
			listExclusionStatuses.Add(CareCreditWebStatus.ExpiredBatch);
			return listExclusionStatuses;
		}

		///<summary>Handles the responses by setting correct error status.</summary>
		public static void HandleResponseError(CareCreditWebResponse ccWebResponse,string resStr) {
			//No remoting role check; no call to db
			ccWebResponse.LastResponseStr=resStr;
			if(ccWebResponse.ProcessingStatus==CareCreditWebStatus.Created) {
				ccWebResponse.ProcessingStatus=CareCreditWebStatus.CreatedError;
			}
			else if(ccWebResponse.ProcessingStatus==CareCreditWebStatus.Pending) {
				ccWebResponse.ProcessingStatus=CareCreditWebStatus.PendingError;
			}
			else {
				ccWebResponse.ProcessingStatus=CareCreditWebStatus.UnknownError;
			}
			ccWebResponse.DateTimeLastError=MiscData.GetNowDateTime();
		}

		public static void AddPatFieldsForMinors(long patNum) {
			PatFieldDef patFieldCC=PatFieldDefs.GetPatFieldCareCredit();
			if(patFieldCC==null) {
				return;
			}
			PatField patField=PatFields.GetByName(patFieldCC.FieldName,PatFields.Refresh(patNum));
			UpsertPatFieldForMinors(patNum,patField);
		}

		///<summary></summary>
		public static void UpdateCareCreditPatField(string status,long patNum,bool isBatch) {
			PatFieldDef patFieldCC=PatFieldDefs.GetPatFieldCareCredit();
			if(patFieldCC==null) {
				return;
			}
			PatField patField=PatFields.GetByName(patFieldCC.FieldName,PatFields.Refresh(patNum));
			if(patField==null) {
				patField=new PatField();
				patField.PatNum=patNum;
			}
			patField.FieldName=patFieldCC.FieldName;
			patField.FieldValue=status;
			PatFields.Upsert(patField);
			if(isBatch) {
				UpsertPatFieldForMinors(patNum,patField);
			}
		}

		private static void UpsertPatFieldForMinors(long patNum,PatField patField) {
			if(patField==null) {
				return;
			}
			List<long> listMinorPatNums=Patients.GetAllPatientsForGuarantor(patNum).FindAll(x => x.Birthdate.Year>1880 && x.Age<18).Select(x => x.PatNum).ToList();
			if(listMinorPatNums.IsNullOrEmpty()) {
				return;
			}
			foreach(long patNumMinor in listMinorPatNums) {
				PatField patFieldMinor=PatFields.GetByName(patField.FieldName,PatFields.Refresh(patNumMinor));
				if(patFieldMinor==null) {
					patFieldMinor=new PatField();
					patFieldMinor.PatNum=patNumMinor;
				}
				patFieldMinor.FieldName=patField.FieldName;
				patFieldMinor.FieldValue=patField.FieldValue;
				PatFields.Upsert(patFieldMinor);
			}
		}


		#endregion Misc Methods
	}
}