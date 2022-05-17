namespace OpenDental {
	partial class FormEhrMedicalOrderRadEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrMedicalOrderRadEdit));
			this.butOK = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.textDateTime = new System.Windows.Forms.TextBox();
			this.labelDateTime = new System.Windows.Forms.Label();
			this.checkIsDiscontinued = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(322, 243);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 11;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(403, 243);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 243);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 12;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(12, 101);
			this.textDescription.Multiline = true;
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(466, 136);
			this.textDescription.TabIndex = 16;
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(11, 77);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(116, 21);
			this.labelDescription.TabIndex = 17;
			this.labelDescription.Text = "Instructions";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDateTime
			// 
			this.textDateTime.Location = new System.Drawing.Point(271, 13);
			this.textDateTime.Name = "textDateTime";
			this.textDateTime.Size = new System.Drawing.Size(207, 20);
			this.textDateTime.TabIndex = 18;
			// 
			// labelDateTime
			// 
			this.labelDateTime.Location = new System.Drawing.Point(153, 15);
			this.labelDateTime.Name = "labelDateTime";
			this.labelDateTime.Size = new System.Drawing.Size(116, 17);
			this.labelDateTime.TabIndex = 19;
			this.labelDateTime.Text = "Date Time";
			this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsDiscontinued
			// 
			this.checkIsDiscontinued.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDiscontinued.Location = new System.Drawing.Point(159, 41);
			this.checkIsDiscontinued.Name = "checkIsDiscontinued";
			this.checkIsDiscontinued.Size = new System.Drawing.Size(126, 18);
			this.checkIsDiscontinued.TabIndex = 23;
			this.checkIsDiscontinued.Text = "Discontinued";
			this.checkIsDiscontinued.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDiscontinued.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(153, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 17);
			this.label1.TabIndex = 30;
			this.label1.Text = "Provider";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.FormattingEnabled = true;
			this.comboProv.Location = new System.Drawing.Point(271, 64);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(207, 21);
			this.comboProv.TabIndex = 29;
			// 
			// FormEhrMedicalOrderRadEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 278);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.checkIsDiscontinued);
			this.Controls.Add(this.textDateTime);
			this.Controls.Add(this.labelDateTime);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrMedicalOrderRadEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Radiology Order Edit";
			this.Load += new System.EventHandler(this.FormMedicalOrderRadEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textDateTime;
		private System.Windows.Forms.Label labelDateTime;
		private System.Windows.Forms.CheckBox checkIsDiscontinued;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboProv;
	}
}