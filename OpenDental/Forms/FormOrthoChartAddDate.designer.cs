namespace OpenDental{
	partial class FormOrthoChartAddDate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoChartAddDate));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDate = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.butToday = new OpenDental.UI.Button();
			this.butNow = new OpenDental.UI.Button();
			this.labelOptional = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(239, 130);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(320, 130);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(88, 31);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(131, 20);
			this.textDate.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(18, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(88, 63);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(184, 21);
			this.comboProv.TabIndex = 7;
			this.comboProv.Text = "comboProv";
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(225, 29);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(75, 24);
			this.butToday.TabIndex = 8;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(306, 29);
			this.butNow.Name = "butNow";
			this.butNow.Size = new System.Drawing.Size(75, 24);
			this.butNow.TabIndex = 9;
			this.butNow.Text = "Now";
			this.butNow.Click += new System.EventHandler(this.butNow_Click);
			// 
			// labelOptional
			// 
			this.labelOptional.Location = new System.Drawing.Point(277, 65);
			this.labelOptional.Name = "labelOptional";
			this.labelOptional.Size = new System.Drawing.Size(68, 17);
			this.labelOptional.TabIndex = 10;
			this.labelOptional.Text = "(optional)";
			this.labelOptional.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormOrthoChartAddDate
			// 
			this.ClientSize = new System.Drawing.Size(407, 166);
			this.Controls.Add(this.labelOptional);
			this.Controls.Add(this.butNow);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoChartAddDate";
			this.Text = "Add New Ortho Chart Date";
			this.Load += new System.EventHandler(this.FormOrthoChartAddDate_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private UI.ComboBoxOD comboProv;
		private UI.Button butToday;
		private UI.Button butNow;
		private System.Windows.Forms.Label labelOptional;
	}
}