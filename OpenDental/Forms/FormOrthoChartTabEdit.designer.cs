namespace OpenDental{
	partial class FormOrthoChartTabEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoChartTabEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textTabName = new System.Windows.Forms.TextBox();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(148, 111);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(229, 111);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "Tab Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTabName
			// 
			this.textTabName.Location = new System.Drawing.Point(123, 32);
			this.textTabName.Name = "textTabName";
			this.textTabName.Size = new System.Drawing.Size(100, 20);
			this.textTabName.TabIndex = 0;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.AutoSize = true;
			this.checkIsHidden.Location = new System.Drawing.Point(66, 58);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsHidden.Size = new System.Drawing.Size(71, 17);
			this.checkIsHidden.TabIndex = 1;
			this.checkIsHidden.Text = "Is Hidden";
			this.checkIsHidden.UseVisualStyleBackColor = true;
			// 
			// FormOrthoChartTabEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(316, 147);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.textTabName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoChartTabEdit";
			this.Text = "Ortho Chart Tab Edit";
			this.Load += new System.EventHandler(this.FormOrthoChartTabEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textTabName;
		private System.Windows.Forms.CheckBox checkIsHidden;
	}
}