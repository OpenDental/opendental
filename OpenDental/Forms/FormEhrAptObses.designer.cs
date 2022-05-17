namespace OpenDental{
	partial class FormEhrAptObses {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrAptObses));
			this.butCancel = new OpenDental.UI.Button();
			this.gridObservations = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butExportHL7 = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(558, 447);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridObservations
			// 
			this.gridObservations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridObservations.Location = new System.Drawing.Point(12, 12);
			this.gridObservations.Name = "gridObservations";
			this.gridObservations.Size = new System.Drawing.Size(621, 429);
			this.gridObservations.TabIndex = 4;
			this.gridObservations.Title = null;
			this.gridObservations.TranslationName = "TableOservations";
			this.gridObservations.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridObservations_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(12, 447);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(86, 24);
			this.butAdd.TabIndex = 73;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butExportHL7
			// 
			this.butExportHL7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExportHL7.Location = new System.Drawing.Point(286, 447);
			this.butExportHL7.Name = "butExportHL7";
			this.butExportHL7.Size = new System.Drawing.Size(75, 24);
			this.butExportHL7.TabIndex = 74;
			this.butExportHL7.Text = "Export HL7";
			this.butExportHL7.Click += new System.EventHandler(this.butExportHL7_Click);
			// 
			// FormEhrAptObses
			// 
			this.ClientSize = new System.Drawing.Size(645, 483);
			this.Controls.Add(this.butExportHL7);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridObservations);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrAptObses";
			this.Text = "Appointment Observations";
			this.Load += new System.EventHandler(this.FormEhrAptObses_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridObservations;
		private UI.Button butAdd;
		private UI.Button butExportHL7;
	}
}