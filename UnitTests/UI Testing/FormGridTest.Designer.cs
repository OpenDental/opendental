namespace UnitTests
{
	partial class FormGridTest
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
			this.gridOld = new OpenDental.UI.GridOld();
			this.butFillOld = new OpenDental.UI.Button();
			this.labelRowsOld = new System.Windows.Forms.Label();
			this.labelTimeOld = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelTimeNew = new System.Windows.Forms.Label();
			this.labelRowsNew = new System.Windows.Forms.Label();
			this.butFillNew = new OpenDental.UI.Button();
			this.gridNew = new OpenDental.UI.GridOD();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.listBoxOD = new OpenDental.UI.ListBox();
			this.gridProgPageNav = new OpenDental.UI.ODGridPageNav();
			this.butEnd = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butScroll = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridOld
			// 
			this.gridOld.HasDropDowns = true;
			this.gridOld.HScrollVisible = true;
			this.gridOld.Location = new System.Drawing.Point(12, 39);
			this.gridOld.Name = "gridOld";
			this.gridOld.SelectionMode = OpenDental.UI.GridSelectionMode.OneRow;
			this.gridOld.ShowContextMenu = false;
			this.gridOld.Size = new System.Drawing.Size(436, 518);
			this.gridOld.TabIndex = 2;
			this.gridOld.Title = "Old Grid";
			this.gridOld.TranslationName = "test";
			// 
			// butFillOld
			// 
			this.butFillOld.Location = new System.Drawing.Point(465, 39);
			this.butFillOld.Name = "butFillOld";
			this.butFillOld.Size = new System.Drawing.Size(75, 24);
			this.butFillOld.TabIndex = 71;
			this.butFillOld.Text = "Fill";
			this.butFillOld.UseVisualStyleBackColor = true;
			this.butFillOld.Click += new System.EventHandler(this.butFillOld_Click);
			// 
			// labelRowsOld
			// 
			this.labelRowsOld.Location = new System.Drawing.Point(462, 79);
			this.labelRowsOld.Name = "labelRowsOld";
			this.labelRowsOld.Size = new System.Drawing.Size(100, 18);
			this.labelRowsOld.TabIndex = 72;
			this.labelRowsOld.Text = "Rows:";
			// 
			// labelTimeOld
			// 
			this.labelTimeOld.Location = new System.Drawing.Point(462, 97);
			this.labelTimeOld.Name = "labelTimeOld";
			this.labelTimeOld.Size = new System.Drawing.Size(100, 18);
			this.labelTimeOld.TabIndex = 73;
			this.labelTimeOld.Text = "Time:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(201, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 74;
			this.label1.Text = "Old Grid";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(782, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 79;
			this.label2.Text = "New Grid";
			// 
			// labelTimeNew
			// 
			this.labelTimeNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTimeNew.Location = new System.Drawing.Point(1029, 97);
			this.labelTimeNew.Name = "labelTimeNew";
			this.labelTimeNew.Size = new System.Drawing.Size(100, 18);
			this.labelTimeNew.TabIndex = 78;
			this.labelTimeNew.Text = "Time:";
			// 
			// labelRowsNew
			// 
			this.labelRowsNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRowsNew.Location = new System.Drawing.Point(1029, 79);
			this.labelRowsNew.Name = "labelRowsNew";
			this.labelRowsNew.Size = new System.Drawing.Size(100, 18);
			this.labelRowsNew.TabIndex = 77;
			this.labelRowsNew.Text = "Rows:";
			// 
			// butFillNew
			// 
			this.butFillNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butFillNew.Location = new System.Drawing.Point(1032, 39);
			this.butFillNew.Name = "butFillNew";
			this.butFillNew.Size = new System.Drawing.Size(75, 24);
			this.butFillNew.TabIndex = 76;
			this.butFillNew.Text = "Fill";
			this.butFillNew.UseVisualStyleBackColor = true;
			this.butFillNew.Click += new System.EventHandler(this.butFillNew_Click);
			// 
			// gridNew
			// 
			this.gridNew.AllowSortingByColumn = true;
			this.gridNew.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridNew.Location = new System.Drawing.Point(579, 39);
			this.gridNew.Name = "gridNew";
			this.gridNew.Size = new System.Drawing.Size(436, 518);
			this.gridNew.TabIndex = 75;
			this.gridNew.Title = "New Grid";
			this.gridNew.TranslationName = "test";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(1032, 307);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 80;
			this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
			// 
			// listBoxOD
			// 
			this.listBoxOD.Location = new System.Drawing.Point(1032, 353);
			this.listBoxOD.Name = "listBoxOD";
			this.listBoxOD.Size = new System.Drawing.Size(120, 134);
			this.listBoxOD.TabIndex = 81;
			// 
			// gridProgPageNav
			// 
			this.gridProgPageNav.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProgPageNav.BackColor = System.Drawing.Color.Transparent;
			this.gridProgPageNav.GridOld = this.gridOld;
			this.gridProgPageNav.Location = new System.Drawing.Point(12, 560);
			this.gridProgPageNav.MinimumSize = new System.Drawing.Size(143, 26);
			this.gridProgPageNav.Name = "gridProgPageNav";
			this.gridProgPageNav.Size = new System.Drawing.Size(436, 26);
			this.gridProgPageNav.TabIndex = 219;
			// 
			// butEnd
			// 
			this.butEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEnd.Location = new System.Drawing.Point(1032, 141);
			this.butEnd.Name = "butEnd";
			this.butEnd.Size = new System.Drawing.Size(75, 24);
			this.butEnd.TabIndex = 220;
			this.butEnd.Text = "End";
			this.butEnd.UseVisualStyleBackColor = true;
			this.butEnd.Click += new System.EventHandler(this.butEnd_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Location = new System.Drawing.Point(940, 563);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 221;
			this.butPrint.Text = "Print";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butScroll
			// 
			this.butScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butScroll.Location = new System.Drawing.Point(1032, 194);
			this.butScroll.Name = "butScroll";
			this.butScroll.Size = new System.Drawing.Size(75, 24);
			this.butScroll.TabIndex = 222;
			this.butScroll.Text = "Scroll";
			this.butScroll.UseVisualStyleBackColor = true;
			this.butScroll.Click += new System.EventHandler(this.butScroll_Click);
			// 
			// FormGridTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1159, 592);
			this.Controls.Add(this.butScroll);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butEnd);
			this.Controls.Add(this.gridProgPageNav);
			this.Controls.Add(this.listBoxOD);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelTimeNew);
			this.Controls.Add(this.labelRowsNew);
			this.Controls.Add(this.butFillNew);
			this.Controls.Add(this.gridNew);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelTimeOld);
			this.Controls.Add(this.labelRowsOld);
			this.Controls.Add(this.butFillOld);
			this.Controls.Add(this.gridOld);
			this.Name = "FormGridTest";
			this.Text = "FormGridTest";
			this.Load += new System.EventHandler(this.FormGridTest_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOld gridOld;
		private OpenDental.UI.Button butFillOld;
		private System.Windows.Forms.Label labelRowsOld;
		private System.Windows.Forms.Label labelTimeOld;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelTimeNew;
		private System.Windows.Forms.Label labelRowsNew;
		private OpenDental.UI.Button butFillNew;
		private OpenDental.UI.GridOD gridNew;
		private System.Windows.Forms.TextBox textBox1;
		private OpenDental.UI.ListBox listBoxOD;
		private OpenDental.UI.ODGridPageNav gridProgPageNav;
		private OpenDental.UI.Button butEnd;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butScroll;
	}
}