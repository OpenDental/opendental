namespace OpenDental{
	partial class FormPhoneComps {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneComps));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.listBoxComputers = new OpenDental.UI.ListBoxOD();
			this.textCustomCompName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butAddCustom = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(488, 550);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(407, 550);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 14);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(282, 560);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "PhoneComp Table";
			this.gridMain.TranslationName = "TablePhoneComps";
			this.gridMain.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellLeave);
			// 
			// listBoxComputers
			// 
			this.listBoxComputers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxComputers.Location = new System.Drawing.Point(407, 38);
			this.listBoxComputers.Name = "listBoxComputers";
			this.listBoxComputers.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxComputers.Size = new System.Drawing.Size(156, 433);
			this.listBoxComputers.TabIndex = 5;
			// 
			// textCustomCompName
			// 
			this.textCustomCompName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textCustomCompName.Location = new System.Drawing.Point(407, 500);
			this.textCustomCompName.Name = "textCustomCompName";
			this.textCustomCompName.Size = new System.Drawing.Size(156, 20);
			this.textCustomCompName.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(407, 476);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(156, 23);
			this.label1.TabIndex = 7;
			this.label1.Text = "Custom Computer Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(407, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(156, 23);
			this.label2.TabIndex = 8;
			this.label2.Text = "Known Computer Names";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Image = global::OpenDental.Properties.Resources.Left;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(309, 226);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 9;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butAddCustom
			// 
			this.butAddCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddCustom.Image = global::OpenDental.Properties.Resources.Left;
			this.butAddCustom.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCustom.Location = new System.Drawing.Point(309, 497);
			this.butAddCustom.Name = "butAddCustom";
			this.butAddCustom.Size = new System.Drawing.Size(75, 26);
			this.butAddCustom.TabIndex = 10;
			this.butAddCustom.Text = "Add";
			this.butAddCustom.Click += new System.EventHandler(this.butAddCustom_Click);
			// 
			// butRemove
			// 
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemove.Image = global::OpenDental.Properties.Resources.Right;
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(309, 290);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 26);
			this.butRemove.TabIndex = 54;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// FormPhoneComps
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(575, 586);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.butAddCustom);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCustomCompName);
			this.Controls.Add(this.listBoxComputers);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneComps";
			this.Text = "Phone Computers";
			this.Load += new System.EventHandler(this.FormPhoneComps_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private UI.ListBoxOD listBoxComputers;
		private System.Windows.Forms.TextBox textCustomCompName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private UI.Button butAdd;
		private UI.Button butAddCustom;
		private UI.Button butRemove;
	}
}