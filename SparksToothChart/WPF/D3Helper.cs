using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace SparksToothChart {
	///<summary>This class is also used in other projects such as Oregon Cryonics.  Only Jordan should touch it, to avoid damage.  It knows nothing about teeth, and is more general purpose.</summary>
	public class D3Helper {
		public static D3ObjectGroup ImportObjectsFromObjFile(string strObjData) {
			D3ObjectGroup d3ObjectGroup=new D3ObjectGroup();
			string[] lines=strObjData.Split(new string[] {"\r\n"},StringSplitOptions.None);
			int i=0;
			List<Vertex3f> listV=new List<Vertex3f>();
			List<Vertex3f> listT=new List<Vertex3f>();
			List<Vertex3f> listN=new List<Vertex3f>();
			Vertex3f vertex;
			Vertex3f texture;
			Vertex3f normal;
			VertexNormal vertnorm;
			D3Object d3Object=null;
			string[] items;
			string[] subitems;
			int idxV;
			int idxT;
			int idxN;
			Face face;
			while(i<lines.Length){
				//Debug.WriteLine(i.ToString());
				if(lines[i].StartsWith("#")//comment
						|| lines[i].StartsWith("mtllib")//material library.  We build our own.
						|| lines[i].StartsWith("usemtl"))//use material
				{
					i++;
					continue;//we don't care about any of these
				}
				if(lines[i].StartsWith("o")) {//object.  For example "o cube1"
					//vertices and normals are per object as they are pulled out of the file,
					//but we need to regroup them per d3Object.
					i++;
					continue;
				}
				if(lines[i].StartsWith("v ")) {//vertex.  For example "v -0.34671119 0.22176000 0.83703486"
					items=lines[i].Split(' ');
					vertex=new Vertex3f();
					vertex.X=Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
					vertex.Y=Convert.ToSingle(items[2],CultureInfo.InvariantCulture);
					vertex.Z=Convert.ToSingle(items[3],CultureInfo.InvariantCulture);
					listV.Add(vertex);
					i++;
					continue;
				}
				if(lines[i].StartsWith("vt")) {//vertex.  For example "vt 0.72553915 0.63685900"
					items=lines[i].Split(' ');
					texture=new Vertex3f();
					texture.X=Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
					texture.Y=Convert.ToSingle(items[2],CultureInfo.InvariantCulture);
					//texture.Z=Convert.ToSingle(items[3],CultureInfo.InvariantCulture);
					listT.Add(texture);
					i++;
					continue;
				}
				if(lines[i].StartsWith("vn")) {//normal.  For example "vn -0.32559605 -0.84121753 -0.43167149"
					items=lines[i].Split(' ');
					normal=new Vertex3f();
					normal.X=Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
					normal.Y=Convert.ToSingle(items[2],CultureInfo.InvariantCulture);
					normal.Z=Convert.ToSingle(items[3],CultureInfo.InvariantCulture);
					listN.Add(normal);
					i++;
					continue;
				}
				if(lines[i].StartsWith("g")) {//group.  For example "g cylinder2_default"
					if(d3Object!=null) {
						d3ObjectGroup.D3Objects.Add(d3Object);//add the previous object to the group
					}
					d3Object=new D3Object();//start the new one, including the new list of VNs
					d3Object.Name=lines[i].Substring(2);
					i++;
					continue;
				}
				if(lines[i].StartsWith("f")) {//face.  For example "f 12//384 51//443 384//336 383//335".  Format is v//n
					items=lines[i].Split(' ');//5 items in this example
					//create triangle fan, preserving counterclockwise order
					for(int f=2;f<items.Length-1;f++) {//one face/triangle per loop, 2 triangles in this case
						face=new Face();
						//only grab three vertices
						//first vertex is always the 0 for the fan
						subitems=items[1].Split('/');//12,,384 for the first triangle
						idxV=Convert.ToInt32(subitems[0])-1;//11 for the first triangle
						if(subitems[1]=="") {//no texture
							idxT=-1;
						}
						else {
							idxT=Convert.ToInt32(subitems[1])-1;
						}
						idxN=Convert.ToInt32(subitems[2])-1;//383 for the first triangle
						vertnorm=new VertexNormal();
						vertnorm.Vertex=listV[idxV];
						if(idxT!=-1) {
							vertnorm.Texture=listT[idxT];
						}
						vertnorm.Normal=listN[idxN];
						face.IndexList.Add(d3Object.GetIndexForVertNorm(vertnorm));
						//second vertex
						subitems=items[f].Split('/');//51,,443 for the first triangle
						idxV=Convert.ToInt32(subitems[0])-1;//50 for the first triangle
						if(subitems[1]=="") {//no texture
							idxT=-1;
						}
						else {
							idxT=Convert.ToInt32(subitems[1])-1;
						}
						idxN=Convert.ToInt32(subitems[2])-1;//442 for the first triangle
						vertnorm=new VertexNormal();
						vertnorm.Vertex=listV[idxV];
						if(idxT!=-1) {
							vertnorm.Texture=listT[idxT];
						}
						vertnorm.Normal=listN[idxN];
						face.IndexList.Add(d3Object.GetIndexForVertNorm(vertnorm));
						//third vertex
						subitems=items[f+1].Split('/');//384,,336 for the first triangle
						idxV=Convert.ToInt32(subitems[0])-1;//383 for the first triangle
						if(subitems[1]=="") {//no texture
							idxT=-1;
						}
						else {
							idxT=Convert.ToInt32(subitems[1])-1;
						}
						idxN=Convert.ToInt32(subitems[2])-1;//335 for the first triangle
						vertnorm=new VertexNormal();
						vertnorm.Vertex=listV[idxV];
						if(idxT!=-1) {
							vertnorm.Texture=listT[idxT];
						}
						vertnorm.Normal=listN[idxN];
						face.IndexList.Add(d3Object.GetIndexForVertNorm(vertnorm));
						d3Object.Faces.Add(face);
					}
					i++;
					continue;
				}
				//might be another kind of row that we missed above
				i++;
			}
			if(d3Object!=null) {
				d3ObjectGroup.D3Objects.Add(d3Object);//add the last object to the group
			}
			return d3ObjectGroup;
		}

		public static BitmapImage ConvertImage(System.Drawing.Bitmap bitmap){
			using(MemoryStream memory=new MemoryStream()) {
				bitmap.Save(memory,ImageFormat.Bmp);//If we ever want to preserve transparency, we need to change this to png
				memory.Position=0;
				BitmapImage bitmapImage=new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource=memory;
				//bitmapImage.StreamSource=new MemoryStream(memory.ToArray());//this line might be an improvement over the one above?
				bitmapImage.CacheOption=BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				return bitmapImage;
			}
		}
	}
}
