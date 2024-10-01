using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;
using WpfControls;

namespace OpenDental {
	/// <summary>The editor is for the EFormField even though we're really editing the EFormFieldDef. This editor is not patient facing.</summary>
	public partial class FrmEFormLabelEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>We need access to a few other fields of the EFormDef.</summary>
		public EFormDef EFormDefCur;
		///<summary>All the siblings</summary>
		public List<EFormField> ListEFormFields;
		///<summary>Set this before opening this window. It's the current language being used in the parent form. Format is the text that's showing in the comboBox. Will be empty string if languages are not set up in pref LanguagesUsedByPatients or if the default language is being used in the parent FrmEFormDefs.</summary>
		public string LanguageShowing="";
		private string _openingBracket;
		///<summary>This is all sibings in a horizontal stack, not including the field passed in. If not in a h-stack, then this is an empty list. Even if the current field is not stacking, it can be part of a stack group if the next field is set as stacking. So this list gets recalculated each time the user checks or unchecks the stacking box. If this is a new field, then it is not yet in the list, but we do know where it will potientially go, based on IdxNew, and that's what we use to create this list.</summary>
		private List<EFormField> _listEFormFieldsSiblings;
		///<summary>We don't fire off a signal to update the language cache on other computers until we hit Save in the form window. So each edit window has this variable to keep track of whether there are any new translations. This bubbles up to the parent.</summary>
		public bool IsChangedLanCache;
		///<summary>Keeps track of which label is in use when translating languages. Defaults to textLabel</summary>
		private TextRich _textRichCurrent;

		///<summary></summary>
		public FrmEFormLabelEdit() {
			InitializeComponent();
			Load+=FrmEFormsLabelEdit_Load;
			PreviewKeyDown+=FrmEFormLabelEdit_PreviewKeyDown;
			listBoxFields.MouseDown+=ListBoxFields_MouseDown;
			checkIsWidthPercentage.Click+=CheckIsWidthPercentage_Click;
			checkIsHorizStacking.Click+=CheckIsHorizStacking_Click;
			textLabelTranslated.Click+=TextLabelTranslated_Click;
			textLabel.Click+=TextLabel_Click;
		}

		private void FrmEFormsLabelEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(LanguageShowing==""){
				//Resize textLabel to fill more space
				textLabel.Height=textLabel.Margin.Top
					-26//toolbar
					+textLabel.Height;
				textLabel.Margin=new Thickness(0,26,0,0);
			}
			else{
				textLanguage.Text=LanguageShowing;
				string translation=LanguagePats.TranslateEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,EFormFieldCur.ValueLabel);
				FlowDocument flowDocumentTranslated=EFormFields.DeserializeFlowDocument(translation);
				textLabelTranslated.richTextBox.Document=flowDocumentTranslated;
			}
			//textLabel.richTextBox.IsInactiveSelectionHighlightEnabled=true;//this doesn't seem to work
			FlowDocument flowDocument=EFormFields.DeserializeFlowDocument(EFormFieldCur.ValueLabel);
			textLabel.richTextBox.Document=flowDocument;
			checkIsHorizStacking.Checked=EFormFieldCur.IsHorizStacking;
			bool isPreviousStackable=EFormFields.IsPreviousStackable(EFormFieldCur,ListEFormFields);
			if(!isPreviousStackable){
				labelStackable.Text="previous field is not stackable";
				checkIsHorizStacking.IsEnabled=false;
			}
			textVIntWidth.Value=EFormFieldCur.Width;
			if(EFormFieldCur.IsWidthPercentage){
				labelWidth.Text="Width%";
				checkIsWidthPercentage.Checked=true;
				textVIntMinWidth.Value=EFormFieldCur.MinWidth;
			}
			else{
				labelMinWidth.Visible=false;
				textVIntMinWidth.Visible=false;
			}
			_listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			//this is just for loading. It will recalc each time CheckIsHorizStacking_Click is raised.
			if(_listEFormFieldsSiblings.Count==0){
				labelWidthIsPercentageNote.Visible=false;
			}
			checkBorder.Checked=EFormFieldCur.Border==EnumEFormBorder.ThreeD;
			bool isLastInHorizStack=EFormFields.IsLastInHorizStack(EFormFieldCur,ListEFormFields);
			if(isLastInHorizStack){
				int spaceBelowDefault=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
				labelSpaceDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceBelowDefault.ToString();
				if(EFormFieldCur.SpaceBelow==-1){
					textSpaceBelow.Text="";
				}
				else{
					textSpaceBelow.Text=EFormFieldCur.SpaceBelow.ToString();
				}
			}
			else{
				labelSpaceDefault.Text=Lang.g(this,"only the right-most field in this row may be set");
				textSpaceBelow.IsEnabled=false;
			}
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(ListEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			List<EnumStaticTextField> listStaticTextFields=Enum.GetValues(typeof(EnumStaticTextField))
				.Cast<EnumStaticTextField>()
				.Where(x => !SheetFields.IsStaticTextFieldObsolete(x))
				.ToList();
			listStaticTextFields.RemoveAll(x => x.In(EnumStaticTextField.apptDateMonthSpelled,EnumStaticTextField.apptProcs,EnumStaticTextField.apptProvNameFormal));
			for(int i=0;i<listStaticTextFields.Count;i++) {
				listBoxFields.Items.Add(listStaticTextFields[i].ToString());
			}
			_textRichCurrent=textLabel;
			LayoutToolBar();
		}

		private void LayoutToolBar() {
			EventHandler eventHandlerCut=(sender,e)=>ApplicationCommands.Cut.Execute(null,_textRichCurrent.richTextBox);
			toolBarMain.Add(Lans.g(this,"Cut"),eventHandlerCut);
			EventHandler eventHandlerCopy=(sender,e)=>ApplicationCommands.Copy.Execute(null,_textRichCurrent.richTextBox);
			toolBarMain.Add(Lans.g(this,"Copy"),eventHandlerCopy);
			EventHandler eventHandlerPaste=(sender,e)=>ApplicationCommands.Paste.Execute(null,_textRichCurrent.richTextBox);
			toolBarMain.Add(Lans.g(this,"Paste"),eventHandlerPaste);
			toolBarMain.AddSeparator();
			EventHandler eventHandlerBold=(sender,e)=>EditingCommands.ToggleBold.Execute(null,_textRichCurrent.richTextBox);
			toolBarMain.Add(Lans.g(this,"Bold"),eventHandlerBold);
			EventHandler eventHandlerItalic=(sender,e)=>EditingCommands.ToggleItalic.Execute(null,_textRichCurrent.richTextBox);
			toolBarMain.Add(Lans.g(this,"Italic"),eventHandlerItalic);
			EventHandler eventHandlerUnderline=(sender,e)=>EditingCommands.ToggleUnderline.Execute(null,_textRichCurrent.richTextBox);
			toolBarMain.Add(Lans.g(this,"Underline"),eventHandlerUnderline);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Font"),Font_Click);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Paragraph"),Paragraph_Click);
		}

		private void TextLabel_Click(object sender,EventArgs e) {
			_textRichCurrent=textLabel;
		}

		private void TextLabelTranslated_Click(object sender,EventArgs e) {
			_textRichCurrent=textLabelTranslated;
		}

		private void Font_Click(object sender,EventArgs e) { 
			FrmFont frmFont=new FrmFont();
			TextSelection textSelection=_textRichCurrent.richTextBox.Selection;
			double fontSize=11.5;
			if(!textSelection.IsEmpty){
				object objFontSize = textSelection.GetPropertyValue(TextElement.FontSizeProperty);
				if(objFontSize != DependencyProperty.UnsetValue){
					fontSize=(double)objFontSize;
				}
			}
			frmFont.FontScale=(int)(fontSize/11.5*100);
			frmFont.IsEmpty=textSelection.IsEmpty;
			TextRange textRange=new TextRange(textSelection.Start, textSelection.End);
			object objectForeground=textRange.GetPropertyValue(TextElement.ForegroundProperty);
			if(objectForeground is SolidColorBrush solidColorBrushFore){
				frmFont.ColorText=solidColorBrushFore.Color;
			}
			else{
				frmFont.ColorText=Colors.Black;
			}
			object objectBackground=textRange.GetPropertyValue(TextElement.BackgroundProperty);
			if(objectBackground==null){
				frmFont.ColorBack=null;
			}
			else if(objectBackground is SolidColorBrush solidColorBrushBack){
				frmFont.ColorBack=solidColorBrushBack.Color;
			}
			else{
				frmFont.ColorBack=Colors.White;
			}
			frmFont.ShowDialog();
			if(frmFont.IsDialogCancel){
				return;
			}
			fontSize=frmFont.FontScale*11.5/100;
			if(!textSelection.IsEmpty){
				textSelection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
				textSelection.ApplyPropertyValue(TextElement.ForegroundProperty,new SolidColorBrush(frmFont.ColorText));
				if(frmFont.ColorBack is null){
					textSelection.ApplyPropertyValue(TextElement.BackgroundProperty,null);
				}
				else{
					textSelection.ApplyPropertyValue(TextElement.BackgroundProperty,new SolidColorBrush(frmFont.ColorBack.Value));
				}
			}
		}

		private void Paragraph_Click(object sender,EventArgs e) { 
			List<Paragraph> listParagraphs=new List<Paragraph>();
			TextSelection textSelection=_textRichCurrent.richTextBox.Selection;
			TextPointer textPointerStart=textSelection.Start;
			TextPointer textPointerEnd=textSelection.End;
			if(textPointerStart.CompareTo(textPointerEnd)==0){
				Paragraph paragraph=textPointerStart.Paragraph;
				if(paragraph!=null){
					listParagraphs.Add(paragraph);
				}
			}
			else{
				TextPointer textPointer=textPointerStart;
				while(true){
					if(textPointer==null || textPointer.CompareTo(textPointerEnd)>0){
						break;
					}
					Paragraph paragraph=textPointer.Paragraph;
					if(paragraph!=null && !listParagraphs.Contains(paragraph)){
						listParagraphs.Add(paragraph);
					}
					textPointer=textPointer.GetNextContextPosition(LogicalDirection.Forward);
				}
			}
			FrmParagraph frmParagraph=new FrmParagraph();
			//we don't support mixed properties yet, so just use the first paragraph value
			if(listParagraphs.Count>0){
				frmParagraph.TextIndent=(int)listParagraphs[0].TextIndent;
				frmParagraph.LeftMargin=(int)listParagraphs[0].Margin.Left;
				frmParagraph.TextAlignment_=listParagraphs[0].TextAlignment;
			}
			frmParagraph.ShowDialog();
			if(frmParagraph.IsDialogCancel){
				return;
			}
			for(int i=0;i<listParagraphs.Count;i++){
				listParagraphs[i].TextIndent=frmParagraph.TextIndent;
				listParagraphs[i].Margin=new Thickness(frmParagraph.LeftMargin,0,0,0);
				listParagraphs[i].TextAlignment=frmParagraph.TextAlignment_;
			}
		}

		private void ListBoxFields_MouseDown(object sender,MouseButtonEventArgs e) {
			//SelectedItem will be null if the user clicks inside the ListBox but not on an item in the list.
			if(listBoxFields.SelectedItem==null) {
				return;
			}
			string fieldStr=listBoxFields.SelectedItem.ToString();
			int idxTextSelectionStart=_textRichCurrent.SelectionStart;
//todo: method for InsertAtCursor
			if(_textRichCurrent.SelectionStart < _textRichCurrent.Text.Length-1) {
				_textRichCurrent.Text=_textRichCurrent.Text.Substring(0,_textRichCurrent.SelectionStart)
					+"["+fieldStr+"]"
					+_textRichCurrent.Text.Substring(_textRichCurrent.SelectionStart);
			}
			else{//otherwise, just tack it on the end
				_textRichCurrent.Text+="["+fieldStr+"]";
			}
			_textRichCurrent.Select(idxTextSelectionStart+fieldStr.Length+2,0);
			_textRichCurrent.Focus();
			listBoxFields.ClearSelected();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//see comments in FrmEFormTextBoxEdit.butDelete_Click
			EFormFieldCur.IsDeleted=true;
			//if the field to the right is stacked and this one is not, then change the field to the right to not be stacked.
			int idx=ListEFormFields.IndexOf(EFormFieldCur);
			if(idx<ListEFormFields.Count-1 
				&& !ListEFormFields[idx].IsHorizStacking
				&& ListEFormFields[idx+1].IsHorizStacking)
			{
				ListEFormFields[idx+1].IsHorizStacking=false;
			}
			IsDialogOK=true;
		}

		private void FrmEFormLabelEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		///<summary>Can be recursive. Opening bracket should normally be empty unless it has recently found an unpaired [. Immediately after that, it will look for a matching closing ] in the next run. If it finds one, then it will give a validation error message. If not, then that means the user just used a lone [ for their own purposes, and we won't complain.</summary>
		private bool ProcessInlines(List<Inline> listInlines){
			for(int i=0;i<listInlines.Count;i++){
				if(listInlines[i] is Span span) {
					ProcessInlines(span.Inlines.ToList());
					continue;
				}
				//so it must be a run
				Run run=listInlines[i] as Run;
				if(_openingBracket!=""){
					//the previous run had an opening bracket. Look for either:
					//1. a corresponding closing bracket or
					//2. just more of the word characters (in case we bold one letter in the middle of a replacement string, for example)
					string pattern2=@"^"//start of string
						+@"\w+"//one or more word characters (letters, digits, or underscores)
						+@"\]";//closing bracket
					Regex regex2=new Regex(pattern2);
					Match match2=regex2.Match(run.Text);
					if(match2 is null || !match2.Success){//we could not find an immediate completion with a closing bracket
						//but the replacement string might span a third run
						pattern2=@"^"//start of string
							+@"\w+"//one or more word characters (letters, digits, or underscores)
							+@"$";//end of string
						regex2=new Regex(pattern2);
						match2=regex2.Match(run.Text);
						if(match2 is null || !match2.Success){//nope.
							//they have an opening bracket that's unrelated to replacement strings, so ignore it.
							_openingBracket="";
						}
						else{
							_openingBracket+=match2.Value;
						}
					}
					else{//we found one
						string fullText="["+_openingBracket+match2.Value;
						if(!MsgBox.Show(MsgBoxButtons.OKCancel,"The following replacement field has mixed formatting and will not function as expected: "+fullText+". Continue anyway?")){
							return false;
						}
					}
				}
				string pattern=@"\["//opening square bracket
					+@"("//beginning of capturing group
					+@"\w+"//one or more word characters (letters, digits, or underscores)
					+@")"//end of capturing group
					+@"(?!\])"//negative lookahead assertion to make sure there is no corresponding closing square bracket.
					+@"$";//end of the string
				Regex regex=new Regex(pattern);
				Match match=regex.Match(run.Text);
				if(match is null || !match.Success){
					continue;
				}
				_openingBracket=match.Groups[1].Value;//Only the capturing group. Groups[0] includes the opening bracket
			}
			return true;
		}

		private void CheckIsHorizStacking_Click(object sender,EventArgs e) {
			_listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			if(_listEFormFieldsSiblings.Count>0){
				labelWidthIsPercentageNote.Visible=true;
			}
			else{
				labelWidthIsPercentageNote.Visible=false;
			}
		}

		private void CheckIsWidthPercentage_Click(object sender,EventArgs e) {
			if(checkIsWidthPercentage.Checked==true){
				labelWidth.Text="Width%";
				labelMinWidth.Visible=true;
				textVIntMinWidth.Visible=true;
			}
			else{
				labelWidth.Text="Width";
				labelMinWidth.Visible=false;
				textVIntMinWidth.Visible=false;
			}
		}

		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=ListEFormFields;
			int idx=ListEFormFields.IndexOf(EFormFieldCur);
			frmEFormFieldPicker.ListSelectedIndices.Add(idx);//Prevents self selection as parent
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			textCondParent.Text=frmEFormFieldPicker.ParentSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			textCondValue.Text=EFormL.PickCondValue(ListEFormFields,textCondParent.Text,textCondValue.Text);
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntWidth.IsValid()
				|| !textVIntMinWidth.IsValid())
			{
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			if(LanguageShowing!="" && textLabelTranslated.Text==""){
				MsgBox.Show(this,"Please enter text first.");
				return;
			}
			FlowDocument flowDocumentTranslated=textLabelTranslated.richTextBox.Document;
			List<Block> listBlocksTranslated=flowDocumentTranslated.Blocks.ToList();
			_openingBracket="";
			for(int b=0;b<listBlocksTranslated.Count;b++) {
				if(!(listBlocksTranslated[b] is Paragraph paragraph)) {
					continue;//shouldn't happen
				}
				List<Inline> listInlines=paragraph.Inlines.ToList();
				bool result=ProcessInlines(listInlines);
				if(!result){
					return;
				}
				//we are not going to test for a replacement that spans two paragraphs. We're only looking within each paragraph
			}
			if(textLabel.Text=="") {
				MsgBox.Show(this,"Please enter text first.");
				return;
			}
			FlowDocument flowDocument=textLabel.richTextBox.Document;
			List<Block> listBlocks=flowDocument.Blocks.ToList();
			_openingBracket="";
			for(int b=0;b<listBlocks.Count;b++) {
				if(!(listBlocks[b] is Paragraph paragraph)) {
					continue;//shouldn't happen
				}
				List<Inline> listInlines=paragraph.Inlines.ToList();
				bool result=ProcessInlines(listInlines);
				if(!result){
					return;
				}
				//we are not going to test for a replacement that spans two paragraphs. We're only looking within each paragraph
			}
			int spaceBelow=-1;
			if(textSpaceBelow.Text!=""){
				try{
					spaceBelow=Convert.ToInt32(textSpaceBelow.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix error in Space Below first.");
					return;
				}
				if(spaceBelow<0 || spaceBelow>200){
					MsgBox.Show(this,"Space Below value is invalid.");
					return;
				}
			}
			//end validation
			if(LanguageShowing!=""){
				string translation=EFormFields.SerializeFlowDocument(flowDocumentTranslated);
				IsChangedLanCache=LanguagePats.SaveTranslationEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,translation);
				if(IsChangedLanCache){
					LanguagePats.RefreshCache();
				}
			}
			EFormFieldCur.ValueLabel=EFormFields.SerializeFlowDocument(flowDocument);
			EFormFieldCur.IsHorizStacking=checkIsHorizStacking.Checked==true;
			EFormFieldCur.Width=textVIntWidth.Value;
			EFormFieldCur.IsWidthPercentage=checkIsWidthPercentage.Checked==true;
			//change all siblings to match
			_listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			for(int i=0;i<_listEFormFieldsSiblings.Count;i++){
				_listEFormFieldsSiblings[i].IsWidthPercentage=EFormFieldCur.IsWidthPercentage;
			}
			if(textVIntMinWidth.Visible){
				EFormFieldCur.MinWidth=textVIntMinWidth.Value;
			}
			else{
				EFormFieldCur.MinWidth=0;
			}
			if(checkBorder.Checked==true){
				EFormFieldCur.Border=EnumEFormBorder.ThreeD;
			}
			else{
				EFormFieldCur.Border=EnumEFormBorder.None;
			}
			EFormFieldCur.SpaceBelow=spaceBelow;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(ListEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}