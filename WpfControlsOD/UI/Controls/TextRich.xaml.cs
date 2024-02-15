using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using OpenDentBusiness;
using CodeBase;
using NHunspell;
using OpenDental;

namespace WpfControls.UI{
	/*
	Jordan is the only one allowed to edit this file.

	How to use the TextRich control:
	-Height of a single line box should be 20
	-This is the new replacement for the old ODTextBox and for any RichTextBoxes.
	-Name it like textRich...  The old naming convention was just text..., so the old names need to be fixed.
	-You can make it single line by setting height to 20 and AllowsCarriageReturns to false.
	-Click event handler usually looks like this:
			private void butRichNotes_Click(object sender,EventArgs e) { etc.
	-TextChanged event handler:
			private void butRichNotes_TextChanged(object sender,EventArgs e) { etc.
	-WPF RichTextBox is very powerful, but also necessarily complex to use.
		The main problem is that the single storage mechanism is a FlowDocument rather than Text.
		FlowDocument is always internally organized as paragraphs and runs, and is full of invisible formatting characters.
		If the user manually selects a range, then it's no problem to apply formatting to that selection.
		But if you programmatically select a range to format, it will generally be very hard to pick the correct range.
		The general strategies we employ for programmatic selection are as follows:
			1. If formatting characters are not present:
				a. Use complex math and loops to convert TextPointers to plain text indices and vice versa.
				b. This requires that all carriage returns are paired as \r\n. We might add code to throw exception for unpaired \r or \n.
				c. We include a method to remove all formatting to help easily implement this.
				d. (unworkable)If calling get/set SelectionLength, SelectionStart, or Select, throw exception if formatting is present.
				e. Because d was unworkable, the conversions are not accurate when formatting is involved, such as red spelling underlines.
			2. If formatting characters are present (bold, red spelling underlines, etc):
				a. Unknown what we should do. Nothing is implemented.
				b. One possibility might be to force external code selection to use paragraphs and runs.
				c. Another possibility might to to allow as long as it's crystal clear that the indices include formatting chars. This doesn't seem very useful.
		Spellcheck is internal, so it was easy to first break everything into runs, and then only look at the runs.
		QuickPaste note replacement also internally breaks into runs.
		Autonote yellow highlighting sticks to strategy #1.

	*/
	///<summary>You might use this control if you need QuickPaste, spell check, rich text, and/or autoNotes. Use sparingly. Jordan would always be involved.</summary>
	public partial class TextRich : UserControl{
		#region Fields
		///<summary>User can open a dialog from this textRich for AutoNote or QuickPaste. This flag is set to true during that time so that TextBox_LostFocus doesn't try to dispose of the textBox in FormSheetFillEdit.</summary>
		public bool IsDlgOpen=false;
		private Color _colorBack=Colors.White;
		private Color _colorText=Colors.Black;
		private ContextMenu contextMenu;
		private DispatcherTimer _dispatcherTimer;
		///<summary>This is shared by all TextRiches in the program.</summary>
		private static Hunspell HunspellGlobal;
		private bool _isMultiline=false;
		private HorizontalAlignment _hAlign=HorizontalAlignment.Left;
		private bool _isChangingFocusToTextBox;
		///<summary>This is a list of right click menuItems that were created by the PopupHelper for things like PatNum, Wiki, etc.</summary>
		private List<MenuItem> _listMenuItemsLinks;
		///<summary>This only includes misspelled words. We don't care about the other words.</summary>
		private List<TextRange> _listTextRangesMisspelled=new List<TextRange>();
		private MenuItem menuItemSuggest1;
		private MenuItem menuItemSuggest2;
		private MenuItem menuItemSuggest3;
		private MenuItem menuItemSuggest4;
		private MenuItem menuItemSuggest5;
		private MenuItem menuItemAddToDict;
		private MenuItem menuItemDisableSpell;
		private MenuItem menuItemInsertDate;
		private MenuItem menuItemInsertQP;
		private MenuItem menuItemInsertAN;
		private MenuItem menuItemCut;
		private MenuItem menuItemCopy;
		private MenuItem menuItemPaste;
		private MenuItem menuItemPastePlain;
		private MenuItem menuItemEditAN;
		private EnumQuickPasteType _quickPasteType=EnumQuickPasteType.None;
		private Separator separatorLinks;
		private bool _readOnly;
		///<summary>The separator below the suggestions. Idx 5.</summary>
		private Separator separator1;
		///<summary>The separator below disable. Idx 8.</summary>
		private Separator separator2;
		///<summary>The separator below the inserts. Idx 12.</summary>
		private Separator separator3;
		//<summary>This is a filename like ODSpelling.lex.  It is located in the temp folder, but that path is not part of this field.  The file is regenerated once per week and also if user on this computer adds any words to custom dict.  Checking for the file existence and timestamp and regenerating it is a slow step, but seems to be a MS limitation.</summary>
		//private string _spellCheckFileLoc="ODSpelling.lex";//couldn't use this. Must build spell check from scratch.
		
		#endregion Fields

		#region Constructor
		public TextRich(){
			InitializeComponent();
			//Width=75;
			//Height=20;
			Focusable=true;//so that .Focus() will work, but then we manually change focus to textBox
			richTextBox.GotKeyboardFocus+=textBox_GotKeyboardFocus;
			richTextBox.KeyUp+=textBox_KeyUp;
			richTextBox.LostFocus+=textBox_LostFocus;
			richTextBox.LostKeyboardFocus+=textBox_LostKeyboardFocus;
			richTextBox.PreviewKeyDown+=textBox_PreviewKeyDown;
			richTextBox.TextChanged+=textBox_TextChanged;
			GotKeyboardFocus+=This_GotKeyboardFocus;
			IsEnabledChanged+=This_IsEnabledChanged;
			Loaded+=This_Loaded;
			LostFocus+=This_LostFocus;
			LostKeyboardFocus+=This_LostKeyboardFocus;
			PreviewMouseLeftButtonDown+=This_PreviewMouseLeftButtonDown;
			bool isDesignMode=DesignerProperties.GetIsInDesignMode(this);
			if(!isDesignMode && HunspellGlobal==null) {
				if(ODBuild.IsDebug()) {
					try {
						HunspellGlobal=new Hunspell(Properties.Resources.en_US_aff,Properties.Resources.en_US_dic);
					}
					catch(Exception ex) {
						ex.DoNothing();
//todo:
						//System.IO.File.Copy(@"..\..\..\..\Required dlls\Hunspellx64.dll","Hunspellx64.dll");
						//System.IO.File.Copy(@"..\..\..\..\Required dlls\Hunspellx86.dll","Hunspellx86.dll");
						HunspellGlobal=new Hunspell(Properties.Resources.en_US_aff,Properties.Resources.en_US_dic);
					}
				}
				else {//not debug
					HunspellGlobal=new Hunspell(Properties.Resources.en_US_aff,Properties.Resources.en_US_dic);
				}
			}
			contextMenu=new ContextMenu(this);
			richTextBox.ContextMenu=contextMenu;
			menuItemSuggest1=new MenuItem("",menuItem_Click);
			contextMenu.Items.Add(menuItemSuggest1);//These five menu items will hold the suggested spelling for misspelled words.  If no misspelled words, they will not be visible.
			menuItemSuggest2=new MenuItem("",menuItem_Click);
			contextMenu.Items.Add(menuItemSuggest2);
			menuItemSuggest3=new MenuItem("",menuItem_Click);
			contextMenu.Items.Add(menuItemSuggest3);
			menuItemSuggest4=new MenuItem("",menuItem_Click);
			contextMenu.Items.Add(menuItemSuggest4);
			menuItemSuggest5=new MenuItem("",menuItem_Click);
			contextMenu.Items.Add(menuItemSuggest5);
			separator1=new Separator();
			contextMenu.Items.Add(separator1);
			menuItemAddToDict=new MenuItem(Lans.g(this,"Add to Dictionary"),menuItem_Click);
			contextMenu.Items.Add(menuItemAddToDict);
			menuItemDisableSpell=new MenuItem(Lans.g(this,"Disable Spell Check"),menuItem_Click);
			contextMenu.Items.Add(menuItemDisableSpell);
			separator2=new Separator();
			contextMenu.Items.Add(separator2);
			menuItemInsertDate=new MenuItem(Lans.g(this,"Insert Date"),menuItem_Click);
			menuItemInsertDate.Shortcut="D";
			contextMenu.Items.Add(menuItemInsertDate);
			menuItemInsertQP=new MenuItem(Lans.g(this,"Insert Quick Paste Note"),menuItem_Click);
			menuItemInsertQP.Shortcut="Q";
			contextMenu.Items.Add(menuItemInsertQP);
			menuItemInsertAN=new MenuItem(Lans.g(this,"Insert Auto Note"),menuItem_Click);
			contextMenu.Items.Add(menuItemInsertAN);
			separator3=new Separator();
			contextMenu.Items.Add(separator3);
			menuItemCut=new MenuItem(Lans.g(this,"Cut"),menuItem_Click);
			menuItemCut.Shortcut="X";
			contextMenu.Items.Add(menuItemCut);
			menuItemCopy=new MenuItem(Lans.g(this,"Copy"),menuItem_Click);
			menuItemCopy.Shortcut="C";
			contextMenu.Items.Add(menuItemCopy);
			menuItemPaste=new MenuItem(Lans.g(this,"Paste"),menuItem_Click);
			menuItemPaste.Shortcut="V";
			contextMenu.Items.Add(menuItemPaste);
			menuItemPastePlain=new MenuItem(Lans.g(this,"Paste Plain Text"),menuItem_Click);
			contextMenu.Items.Add(menuItemPastePlain);
			menuItemEditAN=new MenuItem(Lans.g(this,"Edit Auto Note"),menuItem_Click);
			contextMenu.Items.Add(menuItemEditAN);
			separatorLinks=new Separator();
			contextMenu.Items.Add(separatorLinks);
			separatorLinks.Visibility=Visibility.Collapsed;
			contextMenu.Opened+=ContextMenu_Opened;
			_dispatcherTimer=new DispatcherTimer();
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(500);
			_dispatcherTimer.Tick+=_dispatcherTimer_Tick;
		}
		#endregion Constructor

		#region Events
		public event EventHandler Click;
		[Category("OD")]
		[Description("Try not to use this because it will also fire when changing the value programmatically, like on load. This can cause infinite loops. One good pattern to avoid this is a class level boolean to disable the code inside this event handler during certain situations like load.")]
		public event EventHandler TextChanged;
		#endregion Events

		#region Properties
		[Category("OD")]
		[Description("Set to false to prevent the user from entering carriage returns.")]
		[DefaultValue(true)]
		public bool AllowsCarriageReturns { get; set; }=true;

		[Category("OD")]
		[DefaultValue(typeof(Color),"White")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				richTextBox.Background=new SolidColorBrush(value);
			}
		}

		[Category("OD")]
		[DefaultValue(typeof(Color),"Black")]
		public Color ColorText {
			get {
				return _colorText;
			}
			set {
				_colorText = value;
				richTextBox.Foreground=new SolidColorBrush(value);
			}
		}

		//DetectLinksEnabled
		//No. This is not even used in the old ODTextBox.
		//We seem to also have RightClickLinks,
		//so maybe that was the replacement for this, and someone forgot to remove this.
		//public bool DetectLinksEnabled {

		///<summary>Set true to enable formatted text to be pasted.</summary>
		[Category("OD")]
		[Description("Set true to enable formatted text to be pasted.")]
		[DefaultValue(false)]
		public bool FormattedTextAllowed {get;set; }=false;
		//this was called EditMode in previous version of ODtextBox

		///<summary>Set to true to enable context menu option to insert auto notes.</summary>
		[Category("OD"), Description("Set to true to enable context menu option to insert auto notes.")]
		[DefaultValue(false)]
		public bool HasAutoNotes  { get; set; }=false;

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

		//IsMultiline
		//[Category("OD")]
		//Unlike the normal UI.TextBox, we always wrap. We have to because the RichTextBox does not have a way to turn of wrapping. Also see AllowsCarriageReturns instead of this.
		//public bool IsMultiline {
		//	get{
		//		return _isMultiline;
		//	}
		//	set{
		//		_isMultiline=value;
		//		if(_isMultiline){
		//			textBox.TextWrapping=TextWrapping.Wrap;
		//			textBox.AcceptsReturn=true;
		//			return;
		//		}
		//		textBox.TextWrapping=TextWrapping.NoWrap;
		//		textBox.AcceptsReturn=false;
		//	}
		//}

		///<summary></summary>
		[Category("OD")]
		[Description("If 'None' is used, then QuickPaste notes are disabled. This determines which category of Quick Paste notes opens first.")]
		[DefaultValue(EnumQuickPasteType.None)]
		public EnumQuickPasteType QuickPasteType {
			get {
				return _quickPasteType;
			}
			set {
				_quickPasteType=value;
				//if(value==EnumQuickPasteType.None) {//no, we allow this now
				//	throw new InvalidEnumArgumentException("A value for the TextBoxLoc property must be set.");
				//}
			}
		}

		[Category("OD")]
		[Description("Set true to prevent user from editing. Enabled false also does this, but that also grays out the text. In WinForms, setting this to true also automatically changed the background color to 'Control'. In WPF, this is not paired, but can be done separately.")]
		public bool ReadOnly {
			get{
				return _readOnly;
			}
			set{
				//The MSWF textbox also changes backColor when changing this.  In WPF, they are independent.
				_readOnly=value;
				if(_readOnly){
					richTextBox.IsReadOnly=true;
					return;
				}
				richTextBox.IsReadOnly=false;
			}
		}

		///<summary>If right click, and there is 'PatNum:##', 'TaskNum:##', or 'JobNum:##', then the context menu will show a clickable link. Only used task edit window.</summary>
		[Category("OD")]
		[Description("If right click, and there is 'PatNum:##', 'TaskNum:##', or 'JobNum:##', then the context menu will show a clickable link. Only used in task edit window.")]
		[DefaultValue(false)]
		public bool RightClickLinks { get; set; }=false;

		[Browsable(false)]//this wouldn't mix well with XAML
		public string Rtf{
			get {
				FlowDocument flowDocument=richTextBox.Document;
				TextRange textRange=new TextRange(flowDocument.ContentStart,flowDocument.ContentEnd);
				if(!textRange.CanSave(DataFormats.Rtf)){
					return "";
				}
				using MemoryStream memoryStream=new MemoryStream();
				textRange.Save(memoryStream,DataFormats.Rtf);
				memoryStream.Position=0;
				using StreamReader streamReader=new StreamReader(memoryStream);
				string str=streamReader.ReadToEnd();
				return str;
			}
			set {
				FlowDocument flowDocument=new FlowDocument();
				TextRange textRange=new TextRange(flowDocument.ContentStart,flowDocument.ContentEnd);
				if(!textRange.CanLoad(DataFormats.Rtf)){
					return;
				}
				using MemoryStream memoryStream=new MemoryStream();
				using StreamWriter streamWriter=new StreamWriter(memoryStream);
				streamWriter.Write(value);
				streamWriter.Flush();
				textRange.Load(memoryStream,DataFormats.Rtf);
				richTextBox.Document=flowDocument;
			}
		}

		[Browsable(false)]
		public string SelectedText{
			get {
				if(SelectionLength==0){
					return "";
				}
				TextSelection textSelection=richTextBox.Selection;
				TextPointer textPointerStart=textSelection.Start;
				TextPointer textPointerEnd=textSelection.End;
				TextRange textRange=new TextRange(textPointerStart,textPointerEnd);
				string str=textRange.Text;
				if(str.EndsWith("\r\n")){
					//see the note down in Text.get. So this really only kicks in and matters if they have selected all the way to the end.
					str=str.Substring(0,str.Length-2);
				}
				return str;
			}
			set {
				richTextBox.Selection.Text=value;
				TextSelection textSelection = richTextBox.Selection;
				TextPointer textPointerEnd = textSelection.End;
				richTextBox.Selection.Select(textPointerEnd,textPointerEnd);
			}
		}

		///<summary>This is different from WF RichTextBox because of invisible formatting characters. See discussion at top of this file. Also, in WPF, our buttons do not steal focus, so setting this works a little better/differently. You might need to set focus after using this.</summary>
		[Browsable(false)]
		public int SelectionLength { 
			get{
				ThrowExceptionIfHasFormatting();
				TextSelection textSelection=richTextBox.Selection;
				TextPointer textPointerStart=textSelection.Start;
				TextPointer textPointerEnd=textSelection.End;
				TextPointer textPointerContentStart=richTextBox.Document.ContentStart;
				//adjust content start so that the setter is consistent
				while(true){
					if(textPointerContentStart.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text){
						break;
					}
					textPointerContentStart=textPointerContentStart.GetNextContextPosition(LogicalDirection.Forward);
					if(textPointerContentStart is null){
						return 0;
					}
				}
				int offsetStart=textPointerContentStart.GetOffsetToPosition(textPointerStart);
				int offsetEnd=textPointerContentStart.GetOffsetToPosition(textPointerEnd);
				int idxPlainStart=ConvertIdxToPlain(textPointerContentStart,offsetStart);
				int idxPlainEnd=ConvertIdxToPlain(textPointerContentStart,offsetEnd);
				int idxDiff=idxPlainEnd-idxPlainStart;
				return idxDiff;
			}
			set{
				if(value>0){
					ThrowExceptionIfHasFormatting();
				}
				//else for value of 0, we don't check for formatting because it doesn't matter.
				if(value<0){
					return;//invalid. Do nothing
				}
				/*
				TextPointer textPointerContentStart=richTextBox.Document.ContentStart;
				//move forward to actual text
				while(true){
					if(textPointerContentStart.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text){
						break;
					}
					textPointerContentStart=textPointerContentStart.GetNextContextPosition(LogicalDirection.Forward);
					if(textPointerContentStart is null){
						return;
					}
				}*/
				TextSelection textSelection=richTextBox.Selection;
				TextPointer textPointerStart=textSelection.Start;
				//int idxStart=textPointerContentStart.GetOffsetToPosition(textPointerStart);
				TextPointer textPointerEnd=ConvertLengthFromPlain(textPointerStart,value);
				//int iEnd=iStart+value;//this works even if value is 0
				//TextPointer textPointerEnd=textPointerContentStart.GetPositionAtOffset(iEnd);
				//if(textPointerEnd is null){
				//	return;//invalid.
				//}
				richTextBox.Selection.Select(textPointerStart,textPointerEnd);
				//this is something that the caller might need to do. We shouldn't do it here:
				//if(!richTextBox.IsKeyboardFocused){
				//	richTextBox.Focus();
				//}
			}
		}

		///<summary>This is different from WF RichTextBox because of invisible formatting characters. See discussion at top of this file. Also, in WPF, our buttons do not steal focus, so setting this works a little better/differently. You might need to set focus after using this.</summary>
		[Browsable(false)]
		public int SelectionStart { 
			get{
				ThrowExceptionIfHasFormatting();
				TextSelection textSelection=richTextBox.Selection;
				TextPointer textPointerStart=textSelection.Start;
				TextPointer textPointerContentStart=richTextBox.Document.ContentStart;
				//adjust content start so that the setter is consistent
				while(true){
					if(textPointerContentStart.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text){
						break;
					}
					textPointerContentStart=textPointerContentStart.GetNextContextPosition(LogicalDirection.Forward);
					if(textPointerContentStart is null){
						return 0;
					}
				}
				int offset=textPointerContentStart.GetOffsetToPosition(textPointerStart);
				int idxPlain=ConvertIdxToPlain(textPointerContentStart,offset);
				return idxPlain;
			}
			set{
				if(value>0){
					ThrowExceptionIfHasFormatting();
				}
				//else for value of 0, we don't check for formatting because it doesn't matter.
				if(value<0){
					return;//invalid. Do nothing
				}
				TextPointer textPointerContentStart=richTextBox.Document.ContentStart;
				while(true){
					if(textPointerContentStart.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text){
						break;
					}
					textPointerContentStart=textPointerContentStart.GetNextContextPosition(LogicalDirection.Forward);
					if(textPointerContentStart is null){
						return;
					}
				}
				TextSelection textSelection=richTextBox.Selection;
				TextPointer textPointerStart=textSelection.Start;
				int iStart=textPointerContentStart.GetOffsetToPosition(textPointerStart);
				if(iStart<0){
					iStart=0;
				}
				TextPointer textPointerEnd=textSelection.End;
				int delta=0;
				//The purpose of delta is to maintain the length of the selection when the starting pos changes.
				//Probably need to get rid of it because too complex.
				if(textPointerEnd!=null){
					int iEnd=textPointerContentStart.GetOffsetToPosition(textPointerEnd);
					delta=iEnd-iStart;
				}
				if(delta<0){
					delta=0;
				}
				int idxTextPointer=ConvertIdxFromPlain(value);
				TextPointer textPointerStartNew=textPointerContentStart.GetPositionAtOffset(idxTextPointer);
				if(textPointerStartNew is null){
					return;//invalid.
				}
				TextPointer textPointerEndNew=textPointerContentStart.GetPositionAtOffset(idxTextPointer+delta);
				if(textPointerEndNew is null){
					textPointerEndNew=textPointerStartNew;//length 0
				}
				richTextBox.Selection.Select(textPointerStartNew,textPointerEndNew);
			}
		}

		///<summary>True by default to enable spell checking.</summary>
		[Category("OD")]
		[Description("True by default to enable spell checking.")]
		[DefaultValue(true)]
		public bool SpellCheckIsEnabled {get;set; }=true;

		[Category("OD")]
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			get{
				return richTextBox.TabIndex;
			}
			set{
				richTextBox.TabIndex=value;
				//InvalidateVisual();//if we want the new TabIndex value to show immediately. But there's a performance hit, so no. Also no longer relevant.
			}
		}

		[Category("OD")]
		[DefaultValue("")]
		public string Text {
			get {
				FlowDocument flowDocument=richTextBox.Document;
				TextRange textRange=new TextRange(flowDocument.ContentStart,flowDocument.ContentEnd);
				string str=textRange.Text;
				//A flowdoc paragraph always ends with a CRLF. We need to strip this from the last paragraph.
				//This seems a little hackey, but it does reliably work for both no trailing CRLF and any number of intentional CRLFs.
				if(str.EndsWith("\r\n")){
					str=str.Substring(0,str.Length-2);
				}
				return str;
			}
			set {
				FlowDocument flowDocument=new FlowDocument();
				TextRange textRange=new TextRange(flowDocument.ContentStart,flowDocument.ContentEnd);
				if(value is null){
					textRange.Text="";
				}
				else{
					textRange.Text=value;
					if(value.EndsWith("\r\n")){
						Paragraph paragraph=new Paragraph();
						flowDocument.Blocks.Add(paragraph);
					}
				}
				richTextBox.Document=flowDocument;
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
		#endregion Properties

		#region Methods - public
		public void ClearWavyAll(){
			for(int i=0;i<_listTextRangesMisspelled.Count;i++){
				//string str=_listTextRanges[i].Text;//for testing
				//If we want to be more delicate, like not removing strike through or underlining unrelated to spellcheck, 
				//then we might do something like this (totally thrown together and untested:
				//TextDecorationCollection textDecorationCollection=(TextDecorationCollection)_listTextRanges[i].GetPropertyValue(Inline.TextDecorationsProperty);
				//for(int d=textDecorationCollection.Count;d>=0;d--){//go backwards
				//	if(textDecorationCollection[d].Location!=TextDecorationLocation.Underline){
				//		continue;
				//	}
				//	Need more elegant if statements
					//textDecorationCollection.RemoveAt(d]);
				//}
				_listTextRangesMisspelled[i].ApplyPropertyValue(Inline.TextDecorationsProperty,new TextDecorationCollection());
				//_listTextRanges[i].ClearAllProperties();//overkill. Needs to not clear bold.
			}
		}

		public TextSelection GetSelection(){
			return richTextBox.Selection;
		}

		///<summary>If you're looking for the overload with zero arguments, it's different in WPF. Use SelectAll() or Focus(), depending on your intent. Use SelectAll() to change focus and select all text. In WinForms, Select() was used to change focus without changing the selection (supposedly). The equivalent in WPF for that purpose is Focus().</summary>
		public void Select(int start,int length){
			SelectionStart=start;
			SelectionLength=length;
		}

		public void SelectAll(){
//todo: this line is failing
			bool gotFocus=richTextBox.Focus();
			richTextBox.SelectAll();
		}

		///<summary>Named the same as a MSWF richTextBox property, but just called differently.</summary>
		public void SelectionBackColor(Color color){
			//richTextBox.SelectionBrush=new SolidColorBrush(color);//this is wrong. Color would go away once it's not highlighted.
			TextSelection textSelection=richTextBox.Selection;
			textSelection.ApplyPropertyValue(TextElement.BackgroundProperty,new SolidColorBrush(color));
		}

		public void SpellCheck(){
			ClearWavyAll();
			_listTextRangesMisspelled=new List<TextRange>();
			TextPointer textPointer=richTextBox.Document.ContentStart;
			while(true){
				if(textPointer is null){
					break;//end of content
				}
				if(textPointer.GetPointerContext(LogicalDirection.Forward)!=TextPointerContext.Text){
					textPointer=textPointer.GetNextContextPosition(LogicalDirection.Forward);
					continue;
				}
				//text run found.
				string run=textPointer.GetTextInRun(LogicalDirection.Forward);
				//process the run
				MatchCollection matchCollection=Regex.Matches(run,@"\b[A-Za-z'-]{3,}\b");
				//Regex explanation:
				//\b=word boundary on both sides (whitespace, punctuation, etc)
				//[...]=matches letters, apost, and hyphen. Notice it will not include any word with a number.
				//{3,}=word is at least 3 letters long.
				for(int i=0;i<matchCollection.Count;i++){
					if(HunspellGlobal.Spell(matchCollection[i].Value)) {//spelled correctly
						continue;
					}
					if(HunspellGlobal.Spell(matchCollection[i].Value.ToLower()) ) {//Hunspell is case sensitive, so this tries different casing.
						continue;
					}
					//if(ODBuild.IsDebug()
					//	&& (Environment.MachineName.ToLower()=="jordansgalaxybk" || Environment.MachineName.ToLower()=="jordanhome" || Environment.MachineName.ToLower()=="jordancryo"))
					//{
						//for some testing without a db
					//}
					else{
						if(DictCustoms.GetFirstOrDefault(x => x.WordText.ToLower()==matchCollection[i].Value.ToLower())!=null) {
							continue;
						}
					}
					//spelling is incorrect
					TextPointer textPointerWordStart=textPointer.GetPositionAtOffset(matchCollection[i].Index);
					TextPointer textPointerWordEnd=textPointer.GetPositionAtOffset(matchCollection[i].Index+matchCollection[i].Length);
					TextRange textRange=new TextRange(textPointerWordStart,textPointerWordEnd);
					_listTextRangesMisspelled.Add(textRange);
				}
				/*
				//This code might be useful later to find the word that the caret is on.
				//Or, we could just loop through the list of textRanges that we got previously.
				TextPointer textPointerWordStart=textPointer.GetPositionAtOffset(0);
				TextPointer textPointerWordEnd=textPointer.GetPositionAtOffset(run.Length);
				TextRange textRange=new TextRange(textPointerWordStart,textPointerWordEnd);
				listTextRanges.Add(textRange);*/
				//jump to end of text
				textPointer=textPointer.GetPositionAtOffset(run.Length);
			}
			for(int i=0;i<_listTextRangesMisspelled.Count;i++){
				Pen pen=new Pen();
				pen.Brush=new SolidColorBrush(Color.FromRgb(235,0,0));
				pen.Thickness=0.5;//diagonal lines look great at less than 1.
				pen.EndLineCap=PenLineCap.Square;//add caps so that we have more to work with for splicing.
				pen.StartLineCap=PenLineCap.Square;
				PathSegmentCollection pathSegmentCollection=new PathSegmentCollection();
				//starting point 0,0 not included in segments
				//This looked good but was touching the text: Thick:0.6; Segs:2,2; 4,0; VP:5,3, lineH:4
				//This looked good but was too small when zoomed back to 100: Thick:0.5; Segs:1,1; 2,0; VB:0,-0.5,2,2.5; VP:2,2.5, lineH:3.5
				//The current choices look very good, probably a bit tighter than I would like, but totally acceptable.
				pathSegmentCollection.Add(new LineSegment(new Point(1.5,1.5),true));
				pathSegmentCollection.Add(new LineSegment(new Point(3,0),true));
				PathFigure pathFigure=new PathFigure(start:new Point(0,0),pathSegmentCollection,closed:false);
				PathGeometry pathGeometry=new PathGeometry(new PathFigure[]{pathFigure });
				DrawingBrush drawingBrush=new DrawingBrush();
				//The numbers below were obtained through hours of trial and error.
				//Each tweak required readjusting multiple other numbers,
				//Including pen.Thickness=0.5, segment path, 
				//viewbox is what section of the path we are grabbing. y=-0.5 in order to create white space above, effectively pushing red line away from text.
				//viewport is size of tile on target. Same size as viewbox.
				//target pen is 3.5 so we have room for things.
				drawingBrush.Viewbox=new Rect(x:0,y:-1,width:3,height:3);
				drawingBrush.ViewboxUnits=BrushMappingMode.Absolute;
				drawingBrush.Viewport = new Rect(x:0,y:0,width:3,height:3);
				drawingBrush.ViewportUnits=BrushMappingMode.Absolute;
				drawingBrush.TileMode = TileMode.Tile;
				drawingBrush.Stretch=Stretch.Fill;
				drawingBrush.Drawing=new GeometryDrawing(brush:null,pen,pathGeometry);
				TextDecoration textDecoration=new TextDecoration();
				textDecoration.Pen=new Pen(drawingBrush,4);
				TextDecorationCollection textDecorationCollection=new TextDecorationCollection();
				textDecorationCollection.Add(textDecoration);
				_listTextRangesMisspelled[i].ApplyPropertyValue(Inline.TextDecorationsProperty,textDecorationCollection);
			}
				/*Red wavy line.  This worked and looks gorgeous, but it's too dark and thick. Switching to a simpler zigzag line that will be less distracting.
				//The zigzag was easy, and it even still looks like a wavy line.
				Pen pen=new Pen();
				pen.Brush=new SolidColorBrush(Color.FromRgb(235,0,0));
				pen.Thickness=0.4;//0.4 feels too thick, but anything less quickly fades out and looks terrible.
				pen.EndLineCap=PenLineCap.Square;//add caps so that we have more to work with for splicing.
				pen.StartLineCap=PenLineCap.Square;
				BezierSegment bezierSegment=new BezierSegment(new Point(1,0),new Point(2,2),new Point(3,1),isStroked:true);
				PathFigure pathFigure=new PathFigure(start:new Point(0,1),new PathSegment[]{bezierSegment},closed:false);
				PathGeometry pathGeometry=new PathGeometry(new PathFigure[]{pathFigure });
				DrawingBrush drawingBrush=new DrawingBrush();
				//Viewport: 
				//This is the base for each tile.
				//Height of less than 4 fades out and looks terrible.
				//Unfortunately, they don't quite give us 4 pixels to work with, so we'll try 3.5.
				//Line height should be taller than viewport height because of antialiasing.
				//Y might need to be adjusted by some arbitrary number because the y origin of the fill pattern can depend on the y pos of the line.
				//We sometimes don't really know what the origin is, so the y adjustment compensates for that.
				//Width is a bit wider than height to arrive at an attractive proportion.
				drawingBrush.Viewport = new Rect(x:0,y:3.2,width:5,height:3.5);
				drawingBrush.ViewportUnits=BrushMappingMode.Absolute;
				//Viewbox:
				//Height is slightly taller than the drawing to avoid vertical wrapping.
				//Left and right sides are slightly cut off so that they exactly line up and create a smooth joint.
				//Arrived at by trial and error, and same regardless of the line thickness it's applied to.
				drawingBrush.Viewbox=new Rect(x:0.082,y:0,width:0.836,height:1.1);
				//drawingBrush2.Viewbox=new Rect(0.063,0,.874,1.1);//If we use thickness of 0.3, instead of 0.4
				drawingBrush.TileMode = TileMode.Tile;
				drawingBrush.Stretch=Stretch.Fill;
				drawingBrush.Drawing=new GeometryDrawing(brush:null,pen,pathGeometry);
				TextDecoration textDecoration=new TextDecoration();
				textDecoration.Pen=new Pen(drawingBrush,5);
				TextDecorationCollection textDecorationCollection=new TextDecorationCollection();
				textDecorationCollection.Add(textDecoration);
				_listTextRanges[i].ApplyPropertyValue(Inline.TextDecorationsProperty,textDecorationCollection);*/
			//}
			/*
			TextPointer textPointerContentStart=richTextBox.Document.ContentStart;
			TextPointer textPointerCaret=richTextBox.CaretPosition;
			int idxCaret=textPointerContentStart.GetOffsetToPosition(textPointerCaret);
			int lengthBack=textPointerCaret.GetTextRunLength(LogicalDirection.Backward);
			TextPointer textPointerWordStart=textPointerContentStart.GetPositionAtOffset(idxCaret-lengthBack);
			int lengthWord=textPointerWordStart.GetTextRunLength(LogicalDirection.Forward);
			TextPointer textPointerWordEnd=textPointerWordStart.GetPositionAtOffset(lengthWord);
			TextRange textRange=new TextRange(textPointerWordStart,textPointerWordEnd);
			textRange.ApplyPropertyValue(Inline.TextDecorationsProperty,TextDecorations.Underline);
				//adjust content start so that the setter is consistent
				while(true){
					if(textPointerContentStart.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text){
						break;
					}
					textPointerContentStart=textPointerContentStart.GetNextContextPosition(LogicalDirection.Forward);
				}
				int offset=textPointerContentStart.GetOffsetToPosition(textPointerStart);
				return offset*/
		}
		#endregion Methods - public

		#region Methods - private event handlers
		private void ContextMenu_Opened(object sender,RoutedEventArgs e) {
			if(SelectionLength==0) {
				menuItemCut.IsEnabled=false;
				menuItemCopy.IsEnabled=false;
			}
			else {
				menuItemCut.IsEnabled=true;
				menuItemCopy.IsEnabled=true;
			}
			TextPointer textPointer=richTextBox.CaretPosition;
			List<string> listSuggestions=new List<string>();
			for(int i=0;i<_listTextRangesMisspelled.Count;i++){
				if(!_listTextRangesMisspelled[i].Contains(textPointer)){
					continue;
				}
				listSuggestions=HunspellGlobal.Suggest(_listTextRangesMisspelled[i].Text);
				break;
			}
			if(IsUsingSpellCheck() && listSuggestions.Count>0){
				menuItemSuggest1.Visibility=Visibility.Visible;//1 is always visible
				menuItemSuggest2.Visibility=Visibility.Collapsed;
				menuItemSuggest3.Visibility=Visibility.Collapsed;
				menuItemSuggest4.Visibility=Visibility.Collapsed;
				menuItemSuggest5.Visibility=Visibility.Collapsed;
				if(listSuggestions.Count==0) {
					menuItemSuggest1.Header=Lans.g(this,"No Spelling Suggestions");
					
					menuItemSuggest1.IsEnabled=false;
				}
				if(listSuggestions.Count>=1){
					menuItemSuggest1.Header=listSuggestions[0];
					//menuItemSuggest1.Command=EditingCommands.CorrectSpellingError;
					//menuItemSuggest1.CommandParameter=listSuggestions[0];
					//menuItemSuggest1.CommandTarget=richTextBox;
					menuItemSuggest1.IsEnabled=true;
				}
				if(listSuggestions.Count>=2){
					menuItemSuggest2.Header=listSuggestions[1];
					menuItemSuggest2.Visibility=Visibility.Visible;
				}
				if(listSuggestions.Count>=3){
					menuItemSuggest3.Header=listSuggestions[2];
					menuItemSuggest3.Visibility=Visibility.Visible;
				}
				if(listSuggestions.Count>=4){
					menuItemSuggest4.Header=listSuggestions[3];
					menuItemSuggest4.Visibility=Visibility.Visible;
				}
				if(listSuggestions.Count>=5){
					menuItemSuggest5.Header=listSuggestions[4];
					menuItemSuggest5.Visibility=Visibility.Visible;
				}
				separator1.Visibility=Visibility.Visible;//displays whether or not there is a suggestion for the misspelled word
				menuItemAddToDict.Visibility=Visibility.Visible;
				menuItemDisableSpell.Visibility=Visibility.Visible;
				separator2.Visibility=Visibility.Visible;
			}
			else{
				menuItemSuggest1.Visibility=Visibility.Collapsed;
				menuItemSuggest2.Visibility=Visibility.Collapsed;
				menuItemSuggest3.Visibility=Visibility.Collapsed;
				menuItemSuggest4.Visibility=Visibility.Collapsed;
				menuItemSuggest5.Visibility=Visibility.Collapsed;
				separator1.Visibility=Visibility.Collapsed;
				menuItemAddToDict.Visibility=Visibility.Collapsed;
				menuItemDisableSpell.Visibility=Visibility.Collapsed;
				separator2.Visibility=Visibility.Collapsed;
			}
			if(HasAutoNotes) {
				menuItemInsertAN.Visibility=Visibility.Visible;
			}
			else {
				menuItemInsertAN.Visibility=Visibility.Collapsed;
			}
			if(FormattedTextAllowed) {
				menuItemPastePlain.Visibility=Visibility.Visible;
			}
			else {
				menuItemPastePlain.Visibility=Visibility.Collapsed;
			}
			string textCur=(SelectedText!=""?SelectedText:Text); //Edit Auto Note
			if(Regex.IsMatch(textCur,@"\[Prompt:""[a-zA-Z_0-9 ]+""\]")) {
				menuItemEditAN.Visibility=Visibility.Visible;
			}
			else {
				menuItemEditAN.Visibility=Visibility.Collapsed;
			}
			if(_listMenuItemsLinks==null) {
				_listMenuItemsLinks=new List<MenuItem>();
			}
			_listMenuItemsLinks.ForEach(x => contextMenu.Items.Remove(x));//remove items from previous pass.
			_listMenuItemsLinks.Clear();
			_listMenuItemsLinks.AddRange(PopupHelper2.GetContextMenuItemLinks(Text,RightClickLinks));
			if(_listMenuItemsLinks.Count>0) {
				separatorLinks.Visibility=Visibility.Visible;
				_listMenuItemsLinks.ForEach(x => contextMenu.Items.Add(x));
			}
			else{
				separatorLinks.Visibility=Visibility.Collapsed;
			}
		}

		private void _dispatcherTimer_Tick(object sender,EventArgs e) {
			if(!IsUsingSpellCheck()) {
				return;
			}
			_dispatcherTimer.Stop();
			SpellCheck();
		}

		private void menuItem_Click(object sender,System.EventArgs e) {
			MenuItem menuItem=(MenuItem)sender;
			TextPointer textPointer=richTextBox.CaretPosition;
			TextRange textRange=_listTextRangesMisspelled.Find(x=>x.Contains(textPointer));
			if(menuItem==menuItemSuggest1 && textRange!=null){
				textRange.Text=menuItemSuggest1.Header.ToString();
				return;
			}
			if(menuItem==menuItemSuggest2 && textRange!=null){
				textRange.Text=menuItemSuggest2.Header.ToString();
				return;
			}
			if(menuItem==menuItemSuggest3 && textRange!=null){
				textRange.Text=menuItemSuggest3.Header.ToString();
				return;
			}
			if(menuItem==menuItemSuggest4 && textRange!=null){
				textRange.Text=menuItemSuggest4.Header.ToString();
				return;
			}
			if(menuItem==menuItemSuggest5 && textRange!=null){
				textRange.Text=menuItemSuggest5.Header.ToString();
				return;
			}
			if(menuItem==menuItemAddToDict){
				if(!IsUsingSpellCheck()) {//if spell check disabled, break.  Should never happen since Add to Dict won't show if spell check disabled
					return;
				}
				if(textRange is null){
					return;//should never happen
				}
				//guaranteed to not already exist in custom dictionary, or it wouldn't be underlined.
				DictCustom dictCustom=new DictCustom();
				dictCustom.WordText=textRange.Text;
				DictCustoms.Insert(dictCustom);
				OpenDental.DataValid.SetInvalid(InvalidType.DictCustoms);
				SpellCheck();//to remove the red
				//_listTextRanges.Remove(textRange);
				_dispatcherTimer.Start();
				return;
			}
			if(menuItem==menuItemDisableSpell){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will disable spell checking.  To re-enable, go to Setup | Spell Check and check the \"Spell Check Enabled\" box.")) {
					return;
				}
				Prefs.UpdateBool(PrefName.SpellCheckIsEnabled,false);
				OpenDental.DataValid.SetInvalid(InvalidType.Prefs);
				SpellCheck();//to remove the red
				return;
			}
			if(menuItem==menuItemInsertDate){
				InsertDate();
				return;
			}
			if(menuItem==menuItemInsertQP){
				ShowQuickPaste();
				return;
			}
			if(menuItem==menuItemInsertAN) {
				AddAutoNote();
				return;
			}
			if(menuItem==menuItemCut) {
				richTextBox.Cut();
				return;
			}
			if(menuItem==menuItemCopy) {
				if(ReadOnly) {
					MsgBox.Show(this,"Not allowed because this text box is set to read-only.");
					return;
				}
				richTextBox.Copy();
				return;
			}
			if(menuItem==menuItemPaste) {
				if(FormattedTextAllowed) {
					richTextBox.Paste();
				}
				else {
					PastePlainText();
				}
				return;
			}
			if(menuItem==menuItemPastePlain){
				PastePlainText();
				return;
			}
			if(menuItem==menuItemEditAN){
				EditAutoNote();
				return;
			}
		}

		private void textBox_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter){
				if(!AllowsCarriageReturns) {
					e.Handled=true;
					return;
				}
			}
		}

		private void textBox_KeyUp(object sender,KeyEventArgs e) {
			if(IsUsingSpellCheck()) {//Only spell check if enabled
				_dispatcherTimer.Stop();
			}
			//Ctrl-Q
			if(e.Key==Key.Q && Keyboard.Modifiers==ModifierKeys.Control) {
				ShowQuickPaste();
			}
			if(IsUsingSpellCheck()) {//Only spell check if enabled
				_dispatcherTimer.Start();
			}
			if(QuickPasteType==EnumQuickPasteType.None){
				return;
				//this is mostly here to help me with testing when no db
			}
			List<QuickPasteCat> listQuickPasteCatsForType=QuickPasteCats.GetCategoriesForType(QuickPasteType).OrderBy(x => x.ItemOrder).ToList();
			List<QuickPasteNote> listQuickPasteNotes=QuickPasteNotes.GetForCats(listQuickPasteCatsForType);
			TextPointer textPointer=richTextBox.Document.ContentStart;
			while(true){
				if(textPointer is null){
					break;//end of content
				}
				if(textPointer.GetPointerContext(LogicalDirection.Forward)!=TextPointerContext.Text){
					textPointer=textPointer.GetNextContextPosition(LogicalDirection.Forward);
					continue;
				}
				//text run found.
				string run=textPointer.GetTextInRun(LogicalDirection.Forward);
				//process the run
				for(int i=0;i<listQuickPasteNotes.Count;i++) {
					if(listQuickPasteNotes[i].Abbreviation=="") {
						continue;
					}
					string pattern= @"(?<="//zero-width lookbehind 
						+@"^|[\W])"//Either the beginning or a non-word character such as whitespace or punctuation.
						+Regex.Escape("?"+listQuickPasteNotes[i].Abbreviation)//Example ?med (escaped to \?med). This handles anything like a $ that would otherwise be treated as a special character.
						+@"(?="//zero-width lookahead 
						+@"$|[\W])";//Either the end or a non-word character such as whitespace or punctuation.
					Match match=Regex.Match(run,pattern);
					if(!match.Success) {
						continue;
					}
					//match found. We will only process one match.
					TextPointer textPointerStart=textPointer.GetPositionAtOffset(match.Index);
					TextPointer textPointerEnd=textPointer.GetPositionAtOffset(match.Index+match.Length);
					TextRange textRange=new TextRange(textPointerStart,textPointerEnd);
					textRange.Text=listQuickPasteNotes[i].Note;
					//pointer are not preserved, so recalculate
					int idxEnd=match.Index+listQuickPasteNotes[i].Note.Length
						+Regex.Matches(listQuickPasteNotes[i].Note,@"\r\n").Count*2;//each CR adds 2 char. From \r\n to RunEnd,ParagraphEnd,ParagraphStart,RunStart is an increase of 2
					//if note is multiline, the above always works at the end, but if it's in the middle, there can be edge cases where it's off by a couple.
					//Not sure why, and it's no big deal.
					textPointerEnd=textPointer.GetPositionAtOffset(idxEnd);
					richTextBox.Selection.Select(textPointerEnd,textPointerEnd);
					return;
				}
				//jump to end of text
				textPointer=textPointer.GetPositionAtOffset(run.Length);
			}
		}

		private void textBox_LostFocus(object sender,RoutedEventArgs e) {
			
		}

		private void textBox_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//string name=Name;
			//Debug.WriteLine("GOT KBFocus: nested textBox within "+name);
			if(e.KeyboardDevice.IsKeyDown(Key.Tab)){
				((System.Windows.Controls.RichTextBox)sender).SelectAll();
			}
		}

		private void textBox_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//This happens in a number of situations:
			//1. The user presses Tab.
			//2. Right click for context menu
			//string name=Name;
			//Debug.WriteLine("LOST KBFocus: nested textBox within "+name);
			//SelectionStart=0;
			//SelectionLength=0;
			//TextPointer textPointer=richTextBox.Document.ContentStart;
			//((System.Windows.Controls.RichTextBox)sender).Selection.Select(textPointer,textPointer);
		}

		private void textBox_TextChanged(object sender,TextChangedEventArgs e) {
			TextChanged?.Invoke(this,new EventArgs());
		}

		private void This_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//This does fire when user clicks on textBox
			//Somehow, this is also currently firing on startup, even though I don't think I set it.
			//string name=Name;//for debugging
			if(richTextBox.IsFocused){
				//Debug.WriteLine("GOT KBFocus: "+name+". Nested textBox already has focus.");
				return;
			}
			//else{
			//	Debug.WriteLine("GOT KBFocus: "+name+". Setting focus on nested textBox.");
			//}
			_isChangingFocusToTextBox=true;
			bool isFocused=richTextBox.Focus();
			//if(isFocused){
			//	Debug.WriteLine("Setting focus was successful.");
			////}
			//else{
			//	Debug.WriteLine("Setting focus failed.");
			//}
		}

		private void This_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		private void This_Loaded(object sender,RoutedEventArgs e) {
			/*
			if(!IsUsingSpellCheck()){
				return;
			}
			string pathLex=System.IO.Path.GetTempPath()+_spellCheckFileLoc;//does not verify existence or security
			if(!File.Exists(pathLex)){
				CreateLex();
			}
			if(!File.Exists(pathLex)){
				return;
			}
			DateTime dateTimeLast=File.GetLastWriteTime(pathLex);
			TimeSpan timeSpan=DateTime.Now-dateTimeLast;
			if(timeSpan.Days>6){
				CreateLex();
			}
			IList iListDicts=SpellCheck.GetCustomDictionaries(richTextBox);
			Uri uri=new Uri("file:///"+pathLex);
			
			//uri=uri.MakeRelativeUri(uri);
			//new Uri(@"pack://siteoforigin:,,,"));
			int idxInsert=iListDicts.Add(uri);*/
		}
		
		private void This_LostFocus(object sender,RoutedEventArgs e) {
			if(_isChangingFocusToTextBox) {
				e.Handled=true;
				_isChangingFocusToTextBox=false;
			}
		}

		private void This_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//string name=Name;
			//Debug.WriteLine("LOST KBFocus: "+name);
		}

		private void This_PreviewMouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Click?.Invoke(this,new EventArgs());
		}
		#endregion Methods - private event handlers

		#region Methods - private
		/*
		private void CreateLex() {
			//This failed miserably.MS does not allow late creation of spelling files.They need to be compiled.
			string pathLex = System.IO.Path.GetTempPath()+_spellCheckFileLoc;//does not verify existence or security
			if(File.Exists(pathLex)) {
				try {
					File.Delete(pathLex);
				}
				catch {
					return;//we tried. Better to kick out now than to build a list from db that can't save.
				}
			}
			List<string> listWords = new List<string>();
			listWords.Add("asdf");
			StringBuilder stringBuilder = new StringBuilder();
			for(int i = 0;i<listWords.Count;i++) {
				stringBuilder.AppendLine(listWords[i]);
			}
			try {
				File.WriteAllText(pathLex,stringBuilder.ToString());//creates, writes, closes, overwrites. Can be empty.
			}
			catch {
				//do nothing
			}
		}*/

		///<summary>Add an auto note</summary>
		private void AddAutoNote() {
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			IsDlgOpen=true;
			frmAutoNoteCompose.ShowDialog();
			IsDlgOpen=false;
			if(frmAutoNoteCompose.IsDialogOK) {
				SelectedText=frmAutoNoteCompose.StrCompletedNote;
			}
		}

		///<summary>Takes an index within the plaintext string and converts it to an index within the RFT. Accounts for paragraphs and single runs within each paragraph, but not for any further formatting. See discussion at top of this file.</summary>
		private int ConvertIdxFromPlain(int idx){
			//count number of paragraph crossings prior to this idx
			if(idx>Text.Length){//Example Text length is 4, so valid settings are 0 through 4, where 4 is after the last char.
				//5>4 which is not allowed. But 0-4 are allowed.
				return 0;
			}
			string plainTextPrior=Text.Substring(0,idx);
			int count=plainTextPrior.Count(x=>x=='\r');
			return idx+count*2;//from \r\n to RunEnd,ParagraphEnd,ParagraphStart,RunStart is an increase of 2
		}

		///<summary></summary>
		private TextPointer ConvertLengthFromPlain(TextPointer textPointer,int lengthPlain){
			//scan from textPointer, compensating for paragraphs as we go.
			int countScanned=0;
			while(true){
				if(textPointer.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text){
					string textRun=textPointer.GetTextInRun(LogicalDirection.Forward);
					if(countScanned+textRun.Length>=lengthPlain){//example: 10+5>=12
						textPointer=textPointer.GetPositionAtOffset(lengthPlain-countScanned);//example: 12-10=2
						return textPointer;
					}
					countScanned+=textRun.Length;
				}
				if(textPointer.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.ElementEnd){
					if(textPointer.Parent is Paragraph) {
						//this is a paragraph boundary
						countScanned+=2;// for the \r\n
					}
				}
				if(countScanned>=lengthPlain){
					break;
				}
				//because the original textPointer was passed in by value, this does not affect the original. It's new each time.
				textPointer = textPointer.GetNextContextPosition(LogicalDirection.Forward);
			}
			return textPointer;
		}

		///<summary>Takes an index within the RTF and converts it to an index within the plaintext string. Accounts for paragraphs and single runs within each paragraph, but not for any further formatting. See discussion at top of this file.</summary>
		private int ConvertIdxToPlain(TextPointer textPointerStart,int idxTextPointer){
			//Don't need to break into paragraphs.
			TextPointer textPointerEnd=textPointerStart.GetPositionAtOffset(idxTextPointer);
			int countParagraphBoundaries=0;
			while(true){
				if(textPointerStart.CompareTo(textPointerEnd) >= 0){
					break;
				}
				if(textPointerStart.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.ElementEnd){
					if(textPointerStart.Parent is Paragraph) {
						countParagraphBoundaries++;
					}
				}
				textPointerStart = textPointerStart.GetNextContextPosition(LogicalDirection.Forward);
			}
			return idxTextPointer-countParagraphBoundaries*2;//from RunEnd,ParagraphEnd,ParagraphStart,RunStart to \r\n is a decrease of 2
		}

		///<summary></summary>
		private void EditAutoNote() {
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			if(SelectedText==""){
				frmAutoNoteCompose.StrMainTextNote=Text;
			}
			else{
				frmAutoNoteCompose.StrMainTextNote=SelectedText;
			}
			IsDlgOpen=true;
			frmAutoNoteCompose.ShowDialog();
			IsDlgOpen=false;
			if(!frmAutoNoteCompose.IsDialogOK) {
				return;
			}
			if(SelectedText==""){
				Text=frmAutoNoteCompose.StrCompletedNote;
				SelectionStart=Text.Length;
				return;
			}
			SelectedText=frmAutoNoteCompose.StrCompletedNote;
		}

		private void InsertDate(){
			//Replacing the text is easy, but we also need the caret to be set to the end of the new text.
			//That's harder. To simplify, we will ignore the situation where user has highlighted some text that gets replaced.
			TextSelection textSelection=richTextBox.Selection;
			TextPointer textPointerStart=textSelection.Start;
			string strDate=DateTime.Today.ToShortDateString();
			richTextBox.Selection.Text=strDate;
			//TextPointers are copies/Selections, not references.
			//So we have to get them again.
			textSelection=richTextBox.Selection;
			textPointerStart=textSelection.Start;
			TextPointer textPointerEnd=textPointerStart.GetPositionAtOffset(strDate.Length,LogicalDirection.Forward);
			richTextBox.Selection.Select(textPointerEnd,textPointerEnd);
		}

		///<summary>This gets passed as an Action to FrmQuickPaste.</summary>
		private void QuickPasteInsertValue(string strPaste) {
			//Nearly all the work here is putting the caret in the correct position afterward.
			TextSelection textSelection=richTextBox.Selection;
			TextPointer textPointerStart=textSelection.Start;
			richTextBox.Selection.Text=strPaste;
			//TextPointers/Selections are copies, not references.
			//So we have to get them again.
			textSelection=richTextBox.Selection;
			textPointerStart=textSelection.Start;
			//below is inspired by the code in textBox_KeyUp
			int lengthPaste=strPaste.Length
				+Regex.Matches(strPaste,@"\r\n").Count*2;//each CR adds 2 char. From \r\n to RunEnd,ParagraphEnd,ParagraphStart,RunStart is an increase of 2
			TextPointer textPointerEnd=textPointerStart.GetPositionAtOffset(lengthPaste,LogicalDirection.Forward);
			richTextBox.Selection.Select(textPointerEnd,textPointerEnd);
		}

		private bool IsUsingSpellCheck(){
			//if(ODBuild.IsDebug()) {
			//	if(Environment.MachineName.ToLower()=="jordansgalaxybk" || Environment.MachineName.ToLower()=="jordanhome" || Environment.MachineName.ToLower()=="jordancryo") {
			//		return true;//for some testing without a db
			//	}
			//}
			if(!SpellCheckIsEnabled){//for this control, as set in the designer
				return false;
			}
			return PrefC.GetBool(PrefName.SpellCheckIsEnabled);
		}

		///<summary>Pastes the contents of the clipboard, excluding the formatting.</summary>
		private void PastePlainText() {
			string text = Clipboard.GetText();
			SelectedText=text;
		}

		///<summary></summary>
		private void SetColors(){
			//Nothing to do here.  The textbox that this control wraps already turns gray on is own.
		}

		private void ShowQuickPaste() {
			FrmQuickPaste frmQuickPaste=new FrmQuickPaste();
			frmQuickPaste.IsSelectionMode=true;
			frmQuickPaste.ActionInsertVal=QuickPasteInsertValue;
			frmQuickPaste.QuickPasteType_=_quickPasteType;
			IsDlgOpen=true;
			frmQuickPaste.ShowDialog();
			IsDlgOpen=false;
		}

		///<summary>Throws exception if any text formatting is present. See discussion at top of file.</summary>
		private void ThrowExceptionIfHasFormatting(){
			//Not sure if this is really the best approach
			//Our spelling underlines are also a form of formatting.
			/*
			FlowDocument flowDocument=richTextBox.Document;
			BlockCollection blockCollection=flowDocument.Blocks;
			//Blocks include paragraph, section, list, table.
			for(int i=0;i<blockCollection.Count;i++){
				if(blockCollection.ElementAt(i).GetType()!=typeof(Paragraph)){
					//We only support paragraph. All others throw exception.
					throw new Exception("Paragraphs are the only type of content allowed.");
				}
			}
			for(int b=0;b<blockCollection.Count;b++){
				Paragraph paragraph=(Paragraph)blockCollection.ElementAt(b);
				//should contain exactly zero or one run of text
				if(paragraph.Inlines.Count==0){
					continue;
				}
				if(paragraph.Inlines.Count>1){
					throw new Exception("Text formatting symbol found.");
				}
				Inline inline=paragraph.Inlines.ElementAt(0);
				if(inline.GetType()!=typeof(Run)){
					throw new Exception("Text formatting symbol found.");
				}
				Run run=(Run)inline;
				TextPointer textPointer=run.ContentStart;
				TextPointer textPointerEnd=run.ContentEnd;
				while(true){
					TextPointerContext textPointerContext=textPointer.GetPointerContext(LogicalDirection.Forward);
					//DependencyObject dependencyObject=textPointer.GetAdjacentElement(LogicalDirection.Forward);
					if(textPointerContext==TextPointerContext.Text){
						//tests one symbol/run at a time. This section is poorly written because there's only one run, but it acts like there's more. But it works.
						textPointer=textPointer.GetNextContextPosition(LogicalDirection.Forward);
						continue;
					}
					//non-text found
					int offset=textPointer.GetOffsetToPosition(textPointerEnd);
					if(offset==0){
						break;
					}
					throw new Exception("Text formatting symbol found.");
				}
			}*/
		}
		#endregion Methods - private

		//#region Classes nested
		//private class Word{
			//The only reason for this class is if we want to bundle the TextRange with the result of spell check
			//But we might not need to do that except for unit testing.
			//We can probably just attach the decoration to the TextRange as we spell check.
			//We can always derive the word starting and ending index from the TextRange itself. Maybe not. See top of file.
			//When we right click, we can just spell check it again.
			//public TextRange TextRange_;
			//public bool IsSpellingCorrect;
		//}
		//#endregion Classes nested
	}
}

//Todo: Test this with FormPopupEdit.