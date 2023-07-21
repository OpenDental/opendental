using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;//in PresentationFramework.dll
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;//in PresentationCore.dll
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
/*
Conversion process (checklist):
-Review the form that you are about to convert.  Any unsupported controls or properties?  Fix those first and get reviewed before continuing.
-Look in the code for any references to other Forms. If those forms have not been converted, then stop.  Convert those forms first.
-Run UnitTests FormWpfConverter. Type in the Form name and convert.
-Find the new files in WpfControlsOD/Frms.  You'll need to have "Show all Files" toggled a the top. Right click, Include in Project.
-NO, this was wrong.  Remove.  Add :base() to the constructor.
-Fix all the red areas.  
-Address all the issues suggested at the top in comments (if any). Leave the comments in place for review.
-Wire up all the events.
-Possibly make some labels or other controls slightly bigger due to font change.
-Change OK button to Save and get rid of Cancel button (in Edit windows). Put any old Cancel button functionality into a Close event handler.
-Change all the places where the form is called to now call the new Frm.
-Test thoroughly
-If behavior and look are not absolutely identical, notify Jordan.
-After the new files are reviewed and committed, delete the old Winform files. That gets reviewed on the next round.

Things to look out for:
Lan gets changed to Lans
Lan.F is unresolved. For now, just comment that line out.
See the top of each UI control for information unique to that control.
Loaded event: change EventArgs to RoutedEventArgs.
DialogResults will now be IsDialogOK: True/False.
Use ColorOD to convert colors to/from WPF
Components like Timers and ColorDialogs do not convert. Each is done very differently from original.
ProgressBars will not carry over. We will need to switch to a new progressbar that has not been built yet.
SetFilterControlsAndAction will be handled by a new class that has not been built yet.
Events are very easy to type into XAML.
*/

	///<summary>This converts a form from WF to WPF.</summary>
	public class WpfConverter {
		private FormODBase _formODBase;
		///<summary>Includes trailing slash</summary>
		private string _pathFolderOD;
		///<summary>Without the Form. Example AccountEdit</summary>
		private string _typeName;
		///<summary>Various comments will be added to this string. The comments will then end up in the codebehind as things for the engineer to be aware of or clean up.</summary>
		private string _issues="";

		public void ConvertToWpf(FormODBase formODBase,bool overwrite){
			_formODBase=formODBase;
			_pathFolderOD="C:\\Development\\Versioned\\OpenDental\\";
			if(Environment.MachineName.ToLower().Contains("jordan")){
				_pathFolderOD="E:\\Documents\\GIT REPOS\\Versioned\\OpenDental\\";
			}
			Type type=formODBase.GetType();
			_typeName=type.Name.Substring(4);
			string fileNameXaml=_pathFolderOD+"WpfControlsOD\\Frms\\Frm"+_typeName+".xaml";
			string fileNameXamlCs=_pathFolderOD+"WpfControlsOD\\Frms\\Frm"+_typeName+".xaml.cs";
			if(!overwrite){
				if(File.Exists(fileNameXaml) || File.Exists(fileNameXamlCs)){
					if(!MsgBox.Show(MsgBoxButtons.OKCancel,"File already exists: "+fileNameXaml+" or "+fileNameXamlCs+".  Overwrite?")){
						return;
					}
				}
			}
			string s="<local:FrmODBase x:Class=\"OpenDental.Frm"+_typeName+"\"\r\n";
			s+="             xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n";
			s+="             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n";
			s+="             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\r\n";
			s+="             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\r\n";
			s+="             xmlns:local=\"clr-namespace:OpenDental\"\r\n";
			s+="             xmlns:ui=\"clr-namespace:WpfControls.UI\"\r\n";
			s+="             mc:Ignorable=\"d\"\r\n";
			s+="             ";
			s+="Width=\""+formODBase.ClientSize.Width.ToString()+"\" ";//Uc has no border, so it's just the ClientSize
			s+="Height=\""+formODBase.ClientSize.Height.ToString()+"\" \r\n";
			s+="             ";
			s+="Text=\""+formODBase.Text+"\" ";
			s+="Loaded=\"Frm"+_typeName+"_Loaded\" ";
			s+=">\r\n";
			s+="	<Grid Background=\""+ColorOD.ToWpf(formODBase.BackColor).ToString()+"\" ";
			s+=">\r\n";
			for(int i=0;i<formODBase.Controls.Count;i++){
				s+=ConvertControl(formODBase.Controls[i],indentLevel:2);
			}
			s+="	</Grid>\r\n";
			s+="</local:FrmODBase>";
			File.WriteAllText(fileNameXaml,s);
			ConvertCodeBehind();
			MsgBox.Show("Done");
//todo: key preview
		}

		private void ConvertCodeBehind(){
			if(_typeName=="UIManagerTests"){
				return;
			}
			string fileNameCodeOrig=_pathFolderOD+"OpenDental\\Forms\\Form"+_typeName+".cs";
			string fileNameXamlCs=_pathFolderOD+"WpfControlsOD\\Frms\\Frm"+_typeName+".xaml.cs";
			string s=@"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {";
			if(_issues!=""){
				s+="\r\n/*\r\n"+_issues+"*/";
			}
			string stringOrig=File.ReadAllText(fileNameCodeOrig);
			int idxFirst=stringOrig.IndexOf("{");
			s+=stringOrig.Substring(idxFirst+1);//this will start with a CR
			File.WriteAllText(fileNameXamlCs,s);
		}

		///<summary>Recursive. Converts self and children.</summary>
		private string ConvertControl(System.Windows.Forms.Control control,int indentLevel){
			if(!VisibleSeriously(control)){
				_issues+=control.Name+" Visible=false is not allowed in designer and must be done in code, typically in the Loaded event with some context for why it's being set.\r\n";
			}
			if(control.RightToLeft==System.Windows.Forms.RightToLeft.Yes){
				throw new Exception(control.Name+" Right to Left is not allowed and must be fixed before conversion.");
			}
			if(control is OpenDental.UI.Button button){
				return ConvertButton(control,indentLevel);
			}
			else if(control.GetType()==typeof(OpenDental.UI.CheckBox)){
				return ConvertCheckBox(control,indentLevel);
			}
			else if(control is OpenDental.UI.ComboBox comboBox){
				return ConvertComboBox(control,indentLevel);
			}
			else if(control is OpenDental.UI.GridOD grid){
				return ConvertGrid(control,indentLevel);
			}
			else if(control is OpenDental.UI.GroupBox groupBox){
				return ConvertGroupBox(control,indentLevel);
			}
			else if(control.GetType()==typeof(System.Windows.Forms.Label)){//does not match derived like LinkLabel
				return ConvertLabel(control,indentLevel);
			}
			else if(control is System.Windows.Forms.LinkLabel){
				return ConvertLinkLabel(control,indentLevel);
			}
			else if(control.GetType()==typeof(OpenDental.UI.ListBox)){
				return ConvertListBox(control,indentLevel);
			}
			else if(control is OpenDental.UI.MenuOD){
				return ConvertMenu(control,indentLevel);
			}
			else if(control is OpenDental.UI.MenuStripOD){
				return "";//This is not actually a control, so there's no way to add it.
			}
			else if(control.GetType()==typeof(System.Windows.Forms.Panel)){//does not match derived like tabPage or PanelOD
				return ConvertPanel(control,indentLevel);
			}
			else if(control is System.Windows.Forms.PictureBox pictureBox){
				return ConvertPictureBox(control,indentLevel);
			}
			else if(control is System.Windows.Forms.RadioButton radioButton){
				return ConvertRadioButton(control,indentLevel);
			}
			else if(control is OpenDental.UI.TabControl tabControl){
				return ConvertTabControl(control,indentLevel);
			}
			else if(control is OpenDental.UI.TabPage tabPage){
				//called from inside ConvertTabControl
				return ConvertTabPage(control,indentLevel);
			}
			else if(control is OpenDental.UI.TestForWpf testForWpf){
				return "";
			}
			else if(control.GetType()==typeof(System.Windows.Forms.TextBox)){//does not match derived like valid...
				return ConvertTextBox(control,indentLevel);
			}
			else if(control is OpenDental.ValidDate){
				return ConvertTextVDate(control,indentLevel);
			}
			else if(control is OpenDental.ValidDouble validDouble){
				return ConvertTextVDouble(control,indentLevel);
			}
			else if(control is OpenDental.ValidNum){
				return ConvertTextVInt(control,indentLevel);
			}
			else if(control is OpenDental.UI.ToolBarOD toolBar){
				return ConvertToolBar(control,indentLevel);
			}
			else{
				throw new Exception("Unsupported control type: "+control.GetType().ToString());
			}			
		}

		#region Methods common to all types
		///<summary>So far, just click.</summary>
		private string AddCommonEvents(System.Windows.Forms.Control control){
			FieldInfo fieldInfo = typeof(System.Windows.Forms.Control).GetField("EventClick",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo propertyInfo = typeof(System.Windows.Forms.Control).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			EventHandlerList eventHandlerList = propertyInfo.GetValue(control, new object[] { }) as EventHandlerList;
			object eventKey = fieldInfo.GetValue(control);
			Delegate delegateMy = eventHandlerList[eventKey] as Delegate;
			if(delegateMy==null){
				return "";
			}
			MethodInfo methodInfo=delegateMy.Method;
			string methodName=methodInfo.Name;
			string s="Click=\""+methodName+"\" ";
			return s;
		}

		private string EscapeForXml(string input){
			string retVal=input.Replace("&","&amp;");//must come first to avoid double coversion
			retVal=retVal.Replace("\"","&quot;");
			//retVal=retVal.Replace("'","&apos;");//Tested.  Not necessary, probably because it's inside of double quotes
			retVal=retVal.Replace("<","&lt;");
			retVal=retVal.Replace(">","&gt;");
			return retVal;
		}

		private string SetBoundsAndAnchoring(System.Windows.Forms.Control control){
			string s="";
			int left=control.Left;
			int top=control.Top;
			int right=0;
			int bottom=0;
			//dock top
			if(control.Dock==System.Windows.Forms.DockStyle.Top){
				right=_formODBase.ClientSize.Width-left-control.Width;//should be zero
				s+="HorizontalAlignment=\"Stretch\" ";
				//cannot specify width, or it will malfunction
				s+="Height=\""+control.Height.ToString()+"\" ";
				s+="Margin=\""+left.ToString()+","+top.ToString()+","+right.ToString()+","+bottom.ToString()+"\" ";
				return s;
			}
			if(control.Dock==System.Windows.Forms.DockStyle.Bottom
				|| control.Dock==System.Windows.Forms.DockStyle.Fill
				|| control.Dock==System.Windows.Forms.DockStyle.Left
				|| control.Dock==System.Windows.Forms.DockStyle.Right)
			{
				throw new Exception("This dock style is not yet supported.");
			}
			//we don't support no anchor.
			if((control.Anchor & (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)) == (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)){//left and right
				right=_formODBase.ClientSize.Width-left-control.Width;
				s+="HorizontalAlignment=\"Stretch\" ";
				//cannot specify width, or it will malfunction
			}
			else if((control.Anchor & (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)) == System.Windows.Forms.AnchorStyles.Right){//only right
				right=_formODBase.ClientSize.Width-left-control.Width;
				left=0;
				s+="HorizontalAlignment=\"Right\" ";
				s+="Width=\""+control.Width.ToString()+"\" ";
			}
			else if((control.Anchor & (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)) == System.Windows.Forms.AnchorStyles.Left){//only left
				//s+="HorizontalAlignment=\"Left\" ";//default
				s+="Width=\""+control.Width.ToString()+"\" ";
			}
			if((control.Anchor & (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)) == (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)){//top and bottom
				bottom=_formODBase.ClientSize.Height-top-control.Height;
				s+="VerticalAlignment=\"Stretch\" ";
				//do not specify height
			}
			else if((control.Anchor & (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)) == System.Windows.Forms.AnchorStyles.Bottom){//bottom only
				bottom=_formODBase.ClientSize.Height-top-control.Height;
				top=0;
				s+="VerticalAlignment=\"Bottom\" ";
				s+="Height=\""+control.Height.ToString()+"\" ";
			}
			else if((control.Anchor & (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)) == System.Windows.Forms.AnchorStyles.Top){//top only
				//s+="VerticalAlignment=\"Top\" ";//default
				s+="Height=\""+control.Height.ToString()+"\" ";
			}
			s+="Margin=\""+left.ToString()+","+top.ToString()+","+right.ToString()+","+bottom.ToString()+"\" ";
			return s;
		}

		private bool VisibleSeriously(System.Windows.Forms.Control control) {
			//WinForms has an annoying behavior:
			//If a container is not yet visible, then the controls inside return Visible=false.
			//But when the container becomes visible, these controls magically start returning correct value for Visible.
			//This hacky method gets the real value for Visible.
			MethodInfo methodInfo = control.GetType().GetMethod("GetState",BindingFlags.Instance | BindingFlags.NonPublic);
			//if(methodInfo == null){
			//	return control.Visible;
			//}
			return (bool)(methodInfo.Invoke(control,new object[] { 2 }));//I have no idea what the 2 means.  Pulled it from internet and can't determine meaning.
		}
		#endregion Methods common to all types

		private string ConvertButton(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.Button button=(OpenDental.UI.Button)control;
			string wName="ui:Button";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			string textNew=Regex.Replace(button.Text,"(?<!&)&(?!&)","_");//only replaces single ampersands, not doubles.
			textNew=textNew.Replace("&&","&");//doubles
			textNew=EscapeForXml(textNew);
			s+="Text=\""+textNew+"\" ";
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			bool hasImage=false;
			WpfControls.UI.EnumIcons iconNew=WpfControls.UI.EnumIcons.None;
			if(button.Icon!=UI.EnumIcons.None){
				string txtIconOld=button.Icon.ToString();
				try{
					iconNew=(WpfControls.UI.EnumIcons)Enum.Parse(typeof(WpfControls.UI.EnumIcons),txtIconOld);
					hasImage=true;
				}
				catch{ 
					_issues+=control.Name+" could not convert Icon.\r\n";
				}
			}
			if(iconNew!=WpfControls.UI.EnumIcons.None){
				s+="Icon=\""+iconNew.ToString()+"\" ";
			}
			if(button.Image!=null){
				_issues+=control.Name+" Image cannot be converted automatically. See WpfControlsOD/UI/Button.xaml.cs for instruction on how to add image.\r\n";
				hasImage=true;
			}
			if(hasImage && button.ImageAlign!=System.Drawing.ContentAlignment.MiddleLeft){
				throw new Exception("Button image alignment not supported yet: "+button.ImageAlign.ToString());
			}
			//If Form.AcceptButton is this button, then we need to hook this up to the Enter key.
			//We will let it bubble up and handle it at the window level so that multiline textboxes can intercept it as needed.
			if(_formODBase.AcceptButton==button){
				_issues+=control.Name+" is set as the AcceptButton (triggered when click Enter). You might need to implement that manually as described in FrmODBase.\r\n";
			}
			//Esc key is hooked up for all windows to close the window, so we ignore formODBase.CancelButton. Cancel buttons are going away and will be replaced by X.
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertCheckBox(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.CheckBox checkBox=(OpenDental.UI.CheckBox)control;
			string wName="ui:CheckBox";
			if(checkBox.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
				control.Left+=2;//Checkbox is bigger than in winforms. Adjust so that it stays lined up.
			}
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			string textNew=EscapeForXml(control.Text);
			s+="Text=\""+textNew+"\" ";
			if(checkBox.CheckAlign==System.Drawing.ContentAlignment.TopLeft){
				s+="CheckAlign=\"TopLeft\" ";
			}
			if(checkBox.CheckAlign==System.Drawing.ContentAlignment.MiddleLeft){
				s+="CheckAlign=\"MiddleLeft\" ";
			}
			if(checkBox.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
				s+="CheckAlign=\"MiddleRight\" ";
			}
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			if(checkBox.ThreeState){
				s+="IsThreeState=\"True\" ";
				if(checkBox.CheckState==System.Windows.Forms.CheckState.Checked){
					s+="Checked=\"True\" ";
				}
				//if(checkBox.CheckState==System.Windows.Forms.CheckState.Unchecked){
				//	s+="Checked=\"False\" ";//default
				//}
				if(checkBox.CheckState==System.Windows.Forms.CheckState.Indeterminate){
					s+="Checked=\"{x:Null}\" ";
				}
			}
			else{
				if(checkBox.Checked){
					s+="Checked=\"True\" ";
				}
			}
			//if(!VisibleSeriously(control)){
			//	s+="Visible=\"False\" ";
			//}
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertComboBox(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.ComboBox comboBox=(OpenDental.UI.ComboBox)control;
			string wName="ui:ComboBox";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(comboBox.SelectionModeMulti){
				s+="IsMultiSelect=\"True\" ";
			}
			//Items and selected index usually set in code
//todo: events
			s+="/>\r\n";
			return s;
		}

		private string ConvertGrid(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.GridOD grid=(OpenDental.UI.GridOD)control;
			string wName="ui:Grid";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(!grid.AllowSelection) {
				s+="AllowSelection=\"False\" ";
			}
			if(grid.AllowSortingByColumn){
				s+="SortingByColumnAllowed=\"True\" ";
			}
			if(grid.ArrowsWhenNoFocus){
				s+="ArrowsWhenNoFocus=\"True\" ";
			}
			if(grid.ColorSelectedRow.ToArgb()!=System.Drawing.Color.FromArgb(205,220,235).ToArgb()){
				Color color=ColorOD.ToWpf(grid.ColorSelectedRow);
				s+="ColorSelectedRow=\""+color.ToString()+"\" ";//hex
			}
			if(grid.DoShowRightClickLinks){
				s+="RightClickLinks=\"True\" ";
				_issues+=control.Name+" DoShowRightClickLinks is set to true. This is not yet supported.\r\n";
			}
			if(grid.EditableAcceptsCR){
				s+="EditableAcceptsCR=\"True\" ";
			}
			if(grid.EditableEnterMovesDown){
				s+="EditableEnterMovesDown=\"True\" ";
			}
			if(grid.EditableUsesRTF){
				throw new Exception(control.Name+" EditableUsesRTF is set to true. This will never be supported.");
			}
			if(grid.HasAddButton){
				//fails silently. No need to warn because this is only used in JobManager.
			}
			if(grid.HasAlternateRowsColored){
				s+="AlternateRowsColored=\"True\" ";
			}
			if(grid.HasAutoWrappedHeaders){
				//fails silently. No need to warn because it's redundant.
			}
			if(grid.HasDropDowns){
				s+="DropDownNesting=\"True\" ";
				_issues+=control.Name+" HasDropDowns is set to true. This is not yet supported.\r\n";
			}
			if(!grid.HasLinkDetect){
				//fails silently. No need to warn because it's not a useful property.
			}
			if(grid.HasMultilineHeaders){
				s+="HeadersMultiline=\"True\" ";
				_issues+=control.Name+" HasMultilineHeaders is set to true. This is not yet supported.\r\n";
			}
			if(!grid.HeadersVisible){
				s+="HeadersVisible=\"False\" ";
				_issues+=control.Name+" HeadersVisible is set to false. This is not yet supported.\r\n";
			}
			if(grid.HScrollVisible){
				s+="HScrollVisible=\"True\" ";
				_issues+=control.Name+" HScrollVisible is set to true. This is not yet supported.\r\n";
			}
			if(grid.NoteSpanStart!=0){
				s+="NoteSpanStart=\""+grid.NoteSpanStart.ToString()+"\" ";
				_issues+=control.Name+" NoteSpanStart is not yet supported.\r\n";
			}
			if(grid.NoteSpanStop!=0){
				s+="NoteSpanStop=\""+grid.NoteSpanStop.ToString()+"\" ";
				_issues+=control.Name+" NoteSpanStop is not yet supported.\r\n";
			}
			if(grid.SelectionMode!=OpenDental.UI.GridSelectionMode.OneRow){
				s+="SelectionMode=\""+grid.SelectionMode.ToString()+"\" ";
			}
			if(!grid.ShowContextMenu){
				s+="ContextMenuShows=\"False\" ";
				_issues+=control.Name+" ShowContextMenu is set to false. This is not yet supported.\r\n";
			}
			if(grid.Title!=""){
				s+="Title=\""+grid.Title+"\" ";
			}
			if(!grid.TitleVisible){
				s+="TitleVisible=\"False\" ";
				_issues+=control.Name+" TitleVisible is set to false. This is not yet supported.\r\n";
			}
			if(grid.TranslationName!=""){
				s+="TranslationName=\""+grid.TranslationName+"\" ";
			}
			if(!grid.VScrollVisible){
				//Fail silently. VScrollVisiblity is now always automatic.
			}
			if(!grid.WrapText){
				s+="WrapText=\"False\" ";
				_issues+=control.Name+" WrapText is set to false. This is not yet supported.\r\n";
			}
//todo: events, using original names
			s+="/>\r\n";
			return s;
		}

		private string ConvertGroupBox(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.GroupBox groupBox = (OpenDental.UI.GroupBox)control;
			string wName = "ui:GroupBox";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			string textNew=EscapeForXml(control.Text);
			s+="Text=\""+textNew+"\" ";
			s+=">\r\n";
			for(int i=0;i<control.Controls.Count;i++){
				s+=ConvertControl(control.Controls[i],indentLevel+1);
			}
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
		}

		private string ConvertLabel(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.Label label=(System.Windows.Forms.Label)control;
			string wName="ui:Label";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(_formODBase.BackColor!=label.BackColor){//otherwise, stays transparent
				Color color=ColorOD.ToWpf(label.BackColor);
				s+="ColorBack=\""+color.ToString()+"\" ";
			}
			Color colorText=ColorOD.ToWpf(label.ForeColor);
			if(colorText!=Colors.Black){
				s+="ColorText=\""+colorText.ToString()+"\" ";
			}
			string textNew=EscapeForXml(control.Text);
			s+="Text=\""+textNew+"\" ";
			if(label.TextAlign.In(System.Drawing.ContentAlignment.TopRight,System.Drawing.ContentAlignment.BottomRight,System.Drawing.ContentAlignment.MiddleRight)){
				s+="HAlign=\"Right\" ";
			}
			else if(label.TextAlign.In(System.Drawing.ContentAlignment.TopCenter,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.BottomCenter)){
				s+="HAlign=\"Center\" ";
			}
			else{
				//s+="HorizontalContentAlignment=\"Left\" "
			}
			if(label.TextAlign.In(System.Drawing.ContentAlignment.MiddleLeft,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.MiddleRight)){
				s+="VAlign=\"Center\" ";
			}
			else if(label.TextAlign.In(System.Drawing.ContentAlignment.BottomLeft,System.Drawing.ContentAlignment.BottomCenter,System.Drawing.ContentAlignment.BottomRight)){
				s+="VAlign=\"Bottom\" ";
			}
			else{
				//s+="VerticalContentAlignment=\"Top\" ";
			}
			//if(!VisibleSeriously(control)){
			//	s+="Visible=\"False\" ";
			//}
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertLinkLabel(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.LinkLabel linkLabel=(System.Windows.Forms.LinkLabel)control;
			string wName="ui:LinkLabel";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(_formODBase.BackColor!=linkLabel.BackColor){//otherwise, stays transparent
				Color colorBack=ColorOD.ToWpf(linkLabel.BackColor);
				s+="ColorBack=\""+colorBack.ToString()+"\" ";
			}
			string textNew=EscapeForXml(control.Text);
			s+="Text=\""+textNew+"\" ";
			if(linkLabel.TextAlign.In(System.Drawing.ContentAlignment.TopRight,System.Drawing.ContentAlignment.BottomRight,System.Drawing.ContentAlignment.MiddleRight)){
				s+="HAlign=\"Right\" ";
			}
			else if(linkLabel.TextAlign.In(System.Drawing.ContentAlignment.TopCenter,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.BottomCenter)){
				s+="HAlign=\"Center\" ";
			}
			else{
				//s+="HorizontalContentAlignment=\"Left\" "
			}
			if(linkLabel.TextAlign.In(System.Drawing.ContentAlignment.MiddleLeft,System.Drawing.ContentAlignment.MiddleCenter,System.Drawing.ContentAlignment.MiddleRight)){
				s+="VAlign=\"Center\" ";
			}
			else if(linkLabel.TextAlign.In(System.Drawing.ContentAlignment.BottomLeft,System.Drawing.ContentAlignment.BottomCenter,System.Drawing.ContentAlignment.BottomRight)){
				s+="VAlign=\"Bottom\" ";
			}
			else{
				//s+="VerticalContentAlignment=\"Top\" ";
			}
//todo: check link locations for links that use escaped characters
			s+="LinkLength=\""+linkLabel.LinkArea.Length.ToString()+"\" ";
			s+="LinkStart=\""+linkLabel.LinkArea.Start.ToString()+"\"\r\n";
			s+=GetIndents(indentLevel)+"\t\t";
			s+="LinkClicked=\""+control.Name+"_LinkClicked\" ";
//todo: events, using original names
			s+="/>\r\n";
			return s;
		}

		private string ConvertListBox(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.ListBox listBox=(OpenDental.UI.ListBox)control;
			string wName="ui:ListBox";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			if(!listBox.ItemStrings.IsNullOrEmpty()){
				string itemStrings=string.Join(",",listBox.ItemStrings);
				itemStrings=EscapeForXml(itemStrings);
				s+="ItemStrings=\""+itemStrings+"\" ";
			}
			s+=SetBoundsAndAnchoring(control);
			if(listBox.SelectionMode==UI.SelectionMode.MultiExtended) {//default of One is already handled.
				s+="SelectionMode=\"MultiExtended\" ";
			}
			if(listBox.SelectionMode==UI.SelectionMode.None) {
				s+="SelectionMode=\"None\" ";
			}
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertMenu(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.MenuOD menuOD=(OpenDental.UI.MenuOD)control;
			string wName="ui:Menu";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			//MenuItems are not set in designer, so nothing else to do.
			s+="/>\r\n";
			return s;
		}

		private string ConvertPanel(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.Panel panel = (System.Windows.Forms.Panel)control;
			string wName = "ui:Panel";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(panel.AutoScroll){
				s+="AutoScroll=\"True\" ";//default is false
			}
			Color color=ColorOD.ToWpf(panel.BackColor);
			s+="ColorBack=\""+color.ToString()+"\" ";
			if(panel.BorderStyle==System.Windows.Forms.BorderStyle.FixedSingle){
				s+="ColorBorder=\"DarkGray\" ";
			}
			s+=AddCommonEvents(control);
			s+=">\r\n";
			for(int i=0;i<control.Controls.Count;i++){
				s+=ConvertControl(control.Controls[i],indentLevel+1);
			}
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
		}

		private string ConvertPictureBox(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.PictureBox pictureBox=(System.Windows.Forms.PictureBox)control;
			string wName="ui:PictureBox";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			_issues+=control.Name+" Image cannot be converted automatically. See WpfControlsOD/UI/PictureBox.xaml.cs for instruction on how to add image.\r\n";
			if(pictureBox.SizeMode==System.Windows.Forms.PictureBoxSizeMode.Normal){
				//s+="Stretch=\"None\" ";
				//Default, so nothing to add
			}
			else if(pictureBox.SizeMode==System.Windows.Forms.PictureBoxSizeMode.Zoom){
				s+="Stretch=\"Fit\" ";
			}
			//else if(pictureBox.SizeMode==System.Windows.Forms.PictureBoxSizeMode.){
			//	//This one has no WinForms equivalent
			//	s+="Stretch=\"FillOverflow\" ";
			//}
			else{
			//AutoSize, CenterImage, and StretchImage
				throw new Exception(control.Name+" SizeMode not supported.");
			}
			if(pictureBox.BorderStyle==System.Windows.Forms.BorderStyle.None){
				//do nothing
			}
			else if(pictureBox.BorderStyle==System.Windows.Forms.BorderStyle.FixedSingle
				|| pictureBox.BorderStyle==System.Windows.Forms.BorderStyle.Fixed3D)
			{
				s+="ColorBorder=\"DarkGray\" ";
			}
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertRadioButton(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.RadioButton radioButton=(System.Windows.Forms.RadioButton)control;
			string wName="ui:RadioButton";
			//if(radioButton.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
			//	control.Left+=2;//Checkbox is bigger than in winforms. Adjust so that it stays lined up.
			//}
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			string textNew=EscapeForXml(control.Text);
			s+="Text=\""+textNew+"\" ";
			if(radioButton.CheckAlign==System.Drawing.ContentAlignment.TopLeft){
				s+="CheckAlign=\"TopLeft\" ";
			}
			if(radioButton.CheckAlign==System.Drawing.ContentAlignment.MiddleLeft){
				s+="CheckAlign=\"MiddleLeft\" ";
			}
			if(radioButton.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
				s+="CheckAlign=\"MiddleRight\" ";
			}
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			if(radioButton.Checked){
				s+="Checked=\"True\" ";
			}
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
			//return "";
		}

		private string ConvertTabControl(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.TabControl tabControl = (OpenDental.UI.TabControl)control;
			string wName = "ui:TabControl";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+=">\r\n";
			for(int i=0;i<control.Controls.Count;i++){//TabItems
				s+=ConvertControl(control.Controls[i],indentLevel+1);
			}
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
		}

		private string ConvertTabPage(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.TabPage tabPage = (OpenDental.UI.TabPage)control;
			string wName = "TabItem";//this is a stock MS TabItem
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			//s+=SetBoundsAndAnchoring(control);//no
			string header=EscapeForXml(tabPage.Text);
			s+="Header=\""+header+"\" ";
			s+=">\r\n";
			//tabItem can only have a single content.
			//This anchors all 4 directions with zero margin.
			s+=GetIndents(indentLevel+1)+"<ui:Panel HorizontalAlignment=\"Stretch\" VerticalAlignment=\"Stretch\" >\r\n";
			for(int i=0;i<control.Controls.Count;i++){
				s+=ConvertControl(control.Controls[i],indentLevel+2);
			}
			s+=GetIndents(indentLevel+1)+"</ui:Panel>\r\n";
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
		}

		private string ConvertTextBox(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)control;
			string wName="ui:TextBox";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			Color colorBack=ColorOD.ToWpf(textBox.BackColor);
			if(colorBack!=Colors.White){
				s+="ColorBack=\""+colorBack.ToString()+"\" ";
			}
			Color colorText=ColorOD.ToWpf(textBox.ForeColor);
			if(colorText!=Colors.Black){
				s+="ColorText=\""+colorText.ToString()+"\" ";
			}
			if(textBox.Multiline){
				s+="IsMultiline=\"True\" ";
			}
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			if(textBox.ReadOnly){
				s+="ReadOnly=\"True\" ";
			}
			if(textBox.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			//default left already handled
			if(textBox.TextAlign==System.Windows.Forms.HorizontalAlignment.Center){
				s+="HAlign=\"Center\" ";
			}
			if(textBox.TextAlign==System.Windows.Forms.HorizontalAlignment.Right){
				s+="HAlign=\"Right\" ";
			}
			s+=AddCommonEvents(control);
			//Click is already handled above.
			//TextChanged:
			//This commented code is very handy for finding the internal names of events.
			//FieldInfo[] fieldInfoArray=typeof(System.Windows.Forms.Control).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			//List<string> listStrings=new List<string>();
			//for(int i=0;i<fieldInfoArray.Length;i++){
			//	string name=fieldInfoArray[i].Name;
			//	listStrings.Add(name);
			//}
			FieldInfo fieldInfo = typeof(System.Windows.Forms.Control).GetField("EventText",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo propertyInfo = typeof(System.Windows.Forms.Control).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			EventHandlerList eventHandlerList = propertyInfo.GetValue(control, new object[] { }) as EventHandlerList;
			object eventKey = fieldInfo.GetValue(control);
			Delegate delegateMy = eventHandlerList[eventKey] as Delegate;
			if(delegateMy!=null){
				MethodInfo methodInfo=delegateMy.Method;
				string methodName=methodInfo.Name;
				s+="TextChanged=\""+methodName+"\" ";
			}
			s+="/>\r\n";
			return s;
		}

		private string ConvertTextVDate(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.ValidDate validDate=(OpenDental.ValidDate)control;
			string wName="ui:TextVDate";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(validDate.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
//todo: events, using original names
			s+="/>\r\n";
			return s;
		}

		private string ConvertTextVDouble(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.ValidDouble validDouble=(OpenDental.ValidDouble)control;
			string wName="ui:TextVDouble";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(validDouble.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			s+="MinVal=\""+validDouble.MinVal.ToString()+"\" ";
			s+="MaxVal=\""+validDouble.MaxVal.ToString()+"\" ";
//todo: events, using original names
			s+="/>\r\n";
			return s;
		}

		private string ConvertTextVInt(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.ValidNum validNum=(OpenDental.ValidNum)control;
			string wName="ui:TextVInt";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(validNum.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			s+="MinVal=\""+validNum.MinVal.ToString()+"\" ";
			s+="MaxVal=\""+validNum.MaxVal.ToString()+"\" ";
			if(!validNum.ShowZero){
				s+="ShowZero=\"False\" ";
			}
//todo: events, using original names
			s+="/>\r\n";
			return s;
		}

		private string ConvertToolBar(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.ToolBarOD menuOD=(OpenDental.UI.ToolBarOD)control;
			string wName="ui:ToolBar";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+="/>\r\n";
			//ToolBarButtons are not set in designer, so nothing else to do.
			return s;
		}

		private string GetIndents(int indentLevel){
			string indents="";
			for(int i=0;i<indentLevel;i++){
				indents+="	";
			}
			return indents;
		}
	}
}
