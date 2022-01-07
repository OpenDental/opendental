using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDisplayFieldOrthoEdit : FormODBase {
		private DisplayField _fieldCur;
		///<summary>Used to make sure the user is not adding a duplicate field.</summary>
		private List<DisplayField> _listAllFields;
		private Font headerFont=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);

		///<summary>When ListOrthoChartTabLinks and OrthoChartTab are not null this should be set to the link we are editing.
		///When not null _tabLinkCur.ColumnWidthOverride will be updated instead of FieldCur.ColumnWidth.</summary>
		private OrthoChartTabLink _tabLinkCur=null;
		///<summary>Helper property that simply returns if _tablLinkCur is not null. Used for readability purposes.</summary>
		private bool _isOverrideMode {
			get {
				return (_tabLinkCur!=null);
			}	
		}

		public FormDisplayFieldOrthoEdit(DisplayField fieldCur,List<DisplayField> listAllFields,OrthoChartTabLink tabLinkCur=null) {
			_fieldCur=fieldCur;
			_listAllFields=listAllFields;
			_tabLinkCur=tabLinkCur;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFieldOrthoEdit_Load(object sender,EventArgs e) {
			textDescriptionOverride.Text=_fieldCur.DescriptionOverride;
			textDescription.Text=_fieldCur.Description;
			if(_isOverrideMode) {//Not in default mode, changes to ColWidth override for this tab only.
				labelDefaultMode.Text="("+Lan.g(this,"this tab only")+")";
			}
			int columnWidth=_fieldCur.ColumnWidth;
			if(_tabLinkCur!=null && _tabLinkCur.ColumnWidthOverride>0) {
				columnWidth=_tabLinkCur.ColumnWidthOverride;
			}
			textWidth.Text=POut.Int(columnWidth);
			if(_fieldCur.PickList!=""){
				radioPickList.Checked=true;
				SetPickListVisibility(true);
				textPickList.Text=_fieldCur.PickList;
			}
			else if(_fieldCur.InternalName=="Signature") {
				radioSignature.Checked=true;
				SetPickListVisibility(false);
			}
			else if(_fieldCur.InternalName=="Provider") {
				radioProvider.Checked=true;
				SetPickListVisibility(false);
			}
			else{
				radioText.Checked=true;
				SetPickListVisibility(false);
			}
			FillWidth();
		}

		private void SetPickListVisibility(bool show){
			if(show){
				labelPickList.Visible=true;
				textPickList.Visible=true;
				butUp.Visible=true;
				butDown.Visible=true;
			}
			else{
				labelPickList.Visible=false;
				textPickList.Visible=false;
				butUp.Visible=false;
				butDown.Visible=false;
			}
		}

		private void FillWidth(){
			Graphics g=this.CreateGraphics();
			//Use the display name text box by default because it is what the user will see if it is set.  It is optional so check for empty string.
			string text=string.IsNullOrEmpty(textDescriptionOverride.Text) ? textDescription.Text : textDescriptionOverride.Text;
			int width=(int)g.MeasureString(text,headerFont).Width;
			textWidthMin.Text=width.ToString();
			g.Dispose();
		}

		private void textDisplayName_TextChanged(object sender,EventArgs e) {
			FillWidth();
		}

		private void textInternalName_TextChanged(object sender,EventArgs e) {
			FillWidth();
		}

		private void radioText_Click(object sender,EventArgs e) {
			//these radio buttons are not autocheck
			if(textPickList.Text!=""){//no advantage to checking visibility
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Pick list will be cleared.  Continue?")){
					return;
				}
				textPickList.Text="";
			}
			SetPickListVisibility(false);
			radioText.Checked=true;
			radioPickList.Checked=false;
			radioSignature.Checked=false;
			radioProvider.Checked=false;
		}

		private void radioPickList_Click(object sender,EventArgs e) {
			SetPickListVisibility(true);
			radioText.Checked=false;
			radioPickList.Checked=true;
			radioSignature.Checked=false;
			radioProvider.Checked=false;
		}

		private void radioSignature_Click(object sender,EventArgs e) {
			if(textPickList.Text!=""){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Pick list will be cleared.  Continue?")){
					return;
				}
				textPickList.Text="";
			}
			SetPickListVisibility(false);
			radioText.Checked=false;
			radioPickList.Checked=false;
			radioSignature.Checked=true;
			radioProvider.Checked=false;
		}

		private void radioProvider_Click(object sender,EventArgs e) {
			if(textPickList.Text!=""){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Pick list will be cleared.  Continue?")){
					return;
				}
				textPickList.Text="";
			}
			SetPickListVisibility(false);
			radioText.Checked=false;
			radioPickList.Checked=false;
			radioSignature.Checked=false;
			radioProvider.Checked=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(textPickList.Text==""){
				return;
			}
			int selectionStart=textPickList.SelectionStart;
			//calculate which row to highlight, based on selection start.
			int selectedRow=0;
			int sumPreviousLines=0;
			string[] linesOrig=new string[textPickList.Lines.Length];
			textPickList.Lines.CopyTo(linesOrig,0);
			for(int i=0;i<textPickList.Lines.Length;i++) {
				if(i>0) {
					sumPreviousLines+=textPickList.Lines[i-1].Length+2;//the 2 is for \r\n
				}
				if(selectionStart < sumPreviousLines+textPickList.Lines[i].Length) {
					selectedRow=i;
					break;
				}
			}
			//swap rows
			int newSelectedRow;
			if(selectedRow==0) {
				newSelectedRow=0;//and no swap
			}
			else {
				//doesn't allow me to directly set lines, so:
				string newtext="";
				for(int i=0;i<textPickList.Lines.Length;i++) {
					if(i>0) {
						newtext+="\r\n";
					}
					if(i==selectedRow) {
						newtext+=linesOrig[selectedRow-1];
					}
					else if(i==selectedRow-1) {
						newtext+=linesOrig[selectedRow];
					}
					else {
						newtext+=linesOrig[i];
					}
				}
				textPickList.Text=newtext;
				newSelectedRow=selectedRow-1;
			}
			//highlight the newSelectedRow
			sumPreviousLines=0;
			for(int i=0;i<textPickList.Lines.Length;i++) {
				if(i>0) {
					sumPreviousLines+=textPickList.Lines[i-1].Length+2;//the 2 is for \r\n
				}
				if(newSelectedRow==i) {
					textPickList.Select(sumPreviousLines,textPickList.Lines[i].Length);
					break;
				}
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(textPickList.Text=="") {
				return;
			}
			int selectionStart=textPickList.SelectionStart;
			//calculate which row to highlight, based on selection start.
			int selectedRow=0;
			int sumPreviousLines=0;
			string[] linesOrig=new string[textPickList.Lines.Length];
			textPickList.Lines.CopyTo(linesOrig,0);
			for(int i=0;i<textPickList.Lines.Length;i++) {
				if(i>0) {
					sumPreviousLines+=textPickList.Lines[i-1].Length+2;//the 2 is for \r\n
				}
				if(selectionStart < sumPreviousLines+textPickList.Lines[i].Length) {
					selectedRow=i;
					break;
				}
			}
			//swap rows
			int newSelectedRow;
			if(selectedRow==textPickList.Lines.Length-1) {
				newSelectedRow=textPickList.Lines.Length-1;//and no swap
			}
			else {
				//doesn't allow me to directly set lines, so:
				string newtext="";
				for(int i=0;i<textPickList.Lines.Length;i++) {
					if(i>0) {
						newtext+="\r\n";
					}
					if(i==selectedRow) {
						newtext+=linesOrig[selectedRow+1];
					}
					else if(i==selectedRow+1) {
						newtext+=linesOrig[selectedRow];
					}
					else {
						newtext+=linesOrig[i];
					}
				}
				textPickList.Text=newtext;
				newSelectedRow=selectedRow+1;
			}
			//highlight the newSelectedRow
			sumPreviousLines=0;
			for(int i=0;i<textPickList.Lines.Length;i++) {
				if(i>0) {
					sumPreviousLines+=textPickList.Lines[i-1].Length+2;//the 2 is for \r\n
				}
				if(newSelectedRow==i) {
					textPickList.Select(sumPreviousLines,textPickList.Lines[i].Length);
					break;
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textWidth.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDescription.Text.Trim()=="") {
				MsgBox.Show(this,"Internal Name cannot be blank.");
				return;
			}
			//Verify that the user did not change the field name to the same name as another field.
			//We must verify using textInternalName.Text because _fieldCur is not updated at this time.
			DisplayField displayFieldOther=_listAllFields.FirstOrDefault(x => x!=_fieldCur && x.Description==textDescription.Text);
			if(displayFieldOther!=null) {
				MsgBox.Show(this,"An ortho chart field with that Internal Name already exists.");
				return;
			}
			if(radioPickList.Checked && textPickList.Text==""){
				MsgBox.Show(this,"Pick list values must be entered first");
				return;
			}
			_fieldCur.Description=textDescription.Text;
			_fieldCur.DescriptionOverride=textDescriptionOverride.Text;
			int colWidth=PIn.Int(textWidth.Text);
			if(_isOverrideMode) {//Editing ColumnWidthOverride, 
				_tabLinkCur.ColumnWidthOverride=(colWidth==_fieldCur.ColumnWidth?0:colWidth);
			}
			else {//Editing the default ColumnWidth of the DisplayField.
				_fieldCur.ColumnWidth=PIn.Int(textWidth.Text);//Use FieldCur
			}
			if(radioText.Checked){
				_fieldCur.InternalName="";
			}
			else if(radioPickList.Checked){
				_fieldCur.InternalName="";
			}
			else if(radioSignature.Checked){
				_fieldCur.InternalName="Signature";
			}
			else{//prov
				_fieldCur.InternalName="Provider";
			}
			_fieldCur.PickList=textPickList.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















