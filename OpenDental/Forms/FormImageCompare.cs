using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormImageCompare:FormODBase {

		public FormImageCompare(Bitmap bitmapOne,Bitmap bitmapTwo) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);			
			Bitmap bitmap1=bitmapOne;
			Bitmap bitmap2=bitmapTwo;
			if(bitmap1.Size.Width>panelImage1.Size.Width || bitmap1.Size.Height>panelImage1.Size.Height) {
				bitmap1=new Bitmap(bitmap1,panelImage1.Size);
			}
			if(bitmap2.Size.Width>panelImage2.Size.Width || bitmap2.Size.Height>panelImage2.Size.Height) {
				bitmap2=new Bitmap(bitmap2,panelImage2.Size);
			}
			panelImage1.BackgroundImage=bitmap1;
			panelImage2.BackgroundImage=bitmap2;
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