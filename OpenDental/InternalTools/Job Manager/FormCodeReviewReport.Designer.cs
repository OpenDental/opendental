namespace OpenDental {
	partial class FormCodeReviewReport {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			this.butRunReport = new OpenDental.UI.Button();
			this.comboUser = new System.Windows.Forms.ComboBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.dateTimeTo = new System.Windows.Forms.DateTimePicker();
			this.dateTimeFrom = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butRunReport
			// 
			this.butRunReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRunReport.Location = new System.Drawing.Point(469, 12);
			this.butRunReport.Name = "butRunReport";
			this.butRunReport.Size = new System.Drawing.Size(105, 27);
			this.butRunReport.TabIndex = 0;
			this.butRunReport.Text = "Run";
			this.butRunReport.UseVisualStyleBackColor = true;
			this.butRunReport.Click += new System.EventHandler(this.butRunReport_Click);
			// 
			// comboUser
			// 
			this.comboUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUser.FormattingEnabled = true;
			this.comboUser.Location = new System.Drawing.Point(80, 15);
			this.comboUser.Name = "comboUser";
			this.comboUser.Size = new System.Drawing.Size(84, 21);
			this.comboUser.TabIndex = 237;
			this.comboUser.SelectedIndexChanged += new System.EventHandler(this.comboUser_SelectedIndexChanged);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 52);
			this.gridMain.Margin = new System.Windows.Forms.Padding(10);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(562, 376);
			this.gridMain.TabIndex = 245;
			this.gridMain.Title = "Jobs Reviewed by Engineer";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(318, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 241;
			this.label2.Text = "To";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(167, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 240;
			this.label1.Text = "From";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateTimeTo
			// 
			this.dateTimeTo.CustomFormat = "YYYY-MM-DD";
			this.dateTimeTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeTo.Location = new System.Drawing.Point(349, 15);
			this.dateTimeTo.Name = "dateTimeTo";
			this.dateTimeTo.Size = new System.Drawing.Size(103, 20);
			this.dateTimeTo.TabIndex = 239;
			// 
			// dateTimeFrom
			// 
			this.dateTimeFrom.CustomFormat = "";
			this.dateTimeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeFrom.Location = new System.Drawing.Point(213, 15);
			this.dateTimeFrom.Name = "dateTimeFrom";
			this.dateTimeFrom.Size = new System.Drawing.Size(103, 20);
			this.dateTimeFrom.TabIndex = 238;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(14, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 13);
			this.label3.TabIndex = 246;
			this.label3.Text = "Engineer";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCodeReviewReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 441);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dateTimeTo);
			this.Controls.Add(this.dateTimeFrom);
			this.Controls.Add(this.butRunReport);
			this.Controls.Add(this.comboUser);
			this.Name = "FormCodeReviewReport";
			this.Text = "Code Review Report";
			this.Load += new System.EventHandler(this.FormCodeReviewReport_Load);
			this.ResumeLayout(false);

		}

		#endregion\
		private System.Windows.Forms.ComboBox comboUser;
		private UI.Button butRunReport;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dateTimeTo;
		private System.Windows.Forms.DateTimePicker dateTimeFrom;
		private System.Windows.Forms.Label label3;
	}
}