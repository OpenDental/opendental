using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI {
	///<summary></summary>
	public delegate void ODButtonPanelEventHandler(object sender,ODButtonPanelEventArgs e);

	///<summary>Allows for a button panel that is customizable and generated from data in the DB.</summary>
	public partial class ODButtonPanel:UserControl {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Fixed 18 at 96dpi</summary>
		private int _heightRow=18;
		private Point _pointMouseClick;
		public List<ODPanelItem> ListODPanelItems;
		public bool IsUpdating;
		///<summary>The last item that was clicked on. Can be null.</summary>
		public ODPanelItem SelectedItem;
		///<summary>The last row that was clicked on. -1 if no row selected.</summary>
		public int SelectedRow;
		//These static objects don't get disposed------------------------------------------------
		private static SolidBrush _brushLabelBackground=(SolidBrush)Brushes.White;
		private static SolidBrush _brushLabel=(SolidBrush)Brushes.Black;
		private static SolidBrush _brushBackgroundShadow=new SolidBrush(Color.FromArgb(202,212,222));
		private static SolidBrush _brushBackground=(SolidBrush)Brushes.White;
		private LinearGradientBrush _brushButtonPanel=null;
		private static Pen _penOutline=new Pen(Color.FromArgb(47,70,117));
		///<summary>Ignore the public font.  We want to control font internally.</summary>
		private Font _font;

		#region EventHandlers
		///<summary></summary>
		[Category("Action"),Description("Occurs when a button item is single clicked.")]
		public event ODButtonPanelEventHandler ItemClickBut=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when a row is double clicked.")]
		public event ODButtonPanelEventHandler RowDoubleClick=null;
		#endregion

		public ODButtonPanel() {
			InitializeComponent();
			ListODPanelItems=new List<ODPanelItem>();
			SelectedRow=-1;
			DoubleBuffered=true;
		}

		///<summary></summary>
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
		}

		///<summary></summary>
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			_font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.5f));
			_heightRow=LayoutManager.Scale(18);
			_brushButtonPanel?.Dispose();
			_brushButtonPanel=new LinearGradientBrush(new Point(0,0),new Point(0,_heightRow),Color.FromArgb(255,255,255),Color.FromArgb(205,212,215));
			Invalidate();
		}

		#region Computations
		///<summary>Computes the position of each column and the overall width.  Called from endUpdate and also from OnPaint.</summary>
		private void ComputeWidthsAndLocations(Graphics g) {
			ListODPanelItems.Sort(ODPanelItem.SortYX);
			for(int i=0;i<ListODPanelItems.Count;i++) {
				ListODPanelItems[i].CalculateWidth(g,Font);
				ListODPanelItems[i].Location.Y=ListODPanelItems[i].YPos*_heightRow;
				//Then set xPos using previous cell's position and width:
				if(i>0 && ListODPanelItems[i].YPos==ListODPanelItems[i-1].YPos) {//Previous item was on the same row. First item on each row is 0
					ListODPanelItems[i].Location.X=ListODPanelItems[i-1].Location.X+ListODPanelItems[i-1].ItemWidth;
				}
			}
		}

		///<summary>Returns the panel item clicked on. Returns null if no item found.</summary>
		public ODPanelItem PointToItem(Point loc) {
			ODPanelItem retVal=null;
			retVal=ListODPanelItems.Find(item =>
				item.YPos==(loc.Y/_heightRow) //item is on the clicked row
				&& item.Location.X<loc.X //point is to to the right of the left side
				&& item.Location.X+item.ItemWidth>loc.X); //point is the the left of the right side
			return retVal;
		}
		#endregion Painting

		#region Painting
		///<summary></summary>
		protected override void OnPaintBackground(PaintEventArgs pea) {
			//base.OnPaintBackground (pea);
			//don't paint background.  This reduces flickering.
		}

		///<summary>Runs any time the control is invalidated.</summary>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			if(IsUpdating) {
				return;
			}
			if(Width<1 || Height<1) {
				return;
			}
			try {
				e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
				ComputeWidthsAndLocations(e.Graphics);
				DrawBackG(e.Graphics);
				DrawItems(e.Graphics);
				DrawOutline(e.Graphics);
			}
			catch(Exception ex) {
				//We had one customer who was receiving overflow exceptions because the ClientRetangle provided by the system was invalid,
				//due to a graphics device hardware state change when loading the Dexis client application via our Dexis bridge.
				//If we receive an invalid ClientRectangle, then we will simply not draw the button for a frame or two until the system has initialized.
				//A couple of frames later the system should return to normal operation and we will be able to draw the button again.
				ex.DoNothing();
			}
		}

		///<summary>Draws a solid gray background.</summary>
		private void DrawBackG(Graphics g) {
			g.FillRectangle(_brushBackgroundShadow,
				0,0,
				Width,Height);//Creates a shadow on top and left of control.
			g.FillRectangle(_brushBackground,
				1,1,
				Width-1,Height-1);
		}

		private void DrawItems(Graphics g) {
			for(int i=0;i<ListODPanelItems.Count;i++) {
				switch(ListODPanelItems[i].ItemType) {
					case ODPanelItemType.Button:
						DrawItemBut(g,ListODPanelItems[i]);
						break;
					case ODPanelItemType.Label:
						DrawItemLabel(g,ListODPanelItems[i]);
						break;
				}
			}
		}

		private void DrawItemLabel(Graphics g,ODPanelItem item) {
			RectangleF itemRect=new RectangleF(
				item.Location.X,item.Location.Y,
				item.ItemWidth,_heightRow);
			g.FillRectangle(_brushLabelBackground,itemRect);
			g.DrawString(item.Text,_font,_brushLabel,itemRect,
				new StringFormat { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center });
		}

		private void DrawItemBut(Graphics g,ODPanelItem odPanelItem) {
			Rectangle recOutline=new Rectangle(odPanelItem.Location.X,odPanelItem.Location.Y,odPanelItem.ItemWidth-1,_heightRow-1);
			Button.DrawSimpleButton(g,recOutline,odPanelItem.Text,_font,_brushButtonPanel);
		}

		///<summary>Draws outline around entire control.</summary>
		private void DrawOutline(Graphics g) {
			g.DrawRectangle(_penOutline,0,0,Width-1,Height-1);
		}
		#endregion

		#region Single Click
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			SelectedItem=PointToItem(_pointMouseClick);
			SelectedRow=_pointMouseClick.Y/_heightRow;
			if(SelectedItem!=null) {
				//OnClickItem(SelectedItem);
				switch(SelectedItem.ItemType) {
					case ODPanelItemType.Button:
						OnClickButton(SelectedItem);
						break;
					//case ODPanelItemType.Label:
					//	OnClickLabel(SelectedItem);
					//	break;
				}
			}
			//OnRowClick(SelectedRow);
		}

		private void OnClickButton(ODPanelItem itemClick) {
			ODButtonPanelEventArgs pArgs=new ODButtonPanelEventArgs(SelectedItem,SelectedRow,MouseButtons.Left);
			if(ItemClickBut!=null) {
				ItemClickBut(this,pArgs);
			}
		}
		#endregion Single Click

		#region Double Click
		///<summary></summary>
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			SelectedItem=PointToItem(_pointMouseClick);
			SelectedRow=_pointMouseClick.Y/_heightRow;
			//if(SelectedItem!=null) {
			//	OnDoubleClickItem(SelectedItem);
			//	switch(SelectedItem.ItemType) {
			//		case ODPanelItemType.Button:
			//			OnDoubleClickButton(SelectedItem);
			//			break;
			//		case ODPanelItemType.Label:
			//			OnDoubleClickLabel(SelectedItem);
			//			break;
			//	}
			//}
			OnRowDoubleClick(_pointMouseClick.Y/_heightRow);
		}

		private void OnRowDoubleClick(int p) {
			ODButtonPanelEventArgs pArgs=new ODButtonPanelEventArgs(SelectedItem,SelectedRow,MouseButtons.Left);
			if(RowDoubleClick!=null) {
				RowDoubleClick(this,pArgs);
			}
		}
		#endregion Double Click

		#region MouseEvents

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			_pointMouseClick=new Point(e.X,e.Y);
			//ODPanelItem item=PointToItem(_pointMouseClick);
			//int row=e.Y/_rowHeight;
		}

		#endregion MouseEvents

		#region BeginEndUpdate
		///<summary>Call this before adding any rows.  You would typically call Rows.Clear after this.</summary>
		public void BeginUpdate() {
			IsUpdating=true;
		}

		///<summary>Must be called after adding rows.  This computes the columns, computes the rows, lays out the scrollbars, clears SelectedIndices, and invalidates.</summary>
		public void EndUpdate() {
			using(Graphics g=this.CreateGraphics()) {
				ComputeWidthsAndLocations(g);
			}
			IsUpdating=false;
			Invalidate();
		}
		#endregion BeginEndUpdate

	}

	///<summary></summary>
	public class ODButtonPanelEventArgs {
		private ODPanelItem item;
		private int row;
		private MouseButtons button;

		///<summary></summary>
		public ODButtonPanelEventArgs(ODPanelItem item,int row,MouseButtons button) {
			this.item=item;
			this.row=row;
			this.button=button;
		}

		///<summary>Can be null.</summary>
		public ODPanelItem Item {
			get {
				return item;
			}
		}

		///<summary></summary>
		public int Row {
			get {
				return row;
			}
		}

		///<summary>Gets which mouse button was pressed.</summary>
		public MouseButtons Button {
			get {
				return button;
			}
		}

	}

}
