using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFamilyBalancer:FormODBase {
		///<summary>Set to the total number of guarantors in the database by the first balancer thread that runs.</summary>
		private int _guarantorTotal;
		///<summary>The number of new payments made by the balancer thread.</summary>
		private int _paymentCount;
		///<summary>The thread that executes a rigorous income transfer for every family in the database.</summary>
		private ODThread _threadRigorousBalancer;
		///<summary>The thread that executes a FIFO income transfer for every family in the database.</summary>
		private ODThread _threadFifoBalancer;
		///<summary>A queue of PatNums for guarantors that need to be processed by the balancer thread.</summary>
		private ConcurrentQueue<long> _queueGuarantors;

		public FormFamilyBalancer() {
			InitializeComponent();
			InitializeLayoutManager();
			labelProgress.Text="";
			labelPayments.Text="";
			datePickerIncomeTransferDate.Value=DateTime.Now;
			datePickerIncomeTransferDate.MaxDate=DateTime.Now;//Users can only pick dates in the past.
			datePickerAsOfDate.Value=DateTime.Now;
			datePickerChangedSinceDate.Value=DateTime.Now.AddDays(-14);//Designed for bi-weekly usage.
		}

		private void timerProgress_Tick(object sender,EventArgs e) {
			if(_queueGuarantors==null) {
				return;
			}
			progressBarTransfer.Maximum=_guarantorTotal;
			int progressBarValue=(_guarantorTotal-_queueGuarantors.Count);
			progressBarTransfer.Value=progressBarValue;
			//Use the StringFormat N0 so that commas show for larger numbers.
			labelProgress.Text=$"{progressBarValue:N0} / {_guarantorTotal:N0}";
			if(_queueGuarantors.Count==0) {
				labelProgress.Text="Done";
				butRigorous.Text="Start Rigorous";
				butFIFO.Text="Start FIFO";
				SetEnabledOnInteractableControls(true);
				_threadRigorousBalancer=null;
				_threadFifoBalancer=null;
				_queueGuarantors=null;
			}
			labelPayments.Text=$"{_paymentCount:N0} New Payments";
		}

		private void InstantiateRigorousBalancerThread() {
			if(_threadRigorousBalancer!=null && !_threadRigorousBalancer.HasQuit) {
				return;
			}
			_threadRigorousBalancer=new ODThread(ThreadWorker);
			_threadRigorousBalancer.Tag=new FamilyBalancerOptions() {
				DateIncomeTransfer=datePickerIncomeTransferDate.Value,
				DateAsOf=datePickerAsOfDate.Value,
				DateChangedSince=datePickerChangedSinceDate.Value,
				DoDeleteTransfers=checkDeleteTransfers.Checked,
				IsRigorous=true,
				UseDateChangedSince=checkChangedSinceDate.Checked,
			};
			_threadRigorousBalancer.Name="FamilyBalancerRigorousThread";
			_threadRigorousBalancer.AddExceptionHandler(ex => ex.DoNothing());
		}

		private void InstantiateFifoBalancerThread() {
			if(_threadFifoBalancer!=null && !_threadFifoBalancer.HasQuit) {
				return;
			}
			_threadFifoBalancer=new ODThread(ThreadWorker);
			_threadFifoBalancer.Tag=new FamilyBalancerOptions() {
				DateIncomeTransfer=datePickerIncomeTransferDate.Value,
				DateAsOf=datePickerAsOfDate.Value,
				DateChangedSince=datePickerChangedSinceDate.Value,
				DoDeleteTransfers=checkDeleteTransfers.Checked,
				IsRigorous=false,
				UseDateChangedSince=checkChangedSinceDate.Checked,
			};
			_threadFifoBalancer.Name="FamilyBalancerFifoThread";
			_threadFifoBalancer.AddExceptionHandler(ex => ex.DoNothing());
		}

		private bool IsValid() {
			//FormIncomeTransferManage requires PaymentCreate to run.
			//This form requires SecurityAdmin and a password to open, and although rare, a SecuirtyAdmin doesn't have to have PaymentCreate permission.
			if(!Security.IsAuthorized(Permissions.PaymentCreate,datePickerIncomeTransferDate.Value.Date)) {
				return false;
			}
			if(checkDeleteTransfers.Checked) {
				//Purposefully give a warning message that the user has to say No to in order to continue. Make sure they read it and understand.
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Data may be deleted due to 'Delete Prior Transfers' being checked.\r\n"
					+"Would you like to go make a manual backup first?"))
				{
					return false;
				}
			}
			//Make sure the user wants to run the tool.
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This process can take a long time.\r\n\r\nContinue?")) {
				return false;
			}
			return true;
		}

		private void ThreadWorker(ODThread thread) {
			if(thread.Tag==null || thread.Tag.GetType()!=typeof(FamilyBalancerOptions)) {
				return;
			}
			FamilyBalancerOptions familyBalanceOptions=(FamilyBalancerOptions)thread.Tag;
			if(_queueGuarantors==null) {
				//Fill the queue of guarantors that the thread will use as a way to determine if it has more work to do.
				List<long> listGuarantors;
				if(familyBalanceOptions.UseDateChangedSince) {
					listGuarantors=Patients.GetGuarantorsWithFinancialDataChangedAfterDate(familyBalanceOptions.DateChangedSince);
				}
				else {
					listGuarantors=Patients.GetAllGuarantors();
				}
				_guarantorTotal=listGuarantors.Count;
				_queueGuarantors=new ConcurrentQueue<long>(listGuarantors);
			}
			string strLogic="FIFO logic";
			if(familyBalanceOptions.IsRigorous) {
				strLogic="Rigorous logic";
			}
			while(!thread.HasQuit && _queueGuarantors!=null && _queueGuarantors.TryDequeue(out long guarantor)) {
				Family fam=Patients.GetFamily(guarantor);
				Patient patCur=fam.GetPatient(guarantor);
				if(familyBalanceOptions.DoDeleteTransfers) {
					PaymentEdit.DeleteTransfersForFamily(fam);
				}
				try {
					PaymentEdit.TransferClaimsPayAsTotal(patCur.PatNum,fam.GetPatNums(),"Automatic transfer of claims pay as total from family balancer.");
				}
				catch(Exception) {
					continue;
				}
				List<PaySplit> listPaySplits;
				if(familyBalanceOptions.IsRigorous) {
					PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(fam.GetPatNums(),patCur.PatNum,
						new List<PaySplit>(),new Payment(),new List<AccountEntry>(),isIncomeTxfr:true,dateAsOf:familyBalanceOptions.DateAsOf);
					if(!PaymentEdit.TryCreateIncomeTransfer(constructResults.ListAccountCharges,familyBalanceOptions.DateIncomeTransfer,out PaymentEdit.IncomeTransferData data)) {
						continue;
					}
					listPaySplits=data.ListSplitsCur;
				}
				else {//FIFO
					try {
						PaymentEdit.IncomeTransferData incomeTransferData=PaymentEdit.GetIncomeTransferDataFIFO(patCur.PatNum,familyBalanceOptions.DateIncomeTransfer,
							dateAsOf:familyBalanceOptions.DateAsOf);
						listPaySplits=incomeTransferData.ListSplitsCur;
					}
					catch(Exception ex) {
						ex.DoNothing();
						continue;
					}
				}
				if(listPaySplits.IsNullOrEmpty()) {
					continue;
				}
				Payment paymentCur=new Payment();
				paymentCur.PayDate=familyBalanceOptions.DateIncomeTransfer;
				paymentCur.PatNum=patCur.PatNum;
				//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
				//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
				paymentCur.ClinicNum=0;
				if(PrefC.HasClinicsEnabled) {
					paymentCur.ClinicNum=Clinics.ClinicNum;
					if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
						paymentCur.ClinicNum=patCur.ClinicNum;
					}
					else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
						paymentCur.ClinicNum=(Clinics.ClinicNum==0 ? patCur.ClinicNum : Clinics.ClinicNum);
					}
				}
				paymentCur.DateEntry=DateTime.Today;
				paymentCur.PaymentSource=CreditCardSource.None;
				paymentCur.ProcessStatus=ProcessStat.OfficeProcessed;
				paymentCur.PayAmt=0;
				paymentCur.PayType=0;
				Payments.Insert(paymentCur,listPaySplits);
				SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,patCur.PatNum,$"Income transfer created by the Family Balancer tool using {strLogic}.");
				_paymentCount++;
				Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(patCur.PatNum,listPaySplits);
			}
		}

		private void SetEnabledOnInteractableControls(bool isEnabled) {
			//Date Pickers
			datePickerIncomeTransferDate.Enabled=isEnabled;
			datePickerAsOfDate.Enabled=isEnabled;
			datePickerChangedSinceDate.Enabled=isEnabled;
			//Check Boxes
			checkDeleteTransfers.Enabled=isEnabled;
			checkChangedSinceDate.Enabled=isEnabled;
			//Buttons
			butRigorous.Enabled=isEnabled;
			butFIFO.Enabled=isEnabled;
			//Labels
			labelAsOfDate.Enabled=isEnabled;
			labelAsOfDateDesc.Enabled=isEnabled;
			labelChangedSinceDate.Enabled=isEnabled;
			labelDeleteTransfers.Enabled=isEnabled;
			labelIncomeTransferDate.Enabled=isEnabled;
			labelIncomeTransferDateDesc.Enabled=isEnabled;
			//Changed Since Date controls are special and need to be disabled if their corresponding check box is not checked and calling method is enabling interactable controls.
			if(isEnabled && !checkChangedSinceDate.Checked) {
				datePickerChangedSinceDate.Enabled=false;
				labelChangedSinceDate.Enabled=false;
			}
		}

		private void checkChangedSinceDate_CheckedChanged(object sender,EventArgs e) {
			labelChangedSinceDate.Enabled=checkChangedSinceDate.Checked;
			datePickerChangedSinceDate.Enabled=checkChangedSinceDate.Checked;
		}

		private void butRigorous_Click(object sender,EventArgs e) {
			if(_threadFifoBalancer!=null) {
				MsgBox.Show(this,"Cannot make Rigorous transfers once FIFO transfers have started.");
				return;
			}
			if(_threadRigorousBalancer==null) {
				if(!IsValid()) {
					return;
				}
				_paymentCount=0;
				//Disable the UI so that it cannot change while the thread is working.
				SetEnabledOnInteractableControls(false);
				//However, leave the rigorous button enabled so that the process can be paused and started at will.
				butRigorous.Enabled=true;
				//Start the progress timer and let it run for the rest of the life of this form.
				timerProgress.Start();
				//Instantiate the thread so that we never get back into this if statement again. The thread will be told to start later.
				InstantiateRigorousBalancerThread();
			}
			if(butRigorous.Text=="Start Rigorous") {
				//Since the user has the ability to start and stop this tool, check to see if the thread was manually stopped (HasQuit==true).
				if(_threadRigorousBalancer.HasQuit) {
					//Instantiate a new thread that will pick up where the last one left off. This is because we do not have access to flip HasQuit to false.
					InstantiateRigorousBalancerThread();
				}
				_threadRigorousBalancer.Start();
				butRigorous.Text="Pause Rigorous";
			}
			else {
				_threadRigorousBalancer.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
				butRigorous.Text="Start Rigorous";
			}
		}

		private void butFIFO_Click(object sender,EventArgs e) {
			if(_threadRigorousBalancer!=null) {
				MsgBox.Show(this,"Cannot make FIFO transfers once Rigorous transfers have started.");
				return;
			}
			if(_threadFifoBalancer==null) {
				if(!IsValid()) {
					return;
				}
				_paymentCount=0;
				//Disable the UI so that it cannot change while the thread is working.
				SetEnabledOnInteractableControls(false);
				//However, leave the FIFO button enabled so that the process can be paused and started at will.
				butFIFO.Enabled=true;
				//Start the progress timer and let it run for the rest of the life of this form.
				timerProgress.Start();
				//Instantiate the thread so that we never get back into this if statement again. The thread will be told to start later.
				InstantiateFifoBalancerThread();
			}
			if(butFIFO.Text=="Start FIFO") {
				//Since the user has the ability to start and stop this tool, check to see if the thread was manually stopped (HasQuit==true).
				if(_threadFifoBalancer.HasQuit) {
					//Instantiate a new thread that will pick up where the last one left off. This is because we do not have access to flip HasQuit to false.
					InstantiateFifoBalancerThread();
				}
				_threadFifoBalancer.Start();
				butFIFO.Text="Pause FIFO";
			}
			else {
				_threadFifoBalancer.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
				butFIFO.Text="Start FIFO";
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormFamilyBalancer_FormClosing(object sender,FormClosingEventArgs e) {
			if(_threadRigorousBalancer!=null && !_threadRigorousBalancer.HasQuit) {
				_threadRigorousBalancer.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
			}
			if(_threadFifoBalancer!=null && !_threadFifoBalancer.HasQuit) {
				_threadFifoBalancer.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
			}
		}

		private class FamilyBalancerOptions {
			///<summary>The date that the user has selected as the date to use on all new payments and splits that are created by this tool.</summary>
			public DateTime DateIncomeTransfer;
			///<summary>The date that income transfer systems will use in order to ignore all account entries that come after this date.</summary>
			public DateTime DateAsOf;
			///<summary>The date that income transfer systems will use in order to ignore all families with financial data that hasn't changed since this date.</summary>
			public DateTime DateChangedSince;
			///<summary>Indicates that this tool should delete all income transfers for the family prior to executing the income transfer logic.</summary>
			public bool DoDeleteTransfers;
			public bool IsRigorous;
			public bool UseDateChangedSince;
		}
	}
}

