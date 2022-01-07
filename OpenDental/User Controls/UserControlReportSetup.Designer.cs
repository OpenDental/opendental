namespace OpenDental.User_Controls {
	partial class UserControlReportSetup {
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
			this.labelODInternal = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.labelUserGroup = new System.Windows.Forms.Label();
			this.comboUserGroup = new System.Windows.Forms.ComboBox();
			this.gridProdInc = new OpenDental.UI.GridOD();
			this.gridDaily = new OpenDental.UI.GridOD();
			this.gridMonthly = new OpenDental.UI.GridOD();
			this.gridLists = new OpenDental.UI.GridOD();
			this.butDown = new OpenDental.UI.Button();
			this.gridPublicHealth = new OpenDental.UI.GridOD();
			this.butUp = new OpenDental.UI.Button();
			this.butSetAll = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelODInternal
			// 
			this.labelODInternal.Location = new System.Drawing.Point(311, 512);
			this.labelODInternal.Name = "labelODInternal";
			this.labelODInternal.Size = new System.Drawing.Size(161, 15);
			this.labelODInternal.TabIndex = 222;
			this.labelODInternal.Text = "None";
			this.labelODInternal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(264, 462);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(255, 42);
			this.label1.TabIndex = 221;
			this.label1.Text = "Move the selected item within its list.\r\nThe current selection\'s internal name is" +
    ":";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelUserGroup
			// 
			this.labelUserGroup.Location = new System.Drawing.Point(10, 5);
			this.labelUserGroup.Name = "labelUserGroup";
			this.labelUserGroup.Size = new System.Drawing.Size(100, 23);
			this.labelUserGroup.TabIndex = 224;
			this.labelUserGroup.Text = "User Group";
			this.labelUserGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUserGroup
			// 
			this.comboUserGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUserGroup.Location = new System.Drawing.Point(111, 5);
			this.comboUserGroup.Name = "comboUserGroup";
			this.comboUserGroup.Size = new System.Drawing.Size(147, 21);
			this.comboUserGroup.TabIndex = 225;
			this.comboUserGroup.SelectionChangeCommitted += new System.EventHandler(this.comboUserGroup_SelectionChangeCommitted);
			// 
			// gridProdInc
			// 
			this.gridProdInc.HasMultilineHeaders = true;
			this.gridProdInc.Location = new System.Drawing.Point(3, 35);
			this.gridProdInc.Name = "gridProdInc";
			this.gridProdInc.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridProdInc.Size = new System.Drawing.Size(255, 151);
			this.gridProdInc.TabIndex = 214;
			this.gridProdInc.Title = "Production & Income";
			this.gridProdInc.TranslationName = "TableProductionIncome";
			this.gridProdInc.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			this.gridProdInc.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellLeave);
			// 
			// gridDaily
			// 
			this.gridDaily.HasMultilineHeaders = true;
			this.gridDaily.Location = new System.Drawing.Point(3, 192);
			this.gridDaily.Name = "gridDaily";
			this.gridDaily.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridDaily.Size = new System.Drawing.Size(255, 151);
			this.gridDaily.TabIndex = 215;
			this.gridDaily.Title = "Daily";
			this.gridDaily.TranslationName = "TableDaily";
			this.gridDaily.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			this.gridDaily.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellLeave);
			// 
			// gridMonthly
			// 
			this.gridMonthly.HasMultilineHeaders = true;
			this.gridMonthly.Location = new System.Drawing.Point(3, 346);
			this.gridMonthly.Name = "gridMonthly";
			this.gridMonthly.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMonthly.Size = new System.Drawing.Size(255, 261);
			this.gridMonthly.TabIndex = 216;
			this.gridMonthly.Title = "Monthly";
			this.gridMonthly.TranslationName = "TableMonthly";
			this.gridMonthly.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			this.gridMonthly.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellLeave);
			// 
			// gridLists
			// 
			this.gridLists.HasMultilineHeaders = true;
			this.gridLists.Location = new System.Drawing.Point(267, 35);
			this.gridLists.Name = "gridLists";
			this.gridLists.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridLists.Size = new System.Drawing.Size(255, 308);
			this.gridLists.TabIndex = 217;
			this.gridLists.Title = "Lists";
			this.gridLists.TranslationName = "TableLists";
			this.gridLists.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			this.gridLists.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellLeave);
			// 
			// butDown
			// 
			this.butDown.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(448, 5);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(71, 24);
			this.butDown.TabIndex = 220;
			this.butDown.Text = "Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// gridPublicHealth
			// 
			this.gridPublicHealth.HasMultilineHeaders = true;
			this.gridPublicHealth.Location = new System.Drawing.Point(267, 346);
			this.gridPublicHealth.Name = "gridPublicHealth";
			this.gridPublicHealth.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridPublicHealth.Size = new System.Drawing.Size(255, 103);
			this.gridPublicHealth.TabIndex = 218;
			this.gridPublicHealth.Title = "Public Health";
			this.gridPublicHealth.TranslationName = "TableHealth";
			this.gridPublicHealth.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			this.gridPublicHealth.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellLeave);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(371, 5);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(71, 24);
			this.butUp.TabIndex = 219;
			this.butUp.Text = "Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butSetAll
			// 
			this.butSetAll.Location = new System.Drawing.Point(267, 5);
			this.butSetAll.Name = "butSetAll";
			this.butSetAll.Size = new System.Drawing.Size(79, 24);
			this.butSetAll.TabIndex = 223;
			this.butSetAll.Text = "Set All";
			this.butSetAll.Click += new System.EventHandler(this.butSetAll_Click);
			// 
			// UserControlReportSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboUserGroup);
			this.Controls.Add(this.labelUserGroup);
			this.Controls.Add(this.gridProdInc);
			this.Controls.Add(this.gridDaily);
			this.Controls.Add(this.labelODInternal);
			this.Controls.Add(this.gridMonthly);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridLists);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.gridPublicHealth);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butSetAll);
			this.Name = "UserControlReportSetup";
			this.Size = new System.Drawing.Size(525, 610);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butUp;
		private UI.GridOD gridPublicHealth;
		private UI.Button butDown;
		private UI.GridOD gridLists;
		private System.Windows.Forms.Label label1;
		private UI.GridOD gridMonthly;
		private System.Windows.Forms.Label labelODInternal;
		private UI.GridOD gridDaily;
		private UI.GridOD gridProdInc;
		private UI.Button butSetAll;
		private System.Windows.Forms.Label labelUserGroup;
		private System.Windows.Forms.ComboBox comboUserGroup;
	}
}
