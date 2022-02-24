using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental{
///<summary></summary>
	public class FormTranslationCat : FormODBase {
		private OpenDental.UI.ListBoxOD listCats;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butExport;
		private OpenDental.UI.Button butDownload;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		public FormTranslationCat(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.groupBox1.SuspendLayout();
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
			this.butDownload.Location = new System.Drawing.Point(14, 22);
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
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(14, 58);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(178, 58);
			this.textBox1.TabIndex = 6;
			this.textBox1.Text = "Download and install the most current translations from our website.  This will o" +
    "verwrite all current translations.";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butDownload);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(306, 30);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 122);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "NO ENGLISH ??";
			// 
			// FormTranslationCat
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(520, 648);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listCats);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTranslationCat";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Category";
			this.Load += new System.EventHandler(this.FormTranslation_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void FormTranslation_Load(object sender, System.EventArgs e) {
			//MessageBox.Show(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
			if(CultureInfo.CurrentCulture.Name=="en-US"){
				MessageBox.Show("You must change your culture in Windows first to something other than English-US.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			FillList();
		}

		private void FillList(){
			listCats.Items.Clear();
			listCats.Items.AddList(Lans.GetListCat(),x => x.ToString());
		}

		private void listCats_DoubleClick(object sender, System.EventArgs e){
			if(listCats.SelectedIndex==-1){
				return;
			}
			using FormTranslation FormT=new FormTranslation(listCats.GetSelected<string>()); 
			FormT.ShowDialog();
		}

		private void butDownload_Click(object sender, System.EventArgs e) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				MessageBox.Show("Not allowed when using client web.");
				return;
			}
			string remoteUri = "http://www.opendental.com/cultures/";
			string fileName = CultureInfo.CurrentCulture.Name+".txt";//eg. en-US.txt
			string myStringWebResource = null;
			WebClient myWebClient = new WebClient();
			myStringWebResource=remoteUri+fileName;
			Cursor=Cursors.WaitCursor;
			try{
				myWebClient.DownloadFile(myStringWebResource,fileName);
			}
			catch{
				Cursor=Cursors.Default;
				MessageBox.Show("Either you do not have internet access, or no translations are available for "+CultureInfo.CurrentCulture.DisplayName);
				return;
			}
			//ClassConvertDatabase ConvertDB=new ClassConvertDatabase();
			try{
				//ConvertDB.ExecuteFile(fileName);
				string content = File.ReadAllText(fileName).Trim();
				Lans.LoadTranslationsFromTextFile(content);
			}
			catch{
				Cursor=Cursors.Default;
				MessageBox.Show("Translations not installed properly.");
				return;
			}
			LanguageForeigns.RefreshCache();
			Cursor=Cursors.Default;
			MessageBox.Show("Done");
		}

		///<summary>Only exports for the current culture.</summary>
		private void butExport_Click(object sender, System.EventArgs e) {
			string fileName=CultureInfo.CurrentCulture.Name+".sql";//eg en-US.sql
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
			}
			else {
				saveFileDialog1.InitialDirectory=Application.StartupPath;
				saveFileDialog1.FileName=fileName;
				if(saveFileDialog1.ShowDialog()!=DialogResult.OK) {
					saveFileDialog1.Dispose();
					return;
				}
				filePath=saveFileDialog1.FileName;
				saveFileDialog1.Dispose();
			}
			StreamWriter sw=new StreamWriter(filePath,false,System.Text.Encoding.UTF8);
			sw.WriteLine("DELETE FROM languageforeign WHERE Culture='"+CultureInfo.CurrentCulture.Name+"';");
			LanguageForeign[] LFList=LanguageForeigns.GetListForCurrentCulture();
			for(int i=0;i<LFList.Length;i++){
				sw.WriteLine(
					"INSERT INTO languageforeign (ClassType,English,Culture,Translation,Comments) VALUES ('"+POut.String(LFList[i].ClassType)
					+"', '"+POut.String(LFList[i].English)
					+"', '"+POut.String(LFList[i].Culture)
					+"', '"+POut.String(LFList[i].Translation)
					+"', '"+POut.String(LFList[i].Comments)+"');"
				);
			}//for
			sw.Close();
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MessageBox.Show("Done");
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}


	}
}
