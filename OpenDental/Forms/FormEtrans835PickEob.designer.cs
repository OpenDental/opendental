namespace OpenDental{
	partial class FormEtrans835PickEob {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtrans835PickEob));
			this.label1 = new System.Windows.Forms.Label();
			this.gridEobs = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(468, 18);
			this.label1.TabIndex = 4;
			this.label1.Text = "Choose an EOB from the list below.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridEobs
			// 
			this.gridEobs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEobs.Location = new System.Drawing.Point(15, 30);
			this.gridEobs.Name = "gridEobs";
			this.gridEobs.Size = new System.Drawing.Size(305, 271);
			this.gridEobs.TabIndex = 5;
			this.gridEobs.Title = "EOB List";
			this.gridEobs.TranslationName = "TableEOB";
			this.gridEobs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEobs_CellDoubleClick);
			// 
			// FormEtrans835PickEob
			// 
			this.ClientSize = new System.Drawing.Size(332, 313);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.gridEobs);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEtrans835PickEob";
			this.Text = "EOBs";
			this.Load += new System.EventHandler(this.FormEtrans835PickEob_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private UI.GridOD gridEobs;
	}
}