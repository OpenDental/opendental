namespace OpenDental{
	partial class FormImageDrawEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageDrawEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textDrawText = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textLocX = new OpenDental.ValidNum();
			this.textLocY = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.textFontSize = new OpenDental.ValidDouble();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.butColor = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.textFontApparent = new OpenDental.ValidDouble();
			this.butCalculateActual = new OpenDental.UI.Button();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.butCalculateApparent = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.butColorBack = new System.Windows.Forms.Button();
			this.checkTransparent = new OpenDental.UI.CheckBox();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(413, 292);
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
			this.butCancel.Location = new System.Drawing.Point(413, 322);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 322);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDrawText
			// 
			this.textDrawText.Location = new System.Drawing.Point(137, 26);
			this.textDrawText.Name = "textDrawText";
			this.textDrawText.Size = new System.Drawing.Size(281, 20);
			this.textDrawText.TabIndex = 0;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(55, 27);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(81, 18);
			this.label6.TabIndex = 149;
			this.label6.Text = "Text";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(55, 53);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 18);
			this.label1.TabIndex = 151;
			this.label1.Text = "Location X";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLocX
			// 
			this.textLocX.Location = new System.Drawing.Point(137, 53);
			this.textLocX.MaxVal = 100000000;
			this.textLocX.Name = "textLocX";
			this.textLocX.Size = new System.Drawing.Size(42, 20);
			this.textLocX.TabIndex = 153;
			// 
			// textLocY
			// 
			this.textLocY.Location = new System.Drawing.Point(137, 79);
			this.textLocY.MaxVal = 100000000;
			this.textLocY.Name = "textLocY";
			this.textLocY.Size = new System.Drawing.Size(42, 20);
			this.textLocY.TabIndex = 155;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(55, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 18);
			this.label2.TabIndex = 154;
			this.label2.Text = "Location Y";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFontSize
			// 
			this.textFontSize.BackColor = System.Drawing.SystemColors.Window;
			this.textFontSize.Location = new System.Drawing.Point(107, 25);
			this.textFontSize.MaxVal = 100000000D;
			this.textFontSize.MinVal = 1D;
			this.textFontSize.Name = "textFontSize";
			this.textFontSize.Size = new System.Drawing.Size(42, 20);
			this.textFontSize.TabIndex = 156;
			this.textFontSize.Validating += new System.ComponentModel.CancelEventHandler(this.textFontSize_Validating);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 18);
			this.label3.TabIndex = 157;
			this.label3.Text = "Actual Size";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(55, 204);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 18);
			this.label5.TabIndex = 159;
			this.label5.Text = "Text Color";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(137, 203);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 160;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(13, 46);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(93, 36);
			this.label7.TabIndex = 162;
			this.label7.Text = "Apparent Size at Current Zoom";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFontApparent
			// 
			this.textFontApparent.BackColor = System.Drawing.SystemColors.Window;
			this.textFontApparent.Location = new System.Drawing.Point(107, 55);
			this.textFontApparent.MaxVal = 100000000D;
			this.textFontApparent.MinVal = 1D;
			this.textFontApparent.Name = "textFontApparent";
			this.textFontApparent.Size = new System.Drawing.Size(42, 20);
			this.textFontApparent.TabIndex = 161;
			this.textFontApparent.Validating += new System.ComponentModel.CancelEventHandler(this.textFontApparent_Validating);
			// 
			// butCalculateActual
			// 
			this.butCalculateActual.Location = new System.Drawing.Point(170, 53);
			this.butCalculateActual.Name = "butCalculateActual";
			this.butCalculateActual.Size = new System.Drawing.Size(106, 24);
			this.butCalculateActual.TabIndex = 163;
			this.butCalculateActual.Text = "Calculate Actual";
			this.butCalculateActual.Click += new System.EventHandler(this.butCalculateActual_Click);
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.butCalculateApparent);
			this.groupBoxOD1.Controls.Add(this.textFontSize);
			this.groupBoxOD1.Controls.Add(this.butCalculateActual);
			this.groupBoxOD1.Controls.Add(this.label3);
			this.groupBoxOD1.Controls.Add(this.label7);
			this.groupBoxOD1.Controls.Add(this.textFontApparent);
			this.groupBoxOD1.Location = new System.Drawing.Point(30, 106);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(291, 89);
			this.groupBoxOD1.TabIndex = 164;
			this.groupBoxOD1.Text = "Font (decimals are allowed)";
			// 
			// butCalculateApparent
			// 
			this.butCalculateApparent.Location = new System.Drawing.Point(170, 23);
			this.butCalculateApparent.Name = "butCalculateApparent";
			this.butCalculateApparent.Size = new System.Drawing.Size(106, 24);
			this.butCalculateApparent.TabIndex = 164;
			this.butCalculateApparent.Text = "Calculate Apparent";
			this.butCalculateApparent.Click += new System.EventHandler(this.butCalculateApparent_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(30, 230);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 18);
			this.label4.TabIndex = 165;
			this.label4.Text = "Background Color";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorBack
			// 
			this.butColorBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorBack.Location = new System.Drawing.Point(137, 229);
			this.butColorBack.Name = "butColorBack";
			this.butColorBack.Size = new System.Drawing.Size(30, 20);
			this.butColorBack.TabIndex = 166;
			this.butColorBack.Click += new System.EventHandler(this.butColorBack_Click);
			// 
			// checkTransparent
			// 
			this.checkTransparent.Location = new System.Drawing.Point(175, 231);
			this.checkTransparent.Name = "checkTransparent";
			this.checkTransparent.Size = new System.Drawing.Size(166, 18);
			this.checkTransparent.TabIndex = 167;
			this.checkTransparent.Text = "Transparent";
			this.checkTransparent.Click += new System.EventHandler(this.checkTransparent_Click);
			// 
			// FormImageDrawEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(500, 358);
			this.Controls.Add(this.checkTransparent);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butColorBack);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.textLocY);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textLocX);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textDrawText);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageDrawEdit";
			this.Text = "Edit Text";
			this.Load += new System.EventHandler(this.FormImageDrawEdit_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textDrawText;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label1;
		private ValidNum textLocX;
		private ValidNum textLocY;
		private System.Windows.Forms.Label label2;
		private ValidDouble textFontSize;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Label label7;
		private ValidDouble textFontApparent;
		private UI.Button butCalculateActual;
		private UI.GroupBox groupBoxOD1;
		private UI.Button butCalculateApparent;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button butColorBack;
		private OpenDental.UI.CheckBox checkTransparent;
	}
}