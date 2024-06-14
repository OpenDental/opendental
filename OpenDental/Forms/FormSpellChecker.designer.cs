namespace OpenDental{
	partial class FormSpellChecker {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSpellChecker));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.textMain = new OpenDental.ODtextBox();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// textMain
			// 
			this.textMain.AcceptsTab = true;
			this.textMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMain.BackColor = System.Drawing.SystemColors.Window;
			this.textMain.DetectLinksEnabled = false;
			this.textMain.DetectUrls = false;
			this.textMain.Location = new System.Drawing.Point(12, 27);
			this.textMain.Name = "textMain";
			this.textMain.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.CommLog;
			this.textMain.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMain.Size = new System.Drawing.Size(818, 564);
			this.textMain.TabIndex = 6;
			this.textMain.Text = "";
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(842, 24);
			this.menuMain.TabIndex = 7;
			// 
			// FormSpellChecker
			// 
			this.ClientSize = new System.Drawing.Size(842, 603);
			this.Controls.Add(this.textMain);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSpellChecker";
			this.Text = "Spell Checker";
			this.Load += new System.EventHandler(this.FormSpellChecker_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private ODtextBox textMain;
		private UI.MenuOD menuMain;
	}
}