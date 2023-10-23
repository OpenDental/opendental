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
//Copy this checklist into the top of the form that you are about to convert, right below the namespace.
//Fill it out as you do the conversion.
//But if your answer to the first 2 questions makes you decide not to convert, then don't paste it in yet.
/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |
   Search for "progress". Any progress bars?                         |
   Anything in the Tray?                                             |
   Search for "filter". Any use of SetFilterControlsAndAction?       |
   If yes, then STOP here. Talk to Jordan for strategy               |
-Look in the code for any references to other Forms. If those forms  |
   have not been converted, then STOP.  Convert those forms first.   |
-Will we include TabIndexes?  If so, up to what index?  This applies |
   even if one single control is set so that cursor will start there |
-Grids: get familiar with properties in bold and with events.        |
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|
-Any conversion exceptions? Consider reverting.                      |
-In WpfControlsOD/Frms, include the new files in the project.        |
-Switch to using this checklist in the new Frm. Delete the other one-|
-Do the red areas and issues at top look fixable? Consider reverting |
-Does convert script need any changes instead of fixing manually?    |
-Fix all the red areas.                                              |
-Address all the issues at the top. Leave in place for review.       |
-Verify that the Button click events converted automatically.        |
-Attach all orphaned event handlers to events in constructor.        |
-Possibly make some labels or other controls slightly bigger due to  |
   font change.                                                      |
-Change OK button to Save and get rid of Cancel button (in Edit      |
   windows). Put any old Cancel button functionality into a Close    |
   event handler.                                                    |
-Change all places where the form is called to now call the new Frm. |
-Test thoroughly                                                     |
-Are behavior and look absolutely identical? List any variation.     |
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
End of Checklist=========================================================================================================================
*/

	/*
Things to look out for and Explanations:
Events other than button clicks must go in the constructor instead of XAML/properties docker
	This is so that the references can be found. An annoyance of VS shows zero refs if you enter the events into XAML.
Lan gets changed to Lang
Lang.F must go down in the Load event instead of Ctr.
See the top of each UI control for information unique to that control.
Load event goes in constructor.
DialogResults will now be IsDialogOK: True/False.
Use ColorOD to convert colors to/from WPF
Components like Timers and ColorDialogs do not convert. Each is done very differently from original.
ProgressBars will not carry over. We will need to switch to a new progressbar that has not been built yet.
SetFilterControlsAndAction will be handled by a new class that has not been built yet.
*/

	///<summary>This converts a form from WF to WPF.</summary>
	public class WpfConverter {
		///<summary>Default is int.MaxValue. If user sets this during a conversion, it will convert up to this value. If user leaves it blank, it will be int.MaxValue, indicating that no TabIndexes should be converted.</summary>
		public int TabIndex;

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
			if(!formODBase.HasHelpButton){
				s+="HasHelpButton=\"False\" ";
			}
			if(formODBase.IsBorderLocked){
//todo:
				throw new Exception("Setting IsBorderLocked is not supported. Talk to Jordan.");
			}
			if(formODBase.MaximizeBox!=formODBase.MinimizeBox){//if they don't match
				throw new Exception("MaximizeBox and MinimizeBox should be the same. Talk to Jordan.");
			}
			if(!formODBase.MaximizeBox){
				s+="MinMaxBoxes=\"False\" ";
			}
			//else if both are true, that's normal and nothing to do.
			//if(!formODBase.ShowInTaskbar){
			//We disregard the existing setting. We will always use ShowInTaskbar true.
			//	throw new Exception("Setting ShowInTaskbar to false is not supported.");
			//}
			if(formODBase.StartPosition!=System.Windows.Forms.FormStartPosition.CenterScreen && formODBase.StartPosition!=System.Windows.Forms.FormStartPosition.CenterParent){
				//We always just do center parent, but we allow both of the above as part of conversion.
				throw new Exception("StartPosition must be CenterScreen or CenterParent. Talk to Jordan.");
			}
			if(formODBase.WindowState==System.Windows.Forms.FormWindowState.Maximized){
				s+="StartMaximized=\"True\" ";
			}
			//s+="Loaded=\"Frm"+_typeName+"_Loaded\" ";//add manually in constructor
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
			if(_typeName=="TestAllControls"){
				return;
			}
			string fileNameCodeOrig=_pathFolderOD+"OpenDental\\Forms\\Form"+_typeName+".cs";
			string fileNameXamlCs=_pathFolderOD+"WpfControlsOD\\Frms\\Frm"+_typeName+".xaml.cs";
			string s=@"using System;
using System.Collections.Generic;
using System.ComponentModel;
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
			else if(control.GetType()==typeof(System.Windows.Forms.CheckedListBox)){
				return ConvertCheckedListBox(control,indentLevel);
			}
			else if(control is OpenDental.UI.ComboBox comboBox){
				return ConvertComboBox(control,indentLevel);
			}
			else if(control is OpenDental.UI.ComboBoxClinicPicker){
				return ConvertComboBoxClinicPicker(control,indentLevel);
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
			else if(control is OpenDental.UI.MonthCalendarOD){
				return ConvertMonthCalendar(control,indentLevel);
			}
			else if(control is OpenDental.UI.ODDatePicker){
				return ConvertODDatePicker(control,indentLevel);
			}
			else if(control.GetType()==typeof(OpenDental.ODtextBox)){
				return ConvertODTextBox(control,indentLevel);
			}
			else if(control.GetType()==typeof(System.Windows.Forms.Panel)){//does not match derived like tabPage or PanelOD
				return ConvertPanel(control,indentLevel);
			}
			else if(control.GetType()==typeof(OpenDental.UI.PanelOD)){
				return ConvertPanelOD(control,indentLevel);
			}
			else if(control is System.Windows.Forms.PictureBox pictureBox){
				return ConvertPictureBox(control,indentLevel);
			}
			else if(control is System.Windows.Forms.RadioButton radioButton){
				return ConvertRadioButton(control,indentLevel);
			}
			else if(control is System.Windows.Forms.RichTextBox richTextBox){
				return ConvertRichTextBox(control,indentLevel);
			}
			else if(control is OpenDental.UI.SplitContainer){
				return ConvertSplitContainer(control,indentLevel);
			}
			else if(control is OpenDental.UI.SplitterPanel){
				//called from inside ConvertSplitContainer
				return ConvertSplitterPanel(control,indentLevel);
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
			else if(control is System.Windows.Forms.TreeView treeView){
				return ConvertTreeView(control,indentLevel);
			}
			else if(control is System.Windows.Forms.WebBrowser){
				return ConvertWebBrowser(control,indentLevel);
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
			//dock fill
			if(control.Dock==System.Windows.Forms.DockStyle.Fill){
				s+="HorizontalAlignment=\"Stretch\" ";
				s+="VerticalAlignment=\"Stretch\" ";
				return s;
			}
			if(control.Dock==System.Windows.Forms.DockStyle.Bottom
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
			if(hasImage && button.ImageAlign!=System.Drawing.ContentAlignment.MiddleLeft
				&& button.ImageAlign!=System.Drawing.ContentAlignment.MiddleRight)
			{
				throw new Exception("Button image alignment not supported yet: "+button.ImageAlign.ToString());
			}
			if(hasImage && button.ImageAlign==System.Drawing.ContentAlignment.MiddleRight){
				s+="ImageAlign=\"Right\" ";
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
			//if(checkBox.CheckAlign==System.Drawing.ContentAlignment.MiddleRight){
			//	control.Left+=1;//Checkbox is bigger than in winforms. Adjust so that it stays lined up.
			//}
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

		private string ConvertCheckedListBox(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.CheckedListBox checkedListBox=(System.Windows.Forms.CheckedListBox)control;
			string wName="ui:ListBox";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+="SelectionMode=\"CheckBoxes\" ";
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
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
			s+="/>\r\n";
			return s;
		}

		private string ConvertComboBoxClinicPicker(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.ComboBoxClinicPicker comboBoxClinicPicker=(OpenDental.UI.ComboBoxClinicPicker)control;
			string wName="ui:ComboClinic";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(comboBoxClinicPicker.ForceShowUnassigned){
				s+="ForceShowUnassigned=\"True\" ";
			}
			if(comboBoxClinicPicker.HqDescription!="Unassigned"){
				s+="HqDescription=\""+comboBoxClinicPicker.HqDescription+"\" ";
			}
			if(comboBoxClinicPicker.IncludeAll){
				s+="IncludeAll=\"True\" ";
			}
			if(comboBoxClinicPicker.IncludeHiddenInAll){
				s+="IncludeHiddenInAll=\"True\" ";
			}
			if(comboBoxClinicPicker.IncludeUnassigned){
				s+="IncludeUnassigned=\"True\" ";
			}
			if(comboBoxClinicPicker.IsMultiSelect){
				s+="IsMultiSelect=\"True\" ";
			}
			if(!comboBoxClinicPicker.ShowLabel){
				s+="ShowLabel=\"False\" ";
			}
			//Items and selected index usually set in code
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
				s+="SortingAllowByColumn=\"True\" ";
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
			if(TabIndex<int.MaxValue){
				if(groupBox.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+groupBox.TabIndex.ToString()+"\" ";
				}
			}
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

		private string ConvertMonthCalendar(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.MonthCalendarOD monthCalendar = (OpenDental.UI.MonthCalendarOD)control;
			string wName = "ui:MonthCalendar";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertODDatePicker(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.ODDatePicker oDDatePicker = (OpenDental.UI.ODDatePicker)control;
			string wName = "ui:DatePicker";	
			//The new DatePicker won't have that silly large blank area behind it. This compensates for the change:
			control.Left+=63;
			control.Width=102;
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			//there are no properties or events to convert
			s+="/>\r\n";
			return s;
		}

		private string ConvertODTextBox(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.ODtextBox odtextBox=(OpenDental.ODtextBox)control;
			string wName="ui:TextRich";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(!odtextBox.AllowsCarriageReturns){//default is true
				s+="AllowsCarriageReturns=\"False\" ";
				//IsMultiline not applicable.
			}
			Color colorBack=ColorOD.ToWpf(odtextBox.BackColor);
			if(colorBack!=Colors.White){
				s+="ColorBack=\""+colorBack.ToString()+"\" ";
			}
			Color colorText=ColorOD.ToWpf(odtextBox.ForeColor);
			if(colorText!=Colors.Black){
				s+="ColorText=\""+colorText.ToString()+"\" ";
			}
			if(odtextBox.FormattedTextAllowed){
				s+="FormattedTextAllowed=\"True\" ";
			}
			if(odtextBox.HasAutoNotes){
				s+="HasAutoNotes=\"True\" ";
			}
			if(!control.Enabled){//default is true
				s+="IsEnabled=\"False\" ";
			}
			if(odtextBox.QuickPasteType!=EnumQuickPasteType.None){
				s+="QuickPasteType=\""+odtextBox.QuickPasteType.ToString()+"\" ";
			}
			if(odtextBox.ReadOnly){
				s+="ReadOnly=\"True\" ";
			}
			if(odtextBox.RightClickLinks){
				s+="RightClickLinks=\"True\" ";
			}
			if(!odtextBox.SpellCheckIsEnabled){//default true
				s+="SpellCheckIsEnabled=\"False\" ";
			}
			if(TabIndex<int.MaxValue){
				if(odtextBox.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+odtextBox.TabIndex.ToString()+"\" ";
				}
			}
			//if(!odtextBox.Rtf.IsNullOrEmpty()){//It's never null, even if textbox is empty, so don't check
			//	throw new Exception("Not implemented. Rtf only gets set in code.");
			//}
			if(odtextBox.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			//TextAlign N/A. It's always left top.
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

		private string ConvertPanel(System.Windows.Forms.Control control,int indentLevel){
			if(control.Name=="PanelClient"){
				//it hits this and PanelBorders when it starts maximized.
				string str="";
				for(int i=0;i<control.Controls.Count;i++){
					str+=ConvertControl(control.Controls[i],indentLevel);
				}
				return str;
			}
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
			if(TabIndex<int.MaxValue){
				if(panel.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+panel.TabIndex.ToString()+"\" ";
				}
			}
			s+=AddCommonEvents(control);
			s+=">\r\n";
			for(int i=0;i<control.Controls.Count;i++){
				s+=ConvertControl(control.Controls[i],indentLevel+1);
			}
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
		}

		private string ConvertPanelOD(System.Windows.Forms.Control control,int indentLevel){
			if(control.Name=="PanelBorders"){
				//it hits this and PanelClient when it starts maximized.
				return "";
			}
			OpenDental.UI.PanelOD panel = (OpenDental.UI.PanelOD)control;
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
				Color colorBorder=ColorOD.ToWpf(panel.BorderColor);
				s+="ColorBorder=\""+colorBorder.ToString()+"\" ";
			}
			if(TabIndex<int.MaxValue){
				if(panel.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+panel.TabIndex.ToString()+"\" ";
				}
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
		
		private string ConvertRichTextBox(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.RichTextBox richTextBox=(System.Windows.Forms.RichTextBox)control;
			string wName="ui:TextRich";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(!richTextBox.Multiline){
				s+="AllowsCarriageReturns=\"False\" ";
			}
			Color colorBack=ColorOD.ToWpf(richTextBox.BackColor);
			if(colorBack!=Colors.White){
				s+="ColorBack=\""+colorBack.ToString()+"\" ";
			}
			Color colorText=ColorOD.ToWpf(richTextBox.ForeColor);
			if(colorText!=Colors.Black){
				s+="ColorText=\""+colorText.ToString()+"\" ";
			}
			//FormattedTextAllowed N/A
			//HasAutoNotes N/A
			if(!control.Enabled){
				s+="IsEnabled=\"False\" ";
			}
			//QuickPasteType N/A
			if(richTextBox.ReadOnly){
				s+="ReadOnly=\"True\" ";
			}
			//RightClickLinks N/A
			//SpellCheckIsEnabled N/A
			if(TabIndex<int.MaxValue){
				if(richTextBox.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+richTextBox.TabIndex.ToString()+"\" ";
				}
			}
			//if(!odtextBox.Rtf.IsNullOrEmpty()){//It's never null, even if textbox is empty, so don't check
			//	throw new Exception("Not implemented. Rtf only gets set in code.");
			//}
			if(richTextBox.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			s+=AddCommonEvents(control);
			//Click is already handled above.
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

		private string ConvertSplitContainer(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.UI.SplitContainer splitContainer = (OpenDental.UI.SplitContainer)control;
			string wName = "ui:SplitContainer";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+=">\r\n";
			s+=GetIndents(indentLevel+1)+"<ui:SplitContainer.RowDefinitions>\r\n";
			s+=GetIndents(indentLevel+2)+"<RowDefinition/>\r\n";
			s+=GetIndents(indentLevel+2)+"<RowDefinition Height=\"Auto\"/>\r\n";
			s+=GetIndents(indentLevel+2)+"<RowDefinition/>\r\n";
			s+=GetIndents(indentLevel+1)+"</ui:SplitContainer.RowDefinitions>\r\n";
			s+=GetIndents(indentLevel+1)+"<Border BorderBrush=\"#FFC1C0C0\" BorderThickness=\"1\" Grid.RowSpan=\"3\"/>\r\n";
			s+=GetIndents(indentLevel+1)+"<GridSplitter Grid.Row=\"1\" Height=\"5\" HorizontalAlignment=\"Stretch\" Background=\"Silver\"/>\r\n";
			if(control.Controls.Count!=2){//SplitterPanels
				throw new Exception("SplitContainer must have exactly two SplitterPanels.");
			}
			if(splitContainer.Orientation==System.Windows.Forms.Orientation.Vertical){
				throw new Exception("SplitContainer orientation vertical (splitter is vertical, so panels are side by side) is not supported.");
			}
			s+=ConvertControl(control.Controls[0],indentLevel+1);
			s+=ConvertSplitterPanel(control.Controls[1],indentLevel+1,row:2);
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
		}

		private string ConvertSplitterPanel(System.Windows.Forms.Control control,int indentLevel,int row=0){
			OpenDental.UI.SplitterPanel splitterPanel = (OpenDental.UI.SplitterPanel)control;
			string wName = "ui:Panel";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			if(row>0){
				s+="Grid.Row=\""+row.ToString()+"\" ";
			}
			s+="HorizontalAlignment=\"Stretch\" ";
			s+="VerticalAlignment=\"Stretch\" ";
			s+=">\r\n";
			for(int i=0;i<control.Controls.Count;i++){
				s+=ConvertControl(control.Controls[i],indentLevel+1);
			}
			s+=GetIndents(indentLevel)+"</"+wName+">\r\n";
			return s;
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
			string wName = "ui:TabPage";
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
			if(TabIndex<int.MaxValue){
				if(textBox.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+textBox.TabIndex.ToString()+"\" ";
				}
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
			if(TabIndex<int.MaxValue){
				if(validDate.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+validDate.TabIndex.ToString()+"\" ";
				}
			}
			if(validDate.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			s+="/>\r\n";
			return s;
		}

		private string ConvertTextVDouble(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.ValidDouble validDouble=(OpenDental.ValidDouble)control;
			string wName="ui:TextVDouble";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(TabIndex<int.MaxValue){
				if(validDouble.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+validDouble.TabIndex.ToString()+"\" ";
				}
			}
			if(validDouble.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			s+="MinVal=\""+validDouble.MinVal.ToString()+"\" ";
			s+="MaxVal=\""+validDouble.MaxVal.ToString()+"\" ";
			s+="/>\r\n";
			return s;
		}

		private string ConvertTextVInt(System.Windows.Forms.Control control,int indentLevel){
			OpenDental.ValidNum validNum=(OpenDental.ValidNum)control;
			string wName="ui:TextVInt";
			string s=GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			if(TabIndex<int.MaxValue){
				if(validNum.TabIndex<=TabIndex){
					s+="TabIndexOD=\""+validNum.TabIndex.ToString()+"\" ";
				}
			}
			if(validNum.Text!=""){
				string textNew=EscapeForXml(control.Text);
				s+="Text=\""+textNew+"\" ";
			}
			s+="MinVal=\""+validNum.MinVal.ToString()+"\" ";
			s+="MaxVal=\""+validNum.MaxVal.ToString()+"\" ";
			if(!validNum.ShowZero){
				s+="ShowZero=\"False\" ";
			}
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

		private string ConvertTreeView(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.TreeView treeView = (System.Windows.Forms.TreeView)control;
			string wName = "ui:TreeView";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+=AddCommonEvents(control);
			s+="/>\r\n";
			return s;
		}

		private string ConvertWebBrowser(System.Windows.Forms.Control control,int indentLevel){
			System.Windows.Forms.WebBrowser webBrowser = (System.Windows.Forms.WebBrowser)control;
			string wName = "ui:WebBrowser";
			string s = GetIndents(indentLevel)+"<"+wName+" ";
			s+="x:Name=\""+control.Name+"\" ";
			s+=SetBoundsAndAnchoring(control);
			s+=AddCommonEvents(control);
			s+="/>\r\n";
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
