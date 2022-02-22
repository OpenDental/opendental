using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMountLayoutEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMountLayoutEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.textHeight = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.panelSplitter = new System.Windows.Forms.Panel();
			this.groupBox1 = new OpenDental.UI.GroupBoxOD();
			this.labelReorder = new System.Windows.Forms.Label();
			this.butReorder = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.controlDrawing = new OpenDental.UI.ControlDoubleBuffered();
			this.panelSplitter.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1151, 666);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1151, 634);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(1102, 25);
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
			this.textWidth.Location = new System.Drawing.Point(1157, 25);
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
			this.textHeight.Location = new System.Drawing.Point(1157, 46);
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
			this.label4.Location = new System.Drawing.Point(1102, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 17);
			this.label4.TabIndex = 28;
			this.label4.Text = "Height";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// panelSplitter
			// 
			this.panelSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelSplitter.Controls.Add(this.groupBox1);
			this.panelSplitter.Controls.Add(this.butAdd);
			this.panelSplitter.Controls.Add(this.label5);
			this.panelSplitter.Controls.Add(this.labelWarning);
			this.panelSplitter.Location = new System.Drawing.Point(1022, 0);
			this.panelSplitter.Name = "panelSplitter";
			this.panelSplitter.Size = new System.Drawing.Size(208, 696);
			this.panelSplitter.TabIndex = 31;
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
			this.groupBox1.Location = new System.Drawing.Point(5, 290);
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
			this.butAdd.Location = new System.Drawing.Point(18, 251);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(81, 24);
			this.butAdd.TabIndex = 39;
			this.butAdd.Text = "Add Item";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(6, 603);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(122, 49);
			this.label5.TabIndex = 43;
			this.label5.Text = "Items get saved as they are added, not when clicking OK here";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWarning.ForeColor = System.Drawing.Color.Firebrick;
			this.labelWarning.Location = new System.Drawing.Point(3, 433);
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
			this.controlDrawing.Location = new System.Drawing.Point(0, 0);
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
			// FormMountLayoutEdit
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.controlDrawing);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.panelSplitter);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = System.Windows.Forms.ImeMode.On;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMountLayoutEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Mount Layout";
			this.Load += new System.EventHandler(this.FormMountLayoutEdit_Load);
			this.SizeChanged += new System.EventHandler(this.FormMountDefEdit_SizeChanged);
			this.panelSplitter.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label3;
		private ValidNum textWidth;
		private ValidNum textHeight;
		private Label label4;
		private Panel panelSplitter;
		private UI.Button butDown;
		private UI.Button butUp;
		private OpenDental.UI.GroupBoxOD groupBox1;
		private Label labelWarning;
		private Label label5;
		private Label label7;
		private Label label6;
		private UI.ControlDoubleBuffered controlDrawing;
		private Label labelReorder;
		private UI.Button butReorder;
		private UI.Button butAdd;
	}
}
