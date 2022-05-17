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
			this.butAddField = new OpenDental.UI.Button();
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
			this.listFields = new OpenDental.UI.ListBoxOD();
			this.butPaste = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butCopy = new OpenDental.UI.Button();
			this.butEdit = new OpenDental.UI.Button();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.panelRight = new System.Windows.Forms.Panel();
			this.butUndo = new OpenDental.UI.Button();
			this.butRedo = new OpenDental.UI.Button();
			this.groupBoxSubViews.SuspendLayout();
			this.groupAlignH.SuspendLayout();
			this.groupPage.SuspendLayout();
			this.panelLeft.SuspendLayout();
			this.panelRight.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelMain.BackColor = System.Drawing.Color.Transparent;
			this.panelMain.Location = new System.Drawing.Point(4, 4);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(660, 788);
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
			this.butAlignTop.Location = new System.Drawing.Point(92, 203);
			this.butAlignTop.Name = "butAlignTop";
			this.butAlignTop.Size = new System.Drawing.Size(66, 24);
			this.butAlignTop.TabIndex = 88;
			this.butAlignTop.TabStop = false;
			this.butAlignTop.Text = "Align Tops";
			this.butAlignTop.Click += new System.EventHandler(this.butAlignTop_Click);
			// 
			// butAddField
			// 
			this.butAddField.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddField.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddField.Location = new System.Drawing.Point(5, 77);
			this.butAddField.Name = "butAddField";
			this.butAddField.Size = new System.Drawing.Size(90, 24);
			this.butAddField.TabIndex = 101;
			this.butAddField.TabStop = false;
			this.butAddField.Text = "Add Field";
			this.butAddField.Click += new System.EventHandler(this.butAddField_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(87, 763);
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
			this.butOK.Location = new System.Drawing.Point(87, 736);
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
			this.groupBoxSubViews.Location = new System.Drawing.Point(4, 583);
			this.groupBoxSubViews.Name = "groupBoxSubViews";
			this.groupBoxSubViews.Size = new System.Drawing.Size(153, 60);
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
			this.butAutoSnap.Location = new System.Drawing.Point(4, 203);
			this.butAutoSnap.Name = "butAutoSnap";
			this.butAutoSnap.Size = new System.Drawing.Size(76, 24);
			this.butAutoSnap.TabIndex = 99;
			this.butAutoSnap.TabStop = false;
			this.butAutoSnap.Text = "Grid Snap";
			this.butAutoSnap.Click += new System.EventHandler(this.butAutoSnap_Click);
			// 
			// butMobile
			// 
			this.butMobile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMobile.Location = new System.Drawing.Point(5, 52);
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
			this.groupAlignH.Location = new System.Drawing.Point(4, 154);
			this.groupAlignH.Name = "groupAlignH";
			this.groupAlignH.Size = new System.Drawing.Size(154, 46);
			this.groupAlignH.TabIndex = 96;
			this.groupAlignH.Text = "Horizontal Align";
			// 
			// butAlignRight
			// 
			this.butAlignRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlignRight.Location = new System.Drawing.Point(106, 15);
			this.butAlignRight.Name = "butAlignRight";
			this.butAlignRight.Size = new System.Drawing.Size(43, 24);
			this.butAlignRight.TabIndex = 91;
			this.butAlignRight.TabStop = false;
			this.butAlignRight.Text = "Right";
			this.butAlignRight.Click += new System.EventHandler(this.butAlignRight_Click);
			// 
			// butAlignCenterH
			// 
			this.butAlignCenterH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlignCenterH.Location = new System.Drawing.Point(56, 15);
			this.butAlignCenterH.Name = "butAlignCenterH";
			this.butAlignCenterH.Size = new System.Drawing.Size(43, 24);
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
			this.butAlignLeft.Size = new System.Drawing.Size(43, 24);
			this.butAlignLeft.TabIndex = 89;
			this.butAlignLeft.TabStop = false;
			this.butAlignLeft.Text = "Left";
			this.butAlignLeft.Click += new System.EventHandler(this.butAlignLeft_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(6, 5);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(149, 20);
			this.textDescription.TabIndex = 0;
			// 
			// groupPage
			// 
			this.groupPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupPage.Controls.Add(this.butPageAdd);
			this.groupPage.Controls.Add(this.butPageRemove);
			this.groupPage.Location = new System.Drawing.Point(4, 645);
			this.groupPage.Name = "groupPage";
			this.groupPage.Size = new System.Drawing.Size(153, 44);
			this.groupPage.TabIndex = 95;
			this.groupPage.Text = "Pages";
			// 
			// butPageAdd
			// 
			this.butPageAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPageAdd.Location = new System.Drawing.Point(7, 15);
			this.butPageAdd.Name = "butPageAdd";
			this.butPageAdd.Size = new System.Drawing.Size(69, 24);
			this.butPageAdd.TabIndex = 95;
			this.butPageAdd.TabStop = false;
			this.butPageAdd.Text = "Add";
			this.butPageAdd.Click += new System.EventHandler(this.butPageAdd_Click);
			// 
			// butPageRemove
			// 
			this.butPageRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPageRemove.Location = new System.Drawing.Point(81, 15);
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
			this.butDelete.Location = new System.Drawing.Point(9, 763);
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
			this.butTabOrder.Location = new System.Drawing.Point(12, 696);
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
			this.labelInternal.Location = new System.Drawing.Point(9, 687);
			this.labelInternal.Name = "labelInternal";
			this.labelInternal.Size = new System.Drawing.Size(144, 46);
			this.labelInternal.TabIndex = 82;
			this.labelInternal.Text = "This is an internal sheet, so it may not be edited.  Make a copy instead.";
			this.labelInternal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// linkLabelTips
			// 
			this.linkLabelTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabelTips.Location = new System.Drawing.Point(126, 691);
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
			this.listFields.Location = new System.Drawing.Point(8, 250);
			this.listFields.Name = "listFields";
			this.listFields.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listFields.Size = new System.Drawing.Size(147, 327);
			this.listFields.TabIndex = 83;
			this.listFields.SelectionChangeCommitted += new System.EventHandler(this.listFields_SelectionChangeCommitted);
			this.listFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFields_MouseDoubleClick);
			// 
			// butPaste
			// 
			this.butPaste.Location = new System.Drawing.Point(83, 127);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(74, 24);
			this.butPaste.TabIndex = 91;
			this.butPaste.TabStop = false;
			this.butPaste.Text = "Paste";
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(7, 231);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 16);
			this.label2.TabIndex = 84;
			this.label2.Text = "Fields";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCopy
			// 
			this.butCopy.Location = new System.Drawing.Point(5, 127);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(74, 24);
			this.butCopy.TabIndex = 90;
			this.butCopy.TabStop = false;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(5, 27);
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
			this.panelLeft.Size = new System.Drawing.Size(663, 794);
			this.panelLeft.TabIndex = 102;
			// 
			// panelRight
			// 
			this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRight.Controls.Add(this.butRedo);
			this.panelRight.Controls.Add(this.butUndo);
			this.panelRight.Controls.Add(this.butAlignTop);
			this.panelRight.Controls.Add(this.butAddField);
			this.panelRight.Controls.Add(this.butCancel);
			this.panelRight.Controls.Add(this.butOK);
			this.panelRight.Controls.Add(this.groupBoxSubViews);
			this.panelRight.Controls.Add(this.butAutoSnap);
			this.panelRight.Controls.Add(this.butMobile);
			this.panelRight.Controls.Add(this.groupAlignH);
			this.panelRight.Controls.Add(this.textDescription);
			this.panelRight.Controls.Add(this.groupPage);
			this.panelRight.Controls.Add(this.butDelete);
			this.panelRight.Controls.Add(this.butTabOrder);
			this.panelRight.Controls.Add(this.labelInternal);
			this.panelRight.Controls.Add(this.linkLabelTips);
			this.panelRight.Controls.Add(this.listFields);
			this.panelRight.Controls.Add(this.butPaste);
			this.panelRight.Controls.Add(this.label2);
			this.panelRight.Controls.Add(this.butCopy);
			this.panelRight.Controls.Add(this.butEdit);
			this.panelRight.Location = new System.Drawing.Point(663, 0);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(165, 794);
			this.panelRight.TabIndex = 103;
			// 
			// butUndo
			// 
			this.butUndo.Location = new System.Drawing.Point(5, 102);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(74, 24);
			this.butUndo.TabIndex = 102;
			this.butUndo.TabStop = false;
			this.butUndo.Text = "Undo Ctrl-Z";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// butRedo
			// 
			this.butRedo.Location = new System.Drawing.Point(83, 102);
			this.butRedo.Name = "butRedo";
			this.butRedo.Size = new System.Drawing.Size(74, 24);
			this.butRedo.TabIndex = 103;
			this.butRedo.TabStop = false;
			this.butRedo.Text = "Redo Ctrl-Y";
			this.butRedo.Click += new System.EventHandler(this.butRedo_Click);
			// 
			// FormSheetDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(828, 794);
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
		private UI.Button butAddField;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Panel panelRight;
		private UI.Button butUndo;
		private UI.Button butRedo;
	}
}