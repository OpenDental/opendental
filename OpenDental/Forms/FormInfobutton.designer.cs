namespace OpenDental{
	partial class FormInfobutton {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfobutton));
			this.groupBoxContext = new System.Windows.Forms.GroupBox();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.textOrgID = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textOrgName = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.butProvSelect = new OpenDental.UI.Button();
			this.comboProvLang = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textProvID = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textProvName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.textEncLocID = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboEncType = new System.Windows.Forms.ComboBox();
			this.label26 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.butPatSelect = new OpenDental.UI.Button();
			this.comboPatLang = new System.Windows.Forms.ComboBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioPatGenUn = new System.Windows.Forms.RadioButton();
			this.radioPatGenFem = new System.Windows.Forms.RadioButton();
			this.radioPatGenMale = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.textPatBirth = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPatName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioRecPat = new System.Windows.Forms.RadioButton();
			this.radioRecProv = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioReqPat = new System.Windows.Forms.RadioButton();
			this.radioReqProv = new System.Windows.Forms.RadioButton();
			this.comboTask = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.butAddLoinc = new OpenDental.UI.Button();
			this.butAddIcd10 = new OpenDental.UI.Button();
			this.button3 = new OpenDental.UI.Button();
			this.butAddIcd9 = new OpenDental.UI.Button();
			this.butAddSnomed = new OpenDental.UI.Button();
			this.butAddAllergy = new OpenDental.UI.Button();
			this.butAddDisease = new OpenDental.UI.Button();
			this.butAddRxNorm = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butPreviewRequest = new OpenDental.UI.Button();
			this.butSend = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.groupBoxContext.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxContext
			// 
			this.groupBoxContext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxContext.Controls.Add(this.groupBox7);
			this.groupBoxContext.Controls.Add(this.groupBox6);
			this.groupBoxContext.Controls.Add(this.groupBox5);
			this.groupBoxContext.Controls.Add(this.groupBox3);
			this.groupBoxContext.Controls.Add(this.groupBox2);
			this.groupBoxContext.Controls.Add(this.groupBox1);
			this.groupBoxContext.Controls.Add(this.comboTask);
			this.groupBoxContext.Controls.Add(this.label17);
			this.groupBoxContext.Location = new System.Drawing.Point(12, 12);
			this.groupBoxContext.Name = "groupBoxContext";
			this.groupBoxContext.Size = new System.Drawing.Size(564, 337);
			this.groupBoxContext.TabIndex = 4;
			this.groupBoxContext.TabStop = false;
			this.groupBoxContext.Text = "Context";
			this.groupBoxContext.Enter += new System.EventHandler(this.groupBoxContext_Enter);
			// 
			// groupBox7
			// 
			this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox7.Controls.Add(this.textOrgID);
			this.groupBox7.Controls.Add(this.label9);
			this.groupBox7.Controls.Add(this.textOrgName);
			this.groupBox7.Controls.Add(this.label10);
			this.groupBox7.Location = new System.Drawing.Point(285, 115);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(273, 88);
			this.groupBox7.TabIndex = 171;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Organization";
			// 
			// textOrgID
			// 
			this.textOrgID.Location = new System.Drawing.Point(124, 38);
			this.textOrgID.Name = "textOrgID";
			this.textOrgID.Size = new System.Drawing.Size(137, 20);
			this.textOrgID.TabIndex = 167;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(27, 42);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(94, 16);
			this.label9.TabIndex = 168;
			this.label9.Text = "Organization ID";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textOrgName
			// 
			this.textOrgName.Location = new System.Drawing.Point(124, 15);
			this.textOrgName.Name = "textOrgName";
			this.textOrgName.Size = new System.Drawing.Size(137, 20);
			this.textOrgName.TabIndex = 165;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(27, 19);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(94, 16);
			this.label10.TabIndex = 166;
			this.label10.Text = "Name";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox6
			// 
			this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox6.Controls.Add(this.butProvSelect);
			this.groupBox6.Controls.Add(this.comboProvLang);
			this.groupBox6.Controls.Add(this.label7);
			this.groupBox6.Controls.Add(this.textProvID);
			this.groupBox6.Controls.Add(this.label6);
			this.groupBox6.Controls.Add(this.textProvName);
			this.groupBox6.Controls.Add(this.label5);
			this.groupBox6.Location = new System.Drawing.Point(8, 115);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(273, 88);
			this.groupBox6.TabIndex = 164;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Provider";
			// 
			// butProvSelect
			// 
			this.butProvSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProvSelect.Location = new System.Drawing.Point(9, 19);
			this.butProvSelect.Name = "butProvSelect";
			this.butProvSelect.Size = new System.Drawing.Size(29, 25);
			this.butProvSelect.TabIndex = 174;
			this.butProvSelect.Text = "...";
			// 
			// comboProvLang
			// 
			this.comboProvLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvLang.Location = new System.Drawing.Point(124, 61);
			this.comboProvLang.MaxDropDownItems = 30;
			this.comboProvLang.Name = "comboProvLang";
			this.comboProvLang.Size = new System.Drawing.Size(137, 21);
			this.comboProvLang.TabIndex = 171;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(27, 65);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(94, 16);
			this.label7.TabIndex = 170;
			this.label7.Text = "Language";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textProvID
			// 
			this.textProvID.Location = new System.Drawing.Point(124, 38);
			this.textProvID.Name = "textProvID";
			this.textProvID.Size = new System.Drawing.Size(137, 20);
			this.textProvID.TabIndex = 167;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(27, 42);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(94, 16);
			this.label6.TabIndex = 168;
			this.label6.Text = "Provider ID";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textProvName
			// 
			this.textProvName.Location = new System.Drawing.Point(124, 15);
			this.textProvName.Name = "textProvName";
			this.textProvName.Size = new System.Drawing.Size(137, 20);
			this.textProvName.TabIndex = 165;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(27, 19);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(94, 16);
			this.label5.TabIndex = 166;
			this.label5.Text = "Name";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.textEncLocID);
			this.groupBox5.Controls.Add(this.label8);
			this.groupBox5.Controls.Add(this.comboEncType);
			this.groupBox5.Controls.Add(this.label26);
			this.groupBox5.Location = new System.Drawing.Point(8, 203);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(550, 66);
			this.groupBox5.TabIndex = 163;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Encounter";
			// 
			// textEncLocID
			// 
			this.textEncLocID.Location = new System.Drawing.Point(170, 37);
			this.textEncLocID.Name = "textEncLocID";
			this.textEncLocID.Size = new System.Drawing.Size(137, 20);
			this.textEncLocID.TabIndex = 169;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 41);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(161, 16);
			this.label8.TabIndex = 170;
			this.label8.Text = "Service Delivery Location ID";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboEncType
			// 
			this.comboEncType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEncType.Location = new System.Drawing.Point(170, 13);
			this.comboEncType.MaxDropDownItems = 30;
			this.comboEncType.Name = "comboEncType";
			this.comboEncType.Size = new System.Drawing.Size(151, 21);
			this.comboEncType.TabIndex = 128;
			this.comboEncType.SelectedIndexChanged += new System.EventHandler(this.comboEncType_SelectedIndexChanged);
			// 
			// label26
			// 
			this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label26.Location = new System.Drawing.Point(6, 17);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(162, 14);
			this.label26.TabIndex = 129;
			this.label26.Text = "Type";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.butPatSelect);
			this.groupBox3.Controls.Add(this.comboPatLang);
			this.groupBox3.Controls.Add(this.groupBox4);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.textPatBirth);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.textPatName);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Location = new System.Drawing.Point(8, 19);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(550, 96);
			this.groupBox3.TabIndex = 163;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Patient";
			// 
			// butPatSelect
			// 
			this.butPatSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPatSelect.Location = new System.Drawing.Point(9, 19);
			this.butPatSelect.Name = "butPatSelect";
			this.butPatSelect.Size = new System.Drawing.Size(29, 25);
			this.butPatSelect.TabIndex = 173;
			this.butPatSelect.Text = "...";
			// 
			// comboPatLang
			// 
			this.comboPatLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPatLang.Location = new System.Drawing.Point(124, 64);
			this.comboPatLang.MaxDropDownItems = 30;
			this.comboPatLang.Name = "comboPatLang";
			this.comboPatLang.Size = new System.Drawing.Size(197, 21);
			this.comboPatLang.TabIndex = 172;
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.radioPatGenUn);
			this.groupBox4.Controls.Add(this.radioPatGenFem);
			this.groupBox4.Controls.Add(this.radioPatGenMale);
			this.groupBox4.Location = new System.Drawing.Point(327, 11);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(217, 74);
			this.groupBox4.TabIndex = 162;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Administrative Gender";
			// 
			// radioPatGenUn
			// 
			this.radioPatGenUn.Location = new System.Drawing.Point(6, 51);
			this.radioPatGenUn.Name = "radioPatGenUn";
			this.radioPatGenUn.Size = new System.Drawing.Size(205, 17);
			this.radioPatGenUn.TabIndex = 162;
			this.radioPatGenUn.Text = "Undifferentiated";
			this.radioPatGenUn.UseVisualStyleBackColor = true;
			// 
			// radioPatGenFem
			// 
			this.radioPatGenFem.Checked = true;
			this.radioPatGenFem.Location = new System.Drawing.Point(6, 15);
			this.radioPatGenFem.Name = "radioPatGenFem";
			this.radioPatGenFem.Size = new System.Drawing.Size(205, 17);
			this.radioPatGenFem.TabIndex = 161;
			this.radioPatGenFem.TabStop = true;
			this.radioPatGenFem.Text = "Female";
			this.radioPatGenFem.UseVisualStyleBackColor = true;
			// 
			// radioPatGenMale
			// 
			this.radioPatGenMale.Location = new System.Drawing.Point(6, 32);
			this.radioPatGenMale.Name = "radioPatGenMale";
			this.radioPatGenMale.Size = new System.Drawing.Size(205, 17);
			this.radioPatGenMale.TabIndex = 160;
			this.radioPatGenMale.Text = "Male";
			this.radioPatGenMale.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(27, 69);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(94, 16);
			this.label4.TabIndex = 168;
			this.label4.Text = "Language";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPatBirth
			// 
			this.textPatBirth.Location = new System.Drawing.Point(124, 42);
			this.textPatBirth.Name = "textPatBirth";
			this.textPatBirth.Size = new System.Drawing.Size(197, 20);
			this.textPatBirth.TabIndex = 165;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(27, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(94, 16);
			this.label3.TabIndex = 166;
			this.label3.Text = "Birthday";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPatName
			// 
			this.textPatName.Location = new System.Drawing.Point(124, 19);
			this.textPatName.Name = "textPatName";
			this.textPatName.Size = new System.Drawing.Size(197, 20);
			this.textPatName.TabIndex = 163;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(27, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 16);
			this.label1.TabIndex = 164;
			this.label1.Text = "Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioRecPat);
			this.groupBox2.Controls.Add(this.radioRecProv);
			this.groupBox2.Location = new System.Drawing.Point(150, 275);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(136, 54);
			this.groupBox2.TabIndex = 162;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Recipient";
			// 
			// radioRecPat
			// 
			this.radioRecPat.Checked = true;
			this.radioRecPat.Location = new System.Drawing.Point(6, 15);
			this.radioRecPat.Name = "radioRecPat";
			this.radioRecPat.Size = new System.Drawing.Size(124, 17);
			this.radioRecPat.TabIndex = 161;
			this.radioRecPat.TabStop = true;
			this.radioRecPat.Text = "Patient";
			this.radioRecPat.UseVisualStyleBackColor = true;
			// 
			// radioRecProv
			// 
			this.radioRecProv.Location = new System.Drawing.Point(6, 32);
			this.radioRecProv.Name = "radioRecProv";
			this.radioRecProv.Size = new System.Drawing.Size(124, 17);
			this.radioRecProv.TabIndex = 160;
			this.radioRecProv.Text = "Provider";
			this.radioRecProv.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioReqPat);
			this.groupBox1.Controls.Add(this.radioReqProv);
			this.groupBox1.Location = new System.Drawing.Point(8, 275);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(136, 54);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Requestor";
			// 
			// radioReqPat
			// 
			this.radioReqPat.Checked = true;
			this.radioReqPat.Location = new System.Drawing.Point(6, 15);
			this.radioReqPat.Name = "radioReqPat";
			this.radioReqPat.Size = new System.Drawing.Size(124, 17);
			this.radioReqPat.TabIndex = 161;
			this.radioReqPat.TabStop = true;
			this.radioReqPat.Text = "Patient";
			this.radioReqPat.UseVisualStyleBackColor = true;
			// 
			// radioReqProv
			// 
			this.radioReqProv.Location = new System.Drawing.Point(6, 32);
			this.radioReqProv.Name = "radioReqProv";
			this.radioReqProv.Size = new System.Drawing.Size(124, 17);
			this.radioReqProv.TabIndex = 160;
			this.radioReqProv.Text = "Provider";
			this.radioReqProv.UseVisualStyleBackColor = true;
			// 
			// comboTask
			// 
			this.comboTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboTask.Location = new System.Drawing.Point(399, 280);
			this.comboTask.MaxDropDownItems = 30;
			this.comboTask.Name = "comboTask";
			this.comboTask.Size = new System.Drawing.Size(159, 21);
			this.comboTask.TabIndex = 128;
			this.comboTask.SelectedIndexChanged += new System.EventHandler(this.comboTask_SelectedIndexChanged);
			// 
			// label17
			// 
			this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.Location = new System.Drawing.Point(317, 282);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(76, 14);
			this.label17.TabIndex = 129;
			this.label17.Text = "Task Code";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAddLoinc
			// 
			this.butAddLoinc.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddLoinc.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddLoinc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddLoinc.Location = new System.Drawing.Point(311, 384);
			this.butAddLoinc.Name = "butAddLoinc";
			this.butAddLoinc.Size = new System.Drawing.Size(94, 23);
			this.butAddLoinc.TabIndex = 207;
			this.butAddLoinc.Text = "Loinc";
			this.butAddLoinc.Click += new System.EventHandler(this.butAddLoinc_Click);
			// 
			// butAddIcd10
			// 
			this.butAddIcd10.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddIcd10.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddIcd10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddIcd10.Location = new System.Drawing.Point(212, 384);
			this.butAddIcd10.Name = "butAddIcd10";
			this.butAddIcd10.Size = new System.Drawing.Size(94, 23);
			this.butAddIcd10.TabIndex = 206;
			this.butAddIcd10.Text = "Icd10";
			this.butAddIcd10.Click += new System.EventHandler(this.butAddIcd10_Click);
			// 
			// button3
			// 
			this.button3.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.button3.Icon = OpenDental.UI.EnumIcons.Add;
			this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button3.Location = new System.Drawing.Point(312, 355);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(94, 23);
			this.button3.TabIndex = 205;
			this.button3.Text = "Lab Result";
			this.button3.Visible = false;
			// 
			// butAddIcd9
			// 
			this.butAddIcd9.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddIcd9.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddIcd9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddIcd9.Location = new System.Drawing.Point(112, 384);
			this.butAddIcd9.Name = "butAddIcd9";
			this.butAddIcd9.Size = new System.Drawing.Size(94, 23);
			this.butAddIcd9.TabIndex = 204;
			this.butAddIcd9.Text = "Icd9";
			this.butAddIcd9.Click += new System.EventHandler(this.butAddIcd9_Click);
			// 
			// butAddSnomed
			// 
			this.butAddSnomed.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddSnomed.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSnomed.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSnomed.Location = new System.Drawing.Point(12, 384);
			this.butAddSnomed.Name = "butAddSnomed";
			this.butAddSnomed.Size = new System.Drawing.Size(96, 23);
			this.butAddSnomed.TabIndex = 203;
			this.butAddSnomed.Text = "SNOMEDCT";
			this.butAddSnomed.Click += new System.EventHandler(this.butAddSnomed_Click);
			// 
			// butAddAllergy
			// 
			this.butAddAllergy.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddAllergy.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddAllergy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAllergy.Location = new System.Drawing.Point(212, 355);
			this.butAddAllergy.Name = "butAddAllergy";
			this.butAddAllergy.Size = new System.Drawing.Size(94, 23);
			this.butAddAllergy.TabIndex = 202;
			this.butAddAllergy.Text = "Allergy";
			this.butAddAllergy.Click += new System.EventHandler(this.butAddAllergy_Click);
			// 
			// butAddDisease
			// 
			this.butAddDisease.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddDisease.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddDisease.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddDisease.Location = new System.Drawing.Point(12, 355);
			this.butAddDisease.Name = "butAddDisease";
			this.butAddDisease.Size = new System.Drawing.Size(94, 23);
			this.butAddDisease.TabIndex = 200;
			this.butAddDisease.Text = "Problem";
			this.butAddDisease.Click += new System.EventHandler(this.butAddDisease_Click);
			// 
			// butAddRxNorm
			// 
			this.butAddRxNorm.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAddRxNorm.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddRxNorm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddRxNorm.Location = new System.Drawing.Point(112, 355);
			this.butAddRxNorm.Name = "butAddRxNorm";
			this.butAddRxNorm.Size = new System.Drawing.Size(94, 23);
			this.butAddRxNorm.TabIndex = 201;
			this.butAddRxNorm.Text = "RxNorm";
			this.butAddRxNorm.Click += new System.EventHandler(this.butAddRxNorm_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 413);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(564, 132);
			this.gridMain.TabIndex = 199;
			this.gridMain.Title = "Knowledge Request Items";
			this.gridMain.WrapText = false;
			// 
			// butPreviewRequest
			// 
			this.butPreviewRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPreviewRequest.Location = new System.Drawing.Point(214, 553);
			this.butPreviewRequest.Name = "butPreviewRequest";
			this.butPreviewRequest.Size = new System.Drawing.Size(127, 24);
			this.butPreviewRequest.TabIndex = 198;
			this.butPreviewRequest.Text = "Preview Request";
			this.butPreviewRequest.Click += new System.EventHandler(this.butPreviewRequest_Click);
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(420, 553);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 3;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(501, 553);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPreview.Location = new System.Drawing.Point(12, 553);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(127, 24);
			this.butPreview.TabIndex = 8;
			this.butPreview.Text = "&Preview XML";
			this.butPreview.Visible = false;
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// FormInfobutton
			// 
			this.ClientSize = new System.Drawing.Size(588, 584);
			this.Controls.Add(this.butAddLoinc);
			this.Controls.Add(this.butAddIcd10);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.butAddIcd9);
			this.Controls.Add(this.butAddSnomed);
			this.Controls.Add(this.butAddAllergy);
			this.Controls.Add(this.butAddDisease);
			this.Controls.Add(this.butAddRxNorm);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butPreviewRequest);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.groupBoxContext);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInfobutton";
			this.Text = "InfoButton Portal";
			this.Load += new System.EventHandler(this.FormInfobutton_Load);
			this.groupBoxContext.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSend;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupBoxContext;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioReqPat;
		private System.Windows.Forms.RadioButton radioReqProv;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioRecPat;
		private System.Windows.Forms.RadioButton radioRecProv;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.TextBox textOrgID;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textOrgName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textProvID;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textProvName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioPatGenUn;
		private System.Windows.Forms.RadioButton radioPatGenFem;
		private System.Windows.Forms.RadioButton radioPatGenMale;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textPatBirth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPatName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textEncLocID;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboEncType;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.ComboBox comboProvLang;
		private System.Windows.Forms.ComboBox comboPatLang;
		private System.Windows.Forms.ComboBox comboTask;
		private System.Windows.Forms.Label label17;
		private UI.Button butPreviewRequest;
		private UI.GridOD gridMain;
		private UI.Button butAddDisease;
		private UI.Button butAddRxNorm;
		private UI.Button butAddAllergy;
		private UI.Button butPatSelect;
		private UI.Button butAddSnomed;
		private UI.Button butAddIcd9;
		private UI.Button button3;
		private UI.Button butAddIcd10;
		private UI.Button butAddLoinc;
		private UI.Button butProvSelect;
		private UI.Button butPreview;
	}
}