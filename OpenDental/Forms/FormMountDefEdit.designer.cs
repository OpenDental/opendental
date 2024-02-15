using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMountDefEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMountDefEdit));
			this.butSave = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.textHeight = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.butGenerate = new OpenDental.UI.Button();
			this.panelRight = new System.Windows.Forms.Panel();
			this.checkAdjModeAfterSeries = new OpenDental.UI.CheckBox();
			this.checkFlipOnAcquire = new OpenDental.UI.CheckBox();
			this.comboDefaultCat = new OpenDental.UI.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.textDecimals = new OpenDental.ValidNum();
			this.textScale = new OpenDental.ValidDouble();
			this.textUnits = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.labelReorder = new System.Windows.Forms.Label();
			this.butReorder = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butColorTextBack = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.butColorFore = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.butColorBack = new System.Windows.Forms.Button();
			this.labelColor = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.controlDrawing = new OpenDental.UI.ControlDoubleBuffered();
			this.checkTransparent = new OpenDental.UI.CheckBox();
			this.panelRight.SuspendLayout();
			this.groupBoxOD1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(126, 665);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(7, 665);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(1072, 4);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(155, 20);
			this.textDescription.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(1, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 17);
			this.label1.TabIndex = 11;
			this.label1.Text = "Descript";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(1102, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 17);
			this.label3.TabIndex = 14;
			this.label3.Text = "Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textWidth
			// 
			this.textWidth.AcceptsReturn = true;
			this.textWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textWidth.Location = new System.Drawing.Point(1157, 48);
			this.textWidth.MaxVal = 20000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(48, 20);
			this.textWidth.TabIndex = 1;
			this.textWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textWidth_KeyDown);
			this.textWidth.Validated += new System.EventHandler(this.textWidth_Validated);
			// 
			// textHeight
			// 
			this.textHeight.AcceptsReturn = true;
			this.textHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textHeight.Location = new System.Drawing.Point(1157, 69);
			this.textHeight.MaxVal = 10000;
			this.textHeight.MinVal = 1;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(48, 20);
			this.textHeight.TabIndex = 2;
			this.textHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textHeight_KeyDown);
			this.textHeight.Validated += new System.EventHandler(this.textHeight_Validated);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(1102, 69);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 17);
			this.label4.TabIndex = 28;
			this.label4.Text = "Height";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butGenerate
			// 
			this.butGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butGenerate.Location = new System.Drawing.Point(18, 296);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(65, 24);
			this.butGenerate.TabIndex = 30;
			this.butGenerate.Text = "Generate";
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// panelRight
			// 
			this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelRight.Controls.Add(this.checkTransparent);
			this.panelRight.Controls.Add(this.checkAdjModeAfterSeries);
			this.panelRight.Controls.Add(this.checkFlipOnAcquire);
			this.panelRight.Controls.Add(this.comboDefaultCat);
			this.panelRight.Controls.Add(this.label13);
			this.panelRight.Controls.Add(this.groupBoxOD1);
			this.panelRight.Controls.Add(this.butSave);
			this.panelRight.Controls.Add(this.groupBox1);
			this.panelRight.Controls.Add(this.butAdd);
			this.panelRight.Controls.Add(this.butColorTextBack);
			this.panelRight.Controls.Add(this.label2);
			this.panelRight.Controls.Add(this.label9);
			this.panelRight.Controls.Add(this.label1);
			this.panelRight.Controls.Add(this.butGenerate);
			this.panelRight.Controls.Add(this.butDelete);
			this.panelRight.Controls.Add(this.butColorFore);
			this.panelRight.Controls.Add(this.label8);
			this.panelRight.Controls.Add(this.butColorBack);
			this.panelRight.Controls.Add(this.labelColor);
			this.panelRight.Controls.Add(this.label5);
			this.panelRight.Controls.Add(this.labelWarning);
			this.panelRight.Location = new System.Drawing.Point(1022, 0);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(208, 696);
			this.panelRight.TabIndex = 31;
			// 
			// checkAdjModeAfterSeries
			// 
			this.checkAdjModeAfterSeries.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAdjModeAfterSeries.Location = new System.Drawing.Point(18, 190);
			this.checkAdjModeAfterSeries.Name = "checkAdjModeAfterSeries";
			this.checkAdjModeAfterSeries.Size = new System.Drawing.Size(164, 17);
			this.checkAdjModeAfterSeries.TabIndex = 54;
			this.checkAdjModeAfterSeries.Text = "Use Adj Mode After Series";
			// 
			// checkFlipOnAcquire
			// 
			this.checkFlipOnAcquire.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFlipOnAcquire.Location = new System.Drawing.Point(61, 172);
			this.checkFlipOnAcquire.Name = "checkFlipOnAcquire";
			this.checkFlipOnAcquire.Size = new System.Drawing.Size(121, 17);
			this.checkFlipOnAcquire.TabIndex = 53;
			this.checkFlipOnAcquire.Text = "Flip on Acquire";
			// 
			// comboDefaultCat
			// 
			this.comboDefaultCat.Location = new System.Drawing.Point(71, 24);
			this.comboDefaultCat.Name = "comboDefaultCat";
			this.comboDefaultCat.Size = new System.Drawing.Size(133, 21);
			this.comboDefaultCat.TabIndex = 52;
			this.comboDefaultCat.Text = "comboDefaultCat";
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label13.Location = new System.Drawing.Point(1, 26);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(72, 17);
			this.label13.TabIndex = 51;
			this.label13.Text = "Default Cat";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.textDecimals);
			this.groupBoxOD1.Controls.Add(this.textScale);
			this.groupBoxOD1.Controls.Add(this.textUnits);
			this.groupBoxOD1.Controls.Add(this.label12);
			this.groupBoxOD1.Controls.Add(this.label11);
			this.groupBoxOD1.Controls.Add(this.label10);
			this.groupBoxOD1.Location = new System.Drawing.Point(5, 211);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(196, 82);
			this.groupBoxOD1.TabIndex = 50;
			this.groupBoxOD1.Text = "Measurement Scale";
			// 
			// textDecimals
			// 
			this.textDecimals.Location = new System.Drawing.Point(129, 58);
			this.textDecimals.MaxVal = 20;
			this.textDecimals.Name = "textDecimals";
			this.textDecimals.Size = new System.Drawing.Size(48, 20);
			this.textDecimals.TabIndex = 2;
			// 
			// textScale
			// 
			this.textScale.Location = new System.Drawing.Point(129, 37);
			this.textScale.MaxVal = 100000000D;
			this.textScale.MinVal = -100000000D;
			this.textScale.Name = "textScale";
			this.textScale.Size = new System.Drawing.Size(48, 20);
			this.textScale.TabIndex = 1;
			// 
			// textUnits
			// 
			this.textUnits.Location = new System.Drawing.Point(129, 16);
			this.textUnits.Name = "textUnits";
			this.textUnits.Size = new System.Drawing.Size(48, 20);
			this.textUnits.TabIndex = 0;
			this.textUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(4, 38);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(124, 17);
			this.label12.TabIndex = 46;
			this.label12.Text = "Scale, pixels per unit";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(10, 17);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(118, 17);
			this.label11.TabIndex = 45;
			this.label11.Text = "Optional Units (mm)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(28, 59);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(100, 17);
			this.label10.TabIndex = 44;
			this.label10.Text = "Decimal Places";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.labelReorder);
			this.groupBox1.Controls.Add(this.butReorder);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.butUp);
			this.groupBox1.Controls.Add(this.butDown);
			this.groupBox1.Location = new System.Drawing.Point(5, 357);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(196, 140);
			this.groupBox1.TabIndex = 42;
			this.groupBox1.Text = "Item Order";
			// 
			// labelReorder
			// 
			this.labelReorder.Location = new System.Drawing.Point(7, 108);
			this.labelReorder.Name = "labelReorder";
			this.labelReorder.Size = new System.Drawing.Size(183, 30);
			this.labelReorder.TabIndex = 46;
			this.labelReorder.Text = "This lets you reorder all items by clicking through them in sequence";
			// 
			// butReorder
			// 
			this.butReorder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReorder.Location = new System.Drawing.Point(9, 79);
			this.butReorder.Name = "butReorder";
			this.butReorder.Size = new System.Drawing.Size(78, 24);
			this.butReorder.TabIndex = 45;
			this.butReorder.Text = "Reorder All";
			this.butReorder.Click += new System.EventHandler(this.butReorder_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(76, 52);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(58, 18);
			this.label7.TabIndex = 44;
			this.label7.Text = "(higher #)";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(76, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 18);
			this.label6.TabIndex = 43;
			this.label6.Text = "(lower #)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(9, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(65, 24);
			this.butUp.TabIndex = 41;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(9, 49);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(65, 24);
			this.butDown.TabIndex = 40;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(18, 327);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(81, 24);
			this.butAdd.TabIndex = 39;
			this.butAdd.Text = "Add Item";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butColorTextBack
			// 
			this.butColorTextBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butColorTextBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorTextBack.Location = new System.Drawing.Point(152, 132);
			this.butColorTextBack.Name = "butColorTextBack";
			this.butColorTextBack.Size = new System.Drawing.Size(30, 20);
			this.butColorTextBack.TabIndex = 48;
			this.butColorTextBack.Click += new System.EventHandler(this.butColorTextBack_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(86, 292);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 34);
			this.label2.TabIndex = 32;
			this.label2.Text = "(start an entirely new layout)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.Location = new System.Drawing.Point(24, 134);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(125, 16);
			this.label9.TabIndex = 49;
			this.label9.Text = "Text Background Color";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorFore
			// 
			this.butColorFore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butColorFore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorFore.Location = new System.Drawing.Point(152, 111);
			this.butColorFore.Name = "butColorFore";
			this.butColorFore.Size = new System.Drawing.Size(30, 20);
			this.butColorFore.TabIndex = 46;
			this.butColorFore.Click += new System.EventHandler(this.butColorFore_Click);
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(24, 113);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(125, 16);
			this.label8.TabIndex = 47;
			this.label8.Text = "Text and Line Color";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorBack
			// 
			this.butColorBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butColorBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorBack.Location = new System.Drawing.Point(152, 90);
			this.butColorBack.Name = "butColorBack";
			this.butColorBack.Size = new System.Drawing.Size(30, 20);
			this.butColorBack.TabIndex = 44;
			this.butColorBack.Click += new System.EventHandler(this.butColorBack_Click);
			// 
			// labelColor
			// 
			this.labelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelColor.Location = new System.Drawing.Point(24, 92);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(125, 16);
			this.labelColor.TabIndex = 45;
			this.labelColor.Text = "Background Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(6, 603);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(122, 49);
			this.label5.TabIndex = 43;
			this.label5.Text = "Items get saved as they are changed, not when clicking Save here";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWarning.ForeColor = System.Drawing.Color.Firebrick;
			this.labelWarning.Location = new System.Drawing.Point(3, 495);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(202, 69);
			this.labelWarning.TabIndex = 43;
			this.labelWarning.Text = "Warning!  At least one item is not showing because it\'s outside the bounds of the" +
    " Mount.  Consider enlarging your mount to find it.";
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWarning.Visible = false;
			// 
			// controlDrawing
			// 
			this.controlDrawing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controlDrawing.Location = new System.Drawing.Point(1, 0);
			this.controlDrawing.Name = "controlDrawing";
			this.controlDrawing.Size = new System.Drawing.Size(1021, 694);
			this.controlDrawing.TabIndex = 43;
			this.controlDrawing.Text = "controlDoubleBuffered1";
			this.controlDrawing.Paint += new System.Windows.Forms.PaintEventHandler(this.controlDrawing_Paint);
			this.controlDrawing.KeyDown += new System.Windows.Forms.KeyEventHandler(this.controlDrawing_KeyDown);
			this.controlDrawing.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.controlDrawing_MouseDoubleClick);
			this.controlDrawing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.controlDrawing_MouseDown);
			this.controlDrawing.MouseMove += new System.Windows.Forms.MouseEventHandler(this.controlDrawing_MouseMove);
			this.controlDrawing.MouseUp += new System.Windows.Forms.MouseEventHandler(this.controlDrawing_MouseUp);
			this.controlDrawing.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.controlDrawing_PreviewKeyDown);
			// 
			// checkTransparent
			// 
			this.checkTransparent.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTransparent.Location = new System.Drawing.Point(33, 154);
			this.checkTransparent.Name = "checkTransparent";
			this.checkTransparent.Size = new System.Drawing.Size(149, 17);
			this.checkTransparent.TabIndex = 55;
			this.checkTransparent.Text = "Text BackG Transparent";
			this.checkTransparent.Click += new System.EventHandler(this.checkTransparent_Click);
			// 
			// FormMountDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.controlDrawing);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.panelRight);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = System.Windows.Forms.ImeMode.On;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMountDefEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Mount Def";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMountDefEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormMountDefEdit_Load);
			this.SizeChanged += new System.EventHandler(this.FormMountDefEdit_SizeChanged);
			this.panelRight.ResumeLayout(false);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.Button butDelete;
		private TextBox textDescription;
		private Label label1;
		private Label label3;
		private ValidNum textWidth;
		private ValidNum textHeight;
		private Label label4;
		private UI.Button butGenerate;
		private Panel panelRight;
		private Label label2;
		private UI.Button butDown;
		private UI.Button butUp;
		private OpenDental.UI.GroupBox groupBox1;
		private Label labelWarning;
		private Label label5;
		private Label label7;
		private Label label6;
		private Button butColorBack;
		private Label labelColor;
		private UI.ControlDoubleBuffered controlDrawing;
		private Label labelReorder;
		private UI.Button butReorder;
		private UI.Button butAdd;
		private Button butColorTextBack;
		private Label label9;
		private Button butColorFore;
		private Label label8;
		private Label label10;
		private UI.GroupBox groupBoxOD1;
		private Label label12;
		private Label label11;
		private TextBox textUnits;
		private ValidDouble textScale;
		private ValidNum textDecimals;
		private Label label13;
		private UI.ComboBox comboDefaultCat;
		private OpenDental.UI.CheckBox checkFlipOnAcquire;
		private OpenDental.UI.CheckBox checkAdjModeAfterSeries;
		private OpenDental.UI.CheckBox checkTransparent;
	}
}
