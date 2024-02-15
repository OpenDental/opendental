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
			this.gridODSubscribers = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// gridODSubscribers
			// 
			this.gridODSubscribers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridODSubscribers.Location = new System.Drawing.Point(10, 10);
			this.gridODSubscribers.Name = "gridODSubscribers";
			this.gridODSubscribers.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridODSubscribers.Size = new System.Drawing.Size(322, 472);
			this.gridODSubscribers.TabIndex = 2;
			// 
			// FormTaskSubscribers
			// 
			this.ClientSize = new System.Drawing.Size(344, 494);
			this.Controls.Add(this.gridODSubscribers);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskSubscribers";
			this.Text = "Subscribers";
			this.Shown += new System.EventHandler(this.FormTaskSubscribers_Shown);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridODSubscribers;
	}
}