
namespace UnitTests{
	partial class FormSplitContainerTests {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.button4 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.splitContainerOD = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainerOD.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Location = new System.Drawing.Point(60, 60);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.splitContainer1.Panel2.Controls.Add(this.panel4);
			this.splitContainer1.Size = new System.Drawing.Size(443, 301);
			this.splitContainer1.SplitterDistance = 104;
			this.splitContainer1.TabIndex = 0;
			this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(382, 75);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "location";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(46, 193);
			this.panel4.TabIndex = 0;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(229, 385);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 8;
			this.button4.Text = "Set 50";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(84, 385);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 9;
			this.button1.Text = "Set 20";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// splitContainerOD
			// 
			this.splitContainerOD.Controls.Add(this.splitterPanel1);
			this.splitContainerOD.Controls.Add(this.splitterPanel2);
			this.splitContainerOD.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainerOD.Location = new System.Drawing.Point(598, 60);
			this.splitContainerOD.Name = "splitContainerOD";
			this.splitContainerOD.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainerOD.Panel1 = this.splitterPanel1;
			this.splitContainerOD.Panel1MinSize = 20;
			this.splitContainerOD.Panel2 = this.splitterPanel2;
			this.splitContainerOD.Panel2MinSize = 100;
			this.splitContainerOD.Size = new System.Drawing.Size(443, 301);
			this.splitContainerOD.SplitterDistance = 145;
			this.splitContainerOD.TabIndex = 10;
			this.splitContainerOD.SplitterMoved += new System.EventHandler(this.splitContainerOD_SplitterMoved);
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(443, 145);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Location = new System.Drawing.Point(0, 149);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(443, 152);
			this.splitterPanel2.TabIndex = 14;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(692, 385);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 12;
			this.button2.Text = "Set 60pix";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(837, 385);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 11;
			this.button3.Text = "Set 150pix";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// FormSplitContainerTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1115, 530);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.splitContainerOD);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FormSplitContainerTests";
			this.Text = "FormSplitContainer";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainerOD.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel4;
		private OpenDental.UI.SplitContainer splitContainerOD;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private OpenDental.UI.SplitterPanel splitterPanel2;
		private OpenDental.UI.SplitterPanel splitterPanel1;
	}
}