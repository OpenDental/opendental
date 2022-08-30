using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	///<summary>Used to view an image when user double clicks on a thumbnail at the bottom of the Chart module.</summary>
	public partial class FormImageViewer : FormODBase {
		private Point _pointMouseDownOrigin;
		private bool _isMouseDown=false;
		private Document _documentdisplayed=null;
		///<summary>The offset of the image due to the grab tool. Used as a basis for calculating imageTranslation.</summary>
		private PointF _pointFImageLocation=new PointF(0,0);
		///<summary>The true offset of the image in screen-space.</summary>
		private PointF _pointFImageTranslation=new PointF(0,0);
		///<summary>The viewable area of the picture.</summary>
		public Bitmap BitmapBackBuffer=null;
		public Graphics GraphicsBackBuffer=null;
		public Bitmap BitmapRender=null;
		private Bitmap _bitmap=null;
		///<summary>The current zoom of the image. 1 implies normal size, <1 implies the image is shrunk, >1 imples the image is blown-up.</summary>
		private float _imageZoom=1.0f;
		///<summary>The current amount. The ZoomLevel is 0 after an image is loaded. The image is zoomed a factor of (initial image zoom)*(2^ZoomLevel)</summary>
		private int _zoomLevel=0;
		///<summary>Represents the current factor for level of zoom from the initial zoom of the image. This is calculated directly as 2^ZoomLevel every time a zoom occurs. Recalculated from ZoomLevel each time, so that ZoomOverall always hits the exact same values for the exact same zoom levels (not loss of data).</summary>
		private float _zoomFactor=1;

		///<summary></summary>
		public FormImageViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			//even the title of this window is set externally, so no Lan.F is necessary
		}

		private void FormImageViewer_Load(object sender, System.EventArgs e) {
			LayoutToolBar();
		}

		///<summary>This form will get the necessary images off disk or from the cloud so that it can control layout.</summary>
		public void SetImage(Document document,string title) {
			//for now, the document is single. Later, it will get groups for composite images/mounts.
			Text=title;
			_documentdisplayed=document;
			List<long> listDocNums=new List<long>();
			listDocNums.Add(document.DocNum);
			string fileName=CloudStorage.PathTidy(Documents.GetPaths(listDocNums,ImageStore.GetPreferredAtoZpath())[0]);
			if(!FileAtoZ.Exists(fileName)) {
				MessageBox.Show(fileName+" +"+Lan.g(this,"could not be found."));
				return;
			}
			_bitmap=(Bitmap)FileAtoZ.GetImage(fileName);
			DisplayImage();
		}

		///<summary>This method will display the image passed in.</summary>
		public void SetImage(Bitmap bitmap,string title) {
			Text=title;
			_documentdisplayed=new Document();
			_bitmap=bitmap;
			DisplayImage();
		}

		private void DisplayImage() {
			if(_bitmap==null) {//Likely the user canceled out of downloading
				return;
			}
			try {				
				BitmapRender=ImageHelper.ApplyDocumentSettingsToImage(_documentdisplayed,_bitmap,
					ImageSettingFlags.CROP | ImageSettingFlags.COLORFUNCTION);
				if(BitmapRender==null) {
					_imageZoom=1;
					_pointFImageTranslation=new PointF(0,0);
				}
				else {
					float widthMatch=BitmapBackBuffer.Width-16;
					widthMatch=(widthMatch<=0?1:widthMatch);
					float heightMatch=BitmapBackBuffer.Height-16;
					heightMatch=(heightMatch<=0?1:heightMatch);
					_imageZoom=Math.Min(widthMatch/BitmapRender.Width,heightMatch/BitmapRender.Height);
					_pointFImageTranslation=new PointF(BitmapBackBuffer.Width/2.0f,BitmapBackBuffer.Height/2.0f);
				}
				_zoomLevel=0;
				_zoomFactor=1;
			}
			catch(Exception exception) {
				MessageBox.Show(Lan.g(this,exception.Message));
				_bitmap=null;
				BitmapRender=null;
			}
			UpdatePictureBox();
		}

		private void FormImageViewer_Resize(object sender,System.EventArgs e) {
			if(GraphicsBackBuffer!=null) {
				GraphicsBackBuffer.Dispose();
				GraphicsBackBuffer=null;
			}
			if(BitmapBackBuffer!=null) {
				BitmapBackBuffer.Dispose();
				BitmapBackBuffer=null;
			}
			int width=PictureBox1.Bounds.Width;
			int height=PictureBox1.Bounds.Height;
			if(width>0 && height>0) {
				BitmapBackBuffer=new Bitmap(width,height);
				GraphicsBackBuffer=Graphics.FromImage(BitmapBackBuffer);
			}
			PictureBox1.Image=BitmapBackBuffer;
			UpdatePictureBox();
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			//ODToolBarButton button;
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton("",0,Lan.g(this,"Zoom In"),"ZoomIn"));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",1,Lan.g(this,"Zoom Out"),"ZoomOut"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"White"),-1,Lan.g(this,"Clear screen to solid white"),"White"));
			ToolBarMain.Invalidate();
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()){
				case "ZoomIn":
					OnZoomIn_Click();
					break;
				case "ZoomOut":
					OnZoomOut_Click();
					break;
				case "White":
					OnWhite_Click();
					break;
			}
		}

		private void OnZoomIn_Click() {
			_zoomLevel++;
			PointF pointFClientArea=new PointF(PictureBox1.ClientRectangle.Width/2.0f,PictureBox1.ClientRectangle.Height/2.0f);
			PointF pointFOffset=new PointF(pointFClientArea.X-_pointFImageTranslation.X,pointFClientArea.Y-_pointFImageTranslation.Y);
			_pointFImageTranslation=new PointF(_pointFImageTranslation.X-pointFOffset.X,_pointFImageTranslation.Y-pointFOffset.Y);
			_zoomFactor=(float)Math.Pow(2,_zoomLevel);
			UpdatePictureBox();
		}

		private void OnZoomOut_Click() {
			_zoomLevel--;
			PointF pointFClientArea=new PointF(PictureBox1.ClientRectangle.Width/2.0f,PictureBox1.ClientRectangle.Height/2.0f);
			PointF pointFOffset=new PointF(pointFClientArea.X-_pointFImageTranslation.X,pointFClientArea.Y-_pointFImageTranslation.Y);
			_pointFImageTranslation=new PointF(_pointFImageTranslation.X+pointFOffset.X/2.0f,_pointFImageTranslation.Y+pointFOffset.Y/2.0f);
			_zoomFactor=(float)Math.Pow(2,_zoomLevel);
			UpdatePictureBox();
		}

		private void OnWhite_Click(){
			_bitmap=new Bitmap(1,1);
			BitmapRender=new Bitmap(1,1);
			UpdatePictureBox();
		}

		private void PictureBox1_MouseDown(object sender,MouseEventArgs e) {
			_pointMouseDownOrigin=new Point(e.X,e.Y);
			_isMouseDown=true;
			_pointFImageLocation=new PointF(_pointFImageTranslation.X,_pointFImageTranslation.Y);
			PictureBox1.Cursor=Cursors.Hand;
		}

		private void PictureBox1_MouseMove(object sender,MouseEventArgs e) {
			if(_isMouseDown) {
				if(_bitmap!=null) {
					_pointFImageTranslation=new PointF(_pointFImageLocation.X+(e.Location.X-_pointMouseDownOrigin.X),
						_pointFImageLocation.Y+(e.Location.Y-_pointMouseDownOrigin.Y));
					UpdatePictureBox();
				}
			}
		}

		private void PictureBox1_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDown=false;
			PictureBox1.Cursor=Cursors.Default;
		}

		private void UpdatePictureBox() {
			try {
				GraphicsBackBuffer.Clear(Pens.White.Color);
				GraphicsBackBuffer.Transform=ControlImages.GetScreenMatrix(_documentdisplayed,_bitmap.Width,_bitmap.Height,_imageZoom*_zoomFactor,_pointFImageTranslation);
				GraphicsBackBuffer.DrawImage(BitmapRender,0,0);
				PictureBox1.Refresh();
			}
			catch {
				//Not being able to render the image is non-fatal and probably due to a simple change in state or rounding errors.
			}
		}
	}
}