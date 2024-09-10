using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;
using OpenDental.Drawing;

namespace OpenDental {
/*
Jordan is the only one allowed to edit this file.

This is fairly easy to use when you have one simple input, like a textbox.
For more complex situations, you will frequently need to look at the code because it's not very well documented.
Examples are in UnitTests\UI Testing\FormInputBoxTests.cs

Window size is based on how much space the controls take.
For a single textbox, it would be 
width=8(left window border)+32(left margin)+300(widthBigControls)+40(right margin)+8(right window border). Total width = 388
height=26(top window title)+20(top margin)+18(label)+2(space below label)+20(textbox)+60(bottom margin)+8(bottom window border). Total height=154
The above numbers are all in 96 dpi pixels. 
If Windows monitor is set to 150%, for example, then all of the above numbers are multiplied by 1.5.
But the programmer does not need to worry about that.

Some comments about control layout:
They are all anchored UL since any other anchoring gets too complex.
No control size is based on any other control size. 
Controls and their labels are stacked vertically, and then the window size is adjusted to fit everything.
Some labels are bigger because they can wrap to multiple lines.

Someday, I will eliminate all the ctor overloads.
*/

	/// <summary>A quick entry form for various purposes. You can put several different types of controls on this form.</summary>
	public partial class InputBox : FrmODBase {
		public bool IsDeleteClicked;
		private List<InputBoxParam> _listInputBoxParams=new List<InputBoxParam>();
		public List<System.Windows.Controls.UserControl> ListControls;
		///<summary>This is a separate list from ListControls because users won't need to later query this list. It is not 1:1 with ListControls because some controls lack a label and some controls have an extra label to their right.</summary>
		private List<Label> _listLabels;
		///<summary>Only used in one spot. If true, this window will close in 15 seconds if no user input.</summary>
		public bool HasTimeout=false;
		///<summary>If true, this window has had 15 seconds without user clicking OK or X, so close the window.</summary>
		public bool HasTimedOut=false;
		public Func<string,bool> FuncOkClick;
		private DispatcherTimer _dispatcherTimer;
		///<summary>These are the margins for the entire window, not counting the OK and Delete buttons.</summary>
		private Thickness _thicknessMargins=new Thickness(left:32,top:20,right:40,bottom:60);
		///<summary>This is the standard width of textboxes, comboBoxes, etc. It's also the max width for labels, which are sized according to their content, but will wrap instead of getting wider than this.</summary>
		private double _widthBigControls=300;
		///<summary>As controls are added, this keeps track of the max. We will use this to size our window later.</summary>
		private float _widthAllControls;
		///<summary>As controls are laid out, this is the vertical position. After they are laid out, this tells us how big to make the window.</summary>
		private float _yPos;

		#region Constructors
		///<summary></summary>
		public InputBox(){	
			InitializeComponent();
			_dispatcherTimer=new DispatcherTimer();
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(15000);
			_dispatcherTimer.Tick+=Timer_Tick;
			KeyDown+=Frm_KeyDown;
			Load+=InputBox_Load;
			Shown+=InputBox_Shown;
			PreviewKeyDown+=InputBox_PreviewKeyDown;
			butDelete.Visible=false;
			labelPrompt.Visible=false;
		}

		///<summary>Creates a textbox with a label containing the given prompt. Optional default text to go in the textbox.</summary>
		public InputBox(string prompt,string defaultText="") : this() {	
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.TextBox;
			inputBoxParam.LabelText=prompt; 
			inputBoxParam.Text=defaultText;
			_listInputBoxParams.Add(inputBoxParam);
		}

		///<summary>Creates a comboBox (not multi). Send in a list of strings for user to select from.</summary>
		public InputBox(string prompt,List<string> listSelections,int selectedIndex=-1) : this(){
			//always comboselect
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.ComboSelect;
			inputBoxParam.LabelText=prompt; 
			inputBoxParam.ListSelections=listSelections;
			if(selectedIndex!=-1){
				inputBoxParam.ListIndicesSelected=new List<int>{selectedIndex};
			}
			_listInputBoxParams.Add(inputBoxParam);
		}

		///<summary>Use this constructor to create multiple input controls. </summary>
		public InputBox(List<InputBoxParam> listInputBoxParams) : this(){
			_listInputBoxParams=listInputBoxParams;
		}

		public InputBox(params InputBoxParam[] arrayInputBoxParams) :this(){
			_listInputBoxParams=arrayInputBoxParams.ToList();
		}
		#endregion Constructors

		#region Properties
		///<summary>For checkboxes, returns true if the checkbox is checked.</summary>
		public bool BoolResult{
			get{
				CheckBox checkBox=(CheckBox)ListControls.Find(x => x is CheckBox);
				if(checkBox is null){
					return false;
				}
				return checkBox.Checked==true;
			}
		}

		public DateTime DateResult {
			get {
				return PIn.Date(((TextVDate)ListControls.Find(x => x is TextVDate)).Text);
			}
		}

		[Browsable(false)]
		public bool ShowDelete {
			get {
				return butDelete.Visible;
			}
			set {
				butDelete.Visible=value;
			}
		}

		public string RadioButtonResult{
			get{
				List<RadioButton> listRadioButtons=ListControls.OfType<RadioButton>().ToList();
				if(listRadioButtons.Count>0){
					RadioButton radioButton=listRadioButtons.Find(x=>x.Checked==true);
					return radioButton.Text;
				}
				return "";
			}
		}

		public List<int> SelectedIndices {
			get {
				System.Windows.Controls.UserControl control = ListControls.Find(x => x is ComboBox || x is ListBox);
				if(control==null) {
					return new List<int>();
				}
				if(control is ComboBox) {
					return ((ComboBox)control).SelectedIndices;
				}
				if(control is ListBox) {
					return ((ListBox)control).SelectedIndices;
				}
				return new List<int>();
			}
		}

		public int SelectedIndex {
			get {
				if(SelectedIndices.Count<1) {
					return -1;
				}
				return SelectedIndices[0];
			}
		}

		public string StringResult{
			get{
				TextBox textBox=(TextBox)ListControls.Find(x => x.GetType()==typeof(TextBox));
				if(textBox!=null){
					return textBox.Text;
				}
				TextVDate textVDate=(TextVDate)ListControls.Find(x=>x.GetType()==typeof(TextVDate));
				if(textVDate!=null){
					return textVDate.Text;
				}
				TextVDouble textVDouble=(TextVDouble)ListControls.Find(x=>x.GetType()==typeof(TextVDouble));
				if(textVDouble!=null){
					return textVDouble.Text;
				}
				TextVTime textVTime=(TextVTime)ListControls.Find(x=>x.GetType()==typeof(TextVTime));
				if(textVTime!=null){
					return textVTime.Text;
				}
				TextPassword textPassword=(TextPassword)ListControls.Find(x=>x.GetType()==typeof(TextPassword));
				if(textPassword!=null){
					return textPassword.Text;
				}
				return "";
			}
		}

		///<summary>Will return a TimeSpan of zero if user did not enter anything.</summary>
		public TimeSpan TimeSpanResult {
			get {
				DateTime dateTime=PIn.DateT(((TextVTime)ListControls.Find(x => x is TextVTime)).Text);
				return dateTime.TimeOfDay;
			}
		}
		#endregion Properties

		#region Methods - Event Handlers
		private void InputBox_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleteClicked=true;
			IsDialogOK=true;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(ListControls.Where(x => x is TextVDate).Any(x => !((TextVDate)x).IsValid())
				|| ListControls.Where(x => x is TextVTime).Any(x => !((TextVTime)x).IsValid())) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(ListControls.OfType<ComboBox>().Any(x=>!x.IsMultiSelect && x.SelectedIndex==-1)) {//single selection
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(ListControls.OfType<ComboBox>().Any(x=>x.IsMultiSelect && x.SelectedIndices.Count==0)) {//multi selection
				MsgBox.Show(this,"Please make at least one selection.");
				return;
			}
			if(ListControls.OfType<CheckBox>().ToList().Count>1 && ListControls.OfType<CheckBox>().Where(x => x.Checked==true).Count()==0) {
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(ListControls.OfType<CheckBox>().ToList().Count>1 && ListControls.OfType<CheckBox>().Where(x => x.Checked==true).Count()>1) {
				MsgBox.Show(this,"Can only make one selection.");
				return;
			}
			if(ListControls.OfType<RadioButton>().ToList().Count>1 && ListControls.OfType<RadioButton>().Where(x => x.Checked).Count()==0) {
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(FuncOkClick!=null && !FuncOkClick(StringResult)) {
				//It is up to the implementor for _onOkClick to handle any messages to portray to the user.
				//We will simply block the user from closing the window.
				return;
			}
			IsDialogOK=true;
		}

		private void InputBox_Load(object sender, EventArgs e){
			Lang.F(this);
			AddInputControls();
			_formFrame.Size=new System.Drawing.Size(
				(int)ScaleF(8+(float)_thicknessMargins.Left+_widthAllControls+(float)_thicknessMargins.Right+8),
				(int)ScaleF(26+_yPos+(float)_thicknessMargins.Bottom+8));
			//if(SizeInitial.Width!=0 && SizeInitial.Height!=0){
			//	_formFrame.Size=new System.Drawing.Size((int)Math.Round(SizeInitial.Width*ScaleF(1)),(int)Math.Round(SizeInitial.Height*ScaleF(1)));
			//}
			for(int i=0;i<_listLabels.Count;i++)	{
				grid.Children.Add(_listLabels[i]);
			}
			bool isFocusSet=false;
			for(int i=0;i<ListControls.Count;i++) {
				grid.Children.Add(ListControls[i]);
				if(isFocusSet){
					continue;
				}
				if(!ListControls[i].Name.Contains("text")) {
					//There are about 6 different kinds of textboxes. 
					//This feels a little hackey, but we control the names, so it will work.
					continue;
				}
				int indexFirstTextbox=grid.Children.Count-1;
				Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() =>{
					grid.Children[indexFirstTextbox].Focus();
				}));
				isFocusSet=true;
			}

		}

		private void InputBox_Shown(object sender,EventArgs e) {
			if(_dispatcherTimer!=null && HasTimeout) {
				_dispatcherTimer.IsEnabled=true;
			}
		}

		private void Timer_Tick(object sender,EventArgs e) {
			_dispatcherTimer.IsEnabled=false;
			IsDialogCancel=true;
			HasTimedOut=true;
		}
		#endregion Methods - Event Handlers

		#region Methods
		///<summary>Adds all controls to a list. The controls will later be added to the form. The y positions ignore any height restriction, as the form height will be changed later to match.</summary>
		private void AddInputControls() {
			ListControls=new List<System.Windows.Controls.UserControl>();
			_listLabels=new List<Label>();
			_yPos=(float)_thicknessMargins.Top;
			double xPos=_thicknessMargins.Left;
			_widthAllControls=0;
			for(int i=0;i<_listInputBoxParams.Count;i++) {
				if(_listInputBoxParams[i]==null) {
					continue;
				}
				int itemOrder=i+1;
				if(!string.IsNullOrEmpty(_listInputBoxParams[i].LabelText)) {
					if(i>0) {
						_yPos+=10;
					}
					Label label=new Label();
					label.Text=_listInputBoxParams[i].LabelText;
					Size size=GetTextSize(label.Text+"some fluff",_widthBigControls);
					label.Width=size.Width;
					label.Height=size.Height;
					label.Name="labelPrompt"+itemOrder;
					label.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
					label.Tag=_listInputBoxParams[i];
					_listLabels.Add(label);
					_yPos+=(int)label.Height+2;
				}
				System.Windows.Controls.UserControl control;
				switch(_listInputBoxParams[i].InputBoxType_) {
					case InputBoxType.TextBox:
						if(_listInputBoxParams[i].IsPassswordCharStar){
							TextPassword textPassword=new TextPassword();
							textPassword.Name="textBox"+itemOrder;
							textPassword.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
							textPassword.Width=_widthBigControls;
							textPassword.Height=20;
							textPassword.Text=_listInputBoxParams[i].Text;
							control=textPassword;
							break;
						}
						TextBox textBox=new TextBox();
						textBox.Name="textBox"+itemOrder;
						textBox.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						textBox.Width=_widthBigControls;
						textBox.Height=20;
						textBox.Text=_listInputBoxParams[i].Text;
						if(!String.IsNullOrEmpty(textBox.Text)) {
							textBox.SelectionStart=0;
							textBox.SelectionLength=textBox.Text.Length;
						}
						if(_listInputBoxParams[i].MaxLengthText!=0){
							textBox.MaxLength=_listInputBoxParams[i].MaxLengthText;
						}
						control=textBox;
						break;
					case InputBoxType.TextBoxMultiLine:
						TextBox textBoxMulti=new TextBox();
						textBoxMulti.Name="textBox"+itemOrder;
						textBoxMulti.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						textBoxMulti.Width=_widthBigControls;
						textBoxMulti.Height=100;
						textBoxMulti.IsMultiline=true;
						textBoxMulti.Text=_listInputBoxParams[i].Text;
						KeyDown-=Frm_KeyDown; //Remove event set in constructor.
						//textBoxMulti.ScrollBars=ScrollBars.Vertical;
						control=textBoxMulti;
						break;
					case InputBoxType.CheckBox:
						CheckBox checkBox=new CheckBox();
						checkBox.Name="checkBox"+itemOrder;
						Point point=new Point(xPos+_listInputBoxParams[i].PointPosition.X,_yPos+_listInputBoxParams[i].PointPosition.Y);
						checkBox.Margin=new Thickness(left:point.X,top:point.Y,right:0,bottom:0);
						checkBox.Width=_widthBigControls;//We might be want to improve this
						checkBox.Height=20;
						if(_listInputBoxParams[i].SizeParam.Width!=0 && _listInputBoxParams[i].SizeParam.Height!=0){
							checkBox.Width=_listInputBoxParams[i].SizeParam.Width;
							checkBox.Height=_listInputBoxParams[i].SizeParam.Height;
						}
						checkBox.Text=_listInputBoxParams[i].Text;
						checkBox.IsEnabled=_listInputBoxParams[i].Enabled;
						control=checkBox;
						break;
					case InputBoxType.ComboSelect:
						ComboBox comboBox=new ComboBox();
						comboBox.Name="comboBox"+itemOrder;
						comboBox.Width=_widthBigControls;
						comboBox.Height=20;
						if(_listInputBoxParams[i].SizeParam.Width!=0 && _listInputBoxParams[i].SizeParam.Height!=0){
							comboBox.Width=_listInputBoxParams[i].SizeParam.Width;
							comboBox.Height=_listInputBoxParams[i].SizeParam.Height;
						}
						comboBox.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						comboBox.Items.AddList<string>(_listInputBoxParams[i].ListSelections,x=>x);
						if(_listInputBoxParams[i].ListIndicesSelected.Count>0 && _listInputBoxParams[i].ListIndicesSelected[0].Between(0,comboBox.Items.Count-1)) {
							comboBox.SetSelected(_listInputBoxParams[i].ListIndicesSelected[0]);//If there is a valid initial selection, select it.
						}
						if(_listInputBoxParams[i].SelectedIndex!=-1) {
							comboBox.SetSelected(_listInputBoxParams[i].SelectedIndex);
						}
						comboBox.IsEnabled=_listInputBoxParams[i].Enabled;
						control=comboBox;
						break;
					case InputBoxType.ComboMultiSelect:
						ComboBox comboBox2=new ComboBox();
						comboBox2.IsMultiSelect=true;
						comboBox2.Name="comboBox"+itemOrder;
						comboBox2.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						comboBox2.Width=_widthBigControls;
						comboBox2.Height=21;
						for(int j=0;j<_listInputBoxParams[i].ListSelections.Count;j++) {
							comboBox2.Items.Add(_listInputBoxParams[i].ListSelections[j]);
						}
						for(int j = 0;j<_listInputBoxParams[i].ListIndicesSelected.Count;j++) {
							if(_listInputBoxParams[i].ListIndicesSelected[j].Between(0,comboBox2.Items.Count-1)) {
								comboBox2.SetSelected(_listInputBoxParams[i].ListIndicesSelected[j]);//If there is a valid initial selection, select it.
							}
						}
						control=comboBox2;
						break;
					case InputBoxType.ValidDate:
						TextVDate textVDate=new TextVDate();
						textVDate.Name="textVDate"+itemOrder;
						textVDate.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						textVDate.Width=100;
						textVDate.Height=20;
						textVDate.Text=_listInputBoxParams[i].Text;
						control=textVDate;
						Label labelRight=new Label();//This is a second label to the right of textbox
						labelRight.Height=textVDate.Height;
						labelRight.Width=textVDate.Width;
						labelRight.Text=$"({CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern})"; //Gets the cultural format for date
						labelRight.Name="labelDateFormat"+itemOrder;
						Point pointLabel=new Point(xPos+textVDate.Width+12,_yPos);
						labelRight.Margin=new Thickness(left:pointLabel.X,top:pointLabel.Y,right:0,bottom:0);
						labelRight.Tag=_listInputBoxParams[i];
						_listLabels.Add(labelRight);
						break;
					case InputBoxType.ValidTime:
						TextVTime textVTime=new TextVTime();
						textVTime.Name="textVTime"+itemOrder;
						textVTime.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						textVTime.Width=120;
						textVTime.Height=20;
						control=textVTime;
						break;
					case InputBoxType.ValidDouble:
						TextVDouble textVDouble=new TextVDouble();
						textVDouble.Name="textVDouble"+itemOrder;
						textVDouble.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						textVDouble.Width=120;
						textVDouble.Height=20;
						control=textVDouble;
						break;
					case InputBoxType.ValidPhone:
						TextBox textPhone=new TextBox();
						textPhone.Name="textPhone"+itemOrder;
						textPhone.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						textPhone.Width=140;
						textPhone.Height=20;
						textPhone.Text=_listInputBoxParams[i].Text;
						if(!String.IsNullOrEmpty(textPhone.Text)) {
							textPhone.SelectionStart=0;
							textPhone.SelectionLength=textPhone.Text.Length;
						}
						control=textPhone;
						break;
					case InputBoxType.ListBoxMulti:
						ListBox listBox=new ListBox();
						listBox.Name="listBox"+itemOrder;
						listBox.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						listBox.Background=SystemColors.WindowBrush;
						listBox.SelectionMode=SelectionMode.MultiExtended;
						listBox.Items.AddList(_listInputBoxParams[i].ListSelections,x => x.ToString());
						listBox.Width=_widthBigControls;
						listBox.Height=95; //Default height used in original Form
						control=listBox;
						break;
					case InputBoxType.RadioButton:
						RadioButton radioButton=new RadioButton();
						radioButton.Name="radioButton"+itemOrder;
						radioButton.Margin=new Thickness(left:xPos,top:_yPos,right:0,bottom:0);
						radioButton.Width=_widthBigControls;
						radioButton.Height=20;
						radioButton.Text=_listInputBoxParams[i].Text;
						control=radioButton;
						break;
					default:
						throw new NotImplementedException("InputBoxType: "+_listInputBoxParams[i].InputBoxType_+" not implemented.");
				}
				if(_listInputBoxParams[i].InputBoxType_==InputBoxType.ValidDate){
					//this is special because of the label to the right.
					if(200>_widthAllControls){
						_widthAllControls=212;
					}
				}
				else if(control.Width>_widthAllControls){
					_widthAllControls=(float)control.Width;
				}
				control.TabIndex=i;
				control.Tag=_listInputBoxParams[i];
				ListControls.Add(control);
				//control.Width could be 385 form many controls
				//It could be a bit less for others. For example, date is only 100 (although it also has a label to its right)
				//Also, it could be bigger if SizeParam was set.
				//So by setting width no less than 250, and assuming an 80 pix L/R border?, we are leaving 170 as the min width avail for controls.
				//_minWidth=(int)Math.Max(_minWidth,control.Width+80);
				_yPos+=(float)control.Height+2;
			}
		}

		public void SetTitle(string title) {
			this.Text=title;
		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butOK_Click(this,new EventArgs());
			}
		}

		private Size GetTextSize(string textToMeasure,double widthMax){
			Font font=Font.ForWpf();
			Graphics g=Graphics.MeasureBegin();
			return g.MeasureString(textToMeasure,font,widthMax);
		}

		#endregion Methods
	}

	public class InputBoxParam {
		///<summary>Lets you set a control to not be enabled. Only supported for checkBoxes and comboBoxes (single) so far.</summary>
		public bool Enabled=true;
		public InputBoxType InputBoxType_;
		///<summary>Only works for textbox. Setting to true will set PasswordChar to '*'.</summary>
		public bool IsPassswordCharStar;
		///<summary>Only for ComboSelect and ComboMultiSelect. Also OK to use SelectedIndex for ComboSelect.</summary>
		public List<int> ListIndicesSelected=new List<int>();
		public List<string> ListSelections=new List<string>();
		///<summary>This is the text on the label that's above the input item. Typically not used with a checkbox which has its text set with Text.</summary>
		public string LabelText;
		///<summary>Only used in one place for textbox.</summary>
		public int MaxLengthText;
		///<summary>Only applies to checkbox. Shifts those controls by a set amount from the original location.</summary>
		public Point PointPosition;
		///<summary>Only for ComboSelect (not multi). Also OK to use a single entry in ListIndicesSelected for the same purpose.</summary>
		public int SelectedIndex=-1;
		///<summary>Normally Size.Empty, which is same as 0,0. Allows override of size of checkbox or combobox.</summary>
		public Size SizeParam;
		///<summary>This is the text on a checkbox or radiobutton. It can also be used to prefill a textbox, textBoxMultiLine, validDate, or validPhone.</summary>
		public string Text="";
	}

	public enum InputBoxType {
		TextBox,
		///<summary>Height of the textbox is 100</summary>
		TextBoxMultiLine,
		ComboSelect,
		ComboMultiSelect,
		ValidDate,
		ValidTime,
		ValidDouble,
		ValidPhone,
		CheckBox,
		ListBoxMulti,
		RadioButton,
	}
	
}





















