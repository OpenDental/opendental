using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFamilyBalancer:FormODBase {
		///<summary>Setting this field before loading the window will cause all tools within to perform their functions against the family of this PatNum only.</summary>
		public long PatNum;
		///<summary>Set to the total number of guarantors in the database by the first balancer thread that runs.</summary>
		private int _guarantorTotal;
		///<summary>The number of new payments made by the balancer thread.</summary>
		private int _paymentCount;
		///<summary>The thread that executes a rigorous income transfer for every family in the database.</summary>
		private ODThread _threadRigorousBalancer;
		///<summary>The thread that executes a FIFO income transfer for every family in the database.</summary>
		private ODThread _threadFifoBalancer;
		///<summary>The thread that executes a FIFO income transfer for every family in the database.</summary>
		private ODThread _threadRecreate;
		///<summary>A queue of PatNums for guarantors that need to be processed by the balancer thread.</summary>
		private ConcurrentQueue<long> _queueGuarantors;
		///<summary>A queue of text that needs to be appended to the Output text box.</summary>
		private ConcurrentQueue<OutputMessage> _queueOutput=new ConcurrentQueue<OutputMessage>();
		///<summary>A detailed log of everything that has happened since this window has been opened. Useful if the user wants to make a log file out of what happened.</summary>
		private StringBuilder _stringBuilderLog=new StringBuilder();

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

		private void FormFamilyBalancer_Load(object sender,EventArgs e) {
			if(PatNum > 0) {
				this.Text+=" - "+Lan.g(this,"Single Family Mode");
			}
		}

		private bool AreOtherThreadsRunning(ODThread threadToExclude) {
			List<ODThread> listThreads=new List<ODThread>() {
				_threadRigorousBalancer,
				_threadFifoBalancer,
				_threadRecreate,
			};
			listThreads.Remove(threadToExclude);
			return listThreads.Any(x => x!=null);
		}

		private void FillGuarantorQueue(FamilyBalancerOptions familyBalanceOptions) {
			if(_queueGuarantors==null) {
				List<long> listGuarantors;
				if(PatNum > 0) {//Single Family Mode
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$"Getting the guarantor for the family of PatNum:{PatNum}..."));
					listGuarantors=Patients.GetGuarantorsForPatNums(ListTools.FromSingle(PatNum));
				}
				else if(familyBalanceOptions.UseDateChangedSince) {//Families with financial data changed since a specified date.
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,
						$"Getting the guarantors of families with financial data changed since:{familyBalanceOptions.DateChangedSince.ToShortDateString()}..."));
					listGuarantors=Patients.GetGuarantorsWithFinancialDataChangedAfterDate(familyBalanceOptions.DateChangedSince);
				}
				else {//Entire Database
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,"Getting every guarantor in the database..."));
					listGuarantors=Patients.GetAllGuarantors();
				}
				_guarantorTotal=listGuarantors.Count;
				//Fill the queue of guarantors that the thread will use as a way to determine if it has more work to do.
				_queueGuarantors=new ConcurrentQueue<long>(listGuarantors);
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$"{_guarantorTotal:N0} guarantors queued for processing."));
			}
			else {
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,"Unprocessed guarantors detected. Continuing to process instead of starting over."));
			}
		}

		private void SetEnabledOnInteractableControls(bool isEnabled) {
			//Date Pickers
			datePickerIncomeTransferDate.Enabled=isEnabled;
			datePickerAsOfDate.Enabled=isEnabled;
			datePickerAsOfDateRecreate.Enabled=isEnabled;
			datePickerChangedSinceDate.Enabled=isEnabled;
			//Check Boxes
			checkDeleteTransfers.Enabled=isEnabled;
			checkDeleteTransfersRecreate.Enabled=isEnabled;
			checkChangedSinceDate.Enabled=isEnabled;
			//Buttons
			butRigorous.Enabled=isEnabled;
			butFIFO.Enabled=isEnabled;
			butRecreate.Enabled=isEnabled;
			//Labels
			labelAsOfDate.Enabled=isEnabled;
			labelAsOfDateRecreate.Enabled=isEnabled;
			labelAsOfDateDesc.Enabled=isEnabled;
			labelAsOfDateDescRecreate.Enabled=isEnabled;
			labelChangedSinceDate.Enabled=isEnabled;
			labelDeleteTransfers.Enabled=isEnabled;
			labelDeleteTransfersRecreate.Enabled=isEnabled;
			labelIncomeTransferDate.Enabled=isEnabled;
			labelIncomeTransferDateDesc.Enabled=isEnabled;
			//Changed Since Date controls are special and need to be disabled if their corresponding check box is not checked and calling method is enabling interactable controls.
			if(isEnabled && !checkChangedSinceDate.Checked) {
				datePickerChangedSinceDate.Enabled=false;
				labelChangedSinceDate.Enabled=false;
			}
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
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Finished processing all families at {DateTime.Now}"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				labelProgress.Text="Done";
				butRigorous.Text="Start Rigorous";
				butFIFO.Text="Start FIFO";
				butRecreate.Text="Start Recreate";
				SetEnabledOnInteractableControls(true);
				_threadRigorousBalancer=null;
				_threadFifoBalancer=null;
				_threadRecreate=null;
				_queueGuarantors=null;
			}
			labelPayments.Text=$"{_paymentCount:N0} New Payments";
		}

		private void timerOutput_Tick(object sender,EventArgs e) {
			while(_queueOutput.TryDequeue(out OutputMessage outputMessage)) {
				if(!checkOutputVerbose.Checked && outputMessage.LogLevel!=LogLevel.Error) {
					continue;
				}
				if(textOutput.TextLength > 0) {
					textOutput.AppendText("\r\n");
					_stringBuilderLog.Append("\r\n");
				}
				textOutput.AppendText(outputMessage.Text);
				_stringBuilderLog.Append(outputMessage.Text);
			}
			//Trim the text box content when it gets too long (the UI will start slowing down).
			int lineCount=textOutput.Lines.Length;
			int lineLimit=1000;
			if(lineCount > lineLimit) {
				string[] newlines=new string[lineLimit];
				Array.Copy(textOutput.Lines,lineCount-lineLimit,newlines,0,lineLimit);
				textOutput.Lines=newlines;
				textOutput.SelectionStart=textOutput.Text.Length-1;
				textOutput.SelectionLength=0;
			}
		}

		#region Income Transfers

		private void InstantiateRigorousBalancerThread() {
			if(_threadRigorousBalancer!=null && !_threadRigorousBalancer.HasQuit) {
				return;
			}
			_threadRigorousBalancer=new ODThread(ThreadWorkerIncomeTransfer);
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
			_threadFifoBalancer=new ODThread(ThreadWorkerIncomeTransfer);
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

		private bool IsValidIncomeTransferUI() {
			//FormIncomeTransferManage requires PaymentCreate to run.
			//This form requires SecurityAdmin and a password to open, and although rare, a SecuirtyAdmin doesn't have to have PaymentCreate permission.
			if(!Security.IsAuthorized(Permissions.PaymentCreate,datePickerIncomeTransferDate.Value.Date)) {
				return false;
			}
			if(checkDeleteTransfers.Checked) {
				//Purposefully give a warning message that the user has to say No to in order to continue. Make sure they read it and understand.
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Data may be deleted due to 'Delete All Transfers' being checked.\r\n"
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

		private void ThreadWorkerIncomeTransfer(ODThread thread) {
			if(thread.Tag==null || thread.Tag.GetType()!=typeof(FamilyBalancerOptions)) {
				return;
			}
			FamilyBalancerOptions familyBalanceOptions=(FamilyBalancerOptions)thread.Tag;
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Income Transfer thread started at {DateTime.Now} with the following settings:"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Income Transfer Date: {familyBalanceOptions.DateIncomeTransfer.ToShortDateString()}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"As of Date: {familyBalanceOptions.DateAsOf.ToShortDateString()}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Delete All Transfers: {(familyBalanceOptions.DoDeleteTransfers ? "True" : "False")}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Use Date Changed Since: {(familyBalanceOptions.UseDateChangedSince ? "True" : "False")}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Date Changed Since: {familyBalanceOptions.DateChangedSince.ToShortDateString()}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Is Rigorous: {(familyBalanceOptions.IsRigorous ? "True" : "False")}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			FillGuarantorQueue(familyBalanceOptions);
			string strLogic="FIFO logic";
			if(familyBalanceOptions.IsRigorous) {
				strLogic="Rigorous logic";
			}
			while(!thread.HasQuit && _queueGuarantors!=null && _queueGuarantors.TryDequeue(out long guarantor)) {
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Processing the family of guarantor: {guarantor} ------------------"));
				Family fam=Patients.GetFamily(guarantor);
				Patient patCur=fam.GetPatient(guarantor);
				if(familyBalanceOptions.DoDeleteTransfers) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Deleting transfers for family..."));
					PaymentEdit.DeleteTransfersForFamily(fam.GetPatNums());
				}
				try {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Transferring claim pay as totals..."));
					PaymentEdit.TransferClaimsPayAsTotal(patCur.PatNum,fam.GetPatNums(),"Automatic transfer of claims pay as total from family balancer.");
				}
				catch(Exception ex) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"CRITICAL ERROR: {ex.Message}"));
					continue;
				}
				List<PaySplit> listPaySplits;
				if(familyBalanceOptions.IsRigorous) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Rigorous income transfer logic running..."));
					PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(fam.GetPatNums(),patCur.PatNum,
						new List<PaySplit>(),new Payment(),new List<AccountEntry>(),isIncomeTxfr:true,dateAsOf:familyBalanceOptions.DateAsOf);
					if(!PaymentEdit.TryCreateIncomeTransfer(constructResults.ListAccountEntries,familyBalanceOptions.DateIncomeTransfer,out PaymentEdit.IncomeTransferData data)) {
						_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Failed to create an income transfer due to the following:"));
						if(data.StringBuilderWarnings.Length > 0) {
							_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Warning - {data.StringBuilderWarnings}"));
						}
						if(data.StringBuilderErrors.Length > 0) {
							_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Error - {data.StringBuilderErrors}"));
						}
						continue;
					}
					listPaySplits=data.ListSplitsCur;
				}
				else {//FIFO
					try {
						_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"FIFO income transfer logic running..."));
						PaymentEdit.IncomeTransferData incomeTransferData=PaymentEdit.GetIncomeTransferDataFIFO(patCur.PatNum,familyBalanceOptions.DateIncomeTransfer,
							dateAsOf:familyBalanceOptions.DateAsOf);
						listPaySplits=incomeTransferData.ListSplitsCur;
					}
					catch(Exception ex) {
						_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"CRITICAL ERROR: {ex.Message}"));
						continue;
					}
				}
				if(listPaySplits.IsNullOrEmpty()) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"No income transfer needed for family."));
					continue;
				}
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"An income transfer with {listPaySplits.Count} payment splits was created to balance the family account."));
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

		private void checkChangedSinceDate_CheckedChanged(object sender,EventArgs e) {
			labelChangedSinceDate.Enabled=checkChangedSinceDate.Checked;
			datePickerChangedSinceDate.Enabled=checkChangedSinceDate.Checked;
		}

		private void butRigorous_Click(object sender,EventArgs e) {
			if(AreOtherThreadsRunning(_threadRigorousBalancer)) {
				MsgBox.Show(this,"Cannot make Rigorous transfers once another process has started.");
				return;
			}
			if(_threadRigorousBalancer==null) {
				if(!IsValidIncomeTransferUI()) {
					return;
				}
				_paymentCount=0;
				textOutput.Clear();
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
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Rigorous thread paused at {DateTime.Now}"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butRigorous.Text="Start Rigorous";
			}
		}

		private void butFIFO_Click(object sender,EventArgs e) {
			if(AreOtherThreadsRunning(_threadFifoBalancer)) {
				MsgBox.Show(this,"Cannot make FIFO transfers once another process has started.");
				return;
			}
			if(_threadFifoBalancer==null) {
				if(!IsValidIncomeTransferUI()) {
					return;
				}
				_paymentCount=0;
				textOutput.Clear();
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
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"FIFO thread paused at {DateTime.Now}"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butFIFO.Text="Start FIFO";
			}
		}

		#endregion

		#region Recreate Splits

		private void InstantiateRecreateThread() {
			if(_threadRecreate!=null && !_threadRecreate.HasQuit) {
				return;
			}
			_threadRecreate=new ODThread(ThreadWorkerRecreate);
			_threadRecreate.Tag=new FamilyBalancerOptions() {
				DateAsOf=datePickerAsOfDateRecreate.Value,
				DoDeleteTransfers=checkDeleteTransfersRecreate.Checked,
			};
			_threadRecreate.Name="FamilyBalancerRecreateThread";
			_threadRecreate.AddExceptionHandler(ex => ex.DoNothing());
		}

		private bool IsValidRecreateSplitsUI() {
			if(checkDeleteTransfersRecreate.Checked) {
				//Purposefully give a warning message that the user has to say No to in order to continue. Make sure they read it and understand.
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Data may be deleted due to 'Delete All Transfers' being checked.\r\n"
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

		private void ThreadWorkerRecreate(ODThread thread) {
			if(thread.Tag==null || thread.Tag.GetType()!=typeof(FamilyBalancerOptions)) {
				return;
			}
			FamilyBalancerOptions familyBalanceOptions=(FamilyBalancerOptions)thread.Tag;
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Recreate thread started at {DateTime.Now} with the following settings:"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"As of Date: {familyBalanceOptions.DateAsOf.ToShortDateString()}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Delete All Transfers: {(familyBalanceOptions.DoDeleteTransfers ? "True" : "False")}"));
			_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
			FillGuarantorQueue(familyBalanceOptions);
			while(!thread.HasQuit && _queueGuarantors!=null && _queueGuarantors.TryDequeue(out long guarantor)) {
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Processing the family of guarantor: {guarantor} ------------------"));
				if(familyBalanceOptions.DoDeleteTransfers) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Deleting transfers for family..."));
					List<long> listPatNums=Patients.GetFamily(guarantor).GetPatNums();
					PaymentEdit.DeleteTransfersForFamily(listPatNums,isPayTypeIgnored:true);
				}
				try {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Verbose,$@"Recreating all payment splits..."));
					PaymentEdit.ReAutoSplitAllPaymentsForFamily(guarantor,dateAsOf:familyBalanceOptions.DateAsOf);
				}
				catch(ODException odex) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$"Error processing the family of guarantor: {guarantor}\r\n  {odex.Message}"));
					continue;
				}
				catch(Exception ex) {
					_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$"CRITICAL ERROR processing the family of guarantor: {guarantor}\r\n  {ex.Message}"));
					continue;
				}
			}
		}

		private void butRecreate_Click(object sender,EventArgs e) {
			if(AreOtherThreadsRunning(_threadRecreate)) {
				MsgBox.Show(this,"Cannot make Recreate Splits once another process has started.");
				return;
			}
			if(_threadRecreate==null) {
				if(!IsValidRecreateSplitsUI()) {
					return;
				}
				_paymentCount=0;
				textOutput.Clear();
				//Disable the UI so that it cannot change while the thread is working.
				SetEnabledOnInteractableControls(false);
				//However, leave the recreate button enabled so that the process can be paused and started at will.
				butRecreate.Enabled=true;
				//Start the progress timer and let it run for the rest of the life of this form.
				timerProgress.Start();
				//Instantiate the thread so that we never get back into this if statement again. The thread will be told to start later.
				InstantiateRecreateThread();
			}
			if(butRecreate.Text=="Start Recreate") {
				//Since the user has the ability to start and stop this tool, check to see if the thread was manually stopped (HasQuit==true).
				if(_threadRecreate.HasQuit) {
					//Instantiate a new thread that will pick up where the last one left off. This is because we do not have access to flip HasQuit to false.
					InstantiateRecreateThread();
				}
				_threadRecreate.Start();
				butRecreate.Text="Pause Recreate";
			}
			else {
				_threadRecreate.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"Recreate thread paused at {DateTime.Now}"));
				_queueOutput.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butRecreate.Text="Start Recreate";
			}
		}

		#endregion

		private void butLog_Click(object sender,EventArgs e) {
			if(_stringBuilderLog.Length==0) {
				MsgBox.Show("No Output content to log.");
				return;
			}
			SaveFileDialog saveFileDialog=new SaveFileDialog();
			saveFileDialog.Filter="txt files (*.txt)|*.txt|All files (*.*)|*.*";
			saveFileDialog.FilterIndex=2;
			saveFileDialog.FileName=$"FamilyBalancerOutput_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
			saveFileDialog.RestoreDirectory=true;
			if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			try {
				File.WriteAllText(saveFileDialog.FileName,_stringBuilderLog.ToString());
			}
			catch(Exception ex) {
				FriendlyException.Show("Error saving Output content into the specified log file. Please try again.",ex);
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Log file created.  Would you like to open the file?")) {
				try {
					Process.Start(saveFileDialog.FileName);
				}
				catch(Exception ex) {
					FriendlyException.Show("Error opening log file. Please manually open the file.",ex);
					return;
				}
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
			if(_threadRecreate!=null && !_threadRecreate.HasQuit) {
				_threadRecreate.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
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

		private class OutputMessage {
			public LogLevel LogLevel;
			public string Text;

			public OutputMessage(LogLevel logLevel,string text) {
				LogLevel=logLevel;
				Text=text;
			}
		}

	}
}

