namespace OpenDental{
	partial class FormShortCodeOptIn {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShortCodeOptIn));
			this.butClose = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.checkOptIn = new System.Windows.Forms.CheckBox();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(458, 205);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(521, 150);
			this.label1.TabIndex = 8;
			this.label1.Text = "Would you like to receive Appointment Texts?\r\nYou can cancel at any time by texti" +
    "ng STOP to 25975.";
			// 
			// checkOptIn
			// 
			this.checkOptIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkOptIn.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkOptIn.Location = new System.Drawing.Point(12, 210);
			this.checkOptIn.Name = "checkOptIn";
			this.checkOptIn.Size = new System.Drawing.Size(440, 19);
			this.checkOptIn.TabIndex = 0;
			this.checkOptIn.Text = "Patient agrees to receive appointment texts.";
			this.checkOptIn.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkOptIn.UseVisualStyleBackColor = true;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(545, 24);
			this.menuMain.TabIndex = 9;
			// 
			// FormShortCodeOptIn
			// 
			this.ClientSize = new System.Drawing.Size(545, 241);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.checkOptIn);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormShortCodeOptIn";
			this.Text = "Appointment Texts";
			this.Load += new System.EventHandler(this.FormShortCodeOptIn_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkOptIn;
		private UI.MenuOD menuMain;
	}
}