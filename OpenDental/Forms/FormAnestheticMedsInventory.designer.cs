namespace OpenDental{
	partial class FormAnestheticMedsInventory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAnestheticMedsInventory));
			this.groupAnestheticMeds = new System.Windows.Forms.GroupBox();
			this.labelIntakeNewMeds = new System.Windows.Forms.Label();
			this.butAnesthMedIntake = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAddAnesthMeds = new OpenDental.UI.Button();
			this.gridAnesthMedsInventory = new OpenDental.UI.ODGrid();
			this.label1 = new System.Windows.Forms.Label();
			this.groupAnestheticMeds.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupAnestheticMeds
			// 
			this.groupAnestheticMeds.Controls.Add(this.label1);
			this.groupAnestheticMeds.Controls.Add(this.labelIntakeNewMeds);
			this.groupAnestheticMeds.Controls.Add(this.butAnesthMedIntake);
			this.groupAnestheticMeds.Controls.Add(this.butClose);
			this.groupAnestheticMeds.Controls.Add(this.butCancel);
			this.groupAnestheticMeds.Controls.Add(this.butAddAnesthMeds);
			this.groupAnestheticMeds.Controls.Add(this.gridAnesthMedsInventory);
			this.groupAnestheticMeds.Location = new System.Drawing.Point(20, 20);
			this.groupAnestheticMeds.Name = "groupAnestheticMeds";
			this.groupAnestheticMeds.Size = new System.Drawing.Size(705, 331);
			this.groupAnestheticMeds.TabIndex = 5;
			this.groupAnestheticMeds.TabStop = false;
			this.groupAnestheticMeds.Text = "Anesthetic Medications";
			// 
			// labelIntakeNewMeds
			// 
			this.labelIntakeNewMeds.Location = new System.Drawing.Point(246, 276);
			this.labelIntakeNewMeds.Name = "labelIntakeNewMeds";
			this.labelIntakeNewMeds.Size = new System.Drawing.Size(272, 26);
			this.labelIntakeNewMeds.TabIndex = 147;
			this.labelIntakeNewMeds.Text = "This button should only be used after anesthetic  medications are added to the li" +
				"st above";
			// 
			// butAnesthMedIntake
			// 
			this.butAnesthMedIntake.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAnesthMedIntake.Autosize = true;
			this.butAnesthMedIntake.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAnesthMedIntake.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAnesthMedIntake.CornerRadius = 4F;
			this.butAnesthMedIntake.Image = global::OpenDental.Properties.Resources.Add;
			this.butAnesthMedIntake.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAnesthMedIntake.Location = new System.Drawing.Point(104, 276);
			this.butAnesthMedIntake.Name = "butAnesthMedIntake";
			this.butAnesthMedIntake.Size = new System.Drawing.Size(136, 26);
			this.butAnesthMedIntake.TabIndex = 146;
			this.butAnesthMedIntake.Text = "Intake new meds";
			this.butAnesthMedIntake.UseVisualStyleBackColor = true;
			this.butAnesthMedIntake.Click += new System.EventHandler(this.butAnesthMedIntake_Click);
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClose.Location = new System.Drawing.Point(596, 276);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(90, 26);
			this.butClose.TabIndex = 145;
			this.butClose.Text = "Save and Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.CornerRadius = 4F;
			this.butCancel.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCancel.Location = new System.Drawing.Point(524, 276);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(66, 26);
			this.butCancel.TabIndex = 144;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAddAnesthMeds
			// 
			this.butAddAnesthMeds.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAddAnesthMeds.Autosize = true;
			this.butAddAnesthMeds.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddAnesthMeds.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddAnesthMeds.CornerRadius = 4F;
			this.butAddAnesthMeds.Image = global::OpenDental.Properties.Resources.Add;
			this.butAddAnesthMeds.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAnesthMeds.Location = new System.Drawing.Point(11, 49);
			this.butAddAnesthMeds.Name = "butAddAnesthMeds";
			this.butAddAnesthMeds.Size = new System.Drawing.Size(82, 26);
			this.butAddAnesthMeds.TabIndex = 76;
			this.butAddAnesthMeds.Text = "Add New";
			this.butAddAnesthMeds.UseVisualStyleBackColor = true;
			this.butAddAnesthMeds.Click += new System.EventHandler(this.butAddAnesthMeds_Click);
			// 
			// gridAnesthMedsInventory
			// 
			this.gridAnesthMedsInventory.HScrollVisible = false;
			this.gridAnesthMedsInventory.Location = new System.Drawing.Point(104, 33);
			this.gridAnesthMedsInventory.Name = "gridAnesthMedsInventory";
			this.gridAnesthMedsInventory.ScrollValue = 0;
			this.gridAnesthMedsInventory.Size = new System.Drawing.Size(580, 206);
			this.gridAnesthMedsInventory.TabIndex = 4;
			this.gridAnesthMedsInventory.Title = "Anesthetic Medication Inventory";
			this.gridAnesthMedsInventory.TranslationName = null;
			this.gridAnesthMedsInventory.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAnesthMedsInventory_CellDoubleClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(277, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(203, 13);
			this.label1.TabIndex = 148;
			this.label1.Text = "Double-click to adjust inventory quantities";
			// 
			// FormAnestheticMedsInventory
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(754, 381);
			this.Controls.Add(this.groupAnestheticMeds);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAnestheticMedsInventory";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Anesthetic Medication Inventory";
			this.Load += new System.EventHandler(this.FormAnestheticMedsInventory_Load);
			this.groupAnestheticMeds.ResumeLayout(false);
			this.groupAnestheticMeds.PerformLayout();
			this.ResumeLayout(false);

	}

		#endregion

		private OpenDental.UI.ODGrid gridAnesthMedsInventory;
		private System.Windows.Forms.GroupBox groupAnestheticMeds;
		private OpenDental.UI.Button butAddAnesthMeds;
		private System.Windows.Forms.Label labelIntakeNewMeds;
		private OpenDental.UI.Button butAnesthMedIntake;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;

	}
}