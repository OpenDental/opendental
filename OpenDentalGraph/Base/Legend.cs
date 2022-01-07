using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentalGraph.Enumerations;
using System.Linq;
using CodeBase;

namespace OpenDentalGraph {
	public partial class Legend:UserControl {
		#region Private Data
		private LegendDockType _legendDock=LegendDockType.Left;
		private List<ODGraphLegendItem> _legendItems=new List<ODGraphLegendItem>();
		private float _paddingPx=3f;
		private int _scrollOffsetX=0;
		private int _contentWidth=0;
		private DateTime _stepDownStart;
		private DateTime _stepUpStart;
		private bool _mouseIsDown=false;
		private int _lastMouseX=0;
		#endregion

		#region Properties
		public LegendDockType LegendDock {
			get { return _legendDock; }
			set {
				_legendDock=value;
				_scrollOffsetX=0;
				try {
					switch(_legendDock) {
						case LegendDockType.Bottom:
							panelDraw.AutoScroll=false;
							panelDraw.AutoScrollMinSize=new Size(0,0);
							tableLayoutPanel1.ColumnStyles[0].Width=68;
							tableLayoutPanel1.ColumnStyles[2].Width=68;
							break;
						case LegendDockType.Left:
							panelDraw.AutoScroll=true;
							tableLayoutPanel1.ColumnStyles[0].Width=0;
							tableLayoutPanel1.ColumnStyles[2].Width=0;
							break;
						case LegendDockType.None:
						default:
							break;
					}
				}
				catch(Exception e) {
					e.DoNothing();					
				}
				panelDraw.Invalidate();
			}
		}				
		public float PaddingPx {
			get { return _paddingPx; }
			set { _paddingPx=value; panelDraw.Invalidate(); }
		}
		#endregion

		#region Ctor/Init
		public Legend() {
			InitializeComponent();
		}

		public void SetLegendItems(List<ODGraphLegendItem> items) {
			_legendItems=items;
			float leftBoxPx=0;
			float boxEdgePx=0;
			using(Graphics g = panelDraw.CreateGraphics()) {
				boxEdgePx=g.MeasureString("A",this.Font).Height;
				foreach(ODGraphLegendItem legendItem in _legendItems) {
					leftBoxPx+=PaddingPx+boxEdgePx+PaddingPx+g.MeasureString(legendItem.ItemName,this.Font).Width+PaddingPx;
				}
			}
			_contentWidth=(int)Math.Ceiling(leftBoxPx);
			this.Invalidate();
			panelDraw.Invalidate();
		}
		#endregion

		#region Drawing
		private void DrawLegendBottom(PaintEventArgs e) {			
			float topPx=5;
			float leftBoxPx=0;
			float boxEdgePx=e.Graphics.MeasureString("A",this.Font).Height;
			float maxBottom=boxEdgePx+PaddingPx;
			e.Graphics.TranslateTransform(_scrollOffsetX,0);
			foreach(ODGraphLegendItem legendItem in _legendItems) {
				using(Brush brushText=new SolidBrush(legendItem.IsEnabled ? this.ForeColor : Color.FromArgb(100,this.ForeColor))) {
					leftBoxPx+=PaddingPx;
					SizeF sizeText=e.Graphics.MeasureString(legendItem.ItemName,this.Font);
					using(Brush hoverBox=new SolidBrush(legendItem.Hovered?Color.LightCoral:panelDraw.BackColor)) {
						if(legendItem.Hovered) {
							e.Graphics.FillRectangle(hoverBox,leftBoxPx-2,topPx-2,boxEdgePx+4,boxEdgePx+4);
						}
						else {
							e.Graphics.FillRectangle(hoverBox,leftBoxPx-2,topPx-2,boxEdgePx+4,boxEdgePx+4);
						}
					}
					using(Brush brushBox=new SolidBrush(legendItem.IsEnabled?legendItem.ItemColor:Color.FromArgb(50,legendItem.ItemColor))) {
						e.Graphics.FillRectangle(brushBox,leftBoxPx,topPx,boxEdgePx,boxEdgePx);
						legendItem.LocationBox=new Rectangle((int)leftBoxPx,(int)topPx,(int)boxEdgePx,(int)boxEdgePx);
					}
					float textLeftPx=leftBoxPx+boxEdgePx+PaddingPx;
					e.Graphics.DrawString(legendItem.ItemName,this.Font,brushText,textLeftPx,topPx);
					leftBoxPx=textLeftPx+sizeText.Width+PaddingPx;					
				}
			}
			Size sizeContents=new Size((int)Math.Ceiling(leftBoxPx),(int)Math.Ceiling(maxBottom));
		}

		private void DrawLegendLeft(PaintEventArgs e) {
			float topPx=0;
			float leftBoxPx=PaddingPx;
			float boxEdgePx=e.Graphics.MeasureString("A",this.Font).Height;
			float maxRightEdge=0f;
			float maxBottom=0f;
			e.Graphics.TranslateTransform(panelDraw.AutoScrollPosition.X,panelDraw.AutoScrollPosition.Y);
			foreach(ODGraphLegendItem legendItem in _legendItems) {
				using(Brush brushText=new SolidBrush(legendItem.IsEnabled ? this.ForeColor : Color.FromArgb(100,this.ForeColor))) {
					topPx+=PaddingPx;
					SizeF size=e.Graphics.MeasureString(legendItem.ItemName,this.Font);
					using(Brush hoverBox = new SolidBrush(legendItem.Hovered ? Color.LightCoral : panelDraw.BackColor)) {
						if(legendItem.Hovered) {
							e.Graphics.FillRectangle(hoverBox,leftBoxPx-2,topPx-2,boxEdgePx+4,boxEdgePx+4);
						}
						else {
							e.Graphics.FillRectangle(hoverBox,leftBoxPx-2,topPx-2,boxEdgePx+4,boxEdgePx+4);
						}
					}
					using(Brush brushBox=new SolidBrush(legendItem.IsEnabled ? legendItem.ItemColor : Color.FromArgb(50,legendItem.ItemColor))) {
						e.Graphics.FillRectangle(brushBox,leftBoxPx,topPx,boxEdgePx,boxEdgePx);
						legendItem.LocationBox=new Rectangle((int)leftBoxPx,(int)topPx,(int)boxEdgePx,(int)boxEdgePx);
					}
					float textLeftPx=leftBoxPx+boxEdgePx+PaddingPx;
					e.Graphics.DrawString(legendItem.ItemName,this.Font,brushText,textLeftPx,topPx);
					topPx+=boxEdgePx;
					maxRightEdge=Math.Max(maxRightEdge,textLeftPx+size.Width);
					maxBottom+=PaddingPx+size.Height;
				}
			}
			Size sizeContents=new Size((int)Math.Ceiling(maxRightEdge),(int)Math.Ceiling(maxBottom));			
			if(panelDraw.AutoScrollMinSize!=sizeContents) {
				panelDraw.AutoScrollMinSize=sizeContents;
				//this.AutoScrollPosition=new Point(0,0);
			}
		}

		private void Legend_Paint(object sender,PaintEventArgs e) {
			e.Graphics.Clear(this.BackColor);
			switch(LegendDock) {
				case LegendDockType.Bottom:
					DrawLegendBottom(e);
					break;
				case LegendDockType.Left:
					DrawLegendLeft(e);
					break;
				case LegendDockType.None:
				default:
					break;
			}

		}
		#endregion

		#region Scrolling
		private void butScrollEnd_Click(object sender,EventArgs e) {
			int maxScroll=_contentWidth-panelDraw.Width;
			_scrollOffsetX=-maxScroll;
			panelDraw.Invalidate();
		}

		private void butScrollDownStep_Click(object sender,EventArgs e) {
			_scrollOffsetX-=50;
			int maxScroll=_contentWidth-panelDraw.Width;
			if(Math.Abs(_scrollOffsetX)>maxScroll) {
				_scrollOffsetX=-maxScroll;
			}
			if(_scrollOffsetX>0) {
				_scrollOffsetX=0;
			}
			panelDraw.Invalidate();
		}

		private void butScrollStart_Click(object sender,EventArgs e) {
			_scrollOffsetX=0;
			panelDraw.Invalidate();
		}

		private void butScrollUpStep_Click(object sender,EventArgs e) {
			_scrollOffsetX+=50;
			if(_scrollOffsetX>0) {
				_scrollOffsetX=0;
			}
			panelDraw.Invalidate();
		}
		
		private void timerStepUp_Tick(object sender,EventArgs e) {
			if(DateTime.Now.Subtract(_stepUpStart).TotalMilliseconds>400) {
				timerStepUp.Interval=20;
			}
			butScrollUpStep_Click(sender,e);
		}

		private void timerStepDown_Tick(object sender,EventArgs e) {
			if(DateTime.Now.Subtract(_stepDownStart).TotalMilliseconds>400) {
				timerStepDown.Interval=20;
			}
			butScrollDownStep_Click(sender,e);
		}

		private void butScrollDownStep_MouseDown(object sender,MouseEventArgs e) {
			_stepDownStart=DateTime.Now;
			timerStepDown.Interval=200;
			timerStepDown.Start();
		}

		private void butScrollDownStep_MouseUp(object sender,MouseEventArgs e) {
			timerStepDown.Stop();
		}
		
		private void butScrollUpStep_MouseDown(object sender,MouseEventArgs e) {
			_stepUpStart=DateTime.Now;
			timerStepUp.Interval=200;
			timerStepUp.Start();
		}

		private void butScrollUpStep_MouseUp(object sender,MouseEventArgs e) {
			timerStepUp.Stop();
		}
		
		private void panelDraw_MouseDown(object sender,MouseEventArgs e) {
			_mouseIsDown=true;
			_lastMouseX=e.X;
			ODGraphLegendItem clicked=null;
			if(LegendDock==LegendDockType.Bottom) {
				clicked=_legendItems.
				FirstOrDefault(x =>
					new Rectangle(x.LocationBox.X+_scrollOffsetX,x.LocationBox.Y,x.LocationBox.Width,x.LocationBox.Height)
				.Contains(e.X,e.Y));
			}
			else {
				clicked=_legendItems.
			FirstOrDefault(x =>
				new Rectangle(x.LocationBox.X,x.LocationBox.Y+panelDraw.AutoScrollPosition.Y,x.LocationBox.Width,x.LocationBox.Height)
			.Contains(e.X,e.Y));
			}
			if(clicked==null) {
				return;
			}
			clicked.IsEnabled=!clicked.IsEnabled;
			clicked.Filter();
		}

		private void panelDraw_MouseMove(object sender,MouseEventArgs e) {
			foreach(ODGraphLegendItem legendItem in _legendItems) { //reset hovered status.
				legendItem.Hovered=false;
			}
			if(!_mouseIsDown) { //hovering
				ODGraphLegendItem hovered=null;
				if(LegendDock==LegendDockType.Bottom) {
					hovered=_legendItems //find the hovered item.
					.FirstOrDefault(x => new Rectangle(x.LocationBox.X+_scrollOffsetX,x.LocationBox.Y,x.LocationBox.Width,x.LocationBox.Height)
					.Contains(e.X,e.Y));
				}
				else {
					hovered=_legendItems //find the hovered item.
					.FirstOrDefault(x => new Rectangle(x.LocationBox.X,x.LocationBox.Y+panelDraw.AutoScrollPosition.Y,x.LocationBox.Width,x.LocationBox.Height)
					.Contains(e.X,e.Y));
				}
				if(hovered==null) { //not hovering over a box, don't mark any hovered and redraw.
					panelDraw.Invalidate();
					return;
				}
				hovered.Hovered=true; //hovering over a box, mark that box as hovered and redraw.
				panelDraw.Invalidate();
			}
			else { //dragging
				if(LegendDock!=LegendDockType.Bottom) { //no dragging for Left-Docked legends; they get a scrollbar.
					return;
				}
				int move=e.X-_lastMouseX;
				_lastMouseX=e.X;
				_scrollOffsetX+=move;
				int maxScroll=_contentWidth-panelDraw.Width;
				if(Math.Abs(_scrollOffsetX)>maxScroll) {
					_scrollOffsetX=-maxScroll;
				}
				if(_scrollOffsetX>0) {
					_scrollOffsetX=0;
				}
				panelDraw.Invalidate();
			}
		}

		private void panelDraw_MouseUp(object sender,MouseEventArgs e) {
			_mouseIsDown=false;
			_lastMouseX=0;
		}

		#endregion

		///<summary>Returns a copy of the current legend without any of the buttons for printing purposes.</summary>
		public Legend PrintCopy() {
			Legend retVal = new Legend() {
				BackColor=Color.White,
				LegendDock=LegendDockType.Bottom,
				BorderStyle=BorderStyle.Fixed3D,
				Dock=DockStyle.None,
			};
			retVal.SetLegendItems(_legendItems);
			retVal.tableLayoutPanel1.ColumnStyles[0].SizeType=SizeType.Absolute;
			retVal.tableLayoutPanel1.ColumnStyles[0].Width=0;
			retVal.tableLayoutPanel1.ColumnStyles[2].SizeType=SizeType.Absolute;
			retVal.tableLayoutPanel1.ColumnStyles[2].Width=0;
			return retVal;
		}


		private class PanelNoFlicker:Panel {
			public PanelNoFlicker() {
				this.DoubleBuffered=true;
			}
		}
	}
}
