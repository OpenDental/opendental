namespace OpenDental{
	partial class FormEtrans835PickEra {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtrans835PickEra));
			this.label1 = new System.Windows.Forms.Label();
			this.gridEras = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(468, 18);
			this.label1.TabIndex = 4;
			this.label1.Text = "Choose an ERA from the list below.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridEras
			// 
			this.gridEras.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEras.Location = new System.Drawing.Point(15, 30);
			this.gridEras.Name = "gridEras";
			this.gridEras.Size = new System.Drawing.Size(471, 261);
			this.gridEras.TabIndex = 5;
			this.gridEras.Title = "ERA List";
			this.gridEras.TranslationName = "TableERA";
			this.gridEras.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEras_CellDoubleClick);
			// 
			// FormEtrans835PickEra
			// 
			this.ClientSize = new System.Drawing.Size(498, 303);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.gridEras);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEtrans835PickEra";
			this.Text = "ERAs";
			this.Load += new System.EventHandler(this.FormEtrans835PickEra_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private UI.GridOD gridEras;
	}
}