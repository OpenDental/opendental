namespace OpenDental {
	partial class FormFeatureRequestPrompt {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeatureRequestPrompt));
			this.butAuto = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butManual = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butAuto
			// 
			this.butAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAuto.Location = new System.Drawing.Point(12, 79);
			this.butAuto.Name = "butAuto";
			this.butAuto.Size = new System.Drawing.Size(75, 24);
			this.butAuto.TabIndex = 3;
			this.butAuto.Text = "Auto";
			this.butAuto.Click += new System.EventHandler(this.butAuto_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(294, 79);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(357, 37);
			this.label1.TabIndex = 4;
			this.label1.Text = "There are incomplete feature requests attached to this job.\r\nWould you like to au" +
    "to-complete or complete them manually?\r\n";
			// 
			// butManual
			// 
			this.butManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butManual.Location = new System.Drawing.Point(93, 79);
			this.butManual.Name = "butManual";
			this.butManual.Size = new System.Drawing.Size(75, 24);
			this.butManual.TabIndex = 5;
			this.butManual.Text = "Manual";
			this.butManual.Click += new System.EventHandler(this.butManual_Click);
			// 
			// FormFeatureRequestPrompt
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(381, 115);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butManual);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAuto);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFeatureRequestPrompt";
			this.Text = "";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butAuto;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private UI.Button butManual;
	}
}