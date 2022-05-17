namespace OpenDental{
	partial class FormZoom {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormZoom));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textResolution1 = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.textScale1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textMax1 = new System.Windows.Forms.TextBox();
			this.textScaleTotal1 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textWorkingArea1 = new System.Windows.Forms.TextBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textZoom = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.butSettings = new OpenDental.UI.Button();
			this.label19 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textMax2 = new System.Windows.Forms.TextBox();
			this.textScaleTotal2 = new System.Windows.Forms.TextBox();
			this.textWorkingArea2 = new System.Windows.Forms.TextBox();
			this.textScale2 = new System.Windows.Forms.TextBox();
			this.textResolution2 = new System.Windows.Forms.TextBox();
			this.textFit2 = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textFit1 = new System.Windows.Forms.TextBox();
			this.textDesign2 = new System.Windows.Forms.TextBox();
			this.butFit = new OpenDental.UI.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.butReset = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(431, 328);
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
			this.butCancel.Location = new System.Drawing.Point(512, 328);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textResolution1
			// 
			this.textResolution1.BackColor = System.Drawing.SystemColors.Control;
			this.textResolution1.Location = new System.Drawing.Point(134, 72);
			this.textResolution1.Name = "textResolution1";
			this.textResolution1.ReadOnly = true;
			this.textResolution1.Size = new System.Drawing.Size(90, 20);
			this.textResolution1.TabIndex = 4;
			this.textResolution1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(54, 73);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(78, 18);
			this.label21.TabIndex = 5;
			this.label21.Text = "Resolution";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textScale1
			// 
			this.textScale1.BackColor = System.Drawing.SystemColors.Control;
			this.textScale1.Location = new System.Drawing.Point(134, 124);
			this.textScale1.Name = "textScale1";
			this.textScale1.ReadOnly = true;
			this.textScale1.Size = new System.Drawing.Size(90, 20);
			this.textScale1.TabIndex = 6;
			this.textScale1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 125);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 18);
			this.label1.TabIndex = 7;
			this.label1.Text = "Microsoft Scale";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(47, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(139, 18);
			this.label2.TabIndex = 8;
			this.label2.Text = "Microsoft Display Settings";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(54, 259);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(78, 18);
			this.label12.TabIndex = 15;
			this.label12.Text = "Maximum Size";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMax1
			// 
			this.textMax1.BackColor = System.Drawing.SystemColors.Control;
			this.textMax1.Location = new System.Drawing.Point(134, 258);
			this.textMax1.Name = "textMax1";
			this.textMax1.ReadOnly = true;
			this.textMax1.Size = new System.Drawing.Size(90, 20);
			this.textMax1.TabIndex = 14;
			this.textMax1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textScaleTotal1
			// 
			this.textScaleTotal1.BackColor = System.Drawing.SystemColors.Control;
			this.textScaleTotal1.Location = new System.Drawing.Point(134, 206);
			this.textScaleTotal1.Name = "textScaleTotal1";
			this.textScaleTotal1.ReadOnly = true;
			this.textScaleTotal1.Size = new System.Drawing.Size(90, 20);
			this.textScaleTotal1.TabIndex = 10;
			this.textScaleTotal1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(54, 207);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(78, 18);
			this.label10.TabIndex = 11;
			this.label10.Text = "Total Scale";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 99);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(84, 18);
			this.label3.TabIndex = 9;
			this.label3.Text = "Working Area";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWorkingArea1
			// 
			this.textWorkingArea1.BackColor = System.Drawing.SystemColors.Control;
			this.textWorkingArea1.Location = new System.Drawing.Point(134, 98);
			this.textWorkingArea1.Name = "textWorkingArea1";
			this.textWorkingArea1.ReadOnly = true;
			this.textWorkingArea1.Size = new System.Drawing.Size(90, 20);
			this.textWorkingArea1.TabIndex = 8;
			this.textWorkingArea1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textBox4
			// 
			this.textBox4.BackColor = System.Drawing.SystemColors.Control;
			this.textBox4.Location = new System.Drawing.Point(134, 232);
			this.textBox4.Name = "textBox4";
			this.textBox4.ReadOnly = true;
			this.textBox4.Size = new System.Drawing.Size(90, 20);
			this.textBox4.TabIndex = 6;
			this.textBox4.Text = "1246 x 735";
			this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(25, 232);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(107, 18);
			this.label6.TabIndex = 7;
			this.label6.Text = "Design Specs";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textZoom
			// 
			this.textZoom.Location = new System.Drawing.Point(199, 150);
			this.textZoom.Name = "textZoom";
			this.textZoom.Size = new System.Drawing.Size(56, 20);
			this.textZoom.TabIndex = 10;
			this.textZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textZoom.TextChanged += new System.EventHandler(this.textZoom_TextChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(97, 151);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 18);
			this.label8.TabIndex = 11;
			this.label8.Text = "Additional Scale";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(259, 151);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(198, 18);
			this.label9.TabIndex = 12;
			this.label9.Text = "Only applies to this computer.";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSettings
			// 
			this.butSettings.Location = new System.Drawing.Point(190, 11);
			this.butSettings.Name = "butSettings";
			this.butSettings.Size = new System.Drawing.Size(75, 24);
			this.butSettings.TabIndex = 13;
			this.butSettings.Text = "Settings";
			this.butSettings.Click += new System.EventHandler(this.butSettings_Click);
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(324, 232);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(276, 18);
			this.label19.TabIndex = 12;
			this.label19.Text = "Maximum size of window that our engineers will design";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(140, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 18);
			this.label4.TabIndex = 16;
			this.label4.Text = "Monitor 1";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(236, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 18);
			this.label5.TabIndex = 23;
			this.label5.Text = "Monitor 2";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textMax2
			// 
			this.textMax2.BackColor = System.Drawing.SystemColors.Control;
			this.textMax2.Location = new System.Drawing.Point(230, 258);
			this.textMax2.Name = "textMax2";
			this.textMax2.ReadOnly = true;
			this.textMax2.Size = new System.Drawing.Size(90, 20);
			this.textMax2.TabIndex = 22;
			this.textMax2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textScaleTotal2
			// 
			this.textScaleTotal2.BackColor = System.Drawing.SystemColors.Control;
			this.textScaleTotal2.Location = new System.Drawing.Point(230, 206);
			this.textScaleTotal2.Name = "textScaleTotal2";
			this.textScaleTotal2.ReadOnly = true;
			this.textScaleTotal2.Size = new System.Drawing.Size(90, 20);
			this.textScaleTotal2.TabIndex = 21;
			this.textScaleTotal2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textWorkingArea2
			// 
			this.textWorkingArea2.BackColor = System.Drawing.SystemColors.Control;
			this.textWorkingArea2.Location = new System.Drawing.Point(230, 98);
			this.textWorkingArea2.Name = "textWorkingArea2";
			this.textWorkingArea2.ReadOnly = true;
			this.textWorkingArea2.Size = new System.Drawing.Size(90, 20);
			this.textWorkingArea2.TabIndex = 19;
			this.textWorkingArea2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textScale2
			// 
			this.textScale2.BackColor = System.Drawing.SystemColors.Control;
			this.textScale2.Location = new System.Drawing.Point(230, 124);
			this.textScale2.Name = "textScale2";
			this.textScale2.ReadOnly = true;
			this.textScale2.Size = new System.Drawing.Size(90, 20);
			this.textScale2.TabIndex = 18;
			this.textScale2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textResolution2
			// 
			this.textResolution2.BackColor = System.Drawing.SystemColors.Control;
			this.textResolution2.Location = new System.Drawing.Point(230, 72);
			this.textResolution2.Name = "textResolution2";
			this.textResolution2.ReadOnly = true;
			this.textResolution2.Size = new System.Drawing.Size(90, 20);
			this.textResolution2.TabIndex = 17;
			this.textResolution2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textFit2
			// 
			this.textFit2.BackColor = System.Drawing.SystemColors.Control;
			this.textFit2.Location = new System.Drawing.Point(230, 284);
			this.textFit2.Name = "textFit2";
			this.textFit2.ReadOnly = true;
			this.textFit2.Size = new System.Drawing.Size(90, 20);
			this.textFit2.TabIndex = 26;
			this.textFit2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(54, 285);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 18);
			this.label7.TabIndex = 25;
			this.label7.Text = "Fit";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFit1
			// 
			this.textFit1.BackColor = System.Drawing.SystemColors.Control;
			this.textFit1.Location = new System.Drawing.Point(134, 284);
			this.textFit1.Name = "textFit1";
			this.textFit1.ReadOnly = true;
			this.textFit1.Size = new System.Drawing.Size(90, 20);
			this.textFit1.TabIndex = 24;
			this.textFit1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textDesign2
			// 
			this.textDesign2.BackColor = System.Drawing.SystemColors.Control;
			this.textDesign2.Location = new System.Drawing.Point(230, 232);
			this.textDesign2.Name = "textDesign2";
			this.textDesign2.ReadOnly = true;
			this.textDesign2.Size = new System.Drawing.Size(90, 20);
			this.textDesign2.TabIndex = 27;
			this.textDesign2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// butFit
			// 
			this.butFit.Location = new System.Drawing.Point(230, 176);
			this.butFit.Name = "butFit";
			this.butFit.Size = new System.Drawing.Size(56, 24);
			this.butFit.TabIndex = 28;
			this.butFit.Text = "Fit";
			this.butFit.Click += new System.EventHandler(this.butFit_Click);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(290, 179);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(198, 18);
			this.label11.TabIndex = 29;
			this.label11.Text = "Calculates a scale that will make it fit";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(271, 15);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(228, 18);
			this.label13.TabIndex = 30;
			this.label13.Text = "If you make changes, reopen this window";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(324, 100);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(104, 18);
			this.label14.TabIndex = 31;
			this.label14.Text = "Excludes taskbar";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(324, 125);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(185, 18);
			this.label15.TabIndex = 32;
			this.label15.Text = "Use this for most scaling";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butReset
			// 
			this.butReset.Location = new System.Drawing.Point(168, 176);
			this.butReset.Name = "butReset";
			this.butReset.Size = new System.Drawing.Size(56, 24);
			this.butReset.TabIndex = 33;
			this.butReset.Text = "Reset";
			this.butReset.Click += new System.EventHandler(this.butReset_Click);
			// 
			// FormZoom
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(599, 364);
			this.Controls.Add(this.butReset);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.butFit);
			this.Controls.Add(this.textDesign2);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.textFit2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textFit1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textMax2);
			this.Controls.Add(this.textScaleTotal2);
			this.Controls.Add(this.textWorkingArea2);
			this.Controls.Add(this.textScale2);
			this.Controls.Add(this.textResolution2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.butSettings);
			this.Controls.Add(this.textMax1);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textZoom);
			this.Controls.Add(this.textScaleTotal1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textWorkingArea1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textScale1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.textResolution1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormZoom";
			this.Text = "Zoom";
			this.Load += new System.EventHandler(this.FormZoom_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textResolution1;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.TextBox textScale1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textMax1;
		private System.Windows.Forms.TextBox textScaleTotal1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textWorkingArea1;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textZoom;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private UI.Button butSettings;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textMax2;
		private System.Windows.Forms.TextBox textScaleTotal2;
		private System.Windows.Forms.TextBox textWorkingArea2;
		private System.Windows.Forms.TextBox textScale2;
		private System.Windows.Forms.TextBox textResolution2;
		private System.Windows.Forms.TextBox textFit2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textFit1;
		private System.Windows.Forms.TextBox textDesign2;
		private UI.Button butFit;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private UI.Button butReset;
	}
}