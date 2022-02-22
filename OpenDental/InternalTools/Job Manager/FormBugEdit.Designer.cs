namespace OpenDental {
	partial class FormBugEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBugEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textVersionsFound = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.textVersionsFixed = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.comboPriority = new System.Windows.Forms.ComboBox();
			this.textPrivateDesc = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textLongDesc = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textDiscussion = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textSubmitter = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textCreationDate = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBugId = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.gridSubs = new OpenDental.UI.GridOD();
			this.butLast3found = new OpenDental.UI.Button();
			this.butLast2found = new OpenDental.UI.Button();
			this.butLast1found = new OpenDental.UI.Button();
			this.butLast3 = new OpenDental.UI.Button();
			this.butLast2 = new OpenDental.UI.Button();
			this.butLast1 = new OpenDental.UI.Button();
			this.butCopyDown = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(71, 89);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 18);
			this.label1.TabIndex = 3;
			this.label1.Text = "Priority Level";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVersionsFound
			// 
			this.textVersionsFound.Location = new System.Drawing.Point(150, 115);
			this.textVersionsFound.Name = "textVersionsFound";
			this.textVersionsFound.Size = new System.Drawing.Size(171, 20);
			this.textVersionsFound.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(42, 115);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 18);
			this.label2.TabIndex = 5;
			this.label2.Text = "Versions found";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(71, 62);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(76, 18);
			this.label5.TabIndex = 14;
			this.label5.Text = "Status";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.FormattingEnabled = true;
			this.comboStatus.Items.AddRange(new object[] {
            "None",
            "Accepted",
            "Verified",
            "Fixed",
            "DontUnderstand",
            "ByDesign",
            "Deleted",
            "WorksForMe",
            "WontFix",
            "Duplicate"});
			this.comboStatus.Location = new System.Drawing.Point(150, 62);
			this.comboStatus.MaxDropDownItems = 10;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(121, 21);
			this.comboStatus.TabIndex = 15;
			// 
			// textVersionsFixed
			// 
			this.textVersionsFixed.Location = new System.Drawing.Point(150, 141);
			this.textVersionsFixed.Name = "textVersionsFixed";
			this.textVersionsFixed.Size = new System.Drawing.Size(171, 20);
			this.textVersionsFixed.TabIndex = 17;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(42, 141);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 18);
			this.label6.TabIndex = 16;
			this.label6.Text = "Versions fixed";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(150, 167);
			this.textDescription.Multiline = true;
			this.textDescription.Name = "textDescription";
			this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(520, 65);
			this.textDescription.TabIndex = 19;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(42, 167);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(105, 18);
			this.label7.TabIndex = 18;
			this.label7.Text = "Description";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPriority
			// 
			this.comboPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPriority.FormattingEnabled = true;
			this.comboPriority.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
			this.comboPriority.Location = new System.Drawing.Point(150, 89);
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(59, 21);
			this.comboPriority.TabIndex = 20;
			// 
			// textPrivateDesc
			// 
			this.textPrivateDesc.Location = new System.Drawing.Point(150, 309);
			this.textPrivateDesc.Multiline = true;
			this.textPrivateDesc.Name = "textPrivateDesc";
			this.textPrivateDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textPrivateDesc.Size = new System.Drawing.Size(520, 65);
			this.textPrivateDesc.TabIndex = 22;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(15, 309);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(132, 18);
			this.label8.TabIndex = 21;
			this.label8.Text = "Private Description";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLongDesc
			// 
			this.textLongDesc.Location = new System.Drawing.Point(150, 238);
			this.textLongDesc.Multiline = true;
			this.textLongDesc.Name = "textLongDesc";
			this.textLongDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textLongDesc.Size = new System.Drawing.Size(520, 65);
			this.textLongDesc.TabIndex = 24;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(15, 238);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(132, 18);
			this.label9.TabIndex = 23;
			this.label9.Text = "Long Description";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscussion
			// 
			this.textDiscussion.Location = new System.Drawing.Point(150, 380);
			this.textDiscussion.Multiline = true;
			this.textDiscussion.Name = "textDiscussion";
			this.textDiscussion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textDiscussion.Size = new System.Drawing.Size(520, 65);
			this.textDiscussion.TabIndex = 26;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(15, 380);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(132, 18);
			this.label10.TabIndex = 25;
			this.label10.Text = "Discussion";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubmitter
			// 
			this.textSubmitter.Location = new System.Drawing.Point(150, 451);
			this.textSubmitter.Name = "textSubmitter";
			this.textSubmitter.ReadOnly = true;
			this.textSubmitter.Size = new System.Drawing.Size(171, 20);
			this.textSubmitter.TabIndex = 28;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(42, 451);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 18);
			this.label3.TabIndex = 27;
			this.label3.Text = "Submitter";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCreationDate
			// 
			this.textCreationDate.Location = new System.Drawing.Point(150, 36);
			this.textCreationDate.Name = "textCreationDate";
			this.textCreationDate.ReadOnly = true;
			this.textCreationDate.Size = new System.Drawing.Size(171, 20);
			this.textCreationDate.TabIndex = 30;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(42, 36);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(105, 18);
			this.label4.TabIndex = 29;
			this.label4.Text = "Creation Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBugId
			// 
			this.textBugId.Location = new System.Drawing.Point(150, 10);
			this.textBugId.Name = "textBugId";
			this.textBugId.ReadOnly = true;
			this.textBugId.Size = new System.Drawing.Size(59, 20);
			this.textBugId.TabIndex = 32;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(42, 10);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(105, 18);
			this.label11.TabIndex = 31;
			this.label11.Text = "Bug ID";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridSubs
			// 
			this.gridSubs.HasAddButton = true;
			this.gridSubs.HeadersVisible = false;
			this.gridSubs.Location = new System.Drawing.Point(327, 10);
			this.gridSubs.Name = "gridSubs";
			this.gridSubs.Size = new System.Drawing.Size(343, 98);
			this.gridSubs.TabIndex = 42;
			this.gridSubs.Title = "Bug Submissions";
			this.gridSubs.TranslationName = "TableBugs";
			this.gridSubs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSubs_CellDoubleClick);
			this.gridSubs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSubs_CellClick);
			this.gridSubs.TitleAddClick += new System.EventHandler(this.gridSubs_TitleAddClick);
			// 
			// butLast3found
			// 
			this.butLast3found.Location = new System.Drawing.Point(455, 114);
			this.butLast3found.Name = "butLast3found";
			this.butLast3found.Size = new System.Drawing.Size(58, 23);
			this.butLast3found.TabIndex = 41;
			this.butLast3found.Text = "Last 3";
			this.butLast3found.UseVisualStyleBackColor = true;
			this.butLast3found.Click += new System.EventHandler(this.butLast3found_Click);
			// 
			// butLast2found
			// 
			this.butLast2found.Location = new System.Drawing.Point(391, 114);
			this.butLast2found.Name = "butLast2found";
			this.butLast2found.Size = new System.Drawing.Size(58, 23);
			this.butLast2found.TabIndex = 40;
			this.butLast2found.Text = "Last 2";
			this.butLast2found.UseVisualStyleBackColor = true;
			this.butLast2found.Click += new System.EventHandler(this.butLast2found_Click);
			// 
			// butLast1found
			// 
			this.butLast1found.Location = new System.Drawing.Point(327, 114);
			this.butLast1found.Name = "butLast1found";
			this.butLast1found.Size = new System.Drawing.Size(58, 23);
			this.butLast1found.TabIndex = 39;
			this.butLast1found.Text = "Last 1";
			this.butLast1found.UseVisualStyleBackColor = true;
			this.butLast1found.Click += new System.EventHandler(this.butLast1found_Click);
			// 
			// butLast3
			// 
			this.butLast3.Location = new System.Drawing.Point(455, 140);
			this.butLast3.Name = "butLast3";
			this.butLast3.Size = new System.Drawing.Size(58, 23);
			this.butLast3.TabIndex = 37;
			this.butLast3.Text = "Last 3";
			this.butLast3.UseVisualStyleBackColor = true;
			this.butLast3.Click += new System.EventHandler(this.butLast3_Click);
			// 
			// butLast2
			// 
			this.butLast2.Location = new System.Drawing.Point(391, 140);
			this.butLast2.Name = "butLast2";
			this.butLast2.Size = new System.Drawing.Size(58, 23);
			this.butLast2.TabIndex = 36;
			this.butLast2.Text = "Last 2";
			this.butLast2.UseVisualStyleBackColor = true;
			this.butLast2.Click += new System.EventHandler(this.butLast2_Click);
			// 
			// butLast1
			// 
			this.butLast1.Location = new System.Drawing.Point(327, 140);
			this.butLast1.Name = "butLast1";
			this.butLast1.Size = new System.Drawing.Size(58, 23);
			this.butLast1.TabIndex = 35;
			this.butLast1.Text = "Last 1";
			this.butLast1.UseVisualStyleBackColor = true;
			this.butLast1.Click += new System.EventHandler(this.butLast1_Click);
			// 
			// butCopyDown
			// 
			this.butCopyDown.Location = new System.Drawing.Point(519, 125);
			this.butCopyDown.Name = "butCopyDown";
			this.butCopyDown.Size = new System.Drawing.Size(75, 23);
			this.butCopyDown.TabIndex = 33;
			this.butCopyDown.Text = "Copy down";
			this.butCopyDown.UseVisualStyleBackColor = true;
			this.butCopyDown.Click += new System.EventHandler(this.butCopyDown_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 490);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(65, 23);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(626, 490);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65, 23);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(555, 490);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(65, 23);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormBugEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(703, 525);
			this.Controls.Add(this.gridSubs);
			this.Controls.Add(this.butLast3found);
			this.Controls.Add(this.butLast2found);
			this.Controls.Add(this.butLast1found);
			this.Controls.Add(this.butLast3);
			this.Controls.Add(this.butLast2);
			this.Controls.Add(this.butLast1);
			this.Controls.Add(this.butCopyDown);
			this.Controls.Add(this.textBugId);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textCreationDate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textSubmitter);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDiscussion);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textLongDesc);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textPrivateDesc);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.comboPriority);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textVersionsFixed);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textVersionsFound);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBugEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Bug Edit";
			this.Load += new System.EventHandler(this.FormBugEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textVersionsFound;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.TextBox textVersionsFixed;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboPriority;
		private System.Windows.Forms.TextBox textPrivateDesc;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textLongDesc;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textDiscussion;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textSubmitter;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textCreationDate;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBugId;
		private System.Windows.Forms.Label label11;
		private OpenDental.UI.Button butCopyDown;
		private OpenDental.UI.Button butLast1;
		private OpenDental.UI.Button butLast2;
		private OpenDental.UI.Button butLast3;
		private OpenDental.UI.Button butLast3found;
		private OpenDental.UI.Button butLast2found;
		private OpenDental.UI.Button butLast1found;
		private UI.GridOD gridSubs;
	}
}