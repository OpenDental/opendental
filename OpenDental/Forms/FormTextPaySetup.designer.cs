namespace OpenDental{
	partial class FormTextPaySetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTextPaySetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboBoxClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.textMessageTemplate = new System.Windows.Forms.TextBox();
			this.contextMenuStripTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemInsertFields = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxTemplates = new OpenDental.UI.ComboBoxOD();
			this.checkUseDefaults = new System.Windows.Forms.CheckBox();
			this.contextMenuStripTextTemplate.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(363, 269);
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
			this.butCancel.Location = new System.Drawing.Point(282, 269);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboBoxClinicPicker
			// 
			this.comboBoxClinicPicker.Location = new System.Drawing.Point(48, 12);
			this.comboBoxClinicPicker.Name = "comboBoxClinicPicker";
			this.comboBoxClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.comboBoxClinicPicker.TabIndex = 4;
			this.comboBoxClinicPicker.SelectionChangeCommitted += new System.EventHandler(this.comboBoxClinicPicker_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 21);
			this.label1.TabIndex = 7;
			this.label1.Text = "Template";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMessageTemplate
			// 
			this.textMessageTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMessageTemplate.ContextMenuStrip = this.contextMenuStripTextTemplate;
			this.textMessageTemplate.Location = new System.Drawing.Point(85, 66);
			this.textMessageTemplate.Multiline = true;
			this.textMessageTemplate.Name = "textMessageTemplate";
			this.textMessageTemplate.Size = new System.Drawing.Size(353, 196);
			this.textMessageTemplate.TabIndex = 8;
			// 
			// contextMenuStripTextTemplate
			// 
			this.contextMenuStripTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemInsertFields,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
			this.contextMenuStripTextTemplate.Name = "contextMenuStripTextTemplate";
			this.contextMenuStripTextTemplate.Size = new System.Drawing.Size(137, 136);
			// 
			// toolStripMenuItemInsertFields
			// 
			this.toolStripMenuItemInsertFields.Name = "toolStripMenuItemInsertFields";
			this.toolStripMenuItemInsertFields.Size = new System.Drawing.Size(136, 22);
			this.toolStripMenuItemInsertFields.Text = "Insert Fields";
			this.toolStripMenuItemInsertFields.Click += new System.EventHandler(this.toolStripMenuItemInsertFields_Click);
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemUndo_Click);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemCut_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemPaste_Click);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectAllToolStripMenuItem.Text = "Select All";
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemSelectAll_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(15, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 21);
			this.label2.TabIndex = 9;
			this.label2.Text = "Message";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxTemplates
			// 
			this.comboBoxTemplates.Location = new System.Drawing.Point(85, 39);
			this.comboBoxTemplates.Name = "comboBoxTemplates";
			this.comboBoxTemplates.Size = new System.Drawing.Size(163, 21);
			this.comboBoxTemplates.TabIndex = 10;
			this.comboBoxTemplates.Text = "comboBoxOD1";
			this.comboBoxTemplates.SelectionChangeCommitted += new System.EventHandler(this.comboBoxTemplates_SelectionChangeCommitted);
			// 
			// checkUseDefaults
			// 
			this.checkUseDefaults.Location = new System.Drawing.Point(254, 12);
			this.checkUseDefaults.Name = "checkUseDefaults";
			this.checkUseDefaults.Size = new System.Drawing.Size(184, 21);
			this.checkUseDefaults.TabIndex = 11;
			this.checkUseDefaults.Text = "Use Default Template";
			this.checkUseDefaults.UseVisualStyleBackColor = true;
			this.checkUseDefaults.Visible = false;
			this.checkUseDefaults.Click += new System.EventHandler(this.checkUseDefaults_Click);
			// 
			// FormTextPaySetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(450, 305);
			this.Controls.Add(this.checkUseDefaults);
			this.Controls.Add(this.comboBoxTemplates);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textMessageTemplate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxClinicPicker);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTextPaySetup";
			this.Text = "Text Pay Template Edit";
			this.Load += new System.EventHandler(this.FormTextPaySetup_Load);
			this.contextMenuStripTextTemplate.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.ComboBoxClinicPicker comboBoxClinicPicker;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMessageTemplate;
		private System.Windows.Forms.Label label2;
		private UI.ComboBoxOD comboBoxTemplates;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemInsertFields;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.CheckBox checkUseDefaults;
	}
}