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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace WpfControls.UI{
	/*
Jordan is the only one allowed to edit this file.
Also see the similar controls: ComboClinic and UI.ListBox
Height should always be 21.
This comboBox is optimized for very long lists that are even taller than the screen.
Does not allow text entry.  It will always behave like a dropdown listbox.
Exception to above: It sometimes needs to display text for an item that's not in the list due to perms, but still no text entry.
Naming should be like combo... Even though comboBox... might have made more sense, we don't want to bother renaming all existing comboBoxes.

PROVIDER as a field:--------------------------------------------------------------------------------------------
comboProv.Items.Clear();//skip in Load()
comboProv.Items.AddProvNone();//optional
comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
comboProv.SetSelectedProvNum(adj.ProvNum);
...
adj.ProvNum=comboProv.GetSelectedProvNum();

PROVIDERS on reports:--------------------------------------------------------------------------------------------
(in Designer) SelectionModeMulti=true;
comboProvs.IncludeAll=true;
comboProvs.Items.AddProvsFull(Providers.GetListReports());
comboProvs.IsAllSelected=true;
...
string stringDisplayProvs=comboProvs.GetStringSelectedItems();
List<long> listProvNums=comboProvs.GetSelectedProvNums();
RunReport(listProvNums,stringDisplayProvs);

DEF:--------------------------------------------------------------------------------------------------------------
comboDefs.Items.Clear();//skip in Load()
comboDefs.Items.AddDefNone();//optional
comboDefs.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptStatus,isShort:true));
comboDefs.SetSelectedDefNum(appt.AptStatus); 
...
appt.AptStatus=comboDefs.GetSelectedDefNum();

ENUM---------------------------------------------------------------------------------------------------------------
comboArea.Items.AddEnums<EnumArea>();
Or, rarely: comboArea.Items.Add(Lan.g("enumArea","First Item"),EnumArea.FirstItem);
Or, to exclude an enum:
List<EraAutomationMode> listEraAutomationModeValues=typeof(EraAutomationMode).GetEnumValues()
				.AsEnumerable<EraAutomationMode>()
				.Where(x => x!=EraAutomationMode.UseGlobal)
				.ToList();
comboEraAutomation.Items.AddListEnum(listEraAutomationModeValues);
To select an enum after filling the list:
comboArea.SetSelected((int)proc.Area);
Or: comboArea.SetSelectedEnum(proc.Area);//type is inferred 
...
proc.Area=comboArea.GetSelected<EnumArea>();

Other db table types----------------------------------------------------------------------------------------------
These are a little more complex, but they are really hardly ever used
comboObj.Items.Clear();//skip in Load()
comboObj.IncludeAll=true;//optional
comboObj.Items.AddNone<ObjType>();//optional
comboObj.Items.AddList(listObjs,x=>x.LName);//the abbr parameter is usually skipped. <T> is inferred.
comboObj.SetSelectedKey<ObjType>(adj.ObjNum,x=>x.ObjNum,x=>Objs.GetName(x)); 
...
adj.ObjNum=comboObj.GetSelectedKey<ObjType>(x=>x.ObjNum);

	*/

	///<summary></summary>
	public partial class ComboBox : UserControl{
		#region Fields - Private
		private bool _isExpanded;
		private bool _isHover;
		private bool _isMultiSelect=false;
		private bool _isMouseDown;
		///<summary>If selected index is -1, this can be used to store and retrieve the primary key. _textWhenMissing is what shows to the user.</summary>
		private long _keyWhenMissing=0;//odd note: FormClaimCustomTrackingUpdate sets this to -1 to indicate none, which should be fine.
		private SolidColorBrush _solidColorBrushHoverBackground=new SolidColorBrush(Color.FromArgb(10,0,100,255));//20% blue wash
		private SolidColorBrush _solidColorBrushHoverBorder=new SolidColorBrush(Color.FromRgb(126,180,234));//blue
		///<summary>This gets set when the user selects an item that is not present in the list. Selected index is also set to -1.
		private string _textWhenMissing="";
		private string _textForAllOption="All";
		private WindowComboPicker _windowComboPicker;
		private Color _colorBack=Colors.White;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _includeAll=false;
		private bool _isAllSelected=false;
		///<summary>This is the only internal storage for tracking selected indices.  All properties refer to this same list. This list never includes the All option.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		#endregion Fields - Private for Properties

		#region Constructor
		public ComboBox(){
			InitializeComponent();
			//Width=100;//Can't do this, or comboClinc gets messed up
			//Height=21;
			Focusable=true;//so that .Focus() will work in Grid for example
			MouseLeave+=comboBox_MouseLeave;
			MouseLeftButtonDown+=comboBox_MouseLeftButtonDown;
			MouseLeftButtonUp+=comboBox_MouseLeftButtonUp;
			MouseMove+=comboBox_MouseMove;
			Items=new ComboBoxItemCollection(this);
		}
		#endregion Constructor

		#region Events
		///<summary>Occurs when user selects item(s) from the drop-down list and the drop-down closes. This is designed to fire whether or not changed. For example, FormPatientAddAll.ComboClinic1 needs to change the others even if it doesn't change. Use SelectionTrulyChanged if you don't want that behavior.</summary>
		[Category("OD")]
		[Description("Occurs when user selects item(s) from the drop-down list and the drop-down closes. This is designed to fire whether or not changed. For example, FormPatientAddAll.ComboClinic1 needs to change the others even if it doesn't change. Use SelectionTrulyChanged if you don't want that behavior.")]
		public event EventHandler SelectionChangeCommitted;

		///<summary>Occurs when user selects item(s) from the drop-down list and the drop-down closes. Unlike SelectionChangeCommitted, this will only fire when item truly changes.</summary>
		[Category("OD")]
		[Description("Occurs when user selects item(s) from the drop-down list and the drop-down closes. Unlike SelectionChangeCommitted, this will only fire when item truly changes.")]
		public event EventHandler SelectionTrulyChanged;

		///<summary>Try not to use this. The preferred technique is to use SelectionChangeCommitted or SelectionTrulyChanged to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.</summary>
		[Category("OD")]
		[Description("Try not to use this. The preferred technique is to use SelectionChangeCommitted or SelectionTrulyChanged to react to each user click. In contrast, this event will fire even if the selection programmatically changes. Rarely, you might actually want that behavior.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events

		#region Properties - Public Browsable
		//We're not going to do this at first.  Only used in a few internal tools.
		//public bool AllowScroll { get; set; } = true;

		[Category("OD")]
		[DefaultValue(false)]
		public bool IsMultiSelect{
			get{
				return _isMultiSelect;
			}
			set{
				_isMultiSelect=value;
			}
		}

		//[Category("OD")]
		//[DefaultValue(int.MaxValue)]
		//[Description("Use this instead of TabIndex.")]
		//public int TabIndexOD{
			 //TabIndex is just for textboxes for now.
		//}
		#endregion Properties - Public Browsable

		#region Properties - Public not Browsable
		/// <summary>Gets/Sets the background color for the combobox</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(typeof(Color),"White")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				grid.Background=new SolidColorBrush(value);
			}
		}

		/// <summary>Returns if the combobox is expanded or not.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsExpanded{
			get{ return _isExpanded;}
		}

		///<summary>Set to true to include 'All' as a selection option at the top. 'All' can sometimes be intended to indicate more items than are actually showing in list.  Test IsAllSelected separately if this is the case.  This works for both single and multi selection mode.  This extra row is never part of the Items or internal _listSelectedIndices.  But, if you get SelectedIndices, and the user has selected All, then indices for all the items in the list will be returned.</summary>
		[Browsable(false)]//because it's better to see this in the code than to only see it in the designer.  Intentionally different than ComboClinic.
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(false)]
		public bool IncludeAll {
			//Note: Intentionally not adding other special types like "none".  
			//Those are easy to implement manually, whereas "all" is more complex and benefits more from internalization.
			get {
				return _includeAll;
			}
			set {
				_includeAll=value;
				SetText();
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
				SetText();
			}
		}

		///<summary>Gets or sets the selected index. Setter has same behavior for SelectionModeMulti or not. Get throws exception for SelectionModeMulti.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex { 
			get{
				if(IsMultiSelect){
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
				if(_listSelectedIndices.Contains(value)){
					return;//because it was set programmatically, we shouldn't fire event
				}
				_listSelectedIndices.Clear();
				_textWhenMissing="";
				_keyWhenMissing=0;
				if(value!=-1){
					_listSelectedIndices.Add(value);
				}
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetText();
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
				if(!IsMultiSelect){
					throw new Exception("Cannot set SelectedIndices when not IsMultiSelect. Use SelectedIndex, SetSelected, etc.");
				}
				_listSelectedIndices.Clear();
				for(int i = 0;i<value.Count;i++) {
					if(value[i]<-1 || value[i]>Items.Count-1){
						continue;//ignore out of range
					}
					_listSelectedIndices.Add(value[i]);
				}
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetText();
			}
		}

		///<summary>Do not normally use the getter. Mostly just for compatability with the standard Microsoft combobox. Instead, use GetSelected with type in angle brackets.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedItem { 
			get {
				if(_listSelectedIndices.Count==0) {
					return null;
				}
				ComboBoxItem comboBoxItem=Items.GetComboBoxItemAt(_listSelectedIndices[0]);
				if(comboBoxItem.Item != null) {
					return comboBoxItem.Item;
				}
				if(comboBoxItem.Text != null) {
					return comboBoxItem.Text;
				}
				return null;
			}
			set{
				for(int i = 0;i<Items.Count;++i) {
					if(Items.GetObjectAt(i)?.Equals(value)??false) {
						SetSelected(i);
					}
					if(Items.GetTextShowingAt(i)?.Equals(value)??false) {
						SetSelected(i);
					}
				}
				//If not found in list, don't change selection. Consistent with MS.
			}
		}

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
		#endregion Properties - Public not Browsable

		#region Methods - Public
		///<summary>Returns a list of the selected objects. List can be empty.  Items can be null. If special 'All' is set in multi-mode(regardless of other selections), this will include all objects in the combobox.  If 'All' is set in single mode, it will be ignored here and list could be empty.</summary>
		public List<T> GetListSelected<T>() {
			List<T> listSelected=new List<T>();
			if(IsMultiSelect && IsAllSelected){
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

		///<summary>Gets the selected object.  Can be null for object or 0 for enum.  Throws exception for SelectionModeMulti. Never includes All.</summary>
		public T GetSelected<T>() {
			if(IsMultiSelect){
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

		///<summary>Gets the key like PatNum from the selected index. ProvNum and DefNum have their own selectors.  funcSelectKey example x=>x.PatNum.  If selected index is -1, it will try to grab the key that was passed in earlier with SetSelectedKey.  If there is none, then it will return 0.  Completely ignores IsAllSelected, so if you are interested in that, test it first. Throws exception for MultiSelect.</summary>
		public long GetSelectedKey<T>(Func<T,long> funcSelectKey){
			//if(IsMultiSelect){
			//	throw new Exception("GetSelected is ambiguous when IsMultiSelect.");
			//can't do this because we call it from a multiselect in ComboClinic ListClinicNumsSelected
			//}
			if(_listSelectedIndices.Count==0){
				return _keyWhenMissing;//could be zero
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
					return _textForAllOption;//"All";
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

		///<summary>Ignore. Just used in one spot: FormScheduleDayEdit for convenience.</summary>
		public string GetText(){
			return textBlock.Text;
		}

		///<summary>Sets all rows either selected or unselected. If 'All' is present, it gets ignored because it's handled separately from the normal items.</summary>
		public void SetAll(bool setToValue){
			_listSelectedIndices.Clear();
			if(setToValue){//if setting all true
				if(!IsMultiSelect){
					throw new Exception("SetAll is not allowed when not IsMultiSelect.");
				}
				for(int i=0;i<Items.Count;i++){
					_listSelectedIndices.Add(i);
				}
			}
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			SetText();
		}

		///<summary>Sets one row to selected. Does not clear other selected rows unless single selection mode.</summary>
		public void SetSelected(int index){
			SetSelected(index,true);
		}

		///<summary>Sets one row either selected or unselected. Does not clear other selected rows unless single selection mode.</summary>
		public void SetSelected(int index,bool value){
			if(value){//setting true
				if(_listSelectedIndices.Contains(index)){
					return;//nothing to do
				}
				if(!IsMultiSelect){
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
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			SetText();
		}

		///<summary>Only for comboBoxes with a list of Defs. If you set a DefNum that is hidden from the combo, this method will go try to find it and display the name, including (hidden) if that applies.  If you set defNum to 0, it will show "none" or could also show text for any dummy 0 def that you added manually to the combo.</summary>
		public void SetSelectedDefNum(long defNum){
			if(defNum==0){//special handling for 0. We don't want to go looking in the cache.
				_listSelectedIndices.Clear();
				_textWhenMissing="";
				_keyWhenMissing=0;
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
						SelectedIndexChanged?.Invoke(this,new EventArgs());
						SetText();
						return;
					}
				}
				//0 is not in list
				_textWhenMissing=Lans.g("Defs","none");
				_keyWhenMissing=0;//still, and selectedIndex is -1
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetText();
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
			_textWhenMissing="";
			_keyWhenMissing=0;
			for(int i=0;i<Items.Count;i++) {
				if(Items.GetObjectAt(i)==null){
					continue;
				}
				if(typeof(T)!=Items.GetObjectAt(i).GetType()) {
					continue;
				}
				if(funcSelectKey((T)Items.GetObjectAt(i))==key) {
					_listSelectedIndices.Add(i);
					SelectedIndexChanged?.Invoke(this,new EventArgs());
					SetText();
					return;
				}
			}
			//couldn't locate key in list
			//Ignore this note: we do have the option of including a parameter in this method that makes the caller pass in a func that will attempt to get the actual object.
			//Like this: x=>Providers.GetProv(x).  That would be handy to test for null, but we would still need a func to get the overrideText from that.
			//It also has the downside of no fixed text, and I also don't see a spot where it's needed, so not doing that.
			if(funcOverrideText==null){
				_textWhenMissing=key.ToString();
			}
			else{
				_textWhenMissing=funcOverrideText(key);
			}
			if(_textWhenMissing==null || _textWhenMissing==""){
				_textWhenMissing=key.ToString();//show the number because we don't want to show nothing
			}
			_keyWhenMissing=key;
			SelectedIndexChanged?.Invoke(this,new EventArgs());
			SetText();
		}

		///<summary>Only for comboBoxes with a list of Providers. This is a specific use of SetSelectedKey. If the ProvNum is not currently in the comboBox and can't be matched, then selectedIndex is set to -1, the key is stored internally for reference, and the prov. It will remember that key and return it back in any subsequent GetSelectedKey as long as selectedIndex is still -1.  Setting ProvNum to 0 works well if you already used Items.AddProvNone(). But even if you don't have a dummy 0 provider, it will still work well and show "none", leaving SelectedIndex -1.</summary>
		public void SetSelectedProvNum(long provNum){
			if(provNum==0){//special handling for 0. We don't want to go looking in the cache.
				_listSelectedIndices.Clear();
				_textWhenMissing="";
				_keyWhenMissing=0;
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
						SelectedIndexChanged?.Invoke(this,new EventArgs());
						SetText();
						return;
					}
				}
				//0 is not in list
				_textWhenMissing=Lans.g("Providers","none");
				_keyWhenMissing=0;//still, and selectedIndex is -1
				SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetText();
				return;
			}
			SetSelectedKey<Provider>(provNum,x=>x.ProvNum,x=>Providers.GetAbbr(x,true));//won't use GetAbbr unless it has to
			//In case the provider long descriptions are being used, this method won't know about that, and will just use Abbr,
			//but it will include (hidden), so that should be more than acceptable.
		}

		///<summary>If IncludeAll, this allows us to set the text. Otherwise, the default is All.</summary>
		public void SetTextForAllOption(string text){
			_textForAllOption=text;
		}
		#endregion Methods - Public

		#region Methods - event handlers, mouse
		private void comboBox_MouseLeave(object sender,MouseEventArgs e) {
			_isHover=false;
			SetColors();
		}

		private void comboBox_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			_isMouseDown=true;
		}

		private void comboBox_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			//We're using mouse up because if mouse down was used, and the new window fell on the cursor, and the window position had to be adjusted,
			//then the mouse would still be down and it would automatically select a group of items.
			//if(_windowComboPicker != null && _windowComboPicker.IsVisible){//this doesn't work because it would have already just closed when it lost focus
			if(!_isMouseDown){//Check before setting false. Prevent MouseUp from triggering if MouseDown didn't occur on the combo box.
				return;
			}
			_isMouseDown=false;
			if(_isExpanded){
				//_windowComboPicker.Close();//It closes automatically when it loses focus, so this isn't necessary
				_isExpanded=false;
				//e.Handled=true;//prevents mousedown from firing, which would expand it again
				return;
			}
			if(Items.Count==0){//even if IncludeAll, we still don't want it to show.
				return;
			}
			_isExpanded=true;
			_windowComboPicker=new WindowComboPicker();
			ElementHost.EnableModelessKeyboardInterop(_windowComboPicker);
			_windowComboPicker.Closed+=_windowComboPicker_Closed;
			_windowComboPicker.PreviewMouseLeftButtonUp+=_windowComboPicker_PreviewLeftButtonUp;
			List<string> listStrings=new List<string>();
			List<string> listAbbrevs=new List<string>();
			List<int> selectedIndices=new List<int>();
			int indexOffset=0;
			if(IncludeAll){
				listStrings.Add(_textForAllOption);
				listAbbrevs.Add(_textForAllOption);
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
			_windowComboPicker.ListStrings=listStrings;
			_windowComboPicker.ListAbbrevs=listAbbrevs;
			_windowComboPicker.PointInitial=PointToScreen(new Point(0,ActualHeight));
			//_windowComboPicker.MinimumSize=new Size(15,15);
			_windowComboPicker.Width=ActualWidth;
			if(IsMultiSelect){
				_windowComboPicker.IsMultiSelect=true;
				_windowComboPicker.ListIndicesSelected=selectedIndices;
				_windowComboPicker.SelectionChanged+=_windowComboPicker_SelectionChanged;
			}
			else{
				if(selectedIndices.Count==0){
					_windowComboPicker.SelectedIndex=-1;
				}
				else{
					_windowComboPicker.SelectedIndex=selectedIndices[0];
				}
			}
			_windowComboPicker.Show();
		}

		private void comboBox_MouseMove(object sender,MouseEventArgs e) {
			_isHover=true;
			SetColors();
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(isMouseDown){
				return;
			}
			if(_windowComboPicker != null && _windowComboPicker.IsVisible){
				_isExpanded=true;
			}
			else{
				_isExpanded=false;
			}
		}

		private void _windowComboPicker_Closed(object sender,EventArgs e) {
			//It might be an improvement to order both lists here first before comparing
			bool selectionChanged=false;
			int indexOffset=0;
			if(IncludeAll){
				if(_windowComboPicker.ListIndicesSelected.Contains(0) != IsAllSelected){
					selectionChanged=true;
				}
				indexOffset=1;
			}
			if(IsMultiSelect){
				if(!IsAllSelected){//if all is selected, we ignore other selections
					//Can't test with SequenceEqual because of indexOffset
					//This multiselect is kind of tricky to write so we will just set to true.
					//If someone actually needs this to work someday, we can enhance it.
					selectionChanged=true;
				}
			}
			else{//single select mode
				if(!IsAllSelected){//if all is selected, we ignore other selections
					int selectedIdx=-1;
					if(_listSelectedIndices.Count>0){
						selectedIdx=_listSelectedIndices[0];
					}
					if(selectedIdx!=_windowComboPicker.SelectedIndex-indexOffset){
						selectionChanged=true;
					}
				}
			}
			if(!selectionChanged){
				//These events are always raised, even if no changes.
				SelectionChangeCommitted?.Invoke(this,e);
				SelectedIndexChanged?.Invoke(this,e);
				return;
			}
			_listSelectedIndices.Clear();
			if(IncludeAll){
				if(_windowComboPicker.ListIndicesSelected.Contains(0)){
					IsAllSelected=true;
				}
				else{
					IsAllSelected=false;
				}
			}
			if(IsMultiSelect){
				if(!IsAllSelected){//if all is selected, we ignore other selections
					for(int i=0;i<_windowComboPicker.ListIndicesSelected.Count;i++){
						_listSelectedIndices.Add(_windowComboPicker.ListIndicesSelected[i]-indexOffset);
					}
				}
			}
			else{//single select mode
				if(!IsAllSelected){//if all is selected, we ignore other selections
					if(_windowComboPicker.SelectedIndex!=-1){
						_listSelectedIndices.Add(_windowComboPicker.SelectedIndex-indexOffset);
					}
				}
			}
			SelectionTrulyChanged?.Invoke(this,e);
			SelectionChangeCommitted?.Invoke(this,e);
			SelectedIndexChanged?.Invoke(this,e);
			SetText();
		}

		private void _windowComboPicker_PreviewLeftButtonUp(object sender, MouseButtonEventArgs e){
			if(_windowComboPicker.ScrollBar!=null//Scrollbar should never be null, even if it's not showing, but just in case
				&& _windowComboPicker.ScrollBar.IsMouseOver)
			{
				return;//If the user is using the scrollbar, don't close the combo picker.
			}
			//This prevents clicks from "going through" the item list and clicking on whatever is below the dropdown.
			e.Handled=true;
			_windowComboPicker.Close();
		}

		private void _windowComboPicker_SelectionChanged(object sender,EventArgs e) {
			//only fires in multiSelect 
			if(IncludeAll){
				if(_windowComboPicker.ListIndicesSelected.Contains(0)){
					IsAllSelected=true;
				}
				else{
					IsAllSelected=false;
				}
			}
			if(IsAllSelected){//if all is selected, we ignore other selections
				return;
			}
			_listSelectedIndices.Clear();
			for(int i=0;i<_windowComboPicker.ListIndicesSelected.Count;i++){
				if(IncludeAll){
					_listSelectedIndices.Add(_windowComboPicker.ListIndicesSelected[i]-1);//All is in position 0
				}
				else{
					_listSelectedIndices.Add(_windowComboPicker.ListIndicesSelected[i]);
				}
			}
			SetText();
		}
		#endregion Methods - event handlers, mouse

		#region Methods - private
		///<summary>Uses the full text if one item is selected.  If multiple items are selected, we string abbreviations together with commas.  But if that string is wider than widthMax, we instead show "Multiple".</summary>
		private string GetDisplayText(int widthMax){
			if(IsAllSelected){//works the same for IsMultiselect or not
				//_textWhenMissing is not checked here because All only makes sense for multi, and _textWhenMissing only makes sense for single.
				return _textForAllOption;
			}
			if(_listSelectedIndices.Count==0){
				if(_textWhenMissing!=""){
					return _textWhenMissing;
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
			Typeface typeFace=new Typeface(textBlock.FontFamily.ToString());
				//new Typeface("Segoe UI");
			FormattedText formattedText = new FormattedText(str,CultureInfo.CurrentCulture,FlowDirection.LeftToRight,typeFace,
				textBlock.FontSize,Brushes.Black,VisualTreeHelper.GetDpi(this.textBlock).PixelsPerDip);
			if(formattedText.Width>widthMax){
				return "Multiple";
			}
			return str;
		}

		///<summary></summary>
		private void SetColors(){
			if(_isHover){
				border.Background=_solidColorBrushHoverBackground;
				border.BorderBrush=_solidColorBrushHoverBorder;
				grid.Background=new SolidColorBrush(Colors.White);//so the blue wash looks nicer
			}
			else{
				border.Background=Brushes.Transparent;
				border.BorderBrush=Brushes.DarkGray;
				grid.Background=new SolidColorBrush(_colorBack);
			}
		}

		///<summary></summary>
		private void SetText(){
			textBlock.Text=GetDisplayText((int)textBlock.ActualWidth);
		}
		#endregion Methods - private

		#region Class - ComboBoxItemCollection
		///<summary>Nested class for the collection of items.  Each items must be a ComboBoxItem, not null.  Each field of a ComboBoxItem may be null.</summary>
		public class ComboBoxItemCollection{
			private ComboBox _comboBoxParent;
			private List<ComboBoxItem> _listComboBoxItems;

			public ComboBoxItemCollection(ComboBox comboBox){
				_comboBoxParent=comboBox;
				_listComboBoxItems=new List<ComboBoxItem>();
			}

			///<summary>Specify the text to show. Optionally, specify the object represented by that text. Also, optionally display an abbreviation for each item to display in the selected summary above.  There are easier ways to add anything common, so this is really just for difficult cases.</summary>
			public int Add(string text,object item=null,string abbr=null){
				if(item is null) {
					item=text;
				}
				_listComboBoxItems.Add(new ComboBoxItem(text,item,abbr));
				_comboBoxParent.SetText();
				return _listComboBoxItems.Count-1;//for consistency with MS
			}

			///<summary>Adds a dummy def called "None", with a DefNum of 0.  If you pass in a string to show instead of "None", you should run in through translation first.</summary>
			public void AddDefNone(string textShowing=null){
				if(textShowing==null){
					Add(Lans.g("combo","None"),new Def());
				}
				else{
					Add(textShowing,new Def());
				}
			}

			///<summary>Adds a list of Defs to the items. Does not Clear first.  Defs will show as ItemName, and (hidden) if applicable.</summary>
			public void AddDefs(List<Def> listDefs){
				Func<Def,string> funcItemToString= x => {
					if(x.IsHidden){
						return x.ItemName+" "+Lans.g("Defs","(hidden)");
					}
					return x.ItemName;
				};
				AddList(listDefs,funcItemToString);
			}

			///<summary>Adds the values of an enum to the list of Items.  Does not Clear first.  Descriptions are pulled from DescriptionAttribute or .ToString, then run through translation.  If you want add only some enums, or in a different order, use AddListEnum<T>(IEnumerable<T> items). If you want to display ShortDescriptionAttribute, you have to add the Enums individually with your own text.</summary>
			public void AddEnums<T>() where T : Enum {//struct,IConvertible{
				AddList(Enum.GetValues(typeof(T)).Cast<T>(),x => GetEnumTranslation<T>(x));
				_comboBoxParent.SetText();
			}

			///<summary>Like AddEnums<T>(), but you can provide a subset of items if needed.</summary>
			public void AddListEnum<T>(IEnumerable<T> items) where T : Enum {
				AddList(items,x => GetEnumTranslation<T>(x));
			}

			///<summary>Translates the result of GetEnumDescription(Enum value).</summary>
			private string GetEnumTranslation<T>(Enum value) where T : Enum {
				return Lans.g("enum"+typeof(T).Name,GetEnumDescription(value));
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
			public void AddList<T>(IEnumerable<T> items,Func<T,string> funcItemToString=null,Func<T,string> funcItemToAbbr=null){
				//I've gone back and forth on whether to make this optional.  Option now to allow same functionality as AddRange().
				funcItemToString=funcItemToString??(x => x.ToString());
				string abbr=null;
				foreach(T item in items) {
					if(funcItemToAbbr!=null){
						abbr=funcItemToAbbr(item);
					}
					_listComboBoxItems.Add(new ComboBoxItem(funcItemToString(item),item,abbr));
				}
				_comboBoxParent.SetText();
			}

			///<summary>Adds a dummy object called "None", with a key of 0.</summary>
			public void AddNone<T>() where T:new(){
				Add(Lans.g("combo","None"),new T());
			}

			///<summary>Adds a dummy provider called "None", with a ProvNum of 0.  If you pass in a string to show instead of "None", you should run in through translation first.</summary>
			public void AddProvNone(string textShowing=null){
				if(textShowing==null){
					Add(Lans.g("combo","None"),new Provider(){Abbr=Lans.g("combo","None") });
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
				_comboBoxParent._listSelectedIndices.Clear();
				//_comboBoxOdParent.SelectedIndexChanged?.Invoke(this,new EventArgs());//MS comboBox does not fire this event on Clear.
				_comboBoxParent.SetText();
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
