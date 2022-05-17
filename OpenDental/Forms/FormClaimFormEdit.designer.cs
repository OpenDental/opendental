using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimFormEdit {
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimFormEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.panelClaimForm = new System.Windows.Forms.PictureBox();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listItems = new System.Windows.Forms.ListBox();
			this.butAdd = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textXPos = new System.Windows.Forms.TextBox();
			this.textYPos = new System.Windows.Forms.TextBox();
			this.textWidth = new System.Windows.Forms.TextBox();
			this.textHeight = new System.Windows.Forms.TextBox();
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.butFont = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.checkPrintImages = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textOffsetX = new OpenDental.ValidNum();
			this.textOffsetY = new OpenDental.ValidNum();
			this.labelInternal = new System.Windows.Forms.Label();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textFormHeight = new OpenDental.ValidNum();
			this.textFormWidth = new OpenDental.ValidNum();
			this.panelControls = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.panelClaimForm)).BeginInit();
			this.panelControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(25, 716);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(25, 686);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// panelClaimForm
			// 
			this.panelClaimForm.BackColor = System.Drawing.Color.White;
			this.panelClaimForm.Location = new System.Drawing.Point(0, 0);
			this.panelClaimForm.Name = "panelClaimForm";
			this.panelClaimForm.Size = new System.Drawing.Size(850, 1100);
			this.panelClaimForm.TabIndex = 2;
			this.panelClaimForm.TabStop = false;
			this.panelClaimForm.Paint += new System.Windows.Forms.PaintEventHandler(this.panelClaimForm_Paint);
			this.panelClaimForm.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelClaimForm_MouseDown);
			this.panelClaimForm.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelClaimForm_MouseMove);
			this.panelClaimForm.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelClaimForm_MouseUp);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point(850, 0);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 650);
			this.vScrollBar1.TabIndex = 3;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBars_Scroll);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(3, 21);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(119, 20);
			this.textDescription.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(119, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHidden.Location = new System.Drawing.Point(3, 88);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(119, 17);
			this.checkIsHidden.TabIndex = 6;
			this.checkIsHidden.Text = "Is Hidden";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 205);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 17);
			this.label2.TabIndex = 7;
			this.label2.Text = "Items:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listItems
			// 
			this.listItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listItems.Location = new System.Drawing.Point(3, 223);
			this.listItems.MultiColumn = true;
			this.listItems.Name = "listItems";
			this.listItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listItems.Size = new System.Drawing.Size(119, 329);
			this.listItems.TabIndex = 8;
			this.listItems.DoubleClick += new System.EventHandler(this.listItems_DoubleClick);
			this.listItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listItems_MouseUp);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Location = new System.Drawing.Point(85, 201);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(37, 20);
			this.butAdd.TabIndex = 9;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(3, 556);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 17);
			this.label4.TabIndex = 10;
			this.label4.Text = "X Pos";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(3, 578);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 17);
			this.label5.TabIndex = 11;
			this.label5.Text = "Y Pos";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(3, 600);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(52, 17);
			this.label6.TabIndex = 12;
			this.label6.Text = "Width";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(3, 622);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(52, 17);
			this.label7.TabIndex = 13;
			this.label7.Text = "Height";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textXPos
			// 
			this.textXPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textXPos.Location = new System.Drawing.Point(56, 554);
			this.textXPos.Name = "textXPos";
			this.textXPos.Size = new System.Drawing.Size(66, 20);
			this.textXPos.TabIndex = 14;
			this.textXPos.Validated += new System.EventHandler(this.textXPos_Validated);
			// 
			// textYPos
			// 
			this.textYPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textYPos.Location = new System.Drawing.Point(56, 576);
			this.textYPos.Name = "textYPos";
			this.textYPos.Size = new System.Drawing.Size(66, 20);
			this.textYPos.TabIndex = 15;
			this.textYPos.Validated += new System.EventHandler(this.textYPos_Validated);
			// 
			// textWidth
			// 
			this.textWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textWidth.Location = new System.Drawing.Point(56, 598);
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(66, 20);
			this.textWidth.TabIndex = 16;
			this.textWidth.Validated += new System.EventHandler(this.textWidth_Validated);
			// 
			// textHeight
			// 
			this.textHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textHeight.Location = new System.Drawing.Point(56, 620);
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(66, 20);
			this.textHeight.TabIndex = 17;
			this.textHeight.Validated += new System.EventHandler(this.textHeight_Validated);
			// 
			// fontDialog1
			// 
			this.fontDialog1.MaxSize = 25;
			this.fontDialog1.MinSize = 5;
			this.fontDialog1.ShowEffects = false;
			// 
			// butFont
			// 
			this.butFont.Location = new System.Drawing.Point(3, 129);
			this.butFont.Name = "butFont";
			this.butFont.Size = new System.Drawing.Size(119, 21);
			this.butFont.TabIndex = 20;
			this.butFont.Text = "&Font";
			this.butFont.Click += new System.EventHandler(this.butFont_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Location = new System.Drawing.Point(25, 656);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 22;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// checkPrintImages
			// 
			this.checkPrintImages.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPrintImages.Location = new System.Drawing.Point(3, 108);
			this.checkPrintImages.Name = "checkPrintImages";
			this.checkPrintImages.Size = new System.Drawing.Size(119, 17);
			this.checkPrintImages.TabIndex = 25;
			this.checkPrintImages.Text = "Print Images";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 156);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 17);
			this.label3.TabIndex = 26;
			this.label3.Text = "Offset X";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 178);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(52, 17);
			this.label8.TabIndex = 28;
			this.label8.Text = "Offset Y";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOffsetX
			// 
			this.textOffsetX.Location = new System.Drawing.Point(56, 154);
			this.textOffsetX.MaxVal = 255;
			this.textOffsetX.MinVal = -9999;
			this.textOffsetX.Name = "textOffsetX";
			this.textOffsetX.Size = new System.Drawing.Size(66, 20);
			this.textOffsetX.TabIndex = 30;
			// 
			// textOffsetY
			// 
			this.textOffsetY.Location = new System.Drawing.Point(56, 176);
			this.textOffsetY.MaxVal = 255;
			this.textOffsetY.MinVal = -9999;
			this.textOffsetY.Name = "textOffsetY";
			this.textOffsetY.Size = new System.Drawing.Size(66, 20);
			this.textOffsetY.TabIndex = 31;
			// 
			// labelInternal
			// 
			this.labelInternal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInternal.Location = new System.Drawing.Point(3, 645);
			this.labelInternal.Name = "labelInternal";
			this.labelInternal.Size = new System.Drawing.Size(119, 66);
			this.labelInternal.TabIndex = 32;
			this.labelInternal.Text = "This is an internal form. \r\nTo make changes, copy it over to a custom form.";
			this.labelInternal.Visible = false;
			// 
			// hScrollBar
			// 
			this.hScrollBar.Location = new System.Drawing.Point(0, 676);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(850, 17);
			this.hScrollBar.TabIndex = 33;
			this.hScrollBar.Visible = false;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBars_Scroll);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(3, 45);
			this.label9.Name = "label9";
			this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label9.Size = new System.Drawing.Size(52, 17);
			this.label9.TabIndex = 36;
			this.label9.Text = "Width";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(3, 67);
			this.label10.Name = "label10";
			this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label10.Size = new System.Drawing.Size(52, 17);
			this.label10.TabIndex = 37;
			this.label10.Text = "Height";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFormHeight
			// 
			this.textFormHeight.Location = new System.Drawing.Point(56, 65);
			this.textFormHeight.MaxVal = 4000;
			this.textFormHeight.MinVal = 100;
			this.textFormHeight.Name = "textFormHeight";
			this.textFormHeight.Size = new System.Drawing.Size(66, 20);
			this.textFormHeight.TabIndex = 38;
			this.textFormHeight.Validated += new System.EventHandler(this.textFormHeight_Validated);
			// 
			// textFormWidth
			// 
			this.textFormWidth.Location = new System.Drawing.Point(56, 43);
			this.textFormWidth.MaxVal = 4000;
			this.textFormWidth.MinVal = 100;
			this.textFormWidth.Name = "textFormWidth";
			this.textFormWidth.Size = new System.Drawing.Size(66, 20);
			this.textFormWidth.TabIndex = 39;
			this.textFormWidth.Validated += new System.EventHandler(this.textFormWidth_Validated);
			// 
			// panelControls
			// 
			this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelControls.Controls.Add(this.textDescription);
			this.panelControls.Controls.Add(this.butAdd);
			this.panelControls.Controls.Add(this.listItems);
			this.panelControls.Controls.Add(this.textFormWidth);
			this.panelControls.Controls.Add(this.textFormHeight);
			this.panelControls.Controls.Add(this.label1);
			this.panelControls.Controls.Add(this.textOffsetY);
			this.panelControls.Controls.Add(this.checkIsHidden);
			this.panelControls.Controls.Add(this.textOffsetX);
			this.panelControls.Controls.Add(this.label2);
			this.panelControls.Controls.Add(this.label4);
			this.panelControls.Controls.Add(this.textHeight);
			this.panelControls.Controls.Add(this.label5);
			this.panelControls.Controls.Add(this.textWidth);
			this.panelControls.Controls.Add(this.label6);
			this.panelControls.Controls.Add(this.textYPos);
			this.panelControls.Controls.Add(this.label7);
			this.panelControls.Controls.Add(this.textXPos);
			this.panelControls.Controls.Add(this.checkPrintImages);
			this.panelControls.Controls.Add(this.butPrint);
			this.panelControls.Controls.Add(this.label3);
			this.panelControls.Controls.Add(this.butFont);
			this.panelControls.Controls.Add(this.label8);
			this.panelControls.Controls.Add(this.butCancel);
			this.panelControls.Controls.Add(this.butOK);
			this.panelControls.Controls.Add(this.labelInternal);
			this.panelControls.Controls.Add(this.label10);
			this.panelControls.Controls.Add(this.label9);
			this.panelControls.Location = new System.Drawing.Point(867, 0);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(125, 745);
			this.panelControls.TabIndex = 41;
			// 
			// FormClaimFormEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(992, 745);
			this.Controls.Add(this.panelControls);
			this.Controls.Add(this.hScrollBar);
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.panelClaimForm);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimFormEdit";
			this.ShowInTaskbar = false;
			this.Text = "Claim Form Edit";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClaimFormEdit_Closing);
			this.Load += new System.EventHandler(this.FormClaimFormEdit_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormClaimFormEdit_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormClaimFormEdit_KeyUp);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormClaimFormEdit_Layout);
			((System.ComponentModel.ISupportInitialize)(this.panelClaimForm)).EndInit();
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.PictureBox panelClaimForm;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkIsHidden;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listItems;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textXPos;
		private System.Windows.Forms.TextBox textYPos;
		private System.Windows.Forms.TextBox textWidth;
		private System.Windows.Forms.TextBox textHeight;
		private System.Windows.Forms.FontDialog fontDialog1;
		private OpenDental.UI.Button butFont;
		private OpenDental.UI.Button butPrint;
		private Label label9;
		private Label label10;
		private ValidNum textFormHeight;
		private ValidNum textFormWidth;
		private System.Windows.Forms.CheckBox checkPrintImages;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private OpenDental.ValidNum textOffsetX;
		private OpenDental.ValidNum textOffsetY;
		private Label labelInternal;
		private Panel panelControls;
	}
}
