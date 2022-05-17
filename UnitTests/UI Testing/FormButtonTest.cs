using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;
using OpenDental.UI;

namespace UnitTests{
	public partial class FormButtonTest : FormODBase{
		public FormButtonTest()
		{
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormButtonTest_Load(object sender, EventArgs e)
		{
			//CodeBase.OdThemeModernGrey.SetTheme(CodeBase.OdTheme.MonoFlatBlue);
			Size sizeCanvas=new Size(1000,1000);
			Size sizeImage=new Size(500,800);
			zoomSlider1.SetValue(sizeCanvas,sizeImage,0);
		}

		private void ButDeleteProc_Click(object sender, EventArgs e)
		{
			
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			button2.Text="Longer text result";
		}
	}
}
