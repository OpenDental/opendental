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
	/// <summary>A quick entry form for various purposes. You can put several different types of controls on this form.</summary>
	public partial class InputBox : FrmODBase {
		public bool IsDeleteClicked;
		private List<InputBoxParam> _listInputBoxParams=new List<InputBoxParam>();
		private List<System.Windows.Controls.UserControl> _listControls;
		private List<Label> _listLabels;
		///<summary>Only used in one spot. If true, this window will close in 15 seconds if no user input.</summary>
		public bool HasTimeout=false;
		public Func<string,bool> FuncOkClick;
		int _curLocationY=2;
		int _minWidth=250;
		///<summary>Must use this to set the initial size rather than Size directly, because of layout manager.  Pass in a size in 96 dpi. It will get scaled.</summary>
		public Size SizeInitial;
		private DispatcherTimer _dispatcherTimer;

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
				CheckBox checkBox=(CheckBox)_listControls.Find(x => x is CheckBox);
				if(checkBox is null){
					return false;
				}
				return checkBox.Checked==true;
			}
		}

		public DateTime DateResult {
			get {
				return PIn.Date(((TextVDate)_listControls.Find(x => x is TextVDate)).Text);
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
				List<RadioButton> listRadioButtons=_listControls.OfType<RadioButton>().ToList();
				if(listRadioButtons.Count>0){
					RadioButton radioButton=listRadioButtons.Find(x=>x.Checked==true);
					return radioButton.Text;
				}
				return "";
			}
		}

		public List<int> SelectedIndices {
			get {
				System.Windows.Controls.UserControl control = _listControls.Find(x => x is ComboBox || x is ListBox);
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
				TextBox textBox=(TextBox)_listControls.Find(x => x.GetType()==typeof(TextBox));
				if(textBox!=null){
					return textBox.Text;
				}
				TextVDate textVDate=(TextVDate)_listControls.Find(x=>x.GetType()==typeof(TextVDate));
				if(textVDate!=null){
					return textVDate.Text;
				}
				TextVDouble textVDouble=(TextVDouble)_listControls.Find(x=>x.GetType()==typeof(TextVDouble));
				if(textVDouble!=null){
					return textVDouble.Text;
				}
				TextVTime textVTime=(TextVTime)_listControls.Find(x=>x.GetType()==typeof(TextVTime));
				if(textVTime!=null){
					return textVTime.Text;
				}
				TextPassword textPassword=(TextPassword)_listControls.Find(x=>x.GetType()==typeof(TextPassword));
				if(textPassword!=null){
					return textPassword.Text;
				}
				return "";
			}
		}

		///<summary>Will return a TimeSpan of zero if user did not enter anything.</summary>
		public TimeSpan TimeSpanResult {
			get {
				DateTime dateTime=PIn.DateT(((TextVTime)_listControls.Find(x => x is TextVTime)).Text);
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
			if(_listControls.Where(x => x is TextVDate).Any(x => !((TextVDate)x).IsValid())
				|| _listControls.Where(x => x is TextVTime).Any(x => !((TextVTime)x).IsValid())) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(_listControls.OfType<ComboBox>().Any(x=>!x.IsMultiSelect && x.SelectedIndex==-1)) {//single selection
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_listControls.OfType<ComboBox>().Any(x=>x.IsMultiSelect && x.SelectedIndices.Count==0)) {//multi selection
				MsgBox.Show(this,"Please make at least one selection.");
				return;
			}
			if(_listControls.OfType<CheckBox>().ToList().Count>1 && _listControls.OfType<CheckBox>().Where(x => x.Checked==true).Count()==0) {
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_listControls.OfType<CheckBox>().ToList().Count>1 && _listControls.OfType<CheckBox>().Where(x => x.Checked==true).Count()>1) {
				MsgBox.Show(this,"Can only make one selection.");
				return;
			}
			if(_listControls.OfType<RadioButton>().ToList().Count>1 && _listControls.OfType<RadioButton>().Where(x => x.Checked).Count()==0) {
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
			double heightDifference=0;
			_formFrame.Size=new System.Drawing.Size((int)ScaleF(_minWidth),(int)ScaleF(_curLocationY+90));
			if(SizeInitial.Width!=0 && SizeInitial.Height!=0){
				_formFrame.Size=new System.Drawing.Size((int)Math.Round(SizeInitial.Width*ScaleF(1)),(int)Math.Round(SizeInitial.Height*ScaleF(1)));
			}
			//Add the controls to form after sizing the window to prevent right-anchored controls from moving.
			for(int i=0;i<_listLabels.Count;i++)	{
				if(_listLabels[i].Width>=_formFrame.Width/ScaleF(1)) {
					_listLabels[i].Width=_formFrame.Width/ScaleF(1)-_listLabels[i].Margin.Left*2;
				}
				double heightOriginal=_listLabels[i].Height;
				_listLabels[i].Height=GetPreferredHeight(_listLabels[i].Text,_listLabels[i].Width).Height;
				if(heightOriginal!=_listLabels[i].Height){
					_formFrame.Height+=(int)(((int)_listLabels[i].Height-heightOriginal)*ScaleF(1));
					heightDifference=_listLabels[i].Height-heightOriginal;
				}
				grid.Children.Add(_listLabels[i]);
			}
			bool isFocusSet=false;
			for(int i=0;i<_listControls.Count;i++) {
				Point point=new Point(_listControls[i].Margin.Left,_listControls[i].Margin.Top);
				if(_listControls[i] is ComboBox || _listInputBoxParams[i].InputBoxType_==InputBoxType.TextBox){
					_listControls[i].Width=(int)Math.Round(_listControls[i].Width);
					double right=_formFrame.Width/ScaleF(1)-point.X-_listControls[i].Width;
					_listControls[i].Margin=new Thickness(left:point.X,top:point.Y+heightDifference,right:right,bottom:0);
					_listControls[i].Width=double.NaN;
				}
				grid.Children.Add(_listControls[i]);
				if(isFocusSet){
					continue;
				}
				if(!_listControls[i].Name.Contains("text")) {
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
		}
		#endregion Methods - Event Handlers

		#region Methods
		///<summary>Adds the requested controls to the form.</summary>
		private void AddInputControls() {
			_listControls=new List<System.Windows.Controls.UserControl>();
			_listLabels=new List<Label>();
			_curLocationY=12;
			double controlWidth=385;
			_minWidth=250;
			double posX=32;
			int itemOrder=1;
			for(int i=0;i<_listInputBoxParams.Count;i++) {
				if(_listInputBoxParams[i]==null) {
					continue;
				}
				if(!string.IsNullOrEmpty(_listInputBoxParams[i].LabelText)) {
					if(i>0) {
						_curLocationY+=10;
					}
					Label label=new Label();
					label.Text=_listInputBoxParams[i].LabelText;
					//int labelH=label.GetPreferredSize(new Size(controlWidth,0)).Height; // Not supported in WPF
					label.Measure(new Size(controlWidth,double.PositiveInfinity));
					double labelH=GetPreferredHeight(label.Text,controlWidth).Height;
					label.Width=controlWidth;
					label.Height=labelH;
					label.Name="labelPrompt"+itemOrder;
					label.HAlign=HorizontalAlignment.Left;
					label.VAlign=VerticalAlignment.Bottom;
					label.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
					label.Tag=_listInputBoxParams[i];
					_listLabels.Add(label);
					_curLocationY+=(int)label.Height+2;
				}
				System.Windows.Controls.UserControl control;
				switch(_listInputBoxParams[i].InputBoxType_) {
					case InputBoxType.TextBox:
						double right=_formFrame.Width-posX-controlWidth;
						if(_listInputBoxParams[i].IsPassswordCharStar){
							TextPassword textPassword=new TextPassword();
							textPassword.Name="textBox"+itemOrder;
							textPassword.Margin=new Thickness(left:posX,top:_curLocationY,right:right,bottom:0);
							textPassword.Width=controlWidth;
							textPassword.Height=20;
							textPassword.HorizontalAlignment=HorizontalAlignment.Stretch;
							textPassword.Text=_listInputBoxParams[i].Text;
							control=textPassword;
							break;
						}
						TextBox textBox=new TextBox();
						textBox.Name="textBox"+itemOrder;
						textBox.Margin=new Thickness(left:posX,top:_curLocationY,right:right,bottom:0);
						textBox.Width=controlWidth;
						textBox.Height=20;
						textBox.HorizontalAlignment=HorizontalAlignment.Stretch;
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
						textBoxMulti.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
						textBoxMulti.Width=controlWidth;
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
						Point point=new Point(posX+_listInputBoxParams[i].PointPosition.X,_curLocationY+_listInputBoxParams[i].PointPosition.Y);
						checkBox.Margin=new Thickness(left:point.X,top:point.Y,right:0,bottom:0);
						checkBox.Width=controlWidth;
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
						comboBox.Width=controlWidth;
						comboBox.Height=20;
						if(_listInputBoxParams[i].SizeParam.Width!=0 && _listInputBoxParams[i].SizeParam.Height!=0){
							comboBox.Width=_listInputBoxParams[i].SizeParam.Width;
							comboBox.Height=_listInputBoxParams[i].SizeParam.Height;
						}
						double comboRight=_formFrame.Width-posX-comboBox.Width;
						comboBox.Margin=new Thickness(left:posX,top:_curLocationY,right:comboRight,bottom:0);
						comboBox.HorizontalAlignment=HorizontalAlignment.Stretch;
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
						double combo2Right=_formFrame.Width-posX-controlWidth;
						comboBox2.Margin=new Thickness(left:posX,top:_curLocationY,right:combo2Right,bottom:0);
						comboBox2.Width=controlWidth;
						comboBox2.Height=21;
						comboBox2.HorizontalAlignment=HorizontalAlignment.Stretch;
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
						textVDate.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
						textVDate.Width=100;
						textVDate.Height=20;
						textVDate.Text=_listInputBoxParams[i].Text;
						control=textVDate;
						Label labelRight=new Label();//This is a second label to the right of textbox
						labelRight.Height=textVDate.Height;
						labelRight.Width=textVDate.Width;
						labelRight.Text=$"({CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern})"; //Gets the cultural format for date
						labelRight.Name="labelDateFormat"+itemOrder;
						Point pointLabel=new Point(posX+textVDate.Width+12,_curLocationY);
						labelRight.Margin=new Thickness(left:pointLabel.X,top:pointLabel.Y,right:0,bottom:0);
						labelRight.Tag=_listInputBoxParams[i];
						_listLabels.Add(labelRight);
						break;
					case InputBoxType.ValidTime:
						TextVTime textVTime=new TextVTime();
						textVTime.Name="textVTime"+itemOrder;
						textVTime.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
						textVTime.Width=120;
						textVTime.Height=20;
						control=textVTime;
						break;
					case InputBoxType.ValidDouble:
						TextVDouble textVDouble=new TextVDouble();
						textVDouble.Name="textVDouble"+itemOrder;
						textVDouble.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
						textVDouble.Width=120;
						textVDouble.Height=20;
						control=textVDouble;
						break;
					case InputBoxType.ValidPhone:
						TextBox textPhone=new TextBox();
						textPhone.Name="textPhone"+itemOrder;
						textPhone.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
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
						listBox.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
						listBox.Background=SystemColors.WindowBrush;
						listBox.SelectionMode=SelectionMode.MultiExtended;
						listBox.Items.AddList(_listInputBoxParams[i].ListSelections,x => x.ToString());
						listBox.Width=controlWidth;
						listBox.Height=95; //Default height used in original Form
						control=listBox;
						break;
					case InputBoxType.RadioButton:
						RadioButton radioButton=new RadioButton();
						radioButton.Name="radioButton"+itemOrder;
						radioButton.Margin=new Thickness(left:posX,top:_curLocationY,right:0,bottom:0);
						radioButton.Width=controlWidth;
						radioButton.Height=20;
						radioButton.Text=_listInputBoxParams[i].Text;
						control=radioButton;
						break;
					default:
						throw new NotImplementedException("InputBoxType: "+_listInputBoxParams[i].InputBoxType_+" not implemented.");
				}
				control.TabIndex=itemOrder++;
				control.Tag=_listInputBoxParams[i];
				_listControls.Add(control);
				_minWidth=(int)Math.Max(_minWidth,control.Width+80);
				_curLocationY+=(int)control.Height+2;
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

		private Size GetPreferredHeight(string textToMeasure,double width){
			Font font=new Font();
			font.Name="Segoe UI";
			font.Size=9;
			Graphics g=Graphics.MeasureBegin();
			return g.MeasureString(textToMeasure,font,width);
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





















