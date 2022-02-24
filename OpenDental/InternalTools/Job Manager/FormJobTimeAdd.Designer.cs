namespace OpenDental{
	partial class FormJobTimeAdd {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobTimeAdd));
			this.textDescription = new OpenDental.ODtextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textDate = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textUser = new System.Windows.Forms.TextBox();
			this.labelReviewTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textTimeHours = new OpenDental.ValidNum();
			this.textTimeMinutes = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// textDescription
			// 
			this.textDescription.AcceptsTab = true;
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.BackColor = System.Drawing.SystemColors.Window;
			this.textDescription.DetectLinksEnabled = false;
			this.textDescription.DetectUrls = false;
			this.textDescription.Location = new System.Drawing.Point(15, 105);
			this.textDescription.Name = "textDescription";
			this.textDescription.QuickPasteType = OpenDentBusiness.QuickPasteType.CommLog;
			this.textDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(389, 157);
			this.textDescription.TabIndex = 28;
			this.textDescription.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(9, 14);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(134, 20);
			this.label10.TabIndex = 22;
			this.label10.Text = "Entry Date";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(144, 14);
			this.textDate.MaxLength = 100;
			this.textDate.Name = "textDate";
			this.textDate.ReadOnly = true;
			this.textDate.Size = new System.Drawing.Size(183, 20);
			this.textDate.TabIndex = 23;
			this.textDate.TabStop = false;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(12, 82);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(83, 20);
			this.label9.TabIndex = 24;
			this.label9.Text = "Note";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 20);
			this.label1.TabIndex = 26;
			this.label1.Text = "User";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(248, 268);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 29;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(329, 268);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 30;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(144, 36);
			this.textUser.MaxLength = 100;
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(183, 20);
			this.textUser.TabIndex = 36;
			this.textUser.TabStop = false;
			// 
			// labelReviewTime
			// 
			this.labelReviewTime.Location = new System.Drawing.Point(9, 58);
			this.labelReviewTime.Name = "labelReviewTime";
			this.labelReviewTime.Size = new System.Drawing.Size(134, 20);
			this.labelReviewTime.TabIndex = 34;
			this.labelReviewTime.Text = "Hours";
			this.labelReviewTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(205, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 20);
			this.label2.TabIndex = 38;
			this.label2.Text = "Minutes";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeHours
			// 
			this.textTimeHours.Location = new System.Drawing.Point(144, 58);
			this.textTimeHours.MaxVal = 2000;
			this.textTimeHours.MinVal = -2000;
			this.textTimeHours.Name = "textTimeHours";
			this.textTimeHours.Size = new System.Drawing.Size(55, 20);
			this.textTimeHours.TabIndex = 40;
			this.textTimeHours.ShowZero = false;
			// 
			// textTimeMinutes
			// 
			this.textTimeMinutes.Location = new System.Drawing.Point(272, 58);
			this.textTimeMinutes.MaxVal = 2000;
			this.textTimeMinutes.MinVal = -2000;
			this.textTimeMinutes.Name = "textTimeMinutes";
			this.textTimeMinutes.Size = new System.Drawing.Size(55, 20);
			this.textTimeMinutes.TabIndex = 41;
			this.textTimeMinutes.ShowZero = false;
			// 
			// FormJobTimeAdd
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(413, 306);
			this.Controls.Add(this.textTimeMinutes);
			this.Controls.Add(this.textTimeHours);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.labelReviewTime);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobTimeAdd";
			this.Text = "Time Log Add";
			this.Load += new System.EventHandler(this.FormJobTimeAdd_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private ODtextBox textDescription;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textDate;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label labelReviewTime;
		private System.Windows.Forms.Label label2;
		private ValidNum textTimeHours;
		private ValidNum textTimeMinutes;
	}
}