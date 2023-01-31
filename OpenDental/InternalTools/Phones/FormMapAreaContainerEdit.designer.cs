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
			this.label3 = new System.Windows.Forms.Label();
			this.textSite = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textHeight = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textWidth = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.checkSnap = new OpenDental.UI.CheckBox();
			this.butAddLabel = new OpenDental.UI.Button();
			this.butAddBig = new OpenDental.UI.Button();
			this.butAddSmall = new OpenDental.UI.Button();
			this.butEdit = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.mapPanel = new OpenDental.InternalTools.Phones.MapPanel();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(996, 92);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(36, 16);
			this.label3.TabIndex = 128;
			this.label3.Text = "Site";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSite
			// 
			this.textSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textSite.Location = new System.Drawing.Point(1032, 91);
			this.textSite.Name = "textSite";
			this.textSite.ReadOnly = true;
			this.textSite.Size = new System.Drawing.Size(69, 20);
			this.textSite.TabIndex = 127;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(1020, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 16);
			this.label2.TabIndex = 126;
			this.label2.Text = "Height";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHeight
			// 
			this.textHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textHeight.Location = new System.Drawing.Point(1068, 70);
			this.textHeight.Name = "textHeight";
			this.textHeight.ReadOnly = true;
			this.textHeight.Size = new System.Drawing.Size(33, 20);
			this.textHeight.TabIndex = 125;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(1020, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 16);
			this.label1.TabIndex = 124;
			this.label1.Text = "Width";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWidth
			// 
			this.textWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textWidth.Location = new System.Drawing.Point(1068, 49);
			this.textWidth.Name = "textWidth";
			this.textWidth.ReadOnly = true;
			this.textWidth.Size = new System.Drawing.Size(33, 20);
			this.textWidth.TabIndex = 123;
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
			// checkSnap
			// 
			this.checkSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSnap.Checked = true;
			this.checkSnap.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSnap.Location = new System.Drawing.Point(1008, 232);
			this.checkSnap.Name = "checkSnap";
			this.checkSnap.Size = new System.Drawing.Size(102, 18);
			this.checkSnap.TabIndex = 122;
			this.checkSnap.Text = "Snap to feet";
			this.checkSnap.Click += new System.EventHandler(this.checkSnap_Click);
			// 
			// butAddLabel
			// 
			this.butAddLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddLabel.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddLabel.Location = new System.Drawing.Point(1008, 376);
			this.butAddLabel.Name = "butAddLabel";
			this.butAddLabel.Size = new System.Drawing.Size(93, 24);
			this.butAddLabel.TabIndex = 121;
			this.butAddLabel.Text = "Add Label";
			this.butAddLabel.Click += new System.EventHandler(this.butAddLabel_Click);
			// 
			// butAddBig
			// 
			this.butAddBig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddBig.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddBig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddBig.Location = new System.Drawing.Point(1008, 346);
			this.butAddBig.Name = "butAddBig";
			this.butAddBig.Size = new System.Drawing.Size(93, 24);
			this.butAddBig.TabIndex = 120;
			this.butAddBig.Text = "Add Big";
			this.butAddBig.Click += new System.EventHandler(this.butAddBig_Click);
			// 
			// butAddSmall
			// 
			this.butAddSmall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddSmall.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSmall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSmall.Location = new System.Drawing.Point(1008, 316);
			this.butAddSmall.Name = "butAddSmall";
			this.butAddSmall.Size = new System.Drawing.Size(93, 24);
			this.butAddSmall.TabIndex = 119;
			this.butAddSmall.Text = "Add Small";
			this.butAddSmall.Click += new System.EventHandler(this.butAddSmall_Click);
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(1015, 119);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 24);
			this.butEdit.TabIndex = 118;
			this.butEdit.Text = "Edit Details";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
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
			// mapPanel
			// 
			this.mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mapPanel.Location = new System.Drawing.Point(0, 0);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(996, 744);
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
			// FormMapAreaContainerEdit
			// 
			this.ClientSize = new System.Drawing.Size(1109, 746);
			this.Controls.Add(this.mapPanel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textSite);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.checkSnap);
			this.Controls.Add(this.butAddLabel);
			this.Controls.Add(this.butAddBig);
			this.Controls.Add(this.butAddSmall);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMapAreaContainerEdit";
			this.Text = "Map Area Container Edit";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMapAreaContainerEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormMapAreaContainerEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private OpenDental.InternalTools.Phones.MapPanel mapPanel;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDescription;
		private UI.Button butEdit;
		private UI.Button butAddSmall;
		private UI.Button butAddBig;
		private UI.Button butAddLabel;
		private UI.CheckBox checkSnap;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textWidth;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textHeight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textSite;
	}
}