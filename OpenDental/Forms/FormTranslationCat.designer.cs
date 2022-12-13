using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTranslationCat {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTranslationCat));
			this.listCats = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butClose = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butDownload = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.labelLanguage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listCats
			// 
			this.listCats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listCats.Location = new System.Drawing.Point(28, 34);
			this.listCats.Name = "listCats";
			this.listCats.Size = new System.Drawing.Size(262, 589);
			this.listCats.TabIndex = 0;
			this.listCats.DoubleClick += new System.EventHandler(this.listCats_DoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(28, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(214, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select a category";
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(416, 600);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butExport.Location = new System.Drawing.Point(310, 194);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(96, 26);
			this.butExport.TabIndex = 3;
			this.butExport.Text = "&Export All";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butDownload
			// 
			this.butDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDownload.Location = new System.Drawing.Point(310, 34);
			this.butDownload.Name = "butDownload";
			this.butDownload.Size = new System.Drawing.Size(102, 26);
			this.butDownload.TabIndex = 4;
			this.butDownload.Text = "&Download";
			this.butDownload.Click += new System.EventHandler(this.butDownload_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(310, 226);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(182, 82);
			this.label2.TabIndex = 5;
			this.label2.Text = "Use this to create a file to send to us with all translations.  You can ONLY do t" +
    "his if you are the manager for your language.";
			// 
			// labelLanguage
			// 
			this.labelLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelLanguage.Location = new System.Drawing.Point(310, 63);
			this.labelLanguage.Name = "labelLanguage";
			this.labelLanguage.Size = new System.Drawing.Size(198, 81);
			this.labelLanguage.TabIndex = 6;
			this.labelLanguage.Text = "Language/Culture: ";
			// 
			// FormTranslationCat
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(520, 648);
			this.Controls.Add(this.labelLanguage);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listCats);
			this.Controls.Add(this.butDownload);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTranslationCat";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Select Category";
			this.Load += new System.EventHandler(this.FormTranslation_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.ListBoxOD listCats;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butExport;
		private OpenDental.UI.Button butDownload;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private Label labelLanguage;
	}
}
