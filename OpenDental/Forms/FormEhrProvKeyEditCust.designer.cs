namespace OpenDental{
	partial class FormEhrProvKeyEditCust {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrProvKeyEditCust));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textLName = new System.Windows.Forms.TextBox();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textEhrKey = new System.Windows.Forms.TextBox();
			this.labelEhrKey = new System.Windows.Forms.Label();
			this.butGenerate = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textFullTimeEquiv = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textNotes = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textCalYear = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(511, 414);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(606, 414);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(188, 124);
			this.textLName.MaxLength = 100;
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(161, 20);
			this.textLName.TabIndex = 115;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(188, 147);
			this.textFName.MaxLength = 100;
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(161, 20);
			this.textFName.TabIndex = 116;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(54, 128);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(132, 14);
			this.label10.TabIndex = 118;
			this.label10.Text = "Prov Last Name";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(60, 151);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(127, 14);
			this.label8.TabIndex = 117;
			this.label8.Text = "Prov First Name";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(78, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(422, 89);
			this.label1.TabIndex = 114;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// textEhrKey
			// 
			this.textEhrKey.Location = new System.Drawing.Point(188, 190);
			this.textEhrKey.MaxLength = 15;
			this.textEhrKey.Name = "textEhrKey";
			this.textEhrKey.Size = new System.Drawing.Size(161, 20);
			this.textEhrKey.TabIndex = 112;
			// 
			// labelEhrKey
			// 
			this.labelEhrKey.Location = new System.Drawing.Point(49, 194);
			this.labelEhrKey.Name = "labelEhrKey";
			this.labelEhrKey.Size = new System.Drawing.Size(139, 14);
			this.labelEhrKey.TabIndex = 113;
			this.labelEhrKey.Text = "Provider EHR Key";
			this.labelEhrKey.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butGenerate
			// 
			this.butGenerate.Location = new System.Drawing.Point(355, 189);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(75, 24);
			this.butGenerate.TabIndex = 119;
			this.butGenerate.Text = "Generate";
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(30, 414);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 120;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textFullTimeEquiv
			// 
			this.textFullTimeEquiv.Location = new System.Drawing.Point(188, 213);
			this.textFullTimeEquiv.MaxLength = 15;
			this.textFullTimeEquiv.Name = "textFullTimeEquiv";
			this.textFullTimeEquiv.Size = new System.Drawing.Size(46, 20);
			this.textFullTimeEquiv.TabIndex = 122;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 217);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(176, 14);
			this.label2.TabIndex = 123;
			this.label2.Text = "FTE (Full-Time Equivalent)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNotes
			// 
			this.textNotes.Location = new System.Drawing.Point(188, 252);
			this.textNotes.MaxLength = 30000;
			this.textNotes.Multiline = true;
			this.textNotes.Name = "textNotes";
			this.textNotes.Size = new System.Drawing.Size(319, 92);
			this.textNotes.TabIndex = 124;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(100, 256);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 14);
			this.label3.TabIndex = 125;
			this.label3.Text = "Notes";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(238, 217);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(463, 32);
			this.label4.TabIndex = 126;
			this.label4.Text = "Usually 1. For example, half-time would be .5 and 1 day a week would be about .25" +
    "\r\n0 may be used if the provider is no longer working";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(60, 172);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(127, 14);
			this.label5.TabIndex = 128;
			this.label5.Text = "Year";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(240, 172);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(127, 14);
			this.label6.TabIndex = 129;
			this.label6.Text = "Ex. 12";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCalYear
			// 
			this.textCalYear.Location = new System.Drawing.Point(188, 169);
			this.textCalYear.MaxVal = 99;
			this.textCalYear.MinVal = 0;
			this.textCalYear.Name = "textCalYear";
			this.textCalYear.Size = new System.Drawing.Size(46, 20);
			this.textCalYear.TabIndex = 130;
			this.textCalYear.ShowZero = false;
			// 
			// FormEhrProvKeyEditCust
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(706, 458);
			this.Controls.Add(this.textCalYear);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textFullTimeEquiv);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.textLName);
			this.Controls.Add(this.textFName);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textEhrKey);
			this.Controls.Add(this.labelEhrKey);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrProvKeyEditCust";
			this.Text = "Edit Provider Key";
			this.Load += new System.EventHandler(this.FormEhrProvKeyEditCust_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textEhrKey;
		private System.Windows.Forms.Label labelEhrKey;
		private UI.Button butGenerate;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textFullTimeEquiv;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textNotes;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private ValidNum textCalYear;
	}
}