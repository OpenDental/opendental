namespace OpenDental{
	partial class FormHL7DefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHL7DefEdit));
			this.label15 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.labelOutSocketOrDirEx = new System.Windows.Forms.Label();
			this.labelInSocketEx = new System.Windows.Forms.Label();
			this.textInternalTypeVersion = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textInternalType = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.textEscChar = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.checkInternal = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textSubcompSep = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textRepSep = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textCompSep = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textFieldSep = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.comboModeTx = new System.Windows.Forms.ComboBox();
			this.textSftpPassword = new System.Windows.Forms.TextBox();
			this.labelSftpPassword = new System.Windows.Forms.Label();
			this.textSftpUsername = new System.Windows.Forms.TextBox();
			this.labelSftpUsername = new System.Windows.Forms.Label();
			this.textOutPathSocketOrDir = new System.Windows.Forms.TextBox();
			this.labelOutPathSocketOrDir = new System.Windows.Forms.Label();
			this.textInPathOrSocket = new System.Windows.Forms.TextBox();
			this.labelInPathOrSocket = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelDelete = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.textHL7ServiceName = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textHL7Server = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.groupShowDemographics = new System.Windows.Forms.GroupBox();
			this.label20 = new System.Windows.Forms.Label();
			this.radioChangeAndAdd = new System.Windows.Forms.RadioButton();
			this.radioChange = new System.Windows.Forms.RadioButton();
			this.radioShow = new System.Windows.Forms.RadioButton();
			this.radioHide = new System.Windows.Forms.RadioButton();
			this.checkShowAccount = new System.Windows.Forms.CheckBox();
			this.checkShowAppts = new System.Windows.Forms.CheckBox();
			this.checkQuadAsToothNum = new System.Windows.Forms.CheckBox();
			this.labelLabImageCat = new System.Windows.Forms.Label();
			this.comboLabImageCat = new System.Windows.Forms.ComboBox();
			this.groupDelimeters = new System.Windows.Forms.GroupBox();
			this.groupHL7Comm = new System.Windows.Forms.GroupBox();
			this.butBrowseOut = new OpenDental.UI.Button();
			this.butBrowseIn = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkLongDCodes = new System.Windows.Forms.CheckBox();
			this.checkProcsAppt = new System.Windows.Forms.CheckBox();
			this.groupShowDemographics.SuspendLayout();
			this.groupDelimeters.SuspendLayout();
			this.groupHL7Comm.SuspendLayout();
			this.SuspendLayout();
			// 
			// label15
			// 
			this.label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label15.Location = new System.Drawing.Point(166, 93);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(91, 18);
			this.label15.TabIndex = 0;
			this.label15.Text = "Default: \\";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label6.Location = new System.Drawing.Point(166, 73);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(91, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Default: &";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label6.UseMnemonic = false;
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label5.Location = new System.Drawing.Point(166, 53);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(91, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Default: ^";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label4.Location = new System.Drawing.Point(166, 33);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(91, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Default: ~";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label3.Location = new System.Drawing.Point(166, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "Default: |";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(443, 184);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(368, 56);
			this.textNote.TabIndex = 62;
			// 
			// labelOutSocketOrDirEx
			// 
			this.labelOutSocketOrDirEx.Location = new System.Drawing.Point(336, 39);
			this.labelOutSocketOrDirEx.Name = "labelOutSocketOrDirEx";
			this.labelOutSocketOrDirEx.Size = new System.Drawing.Size(182, 18);
			this.labelOutSocketOrDirEx.TabIndex = 0;
			this.labelOutSocketOrDirEx.Text = "Ex: 192.168.0.23:5846";
			this.labelOutSocketOrDirEx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelOutSocketOrDirEx.Visible = false;
			// 
			// labelInSocketEx
			// 
			this.labelInSocketEx.Location = new System.Drawing.Point(336, 17);
			this.labelInSocketEx.Name = "labelInSocketEx";
			this.labelInSocketEx.Size = new System.Drawing.Size(182, 18);
			this.labelInSocketEx.TabIndex = 0;
			this.labelInSocketEx.Text = "Ex: server.address.com:12345";
			this.labelInSocketEx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelInSocketEx.Visible = false;
			// 
			// textInternalTypeVersion
			// 
			this.textInternalTypeVersion.Location = new System.Drawing.Point(155, 112);
			this.textInternalTypeVersion.Name = "textInternalTypeVersion";
			this.textInternalTypeVersion.ReadOnly = true;
			this.textInternalTypeVersion.Size = new System.Drawing.Size(125, 20);
			this.textInternalTypeVersion.TabIndex = 44;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(6, 113);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(148, 18);
			this.label13.TabIndex = 0;
			this.label13.Text = "Internal Type Version";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInternalType
			// 
			this.textInternalType.Location = new System.Drawing.Point(155, 92);
			this.textInternalType.Name = "textInternalType";
			this.textInternalType.ReadOnly = true;
			this.textInternalType.Size = new System.Drawing.Size(125, 20);
			this.textInternalType.TabIndex = 43;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(6, 93);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(148, 18);
			this.label14.TabIndex = 0;
			this.label14.Text = "Internal Type";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEnabled
			// 
			this.checkEnabled.Checked = true;
			this.checkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(6, 30);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEnabled.Size = new System.Drawing.Size(162, 18);
			this.checkEnabled.TabIndex = 40;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.CheckedChanged += new System.EventHandler(this.checkEnabled_CheckedChanged);
			this.checkEnabled.Click += new System.EventHandler(this.checkEnabled_Click);
			// 
			// textEscChar
			// 
			this.textEscChar.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.textEscChar.Location = new System.Drawing.Point(138, 92);
			this.textEscChar.Name = "textEscChar";
			this.textEscChar.Size = new System.Drawing.Size(27, 20);
			this.textEscChar.TabIndex = 49;
			// 
			// label12
			// 
			this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label12.Location = new System.Drawing.Point(6, 93);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(131, 18);
			this.label12.TabIndex = 0;
			this.label12.Text = "Escape";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInternal
			// 
			this.checkInternal.Enabled = false;
			this.checkInternal.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInternal.Location = new System.Drawing.Point(6, 12);
			this.checkInternal.Name = "checkInternal";
			this.checkInternal.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkInternal.Size = new System.Drawing.Size(162, 18);
			this.checkInternal.TabIndex = 31;
			this.checkInternal.TabStop = false;
			this.checkInternal.Text = "Internal";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(292, 185);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(150, 18);
			this.label11.TabIndex = 0;
			this.label11.Text = "Note";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubcompSep
			// 
			this.textSubcompSep.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.textSubcompSep.Location = new System.Drawing.Point(138, 72);
			this.textSubcompSep.Name = "textSubcompSep";
			this.textSubcompSep.Size = new System.Drawing.Size(27, 20);
			this.textSubcompSep.TabIndex = 48;
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label9.Location = new System.Drawing.Point(6, 73);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(131, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "Subcomponent";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRepSep
			// 
			this.textRepSep.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.textRepSep.Location = new System.Drawing.Point(138, 32);
			this.textRepSep.Name = "textRepSep";
			this.textRepSep.Size = new System.Drawing.Size(27, 20);
			this.textRepSep.TabIndex = 46;
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label10.Location = new System.Drawing.Point(6, 33);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(131, 18);
			this.label10.TabIndex = 0;
			this.label10.Text = "Repetition";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCompSep
			// 
			this.textCompSep.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.textCompSep.Location = new System.Drawing.Point(138, 52);
			this.textCompSep.Name = "textCompSep";
			this.textCompSep.Size = new System.Drawing.Size(27, 20);
			this.textCompSep.TabIndex = 47;
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label8.Location = new System.Drawing.Point(6, 53);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(131, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "Component";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFieldSep
			// 
			this.textFieldSep.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.textFieldSep.Location = new System.Drawing.Point(138, 12);
			this.textFieldSep.Name = "textFieldSep";
			this.textFieldSep.Size = new System.Drawing.Size(27, 20);
			this.textFieldSep.TabIndex = 45;
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label7.Location = new System.Drawing.Point(6, 13);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(131, 18);
			this.label7.TabIndex = 0;
			this.label7.Text = "Field";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboModeTx
			// 
			this.comboModeTx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboModeTx.Location = new System.Drawing.Point(155, 71);
			this.comboModeTx.MaxDropDownItems = 100;
			this.comboModeTx.Name = "comboModeTx";
			this.comboModeTx.Size = new System.Drawing.Size(125, 21);
			this.comboModeTx.TabIndex = 42;
			this.comboModeTx.SelectedIndexChanged += new System.EventHandler(this.comboModeTx_SelectedIndexChanged);
			// 
			// textSftpPassword
			// 
			this.textSftpPassword.Location = new System.Drawing.Point(157, 78);
			this.textSftpPassword.Name = "textSftpPassword";
			this.textSftpPassword.PasswordChar = '*';
			this.textSftpPassword.Size = new System.Drawing.Size(125, 20);
			this.textSftpPassword.TabIndex = 58;
			// 
			// labelSftpPassword
			// 
			this.labelSftpPassword.Location = new System.Drawing.Point(6, 79);
			this.labelSftpPassword.Name = "labelSftpPassword";
			this.labelSftpPassword.Size = new System.Drawing.Size(150, 18);
			this.labelSftpPassword.TabIndex = 0;
			this.labelSftpPassword.Text = "Sftp Password";
			this.labelSftpPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSftpUsername
			// 
			this.textSftpUsername.Location = new System.Drawing.Point(157, 58);
			this.textSftpUsername.Name = "textSftpUsername";
			this.textSftpUsername.Size = new System.Drawing.Size(125, 20);
			this.textSftpUsername.TabIndex = 57;
			// 
			// labelSftpUsername
			// 
			this.labelSftpUsername.Location = new System.Drawing.Point(6, 59);
			this.labelSftpUsername.Name = "labelSftpUsername";
			this.labelSftpUsername.Size = new System.Drawing.Size(150, 18);
			this.labelSftpUsername.TabIndex = 0;
			this.labelSftpUsername.Text = "Sftp Username";
			this.labelSftpUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOutPathSocketOrDir
			// 
			this.textOutPathSocketOrDir.Location = new System.Drawing.Point(157, 38);
			this.textOutPathSocketOrDir.Name = "textOutPathSocketOrDir";
			this.textOutPathSocketOrDir.Size = new System.Drawing.Size(177, 20);
			this.textOutPathSocketOrDir.TabIndex = 55;
			// 
			// labelOutPathSocketOrDir
			// 
			this.labelOutPathSocketOrDir.Location = new System.Drawing.Point(6, 39);
			this.labelOutPathSocketOrDir.Name = "labelOutPathSocketOrDir";
			this.labelOutPathSocketOrDir.Size = new System.Drawing.Size(150, 18);
			this.labelOutPathSocketOrDir.TabIndex = 0;
			this.labelOutPathSocketOrDir.Text = "Outgoing Folder";
			this.labelOutPathSocketOrDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInPathOrSocket
			// 
			this.textInPathOrSocket.Location = new System.Drawing.Point(157, 16);
			this.textInPathOrSocket.Name = "textInPathOrSocket";
			this.textInPathOrSocket.Size = new System.Drawing.Size(177, 20);
			this.textInPathOrSocket.TabIndex = 53;
			// 
			// labelInPathOrSocket
			// 
			this.labelInPathOrSocket.Location = new System.Drawing.Point(6, 17);
			this.labelInPathOrSocket.Name = "labelInPathOrSocket";
			this.labelInPathOrSocket.Size = new System.Drawing.Size(150, 18);
			this.labelInPathOrSocket.TabIndex = 0;
			this.labelInPathOrSocket.Text = "Incoming Folder";
			this.labelInPathOrSocket.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(148, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "ModeTx";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(155, 50);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(125, 20);
			this.textDescription.TabIndex = 41;
			// 
			// labelDelete
			// 
			this.labelDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDelete.Location = new System.Drawing.Point(108, 654);
			this.labelDelete.Name = "labelDelete";
			this.labelDelete.Size = new System.Drawing.Size(266, 28);
			this.labelDelete.TabIndex = 0;
			this.labelDelete.Text = "This HL7Def is internal. To edit this HL7Def you must first copy it to the Custom" +
    " list.";
			this.labelDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelDelete.Visible = false;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(570, 168);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(205, 16);
			this.label16.TabIndex = 0;
			this.label16.Text = "--Typically OpenDentalHL7.";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(570, 143);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(205, 26);
			this.label17.TabIndex = 0;
			this.label17.Text = "--The computer name (not IP) where\r\n     the HL7 Service is running.";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textHL7ServiceName
			// 
			this.textHL7ServiceName.Location = new System.Drawing.Point(443, 164);
			this.textHL7ServiceName.Name = "textHL7ServiceName";
			this.textHL7ServiceName.Size = new System.Drawing.Size(125, 20);
			this.textHL7ServiceName.TabIndex = 61;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(292, 165);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(150, 18);
			this.label18.TabIndex = 0;
			this.label18.Text = "HL7 Service Name";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHL7Server
			// 
			this.textHL7Server.Location = new System.Drawing.Point(443, 144);
			this.textHL7Server.Name = "textHL7Server";
			this.textHL7Server.Size = new System.Drawing.Size(125, 20);
			this.textHL7Server.TabIndex = 60;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(292, 145);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(150, 18);
			this.label19.TabIndex = 0;
			this.label19.Text = "OpenDental HL7 Server";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupShowDemographics
			// 
			this.groupShowDemographics.Controls.Add(this.label20);
			this.groupShowDemographics.Controls.Add(this.radioChangeAndAdd);
			this.groupShowDemographics.Controls.Add(this.radioChange);
			this.groupShowDemographics.Controls.Add(this.radioShow);
			this.groupShowDemographics.Controls.Add(this.radioHide);
			this.groupShowDemographics.Location = new System.Drawing.Point(286, 242);
			this.groupShowDemographics.Name = "groupShowDemographics";
			this.groupShowDemographics.Size = new System.Drawing.Size(525, 85);
			this.groupShowDemographics.TabIndex = 0;
			this.groupShowDemographics.TabStop = false;
			this.groupShowDemographics.Text = "Show Demographics (Address, etc.)";
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(172, 50);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(295, 31);
			this.label20.TabIndex = 0;
			this.label20.Text = "Changes to patient demographic information might get overwritten by incoming HL7 " +
    "messages.";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioChangeAndAdd
			// 
			this.radioChangeAndAdd.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioChangeAndAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioChangeAndAdd.Location = new System.Drawing.Point(9, 66);
			this.radioChangeAndAdd.Name = "radioChangeAndAdd";
			this.radioChangeAndAdd.Size = new System.Drawing.Size(162, 16);
			this.radioChangeAndAdd.TabIndex = 66;
			this.radioChangeAndAdd.TabStop = true;
			this.radioChangeAndAdd.Text = "Change and Add Pts";
			this.radioChangeAndAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioChangeAndAdd.UseVisualStyleBackColor = true;
			this.radioChangeAndAdd.Click += new System.EventHandler(this.RadioAddPts_Click);
			// 
			// radioChange
			// 
			this.radioChange.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioChange.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioChange.Location = new System.Drawing.Point(9, 49);
			this.radioChange.Name = "radioChange";
			this.radioChange.Size = new System.Drawing.Size(162, 16);
			this.radioChange.TabIndex = 65;
			this.radioChange.TabStop = true;
			this.radioChange.Text = "Change";
			this.radioChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioChange.UseVisualStyleBackColor = true;
			// 
			// radioShow
			// 
			this.radioShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioShow.Location = new System.Drawing.Point(9, 32);
			this.radioShow.Name = "radioShow";
			this.radioShow.Size = new System.Drawing.Size(162, 16);
			this.radioShow.TabIndex = 64;
			this.radioShow.TabStop = true;
			this.radioShow.Text = "Show";
			this.radioShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioShow.UseVisualStyleBackColor = true;
			// 
			// radioHide
			// 
			this.radioHide.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioHide.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioHide.Location = new System.Drawing.Point(9, 15);
			this.radioHide.Name = "radioHide";
			this.radioHide.Size = new System.Drawing.Size(162, 16);
			this.radioHide.TabIndex = 63;
			this.radioHide.TabStop = true;
			this.radioHide.Text = "Hide";
			this.radioHide.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowAccount
			// 
			this.checkShowAccount.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowAccount.Location = new System.Drawing.Point(63, 272);
			this.checkShowAccount.Name = "checkShowAccount";
			this.checkShowAccount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkShowAccount.Size = new System.Drawing.Size(162, 18);
			this.checkShowAccount.TabIndex = 51;
			this.checkShowAccount.Text = "Show Account Module";
			// 
			// checkShowAppts
			// 
			this.checkShowAppts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowAppts.Location = new System.Drawing.Point(63, 255);
			this.checkShowAppts.Name = "checkShowAppts";
			this.checkShowAppts.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkShowAppts.Size = new System.Drawing.Size(162, 18);
			this.checkShowAppts.TabIndex = 50;
			this.checkShowAppts.Text = "Show Appts Module";
			// 
			// checkQuadAsToothNum
			// 
			this.checkQuadAsToothNum.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkQuadAsToothNum.Location = new System.Drawing.Point(63, 289);
			this.checkQuadAsToothNum.Name = "checkQuadAsToothNum";
			this.checkQuadAsToothNum.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkQuadAsToothNum.Size = new System.Drawing.Size(162, 18);
			this.checkQuadAsToothNum.TabIndex = 52;
			this.checkQuadAsToothNum.Text = "Send Quad as Tooth Num";
			// 
			// labelLabImageCat
			// 
			this.labelLabImageCat.Location = new System.Drawing.Point(6, 100);
			this.labelLabImageCat.Name = "labelLabImageCat";
			this.labelLabImageCat.Size = new System.Drawing.Size(150, 18);
			this.labelLabImageCat.TabIndex = 0;
			this.labelLabImageCat.Text = "Lab Result Image Category";
			this.labelLabImageCat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLabImageCat
			// 
			this.comboLabImageCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLabImageCat.Location = new System.Drawing.Point(157, 99);
			this.comboLabImageCat.MaxDropDownItems = 100;
			this.comboLabImageCat.Name = "comboLabImageCat";
			this.comboLabImageCat.Size = new System.Drawing.Size(125, 21);
			this.comboLabImageCat.TabIndex = 59;
			// 
			// groupDelimeters
			// 
			this.groupDelimeters.Controls.Add(this.textFieldSep);
			this.groupDelimeters.Controls.Add(this.label7);
			this.groupDelimeters.Controls.Add(this.label8);
			this.groupDelimeters.Controls.Add(this.textCompSep);
			this.groupDelimeters.Controls.Add(this.label10);
			this.groupDelimeters.Controls.Add(this.textRepSep);
			this.groupDelimeters.Controls.Add(this.label9);
			this.groupDelimeters.Controls.Add(this.textSubcompSep);
			this.groupDelimeters.Controls.Add(this.label12);
			this.groupDelimeters.Controls.Add(this.textEscChar);
			this.groupDelimeters.Controls.Add(this.label3);
			this.groupDelimeters.Controls.Add(this.label4);
			this.groupDelimeters.Controls.Add(this.label5);
			this.groupDelimeters.Controls.Add(this.label6);
			this.groupDelimeters.Controls.Add(this.label15);
			this.groupDelimeters.Location = new System.Drawing.Point(17, 132);
			this.groupDelimeters.Name = "groupDelimeters";
			this.groupDelimeters.Size = new System.Drawing.Size(263, 118);
			this.groupDelimeters.TabIndex = 0;
			this.groupDelimeters.TabStop = false;
			this.groupDelimeters.Text = "Delimeters";
			// 
			// groupHL7Comm
			// 
			this.groupHL7Comm.Controls.Add(this.textSftpPassword);
			this.groupHL7Comm.Controls.Add(this.labelLabImageCat);
			this.groupHL7Comm.Controls.Add(this.textSftpUsername);
			this.groupHL7Comm.Controls.Add(this.labelSftpPassword);
			this.groupHL7Comm.Controls.Add(this.textOutPathSocketOrDir);
			this.groupHL7Comm.Controls.Add(this.labelSftpUsername);
			this.groupHL7Comm.Controls.Add(this.textInPathOrSocket);
			this.groupHL7Comm.Controls.Add(this.labelOutPathSocketOrDir);
			this.groupHL7Comm.Controls.Add(this.butBrowseOut);
			this.groupHL7Comm.Controls.Add(this.labelInPathOrSocket);
			this.groupHL7Comm.Controls.Add(this.butBrowseIn);
			this.groupHL7Comm.Controls.Add(this.comboLabImageCat);
			this.groupHL7Comm.Controls.Add(this.labelOutSocketOrDirEx);
			this.groupHL7Comm.Controls.Add(this.labelInSocketEx);
			this.groupHL7Comm.Location = new System.Drawing.Point(286, 12);
			this.groupHL7Comm.Name = "groupHL7Comm";
			this.groupHL7Comm.Size = new System.Drawing.Size(525, 126);
			this.groupHL7Comm.TabIndex = 0;
			this.groupHL7Comm.TabStop = false;
			this.groupHL7Comm.Text = "HL7 Communication Options";
			// 
			// butBrowseOut
			// 
			this.butBrowseOut.Location = new System.Drawing.Point(340, 38);
			this.butBrowseOut.Name = "butBrowseOut";
			this.butBrowseOut.Size = new System.Drawing.Size(76, 24);
			this.butBrowseOut.TabIndex = 56;
			this.butBrowseOut.Text = "&Browse";
			this.butBrowseOut.Click += new System.EventHandler(this.butBrowseOut_Click);
			// 
			// butBrowseIn
			// 
			this.butBrowseIn.Location = new System.Drawing.Point(340, 12);
			this.butBrowseIn.Name = "butBrowseIn";
			this.butBrowseIn.Size = new System.Drawing.Size(76, 24);
			this.butBrowseIn.TabIndex = 54;
			this.butBrowseIn.Text = "&Browse";
			this.butBrowseIn.Click += new System.EventHandler(this.butBrowseIn_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(511, 656);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 68;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(650, 656);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 69;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(736, 656);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 70;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(17, 656);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(85, 24);
			this.butDelete.TabIndex = 67;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(17, 347);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(796, 303);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Messages / Segments";
			this.gridMain.TranslationName = "TableMessages";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// checkLongDCodes
			// 
			this.checkLongDCodes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLongDCodes.Location = new System.Drawing.Point(63, 306);
			this.checkLongDCodes.Name = "checkLongDCodes";
			this.checkLongDCodes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkLongDCodes.Size = new System.Drawing.Size(162, 18);
			this.checkLongDCodes.TabIndex = 71;
			this.checkLongDCodes.Text = "Send Long D Codes";
			// 
			// checkProcsAppt
			// 
			this.checkProcsAppt.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcsAppt.Location = new System.Drawing.Point(6, 323);
			this.checkProcsAppt.Name = "checkProcsAppt";
			this.checkProcsAppt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkProcsAppt.Size = new System.Drawing.Size(219, 18);
			this.checkProcsAppt.TabIndex = 72;
			this.checkProcsAppt.Text = "Warn if Procs not attached to Appt";
			// 
			// FormHL7DefEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(823, 696);
			this.Controls.Add(this.checkProcsAppt);
			this.Controls.Add(this.checkLongDCodes);
			this.Controls.Add(this.groupHL7Comm);
			this.Controls.Add(this.groupDelimeters);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.checkShowAccount);
			this.Controls.Add(this.checkShowAppts);
			this.Controls.Add(this.groupShowDemographics);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.textHL7ServiceName);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.textHL7Server);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.labelDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textInternalTypeVersion);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textInternalType);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.checkInternal);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.comboModeTx);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.checkQuadAsToothNum);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormHL7DefEdit";
			this.ShowInTaskbar = false;
			this.Text = "HL7 Def Edit";
			this.Load += new System.EventHandler(this.FormHL7DefEdit_Load);
			this.groupShowDemographics.ResumeLayout(false);
			this.groupDelimeters.ResumeLayout(false);
			this.groupDelimeters.PerformLayout();
			this.groupHL7Comm.ResumeLayout(false);
			this.groupHL7Comm.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label labelOutSocketOrDirEx;
		private System.Windows.Forms.Label labelInSocketEx;
		private System.Windows.Forms.TextBox textInternalTypeVersion;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textInternalType;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.TextBox textEscChar;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox checkInternal;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textSubcompSep;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textRepSep;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textCompSep;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboModeTx;
		private UI.Button butBrowseOut;
		private UI.Button butBrowseIn;
		private System.Windows.Forms.TextBox textSftpPassword;
		private System.Windows.Forms.Label labelSftpPassword;
		private System.Windows.Forms.TextBox textSftpUsername;
		private System.Windows.Forms.Label labelSftpUsername;
		private System.Windows.Forms.TextBox textOutPathSocketOrDir;
		private System.Windows.Forms.Label labelOutPathSocketOrDir;
		private System.Windows.Forms.TextBox textInPathOrSocket;
		private System.Windows.Forms.Label labelInPathOrSocket;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelDelete;
		private UI.Button butAdd;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox textHL7ServiceName;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox textHL7Server;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.GroupBox groupShowDemographics;
		private System.Windows.Forms.RadioButton radioChangeAndAdd;
		private System.Windows.Forms.RadioButton radioChange;
		private System.Windows.Forms.RadioButton radioShow;
		private System.Windows.Forms.RadioButton radioHide;
		private System.Windows.Forms.CheckBox checkShowAccount;
		private System.Windows.Forms.CheckBox checkShowAppts;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.CheckBox checkQuadAsToothNum;
		private System.Windows.Forms.Label labelLabImageCat;
		private System.Windows.Forms.ComboBox comboLabImageCat;
		private System.Windows.Forms.GroupBox groupDelimeters;
		private System.Windows.Forms.TextBox textFieldSep;
		private System.Windows.Forms.GroupBox groupHL7Comm;
		private System.Windows.Forms.CheckBox checkLongDCodes;
		private System.Windows.Forms.CheckBox checkProcsAppt;
	}
}