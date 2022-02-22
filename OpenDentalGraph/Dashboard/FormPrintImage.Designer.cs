namespace OpenDentalGraph {
	partial class FormPrintImage {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrintImage));
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.butExport = new System.Windows.Forms.Button();
			this.butPrint = new System.Windows.Forms.Button();
			this.groupChartPosition = new System.Windows.Forms.GroupBox();
			this.textXPos = new System.Windows.Forms.TextBox();
			this.textYPos = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkLandscape = new System.Windows.Forms.CheckBox();
			this.groupMargins = new System.Windows.Forms.GroupBox();
			this.textMarginWidth = new System.Windows.Forms.TextBox();
			this.textMarginHeight = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupChartSize = new System.Windows.Forms.GroupBox();
			this.textWidth = new System.Windows.Forms.TextBox();
			this.textHeight = new System.Windows.Forms.TextBox();
			this.labelHeight = new System.Windows.Forms.Label();
			this.labelWidth = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.printPreviewControl = new System.Windows.Forms.PrintPreviewControl();
			this.butClose = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.printDocument1 = new System.Drawing.Printing.PrintDocument();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.groupChartPosition.SuspendLayout();
			this.groupMargins.SuspendLayout();
			this.groupChartSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.IsSplitterFixed = true;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.butExport);
			this.splitContainer.Panel1.Controls.Add(this.butPrint);
			this.splitContainer.Panel1.Controls.Add(this.groupChartPosition);
			this.splitContainer.Panel1.Controls.Add(this.checkLandscape);
			this.splitContainer.Panel1.Controls.Add(this.groupMargins);
			this.splitContainer.Panel1.Controls.Add(this.groupChartSize);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.splitContainer1);
			this.splitContainer.Size = new System.Drawing.Size(831, 645);
			this.splitContainer.SplitterDistance = 89;
			this.splitContainer.SplitterWidth = 1;
			this.splitContainer.TabIndex = 1;
			// 
			// butExport
			// 
			this.butExport.Location = new System.Drawing.Point(466, 26);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(83, 23);
			this.butExport.TabIndex = 3;
			this.butExport.Text = "Export...";
			this.butExport.UseVisualStyleBackColor = true;
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butPrint
			// 
			this.butPrint.Location = new System.Drawing.Point(466, 53);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(83, 23);
			this.butPrint.TabIndex = 4;
			this.butPrint.Text = "Print...";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// groupChartPosition
			// 
			this.groupChartPosition.Controls.Add(this.textXPos);
			this.groupChartPosition.Controls.Add(this.textYPos);
			this.groupChartPosition.Controls.Add(this.label1);
			this.groupChartPosition.Controls.Add(this.label2);
			this.groupChartPosition.Location = new System.Drawing.Point(311, 6);
			this.groupChartPosition.Name = "groupChartPosition";
			this.groupChartPosition.Size = new System.Drawing.Size(142, 72);
			this.groupChartPosition.TabIndex = 2;
			this.groupChartPosition.TabStop = false;
			this.groupChartPosition.Text = "Image Position";
			// 
			// textXPos
			// 
			this.textXPos.Location = new System.Drawing.Point(76, 19);
			this.textXPos.Name = "textXPos";
			this.textXPos.Size = new System.Drawing.Size(53, 20);
			this.textXPos.TabIndex = 0;
			this.textXPos.Text = "0";
			this.textXPos.TextChanged += new System.EventHandler(this.refresh_Event);
			// 
			// textYPos
			// 
			this.textYPos.Location = new System.Drawing.Point(76, 41);
			this.textYPos.Name = "textYPos";
			this.textYPos.Size = new System.Drawing.Size(53, 20);
			this.textYPos.TabIndex = 1;
			this.textYPos.Text = "0";
			this.textYPos.TextChanged += new System.EventHandler(this.refresh_Event);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "y:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "x:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkLandscape
			// 
			this.checkLandscape.Checked = true;
			this.checkLandscape.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkLandscape.Location = new System.Drawing.Point(555, 59);
			this.checkLandscape.Name = "checkLandscape";
			this.checkLandscape.Size = new System.Drawing.Size(142, 17);
			this.checkLandscape.TabIndex = 5;
			this.checkLandscape.Text = "Landscape";
			this.checkLandscape.UseVisualStyleBackColor = true;
			this.checkLandscape.CheckedChanged += new System.EventHandler(this.checkLandscape_CheckedChanged);
			// 
			// groupMargins
			// 
			this.groupMargins.Controls.Add(this.textMarginWidth);
			this.groupMargins.Controls.Add(this.textMarginHeight);
			this.groupMargins.Controls.Add(this.label4);
			this.groupMargins.Controls.Add(this.label5);
			this.groupMargins.Location = new System.Drawing.Point(164, 6);
			this.groupMargins.Name = "groupMargins";
			this.groupMargins.Size = new System.Drawing.Size(141, 72);
			this.groupMargins.TabIndex = 1;
			this.groupMargins.TabStop = false;
			this.groupMargins.Text = "Margins";
			// 
			// textMarginWidth
			// 
			this.textMarginWidth.Location = new System.Drawing.Point(76, 19);
			this.textMarginWidth.Name = "textMarginWidth";
			this.textMarginWidth.Size = new System.Drawing.Size(53, 20);
			this.textMarginWidth.TabIndex = 0;
			this.textMarginWidth.Text = "20";
			this.textMarginWidth.TextChanged += new System.EventHandler(this.refresh_Event);
			// 
			// textMarginHeight
			// 
			this.textMarginHeight.Location = new System.Drawing.Point(76, 41);
			this.textMarginHeight.Name = "textMarginHeight";
			this.textMarginHeight.Size = new System.Drawing.Size(53, 20);
			this.textMarginHeight.TabIndex = 1;
			this.textMarginHeight.Text = "20";
			this.textMarginHeight.TextChanged += new System.EventHandler(this.refresh_Event);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(7, 44);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Height:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(7, 22);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Width:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupChartSize
			// 
			this.groupChartSize.Controls.Add(this.textWidth);
			this.groupChartSize.Controls.Add(this.textHeight);
			this.groupChartSize.Controls.Add(this.labelHeight);
			this.groupChartSize.Controls.Add(this.labelWidth);
			this.groupChartSize.Location = new System.Drawing.Point(10, 6);
			this.groupChartSize.Name = "groupChartSize";
			this.groupChartSize.Size = new System.Drawing.Size(142, 72);
			this.groupChartSize.TabIndex = 0;
			this.groupChartSize.TabStop = false;
			this.groupChartSize.Text = "Image Size";
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(76, 19);
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(53, 20);
			this.textWidth.TabIndex = 2;
			this.textWidth.TextChanged += new System.EventHandler(this.refresh_Event);
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(76, 41);
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(53, 20);
			this.textHeight.TabIndex = 3;
			this.textHeight.TextChanged += new System.EventHandler(this.refresh_Event);
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(7, 44);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(68, 13);
			this.labelHeight.TabIndex = 1;
			this.labelHeight.Text = "Height:";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWidth
			// 
			this.labelWidth.Location = new System.Drawing.Point(7, 22);
			this.labelWidth.Name = "labelWidth";
			this.labelWidth.Size = new System.Drawing.Size(68, 13);
			this.labelWidth.TabIndex = 0;
			this.labelWidth.Text = "Width:";
			this.labelWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.printPreviewControl);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.butClose);
			this.splitContainer1.Size = new System.Drawing.Size(831, 555);
			this.splitContainer1.SplitterDistance = 529;
			this.splitContainer1.SplitterWidth = 1;
			this.splitContainer1.TabIndex = 0;
			// 
			// printPreviewControl
			// 
			this.printPreviewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.printPreviewControl.Location = new System.Drawing.Point(0, 0);
			this.printPreviewControl.Name = "printPreviewControl";
			this.printPreviewControl.Size = new System.Drawing.Size(831, 529);
			this.printPreviewControl.TabIndex = 0;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(744, 0);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 750;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// printDocument1
			// 
			this.printDocument1.OriginAtMargins = true;
			this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.ImageGenericFormat_PrintPage);
			// 
			// FormPrintImage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(831, 645);
			this.Controls.Add(this.splitContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrintImage";
			this.Text = "Print Settings";
			this.Load += new System.EventHandler(this.FormPrintSettings_Load);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.groupChartPosition.ResumeLayout(false);
			this.groupChartPosition.PerformLayout();
			this.groupMargins.ResumeLayout(false);
			this.groupMargins.PerformLayout();
			this.groupChartSize.ResumeLayout(false);
			this.groupChartSize.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.CheckBox checkLandscape;
		private System.Windows.Forms.Label labelWidth;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.TextBox textHeight;
		private System.Windows.Forms.TextBox textWidth;
		private System.Windows.Forms.GroupBox groupChartSize;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textMarginHeight;
		private System.Windows.Forms.TextBox textMarginWidth;
		private System.Windows.Forms.GroupBox groupMargins;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textYPos;
		private System.Windows.Forms.TextBox textXPos;
		private System.Windows.Forms.GroupBox groupChartPosition;
		private System.Windows.Forms.Button butPrint;
		private System.Windows.Forms.Button butExport;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PrintPreviewControl printPreviewControl;
		private System.Windows.Forms.Timer timer1;
		private System.Drawing.Printing.PrintDocument printDocument1;
	}
}