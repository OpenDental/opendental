namespace OpenDental{
	partial class FormMassEmailSendResults {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailSendResults));
			this.gridResults = new OpenDental.UI.GridOD();
			this.butPrint = new OpenDental.UI.Button();
			this.labelResultCount = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridResults
			// 
			this.gridResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridResults.Location = new System.Drawing.Point(12, 12);
			this.gridResults.Name = "gridResults";
			this.gridResults.Size = new System.Drawing.Size(1013, 479);
			this.gridResults.TabIndex = 4;
			this.gridResults.Title = "Email Status";
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Location = new System.Drawing.Point(12, 497);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 5;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// labelResultCount
			// 
			this.labelResultCount.Location = new System.Drawing.Point(93, 497);
			this.labelResultCount.Name = "labelResultCount";
			this.labelResultCount.Size = new System.Drawing.Size(190, 24);
			this.labelResultCount.TabIndex = 6;
			this.labelResultCount.Text = "Total sent: ";
			// 
			// FormMassEmailSendResults
			// 
			this.ClientSize = new System.Drawing.Size(1037, 536);
			this.Controls.Add(this.labelResultCount);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridResults);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailSendResults";
			this.Text = "Mass Email Results";
			this.Load += new System.EventHandler(this.FormMassEmailSendResults_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridResults;
		private UI.Button butPrint;
		private System.Windows.Forms.Label labelResultCount;
	}
}