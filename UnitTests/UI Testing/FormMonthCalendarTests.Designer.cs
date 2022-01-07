namespace UnitTests {
	partial class FormMonthCalendarTests {
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
			this.monthCalendar = new System.Windows.Forms.MonthCalendar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new OpenDental.UI.Button();
			this.button2 = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.monthCalendarOD2 = new OpenDental.UI.MonthCalendarOD();
			this.monthCalendarOD1 = new OpenDental.UI.MonthCalendarOD();
			this.SuspendLayout();
			// 
			// monthCalendar
			// 
			this.monthCalendar.Location = new System.Drawing.Point(42, 91);
			this.monthCalendar.Name = "monthCalendar";
			this.monthCalendar.TabIndex = 0;
			this.monthCalendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar_DateSelected);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(43, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "Microsoft";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(277, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 3;
			this.label2.Text = "OD";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(119, 62);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 24);
			this.button1.TabIndex = 4;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(356, 62);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 24);
			this.button2.TabIndex = 5;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(515, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 18);
			this.label3.TabIndex = 7;
			this.label3.Text = "Double";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// monthCalendarOD2
			// 
			this.monthCalendarOD2.Location = new System.Drawing.Point(517, 91);
			this.monthCalendarOD2.Name = "monthCalendarOD2";
			this.monthCalendarOD2.Size = new System.Drawing.Size(227, 162);
			this.monthCalendarOD2.TabIndex = 6;
			this.monthCalendarOD2.Text = "monthCalendarOD1";
			// 
			// monthCalendarOD1
			// 
			this.monthCalendarOD1.Location = new System.Drawing.Point(279, 92);
			this.monthCalendarOD1.Name = "monthCalendarOD1";
			this.monthCalendarOD1.Size = new System.Drawing.Size(227, 162);
			this.monthCalendarOD1.TabIndex = 8;
			this.monthCalendarOD1.Text = "monthCalendarOD1";
			// 
			// FormMonthCalendarTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1057, 561);
			this.Controls.Add(this.monthCalendarOD1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.monthCalendarOD2);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.monthCalendar);
			this.Name = "FormMonthCalendarTests";
			this.Text = "FormDatePickerTesting";
			this.Load += new System.EventHandler(this.FormMonthCalendarTests_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.MonthCalendar monthCalendar;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button button1;
		private OpenDental.UI.Button button2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.MonthCalendarOD monthCalendarOD2;
		private OpenDental.UI.MonthCalendarOD monthCalendarOD1;
	}
}