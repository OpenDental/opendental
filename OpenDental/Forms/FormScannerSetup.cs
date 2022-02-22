using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public class FormScannerSetup:System.Windows.Forms.Form {
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label37;
		private ValidNum textDoc;

		///<summary></summary>
		public FormScannerSetup(){
			InitializeComponent();
			//too many labels to use Lan.F()
			Lan.C(this, new System.Windows.Forms.Control[]
			{
				this,
				this.groupBox1,
				this.groupBox2,
				this.groupBox3,
				this.label12,
				this.label13,
				this.label25,
				this.label37
			});
			Lan.C("All", new System.Windows.Forms.Control[] {
				butOK,
				butCancel,
			});
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textDoc = new OpenDental.ValidNum();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label25 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label37 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Autosize = true;
			this.butOK.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOK.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOK.Location = new System.Drawing.Point(564,217);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(564,255);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(14,12);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(392,31);
			this.label12.TabIndex = 14;
			this.label12.Text = "JPEG Compression - Quality After Scanning, 0-100. 100=No compression.  Typical se" +
    "tting: 40.";
			this.label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(14,71);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(191,31);
			this.label13.TabIndex = 15;
			this.label13.Text = "Suggested setting for scanning documents is Greyscale, 150 dpi.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textDoc);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(20,19);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(484,109);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Documents";
			// 
			// textDoc
			// 
			this.textDoc.Location = new System.Drawing.Point(16,48);
			this.textDoc.MaxVal = 255;
			this.textDoc.MinVal = 0;
			this.textDoc.Name = "textDoc";
			this.textDoc.Size = new System.Drawing.Size(68,20);
			this.textDoc.TabIndex = 20;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label25);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(20,136);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(484,68);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Radiographs";
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(13,26);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(209,32);
			this.label25.TabIndex = 15;
			this.label25.Text = "Suggested setting for scanning panos is Greyscale, 300 dpi.  For BWs, 400dpi.";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label37);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(20,213);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(484,68);
			this.groupBox3.TabIndex = 19;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Photos";
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(14,26);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(176,34);
			this.label37.TabIndex = 15;
			this.label37.Text = "Suggested setting for scanning photos is Color, 300 dpi.";
			// 
			// FormScannerSetup
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(666,306);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScannerSetup";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Scanner Setup";
			this.Load += new System.EventHandler(this.FormScannerSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormScannerSetup_Load(object sender, System.EventArgs e) {
			//Prefs.Cur=(Pref)PrefB.HList["ScannerCompression"];
			//try{
				//trackQ.Value=Convert.ToInt32(Prefs.Cur.ValueString);
			//}
			//catch{}
			textDoc.Text=PrefB.GetInt("ScannerCompression").ToString();
			//trackQ.Value=
			//textCropDelta.Text=
				//((Pref)PrefB.HList["CropDelta"]).ValueString;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(  textDoc.errorProvider1.GetError(textDoc)!=""
				//|| textAmount.errorProvider1.GetError(textAmount)!=""
				){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			Prefs.UpdateInt("ScannerCompression",PIn.PInt(textDoc.Text));
			//Prefs.UpdateInt("ScannerCompressionRadiographs",trackRadiographs.Value);
			//Prefs.UpdateInt("ScannerCompressionPhotos",trackPhotos.Value);
			
			/*Prefs.Cur=(Pref)PrefB.HList["CropDelta"];
			Prefs.Cur.ValueString=textCropDelta.Text;
			Prefs.UpdateCur();*/
			DataValid.SetInvalid(InvalidTypes.Prefs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}
