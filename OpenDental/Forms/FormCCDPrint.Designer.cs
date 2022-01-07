namespace OpenDental {
	partial class FormCCDPrint {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCCDPrint));
			this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
			this.labelPage = new System.Windows.Forms.Label();
			this.butForward = new OpenDental.UI.Button();
			this.butBack = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butPrintDentist = new OpenDental.UI.Button();
			this.butOverride = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// printPreviewControl1
			// 
			this.printPreviewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.printPreviewControl1.Location = new System.Drawing.Point(0, 0);
			this.printPreviewControl1.Name = "printPreviewControl1";
			this.printPreviewControl1.Size = new System.Drawing.Size(888, 657);
			this.printPreviewControl1.TabIndex = 0;
			// 
			// labelPage
			// 
			this.labelPage.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.labelPage.AutoSize = true;
			this.labelPage.Location = new System.Drawing.Point(442, 667);
			this.labelPage.Name = "labelPage";
			this.labelPage.Size = new System.Drawing.Size(24, 13);
			this.labelPage.TabIndex = 3;
			this.labelPage.Text = "0/0";
			// 
			// butForward
			// 
			this.butForward.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butForward.Location = new System.Drawing.Point(472, 662);
			this.butForward.Name = "butForward";
			this.butForward.Size = new System.Drawing.Size(75, 23);
			this.butForward.TabIndex = 6;
			this.butForward.Text = "Forward";
			this.butForward.UseVisualStyleBackColor = true;
			this.butForward.Click += new System.EventHandler(this.butFwd_Click);
			// 
			// butBack
			// 
			this.butBack.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butBack.Location = new System.Drawing.Point(361, 662);
			this.butBack.Name = "butBack";
			this.butBack.Size = new System.Drawing.Size(75, 23);
			this.butBack.TabIndex = 5;
			this.butBack.Text = "Back";
			this.butBack.UseVisualStyleBackColor = true;
			this.butBack.Click += new System.EventHandler(this.butBack_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Location = new System.Drawing.Point(638, 663);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(116, 23);
			this.butPrint.TabIndex = 4;
			this.butPrint.Text = "Print Patient Copy";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butPrintDentist
			// 
			this.butPrintDentist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintDentist.Location = new System.Drawing.Point(760, 663);
			this.butPrintDentist.Name = "butPrintDentist";
			this.butPrintDentist.Size = new System.Drawing.Size(116, 23);
			this.butPrintDentist.TabIndex = 7;
			this.butPrintDentist.Text = "Print Dentist Copy";
			this.butPrintDentist.UseVisualStyleBackColor = true;
			this.butPrintDentist.Click += new System.EventHandler(this.butPrintDentist_Click);
			// 
			// butOverride
			// 
			this.butOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butOverride.Location = new System.Drawing.Point(2, 662);
			this.butOverride.Name = "butOverride";
			this.butOverride.Size = new System.Drawing.Size(163, 23);
			this.butOverride.TabIndex = 8;
			this.butOverride.Text = "Write Amounts To Claim";
			this.butOverride.UseVisualStyleBackColor = true;
			this.butOverride.Click += new System.EventHandler(this.butOverride_Click);
			// 
			// FormCCDPrint
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(888, 690);
			this.Controls.Add(this.butOverride);
			this.Controls.Add(this.butPrintDentist);
			this.Controls.Add(this.butForward);
			this.Controls.Add(this.butBack);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.labelPage);
			this.Controls.Add(this.printPreviewControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCCDPrint";
			this.Text = "Continuity of Care Document Print";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Resize += new System.EventHandler(this.FormCCDPrint_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PrintPreviewControl printPreviewControl1;
		private System.Windows.Forms.Label labelPage;
		private UI.Button butPrint;
		private UI.Button butBack;
		private UI.Button butForward;
		private UI.Button butPrintDentist;
		private UI.Button butOverride;
	}
}

