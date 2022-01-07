namespace OpenDental{
	partial class FormDiscountPlanMerge {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiscountPlanMerge));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textAdjTypeInto = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butChangePlanInto = new OpenDental.UI.Button();
			this.textFeeSchedInto = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescriptionInto = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textAdjTypeFrom = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butChangePlanFrom = new OpenDental.UI.Button();
			this.textFeeSchedFrom = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textDescriptionFrom = new System.Windows.Forms.TextBox();
			this.butMerge = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textAdjTypeInto);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.butChangePlanInto);
			this.groupBox1.Controls.Add(this.textFeeSchedInto);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textDescriptionInto);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(638, 88);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Plan to merge into. Patients with the plan chosen below will have this plan after" +
    " merging.";
			// 
			// textAdjTypeInto
			// 
			this.textAdjTypeInto.Location = new System.Drawing.Point(396, 37);
			this.textAdjTypeInto.Name = "textAdjTypeInto";
			this.textAdjTypeInto.ReadOnly = true;
			this.textAdjTypeInto.Size = new System.Drawing.Size(126, 20);
			this.textAdjTypeInto.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(393, 18);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Adjustment Type";
			// 
			// butChangePlanInto
			// 
			this.butChangePlanInto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangePlanInto.Location = new System.Drawing.Point(551, 34);
			this.butChangePlanInto.Name = "butChangePlanInto";
			this.butChangePlanInto.Size = new System.Drawing.Size(75, 24);
			this.butChangePlanInto.TabIndex = 4;
			this.butChangePlanInto.Text = "Change";
			this.butChangePlanInto.Click += new System.EventHandler(this.butChangePlanInto_Click);
			// 
			// textFeeSchedInto
			// 
			this.textFeeSchedInto.Location = new System.Drawing.Point(153, 37);
			this.textFeeSchedInto.Name = "textFeeSchedInto";
			this.textFeeSchedInto.ReadOnly = true;
			this.textFeeSchedInto.Size = new System.Drawing.Size(237, 20);
			this.textFeeSchedInto.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(150, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Fee Schedule";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Description";
			// 
			// textDescriptionInto
			// 
			this.textDescriptionInto.Location = new System.Drawing.Point(6, 37);
			this.textDescriptionInto.Name = "textDescriptionInto";
			this.textDescriptionInto.ReadOnly = true;
			this.textDescriptionInto.Size = new System.Drawing.Size(141, 20);
			this.textDescriptionInto.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textAdjTypeFrom);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.butChangePlanFrom);
			this.groupBox2.Controls.Add(this.textFeeSchedFrom);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.textDescriptionFrom);
			this.groupBox2.Location = new System.Drawing.Point(13, 112);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(638, 88);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Plan to merge from. Patients with this plan will be changed to have the plan abov" +
    "e.";
			// 
			// textAdjTypeFrom
			// 
			this.textAdjTypeFrom.Location = new System.Drawing.Point(396, 37);
			this.textAdjTypeFrom.Name = "textAdjTypeFrom";
			this.textAdjTypeFrom.ReadOnly = true;
			this.textAdjTypeFrom.Size = new System.Drawing.Size(126, 20);
			this.textAdjTypeFrom.TabIndex = 11;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(396, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(86, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "Adjustment Type";
			// 
			// butChangePlanFrom
			// 
			this.butChangePlanFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangePlanFrom.Location = new System.Drawing.Point(550, 34);
			this.butChangePlanFrom.Name = "butChangePlanFrom";
			this.butChangePlanFrom.Size = new System.Drawing.Size(75, 24);
			this.butChangePlanFrom.TabIndex = 9;
			this.butChangePlanFrom.Text = "Change";
			this.butChangePlanFrom.Click += new System.EventHandler(this.butChangePlanFrom_Click);
			// 
			// textFeeSchedFrom
			// 
			this.textFeeSchedFrom.Location = new System.Drawing.Point(153, 37);
			this.textFeeSchedFrom.Name = "textFeeSchedFrom";
			this.textFeeSchedFrom.ReadOnly = true;
			this.textFeeSchedFrom.Size = new System.Drawing.Size(237, 20);
			this.textFeeSchedFrom.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(150, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(73, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Fee Schedule";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Description";
			// 
			// textDescriptionFrom
			// 
			this.textDescriptionFrom.Location = new System.Drawing.Point(6, 37);
			this.textDescriptionFrom.Name = "textDescriptionFrom";
			this.textDescriptionFrom.ReadOnly = true;
			this.textDescriptionFrom.Size = new System.Drawing.Size(141, 20);
			this.textDescriptionFrom.TabIndex = 5;
			// 
			// butMerge
			// 
			this.butMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(479, 213);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 3;
			this.butMerge.Text = "Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(563, 213);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormDiscountPlanMerge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(663, 250);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butMerge);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDiscountPlanMerge";
			this.Text = "Merge Discount Plans";
			this.Load += new System.EventHandler(this.FormDiscountPlanMerge_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butMerge;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textFeeSchedInto;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescriptionInto;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.Button butChangePlanInto;
		private OpenDental.UI.Button butChangePlanFrom;
		private System.Windows.Forms.TextBox textFeeSchedFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDescriptionFrom;
		private System.Windows.Forms.TextBox textAdjTypeInto;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textAdjTypeFrom;
		private System.Windows.Forms.Label label6;
	}
}