namespace OpenDental{
	partial class FormAutoCommExclusionDays {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoCommExclusionDays));
            this.butOK = new OpenDental.UI.Button();
            this.butCancel = new OpenDental.UI.Button();
            this.checkUseHQ = new System.Windows.Forms.CheckBox();
            this.comboBoxClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
            this.listBoxExclusionDays = new OpenDental.UI.ListBoxOD();
            this.listBoxExclusionDates = new OpenDental.UI.ListBoxOD();
            this.labelExclusionDays = new System.Windows.Forms.Label();
            this.labelExclusionDates = new System.Windows.Forms.Label();
            this.butDelete = new OpenDental.UI.Button();
            this.butAdd = new OpenDental.UI.Button();
            this.labelUseHQMessage = new System.Windows.Forms.Label();
            this.checkShowPastDates = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(309, 467);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 24);
            this.butOK.TabIndex = 3;
            this.butOK.Text = "&OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(395, 467);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 24);
            this.butCancel.TabIndex = 2;
            this.butCancel.Text = "&Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // checkUseHQ
            // 
            this.checkUseHQ.Location = new System.Drawing.Point(67, 69);
            this.checkUseHQ.Name = "checkUseHQ";
            this.checkUseHQ.Size = new System.Drawing.Size(125, 24);
            this.checkUseHQ.TabIndex = 4;
            this.checkUseHQ.Text = "Use HQ Settings";
            this.checkUseHQ.UseVisualStyleBackColor = true;
            this.checkUseHQ.CheckedChanged += new System.EventHandler(this.checkUseHQ_CheckedChanged);
            // 
            // comboBoxClinicPicker
            // 
            this.comboBoxClinicPicker.HqDescription = "HQ";
            this.comboBoxClinicPicker.IncludeUnassigned = true;
            this.comboBoxClinicPicker.Location = new System.Drawing.Point(60, 33);
            this.comboBoxClinicPicker.Name = "comboBoxClinicPicker";
            this.comboBoxClinicPicker.Size = new System.Drawing.Size(200, 21);
            this.comboBoxClinicPicker.TabIndex = 5;
            this.comboBoxClinicPicker.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxClinicPicker_SelectionChangeCommitted);
            // 
            // listBoxExclusionDays
            // 
            this.listBoxExclusionDays.Location = new System.Drawing.Point(67, 122);
            this.listBoxExclusionDays.Name = "listBoxExclusionDays";
            this.listBoxExclusionDays.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
            this.listBoxExclusionDays.Size = new System.Drawing.Size(120, 95);
            this.listBoxExclusionDays.TabIndex = 6;
            this.listBoxExclusionDays.Text = "listBoxExclusionDays";
            // 
            // listBoxExclusionDates
            // 
            this.listBoxExclusionDates.Cursor = System.Windows.Forms.Cursors.Default;
            this.listBoxExclusionDates.Location = new System.Drawing.Point(193, 122);
            this.listBoxExclusionDates.Name = "listBoxExclusionDates";
            this.listBoxExclusionDates.Size = new System.Drawing.Size(120, 316);
            this.listBoxExclusionDates.TabIndex = 7;
            this.listBoxExclusionDates.Text = "listBoxExclusionDates";
            // 
            // labelExclusionDays
            // 
            this.labelExclusionDays.Location = new System.Drawing.Point(64, 100);
            this.labelExclusionDays.Name = "labelExclusionDays";
            this.labelExclusionDays.Size = new System.Drawing.Size(123, 18);
            this.labelExclusionDays.TabIndex = 8;
            this.labelExclusionDays.Text = "Exclusion Days";
            this.labelExclusionDays.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelExclusionDates
            // 
            this.labelExclusionDates.Location = new System.Drawing.Point(190, 100);
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
            this.butDelete.Location = new System.Drawing.Point(319, 184);
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
            this.butAdd.Location = new System.Drawing.Point(319, 154);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(75, 24);
            this.butAdd.TabIndex = 11;
            this.butAdd.Text = "&Add";
            this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
            // 
            // labelUseHQMessage
            // 
            this.labelUseHQMessage.ForeColor = System.Drawing.Color.Firebrick;
            this.labelUseHQMessage.Location = new System.Drawing.Point(190, 69);
            this.labelUseHQMessage.Name = "labelUseHQMessage";
            this.labelUseHQMessage.Size = new System.Drawing.Size(175, 23);
            this.labelUseHQMessage.TabIndex = 12;
            this.labelUseHQMessage.Text = "Using HQ Settings";
            this.labelUseHQMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelUseHQMessage.Visible = false;
            // 
            // checkShowPastDates
            // 
            this.checkShowPastDates.Location = new System.Drawing.Point(319, 124);
            this.checkShowPastDates.Name = "checkShowPastDates";
            this.checkShowPastDates.Size = new System.Drawing.Size(161, 24);
            this.checkShowPastDates.TabIndex = 13;
            this.checkShowPastDates.Text = "Show Past Dates";
            this.checkShowPastDates.UseVisualStyleBackColor = true;
            this.checkShowPastDates.CheckedChanged += new System.EventHandler(this.checkShowPastDates_CheckedChanged);
            // 
            // FormAutoCommExclusionDays
            // 
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(482, 503);
            this.Controls.Add(this.labelUseHQMessage);
            this.Controls.Add(this.checkShowPastDates);
            this.Controls.Add(this.butAdd);
            this.Controls.Add(this.butDelete);
            this.Controls.Add(this.labelExclusionDates);
            this.Controls.Add(this.listBoxExclusionDates);
            this.Controls.Add(this.listBoxExclusionDays);
            this.Controls.Add(this.labelExclusionDays);
            this.Controls.Add(this.comboBoxClinicPicker);
            this.Controls.Add(this.checkUseHQ);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.butCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAutoCommExclusionDays";
            this.Text = "Auto Comm Exclusion Days";
            this.Load += new System.EventHandler(this.FormAutoCommExclusionDays_Load);
            this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
        private System.Windows.Forms.CheckBox checkUseHQ;
        private UI.ComboBoxClinicPicker comboBoxClinicPicker;
        private UI.ListBoxOD listBoxExclusionDays;
        private UI.ListBoxOD listBoxExclusionDates;
        private System.Windows.Forms.Label labelExclusionDays;
        private System.Windows.Forms.Label labelExclusionDates;
        private UI.Button butDelete;
        private UI.Button butAdd;
        private System.Windows.Forms.Label labelUseHQMessage;
        private System.Windows.Forms.CheckBox checkShowPastDates;
    }
}