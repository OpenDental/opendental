namespace OpenDental{
	partial class FormDatePicker {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatePicker));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDate = new OpenDental.ValidDate();
			this.labelDateEntered = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(73, 68);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(65, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(144, 68);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(68, 26);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 1;
			// 
			// labelDateEntered
			// 
			this.labelDateEntered.Location = new System.Drawing.Point(15, 29);
			this.labelDateEntered.Name = "labelDateEntered";
			this.labelDateEntered.Size = new System.Drawing.Size(52, 17);
			this.labelDateEntered.TabIndex = 0;
			this.labelDateEntered.Text = "Date";
			this.labelDateEntered.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDatePicker
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(220, 103);
			this.Controls.Add(this.labelDateEntered);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDatePicker";
			this.Text = "";
			this.Load += new System.EventHandler(this.FormDatePicker_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidDate textDate;
		private System.Windows.Forms.Label labelDateEntered;
	}
}