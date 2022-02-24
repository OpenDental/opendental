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
	public delegate void ODjobTextEditorSaveEventHandler(object sender,EventArgs e);

	[DefaultEvent("SaveClick")]
	public partial class ODjobTextEditor:UserControl {
		private bool _hasSaveButton;
		private bool _hasEditorOptions;
		private bool _isConceptReadOnly;
		private bool _isMouseDownSplitter1;
		private bool _isMouseDownSplitter2;
		private bool _isRequirementsGridReadOnly;
		private bool _isWriteupReadOnly;
		private int _ySplitter1Original;
		private int _yMouseOriginal;
		private int _ySplitter2Original;
		///<summary>Used to set the button color back after hovering</summary>
		private Color _backColor;
		///<summary>Used when highlighting.</summary>
		private Color _highlightColor;
		private RichTextBox _textFocused;
		private List<JobRequirement> _listJobRequirements=new List<JobRequirement>();

		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the control contains a save button or not.")]
		public bool HasSaveButton {
			get {
				return _hasSaveButton;
			}
			set {
				_hasSaveButton=value;
				ODjobTextEditor_Layout(this,null);
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
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the concept can be edited.")]
		public bool ReadOnlyConcept {
			get {
				return _isConceptReadOnly;
			}
			set {
				_isConceptReadOnly=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the requirements can be edited.")]
		public bool ReadOnlyRequirementsGrid {
			get {
				return _isRequirementsGridReadOnly;
			}
			set {
				_isRequirementsGridReadOnly=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("Appearance"),Description("Toggles whether the writeup can be edited.")]
		public bool ReadOnlyWriteup {
			get {
				return _isWriteupReadOnly;
			}
			set {
				_isWriteupReadOnly=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the Concept textbox text.</summary>
		public string ConceptTitle {
			set {
				labelConcept.Text="Concept - "+value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the Concept textbox text.</summary>
		public string ConceptText {
			get {
				return textConcept.Text;
			}
			set {
				textConcept.Text=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the Concept textbox RTF format text.</summary>
		public string ConceptRtf {
			get {
				return textConcept.Rtf;
			}
			set {
				textConcept.Rtf=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the Writeup textbox text.</summary>
		public string WriteupText {
			get {
				return textWriteup.Text;
			}
			set {
				textWriteup.Text=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		///<summary>Gets or sets the Writeup RTF format text.</summary>
		public string WriteupRtf {
			get {
				return textWriteup.Rtf;
			}
			set {
				textWriteup.Rtf=value;
				ODjobTextEditor_Layout(this,null);
				Invalidate();
			}
		}

		public List<JobRequirement> GetListJobRequirements() {
			return _listJobRequirements??new List<JobRequirement>();
		}

		public void SetListJobRequirements(List<JobRequirement> listReq) {
			_listJobRequirements=listReq;
			FillGridRequirements();
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

		[Category("Action"),Description("Occurs when the save button is clicked.")]
		public event ODtextEditorSaveEventHandler SaveClick=null;

		public delegate void textChangedEventHandler();
		[Category("Action"),Description("Occurs as text is changed.")]
		public event textChangedEventHandler OnTextEdited=null;

		public ODjobTextEditor() {
			InitializeComponent();
			_textFocused=textConcept;
			_hasSaveButton=true;
		}

		#region Public Methods
		public void Append(string appendText) {
			_textFocused.AppendText(appendText);
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
			butSpellCheck.BackColor=textWriteup.SpellCheckIsEnabled?Color.LightGreen:Color.LightCoral;
			_highlightColor=butHighlight.BackColor;
		}

    private void ODjobTextEditor_Layout(object sender,LayoutEventArgs e) {
			flowLayoutPanelMenu.Visible=_hasEditorOptions;
			butSave.Visible=_hasSaveButton;
			textConcept.ReadOnly=_isConceptReadOnly;
			textWriteup.ReadOnly=_isWriteupReadOnly;
			bool isEnabled = (!_isWriteupReadOnly || !_isConceptReadOnly);
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

		private void FillGridRequirements() {
			gridRequirements.BeginUpdate();
			gridRequirements.ListGridColumns.Clear();
			gridRequirements.ListGridColumns.Add(new GridColumn("Concept Requirements List",100,!ReadOnlyRequirementsGrid){ IsWidthDynamic=true });
			gridRequirements.ListGridColumns.Add(new GridColumn("Expert",60,HorizontalAlignment.Center));
			gridRequirements.ListGridColumns.Add(new GridColumn("Engineer",60,HorizontalAlignment.Center));
			gridRequirements.ListGridColumns.Add(new GridColumn("Reviewer",60,HorizontalAlignment.Center));
			gridRequirements.ListGridColumns.Add(new GridColumn("",20,HorizontalAlignment.Center));
			gridRequirements.ListGridColumns.Add(new GridColumn("",20,HorizontalAlignment.Center));
			gridRequirements.ListGridRows.Clear();
			foreach(JobRequirement jobReq in GetListJobRequirements()) {
				gridRequirements.ListGridRows.Add(
					new GridRow(
						new GridCell(jobReq.Description),
						new GridCell(jobReq.HasExpert?"X":""),
						new GridCell(jobReq.HasEngineer?"X":""),
						new GridCell(jobReq.HasReviewer?"X":""),
						new GridCell("▲"),
						new GridCell("▼")
						)
				);
			}
			gridRequirements.EndUpdate();
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
			if(_textFocused.ReadOnly) {
				return;
			}
			_textFocused.Cut();
			_textFocused.Focus();
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			_textFocused.Copy();
			_textFocused.Focus();
		}

		private void butPaste_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			_textFocused.Paste();
			_textFocused.Focus();
		}

		private void butUndo_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			_textFocused.Undo();
			_textFocused.Focus();
		}

		private void butRedo_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			_textFocused.Redo();
			_textFocused.Focus();
		}

		private void butBold_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font(_textFocused.SelectionFont,_textFocused.SelectionFont.Style ^ FontStyle.Bold);
				UpdateSelectedFontStyles(_textFocused);
				_backColor=_backColor==UnselectedColor ? SelectedColor:UnselectedColor;
				_textFocused.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butItalics_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font(_textFocused.SelectionFont,_textFocused.SelectionFont.Style ^ FontStyle.Italic);
				UpdateSelectedFontStyles(_textFocused);
				_backColor=_backColor==UnselectedColor ? SelectedColor:UnselectedColor;
				_textFocused.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butUnderline_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font(_textFocused.SelectionFont,_textFocused.SelectionFont.Style ^ FontStyle.Underline);
				UpdateSelectedFontStyles(_textFocused);
				_backColor=_backColor==UnselectedColor ? SelectedColor:UnselectedColor;
				_textFocused.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butStrikeout_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font(_textFocused.SelectionFont,_textFocused.SelectionFont.Style ^ FontStyle.Strikeout);
				UpdateSelectedFontStyles(_textFocused);
				_backColor=_backColor==UnselectedColor ? SelectedColor:UnselectedColor;
				_textFocused.Focus();
			}
			catch {
				//labelWarning.Text="Cannot format multiple Fonts";
			}
		}

		private void butBullet_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			if(_textFocused.SelectionBullet) {
				_textFocused.SelectionBullet=false;
				UpdateSelectedFontStyles(_textFocused);
				_backColor=_backColor==UnselectedColor ? SelectedColor:UnselectedColor;
				_textFocused.Focus();
			}
			else {
				_textFocused.SelectionBullet=true;
				UpdateSelectedFontStyles(_textFocused);
				_backColor=_backColor==UnselectedColor ? SelectedColor:UnselectedColor;
				_textFocused.Focus();
			}
		}

		private void comboFontType_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font((string)comboFontType.SelectedItem,PIn.Float(comboFontSize.SelectedItem.ToString()),_textFocused.SelectionFont.Style);
				UpdateSelectedFontStyles(_textFocused);
				_textFocused.Focus();
			}
			catch {

			}
		}

		private void comboFontSize_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font((string)comboFontType.SelectedItem,PIn.Float(comboFontSize.SelectedItem.ToString()),_textFocused.SelectionFont.Style);
				UpdateSelectedFontStyles(_textFocused);
				_textFocused.Focus();
			}
			catch {

			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			_textFocused.SelectionColor=butColor.ForeColor;
			_textFocused.Focus();
		}

		private void butColorSelect_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			colorDialog1.Color=butColor.ForeColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK) {
				return;
			}
			butColor.ForeColor=colorDialog1.Color;
			_textFocused.SelectionColor=butColor.ForeColor;
			_textFocused.Focus();
		}

		private void butHighlight_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			if(_textFocused.SelectionBackColor==_highlightColor) {
				_textFocused.SelectionColor=_textFocused.ForeColor;
				_textFocused.SelectionBackColor=_textFocused.BackColor;
			}
			else {
				_textFocused.SelectionBackColor=_highlightColor;
			}
			_textFocused.Focus();
		}

		private void butHighlightSelect_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			colorDialog1.Color=butHighlight.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK) {
				return;
			}
			butHighlight.BackColor=colorDialog1.Color;
			_highlightColor=colorDialog1.Color;
			_textFocused.SelectionBackColor=_highlightColor;
			_textFocused.Focus();
		}

		///<summary></summary>
		private void butSave_Click(object sender,EventArgs e) {
			SaveText();
		}

		private void SaveText() {
			EventArgs gArgs=new EventArgs();
			if(SaveClick!=null) {
				SaveClick(this,gArgs);
				_textFocused.Focus();
			}
		}

		///<summary>Called on initial job load.</summary>
		public void ResizeTextFields() {
			//Remove all non-resizable controls
			int usableHeight=Height-flowLayoutPanelMenu.Height-panelSplitter1.Height-panelSplitter2.Height;
			//Get gridRequirements max height
			int heightRequirementsMax=gridRequirements.ListGridRows.Sum(x => x.State.HeightMain)
				+18//gridRequirements.TitleHeight
				+15;//gridRequirements.HeaderHeight;
			//Limit the requirements grid to 1/3 usable height
			if(heightRequirementsMax*3>usableHeight) {
				heightRequirementsMax=usableHeight/3;
			}
			panelRequirements.Height=heightRequirementsMax;
			//Remove heightRequirementsMax from usable height. Requirements grid is the most important part so it gets preferential treatment
			usableHeight-=heightRequirementsMax;
			//Get percent values for the textboxes
			double conceptPercent=.7;
			double writeupPercent=.3;
			//If writeup is not empty give a minimum of 70% weight
			if(!string.IsNullOrEmpty(textWriteup.Text)) {
				conceptPercent=.3;
				writeupPercent=.7;
			}
			int conceptHeight=(int)(usableHeight*conceptPercent);
			int writeupHeight=(int)(usableHeight*writeupPercent);
			panelConcept.Height=conceptHeight;
			textConcept.Height=conceptHeight-5;
			panelSplitter1.Location=new Point(0,panelConcept.Bottom);
			panelRequirements.Location=new Point(0,panelSplitter1.Bottom);
			panelSplitter2.Location=new Point(0,panelRequirements.Bottom);
			panelWriteup.Bounds=new Rectangle(0,panelSplitter2.Bottom,Width,writeupHeight);
		}

		private void panelSplitter1_MouseDown(object sender,MouseEventArgs e) {
			_isMouseDownSplitter1=true;
			_ySplitter1Original=panelSplitter1.Top;
			_yMouseOriginal=panelSplitter1.Top+e.Y;
		}

		private void panelSplitter1_MouseMove(object sender,MouseEventArgs e) {
			if(!_isMouseDownSplitter1) {
				return;
			}
			int splitterNewY=_yMouseOriginal+panelSplitter1.Top+e.Y-_ySplitter1Original;
			//Don't go higher than two lines remaining in the concept section
			if(splitterNewY<70) {
				splitterNewY=70;
			}
			//Don't go lower than two lines remaining in the writeup section
			if(splitterNewY>panelWriteup.Bottom-panelRequirements.Height-panelSplitter2.Height-50) {
				splitterNewY=panelWriteup.Bottom-panelRequirements.Height-panelSplitter2.Height-50;
			}
			panelSplitter1.Top=splitterNewY;
			panelConcept.Height=panelSplitter1.Top-panelConcept.Top;
			panelRequirements.Top=panelSplitter1.Bottom;
			panelSplitter2.Top=panelRequirements.Bottom;
			panelWriteup.Top=panelSplitter2.Bottom;
			panelWriteup.Height=this.Height-panelSplitter2.Bottom;  
		}

		private void panelSplitter1_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDownSplitter1=false;
		}

		private void panelSplitter2_MouseDown(object sender,MouseEventArgs e) {
			_isMouseDownSplitter2=true;
			_ySplitter2Original=panelSplitter2.Top;
			_yMouseOriginal=panelSplitter2.Top+e.Y;
		}

		private void panelSplitter2_MouseMove(object sender,MouseEventArgs e) {
			if(!_isMouseDownSplitter2) {
				return;
			}
			int splitterNewY=_yMouseOriginal+panelSplitter2.Top+e.Y-_ySplitter2Original;
			//Don't go higher than two lines remaining in the concept section
			if(splitterNewY<panelRequirements.Top+35) {
				splitterNewY=panelRequirements.Top+35;
			}
			//Don't go lower than two lines remaining in the writeup section
			if(splitterNewY>this.Bottom-50) {
				splitterNewY=this.Bottom-50;
			}
			panelSplitter2.Top=splitterNewY;
			panelRequirements.Height=panelSplitter2.Top-panelSplitter1.Bottom;
			panelWriteup.Top=panelSplitter2.Bottom;
			panelWriteup.Height=this.Height-panelSplitter2.Bottom; 
		}

		private void panelSplitter2_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDownSplitter2=false;
		}

		private void textDescription_TextChanged(object sender,EventArgs e) {
			if(OnTextEdited!=null) {
				OnTextEdited();
			}
		}

		private void butSpellCheck_Click(object sender,EventArgs e) {
			textWriteup.SpellCheckIsEnabled=!textWriteup.SpellCheckIsEnabled;
			textWriteup.Refresh();
			textWriteup.SpellCheck();
			butSpellCheck.BackColor=textWriteup.SpellCheckIsEnabled?Color.LightGreen:Color.LightCoral;
		}
		
		public void RefreshSpellCheckConcept() {
			textConcept.Refresh();
			textConcept.SpellCheck();
		}

		public void RefreshSpellCheckWriteup() {
			if(textWriteup.SpellCheckIsEnabled) {
				textWriteup.Refresh();
				textWriteup.SpellCheck();
			}
		}

		private void butClearFormatting_Click(object sender,EventArgs e) {
			if(_textFocused.ReadOnly) {
				return;
			}
			try {
				_textFocused.SelectionFont=new Font(comboFontType.SelectedItem.ToString(),float.Parse(comboFontSize.SelectedItem.ToString()));
				_textFocused.SelectionBullet=false;
				_textFocused.SelectionColor=_textFocused.ForeColor;
				_textFocused.SelectionBackColor=_textFocused.BackColor;
				UpdateSelectedFontStyles(_textFocused);
				_textFocused.Focus();
			}
			catch (Exception ex){
				MessageBox.Show(this,ex.Message);
			}
		}

		private void textDescription_Enter(object sender,EventArgs e) {
			_textFocused=textConcept;
		}

		private void textImplementation_Enter(object sender,EventArgs e) {
			_textFocused=textWriteup;
		}

		private void textRequirements_KeyUp(object sender,KeyEventArgs e) {
			if(e.Control && e.KeyCode == Keys.S) {
				SaveText();
			}
		}

		private void textImplementation_KeyUp(object sender,KeyEventArgs e) {
			if(e.Control && e.KeyCode == Keys.S) {
				SaveText();
			}
		}

		private void textRequirements_SelectionChanged(object sender,EventArgs e) {
			UpdateSelectedFontStyles(textConcept);
		}

		private void textImplementation_SelectionChanged(object sender,EventArgs e) {
			UpdateSelectedFontStyles(textWriteup);
		}

		private void UpdateSelectedFontStyles(RichTextBox curTextBox) {
			Font selectedFont=curTextBox.SelectionFont;
			if(selectedFont==null) {//this happens if the selected text contains characters of more than one font
				return;
			}
			butBold.BackColor=selectedFont.Bold ? SelectedColor:UnselectedColor;
			butItalics.BackColor=selectedFont.Italic ? SelectedColor:UnselectedColor;
			butUnderline.BackColor=selectedFont.Underline ? SelectedColor:UnselectedColor;
			butStrikeout.BackColor=selectedFont.Strikeout ? SelectedColor:UnselectedColor;
			butBullet.BackColor=curTextBox.SelectionBullet ? SelectedColor:UnselectedColor;
			comboFontSize.Text=selectedFont.SizeInPoints.ToString();
			comboFontType.Text=selectedFont.FontFamily.Name;
		}

		private void gridRequirements_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			if(ReadOnlyRequirementsGrid) {
				return;
			}
			ContextMenu menu=new ContextMenu();
			menu.MenuItems.Add("Add Requirement",(o,arg) => {
				gridRequirements.Focus();
				int indexAdded=AddNewRequirement();
				gridRequirements.ScrollToIndexBottom(indexAdded);
				gridRequirements.SetSelected(new Point(0,indexAdded));
			});
			menu.MenuItems.Add("Remove Requirement",(o,arg) => {
				gridRequirements.Focus();
				int selectedIndex=gridRequirements.GetSelectedIndex();
				if(selectedIndex==-1) {
					return;//Nothing to remove.
				}
				gridRequirements.Focus();//Force the grid to have focus (even though it was just clicked on) because an edit box might technically have focus.
				List<JobRequirement> listRequirements=GetListJobRequirements();
				listRequirements.RemoveAt(selectedIndex);//Should be fine since the grid and this list are always 1:1
				SetListJobRequirements(listRequirements);
				ResizeTextFields();
				OnTextEdited();
			});
			menu.Show(gridRequirements,gridRequirements.PointToClient(Cursor.Position));
		}

		private void gridRequirements_DoubleClick(object sender, EventArgs e) {
			if(ReadOnlyRequirementsGrid) {
				return;
			}
			Point cell=gridRequirements.SelectedCell;
			if(cell.X==-1) {
				gridRequirements.Focus();
				gridRequirements.ScrollToIndexBottom(AddNewRequirement());
				gridRequirements.SetSelected(new Point(0,gridRequirements.ListGridRows.Count-1));//Select the new cell so the user can start typing immediately.
			}
			if(cell.Y==gridRequirements.ListGridRows.Count()-1) {
				gridRequirements.Focus();//Force the grid to have focus because an edit box might technically have focus and we need to save the text value.
				gridRequirements.ScrollToIndexBottom(AddNewRequirement());
				gridRequirements.SetSelected(new Point(cell.X,cell.Y+1));//Select the new cell so the user can start typing immediately.
			}
		}

		private void gridRequirements_CellLeave(object sender,ODGridClickEventArgs e) {
			List<JobRequirement> listRequirements = GetListJobRequirements();
			JobRequirement jobRequirement = listRequirements[e.Row];
			jobRequirement.Description=gridRequirements.ListGridRows[e.Row].Cells[e.Col].Text;
			//Reset the "X" columns that act like check boxes if the description of the requirement just changed.
			if(listRequirements[e.Row].Description!=jobRequirement.Description) {
				jobRequirement.HasExpert=false;
				jobRequirement.HasEngineer=false;
				jobRequirement.HasReviewer=false;
			}
			listRequirements[e.Row]=jobRequirement;
			SetListJobRequirements(listRequirements);
			OnTextEdited();
		}

		private void gridRequirements_CellKeyDown(object sender,ODGridKeyEventArgs e) {
			if(e.KeyEventArgs.KeyCode==Keys.Enter) {
				Point cell=gridRequirements.SelectedCell;
				if(cell.Y==gridRequirements.ListGridRows.Count()-1) {
					gridRequirements.Focus();//Force the grid to have focus because an edit box might technically have focus and we need to save the text value.
					gridRequirements.ScrollToIndexBottom(AddNewRequirement());
					gridRequirements.SetSelected(new Point(cell.X,cell.Y+1));//Select the new cell so the user can start typing immediately.
					e.KeyEventArgs.Handled=true;
				}
			}
		}

		private void gridRequirements_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Button==MouseButtons.Right) {
				return;
			}
			//Do not check ReadOnly here.
			List<JobRequirement> listRequirements=GetListJobRequirements();
			JobRequirement jobRequirement=listRequirements[e.Row];
			switch(e.Col) {
				case 1://Expert
					jobRequirement.HasExpert=!jobRequirement.HasExpert;
					listRequirements[e.Row]=jobRequirement;
					SetListJobRequirements(listRequirements);
					OnTextEdited();
					break;
				case 2://Engineer
					jobRequirement.HasEngineer=!jobRequirement.HasEngineer;
					listRequirements[e.Row]=jobRequirement;
					SetListJobRequirements(listRequirements);
					OnTextEdited();
					break;
				case 3://Reviewer
					jobRequirement.HasReviewer=!jobRequirement.HasReviewer;
					listRequirements[e.Row]=jobRequirement;
					SetListJobRequirements(listRequirements);
					OnTextEdited();
					break;
				case 4://Up
					if(e.Row==0) {
						return;
					}
					listRequirements.Reverse(e.Row-1,2);
					SetListJobRequirements(listRequirements);
					OnTextEdited();
					break;
				case 5://Down
					if(e.Row==gridRequirements.ListGridRows.Count-1) {
						return;
					}
					listRequirements.Reverse(e.Row,2);
					SetListJobRequirements(listRequirements);
					OnTextEdited();
					break;
				default:
					//Do Nothing
					break;
			}
		}

		private int AddNewRequirement() {
			List<JobRequirement> listRequirements = GetListJobRequirements();
			JobRequirement jobRequirement = new JobRequirement();
			jobRequirement.Description="";
			jobRequirement.HasExpert=false;
			jobRequirement.HasEngineer=false;
			jobRequirement.HasReviewer=false;
			listRequirements.Add(jobRequirement);
			SetListJobRequirements(listRequirements);
			OnTextEdited();
			ResizeTextFields();
			return listRequirements.Count()-1;
		}

		
	}

}
