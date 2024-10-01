using CodeBase;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace OpenDentBusiness.Misc {
	public class FamilyBalancer {
		public long PatNum;
		///<summary>Set to the total number of guarantors in the database by the first balancer thread that runs.</summary>
		public int GuarantorTotal;
		///<summary>The number of new payments made by the balancer thread.</summary>
		public int PaymentCount;
		///<summary>A queue of PatNums for guarantors that need to be processed by the balancer thread.</summary>
		public ConcurrentQueue<long> ConcurrentQueueGuarantors;
		///<summary>A queue of text that needs to be appended to the Output text box.</summary>
		public ConcurrentQueue<OutputMessage> ConcurrentQueueOutputMessages=new ConcurrentQueue<OutputMessage>();

		public FamilyBalancer() { }

		public void ThreadWorkerIncomeTransfer(ODThread thread) {
			if(thread.Tag==null || thread.Tag.GetType()!=typeof(FamilyBalancerOptions)) {
				return;
			}
			FamilyBalancerOptions familyBalanceOptions=(FamilyBalancerOptions)thread.Tag;
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Income Transfer thread started at {DateTime.Now} with the following settings:"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Income Transfer Date: {familyBalanceOptions.DateIncomeTransfer.ToShortDateString()}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"As of Date: {familyBalanceOptions.DateAsOf.ToShortDateString()}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Delete All Transfers: {(familyBalanceOptions.DoDeleteTransfers ? "True" : "False")}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Use Date Changed Since: {(familyBalanceOptions.UseDateChangedSince ? "True" : "False")}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Date Changed Since: {familyBalanceOptions.DateChangedSince.ToShortDateString()}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Is Rigorous: {(familyBalanceOptions.IsRigorous ? "True" : "False")}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			FillGuarantorQueue(familyBalanceOptions);
			string strLogic="FIFO logic";
			if(familyBalanceOptions.IsRigorous) {
				strLogic="Rigorous logic";
			}
			while(!thread.HasQuit && ConcurrentQueueGuarantors!=null && ConcurrentQueueGuarantors.TryDequeue(out long guarantor)) {
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Processing the family of guarantor: {guarantor} ------------------"));
				Family family=Patients.GetFamily(guarantor);
				Patient patient=family.GetPatient(guarantor);
				if(familyBalanceOptions.DoDeleteTransfers) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Deleting transfers for family..."));
					PaymentEdit.DeleteTransfersForFamily(family.GetPatNums());
				}
				if(!TransferClaimsPayAsTotal(family,patient)) {
					continue;
				}
				List<PaySplit> listPaySplits;
				if(familyBalanceOptions.IsRigorous) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Rigorous income transfer logic running..."));
					PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(family.GetPatNums(),patient.PatNum,
						new List<PaySplit>(),new Payment(),new List<AccountEntry>(),isIncomeTxfr:true,dateAsOf:familyBalanceOptions.DateAsOf);
					if(!PaymentEdit.TryCreateIncomeTransfer(constructResults.ListAccountEntries,familyBalanceOptions.DateIncomeTransfer,out PaymentEdit.IncomeTransferData data)) {
						ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Failed to create an income transfer due to the following:"));
						if(data.StringBuilderWarnings.Length > 0) {
							ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Warning - {data.StringBuilderWarnings}"));
						}
						if(data.StringBuilderErrors.Length > 0) {
							ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Error - {data.StringBuilderErrors}"));
						}
						continue;
					}
					listPaySplits=data.ListSplitsCur;
				}
				else {//FIFO
					try {
						ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"FIFO income transfer logic running..."));
						PaymentEdit.IncomeTransferData incomeTransferData=PaymentEdit.GetIncomeTransferDataFIFO(patient.PatNum,familyBalanceOptions.DateIncomeTransfer,
							dateAsOf:familyBalanceOptions.DateAsOf);
						listPaySplits=incomeTransferData.ListSplitsCur;
					}
					catch(Exception ex) {
						ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"CRITICAL ERROR: {ex.Message}"));
						continue;
					}
				}
				if(listPaySplits.IsNullOrEmpty()) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"No income transfer needed for family."));
					continue;
				}
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"An income transfer with {listPaySplits.Count} payment splits was created to balance the family account."));
				Payment payment=new Payment();
				payment.PayDate=familyBalanceOptions.DateIncomeTransfer;
				payment.PatNum=patient.PatNum;
				//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
				//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
				payment.ClinicNum=0;
				if(PrefC.HasClinicsEnabled) {
					payment.ClinicNum=Clinics.ClinicNum;
					if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
						payment.ClinicNum=patient.ClinicNum;
					}
					else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
						payment.ClinicNum=(Clinics.ClinicNum==0 ? patient.ClinicNum : Clinics.ClinicNum);
					}
				}
				payment.DateEntry=DateTime.Today;
				payment.PaymentSource=CreditCardSource.None;
				payment.ProcessStatus=ProcessStat.OfficeProcessed;
				payment.PayAmt=0;
				payment.PayType=0;
				Payments.Insert(payment,listPaySplits);
				SecurityLogs.MakeLogEntry(EnumPermType.PaymentCreate,patient.PatNum,$"Income transfer created by the Family Balancer tool using {strLogic}.");
				PaymentCount++;
				Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(patient.PatNum,listPaySplits);
			}
		}

		private void FillGuarantorQueue(FamilyBalancerOptions familyBalanceOptions) {
			if(ConcurrentQueueGuarantors==null) {
				List<long> listGuarantors;
				if(PatNum > 0) {//Single Family Mode
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$"Getting the guarantor for the family of PatNum:{PatNum}..."));
					listGuarantors=Patients.GetGuarantorsForPatNums(ListTools.FromSingle(PatNum));
				}
				else if(familyBalanceOptions.UseDateChangedSince) {//Families with financial data changed since a specified date.
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,
						$"Getting the guarantors of families with financial data changed since:{familyBalanceOptions.DateChangedSince.ToShortDateString()}..."));
					listGuarantors=Patients.GetGuarantorsWithFinancialDataChangedAfterDate(familyBalanceOptions.DateChangedSince);
				}
				else {//Entire Database
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,"Getting every guarantor in the database..."));
					listGuarantors=Patients.GetAllGuarantors();
				}
				GuarantorTotal=listGuarantors.Count;
				//Fill the queue of guarantors that the thread will use as a way to determine if it has more work to do.
				ConcurrentQueueGuarantors=new ConcurrentQueue<long>(listGuarantors);
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$"{GuarantorTotal:N0} guarantors queued for processing."));
			}
			else {
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,"Unprocessed guarantors detected. Continuing to process instead of starting over."));
			}
		}

		///<summary>Returns true if claim pay as totals were transferred or if there were none to process. Returns false if there was an error.</summary>
		public bool TransferClaimsPayAsTotal(Family family,Patient patient) {
			try {
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Transferring claim pay as totals..."));
				ClaimTransferResult claimTransferResult=PaymentEdit.TransferClaimsPayAsTotal(patient.PatNum,family.GetPatNums(),
					"Automatic transfer of claims pay as total from family balancer.");
				//Process the claim transfer result for logging purposes.
				if(claimTransferResult==null || claimTransferResult.ListClaimProcsInserted.IsNullOrEmpty()) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"No claim pay as total transfers needed."));
				}
				else {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"The following claims needed a pay as total transfer:"));
					List<ClaimTransferBreakdown> listClaimTransferBreakdowns=claimTransferResult.ListClaimProcsInserted.GroupBy(x => x.ClaimNum)
						.ToDictionary(x => x.Key,x => x.ToList())
						.Select(x => new ClaimTransferBreakdown() { ClaimNum=x.Key,CountNewClaimProcs=x.Value.Count })
						.ToList();
					List<string> listBreakdownStrings=listClaimTransferBreakdowns
						.Select(x => $"  ClaimNum: {x.ClaimNum} added {x.CountNewClaimProcs} new claimprocs.")
						.ToList();
					string strClaimProcDesc=string.Join("\r\n",listBreakdownStrings);
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,strClaimProcDesc));
					int countPaySplits=claimTransferResult.ListPaySplitsInserted.Count;
					string strSplitsTotal=claimTransferResult.ListPaySplitsInserted.Sum(x => x.SplitAmt).ToString("C");
					string strPaySplitDesc=$"  There were {countPaySplits} new unearned payment splits totalling {strSplitsTotal} inserted.";
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,strPaySplitDesc));
				}
			}
			catch(Exception ex) {
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"CRITICAL ERROR: {ex.Message}"));
				return false;
			}
			return true;
		}

		public void ThreadWorkerRecreate(ODThread thread) {
			if(thread.Tag==null || thread.Tag.GetType()!=typeof(FamilyBalancerOptions)) {
				return;
			}
			FamilyBalancerOptions familyBalanceOptions=(FamilyBalancerOptions)thread.Tag;
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Recreate thread started at {DateTime.Now} with the following settings:"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"As of Date: {familyBalanceOptions.DateAsOf.ToShortDateString()}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Delete All Transfers: {(familyBalanceOptions.DoDeleteTransfers ? "True" : "False")}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			FillGuarantorQueue(familyBalanceOptions);
			while(!thread.HasQuit && ConcurrentQueueGuarantors!=null && ConcurrentQueueGuarantors.TryDequeue(out long guarantor)) {
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Processing the family of guarantor: {guarantor} ------------------"));
				if(familyBalanceOptions.DoDeleteTransfers) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Deleting transfers for family..."));
					List<long> listPatNums=Patients.GetFamily(guarantor).GetPatNums();
					PaymentEdit.DeleteTransfersForFamily(listPatNums,isPayTypeIgnored:true);
				}
				try {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Recreating all payment splits..."));
					PaymentEdit.ReAutoSplitAllPaymentsForFamily(guarantor,dateAsOf:familyBalanceOptions.DateAsOf);
				}
				catch(ODException odex) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$"Error processing the family of guarantor: {guarantor}\r\n  {odex.Message}"));
					continue;
				}
				catch(Exception ex) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$"CRITICAL ERROR processing the family of guarantor: {guarantor}\r\n  {ex.Message}"));
					continue;
				}
			}
		}

		public void ThreadWorkerOverpay(ODThread thread) {
			if(thread.Tag==null || thread.Tag.GetType()!=typeof(FamilyBalancerOptions)) {
				return;
			}
			FamilyBalancerOptions familyBalanceOptions=(FamilyBalancerOptions)thread.Tag;
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Overpay thread started at {DateTime.Now}"));
			ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			FillGuarantorQueue(familyBalanceOptions);
			while(!thread.HasQuit && ConcurrentQueueGuarantors!=null && ConcurrentQueueGuarantors.TryDequeue(out long guarantor)) {
				ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Processing the family of guarantor: {guarantor} ------------------"));
				Family family=Patients.GetFamily(guarantor);
				Patient patient=family.GetPatient(guarantor);
				if(!TransferClaimsPayAsTotal(family,patient)) {
					continue;
				}
				try {
					InsOverpayResult insOverpayResult=PaymentEdit.TransferInsuranceOverpaymentsForFamily(family,
						payDate:familyBalanceOptions.DatePay,
						defNumPayType:familyBalanceOptions.DefNumPayType);
					if(insOverpayResult.StringBuilderErrors.Length==0) {
						PaymentEdit.InsertInsOverpayResult(insOverpayResult,$"Created via Insurance Overpayments within the Family Balancer tool.");
						PaymentCount+=insOverpayResult.ListPayNumPaySplitsGroups.Count;
						if(insOverpayResult.StringBuilderVerbose.Length==0) {
							ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,"No insurance overpayments detected."));
						}
						else {
							ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,insOverpayResult.StringBuilderVerbose.ToString().Trim()));
						}
					}
					else {
						ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Verbose,insOverpayResult.StringBuilderErrors.ToString().Trim()));
					}
				}
				catch(Exception ex) {
					ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$"CRITICAL ERROR processing the family of guarantor: {guarantor}\r\n  {ex.Message}"));
					continue;
				}
			}
		}
	}

	public class FamilyBalancerOptions {
		///<summary>The date that the user has selected as the date to use on all new payments and splits that are created by this tool.</summary>
		public DateTime DateIncomeTransfer;
		///<summary>The date that income transfer systems will use in order to ignore all account entries that come after this date.</summary>
		public DateTime DateAsOf;
		///<summary>The date that income transfer systems will use in order to ignore all families with financial data that hasn't changed since this date.</summary>
		public DateTime DateChangedSince;
		///<summary>The date that the insurance overpayment system will use on all payments and payment splits created.</summary>
		public DateTime DatePay;
		///<summary>The payment type that the insurance overpayment system will use on all payments created.</summary>
		public long DefNumPayType;
		///<summary>Indicates that this tool should delete all income transfers for the family prior to executing the income transfer logic.</summary>
		public bool DoDeleteTransfers;
		///<summary>If true, runs rigorous income transfer. If false, runs FIFO.</summary>
		public bool IsRigorous;
		public bool UseDateChangedSince;
	}

	public class OutputMessage {
		public LogLevel LogLevel;
		public string Text;

		public OutputMessage(LogLevel logLevel,string text) {
			LogLevel=logLevel;
			Text=text;
		}
	}

	public class ClaimTransferBreakdown {
		public long ClaimNum;
		public int CountNewClaimProcs;
	}
}
