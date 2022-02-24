namespace OpenDental {
	partial class FormEhrSummaryCcdEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrSummaryCcdEdit));
			this.butClose = new System.Windows.Forms.Button();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.butPrint = new System.Windows.Forms.Button();
			this.butReconcileMedications = new System.Windows.Forms.Button();
			this.labelReconcile = new System.Windows.Forms.Label();
			this.butReconcileProblems = new System.Windows.Forms.Button();
			this.butReconcileAllergies = new System.Windows.Forms.Button();
			this.butShowXml = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(804, 610);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// webBrowser1
			// 
			this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser1.Location = new System.Drawing.Point(1, 1);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(882, 603);
			this.webBrowser1.TabIndex = 1;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Location = new System.Drawing.Point(628, 610);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 23);
			this.butPrint.TabIndex = 2;
			this.butPrint.Text = "Print";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butReconcileMedications
			// 
			this.butReconcileMedications.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReconcileMedications.Location = new System.Drawing.Point(104, 610);
			this.butReconcileMedications.Name = "butReconcileMedications";
			this.butReconcileMedications.Size = new System.Drawing.Size(75, 23);
			this.butReconcileMedications.TabIndex = 3;
			this.butReconcileMedications.Text = "Medications";
			this.butReconcileMedications.UseVisualStyleBackColor = true;
			this.butReconcileMedications.Click += new System.EventHandler(this.butReconcileMedications_Click);
			// 
			// labelReconcile
			// 
			this.labelReconcile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelReconcile.Location = new System.Drawing.Point(1, 613);
			this.labelReconcile.Name = "labelReconcile";
			this.labelReconcile.Size = new System.Drawing.Size(97, 16);
			this.labelReconcile.TabIndex = 4;
			this.labelReconcile.Text = "Reconcile:";
			this.labelReconcile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelReconcile.Visible = false;
			// 
			// butReconcileProblems
			// 
			this.butReconcileProblems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReconcileProblems.Location = new System.Drawing.Point(185, 610);
			this.butReconcileProblems.Name = "butReconcileProblems";
			this.butReconcileProblems.Size = new System.Drawing.Size(75, 23);
			this.butReconcileProblems.TabIndex = 5;
			this.butReconcileProblems.Text = "Problems";
			this.butReconcileProblems.UseVisualStyleBackColor = true;
			this.butReconcileProblems.Click += new System.EventHandler(this.butReconcileProblems_Click);
			// 
			// butReconcileAllergies
			// 
			this.butReconcileAllergies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReconcileAllergies.Location = new System.Drawing.Point(266, 610);
			this.butReconcileAllergies.Name = "butReconcileAllergies";
			this.butReconcileAllergies.Size = new System.Drawing.Size(75, 23);
			this.butReconcileAllergies.TabIndex = 6;
			this.butReconcileAllergies.Text = "Allergies";
			this.butReconcileAllergies.UseVisualStyleBackColor = true;
			this.butReconcileAllergies.Click += new System.EventHandler(this.butReconcileAllergies_Click);
			// 
			// butShowXml
			// 
			this.butShowXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butShowXml.Location = new System.Drawing.Point(453, 610);
			this.butShowXml.Name = "butShowXml";
			this.butShowXml.Size = new System.Drawing.Size(75, 23);
			this.butShowXml.TabIndex = 7;
			this.butShowXml.Text = "Show xml";
			this.butShowXml.UseVisualStyleBackColor = true;
			this.butShowXml.Click += new System.EventHandler(this.butShowXml_Click);
			// 
			// FormEhrSummaryCcdEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 639);
			this.Controls.Add(this.butShowXml);
			this.Controls.Add(this.butReconcileAllergies);
			this.Controls.Add(this.butReconcileProblems);
			this.Controls.Add(this.labelReconcile);
			this.Controls.Add(this.butReconcileMedications);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.webBrowser1);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrSummaryCcdEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Summary of Care";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormEhrSummaryCcdEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private System.Windows.Forms.Button butPrint;
		private System.Windows.Forms.Button butReconcileMedications;
		private System.Windows.Forms.Label labelReconcile;
		private System.Windows.Forms.Button butReconcileProblems;
		private System.Windows.Forms.Button butReconcileAllergies;
		private System.Windows.Forms.Button butShowXml;
	}
}