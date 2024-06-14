using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
//using System.Windows.Controls;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary>Lang is short for language.  Used to translate text to another language.  There are to other similar classes: Lan is used in OD proper by WinForms, but it's not available to WPFControlsOD.  Lans is in the business layer, so it could be used, but it can't handle UI controls.</summary>
	public class Lang{

		//strings-----------------------------------------------
		///<summary>Converts a string to the current language.</summary>
		public static string g(string classType,string text) {
			if(classType.StartsWith("Frm")){
				classType="Form"+classType.Substring(3);
			}
			string retVal=Lans.ConvertString(classType,text);
			return retVal;
		}

		///<summary>Converts a string to the current language.</summary>
		public static string g(object sender,string text) {
			string classType="All";
			if(sender!=null){
				classType=sender.GetType().Name;
			}
			if(classType.StartsWith("Frm")){
				classType="Form"+classType.Substring(3);
			}
			string retVal=Lans.ConvertString(classType,text);
			return retVal;
		}
		
		//forms----------------------------------------------------------------------------------------
		///<summary>F is for Frm. Translates the following controls on the entire frm: title Text, labels, buttons, groupboxes, checkboxes, radiobuttons, grid.  Can include a list of controls to exclude. Also puts all the correct controls into the All category (OK,Cancel,Close,Delete,etc).</summary>
		public static void F(FrmODBase frmODBase,params FrameworkElement[] frameworkElementArrayExclusions) {
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				return;
			}
			List<FrameworkElement> listFrameworkElementExclusions=frameworkElementArrayExclusions.ToList();
			string senderTypeName=frmODBase.GetType().Name;
			if(senderTypeName.StartsWith("Frm")){
				senderTypeName="Form"+senderTypeName.Substring(3);
			}
			//first translate the main title Text on the form:
			if(!listFrameworkElementExclusions.Contains(frmODBase)) {
				string strConverted=Lans.ConvertString(senderTypeName,frmODBase.Text);
				frmODBase.Text=strConverted;
			}
			List<FrameworkElement> listFrameworkElements=frmODBase.GetAllChildControlsFlat();
			for(int i=0;i<listFrameworkElements.Count;i++){
				Type typeContr=listFrameworkElements[i].GetType();
				if(listFrameworkElementExclusions.Contains(listFrameworkElements[i])) {
					continue;//Do not translate because it is excluded.
				}
				if(listFrameworkElements[i] is Button button){
					button.Text=TranslateButton(senderTypeName,button.Text);
					continue;
				}
				if(listFrameworkElements[i] is CheckBox checkBox){
					checkBox.Text=Lans.ConvertString(senderTypeName,checkBox.Text);
					continue;
				}
				//combobox no
				if(listFrameworkElements[i] is Grid grid){
					grid.Title=Lans.ConvertString(grid.TranslationName,grid.Title);
					continue;
				}
				if(listFrameworkElements[i] is GroupBox groupBox){
					groupBox.Text=Lans.ConvertString(senderTypeName,groupBox.Text);
					continue;
				}
				if(listFrameworkElements[i] is Label label){
					label.Text=Lans.ConvertString(senderTypeName,label.Text);
					continue;
				}
				if(listFrameworkElements[i] is LinkLabel linkLabel){
					linkLabel.Text=Lans.ConvertString(senderTypeName,linkLabel.Text);
					continue;
				}
				//listbox no
				//Menu?  Later?
				//panel no
				//picturebox no
				if(listFrameworkElements[i] is RadioButton radioButton){
					radioButton.Text=Lans.ConvertString(senderTypeName,radioButton.Text);
					continue;
				}
				//tabcontrol no
				if(listFrameworkElements[i] is TabPage tabPage){
					tabPage.Header=Lans.ConvertString(senderTypeName,tabPage.Header.ToString());
					continue;
				}
				//textbox, textRich, textV..., etc:  no
				//Toolbar?  Later?
				//ToolBarButton
			}
		}

		private static string TranslateButton(string classType,string strOrig) {
			if(strOrig=="OK"
					|| strOrig=="_OK"
					|| strOrig=="Cancel"
					|| strOrig=="_Cancel"
					|| strOrig=="Close"
					|| strOrig=="_Close"
					|| strOrig=="Add"
					|| strOrig=="_Add"
					|| strOrig=="Delete"
					|| strOrig=="_Delete"
					|| strOrig=="Up"
					|| strOrig=="_Up"
					|| strOrig=="Down"
					|| strOrig=="_Down"
					|| strOrig=="Print"
					|| strOrig=="_Print"
					|| strOrig=="Save"
					|| strOrig=="_Save") 
			{
				return Lans.g("All",strOrig);
			}
			return Lans.g(classType,strOrig);
		}

	}
}