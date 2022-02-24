namespace OpenDental{
	partial class FormFilePicker {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFilePicker));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butPreview = new OpenDental.UI.Button();
			this.butFileChoose = new OpenDental.UI.Button();
			this.butGo = new OpenDental.UI.Button();
			this.groupThumbnail = new System.Windows.Forms.GroupBox();
			this.labelThumbnail = new System.Windows.Forms.Label();
			this.odPictureBox = new OpenDental.UI.ODPictureBox();
			this.butImport = new OpenDental.UI.Button();
			this.groupThumbnail.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(405, 446);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(405, 476);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.CausesValidation = false;
			this.gridMain.Location = new System.Drawing.Point(13, 39);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(356, 461);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Files in path";
			this.gridMain.TranslationName = "FilePickerTable";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// textPath
			// 
			this.textPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPath.Location = new System.Drawing.Point(56, 12);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(290, 20);
			this.textPath.TabIndex = 5;
			this.textPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textPath_KeyPress);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(1, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 23);
			this.label1.TabIndex = 6;
			this.label1.Text = "Path:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPreview.Location = new System.Drawing.Point(405, 193);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(75, 24);
			this.butPreview.TabIndex = 7;
			this.butPreview.Text = "Preview";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// butFileChoose
			// 
			this.butFileChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butFileChoose.Location = new System.Drawing.Point(387, 273);
			this.butFileChoose.Name = "butFileChoose";
			this.butFileChoose.Size = new System.Drawing.Size(109, 24);
			this.butFileChoose.TabIndex = 8;
			this.butFileChoose.Text = "Select Local File";
			this.butFileChoose.Click += new System.EventHandler(this.butFileChoose_Click);
			// 
			// butGo
			// 
			this.butGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butGo.Image = global::OpenDental.Properties.Resources.Right;
			this.butGo.Location = new System.Drawing.Point(349, 11);
			this.butGo.Name = "butGo";
			this.butGo.Size = new System.Drawing.Size(20, 20);
			this.butGo.TabIndex = 9;
			this.butGo.Click += new System.EventHandler(this.butGo_Click);
			// 
			// groupThumbnail
			// 
			this.groupThumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupThumbnail.Controls.Add(this.labelThumbnail);
			this.groupThumbnail.Controls.Add(this.odPictureBox);
			this.groupThumbnail.Location = new System.Drawing.Point(375, 39);
			this.groupThumbnail.Name = "groupThumbnail";
			this.groupThumbnail.Size = new System.Drawing.Size(135, 148);
			this.groupThumbnail.TabIndex = 10;
			this.groupThumbnail.TabStop = false;
			this.groupThumbnail.Text = "Thumbnail";
			// 
			// labelThumbnail
			// 
			this.labelThumbnail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelThumbnail.Location = new System.Drawing.Point(9, 55);
			this.labelThumbnail.Name = "labelThumbnail";
			this.labelThumbnail.Size = new System.Drawing.Size(120, 41);
			this.labelThumbnail.TabIndex = 1;
			this.labelThumbnail.Text = "Thumbnail cannot be displayed";
			this.labelThumbnail.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.labelThumbnail.Visible = false;
			// 
			// odPictureBox
			// 
			this.odPictureBox.HasBorder = false;
			this.odPictureBox.Location = new System.Drawing.Point(3, 14);
			this.odPictureBox.Name = "odPictureBox";
			this.odPictureBox.Size = new System.Drawing.Size(128, 128);
			this.odPictureBox.TabIndex = 0;
			this.odPictureBox.Text = "Thumbnail";
			this.odPictureBox.TextNullImage = null;
			// 
			// butImport
			// 
			this.butImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butImport.Location = new System.Drawing.Point(405, 223);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(75, 24);
			this.butImport.TabIndex = 11;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// FormFilePicker
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(514, 512);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.groupThumbnail);
			this.Controls.Add(this.butGo);
			this.Controls.Add(this.butFileChoose);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFilePicker";
			this.Text = "Select Files";
			this.Load += new System.EventHandler(this.FormFilePicker_Load);
			this.groupThumbnail.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textPath;
		private System.Windows.Forms.Label label1;
		private UI.Button butPreview;
		private UI.Button butFileChoose;
		private UI.Button butGo;
		private System.Windows.Forms.GroupBox groupThumbnail;
		private UI.ODPictureBox odPictureBox;
		private System.Windows.Forms.Label labelThumbnail;
		private UI.Button butImport;
	}
}