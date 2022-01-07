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
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.treeSubscriptions = new System.Windows.Forms.TreeView();
			this.butSetAll = new OpenDental.UI.Button();
			this.butSetNone = new OpenDental.UI.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(268, 406);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(349, 406);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
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
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(436, 442);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.butSetNone);
			this.Controls.Add(this.butSetAll);
			this.Controls.Add(this.treeSubscriptions);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskListBlocks";
			this.Text = "Block Task Popups for Subscriptions";
			this.Load += new System.EventHandler(this.FormTaskListBlock_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TreeView treeSubscriptions;
		private UI.Button butSetAll;
		private UI.Button butSetNone;
		private System.Windows.Forms.Label labelTitle;
	}
}