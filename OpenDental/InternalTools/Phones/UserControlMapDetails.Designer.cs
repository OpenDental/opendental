namespace OpenDental.InternalTools.Phones {
	partial class UserControlMapDetails {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.labelUserName = new System.Windows.Forms.Label();
			this.labelExtensionDesc = new System.Windows.Forms.Label();
			this.labelStatusTime = new System.Windows.Forms.Label();
			this.labelCustomer = new System.Windows.Forms.Label();
			this.odPictureBoxEmployee = new OpenDental.UI.ODPictureBox();
			this.SuspendLayout();
			// 
			// labelUserName
			// 
			this.labelUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUserName.Location = new System.Drawing.Point(179, 1);
			this.labelUserName.Name = "labelUserName";
			this.labelUserName.Size = new System.Drawing.Size(179, 32);
			this.labelUserName.TabIndex = 1;
			this.labelUserName.Text = "Employee";
			this.labelUserName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelExtensionDesc
			// 
			this.labelExtensionDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelExtensionDesc.Location = new System.Drawing.Point(180, 35);
			this.labelExtensionDesc.Name = "labelExtensionDesc";
			this.labelExtensionDesc.Size = new System.Drawing.Size(178, 28);
			this.labelExtensionDesc.TabIndex = 2;
			this.labelExtensionDesc.Text = "x0000";
			this.labelExtensionDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelStatusTime
			// 
			this.labelStatusTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatusTime.Location = new System.Drawing.Point(180, 68);
			this.labelStatusTime.Name = "labelStatusTime";
			this.labelStatusTime.Size = new System.Drawing.Size(178, 28);
			this.labelStatusTime.TabIndex = 3;
			this.labelStatusTime.Text = "Available";
			this.labelStatusTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelCustomer
			// 
			this.labelCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
			this.labelCustomer.Location = new System.Drawing.Point(174, 101);
			this.labelCustomer.Name = "labelCustomer";
			this.labelCustomer.Size = new System.Drawing.Size(184, 28);
			this.labelCustomer.TabIndex = 4;
			this.labelCustomer.Text = "Customer";
			this.labelCustomer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// odPictureBoxEmployee
			// 
			this.odPictureBoxEmployee.HasBorder = false;
			this.odPictureBoxEmployee.Location = new System.Drawing.Point(-2, -1);
			this.odPictureBoxEmployee.Margin = new System.Windows.Forms.Padding(0);
			this.odPictureBoxEmployee.Name = "odPictureBoxEmployee";
			this.odPictureBoxEmployee.Size = new System.Drawing.Size(175, 129);
			this.odPictureBoxEmployee.TabIndex = 0;
			this.odPictureBoxEmployee.Text = "Employee Picture";
			this.odPictureBoxEmployee.TextNullImage = "No Image Available";
			// 
			// UserControlMapDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelCustomer);
			this.Controls.Add(this.labelStatusTime);
			this.Controls.Add(this.labelExtensionDesc);
			this.Controls.Add(this.labelUserName);
			this.Controls.Add(this.odPictureBoxEmployee);
			this.Name = "UserControlMapDetails";
			this.Size = new System.Drawing.Size(358, 130);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.ODPictureBox odPictureBoxEmployee;
		private System.Windows.Forms.Label labelUserName;
		private System.Windows.Forms.Label labelExtensionDesc;
		private System.Windows.Forms.Label labelStatusTime;
		private System.Windows.Forms.Label labelCustomer;
	}
}
