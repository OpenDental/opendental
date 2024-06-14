namespace OpenDental{
	partial class FormBlockoutDuplicatesFix {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBlockoutDuplicatesFix));
			this.label1 = new System.Windows.Forms.Label();
			this.labelCount = new System.Windows.Forms.Label();
			this.labelInstructions = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(175, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "Duplicate blockouts found:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCount
			// 
			this.labelCount.Location = new System.Drawing.Point(186, 45);
			this.labelCount.Name = "labelCount";
			this.labelCount.Size = new System.Drawing.Size(76, 20);
			this.labelCount.TabIndex = 5;
			this.labelCount.Text = "100000";
			this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelInstructions
			// 
			this.labelInstructions.Location = new System.Drawing.Point(48, 82);
			this.labelInstructions.Name = "labelInstructions";
			this.labelInstructions.Size = new System.Drawing.Size(325, 41);
			this.labelInstructions.TabIndex = 6;
			this.labelInstructions.Text = "Click the Clear button to fix the duplicates.  You should not have to run this to" +
    "ol again in the future.";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(48, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(375, 20);
			this.label4.TabIndex = 7;
			this.label4.Text = "Duplicate blockouts can cause slowness in the Appointment module.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(187, 148);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "Clear";
			this.butOK.Click += new System.EventHandler(this.butClear_Click);
			// 
			// FormBlockoutDuplicatesFix
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(424, 196);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelInstructions);
			this.Controls.Add(this.labelCount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBlockoutDuplicatesFix";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Clear Duplicate Blockouts";
			this.Load += new System.EventHandler(this.FormBlockoutDuplicatesFix_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCount;
		private System.Windows.Forms.Label labelInstructions;
		private System.Windows.Forms.Label label4;
	}
}