namespace OpenDental.UI {
	partial class ODDateRangePicker {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ODDateRangePicker));
			this.imageListCalendar = new System.Windows.Forms.ImageList(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panelWeek = new OpenDental.UI.PanelOD();
			this.butWeekPrevious = new OpenDental.UI.Button();
			this.butWeekNext = new OpenDental.UI.Button();
			this.datePickerFrom = new OpenDental.UI.ODDatePicker();
			this.datePickerTo = new OpenDental.UI.ODDatePicker();
			this.panelWeek.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListCalendar
			// 
			this.imageListCalendar.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListCalendar.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListCalendar.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 18);
			this.label1.TabIndex = 58;
			this.label1.Text = "From";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(263, 2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 18);
			this.label2.TabIndex = 60;
			this.label2.Text = "To";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panelWeek
			// 
			this.panelWeek.BackColor = System.Drawing.Color.Transparent;
			this.panelWeek.Controls.Add(this.butWeekPrevious);
			this.panelWeek.Controls.Add(this.butWeekNext);
			this.panelWeek.Location = new System.Drawing.Point(193, -1);
			this.panelWeek.Name = "panelWeek";
			this.panelWeek.Size = new System.Drawing.Size(80, 24);
			this.panelWeek.TabIndex = 71;
			// 
			// butWeekPrevious
			// 
			this.butWeekPrevious.AdjustImageLocation = new System.Drawing.Point(-3, -1);
			this.butWeekPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butWeekPrevious.Image = ((System.Drawing.Image)(resources.GetObject("butWeekPrevious.Image")));
			this.butWeekPrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butWeekPrevious.Location = new System.Drawing.Point(5, 1);
			this.butWeekPrevious.Name = "butWeekPrevious";
			this.butWeekPrevious.Size = new System.Drawing.Size(33, 22);
			this.butWeekPrevious.TabIndex = 67;
			this.butWeekPrevious.Text = "W";
			this.butWeekPrevious.Click += new System.EventHandler(this.butWeekPrevious_Click);
			// 
			// butWeekNext
			// 
			this.butWeekNext.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butWeekNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butWeekNext.Image = ((System.Drawing.Image)(resources.GetObject("butWeekNext.Image")));
			this.butWeekNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butWeekNext.Location = new System.Drawing.Point(43, 1);
			this.butWeekNext.Name = "butWeekNext";
			this.butWeekNext.Size = new System.Drawing.Size(33, 22);
			this.butWeekNext.TabIndex = 66;
			this.butWeekNext.Text = "W";
			this.butWeekNext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butWeekNext.Click += new System.EventHandler(this.butWeekNext_Click);
			// 
			// datePickerFrom
			// 
			this.datePickerFrom.BackColor = System.Drawing.Color.Transparent;
			this.datePickerFrom.HideCalendarOnLeave = false;
			this.datePickerFrom.Location = new System.Drawing.Point(2, 1);
			this.datePickerFrom.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerFrom.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerFrom.Name = "datePickerFrom";
			this.datePickerFrom.Size = new System.Drawing.Size(227, 23);
			this.datePickerFrom.TabIndex = 72;
			this.datePickerFrom.CalendarOpened += new OpenDental.UI.CalendarOpenedHandler(this.datePickerFrom_CalendarOpened);
			this.datePickerFrom.CalendarClosed += new OpenDental.UI.CalendarClosedHandler(this.datePickerFrom_CalendarClosed);
			this.datePickerFrom.CalendarSelectionChanged += new OpenDental.UI.CalendarSelectionHandler(this.datePickerFrom_CalendarSelectionChanged);
			// 
			// datePickerTo
			// 
			this.datePickerTo.BackColor = System.Drawing.Color.Transparent;
			this.datePickerTo.HideCalendarOnLeave = false;
			this.datePickerTo.Location = new System.Drawing.Point(237, 1);
			this.datePickerTo.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerTo.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerTo.Name = "datePickerTo";
			this.datePickerTo.Size = new System.Drawing.Size(227, 23);
			this.datePickerTo.TabIndex = 73;
			this.datePickerTo.CalendarOpened += new OpenDental.UI.CalendarOpenedHandler(this.datePickerTo_CalendarOpened);
			this.datePickerTo.CalendarClosed += new OpenDental.UI.CalendarClosedHandler(this.datePickerTo_CalendarClosed);
			this.datePickerTo.CalendarSelectionChanged += new OpenDental.UI.CalendarSelectionHandler(this.datePickerTo_CalendarSelectionChanged);
			// 
			// ODDateRangePicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.panelWeek);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.datePickerFrom);
			this.Controls.Add(this.datePickerTo);
			this.Name = "ODDateRangePicker";
			this.Size = new System.Drawing.Size(453, 24);
			this.Load += new System.EventHandler(this.ODDateRangePicker_Load);
			this.panelWeek.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		protected System.Windows.Forms.ImageList imageListCalendar;
		protected Button butWeekPrevious;
		protected Button butWeekNext;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.Label label2;
		protected PanelOD panelWeek;
		private ODDatePicker datePickerFrom;
		private ODDatePicker datePickerTo;
	}
}
