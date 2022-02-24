using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {
	public partial class EscalationViewControl:UserControl, INotifyPropertyChanged {
		
		public event PropertyChangedEventHandler PropertyChanged;
		///<summary>Key: Employee name, Value: True if the employee is at their desk.</summary>
		public Dictionary<string,bool> DictProximity=new Dictionary<string, bool>();
		///<summary>Key: Employee name, Value: True if the row should show the employee extension instead of the proximity figure.</summary>
		public Dictionary<string,bool> DictShowExtension=new Dictionary<string,bool>();
		///<summary>Key: Employee name, Value: The extension.</summary>
		public Dictionary<string,int> DictExtensions=new Dictionary<string,int>();
		///<summary>Key: Employee name, Value: True if the employee is on a web chat.</summary>
		public Dictionary<string,bool> DictWebChat=new Dictionary<string, bool>();
		///<summary>Key: Employee name, Value: True if the employee is on a GTA chat.</summary>
		public Dictionary<string,bool> DictGTAChat=new Dictionary<string, bool>();
		///<summary>Key: Employee name, Value: True if the employee is in a Remote Support session.</summary>
		public Dictionary<string,bool> DictRemoteSupport=new Dictionary<string, bool>();

		private bool _isUpdating=false;
		///<summary>When the mouse wheel scrolls we adjust the starting index for the strings we draw from _items.</summary>
		private int indexItemStart=0;
		///<summary>Flag used to indicate when we are passing in new items to draw, when true we do not reset the scroll value.</summary>
		public bool IsNewItems;
		///<summary>The size it takes to measure the letter 'a' using the controls current Font.</summary>
		private Size _sizeFont;
		///<summary>The size it takes to measure the extension '9999' using the controls current Font.</summary>
		private SizeF _sizeExtension;

		private int _rowTextHeight {
			get {
				if(_sizeFont.Width==0 && _sizeFont.Height==0) {
					_sizeFont=TextRenderer.MeasureText("a",Font);
				}
				return _sizeFont.Height;
			}
		}

		private float _rowTotalHeight {
			get {
				return _rowTextHeight+(LinePadding*2);
			}
		}

		private BindingList<String> _items=new BindingList<string>();
		[Category("Appearance")]
		[Description("Strings to be printed")]
		public BindingList<String> Items {
			get {
				return _items;
			}
			set {
				_items=value;
				PropertyChanged(this,new PropertyChangedEventArgs("Items"));
			}
		}

		private int _borderThickness=6;
		[Category("Appearance")]
		[Description("Thickness of the border drawn around the control")]
		public int BorderThickness {
			get {
				return _borderThickness;
			}
			set {
				_borderThickness=value;
				PropertyChanged(this,new PropertyChangedEventArgs("BorderThickness"));
			}
		}

		private Color _outerColor=Color.Black;
		[Category("Appearance")]
		[Description("Exterior Border Color")]
		public Color OuterColor {
			get {
				return _outerColor;
			}
			set {
				_outerColor=value;
				PropertyChanged(this,new PropertyChangedEventArgs("OuterColor"));
			}
		}

		private int _linePadding=-6;
		[Category("Appearance")]
		[Description("Padding of each line. Suggest -6 for 0 padding between lines. Must be an even number.")]
		public int LinePadding {
			get {
				return _linePadding;
			}
			set {
				_linePadding=value;
				PropertyChanged(this,new PropertyChangedEventArgs("LinePadding"));
			}
		}

		private int _startFadeIndex=4;
		[Category("Appearance")]
		[Description("Lines will start to fade at this 0-based index.")]
		public int StartFadeIndex {
			get {
				return _startFadeIndex;
			}
			set {
				_startFadeIndex=value;
				PropertyChanged(this,new PropertyChangedEventArgs("StartFadeIndex"));
			}
		}

		private int _fadeAlphaIncrement=40;
		[Category("Appearance")]
		[Description("Alpha increment to be subtracted from each faded line. Will be subtracted from each line after StartFadeIndex and eventually bottom out at 0 (fully transparent).")]
		public int FadeAlphaIncrement {
			get {
				return _fadeAlphaIncrement;
			}
			set {
				_fadeAlphaIncrement=value;
				PropertyChanged(this,new PropertyChangedEventArgs("FadeAlphaIncrement"));
			}
		}

		private int _minAlpha=60;
		[Category("Appearance")]
		[Description("Minimum alpha transparency value. Set to 0 if full transparency is desired. Otherwise a number between 0-255. 0 is full transparent, 255 is full opaque.")]
		public int MinAlpha {
			get {
				return _minAlpha;
			}
			set {
				_minAlpha=value;
				PropertyChanged(this,new PropertyChangedEventArgs("MinAlpha"));
			}
		}
	
		public EscalationViewControl() {
			InitializeComponent();
			PropertyChanged+=EscalationViewControl_PropertyChanged;

		}

		private void EscalationViewControl_PropertyChanged(object sender,PropertyChangedEventArgs e) {
			Invalidate();
		}

		private void EscalationViewControl_Paint(object sender,PaintEventArgs e) {
			if(_isUpdating) {
				return;
			}
			Pen penOuter=new Pen(OuterColor,BorderThickness);
			try {
				Image ProxImage=Properties.Resources.Figure;
				Image webChatIcon=Properties.Resources.WebChatIcon;
				Image gtaIcon=Properties.Resources.gtaicon3;
				Image remoteSupportIcon=Properties.Resources.remoteSupportIcon;
				float chatIconWidth=18F;//Both the web chat and GTA icons are 18 pixels wide.
				RectangleF rcOuter=this.ClientRectangle;
				if(_sizeExtension.Width==0 && _sizeExtension.Height==0) {
					_sizeExtension=e.Graphics.MeasureString("9999",Font);
				}
				//clear control canvas
				e.Graphics.Clear(this.BackColor);
				float halfPenThickness=BorderThickness/(float)2;
				//deflate for border
				rcOuter.Inflate(-halfPenThickness,-halfPenThickness);
				//draw border
				e.Graphics.DrawRectangle(penOuter,rcOuter.X,rcOuter.Y,rcOuter.Width,rcOuter.Height);
				//deflate to drawable region
				rcOuter.Inflate(-halfPenThickness,-halfPenThickness);
				int alpha=255;
				int countDrawn=0;//Count of items drawn to the control, used to calculate y position for drawing.
				for(int i=0;i<_items.Count;i++) {
					string item=_items[i];
					if(i>StartFadeIndex) { //Only start fading after the user defined fade index.
						//Move toward transparency.
						alpha=Math.Max(MinAlpha,alpha-FadeAlphaIncrement);
					}
					if(_items.IndexOf(item)<indexItemStart) {
						continue;//We have scrolled past the current item, do not draw.
					}
					//Set the bounds of the drawing rectangle.
					float y=rcOuter.Y+(countDrawn*_rowTextHeight)+(countDrawn*(2*LinePadding));
					RectangleF rcItem=new RectangleF(rcOuter.X,y,rcOuter.Width,_rowTotalHeight);
					//Figure out the maximum size to allot for the item text (employee name or office down text).
					float _startExtensionX=rcOuter.X+rcOuter.Width-_sizeExtension.Width;
					float _startProxImageX=_startExtensionX-ProxImage.Width;
					float _startChatIconX=_startProxImageX-chatIconWidth;
					bool hasExtension=(DictShowExtension.ContainsKey(item) && DictShowExtension[item]==true && DictExtensions.ContainsKey(item));
					DictWebChat.TryGetValue(item,out bool hasWebChat);
					DictGTAChat.TryGetValue(item,out bool hasGtaChat);
					DictProximity.TryGetValue(item,out bool hasProximity);
					DictRemoteSupport.TryGetValue(item,out bool hasRemoteSupportSession);
					#region Draw Item Text (emp name or office down)
					StringFormat sf=new StringFormat(StringFormatFlags.NoWrap);
					sf.LineAlignment=StringAlignment.Center;//Vertically align the text in the center of the row.
					using(Brush brushText=new SolidBrush(Color.FromArgb(alpha,ForeColor))) {
						if(!hasExtension && !hasWebChat && !hasGtaChat && !hasProximity && !hasRemoteSupportSession) {
							//Typically "offices down" which will have the time they've been down, followed by a dash (-) and then the patnum.
							//Use the entire drawable area because there are no icons or extensions that are right aligned.
							e.Graphics.DrawString(item,Font,brushText,rcItem,sf);
						}
						else {//Typically an employee name, some icons, and an extension.
							//Make a smaller rectangle that will not overlap any extensions.
							//We purposefully let the name overlap the icons because BenjaminH has the 'H' cut off if we don't allow it to overlap.
							RectangleF rectDisplayText=new RectangleF(rcItem.X,y,_startExtensionX,_rowTotalHeight);
							e.Graphics.DrawString(item,Font,brushText,rectDisplayText,sf);
						}
					}
					#endregion
					countDrawn++;
					#region Extension
					//We always want to draw the extension for employees within escalation views.
					if(hasExtension) {
						//Display the extension in the right hand side of the view.
						string ext=DictExtensions[item].ToString();
						rcItem=new RectangleF(_startExtensionX,y,_sizeExtension.Width,_rowTotalHeight);
						using(Brush brushText=new SolidBrush(Color.FromArgb(alpha,ForeColor))) {
							e.Graphics.DrawString(ext,Font,brushText,rcItem,sf);
						}
					}
					#endregion
					#region Chat Icons
					if(hasWebChat) {
						using(Bitmap bitmap = new Bitmap(webChatIcon)) {
							float ImgY = y+(webChatIcon.Height/2);
							RectangleF rectImage = new RectangleF(_startChatIconX,ImgY,webChatIcon.Width,webChatIcon.Height);
							e.Graphics.DrawImage(
								webChatIcon,
								rectImage,
								new RectangleF(0,0,bitmap.Width,bitmap.Height),
								GraphicsUnit.Pixel);
						}
					}
					else if(hasGtaChat) {
						using(Bitmap bitmap = new Bitmap(gtaIcon)) {
							float ImgY = y+(gtaIcon.Height/2);
							RectangleF rectImage = new RectangleF(_startChatIconX,ImgY,gtaIcon.Width,gtaIcon.Height);
							e.Graphics.DrawImage(
								gtaIcon,
								rectImage,
								new RectangleF(0,0,bitmap.Width,bitmap.Height),
								GraphicsUnit.Pixel);
						}
					}
					else if(hasRemoteSupportSession) {
						using(Bitmap bitmap=new Bitmap(remoteSupportIcon)) {
							float ImgY=(y + (remoteSupportIcon.Height/2));
							RectangleF rectImage=new RectangleF(_startChatIconX,ImgY,remoteSupportIcon.Width,remoteSupportIcon.Height);
							e.Graphics.DrawImage(
								remoteSupportIcon,
								rectImage,
								new RectangleF(0,0,bitmap.Width,bitmap.Height),
								GraphicsUnit.Pixel);
						}
					}
					#endregion
					#region Little Proximity Guy
					//The little proximity guy should only show for employees that are proximal AND at the same location that the currently selected room is at.
					if(hasProximity) {
						//If we're not displaying the extension, we'll show the little proximity figure.
						using(Bitmap bitmap=new Bitmap(ProxImage)) {
							float proxImgY=y+(ProxImage.Height/2);
							RectangleF rectImage=new RectangleF(_startProxImageX,proxImgY,ProxImage.Width,ProxImage.Height);
							e.Graphics.DrawImage(
								ProxImage,
								rectImage,
								new RectangleF(0,0,bitmap.Width,bitmap.Height),
								GraphicsUnit.Pixel);
						}
					}
					#endregion
				}
			}
			catch(Exception ex) {
				//Something went wrong with paiting the control.  No reason to crash the program.  Odds are paint will get called again very soon.
				ex.DoNothing();
			}
			finally {
				penOuter.Dispose();
			}
		}

		public void BeginUpdate() {
			_isUpdating=true;
		}

		public void EndUpdate() {
			_isUpdating=false;
			if(IsNewItems) {
				indexItemStart=0;//Resets scroll value after updates.
				IsNewItems=false;
			}
			Invalidate();
		}

		private void EscalationViewControl_MouseWheel(object sender,MouseEventArgs e) {
			if(e.Delta>0) {//MouseWheel up
				indexItemStart=Math.Max(indexItemStart-1,0);//Ensures we do not scroll above first item.
			}
			else {//MouseWheel down
				indexItemStart=Math.Min(indexItemStart+1,_items.Count-1);//-1 so that there will always be 1 item drawn.
				if(_items.Count-indexItemStart<8) {//Difference represents the number of items we are goin to draw.
					indexItemStart=_items.Count-8;//Always keep the escalation view full, last item will be at bottom of the view.
				}
			}
			PropertyChanged(this,new PropertyChangedEventArgs("MouseWheel"));
		}
	}
}
