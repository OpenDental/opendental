using System;
using System.Collections.Generic;
using System.Text;

namespace SparksToothChart {
	///<summary></summary>
	public class Vertex3f {
		public float X;
		public float Y;
		public float Z;

		public Vertex3f() {

		}

		public Vertex3f(float x,float y,float z) {
			X=x;
			Y=y;
			Z=z;
		}

		public float[] GetFloatArray() {
			float[] retVal=new float[3];
			retVal[0]=X;
			retVal[1]=Y;
			retVal[2]=Z;
			return retVal;
		}

		public override string ToString() {
			return X.ToString()+","+Y.ToString()+","+Z.ToString();
		}

		public Vertex3f Copy() {
			Vertex3f vf=new Vertex3f(this.X,this.Y,this.Z);
			return vf;
		}
	}
}
