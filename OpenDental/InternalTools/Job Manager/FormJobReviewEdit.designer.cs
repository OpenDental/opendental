namespace OpenDental{
	partial class FormJobReviewEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobReviewEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textDateLastEdited = new System.Windows.Forms.TextBox();
			this.listBoxStatus = new OpenDental.UI.ListBoxOD();
			this.textDescription = new OpenDental.ODtextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboReviewer = new System.Windows.Forms.ComboBox();
			this.butLogin = new OpenDental.UI.Button();
			this.labelReviewTime = new System.Windows.Forms.Label();
			this.textReviewTime = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Reviewer";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 60);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(132, 20);
			this.label5.TabIndex = 0;
			this.label5.Text = "Status";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(12, 198);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(83, 20);
			this.label9.TabIndex = 0;
			this.label9.Text = "Description";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 16);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(134, 20);
			this.label10.TabIndex = 0;
			this.label10.Text = "Last Edited Date";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateLastEdited
			// 
			this.textDateLastEdited.Location = new System.Drawing.Point(141, 16);
			this.textDateLastEdited.MaxLength = 100;
			this.textDateLastEdited.Name = "textDateLastEdited";
			this.textDateLastEdited.ReadOnly = true;
			this.textDateLastEdited.Size = new System.Drawing.Size(183, 20);
			this.textDateLastEdited.TabIndex = 0;
			this.textDateLastEdited.TabStop = false;
			// 
			// listBoxStatus
			// 
			this.listBoxStatus.Location = new System.Drawing.Point(141, 61);
			this.listBoxStatus.Name = "listBoxStatus";
			this.listBoxStatus.Size = new System.Drawing.Size(183, 108);
			this.listBoxStatus.TabIndex = 4;
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
			this.textDescription.Location = new System.Drawing.Point(12, 221);
			this.textDescription.Name = "textDescription";
			this.textDescription.QuickPasteType = OpenDentBusiness.QuickPasteType.CommLog;
			this.textDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(389, 199);
			this.textDescription.TabIndex = 11;
			this.textDescription.Text = "";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 426);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 17;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(245, 426);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 15;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(326, 426);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 16;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboReviewer
			// 
			this.comboReviewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReviewer.FormattingEnabled = true;
			this.comboReviewer.Location = new System.Drawing.Point(141, 38);
			this.comboReviewer.Name = "comboReviewer";
			this.comboReviewer.Size = new System.Drawing.Size(183, 21);
			this.comboReviewer.TabIndex = 18;
			// 
			// butLogin
			// 
			this.butLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butLogin.Location = new System.Drawing.Point(326, 191);
			this.butLogin.Name = "butLogin";
			this.butLogin.Size = new System.Drawing.Size(75, 24);
			this.butLogin.TabIndex = 19;
			this.butLogin.Text = "Login as...";
			this.butLogin.Click += new System.EventHandler(this.butLogin_Click);
			// 
			// labelReviewTime
			// 
			this.labelReviewTime.Location = new System.Drawing.Point(6, 171);
			this.labelReviewTime.Name = "labelReviewTime";
			this.labelReviewTime.Size = new System.Drawing.Size(134, 20);
			this.labelReviewTime.TabIndex = 20;
			this.labelReviewTime.Text = "Review Time (Minutes)";
			this.labelReviewTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReviewTime
			// 
			this.textReviewTime.Location = new System.Drawing.Point(141, 171);
			this.textReviewTime.MaxVal = 1000000;
			this.textReviewTime.MinVal = 0;
			this.textReviewTime.Name = "textReviewTime";
			this.textReviewTime.Size = new System.Drawing.Size(61, 20);
			this.textReviewTime.TabIndex = 21;
			this.textReviewTime.ShowZero = false;
			// 
			// FormJobReviewEdit
			// 
			this.ClientSize = new System.Drawing.Size(413, 462);
			this.Controls.Add(this.textReviewTime);
			this.Controls.Add(this.labelReviewTime);
			this.Controls.Add(this.butLogin);
			this.Controls.Add(this.comboReviewer);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.listBoxStatus);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textDateLastEdited);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobReviewEdit";
			this.Text = "Job Review Edit";
			this.Load += new System.EventHandler(this.FormJobReviewEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textDateLastEdited;
		private OpenDental.UI.ListBoxOD listBoxStatus;
		private UI.Button butDelete;
		private ODtextBox textDescription;
		private System.Windows.Forms.ComboBox comboReviewer;
		private UI.Button butLogin;
		private System.Windows.Forms.Label labelReviewTime;
		private ValidNum textReviewTime;
	}
}