namespace OpenDental{
	partial class FormApptBreak {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptBreak));
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioMissed = new System.Windows.Forms.RadioButton();
			this.radioCancelled = new System.Windows.Forms.RadioButton();
			this.butUnsched = new OpenDental.UI.Button();
			this.butPinboard = new OpenDental.UI.Button();
			this.butApptBook = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(123, 158);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioMissed);
			this.groupBox1.Controls.Add(this.radioCancelled);
			this.groupBox1.Location = new System.Drawing.Point(7, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(191, 47);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Broken Procedure Type";
			// 
			// radioMissed
			// 
			this.radioMissed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioMissed.Location = new System.Drawing.Point(6, 19);
			this.radioMissed.Name = "radioMissed";
			this.radioMissed.Size = new System.Drawing.Size(75, 17);
			this.radioMissed.TabIndex = 0;
			this.radioMissed.TabStop = true;
			this.radioMissed.Text = "Missed";
			this.radioMissed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioMissed.UseVisualStyleBackColor = true;
			// 
			// radioCancelled
			// 
			this.radioCancelled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioCancelled.Location = new System.Drawing.Point(87, 19);
			this.radioCancelled.Name = "radioCancelled";
			this.radioCancelled.Size = new System.Drawing.Size(93, 17);
			this.radioCancelled.TabIndex = 1;
			this.radioCancelled.TabStop = true;
			this.radioCancelled.Text = "Cancelled";
			this.radioCancelled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioCancelled.UseVisualStyleBackColor = true;
			// 
			// butUnsched
			// 
			this.butUnsched.Location = new System.Drawing.Point(29, 65);
			this.butUnsched.Name = "butUnsched";
			this.butUnsched.Size = new System.Drawing.Size(143, 23);
			this.butUnsched.TabIndex = 4;
			this.butUnsched.Text = "Send to Unscheduled List";
			this.butUnsched.UseVisualStyleBackColor = true;
			this.butUnsched.Click += new System.EventHandler(this.butUnsched_Click);
			// 
			// butPinboard
			// 
			this.butPinboard.Location = new System.Drawing.Point(29, 94);
			this.butPinboard.Name = "butPinboard";
			this.butPinboard.Size = new System.Drawing.Size(143, 23);
			this.butPinboard.TabIndex = 5;
			this.butPinboard.Text = "Copy to Pinboard";
			this.butPinboard.UseVisualStyleBackColor = true;
			this.butPinboard.Click += new System.EventHandler(this.butPinboard_Click);
			// 
			// butApptBook
			// 
			this.butApptBook.Location = new System.Drawing.Point(29, 123);
			this.butApptBook.Name = "butApptBook";
			this.butApptBook.Size = new System.Drawing.Size(143, 23);
			this.butApptBook.TabIndex = 6;
			this.butApptBook.Text = "Leave in Appt Module";
			this.butApptBook.UseVisualStyleBackColor = true;
			this.butApptBook.Click += new System.EventHandler(this.butApptBook_Click);
			// 
			// FormApptBreak
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(206, 194);
			this.Controls.Add(this.butApptBook);
			this.Controls.Add(this.butPinboard);
			this.Controls.Add(this.butUnsched);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptBreak";
			this.Text = "Broken Appt Options";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormApptBreak_FormClosing);
			this.Load += new System.EventHandler(this.FormApptBreak_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioCancelled;
		private System.Windows.Forms.RadioButton radioMissed;
		private UI.Button butUnsched;
		private UI.Button butPinboard;
		private UI.Button butApptBook;
	}
}