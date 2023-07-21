namespace UnitTests
{
	partial class FormUIManagerTests
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUIManagerTests));
			this.button2 = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox3 = new OpenDental.UI.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBox6 = new OpenDental.UI.CheckBox();
			this.button3 = new OpenDental.UI.Button();
			this.listBox1 = new OpenDental.UI.ListBox();
			this.comboBox1 = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tabControl = new OpenDental.UI.TabControl();
			this.tabPage1 = new OpenDental.UI.TabPage();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioStudentN = new System.Windows.Forms.RadioButton();
			this.checkBox1 = new OpenDental.UI.CheckBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.tabPage2 = new OpenDental.UI.TabPage();
			this.panel4 = new System.Windows.Forms.Panel();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.checkBox2 = new OpenDental.UI.CheckBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemChild = new System.Windows.Forms.ToolStripMenuItem();
			this.openPaymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.voidPaymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.processReturnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.button1 = new OpenDental.UI.Button();
			this.button4 = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.toolBarMain = new OpenDental.UI.ToolBarOD();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.textVDateFrom = new OpenDental.ValidDate();
			this.label4 = new System.Windows.Forms.Label();
			this.testForWpf1 = new OpenDental.UI.TestForWpf();
			this.testForWpf2 = new OpenDental.UI.TestForWpf();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.butGetTag = new OpenDental.UI.Button();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.butLaunchWin = new OpenDental.UI.Button();
			this.butToggleVis = new OpenDental.UI.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.textDouble = new OpenDental.ValidDouble();
			this.label14 = new System.Windows.Forms.Label();
			this.textVInt = new OpenDental.ValidNum();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.groupBox3.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel4.SuspendLayout();
			this.contextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// button2
			// 
			this.button2.Icon = OpenDental.UI.EnumIcons.CommLog;
			this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button2.Location = new System.Drawing.Point(155, 55);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(85, 48);
			this.button2.TabIndex = 5;
			this.button2.Text = "&Set 3 selected";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.LightBlue;
			this.label1.Location = new System.Drawing.Point(701, 348);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 45);
			this.label1.TabIndex = 1;
			this.label1.Text = "Here\'s multiline label so we can check alignment";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.Location = new System.Drawing.Point(2, 157);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(107, 39);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "text in box";
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(115, 157);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(135, 110);
			this.groupBox3.TabIndex = 58;
			this.groupBox3.Text = "My Group Box";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(127, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "Label in my groupBox";
			// 
			// checkBox6
			// 
			this.checkBox6.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox6.Checked = true;
			this.checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox6.Enabled = false;
			this.checkBox6.Location = new System.Drawing.Point(216, 106);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new System.Drawing.Size(162, 33);
			this.checkBox6.TabIndex = 60;
			this.checkBox6.Text = "CheckBox6 with lots of text that wraps";
			this.checkBox6.Click += new System.EventHandler(this.checkBox6_Click);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button3.Location = new System.Drawing.Point(783, 564);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(81, 24);
			this.button3.TabIndex = 61;
			this.button3.Text = "Cancel";
			this.button3.UseVisualStyleBackColor = false;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// listBox1
			// 
			this.listBox1.Enabled = false;
			this.listBox1.ItemStrings = new string[] {
        "First",
        "Second",
        "Third",
        "Fourth",
        "Fifth",
        "Sixth",
        "Seventh",
        "Eighth",
        "Ninth"};
			this.listBox1.Location = new System.Drawing.Point(258, 146);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(120, 121);
			this.listBox1.TabIndex = 64;
			// 
			// comboBox1
			// 
			this.comboBox1.BackColor = System.Drawing.SystemColors.Window;
			this.comboBox1.Location = new System.Drawing.Point(2, 279);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.SelectionModeMulti = true;
			this.comboBox1.Size = new System.Drawing.Size(107, 21);
			this.comboBox1.TabIndex = 73;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(-1, 258);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 18);
			this.label3.TabIndex = 2;
			this.label3.Text = "ComboBox";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControl.Location = new System.Drawing.Point(393, 70);
			this.tabControl.Name = "tabControl";
			this.tabControl.Size = new System.Drawing.Size(185, 143);
			this.tabControl.TabIndex = 76;
			// 
			// tabPage1
			// 
			this.tabPage1.AutoScroll = true;
			this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage1.Controls.Add(this.radioButton1);
			this.tabPage1.Controls.Add(this.radioStudentN);
			this.tabPage1.Controls.Add(this.checkBox1);
			this.tabPage1.Controls.Add(this.textBox2);
			this.tabPage1.Location = new System.Drawing.Point(2, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(181, 120);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(29, 96);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(117, 16);
			this.radioButton1.TabIndex = 64;
			this.radioButton1.Text = "Radio2";
			// 
			// radioStudentN
			// 
			this.radioStudentN.Checked = true;
			this.radioStudentN.Enabled = false;
			this.radioStudentN.Location = new System.Drawing.Point(29, 77);
			this.radioStudentN.Name = "radioStudentN";
			this.radioStudentN.Size = new System.Drawing.Size(117, 16);
			this.radioStudentN.TabIndex = 63;
			this.radioStudentN.TabStop = true;
			this.radioStudentN.Text = "Radio";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(16, 41);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(96, 18);
			this.checkBox1.TabIndex = 61;
			this.checkBox1.Text = "checkBox1";
			this.checkBox1.ThreeState = true;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(29, 15);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 20);
			this.textBox2.TabIndex = 62;
			this.textBox2.Text = "text in box";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage2.Location = new System.Drawing.Point(2, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(181, 120);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			// 
			// panel4
			// 
			this.panel4.AutoScroll = true;
			this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.textBox4);
			this.panel4.Controls.Add(this.checkBox2);
			this.panel4.Location = new System.Drawing.Point(584, 86);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(182, 110);
			this.panel4.TabIndex = 80;
			this.panel4.Click += new System.EventHandler(this.panel4_Click);
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(11, 100);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(100, 20);
			this.textBox4.TabIndex = 63;
			this.textBox4.Text = "text in box";
			// 
			// checkBox2
			// 
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Location = new System.Drawing.Point(16, 19);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(131, 18);
			this.checkBox2.TabIndex = 61;
			this.checkBox2.Text = "checkBox2";
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(60, 55);
			this.textBox3.Name = "textBox3";
			this.textBox3.ReadOnly = true;
			this.textBox3.Size = new System.Drawing.Size(90, 20);
			this.textBox3.TabIndex = 3;
			this.textBox3.Text = "text in box";
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.ArrowsWhenNoFocus = true;
			this.gridMain.ContextMenuStrip = this.contextMenu;
			this.gridMain.HasDropDowns = true;
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(115, 279);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.ShowContextMenu = false;
			this.gridMain.Size = new System.Drawing.Size(263, 226);
			this.gridMain.TabIndex = 81;
			this.gridMain.Title = "New Grid";
			this.gridMain.TranslationName = "test";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGoTo,
            this.openPaymentToolStripMenuItem,
            this.voidPaymentToolStripMenuItem,
            this.processReturnToolStripMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(154, 92);
			this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemChild});
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(153, 22);
			this.menuItemGoTo.Text = "Go To Account";
			// 
			// menuItemChild
			// 
			this.menuItemChild.Name = "menuItemChild";
			this.menuItemChild.Size = new System.Drawing.Size(129, 22);
			this.menuItemChild.Text = "Child Item";
			this.menuItemChild.Click += new System.EventHandler(this.menuItemChild_Click);
			// 
			// openPaymentToolStripMenuItem
			// 
			this.openPaymentToolStripMenuItem.Name = "openPaymentToolStripMenuItem";
			this.openPaymentToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.openPaymentToolStripMenuItem.Text = "Open Payment";
			this.openPaymentToolStripMenuItem.Click += new System.EventHandler(this.openPaymentToolStripMenuItem_Click);
			// 
			// voidPaymentToolStripMenuItem
			// 
			this.voidPaymentToolStripMenuItem.Name = "voidPaymentToolStripMenuItem";
			this.voidPaymentToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.voidPaymentToolStripMenuItem.Text = "Void Payment";
			this.voidPaymentToolStripMenuItem.Click += new System.EventHandler(this.voidPaymentToolStripMenuItem_Click);
			// 
			// processReturnToolStripMenuItem
			// 
			this.processReturnToolStripMenuItem.Name = "processReturnToolStripMenuItem";
			this.processReturnToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.processReturnToolStripMenuItem.Text = "Process Return";
			// 
			// button1
			// 
			this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button1.Location = new System.Drawing.Point(681, 564);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(85, 24);
			this.button1.TabIndex = 82;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click_2);
			// 
			// button4
			// 
			this.button4.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button4.Location = new System.Drawing.Point(2, 529);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(85, 24);
			this.button4.TabIndex = 83;
			this.button4.Text = "Delete";
			this.button4.UseVisualStyleBackColor = false;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(864, 24);
			this.menuMain.TabIndex = 84;
			// 
			// toolBarMain
			// 
			this.toolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarMain.Location = new System.Drawing.Point(0, 24);
			this.toolBarMain.Name = "toolBarMain";
			this.toolBarMain.Size = new System.Drawing.Size(864, 25);
			this.toolBarMain.TabIndex = 85;
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "editPencil.gif");
			// 
			// textVDateFrom
			// 
			this.textVDateFrom.Location = new System.Drawing.Point(60, 77);
			this.textVDateFrom.Name = "textVDateFrom";
			this.textVDateFrom.Size = new System.Drawing.Size(90, 20);
			this.textVDateFrom.TabIndex = 2;
			this.textVDateFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(-44, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(98, 18);
			this.label4.TabIndex = 145;
			this.label4.Text = "validDate";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// testForWpf1
			// 
			this.testForWpf1.Bitmap_ = ((System.Drawing.Bitmap)(resources.GetObject("testForWpf1.Bitmap_")));
			this.testForWpf1.Location = new System.Drawing.Point(424, 227);
			this.testForWpf1.Name = "testForWpf1";
			this.testForWpf1.Size = new System.Drawing.Size(98, 24);
			this.testForWpf1.TabIndex = 146;
			this.testForWpf1.Text = "testForWpf1";
			// 
			// testForWpf2
			// 
			this.testForWpf2.Location = new System.Drawing.Point(537, 227);
			this.testForWpf2.Name = "testForWpf2";
			this.testForWpf2.Size = new System.Drawing.Size(98, 24);
			this.testForWpf2.TabIndex = 147;
			this.testForWpf2.Text = "testForWpf2";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.LightBlue;
			this.label5.Location = new System.Drawing.Point(701, 397);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(145, 45);
			this.label5.TabIndex = 148;
			this.label5.Text = "Here\'s multiline label so we can check alignment";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.LightBlue;
			this.label6.Location = new System.Drawing.Point(701, 446);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(145, 45);
			this.label6.TabIndex = 149;
			this.label6.Text = "Here\'s multiline label so we can check alignment";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.Color.LightBlue;
			this.label7.Location = new System.Drawing.Point(552, 348);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(145, 45);
			this.label7.TabIndex = 150;
			this.label7.Text = "Here\'s multiline label so we can check alignment";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.Color.LightBlue;
			this.label8.Location = new System.Drawing.Point(403, 348);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(145, 45);
			this.label8.TabIndex = 151;
			this.label8.Text = "Here\'s multiline label so we can check alignment";
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.Color.LightBlue;
			this.label9.Location = new System.Drawing.Point(403, 397);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(145, 45);
			this.label9.TabIndex = 152;
			this.label9.Text = "Here\'s multiline label so we can check alignment";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.BackColor = System.Drawing.Color.LightBlue;
			this.label10.Location = new System.Drawing.Point(552, 397);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(145, 45);
			this.label10.TabIndex = 153;
			this.label10.Text = "Here\'s multiline label so we can check alignment";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label11
			// 
			this.label11.BackColor = System.Drawing.Color.LightBlue;
			this.label11.Location = new System.Drawing.Point(552, 446);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(145, 45);
			this.label11.TabIndex = 154;
			this.label11.Text = "Here\'s multiline label so we can check alignment";
			this.label11.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label12
			// 
			this.label12.BackColor = System.Drawing.Color.LightBlue;
			this.label12.Location = new System.Drawing.Point(403, 446);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(145, 45);
			this.label12.TabIndex = 155;
			this.label12.Text = "Here\'s multiline label so we can check alignment";
			this.label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butGetTag
			// 
			this.butGetTag.Image = global::UnitTests.Properties.Resources.ApptFind;
			this.butGetTag.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGetTag.Location = new System.Drawing.Point(392, 298);
			this.butGetTag.Name = "butGetTag";
			this.butGetTag.Size = new System.Drawing.Size(85, 44);
			this.butGetTag.TabIndex = 156;
			this.butGetTag.Text = "Get && Tag";
			this.butGetTag.UseVisualStyleBackColor = false;
			this.butGetTag.Click += new System.EventHandler(this.butGetTag_Click);
			// 
			// linkLabel1
			// 
			this.linkLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(27, 64);
			this.linkLabel1.Location = new System.Drawing.Point(160, 520);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(686, 29);
			this.linkLabel1.TabIndex = 157;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "The X-Charge website is at https://opendental.com/resources/redirects/redirectope" +
    "nedge.html and more more";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// butLaunchWin
			// 
			this.butLaunchWin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLaunchWin.Location = new System.Drawing.Point(569, 269);
			this.butLaunchWin.Name = "butLaunchWin";
			this.butLaunchWin.Size = new System.Drawing.Size(85, 24);
			this.butLaunchWin.TabIndex = 158;
			this.butLaunchWin.Text = "Launch Win";
			this.butLaunchWin.UseVisualStyleBackColor = false;
			this.butLaunchWin.Click += new System.EventHandler(this.butLaunchWin_Click);
			// 
			// butToggleVis
			// 
			this.butToggleVis.Enabled = false;
			this.butToggleVis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butToggleVis.Location = new System.Drawing.Point(12, 231);
			this.butToggleVis.Name = "butToggleVis";
			this.butToggleVis.Size = new System.Drawing.Size(85, 24);
			this.butToggleVis.TabIndex = 159;
			this.butToggleVis.Text = "Toggle Vis";
			this.butToggleVis.UseVisualStyleBackColor = false;
			this.butToggleVis.Click += new System.EventHandler(this.butToggleVis_Click);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(-44, 100);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(98, 18);
			this.label13.TabIndex = 161;
			this.label13.Text = "textVDouble";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDouble
			// 
			this.textDouble.Location = new System.Drawing.Point(60, 100);
			this.textDouble.MaxVal = 100000000D;
			this.textDouble.MinVal = 1D;
			this.textDouble.Name = "textDouble";
			this.textDouble.Size = new System.Drawing.Size(90, 20);
			this.textDouble.TabIndex = 1;
			this.textDouble.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(-44, 122);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(98, 18);
			this.label14.TabIndex = 163;
			this.label14.Text = "textVInt";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVInt
			// 
			this.textVInt.Location = new System.Drawing.Point(60, 122);
			this.textVInt.MaxVal = 999;
			this.textVInt.Name = "textVInt";
			this.textVInt.ShowZero = false;
			this.textVInt.Size = new System.Drawing.Size(90, 20);
			this.textVInt.TabIndex = 162;
			this.textVInt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(686, 216);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(100, 50);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 164;
			this.pictureBox1.TabStop = false;
			// 
			// FormUIManagerTests
			// 
			this.AcceptButton = this.button1;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.CancelButton = this.button3;
			this.ClientSize = new System.Drawing.Size(864, 588);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.textVInt);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textDouble);
			this.Controls.Add(this.butToggleVis);
			this.Controls.Add(this.butLaunchWin);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.butGetTag);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.testForWpf2);
			this.Controls.Add(this.testForWpf1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textVDateFrom);
			this.Controls.Add(this.toolBarMain);
			this.Controls.Add(this.menuMain);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.checkBox6);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.button2);
			this.Name = "FormUIManagerTests";
			this.Text = "UI Manager Tests";
			this.Load += new System.EventHandler(this.FormUIManagerTests_Load);
			this.groupBox3.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.contextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private OpenDental.UI.GroupBox groupBox3;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.CheckBox checkBox6;
		private OpenDental.UI.Button button3;
		private OpenDental.UI.ListBox listBox1;
		private OpenDental.UI.ComboBox comboBox1;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.TabControl tabControl;
		private OpenDental.UI.TabPage tabPage1;
		private OpenDental.UI.TabPage tabPage2;
		private OpenDental.UI.CheckBox checkBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Panel panel4;
		private OpenDental.UI.CheckBox checkBox2;
		private System.Windows.Forms.TextBox textBox3;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button button1;
		private OpenDental.UI.Button button4;
		private OpenDental.UI.MenuOD menuMain;
		private OpenDental.UI.ToolBarOD toolBarMain;
		private System.Windows.Forms.ImageList imageList;
		private OpenDental.ValidDate textVDateFrom;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.TestForWpf testForWpf1;
		private OpenDental.UI.TestForWpf testForWpf2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private OpenDental.UI.Button butGetTag;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemGoTo;
		private System.Windows.Forms.ToolStripMenuItem openPaymentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem voidPaymentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem processReturnToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemChild;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private OpenDental.UI.Button butLaunchWin;
		private System.Windows.Forms.TextBox textBox4;
		private OpenDental.UI.Button butToggleVis;
		private System.Windows.Forms.RadioButton radioStudentN;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.Label label13;
		private OpenDental.ValidDouble textDouble;
		private System.Windows.Forms.Label label14;
		private OpenDental.ValidNum textVInt;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}