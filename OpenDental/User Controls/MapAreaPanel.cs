using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class MapAreaPanel:Panel {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Properties
		[Category("OD")]
		[Description("Turn dragging on or off")]
		public bool AllowDragging { get; set; }

		[Category("OD")]
		[Description("Turn editing on or off")]
		public bool AllowEditing { get; set; }

		private Font _fontLabel=SystemFonts.DefaultFont;
		[Category("OD")]
		[Description("Font sized used for labels")]
		public Font FontLabel {
			get {
				return _fontLabel;
			}
			set {
				_fontLabel=value;
				ResizeScrollbarsToFitContents();
				Invalidate(true);
			}
		}

		private Font _fontCubicle=SystemFonts.DefaultFont;
		[Category("OD")]
		[Description("Font sized used for individual cubicles")]
		public Font FontCubicle {
			get {
				return _fontCubicle;
			}
			set {
				_fontCubicle=value;
				ResizeScrollbarsToFitContents();
				Invalidate(true);
			}
		}

		private Font _fontCubicleHeader=SystemFonts.DefaultFont;
		[Category("OD")]
		[Description("Font sized used for first row header in the cubicle")]
		public Font FontCubicleHeader {
			get {
				return _fontCubicleHeader;
			}
			set {
				_fontCubicleHeader=value;
				ResizeScrollbarsToFitContents();
				Invalidate(true);
			}
		}

		private int _widthFloorFeet=80;
		[Category("OD")]
		[Description("Number of feet left to right")]
		[DefaultValue(80)]
		public int WidthFloorFeet {
			get {
				return _widthFloorFeet;
			}
			set {
				_widthFloorFeet=value;
				ResizeScrollbarsToFitContents();
				ResizeCubicles();
				Invalidate(true);
			}
		}

		private int _heightFloorFeet=80;
		[Category("OD")]
		[Description("Number of feet top to bottom")]
		[DefaultValue(80)]
		public int HeightFloorFeet {
			get {
				return _heightFloorFeet;
			}
			set {
				_heightFloorFeet=value;
				ResizeScrollbarsToFitContents();
				ResizeCubicles();
				Invalidate(true);
			}
		}

		private int _pixelsPerFoot=10;
		[Category("OD")]
		[Description("Number of pixels used per each foot. Change this to scale the drawing.")]
		public int PixelsPerFoot {
			get {
				return _pixelsPerFoot;
			}
			set {
				_pixelsPerFoot=value;
				ResizeScrollbarsToFitContents();
				ResizeCubicles();
				Invalidate(true);
			}
		}

		private bool _showGrid=false;
		[Category("OD")]
		[Description("Draws the overlay grid underneath the cubicles")]
		public bool ShowGrid {
			get {
				return _showGrid;
			}
			set {
				_showGrid=value;
				Invalidate();
			}
		}

		private bool _showOutline=false;
		[Category("OD")]
		[Description("Draws outline around the control")]
		public bool ShowOutline {
			get {
				return _showOutline;
			}
			set {
				_showOutline=value;
				Invalidate();
			}
		}

		private Color _colorGrid=Color.DarkGray;
		[Category("OD")]
		[Description("Color used to draw the grid lines")]
		public Color ColorGrid {
			get {
				return _colorGrid;
			}
			set {
				_colorGrid=value;
				Invalidate();
			}
		}

		private Color _colorFloor=Color.White;
		[Category("OD")]
		[Description("Color used to draw the floor")]
		public Color ColorFloor {
			get {
				return _colorFloor;
			}
			set {
				_colorFloor=value;
				//This is effective the BackColor of this panel so set child controls BackColor to match.
				for(int i=0;i<this.Controls.Count;i++) {
					this.Controls[i].BackColor=_colorFloor;
				}
				Invalidate(true);
			}
		}

		#endregion Properties

		#region Events - Raise

		public event EventHandler MapCubicleEdited;
		public event EventHandler MapCubicleClicked;
		///<summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler ClickedGoTo=null;

		#endregion Events - Raise

		#region Used for testing.

		private static Random _random = new Random();

		public int GetRandomDimension() {
			return _random.Next(0,2)==1?6:9;
		}

		public int GetRandomXPos(int cubicleWidth) {
			return _random.Next(0,WidthFloorFeet-cubicleWidth);
		}

		public int GetRandomYPos(int cubicleHeight) {
			return _random.Next(0,HeightFloorFeet-cubicleHeight);
		}

		public MapAreaPanel() {
			InitializeComponent();
			//prevent flickering
			this.DoubleBuffered=true;
		}

		#endregion

		#region Methods - Event Handlers
		protected override void OnClick(EventArgs e) {
			// remove this ? or should something be happening here ?
		}

		private void mapCubicle_Clicked(object sender,EventArgs e) {
			MapCubicleClicked?.Invoke(sender,new EventArgs());
		}

		///<summary>Alert parent that something has changed</summary>
		private void mapCubicle_ClickedGoTo(object sender,EventArgs e) {
			ClickedGoTo?.Invoke(sender,new EventArgs());
		}

		///<summary>Alert parent that something has changed</summary>
		private void mapCubicle_Edited(object sender,EventArgs e) {
			MapCubicleEdited?.Invoke(sender,new EventArgs());
		}

		///<summary>Handle the Cubicle.DragDone event</summary>
		void mapCubicle_DragDone(object sender,EventArgs e) {
			if(sender==null) {
				return;
			}
			Control control=null;
			MapArea mapAreaClinic=null;
			if(sender is MapCubicle) {
				control=(Control)sender;
				mapAreaClinic=((MapCubicle)sender).MapAreaCur;
			}
			else if(sender is MapLabel) {
				control=(Control)sender;
				mapAreaClinic=((MapLabel)sender).MapAreaCur;
			}
			else {
				return;
			}
			//recalculate XPos and YPos based on new location in the panel
			PointF pointF=ConvertScreenLocationToXY(control.Location,PixelsPerFoot);
			mapAreaClinic.XPos=Math.Round(pointF.X,3);
			mapAreaClinic.YPos=Math.Round(pointF.Y,3);
			//save new cubicle location to db
			MapAreas.Update(mapAreaClinic);
			//alert the parent
			mapCubicle_Edited(sender,new EventArgs());
		}
		#endregion Methods - Event Handlers

		#region Methods
		///<summary>Clear the form. Optionally delete the records from the database. Use this option sparingly (if ever).</summary>
		public void Clear(bool deleteFromDatabase) {
			if(deleteFromDatabase) {
				for(int i=0;i<this.Controls.Count;i++) {
					if(!(this.Controls[i] is MapCubicle)) {
						return;
					}
					MapAreas.Delete(((MapCubicle)this.Controls[i]).MapAreaCur.MapAreaNum);
				}			
			}
			for(int i=0;i<this.Controls.Count;i++) {
				ODException.SwallowAnyException(() => {
					this.Controls[i].Dispose();
				});
			}
			this.Controls.Clear();
		}

		///<summary>For testing only. Create a random cubicle and add it to the panel.</summary>
		public MapArea AddRandomCubicle() {
			MapArea mapArea=new MapArea();
			mapArea.MapAreaNum=_random.Next(1,1000000);
			mapArea.Description="";
			mapArea.Extension=_random.Next(100,200);
			mapArea.Width=GetRandomDimension();
			mapArea.Height=GetRandomDimension();
			mapArea.XPos=GetRandomXPos((int)mapArea.Width);
			mapArea.YPos=GetRandomYPos((int)mapArea.Height);
			AddCubicle(mapArea);
			return mapArea;
		}

		///<summary>Add a cubicle to the panel.</summary>
		public void AddCubicle(MapArea mapArea,bool allowRightClickOptions=true) {
			mapArea.ItemType=MapItemType.Cubicle;
			MapCubicle mapCubicle=new MapCubicle();
			mapCubicle.MapAreaCur=mapArea;
			mapCubicle.TimeSpanElapsed = TimeSpan.FromSeconds(_random.Next(60,1200));
			mapCubicle.EmployeeName = "Emp: "+this.Controls.Count.ToString();
			mapCubicle.EmployeeNum = this.Controls.Count;
			mapCubicle.Extension = mapArea.Extension.ToString();
			mapCubicle.Status = "Status";
			mapCubicle.Font = this.FontCubicle;//these two fonts seem to already scale with the control somehow
			mapCubicle.FontHeader=this.FontCubicleHeader;
			mapCubicle.Location = GetScreenLocation(mapArea.XPos,mapArea.YPos,this.PixelsPerFoot);
			mapCubicle.Size=GetScreenSize(mapArea.Width,mapArea.Height,this.PixelsPerFoot);
			mapCubicle.ColorInner = Color.FromArgb(40,Color.Red);
			mapCubicle.ColorOuter = Color.Red;
			mapCubicle.BackColor=this.ColorFloor;
			mapCubicle.ImagePhone = Properties.Resources.phoneInUse;
			mapCubicle.ImageChat = Properties.Resources.gtaicon3;
			mapCubicle.ImageWebChat=Properties.Resources.WebChatIcon;
			mapCubicle.ImageRemoteSupport=Properties.Resources.remoteSupportIcon;
			mapCubicle.AllowDragging=AllowDragging;
			mapCubicle.IsEditAllowed=AllowEditing;
			mapCubicle.Name=mapArea.MapAreaNum.ToString();
			mapCubicle.PhoneCur=Phones.GetPhoneForExtensionDB(PIn.Int(mapCubicle.Extension));
			mapCubicle.AllowRightClick=allowRightClickOptions;
			mapCubicle.DragDone+=mapCubicle_DragDone;
			mapCubicle.MapCubicleEdited+=mapCubicle_Edited;
			mapCubicle.RoomControlClicked+=mapCubicle_Clicked;
			mapCubicle.ClickedGoTo+=mapCubicle_ClickedGoTo;
			if (mapCubicle.Handle!=IntPtr.Zero) {
				LayoutManager.Add(mapCubicle,this);
			}
		}

		///<summary>Add a display label to the panel.</summary>
		public void AddDisplayLabel(MapArea mapArea) {
			MapLabel mapLabel=new MapLabel(
				mapArea,
				new Font("Calibri",LayoutManager.ScaleF(14),FontStyle.Bold),//this.FontLabel,
				this.ForeColor,
				this.ColorFloor, //This is effectively the BackColor of this panel, so set DisplayLabel controls BackColor to match.
				GetScreenLocation(mapArea.XPos,mapArea.YPos,this.PixelsPerFoot),
				this.PixelsPerFoot,
				this.AllowDragging,
				this.AllowEditing);
			mapLabel.DragDone+=mapCubicle_DragDone;
			mapLabel.MapAreaDisplayLabelChanged+=mapCubicle_Edited;
			if(mapLabel.Handle!=IntPtr.Zero){
				LayoutManager.Add(mapLabel,this);
			}
		}

		///<summary>Call this BEFORE calling ResizeCubicles.</summary>
		public void ResizeScrollbarsToFitContents() {
			Size sizeControl=new Size(this.WidthFloorFeet*this.PixelsPerFoot,this.HeightFloorFeet*this.PixelsPerFoot);
			if(this.AutoScrollMinSize!=sizeControl) {
				//todo: removed scrolling for now. It was causing more problems than it was worth.
				//this.AutoScrollMinSize=sizeControl;
				this.AutoScrollPosition=new Point(0,0);
			}
		}

		///<summary>Call this BEFORE calling Invalidate(true).</summary>
		private void ResizeCubicles() {
			if(this.Controls is null || this.Controls.Count==0) {
				return;
			}
			for(int i=0;i<this.Controls.Count;i++) {
				if(this.Controls[i]==null) {
					continue;
				}
				else if(this.Controls[i] is MapCubicle) {
					MapCubicle mapCubicle=(MapCubicle)this.Controls[i];
					mapCubicle.Location=GetScreenLocation(mapCubicle.MapAreaCur.XPos,mapCubicle.MapAreaCur.YPos,this.PixelsPerFoot);
					mapCubicle.Size=GetScreenSize(mapCubicle.MapAreaCur.Width,mapCubicle.MapAreaCur.Height,this.PixelsPerFoot);
				}
				else if(this.Controls[i] is MapLabel) {
					MapLabel mapLabel=(MapLabel)this.Controls[i];
					mapLabel.Location=GetScreenLocation(mapLabel.MapAreaCur.XPos,mapLabel.MapAreaCur.YPos,this.PixelsPerFoot);
					mapLabel.Size=MapLabel.GetDrawingSize(mapLabel,this.PixelsPerFoot);
					//draw labels on top of all other controls
					mapLabel.BringToFront();
				}
			}
		}

		#endregion Methods

		#region Drawing
		private void MapAreaPanel_Paint(object sender,PaintEventArgs e) {
			//draw the floor color as the background
			using(Brush brushFloor=new SolidBrush(this.ColorFloor)) {
				e.Graphics.FillRectangle(brushFloor,0,0,(this.WidthFloorFeet*this.PixelsPerFoot),(this.HeightFloorFeet*this.PixelsPerFoot));
			}
			if(ShowGrid) {
				DrawGrid(e.Graphics);
			}
			if(ShowOutline) {
				DrawOutline(e.Graphics);
			}
		}

		private void DrawGrid(Graphics g) {
			using Pen pen=new Pen(this.ColorGrid,1F);
			g.TranslateTransform(this.AutoScrollPosition.X,this.AutoScrollPosition.Y);
			//draw vertical lines
			int x=0;
			while(x<=this.WidthFloorFeet) {
				Point pointTop=new Point(x*PixelsPerFoot,0);
				Point pointBottom=new Point(x*PixelsPerFoot,this.HeightFloorFeet*PixelsPerFoot);
				g.DrawLine(pen,pointTop,pointBottom);
				x++;
			}
			//draw horizontal lines
			int y=0;
			while(y<=this.HeightFloorFeet) {
				Point pointLeft=new Point(0,y*PixelsPerFoot);
				Point pointRight=new Point(this.WidthFloorFeet*PixelsPerFoot,y*PixelsPerFoot);
				g.DrawLine(pen,pointLeft,pointRight);
				y++;
			}
		}

		private void DrawOutline(Graphics g) {
			//draw the oultine around the entire panel
			using(Pen penOutline=new Pen(Color.FromArgb(128,Color.Black),3)) {
				float halfPenWidth=(float)penOutline.Width/2;
				g.DrawRectangle(penOutline,halfPenWidth,halfPenWidth,(this.WidthFloorFeet*this.PixelsPerFoot)-halfPenWidth,(this.HeightFloorFeet*this.PixelsPerFoot)-halfPenWidth);
			}
		}

		///<summary>Calculate the screen location of this cubicle based on x, y, and pixel scaling.</summary>
		public static Point GetScreenLocation(double xPos,double yPos,int pixelsPerFoot) {
			return new Point((int)(xPos*pixelsPerFoot),(int)(yPos*pixelsPerFoot));
		}

		///<summary>Calculate the XPos and YPos of this cubicle base on screen location and pixel scaling.</summary>
		public static PointF ConvertScreenLocationToXY(Point screenLocation,int pixelsPerFoot) {
			return new PointF((float)screenLocation.X/pixelsPerFoot,(float)screenLocation.Y/pixelsPerFoot);
		}

		///<summary>Calculate the screen (drawing) size of this cubicle based on width, height, and pixel scaling.</summary>
		public static Size GetScreenSize(double width,double height,int pixelsPerFoot) {
			return new System.Drawing.Size((int)(width*pixelsPerFoot),(int)(height*pixelsPerFoot));
		}

		#endregion
	}
}
