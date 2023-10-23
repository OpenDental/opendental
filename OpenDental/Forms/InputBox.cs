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
		private List<InputBoxParam> _listInputBoxParams;
		private List<Control> _listControls;
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

		public System.Windows.Forms.TextBox textResult {
			get {
				return (System.Windows.Forms.TextBox)_listControls.FirstOrDefault(x => x is System.Windows.Forms.TextBox);
			}
		}

		public UI.ComboBox comboSelection {
			get {
				return (UI.ComboBox)_listControls.FirstOrDefault(x => x is UI.ComboBox);
			}
		}

		public DateTime DateEntered {
			get {
				return PIn.Date(((ValidDate)_listControls.FirstOrDefault(x => x is ValidDate)).Text);
			}
		}

		public TimeSpan TimeEntered {
			get {
				return PIn.DateT(((ValidTime)_listControls.FirstOrDefault(x => x is ValidTime)).Text).TimeOfDay;
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

		public UI.CheckBox checkBoxResult {
			get {
				return (UI.CheckBox)_listControls.FirstOrDefault(x => x is UI.CheckBox);
			}
		}

		public List<int> SelectedIndices {
			get {
				Control control = _listControls.FirstOrDefault(x => x is UI.ComboBox || x is UI.ListBox);
				if(control==null) {
					return new List<int>();
				}
				if(control is UI.ComboBox) {
					return ((UI.ComboBox)control).SelectedIndices;
				}
				if(control is UI.ListBox) {
					return ((UI.ListBox)control).SelectedIndices;
				}
				return new List<int>();
			}
		}

		///<summary>Returns the text that was entered in the textboxes. Will be in the order they were passed into the constructor.</summary>
		public List<string> ListTextEntered {
			get {
				//Includes ValidDate and ValidTime since they inherit from TextBox.
				return _listControls.Where(x => x is System.Windows.Forms.TextBox).Select(x => ((System.Windows.Forms.TextBox)x).Text).ToList();
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
					new InputBoxParam(InputBoxType.ComboSelect,prompt,listSelections,listIndicesSelected:new List<int> { selectedIndex }) }) {	
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
			_listInputBoxParams=listInputBoxParams;
			Lan.F(this);
			AddInputControls();
			_funcOkClick=funcOkClick;
		}
		#endregion Constructors

		private void InputBox_Load(object sender, EventArgs e){
			int yChange=0;
			Height=LayoutManager.Scale(_curLocationY+90);
			Width=LayoutManager.Scale(_minWidth);
			if(SizeInitial!=Size.Empty){
				Size=LayoutManager.ScaleSize(SizeInitial);
			}
			for(int i=0;i<_listLabels.Count;i++)	{
				_listLabels[i].Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(8.25f));
				Size sizeOriginal=LayoutManager.ScaleSize(_listLabels[i].Size);
				_listLabels[i].Size=LayoutManager.ScaleSize(_listLabels[i].Size);
				//Only update height because running GetPreferredSize() for width will make unwanted changes and width is already the right size
				_listLabels[i].Height=_listLabels[i].GetPreferredSize(_listLabels[i].Size).Height;
				_listLabels[i].Location=new Point(LayoutManager.Scale(_listLabels[i].Left),LayoutManager.Scale(_listLabels[i].Top)+yChange);
				//Add to the window's size, the changes that GetPreferredSize() caused 
				Height+=_listLabels[i].Height-sizeOriginal.Height;
				yChange+=_listLabels[i].Height-sizeOriginal.Height;
			}
			//Add the controls to form after sizing the window to prevent right-anchored controls from moving.
			for(int i=0;i<_listLabels.Count;i++)	{
				LayoutManager.Add(_listLabels[i],this);
			}
			for(int i=0;i<_listControls.Count;i++) {
				_listControls[i].Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(8.25f));
				if(_listControls[i] is System.Windows.Forms.TextBox textBox){
					//textboxes must be fully scaled
					_listControls[i].Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));
				}
				_listControls[i].Size=LayoutManager.ScaleSize(_listControls[i].Size);
				_listControls[i].Location=new Point(LayoutManager.Scale(_listControls[i].Left),LayoutManager.Scale(_listControls[i].Top)+yChange);
				LayoutManager.Add(_listControls[i],this);
			}
		}

		///<summary>Adds the requested controls to the form.</summary>
		private void AddInputControls() {
			_listControls=new List<Control>();
			_listLabels=new List<Label>();
			_curLocationY=12;
			int controlWidth=385;
			_minWidth=250;
			int posX=32;
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
					label.Size=_listInputBoxParams[i].SizeParam;
					if(_listInputBoxParams[i].SizeParam==Size.Empty) {
						int labelH=label.GetPreferredSize(new Size(controlWidth,0)).Height;
						label.Size=new Size(controlWidth,labelH);
					}
					label.Name="labelPrompt"+itemOrder;
					label.TextAlign=ContentAlignment.BottomLeft;
					label.Location=new Point(posX,_curLocationY);
					label.Tag=_listInputBoxParams[i];
					_listLabels.Add(label);
					_curLocationY+=label.Height+2;
				}
				Control control;
				switch(_listInputBoxParams[i].InputBoxType_) {
					case InputBoxType.TextBox:
						System.Windows.Forms.TextBox textBox=new System.Windows.Forms.TextBox();
						textBox.Name="textBox"+itemOrder;
						textBox.Location=new Point(posX,_curLocationY);
						textBox.Size=new Size(controlWidth,20);
						textBox.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
						textBox.Text=_listInputBoxParams[i].Text;
						if(!String.IsNullOrEmpty(textBox.Text)) {
							textBox.SelectionStart=0;
							textBox.SelectionLength=textBox.Text.Length;
						}
						control=textBox;
						break;
					case InputBoxType.TextBoxMultiLine:
						System.Windows.Forms.TextBox textBoxMulti=new System.Windows.Forms.TextBox();
						textBoxMulti.Name="textBox"+itemOrder;
						textBoxMulti.Location=new Point(posX,_curLocationY);
						textBoxMulti.Size=new Size(controlWidth,100);
						textBoxMulti.Multiline=true;
						textBoxMulti.Text=_listInputBoxParams[i].Text;
						this.AcceptButton=null;
						textBoxMulti.ScrollBars=ScrollBars.Vertical;
						control=textBoxMulti;
						break;
					case InputBoxType.CheckBox:
						UI.CheckBox checkBox=new UI.CheckBox();
						checkBox.Name="checkBox"+itemOrder;
						checkBox.Location=new Point(posX+_listInputBoxParams[i].PointPosition.X,_curLocationY+_listInputBoxParams[i].PointPosition.Y);
						checkBox.Size=_listInputBoxParams[i].SizeParam==Size.Empty ? new Size(controlWidth,20) : _listInputBoxParams[i].SizeParam;
						checkBox.Text=_listInputBoxParams[i].Text;
						control=checkBox;
						if(_listInputBoxParams[i].HasTimeout) {
							_hasTimeout=true;
						}
						break;
					case InputBoxType.ComboSelect:
						UI.ComboBox comboBoxPlus=new UI.ComboBox();
						comboBoxPlus.Name="comboBox"+itemOrder;
						comboBoxPlus.Location=new Point(posX,_curLocationY);
						comboBoxPlus.Size=_listInputBoxParams[i].SizeParam==Size.Empty ? new Size(controlWidth,21) : _listInputBoxParams[i].SizeParam;
						comboBoxPlus.Anchor=AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						comboBoxPlus.Items.AddList<string>(_listInputBoxParams[i].ListSelections,x=>x);
						if(_listInputBoxParams[i].ListIndicesSelected.Count>0 && _listInputBoxParams[i].ListIndicesSelected[0].Between(0,comboBoxPlus.Items.Count-1)) {
							comboBoxPlus.SetSelected(_listInputBoxParams[i].ListIndicesSelected[0]);//If there is a valid initial selection, select it.
						}
						control=comboBoxPlus;
						break;
					case InputBoxType.ComboMultiSelect:
						UI.ComboBox comboBoxPlus2=new UI.ComboBox();
						comboBoxPlus2.SelectionModeMulti=true;
						comboBoxPlus2.Name="comboBox"+itemOrder;
						comboBoxPlus2.Location=new Point(posX,_curLocationY);
						comboBoxPlus2.Size=new Size(controlWidth,21);
						comboBoxPlus2.Anchor=AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						comboBoxPlus2.BackColor=SystemColors.Window;
						for(int j=0;j<_listInputBoxParams[i].ListSelections.Count;j++) {
							comboBoxPlus2.Items.Add(_listInputBoxParams[i].ListSelections[j]);
						}
						for(int j = 0;j<_listInputBoxParams[i].ListIndicesSelected.Count;j++) {
							if(_listInputBoxParams[i].ListIndicesSelected[j].Between(0,comboBoxPlus2.Items.Count-1)) {
								comboBoxPlus2.SetSelected(_listInputBoxParams[i].ListIndicesSelected[j]);//If there is a valid initial selection, select it.
							}
						}
						control=comboBoxPlus2;
						break;
					case InputBoxType.ValidDate:
						ValidDate validDate=new ValidDate();
						validDate.Name="validDate"+itemOrder;
						validDate.Location=new Point(posX,_curLocationY);
						validDate.Size=new Size(100,20);
						validDate.Text=_listInputBoxParams[i].Text;
						control=validDate;
						Label label=new Label();
						label.Size=new Size(label.Width,validDate.Height);
						label.Text=$"({CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern})";
						label.Name="labelDateFormat"+itemOrder;
						label.TextAlign=ContentAlignment.MiddleLeft;
						label.Location=new Point(validDate.Location.X+validDate.Width+12,_curLocationY);
						label.Tag=_listInputBoxParams[i];
						_listLabels.Add(label);
						break;
					case InputBoxType.ValidTime:
						ValidTime validTime=new ValidTime();
						validTime.Name="validTime"+itemOrder;
						validTime.Location=new Point(posX,_curLocationY);
						validTime.Size=new Size(120,20);
						control=validTime;
						break;
					case InputBoxType.ValidDouble:
						ValidDouble validDouble=new ValidDouble();
						validDouble.Name="validDouble"+itemOrder;
						validDouble.Location=new Point(posX,_curLocationY);
						validDouble.Size=new Size(120,20);
						control=validDouble;
						break;
					case InputBoxType.ValidPhone:
						ValidPhone validPhone=new ValidPhone();
						validPhone.Name="validPhone"+itemOrder;
						validPhone.Location=new Point(posX,_curLocationY);
						validPhone.Size=new Size(140,20);
						validPhone.Text=_listInputBoxParams[i].Text;
						if(!String.IsNullOrEmpty(validPhone.Text)) {
							validPhone.SelectionStart=0;
							validPhone.SelectionLength=validPhone.Text.Length;
						}
						control=validPhone;
						break;
					case InputBoxType.ListBoxMulti:
						UI.ListBox listBox=new UI.ListBox();
						listBox.Name="listBox"+itemOrder;
						listBox.Location=new Point(posX,_curLocationY);
						listBox.BackColor=SystemColors.Window;
						listBox.SelectionMode=OpenDental.UI.SelectionMode.MultiExtended;
						listBox.Items.AddList(_listInputBoxParams[i].ListSelections,x => x.ToString());
						listBox.Size=new Size(controlWidth,listBox.Height);
						control=listBox;
						break;
					case InputBoxType.RadioButton:
						RadioButton radioButton=new RadioButton();
						radioButton.Name="radioButton"+itemOrder;
						radioButton.Location=new Point(posX+_listInputBoxParams[i].PointPosition.X,_curLocationY+_listInputBoxParams[i].PointPosition.Y);
						radioButton.Size=_listInputBoxParams[i].SizeParam==Size.Empty ? new Size(controlWidth,20) : _listInputBoxParams[i].SizeParam;
						radioButton.Text=_listInputBoxParams[i].Text;
						control=radioButton;
						if(_listInputBoxParams[i].HasTimeout) {
							_hasTimeout=true;
						}
						break;
					default:
						throw new NotImplementedException("InputBoxType: "+_listInputBoxParams[i].InputBoxType_+" not implemented.");
				}
				control.TabIndex=itemOrder++;
				control.Tag=_listInputBoxParams[i];
				_listControls.Add(control);
				_minWidth=Math.Max(_minWidth,control.Width+80);
				_curLocationY+=control.Height+2;
			}
			//Now that we know the minWidth, we can center any controls that need to be centered.
			List<Control> listControlsLables=_listLabels.Cast<Control>().ToList();
			List<Control> listControls=_listControls.Union(listControlsLables).ToList();
			for(int j=0;j<listControls.Count;j++) { 
				InputBoxParam inputBoxParam=(InputBoxParam)listControls[j].Tag;
				if(inputBoxParam.HorizontalAlign!=HorizontalAlignment.Left) {
					listControls[j].Location=new Point((_minWidth-listControls[j].Width) / 2,listControls[j].Location.Y);
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
			if(_listControls.Where(x => x is ValidDate).Any(x => !((ValidDate)x).IsValid())
				|| _listControls.Where(x => x is ValidTime).Any(x => !((ValidTime)x).IsValid())) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(_listControls.OfType<UI.ComboBox>().Any(x=>!x.SelectionModeMulti && x.SelectedIndex==-1)) {//single selection
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_listControls.OfType<UI.ComboBox>().Any(x=>x.SelectionModeMulti && x.SelectedIndices.Count==0)) {//multi selection
				MsgBox.Show(this,"Please make at least one selection.");
				return;
			}
			if(_listControls.OfType<UI.CheckBox>().ToList().Count>1 && _listControls.OfType<UI.CheckBox>().Where(x => x.Checked).Count()==0) {
				MsgBox.Show(this,"Please make a selection.");
				return;
			}
			if(_listControls.OfType<UI.CheckBox>().ToList().Count>1 && _listControls.OfType<UI.CheckBox>().Where(x => x.Checked).Count()>1) {
				MsgBox.Show(this,"Can only make one selection.");
				return;
			}
			if(_listControls.OfType<RadioButton>().ToList().Count>1 && _listControls.OfType<RadioButton>().Where(x => x.Checked).Count()==0) {
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
		public InputBoxType InputBoxType_;
		public string LabelText;
		public List<string> ListSelections=new List<string>();
		public string Text="";
		public List<int> ListIndicesSelected=new List<int>();
		public bool HasTimeout;
		public Point PointPosition;
		public Size SizeParam;
		public HorizontalAlignment HorizontalAlign=HorizontalAlignment.Left;

		public InputBoxParam() {

		}

		public InputBoxParam(InputBoxType inputBoxType,string labelText,string text,Size sizeParam) {
			LabelText=labelText;
			InputBoxType_=inputBoxType;
			Text=text;
			SizeParam=sizeParam;
		}

		public InputBoxParam(InputBoxType inputBoxType,string labelText,string text,bool hasTimeout,Point pointPosition) {
			LabelText=labelText;
			InputBoxType_=inputBoxType;
			Text=text;
			HasTimeout=hasTimeout;
			PointPosition=pointPosition;
		}

		///<summary>If InputBoxType = ComboSelect or ComboMultiSelect, listComboSelections will be the items in the combobox.  If InputBoxType = TextBox or TextBoxMultiLine, textBoxText will be the default text in that text box. listSelectedIndices actually only works for single select.</summary>
		public InputBoxParam(InputBoxType inputBoxType,string labelText,List<string> listComboSelections=null,string text=null,
			List<int> listIndicesSelected=null) 
		{
			LabelText=labelText;
			InputBoxType_=inputBoxType;
			if(listComboSelections!=null) {
				ListSelections=listComboSelections;
			}
			if(!string.IsNullOrEmpty(text)) {
				Text = text;
			}
			if(listIndicesSelected!=null) {
				ListIndicesSelected=listIndicesSelected;
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





















