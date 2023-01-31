using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.InternalTools.Phones {
	public partial class EscalationView:Control {
		#region Fields - Public
		public int FadeAlphaIncrement=0;
		public bool IsNewItems;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public List<EscalationItem> ListEscalationItems=new List<EscalationItem>();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>When the mouse wheel scrolls we adjust the starting index for the strings we draw from _items.</summary>
		private int _indexItemStart=0;
		///<summary>Flag used to indicate when we are passing in new items to draw, when true we reset the scroll value.</summary>
		private bool _isUpdating=false;
		#endregion Fields - Private

		#region Constructor
		public EscalationView() {
			InitializeComponent();
			DoubleBuffered=true;
		}
		#endregion Constructor

		protected override void OnPaint(PaintEventArgs pe) {
			//base.OnPaint(pe);
			if(_isUpdating) {
				return;
			}
			Graphics g=pe.Graphics;
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			using Bitmap bitmapFigure=Properties.Resources.Figure;
			using Bitmap bitmapWebChat=Properties.Resources.WebChatIcon;
			using Bitmap bitmapGta=Properties.Resources.gtaicon3;
			using Bitmap bitmapRemoteSupport=Properties.Resources.remoteSupportIcon;
			float widthChatIcon=18F;//Both the web chat and GTA icons are 18 pixels wide.
			//float widthExtension=LayoutManager.ScaleMS(g.MeasureString("9999",Font).Width);
			float widthExtension=g.MeasureString("9999",Font).Width;//I think the above line is correct, but this one looks better
			g.Clear(BackColor);
			g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			int alpha=255;
			int countDrawn=0;//Count of items drawn to the control, used to calculate y position for drawing.
			int idxStartFade=0;
			int minAlpha=60;
			//float rowTextHeight=Font.Height;
			int linePadding=-LayoutManager.Scale(6);
			float heightFont=LayoutManager.ScaleMS(Font.Height);
			float heightRowTotal=heightFont+(linePadding*2);
			for(int i=0;i<ListEscalationItems.Count;i++){
				if(i>idxStartFade) { //Only start fading after the user defined fade index.
					//Move toward transparency.
					alpha=Math.Max(minAlpha,alpha-FadeAlphaIncrement);
				}
				if(i<_indexItemStart) {
					continue;//We have scrolled past the current item, do not draw.
				}
				float y=(countDrawn*heightFont)+(countDrawn*(2*linePadding));
				RectangleF rectangleF=new RectangleF(0,y,Width,heightRowTotal);
				//Figure out the maximum size to allot for the item text (employee name or office down text).
				float startExtensionX=Width-widthExtension;
				float startProxImageX=startExtensionX-LayoutManager.ScaleF(bitmapFigure.Width);
				float startChatIconX=startProxImageX-LayoutManager.ScaleF(widthChatIcon);
				using StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
				stringFormat.LineAlignment=StringAlignment.Center;//Vertically align the text in the center of the row.
				using Brush brushText=new SolidBrush(Color.FromArgb(alpha,ForeColor));
				if(ListEscalationItems[i].ShowExtension || ListEscalationItems[i].IsWebChat || ListEscalationItems[i].IsGTAChat || ListEscalationItems[i].IsProximity || ListEscalationItems[i].IsRemoteSupport)
				{//Typically an employee name, some icons, and an extension.
					//Make a smaller rectangle that will not overlap any extensions.
					//We purposefully let the name overlap the icons because BenjaminH has the 'H' cut off if we don't allow it to overlap.
					RectangleF rectDisplayText=new RectangleF(rectangleF.X,y,startExtensionX,heightRowTotal);
					g.DrawString(ListEscalationItems[i].EmpName,Font,brushText,rectDisplayText,stringFormat);
				}
				else{//Typically "offices down" which will have the time they've been down, followed by a dash (-) and then the patnum.
					//Use the entire drawable area because there are no icons or extensions that are right aligned.
					g.DrawString(ListEscalationItems[i].EmpName,Font,brushText,rectangleF,stringFormat);
				}
				countDrawn++;
				//We always want to draw the extension for employees within escalation views.
				if(ListEscalationItems[i].ShowExtension && ListEscalationItems[i].Extension>0) {
					//Display the extension in the right hand side of the view.
					rectangleF=new RectangleF(startExtensionX,y,widthExtension,heightRowTotal);
					using Brush brushText2=new SolidBrush(Color.FromArgb(alpha,ForeColor));
					g.DrawString(ListEscalationItems[i].Extension.ToString(),Font,brushText2,rectangleF,stringFormat);
				}
				if(ListEscalationItems[i].IsWebChat) {
					g.DrawImage(bitmapWebChat,
						x:startChatIconX,
						y:y+(LayoutManager.ScaleF(bitmapWebChat.Height)/2),
						width:LayoutManager.ScaleF(bitmapWebChat.Width),
						height:LayoutManager.ScaleF(bitmapWebChat.Height));
				}
				else if(ListEscalationItems[i].IsGTAChat) {
					//float ImgY = y+(gtaIcon.Height/2);
					//RectangleF rectImage = new RectangleF(startChatIconX,ImgY,gtaIcon.Width,gtaIcon.Height);
					g.DrawImage(bitmapGta,
						startChatIconX,y+(LayoutManager.ScaleF(bitmapGta.Height)/2),
						LayoutManager.ScaleF(bitmapGta.Width),LayoutManager.ScaleF(bitmapGta.Height));
				}
				else if(ListEscalationItems[i].IsRemoteSupport) {
					float ImgY=(y + (LayoutManager.ScaleF(bitmapRemoteSupport.Height)/2));
					RectangleF rectImage=new RectangleF(startChatIconX,ImgY,LayoutManager.ScaleF(bitmapRemoteSupport.Width),LayoutManager.ScaleF(bitmapRemoteSupport.Height));
					g.DrawImage(bitmapRemoteSupport,
						startChatIconX,y+(LayoutManager.ScaleF(bitmapRemoteSupport.Height)/2),
						LayoutManager.ScaleF(bitmapRemoteSupport.Width),LayoutManager.ScaleF(bitmapRemoteSupport.Height));
				}
				//The little proximity guy should only show for employees that are proximal AND at the same location that the currently selected room is at.
				if(ListEscalationItems[i].IsProximity) {
					//If we're not displaying the extension, we'll show the little proximity figure.
					float proxImgY=y+(LayoutManager.ScaleF(bitmapFigure.Height)/2);
					RectangleF rectImage=new RectangleF(startProxImageX,proxImgY,LayoutManager.ScaleF(bitmapFigure.Width),LayoutManager.ScaleF(bitmapFigure.Height));
					g.DrawImage(bitmapFigure,
						startProxImageX,y+(LayoutManager.ScaleF(bitmapFigure.Height)/2),
						LayoutManager.ScaleF(bitmapFigure.Width),LayoutManager.ScaleF(bitmapFigure.Height));
				}
			}

		}

		public void BeginUpdate() {
			_isUpdating=true;
		}

		public void EndUpdate() {
			_isUpdating=false;
			if(IsNewItems) {
				_indexItemStart=0;//Resets scroll value after updates.
				IsNewItems=false;
			}
			Invalidate();
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(e.Delta>0) {//MouseWheel up
				_indexItemStart=Math.Max(_indexItemStart-1,0);//Ensures we do not scroll above first item.
			}
			else {//MouseWheel down
				_indexItemStart=Math.Min(_indexItemStart+1,ListEscalationItems.Count-1);//-1 so that there will always be 1 item drawn.
				if(ListEscalationItems.Count-_indexItemStart<1) {//Difference represents the number of items we are goin to draw.
					_indexItemStart=ListEscalationItems.Count-1;//Always keep the escalation view full, last item will be at bottom of the view.
				}
			}
			Invalidate();
		}
	}

	public class EscalationItem{
		///<summary>Or office down</summary>
		public string EmpName;
		///<summary>True if the employee is at their desk</summary>
		public bool IsProximity;
		///<summary>True if the row should show the employee extension instead of the proximity figure</summary>
		public bool ShowExtension;
		///<summary></summary>
		public int Extension;
		///<summary>True if the employee is on a web chat</summary>
		public bool IsWebChat;
		///<summary>True if the employee is on a GTA chat</summary>
		public bool IsGTAChat;
		///<summary>True if the employee is in a Remote Support session</summary>
		public bool IsRemoteSupport;
	}
}
