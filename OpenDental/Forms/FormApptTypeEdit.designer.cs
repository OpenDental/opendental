namespace OpenDental{
	partial class FormApptTypeEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptTypeEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelColor = new System.Windows.Forms.Label();
			this.butColorClear = new OpenDental.UI.Button();
			this.butColor = new System.Windows.Forms.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.checkIsHidden = new OpenDental.UI.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butSlider = new System.Windows.Forms.Button();
			this.tbTime = new OpenDental.TableTimeBar();
			this.labelTime = new System.Windows.Forms.Label();
			this.listBoxProcCodes = new OpenDental.UI.ListBox();
			this.butAdd = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.labelTreatmentPlannedProcedures = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.textTime = new System.Windows.Forms.TextBox();
			this.listBoxProcCodesRequired = new OpenDental.UI.ListBox();
			this.butAddRequired = new OpenDental.UI.Button();
			this.butRemoveRequired = new OpenDental.UI.Button();
			this.listBoxBlockoutTypes = new OpenDental.UI.ListBox();
			this.labelBlockoutTypes = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.radioButtonAll = new System.Windows.Forms.RadioButton();
			this.radioButtonAtLeastOne = new System.Windows.Forms.RadioButton();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(549, 500);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 16;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(630, 500);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 17;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(29, 48);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(95, 20);
			this.labelColor.TabIndex = 182;
			this.labelColor.Text = "Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorClear
			// 
			this.butColorClear.Location = new System.Drawing.Point(155, 45);
			this.butColorClear.Name = "butColorClear";
			this.butColorClear.Size = new System.Drawing.Size(53, 24);
			this.butColorClear.TabIndex = 2;
			this.butColorClear.Text = "None";
			this.butColorClear.Click += new System.EventHandler(this.butColorClear_Click);
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(129, 48);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(20, 20);
			this.butColor.TabIndex = 1;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(29, 20);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(95, 20);
			this.labelName.TabIndex = 184;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(129, 20);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(272, 20);
			this.textName.TabIndex = 0;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Checked = true;
			this.checkIsHidden.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIsHidden.Location = new System.Drawing.Point(51, 72);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsHidden.Size = new System.Drawing.Size(91, 18);
			this.checkIsHidden.TabIndex = 3;
			this.checkIsHidden.Text = "Hidden";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(28, 500);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butSlider
			// 
			this.butSlider.BackColor = System.Drawing.SystemColors.ControlDark;
			this.butSlider.Location = new System.Drawing.Point(9, 96);
			this.butSlider.Name = "butSlider";
			this.butSlider.Size = new System.Drawing.Size(12, 15);
			this.butSlider.TabIndex = 188;
			this.butSlider.TabStop = false;
			this.butSlider.UseVisualStyleBackColor = false;
			this.butSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseDown);
			this.butSlider.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseMove);
			this.butSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseUp);
			// 
			// tbTime
			// 
			this.tbTime.BackColor = System.Drawing.SystemColors.Window;
			this.tbTime.Location = new System.Drawing.Point(7, 12);
			this.tbTime.Name = "tbTime";
			this.tbTime.ScrollValue = 150;
			this.tbTime.SelectedIndices = new int[0];
			this.tbTime.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbTime.Size = new System.Drawing.Size(15, 561);
			this.tbTime.TabIndex = 187;
			this.tbTime.TabStop = false;
			this.tbTime.CellClicked += new OpenDental.ContrTable.CellEventHandler(this.tbTime_CellClicked);
			// 
			// labelTime
			// 
			this.labelTime.Location = new System.Drawing.Point(29, 95);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(95, 20);
			this.labelTime.TabIndex = 190;
			this.labelTime.Text = "Time";
			this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxProcCodes
			// 
			this.listBoxProcCodes.Location = new System.Drawing.Point(129, 126);
			this.listBoxProcCodes.Name = "listBoxProcCodes";
			this.listBoxProcCodes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxProcCodes.Size = new System.Drawing.Size(120, 160);
			this.listBoxProcCodes.TabIndex = 6;
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(252, 126);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(65, 24);
			this.butAdd.TabIndex = 7;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butRemove
			// 
			this.butRemove.Location = new System.Drawing.Point(252, 154);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(65, 24);
			this.butRemove.TabIndex = 8;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// labelTreatmentPlannedProcedures
			// 
			this.labelTreatmentPlannedProcedures.Location = new System.Drawing.Point(29, 126);
			this.labelTreatmentPlannedProcedures.Name = "labelTreatmentPlannedProcedures";
			this.labelTreatmentPlannedProcedures.Size = new System.Drawing.Size(99, 52);
			this.labelTreatmentPlannedProcedures.TabIndex = 195;
			this.labelTreatmentPlannedProcedures.Text = "Procedures to Add to Appointment";
			this.labelTreatmentPlannedProcedures.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(282, 93);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(65, 24);
			this.butClear.TabIndex = 5;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// textTime
			// 
			this.textTime.Location = new System.Drawing.Point(129, 95);
			this.textTime.Name = "textTime";
			this.textTime.ReadOnly = true;
			this.textTime.Size = new System.Drawing.Size(147, 20);
			this.textTime.TabIndex = 4;
			// 
			// listBoxProcCodesRequired
			// 
			this.listBoxProcCodesRequired.Location = new System.Drawing.Point(27, 28);
			this.listBoxProcCodesRequired.Name = "listBoxProcCodesRequired";
			this.listBoxProcCodesRequired.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxProcCodesRequired.Size = new System.Drawing.Size(120, 160);
			this.listBoxProcCodesRequired.TabIndex = 10;
			// 
			// butAddRequired
			// 
			this.butAddRequired.Location = new System.Drawing.Point(153, 28);
			this.butAddRequired.Name = "butAddRequired";
			this.butAddRequired.Size = new System.Drawing.Size(65, 24);
			this.butAddRequired.TabIndex = 11;
			this.butAddRequired.Text = "Add";
			this.butAddRequired.Click += new System.EventHandler(this.butAddRequired_Click);
			// 
			// butRemoveRequired
			// 
			this.butRemoveRequired.Location = new System.Drawing.Point(153, 56);
			this.butRemoveRequired.Name = "butRemoveRequired";
			this.butRemoveRequired.Size = new System.Drawing.Size(65, 24);
			this.butRemoveRequired.TabIndex = 12;
			this.butRemoveRequired.Text = "Remove";
			this.butRemoveRequired.Click += new System.EventHandler(this.butRemoveRequired_Click);
			// 
			// listBoxBlockoutTypes
			// 
			this.listBoxBlockoutTypes.Location = new System.Drawing.Point(128, 296);
			this.listBoxBlockoutTypes.Name = "listBoxBlockoutTypes";
			this.listBoxBlockoutTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxBlockoutTypes.Size = new System.Drawing.Size(120, 160);
			this.listBoxBlockoutTypes.TabIndex = 9;
			// 
			// labelBlockoutTypes
			// 
			this.labelBlockoutTypes.Location = new System.Drawing.Point(28, 296);
			this.labelBlockoutTypes.Name = "labelBlockoutTypes";
			this.labelBlockoutTypes.Size = new System.Drawing.Size(99, 37);
			this.labelBlockoutTypes.TabIndex = 207;
			this.labelBlockoutTypes.Text = "Allowed Blockout Types";
			this.labelBlockoutTypes.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.radioButtonAll);
			this.groupBoxOD1.Controls.Add(this.radioButtonAtLeastOne);
			this.groupBoxOD1.Controls.Add(this.listBoxProcCodesRequired);
			this.groupBoxOD1.Controls.Add(this.butRemoveRequired);
			this.groupBoxOD1.Controls.Add(this.butAddRequired);
			this.groupBoxOD1.Location = new System.Drawing.Point(420, 96);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(251, 244);
			this.groupBoxOD1.TabIndex = 211;
			this.groupBoxOD1.Text = "Required Procedures";
			// 
			// radioButtonAll
			// 
			this.radioButtonAll.Location = new System.Drawing.Point(27, 219);
			this.radioButtonAll.Name = "radioButtonAll";
			this.radioButtonAll.Size = new System.Drawing.Size(87, 18);
			this.radioButtonAll.TabIndex = 14;
			this.radioButtonAll.TabStop = true;
			this.radioButtonAll.Text = "All";
			this.radioButtonAll.UseVisualStyleBackColor = true;
			// 
			// radioButtonAtLeastOne
			// 
			this.radioButtonAtLeastOne.Location = new System.Drawing.Point(27, 200);
			this.radioButtonAtLeastOne.Name = "radioButtonAtLeastOne";
			this.radioButtonAtLeastOne.Size = new System.Drawing.Size(120, 18);
			this.radioButtonAtLeastOne.TabIndex = 13;
			this.radioButtonAtLeastOne.TabStop = true;
			this.radioButtonAtLeastOne.Text = "At Least One";
			this.radioButtonAtLeastOne.UseVisualStyleBackColor = true;
			// 
			// FormApptTypeEdit
			// 
			this.ClientSize = new System.Drawing.Size(717, 536);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.labelBlockoutTypes);
			this.Controls.Add(this.listBoxBlockoutTypes);
			this.Controls.Add(this.textTime);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.labelTreatmentPlannedProcedures);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.listBoxProcCodes);
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.butSlider);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.labelColor);
			this.Controls.Add(this.butColorClear);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.tbTime);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptTypeEdit";
			this.Text = "Appointment Type Edit";
			this.Load += new System.EventHandler(this.FormApptTypeEdit_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.Label labelColor;
		private UI.Button butColorClear;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textName;
		private OpenDental.UI.CheckBox checkIsHidden;
		private UI.Button butDelete;
		private System.Windows.Forms.Button butSlider;
		private TableTimeBar tbTime;
		private System.Windows.Forms.Label labelTime;
		private OpenDental.UI.ListBox listBoxProcCodes;
		private UI.Button butAdd;
		private UI.Button butRemove;
		private System.Windows.Forms.Label labelTreatmentPlannedProcedures;
		private UI.Button butClear;
		private System.Windows.Forms.TextBox textTime;
		private UI.ListBox listBoxProcCodesRequired;
        private UI.Button butAddRequired;
        private UI.Button butRemoveRequired;
        private UI.ListBox listBoxBlockoutTypes;
        private System.Windows.Forms.Label labelBlockoutTypes;
		private UI.GroupBox groupBoxOD1;
		private System.Windows.Forms.RadioButton radioButtonAll;
		private System.Windows.Forms.RadioButton radioButtonAtLeastOne;
	}
}