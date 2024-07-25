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
		public List<EFormField> _listEFormFields;
		///<summary></summary>
		public bool IsPreviousStackable;
		private string _openingBracket;

		///<summary></summary>
		public FrmEFormLabelEdit() {
			InitializeComponent();
			Load+=FrmEFormsLabelEdit_Load;
			PreviewKeyDown+=FrmEFormLabelEdit_PreviewKeyDown;
			listBoxFields.MouseDown+=ListBoxFields_MouseDown;
		}

		private void FrmEFormsLabelEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			//textLabel.richTextBox.IsInactiveSelectionHighlightEnabled=true;//this doesn't seem to work
			FlowDocument flowDocument=EFormFields.DeserializeFlowDocument(EFormFieldCur.ValueLabel);
			textLabel.richTextBox.Document=flowDocument;
			checkIsHorizStacking.Checked=EFormFieldCur.IsHorizStacking;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.CondValueStrConverter(_listEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);//This is used to make checkbox values, "X" and "", more user readable by converting them to "Checked" and "Unchecked".
			if(!IsPreviousStackable){
				labelStackable.Text="previous field is not stackable";
				checkIsHorizStacking.IsEnabled=false;
			}
			textVIntWidth.Value=EFormFieldCur.Width;
			List<EnumStaticTextField> listStaticTextFields=Enum.GetValues(typeof(EnumStaticTextField))
				.Cast<EnumStaticTextField>()
				.Where(x => !SheetFields.IsStaticTextFieldObsolete(x))
				.ToList();
			listStaticTextFields.RemoveAll(x => x.In(EnumStaticTextField.apptDateMonthSpelled,EnumStaticTextField.apptProcs,EnumStaticTextField.apptProvNameFormal));
			for(int i=0;i<listStaticTextFields.Count;i++) {
				listBoxFields.Items.Add(listStaticTextFields[i].ToString());
			}
			LayoutToolBar();
		}

		private void LayoutToolBar() {
			EventHandler eventHandlerCut=(sender,e)=>ApplicationCommands.Cut.Execute(null,textLabel.richTextBox);
			toolBarMain.Add(Lans.g(this,"Cut"),eventHandlerCut);
			EventHandler eventHandlerCopy=(sender,e)=>ApplicationCommands.Copy.Execute(null,textLabel.richTextBox);
			toolBarMain.Add(Lans.g(this,"Copy"),eventHandlerCopy);
			EventHandler eventHandlerPaste=(sender,e)=>ApplicationCommands.Paste.Execute(null,textLabel.richTextBox);
			toolBarMain.Add(Lans.g(this,"Paste"),eventHandlerPaste);
			toolBarMain.AddSeparator();
			EventHandler eventHandlerBold=(sender,e)=>EditingCommands.ToggleBold.Execute(null,textLabel.richTextBox);
			toolBarMain.Add(Lans.g(this,"Bold"),eventHandlerBold);
			EventHandler eventHandlerItalic=(sender,e)=>EditingCommands.ToggleItalic.Execute(null,textLabel.richTextBox);
			toolBarMain.Add(Lans.g(this,"Italic"),eventHandlerItalic);
			EventHandler eventHandlerUnderline=(sender,e)=>EditingCommands.ToggleUnderline.Execute(null,textLabel.richTextBox);
			toolBarMain.Add(Lans.g(this,"Underline"),eventHandlerUnderline);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Font"),Font_Click);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Paragraph"),Paragraph_Click);
		}

		private void Font_Click(object sender,EventArgs e) { 
			FrmFont frmFont=new FrmFont();
			TextSelection textSelection=textLabel.richTextBox.Selection;
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
			TextSelection textSelection=textLabel.richTextBox.Selection;
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
			int idxTextSelectionStart=textLabel.SelectionStart;
//todo: method for InsertAtCursor
			if(textLabel.SelectionStart < textLabel.Text.Length-1) {
				textLabel.Text=textLabel.Text.Substring(0,textLabel.SelectionStart)
					+"["+fieldStr+"]"
					+textLabel.Text.Substring(textLabel.SelectionStart);
			}
			else{//otherwise, just tack it on the end
				textLabel.Text+="["+fieldStr+"]";
			}
			textLabel.Select(idxTextSelectionStart+fieldStr.Length+2,0);
			textLabel.Focus();
			listBoxFields.ClearSelected();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur=null;
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
		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=_listEFormFields;
			int idx=_listEFormFields.IndexOf(EFormFieldCur);
			frmEFormFieldPicker.ListSelectedIndices.Add(idx);//Prevents self selection as parent
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			textCondParent.Text=frmEFormFieldPicker.LabelSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			if(textCondParent.Text==""){
				MsgBox.Show("Please enter a name in the Parent field first.");
				return;
			}
			EFormConditionValueSetter conditionValueSetter=EFormL.SetCondValue(_listEFormFields,textCondParent.Text,textCondValue.Text);
			if(conditionValueSetter.ErrorMsg!="") {
				MsgBox.Show(conditionValueSetter.ErrorMsg);
				return;
			}
			textCondValue.Text=conditionValueSetter.SelectedValue;
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntWidth.IsValid()){
				MsgBox.Show("Please fix entry errors first.");
				return;
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
			//end validation
			EFormFieldCur.ValueLabel=EFormFields.SerializeFlowDocument(flowDocument);
			EFormFieldCur.IsHorizStacking=checkIsHorizStacking.Checked==true;
			EFormFieldCur.Width=textVIntWidth.Value;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.CondValueStrConverter(_listEFormFields,textCondParent.Text,textCondValue.Text);//This is used to convert the user readable checkbox values, "Checked" and "Unchecked", into "X" and "" which are what we store in the database. 
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}