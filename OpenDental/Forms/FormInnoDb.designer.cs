namespace OpenDental{
	partial class FormInnoDb {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInnoDb));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butToMyIsam = new OpenDental.UI.Button();
			this.butToInnoDb = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(539,500);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(625,500);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Font = new System.Drawing.Font("Courier New",8.25F);
			this.textBox1.Location = new System.Drawing.Point(21,32);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(679,456);
			this.textBox1.TabIndex = 18;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14,9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(507,20);
			this.label1.TabIndex = 19;
			this.label1.Text = "This tool will convert all tables in the database to the selected storage engine." +
    "";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butToMyIsam
			// 
			this.butToMyIsam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butToMyIsam.Location = new System.Drawing.Point(21,500);
			this.butToMyIsam.Name = "butToMyIsam";
			this.butToMyIsam.Size = new System.Drawing.Size(104,24);
			this.butToMyIsam.TabIndex = 20;
			this.butToMyIsam.Text = "Convert to MyIsam";
			this.butToMyIsam.Click += new System.EventHandler(this.butToMyIsam_Click);
			// 
			// butToInnoDb
			// 
			this.butToInnoDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butToInnoDb.Location = new System.Drawing.Point(136,500);
			this.butToInnoDb.Name = "butToInnoDb";
			this.butToInnoDb.Size = new System.Drawing.Size(104,24);
			this.butToInnoDb.TabIndex = 21;
			this.butToInnoDb.Text = "Convert to InnoDb";
			this.butToInnoDb.Click += new System.EventHandler(this.butToInnoDb_Click);
			// 
			// FormInnoDb
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(725,534);
			this.Controls.Add(this.butToInnoDb);
			this.Controls.Add(this.butToMyIsam);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInnoDb";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Convert Database Storage Engine";
			this.Load += new System.EventHandler(this.FormInnoDb_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private UI.Button butToMyIsam;
		private UI.Button butToInnoDb;
	}
}