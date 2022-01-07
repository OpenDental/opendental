using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	//Jordan is the only one allowed to edit this file.
	//See bottom of this file for example code for adding menu items.
	//Typically dock it to the top in the designer.  

	///<summary>Used in OD instead of MainMenu or MenuStrip.  Those will fail on high dpi monitors.  Never set item Visibility.  Use Available instead.</summary>
	public class MenuOD:Control{
		#region Fields
		///<summary></summary>
		private MenuStripOD _menuStripOD;
		///<summary>Just holds the scaling factor.</summary>
		private LayoutManagerForms _layoutManager=new LayoutManagerForms();
		#endregion Fields

		#region Constructor
		public MenuOD(){
			_menuStripOD=new MenuStripOD();
			if(LicenseManager.UsageMode==LicenseUsageMode.Designtime){
				return;
			}
			//_menuStripOD.Location=new Point(0,-1);//menu is too tall.  Hide the top edge. Maybe make this relative to font.
			_menuStripOD.Anchor=AnchorStyles.Top | AnchorStyles.Left;//it still insists on drawing centered vertically.
			//_menuStripOD.Padding=new Padding(0);//didn't change anything
			//_menuStripOD.Height=12;//gets ignored. Set in MenuItem instead.
			//_menuStripOD.Width gets set during LayoutItems, when it sets the bounds of each menuItem.
			_menuStripOD.Name="menuStripOD1";
			_menuStripOD.Renderer=new MenuRenderer();
			Controls.Add(_menuStripOD);
		}
		#endregion Constructor

		#region Properties
		protected override Size DefaultSize => new Size(200,24);//shouldn't change this unless we change all existing menus heights

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutManagerForms LayoutManager{
			get => _layoutManager; 
			set{
				_layoutManager = value;
				_menuStripOD.LayoutManager=_layoutManager;
			}
		}
		#endregion Properties

		#region Methods - Public
		public void Add(ToolStripItem toolStripItem){
			_menuStripOD.Items.Add(toolStripItem);
			_menuStripOD.LayoutItems();
			Invalidate();
		}

		public void Add(string text,EventHandler click){
			_menuStripOD.Items.Add(new MenuItemOD(text,click));
			_menuStripOD.LayoutItems();
			Invalidate();
		}

		///<summary>Optional. When adding a large number of menu items, this can be used to slightly increase efficiency.  Use EndUpdate after adding all the menu items.</summary>
		public void BeginUpdate(){
			_menuStripOD.IsUpdating=true;
		}

		public void EndUpdate(){
			_menuStripOD.IsUpdating=false;
			_menuStripOD.LayoutItems();
			Invalidate();
		}

		public void TranslateMenuItems(string classType) {
			for(int i=0;i<_menuStripOD.Items.Count;i++) {
				Lan.TranslateToolStripMenuItems(classType,(MenuItemOD)_menuStripOD.Items[i]);
			}
		}
		#endregion Methods - Public

		#region Methods - Event Handlers
		protected override void OnPaint(PaintEventArgs e){
			base.OnPaint(e);
			if(DesignMode){
				using Brush brush= new SolidBrush(BackColor);
				e.Graphics.FillRectangle(brush,ClientRectangle);
				e.Graphics.DrawString(Name,Font,Brushes.Black,10,5);
				e.Graphics.DrawRectangle(Pens.SlateGray,0,0,Width-1,Height-1);
			}
			else{
				//e.Graphics.FillRectangle(Brushes.Aquamarine,this.Bounds);
				e.Graphics.DrawLine(Pens.SlateGray,0,Height-1,Width,Height-1);//line to right of menu
			}
		}

		protected override void OnSizeChanged(EventArgs e){
			base.OnSizeChanged(e);
			_menuStripOD.Height=this.Height-1;
		}

		protected override void OnFontChanged(EventArgs e){
			base.OnFontChanged(e);
			_menuStripOD.Font=Font;
		}
		#endregion Methods - Event Handlers
	}

	///<summary>This is only used from inside MenuOD.</summary>
	public class MenuStripOD:MenuStrip{
		#region Fields
		public bool IsUpdating;
		///<summary>Just holds the scaling factor.</summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields

		#region Constructor
		public MenuStripOD(){
			
		
		}
		#endregion Constructor

		#region Methods - Public Static
		///<summary>Recursive. Will return null if not attached to a menu yet.</summary>
		public static MenuStripOD GetMenuStripOD(ToolStripItem toolStripItem){
			if(toolStripItem.Owner==null){//not attached to any menu yet.
				return null;
			}
			if(toolStripItem.OwnerItem==null){	
				return (MenuStripOD)toolStripItem.Owner;
			}
			return GetMenuStripOD(toolStripItem.OwnerItem);
		}
		#endregion Methods - Public Static

		#region Methods - Public
		public void LayoutItems(){
			if(IsUpdating){
				return;
			}
			if(LayoutManager==null){
				LayoutManager=new LayoutManagerForms();
			}
			//Font=new Font("Segoe UI",LayoutManager.ScaleF(9));
				//"Microsoft Sans Serif",Dpi.ScaleF(this,8.25f));//
			int xpos=0;
			Graphics g=this.CreateGraphics();
			//The MS control is incapable of figuring out where its buttons belong after a resize, so we do it for them.
			for(int i=0;i<Items.Count;i++){
				if(!Items[i].Available){
					((MenuItemOD)Items[i]).SetRectBounds(new Rectangle(xpos,0,0,0));
					continue;
				}
				int width=(int)g.MeasureString(Items[i].Text,Font).Width+10;
				int height=LayoutManager.Scale(19);//this number does not affect dropdowns, which is handled below.
				//also, the space this gives us on the screen for text is smaller, so this seems to include padding and border.
				((MenuItemOD)Items[i]).SetRectBounds(new Rectangle(xpos,0,width,height));
				xpos+=Items[i].Width;
				LayoutDropdown((ToolStripDropDownItem)Items[i],g);
			}
			g.Dispose();
		}
		#endregion Methods - Public

		#region Methods - Event Handlers
		protected override void OnFontChanged(EventArgs e){
			base.OnFontChanged(e);
			LayoutItems();
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e){
			base.OnPaint(e);
			e.Graphics.DrawLine(Pens.SlateGray,0,Height-1,Width,Height-1);//line under menu. Looks good at 96dpi, but creates a double line when scaled.  No simple solution.
		}

		protected override void OnResize(EventArgs e){
			base.OnResize(e);
			LayoutItems();
			Invalidate();
		}
		#endregion Methods - Event Handlers
		
		#region Methods - Private
		///<summary>Recursive</summary>
		private void LayoutDropdown(ToolStripDropDownItem toolStripDropDownItem,Graphics g){
			if(toolStripDropDownItem.DropDownItems.Count==0){
				return;
			}
			int widthDrop=0;//We want all the widths to be the same.
			int widthMax=300;
			for(int d=0;d<toolStripDropDownItem.DropDownItems.Count;d++){
				int widthText=(int)(g.MeasureString(toolStripDropDownItem.DropDownItems[d].Text,Font).Width);
				if(widthText>widthMax){
					widthDrop=widthMax;
					break;
				}
				if(widthText>widthDrop){
					widthDrop=widthText;
				}
			}
			widthDrop+=LayoutManager.Scale(85);//width includes the bar at the left plus dropdown arrows at right plus space at right for shortcut descriptions.
			//This is not consistent across different dpis. Revisit.
			int heightDrop=0;
			int heightText=LayoutManager.Scale(20);//system is 22
			Size sizeProposed=new Size(widthDrop,0);
			for(int d=0;d<toolStripDropDownItem.DropDownItems.Count;d++){
				if(!toolStripDropDownItem.DropDownItems[d].Available){
					continue;
				}
				if(toolStripDropDownItem.DropDownItems[d].GetType()==typeof(ToolStripSeparator)){
					toolStripDropDownItem.DropDownItems[d].Height=LayoutManager.Scale(6);//system is 6
					heightDrop+=LayoutManager.Scale(6);
					continue;
				}
				//heightText=(int)(g.MeasureString(menuItemOD.DropDownItems[d].Text,Font,widthDrop).Height);
				//if(heightText<heightMin){
				//	heightText=heightMin;
				//}
				toolStripDropDownItem.DropDownItems[d].Height=heightText;
				heightDrop+=heightText;
				toolStripDropDownItem.DropDownItems[d].Width=widthDrop;
			}
			toolStripDropDownItem.DropDown.AutoSize=false;
			toolStripDropDownItem.DropDown.Size=new Size(widthDrop+2,heightDrop+5);
			for(int d=0;d<toolStripDropDownItem.DropDownItems.Count;d++){
				if(toolStripDropDownItem.DropDownItems[d].GetType()==typeof(ToolStripSeparator)){
					continue;
				}
				LayoutDropdown((ToolStripDropDownItem)toolStripDropDownItem.DropDownItems[d],g);//recursive
			}
		}
		#endregion Methods - Private
	}

	public class MenuItemOD:ToolStripMenuItem{
		#region Constructors
		public MenuItemOD(){
			AutoSize=false;
			//Margin=new Padding(0);//this didn't change anything
			//Padding=new Padding(0);//this didn't change anything
			//MouseHover += (obj, arg) => ((ToolStripDropDownItem)obj).ShowDropDown();
		}

		public MenuItemOD(string text){
			Text=text;
			AutoSize=false;
			//MouseHover += (obj, arg) => ((ToolStripDropDownItem)obj).ShowDropDown();
		}

		public MenuItemOD(string text,EventHandler click){
			Text=text;
			Click+=click;
			AutoSize=false;
			//MouseHover += (obj, arg) => ((ToolStripDropDownItem)obj).ShowDropDown();
			//Name is useless
			//Size gets calculated, so it's ignored
		}
		#endregion Constructors

		#region Methods - Public
		///<summary>Add either a MenuItemOD or a MenuDropItemOD.</summary>
		public void Add(ToolStripItem toolStripItem){
			DropDownItems.Add(toolStripItem);
			MenuStripOD menuStripOD=MenuStripOD.GetMenuStripOD(this);
			if(menuStripOD!=null){
				menuStripOD.LayoutItems();
			}
		}

		///<summary>Add a MenuItemOD.</summary>
		public void Add(string text,EventHandler click){
			DropDownItems.Add(new MenuItemOD(text,click));
			MenuStripOD menuStripOD=MenuStripOD.GetMenuStripOD(this);
			if(menuStripOD!=null){
				menuStripOD.LayoutItems();
			}
		}

		public void AddSeparator(){
			DropDownItems.Add(new ToolStripSeparator());
			MenuStripOD menuStripOD=MenuStripOD.GetMenuStripOD(this);
			if(menuStripOD!=null){
				menuStripOD.LayoutItems();
			}
		}

		public void SetRectBounds(Rectangle rect){
			SetBounds(rect);
		}
		#endregion Methods - Public

		#region Methods - Event Handlers
		protected override void OnAvailableChanged(EventArgs e){
			base.OnAvailableChanged(e);
			MenuStripOD menuStripOD=MenuStripOD.GetMenuStripOD(this);
			if(menuStripOD!=null){
				menuStripOD.LayoutItems();
			}
		}

		/*
		protected override void OnPaint(PaintEventArgs e){
			//base.OnPaint(e);
			//if(Owner==null){//not attached to a ToolStrip yet.
			//	return;
			//}
			ToolStripItemRenderEventArgs ea=new ToolStripItemRenderEventArgs(e.Graphics,this);
			Owner.Renderer.DrawMenuItemBackground(ea);
			//no image
			//this is what would be needed for text wrap, button outlines, etc., but it would take a lot of time to flesh it out.  Would need to calc rect, etc.
			ToolStripItemTextRenderEventArgs rea=new ToolStripItemTextRenderEventArgs(e.Graphics,this,Text,ContentRectangle,ForeColor,Font,TextFormatFlags.Left);
			Owner.Renderer.DrawItemText(rea);

			Owner.Renderer.DrawDropDownButtonBackground
		}*/
		#endregion Methods - Event Handlers

		#region Methods - Private
		protected override void SetBounds(Rectangle rect){//No way to set bounds from outside without an override.
			base.SetBounds(rect);
		}
		#endregion Methods - Private
	}

	/// <summary>This custom renderer is based on Professional to give us a nice modern look.  Then, we could also tweak a lot of drawing here if we had time.</summary>
	internal class MenuRenderer:ToolStripProfessionalRenderer{
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e){
			//ToolStripItemTextRenderEventArgs eNew=new ToolStripItemTextRenderEventArgs(
			//	e.Graphics,e.Item,e.Text,e.TextRectangle,Color.Red,e.TextFont,e.TextFormat);
			base.OnRenderItemText(e);
			//or, we could draw the whole thing ourselves here
			/*
			MenuItemOD menuItemOD=e.Item as MenuItemOD;
			if(menuItemOD==null){
				return;
			}
			Graphics g=e.Graphics;//no dispose ref
			g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
			SolidBrush brushText=new SolidBrush(e.TextColor);
			StringFormat stringFormat=new StringFormat();
			//stringFormat.FormatFlags=StringFormatFlags.NoWrap;
			stringFormat.LineAlignment=StringAlignment.Center;
			g.DrawString(e.Text,e.TextFont,brushText,e.TextRectangle,stringFormat);
			brushText?.Dispose();
			stringFormat?.Dispose();*/
		}

		//protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e){
			//ToolStripItemRenderEventArgs eNew=new ToolStripItemRenderEventArgs(
			//base.OnRenderMenuItemBackground(e);
			//e.Graphics.FillRectangle(Brushes.Green,e.Item.ContentRectangle);//content rectangle is very small
		//}

		//protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e){
			//ToolStripRenderEventArgs eNew=new ToolStripRenderEventArgs(
			//base.OnRenderToolStripBorder(e);
			//This worked identical to the way we do it now, in OnPaint
			//e.Graphics.DrawLine(Pens.Black,0,e.ToolStrip.Height-1,e.ToolStrip.Width,e.ToolStrip.Height-1);
		//}


	}

}

#region Notes, Ignore
//MS used a LOT of classes to build menustrips, etc.  Hard to keep track of all the inheritance and objects.
//MenuItemOD is a ToolStripMenuItem/ToolStripDropDownItem/ToolStripItem
//MenuStripOD is a MenuStrip/ToolStrip
//MenuItemOD are attached to a ToolStripDropDownItem.DropDown, which is a ToolStripDropDownMenu/ToolStripDropDown/ToolStrip
//ToolStripItem.Owner is a ToolStrip
//ToolStripItem.Parent is a ToolStrip (which could be in the overflow area)
//ToolStripItem.OwnerItem is a ToolStripItem (this jumps up the tree faster, skipping the dropdown)
//ToolStripItem class has a good OnPaint example for custom rendering

//https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-add-enhancements-to-toolstripmenuitems

#endregion Notes, Ignore

//Boilerplate for adding menu items
/*
		private void LayoutMenu(){//typically called in Load()
			menuMain.BeginUpdate();//only for huge menu
			//File-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("File",menuItemFile_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemReports=new MenuItemOD("Reports");//Use a local variable if there are children
			menuMain.Add(menuItemReports);
			menuItemReports.Add("Daily",menuItemDaily_Click);//Normal simple pattern when there are no children
			MenuItemOD menuItemWeekly=new MenuItemOD("Weekly",menuItemWeekly_Click);//Also use a local variable if you want to set more properties.
			menuItemWeekly.Checked=true;
			menuItemReports.Add(menuItemWeekly);
			menuItemReports.AddSeparator();
			_menuItemMonthly=new MenuItemOD("Monthly",menuItemMonthly_Click);//Use class field when you need access later, for example to set Available=false
			menuItemReports.Add(_menuItemMonthly);
			//Help-----------------------------------------------------------------------------------------------------------
			menuMain.Add("Help",menuItemHelp_Click);
			menuMain.EndUpdate();//only if used BeginUpdate
		}
*/
