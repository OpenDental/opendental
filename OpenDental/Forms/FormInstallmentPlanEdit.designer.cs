namespace OpenDental{
	partial class FormInstallmentPlanEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInstallmentPlanEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textAPR = new OpenDental.ValidDouble();
			this.label6 = new System.Windows.Forms.Label();
			this.textDateFirstPay = new OpenDental.ValidDate();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateAgreement = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.textMonthlyPayment = new OpenDental.ValidDouble();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(370, 227);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(370, 268);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textAPR
			// 
			this.textAPR.Location = new System.Drawing.Point(131, 92);
			this.textAPR.MaxVal = 100000000D;
			this.textAPR.MinVal = -100000000D;
			this.textAPR.Name = "textAPR";
			this.textAPR.Size = new System.Drawing.Size(47, 20);
			this.textAPR.TabIndex = 3;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(-10, 93);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(138, 17);
			this.label6.TabIndex = 22;
			this.label6.Text = "APR";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateFirstPay
			// 
			this.textDateFirstPay.Location = new System.Drawing.Point(131, 51);
			this.textDateFirstPay.Name = "textDateFirstPay";
			this.textDateFirstPay.Size = new System.Drawing.Size(85, 20);
			this.textDateFirstPay.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(-6, 52);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(135, 17);
			this.label5.TabIndex = 20;
			this.label5.Text = "Date of First Payment";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-4, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(133, 17);
			this.label2.TabIndex = 16;
			this.label2.Text = "Date of Agreement";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateAgreement
			// 
			this.textDateAgreement.Location = new System.Drawing.Point(131, 30);
			this.textDateAgreement.Name = "textDateAgreement";
			this.textDateAgreement.Size = new System.Drawing.Size(85, 20);
			this.textDateAgreement.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-6, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 17);
			this.label1.TabIndex = 24;
			this.label1.Text = "Monthly Payment";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMonthlyPayment
			// 
			this.textMonthlyPayment.Location = new System.Drawing.Point(131, 71);
			this.textMonthlyPayment.MaxVal = 100000000D;
			this.textMonthlyPayment.MinVal = -100000000D;
			this.textMonthlyPayment.Name = "textMonthlyPayment";
			this.textMonthlyPayment.Size = new System.Drawing.Size(85, 20);
			this.textMonthlyPayment.TabIndex = 2;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(131, 114);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(209, 101);
			this.textNote.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(177, 93);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(138, 17);
			this.label3.TabIndex = 22;
			this.label3.Text = "(for example 0 or 18)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(-9, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(138, 17);
			this.label4.TabIndex = 22;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(35, 268);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormInstallmentPlanEdit
			// 
			this.ClientSize = new System.Drawing.Size(470, 319);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textMonthlyPayment);
			this.Controls.Add(this.textAPR);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDateFirstPay);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDateAgreement);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInstallmentPlanEdit";
			this.Text = "Installment Plan";
			this.Load += new System.EventHandler(this.FormInstallmentPlanEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidDouble textAPR;
		private System.Windows.Forms.Label label6;
		private ValidDate textDateFirstPay;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
		private ValidDate textDateAgreement;
		private System.Windows.Forms.Label label1;
		private ValidDouble textMonthlyPayment;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private UI.Button butDelete;
	}
}