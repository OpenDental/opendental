using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestToothChart {
	public partial class FormImageViewer:Form {
		public Bitmap Bmp;

		public FormImageViewer() {
			InitializeComponent();
		}

		private void FormImageViewer_Load(object sender,EventArgs e) {
			Width=Bmp.Width+16;
			Height=Bmp.Height+36;
			pictureBox1.Image=Bmp;
		}




	}
}
