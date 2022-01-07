using System;
using System.Collections.Generic;
using System.Text;

namespace SparksToothChart {
	///<summary>A face is a single polygon, usually a rectangle.  Will soon be only triangles.</summary>
	public class Face {
		//public List<VertexNormal> VertexNormals;
		///<summary>A list of indices to the VertexNormal list contained in the ToothGraphic object.  0 indexed, unlike the raw files which are 1 indexed.  Always exactly 3 for WPF.</summary>
		public List<int> IndexList;

		public Face() {
			//VertexNormals=new List<VertexNormal>();
			IndexList=new List<int>();
		}

		public override string ToString() {
			string retVal="";
			for(int i=0;i<IndexList.Count;i++) {
				if(i>0) {
					retVal+=",";
				}
				retVal+=IndexList[i].ToString();
			}
			return retVal;
		}

		public Face Copy() {
			Face f=new Face();
			f.IndexList=new List<int>(this.IndexList);
			return f;
		}
	}
}
