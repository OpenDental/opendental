namespace OpenDental {
	partial class FormAutoNoteExport {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNoteExport));
			this.treeNotes = new System.Windows.Forms.TreeView();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.labelExportSelect = new System.Windows.Forms.Label();
			this.checkCollapse = new System.Windows.Forms.CheckBox();
			this.butExport = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// treeNotes
			// 
			this.treeNotes.AllowDrop = true;
			this.treeNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeNotes.CheckBoxes = true;
			this.treeNotes.ImageIndex = 1;
			this.treeNotes.ImageList = this.imageListTree;
			this.treeNotes.Indent = 12;
			this.treeNotes.Location = new System.Drawing.Point(18, 21);
			this.treeNotes.Name = "treeNotes";
			this.treeNotes.SelectedImageIndex = 1;
			this.treeNotes.Size = new System.Drawing.Size(263, 636);
			this.treeNotes.TabIndex = 0;
			this.treeNotes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.node_AfterCheck);
			// 
			// imageListTree
			// 
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "imageFolder");
			this.imageListTree.Images.SetKeyName(1, "imageText");
			// 
			// labelExportSelect
			// 
			this.labelExportSelect.Location = new System.Drawing.Point(17, 4);
			this.labelExportSelect.Name = "labelExportSelect";
			this.labelExportSelect.Size = new System.Drawing.Size(308, 14);
			this.labelExportSelect.TabIndex = 1;
			this.labelExportSelect.Text = "Select the Auto Notes you would like to export.";
			// 
			// checkCollapse
			// 
			this.checkCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCollapse.Location = new System.Drawing.Point(292, 21);
			this.checkCollapse.Name = "checkCollapse";
			this.checkCollapse.Size = new System.Drawing.Size(100, 20);
			this.checkCollapse.TabIndex = 3;
			this.checkCollapse.Text = "Collapse All";
			this.checkCollapse.UseVisualStyleBackColor = true;
			this.checkCollapse.CheckedChanged += new System.EventHandler(this.checkCollapse_CheckedChanged);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.Location = new System.Drawing.Point(292, 106);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(89, 26);
			this.butExport.TabIndex = 2;
			this.butExport.Text = "Export Selected";
			this.butExport.UseVisualStyleBackColor = true;
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(292, 631);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(89, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butClear
			// 
			this.butClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClear.Location = new System.Drawing.Point(292, 74);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(89, 26);
			this.butClear.TabIndex = 6;
			this.butClear.Text = "Clear Selection";
			this.butClear.UseVisualStyleBackColor = true;
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSelectAll.Location = new System.Drawing.Point(292, 44);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(89, 24);
			this.butSelectAll.TabIndex = 7;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.UseVisualStyleBackColor = true;
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// FormAutoNoteExport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(393, 675);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.checkCollapse);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.labelExportSelect);
			this.Controls.Add(this.treeNotes);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAutoNoteExport";
			this.Text = "Auto Note Export";
			this.Load += new System.EventHandler(this.FormAutoNoteExport_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeNotes;
		private System.Windows.Forms.Label labelExportSelect;
		private System.Windows.Forms.ImageList imageListTree;
		private UI.Button butExport;
		private System.Windows.Forms.CheckBox checkCollapse;
		private UI.Button butCancel;
		private UI.Button butClear;
		private UI.Button butSelectAll;
	}
}