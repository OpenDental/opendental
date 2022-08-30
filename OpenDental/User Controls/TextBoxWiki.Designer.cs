namespace OpenDental {
	partial class TextBoxWiki {
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.textBoxMain = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBoxMain
			// 
			this.textBoxMain.AcceptsReturn = true;
			this.textBoxMain.AcceptsTab = true;
			this.textBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxMain.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxMain.HideSelection = false;
			this.textBoxMain.Location = new System.Drawing.Point(0, 0);
			this.textBoxMain.MaxLength = 1000000;
			this.textBoxMain.Multiline = true;
			this.textBoxMain.Name = "textBoxMain";
			this.textBoxMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxMain.Size = new System.Drawing.Size(150, 150);
			this.textBoxMain.TabIndex = 0;
			this.textBoxMain.Text = "this is a test";
			this.textBoxMain.TextChanged += new System.EventHandler(this.textBoxMain_TextChanged);
			this.textBoxMain.DoubleClick += new System.EventHandler(this.textBoxMain_DoubleClick);
			this.textBoxMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMain_KeyDown);
			this.textBoxMain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMain_KeyPress);
			this.textBoxMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBoxMain_MouseDoubleClick);
			this.textBoxMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxMain_MouseDown);
			this.textBoxMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBoxMain_MouseMove);
			this.textBoxMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textBoxMain_MouseUp);
			// 
			// TextBoxWiki
			// 
			this.Controls.Add(this.textBoxMain);
			this.DoubleBuffered = true;
			this.Name = "TextBoxWiki";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxMain;
	}
}
