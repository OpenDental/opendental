using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Globalization;

namespace OpenDental{
	/// <summary>A quick entry form for various purposes. You can put several different types of controls on this form.</summary>
	public partial class InputBox : FormODBase {
		private List<InputBoxParam> _listInputParams;
		private List<Control> _listInputControls;
		private List<Label> _listLabels;
		private bool _hasTimeout=false;
		private Func<string,bool> _funcOkClick;
		int _curLocationY=2;
		int _minWidth=250;
		///<summary>Must use this to set the initial size rather than Size directly, because of layout manager.  Pass in a size in 96 dpi. It will get scaled.</summary>
		public Size SizeInitial;

		#region Properties - Public
		public int MaxInputTextLength {
			get {
				if(textResult==null) {
					return -1;
				}
				return textResult.MaxLength;
			}
			set {
				if(textResult!=null) {
					textResult.MaxLength=value;
				}
			}
		}

		public bool IsDeleteClicked { get; set; }

		public bool ShowDelete {
			get {
				return butDelete.Visible;
			}
			set {
				butDelete.Visible=value;
			}
		}

		public TextBox textResult {
			get {
				return (TextBox)_listInputControls.FirstOrDefault(x => x is TextBox);
			}
		}

		public ComboBoxOD comboSelection {
			get {
				return (ComboBoxOD)_listInputControls.FirstOrDefault(x => x is ComboBoxOD);
			}
		}

		public DateTime DateEntered {
			get {
				return PIn.Date(((ValidDate)_listInputControls.FirstOrDefault(x => x is ValidDate)).Text);
			}
		}

		public TimeSpan TimeEntered {
			get {
				return PIn.DateT(((ValidTime)_listInputControls.FirstOrDefault(x => x is ValidTime)).Text).TimeOfDay;
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

		public CheckBox checkBoxResult {
			get {
				return (CheckBox)_listInputControls.FirstOrDefault(x => x is CheckBox);
			}
		}

		public List<int> SelectedIndices {
			get {
				Control control=_listInputControls.FirstOrDefault(x => x is ComboBoxOD || x is ListBoxOD);
				if(control==null) {
					return new List<int>();
				}
				if(control is ComboBoxOD) {
					return ((ComboBoxOD)control).SelectedIndices;
				}
				if(control is ListBoxOD) {
					return ((ListBoxOD)control).SelectedIndices;
				}
				return new List<int>();
			}
		}

		///<summary>Returns the text that was entered in the textboxes. Will be in the order they were passed into the constructor.</summary>
		public List<string> ListTextEntered {
			get {
				//Includes ValidDate and ValidTime since they inherit from TextBox.
				return _listInputControls.Where(x => x is TextBox).Select(x => ((TextBox)x).Text).ToList();
			}
		}
		#endregion Properties - Public

		#region Constructors
		///<summary>Creates a textbox with a label containing the given prompt.</summary>
		public InputBox(string prompt) 
			: this(prompt,false) {	
					
		}

		///<summary>Creates a textbox with a label containing the given prompt, with a default string in the text field.</summary>
		public InputBox(string prompt,string defaultText) 
			: this(prompt,false,defaultText,Size.Empty) {	
					
		}

		public InputBox(string prompt,string defaultText,bool hasTimeout,Point position)
			: this(new List<InputBoxParam> { new InputBoxParam(InputBoxType.CheckBox,prompt,defaultText,hasTimeout,position) }) {

		}

		///<summary>Creates a textbox with a label containing the given prompt.</summary>
		public InputBox(string prompt,bool isMultiLine)
			: this(new List<InputBoxParam> { new InputBoxParam(isMultiLine ? InputBoxType.TextBoxMultiLine : InputBoxType.TextBox,prompt) }) {

		}

		///<summary>Creates a textbox with a label containing the given prompt, with a default string in the text field.</summary>
		public InputBox(string prompt,bool isMultiLine,string defaultText,Size paramSize)
			: this(new List<InputBoxParam> { new InputBoxParam(isMultiLine ? InputBoxType.TextBoxMultiLine : InputBoxType.TextBox,prompt,defaultText,paramSize) }) {

		}

		///<summary>This constructor allows a list of strings to be sent in and fill a comboBox for users to select from.</summary>
		public InputBox(string prompt,List<string> listSelections) 
			: this(prompt,listSelections,false) {	

		}
		
		///<summary>This constructor allows a list of strings to be sent in and fill a comboBox for users to select from.</summary>
		public InputBox(string prompt,List<string> listSelections,int selectedIndex) 
			: this(new List<InputBoxParam> {
					new InputBoxParam(InputBoxType.ComboSelect,prompt,listSelections,listSelectedIndices:new List<int> { selectedIndex }) }) {	
		}
		
		///<summary>This constructor allows a list of strings to be sent in and fill a listbox or comboBox for users to select from.</summary>
		public InputBox(string prompt,List<string> listSelections,bool isMultiSelect)
			: this(new List<InputBoxParam> {
					new InputBoxParam(isMultiSelect ? (listSelections.Count>=10 ? InputBoxType.ComboMultiSelect : InputBoxType.ListBoxMulti) : InputBoxType.ComboSelect,prompt,listSelections) }) {

		}

		public InputBox(Func<string,bool> onOkClick,params InputBoxParam[] arrayInputBoxParams) 
			: this(arrayInputBoxParams.ToList(),onOkClick) {

		}

		///<summary>Use this constructor to create multiple input controls.</summary>
		public InputBox(List<InputBoxParam> listInputBoxParams,Func<string,bool> funcOkClick=null) {
			InitializeComponent();
			InitializeLayoutManager();
			_listInputParams=listInputBoxParams;
			Lan.F(this);
			AddInputControls();
			_funcOkClick=funcOkClick;
		}
		#endregion Constructors

		private void InputBox_Load(object sender, EventArgs e){
			Height=LayoutManager.Scale(_curLocationY+90);
			Width=LayoutManager.Scale(_minWidth);
			if(SizeInitial!=Size.Empty){
				Size=LayoutManager.ScaleSize(SizeInitial);
			}
			//Add the controls to form after sizing the window to prevent right-anchored controls from moving.
			for(int i=0;i<_listLabels.Count;i++)	{
				_listLabels[i].Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));
				_listLabels[i].Size=LayoutManager.ScaleSize(_listLabels[i].Size);
				_listLabels[i].Location=new Point(LayoutManager.Scale(_listLabels[i].Left),LayoutManager.Scale(_listLabels[i].Top));
				LayoutManager.Add(_listLabels[i],this);
			}
			for(int i=0;i<_listInputControls.Count;i++) {
				_listInputControls[i].Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));
				_listInputControls[i].Size=LayoutManager.ScaleSize(_listInputControls[i].Size);
				_listInputControls[i].Location=new Point(LayoutManager.Scale(_listInputControls[i].Left),LayoutManager.Scale(_listInputControls[i].Top));
				LayoutManager.Add(_listInputControls[i],this);
			}
		}

		///<summary>Adds the requested controls to the form.</summary>
		private void AddInputControls() {
			_listInputControls=new List<Control>();
			_listLabels=new List<Label>();
			_curLocationY=2;
			int controlWidth=385;
			_minWidth=250;
			int posX=32;
			int itemOrder=1;
			foreach(InputBoxParam inputParam in _listInputParams) {
				if(inputParam==null) {
					continue;
				}
				if(!string.IsNullOrEmpty(inputParam.LabelText)) {
					Label label=new Label();
					label.AutoSize=false;
					label.Size=new Size(inputParam.ParamSize==Size.Empty ? controlWidth : inputParam.ParamSize.Width,36);
					label.Text=inputParam.LabelText;
					label.Name="labelPrompt"+itemOrder;
					label.TextAlign=ContentAlignment.BottomLeft;
					label.Location=new Point(posX,_curLocationY);
					label.Tag=inputParam;
					_listLabels.Add(label);
					_curLocationY+=38;
				}
				Control inputControl;
				switch(inputParam.ParamType) {
					case InputBoxType.TextBox:
						TextBox textBox=new TextBox();
						textBox.Name="textBox"+itemOrder;
						textBox.Location=new Point(posX,_curLocationY);
						textBox.Size=new Size(controlWidth,20);
						textBox.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
						textBox.Text=inputParam.Text;
						if(!String.IsNullOrEmpty(textBox.Text)) {
							textBox.SelectionStart=0;
							textBox.SelectionLength=textBox.Text.Length;
						}
						inputControl=textBox;
						_curLocationY+=22;
						break;
					case InputBoxType.TextBoxMultiLine:
						TextBox textBoxMulti=new TextBox();
						textBoxMulti.Name="textBox"+itemOrder;
						textBoxMulti.Location=new Point(posX,_curLocationY);
						textBoxMulti.Size=new Size(controlWidth,100);
						textBoxMulti.Multiline=true;
						textBoxMulti.Text=inputParam.Text;
						this.AcceptButton=null;
						textBoxMulti.ScrollBars=ScrollBars.Vertical;
						inputControl=textBoxMulti;
						_curLocationY+=102;
						break;
					case InputBoxType.CheckBox:
						CheckBox checkBox=new CheckBox();
						checkBox.Name="checkBox"+itemOrder;
						checkBox.Location=new Point(posX+inputParam.Position.X,_curLocationY+inputParam.Position.Y);
						checkBox.Size=inputParam.ParamSize==Size.Empty ? new Size(controlWidth,20) : inputParam.ParamSize;
						checkBox.Text=inputParam.Text;
						checkBox.FlatStyle=FlatStyle.System;
						inputControl=checkBox;
						if(inputParam.HasTimeout) {
							_hasTimeout=true;
						}
						_curLocationY+=checkBox.Size.Height+2;
						break;
					case InputBoxType.ComboSelect:
						UI.ComboBoxOD comboBoxPlus=new UI.ComboBoxOD();
						comboBoxPlus.Name="comboBox"+itemOrder;
						comboBoxPlus.Location=new Point(posX,_curLocationY);
						comboBoxPlus.Size=inputParam.ParamSize==Size.Empty ? new Size(controlWidth,21) : inputParam.ParamSize;
						comboBoxPlus.Anchor=AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						comboBoxPlus.Items.AddList<string>(inputParam.ListSelections,x=>x);
						if(inputParam.ListSelectedIndices.Count>0 && inputParam.ListSelectedIndices[0].Between(0,comboBoxPlus.Items.Count-1)) {
							comboBoxPlus.SetSelected(inputParam.ListSelectedIndices[0]);//If there is a valid initial selection, select it.
						}
						inputControl=comboBoxPlus;
						_curLocationY+=23;
						break;
					case InputBoxType.ComboMultiSelect:
						UI.ComboBoxOD comboBoxPlus2=new UI.ComboBoxOD();
						comboBoxPlus2.SelectionModeMulti=true;
						comboBoxPlus2.Name="comboBox"+itemOrder;
						comboBoxPlus2.Location=new Point(posX,_curLocationY);
						comboBoxPlus2.Size=new Size(controlWidth,21);
						comboBoxPlus2.Anchor=AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						comboBoxPlus2.BackColor=SystemColors.Window;
						foreach(string selection in inputParam.ListSelections) {
							comboBoxPlus2.Items.Add(selection);
						}
						foreach(int selection in inputParam.ListSelectedIndices) {
							if(selection.Between(0,comboBoxPlus2.Items.Count-1)) {
								comboBoxPlus2.SetSelected(selection);//If there is a valid initial selection, select it.
							}
						}
						inputControl=comboBoxPlus2;
						_curLocationY+=23;
						break;
					case InputBoxType.ValidDate:
						ValidDate validDate=new ValidDate();
						validDate.Name="validDate"+itemOrder;
						validDate.Location=new Point(posX,_curLocationY);
						validDate.Size=new Size(100,20);
						validDate.Text=inputParam.Text;
						inputControl=validDate;
						Label label=new Label();
						label.Size=new Size(label.Width,validDate.Height);
						label.Text=$"({CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern})";
						label.Name="labelDateFormat"+itemOrder;
						label.TextAlign=ContentAlignment.MiddleLeft;
						label.Location=new Point(validDate.Location.X+validDate.Width+12,_curLocationY);
						label.Tag=inputParam;
						_listLabels.Add(label);
						_curLocationY+=22;
						break;
					case InputBoxType.ValidTime:
						ValidTime validTime=new ValidTime();
						validTime.Name="validTime"+itemOrder;
						validTime.Location=new Point(posX,_curLocationY);
						validTime.Size=new Size(120,20);
						inputControl=validTime;
						_curLocationY+=22;
						break;
					case InputBoxType.ValidDouble:
						ValidDouble validDouble=new ValidDouble();
						validDouble.Name="validDouble"+itemOrder;
						validDouble.Location=new Point(posX,_curLocationY);
						validDouble.Size=new Size(120,20);
						inputControl=validDouble;
						_curLocationY+=22;
						break;
					case InputBoxType.ValidPhone:
						ValidPhone validPhone=new ValidPhone();
						validPhone.Name="validPhone"+itemOrder;
						validPhone.Location=new Point(posX,_curLocationY);
						validPhone.Size=new Size(140,20);
						validPhone.Text=inputParam.Text;
						if(!String.IsNullOrEmpty(validPhone.Text)) {
							validPhone.SelectionStart=0;
							validPhone.SelectionLength=validPhone.Text.Length;
						}
						inputControl=validPhone;
						_curLocationY+=22;
						break;
					case InputBoxType.ListBoxMulti:
						ListBoxOD listBox=new ListBoxOD();
						listBox.Name="listBox"+itemOrder;
						listBox.Location=new Point(posX,_curLocationY);
						listBox.BackColor=SystemColors.Window;
						listBox.SelectionMode=OpenDental.UI.SelectionMode.MultiExtended;
						listBox.Items.AddList(inputParam.ListSelections,x => x.ToString());
						listBox.Size=new Size(controlWidth,listBox.Height);
						inputControl=listBox;
						_curLocationY+=(listBox.Height)+2;
						break;
					case InputBoxType.RadioButton:
						RadioButton radioButton=new RadioButton();
						radioButton.Name="radioButton"+itemOrder;
						radioButton.Location=new Point(posX+inputParam.Position.X,_curLocationY+inputParam.Position.Y);
						radioButton.Size=inputParam.ParamSize==Size.Empty ? new Size(controlWidth,20) : inputParam.ParamSize;
						radioButton.Text=inputParam.Text;
						radioButton.FlatStyle=FlatStyle.System;
						inputControl=radioButton;
						if(inputParam.HasTimeout) {
							_hasTimeout=true;
						}
						_curLocationY+=radioButton.Size.Height+2;
						break;
					default:
						throw new NotImplementedException("InputBoxType: "+inputParam.ParamType+" not implemented.");
				}
				inputControl.TabIndex=itemOrder;
				inputControl.Tag=inputParam;
				_listInputControls.Add(inputControl);
				_minWidth=Math.Max(_minWidth,inputControl.Width+80);
			}
			//Now that we know the minWidth, we can center any controls that need to be centered.
			foreach(Control inputControl in _listInputControls.Union(_listLabels)) {
				InputBoxParam inputParam=(InputBoxParam)inputControl.Tag;
				if(inputParam.HorizontalAlign!=HorizontalAlignment.Left) {
					inputControl.Location=new Point((_minWidth-inputControl.Width) / 2,inputControl.Location.Y);
				}
			}
		}

		private void InputBox_Shown(object sender,EventArgs e) {
			if(_timer!=null && _hasTimeout) {
				_timer.Enabled=true;
			}
		}

		private void OnTimerTick(object sender,EventArgs e) {
			_timer.Enabled=false;
			_timer.Dispose();
			DialogResult=DialogResult.Abort;
		}

		public void setTitle(string title) {
			this.Text=title;
		}
		
		private void butDelete_Click(object sender,EventArgs e) {
			IsDeleteClicked=true;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(_listInputControls.Where(x => x is ValidDate)
				.Any(x => !((ValidDate)x).IsValid())
				|| _listInputControls.Where(x => x is ValidTime)
				.Any(x => !((ValidTime)x).IsValid())) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(_listInputControls.OfType<ComboBoxOD>().Any(x=>!x.SelectionModeMulti && x.SelectedIndex==-1)) {//single selection
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_listInputControls.OfType<ComboBoxOD>().Any(x=>x.SelectionModeMulti && x.SelectedIndices.Count==0)) {//multi selection
				MsgBox.Show(this,"Please make at least one selection.");
				return;
			}
			if(_listInputControls.OfType<CheckBox>().ToList().Count>1 && _listInputControls.OfType<CheckBox>().Where(x => x.Checked).Count()==0) {
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_listInputControls.OfType<CheckBox>().ToList().Count>1 && _listInputControls.OfType<CheckBox>().Where(x => x.Checked).Count()>1) {
				MsgBox.Show(this,"Can only make one selection.");
				return;
			}
			if(_listInputControls.OfType<RadioButton>().ToList().Count>1 && _listInputControls.OfType<RadioButton>().Where(x => x.Checked).Count()==0) {
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_funcOkClick!=null && !_funcOkClick(textResult.Text)) {
				//It is up to the implementor for _onOkClick to handle any messages to portray to the user.
				//We will simply block the user from closing the window.
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}

	public class InputBoxParam {
		public InputBoxType ParamType;
		public string LabelText;
		public List<string> ListSelections=new List<string>();
		public string Text="";
		public List<int> ListSelectedIndices=new List<int>();
		public bool HasTimeout;
		public Point Position;
		public Size ParamSize;
		public HorizontalAlignment HorizontalAlign=HorizontalAlignment.Left;

		public InputBoxParam() {

		}

		public InputBoxParam(InputBoxType paramType,string labelText,string text,Size paramSize) {
			LabelText=labelText;
			ParamType=paramType;
			Text=text;
			ParamSize=paramSize;
		}

		public InputBoxParam(InputBoxType paramType,string labelText,string text,bool hasTimeout,Point position) {
			LabelText=labelText;
			ParamType=paramType;
			Text=text;
			HasTimeout=hasTimeout;
			Position=position;
		}

		///<summary>If InputBoxType = ComboSelect or ComboMultiSelect, listComboSelections will be the items in the combobox.  If InputBoxType = TextBox or TextBoxMultiLine, textBoxText will be the default text in that text box. listSelectedIndices actually only works for single select.</summary>
		public InputBoxParam(InputBoxType paramType,string labelText,List<string> listComboSelections=null,string text=null,
			List<int> listSelectedIndices=null) 
		{
			LabelText=labelText;
			ParamType=paramType;
			if(listComboSelections!=null) {
				ListSelections=listComboSelections;
			}
			if(!string.IsNullOrEmpty(text)) {
				Text = text;
			}
			if(listSelectedIndices!=null) {
				ListSelectedIndices=listSelectedIndices;
			}
		}
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





















