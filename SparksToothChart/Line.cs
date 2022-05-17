using System;
using System.Collections.Generic;
using System.Text;

namespace SparksToothChart {
	///<summary>A series of vertices that are all connected into one continuous simple line.</summary>
	public class LineSimple {
		public List<Vertex3f> Vertices;

		///<summary></summary>
		public LineSimple() {
			Vertices=new List<Vertex3f>();
		}

		///<summary>Specify a line as a series of points.  It's implied that they are grouped by threes.</summary>
		public LineSimple(params float[] coords) {
			Vertices=new List<Vertex3f>();
			Vertex3f vertex=new Vertex3f();
			for(int i=0;i<coords.Length;i++) {
				vertex.X=coords[i];
				i++;
				vertex.Y=coords[i];
				i++;
				vertex.Z=coords[i];
				Vertices.Add(vertex);
				vertex=new Vertex3f();
			}
		}
	}
}
