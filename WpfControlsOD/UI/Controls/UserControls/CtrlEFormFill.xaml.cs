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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Newtonsoft.Json;
using OpenDental.Drawing;
using CodeBase;

namespace OpenDental {
	/*
	Control Groups:
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

	Border Boxes:
	Each field can have an optional border box that can be turned on by the user.
	A border never includes multiple fields.
	This border takes up additional space, making the field look bigger in both dimensions.
	The border does not change the margins and spacing described below.
	The extra white space that's created within the border should be thought of as padding within the field rather then margins between fields.
	The user will never have control over this padding.
	For a fixed width field, the specified width is for the field itself, without the surrounding padding or border box thickness.
	The border itself is not uniform. The bottom takes 6 pixels (5 for shadow) and the other three sides take one pixel.
	The thickness of the border does not contribute to either the padding or the margins of each field.
	So the contents of a field is surrounded by padding + border + margin.
	All of those values are clearly defined here and must be copied precisely in any implementation.
	To make the left edges of fields line up whether they have a border or not, we include left padding and left border thickness whether a border is present or not.
	One way to do that is to define a different border in that case which is white with certain thicknesses.
	Padding and border thickness on the other three sides should only be present when a border is present.
	I want two label fields to be able to get very close to each other vertically when there is no border.
	In C#, the way to make this work is for the field to be a child of the border. A C# border can have thickness, margin, and padding.

	Margins and spacing:
	I apologize for the complexity of this code. 
	Anyone using this code to try to build a UI in a different language should use a much simpler approach.
	You will not have to support drag, drop, hover, and selection in the setup window.
	So try to imagine it simpler:
	There must be a certain amount of white space around each field (or around each border box).
	You must have a very solid plan for how to create this white space. There are lots of ways to get it wrong.
	Each of these spaces is well defined at the class level below.
	Variables are best because we will gradually be giving users more control.
	For example, they now have control over vertical spacing below each field.
	We plan to give them control over more spacing in the future.
	Since this section is about margins, it's really the white space between border boxes.
	None of this has anything to do with border thickness or padding.
	Each field should have right and bottom margins.
	It's required to do it this way because of how we give users control.
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
	The calculation of width is contained in a new method: CalcFieldWidth
	The complexity increased significantly with the addition of percentage width fields.
	DO follow my exact algorithm for calculating the width for each field.
	There's not a shorter way to do it.
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
	Furthermore, the math must work for both fixed widths and percentage widths.
	And whatever I come up with here must be very easy to implement in any UI language.
	I decided that we must always calculate each field width ourselves as part of layout.
	We still put them in a wrapPanel, and we leave the heights auto, but we must calculate the widths.
	So set the width of each field container very intentionally according to my algorithm.
	To determine available width, subtract the right margin (ex 10) from the StackPanel width.
	The controls inside the field can stretch horizontally to fill; that's easy.
	Once we do the math to set each field width, it all works automatically.
	This math can be done in any UI language.
	It also follows that if the screen size changes, we must redo the layout.
	But screen size really shouldn't be changing unless the user maybe rotates the layout.

	Tags:
	Tags are used as follows:
	1. Each EFormField in the list has its TagOD set to the field(borderBox actually) that it's associated with.
			One purpose is to allow us to pull values from the UI into our EFormField objects.
			Another purpose is to allow our highlighting for hover effects and selection in the setup window,
				but the patient UI does not need highlighting, so that can be ignored there.
			These tags also allow "paging" by collapsing the fields not on the current page.
			Finally, the tags allow collapsing for conditional logic.
	2. We also have DragLocation tags on the various borders that we use for drag/drop.
			The patient UI clearly does not need those.

	Heirarchy of controls:
	This heirarchy is unique to this C# implementation.
	It will be different and simpler in any other language because no support for setup is needed.
	-scrollViewer
		-stackPanel
			-borderTopOfPage
			-gridWrap
				-borderLeftOfWrap
				-wrapPanel (horiz)
					-gridForField
						-borderOverlayFieldHover (colspan 2, z-index2, child0)
						-borderBox (col0, z-index1 child1)
							-(1 container for field controls, varies by type)
						-borderRightOfField (col1, child2)
						-(some drop shadow elements) (col0, z-index0, child3+)
					-gridForField
						-borderOverlayFieldHover
						-borderBox (col0)
							-(1 container for field controls)
						-borderRightOfField
					-gridForField
						-borderOverlayFieldHover
						-borderBox (col0)
							-(1 container for field controls)
						-borderRightOfField
			-borderTopOfWrap
			-gridWrap
				-borderLeftOfWrap
				-wrapPanel (horiz)
					-gridForField
						-borderOverlayFieldHover
						-borderBox (col0)
							-(1 container for field controls)
						-borderRightOfField
			-borderTopOfWrap
			-gridWrap
				-borderLeftOfWrap
				-wrapPanel (horiz)
					-gridForField
						-borderOverlayFieldHover
						-borderBox (col0)
							-(1 container for field controls)
						-borderRightOfField
					-gridForField
						-borderOverlayFieldHover
						-borderBox (col0)
							-(1 container for field controls)
						-borderRightOfField
					-gridForField
						-borderOverlayFieldHover
						-borderBox (col0)
							-(1 container for field controls)
						-borderRightOfField




	*/
	///<summary></summary>
	public partial class CtrlEFormFill:UserControl {
		#region Fields - public
		///<summary>In setup mode, the UI behavior for mouse is quite different. Layout is the similar.</summary>
		public bool IsSetupMode;
		///<summary>This is referenced extensively by the parent form. Items are sometimes changed directly, and sometimes converted to EFormFieldDefs, manipulated, and then converted back to EFormFields.</summary>
		public List<EFormField> ListEFormFields;
		///<summary>This contains all the EFormField.DefNums that were deleted during this session, but only if they were originally part of the eForm. Any field that was created in this session and then deleted does not get included here. Those get truly deleted when user clicks Delete within each field edit window. This list is public because the parent form does all the actual deletions.</summary>
		public List<long> ListEFormFieldDefNumsToDelete;
		///<summary>Stores the indices of all the EFormFields that are at the rightmost position of a horizontal stack.</summary>
		public List<int> ListIndicesLastInHorizStack;
		public int PagesPrinted;
		///<summary>Set this whenever comboBox changes. It's the current language being used.  Will be empty string if languages are not set up in pref LanguagesUsedByPatients or if the default language is being used in the parent FrmEFormDefs.</summary>
		public string LanguageShowing="";
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
		///<summary>Property backer</summary>
		private bool _isReadOnly;
		///<summary>This is a list of the borders that are used for drag drop.</summary>
		private List<Border> _listBordersDrops=new List<Border>();
		///<summary>This is the only internal storage for tracking selected indices. Not guaranteed to be ordered least to greatest.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		///<summary>This is a copy of the selected indices whenever we start dragging to select multiple.</summary>
		private List<int> _listSelectedIndicesWhenSelectionStart=new List<int>();
		private double _marginTopOfPage=10;
		private double _marginLeftOfPage=5;
		///<summary>This also creates the margin for the right of the page. Right now, that's no problem since they are all the same. I think any page margin in the future would be in addition to this, not instead of it.</summary>
		private double _marginRightOfField=10;
		///<summary>This is stored in a db pref and set on load. This also creates the margin at the bottom of the page. In the future, we could do something different at the bottom of the page, either replacing or supplementing this.</summary>
		private double _marginBelowFields;
		///<summary>This is the margin to the right of each radiobutton within a group.</summary>
		private double _marginRightOfRadioButton=10;
		private int _mouseDownIndex;
		///<summary>=4 The amount of padding on the left side of each field within its border box.</summary>
		private int _paddingLeft=4;
		///<summary>=4 The amount of padding on the right side of each field within its border box.</summary>
		private int _paddingRight=4;
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
		///<summary>This the index (not count) of the last stackPanel child that we have printed.</summary>
		private int _printedStackPanelChildren;
		///<summary>This is class level because when multiple random numbers are quickly generated, they are based on the system clock and will be identical unless Random is reused.</summary>
		private Random _random=new Random();
		///<summary>This is the sum of the thickness of the left and right of the border box. It's 1+1=2</summary>
		private int _thicknessLRBorders=2;
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
		///<summary>This isn't used in setup mode. Nav buttons will still work.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsReadOnly { 
			get{
				return _isReadOnly;
			}
			set{
				_isReadOnly=value;
				if(_isReadOnly){
					rectangleBlocker.Visibility=Visibility.Visible;
				}
				else{
					rectangleBlocker.Visibility=Visibility.Collapsed;
				}
			}
		} 

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
		///<summary>If a single field is passed in, only that field will be filled. If null, all fields will be filled. We generally call this (and SetVisibilities) for a single field every time a user clicks a checkbox or radiobutton. This is so we can have immediate results for conditional logic. But we don't call it for textboxes, for example, but it's hard to know when to do so.</summary>
		public void FillFieldsFromControls(EFormField eFormField=null){			
			for(int i=0;i<ListEFormFields.Count;i++){
				if(eFormField!=null && ListEFormFields[i]!=eFormField){
					continue;
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
					Border borderBox=ListEFormFields[i].TagOD as Border;
					CheckBox checkBox=borderBox.Child as CheckBox;
					if(checkBox.IsChecked==true) {
						ListEFormFields[i].ValueString="X";
						continue;
					}
					ListEFormFields[i].ValueString="";
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.DateField){
					Border borderBox=ListEFormFields[i].TagOD as Border;
					StackPanel stackPanel=borderBox.Child as StackPanel;
					WpfControls.UI.TextVDate textVDate=stackPanel.Children[1] as WpfControls.UI.TextVDate;
					ListEFormFields[i].ValueString=textVDate.Text;
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.Label){
					Border borderBox=ListEFormFields[i].TagOD as Border;
					WpfControls.UI.TextRich textRich=borderBox.Child as WpfControls.UI.TextRich;
					ListEFormFields[i].ValueLabel=EFormFields.SerializeFlowDocument(textRich.richTextBox.Document);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.MedicationList){
					Border borderBox=ListEFormFields[i].TagOD as Border;
					StackPanel stackPanelVert=borderBox.Child as StackPanel;
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
					Border borderBox=ListEFormFields[i].TagOD as Border;
					StackPanel stackPanel=borderBox.Child as StackPanel;
					WrapPanel wrapPanel=null;
					if(ListEFormFields[i].LabelAlign==EnumEFormLabelAlign.TopLeft
						|| ListEFormFields[i].LabelAlign==EnumEFormLabelAlign.LeftLeft)
					{
						wrapPanel=stackPanel.Children[1] as WrapPanel;
					}
					if(ListEFormFields[i].LabelAlign==EnumEFormLabelAlign.Right){
						wrapPanel=stackPanel.Children[0] as WrapPanel;
					}
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

					Border borderBox=ListEFormFields[i].TagOD as Border;
					StackPanel stackPanel=borderBox.Child as StackPanel;
					WpfControls.UI.TextBox textBox=stackPanel.Children[1] as WpfControls.UI.TextBox;
					ListEFormFields[i].ValueString=textBox.Text;
				}
			}
			//After pulling all other fields, we can pull signatures from sigboxes 
			for(int i=0;i<ListEFormFields.Count;i++){
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.SigBox){
					continue;
				}
				Border borderBox=ListEFormFields[i].TagOD as Border;
				StackPanel stackPanel=borderBox.Child as StackPanel;
				WpfControls.UI.SignatureBoxWrapper signatureBoxWrapper=stackPanel.Children[1] as WpfControls.UI.SignatureBoxWrapper;
				string keyData=EForms.GetSignatureKeyData(ListEFormFields);
				string signature="0";
				if(signatureBoxWrapper.GetSigIsTopaz()){
					signature="1";
				}
				signature+=signatureBoxWrapper.GetSignature(keyData);
				ListEFormFields[i].ValueString=signature;
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

		public bool Pd_PrintPage(Graphics g) {
			if(PagesPrinted==0){
				_printedStackPanelChildren=0;
				SetVisibilities(pgRequested:-1,forceRefresh:true);
				stackPanel.UpdateLayout();
			}
			double yPos=0;//yPos is for this page only
			for(int i=0;i<stackPanel.Children.Count;i++){
				if(_printedStackPanelChildren>=i){
					//this is how we start part way down after the first page
					//example, we've printed 0 to 4, so _printedStackPanelChildren=4.
					//On second page, at i==4, 4>=4 is true so kicks out. at i=5, 4>=5 is false, so it prints.
					continue;
				}
				FrameworkElement frameworkElement=(FrameworkElement)stackPanel.Children[i];
				Grid gridWrap=frameworkElement as Grid; 
				if(gridWrap is null){
					//alternative is borderTopOfPage/Wrap
					//g.DrawFrameworkElement(frameworkElement,0,yPos);//no need to draw
					yPos+=frameworkElement.ActualHeight;
					_printedStackPanelChildren=i;
					continue;
				}
				//see if there's room to draw
				if(frameworkElement.ActualHeight>g.Height){
					//This element is taller than the whole page.
					//For now, just print what we can. Later enhancement could chop up a single gridWrap, which is probably a huge label.
				}
				else if(yPos+frameworkElement.ActualHeight>g.Height){
					PagesPrinted++;
					return true;//There will be another page. On the next page, yPos will be higher up, so it will fit
				}
				//Look for page break
				for(int k=0;k<gridWrap.Children.Count;k++){
					WrapPanel wrapPanel=gridWrap.Children[k] as WrapPanel;
					if(wrapPanel is null){
						continue;//alternative is borderLeftOfWrap
					}
					Grid gridForField=(Grid)wrapPanel.Children[0];
					//a page break has no children in its gridForField except for the borderOverlayFieldHover and borderRightOfField
					if(gridForField.Children.Count==2){
						g.DrawLine(Colors.SlateGray,_marginLeftOfPage,yPos,800,yPos);
					}
				}
				if(frameworkElement.ActualHeight==0){
					//a pageBreak will also be 0 height
					_printedStackPanelChildren=i;
					continue;
				}
				//Look for sigBox
				bool isSigBox=false;
				for(int k=0;k<gridWrap.Children.Count;k++){
					WrapPanel wrapPanel=gridWrap.Children[k] as WrapPanel;
					if(wrapPanel is null){
						continue;//alternative is borderLeftOfWrap
					}
					Grid gridForField=(Grid)wrapPanel.Children[0];
					//many wrapPanels will have more than one child gridForField, but for sigs, we only care about first.
					EFormField eFormField=ListEFormFields.Find(x=>x.TagOD==gridForField);
					if(eFormField is null){
						continue;
					}
					if(eFormField.FieldType!=EnumEFormFieldType.SigBox){
						continue;
					}
					StackPanel stackPanel2=(StackPanel)gridForField.Children[1];//child 0 is borderOverlayFieldHover
					StackPanel stackPanelLabel=(StackPanel)stackPanel2.Children[0];
					g.DrawFrameworkElement(stackPanelLabel,_marginLeftOfPage,yPos);
					yPos+=stackPanelLabel.ActualHeight;
					WpfControls.UI.SignatureBoxWrapper signatureBoxWrapper=(WpfControls.UI.SignatureBoxWrapper)stackPanel2.Children[1];
					BitmapImage bitmapImage=signatureBoxWrapper.GetBitmapImage();
					g.DrawImage(bitmapImage,_marginLeftOfPage,yPos);
					yPos+=frameworkElement.ActualHeight;
					_printedStackPanelChildren=i;
					isSigBox=true;
				}
				if(isSigBox){
					continue;
				}
				g.DrawFrameworkElement(frameworkElement,0,yPos);
				yPos+=frameworkElement.ActualHeight;
				_printedStackPanelChildren=i;
			}
			return false;//no more pages.
		}

		///<summary>Fixes stacking, clears the stackpanel, adds back all new fresh controls to the stackpanel, scrolls back to the same offset as before, reselects if one was selected.</summary>
		public void RefreshLayout(){
			FixAllStacking();
			ListIndicesLastInHorizStack=new List<int>();
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
					borderTopOfPage.Height=_marginTopOfPage;
					borderTopOfPage.Margin=new Thickness(left:_marginLeftOfPage,0,right:_marginRightOfField,0);
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
					//There might be an edge case for the first item on a page being set to horiz.
					//That will be ignored as invalid. Here's how to do that:
					|| ListEFormFields[i-1].FieldType==EnumEFormFieldType.PageBreak)
				{
					wrapPanel=new WrapPanel();
					wrapPanel.Orientation=Orientation.Horizontal;
					//But because we need a margin on the left of the page,
					//and that margin needs to be filled with a border for hover effects,
					//we will actually fill this stackpanel with grids.
					wrapPanel.Margin=new Thickness(left:_marginLeftOfPage,0,0,0);
					gridWrap=new Grid();
					//gridWrap.Tag=_pageCount;
					//This drag/drop border is to the left of the entire wrap panel
					Border borderLeftOfWrap=CreateBorderForDrop();
					borderLeftOfWrap.Width=_marginLeftOfPage;
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
				//GridForField-----------------------------------------------------------------------------------------------------------------------------
				Grid gridForField=new Grid();
				ColumnDefinition columnDefinition;
				columnDefinition=new ColumnDefinition();
				columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//main content
				gridForField.ColumnDefinitions.Add(columnDefinition);
				columnDefinition=new ColumnDefinition();
				columnDefinition.Width=new GridLength(_marginRightOfField);
				gridForField.ColumnDefinitions.Add(columnDefinition);
				Border borderOverlayFieldHover=new Border();
				gridForField.Children.Add(borderOverlayFieldHover);
				if(IsSetupMode){
					//borderOverlayFieldHover.BorderThickness=new Thickness(1);//not sure why this is here. Probably a mistake.
					Grid.SetZIndex(borderOverlayFieldHover,2);//in front of the others
				}
				//end of GridForField-----------------------------------------------------------------------------------------------------------------------
				Border borderBox=new Border();
				
				if(ListEFormFields[i].Border==EnumEFormBorder.None){
					borderBox.BorderThickness=new Thickness(left:1,0,0,0);//only left border thickness remains
					borderBox.BorderBrush=Brushes.White;
					borderBox.Margin=new Thickness(0);
					borderBox.Padding=new Thickness(left:_paddingLeft,top:0,right:0,bottom:0);//only left padding remains
				}
				else{
					borderBox.BorderThickness=new Thickness(1);
					borderBox.BorderBrush=new SolidColorBrush(ColorOD.Gray_Wpf(220));
					borderBox.Margin=new Thickness(left:0,top:0,right:0,bottom:5);//leaves room for the drop shadow
					borderBox.Padding=new Thickness(left:_paddingLeft,top:0,right:_paddingRight,bottom:3);//the contents are all slightly inset.
				}
				borderBox.Background=Brushes.White;//The drop shadow includes the contents of the border. This hides all of that.
				borderBox.CornerRadius=new CornerRadius(3);
				Grid.SetZIndex(borderBox,1);//in front of drop shadow
				//DropShadowEffect dropShadowEffect=new DropShadowEffect();
				//dropShadowEffect.ShadowDepth=3;
				//dropShadowEffect.BlurRadius=7;
				//dropShadowEffect.Direction=270;//straight down
				//dropShadowEffect.Color=Colors.Black;
				//dropShadowEffect.Opacity=0.2;
				//borderBox.Effect=dropShadowEffect;
				//This drop shadow was ugly, so I'm doing it further down with a border instead
				ListEFormFields[i].TagOD=borderBox;
				gridForField.Children.Add(borderBox);
				wrapPanel.Children.Add(gridForField);
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.CheckBox){
					AddCheckBox(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.DateField){
					AddDateField(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.Label){
					AddLabel(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.MedicationList){
					AddMedicationList(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak){
					AddPageBreak(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.RadioButtons){
					AddRadioButtons(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.SigBox){
					AddSigBox(borderBox,ListEFormFields[i]);
				}
				if(ListEFormFields[i].FieldType==EnumEFormFieldType.TextField){
					AddTextBox(borderBox,ListEFormFields[i]);
				}
				//Always add a margin to the right of each field
				//Yes, this even works for the last row because we want a border to right of the last row.
				Border borderRightOfField=CreateBorderForDrop();
				borderRightOfField.Width=_marginRightOfField;
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
				//Drop shadow------------------------------------------------------------------------------------------------------------------------------------------
				//This is complicated. I want a lot of control on how it fades off in the corners, so that will add a couple of circles.
				Ellipse ellipseLL=new Ellipse();
				RadialGradientBrush radialGradientBrush=new RadialGradientBrush(ColorOD.Gray_Wpf(200),Colors.White);//the first color is at the middle of the circle
				ellipseLL.Fill=radialGradientBrush;
				ellipseLL.Width=10;
				ellipseLL.Height=10;
				ellipseLL.HorizontalAlignment=HorizontalAlignment.Left;
				ellipseLL.VerticalAlignment=VerticalAlignment.Bottom;
				Ellipse ellipseLR=new Ellipse();  
				ellipseLR.Fill=radialGradientBrush;
				ellipseLR.Width=10;
				ellipseLR.Height=10;
				ellipseLR.HorizontalAlignment=HorizontalAlignment.Right;
				ellipseLR.VerticalAlignment=VerticalAlignment.Bottom;
				Border borderDropShadow=new Border();
				borderDropShadow.Height=10;
				//borderDropShadow.Width=20;
				borderDropShadow.VerticalAlignment=VerticalAlignment.Bottom;
				borderDropShadow.Margin=new Thickness(left:5,top:0,right:5,bottom:0);
				GradientStopCollection gradientStopCollection=new GradientStopCollection();
				gradientStopCollection.Add(new GradientStop(ColorOD.Gray_Wpf(190),0));
				gradientStopCollection.Add(new GradientStop(ColorOD.Gray_Wpf(190),0.5));
				gradientStopCollection.Add(new GradientStop(ColorOD.Gray_Wpf(225),0.75));
				gradientStopCollection.Add(new GradientStop(Colors.White,1));
				LinearGradientBrush linearGradientBrush=new LinearGradientBrush(gradientStopCollection,90);
				borderDropShadow.Background=//Brushes.Black;
					linearGradientBrush;
				//borderDropShadow.CornerRadius=new CornerRadius(topLeft:0,topRight:0,bottomRight:5,bottomLeft:5);
				Grid.SetZIndex(borderDropShadow,0);
				if(ListEFormFields[i].Border==EnumEFormBorder.ThreeD){
					gridForField.Children.Add(ellipseLL);
					gridForField.Children.Add(ellipseLR);
					gridForField.Children.Add(borderDropShadow);
				}
				//End of drop shadow-----------------------------------------------------------------------------------------------------------------------------------
				if(dragLocationRightOfField.IsRightOfWrap
					&& ListEFormFields[i].FieldType.In(
						EnumEFormFieldType.TextField,
						EnumEFormFieldType.DateField,
						EnumEFormFieldType.Label,
						EnumEFormFieldType.CheckBox
						))//Field types that are not horiz stackable do not need to be added to the list as we already know they are alone in their own horiz stack
				{
					ListIndicesLastInHorizStack.Add(i);
				}
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
						double spaceBelow;
						if(ListEFormFields[i].SpaceBelow==-1){
							spaceBelow=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
						}
						else{
							spaceBelow=ListEFormFields[i].SpaceBelow;
						}
						borderTopOfWrap.Height=spaceBelow;
						borderTopOfWrap.VerticalAlignment=VerticalAlignment.Bottom;
						borderTopOfWrap.Margin=new Thickness(left:_marginLeftOfPage,0,right:_marginRightOfField,0);
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

		///<summary>Sets visibility of each field for pages and conditional logic. Pass in pgRequested -1 to show all pages.</summary>
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
			if(pgRequested!=-1){//don't change pageShowing in this case
				_pageShowing=pgGranted;
			}
			if(pgRequested==-1){
				//don't do the things in the elses
			}
			else if(IsSetupMode) {//Show all pages in Setup Mode.
				label.Text=_pageShowing.ToString()+"/"+_pageCount.ToString();
			}
			else {//Only show visible pages when patient is filling out the eForm.
				_pageCountFiltered=_pageCount;
				SetIsHiddenConditFlags();
				for(int i=1;i<=_pageCount;i++) {
					List<EFormField> listEFormFieldsThisPage=ListEFormFields.FindAll(x=>x.Page==i && x.FieldType!=EnumEFormFieldType.PageBreak);
					if(listEFormFieldsThisPage.IsNullOrEmpty()) {
						continue;
					}
					if(listEFormFieldsThisPage.All(x=>x.IsHiddenCondit)) {
						_pageCountFiltered--;
					}
				}
				label.Text=_pageShowingFiltered.ToString()+"/"+_pageCountFiltered.ToString();
			}
			for(int i=0;i<ListEFormFields.Count;i++){
				//first, pages
				Border borderBox=(Border)ListEFormFields[i].TagOD;
				Grid gridForField=(Grid)borderBox.Parent;
				if(pgRequested==-1){
					gridForField.Visibility=Visibility.Visible;
				}
				else if(ListEFormFields[i].Page==_pageShowing){
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
				if(ListEFormFields[i].IsHiddenCondit){
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
				if(pgRequested==-1){
					uIElementCollection[i].Visibility=Visibility.Visible;
					continue;
				}
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

		private void GridMain_MouseMove(object sender,MouseEventArgs e) {
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
				Border borderBox=ListEFormFields[i].TagOD as Border;
				//test all 4 corners
				//Points are relative to entire control, not StackPanel
				Point pointFieldUL=PointFromScreen(borderBox.PointToScreen(new Point(0,0)));
				Point pointFieldBR=PointFromScreen(borderBox.PointToScreen(new Point(borderBox.ActualWidth,borderBox.ActualHeight)));
				//this rectangle won't be tangled
				Rect rectField=new Rect(pointFieldUL.X,pointFieldUL.Y,width:pointFieldBR.X-pointFieldUL.X,height:pointFieldBR.Y-pointFieldUL.Y);
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
				Border borderBox=ListEFormFields[i].TagOD as Border;
				int pageNum=ListEFormFields[i].Page;
				if(pageNum!=_pageShowing) {
					continue;
				}
				Point point=e.GetPosition(borderBox);
				Rect rectFieldBounds=new Rect(0,0,borderBox.ActualWidth,borderBox.ActualHeight);
				if(rectFieldBounds.Contains(point)){
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
				Border borderBox=ListEFormFields[_mouseDownIndex].TagOD as Border;
				Grid gridForPageBreak=borderBox.Child as Grid;
				WpfControls.UI.Button button=gridForPageBreak.Children[2] as WpfControls.UI.Button;
				if(button.Visible) {
					Point point=e.GetPosition(button);
					Rect rectBounds=new Rect(0,0,button.ActualWidth,button.ActualHeight);
					if(!rectBounds.Contains(point)) {
						return;
					}
					if(ListEFormFields[_mouseDownIndex].IsNew){//the field was added in this session previously
						//Immediately delete from db
						EFormFieldDefs.Delete(ListEFormFields[_mouseDownIndex].EFormFieldDefNum);//also deletes languagepats.
					}
					else{
						//Mark it for later deletion. Don't delete yet because user might cancel out of this form window.
						ListEFormFieldDefNumsToDelete.Add(ListEFormFields[_mouseDownIndex].EFormFieldDefNum);//these will get deleted when user clicks Save
					}
					//Whether new field or existing field, remove it from the list
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
			if(_listSelectedIndices.Count==1 && idxTo!=ListEFormFields.Count){
				//Again, only for single. If adding field to the end of a form, no need to alter destination (See Example3 for idxTo==ListEFormFields.Count scenario).
				//We alter destination here after removal, because otherwise we risk changing the field we are moving.
				//Example: move idx 4 to 3, up from single to end of row:
				//   loop will have removed field 4 from list.
				//   this will alter the new 4 which is the old 5
				//Example2: move idx 3 to 5, from end of row to middle of next row.
				//   loop will have removed field 3 from list.
				//   idxTo was 5, but is now 4
				//   this will alter the new 4, which is the old 5
				//Example3: There are 5 fields, indexes 0 through 4.
				//   move idx 3 to 5, from beginning of row to end of row 
				//   loop will have removed field 3 from list.
				//   idxTo was 5, but is now 4 which is still impossible because we only have indexes 0 through 3.
				ListEFormFields[idxTo].IsHorizStacking=dragLocation.IsHorizStackingNext;
			}
			//for Example3, we had 5 fields: 0 to 4. We removed one, so now we have 0,1,2,3. InsertRange(4... puts it after 3.
			ListEFormFields.InsertRange(idxTo,listEFormFields);
			_borderDropHover=null;
			int countSelected=_listSelectedIndices.Count;
			_listSelectedIndices.Clear();
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
							if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[_listSelectedIndices[0]].FieldType)){
								//The source field that we are trying to move does not allow stacking
								continue;
							}
							if(dragLocation.IsHorizStacking){
								//check that the field to the left allows stacking
								if(dragLocation.Idx!=0){//the destination is not 0 (probably already handled when setting IsHorizStacking
									if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[dragLocation.Idx-1].FieldType)){
										continue;//not allowed
									}
								}
							}
							if(dragLocation.IsHorizStackingNext){
								//check that the field to the right (idx) allows stacking
								if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[dragLocation.Idx].FieldType)){
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
				Border borderBox=ListEFormFields[i].TagOD as Border;
				Grid gridForField=borderBox.Parent as Grid;
				int pageNum=ListEFormFields[i].Page;
				if(pageNum!=_pageShowing) {
					continue;
				}
				Border borderOverlayFieldHover=gridForField.Children[0] as Border; 
				Point point=e.GetPosition(borderOverlayFieldHover);
				Rect rectBounds=new Rect(0,0,borderOverlayFieldHover.ActualWidth,borderOverlayFieldHover.ActualHeight);
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
							if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[_listSelectedIndices[0]].FieldType)){
								//The source field that we are trying to move does not allow stacking
								break;
							}
							if(dragLocation.IsHorizStacking){
								//check that the field to the left allows stacking
								if(dragLocation.Idx!=0 ){//the destination is not 0 (probably already handled when setting IsHorizStacking)
									if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[dragLocation.Idx-1].FieldType)){
										break;//not allowed
									}
								}
							}
							if(dragLocation.IsHorizStackingNext){
								//check that the field to the right (idx) allows stacking
								if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[dragLocation.Idx].FieldType)){
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
				SetIsHiddenConditFlags();
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
				SetIsHiddenConditFlags();
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
			if(!DesignerProperties.GetIsInDesignMode(this)){
				_marginBelowFields=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
			}
			if(!IsSetupMode){
				return;
			}
			Keyboard.Focus(stackPanel);
			stackPanel.MouseLeave+=StackPanel_MouseLeave;
			stackPanel.MouseLeftButtonDown+=StackPanel_MouseLeftButtonDown;
			stackPanel.MouseLeftButtonUp+=StackPanel_MouseLeftButtonUp;
			stackPanel.MouseMove+=StackPanel_MouseMove;
			stackPanel.KeyDown+=StackPanel_KeyDown;
			gridMain.MouseLeftButtonDown+=Grid_MouseLeftButtonDown;
			gridMain.MouseLeftButtonUp+=Grid_MouseLeftButtonUp;
			gridMain.MouseMove+=GridMain_MouseMove;
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
				for(int i=_listSelectedIndices.Count-1;i>=0;i--) {
					if(ListEFormFields[_listSelectedIndices[i]].IsNew){//the field was added in this session previously
						//Immediately delete from db
						EFormFieldDefs.Delete(ListEFormFields[_listSelectedIndices[i]].EFormFieldDefNum);//also deletes languagepats.
					}
					else{
						//Mark it for later deletion. Don't delete yet because user might cancel out of this form window.
						ListEFormFieldDefNumsToDelete.Add(ListEFormFields[_listSelectedIndices[i]].EFormFieldDefNum);//these will get deleted when user clicks Save
					}
					//Whether new field or existing field, remove it from the list
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
		private void AddCheckBox(Border borderBox,EFormField eFormField){
			CheckBox checkBox=new CheckBox();
			borderBox.Child=checkBox;
			checkBox.Width=EFormFields.CalcFieldWidth(eFormField,ListEFormFields,stackPanel.ActualWidth);
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			//WPF label is always to the right of the checkbox. We would use our UI.Checkbox if we want to give users a choice.
			checkBox.VerticalContentAlignment=VerticalAlignment.Center;
			checkBox.FontSize=FontSize*eFormField.FontScale/100;
			checkBox.IsChecked=eFormField.ValueString=="X";
			Label label=new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0);//default is 5
			label.Content=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,eFormField.ValueLabel);
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
				&& eFormField.ValueLabel!=""
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
			bool isConditionalChild=false;
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
				ClearSignatures();
			};
		}

		private void AddDateField(Border borderBox,EFormField eFormField){
			StackPanel stackPanel2=new StackPanel();
			borderBox.Child=stackPanel2;
			stackPanel2.Width=EFormFields.CalcFieldWidth(eFormField,ListEFormFields,stackPanel.ActualWidth);
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			stackPanelLabel.Margin=new Thickness(0,0,0,bottom:4);
			Label label = new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0);//default is 5
			label.Content=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,eFormField.ValueLabel);
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
				&& eFormField.ValueLabel!=""
				&& ListEFormFields.Exists(x=>x.ConditionalParent==eFormField.ValueLabel))
			{
				isConditionalParent=true;
			}
			if(isConditionalParent) {
				Label labelCondParent=new Label();
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
			textVDate.Height=20;
			if(eFormField.Border==EnumEFormBorder.ThreeD){
				textVDate.IsUnderline=true;//convert border to underline
			}
			textVDate.Text=eFormField.ValueString;
			textVDate.FontSize=FontSize*eFormField.FontScale/100;
			textVDate.TextChanged+=(sender,e)=>ClearSignatures();
			textVDate.LostFocus+=(sender,e)=>{
				FillFieldsFromControls(eFormField);
				SetVisibilities(_pageShowing,forceRefresh:true);
				//this is so that Age conditions will be tested
			};
			stackPanel2.Children.Add(textVDate);
		}

		private void TextVDate_LostFocus(object sender,RoutedEventArgs e) {
			throw new NotImplementedException();
		}

		private void AddLabel(Border borderBox,EFormField eFormField){
			WpfControls.UI.TextRich textRich=new WpfControls.UI.TextRich();
			textRich.SpellCheckIsEnabled=false;
			//One reason it's a textbox so that office/patient can edit during fill. This is a feature of sheets that we don't want to lose.
			//And this implementation works better than sheets because it doesn't jump when clicking into it.
			//The other reason it's a textbox is because that's how wpf shows flow documents.
			textRich.richTextBox.BorderThickness=new Thickness(0);
			borderBox.Child=textRich;
			textRich.Width=EFormFields.CalcFieldWidth(eFormField,ListEFormFields,stackPanel.ActualWidth);
			//textBlock.FontSize=FontSize*eFormField.FontScale/100;//no, this is never set for labels
			//textBlock.Padding=new Thickness(0);//default is 5
			string xmlString=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,eFormField.ValueLabel);
			FlowDocument flowDocument=EFormFields.DeserializeFlowDocument(xmlString);
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
			textRich.TextChanged+=(sender,e)=>ClearSignatures();
		}

		private void AddMedicationList(Border borderBox,EFormField eFormField) {
			//This could be done many different ways in different languages. We chose the following:
			//This is a vertical stackpanel of 3 items: title, a grid, and a footer with Add button and None checkbox
			//The grid has a header row as well as a row for each med.
			//To add and delete rows, we don't want to refresh layout because that would clear all the entered data.
			//So instead, we will dynamically alter the children and row defs of the grid.
			//For Add, grid gets another row at the end.
			//For Delete, the row objects get deleted, then following rows get moved up, and finally, grid loses row at end.
			StackPanel stackPanelVert=new StackPanel();
			borderBox.Child=stackPanelVert;
			double widthAvail=stackPanel.ActualWidth-_marginLeftOfPage-_marginRightOfField;
			widthAvail-=_thicknessLRBorders+_paddingLeft+_paddingRight;
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
				textBoxMed.TextChanged+=(sender,e)=>ClearSignatures();
				Grid.SetRow(textBoxMed,i+1);
				WpfControls.UI.TextBox textBoxFreq=new WpfControls.UI.TextBox();
				gridMeds.Children.Add(textBoxFreq);
				textBoxFreq.Width=widthCol2;
				textBoxFreq.Text=listEFormMeds[i].StrengthFreq;
				textBoxFreq.FontSize=fontSize;
				textBoxFreq.TextChanged+=(sender,e)=>ClearSignatures();
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

		private void AddPageBreak(Border borderBox,EFormField eFormField) {
			//This only needs to be shown for setup, not for patient UI.
			//This is just to give the user a place to click to remove or move a page break.
			if(!IsSetupMode) {//If filling out an eForm, don't draw the pagebreak field.
				return;
			}
			Grid gridForPageBreak=new Grid();
			borderBox.Child=gridForPageBreak;
			Border border = new Border();
			border.BorderBrush=new SolidColorBrush(ColorOD.Gray_Wpf(192));
			border.BorderThickness=new Thickness(1);
			border.Height=30;
			gridForPageBreak.Children.Add(border);//0
			Label label = new Label();
			gridForPageBreak.Children.Add(label);//1
			double widthAvail=stackPanel.ActualWidth-_marginLeftOfPage-_marginRightOfField;
			widthAvail-=_thicknessLRBorders+_paddingLeft+_paddingRight;
			if(widthAvail<0) {
				widthAvail=0;
			}
			label.Width=widthAvail;
			//this control is unique and will not have borders to left, right, or bottom for drag/drop.
			//so we set some margins here
			gridForPageBreak.Margin=new Thickness(0,0,right:_marginRightOfField,bottom:_marginBelowFields);
			label.Padding=new Thickness(4,0,0,3);
			label.Content="(page break)";
			label.VerticalAlignment=VerticalAlignment.Center;
			//add a button to delete
			WpfControls.UI.Button button = new WpfControls.UI.Button();
			gridForPageBreak.Children.Add(button);//2
			Grid.SetZIndex(button,3);//bring it in front of the hover border so it's clickable
			//button.Margin=new Thickness(15,0,0,0);
			button.Icon=WpfControls.UI.EnumIcons.DeleteX;
			button.Height=24;
			button.VerticalAlignment=VerticalAlignment.Center;
			button.HorizontalAlignment=HorizontalAlignment.Right;
			button.Visible=false;//Don't show the button until hovering over the field.
			button.Margin=new Thickness(0,0,right:2,0);
		}

		private void AddRadioButtons(Border borderBox,EFormField eFormField){
			int numRadioBtns=eFormField.PickListVis.Split(',').ToList().Count();
			string strLabels=eFormField.ValueLabel+","+eFormField.PickListVis;
			string strTranslations=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,strLabels);
			int numTranslations=strTranslations.Split(',').ToList().Count()-1;//subtract 1 from translations for the value label at idx 0.
			if(numTranslations!=numRadioBtns){
				LanguagePats.SyncRadioButtonTranslations(eFormField);//Ensures translations are in sync with PickListVis. Only syncs here if translations don't match up with radio buttons.
			}
			StackPanel stackPanelRadio=new StackPanel();
			//stackPanelRadio is our main stackPanel
			//It has two children: stackPanelLabel and wrapPanelRadio
			if(eFormField.LabelAlign==EnumEFormLabelAlign.TopLeft){
				stackPanelRadio.Orientation=Orientation.Vertical;
			}
			if(eFormField.LabelAlign==EnumEFormLabelAlign.LeftLeft
				|| eFormField.LabelAlign==EnumEFormLabelAlign.Right)
			{
				stackPanelRadio.Orientation=Orientation.Horizontal;
			}
			borderBox.Child=stackPanelRadio;
			double widthAvail=stackPanel.ActualWidth-_marginLeftOfPage-_marginRightOfField;
			widthAvail-=_thicknessLRBorders+_paddingLeft+_paddingRight;
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
			strTranslations=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,strLabels);//translate again in case language translations needed to sync.
			List<string> listTranslations=strTranslations.Split(',').ToList();//Ex: [label,button1,button2]
			label.Content=listTranslations[0];
			if(eFormField.LabelAlign==EnumEFormLabelAlign.LeftLeft){
				stackPanelLabel.Margin=new Thickness(0,0,right:10,0);
			}
			else if(eFormField.LabelAlign==EnumEFormLabelAlign.TopLeft){
				stackPanelLabel.Margin=new Thickness(0,0,0,bottom:5);
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
				&& eFormField.ValueLabel!=""
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
			List<string> listPickLang=new List<string>();
			for(int i=1;i<listTranslations.Count;i++){//skip index 0, that's the translated eFormField.ValueLabel.
				listPickLang.Add(listTranslations[i]);
			}
			List<string> listPickVis=eFormField.PickListVis.Split(',').ToList();
			List<string> listPickDb=eFormField.PickListDb.Split(',').ToList();
			for(int i=0;i<listPickVis.Count;i++){
				//I decided to use the OD radiobutton because I have more control over the layout and behavior
				WpfControls.UI.RadioButton radioButton=new WpfControls.UI.RadioButton();
				radioButton.HorizontalAlignment=HorizontalAlignment.Stretch;
				radioButton.VerticalAlignment=VerticalAlignment.Stretch;
				radioButton.textBlock.Margin=new Thickness(left:1,0,0,0);//instead of 4 to make it closer
				radioButton.Margin=new Thickness(0,0,right:_marginRightOfRadioButton,0);
				radioButton.Text=listPickLang[i];
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
					ClearSignatures();
				};
				wrapPanelRadio.Children.Add(radioButton);
			}
			if(eFormField.LabelAlign==EnumEFormLabelAlign.TopLeft
				|| eFormField.LabelAlign==EnumEFormLabelAlign.LeftLeft)
			{
				stackPanelRadio.Children.Add(stackPanelLabel);
				stackPanelRadio.Children.Add(wrapPanelRadio);
			}
			if(eFormField.LabelAlign==EnumEFormLabelAlign.Right){
				stackPanelRadio.Children.Add(wrapPanelRadio);
				stackPanelRadio.Children.Add(stackPanelLabel);
			}
		}

		private void AddSigBox(Border borderBox,EFormField eFormField){
			StackPanel stackPanel2=new StackPanel();
			borderBox.Child=stackPanel2;
			double widthAvail=stackPanel.ActualWidth-_marginLeftOfPage-_marginRightOfField;
			widthAvail-=_thicknessLRBorders+_paddingLeft+_paddingRight;
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
			label.Content=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,eFormField.ValueLabel);
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
			if(IsSetupMode){
				Rectangle rectangle=new Rectangle();
				rectangle.HorizontalAlignment=HorizontalAlignment.Left;
				rectangle.Width=362;
				rectangle.Height=79;
				rectangle.Stroke=Brushes.Silver;
				rectangle.StrokeThickness=1;
				stackPanel2.Children.Add(rectangle);
			}
			else{
				WpfControls.UI.SignatureBoxWrapper signatureBoxWrapper=new WpfControls.UI.SignatureBoxWrapper();
				signatureBoxWrapper.Width=362;
				signatureBoxWrapper.Height=79;
				string keyData=EForms.GetSignatureKeyData(ListEFormFields);
				bool isSigTopaz=false;
				string signature=eFormField.ValueString;
				if(signature.Length>0){
					if(signature.Substring(0,1)=="1"){
						isSigTopaz=true;
					}
					signature=signature.Substring(1);
				}
				signatureBoxWrapper.FillSignature(isSigTopaz,keyData,signature);
				stackPanel2.Children.Add(signatureBoxWrapper);
			}			
		}

		private void AddTextBox(Border borderBox,EFormField eFormField){
			StackPanel stackPanel2=new StackPanel();
			borderBox.Child=stackPanel2;
			stackPanel2.Width=EFormFields.CalcFieldWidth(eFormField,ListEFormFields,stackPanel.ActualWidth);
			StackPanel stackPanelLabel=new StackPanel();
			stackPanelLabel.Orientation=Orientation.Horizontal;
			stackPanelLabel.Margin=new Thickness(0,0,0,bottom:4);
			Label label = new Label();
			label.FontSize=FontSize*eFormField.FontScale/100;
			label.Padding=new Thickness(0);//default is 5
			label.Content=LanguagePats.TranslateEFormField(eFormField.EFormFieldDefNum,LanguageShowing,eFormField.ValueLabel);
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
			WpfControls.UI.TextBox textBox=new WpfControls.UI.TextBox();
			textBox.HorizontalAlignment=HorizontalAlignment.Stretch;
			if(eFormField.IsTextWrap){
				textBox.IsMultiline=true;
				//keep the full outline on 4 sides
			}
			else{
				textBox.Height=20;
				if(eFormField.Border==EnumEFormBorder.ThreeD){
					//single row textbox with border gets converted to underline
					textBox.IsUnderline=true;
				}
			}
			textBox.Text=eFormField.ValueString;
			textBox.FontSize=FontSize*eFormField.FontScale/100;
			textBox.TextChanged+=(sender,e)=>ClearSignatures();
			stackPanel2.Children.Add(textBox);
		}

		///<summary>Gets called whenever the patient changes a field. Clears existing signatures unless they just signed it in this session.</summary>
		private void ClearSignatures() {
			for(int i=0;i<ListEFormFields.Count;i++){
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.SigBox){
					continue;
				}
				Border borderBox=ListEFormFields[i].TagOD as Border;
				StackPanel stackPanel=borderBox.Child as StackPanel;
				WpfControls.UI.SignatureBoxWrapper signatureBoxWrapper=stackPanel.Children[1] as WpfControls.UI.SignatureBoxWrapper;
				if(signatureBoxWrapper.GetSigChanged()){
					//if the user already signed during this session, then they don't want to clear it again.
				}
				else{
					signatureBoxWrapper.ClearSignature();
				}
			}
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

		///<summary>Loops through all fields and fixes invalid stacking. Used after a multi drag drop, when first loading to fix any prior db issues, and after editing each field because changing stacking in one field can affect stacking in other fields. Doesn't change orders of any fields.</summary>
		private void FixAllStacking(){
			for(int i=0;i<ListEFormFields.Count;i++){
				if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[i].FieldType)){
					//the simple obvious fix for self
					ListEFormFields[i].IsHorizStacking=false;
				}
			}
			//Now that all selves are fixed, another loop will check field to left of each
			for(int i=1;i<ListEFormFields.Count;i++){
				//if field to left can't stack, neither can this one.
				if(!EFormFieldDefs.IsHorizStackableType(ListEFormFields[i-1].FieldType)){
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

		///<summary>Sets background colors for all items, based on selected indices and hover. Also, sets visibility of Delete button on pagebreak field.</summary>
		private void SetColors(){
			for(int i=0;i<ListEFormFields.Count;i++){
				Border borderBox=ListEFormFields[i].TagOD as Border;
				Grid gridForField=borderBox.Parent as Grid;
				Border borderOverlayFieldHover=gridForField.Children[0] as Border;
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
				borderOverlayFieldHover.Background=new SolidColorBrush(colorBack);
				borderOverlayFieldHover.BorderBrush=new SolidColorBrush(colorBorder);
			}
			//pagebreak Delete button visibility
			for(int i = 0;i<ListEFormFields.Count;i++) {
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.PageBreak) {
					continue;
				}
				Border borderBox=ListEFormFields[i].TagOD as Border;
				Grid gridForPageBreak=borderBox.Child as Grid;
				WpfControls.UI.Button button = gridForPageBreak.Children[2] as WpfControls.UI.Button;
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

		///<summary>This method will set a flag to hide controls based on conditional logic. This does not get set when hiding pages, but if all controls on a page are hidden then the page will also be hidden. This only gets called when filling out a form. We don't want to hide controls when they are in setup mode.</summary>
		private void SetIsHiddenConditFlags() {
			for(int i=0;i<ListEFormFields.Count;i++) {
				ListEFormFields[i].IsHiddenCondit=false;
				if(ListEFormFields[i].ConditionalParent=="") {
					continue;
				}
				EFormField eFormFieldParent=ListEFormFields.Find(x=>
					x.ValueLabel!=""
					&& x.ValueLabel.Substring(0,Math.Min(x.ValueLabel.Length,255))==ListEFormFields[i].ConditionalParent
					&& x.FieldType.In(EnumEFormFieldType.CheckBox,EnumEFormFieldType.RadioButtons,EnumEFormFieldType.DateField));
				if(eFormFieldParent is null) {
					continue;//they might have set the ConditionalParent string wrong
				}
				bool isConditionMet=false;
				if(eFormFieldParent.FieldType==EnumEFormFieldType.RadioButtons){
					string valParent=EFormFields.GetValParent(eFormFieldParent);
					if(valParent==ListEFormFields[i].ConditionalValue){
						isConditionMet=true;
					}
				}
				if(eFormFieldParent.FieldType==EnumEFormFieldType.CheckBox 
					&& eFormFieldParent.ValueString==ListEFormFields[i].ConditionalValue)
				{
					isConditionMet=true;
				}
				if(eFormFieldParent.FieldType==EnumEFormFieldType.DateField){
					//We didn't bother to check the name of the field even though it was really only designed for Birthdate
					isConditionMet=true;//this is our fallback if anything below goes wrong. We do NOT want it to fallback to false.
					DateTime dateBirth=DateTime.MinValue;
					try{
						dateBirth=DateTime.Parse(eFormFieldParent.ValueString);
					}
					catch{}
					if(dateBirth!=DateTime.MinValue){
						int agePatient=Patients.DateToAge(dateBirth);
						int ageCondition=-1;
						if(ListEFormFields[i].ConditionalValue.StartsWith("<") || ListEFormFields[i].ConditionalValue.StartsWith(">")){
							try{
								ageCondition=int.Parse(ListEFormFields[i].ConditionalValue.Substring(1));
							}
							catch{ }
						}
						if(ageCondition!=-1){
							if(ListEFormFields[i].ConditionalValue.StartsWith("<")){
								if(agePatient>=ageCondition){//flipped because we're setting to false
									isConditionMet=false;
								}
							}
							if(ListEFormFields[i].ConditionalValue.StartsWith(">")){
								if(agePatient<=ageCondition){
									isConditionMet=false;
								}
							}
						}
					}
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
