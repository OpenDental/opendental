namespace OpenDental {
	partial class FormEhrMeasureEventEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrMeasureEventEdit));
			this.textMoreInfo = new System.Windows.Forms.TextBox();
			this.labelDateTime = new System.Windows.Forms.Label();
			this.textType = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelMoreInfo = new System.Windows.Forms.Label();
			this.textResult = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textDateTime = new System.Windows.Forms.TextBox();
			this.labelTobaccoStartDate = new System.Windows.Forms.Label();
			this.textTobaccoDuration = new System.Windows.Forms.TextBox();
			this.textTobaccoDesireToQuit = new OpenDental.ValidNum();
			this.labelTobaccoDesireToQuit = new System.Windows.Forms.Label();
			this.labelTobaccoDesireScale = new System.Windows.Forms.Label();
			this.textTobaccoStartDate = new OpenDental.ValidDate();
			this.SuspendLayout();
			// 
			// textMoreInfo
			// 
			this.textMoreInfo.Location = new System.Drawing.Point(200, 113);
			this.textMoreInfo.Multiline = true;
			this.textMoreInfo.Name = "textMoreInfo";
			this.textMoreInfo.Size = new System.Drawing.Size(317, 55);
			this.textMoreInfo.TabIndex = 5;
			// 
			// labelDateTime
			// 
			this.labelDateTime.Location = new System.Drawing.Point(6, 18);
			this.labelDateTime.Name = "labelDateTime";
			this.labelDateTime.Size = new System.Drawing.Size(190, 17);
			this.labelDateTime.TabIndex = 0;
			this.labelDateTime.Text = "Date Time";
			this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textType
			// 
			this.textType.Location = new System.Drawing.Point(200, 65);
			this.textType.Name = "textType";
			this.textType.ReadOnly = true;
			this.textType.Size = new System.Drawing.Size(317, 20);
			this.textType.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(190, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Event";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMoreInfo
			// 
			this.labelMoreInfo.Location = new System.Drawing.Point(6, 114);
			this.labelMoreInfo.Name = "labelMoreInfo";
			this.labelMoreInfo.Size = new System.Drawing.Size(190, 54);
			this.labelMoreInfo.TabIndex = 0;
			this.labelMoreInfo.Text = "More information about the event";
			this.labelMoreInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textResult
			// 
			this.textResult.Location = new System.Drawing.Point(200, 89);
			this.textResult.Name = "textResult";
			this.textResult.ReadOnly = true;
			this.textResult.Size = new System.Drawing.Size(317, 20);
			this.textResult.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 90);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(190, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Result";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(200, 41);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(151, 20);
			this.textPatient.TabIndex = 2;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(6, 42);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(190, 17);
			this.labelPatient.TabIndex = 0;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(475, 226);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(394, 226);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 24);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(15, 226);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(76, 24);
			this.butDelete.TabIndex = 10;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDateTime
			// 
			this.textDateTime.Location = new System.Drawing.Point(200, 17);
			this.textDateTime.Name = "textDateTime";
			this.textDateTime.Size = new System.Drawing.Size(151, 20);
			this.textDateTime.TabIndex = 1;
			// 
			// labelTobaccoStartDate
			// 
			this.labelTobaccoStartDate.Location = new System.Drawing.Point(6, 173);
			this.labelTobaccoStartDate.Name = "labelTobaccoStartDate";
			this.labelTobaccoStartDate.Size = new System.Drawing.Size(190, 17);
			this.labelTobaccoStartDate.TabIndex = 9;
			this.labelTobaccoStartDate.Text = "Tobacco Use Start Date";
			this.labelTobaccoStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTobaccoStartDate.Visible = false;
			// 
			// textTobaccoDuration
			// 
			this.textTobaccoDuration.Location = new System.Drawing.Point(296, 172);
			this.textTobaccoDuration.Name = "textTobaccoDuration";
			this.textTobaccoDuration.ReadOnly = true;
			this.textTobaccoDuration.Size = new System.Drawing.Size(55, 20);
			this.textTobaccoDuration.TabIndex = 0;
			this.textTobaccoDuration.TabStop = false;
			this.textTobaccoDuration.Visible = false;
			// 
			// textTobaccoDesireToQuit
			// 
			this.textTobaccoDesireToQuit.Location = new System.Drawing.Point(200, 196);
			this.textTobaccoDesireToQuit.MaxVal = 10;
			this.textTobaccoDesireToQuit.MinVal = 0;
			this.textTobaccoDesireToQuit.Name = "textTobaccoDesireToQuit";
			this.textTobaccoDesireToQuit.Size = new System.Drawing.Size(35, 20);
			this.textTobaccoDesireToQuit.TabIndex = 7;
			this.textTobaccoDesireToQuit.Visible = false;
			// 
			// labelTobaccoDesireToQuit
			// 
			this.labelTobaccoDesireToQuit.Location = new System.Drawing.Point(6, 197);
			this.labelTobaccoDesireToQuit.Name = "labelTobaccoDesireToQuit";
			this.labelTobaccoDesireToQuit.Size = new System.Drawing.Size(190, 17);
			this.labelTobaccoDesireToQuit.TabIndex = 14;
			this.labelTobaccoDesireToQuit.Text = "Tobacco Use Desire to Quit";
			this.labelTobaccoDesireToQuit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTobaccoDesireToQuit.Visible = false;
			// 
			// labelTobaccoDesireScale
			// 
			this.labelTobaccoDesireScale.Location = new System.Drawing.Point(239, 197);
			this.labelTobaccoDesireScale.Name = "labelTobaccoDesireScale";
			this.labelTobaccoDesireScale.Size = new System.Drawing.Size(107, 17);
			this.labelTobaccoDesireScale.TabIndex = 15;
			this.labelTobaccoDesireScale.Text = "scale of 1-10";
			this.labelTobaccoDesireScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelTobaccoDesireScale.Visible = false;
			// 
			// textTobaccoStartDate
			// 
			this.textTobaccoStartDate.Location = new System.Drawing.Point(200, 172);
			this.textTobaccoStartDate.Name = "textTobaccoStartDate";
			this.textTobaccoStartDate.Size = new System.Drawing.Size(95, 20);
			this.textTobaccoStartDate.TabIndex = 6;
			this.textTobaccoStartDate.Visible = false;
			this.textTobaccoStartDate.Validated += new System.EventHandler(this.textTobaccoStartDate_Validated);
			// 
			// FormEhrMeasureEventEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(564, 261);
			this.Controls.Add(this.textTobaccoStartDate);
			this.Controls.Add(this.labelTobaccoDesireScale);
			this.Controls.Add(this.labelTobaccoDesireToQuit);
			this.Controls.Add(this.textTobaccoDesireToQuit);
			this.Controls.Add(this.textTobaccoDuration);
			this.Controls.Add(this.labelTobaccoStartDate);
			this.Controls.Add(this.textDateTime);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.textResult);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelMoreInfo);
			this.Controls.Add(this.textType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelDateTime);
			this.Controls.Add(this.textMoreInfo);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrMeasureEventEdit";
			this.Text = "EhrMeasureEvent Edit";
			this.Load += new System.EventHandler(this.FormEhrMeasureEventEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textMoreInfo;
		private System.Windows.Forms.Label labelDateTime;
		private System.Windows.Forms.TextBox textType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelMoreInfo;
		private System.Windows.Forms.TextBox textResult;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label labelPatient;
		private UI.Button butCancel;
		private UI.Button butOK;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textDateTime;
		private System.Windows.Forms.Label labelTobaccoStartDate;
		private System.Windows.Forms.TextBox textTobaccoDuration;
		private ValidNum textTobaccoDesireToQuit;
		private System.Windows.Forms.Label labelTobaccoDesireToQuit;
		private System.Windows.Forms.Label labelTobaccoDesireScale;
		private ValidDate textTobaccoStartDate;
	}
}