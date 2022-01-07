using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SparksToothChart {
	///<summary>3D objects built in Wings3D may have multiple objects in a single file.  They may also have multiple groups per object.  As they are imported, both are converted to D3Objects of single material/color specs.  The indices are all reworked to be per object. This loosely corresponds to System.Windows.Media.Media3D.Model3DGroup, but without the lights.</summary>
	public class D3ObjectGroup {
		///<summary>Just directly manipulate the public collection.</summary>
		public List<D3Object> D3Objects;
		///<summary>Can be null.  This applies to the entire group of objects.  Individual objects can have their own TextureMaps that override this one.</summary>
		public Bitmap TextureMap;

		public D3ObjectGroup(){
			D3Objects=new List<D3Object>();
			
		}
	}
}
