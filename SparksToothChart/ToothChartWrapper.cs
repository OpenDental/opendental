using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace SparksToothChart {
	///<summary>This is the old tooth chart control.  It "wraps" three different old tooth charts: OpenGL, DirectX9, and 2D.</summary>
	public partial class ToothChartWrapper:UserControl {
		private string[] selectedTeeth;
		///<summary>True for hardware graphics, false for software graphics.</summary>
		private bool hardwareMode=false;
		private ToothChartOpenGL toothChartOpenGL;
		private ToothChartDirectX toothChartDirectX;
		private int preferredPixelFormatNum;
		private ToothChartDirectX.DirectXDeviceFormat deviceFormat=null;
		private DrawingMode drawMode;
		private ToothChartData tcData;

		///<summary></summary>
		[Category("Action"),Description("Occurs when the mouse goes up ending a drawing segment.")]
		public event ToothChartDrawEventHandler SegmentDrawn=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when the mouse goes up committing tooth selection.")]
		public event ToothChartSelectionEventHandler ToothSelectionsChanged=null;
		
		public ToothChartWrapper() {
			drawMode=DrawingMode.Simple2D;
			tcData=new ToothChartData();
			InitializeComponent();
			ResetControls();
		}

		#region Properties

		public DrawingMode DrawMode{
			get{
				return drawMode;
			}
			set{
				if(Environment.OSVersion.Platform==PlatformID.Unix) {
					return;//disallow changing from simpleMode if platform is Unix
				}
				//do not break out if not changing mode.  ContrChart.InitializeOnStartup assumes this code will always run.
				if(drawMode==DrawingMode.DirectX && value!=DrawingMode.DirectX){
					//If switching from from DirectX to another drawing mode,
					//then we need to cleanup DirectX resources in case the 
					//chart is never switched back to DirectX mode.
					toothChartDirectX.Dispose();//Calls CleanupDirectX() and device.Dispose().
					toothChartDirectX=null;
				}
				try {
					drawMode=value;
					ResetControls();
				}
				catch {
					drawMode=DrawingMode.Simple2D;
					ResetControls();
				}
			}
		}

		///<summary>This data object holds nearly all information about what to draw.  It is normally acted on by public methods of the wrapper instead of being accessed directly.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToothChartData TcData {
			get {
				return tcData;
			}
			set {
				if(drawMode==DrawingMode.Simple2D) {
					toothChart2D.TcData=value;
				}
				else if(drawMode==DrawingMode.DirectX) {
					if(tcData!=null){
						tcData.CleanupDirectX();//Clean up old tc data DirectX objects to free video memory.
					}
					toothChartDirectX.TcData=value;
					toothChartDirectX.TcData.PrepareForDirectX(toothChartDirectX.device);
				}
				else if(drawMode==DrawingMode.OpenGL) {
					toothChartOpenGL.TcData=value;
				}
				tcData=value;//Must set last so old tcdata can be cleaned up first.
			}
		}

		///<summary>Valid values are 1-32 and A-Z.</summary>
		public List<string> SelectedTeeth {
			get {
				return tcData.SelectedTeeth;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorBackground {
			get {
				return tcData.ColorBackground;
			}
			set {
				tcData.ColorBackground=value;
				Invalidate();
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorText{
			set {
				tcData.ColorText=value;
				Invalidate();
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorTextHighlight {
			set {
				tcData.ColorTextHighlight=value;
				Invalidate();
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorBackHighlight {
			set {
				tcData.ColorBackHighlight=value;
				Invalidate();
			}
		}

		///<summary>Set to true when using hardware rendering in OpenGL, and false otherwise. This will have no effect when in simple 2D graphics mode.</summary>
		[Browsable(false)]
		public bool UseHardware{
			get{
				return hardwareMode;
			}
			set{
				hardwareMode=value;
			}
		}

		[Browsable(false)]
		public bool AutoFinish{
			get{
				if(drawMode==DrawingMode.OpenGL) {
					return toothChartOpenGL.autoFinish;
				}
				else {
					return false;
				}
			}
			set{
				if(drawMode==DrawingMode.OpenGL) {
					toothChartOpenGL.autoFinish=value;
				}
			}
		}

		[Browsable(false)]
		public int PreferredPixelFormatNumber{
			get{
				return preferredPixelFormatNum;
			}
			set{
				preferredPixelFormatNum=value;
			}
		}

		[Browsable(false)]
		public ToothChartDirectX.DirectXDeviceFormat DeviceFormat {
			get {
				return deviceFormat;
			}
			set {
				deviceFormat=value;
			}
		}

		[Browsable(false)]
		public CursorTool CursorTool{
			get{
				return tcData.CursorTool;
			}
			set{
				tcData.CursorTool=value;
				if(tcData.CursorTool==CursorTool.Pointer) {
					this.Cursor=Cursors.Default;
				}
				if(tcData.CursorTool==CursorTool.Pen) {
					this.Cursor=new Cursor(GetType(),"Pen.cur");
				}
				if(tcData.CursorTool==CursorTool.Eraser) {
					this.Cursor=new Cursor(GetType(),"EraseCircle.cur");
				}
				if(tcData.CursorTool==CursorTool.ColorChanger) {
					this.Cursor=new Cursor(GetType(),"ColorChanger.cur");
				}
				//if(drawMode!=DrawingMode.Simple2D) {
				//	toothChartOpenGL.CursorTool=value;
				//}
			}
		}

		///<summary>For the freehand drawing tool.</summary>
		[Browsable(false)]
		public Color ColorDrawing{
			set{
				tcData.ColorDrawing=value;
			}
		}
		
		[Browsable(false)]
		public bool PerioMode {
			get {
				return tcData.PerioMode;
			}
			set {
				if(drawMode!=DrawingMode.DirectX && value==true) {
					throw new Exception("Only allowed in DirectX");
				}
				tcData.PerioMode=value;
				Invalidate();
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorBleeding {
			set {
				tcData.ColorBleeding=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorSuppuration {
			set {
				tcData.ColorSuppuration=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorFurcations {
			set {
				tcData.ColorFurcations=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorFurcationsRed {
			set {
				tcData.ColorFurcationsRed=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorGingivalMargin {
			set {
				tcData.ColorGingivalMargin=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorCAL {
			set {
				tcData.ColorCAL=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorMGJ {
			set {
				tcData.ColorMGJ=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorProbing {
			set {
				tcData.ColorProbing=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public Color ColorProbingRed {
			set {
				tcData.ColorProbingRed=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public int RedLimitProbing {
			set {
				tcData.RedLimitProbing=value;
			}
		}

		///<summary></summary>
		[Browsable(false)]
		public int RedLimitFurcations {
			set {
				tcData.RedLimitFurcations=value;
			}
		}

		#endregion Properties

		protected override void OnInvalidated(InvalidateEventArgs e) {
			base.OnInvalidated(e);
			if(drawMode==DrawingMode.Simple2D) {
				toothChart2D.Invalidate();
			}
			else if(drawMode==DrawingMode.DirectX) {
				toothChartDirectX.Invalidate();
			}
			else if(drawMode==DrawingMode.OpenGL) {
				toothChartOpenGL.Invalidate();
				//toothChartOpenGL.TaoDraw();
			}
		}

		private void ResetControls(){
			selectedTeeth=new string[0];
			this.Controls.Clear();
			if(drawMode==DrawingMode.Simple2D){
				//this.Invalidate();
				toothChart2D=new ToothChart2D();
				toothChart2D.Dock = System.Windows.Forms.DockStyle.Fill;
				toothChart2D.Location = new System.Drawing.Point(0,0);
				toothChart2D.Name = "toothChart2D";
				//toothChart2D.Size = new System.Drawing.Size(719,564);//unnecessary?
				toothChart2D.SegmentDrawn+=new ToothChartDrawEventHandler(toothChart_SegmentDrawn);
				toothChart2D.ToothSelectionsChanged+=new ToothChartSelectionEventHandler(toothChart_ToothSelectionsChanged);
				toothChart2D.TcData=tcData;
				toothChart2D.SuspendLayout();
				this.Controls.Add(toothChart2D);
				ResetTeeth();
				toothChart2D.InitializeGraphics();
				toothChart2D.ResumeLayout();
			}
			else if(drawMode==DrawingMode.DirectX){
				//I noticed that this code executes when the program starts, then also when the Chart module is selected for the first time.
				//Thus the Chart graphic was loading twice before the user could see it.
				bool isInitialized=true;
				if(toothChartDirectX==null) {
					isInitialized=false;
				}
				if(isInitialized) {
					//Since the control is already initialized, reuse it.  This helps the load time of the Chart module and helps to prevent an issue.
					//This flag helps prevent a red X on the tooth chart when the Chart module is left open and the Windows user is switched then switched back.
				}
				else {
					toothChartDirectX=new ToothChartDirectX();//(hardwareMode,preferredPixelFormatNum);
				}
				//preferredPixelFormatNum=toothChart.SelectedPixelFormatNumber;
				//toothChartDirectX.ColorText=colorText;
				toothChartDirectX.Dock = System.Windows.Forms.DockStyle.Fill;
				toothChartDirectX.Location = new System.Drawing.Point(0,0);
				toothChartDirectX.Name = "toothChartDirectX";
				//toothChartDirectX.Size = new System.Drawing.Size(719,564);//unnecessary?
				toothChartDirectX.SegmentDrawn+=new ToothChartDrawEventHandler(toothChart_SegmentDrawn);
				toothChartDirectX.ToothSelectionsChanged+=new ToothChartSelectionEventHandler(toothChart_ToothSelectionsChanged);
				toothChartDirectX.TcData=tcData;
				toothChartDirectX.SuspendLayout();//Might help with the MDA debug error we used to get (if the option wasn't disabled in our compilers).
				this.Controls.Add(toothChartDirectX);
				ResetTeeth();
				toothChartDirectX.deviceFormat=deviceFormat;
				if(!isInitialized) {
					toothChartDirectX.InitializeGraphics();
				}
				toothChartDirectX.ResumeLayout();//Might help with the MDA debug error we used to get (if the option wasn't disabled in our compilers).
			}
			else if(drawMode==DrawingMode.OpenGL){
				toothChartOpenGL=new ToothChartOpenGL(hardwareMode,preferredPixelFormatNum);
				preferredPixelFormatNum=toothChartOpenGL.SelectedPixelFormatNumber;
				//toothChartOpenGL.ColorText=colorText;
				toothChartOpenGL.Dock = System.Windows.Forms.DockStyle.Fill;
				toothChartOpenGL.Location = new System.Drawing.Point(0,0);
				toothChartOpenGL.Name = "toothChartOpenGL";
				//toothChartOpenGL.Size = new System.Drawing.Size(719,564);//unnecessary?
				toothChartOpenGL.TcData=tcData;
				toothChartOpenGL.SegmentDrawn+=new ToothChartDrawEventHandler(toothChart_SegmentDrawn);
				toothChartOpenGL.ToothSelectionsChanged+=new ToothChartSelectionEventHandler(toothChart_ToothSelectionsChanged);
				toothChartOpenGL.SuspendLayout();
				this.Controls.Add(toothChartOpenGL);
				ResetTeeth();
				toothChartOpenGL.InitializeGraphics();//MakeDisplayLists();
				toothChartOpenGL.ResumeLayout();
			}
		}

		//This step usually happens when the DrawMode property is set on the tooth chart wrapper, by way of the ResetControls() function.
		///<summary>Not normally used unless we are just trying to make a copy of an existing directX control.</summary>
		//public void InitializeDirectXGraphics() {
		//  toothChartDirectX.InitializeGraphics();
		//}

		#region Public Methods

		///<summary>If ListToothGraphics is empty, then this fills it, including the complex process of loading all drawing points from local resources.  Or if not empty, then this resets all 32+20 teeth to default postitions, no restorations, etc. Primary teeth set to visible false.  Also clears selected.  Should surround with SuspendLayout / ResumeLayout.</summary>
		public void ResetTeeth() {
			//selectedTeeth=new string[0];
			//this will only happen once when program first loads.  Unfortunately, there is no way to tell what the drawMode is going to be when loading the graphics from the file.  So any other initialization must happen in resetControls.
			if(tcData.ListToothGraphics.Count==0) {
				tcData.ListToothGraphics.Clear();
				ToothGraphic tooth;
				for(int i=1;i<=32;i++) {
					tooth=new ToothGraphic(i.ToString());
					tooth.Visible=true;
					tcData.ListToothGraphics.Add(tooth);
					//primary
					if(Tooth.PermToPri(i.ToString())!="") {
						tooth=new ToothGraphic(Tooth.PermToPri(i.ToString()));
						tooth.Visible=false;
						tcData.ListToothGraphics.Add(tooth);
					}
				}
				tooth=new ToothGraphic("implant");
				tcData.ListToothGraphics.Add(tooth);
			}
			else {//list was already initially filled, but now user needs to reset it.
				for(int i=0;i<tcData.ListToothGraphics.Count;i++) {//loop through all perm and pri teeth.
					tcData.ListToothGraphics[i].Reset();
				}
			}
			tcData.SelectedTeeth.Clear();
			tcData.DrawingSegmentList=new List<ToothInitial>();
			tcData.PointList=new List<PointF>();
			Invalidate();
		}

		///<summary>Moves position of tooth.  Rotations first in order listed, then translations.  Tooth doesn't get moved immediately, just when painting.  All changes are cumulative and are in addition to any previous translations and rotations.</summary>
		public void MoveTooth(string toothID,float rotate,float tipM,float tipB,float shiftM,float shiftO,float shiftB) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].ShiftM+=shiftM;
			tcData.ListToothGraphics[toothID].ShiftO+=shiftO;
			tcData.ListToothGraphics[toothID].ShiftB+=shiftB;
			tcData.ListToothGraphics[toothID].Rotate+=rotate;
			tcData.ListToothGraphics[toothID].TipM+=tipM;
			tcData.ListToothGraphics[toothID].TipB+=tipB;
			Invalidate();
		}

		///<summary>Sets the specified permanent tooth to primary. Works as follows: Sets ShowPrimaryLetter to true for the perm tooth.  Makes pri tooth visible=true.  Also repositions perm tooth by translating -Y.  Moves primary tooth slightly to M or D sometimes for better alignment.  And if 2nd primary molar, then because of the larger size, it must move all perm molars to distal.</summary>
		public void SetPrimary(string toothID) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			if(Tooth.IsPrimary(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].ShiftO-=12;
			if(ToothGraphic.IsValidToothID(Tooth.PermToPri(toothID))) {//if there's a primary tooth at this location
				tcData.ListToothGraphics[Tooth.PermToPri(toothID)].Visible=true;//show the primary.
				tcData.ListToothGraphics[toothID].ShowPrimaryLetter=true;
			}		
			//first pri mand molars, shift slightly to M
			if(toothID=="21") {
				tcData.ListToothGraphics["J"].ShiftM+=0.5f;
			}
			if(toothID=="28") {
				tcData.ListToothGraphics["S"].ShiftM+=0.5f;
			}
			//second pri molars are huge, so shift distally for space
			//and move all the perm molars distally too
			if(toothID=="4") {
				tcData.ListToothGraphics["A"].ShiftM-=0.5f;
				tcData.ListToothGraphics["1"].ShiftM-=1;
				tcData.ListToothGraphics["2"].ShiftM-=1;
				tcData.ListToothGraphics["3"].ShiftM-=1;
			}
			if(toothID=="13") {
				tcData.ListToothGraphics["J"].ShiftM-=0.5f;
				tcData.ListToothGraphics["14"].ShiftM-=1;
				tcData.ListToothGraphics["15"].ShiftM-=1;
				tcData.ListToothGraphics["16"].ShiftM-=1;
			}
			if(toothID=="20") {
				tcData.ListToothGraphics["K"].ShiftM-=1.2f;
				tcData.ListToothGraphics["17"].ShiftM-=2.3f;
				tcData.ListToothGraphics["18"].ShiftM-=2.3f;
				tcData.ListToothGraphics["19"].ShiftM-=2.3f;
			}
			if(toothID=="29") {
				tcData.ListToothGraphics["T"].ShiftM-=1.2f;
				tcData.ListToothGraphics["30"].ShiftM-=2.3f;
				tcData.ListToothGraphics["31"].ShiftM-=2.3f;
				tcData.ListToothGraphics["32"].ShiftM-=2.3f;
			}
			Invalidate();
		}

		///<summary>This is used for crowns and for retainers.  Crowns will be visible on missing teeth with implants.  Crowns are visible on F and O views, unlike ponics which are only visible on F view.  If the tooth is not visible, that should be set before this call, because then, this will set the root invisible. Tooth numbers 1-32 or A-T.  Supernumeraries not supported here yet.</summary>
		public void SetCrown(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].IsCrown=true;
			if(!tcData.ListToothGraphics[toothID].Visible) {//tooth not visible, so set root invisible.
				tcData.ListToothGraphics[toothID].SetGroupVisibility(ToothGroupType.Cementum,false);
			}
			tcData.ListToothGraphics[toothID].SetSurfaceColors("MODBLFIV",color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.Enamel,color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.EnamelF,color);
			this.Invalidate();
		}

		///<summary>A series of color settings will result in the last ones entered overriding earlier entries.</summary>
		public void SetSurfaceColors(string toothID,string surfaces,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].SetSurfaceColors(surfaces,color);
			Invalidate();
		}

		///<summary>Used for missing teeth.  This should always be done before setting restorations, because a pontic will cause the tooth to become visible again except for the root.  So if setMissing after a pontic, then the pontic can't show.</summary>
		public void SetMissing(string toothID) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].Visible=false;
			Invalidate();
		}

		///<summary>This is just the same as SetMissing, except that it also hides the number from showing.  This is used, for example, if premolars are missing, and ortho has completely closed the space.  User will not be able to select this tooth because the number is hidden.</summary>
		public void SetHidden(string toothID) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].Visible=false;
			tcData.ListToothGraphics[toothID].HideNumber=true;
			Invalidate();
		}

		///<summary>This is used for any pontic, including bridges, full dentures, and partials.  It is usually used on a tooth that has already been set invisible.  This routine cuases the tooth to show again, but the root needs to be invisible.  Then, it sets the entire crown to the specified color.  If the tooth is already visible, then it does not set the root invisible.
		///Tooth numbers 1-32 or A-J or T-K.  Supernumeraries not supported here yet.</summary>
		public void SetPontic(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].IsPontic=true;
			if(!tcData.ListToothGraphics[toothID].Visible) {
				//tooth not visible, but since IsPontic changes the visibility behavior of the tooth, we need to set the root invisible.
				tcData.ListToothGraphics[toothID].SetGroupVisibility(ToothGroupType.Cementum,false);
			}
			tcData.ListToothGraphics[toothID].SetSurfaceColors("MODBLFIV",color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.Enamel,color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.EnamelF,color);
			Invalidate();
		}

		///<summary>Root canals are initially not visible.  This routine sets the canals visible, changes the color to the one specified, and also sets the cementum for the tooth to be semitransparent so that the canals can be seen.  Also sets the IsRCT flag for the tooth to true.</summary>
		public void SetRCT(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].IsRCT=true;
			tcData.ListToothGraphics[toothID].colorRCT=color;
			Invalidate();
		}

		///<summary>This draws a big red extraction X right on top of the tooth.  It's up to the calling application to figure out when it's appropriate to do this.  Even if the tooth has been marked invisible, there's a good chance that this will still get drawn because a tooth can be set visible again for the drawing the pontic.  So the calling application needs to figure out when it's appropriate to draw the X, and not set this otherwise.</summary>
		public void SetBigX(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].DrawBigX=true;
			tcData.ListToothGraphics[toothID].colorX=color;
			Invalidate();
		}

		///<summary>Set this tooth to show a BU or post.</summary>
		public void SetBU(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			//TcData.ListToothGraphics[toothID].IsBU=true;
			//TcData.ListToothGraphics[toothID].colorBU=color;
			//Buildups are now just another group, so
			tcData.ListToothGraphics[toothID].SetGroupVisibility(ToothGroupType.Buildup,true);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.Buildup,color);
			Invalidate();
		}

		///<summary>Set this tooth to show an implant</summary>
		public void SetImplant(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].IsImplant=true;
			tcData.ListToothGraphics[toothID].colorImplant=color;
			Invalidate();
		}

		///<summary>Set this tooth to show a sealant</summary>
		public void SetSealant(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].IsSealant=true;
			tcData.ListToothGraphics[toothID].colorSealant=color;
			Invalidate();
		}

		///<summary>This will mostly only be successful on certain anterior teeth.   For others, it will just show F coloring.</summary>
		public void SetVeneer(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].SetSurfaceColors("BFV",color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.EnamelF,color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.DF,color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.MF,color);
			tcData.ListToothGraphics[toothID].SetGroupColor(ToothGroupType.IF,color);
			Invalidate();
		}

		///<summary>Set this tooth to show a 'W' to indicate that the tooth is being watched.</summary>
		public void SetWatch(string toothID,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].Watch=true;
			tcData.ListToothGraphics[toothID].colorWatch=color;
			Invalidate();
		}

		///<summary></summary>
		public void AddDrawingSegment(ToothInitial drawingSegment) {
			bool alreadyAdded=false;
			for(int i=0;i<tcData.DrawingSegmentList.Count;i++) {
				if(tcData.DrawingSegmentList[i].DrawingSegment==drawingSegment.DrawingSegment) {
					alreadyAdded=true;
					break;
				}
			}
			if(!alreadyAdded){
				tcData.DrawingSegmentList.Add(drawingSegment);
			}
			Invalidate();
			//toothChartOpenGL.AddDrawingSegment(drawingSegment);
		}

		///<summary>Returns a bitmap of what is showing in the control.  Used for printing.</summary>
		public Bitmap GetBitmap() {
			/*
			Bitmap bmp=new Bitmap(this.Width,this.Height);
			Graphics g=Graphics.FromImage(dummy);
			PaintEventArgs e=new PaintEventArgs(g,new Rectangle(0,0,dummy.Width,dummy.Height));
			if(simpleMode) {
				OnPaint(e);
				return dummy;
			}
			toothChartOpenGL.Render(e);
			Bitmap result=toothChartOpenGL.ReadFrontBuffer();
			g.Dispose();
			return result;
			return null;*/
			if(drawMode==DrawingMode.Simple2D) {
				return toothChart2D.GetBitmap();
			}
			if(drawMode==DrawingMode.OpenGL) {
				return toothChartOpenGL.GetBitmap();
			}
			if(drawMode==DrawingMode.DirectX) {
				return toothChartDirectX.GetBitmap();
			}
			return null;
		}

		public void SetToothNumberingNomenclature(ToothNumberingNomenclature nomenclature) {
			tcData.ToothNumberingNomenclature=nomenclature;
			Invalidate();
		}

		public void SetMobility(string toothID,string mobility,Color color) {
			if(!ToothGraphic.IsValidToothID(toothID)) {
				return;
			}
			tcData.ListToothGraphics[toothID].Mobility=mobility;
			tcData.ListToothGraphics[toothID].colorMobility=color;
			Invalidate();
		}

		public void AddPerioMeasure(int intTooth,PerioSequenceType sequenceType,int mb,int b,int db,int ml,int l, int dl) {
			PerioMeasure pm=new PerioMeasure();
			pm.MBvalue=mb;
			pm.Bvalue=b;
			pm.DBvalue=db;
			pm.MLvalue=ml;
			pm.Lvalue=l;
			pm.DLvalue=dl;
			pm.IntTooth=intTooth;
			pm.SequenceType=sequenceType;
			tcData.ListPerioMeasure.Add(pm);
		}

		public void AddPerioMeasure(PerioMeasure pm) {
			tcData.ListPerioMeasure.Add(pm);
		}

		#endregion

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			tcData.SizeControl=this.Size;
			Invalidate();
			if(drawMode==DrawingMode.DirectX){
				//Fire the resize event for the DirectX tooth chart.
				//For some reason the Resize() and Size() events don't fire on the DirectX control
				//if you create them through the designer. Perhaps there is something wrong, but this
				//works for now.
				toothChartDirectX.SetSize(this.Size);
			}
		}

		public void SetSelected(string tooth_id,bool setValue) {
			tcData.SetSelected(tooth_id,setValue);
			Invalidate();
		}

		///<summary></summary>
		protected void OnSegmentDrawn(string drawingSegment){
			ToothChartDrawEventArgs tArgs=new ToothChartDrawEventArgs(drawingSegment);
			if(SegmentDrawn!=null){
				SegmentDrawn(this,tArgs);
			}
		}

		///<summary></summary>
		protected void OnToothSelectionsChanged(){
			if(ToothSelectionsChanged!=null){
				ToothSelectionsChanged(this);
			}
		}

		private void toothChart_SegmentDrawn(object sender,ToothChartDrawEventArgs e) {
			OnSegmentDrawn(e.DrawingSegement);
		}

		private void toothChart_ToothSelectionsChanged(object sender) {
			OnToothSelectionsChanged();
		}

		//public void SimulateMouseDown(MouseEventArgs e) {

		//}
		

	}

	public enum CursorTool{
		Pointer,
		Pen,
		Eraser,
		ColorChanger,
		///<summary>Only supported in Sparks3D</summary>
		MoveText
	}

	///<summary></summary>
	public delegate void ToothChartDrawEventHandler(object sender,ToothChartDrawEventArgs e);
	///<summary></summary>
	public delegate void ToothChartSelectionEventHandler(object sender);
	
	///<summary></summary>
	public class ToothChartDrawEventArgs{
		private string drawingSegment;
		//private bool isInsert;

		///<summary></summary>
		public ToothChartDrawEventArgs(string drawingSeg){//,bool isInsert){
			this.drawingSegment=drawingSeg;
			//this.isInsert=isInsert;
		}

		///<summary></summary>
		public string DrawingSegement{
			get{ 
				return drawingSegment;
			}
		}

		/*//<summary></summary>
		public bool IsInsert{
			get{ 
				return isInsert;
			}
		}*/
	}

	
}
