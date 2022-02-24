namespace OpenDental{
	partial class FormEServicesWebSchedNotify {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesWebSchedNotify));
			this.contextMenuStripTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemInsertReplacements = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butRestoreWebSchedVerify = new OpenDental.UI.Button();
			this.label28 = new System.Windows.Forms.Label();
			this.checkUseDefaultsVerify = new System.Windows.Forms.CheckBox();
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.groupBoxTextTemplate = new System.Windows.Forms.GroupBox();
			this.textMessageTemplate = new System.Windows.Forms.TextBox();
			this.groupBoxRecallEmail = new System.Windows.Forms.GroupBox();
			this.butEditEmail = new OpenDental.UI.Button();
			this.browserEmailBody = new System.Windows.Forms.WebBrowser();
			this.textEmailSubj = new System.Windows.Forms.TextBox();
			this.groupBoxRadio = new System.Windows.Forms.GroupBox();
			this.radioTextAndEmail = new System.Windows.Forms.RadioButton();
			this.radioEmail = new System.Windows.Forms.RadioButton();
			this.radioText = new System.Windows.Forms.RadioButton();
			this.radioNone = new System.Windows.Forms.RadioButton();
			this.comboClinicVerify = new OpenDental.UI.ComboBoxClinicPicker();
			this.contextMenuStripTextTemplate.SuspendLayout();
			this.groupBox.SuspendLayout();
			this.groupBoxTextTemplate.SuspendLayout();
			this.groupBoxRecallEmail.SuspendLayout();
			this.groupBoxRadio.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStripTextTemplate
			// 
			this.contextMenuStripTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.toolStripMenuItemInsertReplacements,
				this.toolStripMenuItemUndo,
				this.toolStripMenuItemCut,
				this.toolStripMenuItemCopy,
				this.toolStripMenuItemPaste,
				this.toolStripMenuItemSelectAll});
			this.contextMenuStripTextTemplate.Name = "contextMenuStripTextTemplate";
			this.contextMenuStripTextTemplate.Size = new System.Drawing.Size(137,136);
			// 
			// toolStripMenuItemInsertReplacements
			// 
			this.toolStripMenuItemInsertReplacements.Name = "insertReplacementsToolStripMenuItem";
			this.toolStripMenuItemInsertReplacements.Size = new System.Drawing.Size(136,22);
			this.toolStripMenuItemInsertReplacements.Text = "Insert Fields";
			this.toolStripMenuItemInsertReplacements.Click += new System.EventHandler(this.toolStripMenuItemInsertReplacements_Click);
			// 
			// toolStripMenuItemUndo
			// 
			this.toolStripMenuItemUndo.Name = "undoToolStripMenuItem";
			this.toolStripMenuItemUndo.Size = new System.Drawing.Size(136,22);
			this.toolStripMenuItemUndo.Text = "Undo";
			this.toolStripMenuItemUndo.Click += new System.EventHandler(this.toolStripMenuItemUndo_Click);
			// 
			// toolStripMenuItemCut
			// 
			this.toolStripMenuItemCut.Name = "cutToolStripMenuItem";
			this.toolStripMenuItemCut.Size = new System.Drawing.Size(136,22);
			this.toolStripMenuItemCut.Text = "Cut";
			this.toolStripMenuItemCut.Click += new System.EventHandler(this.toolStripMenuItemCut_Click);
			// 
			// toolStripMenuItemCopy
			// 
			this.toolStripMenuItemCopy.Name = "copyToolStripMenuItem";
			this.toolStripMenuItemCopy.Size = new System.Drawing.Size(136,22);
			this.toolStripMenuItemCopy.Text = "Copy";
			this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
			// 
			// toolStripMenuItemPaste
			// 
			this.toolStripMenuItemPaste.Name = "pasteToolStripMenuItem";
			this.toolStripMenuItemPaste.Size = new System.Drawing.Size(136,22);
			this.toolStripMenuItemPaste.Text = "Paste";
			this.toolStripMenuItemPaste.Click += new System.EventHandler(this.toolStripMenuItemPaste_Click);
			// 
			// toolStripMenuItemSelectAll
			// 
			this.toolStripMenuItemSelectAll.Name = "selectAllToolStripMenuItem";
			this.toolStripMenuItemSelectAll.Size = new System.Drawing.Size(136,22);
			this.toolStripMenuItemSelectAll.Text = "Select All";
			this.toolStripMenuItemSelectAll.Click += new System.EventHandler(this.toolStripMenuItemSelectAll_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(277, 601);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(358, 601);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butRestoreWebSchedVerify
			// 
			this.butRestoreWebSchedVerify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRestoreWebSchedVerify.Location = new System.Drawing.Point(12, 602);
			this.butRestoreWebSchedVerify.Name = "butRestoreWebSchedVerify";
			this.butRestoreWebSchedVerify.Size = new System.Drawing.Size(75, 23);
			this.butRestoreWebSchedVerify.TabIndex = 311;
			this.butRestoreWebSchedVerify.Text = "Undo All";
			this.butRestoreWebSchedVerify.UseVisualStyleBackColor = true;
			this.butRestoreWebSchedVerify.Click += new System.EventHandler(this.butRestore_Click);
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(15, 41);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(343, 28);
			this.label28.TabIndex = 310;
			this.label28.Text = "Right-click on any text box to choose from a list of valid template fields.";
			this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUseDefaultsVerify
			// 
			this.checkUseDefaultsVerify.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseDefaultsVerify.Location = new System.Drawing.Point(246, 14);
			this.checkUseDefaultsVerify.Name = "checkUseDefaultsVerify";
			this.checkUseDefaultsVerify.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsVerify.TabIndex = 308;
			this.checkUseDefaultsVerify.Text = "Use Defaults";
			this.checkUseDefaultsVerify.CheckedChanged += new System.EventHandler(this.checkUseDefaults_CheckChanged);
			// 
			// groupBox
			// 
			this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox.Controls.Add(this.groupBoxTextTemplate);
			this.groupBox.Controls.Add(this.groupBoxRecallEmail);
			this.groupBox.Controls.Add(this.groupBoxRadio);
			this.groupBox.Location = new System.Drawing.Point(12, 72);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(421, 497);
			this.groupBox.TabIndex = 305;
			this.groupBox.TabStop = false;
			// 
			// groupBoxTextTemplate
			// 
			this.groupBoxTextTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxTextTemplate.Controls.Add(this.textMessageTemplate);
			this.groupBoxTextTemplate.Location = new System.Drawing.Point(6, 139);
			this.groupBoxTextTemplate.Name = "groupBoxTextTemplate";
			this.groupBoxTextTemplate.Size = new System.Drawing.Size(409, 120);
			this.groupBoxTextTemplate.TabIndex = 121;
			this.groupBoxTextTemplate.TabStop = false;
			this.groupBoxTextTemplate.Text = "Text Message";
			// 
			// textMessageTemplate
			// 
			this.textMessageTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMessageTemplate.ContextMenuStrip = this.contextMenuStripTextTemplate;
			this.textMessageTemplate.Location = new System.Drawing.Point(6, 19);
			this.textMessageTemplate.Multiline = true;
			this.textMessageTemplate.Name = "textMessageTemplate";
			this.textMessageTemplate.Size = new System.Drawing.Size(397, 95);
			this.textMessageTemplate.TabIndex = 314;
			this.textMessageTemplate.Leave += new System.EventHandler(this.textMessageTemplate_Leave);
			// 
			// groupBoxRecallEmail
			// 
			this.groupBoxRecallEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxRecallEmail.Controls.Add(this.butEditEmail);
			this.groupBoxRecallEmail.Controls.Add(this.browserEmailBody);
			this.groupBoxRecallEmail.Controls.Add(this.textEmailSubj);
			this.groupBoxRecallEmail.Location = new System.Drawing.Point(6, 259);
			this.groupBoxRecallEmail.Name = "groupBoxRecallEmail";
			this.groupBoxRecallEmail.Size = new System.Drawing.Size(409, 232);
			this.groupBoxRecallEmail.TabIndex = 122;
			this.groupBoxRecallEmail.TabStop = false;
			this.groupBoxRecallEmail.Text = "E-mail Subject and Body";
			// 
			// butEditEmail
			// 
			this.butEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditEmail.Location = new System.Drawing.Point(333, 208);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(70, 20);
			this.butEditEmail.TabIndex = 318;
			this.butEditEmail.Text = "Edit";
			this.butEditEmail.UseVisualStyleBackColor = true;
			this.butEditEmail.Click += new System.EventHandler(this.butEditEmail_Click);
			// 
			// browserEmailBody
			// 
			this.browserEmailBody.AllowWebBrowserDrop = false;
			this.browserEmailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browserEmailBody.Location = new System.Drawing.Point(7, 45);
			this.browserEmailBody.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmailBody.Name = "browserEmailBody";
			this.browserEmailBody.Size = new System.Drawing.Size(396, 158);
			this.browserEmailBody.TabIndex = 317;
			this.browserEmailBody.WebBrowserShortcutsEnabled = false;
			// 
			// textEmailSubj
			// 
			this.textEmailSubj.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEmailSubj.ContextMenuStrip = this.contextMenuStripTextTemplate;
			this.textEmailSubj.Location = new System.Drawing.Point(6, 19);
			this.textEmailSubj.Multiline = true;
			this.textEmailSubj.Name = "textEmailSubj";
			this.textEmailSubj.Size = new System.Drawing.Size(397, 20);
			this.textEmailSubj.TabIndex = 314;
			this.textEmailSubj.Leave += new System.EventHandler(this.textEmailSubj_Leave);
			// 
			// groupBoxRadio
			// 
			this.groupBoxRadio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxRadio.Controls.Add(this.radioTextAndEmail);
			this.groupBoxRadio.Controls.Add(this.radioEmail);
			this.groupBoxRadio.Controls.Add(this.radioText);
			this.groupBoxRadio.Controls.Add(this.radioNone);
			this.groupBoxRadio.Location = new System.Drawing.Point(6, 18);
			this.groupBoxRadio.Name = "groupBoxRadio";
			this.groupBoxRadio.Size = new System.Drawing.Size(409, 115);
			this.groupBoxRadio.TabIndex = 1;
			this.groupBoxRadio.TabStop = false;
			this.groupBoxRadio.Text = "Communication Method";
			// 
			// radioTextAndEmail
			// 
			this.radioTextAndEmail.Location = new System.Drawing.Point(7, 89);
			this.radioTextAndEmail.Name = "radioTextAndEmail";
			this.radioTextAndEmail.Size = new System.Drawing.Size(175, 17);
			this.radioTextAndEmail.TabIndex = 3;
			this.radioTextAndEmail.TabStop = true;
			this.radioTextAndEmail.Tag = OpenDentBusiness.WebSchedVerifyType.TextAndEmail;
			this.radioTextAndEmail.Text = "Text and E-mail";
			this.radioTextAndEmail.UseVisualStyleBackColor = true;
			this.radioTextAndEmail.CheckedChanged += new System.EventHandler(this.WebSchedVerify_RadioButtonCheckChanged);
			// 
			// radioEmail
			// 
			this.radioEmail.Location = new System.Drawing.Point(7, 66);
			this.radioEmail.Name = "radioEmail";
			this.radioEmail.Size = new System.Drawing.Size(175, 17);
			this.radioEmail.TabIndex = 2;
			this.radioEmail.TabStop = true;
			this.radioEmail.Tag = OpenDentBusiness.WebSchedVerifyType.Email;
			this.radioEmail.Text = "E-mail";
			this.radioEmail.UseVisualStyleBackColor = true;
			this.radioEmail.CheckedChanged += new System.EventHandler(this.WebSchedVerify_RadioButtonCheckChanged);
			// 
			// radioText
			// 
			this.radioText.Location = new System.Drawing.Point(7, 43);
			this.radioText.Name = "radioText";
			this.radioText.Size = new System.Drawing.Size(175, 17);
			this.radioText.TabIndex = 1;
			this.radioText.TabStop = true;
			this.radioText.Tag = OpenDentBusiness.WebSchedVerifyType.Text;
			this.radioText.Text = "Text";
			this.radioText.UseVisualStyleBackColor = true;
			this.radioText.CheckedChanged += new System.EventHandler(this.WebSchedVerify_RadioButtonCheckChanged);
			// 
			// radioNone
			// 
			this.radioNone.Location = new System.Drawing.Point(7, 20);
			this.radioNone.Name = "radioNone";
			this.radioNone.Size = new System.Drawing.Size(175, 17);
			this.radioNone.TabIndex = 0;
			this.radioNone.TabStop = true;
			this.radioNone.Tag = OpenDentBusiness.WebSchedVerifyType.None;
			this.radioNone.Text = "None";
			this.radioNone.UseVisualStyleBackColor = true;
			this.radioNone.CheckedChanged += new System.EventHandler(this.WebSchedVerify_RadioButtonCheckChanged);
			// 
			// comboClinicVerify
			// 
			this.comboClinicVerify.HqDescription = "Default";
			this.comboClinicVerify.IncludeUnassigned = true;
			this.comboClinicVerify.Location = new System.Drawing.Point(12, 12);
			this.comboClinicVerify.Name = "comboClinicVerify";
			this.comboClinicVerify.Size = new System.Drawing.Size(216, 21);
			this.comboClinicVerify.TabIndex = 309;
			this.comboClinicVerify.SelectionChangeCommitted += new System.EventHandler(this.comboClinicVerify_SelectionChangeCommitted);
			// 
			// FormEServicesWebSchedNotify
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(445, 637);
			this.Controls.Add(this.butRestoreWebSchedVerify);
			this.Controls.Add(this.label28);
			this.Controls.Add(this.checkUseDefaultsVerify);
			this.Controls.Add(this.groupBox);
			this.Controls.Add(this.comboClinicVerify);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesWebSchedNotify";
			this.Text = "eServices Web Sched Notify";
			this.Load += new System.EventHandler(this.FormEServicesWebSchedNotify_Load);
			this.contextMenuStripTextTemplate.ResumeLayout(false);
			this.groupBox.ResumeLayout(false);
			this.groupBoxTextTemplate.ResumeLayout(false);
			this.groupBoxTextTemplate.PerformLayout();
			this.groupBoxRecallEmail.ResumeLayout(false);
			this.groupBoxRecallEmail.PerformLayout();
			this.groupBoxRadio.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertReplacements;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUndo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPaste;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectAll;
		private UI.Button butRestoreWebSchedVerify;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.CheckBox checkUseDefaultsVerify;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.GroupBox groupBoxTextTemplate;
		private System.Windows.Forms.TextBox textMessageTemplate;
		private System.Windows.Forms.GroupBox groupBoxRecallEmail;
		private UI.Button butEditEmail;
		private System.Windows.Forms.WebBrowser browserEmailBody;
		private System.Windows.Forms.TextBox textEmailSubj;
		private System.Windows.Forms.GroupBox groupBoxRadio;
		private System.Windows.Forms.RadioButton radioTextAndEmail;
		private System.Windows.Forms.RadioButton radioEmail;
		private System.Windows.Forms.RadioButton radioText;
		private System.Windows.Forms.RadioButton radioNone;
		private UI.ComboBoxClinicPicker comboClinicVerify;
	}
}