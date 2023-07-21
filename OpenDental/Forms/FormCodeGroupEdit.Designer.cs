namespace OpenDental {
	partial class FormCodeGroupEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCodeGroupEdit));
			this.butSave = new OpenDental.UI.Button();
			this.textGroupName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textProcCodes = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.comboCodeGroupFixed = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butProcCodesAdd = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(584, 264);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 5;
			this.butSave.Text = "Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textGroupName
			// 
			this.textGroupName.Location = new System.Drawing.Point(149, 61);
			this.textGroupName.MaxLength = 50;
			this.textGroupName.Name = "textGroupName";
			this.textGroupName.Size = new System.Drawing.Size(167, 20);
			this.textGroupName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 14);
			this.label1.TabIndex = 283;
			this.label1.Text = "Group Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textProcCodes
			// 
			this.textProcCodes.Location = new System.Drawing.Point(149, 146);
			this.textProcCodes.Multiline = true;
			this.textProcCodes.Name = "textProcCodes";
			this.textProcCodes.Size = new System.Drawing.Size(410, 49);
			this.textProcCodes.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 150);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 14);
			this.label2.TabIndex = 285;
			this.label2.Text = "Proc Codes";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(68, 35);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(95, 20);
			this.checkIsHidden.TabIndex = 4;
			this.checkIsHidden.Text = "Is Hidden";
			this.checkIsHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.UseVisualStyleBackColor = true;
			// 
			// comboCodeGroupFixed
			// 
			this.comboCodeGroupFixed.Location = new System.Drawing.Point(149, 93);
			this.comboCodeGroupFixed.Name = "comboCodeGroupFixed";
			this.comboCodeGroupFixed.Size = new System.Drawing.Size(167, 21);
			this.comboCodeGroupFixed.TabIndex = 1;
			this.comboCodeGroupFixed.Text = "Code Group Fixed";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 97);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 14);
			this.label3.TabIndex = 288;
			this.label3.Text = "Fixed Group";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butProcCodesAdd
			// 
			this.butProcCodesAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butProcCodesAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProcCodesAdd.Location = new System.Drawing.Point(565, 146);
			this.butProcCodesAdd.Name = "butProcCodesAdd";
			this.butProcCodesAdd.Size = new System.Drawing.Size(75, 24);
			this.butProcCodesAdd.TabIndex = 3;
			this.butProcCodesAdd.Text = "&Add";
			this.butProcCodesAdd.Click += new System.EventHandler(this.butProcCodesAdd_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(146, 198);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(295, 14);
			this.label4.TabIndex = 289;
			this.label4.Text = "Codes should be comma separated.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(322, 81);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(325, 59);
			this.label5.TabIndex = 290;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// FormCodeGroupEdit
			// 
			this.ClientSize = new System.Drawing.Size(671, 300);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butProcCodesAdd);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboCodeGroupFixed);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.textProcCodes);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textGroupName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCodeGroupEdit";
			this.Text = "Code Group Edit";
			this.Load += new System.EventHandler(this.FormCodeGroupEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.TextBox textGroupName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textProcCodes;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkIsHidden;
		private UI.ComboBox comboCodeGroupFixed;
		private System.Windows.Forms.Label label3;
		private UI.Button butProcCodesAdd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
	}
}