namespace OpenDental {
	partial class FormEhrMedicalOrderLabEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrMedicalOrderLabEdit));
			this.butOK = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.textDateTime = new System.Windows.Forms.TextBox();
			this.labelDateTime = new System.Windows.Forms.Label();
			this.checkIsDiscontinued = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAttach = new System.Windows.Forms.Button();
			this.butRemove = new System.Windows.Forms.Button();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(322, 346);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 11;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(403, 346);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(11, 346);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 12;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(12, 97);
			this.textDescription.Multiline = true;
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(466, 106);
			this.textDescription.TabIndex = 16;
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(11, 73);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(116, 21);
			this.labelDescription.TabIndex = 17;
			this.labelDescription.Text = "Instructions";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDateTime
			// 
			this.textDateTime.Location = new System.Drawing.Point(271, 8);
			this.textDateTime.Name = "textDateTime";
			this.textDateTime.Size = new System.Drawing.Size(207, 20);
			this.textDateTime.TabIndex = 18;
			// 
			// labelDateTime
			// 
			this.labelDateTime.Location = new System.Drawing.Point(153, 10);
			this.labelDateTime.Name = "labelDateTime";
			this.labelDateTime.Size = new System.Drawing.Size(116, 17);
			this.labelDateTime.TabIndex = 19;
			this.labelDateTime.Text = "Date Time";
			this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsDiscontinued
			// 
			this.checkIsDiscontinued.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDiscontinued.Location = new System.Drawing.Point(159, 36);
			this.checkIsDiscontinued.Name = "checkIsDiscontinued";
			this.checkIsDiscontinued.Size = new System.Drawing.Size(126, 18);
			this.checkIsDiscontinued.TabIndex = 23;
			this.checkIsDiscontinued.Text = "Discontinued";
			this.checkIsDiscontinued.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDiscontinued.UseVisualStyleBackColor = true;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 209);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(385, 131);
			this.gridMain.TabIndex = 24;
			this.gridMain.Title = "Attached Lab Panels";
			this.gridMain.TranslationName = "TableAttachedPanels";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAttach
			// 
			this.butAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAttach.Location = new System.Drawing.Point(403, 209);
			this.butAttach.Name = "butAttach";
			this.butAttach.Size = new System.Drawing.Size(75, 24);
			this.butAttach.TabIndex = 25;
			this.butAttach.Text = "Attach";
			this.butAttach.UseVisualStyleBackColor = true;
			this.butAttach.Click += new System.EventHandler(this.butAttach_Click);
			// 
			// butRemove
			// 
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemove.Location = new System.Drawing.Point(403, 238);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 24);
			this.butRemove.TabIndex = 26;
			this.butRemove.Text = "Remove";
			this.butRemove.UseVisualStyleBackColor = true;
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.FormattingEnabled = true;
			this.comboProv.Location = new System.Drawing.Point(271, 59);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(207, 21);
			this.comboProv.TabIndex = 27;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(153, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 17);
			this.label1.TabIndex = 28;
			this.label1.Text = "Provider";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEhrMedicalOrderLabEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 381);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.butAttach);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.checkIsDiscontinued);
			this.Controls.Add(this.textDateTime);
			this.Controls.Add(this.labelDateTime);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrMedicalOrderLabEdit";
			this.Text = "Lab Order Edit";
			this.Load += new System.EventHandler(this.FormMedicalOrderLabEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textDateTime;
		private System.Windows.Forms.Label labelDateTime;
		private System.Windows.Forms.CheckBox checkIsDiscontinued;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butAttach;
		private System.Windows.Forms.Button butRemove;
		private System.Windows.Forms.ComboBox comboProv;
		private System.Windows.Forms.Label label1;
	}
}