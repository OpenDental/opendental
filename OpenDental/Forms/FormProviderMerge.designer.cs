namespace OpenDental{
	partial class FormProviderMerge {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProviderMerge));
			this.groupBoxInto = new System.Windows.Forms.GroupBox();
			this.textAbbrInto = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textNpiInto = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textFullNameInto = new System.Windows.Forms.TextBox();
			this.butChangeProvInto = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textProvNumInto = new System.Windows.Forms.TextBox();
			this.groupBoxFrom = new System.Windows.Forms.GroupBox();
			this.checkDeletedProvs = new System.Windows.Forms.CheckBox();
			this.textAbbrFrom = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textNpiFrom = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.butChangeProvFrom = new OpenDental.UI.Button();
			this.textFullNameFrom = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textProvNumFrom = new System.Windows.Forms.TextBox();
			this.butMerge = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupBoxInto.SuspendLayout();
			this.groupBoxFrom.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxInto
			// 
			this.groupBoxInto.Controls.Add(this.textAbbrInto);
			this.groupBoxInto.Controls.Add(this.label7);
			this.groupBoxInto.Controls.Add(this.textNpiInto);
			this.groupBoxInto.Controls.Add(this.label5);
			this.groupBoxInto.Controls.Add(this.textFullNameInto);
			this.groupBoxInto.Controls.Add(this.butChangeProvInto);
			this.groupBoxInto.Controls.Add(this.label2);
			this.groupBoxInto.Controls.Add(this.label1);
			this.groupBoxInto.Controls.Add(this.textProvNumInto);
			this.groupBoxInto.Location = new System.Drawing.Point(12, 12);
			this.groupBoxInto.Name = "groupBoxInto";
			this.groupBoxInto.Size = new System.Drawing.Size(665, 89);
			this.groupBoxInto.TabIndex = 6;
			this.groupBoxInto.TabStop = false;
			this.groupBoxInto.Text = "Provider to merge into. The provider chosen below will be merged into this provid" +
    "er.";
			// 
			// textAbbrInto
			// 
			this.textAbbrInto.Location = new System.Drawing.Point(153, 37);
			this.textAbbrInto.Name = "textAbbrInto";
			this.textAbbrInto.ReadOnly = true;
			this.textAbbrInto.Size = new System.Drawing.Size(110, 20);
			this.textAbbrInto.TabIndex = 8;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(150, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(113, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "Abbr";
			// 
			// textNpiInto
			// 
			this.textNpiInto.Location = new System.Drawing.Point(437, 37);
			this.textNpiInto.Name = "textNpiInto";
			this.textNpiInto.ReadOnly = true;
			this.textNpiInto.Size = new System.Drawing.Size(123, 20);
			this.textNpiInto.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(434, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "NPI";
			// 
			// textFullNameInto
			// 
			this.textFullNameInto.Location = new System.Drawing.Point(269, 37);
			this.textFullNameInto.Name = "textFullNameInto";
			this.textFullNameInto.ReadOnly = true;
			this.textFullNameInto.Size = new System.Drawing.Size(162, 20);
			this.textFullNameInto.TabIndex = 3;
			// 
			// butChangeProvInto
			// 
			this.butChangeProvInto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeProvInto.Location = new System.Drawing.Point(577, 33);
			this.butChangeProvInto.Name = "butChangeProvInto";
			this.butChangeProvInto.Size = new System.Drawing.Size(75, 24);
			this.butChangeProvInto.TabIndex = 4;
			this.butChangeProvInto.Text = "Change";
			this.butChangeProvInto.Click += new System.EventHandler(this.butChangeProvInto_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(266, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(125, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Full Name";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "ProvNum";
			// 
			// textProvNumInto
			// 
			this.textProvNumInto.Location = new System.Drawing.Point(6, 37);
			this.textProvNumInto.Name = "textProvNumInto";
			this.textProvNumInto.ReadOnly = true;
			this.textProvNumInto.Size = new System.Drawing.Size(141, 20);
			this.textProvNumInto.TabIndex = 0;
			// 
			// groupBoxFrom
			// 
			this.groupBoxFrom.Controls.Add(this.checkDeletedProvs);
			this.groupBoxFrom.Controls.Add(this.textAbbrFrom);
			this.groupBoxFrom.Controls.Add(this.label8);
			this.groupBoxFrom.Controls.Add(this.textNpiFrom);
			this.groupBoxFrom.Controls.Add(this.label9);
			this.groupBoxFrom.Controls.Add(this.butChangeProvFrom);
			this.groupBoxFrom.Controls.Add(this.textFullNameFrom);
			this.groupBoxFrom.Controls.Add(this.label10);
			this.groupBoxFrom.Controls.Add(this.label11);
			this.groupBoxFrom.Controls.Add(this.textProvNumFrom);
			this.groupBoxFrom.Location = new System.Drawing.Point(12, 107);
			this.groupBoxFrom.Name = "groupBoxFrom";
			this.groupBoxFrom.Size = new System.Drawing.Size(665, 91);
			this.groupBoxFrom.TabIndex = 9;
			this.groupBoxFrom.TabStop = false;
			this.groupBoxFrom.Text = "Provider to merge from. This provider will be merged into the one above. This pro" +
    "vider will be deleted.";
			// 
			// checkDeletedProvs
			// 
			this.checkDeletedProvs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDeletedProvs.Location = new System.Drawing.Point(536, 61);
			this.checkDeletedProvs.Name = "checkDeletedProvs";
			this.checkDeletedProvs.Size = new System.Drawing.Size(116, 24);
			this.checkDeletedProvs.TabIndex = 9;
			this.checkDeletedProvs.Text = "Show Deleted";
			this.checkDeletedProvs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDeletedProvs.UseVisualStyleBackColor = true;
			// 
			// textAbbrFrom
			// 
			this.textAbbrFrom.Location = new System.Drawing.Point(153, 37);
			this.textAbbrFrom.Name = "textAbbrFrom";
			this.textAbbrFrom.ReadOnly = true;
			this.textAbbrFrom.Size = new System.Drawing.Size(110, 20);
			this.textAbbrFrom.TabIndex = 8;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(150, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(110, 13);
			this.label8.TabIndex = 7;
			this.label8.Text = "Abbr";
			// 
			// textNpiFrom
			// 
			this.textNpiFrom.Location = new System.Drawing.Point(437, 37);
			this.textNpiFrom.Name = "textNpiFrom";
			this.textNpiFrom.ReadOnly = true;
			this.textNpiFrom.Size = new System.Drawing.Size(123, 20);
			this.textNpiFrom.TabIndex = 6;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(437, 22);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(99, 13);
			this.label9.TabIndex = 5;
			this.label9.Text = "NPI";
			// 
			// butChangeProvFrom
			// 
			this.butChangeProvFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeProvFrom.Location = new System.Drawing.Point(577, 34);
			this.butChangeProvFrom.Name = "butChangeProvFrom";
			this.butChangeProvFrom.Size = new System.Drawing.Size(75, 24);
			this.butChangeProvFrom.TabIndex = 4;
			this.butChangeProvFrom.Text = "Change";
			this.butChangeProvFrom.Click += new System.EventHandler(this.butChangeProvFrom_Click);
			// 
			// textFullNameFrom
			// 
			this.textFullNameFrom.Location = new System.Drawing.Point(269, 37);
			this.textFullNameFrom.Name = "textFullNameFrom";
			this.textFullNameFrom.ReadOnly = true;
			this.textFullNameFrom.Size = new System.Drawing.Size(162, 20);
			this.textFullNameFrom.TabIndex = 3;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(266, 22);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(125, 13);
			this.label10.TabIndex = 2;
			this.label10.Text = "Full Name";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(3, 22);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(99, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "ProvNum";
			// 
			// textProvNumFrom
			// 
			this.textProvNumFrom.Location = new System.Drawing.Point(6, 37);
			this.textProvNumFrom.Name = "textProvNumFrom";
			this.textProvNumFrom.ReadOnly = true;
			this.textProvNumFrom.Size = new System.Drawing.Size(141, 20);
			this.textProvNumFrom.TabIndex = 0;
			// 
			// butMerge
			// 
			this.butMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(505, 209);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 3;
			this.butMerge.Text = "Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(589, 209);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormProviderMerge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(689, 245);
			this.Controls.Add(this.groupBoxFrom);
			this.Controls.Add(this.groupBoxInto);
			this.Controls.Add(this.butMerge);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProviderMerge";
			this.Text = "Merge Providers";
			this.groupBoxInto.ResumeLayout(false);
			this.groupBoxInto.PerformLayout();
			this.groupBoxFrom.ResumeLayout(false);
			this.groupBoxFrom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butMerge;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.GroupBox groupBoxInto;
		private System.Windows.Forms.TextBox textNpiInto;
		private System.Windows.Forms.Label label5;
		private UI.Button butChangeProvInto;
		private System.Windows.Forms.TextBox textFullNameInto;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textProvNumInto;
		private System.Windows.Forms.TextBox textAbbrInto;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBoxFrom;
		private System.Windows.Forms.TextBox textAbbrFrom;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textNpiFrom;
		private System.Windows.Forms.Label label9;
		private UI.Button butChangeProvFrom;
		private System.Windows.Forms.TextBox textFullNameFrom;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textProvNumFrom;
		private System.Windows.Forms.CheckBox checkDeletedProvs;
	}
}