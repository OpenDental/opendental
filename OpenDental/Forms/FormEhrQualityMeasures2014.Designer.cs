namespace OpenDental {
	partial class FormEhrQualityMeasures2014 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrQualityMeasures2014));
			this.butClose = new System.Windows.Forms.Button();
			this.butRefresh = new System.Windows.Forms.Button();
			this.textDateEnd = new System.Windows.Forms.TextBox();
			this.textDateStart = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butCreateQRDAs = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(707, 408);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(666, 8);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 21;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(176, 8);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(100, 20);
			this.textDateEnd.TabIndex = 19;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(50, 8);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 18;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(137, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 20;
			this.label1.Text = "to";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(-13, 13);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 13);
			this.label5.TabIndex = 17;
			this.label5.Text = "Period";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.FormattingEnabled = true;
			this.comboProv.Location = new System.Drawing.Point(428, 8);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(215, 21);
			this.comboProv.TabIndex = 24;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(306, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118, 13);
			this.label2.TabIndex = 23;
			this.label2.Text = "Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCreateQRDAs
			// 
			this.butCreateQRDAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCreateQRDAs.Location = new System.Drawing.Point(183, 408);
			this.butCreateQRDAs.Name = "butCreateQRDAs";
			this.butCreateQRDAs.Size = new System.Drawing.Size(93, 23);
			this.butCreateQRDAs.TabIndex = 25;
			this.butCreateQRDAs.Text = "Create QRDAs";
			this.butCreateQRDAs.UseVisualStyleBackColor = true;
			this.butCreateQRDAs.Click += new System.EventHandler(this.butCreateQRDAs_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 36);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(770, 366);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Clinical Quality Measures 2014";
			this.gridMain.TranslationName = "TableMeasures";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormEhrQualityMeasures2014
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(794, 438);
			this.Controls.Add(this.butCreateQRDAs);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrQualityMeasures2014";
			this.Text = "Clinical Quality Measures 2014";
			this.Load += new System.EventHandler(this.FormQualityMeasures_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butRefresh;
		private System.Windows.Forms.TextBox textDateEnd;
		private System.Windows.Forms.TextBox textDateStart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboProv;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button butCreateQRDAs;
	}
}