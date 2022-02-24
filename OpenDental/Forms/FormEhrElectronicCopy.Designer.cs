namespace OpenDental {
	partial class FormEhrElectronicCopy {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrElectronicCopy));
			this.butClose = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.butRequest = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butShowXhtml = new System.Windows.Forms.Button();
			this.butShowXml = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butExport = new System.Windows.Forms.Button();
			this.butSendEmail = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(331, 448);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(27, 448);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butRequest
			// 
			this.butRequest.Location = new System.Drawing.Point(29, 12);
			this.butRequest.Name = "butRequest";
			this.butRequest.Size = new System.Drawing.Size(103, 23);
			this.butRequest.TabIndex = 17;
			this.butRequest.Text = "Requested";
			this.butRequest.UseVisualStyleBackColor = true;
			this.butRequest.Click += new System.EventHandler(this.butRequest_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(28, 149);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(378, 283);
			this.gridMain.TabIndex = 16;
			this.gridMain.Title = "History";
			this.gridMain.TranslationName = "TableHistory";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(135, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 18);
			this.label1.TabIndex = 18;
			this.label1.Text = "(optional)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butShowXhtml);
			this.groupBox1.Controls.Add(this.butShowXml);
			this.groupBox1.Location = new System.Drawing.Point(258, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(103, 77);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show";
			// 
			// butShowXhtml
			// 
			this.butShowXhtml.Location = new System.Drawing.Point(10, 19);
			this.butShowXhtml.Name = "butShowXhtml";
			this.butShowXhtml.Size = new System.Drawing.Size(84, 23);
			this.butShowXhtml.TabIndex = 18;
			this.butShowXhtml.Text = "Show xhtml";
			this.butShowXhtml.UseVisualStyleBackColor = true;
			this.butShowXhtml.Click += new System.EventHandler(this.butShowXhtml_Click);
			// 
			// butShowXml
			// 
			this.butShowXml.Location = new System.Drawing.Point(10, 46);
			this.butShowXml.Name = "butShowXml";
			this.butShowXml.Size = new System.Drawing.Size(84, 23);
			this.butShowXml.TabIndex = 19;
			this.butShowXml.Text = "Show xml";
			this.butShowXml.UseVisualStyleBackColor = true;
			this.butShowXml.Click += new System.EventHandler(this.butShowXml_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.butExport);
			this.groupBox3.Controls.Add(this.butSendEmail);
			this.groupBox3.Location = new System.Drawing.Point(28, 48);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(201, 91);
			this.groupBox3.TabIndex = 24;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Provide Electronic Copy to Patient";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(192, 43);
			this.label2.TabIndex = 23;
			this.label2.Text = "includes 2 files:\r\nccd.xml - the data\r\nccd.xsl - for human readable viewing";
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
			// FormEhrElectronicCopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(437, 483);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butRequest);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrElectronicCopy";
			this.Text = "Electronic Copy for Patient";
			this.Load += new System.EventHandler(this.FormElectronicCopy_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butClose;
				private OpenDental.UI.GridOD gridMain;
				private System.Windows.Forms.Button butDelete;
				private System.Windows.Forms.Button butRequest;
				private System.Windows.Forms.Label label1;
				private System.Windows.Forms.GroupBox groupBox1;
				private System.Windows.Forms.Button butShowXhtml;
				private System.Windows.Forms.Button butShowXml;
				private System.Windows.Forms.GroupBox groupBox3;
				private System.Windows.Forms.Label label2;
				private System.Windows.Forms.Button butExport;
				private System.Windows.Forms.Button butSendEmail;
	}
}