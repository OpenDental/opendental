namespace CentralManager {
	partial class FormCentralConnectionGroups {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralConnectionGroups));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.comboConnectionGroup = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(13, 32);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(341, 377);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Groups";
			this.gridMain.TranslationName = "TableGroups";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(370, 385);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.butAdd.Image = ((System.Drawing.Image)(resources.GetObject("butAdd.Image")));
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(370, 199);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 218;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// comboConnectionGroup
			// 
			this.comboConnectionGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConnectionGroup.FormattingEnabled = true;
			this.comboConnectionGroup.Location = new System.Drawing.Point(12, 5);
			this.comboConnectionGroup.MaxDropDownItems = 20;
			this.comboConnectionGroup.Name = "comboConnectionGroup";
			this.comboConnectionGroup.Size = new System.Drawing.Size(169, 21);
			this.comboConnectionGroup.TabIndex = 221;
			this.comboConnectionGroup.SelectionChangeCommitted += new System.EventHandler(this.comboConnectionGroup_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(187, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(244, 15);
			this.label1.TabIndex = 220;
			this.label1.Text = "Default Group on Startup";
			// 
			// FormCentralConnectionGroups
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(457, 421);
			this.Controls.Add(this.comboConnectionGroup);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.gridMain);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(235, 242);
			this.Name = "FormCentralConnectionGroups";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connection Groups";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCentralConnectionGroups_FormClosing);
			this.Load += new System.EventHandler(this.FormCentralConnectionGroups_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.ComboBox comboConnectionGroup;
		private System.Windows.Forms.Label label1;
	}
}