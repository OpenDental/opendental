using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace WpfControls.UI {
/*
Jordan is the only one allowed to edit this file.
*/
	///<summary>For a comboBox, this is the part that comes up as the "list" to pick from.  It can handle thousands of entries.</summary>
	public partial class WindowComboPicker:Window {
		#region Fields - Public
		///<summary>The strings to show in the listbox.</summary>
		public List<string> ListStrings;
		///<summary>Required, but can be the same as ListStrings.  These strings are used in the summary at the top of the listbox.</summary>
		public List<string> ListAbbrevs;
		///<summary>The initial point where the UL corner of this picker window should start, in Desktop coordinates.  It might grow up from here if it runs out of room below. This point is not in DIPs, but is actual pixels of the entire desktop.  So it could be a very big number if on the righthand monitor at high dpi.</summary>
		public Point PointInitial;
		public System.Windows.Controls.Primitives.ScrollBar ScrollBar;
		#endregion Fields - Public

		#region Fields - Private
		private int _hoverIndex=-1;
		///<summary>Usually 0.  If scrolled, then this would be the top item showing.</summary>
		private int _indexTopShowing=0;
		private bool _isClosed;
		private bool _isCtrlDown;
		//private bool _isMouseDown;
		private bool _isShiftDown;
		///<summary>On mouse down, this copy is made.  Use as needed for logic.  No need to clear when done.</summary>
		private List<int> _listSelectedOrig=new List<int>();
		///<summary></summary>
		private int _mouseDownIndex;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _isMultiSelect=false;
		private List<int> _listIndicesSelected=new List<int>();
		#endregion Fields - Private for Properties

		#region Constructor
		public WindowComboPicker() {
			InitializeComponent();
			SetZoom();
			Closing+=WindowComboPicker_Closing;
			Loaded+=WindowComboPicker_Loaded;
			//todo: make sure the first arrow key works.  
			//Shown += new System.EventHandler(FormComboPicker_Shown);
			stackPanel.KeyDown+=StackPanel_KeyDown;
			stackPanel.KeyUp +=StackPanel_KeyUp;
		}
		#endregion Constructor

		#region Events
		///<summary>Only relevant if IsMultiSelect, so generally not raised for single.</summary>
		public event EventHandler SelectionChanged;
		#endregion Events

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
				if(_listIndicesSelected.Count==0){
					return -1;
				}
				return _listIndicesSelected[0];
			}
			set{
				if(value<-1 || value>ListStrings.Count-1){
					return;//ignore out of range
				}
				_listIndicesSelected.Clear();
				if(value!=-1){
					_listIndicesSelected.Add(value);
				}
			}
		} 

		///<summary>Only used when IsMultiSelect=true;</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> ListIndicesSelected{
			get{
				//todo (low priority)
				//returns a value, not a reference?
				return _listIndicesSelected;
			}
			set{
				if(!_isMultiSelect){
					throw new Exception("Cannot set SelectedIndices when not IsMultiSelect. Use SelectedIndex, SetSelected, etc.");
				}
				_listIndicesSelected=value;
				SetColors();
			}
		}
		#endregion Properties

		#region Methods - Event Handlers - Key Press
		private void StackPanel_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				Close();
			}
			if(e.Key==Key.LeftCtrl || e.Key==Key.RightCtrl) {
				_isCtrlDown=true;
				_isShiftDown=false;
			}
			//Shift can't override Ctrl, but Ctrl can override shift
			if ((e.Key==Key.LeftShift || e.Key==Key.RightShift) && !_isCtrlDown) {
				_isShiftDown=true;
			}
			if(e.Key==Key.Up) {//arrow key up
				if(_listIndicesSelected.Count==0) {
					_listIndicesSelected.Add(0);//top
				}
				else if(_isMultiSelect){
					//I considered using the same algorithm as MS listbox,
					//where up and down both start at the most recently selected item.
					//But a little extra work would be required and I just don't think anyone cares.
					int topSelected=_listIndicesSelected.Min();
					_listIndicesSelected=new List<int>();
					_listIndicesSelected.Add(topSelected);
				}
				_listIndicesSelected[0]--;
				if(_listIndicesSelected[0]<0) {
					_listIndicesSelected[0]=0;
				}
				//SetVScrollValue();
			}
			if(e.Key==Key.Down) {//arrow key down
				if(_listIndicesSelected.Count==0) {
					_listIndicesSelected.Add(-1);//this will move down to 0
				}
				else if(_isMultiSelect){
					int bottomSelected=_listIndicesSelected.Max();
					_listIndicesSelected=new List<int>();
					_listIndicesSelected.Add(bottomSelected);
				}
				_listIndicesSelected[0]++;
				if(_listIndicesSelected[0]>ListStrings.Count-1) {
					_listIndicesSelected[0]=ListStrings.Count-1;
				}
				//SetVScrollValue();
			}
			/*
			char charKey=(char)e.Key;
			if(e.Key>=Key.NumPad0 && e.Key<=Key.NumPad9) {
				charKey=e.Key.ToString().Replace("NumPad","")[0];
			}
			//nobody cares about this, but it can be added later
			if(char.IsLetterOrDigit(charKey) && !_isMultiSelect) {//alpha or numeric character down
				if(_listIndicesSelected.Count<1) {
					SetSearchedIndex(0,charKey,true);
					Invalidate();
					return;
				}
				bool foundMatch=SetSearchedIndex(_listIndicesSelected[0]+1,charKey);
				if(!foundMatch) {//if nothing is found, then start the search from the beginning
					SetSearchedIndex(0,charKey);
				}
			}*/
			//Invalidate();
			SelectionChanged?.Invoke(this,new EventArgs());
			SetColors();
		}

		private void StackPanel_KeyUp(object sender,KeyEventArgs e) {
			if(e.Key==Key.LeftCtrl || e.Key==Key.RightCtrl){
				_isCtrlDown=false;
				//if(_isMultiSelect){
				//	Close();
				//}
			}
			if(e.Key==Key.LeftShift || e.Key==Key.RightShift) {
				_isShiftDown=false;
				//if(_isMultiSelect) {
				//	Close();
				//}
			}
		}
		#endregion Methods - Event Handlers - Key Press
		
		#region Methods - Event Handlers - Mouse
		private void Item_MouseLeave(object sender,MouseEventArgs e) {
			_hoverIndex=-1;
			SetColors();
		}

		private void Item_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			IInputElement iInputElement=Keyboard.FocusedElement;
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			Point point=e.GetPosition(this);
			_mouseDownIndex=IndexFromPoint(point); 
			_listSelectedOrig=new List<int>(_listIndicesSelected);//for both multi and single, we need to remember this
			if(IsMultiSelect) {
				_listSelectedOrig=new List<int>(_listIndicesSelected);
				CalcSelectedIndices();
				SelectionChanged?.Invoke(this,new EventArgs());
			}
			else{//single
				_listIndicesSelected=new List<int>(){_mouseDownIndex};
				//Close(); //This is handled from the combobox, otherwise the _windowComboPicker_PreviewLeftButtonUp event won't fire 
			}
			SetColors();
//todo: test this issue
			//base.OnMouseDown(e);//at end so that index will be selected before mouse down fires somewhere else (e.g. FormApptEdit).  Matches MS.
			//but, sometimes, if an event from above resulted in a dialog, then there will be no mouse up event.  Handle that below.
			//MouseButtons mouseButtons=Control.MouseButtons;//introducing variable for debugging because this state is not preserved at break points.
			//if(mouseButtons==MouseButtons.None){
			//	_isMouseDown=false;
			//}
		}

		private void Item_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			//_isMouseDown=false;
			//this only fires when isMultiSelect because otherwise mouse down closes.
			//if(_isCtrlDown) {
			//	return;
			//}
			//if(_isShiftDown) {
			//	return;
			//}
			//Close();
			//SetColors();
		}

		private void Item_MouseMove(object sender,MouseEventArgs e) {
			Point point=e.GetPosition(this);
			_hoverIndex=IndexFromPoint(point);
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(!isMouseDown) {
				SetColors();
				return;
			}
			double y=e.GetPosition(this).Y;
			//from here down, dragging
			List<int> listSelectedOrig=new List<int>(_listIndicesSelected);//for both multi and single, we need to remember this. Just local.
			if(IsMultiSelect) {
				CalcSelectedIndices();
				SelectionChanged?.Invoke(this,new EventArgs());
			}
			else{//single select while mouse down
				//only add valid indices
				if(_hoverIndex>=0 && _hoverIndex < ListStrings.Count) {
					_listIndicesSelected=new List<int>(){_hoverIndex};
				}
			}
			SetColors();
		}
		#endregion Methods - Event Handlers - Mouse

		#region Methods - Event Handlers
		private void stackPanel_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//IInputElement iInputElement=Keyboard.FocusedElement;
			if(_isClosed){
				return;
			}
			Close();
		}

		private void WindowComboPicker_Closing(object sender,CancelEventArgs e) {
			_isClosed=true;
		}

		private void WindowComboPicker_Loaded(object sender,RoutedEventArgs e) {
			Keyboard.Focus(stackPanel);
			if(ListStrings.Count==0){
				Close();
				return;
			}
			for(int i=0;i<ListStrings.Count;i++){
				Border border=new Border();//default isfocusable false
				TextBlock textBlock=new TextBlock();//default isfocusable false
				border.Child=textBlock;
				textBlock.Text=ListStrings[i];
				textBlock.Margin=new Thickness(2,0,0,0);
				border.MouseLeave+=Item_MouseLeave;
				border.MouseLeftButtonDown+=Item_MouseLeftButtonDown;
				border.MouseLeftButtonUp+=Item_MouseLeftButtonUp;
				border.MouseMove+=Item_MouseMove;
				stackPanel.Children.Add(border);//stackpanel default isfocusable false
			}
			stackPanel.UpdateLayout();//so that height measurement will work
			SetColors();
			//Point point=PointToScreen(PointInitial);//wrong
			System.Drawing.Point drawing_Point=new System.Drawing.Point((int)PointInitial.X,(int)PointInitial.Y);
			System.Drawing.Rectangle drawing_Rectangle=System.Windows.Forms.Screen.GetWorkingArea(drawing_Point);
			Rect rectScreenBounds=new Rect(drawing_Rectangle.X,drawing_Rectangle.Y,drawing_Rectangle.Width,drawing_Rectangle.Height);//Example: {4035,0,3645,2066}
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			double scaleWindows=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			//Another option seems to be:
			//VisualTreeHelper.GetDpi(this).DpiScaleX or PixelsPerDip
			//When specifying the location for this window, it must be in desktop coords, but adjusted to use DIPs.
			//I tried using PointFromScreen to convert back to DIPs, but it gave different results each time, even with the same input.
			Point pointNewDesktop=PointInitial;
			double heightAllPix=(stackPanel.ActualHeight+3)*scaleWindows;
			if(heightAllPix>rectScreenBounds.Height) {//full screenheight
				Height=rectScreenBounds.Height/scaleWindows;//DIP
				pointNewDesktop.Y=rectScreenBounds.Top;
				pointNewDesktop.X-=14*scaleWindows;
			}
			//less than full screen height:
			else if(pointNewDesktop.Y+heightAllPix>rectScreenBounds.Bottom) {
				//hitting the bottom, so bump it up
				Height=heightAllPix/scaleWindows;//DIP
				pointNewDesktop.Y=rectScreenBounds.Bottom-heightAllPix;
				//Move list left without changing width. This allows the dropdown button to still show.
				pointNewDesktop.X-=14*scaleWindows;
			}
			else{
				Height=heightAllPix/scaleWindows;//DIP
			}
			Point pointDIP=new Point(pointNewDesktop.X/scaleWindows,pointNewDesktop.Y/scaleWindows);
			Left=pointDIP.X;
			Top=pointDIP.Y;
			ScrollBar = scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) as System.Windows.Controls.Primitives.ScrollBar;
		}

		/*
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
		}*/

		/*
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
		}*/
		#endregion Methods - Event Handlers

		#region Methods - private
		///<summary>Called on mouse down and mouse move.  Recalculates the _listIndicesSelected, based on _listSelectedOrig, _mouseDownIndex, _hoverIndex, isShiftDown, isCtrlDown.  By looping through all items in entire list each time.</summary>
		private void CalcSelectedIndices() {
			if(_isShiftDown) {
				//shift only responds to clicking, not dragging
				if(_listIndicesSelected.Count==0) {
					_listIndicesSelected.Add(_mouseDownIndex);
				}
				else {
					//The first row that was selected in the list
					int fromRow=_listIndicesSelected[0];
					//Nothing needs to change 
					if(_mouseDownIndex==fromRow) {
						return;
					}
					_listIndicesSelected.Clear();
					if(_mouseDownIndex<fromRow) { //Dragging up
						for(int i=_mouseDownIndex;i<=fromRow;i++) {
							_listIndicesSelected.Add(i);
						}
					}
					else { //Dragging down
						for(int i=fromRow;i<=_mouseDownIndex;i++) {
							_listIndicesSelected.Add(i);
						}
					}
				}
				return;//If shift is down, that means that ctrl can't be down, so that means we can return here
			}
			_listIndicesSelected.Clear();
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
							_listIndicesSelected.Add(i);//opposite of original
						}
					}
					else {//out of range
						if(_listSelectedOrig.Contains(i)) {
							_listIndicesSelected.Add(i);//same of original
						}
					}
					continue;
				}
				//ctrl not down:
				if (isInRange) {
					_listIndicesSelected.Add(i);
				}
			}
		}

		///<summary>Gets the index at the specified point. Returns -1 if no index can be selected at that point. Pass in the y pos relative to this control. It can fall outside the control when dragging.</summary>
		private int IndexFromPoint(Point point){
			double offset=scrollViewer.VerticalOffset;//example 12 if scrolled down one line
			Point pointRelativeToStack=new Point(point.X,point.Y+offset);//example, yRelativeToThis=5, so yRelativeToStack=17.
			for(int i=0;i<stackPanel.Children.Count;i++){
				Border border=(Border)stackPanel.Children[i];
				Point pointRelativeToChild=stackPanel.TranslatePoint(pointRelativeToStack,border);
				//Example. For the second item, pointRelativeToChild=5, which is within this item.
				if(pointRelativeToChild.Y < 0){
					continue;
				}
				if(pointRelativeToChild.Y > border.ActualHeight){
					continue;
				}
				return i;
			}
			return -1;
		}

		///<summary>Sets background colors for all rows, based on selected indices and hover.</summary>
		private void SetColors(){
			for(int i=0;i<stackPanel.Children.Count;i++){
				Color colorBack=Colors.White;
				if(_listIndicesSelected.Contains(i)){
					colorBack=Color.FromRgb(186,199,219);//#BAC7DB,   grid default was Silver: C0C0C0 (192 gray)
				}
				else if(_hoverIndex==i){
					colorBack=Color.FromRgb(229,239,251);//#E5EFFB
				}
				Border border=(Border)stackPanel.Children[i];
				border.Background=new SolidColorBrush(colorBack);
			}
		}

		private void SetZoom(){
			if(ComputerPrefs.IsLocalComputerNull()){
				return;
			}
			float zoom=1;
			try{
				zoom=ComputerPrefs.LocalComputer.Zoom/100f;//example 1.2
			}
			catch{
				return;
			}
			if(zoom==0){
				zoom=1;
			}
			LayoutTransform=new ScaleTransform(zoom,zoom);
		}



		#endregion Methods - private

		
	}
}
