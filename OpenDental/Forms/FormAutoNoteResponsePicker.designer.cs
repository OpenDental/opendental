namespace OpenDental{
	partial class FormAutoNoteResponsePicker {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNoteResponsePicker));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelResponseText = new System.Windows.Forms.Label();
			this.textResponseText = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(363, 440);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(363, 470);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelResponseText
			// 
			this.labelResponseText.Location = new System.Drawing.Point(12, 10);
			this.labelResponseText.Name = "labelResponseText";
			this.labelResponseText.Size = new System.Drawing.Size(92, 18);
			this.labelResponseText.TabIndex = 115;
			this.labelResponseText.Text = "ResponseText";
			this.labelResponseText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textResponseText
			// 
			this.textResponseText.Location = new System.Drawing.Point(106, 9);
			this.textResponseText.Name = "textResponseText";
			this.textResponseText.Size = new System.Drawing.Size(215, 20);
			this.textResponseText.TabIndex = 114;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(25, 35);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(296, 463);
			this.gridMain.TabIndex = 116;
			this.gridMain.Title = "Available Auto Notes";
			this.gridMain.TranslationName = "FormAutoNoteEdit";
			// 
			// FormAutoNoteResponsePicker
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(450, 506);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelResponseText);
			this.Controls.Add(this.textResponseText);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAutoNoteResponsePicker";
			this.Text = "Auto Note Response Picker";
			this.Load += new System.EventHandler(this.FormAutoNoteResponsePicker_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelResponseText;
		private System.Windows.Forms.TextBox textResponseText;
		private UI.GridOD gridMain;
	}
}