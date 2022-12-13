namespace OpenDental{
	partial class FormCommItemHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCommItemHistory));
			this.butClose = new OpenDental.UI.Button();
			this.gridCommlogHist = new OpenDental.UI.GridOD();
			this.textNote = new OpenDental.ODtextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(870, 499);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridCommlogHist
			// 
			this.gridCommlogHist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridCommlogHist.Location = new System.Drawing.Point(12, 12);
			this.gridCommlogHist.Name = "gridCommlogHist";
			this.gridCommlogHist.Size = new System.Drawing.Size(468, 474);
			this.gridCommlogHist.TabIndex = 4;
			this.gridCommlogHist.Title = "Commlog History";
			this.gridCommlogHist.TranslationName = "TableCommlogHistory";
			this.gridCommlogHist.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCommlogHist_CellDoubleClick);
			this.gridCommlogHist.SelectionCommitted += new System.EventHandler(this.gridCommlogHist_SelectionCommitted);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.BackColor = System.Drawing.SystemColors.Control;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(486, 46);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textNote.ReadOnly = true;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(459, 440);
			this.textNote.TabIndex = 5;
			this.textNote.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(486, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 16);
			this.label2.TabIndex = 122;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormCommItemHistory
			// 
			this.ClientSize = new System.Drawing.Size(957, 535);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.gridCommlogHist);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCommItemHistory";
			this.Text = "Communication Item History";
			this.Load += new System.EventHandler(this.FormCommItemHistory_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridCommlogHist;
		private ODtextBox textNote;
		private System.Windows.Forms.Label label2;
	}
}