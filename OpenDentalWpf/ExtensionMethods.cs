using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace OpenDentalWpf {
	public static class ExtensionMethods {
		///<summary>Extension Method to get the top of the shape.  Only works so far if parent control is a Canvas and if the shape is set relative to </summary>
		public static double Top(this Rectangle rect){
			return Canvas.GetTop(rect);
		}

		///<summary>Extension Method to get the bottom of the shape.  Only works so far if parent control is a Canvas.</summary>
		public static double Bottom(this Rectangle rect) {
			return Canvas.GetTop(rect)+rect.Height;
		}

		///<summary>Extension Method to get the left of the shape.  Only works so far if parent control is a Canvas.</summary>
		public static double Left(this Rectangle rect) {
			return Canvas.GetLeft(rect);
		}

		///<summary>Extension Method to get the right of the shape.  Only works so far if parent control is a Canvas.</summary>
		public static double Right(this Rectangle rect) {
			return Canvas.GetLeft(rect)+rect.Width;
		}

	}
}
