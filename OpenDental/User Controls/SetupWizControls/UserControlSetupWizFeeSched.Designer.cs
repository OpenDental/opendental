namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizFeeSched {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlSetupWizFeeSched));
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butImport = new OpenDental.UI.Button();
			this.butEditFee = new OpenDental.UI.Button();
			this.labelEdit = new System.Windows.Forms.Label();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.labelAdd = new System.Windows.Forms.Label();
			this.labelImport = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(400, 24);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 24);
			this.butAdd.TabIndex = 25;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(21, 7);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(372, 489);
			this.gridMain.TabIndex = 36;
			this.gridMain.Title = "FeeSchedules";
			this.gridMain.TranslationName = "TableFeeScheds";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butImport
			// 
			this.butImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImport.Location = new System.Drawing.Point(400, 128);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(79, 24);
			this.butImport.TabIndex = 37;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butEditFee
			// 
			this.butEditFee.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditFee.Location = new System.Drawing.Point(400, 77);
			this.butEditFee.Name = "butEditFee";
			this.butEditFee.Size = new System.Drawing.Size(79, 24);
			this.butEditFee.TabIndex = 38;
			this.butEditFee.Text = "Edit Fees";
			this.butEditFee.Click += new System.EventHandler(this.butEditFee_Click);
			// 
			// labelEdit
			// 
			this.labelEdit.ImageIndex = 0;
			this.labelEdit.ImageList = this.imageList1;
			this.labelEdit.Location = new System.Drawing.Point(485, 79);
			this.labelEdit.Name = "labelEdit";
			this.labelEdit.Size = new System.Drawing.Size(20, 20);
			this.labelEdit.TabIndex = 94;
			this.labelEdit.Tag = resources.GetString("labelEdit.Tag");
			this.labelEdit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelEdit.Click += new System.EventHandler(this.labelEdit_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "iButton_Blue.png");
			// 
			// labelAdd
			// 
			this.labelAdd.ImageIndex = 0;
			this.labelAdd.ImageList = this.imageList1;
			this.labelAdd.Location = new System.Drawing.Point(485, 26);
			this.labelAdd.Name = "labelAdd";
			this.labelAdd.Size = new System.Drawing.Size(20, 20);
			this.labelAdd.TabIndex = 95;
			this.labelAdd.Tag = resources.GetString("labelAdd.Tag");
			this.labelAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelAdd.Click += new System.EventHandler(this.labelAdd_Click);
			// 
			// labelImport
			// 
			this.labelImport.ImageIndex = 0;
			this.labelImport.ImageList = this.imageList1;
			this.labelImport.Location = new System.Drawing.Point(485, 130);
			this.labelImport.Name = "labelImport";
			this.labelImport.Size = new System.Drawing.Size(20, 20);
			this.labelImport.TabIndex = 96;
			this.labelImport.Tag = resources.GetString("labelImport.Tag");
			this.labelImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelImport.Click += new System.EventHandler(this.labelImport_Click);
			// 
			// UserControlSetupWizFeeSched
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelImport);
			this.Controls.Add(this.labelAdd);
			this.Controls.Add(this.labelEdit);
			this.Controls.Add(this.butEditFee);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAdd);
			this.Name = "UserControlSetupWizFeeSched";
			this.Size = new System.Drawing.Size(834, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizFeeSched_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butAdd;
		private UI.GridOD gridMain;
		private UI.Button butImport;
		private UI.Button butEditFee;
		private System.Windows.Forms.Label labelEdit;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label labelAdd;
		private System.Windows.Forms.Label labelImport;
	}
}
