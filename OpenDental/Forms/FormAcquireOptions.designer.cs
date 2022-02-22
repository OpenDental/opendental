namespace OpenDental{
	partial class FormAcquireOptions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAcquireOptions));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.radioSingle = new System.Windows.Forms.RadioButton();
			this.radioMount = new System.Windows.Forms.RadioButton();
			this.labelMountUnavail = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(161, 305);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(242, 305);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// radioSingle
			// 
			this.radioSingle.Location = new System.Drawing.Point(28, 12);
			this.radioSingle.Name = "radioSingle";
			this.radioSingle.Size = new System.Drawing.Size(160, 20);
			this.radioSingle.TabIndex = 4;
			this.radioSingle.TabStop = true;
			this.radioSingle.Text = "Single image";
			this.radioSingle.UseVisualStyleBackColor = true;
			// 
			// radioMount
			// 
			this.radioMount.Location = new System.Drawing.Point(28, 33);
			this.radioMount.Name = "radioMount";
			this.radioMount.Size = new System.Drawing.Size(236, 20);
			this.radioMount.TabIndex = 5;
			this.radioMount.TabStop = true;
			this.radioMount.Text = "Acquire into selected mount position";
			this.radioMount.UseVisualStyleBackColor = true;
			// 
			// labelMountUnavail
			// 
			this.labelMountUnavail.Location = new System.Drawing.Point(43, 51);
			this.labelMountUnavail.Name = "labelMountUnavail";
			this.labelMountUnavail.Size = new System.Drawing.Size(204, 30);
			this.labelMountUnavail.TabIndex = 19;
			this.labelMountUnavail.Text = "(this is not an option because an empty mount position is not selected)";
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(19, 93);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(298, 201);
			this.gridMain.TabIndex = 20;
			this.gridMain.Title = "Devices";
			this.gridMain.TranslationName = "TableImagingDevices";
			// 
			// FormAcquireOptions
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(332, 339);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelMountUnavail);
			this.Controls.Add(this.radioMount);
			this.Controls.Add(this.radioSingle);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAcquireOptions";
			this.Text = "Acquire Options";
			this.Load += new System.EventHandler(this.FormAcquireOptions_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.RadioButton radioSingle;
		private System.Windows.Forms.RadioButton radioMount;
		private System.Windows.Forms.Label labelMountUnavail;
		private UI.GridOD gridMain;
	}
}