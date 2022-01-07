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
		private Point MouseDownOrigin;
		private bool MouseIsDown=false;
		private Document displayedDoc=null;
		///<summary>The offset of the image due to the grab tool. Used as a basis for calculating imageTranslation.</summary>
		PointF imageLocation=new PointF(0,0);
		///<summary>The true offset of the image in screen-space.</summary>
		PointF imageTranslation=new PointF(0,0);
		///<summary>The viewable area of the picture.</summary>
		Bitmap backBuffer=null;
		Graphics backBuffGraph=null;
		Bitmap renderImage=null;
		private Bitmap ImageCurrent=null;
		///<summary>The current zoom of the image. 1 implies normal size, <1 implies the image is shrunk, >1 imples the image is blown-up.</summary>
		float imageZoom=1.0f;
		///<summary>The current amount. The ZoomLevel is 0 after an image is loaded. The image is zoomed a factor of (initial image zoom)*(2^ZoomLevel)</summary>
		int zoomLevel=0;
		///<summary>Represents the current factor for level of zoom from the initial zoom of the image. This is calculated directly as 2^ZoomLevel every time a zoom occurs. Recalculated from ZoomLevel each time, so that ZoomOverall always hits the exact same values for the exact same zoom levels (not loss of data).</summary>
		float zoomFactor=1;

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
		public void SetImage(Document thisDocument,string displayTitle) {
			//for now, the document is single. Later, it will get groups for composite images/mounts.
			Text=displayTitle;
			displayedDoc=thisDocument;
			List<long> docNums=new List<long>();
			docNums.Add(thisDocument.DocNum);
			string fileName=CloudStorage.PathTidy(Documents.GetPaths(docNums,ImageStore.GetPreferredAtoZpath())[0]);
			if(!FileAtoZ.Exists(fileName)) {
				MessageBox.Show(fileName+" +"+Lan.g(this,"could not be found."));
				return;
			}
			ImageCurrent=(Bitmap)FileAtoZ.GetImage(fileName);
			DisplayImage();
		}

		///<summary>This method will display the image passed in.</summary>
		public void SetImage(Bitmap image,string displayTitle) {
			Text=displayTitle;
			displayedDoc=new Document();
			ImageCurrent=image;
			DisplayImage();
		}

		private void DisplayImage() {
			if(ImageCurrent==null) {//Likely the user canceled out of downloading
				return;
			}
			try {				
				renderImage=ImageHelper.ApplyDocumentSettingsToImage(displayedDoc,ImageCurrent,
					ImageSettingFlags.CROP | ImageSettingFlags.COLORFUNCTION);
				if(renderImage==null) {
					imageZoom=1;
					imageTranslation=new PointF(0,0);
				}
				else {
					float matchWidth=backBuffer.Width-16;
					matchWidth=(matchWidth<=0?1:matchWidth);
					float matchHeight=backBuffer.Height-16;
					matchHeight=(matchHeight<=0?1:matchHeight);
					imageZoom=Math.Min(matchWidth/renderImage.Width,matchHeight/renderImage.Height);
					imageTranslation=new PointF(backBuffer.Width/2.0f,backBuffer.Height/2.0f);
				}
				zoomLevel=0;
				zoomFactor=1;
			}
			catch(Exception exception) {
				MessageBox.Show(Lan.g(this,exception.Message));
				ImageCurrent=null;
				renderImage=null;
			}
			UpdatePictureBox();
		}

		private void FormImageViewer_Resize(object sender,System.EventArgs e) {
			if(backBuffGraph!=null) {
				backBuffGraph.Dispose();
				backBuffGraph=null;
			}
			if(backBuffer!=null) {
				backBuffer.Dispose();
				backBuffer=null;
			}
			int width=PictureBox1.Bounds.Width;
			int height=PictureBox1.Bounds.Height;
			if(width>0 && height>0) {
				backBuffer=new Bitmap(width,height);
				backBuffGraph=Graphics.FromImage(backBuffer);
			}
			PictureBox1.Image=backBuffer;
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
			zoomLevel++;
			PointF c=new PointF(PictureBox1.ClientRectangle.Width/2.0f,PictureBox1.ClientRectangle.Height/2.0f);
			PointF p=new PointF(c.X-imageTranslation.X,c.Y-imageTranslation.Y);
			imageTranslation=new PointF(imageTranslation.X-p.X,imageTranslation.Y-p.Y);
			zoomFactor=(float)Math.Pow(2,zoomLevel);
			UpdatePictureBox();
		}

		private void OnZoomOut_Click() {
			zoomLevel--;
			PointF c=new PointF(PictureBox1.ClientRectangle.Width/2.0f,PictureBox1.ClientRectangle.Height/2.0f);
			PointF p=new PointF(c.X-imageTranslation.X,c.Y-imageTranslation.Y);
			imageTranslation=new PointF(imageTranslation.X+p.X/2.0f,imageTranslation.Y+p.Y/2.0f);
			zoomFactor=(float)Math.Pow(2,zoomLevel);
			UpdatePictureBox();
		}

		private void OnWhite_Click(){
			ImageCurrent=new Bitmap(1,1);
			renderImage=new Bitmap(1,1);
			UpdatePictureBox();
		}

		private void PictureBox1_MouseDown(object sender,MouseEventArgs e) {
			MouseDownOrigin=new Point(e.X,e.Y);
			MouseIsDown=true;
			imageLocation=new PointF(imageTranslation.X,imageTranslation.Y);
			PictureBox1.Cursor=Cursors.Hand;
		}

		private void PictureBox1_MouseMove(object sender,MouseEventArgs e) {
			if(MouseIsDown) {
				if(ImageCurrent!=null) {
					imageTranslation=new PointF(imageLocation.X+(e.Location.X-MouseDownOrigin.X),
						imageLocation.Y+(e.Location.Y-MouseDownOrigin.Y));
					UpdatePictureBox();
				}
			}
		}

		private void PictureBox1_MouseUp(object sender,MouseEventArgs e) {
			MouseIsDown=false;
			PictureBox1.Cursor=Cursors.Default;
		}

		private void UpdatePictureBox() {
			try {
				backBuffGraph.Clear(Pens.White.Color);
				backBuffGraph.Transform=ControlImages.GetScreenMatrix(displayedDoc,ImageCurrent.Width,ImageCurrent.Height,imageZoom*zoomFactor,imageTranslation);
				backBuffGraph.DrawImage(renderImage,0,0);
				PictureBox1.Refresh();
			}
			catch {
				//Not being able to render the image is non-fatal and probably due to a simple change in state or rounding errors.
			}
		}
	}
}