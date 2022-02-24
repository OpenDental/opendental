namespace OpenDental {
	partial class FormEhrClinicalSummary {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrClinicalSummary));
			this.butClose = new System.Windows.Forms.Button();
			this.butSendToPortal = new System.Windows.Forms.Button();
			this.gridEHRMeasureEvents = new OpenDental.UI.GridOD();
			this.butDelete = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butShowXhtml = new System.Windows.Forms.Button();
			this.butShowXml = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butExport = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(180, 440);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSendToPortal
			// 
			this.butSendToPortal.Location = new System.Drawing.Point(109, 19);
			this.butSendToPortal.Name = "butSendToPortal";
			this.butSendToPortal.Size = new System.Drawing.Size(84, 23);
			this.butSendToPortal.TabIndex = 11;
			this.butSendToPortal.Text = "to Portal";
			this.butSendToPortal.UseVisualStyleBackColor = true;
			this.butSendToPortal.Click += new System.EventHandler(this.butSendToPortal_Click);
			// 
			// gridEHRMeasureEvents
			// 
			this.gridEHRMeasureEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEHRMeasureEvents.Location = new System.Drawing.Point(15, 166);
			this.gridEHRMeasureEvents.Name = "gridEHRMeasureEvents";
			this.gridEHRMeasureEvents.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridEHRMeasureEvents.Size = new System.Drawing.Size(240, 261);
			this.gridEHRMeasureEvents.TabIndex = 13;
			this.gridEHRMeasureEvents.Title = "Clinical Summaries Sent to Patient";
			this.gridEHRMeasureEvents.TranslationName = "TableSummaries";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(15, 440);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 12;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butShowXhtml);
			this.groupBox1.Controls.Add(this.butShowXml);
			this.groupBox1.Location = new System.Drawing.Point(39, 108);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(201, 50);
			this.groupBox1.TabIndex = 21;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show";
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
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.butExport);
			this.groupBox2.Controls.Add(this.butSendToPortal);
			this.groupBox2.Location = new System.Drawing.Point(39, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(201, 91);
			this.groupBox2.TabIndex = 22;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Send Clinical Summary to Patient";
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
			// FormEhrClinicalSummary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(273, 475);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridEHRMeasureEvents);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrClinicalSummary";
			this.Text = "Clinical Summary";
			this.Load += new System.EventHandler(this.FormClinicalSummary_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butSendToPortal;
        private OpenDental.UI.GridOD gridEHRMeasureEvents;
				private System.Windows.Forms.Button butDelete;
				private System.Windows.Forms.GroupBox groupBox1;
				private System.Windows.Forms.Button butShowXhtml;
				private System.Windows.Forms.Button butShowXml;
				private System.Windows.Forms.GroupBox groupBox2;
				private System.Windows.Forms.Button butExport;
				private System.Windows.Forms.Label label1;
	}
}