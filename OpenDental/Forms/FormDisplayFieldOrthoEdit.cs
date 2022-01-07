using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDisplayFieldOrthoEdit : FormODBase {
		private DisplayField _displayField;
		///<summary>Used to make sure the user is not adding a duplicate field.</summary>
		private List<DisplayField> _listDisplayFields;
		private Font _font=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);

		///<summary>When ListOrthoChartTabLinks and OrthoChartTab are not null this should be set to the link we are editing.
		///When not null _tabLinkCur.ColumnWidthOverride will be updated instead of FieldCur.ColumnWidth.</summary>
		private OrthoChartTabLink _orthoChartTabLink=null;

		public FormDisplayFieldOrthoEdit(DisplayField fieldCur,List<DisplayField> listAllFields,OrthoChartTabLink tabLinkCur=null) {
			_displayField=fieldCur;
			_listDisplayFields=listAllFields;
			_orthoChartTabLink=tabLinkCur;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFieldOrthoEdit_Load(object sender,EventArgs e) {
			textDescriptionOverride.Text=_displayField.DescriptionOverride;
			textDescription.Text=_displayField.Description;
			if(IsOverrideMode()) {//Not in default mode, changes to widthCol override for this tab only.
				labelDefaultMode.Text="("+Lan.g(this,"this tab only")+")";
			}
			int widthCol=_displayField.ColumnWidth;
			if(_orthoChartTabLink!=null && _orthoChartTabLink.ColumnWidthOverride>0) {
				widthCol=_orthoChartTabLink.ColumnWidthOverride;
			}
			textWidth.Text=POut.Int(widthCol);
			if(_displayField.PickList!=""){
				radioPickList.Checked=true;
				SetPickListVisibility(true);
				textPickList.Text=_displayField.PickList;
			}
			else if(_displayField.InternalName=="Signature") {
				radioSignature.Checked=true;
				SetPickListVisibility(false);
			}
			else if(_displayField.InternalName=="Provider") {
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
			string text=textDescription.Text;
			if(!string.IsNullOrEmpty(textDescriptionOverride.Text)) { 
				text=textDescriptionOverride.Text;
			}
			int width=(int)g.MeasureString(text,_font).Width;
			textWidthMin.Text=width.ToString();
			g.Dispose();
		}

		///<summary>Simply returns if _tablLinkCur is not null. Used for readability purposes.</summary>
		private bool IsOverrideMode() {
			return (_orthoChartTabLink!=null);
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
			string[] stringArrayLinesOriginal=new string[textPickList.Lines.Length];
			textPickList.Lines.CopyTo(stringArrayLinesOriginal,0);
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
				string newText="";
				for(int i=0;i<textPickList.Lines.Length;i++) {
					if(i>0) {
						newText+="\r\n";
					}
					if(i==selectedRow) {
						newText+=stringArrayLinesOriginal[selectedRow-1];
					}
					else if(i==selectedRow-1) {
						newText+=stringArrayLinesOriginal[selectedRow];
					}
					else {
						newText+=stringArrayLinesOriginal[i];
					}
				}
				textPickList.Text=newText;
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
			string[] stringArrayLinesOriginal=new string[textPickList.Lines.Length];
			textPickList.Lines.CopyTo(stringArrayLinesOriginal,0);
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
				string newText="";
				for(int i=0;i<textPickList.Lines.Length;i++) {
					if(i>0) {
						newText+="\r\n";
					}
					if(i==selectedRow) {
						newText+=stringArrayLinesOriginal[selectedRow+1];
					}
					else if(i==selectedRow+1) {
						newText+=stringArrayLinesOriginal[selectedRow];
					}
					else {
						newText+=stringArrayLinesOriginal[i];
					}
				}
				textPickList.Text=newText;
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
			//We must verify using textInternalName.Text because _displayFieldCur is not updated at this time.
			DisplayField displayFieldOther=_listDisplayFields.FirstOrDefault(x => x!=_displayField && x.Description==textDescription.Text);
			if(displayFieldOther!=null) {
				MsgBox.Show(this,"An ortho chart field with that Internal Name already exists.");
				return;
			}
			if(radioPickList.Checked && textPickList.Text==""){
				MsgBox.Show(this,"Pick list values must be entered first");
				return;
			}
			_displayField.Description=textDescription.Text;
			_displayField.DescriptionOverride=textDescriptionOverride.Text;
			int widthCol=PIn.Int(textWidth.Text);
			if(IsOverrideMode()) {//Editing ColumnWidthOverride,
				_orthoChartTabLink.ColumnWidthOverride=widthCol;
				if(widthCol==_displayField.ColumnWidth) {
					_orthoChartTabLink.ColumnWidthOverride=0;
				}
			}
			else {//Editing the default ColumnWidth of the DisplayField.
				_displayField.ColumnWidth=PIn.Int(textWidth.Text);//Use _displayFieldCur
			}
			if(radioText.Checked){
				_displayField.InternalName="";
			}
			else if(radioPickList.Checked){
				_displayField.InternalName="";
			}
			else if(radioSignature.Checked){
				_displayField.InternalName="Signature";
			}
			else{//prov
				_displayField.InternalName="Provider";
			}
			_displayField.PickList=textPickList.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















