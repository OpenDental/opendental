namespace OpenDental{
	partial class FormTimePick {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimePick));
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.radioPM = new System.Windows.Forms.RadioButton();
			this.radioAM = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.comboMinute = new System.Windows.Forms.ComboBox();
			this.comboHour = new System.Windows.Forms.ComboBox();
			this.groupDate = new System.Windows.Forms.GroupBox();
			this.groupTime = new System.Windows.Forms.GroupBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupDate.SuspendLayout();
			this.groupTime.SuspendLayout();
			this.SuspendLayout();
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.Location = new System.Drawing.Point(8, 19);
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
			this.dateTimePicker.TabIndex = 65;
			// 
			// radioPM
			// 
			this.radioPM.Location = new System.Drawing.Point(147, 34);
			this.radioPM.Name = "radioPM";
			this.radioPM.Size = new System.Drawing.Size(70, 17);
			this.radioPM.TabIndex = 63;
			this.radioPM.Text = "PM";
			this.radioPM.UseVisualStyleBackColor = true;
			// 
			// radioAM
			// 
			this.radioAM.Checked = true;
			this.radioAM.Location = new System.Drawing.Point(147, 13);
			this.radioAM.Name = "radioAM";
			this.radioAM.Size = new System.Drawing.Size(70, 17);
			this.radioAM.TabIndex = 62;
			this.radioAM.TabStop = true;
			this.radioAM.Text = "AM";
			this.radioAM.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(62, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(17, 21);
			this.label1.TabIndex = 61;
			this.label1.Text = ":";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboMinute
			// 
			this.comboMinute.FormattingEnabled = true;
			this.comboMinute.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
			this.comboMinute.Location = new System.Drawing.Point(79, 22);
			this.comboMinute.MaxDropDownItems = 48;
			this.comboMinute.Name = "comboMinute";
			this.comboMinute.Size = new System.Drawing.Size(54, 21);
			this.comboMinute.TabIndex = 60;
			// 
			// comboHour
			// 
			this.comboHour.FormattingEnabled = true;
			this.comboHour.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
			this.comboHour.Location = new System.Drawing.Point(7, 22);
			this.comboHour.MaxDropDownItems = 48;
			this.comboHour.Name = "comboHour";
			this.comboHour.Size = new System.Drawing.Size(54, 21);
			this.comboHour.TabIndex = 59;
			// 
			// groupDate
			// 
			this.groupDate.Controls.Add(this.dateTimePicker);
			this.groupDate.Location = new System.Drawing.Point(16, 10);
			this.groupDate.Name = "groupDate";
			this.groupDate.Size = new System.Drawing.Size(222, 46);
			this.groupDate.TabIndex = 66;
			this.groupDate.TabStop = false;
			this.groupDate.Text = "Pick Date";
			// 
			// groupTime
			// 
			this.groupTime.Controls.Add(this.comboHour);
			this.groupTime.Controls.Add(this.radioPM);
			this.groupTime.Controls.Add(this.comboMinute);
			this.groupTime.Controls.Add(this.radioAM);
			this.groupTime.Controls.Add(this.label1);
			this.groupTime.Location = new System.Drawing.Point(16, 65);
			this.groupTime.Name = "groupTime";
			this.groupTime.Size = new System.Drawing.Size(222, 55);
			this.groupTime.TabIndex = 67;
			this.groupTime.TabStop = false;
			this.groupTime.Text = "Pick Time";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(81, 135);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(166, 135);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormTimePick
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(253, 171);
			this.Controls.Add(this.groupTime);
			this.Controls.Add(this.groupDate);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTimePick";
			this.Text = "Pick Time";
			this.Load += new System.EventHandler(this.FormTimePick_Load);
			this.groupDate.ResumeLayout(false);
			this.groupTime.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ComboBox comboHour;
		private System.Windows.Forms.ComboBox comboMinute;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioAM;
		private System.Windows.Forms.RadioButton radioPM;
		private System.Windows.Forms.DateTimePicker dateTimePicker;
		private System.Windows.Forms.GroupBox groupDate;
		private System.Windows.Forms.GroupBox groupTime;
	}
}