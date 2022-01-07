using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SparksToothChart {
	/// <summary>This control is also used in other projects such as Oregon Cryonics.  Only Jordan should touch it, to avoid damage.  It knows nothing about teeth, and is just a container where structured 3D drawing can take place.  A crash inside this control will not nicely show in the debugger.</summary>
	public partial class Wpf3dControl:UserControl {
		private Model3DGroup _model3Dgroup;
		///<summary>1:1 relationship with the objects.  Helps us find the correct object later when making modifications.</summary>
		private List<string> objectNames;

		public Wpf3dControl() {
			InitializeComponent();
			AddCamera();
		}

		private void AddCamera() {
			PerspectiveCamera camera=new PerspectiveCamera();
			camera.Position=new Point3D(0,3.3,8);//in WPF, z axis is positive coming out from the screen toward the viewer.
			//camera.FarPlaneDistance=20;
			camera.LookDirection=new Vector3D(0,-.4,-1);
			//camera.UpDirection=new Vector3D(0,1,0);
			//camera.NearPlaneDistance=0;
			camera.FieldOfView=45;
			myViewport.Camera=camera;
		}

		public void SetCamera(Point3D position,Vector3D lookDirection,double fieldOfView) {
			PerspectiveCamera camera=(PerspectiveCamera)myViewport.Camera;
			camera.Position=position;
			camera.LookDirection=lookDirection;
			camera.FieldOfView=fieldOfView;
		}

		///<summary>Objects are added in this step as all gray.  Colors are set in a separate step.</summary>
		public void AddObjectGroup(D3ObjectGroup group) {
			_model3Dgroup=new Model3DGroup();		
			DirectionalLight light=new DirectionalLight(Colors.WhiteSmoke,new Vector3D(-3,-3,-3));
			_model3Dgroup.Children.Add(light);
			DirectionalLight light2=new DirectionalLight(Colors.LightGray,new Vector3D(2.8,-3,-3));
			_model3Dgroup.Children.Add(light2);
			//AmbientLight ambientLight=new AmbientLight(Colors.DarkGray);
			//_model3Dgroup.Children.Add(ambientLight);
			objectNames=new List<string>();
			objectNames.Add("light1");
			objectNames.Add("light2");//to maintain 1:1
			for(int i=0;i<group.D3Objects.Count;i++) {
				MeshGeometry3D meshGeometry3D=new MeshGeometry3D();
				Point3DCollection points=group.D3Objects[i].GenerateVertices();
				meshGeometry3D.Positions=points;
				PointCollection textures=group.D3Objects[i].GenerateTextures();
				meshGeometry3D.TextureCoordinates=textures;
				Vector3DCollection vectors=group.D3Objects[i].GenerateNormals();
				meshGeometry3D.Normals=vectors;
				Int32Collection indices=group.D3Objects[i].GenerateIndices();
				meshGeometry3D.TriangleIndices=indices;
				GeometryModel3D geometryModel3D=new GeometryModel3D();
				geometryModel3D.Geometry=meshGeometry3D;
				//materials
				MaterialGroup materialGroup=new MaterialGroup();
				DiffuseMaterial diffuseMaterial=new DiffuseMaterial();
				if(group.D3Objects[i].TextureMap!=null) {
					ImageBrush imageBrush=new ImageBrush(D3Helper.ConvertImage(group.D3Objects[i].TextureMap));
					imageBrush.ViewportUnits=BrushMappingMode.Absolute;
					ScaleTransform scaleTransform=new ScaleTransform(1,-1);//scale y -1 to flip vertically
					TranslateTransform translateTransform=new TranslateTransform(0,1);//shift up one after flipping
					TransformGroup transformGroup=new TransformGroup();
					transformGroup.Children.Add(scaleTransform);
					transformGroup.Children.Add(translateTransform);
					imageBrush.Transform=transformGroup;
					diffuseMaterial.Brush=imageBrush;
				}
				else if(group.TextureMap!=null && group.D3Objects[i].VertexNormals[0].Texture!=null) {//a group texture is specified and this object uses texture mapping
					ImageBrush imageBrush=new ImageBrush(D3Helper.ConvertImage(group.TextureMap));
					imageBrush.ViewportUnits=BrushMappingMode.Absolute;
					ScaleTransform scaleTransform=new ScaleTransform(1,-1);//scale y -1 to flip vertically
					TranslateTransform translateTransform=new TranslateTransform(0,1);//shift up one after flipping
					TransformGroup transformGroup=new TransformGroup();
					transformGroup.Children.Add(scaleTransform);
					transformGroup.Children.Add(translateTransform);
					imageBrush.Transform=transformGroup;
					diffuseMaterial.Brush=imageBrush;
				}
				else {
					diffuseMaterial.Brush=new SolidColorBrush(Colors.Gray);
					//diffuseMaterial.Color=Colors.Gray;//this didn't work.  Needs brush.
				}
				materialGroup.Children.Add(diffuseMaterial);
				//specular material at 1
				SpecularMaterial specularMaterial=new SpecularMaterial();
				specularMaterial.Brush=new SolidColorBrush(Colors.White);
				specularMaterial.SpecularPower=150;//smaller numbers give more reflection.  150 is minimal specular.
				materialGroup.Children.Add(specularMaterial);
				geometryModel3D.Material=materialGroup;
				_model3Dgroup.Children.Add(geometryModel3D);
				objectNames.Add(group.D3Objects[i].Name);
			}
			ModelVisual3D modelVisual3D=new ModelVisual3D();
			modelVisual3D.Content=_model3Dgroup;
			myViewport.Children.Add(modelVisual3D);
		}

		///<summary></summary>
		public void SetDiffuseColor(string objName,System.Drawing.Color color) {
			if(!objectNames.Contains(objName)){
				return;
			}
			Color colorw=Color.FromArgb(color.A,color.R,color.G,color.B);
			MaterialGroup materialGroup=(MaterialGroup)((GeometryModel3D)_model3Dgroup.Children[objectNames.IndexOf(objName)]).Material;
			((DiffuseMaterial)materialGroup.Children[0]).Brush=new SolidColorBrush(colorw);//by convention, our diffuse material is at 0
			//material.Brush=new SolidColorBrush(colorw);
		}

		///<summary></summary>
		public void SetSpecular(string objName,double specularPower) {
			if(!objectNames.Contains(objName)) {
				return;
			}
			Color colorw=Colors.White;//Color.FromArgb(color.A,color.R,color.G,color.B);
			MaterialGroup materialGroup=(MaterialGroup)((GeometryModel3D)_model3Dgroup.Children[objectNames.IndexOf(objName)]).Material;
			SpecularMaterial material=(SpecularMaterial)materialGroup.Children[1];//by convention, our specular material is at 1
			material.Brush=new SolidColorBrush(colorw);
			material.SpecularPower=specularPower;
		}

		///<summary>Of the diffuse material, whether solid or texture.</summary>
		public void SetOpacity(string objName,double opacity) {
			if(!objectNames.Contains(objName)) {
				return;
			}
			MaterialGroup materialGroup=(MaterialGroup)((GeometryModel3D)_model3Dgroup.Children[objectNames.IndexOf(objName)]).Material;
			DiffuseMaterial material=(DiffuseMaterial)materialGroup.Children[0];//by convention, our diffuse material is at 0
			material.Brush.Opacity=opacity;
		}

		///<summary>Rotate not included yet.  Completely overrides existing instead of any additive effect.</summary>
		public void SetTransform(string objName,Vector3D offset,Vector3D scale) {
			if(!objectNames.Contains(objName)) {
				return;
			}
			GeometryModel3D geometryModel3D=(GeometryModel3D)_model3Dgroup.Children[objectNames.IndexOf(objName)];
			TranslateTransform3D translateTransform3D=new TranslateTransform3D(offset);
			ScaleTransform3D scaleTransform3D=new ScaleTransform3D(scale);
			Transform3DGroup transform3DGroup=new Transform3DGroup();
			transform3DGroup.Children.Add(translateTransform3D);
			transform3DGroup.Children.Add(scaleTransform3D);
			geometryModel3D.Transform=transform3DGroup;
		}

	}
}
