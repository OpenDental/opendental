namespace OpenDental {
	partial class EServiceMetricsControl {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.panelAlertColor = new System.Windows.Forms.Panel();
			this.labelAccountBalance = new System.Windows.Forms.Label();
			this.timerFlash = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 23F);
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 38);
			this.label1.TabIndex = 0;
			this.label1.Text = "eServ:";
			// 
			// panelAlertColor
			// 
			this.panelAlertColor.Location = new System.Drawing.Point(92, 6);
			this.panelAlertColor.Name = "panelAlertColor";
			this.panelAlertColor.Size = new System.Drawing.Size(45, 45);
			this.panelAlertColor.TabIndex = 1;
			this.panelAlertColor.Paint += new System.Windows.Forms.PaintEventHandler(this.panelAlertColor_Paint);
			// 
			// labelAccountBalance
			// 
			this.labelAccountBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAccountBalance.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelAccountBalance.Font = new System.Drawing.Font("Calibri", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAccountBalance.Location = new System.Drawing.Point(143, 4);
			this.labelAccountBalance.Name = "labelAccountBalance";
			this.labelAccountBalance.Size = new System.Drawing.Size(145, 48);
			this.labelAccountBalance.TabIndex = 3;
			this.labelAccountBalance.Text = "%5555";
			this.labelAccountBalance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelAccountBalance.DoubleClick += new System.EventHandler(this.labelAccountBalance_DoubleClick);
			// 
			// timerFlash
			// 
			this.timerFlash.Interval = 300;
			this.timerFlash.Tick += new System.EventHandler(this.timerFlash_Tick);
			// 
			// EServiceMetricsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.labelAccountBalance);
			this.Controls.Add(this.panelAlertColor);
			this.Controls.Add(this.label1);
			this.Name = "EServiceMetricsControl";
			this.Size = new System.Drawing.Size(291, 56);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panelAlertColor;
		private System.Windows.Forms.Label labelAccountBalance;
		private System.Windows.Forms.Timer timerFlash;
	}
}
