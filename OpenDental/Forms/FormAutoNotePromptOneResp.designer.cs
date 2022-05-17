namespace OpenDental{
	partial class FormAutoNotePromptOneResp {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNotePromptOneResp));
			this.labelPrompt = new System.Windows.Forms.Label();
			this.listMain = new OpenDental.UI.ListBoxOD();
			this.butPreview = new OpenDental.UI.Button();
			this.butSkip = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butBack = new OpenDental.UI.Button();
			this.butNext = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelPrompt
			// 
			this.labelPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPrompt.Location = new System.Drawing.Point(12, 3);
			this.labelPrompt.Name = "labelPrompt";
			this.labelPrompt.Size = new System.Drawing.Size(400, 56);
			this.labelPrompt.TabIndex = 114;
			this.labelPrompt.Text = "Prompt";
			this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listMain
			// 
			this.listMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listMain.Location = new System.Drawing.Point(11, 62);
			this.listMain.Name = "listMain";
			this.listMain.Size = new System.Drawing.Size(395, 212);
			this.listMain.TabIndex = 1;
			this.listMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listMain_MouseDoubleClick);
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPreview.Location = new System.Drawing.Point(156, 280);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(75, 24);
			this.butPreview.TabIndex = 15;
			this.butPreview.Text = "&Preview";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// butSkip
			// 
			this.butSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSkip.Location = new System.Drawing.Point(231, 280);
			this.butSkip.Name = "butSkip";
			this.butSkip.Size = new System.Drawing.Size(98, 24);
			this.butSkip.TabIndex = 10;
			this.butSkip.Text = "&Remove Prompt";
			this.butSkip.Click += new System.EventHandler(this.butRemovePrompt_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(337, 325);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 20;
			this.butCancel.Text = "&Exit";
			this.butCancel.Click += new System.EventHandler(this.butExit_Click);
			// 
			// butBack
			// 
			this.butBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butBack.Image = global::OpenDental.Properties.Resources.Left;
			this.butBack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBack.Location = new System.Drawing.Point(81, 280);
			this.butBack.Name = "butBack";
			this.butBack.Size = new System.Drawing.Size(75, 24);
			this.butBack.TabIndex = 117;
			this.butBack.Text = "&Back";
			this.butBack.Visible = false;
			this.butBack.Click += new System.EventHandler(this.butBack_Click);
			// 
			// butNext
			// 
			this.butNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNext.Image = global::OpenDental.Properties.Resources.Right;
			this.butNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butNext.Location = new System.Drawing.Point(329, 280);
			this.butNext.Name = "butNext";
			this.butNext.Size = new System.Drawing.Size(77, 24);
			this.butNext.TabIndex = 119;
			this.butNext.Text = "&Next";
			this.butNext.Click += new System.EventHandler(this.butNext_Click);
			// 
			// FormAutoNotePromptOneResp
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(422, 361);
			this.Controls.Add(this.butNext);
			this.Controls.Add(this.butBack);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.butSkip);
			this.Controls.Add(this.listMain);
			this.Controls.Add(this.labelPrompt);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAutoNotePromptOneResp";
			this.Text = "Prompt One Response";
			this.Load += new System.EventHandler(this.FormAutoNotePromptOneResp_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAutoNotePromptOneResp_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelPrompt;
		private OpenDental.UI.ListBoxOD listMain;
		private OpenDental.UI.Button butSkip;
		private OpenDental.UI.Button butPreview;
		private UI.Button butBack;
		private UI.Button butNext;
	}
}