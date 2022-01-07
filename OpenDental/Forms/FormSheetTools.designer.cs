namespace OpenDental{
	partial class FormSheetTools {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetTools));
			this.butExport = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butExport
			// 
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(21, 92);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(80, 24);
			this.butExport.TabIndex = 2;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butImport
			// 
			this.butImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImport.Location = new System.Drawing.Point(21, 28);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(80, 24);
			this.butImport.TabIndex = 1;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(283, 165);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(107, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(251, 60);
			this.label2.TabIndex = 27;
			this.label2.Text = "Import a sheet file in xml format that has been exported from another Open Dental" +
    " database.\r\n";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(107, 92);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(251, 60);
			this.label1.TabIndex = 28;
			this.label1.Text = "Export a custom sheet to an xml file so that you can later import it to any other" +
    " Open Dental database.";
			// 
			// FormSheetTools
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(370, 201);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetTools";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Sheet Tools";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.Button butImport;
		private UI.Button butExport;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}