namespace OpenDental {
	partial class FormPayPlanTemplates {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanTemplates));
			this.comboBoxClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridPayPlanTemplates = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butAddTemplate = new OpenDental.UI.Button();
			this.checkShowHidden = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// comboBoxClinic
			// 
			this.comboBoxClinic.IncludeAll = true;
			this.comboBoxClinic.Location = new System.Drawing.Point(26, 19);
			this.comboBoxClinic.Name = "comboBoxClinic";
			this.comboBoxClinic.Size = new System.Drawing.Size(201, 21);
			this.comboBoxClinic.TabIndex = 208;
			this.comboBoxClinic.SelectionChangeCommitted += new System.EventHandler(this.comboBoxClinic_SelectionChangeCommitted);
			// 
			// gridPayPlanTemplates
			// 
			this.gridPayPlanTemplates.Location = new System.Drawing.Point(26, 55);
			this.gridPayPlanTemplates.Name = "gridPayPlanTemplates";
			this.gridPayPlanTemplates.Size = new System.Drawing.Size(900, 401);
			this.gridPayPlanTemplates.TabIndex = 4;
			this.gridPayPlanTemplates.Title = "Pay Plan Templates";
			this.gridPayPlanTemplates.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPayPlanTemplates_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(885, 480);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAddTemplate
			// 
			this.butAddTemplate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTemplate.Location = new System.Drawing.Point(853, 19);
			this.butAddTemplate.Name = "butAddTemplate";
			this.butAddTemplate.Size = new System.Drawing.Size(73, 24);
			this.butAddTemplate.TabIndex = 209;
			this.butAddTemplate.Text = "Add";
			this.butAddTemplate.UseVisualStyleBackColor = true;
			this.butAddTemplate.Visible = false;
			this.butAddTemplate.Click += new System.EventHandler(this.butAddTemplate_Click);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Location = new System.Drawing.Point(233, 19);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(103, 21);
			this.checkShowHidden.TabIndex = 210;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.Visible = false;
			this.checkShowHidden.Click += new System.EventHandler(this.checkShowHidden_Click);
			// 
			// FormPayPlanTemplates
			// 
			this.ClientSize = new System.Drawing.Size(972, 516);
			this.Controls.Add(this.checkShowHidden);
			this.Controls.Add(this.butAddTemplate);
			this.Controls.Add(this.comboBoxClinic);
			this.Controls.Add(this.gridPayPlanTemplates);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayPlanTemplates";
			this.Text = "Pay Plan Templates";
			this.Load += new System.EventHandler(this.FormPayPlanTemplates_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private UI.GridOD gridPayPlanTemplates;
		private UI.ComboBoxClinicPicker comboBoxClinic;
		private UI.Button butAddTemplate;
		private UI.CheckBox checkShowHidden;
	}
}