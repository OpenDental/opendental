using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>Helper class that holds all of the data necessary for generating a billing list.
	///This class centralizes some logic which allows things to execute faster and with far less network trips when utilizing the Middle Tier.
	///Only used in billing options window for generating statements.</summary>
	public class AgingData {

		///<summary>Returns a SerializableDictionary with key=PatNum, value=PatAgingData with the filters applied.</summary>
		public static SerializableDictionary<long,PatAgingData> GetAgingData(bool isSinglePatient,bool includeChanged,bool excludeInsPending,
			bool excludeIfUnsentProcs,bool isSuperBills,List<long> listClinicNums)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,PatAgingData>>(MethodBase.GetCurrentMethod(),isSinglePatient,includeChanged,
					excludeInsPending,excludeIfUnsentProcs,isSuperBills,listClinicNums);
			}
			SerializableDictionary<long,PatAgingData> dictPatAgingData=new SerializableDictionary<long,PatAgingData>();
			string command="";
			string guarOrPat="guar";
			if(isSinglePatient) {
				guarOrPat="patient";
			}
			string whereAndClinNum="";
			if(!listClinicNums.IsNullOrEmpty()) {
				whereAndClinNum=$@"AND {guarOrPat}.ClinicNum IN ({string.Join(",",listClinicNums)})";
			}
			if(includeChanged || excludeIfUnsentProcs) {
				command=$@"SELECT {guarOrPat}.PatNum,{guarOrPat}.ClinicNum,MAX(procedurelog.ProcDate) MaxProcDate";
				if(excludeIfUnsentProcs) {
					command+=",MAX(CASE WHEN insplan.IsMedical=1 THEN 0 ELSE COALESCE(claimproc.ProcNum,0) END)>0 HasUnsentProcs";
				}
				command+=$@" FROM patient
					INNER JOIN patient guar ON guar.PatNum=patient.Guarantor
					INNER JOIN procedurelog ON procedurelog.PatNum = patient.PatNum ";
				if(excludeIfUnsentProcs) {
					command+=$@"LEFT JOIN claimproc ON claimproc.ProcNum = procedurelog.ProcNum
						AND claimproc.NoBillIns=0
						AND claimproc.Status = {POut.Int((int)ClaimProcStatus.Estimate)}
						AND procedurelog.ProcDate > CURDATE()-INTERVAL 6 MONTH
					LEFT JOIN insplan ON insplan.PlanNum=claimproc.PlanNum ";
				}
				command+=$@"WHERE procedurelog.ProcFee > 0
					AND procedurelog.ProcStatus = {POut.Int((int)ProcStat.C)}
					{whereAndClinNum}
					GROUP BY {guarOrPat}.PatNum
					ORDER BY NULL";
				using(DataTable tableChangedAndUnsent=Db.GetTable(command)) {
					foreach(DataRow row in tableChangedAndUnsent.Rows) {
						long patNum=PIn.Long(row["PatNum"].ToString());
						if(!dictPatAgingData.ContainsKey(patNum)) {
							dictPatAgingData[patNum]=new PatAgingData(PIn.Long(row["ClinicNum"].ToString()));
						}
						if(includeChanged) {
							dictPatAgingData[patNum].ListPatAgingTransactions
								.Add(new PatAgingTransaction(PatAgingTransaction.TransactionTypes.Procedure,PIn.Date(row["MaxProcDate"].ToString())));
						}
						if(excludeIfUnsentProcs) {
							dictPatAgingData[patNum].HasUnsentProcs=PIn.Bool(row["HasUnsentProcs"].ToString());
						}
					}
				}
			}
			if(includeChanged) {
				command=$@"SELECT {guarOrPat}.PatNum,{guarOrPat}.ClinicNum,MAX(claimproc.DateCP) maxDateCP
					FROM claimproc
					INNER JOIN patient ON patient.PatNum = claimproc.PatNum
					INNER JOIN patient guar ON guar.PatNum=patient.Guarantor
					WHERE claimproc.InsPayAmt > 0
					{whereAndClinNum}
					GROUP BY {guarOrPat}.PatNum";
				using(DataTable tableMaxPayDate=Db.GetTable(command)) {
					foreach(DataRow row in tableMaxPayDate.Rows) {
						long patNum=PIn.Long(row["PatNum"].ToString());
						if(!dictPatAgingData.ContainsKey(patNum)) {
							dictPatAgingData[patNum]=new PatAgingData(PIn.Long(row["ClinicNum"].ToString()));
						}
						dictPatAgingData[patNum].ListPatAgingTransactions
							.Add(new PatAgingTransaction(PatAgingTransaction.TransactionTypes.ClaimProc,PIn.Date(row["maxDateCP"].ToString())));
					}
				}
				command=$@"SELECT {guarOrPat}.PatNum,{guarOrPat}.ClinicNum,MAX(payplancharge.ChargeDate) maxDatePPC,
						MAX(payplancharge.SecDateTEntry) maxDatePPCSDTE
					FROM payplancharge
					INNER JOIN patient ON patient.PatNum = payplancharge.PatNum
					INNER JOIN patient guar ON guar.PatNum=patient.Guarantor
					INNER JOIN payplan ON payplan.PayPlanNum = payplancharge.PayPlanNum
						AND payplan.PlanNum = 0 "//don't want insurance payment plans to make patients appear in the billing list
					+$@"WHERE payplancharge.Principal + payplancharge.Interest>0
					AND payplancharge.ChargeType = {(int)PayPlanChargeType.Debit} "
					//include all charges in the past or due 'PayPlanBillInAdvance' days into the future.
					+$@"AND payplancharge.ChargeDate <= {POut.Date(DateTime.Today.AddDays(PrefC.GetDouble(PrefName.PayPlansBillInAdvanceDays)))}
					{whereAndClinNum}
					GROUP BY {guarOrPat}.PatNum";
				using(DataTable tableMaxPPCDate=Db.GetTable(command)) {
					foreach(DataRow row in tableMaxPPCDate.Rows) {
						long patNum=PIn.Long(row["PatNum"].ToString());
						if(!dictPatAgingData.ContainsKey(patNum)) {
							dictPatAgingData[patNum]=new PatAgingData(PIn.Long(row["ClinicNum"].ToString()));
						}
						dictPatAgingData[patNum].ListPatAgingTransactions
							.Add(new PatAgingTransaction(
								PatAgingTransaction.TransactionTypes.PayPlanCharge,
								PIn.Date(row["maxDatePPC"].ToString()),
								secDateTEntryTrans:PIn.Date(row["maxDatePPCSDTE"].ToString()))
							);
					}
				}
			}
			if(excludeInsPending) {
				command=$@"SELECT {guarOrPat}.PatNum,{guarOrPat}.ClinicNum
					FROM claim
					INNER JOIN patient ON patient.PatNum=claim.PatNum
					INNER JOIN patient guar ON guar.PatNum=patient.Guarantor
					WHERE claim.ClaimStatus IN ('U','H','W','S')
					AND claim.ClaimType IN ('P','S','Other')
					{whereAndClinNum}
					GROUP BY {guarOrPat}.PatNum";
				using(DataTable tableInsPending=Db.GetTable(command)) {
					foreach(DataRow row in tableInsPending.Rows) {
						long patNum=PIn.Long(row["PatNum"].ToString());
						if(!dictPatAgingData.ContainsKey(patNum)) {
							dictPatAgingData[patNum]=new PatAgingData(PIn.Long(row["ClinicNum"].ToString()));
						}
						dictPatAgingData[patNum].HasPendingIns=true;
					}
				}
			}
			DateTime dateAsOf=DateTime.Today;//used to determine when the balance on this date began
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {//if aging calculated monthly, use the last aging date instead of today
				dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
			}
			List<PatComm> listPatComms=new List<PatComm>();
			using(DataTable tableDateBalsBegan=Ledgers.GetDateBalanceBegan(null,dateAsOf,isSuperBills,listClinicNums)) {
				foreach(DataRow row in tableDateBalsBegan.Rows) {
					long patNum=PIn.Long(row["PatNum"].ToString());
					if(!dictPatAgingData.ContainsKey(patNum)) {
						dictPatAgingData[patNum]=new PatAgingData(PIn.Long(row["ClinicNum"].ToString()));
					}
					dictPatAgingData[patNum].DateBalBegan=PIn.Date(row["DateAccountAge"].ToString());
					dictPatAgingData[patNum].DateBalZero=PIn.Date(row["DateZeroBal"].ToString());
				}
				listPatComms=Patients.GetPatComms(tableDateBalsBegan.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList(),null);
			}
			foreach(PatComm pComm in listPatComms) {
				if(!dictPatAgingData.ContainsKey(pComm.PatNum)) {
					dictPatAgingData[pComm.PatNum]=new PatAgingData(pComm.ClinicNum);
				}
				dictPatAgingData[pComm.PatNum].PatComm=pComm;
			}
			return dictPatAgingData;
		}

		///<summary>Returns the max date of the last transaction for the transactions passed in.  Returns MinValue if none found.
		///This method assumes that every aging transaction passed can cause a statement. i.e. the PayPlanCharges passed in are within the today's date
		///plus the bill in advance days.</summary>
		public static DateTime GetDateLastTrans(List<PatAgingTransaction> listPatAgingTransactions,DateTime dateLastStatement) {
			//No need to check RemotingRole; no call to db.
			//Procedures and claimprocs are straight forward in the sense that a statement will be required if their date is after dateLastStatement.
			//Payment plans are tricky in the sense that we have a preference that allows billing patients X days in advance.
			//If there is a valid procedure or claimproc date and it falls after dateLastStatement then this patient needs a statement.
			DateTime dateLastTrans=new DateTime[] {
				GetMaxDateLastTransForType(listPatAgingTransactions,PatAgingTransaction.TransactionTypes.Procedure),
				GetMaxDateLastTransForType(listPatAgingTransactions,PatAgingTransaction.TransactionTypes.ClaimProc),
			}.Max();
			//Check to see if this patient has a payment plan that has a charge date greater than the last procedure or claimproc date.
			DateTime datePayPlanChargeMax=GetMaxDateLastTransForType(listPatAgingTransactions,PatAgingTransaction.TransactionTypes.PayPlanCharge);
			if(datePayPlanChargeMax > dateLastTrans) {
				//There is a chance that this patient has already received a statement due to this payment plan charge due to the fact that we allow
				//for "billing X days in advance" for payment plans only (via PayPlansBillInAdvanceDays).
				long billInAdvanceDays=PrefC.GetLong(PrefName.PayPlansBillInAdvanceDays);
				//Only set dateLastTrans to a payment plan charge date if the charge falls outside of the "bill X days in advance" preference.
				//E.g. A statement on the 1st of the month is treated as having covored all payment plan charges until the 11th when pref is set to 10 days.
				//However, dateLastTrans needs to be set when a billing list is created on the 5th with a new payment plan charge on the 14th.
				//This is because the statement created on the 1st does not cover the payment plan charge on the 14th.
				DateTime datePayPlanCreateMax=GetMaxDateLastTransForType(listPatAgingTransactions,PatAgingTransaction.TransactionTypes.PayPlanCharge,true);
				if(datePayPlanChargeMax > dateLastStatement.AddDays(billInAdvanceDays) || datePayPlanCreateMax > dateLastStatement) {
					dateLastTrans=datePayPlanChargeMax;
				}
			}
			return dateLastTrans;
		}

		///<summary>Returns the max DateLastTrans for all PatAgingTransactions of the specified type.  Returns MinValue if none found.
		///Set isSecDateTEntry true if the max SecDateTEntryTrans should be returned instead of the max DateLastTrans.</summary>
		private static DateTime GetMaxDateLastTransForType(List<PatAgingTransaction> ListPatAgingTransactions,
			PatAgingTransaction.TransactionTypes transactionType,bool useSecDateTEntry=false)
		{
			//No need to check RemotingRole; no call to db.
			List<PatAgingTransaction> listTransForType=ListPatAgingTransactions.FindAll(x => x.TransactionType==transactionType);
			if(listTransForType.IsNullOrEmpty()) {
				return DateTime.MinValue;
			}
			if(useSecDateTEntry) {
				return listTransForType.Max(x => x.SecDateTEntryTrans).Date;
			}
			else {
				return listTransForType.Max(x => x.DateLastTrans);
			}
		}

	}

	///<summary>Not a db table.  Used for generating statements</summary>
	[Serializable]
	public class PatAgingData {
		public DateTime DateBalBegan;
		public DateTime DateBalZero;
		///<summary>A list of transaction types along with their most recent date for this patient.</summary>
		public List<PatAgingTransaction> ListPatAgingTransactions;
		public PatComm PatComm;
		public bool HasUnsentProcs;
		public bool HasPendingIns;
		public long ClinicNum;

		///<summary>Only for serialization purposes.  Private so it won't be used.  Use the constructor that takes a ClinicNum instead.  This means we can
		///always expect the PatAgingData to have a ClinicNum.</summary>
		private PatAgingData() { }

		public PatAgingData(long clinicNum) {
			ClinicNum=clinicNum;
			DateBalBegan=DateTime.MinValue;
			DateBalZero=DateTime.MinValue;
			ListPatAgingTransactions=new List<PatAgingTransaction>();
		}

		///<summary></summary>
		public PatAgingData Copy() {
			PatAgingData patAgingData=(PatAgingData)this.MemberwiseClone();
			patAgingData.ListPatAgingTransactions=this.ListPatAgingTransactions.Select(x => x.Copy()).ToList();
			return patAgingData;
		}
	}

	///<summary>This object helps track of max date for certain billing transaction types.</summary>
	[Serializable]
	[XmlType(TypeName="A")]
	public class PatAgingTransaction {
		///<summary>The type of transaction which we are tracking max date for.</summary>
		[XmlIgnore]
		public TransactionTypes TransactionType;
		///<summary>The max date for the corresponding transaction type entity.  E.g. procedurelog.ProcDate, claimproc.DateCP, etc.</summary>
		[XmlElement("B",typeof(DateTime))]
		public DateTime DateLastTrans;
		///<summary>Optional field used to keep track of the security DateT entry for the corresponding transaction type.</summary>
		[XmlElement("C",typeof(DateTime))]
		public DateTime SecDateTEntryTrans;

		///<summary>Xml serialized as an int to reduce space.</summary>
		[XmlElement("D",typeof(int))]
		public int TransactionTypeInt {
			get {
				return (int)TransactionType;
			}
			set {
				TransactionType=(TransactionTypes)value;
			}
		}

		///<summary>Only for serialization purposes.  Private so it won't be used.</summary>
		private PatAgingTransaction() { }

		///<summary></summary>
		public PatAgingTransaction(TransactionTypes transactionType,DateTime dateLastTrans,DateTime secDateTEntryTrans=default) {
			TransactionType=transactionType;
			DateLastTrans=dateLastTrans;
			SecDateTEntryTrans=secDateTEntryTrans;
		}

		///<summary></summary>
		public PatAgingTransaction Copy() {
			return (PatAgingTransaction)this.MemberwiseClone();
		}

		///<summary>Types which max dates are tracked for.</summary>
		public enum TransactionTypes {
			///<summary></summary>
			Undefined,
			///<summary></summary>
			Procedure,
			///<summary></summary>
			ClaimProc,
			///<summary></summary>
			PayPlanCharge,
		}
	}

}
