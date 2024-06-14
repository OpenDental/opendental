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
	///<summary>This is meant to replace all existing MS Splitters and SplitContainers. The best way to use it is to copy an existing blank SplitContainer from OpenDental/Forms/BasicTemplate.cs because it already has both splitter panels.</summary>
	//Jordan is the only one allowed to edit this file.
	[Designer(typeof(Design.SplitContainerDesigner))]//,typeof(IRootDesigner))]
	public class SplitContainer:UserControl {
		//There are a number of reasons we needed this:  Neither the MS Splitter nor MS SplitContainer work well with our custom layout manager. They are especially troublesome when nested or when interacting with TabControls. They also have issues with docking vs anchoring to 4 sides. They do not support high dpi layout, which is a deal breaker. The MS SplitContainer also has an ugly drag animation and is also just buggy. Completely separately from our layout manager issues, the MS SplitContainer will frequently inaccurately serialize/deserialize in the designer, causing a mangled layout. This is unacceptable.  We did not replace all of the MS splitContainers.  A few still remain in forms related to JobManager.
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		private Color _colorBorder=Color.Silver;
		private Color _colorSplitter=Color.Silver;
		private bool _isMouseDown;
		private System.Windows.Forms.Orientation _orientation=Orientation.Vertical;
		private SplitterPanel panel1;
		private SplitterPanel panel2;
		///<summary>for design time refreshing of Properties Window.</summary>
		private IComponentChangeService _iComponentChangeService;
		///<summary>for design time selection.</summary>
		private ISelectionService _iSelectionService;
		private bool _isPanel1Collapsed;
		private bool _isPanel2Collapsed;
		private int _panel1MinSize96=0;
		private int _panel2MinSize96=0;
		private Point _pointMouseDown;
		private int _splitterDistDragging;
		private int _splitterDistWhenMouseDown;
		private int _splitterDist96=50;
		private int _splitterWidth96=4;
		#endregion Fields - Private

		#region Constructor
		[RefreshProperties(RefreshProperties.All)]
		public SplitContainer(){
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			DoubleBuffered=true;
		}
		#endregion Constructor

		#region InitializeComponent
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// SplitContainer
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Name = "SplitContainer";
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
		///<summary>Occurs when the splitter is done being moved, whether it's moved programmatically or by the user.</summary>
		[Category("OD")]
		[Description("Occurs when the splitter is done being moved, whether it's moved programmatically or by the user.")]
		public event EventHandler SplitterMoved;
		#endregion Events - Public Raise

		#region Properties
		///<summary>The color of the border. Default is Silver.</summary>
		[Category("OD")]
		[Description("The color of the border. Default is Silver.")]
		[DefaultValue(typeof(Color), "Silver")]
		public Color ColorBorder { 
			get{
				return _colorBorder;
			}
			set{
				_colorBorder= value;
				Invalidate();
				panel1?.Invalidate();
				panel2?.Invalidate();
			}
		} 

		///<summary>The color of the splitter. Default is Silver.</summary>
		[Category("OD")]
		[Description("The color of the splitter. Default is Silver.")]
		[DefaultValue(typeof(Color), "Silver")]
		public Color ColorSplitter { 
			get{
				return _colorSplitter;
			}
			set{
				_colorSplitter= value;
				Invalidate();
			}
		} 

		///<summary>Determines if the Splitter can move.</summary>
		[Category("OD")]
		[Description("Determines if the Splitter can move.")]
		[DefaultValue(false)]
		public bool IsSplitterFixed{get;set; }

		///<summary>Determines if the splitter is vertical or horizontal.</summary>
		[Category("OD")]
		[Description("Determines if the splitter is vertical or horizontal.")]
		[DefaultValue(Orientation.Vertical)]
		public System.Windows.Forms.Orientation Orientation { 
			get=>_orientation; 
			set{
				_orientation=value;
				LayoutPanels();
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public SplitterPanel Panel1{
			get{
				return panel1;
			}
			set{
				panel1=value;
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public SplitterPanel Panel2{
			get{
				return panel2;
			}
			set{
				panel2=value;
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool Panel1Collapsed{
			get{
				return _isPanel1Collapsed;
			}
			set{
				if(value==true){
					if(_isPanel2Collapsed){
						_isPanel2Collapsed=false;//they can't both be collapsed
					}
				}
				_isPanel1Collapsed=value;
				LayoutPanels();
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool Panel2Collapsed{
			get{
				return _isPanel2Collapsed;
			}
			set{
				if(value==true){
					if(_isPanel1Collapsed){
						_isPanel1Collapsed=false;//they can't both be collapsed
					}
				}
				_isPanel2Collapsed=value;
				LayoutPanels();
			}
		}

		///<summary>Use current dpi actual pixels.</summary>
		[Category("OD")]
		[Description("Use current dpi actual pixels.")]
		[DefaultValue(0)]
		[RefreshProperties(RefreshProperties.All)]//so that the SplitterDistance property will change in property grid
		public int Panel1MinSize { 
			get{
				return LayoutManager.Scale(_panel1MinSize96);
			}
			set{
				_panel1MinSize96=(int)LayoutManager.UnscaleF(value);
				SplitterDistance=SplitterDistance;//this run logic to adjust splitter if it's in an unacceptable spot.
				LayoutPanels();
				Invalidate();
			}
		}

		///<summary>Use current dpi actual pixels.</summary>
		[Category("OD")]
		[Description("Use current dpi actual pixels.")]
		[DefaultValue(0)]
		[RefreshProperties(RefreshProperties.All)]
		public int Panel2MinSize { 
			get{
				return LayoutManager.Scale(_panel2MinSize96);
			}
			set{
				_panel2MinSize96=(int)LayoutManager.UnscaleF(value);
				SplitterDistance=SplitterDistance;
				LayoutPanels();
				Invalidate();
			}
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
				_iComponentChangeService = (IComponentChangeService)this.Site.GetService(typeof(IComponentChangeService));
				_iSelectionService = (ISelectionService)this.Site.GetService(typeof(ISelectionService));
			}
		}
		
		///<summary>Use current dpi actual pixels.</summary>
		[Category("OD")]
		[Description("Use current dpi actual pixels.")]
		[DefaultValue(50)]
		//[RefreshProperties(RefreshProperties.All)]//this only fires when you change in Properties Window, so not useful here.
		public int SplitterDistance { 
			get{
				if(_isMouseDown){
					//then we want to work with real screen coords, not conversion from 96dpi
					return _splitterDistDragging;
				}
				int splitterDist=LayoutManager.Scale(_splitterDist96);
				return splitterDist;
			}
			set{
				//this happens on mouse up, but not during mouse drag
				int splitterDist=value;
				int panel1MinSize=Panel1MinSize;//scaled
				int panel2MinSize=Panel2MinSize;
				int splitterWidth=SplitterWidth;
				if(splitterDist<panel1MinSize){
					splitterDist=panel1MinSize;
				}
				if(Orientation==Orientation.Vertical){
					if(splitterDist>Width-panel2MinSize-splitterWidth){
						splitterDist=Width-panel2MinSize-splitterWidth;
					}
				}
				else{//horizontal
					if(splitterDist>Height-panel2MinSize-splitterWidth){
						splitterDist=Height-panel2MinSize-splitterWidth;
					}
				}
				int splitterDist96=(int)LayoutManager.UnscaleF(splitterDist);
				if(splitterDist96==_splitterDist96){
					return;//no change
				}
				_splitterDist96=splitterDist96;
				LayoutPanels();
				if(DesignMode){
					_iComponentChangeService.OnComponentChanged(this,TypeDescriptor.GetProperties(this)["SplitterDistance"],null,splitterDist);
					//this pushes the new value to the Properties Window, which would not otherwise refresh.
				}
				SplitterMoved?.Invoke(this,new EventArgs());
			}
		}

		///<summary>Default is 4 pixels. This does get scaled with zoom, so it would be 5 pixels at about 125% zoom.</summary>
		[Category("OD")]
		[Description("Default is 4 pixels.")]
		[DefaultValue(4)]
		public int SplitterWidth { 
			get{
				int splitterWidth=LayoutManager.Scale(_splitterWidth96);
				return splitterWidth;
			}
			set{
				_splitterWidth96=(int)LayoutManager.UnscaleF(value);
				LayoutPanels();
			}
		}
		#endregion Properties

		#region Methods - Event Handlers
		protected override void OnLayout(LayoutEventArgs e) {
			base.OnLayout(e);
			LayoutPanels();
		}
		#endregion Methods - Event Handlers

		#region Methods - Paint
		protected override void OnPaint(PaintEventArgs e) {
			Graphics g=e.Graphics;
			using SolidBrush solidBrush=new SolidBrush(_colorSplitter);
			g.FillRectangle(solidBrush,ClientRectangle);//fill the whole thing.  Only the "splitter" will peek through
			//using Pen penBorder=new Pen(_colorBorder);
			//g.DrawRectangle(penBorder,0,0,Width-1,Height-1);//so we can see it in designer if no panels.
			if(panel1!=null && panel2!=null){
				return;
			}
			string str="You must set Panel1 and/or Panel2 properties.\r\n"
				+"The best way to do this is to copy an existing blank SplitContainer from OpenDental/Forms/BasicTemplate.cs because it already has both splitter panels.";
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			g.DrawString(str,Font,Brushes.Black,ClientRectangle,stringFormat);
		}
		#endregion Methods - Paint

		#region Methods - Mouse
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			SetSelectedComponent(this);
			if(IsSplitterFixed){
				return;
			}
			_isMouseDown=true;
			_pointMouseDown=e.Location;
			_splitterDistWhenMouseDown=LayoutManager.Scale(_splitterDist96);
			if(Orientation==Orientation.Vertical){
				Cursor=Cursors.VSplit;
			}
			else{//horizontal
				Cursor=Cursors.HSplit;
			}
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			//this can't happen if mouse is down
			Cursor=Cursors.Default;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(IsSplitterFixed){
				return;//don't even change the cursor.
			}
			if(Orientation==Orientation.Vertical){
				Cursor=Cursors.VSplit;
			}
			else{//horizontal
				Cursor=Cursors.HSplit;
			}
			if(!_isMouseDown){
				return;
			}
			//mouse is down
			if(Orientation==Orientation.Vertical){
				_splitterDistDragging=_splitterDistWhenMouseDown+e.X-_pointMouseDown.X;
			}
			else{//horizontal
				_splitterDistDragging=_splitterDistWhenMouseDown+e.Y-_pointMouseDown.Y;
			}
			LayoutPanels();
			panel1.Invalidate();
			panel2.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e){
			base.OnMouseUp(e);
			if(IsSplitterFixed){
				return;
			}
			_isMouseDown=false;
			if(_pointMouseDown==e.Location){//no mouse movement
				Cursor=Cursors.Default;
				return;
			}
			SplitterDistance=_splitterDistDragging;//if this is an unacceptable value due to min sizes, then the setter will handle it.
			Cursor=Cursors.Default;
		}
		#endregion Methods - Mouse

		#region Methods - Public
		// <summary>For ISupportInitialize</summary>
		//public void BeginInit(){
			//This happens after initialization, but before suspendLayout
		//}
		
		// <summary>For ISupportInitialize</summary>
		//public void EndInit(){
			//This happens just after the panels resume layout, but just before this container resumes layout
			//panel1.SuspendLayout();//when we change panel sizes, we don't want children to move
			//panel2.SuspendLayout();
			//LayoutPanels();
			//panel1.ResumeLayout(false);
			//panel2.ResumeLayout(false);
		//}
		#endregion Methods - Public

		#region Methods - Private
		private void LayoutPanels(){
			if(panel1 is null){
				return;
			}
			if(panel2 is null){
				return;
			}
			int splitterDist=SplitterDistance;//This gets us the scaled distance that also handles dragging situations
			int panel1MinSize=Panel1MinSize;//scaled
			int panel2MinSize=Panel2MinSize;
			int splitterWidth=SplitterWidth;
			if(splitterDist<panel1MinSize){
				splitterDist=panel1MinSize;
			}
			if(Orientation==Orientation.Vertical){
				if(splitterDist>Width-panel2MinSize-splitterWidth){
					splitterDist=Width-panel2MinSize-splitterWidth;
				}
			}
			else{//horizontal
				if(splitterDist>Height-panel2MinSize-splitterWidth){
					splitterDist=Height-panel2MinSize-splitterWidth;
				}
			}
			panel1.Visible=true;
			panel2.Visible=true;
			if(_isPanel1Collapsed){
				panel1.Visible=false;
				panel2.Bounds=ClientRectangle;
			}
			else if(_isPanel2Collapsed){
				panel1.Bounds=ClientRectangle;
				panel2.Visible=false;
			}
			else{
				if(Orientation==Orientation.Vertical){
					panel1.Bounds=new Rectangle(0,0,splitterDist,Height);
					panel2.Bounds=new Rectangle(splitterDist+splitterWidth,0,Width-splitterDist-splitterWidth,Height);
				}
				else{//horizontal
					panel1.Bounds=new Rectangle(0,0,Width,splitterDist);
					panel2.Bounds=new Rectangle(0,splitterDist+splitterWidth,Width,Height-splitterDist-splitterWidth);
				}
			}
			panel1.Invalidate();
			panel2.Invalidate();
			LayoutManager.LayoutControlBoundsAndFonts(panel1);
			LayoutManager.LayoutControlBoundsAndFonts(panel2);
		}

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


