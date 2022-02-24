namespace OpenDental{
	partial class FormMapHQPrefs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapHQPrefs));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelRedCalls = new System.Windows.Forms.Label();
			this.labelRedTime = new System.Windows.Forms.Label();
			this.groupRedTriage = new System.Windows.Forms.GroupBox();
			this.textRedTime = new OpenDental.ValidNum();
			this.textRedCalls = new OpenDental.ValidNum();
			this.groupVoicemail = new System.Windows.Forms.GroupBox();
			this.textVMTime = new OpenDental.ValidNum();
			this.textVMCalls = new OpenDental.ValidNum();
			this.labelVMTime = new System.Windows.Forms.Label();
			this.labelVMCalls = new System.Windows.Forms.Label();
			this.groupTriage = new System.Windows.Forms.GroupBox();
			this.textTime = new OpenDental.ValidNum();
			this.textTimeWarning = new OpenDental.ValidNum();
			this.textCalls = new OpenDental.ValidNum();
			this.textCallsWarning = new OpenDental.ValidNum();
			this.labelTime = new System.Windows.Forms.Label();
			this.labelCalls = new System.Windows.Forms.Label();
			this.labelTimeWarning = new System.Windows.Forms.Label();
			this.labelCallsWarning = new System.Windows.Forms.Label();
			this.labelIntro = new System.Windows.Forms.Label();
			this.groupRedTriage.SuspendLayout();
			this.groupVoicemail.SuspendLayout();
			this.groupTriage.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(273, 266);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(354, 266);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelRedCalls
			// 
			this.labelRedCalls.Location = new System.Drawing.Point(6, 20);
			this.labelRedCalls.Name = "labelRedCalls";
			this.labelRedCalls.Size = new System.Drawing.Size(143, 21);
			this.labelRedCalls.TabIndex = 8;
			this.labelRedCalls.Text = "# Calls Red At:";
			this.labelRedCalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRedTime
			// 
			this.labelRedTime.Location = new System.Drawing.Point(206, 20);
			this.labelRedTime.Name = "labelRedTime";
			this.labelRedTime.Size = new System.Drawing.Size(143, 21);
			this.labelRedTime.TabIndex = 9;
			this.labelRedTime.Text = "Minutes Red At:";
			this.labelRedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupRedTriage
			// 
			this.groupRedTriage.Controls.Add(this.textRedTime);
			this.groupRedTriage.Controls.Add(this.textRedCalls);
			this.groupRedTriage.Controls.Add(this.labelRedTime);
			this.groupRedTriage.Controls.Add(this.labelRedCalls);
			this.groupRedTriage.Location = new System.Drawing.Point(13, 33);
			this.groupRedTriage.Name = "groupRedTriage";
			this.groupRedTriage.Size = new System.Drawing.Size(415, 56);
			this.groupRedTriage.TabIndex = 23;
			this.groupRedTriage.TabStop = false;
			this.groupRedTriage.Text = "Red Tasks";
			// 
			// textRedTime
			// 
			this.textRedTime.Location = new System.Drawing.Point(350, 20);
			this.textRedTime.MaxVal = 255;
			this.textRedTime.MinVal = 0;
			this.textRedTime.Name = "textRedTime";
			this.textRedTime.Size = new System.Drawing.Size(50, 20);
			this.textRedTime.TabIndex = 2;
			// 
			// textRedCalls
			// 
			this.textRedCalls.Location = new System.Drawing.Point(150, 20);
			this.textRedCalls.MaxVal = 255;
			this.textRedCalls.MinVal = 0;
			this.textRedCalls.Name = "textRedCalls";
			this.textRedCalls.Size = new System.Drawing.Size(50, 20);
			this.textRedCalls.TabIndex = 1;
			// 
			// groupVoicemail
			// 
			this.groupVoicemail.Controls.Add(this.textVMTime);
			this.groupVoicemail.Controls.Add(this.textVMCalls);
			this.groupVoicemail.Controls.Add(this.labelVMTime);
			this.groupVoicemail.Controls.Add(this.labelVMCalls);
			this.groupVoicemail.Location = new System.Drawing.Point(13, 95);
			this.groupVoicemail.Name = "groupVoicemail";
			this.groupVoicemail.Size = new System.Drawing.Size(415, 56);
			this.groupVoicemail.TabIndex = 24;
			this.groupVoicemail.TabStop = false;
			this.groupVoicemail.Text = "Voicemail";
			// 
			// textVMTime
			// 
			this.textVMTime.Location = new System.Drawing.Point(350, 20);
			this.textVMTime.MaxVal = 255;
			this.textVMTime.MinVal = 0;
			this.textVMTime.Name = "textVMTime";
			this.textVMTime.Size = new System.Drawing.Size(50, 20);
			this.textVMTime.TabIndex = 4;
			// 
			// textVMCalls
			// 
			this.textVMCalls.Location = new System.Drawing.Point(150, 20);
			this.textVMCalls.MaxVal = 255;
			this.textVMCalls.MinVal = 0;
			this.textVMCalls.Name = "textVMCalls";
			this.textVMCalls.Size = new System.Drawing.Size(50, 20);
			this.textVMCalls.TabIndex = 3;
			// 
			// labelVMTime
			// 
			this.labelVMTime.Location = new System.Drawing.Point(206, 20);
			this.labelVMTime.Name = "labelVMTime";
			this.labelVMTime.Size = new System.Drawing.Size(143, 21);
			this.labelVMTime.TabIndex = 11;
			this.labelVMTime.Text = "Minutes Red At:";
			this.labelVMTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVMCalls
			// 
			this.labelVMCalls.Location = new System.Drawing.Point(6, 20);
			this.labelVMCalls.Name = "labelVMCalls";
			this.labelVMCalls.Size = new System.Drawing.Size(143, 21);
			this.labelVMCalls.TabIndex = 11;
			this.labelVMCalls.Text = "# Calls Red At:";
			this.labelVMCalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupTriage
			// 
			this.groupTriage.Controls.Add(this.textTime);
			this.groupTriage.Controls.Add(this.textTimeWarning);
			this.groupTriage.Controls.Add(this.textCalls);
			this.groupTriage.Controls.Add(this.textCallsWarning);
			this.groupTriage.Controls.Add(this.labelTime);
			this.groupTriage.Controls.Add(this.labelCalls);
			this.groupTriage.Controls.Add(this.labelTimeWarning);
			this.groupTriage.Controls.Add(this.labelCallsWarning);
			this.groupTriage.Location = new System.Drawing.Point(13, 157);
			this.groupTriage.Name = "groupTriage";
			this.groupTriage.Size = new System.Drawing.Size(415, 94);
			this.groupTriage.TabIndex = 25;
			this.groupTriage.TabStop = false;
			this.groupTriage.Text = "Triage Tasks";
			// 
			// textTime
			// 
			this.textTime.Location = new System.Drawing.Point(350, 58);
			this.textTime.MaxVal = 255;
			this.textTime.MinVal = 0;
			this.textTime.Name = "textTime";
			this.textTime.Size = new System.Drawing.Size(50, 20);
			this.textTime.TabIndex = 8;
			// 
			// textTimeWarning
			// 
			this.textTimeWarning.Location = new System.Drawing.Point(350, 20);
			this.textTimeWarning.MaxVal = 255;
			this.textTimeWarning.MinVal = 0;
			this.textTimeWarning.Name = "textTimeWarning";
			this.textTimeWarning.Size = new System.Drawing.Size(50, 20);
			this.textTimeWarning.TabIndex = 6;
			// 
			// textCalls
			// 
			this.textCalls.Location = new System.Drawing.Point(150, 58);
			this.textCalls.MaxVal = 255;
			this.textCalls.MinVal = 0;
			this.textCalls.Name = "textCalls";
			this.textCalls.Size = new System.Drawing.Size(50, 20);
			this.textCalls.TabIndex = 7;
			this.textCalls.ShowZero = false;
			// 
			// textCallsWarning
			// 
			this.textCallsWarning.Location = new System.Drawing.Point(150, 20);
			this.textCallsWarning.MaxVal = 255;
			this.textCallsWarning.MinVal = 0;
			this.textCallsWarning.Name = "textCallsWarning";
			this.textCallsWarning.Size = new System.Drawing.Size(50, 20);
			this.textCallsWarning.TabIndex = 5;
			// 
			// labelTime
			// 
			this.labelTime.Location = new System.Drawing.Point(206, 58);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(143, 21);
			this.labelTime.TabIndex = 15;
			this.labelTime.Text = "Minutes Red At:";
			this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCalls
			// 
			this.labelCalls.Location = new System.Drawing.Point(6, 58);
			this.labelCalls.Name = "labelCalls";
			this.labelCalls.Size = new System.Drawing.Size(143, 21);
			this.labelCalls.TabIndex = 13;
			this.labelCalls.Text = "# Calls Red At:";
			this.labelCalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTimeWarning
			// 
			this.labelTimeWarning.Location = new System.Drawing.Point(206, 20);
			this.labelTimeWarning.Name = "labelTimeWarning";
			this.labelTimeWarning.Size = new System.Drawing.Size(143, 21);
			this.labelTimeWarning.TabIndex = 11;
			this.labelTimeWarning.Text = "Minutes Yellow At:";
			this.labelTimeWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCallsWarning
			// 
			this.labelCallsWarning.Location = new System.Drawing.Point(6, 20);
			this.labelCallsWarning.Name = "labelCallsWarning";
			this.labelCallsWarning.Size = new System.Drawing.Size(143, 21);
			this.labelCallsWarning.TabIndex = 11;
			this.labelCallsWarning.Text = "# Calls Yellow At:";
			this.labelCallsWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelIntro
			// 
			this.labelIntro.AutoSize = true;
			this.labelIntro.Location = new System.Drawing.Point(12, 9);
			this.labelIntro.Name = "labelIntro";
			this.labelIntro.Size = new System.Drawing.Size(114, 13);
			this.labelIntro.TabIndex = 26;
			this.labelIntro.Text = "Enter Whole Numbers:";
			this.labelIntro.Click += new System.EventHandler(this.labelIntro_Click);
			// 
			// FormMapHQPrefs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(441, 302);
			this.Controls.Add(this.labelIntro);
			this.Controls.Add(this.groupTriage);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupRedTriage);
			this.Controls.Add(this.groupVoicemail);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMapHQPrefs";
			this.Text = "Call Center Thresholds";
			this.Load += new System.EventHandler(this.FormFillTriagePref_Load);
			this.groupRedTriage.ResumeLayout(false);
			this.groupRedTriage.PerformLayout();
			this.groupVoicemail.ResumeLayout(false);
			this.groupVoicemail.PerformLayout();
			this.groupTriage.ResumeLayout(false);
			this.groupTriage.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelRedCalls;
		private System.Windows.Forms.Label labelRedTime;
		private System.Windows.Forms.GroupBox groupRedTriage;
		private System.Windows.Forms.GroupBox groupVoicemail;
		private System.Windows.Forms.Label labelVMTime;
		private System.Windows.Forms.Label labelVMCalls;
		private System.Windows.Forms.GroupBox groupTriage;
		private System.Windows.Forms.Label labelTimeWarning;
		private System.Windows.Forms.Label labelCallsWarning;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Label labelCalls;
		private ValidNum textRedCalls;
		private ValidNum textVMCalls;
		private ValidNum textCalls;
		private ValidNum textCallsWarning;
		private ValidNum textRedTime;
		private ValidNum textVMTime;
		private ValidNum textTime;
		private ValidNum textTimeWarning;
		private System.Windows.Forms.Label labelIntro;
	}
}