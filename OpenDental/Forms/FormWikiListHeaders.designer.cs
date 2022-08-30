namespace OpenDental{
	partial class FormWikiListHeaders {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiListHeaders));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.gridPickList = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butHideColumn = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(594, 468);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 21;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(675, 468);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 20;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 70);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(388, 364);
			this.gridMain.TabIndex = 26;
			this.gridMain.Title = "Wiki List Headers";
			this.gridMain.TranslationName = "TableWikiListHeaders";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// gridPickList
			// 
			this.gridPickList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPickList.Location = new System.Drawing.Point(406, 70);
			this.gridPickList.Name = "gridPickList";
			this.gridPickList.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridPickList.Size = new System.Drawing.Size(263, 235);
			this.gridPickList.TabIndex = 28;
			this.gridPickList.Title = "Pick List Options";
			this.gridPickList.TranslationName = "TableWikiListHeaderPickList";
			this.gridPickList.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridPickList_CellLeave);
			this.gridPickList.CellEnter += new OpenDental.UI.ODGridClickEventHandler(this.gridPickList_CellEnter);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(675, 162);
			this.butAdd.Name = "butAdd";
			this.butAdd.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butAdd.Size = new System.Drawing.Size(75, 23);
			this.butAdd.TabIndex = 29;
			this.butAdd.Text = "Add";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butRemove
			// 
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemove.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(675, 217);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 23);
			this.butRemove.TabIndex = 30;
			this.butRemove.Text = "Remove";
			this.butRemove.UseVisualStyleBackColor = true;
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(406, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(263, 57);
			this.label1.TabIndex = 31;
			this.label1.Text = "A pick list is only used when you want to restrict user entry in a Wiki List to a" +
    " preset list of options.  The pick list is specific to one item at the left.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(263, 57);
			this.label2.TabIndex = 32;
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.label3.Location = new System.Drawing.Point(12, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(362, 57);
			this.label3.TabIndex = 33;
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butHideColumn
			// 
			this.butHideColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butHideColumn.Enabled = false;
			this.butHideColumn.Location = new System.Drawing.Point(12, 440);
			this.butHideColumn.Name = "butHideColumn";
			this.butHideColumn.Size = new System.Drawing.Size(75, 24);
			this.butHideColumn.TabIndex = 34;
			this.butHideColumn.Text = "Hide Column";
			this.butHideColumn.UseVisualStyleBackColor = true;
			this.butHideColumn.Click += new System.EventHandler(this.butHideColumn_Click);
			// 
			// FormWikiListHeaders
			// 
			this.ClientSize = new System.Drawing.Size(764, 504);
			this.Controls.Add(this.butHideColumn);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridPickList);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiListHeaders";
			this.Text = "Edit Wiki List Headers";
			this.Load += new System.EventHandler(this.FormWikiListHeaders_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.GridOD gridMain;
		private UI.GridOD gridPickList;
		private UI.Button butAdd;
		private UI.Button butRemove;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private UI.Button butHideColumn;
	}
}