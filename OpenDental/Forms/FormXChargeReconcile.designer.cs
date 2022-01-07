namespace OpenDental{
	partial class FormXChargeReconcile {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXChargeReconcile));
			this.label5 = new System.Windows.Forms.Label();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.butExtra = new OpenDental.UI.Button();
			this.butMissing = new OpenDental.UI.Button();
			this.butPayments = new OpenDental.UI.Button();
			this.butViewImported = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label5.Location = new System.Drawing.Point(105, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(244, 20);
			this.label5.TabIndex = 14;
			this.label5.Text = "Import X-Charge transactions from text.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(282, 94);
			this.date2.Name = "date2";
			this.date2.TabIndex = 24;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(18, 94);
			this.date1.Name = "date1";
			this.date1.TabIndex = 23;
			// 
			// label2
			// 
			this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label2.Location = new System.Drawing.Point(105, 344);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(296, 20);
			this.label2.TabIndex = 14;
			this.label2.Text = "View X-Charge transactions with no payment in OD.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label3.Location = new System.Drawing.Point(105, 372);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(272, 20);
			this.label3.TabIndex = 14;
			this.label3.Text = "View payments in OD with no X-Charge transaction.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label4.Location = new System.Drawing.Point(105, 270);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(244, 20);
			this.label4.TabIndex = 14;
			this.label4.Text = "View imported X-Charge transactions.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label7.Location = new System.Drawing.Point(105, 300);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(296, 20);
			this.label7.TabIndex = 14;
			this.label7.Text = "View credit card payments in Open Dental.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butExtra
			// 
			this.butExtra.Location = new System.Drawing.Point(18, 370);
			this.butExtra.Name = "butExtra";
			this.butExtra.Size = new System.Drawing.Size(81, 24);
			this.butExtra.TabIndex = 3;
			this.butExtra.Text = "Extra";
			this.butExtra.Click += new System.EventHandler(this.butExtra_Click);
			// 
			// butMissing
			// 
			this.butMissing.Location = new System.Drawing.Point(18, 342);
			this.butMissing.Name = "butMissing";
			this.butMissing.Size = new System.Drawing.Size(81, 24);
			this.butMissing.TabIndex = 3;
			this.butMissing.Text = "Missing";
			this.butMissing.Click += new System.EventHandler(this.butMissing_Click);
			// 
			// butPayments
			// 
			this.butPayments.Location = new System.Drawing.Point(18, 298);
			this.butPayments.Name = "butPayments";
			this.butPayments.Size = new System.Drawing.Size(81, 24);
			this.butPayments.TabIndex = 3;
			this.butPayments.Text = "Payments";
			this.butPayments.Click += new System.EventHandler(this.butPayments_Click);
			// 
			// butViewImported
			// 
			this.butViewImported.Location = new System.Drawing.Point(18, 268);
			this.butViewImported.Name = "butViewImported";
			this.butViewImported.Size = new System.Drawing.Size(81, 24);
			this.butViewImported.TabIndex = 3;
			this.butViewImported.Text = "View Imported";
			this.butViewImported.Click += new System.EventHandler(this.butViewImported_Click);
			// 
			// butImport
			// 
			this.butImport.Location = new System.Drawing.Point(18, 33);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(81, 24);
			this.butImport.TabIndex = 3;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(414, 378);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormXChargeReconcile
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(527, 432);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butExtra);
			this.Controls.Add(this.butMissing);
			this.Controls.Add(this.butPayments);
			this.Controls.Add(this.butViewImported);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormXChargeReconcile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "X-Charge Reconcile";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.Button butImport;
		private System.Windows.Forms.Label label5;
		private UI.Button butViewImported;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private UI.Button butMissing;
		private UI.Button butExtra;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private UI.Button butPayments;
		private System.Windows.Forms.Label label7;
	}
}