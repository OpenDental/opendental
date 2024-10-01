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
			this.butDeleteSchedProc = new OpenDental.UI.Button();
			this.textTimeToRun = new OpenDental.ValidTime();
			this.labelTimeToRun = new System.Windows.Forms.Label();
			this.labelFrequencyToRun = new System.Windows.Forms.Label();
			this.labelScheduledAction = new System.Windows.Forms.Label();
			this.comboFrequency = new OpenDental.UI.ComboBox();
			this.comboScheduledAction = new OpenDental.UI.ComboBox();
			this.butSave = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butDeleteSchedProc
			// 
			this.butDeleteSchedProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteSchedProc.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteSchedProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteSchedProc.Location = new System.Drawing.Point(20, 143);
			this.butDeleteSchedProc.Name = "butDeleteSchedProc";
			this.butDeleteSchedProc.Size = new System.Drawing.Size(75, 24);
			this.butDeleteSchedProc.TabIndex = 5;
			this.butDeleteSchedProc.Text = "Delete";
			this.butDeleteSchedProc.UseVisualStyleBackColor = true;
			this.butDeleteSchedProc.Click += new System.EventHandler(this.butDeleteSchedProc_Click);
			// 
			// textTimeToRun
			// 
			this.textTimeToRun.IsShortTimeString = true;
			this.textTimeToRun.Location = new System.Drawing.Point(130, 98);
			this.textTimeToRun.Name = "textTimeToRun";
			this.textTimeToRun.Size = new System.Drawing.Size(120, 20);
			this.textTimeToRun.TabIndex = 2;
			// 
			// labelTimeToRun
			// 
			this.labelTimeToRun.Location = new System.Drawing.Point(30, 98);
			this.labelTimeToRun.Name = "labelTimeToRun";
			this.labelTimeToRun.Size = new System.Drawing.Size(101, 20);
			this.labelTimeToRun.TabIndex = 6;
			this.labelTimeToRun.Text = "Time to Run";
			this.labelTimeToRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFrequencyToRun
			// 
			this.labelFrequencyToRun.Location = new System.Drawing.Point(20, 62);
			this.labelFrequencyToRun.Name = "labelFrequencyToRun";
			this.labelFrequencyToRun.Size = new System.Drawing.Size(110, 20);
			this.labelFrequencyToRun.TabIndex = 7;
			this.labelFrequencyToRun.Text = "Frequency to Run";
			this.labelFrequencyToRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelScheduledAction
			// 
			this.labelScheduledAction.Location = new System.Drawing.Point(17, 28);
			this.labelScheduledAction.Name = "labelScheduledAction";
			this.labelScheduledAction.Size = new System.Drawing.Size(113, 20);
			this.labelScheduledAction.TabIndex = 8;
			this.labelScheduledAction.Text = "Scheduled Action";
			this.labelScheduledAction.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFrequency
			// 
			this.comboFrequency.Location = new System.Drawing.Point(130, 62);
			this.comboFrequency.Name = "comboFrequency";
			this.comboFrequency.Size = new System.Drawing.Size(121, 21);
			this.comboFrequency.TabIndex = 1;
			// 
			// comboScheduledAction
			// 
			this.comboScheduledAction.Location = new System.Drawing.Point(130, 28);
			this.comboScheduledAction.Name = "comboScheduledAction";
			this.comboScheduledAction.Size = new System.Drawing.Size(121, 21);
			this.comboScheduledAction.TabIndex = 0;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(252, 143);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// FormScheduledProcessesEdit
			// 
			this.ClientSize = new System.Drawing.Size(339, 179);
			this.Controls.Add(this.butDeleteSchedProc);
			this.Controls.Add(this.textTimeToRun);
			this.Controls.Add(this.labelTimeToRun);
			this.Controls.Add(this.labelFrequencyToRun);
			this.Controls.Add(this.labelScheduledAction);
			this.Controls.Add(this.comboFrequency);
			this.Controls.Add(this.comboScheduledAction);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScheduledProcessesEdit";
			this.Text = "Scheduled Process Edit";
			this.Load += new System.EventHandler(this.FormScheduledProcessesEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.ComboBox comboScheduledAction;
		private UI.ComboBox comboFrequency;
		private System.Windows.Forms.Label labelScheduledAction;
		private System.Windows.Forms.Label labelFrequencyToRun;
		private System.Windows.Forms.Label labelTimeToRun;
		private ValidTime textTimeToRun;
		private UI.Button butDeleteSchedProc;
	}
}