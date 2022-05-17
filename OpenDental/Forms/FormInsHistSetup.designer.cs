namespace OpenDental{
	partial class FormInsHistSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsHistSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelHeading = new System.Windows.Forms.Label();
			this.labelExam = new System.Windows.Forms.Label();
			this.textDateExam = new System.Windows.Forms.TextBox();
			this.labelProphy = new System.Windows.Forms.Label();
			this.textDateProphy = new System.Windows.Forms.TextBox();
			this.labelBitewing = new System.Windows.Forms.Label();
			this.textDateBW = new System.Windows.Forms.TextBox();
			this.labelFmxPano = new System.Windows.Forms.Label();
			this.textDateFmxPano = new System.Windows.Forms.TextBox();
			this.labelPerioScalingUR = new System.Windows.Forms.Label();
			this.textDatePerioScalingUR = new System.Windows.Forms.TextBox();
			this.labelPerioScalingUL = new System.Windows.Forms.Label();
			this.textDatePerioScalingUL = new System.Windows.Forms.TextBox();
			this.labelPerioScalingLR = new System.Windows.Forms.Label();
			this.textDatePerioScalingLR = new System.Windows.Forms.TextBox();
			this.labelPerioScalingLL = new System.Windows.Forms.Label();
			this.textDatePerioScalingLL = new System.Windows.Forms.TextBox();
			this.labelPerioMaint = new System.Windows.Forms.Label();
			this.textDatePerioMaint = new System.Windows.Forms.TextBox();
			this.labelDebridgement = new System.Windows.Forms.Label();
			this.textDateDebridgement = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(309, 241);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(309, 271);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 11;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelHeading
			// 
			this.labelHeading.Location = new System.Drawing.Point(12, 9);
			this.labelHeading.Name = "labelHeading";
			this.labelHeading.Size = new System.Drawing.Size(348, 65);
			this.labelHeading.TabIndex = 4;
			this.labelHeading.Text = resources.GetString("labelHeading.Text");
			// 
			// labelExam
			// 
			this.labelExam.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelExam.Location = new System.Drawing.Point(5, 123);
			this.labelExam.Name = "labelExam";
			this.labelExam.Size = new System.Drawing.Size(138, 17);
			this.labelExam.TabIndex = 150;
			this.labelExam.Text = "Exam\r\n";
			this.labelExam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateExam
			// 
			this.textDateExam.Location = new System.Drawing.Point(144, 121);
			this.textDateExam.Name = "textDateExam";
			this.textDateExam.Size = new System.Drawing.Size(151, 20);
			this.textDateExam.TabIndex = 2;
			this.textDateExam.Tag = "InsHistExamCodes";
			this.textDateExam.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelProphy
			// 
			this.labelProphy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProphy.Location = new System.Drawing.Point(5, 145);
			this.labelProphy.Name = "labelProphy";
			this.labelProphy.Size = new System.Drawing.Size(138, 17);
			this.labelProphy.TabIndex = 152;
			this.labelProphy.Text = "Prophy";
			this.labelProphy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateProphy
			// 
			this.textDateProphy.Location = new System.Drawing.Point(144, 143);
			this.textDateProphy.Name = "textDateProphy";
			this.textDateProphy.Size = new System.Drawing.Size(151, 20);
			this.textDateProphy.TabIndex = 3;
			this.textDateProphy.Tag = "InsHistProphyCodes";
			this.textDateProphy.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelBitewing
			// 
			this.labelBitewing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelBitewing.Location = new System.Drawing.Point(5, 79);
			this.labelBitewing.Name = "labelBitewing";
			this.labelBitewing.Size = new System.Drawing.Size(138, 17);
			this.labelBitewing.TabIndex = 154;
			this.labelBitewing.Text = "Bitewing";
			this.labelBitewing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateBW
			// 
			this.textDateBW.Location = new System.Drawing.Point(144, 77);
			this.textDateBW.Name = "textDateBW";
			this.textDateBW.Size = new System.Drawing.Size(151, 20);
			this.textDateBW.TabIndex = 0;
			this.textDateBW.Tag = "InsHistBWCodes";
			this.textDateBW.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelFmxPano
			// 
			this.labelFmxPano.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFmxPano.Location = new System.Drawing.Point(5, 101);
			this.labelFmxPano.Name = "labelFmxPano";
			this.labelFmxPano.Size = new System.Drawing.Size(138, 17);
			this.labelFmxPano.TabIndex = 156;
			this.labelFmxPano.Text = "FMX/Pano";
			this.labelFmxPano.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateFmxPano
			// 
			this.textDateFmxPano.Location = new System.Drawing.Point(144, 99);
			this.textDateFmxPano.Name = "textDateFmxPano";
			this.textDateFmxPano.Size = new System.Drawing.Size(151, 20);
			this.textDateFmxPano.TabIndex = 1;
			this.textDateFmxPano.Tag = "InsHistPanoCodes";
			this.textDateFmxPano.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelPerioScalingUR
			// 
			this.labelPerioScalingUR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerioScalingUR.Location = new System.Drawing.Point(5, 167);
			this.labelPerioScalingUR.Name = "labelPerioScalingUR";
			this.labelPerioScalingUR.Size = new System.Drawing.Size(138, 17);
			this.labelPerioScalingUR.TabIndex = 158;
			this.labelPerioScalingUR.Text = "Perio Scaling UR";
			this.labelPerioScalingUR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatePerioScalingUR
			// 
			this.textDatePerioScalingUR.Location = new System.Drawing.Point(144, 165);
			this.textDatePerioScalingUR.Name = "textDatePerioScalingUR";
			this.textDatePerioScalingUR.Size = new System.Drawing.Size(151, 20);
			this.textDatePerioScalingUR.TabIndex = 4;
			this.textDatePerioScalingUR.Tag = "InsHistPerioURCodes";
			this.textDatePerioScalingUR.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelPerioScalingUL
			// 
			this.labelPerioScalingUL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerioScalingUL.Location = new System.Drawing.Point(5, 189);
			this.labelPerioScalingUL.Name = "labelPerioScalingUL";
			this.labelPerioScalingUL.Size = new System.Drawing.Size(138, 17);
			this.labelPerioScalingUL.TabIndex = 160;
			this.labelPerioScalingUL.Text = "Perio Scaling UL";
			this.labelPerioScalingUL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatePerioScalingUL
			// 
			this.textDatePerioScalingUL.Location = new System.Drawing.Point(144, 187);
			this.textDatePerioScalingUL.Name = "textDatePerioScalingUL";
			this.textDatePerioScalingUL.Size = new System.Drawing.Size(151, 20);
			this.textDatePerioScalingUL.TabIndex = 5;
			this.textDatePerioScalingUL.Tag = "InsHistPerioULCodes";
			this.textDatePerioScalingUL.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelPerioScalingLR
			// 
			this.labelPerioScalingLR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerioScalingLR.Location = new System.Drawing.Point(5, 211);
			this.labelPerioScalingLR.Name = "labelPerioScalingLR";
			this.labelPerioScalingLR.Size = new System.Drawing.Size(138, 17);
			this.labelPerioScalingLR.TabIndex = 162;
			this.labelPerioScalingLR.Text = "Perio Scaling LR";
			this.labelPerioScalingLR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatePerioScalingLR
			// 
			this.textDatePerioScalingLR.Location = new System.Drawing.Point(144, 209);
			this.textDatePerioScalingLR.Name = "textDatePerioScalingLR";
			this.textDatePerioScalingLR.Size = new System.Drawing.Size(151, 20);
			this.textDatePerioScalingLR.TabIndex = 6;
			this.textDatePerioScalingLR.Tag = "InsHistPerioLRCodes";
			this.textDatePerioScalingLR.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelPerioScalingLL
			// 
			this.labelPerioScalingLL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerioScalingLL.Location = new System.Drawing.Point(5, 233);
			this.labelPerioScalingLL.Name = "labelPerioScalingLL";
			this.labelPerioScalingLL.Size = new System.Drawing.Size(138, 17);
			this.labelPerioScalingLL.TabIndex = 164;
			this.labelPerioScalingLL.Text = "Perio Scaling LL";
			this.labelPerioScalingLL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatePerioScalingLL
			// 
			this.textDatePerioScalingLL.Location = new System.Drawing.Point(144, 231);
			this.textDatePerioScalingLL.Name = "textDatePerioScalingLL";
			this.textDatePerioScalingLL.Size = new System.Drawing.Size(151, 20);
			this.textDatePerioScalingLL.TabIndex = 7;
			this.textDatePerioScalingLL.Tag = "InsHistPerioLLCodes";
			this.textDatePerioScalingLL.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelPerioMaint
			// 
			this.labelPerioMaint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerioMaint.Location = new System.Drawing.Point(5, 255);
			this.labelPerioMaint.Name = "labelPerioMaint";
			this.labelPerioMaint.Size = new System.Drawing.Size(138, 17);
			this.labelPerioMaint.TabIndex = 166;
			this.labelPerioMaint.Text = "Perio Maintenance";
			this.labelPerioMaint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatePerioMaint
			// 
			this.textDatePerioMaint.Location = new System.Drawing.Point(144, 253);
			this.textDatePerioMaint.Name = "textDatePerioMaint";
			this.textDatePerioMaint.Size = new System.Drawing.Size(151, 20);
			this.textDatePerioMaint.TabIndex = 8;
			this.textDatePerioMaint.Tag = "InsHistPerioMaintCodes";
			this.textDatePerioMaint.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// labelDebridgement
			// 
			this.labelDebridgement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDebridgement.Location = new System.Drawing.Point(5, 277);
			this.labelDebridgement.Name = "labelDebridgement";
			this.labelDebridgement.Size = new System.Drawing.Size(138, 17);
			this.labelDebridgement.TabIndex = 168;
			this.labelDebridgement.Text = "Full Mouth Debridement";
			this.labelDebridgement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateDebridgement
			// 
			this.textDateDebridgement.Location = new System.Drawing.Point(144, 275);
			this.textDateDebridgement.Name = "textDateDebridgement";
			this.textDateDebridgement.Size = new System.Drawing.Size(151, 20);
			this.textDateDebridgement.TabIndex = 9;
			this.textDateDebridgement.Tag = "InsHistDebridementCodes";
			this.textDateDebridgement.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxValidating);
			// 
			// FormInsHistSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(396, 307);
			this.Controls.Add(this.labelDebridgement);
			this.Controls.Add(this.textDateDebridgement);
			this.Controls.Add(this.labelPerioMaint);
			this.Controls.Add(this.textDatePerioMaint);
			this.Controls.Add(this.labelPerioScalingLL);
			this.Controls.Add(this.textDatePerioScalingLL);
			this.Controls.Add(this.labelPerioScalingLR);
			this.Controls.Add(this.textDatePerioScalingLR);
			this.Controls.Add(this.labelPerioScalingUL);
			this.Controls.Add(this.textDatePerioScalingUL);
			this.Controls.Add(this.labelPerioScalingUR);
			this.Controls.Add(this.textDatePerioScalingUR);
			this.Controls.Add(this.labelFmxPano);
			this.Controls.Add(this.textDateFmxPano);
			this.Controls.Add(this.labelBitewing);
			this.Controls.Add(this.textDateBW);
			this.Controls.Add(this.labelProphy);
			this.Controls.Add(this.textDateProphy);
			this.Controls.Add(this.labelExam);
			this.Controls.Add(this.textDateExam);
			this.Controls.Add(this.labelHeading);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsHistSetup";
			this.Text = "Insurance History";
			this.Load += new System.EventHandler(this.FormInsHistSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelHeading;
		private System.Windows.Forms.Label labelExam;
		private System.Windows.Forms.TextBox textDateExam;
		private System.Windows.Forms.Label labelProphy;
		private System.Windows.Forms.TextBox textDateProphy;
		private System.Windows.Forms.Label labelBitewing;
		private System.Windows.Forms.TextBox textDateBW;
		private System.Windows.Forms.Label labelFmxPano;
		private System.Windows.Forms.TextBox textDateFmxPano;
		private System.Windows.Forms.Label labelPerioScalingUR;
		private System.Windows.Forms.TextBox textDatePerioScalingUR;
		private System.Windows.Forms.Label labelPerioScalingUL;
		private System.Windows.Forms.TextBox textDatePerioScalingUL;
		private System.Windows.Forms.Label labelPerioScalingLR;
		private System.Windows.Forms.TextBox textDatePerioScalingLR;
		private System.Windows.Forms.Label labelPerioScalingLL;
		private System.Windows.Forms.TextBox textDatePerioScalingLL;
		private System.Windows.Forms.Label labelPerioMaint;
		private System.Windows.Forms.TextBox textDatePerioMaint;
		private System.Windows.Forms.Label labelDebridgement;
		private System.Windows.Forms.TextBox textDateDebridgement;
	}
}