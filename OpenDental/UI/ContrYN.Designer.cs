namespace OpenDental.UI{ 
	///<summary></summary>
	partial class ContrYN {
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
			this.checkY = new System.Windows.Forms.CheckBox();
			this.checkN = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkY
			// 
			this.checkY.AutoSize = true;
			this.checkY.Location = new System.Drawing.Point(0,3);
			this.checkY.Name = "checkY";
			this.checkY.Size = new System.Drawing.Size(44,17);
			this.checkY.TabIndex = 0;
			this.checkY.Text = "Yes";
			this.checkY.UseVisualStyleBackColor = true;
			this.checkY.Click += new System.EventHandler(this.checkY_Click);
			// 
			// checkN
			// 
			this.checkN.AutoSize = true;
			this.checkN.Location = new System.Drawing.Point(55,3);
			this.checkN.Name = "checkN";
			this.checkN.Size = new System.Drawing.Size(40,17);
			this.checkN.TabIndex = 1;
			this.checkN.Text = "No";
			this.checkN.UseVisualStyleBackColor = true;
			this.checkN.Click += new System.EventHandler(this.checkN_Click);
			// 
			// ContrYN
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkN);
			this.Controls.Add(this.checkY);
			this.Name = "ContrYN";
			this.Size = new System.Drawing.Size(231,21);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkY;
		private System.Windows.Forms.CheckBox checkN;
	}
}
