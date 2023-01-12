using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI
{
	/// <summary>Better and simpler than the MS picturebox.  Always resizes the image to fit in the box.  Never crops or stretches.</summary>
	public class ODPictureBox : System.Windows.Forms.Control
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Image image;
		private string textNullImage;
		private bool _hasBorder;

		///<summary></summary>
		public ODPictureBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			DoubleBuffered=true;
			HasBorder=true;//Required because we specified a default value of true for the designer.
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				image?.Dispose();
				components?.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		///<summary></summary>
		[Category("OD"),Description("The image displayed in the PictureBox.")]
		[DefaultValue(null)]
		public Image Image{
			set{
				//Return early if same image, otherwise when we dispose of 'image', 'value' will also be disposed of as well.
				if(image==value) {
					return;
				}
				image?.Dispose();
				image=value;
				Invalidate();
			}
			get{
				return image;
			}
		}

		///<summary></summary>
		[Category("OD"),Description("The text that will display if the image is null.")]
		public string TextNullImage{
			set{
				textNullImage=value;
				Invalidate();
			}
			get{
				return textNullImage;
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Determines whether to draw the border of the picturebox.")]
		[DefaultValue(true)]
		public bool HasBorder {
			set {
				_hasBorder=value;
				Invalidate();
			}
			get {
				return _hasBorder;
			}
		}

		///<summary></summary>
		protected override void OnPaint(PaintEventArgs e) {
			//base.OnPaint (e);
			Graphics g=e.Graphics;
			g.InterpolationMode=InterpolationMode.High;
			if(HasBorder) {
				g.DrawRectangle(Pens.Gray,0,0,Width-1,Height-1);
			}
			float heightImage=-1;
			float widthImage=-1;
			if(image!=null) {
				ODException.SwallowAnyException(() => {
					heightImage=(float)image.Height;
					widthImage=(float)image.Width;
				});
			}
			if(image==null || heightImage<=0 || widthImage<=0){
				StringFormat stringFormat=new StringFormat();
				stringFormat.Alignment=StringAlignment.Center;
				stringFormat.LineAlignment=StringAlignment.Center;
				g.DrawString(textNullImage,this.Font,new SolidBrush(Color.Gray),new RectangleF(0,0,Width,Height),stringFormat);
			}
			else{
				float ratio;
				//amount of space around the outside edge of the image in pixels. 1px shows border, 2px shows border and a little whitespace.
				int padding=0;
				if(HasBorder) {
					padding=1;
				}
				//Debug.WriteLine("Hratio:"+(float)image.Height/(float)Height+"Wratio:"+(float)image.Width/(float)Width);
				if(heightImage/(float)Height > widthImage/(float)Width){//Image is proportionally taller
					ratio=(float)Height/heightImage;
					g.DrawImage(image,new RectangleF(Width/2-(widthImage*ratio)/2+padding,padding,widthImage*ratio-(2*padding),Height-(2*padding)));
				}
				else{//image proportionally wider
					ratio=(float)Width/widthImage;
					g.DrawImage(image,new RectangleF(padding,(float)Height/2-(heightImage*ratio)/2+padding,Width-(2*padding),heightImage*ratio-(2*padding)));
				}
			}
		}

		///<summary></summary>
		protected override void OnResize(EventArgs e) {
			base.OnResize (e);
			Invalidate();
		}





	}


}
