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
		private WpfControls.UI.ZoomSlider zoomSlider;
		private WpfControls.UI.WindowingSlider windowingSlider2;

		public FormButtonTest()
		{
			InitializeComponent();
			InitializeLayoutManager();
			zoomSlider=new WpfControls.UI.ZoomSlider();
			zoomSlider.IsEnabled=false;
			elementHost.Child=zoomSlider;
			windowingSlider.MinVal=40;
			windowingSlider.MaxVal=225;
			windowingSlider.Enabled=true;
			windowingSlider2=new WpfControls.UI.WindowingSlider();
			elementHostWindowingSlider.Child=windowingSlider2;
			windowingSlider2.Width=154;
			windowingSlider2.IsEnabled=true;
			windowingSlider2.MinVal=40;
			windowingSlider2.MaxVal=225;
		}

		private void FormButtonTest_Load(object sender, EventArgs e)
		{
			//CodeBase.OdThemeModernGrey.SetTheme(CodeBase.OdTheme.MonoFlatBlue);
			Size sizeCanvas=new Size(1000,1000);
			Size sizeImage=new Size(500,800);
			zoomSlider.SetValueInitialFit(new System.Windows.Size(sizeCanvas.Width,sizeCanvas.Height),new System.Windows.Size(sizeImage.Width,sizeImage.Height),0);
			zoomSlider.IsEnabled=false;
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
