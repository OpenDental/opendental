namespace OpenDental{
	partial class FormMapAreaContainerEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapAreaContainerEdit));
			this.panelContainer = new OpenDental.UI.PanelOD();
			this.mapPanel = new OpenDental.InternalTools.Phones.MapPanel();
			this.butClose = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butEdit = new OpenDental.UI.Button();
			this.butAddSmall = new OpenDental.UI.Button();
			this.butAddBig = new OpenDental.UI.Button();
			this.butAddLabel = new OpenDental.UI.Button();
			this.panelContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelContainer
			// 
			this.panelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelContainer.AutoScroll = true;
			this.panelContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelContainer.Controls.Add(this.mapPanel);
			this.panelContainer.Location = new System.Drawing.Point(0, 0);
			this.panelContainer.Name = "panelContainer";
			this.panelContainer.Size = new System.Drawing.Size(993, 740);
			this.panelContainer.TabIndex = 108;
			// 
			// mapPanel
			// 
			this.mapPanel.Location = new System.Drawing.Point(0, 0);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(942, 532);
			this.mapPanel.TabIndex = 106;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1026, 716);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(1026, 623);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 111;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(999, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 117;
			this.label4.Text = "Description";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(1001, 28);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(100, 20);
			this.textDescription.TabIndex = 116;
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(1026, 76);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 24);
			this.butEdit.TabIndex = 118;
			this.butEdit.Text = "Edit Details";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// butAddSmall
			// 
			this.butAddSmall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddSmall.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSmall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSmall.Location = new System.Drawing.Point(1008, 316);
			this.butAddSmall.Name = "butAddSmall";
			this.butAddSmall.Size = new System.Drawing.Size(93, 24);
			this.butAddSmall.TabIndex = 119;
			this.butAddSmall.Text = "Add Small";
			this.butAddSmall.Click += new System.EventHandler(this.butAddSmall_Click);
			// 
			// butAddBig
			// 
			this.butAddBig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddBig.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddBig.Location = new System.Drawing.Point(1008, 346);
			this.butAddBig.Name = "butAddBig";
			this.butAddBig.Size = new System.Drawing.Size(93, 24);
			this.butAddBig.TabIndex = 120;
			this.butAddBig.Text = "Add Big";
			this.butAddBig.Click += new System.EventHandler(this.butAddBig_Click);
			// 
			// butAddLabel
			// 
			this.butAddLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddLabel.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddLabel.Location = new System.Drawing.Point(1008, 376);
			this.butAddLabel.Name = "butAddLabel";
			this.butAddLabel.Size = new System.Drawing.Size(93, 24);
			this.butAddLabel.TabIndex = 121;
			this.butAddLabel.Text = "Add Label";
			this.butAddLabel.Click += new System.EventHandler(this.butAddLabel_Click);
			// 
			// FormMapAreaContainerEdit
			// 
			this.ClientSize = new System.Drawing.Size(1109, 746);
			this.Controls.Add(this.butAddLabel);
			this.Controls.Add(this.butAddBig);
			this.Controls.Add(this.butAddSmall);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.panelContainer);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMapAreaContainerEdit";
			this.Text = "Map Area Container Edit";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormMapAreaContainerEdit_Load);
			this.panelContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.PanelOD panelContainer;
		private InternalTools.Phones.MapPanel mapPanel;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDescription;
		private UI.Button butEdit;
		private UI.Button butAddSmall;
		private UI.Button butAddBig;
		private UI.Button butAddLabel;
	}
}