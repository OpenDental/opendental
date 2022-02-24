namespace OpenDental {
	public partial class FormProcSelect {
		#region Designer Variables
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupCreditLogic;
		private System.Windows.Forms.RadioButton radioExcludeAllCredits;
		private System.Windows.Forms.RadioButton radioOnlyAllocatedCredits;
		private System.Windows.Forms.RadioButton radioIncludeAllCredits;
		private System.Windows.Forms.GroupBox groupBreakdown;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label labelCurrentSplits;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label labelAmtEnd;
		private System.Windows.Forms.Label labelTitleWriteOffEst;
		private System.Windows.Forms.Label labelWriteOffEst;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labelWriteOff;
		private System.Windows.Forms.Label labelTitleInsEst;
		private System.Windows.Forms.Label labelInsEst;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelPaySplits;
		private System.Windows.Forms.Label labelAmtOriginal;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelPositiveAdjs;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelInsPay;
		private System.Windows.Forms.Label labelNegativeAdjs;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelPayPlanCredits;
		private System.Windows.Forms.Label label5;
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcSelect));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupCreditLogic = new System.Windows.Forms.GroupBox();
			this.radioExcludeAllCredits = new System.Windows.Forms.RadioButton();
			this.radioOnlyAllocatedCredits = new System.Windows.Forms.RadioButton();
			this.radioIncludeAllCredits = new System.Windows.Forms.RadioButton();
			this.groupBreakdown = new System.Windows.Forms.GroupBox();
			this.label12 = new System.Windows.Forms.Label();
			this.labelCurrentSplits = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.labelAmtEnd = new System.Windows.Forms.Label();
			this.labelTitleWriteOffEst = new System.Windows.Forms.Label();
			this.labelWriteOffEst = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.labelWriteOff = new System.Windows.Forms.Label();
			this.labelTitleInsEst = new System.Windows.Forms.Label();
			this.labelInsEst = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.labelPaySplits = new System.Windows.Forms.Label();
			this.labelAmtOriginal = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelPositiveAdjs = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelInsPay = new System.Windows.Forms.Label();
			this.labelNegativeAdjs = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.labelPayPlanCredits = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelUnallocated = new System.Windows.Forms.Label();
			this.groupCreditLogic.SuspendLayout();
			this.groupBreakdown.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(15, 76);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(675, 503);
			this.gridMain.TabIndex = 140;
			this.gridMain.Title = "Procedures";
			this.gridMain.TranslationName = "TableProcSelect";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(705, 553);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(786, 553);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupCreditLogic
			// 
			this.groupCreditLogic.Controls.Add(this.radioExcludeAllCredits);
			this.groupCreditLogic.Controls.Add(this.radioOnlyAllocatedCredits);
			this.groupCreditLogic.Controls.Add(this.radioIncludeAllCredits);
			this.groupCreditLogic.Location = new System.Drawing.Point(15, 1);
			this.groupCreditLogic.Name = "groupCreditLogic";
			this.groupCreditLogic.Size = new System.Drawing.Size(331, 73);
			this.groupCreditLogic.TabIndex = 143;
			this.groupCreditLogic.TabStop = false;
			this.groupCreditLogic.Text = "Credit Filter";
			// 
			// radioExcludeAllCredits
			// 
			this.radioExcludeAllCredits.Location = new System.Drawing.Point(20, 49);
			this.radioExcludeAllCredits.Name = "radioExcludeAllCredits";
			this.radioExcludeAllCredits.Size = new System.Drawing.Size(305, 17);
			this.radioExcludeAllCredits.TabIndex = 2;
			this.radioExcludeAllCredits.TabStop = true;
			this.radioExcludeAllCredits.Text = "Exclude all credits";
			this.radioExcludeAllCredits.UseVisualStyleBackColor = true;
			this.radioExcludeAllCredits.Click += new System.EventHandler(this.radioCreditCalc_Click);
			// 
			// radioOnlyAllocatedCredits
			// 
			this.radioOnlyAllocatedCredits.Checked = true;
			this.radioOnlyAllocatedCredits.Location = new System.Drawing.Point(20, 15);
			this.radioOnlyAllocatedCredits.Name = "radioOnlyAllocatedCredits";
			this.radioOnlyAllocatedCredits.Size = new System.Drawing.Size(305, 17);
			this.radioOnlyAllocatedCredits.TabIndex = 0;
			this.radioOnlyAllocatedCredits.TabStop = true;
			this.radioOnlyAllocatedCredits.Text = "Only allocated credits";
			this.radioOnlyAllocatedCredits.UseVisualStyleBackColor = true;
			this.radioOnlyAllocatedCredits.Click += new System.EventHandler(this.radioCreditCalc_Click);
			// 
			// radioIncludeAllCredits
			// 
			this.radioIncludeAllCredits.Location = new System.Drawing.Point(20, 32);
			this.radioIncludeAllCredits.Name = "radioIncludeAllCredits";
			this.radioIncludeAllCredits.Size = new System.Drawing.Size(305, 17);
			this.radioIncludeAllCredits.TabIndex = 1;
			this.radioIncludeAllCredits.TabStop = true;
			this.radioIncludeAllCredits.Text = "Include all credits";
			this.radioIncludeAllCredits.UseVisualStyleBackColor = true;
			this.radioIncludeAllCredits.Click += new System.EventHandler(this.radioCreditCalc_Click);
			// 
			// groupBreakdown
			// 
			this.groupBreakdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBreakdown.Controls.Add(this.label12);
			this.groupBreakdown.Controls.Add(this.labelCurrentSplits);
			this.groupBreakdown.Controls.Add(this.label8);
			this.groupBreakdown.Controls.Add(this.label9);
			this.groupBreakdown.Controls.Add(this.labelAmtEnd);
			this.groupBreakdown.Controls.Add(this.labelTitleWriteOffEst);
			this.groupBreakdown.Controls.Add(this.labelWriteOffEst);
			this.groupBreakdown.Controls.Add(this.label10);
			this.groupBreakdown.Controls.Add(this.labelWriteOff);
			this.groupBreakdown.Controls.Add(this.labelTitleInsEst);
			this.groupBreakdown.Controls.Add(this.labelInsEst);
			this.groupBreakdown.Controls.Add(this.label1);
			this.groupBreakdown.Controls.Add(this.labelPaySplits);
			this.groupBreakdown.Controls.Add(this.labelAmtOriginal);
			this.groupBreakdown.Controls.Add(this.label2);
			this.groupBreakdown.Controls.Add(this.labelPositiveAdjs);
			this.groupBreakdown.Controls.Add(this.label13);
			this.groupBreakdown.Controls.Add(this.label3);
			this.groupBreakdown.Controls.Add(this.labelInsPay);
			this.groupBreakdown.Controls.Add(this.labelNegativeAdjs);
			this.groupBreakdown.Controls.Add(this.label7);
			this.groupBreakdown.Controls.Add(this.labelPayPlanCredits);
			this.groupBreakdown.Controls.Add(this.label5);
			this.groupBreakdown.Location = new System.Drawing.Point(694, 76);
			this.groupBreakdown.Name = "groupBreakdown";
			this.groupBreakdown.Size = new System.Drawing.Size(167, 270);
			this.groupBreakdown.TabIndex = 144;
			this.groupBreakdown.TabStop = false;
			this.groupBreakdown.Text = "Breakdown";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(7, 216);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(86, 18);
			this.label12.TabIndex = 152;
			this.label12.Text = "Current Splits:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCurrentSplits
			// 
			this.labelCurrentSplits.Location = new System.Drawing.Point(99, 216);
			this.labelCurrentSplits.Name = "labelCurrentSplits";
			this.labelCurrentSplits.Size = new System.Drawing.Size(61, 18);
			this.labelCurrentSplits.TabIndex = 151;
			this.labelCurrentSplits.Text = "0.00";
			this.labelCurrentSplits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label8.Location = new System.Drawing.Point(5, 239);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(155, 2);
			this.label8.TabIndex = 150;
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(7, 245);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(86, 18);
			this.label9.TabIndex = 149;
			this.label9.Text = "Amt End:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAmtEnd
			// 
			this.labelAmtEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAmtEnd.Location = new System.Drawing.Point(99, 245);
			this.labelAmtEnd.Name = "labelAmtEnd";
			this.labelAmtEnd.Size = new System.Drawing.Size(61, 18);
			this.labelAmtEnd.TabIndex = 148;
			this.labelAmtEnd.Text = "0.00";
			this.labelAmtEnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTitleWriteOffEst
			// 
			this.labelTitleWriteOffEst.Location = new System.Drawing.Point(7, 194);
			this.labelTitleWriteOffEst.Name = "labelTitleWriteOffEst";
			this.labelTitleWriteOffEst.Size = new System.Drawing.Size(86, 18);
			this.labelTitleWriteOffEst.TabIndex = 147;
			this.labelTitleWriteOffEst.Text = "WriteOff Ests:";
			this.labelTitleWriteOffEst.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWriteOffEst
			// 
			this.labelWriteOffEst.Location = new System.Drawing.Point(99, 194);
			this.labelWriteOffEst.Name = "labelWriteOffEst";
			this.labelWriteOffEst.Size = new System.Drawing.Size(61, 18);
			this.labelWriteOffEst.TabIndex = 146;
			this.labelWriteOffEst.Text = "0.00";
			this.labelWriteOffEst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(7, 150);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(86, 18);
			this.label10.TabIndex = 145;
			this.label10.Text = "WriteOffs:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWriteOff
			// 
			this.labelWriteOff.Location = new System.Drawing.Point(99, 150);
			this.labelWriteOff.Name = "labelWriteOff";
			this.labelWriteOff.Size = new System.Drawing.Size(61, 18);
			this.labelWriteOff.TabIndex = 144;
			this.labelWriteOff.Text = "0.00";
			this.labelWriteOff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTitleInsEst
			// 
			this.labelTitleInsEst.Location = new System.Drawing.Point(7, 172);
			this.labelTitleInsEst.Name = "labelTitleInsEst";
			this.labelTitleInsEst.Size = new System.Drawing.Size(86, 18);
			this.labelTitleInsEst.TabIndex = 23;
			this.labelTitleInsEst.Text = "Ins Ests:";
			this.labelTitleInsEst.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsEst
			// 
			this.labelInsEst.Location = new System.Drawing.Point(99, 172);
			this.labelInsEst.Name = "labelInsEst";
			this.labelInsEst.Size = new System.Drawing.Size(61, 18);
			this.labelInsEst.TabIndex = 22;
			this.labelInsEst.Text = "0.00";
			this.labelInsEst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(7, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 18);
			this.label1.TabIndex = 27;
			this.label1.Text = "Amt Original:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPaySplits
			// 
			this.labelPaySplits.Location = new System.Drawing.Point(99, 106);
			this.labelPaySplits.Name = "labelPaySplits";
			this.labelPaySplits.Size = new System.Drawing.Size(61, 18);
			this.labelPaySplits.TabIndex = 14;
			this.labelPaySplits.Text = "0.00";
			this.labelPaySplits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAmtOriginal
			// 
			this.labelAmtOriginal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAmtOriginal.Location = new System.Drawing.Point(99, 18);
			this.labelAmtOriginal.Name = "labelAmtOriginal";
			this.labelAmtOriginal.Size = new System.Drawing.Size(61, 18);
			this.labelAmtOriginal.TabIndex = 26;
			this.labelAmtOriginal.Text = "0.00";
			this.labelAmtOriginal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 106);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 18);
			this.label2.TabIndex = 15;
			this.label2.Text = "PaySplits:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPositiveAdjs
			// 
			this.labelPositiveAdjs.Location = new System.Drawing.Point(99, 40);
			this.labelPositiveAdjs.Name = "labelPositiveAdjs";
			this.labelPositiveAdjs.Size = new System.Drawing.Size(61, 18);
			this.labelPositiveAdjs.TabIndex = 16;
			this.labelPositiveAdjs.Text = "0.00";
			this.labelPositiveAdjs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(7, 128);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(86, 18);
			this.label13.TabIndex = 25;
			this.label13.Text = "Ins Payments:";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(86, 18);
			this.label3.TabIndex = 17;
			this.label3.Text = "Positive Adjs:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsPay
			// 
			this.labelInsPay.Location = new System.Drawing.Point(99, 128);
			this.labelInsPay.Name = "labelInsPay";
			this.labelInsPay.Size = new System.Drawing.Size(61, 18);
			this.labelInsPay.TabIndex = 24;
			this.labelInsPay.Text = "0.00";
			this.labelInsPay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelNegativeAdjs
			// 
			this.labelNegativeAdjs.Location = new System.Drawing.Point(99, 62);
			this.labelNegativeAdjs.Name = "labelNegativeAdjs";
			this.labelNegativeAdjs.Size = new System.Drawing.Size(61, 18);
			this.labelNegativeAdjs.TabIndex = 18;
			this.labelNegativeAdjs.Text = "0.00";
			this.labelNegativeAdjs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(7, 62);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(86, 18);
			this.label7.TabIndex = 19;
			this.label7.Text = "Negative Adjs:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPayPlanCredits
			// 
			this.labelPayPlanCredits.Location = new System.Drawing.Point(99, 84);
			this.labelPayPlanCredits.Name = "labelPayPlanCredits";
			this.labelPayPlanCredits.Size = new System.Drawing.Size(61, 18);
			this.labelPayPlanCredits.TabIndex = 20;
			this.labelPayPlanCredits.Text = "0.00";
			this.labelPayPlanCredits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(7, 84);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 18);
			this.label5.TabIndex = 21;
			this.label5.Text = "PayPlan Credits:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelUnallocated
			// 
			this.labelUnallocated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelUnallocated.Location = new System.Drawing.Point(390, 16);
			this.labelUnallocated.Name = "labelUnallocated";
			this.labelUnallocated.Size = new System.Drawing.Size(420, 45);
			this.labelUnallocated.TabIndex = 155;
			this.labelUnallocated.Text = "This patient has unallocated unearned income. Please select procedures to allocat" +
    "e this income towards.";
			this.labelUnallocated.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelUnallocated.Visible = false;
			// 
			// FormProcSelect
			// 
			this.ClientSize = new System.Drawing.Size(873, 586);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelUnallocated);
			this.Controls.Add(this.groupBreakdown);
			this.Controls.Add(this.groupCreditLogic);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Procedure";
			this.Load += new System.EventHandler(this.FormProcSelect_Load);
			this.groupCreditLogic.ResumeLayout(false);
			this.groupBreakdown.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
