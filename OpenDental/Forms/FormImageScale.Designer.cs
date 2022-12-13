namespace OpenDental{
	partial class FormImageScale {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageScale));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelInstructions = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.textKnownLength = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textUnits = new System.Windows.Forms.TextBox();
			this.labelScale = new System.Windows.Forms.Label();
			this.labelUnits = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textPixels = new System.Windows.Forms.TextBox();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butCalculate = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDecimals = new OpenDental.ValidNum();
			this.textScale = new OpenDental.ValidDouble();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(410, 254);
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
			this.butCancel.Location = new System.Drawing.Point(410, 284);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelInstructions
			// 
			this.labelInstructions.Location = new System.Drawing.Point(8, 9);
			this.labelInstructions.Name = "labelInstructions";
			this.labelInstructions.Size = new System.Drawing.Size(486, 18);
			this.labelInstructions.TabIndex = 163;
			this.labelInstructions.Text = "If you don\'t know your scale, then cancel out of this window and click on a line " +
    "of known length.";
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(2, 20);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(107, 18);
			this.label21.TabIndex = 165;
			this.label21.Text = "Known Length";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textKnownLength
			// 
			this.textKnownLength.Location = new System.Drawing.Point(111, 19);
			this.textKnownLength.Name = "textKnownLength";
			this.textKnownLength.Size = new System.Drawing.Size(66, 20);
			this.textKnownLength.TabIndex = 164;
			this.textKnownLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(54, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 18);
			this.label1.TabIndex = 167;
			this.label1.Text = "Optional Units (mm)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUnits
			// 
			this.textUnits.Location = new System.Drawing.Point(176, 43);
			this.textUnits.Name = "textUnits";
			this.textUnits.Size = new System.Drawing.Size(66, 20);
			this.textUnits.TabIndex = 166;
			this.textUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textUnits.Validating += new System.ComponentModel.CancelEventHandler(this.textUnits_Validating);
			// 
			// labelScale
			// 
			this.labelScale.Location = new System.Drawing.Point(62, 214);
			this.labelScale.Name = "labelScale";
			this.labelScale.Size = new System.Drawing.Size(112, 18);
			this.labelScale.TabIndex = 169;
			this.labelScale.Text = "Scale, pixels per mm";
			this.labelScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelUnits
			// 
			this.labelUnits.Location = new System.Drawing.Point(181, 20);
			this.labelUnits.Name = "labelUnits";
			this.labelUnits.Size = new System.Drawing.Size(62, 18);
			this.labelUnits.TabIndex = 170;
			this.labelUnits.Text = "mm";
			this.labelUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(25, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 18);
			this.label4.TabIndex = 172;
			this.label4.Text = "Pixels";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPixels
			// 
			this.textPixels.Location = new System.Drawing.Point(111, 45);
			this.textPixels.Name = "textPixels";
			this.textPixels.ReadOnly = true;
			this.textPixels.Size = new System.Drawing.Size(66, 20);
			this.textPixels.TabIndex = 171;
			this.textPixels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.label2);
			this.groupBoxOD1.Controls.Add(this.butCalculate);
			this.groupBoxOD1.Controls.Add(this.textKnownLength);
			this.groupBoxOD1.Controls.Add(this.label4);
			this.groupBoxOD1.Controls.Add(this.label21);
			this.groupBoxOD1.Controls.Add(this.textPixels);
			this.groupBoxOD1.Controls.Add(this.labelUnits);
			this.groupBoxOD1.Location = new System.Drawing.Point(65, 74);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(246, 124);
			this.groupBoxOD1.TabIndex = 173;
			this.groupBoxOD1.Text = "From clicking on a line";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(230, 18);
			this.label2.TabIndex = 175;
			this.label2.Text = "Or you can manually enter a scale below";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butCalculate
			// 
			this.butCalculate.Location = new System.Drawing.Point(106, 73);
			this.butCalculate.Name = "butCalculate";
			this.butCalculate.Size = new System.Drawing.Size(75, 24);
			this.butCalculate.TabIndex = 174;
			this.butCalculate.Text = "Calculate";
			this.butCalculate.Click += new System.EventHandler(this.butCalculate_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(54, 273);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(226, 35);
			this.label5.TabIndex = 176;
			this.label5.Text = "Once you establish a scale, you can manually assign it to a mount or mount def.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label5.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(62, 240);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 18);
			this.label3.TabIndex = 178;
			this.label3.Text = "Decimal Places";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDecimals
			// 
			this.textDecimals.Location = new System.Drawing.Point(176, 238);
			this.textDecimals.MaxVal = 20;
			this.textDecimals.Name = "textDecimals";
			this.textDecimals.Size = new System.Drawing.Size(66, 20);
			this.textDecimals.TabIndex = 179;
			// 
			// textScale
			// 
			this.textScale.Location = new System.Drawing.Point(176, 214);
			this.textScale.MaxVal = 1000000D;
			this.textScale.MinVal = 1E-06D;
			this.textScale.Name = "textScale";
			this.textScale.Size = new System.Drawing.Size(66, 20);
			this.textScale.TabIndex = 180;
			// 
			// FormImageScale
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(497, 320);
			this.Controls.Add(this.textScale);
			this.Controls.Add(this.textDecimals);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.labelScale);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textUnits);
			this.Controls.Add(this.labelInstructions);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageScale";
			this.Text = "Image Scale";
			this.Load += new System.EventHandler(this.FormImageScale_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelInstructions;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.TextBox textKnownLength;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textUnits;
		private System.Windows.Forms.Label labelScale;
		private System.Windows.Forms.Label labelUnits;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textPixels;
		private UI.GroupBoxOD groupBoxOD1;
		private System.Windows.Forms.Label label2;
		private UI.Button butCalculate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private ValidNum textDecimals;
		private ValidDouble textScale;
	}
}