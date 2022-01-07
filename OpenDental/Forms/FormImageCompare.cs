using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormImageCompare:FormODBase {

		public FormImageCompare(Bitmap imgOne,Bitmap imgTwo) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);			
			Bitmap img1=imgOne;
			Bitmap img2=imgTwo;
			if(img1.Size.Width>panelImage1.Size.Width || img1.Size.Height>panelImage1.Size.Height) {
				img1=new Bitmap(img1,panelImage1.Size);
			}
			if(img2.Size.Width>panelImage2.Size.Width || img2.Size.Height>panelImage2.Size.Height) {
				img2=new Bitmap(img2,panelImage2.Size);
			}
			panelImage1.BackgroundImage=img1;
			panelImage2.BackgroundImage=img2;
		}

		private void FormImageCompare_Load(object sender,EventArgs e) {
			
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}