namespace OpenDental{
	partial class FormPrintReport {
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
			this.printPanel = new CodeBase.PrintPanel();
			this.labPageNum = new System.Windows.Forms.Label();
			this.vScroll = new System.Windows.Forms.VScrollBar();
			this.butPrint = new OpenDental.UI.Button();
			this.butPreviousPage = new OpenDental.UI.Button();
			this.butNextPage = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// printPanel
			// 
			this.printPanel.BackColor = System.Drawing.Color.White;
			this.printPanel.Location = new System.Drawing.Point(10,33);
			this.printPanel.Name = "printPanel";
			this.printPanel.Origin = new System.Drawing.Point(0,0);
			this.printPanel.Size = new System.Drawing.Size(650,620);
			this.printPanel.TabIndex = 0;
			// 
			// labPageNum
			// 
			this.labPageNum.AutoSize = true;
			this.labPageNum.BackColor = System.Drawing.SystemColors.ButtonShadow;
			this.labPageNum.Location = new System.Drawing.Point(156,9);
			this.labPageNum.Name = "labPageNum";
			this.labPageNum.Size = new System.Drawing.Size(38,13);
			this.labPageNum.TabIndex = 4;
			this.labPageNum.Text = "Page: ";
			this.labPageNum.Visible = false;
			// 
			// vScroll
			// 
			this.vScroll.LargeChange = 30;
			this.vScroll.Location = new System.Drawing.Point(663,33);
			this.vScroll.Minimum = 1;
			this.vScroll.Name = "vScroll";
			this.vScroll.Size = new System.Drawing.Size(20,620);
			this.vScroll.TabIndex = 5;
			this.vScroll.Value = 1;
			this.vScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScroll_Scroll);
			// 
			// butPrint
			// 
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.Location = new System.Drawing.Point(101,4);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(49,23);
			this.butPrint.TabIndex = 3;
			this.butPrint.Text = "Print";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butPreviousPage
			// 
			this.butPreviousPage.Image = global::OpenDental.Properties.Resources.up;
			this.butPreviousPage.Location = new System.Drawing.Point(50,4);
			this.butPreviousPage.Name = "butPreviousPage";
			this.butPreviousPage.Size = new System.Drawing.Size(45,23);
			this.butPreviousPage.TabIndex = 2;
			this.butPreviousPage.Text = "Previous Page";
			this.butPreviousPage.UseVisualStyleBackColor = true;
			this.butPreviousPage.Click += new System.EventHandler(this.butPreviousPage_Click);
			// 
			// butNextPage
			// 
			this.butNextPage.Image = global::OpenDental.Properties.Resources.down;
			this.butNextPage.Location = new System.Drawing.Point(5,4);
			this.butNextPage.Name = "butNextPage";
			this.butNextPage.Size = new System.Drawing.Size(39,23);
			this.butNextPage.TabIndex = 1;
			this.butNextPage.Text = "Next Page";
			this.butNextPage.UseVisualStyleBackColor = true;
			this.butNextPage.Click += new System.EventHandler(this.butNextPage_Click);
			// 
			// FormPrintReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(686,666);
			this.Controls.Add(this.vScroll);
			this.Controls.Add(this.labPageNum);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butPreviousPage);
			this.Controls.Add(this.butNextPage);
			this.Controls.Add(this.printPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPrintReport";
			this.Text = "Print Report";
			this.Load += new System.EventHandler(this.FormPrintReport_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrintReport_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CodeBase.PrintPanel printPanel;
		private OpenDental.UI.Button butNextPage;
		private OpenDental.UI.Button butPreviousPage;
		private OpenDental.UI.Button butPrint;
		private System.Windows.Forms.Label labPageNum;
		private System.Windows.Forms.VScrollBar vScroll;

	}
}