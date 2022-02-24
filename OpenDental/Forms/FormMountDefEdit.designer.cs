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
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.textHeight = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.butGenerate = new OpenDental.UI.Button();
			this.panelSplitter = new System.Windows.Forms.Panel();
			this.butColor = new System.Windows.Forms.Button();
			this.labelColor = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.controlDrawing = new OpenDental.UI.ControlDoubleBuffered();
			this.butReorder = new OpenDental.UI.Button();
			this.labelReorder = new System.Windows.Forms.Label();
			this.panelSplitter.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(909, 634);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(909, 602);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(822, 634);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(825, 27);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(155, 20);
			this.textDescription.TabIndex = 10;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(822, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 17);
			this.label1.TabIndex = 11;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(848, 50);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 17);
			this.label3.TabIndex = 14;
			this.label3.Text = "Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textWidth
			// 
			this.textWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textWidth.Location = new System.Drawing.Point(903, 50);
			this.textWidth.MaxVal = 20000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(48, 20);
			this.textWidth.TabIndex = 27;
			this.textWidth.TextChanged += new System.EventHandler(this.textWidth_TextChanged);
			// 
			// textHeight
			// 
			this.textHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textHeight.Location = new System.Drawing.Point(903, 73);
			this.textHeight.MaxVal = 10000;
			this.textHeight.MinVal = 1;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(48, 20);
			this.textHeight.TabIndex = 29;
			this.textHeight.TextChanged += new System.EventHandler(this.textHeight_TextChanged);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(848, 73);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 17);
			this.label4.TabIndex = 28;
			this.label4.Text = "Height";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butGenerate
			// 
			this.butGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butGenerate.Location = new System.Drawing.Point(825, 129);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(65, 24);
			this.butGenerate.TabIndex = 30;
			this.butGenerate.Text = "Generate";
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// panelSplitter
			// 
			this.panelSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelSplitter.Controls.Add(this.butColor);
			this.panelSplitter.Controls.Add(this.labelColor);
			this.panelSplitter.Controls.Add(this.label5);
			this.panelSplitter.Controls.Add(this.labelWarning);
			this.panelSplitter.Location = new System.Drawing.Point(816, 0);
			this.panelSplitter.Name = "panelSplitter";
			this.panelSplitter.Size = new System.Drawing.Size(172, 672);
			this.panelSplitter.TabIndex = 31;
			// 
			// butColor
			// 
			this.butColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(86, 95);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 44;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// labelColor
			// 
			this.labelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelColor.Location = new System.Drawing.Point(14, 97);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(69, 16);
			this.labelColor.TabIndex = 45;
			this.labelColor.Text = "Back Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(4, 556);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(138, 42);
			this.label5.TabIndex = 43;
			this.label5.Text = "Items get saved as they are added, not when clicking OK here";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWarning.ForeColor = System.Drawing.Color.Firebrick;
			this.labelWarning.Location = new System.Drawing.Point(2, 382);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(167, 69);
			this.labelWarning.TabIndex = 43;
			this.labelWarning.Text = "Warning!  At least one item is not showing because it\'s outside the bounds of the" +
    " Mount.  Consider enlarging your mount to find it.";
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWarning.Visible = false;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(893, 123);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 34);
			this.label2.TabIndex = 32;
			this.label2.Text = "(start an entirely new layout)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(825, 168);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(81, 24);
			this.butAdd.TabIndex = 39;
			this.butAdd.Text = "Add Item";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
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
			this.groupBox1.Location = new System.Drawing.Point(822, 204);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(142, 155);
			this.groupBox1.TabIndex = 42;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Item Order";
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
			// controlDrawing
			// 
			this.controlDrawing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controlDrawing.Location = new System.Drawing.Point(0, 0);
			this.controlDrawing.Name = "controlDrawing";
			this.controlDrawing.Size = new System.Drawing.Size(815, 670);
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
			// labelReorder
			// 
			this.labelReorder.Location = new System.Drawing.Point(7, 108);
			this.labelReorder.Name = "labelReorder";
			this.labelReorder.Size = new System.Drawing.Size(128, 42);
			this.labelReorder.TabIndex = 46;
			this.labelReorder.Text = "This lets you reorder all items by clicking through them in sequence";
			// 
			// FormMountDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(988, 672);
			this.Controls.Add(this.controlDrawing);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.panelSplitter);
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
			this.panelSplitter.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private TextBox textDescription;
		private Label label1;
		private Label label3;
		private ValidNum textWidth;
		private ValidNum textHeight;
		private Label label4;
		private UI.Button butGenerate;
		private Panel panelSplitter;
		private Label label2;
		private UI.Button butDown;
		private UI.Button butUp;
		private GroupBox groupBox1;
		private Label labelWarning;
		private Label label5;
		private Label label7;
		private Label label6;
		private Button butColor;
		private Label labelColor;
		private UI.ControlDoubleBuffered controlDrawing;
		private Label labelReorder;
		private UI.Button butReorder;
		private UI.Button butAdd;
	}
}
