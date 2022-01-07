
namespace OpenDental.Bridges {
	partial class FormDrCeph {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDrCeph));
			this.labelRace = new System.Windows.Forms.Label();
			this.labelDate = new System.Windows.Forms.Label();
			this.labelXRayPath = new System.Windows.Forms.Label();
			this.labelTxPhase = new System.Windows.Forms.Label();
			this.dateXRay = new System.Windows.Forms.DateTimePicker();
			this.labelPhotoPath = new System.Windows.Forms.Label();
			this.butOk = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butCephBrowse = new OpenDental.UI.Button();
			this.butPhotoBrowse = new OpenDental.UI.Button();
			this.textCephLocation = new System.Windows.Forms.TextBox();
			this.textPhotoLocation = new System.Windows.Forms.TextBox();
			this.listBoxRace = new OpenDental.UI.ListBoxOD();
			this.listBoxTxPhase = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// labelRace
			// 
			this.labelRace.Location = new System.Drawing.Point(182, 28);
			this.labelRace.Name = "labelRace";
			this.labelRace.Size = new System.Drawing.Size(120, 23);
			this.labelRace.TabIndex = 1;
			this.labelRace.Text = "Patient Race";
			this.labelRace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(25, 268);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(151, 23);
			this.labelDate.TabIndex = 2;
			this.labelDate.Text = "Ceph X-Ray Date";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelXRayPath
			// 
			this.labelXRayPath.Location = new System.Drawing.Point(25, 183);
			this.labelXRayPath.Name = "labelXRayPath";
			this.labelXRayPath.Size = new System.Drawing.Size(151, 23);
			this.labelXRayPath.TabIndex = 3;
			this.labelXRayPath.Text = "Ceph X-Ray Location";
			this.labelXRayPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTxPhase
			// 
			this.labelTxPhase.Location = new System.Drawing.Point(380, 28);
			this.labelTxPhase.Name = "labelTxPhase";
			this.labelTxPhase.Size = new System.Drawing.Size(120, 23);
			this.labelTxPhase.TabIndex = 5;
			this.labelTxPhase.Text = "Treatment Phase";
			this.labelTxPhase.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dateXRay
			// 
			this.dateXRay.CustomFormat = "MMM d, yyyy";
			this.dateXRay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateXRay.Location = new System.Drawing.Point(182, 269);
			this.dateXRay.Name = "dateXRay";
			this.dateXRay.Size = new System.Drawing.Size(159, 20);
			this.dateXRay.TabIndex = 9;
			this.dateXRay.TabStop = false;
			this.dateXRay.Value = new System.DateTime(2021, 8, 19, 0, 0, 0, 0);
			// 
			// labelPhotoPath
			// 
			this.labelPhotoPath.Location = new System.Drawing.Point(25, 225);
			this.labelPhotoPath.Name = "labelPhotoPath";
			this.labelPhotoPath.Size = new System.Drawing.Size(151, 23);
			this.labelPhotoPath.TabIndex = 10;
			this.labelPhotoPath.Text = "Lateral Photo Location";
			this.labelPhotoPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(490, 288);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 11;
			this.butOk.Text = "&OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(571, 288);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butCephBrowse
			// 
			this.butCephBrowse.Location = new System.Drawing.Point(515, 184);
			this.butCephBrowse.Name = "butCephBrowse";
			this.butCephBrowse.Size = new System.Drawing.Size(27, 20);
			this.butCephBrowse.TabIndex = 13;
			this.butCephBrowse.Text = "...";
			this.butCephBrowse.UseVisualStyleBackColor = true;
			this.butCephBrowse.Click += new System.EventHandler(this.butCephBrowse_Click);
			// 
			// butPhotoBrowse
			// 
			this.butPhotoBrowse.Location = new System.Drawing.Point(515, 226);
			this.butPhotoBrowse.Name = "butPhotoBrowse";
			this.butPhotoBrowse.Size = new System.Drawing.Size(27, 20);
			this.butPhotoBrowse.TabIndex = 14;
			this.butPhotoBrowse.Text = "...";
			this.butPhotoBrowse.UseVisualStyleBackColor = true;
			this.butPhotoBrowse.Click += new System.EventHandler(this.butPhotoBrowse_Click);
			// 
			// textCephLocation
			// 
			this.textCephLocation.Location = new System.Drawing.Point(182, 184);
			this.textCephLocation.Name = "textCephLocation";
			this.textCephLocation.Size = new System.Drawing.Size(318, 20);
			this.textCephLocation.TabIndex = 15;
			// 
			// textPhotoLocation
			// 
			this.textPhotoLocation.Location = new System.Drawing.Point(182, 226);
			this.textPhotoLocation.Name = "textPhotoLocation";
			this.textPhotoLocation.Size = new System.Drawing.Size(318, 20);
			this.textPhotoLocation.TabIndex = 16;
			// 
			// listBoxRace
			// 
			this.listBoxRace.Location = new System.Drawing.Point(182, 54);
			this.listBoxRace.Name = "listBoxRace";
			this.listBoxRace.Size = new System.Drawing.Size(120, 108);
			this.listBoxRace.TabIndex = 17;
			// 
			// listBoxTxPhase
			// 
			this.listBoxTxPhase.Location = new System.Drawing.Point(380, 54);
			this.listBoxTxPhase.Name = "listBoxTxPhase";
			this.listBoxTxPhase.Size = new System.Drawing.Size(120, 108);
			this.listBoxTxPhase.TabIndex = 18;
			// 
			// FormDrCeph
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(667, 328);
			this.Controls.Add(this.listBoxTxPhase);
			this.Controls.Add(this.listBoxRace);
			this.Controls.Add(this.textPhotoLocation);
			this.Controls.Add(this.textCephLocation);
			this.Controls.Add(this.butPhotoBrowse);
			this.Controls.Add(this.butCephBrowse);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.labelPhotoPath);
			this.Controls.Add(this.dateXRay);
			this.Controls.Add(this.labelTxPhase);
			this.Controls.Add(this.labelXRayPath);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.labelRace);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDrCeph";
			this.Text = "FormDrCeph";
			this.Load += new System.EventHandler(this.FormDrCeph_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelRace;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label labelXRayPath;
		private System.Windows.Forms.Label labelTxPhase;
		private System.Windows.Forms.DateTimePicker dateXRay;
		private System.Windows.Forms.Label labelPhotoPath;
		private UI.Button butOk;
		private UI.Button butCancel;
		private UI.Button butCephBrowse;
		private UI.Button butPhotoBrowse;
		private System.Windows.Forms.TextBox textCephLocation;
		private System.Windows.Forms.TextBox textPhotoLocation;
		private UI.ListBoxOD listBoxRace;
		private UI.ListBoxOD listBoxTxPhase;
	}
}