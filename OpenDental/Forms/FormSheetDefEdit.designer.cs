namespace OpenDental{
	partial class FormSheetDefEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				// components can be null at this point, so dispose everything else regardless
				components?.Dispose();
				_graphicsBackground?.Dispose();
				_graphicsBackground=null;
				_bitmapBackground?.Dispose();
				_bitmapBackground=null;
				_imageToothChart?.Dispose();
				_imageToothChart=null;
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetDefEdit));
			this.panelMain = new OpenDental.PanelGraphics();
			this.butAlignTop = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBoxSubViews = new OpenDental.UI.GroupBoxOD();
			this.checkSynchMatchedFields = new System.Windows.Forms.CheckBox();
			this.comboLanguages = new OpenDental.UI.ComboBoxOD();
			this.butAutoSnap = new OpenDental.UI.Button();
			this.butMobile = new OpenDental.UI.Button();
			this.groupAlignH = new OpenDental.UI.GroupBoxOD();
			this.butAlignRight = new OpenDental.UI.Button();
			this.butAlignCenterH = new OpenDental.UI.Button();
			this.butAlignLeft = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.groupPage = new OpenDental.UI.GroupBoxOD();
			this.butPageAdd = new OpenDental.UI.Button();
			this.butPageRemove = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butTabOrder = new OpenDental.UI.Button();
			this.labelInternal = new System.Windows.Forms.Label();
			this.linkLabelTips = new System.Windows.Forms.LinkLabel();
			this.listBoxFields = new OpenDental.UI.ListBoxOD();
			this.butPaste = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butCopy = new OpenDental.UI.Button();
			this.butEdit = new OpenDental.UI.Button();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.panelRight = new System.Windows.Forms.Panel();
			this.groupShowField = new OpenDental.UI.GroupBoxOD();
			this.checkShowScreenChart = new System.Windows.Forms.CheckBox();
			this.checkShowGrid = new System.Windows.Forms.CheckBox();
			this.checkShowSpecial = new System.Windows.Forms.CheckBox();
			this.checkShowSigBoxPractice = new System.Windows.Forms.CheckBox();
			this.checkShowSigBox = new System.Windows.Forms.CheckBox();
			this.checkShowRectangle = new System.Windows.Forms.CheckBox();
			this.checkShowLine = new System.Windows.Forms.CheckBox();
			this.checkShowPatImage = new System.Windows.Forms.CheckBox();
			this.checkShowImage = new System.Windows.Forms.CheckBox();
			this.checkShowComboBox = new System.Windows.Forms.CheckBox();
			this.butShowNone = new OpenDental.UI.Button();
			this.butShowAll = new OpenDental.UI.Button();
			this.checkShowCheckBox = new System.Windows.Forms.CheckBox();
			this.checkShowStaticText = new System.Windows.Forms.CheckBox();
			this.checkShowInputField = new System.Windows.Forms.CheckBox();
			this.checkShowOutputText = new System.Windows.Forms.CheckBox();
			this.checkBlue = new System.Windows.Forms.CheckBox();
			this.groupAddField = new OpenDental.UI.GroupBoxOD();
			this.butScreenChart = new OpenDental.UI.Button();
			this.butSpecial = new OpenDental.UI.Button();
			this.butGrid = new OpenDental.UI.Button();
			this.butSigBox = new OpenDental.UI.Button();
			this.butSigBoxPractice = new OpenDental.UI.Button();
			this.butLine = new OpenDental.UI.Button();
			this.butRectangle = new OpenDental.UI.Button();
			this.butPatImage = new OpenDental.UI.Button();
			this.butImage = new OpenDental.UI.Button();
			this.butComboBox = new OpenDental.UI.Button();
			this.butCheckBox = new OpenDental.UI.Button();
			this.butStaticText = new OpenDental.UI.Button();
			this.butInputField = new OpenDental.UI.Button();
			this.butOutputText = new OpenDental.UI.Button();
			this.butRedo = new OpenDental.UI.Button();
			this.butUndo = new OpenDental.UI.Button();
			this.groupBoxSubViews.SuspendLayout();
			this.groupAlignH.SuspendLayout();
			this.groupPage.SuspendLayout();
			this.panelLeft.SuspendLayout();
			this.panelRight.SuspendLayout();
			this.groupShowField.SuspendLayout();
			this.groupAddField.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelMain.BackColor = System.Drawing.Color.Transparent;
			this.panelMain.Location = new System.Drawing.Point(12, 13);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(520, 671);
			this.panelMain.TabIndex = 1;
			this.panelMain.TabStop = true;
			this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
			this.panelMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDoubleClick);
			this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
			this.panelMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseMove);
			this.panelMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseUp);
			this.panelMain.Resize += new System.EventHandler(this.panelMain_Resize);
			// 
			// butAlignTop
			// 
			this.butAlignTop.Location = new System.Drawing.Point(149, 18);
			this.butAlignTop.Name = "butAlignTop";
			this.butAlignTop.Size = new System.Drawing.Size(46, 24);
			this.butAlignTop.TabIndex = 88;
			this.butAlignTop.TabStop = false;
			this.butAlignTop.Text = "Top";
			this.butAlignTop.Click += new System.EventHandler(this.butAlignTop_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(210, 669);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(72, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.TabStop = false;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(136, 669);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(72, 24);
			this.butOK.TabIndex = 3;
			this.butOK.TabStop = false;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupBoxSubViews
			// 
			this.groupBoxSubViews.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxSubViews.Controls.Add(this.checkSynchMatchedFields);
			this.groupBoxSubViews.Controls.Add(this.comboLanguages);
			this.groupBoxSubViews.Location = new System.Drawing.Point(4, 567);
			this.groupBoxSubViews.Name = "groupBoxSubViews";
			this.groupBoxSubViews.Size = new System.Drawing.Size(150, 60);
			this.groupBoxSubViews.TabIndex = 100;
			this.groupBoxSubViews.Text = "Language";
			// 
			// checkSynchMatchedFields
			// 
			this.checkSynchMatchedFields.Location = new System.Drawing.Point(6, 15);
			this.checkSynchMatchedFields.Name = "checkSynchMatchedFields";
			this.checkSynchMatchedFields.Size = new System.Drawing.Size(130, 18);
			this.checkSynchMatchedFields.TabIndex = 3;
			this.checkSynchMatchedFields.Text = "Synch matched fields";
			this.checkSynchMatchedFields.UseVisualStyleBackColor = true;
			this.checkSynchMatchedFields.CheckedChanged += new System.EventHandler(this.checkSynchMatchedFields_CheckedChanged);
			// 
			// comboLanguages
			// 
			this.comboLanguages.Location = new System.Drawing.Point(6, 35);
			this.comboLanguages.Name = "comboLanguages";
			this.comboLanguages.Size = new System.Drawing.Size(131, 21);
			this.comboLanguages.TabIndex = 2;
			this.comboLanguages.SelectionChangeCommitted += new System.EventHandler(this.comboLanguages_SelectionChangeCommitted);
			// 
			// butAutoSnap
			// 
			this.butAutoSnap.Location = new System.Drawing.Point(174, 77);
			this.butAutoSnap.Name = "butAutoSnap";
			this.butAutoSnap.Size = new System.Drawing.Size(61, 24);
			this.butAutoSnap.TabIndex = 99;
			this.butAutoSnap.TabStop = false;
			this.butAutoSnap.Text = "Grid Snap";
			this.butAutoSnap.Click += new System.EventHandler(this.butAutoSnap_Click);
			// 
			// butMobile
			// 
			this.butMobile.Location = new System.Drawing.Point(146, 27);
			this.butMobile.Name = "butMobile";
			this.butMobile.Size = new System.Drawing.Size(90, 24);
			this.butMobile.TabIndex = 98;
			this.butMobile.TabStop = false;
			this.butMobile.Text = "Show Mobile";
			this.butMobile.Click += new System.EventHandler(this.butMobile_Click);
			// 
			// groupAlignH
			// 
			this.groupAlignH.Controls.Add(this.butAlignRight);
			this.groupAlignH.Controls.Add(this.butAlignCenterH);
			this.groupAlignH.Controls.Add(this.butAlignLeft);
			this.groupAlignH.Controls.Add(this.butAlignTop);
			this.groupAlignH.Location = new System.Drawing.Point(4, 104);
			this.groupAlignH.Name = "groupAlignH";
			this.groupAlignH.Size = new System.Drawing.Size(200, 46);
			this.groupAlignH.TabIndex = 96;
			this.groupAlignH.Text = "Align";
			// 
			// butAlignRight
			// 
			this.butAlignRight.Location = new System.Drawing.Point(101, 18);
			this.butAlignRight.Name = "butAlignRight";
			this.butAlignRight.Size = new System.Drawing.Size(46, 24);
			this.butAlignRight.TabIndex = 91;
			this.butAlignRight.TabStop = false;
			this.butAlignRight.Text = "Right";
			this.butAlignRight.Click += new System.EventHandler(this.butAlignRight_Click);
			// 
			// butAlignCenterH
			// 
			this.butAlignCenterH.Location = new System.Drawing.Point(51, 18);
			this.butAlignCenterH.Name = "butAlignCenterH";
			this.butAlignCenterH.Size = new System.Drawing.Size(48, 24);
			this.butAlignCenterH.TabIndex = 90;
			this.butAlignCenterH.TabStop = false;
			this.butAlignCenterH.Text = "Center";
			this.butAlignCenterH.Click += new System.EventHandler(this.butAlignCenterH_Click);
			// 
			// butAlignLeft
			// 
			this.butAlignLeft.Location = new System.Drawing.Point(3, 18);
			this.butAlignLeft.Name = "butAlignLeft";
			this.butAlignLeft.Size = new System.Drawing.Size(46, 24);
			this.butAlignLeft.TabIndex = 89;
			this.butAlignLeft.TabStop = false;
			this.butAlignLeft.Text = "Left";
			this.butAlignLeft.Click += new System.EventHandler(this.butAlignLeft_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(4, 5);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(277, 20);
			this.textDescription.TabIndex = 0;
			// 
			// groupPage
			// 
			this.groupPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupPage.Controls.Add(this.butPageAdd);
			this.groupPage.Controls.Add(this.butPageRemove);
			this.groupPage.Location = new System.Drawing.Point(158, 584);
			this.groupPage.Name = "groupPage";
			this.groupPage.Size = new System.Drawing.Size(122, 43);
			this.groupPage.TabIndex = 95;
			this.groupPage.Text = "Pages";
			// 
			// butPageAdd
			// 
			this.butPageAdd.Location = new System.Drawing.Point(6, 15);
			this.butPageAdd.Name = "butPageAdd";
			this.butPageAdd.Size = new System.Drawing.Size(42, 24);
			this.butPageAdd.TabIndex = 95;
			this.butPageAdd.TabStop = false;
			this.butPageAdd.Text = "Add";
			this.butPageAdd.Click += new System.EventHandler(this.butPageAdd_Click);
			// 
			// butPageRemove
			// 
			this.butPageRemove.Location = new System.Drawing.Point(50, 15);
			this.butPageRemove.Name = "butPageRemove";
			this.butPageRemove.Size = new System.Drawing.Size(69, 24);
			this.butPageRemove.TabIndex = 96;
			this.butPageRemove.TabStop = false;
			this.butPageRemove.Text = "Remove";
			this.butPageRemove.Click += new System.EventHandler(this.butPageRemove_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(49, 669);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(70, 24);
			this.butDelete.TabIndex = 80;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butTabOrder
			// 
			this.butTabOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTabOrder.Location = new System.Drawing.Point(5, 636);
			this.butTabOrder.Name = "butTabOrder";
			this.butTabOrder.Size = new System.Drawing.Size(70, 24);
			this.butTabOrder.TabIndex = 94;
			this.butTabOrder.TabStop = false;
			this.butTabOrder.Text = "Tab Order";
			this.butTabOrder.Click += new System.EventHandler(this.butTabOrder_Click);
			// 
			// labelInternal
			// 
			this.labelInternal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInternal.Location = new System.Drawing.Point(9, 630);
			this.labelInternal.Name = "labelInternal";
			this.labelInternal.Size = new System.Drawing.Size(208, 34);
			this.labelInternal.TabIndex = 82;
			this.labelInternal.Text = "This is an internal sheet, so it may not be edited.  Make a copy instead.";
			this.labelInternal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// linkLabelTips
			// 
			this.linkLabelTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabelTips.Location = new System.Drawing.Point(238, 639);
			this.linkLabelTips.Name = "linkLabelTips";
			this.linkLabelTips.Size = new System.Drawing.Size(31, 17);
			this.linkLabelTips.TabIndex = 93;
			this.linkLabelTips.TabStop = true;
			this.linkLabelTips.Text = "tips";
			this.linkLabelTips.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.linkLabelTips.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTips_LinkClicked);
			// 
			// listBoxFields
			// 
			this.listBoxFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxFields.IntegralHeight = false;
			this.listBoxFields.Location = new System.Drawing.Point(4, 169);
			this.listBoxFields.Name = "listBoxFields";
			this.listBoxFields.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxFields.Size = new System.Drawing.Size(151, 392);
			this.listBoxFields.TabIndex = 83;
			this.listBoxFields.SelectionChangeCommitted += new System.EventHandler(this.listFields_SelectionChangeCommitted);
			this.listBoxFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFields_MouseDoubleClick);
			// 
			// butPaste
			// 
			this.butPaste.Location = new System.Drawing.Point(82, 77);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(74, 24);
			this.butPaste.TabIndex = 91;
			this.butPaste.TabStop = false;
			this.butPaste.Text = "Paste Ctrl-V";
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(5, 152);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(193, 16);
			this.label2.TabIndex = 84;
			this.label2.Text = "Fields";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCopy
			// 
			this.butCopy.Location = new System.Drawing.Point(4, 77);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(74, 24);
			this.butCopy.TabIndex = 90;
			this.butCopy.TabStop = false;
			this.butCopy.Text = "Copy Ctrl-C";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butEdit
			// 
			this.butEdit.Location = new System.Drawing.Point(4, 27);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(90, 24);
			this.butEdit.TabIndex = 87;
			this.butEdit.TabStop = false;
			this.butEdit.Text = "Edit Properties";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// panelLeft
			// 
			this.panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelLeft.AutoScroll = true;
			this.panelLeft.Controls.Add(this.panelMain);
			this.panelLeft.Location = new System.Drawing.Point(0, 0);
			this.panelLeft.Name = "panelLeft";
			this.panelLeft.Size = new System.Drawing.Size(544, 696);
			this.panelLeft.TabIndex = 102;
			// 
			// panelRight
			// 
			this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRight.Controls.Add(this.groupShowField);
			this.panelRight.Controls.Add(this.checkBlue);
			this.panelRight.Controls.Add(this.groupAddField);
			this.panelRight.Controls.Add(this.butRedo);
			this.panelRight.Controls.Add(this.groupPage);
			this.panelRight.Controls.Add(this.butUndo);
			this.panelRight.Controls.Add(this.butCancel);
			this.panelRight.Controls.Add(this.butOK);
			this.panelRight.Controls.Add(this.groupBoxSubViews);
			this.panelRight.Controls.Add(this.butAutoSnap);
			this.panelRight.Controls.Add(this.butMobile);
			this.panelRight.Controls.Add(this.groupAlignH);
			this.panelRight.Controls.Add(this.textDescription);
			this.panelRight.Controls.Add(this.butDelete);
			this.panelRight.Controls.Add(this.butTabOrder);
			this.panelRight.Controls.Add(this.labelInternal);
			this.panelRight.Controls.Add(this.linkLabelTips);
			this.panelRight.Controls.Add(this.listBoxFields);
			this.panelRight.Controls.Add(this.butPaste);
			this.panelRight.Controls.Add(this.label2);
			this.panelRight.Controls.Add(this.butCopy);
			this.panelRight.Controls.Add(this.butEdit);
			this.panelRight.Location = new System.Drawing.Point(544, 0);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(284, 696);
			this.panelRight.TabIndex = 103;
			// 
			// groupShowField
			// 
			this.groupShowField.Controls.Add(this.checkShowScreenChart);
			this.groupShowField.Controls.Add(this.checkShowGrid);
			this.groupShowField.Controls.Add(this.checkShowSpecial);
			this.groupShowField.Controls.Add(this.checkShowSigBoxPractice);
			this.groupShowField.Controls.Add(this.checkShowSigBox);
			this.groupShowField.Controls.Add(this.checkShowRectangle);
			this.groupShowField.Controls.Add(this.checkShowLine);
			this.groupShowField.Controls.Add(this.checkShowPatImage);
			this.groupShowField.Controls.Add(this.checkShowImage);
			this.groupShowField.Controls.Add(this.checkShowComboBox);
			this.groupShowField.Controls.Add(this.butShowNone);
			this.groupShowField.Controls.Add(this.butShowAll);
			this.groupShowField.Controls.Add(this.checkShowCheckBox);
			this.groupShowField.Controls.Add(this.checkShowStaticText);
			this.groupShowField.Controls.Add(this.checkShowInputField);
			this.groupShowField.Controls.Add(this.checkShowOutputText);
			this.groupShowField.Location = new System.Drawing.Point(240, 96);
			this.groupShowField.Name = "groupShowField";
			this.groupShowField.Size = new System.Drawing.Size(40, 461);
			this.groupShowField.TabIndex = 97;
			this.groupShowField.Text = "Show";
			// 
			// checkShowScreenChart
			// 
			this.checkShowScreenChart.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowScreenChart.Checked = true;
			this.checkShowScreenChart.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowScreenChart.Location = new System.Drawing.Point(9, 437);
			this.checkShowScreenChart.Name = "checkShowScreenChart";
			this.checkShowScreenChart.Size = new System.Drawing.Size(20, 18);
			this.checkShowScreenChart.TabIndex = 118;
			this.checkShowScreenChart.UseVisualStyleBackColor = true;
			this.checkShowScreenChart.Click += new System.EventHandler(this.checkShowScreenChart_Click);
			// 
			// checkShowGrid
			// 
			this.checkShowGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowGrid.Checked = true;
			this.checkShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowGrid.Location = new System.Drawing.Point(9, 413);
			this.checkShowGrid.Name = "checkShowGrid";
			this.checkShowGrid.Size = new System.Drawing.Size(20, 18);
			this.checkShowGrid.TabIndex = 117;
			this.checkShowGrid.UseVisualStyleBackColor = true;
			this.checkShowGrid.Click += new System.EventHandler(this.checkShowGrid_Click);
			// 
			// checkShowSpecial
			// 
			this.checkShowSpecial.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowSpecial.Checked = true;
			this.checkShowSpecial.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowSpecial.Location = new System.Drawing.Point(9, 389);
			this.checkShowSpecial.Name = "checkShowSpecial";
			this.checkShowSpecial.Size = new System.Drawing.Size(20, 18);
			this.checkShowSpecial.TabIndex = 116;
			this.checkShowSpecial.UseVisualStyleBackColor = true;
			this.checkShowSpecial.Click += new System.EventHandler(this.checkShowSpecial_Click);
			// 
			// checkShowSigBoxPractice
			// 
			this.checkShowSigBoxPractice.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowSigBoxPractice.Checked = true;
			this.checkShowSigBoxPractice.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowSigBoxPractice.Location = new System.Drawing.Point(9, 353);
			this.checkShowSigBoxPractice.Name = "checkShowSigBoxPractice";
			this.checkShowSigBoxPractice.Size = new System.Drawing.Size(20, 18);
			this.checkShowSigBoxPractice.TabIndex = 115;
			this.checkShowSigBoxPractice.UseVisualStyleBackColor = true;
			this.checkShowSigBoxPractice.Click += new System.EventHandler(this.checkShowSigBoxPractice_Click);
			// 
			// checkShowSigBox
			// 
			this.checkShowSigBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowSigBox.Checked = true;
			this.checkShowSigBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowSigBox.Location = new System.Drawing.Point(9, 329);
			this.checkShowSigBox.Name = "checkShowSigBox";
			this.checkShowSigBox.Size = new System.Drawing.Size(20, 18);
			this.checkShowSigBox.TabIndex = 114;
			this.checkShowSigBox.UseVisualStyleBackColor = true;
			this.checkShowSigBox.Click += new System.EventHandler(this.checkShowSigBox_Click);
			// 
			// checkShowRectangle
			// 
			this.checkShowRectangle.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowRectangle.Checked = true;
			this.checkShowRectangle.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowRectangle.Location = new System.Drawing.Point(9, 293);
			this.checkShowRectangle.Name = "checkShowRectangle";
			this.checkShowRectangle.Size = new System.Drawing.Size(20, 18);
			this.checkShowRectangle.TabIndex = 113;
			this.checkShowRectangle.UseVisualStyleBackColor = true;
			this.checkShowRectangle.Click += new System.EventHandler(this.checkShowRectangle_Click);
			// 
			// checkShowLine
			// 
			this.checkShowLine.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowLine.Checked = true;
			this.checkShowLine.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowLine.Location = new System.Drawing.Point(9, 269);
			this.checkShowLine.Name = "checkShowLine";
			this.checkShowLine.Size = new System.Drawing.Size(20, 18);
			this.checkShowLine.TabIndex = 112;
			this.checkShowLine.UseVisualStyleBackColor = true;
			this.checkShowLine.Click += new System.EventHandler(this.checkShowLine_Click);
			// 
			// checkShowPatImage
			// 
			this.checkShowPatImage.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowPatImage.Checked = true;
			this.checkShowPatImage.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowPatImage.Location = new System.Drawing.Point(9, 233);
			this.checkShowPatImage.Name = "checkShowPatImage";
			this.checkShowPatImage.Size = new System.Drawing.Size(20, 18);
			this.checkShowPatImage.TabIndex = 111;
			this.checkShowPatImage.UseVisualStyleBackColor = true;
			this.checkShowPatImage.Click += new System.EventHandler(this.checkShowPatImage_Click);
			// 
			// checkShowImage
			// 
			this.checkShowImage.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowImage.Checked = true;
			this.checkShowImage.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowImage.Location = new System.Drawing.Point(9, 209);
			this.checkShowImage.Name = "checkShowImage";
			this.checkShowImage.Size = new System.Drawing.Size(20, 18);
			this.checkShowImage.TabIndex = 110;
			this.checkShowImage.UseVisualStyleBackColor = true;
			this.checkShowImage.Click += new System.EventHandler(this.checkShowImage_Click);
			// 
			// checkShowComboBox
			// 
			this.checkShowComboBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowComboBox.Checked = true;
			this.checkShowComboBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowComboBox.Location = new System.Drawing.Point(9, 173);
			this.checkShowComboBox.Name = "checkShowComboBox";
			this.checkShowComboBox.Size = new System.Drawing.Size(20, 18);
			this.checkShowComboBox.TabIndex = 109;
			this.checkShowComboBox.UseVisualStyleBackColor = true;
			this.checkShowComboBox.Click += new System.EventHandler(this.checkShowComboBox_Click);
			// 
			// butShowNone
			// 
			this.butShowNone.Location = new System.Drawing.Point(2, 46);
			this.butShowNone.Name = "butShowNone";
			this.butShowNone.Size = new System.Drawing.Size(36, 24);
			this.butShowNone.TabIndex = 108;
			this.butShowNone.TabStop = false;
			this.butShowNone.Text = "None";
			this.butShowNone.Click += new System.EventHandler(this.butShowNone_Click);
			// 
			// butShowAll
			// 
			this.butShowAll.Location = new System.Drawing.Point(2, 19);
			this.butShowAll.Name = "butShowAll";
			this.butShowAll.Size = new System.Drawing.Size(36, 24);
			this.butShowAll.TabIndex = 97;
			this.butShowAll.TabStop = false;
			this.butShowAll.Text = "All";
			this.butShowAll.Click += new System.EventHandler(this.butShowAll_Click);
			// 
			// checkShowCheckBox
			// 
			this.checkShowCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowCheckBox.Checked = true;
			this.checkShowCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowCheckBox.Location = new System.Drawing.Point(9, 149);
			this.checkShowCheckBox.Name = "checkShowCheckBox";
			this.checkShowCheckBox.Size = new System.Drawing.Size(20, 18);
			this.checkShowCheckBox.TabIndex = 107;
			this.checkShowCheckBox.UseVisualStyleBackColor = true;
			this.checkShowCheckBox.Click += new System.EventHandler(this.checkShowCheckBox_Click);
			// 
			// checkShowStaticText
			// 
			this.checkShowStaticText.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowStaticText.Checked = true;
			this.checkShowStaticText.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowStaticText.Location = new System.Drawing.Point(9, 125);
			this.checkShowStaticText.Name = "checkShowStaticText";
			this.checkShowStaticText.Size = new System.Drawing.Size(20, 18);
			this.checkShowStaticText.TabIndex = 106;
			this.checkShowStaticText.UseVisualStyleBackColor = true;
			this.checkShowStaticText.Click += new System.EventHandler(this.checkShowStaticText_Click);
			// 
			// checkShowInputField
			// 
			this.checkShowInputField.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowInputField.Checked = true;
			this.checkShowInputField.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowInputField.Location = new System.Drawing.Point(9, 101);
			this.checkShowInputField.Name = "checkShowInputField";
			this.checkShowInputField.Size = new System.Drawing.Size(20, 18);
			this.checkShowInputField.TabIndex = 105;
			this.checkShowInputField.UseVisualStyleBackColor = true;
			this.checkShowInputField.Click += new System.EventHandler(this.checkShowInputField_Click);
			// 
			// checkShowOutputText
			// 
			this.checkShowOutputText.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkShowOutputText.Checked = true;
			this.checkShowOutputText.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowOutputText.Location = new System.Drawing.Point(9, 77);
			this.checkShowOutputText.Name = "checkShowOutputText";
			this.checkShowOutputText.Size = new System.Drawing.Size(20, 18);
			this.checkShowOutputText.TabIndex = 104;
			this.checkShowOutputText.UseVisualStyleBackColor = true;
			this.checkShowOutputText.Click += new System.EventHandler(this.checkShowOutputText_Click);
			// 
			// checkBlue
			// 
			this.checkBlue.Location = new System.Drawing.Point(166, 56);
			this.checkBlue.Name = "checkBlue";
			this.checkBlue.Size = new System.Drawing.Size(56, 18);
			this.checkBlue.TabIndex = 4;
			this.checkBlue.Text = "Blue";
			this.checkBlue.UseVisualStyleBackColor = true;
			this.checkBlue.Click += new System.EventHandler(this.checkBlue_Click);
			// 
			// groupAddField
			// 
			this.groupAddField.Controls.Add(this.butScreenChart);
			this.groupAddField.Controls.Add(this.butSpecial);
			this.groupAddField.Controls.Add(this.butGrid);
			this.groupAddField.Controls.Add(this.butSigBox);
			this.groupAddField.Controls.Add(this.butSigBoxPractice);
			this.groupAddField.Controls.Add(this.butLine);
			this.groupAddField.Controls.Add(this.butRectangle);
			this.groupAddField.Controls.Add(this.butPatImage);
			this.groupAddField.Controls.Add(this.butImage);
			this.groupAddField.Controls.Add(this.butComboBox);
			this.groupAddField.Controls.Add(this.butCheckBox);
			this.groupAddField.Controls.Add(this.butStaticText);
			this.groupAddField.Controls.Add(this.butInputField);
			this.groupAddField.Controls.Add(this.butOutputText);
			this.groupAddField.Location = new System.Drawing.Point(158, 151);
			this.groupAddField.Name = "groupAddField";
			this.groupAddField.Size = new System.Drawing.Size(79, 406);
			this.groupAddField.TabIndex = 97;
			this.groupAddField.Text = "Add Field";
			// 
			// butScreenChart
			// 
			this.butScreenChart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butScreenChart.Location = new System.Drawing.Point(2, 378);
			this.butScreenChart.Name = "butScreenChart";
			this.butScreenChart.Size = new System.Drawing.Size(75, 24);
			this.butScreenChart.TabIndex = 124;
			this.butScreenChart.TabStop = false;
			this.butScreenChart.Text = "ScreenChart";
			this.butScreenChart.Visible = false;
			this.butScreenChart.Click += new System.EventHandler(this.butScreenChart_Click);
			// 
			// butSpecial
			// 
			this.butSpecial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSpecial.Location = new System.Drawing.Point(2, 330);
			this.butSpecial.Name = "butSpecial";
			this.butSpecial.Size = new System.Drawing.Size(75, 24);
			this.butSpecial.TabIndex = 122;
			this.butSpecial.TabStop = false;
			this.butSpecial.Text = "Special";
			this.butSpecial.Visible = false;
			this.butSpecial.Click += new System.EventHandler(this.butSpecial_Click);
			// 
			// butGrid
			// 
			this.butGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGrid.Location = new System.Drawing.Point(2, 354);
			this.butGrid.Name = "butGrid";
			this.butGrid.Size = new System.Drawing.Size(75, 24);
			this.butGrid.TabIndex = 123;
			this.butGrid.TabStop = false;
			this.butGrid.Text = "Grid";
			this.butGrid.Click += new System.EventHandler(this.butGrid_Click);
			// 
			// butSigBox
			// 
			this.butSigBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSigBox.Location = new System.Drawing.Point(2, 270);
			this.butSigBox.Name = "butSigBox";
			this.butSigBox.Size = new System.Drawing.Size(75, 24);
			this.butSigBox.TabIndex = 120;
			this.butSigBox.TabStop = false;
			this.butSigBox.Text = "Signature";
			this.butSigBox.Click += new System.EventHandler(this.butSigBox_Click);
			// 
			// butSigBoxPractice
			// 
			this.butSigBoxPractice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSigBoxPractice.Location = new System.Drawing.Point(2, 294);
			this.butSigBoxPractice.Name = "butSigBoxPractice";
			this.butSigBoxPractice.Size = new System.Drawing.Size(75, 24);
			this.butSigBoxPractice.TabIndex = 121;
			this.butSigBoxPractice.TabStop = false;
			this.butSigBoxPractice.Text = "SigPractice";
			this.butSigBoxPractice.Click += new System.EventHandler(this.butSigBoxPractice_Click);
			// 
			// butLine
			// 
			this.butLine.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLine.Location = new System.Drawing.Point(2, 210);
			this.butLine.Name = "butLine";
			this.butLine.Size = new System.Drawing.Size(75, 24);
			this.butLine.TabIndex = 118;
			this.butLine.TabStop = false;
			this.butLine.Text = "Line";
			this.butLine.Click += new System.EventHandler(this.butLine_Click);
			// 
			// butRectangle
			// 
			this.butRectangle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRectangle.Location = new System.Drawing.Point(2, 234);
			this.butRectangle.Name = "butRectangle";
			this.butRectangle.Size = new System.Drawing.Size(75, 24);
			this.butRectangle.TabIndex = 119;
			this.butRectangle.TabStop = false;
			this.butRectangle.Text = "Rectangle";
			this.butRectangle.Click += new System.EventHandler(this.butRectangle_Click);
			// 
			// butPatImage
			// 
			this.butPatImage.Location = new System.Drawing.Point(2, 174);
			this.butPatImage.Name = "butPatImage";
			this.butPatImage.Size = new System.Drawing.Size(75, 24);
			this.butPatImage.TabIndex = 116;
			this.butPatImage.Text = "Pat Image";
			this.butPatImage.Click += new System.EventHandler(this.butPatImage_Click);
			// 
			// butImage
			// 
			this.butImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImage.Location = new System.Drawing.Point(2, 150);
			this.butImage.Name = "butImage";
			this.butImage.Size = new System.Drawing.Size(75, 24);
			this.butImage.TabIndex = 117;
			this.butImage.TabStop = false;
			this.butImage.Text = "Static Image";
			this.butImage.Click += new System.EventHandler(this.butImage_Click);
			// 
			// butComboBox
			// 
			this.butComboBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butComboBox.Location = new System.Drawing.Point(2, 114);
			this.butComboBox.Name = "butComboBox";
			this.butComboBox.Size = new System.Drawing.Size(75, 24);
			this.butComboBox.TabIndex = 115;
			this.butComboBox.TabStop = false;
			this.butComboBox.Text = "Combo Box";
			this.butComboBox.Click += new System.EventHandler(this.butComboBox_Click);
			// 
			// butCheckBox
			// 
			this.butCheckBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCheckBox.Location = new System.Drawing.Point(2, 90);
			this.butCheckBox.Name = "butCheckBox";
			this.butCheckBox.Size = new System.Drawing.Size(75, 24);
			this.butCheckBox.TabIndex = 114;
			this.butCheckBox.TabStop = false;
			this.butCheckBox.Text = "Check Box";
			this.butCheckBox.Click += new System.EventHandler(this.butCheckBox_Click);
			// 
			// butStaticText
			// 
			this.butStaticText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butStaticText.Location = new System.Drawing.Point(2, 66);
			this.butStaticText.Name = "butStaticText";
			this.butStaticText.Size = new System.Drawing.Size(75, 24);
			this.butStaticText.TabIndex = 113;
			this.butStaticText.TabStop = false;
			this.butStaticText.Text = "Static Text";
			this.butStaticText.Click += new System.EventHandler(this.butStaticText_Click);
			// 
			// butInputField
			// 
			this.butInputField.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butInputField.Location = new System.Drawing.Point(2, 42);
			this.butInputField.Name = "butInputField";
			this.butInputField.Size = new System.Drawing.Size(75, 24);
			this.butInputField.TabIndex = 112;
			this.butInputField.TabStop = false;
			this.butInputField.Text = "Input Field";
			this.butInputField.Click += new System.EventHandler(this.butInputField_Click);
			// 
			// butOutputText
			// 
			this.butOutputText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butOutputText.Location = new System.Drawing.Point(2, 18);
			this.butOutputText.Name = "butOutputText";
			this.butOutputText.Size = new System.Drawing.Size(75, 24);
			this.butOutputText.TabIndex = 111;
			this.butOutputText.TabStop = false;
			this.butOutputText.Text = "Output Text";
			this.butOutputText.Click += new System.EventHandler(this.butOutputText_Click);
			// 
			// butRedo
			// 
			this.butRedo.Location = new System.Drawing.Point(82, 52);
			this.butRedo.Name = "butRedo";
			this.butRedo.Size = new System.Drawing.Size(74, 24);
			this.butRedo.TabIndex = 103;
			this.butRedo.TabStop = false;
			this.butRedo.Text = "Redo Ctrl-Y";
			this.butRedo.Click += new System.EventHandler(this.butRedo_Click);
			// 
			// butUndo
			// 
			this.butUndo.Location = new System.Drawing.Point(4, 52);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(74, 24);
			this.butUndo.TabIndex = 102;
			this.butUndo.TabStop = false;
			this.butUndo.Text = "Undo Ctrl-Z";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// FormSheetDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(828, 696);
			this.Controls.Add(this.panelLeft);
			this.Controls.Add(this.panelRight);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDefEdit";
			this.Text = "Edit Sheet Def";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSheetDefEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormSheetDefEdit_Load);
			this.Shown += new System.EventHandler(this.FormSheetDefEdit_Shown);
			this.ResizeEnd += new System.EventHandler(this.FormSheetDefEdit_ResizeEnd);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormSheetDefEdit_KeyUp);
			this.Resize += new System.EventHandler(this.FormSheetDefEdit_Resize);
			this.groupBoxSubViews.ResumeLayout(false);
			this.groupAlignH.ResumeLayout(false);
			this.groupPage.ResumeLayout(false);
			this.panelLeft.ResumeLayout(false);
			this.panelRight.ResumeLayout(false);
			this.panelRight.PerformLayout();
			this.groupShowField.ResumeLayout(false);
			this.groupAddField.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDescription;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label labelInternal;
		private OpenDental.UI.ListBoxOD listBoxFields;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butEdit;
		private UI.Button butAlignTop;
		private UI.Button butAlignLeft;
		private UI.Button butCopy;
		private UI.Button butPaste;
		private System.Windows.Forms.LinkLabel linkLabelTips;
		private UI.Button butTabOrder;
		private UI.Button butPageAdd;
		private UI.Button butPageRemove;
		private OpenDental.UI.GroupBoxOD groupPage;
		private OpenDental.UI.GroupBoxOD groupAlignH;
		private UI.Button butAlignRight;
		private UI.Button butAlignCenterH;
		private UI.Button butMobile;
		private PanelGraphics panelMain;
		private UI.Button butAutoSnap;
		private OpenDental.UI.GroupBoxOD groupBoxSubViews;
		private OpenDental.UI.ComboBoxOD comboLanguages;
		private System.Windows.Forms.CheckBox checkSynchMatchedFields;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Panel panelRight;
		private UI.Button butUndo;
		private UI.Button butRedo;
		private UI.GroupBoxOD groupAddField;
		private UI.Button butComboBox;
		private UI.Button butCheckBox;
		private UI.Button butStaticText;
		private UI.Button butInputField;
		private UI.Button butOutputText;
		private UI.Button butPatImage;
		private UI.Button butImage;
		private UI.Button butLine;
		private UI.Button butRectangle;
		private UI.Button butSigBox;
		private UI.Button butSigBoxPractice;
		private UI.Button butScreenChart;
		private UI.Button butSpecial;
		private UI.Button butGrid;
		private System.Windows.Forms.CheckBox checkBlue;
		private UI.GroupBoxOD groupShowField;
		private System.Windows.Forms.CheckBox checkShowScreenChart;
		private System.Windows.Forms.CheckBox checkShowGrid;
		private System.Windows.Forms.CheckBox checkShowSpecial;
		private System.Windows.Forms.CheckBox checkShowSigBoxPractice;
		private System.Windows.Forms.CheckBox checkShowSigBox;
		private System.Windows.Forms.CheckBox checkShowRectangle;
		private System.Windows.Forms.CheckBox checkShowLine;
		private System.Windows.Forms.CheckBox checkShowPatImage;
		private System.Windows.Forms.CheckBox checkShowImage;
		private System.Windows.Forms.CheckBox checkShowComboBox;
		private UI.Button butShowNone;
		private UI.Button butShowAll;
		private System.Windows.Forms.CheckBox checkShowCheckBox;
		private System.Windows.Forms.CheckBox checkShowStaticText;
		private System.Windows.Forms.CheckBox checkShowInputField;
		private System.Windows.Forms.CheckBox checkShowOutputText;
	}
}