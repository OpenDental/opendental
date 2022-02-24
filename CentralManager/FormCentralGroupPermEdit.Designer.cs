namespace CentralManager {
	partial class FormCentralGroupPermEdit {
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textDate = new OpenDental.ValidDate();
			this.textDays = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textDate);
			this.groupBox1.Controls.Add(this.textDays);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(21, 50);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(433, 126);
			this.groupBox1.TabIndex = 58;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Only if newer than";
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(130, 19);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 50;
			this.textDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDate_KeyDown);
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(130, 45);
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(46, 20);
			this.textDays.TabIndex = 0;
			this.textDays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDays_KeyDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 18);
			this.label1.TabIndex = 46;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(126, 16);
			this.label2.TabIndex = 47;
			this.label2.Text = "Days";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(87, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(320, 40);
			this.label3.TabIndex = 52;
			this.label3.Text = "For instance, if you set days to 1, then user will only have permission if the da" +
    "te of the item is today";
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(145, 17);
			this.textName.MaxLength = 100;
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(260, 20);
			this.textName.TabIndex = 56;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(48, 20);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(95, 14);
			this.label10.TabIndex = 57;
			this.label10.Text = "Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(388, 191);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 55;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butOK.Location = new System.Drawing.Point(307, 191);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 54;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormCentralGroupPermEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(475, 229);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(491, 267);
			this.Name = "FormCentralGroupPermEdit";
			this.Text = "Group Permission Edit";
			this.Load += new System.EventHandler(this.FormCentralGroupPermEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.TextBox textDays;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label10;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;

	}
}