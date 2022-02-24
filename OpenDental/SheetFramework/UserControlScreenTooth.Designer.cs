namespace OpenDental {
	partial class UserControlScreenTooth {
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
			this.sheetComboBox3 = new OpenDental.SheetComboBox();
			this.sheetComboBox2 = new OpenDental.SheetComboBox();
			this.sheetComboBox1 = new OpenDental.SheetComboBox();
			this.SuspendLayout();
			// 
			// sheetComboBox3
			// 
			this.sheetComboBox3.Location = new System.Drawing.Point(0, 22);
			this.sheetComboBox3.Name = "sheetComboBox3";
			this.sheetComboBox3.Size = new System.Drawing.Size(90, 21);
			this.sheetComboBox3.TabIndex = 2;
			this.sheetComboBox3.Text = "sheetComboBox3";
			this.sheetComboBox3.ToothChart = true;
			// 
			// sheetComboBox2
			// 
			this.sheetComboBox2.Location = new System.Drawing.Point(45, 0);
			this.sheetComboBox2.Name = "sheetComboBox2";
			this.sheetComboBox2.Size = new System.Drawing.Size(45, 23);
			this.sheetComboBox2.TabIndex = 1;
			this.sheetComboBox2.Text = "sheetComboBox2";
			this.sheetComboBox2.ToothChart = true;
			// 
			// sheetComboBox1
			// 
			this.sheetComboBox1.Location = new System.Drawing.Point(0, 0);
			this.sheetComboBox1.Name = "sheetComboBox1";
			this.sheetComboBox1.Size = new System.Drawing.Size(45, 23);
			this.sheetComboBox1.TabIndex = 0;
			this.sheetComboBox1.Text = "sheetComboBox1";
			this.sheetComboBox1.ToothChart = true;
			// 
			// UserControlTooth
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.sheetComboBox3);
			this.Controls.Add(this.sheetComboBox2);
			this.Controls.Add(this.sheetComboBox1);
			this.Name = "UserControlTooth";
			this.Size = new System.Drawing.Size(90, 43);
			this.Load += new System.EventHandler(this.UserControlTooth_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private SheetComboBox sheetComboBox1;
		private SheetComboBox sheetComboBox2;
		private SheetComboBox sheetComboBox3;
	}
}
