namespace OpenDental{
	partial class FormSheetImport {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetImport));
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.pictureBoxSecondaryInsuranceBack = new OpenDental.UI.ODPictureBox();
			this.pictureBoxSecondaryInsuranceFront = new OpenDental.UI.ODPictureBox();
			this.pictureBoxPrimaryInsuranceBack = new OpenDental.UI.ODPictureBox();
			this.pictureBoxPrimaryInsuranceFront = new OpenDental.UI.ODPictureBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(24, 668);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(411, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "Double click to edit an import value.";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(724, 666);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "Import";
			this.butOK.Click += new System.EventHandler(this.butImport_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(27, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridMain.Size = new System.Drawing.Size(772, 648);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Sheet Import";
			this.gridMain.TranslationName = "FormSheetImport";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// pictureBoxSecondaryInsuranceBack
			// 
			this.pictureBoxSecondaryInsuranceBack.Location = new System.Drawing.Point(815, 515);
			this.pictureBoxSecondaryInsuranceBack.Name = "pictureBoxSecondaryInsuranceBack";
			this.pictureBoxSecondaryInsuranceBack.Size = new System.Drawing.Size(216, 162);
			this.pictureBoxSecondaryInsuranceBack.TabIndex = 8;
			this.pictureBoxSecondaryInsuranceBack.Text = "odPictureBox2";
			this.pictureBoxSecondaryInsuranceBack.TextNullImage = null;
			// 
			// pictureBoxSecondaryInsuranceFront
			// 
			this.pictureBoxSecondaryInsuranceFront.Location = new System.Drawing.Point(815, 348);
			this.pictureBoxSecondaryInsuranceFront.Name = "pictureBoxSecondaryInsuranceFront";
			this.pictureBoxSecondaryInsuranceFront.Size = new System.Drawing.Size(216, 162);
			this.pictureBoxSecondaryInsuranceFront.TabIndex = 9;
			this.pictureBoxSecondaryInsuranceFront.Text = "odPictureBox1";
			this.pictureBoxSecondaryInsuranceFront.TextNullImage = null;
			// 
			// pictureBoxPrimaryInsuranceBack
			// 
			this.pictureBoxPrimaryInsuranceBack.Location = new System.Drawing.Point(815, 181);
			this.pictureBoxPrimaryInsuranceBack.Name = "pictureBoxPrimaryInsuranceBack";
			this.pictureBoxPrimaryInsuranceBack.Size = new System.Drawing.Size(216, 162);
			this.pictureBoxPrimaryInsuranceBack.TabIndex = 10;
			this.pictureBoxPrimaryInsuranceBack.Text = "odPictureBox3";
			this.pictureBoxPrimaryInsuranceBack.TextNullImage = null;
			// 
			// pictureBoxPrimaryInsuranceFront
			// 
			this.pictureBoxPrimaryInsuranceFront.Location = new System.Drawing.Point(815, 14);
			this.pictureBoxPrimaryInsuranceFront.Name = "pictureBoxPrimaryInsuranceFront";
			this.pictureBoxPrimaryInsuranceFront.Size = new System.Drawing.Size(216, 162);
			this.pictureBoxPrimaryInsuranceFront.TabIndex = 11;
			this.pictureBoxPrimaryInsuranceFront.Text = "odPictureBox4";
			this.pictureBoxPrimaryInsuranceFront.TextNullImage = null;
			// 
			// FormSheetImport
			// 
			this.ClientSize = new System.Drawing.Size(1046, 696);
			this.Controls.Add(this.pictureBoxPrimaryInsuranceFront);
			this.Controls.Add(this.pictureBoxPrimaryInsuranceBack);
			this.Controls.Add(this.pictureBoxSecondaryInsuranceFront);
			this.Controls.Add(this.pictureBoxSecondaryInsuranceBack);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetImport";
			this.Text = "Sheet Import";
			this.Load += new System.EventHandler(this.FormSheetImport_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private UI.ODPictureBox pictureBoxSecondaryInsuranceBack;
		private UI.ODPictureBox pictureBoxSecondaryInsuranceFront;
		private UI.ODPictureBox pictureBoxPrimaryInsuranceBack;
		private UI.ODPictureBox pictureBoxPrimaryInsuranceFront;
	}
}