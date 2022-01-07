using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace OpenDentBusiness{

	///<summary>Transworld Systems Inc (TSI) transaction log.  Logs communication between the Open Dental program and TSI.  Entries contain information
	///about accounts placed with TSI, payments or adjustments to accounts placed, or transactions to Suspend, Reinstate or Cancel accounts.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class TsiTransLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TsiTransLogNum;
		///<summary>FK to patient.PatNum for the guarantor of the account sent to TSI for collection services.  TSI refers to this as the Debtor Reference
		///or Responsible Party Account Number.</summary>
		public long PatNum;
		///<summary>FK to userod.UserNum.  The user who sent the account for placement with TSI or who suspended, reinstated or cancelled collection
		///services for an account placed with TSI or who created the payment/adjustment for an account placed with TSI.</summary>
		public long UserNum;
		///<summary>Enum:TsiTransType - Identifies the transaction message sent to TSI.  Can be a message for placing/cancelling/suspending/reinstating
		///collection services for an account or to notify TSI of a payment/writeoff/adjustment entered into OD.</summary>
		public TsiTransType TransType;
		///<summary>Timestamp at which this row was created. Auto generated on insert.  Identifies exactly when the action happened in OD to cause the
		///message to be sent to TSI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime TransDateTime;
		///<summary>Enum:TsiServiceType - for placements, this is the type of collection activity that will start on the account being placed.</summary>
		public TsiServiceType ServiceType;
		///<summary>Enum:TsiServiceCode - for placements, intensity of first letter sent to guarantor.  Will usually be 0 - Diplomatic.</summary>
		public TsiServiceCode ServiceCode;
		///<summary>If ServiceType for the placement is TsiDemandType.AcceleratorPr, this will be the Accelerator/Profit Recovery client ID.
		///If TsiServiceType.Collection it will be the Collection client ID. Will always match the first field, Client Number, in the RawMsgText.</summary>
		public string ClientId;
		///<summary>Used for payments/writeoffs/adjustments, amount applied to the debt.</summary>
		public double TransAmt;
		///<summary>Total balance due on the account by the patient, i.e. BalTotal-InsPayEst-WoEst.  If this is a placement, this is the debt amount TSI
		///is going to attempt to collect.  If this is a payment/writeoff/adjustment, this is the new balance after the transaction amount is applied to
		///the debt.</summary>
		public double AccountBalance;
		///<summary>Enum:TsiFKeyType - Used in conjunction with FKey to point to the item that this log row represents.</summary>
		public TsiFKeyType FKeyType;
		///<summary>Foreign key to the table defined by the corresponding FKeyType.  Currently supports paysplit.SplitNum, claimproc.ClaimProcNum,
		///adjustment.AdjNum, procedurelog.ProcNum, payplan.PayPlanNum, payplancharge.PayPlanChargeNum.</summary>
		public long FKey;
		///<summary>Raw pipe-delimited message sent to TSI.</summary>
		public string RawMsgText;
		///<summary>Json serialized string representation of the TsiTrans list used to calculate the account balance for this guarantor at the time of
		///placement with Transworld.  Used to update Transworld if any of the transactions are modified after placement.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string TransJson;
		///<summary>FK to clinic.ClinicNum.  This will be 0 if clinics are not enabled.  This will be 0 for logs prior to version 18.4.</summary>
		public long ClinicNum;
		///<summary>FK to tsitranslog.TransLogNum.  Will be 0 if not part of an aggregate group.  Will be 0 for logs prior to version 18.4.</summary>
		public long AggTransLogNum;

		///<summary>Not a database column.  Only used temporarily by the OpenDentalService.TransworldThread in order to aggregate claimproc amounts
		///(InsPayAmt+WriteOff or InsPayEst+WriteOffEst) into their corresponding proc fees so we can send TSI a net account balance change.  Users were
		///being charged by TSI for insurance estimates on new procs when they haven't actually received any income.  Every transaction will still have a
		///row in the TsiTransLog table and we don't need to store the ProcNum for use after the sync.  It will only be used during the sync process for
		///proc fee and claimproc messages.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public long ProcNum;
		///<summary>Not a database column.  Only used temporarily by the OpenDentalService.TransworldThread in order to aggregate paysplit amounts into
		///their corresponding payments so we can send TSI a net account balance change.  We need to aggregate paysplits so that income transfers don't
		///cause paid in full messages to TSI.  Every transaction will still have a row in the TsiTransLog table and we don't need to store the PayNum
		///for use after the sync.  It will only be used during the sync process for paysplit messages.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public long PayNum;
		///<summary>Not a database column.  Only used temporarily by the OpenDentalService.TransworldThread in order to process transactions in the order
		///they occurred in the guarantor's account.  This will allow us to mark an account as paid in full if, on any transaction date, the credits for
		///the date reduce the account balance to &lt;= $0.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public DateTime TranDate;
		///<summary>Not a database column.  The dictionary with key=Tuple&lt;TsiFKeyType,long>, value=the trans with that type and pri key for the guar at
		///the time of placement with Transworld.</summary>
		[XmlIgnore,JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		private Dictionary<Tuple<TsiFKeyType,long>,TsiTrans> _dictTransByType;

		///<summary>The dictionary with key=Tuple&lt;TsiFKeyType,long>, value=the trans with the type and pri key for the guar when placed with Transworld.</summary>
		[XmlIgnore,JsonIgnore]
		public Dictionary<Tuple<TsiFKeyType,long>,TsiTrans> DictTransByType {
			get {
				if(_dictTransByType==null) {
					if(string.IsNullOrWhiteSpace(TransJson)) {
						_dictTransByType=new Dictionary<Tuple<TsiFKeyType,long>,TsiTrans>();
					}
					else {
						//TransJson will be stored as a serialized list of TsiTrans objects that is easiest to use when in a dictionary form.
						_dictTransByType=JsonConvert.DeserializeObject<List<TsiTrans>>(TransJson,new JsonSerializerSettings() { TypeNameHandling=TypeNameHandling.Auto })
							.ToDictionary(x => new Tuple<TsiFKeyType,long>(x.KeyType,x.PriKey));
					}
				}
				return _dictTransByType;
			}
			set {
				_dictTransByType=value;
				TransJson="";
				if(value!=null && value.Count>0) {
					TransJson=JsonConvert.SerializeObject(value.Select(x => x.Value).ToList(),
						typeof(List<TsiTrans>),
						new JsonSerializerSettings() { TypeNameHandling=TypeNameHandling.Auto });
				}
			}
		}
		
		public TsiTransLog Copy() {
			TsiTransLog retval=(TsiTransLog)this.MemberwiseClone();
			retval._dictTransByType=new Dictionary<Tuple<TsiFKeyType, long>, TsiTrans>();
			foreach(Tuple<TsiFKeyType,long> key in this.DictTransByType.Keys) {
				retval._dictTransByType[key]=this.DictTransByType[key].Copy();
			}
			return retval;
		}
	}

	///<summary>Identifies the transaction type represented by this log entry.  Could be a for placing/cancelling/suspending/reinstating an account or
	///for a payment/writeoff/adjustment to an account balance.  Don't remove items or alter order of this enum, FKey requires this to be static.</summary>
	public enum TsiTransType {
		///<summary>-1 - None: Used for marking trans that represent account items NOT sent to Transworld.  The TsiTransLogs with this trans type are used
		///to prevent the ODService from sending the trans to Transworld.  Example: Transworld collects a payment from a patient and deduct their fee and
		///send the practice the remaining amt.  Office staff will enter the payment in the patient's account and then enter an adjustment to account for
		///Transworld's fee.  TsiTransLogs will be inserted for the paysplits and adjustment so that we don't send those trans to Transworld and cause an
		///infinite loop of sorts.  Logs with this type will be update by the ODService if the ledger trans amt is updated.</summary>
		[Description("None")]
		None=-1,
		///<summary>0 - Cancel: cancel collection services for an account. Collection services can be restarted but will incur another TSI fee.</summary>
		[Description("Cancel")]
		CN,
		///<summary>1 - Credit Adjustment: negative adjustment to reduce balance. Example: a discount given or portion of the debt written off.</summary>
		[Description("Credit Adjustment")]
		CR,
		///<summary>2 - Debit Adjustment: positive adjustment to increase balance.  Offices are supposed to stop all finance charges once placed with TSI,
		///but there may be other transactions that require increasing the amount owed.</summary>
		[Description("Debit Adjustment")]
		DB,
		///<summary>3 - Paid in Full: payment entered that pays off account balance.  Closes account with TSI and stops collection activity.</summary>
		[Description("Paid in Full")]
		PF,
		///<summary>4 - Placement: account sent to TSI for Accelerator/Profit Recovery/Collection services.</summary>
		[Description("Placement")]
		PL,
		///<summary>5 - Partial Payment: payment by either patient or ins payment/writeoff that pays a portion of the balance.</summary>
		[Description("Partial Payment")]
		PP,
		///<summary>6 - Paid in Full, Thank You: payment entered that pays off account balance.  Closes account with TSI and stops collection activity.
		///TSI will send a Thank You letter to the patient free of charge.</summary>
		[Description("Paid in Full, Thank You")]
		PT,
		///<summary>7 - Reinstate: an account that has been suspended can be reinstated within 50 days and the collection services will resume where it
		///left off.  After 50 days the account is automatically cancelled and in order to restart collection services the office would have to initiate a
		///new placement, which will incur another TSI fee.</summary>
		[Description("Reinstate")]
		RI,
		///<summary>8 - Suspend: places collection services for the account on hold for up to 50 days.  Example: After an account is placed with TSI, the
		///customer comes into the office and agrees to a payment plan.  The account can be suspended and if the patient fails to make a payment within 50
		///days the account can be reinstated and the collection process will resume where it left off and TSI will not charge an additional fee.  After
		///50 days the account is automatically cancelled by TSI and in order to restart the collection process, the office would have to initiate a new
		///placement which starts the collection process over and will result in an additional TSI fee.</summary>
		[Description("Suspend")]
		SS,
		///<summary>9 - To differentiate aggregate rows from rows linked to transactions in the OD db.</summary>
		[Description("Aggregate")]
		Agg,
		///<summary>10 - Used for adjustments entered with the SyncExcludePosAdjType or SyncExcludeNegAdjType set in the Transworld program link.
		///Excluded from syncing with TSI and from the amount due calculation used in future msgs or to determine if the acct is paid in full.</summary>
		[Description("Excluded")]
		Excluded,
	}

	///<summary>Don't remove items or alter order of this enum, FKey requires this to be static.</summary>
	public enum TsiServiceType {
		///<summary>0 - Accelerator</summary>
		[Description("Accelerator")]
		Accelerator,
		///<summary>1 - Profit Recovery</summary>
		[Description("Profit Recovery")]
		ProfitRecovery,
		///<summary>2 - Professional Collections</summary>
		[Description("Professional Collections")]
		ProfessionalCollections
	}

	///<summary>The service code determines the intensity of the first letter.  According to a TSI rep during a conference call, Diplomatic will be what
	///most offices will want to use.  Bad check is rarely used.  Don't remove items or alter order of this enum, FKey requires this to be static.</summary>
	public enum TsiServiceCode {
		///<summary>0 - Diplomatic: most commonly used service code.</summary>
		[Description("Diplomatic")]
		Diplomatic,
		///<summary>1 - Intensive:  More intense first letter.</summary>
		[Description("Intensive")]
		Intensive,
		///<summary>3 - Bad Check: in a conference call with TSI one of the reps said this is rarely used.</summary>
		[Description("Bad Check")]
		BadCheck
	}

	///<summary>Identifies the table to which FKey points.  Don't remove items or alter order of this enum, FKey requires this to be static.</summary>
	public enum TsiFKeyType {
		///<summary>-1 - None.  For place, suspend, cancel, agg.</summary>
		None=-1,
		///<summary>0 - adjustment.AdjNum.  Can be a positive (Debit) or negative (Credit) adjustment to the amount owed.  The resulting message
		///TsiTransType is DB (Debit) for positive adjustments or CR (Credit) for negative adjustments.</summary>
		Adjustment=0,
		///<summary>1 - claimproc.ClaimProcNum. For ins payments and/or writeoffs entered after the account has been placed with TSI.  The resulting
		///message TsiTransType is PP (Partial Payment), PF (Paid in Full), or PT (Paid in Full, Thank You).</summary>
		Claimproc,
		///<summary>2 - payplan.PayPlanNum.  In payplan version 1 the entire CompletedAmt is aged, so it will be negative (credit) and decrease the amount
		///owed.  The resulting message TsiTransType is CR (Credit).</summary>
		PayPlan,
		///<summary>3 - payplancharge.PayPlanChargeNum.  Depends on payplan version, could be negative (credit - decrease amount owed) or
		///positive (debit - increase amount owed).  The resulting message TsiTransType is DB (Debit) if positive or CR (Credit) if negative.</summary>
		PayPlanCharge,
		///<summary>4 - paysplit.SplitNum.  Patient payment on an account placed with TSI.  The resulting message TsiTransType is PP (Partial Payment),
		///PF (Paid in Full), or PT (Paid in Full, Thank You).</summary>
		PaySplit,
		///<summary>5 - procedurelog.ProcNum.  Positive (debit, increases the amount owed).  The resulting message TsiTransType is DB (Debit).</summary>
		Procedure,
	}

	///<summary>Not a database table.  Transworld account transaction object.  Used for the following objects: procedurelog, claimproc, paysplit,
	///adjustment, payplan, payplancharge.  This will be used for storing the account transactions that comprise an account at the time it's sent to
	///Transworld for collection.  The stored values will be used if transactions prior to the placement date are modified and the balance at placement
	///time has changed.</summary>
	[Serializable]
	public class TsiTrans {
		public TsiFKeyType KeyType;
		public long PriKey;
		public long ProcNum;
		public long PayNum;
		public long PatNum;
		public long Guarantor;
		public DateTime TranDate;
		public double TranAmt;

		///<summary>For serialization.</summary>
		public TsiTrans() {
		}

		public TsiTrans(long priKey,TsiFKeyType keyType,long procNum,long payNum,long patNum,long guarantor,DateTime tranDate,double tranAmt) {
			KeyType=keyType;
			PriKey=priKey;
			ProcNum=procNum;
			PayNum=payNum;
			PatNum=patNum;
			Guarantor=guarantor;
			TranDate=tranDate;
			TranAmt=tranAmt;
		}

		public TsiTrans Copy() {
			return (TsiTrans)MemberwiseClone();
		}
	}

}