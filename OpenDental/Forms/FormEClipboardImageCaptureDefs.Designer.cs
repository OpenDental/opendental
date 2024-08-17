namespace OpenDental{
	partial class FormEClipboardImageCaptureDefs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEClipboardImageCaptureDefs));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.labelHelp = new System.Windows.Forms.Label();
			this.gridEClipboardImagesInUse = new OpenDental.UI.GridOD();
			this.gridAvailableEClipboardImages = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(827, 507);
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
			this.butCancel.Location = new System.Drawing.Point(913, 507);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLeft.Location = new System.Drawing.Point(396, 151);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(84, 24);
			this.butLeft.TabIndex = 6;
			this.butLeft.Text = "Remove";
			this.butLeft.UseVisualStyleBackColor = true;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butRight.Location = new System.Drawing.Point(396, 121);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(84, 24);
			this.butRight.TabIndex = 7;
			this.butRight.Text = "Select";
			this.butRight.UseVisualStyleBackColor = true;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// labelHelp
			// 
			this.labelHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelHelp.Location = new System.Drawing.Point(9, 9);
			this.labelHelp.Name = "labelHelp";
			this.labelHelp.Size = new System.Drawing.Size(662, 46);
			this.labelHelp.TabIndex = 8;
			this.labelHelp.Text = resources.GetString("labelHelp.Text");
			// 
			// gridEClipboardImagesInUse
			// 
			this.gridEClipboardImagesInUse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEClipboardImagesInUse.Location = new System.Drawing.Point(493, 59);
			this.gridEClipboardImagesInUse.Name = "gridEClipboardImagesInUse";
			this.gridEClipboardImagesInUse.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridEClipboardImagesInUse.Size = new System.Drawing.Size(495, 438);
			this.gridEClipboardImagesInUse.TabIndex = 5;
			this.gridEClipboardImagesInUse.Title = "eClipboard Images In Use";
			this.gridEClipboardImagesInUse.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEClipboardImagesInUse_CellDoubleClick);
			// 
			// gridAvailableEClipboardImages
			// 
			this.gridAvailableEClipboardImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridAvailableEClipboardImages.Location = new System.Drawing.Point(12, 58);
			this.gridAvailableEClipboardImages.Name = "gridAvailableEClipboardImages";
			this.gridAvailableEClipboardImages.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAvailableEClipboardImages.Size = new System.Drawing.Size(371, 439);
			this.gridAvailableEClipboardImages.TabIndex = 0;
			this.gridAvailableEClipboardImages.Title = "Available eClipboard Images";
			// 
			// FormEClipboardImageCaptureDefs
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1000, 543);
			this.Controls.Add(this.labelHelp);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.gridEClipboardImagesInUse);
			this.Controls.Add(this.gridAvailableEClipboardImages);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEClipboardImageCaptureDefs";
			this.Text = "eClipboard Images";
			this.Load += new System.EventHandler(this.FormEClipboardImageCaptureDefs_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridAvailableEClipboardImages;
		private UI.GridOD gridEClipboardImagesInUse;
		private UI.Button butLeft;
		private UI.Button butRight;
		private System.Windows.Forms.Label labelHelp;
	}
}