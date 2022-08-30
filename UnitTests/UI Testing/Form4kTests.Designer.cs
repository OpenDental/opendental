namespace UnitTests
{
	partial class Form4kTests
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
			if (disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4kTests));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.button3 = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.button1 = new OpenDental.UI.Button();
			this.button4 = new OpenDental.UI.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.button5 = new OpenDental.UI.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.butFont = new OpenDental.UI.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.gridOD1 = new OpenDental.UI.GridOD();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.gridOD2 = new OpenDental.UI.GridOD();
			this.butMove = new OpenDental.UI.Button();
			this.butTabRemove = new OpenDental.UI.Button();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.butTabAdd = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.butMaximized = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "Add.gif");
			this.imageListMain.Images.SetKeyName(1, "editPencil.gif");
			this.imageListMain.Images.SetKeyName(2, "butExport.gif");
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(49, 24);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(870, 25);
			this.ToolBarMain.TabIndex = 61;
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(840, 626);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 24);
			this.button3.TabIndex = 60;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Image = global::UnitTests.Properties.Resources.deleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(4, 626);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.LocationChanged += new System.EventHandler(this.butDelete_LocationChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(4, 53);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "3D Chart";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button4.Location = new System.Drawing.Point(840, 53);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 24);
			this.button4.TabIndex = 61;
			this.button4.Text = "Change";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.IntegralHeight = false;
			this.listBox1.Items.AddRange(new object[] {
            "Item1",
            "Item2",
            "Item3",
            "Item4",
            "Item5",
            "Item6"});
			this.listBox1.Location = new System.Drawing.Point(399, 541);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(120, 78);
			this.listBox1.TabIndex = 4;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.radioButton1);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(31, 426);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 100);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// checkBox1
			// 
			this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox1.Location = new System.Drawing.Point(23, 68);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(104, 20);
			this.checkBox1.TabIndex = 2;
			this.checkBox1.Text = "checkBox1";
			this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(23, 42);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(104, 20);
			this.radioButton1.TabIndex = 1;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "radioButton1";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(20, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridMain.Location = new System.Drawing.Point(0, 0);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(435, 69);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Test Grid";
			this.gridMain.TranslationName = "test";
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(919, 24);
			this.menuMain.TabIndex = 62;
			// 
			// button5
			// 
			this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button5.Location = new System.Drawing.Point(545, 219);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 24);
			this.button5.TabIndex = 64;
			this.button5.Text = "Add Button";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.LightSalmon;
			this.panel1.Controls.Add(this.label2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(49, 630);
			this.panel1.TabIndex = 65;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-1, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 15);
			this.label2.TabIndex = 0;
			this.label2.Text = "Left";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.MediumTurquoise;
			this.panel2.Controls.Add(this.label3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(49, 619);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(870, 35);
			this.panel2.TabIndex = 66;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 15);
			this.label3.TabIndex = 0;
			this.label3.Text = "Left";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.Plum;
			this.panel3.Controls.Add(this.label4);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel3.Location = new System.Drawing.Point(871, 49);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(48, 570);
			this.panel3.TabIndex = 67;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 15);
			this.label4.TabIndex = 0;
			this.label4.Text = "Right";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.butMaximized);
			this.panel4.Controls.Add(this.checkBox2);
			this.panel4.Controls.Add(this.radioButton2);
			this.panel4.Controls.Add(this.label6);
			this.panel4.Controls.Add(this.butFont);
			this.panel4.Controls.Add(this.richTextBox1);
			this.panel4.Controls.Add(this.tabControl1);
			this.panel4.Controls.Add(this.butMove);
			this.panel4.Controls.Add(this.groupBox1);
			this.panel4.Controls.Add(this.butTabRemove);
			this.panel4.Controls.Add(this.splitContainer);
			this.panel4.Controls.Add(this.butTabAdd);
			this.panel4.Controls.Add(this.label5);
			this.panel4.Controls.Add(this.button5);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(49, 49);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(822, 570);
			this.panel4.TabIndex = 68;
			this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
			// 
			// checkBox2
			// 
			this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox2.Location = new System.Drawing.Point(28, 386);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(456, 20);
			this.checkBox2.TabIndex = 81;
			this.checkBox2.Text = "checkBox2 This is some really long text that barely fits. super long hardly fits." +
    " A bit 5 4 3 2 1";
			this.checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButton2.Location = new System.Drawing.Point(30, 360);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(461, 20);
			this.radioButton2.TabIndex = 80;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "radioButton2 This is some really long text that barely fits. super long hardly fi" +
    "ts. A bit 5 4 3 2 1";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(27, 339);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(414, 18);
			this.label6.TabIndex = 79;
			this.label6.Text = "label6  This is some really long text that barely fits. super long hardly fits. A" +
    " bit 5 4 3 2 1";
			// 
			// butFont
			// 
			this.butFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butFont.Location = new System.Drawing.Point(558, 54);
			this.butFont.Name = "butFont";
			this.butFont.Size = new System.Drawing.Size(75, 24);
			this.butFont.TabIndex = 75;
			this.butFont.Text = "Font";
			this.butFont.UseVisualStyleBackColor = true;
			this.butFont.Click += new System.EventHandler(this.butFont_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox1.Location = new System.Drawing.Point(558, 84);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(197, 44);
			this.richTextBox1.TabIndex = 74;
			this.richTextBox1.Text = "Some text";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(30, 207);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(437, 129);
			this.tabControl1.TabIndex = 65;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.gridOD1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(429, 103);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// gridOD1
			// 
			this.gridOD1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridOD1.Location = new System.Drawing.Point(0, 0);
			this.gridOD1.Name = "gridOD1";
			this.gridOD1.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridOD1.Size = new System.Drawing.Size(429, 103);
			this.gridOD1.TabIndex = 1;
			this.gridOD1.Title = "Test Grid 1";
			this.gridOD1.TranslationName = "test";
			this.gridOD1.LocationChanged += new System.EventHandler(this.gridOD1_LocationChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.gridOD2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(429, 103);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// gridOD2
			// 
			this.gridOD2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridOD2.Location = new System.Drawing.Point(3, 3);
			this.gridOD2.Name = "gridOD2";
			this.gridOD2.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridOD2.Size = new System.Drawing.Size(423, 97);
			this.gridOD2.TabIndex = 2;
			this.gridOD2.Title = "Test Grid 2";
			this.gridOD2.TranslationName = "test";
			// 
			// butMove
			// 
			this.butMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMove.Location = new System.Drawing.Point(545, 166);
			this.butMove.Name = "butMove";
			this.butMove.Size = new System.Drawing.Size(75, 24);
			this.butMove.TabIndex = 73;
			this.butMove.Text = "Move";
			this.butMove.UseVisualStyleBackColor = true;
			this.butMove.Click += new System.EventHandler(this.butMove_Click);
			// 
			// butTabRemove
			// 
			this.butTabRemove.Location = new System.Drawing.Point(633, 298);
			this.butTabRemove.Name = "butTabRemove";
			this.butTabRemove.Size = new System.Drawing.Size(80, 24);
			this.butTabRemove.TabIndex = 72;
			this.butTabRemove.Text = "Tab Remove";
			this.butTabRemove.UseVisualStyleBackColor = true;
			this.butTabRemove.Click += new System.EventHandler(this.butTabRemove_Click);
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.BackColor = System.Drawing.Color.PaleGoldenrod;
			this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer.Location = new System.Drawing.Point(30, 34);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer.Panel1.Controls.Add(this.gridMain);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.BackColor = System.Drawing.Color.PeachPuff;
			this.splitContainer.Size = new System.Drawing.Size(437, 144);
			this.splitContainer.SplitterDistance = 71;
			this.splitContainer.TabIndex = 70;
			this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
			// 
			// butTabAdd
			// 
			this.butTabAdd.Location = new System.Drawing.Point(548, 298);
			this.butTabAdd.Name = "butTabAdd";
			this.butTabAdd.Size = new System.Drawing.Size(75, 24);
			this.butTabAdd.TabIndex = 69;
			this.butTabAdd.Text = "Tab Add";
			this.butTabAdd.UseVisualStyleBackColor = true;
			this.butTabAdd.Click += new System.EventHandler(this.butTab_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 5);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(47, 15);
			this.label5.TabIndex = 0;
			this.label5.Text = "Fill";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butMaximized
			// 
			this.butMaximized.Location = new System.Drawing.Point(545, 15);
			this.butMaximized.Name = "butMaximized";
			this.butMaximized.Size = new System.Drawing.Size(75, 24);
			this.butMaximized.TabIndex = 69;
			this.butMaximized.Text = "Maximized";
			this.butMaximized.UseVisualStyleBackColor = true;
			this.butMaximized.Click += new System.EventHandler(this.butMaximized_Click);
			// 
			// Form4kTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(919, 654);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuMain);
			this.Name = "Form4kTests";
			this.Text = "4K Tests";
			this.Load += new System.EventHandler(this.FormGridTest_Load);
			this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.Form4kTest_DpiChanged);
			this.ResizeEnd += new System.EventHandler(this.Form4kTest_ResizeEnd);
			this.SizeChanged += new System.EventHandler(this.Form4kTest_SizeChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form4kTest_Paint);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.Form4kTest_Layout);
			this.DpiChangedBeforeParent += new System.EventHandler(this.Form4kTest_DpiChangedBeforeParent);
			this.DpiChangedAfterParent += new System.EventHandler(this.Form4kTest_DpiChangedAfterParent);
			this.Resize += new System.EventHandler(this.Form4kTest_Resize);
			this.groupBox1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button button1;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button button3;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private OpenDental.UI.Button button4;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.MenuOD menuMain;
		private OpenDental.UI.Button button5;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.Button butTabAdd;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.SplitContainer splitContainer;
		private OpenDental.UI.GridOD gridOD1;
		private OpenDental.UI.Button butTabRemove;
		private OpenDental.UI.Button butMove;
		private OpenDental.UI.GridOD gridOD2;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private OpenDental.UI.Button butFont;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.Button butMaximized;
	}
}