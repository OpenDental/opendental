namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizEmployee {
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
			this.timerBlink = new System.Windows.Forms.Timer(this.components);
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.pictureAdd = new OpenDental.UI.ODPictureBox();
			this.butAdvanced = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// timerBlink
			// 
			this.timerBlink.Interval = 500;
			this.timerBlink.Tick += new System.EventHandler(this.timerBlink_Tick);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(166, 55);
			this.label3.TabIndex = 34;
			this.label3.Text = "Items that need attention are highlighted in red.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(695, 92);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(214, 66);
			this.label2.TabIndex = 30;
			this.label2.Text = "Double click a row to set up the specific employee.\r\n";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(694, 158);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(215, 77);
			this.label1.TabIndex = 29;
			this.label1.Text = "Further modifications to this list can be made by going from Lists -> Employees.\r" +
    "\n";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(203, 66);
			this.label8.TabIndex = 27;
			this.label8.Text = "Here is the list of your current employees. Click \'Add\' to add more employees. \r\n" +
    "Each employee must have a First Name and a Last Name.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(224, 19);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(460, 472);
			this.gridMain.TabIndex = 14;
			this.gridMain.Title = "Employees";
			this.gridMain.TranslationName = "TableProviderSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(698, 19);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(82, 26);
			this.butAdd.TabIndex = 28;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// pictureAdd
			// 
			this.pictureAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureAdd.HasBorder = false;
			this.pictureAdd.Image = global::OpenDental.Properties.Resources.Left;
			this.pictureAdd.Location = new System.Drawing.Point(781, 23);
			this.pictureAdd.Name = "pictureAdd";
			this.pictureAdd.Size = new System.Drawing.Size(22, 20);
			this.pictureAdd.TabIndex = 31;
			this.pictureAdd.TextNullImage = null;
			this.pictureAdd.Visible = false;
			// 
			// butAdvanced
			// 
			this.butAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdvanced.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdvanced.Location = new System.Drawing.Point(698, 464);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(82, 26);
			this.butAdvanced.TabIndex = 38;
			this.butAdvanced.Text = "Advanced";
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(695, 394);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(217, 67);
			this.label4.TabIndex = 37;
			this.label4.Text = "Further modifications to this list can be made by going to Lists -> Employees, or" +
    " clicking \"Advanced\".";
			// 
			// UserControlSetupWizEmployee
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butAdvanced);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.pictureAdd);
			this.Name = "UserControlSetupWizEmployee";
			this.Size = new System.Drawing.Size(930, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizEmployee_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label8;
		private UI.Button butAdd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Timer timerBlink;
		private UI.ODPictureBox pictureAdd;
		private System.Windows.Forms.Label label3;
		private UI.Button butAdvanced;
		private System.Windows.Forms.Label label4;
	}
}
