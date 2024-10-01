using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenDental.UI {
	public class ODPanelItem {
		///<summary>Text to be displayed on the control.</summary>
		public string Text;
		///<summary>Currently label or button, determines how item will be drawn.</summary>
		public ODPanelItemType ItemType;
		///<summary>Zero based vertical row position.</summary>
		public int YPos;
		///<summary>Zero based horizontal ordering of controls on the same row.</summary>
		public int ItemOrder;
		///<summary>Used for attaching objects to this control. Potential uses: Images, procedures, delegate functions, etc. Will revisit later, maybe.</summary>
		public List<object> Tags;
		///<summary>Computed item width based on text, font, and graphics. Value is recalculated by ODButtonPanel when painting.</summary>
		public int ItemWidth;
		public Point Location;

		public ODPanelItem() {
			Tags=new List<object>();
			Location=new Point();
		}

		public static int SortYX(ODPanelItem p1,ODPanelItem p2) {
			if(p1.YPos!=p2.YPos) {
				return p1.YPos.CompareTo(p2.YPos);
			}
			if(p1.ItemOrder!=p2.ItemOrder) {
				return p1.ItemOrder.CompareTo(p2.ItemOrder);
			}
			return p1.Text.CompareTo(p2.Text);//should never happen, only if two buttons are at the same location.
		}

	}

	public enum ODPanelItemType{
		Button,
		Label
	}
}
