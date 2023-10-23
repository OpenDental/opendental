namespace OpenDental{
	partial class FormMobileBrandingProfileEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMobileBrandingProfileEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.butSelectImage = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new OpenDental.ODtextBox();
			this.odPictureBoxPreview = new OpenDental.UI.ODPictureBox();
			this.textFilePathImage = new OpenDental.ODtextBox();
			this.butClear = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(406, 215);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(487, 215);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "Clinic Logo";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// butSelectImage
			// 
			this.butSelectImage.Location = new System.Drawing.Point(155, 81);
			this.butSelectImage.Name = "butSelectImage";
			this.butSelectImage.Size = new System.Drawing.Size(115, 24);
			this.butSelectImage.TabIndex = 4;
			this.butSelectImage.Text = "Select Logo Image";
			this.butSelectImage.UseVisualStyleBackColor = true;
			this.butSelectImage.Click += new System.EventHandler(this.butSelectImage_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 161);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 20);
			this.label1.TabIndex = 5;
			this.label1.Text = "Clinic Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.AcceptsTab = true;
			this.textDescription.BackColor = System.Drawing.SystemColors.Window;
			this.textDescription.DetectLinksEnabled = false;
			this.textDescription.DetectUrls = false;
			this.textDescription.Location = new System.Drawing.Point(95, 161);
			this.textDescription.Multiline = false;
			this.textDescription.Name = "textDescription";
			this.textDescription.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Letter;
			this.textDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(230, 20);
			this.textDescription.TabIndex = 9;
			this.textDescription.Text = "";
			// 
			// odPictureBoxPreview
			// 
			this.odPictureBoxPreview.Location = new System.Drawing.Point(276, 56);
			this.odPictureBoxPreview.Name = "odPictureBoxPreview";
			this.odPictureBoxPreview.Padding = new System.Windows.Forms.Padding(30);
			this.odPictureBoxPreview.Size = new System.Drawing.Size(90, 90);
			this.odPictureBoxPreview.TabIndex = 10;
			this.odPictureBoxPreview.Text = "odPictureBox1";
			this.odPictureBoxPreview.TextNullImage = "Choose an image.";
			// 
			// textFilePathImage
			// 
			this.textFilePathImage.AcceptsTab = true;
			this.textFilePathImage.AllowsCarriageReturns = false;
			this.textFilePathImage.BackColor = System.Drawing.SystemColors.Window;
			this.textFilePathImage.DetectLinksEnabled = false;
			this.textFilePathImage.DetectUrls = false;
			this.textFilePathImage.Location = new System.Drawing.Point(95, 56);
			this.textFilePathImage.Multiline = false;
			this.textFilePathImage.Name = "textFilePathImage";
			this.textFilePathImage.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Letter;
			this.textFilePathImage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textFilePathImage.Size = new System.Drawing.Size(175, 20);
			this.textFilePathImage.SpellCheckIsEnabled = false;
			this.textFilePathImage.TabIndex = 11;
			this.textFilePathImage.Text = "";
			this.textFilePathImage.Leave += new System.EventHandler(this.textFilePathImage_Leave);
			// 
			// butClear
			// 
			this.butClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClear.Location = new System.Drawing.Point(7, 215);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 24);
			this.butClear.TabIndex = 12;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(372, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(181, 59);
			this.label4.TabIndex = 14;
			this.label4.Text = "Any image bigger than 90x90 will be reduced in size. Image does not need to be sq" +
    "uare.";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(21, 23);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(408, 30);
			this.label3.TabIndex = 13;
			this.label3.Text = "The image and name below will show to patients on your office\'s eClipboard.";
			// 
			// FormMobileBrandingProfileEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(574, 251);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.textFilePathImage);
			this.Controls.Add(this.odPictureBoxPreview);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSelectImage);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMobileBrandingProfileEdit";
			this.Text = "Mobile Branding Profile Edit";
			this.Load += new System.EventHandler(this.FormMobileBrandingProfileEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private UI.Button butSelectImage;
		private System.Windows.Forms.Label label1;
		private ODtextBox textDescription;
		private UI.ODPictureBox odPictureBoxPreview;
		private ODtextBox textFilePathImage;
		private UI.Button butClear;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
	}
}