using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SparksToothChart {
	///<summary>A 3D object may only have one material.</summary>
	public class D3Object {
		///<summary>Pulled from the file group name.  Automatically includes object_group.  For example "cylinder2_default".</summary>
		public string Name;
		///<summary>Just the VTNs for this object/group.</summary>
		public List<VertexNormal> VertexNormals;
		///<summary>The faces in this object.  Each face keeps track of its own vertex/normal indices.</summary>
		public List<Face> Faces;
		///<summary>Not required.  This overrides the D3ObjectGroup.TextureMap.  If null, the D3ObjectGroup.TextureMap will be used.</summary>
		public Bitmap TextureMap;

		public D3Object() {
			VertexNormals=new List<VertexNormal>();
			Faces=new List<Face>();
		}

		///<summary>Tries to find an existing VertexNormal in the list for this object.  If it can, then it returns that index.  If it can't then it adds this VertexNormal to the list and returns the last index.</summary>
		public int GetIndexForVertNorm(VertexNormal vertnorm) {
			for(int i=0;i<VertexNormals.Count;i++) {
				if(VertexNormals[i].Vertex.X != vertnorm.Vertex.X){
					continue;
				}
				if(VertexNormals[i].Vertex.Y != vertnorm.Vertex.Y){
					continue;
				}
				if(VertexNormals[i].Vertex.Z != vertnorm.Vertex.Z){
					continue;
				}
				if(VertexNormals[i].Normal.X != vertnorm.Normal.X){
					continue;
				}
				if(VertexNormals[i].Normal.Y != vertnorm.Normal.Y){
					continue;
				}
				if(VertexNormals[i].Normal.Z != vertnorm.Normal.Z){
					continue;
				}
				//match
				return i;
			}
			//couldn't find
			VertexNormals.Add(vertnorm);
			return VertexNormals.Count-1;
		}

		public Point3DCollection GenerateVertices() {
			Point3DCollection points=new Point3DCollection();
			Point3D point;
			for(int i=0;i<VertexNormals.Count;i++) {
				point=new Point3D(VertexNormals[i].Vertex.X,VertexNormals[i].Vertex.Y,VertexNormals[i].Vertex.Z);
				points.Add(point);
			}
			return points;
		}

		public PointCollection GenerateTextures() {
			PointCollection points=new PointCollection();
			System.Windows.Point point;
			for(int i=0;i<VertexNormals.Count;i++) {
				if(VertexNormals[i].Texture==null) {
					points.Add(new System.Windows.Point(0,0));
				}
				else {
					point=new System.Windows.Point(VertexNormals[i].Texture.X,VertexNormals[i].Texture.Y);
					points.Add(point);
				}
			}
			return points;
		}

		public Vector3DCollection GenerateNormals() {
			Vector3DCollection vectors=new Vector3DCollection();
			Vector3D vector;
			for(int i=0;i<VertexNormals.Count;i++) {
				vector=new Vector3D(VertexNormals[i].Normal.X,VertexNormals[i].Normal.Y,VertexNormals[i].Normal.Z);
				vectors.Add(vector);
			}
			return vectors;
		}

		public Int32Collection GenerateIndices() {
			Int32Collection indices=new Int32Collection();
			for(int i=0;i<Faces.Count;i++) {
				indices.Add(Faces[i].IndexList[0]);
				indices.Add(Faces[i].IndexList[1]);
				indices.Add(Faces[i].IndexList[2]);
			}
			return indices;
		}

	}
}
