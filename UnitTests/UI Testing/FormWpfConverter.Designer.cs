namespace UnitTests {
	partial class FormWpfConverter {
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
			this.textName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butConvert = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.checkOverwrite = new OpenDental.UI.CheckBox();
			this.butConvert2 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textName2 = new System.Windows.Forms.TextBox();
			this.butShowTest = new System.Windows.Forms.Button();
			this.butWPFgrid = new System.Windows.Forms.Button();
			this.butGridOD = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.butShowFocus = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textTabIndex = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butFilterActs = new System.Windows.Forms.Button();
			this.buttonSplitTests = new System.Windows.Forms.Button();
			this.butSplitsWPF = new System.Windows.Forms.Button();
			this.butCombosWPF = new System.Windows.Forms.Button();
			this.butDatePickerTests = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(211, 52);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(302, 20);
			this.textName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(159, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Name of Form to convert";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butConvert
			// 
			this.butConvert.Location = new System.Drawing.Point(519, 50);
			this.butConvert.Name = "butConvert";
			this.butConvert.Size = new System.Drawing.Size(75, 23);
			this.butConvert.TabIndex = 2;
			this.butConvert.Text = "Convert";
			this.butConvert.UseVisualStyleBackColor = true;
			this.butConvert.Click += new System.EventHandler(this.butConvert_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(208, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(177, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Example: FormAccountEdit";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkOverwrite
			// 
			this.checkOverwrite.Checked = true;
			this.checkOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkOverwrite.Location = new System.Drawing.Point(393, 78);
			this.checkOverwrite.Name = "checkOverwrite";
			this.checkOverwrite.Size = new System.Drawing.Size(120, 18);
			this.checkOverwrite.TabIndex = 4;
			this.checkOverwrite.Text = "Overwrite existing";
			// 
			// butConvert2
			// 
			this.butConvert2.Location = new System.Drawing.Point(519, 183);
			this.butConvert2.Name = "butConvert2";
			this.butConvert2.Size = new System.Drawing.Size(75, 23);
			this.butConvert2.TabIndex = 7;
			this.butConvert2.Text = "Convert";
			this.butConvert2.UseVisualStyleBackColor = true;
			this.butConvert2.Click += new System.EventHandler(this.butConvert2_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(46, 183);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(159, 23);
			this.label4.TabIndex = 6;
			this.label4.Text = "UnitTests";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName2
			// 
			this.textName2.Location = new System.Drawing.Point(211, 185);
			this.textName2.Name = "textName2";
			this.textName2.ReadOnly = true;
			this.textName2.Size = new System.Drawing.Size(302, 20);
			this.textName2.TabIndex = 5;
			this.textName2.Text = "FormTestAllControls";
			// 
			// butShowTest
			// 
			this.butShowTest.Location = new System.Drawing.Point(349, 272);
			this.butShowTest.Name = "butShowTest";
			this.butShowTest.Size = new System.Drawing.Size(102, 23);
			this.butShowTest.TabIndex = 10;
			this.butShowTest.Text = "Show WPF Tests";
			this.butShowTest.UseVisualStyleBackColor = true;
			this.butShowTest.Click += new System.EventHandler(this.butShowTest_Click);
			// 
			// butWPFgrid
			// 
			this.butWPFgrid.Location = new System.Drawing.Point(348, 315);
			this.butWPFgrid.Name = "butWPFgrid";
			this.butWPFgrid.Size = new System.Drawing.Size(103, 23);
			this.butWPFgrid.TabIndex = 11;
			this.butWPFgrid.Text = "Show WPF Grid";
			this.butWPFgrid.UseVisualStyleBackColor = true;
			this.butWPFgrid.Click += new System.EventHandler(this.butWPFgrid_Click);
			// 
			// butGridOD
			// 
			this.butGridOD.Location = new System.Drawing.Point(200, 315);
			this.butGridOD.Name = "butGridOD";
			this.butGridOD.Size = new System.Drawing.Size(102, 23);
			this.butGridOD.TabIndex = 12;
			this.butGridOD.Text = "Show GridOD";
			this.butGridOD.UseVisualStyleBackColor = true;
			this.butGridOD.Click += new System.EventHandler(this.butGridOD_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(200, 272);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(122, 23);
			this.button1.TabIndex = 13;
			this.button1.Text = "Show WinForm Tests";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// butShowFocus
			// 
			this.butShowFocus.Location = new System.Drawing.Point(486, 272);
			this.butShowFocus.Name = "butShowFocus";
			this.butShowFocus.Size = new System.Drawing.Size(102, 23);
			this.butShowFocus.TabIndex = 14;
			this.butShowFocus.Text = "Show Focus Test";
			this.butShowFocus.UseVisualStyleBackColor = true;
			this.butShowFocus.Click += new System.EventHandler(this.butShowFocus_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(159, 23);
			this.label3.TabIndex = 16;
			this.label3.Text = "Include TabIndex up to";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTabIndex
			// 
			this.textTabIndex.Location = new System.Drawing.Point(211, 102);
			this.textTabIndex.Name = "textTabIndex";
			this.textTabIndex.Size = new System.Drawing.Size(55, 20);
			this.textTabIndex.TabIndex = 15;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(269, 102);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(227, 20);
			this.label5.TabIndex = 17;
			this.label5.Text = "Enter a hyphen to not convert TabIndexes";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butFilterActs
			// 
			this.butFilterActs.Location = new System.Drawing.Point(200, 344);
			this.butFilterActs.Name = "butFilterActs";
			this.butFilterActs.Size = new System.Drawing.Size(103, 23);
			this.butFilterActs.TabIndex = 18;
			this.butFilterActs.Text = "Show Filter Acts";
			this.butFilterActs.UseVisualStyleBackColor = true;
			this.butFilterActs.Click += new System.EventHandler(this.butFilterActs_Click);
			// 
			// buttonSplitTests
			// 
			this.buttonSplitTests.Location = new System.Drawing.Point(200, 373);
			this.buttonSplitTests.Name = "buttonSplitTests";
			this.buttonSplitTests.Size = new System.Drawing.Size(102, 23);
			this.buttonSplitTests.TabIndex = 19;
			this.buttonSplitTests.Text = "Show Split Tests";
			this.buttonSplitTests.UseVisualStyleBackColor = true;
			this.buttonSplitTests.Click += new System.EventHandler(this.buttonSplitTests_Click);
			// 
			// butSplitsWPF
			// 
			this.butSplitsWPF.Location = new System.Drawing.Point(348, 373);
			this.butSplitsWPF.Name = "butSplitsWPF";
			this.butSplitsWPF.Size = new System.Drawing.Size(102, 23);
			this.butSplitsWPF.TabIndex = 20;
			this.butSplitsWPF.Text = "Show WPF Splits";
			this.butSplitsWPF.UseVisualStyleBackColor = true;
			this.butSplitsWPF.Click += new System.EventHandler(this.butSplitsWPF_Click);
			// 
			// butCombosWPF
			// 
			this.butCombosWPF.Location = new System.Drawing.Point(349, 344);
			this.butCombosWPF.Name = "butCombosWPF";
			this.butCombosWPF.Size = new System.Drawing.Size(114, 23);
			this.butCombosWPF.TabIndex = 21;
			this.butCombosWPF.Text = "Show WPF Combo";
			this.butCombosWPF.UseVisualStyleBackColor = true;
			this.butCombosWPF.Click += new System.EventHandler(this.butCombosWPF_Click);
			// 
			// butDatePickerTests
			// 
			this.butDatePickerTests.Location = new System.Drawing.Point(83, 373);
			this.butDatePickerTests.Name = "butDatePickerTests";
			this.butDatePickerTests.Size = new System.Drawing.Size(102, 23);
			this.butDatePickerTests.TabIndex = 22;
			this.butDatePickerTests.Text = "DatePicker Tests";
			this.butDatePickerTests.UseVisualStyleBackColor = true;
			this.butDatePickerTests.Click += new System.EventHandler(this.butDatePickerTests_Click);
			// 
			// FormWpfConverter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(658, 403);
			this.Controls.Add(this.butDatePickerTests);
			this.Controls.Add(this.butCombosWPF);
			this.Controls.Add(this.butSplitsWPF);
			this.Controls.Add(this.buttonSplitTests);
			this.Controls.Add(this.butFilterActs);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textTabIndex);
			this.Controls.Add(this.butShowFocus);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.butGridOD);
			this.Controls.Add(this.butWPFgrid);
			this.Controls.Add(this.butShowTest);
			this.Controls.Add(this.butConvert2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textName2);
			this.Controls.Add(this.checkOverwrite);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butConvert);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textName);
			this.Name = "FormWpfConverter";
			this.Text = "FormWpfConverter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butConvert;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.CheckBox checkOverwrite;
		private System.Windows.Forms.Button butConvert2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textName2;
		private System.Windows.Forms.Button butShowTest;
		private System.Windows.Forms.Button butWPFgrid;
		private System.Windows.Forms.Button butGridOD;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button butShowFocus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textTabIndex;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button butFilterActs;
		private System.Windows.Forms.Button buttonSplitTests;
		private System.Windows.Forms.Button butSplitsWPF;
		private System.Windows.Forms.Button butCombosWPF;
		private System.Windows.Forms.Button butDatePickerTests;
	}
}