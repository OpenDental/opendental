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
			this.butSave = new OpenDental.UI.Button();
			this.pictureBoxClaimForm = new System.Windows.Forms.PictureBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIsHidden = new OpenDental.UI.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listItems = new OpenDental.UI.ListBox();
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
			this.checkPrintImages = new OpenDental.UI.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textOffsetX = new OpenDental.ValidNum();
			this.textOffsetY = new OpenDental.ValidNum();
			this.labelInternal = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textFormHeight = new OpenDental.ValidNum();
			this.textFormWidth = new OpenDental.ValidNum();
			this.panelControls = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxClaimForm)).BeginInit();
			this.panelControls.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(64, 715);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(59, 26);
			this.butSave.TabIndex = 1;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// pictureBoxClaimForm
			// 
			this.pictureBoxClaimForm.BackColor = System.Drawing.Color.White;
			this.pictureBoxClaimForm.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxClaimForm.Name = "pictureBoxClaimForm";
			this.pictureBoxClaimForm.Size = new System.Drawing.Size(735, 643);
			this.pictureBoxClaimForm.TabIndex = 2;
			this.pictureBoxClaimForm.TabStop = false;
			this.pictureBoxClaimForm.Paint += new System.Windows.Forms.PaintEventHandler(this.panelClaimForm_Paint);
			this.pictureBoxClaimForm.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxClaimForm_MouseDown);
			this.pictureBoxClaimForm.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelClaimForm_MouseMove);
			this.pictureBoxClaimForm.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelClaimForm_MouseUp);
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
			this.listItems.IntegralHeight = false;
			this.listItems.Location = new System.Drawing.Point(3, 223);
			this.listItems.Name = "listItems";
			this.listItems.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listItems.Size = new System.Drawing.Size(119, 369);
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
			this.label4.Location = new System.Drawing.Point(3, 596);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 17);
			this.label4.TabIndex = 10;
			this.label4.Text = "X Pos";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(3, 618);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 17);
			this.label5.TabIndex = 11;
			this.label5.Text = "Y Pos";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(3, 640);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(52, 17);
			this.label6.TabIndex = 12;
			this.label6.Text = "Width";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(3, 662);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(52, 17);
			this.label7.TabIndex = 13;
			this.label7.Text = "Height";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textXPos
			// 
			this.textXPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textXPos.Location = new System.Drawing.Point(56, 594);
			this.textXPos.Name = "textXPos";
			this.textXPos.Size = new System.Drawing.Size(66, 20);
			this.textXPos.TabIndex = 14;
			this.textXPos.Validated += new System.EventHandler(this.textXPos_Validated);
			// 
			// textYPos
			// 
			this.textYPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textYPos.Location = new System.Drawing.Point(56, 616);
			this.textYPos.Name = "textYPos";
			this.textYPos.Size = new System.Drawing.Size(66, 20);
			this.textYPos.TabIndex = 15;
			this.textYPos.Validated += new System.EventHandler(this.textYPos_Validated);
			// 
			// textWidth
			// 
			this.textWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textWidth.Location = new System.Drawing.Point(56, 638);
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(66, 20);
			this.textWidth.TabIndex = 16;
			this.textWidth.Validated += new System.EventHandler(this.textWidth_Validated);
			// 
			// textHeight
			// 
			this.textHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textHeight.Location = new System.Drawing.Point(56, 660);
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
			this.butPrint.Location = new System.Drawing.Point(2, 715);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(59, 26);
			this.butPrint.TabIndex = 22;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// checkPrintImages
			// 
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
			this.textOffsetX.MinVal = -9999;
			this.textOffsetX.Name = "textOffsetX";
			this.textOffsetX.Size = new System.Drawing.Size(66, 20);
			this.textOffsetX.TabIndex = 30;
			// 
			// textOffsetY
			// 
			this.textOffsetY.Location = new System.Drawing.Point(56, 176);
			this.textOffsetY.MinVal = -9999;
			this.textOffsetY.Name = "textOffsetY";
			this.textOffsetY.Size = new System.Drawing.Size(66, 20);
			this.textOffsetY.TabIndex = 31;
			// 
			// labelInternal
			// 
			this.labelInternal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInternal.Location = new System.Drawing.Point(3, 685);
			this.labelInternal.Name = "labelInternal";
			this.labelInternal.Size = new System.Drawing.Size(119, 55);
			this.labelInternal.TabIndex = 32;
			this.labelInternal.Text = "This is an internal form. \r\nTo make changes, copy it over to a custom form.";
			this.labelInternal.Visible = false;
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
			this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
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
			this.panelControls.Controls.Add(this.butSave);
			this.panelControls.Controls.Add(this.labelInternal);
			this.panelControls.Controls.Add(this.label10);
			this.panelControls.Controls.Add(this.label9);
			this.panelControls.Location = new System.Drawing.Point(867, 0);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(125, 745);
			this.panelControls.TabIndex = 41;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.panel1.Controls.Add(this.pictureBoxClaimForm);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(865, 743);
			this.panel1.TabIndex = 42;
			// 
			// FormClaimFormEdit
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(992, 745);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panelControls);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "FormClaimFormEdit";
			this.ShowInTaskbar = false;
			this.Text = "Claim Form Edit";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClaimFormEdit_Closing);
			this.Load += new System.EventHandler(this.FormClaimFormEdit_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormClaimFormEdit_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormClaimFormEdit_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxClaimForm)).EndInit();
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.PictureBox pictureBoxClaimForm;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.CheckBox checkIsHidden;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBox listItems;
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
		private OpenDental.UI.CheckBox checkPrintImages;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private OpenDental.ValidNum textOffsetX;
		private OpenDental.ValidNum textOffsetY;
		private Label labelInternal;
		private Panel panelControls;
		private Panel panel1;
	}
}
