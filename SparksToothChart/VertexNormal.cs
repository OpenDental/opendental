using System;
using System.Collections.Generic;
using System.Text;

namespace SparksToothChart {
	///<summary>Contains one vertex (xyz), one normal, and possibly one texture coordinate.</summary>
	public class VertexNormal {
		public Vertex3f Vertex;
		public Vertex3f Normal;
		///<summary>2D, So the third value is always zero.  Values are between 0 and 1.  Can be null.</summary>
		public Vertex3f Texture;

		public override string ToString() {
			string retVal="v:"+Vertex.ToString()+" n:"+Normal.ToString();
			if(Texture!=null) {
				retVal+=" t:"+Texture.ToString();
			}
			return retVal;
		}

		public VertexNormal Copy() {
			VertexNormal vn=new VertexNormal();
			vn.Vertex=this.Vertex.Copy();
			vn.Normal=this.Normal.Copy();
			if(vn.Texture!=null) {
				vn.Texture=this.Texture.Copy();
			}
			return vn;
		}
	}
}
