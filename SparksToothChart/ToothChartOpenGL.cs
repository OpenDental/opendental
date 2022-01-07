/*=============================================================================================================
Copyright (C) 2006  Jordan Sparks, DMD.  http://www.open-dent.com,  http://www.docsparks.com

This program is free software; you can redistribute it and/or modify it under the terms of the
GNU Db Public License as published by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

This program is distributed in the hope that it will be useful, but without any warranty. See the GNU Db Public License
for more details, available at http://www.opensource.org/licenses/gpl-license.php

Any changes to this program must follow the guidelines of the GPL license if a modified version is to be
redistributed.
===============================================================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using Tao.OpenGl;
using Tao.Platform.Windows;
using OpenDentBusiness;

namespace SparksToothChart {
	public partial class ToothChartOpenGL:CodeBase.OpenGLWinFormsControl{//.SimpleOpenGlControl {
		float[] specular_color_normal;//white
		float[] specular_color_cementum;//gray
		float[] shininess;		
		private bool MouseIsDown;
		///<summary>Mouse move causes this variable to be updated with the current tooth that the mouse is hovering over.</summary>
		private string hotTooth;
		///<summary>The previous hotTooth.  If this is different than hotTooth, then mouse has just now moved to a new tooth.  Can be 0 to represent no previous.</summary>
		private string hotToothOld;
		///<summary>The base offset of the first GL display list for the first font character.  Every other character is in seqeuence from this base offset.</summary>
		private int fontOffset;
		private int displayListOffset;
		///<summary>An array of all supported font symbols such that the first index is the character value (such as '9') and the second index is a pixel row indicating which pixels are on (1) and which are off (0).</summary>
		private string[][] fontsymbols;
		//<summary>This gets set to true during certain operations where we do not need to redraw all the teeth.  Specifically, during tooth selection where only the color of the tooth number text needs to change.  In this case, the rest of the scene will not be rendered again.</summary>
		//private bool suspendRendering;
		private int selectedPixelFormat;
		///<summary></summary>
		[Category("Action"),Description("Occurs when the mouse goes up ending a drawing segment.")]
		public event ToothChartDrawEventHandler SegmentDrawn=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when the mouse goes up committing tooth selection.")]
		public event ToothChartSelectionEventHandler ToothSelectionsChanged=null;
		///<summary>This is a reference to the TcData object that's at the wrapper level.</summary>
		public ToothChartData TcData;
		//<summary>GDI+ handle to this control. Used for line drawing and font measurement.</summary>
		//private Graphics g=null;
		private List<string> _listSelectedTeethOld=new List<string>();

		///<summary>Specify the hardware mode to create the tooth chart with. Set hardwareMode=true to try for hardware accelerated graphics, and set hardwareMode=false to try and get software graphics.</summary>
		public ToothChartOpenGL(bool hardwareMode,int preferredPixelFormatNum) {
			usehardware=hardwareMode;
			InitializeComponent();
			this.TaoSetupContext += new System.EventHandler(ToothChart_TaoSetupContext);
			this.TaoRenderScene += new System.EventHandler(ToothChart_TaoRenderScene);
			selectedPixelFormat=TaoInitializeContexts(preferredPixelFormatNum);
			TaoRenderEnabled=true;
			Gl.glDisable(Gl.GL_TEXTURE);
			//Disable texturing, since we don't use it.  This should prevent a glCopyPixels() problem in Gdi.SwapBuffersFast() on ATI graphics cards.
			/* This didn't work because the OpenGL control is not designed to be a container.  I don't have time to wrap it in another layer.
			pictBox=new PictureBox();
			pictBox.Location=new Point(0,0);
			pictBox.Name="pictBox";
			pictBox.Size=this.Size;
			pictBox.Dock=DockStyle.Fill;
			pictBox.Image=bitmapInPictBox;
			Controls.Add(pictBox);*/
		}

		public void InitializeGraphics() {
			MakeDisplayLists();
			//g=this.CreateGraphics();
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
		}

		private void ToothChart_TaoSetupContext(object sender, System.EventArgs e){//event from base class when context needs to be setup.
			MakeRasterFont();
		}

		public int SelectedPixelFormatNumber {
			get{
				return selectedPixelFormat;
			}
		}

		///<summary>Returns a bitmap of what is showing in the control.  Used for printing.</summary>
		public Bitmap GetBitmap(){
			//This doesn't seem to work sometimes
			//return GetBitmapOfOpenGL();
			//this seems convoluted
			Bitmap dummy=new Bitmap(Width,Height);
			Graphics g=Graphics.FromImage(dummy);
			PaintEventArgs e=new PaintEventArgs(g,new Rectangle(0,0,Width,Height));
			Render(e);
			//Bitmap result=ReadFrontBuffer();
			Bitmap result=GetBitmapOfOpenGL();
			g.Dispose();
			return result;
		}

		/// <summary>This only gets the 3D scene.</summary>
		private Bitmap GetBitmapOfOpenGL(){
			Gl.glFlush();
			Bitmap bitmap = new Bitmap(Width,Height);
			BitmapData bitmapData=bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.WriteOnly,PixelFormat.Format24bppRgb);
			Gl.glReadPixels(0,0,bitmap.Width,bitmap.Height,Gl.GL_BGR_EXT,Gl.GL_UNSIGNED_BYTE,bitmapData.Scan0);
			bitmap.UnlockBits(bitmapData);
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return bitmap; 
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
		}

		private void ToothChart_TaoRenderScene(object sender, System.EventArgs e){
			//if(suspendRendering){
			//	return;
			//}
			//This first part was originally in setup context
			Gl.glClearColor((float)TcData.ColorBackground.R/255f,(float)TcData.ColorBackground.G/255f,(float)TcData.ColorBackground.B/255f,0f);
			Gl.glClearAccum(0f,0f,0f,0f);
			//Lighting
			float ambI=.2f;//.1f;//Darker for testing
			float difI=.6f;//.3f;//Darker for testing
			float specI=1f;
			float[] light_ambient = new float[] { ambI,ambI,ambI,1f };//RGB,A=1 for no transparency. Default 0001
			float[] light_diffuse = new float[] { difI,difI,difI,1f };//RGBA. Default 1111. 'typical' 
			float[] light_specular = new float[] { specI,specI,specI,1f };//RGBA. Default 1111
			float[] light_position = new float[] { -0.5f,0.1f,1f,0f };//xyz(direction, not position), w=0 for infinite
			Gl.glLightfv(Gl.GL_LIGHT0,Gl.GL_AMBIENT,light_ambient);
			Gl.glLightfv(Gl.GL_LIGHT0,Gl.GL_DIFFUSE,light_diffuse);
			Gl.glLightfv(Gl.GL_LIGHT0,Gl.GL_SPECULAR,light_specular);
			Gl.glLightfv(Gl.GL_LIGHT0,Gl.GL_POSITION,light_position);
			//Materials
			Gl.glShadeModel(Gl.GL_SMOOTH);
			//OK to just set these three once.
			specular_color_normal = new float[] { 1.0f,1.0f,1.0f,1.0f };//1111 for white. RGBA
			specular_color_cementum = new float[] { 0.1f,0.1f,0.1f,1.0f };//gray
			shininess = new float[] { 90f };//0 to 128. Size of specular reflection. 128 smallest
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_LIGHT0);
			//Render Scene starts here----------------------------------------------------------------------------------
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);//Clears the color buffer and depth buffer.
			//viewing transformation.  gluLookAt is too complex, so not used
			//default was Z=1, looking towards the origin
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();//clears the matrix
			Gl.glMatrixMode(Gl.GL_PROJECTION);//only the projection matrix will be affected.
			Gl.glLoadIdentity();
			//double HeightProjection=WidthProjection*this.Height/this.Width;
			//Gl.glOrtho(-WidthProjection/2,WidthProjection/2,//orthographic projection. L,R
			//	-HeightProjection/2,HeightProjection/2,//Bot,Top
			//	-WidthProjection/2,WidthProjection/2);//Near,Far
			//double widthProj=(double)TcData.SizeOriginalProjection.Width;
			//double heightProj=widthProj*Height/Width;
			double heightProj=(double)TcData.SizeOriginalProjection.Height;
			double widthProj=(double)TcData.SizeOriginalProjection.Width;
			if(TcData.IsWide) {
				widthProj=heightProj*Width/Height;
			}
			else {//tall
				heightProj=widthProj*Height/Width;
			}
			Gl.glOrtho(-widthProj/2,widthProj/2,//orthographic projection. L,R
				-heightProj/2,heightProj/2,//Bot,Top
				-widthProj/2,widthProj/2);//Near,Far
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			//viewport transformation not used. Default is to fill entire control.
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glEnable(Gl.GL_LINE_SMOOTH);
			Gl.glHint(Gl.GL_LINE_SMOOTH_HINT,Gl.GL_DONT_CARE);
			DrawScene();
			//jitter code for antialias starts here, but I can't get it to work:
			//Gl.glFlush();//handled for me in base class
		}

		private void DrawScene() {
			ToothChartWrapper wrapper=(ToothChartWrapper)this.Parent;
			string wrappername=wrapper.Name;
			bool isRCT8=wrapper.TcData.ListToothGraphics["8"].IsRCT;
			for(int t=0;t<TcData.ListToothGraphics.Count;t++) {//loop through each tooth
				if(TcData.ListToothGraphics[t].ToothID=="implant") {//this is not an actual tooth.
					continue;
				}
				DrawFacialView(TcData.ListToothGraphics[t]);
				DrawOcclusalView(TcData.ListToothGraphics[t]);
			}
			// 
			//bitmapInPictBox=GetBitmapOfOpenGL();
			//gg=Graphics.FromImage(bitmapInPictBox);
			DrawWatches();
			DrawNumbersAndLines();
			DrawDrawingSegments();
			//g.DrawImage(bitmapInPictBox,0,0);
			Gl.glFlush();
		}

		private void DrawFacialView(ToothGraphic toothGraphic) {
			Gl.glPushMatrix();//remember position of origin
			Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),//Move the tooth to the correct position for facial view
				TcData.GetTransYfacial(toothGraphic.ToothID),
				0);
			RotateAndTranslateUser(toothGraphic);
			if(toothGraphic.Visible
				|| (toothGraphic.IsCrown && toothGraphic.IsImplant)
				|| toothGraphic.IsPontic)
			{
				DrawTooth(toothGraphic);
			}
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			if(toothGraphic.DrawBigX) {
				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glEnable(Gl.GL_BLEND);
				//move the bigX 6mm to the Facial so it will paint in front of the tooth
				Gl.glTranslatef(0,0,6f);
				Gl.glBlendFunc(Gl.GL_SRC_ALPHA,Gl.GL_ONE_MINUS_SRC_ALPHA);
				Gl.glLineWidth(2f*TcData.PixelScaleRatio);//thickness of line depends on size of window
				Gl.glColor3f (
					(float)toothGraphic.colorX.R/255f,
					(float)toothGraphic.colorX.G/255f,
					(float)toothGraphic.colorX.B/255f);
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
					Gl.glBegin(Gl.GL_LINES);
					Gl.glVertex2f(-2f,12f);
					Gl.glVertex2f(2f,-6f);
					Gl.glEnd();
					Gl.glBegin(Gl.GL_LINES);
					Gl.glVertex2f(2f,12f);
					Gl.glVertex2f(-2f,-6f);
					Gl.glEnd();
				}
				else{
					Gl.glBegin(Gl.GL_LINES);
					Gl.glVertex2f(-2f,6f);
					Gl.glVertex2f(2f,-12f);
					Gl.glEnd();
					Gl.glBegin(Gl.GL_LINES);
					Gl.glVertex2f(2f,6f);
					Gl.glVertex2f(-2f,-12f);
					Gl.glEnd();
				}
			}
			Gl.glPopMatrix();//reset to origin
			if(toothGraphic.Visible && toothGraphic.IsRCT) {//draw RCT
				Gl.glPushMatrix();
				Gl.glTranslatef(0,0,10f);//move RCT forward 10mm so it will be visible.
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),TcData.GetTransYfacial(toothGraphic.ToothID),0);
				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glColor3f(
					(float)toothGraphic.colorRCT.R/255f,
					(float)toothGraphic.colorRCT.G/255f,
					(float)toothGraphic.colorRCT.B/255f);
					//.5f);//only 1/2 darkness
				Gl.glBlendFunc(Gl.GL_SRC_ALPHA,Gl.GL_ONE_MINUS_SRC_ALPHA);
				Gl.glLineWidth(2.2f*TcData.PixelScaleRatio);
				Gl.glPointSize(1.8f*TcData.PixelScaleRatio);//point is slightly smaller since no antialiasing
				RotateAndTranslateUser(toothGraphic);
				List<LineSimple> lines=toothGraphic.GetRctLines();
				for(int i=0;i<lines.Count;i++){
					Gl.glBegin(Gl.GL_LINE_STRIP);
					for(int j=0;j<lines[i].Vertices.Count;j++){
						Gl.glVertex3f(lines[i].Vertices[j].X,lines[i].Vertices[j].Y,lines[i].Vertices[j].Z);
					}
					Gl.glEnd();
				}
				Gl.glPopMatrix();
				//This section is a necessary workaround for OpenGL.
				//It draws a point at each intersection to hide the unsightly transitions between line segments.
				Gl.glPushMatrix();
				Gl.glTranslatef(0,0,10.5f);//move forward 10.5mm so it will cover the lines
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),TcData.GetTransYfacial(toothGraphic.ToothID),0);
				RotateAndTranslateUser(toothGraphic);
				Gl.glDisable(Gl.GL_BLEND);
				for(int i=0;i<lines.Count;i++){
					Gl.glBegin(Gl.GL_POINTS);
					for(int j=0;j<lines[i].Vertices.Count;j++) {
						//but ignore the first and last.  We are only concerned with where lines meet.
						if(j==0 || j==lines[i].Vertices.Count-1) {
							continue;
						}
						Gl.glVertex3f(lines[i].Vertices[j].X,lines[i].Vertices[j].Y,lines[i].Vertices[j].Z);
					}
					Gl.glEnd();
				}
				Gl.glPopMatrix();
			}
			ToothGroup groupBU=toothGraphic.GetGroup(ToothGroupType.Buildup);//during debugging, not all teeth have a BU group yet.
			if(toothGraphic.Visible && groupBU!=null && groupBU.Visible) {//BU or Post
				Gl.glPushMatrix();
				Gl.glTranslatef(0,0,13f);//move BU forward 13mm so it will be visible.
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),TcData.GetTransYfacial(toothGraphic.ToothID),0);
				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glDisable(Gl.GL_BLEND);
				Color colorBU=toothGraphic.GetGroup(ToothGroupType.Buildup).PaintColor;
				Gl.glColor3f(
					(float)colorBU.R/255f,
					(float)colorBU.G/255f,
					(float)colorBU.B/255f);
				RotateAndTranslateUser(toothGraphic);
				Gl.glCallList(displayListOffset+toothGraphic.GetIndexForDisplayList(toothGraphic.GetGroup(ToothGroupType.Buildup)));
				//Triangle poly=toothGraphic.GetBUpoly();
				//Gl.glBegin(Gl.GL_POLYGON);
				//for(int i=0;i<poly.Vertices.Count;i++) {
				//	Gl.glVertex3f(poly.Vertices[i].X,poly.Vertices[i].Y,poly.Vertices[i].Z);
				//}
				//Gl.glEnd();
				Gl.glPopMatrix();
			}
			if(toothGraphic.IsImplant){
				Gl.glPushMatrix();
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),//Move the tooth to the correct position for facial view
					TcData.GetTransYfacial(toothGraphic.ToothID),
					0);
				RotateAndTranslateUser(toothGraphic);
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
					//flip the implant upside down
					Gl.glRotatef(180f,0,0,1f);
				}
				Gl.glEnable(Gl.GL_LIGHTING);
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glEnable(Gl.GL_DEPTH_TEST);
				ToothGroup group=(ToothGroup)TcData.ListToothGraphics["implant"].Groups[0];
				float[] material_color=new float[] {
					(float)toothGraphic.colorImplant.R/255f,
					(float)toothGraphic.colorImplant.G/255f,
					(float)toothGraphic.colorImplant.B/255f,
					(float)toothGraphic.colorImplant.A/255f
				};//RGBA
				Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_SPECULAR,specular_color_normal);
				Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_SHININESS,shininess);
				Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_AMBIENT_AND_DIFFUSE,material_color);
				Gl.glBlendFunc(Gl.GL_ONE,Gl.GL_ZERO);
				Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT,Gl.GL_NICEST);
				for(int i=0;i<group.Faces.Count;i++){//  .GetLength(0);i++) {//loop through each face
					Gl.glBegin(Gl.GL_POLYGON);
					for(int j=0;j<group.Faces[i].IndexList.Count;j++){//.Length;j++) {//loop through each vertex
						//The index for both will always be the same because we enforce a 1:1 relationship.
						//We show grabbing a float[3], but we could just as easily use the index itself.
						Gl.glVertex3fv(TcData.ListToothGraphics["implant"].VertexNormals[group.Faces[i].IndexList[j]].Vertex.GetFloatArray());//Vertices[group.Faces[i][j][0]]);
						Gl.glNormal3fv(TcData.ListToothGraphics["implant"].VertexNormals[group.Faces[i].IndexList[j]].Normal.GetFloatArray()); //.Normals[group.Faces[i][j][1]]);
					}
					Gl.glEnd();
				}
				Gl.glPopMatrix();
			}
		}

		private void DrawOcclusalView(ToothGraphic toothGraphic) {
			//now the occlusal surface. Notice that it's relative to origin again
			Gl.glPushMatrix();//remember position of origin
			Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),TcData.GetTransYocclusal(toothGraphic.ToothID),0);
			if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
				Gl.glRotatef(-110f,1f,0,0);//rotate angle about line from origin to x,y,z
			}
			else {//mandibular
				if(ToothGraphic.IsAnterior(toothGraphic.ToothID)) {
					Gl.glRotatef(110f,1f,0,0);
				}
				else {
					Gl.glRotatef(120f,1f,0,0);
				}
			}
			RotateAndTranslateUser(toothGraphic);
			if(!Tooth.IsPrimary(toothGraphic.ToothID)//if perm tooth
				&& Tooth.IsValidDB(Tooth.PermToPri(toothGraphic.ToothID))
				&& TcData.ListToothGraphics[Tooth.PermToPri(toothGraphic.ToothID)].Visible)//and the primary tooth is visible
			{
				//do not paint
			}
			else if(toothGraphic.Visible//might not be visible if an implant
				|| (toothGraphic.IsCrown && toothGraphic.IsImplant))//a crown on an implant will paint
				//pontics won't paint, because tooth is invisible
			{
				DrawTooth(toothGraphic);
			}
			Gl.glPopMatrix();//reset to origin
			if(toothGraphic.Visible && toothGraphic.IsSealant){//draw sealant
				Gl.glPushMatrix();
				Gl.glTranslatef(0,0,6f);//move forward 6mm so it will be visible.
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),TcData.GetTransYocclusal(toothGraphic.ToothID),0);
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
					Gl.glRotatef(-110f,1f,0,0);//rotate angle about line from origin to x,y,z
				}
				else {//mandibular
					if(ToothGraphic.IsAnterior(toothGraphic.ToothID)) {
						Gl.glRotatef(110f,1f,0,0);
					}
					else {
						Gl.glRotatef(120f,1f,0,0);
					}
				}
				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glColor3f(
					(float)toothGraphic.colorSealant.R/255f,
					(float)toothGraphic.colorSealant.G/255f,
					(float)toothGraphic.colorSealant.B/255f);
				//.5f);//only 1/2 darkness
				Gl.glBlendFunc(Gl.GL_SRC_ALPHA,Gl.GL_ONE_MINUS_SRC_ALPHA);
				Gl.glLineWidth(2f*TcData.PixelScaleRatio);
				Gl.glPointSize(1.7f*TcData.PixelScaleRatio);//point is slightly smaller since no antialiasing
				RotateAndTranslateUser(toothGraphic);
				LineSimple line=toothGraphic.GetSealantLine();
				Gl.glBegin(Gl.GL_LINE_STRIP);
				for(int j=0;j<line.Vertices.Count;j++) {//loop through each vertex
					Gl.glVertex3f(line.Vertices[j].X,line.Vertices[j].Y,line.Vertices[j].Z);
				}
				Gl.glEnd();
				//The next 30 or so lines are all a stupid OpenGL workaround to hide the line intersections with big dots.
				Gl.glPopMatrix();
				//now, draw a point at each intersection to hide the unsightly transitions
				Gl.glPushMatrix();
				//move foward so it will cover the lines
				Gl.glTranslatef(0,0,6.5f);
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID),TcData.GetTransYocclusal(toothGraphic.ToothID),0);
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
					Gl.glRotatef(-110f,1f,0,0);//rotate angle about line from origin to x,y,z
				}
				else {//mandibular
					if(ToothGraphic.IsAnterior(toothGraphic.ToothID)) {
						Gl.glRotatef(110f,1f,0,0);
					}
					else {
						Gl.glRotatef(120f,1f,0,0);
					}
				}
				RotateAndTranslateUser(toothGraphic);
				Gl.glDisable(Gl.GL_BLEND);
				Gl.glBegin(Gl.GL_POINTS);
				for(int j=0;j<line.Vertices.Count;j++) {//loop through each vertex
					//but ignore the first and last.  We are only concerned with where lines meet.
					if(j==0 || j==line.Vertices.Count-1) {
						continue;
					}
					Gl.glVertex3f(line.Vertices[j].X,line.Vertices[j].Y,line.Vertices[j].Z);
				}
				Gl.glEnd();
				Gl.glPopMatrix();
			}
		}

		private void DrawWatches(){
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_BLEND);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Hashtable watchTeeth=new Hashtable(TcData.ListToothGraphics.Count);
			for(int t=0;t<TcData.ListToothGraphics.Count;t++) {//loop through each adult tooth
			  ToothGraphic toothGraphic=TcData.ListToothGraphics[t];
				//If a tooth is marked to be watched then it is always visible, even if the tooth is missing/hidden.
			  if(toothGraphic.ToothID=="implant" || !toothGraphic.Watch || Tooth.IsPrimary(toothGraphic.ToothID)) {
			    continue;
			  }
			  watchTeeth[toothGraphic.ToothID]=toothGraphic;
			}
			for(int t=0;t<TcData.ListToothGraphics.Count;t++) {//loop through each primary tooth
			  ToothGraphic toothGraphic=TcData.ListToothGraphics[t];
				//If a tooth is marked to be watched then it is always visible, even if the tooth is missing/hidden.
			  if(toothGraphic.ToothID=="implant"|| !toothGraphic.Watch || !Tooth.IsPrimary(toothGraphic.ToothID) || !toothGraphic.Visible) {
			    continue;
			  }
			  watchTeeth[Tooth.PriToPerm(toothGraphic.ToothID)]=toothGraphic;
			}
			foreach(DictionaryEntry toothGraphic in watchTeeth){
			  RenderToothWatch((ToothGraphic)toothGraphic.Value);
			}
		}

		private void RenderToothWatch(ToothGraphic toothGraphic){
			Gl.glPushMatrix();
			if(ToothGraphic.IsRight(toothGraphic.ToothID)) {
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID)+toothGraphic.ShiftM,0,0);
			} 
			else {
				Gl.glTranslatef(TcData.GetTransX(toothGraphic.ToothID)-toothGraphic.ShiftM,0,0);
			}
			float toMm=1f/TcData.ScaleMmToPix;
			LineSimple line=toothGraphic.GetWatchLine();
			Gl.glLineWidth(3f*TcData.PixelScaleRatio);
			Gl.glColor3f(1.0f,1.0f,1.0f);//White
			Gl.glBegin(Gl.GL_LINE_STRIP);
			for(int j=0;j<line.Vertices.Count;j++) {//loop through each vertex and render the white W lines.
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
					Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(150f*toMm),line.Vertices[j].Z-7f);
				}
				else{
					Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(-140f*toMm),line.Vertices[j].Z-7f);
				}
			}
			Gl.glEnd();
			Gl.glLineWidth(0.6f*TcData.PixelScaleRatio);
			Gl.glColor3f(
				(float)toothGraphic.colorWatch.R/255f,
				(float)toothGraphic.colorWatch.G/255f,
				(float)toothGraphic.colorWatch.B/255f);
			Gl.glBegin(Gl.GL_LINE_STRIP);
			for(int j=0;j<line.Vertices.Count;j++) {//loop through each vertex and render the colored W lines.
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
					Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(150f*toMm),line.Vertices[j].Z-6f);
				}
				else{
					Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(-140f*toMm),line.Vertices[j].Z-6f);
				}
			}
			Gl.glEnd();
			Gl.glDisable(Gl.GL_BLEND);
			Gl.glPointSize(2f*TcData.PixelScaleRatio);
			Gl.glColor3f(1.0f,1.0f,1.0f);//White
			Gl.glBegin(Gl.GL_POINTS);
			for(int j=0;j<line.Vertices.Count;j++) {//loop through each vertex and render the colored W points.
			  if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
			    Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(150f*toMm),line.Vertices[j].Z+7f);
			  }
			  else{
			    Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(-140f*toMm),line.Vertices[j].Z+7f);
			  }
			}
			Gl.glEnd();
			Gl.glPointSize(0.4f*TcData.PixelScaleRatio);//point is slightly smaller since no antialiasing
			Gl.glColor3f(
				(float)toothGraphic.colorWatch.R/255f,
				(float)toothGraphic.colorWatch.G/255f,
				(float)toothGraphic.colorWatch.B/255f);
			Gl.glBegin(Gl.GL_POINTS);
			for(int j=0;j<line.Vertices.Count;j++) {//loop through each vertex and render the colored W points.
			  if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
			    Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(150f*toMm),line.Vertices[j].Z+6f);
			  }
			  else{
			    Gl.glVertex3f(line.Vertices[j].X+TcData.PixelScaleRatio*(-6f*toMm),line.Vertices[j].Y+TcData.PixelScaleRatio*(-140f*toMm),line.Vertices[j].Z+6f);
			  }
			}
			Gl.glEnd();
			Gl.glPopMatrix();
		}

		private void DrawNumbersAndLines() {
			Gl.glPushMatrix();
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_BLEND);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glColor3f(
				(float)TcData.ColorText.R/255f,
				(float)TcData.ColorText.G/255f,
				(float)TcData.ColorText.B/255f);
			//this still doesn't seem to work quite right, but it's tolerable
			Gl.glLineWidth(1f*TcData.PixelScaleRatio);
			Gl.glBegin(Gl.GL_LINE_STRIP);
				//Gl.glVertex3f(-(float)WidthProjection/2f,0,0);
				//Gl.glVertex3f((float)WidthProjection/2f,0,0);
				Gl.glVertex3f(-TcData.SizeOriginalProjection.Width/2f,0,0);
				Gl.glVertex3f(TcData.SizeOriginalProjection.Width/2f,0,0);
			Gl.glEnd();
			string tooth_id;
			for(int i=1;i<=52;i++){
				tooth_id=Tooth.FromOrdinal(i);
				if(TcData.SelectedTeeth.Contains(tooth_id)) {
					DrawNumber(tooth_id,true,true);
				}
				else {
					DrawNumber(tooth_id,false,true);
				}
			}
			Gl.glPopMatrix();
		}

		private void DrawDrawingSegments(){
			/*string[] pointStr;
			List<Point> points;
			Point point;
			string[] xy;
			Pen pen;
			for(int s=0;s<TcData.DrawingSegmentList.Count;s++) {
				pen=new Pen(TcData.DrawingSegmentList[s].ColorDraw,2f);
				pointStr=TcData.DrawingSegmentList[s].DrawingSegment.Split(';');
				points=new List<Point>();
				for(int p=0;p<pointStr.Length;p++) {
					xy=pointStr[p].Split(',');
					if(xy.Length==2) {
						point=new Point(int.Parse(xy[0]),int.Parse(xy[1]));
						points.Add(point);
					}
				}
				for(int i=1;i<points.Count;i++) {
					//if we set 0,0 to center, then this is where we would convert it back.
					gg.DrawLine(pen,points[i-1].X,
						points[i-1].Y,
						points[i].X,
						points[i].Y);
				}
			}*/
			Gl.glPushMatrix();
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA,Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			//
			//
			Gl.glLineWidth(2.2f*TcData.PixelScaleRatio);
			Gl.glPointSize(1.8f*TcData.PixelScaleRatio);//slightly smaller
			string[] pointStr;
			List<PointF> points;
			PointF point;
			string[] xy;
			PointF pointMm;
			Color color;
			float scaleDrawing=(float)Width/(float)ToothChartData.SizeOriginalDrawing.Width;
			for(int s=0;s<TcData.DrawingSegmentList.Count;s++) {
				color=TcData.DrawingSegmentList[s].ColorDraw;
				Gl.glColor3f(
					(float)color.R/255f,
					(float)color.G/255f,
					(float)color.B/255f);
				pointStr=TcData.DrawingSegmentList[s].DrawingSegment.Split(';');
				points=new List<PointF>();
				for(int p=0;p<pointStr.Length;p++){
					xy=pointStr[p].Split(',');
					if(IsValidCoordinate(xy,out float x,out float y)) {
						point=new PointF(x,y);
						pointMm=TcData.PointDrawingPixToMm(point);
						points.Add(pointMm);
					}
				}
				Gl.glBegin(Gl.GL_LINE_STRIP);
				for(int i=0;i<points.Count;i++){
					//if we set 0,0 to center, then this is where we would convert it back.
					//pointMm=TcData.PixToMm(new Point(points[i].X,points[i].Y));
					//Gl.glVertex3f(pointMm.X,pointMm.Y,0);
					Gl.glVertex3f(points[i].X,points[i].Y,0);
				}
				Gl.glEnd();
				//now draw a filled circle at each line strip intersection to make it look nicer when viewing fullscreen
				Gl.glBegin(Gl.GL_POINTS);
				for(int i=0;i<points.Count;i++){
					//but ignore the first and last.  We are only concerned with where lines meet.
					if(i==0 || i==points.Count-1){
						continue;
					}
					Gl.glVertex3f(points[i].X,points[i].Y,0);
				}
				Gl.glEnd();
			}
			Gl.glPopMatrix();
		}

		///<summary>Draws the number and the small rectangle behind it.  Draws in the appropriate color.  isFullRedraw means that all of the toothnumbers are being redrawn.  This helps with a few optimizations and with not painting blank rectangles when not needed.</summary>
		private void DrawNumber(string tooth_id, bool isSelected, bool isFullRedraw) {
			/*if(!Tooth.IsValidDB(tooth_id)) {
				return;
			}
			if(isFullRedraw) {//if redrawing all numbers
				if(TcData.ListToothGraphics[tooth_id].HideNumber) {//and this is a "hidden" number
					return;//skip
				}
				if(Tooth.IsPrimary(tooth_id)
					&& !TcData.ListToothGraphics[Tooth.PriToPerm(tooth_id)].ShowPrimaryLetter)//but not set to show primary letters
				{
					return;
				}
			}
			//fix this.  No calls to OpenDentBusiness that require database.
			//string displayNum=OpenDentBusiness.Tooth.GetToothLabelGraphic(tooth_id);
			string displayNum=tooth_id;
			float toMm=1f/TcData.ScaleMmToPix;
			Rectangle rec=TcData.GetNumberRecPix(tooth_id,g);
			//Rectangle recPix=TcData.ConvertRecToPix(recMm);
			if(isSelected) {
				gg.FillRectangle(new SolidBrush(TcData.ColorBackHighlight),rec);
			}
			else {
				gg.FillRectangle(new SolidBrush(TcData.ColorBackground),rec);
			}
			if(TcData.ListToothGraphics[tooth_id].HideNumber) {//If number is hidden.
				//do not print string
			}
			else if(Tooth.IsPrimary(tooth_id)
				&& !TcData.ListToothGraphics[Tooth.PriToPerm(tooth_id)].ShowPrimaryLetter) {
				//do not print string
			}
			else if(isSelected) {
				gg.DrawString(displayNum,Font,new SolidBrush(TcData.ColorTextHighlight),rec.X,rec.Y);
			}
			else {
				gg.DrawString(displayNum,Font,new SolidBrush(TcData.ColorText),rec.X,rec.Y);
			}*/
			if(!Tooth.IsValidDB(tooth_id)) {
				return;
			}
			Gl.glPushMatrix();
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_BLEND);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			//string tooth_id=intTooth.ToString();
			//if(ToothGraphic.IsValidToothID(ToothGraphic.PermToPri(intTooth.ToString()))//pri is valid
			//	&& TcData.ListToothGraphics[ToothGraphic.PermToPri(intTooth.ToString())].Visible)//and pri visible
			//{
			//	tooth_id=ToothGraphic.PermToPri(intTooth.ToString());
			//}
			if(isFullRedraw) {//if redrawing all numbers
				if(TcData.ListToothGraphics[tooth_id].HideNumber) {//and this is a "hidden" number
					Gl.glPopMatrix();
					return;//skip
				}
				if(Tooth.IsPrimary(tooth_id)
					&& !TcData.ListToothGraphics[Tooth.PriToPerm(tooth_id)].ShowPrimaryLetter)//but not set to show primary letters
				{
					Gl.glPopMatrix();
					return;
				}
			}
			string displayNum=OpenDentBusiness.Tooth.GetToothLabelGraphic(tooth_id,TcData.ToothNumberingNomenclature);
			//string displayNum=tooth_id;
			float toMm=1f/TcData.ScaleMmToPix;
			//float toMm=(float)WidthProjection/(float)Width;//mm/pix, a ratio that is used for conversions below. Fix this.
			//float strWidthMm=MeasureStringMm(displayNum);
			SizeF labelSizeF=MeasureStringMm(displayNum);// g.MeasureString(displayNum,Font).Width/TcData.ScaleMmToPix;
			//SizeF labelSizeF
			RectangleF recMm=TcData.GetNumberRecMm(tooth_id,labelSizeF);
			//Rectangle recPix=TcData.ConvertRecToPix(recMm);
			if(isSelected){
				Gl.glColor3f(
					(float)TcData.ColorBackHighlight.R/255f,
					(float)TcData.ColorBackHighlight.G/255f,
					(float)TcData.ColorBackHighlight.B/255f);
				Gl.glBegin(Gl.GL_QUADS);
					Gl.glVertex3f(recMm.X,recMm.Y,14);//LL
					Gl.glVertex3f(recMm.X,recMm.Y+recMm.Height,14);//UL
					Gl.glVertex3f(recMm.X+recMm.Width,recMm.Y+recMm.Height,14);//UR
					Gl.glVertex3f(recMm.X+recMm.Width,recMm.Y,14);//LR
				Gl.glEnd();
				Gl.glColor3f(
					(float)TcData.ColorTextHighlight.R/255f,
					(float)TcData.ColorTextHighlight.G/255f,
					(float)TcData.ColorTextHighlight.B/255f);
				//Gl.glRasterPos3f(recMm.X+2f*toMm,recMm.Y+2f*toMm,15f);
			} 
			else{
				Gl.glColor3f(
					(float)TcData.ColorBackground.R/255f,
					(float)TcData.ColorBackground.G/255f,
					(float)TcData.ColorBackground.B/255f);
				Gl.glBegin(Gl.GL_QUADS);
					Gl.glVertex3f(recMm.X,recMm.Y,14);//LL
					Gl.glVertex3f(recMm.X,recMm.Y+recMm.Height,14);//UL
					Gl.glVertex3f(recMm.X+recMm.Width,recMm.Y+recMm.Height,14);//UR
					Gl.glVertex3f(recMm.X+recMm.Width,recMm.Y,14);//LR
				Gl.glEnd();
				Gl.glColor3f(
					(float)TcData.ColorText.R/255f,
					(float)TcData.ColorText.G/255f,
					(float)TcData.ColorText.B/255f);
				//Gl.glRasterPos3f(recMm.X+2f*toMm,recMm.Y+2f*toMm,15f);
			}
			if(TcData.ListToothGraphics[tooth_id].HideNumber){//If number is hidden.
				//do not print string
			}
			else if(Tooth.IsPrimary(tooth_id)
				&& !TcData.ListToothGraphics[Tooth.PriToPerm(tooth_id)].ShowPrimaryLetter) 
			{
				//do not print string
			}
			else{
				Gl.glRasterPos3f(recMm.X+2f*toMm,recMm.Y+2f*toMm,15f);
				PrintString(displayNum);
			}
			Gl.glPopMatrix();
//todo: get rid of this?
			Gl.glFlush();
		}

		/*
		///<summary>Return value is in tooth coordinates, not pixels.  I left this in OpenGL rather than moving it to ToothChartData because the measurement strategy is very specific to the raster font defined here.</summary>
		private float MeasureStringPix(string text){
			
			return retVal;
		}*/
		
		/// <summary>  I left this in OpenGL rather than moving it to ToothChartData because the measurement strategy is very specific to the raster font defined here.</summary>
		private SizeF MeasureStringMm(string text){
			float pixelW=0;
			for(int i=0;i<text.Length;i++){
				if(fontsymbols[(byte)text[i]] != null) {
					pixelW+=fontsymbols[(byte)text[i]][0].Length+1;
				}
			}
			pixelW+=2;
			return new SizeF(pixelW/TcData.ScaleMmToPix,12f/TcData.ScaleMmToPix);
		}

		///<summary>Creates the font for specific characters.  Each character will be its own display list and can be located using the character code.</summary>
		private void MakeRasterFont() {
			fontsymbols=new string[255][];//Must be 255 so we can support any ASCII characters.
			fontsymbols['+']=new string[] { "00000", "00000", "00100", "00100", "11111", "00100", "00100", "00000", "00000" };
			fontsymbols['-']=new string[] { "00000", "00000", "00000", "00000", "11111", "00000", "00000", "00000", "00000" };
			fontsymbols['A']=new string[] { "0001000", "0001000", "0010100", "0010100", "0100010", "0100010", "0111110", "1000001", "1000001" };//A
			fontsymbols['B']=new string[] { "11110","10001","10001","10001","11110","10001","10001","10001","11110" };//B
			fontsymbols['C']=new string[] { "011110","100001","100000","100000","100000","100000","100000","100001","011110" };//C
			fontsymbols['D']=new string[] { "111100","100010","100001","100001","100001","100001","100001","100010","111100" };//D
			fontsymbols['E']=new string[] { "11111","10000","10000","10000","11110","10000","10000","10000","11111" };//E
			fontsymbols['F']=new string[] { "11111","10000","10000","10000","11110","10000","10000","10000","10000" };//F
			fontsymbols['G']=new string[] { "011110","100001","100000","100000","100111","100001","100001","100011","011101" };//G
			fontsymbols['H']=new string[] { "100001","100001","100001","100001","111111","100001","100001","100001","100001" };//H
			fontsymbols['I']=new string[] { "111","010","010","010","010","010","010","010","111" };//I
			fontsymbols['J']=new string[] { "00001","00001","00001","00001","00001","00001","10001","10001","01110" };//J
			fontsymbols['K']=new string[] { "100001","100010","100100","101000","110000","101000","100100","100010","100001" };//K
			fontsymbols['L']=new string[] { "10000","10000","10000","10000","10000","10000","10000","10000","11111" };//L
			fontsymbols['M']=new string[] { "1000001","1000001","1100011","1100011","1010101","1010101","1001001","1001001","1000001" };//M
			fontsymbols['N']=new string[] { "100001","110001","110001","101001","101001","100101","100011","100011","100001" };//N
			fontsymbols['O']=new string[] { "011110","100001","100001","100001","100001","100001","100001","100001","011110" };//O
			fontsymbols['P']=new string[] { "111110","100001","100001","100001","111110","100000","100000","100000","100000" };//P
			fontsymbols['Q']=new string[] { "011110","100001","100001","100001","100001","100001","100101","100010","011101" };//Q
			fontsymbols['R']=new string[] { "111110","100001","100001","100001","111110","100100","100010","100010","100001" };//R
			fontsymbols['S']=new string[] { "011110","100001","100000","100000","011110","000001","000001","100001","011110" };//S
			fontsymbols['T']=new string[] { "1111111","0001000","0001000","0001000","0001000","0001000","0001000","0001000","0001000" };//T
			fontsymbols['0']=new string[] { "01110", "10001", "10001", "10001", "10001", "10001", "10001", "10001", "01110" };//0
			fontsymbols['1']=new string[] { "0010", "1110", "0010", "0010", "0010", "0010", "0010", "0010", "0010" };//1
			fontsymbols['2']=new string[] { "01110", "10001", "00001", "00001", "00010", "00100", "01000", "10000", "11111" };//2
			fontsymbols['3']=new string[] { "01110", "10001", "00001", "00001", "00110", "00001", "00001", "10001", "01110" };//3
			fontsymbols['4']=new string[] { "00010", "00110", "00110", "01010", "01010", "10010", "11111", "00010", "00010" };//4
			fontsymbols['5']=new string[] { "11111", "10000", "10000", "11110", "10001", "00001", "00001", "10001", "01110" };//5
			fontsymbols['6']=new string[] { "01110", "10001", "10000", "10000", "11110", "10001", "10001", "10001", "01110" };//6
			fontsymbols['7']=new string[] { "11111", "00001", "00010", "00010", "00100", "00100", "01000", "01000", "01000" };//7
			fontsymbols['8']=new string[] { "01110", "10001", "10001", "10001", "01110", "10001", "10001", "10001", "01110" };//8
			fontsymbols['9']=new string[] { "01110", "10001", "10001", "10001", "01111", "00001", "00001", "10001", "01110" };//9
			Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT,1);
			fontOffset=Gl.glGenLists(128);//Create 128 display lists.  Returns the base display list number.  Display lists will be consecutive.
			for(int i=0;i<fontsymbols.Length;i++) {
				string[] arrayBitsForSymbol=fontsymbols[i];
				if(arrayBitsForSymbol==null) {
					continue;//We have not defined a byte sequence for the current ASCII character.  Do not generate a GL list.
				}
				int letterW=arrayBitsForSymbol[0].Length;//All rows of pixels for this symbol will be the same number of pixels wide.
				int letterH=arrayBitsForSymbol.Length;//The number of pixels the symbol is tall is equal to the number of pixel rows.
				byte[] arrayBytesForLetter=new byte[letterH];
				for(int h=0;h<letterH;h++) {//Draws the letter from the bottom up.
					arrayBytesForLetter[h]=0;
					string pixelRow=arrayBitsForSymbol[letterH-h-1];//from bottom and up
					for(int w=0;w<letterW;w++){
						if(pixelRow.Substring(w,1)=="1"){//If row pixel w is "ON", then enable the bit.
							arrayBytesForLetter[h]|=(byte)Math.Pow(2,7-w);//We use 2^(7-w) (from right to left) to set one bit of the byte (int).
						}
					}
				}
				Gl.glNewList(fontOffset+i,Gl.GL_COMPILE);
				Gl.glBitmap(letterW,letterH,0,0,letterW+1,0,arrayBytesForLetter);
				Gl.glEndList();
			}
		}

		private void PrintString(string text) {
			Gl.glListBase(fontOffset);
			byte[] textbytes = new byte[text.Length];
			for(int i = 0;i < text.Length;i++){
				textbytes[i] = (byte)text[i];
			}
			try{
				Gl.glCallLists(text.Length,Gl.GL_UNSIGNED_BYTE,textbytes);
			}
			catch{
				//Do nothing
			}
		}

		///<summary>Performs the rotations and translations entered by user for this tooth.  Usually, all numbers are just 0, resulting in no movement here.</summary>
		private void RotateAndTranslateUser(ToothGraphic toothGraphic) {
			//remembering that they actually show in the opposite order, so:
			//1: translate
			//2: tipM last
			//3: tipB second
			//4: rotate first
			if(ToothGraphic.IsRight(toothGraphic.ToothID)) {
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {//UR
					Gl.glTranslatef(toothGraphic.ShiftM,-toothGraphic.ShiftO,toothGraphic.ShiftB);
					Gl.glRotatef(toothGraphic.TipM,0,0,1f);
					Gl.glRotatef(-toothGraphic.TipB,1f,0,0);
					Gl.glRotatef(toothGraphic.Rotate,0,1f,0);
				}
				else {//LR
					Gl.glTranslatef(toothGraphic.ShiftM,toothGraphic.ShiftO,toothGraphic.ShiftB);
					Gl.glRotatef(-toothGraphic.TipM,0,0,1f);
					Gl.glRotatef(toothGraphic.TipB,1f,0,0);
					Gl.glRotatef(-toothGraphic.Rotate,0,1f,0);
				}
			}
			else {
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {//UL
					Gl.glTranslatef(-toothGraphic.ShiftM,-toothGraphic.ShiftO,toothGraphic.ShiftB);
					Gl.glRotatef(-toothGraphic.TipM,0,0,1f);
					Gl.glRotatef(-toothGraphic.TipB,1f,0,0);
					Gl.glRotatef(toothGraphic.Rotate,0,1f,0);
				}
				else {//LL
					Gl.glTranslatef(-toothGraphic.ShiftM,toothGraphic.ShiftO,toothGraphic.ShiftB);
					Gl.glRotatef(toothGraphic.TipM,0,0,1f);
					Gl.glRotatef(toothGraphic.TipB,1f,0,0);
					Gl.glRotatef(-toothGraphic.Rotate,0,1f,0);
				}
			}
		}

		///<summary></summary>
		private void DrawTooth(ToothGraphic toothGraphic) {
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			ToothGroup group;
			float[] material_color;
			for(int g=0;g<toothGraphic.Groups.Count;g++) {
				group=(ToothGroup)toothGraphic.Groups[g];
				if(group.GroupType==ToothGroupType.Buildup) {
					continue;
				}
				if(group.GroupType==ToothGroupType.None) {
					continue;
				}
				if(!group.Visible) {
					continue;
				}
				//group.PaintColor=Color.FromArgb(255,255,253,209);//temp only for testing
				if(toothGraphic.ShiftO<-10){//if unerupted
					material_color=new float[] {
						(float)group.PaintColor.R/255f/2f,
						(float)group.PaintColor.G/255f/2f,
						(float)group.PaintColor.B/255f/2f,
						(float)group.PaintColor.A/255f/2f
					};
				}
				else{
					material_color=new float[] {
						(float)group.PaintColor.R/255f,
						(float)group.PaintColor.G/255f,
						(float)group.PaintColor.B/255f,
						(float)group.PaintColor.A/255f
					};
				}
				if(group.GroupType==ToothGroupType.Cementum) {
					Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_SPECULAR,specular_color_cementum);
				}
				else {
					Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_SPECULAR,specular_color_normal);
				}
				Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_SHININESS,shininess);
				Gl.glMaterialfv(Gl.GL_FRONT,Gl.GL_AMBIENT_AND_DIFFUSE,material_color);
				Gl.glBlendFunc(Gl.GL_ONE,Gl.GL_ZERO);
				Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT,Gl.GL_NICEST);
				Gl.glListBase(displayListOffset);
				//draw the group
				Gl.glCallList(displayListOffset+toothGraphic.GetIndexForDisplayList(group));
			}
		}

		///<summary>Only called once as part of initialization.</summary>
		public void MakeDisplayLists(){
			//total number of display lists will be: (52 teeth) x (15 group types)=780. But 1-14 not used, and 780-794 are used.
				//520. But 1-9 not used, and 521-529 are used. 
			//displayListOffset=Gl.glGenLists(530);//not sure if I did this right
			displayListOffset=Gl.glGenLists(795);
			ToothGraphic toothGraphic;
			ToothGroup group;
			for(int t=1;t<=52;t++) {
				if(t>32 && t<=42){//33-42:  A-J = 4-13
					toothGraphic=TcData.ListToothGraphics[Tooth.PermToPri(t-29)];
				}
				else if(t>42 && t<=52) {//43-52:  K-T = 20-29
					toothGraphic=TcData.ListToothGraphics[Tooth.PermToPri(t-23)];
				}
				else{//perm
					toothGraphic=TcData.ListToothGraphics[t.ToString()];
				}
				for(int g=0;g<15;g++){//groups 0-14
					group=toothGraphic.GetGroup((ToothGroupType)g);
					Gl.glNewList(displayListOffset+(t*15)+g,Gl.GL_COMPILE);
						//ToothGraphic.GetDisplayListNum(i.ToString())
					if(group!=null){
						for(int f=0;f<group.Faces.Count;f++){//.GetLength(0);f++) {//loop through each face
							Gl.glBegin(Gl.GL_POLYGON);
							for(int j=0;j<group.Faces[f].IndexList.Count;j++) {//.Length;j++) {//loop through each vertex
								//The index for both will always be the same because we enforce a 1:1 relationship.
								//We show grabbing a float[3], but we could just as easily use the index itself.
								Gl.glNormal3fv(toothGraphic.VertexNormals[group.Faces[f].IndexList[j]].Normal.GetFloatArray()); 
								Gl.glVertex3fv(toothGraphic.VertexNormals[group.Faces[f].IndexList[j]].Vertex.GetFloatArray());
							}
							//for(int v=0;v<group.Faces[f].VertexNormals.Count;v++){//  .Length;v++) {//loop through each vertex
							//	Gl.glNormal3fv(toothGraphic.Normals[group.Faces[f][v][1]]);
							//	Gl.glVertex3fv(toothGraphic.Vertices[group.Faces[f][v][0]]);
							//}
							Gl.glEnd();
						}
					}
					Gl.glEndList();
				}
			}
		}

		#region Mouse And Selections

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			MouseIsDown=true;
			if(TcData.CursorTool==CursorTool.Pointer) {
				_listSelectedTeethOld=TcData.SelectedTeeth.FindAll(x => x!=null);//Make a copy of the list.  No elements should ever be null (copy all).
				string toothClicked=TcData.GetToothAtPoint(e.Location);
				//MessageBox.Show(toothClicked.ToString());
				if(TcData.SelectedTeeth.Contains(toothClicked)) {
					SetSelected(toothClicked,false);
				}
				else {
					SetSelected(toothClicked,true);
				}
			}
			else if(TcData.CursorTool==CursorTool.Pen) {
				TcData.PointList.Add(new PointF(e.X,e.Y));
			}
			else if(TcData.CursorTool==CursorTool.Eraser) {
				//do nothing
			}
			else if(TcData.CursorTool==CursorTool.ColorChanger) {
				//look for any lines near the "wand".
				//since the line segments are so short, it's sufficient to check end points.
				string[] xy;
				string[] pointStr;
				float x;
				float y;
				float dist;//the distance between the point being tested and the center of the eraser circle.
				float radius=2f;//by trial and error to achieve best feel.
				PointF pointMouseScaled=TcData.GetPointMouseScaled(e.X,e.Y,Size);
				for(int i=0;i<TcData.DrawingSegmentList.Count;i++){
					pointStr=TcData.DrawingSegmentList[i].DrawingSegment.Split(';');
					for(int p=0;p<pointStr.Length;p++){
						xy=pointStr[p].Split(',');
						if(IsValidCoordinate(xy,out x,out y)) {
							dist=(float)Math.Sqrt(Math.Pow(Math.Abs(x-pointMouseScaled.X),2)+Math.Pow(Math.Abs(y-pointMouseScaled.Y),2));
							if(dist<=radius){//testing circle intersection here
								OnSegmentDrawn(TcData.DrawingSegmentList[i].DrawingSegment);
								TcData.DrawingSegmentList[i].ColorDraw=TcData.ColorDrawing;
								Invalidate();
								return;;
							}
						}
					}
				}
			}
		}

		private bool IsValidCoordinate(string[] coordinate,out float x,out float y) {
			x=0;
			y=0;
			return coordinate.Length==2 && float.TryParse(coordinate[0],out x) && float.TryParse(coordinate[1],out y);
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(TcData.CursorTool==CursorTool.Pointer) {
				hotTooth=TcData.GetToothAtPoint(e.Location);
				if(hotTooth==hotToothOld) {//mouse has not moved to another tooth
					return;
				}
				if(MouseIsDown) {//drag action
					List<string> affectedTeeth=TcData.GetAffectedTeeth(hotToothOld,hotTooth,TcData.PointPixToMm(e.Location).Y);
					for(int i=0;i<affectedTeeth.Count;i++) {
						if(TcData.SelectedTeeth.Contains(affectedTeeth[i])) {
							SetSelected(affectedTeeth[i],false);
						}
						else {
							SetSelected(affectedTeeth[i],true);
						}
					}
					hotToothOld=hotTooth;
					//Invalidate();
					//Application.DoEvents();
				}
				else {
					hotToothOld=hotTooth;
				}
			}
			else if(TcData.CursorTool==CursorTool.Pen) {
				if(!MouseIsDown){
					return;
				}
				TcData.PointList.Add(new PointF(e.X,e.Y));
				//just add the last line segment instead of redrawing the whole thing.
				/*
				gg.SmoothingMode=SmoothingMode.HighQuality;
				//g.CompositingMode=CompositingMode.SourceOver;
			
				Pen pen=new Pen(TcData.ColorDrawing,2f);
				int i=TcData.PointList.Count-1;
				gg.DrawLine(pen,TcData.PointList[i-1].X,TcData.PointList[i-1].Y,TcData.PointList[i].X,TcData.PointList[i].Y);
				//g.DrawImage(bitmapInPictBox,0,0);*/
				
				Gl.glPushMatrix();
				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glBlendFunc(Gl.GL_SRC_ALPHA,Gl.GL_ONE_MINUS_SRC_ALPHA);
				Gl.glDisable(Gl.GL_DEPTH_TEST);
				Gl.glColor3f(
					(float)TcData.ColorDrawing.R/255f,
					(float)TcData.ColorDrawing.G/255f,
					(float)TcData.ColorDrawing.B/255f);
				Gl.glLineWidth((float)Width/220f);//300f);//about 2
				int i=TcData.PointList.Count-1;
				Gl.glBegin(Gl.GL_LINE_STRIP);
				//if we set 0,0 to center, then this is where we would convert it back.
				PointF pointMm=TcData.PointPixToMm(TcData.PointList[i-1]);
				Gl.glVertex3f(pointMm.X,pointMm.Y,0);
				pointMm=TcData.PointPixToMm(TcData.PointList[i]);
				Gl.glVertex3f(pointMm.X,pointMm.Y,0);
				Gl.glEnd();
				Gl.glPopMatrix();
				Gl.glFlush();
			}
			else if(TcData.CursorTool==CursorTool.Eraser) {
				if(!MouseIsDown){
					return;
				}
				//look for any lines that intersect the "eraser".
				//since the line segments are so short, it's sufficient to check end points.
				string[] xy;
				string[] pointStr;
				float x;
				float y;
				float dist;//the distance between the point being tested and the center of the eraser circle.
				float radius=8f;//by trial and error to achieve best feel.
				PointF eraserPt=TcData.GetPointMouseScaled(e.X+8.49f,e.Y+8.49f,Size);
				for(int i=0;i<TcData.DrawingSegmentList.Count;i++) {
					pointStr=TcData.DrawingSegmentList[i].DrawingSegment.Split(';');
					for(int p=0;p<pointStr.Length;p++){
						xy=pointStr[p].Split(',');
						if(IsValidCoordinate(xy,out x,out y)) {
							dist=(float)Math.Sqrt(Math.Pow(Math.Abs(x-eraserPt.X),2)+Math.Pow(Math.Abs(y-eraserPt.Y),2));
							if(dist<=radius){//testing circle intersection here
								OnSegmentDrawn(TcData.DrawingSegmentList[i].DrawingSegment);//triggers a deletion from db.
								TcData.DrawingSegmentList.RemoveAt(i);
								Invalidate();
								return;
							}
						}
					}
				}	
			}
			else if(TcData.CursorTool==CursorTool.ColorChanger) {
				//do nothing	
			}
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			MouseIsDown=false;
			if(TcData.CursorTool==CursorTool.Pointer) {
				if(TcData.HasSelectedTeethChanged(_listSelectedTeethOld)) {
					OnToothSelectionsChanged();
				}
			}
			else if(TcData.CursorTool==CursorTool.Pen) {
				string drawingSegment="";
				for(int i=0;i<TcData.PointList.Count;i++) {
					if(i>0){
						drawingSegment+=";";
					}
					PointF pointMouseScaled=TcData.GetPointMouseScaled(TcData.PointList[i].X,TcData.PointList[i].Y,Size);
					//I could compensate to center point here:
					drawingSegment+=pointMouseScaled.X+","+pointMouseScaled.Y;
				}
				OnSegmentDrawn(drawingSegment);
				TcData.PointList=new List<PointF>();
				//Invalidate();
			}
			else if(TcData.CursorTool==CursorTool.Eraser) {
				//do nothing
			}
			else if(TcData.CursorTool==CursorTool.ColorChanger) {
				//do nothing
			}
		}

		///<summary></summary>
		protected void OnSegmentDrawn(string drawingSegment){
			ToothChartDrawEventArgs tArgs=new ToothChartDrawEventArgs(drawingSegment);
			if(SegmentDrawn!=null){
				SegmentDrawn(this,tArgs);
			}
		}

		///<summary></summary>
		protected void OnToothSelectionsChanged() {
			if(ToothSelectionsChanged!=null) {
				ToothSelectionsChanged(this);
			}
		}

		///<summary>Used by mousedown and mouse move to set teeth selected or unselected.  A similar method is used externally in the wrapper to set teeth selected.  This private method might be faster since it only draws the changes.</summary>
		private void SetSelected(string tooth_id,bool setValue) {
			//suspendRendering=true;
			TcData.SetSelected(tooth_id,setValue);
			
			if(setValue) {
				DrawNumber(tooth_id,true,false);
			}
			else {
				DrawNumber(tooth_id,false,false);
			}
			//g.DrawImage(bitmapInPictBox,0,0);
			//RectangleF recMm=TcData.GetNumberRecMm(tooth_id,);
			//Rectangle rec=TcData.ConvertRecToPix(recMm);
			Invalidate();//rec);//but it invalidates the whole thing anyway.  Oh, well.
			//Application.DoEvents();
			//suspendRendering=false;
		}

		#endregion Mouse And Selections


	}
}




