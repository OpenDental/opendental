using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	///<summary>Optimized for Width 45.</summary>
	public partial class ControlApptProvSlider : Control{
		#region Fields - Public
		public int MinPerIncr=10;
		/// <summary>Reference to FormApptEdit's checkTimeLocked checkbox. This slider will check this checkbox when moved.</summary>
		public CheckBox FormApptEdit_CheckTimeLocked=new CheckBox();
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		private bool _isProv2;
		private float _heightRow=12;
		///<summary>When dragging in the prov area, this keeps track of which cell we are in so that we can take action when we change cells.</summary>
		private int _indexMouseDownProv;
		///<summary>Controlled by a timer to flash the cursor on and off when selectedIndex > -1</summary>
		private bool _isCursorFlashOn;
		private bool _isMouseDown;
		///<summary>True if mouse down on the slider and dragging.</summary>
		private bool _isDraggingSlider;
		///<summary>The main rect area is slightly inset on the L and R.</summary>
		private float _marginLR=4;
		private string _pattern="//XX////////";
		///<summary>This is the secondary pattern for a second provider on appointments. The length of this will most likely match Pattern, but it isn't guaranteed.</summary>
		private string _patternSecondary="////////////";
		private Point _pointMouse;
		///<summary>The point at which the mouse was originally down during a drag.</summary>
		private Point _pointMouseDown;
		private string _provBarText="";
		///<summary>The rectangle representing the slider. Gets set during paint.</summary>
		private RectangleF _rectangleFSlider;
		private int _selectedIndex=-1;
		private int _selectedLengthBeforeDrag;
		///<summary>Width of the left prov area (and center prov area).</summary>
		private float _widthLeft=12;
		#endregion Fields - Private

		#region Fields - Private Colors Etc
		//This control is not themeable. 
		//Many system-defined colors are used in addition to the custom colors defined here. 
		//Control(aero)?: 192.192.192
		//WhiteSmoke: 245
		//Gainsboro: 220
		//LightGray: 211
		//Silver: 192
		//DarkGray: 169 (unintuitively lighter than gray)
		//Gray: 128
		//DimGray: 105
		private SolidBrush _brushProv;//disposed
		private SolidBrush _brushProv2;
		///<summary>Property</summary>
		private Color _colorProv=Color.FromArgb(200,220,255);
		private Color _colorProv2=Color.FromArgb(220, 200, 255);
		private Color _colorSliderOutline=Color.FromArgb(28,81,128);//button outline: 28,81,128, grid outline: 119,119,146
		private Font _fontCourier;//disposed
		private Font _fontTime;//disposed
		#endregion Fields - Private Colors Etc

		#region Fields - Private Static
		//this is the pattern we will use for themed colors, with all pens and brushes private static
		//<summary>Persists for the entire life of the program.</summary>
		//private static Pen _penGrayLines;
		#endregion Fields - Private Static

		#region Constructor
		public ControlApptProvSlider(){
			this.Size=new Size(33, 688);
			InitializeComponent();
			this.DoubleBuffered=true;
			_brushProv=new SolidBrush(_colorProv);
			_brushProv2=new SolidBrush(_colorProv2);
			_fontCourier=new Font(FontFamily.GenericMonospace,8.5f);
			_fontCourier=new Font(FontFamily.GenericSansSerif,8.5f);
			//this is the pattern we will use for themed colors, with many pens and brushes private static:
			//if(_penGrayLines==null){
			//	_penGrayLines=new Pen(_colorGrayLines);
			//}
		}
		#endregion Constructor

		#region Properties
		[Browsable(false)]
		[DefaultValue(typeof(Color),"200,220,255")]
		///<summary>Color of the cells that represent provider time because Pattern=X</summary>
		public Color ColorProv{
			get{
				return _colorProv;
			}
			set{
				_colorProv=value;
				_brushProv?.Dispose();
				_brushProv=new SolidBrush(_colorProv);
				Invalidate();
			}
		}

		[Browsable(false)]
		[DefaultValue(typeof(Color),"220,200,255")]
		///<summary>Color of the cells that represent provider time because PatternSecondary=X</summary>
		public Color ColorProv2{
			get{
				return _colorProv2;
			}
			set{
				_colorProv2=value;
				_brushProv2?.Dispose();
				_brushProv2=new SolidBrush(_colorProv2);
				Invalidate();
			}
		}

		protected override Size DefaultSize {
			get {
				return new Size(45, 677);
			}
		}

		[Browsable(false)]
		[DefaultValue("//XX////////")]
		///<summary>Corresponds to appointment.Pattern. 5 min per char, and only allow X or /.</summary>
		public string Pattern{
			get{
				return _pattern;
			}
			set{
				if(value==null){//Appointments.CalculatePattern can pass in null to indicate no change
					return;
				}
				if(_pattern==value){
					return;
				}
				_pattern=value;
				if(!_isDraggingSlider){ //The slider already handles changing the length of the patterns
					if(_patternSecondary.Length>_pattern.Length) {
						_patternSecondary=_patternSecondary.Substring(0,_pattern.Length);
					}
					else if(_patternSecondary.Length<Pattern.Length) {
						_patternSecondary=_patternSecondary.PadRight(_pattern.Length,'/');
					}
				}
				int lengthMax=GetLengthMax();
				if(_pattern.Length>lengthMax){
					_pattern=_pattern.Substring(0,lengthMax);
					MsgBox.Show(this,"Appointment time shortened because it was too long to handle.");
				}
				Invalidate();
			}
		}

		[Browsable(false)]
		[DefaultValue("////////////")]
		///<summary>Corresponds to appointment.PatternSecondary. 5 min per char, and only allow X or /.</summary>
		public string PatternSecondary{
			get{
				return _patternSecondary;
			}
			set{
				if(value==null){//Appointments.CalculatePattern can pass in null to indicate no change
					return;
				}
				if(_patternSecondary==value){
					return;
				}
				_patternSecondary=value;
				if(Pattern.Length==0) {
					return;//assuming the pattern hasn't been set yet
				}
				if(!_isDraggingSlider){//The slider already handles changing the length of the patterns
					if(_patternSecondary.Length>_pattern.Length) {
						_patternSecondary=_patternSecondary.Substring(0,_pattern.Length);
					}
					else if(_patternSecondary.Length<Pattern.Length) {
						_patternSecondary=_patternSecondary.PadRight(_pattern.Length,'/');
					}
				}
				int lengthMax=GetLengthMax();
				if(_patternSecondary.Length>lengthMax){
					_patternSecondary=_patternSecondary.Substring(0,lengthMax);
					MsgBox.Show(this,"Appointment time shortened because it was too long to handle.");
				}
				Invalidate();
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		///<summary>Corresponds to appointment.ProvBarText.  One char per cell. No CR or Tab.  Space represents empty cell. No trailing spaces. Do not use the getter inside this class. This truncates the string to not be longer than will fit, which is desirable externally when saving to db. But this is not the desired behavior inside this class. We never want to truncate internally. Always use the private field internally instead of getter.</summary>
		public string ProvBarText{
			get{
				if(_provBarText.Length>SelectedLength){//if longer than what's showing
					return _provBarText.Substring(0,SelectedLength);//trunc
				}	
				return _provBarText;
			}
			set{
				if(_provBarText==value){
					return;
				}
				if(value==null){
					_provBarText="";
					return;
				}
				_provBarText=value;			
				Invalidate();
			}
		}

		///<summary>The cell that has a flashing cursor in it for typing. Or -1.</summary>
		private int SelectedIndex{
			get{
				return _selectedIndex;
			}
			set{
				if(_selectedIndex==value){
					return;
				}
				if(value<-1){
					_selectedIndex=-1;
				}
				else if(value>SelectedLength-1){
					_selectedIndex=SelectedLength-1;
				}
				else{
					_selectedIndex=value;
				}
				if(_selectedIndex==-1){
					timerCursor.Enabled=false;
				}
				else{
					timerCursor.Enabled=false;
					_isCursorFlashOn=true;//restarts cursor as solid so we don't lose track of it
					timerCursor.Enabled=true;
				}
				Invalidate();
			}
		}

		///<summary>The position of the slider bar in time increments. Derived from pattern. Setting this will directly alter Pattern itself.  It's not needed externally, although there are similar functions elsewhere.</summary>
		private int SelectedLength{
			get{
				if(MinPerIncr==10) {
					return Pattern.Length/2;
				}
				else if(MinPerIncr==15) {
					return Pattern.Length/3;
				}
				else{//5
					return Pattern.Length/1;
				}
			}
			set{
				//convert length passed in into 5 min pattern length
				int lengthNew;
				int lengthMax=GetLengthMax();
				if(MinPerIncr==10) {
					lengthNew=value*2;
				}
				else if(MinPerIncr==15) {
					lengthNew=value*3;
				}
				else{//5
					lengthNew=value*1;
				}
				if(lengthNew==Pattern.Length){//already correct length
					return;
				}
				if(lengthNew<Pattern.Length){//simple shorten
					Pattern=Pattern.Substring(0,lengthNew);
				}
				if(lengthNew<PatternSecondary.Length){
					PatternSecondary=PatternSecondary.Substring(0,lengthNew);
				}
				if(lengthNew>lengthMax){//longer than allowed
					lengthNew=lengthMax;
				}
				Pattern=Pattern.PadRight(lengthNew,'/');//when lengthening, this is how it's stored in db
				PatternSecondary=PatternSecondary.PadRight(lengthNew,'/');
				if(MinPerIncr==10 && SelectedIndex>lengthNew/2-1){//If we are hiding the cursor, then officially set it to -1
					SelectedIndex=-1;
				}
				if(MinPerIncr==15 && SelectedIndex>lengthNew/3-1){
					SelectedIndex=-1;
				}
				if(MinPerIncr==5 && SelectedIndex>lengthNew-1){
					SelectedIndex=-1;
				}
				Invalidate();
			}
		}
		#endregion Properties

		#region Methods - Event Handlers - Keyboard

		protected override void OnGotFocus(EventArgs e){
			base.OnGotFocus(e);
			timerCursor.Enabled=false;
			_isCursorFlashOn=true;//restarts cursor as solid so it comes up faster
			timerCursor.Enabled=true;
			Invalidate();
		}
		
		protected override void OnKeyPress(KeyPressEventArgs e){
			//base.OnKeyPress(e);
			//We operate mostly in "insert" mode, instead of "overtype", which is an obsolete UI behavior.
			//The exception is when filling empty spaces. That does not shift lower text.
			//Db cannot contain CR or tab.  Empty cells are represented in Db by a space. No trailing spaces.
			if(SelectedIndex==-1){
				return;
			}
			if(e.KeyChar=='\r'){//Enter implies user wants to force text down
				if(SelectedIndex<_provBarText.Length){//if this is within existing text, insert a space 
					ProvBarText=_provBarText.Substring(0,SelectedIndex)+" "+_provBarText.Substring(SelectedIndex);
				}
				SelectedIndex++;// if this comes after end of text, then we are just moving down without adding anything
				return;
			}
			if(e.KeyChar=='\t'){
				//no need to do anything here.
				//Tab was intercepted earlier and properly moves focus to the next control
			}
			if(e.KeyChar=='\b'){//backspace
				if(SelectedIndex==0){//can't backspace from here
					return;
				}
				if(SelectedIndex<_provBarText.Length+1){//if this is within or immediately after existing text, delete
					ProvBarText=_provBarText.Substring(0,SelectedIndex-1)+_provBarText.Substring(SelectedIndex);
				}
				SelectedIndex--;//if this comes after end of text, then we are just moving up without adding anything
				return;
			}
			//if(e.KeyChar==){//delete
				//Never makes it this far. See OnPreviewKeyDown
			//}
			if(SelectedIndex<_provBarText.Length-1 && _provBarText[SelectedIndex]==' '){//if the current pos is an intermediate space, replace it
				ProvBarText=_provBarText.Substring(0,SelectedIndex)+e.KeyChar+_provBarText.Substring(SelectedIndex+1);
				SelectedIndex++;
				return;
			}
			if(SelectedIndex>_provBarText.Length){//if there is any gap after the end, fill it. Example 3>2
				ProvBarText=_provBarText.PadRight(SelectedIndex);//Example, Pad to 3
			}
			//this works for SelectedIndex=0 and for the char immediately after end of existing string
			ProvBarText=_provBarText.Substring(0,SelectedIndex)+e.KeyChar+_provBarText.Substring(SelectedIndex);
			SelectedIndex++;
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e){
			//OnKeyDown isn't good enough because it doesn't include up/down, etc.
			if(SelectedIndex==-1){
				return;
			}
			if(e.KeyCode==Keys.Delete){
				if(SelectedIndex<_provBarText.Length){//if this is within text, delete
					ProvBarText=_provBarText.Substring(0,SelectedIndex)+_provBarText.Substring(SelectedIndex+1);
				}
				//Don't change selectedIndex
			}
			if(e.KeyCode==Keys.Down){
				e.IsInputKey=true;
				SelectedIndex++;//this already prevents moving down further
			}
			if(e.KeyCode==Keys.Up){
				e.IsInputKey=true;
				if(SelectedIndex==0){
					timerCursor.Enabled=false;
					_isCursorFlashOn=true;//restarts cursor as solid so we don't lose track of it
					timerCursor.Enabled=true;
					return;
				}
				SelectedIndex--;
			}
			if(e.KeyCode==Keys.Left){
				e.IsInputKey=true;
				if(SelectedIndex==0){
					timerCursor.Enabled=false;
					_isCursorFlashOn=true;
					timerCursor.Enabled=true;
					return;
				}
				SelectedIndex--;
			}
			if(e.KeyCode==Keys.Right){
				e.IsInputKey=true;
				SelectedIndex++;
			}
			//base.OnPreviewKeyDown(e);
		}

		protected override void OnResize(EventArgs e){
			base.OnResize(e);
			_heightRow=LayoutManager.Scale(12);
			_marginLR=LayoutManager.Scale(4);
			_widthLeft=LayoutManager.Scale(12);
			_fontCourier?.Dispose();
			_fontCourier=new Font(FontFamily.GenericMonospace,LayoutManager.ScaleF(8.5f));
			_fontTime?.Dispose();
			_fontTime=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.5f));
			Invalidate();
		}

		private void TimerCursor_Tick(object sender, EventArgs e){
			if(SelectedIndex!=-1){
				_isCursorFlashOn=!_isCursorFlashOn;
				Invalidate();
			}
		}
		#endregion Methods - Event Handlers - Keyboard

		#region Methods - Event Handlers - Mouse
		protected override void OnMouseDown(MouseEventArgs e){
			base.OnMouseDown(e);
			if(e.Button != MouseButtons.Left){
				return;
			}
			_pointMouseDown=e.Location;
			_isMouseDown=true;
			if(_rectangleFSlider.Contains(e.Location)){
				_isDraggingSlider=true;
				_selectedLengthBeforeDrag=SelectedLength;
			}
			else{
				_isDraggingSlider=false;
			}
			if(e.X<_marginLR || e.X>Width-_marginLR || e.Y>SelectedLength*_heightRow-1){
				return;
			}
			Focus();
			if(e.X<_marginLR+_widthLeft){//in the prov area at the left
				SelectedIndex=-1;
				_indexMouseDownProv=(int)(e.Y/_heightRow);
				_isProv2=false;
				ToggleProv(_indexMouseDownProv);
			}
			else if(e.X<_marginLR+_widthLeft*2) {//in the prov2 area
				SelectedIndex=-1;
				_indexMouseDownProv=(int)(e.Y/_heightRow);
				_isProv2=true;
				ToggleProv2(_indexMouseDownProv);
			}
			else{//in the text area at the right
				SelectedIndex=(int)(e.Y/_heightRow);
				_indexMouseDownProv=-1;
			}
			Invalidate();
		}

		private void ToggleProv(int index){
			if(MinPerIncr==10) {
				int pos=index*2;
				if(pos>Pattern.Length-1){
					pos=Pattern.Length-1;//should never hit this failsafe
				}
				string strReplace=Pattern.Substring(pos,1)=="X"?"/":"X";//Opposite, either X or /
				string patternNew="";
				if(pos>0){
					patternNew=Pattern.Substring(0,pos);
				}
				patternNew+=strReplace;
				patternNew+=strReplace;
				if(pos+2<Pattern.Length){
					patternNew+=Pattern.Substring(pos+2);
				}
				Pattern=patternNew;
			}
			else if(MinPerIncr==15) {
				int pos=index*3;
				if(pos>Pattern.Length-1){
					pos=Pattern.Length-1;
				}
				string strReplace=Pattern.Substring(pos,1)=="X"?"/":"X";
				string patternNew="";
				if(pos>0){
					patternNew=Pattern.Substring(0,pos);
				}
				patternNew+=strReplace;
				patternNew+=strReplace;
				patternNew+=strReplace;
				if(pos+3<Pattern.Length){
					patternNew+=Pattern.Substring(pos+3);
				}
				Pattern=patternNew;
			}
			else{//5
				int pos=index*1;
				if(pos>Pattern.Length-1){
					pos=Pattern.Length-1;
				}
				string strReplace=Pattern.Substring(pos,1)=="X"?"/":"X";
				string patternNew="";
				if(pos>0){
					patternNew=Pattern.Substring(0,pos);
				}
				patternNew+=strReplace;
				if(pos+1<Pattern.Length){
					patternNew+=Pattern.Substring(pos+1);
				}
				Pattern=patternNew;
			}
		}

		private void ToggleProv2(int index){
			if(ColorProv2==Color.White) {
				return;//No secondary provider has been selected
			}
			if(MinPerIncr==10) {
				int pos=index*2;
				if(pos>PatternSecondary.Length-1){
					pos=PatternSecondary.Length-1;//should never hit this failsafe
				}
				string strReplace=PatternSecondary.Substring(pos,1)=="X"?"/":"X";//Opposite, either X or /
				string patternNew="";
				if(pos>0){
					patternNew=PatternSecondary.Substring(0,pos);
				}
				patternNew+=strReplace;
				patternNew+=strReplace;
				if(pos+2<PatternSecondary.Length){
					patternNew+=PatternSecondary.Substring(pos+2);
				}
				PatternSecondary=patternNew;
			}
			else if(MinPerIncr==15) {
				int pos=index*3;
				if(pos>PatternSecondary.Length-1){
					pos=PatternSecondary.Length-1;
				}
				string strReplace=PatternSecondary.Substring(pos,1)=="X"?"/":"X";
				string patternNew="";
				if(pos>0){
					patternNew=PatternSecondary.Substring(0,pos);
				}
				patternNew+=strReplace;
				patternNew+=strReplace;
				patternNew+=strReplace;
				if(pos+3<PatternSecondary.Length){
					patternNew+=PatternSecondary.Substring(pos+3);
				}
				PatternSecondary=patternNew;
			}
			else{//5
				int pos=index*1;
				if(pos>PatternSecondary.Length-1){
					pos=PatternSecondary.Length-1;
				}
				string strReplace=PatternSecondary.Substring(pos,1)=="X"?"/":"X";
				string patternNew="";
				if(pos>0){
					patternNew=PatternSecondary.Substring(0,pos);
				}
				patternNew+=strReplace;
				if(pos+1<PatternSecondary.Length){
					patternNew+=PatternSecondary.Substring(pos+1);
				}
				PatternSecondary=patternNew;
			}
		}

		protected override void OnMouseLeave(EventArgs e){
			base.OnMouseLeave(e);
			_pointMouse=new Point(-1,-1);
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			_pointMouse=e.Location;
			if(_isMouseDown){
				if(_isDraggingSlider){
					int yTravel=_pointMouse.Y-_pointMouseDown.Y;
					int idxTravel=(int)Math.Round(yTravel/_heightRow);
					if(_selectedLengthBeforeDrag+idxTravel < 1){
						return;
					}
					SelectedLength=_selectedLengthBeforeDrag+idxTravel;
					FormApptEdit_CheckTimeLocked.Checked=true;
				}
				else if(_indexMouseDownProv!=-1){//dragging in the prov area
					if(e.Y<0){
						return;
					}
					if(e.Y>=_rectangleFSlider.Top-1) {
						return;
					}
					//ignore side to side movements.
					int idxNew=(int)(e.Y/_heightRow);
					if(idxNew!=_indexMouseDownProv){//in a new cell
						if(_isProv2){//in the second prov area
							ToggleProv2(idxNew);
						}
						else{//in the prov area at the left
							ToggleProv(idxNew);
						}
						_indexMouseDownProv=idxNew;
					}
				}
			}
			else{//mouse not down
				//hover effects only
			}
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e){
			base.OnMouseUp(e);
			if(e.Button != MouseButtons.Left){
				return;
			}
			_isDraggingSlider=false;
			_isMouseDown=false;
			_isProv2=false;
			Invalidate();
		}


		#endregion Methods - Event Handlers - Mouse

		#region Methods - Event Handlers - OnPaint
		protected override void OnPaint(PaintEventArgs pe){
			base.OnPaint(pe);
			Graphics g=pe.Graphics;//alias
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.TextRenderingHint=System.Drawing.Text.TextRenderingHint.AntiAlias;//keeps text centered in cells better
			using(SolidBrush brushBack=new SolidBrush(this.BackColor)){
				g.FillRectangle(brushBack,0,0,Width,Height);
			}
			g.FillRectangle(Brushes.White,_marginLR,0,Width-_marginLR*2-1,SelectedLength*_heightRow);
			//g.FillRectangle(Brushes.LightGray,_marginLR,0,_widthLeft,SelectedLength*_heightRow);//left handles
			//lines are drawn above each cell
			for(int i=0;i<SelectedLength;i++){
				if(MinPerIncr==10) {
					if(i*2<Pattern.Length && Pattern.Substring(i*2,1)=="X"){
						g.FillRectangle(_brushProv,_marginLR,i*_heightRow,_widthLeft,_heightRow);
					}
					if(i*2<PatternSecondary.Length && PatternSecondary.Substring(i*2,1)=="X"){
						g.FillRectangle(_brushProv2,_marginLR+_widthLeft,i*_heightRow,_widthLeft,_heightRow);
					}
					if(i%6==0){
						g.DrawLine(Pens.Black,_marginLR,i*_heightRow,Width-_marginLR-1,i*_heightRow);
					}
					else{
						g.DrawLine(Pens.Silver,_marginLR,i*_heightRow,Width-_marginLR-1,i*_heightRow);
					}
				}
				else if(MinPerIncr==15){
					if(i*3<Pattern.Length && Pattern.Substring(i*3,1)=="X"){
						g.FillRectangle(_brushProv,_marginLR,i*_heightRow,_widthLeft,_heightRow);
					}
					if(i*3<PatternSecondary.Length && PatternSecondary.Substring(i*3,1)=="X"){
						g.FillRectangle(_brushProv2,_marginLR+_widthLeft,i*_heightRow,_widthLeft,_heightRow);
					}
					if(i%4==0){
						g.DrawLine(Pens.Black,_marginLR,i*_heightRow,Width-_marginLR-1,i*_heightRow);
					}
					else{
						g.DrawLine(Pens.Silver,_marginLR,i*_heightRow,Width-_marginLR-1,i*_heightRow);
					}
				}
				else{//5
					if(i*1<Pattern.Length && Pattern.Substring(i*1,1)=="X"){
						g.FillRectangle(_brushProv,_marginLR,i*_heightRow,_widthLeft,_heightRow);
					}
					if(i*1<PatternSecondary.Length && PatternSecondary.Substring(i*1,1)=="X"){
						g.FillRectangle(_brushProv2,_marginLR+_widthLeft,i*_heightRow,_widthLeft,_heightRow);
					}
					if(i%12==0){
						g.DrawLine(Pens.Black,_marginLR,i*_heightRow,Width-_marginLR-1,i*_heightRow);
					}
					else{
						g.DrawLine(Pens.Silver,_marginLR,i*_heightRow,Width-_marginLR-1,i*_heightRow);
					}
				}
			}
			//ProvBarText-------------------------------------------------------------------------------
			//needs to be a second loop due to descending characters like g that need to draw on top of gridlines
			for(int i=0;i<SelectedLength;i++){
				if(_provBarText.Length>i && _provBarText[i]!=' '){
					//float widthChar=g.MeasureString(ProvBarText.Substring(i,1),Font).Width;
					//float xCenter=_marginLR+_widthLeft+(Width-_marginLR*2f-_widthLeft)/2f;
					//g.DrawString(ProvBarText.Substring(i,1),_fontCourier,Brushes.Black,xCenter-widthChar/2f,i*_heightRow+1);
					g.DrawString(_provBarText.Substring(i,1),_fontCourier,Brushes.Black,_marginLR+(_widthLeft*2)+1,i*_heightRow+1);
				}
			}
			//outline-----------------------------------------------------------------------------------
			g.DrawLine(Pens.Silver,_marginLR+_widthLeft,0,_marginLR+_widthLeft,SelectedLength*_heightRow);//first vertical line closer to L
			g.DrawLine(Pens.Silver,_marginLR+(_widthLeft*2),0,_marginLR+(_widthLeft*2),SelectedLength*_heightRow);//second vertical line closer to R
			g.DrawLine(Pens.Gray,_marginLR,0,Width-_marginLR-1,0);//top
			g.DrawLine(Pens.Gray,_marginLR,0,_marginLR,SelectedLength*_heightRow);//L above slider
			g.DrawLine(Pens.Gray,Width-_marginLR-1,0,Width-_marginLR-1,SelectedLength*_heightRow);//R above slider
			if(DesignMode){
				g.DrawLine(Pens.LightGray,_marginLR,Height-1,Width-_marginLR-1,Height-1);//bottom
				g.DrawLine(Pens.LightGray,_marginLR,SelectedLength*_heightRow,_marginLR,Height);//L below slider
				g.DrawLine(Pens.LightGray,Width-_marginLR-1,SelectedLength*_heightRow,Width-_marginLR-1,Height);//R below slider
			}
			//Slider------------------------------------------------------------------------------------
			_rectangleFSlider=new RectangleF(0,SelectedLength*_heightRow,Width-1,_heightRow+LayoutManager.Scale(4));
			GraphicsPath graphicsPath=ControlApptPanel.GetRoundedPath(_rectangleFSlider,2);
			if(_rectangleFSlider.Contains(_pointMouse) || _isDraggingSlider){
				g.FillPath(Brushes.White,graphicsPath);//hover effect
			}
			else{
				using(SolidBrush brush237=new SolidBrush(Color.FromArgb(237,237,237))){
					g.FillPath(brush237,graphicsPath);
				}
			}
			using(Pen pen=new Pen(_colorSliderOutline)){
				g.DrawPath(pen,graphicsPath);
			}
			graphicsPath.Dispose();
			TimeSpan timeSpan=TimeSpan.FromMinutes(Pattern.Length*5);
			string strTime=timeSpan.ToString("h':'mm");
			float widthStr=g.MeasureString(strTime,_fontTime).Width;
			g.DrawString(strTime,_fontTime,Brushes.Black,Width/2f-widthStr/2,SelectedLength*_heightRow+LayoutManager.ScaleF(3));
			//SelectedIndex------------------------------------------------------------------------------
			if(SelectedIndex>-1 && _isCursorFlashOn && this.Focused){
				using(Pen pen=new Pen(Color.Red,LayoutManager.ScaleF(1))){
					g.DrawLine(pen,_marginLR+(_widthLeft*2)+LayoutManager.Scale(3),	SelectedIndex*_heightRow+2,
						_marginLR+(_widthLeft*2)+LayoutManager.Scale(3), (SelectedIndex+1)*_heightRow-2);
				}
			}
		}
		#endregion Methods - Event Handlers - OnPaint

		#region Methods - Protected
		///<summary>This is to prevent the form from closing when the user is typing something into the prov slider. It will require that alt must be pressed in order to use Access Keys (&OK).</summary>
		protected override bool ProcessDialogChar(char charCode) {
			//Example: o with no modifier will return false at the bottom.  It also doesn't call base, so it doesn't bubble up to parent form.
			if((Control.ModifierKeys & Keys.Alt) == Keys.Alt) {//But if Alt-char, then it will bubble up
				return base.ProcessDialogChar(charCode);
			}
			return false;
    }
		#endregion Methods - Proctected

		#region Methods - Private
		private int GetLengthMax(){
			//This math is a little tricky, so we don't want multiple copies of it floating around
			if(MinPerIncr==10) {
				return (int)((Height-_heightRow-5)/_heightRow)*2;
			}
			else if(MinPerIncr==15) {
				return (int)((Height-_heightRow-5)/_heightRow)*3;
			}
			else{//5
				return (int)((Height-_heightRow-5)/_heightRow)*1;
			}
		}
		#endregion Methods - Private

	}
}
