namespace OpenDental{
	partial class BasicTemplate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicTemplate));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.splitContainer = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.tabControl = new OpenDental.UI.TabControl();
			this.tabPage1 = new OpenDental.UI.TabPage();
			this.tabPage2 = new OpenDental.UI.TabPage();
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainer.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1143, 630);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1143, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// splitContainer
			// 
			this.splitContainer.Controls.Add(this.splitterPanel1);
			this.splitContainer.Controls.Add(this.splitterPanel2);
			this.splitContainer.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainer.Location = new System.Drawing.Point(148, 90);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer.Panel1 = this.splitterPanel1;
			this.splitContainer.Panel2 = this.splitterPanel2;
			this.splitContainer.Size = new System.Drawing.Size(158, 116);
			this.splitContainer.SplitterDistance = 57;
			this.splitContainer.TabIndex = 11;
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(158, 57);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Location = new System.Drawing.Point(0, 61);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(158, 55);
			this.splitterPanel2.TabIndex = 14;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(154, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(143, 65);
			this.label2.TabIndex = 3;
			this.label2.Text = "This is a UI.SplitContainer along with its 2 SplitterPanels, that you can copy/pa" +
    "ste into any form.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Location = new System.Drawing.Point(386, 90);
			this.tabControl.Name = "tabControl";
			this.tabControl.Size = new System.Drawing.Size(240, 115);
			this.tabControl.TabIndex = 74;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage1.Location = new System.Drawing.Point(2, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(236, 92);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage2.Location = new System.Drawing.Point(2, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(236, 92);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(416, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(169, 53);
			this.label1.TabIndex = 4;
			this.label1.Text = "This is a UI.TabControl along with its 2 TabPages, that you can copy/paste into a" +
    "ny form.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BasicTemplate
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "BasicTemplate";
			this.Text = "Basic Template";
			this.splitContainer.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.SplitContainer splitContainer;
		private UI.SplitterPanel splitterPanel1;
		private System.Windows.Forms.Label label2;
		private UI.SplitterPanel splitterPanel2;
		private UI.TabControl tabControl;
		private UI.TabPage tabPage1;
		private UI.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
	}
}