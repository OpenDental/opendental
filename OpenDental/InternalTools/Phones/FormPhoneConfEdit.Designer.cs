namespace OpenDental {
	partial class FormPhoneConfEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneConfEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textButtonIndex = new OpenDental.ValidDouble();
			this.label1 = new System.Windows.Forms.Label();
			this.textExtension = new OpenDental.ValidDouble();
			this.label2 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboSite = new OpenDental.UI.ComboBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(329, 128);
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
			this.butCancel.Location = new System.Drawing.Point(410, 128);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textButtonIndex
			// 
			this.textButtonIndex.Location = new System.Drawing.Point(190, 50);
			this.textButtonIndex.MaxVal = 19D;
			this.textButtonIndex.MinVal = -1D;
			this.textButtonIndex.Name = "textButtonIndex";
			this.textButtonIndex.Size = new System.Drawing.Size(134, 20);
			this.textButtonIndex.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(35, 53);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "Button Index";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExtension
			// 
			this.textExtension.Location = new System.Drawing.Point(190, 24);
			this.textExtension.MaxVal = 100000000D;
			this.textExtension.MinVal = 1D;
			this.textExtension.Name = "textExtension";
			this.textExtension.Size = new System.Drawing.Size(134, 20);
			this.textExtension.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(35, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(154, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "Extension";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(38, 79);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(149, 16);
			this.label8.TabIndex = 59;
			this.label8.Text = "Site";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboSite.ForeColor = System.Drawing.Color.Black;
			this.comboSite.Location = new System.Drawing.Point(190, 76);
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(134, 21);
			this.comboSite.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(338, 53);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 12);
			this.label3.TabIndex = 60;
			this.label3.Text = "(zero based)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 127);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(82, 25);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormPhoneConfEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(497, 164);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.textExtension);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textButtonIndex);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneConfEdit";
			this.Text = "Conference Room Edit";
			this.Load += new System.EventHandler(this.FormPhoneConfEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private UI.ComboBoxOD comboSite;
		private System.Windows.Forms.Label label3;
		private UI.Button butDelete;
		private ValidDouble textButtonIndex;
		private ValidDouble textExtension;
	}
}