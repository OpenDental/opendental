using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Newtonsoft.Json;
using OpenDental.Drawing;
using CodeBase;

namespace OpenDental {
	/*
	Control Groups
	Each field is represented by a group of controls.
	For example, we might have a label with a textbox below it. That's one field.
	There are many different ways to stack the controls you need that will depend on your language.
	For the textbox example, we chose a stackpanel with two items, but we could have instead used a dockpanel, a grid, or whatever.
	You should decide on a single container for each field (group of controls).
	The container type can be different for each field type.
	Once you have your various containers for each of the fields, you have to think about how to arrange them.
	In the discussion below, I will refer to these as "fields" instead of "control groups".
	Each "field" is a group of controls that correspond to one eFormField.

	Organization of UI:
	The strategies here are described to be as language agnostic as possible.
	We need to be able to recreate this UI in a variety of languages.
	As you can see in the designer, there is a main vertical StackPanel within a ScrollViewer.
	Each item in the stackPanel is supposed to be a horizontal WrapPanel.
	(unfortunately, we had to put each of these WrapPanels in a grid for our hover, drag, drop, etc. You won't need that)
	These horiz wrapPanels then contain the individual fields which stack horizontally and wrap when needed.
	If EFormField.IsHorizStacking=false, then there will be only one field in that WrapPanel.

	Margins and spacing:
	I apologize for the complexity of this code. 
	Anyone using this code to try to build a UI in a different language should use a much simpler approach.
	You will not have to support drag, drop, hover, and selection in the setup window.
	So try to imagine it simpler:
	There must be a certain amount of white space around each field.
	We plan to give users more control of this white space eventually, but for now, I've used 10 pix.
	You must have a very solid plan for how to create this white space. There are lots of ways to get it wrong.
	Each field should have right and bottom margins.
	We plan to give users control over right and bottom spacing, so it's a requirement.
	It also works great with various stacking scenarios.
	By simply adding a right and bottom margin to every field,
		 you get both spacing between fields as well as the overall bottom and right page margins.
	The StackPanel and WrapPanels should then certainly not have right or bottom margins,
		 because those were already handled by field margins.
	All that remain are the left and top margins of each page.
	Create this whitespace by giving left and top margins to the main StackPanel.
	Be aware that we do plan to eventually give users direct control over these two margins as well.
	In our overly complicated code that you see below, we don't do it as described above.
	Make sure to do it as described above, the simpler way.

	Calculation of Width of Each Field:
	This is one calculation that you will need to do with each layout.
	DO follow my algorithm for calculating width for each field.
	The math is simple, and there's not a shorter way to do it.
	Here's why:
	It's important to set each field to have a fixed width and auto height.
	The fixed width is necessary for it to work in the horiz wrapPanel.
	The auto height is necessary for textboxes that have wrap turned on and will grow vertically
	   or for radiobuttons that consume more than one row for example.
	But we have a big problem: The fixed width of the field must be able to shrink when the screen is too narrow.
	Without this, multiline textboxes will spill off to the right,
	   and single line textboxes will also not scroll horizontally like they should,
	   and radiobutton options will get cut off.
	Unfortunately, there is no combination of control properties that will achieve this behavior.
	I've tried lots of different kinds of nesting, max width, stretch, etc. The complexity explodes.
	Furthermore, we would like to add even more behavior later, like proportional widths.
	And whatever I come up with here must be very easy to implement in any UI language.
	I decided that we must always calculate each field width ourselves as part of layout.
	We still put them in a wrapPanel, and we leave the heights auto, but we must calculate the widths.
	So set the width of each field container very intentionally according to my algorithm.
	To determine available width, subtract the right margin (ex 10) from the StackPanel width.
	The controls inside the field can stretch horizontally to fill; that's easy.
	We also already know any desired width because that comes to us from the field.
	As you can see in the algorithm, we must really only do one calculation manually:
	We must shrink it if it's larger than the available width. That's it.
	That can be done in any UI language.
	It also follows that if the screen size changes, we must redo the layout.
	But screen size really shouldn't be changing unless the user maybe rotates the layout.

	Tags:
	Tags are used as follows:
	1. Each EFormField in the list has its TagOD set to the field(grid in our case) that it's associated with.
			One purpose is to allow us to pull values from the UI into our EFormField objects.
			Another purpose is to allow our highlighting for hover effects and selection in the setup window,
				but the patient UI does not need highlighting, so that can be ignored there.
			These tags also allow "paging" by collapsing the fields not on the current page.
			Finally, the tags allow collapsing for conditional logic.
	2. We also have DragLocation tags on the various borders that we use for drag/drop.
			The patient UI clearly does not need those.

	*/
	///<summary></summary>
	public partial class CtrlEFormFill:UserControl {
		#region Fields - public
		///<summary>In setup mode, the UI behavior for mouse is quite different. Layout is the similar.</summary>
		public bool IsSetupMode;
		///<summary>This is referenced extensively by the parent form. Items are sometimes changed directly, and sometimes converted to EFormFieldDefs, manipulated, and then converted back to EFormFields.</summary>
		public List<EFormField> ListEFormFields;
		#endregion Fields - public

		#region Fields - private
		///<summary>Stores the action that gets invoked when the timer interval has elapsed.</summary>
		private Action _actionTimer;
		///<summary>Typically null unless we are currently dragging. In that case, this might have a value. It will only have one value.</summary>
		private Border _borderDropHover;
		private Cursor _cursorDrag;
		///<summary>Stores the dispatcher timer for switching pages and scrolling.</summary>
		private DispatcherTimer _dispatcherTimer;
		private double _heightGridRow=18;
		private int _hoverIndex=-1;
		///<summary>When we capture mouse, it causes a mouse move event that messes up our logic. This allows us to ignore that event for that one line.</summary>
		private bool _ignoreMouseMove;
		///<summary>True if currently dragging a field or fields.</summary>
		private bool _isDraggingField;
		///<summary>Mouse down was not on a field. Immediately start dragging a selector rectangle. Don't show any borderDropHover.</summary>
		private bool _isDraggingSelector;
		///<summary>We must combine this class level field with Mouse.LeftButton==MouseButtonState.Pressed to avoid two different edge case bugs. Using _isMouseDown prevents bug in following scenario: a combobox above this control has a dropdown that user clicks on. This causes the dropdown to close, triggering a mouse move in here while mouse is still down. Using _isMouseDown lets us ignore this because we didn't actually mouse down in here.</summary>
		private bool _isMouseDown;
		///<summary>This is a list of the borders that are used for drag drop.</summary>
		private List<Border> _listBordersDrops=new List<Border>();
		///<summary>This is the only internal storage for tracking selected indices. Not guaranteed to be ordered least to greatest.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		///<summary>This is a copy of the selected indices whenever we start dragging to select multiple.</summary>
		private List<int> _listSelectedIndicesWhenSelectionStart=new List<int>();
		///<summary>This is the vertical and horizontal white space between each field, as well as the margins of the entire page. Later, this can be split into multiple variables or even database fields.</summary>
		private double _margins=10;
		private int _mouseDownIndex;
		///<summary>The actual page count, including all hidden fields. User only sees _pageCountFiltered.</summary>
		private int _pageCount;
		///<summary>Because conditional logic can hide entire pages, this is the page count with those hidden pages excluded. This is what user sees at the bottom.</summary>
		private int _pageCountFiltered;
		///<summary>One based. Includes all hidden fields, so user only sees _pageShowingFiltered.</summary>
		private int _pageShowing=1;
		///<summary>Because conditional logic can hide entire pages, this is the page showing with those hidden pages excluded. This is what user sees at the bottom. One based.</summary>
		private int _pageShowingFiltered;
		///<summary>In coords of this entire control.</summary>
		private Point _pointMouseDown;
		///<summary>This is class level because when multiple random numbers are quickly generated, they are based on the system clock and will be identical unless Random is reused.</summary>
		private Random _random=new Random();
		#endregion Fields - private

		#region Constructor
		public CtrlEFormFill() {
			InitializeComponent();
			_dispatcherTimer=new DispatcherTimer();
			_dispatcherTimer.Tick+=Timer_Tick;
			//stackPanel.Background=Brushes.Green;
			Loaded+=CtrlEFormFill_Loaded;
			_cursorDrag=new Cursor(new MemoryStream(WpfControls.Properties.Resources.CursorDrag));
		}
		#endregion Constructor

		#region Events
		public event EventHandler<int> EventDoubleClickField;
		#endregion Events

		#region Properties
		///<summary>Gets or sets the selected indices.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<int> SelectedIndices { 
			get{
				//returns a value, not a reference, so you can't clear from here.
				return _listSelectedIndices;
			}
			set{
				_listSelectedIndices.Clear();
				for(int i = 0;i<value.Count;i++) {
					if(value[i]<-1 || value[i]>ListEFormFields.Count-1){
						continue;//ignore out of range
					}
					_listSelectedIndices.Add(value[i]);
				}
				//SelectedIndexChanged?.Invoke(this,new EventArgs());
				SetColors();
			}
		} 
		#endregion Properties

		#region Methods - public
		public void Delete(int idx){
			ListEFormFields.RemoveAt(idx);
		}

		///<summary>If a single field is passed in, only that field will be filled. If null, all fields will be filled. We generally call this (and SetVisibilities) for a single field every time a user clicks a checkbox or radiobutton. This is so we can have immediate results for conditional logic. But we don't call it for textboxes, for example, but it's hard to know when to do so.</summary>
		public void FillFieldsFromControls(EFormField eFormField=null){			
			for(int i=0;i<ListEFormFields.Count;i++){
				if(eFormField!=null && ListEFormFields[i]!=eFormField){
					continue;
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
					Grid gridForField=ListEFormFields[i].TagOD as Grid;
					CheckBox checkBox=gridForField.Children[1] as CheckBox;
					if(checkBox.IsChecked==true) {
						ListEFormFields[i].ValueString="X";
						continue;
					}
					ListEFormFields[i].ValueString="";
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.DateField){
					Grid gridForField=ListEFormFields[i].TagOD as Grid;
					StackPanel stackPanel=gridForField.Children[1] as StackPanel;
					WpfControls.UI.TextVDate textVDate=stackPanel.Children[1] as WpfControls.UI.TextVDate;
					ListEFormFields[i].ValueString=textVDate.Text;
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.Label){
					Grid gridForField=ListEFormFields[i].TagOD as Grid;
					WpfControls.UI.TextRich textRich=gridForField.Children[1] as WpfControls.UI.TextRich;
					ListEFormFields[i].ValueLabel=EFormFields.SerializeFlowDocument(textRich.richTextBox.Document);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.MedicationList){
					Grid gridForField=ListEFormFields[i].TagOD as Grid;
					StackPanel stackPanelVert=gridForField.Children[1] as StackPanel;
					Grid gridMeds=stackPanelVert.Children[1] as Grid;
					List<EFormMed> listEFormMeds=new List<EFormMed>();
					for(int m=1;m<gridMeds.RowDefinitions.Count;m++){//start at row 1 to skip header
						EFormMed eFormMed=new EFormMed();
						for(int c=0;c<gridMeds.Children.Count;c++){
							if(!(gridMeds.Children[c] is WpfControls.UI.TextBox)){
								continue;
							}
							WpfControls.UI.TextBox textBox=gridMeds.Children[c] as WpfControls.UI.TextBox;
							if(Grid.GetRow(textBox)!=m){//wrong row
								continue;
							}
							if(Grid.GetColumn(textBox)==0){
								if(textBox.Text==""){
									goto EndOfLoop;//A row with an empty med is equivalent to no row at all 
								}
								eFormMed.MedName=textBox.Text;
							}
							if(Grid.GetColumn(textBox)==1){
								eFormMed.StrengthFreq=textBox.Text;
							}
						}
						listEFormMeds.Add(eFormMed);
						EndOfLoop:
						continue;
					}
					ListEFormFields[i].ValueString=JsonConvert.SerializeObject(listEFormMeds);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.RadioButtons){
					Grid gridForField=ListEFormFields[i].TagOD as Grid;
					StackPanel stackPanel=gridForField.Children[1] as StackPanel;
					WrapPanel wrapPanel=stackPanel.Children[1] as WrapPanel;
					UIElementCollection uIElementCollection=wrapPanel.Children;
					List<string> listPickDb=ListEFormFields[i].PickListDb.Split(',').ToList();
					List<string> listPickVis=ListEFormFields[i].PickListVis.Split(',').ToList();
					for(int c=0;c<uIElementCollection.Count;c++){
						WpfControls.UI.RadioButton radioButton=uIElementCollection[c] as WpfControls.UI.RadioButton;
						if(!radioButton.Checked){
							continue;
						}
						if(ListEFormFields[i].DbLink==""){//none
							ListEFormFields[i].ValueString=listPickVis[c];
						}
						else{//db link
							ListEFormFields[i].ValueString=listPickDb[c];
						}
					}
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.TextField){
					Grid gridForField=ListEFormFields[i].TagOD as Grid;
					StackPanel stackPanel=gridForField.Children[1] as StackPanel;
					TextBox textBox=stackPanel.Children[1] as TextBox;
					ListEFormFields[i].ValueString=textBox.Text;
				}
			}
		}

		///<summary>Will never be 0 because 1-based.</summary>
		public int GetPageShowing() {
			return _pageShowing;
		}

		///<summary>Returns the selected index from the eForm. If multiple selected, it will return the first one in the list. If the list is empty or null, it will return -1.</summary>
		public int GetSelectedIndex() {
			if(_listSelectedIndices!=null && _listSelectedIndices.Count>0) {
				return _listSelectedIndices[0];
			}
			return -1;
		}

		///<summary>Returns the full list of selected indices. If the list is empty or null, it will return an empty list.</summary>
		public List<int> GetSelectedIndices() {
			if(_listSelectedIndices!=null && _listSelectedIndices.Count>0) {
				return _listSelectedIndices;
			}
			return new List<int>();
		}

		///<summary>Fixes stacking, clears the stackpanel, adds back all new fresh controls to the stackpanel, scrolls back to the same offset as before, reselects if one was selected.</summary>
		public void RefreshLayout(){
			FixAllStacking();
			int idxSelected=-1;
			if(_listSelectedIndices.Count==1){
				idxSelected=_listSelectedIndices[0];
			}
			double verticalOffset=scrollViewer.VerticalOffset;
			stackPanel.Children.Clear();
			borderSelect.Visibility=Visibility.Collapsed;
			_pageCount=1;
			_pageShowingFiltered=1;
			WrapPanel wrapPanel=null;
			Grid gridWrap=null;
			_listBordersDrops=new List<Border>();
			_listSelectedIndices=new List<int>();//This is done similarly in Grid.EndUpdate() so I want keep the same pattern.
			//_listGridFields=new List<Grid>();
			for(int i=0;i<ListEFormFields.Count;i++){
				ListEFormFields[i].Page=_pageCount;
				if(i==0 //top of first page
					|| ListEFormFields[i-1].FieldType==EnumEFormFieldType.PageBreak)//top of other pages
				{
					Border borderTopOfPage=CreateBorderForDrop();
					borderTopOfPage.Height=_margins;
					borderTopOfPage.Margin=new Thickness(left:_margins,0,right:_margins,0);
					DragLocation dragLocationTopOfPage=new DragLocation();
					dragLocationTopOfPage.Idx=i;
					dragLocationTopOfPage.IsHorizStacking=false;
					dragLocationTopOfPage.IsPrimary=true;
					dragLocationTopOfPage.IsAbove=true;
					dragLocationTopOfPage.Page=_pageCount;
					borderTopOfPage.Tag=dragLocationTopOfPage;
					stackPanel.Children.Add(borderTopOfPage);
				}
				if(i==0//first row
					|| !ListEFormFields[i].IsHorizStacking//or it belongs on a new line
					//There's an edge case for the first item on a page being set to horiz.
					//That will be ignored as invalid. Here's how to do that:
					|| ListEFormFields[i-1].FieldType==EnumEFormFieldType.PageBreak)
				{
					wrapPanel=new WrapPanel();
					wrapPanel.Orientation=Orientation.Horizontal;
					//But because we need a margin on the left of the page,
					//and that margin needs to be filled with a border for hover effects,
					//we will actually fill this stackpanel with grids.
					wrapPanel.Margin=new Thickness(left:_margins,0,0,0);
					gridWrap=new Grid();
					//gridWrap.Tag=_pageCount;
					//This drag/drop border is to the left of the entire wrap panel
					Border borderLeftOfWrap=CreateBorderForDrop();
					borderLeftOfWrap.Width=_margins;
					borderLeftOfWrap.HorizontalAlignment=HorizontalAlignment.Left;
					DragLocation dragLocationLeftOfWrap=new DragLocation();
					dragLocationLeftOfWrap.Idx=i;
					dragLocationLeftOfWrap.IsHorizStacking=false;
					//if(EFormFieldDefs.IsHorizStackable(ListEFormFields[i].FieldType)) {
					dragLocationLeftOfWrap.IsHorizStackingNext=true;//the item to the right should get set to stack
					//}
					dragLocationLeftOfWrap.IsPrimary=true;//now this is a problem for radiobuttons
					dragLocationLeftOfWrap.IsLeftOfWrap=true;
					dragLocationLeftOfWrap.Page=_pageCount;
					borderLeftOfWrap.Tag=dragLocationLeftOfWrap;
					gridWrap.Children.Add(borderLeftOfWrap);
					gridWrap.Children.Add(wrapPanel);
				}
				//otherwise, keep using the same wrapPanel as before
				Grid gridForField=CreateGridForField();
				ListEFormFields[i].TagOD=gridForField;
				wrapPanel.Children.Add(gridForField);
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
					AddCheckBox(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.DateField){
					AddDateField(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.Label){
					AddLabel(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.MedicationList){
					AddMedicationList(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak){
					AddPageBreak(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.RadioButtons){
					AddRadioButtons(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.SigBox){
					AddSigBox(gridForField,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.TextField){
					AddTextBox(gridForField,ListEFormFields[i]);
				}
				//Always add a margin to the right of each field
				//Yes, this even works for the last row because we want the item moved to right of the last row.
//old:We will end up with a margin to the right of each page break, but we won't show those
				Border borderRightOfField=CreateBorderForDrop();
				borderRightOfField.Width=_margins;
				borderRightOfField.HorizontalAlignment=HorizontalAlignment.Right;
				DragLocation dragLocationRightOfField=new DragLocation();
				dragLocationRightOfField.Idx=i+1;
				//if(EFormFieldDefs.IsHorizStackable(ListEFormFields[i].FieldType)){
				dragLocationRightOfField.IsHorizStacking=true;
				//}
				if(i<ListEFormFields.Count-1){//not last row
					if(ListEFormFields[i+1].IsHorizStacking){//if the next field is horizontal stacking
						//This drag location is the primary for the next field
						dragLocationRightOfField.IsPrimary=true;
						dragLocationRightOfField.IsHorizStackingNext=true;
					}
					else{//this is the last field in the horiz stack, whether it's alone or in a group
						dragLocationRightOfField.IsRightOfWrap=true;
					}
				}
				dragLocationRightOfField.Page=_pageCount;
				borderRightOfField.Tag=dragLocationRightOfField;
				gridForField.Children.Add(borderRightOfField);
				Grid.SetColumn(borderRightOfField,1);
				if(i==ListEFormFields.Count-1//last row
					|| !ListEFormFields[i+1].IsHorizStacking)//the next field is not horizontal stacking
				{
					stackPanel.Children.Add(gridWrap);
					if(ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak){
						//We don't add a border below page break.
						//We could alternately have hidden it.
					}
					else{
						//Even though it feels like we're adding it below a wrap panel,
						//it's really above its corresponding wrap panel.
						//But no point in refactoring to make that more obvious.
						Border borderTopOfWrap=CreateBorderForDrop();
						borderTopOfWrap.Height=_margins;
						borderTopOfWrap.VerticalAlignment=VerticalAlignment.Bottom;
						borderTopOfWrap.Margin=new Thickness(left:_margins,0,right:_margins,0);
						DragLocation dragLocationTopOfWrap=new DragLocation();
						dragLocationTopOfWrap.Idx=i+1;
						//This is the primary for the field below only if that field is not the first of a horiz stack group
						//because in that case, the primary would be to its left.
						if(i<ListEFormFields.Count-2//if there are at least 2 more fields
							&& !ListEFormFields[i+2].IsHorizStacking)//and the next field is not the first of a horiz stack group
						{
							dragLocationTopOfWrap.IsPrimary=true;
						}
						dragLocationTopOfWrap.IsAbove=true;
						dragLocationTopOfWrap.IsHorizStacking=false;
						dragLocationTopOfWrap.Page=_pageCount;
						borderTopOfWrap.Tag=dragLocationTopOfWrap;
						stackPanel.Children.Add(borderTopOfWrap);
					}
				}
				//otherwise, don't add the wrapPanel yet because we need to add more fields.
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak){
					_pageCount++;
				}
			}
			scrollViewer.ScrollToVerticalOffset(verticalOffset);
			SetVisibilities(_pageShowing,forceRefresh:true);
			if(idxSelected!=-1){
				SetSelected(idxSelected);
			}
		}

		///<summary></summary>
		public void SetSelected(int index,bool setValue=true) {
			if(setValue) {//select specified index
				if(index<0) {//check to see if index is within the valid range of values
					return;//if not, then ignore.
				}
				if(!_listSelectedIndices.Contains(index)) {
					_listSelectedIndices.Add(index);
				}
			}
			else {//unselect specified index
				if(_listSelectedIndices.Contains(index)) {
					_listSelectedIndices.Remove(index);
				}
			}
			SetColors();
		}

		///<summary>Sets visibility of each field for pages and conditional logic.</summary>
		public void SetVisibilities(int pgRequested, bool forceRefresh=false){
			int pgGranted=pgRequested;
			if(pgRequested<1){
				pgGranted=1;
			}
			if(pgRequested>_pageCount){
				pgGranted=_pageCount;
			}
			if(!forceRefresh && _pageShowing==pgGranted){
				return;
			}
			_pageShowing=pgGranted;
			if(IsSetupMode) {//Show all pages in Setup Mode.
				label.Text=_pageShowing.ToString()+"/"+_pageCount.ToString();
			}
			else {//Only show visible pages when patient is filling out the eForm.
				_pageCountFiltered=_pageCount;
				SetIsHiddenFlag();
				for(int i=1;i<=_pageCount;i++) {
					List<EFormField> listEFormFields=ListEFormFields.FindAll(x=>x.Page==i && x.FieldType!=EnumEFormFieldType.PageBreak);
					if(listEFormFields.IsNullOrEmpty()) {
						continue;
					}
					if(listEFormFields.All(x=>x.IsHiddenCondit)) {
						_pageCountFiltered--;
					}
				}
				label.Text=_pageShowingFiltered.ToString()+"/"+_pageCountFiltered.ToString();
			}
			for(int i=0;i<ListEFormFields.Count;i++){
				//first, pages
				Grid gridForField=(Grid)ListEFormFields[i].TagOD;
				if(ListEFormFields[i].Page==_pageShowing){
					gridForField.Visibility=Visibility.Visible;
				}
				else{
					gridForField.Visibility=Visibility.Collapsed;
					continue;//no need to check conditional logic. Always collapsed.
				}
				//Then conditional logic, just for this page
				if(IsSetupMode){
					continue;//we don't hide for conditional logic in setup
				}
				if(ListEFormFields[i].ConditionalParent==""){
					continue;
				}
				EFormField eFormFieldParent=ListEFormFields.Find(x=>x.ValueLabel==ListEFormFields[i].ConditionalParent);
				if(eFormFieldParent is null){
					continue;//they might have set the ConditionalParent string wrong
				}
				bool isConditionMet=false;
				if(eFormFieldParent.FieldType==EnumEFormFieldType.RadioButtons 
					&& EFormFields.ConvertValueStringDbToVis(eFormFieldParent)==ListEFormFields[i].ConditionalValue)
				{
					isConditionMet=true;
				}
				if(eFormFieldParent.FieldType==EnumEFormFieldType.CheckBox 
					&& eFormFieldParent.ValueString==ListEFormFields[i].ConditionalValue)
				{
					isConditionMet=true;
				}
				if(isConditionMet){
					gridForField.Visibility=Visibility.Visible;
				}
				else{
					gridForField.Visibility=Visibility.Collapsed;
				}
			}
			//This next loop is only for the drag location borders, which do not exist in the normal UI
			UIElementCollection uIElementCollection = stackPanel.Children;
			for(int i = 0;i<uIElementCollection.Count;i++) {
				if(!(uIElementCollection[i] is Border border)) {
					continue;
				}
				DragLocation dragLocation=border.Tag as DragLocation;
				if(dragLocation.Page!=_pageShowing) {
					uIElementCollection[i].Visibility=Visibility.Collapsed;
					continue;
				}
				uIElementCollection[i].Visibility=Visibility.Visible;
				
			}
		}
		#endregion Methods - public

		#region Methods - private Event Handlers mouse
		private void Grid_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			//This happens after stackPanel.MouseLeftButtonDown
			_pointMouseDown=e.GetPosition(this);
			if(_mouseDownIndex>-1){
				return;
			}
			bool isControlDown=Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			if(isControlDown) {
				//as we drag the selector, we will add to the existing selection
			}
			else{
				_listSelectedIndices.Clear();
				SetColors();
			}
			_listSelectedIndicesWhenSelectionStart.Clear();
			_listSelectedIndicesWhenSelectionStart.AddRange(_listSelectedIndices);
			_isDraggingSelector=true;//immediately show the dragging rectangle
			borderSelect.Visibility=Visibility.Visible;
			borderSelect.Width=0;
			borderSelect.Height=0;
			borderSelect.Margin=new Thickness(_pointMouseDown.X,_pointMouseDown.Y,0,0);
		}

		private void Grid_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			_mouseDownIndex=-1;
			if(!_isDraggingSelector){
				return;
			}
			_isDraggingSelector=false;
			borderSelect.Visibility=Visibility.Collapsed;
		}

		private void Grid_MouseMove(object sender,MouseEventArgs e) {
			if(!_isDraggingSelector){
				return;
			}
			Point pointMouse=e.GetPosition(this);
			double x=_pointMouseDown.X;
			double y=_pointMouseDown.Y;
			double w=pointMouse.X-_pointMouseDown.X;
			double h=pointMouse.Y-_pointMouseDown.Y;
			//untangle. -w or -h are not allowed
			if(w<0){
				x+=w;
				w=-w;
			}
			if(h<0){
				y+=h;
				h=-h;
			}
			borderSelect.Margin=new Thickness(x,y,0,0);
			borderSelect.Width=w;
			borderSelect.Height=h;
			_listSelectedIndices.Clear();
			_listSelectedIndices.AddRange(_listSelectedIndicesWhenSelectionStart);
			Rect rectSelector=new Rect(x,y,w,h);
			for(int i=0;i<ListEFormFields.Count;i++){
				if(ListEFormFields[i].Page!=_pageShowing){
					continue;
				}
				Grid gridForField=(Grid)ListEFormFields[i].TagOD;
				//test all 4 corners
				//Points are relative to entire control, not StackPanel
				//Our gridForFields include the right margin that isn't really part of the field.
				Point pointUL=PointFromScreen(gridForField.PointToScreen(new Point(0,0)));
				//Point pointUR=PointFromScreen(gridForField.PointToScreen(new Point(gridForField.ActualWidth,0)));
				//Point pointBL=PointFromScreen(gridForField.PointToScreen(new Point(0,gridForField.ActualHeight)));
				Point pointBR=PointFromScreen(gridForField.PointToScreen(new Point(gridForField.ActualWidth,gridForField.ActualHeight)));
				//this rectangle won't be tangled
				Rect rectField=new Rect(pointUL.X,pointUL.Y,pointBR.X-pointUL.X-_margins,pointBR.Y-pointUL.Y);
				if(rectSelector.IntersectsWith(rectField)){
					if(_listSelectedIndicesWhenSelectionStart.Contains(i)){
						_listSelectedIndices.Remove(i);
					}
					else{
						_listSelectedIndices.Add(i);
					}
				}
			}
			SetColors();
		}

		private void StackPanel_MouseLeave(object sender,MouseEventArgs e) {
			//When mouse is down and captured, this does not fire until mouse up
			_hoverIndex=-1;
			SetColors();
		}

		private void StackPanel_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			_pointMouseDown=e.GetPosition(this);
			_ignoreMouseMove=true;
			((IInputElement)sender).CaptureMouse();
			_ignoreMouseMove=false;
			_mouseDownIndex=-1;
			for(int i=0;i<ListEFormFields.Count;i++) {
				Grid gridForField=ListEFormFields[i].TagOD as Grid;
				int pageNum=ListEFormFields[i].Page;
				if(pageNum!=_pageShowing) {
					continue;
				}
				Point point=e.GetPosition(gridForField);
				Rect rectBounds=new Rect(0,0,gridForField.ActualWidth-_margins,gridForField.ActualHeight);
				if(rectBounds.Contains(point)){
					_mouseDownIndex=i;
					break;
				}
			}
			if(e.ClickCount==2) {//double click
				if(_mouseDownIndex>=0){
					EventDoubleClickField?.Invoke(this,_mouseDownIndex);
				}
				return;
			}
			_isMouseDown=true;
			if(_mouseDownIndex==-1){
				//User clicked the empty space in between the fields
				return;
				//Nothing else to do.
				//All the code below assumes _mouseDownIndex > -1
			}
			//Check if the page break delete button was clicked. If it was, remove the field from the list and refresh the layout.
			if(ListEFormFields[_mouseDownIndex].FieldType==EnumEFormFieldType.PageBreak) {
				//can't select a page break, but you can click on the delete button
				Grid gridForField=ListEFormFields[_mouseDownIndex].TagOD as Grid;
				WpfControls.UI.Button button=gridForField.Children[3] as WpfControls.UI.Button;
				if(button.Visible) {
					Point point=e.GetPosition(button);
					Rect rectBounds=new Rect(0,0,button.ActualWidth,button.ActualHeight);
					if(!rectBounds.Contains(point)) {
						return;
					}
					ListEFormFields.RemoveAt(_mouseDownIndex);
					RefreshLayout();
				}
				return;
			}
			bool isControlDown=Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			//special edge case:
			if(_listSelectedIndices.Count>1//multiple items are already selected
				&& !isControlDown//user has released Ctrl
				&& _listSelectedIndices.Contains(_mouseDownIndex))//and mouse down on an already selected item
			{
				//Then they might be wanting to start dragging the group.
				//Don't deselect the others until they mouse up, having not dragged
			}
			else if(isControlDown) {
				if(_listSelectedIndices.Contains(_mouseDownIndex)) {
					_listSelectedIndices.Remove(_mouseDownIndex);
				}
				else {
					_listSelectedIndices.Add(_mouseDownIndex);
				}
			}
			else{//Ctrl not down
				_listSelectedIndices=new List<int>() { _mouseDownIndex };
			}
			SetColors();
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;//introducing variable for debugging because this state is not preserved at break points.
			if(!isMouseDown) {
				_isMouseDown=false;
			}
		}

		private void StackPanel_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			((IInputElement)sender).ReleaseMouseCapture();
			_isMouseDown=false;
			_mouseDownIndex=-1;
			_isDraggingField=false;
			Cursor=Cursors.Arrow;
			_dispatcherTimer.Stop();
			_actionTimer=null;
			if(_borderDropHover==null || _listSelectedIndices.Count==0){
				SetColors();
				return;
			}
			if(_borderDropHover.ActualHeight==0 && _borderDropHover.ActualWidth==0) {
				SetColors();
				return;
			}
			//Dropping selected field(s) to the new dragLocation index.
			//NOTE: This will not update the ItemOrder of the field. That is only done on the save click.
			DragLocation dragLocation=_borderDropHover.Tag as DragLocation;
			_listSelectedIndices.Sort();
			if(_listSelectedIndices.Count==1){
				//When we are just moving one field, we work to make it accurate.
				//If we are moving multiple, we skip this and do a fix at the end.
				//The only scenario where dragging away might affect other fields is:
				//Left field in a group. Field to its right needs to be changed to not stack.
				if(!ListEFormFields[_listSelectedIndices[0]].IsHorizStacking//not
					&& _listSelectedIndices[0]<ListEFormFields.Count-1//not last item in list
					&& ListEFormFields[_listSelectedIndices[0]+1].IsHorizStacking)
				{
					ListEFormFields[_listSelectedIndices[0]+1].IsHorizStacking=false;
				}
				//If the stacking specified in dragLocation was not valid, UI would not have allowed a drag.
				//Use what was already precalculated
				ListEFormFields[_listSelectedIndices[0]].IsHorizStacking=dragLocation.IsHorizStacking;
				//StackingNext is done a little further down
			}
			List<EFormField> listEFormFields=_listSelectedIndices.Select(x=>ListEFormFields[x]).ToList();
			//As we remove fields from the list, we need to adjust the idx if they are before the insert point.
			int idxTo=dragLocation.Idx;
			for(int i=_listSelectedIndices.Count-1;i>=0;i--) {//beckward because removing
				ListEFormFields.RemoveAt(_listSelectedIndices[i]);
				if(idxTo>0 && idxTo>_listSelectedIndices[i]) {
					idxTo--;
				}
			}
			if(_listSelectedIndices.Count==1){
				//Again, only for single
				//We alter destination here after removal, because otherwise we risk changing the field we are moving.
				//Example: move idx 4 to 3, up from single to end of row:
				//   loop will have removed field 4 from list.
				//   this will alter the new 4 which is the old 5
				//Example2: move idx 3 to 5, from end of row to middle of next row.
				//   look will have removed field 3 from list.
				//   idxTo was 5, but is now 4
				//   this will alter the new 4, which is the old 5
				ListEFormFields[idxTo].IsHorizStacking=dragLocation.IsHorizStackingNext;
			}
			ListEFormFields.InsertRange(idxTo,listEFormFields);
			_borderDropHover=null;
			int countSelected=_listSelectedIndices.Count;
			RefreshLayout();
			for(int i=0;i<countSelected;i++){
				SetSelected(idxTo+i);
			}
		}

		private void StackPanel_MouseMove(object sender,MouseEventArgs e) {
			if(_ignoreMouseMove){
				return;
			}
			Point pointMouse=e.GetPosition(this);
			//mouse up could have easily happened on a popup, so we need to check again
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(_isDraggingField && !isMouseDown){
				_isDraggingField=false;
				_isMouseDown=false;
				//might need more
			}
			if(_isDraggingSelector){
				return;
			}
			if(_isDraggingField){
				#region Page Pan Scroll
				//Logic to change the page and scrolling when dragging a field.
				//Left (Previous page)
				if(pointMouse.X<0) {
					SetActionTimer(() => SetVisibilities(_pageShowing-1),interval:500);
				}
				//Right (Next page)
				else if(pointMouse.X>this.ActualWidth) {
					SetActionTimer(() => SetVisibilities(_pageShowing+1),interval:500);
				}
				//Up (Scroll up)
				else if(pointMouse.Y<0) {
					if(pointMouse.Y>=-10) {//Slowest interval. -10 to -1.
						SetActionTimer(() => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset-5),interval:5);
					}
					else if(pointMouse.Y>=-20) {//Medium interval. -20 to -11.
						SetActionTimer(() => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset-5),interval:3);
					}
					else {//Fastest interval. Anything less than -20.
						SetActionTimer(() => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset-5),interval:1);
					}
				}
				//Down (Scroll down)
				else if(pointMouse.Y>this.ActualHeight) {
					if(pointMouse.Y>=this.ActualHeight+10) {//Slowest interval. this.ActualHeight+10 to this.ActualHeight+1.
						SetActionTimer(() => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset+5),interval:5);
					}
					else if(pointMouse.Y>=this.ActualHeight+20) {//Medium interval. this.ActualHeight+20 to this.ActualHeight+11.
						SetActionTimer(() => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset+5),interval:3);
					}
					else {//Fastest interval. Anything greater than this.ActualHeight+20.
						SetActionTimer(() => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset+5),interval:1);
					}
				}
				else {//Mouse is inside of the control.
					_dispatcherTimer.Stop();
					_actionTimer=null;
				}
				#endregion Page Pan Scroll
				//If we have already previously triggered the start of a drag, then we quit doing hover effect on fields.
				//Instead, we start the hover effect on drag location borders.
				_borderDropHover=null;
				for(int i=0;i<_listBordersDrops.Count;i++){
					DragLocation dragLocation=_listBordersDrops[i].Tag as DragLocation;
					if(dragLocation.Page!=_pageShowing){
						continue;
					}
					Point point=e.GetPosition(_listBordersDrops[i]);
					Rect rectBounds=new Rect(0,0,_listBordersDrops[i].ActualWidth,_listBordersDrops[i].ActualHeight);
					if(!rectBounds.Contains(point)){
						continue;
					}
					#region Hover Drag Locations
					//But we don't skip when multiple are selected because moving a group is a bit different.
					//When we move a group, we don't suppress anything.
					//Instead, we just plop them all into that spot, and then loop to fix any stacking issues.
					if(_listSelectedIndices.Count==1){
						//The first two big if sections below are to skip any of these drag locations that would not result in an actual move.
						//This happens when trying to move one drag location forward or back in the order from current location.
						//Many of those would not actually result in a move.
						//The third if section is to prevent invalid stack orders at the destination, regardless of how far we are moving.
						if(dragLocation.Idx==_listSelectedIndices[0]){
							//We have a matching index.
							//There are 9 possible Scenarios:
							//1. Horiz stacked field, moving left: suppress. This is primary.
							if(ListEFormFields[_listSelectedIndices[0]].IsHorizStacking){
								continue;
							}
							else if(_listSelectedIndices[0]==ListEFormFields.Count-1//this is the last field
								|| !ListEFormFields[_listSelectedIndices[0]+1].IsHorizStacking)//or there is not a stacked field to its right
							{//must be a sole field
							//2. Sole field, moving left: suppress
								if(dragLocation.IsLeftOfWrap){
									continue;
								}
							//3. Sole field, moving up: suppress. This is primary.
								else if(dragLocation.IsAbove){
									continue;
								}
								else{
							//4. Sole field, moving to right of group above: show
							//5. Sole field, moving to right of sole field above: show
								}
							}
							else{//must be the left field in a group
							//6. Left field in a group, moving left: suppress, since this is primary.
								if(dragLocation.IsLeftOfWrap){
									continue;
								}
								else{
							//7. Left field in a group, moving up: show
							//8. Left field in a group, moving to right of group above: show
							//9. Left field in a group, moving to right of sole field above: show
								}
							}
						}
						if(dragLocation.Idx==_listSelectedIndices[0]+1){
							//If we are moving to the next index, like to the right or down.
							//There are 9 scenarios:
							//1. Field with another horiz stacked field to its right: suppress
							if(_listSelectedIndices[0]<ListEFormFields.Count-1//this is not the last field
								&& ListEFormFields[_listSelectedIndices[0]+1].IsHorizStacking)//and there is a stacked field to its right
							{
								continue;
							}
							else if(ListEFormFields[_listSelectedIndices[0]].IsHorizStacking){//must be right field in a group
							//2. Right field in a group, moving right: suppress
								if(dragLocation.IsRightOfWrap){
									continue;
								}
								else{
							//3. Right field in a group, moving down: show
							//4. Right field in a group, moving to the left of a group below: show
							//5. Right field in a group, moving to the left of a sole field below: show
								}
							}
							else{//sole field
							//6. Sole field, moving right: suppress
								if(dragLocation.IsRightOfWrap){
									continue;
								}
							//7. Sole field, moving down: suppress
								else if(dragLocation.IsAbove){
									continue;
								}
								else{
							//8. Sole field, moving to the left of a group below: show
							//9. Sole field, moving to the left of a sole field below: show
								}
							}
						}
						//Block improper stacking at destination
						bool isAttemptingStack=false;
						if(dragLocation.IsHorizStacking){
							isAttemptingStack=true;
						}
						if(dragLocation.IsHorizStackingNext){
							isAttemptingStack=true;
						}
						if(isAttemptingStack){
							if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[_listSelectedIndices[0]].FieldType)){
								//The source field that we are trying to move does not allow stacking
								continue;
							}
							if(dragLocation.IsHorizStacking){
								//check that the field to the left allows stacking
								if(dragLocation.Idx!=0){//the destination is not 0 (probably already handled when setting IsHorizStacking
									if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[dragLocation.Idx-1].FieldType)){
										continue;//not allowed
									}
								}
							}
							if(dragLocation.IsHorizStackingNext){
								//check that the field to the right (idx) allows stacking
								if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[dragLocation.Idx].FieldType)){
									continue;//not allowed
								}
							}
						}
					}//if(_listSelectedIndices.Count==1){
					#endregion Hover Drag Locations
					//if we got this far, then no exclusion
					_borderDropHover=_listBordersDrops[i];
					break;
				}
			}
			#region Hover Fields
			_hoverIndex=-1;
			for(int i=0;i<ListEFormFields.Count;i++){
				Grid gridForField=ListEFormFields[i].TagOD as Grid;
				int pageNum=ListEFormFields[i].Page;
				if(pageNum!=_pageShowing) {
					continue;
				}
				Border borderHover=gridForField.Children[0] as Border; 
				Point point=e.GetPosition(borderHover);
				Rect rectBounds=new Rect(0,0,borderHover.ActualWidth,borderHover.ActualHeight);
				if(!rectBounds.Contains(point)){
					continue;
				}
				if(_isDraggingField){
					if(_listSelectedIndices.Count==1 && _listSelectedIndices[0]==i){
						//We ignore self. But if multiple are selected, then this restriction doesn't make sense.
						continue;
					}
					//Mouse is over a field instead of a drag location.
					//We want to highlight one related borderDrop, which was already calculated as the primary.
					//This logic might not always result in a highlighted drag location.
					//Exclusions include self, invalid stacking, and special treatment for the field to the right.
					//We don't need to validate where we are dragging from. Any from location is valid.
					//When we mouse up, one edge case is to possibly remove stacking from field to right of source.
					for(int b=0;b<_listBordersDrops.Count;b++){
						DragLocation dragLocation=_listBordersDrops[b].Tag as DragLocation;
						if(dragLocation.Idx!=i){
							continue;
						}
						if(!dragLocation.IsPrimary){
							continue;
						}
						//we have located our correct dragLocation, but it might not be allowed due to stacking
						bool isAttemptingStack=false;
						if(dragLocation.IsHorizStacking){
							isAttemptingStack=true;
						}
						if(dragLocation.IsHorizStackingNext){
							isAttemptingStack=true;
						}
						if(isAttemptingStack){
							if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[_listSelectedIndices[0]].FieldType)){
								//The source field that we are trying to move does not allow stacking
								break;
							}
							if(dragLocation.IsHorizStacking){
								//check that the field to the left allows stacking
								if(dragLocation.Idx!=0){//the destination is not 0 (probably already handled when setting IsHorizStacking
									if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[_listSelectedIndices[0]-1].FieldType)){
										break;//not allowed
									}
								}
							}
							if(dragLocation.IsHorizStackingNext){
								//check that the field to the right (idx) allows stacking
								if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[dragLocation.Idx].FieldType)){
									break;//not allowed
								}
							}
						}
						//If we are hovering over the field to the right of source, then we also have to
						//suppress the same locations that we suppressed earlier when hovering over dragdrop borders.
						//The field we are evaluating each time is the selected field, not the hover field.
						//And we are considering the scenario where that field is moved into the primary dragLocation of the hover field.
						//What that means is that this is similar to moving to selectedIdx+1 rather than to the left.
						if(dragLocation.Idx==_listSelectedIndices[0]+1){
							//There were 9 scenarios, 4 of which were suppressed:
							//Scenarios 1, 2, 6, and 7
							//1. Field with another horiz stacked field to its right: 
							//2. Right field in a group, moving right: that one can be ignored because that dragLocation is not a primary
							//6. Sole field, moving right: also not a primary
							//7. Sole field, moving down:
							//That means we only need to consider 1 and 7
							//So the chunk below is an abbreviated version without as many comments.
							//1. Field with another horiz stacked field to its right: suppress
							if(_listSelectedIndices[0]<ListEFormFields.Count-1//this is not the last field
								&& ListEFormFields[_listSelectedIndices[0]+1].IsHorizStacking)//and there is a stacked field to its right
							{
								break;
							}
							else if(ListEFormFields[_listSelectedIndices[0]].IsHorizStacking){//must be right field in a group
							}
							else{//sole field
							//6. Sole field, moving right: suppress
								if(dragLocation.IsRightOfWrap){
									break;
								}
							//7. Sole field, moving down: suppress
								else if(dragLocation.IsAbove){
									break;
								}
							}
						}
						_borderDropHover=_listBordersDrops[b];
						break;
					}
					//_hoverIndex will remain -1
				}
				else{
					//Not dragging yet. Mouse could still be down.
					//We are interested in hover
					_hoverIndex=i;
					break;
				}
			}
			#endregion Hover Fields
			SetColors();
			if(!isMouseDown || !_isMouseDown) {
				return;
			}
			if(_listSelectedIndices.Count==0){
				//this prevents scenario where you click on a space between fields and start dragging. That's not a drag.
				return;
			}
			//from here down is dragging
			//If we are already dragging, then we don't need to do the test again.
			if(_isDraggingField){
				//still dragging
				return;
			}
			if(Math.Abs(pointMouse.X-_pointMouseDown.X)<3
				&& Math.Abs(pointMouse.Y-_pointMouseDown.Y)<3)
			{
				//They dragged slightly, but not enough to count as a true drag.
				return;
			}
			_isDraggingField=true;
			Cursor=_cursorDrag;
		}
		#endregion Methods - private Event Handlers mouse

		#region Methods - private Event Handlers
		private void butNext_Click(object sender,EventArgs e) {
			int pgRequested=_pageShowing+1;
			if(!IsSetupMode) {
				if(_pageShowingFiltered<_pageCountFiltered) {
					//This is used to show which page number the patient is on. This is not always the same value as _pageShowing. This is just for visuals, nothing more. 
					_pageShowingFiltered++;
				}
				SetIsHiddenFlag();
				while(true) {
					List<EFormField> listEFormFields=ListEFormFields.FindAll(x=>x.Page==pgRequested && x.FieldType!=EnumEFormFieldType.PageBreak).ToList();
					if(listEFormFields!=null && listEFormFields.All(x=>x.IsHiddenCondit)) {
						if(pgRequested==_pageCount) {//If the last page has all of its fields hidden because of condition, don't go to it.
							return;
						}
						pgRequested++;
					}
					else {//We found a page that has visible fields on it.
						break;
					}
					if(pgRequested>_pageCount) {//It has to be > because we need to check the last page for fields also.
						break;
					}
				}
			}
			SetVisibilities(pgRequested);
		}

		private void butPrevious_Click(object sender,EventArgs e) {
			int pgRequested=_pageShowing-1;
			if(!IsSetupMode) {
				if(_pageShowingFiltered>1) {
					//This is used to show which page number the patient is on. This is not always the same value as _pageShowing. This is just for visuals, nothing more. 
					_pageShowingFiltered--;
				}
				SetIsHiddenFlag();
				while(true) {
					List<EFormField> listEFormFields=ListEFormFields.FindAll(x=>x.Page==pgRequested && x.FieldType!=EnumEFormFieldType.PageBreak).ToList();
					if(listEFormFields!=null && listEFormFields.All(x=>x.IsHiddenCondit)) {
						if(pgRequested==1) {//If the first page has all of its fields hidden because of condition, don't go to it. This could happen, but very rare.
							return;
						}
						pgRequested--;
					}
					else {//We found a page that has visible fields on it.
						break;
					}
					if(pgRequested<1) {//It has to be < because we need to check the first page for fields. Really, the first page should always have visible fields.
						break;
					}
				}
			}
			SetVisibilities(pgRequested);
		}

		/*
		private void butTestConditional_Click(object sender,EventArgs e) {
			WpfControls.UI.Button button=sender as WpfControls.UI.Button;
			//if(button.Visible) {
			//Point point=e.GetPosition(button);
			//Rect rectBounds=new Rect(0,0,button.ActualWidth,button.ActualHeight);
			//if(rectBounds.Contains(point)) {
			//ListEFormFields[_mouseDownIndex].PickListVis.Split
			Grid gridForField=button.Parent as Grid;
			StackPanel stackPanelRadio=	gridForField.Children[1] as StackPanel;
			WrapPanel wrapPanelRadio=stackPanelRadio.Children[1] as WrapPanel;
			UIElementCollection uIElementCollection=wrapPanelRadio.Children;//the actual radiobuttons
			//find index of the one that's checked.
			int idxChecked=-1;
			for(int r=0;r<uIElementCollection.Count;r++){
				WpfControls.UI.RadioButton radioButton=uIElementCollection[r] as WpfControls.UI.RadioButton;
				if(radioButton.Checked){
					idxChecked=r;
				}
			}
			if(idxChecked==uIElementCollection.Count-1){//last radiobutton
				idxChecked=-1;//so it will loop to 0
			}
			WpfControls.UI.RadioButton radioButtonToCheck=uIElementCollection[idxChecked+1] as WpfControls.UI.RadioButton;
			radioButtonToCheck.Checked=true;
			EFormField eFormField=ListEFormFields.Find(x=>x.TagOD==gridForField);
			//guaranteed to not be null
			FillFieldsFromControls(eFormField);
			SetVisibilities(_pageShowing,forceRefresh:true);
			return;
			//}
					//}
		}*/

		private void CtrlEFormFill_Loaded(object sender,RoutedEventArgs e) {
			if(!IsSetupMode){
				return;
			}
			Keyboard.Focus(stackPanel);
			stackPanel.MouseLeave+=StackPanel_MouseLeave;
			stackPanel.MouseLeftButtonDown+=StackPanel_MouseLeftButtonDown;
			stackPanel.MouseLeftButtonUp+=StackPanel_MouseLeftButtonUp;
			stackPanel.MouseMove+=StackPanel_MouseMove;
			stackPanel.KeyDown+=StackPanel_KeyDown;
			grid.MouseLeftButtonDown+=Grid_MouseLeftButtonDown;
			grid.MouseLeftButtonUp+=Grid_MouseLeftButtonUp;
			grid.MouseMove+=Grid_MouseMove;
		}

		///<summary>This method is called when the timer interval has elapsed.</summary>
		private void Timer_Tick(object sender, EventArgs e) {
			_actionTimer?.Invoke();
		}
		#endregion Methods - private Event Handlers

		#region Methods - Event Handlers - Med List
		private void ButtonAdd_Click(object sender,EventArgs e) {
			WpfControls.UI.Button button=sender as WpfControls.UI.Button;
			StackPanel stackPanelFooter=button.Parent as StackPanel;
			StackPanel stackPanelVert=stackPanelFooter.Parent as StackPanel;
			Grid gridMeds=stackPanelVert.Children[1] as Grid;
			double heightRow=gridMeds.RowDefinitions[0].Height.Value;
			RowDefinition rowDefinition=new RowDefinition();
			rowDefinition.Height=new GridLength(heightRow);
			gridMeds.RowDefinitions.Add(rowDefinition);
			int idx=gridMeds.RowDefinitions.Count-1;
			double fontSize=button.FontSize;
			WpfControls.UI.TextBox textBoxMed=new WpfControls.UI.TextBox();
			gridMeds.Children.Add(textBoxMed);
			textBoxMed.Width=gridMeds.ColumnDefinitions[0].Width.Value;
			textBoxMed.FontSize=fontSize;
			Grid.SetRow(textBoxMed,idx);
			WpfControls.UI.TextBox textBoxFreq=new WpfControls.UI.TextBox();
			gridMeds.Children.Add(textBoxFreq);
			textBoxFreq.Width=gridMeds.ColumnDefinitions[1].Width.Value;
			textBoxFreq.FontSize=fontSize;
			Grid.SetRow(textBoxFreq,idx);
			Grid.SetColumn(textBoxFreq,1);
			WpfControls.UI.Button buttonDelete=new WpfControls.UI.Button();
			gridMeds.Children.Add(buttonDelete);
			buttonDelete.Text="Delete";
			buttonDelete.Width=gridMeds.ColumnDefinitions[2].Width.Value;
			buttonDelete.Height=heightRow;
			buttonDelete.FontSize=fontSize;
			buttonDelete.Click+=ButtonDelete_Click;
			Grid.SetRow(buttonDelete,idx);
			Grid.SetColumn(buttonDelete,2);
		}

		private void ButtonDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			WpfControls.UI.Button button=sender as WpfControls.UI.Button;
			int row=Grid.GetRow(button);
			Grid gridMeds=button.Parent as Grid;
			for(int c=gridMeds.Children.Count-1;c>=0;c--){//go backward so that removals work properly
				if(gridMeds.Children[c] is WpfControls.UI.TextBox textBox){
					if(Grid.GetRow(textBox)<row){
						continue;
					}
					if(Grid.GetRow(textBox)==row){
						gridMeds.Children.Remove(textBox);
						continue;
					}
					//item is in a row below the Delete button
					Grid.SetRow(textBox,Grid.GetRow(textBox)-1);
				}
				if(gridMeds.Children[c] is WpfControls.UI.Button buttonBelow){
					if(Grid.GetRow(buttonBelow)<row){
						continue;
					}
					if(Grid.GetRow(buttonBelow)==row){
						gridMeds.Children.Remove(buttonBelow);//this button, actually
						continue;
					}
					//item is in a row below Delete button
					Grid.SetRow(buttonBelow,Grid.GetRow(buttonBelow)-1);
				}
			}
			gridMeds.RowDefinitions.RemoveAt(gridMeds.RowDefinitions.Count-1);
		}
		#endregion Methods - Event Handlers - Med List

		#region Method - Event Handlers - Key
		private void StackPanel_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Delete || e.Key==Key.Back) {
				if(_listSelectedIndices.Count==0) {
					return;
				}
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Delete selected field(s)?")) {
					return;
				}
				_listSelectedIndices.Sort();
				for (int i=_listSelectedIndices.Count-1;i>=0;i--) {
					ListEFormFields.RemoveAt(_listSelectedIndices[i]);
				}
				RefreshLayout();//We only want to refresh the layout when ListEFormFields actually changes.
			}
			//When the user clicks Ctrl+A, it will select all of the fields on the page that is currently being viewed.
			if(Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key==Key.A) {
				for(int i=0;i<ListEFormFields.Count;i++) {
					if(ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak) {//We don't want to select pagebreak fields.
						continue;
					}
					if(ListEFormFields[i].Page==_pageShowing) {
						SetSelected(i);
					}
				}
			}
		}
		#endregion Method - Event Handlers - Key

		#region Methods - private
		private void AddCheckBox(Grid gridForField,EFormField eFormField){
			CheckBox checkBox=new CheckBox();
			gridForField.Children.Add(checkBox);
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			checkBox.VerticalContentAlignment=VerticalAlignment.Center;
			checkBox.FontSize=FontSize*eFormField.FontScale/100;
			checkBox.IsChecked=eFormField.ValueString=="X";
			Label label=new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0);//default is 5
			label.Content=eFormField.ValueLabel;
			stackPanelLabel.Children.Add(label);
			if(eFormField.IsRequired) {
				Label labelRequired=new Label();
				labelRequired.FontSize=FontSize*eFormField.FontScale/100;
				labelRequired.Padding=new Thickness(0);
				labelRequired.Content=" *";
				labelRequired.Foreground=Brushes.Red;
				labelRequired.FontWeight=FontWeights.Bold;
				stackPanelLabel.Children.Add(labelRequired);
			}
			bool isConditionalParent=false;
			if(IsSetupMode
				&& ListEFormFields.Exists(x=>x.ConditionalParent==eFormField.ValueLabel))
			{
				isConditionalParent=true;
			}
			if(isConditionalParent) {
				Label labelCondParent = new Label();
				stackPanelLabel.Children.Add(labelCondParent);
				labelCondParent.FontSize=FontSize*eFormField.FontScale/100;
				labelCondParent.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondParent.Content="(CND)";
				labelCondParent.Foreground=Brushes.Red;
			}
			bool isConditionalChild = false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x => x.ValueLabel==eFormField.ConditionalParent)) {
				isConditionalChild=true;
			}
			if(isConditionalChild) {
				//yes, we let them include both parent and child in case a field is both.
				Label labelCondChild = new Label();
				stackPanelLabel.Children.Add(labelCondChild);
				labelCondChild.FontSize=FontSize*eFormField.FontScale/100;
				labelCondChild.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondChild.Content="(cnd)";
				labelCondChild.Foreground=Brushes.Red;
			}
			checkBox.Content=stackPanelLabel;
			checkBox.Click+=(sender,e)=> {
				FillFieldsFromControls(eFormField);
				SetVisibilities(_pageShowing,forceRefresh:true);
			};
		}

		private void AddDateField(Grid gridForField,EFormField eFormField){
			StackPanel stackPanel2=new StackPanel();
			gridForField.Children.Add(stackPanel2);
			double widthAvail=stackPanel.ActualWidth-_margins*2;
			if(widthAvail<0) {
				widthAvail=0;
			}
			if(eFormField.Width==0){//not specified
				stackPanel2.Width=widthAvail;//so full width
			}
			else if(eFormField.Width<widthAvail){
				stackPanel2.Width=eFormField.Width;
			}
			else{
				stackPanel2.Width=widthAvail;
			}
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			Label label = new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0);//default is 5
			label.Content=eFormField.ValueLabel;
			stackPanelLabel.Children.Add(label);
			if(eFormField.IsRequired) {
				Label labelRequired=new Label();
				labelRequired.FontSize=FontSize*eFormField.FontScale/100;
				labelRequired.Padding=new Thickness(0);
				labelRequired.Content=" *";
				labelRequired.Foreground=Brushes.Red;
				labelRequired.FontWeight=FontWeights.Bold;
				stackPanelLabel.Children.Add(labelRequired);
			}
			bool isConditionalChild=false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x=>x.ValueLabel==eFormField.ConditionalParent))
			{
				isConditionalChild=true;
			}
			if(isConditionalChild){
				Label labelCondChild=new Label();
				stackPanelLabel.Children.Add(labelCondChild);
				labelCondChild.FontSize=FontSize*eFormField.FontScale/100;
				labelCondChild.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondChild.Content="(cnd)";
				labelCondChild.Foreground=Brushes.Red;
			}
			stackPanel2.Children.Add(stackPanelLabel);
			WpfControls.UI.TextVDate textVDate=new WpfControls.UI.TextVDate();
			textVDate.HorizontalAlignment=HorizontalAlignment.Stretch;
			textVDate.Height=18;
			textVDate.Text=eFormField.ValueString;
			textVDate.FontSize=FontSize*eFormField.FontScale/100;
			stackPanel2.Children.Add(textVDate);
		}

		private void AddLabel(Grid gridForField,EFormField eFormField){
			WpfControls.UI.TextRich textRich=new WpfControls.UI.TextRich();
			textRich.SpellCheckIsEnabled=false;
			//One reason it's a textbox so that office/patient can edit during fill. This is a feature of sheets that we don't want to lose.
			//And this implementation works better than sheets because it doesn't jump when clicking into it.
			//The other reason it's a textbox is because that's how wpf shows flow documents.
			textRich.richTextBox.BorderThickness=new Thickness(0);
			gridForField.Children.Add(textRich);
			double widthAvail=stackPanel.ActualWidth-_margins*2;
			if(widthAvail<0) {
				widthAvail=0;
			}
			if(eFormField.Width==0){//not specified
				textRich.Width=widthAvail;//so full width
			}
			else if(eFormField.Width<widthAvail){
				textRich.Width=eFormField.Width;
			}
			else{
				textRich.Width=widthAvail;
			}
			//textBlock.FontSize=FontSize*eFormField.FontScale/100;//no, this is never set for labels
			//textBlock.Padding=new Thickness(0);//default is 5
			FlowDocument flowDocument=EFormFields.DeserializeFlowDocument(eFormField.ValueLabel);
			bool isConditionalChild=false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x=>x.ValueLabel==eFormField.ConditionalParent))
			{
				isConditionalChild=true;
			}
			if(isConditionalChild){
				if(flowDocument.Blocks.Count==0) {
					//Return early if there's no blocks
					textRich.richTextBox.Document=flowDocument;
					return;
				}
				if(!(flowDocument.Blocks.Any(x=>x.GetType() == typeof(Paragraph)))) {
					//Return early if there's no paragraph elements
					textRich.richTextBox.Document=flowDocument;
					return;
				}
				//Adds <Run Foreground="#FFFF0000"> (cnd)</Run> to the end of the last <Paragraph> element in the FlowDocument.
				Run runCondChild=new Run(" (cnd)");
				runCondChild.Foreground=new SolidColorBrush(Colors.Red);
				Paragraph paragraph=flowDocument.Blocks.OfType<Paragraph>().Last();
				paragraph.Inlines.Add(runCondChild);
			}
			textRich.richTextBox.Document=flowDocument;
		}

		private void AddMedicationList(Grid gridForField,EFormField eFormField) {
			//This could be done many different ways in different languages. We chose the following:
			//This is a vertical stackpanel of 3 items: title, a grid, and a footer with Add button and None checkbox
			//The grid has a header row as well as a row for each med.
			//To add and delete rows, we don't want to refresh layout because that would clear all the entered data.
			//So instead, we will dynamically alter the children and row defs of the grid.
			//For Add, grid gets another row at the end.
			//For Delete, the row objects get deleted, then following rows get moved up, and finally, grid loses row at end.
			StackPanel stackPanelVert=new StackPanel();
			gridForField.Children.Add(stackPanelVert);
			double widthAvail=stackPanel.ActualWidth-_margins*2;
			if(widthAvail<0) {
				widthAvail=0;
			}
			//stackPanelVert is always full widthAvail. But the grid columns resize.
			stackPanelVert.Width=widthAvail;
			EFormMedListLayout eFormMedListLayout=JsonConvert.DeserializeObject<EFormMedListLayout>(eFormField.ValueLabel);
			List<EFormMed> listEFormMeds=new List<EFormMed>();
			if(!String.IsNullOrEmpty(eFormField.ValueString)){
				listEFormMeds=JsonConvert.DeserializeObject<List<EFormMed>>(eFormField.ValueString);
			}
			if(IsSetupMode){
				EFormMed eFormMed=new EFormMed();
				eFormMed.MedName="Med 1";
				eFormMed.StrengthFreq="Strength/Freq 1";
				listEFormMeds.Add(eFormMed);
				eFormMed=new EFormMed();
				eFormMed.MedName="Med 2";
				eFormMed.StrengthFreq="Strength/Freq 2";
				listEFormMeds.Add(eFormMed);
				eFormMed=new EFormMed();
				eFormMed.MedName="etc...";
				listEFormMeds.Add(eFormMed);
			}
			double fontSize=FontSize*eFormField.FontScale/100;
			double heightRow=_heightGridRow*eFormField.FontScale/100;
			double widthCol1=eFormMedListLayout.WidthCol1;//these are just starting widths and will be reduced below as needed.
			double widthCol2=eFormMedListLayout.WidthCol2;
			double widthCol3=45*eFormField.FontScale/100;//width for delete button
			double widthCol1and2=widthAvail-widthCol3;
			if(widthCol1and2<0){
				widthCol1and2=0;
			}
			//widthCol1and2  is now what we use for available width in all the math below.
			//If either individual column is wider then allowed, here's where we fix that:
			if(widthCol1>widthCol1and2){
				widthCol1=widthCol1and2;
			}
			if(widthCol2>widthCol1and2){
				widthCol2=widthCol1and2;
			}
			if(eFormMedListLayout.IsCol2Visible){
				if(widthCol1>0 && widthCol2>0){
					//both columns have a width specified.
					if(widthCol1+widthCol2>widthCol1and2){
						//The sum is greater than allowed, so reduce them both proportionally
						//Example: widthCol1and2:100, widthCol1:40, widthCol2:80. Fixed widths: widthCol1:33.33, widthCol2:66.66
						double reductionRatio=widthCol1and2/(widthCol1+widthCol2);//Example: 100/(40+80)=.833
						widthCol1=widthCol1*reductionRatio;//Example: 40*.833=33.33
						widthCol2=widthCol2*reductionRatio;//Example: 80*.833=66.66
					}
				}
				//At this point, no column is wider than allowed, but either or both could be zero.
				if(widthCol1==0 && widthCol2==0){
					//50/50
					widthCol1=widthCol1and2/2;
					widthCol2=widthCol1and2/2;
				}
				else if(widthCol1==0){
					widthCol1=widthCol1and2-widthCol2;
				}
				else if(widthCol2==0){
					widthCol2=widthCol1and2-widthCol1;
				}
			}
			else{
				//we only care about col1
				widthCol2=0;//just in case they accidentally set a column width.
				if(widthCol1==0){
					widthCol1=widthCol1and2;
				}
			}
			//Title:
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			Label label = new Label();
			label.FontSize=fontSize;
			label.Padding=new Thickness(0);//default is 5
			label.Content=eFormMedListLayout.Title;
			stackPanelLabel.Children.Add(label);
			if(eFormField.IsRequired) {
				Label labelRequired=new Label();
				labelRequired.FontSize=FontSize*eFormField.FontScale/100;
				labelRequired.Padding=new Thickness(0);
				labelRequired.Content=" *";
				labelRequired.FontWeight=FontWeights.Bold;
				labelRequired.Foreground=Brushes.Red;
				stackPanelLabel.Children.Add(labelRequired);
			}
			bool isConditionalChild=false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x=>x.ValueLabel==eFormField.ConditionalParent))
			{
				isConditionalChild=true;
			}
			if(isConditionalChild){
				Label labelCondChild=new Label();
				stackPanelLabel.Children.Add(labelCondChild);
				labelCondChild.FontSize=FontSize*eFormField.FontScale/100;
				labelCondChild.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondChild.Content="(cnd)";
				labelCondChild.Foreground=Brushes.Red;
			}
			stackPanelVert.Children.Add(stackPanelLabel);
			//Main grid will hold the headers and the med rows.
			//Column widths were all calculated above.
			//The cells that are editable are textboxes.
			//Gridlines are a little tricky. You can skip them in the first pass because the textboxes act as gridlines. Then, only add minimal lines if truly necessary.
			Grid gridMeds=new Grid();
			stackPanelVert.Children.Add(gridMeds);
			ColumnDefinition columnDefinition;
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(widthCol1);
			gridMeds.ColumnDefinitions.Add(columnDefinition);
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(widthCol2);
			gridMeds.ColumnDefinitions.Add(columnDefinition);
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(widthCol3);
			gridMeds.ColumnDefinitions.Add(columnDefinition);
			RowDefinition rowDefinition;
			rowDefinition=new RowDefinition();
			rowDefinition.Height=new GridLength(heightRow);
			gridMeds.RowDefinitions.Add(rowDefinition);
			for(int i=0;i<listEFormMeds.Count;i++){
				rowDefinition=new RowDefinition();
				rowDefinition.Height=new GridLength(heightRow);
				gridMeds.RowDefinitions.Add(rowDefinition);
			}
			//Headers
			Border borderHeader1=new Border();
			borderHeader1.Background=new SolidColorBrush(ColorOD.Gray_Wpf(245));
			borderHeader1.BorderBrush=Brushes.Silver;
			borderHeader1.BorderThickness=new Thickness(1);
			gridMeds.Children.Add(borderHeader1);
			WpfControls.UI.Label labelHeader1 = new WpfControls.UI.Label();
			labelHeader1.HorizontalAlignment=HorizontalAlignment.Stretch;
			labelHeader1.HAlign=HorizontalAlignment.Center;
			labelHeader1.FontSize=fontSize;
			labelHeader1.Text=eFormMedListLayout.HeaderCol1;
			borderHeader1.Child=labelHeader1;
			Border borderHeader2=new Border();
			Grid.SetColumn(borderHeader2,1);
			borderHeader2.Background=new SolidColorBrush(ColorOD.Gray_Wpf(245));
			borderHeader2.BorderBrush=Brushes.Silver;
			borderHeader2.BorderThickness=new Thickness(1);
			gridMeds.Children.Add(borderHeader2);
			WpfControls.UI.Label labelHeader2 = new WpfControls.UI.Label();
			labelHeader2.HorizontalAlignment=HorizontalAlignment.Stretch;
			labelHeader2.HAlign=HorizontalAlignment.Center;
			labelHeader2.FontSize=fontSize;
			labelHeader2.Text=eFormMedListLayout.HeaderCol2;
			borderHeader2.Child=labelHeader2;
			//Med rows
			for(int i=0;i<listEFormMeds.Count;i++){
				WpfControls.UI.TextBox textBoxMed=new WpfControls.UI.TextBox();
				gridMeds.Children.Add(textBoxMed);
				textBoxMed.Width=widthCol1;
				textBoxMed.Text=listEFormMeds[i].MedName;
				textBoxMed.FontSize=fontSize;
				Grid.SetRow(textBoxMed,i+1);
				WpfControls.UI.TextBox textBoxFreq=new WpfControls.UI.TextBox();
				gridMeds.Children.Add(textBoxFreq);
				textBoxFreq.Width=widthCol2;
				textBoxFreq.Text=listEFormMeds[i].StrengthFreq;
				textBoxFreq.FontSize=fontSize;
				Grid.SetRow(textBoxFreq,i+1);
				Grid.SetColumn(textBoxFreq,1);
				WpfControls.UI.Button buttonDelete=new WpfControls.UI.Button();
				gridMeds.Children.Add(buttonDelete);
				buttonDelete.Text="Delete";
				buttonDelete.Width=widthCol3;
				buttonDelete.Height=heightRow;
				buttonDelete.FontSize=fontSize;
				buttonDelete.Click+=ButtonDelete_Click;
				Grid.SetRow(buttonDelete,i+1);
				Grid.SetColumn(buttonDelete,2);
			}
			//Footer
			StackPanel stackPanelFooter=new StackPanel();
			stackPanelFooter.Orientation=Orientation.Horizontal;
			stackPanelVert.Children.Add(stackPanelFooter);
			WpfControls.UI.Button buttonAdd=new WpfControls.UI.Button();
			stackPanelFooter.Children.Add(buttonAdd);
			buttonAdd.Text="Add";
			buttonAdd.Width=40*eFormField.FontScale/100;
			buttonAdd.Height=heightRow;
			buttonAdd.FontSize=fontSize;
			buttonAdd.Click+=ButtonAdd_Click;
			WpfControls.UI.CheckBox checkBoxNone=new WpfControls.UI.CheckBox();
			stackPanelFooter.Children.Add(checkBoxNone);
			checkBoxNone.Text="None";
			checkBoxNone.Margin=new Thickness(left:15,0,0,0);
			checkBoxNone.Height=heightRow;
			checkBoxNone.FontSize=fontSize;
		}

		private void AddPageBreak(Grid gridForField,EFormField eFormField) {
			//This only needs to be shown for setup, not for patient UI.
			//This is just to give the user a place to click to remove or move a page break.
			if(!IsSetupMode) {//If filling out an eForm, don't draw the pagebreak field.
				return;
			}
			Border border = new Border();
			border.BorderBrush=new SolidColorBrush(ColorOD.Gray_Wpf(192));
			border.BorderThickness=new Thickness(1);
			border.Height=30;
			gridForField.Children.Add(border);
			Label label = new Label();
			gridForField.Children.Add(label);
			double widthAvail=stackPanel.ActualWidth-_margins*2-1;
			if(widthAvail<0) {
				widthAvail=0;
			}
			label.Width=widthAvail;
			//this control is unique and will not have borders to left, right, or bottom for drag/drop.
			//so we set some margins here
			gridForField.Margin=new Thickness(0,0,right:_margins,bottom:_margins);
			label.Padding=new Thickness(4,0,0,3);
			label.Content="(page break)";
			label.VerticalAlignment=VerticalAlignment.Center;
			//add a button to delete
			WpfControls.UI.Button button = new WpfControls.UI.Button();
			gridForField.Children.Add(button);
			Grid.SetZIndex(button,3);//bring it in front of the hover border so it's clickable
			//button.Margin=new Thickness(15,0,0,0);
			button.Icon=WpfControls.UI.EnumIcons.DeleteX;
			button.Height=24;
			button.VerticalAlignment=VerticalAlignment.Center;
			button.HorizontalAlignment=HorizontalAlignment.Right;
			button.Visible=false;//Don't show the button until hovering over the field.
			button.Margin=new Thickness(0,0,right:2,0);
		}

		private void AddRadioButtons(Grid gridForField,EFormField eFormField){
			StackPanel stackPanelRadio=new StackPanel();
			//stackPanelRadio is our main stackPanel
			//It has two children: stackPanelLabel and wrapPanelRadio
			if(eFormField.LabelAlign==EnumEFormLabelAlign.TopLeft){
				stackPanelRadio.Orientation=Orientation.Vertical;
			}
			if(eFormField.LabelAlign==EnumEFormLabelAlign.LeftLeft){
				stackPanelRadio.Orientation=Orientation.Horizontal;
			}
			gridForField.Children.Add(stackPanelRadio);
			double widthAvail=stackPanel.ActualWidth-_margins*2;
			if(widthAvail<0) {
				widthAvail=0;
			}
			stackPanelRadio.Width=widthAvail;
			//RadioButton groups are always full width.
			//So this is not a good example of the normal math to use for width.
			//See further down for how setting width affects label size.
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			Label label = new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0,0,0,bottom:0);//default is 5
			label.Content=eFormField.ValueLabel;
			if(eFormField.LabelAlign==EnumEFormLabelAlign.LeftLeft){
				stackPanelLabel.Margin=new Thickness(0,0,right:10,0);
			}
			stackPanelLabel.Children.Add(label);
			if(eFormField.IsRequired) {
				Label labelRequired=new Label();
				labelRequired.FontSize=FontSize*eFormField.FontScale/100;
				labelRequired.Padding=new Thickness(0);
				labelRequired.Content=" *";
				labelRequired.FontWeight=FontWeights.Bold;
				labelRequired.Foreground=Brushes.Red;
				stackPanelLabel.Children.Add(labelRequired);
			}
			bool isConditionalParent=false;
			if(IsSetupMode
				&& ListEFormFields.Exists(x=>x.ConditionalParent==eFormField.ValueLabel))
			{
				isConditionalParent=true;
			}
			if(isConditionalParent){
				Label labelCondParent = new Label();
				stackPanelLabel.Children.Add(labelCondParent);
				labelCondParent.FontSize=FontSize*eFormField.FontScale/100;
				labelCondParent.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondParent.Content="(CND)";
				labelCondParent.Foreground=Brushes.Red;
			}
			bool isConditionalChild=false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x=>x.ValueLabel==eFormField.ConditionalParent))
			{
				isConditionalChild=true;
			}
			if(isConditionalChild){
				//yes, we let them include both parent and child in case a field is both.
				Label labelCondChild = new Label();
				stackPanelLabel.Children.Add(labelCondChild);
				labelCondChild.FontSize=FontSize*eFormField.FontScale/100;
				labelCondChild.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondChild.Content="(cnd)";
				labelCondChild.Foreground=Brushes.Red;
			}
			stackPanelRadio.Children.Add(stackPanelLabel);
			WrapPanel wrapPanelRadio=new WrapPanel();
			if(eFormField.LabelAlign==EnumEFormLabelAlign.LeftLeft){
				Graphics g=Graphics.MeasureBegin();
				Font font=Font.ForWpf();
				font.SizeDip=12*eFormField.FontScale/100;
				//I'm a little unclear if this control is using 12 DIP and/or 12 point, especially for printing.
				//12 DIP looks good on screen and is very close to our default 11.5 DIP.
				//12 point is very typical for printing and I think it's our default for sheets.
				//This will be clarified later
				double wLabel=g.MeasureString(eFormField.ValueLabel,font,includePadding:false).Width;
				wLabel+=10;//the right margin on these labels
				if(eFormField.IsRequired) {
					font.IsBold=true;
					wLabel+=g.MeasureString(" *",font,includePadding:false).Width;
				}
				if(isConditionalParent){
					font.IsBold=false;
					wLabel+=g.MeasureString("(CND)",font,includePadding:false).Width+5;
				}
				if(isConditionalChild){
					font.IsBold=false;
					wLabel+=g.MeasureString("(cnd)",font,includePadding:false).Width+5;
				}
				if(eFormField.Width>0){
					wLabel=eFormField.Width;
					
				}
				stackPanelLabel.Width=wLabel;//here just for debugging. Move up.
				wrapPanelRadio.Width=stackPanelRadio.Width-wLabel;
				//The math above has some sort of flaw.
				//I'm getting a width that isn't wide enough, so too much margin on right.
			}
			wrapPanelRadio.Orientation=Orientation.Horizontal;//radiobuttons go horizontal as much as possible.
			List<string> listPickVis=eFormField.PickListVis.Split(',').ToList();
			List<string> listPickDb=eFormField.PickListDb.Split(',').ToList();
			for(int i=0;i<listPickVis.Count;i++){
				//I decided to use the OD radiobutton because I have more control over the layout and behavior
				WpfControls.UI.RadioButton radioButton=new WpfControls.UI.RadioButton();
				radioButton.HorizontalAlignment=HorizontalAlignment.Stretch;
				radioButton.VerticalAlignment=VerticalAlignment.Stretch;
				radioButton.textBlock.Margin=new Thickness(left:1,0,0,0);//instead of 4 to make it closer
				radioButton.Margin=new Thickness(0,0,right:_margins,0);
				radioButton.Text=listPickVis[i];
				radioButton.FontSize=FontSize*eFormField.FontScale/100;
				if(!IsSetupMode){
					if(eFormField.DbLink==""){//none
						if(listPickVis[i]==eFormField.ValueString){
							radioButton.Checked=true;
						}
					}
					else{//has a db link
						if(listPickDb[i]==eFormField.ValueString){
							radioButton.Checked=true;
						}
					}
				}
				radioButton.Click+=(sender,e)=>{
					FillFieldsFromControls(eFormField);
					SetVisibilities(_pageShowing,forceRefresh:true);
				};
				wrapPanelRadio.Children.Add(radioButton);
			}
			stackPanelRadio.Children.Add(wrapPanelRadio);
		}

		private void AddSigBox(Grid gridForField,EFormField eFormField){
			StackPanel stackPanel2=new StackPanel();
			gridForField.Children.Add(stackPanel2);
			double widthAvail=stackPanel.ActualWidth-_margins*2;
			if(widthAvail<0) {
				widthAvail=0;
			}
			if(eFormField.Width==0){//not specified
				stackPanel2.Width=widthAvail;//so full width
			}
			else if(eFormField.Width<widthAvail){
				stackPanel2.Width=eFormField.Width;
			}
			else{
				stackPanel2.Width=widthAvail;
			}
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			Label label = new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0,0,0,bottom:5);//default is 5
			label.Content=eFormField.ValueLabel;
			stackPanelLabel.Children.Add(label);
			if(eFormField.IsRequired) {
				Label labelRequired=new Label();
				labelRequired.FontSize=FontSize*eFormField.FontScale/100;
				labelRequired.Padding=new Thickness(0);
				labelRequired.Content=" *";
				labelRequired.FontWeight=FontWeights.Bold;
				labelRequired.Foreground=Brushes.Red;
				stackPanelLabel.Children.Add(labelRequired);
			}
			bool isConditionalChild=false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x=>x.ValueLabel==eFormField.ConditionalParent))
			{
				isConditionalChild=true;
			}
			if(isConditionalChild){
				Label labelCondChild=new Label();
				stackPanelLabel.Children.Add(labelCondChild);
				labelCondChild.FontSize=FontSize*eFormField.FontScale/100;
				labelCondChild.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondChild.Content="(cnd)";
				labelCondChild.Foreground=Brushes.Red;
			}
			stackPanel2.Children.Add(stackPanelLabel);
			Rectangle rectangle=new Rectangle();
			rectangle.Width=stackPanel2.Width;
			rectangle.Height=150;
			rectangle.Stroke=Brushes.Silver;
			rectangle.StrokeThickness=1;
			stackPanel2.Children.Add(rectangle);
		}

		private void AddTextBox(Grid gridForField,EFormField eFormField){
			StackPanel stackPanel2=new StackPanel();
			gridForField.Children.Add(stackPanel2);
			double widthAvail=stackPanel.ActualWidth-_margins*2;
			if(widthAvail<0) {
				widthAvail=0;
			}
			if(eFormField.Width==0){//not specified
				stackPanel2.Width=widthAvail;//so full width
			}
			else if(eFormField.Width<widthAvail){
				stackPanel2.Width=eFormField.Width;
			}
			else{
				stackPanel2.Width=widthAvail;
			}
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			Label label = new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0);//default is 5
			label.Content=eFormField.ValueLabel;
			stackPanelLabel.Children.Add(label);
			if(eFormField.IsRequired) {
				Label labelRequired=new Label();
				labelRequired.FontSize=FontSize*eFormField.FontScale/100;
				labelRequired.Padding=new Thickness(0);
				labelRequired.Content=" *";
				labelRequired.FontWeight=FontWeights.Bold;
				labelRequired.Foreground=Brushes.Red;
				stackPanelLabel.Children.Add(labelRequired);
			}
			bool isConditionalChild=false;
			if(IsSetupMode
				&& eFormField.ConditionalParent!=""
				&& ListEFormFields.Exists(x=>x.ValueLabel==eFormField.ConditionalParent))
			{
				isConditionalChild=true;
			}
			if(isConditionalChild){
				//yes, we let them include both parent and child in case a field is both.
				Label labelCondChild = new Label();
				stackPanelLabel.Children.Add(labelCondChild);
				labelCondChild.FontSize=FontSize*eFormField.FontScale/100;
				labelCondChild.Padding=new Thickness(5,0,0,0);//default is 5
				labelCondChild.Content="(cnd)";
				labelCondChild.Foreground=Brushes.Red;
			}
			stackPanel2.Children.Add(stackPanelLabel);
			TextBox textBox=new TextBox();
			if(eFormField.IsTextWrap){
				textBox.TextWrapping=TextWrapping.Wrap;
			}
			else{
				//textBox.BorderThickness=new Thickness(0,0,0,bottom:1);
			}
			textBox.Text=eFormField.ValueString;
			textBox.FontSize=FontSize*eFormField.FontScale/100;
			stackPanel2.Children.Add(textBox);
		}

		///<summary>Creates a new border, sets some values, and adds it to _listBordersDrops.</summary>
		private Border CreateBorderForDrop(){
			Border border=new Border();
			_listBordersDrops.Add(border);
			Color color=Colors.Transparent;
			//just for testing:
			//byte r = (byte)_random.Next(0,256);
			//byte g = (byte)_random.Next(0,256);
			//byte b = (byte)_random.Next(0,256);
			//color = Color.FromRgb(r,g,b);
			border.Background=new SolidColorBrush(color);
			return border;
		}

		private Grid CreateGridForField(){
			//This is the main container for the eFormField.
			//TagOD gets set to the eFormField.
			Grid gridForField=new Grid();
			//_listGridFields.Add(grid);
			ColumnDefinition columnDefinition;
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//main content
			gridForField.ColumnDefinitions.Add(columnDefinition);
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(_margins);
			gridForField.ColumnDefinitions.Add(columnDefinition);
			//We are adding the border here because we use Children[1], for example, in import.
			//This allows us to not fiddle with that code, because the border is child 0.
			Border borderOverlayFieldHover=new Border();
			gridForField.Children.Add(borderOverlayFieldHover);
			if(IsSetupMode){
				borderOverlayFieldHover.BorderThickness=new Thickness(1);
				Grid.SetZIndex(borderOverlayFieldHover,1);//in front of the others
			}
			return gridForField;
		}

		///<summary>Loops through all fields and fixes invalid stacking. Used after a multi drag drop, when first loading to fix any prior db issues, and after editing each field because changing stacking in one field can affect stacking in other fields. Doesn't change orders of any fields.</summary>
		private void FixAllStacking(){
			for(int i=0;i<ListEFormFields.Count;i++){
				if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[i].FieldType)){
					//the simple obvious fix for self
					ListEFormFields[i].IsHorizStacking=false;
				}
			}
			//Now that all selves are fixed, another loop will check field to left of each
			for(int i=1;i<ListEFormFields.Count;i++){
				//if field to left can't stack, neither can this one.
				if(!EFormFieldDefs.IsHorizStackable(ListEFormFields[i-1].FieldType)){
					ListEFormFields[i].IsHorizStacking=false;
				}
			}
			//If a field is stacking or is before a stacking field, give it a set width
			for(int i=0;i<ListEFormFields.Count;i++){
				//too confusing to refactor this if to kick out
				if(ListEFormFields[i].IsHorizStacking//if this field is stacking
					|| (i<ListEFormFields.Count-1 && ListEFormFields[i+1].IsHorizStacking))//or the next field is stacking
				{
					if(ListEFormFields[i].Width>0){
						continue;//a width is already set
					}
					if(ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
						continue;//checkboxes don't have widths. Any width we added would just be ignored.
					}
					//Well, we don't really know what the available width is because it depends on the device.
					//We will pick an arbitrary max.
					//Remember that when laid out, this width will automatically shrink if not enough space
					Graphics g=Graphics.MeasureBegin();
					double width=g.MeasureString(ListEFormFields[i].ValueLabel).Width;
					if(width>300){
						width=300;
					}
					//double widthNew=width;
					if(ListEFormFields[i].FieldType==EnumEFormFieldType.TextField
						|| ListEFormFields[i].FieldType==EnumEFormFieldType.DateField)
					{
						width+=15;
					}
					if(ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
						width+=25;
					}
					ListEFormFields[i].Width=(int)width;
				}
			}
			//IF a field is not stacking, we will leave it alone.
			//It seems harmless to leave them at a fixed width..
			//It gives users more control, and I can see how they would need to make some fields narrower.
		}

		///<summary>Gets the index at the specified point. Returns -1 if no index can be selected at that point. Pass in the y pos relative to this control. It can fall outside the control when dragging.</summary>
		public int IndexFromGrid(Grid grid){
			for(int i=0;i<ListEFormFields.Count;i++){
				if(ListEFormFields[i].TagOD==grid){
					return i; 
				}
			}			
			return -1;
		}

		///<summary>Sets background colors for all items, based on selected indices and hover. Also, sets visibility of Delete button on pagebreak field.</summary>
		private void SetColors(){
			for(int i=0;i<ListEFormFields.Count;i++){
				Grid gridForField=ListEFormFields[i].TagOD as Grid;
				Border border=gridForField.Children[0] as Border;
				Color colorBack=Colors.Transparent;
				Color colorBorder=Colors.Transparent;
				if(_listSelectedIndices.Contains(i)){
					colorBack=Color.FromArgb(30,0,100,200);
					colorBorder=Color.FromArgb(90,0,100,200);
				}
				else if(_hoverIndex==i){
					colorBack=Color.FromArgb(15,0,80,210);
					colorBorder=Color.FromArgb(45,0,80,210);
				}
				border.Background=new SolidColorBrush(colorBack);
				border.BorderBrush=new SolidColorBrush(colorBorder);
			}
			//pagebreak Delete button visibility
			for(int i = 0;i<ListEFormFields.Count;i++) {
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.PageBreak) {
					continue;
				}
				Grid gridForField = ListEFormFields[i].TagOD as Grid;
				WpfControls.UI.Button button = gridForField.Children[3] as WpfControls.UI.Button;
				button.Visible=false;
				if(_hoverIndex==i) {
					button.Visible=true;
				}
			}
			//Conditional test button visibility
			//for(int i = 0;i<ListEFormFields.Count;i++) {
			//	if(ListEFormFields[i].FieldType!=EnumEFormFieldType.RadioButtons) {
			//		continue;
			//	}
			//	if(!ListEFormFields.Exists(x=>x.ConditionalParent==ListEFormFields[i].ValueLabel)){
			//		continue;
			//	}
			//	//button hidden by default
			//	Grid gridForField = ListEFormFields[i].TagOD as Grid;
			//	WpfControls.UI.Button button = gridForField.Children[2] as WpfControls.UI.Button;
			//	button.Visible=false;
			//	if(_hoverIndex==i) {
			//		button.Visible=true;
			//	}
			//}
			for(int i=0;i<_listBordersDrops.Count;i++){
				if(_borderDropHover==_listBordersDrops[i] && _listSelectedIndices.Count>0){
					_listBordersDrops[i].Background=new SolidColorBrush(Color.FromRgb(255,210,180));
				}
				else{
					_listBordersDrops[i].Background=Brushes.Transparent;
				}
			}
		}

		private void SetActionTimer(Action action,double interval) {
			if(_actionTimer!=action) {//This will prevent the timer from stoping when the mouse is still moving outside of the bounds.
				action?.Invoke();//Immediately scroll or change the page.
				_dispatcherTimer.Stop();//Stop any previous timer
				_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(interval);//Set the intervals
				_actionTimer=action;
				_dispatcherTimer.Start();
			}
			if(_dispatcherTimer.Interval!=TimeSpan.FromMilliseconds(interval)) {
				//Speed up or slow down the interval speed. This is dependent on the mouse Y value from the mouseMove method. 
				_dispatcherTimer.Stop();
				_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(interval);
				_dispatcherTimer.Start();
			}
		}

		///<summary>This method will set a flag which is used to skip "empty" pages. It only gets called when filling out a form. We don't want to skip pages when they are in setup mode.</summary>
		private void SetIsHiddenFlag() {
			for(int i=0;i<ListEFormFields.Count;i++) {
				if(ListEFormFields[i].ConditionalParent=="") {
					continue;
				}
				EFormField eFormFieldParent=ListEFormFields.Find(x=>x.ValueLabel==ListEFormFields[i].ConditionalParent);
				if(eFormFieldParent is null) {
					continue;
				}
				bool isConditionMet=false;
				if(eFormFieldParent.FieldType==EnumEFormFieldType.RadioButtons 
					&& EFormFields.ConvertValueStringDbToVis(eFormFieldParent)==ListEFormFields[i].ConditionalValue)
				{
					isConditionMet=true;
				}
				if(eFormFieldParent.FieldType==EnumEFormFieldType.CheckBox 
					&& eFormFieldParent.ValueString==ListEFormFields[i].ConditionalValue)
				{
					isConditionMet=true;
				}
				if(isConditionMet){
					ListEFormFields[i].IsHiddenCondit=false;
				}
				else{
					ListEFormFields[i].IsHiddenCondit=true;
				}
			}
		}
		#endregion Methods - private

		#region Classes nested
		private class DragLocation{
			///<summary>If the drop happens at this location, Idx is the index that the dropped field would get.</summary>
			public int Idx;
			///<summary>This location is one of the wide horizontal areas above a wrap panel.</summary>
			public bool IsAbove;
			///<summary>Set to true to cause a field inserted at this location to get stacked horizontally compared to its previous sibling</summary>
			public bool IsHorizStacking;
			///<summary>Set to true to cause the field to the right of this location to get stacked horizontally. The actual field inserted will not get set to IsHorizStacking. Even gets set to true if stacking is not allowed because that validation happens elsewhere.</summary>
			public bool IsHorizStackingNext;
			///<summary>True if this is the location at the very left of the page.</summary>
			public bool IsLeftOfWrap;
			///<summary>When hovering over a field, this indicates which drag location to use. There can also be secondary drag locations, but those would have to be specifically hovered over.</summary>
			public bool IsPrimary;
			///<summary>True if this is the location at the very right of a horiz wrap panel.</summary>
			public bool IsRightOfWrap;
			public int Page;
		}
		#endregion Classes nested
	}
}
