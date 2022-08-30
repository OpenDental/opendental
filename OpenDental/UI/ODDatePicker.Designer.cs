namespace OpenDental.UI {
	partial class ODDatePicker {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.calendar = new System.Windows.Forms.MonthCalendar();
			this.butDrop = new System.Windows.Forms.Button();
			this.textDate = new OpenDental.ValidDate();
			this.SuspendLayout();
			// 
			// calendar
			// 
			this.calendar.Location = new System.Drawing.Point(0, 21);
			this.calendar.MaxSelectionCount = 1;
			this.calendar.Name = "calendar";
			this.calendar.TabIndex = 62;
			this.calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendar_DateSelected);
			// 
			// butDrop
			// 
			this.butDrop.BackColor = System.Drawing.Color.White;
			this.butDrop.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.butDrop.FlatAppearance.BorderSize = 0;
			this.butDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
			this.butDrop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
			this.butDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butDrop.Image = global::OpenDental.Properties.Resources.downArrowWinForm;
			this.butDrop.Location = new System.Drawing.Point(148, 2);
			this.butDrop.Name = "butDrop";
			this.butDrop.Size = new System.Drawing.Size(16, 18);
			this.butDrop.TabIndex = 63;
			this.butDrop.UseVisualStyleBackColor = false;
			this.butDrop.Click += new System.EventHandler(this.butToggleCalendar_Click);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(63, 1);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(102, 20);
			this.textDate.TabIndex = 59;
			this.textDate.SizeChanged += new System.EventHandler(this.textDate_SizeChanged);
			// 
			// ODDatePicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.butDrop);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.calendar);
			this.Name = "ODDatePicker";
			this.Size = new System.Drawing.Size(227, 23);
			this.Load += new System.EventHandler(this.ODDatePicker_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MonthCalendar calendar;
		private System.Windows.Forms.Button butDrop;
		private ValidDate textDate;
	}
}
