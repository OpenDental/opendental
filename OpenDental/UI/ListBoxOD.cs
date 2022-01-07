using Health.Direct.Common.Extensions;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace OpenDental.UI {
	//Jordan is the only one allowed to edit this file.
	//Problems with stock listBox that we needed to solve: Color highlight was stupid blue, no hover effect, flicker on resize, poor high dpi support.
	//This will also finally allow us to get rid of all the CodeBase.(Utilities).ODUIExtensions listBox extension methods.

	///<summary>A listBox designed to replace the stock MS listBox throughout OD. This supports multi-select or single-select.  It stores objects, paired with their display strings.  It has special handling for Enums.  Unlike ComboBoxOD, there is no special support for Providers, Defs, or "All".  If you want "All", consider a ComboBoxOD.  It handles setting to a value that is not in the list.  You can manually add a none/0 item to any listBox.  See the bottom of this file for examples of how to use.</summary>
	public partial class ListBoxOD : Control {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private Static
		//These colors etc are static and shared among all listboxes for life of program. Not disposed
		///<summary>A compromise between the absurd MS blue and boring gray.  This looks great for enabled and disabled.</summary>
		private static SolidBrush _brushSelectedBack=new SolidBrush(Color.FromArgb(186,199,219));//grid default is Silver: C0C0C0 (192 gray)
		private static SolidBrush _brushHover=new SolidBrush(ColorOD.Hover);
		#endregion Fields - Private Static

		#region Fields - Private
		///<summary>Based on Font. 13 at 96dpi.</summary>
		private int _heightLine=13;
		private int _hoverIndex=-1;
		///<summary>Usually 0.  If scrolled, then this would be the top item showing.</summary>
		private int _indexTopShowing=0;
		private bool _isMouseDown;
		///<summary>On mouse down, this copy is made.  Use as needed for logic.  No need to clear when done.</summary>
		private List<int> _listSelectedOrig=new List<int>();
		///<summary></summary>
		private int _mouseDownIndex;
		///<summary>This gets set when the user sets an item that is not present in the list. Selected index is also set to -1.
		private string _overrideText="";
		///<summary>If selected index is -1, this can be used to store and retrieve the primary key. _overrideText is what shows to the user.</summary>
		private long _selectedKey=0;
		private System.Windows.Forms.VScrollBar vScroll;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private string[] _itemStrings=null;
		///<summary>This is the only internal storage for tracking selected indices.  All properties refer to this same list.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		#endregion Fields - Private for Properties

		#region Constructors
		public ListBoxOD() {
			vScroll=new VScrollBar();
			vScroll.Scroll+=VScroll_Scroll;
			vScroll.KeyDown+=VScroll_KeyDown;
			vScroll.Minimum=0;
			this.Controls.Add(vScroll);
			vScroll.Visible=false;
			vScroll.Enabled=false;
			InitializeComponent();
			DoubleBuffered=true;
			Size=new Size(120,95);//same as default
			Items=new ListBoxItemCollection(this);
			_heightLine=Font.Height;
		}
		#endregion Constructors

		#region Events - Public Raise
		///<summary>Occurs when user selects item(s) from the list. Will fire only when selected indices actually change.  If it must fire on click, use MouseClick or similar.</summary>
		[Category("OD")]
		[Description("Occurs when user selects item(s) from the list. Will fire only when selected indices actually change.  If it must fire on click, use MouseClick or similar.")]
		public event EventHandler SelectionChangeCommitted;

		///<summary>Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.</summary>
		[Category("OD")]
		[Description("Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events - Public Raise

		#region Methods - Event Handlers - Key Press
		protected override void OnKeyDown(KeyEventArgs e) {
			//e.Handled=true;
			base.OnKeyDown(e);
			if(SelectionMode==SelectionMode.None) {
				return;
			}
			if(e.KeyCode==Keys.Up) {//arrow key up
				if(_listSelectedIndices.Count<1) {//start at the 0th index
					_listSelectedIndices.Add(0);
				}
				int selected=_listSelectedIndices[_listSelectedIndices.Count-1];//get the last selected index
				_listSelectedIndices.Clear();//clear the list so that it removes all of the multiple selected indices
				_listSelectedIndices.Add(selected);
				_listSelectedIndices[0]-=1;
				if(_listSelectedIndices[0]<0) {
					_listSelectedIndices[0]=0;
				}
				SetVScrollValue();
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
			}
			if(e.KeyCode==Keys.Down) {//arrow key down
				if(_listSelectedIndices.Count<1) {//start at a negative index so that it can be incremented later
					_listSelectedIndices.Add(-1);
				}
				int selected=_listSelectedIndices[_listSelectedIndices.Count-1];//get the last selected index
				_listSelectedIndices.Clear();//clear the list so that it removes all of the multiple selected indices
				_listSelectedIndices.Add(selected);
				_listSelectedIndices[0]+=1;
				if(_listSelectedIndices[0]>Items.Count-1) {
					_listSelectedIndices[0]=Items.Count-1;
				}
				SetVScrollValue();
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
			}
			char charKey=(char)e.KeyCode;
			if(e.KeyCode>=Keys.NumPad0 && e.KeyCode<=Keys.NumPad9) {
				charKey=e.KeyCode.ToString().Replace("NumPad","")[0];
			}
			if(char.IsLetterOrDigit(charKey)) {//alpha or numeric character down
				if(AltIsDown()) {
					return;
				}
				if(_listSelectedIndices.Count<1) {
					SetSearchedIndex(0,charKey);
					Invalidate();
					e.SuppressKeyPress=true;
					return;
				}
				int selected=_listSelectedIndices[_listSelectedIndices.Count-1];//get the last selected index
				_listSelectedIndices.Clear();//clear the list so that it removes all of the multiple selected indices
				_listSelectedIndices.Add(selected);
				bool foundMatch=SetSearchedIndex(_listSelectedIndices[0]+1,charKey);
				if(!foundMatch) {//if nothing is found, then start the search from the beginning
					foundMatch=SetSearchedIndex(0,charKey);
				}
				if(foundMatch){
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					SelectionChangeCommitted?.Invoke(this,new EventArgs());
				}
				e.SuppressKeyPress=true;
			}
			Invalidate();
		}
		#endregion Methods - Event Handlers - Key Press

		#region Event - Event Handlers - OnPaint
		protected override void OnPaint(PaintEventArgs e){
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(Color.White);
			if(DesignMode){
				if(_itemStrings==null || _itemStrings.Length==0){
					g.DrawString(this.Name,Font,Brushes.Black,2,2);
				}
				else{
					for(int i=0;i<_itemStrings.Length;i++){
						g.DrawString(_itemStrings[i],Font,Brushes.Black,2,2+_heightLine*i);
					}
				}
				g.DrawRectangle(Pens.SlateGray,new Rectangle(0,0,Width-1,Height-1));
				return;
			}
			_heightLine=Font.Height;
			int maxItems=(this.Height/_heightLine);//rounds down
			if(Items.Count>maxItems) {
				LayoutManager.Move(vScroll,new Rectangle(Width-vScroll.Width-1,1,LayoutManager.Scale(17),Height-2));
				//Give the maxmimum one extra line item so that the real last line item isn't cut off
				int oldScrollMaxValue=vScroll.Maximum;
				vScroll.Maximum=((Items.Count+1)*_heightLine);
				vScroll.SmallChange=_heightLine;
				vScroll.LargeChange=this.Height;
				vScroll.Visible=true;
				vScroll.Enabled=true;
				if(vScroll.Maximum!=oldScrollMaxValue){//set the scrollbar value on update/load of the ListBox items
					SetVScrollValue();
				}
			}
			else {
				vScroll.Value=vScroll.Minimum;//shows the remaining items
				vScroll.Visible=false;
				vScroll.Enabled=false;
			}
			Brush brushText=Brushes.Black;//no dispose
			if(!Enabled){
				brushText=Brushes.Gray;
			}
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.LineAlignment=StringAlignment.Center;
			//Determines where the index should start when the user has scrolled the list (always needs to round down)
			_indexTopShowing=vScroll.Value/_heightLine;
			//Only try to paint 200 objects at most
			int totalToDraw=(200+_indexTopShowing)>Items.Count ? Items.Count : 200+_indexTopShowing;
			for(int i=_indexTopShowing; i<totalToDraw; i++) {
				bool isSelected=false;
				if(_listSelectedIndices.Contains(i)) {
					isSelected=true;
				}
				if(isSelected) { //Draw the selected index with gray background
					g.FillRectangle(_brushSelectedBack,new Rectangle(2,((i-_indexTopShowing)*_heightLine)+2,this.Width-4,_heightLine));
				}
				else if(_hoverIndex==i) { //Draw the hovered index with a light blue background (only when being moused over)
					g.FillRectangle(_brushHover,new Rectangle(2,((i-_indexTopShowing)*_heightLine)+2,this.Width-4,_heightLine));
				}
				Rectangle rectangle=new Rectangle(2,((i-_indexTopShowing)*_heightLine)+2,Width-2,_heightLine);
				g.DrawString(Items.GetTextShowingAt(i),this.Font,brushText,rectangle);
			}
			g.DrawRectangle(Pens.SlateGray,new Rectangle(0,0,Width-1,Height-1));
		}
		#endregion Event - Event Handlers - OnPaint

		#region Events - Event Handlers - Mouse
		protected override void OnMouseDown(MouseEventArgs e){
			Focus();
			if(SelectionMode==SelectionMode.None) {
				base.OnMouseDown(e);
				return;
			}
			_isMouseDown=true;
			_mouseDownIndex=((e.Location.Y-3)/_heightLine)+_indexTopShowing;
			if(_mouseDownIndex>Items.Count-1){//clicked below items
				return;//don't deselect anything, but preserve the clicked index in case they drag up
			}
			_listSelectedOrig=new List<int>(_listSelectedIndices);//for both multi and single, we need to remember this
			if(SelectionMode==SelectionMode.MultiExtended) {
				CalcSelectedIndices();
			}
			else{//single
				_listSelectedIndices=new List<int>(){_mouseDownIndex};
			}
			if(!_listSelectedOrig.SequenceEqual(_listSelectedIndices)){
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
				SelectedIndexChanged?.Invoke(this,new EventArgs());
			}
			Invalidate();
			base.OnMouseDown(e);//at end so that index will be selected before mouse down fires somewhere else (e.g. FormApptEdit).  Matches MS.
			//but, sometimes, if an event from above resulted in a dialog, then there will be no mouse up event.  Handle that below.
			MouseButtons mouseButtons=Control.MouseButtons;//introducing variable for debugging because this state is not preserved at break points.
			if(mouseButtons==MouseButtons.None){
				_isMouseDown=false;
			}
		}

		protected override void OnMouseLeave(EventArgs e){
			base.OnMouseLeave(e);
			_hoverIndex=-1;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			//subtract 3 from the location to take border offset into account
			_hoverIndex=((e.Location.Y-3)/_heightLine)+_indexTopShowing;
			if(!_isMouseDown) {
				Invalidate();
				return;
			}
			//from here down, dragging
			if(vScroll.Enabled) {
				if(e.Y<0) {//if mouse is above the top of the ListBox while mouse down
					VScrollMoveUp();
				}
				else if(e.Y>Height) {//if mouse is below the bottom of the ListBox while mouse down
					VScrollMoveDown();
				}
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
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(SelectionMode==SelectionMode.None) {
				return;
			}
			_isMouseDown=false;
			Invalidate();
		}
		#endregion Events - Event Handlers - Mouse

		#region Methods - Event Handlers
		private void ListBoxOD_Scroll(object sender,ScrollEventArgs e) {
			//Called from WndProc, so if it is not enabled, then return
			if(!vScroll.Enabled) {
				return;
			}
			if(e.OldValue>e.NewValue) {
				VScrollMoveUp();
			}
			else {
				VScrollMoveDown();
			}
			Invalidate();
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(vScroll.Enabled && vScroll.Value>0) {
				if(vScroll.Value>vScroll.Maximum-this.Height && vScroll.Maximum-this.Height>=0) {//checks when resizing if the items need to move down
					vScroll.Value=vScroll.Maximum-this.Height+2;
				}
			}
			if(!DesignMode){
				return;
			}
			if(!IntegralHeight){
				return;
			}
			//This only happens in design mode, so LayoutManager not involved.
			int rows=Height/_heightLine;//rounds down
			Height=_heightLine*rows+4;//this very nicely matches MS behavior
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

		#region Properties - override
		protected override Size DefaultSize => new Size(120,95);
		#endregion Properties - override

		#region Properties - Public Browsable
		[Description("Height of control jumps in increments.")]
		[Category("OD")]
		[DefaultValue(true)]
		public bool IntegralHeight{ get; set; } = true;

		[Description("Initial Items can be set here in the designer.")]
		[Category("OD")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(null)]
		public string[] ItemStrings{
			get{
				return _itemStrings;
			}
			set{
				_itemStrings=value;
				if(!DesignMode){
					Items.Clear();
					for(int i=0;i<_itemStrings.Length;i++){
						Items.Add(_itemStrings[i]);
					}
				}
				Invalidate();
			}
		}

		[Category("OD")]
		[Description("Set to allow none, single, or multiple selections.")]
		[DefaultValue(SelectionMode.One)]
		public SelectionMode SelectionMode { get; set; } = SelectionMode.One;
		#endregion Properties - Public Browsable

		#region Properties - Public not Browsable
		///<summary>This is the list of Items to show.</summary>
		[Browsable(false)]
		//[Description("This is the list of Items to show.")]
		//[Category("OD")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ListBoxItemCollection Items { get;	} //had to be initialized in constructor

		///<summary>Gets or sets the selected index. Setter has same behavior for SelectionMode.MultiExtended or not. Get throws exception for SelectionMode.MultiExtended.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex { 
			get{
				if(SelectionMode==SelectionMode.MultiExtended){
					throw new Exception("SelectedIndex.Get is ambiguous when SelectionMode.MultiExtended.");
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
				if(value<-1 || value>Items.Count-1){
					return;//ignore out of range
				}
				_listSelectedIndices.Clear();
				_overrideText="";
				if(value!=-1){
					_listSelectedIndices.Add(value);
				}
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetVScrollValue();
				Invalidate();
			}
		} 

		///<summary>Gets or sets one selected object. Get throws exception for SelectionMode.MultiExtended.</summary>
		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedItem {
			get{
				if(!DesignMode && SelectionMode==SelectionMode.MultiExtended){
					throw new Exception("SelectedItem.Get is ambiguous when SelectionMode.MultiExtended.");
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
				if(!DesignMode && SelectionMode==SelectionMode.None) {
					throw new Exception("SelectedItem is not allowed when SelectionMode.None.");
				}
				if(value==null) {
					//deselect, just like MS behavior
					if(_listSelectedIndices.Count==0) {
						return;
					}
					_listSelectedIndices.Clear();
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					Invalidate();
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
				SetVScrollValue();
				Invalidate();
				return;
			}
		}

		///<summary>Gets or sets the selected indices. Getter has same behavior for SelectionMode.MultiExtended or not.  Set throws exception if not SelectionMode.MultiExtended.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> SelectedIndices { 
			get{
				//returns a value, not a reference
				return _listSelectedIndices;
			}
			set{
				if(SelectionMode==SelectionMode.None) {
					throw new Exception("SelectedIndices is not allowed when SelectionMode.None.");
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
				Invalidate();
			}
		} 
		#endregion Properties - Public not Browsable

		#region Methods - Public
		///<summary>Same as using SetAll(false), but this is here to mirror MS listBoxes.</summary>
		public void ClearSelected(){
			_listSelectedIndices.Clear();
			SelectedIndexChanged?.Invoke(this, new EventArgs());
			Invalidate();
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
			if(_listSelectedIndices.Count==0){
				return _selectedKey;//could be zero
			}
			if(Items.GetObjectAt(_listSelectedIndices[0])==null){//just in case
				return 0;
			}
			return funcSelectKey((T)Items.GetObjectAt(_listSelectedIndices[0]));
		}

		///<summary>Gets a string of all selected items, separated by commas.</summary>
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

		///<summary>Gets the index at the specified point. Returns -1 if no index can be selected at that point.</summary>
		public int IndexFromPoint(Point point) {
			_heightLine=Font.Height;
			int indexAtPoint=((point.Y-3)/_heightLine)+_indexTopShowing;
			if(indexAtPoint>=Items.Count) {
				indexAtPoint=-1;
			}
			if(indexAtPoint<0) {
				indexAtPoint=-1;
			}
			return indexAtPoint;
		}

		///<summary>Sets all rows either selected or unselected.</summary>
		public void SetAll(bool setToValue){
			if(SelectionMode==SelectionMode.None) {
				throw new Exception("SetAll is not allowed when SelectionMode.None.");
			}
			_listSelectedIndices.Clear();
			if(setToValue){//if setting all true
				if(SelectionMode!=SelectionMode.MultiExtended){
					throw new Exception("SetAll is not allowed when not SelectionMode.MultiExtended.");
				}
				for(int i=0;i<Items.Count;i++){
					_listSelectedIndices.Add(i);
				}
			}
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			Invalidate();
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
				if(SelectionMode!=SelectionMode.MultiExtended){
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
			SetVScrollValue();
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			Invalidate();
		}

		///<summary>Really only needed if enum is not 0,1,2,... or if enums were manually added to the list, ignoring their normal idx order.  Otherwise, you could also use SetSelected((int)enumVal);  If the enum val is not present in the listBox, it will do nothing.</summary>
		public void SetSelectedEnum<T>(T enumVal){
			if(SelectionMode==SelectionMode.None) {
				throw new Exception("SetSelectedEnum is not allowed when SelectionMode.None.");
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
					SetVScrollValue();
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					Invalidate();
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
			Invalidate();
		}
		#endregion Methods - Public

		#region Methods - Protected
		protected override bool IsInputKey(Keys keyData) {
			//up and down keys should be sent to this control instead of causing jump to new control
			if(keyData==Keys.Up || keyData==Keys.Down) {
				return true;
			}
			return base.IsInputKey(keyData);
		}

		///<summary>We have to intercept the mouse scroll event, because there are no actual controls on the screen so there is no way to actually scroll as there is no content</summary>
		protected override void WndProc(ref Message m) {
			if(m.Msg==WM_MOUSEWHEEL) {//scrolling
				int delta;
				if((long)m.WParam>=(long)Int32.MaxValue) { //Just in case the param is larger than the max
					var wParam = new IntPtr((long)m.WParam << 32 >> 32);
					delta=wParam.ToInt32() >> 16;
				}
				else {
					delta=m.WParam.ToInt32() >> 16;
				}
				delta*=-1;
				//If true, then scroll up, otherwise scroll down
				ScrollEventArgs sarg;
				if(delta > 0){
					sarg=new ScrollEventArgs(ScrollEventType.EndScroll,0,1);
				}
				else{
					sarg=new ScrollEventArgs(ScrollEventType.EndScroll,1,0);
				}
				ListBoxOD_Scroll(this,sarg);
			}
			//if(m.Msg==WM_DPICHANGED) {
			//	return;//ignore.  
			//}
			base.WndProc(ref m);
		}

		//private const int WM_DPICHANGED=0x02E0;
		private const int WM_MOUSEWHEEL=0x020A;
		#endregion Methods - Protected

		#region Methods - Private
		///<summary>Called on mouse down and mouse move.  Recalculates the _listSelectedIndices, based on _listSelectedOrig, _mouseDownIndex, _hoverIndex, ShiftIsDown, and ControlIsDown.  By looping through all items in entire list each time.</summary>
		private void CalcSelectedIndices() {
			if(ShiftIsDown()) {
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
			_listSelectedIndices.Clear();
			for(int i=0;i<Items.Count; i++) {
				bool isInRange=false;
				if(i>=_mouseDownIndex && i<=_hoverIndex) { //Mouse is lower than start
					isInRange=true;
				}
				if(i<=_mouseDownIndex && i>=_hoverIndex) {//Mouse is higher than start
					isInRange=true;
				}
				if(ControlIsDown()) {
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

		private bool AltIsDown() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightAlt);
		}

		private bool ControlIsDown() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
		}

		///<summary>Used to search through a list box to find a matching index and set it. Returns true if it found a match, otherwise false.</summary>
		private bool SetSearchedIndex(int startIdx, char charKey) {
			for(int i=startIdx;i<Items.Count;i++) {//loop through all of the items and only add the item if it is found
				if(Items.GetTextShowingAt(i).ToUpper().StartsWith(charKey.ToString())) {//charKey is already uppercased
					_listSelectedIndices.Clear();
					SetSelected(i);
					return true;
				}
			}
			return false;
		}

		///<summary>Updates the value of the scroll bar based off of selecting or deleting items.</summary>
		private void SetVScrollValue() {
			if(!vScroll.Enabled) {
				return;
			}
			if(vScroll.Value>vScroll.Maximum-this.Height) {//check if the scrollbar is below the last item
				int valueBottom=vScroll.Maximum-this.Height+2;
				valueBottom=Math.Min(vScroll.Value,valueBottom);
				valueBottom=Math.Max(vScroll.Minimum,valueBottom);
				vScroll.Value=valueBottom;
			}
			if(_listSelectedIndices.Count<1) {
				return;
			}
			if(_listSelectedIndices[_listSelectedIndices.Count-1]<_indexTopShowing) {//the latest selected index is above the bounds
				for(int i=_indexTopShowing;i>0;i--) {
					if(i==_listSelectedIndices[_listSelectedIndices.Count-1]) {
						break;
					}
					VScrollMoveUp();//subtracts from vScroll.Value
					if(vScroll.Value==0) {
						break;
					}
				}
			}
			else if((_listSelectedIndices[_listSelectedIndices.Count-1]-_indexTopShowing+1)*_heightLine>this.Height) {//the latest selected index is hidden below bounds
				for(int i=this.Height;i<=(_listSelectedIndices[_listSelectedIndices.Count-1]-_indexTopShowing+1)*_heightLine;i+=_heightLine) {
					if(i==_listSelectedIndices[_listSelectedIndices.Count-1]) {
						break;
					}
					VScrollMoveDown();//adds to vScroll.Value
					if(vScroll.Value==vScroll.Maximum-this.Height+2) {
						break;
					}
				}
			}
			_indexTopShowing=vScroll.Value/_heightLine;//update the top index of the ListBox showing since VScrollMoveUp/Down updates vScroll.Value.
		}

		private bool ShiftIsDown() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
		}

		///<summary>Moves the scrollbar down by increasing the vScroll value or moves to the bottom if near the bottom.</summary>
		private void VScrollMoveDown() {
			if(vScroll.Value+vScroll.SmallChange>vScroll.Maximum-this.Height) {//if scrollbar is near the bottom
				vScroll.Value=vScroll.Maximum-this.Height+2;//set scrollbar to the bottom
			}
			else {
				vScroll.Value+=vScroll.SmallChange;//move scrollbar one item down
			}
		}

		///<summary>Moves the scrollbar up by decreasing the vScroll value or moves to the top if near the top.</summary>
		private void VScrollMoveUp() {
			if(vScroll.Value-vScroll.SmallChange<0) {//if scrollbar is near top
				vScroll.Value=0;//set scrollbar to the top
			}
			else {
				vScroll.Value-=vScroll.SmallChange;//move scrollbar one item up
			}
		}
		#endregion Methods - Private

		#region Class - ListBoxItemCollection
		///<summary>Nested class for the collection of items.  Each items must be a ListBoxItem, not null.  Each field of a ListBoxItem may be null.</summary>
		public class ListBoxItemCollection{
			///<summary>The listBoxOD that this collection is attached to.</summary>
			private ListBoxOD _listBoxODParent;
			///<summary>The internal list of items, exposed through methods.</summary>
			private List<ListBoxItem> _listListBoxItems;

			public ListBoxItemCollection(ListBoxOD listBoxOD){
				_listBoxODParent=listBoxOD;
				_listListBoxItems=new List<ListBoxItem>();
			}

			///<summary>Specify the text to show. Optionally, specify the object represented by that text. Also, optionally display an abbreviation for each item to display in the selected summary above.</summary>
			public void Add(string text,object item=null,string abbr=null){
				if(item is null) {
					item=text;
				}
				_listListBoxItems.Add(new ListBoxItem(text,item,abbr));
				_listBoxODParent.Invalidate();
			}

			///<summary>Adds the values of an enum to the list of Items.  Does not Clear first.  Descriptions are pulled from DescriptionAttribute or .ToString, then run through translation.  If you want add only some enums, or in a different order, or display ShortDescriptionAttribute, you have to add the Enums individually with your own text.</summary>
			public void AddEnums<T>() where T : Enum {//struct,IConvertible{
				AddList(Enum.GetValues(typeof(T)).Cast<T>(),x=>Lan.g("enum"+typeof(T).Name,GetEnumDescription(x)));
				_listBoxODParent.Invalidate();
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

			///<summary>Adds a collection to the items. Does not Clear first.  funcItemToString specifies a string to be displayed for this item, example x=>x.LName or x=>x.ToString(). funcItemToAbbr is similar for the abbreviation used for summary at top of list and for GetStringSelectedItems.  See bottom of this file for example.</summary>
			public void AddList<T>(IEnumerable<T> items,Func<T,string> funcItemToString,Func<T,string> funcItemToAbbr=null){
				//It was too dangerous to make this optional.  People forget.  The string to show needs to be required.
				//funcItemToString=funcItemToString??(x => x.ToString());
				string abbr=null;
				foreach(T item in items) {
					if(funcItemToAbbr!=null){
						abbr=funcItemToAbbr(item);
					}
					_listListBoxItems.Add(new ListBoxItem(funcItemToString(item),item,abbr));
				}
				_listBoxODParent.Invalidate();
			}

			///<summary>Adds a dummy object called "None", with a key of 0.</summary>
			public void AddNone<T>() where T:new(){
				Add(Lan.g("list","None"),new T());
			}

			///<summary>This is just for the designer serialization to make it compatible with MS listBox. But if you want enums in the listbox instead of strings, you will need to add them in code instead of in the designer.  New listBoxes would use ItemStrings instead of AddRange. Clears first.</summary>
			public void AddRange(object[] arrayObjects) {
				if(arrayObjects==null) {
					return; 
				}
				_listListBoxItems.Clear();//did not test MS behavior here, but it doesn't matter.
				_listBoxODParent.ItemStrings=arrayObjects.Cast<string>().ToArray();
				_listBoxODParent.Invalidate();
			}

			///<summary>Adds a collection of strings to the items. Does not Clear first.</summary>
			public void AddStrings(IEnumerable<string> items){
				for(int i=0;i<items.Count();i++) {
					_listListBoxItems.Add(new ListBoxItem(items.ElementAt(i),items.ElementAt(i)));
				}
				_listBoxODParent.Invalidate();
			}

			public void Clear(){
				if(_listListBoxItems.Count==0){
					return;//prevent premature firing of SelectedIndexChanged
				}
				bool isSelected=false;
				if(_listBoxODParent._listSelectedIndices.Count>0){
					isSelected=true;
				}
				_listListBoxItems.Clear();
				_listBoxODParent._listSelectedIndices.Clear();
				if(isSelected){
					_listBoxODParent.SelectedIndexChanged?.Invoke(this,new EventArgs());
				}
				_listBoxODParent.Invalidate();
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

			///<summary>This gives you the entire ListBoxItem.  If you just want the Item object, use [i] or GetObjectAt(i). Can be null.</summary>
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
				_listBoxODParent.Invalidate();
			}

			///<summary>Rarely used. You can change the value of an existing item in the list.</summary>
			public void SetValue(int index,object value) {
				if(index>=_listListBoxItems.Count || index==-1) {
					return;
				}
				_listListBoxItems[index].Item=value;
				_listListBoxItems[index].Text=value.ToString();
				_listBoxODParent.Invalidate();
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
		MultiExtended
	}
	#endregion
}


//To replace an existing ListBox with a ListBoxOD:
//Research: Ctrl-F to highlight and review all instances of the existing ListBox in code.
//Edit the designer.cs to change the type.
//Fix all the places where this breaks the code.
//Adapt existing code to the patterns supported by the new listBox type.
//Review every instance of the listBox in that class, even the ones that didn't break.
//
//ENUM---------------------------------------------------------------------------------------------------------------
//listArea.Items.AddEnums<EnumArea>();
//Or, rarely: listArea.Items.Add(Lan.g("enumArea","First Item"),EnumArea.FirstItem);
//listArea.SetSelected((int)proc.Area);
//or
//listArea.SetSelectedEnum(proc.Area);//type is inferred 
//...
//proc.Area=listArea.GetSelected<EnumArea>();
//
//Other db table types----------------------------------------------------------------------------------------------
//listObj.Items.Clear();//skip in Load()
//listObj.Items.AddNone<ObjType>();//optional
//listObj.Items.AddList(listObjs,x=>x.LName);//the abbr parameter is usually skipped. <T> is inferred.
//listObj.SetSelectedKey<ObjType>(adj.ObjNum,x=>x.ObjNum,x=>Objs.GetName(x)); 
//...
//These are some ways to get a selected primary key:
//adj.ObjNum=listObj.GetSelected<ObjType>().ObjNum;
//adj.ObjNum=((ObjType)listObj.SelectedItem).ObjNum;
//adj.ObjNum=_listObjs[listObjs.SelectedIndex].ObjNum;
//Be aware that if selected index is -1, then object would be null, which would crash.
//They might be safe if you are guaranteed to always have a selection.  Single select ListBoxes do not allow deselection for this reason.
//The following approach won't crash, but a -1 result might be nonsensical and not allowed in db.
//adj.ObjNum=listObj.GetSelectedKey<ObjType>(x=>x.ObjNum);

