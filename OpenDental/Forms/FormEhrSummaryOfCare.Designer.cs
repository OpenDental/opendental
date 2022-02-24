namespace OpenDental {
	partial class FormEhrSummaryOfCare {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrSummaryOfCare));
			this.butClose = new System.Windows.Forms.Button();
			this.butRecEmail = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.butShowXhtml = new System.Windows.Forms.Button();
			this.butShowXml = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gridRec = new OpenDental.UI.GridOD();
			this.gridSent = new OpenDental.UI.GridOD();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butRecFile = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butExport = new System.Windows.Forms.Button();
			this.butSendEmail = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(343, 401);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butRecEmail
			// 
			this.butRecEmail.Location = new System.Drawing.Point(103, 19);
			this.butRecEmail.Name = "butRecEmail";
			this.butRecEmail.Size = new System.Drawing.Size(82, 23);
			this.butRecEmail.TabIndex = 10;
			this.butRecEmail.Text = "Email";
			this.butRecEmail.UseVisualStyleBackColor = true;
			this.butRecEmail.Click += new System.EventHandler(this.butRecEmail_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(27, 401);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butShowXhtml
			// 
			this.butShowXhtml.Location = new System.Drawing.Point(8, 19);
			this.butShowXhtml.Name = "butShowXhtml";
			this.butShowXhtml.Size = new System.Drawing.Size(84, 23);
			this.butShowXhtml.TabIndex = 18;
			this.butShowXhtml.Text = "Show xhtml";
			this.butShowXhtml.UseVisualStyleBackColor = true;
			this.butShowXhtml.Click += new System.EventHandler(this.butShowXhtml_Click);
			// 
			// butShowXml
			// 
			this.butShowXml.Location = new System.Drawing.Point(109, 19);
			this.butShowXml.Name = "butShowXml";
			this.butShowXml.Size = new System.Drawing.Size(84, 23);
			this.butShowXml.TabIndex = 19;
			this.butShowXml.Text = "Show xml";
			this.butShowXml.UseVisualStyleBackColor = true;
			this.butShowXml.Click += new System.EventHandler(this.butShowXml_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butShowXhtml);
			this.groupBox1.Controls.Add(this.butShowXml);
			this.groupBox1.Location = new System.Drawing.Point(20, 100);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(201, 50);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show";
			// 
			// gridRec
			// 
			this.gridRec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridRec.Location = new System.Drawing.Point(240, 65);
			this.gridRec.Name = "gridRec";
			this.gridRec.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridRec.Size = new System.Drawing.Size(178, 323);
			this.gridRec.TabIndex = 17;
			this.gridRec.Title = "Received";
			this.gridRec.TranslationName = "TableReceived";
			this.gridRec.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRec_CellDoubleClick);
			// 
			// gridSent
			// 
			this.gridSent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridSent.Location = new System.Drawing.Point(28, 154);
			this.gridSent.Name = "gridSent";
			this.gridSent.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridSent.Size = new System.Drawing.Size(185, 234);
			this.gridSent.TabIndex = 16;
			this.gridSent.Title = "Sent";
			this.gridSent.TranslationName = "TableSent";
			this.gridSent.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSent_CellDoubleClick);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butRecFile);
			this.groupBox2.Controls.Add(this.butRecEmail);
			this.groupBox2.Location = new System.Drawing.Point(233, 7);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(192, 52);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Receive by";
			// 
			// butRecFile
			// 
			this.butRecFile.Location = new System.Drawing.Point(7, 19);
			this.butRecFile.Name = "butRecFile";
			this.butRecFile.Size = new System.Drawing.Size(82, 23);
			this.butRecFile.TabIndex = 11;
			this.butRecFile.Text = "File Import";
			this.butRecFile.UseVisualStyleBackColor = true;
			this.butRecFile.Click += new System.EventHandler(this.butRecFile_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.butExport);
			this.groupBox3.Controls.Add(this.butSendEmail);
			this.groupBox3.Location = new System.Drawing.Point(20, 7);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(201, 91);
			this.groupBox3.TabIndex = 23;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Send Summary of Care to Doctor";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 43);
			this.label1.TabIndex = 23;
			this.label1.Text = "includes 2 files:\r\nccd.xml - the data\r\nccd.xsl - for human readable viewing";
			// 
			// butExport
			// 
			this.butExport.Location = new System.Drawing.Point(8, 19);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(84, 23);
			this.butExport.TabIndex = 12;
			this.butExport.Text = "Export";
			this.butExport.UseVisualStyleBackColor = true;
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butSendEmail
			// 
			this.butSendEmail.Location = new System.Drawing.Point(109, 19);
			this.butSendEmail.Name = "butSendEmail";
			this.butSendEmail.Size = new System.Drawing.Size(84, 23);
			this.butSendEmail.TabIndex = 11;
			this.butSendEmail.Text = "by E-mail";
			this.butSendEmail.UseVisualStyleBackColor = true;
			this.butSendEmail.Click += new System.EventHandler(this.butSendEmail_Click);
			// 
			// FormEhrSummaryOfCare
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(441, 434);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridRec);
			this.Controls.Add(this.gridSent);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrSummaryOfCare";
			this.Text = "Summary of Care";
			this.Load += new System.EventHandler(this.FormSummaryOfCare_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butRecEmail;
		private OpenDental.UI.GridOD gridSent;
				private System.Windows.Forms.Button butDelete;
				private OpenDental.UI.GridOD gridRec;
				private System.Windows.Forms.Button butShowXhtml;
				private System.Windows.Forms.Button butShowXml;
				private System.Windows.Forms.GroupBox groupBox1;
				private System.Windows.Forms.GroupBox groupBox2;
				private System.Windows.Forms.Button butRecFile;
				private System.Windows.Forms.GroupBox groupBox3;
				private System.Windows.Forms.Label label1;
				private System.Windows.Forms.Button butExport;
				private System.Windows.Forms.Button butSendEmail;
	}
}