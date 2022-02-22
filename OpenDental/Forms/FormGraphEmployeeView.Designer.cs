namespace OpenDental {
	partial class FormGraphEmployeeView {
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
			this.listBoxEmployees = new UI.ListBoxOD();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.butClose = new OpenDental.UI.Button();
			this.butPaste = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.odDatePickerTo = new OpenDental.UI.ODDatePicker();
			this.odDatePickerFrom = new OpenDental.UI.ODDatePicker();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelCopyPaste = new System.Windows.Forms.Label();
			this.butPrint = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listBoxEmployees
			// 
			this.listBoxEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxEmployees.Location = new System.Drawing.Point(13, 62);
			this.listBoxEmployees.Name = "listBoxEmployees";
			this.listBoxEmployees.Size = new System.Drawing.Size(142, 862);
			this.listBoxEmployees.TabIndex = 4;
			this.listBoxEmployees.SelectedIndexChanged += new System.EventHandler(this.ListBoxEmployees_SelectedIndexChanged);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(1, 36);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(44, 23);
			this.label14.TabIndex = 8;
			this.label14.Text = "To";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(10, 13);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(35, 23);
			this.label13.TabIndex = 7;
			this.label13.Text = "From";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1124, 931);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 11;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.ButClose_Click);
			// 
			// butPaste
			// 
			this.butPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPaste.Location = new System.Drawing.Point(246, 931);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(75, 24);
			this.butPaste.TabIndex = 10;
			this.butPaste.Text = "Paste";
			this.butPaste.UseVisualStyleBackColor = true;
			this.butPaste.Click += new System.EventHandler(this.ButPaste_Click);
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopy.Location = new System.Drawing.Point(164, 931);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 9;
			this.butCopy.Text = "Copy";
			this.butCopy.UseVisualStyleBackColor = true;
			this.butCopy.Click += new System.EventHandler(this.ButCopy_Click);
			// 
			// odDatePickerTo
			// 
			this.odDatePickerTo.AdjustCalendarLocation = new System.Drawing.Point(23, 0);
			this.odDatePickerTo.BackColor = System.Drawing.Color.Transparent;
			this.odDatePickerTo.Location = new System.Drawing.Point(-11, 36);
			this.odDatePickerTo.Name = "odDatePickerTo";
			this.odDatePickerTo.Size = new System.Drawing.Size(169, 23);
			this.odDatePickerTo.TabIndex = 6;
			this.odDatePickerTo.DateTextChanged += new OpenDental.UI.DateTextChangedHandler(this.OdDatePickerTo_DateTextChanged);
			// 
			// odDatePickerFrom
			// 
			this.odDatePickerFrom.AdjustCalendarLocation = new System.Drawing.Point(23, 0);
			this.odDatePickerFrom.BackColor = System.Drawing.Color.Transparent;
			this.odDatePickerFrom.Location = new System.Drawing.Point(-11, 13);
			this.odDatePickerFrom.Name = "odDatePickerFrom";
			this.odDatePickerFrom.Size = new System.Drawing.Size(169, 23);
			this.odDatePickerFrom.TabIndex = 5;
			this.odDatePickerFrom.DateTextChanged += new OpenDental.UI.DateTextChangedHandler(this.OdDatePickerFrom_DateTextChanged);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(164, 13);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1035, 911);
			this.gridMain.TabIndex = 3;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridMain_CellDoubleClick);
			// 
			// labelCopyPaste
			// 
			this.labelCopyPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelCopyPaste.Location = new System.Drawing.Point(328, 931);
			this.labelCopyPaste.Name = "labelCopyPaste";
			this.labelCopyPaste.Size = new System.Drawing.Size(559, 23);
			this.labelCopyPaste.TabIndex = 12;
			this.labelCopyPaste.Text = "Select one row to copy and select any number of rows to paste into. Doesn\'t copy " +
    "employee schedule.";
			this.labelCopyPaste.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butPrint
			// 
			this.butPrint.Location = new System.Drawing.Point(13, 931);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 13;
			this.butPrint.Text = "Print";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.ButPrint_Click);
			// 
			// FormGraphEmployeeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1213, 963);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.labelCopyPaste);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPaste);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.listBoxEmployees);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.odDatePickerTo);
			this.Controls.Add(this.odDatePickerFrom);
			this.Controls.Add(this.gridMain);
			this.Name = "FormGraphEmployeeView";
			this.Text = "Employee Schedules";
			this.Load += new System.EventHandler(this.FormGraphEmployeeView_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private UI.ListBoxOD listBoxEmployees;
		private UI.ODDatePicker odDatePickerFrom;
		private UI.ODDatePicker odDatePickerTo;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private UI.Button butCopy;
		private UI.Button butPaste;
		private UI.Button butClose;
		private System.Windows.Forms.Label labelCopyPaste;
		private UI.Button butPrint;
	}
}