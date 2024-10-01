namespace OpenDental {
  partial class FormDoseSpotAssignClinicId {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDoseSpotAssignClinicId));
			this.butSave = new OpenDental.UI.Button();
			this.textClinicId = new System.Windows.Forms.TextBox();
			this.labelUserId = new System.Windows.Forms.Label();
			this.comboClinics = new OpenDental.UI.ComboBox();
			this.butClinicPick = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textClinicKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textClinicDesc = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(272, 145);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textClinicId
			// 
			this.textClinicId.Location = new System.Drawing.Point(160, 16);
			this.textClinicId.Name = "textClinicId";
			this.textClinicId.ReadOnly = true;
			this.textClinicId.Size = new System.Drawing.Size(149, 20);
			this.textClinicId.TabIndex = 4;
			// 
			// labelUserId
			// 
			this.labelUserId.Location = new System.Drawing.Point(36, 16);
			this.labelUserId.Name = "labelUserId";
			this.labelUserId.Size = new System.Drawing.Size(118, 20);
			this.labelUserId.TabIndex = 5;
			this.labelUserId.Text = "DoseSpot Clinic ID";
			this.labelUserId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinics
			// 
			this.comboClinics.Location = new System.Drawing.Point(160, 100);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(120, 21);
			this.comboClinics.TabIndex = 6;
			// 
			// butClinicPick
			// 
			this.butClinicPick.Location = new System.Drawing.Point(282, 97);
			this.butClinicPick.Name = "butClinicPick";
			this.butClinicPick.Size = new System.Drawing.Size(27, 24);
			this.butClinicPick.TabIndex = 24;
			this.butClinicPick.Text = "...";
			this.butClinicPick.Click += new System.EventHandler(this.butClinicPick_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(36, 100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 20);
			this.label1.TabIndex = 25;
			this.label1.Text = "Clinic to Assign";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(36, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118, 20);
			this.label2.TabIndex = 27;
			this.label2.Text = "DoseSpot Clinic Key";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClinicKey
			// 
			this.textClinicKey.Location = new System.Drawing.Point(160, 44);
			this.textClinicKey.Name = "textClinicKey";
			this.textClinicKey.ReadOnly = true;
			this.textClinicKey.Size = new System.Drawing.Size(149, 20);
			this.textClinicKey.TabIndex = 26;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(36, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 20);
			this.label3.TabIndex = 29;
			this.label3.Text = "Clinic Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClinicDesc
			// 
			this.textClinicDesc.Location = new System.Drawing.Point(160, 72);
			this.textClinicDesc.Name = "textClinicDesc";
			this.textClinicDesc.ReadOnly = true;
			this.textClinicDesc.Size = new System.Drawing.Size(149, 20);
			this.textClinicDesc.TabIndex = 28;
			// 
			// FormDoseSpotAssignClinicId
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(372, 181);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textClinicDesc);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textClinicKey);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butClinicPick);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.labelUserId);
			this.Controls.Add(this.textClinicId);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDoseSpotAssignClinicId";
			this.Text = "Assign Clinic ID";
			this.Load += new System.EventHandler(this.FormDoseSpotAssignUserId_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private OpenDental.UI.Button butSave;
    private System.Windows.Forms.TextBox textClinicId;
    private System.Windows.Forms.Label labelUserId;
    private UI.ComboBox comboClinics;
    private UI.Button butClinicPick;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textClinicKey;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textClinicDesc;
  }
}