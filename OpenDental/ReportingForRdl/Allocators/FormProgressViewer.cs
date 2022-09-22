

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
/*
This Program is free software; you can redistribute it and/or modify it under the terms of the
GNU Db Public License as published by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

This program is distributed in the hope that it will be useful, but without any warranty. See the GNU Db Public License
for more details, available at http://www.opensource.org/licenses/gpl-license.php

Any changes to this program must follow the guidelines of the GPL license if a modified version is to be
redistributed.
*/
namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	/// <summary>
	///	This Form is a simple encapsulation of the BacgroundWorker with a progressbar control.
	/// 
	/// Several buttons are available for users to control the process.
	/// 
	/// Basic plan is for the Programmer to design a method that fits 
	/// the DoWorkEventHandler signature --->void bw_DoWork(object sender, DoWorkEventArgs e)
	/// 
	/// Progress Reporting is enabled
	/// Support for canceling is enabled.
	/// 
	/// Remeber if DoWorkEventArgs e.Cancled is set to true (by you in the bw_DoWork) then
	/// you cannot get the e.Result and thus the _FinalResult object will be null.
	/// 
	/// Steps:
	///		SET_WORKER_METHOD first
	///		START_WORK()
	/// 
	/// Other Features
	///		RUN_ONLY_ONCE		if set to false a Start button pops up and you can re run the method assigned. Default is true
	///		FINAL_RESULT		the value set to e.Result in your DoWork method
	///		CANCLED_BY_USER		true if user cancled activity
	///		MESSAGE_LAST		Last Progress Message assigned.  Will be overwritten by bw_RunWorkerCompleted code
	///		MESSAGES_ALL		A string containing all the Progress Messages sent
	///		CONTINOUS_PROGRESS	The progress bar will just cycle while work is done.  Does not affect messages.
	/// Author:	Daniel W. Krueger
	/// </summary>
	public partial class FormProgressViewer :FormODBase
	{
		private  BackgroundWorker bw = new BackgroundWorker();
		private DoWorkEventHandler _AsyncMethod = null; //void bw_DoWork(object sender, DoWorkEventArgs e)
		private string _Message = ""; // update these instead of form variables.  Then you can control when they update.
		private string _Message_Cummulative = "";
		private int _ProgValue = 10;
		private bool _RunOnceOnly = true; //default value to indicate that you can only run the bw.dowork event only once
		private bool _HasRunOnce = false;
		private object _FinalResult = null;
		private bool _UserCanceled = false;
		private bool _Continuos_progress = false;

		

		//private bool _TimerOn = false;
		public FormProgressViewer()
		{
			InitializeComponent();

		}
		#region Public Methods and Properties
		/// <summary>
		/// Design your method to have DoWorkEventHandler signature
		/// void bw_DoWork(object sender, DoWorkEventArgs e)
		/// 
		/// You can cast sender as backgroundworker.
		/// The backgroundworker supports canceling and reporting progress.
		/// 
		/// Public properties expose parameters after job is finished.
		/// Set your DoWorkEventArgs e.Result to the final result of your
		/// project then when WorkerRun is complete it will become available
		/// in the _FinalResult Object.  Final Result's ToString will display in
		/// the Message area.  If you do not want this create an object that
		/// will overide the Result object's ToString() method.
		/// </summary>
		public DoWorkEventHandler SET_WORKER_METHOD
		{
			set
			{
				bw = new BackgroundWorker();
				bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
				bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
				bw.WorkerReportsProgress = true;
				bw.WorkerSupportsCancellation = true;
				bw.DoWork += value; 
				_AsyncMethod = value; // just so I can check to see if it exists
				SetButtonState();
			}
		}
		public void START_WORK()
		{
			if (_AsyncMethod == null)
			{
				MessageBox.Show(this,"No background task assigned.  Task Complete!","No Task!");				
				return;
			}

			this._UserCanceled = false;
			this._FinalResult = null;
			this._Message_Cummulative += this._Message = "Process Started\n";
			bw.RunWorkerAsync();
			_HasRunOnce = true;
			SetButtonState();
			this.timer1.Enabled = true;
			
		}
		/// <summary>
		/// Gets or Sets whether to allow the user to run the even more than
		/// one by pressing start after the DoWork event is complete.
		/// Default is true.  Set to False to allow pressing start again.
		/// </summary>
		public bool RUN_ONLY_ONCE
		{
			get { return this._RunOnceOnly; }
			set { this._RunOnceOnly = value; }
		}
		/// <summary>
		/// Reference should be okay because it is only set in the bw_RunWorkerCompleted and
		/// is only used by setting to null with the START_WORK()
		/// </summary>
		public object FINAL_RESULT { get { return this._FinalResult; } }
		public bool CANCLED_BY_USER { get { return this._UserCanceled; } }
		public string MESSAGE_LAST { get { return this._Message; } }
		public string MESSAGES_ALL { get { return this._Message_Cummulative; } }

		/// <summary>
		/// Indicates that the Progress Bar does not update its value with the 
		/// bw_ProgressChanged handler.  Instead it increments whenever the timer
		/// fires up.  Default is false.  Cannot change the value when 
		/// backgroundworker is running.
		/// </summary>
		public bool CONTINOUS_PROGRESS
		{
			get { return this._Continuos_progress; }
			set
			{
				if (bw != null)
				{
					if (!bw.IsBusy)
						this._Continuos_progress = value;
				}
				else
					this._Continuos_progress = value;
			}

		}


		#endregion

		#region Private Methods and Properties
		private void SetButtonState()
		{
			if (bw != null)
			{
				if (bw.IsBusy)
				{
					this.butStart.Visible = false;
					this.butOK.Visible = true;
					this.butOK.Enabled = false;
					this.butCancel.Visible = true;
					this.butCancel.Enabled = true;
				}
				else
				{
					if (!this._RunOnceOnly)
						this.butStart.Visible = true;
					else if (!this._HasRunOnce)
						this.butStart.Visible = true;
					else
						this.butStart.Visible = false;
					this.butOK.Visible = true;
					this.butOK.Enabled = true;
					this.butCancel.Visible = true;
					this.butCancel.Enabled = false;

				}
			}
			else
			{
				this.butStart.Visible = false;
				this.butOK.Visible = true;
				this.butOK.Enabled = true;
				this.butCancel.Visible = true;
				this.butCancel.Enabled = false;
				
			}

		}
		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled) // if you set e.cancled to true in your doWork method you cannot get any value out of Result becuase it will throw an exception. So either don't set e.cancled or you need to have your own RunWorkerCompletedEventArgs object if you want to control this.
			{
				_Message = "User Cancelled";
				_Message_Cummulative += "User Cancelled\n";
			}
			else if (e.Error != null)
			{
				_Message = "Error: " + e.Error.Message + "\n";
				_Message_Cummulative += _Message;

			}
			else if (e.Result is int)
			{
				_Message = "Result is: " + e.Result.ToString() + "\n";
				this._FinalResult = e.Result;
				//if ((int)e.Result >= 0 || (int)e.Result <= 100)
				//    _ProgValue = (int)e.Result;
			}
			else
			{
				if (e.Result != null)
				{
					_Message = "Task Finished\n"+ e.Result.ToString() + "\n";
					_Message_Cummulative += _Message;
				}
				this._FinalResult = e.Result;
			}
					//this.progressBar1.Value = (int)e.Result;

			SetButtonState();
		}

		private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (!this.timer1.Enabled)
			{
				this.timer1.Enabled = true;
				this.timer1.Start();
			}
			if (e.UserState is string)
			{
				_Message = e.UserState.ToString();
				_Message_Cummulative += e.UserState.ToString() + "\n";
			}
			if (!_Continuos_progress) 
				if (e.ProgressPercentage >= 0 || e.ProgressPercentage <= 100)
					this._ProgValue = e.ProgressPercentage; // user timer to update Form
		}

		private void butCancel_Click(object sender, EventArgs e)
		{
			if (bw != null)
				bw.CancelAsync();
			SetButtonState();
			this._UserCanceled = true;
		}

		private void butViewLog_Click(object sender, EventArgs e)
		{
			this.richTextBox1.Visible = !this.richTextBox1.Visible;
			this.richTextBox1.Text = _Message_Cummulative;
			this.butViewLog.Text = (this.richTextBox1.Visible ? "Hide Log" : "View Log");
		}

		private void butOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void butStart_Click(object sender, EventArgs e)
		{
			this.START_WORK();
			
		}
		

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.lblMessages.Text = _Message;
			
			if (this.richTextBox1.Text != _Message_Cummulative)
				this.richTextBox1.Text = _Message_Cummulative;
			if (bw != null)
				if (!bw.IsBusy)
					this.timer1.Enabled = false;

			if (_Continuos_progress)
			{
				int newValue = this.progressBar1.Value + 1;
				this.progressBar1.Value = (newValue > 100 ? 0 : newValue);
			}
			else
				this.progressBar1.Value = _ProgValue;

		}

		#endregion

		



	}
}