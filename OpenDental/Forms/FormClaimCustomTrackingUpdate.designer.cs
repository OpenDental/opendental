namespace OpenDental {
	partial class FormClaimCustomTrackingUpdate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimCustomTrackingUpdate));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelClaimTrack = new System.Windows.Forms.Label();
			this.labelNote = new System.Windows.Forms.Label();
			this.textNotes = new OpenDental.ODtextBox();
			this.comboCustomTracking = new UI.ComboBoxOD();
			this.comboErrorCode = new OpenDental.UI.ComboBoxOD();
			this.label = new System.Windows.Forms.Label();
			this.textErrorDesc = new OpenDental.ODtextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(193, 343);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&Update";
			this.butOK.Click += new System.EventHandler(this.butUpdate_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(274, 343);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelClaimTrack
			// 
			this.labelClaimTrack.Location = new System.Drawing.Point(10, 3);
			this.labelClaimTrack.Name = "labelClaimTrack";
			this.labelClaimTrack.Size = new System.Drawing.Size(216, 16);
			this.labelClaimTrack.TabIndex = 58;
			this.labelClaimTrack.Text = "Custom Track Status";
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(12, 129);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(216, 16);
			this.labelNote.TabIndex = 57;
			this.labelNote.Text = "Note";
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.Location = new System.Drawing.Point(13, 148);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.ClaimCustomTrack;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(336, 189);
			this.textNotes.TabIndex = 56;
			this.textNotes.Text = "";
			// 
			// comboCustomTracking
			// 
			this.comboCustomTracking.Location = new System.Drawing.Point(13, 21);
			this.comboCustomTracking.Name = "comboCustomTracking";
			this.comboCustomTracking.Size = new System.Drawing.Size(237, 21);
			this.comboCustomTracking.TabIndex = 156;
			// 
			// comboErrorCode
			// 
			this.comboErrorCode.Location = new System.Drawing.Point(14, 66);
			this.comboErrorCode.Name = "comboErrorCode";
			this.comboErrorCode.Size = new System.Drawing.Size(237, 21);
			this.comboErrorCode.TabIndex = 160;
			this.comboErrorCode.SelectionChangeCommitted += new System.EventHandler(this.comboErrorCode_SelectionChangeCommitted);
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(11, 48);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(216, 16);
			this.label.TabIndex = 159;
			this.label.Text = "Error Code";
			// 
			// textErrorDesc
			// 
			this.textErrorDesc.AcceptsTab = true;
			this.textErrorDesc.BackColor = System.Drawing.SystemColors.Control;
			this.textErrorDesc.DetectLinksEnabled = false;
			this.textErrorDesc.DetectUrls = false;
			this.textErrorDesc.Location = new System.Drawing.Point(14, 90);
			this.textErrorDesc.Name = "roTextErrorDesc";
			this.textErrorDesc.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textErrorDesc.ReadOnly = true;
			this.textErrorDesc.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textErrorDesc.Size = new System.Drawing.Size(237, 36);
			this.textErrorDesc.TabIndex = 161;
			this.textErrorDesc.Text = "";
			// 
			// FormClaimCustomTrackingUpdate
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(361, 379);
			this.Controls.Add(this.textErrorDesc);
			this.Controls.Add(this.comboErrorCode);
			this.Controls.Add(this.label);
			this.Controls.Add(this.comboCustomTracking);
			this.Controls.Add(this.labelClaimTrack);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimCustomTrackingUpdate";
			this.Text = "Custom Tracking Status Update";
			this.Load += new System.EventHandler(this.FormClaimCustomTrackingUpdate_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelClaimTrack;
		private System.Windows.Forms.Label labelNote;
		private ODtextBox textNotes;
		private UI.ComboBoxOD comboCustomTracking;
		private UI.ComboBoxOD comboErrorCode;
		private System.Windows.Forms.Label label;
		private ODtextBox textErrorDesc;
	}
}