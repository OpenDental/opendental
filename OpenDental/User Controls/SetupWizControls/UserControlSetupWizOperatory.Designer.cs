namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizOperatory {
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
			this.label8 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.pictureAdd = new OpenDental.UI.ODPictureBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label3 = new System.Windows.Forms.Label();
			this.butAdvanced = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(20, 28);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(544, 30);
			this.label8.TabIndex = 27;
			this.label8.Text = "Here are your currently set up Operatories. Click \'Add\' to add more.\r\nEach operat" +
    "ory must have a name and an abbreviation.";
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(713, 72);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(82, 26);
			this.butAdd.TabIndex = 28;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(710, 101);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(217, 29);
			this.label2.TabIndex = 30;
			this.label2.Text = "Double click a row to edit the specific operatory.\r\n";
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(18, 61);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(690, 457);
			this.gridMain.TabIndex = 31;
			this.gridMain.Title = "Operatories";
			this.gridMain.TranslationName = "TableProviderSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// pictureAdd
			// 
			this.pictureAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureAdd.HasBorder = false;
			this.pictureAdd.Image = global::OpenDental.Properties.Resources.Left;
			this.pictureAdd.Location = new System.Drawing.Point(792, 75);
			this.pictureAdd.Name = "pictureAdd";
			this.pictureAdd.Size = new System.Drawing.Size(22, 20);
			this.pictureAdd.TabIndex = 32;
			this.pictureAdd.TextNullImage = null;
			this.pictureAdd.Visible = false;
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(533, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(175, 45);
			this.label3.TabIndex = 34;
			this.label3.Text = "Items that need attention are highlighted in red.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAdvanced
			// 
			this.butAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdvanced.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdvanced.Location = new System.Drawing.Point(713, 470);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(82, 26);
			this.butAdvanced.TabIndex = 36;
			this.butAdvanced.Text = "Advanced";
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(710, 400);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(217, 67);
			this.label4.TabIndex = 35;
			this.label4.Text = "Further modifications to this list can be made by going to Setup -> Appointments " +
    "-> Operatories, or clicking \"Advanced\".";
			// 
			// UserControlSetupWizOperatory
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butAdvanced);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.pictureAdd);
			this.Name = "UserControlSetupWizOperatory";
			this.Size = new System.Drawing.Size(930, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizOperatory_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label8;
		private UI.Button butAdd;
		private System.Windows.Forms.Label label2;
		private UI.GridOD gridMain;
		private UI.ODPictureBox pictureAdd;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label label3;
		private UI.Button butAdvanced;
		private System.Windows.Forms.Label label4;
	}
}
