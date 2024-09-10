using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
//using System.Windows.Forms;
using OpenDental;
//using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using WpfControls.UI;

namespace UnitTests
{
	public partial class FormInputBoxTests: FormODBase{
		public FormInputBoxTests()		{
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void butSimpleString_Click(object sender,EventArgs e) {
			InputBox inputBox=new InputBox("Enter a value in the box");
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel){
				return;
			}
			string result=inputBox.StringResult;
			MsgBox.Show(result);
		}

		private void butMultilineString_Click(object sender,EventArgs e) {
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			InputBoxParam inputBoxParam1=new InputBoxParam();
			inputBoxParam1.InputBoxType_=InputBoxType.TextBoxMultiLine;
			inputBoxParam1.LabelText="Enter a multiline string in the box";
			listInputBoxParams.Add(inputBoxParam1);
			InputBox inputBox=new InputBox(listInputBoxParams);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel){
				return;
			}
			string result=inputBox.StringResult;
			MsgBox.Show(result);
		}

		private void butComboBox_Click(object sender,EventArgs e) {
			List<string> listStrings=new List<string>();
			listStrings.Add("Item 1");
			listStrings.Add("Item 2");
			listStrings.Add("Item 3");
			listStrings.Add("Item 4");
			InputBox inputBox=new InputBox("Enter a value in the comboBox",listStrings,selectedIndex:1);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel){
				return;
			}
			WpfControls.UI.ComboBox comboBox=(WpfControls.UI.ComboBox)inputBox.ListControls[0];
			string result=comboBox.SelectedItem.ToString();
			MsgBox.Show(result);
		}

		private void butComplex_Click(object sender,EventArgs e) {
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			//----
			InputBoxParam inputBoxParam1=new InputBoxParam();
			inputBoxParam1.InputBoxType_=InputBoxType.CheckBox;
			inputBoxParam1.LabelText="";//don't use the label for a checkbox
			inputBoxParam1.Text="Check the box";
			listInputBoxParams.Add(inputBoxParam1);
			//----
			InputBoxParam inputBoxParam2=new InputBoxParam();
			inputBoxParam2.InputBoxType_=InputBoxType.ComboMultiSelect;
			inputBoxParam2.LabelText="Combo multi";
			inputBoxParam2.ListSelections=new List<string>();
			inputBoxParam2.ListSelections.Add("Item 1");
			inputBoxParam2.ListSelections.Add("Item 2");
			inputBoxParam2.ListSelections.Add("Item 3");
			inputBoxParam2.ListSelections.Add("Item 4");
			listInputBoxParams.Add(inputBoxParam2);
			//----
			InputBoxParam inputBoxParam3=new InputBoxParam();
			inputBoxParam3.InputBoxType_=InputBoxType.ComboSelect;
			inputBoxParam3.LabelText="Combo";
			inputBoxParam3.ListSelections=new List<string>();
			inputBoxParam3.ListSelections.Add("Item A");
			inputBoxParam3.ListSelections.Add("Item B");
			inputBoxParam3.ListSelections.Add("Item C");
			inputBoxParam3.ListSelections.Add("Item D");
			listInputBoxParams.Add(inputBoxParam3);
			//----
			InputBoxParam inputBoxParam4=new InputBoxParam();
			inputBoxParam4.InputBoxType_=InputBoxType.ListBoxMulti;
			inputBoxParam4.LabelText="List multi";
			inputBoxParam4.ListSelections=new List<string>();
			inputBoxParam4.ListSelections.Add("Item a");
			inputBoxParam4.ListSelections.Add("Item b");
			inputBoxParam4.ListSelections.Add("Item c");
			inputBoxParam4.ListSelections.Add("Item d");
			listInputBoxParams.Add(inputBoxParam4);
			//----
			InputBoxParam inputBoxParam5=new InputBoxParam();
			inputBoxParam5.InputBoxType_=InputBoxType.RadioButton;
			inputBoxParam5.LabelText="Radiobuttons";
			inputBoxParam5.Text="Radio1";
			listInputBoxParams.Add(inputBoxParam5);
			//----
			InputBoxParam inputBoxParam6=new InputBoxParam();
			inputBoxParam6.InputBoxType_=InputBoxType.RadioButton;
			//inputBoxParam6.LabelText="";
			inputBoxParam6.Text="Radio2";
			listInputBoxParams.Add(inputBoxParam6);
			//----
			InputBoxParam inputBoxParam7=new InputBoxParam();
			inputBoxParam7.InputBoxType_=InputBoxType.TextBox;
			inputBoxParam7.LabelText="Textbox. I'm going to make this a really long label that will need to wrap to at least a second line, maybe even a third.";
			//inputBoxParam7.Text="";
			listInputBoxParams.Add(inputBoxParam7);
			//----
			InputBoxParam inputBoxParam8=new InputBoxParam();
			inputBoxParam8.InputBoxType_=InputBoxType.TextBoxMultiLine;
			inputBoxParam8.LabelText="Textbox multi";
			listInputBoxParams.Add(inputBoxParam8);
			//----
			InputBox inputBox=new InputBox(listInputBoxParams);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel){
				return;
			}
			string result="Box checked: ";
			if(inputBox.BoolResult){
				result+="true\r\n";
			}
			else{
				result+="false\r\n";
			}
			ComboBox comboBoxMulti=(ComboBox)inputBox.ListControls[1];
			result+="Combo multi: "+comboBoxMulti.SelectedItem.ToString()+"\r\n";
			ComboBox comboBox=(ComboBox)inputBox.ListControls[2];
			result+="Combo: "+comboBox.SelectedItem.ToString()+"\r\n";
			ListBox listBox=(ListBox)inputBox.ListControls[3];
			result+="List multi: ";
			List<string> listSelected=listBox.GetListSelected<string>();
			for(int i=0;i<listSelected.Count;i++){
				if(i>0){
					result+=",";
				}
				result+=listSelected[i];
			}
			result+="\r\n";
			RadioButton radioButton1=(RadioButton)inputBox.ListControls[4];
			RadioButton radioButton2=(RadioButton)inputBox.ListControls[5];
			result+="Radio buttons: ";
			if(radioButton1.Checked){
				result+="Radio1\r\n";
			}
			else if(radioButton2.Checked){
				result+="Radio1\r\n";
			}
			else{
				result+="none\r\n";
			}
			TextBox textBox=(TextBox)inputBox.ListControls[6];
			result+="Textbox: "+textBox.Text+"\r\n";
			TextBox textBox2=(TextBox)inputBox.ListControls[7];
			result+="Textbox multi: "+textBox2.Text+"\r\n";
			MsgBox.Show(result);
		}

		private void butValids_Click(object sender,EventArgs e) {
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			//----
			InputBoxParam inputBoxParam0 = new InputBoxParam();
			inputBoxParam0.InputBoxType_=InputBoxType.ValidDate;
			inputBoxParam0.LabelText="Date";
			listInputBoxParams.Add(inputBoxParam0);
			//----
			InputBoxParam inputBoxParam1 = new InputBoxParam();
			inputBoxParam1.InputBoxType_=InputBoxType.ValidDouble;
			inputBoxParam1.LabelText="Double";
			listInputBoxParams.Add(inputBoxParam1);
			//----
			InputBoxParam inputBoxParam2 = new InputBoxParam();
			inputBoxParam2.InputBoxType_=InputBoxType.ValidPhone;
			inputBoxParam2.LabelText="Phone";
			listInputBoxParams.Add(inputBoxParam2);
			//----
			InputBoxParam inputBoxParam3 = new InputBoxParam();
			inputBoxParam3.InputBoxType_=InputBoxType.ValidTime;
			inputBoxParam3.LabelText="Time";
			listInputBoxParams.Add(inputBoxParam3);
			//----
			InputBox inputBox=new InputBox(listInputBoxParams);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel){
				return;
			}
			string result="Date: ";
			TextVDate textVDate=(TextVDate)inputBox.ListControls[0];
			result+=textVDate.Value.ToShortDateString()+"\r\n";
			result+="Double: ";
			TextVDouble textVDouble=(TextVDouble)inputBox.ListControls[1];
			result+=textVDouble.Value.ToString()+"\r\n";
			result+="Phone: ";
			TextBox textPhone=(TextBox)inputBox.ListControls[2];
			result+=textPhone.Text+"\r\n";
			result+="Time: ";
			TextVTime textVTime=(TextVTime)inputBox.ListControls[3];
			result+=textVTime.Text+"\r\n";
			MsgBox.Show(result);
		}
	}
}
