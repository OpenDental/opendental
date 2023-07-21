namespace OpenDental {
	partial class FormTaskSubscribers {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskSubscribers));
			this.butClose = new OpenDental.UI.Button();
			this.gridODSubscribers = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(257, 480);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "&Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridODSubscribers
			// 
			this.gridODSubscribers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridODSubscribers.Location = new System.Drawing.Point(10, 10);
			this.gridODSubscribers.Name = "gridODSubscribers";
			this.gridODSubscribers.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridODSubscribers.Size = new System.Drawing.Size(322, 464);
			this.gridODSubscribers.TabIndex = 2;
			// 
			// FormTaskSubscribers
			// 
			this.ClientSize = new System.Drawing.Size(344, 512);
			this.Controls.Add(this.gridODSubscribers);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskSubscribers";
			this.Text = "Subscribers";
			this.Shown += new System.EventHandler(this.FormTaskSubscribers_Shown);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butClose;
		private UI.GridOD gridODSubscribers;
	}
}