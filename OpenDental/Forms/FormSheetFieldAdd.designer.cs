namespace OpenDental{
	partial class FormSheetFieldAdd {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetFieldAdd));
			this.butCancel = new OpenDental.UI.Button();
			this.labelInternal = new System.Windows.Forms.Label();
			this.butPatImage = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(366, 249);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 15;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelInternal
			// 
			this.labelInternal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInternal.Location = new System.Drawing.Point(12, 18);
			this.labelInternal.Name = "labelInternal";
			this.labelInternal.Size = new System.Drawing.Size(366, 35);
			this.labelInternal.TabIndex = 83;
			this.labelInternal.Text = "The only type of field that can currently be added to an existing sheet is a pati" +
    "ent image. We might add more types later.";
			// 
			// butPatImage
			// 
			this.butPatImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPatImage.Location = new System.Drawing.Point(15, 83);
			this.butPatImage.Name = "butPatImage";
			this.butPatImage.Size = new System.Drawing.Size(75, 24);
			this.butPatImage.TabIndex = 84;
			this.butPatImage.Text = "Pat Image";
			this.butPatImage.Click += new System.EventHandler(this.butPatImage_Click);
			// 
			// FormSheetFieldAdd
			// 
			this.ClientSize = new System.Drawing.Size(457, 287);
			this.Controls.Add(this.butPatImage);
			this.Controls.Add(this.labelInternal);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetFieldAdd";
			this.Text = "Add Field";
			this.Load += new System.EventHandler(this.FormSheetFieldAdd_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelInternal;
		private UI.Button butPatImage;
	}
}