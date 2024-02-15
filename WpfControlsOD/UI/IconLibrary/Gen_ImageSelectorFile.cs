using System;
using System.Windows.Media;
using OpenDentBusiness;

namespace WpfControls.UI{
	//---------------This is one of the rare Gen files that's not generated automatically.
	//It's entire manual.
	public class Gen_ImageSelectorFile{
		public static void Draw(DrawSvg drawSvg,double scale){
			//Draw on a 22x22 canvas, and it will generally be shrunk by about 25% to 16x16.
			drawSvg.FillRectangle(Color.FromRgb(220,220,220),2,2,18,18,scale);
			drawSvg.FillRectangle(Colors.DarkBlue,2,2,18,5,scale);
			drawSvg.DrawRectangle(Colors.DarkBlue,2,2,18,18,1,scale);
		}

		public static string GetBase64(){
			return "";
		}
	}
}