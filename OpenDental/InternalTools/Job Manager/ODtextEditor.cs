using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Drawing.Text;
using System.ComponentModel;
using System.Linq;

namespace OpenDental {
	///<summary></summary>
	public delegate void ODtextEditorSaveEventHandler(object sender,EventArgs e);

	[DefaultEvent("SaveClick")]
	public partial class OdtextEditor:UserControl {
		private bool _hasSaveButton;
		private bool _hasEditorOptions;
		private bool _isReadOnly;
		///<summary>Used to set the button color back after hovering</summary>
		private Color _backColor;
		///<summary>Used when highlighting.</summary>
		private Color _highlightColor;

		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the control contains a save button or not.")]
		public bool HasSaveButton {
			get {
				return _hasSaveButton;
			}
			set {
				_hasSaveButton=value;
				OdtextEditor_Layout(this,null);
				Invalidate();
			}
		}		
		
		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the control contains the editor toolbar.")]
		public bool HasEditorOptions {
			get {
				return _hasEditorOptions;
			}
			set {
				_hasEditorOptions=value;
				OdtextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the control can be edited.")]
		public bool ReadOnly {
			get {
				return _isReadOnly;
			}
			set {
				_isReadOnly=value;
				OdtextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the main textbox text.</summary>
		public string MainText {
			get {
				return textDescription.Text;
			}
			set {
				textDescription.Text=value;
				OdtextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the main textbox RTF format text.</summary>
		public string MainRtf {
			get {
				return textDescription.Rtf;
			}
			set {
				textDescription.Rtf=value;
				OdtextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the main textbox RTF format text.</summary>
		public Font MainFont {
			get {
				return textDescription.Font;
			}
			set {
				textDescription.Font=value;
				OdtextEditor_Layout(this,null);
				Invalidate();
			}
		}

		private Color SelectedColor {
			get {
				return Color.LightGray;
			}
		}

		private Color UnselectedColor {
			get {
				return SystemColors.Control;
			}
		}

		///<summary>Gets main textbox.</summary>
		public RichTextBox TextBox {
			get {
				return textDescription;
			}
		}

		[Category("Action"),Description("Occurs when the save button is clicked.")]
		public event ODtextEditorSaveEventHandler SaveClick=null;

		public delegate void textChangedEventHandler();
		[Category("Action"),Description("Occurs as text is changed.")]
		public event textChangedEventHandler OnTextEdited=null;

		public OdtextEditor() {
			InitializeComponent();
			_hasSaveButton=true;
		}

		#region Public Methods
		public void Append(string appendText) {
			textDescription.AppendText(appendText);
		}
		#endregion

		///<summary></summary>
		protected override void OnLoad(EventArgs e) {
			//Fill and set font
			InstalledFontCollection installedFonts=new InstalledFontCollection();
			installedFonts.Families.ToList().ForEach(x => comboFontType.Items.Add(x.Name));
			comboFontType.SelectedIndex=installedFonts.Families.ToList().FindIndex(x => x.Name.Contains("Microsoft Sans Serif"));			
			//Fill and set font size
			for(double i = 7;i<=20;i=i+.5) {
				 comboFontSize.Items.Add(i);
			}
			comboFontSize.SelectedIndex=2;//Size 8;
			butHighlight.BackColor=Color.Yellow;
			_highlightColor=butHighlight.BackColor;
			butSpellCheck.BackColor=textDescription.SpellCheckIsEnabled?Color.LightGreen:Color.LightCoral;
		}

    private void OdtextEditor_Layout(object sender,LayoutEventArgs e) {
			flowLayoutPanel1.Visible=_hasEditorOptions;
			if(!_hasEditorOptions) {
				textDescription.Location=new Point(textDescription.Location.X,textDescription.Location.Y-flowLayoutPanel1.Size.Height);
				textDescription.Size=new Size(textDescription.Size.Width,flowLayoutPanel1.Size.Height);
			}
			butSave.Visible=_hasSaveButton;
      textDescription.ReadOnly=_isReadOnly;
      bool isEnabled=!_isReadOnly;//Only here for readability of the following code.
      butCut.Enabled=isEnabled;
      butCopy.Enabled=isEnabled;
      butPaste.Enabled=isEnabled;
      butUndo.Enabled=isEnabled;
      butRedo.Enabled=isEnabled;
      butBold.Enabled=isEnabled;
      butItalics.Enabled=isEnabled;
      butUnderline.Enabled=isEnabled;
      butStrikeout.Enabled=isEnabled;
      butBullet.Enabled=isEnabled;
      comboFontSize.Enabled=isEnabled;
      comboFontType.Enabled=isEnabled;
      butColor.Enabled=isEnabled;
      butColorSelect.Enabled=isEnabled;
      butHighlight.Enabled=isEnabled;
      butHighlightSelect.Enabled=isEnabled;
      butSave.Enabled=isEnabled;
			butSpellCheck.Enabled=isEnabled;
			butClearFormatting.Enabled=isEnabled;
    }

		private void HoverColorEnter(object sender,EventArgs e) {
			System.Windows.Forms.Button btn=(System.Windows.Forms.Button)sender;
			_backColor=btn.BackColor;
			btn.BackColor=Color.PaleTurquoise;
		}

		private void HoverColorLeave(object sender,EventArgs e) {
			System.Windows.Forms.Button btn=(System.Windows.Forms.Button)sender;
			if(_backColor!=Color.Transparent) {
				btn.BackColor=_backColor;
			}
			_backColor=Color.Transparent;
		}

		private void butCut_Click(object sender,EventArgs e) {
			textDescription.Cut();
			textDescription.Focus();
		}

		private void butCopy_Click(object sender,EventArgs e) {
			textDescription.Copy();
			textDescription.Focus();
		}

		private void butPaste_Click(object sender,EventArgs e) {
			textDescription.Paste();
			textDescription.Focus();
		}

		private void butUndo_Click(object sender,EventArgs e) {
			textDescription.Undo();
			textDescription.Focus();
		}

		private void butRedo_Click(object sender,EventArgs e) {
			textDescription.Redo();
			textDescription.Focus();
		}

		private void butBold_Click(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font(textDescription.SelectionFont,textDescription.SelectionFont.Style ^ FontStyle.Bold);
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butItalics_Click(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font(textDescription.SelectionFont,textDescription.SelectionFont.Style ^ FontStyle.Italic);
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butUnderline_Click(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font(textDescription.SelectionFont,textDescription.SelectionFont.Style ^ FontStyle.Underline);
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butStrikeout_Click(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font(textDescription.SelectionFont,textDescription.SelectionFont.Style ^ FontStyle.Strikeout);
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butBullet_Click(object sender,EventArgs e) {
			if(textDescription.SelectionBullet) {
				textDescription.SelectionBullet=false;
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			else {
				textDescription.SelectionBullet=true;
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
		}

		private void comboFontType_SelectionChangeCommitted(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font((string)comboFontType.SelectedItem,PIn.Float(comboFontSize.SelectedItem.ToString()),textDescription.SelectionFont.Style);
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {

			}
		}

		private void comboFontSize_SelectionChangeCommitted(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font((string)comboFontType.SelectedItem,PIn.Float(comboFontSize.SelectedItem.ToString()),textDescription.SelectionFont.Style);
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {

			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			textDescription.SelectionColor=butColor.ForeColor;
			textDescription.Focus();
		}

		private void butColorSelect_Click(object sender,EventArgs e) {
			colorDialog1.Color=butColor.ForeColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK) {
				return;
			}
			butColor.ForeColor=colorDialog1.Color;
			textDescription.SelectionColor=butColor.ForeColor;
			textDescription.Focus();
		}

		private void butHighlight_Click(object sender,EventArgs e) {
			if(textDescription.ReadOnly) {
				return;
			}
			if(textDescription.SelectionBackColor==_highlightColor) {
				textDescription.SelectionColor=textDescription.ForeColor;
				textDescription.SelectionBackColor=textDescription.BackColor;
			}
			else {
				textDescription.SelectionBackColor=_highlightColor;
			}
			textDescription.Focus();
		}

		private void butHighlightSelect_Click(object sender,EventArgs e) {
			colorDialog1.Color=butHighlight.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK) {
				return;
			}
			butHighlight.BackColor=colorDialog1.Color;
			_highlightColor=colorDialog1.Color;
			textDescription.SelectionBackColor=_highlightColor;
			textDescription.Focus();
		}

		///<summary></summary>
		private void butSave_Click(object sender,EventArgs e) {
			SaveText();
		}

		private void SaveText() {
			EventArgs gArgs=new EventArgs();
			if(SaveClick!=null) {
				SaveClick(this,gArgs);
				textDescription.Focus();
			}
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			if(OnTextEdited!=null) {
				OnTextEdited();
			}
		}

		private void butSpellCheck_Click(object sender,EventArgs e) {
			textDescription.SpellCheckIsEnabled=!textDescription.SpellCheckIsEnabled;
			textDescription.Refresh();
			textDescription.SpellCheck();
			butSpellCheck.BackColor=textDescription.SpellCheckIsEnabled?Color.LightGreen:Color.LightCoral;
		}

		private void butClearFormatting_Click(object sender,EventArgs e) {
			try {
				textDescription.SelectionFont=new Font((string)comboFontType.SelectedItem,(int)comboFontSize.SelectedItem);
				textDescription.SelectionBullet=false;
				textDescription.SelectionColor=DefaultForeColor;
				textDescription.SelectionBackColor=DefaultBackColor;
				UpdateSelectedFontStyles();
				textDescription.Focus();
			}
			catch {

			}
		}

		private void textDescription_KeyUp(object sender,KeyEventArgs e) {
			if(e.Control && e.KeyCode == Keys.S) {
				SaveText();
			}
		}

		private void textDescription_SelectionChanged(object sender,EventArgs e) {
			UpdateSelectedFontStyles();
		}

		private void UpdateSelectedFontStyles() {
			Font selectedFont=textDescription.SelectionFont;
			if(selectedFont==null) {//this happens if the selected text contains characters of more than one font
				return;
			}
			butBold.BackColor=selectedFont.Bold ? SelectedColor:UnselectedColor;
			butItalics.BackColor=selectedFont.Italic ? SelectedColor:UnselectedColor;
			butUnderline.BackColor=selectedFont.Underline ? SelectedColor:UnselectedColor;
			butStrikeout.BackColor=selectedFont.Strikeout ? SelectedColor:UnselectedColor;
			butBullet.BackColor=textDescription.SelectionBullet ? SelectedColor:UnselectedColor;
			comboFontSize.Text=selectedFont.SizeInPoints.ToString();
			comboFontType.Text=selectedFont.FontFamily.Name;
		}
	}

}
