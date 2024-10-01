namespace UnitTests
{
	partial class FormButtonTest
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormButtonTest));
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button2 = new OpenDental.UI.Button();
			this.butDeleteProc = new OpenDental.UI.Button();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.panelRight = new System.Windows.Forms.Panel();
			this.button3 = new System.Windows.Forms.Button();
			this.elementHost = new System.Windows.Forms.Integration.ElementHost();
			this.windowingSlider = new OpenDental.UI.WindowingSlider();
			this.elementHostWindowingSlider = new System.Windows.Forms.Integration.ElementHost();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(150, 275);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(166, 247);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 157;
			this.label1.Text = "Default";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(262, 247);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 158;
			this.label2.Text = "ButtonOld";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(372, 247);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 15);
			this.label3.TabIndex = 159;
			this.label3.Text = "ButtonJ";
			// 
			// button2
			// 
			this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
			this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button2.Location = new System.Drawing.Point(252, 316);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(116, 58);
			this.button2.TabIndex = 2;
			this.button2.Text = "Delete";
			// 
			// butDeleteProc
			// 
			this.butDeleteProc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
			this.butDeleteProc.Image = ((System.Drawing.Image)(resources.GetObject("butDeleteProc.Image")));
			this.butDeleteProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteProc.Location = new System.Drawing.Point(252, 274);
			this.butDeleteProc.Name = "butDeleteProc";
			this.butDeleteProc.Size = new System.Drawing.Size(75, 24);
			this.butDeleteProc.TabIndex = 1;
			this.butDeleteProc.Text = "Delete";
			this.butDeleteProc.UseVisualStyleBackColor = false;
			this.butDeleteProc.Click += new System.EventHandler(this.ButDeleteProc_Click);
			// 
			// panelTop
			// 
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(1399, 34);
			this.panelTop.TabIndex = 162;
			// 
			// panelLeft
			// 
			this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelLeft.Location = new System.Drawing.Point(0, 34);
			this.panelLeft.Name = "panelLeft";
			this.panelLeft.Size = new System.Drawing.Size(77, 740);
			this.panelLeft.TabIndex = 163;
			// 
			// panelRight
			// 
			this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelRight.Location = new System.Drawing.Point(1322, 34);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(77, 740);
			this.panelRight.TabIndex = 164;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(446, 351);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 165;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// elementHost
			// 
			this.elementHost.Location = new System.Drawing.Point(190, 121);
			this.elementHost.Name = "elementHost";
			this.elementHost.Size = new System.Drawing.Size(231, 25);
			this.elementHost.TabIndex = 166;
			this.elementHost.Text = "elementHost1";
			this.elementHost.Child = null;
			// 
			// windowingSlider
			// 
			this.windowingSlider.Enabled = false;
			this.windowingSlider.Location = new System.Drawing.Point(550, 86);
			this.windowingSlider.MaxVal = 255;
			this.windowingSlider.MinVal = 0;
			this.windowingSlider.Name = "windowingSlider";
			this.windowingSlider.Size = new System.Drawing.Size(154, 20);
			this.windowingSlider.TabIndex = 167;
			this.windowingSlider.Text = "contrWindowingSlider1";
			// 
			// elementHostWindowingSlider
			// 
			this.elementHostWindowingSlider.Location = new System.Drawing.Point(550, 119);
			this.elementHostWindowingSlider.Name = "elementHostWindowingSlider";
			this.elementHostWindowingSlider.Size = new System.Drawing.Size(154, 20);
			this.elementHostWindowingSlider.TabIndex = 168;
			this.elementHostWindowingSlider.Text = "elementHost1";
			this.elementHostWindowingSlider.Child = null;
			// 
			// FormButtonTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1399, 774);
			this.Controls.Add(this.elementHostWindowingSlider);
			this.Controls.Add(this.windowingSlider);
			this.Controls.Add(this.elementHost);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.panelRight);
			this.Controls.Add(this.panelLeft);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.butDeleteProc);
			this.EscClosesWindow = false;
			this.Name = "FormButtonTest";
			this.Text = "FormButtonTest";
			this.Load += new System.EventHandler(this.FormButtonTest_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butDeleteProc;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button button2;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Integration.ElementHost elementHost;
		private OpenDental.UI.WindowingSlider windowingSlider;
		private System.Windows.Forms.Integration.ElementHost elementHostWindowingSlider;
	}
}