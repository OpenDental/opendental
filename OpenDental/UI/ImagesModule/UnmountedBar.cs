using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.UI {
	public partial class UnmountedBar:Control {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Changes when dpi changes. Disposed</summary>
		private LinearGradientBrush _linearGradientBrush;
		///<summary>20 at 96 dpi</summary>
		private int _heightHeader96=20;
		///<summary>Copied from GridOD</summary>
		private static Color _colorTitleTop=Color.FromArgb(156,175,230);
		///<summary>Copied from GridOD</summary>
		private static Color _colorTitleBottom=Color.FromArgb(60,90,150);
		private Font _fontTitle=new Font("Segoe UI",10f);
		///<summary>This list just holds secondary references to the various objects, including bitmaps.  This class is not responsible for disposing of bitmaps.</summary>
		private List<UnmountedObject> _listUnmountedObjects;
		private Color _colorBack=Color.White;
		public int SelectedIndex=-1;
		private Button butRemount;
		private Button butDelete;
		private Button butRetake;
		private Button butClose;

		#region Constructor
		public UnmountedBar() {
			InitializeComponent();
			this.DoubleBuffered=true;
			butRemount=new Button();
			butRemount.Name="butRemount";
			butRemount.Text=Lan.g(this,"Remount");
			butRemount.Size=LayoutManager.ScaleSize(new Size(60,20));
			butRemount.Click += butRemount_Click;
			LayoutManager.Add(butRemount,this);
			butDelete=new Button();
			butDelete.Name="butDelete";
			butDelete.Text=Lan.g(this,"Delete");
			butDelete.Location=new Point(LayoutManager.Scale(61),0);
			butDelete.Size=LayoutManager.ScaleSize(new Size(60,20));
			butDelete.Click += butDelete_Click;
			LayoutManager.Add(butDelete,this);
			butRetake=new Button();
			butRetake.Name="butRetake";
			butRetake.Text=Lan.g(this,"Retake");
			butRetake.Location=new Point(LayoutManager.Scale(122),0);
			butRetake.Size=LayoutManager.ScaleSize(new Size(60,20));
			butRetake.Click += butRetake_Click;
			LayoutManager.Add(butRetake,this);
			butClose=new Button();
			butClose.Name="butClose";
			butClose.Text=Lan.g(this,"Close");
			butClose.Location=new Point(LayoutManager.Scale(183),0);
			butClose.Size=LayoutManager.ScaleSize(new Size(60,20));
			butClose.Click += butClose_Click;
			LayoutManager.Add(butClose,this);
		}
		#endregion Constructor

		#region Events - Raise
		///<summary>Occurs when the user clicks the Close button.</summary>
		[Category("OD")]
		[Description("Occurs when the user clicks the Close button.")]
		public event EventHandler EventClose;

		///<summary>Occurs when the parent needs to be refreshed.</summary>
		[Category("OD")]
		[Description("Occurs when the parent needs to be refreshed.")]
		public event EventHandler EventRefreshParent;

		///<summary>Occurs when user clicks the Remount button.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks the Remount button.")]
		public event EventHandler<UnmountedObject> EventRemount;

		///<summary>Occurs when user clicks the Retake button.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks the Retake button.")]
		public event EventHandler EventRetake;
		#endregion Events - Raise

		#region Methods - override
		protected override void OnResize(EventArgs e){
			base.OnResize(e);
			_linearGradientBrush?.Dispose();
			_linearGradientBrush=new LinearGradientBrush(new Point(0,0),new Point(0,LayoutManager.Scale(_heightHeader96)),_colorTitleTop,_colorTitleBottom);
			_fontTitle?.Dispose();
			_fontTitle=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(10f),FontStyle.Bold);
			CalculatePositions();
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs pe) {
			Graphics g=pe.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(_colorBack);
			g.FillRectangle(_linearGradientBrush,0,0,Width,LayoutManager.Scale(_heightHeader96));
			string title="Unmounted";
			g.DrawString(title,_fontTitle,Brushes.White,Width/2-g.MeasureString(title,_fontTitle).Width/2,2);
			if(_listUnmountedObjects!=null){
				for(int i=0;i<_listUnmountedObjects.Count;i++){
					DrawMountOne(g,i);
					using Pen penOutline=new Pen(ColorOD.Gray(100));
					g.DrawRectangle(penOutline,_listUnmountedObjects[i].GetBounds());
				}
				if(SelectedIndex>-1){
					using Pen penOutline=new Pen(Color.Yellow);
					g.DrawRectangle(penOutline,_listUnmountedObjects[SelectedIndex].GetBounds());
				}
			}
			using Pen pen=new Pen(ColorOD.Outline);
			Rectangle rectangle=new Rectangle(0,0,Width-1,Height-1);
			g.DrawRectangle(pen,rectangle);
			base.OnPaint(pe);
		}

		///<summary></summary>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseMove(e);
			
			//Invalidate();
		}

		///<summary>Resets button appearance.  Repaints only if necessary.</summary>
		protected override void OnMouseLeave(System.EventArgs e) {
			
		}

		///<summary>Change the button to a pressed state.</summary>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseDown(e);
			SelectedIndex=HitTest(e.X,e.Y);
			Invalidate();
		}

		///<summary>Change button to hover state and repaint if needed.</summary>
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			
			base.OnMouseUp(e);
			
			Invalidate();
		}
		#endregion Methods - override

		#region Methods - public
		public void AddObject(UnmountedObject unmountedObject){
			_listUnmountedObjects.Add(unmountedObject);
			SelectedIndex=-1;
			CalculatePositions();
			Invalidate();
		}

		public void SetColorBack(Color color){
			_colorBack=color;
			Invalidate();
		}

		public void SetObjects(List<UnmountedObject> listUnmountedObjects){
			_listUnmountedObjects=listUnmountedObjects;
			SelectedIndex=-1;
			CalculatePositions();
			Invalidate();
		}
		#endregion Methods - public

		#region Methods - private
		private void butClose_Click(object sender, EventArgs e){
			if(_listUnmountedObjects.Count>0){
				MsgBox.Show(this,"Cannot close the unmounted bar as long as it still contains images.");
				return;
			}
			EventClose?.Invoke(this,new EventArgs());
		}

		private void butDelete_Click(object sender, EventArgs e){
			if(_listUnmountedObjects.Count==0){
				MsgBox.Show(this,"No unmounted images to delete.");
				return;
			}
			if(SelectedIndex==-1){
				if(_listUnmountedObjects.Count>1){
					MsgBox.Show(this,"Please select an an image below first.");
					return;
				}
				SelectedIndex=0;
				Invalidate();
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Permanently delete the image selected below?")){
				return;
			}
			if(_listUnmountedObjects[SelectedIndex].Document!=null){
				Documents.Delete(_listUnmountedObjects[SelectedIndex].Document);
			}
			MountItems.Delete(_listUnmountedObjects[SelectedIndex].MountItem);
			//_listUnmountedObjects.RemoveAt(SelectedIndex);
			//SelectedIndex=-1;
			//Invalidate();
			EventRefreshParent?.Invoke(this,new EventArgs());
		}

		private void butRemount_Click(object sender, EventArgs e){
			if(_listUnmountedObjects.Count==0){
				MsgBox.Show(this,"No unmounted images to remount.");
				return;
			}
			if(SelectedIndex==-1){
				if(_listUnmountedObjects.Count>1){
					MsgBox.Show(this,"Please select an an image below to remount.");
					return;
				}
				SelectedIndex=0;
				Invalidate();
			}
			EventRemount?.Invoke(this,_listUnmountedObjects[SelectedIndex]);
		}

		private void butRetake_Click(object sender, EventArgs e){
			EventRetake?.Invoke(this,new EventArgs());
		}

		private void CalculatePositions(){
			if(_listUnmountedObjects is null){
				return;
			}
			int padding=3;//just 3 pixels, unscaled, around all 4 sides of each item. 3 pixels between items, not 6
			int xPos=padding;
			int yPos=LayoutManager.Scale(_heightHeader96)+padding;
			int heightTotal=Height;
			if(heightTotal==0){
				heightTotal=LayoutManager.Scale(200);
			}
			//they all have the same height
			int height=heightTotal-LayoutManager.Scale(_heightHeader96)-1-2*padding;
			for(int i=0;i<_listUnmountedObjects.Count;i++){
				_listUnmountedObjects[i].Xpos=xPos;
				_listUnmountedObjects[i].Ypos=yPos;
				if(_listUnmountedObjects[i].MountItem.Width==0 || _listUnmountedObjects[i].MountItem.Height==0){
					//database corruption
					_listUnmountedObjects[i].Width=height;
				}
				else{
					_listUnmountedObjects[i].Width=(int)((float)_listUnmountedObjects[i].MountItem.Width/(float)_listUnmountedObjects[i].MountItem.Height*(float)height);
				}
				_listUnmountedObjects[i].Height=height;
				xPos+=_listUnmountedObjects[i].Width+padding;
			}
		}

		private void DrawMountOne(Graphics g,int i){
			GraphicsState graphicsStateMount=g.Save();
			g.TranslateTransform(_listUnmountedObjects[i].Xpos,_listUnmountedObjects[i].Ypos);//UL of mount position
			g.SetClip(new Rectangle(0,0,_listUnmountedObjects[i].Width,_listUnmountedObjects[i].Height));
			g.TranslateTransform(_listUnmountedObjects[i].Width/2,_listUnmountedObjects[i].Height/2);//rotate and flip about the center of the mount box
			if(_listUnmountedObjects[i].Bitmap==null){
				g.Restore(graphicsStateMount);
				return;
			}
			if(_listUnmountedObjects[i].Document.CropW > 0 && _listUnmountedObjects[i].Document.CropH > 0){
				//In FormImageFloat, the images were already scaled in LoadBitmap().
				//Here, we are taking those scaled images and scaling them again to fit the space we have room for.
				float scale;
				if(_listUnmountedObjects[i].Document.DegreesRotated.In(0,180)){
					scale=(float)_listUnmountedObjects[i].Width/_listUnmountedObjects[i].Document.CropW;//example 100/200=.5 indicates we're making it smaller to fit
				}
				else{//90,270
					//Can't use cropH, because we always assume it's faulty.
					scale=(float)_listUnmountedObjects[i].Height/_listUnmountedObjects[i].Document.CropW;
				}
				g.ScaleTransform(scale,scale);
				//Because we scaled here, we are now in bitmap coords from here down instead of mount coords
				//We are also in the center of the cropped area because of the last two translations that happen just before drawImage
				g.RotateTransform(_listUnmountedObjects[i].Document.DegreesRotated);
				if(_listUnmountedObjects[i].Document.IsFlipped){
					Matrix matrix=new Matrix(-1,0,0,1,0,0);
					g.MultiplyTransform(matrix);
				}
				if(_listUnmountedObjects[i].Document.DegreesRotated.In(0,180)){
					g.TranslateTransform(-_listUnmountedObjects[i].Width/2/scale,-_listUnmountedObjects[i].Height/2/scale);//back to UL corner of cropped area
					g.TranslateTransform(-_listUnmountedObjects[i].Document.CropX,-_listUnmountedObjects[i].Document.CropY);//then to the 00 of the image
				}
				else{//90,270
					g.TranslateTransform(-_listUnmountedObjects[i].Height/2/scale,-_listUnmountedObjects[i].Width/2/scale);
					g.TranslateTransform(-_listUnmountedObjects[i].Document.CropX,-_listUnmountedObjects[i].Document.CropY);
				}
			}
			else{//no crop specified, so just fit it to the mount space
				float scale=ImageTools.CalcScaleFit(
						new Size(_listUnmountedObjects[i].Width,_listUnmountedObjects[i].Height),_listUnmountedObjects[i].Bitmap.Size,_listUnmountedObjects[i].Document.DegreesRotated);
				g.ScaleTransform(scale,scale);
				g.RotateTransform(_listUnmountedObjects[i].Document.DegreesRotated);
				if(_listUnmountedObjects[i].Document.IsFlipped){
					Matrix matrix=new Matrix(-1,0,0,1,0,0);
					g.MultiplyTransform(matrix);
				}
				g.TranslateTransform(-_listUnmountedObjects[i].Bitmap.Width/2,-_listUnmountedObjects[i].Bitmap.Height/2);//from center of the image to UL
			}
			g.DrawImage(_listUnmountedObjects[i].Bitmap,0,0,_listUnmountedObjects[i].Bitmap.Width,_listUnmountedObjects[i].Bitmap.Height);
			g.Restore(graphicsStateMount);
		}

		private int HitTest(int x,int y){
			for(int i=0;i<_listUnmountedObjects.Count;i++){
				if(x<_listUnmountedObjects[i].Xpos){
					continue;
				}
				if(x>_listUnmountedObjects[i].Xpos+_listUnmountedObjects[i].Width){
					continue;
				}
				if(y<_listUnmountedObjects[i].Ypos){
					continue;
				}
				if(y>_listUnmountedObjects[i].Ypos+_listUnmountedObjects[i].Height){
					continue;
				}
				return i;
			}
			return -1;
		}
		#endregion Methods - private

	}

	///<summary>This is a container that stores a mountItem, document, and bitmap.  MountItem must always be valid, while the other values can be null.</summary>
	public class UnmountedObject{
		public MountItem MountItem;
		public Document Document;
		public Bitmap Bitmap;
		//The position and size is completely different here than it was in the mount, so this stores position and size for layout, hit test, etc.
		//The field names mimic the MountItem field names for easier code reuse.
		public int Xpos;
		public int Ypos;
		public int Width;
		public int Height;
		
		public Rectangle GetBounds(){
			return new Rectangle(Xpos,Ypos,Width,Height);
		}
	}
}
