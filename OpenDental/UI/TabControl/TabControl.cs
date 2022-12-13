using OpenDental.UI.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace OpenDental.UI {
	///<summary>This is meant to replace all existing MS tab controls. </summary>
	//Jordan is the only one allowed to edit this file.
	[Designer(typeof(Design.TabControlDesigner))]//,typeof(IRootDesigner))]
	public class TabControl:UserControl {
		//There are a number of reasons we need this: MS has poor support for scaling, it's always hard to see which tab is selected because there's not enough constrast, and the MS version never lays out the hidden tabs for historical performance reasons, and this causes layout bugs.
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>These rectangles are of the unselected tabs.  The selected tab will be a little bigger.</summary>
		public List<Rectangle> ListRectanglesTabs=new List<Rectangle>();
		#endregion Fields - Public

		#region Fields - Private
		private bool _alignmentIsBottom;
		///<summary>This is the actual height, already scaled by the LayoutManager.</summary>
		private int _heightTabs;
		private int _idxHover=-1;
		private bool _ignoreTabPageCollectChanges;
		//<summary>This is for design time selection.</summary>
		private ISelectionService _iSelectionService;
		private int _paddingTabPages=2;
		private int _selectedIndex;
		private ObservableCollection<TabPage> _tabPageCollection;
		private bool _tabsAreCollapsed;
		#endregion Fields - Private

		#region Constructor
		public TabControl(){
			InitializeComponent();
			DoubleBuffered=true;
		}
		#endregion Constructor

		#region InitializeComponent
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// TabControl2
			// 
			this.Name = "TabControl2";
			this.ResumeLayout(false);

		}

		//This doesn't seem to be necessary. Handled by base class?
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion InitializeComponent

		#region Events - Public Raise
		///<summary>Occurs when user selects a tab. Will fire only when SelectedIndex actually changes.  If it must fire on click, use Selecting.</summary>
		[Category("OD")]
		[Description("Occurs when user selects a tab. Will fire only when SelectedIndex actually changes.  If it must fire on click, use Selecting.")]
		public event EventHandler Selected;

		///<summary>This is sometimes used to detect user input, but Selected is better.</summary>
		[Category("OD")]
		[Description("This is sometimes used to detect user input, but Selected is better.")]
		public event EventHandler SelectedIndexChanged;

		///<summary>A little different than the MS version.  This doesn't let you cancel and it also fires even if the tab is already selected. Always valid.  Gives you the int clicked on.  Even fires for tabs set to !Enabled.  Fires prior to SelectedIndex changing so that you can compare if they clicked on a new tab or not, but watch out! This means that SelectedIndex is not yet valid.</summary>
		[Category("OD")]
		[Description("A little different than the MS version.  This doesn't let you cancel and it also fires even if the tab is already selected. Always valid.  Gives you the int clicked on.  Even fires for tabs set to !Enabled.  Fires prior to SelectedIndex changing so that you can compare if they clicked on a new tab or not, but watch out! This means that SelectedIndex is not yet valid.")]
		public event EventHandler<int> Selecting;
		#endregion Events - Public Raise

		#region Properties
		///<summary>If true, then the tabs show at the bottom of the TabControl instead of the top.</summary>
		[Category("OD")]
		[Description("If true, then the tabs show at the bottom of the TabControl instead of the top.")]
		[DefaultValue(false)]
		public bool AlignmentIsBottom{
			get=>_alignmentIsBottom;
			set{
				_alignmentIsBottom=value;
				LayoutTabs();
				Invalidate();
			}
		}

		/*Not needed for anything yet.
		///<summary>This lets you set whether the tabs at the top will respond to clicks.  If not, you can handle them yourself with the TabClicked event.</summary>
		[Category("OD")]
		[Description("This lets you set whether the tabs at the top will respond to clicks.  If not, you can handle them yourself with the TabClicked event.")]
		[DefaultValue(true)]
		public bool RespondsToClicks{get;set; }=true;*/

		///<summary>Unlike in a MS tabControl, this will give you the size of each tabPage (they are all the same). The alternative is to get the size of any one random tab, which also works.</summary>
		[Browsable(false)]
		public new Size ClientSize{
			get{
				LayoutTabs();
				int height=Height-_heightTabs-5;
				if(height<0){//when tabControl is only as high as the tabs and doesn't show any actual pages
					height=0;
				}
				return new Size(Width-8,height);
			}
			set{
				//do nothing
			}
		}

		//Enabled does work because it inherits from Panel, and that flows down to all children, which we explicitly handle.

		///<summary>The padding around the TabPages, not the tab buttons. Default is 2. Setting to 0 will eliminate/cover the border.</summary>
		[Category("OD")]
		[Description("The padding around the TabPages, not the tab buttons. Default is 2. Setting to 0 will eliminate/cover the border.")]
		[DefaultValue(2)]
		public int PaddingTabPages{
			get{
				return _paddingTabPages;
			}
			set{
				_paddingTabPages=value;
				LayoutTabs();
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex{
			get{
				return _selectedIndex;
			}
			set{
				if(value==_selectedIndex){//no change
					return;
				}
				int orig=_selectedIndex;
				if(value!=-1 && !TabPages[value].Enabled){
					return;
				}
				_selectedIndex=value;
				LayoutTabs();
				Invalidate();
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UI.TabPage SelectedTab{
			get{
				if(_selectedIndex==-1){
					return null;
				}
				return TabPages[_selectedIndex];
			}
			set{
				int idx=TabPages.IndexOf(value);
				if(idx==-1){
					return;
				}
				SelectedIndex=idx;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MergableProperty(false)]
		[Category("Behavior")]
		public ObservableCollection<TabPage> TabPages{
			get{
				//we refill our collection each time in case it gets out of synch.
				//Example: This gets hit before TabPages are even added to control. It would remain empty if we didn't refresh.
				if(_tabPageCollection==null){
					_tabPageCollection=new ObservableCollection<TabPage>();
					_tabPageCollection.CollectionChanged+=_tabPageCollection_CollectionChanged;
				}
				_ignoreTabPageCollectChanges=true;
				_tabPageCollection.Clear();
				for(int i=0;i<Controls.Count;i++){
					if(Controls[i] is TabPage tabPage){
						_tabPageCollection.Add(tabPage);
					}
				}
				_ignoreTabPageCollectChanges=false;
				return _tabPageCollection;
			}
		}

		///<summary>Set true to collapse the tabs area. The TabPages will then fill the entire TabControl.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TabsAreCollapsed{
			get{
				return _tabsAreCollapsed;
			}
			set{
				_tabsAreCollapsed=value;
				LayoutTabs();
				Invalidate();
			}
		}
		#endregion Properties

		#region Methods - Event Handlers
		protected override void OnLayout(LayoutEventArgs e) {
			base.OnLayout(e);
			LayoutTabs();
		}

		private void _tabPageCollection_CollectionChanged(object sender,System.Collections.Specialized.NotifyCollectionChangedEventArgs e){
			if(_ignoreTabPageCollectChanges){
				//during each time we fill the collection
				return;
			}
			//These notification don't include property changes of each tabPage, but we don't care about that.
			//We need to keep the controls synched with the changes to the TabPages collection.
			//First, handle tabPages that were added to the collection:
			for(int i=0;i<_tabPageCollection.Count;i++){
				if(!Controls.Contains(_tabPageCollection[i])){
					Controls.Add(_tabPageCollection[i]);
					continue;
				}
			}
			//Deletions
			//_tabPageCollection changes/refreshes from Controls each time it's referenced, so we can't use it.
			//Use a copy
			List<TabPage> listTabPages=new List<TabPage>();
			listTabPages.AddRange(_tabPageCollection);
			for(int i=Controls.Count-1;i>=0;i--){//go backwards
				if(!(Controls[i] is TabPage)){
					continue;
				}
				if(listTabPages.Contains(Controls[i])){
					continue;//then we are good
				}
				//we found a control that's not in the collection
				Controls.RemoveAt(i);
			}
			LayoutTabs();
			Invalidate();
		}
		#endregion Methods - Event Handlers

		#region Methods - Paint
		protected override void OnPaint(PaintEventArgs e) {
			if(Parent is null){
				return;
			}
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			Color colorBack=Parent.BackColor;
			g.Clear(colorBack);
			if(ListRectanglesTabs.Count!=TabPages.Count){
				return;
			}
			using Pen pen=new Pen(ColorOD.Gray(192));//192 is silver, and is the same value as used in groupBoxOD
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;
			stringFormat.LineAlignment=StringAlignment.Center;
			for(int i=0;i<ListRectanglesTabs.Count;i++){
				if(TabsAreCollapsed){
					break;;
				}
				if(SelectedIndex==i){
					continue;//this is done in a separate loop
				}
				GraphicsPath graphicsPath=GetPathTab(ListRectanglesTabs[i]);
				if(TabPages[i].ColorTab.ToArgb()!=Color.Empty.ToArgb()){
					using SolidBrush solidBrushCustom=new SolidBrush(TabPages[i].ColorTab);
					g.FillPath(solidBrushCustom,graphicsPath);
				}
				else if(!TabPages[i].Enabled){
					using SolidBrush solidBrushUnselected=new SolidBrush(ColorOD.Gray(245));
					g.FillPath(solidBrushUnselected,graphicsPath);
				}
				else if(_idxHover==i && !DesignMode){//hover effect only works in run time
					using SolidBrush solidHover=new SolidBrush(ColorOD.Hover);
					g.FillPath(solidHover,graphicsPath);
				}
				else{
					using SolidBrush solidBrushUnselected=new SolidBrush(ColorOD.Gray(245));
					g.FillPath(solidBrushUnselected,graphicsPath);
				}
				g.DrawPath(pen,graphicsPath);
				if(TabPages[i].Enabled){
					g.DrawString(TabPages[i].Text,Font,Brushes.Black,ListRectanglesTabs[i],stringFormat);
				}
				else{
					using SolidBrush solidBrushDisabled=new SolidBrush(ColorOD.Gray(170));
					g.DrawString(TabPages[i].Text,Font,solidBrushDisabled,ListRectanglesTabs[i],stringFormat);
				}
			}
			if(AlignmentIsBottom){
				g.DrawLine(pen,0,0,0,Height-_heightTabs-1);//left
				g.DrawLine(pen,0,Height-_heightTabs,Width-1,Height-_heightTabs);//bottom
				g.DrawLine(pen,Width-1,Height-_heightTabs-1,Width-1,0);//right
				g.DrawLine(pen,0,0,Width-1,0);//top
			}
			else{
				g.DrawLine(pen,0,_heightTabs-1,0,Height-1);//left
				g.DrawLine(pen,0,Height-1,Width-1,Height-1);//bottom
				g.DrawLine(pen,Width-1,Height-1,Width-1,_heightTabs-1);//right
				g.DrawLine(pen,0,_heightTabs-1,Width-1,_heightTabs-1);//top
			}
			if(TabsAreCollapsed){
				return;
			}
			//then the selected tab
			for(int i=0;i<ListRectanglesTabs.Count;i++){
				if(SelectedIndex!=i){
					continue;
				}
				Rectangle rectangleSelected;
				if(AlignmentIsBottom){
					rectangleSelected=new Rectangle(
						x:ListRectanglesTabs[i].Left-2,
						y:ListRectanglesTabs[i].Top,
						width:ListRectanglesTabs[i].Width+4,
						height:ListRectanglesTabs[i].Height+2);
				}
				else{
					rectangleSelected=new Rectangle(
						x:ListRectanglesTabs[i].Left-2,
						y:ListRectanglesTabs[i].Top-2,
						width:ListRectanglesTabs[i].Width+4,
						height:ListRectanglesTabs[i].Height+2);
				}
				GraphicsPath graphicsPath=GetPathTab(rectangleSelected);
				if(TabPages[i].ColorTab.ToArgb()==Color.Empty.ToArgb()){
					using SolidBrush solidBrushSelected=new SolidBrush(Color.FromArgb(170,190,230));
					g.FillPath(solidBrushSelected,graphicsPath);
				}
				else{
					using SolidBrush solidBrushCustom=new SolidBrush(TabPages[i].ColorTab);
					g.FillPath(solidBrushCustom,graphicsPath);
				}
				g.DrawPath(pen,graphicsPath);
				g.DrawString(TabPages[i].Text,Font,Brushes.Black,rectangleSelected,stringFormat);
			}
			stringFormat?.Dispose();
		}

		///<summary>Converts a rectangle into one with rounded top corners.</summary>
		private GraphicsPath GetPathTab(Rectangle rectangle){
			GraphicsPath graphicsPath=new GraphicsPath();
			int radius=3;
			if(AlignmentIsBottom){
				//start at UL and draw clockwise
				graphicsPath.AddLine(rectangle.Left,rectangle.Top,rectangle.Right,rectangle.Top);//top
				graphicsPath.AddLine(rectangle.Right,rectangle.Top,rectangle.Right,rectangle.Bottom-radius);//right
				graphicsPath.AddArc(rectangle.Right-radius*2,rectangle.Bottom-radius*2,radius*2,radius*2,0,90);//LR
				graphicsPath.AddLine(rectangle.Right-radius,rectangle.Bottom,rectangle.Left+radius,rectangle.Bottom);//bottom
				graphicsPath.AddArc(rectangle.Left,rectangle.Bottom-radius*2,radius*2,radius*2,90,90);//LL
				graphicsPath.AddLine(rectangle.Left,rectangle.Bottom-radius,rectangle.Left,rectangle.Top);//left
				return graphicsPath;
			}
			//start at LL and draw clockwise
			graphicsPath.AddLine(rectangle.Left,rectangle.Bottom,rectangle.Left,rectangle.Top+radius);//left
			graphicsPath.AddArc(rectangle.Left,rectangle.Top,radius*2,radius*2,180,90);//UL
			graphicsPath.AddLine(rectangle.Left+radius,rectangle.Top,rectangle.Right-radius,rectangle.Top);//top
			graphicsPath.AddArc(rectangle.Right-radius*2,rectangle.Top,radius*2,radius*2,270,90);//UR
			graphicsPath.AddLine(rectangle.Right,rectangle.Top+radius,rectangle.Right,rectangle.Bottom);//right
			graphicsPath.AddLine(rectangle.Right,rectangle.Bottom,rectangle.Left,rectangle.Bottom);//bottom
			return graphicsPath;
		}
		#endregion Methods - Paint

		#region Methods - Mouse
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			for(int i=0;i<ListRectanglesTabs.Count;i++){
				if(!ListRectanglesTabs[i].Contains(e.Location)){
					continue;
				}
				Selecting?.Invoke(this,i);
				if(!TabPages[i].Enabled){
					break;//also blocked when trying to change SelectedIndex
				}
				if(SelectedIndex!=i){
					Selected?.Invoke(this,new EventArgs());
				}
				SelectedIndex=i;
				break;
			}
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			_idxHover=-1;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			_idxHover=-1;
			Invalidate();
			//make sure it's not in the selected tab, because we will ignore that
			for(int i=0;i<ListRectanglesTabs.Count;i++){
				if(i!=SelectedIndex){
					continue;
				}
				Rectangle rectangleSelected=new Rectangle(
					x:ListRectanglesTabs[i].Left-2,
					y:ListRectanglesTabs[i].Top-2,
					width:ListRectanglesTabs[i].Width+4,
					height:ListRectanglesTabs[i].Height+2);
				if(rectangleSelected.Contains(e.Location)){
					return;
				}
			}
			for(int i=0;i<ListRectanglesTabs.Count;i++){
				if(i==SelectedIndex){
					continue;
				}
				if(ListRectanglesTabs[i].Contains(e.Location)){
					_idxHover=i;
					break;
				}
			}
		}
		#endregion Methods - Mouse

		#region Methods - Public
		public Rectangle GetTabRect(int idx){
			if(idx<0 || idx>ListRectanglesTabs.Count-1){
				return new Rectangle();
			}
			return ListRectanglesTabs[idx];
		}

		public override ISite Site {
			get {
				return base.Site;
			}
			set {
				base.Site = value;
				if(base.Site==null){
					return;
				}
				_iSelectionService = (ISelectionService)this.Site.GetService(typeof(ISelectionService));
			}
		}

		public void LayoutTabs(){
			//In each of the places below where SelectedIndex gets changed, 
			//this will trigger another LayoutTabs to immediately run.
			//This will result in the lower section being run twice, which is harmless.
			if(TabPages.Count==0){
				ListRectanglesTabs=new List<Rectangle>();
				SelectedIndex=-1;
				return;
			}
			//There is at least one tabPage
			if(SelectedIndex==-1){
				SelectedIndex=0;
			}
			if(SelectedIndex>TabPages.Count-1){
				//This fixes the situation where we just deleted the last tab, causing SelectedIndex to be invalid.
				//It also fixes the situation where someone set an invalid SelectedIndex.
				SelectedIndex=TabPages.Count-1;
			}
			ListRectanglesTabs=new List<Rectangle>();
			SuspendLayout();//to prevent this section from triggering Layout to fire again, which would call this method again.
			using Graphics g=CreateGraphics();
			_heightTabs=LayoutManager.Scale(20);//this is one pixel shorter than MS
			if(TabsAreCollapsed){
				_heightTabs=1;//1 instead of 0 for better drawing
			}
				//so this will cause small changes in designer files when transitioning.
			for(int i=0;i<TabPages.Count;i++){
				int widthTab=(int)g.MeasureString(TabPages[i].Text,Font).Width+LayoutManager.Scale(8);
				int x;
				if(i==0){
					x=2;
				}
				else{
					x=ListRectanglesTabs[i-1].Right;
				}
				Rectangle rectangleTab;
				if(AlignmentIsBottom){
					rectangleTab=new Rectangle(x,Height-_heightTabs,widthTab,_heightTabs-3);
				}
				else{
					rectangleTab=new Rectangle(x,2,widthTab,_heightTabs-3);
				}
				ListRectanglesTabs.Add(rectangleTab);
				int heightTabPage=Height-_heightTabs-_paddingTabPages*2+1;
				if(heightTabPage<0){//when tabControl is only as high as the tabs and doesn't show any actual pages
					heightTabPage=0;
				}
				if(AlignmentIsBottom){
					TabPages[i].Bounds=new Rectangle(_paddingTabPages,_paddingTabPages,Width-_paddingTabPages*2,heightTabPage);
				}
				else{
					TabPages[i].Bounds=new Rectangle(_paddingTabPages,_heightTabs+_paddingTabPages-1,Width-_paddingTabPages*2,heightTabPage);
				}
				if(i==SelectedIndex){
					TabPages[i].Visible=true;
					SetSelectedComponent(TabPages[i]);
				}
				else{
					TabPages[i].Visible=false;
				}
			}
			ResumeLayout();
		}
		#endregion Methods - Public

		#region Methods - Private
		/*
		private string GetSelectedComponents(){
			string selectedString = String.Empty;
			object[] objectArray = new object[_iSelectionService.GetSelectedComponents().Count];
			_iSelectionService.GetSelectedComponents().CopyTo(objectArray, 0);
			for(int i=0; i<objectArray.Length; i++){
				if(i != 0 ){
					selectedString += "&& ";
				}
				if( ((IComponent)_iSelectionService.PrimarySelection) == ((IComponent)objectArray[i]) ){
					selectedString += "PrimarySelection:";
				}
				selectedString += ((IComponent)objectArray[i]).Site.Name+" ";
			}
			return selectedString;
		}*/

		private void SetSelectedComponent(Component component){
			if(!DesignMode){
				return;
			}
			List<Component> listComponents=new List<Component>();
			listComponents.Add(component);
			_iSelectionService.SetSelectedComponents(listComponents,SelectionTypes.Replace);
		}
		#endregion Methods - Private

		

	
	}
}

/* Notes, ignore.
Extending design-time support (the bible):
https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/37899azc(v=vs.100)


Example of custom tab page collection editor
 https://stackoverflow.com/questions/45685899/c-sharp-tabcontrol-how-create-custom-tabpages-collection-editor
Example to create a design-time control:
https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/creating-a-wf-control-design-time-features?view=netframeworkdesktop-4.8
Edit and persist collections
https://www.codeproject.com/Articles/5372/How-to-Edit-and-Persist-Collections-with-Collectio
Static event in CollectionEditor
https://stackoverflow.com/questions/26406775/how-to-make-collectioneditor-to-fire-collectionchanged-event-when-items-added-or
Customize drag and drop in the designer
https://social.msdn.microsoft.com/Forums/en-US/f01ce3dd-d7d5-401d-8063-2533cb83a79e/customize-drag-amp-drop-in-the-desginer?forum=winformsdesigner


*/

