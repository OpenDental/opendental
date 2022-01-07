namespace SlowQueryTool {
	partial class FormQueryGroup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			this.textExampleQuery = new System.Windows.Forms.TextBox();
			this.g = new System.Windows.Forms.GroupBox();
			this.textExecutionTotal = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textExecutionMin = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textExecutionMax = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textExecutionAvg = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textExecutionMedian = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textLockTotal = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textLockMin = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textLockMax = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textLockAvg = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textLockMedian = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.textRowsMedian = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textRowsAvg = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textRowsMax = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textRowsMin = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butClose = new System.Windows.Forms.Button();
			this.textCount = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.textAvgTimeBetween = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textGroupNum = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.g.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textExampleQuery
			// 
			this.textExampleQuery.Location = new System.Drawing.Point(12, 210);
			this.textExampleQuery.Multiline = true;
			this.textExampleQuery.Name = "textExampleQuery";
			this.textExampleQuery.ReadOnly = true;
			this.textExampleQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textExampleQuery.Size = new System.Drawing.Size(622, 351);
			this.textExampleQuery.TabIndex = 4;
			// 
			// g
			// 
			this.g.Controls.Add(this.textExecutionTotal);
			this.g.Controls.Add(this.label6);
			this.g.Controls.Add(this.textExecutionMin);
			this.g.Controls.Add(this.label5);
			this.g.Controls.Add(this.textExecutionMax);
			this.g.Controls.Add(this.label4);
			this.g.Controls.Add(this.textExecutionAvg);
			this.g.Controls.Add(this.label3);
			this.g.Controls.Add(this.textExecutionMedian);
			this.g.Controls.Add(this.label1);
			this.g.Location = new System.Drawing.Point(12, 41);
			this.g.Name = "g";
			this.g.Size = new System.Drawing.Size(200, 149);
			this.g.TabIndex = 1;
			this.g.TabStop = false;
			this.g.Text = "Execution Time";
			// 
			// textExecutionTotal
			// 
			this.textExecutionTotal.Location = new System.Drawing.Point(78, 123);
			this.textExecutionTotal.Name = "textExecutionTotal";
			this.textExecutionTotal.ReadOnly = true;
			this.textExecutionTotal.Size = new System.Drawing.Size(100, 20);
			this.textExecutionTotal.TabIndex = 4;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(15, 126);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(57, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "Total Time";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExecutionMin
			// 
			this.textExecutionMin.Location = new System.Drawing.Point(78, 97);
			this.textExecutionMin.Name = "textExecutionMin";
			this.textExecutionMin.ReadOnly = true;
			this.textExecutionMin.Size = new System.Drawing.Size(100, 20);
			this.textExecutionMin.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(26, 100);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(46, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Shortest";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExecutionMax
			// 
			this.textExecutionMax.Location = new System.Drawing.Point(78, 71);
			this.textExecutionMax.Name = "textExecutionMax";
			this.textExecutionMax.ReadOnly = true;
			this.textExecutionMax.Size = new System.Drawing.Size(100, 20);
			this.textExecutionMax.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(27, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Longest";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExecutionAvg
			// 
			this.textExecutionAvg.Location = new System.Drawing.Point(78, 45);
			this.textExecutionAvg.Name = "textExecutionAvg";
			this.textExecutionAvg.ReadOnly = true;
			this.textExecutionAvg.Size = new System.Drawing.Size(100, 20);
			this.textExecutionAvg.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(25, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Average";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExecutionMedian
			// 
			this.textExecutionMedian.Location = new System.Drawing.Point(78, 19);
			this.textExecutionMedian.Name = "textExecutionMedian";
			this.textExecutionMedian.ReadOnly = true;
			this.textExecutionMedian.Size = new System.Drawing.Size(100, 20);
			this.textExecutionMedian.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Median";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textLockTotal);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textLockMin);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.textLockMax);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.textLockAvg);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.textLockMedian);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Location = new System.Drawing.Point(223, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 149);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Lock Time";
			// 
			// textLockTotal
			// 
			this.textLockTotal.Location = new System.Drawing.Point(78, 123);
			this.textLockTotal.Name = "textLockTotal";
			this.textLockTotal.ReadOnly = true;
			this.textLockTotal.Size = new System.Drawing.Size(100, 20);
			this.textLockTotal.TabIndex = 4;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 126);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(57, 13);
			this.label7.TabIndex = 10;
			this.label7.Text = "Total Time";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLockMin
			// 
			this.textLockMin.Location = new System.Drawing.Point(78, 97);
			this.textLockMin.Name = "textLockMin";
			this.textLockMin.ReadOnly = true;
			this.textLockMin.Size = new System.Drawing.Size(100, 20);
			this.textLockMin.TabIndex = 3;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(26, 100);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(46, 13);
			this.label8.TabIndex = 8;
			this.label8.Text = "Shortest";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLockMax
			// 
			this.textLockMax.Location = new System.Drawing.Point(78, 71);
			this.textLockMax.Name = "textLockMax";
			this.textLockMax.ReadOnly = true;
			this.textLockMax.Size = new System.Drawing.Size(100, 20);
			this.textLockMax.TabIndex = 2;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(27, 74);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(45, 13);
			this.label9.TabIndex = 6;
			this.label9.Text = "Longest";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLockAvg
			// 
			this.textLockAvg.Location = new System.Drawing.Point(78, 45);
			this.textLockAvg.Name = "textLockAvg";
			this.textLockAvg.ReadOnly = true;
			this.textLockAvg.Size = new System.Drawing.Size(100, 20);
			this.textLockAvg.TabIndex = 1;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(25, 48);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(47, 13);
			this.label10.TabIndex = 4;
			this.label10.Text = "Average";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLockMedian
			// 
			this.textLockMedian.Location = new System.Drawing.Point(78, 19);
			this.textLockMedian.Name = "textLockMedian";
			this.textLockMedian.ReadOnly = true;
			this.textLockMedian.Size = new System.Drawing.Size(100, 20);
			this.textLockMedian.TabIndex = 0;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(30, 22);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(42, 13);
			this.label11.TabIndex = 2;
			this.label11.Text = "Median";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(12, 193);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(78, 13);
			this.label17.TabIndex = 14;
			this.label17.Text = "Example Query";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(30, 32);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(42, 13);
			this.label16.TabIndex = 2;
			this.label16.Text = "Median";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRowsMedian
			// 
			this.textRowsMedian.Location = new System.Drawing.Point(78, 29);
			this.textRowsMedian.Name = "textRowsMedian";
			this.textRowsMedian.ReadOnly = true;
			this.textRowsMedian.Size = new System.Drawing.Size(100, 20);
			this.textRowsMedian.TabIndex = 0;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(25, 58);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(47, 13);
			this.label15.TabIndex = 4;
			this.label15.Text = "Average";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRowsAvg
			// 
			this.textRowsAvg.Location = new System.Drawing.Point(78, 55);
			this.textRowsAvg.Name = "textRowsAvg";
			this.textRowsAvg.ReadOnly = true;
			this.textRowsAvg.Size = new System.Drawing.Size(100, 20);
			this.textRowsAvg.TabIndex = 1;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(42, 84);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(30, 13);
			this.label14.TabIndex = 6;
			this.label14.Text = "Most";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRowsMax
			// 
			this.textRowsMax.Location = new System.Drawing.Point(78, 81);
			this.textRowsMax.Name = "textRowsMax";
			this.textRowsMax.ReadOnly = true;
			this.textRowsMax.Size = new System.Drawing.Size(100, 20);
			this.textRowsMax.TabIndex = 2;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(31, 110);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(41, 13);
			this.label13.TabIndex = 8;
			this.label13.Text = "Fewest";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRowsMin
			// 
			this.textRowsMin.Location = new System.Drawing.Point(78, 107);
			this.textRowsMin.Name = "textRowsMin";
			this.textRowsMin.ReadOnly = true;
			this.textRowsMin.Size = new System.Drawing.Size(100, 20);
			this.textRowsMin.TabIndex = 3;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textRowsMin);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.textRowsMax);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.textRowsAvg);
			this.groupBox2.Controls.Add(this.label15);
			this.groupBox2.Controls.Add(this.textRowsMedian);
			this.groupBox2.Controls.Add(this.label16);
			this.groupBox2.Location = new System.Drawing.Point(434, 41);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 149);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Rows Examined";
			// 
			// butClose
			// 
			this.butClose.Location = new System.Drawing.Point(559, 567);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 5;
			this.butClose.Text = "&Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// textCount
			// 
			this.textCount.Location = new System.Drawing.Point(248, 8);
			this.textCount.Name = "textCount";
			this.textCount.ReadOnly = true;
			this.textCount.Size = new System.Drawing.Size(100, 20);
			this.textCount.TabIndex = 0;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(110, 11);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(138, 13);
			this.label12.TabIndex = 16;
			this.label12.Text = "Number of Queries in Group";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAvgTimeBetween
			// 
			this.textAvgTimeBetween.Location = new System.Drawing.Point(512, 8);
			this.textAvgTimeBetween.Name = "textAvgTimeBetween";
			this.textAvgTimeBetween.ReadOnly = true;
			this.textAvgTimeBetween.Size = new System.Drawing.Size(100, 20);
			this.textAvgTimeBetween.TabIndex = 17;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(354, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(157, 13);
			this.label2.TabIndex = 18;
			this.label2.Text = "Average Time Between Queries";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGroupNum
			// 
			this.textGroupNum.Location = new System.Drawing.Point(74, 8);
			this.textGroupNum.Name = "textGroupNum";
			this.textGroupNum.ReadOnly = true;
			this.textGroupNum.Size = new System.Drawing.Size(25, 20);
			this.textGroupNum.TabIndex = 19;
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(12, 11);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(61, 13);
			this.label18.TabIndex = 20;
			this.label18.Text = "Group Num";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormQueryGroup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(645, 597);
			this.Controls.Add(this.textGroupNum);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.textAvgTimeBetween);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textCount);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.g);
			this.Controls.Add(this.textExampleQuery);
			this.Name = "FormQueryGroup";
			this.Text = "Query Information";
			this.Load += new System.EventHandler(this.FormQuery_Load);
			this.g.ResumeLayout(false);
			this.g.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textExampleQuery;
		private System.Windows.Forms.GroupBox g;
		private System.Windows.Forms.TextBox textExecutionTotal;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textExecutionMin;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textExecutionMax;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textExecutionAvg;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textExecutionMedian;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textLockTotal;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textLockMin;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textLockMax;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textLockAvg;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textLockMedian;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textRowsMedian;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textRowsAvg;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textRowsMax;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textRowsMin;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.TextBox textCount;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textAvgTimeBetween;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textGroupNum;
		private System.Windows.Forms.Label label18;
	}
}

