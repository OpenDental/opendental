namespace OpenDental{
	partial class FormAutoNoteCompose {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNoteCompose));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textMain = new System.Windows.Forms.RichTextBox();
			this.butSave = new OpenDental.UI.Button();
			this.treeView = new System.Windows.Forms.TreeView();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.butInsert = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(171, 13);
			this.label1.TabIndex = 110;
			this.label1.Text = "Select Auto Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(371, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 13);
			this.label2.TabIndex = 112;
			this.label2.Text = "Note Text";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textMain
			// 
			this.textMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMain.Location = new System.Drawing.Point(374, 28);
			this.textMain.Name = "textMain";
			this.textMain.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMain.Size = new System.Drawing.Size(447, 540);
			this.textMain.TabIndex = 113;
			this.textMain.Text = "";
			this.textMain.TextChanged += new System.EventHandler(this.textMain_TextChanged);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(746, 577);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// treeView
			// 
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.treeView.HideSelection = false;
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageListTree;
			this.treeView.Indent = 12;
			this.treeView.Location = new System.Drawing.Point(13, 28);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = 0;
			this.treeView.Size = new System.Drawing.Size(250, 540);
			this.treeView.TabIndex = 114;
			this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
			// 
			// imageListTree
			// 
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "imageFolder");
			this.imageListTree.Images.SetKeyName(1, "imageText");
			// 
			// butInsert
			// 
			this.butInsert.Image = global::OpenDental.Properties.Resources.Right;
			this.butInsert.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butInsert.Location = new System.Drawing.Point(283, 292);
			this.butInsert.Name = "butInsert";
			this.butInsert.Size = new System.Drawing.Size(75, 24);
			this.butInsert.TabIndex = 115;
			this.butInsert.Text = "&Insert";
			this.butInsert.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.butInsert.Click += new System.EventHandler(this.butInsert_Click);
			// 
			// FormAutoNoteCompose
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(834, 611);
			this.Controls.Add(this.butInsert);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.textMain);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAutoNoteCompose";
			this.Text = "Compose Auto Note";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutoNoteCompose_FormClosing);
			this.Load += new System.EventHandler(this.FormAutoNoteCompose_Load);
			this.Shown += new System.EventHandler(this.FormAutoNoteCompose_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RichTextBox textMain;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.ImageList imageListTree;
		private UI.Button butInsert;
	}
}