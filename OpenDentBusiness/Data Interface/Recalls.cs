using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace OpenDentBusiness {

	///<summary></summary>
	public class Recalls {
		#region Recall Sync All Patient Variables
		///<summary>Queue to hold batches for FIFO processing.  A batch is a dictionary of PatNum keys linked to PatBatchData objects, which hold the
		///pat's list of current recalls, dictionary of last proc dates for recall trigger procs, and dictionary of scheduled recall dates.  One thread
		///fills the queue with db data while the main thread processes the batches of pat data.  The queue will have at most two batches in it at any
		///given time.  If the queue contains 2 batches already, the filling thread will wait for the main thread to remove a batch from the front of the
		///queue before adding another batch to the rear of the queue.
		///Make sure to use _lockObjQueueBatchData when manipulating this queue.</summary>
		private static Queue<Dictionary<long,PatBatchData>> _queueBatchData;
		///<summary>Lock object to keep the queue thread safe.</summary>
		private static object _lockObjQueueBatchData=new object();
		///<summary>False until the filling thread has added the last batch of data to the queue.  Once true AND the queue is empty, the main thread is
		///finished as well.</summary>
		private static bool _isQueueBatchThreadDone;
		private static bool _isQueueBatchThreadDone2;
		///<summary>Number of PatNums the filling thread uses for each batch of data.  The processing takes longer than filling, so we can keep this
		///number relatively small to reduce total program memory consumption.</summary>
		private const int BATCH_SIZE=10000;
		private static int _totalPatCount;
		private static List<long> _listPatNumMaxPerGroup;
		///<summary>If this thread is not null then SynchAllPatients() is in the middle of running.</summary>
		private static ODThread _odThreadQueueData;
		#endregion Recall Sync All Patient Variables

		#region Test Variables
		///<summary>Should only be used for testing. Set to true to run all the actions of Web Sched Recall synchronously.</summary>
		public static bool RunWebSchedSynchronously;
		#endregion

		#region Get Methods
		///<summary>Returns a list of PatNums of patients with conflicting Recall type.
		///A conflicting recall type is when a patient is scheduled for a perio recall but their recall type is set to prophy and vice versa.
		///Only checks for Prophy and Perio recall types.</summary>
		public static List<long> GetConflictingPatNums(List<long> listPatNums) {
			//No need to check RemotingRole; no call to db.
			List<long> retVal=new List<long>();
			Dictionary<long,List<Procedure>> dictProcsOnFutureApts=new Dictionary<long, List<Procedure>>();
			List<RecallType> listRecallTypePractice=RecallTypes.GetActive().FindAll(x => RecallTypes.IsSpecialRecallType(x.RecallTypeNum));
			List<ProcedureCode> listRecallProcsPractice=ProcedureCodes.GetFromCommaDelimitedList(
				string.Join(",",listRecallTypePractice.Select(x=>x.Procedures).ToList().Union(RecallTypes.GetProcs(RecallTypes.ChildProphyType))));
			//List of practice recall TP procedures on future scheduled appointments for the listPatNums.
			List<Procedure> listProcsOnFutureApts=Procedures.GetProcsAttachedToFutureAppt(listPatNums,
				listRecallProcsPractice.Select(x => x.CodeNum).ToList());
			//Dictionary of PatNums and List of Procedures.
			dictProcsOnFutureApts=listProcsOnFutureApts.GroupBy(x => x.PatNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			//Check for conflicting recall types
			//A conflicting recall type is when a patient is scheduled for a perio recall procedure but their recall type is set to prophy and vice versa.
			//Only Checks for Prophy and Perio recall types.
			List<Recall> listRecalls=Recalls.GetList(listPatNums);
			List<Patient> listPats=Patients.GetMultPats(listProcsOnFutureApts.Select(x=>x.PatNum).ToList()).ToList();
			foreach(KeyValuePair<long,List<Procedure>> pat in dictProcsOnFutureApts) {
				List<ProcedureCode> listOppositeRecallProcs=new List<ProcedureCode>();
				List<Recall> listRecallPat=listRecalls.FindAll(x => x.PatNum==pat.Key);
				Patient patCur=listPats.FirstOrDefault(x=>x.PatNum==pat.Key);
				if(listRecallPat.Count==0 || patCur==null) {
					continue;
				}
				if(ListTools.In(RecallTypes.ProphyType,listRecallPat.Select(x => x.RecallTypeNum).ToList())) {
					//Patient has Prophy recall type set. Get procs for Perio recall type.
					listOppositeRecallProcs=ProcedureCodes.GetFromCommaDelimitedList(
						string.Join(",",RecallTypes.GetProcs(RecallTypes.PerioType)));
				}
				else {
					//Patient has Perio recall type set. Get procs for Prophy recall type.
					if(Recalls.IsChildRecall(patCur,pat.Value.Select(x=>x.ProcDate).ToList().Max())) {
						//Patients a child, check only ChildProphyType procedures.
						listOppositeRecallProcs=ProcedureCodes.GetFromCommaDelimitedList(string.Join(",",RecallTypes.GetProcs(RecallTypes.ChildProphyType)));
					}
					else {
						listOppositeRecallProcs=ProcedureCodes.GetFromCommaDelimitedList(string.Join(",",RecallTypes.GetProcs(RecallTypes.ProphyType)));
					}
				}
				List<long> listCodeNumsOppositeRecallType=listOppositeRecallProcs.Select(x=>x.CodeNum).ToList();
				List<long> listCodeNumsOnFutureApt=pat.Value.Select(x=>x.CodeNum).ToList();
				//Recall confliction exists when the patient's future scheduled TP procedures contain all of the procedures of the conflicting recall type.
				if(listCodeNumsOppositeRecallType.All(x => ListTools.In(x,listCodeNumsOnFutureApt))) {
					//PatNum has conflicting recall type. 
					retVal.Add(pat.Key);
				}
			}
			return retVal;
		}
		#endregion

		private const string WEB_SCHED_SIGN_UP_URL="http://www.patientviewer.com/WebSchedSignUp.html";

		///<summary>http://www.patientviewer.com/WebSchedSignUp.html</summary>
		public static string GetWebSchedPromoURL() {
			//No need to check RemotingRole; no call to db.
			return WEB_SCHED_SIGN_UP_URL;
		}

		///<summary>Gets all recalls for the supplied patients, usually a family or single pat.  Result might have a length of zero.  
		///Each recall will also have the DateScheduled filled by pulling that info from other tables.</summary>
		public static List<Recall> GetList(List<long> listPatNums) {
			if(listPatNums==null || listPatNums.Count<=0) {
				return new List<Recall>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Recall>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM recall WHERE recall.PatNum IN ("+string.Join(",",listPatNums)+")";
			return Crud.RecallCrud.SelectMany(command);
		}

		public static List<Recall> GetList(long patNum) {
			//No need to check RemotingRole; no call to db.
			List<long> patNums=new List<long>();
			patNums.Add(patNum);
			return GetList(patNums);
		}

		/// <summary></summary>
		public static List<Recall> GetList(List<Patient> patients){
			//No need to check RemotingRole; no call to db.
			List<long> patNums=new List<long>();
			for(int i=0;i<patients.Count;i++){
				patNums.Add(patients[i].PatNum);
			}
			return GetList(patNums);
		}

		public static Recall GetRecall(long recallNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Recall>(MethodBase.GetCurrentMethod(),recallNum);
			}
			return Crud.RecallCrud.SelectOne(recallNum);
		}

		///<summary>Gets a list of recalls that are past due for the patients passed in. A recall is considered past due if the the DateDue is before 
		///today and the DateScheduled is blank.</summary>
		public static List<Recall> GetPastDueForPats(DateTime dateStart,List<long> listPatNums) {
			if(listPatNums==null || listPatNums.Count<=0) {
				return new List<Recall>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Recall>>(MethodBase.GetCurrentMethod(),dateStart,listPatNums);
			}
			DateTime dateMin=new DateTime(1880,1,1);
			string command="SELECT * FROM recall "+
				"WHERE PatNum IN ("+string.Join(",",listPatNums)+") "+
				"AND DateDue<"+POut.Date(DateTime.Today)+" "+
				"AND DateDue>"+POut.Date(dateMin)+" "+
				$"AND (DateScheduled>{POut.DateT(dateStart)} OR DateScheduled<{POut.Date(dateMin)})";
			return Crud.RecallCrud.SelectMany(command);
		}

		///<summary>Will return a recall or null. Pass in a list of recalls for the patient to save a database call.</summary>
		public static Recall GetRecallProphyOrPerio(long patNum,bool excludeScheduled=false,List<Recall> listRecalls=null) {
			//No need to check RemotingRole; no call to db.
			listRecalls=listRecalls??GetList(patNum);
			return listRecalls.Where(x => x.PatNum==patNum)
				.Where(x => ListTools.In(x.RecallTypeNum,RecallTypes.ProphyType,RecallTypes.PerioType))
				.Where(x => excludeScheduled ? x.DateScheduled.Year < 1880 : true)
				.FirstOrDefault();
		}

		///<summary>Will return true if perio or prophy is scheduled. Pass in a list of recalls for the patient to save a database call.</summary>
		public static bool HasProphyOrPerioScheduled(long patNum,List<Recall> listRecalls=null) {
			//No need to check RemotingRole; no call to db.
			listRecalls=listRecalls??GetList(patNum);
			return listRecalls.Any(x => x.PatNum==patNum 
				&& ListTools.In(x.RecallTypeNum,RecallTypes.ProphyType,RecallTypes.PerioType) 
				&& x.DateScheduled.Year > 1880);
		}

		///<summary>Returns true if recall passed in is either a Perio or Prophy type.</summary>
		public static bool IsRecallProphyOrPerio(Recall recall) {
			//No need to check RemotingRole; no call to db.
			if(recall==null) {
				return false;
			}
			return ListTools.In(recall.RecallTypeNum,RecallTypes.ProphyType,RecallTypes.PerioType);
		}

		///<summary>Returns the recall time pattern for the patient and the specific recall passed in.
		///Loops through all recalls passed in and adds any due recall procedures to the time pattern if recallCur is a special recall type.
		///Set listRecalls to a list of all potential recalls for this patient that MIGHT need to be automatically scheduled for this current appointment.
		///Also, this method will manipulate listProcStrs if any additional procedures are added.</summary>
		public static string GetRecallTimePattern(Recall recallCur,List<Recall> listRecalls,Patient patCur,List<string> listProcStrs) {
			//No need to check RemotingRole; no call to db.  Also, listProcStrs is used like an "out" or "ref" parameter.
			List<RecallType> listRecallTypes=RecallTypes.GetDeepCopy();
			string recallPattern=RecallTypes.GetTimePattern(recallCur.RecallTypeNum);
			if(recallCur.TimePatternOverride!="") {
				recallPattern=recallCur.TimePatternOverride;
			}
			if(RecallTypes.IsSpecialRecallType(recallCur.RecallTypeNum) && IsChildRecall(patCur,recallCur.DateDue)) {
				//Loop through all recall types for a set up ChildProphyType.
				//If the office has a ChildProphyType set up, we will treat all
				//ChildProphyType and ProphyTypes for children as though they were a ChildProphyType
				for(int i=0;i<listRecallTypes.Count;i++) {
					if(listRecallTypes[i].RecallTypeNum==RecallTypes.ChildProphyType 
						&& (recallCur.RecallTypeNum == RecallTypes.ChildProphyType || recallCur.RecallTypeNum == RecallTypes.ProphyType)) 
					{
						List<string> childprocs=RecallTypes.GetProcs(listRecallTypes[i].RecallTypeNum);
						if(childprocs.Count>0) {
							listProcStrs.Clear();
							listProcStrs.AddRange(childprocs);//overrides adult procs.
						}
						string childpattern=RecallTypes.GetTimePattern(listRecallTypes[i].RecallTypeNum);
						if(childpattern!="" && recallCur.TimePatternOverride=="") {
							recallPattern=childpattern;//overrides adult pattern.
						}
					}
				}
			}
			List<string> listProcPatterns=new List<string>() { recallPattern };
			//Add films------------------------------------------------------------------------------------------------------
			if(RecallTypes.IsSpecialRecallType(recallCur.RecallTypeNum)) {//if this is a prophy or perio
				for(int i=0;i<listRecalls.Count;i++) {
					if(recallCur.RecallNum==listRecalls[i].RecallNum) {
						continue;//already handled.
					}
					if(listRecalls[i].IsDisabled) {
						continue;
					}
					if(listRecalls[i].DateDue.Year<1880) {
						continue;
					}
					if(listRecalls[i].DateDue>recallCur.DateDue//if film due date is after prophy due date
						&& listRecalls[i].DateDue>DateTime.Today)//and not overdue
					{
						continue;
					}
					//There is now a flag that users can set on their recall types that dictates if this recall type should be added to this special recall.
					RecallType recallType=listRecallTypes.FirstOrDefault(x => x.RecallTypeNum==listRecalls[i].RecallTypeNum);
					if(recallType==null || !recallType.AppendToSpecial) {
						continue;//Recall type not found or the off has flagged this "manual" recall type to NOT be automatically appended to special recalls.
					}
					listProcStrs.AddRange(RecallTypes.GetProcs(listRecalls[i].RecallTypeNum));
					if(listRecalls[i].TimePatternOverride=="") {
						listProcPatterns.Add(RecallTypes.GetTimePattern(listRecalls[i].RecallTypeNum));
					}
					else {
						listProcPatterns.Add(listRecalls[i].TimePatternOverride);
					}
				}
			}
			return Appointments.GetApptTimePatternFromProcPatterns(listProcPatterns);
		}

		///<summary> Checks the patient's birth date in regards to the age they will be when the recall is due. 
		///E.g. if pt's 12th birthday falls after recall date.</summary>
		public static bool IsChildRecall(Patient patCur,DateTime recallDue) {
			return (patCur.Birthdate.AddYears(PrefC.GetInt(PrefName.RecallAgeAdult)) > ((recallDue>DateTime.Today) ? recallDue : DateTime.Today));
		}

		public static List<Recall> GetChangedSince(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Recall>>(MethodBase.GetCurrentMethod(),changedSince);
			} 
			string command="SELECT * FROM recall WHERE DateTStamp > "+POut.DateT(changedSince);
			return Crud.RecallCrud.SelectMany(command);
		}

		///<summary>Used by FromRecallList, FormASAP, AutoComm to get list of patients with outstanding recalls.
		///Leave provNum or siteNum = 0 in order to avoid filtering on those columns.
		///If provNum > 0 then looks for both provider match in either PriProv or SecProv.
		///If clinicNum is less than 0, will get all clinics. 
		///codeRangeStart and codeRangeEnd will only be used if isAsap=true.</summary>
		public static DataTable GetRecallList(
			DateTime fromDate,DateTime toDate,bool groupByFamilies,long provNum,long clinicNum,long siteNum,
			RecallListSort sortBy,RecallListShowNumberReminders showReminders,bool isAsap=false,string codeRangeStart="",string codeRangeEnd="",
			bool doShowReminded=false,List<RecallType> listRecallTypes=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),fromDate,toDate,groupByFamilies,provNum,clinicNum,siteNum,
					sortBy,showReminders,isAsap,codeRangeStart,codeRangeEnd,doShowReminded,listRecallTypes);
			}
			#region Logging
			Stopwatch sw=new Stopwatch();
			Stopwatch swTotal=new Stopwatch();
			swTotal.Restart();
			string info=$"Start: {DateTime.Now.ToString(Logger.DATETIME_FORMAT)}\r\n  groupByFamilies={groupByFamilies}\r\n  provNum={provNum}"+
				$"\r\n  provName={Providers.GetAbbr(provNum)}\r\n  clinicNum={clinicNum}\r\n  clinicName={Clinics.GetAbbr(clinicNum)}"+
				$"\r\n  siteNum={siteNum}\r\n  sortBy={sortBy}\r\n  showReminders={showReminders}\r\n  isAsap={isAsap}"+
				$"\r\n  fromDate={fromDate.ToString("MM/dd/yy")}\r\n  toDate={toDate.ToString("MM/dd/yy")}";
            string verbose="INFO";
			Action<string,string,int> logQuery=new Action<string,string,int>((queryName,queryCommand,numRows) => {
				sw.Stop();
				info+=$"\r\nQUERY {sw.Elapsed.TotalSeconds.ToString("0.00")}s {queryName} {numRows} rows";
				verbose+=$"\r\nQUERY {sw.Elapsed.TotalSeconds.ToString("0.00")}s {queryName} {numRows} rows\r\n{queryCommand}";
			});
			Action<string> logOther=new Action<string>((line) => {
				sw.Stop();
				info+=$"\r\nOTHER {sw.Elapsed.TotalSeconds.ToString("0.00")}s {line} ";
			});
			#endregion
			string command=
				//Recall.
				"SELECT recall.RecallNum,recall.PatNum,recall.DateDue,recall.DatePrevious,recall.RecallInterval,recall.RecallStatus,recall.Note,"
				+"recall.DisableUntilBalance,recall.DisableUntilDate,recalltype.Description recalltype,recall.Priority,recall.RecallTypeNum, "
				//Patient.
				+"patient.PatNum,patient.LName,patient.FName,patient.Preferred,patient.Birthdate,patient.HmPhone,patient.WkPhone,"
				+"patient.WirelessPhone,patient.Email,patient.ClinicNum,patient.PreferRecallMethod,patient.BillingType,"
				//Guarantor.
				+"patient.Guarantor,patguar.LName guarLName,patguar.FName guarFName,patguar.Email guarEmail,patguar.InsEst,patguar.BalTotal ";
			if(isAsap) {
				#region ASAP Query String
				command+=
					"FROM recall "
					+"INNER JOIN patient ON recall.PatNum=patient.PatNum "
					+"INNER JOIN patient patguar ON patient.Guarantor=patguar.PatNum "
					+"INNER JOIN recalltype ON recall.RecallTypeNum=recalltype.RecallTypeNum ";
				if(!string.IsNullOrEmpty(codeRangeStart)) {
					command+="INNER JOIN (SELECT DISTINCT(RecallTypeNum) recallTypeNum FROM recalltrigger "
						+"INNER JOIN procedurecode ON procedurecode.CodeNum=recalltrigger.CodeNum "
						+"WHERE procedurecode.ProcCode BETWEEN '"+POut.String(codeRangeStart)+"' AND '"+POut.String(codeRangeEnd)+"'"
						+") coderange ON coderange.RecallTypeNum=recalltype.RecallTypeNum ";
				}
				command+=
					"WHERE recall.DateDue BETWEEN "+POut.Date(fromDate)+" AND "+POut.Date(toDate)+" "
					+"AND "+DbHelper.Year("recall.DateScheduled")+" < 1880 "
					+"AND recall.Priority="+POut.Int((int)ApptPriority.ASAP)+" ";
				#endregion
			}
			else {
				#region RECALL Query String
				command+=
					"FROM recall "
					+"INNER JOIN patient ON recall.PatNum=patient.PatNum "
					+"INNER JOIN patient patguar ON patient.Guarantor=patguar.PatNum "
					+"INNER JOIN recalltype ON recall.RecallTypeNum=recalltype.RecallTypeNum "
					+"WHERE recall.DateDue BETWEEN "+POut.Date(fromDate)+" AND "+POut.Date(toDate)+" "
					+"AND recall.IsDisabled=0 ";
				List<string> listRecallTypesCur=PrefC.GetString(PrefName.RecallTypesShowingInList).Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
				if(!listRecallTypes.IsNullOrEmpty()) {
					if(!listRecallTypesCur.IsNullOrEmpty()) {
						listRecallTypesCur=listRecallTypesCur.Intersect(listRecallTypes.Select(x => POut.Long(x.RecallTypeNum))).ToList();
					}
					else {
						listRecallTypesCur=listRecallTypes.Select(x => POut.Long(x.RecallTypeNum)).ToList();
					}
				}
				if(!listRecallTypesCur.IsNullOrEmpty()) {
					command+=$"AND recall.RecallTypeNum IN({string.Join(",",listRecallTypesCur)}) ";
				}
				if(PrefC.GetBool(PrefName.RecallExcludeIfAnyFutureAppt)) {
					command+="AND NOT EXISTS(SELECT * FROM appointment "
						+"WHERE appointment.PatNum=recall.PatNum "
						+"AND appointment.AptDateTime>"+DbHelper.Curdate()+" "//early this morning
						+"AND appointment.AptStatus="+POut.Int((int)ApptStatus.Scheduled)+") ";
				}
				else {
					command+="AND recall.DateScheduled='0001-01-01' "; //Only show rows where no future recall appointment.
				}
				#endregion
			}
			#region Patient Filter
			command+="AND patient.PatStatus="+POut.Int((int)PatientStatus.Patient)+" ";
			if(provNum>0) {
				command+="AND (patient.PriProv="+POut.Long(provNum)+" OR patient.SecProv="+POut.Long(provNum)+") ";
			}
			if(clinicNum>=0) { //Only include appointments that belong to HQ clinic when clinics are enabled and no ClinicNum is specified.
				command+="AND patient.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(siteNum>0) {
				command+="AND patient.SiteNum="+POut.Long(siteNum);
			}
			#endregion
			DataTable table=new DataTable();
			#region Add Columns
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("age");
			table.Columns.Add("billingType");
			table.Columns.Add("contactMethod");//text representation for display
			table.Columns.Add("ClinicNum");
			table.Columns.Add("dateLastReminder");
			table.Columns.Add("DateDue",typeof(DateTime));
			table.Columns.Add("dueDate");//blank if minVal
			table.Columns.Add("Email");
			table.Columns.Add("FName");
			table.Columns.Add("Guarantor");
			table.Columns.Add("guarFName");
			table.Columns.Add("guarLName");
			table.Columns.Add("LName");
			table.Columns.Add("maxDateDue",typeof(DateTime));
			table.Columns.Add("Note");
			table.Columns.Add("numberOfReminders");
			table.Columns.Add("patientName");
			table.Columns.Add("PatNum");
			table.Columns.Add("PreferRecallMethod");
			table.Columns.Add("recallInterval");
			table.Columns.Add("RecallNum");
			table.Columns.Add("recallType");
			table.Columns.Add("RecallTypeNum");
			table.Columns.Add("RecallStatus");
			table.Columns.Add("status");
			table.Columns.Add("WebSchedRecallNum");
			table.Columns.Add("webSchedEmailSendStatus");
			table.Columns.Add("webSchedDateTimeFailed");
			table.Columns.Add("webSchedSendDesc");
			table.Columns.Add("webSchedSendError");
			table.Columns.Add("webSchedSmsSendStatus");
			table.Columns.Add("WirelessPhone");
			table.Columns.Add("Priority");
			#endregion
			List<DataRow> rows=new List<DataRow>();
			#region Run Queries and Create Dictionaries
			#region Recall query
			sw.Restart();
			DataTable rawRecallTable=Db.GetTable(command);
			logQuery("recallTable",command,rawRecallTable.Rows.Count);
			#endregion Recall query
			if(rawRecallTable.Rows.Count<1) {
				return table;//No recalls, no need to proceed any further.
			}
			//Sort recalls into dictionary of PatNum to List<DataRow>, one DataRow for each recall. 
			//Excludes recalls that are disabled.
			sw.Restart();
			Dictionary<long,List<DataRow>> dictRecallRows=rawRecallTable.Rows.OfType<DataRow>()
				.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
				//Multiple recalls per patient.
				.ToDictionary(x => x.Key,x => x.ToList());
			logOther($"dictRecallRows {dictRecallRows.Count} rows");
			//Create dict of PatNum to DataRow continaing pat/guarantor data, one row per patnum.
			sw.Restart();
			Dictionary<long,DataRow> dictPatientRows=rawRecallTable.Rows.OfType<DataRow>()
				.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
				//Just take the first patient from each group.
				.ToDictionary(x => x.Key,x => x.First());
			logOther($"dictPatientRows {dictPatientRows.Count} rows");
			//Create dict Guarantor to max DateDue for all family member recalls in the date range
			sw.Restart();
			Dictionary<long,DateTime> dictGuarMaxDateDue=rawRecallTable.Rows.OfType<DataRow>()
				.GroupBy(x => PIn.Long(x["Guarantor"].ToString()),x => PIn.Long(x["PatNum"].ToString())) //get key=guarantor PatNum, value=family member PatNums
				.ToDictionary(x => x.Key,x => x.Where(y => dictRecallRows.ContainsKey(y)) //where there is a recall for the family member
					.SelectMany(y => dictRecallRows[y] //SelectMany because a patient may have more than one recalltype due
						.Select(z => PIn.Date(z["DateDue"].ToString()))).Max());//Select max DateDue for all recalls for all family members
			logOther($"dictGuarMaxDateDue {dictGuarMaxDateDue.Count} rows");
			//Check the commlog table to find any reminders have been sent to these patients.
			command="SELECT PatNum,CommDateTime "
				+"FROM commlog "
				+"WHERE CommType="+POut.Long(Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL))+" "
				+"AND PatNum IN ("+string.Join(",",dictPatientRows.Keys)+")";
			sw.Restart();
			//Create dictionary of key=PatNum, value=List of CommDateTimes for that patient
			Dictionary<long,List<DateTime>> dictCommlogs=Db.GetTable(command).Select()
				.GroupBy(x => PIn.Long(x["PatNum"].ToString()),x => PIn.DateT(x["CommDateTime"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			logQuery("dictCommlogs",command,dictCommlogs.Values.Sum(x => x.Count()));
			//Get the most recent webschedrecall reminders that have been sent to these patients.
			command="SELECT webschedrecall.WebSchedRecallNum,webschedrecall.PatNum,webschedrecall.SmsSendStatus,webschedrecall.EmailSendStatus,"
				+"webschedrecall.DateTimeSendFailed,webschedrecall.ResponseDescript,webschedrecall.DateTimeReminderSent "
				+"FROM webschedrecall "
				+"INNER JOIN ("
					+"SELECT PatNum,MAX(DateTimeEntry) DateTimeEntry "
					+"FROM webschedrecall "
					+"WHERE PatNum IN ("+string.Join(",",dictPatientRows.Keys)+") "
					+"GROUP BY PatNum "
				+") recent ON recent.PatNum=webschedrecall.PatNum AND recent.DateTimeEntry=webschedrecall.DateTimeEntry";
			sw.Restart();
			DataTable tableWebSchedRecalls=Db.GetTable(command);
			logQuery("tableWebSchedRecalls",command,tableWebSchedRecalls.Rows.Count);
			//Create dictionary of key=PatNum, value=List of WebSchedRecalls for that patient
			sw.Restart();
			Dictionary<long,WebSchedRecall> dictWebSchedRecalls=tableWebSchedRecalls.AsEnumerable()
				.Select(x => new WebSchedRecall {
					WebSchedRecallNum=PIn.Long(x["WebSchedRecallNum"].ToString()),
					PatNum=PIn.Long(x["PatNum"].ToString()),
					SmsSendStatus=PIn.Enum<AutoCommStatus>(PIn.Int(x["SmsSendStatus"].ToString())),
					EmailSendStatus=PIn.Enum<AutoCommStatus>(PIn.Int(x["EmailSendStatus"].ToString())),
					DateTimeSendFailed=PIn.DateT(x["DateTimeSendFailed"].ToString()),
					ResponseDescript=PIn.String(x["ResponseDescript"].ToString()),
					DateTimeReminderSent=PIn.DateT(x["DateTimeReminderSent"].ToString()),
				})
				.GroupBy(x => x.PatNum,x => x)
				.ToDictionary(x => x.Key,x => x.First());
			logOther($"dictWebSchedRecalls {dictWebSchedRecalls.Count} rows");
			#endregion Run Queries and Create Dictionaries
			List<DateTime> listDatesRemindersSent;
			DataRow rowPat;
			DateTime dateDue;
			DateTime datePrevious;
			DateTime dateRemind;
			int numberOfReminders;
			long patNum;
			long guarNum;
			double familyBalance;
			DataRow row;
			#region Create List of Rows for Return Table
			//loop through the patients in the recall dictionary
			sw.Restart();
			foreach(KeyValuePair<long,List<DataRow>> kvp in dictRecallRows) {
				patNum=kvp.Key;
				if(!dictPatientRows.ContainsKey(patNum)) {//patient.PatStatus wasn't 'Patient'
					continue;
				}
				rowPat=dictPatientRows[patNum];
				guarNum=PIn.Long(rowPat["Guarantor"].ToString());
				if(!dictCommlogs.TryGetValue(patNum,out listDatesRemindersSent)) {
					listDatesRemindersSent=new List<DateTime>();
				}
				familyBalance=PIn.Double(rowPat["BalTotal"].ToString());//from the guarantor's patient table
				if(!PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {//typical
					familyBalance-=PIn.Double(rowPat["InsEst"].ToString());
				}
				//loop through the recalls for each patient
				foreach(DataRow rowCur in kvp.Value) {
					dateDue=PIn.Date(rowCur["DateDue"].ToString());
					datePrevious=PIn.Date(rowCur["DatePrevious"].ToString());
					//get list of all reminder dates where the date is after datePrevious for this recall
					List<DateTime> listDTReminders=listDatesRemindersSent.Where(x => x>datePrevious).ToList();
					numberOfReminders=listDTReminders.Distinct().Count();//number of recall reminders that happened after datePrevious
					dateRemind=listDTReminders.DefaultIfEmpty(DateTime.MinValue).Max();
					bool hasRecentWebSchedRecall=dictWebSchedRecalls.TryGetValue(patNum,out WebSchedRecall webSchedSendMostRecent);
					if(!DoIncludeRecall(ref numberOfReminders,dateRemind,showReminders,PIn.Double(rowCur["DisableUntilBalance"].ToString()),
						PIn.Date(rowCur["DisableUntilDate"].ToString()),familyBalance,isAsap,doShowReminded,webSchedSendMostRecent))
					{
						continue;
					}
					#region Create Row
					row=table.NewRow();
					row["age"]=Patients.DateToAge(PIn.Date(rowPat["Birthdate"].ToString())).ToString();
					row["billingType"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(rowPat["BillingType"].ToString()));
					row["ClinicNum"]=PIn.Long(rowPat["ClinicNum"].ToString());
					row["contactMethod"]=GetContactFromMethod(PIn.Enum<ContactMethod>(rowPat["PreferRecallMethod"].ToString()),groupByFamilies
						,rowPat["HmPhone"].ToString(),rowPat["WkPhone"].ToString(),rowPat["WirelessPhone"].ToString(),rowPat["guarEmail"].ToString()
						,rowPat["Email"].ToString());
					row["dateLastReminder"]="";
					if(dateRemind.Year>1880) {
						row["dateLastReminder"]=dateRemind.ToShortDateString();
					}
					row["DateDue"]=dateDue;
					if(dateDue.Year>1880) {
						row["dueDate"]=dateDue.ToShortDateString();
					}
					if(groupByFamilies) {
						row["Email"]=rowPat["guarEmail"].ToString();
					}
					else {
						row["Email"]=rowPat["Email"].ToString();
					}
					row["PatNum"]=patNum.ToString();
					row["FName"]=rowPat["FName"].ToString();
					row["LName"]=rowPat["LName"].ToString();
					row["patientName"]=Patients.GetNameLF(rowPat["LName"].ToString(),rowPat["FName"].ToString(),rowPat["Preferred"].ToString(),"");
					row["Guarantor"]=guarNum.ToString();
					row["guarFName"]=rowPat["guarFName"].ToString();
					row["guarLName"]=rowPat["guarLName"].ToString();
					row["Priority"]=rowCur["Priority"].ToString();
					row["maxDateDue"]=DateTime.MinValue;
					if(dictGuarMaxDateDue.ContainsKey(guarNum)) {
						row["maxDateDue"]=dictGuarMaxDateDue[guarNum];
					}
					row["Note"]=rowCur["Note"].ToString();
					row["numberOfReminders"]="";
					if(numberOfReminders>0) {
						row["numberOfReminders"]=numberOfReminders.ToString();
					}
					row["PreferRecallMethod"]=rowPat["PreferRecallMethod"].ToString();
					row["recallInterval"]=(new Interval(PIn.Int(rowCur["RecallInterval"].ToString()))).ToString();
					row["RecallNum"]=rowCur["RecallNum"].ToString();
					row["recallType"]=rowCur["recalltype"].ToString();
					row["RecallTypeNum"]=rowCur["RecallTypeNum"].ToString();
					row["RecallStatus"]=rowCur["RecallStatus"].ToString();
					row["status"]=Defs.GetName(DefCat.RecallUnschedStatus,PIn.Long(rowCur["RecallStatus"].ToString()));
					row["WebSchedRecallNum"]="0";
					row["webSchedDateTimeFailed"]=DateTime.MinValue;
					row["webSchedEmailSendStatus"]=((int)AutoCommStatus.Undefined).ToString();
					row["webSchedSendDesc"]="";
					row["webSchedSendError"]="";
					row["webSchedSmsSendStatus"]=((int)AutoCommStatus.Undefined).ToString();
					if(hasRecentWebSchedRecall) {
						row["webSchedDateTimeFailed"]=webSchedSendMostRecent.DateTimeSendFailed;
						row["webSchedEmailSendStatus"]=((int)webSchedSendMostRecent.EmailSendStatus).ToString();
						row["webSchedSmsSendStatus"]=((int)webSchedSendMostRecent.SmsSendStatus).ToString();
						row["WebSchedRecallNum"]=webSchedSendMostRecent.WebSchedRecallNum;						
						if(webSchedSendMostRecent.EmailSendStatus==AutoCommStatus.SendNotAttempted
							&& webSchedSendMostRecent.SmsSendStatus==AutoCommStatus.SendNotAttempted) 
						{
							row["webSchedSendDesc"]=Lans.g("FormRecallList","Sending");
						}
						else if(webSchedSendMostRecent.EmailSendStatus!=AutoCommStatus.SendSuccessful
							&& webSchedSendMostRecent.SmsSendStatus==AutoCommStatus.SentAwaitingReceipt) 
						{
							row["webSchedSendDesc"]=Lans.g("FormRecallList","Awaiting Delivery Receipt");
						}
						else if(webSchedSendMostRecent.EmailSendStatus==AutoCommStatus.SendFailed
							|| webSchedSendMostRecent.SmsSendStatus==AutoCommStatus.SendFailed)
						{
							row["webSchedSendDesc"]=Lans.g("FormRecallList","Send Failed");
						}
						row["webSchedSendError"]=webSchedSendMostRecent.ResponseDescript;
					}
					row["WirelessPhone"]=rowPat["WirelessPhone"].ToString();
					#endregion Create Row
					rows.Add(row);
				}
			}
			logOther($"createRows {rows.Count} rows");
			#endregion Create List of Rows for Return Table
			RecallComparer comparer=new RecallComparer();
			comparer.GroupByFamilies=groupByFamilies;
			comparer.SortBy=sortBy;
			sw.Restart();
			rows.Sort(comparer);
			logOther($"sortRows {rows.Count} rows");
			sw.Restart();
			rows.ForEach(x => table.Rows.Add(x));
			logOther($"addRows {table.Rows.Count} rows");
			swTotal.Stop();
			if(ODBuild.IsDebug()) {
				Logger.WriteLine($"\r\n----------SUMMARY TOTAL {swTotal.Elapsed.TotalSeconds.ToString("0.00")}s\r\n{info}\r\n\r\n","FillRecallTableInfo");
				Logger.WriteLine($"\r\n----------INFO TOTAL {swTotal.Elapsed.TotalSeconds.ToString("0.00")}s\r\n{info}\r\n\r\n----------\r\n{verbose}\r\n\r\n","FillRecallTableVerbose");
			}
			return table;
		}

		///<summary>Determines the contact method for the given patient and returns a string with an appropriate representation of that contact method.
		///</summary>
		public static string GetContactFromMethod(ContactMethod contMeth,bool isGroupByFamilies,string hmPhone,string wkPhone,string wirelessPhone
			,string guarEmail,string email) 
		{
			string contactMethod;
			switch(contMeth) {
				case ContactMethod.HmPhone:
					contactMethod=Lans.g("FormRecallList","Hm")+":"+hmPhone;
					break;
				case ContactMethod.WkPhone:
					contactMethod=Lans.g("FormRecallList","Wk")+":"+wkPhone;
					break;
				case ContactMethod.WirelessPh:
					contactMethod=Lans.g("FormRecallList","Cell")+":"+wirelessPhone;
					break;
				case ContactMethod.TextMessage:
					contactMethod=Lans.g("FormRecallList","Text")+":"+wirelessPhone;
					break;
				case ContactMethod.Email:
					if(isGroupByFamilies) {
						contactMethod=guarEmail;//always use guarantor email if grouped by fam
					}
					else {
						contactMethod=email;
					}
					break;
				case ContactMethod.Mail:
					contactMethod=Lans.g("FormRecallList","Mail");
					break;
				case ContactMethod.DoNotCall:
				case ContactMethod.SeeNotes:
					contactMethod=Lans.g("enumContactMethod",contMeth.GetDescription());
					break;
				case ContactMethod.None:
				default:
					if(PrefC.GetBool(PrefName.RecallUseEmailIfHasEmailAddress)) {//if user wants to use email if there is an email address
						if(isGroupByFamilies && guarEmail!="") {
							contactMethod=guarEmail;
							break;
						}
						else if(!isGroupByFamilies && email!="") {
							contactMethod=email;
							break;
						}
					}
					//no email, or user doesn't want to use email even if there is one, default to using HmPhone
					contactMethod=Lans.g("FormRecallList","Hm")+":"+hmPhone;
					break;
			}
			return contactMethod;
		}

		///<summary>Returns true if a recall reminder should be sent for this patient based on the passed in arguments.</summary>
		private static bool DoIncludeRecall(ref int numberOfReminders,DateTime dateRemind,RecallListShowNumberReminders showReminders
			,double disableUntilBalance,DateTime disableUntilDate,double familyBalance,bool isAsap,bool doShowReminded,WebSchedRecall webSchedRecall) 
		{
			if(webSchedRecall?.SmsSendStatus==AutoCommStatus.SentAwaitingReceipt && webSchedRecall?.EmailSendStatus!=AutoCommStatus.SendSuccessful) {
				//A webschedrecall email was not sent(or failed) but a corresponding SMS is sent and waiting for delivery receipt.  For the sake of the
				//recall list, we will consider this scenario a "sent" recall until the receipt arrives, at which time we will know if it was a successful 
				//recall reminder or if it failed.  Considering the sms successfully sent will keep this patient off the recall list so the user doesn't 
				//accidentally send a manual recall.
				numberOfReminders++;
				dateRemind=ODMathLib.Max(webSchedRecall.DateTimeReminderSent,dateRemind);
			}
			//filter by disable until date and balance
			if(disableUntilDate>DateTime.Today) {
				return false;
			}
			if(disableUntilBalance>0 && familyBalance>disableUntilBalance) {
				return false;
			}
			if(showReminders==RecallListShowNumberReminders.All) {
				//don't skip, add all to list
			}
			else if(showReminders<RecallListShowNumberReminders.SixPlus) {
				if(numberOfReminders!=((int)showReminders-1)) {//if numberOfReminders!=enum value cast to int -1 to account for All being 0
					return false;
				}
			}
			else if(showReminders==RecallListShowNumberReminders.SixPlus) {
				if(numberOfReminders<((int)showReminders-1)) {//numberOfReminders<6 (SixPlus is index 7 since All=0 and Zero=1, so -1)
					return false;
				}
			}
			if(isAsap || doShowReminded) {
				//The ASAP list and Include reminded includes recalls regardless of the time since the last reminder and regardless of the max number of reminders.
				return true;
			}
			//filter by number of reminders, if numberOfReminders==0, always show
			return !HasTooManyReminders(numberOfReminders,dateRemind);
		}

		///<summary>Determines if the given numberOfReminders would exceed the allowed amount of Recall reminders, either by recall interval preference
		///or by PrefName.RecallMaxNumberReminders.</summary>
		public static bool HasTooManyReminders(int numberOfReminders,DateTime dateRemind) {
			if(numberOfReminders==1) {
				if(PrefC.GetInt(PrefName.RecallShowIfDaysFirstReminder)==-1) {
					return true;
				}
				if(dateRemind.AddDays(PrefC.GetInt(PrefName.RecallShowIfDaysFirstReminder)).Date>DateTime_.Today) {
					return true;
				}
			}
			else if(numberOfReminders>1) {
				if(PrefC.GetInt(PrefName.RecallShowIfDaysSecondReminder)==-1) {
					return true;
				}
				if(dateRemind.AddDays(PrefC.GetInt(PrefName.RecallShowIfDaysSecondReminder)).Date>DateTime_.Today) {
					return true;
				}
			}			
			long recallMaxNumberReminders=PrefC.GetLong(PrefName.RecallMaxNumberReminders);
			if(recallMaxNumberReminders>-1 && numberOfReminders>recallMaxNumberReminders) {
				return true;
			}
			return false;
		}

		///<summary>Replaces all recall fields in the given message with the given patient's recall information.  Returns the resulting string.
		///Replaces: [DueDate],[URL]</summary>
		public static string ReplaceRecall(string message,Patient pat) {
			if(pat==null) {
				return message;
			}
			string retVal=message;
			List<Recall> listRecalls=Recalls.GetList(pat.PatNum);
			if(listRecalls.Count>0) {
				//Max selected recall date for the patient.
				retVal=retVal.Replace("[DueDate]",listRecalls.Max(x => x.DateDue).ToShortDateString());
			}
			//The link where a patient can go to schedule a recall from the web.
			retVal=retVal.Replace("[URL]",PrefC.GetString(PrefName.PatientPortalURL));
			return retVal;
		}

		///<summary></summary>
		public static long Insert(Recall recall) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				recall.RecallNum=Meth.GetLong(MethodBase.GetCurrentMethod(),recall);
				return recall.RecallNum;
			}
			return Crud.RecallCrud.Insert(recall);
		}

		///<summary></summary>
		public static void Update(Recall recall) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),recall);
				return;
			}
			Crud.RecallCrud.Update(recall);
		}

		///<summary>Returns true if it was updated</summary>
		public static bool Update(Recall recall,Recall recallOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),recall,recallOld);
			}
			return Crud.RecallCrud.Update(recall,recallOld);
		}

		///<summary></summary>
		public static void Delete(Recall recall) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),recall);
				return;
			}
			string command= "DELETE from recall WHERE RecallNum = "+POut.Long(recall.RecallNum);
			Db.NonQ(command);
			DeletedObjects.SetDeleted(DeletedObjectType.RecallPatNum,recall.PatNum);
		}

		///<summary>Returns false if the synch is in the process of running.</summary>
		public static bool SynchAllPatients(bool doThrowException=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),doThrowException);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				throw new ApplicationException("SynchAllPatients is not Oracle compatible, please call support.");
			}
			if(_odThreadQueueData!=null) {
				return false;
			}
			Stopwatch s=new Stopwatch();
			int threadWaitCount=0;
			if(ODBuild.IsDebug()) {		
				s.Start();
			}
			_odThreadQueueData=new ODThread(QueueDataBatches);
			_odThreadQueueData.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => {
				_isQueueBatchThreadDone=true;
			}));
			_odThreadQueueData.Name="RecallSyncQueueDataThread";
			_isQueueBatchThreadDone=false;
			ODThread odThreadQueueData2=new ODThread(QueueDataBatches);
			odThreadQueueData2.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => {
				_isQueueBatchThreadDone2=true;
			}));
			odThreadQueueData2.Name="RecallSyncQueueDataThread2";
			_isQueueBatchThreadDone2=false;
			#region Get Global Parameters
			//Get all of the PatNum milestones which will allow the threads to get "full" batches based on BATCH_SIZE.
			_listPatNumMaxPerGroup=Patients.GetPatNumMaxForGroups(BATCH_SIZE,new List<PatientStatus>() { PatientStatus.Patient });
			if(_listPatNumMaxPerGroup.Count==0) {//not likely to happen, this would mean there are 0 patients in the db, nothing to do
				_odThreadQueueData=null;
				return true;
			}
			//reverse so that the last group, the partial group, is handled first in order to get an accurate count of pats processed for progress bar
			_listPatNumMaxPerGroup.Reverse();
			//last group of pats could be partial group, that count will be added to total in QueueDataBatches once we gather data for that partial group.
			//Setting this now prevents a possible divide by zero error in the progress bar and will be updated later to include the partial group count.
			_totalPatCount=(_listPatNumMaxPerGroup.Count-1)*BATCH_SIZE;
			long prophyTypeNum=PrefC.GetLong(PrefName.RecallTypeSpecialProphy);
			long perioTypeNum=PrefC.GetLong(PrefName.RecallTypeSpecialPerio);
			string command="SELECT recalltype.RecallTypeNum,recalltype.DefaultInterval,recalltrigger.CodeNum "
				+"FROM recalltype "
				+"INNER JOIN recalltrigger ON recalltype.RecallTypeNum=recalltrigger.RecallTypeNum";
			DataTable tableRecallTriggers=Db.GetTable(command);
			if(tableRecallTriggers.Rows.Count==0) {//no recall triggers, nothing to do
				_odThreadQueueData=null;
				return true;
			}
			//make dictionary of RecallTypeNum key to DefaultInterval value
			Dictionary<long,Interval> dictRecallInterval=tableRecallTriggers.Select()
				.GroupBy(x => PIn.Long(x["RecallTypeNum"].ToString()))
				.ToDictionary(x => x.Key,x => new Interval(PIn.Int(x.First()["DefaultInterval"].ToString())));
			//dictionary of RecallTypeNum key to list of trigger procedure CodeNums
			Dictionary<long,List<long>> dictRecallCodes=tableRecallTriggers.Select()
				.GroupBy(x => PIn.Long(x["RecallTypeNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.Select(y => PIn.Long(y["CodeNum"].ToString())).ToList());
			List<long> listSpecialCodes=dictRecallCodes.Where(x => ListTools.In(x.Key,prophyTypeNum,perioTypeNum)).SelectMany(x => x.Value).ToList();
			#endregion Get Global Parameters
			#region Start Batch Data Thread
			lock(_lockObjQueueBatchData) {
				_queueBatchData=new Queue<Dictionary<long,PatBatchData>>();
			}
			//Start two threads that will try and fill _queueBatchData as fast as they can.
			_odThreadQueueData.Start(true);
			odThreadQueueData2.Start(true);
			#endregion Start Batch Data Thread
			int patProcessedCount=0;
			Dictionary<long,PatBatchData> dictPatBatchData=new Dictionary<long,PatBatchData>();
			//The main thread will be responsible for processing the data that the two threads above have queued up.
			#region Process Batches of Data
			try {
				while(!_isQueueBatchThreadDone || !_isQueueBatchThreadDone2 || _queueBatchData.Count>0) {//if batch thread is done and queue is empty, loop is finished
					//queueBatchThread must not be finished gathering batches but the queue is empty, give the batch thread time to catch up
					if(_queueBatchData.Count==0) {
						if(ODBuild.IsDebug()) {
							if(patProcessedCount>0) {
								threadWaitCount++;
							}
						}
						continue;
					}
					try {
						lock(_lockObjQueueBatchData) {
							dictPatBatchData=_queueBatchData.Dequeue();
						}
					}
					catch(Exception ex) {//queue must be empty even though we just checked it before entering the while loop, just loop again and wait if necessary
						ex.DoNothing();
						continue;
					}
					//not likely to happen, this is checked when filling the queue, but just in case
					if(dictPatBatchData==null || dictPatBatchData.Count==0) {
						continue;
					}
					List<Action> listActions=new List<Action>();
					List<Recall> listRecallsForInsert=new List<Recall>();
					object lockObjListRecallsForInsert=new object();
					#region Create List of Actions to Update Patient Recalls
					int curBatchCount=0;
					object lockSecurityLogList=new object();
					List<long> listSecurityLogPatNums=new List<long>();
					foreach(KeyValuePair<long,PatBatchData> patToUpdate in dictPatBatchData) {
						listActions.Add(new Action(() => {
							++patProcessedCount;
							//Only fire a few progress events so that the program doesn't slow down due to the UI updating.
							//Updating too infrequently will cause the main thread to spin too fast.  Mod 5 is a good throttle.
							if(++curBatchCount%5==0 || curBatchCount==dictPatBatchData.Count) {
								RecallSyncEvent.Fire(ODEventType.RecallSync,new ProgressBarHelper(
									Lans.g("Recalls","Recalls Completed")+" "+patProcessedCount+"/"+_totalPatCount+" - "
										+Math.Floor(((double)patProcessedCount/_totalPatCount)*100).ToString()+"%",
									Math.Floor(((double)patProcessedCount/_totalPatCount)*100)+"%",
									patProcessedCount,
									_totalPatCount,
									ProgBarStyle.Blocks,
									progressBarEventType:ProgBarEventType.Header
								));
							}
							#region SynchPatient
							PatBatchData patBatchData=patToUpdate.Value;
							//guaranteed to be at least one row for each patient, isPerio will be the same value for all rows for the patient
							bool isPerio=patBatchData.ListRecalls.Any(x => x.RecallTypeNum==perioTypeNum);
							//could be DateTime.MinValue if no procs have been completed that match the CodeNums assigned to any recall "special type" triggers.
							DateTime dateMaxSpecialType=patBatchData.DictLastProcDates.Where(x => listSpecialCodes.Contains(x.Key))
								.Select(x => x.Value).DefaultIfEmpty(DateTime.MinValue).Max();
							//Loop through all of the recall trigger types and take actions needed for any recall triggers that apply to this patient.
							foreach(KeyValuePair<long,List<long>> kvp in dictRecallCodes) {
								bool isProphySpecialType=(kvp.Key==prophyTypeNum);
								bool isPerioSpecialType=(kvp.Key==perioTypeNum);
								//Skip this recall type if it does not match this patient's recall "special type" (assume there is only one).
								//E.g. This patient is flagged as a perio patient (isPerio = 1) so we need to skip the special prophy type.
								if((isPerio && isProphySpecialType) || (!isPerio && isPerioSpecialType)) {
									continue;
								}
								Interval defaultInterval;
								if(!dictRecallInterval.TryGetValue(kvp.Key,out defaultInterval)) {
									defaultInterval=new Interval();
								}
								DateTime datePrev=DateTime.MinValue;
								//For special recall types only, we need to get the most recent proc date.
								if((isProphySpecialType || isPerioSpecialType) && dateMaxSpecialType.Year>1880) {
									datePrev=dateMaxSpecialType;
								}
								else {
									//Default datePrev to the most recent proc date for all trigger codes for this recall type for this patient
									datePrev=kvp.Value.Where(x => patBatchData.DictLastProcDates.ContainsKey(x))
										.Select(x => patBatchData.DictLastProcDates[x]).DefaultIfEmpty(DateTime.MinValue).Max();
								}
								//At this point, we know that action may be needed for this particular recall type.
								//We will either update recalls, create new recalls, or do nothing for this patient and the particular recall type.
								Recall recallCur=patBatchData.ListRecalls.FirstOrDefault(x => x.RecallTypeNum==kvp.Key);
								if(recallCur==null) {//if there is no existing recall,
									if(isProphySpecialType || isPerioSpecialType || datePrev.Year>1880) {//for other types, if date is not minVal, then add a recall
										//add a recall
										recallCur=new Recall();
										recallCur.RecallTypeNum=kvp.Key;
										recallCur.PatNum=patToUpdate.Key;
										recallCur.DatePrevious=datePrev;//will be min val for prophy/perio with no previous procs
										recallCur.RecallInterval=defaultInterval;
										if(datePrev.Year<1880) {
											recallCur.DateDueCalc=DateTime.MinValue;
										}
										else {
											recallCur.DateDueCalc=datePrev+recallCur.RecallInterval;
										}
										recallCur.DateDue=recallCur.DateDueCalc;
										DateTime dateSched;
										if(!patBatchData.DictRecallTypesSched.TryGetValue(recallCur.RecallTypeNum,out dateSched)) {
											dateSched=DateTime.MinValue;
										}
										recallCur.DateScheduled=dateSched;
										lock(lockObjListRecallsForInsert) {
											listRecallsForInsert.Add(recallCur.Copy());
										}
										patBatchData.ListRecalls.Add(recallCur.Copy());//add to pat recall list so we don't add a duplicate for this recalltype for each trigger
									}
								}
								else {//alter the existing recall
									Recall recallOld=recallCur.Copy();
									if(!recallCur.IsDisabled
										&& recallCur.DisableUntilBalance==0
										&& recallCur.DisableUntilDate.Year<1880
										&& datePrev.Year>1880 //this protects recalls that were manually added as part of a conversion
										&& datePrev != recallCur.DatePrevious) //if datePrevious has changed, reset
									{
										recallCur.RecallStatus=0;
										recallCur.Note="";
										recallCur.DateDue=recallCur.DateDueCalc;//now it is allowed to be changed in the steps below
									}
									if(datePrev.Year<1880) {//if no previous date
										recallCur.DatePrevious=DateTime.MinValue;
										if(recallCur.DateDue==recallCur.DateDueCalc) {//user did not enter a DateDue
											recallCur.DateDue=DateTime.MinValue;
										}
										recallCur.DateDueCalc=DateTime.MinValue;
									}
									else {//if previous date is a valid date
										recallCur.DatePrevious=datePrev;
										if(recallCur.IsDisabled) {//if the existing recall is disabled 
											recallCur.DateDue=DateTime.MinValue;//DateDue is always blank
										}
										else {//but if not disabled
											if(recallCur.DateDue==recallCur.DateDueCalc//if user did not enter a DateDue
												|| recallCur.DateDue.Year<1880)//or DateDue was blank
											{
												recallCur.DateDue=recallCur.DatePrevious+recallCur.RecallInterval;//set same as DateDueCalc
											}
										}
										recallCur.DateDueCalc=recallCur.DatePrevious+recallCur.RecallInterval;
									}
									DateTime dateSched;
									if(!patBatchData.DictRecallTypesSched.TryGetValue(recallCur.RecallTypeNum,out dateSched)) {
										dateSched=DateTime.MinValue;
									}
									recallCur.DateScheduled=dateSched;
									if(recallCur.RecallNum>0 //we could be modifying the recall we added in a previous loop that has not been inserted into the db yet
										&& Recalls.Update(recallCur,recallOld)) 
									{
										lock(lockSecurityLogList) {
											listSecurityLogPatNums.Add(recallCur.PatNum);
										}										
									}
								}
							}
							#endregion SynchPatient
						}));
					}
					#endregion Create List of Actions to Update Patient Recalls
					ODThread.RunParallel(listActions,TimeSpan.FromMinutes(10));//each group of actions gets X minutes.
					SecurityLogs.MakeLogEntry(Permissions.RecallEdit,listSecurityLogPatNums,"Recall updated by Recall Synch for all patients.");
					#region Insert New Recalls
					if(listRecallsForInsert.Count==0) {
						continue;
					}
					RecallSyncEvent.Fire(ODEventType.RecallSync,new ProgressBarHelper(
						Lans.g("Recalls","Recalls Completed")+" "+patProcessedCount+"/"+_totalPatCount+" - "
							+Math.Floor(((double)patProcessedCount/_totalPatCount)*100).ToString()+"% - "
							+Lans.g("Recalls","Inserting Recalls")+": "+listRecallsForInsert.Count,
						Math.Floor(((double)patProcessedCount/_totalPatCount)*100)+"%",
						patProcessedCount,
						_totalPatCount,
						ProgBarStyle.Blocks,
						progressBarEventType:ProgBarEventType.Header
					));
					Crud.RecallCrud.InsertMany(listRecallsForInsert);
					#endregion Insert New Recalls
				}
			}
			catch(Exception ex) {
				if(doThrowException) {
					throw;
				}
				ex.DoNothing();
			}
			finally {
				_odThreadQueueData?.QuitAsync();
				odThreadQueueData2?.QuitAsync();
				_odThreadQueueData=null;
			}
			#endregion Process Batches of Data
			if(ODBuild.IsDebug()) {
				s.Stop();
				Console.WriteLine("Runtime: "+s.Elapsed.Minutes+" min "+(s.Elapsed.TotalSeconds-(s.Elapsed.Minutes*60))+" sec, Main Thread wait count: "+threadWaitCount);
			}
			return true;
		}

		///<summary>Run method of SynchAllPatients threads.  This method expects only certain threads to call it which are named specifically.
		///This method also expects _listPatNumMaxPerGroup to be filled prior to invoking and will manipulate _queueBatchData as it executes.</summary>
		private static void QueueDataBatches(ODThread odThread) {
			//No need to check RemotingRole; private method.
			try {
				int count=_listPatNumMaxPerGroup.Count;
				int minIndex=0;
				//Potentially cut up the entire list of patNum groups to determine which batches each thread will be responsible for.
				if(odThread.Name=="RecallSyncQueueDataThread") {
					if(count>1) {//only if 2 or more batches will we share the work with the second thread, otherwise this thread will handle 0 or 1 batch
						count=count/2;
					}
				}
				else {
					if(count<2) {//0 or 1 batch, the first thread will handle it
						return;
					}
					minIndex=count/2;
				}
				//At this point, the current thread knows how many batches to make.
				for(int i=minIndex;i<count;i++) {
					#region Get Dictionaries and Lists For Batch
					//Get all patients in this batch's PatNum range and their most recent proc date where the codeNums are in the trigger codeNums.
					string command="SELECT patient.PatNum,COALESCE(procedurelog.CodeNum,0) codeNum,"
						+"COALESCE(MAX(procedurelog.ProcDate),"+POut.Date(DateTime.MinValue)+") lastProcDate "
						+"FROM patient "
						+"LEFT JOIN procedurelog ON patient.PatNum=procedurelog.PatNum "
							+"AND procedurelog.ProcStatus IN("+POut.Int((int)ProcStat.C)+","+POut.Int((int)ProcStat.EC)+","+POut.Int((int)ProcStat.EO)+") "
							+"AND procedurelog.CodeNum IN(SELECT CodeNum FROM recalltrigger) "
						+"WHERE patient.PatStatus="+POut.Int((int)PatientStatus.Patient)+" "
						//if _listPatNumMaxPerGroup.Count==1, PatNum ranges won't be restricted
						+(i<_listPatNumMaxPerGroup.Count-1?("AND patient.PatNum>"+_listPatNumMaxPerGroup[i+1]+" "):"")
						+(i>0?("AND patient.PatNum<="+_listPatNumMaxPerGroup[i]+" "):"")
						+"GROUP BY patient.PatNum,procedurelog.CodeNum "
						+"ORDER BY patient.PatNum";
					Dictionary<long,PatBatchData> dictPatBatch=Db.GetTable(command).Select()
						.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
						.ToDictionary(x => x.Key,x => new PatBatchData() {
							DictLastProcDates=x.ToDictionary(y => PIn.Long(y["codeNum"].ToString()),y => PIn.Date(y["lastProcDate"].ToString()))
						});
					//not likely to happen, this would mean no patients with PatStatus=0 in the range of PatNums
					if(dictPatBatch==null || dictPatBatch.Count==0) {
						continue;
					}
					if(i==0) {
						_totalPatCount=((_listPatNumMaxPerGroup.Count-1)*BATCH_SIZE)+dictPatBatch.Count;//last group of pats is first processed, possibly partial
					}
					//Get any existing recalls for the patients in the batch.  These are going to be patient recalls that we will potentially Update.
					command="SELECT recall.* "
						+"FROM recall "
						+"WHERE recall.RecallTypeNum IN(SELECT RecallTypeNum FROM recalltrigger)"
						//if _listPatNumMaxPerGroup.Count==1, PatNum ranges won't be restricted
						+(i<_listPatNumMaxPerGroup.Count-1?(" AND recall.PatNum>"+_listPatNumMaxPerGroup[i+1]):"")
						+(i>0?(" AND recall.PatNum<="+_listPatNumMaxPerGroup[i]):"");
					Crud.RecallCrud.SelectMany(command)
						.GroupBy(x => x.PatNum)
						.Where(x => dictPatBatch.ContainsKey(x.Key)).ToList()
						.ForEach(x => dictPatBatch[x.Key].ListRecalls=x.ToList());
					//Get the closest future scheduled date for the trigger codes.
					command="SELECT procedurelog.PatNum,recalltrigger.RecallTypeNum,MIN("+DbHelper.DtimeToDate("appointment.AptDateTime")+") AS aptDate "
						+"FROM procedurelog "
						+"INNER JOIN recalltrigger ON procedurelog.CodeNum=recalltrigger.CodeNum "
						+"INNER JOIN appointment USE INDEX (StatusDate) ON appointment.AptNum=procedurelog.AptNum "
							+"AND appointment.AptStatus="+POut.Int((int)ApptStatus.Scheduled)+" "
							+"AND appointment.AptDateTime > "+DbHelper.Curdate()+" ";
					if(_listPatNumMaxPerGroup.Count>1) {//if only one group, just include all PatNums
						command+="WHERE "+(i<_listPatNumMaxPerGroup.Count-1?("procedurelog.PatNum>"+_listPatNumMaxPerGroup[i+1]+" "+(i>0?"AND ":"")):"")
						+(i>0?("procedurelog.PatNum<="+_listPatNumMaxPerGroup[i]+" "):"");
					}
					command+="GROUP BY procedurelog.PatNum,recalltrigger.RecallTypeNum";
					Db.GetTable(command).Select()
						.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
						.Where(x => dictPatBatch.ContainsKey(x.Key)).ToList()
						.ForEach(x => x.ToList()
							.ForEach(y => dictPatBatch[x.Key].DictRecallTypesSched[PIn.Long(y["RecallTypeNum"].ToString())]=PIn.Date(y["aptDate"].ToString())));
					#endregion Get Dictionaries and Lists For Batch
					lock(_lockObjQueueBatchData) {
						_queueBatchData.Enqueue(dictPatBatch);
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();//if error happens, just swallow the error and kill the thread
			}
			finally {//always make sure to notify the main thread that the thread is done so the main thread doesn't wait for eternity
				if(odThread.Name=="RecallSyncQueueDataThread") {
					_isQueueBatchThreadDone=true;
				}
				else {
					_isQueueBatchThreadDone2=true;
				}
			}
		}

		///<summary>Synchronizes all recalls for one patient. 
		///If datePrevious has changed, then it completely deletes the old status and note information and sets a new DatePrevious and dateDueCalc.  
		///Also updates dateDue to match dateDueCalc if not disabled.  Creates any recalls as necessary.  
		///Recalls will never get automatically deleted except when all triggers are removed.  Otherwise, the dateDueCalc just gets cleared.</summary>
		public static void Synch(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			Patient pat=Patients.GetPat(patNum);
			if(pat.PatStatus==PatientStatus.Archived) {//do not calculate recall if patient is archived.
				return;
			}
			List<RecallType> typeListActive=RecallTypes.GetActive();
			List<RecallType> typeList=new List<RecallType>(typeListActive);
			string command="SELECT * FROM recall WHERE PatNum="+POut.Long(patNum);
			List<Recall> recallList=Crud.RecallCrud.SelectMany(command);
			//determine if this patient is a perio patient.
			bool isPerio=false;
			for(int i=0;i<recallList.Count;i++){
				if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==recallList[i].RecallTypeNum){
					isPerio=true;
					break;
				}
			}
			//remove types from the list which do not apply to this patient.
			for(int i=0;i<typeList.Count;i++){//it's ok to not go backwards because we immediately break.
				if(isPerio) {
					if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==typeList[i].RecallTypeNum) {
						typeList.RemoveAt(i);
						break;
					}
				}
				else {
					if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==typeList[i].RecallTypeNum) {
						typeList.RemoveAt(i);
						break;
					}
				}
			}
			//get previous dates for all types at once.
			//Because of the inner join, this will not include recall types with no trigger.
			command="SELECT RecallTypeNum,MAX(ProcDate) procDate_ "
				+"FROM procedurelog,recalltrigger "
				+"WHERE PatNum="+POut.Long(patNum)
				+" AND procedurelog.CodeNum=recalltrigger.CodeNum "
				+"AND (";
			if(typeListActive.Count>0) {//This will include both prophy and perio, regardless of whether this is a prophy or perio patient.
				for(int i=0;i<typeListActive.Count;i++) {
					if(i>0) {
						command+=" OR";
					}
					command+=" RecallTypeNum="+POut.Long(typeListActive[i].RecallTypeNum);
				}
			} 
			else {
				command+=" RecallTypeNum=0";//Effectively forces an empty result set, without changing the returned table structure.
			}
			command+=") AND (ProcStatus = "+POut.Long((int)ProcStat.C)+" "
				+"OR ProcStatus = "+POut.Long((int)ProcStat.EC)+" "
				+"OR ProcStatus = "+POut.Long((int)ProcStat.EO)+") "
				+"GROUP BY RecallTypeNum";
			DataTable tableDates=Db.GetTable(command);
			//Go through the type list and either update recalls, or create new recalls.
			//Recalls that are no longer active because their type has no triggers will be ignored.
			//It is assumed that there are no duplicate recall types for a patient.
			DateTime prevDate;
			Recall matchingRecall;
			Recall recallNew;
			DateTime prevDateProphy=DateTime.MinValue;
			DateTime dateProphyTesting;
			for(int i=0;i<typeListActive.Count;i++) {
				if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)!=typeListActive[i].RecallTypeNum
					&& PrefC.GetLong(PrefName.RecallTypeSpecialPerio)!=typeListActive[i].RecallTypeNum) 
				{
					//we are only working with prophy and perio in this loop.
					continue;
				}
				for(int d=0;d<tableDates.Rows.Count;d++) {//procs for patient
					if(tableDates.Rows[d]["RecallTypeNum"].ToString()==typeListActive[i].RecallTypeNum.ToString()) {
						dateProphyTesting=PIn.Date(tableDates.Rows[d]["procDate_"].ToString());
						//but patient could have both perio and prophy.
						//So must test to see if the date is newer
						if(dateProphyTesting>prevDateProphy) {
							prevDateProphy=dateProphyTesting;
						}
						break;
					}
				}
			}
			for(int i=0;i<typeList.Count;i++){//active types for this patient.
				if(RecallTriggers.GetForType(typeList[i].RecallTypeNum).Count==0) {
					//if no triggers for this recall type, then skip it.  Don't try to add or alter.
					continue;
				}
				//set prevDate:
				if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==typeList[i].RecallTypeNum
					|| PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==typeList[i].RecallTypeNum) 
				{
					prevDate=prevDateProphy;
				}
				else {
					prevDate=DateTime.MinValue;
					for(int d=0;d<tableDates.Rows.Count;d++) {//procs for patient
						if(tableDates.Rows[d]["RecallTypeNum"].ToString()==typeList[i].RecallTypeNum.ToString()) {
							prevDate=PIn.Date(tableDates.Rows[d]["procDate_"].ToString());
							break;
						}
					}
				}
				matchingRecall=null;
				for(int r=0;r<recallList.Count;r++){//recalls for patient
					if(recallList[r].RecallTypeNum==typeList[i].RecallTypeNum){
						matchingRecall=recallList[r];
						break;
					}
				}
				if(matchingRecall==null){//if there is no existing recall,
					if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==typeList[i].RecallTypeNum
						|| PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==typeList[i].RecallTypeNum
						|| prevDate.Year>1880)//for other types, if date is not minVal, then add a recall
					{
						//add a recall
						recallNew=new Recall();
						recallNew.RecallTypeNum=typeList[i].RecallTypeNum;
						recallNew.PatNum=patNum;
						recallNew.DatePrevious=prevDate;//will be min val for prophy/perio with no previous procs
						recallNew.RecallInterval=typeList[i].DefaultInterval;
						if(prevDate.Year<1880) {
							recallNew.DateDueCalc=DateTime.MinValue;
						}
						else {
							recallNew.DateDueCalc=prevDate+recallNew.RecallInterval;
						}
						recallNew.DateDue=recallNew.DateDueCalc;
						Recalls.Insert(recallNew);
						SecurityLogs.MakeLogEntry(Permissions.RecallEdit,recallNew.PatNum,"Recall added by Recall Synch.");
					}
				}
				else{//alter the existing recall
					Recall recallOld=matchingRecall.Copy();
					if(!matchingRecall.IsDisabled
						&& matchingRecall.DisableUntilBalance==0
						&& matchingRecall.DisableUntilDate.Year<1880
						&& prevDate.Year>1880//this protects recalls that were manually added as part of a conversion
						&& prevDate != matchingRecall.DatePrevious) 
					{//if datePrevious has changed, reset
						matchingRecall.RecallStatus=0;
						matchingRecall.Note="";
						matchingRecall.DateDue=matchingRecall.DateDueCalc;//now it is allowed to be changed in the steps below
					}
					if(prevDate.Year<1880){//if no previous date
						matchingRecall.DatePrevious=DateTime.MinValue;
						if(matchingRecall.DateDue==matchingRecall.DateDueCalc){//user did not enter a DateDue
							matchingRecall.DateDue=DateTime.MinValue;
						}
						matchingRecall.DateDueCalc=DateTime.MinValue;
						if(Recalls.Update(matchingRecall,recallOld)) {
							SecurityLogs.MakeLogEntry(Permissions.RecallEdit,matchingRecall.PatNum,"Recall updated by Recall Synch.");
						}
					}
					else{//if previous date is a valid date
						matchingRecall.DatePrevious=prevDate;
						if(matchingRecall.IsDisabled){//if the existing recall is disabled 
							matchingRecall.DateDue=DateTime.MinValue;//DateDue is always blank
						}
						else{//but if not disabled
							if(matchingRecall.DateDue==matchingRecall.DateDueCalc//if user did not enter a DateDue
								|| matchingRecall.DateDue.Year<1880)//or DateDue was blank
							{
								matchingRecall.DateDue=matchingRecall.DatePrevious+matchingRecall.RecallInterval;//set same as DateDueCalc
							}
						}
						matchingRecall.DateDueCalc=matchingRecall.DatePrevious+matchingRecall.RecallInterval;
						if(Recalls.Update(matchingRecall,recallOld)) {
							SecurityLogs.MakeLogEntry(Permissions.RecallEdit,matchingRecall.PatNum,"Recall updated by Recall Synch.");
						}
					}
				}
			}
			//now, we need to loop through all the inactive recall types and clear the DateDueCalc
			//We don't do this anymore. User must explicitly delete recalls, either one-by-one, or from the recall type window.
			/*
			List<RecallType> typeListInactive=RecallTypes.GetInactive();
			for(int i=0;i<typeListInactive.Count;i++){
				matchingRecall=null;
				for(int r=0;r<recallList.Count;r++){
					if(recallList[r].RecallTypeNum==typeListInactive[i].RecallTypeNum){
						matchingRecall=recallList[r];
					}
				}
				if(matchingRecall==null){//if there is no existing recall,
					continue;
				}
				Recalls.Delete(matchingRecall);//we'll just delete it
				//There is an existing recall, so alter it if certain conditions are met
				//matchingRecall.DatePrevious=DateTime.MinValue;
				//if(matchingRecall.DateDue==matchingRecall.DateDueCalc){//if user did not enter a DateDue
					//we can safely alter the DateDue
				//	matchingRecall.DateDue=DateTime.MinValue;
				//}
				//matchingRecall.DateDueCalc=DateTime.MinValue;
				//Recalls.Update(matchingRecall);
			}*/
		}

		/// <summary>Returns true if the patNum has a past due recall in the list that matches the types.</summary>
		public static bool IsPatientPastDue(long patNum,DateTime aptDateTime,bool isProphyOrPerio,List<Recall> listPastDueRecalls) {
			//No need to check RemotingRole; no call to db.
			if(listPastDueRecalls==null) {
				return false;
			}
			return listPastDueRecalls.Where(x => x.PatNum==patNum)
				.Where(x => x.DateDue.Date<DateTime.Today.Date && x.DateDue.Date.Year>1880)
				.Where(x => x.DateScheduled.Year<1880 || x.DateScheduled.Date>aptDateTime.Date)
				.Where(x => isProphyOrPerio==ListTools.In(x.RecallTypeNum,RecallTypes.ProphyType,RecallTypes.PerioType))
				.Any();
		}

		///<summary>Synchronizes DateScheduled column in recall table for one patient.  
		///This must be used instead of lazy synch in RecallsForPatient, when deleting an appointment, when sending to unscheduled list, setting an appointment complete, etc.  
		///This is fast, but it would be inefficient to call it too much.</summary>
		public static void SynchScheduledApptFull(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			Patient pat=Patients.GetPat(patNum);
			if(pat.PatStatus==PatientStatus.Archived) {//do not calculate recall if patient is archived.
				return;
			}
			//Clear out DateScheduled column for this pat before changing
			string command="UPDATE recall "
				+"SET recall.DateScheduled="+POut.Date(DateTime.MinValue)+" "
				+"WHERE recall.PatNum="+POut.Long(patNum);
			Db.NonQ(command);
			//Get table of future appointments dates with recall type for this patient, where a procedure is attached that is a recall trigger procedure
			command="SELECT recalltrigger.RecallTypeNum,MIN("+DbHelper.DtimeToDate("appointment.AptDateTime")+") AS AptDateTime "
				+"FROM procedurelog "
				+"INNER JOIN recalltrigger ON procedurelog.CodeNum=recalltrigger.CodeNum "
				+"INNER JOIN recall ON recalltrigger.RecallTypeNum=recall.RecallTypeNum "
					+"AND recall.PatNum="+POut.Long(patNum)+" "
				+"INNER JOIN appointment ON appointment.AptNum=procedurelog.AptNum "
					+"AND appointment.PatNum="+POut.Long(patNum)+" "
					+"AND appointment.AptStatus="+POut.Int((int)ApptStatus.Scheduled)+" "
					+"AND appointment.AptDateTime > "+DbHelper.Curdate()+" "//early this morning
				+"WHERE procedurelog.PatNum="+POut.Long(patNum)+" "
				+"GROUP BY recalltrigger.RecallTypeNum";
			DataTable table=Db.GetTable(command);
			//Update the recalls for this patient with DATE(AptDateTime) where there is a future appointment with recall proc on it
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["RecallTypeNum"].ToString()=="") {
					continue;
				}
				command=@"UPDATE recall	SET recall.DateScheduled="+POut.Date(PIn.Date(table.Rows[i]["AptDateTime"].ToString()))+" " 
					+"WHERE recall.RecallTypeNum="+POut.Long(PIn.Long(table.Rows[i]["RecallTypeNum"].ToString()))+" "
					+"AND recall.PatNum="+POut.Long(patNum)+" ";
				Db.NonQ(command);
			}
		}

		///<summary>Updates RecallInterval and DueDate for all patients that have the recallTypeNum and defaultIntervalOld to use the defaultIntervalNew.</summary>
		public static void UpdateDefaultIntervalForPatients(long recallTypeNum,Interval defaultIntervalOld,Interval defaultIntervalNew) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),recallTypeNum,defaultIntervalOld,defaultIntervalNew);
				return;
			}
			string command="SELECT * FROM recall WHERE IsDisabled=0 AND RecallTypeNum="+POut.Long(recallTypeNum)+" AND RecallInterval="+POut.Int(defaultIntervalOld.ToInt());
			List<Recall> recallList=Crud.RecallCrud.SelectMany(command);
			for(int i=0;i<recallList.Count;i++) {
				if(recallList[i].DateDue!=recallList[i].DateDueCalc) {//User entered a DueDate.
					//Don't change the DateDue since user already overrode it
				}
				else{
					recallList[i].DateDue=recallList[i].DatePrevious+defaultIntervalNew;
				}
				recallList[i].DateDueCalc=recallList[i].DatePrevious+defaultIntervalNew;
				recallList[i].RecallInterval=defaultIntervalNew;
				Update(recallList[i]);
				SecurityLogs.MakeLogEntry(Permissions.RecallEdit,recallList[i].PatNum,"Recall interval updated to Recall Type default interval.");
			}
		}

		public static void DeleteAllOfType(long recallTypeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),recallTypeNum);
				return;
			}
			string command="DELETE FROM recall WHERE RecallTypeNum= "+POut.Long(recallTypeNum);
			Db.NonQ(command);
		}

		///<summary>Shared table structure for Recalls and Reactivations, be careful when making changes.</summary>
		public static DataTable GetAddrTableStructure() {
			DataTable table=new DataTable();
			table.Columns.Add("address");//includes address2. Can be guar.
			table.Columns.Add("City");//Can be guar.
			table.Columns.Add("clinicNum");//will be the guar clinicNum if grouped.
			table.Columns.Add("dateDue");
			table.Columns.Add("email");//Will be guar if grouped by family
			table.Columns.Add("emailPatNum");//Will be guar if grouped by family
			table.Columns.Add("famList");
			table.Columns.Add("guarLName");
			table.Columns.Add("numberOfReminders");//for a family, this will be the max for the family
			table.Columns.Add("patientNameF");//Only used when single email
			table.Columns.Add("patientNameFL");
			table.Columns.Add("patNums");//Comma delimited.  Used in email.
			table.Columns.Add("recallNums");//Comma delimited.  Used during e-mail and eCards
			table.Columns.Add("State");//Can be guar.
			table.Columns.Add("Zip");//Can be guar.
			return table;
		}

		///<summary></summary>
		public static DataTable GetAddrTable(List<long> recallNums,bool groupByFamily,RecallListSort sortBy) {
			//No need to check RemotingRole; no call to db.
			DataTable rawTable=GetAddrTableRaw(recallNums);
			List<DataRow> rawRows=new List<DataRow>();
			for(int i=0;i<rawTable.Rows.Count;i++){
				rawRows.Add(rawTable.Rows[i]);
			}
			RecallComparer comparer=new RecallComparer();
			comparer.GroupByFamilies=groupByFamily;
			comparer.SortBy=sortBy;
			rawRows.Sort(comparer);
			DataTable table=GetAddrTableStructure();
			string familyAptList="";
			string recallNumStr="";
			string patNumStr="";
			DataRow row;
			List<DataRow> rows=new List<DataRow>();
			int maxNumReminders=0;
			int maxRemindersThisPat;
			Patient pat;
			for(int i=0;i<rawRows.Count;i++) {
				if(!groupByFamily) {
					row=table.NewRow();
					row["address"]=rawRows[i]["Address"].ToString();
					if(rawRows[i]["Address2"].ToString()!="") {
						row["address"]+="\r\n"+rawRows[i]["Address2"].ToString();
					}
					row["City"]=rawRows[i]["City"].ToString();
					row["clinicNum"]=rawRows[i]["ClinicNum"].ToString();
					row["dateDue"]=PIn.Date(rawRows[i]["DateDue"].ToString()).ToShortDateString();
					//since not grouping by family, this is always just the patient email
					row["email"]=rawRows[i]["Email"].ToString();
					row["emailPatNum"]=rawRows[i]["PatNum"].ToString();
					row["famList"]="";
					row["guarLName"]=rawRows[i]["guarLName"].ToString();//even though we won't use it.
					row["numberOfReminders"]=PIn.Long(rawRows[i]["numberOfReminders"].ToString()).ToString();
					pat=new Patient();
					pat.LName=rawRows[i]["LName"].ToString();
					pat.FName=rawRows[i]["FName"].ToString();
					pat.Preferred=rawRows[i]["Preferred"].ToString();
					row["patientNameF"]=pat.GetNameFirstOrPreferred();
					row["patientNameFL"]=pat.GetNameFLnoPref();// GetNameFirstOrPrefL();
					row["patNums"]=rawRows[i]["PatNum"].ToString();
					row["recallNums"]=rawRows[i]["RecallNum"].ToString();
					row["State"]=rawRows[i]["State"].ToString();
					row["Zip"]=rawRows[i]["Zip"].ToString();
					rows.Add(row);
					continue;
				}
				//groupByFamily----------------------------------------------------------------------
				if(familyAptList==""){//if this is the first patient in the family
					maxNumReminders=0;
					//loop through the whole family, and determine the maximum number of reminders
					for(int f=i;f<rawRows.Count;f++) {
						maxRemindersThisPat=PIn.Int(rawRows[f]["numberOfReminders"].ToString());
						if(maxRemindersThisPat>maxNumReminders) {
							maxNumReminders=maxRemindersThisPat;
						}
						if(f==rawRows.Count-1//if this is the last row
							|| rawRows[i]["Guarantor"].ToString()!=rawRows[f+1]["Guarantor"].ToString())//or if the guarantor on next line is different
						{
							break;
						}
					}
					//now we know the max number of reminders for the family
					if(i==rawRows.Count-1//if this is the last row
						|| rawRows[i]["Guarantor"].ToString()!=rawRows[i+1]["Guarantor"].ToString())//or if the guarantor on next line is different
					{
						//then this is a single patient, and there are no other family members in the list.
						row=table.NewRow();
						row["address"]=rawRows[i]["Address"].ToString();
						if(rawRows[i]["Address2"].ToString()!="") {
							row["address"]+="\r\n"+rawRows[i]["Address2"].ToString();
						}
						row["City"]=rawRows[i]["City"].ToString();
						row["State"]=rawRows[i]["State"].ToString();
						row["Zip"]=rawRows[i]["Zip"].ToString();
						row["clinicNum"]=rawRows[i]["ClinicNum"].ToString();
						row["dateDue"]=PIn.Date(rawRows[i]["DateDue"].ToString()).ToShortDateString();
						//this will always be the guarantor email
						row["email"]=rawRows[i]["guarEmail"].ToString();
						row["emailPatNum"]=rawRows[i]["Guarantor"].ToString();
						row["famList"]="";
						row["guarLName"]=rawRows[i]["guarLName"].ToString();//even though we won't use it.
						row["numberOfReminders"]=maxNumReminders.ToString();
						//if(rawRows[i]["Preferred"].ToString()=="") {
						row["patientNameF"]=rawRows[i]["FName"].ToString();
						//}
						//else {
						//	row["patientNameF"]=rawRows[i]["Preferred"].ToString();
						//}
						row["patientNameFL"]=rawRows[i]["FName"].ToString()+" "
							+rawRows[i]["MiddleI"].ToString()+" "
							+rawRows[i]["LName"].ToString();
						row["patNums"]=rawRows[i]["PatNum"].ToString();
						row["recallNums"]=rawRows[i]["RecallNum"].ToString();
						rows.Add(row);
						continue;
					}
					else{//this is the first patient of a family with multiple family members
						familyAptList=rawRows[i]["FName"].ToString()+":  "
							+PIn.Date(rawRows[i]["DateDue"].ToString()).ToShortDateString();
						patNumStr=rawRows[i]["PatNum"].ToString();
						recallNumStr=rawRows[i]["RecallNum"].ToString();
						continue;
					}
				}
				else{//not the first patient
					familyAptList+="\r\n"+rawRows[i]["FName"].ToString()+":  "
						+PIn.Date(rawRows[i]["DateDue"].ToString()).ToShortDateString();
					patNumStr+=","+rawRows[i]["PatNum"].ToString();
					recallNumStr+=","+rawRows[i]["RecallNum"].ToString();
				}
				if(i==rawRows.Count-1//if this is the last row
					|| rawRows[i]["Guarantor"].ToString()!=rawRows[i+1]["Guarantor"].ToString())//or if the guarantor on next line is different
				{
					//This part only happens for the last family member of a grouped family
					row=table.NewRow();
					row["address"]=rawRows[i]["guarAddress"].ToString();
					if(rawRows[i]["guarAddress2"].ToString()!="") {
						row["address"]+="\r\n"+rawRows[i]["guarAddress2"].ToString();
					}
					row["City"]=rawRows[i]["guarCity"].ToString();
					row["State"]=rawRows[i]["guarState"].ToString();
					row["Zip"]=rawRows[i]["guarZip"].ToString();
					row["clinicNum"]=rawRows[i]["guarClinicNum"].ToString();
					row["dateDue"]=PIn.Date(rawRows[i]["DateDue"].ToString()).ToShortDateString();
					row["email"]=rawRows[i]["guarEmail"].ToString();
					row["emailPatNum"]=rawRows[i]["Guarantor"].ToString();
					row["famList"]=familyAptList;
					row["guarLName"]=rawRows[i]["guarLName"].ToString();
					row["numberOfReminders"]=maxNumReminders.ToString();
					row["patientNameF"]="";//not used here
					row["patientNameFL"]="";//we won't use this
					row["patNums"]=patNumStr;
					row["recallNums"]=recallNumStr;
					rows.Add(row);
					familyAptList="";
				}	
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary></summary>
		public static DataTable GetAddrTableForWebSched(List<long> recallNums,bool groupByFamily,RecallListSort sortBy) {
			//No need to check RemotingRole; no call to db.
			DataTable rawTable=GetAddrTableRaw(recallNums);
			List<long> listRecallNumsUnsent=WebSchedRecalls.GetAllUnsent().Select(x => x.RecallNum).ToList();
			List<DataRow> rawRows=new List<DataRow>();
			for(int i=0;i<rawTable.Rows.Count;i++) {
				if(listRecallNumsUnsent.Contains(PIn.Long(rawTable.Rows[i]["RecallNum"].ToString()))) {
					continue;//We do not need to insert a WebSchedRecall if one already exists for this RecallNum.
				}
				rawRows.Add(rawTable.Rows[i]);
			}
			RecallComparer comparer=new RecallComparer();
			comparer.GroupByFamilies=groupByFamily;
			comparer.SortBy=sortBy;
			rawRows.Sort(comparer);
			DataTable table=new DataTable();
			table.Columns.Add("clinicNum");//will be the guar clinicNum if grouped.
			table.Columns.Add("dateDue");
			table.Columns.Add("email");//will be guar if grouped by family
			table.Columns.Add("emailPatNum");//will be guar if grouped by family
			table.Columns.Add("numberOfReminders");//for a family, this will be the max for the family
			table.Columns.Add("patientNameF");
			table.Columns.Add("patientNameFL");
			table.Columns.Add("PatNum");
			table.Columns.Add("RecallNum");
			table.Columns.Add("PreferRecallMethod");
			DataRow row;
			List<DataRow> rows=new List<DataRow>();
			Patient pat;
			for(int i=0;i<rawRows.Count;i++) {
				row=table.NewRow();
				if(groupByFamily) {
					//Use guarantors clinic and email for all notifications.
					row["clinicNum"]=rawRows[i]["guarClinicNum"].ToString();
					row["email"]=rawRows[i]["guarEmail"].ToString();
					row["emailPatNum"]=rawRows[i]["Guarantor"].ToString();
				}
				else {
					row["clinicNum"]=rawRows[i]["ClinicNum"].ToString();
					row["email"]=rawRows[i]["Email"].ToString();
					row["emailPatNum"]=rawRows[i]["PatNum"].ToString();
				}
				row["dateDue"]=PIn.Date(rawRows[i]["DateDue"].ToString()).ToShortDateString();
				row["numberOfReminders"]=PIn.Long(rawRows[i]["numberOfReminders"].ToString()).ToString();
				row["PatNum"]=rawRows[i]["PatNum"].ToString();
				pat=new Patient();
				pat.LName=rawRows[i]["LName"].ToString();
				pat.FName=rawRows[i]["FName"].ToString();
				pat.Preferred=rawRows[i]["Preferred"].ToString();
				row["patientNameF"]=pat.GetNameFirstOrPreferred();
				row["patientNameFL"]=pat.GetNameFLnoPref();
				row["RecallNum"]=rawRows[i]["RecallNum"].ToString();
				row["PreferRecallMethod"]=rawRows[i]["PreferRecallMethod"].ToString();
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Gets a base table used for creating recall reminders.</summary>
		public static DataTable GetAddrTableRaw(List<long> recallNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),recallNums);
			}
			string command=@$"SELECT patient.Address,patguar.Address guarAddress,COALESCE(definition.ItemName,'') billingType,
				patient.Address2,patguar.Address2 guarAddress2,patient.City,patguar.City guarCity,patient.ClinicNum,patguar.ClinicNum guarClinicNum,
				recall.DateDue,patient.Email,patguar.Email guarEmail,patient.FName,patguar.FName guarFName,patient.Guarantor,
				patient.LName,patguar.LName guarLName,t.maxDateDue,patient.MiddleI,COUNT(commlog.CommlogNum) numberOfReminders,patient.PatNum,
				patient.Preferred,recall.RecallNum,patient.State,patguar.State guarState,patient.Zip,patguar.Zip guarZip,patguar.PreferRecallMethod
				FROM recall 
				INNER JOIN patient ON patient.PatNum=recall.PatNum 
				INNER JOIN patient patguar ON patient.Guarantor=patguar.PatNum
				LEFT JOIN definition ON definition.DefNum=patient.BillingType
					AND definition.Category={POut.Int((int)DefCat.BillingTypes)}
				LEFT JOIN commlog ON commlog.PatNum=recall.PatNum
					AND commlog.CommType={POut.Long(Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL))}
					AND commlog.CommDateTime > recall.DatePrevious
				LEFT JOIN (
					SELECT patient.Guarantor,MAX(recall.DateDue) maxDateDue
					FROM patient
					INNER JOIN recall ON patient.PatNum=recall.PatNum
					WHERE recall.RecallNum IN ({string.Join(",",recallNums.Select(x => POut.Long(x)))})
					GROUP BY patient.Guarantor
				) t ON t.Guarantor=patient.Guarantor
				WHERE recall.RecallNum IN ({string.Join(",",recallNums.Select(x => POut.Long(x)))})
				GROUP BY recall.RecallNum";
			return Db.GetTable(command);
		}

		/// <summary></summary>
		public static void UpdateStatus(long recallNum,long newStatus) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),recallNum,newStatus);
				return;
			}
			string command="UPDATE recall SET RecallStatus="+newStatus.ToString()
				+" WHERE RecallNum="+recallNum.ToString();
			Db.NonQ(command);
		}

		public static int GetCountForType(long recallTypeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),recallTypeNum);
			}
			string command="SELECT COUNT(*) FROM recall "
				+"JOIN recalltype ON recall.RecallTypeNum=recalltype.RecallTypeNum "
				+"WHERE recalltype.recallTypeNum="+POut.Long(recallTypeNum);
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Return RecallNums that have changed since a paticular time. </summary>
		public static List<long> GetChangedSinceRecallNums(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT RecallNum FROM recall WHERE DateTStamp > "+POut.DateT(changedSince);
			DataTable dt=Db.GetTable(command);
			List<long> recallnums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				recallnums.Add(PIn.Long(dt.Rows[i]["RecallNum"].ToString()));
			}
			return recallnums;
		}

		///<summary>Returns recalls with given list of RecallNums. Used along with GetChangedSinceRecallNums.</summary>
		public static List<Recall> GetMultRecalls(List<long> listRecallNums) {
			if(listRecallNums.IsNullOrEmpty()) {
				return new List<Recall>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Recall>>(MethodBase.GetCurrentMethod(),listRecallNums);
			}
			string command=$"SELECT * FROM recall WHERE RecallNum IN ({string.Join(",",listRecallNums)})";
			return Crud.RecallCrud.SelectMany(command);
		}

		///<summary>Gets the patients that have had a recall reminder sent to them in the date range. If a recall reminder was recorded as a commlog
		///without a row in the webschedrecall table, some fields will be blank.</summary>
		public static List<RecallRecent> GetRecentRecalls(DateTime dateTimeFrom,DateTime dateTimeTo,List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RecallRecent>>(MethodBase.GetCurrentMethod(),dateTimeFrom,dateTimeTo,listClinicNums);
			}
			const string lanThis="FormRecallList";
			string command=@"
				SELECT "+DbHelper.Concat("patient.LName","', '","patient.FName")+@" PatientName,patient.PatNum,recallreminder.DateSent,
				recallreminder.CommMode,patient.BirthDate,COALESCE(recalltype.Description,'') RecallType,COALESCE(definition.ItemName,'') RecallStatus,
				recall.DateDue,(CASE WHEN recallreminder.ClinicNum=-1 THEN patient.ClinicNum ELSE recallreminder.ClinicNum END) ClinicNum,recall.RecallNum,
				COALESCE(recallreminder.CommSource,'') CommSource
				FROM (
					SELECT webschedrecall.DateTimeReminderSent DateSent,webschedrecall.PatNum,webschedrecall.RecallNum,
					(CASE WHEN webschedrecall.Source=1 THEN -1 ELSE -2 END) CommMode,webschedrecall.ClinicNum,"
						+@$"'{POut.Long((long)CommItemSource.WebSched)}'as CommSource 
					FROM webschedrecall
					WHERE "+DbHelper.BetweenDates("webschedrecall.DateTimeReminderSent",dateTimeFrom,dateTimeTo)+@"
					UNION ALL
					SELECT commlog.CommDateTime DateSent,commlog.PatNum,0 RecallNum,commlog.Mode_ CommMode,-1 ClinicNum,commlog.CommSource
					FROM commlog
					WHERE "+DbHelper.BetweenDates("commlog.CommDateTime",dateTimeFrom,dateTimeTo)+@"
					AND commlog.CommType="+POut.Long(Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL))+@"
				) recallreminder
				INNER JOIN patient ON patient.PatNum=recallreminder.PatNum
				LEFT JOIN recall ON recall.RecallNum=recallreminder.RecallNum
				LEFT JOIN recalltype ON recalltype.RecallTypeNum=recall.RecallTypeNum
				LEFT JOIN definition ON definition.DefNum=recall.RecallStatus
				";
			if(listClinicNums.Count > 0) {
				command+="HAVING ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+" )";
			}
			DataTable table=Db.GetTable(command);
			List<RecallRecent> listRecent=new List<RecallRecent>();
			foreach(DataRow row in table.Rows) {
				RecallRecent recent=new RecallRecent {
					DateSent=PIn.DateT(row["DateSent"].ToString()),
					PatientName=PIn.String(row["PatientName"].ToString()),
					PatNum=PIn.Long(row["PatNum"].ToString()),
					Age=Patients.DateToAge(PIn.Date(row["BirthDate"].ToString())),
					RecallType=PIn.String(row["RecallType"].ToString()),
					RecallStatus=PIn.String(row["RecallStatus"].ToString()),
					DueDate=PIn.DateT(row["DateDue"].ToString()),
					RecallNum=PIn.Long(row["RecallNum"].ToString()),
					CommSource=PIn.Enum<CommItemSource>(row["CommSource"].ToString())
				};
				switch(PIn.Int(row["CommMode"].ToString())) {
					case -2:
						recent.ReminderType=Lans.g(lanThis,"Automatic Web Sched Recall");
						break;
					case -1:
						recent.ReminderType=Lans.g(lanThis,"Manual Web Sched Recall");
						break;
					default:
						try {
							recent.ReminderType=Lans.g(lanThis,PIn.Enum<CommItemMode>(PIn.Int(row["CommMode"].ToString())).GetDescription());
						}
						catch(Exception ex) {
							ex.DoNothing();
							recent.ReminderType=Lans.g(lanThis,"UNKNOWN");
						}
						break;
				}
				listRecent.Add(recent);
			}
			listRecent=listRecent
				//Group by patient and date sent,then by commsource which combines commlogs that were a result of a web sched recall
				.GroupBy(x => new { x.PatNum,x.DateSent.Date,x.CommSource })
				//If there are multiple for the same patient and date sent, choose the one with the latest Due Date. Prefer ones that have the recall info.
				.Select(x => x.OrderBy(y => y.RecallNum > 0).ThenBy(y => y.DueDate).Last())				
				.OrderBy(x => x.DateSent).ThenBy(x => x.PatientName).ToList();
			return listRecent;
		}

		#region Web Sched

		///<summary>Insert WebSchedRecalls so that the Auto Comm Web Sched thread can aggregate the recalls and send emails and texts.
		///Returns a list of errors to display to the user if anything went wrong otherwise returns empty list if everything was successful.</summary>
		public static List<string> PrepWebSchedNotifications(List<long> listRecallNums,bool isGroupFamily,RecallListSort sortBy,
			WebSchedRecallSource source,EmailAddress emailAddressOverride=null,HashSet<long> hashSetDontSendEmail=null,
			HashSet<long> hashSetDontSendSms=null) 
		{
			//No need to check RemotingRole; no call to db.
			List<string> listErrors=new List<string>();
			if(listRecallNums==null || listRecallNums.Count < 1) {
				listErrors.Add(Lans.g("WebSched","No Recalls to schedule"));
				return listErrors;
			}
			//Loop through the selected patients and insert WebSchedRecalls so that the Auto Comm Web Sched thread can aggregate the recalls and send
			//messages.
			DataTable addrTable=Recalls.GetAddrTableForWebSched(listRecallNums,isGroupFamily,sortBy);
			for(int i=0;i<addrTable.Rows.Count;i++) {
				#region Insert WebSchedRecall
				WebSchedRecall wsRecall=new WebSchedRecall();
				wsRecall.RecallNum=PIn.Long(addrTable.Rows[i]["RecallNum"].ToString());
				wsRecall.ClinicNum=PIn.Long(addrTable.Rows[i]["ClinicNum"].ToString());
				wsRecall.PatNum=PIn.Long(addrTable.Rows[i]["PatNum"].ToString());
				wsRecall.ReminderCount=PIn.Int(addrTable.Rows[i]["numberOfReminders"].ToString());		
				//It is common for offices to have paitents with a blank recall date (they've never had a recall performed at the office).
				//Instead of showing 01/01/0001 in the email, we will simply show today's date because that is what the Web Sched time slots will start showing.
				wsRecall.DateDue=PIn.Date(addrTable.Rows[i]["dateDue"].ToString());
				if(wsRecall.DateDue.Year < 1880) {
					wsRecall.DateDue=DateTime.Today;
				}
				wsRecall.DateTimeReminderSent=DateTime.MinValue;
				wsRecall.PreferRecallMethod=(ContactMethod)PIn.Int(addrTable.Rows[i]["PreferRecallMethod"].ToString());
				if(hashSetDontSendEmail != null && hashSetDontSendEmail.Contains(wsRecall.RecallNum)) {
					wsRecall.EmailSendStatus=AutoCommStatus.DoNotSend;
				}
				else {
					wsRecall.EmailSendStatus=AutoCommStatus.SendNotAttempted;
				}
				if(hashSetDontSendSms != null && hashSetDontSendSms.Contains(wsRecall.RecallNum)) {
					wsRecall.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				else {
					wsRecall.SmsSendStatus=AutoCommStatus.SendNotAttempted;
				}
				wsRecall.Source=source;
				//todo: Convert to InsertMany().
				WebSchedRecalls.Insert(wsRecall);
				#endregion
			}
			return listErrors;
		}

		///<summary>Creates and inserts an appointment for the recall passed in using the dateStart hour as the beginning of the appointment.
		///It will be scheduled in the first available operatory.
		///<para>The first available operatory is determined by the order in which they are stored in the database (operatory.ItemOrder).</para>
		///<para>This means that (visually to the user) we will be filling up their appointment schedule from the left to the right.</para>
		///<para>Surround with a try catch.  Throws exceptions if anything goes wrong.</para>
		///<para>Returns the list of procedures that were scheduled and the appointment created.</para></summary>
		///<param name="isASAP">If true, then the appointment created will have a priority of ASAP.</param>
		public static Tuple<Appointment,List<Procedure>> CreateRecallApptForWebSched(long recallNum,DateTime dateStart,DateTime dateEnd
			,List<TimeSlot> listAvailableTimeSlots,LogSources source,bool isASAP=false,bool sendVerification=false,string logGuid="") 
		{
			//No need to check RemotingRole; no call to db.
			foreach(TimeSlot timeSlot in listAvailableTimeSlots) {
				if(dateStart!=timeSlot.DateTimeStart || dateEnd!=timeSlot.DateTimeStop) {
					continue;//Not the available slot that the patient selected within the app.
				}
				//At this point we know the slot time that the patient selected matches this open time slot.
				Recall recallCur=Recalls.GetRecall(recallNum);
				if(recallCur==null) {
					throw new ODException("This recall appointment is no longer available.\r\nPlease call us to schedule your appointment.");
				}
				Patient patCur=Patients.GetPat(recallCur.PatNum);
				List<Recall> listRecalls=Recalls.GetList(patCur.PatNum);
				for(int j=0;j<listRecalls.Count;j++) {
					if(listRecalls[j].RecallNum==recallNum) {
						recallCur=listRecalls[j].Copy();
						break;
					}
				}
				Appointment aptCur=new Appointment();
				aptCur.AptDateTime=dateStart; //set the AptDateTime here, so FillAppointmentforRecall uses the correct Date for the procedure
				Family fam=Patients.GetFamily(patCur.PatNum);
				List<Procedure> procList=Procedures.Refresh(patCur.PatNum);
				List<InsSub> listSubs=InsSubs.RefreshForFam(fam);
				List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
				List<string> listProcStrs=RecallTypes.GetProcs(recallCur.RecallTypeNum);
				//Now we need to completely fill the appointment with procedures, claimprocs, etc. for this specific recall.
				List<Procedure> listProcedures=Appointments.FillAppointmentForRecall(aptCur,recallCur,listRecalls,patCur,listProcStrs,listPlans,listSubs);
				Appointment aptOld=aptCur.Copy();
				//Take the recall appointment that was just inserted via FillAppointmentForRecall() and update the time and operatory.
				Operatory opCur=Operatories.GetOperatory(timeSlot.OperatoryNum);
				aptCur.AptStatus=ApptStatus.Scheduled;
				aptCur.Op=opCur.OperatoryNum;
				aptCur.Priority=isASAP ? ApptPriority.ASAP : ApptPriority.Normal;
				aptCur.Confirmed=PrefC.GetLong(PrefName.WebSchedRecallConfirmStatus);
				//Make sure that operatory specific settings are applied to the appointment.
				List<Schedule> listSchedules=Schedules.RefreshDayEdit(aptCur.AptDateTime);
				if(!PrefC.HasClinicsEnabled) {
					aptCur.ClinicNum=0;
				}
				else if(opCur.ClinicNum==0) {
					aptCur.ClinicNum=patCur.ClinicNum;
				}
				else {
					aptCur.ClinicNum=opCur.ClinicNum;
				}
				WebSchedProviderRules rule;
				if(PrefC.HasClinicsEnabled) {
					rule=PIn.Enum<WebSchedProviderRules>(
						ClinicPrefs.GetPref(PrefName.WebSchedProviderRule,aptCur.ClinicNum)?.ValueString??PrefC.GetString(PrefName.WebSchedProviderRule));
				}
				else {
					rule=PIn.Enum<WebSchedProviderRules>(PrefC.GetString(PrefName.WebSchedProviderRule));
				}
				long preferredProvNum=0;
				if(rule!=WebSchedProviderRules.FirstAvailable) {
					//If the recall is supposed to be with a particular provider, then the listAvailableTimeSlots should all have a ProvNum of that 
					//particular provider.
					preferredProvNum=timeSlot.ProvNum;
				}
				long assignedDent=Schedules.GetAssignedProvNumForSpot(listSchedules,opCur,false,aptCur.AptDateTime,preferredProvNum);
				long assignedHyg=Schedules.GetAssignedProvNumForSpot(listSchedules,opCur,true,aptCur.AptDateTime,preferredProvNum);
				if(assignedDent > 0) {//if no dentist is assigned to op, then keep the original dentist.  All appts must have prov.
					aptCur.ProvNum=assignedDent;
				}
				if(assignedHyg > 0) {
					aptCur.ProvHyg=assignedHyg;
				}
				aptCur.IsHygiene=opCur.IsHygiene;
				//Note: We do not need to do any prospective operatory checks here because the query currently excludes prospective ops.
				//Also, aptCur already has the correct time pattern set.  No need to set it again here.
				Appointments.Update(aptCur,aptOld);
				//At this point, the appointment has been fully scheduled. The remaining operations can be run on a thread so that this method can return 
				//faster.
				ODThread thread=new ODThread(o => {
					eServiceCode eServiceCode=eServiceCode.WebSched;
					if(source==LogSources.WebSchedASAP) {
						eServiceCode=eServiceCode.WebSchedASAP;
					}
					if(recallCur.Priority==RecallPriority.ASAP) {
						Recall recallOld=recallCur.Copy();
						recallCur.Priority=RecallPriority.Normal;
						if(Recalls.Update(recallCur,recallOld)) {
							SecurityLogs.MakeLogEntry(Permissions.RecallEdit,recallCur.PatNum,"Recall priority changed to Normal by Web Sched.");
							EServiceLogs.MakeLogEntry(eServiceAction.WSMovedAppt,eServiceType.WSAsap,FKeyType.ApptNum,patNum:aptCur.PatNum,FKey:aptCur.AptNum,clinicNum:aptCur.ClinicNum,logGuid:logGuid);
						}
					}
					//Create a security log so that the office knows where this appointment came from.
					SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,aptCur.PatNum,
						aptCur.AptDateTime.ToString()+", "+aptCur.ProcDescript+"  -  Created via Web Sched",
						aptCur.AptNum,source,aptOld.DateTStamp);
					EServiceLogs.MakeLogEntry(eServiceAction.WSAppointmentScheduledFromServer,eServiceType.WSRecall,FKeyType.ApptNum,patNum:aptCur.PatNum,
						FKey:aptCur.AptNum,clinicNum:aptCur.ClinicNum,logGuid:logGuid);
					if(sendVerification) {
						Appointments.SendWebSchedNotify(aptCur,PrefName.WebSchedVerifyRecallType,PrefName.WebSchedVerifyRecallText,
							PrefName.WebSchedVerifyRecallEmailSubj,PrefName.WebSchedVerifyRecallEmailBody,PrefName.WebSchedVerifyRecallEmailTemplateType);
					}
					//There is no need to make security logs for anything other than the appointment.  That is how the recall list system currently does it.
					Recalls.SynchScheduledApptFull(aptCur.PatNum);//Synch the recalls so that the appointment will disappear from the recall list.
					AlertItem alert=new AlertItem() {
						ClinicNum=aptCur.ClinicNum,
						Description=aptCur.AptDateTime.ToString(),
						Type=AlertType.WebSchedRecallApptCreated,
						Actions=ActionType.MarkAsRead|ActionType.OpenForm|ActionType.Delete,
						FormToOpen=FormType.FormWebSchedAppts,
						Severity=SeverityType.Low,
						FKey=aptCur.AptNum
					};
					AlertItems.Insert(alert);
				});
				thread.Name="FinishWebSchedRecallAppt";
				thread.AddExceptionHandler(e => e.DoNothing());
				thread.Start(true);
				if(RunWebSchedSynchronously) {
					thread.Join(Timeout.Infinite);
				}
				return new Tuple<Appointment,List<Procedure>>(aptCur,listProcedures);
			}
			//It is very possible that from the time the patient loaded the Web Sched app and now that the available time slot has been removed or filled.
			throw new ODException("The selected appointment time is no longer available.\r\nPlease choose a different time slot.",100);
		}
		#endregion
		
		///<summary>List of recalls, dictionary of last completed dates for recall code nums, and dictionary of next scheduled dates for recall types for
		///one patient.  This will be added to a dictionary with key=PatNum for the patient whose data this represents.</summary>
		///[Serializable] //Change dicts to serializable dicts if this needs to be serialized
		private class PatBatchData {
			public List<Recall> ListRecalls=new List<Recall>();
			///<summary>CodeNum to a last completed date.</summary>
			public Dictionary<long,DateTime> DictLastProcDates=new Dictionary<long, DateTime>();
			///<summary>RecallTypeNum to scheduled date.</summary>
			public Dictionary<long,DateTime> DictRecallTypesSched=new Dictionary<long, DateTime>();
		}


		public class RecallRecent {
			public long PatNum;
			///<summary>May be 0 if this is from a commlog.</summary>
			public long RecallNum;
			public DateTime DateSent;
			public string ReminderType;
			public DateTime DueDate;
			public string RecallType;
			public string RecallStatus;
			public string PatientName;
			public int Age;
			public CommItemSource CommSource; 
		}
	}

	///<summary>The supplied DataRows must include the following columns: 
	///Guarantor, PatNum, guarLName, guarFName, LName, FName, DateDue, maxDateDue, billingType.  
	///maxDateDue is the most recent DateDue for all family members in the list and needs to be the same for all family members.  
	///This date will be used for better grouping.</summary>
	class RecallComparer:IComparer<DataRow> {
		public bool GroupByFamilies;
		///<summary>rather than by the ordinary DueDate.</summary>
		public RecallListSort SortBy;

		///<summary></summary>
		public int Compare(DataRow x,DataRow y) {
			//NOTE: Even if grouping by families, each family is not necessarily going to have a guarantor.
			if(GroupByFamilies) {
				if(SortBy==RecallListSort.Alphabetical) {
					//if guarantors are different, sort by guarantor name
					if(x["Guarantor"].ToString() != y["Guarantor"].ToString()) {
						if(x["guarLName"].ToString() != y["guarLName"].ToString()) {
							return x["guarLName"].ToString().CompareTo(y["guarLName"].ToString());
						}
						return x["guarFName"].ToString().CompareTo(y["guarFName"].ToString());
					}
					return 0;//order within family does not matter
				}
				else if(SortBy==RecallListSort.DueDate) {
					DateTime xD=PIn.Date(x["maxDateDue"].ToString());
					DateTime yD=PIn.Date(y["maxDateDue"].ToString());
					if(xD != yD) {
						return (xD.CompareTo(yD));
					}
					//if dates are same, sort/group by guarantor
					if(x["Guarantor"].ToString() != y["Guarantor"].ToString()) {
						return (x["Guarantor"].ToString().CompareTo(y["Guarantor"].ToString()));
					}
					//within the same family, sort by actual DueDate
					xD=PIn.Date(x["DateDue"].ToString());
					yD=PIn.Date(y["DateDue"].ToString());
					return (xD.CompareTo(yD));
					//return 0;
				}
				else if(SortBy==RecallListSort.BillingType){
					if(x["billingType"].ToString()!=y["billingType"].ToString()){
						return x["billingType"].ToString().CompareTo(y["billingType"].ToString());
					}
					//if billing types are the same, sort by dueDate
					DateTime xD=PIn.Date(x["maxDateDue"].ToString());
					DateTime yD=PIn.Date(y["maxDateDue"].ToString());
					if(xD != yD) {
						return (xD.CompareTo(yD));
					}
					//if dates are same, sort/group by guarantor
					if(x["Guarantor"].ToString() != y["Guarantor"].ToString()) {
						return (x["Guarantor"].ToString().CompareTo(y["Guarantor"].ToString()));
					}
				}
			}
			else {//individual patients
				if(SortBy==RecallListSort.Alphabetical) {
					if(x["LName"].ToString() != y["LName"].ToString()) {
						return x["LName"].ToString().CompareTo(y["LName"].ToString());
					}
					return x["FName"].ToString().CompareTo(y["FName"].ToString());
				}
				else if(SortBy==RecallListSort.DueDate) {
					if((DateTime)x["DateDue"] != (DateTime)y["DateDue"]) {
						return ((DateTime)x["DateDue"]).CompareTo(((DateTime)y["DateDue"]));
					}
					//if duedates are the same, sort by LName
					return x["LName"].ToString().CompareTo(y["LName"].ToString());
				}
				else if(SortBy==RecallListSort.BillingType){
					if(x["billingType"].ToString()!=y["billingType"].ToString()){
						return x["billingType"].ToString().CompareTo(y["billingType"].ToString());
					}
					//if billing types are the same, sort by dueDate
					if((DateTime)x["DateDue"] != (DateTime)y["DateDue"]) {
						return ((DateTime)x["DateDue"]).CompareTo(((DateTime)y["DateDue"]));
					}
					//if duedates are the same, sort by LName
					return x["LName"].ToString().CompareTo(y["LName"].ToString());
				}
			}
			return 0;
		}




	}

	public enum RecallListShowNumberReminders {
		All,
		Zero,
		One,
		Two,
		Three,
		Four,
		Five,
		SixPlus
	}

	public enum RecallListSort{
		DueDate,
		Alphabetical,
		BillingType
	}
	

}









