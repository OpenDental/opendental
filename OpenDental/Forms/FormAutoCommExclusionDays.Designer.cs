namespace OpenDental{
	partial class FormeConfimationExclusionDays {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormeConfimationExclusionDays));
			this.butSave = new OpenDental.UI.Button();
			this.checkUseHQ = new OpenDental.UI.CheckBox();
			this.comboBoxClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.listBoxExclusionDays = new OpenDental.UI.ListBox();
			this.listBoxExclusionDates = new OpenDental.UI.ListBox();
			this.labelExclusionDays = new System.Windows.Forms.Label();
			this.labelExclusionDates = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.labelUseDefaultMessage = new System.Windows.Forms.Label();
			this.checkShowPastDates = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(349, 440);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkUseHQ
			// 
			this.checkUseHQ.Location = new System.Drawing.Point(45, 95);
			this.checkUseHQ.Name = "checkUseHQ";
			this.checkUseHQ.Size = new System.Drawing.Size(125, 24);
			this.checkUseHQ.TabIndex = 4;
			this.checkUseHQ.Text = "Use Default Settings";
			this.checkUseHQ.CheckedChanged += new System.EventHandler(this.checkUseHQ_CheckedChanged);
			// 
			// comboBoxClinicPicker
			// 
			this.comboBoxClinicPicker.HqDescription = "Default";
			this.comboBoxClinicPicker.IncludeUnassigned = true;
			this.comboBoxClinicPicker.Location = new System.Drawing.Point(38, 59);
			this.comboBoxClinicPicker.Name = "comboBoxClinicPicker";
			this.comboBoxClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.comboBoxClinicPicker.TabIndex = 5;
			this.comboBoxClinicPicker.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxClinicPicker_SelectionChangeCommitted);
			// 
			// listBoxExclusionDays
			// 
			this.listBoxExclusionDays.Location = new System.Drawing.Point(45, 148);
			this.listBoxExclusionDays.Name = "listBoxExclusionDays";
			this.listBoxExclusionDays.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxExclusionDays.Size = new System.Drawing.Size(120, 115);
			this.listBoxExclusionDays.TabIndex = 6;
			this.listBoxExclusionDays.Text = "listBoxExclusionDays";
			// 
			// listBoxExclusionDates
			// 
			this.listBoxExclusionDates.Cursor = System.Windows.Forms.Cursors.Default;
			this.listBoxExclusionDates.Location = new System.Drawing.Point(171, 148);
			this.listBoxExclusionDates.Name = "listBoxExclusionDates";
			this.listBoxExclusionDates.Size = new System.Drawing.Size(120, 316);
			this.listBoxExclusionDates.TabIndex = 7;
			this.listBoxExclusionDates.Text = "listBoxExclusionDates";
			// 
			// labelExclusionDays
			// 
			this.labelExclusionDays.Location = new System.Drawing.Point(42, 126);
			this.labelExclusionDays.Name = "labelExclusionDays";
			this.labelExclusionDays.Size = new System.Drawing.Size(123, 18);
			this.labelExclusionDays.TabIndex = 8;
			this.labelExclusionDays.Text = "Exclusion Days";
			this.labelExclusionDays.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelExclusionDates
			// 
			this.labelExclusionDates.Location = new System.Drawing.Point(168, 126);
			this.labelExclusionDates.Name = "labelExclusionDates";
			this.labelExclusionDates.Size = new System.Drawing.Size(175, 18);
			this.labelExclusionDates.TabIndex = 9;
			this.labelExclusionDates.Text = "Exclusion Dates";
			this.labelExclusionDates.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDelete
			// 
			this.butDelete.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(297, 210);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 10;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAdd
			// 
			this.butAdd.Image = global::OpenDental.Properties.Resources.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(297, 180);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 11;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// labelUseDefaultMessage
			// 
			this.labelUseDefaultMessage.ForeColor = System.Drawing.Color.Firebrick;
			this.labelUseDefaultMessage.Location = new System.Drawing.Point(168, 95);
			this.labelUseDefaultMessage.Name = "labelUseDefaultMessage";
			this.labelUseDefaultMessage.Size = new System.Drawing.Size(175, 23);
			this.labelUseDefaultMessage.TabIndex = 12;
			this.labelUseDefaultMessage.Text = "Using Default Settings";
			this.labelUseDefaultMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelUseDefaultMessage.Visible = false;
			// 
			// checkShowPastDates
			// 
			this.checkShowPastDates.Location = new System.Drawing.Point(297, 150);
			this.checkShowPastDates.Name = "checkShowPastDates";
			this.checkShowPastDates.Size = new System.Drawing.Size(138, 24);
			this.checkShowPastDates.TabIndex = 13;
			this.checkShowPastDates.Text = "Show Past Dates";
			this.checkShowPastDates.CheckedChanged += new System.EventHandler(this.checkShowPastDates_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(372, 34);
			this.label1.TabIndex = 14;
			this.label1.Text = "Set which days of the week or dates to exclude sending eConfirmations and Web Sch" +
    "ed Recalls";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormeConfimationExclusionDays
			// 
			this.ClientSize = new System.Drawing.Size(439, 482);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelUseDefaultMessage);
			this.Controls.Add(this.checkShowPastDates);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelExclusionDates);
			this.Controls.Add(this.listBoxExclusionDates);
			this.Controls.Add(this.listBoxExclusionDays);
			this.Controls.Add(this.labelExclusionDays);
			this.Controls.Add(this.comboBoxClinicPicker);
			this.Controls.Add(this.checkUseHQ);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormeConfimationExclusionDays";
			this.Text = "eConfirmation and Web Sched Recall Exclusion Days";
			this.Load += new System.EventHandler(this.FormAutoCommExclusionDays_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
        private OpenDental.UI.CheckBox checkUseHQ;
        private UI.ComboBoxClinicPicker comboBoxClinicPicker;
        private UI.ListBox listBoxExclusionDays;
        private UI.ListBox listBoxExclusionDates;
        private System.Windows.Forms.Label labelExclusionDays;
        private System.Windows.Forms.Label labelExclusionDates;
        private UI.Button butDelete;
        private UI.Button butAdd;
        private System.Windows.Forms.Label labelUseDefaultMessage;
        private OpenDental.UI.CheckBox checkShowPastDates;
		private System.Windows.Forms.Label label1;
	}
}