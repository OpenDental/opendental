using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutoNotes {
	private System.ComponentModel.IContainer components;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing )
			{
				if(components != null)	{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoNotes));
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.labelSelection = new System.Windows.Forms.Label();
			this.treeNotes = new System.Windows.Forms.TreeView();
			this.butClose = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.checkCollapse = new System.Windows.Forms.CheckBox();
			this.butExport = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// imageListTree
			// 
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTree.Images.SetKeyName(0, "imageFolder");
			this.imageListTree.Images.SetKeyName(1, "imageText");
			// 
			// labelSelection
			// 
			this.labelSelection.Location = new System.Drawing.Point(15, 4);
			this.labelSelection.Name = "labelSelection";
			this.labelSelection.Size = new System.Drawing.Size(268, 14);
			this.labelSelection.TabIndex = 8;
			this.labelSelection.Text = "Select an Auto Note by double clicking.";
			this.labelSelection.Visible = false;
			// 
			// treeNotes
			// 
			this.treeNotes.AllowDrop = true;
			this.treeNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeNotes.HideSelection = false;
			this.treeNotes.ImageIndex = 1;
			this.treeNotes.ImageList = this.imageListTree;
			this.treeNotes.Indent = 12;
			this.treeNotes.Location = new System.Drawing.Point(18, 21);
			this.treeNotes.Name = "treeNotes";
			this.treeNotes.SelectedImageIndex = 1;
			this.treeNotes.Size = new System.Drawing.Size(307, 641);
			this.treeNotes.TabIndex = 2;
			this.treeNotes.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeNotes_ItemDrag);
			this.treeNotes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeNotes_MouseDoubleClick);
			this.treeNotes.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeNotes_DragDrop);
			this.treeNotes.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeNotes_DragEnter);
			this.treeNotes.DragOver += new System.Windows.Forms.DragEventHandler(this.treeNotes_DragOver);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(340, 637);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(79, 26);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(340, 57);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 26);
			this.butAdd.TabIndex = 7;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// checkCollapse
			// 
			this.checkCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCollapse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCollapse.Location = new System.Drawing.Point(340, 21);
			this.checkCollapse.Name = "checkCollapse";
			this.checkCollapse.Size = new System.Drawing.Size(79, 20);
			this.checkCollapse.TabIndex = 227;
			this.checkCollapse.Text = "Collapse All";
			this.checkCollapse.UseVisualStyleBackColor = true;
			this.checkCollapse.CheckedChanged += new System.EventHandler(this.checkCollapse_CheckedChanged);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.Location = new System.Drawing.Point(340, 89);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 26);
			this.butExport.TabIndex = 228;
			this.butExport.Text = "Export";
			this.butExport.UseVisualStyleBackColor = true;
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butImport
			// 
			this.butImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butImport.Location = new System.Drawing.Point(340, 121);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(79, 26);
			this.butImport.TabIndex = 229;
			this.butImport.Text = "Import";
			this.butImport.UseVisualStyleBackColor = true;
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// FormAutoNotes
			// 
			this.ClientSize = new System.Drawing.Size(431, 675);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.checkCollapse);
			this.Controls.Add(this.labelSelection);
			this.Controls.Add(this.treeNotes);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAutoNotes";
			this.ShowInTaskbar = false;
			this.Text = "Auto Notes";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutoNotes_FormClosing);
			this.Load += new System.EventHandler(this.FormAutoNotes_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TreeView treeNotes;
		private OpenDental.UI.Button butAdd;
		private Label labelSelection;
		private ImageList imageListTree;
		private CheckBox checkCollapse;
		private UI.Button butExport;
		private UI.Button butImport;
	}
}
