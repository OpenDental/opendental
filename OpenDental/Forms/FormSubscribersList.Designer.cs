namespace OpenDental {
	partial class FormSubscribersList {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSubscribersList));
			this.gridSubscribers = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// gridSubscribers
			// 
			this.gridSubscribers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridSubscribers.Location = new System.Drawing.Point(23, 24);
			this.gridSubscribers.Name = "gridSubscribers";
			this.gridSubscribers.Size = new System.Drawing.Size(453, 331);
			this.gridSubscribers.TabIndex = 4;
			this.gridSubscribers.TranslationName = "TableSubscribers";
			// 
			// FormSubscribersList
			// 
			this.ClientSize = new System.Drawing.Size(498, 367);
			this.Controls.Add(this.gridSubscribers);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSubscribersList";
			this.Text = "Other Subscribers List";
			this.Load += new System.EventHandler(this.FormSubscribersList_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridSubscribers;
	}
}