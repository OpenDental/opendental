namespace OpenDental{
	partial class FormTaskHist {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskHist));
			this.gridTaskHist = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// gridTaskHist
			// 
			this.gridTaskHist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridTaskHist.Location = new System.Drawing.Point(12, 12);
			this.gridTaskHist.Name = "gridTaskHist";
			this.gridTaskHist.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridTaskHist.Size = new System.Drawing.Size(741, 392);
			this.gridTaskHist.TabIndex = 5;
			this.gridTaskHist.Title = "History";
			this.gridTaskHist.TranslationName = "TableHist";
			// 
			// FormTaskHist
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(765, 416);
			this.Controls.Add(this.gridTaskHist);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskHist";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Task History";
			this.Load += new System.EventHandler(this.FormTaskHist_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridTaskHist;
	}
}