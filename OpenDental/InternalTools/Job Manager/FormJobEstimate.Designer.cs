namespace OpenDental{
	partial class FormJobEstimate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobEstimate));
			this.labelReviewTime = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textConcept = new ValidDouble();
			this.textWriteup = new ValidDouble();
			this.textDevelopment = new ValidDouble();
			this.textReview = new ValidDouble();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelMain = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelReviewTime
			// 
			this.labelReviewTime.Location = new System.Drawing.Point(9, 82);
			this.labelReviewTime.Name = "labelReviewTime";
			this.labelReviewTime.Size = new System.Drawing.Size(134, 20);
			this.labelReviewTime.TabIndex = 34;
			this.labelReviewTime.Text = "Development Estimate";
			this.labelReviewTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(15, 154);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.CommLog;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(329, 88);
			this.textNote.TabIndex = 28;
			this.textNote.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(9, 38);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(134, 20);
			this.label10.TabIndex = 22;
			this.label10.Text = "Concept Estimate";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(12, 131);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(107, 20);
			this.label9.TabIndex = 24;
			this.label9.Text = "Reason For Change:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 20);
			this.label1.TabIndex = 26;
			this.label1.Text = "Writeup Estimate";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(350, 188);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 29;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(350, 218);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 30;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 20);
			this.label2.TabIndex = 38;
			this.label2.Text = "Review Estimate";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textConcept
			// 
			this.textConcept.Location = new System.Drawing.Point(149, 39);
			this.textConcept.MaxVal = 300D;
			this.textConcept.MinVal = 0D;
			this.textConcept.Name = "textConcept";
			this.textConcept.Size = new System.Drawing.Size(51, 20);
			this.textConcept.TabIndex = 40;
			// 
			// textWriteup
			// 
			this.textWriteup.Location = new System.Drawing.Point(149, 61);
			this.textWriteup.MaxVal = 300D;
			this.textWriteup.MinVal = 0D;
			this.textWriteup.Name = "textWriteup";
			this.textWriteup.Size = new System.Drawing.Size(51, 20);
			this.textWriteup.TabIndex = 41;
			// 
			// textDevelopment
			// 
			this.textDevelopment.Location = new System.Drawing.Point(149, 83);
			this.textDevelopment.MaxVal = 300D;
			this.textDevelopment.MinVal = 0D;
			this.textDevelopment.Name = "textDevelopment";
			this.textDevelopment.Size = new System.Drawing.Size(51, 20);
			this.textDevelopment.TabIndex = 42;
			// 
			// textReview
			// 
			this.textReview.Location = new System.Drawing.Point(149, 105);
			this.textReview.MaxVal = 300D;
			this.textReview.MinVal = 0D;
			this.textReview.Name = "textReview";
			this.textReview.Size = new System.Drawing.Size(51, 20);
			this.textReview.TabIndex = 43;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(208, 105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(213, 20);
			this.label3.TabIndex = 44;
			this.label3.Text = "Only include review time for one engineer";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(206, 39);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(153, 20);
			this.label4.TabIndex = 45;
			this.label4.Text = "Estimates are made in hours";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMain
			// 
			this.labelMain.Location = new System.Drawing.Point(9, 4);
			this.labelMain.Name = "labelMain";
			this.labelMain.Size = new System.Drawing.Size(420, 32);
			this.labelMain.TabIndex = 46;
			this.labelMain.Text = "Please add estimates and include not only your time, but also write-up and review" +
    " time.";
			// 
			// FormJobEstimate
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(434, 256);
			this.Controls.Add(this.labelMain);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textReview);
			this.Controls.Add(this.textDevelopment);
			this.Controls.Add(this.textWriteup);
			this.Controls.Add(this.textConcept);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelReviewTime);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobEstimate";
			this.Text = "Estimate Edit";
			this.Load += new System.EventHandler(this.FormJobEstimate_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelReviewTime;
		private ODtextBox textNote;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private ValidDouble textConcept;
		private ValidDouble textWriteup;
		private ValidDouble textDevelopment;
		private ValidDouble textReview;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelMain;
	}
}