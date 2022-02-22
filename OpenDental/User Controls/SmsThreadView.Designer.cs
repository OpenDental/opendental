namespace OpenDental {
	partial class SmsThreadView {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.panelScroll = new System.Windows.Forms.Panel();
			this.labelCurrentPage = new System.Windows.Forms.Label();
			this.butForwardPage = new OpenDental.UI.Button();
			this.butBackPage = new OpenDental.UI.Button();
			this.butBeginning = new OpenDental.UI.Button();
			this.butEnd = new OpenDental.UI.Button();
			this.panelNavigation = new OpenDental.UI.PanelOD();
			this.panelNavigation.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelScroll
			// 
			this.panelScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelScroll.AutoScroll = true;
			this.panelScroll.Location = new System.Drawing.Point(0, 27);
			this.panelScroll.Name = "panelScroll";
			this.panelScroll.Size = new System.Drawing.Size(250, 475);
			this.panelScroll.TabIndex = 0;
			// 
			// labelCurrentPage
			// 
			this.labelCurrentPage.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelCurrentPage.Location = new System.Drawing.Point(94, 2);
			this.labelCurrentPage.Name = "labelCurrentPage";
			this.labelCurrentPage.Size = new System.Drawing.Size(63, 18);
			this.labelCurrentPage.TabIndex = 177;
			this.labelCurrentPage.Text = "1 of 1";
			this.labelCurrentPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butForwardPage
			// 
			this.butForwardPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butForwardPage.Location = new System.Drawing.Point(37, 2);
			this.butForwardPage.Name = "butForwardPage";
			this.butForwardPage.Size = new System.Drawing.Size(28, 20);
			this.butForwardPage.TabIndex = 176;
			this.butForwardPage.Text = "<";
			this.butForwardPage.Click += new System.EventHandler(this.butForwardPage_Click);
			// 
			// butBackPage
			// 
			this.butBackPage.Location = new System.Drawing.Point(185, 2);
			this.butBackPage.Name = "butBackPage";
			this.butBackPage.Size = new System.Drawing.Size(28, 20);
			this.butBackPage.TabIndex = 175;
			this.butBackPage.Text = ">";
			this.butBackPage.Click += new System.EventHandler(this.butBackPage_Click);
			// 
			// butBeginning
			// 
			this.butBeginning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBeginning.Location = new System.Drawing.Point(3, 2);
			this.butBeginning.Name = "butBeginning";
			this.butBeginning.Size = new System.Drawing.Size(28, 20);
			this.butBeginning.TabIndex = 174;
			this.butBeginning.Text = "<<";
			this.butBeginning.Click += new System.EventHandler(this.butBeginning_Click);
			// 
			// butEnd
			// 
			this.butEnd.Location = new System.Drawing.Point(219, 2);
			this.butEnd.Name = "butEnd";
			this.butEnd.Size = new System.Drawing.Size(28, 20);
			this.butEnd.TabIndex = 173;
			this.butEnd.Text = ">>";
			this.butEnd.Click += new System.EventHandler(this.butEnd_Click);
			// 
			// panelNavigation
			// 
			this.panelNavigation.Controls.Add(this.butBackPage);
			this.panelNavigation.Controls.Add(this.labelCurrentPage);
			this.panelNavigation.Controls.Add(this.butForwardPage);
			this.panelNavigation.Controls.Add(this.butEnd);
			this.panelNavigation.Controls.Add(this.butBeginning);
			this.panelNavigation.Location = new System.Drawing.Point(0, 3);
			this.panelNavigation.Name = "panelNavigation";
			this.panelNavigation.Size = new System.Drawing.Size(250, 24);
			this.panelNavigation.TabIndex = 0;
			this.panelNavigation.Visible = false;
			// 
			// SmsThreadView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.panelNavigation);
			this.Controls.Add(this.panelScroll);
			this.Name = "SmsThreadView";
			this.Size = new System.Drawing.Size(250, 500);
			this.panelNavigation.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelScroll;
		private System.Windows.Forms.Label labelCurrentPage;
		private UI.Button butForwardPage;
		private UI.Button butBackPage;
		private UI.Button butBeginning;
		private UI.Button butEnd;
		private UI.PanelOD panelNavigation;
	}
}
