namespace OpenDental{
	partial class FormRpArizonaPrimaryCareEligibility {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources=new System.ComponentModel.ComponentResourceManager(typeof(FormRpArizonaPrimaryCareEligibility));
			this.textEligibilityFolder=new System.Windows.Forms.TextBox();
			this.label1=new System.Windows.Forms.Label();
			this.textLog=new System.Windows.Forms.TextBox();
			this.label2=new System.Windows.Forms.Label();
			this.folderEligibilityPath=new OpenDental.FolderBrowserDialog();
			this.label3=new System.Windows.Forms.Label();
			this.textEligibilityFile=new System.Windows.Forms.TextBox();
			this.dateTimeFrom=new System.Windows.Forms.DateTimePicker();
			this.groupBox1=new System.Windows.Forms.GroupBox();
			this.label5=new System.Windows.Forms.Label();
			this.dateTimeTo=new System.Windows.Forms.DateTimePicker();
			this.label4=new System.Windows.Forms.Label();
			this.butRun=new OpenDental.UI.Button();
			this.butCopy=new OpenDental.UI.Button();
			this.butBrowse=new OpenDental.UI.Button();
			this.butFinished=new OpenDental.UI.Button();
			this.butCancel=new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textEligibilityFolder
			// 
			this.textEligibilityFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEligibilityFolder.Location=new System.Drawing.Point(12,31);
			this.textEligibilityFolder.Name="textEligibilityFolder";
			this.textEligibilityFolder.Size=new System.Drawing.Size(607,20);
			this.textEligibilityFolder.TabIndex=4;
			this.textEligibilityFolder.Text="C:\\Temp";
			// 
			// label1
			// 
			this.label1.AutoSize=true;
			this.label1.Location=new System.Drawing.Point(12,12);
			this.label1.Name="label1";
			this.label1.Size=new System.Drawing.Size(77,13);
			this.label1.TabIndex=6;
			this.label1.Text="Save report to:";
			// 
			// textLog
			// 
			this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textLog.Location=new System.Drawing.Point(12,194);
			this.textLog.Multiline=true;
			this.textLog.Name="textLog";
			this.textLog.ReadOnly=true;
			this.textLog.ScrollBars=System.Windows.Forms.ScrollBars.Vertical;
			this.textLog.Size=new System.Drawing.Size(607,284);
			this.textLog.TabIndex=8;
			// 
			// label2
			// 
			this.label2.AutoSize=true;
			this.label2.Location=new System.Drawing.Point(13,176);
			this.label2.Name="label2";
			this.label2.Size=new System.Drawing.Size(60,13);
			this.label2.TabIndex=9;
			this.label2.Text="Report Log";
			// 
			// label3
			// 
			this.label3.AutoSize=true;
			this.label3.Location=new System.Drawing.Point(13,57);
			this.label3.Name="label3";
			this.label3.Size=new System.Drawing.Size(89,13);
			this.label3.TabIndex=10;
			this.label3.Text="Output File Name";
			// 
			// textEligibilityFile
			// 
			this.textEligibilityFile.Location=new System.Drawing.Point(12,74);
			this.textEligibilityFile.Name="textEligibilityFile";
			this.textEligibilityFile.ReadOnly=true;
			this.textEligibilityFile.Size=new System.Drawing.Size(164,20);
			this.textEligibilityFile.TabIndex=11;
			this.textEligibilityFile.Text="ApcEligibility.txt";
			// 
			// dateTimeFrom
			// 
			this.dateTimeFrom.Location=new System.Drawing.Point(6,42);
			this.dateTimeFrom.Name="dateTimeFrom";
			this.dateTimeFrom.Size=new System.Drawing.Size(200,20);
			this.dateTimeFrom.TabIndex=14;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.dateTimeTo);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.dateTimeFrom);
			this.groupBox1.Location=new System.Drawing.Point(12,100);
			this.groupBox1.Name="groupBox1";
			this.groupBox1.Size=new System.Drawing.Size(603,71);
			this.groupBox1.TabIndex=15;
			this.groupBox1.TabStop=false;
			this.groupBox1.Text="Date of Last Completed Appointment";
			// 
			// label5
			// 
			this.label5.AutoSize=true;
			this.label5.Location=new System.Drawing.Point(262,22);
			this.label5.Name="label5";
			this.label5.Size=new System.Drawing.Size(20,13);
			this.label5.TabIndex=17;
			this.label5.Text="To";
			// 
			// dateTimeTo
			// 
			this.dateTimeTo.Location=new System.Drawing.Point(262,41);
			this.dateTimeTo.Name="dateTimeTo";
			this.dateTimeTo.Size=new System.Drawing.Size(200,20);
			this.dateTimeTo.TabIndex=16;
			// 
			// label4
			// 
			this.label4.AutoSize=true;
			this.label4.Location=new System.Drawing.Point(7,23);
			this.label4.Name="label4";
			this.label4.Size=new System.Drawing.Size(30,13);
			this.label4.TabIndex=15;
			this.label4.Text="From";
			// 
			// butRun
			// 
			this.butRun.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.Location=new System.Drawing.Point(625,369);
			this.butRun.Name="butRun";
			this.butRun.Size=new System.Drawing.Size(75,24);
			this.butRun.TabIndex=13;
			this.butRun.Text="Run";
			this.butRun.Click+=new System.EventHandler(this.butRun_Click);
			// 
			// butCopy
			// 
			this.butCopy.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy.Location=new System.Drawing.Point(625,323);
			this.butCopy.Name="butCopy";
			this.butCopy.Size=new System.Drawing.Size(83,24);
			this.butCopy.TabIndex=12;
			this.butCopy.Text="Copy Log Text";
			this.butCopy.Click+=new System.EventHandler(this.butCopy_Click);
			// 
			// butBrowse
			// 
			this.butBrowse.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Right)));
			this.butBrowse.Location=new System.Drawing.Point(625,28);
			this.butBrowse.Name="butBrowse";
			this.butBrowse.Size=new System.Drawing.Size(75,24);
			this.butBrowse.TabIndex=7;
			this.butBrowse.Text="Browse";
			this.butBrowse.Click+=new System.EventHandler(this.butBrowse_Click);
			// 
			// butFinished
			// 
			this.butFinished.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Right)));
			this.butFinished.Location=new System.Drawing.Point(625,413);
			this.butFinished.Name="butFinished";
			this.butFinished.Size=new System.Drawing.Size(75,24);
			this.butFinished.TabIndex=3;
			this.butFinished.Text="&Finished";
			this.butFinished.Click+=new System.EventHandler(this.butFinished_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location=new System.Drawing.Point(625,454);
			this.butCancel.Name="butCancel";
			this.butCancel.Size=new System.Drawing.Size(75,24);
			this.butCancel.TabIndex=2;
			this.butCancel.Text="&Cancel";
			this.butCancel.Click+=new System.EventHandler(this.butCancel_Click);
			// 
			// FormRpArizonaPrimaryCareEligibility
			// 
			this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize=new System.Drawing.Size(725,505);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.textEligibilityFile);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textLog);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textEligibilityFolder);
			this.Controls.Add(this.butFinished);
			this.Icon=((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name="FormRpArizonaPrimaryCareEligibility";
			this.StartPosition=System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text="Arizona Primary Care Eligibility File Report";
			this.Load+=new System.EventHandler(this.FormRpArizonaPrimaryCareEligibility_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butFinished;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textEligibilityFolder;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butBrowse;
		private System.Windows.Forms.TextBox textLog;
		private System.Windows.Forms.Label label2;
		private OpenDental.FolderBrowserDialog folderEligibilityPath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textEligibilityFile;
		private OpenDental.UI.Button butCopy;
		private OpenDental.UI.Button butRun;
		private System.Windows.Forms.DateTimePicker dateTimeFrom;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DateTimePicker dateTimeTo;
		private System.Windows.Forms.Label label4;
	}
}