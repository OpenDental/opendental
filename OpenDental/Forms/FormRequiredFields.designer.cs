namespace OpenDental {
	partial class FormRequiredFields {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRequiredFields));
			this.listAvailableFields = new OpenDental.UI.ListBoxOD();
			this.labelExplanation = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.labelFieldType = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkMedicaidLength = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.gridConditions = new OpenDental.UI.GridOD();
			this.butRight = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.comboFieldTypes = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// listAvailableFields
			// 
			this.listAvailableFields.Location = new System.Drawing.Point(12, 74);
			this.listAvailableFields.Name = "listAvailableFields";
			this.listAvailableFields.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listAvailableFields.Size = new System.Drawing.Size(158, 511);
			this.listAvailableFields.TabIndex = 0;
			this.listAvailableFields.TabStop = false;
			// 
			// labelExplanation
			// 
			this.labelExplanation.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelExplanation.Location = new System.Drawing.Point(214, 526);
			this.labelExplanation.Name = "labelExplanation";
			this.labelExplanation.Size = new System.Drawing.Size(415, 15);
			this.labelExplanation.TabIndex = 61;
			this.labelExplanation.Text = "Field must not be blank";
			// 
			// label1
			// 
			this.label1.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label1.Location = new System.Drawing.Point(12, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(158, 15);
			this.label1.TabIndex = 63;
			this.label1.Text = "Available Fields";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelFieldType
			// 
			this.labelFieldType.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelFieldType.Location = new System.Drawing.Point(12, 14);
			this.labelFieldType.Name = "labelFieldType";
			this.labelFieldType.Size = new System.Drawing.Size(158, 15);
			this.labelFieldType.TabIndex = 65;
			this.labelFieldType.Text = "Field Type";
			this.labelFieldType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label2.Location = new System.Drawing.Point(476, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(393, 15);
			this.label2.TabIndex = 70;
			this.label2.Text = "Limit Requirement to only apply if:";
			// 
			// checkMedicaidLength
			// 
			this.checkMedicaidLength.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMedicaidLength.Location = new System.Drawing.Point(217, 545);
			this.checkMedicaidLength.Name = "checkMedicaidLength";
			this.checkMedicaidLength.Size = new System.Drawing.Size(421, 26);
			this.checkMedicaidLength.TabIndex = 216;
			this.checkMedicaidLength.Text = "Validate the number of digits in MedicaidID to be correct for that state";
			this.checkMedicaidLength.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkMedicaidLength.Visible = false;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(788, 293);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 26);
			this.butDelete.TabIndex = 69;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(176, 235);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 24);
			this.butLeft.TabIndex = 68;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(707, 293);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 62;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridConditions
			// 
			this.gridConditions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridConditions.Location = new System.Drawing.Point(474, 50);
			this.gridConditions.Name = "gridConditions";
			this.gridConditions.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridConditions.Size = new System.Drawing.Size(397, 240);
			this.gridConditions.TabIndex = 60;
			this.gridConditions.Title = "Conditions";
			this.gridConditions.TranslationName = "FormRequiredFields";
			this.gridConditions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridConditions_CellDoubleClick);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(176, 195);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 24);
			this.butRight.TabIndex = 59;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(217, 32);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(236, 484);
			this.gridMain.TabIndex = 56;
			this.gridMain.Title = "Required Fields";
			this.gridMain.TranslationName = "FormRequiredFields";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(796, 559);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 5;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboFieldTypes
			// 
			this.comboFieldTypes.FormattingEnabled = true;
			this.comboFieldTypes.Location = new System.Drawing.Point(15, 32);
			this.comboFieldTypes.Name = "comboFieldTypes";
			this.comboFieldTypes.Size = new System.Drawing.Size(121, 21);
			this.comboFieldTypes.TabIndex = 217;
			this.comboFieldTypes.SelectedIndexChanged += new System.EventHandler(this.comboFieldTypes_SelectedIndexChanged);
			// 
			// FormRequiredFields
			// 
			this.ClientSize = new System.Drawing.Size(883, 595);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.comboFieldTypes);
			this.Controls.Add(this.checkMedicaidLength);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.labelFieldType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.labelExplanation);
			this.Controls.Add(this.gridConditions);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.listAvailableFields);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRequiredFields";
			this.Text = "Required Fields";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRequiredFields_FormClosing);
			this.Load += new System.EventHandler(this.FormRequiredFields_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butClose;
		private UI.Button butRight;
		private OpenDental.UI.ListBoxOD listAvailableFields;
		private UI.GridOD gridMain;
		private UI.GridOD gridConditions;
		private System.Windows.Forms.Label labelExplanation;
		private UI.Button butAdd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelFieldType;
		private UI.Button butLeft;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkMedicaidLength;
		private System.Windows.Forms.ComboBox comboFieldTypes;
	}
}