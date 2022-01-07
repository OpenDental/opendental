namespace OpenDental{
	partial class FormImageDrawColor {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageDrawColor));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.butColor = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.butColorBack = new System.Windows.Forms.Button();
			this.checkTransparent = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(110, 108);
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
			this.butCancel.Location = new System.Drawing.Point(202, 108);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(40, 27);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 18);
			this.label5.TabIndex = 159;
			this.label5.Text = "Text Color";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(122, 26);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 160;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(15, 53);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 18);
			this.label4.TabIndex = 165;
			this.label4.Text = "Background Color";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorBack
			// 
			this.butColorBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorBack.Location = new System.Drawing.Point(122, 52);
			this.butColorBack.Name = "butColorBack";
			this.butColorBack.Size = new System.Drawing.Size(30, 20);
			this.butColorBack.TabIndex = 166;
			this.butColorBack.Click += new System.EventHandler(this.butColorBack_Click);
			// 
			// checkNone
			// 
			this.checkTransparent.Location = new System.Drawing.Point(160, 54);
			this.checkTransparent.Name = "checkNone";
			this.checkTransparent.Size = new System.Drawing.Size(117, 18);
			this.checkTransparent.TabIndex = 167;
			this.checkTransparent.Text = "Transparent";
			this.checkTransparent.UseVisualStyleBackColor = true;
			this.checkTransparent.Click += new System.EventHandler(this.checkNone_Click);
			// 
			// FormImageDrawColor
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(289, 144);
			this.Controls.Add(this.checkTransparent);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butColorBack);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageDrawColor";
			this.Text = "Edit Color";
			this.Load += new System.EventHandler(this.FormImageDrawEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button butColorBack;
		private System.Windows.Forms.CheckBox checkTransparent;
	}
}