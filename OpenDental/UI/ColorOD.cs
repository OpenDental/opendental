using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental {
	public class ColorOD {
		//The plan here is to move to our own version of colors similar to SystemColors.
		//Some colors will be named according to how they are to be used, and we will be able to find where they are all used.
		//This is not a move toward themes, but rather a move away from MS themes.
		//One downside is that these colors will not be available in the designer (yet).
		//Here are some int colors as stored in db:
		//Black=-16777216
		//White=-1
		//Transparent=16777215 (0,255,255,255) which would be white, except for 0 alpha.
		//Empty=0 (0,0,0,0) which would be black, except for 0 alpha, so it is another version of transparent that we don't use.
		//Here's how to test if a color is transparent:
		//if(color.ToArgb()==Color.Transparent.ToArgb()){

		#region Fields - Public
		//Yes, these are public static fields, an exception to the rule because they are unchanging and truly globally used.
		///<summary>This slightly bluish offwhite looks more pure than white and keeps the contrast intact.</summary>
		public static Color Background=Color.FromArgb(252,253,254);//
		///<summary>This replaces SystemColors.Control.  Gray 240.</summary>
		public static Color Control=Gray(240);
		///<summary>This is for gridlines where the background is white.  It's gray 180.  GridOD is currently 180.</summary>
		public static Color Gridline=Gray(180);
		///<summary>This is a pale blue color to help give users feedback when the hover over normally white areas.  Used in ListBoxOD and ImageSelector so far.  229,239,251.</summary>
		public static Color Hover=Color.FromArgb(229,239,251);
		///<summary>This is for outlines of controls.  It's gray 90.</summary>
		public static Color Outline=Gray(90);
		#endregion Fields - Public

		#region Fields - Private
		private static SolidBrush _brushControl;
		private static Pen _penControl;
		#endregion Fields - Private

		#region Methods
		///<summary>Static brush for SystemColors.Control, gray 240.  Never disposed.</summary>
		public static SolidBrush BrushControl(){
			if(_brushControl is null){
				_brushControl=new SolidBrush(Control);
			}
			return _brushControl;
		}

		///<summary>Examples: Siver=192, LightGray=211, Control=240</summary>
		public static Color Gray(int saturation){
			return Color.FromArgb(saturation,saturation,saturation);
		}

		/// <summary>Example: amounts of 1 and 4 will be more like the second color.  Amounts of 5 and 1 will be more like the first color.</summary>
		public static Color Mix(Color color1,Color color2,float amount1,float amount2){
			Color color=Color.FromArgb(
				(int)((amount1*color1.R+amount2*color2.R)/(amount1+amount2)),
				(int)((amount1*color1.G+amount2*color2.G)/(amount1+amount2)),
				(int)((amount1*color1.B+amount2*color2.B)/(amount1+amount2)));
			return color;
		}

		///<summary>Static pen for SystemColors.Control, gray 240.  Never disposed.</summary>
		public static Pen PenControl(){
			if(_penControl is null){
				_penControl=new Pen(Control);
			}
			return _penControl;
		}
		#endregion Methods
		



	}
}
