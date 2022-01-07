using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using NHunspell;
using System.Text.RegularExpressions;
using OpenDental.UI;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDental {
	/// <summary>This is used instead of a regular textbox when quickpaste functionality is needed.</summary>
	public class ODtextBox:RichTextBox {//System.ComponentModel.Component

		#region IMM (Input Method Manager) dll import for IME mid composition bug (Korea and east Asian languages)

		//See post by Jon Burchel - Microsoft: https://goo.gl/d1ehJb  (an MSDN forum post shortened using google url shortener)

		[DllImport("imm32.dll",CharSet = CharSet.Unicode)]
		public static extern IntPtr ImmReleaseContext(IntPtr hWnd,IntPtr context);

		[DllImport("imm32.dll",CharSet = CharSet.Unicode)]
		private static extern int ImmGetCompositionString(IntPtr hIMC,uint dwIndex,byte[] lpBuf,int dwBufLen);

		[DllImport("imm32.dll",CharSet = CharSet.Unicode)]
		private static extern IntPtr ImmGetContext(IntPtr hWnd);

		#endregion

		private System.Windows.Forms.ContextMenu contextMenu;
		private IContainer components;// Required designer variable.
		private static Hunspell HunspellGlobal;//We create this object one time for every instance of this textbox control within the entire program.
		private QuickPasteType quickPasteType;
		private Graphics BufferGraphics;
		public Timer timerSpellCheck;
		private Point PositionOfClick;
		public MatchOD ReplWord;
		private bool _spellCheckIsEnabled;//set to true in constructor
		private bool _editMode;//set to false in constructor
		private Point textEndPoint;
		///<summary>Only used when ImeCompositionCompatibility is enabled.  Set to true when the user presses the space bar.
		///This will cause the cursor to move to the next position and no longer have composition affect the current character.
		///E.g. the Korean symbol '역' (dur) will display.  However, typing '여' (du) and then space will cause that char to no longer be affected.
		///This will allow the char 'ㄱ' (r) to appear after '여' instead of '역'.</summary>
		private bool _skipImeComposition=false;
		///<summary>Only used when ImeCompositionCompatibility is enabled.  Set to true when the user is in the middle of composing a symbol.
		///This will cause the cursor to stay over the current character and not move on (or separate) the current symbol being constructed.
		///E.g. the Korean symbol '역' (dur) will not display correctly without this set to true.  
		///When false, it will be broken apart into each character that comprizes it: 'ㅇ ㅕ ㄱ ' (d u r)</summary>
		private bool _imeComposing=false;
		///<summary>Always contains the text that should be displayed in the rich text box.  
		///Also used to store the UNICODE representation of the RichTextBox.Text property (which we override) due to a Korean bug.</summary>
		private string _msgText="";
		///<summary>Pointer to the Rich Edit version 4.1 dll.  Null unless the property () is set to true and the new version is loaded.
		///The new dll is named Msftedit.dll and the ClassName is changed from RichEdit20W to RichEdit50W.
		///The new dll has been available since Windows XP SP1, released in 2002.  .NET is just set to use the old dll by default.</summary>
		private static IntPtr _libPtr;
		private bool isImeComposition;
		private bool _detectLinksEnabled;
		///<summary>Must track menuitems that we have added, so that we know which ones to take away when reconstructing context menu.</summary>
		private bool _hasAutoNotes;
		private List<MenuItem> _listMenuItemLinks;
		///<summary>Stores the current words for all odtextboxes and if they are spelled correctly.  Speeds up spell checking.
		///Words cannot be removed from the spelling dictionary and thus also cannot be removed from this dictionary.
		///Is a concurrent dictionary so multiple odtextboxes can add words at the same time safely.</summary>
		private static ConcurrentDictionary<string,SpellingType> _dictAllWords=new ConcurrentDictionary<string,SpellingType>();

		[Category("OD"), Description("Set true to enable context menu to detect links.")]
		[DefaultValue(true)]
		public bool DetectLinksEnabled {
			get {
				return _detectLinksEnabled;
			}
			set {
				_detectLinksEnabled=value;
			}
		}

		[Category("OD")]
		[Description("Set to false to prevent the user from entering carriage returns.")]
		[DefaultValue(true)]
		public bool AllowsCarriageReturns { get; set; }=true;

		///<summary>Set to true to enable context menu to detect PatNum links. This should not be set true for text boxes in modal windows because it can select a different patient, resulting in a potentially unsafe state.</summary>
		[Category("OD")]
		[Description("Set to true to enable context menu to detect PatNum links. This should not be set true for text boxes in modal windows.")]
		[DefaultValue(false)]
		public bool DoShowPatNumLinks { get; set; }=false;

		///<summary>Set to true to enable context menu to detect TaskNum links. This should not be set true for modal windows because it will allow the user to interact with the Task window and select a different patient via the "Go To" button, resulting in a potentially unsafe state.</summary>
		[Category("OD")]
		[Description("Set to true to enable context menu to detect TaskNum links. This should not be set true for text boxes in modal windows.")]
		[DefaultValue(false)]
		public bool DoShowTaskNumLinks { get; set; }=false;

		///<summary>Set true to enable spell checking in this control.</summary>
		[Category("OD"),Description("Set true to enable spell checking.")]
		[DefaultValue(true)]
		public bool SpellCheckIsEnabled {
			get {
				return _spellCheckIsEnabled;
			}
			set {
				_spellCheckIsEnabled=value;
			}
		}

		///<summary>Set true to enable formatted text to paste in this control.</summary>
		[Category("OD"),Description("Set true to allow edit mode formatting")]
		[DefaultValue(false)]
		public bool EditMode {
			get {
				return _editMode;
			}
			set {
				_editMode=value;
			}
		}

		///<summary>Set to true to enable context menu option to insert auto notes.</summary>
		[Category("OD"), Description("Set to true to enable context menu option to insert auto notes.")]
		[DefaultValue(false)]
		public bool HasAutoNotes {
			get {
				return _hasAutoNotes;
			}
			set {
				_hasAutoNotes=value;
			}
		}

		///<summary>Set true to enable the newer version 4.1 RichEdit library.</summary>
		[Category("OD"), Description("Set true to enable RichEdit version 4.1 enhanced features.")]
		[DefaultValue(false)]
		public bool RichEdit4IsEnabled { get; set; }

		[DllImport("kernel32.dll",EntryPoint="LoadLibrary",CharSet=CharSet.Auto,SetLastError=true)]
		private static extern IntPtr LoadLibrary(string fileName);

		///<summary>By default .NET uses the old library, riched20.dll, which corresponds to Rich Edit versions 2.0 and 3.0.
		///As of Windows XP SP1 (2002 release date), the newer library, msftedit.dll, is included which corresponds to Rich Edit version 4.1.
		///This method attempts to load the newer library, with the enhanced features, and sets the ClassName accordingly.
		///If msftedit.dll is not found, the original default library is used.
		///The msftedit.dll library is only loaded if the libPtr==IntPtr.Zero to prevent memory leaks.</summary>
		protected override CreateParams CreateParams {
			get {
				CreateParams cParams=base.CreateParams;
				if(!RichEdit4IsEnabled) {
					return cParams;
				}
				try {
					if(_libPtr==IntPtr.Zero) {//only try to load the library if not loaded already.
						_libPtr=LoadLibrary("msftedit.dll");
					}
					if(_libPtr==IntPtr.Zero) {//still zero, library was not loaded successfully
						return cParams;
					}
					cParams.ClassName="RichEdit50W";//old ClassName: "RichEdit20W" new ClassName: "RichEdit50W"
				}
				catch(Exception) {
					//msftedit.dll must not exist, or LoadLibrary wasn't loaded, so simply return the base.CreateParams unaltered
				}
				return cParams;
			}
		}

		/*public ODtextBox(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

		}*/

		///<summary></summary>
		public ODtextBox() {
			//We have to try catch this just in case an ODTextBox is shown before upgrading to a version that already has this preference.
			try {
				if(System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime
					|| this.DesignMode || !Db.HasDatabaseConnection()) 
				{
					isImeComposition=false;
				}
				else {
					isImeComposition=PrefC.GetBool(PrefName.ImeCompositionCompatibility);
				}
			}
			catch(Exception ex) {
				ex.DoNothing();//Do nothing.  Just treat the ODTextBox like it always has (no composition support).
			}
			InitializeComponent();// Required for Windows.Forms Class Composition Designer support
			_spellCheckIsEnabled=true;
			this.AcceptsTab=true;//Causes CR to not also trigger OK button on a form when that button is set as AcceptButton on the form.
			this.DetectUrls=false;
			if(System.ComponentModel.LicenseManager.UsageMode!=System.ComponentModel.LicenseUsageMode.Designtime
				&& HunspellGlobal==null) {
				if(ODBuild.IsDebug()) {
					try {
						HunspellGlobal=new Hunspell(Properties.Resources.en_US_aff,Properties.Resources.en_US_dic);
					}
					catch(Exception ex) {
						ex.DoNothing();
						System.IO.File.Copy(@"..\..\..\Required dlls\Hunspellx64.dll","Hunspellx64.dll");
						System.IO.File.Copy(@"..\..\..\Required dlls\Hunspellx86.dll","Hunspellx86.dll");
						HunspellGlobal=new Hunspell(Properties.Resources.en_US_aff,Properties.Resources.en_US_dic);
					}
				}
				else {
					HunspellGlobal=new Hunspell(Properties.Resources.en_US_aff,Properties.Resources.en_US_dic);
				}
			}
			EventHandler onClick=new EventHandler(menuItem_Click);
			contextMenu.MenuItems.Add("",onClick);//These five menu items will hold the suggested spelling for misspelled words.  If no misspelled words, they will not be visible.
			contextMenu.MenuItems.Add("",onClick);
			contextMenu.MenuItems.Add("",onClick);
			contextMenu.MenuItems.Add("",onClick);
			contextMenu.MenuItems.Add("",onClick);
			contextMenu.MenuItems.Add("-");
			contextMenu.MenuItems.Add(Lan.g(this,"Add to Dictionary"),onClick);
			contextMenu.MenuItems.Add(Lan.g(this,"Disable Spell Check"),onClick);
			contextMenu.MenuItems.Add("-");
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Insert Date"),onClick,Shortcut.CtrlD));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Insert Quick Paste Note"),onClick,Shortcut.CtrlQ));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Insert Auto Note"),onClick));
			contextMenu.MenuItems.Add("-");
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Cut"),onClick,Shortcut.CtrlX));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Copy"),onClick,Shortcut.CtrlC));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Paste"),onClick,Shortcut.CtrlV));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Paste Plain Text"),onClick));
			contextMenu.MenuItems.Add(new MenuItem(Lan.g(this,"Edit Auto Note"),onClick));
			base.BackColor=SystemColors.Window;//Needed for OnReadOnlyChanged() to change backcolor when ReadOnly because of an issue with RichTextBox.
		}

		///<summary>Clean up any resources being used.</summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
				if(BufferGraphics!=null) {//Dispose before bitmap.
					BufferGraphics.Dispose();
					BufferGraphics=null;
				}
				//We do not dispose the hunspell object because it will be automatially disposed of when the program closes.
			}
			base.Dispose(disposing);
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.timerSpellCheck = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.Popup += new System.EventHandler(this.contextMenu_Popup);
			// 
			// timerSpellCheck
			// 
			this.timerSpellCheck.Interval = 500;
			this.timerSpellCheck.Tick += new System.EventHandler(this.timerSpellCheck_Tick);
			// 
			// ODtextBox
			// 
			this.ContextMenu = this.contextMenu;
			this.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.ODtextBox_ContentsResized);
			this.VScroll += new System.EventHandler(this.ODtextBox_VScroll);
			this.ResumeLayout(false);

		}
		#endregion

		///<summary>Statuses sent to an application when IME is doing 'composition' for a symbol.
		///See post by Jon Burchel - Microsoft: https://goo.gl/d1ehJb  (an MSDN forum post shortened using google url shortener)</summary>
		private enum WM_IME {
			GCS_RESULTSTR=0x800,
			EM_STREAMOUT=0x044A,
			WM_IME_COMPOSITION=0x10F,
			WM_IME_ENDCOMPOSITION=0x10E,
			WM_IME_STARTCOMPOSITION=0x10D
		}

		protected override void WndProc(ref Message m) {
			//The following code fixes a bug deep down in RichTextBox for foreign users that have a language that needs to use composition symbols.
			//See post by Jon Burchel - Microsoft: https://goo.gl/d1ehJb  (an MSDN forum post shortened using google url shortener)
			if(isImeComposition) {
				switch(m.Msg) {
					case (int)WM_IME.EM_STREAMOUT:
						if(_imeComposing) {
							_skipImeComposition=true;
						}
						base.WndProc(ref m);
						break;
					case (int)WM_IME.WM_IME_COMPOSITION:
						if(m.LParam.ToInt32()==(int)WM_IME.GCS_RESULTSTR) {
							IntPtr hImm=ImmGetContext(this.Handle);
							int dwSize=ImmGetCompositionString(hImm,(int)WM_IME.GCS_RESULTSTR,null,0);
							byte[] outstr=new byte[dwSize];
							ImmGetCompositionString(hImm,(int)WM_IME.GCS_RESULTSTR,outstr,dwSize);
							_msgText+=Encoding.Unicode.GetString(outstr).ToString();
							ImmReleaseContext(this.Handle,hImm);
						}
						if(_skipImeComposition) {
							_skipImeComposition=false;
							break;
						}
						base.WndProc(ref m);
						break;
					case (int)WM_IME.WM_IME_STARTCOMPOSITION:
						_imeComposing=true;
						base.WndProc(ref m);
						break;
					case (int)WM_IME.WM_IME_ENDCOMPOSITION:
						_imeComposing=false;
						base.WndProc(ref m);
						break;
					default:
						base.WndProc(ref m);
						break;
				}
			}
			else {//End IME check.
				base.WndProc(ref m);
			}
		}


		protected override void OnReadOnlyChanged(EventArgs e) {
			base.OnReadOnlyChanged(e);
			//Richtextbox does not redraw the textbox with grey after turning it ReadOnly, so we do this to immitate how textbox works.
			if(ReadOnly){
				base.BackColor=SystemColors.Control;
			}
			else{
				base.BackColor=SystemColors.Window;
			}
		}

		public override string Text {
			get {
				if(!_imeComposing) {
					_msgText=base.Text;
					return base.Text;
				}
				else {
					return _msgText;
				}
			}
			set {
				_msgText=value;
				base.Text=value;
			}
		}

		///<summary></summary>
		[Category("Behavior"),Description("This will determine which category of Quick Paste notes opens first.")]
		public QuickPasteType QuickPasteType {
			get {
				return quickPasteType;
			}
			set {
				quickPasteType=value;
				if(value==QuickPasteType.None) {
					throw new InvalidEnumArgumentException("A value for the QuickPasteType property must be set.");
				}
			}
		}

		private void contextMenu_Popup(object sender,System.EventArgs e) {
			if(SelectionLength==0) {
				contextMenu.MenuItems[13].Enabled=false;//cut
				contextMenu.MenuItems[14].Enabled=false;//copy
			}
			else {
				contextMenu.MenuItems[13].Enabled=true;
				contextMenu.MenuItems[14].Enabled=true;
			}
			if(!this._spellCheckIsEnabled
			  || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)
			  || !IsOnMisspelled(PositionOfClick)) {//did not click on a misspelled word OR spell check is disabled
				contextMenu.MenuItems[0].Visible=false;//suggestion 1
				contextMenu.MenuItems[1].Visible=false;//suggestion 2
				contextMenu.MenuItems[2].Visible=false;//suggestion 3
				contextMenu.MenuItems[3].Visible=false;//suggestion 4
				contextMenu.MenuItems[4].Visible=false;//suggestion 5
				contextMenu.MenuItems[5].Visible=false;//contextMenu separator
				contextMenu.MenuItems[6].Visible=false;//Add to Dictionary
				contextMenu.MenuItems[7].Visible=false;//Disable Spell Check
				contextMenu.MenuItems[8].Visible=false;//separator
			}
			else if(this._spellCheckIsEnabled
			  && PrefC.GetBool(PrefName.SpellCheckIsEnabled)
			  && IsOnMisspelled(PositionOfClick)) {//clicked on or near a misspelled word AND spell check is enabled
				List<string> suggestions=SpellSuggest();
				if(suggestions.Count==0) {//no suggestions
					contextMenu.MenuItems[0].Text=Lan.g(this,"No Spelling Suggestions");
					contextMenu.MenuItems[0].Visible=true;
					contextMenu.MenuItems[0].Enabled=false;//suggestion 1 set to "No Spelling Suggestions"
					contextMenu.MenuItems[1].Visible=false;//suggestion 2
					contextMenu.MenuItems[2].Visible=false;//suggestion 3
					contextMenu.MenuItems[3].Visible=false;//suggestion 4
					contextMenu.MenuItems[4].Visible=false;//suggestion 5
				}
				else {//must be on misspelled word and spell check is enabled globally and locally
					for(int i=0;i<5;i++) {//Only display first 5 suggestions if available
						if(i>=suggestions.Count) {
							contextMenu.MenuItems[i].Visible=false;
							continue;
						}
						contextMenu.MenuItems[i].Text=suggestions[i];
						contextMenu.MenuItems[i].Visible=true;
						contextMenu.MenuItems[i].Enabled=true;
					}
				}
				contextMenu.MenuItems[5].Visible=true;//contextMenu separator, will display whether or not there is a suggestion for the misspelled word
				contextMenu.MenuItems[6].Visible=true;//Add to Dictionary
				contextMenu.MenuItems[7].Visible=true;//Disable Spell Check
				contextMenu.MenuItems[8].Visible=true;//contextMenu separator
			}
			if(HasAutoNotes) {
				contextMenu.MenuItems[11].Visible=true;//Insert Auto Note
				contextMenu.MenuItems[11].Enabled=true;
			}
			else {
				contextMenu.MenuItems[11].Visible=false;
				contextMenu.MenuItems[11].Enabled=false;
			}
			if(EditMode) {
				contextMenu.MenuItems[16].Visible=true;//Paste Plain Text
				contextMenu.MenuItems[16].Enabled=true;
			}
			else {
				contextMenu.MenuItems[16].Visible=false;//Paste Plain Text
				contextMenu.MenuItems[16].Enabled=false;
			}
			string textCur=(SelectedText!=""?SelectedText:Text); //Edit Auto Note
			if(Regex.IsMatch(textCur,@"\[Prompt:""[a-zA-Z_0-9 ]+""\]")) {
				contextMenu.MenuItems[17].Visible=true;
				contextMenu.MenuItems[17].Enabled=true;
			}
			else {
				contextMenu.MenuItems[17].Visible=false;
				contextMenu.MenuItems[17].Enabled=false;
			}
			if(_listMenuItemLinks==null) {
				_listMenuItemLinks=new List<MenuItem>();
			}
			_listMenuItemLinks.ForEach(x => contextMenu.MenuItems.Remove(x));//remove items from previous pass.
			_listMenuItemLinks.Clear();
			_listMenuItemLinks.Add(new MenuItem("-"));
			_listMenuItemLinks.AddRange(PopupHelper.GetContextMenuItemLinks(Text,DoShowPatNumLinks,DoShowTaskNumLinks));
			if(_listMenuItemLinks.Any(x => x.Text!="-")) {//at least one REAL menu item that is not the divider.
				_listMenuItemLinks.ForEach(x => contextMenu.MenuItems.Add(x));
			}
		}

		///<summary>Determines whether the right click was on a misspelled word.  Also sets the start and end index of chars to be replaced in text.</summary>
		private bool IsOnMisspelled(Point PositionOfClick) {
			int charIndex=this.GetCharIndexFromPosition(PositionOfClick);
			Point charLocation=this.GetPositionFromCharIndex(charIndex);
			if(PositionOfClick.Y<charLocation.Y-2 || PositionOfClick.Y>charLocation.Y+this.FontHeight+2) {//this is the closest char but they were not very close when they right clicked
				return false;
			}
			char c=this.GetCharFromPosition(PositionOfClick);
			if(c=='\n') {//if closest char is a new line char, then assume not on a misspelled word
				return false;
			}
			List<MatchOD> words=GetWords(Text);
			words=words.Where(x => x.StartCharIndex<=charIndex && x.StartCharIndex+x.Value.Length-1>=charIndex).ToList();
			if(words.Count==0) {
				return false;
			}
			ReplWord=words.FirstOrDefault();
			if(ReplWord==null) {
				return false;
			}
			SpellingType spellType;
			if(_dictAllWords.TryGetValue(ReplWord.Value,out spellType) && spellType==SpellingType.Incorrect) {
				return true;
			}
			return false;
		}

		private List<string> SpellSuggest() {
			List<string> suggestions=HunspellGlobal.Suggest(ReplWord.Value);
			return suggestions;
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			if(!this.Focused) {
				this.Focus();
			}
			base.OnMouseDown(e);
			PositionOfClick=new Point(e.X,e.Y);
		}

		private void menuItem_Click(object sender,System.EventArgs e) {
			if(ReadOnly && contextMenu.MenuItems.IndexOf((MenuItem)sender)!=14) {//14 = copy.
				MsgBox.Show(this,"This feature is currently disabled due to this text box being read only.");
				return;
			}
			switch(contextMenu.MenuItems.IndexOf((MenuItem)sender)) {
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
					if(!this._spellCheckIsEnabled || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//if spell check disabled, break.  Should never happen since the suggested words won't show if spell check disabled
						break;
					}
					int originalCaret=this.SelectionStart;
					if(ReplWord==null) {
						break;//We have bug submissions for this scenario, but haven't been able to reproduce.  Null means we don't know what word to replace.
					}
					this.SelectionStart=ReplWord.StartCharIndex;
					this.SelectionLength=ReplWord.Value.Length;
					this.SelectedText=((MenuItem)sender).Text;
					if(this.Text.Length<=originalCaret) {
						this.SelectionStart=this.Text.Length;
					}
					else {
						this.SelectionStart=originalCaret;
					}
					timerSpellCheck.Start();
					break;
				//case 5 is separator
				case 6://Add to dict
					if(!this._spellCheckIsEnabled || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//if spell check disabled, break.  Should never happen since Add to Dict won't show if spell check disabled
						break;
					}
					string newWord=ReplWord.Value;
					//guaranteed to not already exist in custom dictionary, or it wouldn't be underlined.
					DictCustom word=new DictCustom();
					word.WordText=newWord;
					DictCustoms.Insert(word);
					DataValid.SetInvalid(InvalidType.DictCustoms);
					//When we add a word to the custom dictionary, it is now correct.  We need to update our storage dict for spellcheck.
					_dictAllWords[newWord.ToLower()]=SpellingType.Custom;
					timerSpellCheck.Start();
					break;
				case 7://Disable spell check
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will disable spell checking.  To re-enable, go to Setup | Spell Check and check the \"Spell Check Enabled\" box.")) {
						break;
					}
					Prefs.UpdateBool(PrefName.SpellCheckIsEnabled,false);
					DataValid.SetInvalid(InvalidType.Prefs);
					ClearWavyLines();
					break;
				//case 8 is separator
				case 9:
					InsertDate();
					break;
				case 10://Insert Quick Paste Note
					ShowFullDialog();
					break;
				case 11://Insert Auto Note
					AddAutoNote();
					break;
				//case 12 is seperator
				case 13://cut
					base.Cut();
					break;
				case 14://copy
					base.Copy();
					break;
				case 15://paste
					if(EditMode) {
						PasteText();
					}
					else {
						PastePlainText();
					}
					break;
				case 16://paste plain text
					PastePlainText();
					break;
				case 17://Edit AutoNote
					EditAutoNote();
					break;
			}
		}

		///<summary>Pastes the content of the clipboard including the formatting.</summary>
		private void PasteText() {
			base.Paste();
		}

		///<summary>Pastes the content of the clipboard excluding the formatting.</summary>
		private void PastePlainText() {
			SelectionFont=Font;
			SelectionColor=ForeColor;
			SelectionBackColor=Color.Transparent;
			SelectionBullet=false;
			base.Paste(DataFormats.GetFormat("Text"));
		}

		///<summary></summary>
		private void EditAutoNote() {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.MainTextNote=SelectedText!=""?SelectedText:Text;
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				if(SelectedText!=""){
					SelectedText=FormA.CompletedNote;
				}
				else {
					Text=FormA.CompletedNote;
					SelectionStart=Text.Length;
				}
			}
		}

		///<summary>Add an auto note</summary>
		private void AddAutoNote() {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				SelectedText=FormA.CompletedNote;
			}
		}

		private void timerSpellCheck_Tick(object sender,EventArgs e) {
			if(!this._spellCheckIsEnabled || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//if spell check disabled, return
				return;
			}
			timerSpellCheck.Stop();
			SpellCheck();
		}

		private void ODtextBox_VScroll(object sender,EventArgs e) {
			if(!this._spellCheckIsEnabled || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//if spell check disabled, return
				return;
			}
			timerSpellCheck.Stop();
			timerSpellCheck.Start();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter){
				if(!AllowsCarriageReturns) {
					e.Handled=true;
					return;
				}
			}
			base.OnKeyDown(e);
			if(!this._spellCheckIsEnabled || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//if spell check disabled, return
				return;
			}
			//The lines were shifted due to new input. This causes the location of the red wavy underline to shift down as well, so clear them.
			if(e.KeyCode==Keys.Enter) {
				ClearWavyLines();
			}
		}

		///<summary>When the contents of the text box is resized, e.g. when word wrap creates a new line, clear red wavy lines so they don't shift down.</summary>
		private void ODtextBox_ContentsResized(object sender,ContentsResizedEventArgs e) {
			try {
				if(DesignMode || !this._spellCheckIsEnabled || !PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//if spell check disabled, return
					return;
				}
			}
			catch {
				//This can only happen if designing and DesignMode is false for some reason.  Has happened when this control is two levels deep.
				//The exception happens in PrefC.GetBool() because there is no database connection in design time.
				return;
			}
			Point textEndPointCur=this.GetPositionFromCharIndex(Text.Length-1);
			if(textEndPoint==new Point(0,0)) {
				textEndPoint=textEndPointCur;
				return;
			}
			if(textEndPointCur.Y!=textEndPoint.Y) {//textEndPoint cannot be null, if not set it defaults to 0,0
				ClearWavyLines(e.NewRectangle.Width);
			}
			textEndPoint=textEndPointCur;
		}

		///<summary></summary>
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(this._spellCheckIsEnabled && PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//Only spell check if enabled
				timerSpellCheck.Stop();
			}
			int originalLength=Text.Length;
			int originalCaret=base.SelectionStart;
			string newText=QuickPasteNotes.Substitute(Text,quickPasteType);
			if(Text!=newText) {
				Text=newText;
				SelectionStart=originalCaret+Text.Length-originalLength;
			}
			//then CtrlQ
			if(e.KeyCode==Keys.Q && e.Modifiers==Keys.Control) {
				ShowFullDialog();
			}
			if(this._spellCheckIsEnabled && PrefC.GetBool(PrefName.SpellCheckIsEnabled)) {//Only spell check if enabled
				timerSpellCheck.Start();
			}
		}

		#region SpellCheck
		///<summary>Red wavy lines are manually drawn onto the graphics handle of this text box.  This method will clear out those wavy lines.
		///The widthOverride parameter should only be explicitly set if the control is in the middle of resizing. 
		///this.Size is stale when calculating line heights in the middle of a content resize event.</summary>
		public void ClearWavyLines(int widthOverride=-1,SpellCheckResult spellCheckResult=null) {
			if(this.Width<=0 || this.Height<=0) {//Width or Height can be 0 if the window or textbox is resized.  Causes a UE when creating a Bitmap. 
				return;//No lines.
			}
			//First we create the graphics objects we'll draw on.
			Bitmap bitmapOverlay=new Bitmap(this.Width,this.Height);
			//If it is not null, we dispose before we assign another image.
			BufferGraphics?.Dispose();
			BufferGraphics=Graphics.FromImage(bitmapOverlay);
			BufferGraphics.Clear(Color.Transparent);//We don't want to overwrite the text in the rich text box.
			Graphics graphicsTextBox=Graphics.FromHwnd(this.Handle);
			if(ODBuild.IsDebug()) {
				if(spellCheckResult==null) {
					spellCheckResult=new SpellCheckResult();
				}
				spellCheckResult.WavyLineArea=new WavyLineArea();
				spellCheckResult.WavyLineArea.ListWavyLineRects=new List<Rectangle>();
			}
			//Now we start checking where we need to clear lines
			if(Text.Length==0) {//all text was deleted, clear the entire text box
				Rectangle rectWavyLineArea=new Rectangle(1,1,this.Width,this.Height);
				BufferGraphics.FillRectangle(new SolidBrush(this.BackColor),rectWavyLineArea);
				graphicsTextBox.DrawImageUnscaled(bitmapOverlay,0,0);
				graphicsTextBox.Dispose();
				bitmapOverlay.Dispose();
				if(ODBuild.IsDebug()) {
					spellCheckResult.WavyLineArea.ListWavyLineRects.Add(rectWavyLineArea);
				}
				return;
			}
			CharBounds charBounds=GetVisibleCharIndices();
			//Get the visible start and end char indices and use them to get the visible line heights.
			List<int> listVisibleLineHeights=GetVisibleLineHeights(widthOverride);//Used for measuring line heights
			if(ODBuild.IsDebug()) {
				spellCheckResult.WavyLineArea.startCharIndex=charBounds.StartCharIndex;
				spellCheckResult.WavyLineArea.startLineIndex=this.GetLineFromCharIndex(charBounds.StartCharIndex);
				spellCheckResult.WavyLineArea.endCharIndex=charBounds.EndCharIndex;
				spellCheckResult.ListVisibleLineHeights=listVisibleLineHeights;
			}
			//The start index is also our offset from the start of the text.
			//The minimum value returned is 0.  If the point is above the text, it will default to 0.
			Point start=GetPositionFromCharIndex(charBounds.StartCharIndex);//start at first character of the textbox
			//word may span more than one line, so white out all lines between the starting char line and the ending char line
			foreach(int lineHeight in listVisibleLineHeights) {
				start.Y+=lineHeight;
				if(start.Y>=this.Height) {//y pos of the bottom of the line is below the visible area, with scroll bar active.
					break;
				}
				Rectangle rectWavyLineArea=new Rectangle(1,start.Y,this.Width,2);
				BufferGraphics.FillRectangle(new SolidBrush(BackColor),rectWavyLineArea);
				if(ODBuild.IsDebug()) {
					spellCheckResult.WavyLineArea.ListWavyLineRects.Add(rectWavyLineArea);
				}
			}
			graphicsTextBox.DrawImageUnscaled(bitmapOverlay,0,0);
			graphicsTextBox.Dispose();
			bitmapOverlay.Dispose();
			return; 
		}

		///<summary>Performs spell checking against indiviudal words against the Hunspell dictionary and the custom internal dictionary.
		///Returns a SpellCheckResult which is a helper object designed for unit tests that only gets filled in debug mode.</summary>
		public SpellCheckResult SpellCheck() {
			SpellCheckResult spellCheckResult=null;//Never keep track of this type of information in a live environment.
			if(ODBuild.IsDebug()) {
				spellCheckResult=new SpellCheckResult();
			}
			//Only spell check if enabled
			if(!this._spellCheckIsEnabled 
				|| !PrefC.GetBool(PrefName.SpellCheckIsEnabled)
				|| PrefC.GetBool(PrefName.ImeCompositionCompatibility))
			{
				//Do not spell check languages that use composition.  If needed in the future, fix the bug where the first char disapears in the box.
				//E.g. go into an ODTextBox, set language input to Korean, and simply type the letter 'ㅇ' (d) and wait.  It will disapear.
				return spellCheckResult;
			}
			if(this.Width <= 0 || this.Height <= 0) {//Width or Height can be 0 if the window or textbox is resized.  Causes a UE when creating a Bitmap.
				return spellCheckResult;
			}
			//Clear out old lines from last draw.
			List<MatchOD> listVisibleWords=GetVisibleWords();
			if(ODBuild.IsDebug()) {
				spellCheckResult.ListVisibleLineHeights=GetVisibleLineHeights();
				spellCheckResult.ListVisibleWords=listVisibleWords;
			}
			//Skip if there aren't any words.
			if(listVisibleWords.Count==0) {				
				//Clear lines and return becuase there's no words to check.
				ClearWavyLines(spellCheckResult:spellCheckResult);
				return spellCheckResult;
			}
			List<MatchOD> listMisspelledWords=new List<MatchOD>();
			foreach(MatchOD wordCur in listVisibleWords) {
				//We try and store words in our internal dictionary to speed up spelling lookup.
				if(_dictAllWords.ContainsKey(wordCur.Value)) {
					//If it's spelled right, next word.
					if(_dictAllWords[wordCur.Value]!=SpellingType.Incorrect) {
						continue;
					}
					//If we can't find the current casing, we try and find it's lower case version from the custom spelling dictionary
					else if(_dictAllWords.ContainsKey(wordCur.Value.ToLower())) {
						if(_dictAllWords[wordCur.Value.ToLower()]==SpellingType.Custom) {
							continue;
						}
					}
					listMisspelledWords.Add(wordCur);
				}
				else {//A word we have not checked yet.
					//When looking in Hunspell we care about casing, but with our custom dictionary we don't.
					//The word as they typed it was not in the correct list, but lower case version is,
					//see if the casing as they typed it is correct by Hunspell ("google" is incorrect but "Google" is correct)
					SpellingType wordType;
					string word=wordCur.Value;
					if(HunspellGlobal.Spell(wordCur.Value)) {
						wordType=SpellingType.HunspellExact;
					}
					else if(HunspellGlobal.Spell(wordCur.Value.ToLower()) ) {
						wordType=SpellingType.HunspellLower;
					}
					else if(DictCustoms.GetFirstOrDefault(x => x.WordText.ToLower()==wordCur.Value.ToLower())!=null) {
						wordType=SpellingType.Custom;
						word=word.ToLower();
					}
					else {
						wordType=SpellingType.Incorrect;
					}
					//If we don't have it in our dictionary yet, we check the word and add it and store if it's correct.
					_dictAllWords.TryAdd(word,wordType);
					if(wordType!=SpellingType.Incorrect) {
						continue;//Is correct
					}
					else {
						//If it is't incorrect add it to both our lists.
						listMisspelledWords.Add(wordCur);
					}
				}
			}			
			//Wait until now to clear lines minimize the amount of time between old and new underlines.
			ClearWavyLines(spellCheckResult:spellCheckResult);
			if(ODBuild.IsDebug()) {
				spellCheckResult.ListMisspelledWords=listMisspelledWords;
			}
			//If we have no lines to draw we return before starting any underlining.
			if(listMisspelledWords.Count==0) {
				return spellCheckResult;
			}
			List<int> listVisibleLineHeights=GetVisibleLineHeights();
			List<WavyLine> listWavyLines=null;
			if(ODBuild.IsDebug()) {
				listWavyLines=new List<WavyLine>();
				spellCheckResult.WavyLineArea.ListWavyLines=listWavyLines;
			}
			//Now we draw all the new lines to the textbox.
			using(Bitmap bitmapOverlay=new Bitmap(this.Width,this.Height)) {
				BufferGraphics?.Dispose();
				BufferGraphics=Graphics.FromImage(bitmapOverlay);
				BufferGraphics.Clear(Color.Transparent);//Remove white and replace with transparent so we don't cover the text in the rich text box.
				//This draws lines on the BufferGraphics and bitmapOverlay using our list of incorrect words.
				foreach(MatchOD word in listMisspelledWords) {
					DrawWave(word,listVisibleLineHeights,listWavyLines);
				}
				//DrawLines drew the underlines we needed, so now we apply it to the textbox.
				using(Graphics graphicsTextBox=Graphics.FromHwnd(this.Handle)) {
					graphicsTextBox.DrawImageUnscaled(bitmapOverlay,0,0);
				}
			}
			return spellCheckResult;
		}

		///<summary>Returns a list of MatchOD objects describing the currently visible text in the control.  It will include lines that are half-visible 
		///because of scrolling.  Does the same regex filtering as GetWords()</summary>
		private List<MatchOD> GetVisibleWords() {
			CharBounds charBounds=GetVisibleCharIndices();
			int startCharIndex=charBounds.StartCharIndex;
			//Add one to the end index so we capture the last character. e.g. [visible tex]t instead of [visible text] 
			//If there's only one character, the endCharIndex would be 0 instead of 1
			int endCharIndex=charBounds.EndCharIndex+1;
			//If we get invalid indices, we just return an empty list.
			if(startCharIndex>=endCharIndex) {
				return new List<MatchOD>();
			}
			if(endCharIndex>Text.Length) {
				endCharIndex=Text.Length;
			}
			string visibleText=Text.Substring(startCharIndex,endCharIndex-startCharIndex);
			return GetWords(visibleText,startCharIndex);
		}
		#endregion SpellCheck

		///<summary></summary>
		private List<MatchOD> GetWords(string input,int offset=0) {
			List<MatchOD> wordList=new List<MatchOD>();
			MatchCollection mc=Regex.Matches(input,@"(\S+)");//use Regex.Matches because our matches include the index within our text for underlining
			foreach(Match m in mc) {
				Group g=m.Groups[0];//Group 0 is the entire match
				if(g.Value.Length<2) {//only allow 'words' that are at least two chars long, 1 char 'words' are assumed spelled correctly
					continue;
				}
				MatchOD word=new MatchOD();
				word.StartCharIndex=g.Index+offset;//this index is the index within Text of the first char of this word (match)
				word.Value=g.Value;
				//loop through starting at the beginning of word looking for first letter or digit
				while(word.Value.Length>1 && !Char.IsLetterOrDigit(word.Value[0])) {
					word.Value=word.Value.Substring(1);
					word.StartCharIndex++;
				}
				//loop through starting at the last char looking for the last letter or digit
				while(word.Value.Length>1 && !Char.IsLetterOrDigit(word.Value[word.Value.Length-1])) {
					word.Value=word.Value.Substring(0,word.Value.Length-1);
				}
				if(word.Value.Length>1) {
					if(Regex.IsMatch(word.Value,@"[^a-zA-Z\'\-]")) {
						continue;
					}
					wordList.Add(word);
				}
			}
			return wordList;
		}

		/// <summary>Returns a CharBounds object that contains the start and end indices of the visible text.</summary>
		public CharBounds GetVisibleCharIndices() {
			int startCharIndex=this.GetCharIndexFromPosition(new Point(0,0));
			//Client Size is used instead of this.Width and this.Height because we want to measure from the corner of the text area, not the control.
			int endCharIndex=this.GetCharIndexFromPosition(new Point(this.ClientSize));
			return new CharBounds(startCharIndex,endCharIndex);
		}

		///<summary>Returns a list of line heights in order of line index. The widthOverride parameter should only
		///be explicitly set if the control is in the middle of resizing. this.Size is stale when calculating
		///line heights in the middle of a content resize event.  Returns an empty list if the current Rtf property is invalid.</summary>
		private List<int> GetVisibleLineHeights(int widthOverride=-1) {
			CharBounds charBounds=GetVisibleCharIndices();
			using(RichTextBox rtb=new RichTextBox()) {
				try {
					//There was a report of this.Rtf being incorrectly formatted at this point (see task #1771785).
					//The Rtf property will throw an ArgumentException saying "File format is not valid" when this is the case.
					rtb.Rtf=this.Rtf;
				}
				catch(ArgumentException ae) {
					ae.DoNothing();
					return new List<int>();//Do nothing, it's just spell checking.  Maybe it will be valid RTF the next time around.
				}
				rtb.Size=new Size((widthOverride==-1 ? this.Size.Width : widthOverride),this.Size.Height);
				rtb.AppendText("\r\n\t");//This will allow us to get the Top of the fake line, which is the bottom of the last line of real text.
				//Get the starting and ending lines in the text box.			
				int startLineIndex=rtb.GetLineFromCharIndex(charBounds.StartCharIndex);
				//No matter what line the endCharIndex the user passed in there will ALWAYS be at least one more line after it (we just added one).
				int endCharLineIndex=(rtb.GetLineFromCharIndex(charBounds.EndCharIndex) + 1);
				List<int> listLines=new List<int>();
				//Go through each line and grab y-position of top of line.
				for(int i=startLineIndex;i<=endCharLineIndex;i++) {
					int charIdx=rtb.GetFirstCharIndexFromLine(i);
					Point posThisLine=rtb.GetPositionFromCharIndex(charIdx);
					listLines.Add(posThisLine.Y);
				}
				List<int> listLineHeights=new List<int>();
				for(int i=0;i<listLines.Count-1;i++) {
					//Top of next line minus top of current line will give us the height of the current line.
					listLineHeights.Add(listLines[i+1]-listLines[i]);
				}
				return listLineHeights;
			}
		}

		///<summary>Draws a red wavy line underneath the match passed in.  The location of the wavy line is dictated by the corresponding line height.
		///Adds a WavyLine object that was just drawn to the list of WavyLines passed in if not null and in debug mode.</summary>
		private void DrawWave(MatchOD match,List<int> listVisibleLineHeights,List<WavyLine> listWavyLines=null) {
			if(listVisibleLineHeights==null || listVisibleLineHeights.Count < 1) {
				return;//No line to draw because there were no heights passed in so we have no idea where to draw them.
			}
			//The match passed in might not reside on the first visible line so we have to figure out the corresponding line height that it is on.
			//Do this by figuring out which line is the first visible line and then compare that to the line that the match resides on.
			//E.g. if there are 100 lines of text in the text box and the first visible line is the 88th line and the match is on the 92nd line we know
			//that the StartLineIndex needs to be set to an index of 4 (92 - 88) because index is zero based;
			//88th = listLineHeights[0], 89th = listLineHeights[1], 90th = listLineHeights[2], 91st = listLineHeights[3], 92nd = listLineHeights[4]
			int lineFirstVisibleChar=this.GetLineFromCharIndex(this.GetCharIndexFromPosition(new Point(0,0)));
			int lineMatchCur=this.GetLineFromCharIndex(match.StartCharIndex);
			int startLineIndex=0;
			if(lineMatchCur>=lineFirstVisibleChar) {
				startLineIndex=lineMatchCur-lineFirstVisibleChar;
			}
			else {
				//Technically a match that is not even visible was passed in...  Nothing to draw.
				return;
			}
			if(startLineIndex>=listVisibleLineHeights.Count) {
				//The match is on a line that is partially visible. No need to draw line.
				return;
			}
			int startLineHeight=listVisibleLineHeights[startLineIndex];
			Point pointStart=this.GetPositionFromCharIndex(match.StartCharIndex);//accounts for scroll position
			Point pointEnd=this.GetPositionFromCharIndex(match.StartCharIndex+match.Value.Length);//accounts for scroll position
			pointStart.Y=pointStart.Y+startLineHeight;//move from top of line to bottom of line
			pointEnd.Y=pointEnd.Y+startLineHeight;//move from top of line to bottom of line
			WavyLine wavyLine=new WavyLine();
			if(ODBuild.IsDebug()) {
				wavyLine.LineIndex=startLineIndex;
				wavyLine.LineHeight=startLineHeight;
				wavyLine.PointStart=new Point(pointStart.X,pointStart.Y);
				wavyLine.PointEnd=new Point(pointEnd.X,pointEnd.Y);
				wavyLine.ListPoints=new List<Point>();
			}
			if(pointStart.Y<=4 || pointStart.Y>=this.Height) {//Don't draw lines for text which is currently not visible.
				return;
			}
			Pen pen=Pens.Red;
			//Misspelled word spans multiple lines.  This should never actually happen as word wrapping
			//does not allow this scenario to happen and ODTextBox defaults word wrapping to 'true'.
			//The only exception is words which are longer than the line, but we are ignoring this case for now (per Nathan).
			if(pointEnd.Y>pointStart.Y) {
				int lineIndex=startLineIndex;
				Point tempEnd=pointStart;
				tempEnd.X=this.Width;
				while(tempEnd.Y<=pointEnd.Y && lineIndex<listVisibleLineHeights.Count) {//One line at a time.
					if(ODBuild.IsDebug()) {
						if(wavyLine==null) {
							wavyLine=new WavyLine();
							wavyLine.LineIndex=lineIndex;
							wavyLine.LineHeight=listVisibleLineHeights[lineIndex];
							wavyLine.ListPoints=new List<Point>();
							wavyLine.PointEnd=pointEnd;
							wavyLine.PointStart=pointStart;
						}
					}
					if((tempEnd.X-pointStart.X)>4) {//Only draw wavy line if mispelled word is at least 4 pixels wide, otherwise draw straight line
						List<Point> listPoints=new List<Point>();
						for(int i=pointStart.X;i<=(tempEnd.X-2);i=i+4) {
							listPoints.Add(new Point(i,pointStart.Y));
							listPoints.Add(new Point(i+2,pointStart.Y+1));
						}
						BufferGraphics.DrawLines(pen,listPoints.ToArray());
						if(ODBuild.IsDebug()) {
							wavyLine.ListPoints.AddRange(listPoints);
						}
					}
					else {
						BufferGraphics.DrawLine(pen,pointStart,pointEnd);
						if(ODBuild.IsDebug()) {
							wavyLine.ListPoints.AddRange(new List<Point>() { pointStart,pointEnd });
						}
					}
					pointStart.X=1;
					//There is a known issue where if a word spans more than 1 line, the fontheight will only calculate correctly for
					//the first line. The second line's fontheight will not recalculate correctly and will use the first line's static value.
					//Per Nathan we are ignoring this case. If we do want to fix it in the future, this is the spot.
					//lineIndex needs to be updated to use the index of the current line the word is on.
					pointStart.Y=pointStart.Y+listVisibleLineHeights[lineIndex];
					tempEnd.Y=pointStart.Y;
					if(tempEnd.Y==pointEnd.Y) {//We incremented to the next line and this is the last line of the mispelled word
						tempEnd.X=pointEnd.X;
					}
					else {//not the last line of mispelled word, so draw wavy line to end of this line
						tempEnd.X=this.Width;
					}
					if(ODBuild.IsDebug()) {
						if(listWavyLines!=null) {
							listWavyLines.Add(wavyLine);
						}
						//Null out the wavyLine object so that the while loop knows to make a new one for the next line (if one is needed).
						wavyLine=null;
					}
					lineIndex++;
				}
			}
			else {//pointEnd.Y==pointStart.Y
				if((pointEnd.X-pointStart.X)>4) {
					List<Point> listPoints=new List<Point>();
					//Only draw wavy line if misspelled word is at least 4 pixels wide, otherwise draw straight line.
					for(int i=pointStart.X;i<=(pointEnd.X-2);i+=4) {
						listPoints.Add(new Point(i,pointStart.Y));
						listPoints.Add(new Point(i+2,pointStart.Y+1));
					}
					BufferGraphics.DrawLines(pen,listPoints.ToArray());
					if(ODBuild.IsDebug()) {
						wavyLine.ListPoints.AddRange(listPoints);
					}
				}
				else {
					BufferGraphics.DrawLine(pen,pointStart,pointEnd);
					if(ODBuild.IsDebug()) {
						wavyLine.ListPoints.AddRange(new List<Point>() { pointStart,pointEnd });
					}
				}
				if(ODBuild.IsDebug()) {
					if(listWavyLines!=null) {
						listWavyLines.Add(wavyLine);
					}
				}
			}
			return;
		}

		private void ShowFullDialog() {
			using FormQuickPaste FormQ=new FormQuickPaste(false);
			FormQ.TextToFill=this;
			FormQ.QuickType=quickPasteType;
			FormQ.ShowDialog();
		}

		private void InsertDate() {
			SelectedText=DateTime.Today.ToShortDateString();
		}

		///<summary>Holds the start and end char indices for the currently visible text.</summary>
		public class CharBounds:ODTuple<int,int> {
			public int StartCharIndex {
				get {
					return Item1;
				}
				set {
					Item1=value;
				}
			}

			public int EndCharIndex {
				get {
					return Item2;
				}
				set {
					Item2=value;
				}
			}

			public CharBounds(int startCharIndex,int endCharIndex) : base(startCharIndex,endCharIndex) {

			}
		}

		///<summary>Analogous to a Match.  We use it to keep track of words that we find and their location within the larger string.</summary>
		public class MatchOD {
			///<summary>This is the 'word' for this match</summary>
			public string Value="";
			///<summary>This is the starting index of the first char of the 'word' within the full textbox text</summary>
			public int StartCharIndex=0;
		}

		///<summary>A helper class that holds information regarding spell checking.  Used for unit testing different ways of spell checking.</summary>
		public class SpellCheckResult {
			///<summary>Set to all of the visible line heights for the text box when this object is created.</summary>
			public List<int> ListVisibleLineHeights;
			///<summary>A list of every visible word.  This list of words will be used for spell checking.</summary>
			public List<MatchOD> ListVisibleWords;
			///<summary>A list of words from the list of visible words that are misspelled.</summary>
			public List<MatchOD> ListMisspelledWords;
			///<summary>A helper object that contains information pertinent to all of the red wavy lines.</summary>
			public WavyLineArea WavyLineArea;
		}

		///<summary>A helper class that holds information regarding all the red wavy lines underneath misspelled words.</summary>
		public class WavyLineArea {
			///<summary>The index for the very first visible character (from Point(0,0)).  Can be a partially visible character.</summary>
			public int startCharIndex;
			///<summary>The index for the very first visible line (from Point(0,0)).  Can be a partially visible line.</summary>
			public int startLineIndex;
			///<summary>The index of the last visible character (from Point(this.Width,this.Height)).  Can be a partially visible character.</summary>
			public int endCharIndex;
			///<summary>A list of rectangles that were used when clearing out the previous wavy lines.</summary>
			public List<Rectangle> ListWavyLineRects;
			///<summary>A list of red wavy lines that were drawn due to the most recent spell check.</summary>
			public List<WavyLine> ListWavyLines;
		}

		///<summary>A helper class that holds information regarding a specific red wavy line underneath a misspelled word.</summary>
		public class WavyLine {
			///<summary>The index within the list of visible lines that this wavy line was drawn.</summary>
			public int LineIndex;
			///<summary>The height of the current visible line which is used in order to know where the wavy line should be drawn.</summary>
			public int LineHeight;
			///<summary>The point that this particular wavy line should start drawing.</summary>
			public Point PointStart;
			///<summary>The very last possible point for the current line / line group (if more than one line is necessary for a single word).
			///This point will repeat several times in a row for wavy lines that represent the same word but span multiple lines.</summary>
			public Point PointEnd;
			///<summary>Every point necessary to draw the current wavy line.</summary>
			public List<Point> ListPoints;
		}

		///<summary>Used to determine if a word is correct and by which spelling dictionary.</summary>
		private enum SpellingType {
			Incorrect,
			HunspellExact,
			HunspellLower,
			Custom
		}
	}
}