namespace OpenDental {
	partial class FormNewCropBilling {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewCropBilling));
			this.gridBillingList = new OpenDental.UI.GridOD();
			this.textBillingFilePath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.butLoad = new OpenDental.UI.Button();
			this.butBrowse = new OpenDental.UI.Button();
			this.butProcess = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBillingYearMonth = new System.Windows.Forms.TextBox();
			this.labelDuplicateProviders = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridBillingList
			// 
			this.gridBillingList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridBillingList.Location = new System.Drawing.Point(12, 100);
			this.gridBillingList.Name = "gridBillingList";
			this.gridBillingList.Size = new System.Drawing.Size(1010, 563);
			this.gridBillingList.TabIndex = 0;
			this.gridBillingList.Title = "Providers Using NewCrop";
			this.gridBillingList.TranslationName = "TableProviders";
			// 
			// textBillingFilePath
			// 
			this.textBillingFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBillingFilePath.Location = new System.Drawing.Point(12, 32);
			this.textBillingFilePath.Name = "textBillingFilePath";
			this.textBillingFilePath.Size = new System.Drawing.Size(972, 20);
			this.textBillingFilePath.TabIndex = 10;
			this.textBillingFilePath.TextChanged += new System.EventHandler(this.textBillingFilePath_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(829, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Billing file path. Must be downloaded from NewCrop customer portal. See Wiki for " +
    "instructions. Use the Load button to populate the grid after a file is selected." +
    "";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "Billing.xml";
			// 
			// butLoad
			// 
			this.butLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butLoad.Location = new System.Drawing.Point(947, 71);
			this.butLoad.Name = "butLoad";
			this.butLoad.Size = new System.Drawing.Size(75, 23);
			this.butLoad.TabIndex = 12;
			this.butLoad.Text = "Load";
			this.butLoad.UseVisualStyleBackColor = true;
			this.butLoad.Click += new System.EventHandler(this.butLoad_Click);
			// 
			// butBrowse
			// 
			this.butBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBrowse.Location = new System.Drawing.Point(990, 30);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(32, 23);
			this.butBrowse.TabIndex = 11;
			this.butBrowse.Text = "...";
			this.butBrowse.UseVisualStyleBackColor = true;
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// butProcess
			// 
			this.butProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butProcess.Location = new System.Drawing.Point(866, 669);
			this.butProcess.Name = "butProcess";
			this.butProcess.Size = new System.Drawing.Size(75, 23);
			this.butProcess.TabIndex = 3;
			this.butProcess.Text = "Process";
			this.butProcess.UseVisualStyleBackColor = true;
			this.butProcess.Click += new System.EventHandler(this.butProcess_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(947, 669);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(227, 16);
			this.label2.TabIndex = 14;
			this.label2.Text = "Billing year and month (YYYYMM)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBillingYearMonth
			// 
			this.textBillingYearMonth.Location = new System.Drawing.Point(12, 74);
			this.textBillingYearMonth.Name = "textBillingYearMonth";
			this.textBillingYearMonth.Size = new System.Drawing.Size(85, 20);
			this.textBillingYearMonth.TabIndex = 15;
			// 
			// labelDuplicateProviders
			// 
			this.labelDuplicateProviders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDuplicateProviders.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDuplicateProviders.ForeColor = System.Drawing.Color.Red;
			this.labelDuplicateProviders.Location = new System.Drawing.Point(12, 672);
			this.labelDuplicateProviders.Name = "labelDuplicateProviders";
			this.labelDuplicateProviders.Size = new System.Drawing.Size(848, 16);
			this.labelDuplicateProviders.TabIndex = 16;
			this.labelDuplicateProviders.Text = "Duplicate providers are displayed in bold red text.  Duplicate providers must be " +
    "removed from NewCrop manually.";
			this.labelDuplicateProviders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelDuplicateProviders.Visible = false;
			// 
			// FormNewCropBilling
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1034, 707);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butProcess);
			this.Controls.Add(this.labelDuplicateProviders);
			this.Controls.Add(this.textBillingYearMonth);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butLoad);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.textBillingFilePath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridBillingList);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormNewCropBilling";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NewCrop Billing";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Resize += new System.EventHandler(this.FormBillingList_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOD gridBillingList;
		private UI.Button butClose;
		private UI.Button butProcess;
		private UI.Button butBrowse;
		private System.Windows.Forms.TextBox textBillingFilePath;
		private System.Windows.Forms.Label label1;
		private UI.Button butLoad;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBillingYearMonth;
		private System.Windows.Forms.Label labelDuplicateProviders;
	}
}