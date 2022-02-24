namespace OpenDental {
	partial class ODjobTextEditor {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ODjobTextEditor));
			this.comboFontSize = new System.Windows.Forms.ComboBox();
			this.butHighlightSelect = new System.Windows.Forms.Button();
			this.comboFontType = new System.Windows.Forms.ComboBox();
			this.butColorSelect = new System.Windows.Forms.Button();
			this.butHighlight = new System.Windows.Forms.Button();
			this.butBullet = new System.Windows.Forms.Button();
			this.butStrikeout = new System.Windows.Forms.Button();
			this.butRedo = new System.Windows.Forms.Button();
			this.butColor = new System.Windows.Forms.Button();
			this.butUnderline = new System.Windows.Forms.Button();
			this.butItalics = new System.Windows.Forms.Button();
			this.butPaste = new System.Windows.Forms.Button();
			this.butBold = new System.Windows.Forms.Button();
			this.butUndo = new System.Windows.Forms.Button();
			this.butCopy = new System.Windows.Forms.Button();
			this.butCut = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.butSave = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.flowLayoutPanelMenu = new System.Windows.Forms.FlowLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.butSpellCheck = new System.Windows.Forms.Button();
			this.panel6 = new System.Windows.Forms.Panel();
			this.butClearFormatting = new System.Windows.Forms.Button();
			this.labelConcept = new System.Windows.Forms.Label();
			this.labelWriteup = new System.Windows.Forms.Label();
			this.panelConcept = new System.Windows.Forms.Panel();
			this.textConcept = new OpenDental.ODtextBox();
			this.panelWriteup = new System.Windows.Forms.Panel();
			this.textWriteup = new OpenDental.ODtextBox();
			this.panelRequirements = new System.Windows.Forms.Panel();
			this.gridRequirements = new OpenDental.UI.GridOD();
			this.panelSplitter2 = new System.Windows.Forms.Panel();
			this.panelSplitter1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			this.flowLayoutPanelMenu.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panelConcept.SuspendLayout();
			this.panelWriteup.SuspendLayout();
			this.panelRequirements.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboFontSize
			// 
			this.comboFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFontSize.FormattingEnabled = true;
			this.comboFontSize.Location = new System.Drawing.Point(137, 1);
			this.comboFontSize.Name = "comboFontSize";
			this.comboFontSize.Size = new System.Drawing.Size(50, 21);
			this.comboFontSize.TabIndex = 186;
			this.comboFontSize.SelectionChangeCommitted += new System.EventHandler(this.comboFontSize_SelectionChangeCommitted);
			// 
			// butHighlightSelect
			// 
			this.butHighlightSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butHighlightSelect.Image = ((System.Drawing.Image)(resources.GetObject("butHighlightSelect.Image")));
			this.butHighlightSelect.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.butHighlightSelect.Location = new System.Drawing.Point(72, 0);
			this.butHighlightSelect.Name = "butHighlightSelect";
			this.butHighlightSelect.Size = new System.Drawing.Size(10, 24);
			this.butHighlightSelect.TabIndex = 184;
			this.butHighlightSelect.Click += new System.EventHandler(this.butHighlightSelect_Click);
			this.butHighlightSelect.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butHighlightSelect.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// comboFontType
			// 
			this.comboFontType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFontType.FormattingEnabled = true;
			this.comboFontType.Location = new System.Drawing.Point(0, 1);
			this.comboFontType.Name = "comboFontType";
			this.comboFontType.Size = new System.Drawing.Size(133, 21);
			this.comboFontType.TabIndex = 183;
			this.comboFontType.SelectionChangeCommitted += new System.EventHandler(this.comboFontType_SelectionChangeCommitted);
			// 
			// butColorSelect
			// 
			this.butColorSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorSelect.Image = ((System.Drawing.Image)(resources.GetObject("butColorSelect.Image")));
			this.butColorSelect.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.butColorSelect.Location = new System.Drawing.Point(26, 0);
			this.butColorSelect.Name = "butColorSelect";
			this.butColorSelect.Size = new System.Drawing.Size(10, 24);
			this.butColorSelect.TabIndex = 182;
			this.butColorSelect.Click += new System.EventHandler(this.butColorSelect_Click);
			this.butColorSelect.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butColorSelect.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butHighlight
			// 
			this.butHighlight.BackColor = System.Drawing.Color.Yellow;
			this.butHighlight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butHighlight.BackgroundImage")));
			this.butHighlight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butHighlight.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butHighlight.Location = new System.Drawing.Point(48, 0);
			this.butHighlight.Name = "butHighlight";
			this.butHighlight.Size = new System.Drawing.Size(24, 24);
			this.butHighlight.TabIndex = 181;
			this.butHighlight.UseVisualStyleBackColor = false;
			this.butHighlight.Click += new System.EventHandler(this.butHighlight_Click);
			this.butHighlight.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butHighlight.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butBullet
			// 
			this.butBullet.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butBullet.BackgroundImage")));
			this.butBullet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butBullet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butBullet.Location = new System.Drawing.Point(99, 0);
			this.butBullet.Name = "butBullet";
			this.butBullet.Size = new System.Drawing.Size(24, 24);
			this.butBullet.TabIndex = 179;
			this.butBullet.Click += new System.EventHandler(this.butBullet_Click);
			this.butBullet.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butBullet.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butStrikeout
			// 
			this.butStrikeout.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butStrikeout.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butStrikeout.Location = new System.Drawing.Point(69, 0);
			this.butStrikeout.Name = "butStrikeout";
			this.butStrikeout.Size = new System.Drawing.Size(30, 24);
			this.butStrikeout.TabIndex = 185;
			this.butStrikeout.Text = "abc";
			this.butStrikeout.Click += new System.EventHandler(this.butStrikeout_Click);
			this.butStrikeout.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butStrikeout.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butRedo
			// 
			this.butRedo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butRedo.BackgroundImage")));
			this.butRedo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butRedo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butRedo.Location = new System.Drawing.Point(96, 0);
			this.butRedo.Name = "butRedo";
			this.butRedo.Size = new System.Drawing.Size(24, 24);
			this.butRedo.TabIndex = 177;
			this.butRedo.Click += new System.EventHandler(this.butRedo_Click);
			this.butRedo.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butRedo.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butColor.Location = new System.Drawing.Point(0, 0);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(26, 24);
			this.butColor.TabIndex = 170;
			this.butColor.Text = "A";
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			this.butColor.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butColor.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butUnderline
			// 
			this.butUnderline.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butUnderline.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butUnderline.Location = new System.Drawing.Point(46, 0);
			this.butUnderline.Name = "butUnderline";
			this.butUnderline.Size = new System.Drawing.Size(24, 24);
			this.butUnderline.TabIndex = 180;
			this.butUnderline.Text = "U";
			this.butUnderline.Click += new System.EventHandler(this.butUnderline_Click);
			this.butUnderline.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butUnderline.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butItalics
			// 
			this.butItalics.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butItalics.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butItalics.Location = new System.Drawing.Point(23, 0);
			this.butItalics.Name = "butItalics";
			this.butItalics.Size = new System.Drawing.Size(24, 24);
			this.butItalics.TabIndex = 176;
			this.butItalics.Text = "I";
			this.butItalics.Click += new System.EventHandler(this.butItalics_Click);
			this.butItalics.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butItalics.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butPaste
			// 
			this.butPaste.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butPaste.BackgroundImage")));
			this.butPaste.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butPaste.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butPaste.Location = new System.Drawing.Point(48, 0);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(24, 24);
			this.butPaste.TabIndex = 175;
			this.butPaste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			this.butPaste.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butPaste.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butBold
			// 
			this.butBold.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butBold.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butBold.Location = new System.Drawing.Point(0, 0);
			this.butBold.Name = "butBold";
			this.butBold.Size = new System.Drawing.Size(24, 24);
			this.butBold.TabIndex = 173;
			this.butBold.Text = "B";
			this.butBold.Click += new System.EventHandler(this.butBold_Click);
			this.butBold.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butBold.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butUndo
			// 
			this.butUndo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butUndo.BackgroundImage")));
			this.butUndo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butUndo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butUndo.Location = new System.Drawing.Point(72, 0);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(24, 24);
			this.butUndo.TabIndex = 172;
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			this.butUndo.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butUndo.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butCopy
			// 
			this.butCopy.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butCopy.BackgroundImage")));
			this.butCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butCopy.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(24, 0);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(24, 24);
			this.butCopy.TabIndex = 174;
			this.butCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			this.butCopy.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butCopy.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butCut
			// 
			this.butCut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butCut.BackgroundImage")));
			this.butCut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butCut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butCut.Location = new System.Drawing.Point(0, 0);
			this.butCut.Name = "butCut";
			this.butCut.Size = new System.Drawing.Size(24, 24);
			this.butCut.TabIndex = 171;
			this.butCut.Click += new System.EventHandler(this.butCut_Click);
			this.butCut.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butCut.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// butSave
			// 
			this.butSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butSave.BackgroundImage")));
			this.butSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.butSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butSave.Location = new System.Drawing.Point(120, 0);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(24, 24);
			this.butSave.TabIndex = 188;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			this.butSave.MouseEnter += new System.EventHandler(this.HoverColorEnter);
			this.butSave.MouseLeave += new System.EventHandler(this.HoverColorLeave);
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.butHighlight);
			this.panel1.Controls.Add(this.butHighlightSelect);
			this.panel1.Controls.Add(this.butColor);
			this.panel1.Controls.Add(this.butColorSelect);
			this.panel1.Location = new System.Drawing.Point(463, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(85, 27);
			this.panel1.TabIndex = 189;
			// 
			// panel3
			// 
			this.panel3.AutoSize = true;
			this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add(this.comboFontType);
			this.panel3.Controls.Add(this.comboFontSize);
			this.panel3.Location = new System.Drawing.Point(273, 0);
			this.panel3.Margin = new System.Windows.Forms.Padding(0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(190, 25);
			this.panel3.TabIndex = 191;
			// 
			// panel4
			// 
			this.panel4.AutoSize = true;
			this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel4.Controls.Add(this.butUndo);
			this.panel4.Controls.Add(this.butSave);
			this.panel4.Controls.Add(this.butCopy);
			this.panel4.Controls.Add(this.butCut);
			this.panel4.Controls.Add(this.butPaste);
			this.panel4.Controls.Add(this.butRedo);
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Margin = new System.Windows.Forms.Padding(0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(147, 27);
			this.panel4.TabIndex = 192;
			// 
			// panel5
			// 
			this.panel5.AutoSize = true;
			this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel5.Controls.Add(this.butBold);
			this.panel5.Controls.Add(this.butItalics);
			this.panel5.Controls.Add(this.butUnderline);
			this.panel5.Controls.Add(this.butStrikeout);
			this.panel5.Controls.Add(this.butBullet);
			this.panel5.Location = new System.Drawing.Point(147, 0);
			this.panel5.Margin = new System.Windows.Forms.Padding(0);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(126, 27);
			this.panel5.TabIndex = 193;
			// 
			// flowLayoutPanelMenu
			// 
			this.flowLayoutPanelMenu.AutoSize = true;
			this.flowLayoutPanelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanelMenu.Controls.Add(this.panel4);
			this.flowLayoutPanelMenu.Controls.Add(this.panel5);
			this.flowLayoutPanelMenu.Controls.Add(this.panel3);
			this.flowLayoutPanelMenu.Controls.Add(this.panel1);
			this.flowLayoutPanelMenu.Controls.Add(this.panel2);
			this.flowLayoutPanelMenu.Controls.Add(this.panel6);
			this.flowLayoutPanelMenu.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanelMenu.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanelMenu.Name = "flowLayoutPanelMenu";
			this.flowLayoutPanelMenu.Size = new System.Drawing.Size(855, 27);
			this.flowLayoutPanelMenu.TabIndex = 195;
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add(this.butSpellCheck);
			this.panel2.Location = new System.Drawing.Point(548, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(77, 27);
			this.panel2.TabIndex = 194;
			// 
			// butSpellCheck
			// 
			this.butSpellCheck.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butSpellCheck.Location = new System.Drawing.Point(0, 0);
			this.butSpellCheck.Name = "butSpellCheck";
			this.butSpellCheck.Size = new System.Drawing.Size(74, 24);
			this.butSpellCheck.TabIndex = 170;
			this.butSpellCheck.Text = "Spell Check";
			this.butSpellCheck.Click += new System.EventHandler(this.butSpellCheck_Click);
			// 
			// panel6
			// 
			this.panel6.AutoSize = true;
			this.panel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel6.Controls.Add(this.butClearFormatting);
			this.panel6.Location = new System.Drawing.Point(625, 0);
			this.panel6.Margin = new System.Windows.Forms.Padding(0);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(96, 27);
			this.panel6.TabIndex = 195;
			// 
			// butClearFormatting
			// 
			this.butClearFormatting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butClearFormatting.Location = new System.Drawing.Point(0, 0);
			this.butClearFormatting.Name = "butClearFormatting";
			this.butClearFormatting.Size = new System.Drawing.Size(93, 24);
			this.butClearFormatting.TabIndex = 170;
			this.butClearFormatting.Text = "Clear Formatting";
			this.butClearFormatting.Click += new System.EventHandler(this.butClearFormatting_Click);
			// 
			// labelConcept
			// 
			this.labelConcept.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelConcept.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelConcept.Location = new System.Drawing.Point(0, 0);
			this.labelConcept.Name = "labelConcept";
			this.labelConcept.Size = new System.Drawing.Size(855, 13);
			this.labelConcept.TabIndex = 197;
			this.labelConcept.Text = "Concept";
			// 
			// labelWriteup
			// 
			this.labelWriteup.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelWriteup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWriteup.Location = new System.Drawing.Point(0, 0);
			this.labelWriteup.Name = "labelWriteup";
			this.labelWriteup.Size = new System.Drawing.Size(855, 13);
			this.labelWriteup.TabIndex = 197;
			this.labelWriteup.Text = "Writeup";
			// 
			// panelConcept
			// 
			this.panelConcept.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelConcept.Controls.Add(this.textConcept);
			this.panelConcept.Controls.Add(this.labelConcept);
			this.panelConcept.Location = new System.Drawing.Point(0, 27);
			this.panelConcept.Name = "panelConcept";
			this.panelConcept.Size = new System.Drawing.Size(855, 207);
			this.panelConcept.TabIndex = 198;
			// 
			// textConcept
			// 
			this.textConcept.AcceptsTab = true;
			this.textConcept.BackColor = System.Drawing.SystemColors.Window;
			this.textConcept.DetectLinksEnabled = false;
			this.textConcept.DetectUrls = false;
			this.textConcept.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textConcept.EditMode = true;
			this.textConcept.HasAutoNotes = true;
			this.textConcept.HideSelection = false;
			this.textConcept.Location = new System.Drawing.Point(0, 13);
			this.textConcept.Name = "textConcept";
			this.textConcept.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textConcept.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.textConcept.Size = new System.Drawing.Size(855, 194);
			this.textConcept.TabIndex = 169;
			this.textConcept.Text = "";
			this.textConcept.SelectionChanged += new System.EventHandler(this.textRequirements_SelectionChanged);
			this.textConcept.TextChanged += new System.EventHandler(this.textDescription_TextChanged);
			this.textConcept.Enter += new System.EventHandler(this.textDescription_Enter);
			this.textConcept.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textRequirements_KeyUp);
			// 
			// panelWriteup
			// 
			this.panelWriteup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelWriteup.Controls.Add(this.textWriteup);
			this.panelWriteup.Controls.Add(this.labelWriteup);
			this.panelWriteup.Location = new System.Drawing.Point(0, 368);
			this.panelWriteup.Name = "panelWriteup";
			this.panelWriteup.Size = new System.Drawing.Size(855, 158);
			this.panelWriteup.TabIndex = 200;
			// 
			// textWriteup
			// 
			this.textWriteup.AcceptsTab = true;
			this.textWriteup.BackColor = System.Drawing.SystemColors.Window;
			this.textWriteup.DetectLinksEnabled = false;
			this.textWriteup.DetectUrls = false;
			this.textWriteup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textWriteup.EditMode = true;
			this.textWriteup.HasAutoNotes = true;
			this.textWriteup.HideSelection = false;
			this.textWriteup.Location = new System.Drawing.Point(0, 13);
			this.textWriteup.Name = "textWriteup";
			this.textWriteup.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textWriteup.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textWriteup.Size = new System.Drawing.Size(855, 145);
			this.textWriteup.SpellCheckIsEnabled = false;
			this.textWriteup.TabIndex = 169;
			this.textWriteup.Text = "";
			this.textWriteup.SelectionChanged += new System.EventHandler(this.textImplementation_SelectionChanged);
			this.textWriteup.TextChanged += new System.EventHandler(this.textDescription_TextChanged);
			this.textWriteup.Enter += new System.EventHandler(this.textImplementation_Enter);
			this.textWriteup.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textImplementation_KeyUp);
			// 
			// panelRequirements
			// 
			this.panelRequirements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRequirements.Controls.Add(this.gridRequirements);
			this.panelRequirements.Location = new System.Drawing.Point(0, 239);
			this.panelRequirements.Name = "panelRequirements";
			this.panelRequirements.Size = new System.Drawing.Size(855, 124);
			this.panelRequirements.TabIndex = 201;
			// 
			// gridRequirements
			// 
			this.gridRequirements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridRequirements.HasAddButton = true;
			this.gridRequirements.Location = new System.Drawing.Point(0, 0);
			this.gridRequirements.Name = "gridRequirements";
			this.gridRequirements.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridRequirements.ShowContextMenu = false;
			this.gridRequirements.Size = new System.Drawing.Size(855, 124);
			this.gridRequirements.TabIndex = 0;
			this.gridRequirements.Title = "Requirements";
			this.gridRequirements.TitleVisible = false;
			this.gridRequirements.TranslationName = "JobRequirements";
			this.gridRequirements.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRequirements_CellClick);
			this.gridRequirements.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridRequirements_CellLeave);
			this.gridRequirements.CellKeyDown += new OpenDental.UI.ODGridKeyEventHandler(this.gridRequirements_CellKeyDown);
			this.gridRequirements.DoubleClick += new System.EventHandler(this.gridRequirements_DoubleClick);
			this.gridRequirements.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridRequirements_MouseClick);
			// 
			// panelSplitter2
			// 
			this.panelSplitter2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelSplitter2.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.panelSplitter2.Location = new System.Drawing.Point(0, 363);
			this.panelSplitter2.Name = "panelSplitter2";
			this.panelSplitter2.Size = new System.Drawing.Size(855, 5);
			this.panelSplitter2.TabIndex = 1;
			this.panelSplitter2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSplitter2_MouseDown);
			this.panelSplitter2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSplitter2_MouseMove);
			this.panelSplitter2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSplitter2_MouseUp);
			// 
			// panelSplitter1
			// 
			this.panelSplitter1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelSplitter1.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.panelSplitter1.Location = new System.Drawing.Point(0, 234);
			this.panelSplitter1.Name = "panelSplitter1";
			this.panelSplitter1.Size = new System.Drawing.Size(855, 5);
			this.panelSplitter1.TabIndex = 1;
			this.panelSplitter1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSplitter1_MouseDown);
			this.panelSplitter1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSplitter1_MouseMove);
			this.panelSplitter1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSplitter1_MouseUp);
			// 
			// ODjobTextEditor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.panelWriteup);
			this.Controls.Add(this.panelSplitter2);
			this.Controls.Add(this.panelRequirements);
			this.Controls.Add(this.panelSplitter1);
			this.Controls.Add(this.panelConcept);
			this.Controls.Add(this.flowLayoutPanelMenu);
			this.Name = "ODjobTextEditor";
			this.Size = new System.Drawing.Size(855, 523);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ODjobTextEditor_Layout);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.flowLayoutPanelMenu.ResumeLayout(false);
			this.flowLayoutPanelMenu.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panelConcept.ResumeLayout(false);
			this.panelWriteup.ResumeLayout(false);
			this.panelRequirements.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboFontSize;
		private System.Windows.Forms.Button butHighlightSelect;
		private System.Windows.Forms.ComboBox comboFontType;
		private System.Windows.Forms.Button butColorSelect;
		private System.Windows.Forms.Button butHighlight;
		private System.Windows.Forms.Button butBullet;
		private System.Windows.Forms.Button butStrikeout;
		private System.Windows.Forms.Button butRedo;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Button butUnderline;
		private System.Windows.Forms.Button butItalics;
		private System.Windows.Forms.Button butPaste;
		private System.Windows.Forms.Button butBold;
		private System.Windows.Forms.Button butUndo;
		private System.Windows.Forms.Button butCopy;
		private System.Windows.Forms.Button butCut;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button butSave;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMenu;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button butSpellCheck;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Button butClearFormatting;
		private ODtextBox textConcept;
		private System.Windows.Forms.Label labelConcept;
		private ODtextBox textWriteup;
		private System.Windows.Forms.Label labelWriteup;
		private UI.GridOD gridRequirements;
		private System.Windows.Forms.Panel panelConcept;
		private System.Windows.Forms.Panel panelWriteup;
		private System.Windows.Forms.Panel panelRequirements;
		private System.Windows.Forms.Panel panelSplitter2;
		private System.Windows.Forms.Panel panelSplitter1;
	}
}
