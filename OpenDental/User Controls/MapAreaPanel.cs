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

		private bool showOutline=false;
		[Category("OD")]
		[Description("Draws outline around the control")]
		public bool ShowOutline {
			get {
				return showOutline;
			}
			set {
				showOutline=value;
				Invalidate();
			}
		}

		private Color gridColor=Color.DarkGray;
		[Category("OD")]
		[Description("Color used to draw the grid lines")]
		public Color GridColor {
			get {
				return gridColor;
			}
			set {
				gridColor=value;
				Invalidate();
			}
		}

		private Color floorColor=Color.White;
		[Category("OD")]
		[Description("Color used to draw the floor")]
		public Color FloorColor {
			get {
				return floorColor;
			}
			set {
				floorColor=value;
				//This is effective the BackColor of this panel so set child controls BackColor to match.
				for(int i=0;i<this.Controls.Count;i++) {
					this.Controls[i].BackColor=floorColor;
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

		private static Random Rand = new Random();

		public int GetRandomDimension() {
			return Rand.Next(0,2)==1?6:9;
		}

		public int GetRandomXPos(int cubicleWidth) {
			return Rand.Next(0,WidthFloorFeet-cubicleWidth);
		}

		public int GetRandomYPos(int cubicleHeight) {
			return Rand.Next(0,HeightFloorFeet-cubicleHeight);
		}

		public MapAreaPanel() {
			InitializeComponent();
			//prevent flickering
			this.DoubleBuffered=true;
		}

		#endregion

		#region Methods - Event Handlers
		protected override void OnClick(EventArgs e) {
			
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
			Control asControl=null;
			MapArea clinicMapItem=null;
			if(sender is MapCubicle) {
				asControl=(Control)sender;
				clinicMapItem=((MapCubicle)sender).MapAreaCur;			
			}
			else if(sender is MapLabel) {
				asControl=(Control)sender;
				clinicMapItem=((MapLabel)sender).MapAreaCur;
			}
			else {
				return;
			}
			//recalculate XPos and YPos based on new location in the panel
			PointF xy=ConvertScreenLocationToXY(asControl.Location,PixelsPerFoot);
			clinicMapItem.XPos=Math.Round(xy.X,3);
			clinicMapItem.YPos=Math.Round(xy.Y,3);
			//save new cubicle location to db
			MapAreas.Update(clinicMapItem);
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
			mapArea.MapAreaNum=Rand.Next(1,1000000);
			mapArea.Description="";
			mapArea.Extension=Rand.Next(100,200);
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
			mapCubicle.Elapsed = TimeSpan.FromSeconds(Rand.Next(60,1200));
			mapCubicle.EmployeeName = "Emp: "+this.Controls.Count.ToString();
			mapCubicle.EmployeeNum = this.Controls.Count;
			mapCubicle.Extension = mapArea.Extension.ToString();
			mapCubicle.Status = "Status";
			mapCubicle.Font = this.FontCubicle;//these two fonts seem to already scale with the control somehow
			mapCubicle.FontHeader=this.FontCubicleHeader;
			mapCubicle.Location = GetScreenLocation(mapArea.XPos,mapArea.YPos,this.PixelsPerFoot);
			mapCubicle.Size=GetScreenSize(mapArea.Width,mapArea.Height,this.PixelsPerFoot);
			mapCubicle.InnerColor = Color.FromArgb(40,Color.Red);
			mapCubicle.OuterColor = Color.Red;
			mapCubicle.BackColor=this.FloorColor;
			mapCubicle.PhoneImage = Properties.Resources.phoneInUse;
			mapCubicle.ChatImage = Properties.Resources.gtaicon3;
			mapCubicle.WebChatImage=Properties.Resources.WebChatIcon;
			mapCubicle.RemoteSupportImage=Properties.Resources.remoteSupportIcon;
			mapCubicle.AllowDragging=AllowDragging;
			mapCubicle.AllowEdit=AllowEditing;
			mapCubicle.Name=mapArea.MapAreaNum.ToString();
			mapCubicle.PhoneCur=Phones.GetPhoneForExtensionDB(PIn.Int(mapCubicle.Extension));
			mapCubicle.AllowRightClick=allowRightClickOptions;
			mapCubicle.DragDone+=mapCubicle_DragDone;
			mapCubicle.MapCubicleEdited+=mapCubicle_Edited;
			mapCubicle.RoomControlClicked+=mapCubicle_Clicked;
			mapCubicle.ClickedGoTo+=mapCubicle_ClickedGoTo;
			try{
				if(mapCubicle.Handle==IntPtr.Zero){
					return;
				}
			}
			catch{
				return;
			}
			LayoutManager.Add(mapCubicle,this);
		}

		///<summary>Add a display label to the panel.</summary>
		public void AddDisplayLabel(MapArea mapArea) {
			MapLabel label=new MapLabel(
				mapArea,
				new Font("Calibri",LayoutManager.ScaleF(14),FontStyle.Bold),//this.FontLabel,
				this.ForeColor,
				this.FloorColor, //This is effectively the BackColor of this panel, so set DisplayLabel controls BackColor to match.				
				GetScreenLocation(mapArea.XPos,mapArea.YPos,this.PixelsPerFoot),
				this.PixelsPerFoot,
				this.AllowDragging,
				this.AllowEditing);
			label.DragDone+=mapCubicle_DragDone;
			label.MapAreaDisplayLabelChanged+=mapCubicle_Edited;
			try{
				if(label.Handle==IntPtr.Zero){
					return;
				}
			}
			catch{
				return;
			}
			LayoutManager.Add(label,this);
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
			if(this.Controls==null || this.Controls.Count<=0) {
				return;
			}
			for(int i=0;i<this.Controls.Count;i++) {
				if(this.Controls[i]==null) {
					continue;
				}
				else if(this.Controls[i] is MapCubicle) {
					MapCubicle cubicle=(MapCubicle)this.Controls[i];
					cubicle.Location=GetScreenLocation(cubicle.MapAreaCur.XPos,cubicle.MapAreaCur.YPos,this.PixelsPerFoot);
					cubicle.Size=GetScreenSize(cubicle.MapAreaCur.Width,cubicle.MapAreaCur.Height,this.PixelsPerFoot);
				}
				else if(this.Controls[i] is MapLabel) {
					MapLabel displayLabel=(MapLabel)this.Controls[i];
					displayLabel.Location=GetScreenLocation(displayLabel.MapAreaCur.XPos,displayLabel.MapAreaCur.YPos,this.PixelsPerFoot);
					displayLabel.Size=MapLabel.GetDrawingSize(displayLabel,this.PixelsPerFoot);
					//draw labels on top of all other controls
					displayLabel.BringToFront();
				}
			}
		}

		#endregion Methods

		#region Drawing
		private void MapAreaPanel_Paint(object sender,PaintEventArgs e) {
			//draw the floor color as the background
			using(Brush brushFloor=new SolidBrush(this.FloorColor)) {
				e.Graphics.FillRectangle(brushFloor,0,0,(this.WidthFloorFeet*this.PixelsPerFoot),(this.HeightFloorFeet*this.PixelsPerFoot));
			}
			if(ShowGrid) {
				DrawGrid(e.Graphics);
			}
			if(ShowOutline) {
				DrawOutline(e.Graphics);
			}
		}

		private void DrawGrid(Graphics graphics) {
			using Pen pen=new Pen(this.GridColor,1F);
			try {
				graphics.TranslateTransform(this.AutoScrollPosition.X,this.AutoScrollPosition.Y);
				//draw vertical vertical lines
				int x=0;
				while(x<=this.WidthFloorFeet) {
					Point top=new Point(x*PixelsPerFoot,0);
					Point bottom=new Point(x*PixelsPerFoot,this.HeightFloorFeet*PixelsPerFoot);
					graphics.DrawLine(pen,top,bottom);
					x++;
				}
				//draw horizontal lines
				int y=0;
				while(y<=this.HeightFloorFeet) {
					Point left=new Point(0,y*PixelsPerFoot);
					Point right=new Point(this.WidthFloorFeet*PixelsPerFoot,y*PixelsPerFoot);
					graphics.DrawLine(pen,left,right);
					y++;
				}
			}
			catch {
			}
		}

		private void DrawOutline(Graphics graphics) {
			//draw the oultine around the entire panel
			using(Pen penOutline=new Pen(Color.FromArgb(128,Color.Black),3)) {
				float halfPenWidth=(float)penOutline.Width/2;
				graphics.DrawRectangle(penOutline,halfPenWidth,halfPenWidth,(this.WidthFloorFeet*this.PixelsPerFoot)-halfPenWidth,(this.HeightFloorFeet*this.PixelsPerFoot)-halfPenWidth);
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
