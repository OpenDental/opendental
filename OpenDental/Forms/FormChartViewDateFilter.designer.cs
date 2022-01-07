namespace OpenDental{
	partial class FormChartViewDateFilter {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChartViewDateFilter));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listPresetDateRanges = new OpenDental.UI.ListBoxOD();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butNowEnd = new OpenDental.UI.Button();
			this.butNowStart = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(19, 87);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "Start Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(19, 113);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "End Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listPresetDateRanges
			// 
			this.listPresetDateRanges.Location = new System.Drawing.Point(105, 12);
			this.listPresetDateRanges.Name = "listPresetDateRanges";
			this.listPresetDateRanges.SelectionMode = UI.SelectionMode.None;
			this.listPresetDateRanges.Size = new System.Drawing.Size(91, 69);
			this.listPresetDateRanges.TabIndex = 8;
			this.listPresetDateRanges.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listPresetDateRanges_MouseClick);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(105, 112);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(91, 20);
			this.textDateEnd.TabIndex = 5;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(105, 86);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(91, 20);
			this.textDateStart.TabIndex = 4;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(125, 168);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(206, 168);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butNowEnd
			// 
			this.butNowEnd.Location = new System.Drawing.Point(206, 112);
			this.butNowEnd.Name = "butNowEnd";
			this.butNowEnd.Size = new System.Drawing.Size(75, 20);
			this.butNowEnd.TabIndex = 23;
			this.butNowEnd.Text = "Now";
			this.butNowEnd.Click += new System.EventHandler(this.butNowEnd_Click);
			// 
			// butNowStart
			// 
			this.butNowStart.Location = new System.Drawing.Point(206, 86);
			this.butNowStart.Name = "butNowStart";
			this.butNowStart.Size = new System.Drawing.Size(75, 20);
			this.butNowStart.TabIndex = 22;
			this.butNowStart.Text = "Now";
			this.butNowStart.Click += new System.EventHandler(this.butNowStart_Click);
			// 
			// FormChartViewDateFilter
			// 
			this.ClientSize = new System.Drawing.Size(299, 206);
			this.Controls.Add(this.butNowEnd);
			this.Controls.Add(this.butNowStart);
			this.Controls.Add(this.listPresetDateRanges);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormChartViewDateFilter";
			this.Text = "Progress Notes Date Range Filter";
			this.Load += new System.EventHandler(this.FormChartViewDateFilter_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidDate textDateStart;
		private ValidDate textDateEnd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listPresetDateRanges;
		private UI.Button butNowEnd;
		private UI.Button butNowStart;
	}
}