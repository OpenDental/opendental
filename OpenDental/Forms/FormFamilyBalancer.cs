using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Misc;
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
		///<summary>The thread that executes a rigorous income transfer for every family in the database.</summary>
		private ODThread _threadRigorousBalancer;
		///<summary>The thread that executes a FIFO income transfer for every family in the database.</summary>
		private ODThread _threadFifoBalancer;
		///<summary>The thread that executes a FIFO income transfer for every family in the database.</summary>
		private ODThread _threadRecreate;
		///<summary>The thread that creates "income transfers" for every overpaid claim in the database.</summary>
		private ODThread _threadInsuranceOverpayment;
		///<summary>A detailed log of everything that has happened since this window has been opened. Useful if the user wants to make a log file out of what happened.</summary>
		private StringBuilder _stringBuilderLog=new StringBuilder();

		private FamilyBalancer _familyBalancer;

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
			_familyBalancer=new FamilyBalancer();
			_familyBalancer.PatNum=PatNum;
			comboOverpayPayType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaymentTypes,true));
		}

		private bool AreOtherThreadsRunning(ODThread threadToExclude) {
			List<ODThread> listThreads=new List<ODThread>() {
				_threadRigorousBalancer,
				_threadFifoBalancer,
				_threadRecreate,
				_threadInsuranceOverpayment,
			};
			listThreads.Remove(threadToExclude);
			return listThreads.Any(x => x!=null);
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
			butOverpay.Enabled=isEnabled;
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
			if(_familyBalancer.ConcurrentQueueGuarantors==null) {
				return;
			}
			progressBarTransfer.Maximum=_familyBalancer.GuarantorTotal;
			int progressBarValue=(_familyBalancer.GuarantorTotal-_familyBalancer.ConcurrentQueueGuarantors.Count);
			progressBarTransfer.Value=progressBarValue;
			//Use the StringFormat N0 so that commas show for larger numbers.
			labelProgress.Text=$"{progressBarValue:N0} / {_familyBalancer.GuarantorTotal:N0}";
			if(_familyBalancer.ConcurrentQueueGuarantors.Count==0) {
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Finished processing all families at {DateTime.Now}"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				labelProgress.Text="Done";
				butRigorous.Text="Start Rigorous";
				butFIFO.Text="Start FIFO";
				butRecreate.Text="Start Recreate";
				butOverpay.Text="Start Overpay";
				SetEnabledOnInteractableControls(true);
				_threadRigorousBalancer=null;
				_threadFifoBalancer=null;
				_threadRecreate=null;
				_threadInsuranceOverpayment=null;
				_familyBalancer.ConcurrentQueueGuarantors=null;
			}
			labelPayments.Text=$"{_familyBalancer.PaymentCount:N0} New Payments";
		}

		private void timerOutput_Tick(object sender,EventArgs e) {
			StringBuilder stringBuilderOutput=new StringBuilder();
			while(_familyBalancer.ConcurrentQueueOutputMessages.TryDequeue(out OutputMessage outputMessage)) {
				if(!checkOutputVerbose.Checked && outputMessage.LogLevel!=LogLevel.Error) {
					continue;
				}
				stringBuilderOutput.AppendLine(outputMessage.Text);
				_stringBuilderLog.AppendLine(outputMessage.Text);
			}
			if(stringBuilderOutput.Length<=0) {
				return;//No new output messages were in the queue.
			}
			if(textOutput.TextLength > 0) {
				textOutput.AppendText("\r\n");
			}
			textOutput.AppendText(stringBuilderOutput.ToString().Trim());
			//Trim the text box content when it gets too long (the UI will start slowing down).
			int lineCount=textOutput.Lines.Length;
			int lineLimit=1000;
			if(lineCount > lineLimit) {
				string[] stringArrayNewLines=new string[lineLimit];
				Array.Copy(textOutput.Lines,lineCount-lineLimit,stringArrayNewLines,0,lineLimit);
				textOutput.Lines=stringArrayNewLines;
			}
			//Automatically put the cursor at the end of the text box which will automatically scroll the most recent content into view.
			textOutput.SelectionStart=textOutput.Text.Length-1;
			textOutput.SelectionLength=0;
		}

		#region Income Transfers

		private void InstantiateRigorousBalancerThread() {
			if(_threadRigorousBalancer!=null && !_threadRigorousBalancer.HasQuit) {
				return;
			}
			_threadRigorousBalancer=new ODThread(_familyBalancer.ThreadWorkerIncomeTransfer);
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
			_threadFifoBalancer=new ODThread(_familyBalancer.ThreadWorkerIncomeTransfer);
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
			if(!Security.IsAuthorized(EnumPermType.PaymentCreate,datePickerIncomeTransferDate.Value.Date)) {
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

		private void checkChangedSinceDate_CheckedChanged(object sender,EventArgs e) {
			labelChangedSinceDate.Enabled=checkChangedSinceDate.Checked;
			datePickerChangedSinceDate.Enabled=checkChangedSinceDate.Checked;
		}

		private bool CanRunWithAutoEnabled() {
			if(!PrefC.GetBool(PrefName.FamilyBalancerEnabled)) {
				return true;
			}
			return MsgBox.Show(this,MsgBoxButtons.YesNo,"Income Transfer is configured to run automatically. Would you still like to continue?");
		}

		private void butRigorous_Click(object sender,EventArgs e) {
			if(!CanRunWithAutoEnabled()) {
				return;
			}
			if(AreOtherThreadsRunning(_threadRigorousBalancer)) {
				MsgBox.Show(this,"Cannot make Rigorous transfers once another process has started.");
				return;
			}
			if(_threadRigorousBalancer==null) {
				if(!IsValidIncomeTransferUI()) {
					return;
				}
				_familyBalancer.PaymentCount=0;
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
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Rigorous thread paused at {DateTime.Now}"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butRigorous.Text="Start Rigorous";
			}
		}

		private void butFIFO_Click(object sender,EventArgs e) {
			if(!CanRunWithAutoEnabled()) {
				return;
			}
			if(AreOtherThreadsRunning(_threadFifoBalancer)) {
				MsgBox.Show(this,"Cannot make FIFO transfers once another process has started.");
				return;
			}
			if(_threadFifoBalancer==null) {
				if(!IsValidIncomeTransferUI()) {
					return;
				}
				_familyBalancer.PaymentCount=0;
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
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"FIFO thread paused at {DateTime.Now}"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butFIFO.Text="Start FIFO";
			}
		}

		#endregion

		#region Recreate Splits

		private void InstantiateRecreateThread() {
			if(_threadRecreate!=null && !_threadRecreate.HasQuit) {
				return;
			}
			_threadRecreate=new ODThread(_familyBalancer.ThreadWorkerRecreate);
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

		private void butRecreate_Click(object sender,EventArgs e) {
			if(AreOtherThreadsRunning(_threadRecreate)) {
				MsgBox.Show(this,"Cannot make Recreate Splits once another process has started.");
				return;
			}
			if(_threadRecreate==null) {
				if(!IsValidRecreateSplitsUI()) {
					return;
				}
				_familyBalancer.PaymentCount=0;
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
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Recreate thread paused at {DateTime.Now}"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butRecreate.Text="Start Recreate";
			}
		}

		#endregion

		#region Insurance Overpayments

		private void InstantiateOverpaymentThread() {
			if(_threadInsuranceOverpayment!=null && !_threadInsuranceOverpayment.HasQuit) {
				return;
			}
			_threadInsuranceOverpayment=new ODThread(_familyBalancer.ThreadWorkerOverpay);
			_threadInsuranceOverpayment.Tag=new FamilyBalancerOptions() {
				DatePay=datePickerOverpayPayDate.Value,
				DefNumPayType=comboOverpayPayType.GetSelectedDefNum(),
			};
			_threadInsuranceOverpayment.Name="FamilyBalancerOverpaymentThread";
			_threadInsuranceOverpayment.AddExceptionHandler(ex => ex.DoNothing());
		}

		private bool IsValidOverpaymentUI() {
			if(comboOverpayPayType.GetSelectedDefNum() < 1) {
				MsgBox.Show(this,"Select a valid Pay Type.");
				return false;
			}
			//Make sure the user wants to run the tool.
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This process cannot be undone.\r\n\r\nContinue?")) {
				return false;
			}
			return true;
		}

		private void butOverpay_Click(object sender,EventArgs e) {
			if(AreOtherThreadsRunning(_threadInsuranceOverpayment)) {
				MsgBox.Show(this,"Cannot transfer Insurance Overpayments once another process has started.");
				return;
			}
			if(_threadInsuranceOverpayment==null) {
				if(!IsValidOverpaymentUI()) {
					return;
				}
				_familyBalancer.PaymentCount=0;
				textOutput.Clear();
				//Disable the UI so that it cannot change while the thread is working.
				SetEnabledOnInteractableControls(false);
				//However, leave the overpay button enabled so that the process can be paused and started at will.
				butOverpay.Enabled=true;
				//Start the progress timer and let it run for the rest of the life of this form.
				timerProgress.Start();
				//Instantiate the thread so that we never get back into this if statement again. The thread will be told to start later.
				InstantiateOverpaymentThread();
			}
			if(butOverpay.Text=="Start Overpay") {
				//Since the user has the ability to start and stop this tool, check to see if the thread was manually stopped (HasQuit==true).
				if(_threadInsuranceOverpayment.HasQuit) {
					//Instantiate a new thread that will pick up where the last one left off. This is because we do not have access to flip HasQuit to false.
					InstantiateOverpaymentThread();
				}
				_threadInsuranceOverpayment.Start();
				butOverpay.Text="Pause Overpay";
			}
			else {
				_threadInsuranceOverpayment.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"Overpay thread paused at {DateTime.Now}"));
				_familyBalancer.ConcurrentQueueOutputMessages.Enqueue(new OutputMessage(LogLevel.Error,$@"======================================================================"));
				butOverpay.Text="Start Overpay";
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

		private void butConfigureAuto_Click(object sender,EventArgs e) {
			FrmFamilyBalancerSetup frmFamilyBalancerAutoEdit=new FrmFamilyBalancerSetup();
			frmFamilyBalancerAutoEdit.ShowDialog();
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
			if(_threadInsuranceOverpayment!=null && !_threadInsuranceOverpayment.HasQuit) {
				_threadInsuranceOverpayment.QuitSync(5000);//Give the thread up to five seconds to quit before aborting.
			}
		}
	}
}