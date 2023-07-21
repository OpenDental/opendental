namespace UnitTests
{
	partial class FormLayoutManagerTests
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
			if(disposing && (components != null))
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
			this.button2 = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox3 = new OpenDental.UI.GroupBox();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.checkBox6 = new OpenDental.UI.CheckBox();
			this.button3 = new OpenDental.UI.Button();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.listBox1 = new OpenDental.UI.ListBox();
			this.comboBox1 = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tabControl = new OpenDental.UI.TabControl();
			this.tabPage1 = new OpenDental.UI.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.checkBox1 = new OpenDental.UI.CheckBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.tabPage2 = new OpenDental.UI.TabPage();
			this.panel4 = new System.Windows.Forms.Panel();
			this.checkBox2 = new OpenDental.UI.CheckBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.button1 = new OpenDental.UI.Button();
			this.button4 = new OpenDental.UI.Button();
			this.groupBox3.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// button2
			// 
			this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button2.Location = new System.Drawing.Point(321, 68);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 33);
			this.button2.TabIndex = 5;
			this.button2.Text = "&Set 3 selected";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(648, 546);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "Here\'s multiline";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.Location = new System.Drawing.Point(1, 1);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(140, 39);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "text in box";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioButton3);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.radioButton4);
			this.groupBox3.Location = new System.Drawing.Point(213, 157);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(256, 110);
			this.groupBox3.TabIndex = 58;
			this.groupBox3.Text = "My Group Box";
			// 
			// radioButton3
			// 
			this.radioButton3.Location = new System.Drawing.Point(94, 80);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(115, 20);
			this.radioButton3.TabIndex = 65;
			this.radioButton3.TabStop = true;
			this.radioButton3.Text = "radioButton3";
			this.radioButton3.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(31, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(127, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "Label in my groupBox";
			// 
			// radioButton4
			// 
			this.radioButton4.Location = new System.Drawing.Point(94, 54);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(115, 20);
			this.radioButton4.TabIndex = 64;
			this.radioButton4.TabStop = true;
			this.radioButton4.Text = "radioButton4";
			this.radioButton4.UseVisualStyleBackColor = true;
			// 
			// checkBox6
			// 
			this.checkBox6.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBox6.Checked = true;
			this.checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox6.Location = new System.Drawing.Point(243, 113);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new System.Drawing.Size(162, 33);
			this.checkBox6.TabIndex = 60;
			this.checkBox6.Text = "CheckBox6 with lots of text that wraps";
			this.checkBox6.Click += new System.EventHandler(this.checkBox6_Click);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button3.Location = new System.Drawing.Point(869, 621);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(87, 24);
			this.button3.TabIndex = 61;
			this.button3.Text = "Cancel";
			this.button3.UseVisualStyleBackColor = false;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// radioButton1
			// 
			this.radioButton1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButton1.Checked = true;
			this.radioButton1.Location = new System.Drawing.Point(447, 105);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(115, 20);
			this.radioButton1.TabIndex = 62;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "radioButton1";
			this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButton2.Location = new System.Drawing.Point(447, 131);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(115, 20);
			this.radioButton2.TabIndex = 63;
			this.radioButton2.Text = "radioButton2";
			this.radioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// listBox1
			// 
			this.listBox1.ItemStrings = new string[] {
        "First",
        "Second",
        "Third",
        "Fourth",
        "Fifth",
        "Sixth",
        "Seventh"};
			this.listBox1.Location = new System.Drawing.Point(506, 157);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(120, 82);
			this.listBox1.TabIndex = 64;
			// 
			// comboBox1
			// 
			this.comboBox1.BackColor = System.Drawing.SystemColors.Window;
			this.comboBox1.Location = new System.Drawing.Point(329, 39);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(160, 21);
			this.comboBox1.TabIndex = 73;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(226, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 18);
			this.label3.TabIndex = 2;
			this.label3.Text = "ComboBox";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControl.Location = new System.Drawing.Point(644, 48);
			this.tabControl.Name = "tabControl";
			this.tabControl.Size = new System.Drawing.Size(189, 139);
			this.tabControl.TabIndex = 76;
			// 
			// tabPage1
			// 
			this.tabPage1.AutoScroll = true;
			this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage1.Controls.Add(this.panel3);
			this.tabPage1.Location = new System.Drawing.Point(2, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(185, 116);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			// 
			// panel3
			// 
			this.panel3.AutoScroll = true;
			this.panel3.Controls.Add(this.checkBox1);
			this.panel3.Controls.Add(this.textBox2);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(179, 110);
			this.panel3.TabIndex = 79;
			// 
			// checkBox1
			// 
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Location = new System.Drawing.Point(20, 41);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(96, 18);
			this.checkBox1.TabIndex = 61;
			this.checkBox1.Text = "checkBox1";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(54, 15);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 20);
			this.textBox2.TabIndex = 62;
			this.textBox2.Text = "text in box";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage2.Location = new System.Drawing.Point(2, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(185, 116);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			// 
			// panel4
			// 
			this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel4.AutoScroll = true;
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.checkBox2);
			this.panel4.Location = new System.Drawing.Point(644, 328);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(229, 151);
			this.panel4.TabIndex = 80;
			// 
			// checkBox2
			// 
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Location = new System.Drawing.Point(20, 41);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(162, 18);
			this.checkBox2.TabIndex = 61;
			this.checkBox2.Text = "checkBox2";
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(856, 1);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(100, 20);
			this.textBox3.TabIndex = 62;
			this.textBox3.Text = "text in box";
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ArrowsWhenNoFocus = true;
			this.gridMain.HasDropDowns = true;
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(190, 284);
			this.gridMain.Name = "gridMain";
			this.gridMain.ShowContextMenu = false;
			this.gridMain.Size = new System.Drawing.Size(417, 350);
			this.gridMain.TabIndex = 81;
			this.gridMain.Title = "New Grid";
			this.gridMain.TranslationName = "test";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button1.Location = new System.Drawing.Point(1, 621);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(85, 24);
			this.button1.TabIndex = 82;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click_2);
			// 
			// button4
			// 
			this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button4.Location = new System.Drawing.Point(92, 621);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(85, 24);
			this.button4.TabIndex = 83;
			this.button4.Text = "OK";
			this.button4.UseVisualStyleBackColor = false;
			// 
			// FormLayoutManagerTests
			// 
			this.AcceptButton = this.button1;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.CancelButton = this.button3;
			this.ClientSize = new System.Drawing.Size(957, 646);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.checkBox6);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.button2);
			this.Name = "FormLayoutManagerTests";
			this.Text = "Layout Manager Tests";
			this.Load += new System.EventHandler(this.FormUIManagerTests_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormUIManagerTests_KeyDown);
			this.groupBox3.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private OpenDental.UI.GroupBox groupBox3;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.CheckBox checkBox6;
		private OpenDental.UI.Button button3;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.RadioButton radioButton4;
		private OpenDental.UI.ListBox listBox1;
		private OpenDental.UI.ComboBox comboBox1;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.TabControl tabControl;
		private OpenDental.UI.TabPage tabPage1;
		private OpenDental.UI.TabPage tabPage2;
		private OpenDental.UI.CheckBox checkBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private OpenDental.UI.CheckBox checkBox2;
		private System.Windows.Forms.TextBox textBox3;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button button1;
		private OpenDental.UI.Button button4;
	}
}