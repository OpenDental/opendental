namespace OpenDental{
	partial class FormResellerServiceEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormResellerServiceEdit));
			this.menuRightClick = new System.Windows.Forms.ContextMenu();
			this.menuItemAccount = new System.Windows.Forms.MenuItem();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDesc = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textCode = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butPick = new OpenDental.UI.Button();
			this.textFee = new ValidDouble();
			this.SuspendLayout();
			// 
			// menuRightClick
			// 
			this.menuRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAccount});
			// 
			// menuItemAccount
			// 
			this.menuItemAccount.Index = 0;
			this.menuItemAccount.Text = "Go to Account";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(254, 175);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 251;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 175);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 41;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(349, 175);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDesc
			// 
			this.textDesc.BackColor = System.Drawing.SystemColors.Control;
			this.textDesc.Location = new System.Drawing.Point(155, 72);
			this.textDesc.Name = "textDesc";
			this.textDesc.Size = new System.Drawing.Size(261, 20);
			this.textDesc.TabIndex = 255;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(26, 73);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(127, 16);
			this.label6.TabIndex = 254;
			this.label6.Text = "Service Description:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(155, 39);
			this.textCode.MaxLength = 15;
			this.textCode.Name = "textCode";
			this.textCode.ReadOnly = true;
			this.textCode.Size = new System.Drawing.Size(100, 20);
			this.textCode.TabIndex = 253;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(86, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 16);
			this.label1.TabIndex = 252;
			this.label1.Text = "Code";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(82, 108);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 16);
			this.label2.TabIndex = 256;
			this.label2.Text = "Fee";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPick
			// 
			this.butPick.Location = new System.Drawing.Point(315, 36);
			this.butPick.Name = "butPick";
			this.butPick.Size = new System.Drawing.Size(101, 24);
			this.butPick.TabIndex = 258;
			this.butPick.Text = "Pick From List";
			this.butPick.Click += new System.EventHandler(this.butPick_Click);
			// 
			// textFee
			// 
			this.textFee.Location = new System.Drawing.Point(155, 107);
			this.textFee.MaxVal = 100000000D;
			this.textFee.MinVal = -100000000D;
			this.textFee.Name = "textFee";
			this.textFee.Size = new System.Drawing.Size(100, 20);
			this.textFee.TabIndex = 259;
			// 
			// FormResellerServiceEdit
			// 
			this.ClientSize = new System.Drawing.Size(446, 211);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textFee);
			this.Controls.Add(this.butPick);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDesc);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormResellerServiceEdit";
			this.Text = "Reseller Service Edit";
			this.Load += new System.EventHandler(this.FormResellerServiceEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private UI.Button butOK;
		private System.Windows.Forms.ContextMenu menuRightClick;
		private System.Windows.Forms.MenuItem menuItemAccount;
		private System.Windows.Forms.TextBox textDesc;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textCode;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private UI.Button butPick;
		private ValidDouble textFee;
	}
}