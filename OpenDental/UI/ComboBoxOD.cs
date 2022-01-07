using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.UI{
	//Jordan is the only one allowed to edit this file.
	//This combobox was built as a replacement for many outdated comboboxes.
	//Allowed elimination of ComboBoxMulti, ComboBoxOD, and ODBoxItem. 
	//Allowed deprecation of all combobox extension methods
	//Some of the syntax is similar to the old ODBoxItems and extension methods.
	//-------------------------------------------
	//Problems it did not solve and features it lacks:
	//It's not a listbox.  See ListBoxOD.
	//It does not replace ComboBoxClinicPicker, although that control could be improved based on patterns established here.
	//Does not allow text entry.  It will always behave like a dropdown listbox.
	//Exception to above: It sometimes needs to display text for an item that's not in the list due to perms, but still no text entry.

	///<summary>A combobox used extensively in OD.  Usually use ComboBoxClinicPicker for Clinics.  This supports multi-select or single-select.  It stores objects, paired with their display strings.  It has special handling for Providers, Defs, and Enums.  It handles setting to a value that is not in the list.  It has a special "All" setting.  Providers and Defs have special handling for key=0, but other types have less support.  You can manually add a none/0 item to any combo.  See the bottom of this file for many examples of how to use.</summary>
	public partial class ComboBoxOD : Control{
		#region Fields - Private Static
		//Colors etc are static and shared among all comboboxes for life of program. Not disposed
		///<summary></summary>
		private static SolidBrush _brushBack=new SolidBrush(Color.FromArgb(240,240,240));//lighter than built-in color of 225
		///<summary></summary>
		private static SolidBrush _brushDisabledBack=new SolidBrush(Color.FromArgb(204,204,204));//copied built-in color
		///<summary></summary>
		private static SolidBrush _brushDisabledText=new SolidBrush(Color.FromArgb(109,109,109));
		///<summary></summary>
		private static SolidBrush _brushHover=new SolidBrush(Color.FromArgb(229,241,251));//light blue
		///<summary></summary>
		private static Pen _penArrow=new Pen(Color.FromArgb(20,20,20),1.5f);
		///<summary></summary>
		private static Pen _penHoverOutline=new Pen(Color.FromArgb(0,120,215));//blue
		///<summary></summary>
		private static Pen _penOutline=new Pen(Color.FromArgb(173,173,173),1);
		#endregion Fields - Private Static

		#region Fields - Private
		///<summary>This is the part that comes up as a "list" to pick from.</summary>
		private FormComboPicker _formComboPicker;
		///<summary>True if the mouse is over the "combobox", to turn it a blue color.</summary>
		private bool _isMouseOver;
		///<summary>Just holds the scaling factor.</summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>This gets set when the user sets an item that is not present in the list. Selected index is also set to -1.
		private string _overrideText="";
		///<summary>If selected index is -1, this can be used to store and retrieve the primary key. _overrideText is what shows to the user.</summary>
		private long _selectedKey=0;//odd note: FormClaimCustomTrackingUpdate sets this to -1 to indicate none, which should be fine.
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _includeAll=false;
		private bool _isAllSelected=false;
		///<summary>This is the only internal storage for tracking selected indices.  All properties refer to this same list. This list never includes the All option.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		#endregion Fields - Private for Properties

		#region Constructors
		public ComboBoxOD(){
			InitializeComponent();
			DoubleBuffered=true;
			//This style needs to be set, otherwise when mousing down on something that inherits from control, it won't actually take 
			//focus. When this style is set, mouse events are no longer handled by the OS. 
			SetStyle(ControlStyles.UserMouse,true);
			Size=new Size(121,21);//same as default
			Items=new ComboBoxItemCollection(this);
		}
		#endregion Constructors

		#region Events - Public Raise
		///<summary>Occurs when user selects item(s) from the drop-down list and the drop-down closes.</summary>
		private void OnSelectionChangeCommitted(object sender,EventArgs e){
			SelectionChangeCommitted?.Invoke(sender,e);
		}
		[Category("OD")]
		[Description("Occurs when user selects item(s) from the drop-down list and the drop-down closes.")]
		public event EventHandler SelectionChangeCommitted;

		
		private void OnSelectedIndexChanged(object sender,EventArgs e){
			SelectedIndexChanged?.Invoke(sender,e);
		}
		///<summary>Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.</summary>
		[Category("OD")]
		[Description("Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events - Public Raise

		#region Event - OnPaint
		protected override void OnPaint(PaintEventArgs e){
			base.OnPaint(e);
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.AntiAlias;
			Rectangle rectangle=new Rectangle(0,0,Width-1,Height-1);
			if(Enabled){
				if(_isMouseOver){
					g.FillRectangle(_brushHover,rectangle);
					g.DrawRectangle(_penHoverOutline,rectangle);
				}
				else{
					g.FillRectangle(_brushBack,rectangle);
					g.DrawRectangle(_penOutline,rectangle);
				}
			}
			else{
				g.FillRectangle(_brushDisabledBack,rectangle);
				g.DrawRectangle(_penOutline,rectangle);
			}
			//the dropdown arrow, starting at the left
			g.DrawLine(_penArrow,Width-LayoutManager.ScaleF(13),LayoutManager.ScaleF(9),Width-LayoutManager.ScaleF(9.5f),LayoutManager.ScaleF(12));
			g.DrawLine(_penArrow,Width-LayoutManager.ScaleF(9.5f),LayoutManager.ScaleF(12),Width-LayoutManager.ScaleF(6),LayoutManager.ScaleF(9));
			RectangleF rectangleFString=new RectangleF();
			rectangleFString.X=rectangle.X+2;
			rectangleFString.Y=rectangle.Y+4;
			rectangleFString.Width=rectangle.Width-2;
			rectangleFString.Height=rectangle.Height-4;
			int widthMax=rectangle.Width-15;
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.LineAlignment=StringAlignment.Center;
			if(Enabled){
				g.DrawString(GetDisplayText(widthMax),this.Font,Brushes.Black,rectangleFString,stringFormat);
			}
			else{
				g.DrawString(GetDisplayText(widthMax),this.Font,_brushDisabledText,rectangleFString,stringFormat);
			}
		}
		#endregion Event - OnPaint

		#region Events - Mouse
		protected override void OnMouseDown(MouseEventArgs e){
			base.OnMouseDown(e);
			if(Items.Count==0){//even if IncludeAll, we still don't want it to show.
				return;
			}
			_formComboPicker=new FormComboPicker();
			_formComboPicker.Font=this.Font;
			_formComboPicker.HeightCombo=this.Height;
			_formComboPicker.FormClosing += _formComboPicker_FormClosing;
			List<string> listStrings=new List<string>();
			List<string> listAbbrevs=new List<string>();
			List<int> selectedIndices=new List<int>();
			int indexOffset=0;
			if(IncludeAll){
				listStrings.Add("All");
				listAbbrevs.Add("All");
				indexOffset=1;
				if(IsAllSelected){
					selectedIndices.Add(0);
				}
			}
			if(!IsAllSelected){//can't include other indices if "all" selected
				for(int i=0;i<_listSelectedIndices.Count;i++){
					selectedIndices.Add(_listSelectedIndices[i]+indexOffset);
				}
			}
			for(int i=0;i<Items.Count;i++){
				listStrings.Add(Items.GetTextShowingAt(i));
				listAbbrevs.Add(Items.GetAbbrShowingAt(i));
			}
			_formComboPicker.ListStrings=listStrings;
			_formComboPicker.ListAbbrevs=listAbbrevs;
			_formComboPicker.PointInitialUR=this.PointToScreen(new Point(this.Width,0));
			_formComboPicker.MinimumSize=new Size(15,15);
			_formComboPicker.Width=this.Width;
			if(SelectionModeMulti){
				_formComboPicker.IsMultiSelect=true;
				_formComboPicker.SelectedIndices=selectedIndices;
			}
			else{
				if(selectedIndices.Count==0){
					_formComboPicker.SelectedIndex=-1;
					_formComboPicker.OverrideText=_overrideText;
				}
				else{
					_formComboPicker.SelectedIndex=selectedIndices[0];
				}
			}
			_formComboPicker.Show();
		}

		protected override void OnMouseLeave(EventArgs e){
			base.OnMouseLeave(e);
			_isMouseOver=false;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			if(_isMouseOver){
				return;
			}
			_isMouseOver=true;
			Invalidate();
		}
		#endregion Events - Mouse

		#region Events - Misc
		protected override void OnHandleDestroyed(EventArgs e){
			//prevents an orphaned form from hanging around
			base.OnHandleDestroyed(e);
			if(_formComboPicker!=null && !_formComboPicker.IsDisposed){
				_formComboPicker.Close();
				_formComboPicker.Dispose();
			}
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode>=Keys.F1 && e.KeyCode<=Keys.F24) {
				return;//Ignore any F keys, so that F key behavior is perserved
			}
			char charKey=(char)e.KeyCode;
			if(e.KeyCode>=Keys.NumPad0 && e.KeyCode<=Keys.NumPad9) {
				charKey=e.KeyCode.ToString().Replace("NumPad","")[0];
			}
			bool foundMatch=false;
			if(char.IsLetterOrDigit(charKey) && !SelectionModeMulti) {//alpha character down
				if(_listSelectedIndices.Count<1) {
					foundMatch=SetSearchedIndex(0,charKey);
					if(foundMatch) {
						OnSelectedIndexChanged(this,new EventArgs());
						OnSelectionChangeCommitted(this,new EventArgs());
					}
					Invalidate();
					return;
				}
				foundMatch=SetSearchedIndex(_listSelectedIndices[0]+1,charKey);
				if(!foundMatch) {//if nothing is found, then start the search from the beginning
					foundMatch=SetSearchedIndex(0,charKey);
				}
			}
			if(foundMatch) {
				OnSelectedIndexChanged(this,new EventArgs());
				OnSelectionChangeCommitted(this,new EventArgs());
			}
			Invalidate();
		}
		#endregion Events - Misc

		#region Events -  Private
		private void _formComboPicker_FormClosing(object sender, FormClosingEventArgs e){
			//This is designed to fire whether or not changed. For example, FormPatientAddAll.ComboClinic1 needs to change the others even if it doesn't change.
			//Unsubscribe this event handler from the _formComboPicker.FormClosing event so other calls to close _formComboPicker won't trigger this method
			//again, e.g. Application.DoEvents will trigger the _formComboPicker.Deactivate event which calls _formComboPicker.Close() and this code would
			//run twice if we stayed subscribed to the FormClosing event.
			//_formComboPicker.FormClosing-=_formComboPicker_FormClosing;
			_listSelectedIndices.Clear();
			int indexOffset=0;
			if(IncludeAll){
				if(_formComboPicker.SelectedIndices.Contains(0)){
					IsAllSelected=true;
				}
				else{
					IsAllSelected=false;
				}
				indexOffset=1;
			}
			if(SelectionModeMulti){
				if(!IsAllSelected){//if all is selected, we ignore other selections
					for(int i=0;i<_formComboPicker.SelectedIndices.Count;i++){
						_listSelectedIndices.Add(_formComboPicker.SelectedIndices[i]-indexOffset);
					}
				}
			}
			else{//single select mode
				if(!IsAllSelected){//if all is selected, we ignore other selections
					if(_formComboPicker.SelectedIndex!=-1){
						_listSelectedIndices.Add(_formComboPicker.SelectedIndex-indexOffset);
					}
				}
			}
			OnSelectionChangeCommitted(this,e);
			OnSelectedIndexChanged(this,e);
			Refresh();//to get rid of flickering when selecting after select and after dropdown closes.
		}
		#endregion Events -  Private

		#region Properties - override
		protected override Size DefaultSize => new Size(121,21);
		#endregion Properties - override

		#region Properties - Public Browsable
		[Category("OD")]
		[Description("Set false to not allow scrolling selection when unexpanded.")]
		[DefaultValue(true)]
		public bool AllowScroll { get; set; } = true;

		[Category("OD")]
		[Description("Set true to allow multiple selections.")]
		[DefaultValue(false)]
		public bool SelectionModeMulti { get; set; } = false;
		#endregion Properties - Public Browsable

		#region Properties - Public not Browsable
		///<summary>Set to true to include 'All' as a selection option at the top. 'All' can sometimes be intended to indicate more items than are actually showing in list.  Test IsAllSelected separately if this is the case.  This works for both single and multi selection mode.  This extra row is never part of the Items or internal _listSelectedIndices.  But, if you get SelectedIndices, and the user has selected All, then indices for all the items in the list will be returned.</summary>
		[Browsable(false)]//because it's better to see this in the code than to only see it in the designer.  Intentionally different than ComboBoxClinicPicker.
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(false)]
		public bool IncludeAll {
			//Note: Intentionally not adding other special types like "none".  
			//Those are easy to implement manually, wherease "all" is more complex and benefits a little from internalization.
			get {
				return _includeAll;
			}
			set {
				_includeAll=value;
				Invalidate();
			}
		}

		///<summary>This is the list of Items to show. Never includes the All option.</summary>
		//This is pretty ugly right now from the designer.  Hiding for now.
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ComboBoxItemCollection Items { get; } //had to be initialized in constructor

		///<summary>This is the only way to get or set the special dummy 'All' option (regardless of any other additional selections). All needs to have been added, first.  The intent of All can vary.  On start, setting All is done manually, not automatically.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAllSelected{
			get{
				return _isAllSelected;
			}
			set{
				_listSelectedIndices.Clear();
				_isAllSelected=value;
				Invalidate();
			}
		}

		///<summary>Gets or sets the selected index. Setter has same behavior for SelectionModeMulti or not. Get throws exception for SelectionModeMulti.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex { 
			get{
				if(SelectionModeMulti){
					throw new Exception("SelectedIndex.Get is ambiguous when IsMultiSelect.");
				}
				if(_listSelectedIndices.Count==0){
					return -1;
				}
				return _listSelectedIndices[0];
			}
			set{
				if(value<-1 || value>Items.Count-1){
					return;//ignore out of range
				}
				_listSelectedIndices.Clear();
				_overrideText="";
				if(value!=-1){
					_listSelectedIndices.Add(value);
				}
				OnSelectedIndexChanged(this,new EventArgs());
				Invalidate();
			}
		} 

		///<summary>Gets or sets the selected indices. Getter has same behavior for SelectionModeMulti or not.  If 'All' option is selected, then this returns indices of all items in list. Set throws exception if not SelectionModeMulti.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> SelectedIndices { 
			get{
				//returns a value, not a reference
				return _listSelectedIndices;
			}
			set{
				if(!SelectionModeMulti){
					throw new Exception("Cannot set SelectedIndices when not IsMultiSelect. Use SelectedIndex, SetSelected, etc.");
				}
				_listSelectedIndices.Clear();
				for(int i = 0;i<value.Count;i++) {
					if(value[i]<-1 || value[i]>Items.Count-1){
						continue;//ignore out of range
					}
					_listSelectedIndices.Add(value[i]);
				}
				OnSelectedIndexChanged(this,new EventArgs());
				Invalidate();
			}
		} 
		#endregion Properties - Public not Browsable

		#region Methods - Public
		///<summary>Returns a list of the selected objects. List can be empty.  Items can be null. If special 'All' is set in multi-mode(regardless of other selections), this will include all objects in the combobox.  If 'All' is set in single mode, it will be ignored here and list could be empty.</summary>
		public List<T> GetListSelected<T>() {
			List<T> listSelected=new List<T>();
			if(SelectionModeMulti && IsAllSelected){
				for(int i=0;i<Items.Count;i++){
					listSelected.Add((T)Items.GetObjectAt(i));
				}
				return listSelected;
			}
			for(int i=0;i<_listSelectedIndices.Count;i++){
				listSelected.Add((T)Items.GetObjectAt(_listSelectedIndices[i]));
			}
			return listSelected;
		}

		///<summary>Gets the selected object.  Can be null for object or 0 for enum.  Throws exception for SelectionModeMulti.</summary>
		public T GetSelected<T>() {
			if(SelectionModeMulti){
				throw new Exception("GetSelected is ambiguous when IsMultiSelect.");
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

		///<summary>Only for comboBoxes with a list of Defs. This is a specific use of GetSelectedKey. If selected index is -1, it will try to grab the key that was passed in earlier with SetSelectedDefNum.  If there is none, then it will return 0.</summary>
		public long GetSelectedDefNum(){
			return GetSelectedKey<Def>(x=>x.DefNum);
		}

		///<summary>Gets the key like PatNum from the selected index. ProvNum and DefNum have their own selectors.  funcSelectKey example x=>x.PatNum.  If selected index is -1, it will try to grab the key that was passed in earlier with SetSelectedKey.  If there is none, then it will return 0.  Completely ignores IsAllSelected, so if you are interested in that, test it first.</summary>
		public long GetSelectedKey<T>(Func<T,long> funcSelectKey){
			if(SelectionModeMulti){
				throw new Exception("GetSelected is ambiguous when IsMultiSelect.");
			}
			if(_listSelectedIndices.Count==0){
				return _selectedKey;//could be zero
			}
			if(Items.GetObjectAt(_listSelectedIndices[0])==null){//just in case
				return 0;
			}
			return funcSelectKey((T)Items.GetObjectAt(_listSelectedIndices[0]));
		}

		///<summary>Only for comboBoxes with a list of Providers. This is a specific use of GetSelectedKey. If selected index is -1, it will try to grab the key that was passed in earlier with SetSelectedProvNum.  If there is none, then it will return 0.</summary>
		public long GetSelectedProvNum(){
			return GetSelectedKey<Provider>(x=>x.ProvNum);
		}

		///<summary>Only for multi-select comboBoxes with a list of Providers. Usually for reports.</summary>
		public List<long> GetSelectedProvNums(){
			return GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
		}

		///<summary>Gets a string of all selected items, separated by commas.  If "All" is selected, then the default is to simply return "All".  Or set ifAllListOut to true to list them out. useAbbr will string together abbreviations instead of the full display strings for each item.</summary>
		public string GetStringSelectedItems(bool ifAllListOut=false,bool useAbbr=false) {
			//works the same for IsMultiselect or not
			string retVal="";
			if(IsAllSelected){
				if(ifAllListOut){
					for(int i=0;i<Items.Count;i++){
						if(i>0){
							retVal+=", ";
						}
						if(useAbbr){
							retVal+=Items.GetAbbrShowingAt(i);
						}
						else{
							retVal+=Items.GetTextShowingAt(i);
						}
					}
					return retVal;
				}
				else{
					return "All";
				}
			}
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

		///<summary>Sets all rows either selected or unselected. If 'All' is present, it gets ignored because it's handled separately from the normal items.</summary>
		public void SetAll(bool setToValue){
			_listSelectedIndices.Clear();
			if(setToValue){//if setting all true
				if(!SelectionModeMulti){
					throw new Exception("SetAll is not allowed when not IsMultiSelect.");
				}
				for(int i=0;i<Items.Count;i++){
					_listSelectedIndices.Add(i);
				}
			}
			OnSelectedIndexChanged(this,new EventArgs());
			Invalidate();
		}

		///<summary>Sets one row to selected.</summary>
		public void SetSelected(int index){
			SetSelected(index,true);
		}

		///<summary>Sets one row either selected or unselected.</summary>
		public void SetSelected(int index,bool value){
			if(value){//setting true
				if(_listSelectedIndices.Contains(index)){
					return;//nothing to do
				}
				if(!SelectionModeMulti){
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
			OnSelectedIndexChanged(this,new EventArgs());
			Invalidate();
		}

		///<summary>Only for comboBoxes with a list of Defs. If you set a DefNum that is hidden from the combo, this method will go try to find it and display the name, including (hidden) if that applies.  If you set defNum to 0, it will show "none" or could also show text for any dummy 0 def that you added manually to the combo.</summary>
		public void SetSelectedDefNum(long defNum){
			if(defNum==0){//special handling for 0. We don't want to go looking in the cache.
				_listSelectedIndices.Clear();
				_overrideText="";
				_selectedKey=0;
				//look for a 0 dummy def in list that user added
				for(int i=0;i<Items.Count;i++) {
					if(Items.GetObjectAt(i)==null){
						continue;
					}
					if(typeof(Def)!=Items.GetObjectAt(i).GetType()) {
						continue;
					}
					if(((Def)Items.GetObjectAt(i)).DefNum==0) {
						_listSelectedIndices.Add(i);//found a 0, so select it
						OnSelectedIndexChanged(this,new EventArgs());
						Invalidate();
						return;
					}
				}
				//0 is not in list
				_overrideText=Lan.g("Defs","none");
				_selectedKey=0;//still, and selectedIndex is -1
				OnSelectedIndexChanged(this,new EventArgs());
				Invalidate();
				return;
			}
			Func<long,string> funcOverrideText= x=>Defs.GetNameWithHidden(x);//won't use this unless it has to
			SetSelectedKey<Def>(defNum,x=>x.DefNum,funcOverrideText);
		}

		///<summary>Really only needed if enum is not 0,1,2,... or if enums were manually added to the combo, ignoring their normal idx order.  Otherwise, you could also use SetSelected((int)enumVal);  If the enum val is not present in the comboBox, it will do nothing.</summary>
		public void SetSelectedEnum<T>(T enumVal){
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

		///<summary>Uses key like PatNum to set the selected index. ProvNum and DefNum have their own Setters. funcSelectKey is a func that gets the key from objects in the list for comparison. If the item is not currently in the comboBox and can't be matched, then selectedIndex is set to -1, and the key is stored internally for reference. It will remember that key and return it back in any subsequent GetSelectedKey as long as selectedIndex is still -1. In that case, it also needs text to display, which is set using funcOverrideText. This all works even if the key is garbage, in which case, it will show the key number in the combobox.  If you are confident that your key is going to be in the comboBox, you can omit funcOverrideText, with the worst consequence that the display might be a number.</summary>
		///<param name="funcSelectKey">Examples: x=>x.PatNum</param>
		///<param name="funcOverrideText">Examples:  x=>Carriers.GetName(x), or x=>"none"</param>
		public void SetSelectedKey<T>(long key,Func<T,long> funcSelectKey,Func<long,string> funcOverrideText=null){
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
					OnSelectedIndexChanged(this,new EventArgs());
					Invalidate();
					return;
				}
			}
			//couldn't locate key in list
			//note: we do have the option of including a parameter in this method that makes the caller pass in a func that will attempt to get the actual object.
			//Like this: x=>Providers.GetProv(x).  That would be handy to test for null, but we would still need a func to get the overrideText from that.
			//It also has the downside of no fixed text, and I also don't see a spot where it's needed, so not doing that.
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
			OnSelectedIndexChanged(this,new EventArgs());
			Invalidate();
		}

		///<summary>Only for comboBoxes with a list of Providers. This is a specific use of SetSelectedKey. If the ProvNum is not currently in the comboBox and can't be matched, then selectedIndex is set to -1, the key is stored internally for reference, and the prov. It will remember that key and return it back in any subsequent GetSelectedKey as long as selectedIndex is still -1.  Setting ProvNum to 0 works well if you already used Items.AddProvNone(). But even if you don't have a dummy 0 provider, it will still work well and show "none", leaving SelectedIndex -1.</summary>
		public void SetSelectedProvNum(long provNum){
			if(provNum==0){//special handling for 0. We don't want to go looking in the cache.
				_listSelectedIndices.Clear();
				_overrideText="";
				_selectedKey=0;
				//look for a 0 dummy prov in list that user added
				for(int i=0;i<Items.Count;i++) {
					if(Items.GetObjectAt(i)==null){
						continue;
					}
					if(typeof(Provider)!=Items.GetObjectAt(i).GetType()) {
						continue;
					}
					if(((Provider)Items.GetObjectAt(i)).ProvNum==0) {
						_listSelectedIndices.Add(i);//found a 0, so select it
						OnSelectedIndexChanged(this,new EventArgs());
						Invalidate();
						return;
					}
				}
				//0 is not in list
				_overrideText=Lan.g("Providers","none");
				_selectedKey=0;//still, and selectedIndex is -1
				OnSelectedIndexChanged(this,new EventArgs());
				Invalidate();
				return;
			}
			SetSelectedKey<Provider>(provNum,x=>x.ProvNum,x=>Providers.GetAbbr(x,true));//won't use GetAbbr unless it has to
			//In case the provider long descriptions are being used, this method won't know about that, and will just use Abbr,
			//but it will include (hidden), so that should be more than acceptable.
		}
		#endregion Methods - Public

		#region Methods - Protected
		///<summary>We have to intercept the mouse scroll event, because there are no actual controls on the screen
		///so there is no way to actually scroll as there is no content</summary>
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(m.Msg==0x020A) { //Check for scrolling
				if(!AllowScroll) {
					return;
				}
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
				ComboBoxJ_Scroll(this, sarg);
			}
		}
		#endregion Methods - Protected

		#region Methods - Private
		private void ComboBoxJ_Scroll(object sender,ScrollEventArgs e) {
			if(SelectionModeMulti) {
				return;//only for single select
			}
			if(_listSelectedIndices.Count==0) {
				SetSelected(0);
				Invalidate();
				return;
			}
			if(e.OldValue>e.NewValue) { //Scroll up
				int index=_listSelectedIndices[0]-1<0? _listSelectedIndices[0] : _listSelectedIndices[0]-1;
				SetSelected(index);
			}
			else { //Scroll down
				int index=_listSelectedIndices[0]+1>=Items.Count? _listSelectedIndices[0] : _listSelectedIndices[0]+1;
				SetSelected(index);
			}
			OnSelectionChangeCommitted(this,new EventArgs());
			Invalidate();
		}

		///<summary>Uses the full text if one item is selected.  If multiple items are selected, we string abbreviations together with commas.  But if that string is wider than widthMax, we instead show "Multiple".</summary>
		private string GetDisplayText(int widthMax){
			//works the same for IsMultiselect or not
			if(IsAllSelected){
				return "All";
			}
			if(_listSelectedIndices.Count==0){
				if(_overrideText!=""){
					return _overrideText;
				}
				return "";
			}
			if(_listSelectedIndices.Count==1){
				return Items.GetTextShowingAt(_listSelectedIndices[0]);//full text
			}
			string str="";
			for(int i=0;i<_listSelectedIndices.Count;i++){
				if(i>0){
					str+=",";
				}
				str+=Items.GetAbbrShowingAt(_listSelectedIndices[i]);//abbr
			}
			if(TextRenderer.MeasureText(str,this.Font).Width>widthMax){
				return "Multiple";
			}
			return str;
		}

		///<summary>Used to search through a combo box to find a matching index and set it. Returns true if it found a match, otherwise false.</summary>
		private bool SetSearchedIndex(int startIdx, char charKey) {
			for(int i=startIdx;i<Items.Count;i++) {//loop through all of the items and only add the item if it is found
				if(Items.GetTextShowingAt(i).ToUpper().StartsWith(charKey.ToString())) {//charKey is already uppercased
					if(Items.GetObjectAt(i) is Def) {
						SetSelectedDefNum(((Def)Items.GetObjectAt(i)).DefNum);
					}
					else if(Items.GetObjectAt(i) is Provider) {
						SetSelectedProvNum(((Provider)Items.GetObjectAt(i)).ProvNum);
					}
					else {
						SetSelected(i);
					}
					return true;
				}
			}
			return false;
		}
		#endregion Methods - Private

		#region Class - ComboBoxItemCollection
		///<summary>Nested class for the collection of items.  Each items must be a ComboBoxItem, not null.  Each field of a ComboBoxItem may be null.</summary>
		public class ComboBoxItemCollection{
			private ComboBoxOD _comboBoxOdParent;
			private List<ComboBoxItem> _listComboBoxItems;

			public ComboBoxItemCollection(ComboBoxOD comboBoxJ){
				_comboBoxOdParent=comboBoxJ;
				_listComboBoxItems=new List<ComboBoxItem>();
			}

			///<summary>Specify the text to show. Optionally, specify the object represented by that text. Also, optionally display an abbreviation for each item to display in the selected summary above.  There are easier ways to add anything common, so this is really just for difficult cases.</summary>
			public void Add(string text,object item=null,string abbr=null){
				if(item is null) {
					item=text;
				}
				_listComboBoxItems.Add(new ComboBoxItem(text,item,abbr));
				_comboBoxOdParent.Invalidate();
			}

			///<summary>Adds a dummy def called "None", with a DefNum of 0.  If you pass in a string to show instead of "None", you should run in through translation first.</summary>
			public void AddDefNone(string textShowing=null){
				if(textShowing==null){
					Add(Lan.g("combo","None"),new Def());
				}
				else{
					Add(textShowing,new Def());
				}
			}

			///<summary>Adds a list of Defs to the items. Does not Clear first.  Defs will show as ItemName, and (hidden) if applicable.</summary>
			public void AddDefs(List<Def> listDefs){
				Func<Def,string> funcItemToString= x => {
					if(x.IsHidden){
						return x.ItemName+" "+Lan.g("Defs","(hidden)");
					}
					return x.ItemName;
				};
				AddList(listDefs,funcItemToString);
			}

			///<summary>Adds the values of an enum to the list of Items.  Does not Clear first.  Descriptions are pulled from DescriptionAttribute or .ToString, then run through translation.  If you want add only some enums, or in a different order, use AddListEnum<T>(IEnumerable<T> items). If you want to display ShortDescriptionAttribute, you have to add the Enums individually with your own text.</summary>
			public void AddEnums<T>() where T : Enum {//struct,IConvertible{
				AddList(Enum.GetValues(typeof(T)).Cast<T>(),x => GetEnumTranslation<T>(x));
				_comboBoxOdParent.Invalidate();
			}

			///<summary>Like AddEnums<T>(), but you can provide a subset of items if needed.</summary>
			public void AddListEnum<T>(IEnumerable<T> items) where T : Enum {
				AddList(items,x => GetEnumTranslation<T>(x));
			}

			///<summary>Translates the result of GetEnumDescription(Enum value).</summary>
			private string GetEnumTranslation<T>(Enum value) where T : Enum {
				return Lan.g("enum"+typeof(T).Name,GetEnumDescription(value));
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

			///<summary>Adds a collection to the items. Does not Clear first.  funcItemToString specifies a string to be displayed for this item, example x=>x.LName or x=>x.ToString(). funcItemToAbbr is similar for the abbreviation used for summary at top of list and for GetStringSelectedItems.  See bottom of this file for example.  Providers and Defs have their own simpler methods for adding.</summary>
			public void AddList<T>(IEnumerable<T> items,Func<T,string> funcItemToString,Func<T,string> funcItemToAbbr=null){
				//It was too dangerous to make this optional.  People forget.  The string to show needs to be required.
				//funcItemToString=funcItemToString??(x => x.ToString());
				string abbr=null;
				foreach(T item in items) {
					if(funcItemToAbbr!=null){
						abbr=funcItemToAbbr(item);
					}
					_listComboBoxItems.Add(new ComboBoxItem(funcItemToString(item),item,abbr));
				}
				_comboBoxOdParent.Invalidate();
			}

			///<summary>Adds a dummy object called "None", with a key of 0.</summary>
			public void AddNone<T>() where T:new(){
				Add(Lan.g("combo","None"),new T());
			}

			///<summary>Adds a dummy provider called "None", with a ProvNum of 0.  If you pass in a string to show instead of "None", you should run in through translation first.</summary>
			public void AddProvNone(string textShowing=null){
				if(textShowing==null){
					Add(Lan.g("combo","None"),new Provider(){Abbr=Lan.g("combo","None") });
				}
				else{
					Add(textShowing,new Provider());
				}
			}

			///<summary>Adds a list of Providers to the items. Does not Clear first.  Providers will show as Abbr with (hidden) if applicable.</summary>
			public void AddProvsAbbr(List<Provider> listProviders){
				AddList(listProviders,x=>x.GetAbbr(),x=>x.GetAbbr());
			}

				///<summary>Adds a list of Providers to the items. Does not Clear first.  Providers will show with Long Descriptions.</summary>
			public void AddProvsFull(List<Provider> listProviders){
				AddList(listProviders,x=>x.GetLongDesc(),x=>x.GetAbbr());
			}

			public void Clear(){
				_listComboBoxItems.Clear();
				_comboBoxOdParent._listSelectedIndices.Clear();
				_comboBoxOdParent.OnSelectedIndexChanged(this,new EventArgs());
				_comboBoxOdParent.Invalidate();
			}

			public int Count{
				get => _listComboBoxItems.Count;
			}

			///<summary>Can't use this because it's unclear if you want the entire ComboBoxItem, the text, or the object.  Use a Get method instead.</summary>
			[Obsolete("Can't use this because it's unclear if you want the entire ComboBoxItem, the text, or the object.  Use a Get method instead.")]
			public object this[int index]{
				get { 
					return null;
				}
			}

			///<summary>This gives you the entire ComboBoxItem.  If you just want the Item object, use [i] or GetObjectAt(i). Can be null.</summary>
			public ComboBoxItem GetComboBoxItemAt(int index){
				if(index>=_listComboBoxItems.Count){
					return null;
				}
				return _listComboBoxItems[index];
			}

			///<summary>This gives you just your Object in the list, not the whole ComboBoxItem container.  You already know what type of object this is, so just cast the type as needed. Can be null</summary>
			public object GetObjectAt(int index){
				if(index>=_listComboBoxItems.Count){
					return null;
				}
				return _listComboBoxItems[index].Item;
			}

			///<summary>Returns a list of all objects in the list of items, not just the selected ones.</summary>
			public List<T> GetAll<T>(){
				List<T> listT=new List<T>();
				for(int i=0;i<_listComboBoxItems.Count;i++){
					listT.Add((T)_listComboBoxItems[i].Item);
				}
				return listT;
			}

			///<summary>Tries to use, in this order: Abbr, Text, Item.ToString(). If all are null, returns "null".</summary>
			public string GetAbbrShowingAt(int index){
				if(index>=_listComboBoxItems.Count){
					return "null";
				}
				if(_listComboBoxItems[index].Abbr==null && _listComboBoxItems[index].Text==null && _listComboBoxItems[index].Item==null){
					return "null";
				}
				if(_listComboBoxItems[index].Abbr!=null){
					return _listComboBoxItems[index].Abbr;
				}
				if(_listComboBoxItems[index].Text!=null){
					return _listComboBoxItems[index].Text;
				}
				return _listComboBoxItems[index].Item.ToString();
			}

			///<summary>Tries to use, in this order: Text, Item.ToString(). If both are null, returns "null".</summary>
			public string GetTextShowingAt(int index){
				if(index>=_listComboBoxItems.Count){
					return "null";
				}
				if(_listComboBoxItems[index].Text==null && _listComboBoxItems[index].Item==null){
					return "null";
				}
				if(_listComboBoxItems[index].Text!=null){
					return _listComboBoxItems[index].Text;
				}
				return _listComboBoxItems[index].Item.ToString();
				
			}
		}
		#endregion Class - ComboBoxItemCollection

		

	}

	#region Class - ComboBoxItem
	///<summary>Storage for an item in a comboBox.    This class also allows you to create items such as ComboBoxItem or List&lt;ComboBoxItem&gt; from outside the comboBox and pass them in later.  If Text is null, Item.ToString() will be displayed.</summary>
	public class ComboBoxItem{

		public ComboBoxItem(){
			
		}

		///<summary>text shows in combobox, item is any full object of interest, and abbr is used for displaying summary of selected items at top.</summary>
		public ComboBoxItem(string text,object item=null,string abbr=null){
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
	#endregion Class - ComboBoxItem

}


//To replace an existing ComboBox with a ComboBoxOD or a ComboBoxClinicPicker:
//Edit the designer.cs to change the type.
//Fix all the places where this breaks the code.
//Adapt existing code to the patterns supported by the new comboBox type.
//Review every instance of the comboBox in that class, even the ones that didn't break.
//
//PROVIDER as a field:--------------------------------------------------------------------------------------------
//comboProv.Items.Clear();//skip in Load()
//comboProv.Items.AddProvNone();//optional
//comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
//comboProv.SetSelectedProvNum(adj.ProvNum);
//...
//adj.ProvNum=comboProv.GetSelectedProvNum();
//
//PROVIDERS on reports:--------------------------------------------------------------------------------------------
//(in Designer) SelectionModeMulti=true;
//comboProvs.IncludeAll=true;
//comboProvs.Items.AddProvsFull(Providers.GetListReports());
//comboProvs.IsAllSelected=true;
//...
//string stringDisplayProvs=comboProvs.GetStringSelectedItems();
//List<long> listProvNums=comboProvs.GetSelectedProvNums();
//RunReport(listProvNums,stringDisplayProvs);
//
//DEF:--------------------------------------------------------------------------------------------------------------
//comboDefs.Items.Clear();//skip in Load()
//comboDefs.Items.AddDefNone();//optional
//comboDefs.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptStatus,true));
//comboDefs.SetSelectedDefNum(appt.AptStatus); 
//...
//appt.AptStatus=comboDefs.GetSelectedDefNum();
//
//ENUM---------------------------------------------------------------------------------------------------------------
//comboArea.Items.AddEnums<EnumArea>();
//Or, rarely: comboArea.Items.Add(Lan.g("enumArea","First Item"),EnumArea.FirstItem);
//Or, to exclude an enum:
//List<EraAutomationMode> listEraAutomationModeValues=typeof(EraAutomationMode).GetEnumValues()
//				.AsEnumerable<EraAutomationMode>()
//				.Where(x => x!=EraAutomationMode.UseGlobal)
//				.ToList();
//comboEraAutomation.Items.AddListEnum(listEraAutomationModeValues);
//To select an enum after filling the list:
//comboArea.SetSelected((int)proc.Area);
//Or: comboArea.SetSelectedEnum(proc.Area);//type is inferred 
//...
//proc.Area=comboArea.GetSelected<EnumArea>();
//
//Other db table types----------------------------------------------------------------------------------------------
//These are a little more complex, but they are really hardly ever used
//comboObj.Items.Clear();//skip in Load()
//comboObj.IncludeAll=true;//optional
//comboObj.Items.AddNone<ObjType>();//optional
//comboObj.Items.AddList(listObjs,x=>x.LName);//the abbr parameter is usually skipped. <T> is inferred.
//comboObj.SetSelectedKey<ObjType>(adj.ObjNum,x=>x.ObjNum,x=>Objs.GetName(x)); 
//...
//adj.ObjNum=comboObj.GetSelectedKey<ObjType>(x=>x.ObjNum);



