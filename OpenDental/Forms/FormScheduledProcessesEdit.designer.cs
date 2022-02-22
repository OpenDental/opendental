namespace OpenDental{
	partial class FormScheduledProcessesEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduledProcessesEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butDeleteSchedProc = new OpenDental.UI.Button();
			this.textTimeToRun = new OpenDental.ValidTime();
			this.labelTimeToRun = new System.Windows.Forms.Label();
			this.labelFrequencyToRun = new System.Windows.Forms.Label();
			this.labelScheduledAction = new System.Windows.Forms.Label();
			this.comboFrequency = new OpenDental.UI.ComboBoxOD();
			this.comboScheduledAction = new OpenDental.UI.ComboBoxOD();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(268, 111);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDeleteSchedProc
			// 
			this.butDeleteSchedProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteSchedProc.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteSchedProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteSchedProc.Location = new System.Drawing.Point(43, 112);
			this.butDeleteSchedProc.Name = "butDeleteSchedProc";
			this.butDeleteSchedProc.Size = new System.Drawing.Size(75, 23);
			this.butDeleteSchedProc.TabIndex = 5;
			this.butDeleteSchedProc.Text = "Delete";
			this.butDeleteSchedProc.UseVisualStyleBackColor = true;
			this.butDeleteSchedProc.Click += new System.EventHandler(this.butDeleteSchedProc_Click);
			// 
			// textTimeToRun
			// 
			this.textTimeToRun.IsShortTimeString = true;
			this.textTimeToRun.Location = new System.Drawing.Point(118, 76);
			this.textTimeToRun.Name = "textTimeToRun";
			this.textTimeToRun.Size = new System.Drawing.Size(120, 20);
			this.textTimeToRun.TabIndex = 2;
			// 
			// labelTimeToRun
			// 
			this.labelTimeToRun.Location = new System.Drawing.Point(18, 76);
			this.labelTimeToRun.Name = "labelTimeToRun";
			this.labelTimeToRun.Size = new System.Drawing.Size(101, 20);
			this.labelTimeToRun.TabIndex = 6;
			this.labelTimeToRun.Text = "Time to Run";
			this.labelTimeToRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFrequencyToRun
			// 
			this.labelFrequencyToRun.Location = new System.Drawing.Point(8, 40);
			this.labelFrequencyToRun.Name = "labelFrequencyToRun";
			this.labelFrequencyToRun.Size = new System.Drawing.Size(110, 20);
			this.labelFrequencyToRun.TabIndex = 7;
			this.labelFrequencyToRun.Text = "Frequency to Run";
			this.labelFrequencyToRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelScheduledAction
			// 
			this.labelScheduledAction.Location = new System.Drawing.Point(5, 6);
			this.labelScheduledAction.Name = "labelScheduledAction";
			this.labelScheduledAction.Size = new System.Drawing.Size(113, 20);
			this.labelScheduledAction.TabIndex = 8;
			this.labelScheduledAction.Text = "Scheduled Action";
			this.labelScheduledAction.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFrequency
			// 
			this.comboFrequency.Location = new System.Drawing.Point(118, 40);
			this.comboFrequency.Name = "comboFrequency";
			this.comboFrequency.Size = new System.Drawing.Size(121, 21);
			this.comboFrequency.TabIndex = 1;
			// 
			// comboScheduledAction
			// 
			this.comboScheduledAction.Location = new System.Drawing.Point(118, 6);
			this.comboScheduledAction.Name = "comboScheduledAction";
			this.comboScheduledAction.Size = new System.Drawing.Size(121, 21);
			this.comboScheduledAction.TabIndex = 0;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(268, 81);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormScheduledProcessesEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(355, 147);
			this.Controls.Add(this.butDeleteSchedProc);
			this.Controls.Add(this.textTimeToRun);
			this.Controls.Add(this.labelTimeToRun);
			this.Controls.Add(this.labelFrequencyToRun);
			this.Controls.Add(this.labelScheduledAction);
			this.Controls.Add(this.comboFrequency);
			this.Controls.Add(this.comboScheduledAction);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScheduledProcessesEdit";
			this.Text = "Scheduled Proccesses Edit";
			this.Load += new System.EventHandler(this.FormScheduledProcessesEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.ComboBoxOD comboScheduledAction;
		private UI.ComboBoxOD comboFrequency;
		private System.Windows.Forms.Label labelScheduledAction;
		private System.Windows.Forms.Label labelFrequencyToRun;
		private System.Windows.Forms.Label labelTimeToRun;
		private ValidTime textTimeToRun;
		private UI.Button butDeleteSchedProc;
	}
}