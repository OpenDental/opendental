namespace OpenDental{
	public partial class FormRxCombine {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRxCombine));
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelCombine = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(904, 412);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(9, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(970, 356);
			this.gridMain.TabIndex = 1;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelCombine
			// 
			this.labelCombine.Location = new System.Drawing.Point(13, 10);
			this.labelCombine.Name = "labelCombine";
			this.labelCombine.Size = new System.Drawing.Size(476, 23);
			this.labelCombine.TabIndex = 4;
			this.labelCombine.Text = "Please select the prescription to keep when combining";
			this.labelCombine.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRxCombine
			// 
			this.ClientSize = new System.Drawing.Size(992, 448);
			this.Controls.Add(this.labelCombine);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRxCombine";
			this.Text = "Combine Prescriptions";
			this.Load += new System.EventHandler(this.FormRxCombine_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelCombine;
	}
}