namespace OpenDentalGraph {
	partial class GroupingOptionsCtrl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioGroupClinics = new System.Windows.Forms.RadioButton();
			this.radioGroupProvs = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioGroupClinics);
			this.groupBox1.Controls.Add(this.radioGroupProvs);
			this.groupBox1.Location = new System.Drawing.Point(5, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(99, 57);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group By";
			// 
			// radioGroupClinics
			// 
			this.radioGroupClinics.Location = new System.Drawing.Point(6, 34);
			this.radioGroupClinics.Name = "radioGroupClinics";
			this.radioGroupClinics.Size = new System.Drawing.Size(89, 17);
			this.radioGroupClinics.TabIndex = 2;
			this.radioGroupClinics.Text = "Clinic";
			this.radioGroupClinics.UseVisualStyleBackColor = true;
			this.radioGroupClinics.CheckedChanged += new System.EventHandler(this.radioGroupByChanged);
			// 
			// radioGroupProvs
			// 
			this.radioGroupProvs.Checked = true;
			this.radioGroupProvs.Location = new System.Drawing.Point(6, 17);
			this.radioGroupProvs.Name = "radioGroupProvs";
			this.radioGroupProvs.Size = new System.Drawing.Size(89, 17);
			this.radioGroupProvs.TabIndex = 0;
			this.radioGroupProvs.TabStop = true;
			this.radioGroupProvs.Text = "Provider";
			this.radioGroupProvs.UseVisualStyleBackColor = true;
			this.radioGroupProvs.CheckedChanged += new System.EventHandler(this.radioGroupByChanged);
			// 
			// GroupingOptionsCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "GroupingOptionsCtrl";
			this.Size = new System.Drawing.Size(108, 63);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioGroupClinics;
		private System.Windows.Forms.RadioButton radioGroupProvs;
	}
}
