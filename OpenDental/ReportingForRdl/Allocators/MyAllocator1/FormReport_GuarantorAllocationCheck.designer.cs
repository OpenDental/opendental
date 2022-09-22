namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	partial class FormReport_GuarantorAllocationCheck
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.butFillReport = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.dgvSummary = new System.Windows.Forms.DataGridView();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.dgridView_ReportData = new System.Windows.Forms.DataGridView();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblSummary = new System.Windows.Forms.Label();
			this.butPrintPreview = new System.Windows.Forms.Button();
			this.dateTimePicker1_FromDate = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgridView_ReportData)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(864, 40);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(116, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "3479";
			// 
			// butFillReport
			// 
			this.butFillReport.Location = new System.Drawing.Point(864, 66);
			this.butFillReport.Name = "butFillReport";
			this.butFillReport.Size = new System.Drawing.Size(116, 23);
			this.butFillReport.TabIndex = 2;
			this.butFillReport.Text = "Fill Report";
			this.butFillReport.UseVisualStyleBackColor = true;
			this.butFillReport.Click += new System.EventHandler(this.butFillReport_Click);
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(861, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(119, 21);
			this.label2.TabIndex = 3;
			this.label2.Text = "Guarantor";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.dgvSummary);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.dgridView_ReportData);
			this.panel1.Controls.Add(this.lblTitle);
			this.panel1.Controls.Add(this.lblSummary);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(843, 949);
			this.panel1.TabIndex = 4;
			// 
			// dgvSummary
			// 
			this.dgvSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSummary.Location = new System.Drawing.Point(0, 613);
			this.dgvSummary.Name = "dgvSummary";
			this.dgvSummary.Size = new System.Drawing.Size(840, 316);
			this.dgvSummary.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(34, 585);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(360, 25);
			this.label3.TabIndex = 3;
			this.label3.Text = "Summary Data - dgridView_ReportData       (this lable not visible)";
			this.label3.Visible = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(29, 143);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(360, 25);
			this.label1.TabIndex = 2;
			this.label1.Text = "Main Report Data - dgridView_ReportData       (this lable not visible)";
			this.label1.Visible = false;
			// 
			// dgridView_ReportData
			// 
			this.dgridView_ReportData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgridView_ReportData.Location = new System.Drawing.Point(0, 171);
			this.dgridView_ReportData.Name = "dgridView_ReportData";
			this.dgridView_ReportData.Size = new System.Drawing.Size(840, 411);
			this.dgridView_ReportData.TabIndex = 1;
			// 
			// lblTitle
			// 
			this.lblTitle.BackColor = System.Drawing.Color.LightCyan;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(2, 28);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(838, 85);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Put Title Text Here";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// lblSummary
			// 
			this.lblSummary.Location = new System.Drawing.Point(0, 589);
			this.lblSummary.Name = "lblSummary";
			this.lblSummary.Size = new System.Drawing.Size(840, 21);
			this.lblSummary.TabIndex = 5;
			this.lblSummary.Text = "Summary";
			this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butPrintPreview
			// 
			this.butPrintPreview.Location = new System.Drawing.Point(864, 95);
			this.butPrintPreview.Name = "butPrintPreview";
			this.butPrintPreview.Size = new System.Drawing.Size(116, 23);
			this.butPrintPreview.TabIndex = 5;
			this.butPrintPreview.Text = "Print Preview";
			this.butPrintPreview.UseVisualStyleBackColor = true;
			this.butPrintPreview.Click += new System.EventHandler(this.butPrintPreview_Click);
			// 
			// dateTimePicker1_FromDate
			// 
			this.dateTimePicker1_FromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePicker1_FromDate.Location = new System.Drawing.Point(893, 124);
			this.dateTimePicker1_FromDate.Name = "dateTimePicker1_FromDate";
			this.dateTimePicker1_FromDate.Size = new System.Drawing.Size(87, 20);
			this.dateTimePicker1_FromDate.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(861, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "From ";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(861, 154);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "To";
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePicker1.Location = new System.Drawing.Point(893, 150);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(87, 20);
			this.dateTimePicker1.TabIndex = 9;
			// 
			// FormReport_GuarantorAllocationCheck
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1071, 973);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.dateTimePicker1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.dateTimePicker1_FromDate);
			this.Controls.Add(this.butPrintPreview);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butFillReport);
			this.Controls.Add(this.textBox1);
			this.Name = "FormReport_GuarantorAllocationCheck";
			this.Text = "TestForm";
			this.Load += new System.EventHandler(this.TestForm_Load);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgridView_ReportData)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button butFillReport;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button butPrintPreview;
		private System.Windows.Forms.DataGridView dgridView_ReportData;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView dgvSummary;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblSummary;
		private System.Windows.Forms.DateTimePicker dateTimePicker1_FromDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
	}
}