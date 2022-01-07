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
			HasBorder=true;//Required because we specified a default value of true for the designer.
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
		[Category("Appearance"),Description("The image displayed in the PictureBox.")]
		[DefaultValue(null)]
		public Image Image{
			set{
				image=value;
				Invalidate();
			}
			get{
				return image;
			}
		}

		///<summary></summary>
		[Category("Appearance"),Description("The text that will display if the image is null.")]
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
		[Category("Appearance"),Description("Determines whether to draw the border of the picturebox.")]
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
			base.OnPaint (e);
			Graphics g=e.Graphics;
			g.InterpolationMode=InterpolationMode.High;
			if(HasBorder) {
				g.DrawRectangle(Pens.Gray,0,0,Width-1,Height-1);
			}
			float imageHeight=-1;
			float imageWidth=-1;
			if(image!=null) {
				ODException.SwallowAnyException(() => {
					imageHeight=(float)image.Height;
					imageWidth=(float)image.Width;
				});
			}
			if(image==null || imageHeight<=0 || imageWidth<=0){
				StringFormat format=new StringFormat();
				format.Alignment=StringAlignment.Center;
				format.LineAlignment=StringAlignment.Center;
				g.DrawString(textNullImage,this.Font,new SolidBrush(Color.Gray),
					new RectangleF(0,0,Width,Height),format);
			}
			else{
				float ratio;
				//Debug.WriteLine("Hratio:"+(float)image.Height/(float)Height+"Wratio:"+(float)image.Width/(float)Width);
				if(imageHeight/(float)Height > imageWidth/(float)Width){//Image is proportionally taller
					ratio=(float)Height/imageHeight;
					g.DrawImage(image,new RectangleF(Width/2-(imageWidth*ratio)/2,0,imageWidth*ratio,Height));
				}
				else{//image proportionally wider
					ratio=(float)Width/imageWidth;
					g.DrawImage(image,new RectangleF(0,(float)Height/2-(imageHeight*ratio)/2,Width,imageHeight*ratio));
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
