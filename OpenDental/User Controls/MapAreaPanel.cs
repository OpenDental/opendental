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

		#region Properties available in designer.

		[Category("Cubicle Farm")]
		[Description("Turn dragging on or off")]
		public bool AllowDragging { get; set; }

		[Category("Cubicle Farm")]
		[Description("Turn editing on or off")]
		public bool AllowEditing { get; set; }

		private Font fontLabel=SystemFonts.DefaultFont;
		[Category("Cubicle Farm")]
		[Description("Font sized used for labels")]
		public Font FontLabel {
			get {
				return fontLabel;
			}
			set {
				fontLabel=value;
				ResizeScrollbarsToFitContents();
				Invalidate(true);
			}
		}

		private Font fontCubicle=SystemFonts.DefaultFont;
		[Category("Cubicle Farm")]
		[Description("Font sized used for individual cubicles")]
		public Font FontCubicle {
			get {
				return fontCubicle;
			}
			set {
				fontCubicle=value;
				ResizeScrollbarsToFitContents();
				Invalidate(true);
			}
		}

		private Font fontCubicleHeader=SystemFonts.DefaultFont;
		[Category("Cubicle Farm")]
		[Description("Font sized used for first row header in the cubicle")]
		public Font FontCubicleHeader {
			get {
				return fontCubicleHeader;
			}
			set {
				fontCubicleHeader=value;
				ResizeScrollbarsToFitContents();
				Invalidate(true);
			}
		}

		private int floorWidthFeet=80;
		[Category("Cubicle Farm")]
		[Description("Number of feet left to right")]
		public int FloorWidthFeet {
			get {
				return floorWidthFeet;
			}
			set {
				floorWidthFeet=value;
				ResizeScrollbarsToFitContents();
				ResizeCubicles();
				Invalidate(true);
			}
		}

		private int floorHeightFeet=80;
		[Category("Cubicle Farm")]
		[Description("Number of feet top to bottom")]
		public int FloorHeightFeet {
			get {
				return floorHeightFeet;
			}
			set {
				floorHeightFeet=value;
				ResizeScrollbarsToFitContents();
				ResizeCubicles();
				Invalidate(true);
			}
		}

		private int pixelsPerFoot=10;
		[Category("Cubicle Farm")]
		[Description("Number of pixels used per each foot. Change this to scale the drawing.")]
		public int PixelsPerFoot {
			get {
				return pixelsPerFoot;
			}
			set {
				pixelsPerFoot=value;
				ResizeScrollbarsToFitContents();
				ResizeCubicles();
				Invalidate(true);
			}
		}

		private bool showGrid=false;
		[Category("Cubicle Farm")]
		[Description("Draws the overlay grid underneath the cubicles")]
		public bool ShowGrid {
			get {
				return showGrid;
			}
			set {
				showGrid=value;
				Invalidate();
			}
		}

		private bool showOutline=false;
		[Category("Cubicle Farm")]
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
		[Category("Cubicle Farm")]
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
		[Category("Cubicle Farm")]
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

		#endregion

		#region Events

		public event EventHandler MapAreaChanged;
		public event EventHandler RoomControlClicked;
		///<summary></summary>
		[Category("Property Changed"),Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToChanged=null;

		#endregion

		#region Used for testing.

		private static Random Rand = new Random();

		public int GetRandomDimension() {
			return Rand.Next(0,2)==1?6:9;
		}

		public int GetRandomXPos(int cubicleWidth) {
			return Rand.Next(0,FloorWidthFeet-cubicleWidth);
		}

		public int GetRandomYPos(int cubicleHeight) {
			return Rand.Next(0,FloorHeightFeet-cubicleHeight);
		}

		public MapAreaPanel() {
			InitializeComponent();
			//prevent flickering
			this.DoubleBuffered=true;
		}

		#endregion
		
		#region Manage cubicle controls.
				
		///<summary>Clear the form. Optionally delete the records from the database. Use this option sparingly (if ever).</summary>
		public void Clear(bool deleteFromDatabase) {
			if(deleteFromDatabase) {
				for(int i=0;i<this.Controls.Count;i++) {
					if(!(this.Controls[i] is MapAreaRoomControl)) {
						return;
					}
					MapAreas.Delete(((MapAreaRoomControl)this.Controls[i]).MapAreaItem.MapAreaNum);
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
			MapAreaRoomControl mapAreaRoomControl=new MapAreaRoomControl(
				mapArea,
				TimeSpan.FromSeconds(Rand.Next(60,1200)),
				"Emp: "+this.Controls.Count.ToString(),
				this.Controls.Count,
				mapArea.Extension.ToString(),
				"Status",
				this.FontCubicle,//these two fonts seem to already scale with the control, somehow
				this.FontCubicleHeader,
				Color.FromArgb(40,Color.Red),
				Color.Red,
				this.FloorColor,
				GetScreenLocation(mapArea.XPos,mapArea.YPos,this.PixelsPerFoot),
				GetScreenSize(mapArea.Width,mapArea.Height,this.PixelsPerFoot),
				Properties.Resources.phoneInUse,
				Properties.Resources.gtaicon3,
				Properties.Resources.WebChatIcon,
				Properties.Resources.remoteSupportIcon,
				this.AllowDragging,
				this.AllowEditing,
				allowRightClickOptions);
			mapAreaRoomControl.DragDone+=mapAreaControl_DragDone;
			mapAreaRoomControl.MapAreaRoomChanged+=mapAreaControl_Changed;
			mapAreaRoomControl.RoomControlClicked+=roomControl_Clicked;
			mapAreaRoomControl.GoToChanged+=GoTo_Changed;
			try{
				if(mapAreaRoomControl.Handle==IntPtr.Zero){
					return;
				}
			}
			catch{
				return;
			}
			LayoutManager.Add(mapAreaRoomControl,this);
		}

		///<summary>Add a display label to the panel.</summary>
		public void AddDisplayLabel(MapArea mapArea) {
			MapAreaDisplayLabelControl label=new MapAreaDisplayLabelControl(
				mapArea,
				new Font("Calibri",LayoutManager.ScaleF(14),FontStyle.Bold),//this.FontLabel,
				this.ForeColor,
				this.FloorColor, //This is effectively the BackColor of this panel, so set DisplayLabel controls BackColor to match.				
				GetScreenLocation(mapArea.XPos,mapArea.YPos,this.PixelsPerFoot),
				this.PixelsPerFoot,
				this.AllowDragging,
				this.AllowEditing);
			label.DragDone+=mapAreaControl_DragDone;
			label.MapAreaDisplayLabelChanged+=mapAreaControl_Changed;
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

		///<summary>Alert parent that something has changed</summary>
		void mapAreaControl_Changed(object sender,EventArgs e) {
			if(MapAreaChanged!=null) {
				MapAreaChanged(sender,new EventArgs());
			}
		}


		void roomControl_Clicked(object sender,EventArgs e) {
			RoomControlClicked?.Invoke(sender,new EventArgs());
		}

		///<summary>Alert parent that something has changed</summary>
		void GoTo_Changed(object sender,EventArgs e) {
			GoToChanged?.Invoke(sender,new EventArgs());
		}
		///<summary>Handle the Cubicle.DragDone event</summary>
		void mapAreaControl_DragDone(object sender,EventArgs e) {
			if(sender==null) {
				return;
			}
			Control asControl=null;
			MapArea clinicMapItem=null;
			if(sender is MapAreaRoomControl) {
				asControl=(Control)sender;
				clinicMapItem=((MapAreaRoomControl)sender).MapAreaItem;			
			}
			else if(sender is MapAreaDisplayLabelControl) {
				asControl=(Control)sender;
				clinicMapItem=((MapAreaDisplayLabelControl)sender).MapAreaItem;
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
			mapAreaControl_Changed(sender,new EventArgs());
		}

		///<summary>Call this BEFORE calling ResizeCubicles.</summary>
		public void ResizeScrollbarsToFitContents() {
			Size sizeControl=new Size(this.FloorWidthFeet*this.PixelsPerFoot,this.FloorHeightFeet*this.PixelsPerFoot);
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
				else if(this.Controls[i] is MapAreaRoomControl) {
					MapAreaRoomControl cubicle=(MapAreaRoomControl)this.Controls[i];
					cubicle.Location=GetScreenLocation(cubicle.MapAreaItem.XPos,cubicle.MapAreaItem.YPos,this.PixelsPerFoot);
					cubicle.Size=GetScreenSize(cubicle.MapAreaItem.Width,cubicle.MapAreaItem.Height,this.PixelsPerFoot);
				}
				else if(this.Controls[i] is MapAreaDisplayLabelControl) {
					MapAreaDisplayLabelControl displayLabel=(MapAreaDisplayLabelControl)this.Controls[i];
					displayLabel.Location=GetScreenLocation(displayLabel.MapAreaItem.XPos,displayLabel.MapAreaItem.YPos,this.PixelsPerFoot);
					displayLabel.Size=MapAreaDisplayLabelControl.GetDrawingSize(displayLabel,this.PixelsPerFoot);
					//draw labels on top of all other controls
					displayLabel.BringToFront();
				}
			}
		}

		#endregion

		#region Drawing

		private void MapAreaPanel_Paint(object sender,PaintEventArgs e) {
			//draw the floor color as the background
			using(Brush brushFloor=new SolidBrush(this.FloorColor)) {
				e.Graphics.FillRectangle(brushFloor,0,0,(this.FloorWidthFeet*this.PixelsPerFoot),(this.FloorHeightFeet*this.PixelsPerFoot));
			}
			if(ShowGrid) {
				DrawGrid(e.Graphics);
			}
			if(ShowOutline) {
				DrawOutline(e.Graphics);
			}
		}

		private void DrawGrid(Graphics graphics) {
			Pen pen=new Pen(this.GridColor,1F);
			try {
				graphics.TranslateTransform(this.AutoScrollPosition.X,this.AutoScrollPosition.Y);
				//draw vertical vertical lines
				int x=0;
				while(x<=this.FloorWidthFeet) {
					Point top=new Point(x*PixelsPerFoot,0);
					Point bottom=new Point(x*PixelsPerFoot,this.FloorHeightFeet*PixelsPerFoot);
					graphics.DrawLine(pen,top,bottom);
					x++;
				}
				//draw horizontal lines
				int y=0;
				while(y<=this.FloorHeightFeet) {
					Point left=new Point(0,y*PixelsPerFoot);
					Point right=new Point(this.FloorWidthFeet*PixelsPerFoot,y*PixelsPerFoot);
					graphics.DrawLine(pen,left,right);
					y++;
				}
			}
			catch {
			}
			finally {
				pen.Dispose();
			}
		}

		private void DrawOutline(Graphics graphics) {
			//draw the oultine around the entire panel
			using(Pen penOutline=new Pen(Color.FromArgb(128,Color.Black),3)) {
				float halfPenWidth=(float)penOutline.Width/2;
				graphics.DrawRectangle(penOutline,halfPenWidth,halfPenWidth,(this.FloorWidthFeet*this.PixelsPerFoot)-halfPenWidth,(this.FloorHeightFeet*this.PixelsPerFoot)-halfPenWidth);
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
