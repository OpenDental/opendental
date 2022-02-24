namespace OpenDental {
	partial class FormMobileBugEdit {
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
			this.label1 = new System.Windows.Forms.Label();
			this.textMobileBugNum = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textCreationDate = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textODVersionsFound = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textODVersionsFixed = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textSubmitter = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textEClipboardVersionFound = new System.Windows.Forms.TextBox();
			this.groupBoxEClipboard = new System.Windows.Forms.GroupBox();
			this.textEClipboardVersionFixed = new System.Windows.Forms.TextBox();
			this.groupBoxODMobile = new System.Windows.Forms.GroupBox();
			this.textODMobileVersionFixed = new System.Windows.Forms.TextBox();
			this.textODMobileVersionFound = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.checkiOS = new System.Windows.Forms.CheckBox();
			this.checkAndroid = new System.Windows.Forms.CheckBox();
			this.checkUWP = new System.Windows.Forms.CheckBox();
			this.checkODMobile = new System.Windows.Forms.CheckBox();
			this.checkEClipboard = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butLast3found = new OpenDental.UI.Button();
			this.butLast2found = new OpenDental.UI.Button();
			this.butLast1found = new OpenDental.UI.Button();
			this.butLast3 = new OpenDental.UI.Button();
			this.butLast2 = new OpenDental.UI.Button();
			this.butLast1 = new OpenDental.UI.Button();
			this.butODVersionCopyDown = new OpenDental.UI.Button();
			this.butODMobileNextVersion = new OpenDental.UI.Button();
			this.butODMobileLastVersion = new OpenDental.UI.Button();
			this.butEClipboardNextVersion = new OpenDental.UI.Button();
			this.butEClipboardLastVersion = new OpenDental.UI.Button();
			this.groupBoxEClipboard.SuspendLayout();
			this.groupBoxODMobile.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(39,10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105,18);
			this.label1.TabIndex = 44;
			this.label1.Text = "Mobile Bug Num";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMobileBugNum
			// 
			this.textMobileBugNum.Location = new System.Drawing.Point(150,10);
			this.textMobileBugNum.Name = "textMobileBugNum";
			this.textMobileBugNum.ReadOnly = true;
			this.textMobileBugNum.Size = new System.Drawing.Size(59,20);
			this.textMobileBugNum.TabIndex = 45;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(39,36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105,18);
			this.label2.TabIndex = 46;
			this.label2.Text = "Creation Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCreationDate
			// 
			this.textCreationDate.Location = new System.Drawing.Point(150,36);
			this.textCreationDate.Name = "textCreationDate";
			this.textCreationDate.ReadOnly = true;
			this.textCreationDate.Size = new System.Drawing.Size(171,20);
			this.textCreationDate.TabIndex = 47;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(68,62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76,18);
			this.label3.TabIndex = 48;
			this.label3.Text = "Status";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.FormattingEnabled = true;
			this.comboStatus.Items.AddRange(new object[] {
			"Found",
			"Fixed"});
			this.comboStatus.Location = new System.Drawing.Point(150,62);
			this.comboStatus.MaxDropDownItems = 10;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(121,21);
			this.comboStatus.TabIndex = 49;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(39,89);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(105,18);
			this.label4.TabIndex = 50;
			this.label4.Text = "OD Versions found";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textODVersionsFound
			// 
			this.textODVersionsFound.Location = new System.Drawing.Point(150,89);
			this.textODVersionsFound.Name = "textODVersionsFound";
			this.textODVersionsFound.Size = new System.Drawing.Size(171,20);
			this.textODVersionsFound.TabIndex = 51;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(39,115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(105,18);
			this.label5.TabIndex = 52;
			this.label5.Text = "OD Versions fixed";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textODVersionsFixed
			// 
			this.textODVersionsFixed.Location = new System.Drawing.Point(150,115);
			this.textODVersionsFixed.Name = "textODVersionsFixed";
			this.textODVersionsFixed.Size = new System.Drawing.Size(171,20);
			this.textODVersionsFixed.TabIndex = 53;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(93,387);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(51,13);
			this.label7.TabIndex = 55;
			this.label7.Text = "Submitter";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(83,313);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(60,13);
			this.label8.TabIndex = 56;
			this.label8.Text = "Description";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(351,13);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50,13);
			this.label9.TabIndex = 58;
			this.label9.Text = "Platforms";
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(150,313);
			this.textDescription.Multiline = true;
			this.textDescription.Name = "textDescription";
			this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(519,65);
			this.textDescription.TabIndex = 60;
			// 
			// textSubmitter
			// 
			this.textSubmitter.Location = new System.Drawing.Point(150,384);
			this.textSubmitter.Name = "textSubmitter";
			this.textSubmitter.ReadOnly = true;
			this.textSubmitter.Size = new System.Drawing.Size(171,20);
			this.textSubmitter.TabIndex = 61;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(375,39);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(26,13);
			this.label6.TabIndex = 62;
			this.label6.Text = "App";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6,22);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(75,13);
			this.label10.TabIndex = 64;
			this.label10.Text = "Version Found";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(11,48);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(70,13);
			this.label11.TabIndex = 65;
			this.label11.Text = "Version Fixed";
			// 
			// textEClipboardVersionFound
			// 
			this.textEClipboardVersionFound.Location = new System.Drawing.Point(88,19);
			this.textEClipboardVersionFound.Name = "textEClipboardVersionFound";
			this.textEClipboardVersionFound.Size = new System.Drawing.Size(171,20);
			this.textEClipboardVersionFound.TabIndex = 66;
			// 
			// groupBoxEClipboard
			// 
			this.groupBoxEClipboard.Controls.Add(this.butEClipboardNextVersion);
			this.groupBoxEClipboard.Controls.Add(this.butEClipboardLastVersion);
			this.groupBoxEClipboard.Controls.Add(this.textEClipboardVersionFixed);
			this.groupBoxEClipboard.Controls.Add(this.textEClipboardVersionFound);
			this.groupBoxEClipboard.Controls.Add(this.label11);
			this.groupBoxEClipboard.Controls.Add(this.label10);
			this.groupBoxEClipboard.Enabled = false;
			this.groupBoxEClipboard.Location = new System.Drawing.Point(62,141);
			this.groupBoxEClipboard.Name = "groupBoxEClipboard";
			this.groupBoxEClipboard.Size = new System.Drawing.Size(607,80);
			this.groupBoxEClipboard.TabIndex = 67;
			this.groupBoxEClipboard.TabStop = false;
			this.groupBoxEClipboard.Text = "eClipboard";
			// 
			// textEClipboardVersionFixed
			// 
			this.textEClipboardVersionFixed.Location = new System.Drawing.Point(88,45);
			this.textEClipboardVersionFixed.Name = "textEClipboardVersionFixed";
			this.textEClipboardVersionFixed.Size = new System.Drawing.Size(171,20);
			this.textEClipboardVersionFixed.TabIndex = 67;
			// 
			// groupBoxODMobile
			// 
			this.groupBoxODMobile.Controls.Add(this.butODMobileNextVersion);
			this.groupBoxODMobile.Controls.Add(this.butODMobileLastVersion);
			this.groupBoxODMobile.Controls.Add(this.textODMobileVersionFixed);
			this.groupBoxODMobile.Controls.Add(this.textODMobileVersionFound);
			this.groupBoxODMobile.Controls.Add(this.label12);
			this.groupBoxODMobile.Controls.Add(this.label13);
			this.groupBoxODMobile.Enabled = false;
			this.groupBoxODMobile.Location = new System.Drawing.Point(62,227);
			this.groupBoxODMobile.Name = "groupBoxODMobile";
			this.groupBoxODMobile.Size = new System.Drawing.Size(607,80);
			this.groupBoxODMobile.TabIndex = 68;
			this.groupBoxODMobile.TabStop = false;
			this.groupBoxODMobile.Text = "ODMobile";
			// 
			// textODMobileVersionFixed
			// 
			this.textODMobileVersionFixed.Location = new System.Drawing.Point(88,45);
			this.textODMobileVersionFixed.Name = "textODMobileVersionFixed";
			this.textODMobileVersionFixed.Size = new System.Drawing.Size(171,20);
			this.textODMobileVersionFixed.TabIndex = 67;
			// 
			// textODMobileVersionFound
			// 
			this.textODMobileVersionFound.Location = new System.Drawing.Point(88,19);
			this.textODMobileVersionFound.Name = "textODMobileVersionFound";
			this.textODMobileVersionFound.Size = new System.Drawing.Size(171,20);
			this.textODMobileVersionFound.TabIndex = 66;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(11,48);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(70,13);
			this.label12.TabIndex = 65;
			this.label12.Text = "Version Fixed";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6,22);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(75,13);
			this.label13.TabIndex = 64;
			this.label13.Text = "Version Found";
			// 
			// checkiOS
			// 
			this.checkiOS.AutoSize = true;
			this.checkiOS.Checked = true;
			this.checkiOS.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkiOS.Location = new System.Drawing.Point(407,12);
			this.checkiOS.Name = "checkiOS";
			this.checkiOS.Size = new System.Drawing.Size(43,17);
			this.checkiOS.TabIndex = 80;
			this.checkiOS.Text = "iOS";
			this.checkiOS.UseVisualStyleBackColor = true;
			// 
			// checkAndroid
			// 
			this.checkAndroid.AutoSize = true;
			this.checkAndroid.Checked = true;
			this.checkAndroid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAndroid.Location = new System.Drawing.Point(456,12);
			this.checkAndroid.Name = "checkAndroid";
			this.checkAndroid.Size = new System.Drawing.Size(62,17);
			this.checkAndroid.TabIndex = 81;
			this.checkAndroid.Text = "Android";
			this.checkAndroid.UseVisualStyleBackColor = true;
			// 
			// checkUWP
			// 
			this.checkUWP.AutoSize = true;
			this.checkUWP.Checked = true;
			this.checkUWP.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkUWP.Location = new System.Drawing.Point(524,12);
			this.checkUWP.Name = "checkUWP";
			this.checkUWP.Size = new System.Drawing.Size(52,17);
			this.checkUWP.TabIndex = 82;
			this.checkUWP.Text = "UWP";
			this.checkUWP.UseVisualStyleBackColor = true;
			// 
			// checkODMobile
			// 
			this.checkODMobile.AutoSize = true;
			this.checkODMobile.Location = new System.Drawing.Point(489,38);
			this.checkODMobile.Name = "checkODMobile";
			this.checkODMobile.Size = new System.Drawing.Size(73,17);
			this.checkODMobile.TabIndex = 83;
			this.checkODMobile.Text = "ODMobile";
			this.checkODMobile.UseVisualStyleBackColor = true;
			this.checkODMobile.CheckedChanged += new System.EventHandler(this.checkODMobile_CheckedChanged);
			// 
			// checkEClipboard
			// 
			this.checkEClipboard.AutoSize = true;
			this.checkEClipboard.Location = new System.Drawing.Point(407,38);
			this.checkEClipboard.Name = "checkEClipboard";
			this.checkEClipboard.Size = new System.Drawing.Size(76,17);
			this.checkEClipboard.TabIndex = 84;
			this.checkEClipboard.Text = "eClipboard";
			this.checkEClipboard.UseVisualStyleBackColor = true;
			this.checkEClipboard.CheckedChanged += new System.EventHandler(this.checkEClipboard_CheckedChanged);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12,413);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(65,23);
			this.butDelete.TabIndex = 78;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(626,413);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65,23);
			this.butCancel.TabIndex = 77;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(555,413);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(65,23);
			this.butOK.TabIndex = 76;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butLast3found
			// 
			this.butLast3found.Location = new System.Drawing.Point(455,87);
			this.butLast3found.Name = "butLast3found";
			this.butLast3found.Size = new System.Drawing.Size(58,23);
			this.butLast3found.TabIndex = 75;
			this.butLast3found.Tag = "3";
			this.butLast3found.Text = "Last 3";
			this.butLast3found.UseVisualStyleBackColor = true;
			this.butLast3found.Click += new System.EventHandler(this.butLast3found_Click);
			// 
			// butLast2found
			// 
			this.butLast2found.Location = new System.Drawing.Point(391,87);
			this.butLast2found.Name = "butLast2found";
			this.butLast2found.Size = new System.Drawing.Size(58,23);
			this.butLast2found.TabIndex = 74;
			this.butLast2found.Tag = "2";
			this.butLast2found.Text = "Last 2";
			this.butLast2found.UseVisualStyleBackColor = true;
			this.butLast2found.Click += new System.EventHandler(this.butLast2found_Click);
			// 
			// butLast1found
			// 
			this.butLast1found.Location = new System.Drawing.Point(327,87);
			this.butLast1found.Name = "butLast1found";
			this.butLast1found.Size = new System.Drawing.Size(58,23);
			this.butLast1found.TabIndex = 73;
			this.butLast1found.Tag = "1";
			this.butLast1found.Text = "Last 1";
			this.butLast1found.UseVisualStyleBackColor = true;
			this.butLast1found.Click += new System.EventHandler(this.butLast1found_Click);
			// 
			// butLast3
			// 
			this.butLast3.Location = new System.Drawing.Point(455,113);
			this.butLast3.Name = "butLast3";
			this.butLast3.Size = new System.Drawing.Size(58,23);
			this.butLast3.TabIndex = 72;
			this.butLast3.Tag = "3";
			this.butLast3.Text = "Last 3";
			this.butLast3.UseVisualStyleBackColor = true;
			this.butLast3.Click += new System.EventHandler(this.butLast3_Click);
			// 
			// butLast2
			// 
			this.butLast2.Location = new System.Drawing.Point(391,113);
			this.butLast2.Name = "butLast2";
			this.butLast2.Size = new System.Drawing.Size(58,23);
			this.butLast2.TabIndex = 71;
			this.butLast2.Tag = "2";
			this.butLast2.Text = "Last 2";
			this.butLast2.UseVisualStyleBackColor = true;
			this.butLast2.Click += new System.EventHandler(this.butLast2_Click);
			// 
			// butLast1
			// 
			this.butLast1.Location = new System.Drawing.Point(327,113);
			this.butLast1.Name = "butLast1";
			this.butLast1.Size = new System.Drawing.Size(58,23);
			this.butLast1.TabIndex = 70;
			this.butLast1.Tag = "1";
			this.butLast1.Text = "Last 1";
			this.butLast1.UseVisualStyleBackColor = true;
			this.butLast1.Click += new System.EventHandler(this.butLast1_Click);
			// 
			// butODVersionCopyDown
			// 
			this.butODVersionCopyDown.Location = new System.Drawing.Point(519,98);
			this.butODVersionCopyDown.Name = "butODVersionCopyDown";
			this.butODVersionCopyDown.Size = new System.Drawing.Size(75,23);
			this.butODVersionCopyDown.TabIndex = 69;
			this.butODVersionCopyDown.Text = "Copy down";
			this.butODVersionCopyDown.UseVisualStyleBackColor = true;
			this.butODVersionCopyDown.Click += new System.EventHandler(this.butODVersionCopyDown_Click);
			// 
			// butODMobileNextVersion
			// 
			this.butODMobileNextVersion.Location = new System.Drawing.Point(265,43);
			this.butODMobileNextVersion.Name = "butODMobileNextVersion";
			this.butODMobileNextVersion.Size = new System.Drawing.Size(74,23);
			this.butODMobileNextVersion.TabIndex = 76;
			this.butODMobileNextVersion.Tag = "1";
			this.butODMobileNextVersion.Text = "Next Version";
			this.butODMobileNextVersion.UseVisualStyleBackColor = true;
			this.butODMobileNextVersion.Click += new System.EventHandler(this.butODMobileNextVersion_Click);
			// 
			// butODMobileLastVersion
			// 
			this.butODMobileLastVersion.Location = new System.Drawing.Point(265,17);
			this.butODMobileLastVersion.Name = "butODMobileLastVersion";
			this.butODMobileLastVersion.Size = new System.Drawing.Size(74,23);
			this.butODMobileLastVersion.TabIndex = 75;
			this.butODMobileLastVersion.Tag = "1";
			this.butODMobileLastVersion.Text = "Last Version";
			this.butODMobileLastVersion.UseVisualStyleBackColor = true;
			this.butODMobileLastVersion.Click += new System.EventHandler(this.butODMobileLastVersion_Click);
			// 
			// butEClipboardNextVersion
			// 
			this.butEClipboardNextVersion.Location = new System.Drawing.Point(265,43);
			this.butEClipboardNextVersion.Name = "butEClipboardNextVersion";
			this.butEClipboardNextVersion.Size = new System.Drawing.Size(74,23);
			this.butEClipboardNextVersion.TabIndex = 75;
			this.butEClipboardNextVersion.Tag = "1";
			this.butEClipboardNextVersion.Text = "Next Version";
			this.butEClipboardNextVersion.UseVisualStyleBackColor = true;
			this.butEClipboardNextVersion.Click += new System.EventHandler(this.butEClipboardNextVersion_Click);
			// 
			// butEClipboardLastVersion
			// 
			this.butEClipboardLastVersion.Location = new System.Drawing.Point(265,17);
			this.butEClipboardLastVersion.Name = "butEClipboardLastVersion";
			this.butEClipboardLastVersion.Size = new System.Drawing.Size(74,23);
			this.butEClipboardLastVersion.TabIndex = 74;
			this.butEClipboardLastVersion.Tag = "1";
			this.butEClipboardLastVersion.Text = "Last Version";
			this.butEClipboardLastVersion.UseVisualStyleBackColor = true;
			this.butEClipboardLastVersion.Click += new System.EventHandler(this.butEClipboardLastVersion_Click);
			// 
			// FormMobileBugEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(703,448);
			this.Controls.Add(this.checkEClipboard);
			this.Controls.Add(this.checkODMobile);
			this.Controls.Add(this.checkUWP);
			this.Controls.Add(this.checkAndroid);
			this.Controls.Add(this.checkiOS);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butLast3found);
			this.Controls.Add(this.butLast2found);
			this.Controls.Add(this.butLast1found);
			this.Controls.Add(this.butLast3);
			this.Controls.Add(this.butLast2);
			this.Controls.Add(this.butLast1);
			this.Controls.Add(this.butODVersionCopyDown);
			this.Controls.Add(this.groupBoxODMobile);
			this.Controls.Add(this.groupBoxEClipboard);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textSubmitter);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textODVersionsFixed);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textODVersionsFound);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCreationDate);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textMobileBugNum);
			this.Controls.Add(this.label1);
			this.Name = "FormMobileBugEdit";
			this.Text = "Mobile Bug Edit";
			this.Load += new System.EventHandler(this.FormMobileBugEdit_Load);
			this.groupBoxEClipboard.ResumeLayout(false);
			this.groupBoxEClipboard.PerformLayout();
			this.groupBoxODMobile.ResumeLayout(false);
			this.groupBoxODMobile.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMobileBugNum;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textCreationDate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textODVersionsFound;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textODVersionsFixed;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.TextBox textSubmitter;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textEClipboardVersionFound;
		private System.Windows.Forms.GroupBox groupBoxEClipboard;
		private System.Windows.Forms.TextBox textEClipboardVersionFixed;
		private System.Windows.Forms.GroupBox groupBoxODMobile;
		private System.Windows.Forms.TextBox textODMobileVersionFixed;
		private System.Windows.Forms.TextBox textODMobileVersionFound;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private UI.Button butLast3found;
		private UI.Button butLast2found;
		private UI.Button butLast1found;
		private UI.Button butLast3;
		private UI.Button butLast2;
		private UI.Button butLast1;
		private UI.Button butODVersionCopyDown;
		private UI.Button butCancel;
		private UI.Button butOK;
		private UI.Button butDelete;
		private System.Windows.Forms.CheckBox checkiOS;
		private System.Windows.Forms.CheckBox checkAndroid;
		private System.Windows.Forms.CheckBox checkUWP;
		private System.Windows.Forms.CheckBox checkODMobile;
		private System.Windows.Forms.CheckBox checkEClipboard;
		private UI.Button butEClipboardNextVersion;
		private UI.Button butEClipboardLastVersion;
		private UI.Button butODMobileNextVersion;
		private UI.Button butODMobileLastVersion;
	}
}