using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	///<summary>For some internal combo boxes, this is the part that comes up as the "list" to pick from.  It's a Form in order to allow more powerful and longer lists that are larger than the containing form.  It can handle thousands of entries instead of just 100.  Jordan is the only one allowed to edit this file.</summary>
	public partial class FormComboPicker : Form{
		#region Fields - Public
		///<summary>The strings to show in the listbox.</summary>
		public List<string> ListStrings;
		///<summary>Required, but can be the same as ListStrings.  These strings are used in the summary at the top of the listbox.</summary>
		public List<string> ListAbbrevs;
		///<summary>The initial point where the UR corner of this picker window should start, in Screen coordinate.  It might grow up from here if it runs out of room below. It might also rarely need to expand right.</summary>
		public Point PointInitialUR;
		public string OverrideText="";
		#endregion Fields - Public

		#region Fields - Private
		///<summary>This is the height of the dummy combobox at the top: 21 at 96dpi.</summary>
		public int HeightCombo=21;
		///<summary>Based on Font. 13 at 96dpi.</summary>
		private int _heightLineItem=13;
		private int _hoverIndex=-1;
		///<summary>Usually 0.  If scrolled, then this would be the top item showing.</summary>
		private int _indexTopShowing=0;
		private bool _isClosed;
		private bool _isCtrlDown;
		private bool _isMouseDown;
		private bool _isShiftDown;
		///<summary>True if we're still at _pointInitialUR.  False, if we had to shift because we hit the bottom of the screen, etc.  Then, we don't want to draw the "combobox" at the top.</summary>
		private bool _isOriginalLocation;
		///<summary>On mouse down, this copy is made.  Use as needed for logic.  No need to clear when done.</summary>
		private List<int> _listSelectedOrig=new List<int>();
		///<summary></summary>
		private int _mouseDownIndex;
		private System.Windows.Forms.VScrollBar vScroll;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _isMultiSelect=false;
		private List<int> _listSelectedIndices=new List<int>();
		#endregion Fields - Private for Properties

		#region Constructor
		public FormComboPicker(){
			vScroll=new VScrollBar();
			vScroll.Scroll+=VScroll_Scroll;
			vScroll.KeyDown+=VScroll_KeyDown;
			this.Controls.Add(vScroll);//ok with Layout Manager because this window can never resize.
			InitializeComponent();
		}
		#endregion Constructor

		#region Methods - Event Handlers - Key Press
		private void FormComboPicker_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter) {
				Close();
			}
			if(e.KeyCode==Keys.ControlKey) {
				_isCtrlDown=true;
				_isShiftDown=false;
			}
			//Shift can't override Ctrl, but Ctrl can override shift
			if (e.KeyCode==Keys.ShiftKey && !_isCtrlDown) {
				_isShiftDown=true;
			}
			if(e.KeyCode==Keys.Up && !_isMultiSelect) {//arrow key up
				if(_listSelectedIndices.Count<1) {//start at the 0th index
					_listSelectedIndices.Add(0);
				}
				_listSelectedIndices[0]-=1;
				if(_listSelectedIndices[0]<0) {
					_listSelectedIndices[0]=0;
				}
				SetVScrollValue();
			}
			if(e.KeyCode==Keys.Down && !_isMultiSelect) {//arrow key down
				if(_listSelectedIndices.Count<1) {//start at a negative index so that it can be incremented later
					_listSelectedIndices.Add(-1);
				}
				_listSelectedIndices[0]+=1;
				if(_listSelectedIndices[0]>ListStrings.Count-1) {
					_listSelectedIndices[0]=ListStrings.Count-1;
				}
				SetVScrollValue();
			}
			char charKey=(char)e.KeyCode;
			if(e.KeyCode>=Keys.NumPad0 && e.KeyCode<=Keys.NumPad9) {
				charKey=e.KeyCode.ToString().Replace("NumPad","")[0];
			}
			if(char.IsLetterOrDigit(charKey) && !_isMultiSelect) {//alpha or numeric character down
				if(_listSelectedIndices.Count<1) {
					SetSearchedIndex(0,charKey,true);
					Invalidate();
					return;
				}
				bool foundMatch=SetSearchedIndex(_listSelectedIndices[0]+1,charKey);
				if(!foundMatch) {//if nothing is found, then start the search from the beginning
					SetSearchedIndex(0,charKey);
				}
			}
			Invalidate();
		}

		private void FormComboPicker_KeyUp(object sender, KeyEventArgs e){
			if(e.KeyCode==Keys.ControlKey){
				_isCtrlDown=false;
				if(_isMultiSelect){
					Close();
				}
			}
			if(e.KeyCode==Keys.ShiftKey) {
				_isShiftDown=false;
				if(_isMultiSelect) {
					Close();
				}
			}
		}
		#endregion Methods - Event Handlers - Key Press

		#region Methods - Event Handlers - Mouse
		private void FormComboPicker_MouseDown(object sender, MouseEventArgs e){
			_isMouseDown=true;
			if(e.Location.Y<HeightCombo) {//if combobox is hidden (!_isOriginalLocation) height=0
				Close();
				return;//No need to do anything here because the mouse is hovering over the combobox
			}
			_mouseDownIndex=((e.Location.Y-3-HeightCombo)/_heightLineItem)+_indexTopShowing;
			if(_mouseDownIndex>ListStrings.Count-1) {
				 //If they clicked below the last item in the list, it can sometimes be an index that is out of bounds.
				_mouseDownIndex=ListStrings.Count-1;
			}
			if(!_isMultiSelect) {
				_listSelectedIndices=new List<int>(){_hoverIndex};
				Invalidate();
				return;
			}
			_listSelectedOrig=new List<int>(_listSelectedIndices);
			CalcSelectedIndices();
			Invalidate();
		}

		private void FormComboPicker_MouseMove(object sender,MouseEventArgs e) {
			if(e.Location.Y<HeightCombo) {
				_hoverIndex=-1;
				return;//No need to do anything here because the mouse is hovering over the combobox
			}
			//subtract 3 from the location to take border offset into account
			_hoverIndex=((e.Location.Y-3-HeightCombo)/_heightLineItem)+_indexTopShowing; 
			//don't add indices less than 0 or greater than the possible number of strings
			if(_hoverIndex<0 || _hoverIndex>=ListStrings.Count) { 
				_hoverIndex=ListStrings.Count-1;
				return;
			}
			if(!_isMouseDown) {
				Invalidate();
				return;
			}
			if(!_isMultiSelect) {//single select while mouse down
				_listSelectedIndices=new List<int>(){_hoverIndex};
				Invalidate();
				return;
			}
			CalcSelectedIndices();
			Invalidate();
		}

		private void FormComboPicker_MouseUp(object sender, MouseEventArgs e){
			_isMouseDown=false;
			if(e.Location.Y<HeightCombo) {
				Close();
				return;//No need to do anything here because the mouse is hovering over the combobox
			}
			if(_isMultiSelect) {
				if(_isCtrlDown) {
					return;
				}
				if(_isShiftDown) {
					return;
				}
			}
			Close();
			Invalidate();
		}
		#endregion Methods - Event Handlers - Mouse

		#region Methods - Event Handlers
		private void FormComboPicker_Deactivate(object sender, EventArgs e){
			//user clicked outside this dropdown "form"
			if(!_isClosed) {
				Close();
			}
		}

		private void FormComboPicker_FormClosing(object sender,FormClosingEventArgs e) {
			_isClosed=true;
		}

		private void FormComboPicker_Load(object sender, EventArgs e){
			if(ListStrings.Count==0){
				Close();
				return;
			}
			Rectangle rectScreenBounds=Screen.GetWorkingArea(this);
			//listBoxMain ItemHeight=13, height=4+(13*items)
			_heightLineItem=Font.Height;
			int maxItems=(rectScreenBounds.Height/_heightLineItem);//rounds down
			this.Left=PointInitialUR.X-this.Width;
			vScroll.Visible=false;
			vScroll.Enabled=false;
			if(ListStrings.Count>maxItems) {
				this.Height=rectScreenBounds.Height;
				this.Top=rectScreenBounds.Height-this.Height;
				vScroll.Dock=DockStyle.Right;
				vScroll.Height=this.Height;
				vScroll.Minimum=0;
				//Give the maxmimum one extra line item so that the real last line item isn't cut off
				vScroll.Maximum=((ListStrings.Count+1)*_heightLineItem);
				vScroll.SmallChange=_heightLineItem;
				vScroll.LargeChange=this.Height;
				vScroll.Visible=true;
				vScroll.Enabled=true;
				_isOriginalLocation=false;
				HeightCombo=0;
				return;
			} 
			//less than full screen height:
			if(PointInitialUR.Y+(ListStrings.Count*_heightLineItem)+1+HeightCombo>rectScreenBounds.Height) {
				HeightCombo=0;
				//bump it up
				this.Height=(ListStrings.Count*_heightLineItem)+4+HeightCombo;
				this.Top=rectScreenBounds.Height-this.Height;
				_isOriginalLocation=false;
			}
			else{
				this.Top=PointInitialUR.Y;
				this.Height=(ListStrings.Count*_heightLineItem)+4+HeightCombo;
				_isOriginalLocation=true;
			}
		}

		private void FormComboPicker_Paint(object sender, PaintEventArgs e){
			//the top portion is painted to look exactly like the combobox and down arrow that are underneath.
			Graphics g=e.Graphics;
			SolidBrush solidBrushBack=new SolidBrush(Color.FromArgb(229,241,251));
			Pen penArrow=new Pen(Color.FromArgb(20,20,20),1.5f);
			Pen penBlueOutline=new Pen(Color.FromArgb(0,120,215));
			Brush brushSelectedBack=new SolidBrush(SystemColors.Highlight);
			Brush brushSelectedText=new SolidBrush(SystemColors.HighlightText);
			Brush brushHover=new SolidBrush(Color.FromArgb(229,241,251));
			if(_isOriginalLocation){
				Rectangle rectangleCombo=new Rectangle();
				rectangleCombo.X=0;
				rectangleCombo.Y=0;
				rectangleCombo.Width=this.Width-rectangleCombo.X-1;
				rectangleCombo.Height=HeightCombo-1;//The minus one is so it's not touching the edge of the control and hiding the drawing
				g.FillRectangle(solidBrushBack,rectangleCombo);
				g.DrawRectangle(penBlueOutline,rectangleCombo);
				//The down arrow, starting at the left
				g.DrawLine(penArrow,Width-13,9,Width-9.5f,12);
				g.DrawLine(penArrow,Width-9.5f,12,Width-6,9);
				RectangleF rectangleFString=new RectangleF();
				rectangleFString.X=rectangleCombo.X+2;
				rectangleFString.Y=rectangleCombo.Y+4;
				rectangleFString.Width=rectangleCombo.Width-2;
				rectangleFString.Height=rectangleCombo.Height-4;
				int widthMax=rectangleCombo.Width-15;
				StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
				stringFormat.LineAlignment=StringAlignment.Center;
				g.DrawString(GetDisplayText(widthMax),this.Font,Brushes.Black,rectangleFString,stringFormat);
				stringFormat.Dispose();
			}
			//Draw blue rectangle around listbox, which doesn't have its own rectangle
			Rectangle rectangleOutline=new Rectangle();
			rectangleOutline.X=0;
			rectangleOutline.Y=HeightCombo;
			rectangleOutline.Width=this.Width-1;
			//Take the height of the combo box into account
			rectangleOutline.Height=this.Height-HeightCombo-1;
			g.DrawRectangle(penBlueOutline,rectangleOutline);
			//Determines where the index should start when the user has scrolled the list (always needs to round down)
			_indexTopShowing=vScroll.Value/_heightLineItem;
			//Only try to paint 200 objects at most
			int totalToDraw=(200+_indexTopShowing)>ListStrings.Count ? ListStrings.Count : 200+_indexTopShowing;
			for(int i=_indexTopShowing; i<totalToDraw; i++) {
				bool isSelected=false;
				if(_listSelectedIndices.Contains(i)) {
					isSelected=true;
				}
				if(isSelected) { //Draw the selected index with blue background
					g.FillRectangle(brushSelectedBack,new Rectangle(1,((i-_indexTopShowing)*_heightLineItem)+2+HeightCombo,this.Width-3,_heightLineItem));
					g.DrawString(ListStrings[i],this.Font,brushSelectedText,2,((i-_indexTopShowing)*_heightLineItem)+2+HeightCombo);
				}
				else if(_hoverIndex==i) { //Draw the hovered index with a light blue background (only when being moused over)
					g.FillRectangle(brushHover,new Rectangle(1,((i-_indexTopShowing)*_heightLineItem)+2+HeightCombo,this.Width-3,_heightLineItem));
					g.DrawString(ListStrings[i],this.Font,Brushes.Black,2,((i-_indexTopShowing)*_heightLineItem)+2+HeightCombo);
				}
				else { //Just draw the standard black text
					g.DrawString(ListStrings[i],this.Font,Brushes.Black,2,((i-_indexTopShowing)*_heightLineItem)+2+HeightCombo);
				}
			}
			solidBrushBack.Dispose();
			penArrow.Dispose();
			penBlueOutline.Dispose();
			brushSelectedBack.Dispose();
			brushSelectedText.Dispose();
			brushHover.Dispose();
		}

		private void FormComboPicker_Scroll(object sender,ScrollEventArgs e) {
			//Called from WndProc, so if it is not enabled, then return
			if(!vScroll.Enabled) {
				return;
			}
			if(e.OldValue>e.NewValue) { //Scroll up
				//Make sure it isn't below the minimum
				if(vScroll.Value-vScroll.SmallChange<0) { 
					vScroll.Value=0;
				}
				else {
					vScroll.Value-=vScroll.SmallChange;
				}
			}
			else { //Scroll down
				//Make sure it isn't past the max
				if(vScroll.Value+vScroll.SmallChange>(vScroll.Maximum-this.Height)) { 
					//Add a little extra to make sure it scrolls down to the last fake index so that all of them are visible 
					vScroll.Value=vScroll.Maximum-this.Height+2; 
				}
				else {
					vScroll.Value+=vScroll.SmallChange;
				}
			}
			Invalidate();
		}

		private void FormComboPicker_Shown(object sender,EventArgs e) {
			//First arrow key would normally not be processed, but would instead just give focus to the vScroll.
			//The line below makes it so that the first arrow key gets sent the the vScroll and Form as intended.
			vScroll.Focus();
			Refresh();
		}

		private void VScroll_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down) {
				e.Handled=true;//don't scroll using arrow keys because we handle that manually
			}
		}

		private void VScroll_Scroll(object sender,ScrollEventArgs e) {
			Invalidate();
		}		
		#endregion Methods - Event Handlers

		#region Properties
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsMultiSelect{
			get{
				return _isMultiSelect;
			}
			set{
				if(_isMultiSelect==value){
					return;
				}
				_isMultiSelect=value;
			}
		}

		///<summary>Only used when IsMultiSelect=false;</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex {
			get{
				if(_listSelectedIndices.Count==0){
					return -1;
				}
				return _listSelectedIndices[0];
			}
			set{
				if(value<-1 || value>ListStrings.Count-1){
					return;//ignore out of range
				}
				_listSelectedIndices.Clear();
				if(value!=-1){
					_listSelectedIndices.Add(value);
				}
			}
		} 

		///<summary>Only used when IsMultiSelect=true;</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> SelectedIndices{
			get{
				//todo (low priority)
				//returns a value, not a reference?
				return _listSelectedIndices;
			}
			set{
				if(!_isMultiSelect){
					throw new Exception("Cannot set SelectedIndices when not IsMultiSelect. Use SelectedIndex, SetSelected, etc.");
				}
				_listSelectedIndices=value;
				Invalidate();
			}
		}
		#endregion Properties

		#region Methods - Protected
		///<summary>We have to intercept the mouse scroll event, because there are no actual controls on the screen so there is no way to actually scroll as there is no content</summary>
		protected override void WndProc(ref Message m) {
			if(m.Msg==WM_MOUSEWHEEL) { //Check for scrolling
				int delta;
				if((long)m.WParam>=(long)Int32.MaxValue) { //Just in case the param is larger than the max
					var wParam=new IntPtr((long)m.WParam << 32 >> 32);
					delta=wParam.ToInt32() >> 16;
				}
				else {
					delta=m.WParam.ToInt32() >> 16;
				}
				delta*=-1;
				//If true, then scroll up, otherwise scroll down
				ScrollEventArgs sarg=delta > 0 ? new ScrollEventArgs(ScrollEventType.EndScroll,0,1) : new ScrollEventArgs(ScrollEventType.EndScroll,1,0);
        FormComboPicker_Scroll(this, sarg);
			}
			if(m.Msg==WM_DPICHANGED){
				return;//ignore.   This form does no dpi scaling whatsoever.  It's under total control of the calling combobox.
			}
			base.WndProc(ref m);
		}

		private const int WM_DPICHANGED=0x02E0;
		private const int WM_MOUSEWHEEL=0x020A;
		#endregion Methods - Protected

		#region Methods - Private
		///<summary>Called on mouse down and mouse move.  Recalculates the _listSelectedIndices, based on _listSelectedOrig, _mouseDownIndex, _hoverIndex, isShiftDown, isCtrlDown.  By looping through all items in entire list each time.</summary>
		private void CalcSelectedIndices() {
			if(_isShiftDown) {
				//shift only responds to clicking, not dragging
				if(_listSelectedIndices.Count==0) {
					_listSelectedIndices.Add(_mouseDownIndex);
				}
				else {
					//The first row that was selected in the list
					int fromRow=_listSelectedIndices[0];
					//Nothing needs to change 
					if(_mouseDownIndex==fromRow) {
						return;
					}
					_listSelectedIndices.Clear();
					if(_mouseDownIndex<fromRow) { //Dragging up
						for(int i=_mouseDownIndex;i<=fromRow;i++) {
							_listSelectedIndices.Add(i);
						}
					}
					else { //Dragging down
						for(int i=fromRow;i<=_mouseDownIndex;i++) {
							_listSelectedIndices.Add(i);
						}
					}
				}
				return;//If shift is down, that means that ctrl can't be down, so that means we can return here
			}
			_listSelectedIndices.Clear();
			for(int i=0; i<ListStrings.Count; i++) {
				bool isInRange=false;
				if(i>=_mouseDownIndex && i<=_hoverIndex) { //Mouse is lower than start
					isInRange=true;
				}
				if(i<=_mouseDownIndex && i>=_hoverIndex) {//Mouse is higher than start
					isInRange=true;
				}
				if(_isCtrlDown) {
					if(isInRange) {
						if(!_listSelectedOrig.Contains(i)) {
							_listSelectedIndices.Add(i);//opposite of original
						}
					}
					else {//out of range
						if(_listSelectedOrig.Contains(i)) {
							_listSelectedIndices.Add(i);//same of original
						}
					}
					continue;
				}
				//ctrl not down:
				if (isInRange) {
					_listSelectedIndices.Add(i);
				}
			}
		}

		///<summary>If multiple items are selected, we string them together with commas.  But if the string is wider than widthMax, we instead show "Multiple".</summary>
		private string GetDisplayText(int widthMax){
			if(_listSelectedIndices.Count==0){
				if(!IsMultiSelect){
					return OverrideText;//this will usually be empty string
				}
			}
			if(_listSelectedIndices.Contains(0) && ListStrings[0]=="All"){//This isn't a very good test, but it is just display text.  CLINIC_NUM_ALL){
				return "All";
			}
			if(_listSelectedIndices.Count==1){
				if(_listSelectedIndices[0]<ListStrings.Count){//If the index is inside the range
					return ListStrings[_listSelectedIndices[0]];//full text
				}
			}
			string str="";
			for(int i=0;i<_listSelectedIndices.Count;i++){
				if(i>0){
					str+=",";
				}
				if(_listSelectedIndices[i]<ListAbbrevs.Count) {
					str+=ListAbbrevs[_listSelectedIndices[i]];
				}
			}
			if(_listSelectedIndices.Count>1){
				if(TextRenderer.MeasureText(str,this.Font).Width>widthMax){
					return "Multiple";
				}
			}
			return str;
		}

		///<summary>Used to search through a combo box to find a matching index and set it. Returns true if it found a match, otherwise false.</summary>
		private bool SetSearchedIndex(int startIdx, char charKey, bool addIdx=false) {
			for(int i=startIdx;i<ListStrings.Count;i++) {//loop through the items and only add the item if it is found
				if(ListStrings[i].ToUpper().StartsWith(charKey.ToString())) {//charKey is already uppercased
					if(addIdx) {
						_listSelectedIndices.Add(i);
					}
					else {
						_listSelectedIndices[0]=i;
					}
					SetVScrollValue();
					return true;
				}
			}
			return false;
		}

		///<summary>Sets the value of the scroll bar to a selected item that is outside of view</summary>
		private void SetVScrollValue() {
			if(_listSelectedIndices.Count<1) {
				return;
			}
			if(_listSelectedIndices[0]<_indexTopShowing) {//the selected index is above the combobox bounds
				for(int i=_indexTopShowing;i>0;i--) {
					if(i==_listSelectedIndices[0]) {
						break;
					}
					if(vScroll.Value-vScroll.SmallChange<0) {
						vScroll.Value=0;
						break;
					}
					vScroll.Value-=vScroll.SmallChange;
				}
			}
			if((_listSelectedIndices[0]-_indexTopShowing+1)*_heightLineItem>this.Height) {//the selected index is hidden below bounds
				for(int i=this.Height;i<=(_listSelectedIndices[0]-_indexTopShowing+1)*_heightLineItem;i+=_heightLineItem) {
					if(i==_listSelectedIndices[0]) {
						break;
					}
					if(vScroll.Value+vScroll.SmallChange>vScroll.Maximum-this.Height) {
						vScroll.Value=vScroll.Maximum-this.Height+2;
						break;
					} 
					vScroll.Value+=vScroll.SmallChange;
				}
			}
		}
		#endregion Methods - Private
	}
}