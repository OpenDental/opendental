namespace OpenDental{
	partial class FormTaskListBlocks {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskListBlocks));
			this.butSave = new OpenDental.UI.Button();
			this.treeSubscriptions = new System.Windows.Forms.TreeView();
			this.butSetAll = new OpenDental.UI.Button();
			this.butSetNone = new OpenDental.UI.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(349, 407);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// treeSubscriptions
			// 
			this.treeSubscriptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeSubscriptions.CheckBoxes = true;
			this.treeSubscriptions.FullRowSelect = true;
			this.treeSubscriptions.Location = new System.Drawing.Point(13, 33);
			this.treeSubscriptions.Name = "treeSubscriptions";
			this.treeSubscriptions.Size = new System.Drawing.Size(411, 367);
			this.treeSubscriptions.TabIndex = 4;
			this.treeSubscriptions.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeSubscriptions_NodeMouseClick);
			// 
			// butSetAll
			// 
			this.butSetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSetAll.Location = new System.Drawing.Point(12, 407);
			this.butSetAll.Name = "butSetAll";
			this.butSetAll.Size = new System.Drawing.Size(75, 23);
			this.butSetAll.TabIndex = 5;
			this.butSetAll.Text = "Select All";
			this.butSetAll.UseVisualStyleBackColor = true;
			this.butSetAll.Click += new System.EventHandler(this.butSetAll_Click);
			// 
			// butSetNone
			// 
			this.butSetNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSetNone.Location = new System.Drawing.Point(93, 407);
			this.butSetNone.Name = "butSetNone";
			this.butSetNone.Size = new System.Drawing.Size(75, 23);
			this.butSetNone.TabIndex = 6;
			this.butSetNone.Text = "Select None";
			this.butSetNone.UseVisualStyleBackColor = true;
			this.butSetNone.Click += new System.EventHandler(this.butSetNone_Click);
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(12, 9);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(116, 21);
			this.labelTitle.TabIndex = 7;
			this.labelTitle.Text = "Task Lists";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormTaskListBlocks
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(436, 442);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.butSetNone);
			this.Controls.Add(this.butSetAll);
			this.Controls.Add(this.treeSubscriptions);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskListBlocks";
			this.Text = "Block Task Popups for Subscriptions";
			this.Load += new System.EventHandler(this.FormTaskListBlock_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.TreeView treeSubscriptions;
		private UI.Button butSetAll;
		private UI.Button butSetNone;
		private System.Windows.Forms.Label labelTitle;
	}
}