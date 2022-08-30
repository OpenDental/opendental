using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CodeBase {
	public class ODMathLib {

		public static void Swap(ref float x1,ref float x2){
			float temp=x1;
			x1=x2;
			x2=temp;
		}

		///<summary>Returns the closed intersection of the given two segments [x1,x2] and [x3,x4]. Returns no floats is there is no intersection and returns 2 floats if the intersection is a segment (although both segment points may be the same).</summary>
		public static float[] IntersectSegments(float x1,float x2,float x3,float x4) {
			if(x2<x1) {
				Swap(ref x1,ref x2);
			}
			if(x4<x3) {
				Swap(ref x3,ref x4);
			}
			if(x4<x1 || x3>x2) {
				return new float[0];//No intersection.
			}
			return new float[2] { Math.Max(x1,x3),Math.Min(x2,x4) };
		}

		///<summary>Returns the intersection of the given two rectangles as a set of 4 floats in the order (x,y,w,h), or returns an array of 0 floats if the two rectangles do not intersect.  jordan Probably obsolete and needs to be removed soon.</summary>
		public static float[] IntersectRectangles(float x1,float y1,float w1,float h1,
																												float x2,float y2,float w2,float h2) {
			float[] xIntersect=IntersectSegments(x1,x1+w1,x2,x2+w2);
			float[] yIntersect=IntersectSegments(y1,y1+h1,y2,y2+h2);
			if(xIntersect.Length==0 || yIntersect.Length==0) {//No intersection?
				return new float[0];
			}
			return new float[4] {xIntersect[0],yIntersect[0],
				xIntersect[1]-xIntersect[0],yIntersect[1]-yIntersect[0]};
		}

		///<summary>Equivalent to Math.Max for DateTimes.</summary>
		public static DateTime Max(DateTime dateTime1,DateTime dateTime2) {
			if(dateTime2 > dateTime1) {
				return dateTime2;
			}
			return dateTime1;
		}

		///<summary>Equivalent to Math.Min for DateTimes.</summary>
		public static DateTime Min(DateTime dateTime1,DateTime dateTime2) {
			if(dateTime2 < dateTime1) {
				return dateTime2;
			}
			return dateTime1;
		}
	}
}
