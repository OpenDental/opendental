namespace OpenDental {
	partial class FormFaqPicker {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFaqPicker));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.dataGridMain = new System.Windows.Forms.DataGridView();
			this.QuestionText = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IsStickied = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FaqNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LinkedStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.textManualPage = new System.Windows.Forms.TextBox();
			this.textManualVersion = new System.Windows.Forms.TextBox();
			this.butRefresh = new System.Windows.Forms.Button();
			this.butAdd = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.checkShowUnlinked = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridMain)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 14);
			this.label1.TabIndex = 7;
			this.label1.Text = "Manual Page Name:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(376, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 14);
			this.label2.TabIndex = 10;
			this.label2.Text = "Manual Version:";
			// 
			// dataGridMain
			// 
			this.dataGridMain.AllowUserToAddRows = false;
			this.dataGridMain.AllowUserToDeleteRows = false;
			this.dataGridMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QuestionText,
            this.Version,
            this.IsStickied,
            this.FaqNum,
            this.LinkedStatus});
			this.dataGridMain.Location = new System.Drawing.Point(12, 41);
			this.dataGridMain.Name = "dataGridMain";
			this.dataGridMain.ReadOnly = true;
			this.dataGridMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridMain.Size = new System.Drawing.Size(666, 279);
			this.dataGridMain.TabIndex = 11;
			this.dataGridMain.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridMain_CellDoubleClick);
			// 
			// QuestionText
			// 
			this.QuestionText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.QuestionText.DataPropertyName = "QuestionText";
			this.QuestionText.HeaderText = "Question Text";
			this.QuestionText.Name = "QuestionText";
			this.QuestionText.ReadOnly = true;
			// 
			// Version
			// 
			this.Version.DataPropertyName = "Version";
			this.Version.HeaderText = "Version";
			this.Version.Name = "Version";
			this.Version.ReadOnly = true;
			// 
			// IsStickied
			// 
			this.IsStickied.DataPropertyName = "IsStickied";
			this.IsStickied.HeaderText = "Is Stickied";
			this.IsStickied.Name = "IsStickied";
			this.IsStickied.ReadOnly = true;
			// 
			// FaqNum
			// 
			this.FaqNum.DataPropertyName = "FaqNum";
			this.FaqNum.HeaderText = "FaqNum";
			this.FaqNum.Name = "FaqNum";
			this.FaqNum.ReadOnly = true;
			this.FaqNum.Visible = false;
			// 
			// LinkedStatus
			// 
			this.LinkedStatus.HeaderText = "Linked Status";
			this.LinkedStatus.Name = "LinkedStatus";
			this.LinkedStatus.ReadOnly = true;
			this.LinkedStatus.Visible = false;
			// 
			// textManualPage
			// 
			this.textManualPage.Location = new System.Drawing.Point(116, 15);
			this.textManualPage.Name = "textManualPage";
			this.textManualPage.Size = new System.Drawing.Size(196, 20);
			this.textManualPage.TabIndex = 12;
			// 
			// textManualVersion
			// 
			this.textManualVersion.Location = new System.Drawing.Point(472, 15);
			this.textManualVersion.Name = "textManualVersion";
			this.textManualVersion.Size = new System.Drawing.Size(100, 20);
			this.textManualVersion.TabIndex = 13;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(684, 44);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 14;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.ButRefresh_Click);
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(684, 173);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(74, 23);
			this.butAdd.TabIndex = 15;
			this.butAdd.Text = "&Add";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.ButAdd_Click);
			// 
			// butCancel
			// 
			this.butCancel.Location = new System.Drawing.Point(685, 328);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 16;
			this.butCancel.Text = "&Close";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkShowUnlinked
			// 
			this.checkShowUnlinked.AutoSize = true;
			this.checkShowUnlinked.Location = new System.Drawing.Point(14, 335);
			this.checkShowUnlinked.Name = "checkShowUnlinked";
			this.checkShowUnlinked.Size = new System.Drawing.Size(249, 17);
			this.checkShowUnlinked.TabIndex = 17;
			this.checkShowUnlinked.Text = "Show Unlinked FAQs (Does not consider filters)";
			this.checkShowUnlinked.UseVisualStyleBackColor = true;
			// 
			// FormFaqPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(772, 362);
			this.Controls.Add(this.checkShowUnlinked);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textManualVersion);
			this.Controls.Add(this.textManualPage);
			this.Controls.Add(this.dataGridMain);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFaqPicker";
			this.Text = "FormFaqPicker";
			this.Load += new System.EventHandler(this.FormFaqPicker_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridMain)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DataGridView dataGridMain;
		private System.Windows.Forms.TextBox textManualPage;
		private System.Windows.Forms.TextBox textManualVersion;
		private System.Windows.Forms.Button butRefresh;
		private System.Windows.Forms.Button butAdd;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.CheckBox checkShowUnlinked;
		private System.Windows.Forms.DataGridViewTextBoxColumn QuestionText;
		private System.Windows.Forms.DataGridViewTextBoxColumn Version;
		private System.Windows.Forms.DataGridViewTextBoxColumn IsStickied;
		private System.Windows.Forms.DataGridViewTextBoxColumn FaqNum;
		private System.Windows.Forms.DataGridViewTextBoxColumn LinkedStatus;
	}
}