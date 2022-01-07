namespace OpenDentalGraph {
	partial class ProductionGraphOptionsCtrl {
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
			this.checkIncludeAdjustments = new System.Windows.Forms.CheckBox();
			this.checkIncludeCompletedProcs = new System.Windows.Forms.CheckBox();
			this.checkIncludeWriteoffs = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkIncludeAdjustments);
			this.groupBox1.Controls.Add(this.checkIncludeCompletedProcs);
			this.groupBox1.Controls.Add(this.checkIncludeWriteoffs);
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(300, 42);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Included Production Sources";
			// 
			// checkIncludeAdjustments
			// 
			this.checkIncludeAdjustments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeAdjustments.Checked = true;
			this.checkIncludeAdjustments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIncludeAdjustments.Location = new System.Drawing.Point(124, 13);
			this.checkIncludeAdjustments.Name = "checkIncludeAdjustments";
			this.checkIncludeAdjustments.Size = new System.Drawing.Size(87, 24);
			this.checkIncludeAdjustments.TabIndex = 3;
			this.checkIncludeAdjustments.Text = "Adjustments";
			this.checkIncludeAdjustments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeAdjustments.UseVisualStyleBackColor = true;
			this.checkIncludeAdjustments.CheckedChanged += new System.EventHandler(this.OnProductionGraphInputsChanged);
			// 
			// checkIncludeCompletedProcs
			// 
			this.checkIncludeCompletedProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeCompletedProcs.Checked = true;
			this.checkIncludeCompletedProcs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIncludeCompletedProcs.Location = new System.Drawing.Point(6, 13);
			this.checkIncludeCompletedProcs.Name = "checkIncludeCompletedProcs";
			this.checkIncludeCompletedProcs.Size = new System.Drawing.Size(112, 24);
			this.checkIncludeCompletedProcs.TabIndex = 5;
			this.checkIncludeCompletedProcs.Text = "Completed Procs";
			this.checkIncludeCompletedProcs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeCompletedProcs.UseVisualStyleBackColor = true;
			this.checkIncludeCompletedProcs.CheckedChanged += new System.EventHandler(this.OnProductionGraphInputsChanged);
			// 
			// checkIncludeWriteoffs
			// 
			this.checkIncludeWriteoffs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeWriteoffs.Checked = true;
			this.checkIncludeWriteoffs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIncludeWriteoffs.Location = new System.Drawing.Point(217, 13);
			this.checkIncludeWriteoffs.Name = "checkIncludeWriteoffs";
			this.checkIncludeWriteoffs.Size = new System.Drawing.Size(70, 24);
			this.checkIncludeWriteoffs.TabIndex = 4;
			this.checkIncludeWriteoffs.Text = "Writeoffs";
			this.checkIncludeWriteoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeWriteoffs.UseVisualStyleBackColor = true;
			this.checkIncludeWriteoffs.CheckedChanged += new System.EventHandler(this.OnProductionGraphInputsChanged);
			// 
			// ProductionGraphOptionsCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "ProductionGraphOptionsCtrl";
			this.Size = new System.Drawing.Size(300, 42);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkIncludeAdjustments;
		private System.Windows.Forms.CheckBox checkIncludeCompletedProcs;
		private System.Windows.Forms.CheckBox checkIncludeWriteoffs;
	}
}
