
namespace OpenDental {
	partial class FormActivityLog {
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
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboBoxClinicMulti = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkDistinctLogGuid = new System.Windows.Forms.CheckBox();
			this.groupBoxFilters = new OpenDental.UI.GroupBoxOD();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.labelEserviceType = new System.Windows.Forms.Label();
			this.labelPatNum = new System.Windows.Forms.Label();
			this.labelEserviceAction = new System.Windows.Forms.Label();
			this.labelLogGuid = new System.Windows.Forms.Label();
			this.textLogGuid = new System.Windows.Forms.TextBox();
			this.comboBoxTypes = new OpenDental.UI.ComboBoxOD();
			this.comboBoxActions = new OpenDental.UI.ComboBoxOD();
			this.butClose = new OpenDental.UI.Button();
			this.labelRows = new System.Windows.Forms.Label();
			this.groupBoxFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.SystemColors.Control;
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.Location = new System.Drawing.Point(0, 11);
			this.datePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(453, 22);
			this.datePicker.TabIndex = 50;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(991, 11);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(77, 23);
			this.butRefresh.TabIndex = 52;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboBoxClinicMulti
			// 
			this.comboBoxClinicMulti.IncludeAll = true;
			this.comboBoxClinicMulti.IncludeUnassigned = true;
			this.comboBoxClinicMulti.Location = new System.Drawing.Point(788, 11);
			this.comboBoxClinicMulti.Name = "comboBoxClinicMulti";
			this.comboBoxClinicMulti.SelectionModeMulti = true;
			this.comboBoxClinicMulti.Size = new System.Drawing.Size(197, 22);
			this.comboBoxClinicMulti.TabIndex = 51;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 94);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1056, 507);
			this.gridMain.TabIndex = 53;
			this.gridMain.Title = "eService Activity Log";
			// 
			// checkDistinctLogGuid
			// 
			this.checkDistinctLogGuid.AutoSize = true;
			this.checkDistinctLogGuid.Location = new System.Drawing.Point(941, 20);
			this.checkDistinctLogGuid.Name = "checkDistinctLogGuid";
			this.checkDistinctLogGuid.Size = new System.Drawing.Size(113, 17);
			this.checkDistinctLogGuid.TabIndex = 54;
			this.checkDistinctLogGuid.Text = "Group By LogGuid";
			this.checkDistinctLogGuid.UseVisualStyleBackColor = true;
			this.checkDistinctLogGuid.CheckedChanged += new System.EventHandler(this.checkDistinctLogGuid_CheckedChanged);
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxFilters.Controls.Add(this.textPatNum);
			this.groupBoxFilters.Controls.Add(this.labelEserviceType);
			this.groupBoxFilters.Controls.Add(this.labelPatNum);
			this.groupBoxFilters.Controls.Add(this.labelEserviceAction);
			this.groupBoxFilters.Controls.Add(this.checkDistinctLogGuid);
			this.groupBoxFilters.Controls.Add(this.labelLogGuid);
			this.groupBoxFilters.Controls.Add(this.textLogGuid);
			this.groupBoxFilters.Controls.Add(this.comboBoxTypes);
			this.groupBoxFilters.Controls.Add(this.comboBoxActions);
			this.groupBoxFilters.Location = new System.Drawing.Point(12, 39);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.Size = new System.Drawing.Size(1056, 49);
			this.groupBoxFilters.TabIndex = 55;
			this.groupBoxFilters.TabStop = false;
			this.groupBoxFilters.Text = "Filters";
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(692, 18);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(48, 20);
			this.textPatNum.TabIndex = 66;
			this.textPatNum.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			// 
			// labelEserviceType
			// 
			this.labelEserviceType.AutoSize = true;
			this.labelEserviceType.Location = new System.Drawing.Point(6, 22);
			this.labelEserviceType.Name = "labelEserviceType";
			this.labelEserviceType.Size = new System.Drawing.Size(79, 13);
			this.labelEserviceType.TabIndex = 65;
			this.labelEserviceType.Text = "eService Type:";
			// 
			// labelPatNum
			// 
			this.labelPatNum.AutoSize = true;
			this.labelPatNum.Location = new System.Drawing.Point(635, 22);
			this.labelPatNum.Name = "labelPatNum";
			this.labelPatNum.Size = new System.Drawing.Size(51, 13);
			this.labelPatNum.TabIndex = 64;
			this.labelPatNum.Text = "Pat Num:";
			// 
			// labelEserviceAction
			// 
			this.labelEserviceAction.AutoSize = true;
			this.labelEserviceAction.Location = new System.Drawing.Point(271, 22);
			this.labelEserviceAction.Name = "labelEserviceAction";
			this.labelEserviceAction.Size = new System.Drawing.Size(85, 13);
			this.labelEserviceAction.TabIndex = 63;
			this.labelEserviceAction.Text = "eService Action:";
			// 
			// labelLogGuid
			// 
			this.labelLogGuid.AutoSize = true;
			this.labelLogGuid.Location = new System.Drawing.Point(746, 22);
			this.labelLogGuid.Name = "labelLogGuid";
			this.labelLogGuid.Size = new System.Drawing.Size(53, 13);
			this.labelLogGuid.TabIndex = 62;
			this.labelLogGuid.Text = "Log Guid:";
			// 
			// textLogGuid
			// 
			this.textLogGuid.Location = new System.Drawing.Point(805, 18);
			this.textLogGuid.Name = "textLogGuid";
			this.textLogGuid.Size = new System.Drawing.Size(130, 20);
			this.textLogGuid.TabIndex = 61;
			this.textLogGuid.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			// 
			// comboBoxTypes
			// 
			this.comboBoxTypes.Location = new System.Drawing.Point(92, 18);
			this.comboBoxTypes.Name = "comboBoxTypes";
			this.comboBoxTypes.Size = new System.Drawing.Size(160, 21);
			this.comboBoxTypes.TabIndex = 58;
			this.comboBoxTypes.Text = "eServiceType";
			this.comboBoxTypes.SelectionChangeCommitted += new System.EventHandler(this.comboBoxTypes_SelectionChangeCommitted);
			// 
			// comboBoxActions
			// 
			this.comboBoxActions.Location = new System.Drawing.Point(360, 18);
			this.comboBoxActions.Name = "comboBoxActions";
			this.comboBoxActions.Size = new System.Drawing.Size(262, 21);
			this.comboBoxActions.TabIndex = 59;
			this.comboBoxActions.Text = "eServiceAction";
			this.comboBoxActions.SelectionChangeCommitted += new System.EventHandler(this.comboBoxActions_SelectionChangeCommitted);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(990, 607);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 56;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelRows
			// 
			this.labelRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRows.AutoSize = true;
			this.labelRows.Location = new System.Drawing.Point(9, 613);
			this.labelRows.Name = "labelRows";
			this.labelRows.Size = new System.Drawing.Size(72, 13);
			this.labelRows.TabIndex = 57;
			this.labelRows.Text = "Row Count: 0";
			// 
			// FormActivityLog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1080, 636);
			this.Controls.Add(this.labelRows);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupBoxFilters);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.datePicker);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.comboBoxClinicMulti);
			this.Name = "FormActivityLog";
			this.Text = "eService Activity Log";
			this.Load += new System.EventHandler(this.FormActivityLog_Load);
			this.groupBoxFilters.ResumeLayout(false);
			this.groupBoxFilters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.ODDateRangePicker datePicker;
		private UI.Button butRefresh;
		private UI.ComboBoxClinicPicker comboBoxClinicMulti;
		private UI.GridOD gridMain;
		private System.Windows.Forms.CheckBox checkDistinctLogGuid;
		private UI.GroupBoxOD groupBoxFilters;
		private UI.Button butClose;
		private System.Windows.Forms.Label labelRows;
		private UI.ComboBoxOD comboBoxTypes;
		private UI.ComboBoxOD comboBoxActions;
		private System.Windows.Forms.TextBox textLogGuid;
		private System.Windows.Forms.Label labelLogGuid;
		private System.Windows.Forms.Label labelEserviceType;
		private System.Windows.Forms.Label labelPatNum;
		private System.Windows.Forms.Label labelEserviceAction;
		private System.Windows.Forms.TextBox textPatNum;
	}
}