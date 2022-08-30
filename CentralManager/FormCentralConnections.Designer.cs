namespace CentralManager {
	partial class FormCentralConnections {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralConnections));
			this.label1 = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.comboConnectionGroups = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupOrdering = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butAlphabetize = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkUseDynamicMode = new System.Windows.Forms.CheckBox();
			this.checkIsAutoLogon = new System.Windows.Forms.CheckBox();
			this.groupPrefs = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupOrdering.SuspendLayout();
			this.groupPrefs.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(366, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 17);
			this.label1.TabIndex = 214;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(438, 7);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(190, 20);
			this.textSearch.TabIndex = 213;
			this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
			// 
			// comboConnectionGroups
			// 
			this.comboConnectionGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConnectionGroups.FormattingEnabled = true;
			this.comboConnectionGroups.Location = new System.Drawing.Point(170, 6);
			this.comboConnectionGroups.MaxDropDownItems = 20;
			this.comboConnectionGroups.Name = "comboConnectionGroups";
			this.comboConnectionGroups.Size = new System.Drawing.Size(190, 21);
			this.comboConnectionGroups.TabIndex = 216;
			this.comboConnectionGroups.SelectionChangeCommitted += new System.EventHandler(this.comboConnectionGroups_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label3.Location = new System.Drawing.Point(12, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(155, 17);
			this.label3.TabIndex = 215;
			this.label3.Text = "Connection Groups";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupOrdering
			// 
			this.groupOrdering.Controls.Add(this.label2);
			this.groupOrdering.Controls.Add(this.butAlphabetize);
			this.groupOrdering.Controls.Add(this.butUp);
			this.groupOrdering.Controls.Add(this.butDown);
			this.groupOrdering.Location = new System.Drawing.Point(647, 88);
			this.groupOrdering.Name = "groupOrdering";
			this.groupOrdering.Size = new System.Drawing.Size(200, 125);
			this.groupOrdering.TabIndex = 219;
			this.groupOrdering.TabStop = false;
			this.groupOrdering.Text = "Conn. Order";
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(88, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 17);
			this.label2.TabIndex = 220;
			this.label2.Text = "(based on note)";
			// 
			// butAlphabetize
			// 
			this.butAlphabetize.Location = new System.Drawing.Point(6, 90);
			this.butAlphabetize.Name = "butAlphabetize";
			this.butAlphabetize.Size = new System.Drawing.Size(77, 24);
			this.butAlphabetize.TabIndex = 16;
			this.butAlphabetize.Text = "Alphabetize";
			this.butAlphabetize.Click += new System.EventHandler(this.butAlphabetize_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = ((System.Drawing.Image)(resources.GetObject("butUp.Image")));
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(6, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(77, 24);
			this.butUp.TabIndex = 4;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Image = ((System.Drawing.Image)(resources.GetObject("butDown.Image")));
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(6, 51);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(77, 24);
			this.butDown.TabIndex = 5;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butAdd
			// 
			this.butAdd.Image = ((System.Drawing.Image)(resources.GetObject("butAdd.Image")));
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(651, 56);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 218;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 33);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(616, 387);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Connections";
			this.gridMain.TranslationName = "TableConnections";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(772, 416);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(772, 448);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkUseDynamicMode
			// 
			this.checkUseDynamicMode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDynamicMode.Location = new System.Drawing.Point(6, 80);
			this.checkUseDynamicMode.Name = "checkUseDynamicMode";
			this.checkUseDynamicMode.Size = new System.Drawing.Size(188, 31);
			this.checkUseDynamicMode.TabIndex = 231;
			this.checkUseDynamicMode.Text = "Dynamic Mode\r\n(launch mismatched versions)";
			this.checkUseDynamicMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDynamicMode.UseVisualStyleBackColor = true;
			this.checkUseDynamicMode.Click += new System.EventHandler(this.CheckUseDynamicMode_Click);
			// 
			// checkIsAutoLogon
			// 
			this.checkIsAutoLogon.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsAutoLogon.Location = new System.Drawing.Point(1, 60);
			this.checkIsAutoLogon.Name = "checkIsAutoLogon";
			this.checkIsAutoLogon.Size = new System.Drawing.Size(193, 17);
			this.checkIsAutoLogon.TabIndex = 230;
			this.checkIsAutoLogon.Text = "Automatically Log On";
			this.checkIsAutoLogon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsAutoLogon.UseVisualStyleBackColor = true;
			this.checkIsAutoLogon.Click += new System.EventHandler(this.CheckIsAutoLogon_Click);
			// 
			// groupPrefs
			// 
			this.groupPrefs.Controls.Add(this.label5);
			this.groupPrefs.Controls.Add(this.label4);
			this.groupPrefs.Controls.Add(this.checkIsAutoLogon);
			this.groupPrefs.Controls.Add(this.checkUseDynamicMode);
			this.groupPrefs.Location = new System.Drawing.Point(647, 231);
			this.groupPrefs.Name = "groupPrefs";
			this.groupPrefs.Size = new System.Drawing.Size(200, 114);
			this.groupPrefs.TabIndex = 232;
			this.groupPrefs.TabStop = false;
			this.groupPrefs.Text = "Prefs";
			// 
			// label5
			// 
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label5.Location = new System.Drawing.Point(10, 33);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(165, 17);
			this.label5.TabIndex = 232;
			this.label5.Text = "Requires restart";
			// 
			// label4
			// 
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label4.Location = new System.Drawing.Point(10, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(165, 17);
			this.label4.TabIndex = 221;
			this.label4.Text = "Applies to all connections.";
			// 
			// FormCentralConnections
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(859, 483);
			this.Controls.Add(this.groupPrefs);
			this.Controls.Add(this.groupOrdering);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.comboConnectionGroups);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(738, 521);
			this.Name = "FormCentralConnections";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connections";
			this.Load += new System.EventHandler(this.FormCentralConnections_Load);
			this.groupOrdering.ResumeLayout(false);
			this.groupPrefs.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSearch;
		private System.Windows.Forms.ComboBox comboConnectionGroups;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.GroupBox groupOrdering;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butAlphabetize;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkUseDynamicMode;
		private System.Windows.Forms.CheckBox checkIsAutoLogon;
		private System.Windows.Forms.GroupBox groupPrefs;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
	}
}