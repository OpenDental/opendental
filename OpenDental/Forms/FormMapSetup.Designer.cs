namespace OpenDental {
	partial class FormMapSetup {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapSetup));
			this.checkShowGrid = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.numFloorWidthFeet = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.numFloorHeightFeet = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.numPixelsPerFoot = new System.Windows.Forms.NumericUpDown();
			this.checkShowOutline = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newCubicleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.gridEmployees = new OpenDental.UI.GridOD();
			this.mapAreaPanel = new OpenDental.MapAreaPanel();
			this.butCancel = new OpenDental.UI.Button();
			this.butAddMapArea = new OpenDental.UI.Button();
			this.comboRoom = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butSave = new OpenDental.UI.Button();
			this.textDescription = new OpenDental.ODtextBox();
			this.butAddRoom = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.comboSite = new OpenDental.UI.ComboBoxOD();
			this.butAddSmall = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			((System.ComponentModel.ISupportInitialize)(this.numFloorWidthFeet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numFloorHeightFeet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numPixelsPerFoot)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.menu.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkShowGrid
			// 
			this.checkShowGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowGrid.Location = new System.Drawing.Point(9, 93);
			this.checkShowGrid.Name = "checkShowGrid";
			this.checkShowGrid.Size = new System.Drawing.Size(120, 16);
			this.checkShowGrid.TabIndex = 3;
			this.checkShowGrid.Text = "Show Grid";
			this.checkShowGrid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowGrid.UseVisualStyleBackColor = true;
			this.checkShowGrid.CheckedChanged += new System.EventHandler(this.checkShowGrid_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 16);
			this.label2.TabIndex = 15;
			this.label2.Text = "Floor Width (in feet)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numFloorWidthFeet
			// 
			this.numFloorWidthFeet.Location = new System.Drawing.Point(115, 20);
			this.numFloorWidthFeet.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numFloorWidthFeet.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numFloorWidthFeet.Name = "numFloorWidthFeet";
			this.numFloorWidthFeet.Size = new System.Drawing.Size(60, 20);
			this.numFloorWidthFeet.TabIndex = 14;
			this.numFloorWidthFeet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numFloorWidthFeet.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numFloorWidthFeet.ValueChanged += new System.EventHandler(this.numericFloorWidthFeet_ValueChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 17;
			this.label3.Text = "Floor Height (in feet)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numFloorHeightFeet
			// 
			this.numFloorHeightFeet.Location = new System.Drawing.Point(115, 45);
			this.numFloorHeightFeet.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numFloorHeightFeet.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numFloorHeightFeet.Name = "numFloorHeightFeet";
			this.numFloorHeightFeet.Size = new System.Drawing.Size(60, 20);
			this.numFloorHeightFeet.TabIndex = 16;
			this.numFloorHeightFeet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numFloorHeightFeet.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numFloorHeightFeet.ValueChanged += new System.EventHandler(this.numericFloorHeightFeet_ValueChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 18;
			this.label4.Text = "Pixels Per Foot";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numPixelsPerFoot
			// 
			this.numPixelsPerFoot.Location = new System.Drawing.Point(115, 70);
			this.numPixelsPerFoot.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numPixelsPerFoot.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numPixelsPerFoot.Name = "numPixelsPerFoot";
			this.numPixelsPerFoot.Size = new System.Drawing.Size(60, 20);
			this.numPixelsPerFoot.TabIndex = 19;
			this.numPixelsPerFoot.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numPixelsPerFoot.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numPixelsPerFoot.ValueChanged += new System.EventHandler(this.numericPixelsPerFoot_ValueChanged);
			// 
			// checkShowOutline
			// 
			this.checkShowOutline.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowOutline.Checked = true;
			this.checkShowOutline.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowOutline.Location = new System.Drawing.Point(8, 113);
			this.checkShowOutline.Name = "checkShowOutline";
			this.checkShowOutline.Size = new System.Drawing.Size(120, 16);
			this.checkShowOutline.TabIndex = 21;
			this.checkShowOutline.Text = "Show Outline";
			this.checkShowOutline.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowOutline.UseVisualStyleBackColor = true;
			this.checkShowOutline.CheckedChanged += new System.EventHandler(this.checkShowOutline_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.checkShowGrid);
			this.groupBox1.Controls.Add(this.checkShowOutline);
			this.groupBox1.Controls.Add(this.numFloorWidthFeet);
			this.groupBox1.Controls.Add(this.numFloorHeightFeet);
			this.groupBox1.Controls.Add(this.numPixelsPerFoot);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(1392, 34);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(182, 138);
			this.groupBox1.TabIndex = 23;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Preview Different Options";
			// 
			// menu
			// 
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCubicleToolStripMenuItem,
            this.newLabelToolStripMenuItem});
			this.menu.Name = "menu";
			this.menu.Size = new System.Drawing.Size(142, 48);
			// 
			// newCubicleToolStripMenuItem
			// 
			this.newCubicleToolStripMenuItem.Name = "newCubicleToolStripMenuItem";
			this.newCubicleToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.newCubicleToolStripMenuItem.Text = "New Cubicle";
			this.newCubicleToolStripMenuItem.Click += new System.EventHandler(this.newCubicleToolStripMenuItem_Click);
			// 
			// newLabelToolStripMenuItem
			// 
			this.newLabelToolStripMenuItem.Name = "newLabelToolStripMenuItem";
			this.newLabelToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.newLabelToolStripMenuItem.Text = "New Label";
			this.newLabelToolStripMenuItem.Click += new System.EventHandler(this.newLabelToolStripMenuItem_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(378, 13);
			this.label1.TabIndex = 26;
			this.label1.Text = "Double-click an item on the map to edit. Right-click the map to add a new item.";
			// 
			// gridEmployees
			// 
			this.gridEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEmployees.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gridEmployees.Location = new System.Drawing.Point(1392, 201);
			this.gridEmployees.Name = "gridEmployees";
			this.gridEmployees.Size = new System.Drawing.Size(182, 727);
			this.gridEmployees.TabIndex = 25;
			this.gridEmployees.Title = "Employees";
			this.gridEmployees.TranslationName = "TableEmployees";
			// 
			// mapAreaPanel
			// 
			this.mapAreaPanel.AllowDragging = true;
			this.mapAreaPanel.AllowEditing = true;
			this.mapAreaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mapAreaPanel.AutoScrollMinSize = new System.Drawing.Size(1326, 935);
			this.mapAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mapAreaPanel.FloorColor = System.Drawing.Color.White;
			this.mapAreaPanel.FloorHeightFeet = 55;
			this.mapAreaPanel.FloorWidthFeet = 78;
			this.mapAreaPanel.Font = new System.Drawing.Font("Calibri", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapAreaPanel.FontCubicle = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapAreaPanel.FontCubicleHeader = new System.Drawing.Font("Calibri", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapAreaPanel.FontLabel = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapAreaPanel.GridColor = System.Drawing.Color.LightGray;
			this.mapAreaPanel.Location = new System.Drawing.Point(12, 34);
			this.mapAreaPanel.Name = "mapAreaPanel";
			this.mapAreaPanel.PixelsPerFoot = 17;
			this.mapAreaPanel.ShowGrid = false;
			this.mapAreaPanel.ShowOutline = true;
			this.mapAreaPanel.Size = new System.Drawing.Size(1374, 969);
			this.mapAreaPanel.TabIndex = 4;
			this.mapAreaPanel.MapAreaChanged += new System.EventHandler(this.mapAreaPanel_MapAreaChanged);
			this.mapAreaPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapAreaPanel_MouseUp);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1499, 1019);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 28;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAddMapArea
			// 
			this.butAddMapArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddMapArea.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddMapArea.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddMapArea.Location = new System.Drawing.Point(1499, 930);
			this.butAddMapArea.Name = "butAddMapArea";
			this.butAddMapArea.Size = new System.Drawing.Size(75, 24);
			this.butAddMapArea.TabIndex = 49;
			this.butAddMapArea.Text = "Add";
			this.butAddMapArea.Click += new System.EventHandler(this.butAddMapArea_Click);
			// 
			// comboRoom
			// 
			this.comboRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboRoom.ForeColor = System.Drawing.Color.Black;
			this.comboRoom.ItemHeight = 13;
			this.comboRoom.Location = new System.Drawing.Point(1252, 8);
			this.comboRoom.MaxDropDownItems = 30;
			this.comboRoom.Name = "comboRoom";
			this.comboRoom.Size = new System.Drawing.Size(134, 21);
			this.comboRoom.TabIndex = 50;
			this.comboRoom.SelectionChangeCommitted += new System.EventHandler(this.comboRoom_SelectionChangeCommitted);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(1158, 11);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(91, 16);
			this.label5.TabIndex = 22;
			this.label5.Text = "Select Room:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(1499, 989);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 51;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textDescription
			// 
			this.textDescription.AcceptsTab = true;
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.BackColor = System.Drawing.SystemColors.Window;
			this.textDescription.DetectLinksEnabled = false;
			this.textDescription.DetectUrls = false;
			this.textDescription.Location = new System.Drawing.Point(1392, 8);
			this.textDescription.Multiline = false;
			this.textDescription.Name = "textDescription";
			this.textDescription.QuickPasteType = OpenDentBusiness.QuickPasteType.Office;
			this.textDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(181, 20);
			this.textDescription.SpellCheckIsEnabled = false;
			this.textDescription.TabIndex = 53;
			this.textDescription.Text = "";
			this.textDescription.TextChanged += new System.EventHandler(this.textDescription_TextChanged);
			// 
			// butAddRoom
			// 
			this.butAddRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddRoom.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddRoom.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddRoom.Location = new System.Drawing.Point(1056, 5);
			this.butAddRoom.Name = "butAddRoom";
			this.butAddRoom.Size = new System.Drawing.Size(96, 24);
			this.butAddRoom.TabIndex = 54;
			this.butAddRoom.Text = "Add Room";
			this.butAddRoom.Click += new System.EventHandler(this.butAddRoom_Click);
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(1392, 177);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(45, 16);
			this.label8.TabIndex = 57;
			this.label8.Text = "Site";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboSite.ForeColor = System.Drawing.Color.Black;
			this.comboSite.Location = new System.Drawing.Point(1440, 174);
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(134, 21);
			this.comboSite.TabIndex = 58;
			this.comboSite.SelectionChangeCommitted += new System.EventHandler(this.comboSite_SelectionChangeCommitted);
			// 
			// butAddSmall
			// 
			this.butAddSmall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddSmall.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSmall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSmall.Location = new System.Drawing.Point(1491, 960);
			this.butAddSmall.Name = "butAddSmall";
			this.butAddSmall.Size = new System.Drawing.Size(83, 24);
			this.butAddSmall.TabIndex = 59;
			this.butAddSmall.Text = "Add Small";
			this.butAddSmall.Click += new System.EventHandler(this.ButAddSmall_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(396, 5);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(106, 24);
			this.butDelete.TabIndex = 62;
			this.butDelete.Text = "Delete Room";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.ButDelete_Click);
			// 
			// FormMapSetup
			// 
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1579, 1045);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAddSmall);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.butAddRoom);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboRoom);
			this.Controls.Add(this.butAddMapArea);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridEmployees);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.mapAreaPanel);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMapSetup";
			this.Text = "Map Setup";
			this.Load += new System.EventHandler(this.FormMapSetup_Load);
			this.ResizeEnd += new System.EventHandler(this.FormMapSetup_ResizeEnd);
			((System.ComponentModel.ISupportInitialize)(this.numFloorWidthFeet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numFloorHeightFeet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numPixelsPerFoot)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.menu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MapAreaPanel mapAreaPanel;
		private System.Windows.Forms.CheckBox checkShowGrid;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numFloorWidthFeet;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numFloorHeightFeet;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numPixelsPerFoot;
		private System.Windows.Forms.CheckBox checkShowOutline;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ContextMenuStrip menu;
		private System.Windows.Forms.ToolStripMenuItem newCubicleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newLabelToolStripMenuItem;
		private UI.GridOD gridEmployees;
		private System.Windows.Forms.Label label1;
		private UI.Button butCancel;
		private UI.Button butAddMapArea;
		private System.Windows.Forms.ComboBox comboRoom;
		private System.Windows.Forms.Label label5;
		private UI.Button butSave;
		private ODtextBox textDescription;
		private UI.Button butAddRoom;
		private System.Windows.Forms.Label label8;
		private UI.ComboBoxOD comboSite;
		private UI.Button butAddSmall;
		private UI.Button butDelete;
	}
}