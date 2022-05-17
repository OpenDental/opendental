using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SharpDX.Direct3D9;

namespace SparksToothChart {
	///<summary>For 3D tooth graphics, this is a group of faces within a single tooth.  Different groups can be assigned different colors and visibility.  Groups might include enamel, the filling surfaces, pulp, canals, and cementum.  If a group does not apply, such as a F on a posterior tooth, then that tooth will not have that group.  The code must be resiliant enough to handle missing groups.  We might add more groups later and subdivide existing groups.  For instance pits, grooves, cusps, cervical areas, etc.  Over the years, this could get quite extensive and complex.  We would have to add a table to the database to handle additional groups painted by user.</summary>
	public class ToothGroup {
		///<summary></summary>
		public bool Visible;
		///<summary></summary>
		public Color PaintColor;
		///<summary></summary>
		public ToothGroupType GroupType;
		///<summary>dim 1=the face. dim 2=the vertex. dim 3 always has length=2, with 1st vertex, and 2nd normal.</summary>
		public List<Face> Faces;
		///<summary>Corresponds to the Faces list. Indicies must be cached to draw DirectX triangles.</summary>
		public IndexBuffer facesDirectX=null;
		///<summary>Corresponds to the number of indicies referenced by the facesDirectX IndexBuffer. This relates to triangles, not to Polygons as in the Faces list. Must be a multiple of 3.</summary>
		public int NumIndicies=0;

		public ToothGroup() {
			Faces=new List<Face>();
		}

		///<summary>If using DirectX, the facesDirectX IndexBuffer variable must be instantiated in a subsequent call to PrepareForDirectX().</summary>
		public ToothGroup Copy() {
			ToothGroup tg=new ToothGroup();
			tg.Visible=this.Visible;
			tg.PaintColor=this.PaintColor;
			tg.GroupType=this.GroupType;
			tg.Faces=new List<Face>();
			for(int i=0;i<this.Faces.Count;i++) {
				tg.Faces.Add(this.Faces[i].Copy());
			}
			return tg;
		}

		public void PrepareForDirectX(Device device){
			CleanupDirectX();
			//When drawing with a single index buffer inside of DirectX, all primitives must be the same type.
			//Furthermore, there are no polygons inside of DirectX, only triangles. Therefore, at this point
			//we break down all faces from polygons into triangles inside of the triangle list so all faces
			//can be drawn using triangles only. This also allows us to optimize face order for increased
			//rendering efficiency.
			List <int> listTriangles=new List<int>();
			for(int i=0;i<Faces.Count;i++) {
				for(int j=1;j<Faces[i].IndexList.Count-1;j++) {
					//We create a triangle fan out of the indices here for simplicity, preserving the original polygon normal directionality.
					listTriangles.Add(Faces[i].IndexList[0]);
					listTriangles.Add(Faces[i].IndexList[j]);
					listTriangles.Add(Faces[i].IndexList[j+1]);
				}
			}
			//Calculate the number of unique indicies so we know exactly now many verticies are referenced
			//and then we can properly optimize our face order with OptimizeFaces() below.
			List <int> uniqueIndicies=new List<int> ();
			for(int i=0;i<listTriangles.Count;i++){
				if(uniqueIndicies.IndexOf(listTriangles[i])<0){
					uniqueIndicies.Add(listTriangles[i]);
				}
			}
			//Optimize the triangle order for excellent vertex caching.
			//OptimizeFaces() returns a list of triangle indices which must be converted back into a list vertex indices (3 per triangle).
			int[] arrayOptimizedIndices=SharpDX.Direct3D9.D3DX.OptimizeFaces(listTriangles.ToArray(),listTriangles.Count/3,uniqueIndicies.Count);
			List <int> listOptimalTriangles=new List<int>();
			for(int f=0;f<arrayOptimizedIndices.Length;f++) {//Each value in arrayOptimizedIndicies represents one triangle.  Enumerating triangles.
				int firstVertexIndex=3*arrayOptimizedIndices[f];//Index of first vertex for current triangle.
				listOptimalTriangles.Add(listTriangles[firstVertexIndex]);
				listOptimalTriangles.Add(listTriangles[firstVertexIndex+1]);
				listOptimalTriangles.Add(listTriangles[firstVertexIndex+2]);
			}
			int[] arrayTriangles=listOptimalTriangles.ToArray();
			facesDirectX=new IndexBuffer(device,sizeof(int)*arrayTriangles.Length,Usage.None,Pool.Managed,false);
			SharpDX.DataStream ds=facesDirectX.Lock(0,0,LockFlags.None);
			ds.WriteRange(arrayTriangles);
			facesDirectX.Unlock();
			NumIndicies=arrayTriangles.Length;
		}

		public void DrawDirectX(Device device,ToothGraphic parentGraphic) {
			device.Indices=facesDirectX;
			device.DrawIndexedPrimitive(PrimitiveType.TriangleList,0,0,parentGraphic.VertexNormals.Count,0,NumIndicies/3);
		}

		public void CleanupDirectX(){
			if(facesDirectX!=null){
				facesDirectX.Dispose();
				facesDirectX=null;
			}
		}

		public override string ToString() {
			return GroupType.ToString()+". Faces:"+Faces.Count.ToString();
		}


	}

	///<summary></summary>
	public enum ToothGroupType{
		///<summary>0- This is areas of the tooth that are not included in any fillings at all.</summary>
		Enamel,
		///<summary>1</summary>
		Cementum,
		///<summary>2</summary>
		M,
		///<summary>3</summary>
		O,
		///<summary>4</summary>
		D,
		///<summary>5</summary>
		B,
		///<summary>6</summary>
		L,
		///<summary>7</summary>
		F,
		///<summary>8</summary>
		I,
		///<summary>9. class V. In addition to B or F</summary>
		V,
		///<summary>10. The pulp chamber and post or buildup.  Because our teeth are not yet transparent, these faces need to be translated forward.</summary>
		Buildup,
		///<summary>11. Only defined on some anterior teeth.  The small bit of enamel on the F that is not included in any filling.</summary>
		EnamelF,
		/// <summary>12. Only defined on some anterior teeth.  The F portion of the D filling.</summary>
		DF,
		/// <summary>13. Only defined on some anterior teeth.  The F portion of the M filling.</summary>
		MF,
		/// <summary>14. Only defined on some anterior teeth.  The F portion of the I filling.</summary>
		IF,
		///<summary>Only present in the special implant tooth object.</summary>
		Implant,
		///<summary>Not used. Just a placeholder</summary>
		Canals,
		///<summary>Used where there are unknown materials present such as 'default'.  Always ignore these groups.</summary>
		None
	}
}
