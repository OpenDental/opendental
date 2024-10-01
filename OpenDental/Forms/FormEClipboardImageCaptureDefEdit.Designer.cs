namespace OpenDental{
	partial class FormEClipboardImageCaptureDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEClipboardImageCaptureDefEdit));
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboImageCaptureType = new OpenDental.UI.ComboBox();
			this.textFrequency = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(304, 165);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(27, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(316, 29);
			this.label1.TabIndex = 5;
			this.label1.Text = "How often should the patient be prompted to resubmit this image (In days, where 0" +
    " or blank indicates at each checkin)?";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(27, 92);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(213, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Image Capture Type";
			// 
			// comboImageCaptureType
			// 
			this.comboImageCaptureType.Location = new System.Drawing.Point(27, 113);
			this.comboImageCaptureType.Name = "comboImageCaptureType";
			this.comboImageCaptureType.Size = new System.Drawing.Size(259, 21);
			this.comboImageCaptureType.TabIndex = 8;
			this.comboImageCaptureType.Text = "comboBox1";
			// 
			// textFrequency
			// 
			this.textFrequency.Location = new System.Drawing.Point(27, 58);
			this.textFrequency.MaxVal = 32767;
			this.textFrequency.Name = "textFrequency";
			this.textFrequency.ShowZero = false;
			this.textFrequency.Size = new System.Drawing.Size(71, 20);
			this.textFrequency.TabIndex = 9;
			// 
			// FormEClipboardImageCaptureDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(391, 201);
			this.Controls.Add(this.textFrequency);
			this.Controls.Add(this.comboImageCaptureType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEClipboardImageCaptureDefEdit";
			this.Text = "eClipboard Image Capture Def Edit";
			this.Load += new System.EventHandler(this.FormEClipboardImageCaptureDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private UI.ComboBox comboImageCaptureType;
		private ValidNum textFrequency;
	}
}