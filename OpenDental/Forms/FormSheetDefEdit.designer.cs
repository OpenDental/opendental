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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panelMain = new OpenDental.PanelGraphics();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBoxSubViews = new System.Windows.Forms.GroupBox();
			this.comboLanguages = new System.Windows.Forms.ComboBox();
			this.radioLayoutTP = new System.Windows.Forms.RadioButton();
			this.radioLayoutDefault = new System.Windows.Forms.RadioButton();
			this.butAutoSnap = new OpenDental.UI.Button();
			this.butMobile = new OpenDental.UI.Button();
			this.groupAlignV = new System.Windows.Forms.GroupBox();
			this.butAlignTop = new OpenDental.UI.Button();
			this.groupAlignH = new System.Windows.Forms.GroupBox();
			this.butAlignRight = new OpenDental.UI.Button();
			this.butAlignCenterH = new OpenDental.UI.Button();
			this.butAlignLeft = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.groupPage = new System.Windows.Forms.GroupBox();
			this.butPageAdd = new OpenDental.UI.Button();
			this.butPageRemove = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butTabOrder = new OpenDental.UI.Button();
			this.labelInternal = new System.Windows.Forms.Label();
			this.linkLabelTips = new System.Windows.Forms.LinkLabel();
			this.listFields = new OpenDental.UI.ListBoxOD();
			this.butPaste = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butCopy = new OpenDental.UI.Button();
			this.groupAddNew = new System.Windows.Forms.GroupBox();
			this.butAddSigBoxPractice = new OpenDental.UI.Button();
			this.butScreenChart = new OpenDental.UI.Button();
			this.butAddCombo = new OpenDental.UI.Button();
			this.butAddSpecial = new OpenDental.UI.Button();
			this.butAddGrid = new OpenDental.UI.Button();
			this.butAddPatImage = new OpenDental.UI.Button();
			this.butAddSigBox = new OpenDental.UI.Button();
			this.butAddCheckBox = new OpenDental.UI.Button();
			this.butAddRect = new OpenDental.UI.Button();
			this.butAddLine = new OpenDental.UI.Button();
			this.butAddImage = new OpenDental.UI.Button();
			this.butAddStaticText = new OpenDental.UI.Button();
			this.butAddInputField = new OpenDental.UI.Button();
			this.butAddOutputText = new OpenDental.UI.Button();
			this.butEdit = new OpenDental.UI.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBoxSubViews.SuspendLayout();
			this.groupAlignV.SuspendLayout();
			this.groupAlignH.SuspendLayout();
			this.groupPage.SuspendLayout();
			this.groupAddNew.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.AutoScroll = true;
			this.splitContainer1.Panel1.Controls.Add(this.panelMain);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.butCancel);
			this.splitContainer1.Panel2.Controls.Add(this.butOK);
			this.splitContainer1.Panel2.Controls.Add(this.groupBoxSubViews);
			this.splitContainer1.Panel2.Controls.Add(this.butAutoSnap);
			this.splitContainer1.Panel2.Controls.Add(this.butMobile);
			this.splitContainer1.Panel2.Controls.Add(this.groupAlignV);
			this.splitContainer1.Panel2.Controls.Add(this.groupAlignH);
			this.splitContainer1.Panel2.Controls.Add(this.textDescription);
			this.splitContainer1.Panel2.Controls.Add(this.groupPage);
			this.splitContainer1.Panel2.Controls.Add(this.butDelete);
			this.splitContainer1.Panel2.Controls.Add(this.butTabOrder);
			this.splitContainer1.Panel2.Controls.Add(this.labelInternal);
			this.splitContainer1.Panel2.Controls.Add(this.linkLabelTips);
			this.splitContainer1.Panel2.Controls.Add(this.listFields);
			this.splitContainer1.Panel2.Controls.Add(this.butPaste);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.butCopy);
			this.splitContainer1.Panel2.Controls.Add(this.groupAddNew);
			this.splitContainer1.Panel2.Controls.Add(this.butEdit);
			this.splitContainer1.Size = new System.Drawing.Size(828, 794);
			this.splitContainer1.SplitterDistance = 660;
			this.splitContainer1.TabIndex = 98;
			this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelMain.BackColor = System.Drawing.Color.Transparent;
			this.panelMain.Location = new System.Drawing.Point(4, 4);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(649, 784);
			this.panelMain.TabIndex = 1;
			this.panelMain.TabStop = true;
			this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
			this.panelMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDoubleClick);
			this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
			this.panelMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseMove);
			this.panelMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseUp);
			this.panelMain.Resize += new System.EventHandler(this.panelMain_Resize);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(85, 759);
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
			this.butOK.Location = new System.Drawing.Point(85, 730);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(72, 24);
			this.butOK.TabIndex = 3;
			this.butOK.TabStop = false;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupBoxSubViews
			// 
			this.groupBoxSubViews.Controls.Add(this.comboLanguages);
			this.groupBoxSubViews.Controls.Add(this.radioLayoutTP);
			this.groupBoxSubViews.Controls.Add(this.radioLayoutDefault);
			this.groupBoxSubViews.Location = new System.Drawing.Point(9, 218);
			this.groupBoxSubViews.Name = "groupBoxSubViews";
			this.groupBoxSubViews.Size = new System.Drawing.Size(141, 41);
			this.groupBoxSubViews.TabIndex = 100;
			this.groupBoxSubViews.TabStop = false;
			this.groupBoxSubViews.Text = "Language";
			// 
			// comboLanguages
			// 
			this.comboLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLanguages.Location = new System.Drawing.Point(6, 15);
			this.comboLanguages.Name = "comboLanguages";
			this.comboLanguages.Size = new System.Drawing.Size(131, 21);
			this.comboLanguages.TabIndex = 2;
			this.comboLanguages.SelectionChangeCommitted += new System.EventHandler(this.comboLanguages_SelectionChangeCommitted);
			// 
			// radioLayoutTP
			// 
			this.radioLayoutTP.AutoSize = true;
			this.radioLayoutTP.Location = new System.Drawing.Point(72, 20);
			this.radioLayoutTP.Name = "radioLayoutTP";
			this.radioLayoutTP.Size = new System.Drawing.Size(71, 17);
			this.radioLayoutTP.TabIndex = 1;
			this.radioLayoutTP.TabStop = true;
			this.radioLayoutTP.Text = "TreatPlan";
			this.radioLayoutTP.UseVisualStyleBackColor = true;
			// 
			// radioLayoutDefault
			// 
			this.radioLayoutDefault.AutoSize = true;
			this.radioLayoutDefault.Location = new System.Drawing.Point(7, 20);
			this.radioLayoutDefault.Name = "radioLayoutDefault";
			this.radioLayoutDefault.Size = new System.Drawing.Size(59, 17);
			this.radioLayoutDefault.TabIndex = 0;
			this.radioLayoutDefault.TabStop = true;
			this.radioLayoutDefault.Text = "Default";
			this.radioLayoutDefault.UseVisualStyleBackColor = true;
			this.radioLayoutDefault.CheckedChanged += new System.EventHandler(this.radioLayoutDefault_CheckedChanged);
			// 
			// butAutoSnap
			// 
			this.butAutoSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAutoSnap.Location = new System.Drawing.Point(9, 534);
			this.butAutoSnap.Name = "butAutoSnap";
			this.butAutoSnap.Size = new System.Drawing.Size(148, 24);
			this.butAutoSnap.TabIndex = 99;
			this.butAutoSnap.TabStop = false;
			this.butAutoSnap.Text = "Setup Auto Snap";
			this.butAutoSnap.Click += new System.EventHandler(this.butAutoSnap_Click);
			// 
			// butMobile
			// 
			this.butMobile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMobile.Location = new System.Drawing.Point(9, 31);
			this.butMobile.Name = "butMobile";
			this.butMobile.Size = new System.Drawing.Size(59, 24);
			this.butMobile.TabIndex = 98;
			this.butMobile.TabStop = false;
			this.butMobile.Text = "Mobile";
			this.butMobile.Click += new System.EventHandler(this.butMobile_Click);
			// 
			// groupAlignV
			// 
			this.groupAlignV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupAlignV.Controls.Add(this.butAlignTop);
			this.groupAlignV.Location = new System.Drawing.Point(9, 571);
			this.groupAlignV.Name = "groupAlignV";
			this.groupAlignV.Size = new System.Drawing.Size(142, 46);
			this.groupAlignV.TabIndex = 97;
			this.groupAlignV.TabStop = false;
			this.groupAlignV.Text = "Vertical Align";
			// 
			// butAlignTop
			// 
			this.butAlignTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlignTop.Location = new System.Drawing.Point(4, 19);
			this.butAlignTop.Name = "butAlignTop";
			this.butAlignTop.Size = new System.Drawing.Size(66, 20);
			this.butAlignTop.TabIndex = 88;
			this.butAlignTop.TabStop = false;
			this.butAlignTop.Text = "Top";
			this.butAlignTop.Click += new System.EventHandler(this.butAlignTop_Click);
			// 
			// groupAlignH
			// 
			this.groupAlignH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupAlignH.Controls.Add(this.butAlignRight);
			this.groupAlignH.Controls.Add(this.butAlignCenterH);
			this.groupAlignH.Controls.Add(this.butAlignLeft);
			this.groupAlignH.Location = new System.Drawing.Point(9, 617);
			this.groupAlignH.Name = "groupAlignH";
			this.groupAlignH.Size = new System.Drawing.Size(144, 46);
			this.groupAlignH.TabIndex = 96;
			this.groupAlignH.TabStop = false;
			this.groupAlignH.Text = "Horizontal Align";
			// 
			// butAlignRight
			// 
			this.butAlignRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlignRight.Location = new System.Drawing.Point(94, 15);
			this.butAlignRight.Name = "butAlignRight";
			this.butAlignRight.Size = new System.Drawing.Size(43, 20);
			this.butAlignRight.TabIndex = 91;
			this.butAlignRight.TabStop = false;
			this.butAlignRight.Text = "Right";
			this.butAlignRight.Click += new System.EventHandler(this.butAlignRight_Click);
			// 
			// butAlignCenterH
			// 
			this.butAlignCenterH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlignCenterH.Location = new System.Drawing.Point(50, 15);
			this.butAlignCenterH.Name = "butAlignCenterH";
			this.butAlignCenterH.Size = new System.Drawing.Size(43, 20);
			this.butAlignCenterH.TabIndex = 90;
			this.butAlignCenterH.TabStop = false;
			this.butAlignCenterH.Text = "Center";
			this.butAlignCenterH.Click += new System.EventHandler(this.butAlignCenterH_Click);
			// 
			// butAlignLeft
			// 
			this.butAlignLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlignLeft.Location = new System.Drawing.Point(6, 15);
			this.butAlignLeft.Name = "butAlignLeft";
			this.butAlignLeft.Size = new System.Drawing.Size(43, 20);
			this.butAlignLeft.TabIndex = 89;
			this.butAlignLeft.TabStop = false;
			this.butAlignLeft.Text = "Left";
			this.butAlignLeft.Click += new System.EventHandler(this.butAlignLeft_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(9, 10);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(148, 20);
			this.textDescription.TabIndex = 0;
			// 
			// groupPage
			// 
			this.groupPage.Controls.Add(this.butPageAdd);
			this.groupPage.Controls.Add(this.butPageRemove);
			this.groupPage.Location = new System.Drawing.Point(9, 265);
			this.groupPage.Name = "groupPage";
			this.groupPage.Size = new System.Drawing.Size(144, 39);
			this.groupPage.TabIndex = 95;
			this.groupPage.TabStop = false;
			this.groupPage.Text = "Pages";
			// 
			// butPageAdd
			// 
			this.butPageAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPageAdd.Location = new System.Drawing.Point(3, 15);
			this.butPageAdd.Name = "butPageAdd";
			this.butPageAdd.Size = new System.Drawing.Size(69, 20);
			this.butPageAdd.TabIndex = 95;
			this.butPageAdd.TabStop = false;
			this.butPageAdd.Text = "Add";
			this.butPageAdd.Click += new System.EventHandler(this.butPageAdd_Click);
			// 
			// butPageRemove
			// 
			this.butPageRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPageRemove.Location = new System.Drawing.Point(72, 15);
			this.butPageRemove.Name = "butPageRemove";
			this.butPageRemove.Size = new System.Drawing.Size(69, 20);
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
			this.butDelete.Location = new System.Drawing.Point(15, 759);
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
			this.butTabOrder.Location = new System.Drawing.Point(15, 693);
			this.butTabOrder.Name = "butTabOrder";
			this.butTabOrder.Size = new System.Drawing.Size(70, 20);
			this.butTabOrder.TabIndex = 94;
			this.butTabOrder.TabStop = false;
			this.butTabOrder.Text = "Tab Order";
			this.butTabOrder.Click += new System.EventHandler(this.butTabOrder_Click);
			// 
			// labelInternal
			// 
			this.labelInternal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInternal.Location = new System.Drawing.Point(9, 684);
			this.labelInternal.Name = "labelInternal";
			this.labelInternal.Size = new System.Drawing.Size(144, 46);
			this.labelInternal.TabIndex = 82;
			this.labelInternal.Text = "This is an internal sheet, so it may not be edited.  Make a copy instead.";
			this.labelInternal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// linkLabelTips
			// 
			this.linkLabelTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabelTips.Location = new System.Drawing.Point(126, 688);
			this.linkLabelTips.Name = "linkLabelTips";
			this.linkLabelTips.Size = new System.Drawing.Size(31, 17);
			this.linkLabelTips.TabIndex = 93;
			this.linkLabelTips.TabStop = true;
			this.linkLabelTips.Text = "tips";
			this.linkLabelTips.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.linkLabelTips.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTips_LinkClicked);
			// 
			// listFields
			// 
			this.listFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listFields.IntegralHeight = false;
			this.listFields.Location = new System.Drawing.Point(9, 323);
			this.listFields.Name = "listFields";
			this.listFields.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listFields.Size = new System.Drawing.Size(148, 209);
			this.listFields.TabIndex = 83;
			this.listFields.SelectionChangeCommitted += new System.EventHandler(this.listFields_SelectionChangeCommitted);
			this.listFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFields_MouseDoubleClick);
			// 
			// butPaste
			// 
			this.butPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPaste.Location = new System.Drawing.Point(85, 665);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(72, 20);
			this.butPaste.TabIndex = 91;
			this.butPaste.TabStop = false;
			this.butPaste.Text = "Paste";
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(9, 307);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 84;
			this.label2.Text = "Fields";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopy.Location = new System.Drawing.Point(15, 665);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(70, 20);
			this.butCopy.TabIndex = 90;
			this.butCopy.TabStop = false;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// groupAddNew
			// 
			this.groupAddNew.Controls.Add(this.butAddSigBoxPractice);
			this.groupAddNew.Controls.Add(this.butScreenChart);
			this.groupAddNew.Controls.Add(this.butAddCombo);
			this.groupAddNew.Controls.Add(this.butAddSpecial);
			this.groupAddNew.Controls.Add(this.butAddGrid);
			this.groupAddNew.Controls.Add(this.butAddPatImage);
			this.groupAddNew.Controls.Add(this.butAddSigBox);
			this.groupAddNew.Controls.Add(this.butAddCheckBox);
			this.groupAddNew.Controls.Add(this.butAddRect);
			this.groupAddNew.Controls.Add(this.butAddLine);
			this.groupAddNew.Controls.Add(this.butAddImage);
			this.groupAddNew.Controls.Add(this.butAddStaticText);
			this.groupAddNew.Controls.Add(this.butAddInputField);
			this.groupAddNew.Controls.Add(this.butAddOutputText);
			this.groupAddNew.Location = new System.Drawing.Point(9, 55);
			this.groupAddNew.Name = "groupAddNew";
			this.groupAddNew.Size = new System.Drawing.Size(144, 161);
			this.groupAddNew.TabIndex = 86;
			this.groupAddNew.TabStop = false;
			this.groupAddNew.Text = "Add new";
			// 
			// butAddSigBoxPractice
			// 
			this.butAddSigBoxPractice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSigBoxPractice.Location = new System.Drawing.Point(72, 95);
			this.butAddSigBoxPractice.Name = "butAddSigBoxPractice";
			this.butAddSigBoxPractice.Size = new System.Drawing.Size(69, 20);
			this.butAddSigBoxPractice.TabIndex = 98;
			this.butAddSigBoxPractice.TabStop = false;
			this.butAddSigBoxPractice.Text = "SigPractice";
			this.butAddSigBoxPractice.Click += new System.EventHandler(this.butAddSigBoxPractice_Click);
			// 
			// butScreenChart
			// 
			this.butScreenChart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butScreenChart.Location = new System.Drawing.Point(3, 115);
			this.butScreenChart.Name = "butScreenChart";
			this.butScreenChart.Size = new System.Drawing.Size(69, 20);
			this.butScreenChart.TabIndex = 97;
			this.butScreenChart.TabStop = false;
			this.butScreenChart.Text = "ScreenChart";
			this.butScreenChart.Visible = false;
			this.butScreenChart.Click += new System.EventHandler(this.butScreenChart_Click);
			// 
			// butAddCombo
			// 
			this.butAddCombo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCombo.Location = new System.Drawing.Point(3, 95);
			this.butAddCombo.Name = "butAddCombo";
			this.butAddCombo.Size = new System.Drawing.Size(69, 20);
			this.butAddCombo.TabIndex = 96;
			this.butAddCombo.TabStop = false;
			this.butAddCombo.Text = "ComboBox";
			this.butAddCombo.Click += new System.EventHandler(this.butAddCombo_Click);
			// 
			// butAddSpecial
			// 
			this.butAddSpecial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSpecial.Location = new System.Drawing.Point(3, 135);
			this.butAddSpecial.Name = "butAddSpecial";
			this.butAddSpecial.Size = new System.Drawing.Size(69, 20);
			this.butAddSpecial.TabIndex = 94;
			this.butAddSpecial.TabStop = false;
			this.butAddSpecial.Text = "Special";
			this.butAddSpecial.Visible = false;
			this.butAddSpecial.Click += new System.EventHandler(this.butAddSpecial_Click);
			// 
			// butAddGrid
			// 
			this.butAddGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddGrid.Location = new System.Drawing.Point(72, 135);
			this.butAddGrid.Name = "butAddGrid";
			this.butAddGrid.Size = new System.Drawing.Size(69, 20);
			this.butAddGrid.TabIndex = 95;
			this.butAddGrid.TabStop = false;
			this.butAddGrid.Text = "Grid";
			this.butAddGrid.Click += new System.EventHandler(this.butAddGrid_Click);
			// 
			// butAddPatImage
			// 
			this.butAddPatImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddPatImage.Location = new System.Drawing.Point(72, 115);
			this.butAddPatImage.Name = "butAddPatImage";
			this.butAddPatImage.Size = new System.Drawing.Size(69, 20);
			this.butAddPatImage.TabIndex = 93;
			this.butAddPatImage.TabStop = false;
			this.butAddPatImage.Text = "Pat Image";
			this.butAddPatImage.Click += new System.EventHandler(this.butAddPatImage_Click);
			// 
			// butAddSigBox
			// 
			this.butAddSigBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSigBox.Location = new System.Drawing.Point(72, 75);
			this.butAddSigBox.Name = "butAddSigBox";
			this.butAddSigBox.Size = new System.Drawing.Size(69, 20);
			this.butAddSigBox.TabIndex = 92;
			this.butAddSigBox.TabStop = false;
			this.butAddSigBox.Text = "Signature";
			this.butAddSigBox.Click += new System.EventHandler(this.butAddSigBox_Click);
			// 
			// butAddCheckBox
			// 
			this.butAddCheckBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCheckBox.Location = new System.Drawing.Point(3, 55);
			this.butAddCheckBox.Name = "butAddCheckBox";
			this.butAddCheckBox.Size = new System.Drawing.Size(69, 20);
			this.butAddCheckBox.TabIndex = 91;
			this.butAddCheckBox.TabStop = false;
			this.butAddCheckBox.Text = "CheckBox";
			this.butAddCheckBox.Click += new System.EventHandler(this.butAddCheckBox_Click);
			// 
			// butAddRect
			// 
			this.butAddRect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddRect.Location = new System.Drawing.Point(72, 55);
			this.butAddRect.Name = "butAddRect";
			this.butAddRect.Size = new System.Drawing.Size(69, 20);
			this.butAddRect.TabIndex = 90;
			this.butAddRect.TabStop = false;
			this.butAddRect.Text = "Rectangle";
			this.butAddRect.Click += new System.EventHandler(this.butAddRect_Click);
			// 
			// butAddLine
			// 
			this.butAddLine.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddLine.Location = new System.Drawing.Point(72, 35);
			this.butAddLine.Name = "butAddLine";
			this.butAddLine.Size = new System.Drawing.Size(69, 20);
			this.butAddLine.TabIndex = 89;
			this.butAddLine.TabStop = false;
			this.butAddLine.Text = "Line";
			this.butAddLine.Click += new System.EventHandler(this.butAddLine_Click);
			// 
			// butAddImage
			// 
			this.butAddImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddImage.Location = new System.Drawing.Point(3, 75);
			this.butAddImage.Name = "butAddImage";
			this.butAddImage.Size = new System.Drawing.Size(69, 20);
			this.butAddImage.TabIndex = 88;
			this.butAddImage.TabStop = false;
			this.butAddImage.Text = "StaticImage";
			this.butAddImage.Click += new System.EventHandler(this.butAddImage_Click);
			// 
			// butAddStaticText
			// 
			this.butAddStaticText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddStaticText.Location = new System.Drawing.Point(72, 15);
			this.butAddStaticText.Name = "butAddStaticText";
			this.butAddStaticText.Size = new System.Drawing.Size(69, 20);
			this.butAddStaticText.TabIndex = 87;
			this.butAddStaticText.TabStop = false;
			this.butAddStaticText.Text = "StaticText";
			this.butAddStaticText.Click += new System.EventHandler(this.butAddStaticText_Click);
			// 
			// butAddInputField
			// 
			this.butAddInputField.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddInputField.Location = new System.Drawing.Point(3, 35);
			this.butAddInputField.Name = "butAddInputField";
			this.butAddInputField.Size = new System.Drawing.Size(69, 20);
			this.butAddInputField.TabIndex = 86;
			this.butAddInputField.TabStop = false;
			this.butAddInputField.Text = "InputField";
			this.butAddInputField.Click += new System.EventHandler(this.butAddInputField_Click);
			// 
			// butAddOutputText
			// 
			this.butAddOutputText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddOutputText.Location = new System.Drawing.Point(3, 15);
			this.butAddOutputText.Name = "butAddOutputText";
			this.butAddOutputText.Size = new System.Drawing.Size(69, 20);
			this.butAddOutputText.TabIndex = 85;
			this.butAddOutputText.TabStop = false;
			this.butAddOutputText.Text = "OutputText";
			this.butAddOutputText.Click += new System.EventHandler(this.butAddOutputText_Click);
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(67, 31);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(90, 24);
			this.butEdit.TabIndex = 87;
			this.butEdit.TabStop = false;
			this.butEdit.Text = "Edit Properties";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// FormSheetDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(828, 794);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDefEdit";
			this.Text = "Edit Sheet Def";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSheetDefEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormSheetDefEdit_Load);
			this.Shown += new System.EventHandler(this.FormSheetDefEdit_Shown);
			this.ResizeEnd += new System.EventHandler(this.FormSheetDefEdit_ResizeEnd);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormSheetDefEdit_KeyUp);
			this.Resize += new System.EventHandler(this.FormSheetDefEdit_Resize);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBoxSubViews.ResumeLayout(false);
			this.groupBoxSubViews.PerformLayout();
			this.groupAlignV.ResumeLayout(false);
			this.groupAlignH.ResumeLayout(false);
			this.groupPage.ResumeLayout(false);
			this.groupAddNew.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDescription;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label labelInternal;
		private OpenDental.UI.ListBoxOD listFields;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butAddOutputText;
		private System.Windows.Forms.GroupBox groupAddNew;
		private OpenDental.UI.Button butAddStaticText;
		private OpenDental.UI.Button butAddInputField;
		private OpenDental.UI.Button butEdit;
		private OpenDental.UI.Button butAddImage;
		private OpenDental.UI.Button butAddRect;
		private OpenDental.UI.Button butAddLine;
		private OpenDental.UI.Button butAddCheckBox;
		private OpenDental.UI.Button butAddSigBox;
		private OpenDental.UI.Button butAddPatImage;
		private UI.Button butAlignTop;
		private UI.Button butAlignLeft;
		private UI.Button butCopy;
		private UI.Button butPaste;
		private System.Windows.Forms.LinkLabel linkLabelTips;
		private UI.Button butTabOrder;
		private UI.Button butAddSpecial;
		private UI.Button butPageAdd;
		private UI.Button butPageRemove;
		private System.Windows.Forms.GroupBox groupPage;
		private UI.Button butAddGrid;
		private System.Windows.Forms.GroupBox groupAlignH;
		private UI.Button butAlignRight;
		private UI.Button butAlignCenterH;
		private System.Windows.Forms.GroupBox groupAlignV;
		private UI.Button butAddCombo;
		private UI.Button butScreenChart;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private UI.Button butMobile;
		private PanelGraphics panelMain;
		private UI.Button butAddSigBoxPractice;
		private UI.Button butAutoSnap;
		private System.Windows.Forms.GroupBox groupBoxSubViews;
		private System.Windows.Forms.RadioButton radioLayoutTP;
		private System.Windows.Forms.RadioButton radioLayoutDefault;
		private System.Windows.Forms.ComboBox comboLanguages;
	}
}