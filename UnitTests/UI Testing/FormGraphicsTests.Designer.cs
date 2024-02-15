namespace UnitTests
{
	partial class FormGraphicsTests
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
			this.panelSheetPreview = new OpenDental.UI.PanelOD();
			this.elementHost = new System.Windows.Forms.Integration.ElementHost();
			this.butPrintWPF = new OpenDental.UI.Button();
			this.labelTimeOld = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butPrintOld = new OpenDental.UI.Button();
			this.butPreviewOld = new OpenDental.UI.Button();
			this.butPreviewWPF = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// panelSheetPreview
			// 
			this.panelSheetPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panelSheetPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.panelSheetPreview.Location = new System.Drawing.Point(12, 38);
			this.panelSheetPreview.Name = "panelSheetPreview";
			this.panelSheetPreview.Size = new System.Drawing.Size(300, 717);
			this.panelSheetPreview.TabIndex = 1;
			this.panelSheetPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSheetPreview_Paint);
			// 
			// elementHost
			// 
			this.elementHost.Location = new System.Drawing.Point(333, 38);
			this.elementHost.Name = "elementHost";
			this.elementHost.Size = new System.Drawing.Size(300, 717);
			this.elementHost.TabIndex = 39;
			this.elementHost.Text = "elementHost1";
			this.elementHost.Child = null;
			// 
			// butPrintWPF
			// 
			this.butPrintWPF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintWPF.Location = new System.Drawing.Point(446, 794);
			this.butPrintWPF.Name = "butPrintWPF";
			this.butPrintWPF.Size = new System.Drawing.Size(75, 24);
			this.butPrintWPF.TabIndex = 222;
			this.butPrintWPF.Text = "Print";
			this.butPrintWPF.UseVisualStyleBackColor = true;
			this.butPrintWPF.Click += new System.EventHandler(this.butPrintWPF_Click);
			// 
			// labelTimeOld
			// 
			this.labelTimeOld.Location = new System.Drawing.Point(116, 17);
			this.labelTimeOld.Name = "labelTimeOld";
			this.labelTimeOld.Size = new System.Drawing.Size(100, 18);
			this.labelTimeOld.TabIndex = 223;
			this.labelTimeOld.Text = "WinForms";
			this.labelTimeOld.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(443, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 224;
			this.label1.Text = "WPF";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// butPrintOld
			// 
			this.butPrintOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintOld.Location = new System.Drawing.Point(119, 794);
			this.butPrintOld.Name = "butPrintOld";
			this.butPrintOld.Size = new System.Drawing.Size(75, 24);
			this.butPrintOld.TabIndex = 225;
			this.butPrintOld.Text = "Print";
			this.butPrintOld.UseVisualStyleBackColor = true;
			this.butPrintOld.Click += new System.EventHandler(this.butPrintOld_Click);
			// 
			// butPreviewOld
			// 
			this.butPreviewOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPreviewOld.Location = new System.Drawing.Point(119, 763);
			this.butPreviewOld.Name = "butPreviewOld";
			this.butPreviewOld.Size = new System.Drawing.Size(75, 24);
			this.butPreviewOld.TabIndex = 227;
			this.butPreviewOld.Text = "Preview";
			this.butPreviewOld.UseVisualStyleBackColor = true;
			this.butPreviewOld.Click += new System.EventHandler(this.butPreviewOld_Click);
			// 
			// butPreviewWPF
			// 
			this.butPreviewWPF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPreviewWPF.Location = new System.Drawing.Point(446, 763);
			this.butPreviewWPF.Name = "butPreviewWPF";
			this.butPreviewWPF.Size = new System.Drawing.Size(75, 24);
			this.butPreviewWPF.TabIndex = 226;
			this.butPreviewWPF.Text = "Preview";
			this.butPreviewWPF.UseVisualStyleBackColor = true;
			this.butPreviewWPF.Click += new System.EventHandler(this.butPreviewWPF_Click);
			// 
			// FormGraphicsTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 830);
			this.Controls.Add(this.butPreviewOld);
			this.Controls.Add(this.butPreviewWPF);
			this.Controls.Add(this.butPrintOld);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelTimeOld);
			this.Controls.Add(this.butPrintWPF);
			this.Controls.Add(this.elementHost);
			this.Controls.Add(this.panelSheetPreview);
			this.Name = "FormGraphicsTests";
			this.Text = "FormGraphicsTests";
			this.Load += new System.EventHandler(this.FormGridTest_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.PanelOD panelSheetPreview;
		private System.Windows.Forms.Integration.ElementHost elementHost;
		private OpenDental.UI.Button butPrintWPF;
		private System.Windows.Forms.Label labelTimeOld;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butPrintOld;
		private OpenDental.UI.Button butPreviewOld;
		private OpenDental.UI.Button butPreviewWPF;
	}
}