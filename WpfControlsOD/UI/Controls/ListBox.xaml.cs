using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.
This listBox is written from scratch in a way that should exactly duplicate the OpenDental.UI.ListBox.
Compared to ComboBox, this is optimized for smaller lists that can all fit inside a form without scrolling. Enums are the typical example.
(todo: implement arrow up/down keys)

Code Examples:
ENUM---------------------------------------------------------------------------------------------------------------
listArea.Items.AddEnums<EnumArea>();
Or, rarely: listArea.Items.Add(Lans.g("enumArea","First Item"),EnumArea.FirstItem);
listArea.SetSelected((int)proc.Area);
or
listArea.SetSelectedEnum(proc.Area);//type is inferred 
...
proc.Area=listArea.GetSelected<EnumArea>();

Other db table types----------------------------------------------------------------------------------------------
listObj.Items.Clear();//skip in Load()
listObj.Items.AddNone<ObjType>();//optional
listObj.Items.AddList(listObjs,x=>x.LName);//the abbr parameter is usually skipped. <T> is inferred.
listObj.SetSelectedKey<ObjType>(adjustment.ObjNum,x=>x.ObjNum,x=>Objs.GetName(x)); 
...
These are some ways to get a selected primary key:
adjustment.ObjNum=listObj.GetSelected<ObjType>().ObjNum;
adjustment.ObjNum=((ObjType)listObj.SelectedItem).ObjNum;
adjustment.ObjNum=_listObjs[listObjs.SelectedIndex].ObjNum;e select ListBoxes do not allow deselection for this reason.
The following approach won't crash, but a -1 result might be nonsensical and not allowed in db.
adjustment.ObjNum=listObj.GetSelectedKey<ObjType>(x=>x.ObjNum);
*/

	///<summary></summary>
	public partial class ListBox : UserControl{
		#region Fields
		private string _itemStrings="";
		private int _hoverIndex=-1;
		///<summary>When we capture mouse, it causes a mouse move event that messes up our logic. This allows us to ignore that event for that one line.</summary>
		private bool _ignoreMouseMove;
		///<summary>This is the only internal storage for tracking selected indices.  All properties refer to this same list. Don't change this list from outside of the three big properties unless you also refresh the UI.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		///<summary>On mouse down, this copy is made.  Use as needed for logic.  No need to clear when done.</summary>
		private List<int> _listSelectedOrig=new List<int>();
		private int _mouseDownIndex;
		///<summary>This gets set when the user sets an item that is not present in the list. Selected index is also set to -1.
		private string _overrideText="";
		///<summary>If selected index is -1, this can be used to store and retrieve the primary key. _overrideText is what shows to the user.</summary>
		private long _selectedKey=0;
		private SelectionMode _selectionMode=SelectionMode.One;
		private Color _colorSelectedBack=Color.FromRgb(186,199,219);//#BAC7DB, grid default was Silver: C0C0C0 (192 gray)
		private Color _colorBack=Colors.White;
		#endregion Fields

		#region Constructor
		public ListBox(){
			InitializeComponent();
			//Width=150;
			//Height=200;
			scrollViewer.CanContentScroll=false;//results in physical(pixel) scrolling instead of logical(Item) scrolling.
			Items=new ListBoxItemCollection(this);
			IsEnabledChanged+=ListBox_IsEnabledChanged;
		}
		
		#endregion Constructor

		#region Events - Public Raise
		///<summary>Occurs when user selects item(s) from the list. Will fire only when selected indices actually change.  If it must fire on click even if the items is already selected, then use MouseClick or similar.</summary>
		[Category("OD")]
		[Description("Occurs when user selects item(s) from the list. Will fire only when selected indices actually change.  If it must fire on click even if the item is already selected, then use MouseClick or similar.")]
		public event EventHandler SelectionChangeCommitted;

		///<summary>Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.</summary>
		[Category("OD")]
		[Description("Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events - Public Raise

		#region Properties
		[Category("OD")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		///Sets the background color for the entire listbox.
		public Color ColorBack {
			get{
				return _colorBack;
			}
			set{
				_colorBack=value;
				SetColors();
			}
		}

		[Category("OD")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		///Sets the highlight color for the selected item in that listbox.
		public Color ColorSelectedBack {
			get{
				return _colorSelectedBack;
			}
			set{
				_colorSelectedBack=value;
				SetColors();
			}
		}

		[Category("OD")]
		[DefaultValue(true)]
		public new bool IsEnabled{
			//This doesn't actually ever get hit. 
			//It's just here to move IsEnabled down into the OD category.
			get{
				return base.IsEnabled;
			}
			set{
				base.IsEnabled=value;
			}
		}

		[Category("OD")]
		[Description("Comma delimited list of items. Only intended for use in design time.")]
		[DefaultValue("")]
		public string ItemStrings{
			get{
				return _itemStrings;
			}
			set{
				_itemStrings=value;
				List<string> listStrings=_itemStrings.Split(',').ToList();
				Items.Clear();
				Items.AddList(listStrings,x=>x.ToString());
			}
		}
			
		public ListBoxItemCollection Items { get;	} //had to be initialized in constructor

		///<summary>Gets or sets the selected index. Setter has same behavior for SelectionMode.MultiExtended or not. Get throws exception for SelectionMode.MultiExtended.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex { 
			get{
				if(SelectionMode==SelectionMode.MultiExtended){
					throw new Exception("SelectedIndex.Get is ambiguous when SelectionMode.MultiExtended.");
				}
				if(SelectionMode==SelectionMode.CheckBoxes){
					throw new Exception("SelectedIndex.Get is ambiguous when SelectionMode.CheckBoxes.");
				}
				if(_listSelectedIndices.Count==0){
					return -1;
				}
				return _listSelectedIndices[0];
			}
			set{
				if(SelectionMode==SelectionMode.None) {
					throw new Exception("SelectedIndex is not allowed when SelectionMode.None.");
				}
				if(SelectionMode==SelectionMode.CheckBoxes) {
					throw new Exception("SelectedIndex is not implemented for SelectionMode.CheckBoxes.");
				}
				if(value<-1 || value>Items.Count-1){
					return;//ignore out of range
				}
				_listSelectedIndices.Clear();
				_overrideText="";
				if(value!=-1){
					_listSelectedIndices.Add(value);
				}
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetColors();
			}
		} 

		///<summary>Gets or sets one selected object. Get throws exception for SelectionMode.MultiExtended.</summary>
		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedItem {
			get{
				if(!DesignerProperties.GetIsInDesignMode(this) && SelectionMode==SelectionMode.MultiExtended){
					throw new Exception("SelectedItem.Get is ambiguous when SelectionMode.MultiExtended.");
				}
				if(!DesignerProperties.GetIsInDesignMode(this) && SelectionMode==SelectionMode.CheckBoxes){
					throw new Exception("SelectedItem.Get is ambiguous when SelectionMode.CheckBoxes.");
				}
				if(_listSelectedIndices.Count==0){
					return default;
				}
				object item=Items.GetObjectAt(_listSelectedIndices[0]);
				if(item==null) {
					item=Items.GetTextShowingAt(_listSelectedIndices[0]);
				}
				return item;
			}
			set{
				if(!DesignerProperties.GetIsInDesignMode(this) && SelectionMode==SelectionMode.None) {
					throw new Exception("SelectedItem is not allowed when SelectionMode.None.");
				}
				if(!DesignerProperties.GetIsInDesignMode(this) && SelectionMode==SelectionMode.CheckBoxes) {
					throw new Exception("SelectedItem is not implemented for SelectionMode.CheckBoxes.");
				}
				if(value==null) {
					//deselect, just like MS behavior
					if(_listSelectedIndices.Count==0) {
						return;
					}
					_listSelectedIndices.Clear();
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					SetColors();
					return;
				}
				int index=-1;
				for(int i=0; i<Items.Count;i++) {
					if(value is string) {
						if((string)value!=Items.GetTextShowingAt(i)) {
							continue;
						}
					}
					else {
						if(value!=Items.GetObjectAt(i)) {
							continue;
						}
					}
					index=i;
					break;
				}
				if(index==-1) {
					return;//ignore an invalid set value
				}
				if(_listSelectedIndices.Contains(index)) {
					return;//already selected
				}
				_listSelectedIndices.Clear();//since the name of this method is singular.
				_listSelectedIndices.Add(index);
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetColors();
				return;
			}
		}

		///<summary>Gets or sets the selected indices. Getter has same behavior for SelectionMode.MultiExtended or not.  Set throws exception if not SelectionMode.MultiExtended.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> SelectedIndices { 
			get{
				//returns a value, not a reference, so you can't clear from here.
				return _listSelectedIndices;
			}
			set{
				if(SelectionMode==SelectionMode.None) {
					throw new Exception("SelectedIndices is not allowed when SelectionMode.None.");
				}
				if(SelectionMode==SelectionMode.CheckBoxes) {
					throw new Exception("SelectedIndices is not implemented for SelectionMode.CheckBoxes.");//because not needed
				}
				if(SelectionMode!=SelectionMode.MultiExtended){
					throw new Exception("Cannot set SelectedIndices when not SelectionMode.MultiExtended. Use SelectedIndex, SetSelected, etc.");
				}
				_listSelectedIndices.Clear();
				for(int i = 0;i<value.Count;i++) {
					if(value[i]<-1 || value[i]>Items.Count-1){
						continue;//ignore out of range
					}
					_listSelectedIndices.Add(value[i]);
				}
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetColors();
			}
		} 

		[Category("OD")]
		[Description("Set to allow none, single, multiple, or checkboxes.")]
		[DefaultValue(SelectionMode.One)]
		public SelectionMode SelectionMode { 
			get{
				return _selectionMode;
			}
			set{
				_selectionMode=value;
				if(_selectionMode==SelectionMode.CheckBoxes){
					scrollViewer.HorizontalScrollBarVisibility=ScrollBarVisibility.Auto;
				}
				else{
					scrollViewer.HorizontalScrollBarVisibility=ScrollBarVisibility.Disabled;
				}
			} 
		}

		//[Category("OD")]
		//[DefaultValue(int.MaxValue)]
		//[Description("Use this instead of TabIndex.")]
		//public int TabIndexOD{
			 //TabIndex is just for textboxes for now.
		//}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}
		#endregion Properties

		#region Methods - event handlers, mouse
		private void CheckBox_Click(object sender,EventArgs e) {
			//checkbox value already changed.
			FillSelectedIndicesFromCheckBoxes();
		}

		private void Item_MouseLeave(object sender,MouseEventArgs e) {
			_hoverIndex=-1;
			SetColors();
		}

		private void Item_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			//Focus();
			if(SelectionMode==SelectionMode.None
				|| SelectionMode==SelectionMode.CheckBoxes) //handled instead with checkbox_click event
			{
				return;
			}
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			_ignoreMouseMove=true;
			((IInputElement)sender).CaptureMouse();
			_ignoreMouseMove=false;
			Point point=e.GetPosition(this);
			_mouseDownIndex=IndexFromPoint(point); 
			if(_mouseDownIndex>Items.Count-1){//clicked below items
				SetColors();
				return;//don't deselect anything, but preserve the clicked index in case they drag up
			}
			_listSelectedOrig=new List<int>(_listSelectedIndices);//for both multi and single, we need to remember this
			if(SelectionMode==SelectionMode.MultiExtended) {
				CalcSelectedIndices();
			}
			else{//single
				_listSelectedIndices=new List<int>(){_mouseDownIndex};
			}
			if(!_listSelectedOrig.SequenceEqual(_listSelectedIndices)) {
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
			SetColors();
			MouseButtonEventArgs mouseButtonEventArgs=new MouseButtonEventArgs(e.MouseDevice,e.Timestamp,MouseButton.Left);
			mouseButtonEventArgs.RoutedEvent=MouseDownEvent;
			RaiseEvent(mouseButtonEventArgs);
			//because OnMouseDown(e) wasn't working.
			//At end so that index will be selected before mouse down fires somewhere else (e.g. FormApptEdit).  Matches MS.
			//but, sometimes, if an event from above resulted in a dialog, then there will be no mouse up event.  Handle that below.
			//Not necessary because we improved this WPF version to use local isMouseDown variables.
			//MouseButtons mouseButtons=Control.MouseButtons;//introducing variable for debugging because this state is not preserved at break points.
			//if(mouseButtons==MouseButtons.None){
			//	_isMouseDown=false;
			//}
		}

		private void Item_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			((IInputElement)sender).ReleaseMouseCapture();
			if(SelectionMode==SelectionMode.None
				|| SelectionMode==SelectionMode.CheckBoxes) 
			{
				return;
			}
			//_isMouseDown=false;
			SetColors();
		}

		private void Item_MouseMove(object sender,MouseEventArgs e) {
			if(_ignoreMouseMove){
				return;
			}
			if(SelectionMode==SelectionMode.CheckBoxes) {
				//we don't want any hover effect other than what the checkboxes supply
				return;
			}
			Point point=e.GetPosition(this);
			_hoverIndex=IndexFromPoint(point);
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(!isMouseDown) {
				SetColors();
				return;
			}
			double y=e.GetPosition(this).Y;
			//from here down, dragging
			//If user drags outside the control, then it will start scrolling, but it's in a fast loop.
			//We will have two different speeds of scrolling: slow within 10 pixels, and faster when more than that.
			if(y<-10) {//above, fast
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset-1);
				Thread.Sleep(10);
			}
			else if(y<0) {//above, slow
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset-1);
				Thread.Sleep(30);
			}
			else if(y>Height+10) {//below fast
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset+1);
				Thread.Sleep(10);
			}
			else if(y>Height) {//below slow
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset+1);
				Thread.Sleep(30);
			}
			List<int> listSelectedOrig=new List<int>(_listSelectedIndices);//for both multi and single, we need to remember this. Just local.
			if(SelectionMode==SelectionMode.MultiExtended) {
				CalcSelectedIndices();
			}
			else{//single select while mouse down
				//only add valid indices
				if(_hoverIndex>=0 && _hoverIndex<Items.Count) {
					_listSelectedIndices=new List<int>(){_hoverIndex};
				}
			}
			if(!listSelectedOrig.SequenceEqual(_listSelectedIndices)){//just comparing to the local list for this move
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
			SetColors();
		}
		#endregion Methods - event handlers, mouse

		#region Methods - Public
		///<summary>Same as using SetAll(false), but this is here to mirror MS listBoxes.</summary>
		public void ClearSelected(){
			_listSelectedIndices.Clear();
			SelectedIndexChanged?.Invoke(this, new EventArgs());
			SetColors();
		}

		///<summary>Returns a list of the selected objects. List can be empty.  Items can be null.</summary>
		public List<T> GetListSelected<T>() {
			List<T> listSelected=new List<T>();
			for(int i=0;i<_listSelectedIndices.Count;i++){
				listSelected.Add((T)Items.GetObjectAt(_listSelectedIndices[i]));
			}
			return listSelected;
		}

		///<summary>Gets the selected object.  Can be null for object or 0 for enum.  Throws exception for SelectionMode.MultiExtended.</summary>
		public T GetSelected<T>() {
			if(SelectionMode==SelectionMode.MultiExtended){
				throw new Exception("GetSelected is ambiguous when SelectionMode.MultiExtended.");
			}
			if(SelectionMode==SelectionMode.CheckBoxes){
				throw new Exception("GetSelected is ambiguous when SelectionMode.CheckBoxes.");
			}
			if(_listSelectedIndices.Count==0){
				return default;//usually null. For an enum, this will be item 0.
			}
			try {
				return (T)Items.GetObjectAt(_listSelectedIndices[0]);
			} 
			catch {
				return default;
			}
		}

		///<summary>Gets the key like PatNum from the selected index. funcSelectKey example x=>x.PatNum.  If selected index is -1, it will try to grab the key that was passed in earlier with SetSelectedKey.  If there is none, then it will return 0.</summary>
		public long GetSelectedKey<T>(Func<T,long> funcSelectKey){
			if(SelectionMode==SelectionMode.MultiExtended){
				throw new Exception("GetSelected is ambiguous when SelectionMode.MultiExtended.");
			}
			if(SelectionMode==SelectionMode.CheckBoxes){
				throw new Exception("GetSelected is ambiguous when SelectionMode.CheckBoxes.");
			}
			if(_listSelectedIndices.Count==0){
				return _selectedKey;//could be zero
			}
			if(Items.GetObjectAt(_listSelectedIndices[0])==null){//just in case
				return 0;
			}
			return funcSelectKey((T)Items.GetObjectAt(_listSelectedIndices[0]));
		}

		///<summary>Gets a string of all selected items, separated by commas and spaces. Example: item1, item2, ...</summary>
		public string GetStringSelectedItems(bool useAbbr=false) {
			//works the same for SelectionMode.MultiExtended or not
			string retVal="";
			for(int i=0;i<_listSelectedIndices.Count;i++){
				if(i>0){
					retVal+=", ";
				}
				if(useAbbr){
					retVal+=Items.GetAbbrShowingAt(_listSelectedIndices[i]);
				}
				else{
					retVal+=Items.GetTextShowingAt(_listSelectedIndices[i]);
				}
			}
			return retVal;
		}

		///<summary>Gets the index at the specified point. Returns -1 if no index can be selected at that point.</summary>
		public int IndexFromPoint(int x,int y) {
			return IndexFromPoint(new Point(x,y));
		}

		///<summary>Gets the index at the specified point. Returns -1 if no index can be selected at that point. Pass in the y pos relative to this control. It can fall outside the control when dragging.</summary>
		public int IndexFromPoint(Point point){
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

		///<summary>Sets all rows either selected or unselected.</summary>
		public void SetAll(bool setToValue){
			if(SelectionMode==SelectionMode.None) {
				throw new Exception("SetAll is not allowed when SelectionMode.None.");
			}
			_listSelectedIndices.Clear();
			if(setToValue){//if setting all true
				for(int i=0;i<Items.Count;i++){
					_listSelectedIndices.Add(i);
				}
			}
			if(SelectionMode==SelectionMode.CheckBoxes) {
				FillCheckBoxesFromSelectedIndices();
			}
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			SetColors();
		}

		///<summary>Sets one row either selected or unselected.</summary>
		public void SetSelected(int index,bool value=true){
			if(SelectionMode==SelectionMode.None) {
				throw new Exception("SetSelected is not allowed when SelectionMode.None.");
			}
			if(value){//setting true
				if(_listSelectedIndices.Contains(index)){
					return;//nothing to do
				}
				if(SelectionMode==SelectionMode.One){
					_listSelectedIndices.Clear();
				}
				_listSelectedIndices.Add(index);
			}
			else{//setting false
				if(!_listSelectedIndices.Contains(index)){
					return;//nothing to do
				}
				_listSelectedIndices.Remove(index);
			}
			if(SelectionMode==SelectionMode.CheckBoxes) {
				FillCheckBoxesFromSelectedIndices();
			}
			//SetVScrollValue();//overkill. These don't often scroll or get set externally
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			SetColors();
		}

		///<summary>Really only needed if enum is not 0,1,2,... or if enums were manually added to the list, ignoring their normal idx order.  Otherwise, you could also use SetSelected((int)enumVal);  If the enum val is not present in the listBox, it will do nothing.</summary>
		public void SetSelectedEnum<T>(T enumVal){
			if(SelectionMode==SelectionMode.None) {
				throw new Exception("SetSelectedEnum is not allowed when SelectionMode.None.");
			}
			if(SelectionMode==SelectionMode.CheckBoxes) {
				throw new Exception("SetSelected is not implemented for SelectionMode.CheckBoxes.");
			}
			int idx=-1;
			for(int i=0;i<Items.Count;i++){
				if(enumVal.Equals((T)Items.GetObjectAt(i))){
					idx=i;
					break;
				}
			}
			if(idx==-1){
				//don't do anything.  We don't want to deselect an enum because that's unexpected.
				return;
			}
			SetSelected(idx);
		}

		///<summary>Uses key like PatNum to set the selected index. funcSelectKey is a func that gets the key from objects in the list for comparison. If the item is not currently in the listBox and can't be matched, then selectedIndex is set to -1, and the key is stored internally for reference. It will remember that key and return it back in any subsequent GetSelectedKey as long as selectedIndex is still -1. In that case, it also needs text to display, which is set using funcOverrideText. This all works even if the key is garbage, in which case, it will show the key number in the listbox.  If you are confident that your key is going to be in the listBox, you can omit funcOverrideText, with the worst consequence that the display might be a number.</summary>
		///<param name="funcSelectKey">Examples: x=>x.PatNum</param>
		///<param name="funcOverrideText">Examples:  x=>Carriers.GetName(x), or x=>"none"</param>
		public void SetSelectedKey<T>(long key,Func<T,long> funcSelectKey,Func<long,string> funcOverrideText=null){
			if(SelectionMode==SelectionMode.None) {
				throw new Exception("SetSelectedKey is not allowed when SelectionMode.None.");
			}
			if(SelectionMode==SelectionMode.CheckBoxes) {
				throw new Exception("SetSelectedKey is not implemented for SelectionMode.CheckBoxes.");
			}
			_listSelectedIndices.Clear();
			_overrideText="";
			_selectedKey=0;
			for(int i=0;i<Items.Count;i++) {
				if(Items.GetObjectAt(i)==null){
					continue;
				}
				if(typeof(T)!=Items.GetObjectAt(i).GetType()) {
					continue;
				}
				if(funcSelectKey((T)Items.GetObjectAt(i))==key) {
					_listSelectedIndices.Add(i);
					//SetVScrollValue();
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					SetColors();
					return;
				}
			}
			//couldn't locate key in list
			if(funcOverrideText==null){
				_overrideText=key.ToString();
			}
			else{
				_overrideText=funcOverrideText(key);
			}
			if(_overrideText==null || _overrideText==""){
				_overrideText=key.ToString();//show the number because we don't want to show nothing
			}
			_selectedKey=key;
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			SetColors();
		}
		#endregion Methods - Public

		#region Methods - private
		///<summary>Called on mouse down and mouse move.  Recalculates the _listSelectedIndices, based on _listSelectedOrig, _mouseDownIndex, _hoverIndex, ShiftIsDown, and ControlIsDown.  By looping through all items in entire list each time.</summary>
		private void CalcSelectedIndices() {
			bool isShiftDown=Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			if(isShiftDown) {
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
				return;//If shift is down, ctrl gets ignored
			}
			bool isControlDown=Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			_listSelectedIndices.Clear();
			for(int i=0;i<Items.Count; i++) {
				bool isInRange=false;
				if(i>=_mouseDownIndex && i<=_hoverIndex) { //Mouse is lower than start
					isInRange=true;
				}
				if(i<=_mouseDownIndex && i>=_hoverIndex) {//Mouse is higher than start
					isInRange=true;
				}
				if(isControlDown) {
					if(isInRange) {
						if(!_listSelectedOrig.Contains(_mouseDownIndex)) {
							_listSelectedIndices.Add(i);//same selection as initial selected index
						}
					}
					else {//out of range
						if(_listSelectedOrig.Contains(i)) {
							_listSelectedIndices.Add(i);//keep original selection
						}
					}
					continue;
				}
				//ctrl not down:
				if(isInRange) {
					_listSelectedIndices.Add(i);
				}
			}
		}
		/*
		//this worked, but I need pixel control, not just item level control
		///<summary>Pass in the object where the event originated from. Return idx of that row, or -1 if no row.</summary>
		private int HitTest(object objectOriginalSource){
			DependencyObject dependencyObject=(DependencyObject)objectOriginalSource;
			while(true){
				if(dependencyObject is null){
					break;
				}
				if(dependencyObject is Border){
					break;
				}
				dependencyObject=VisualTreeHelper.GetParent(dependencyObject);
			}
			if(dependencyObject is null){
				return -1;
			}
			Border border=(Border)dependencyObject;
			if(!stackPanel.Children.Contains(border)){
				return -1;
			}
			int idx=stackPanel.Children.IndexOf(border);
			return idx;
		}*/

		private void FillCheckBoxesFromSelectedIndices(){
			for(int i=0;i<stackPanel.Children.Count;i++){
				Border border=(Border)stackPanel.Children[i];
				CheckBox checkBox=(CheckBox)border.Child;
				if(_listSelectedIndices.Contains(i)){
					checkBox.Checked=true;
				}
				else{
					checkBox.Checked=false;
				}
			}
		}

		private void FillSelectedIndicesFromCheckBoxes(){
			_listSelectedIndices=new List<int>();
			for(int i=0;i<stackPanel.Children.Count;i++){
				Border border=(Border)stackPanel.Children[i];
				CheckBox checkBox=(CheckBox)border.Child;
				if(checkBox.Checked==true){
					_listSelectedIndices.Add(i);
				}
			}
		}

		private void ListBox_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		///<summary>Sets background colors for all rows, based on selected indices and hover.</summary>
		private void SetColors(){
			if(SelectionMode==SelectionMode.CheckBoxes){
				return;
			}
			//I'm really not sure how the textBlocks get greyed out for IsEnabled.
			//Seems to work automatically, so I won't complain.
			for(int i=0;i<stackPanel.Children.Count;i++){
				Color colorBack=_colorBack;
				if(_listSelectedIndices.Contains(i)){
					if(IsEnabled){
						colorBack=_colorSelectedBack;
					}
					else{
						colorBack=Color.FromRgb(230,230,230);
					}
				}
				else if(_hoverIndex==i){
					colorBack=Color.FromRgb(229,239,251);//#E5EFFB
				}
				Border border2=(Border)stackPanel.Children[i];
				border2.Background=new SolidColorBrush(colorBack);
			}
			//if(IsEnabled){//change border?
		}
		#endregion Methods - private

		#region Class - ListBoxItemCollection
		///<summary>Nested class for the collection of items.  Each item must be a ListBoxItem, not null.  Each field of a ListBoxItem may be null.</summary>
		public class ListBoxItemCollection{
			///<summary>The listBox that this collection is attached to.</summary>
			private ListBox _listBoxParent;
			///<summary>The internal list of items, exposed through methods.</summary>
			private List<ListBoxItem> _listListBoxItems;

			public ListBoxItemCollection(ListBox listBoxParent){
				_listBoxParent=listBoxParent;
				_listListBoxItems=new List<ListBoxItem>();
			}

			///<summary>Specify the text to show. Optionally, specify the object represented by that text. Also, optionally display an abbreviation for each item to display in the selected summary above.</summary>
			public void Add(string text,object item=null,string abbr=null){
				if(item is null) {
					item=text;
				}
				_listListBoxItems.Add(new ListBoxItem(text,item,abbr));
				AddItemToUI(text);
			}

			///<summary>Adds the values of an enum to the list of Items.  Does not Clear first.  Descriptions are pulled from DescriptionAttribute or .ToString, then run through translation.  If you want add only some enums, or in a different order, or display ShortDescriptionAttribute, you have to add the Enums individually with your own text.</summary>
			public void AddEnums<T>() where T : Enum {//struct,IConvertible{
				AddList(Enum.GetValues(typeof(T)).Cast<T>(),x=>Lans.g("enum"+typeof(T).Name,GetEnumDescription(x)));
			}

			public bool Contains<T>(T item) {
				for(int i=0; i<_listListBoxItems.Count;i++) {
					if(item is string) {
						if(_listListBoxItems[i].Text.Equals(item)) {
							return true;
						}
					}
					else {
						if(_listListBoxItems[i].Item.Equals(item)) {//this would fail if this method was not generic
							return true;
						}
					}
				}
				return false;
			}

			///<summary>A copy of the extension method in ODPrimitiveExtensions, except without ShortDescriptionAttribute. </summary>
			private string GetEnumDescription(Enum value) {
				Type type = value.GetType();
				string name = Enum.GetName(type,value);
				if(name==null) {
					return value.ToString();
				}
				FieldInfo fieldInfo = type.GetField(name);
				if(fieldInfo==null) {
					return value.ToString();
				}
				DescriptionAttribute attr=(DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo,typeof(DescriptionAttribute));
				if(attr==null) {
					return value.ToString();
				}
				return attr.Description;
			}

			public void AddList(List<ListBoxItem> listListBoxItems){
				_listListBoxItems.AddRange(listListBoxItems);
				for(int i=0;i<listListBoxItems.Count;i++){
					AddItemToUI(listListBoxItems[i].Text);
				}
			}

			///<summary>Adds a collection to the items. Does not Clear first.  funcItemToString specifies a string to be displayed for this item, example x=>x.LName or x=>x.ToString(). funcItemToAbbr is similar for the abbreviation used for summary at top of list and for GetStringSelectedItems.  See bottom of this file for example.</summary>
			public void AddList<T>(IEnumerable<T> items,Func<T,string> funcItemToString,Func<T,string> funcItemToAbbr=null){
				//It was too dangerous to make this optional.  People forget.  The string to show needs to be required.
				//funcItemToString=funcItemToString??(x => x.ToString());
				string abbr=null;
				foreach(T item in items) {//cannot apply indexing to items because of T
					if(funcItemToAbbr!=null){
						abbr=funcItemToAbbr(item);
					}
					_listListBoxItems.Add(new ListBoxItem(funcItemToString(item),item,abbr));
					AddItemToUI(funcItemToString(item));
				}
			}

			///<summary>Adds a dummy object called "None", with a key of 0.</summary>
			public void AddNone<T>() where T:new(){
				Add("None",new T());
			}

			///<summary>Adds a collection of strings to the items. Does not Clear first.</summary>
			public void AddStrings(IEnumerable<string> items){
				for(int i=0;i<items.Count();i++) {
					_listListBoxItems.Add(new ListBoxItem(items.ElementAt(i),items.ElementAt(i)));
					AddItemToUI(items.ElementAt(i));
				}
			}

			public void Clear(){
				if(_listListBoxItems.Count==0){
					return;//prevent premature firing of SelectedIndexChanged
				}
				bool isSelected=false;
				if(_listBoxParent._listSelectedIndices.Count>0){
					isSelected=true;
				}
				_listListBoxItems.Clear();
				_listBoxParent.stackPanel.Children.Clear();
				_listBoxParent._listSelectedIndices.Clear();
				_listBoxParent.SetColors();
				if(isSelected){
					_listBoxParent.SelectedIndexChanged?.Invoke(this,new EventArgs());
				}
			}

			public int Count{
				get => _listListBoxItems.Count;
			}

			///<summary>Can't use this because it's unclear if you want the entire ListBoxItem, the text, or the object.  Use a Get method instead.</summary>
			[Obsolete("Can't use this because it's unclear if you want the entire ListBoxItem, the text, or the object.  Use a Get method instead.")]
			public object this[int index]{
				get { 
					return null;
				}
			}

			///<summary>This gives you the entire ListBoxItem.  If you just want the Item object, use GetObjectAt(i). Can be null.</summary>
			public ListBoxItem GetListBoxItemAt(int index){
				if(index>=_listListBoxItems.Count){
					return null;
				}
				return _listListBoxItems[index];
			}

			///<summary>This gives you just your Object in the list, not the whole ListBoxItem container.  You already know what type of object this is, so just cast the type as needed. Can be null</summary>
			public object GetObjectAt(int index){
				if(index>=_listListBoxItems.Count){
					return null;
				}
				return _listListBoxItems[index].Item;
			}

			///<summary>Returns a list of all objects in the list of items, not just the selected ones.</summary>
			public List<T> GetAll<T>(){
				List<T> listT=new List<T>();
				for(int i=0;i<_listListBoxItems.Count;i++){
					listT.Add((T)_listListBoxItems[i].Item);
				}
				return listT;
			}

			///<summary>Tries to use, in this order: Abbr, Text, Item.ToString(). If all are null, returns "null".</summary>
			public string GetAbbrShowingAt(int index){
				if(index>=_listListBoxItems.Count){
					return "null";
				}
				if(_listListBoxItems[index].Abbr==null && _listListBoxItems[index].Text==null && _listListBoxItems[index].Item==null){
					return "null";
				}
				if(_listListBoxItems[index].Abbr!=null){
					return _listListBoxItems[index].Abbr;
				}
				if(_listListBoxItems[index].Text!=null){
					return _listListBoxItems[index].Text;
				}
				return _listListBoxItems[index].Item.ToString();
			}

			///<summary>Tries to use, in this order: Text, Item.ToString(). If both are null, returns "null".</summary>
			public string GetTextShowingAt(int index){
				if(index>=_listListBoxItems.Count){
					return "null";
				}
				if(_listListBoxItems[index].Text==null && _listListBoxItems[index].Item==null){
					return "null";
				}
				if(_listListBoxItems[index].Text!=null){
					return _listListBoxItems[index].Text;
				}
				return _listListBoxItems[index].Item.ToString();
				
			}

			///<summary>Removes the item at the passed in index.</summary>
			public void RemoveAt(int index) {
				if(index>=_listListBoxItems.Count || index==-1) {
					return;
				}
				_listListBoxItems.RemoveAt(index);
				_listBoxParent.stackPanel.Children.RemoveAt(index);
			}

			///<summary>Rarely used. You can change the value of an existing item in the list.</summary>
			public void SetValue(int index,object value,bool setText=true) {
				if(index>=_listListBoxItems.Count || index==-1) {
					return;
				}
				_listListBoxItems[index].Item=value;
				if(setText){
					_listListBoxItems[index].Text=value.ToString();
					Border border=(Border)_listBoxParent.stackPanel.Children[index];
					((TextBlock)border.Child).Text=value.ToString();
				}
			}

			private void AddItemToUI(string text){
				Border border=new Border();
				if(_listBoxParent.SelectionMode==SelectionMode.CheckBoxes){
					CheckBox checkBox=new CheckBox();
					checkBox.Text=text;
					//checkBox.Width=5000;//to prevent wrapping
					checkBox.Margin=new Thickness(left:2,0,0,0);
					checkBox.Click+=_listBoxParent.CheckBox_Click;
					border.Child=checkBox;
				}
				else{
					TextBlock textBlock=new TextBlock();
					textBlock.Text=text;
					textBlock.Margin=new Thickness(2,0,0,0);
					border.Child=textBlock;
				}
				/*System.Windows.Controls.Grid grid=new System.Windows.Controls.Grid();
				border.Child=grid;
				ColumnDefinition columnDefinition1=new ColumnDefinition();
				GridLength gridLength1=new GridLength(1,GridUnitType.Auto);//checkbox might be present or absent.
				columnDefinition1.Width=gridLength1;
				grid.ColumnDefinitions.Add(columnDefinition1);
				ColumnDefinition columnDefinition2=new ColumnDefinition();
				GridLength gridLength2=new GridLength(1,GridUnitType.Star);//text is remainder
				columnDefinition2.Width=gridLength2;
				grid.ColumnDefinitions.Add(columnDefinition2);
				if(_listBoxParent.SelectionMode==SelectionMode.CheckBoxes){
					CheckBox checkBox=new CheckBox();
					//System.Windows.Controls.Grid.SetColumn(checkBox,0);
					grid.Children.Add(checkBox);
				}*/
				border.MouseLeave+=_listBoxParent.Item_MouseLeave;
				border.MouseLeftButtonDown+=_listBoxParent.Item_MouseLeftButtonDown;
				border.MouseLeftButtonUp+=_listBoxParent.Item_MouseLeftButtonUp;
				border.MouseMove+=_listBoxParent.Item_MouseMove;
				_listBoxParent.stackPanel.Children.Add(border);
			}
		}
		#endregion Class - ListBoxItemCollection
	}

	#region Class - ListBoxItem
	///<summary>Storage for an item in a ListBox.    This class also allows you to create items such as ListBoxItem or List&lt;ListBoxItem&gt; from outside the listBox and pass them in later.  If Text is null, Item.ToString() will be displayed.</summary>
	public class ListBoxItem{

		public ListBoxItem(){
			
		}

		///<summary>Text shows in list, item is any full object of interest, and abbr is used for displaying summary of selected items at top.</summary>
		public ListBoxItem(string text,object item=null,string abbr=null){
			Text=text;
			Item=item;
			Abbr=abbr;
		}

		///<summary></summary>
		public string Text{ get; set; } = null;

		///<summary></summary>
		public object Item{ get; set; } = null;

		///<summary></summary>
		public string Abbr{ get; set; } = null;
	}
	#endregion Class - ListBoxItem

	#region Enum - SelectionMode
	public enum SelectionMode {
		None,
		One,
		MultiExtended,
		CheckBoxes
	}
	#endregion
}
