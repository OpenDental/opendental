namespace OpenDental

{
	partial class FormAnesthMedDelDose
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAnesthMedDelDose));
			this.textDoseTimeStamp = new System.Windows.Forms.TextBox();
			this.groupBoxAnesthMedDelete = new System.Windows.Forms.GroupBox();
			this.labelQtyWasted = new System.Windows.Forms.Label();
			this.textQtyWasted = new System.Windows.Forms.TextBox();
			this.labelDoseTimeStamp = new System.Windows.Forms.Label();
			this.textAnesthMedName = new System.Windows.Forms.TextBox();
			this.textDose = new System.Windows.Forms.TextBox();
			this.labelDose = new System.Windows.Forms.Label();
			this.labelDeleteInstrux = new System.Windows.Forms.Label();
			this.butDelAnesthMeds = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupDelMed = new System.Windows.Forms.GroupBox();
			this.groupBoxAnesthMedDelete.SuspendLayout();
			this.groupDelMed.SuspendLayout();
			this.SuspendLayout();
			// 
			// textDoseTimeStamp
			// 
			this.textDoseTimeStamp.Location = new System.Drawing.Point(15, 72);
			this.textDoseTimeStamp.Name = "textDoseTimeStamp";
			this.textDoseTimeStamp.Size = new System.Drawing.Size(137, 20);
			this.textDoseTimeStamp.TabIndex = 9;
			this.textDoseTimeStamp.TextChanged += new System.EventHandler(this.textDate_TextChanged);
			// 
			// groupBoxAnesthMedDelete
			// 
			this.groupBoxAnesthMedDelete.Controls.Add(this.labelQtyWasted);
			this.groupBoxAnesthMedDelete.Controls.Add(this.textQtyWasted);
			this.groupBoxAnesthMedDelete.Controls.Add(this.labelDoseTimeStamp);
			this.groupBoxAnesthMedDelete.Controls.Add(this.textAnesthMedName);
			this.groupBoxAnesthMedDelete.Controls.Add(this.textDose);
			this.groupBoxAnesthMedDelete.Controls.Add(this.labelDose);
			this.groupBoxAnesthMedDelete.Controls.Add(this.textDoseTimeStamp);
			this.groupBoxAnesthMedDelete.Location = new System.Drawing.Point(42, 33);
			this.groupBoxAnesthMedDelete.Name = "groupBoxAnesthMedDelete";
			this.groupBoxAnesthMedDelete.Size = new System.Drawing.Size(293, 118);
			this.groupBoxAnesthMedDelete.TabIndex = 139;
			this.groupBoxAnesthMedDelete.TabStop = false;
			this.groupBoxAnesthMedDelete.Text = "Anesthetic Medication";
			this.groupBoxAnesthMedDelete.Enter += new System.EventHandler(this.groupBoxAnesthMedDelete_Enter);
			// 
			// labelQtyWasted
			// 
			this.labelQtyWasted.AutoSize = true;
			this.labelQtyWasted.Location = new System.Drawing.Point(197, 53);
			this.labelQtyWasted.Name = "labelQtyWasted";
			this.labelQtyWasted.Size = new System.Drawing.Size(83, 13);
			this.labelQtyWasted.TabIndex = 143;
			this.labelQtyWasted.Text = "Qty wasted (mL)";
			// 
			// textQtyWasted
			// 
			this.textQtyWasted.Location = new System.Drawing.Point(203, 72);
			this.textQtyWasted.Name = "textQtyWasted";
			this.textQtyWasted.Size = new System.Drawing.Size(65, 20);
			this.textQtyWasted.TabIndex = 1;
			// 
			// labelDoseTimeStamp
			// 
			this.labelDoseTimeStamp.AutoSize = true;
			this.labelDoseTimeStamp.Location = new System.Drawing.Point(12, 53);
			this.labelDoseTimeStamp.Name = "labelDoseTimeStamp";
			this.labelDoseTimeStamp.Size = new System.Drawing.Size(63, 13);
			this.labelDoseTimeStamp.TabIndex = 141;
			this.labelDoseTimeStamp.Text = "Time Stamp";
			// 
			// textAnesthMedName
			// 
			this.textAnesthMedName.Location = new System.Drawing.Point(15, 20);
			this.textAnesthMedName.Name = "textAnesthMedName";
			this.textAnesthMedName.ReadOnly = true;
			this.textAnesthMedName.Size = new System.Drawing.Size(137, 20);
			this.textAnesthMedName.TabIndex = 5;
			// 
			// textDose
			// 
			this.textDose.Location = new System.Drawing.Point(203, 20);
			this.textDose.Name = "textDose";
			this.textDose.Size = new System.Drawing.Size(65, 20);
			this.textDose.TabIndex = 0;
			this.textDose.TextChanged += new System.EventHandler(this.textDose_TextChanged);
			// 
			// labelDose
			// 
			this.labelDose.AutoSize = true;
			this.labelDose.Location = new System.Drawing.Point(208, 0);
			this.labelDose.Name = "labelDose";
			this.labelDose.Size = new System.Drawing.Size(55, 13);
			this.labelDose.TabIndex = 4;
			this.labelDose.Text = "Dose (mL)";
			// 
			// labelDeleteInstrux
			// 
			this.labelDeleteInstrux.AutoSize = true;
			this.labelDeleteInstrux.Location = new System.Drawing.Point(16, 17);
			this.labelDeleteInstrux.Name = "labelDeleteInstrux";
			this.labelDeleteInstrux.Size = new System.Drawing.Size(362, 13);
			this.labelDeleteInstrux.TabIndex = 141;
			this.labelDeleteInstrux.Text = "Click \'Delete\' to delete this dose, or enter a quantity to waste and click \'OK\' ";
			// 
			// butDelAnesthMeds
			// 
			this.butDelAnesthMeds.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDelAnesthMeds.Autosize = true;
			this.butDelAnesthMeds.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDelAnesthMeds.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDelAnesthMeds.CornerRadius = 4F;
			this.butDelAnesthMeds.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butDelAnesthMeds.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelAnesthMeds.Location = new System.Drawing.Point(47, 184);
			this.butDelAnesthMeds.Name = "butDelAnesthMeds";
			this.butDelAnesthMeds.Size = new System.Drawing.Size(82, 26);
			this.butDelAnesthMeds.TabIndex = 2;
			this.butDelAnesthMeds.Text = "Delete";
			this.butDelAnesthMeds.UseVisualStyleBackColor = true;
			this.butDelAnesthMeds.Click += new System.EventHandler(this.butDelAnesthMeds_Click);
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butOK.Autosize = true;
			this.butOK.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOK.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOK.CornerRadius = 4F;
			this.butOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butOK.Location = new System.Drawing.Point(284, 184);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(72, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butClose_Click);
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
			this.butCancel.Location = new System.Drawing.Point(209, 184);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(66, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupDelMed
			// 
			this.groupDelMed.Controls.Add(this.groupBoxAnesthMedDelete);
			this.groupDelMed.Location = new System.Drawing.Point(13, 17);
			this.groupDelMed.Name = "groupDelMed";
			this.groupDelMed.Size = new System.Drawing.Size(378, 209);
			this.groupDelMed.TabIndex = 142;
			this.groupDelMed.TabStop = false;
			this.groupDelMed.Text = "Click \'Delete\' to delete this dose, or enter a quantity to waste and click \'OK\' ";
			// 
			// FormAnesthMedDelDose
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(423, 252);
			this.Controls.Add(this.labelDeleteInstrux);
			this.Controls.Add(this.butDelAnesthMeds);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupDelMed);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAnesthMedDelDose";
			this.Text = "Delete/Waste Anesthetic Medication Dose";
			this.Load += new System.EventHandler(this.FormAnesthMedDelDose_Load);
			this.groupBoxAnesthMedDelete.ResumeLayout(false);
			this.groupBoxAnesthMedDelete.PerformLayout();
			this.groupDelMed.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textDoseTimeStamp;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.GroupBox groupBoxAnesthMedDelete;
		private System.Windows.Forms.TextBox textDose;
		private System.Windows.Forms.Label labelDose;
		private System.Windows.Forms.Label labelDoseTimeStamp;
		private System.Windows.Forms.TextBox textAnesthMedName;
		private OpenDental.UI.Button butDelAnesthMeds;
		private System.Windows.Forms.Label labelDeleteInstrux;
		private System.Windows.Forms.TextBox textQtyWasted;
		private System.Windows.Forms.Label labelQtyWasted;
		private System.Windows.Forms.GroupBox groupDelMed;
        
	}
}