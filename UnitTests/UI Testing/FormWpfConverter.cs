using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;

namespace UnitTests {
	public partial class FormWpfConverter:FormODBase {
		public FormWpfConverter() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void butConvert_Click(object sender,EventArgs e) {
			if(textTabIndex.Text==""){
				MsgBox.Show("Please enter tab index.");
				return;
			}
			int tabIndex=int.MaxValue;
			if(textTabIndex.Text!="-"){
				try{
					tabIndex=int.Parse(textTabIndex.Text);
				}
				catch{
					MsgBox.Show("TabIndex invalid.");
					return;
				} 
			}
			Type type= Type.GetType("OpenDental."+textName.Text+", OpenDental");
			if(type is null){
				MsgBox.Show("Type not found.  Check spelling.");
				return;
			}
			//type.GetConstructor(BindingFlags.Instance | BindingFlags.Public,null,CallingConventions.HasThis,
			ConstructorInfo[] constructorInfoArray=type.GetConstructors();
			ParameterInfo[] parameterInfoArray;
			if(type==typeof(InputBox)){
				//no warning. Takes constructor 0
			}
			else if(constructorInfoArray.Length!=1){
				throw new Exception("More than one constructor found.  You will need to consolidate them into a single constructor, hopefully without parameters.");
			}
			parameterInfoArray=constructorInfoArray[0].GetParameters();
			FormODBase formODBase;
			if(parameterInfoArray.Length==0){
				formODBase=(FormODBase)Activator.CreateInstance(type);
			}
			else{
				object[] objectArray=new object[parameterInfoArray.Length];
				for(int i=0;i<parameterInfoArray.Length;i++){
					Type typeParam=parameterInfoArray[i].GetType();
					if(typeParam.IsValueType){
						objectArray[i]=Activator.CreateInstance(typeParam);
					}
					else{
						objectArray[i]=null;
					}
				}
				formODBase=(FormODBase)Activator.CreateInstance(type,objectArray);
			}
			WpfConverter wpfConverter=new WpfConverter();
			wpfConverter.TabIndex=tabIndex;
			wpfConverter.ConvertToWpf(formODBase,checkOverwrite.Checked);
			Close();
		}

		private void butConvert2_Click(object sender,EventArgs e) {
			if(textTabIndex.Text==""){
				MsgBox.Show("Please enter tab index.");
				return;
			}
			int tabIndex=int.MaxValue;
			if(textTabIndex.Text!="-"){
				try{
					tabIndex=int.Parse(textTabIndex.Text);
				}
				catch{
					MsgBox.Show("TabIndex invalid.");
					return;
				} 
			}
			FormTestAllControls formTestAllControls=new FormTestAllControls();
			WpfConverter wpfConverter=new WpfConverter();
			wpfConverter.TabIndex=tabIndex;
			wpfConverter.ConvertToWpf(formTestAllControls,true);
			Close();
		}

		private void butShowTest_Click(object sender,EventArgs e) {
			FrmTestAllControls frmTestAllControls=new FrmTestAllControls();
			frmTestAllControls.ShowDialog();
		}

		private void butWPFgrid_Click(object sender,EventArgs e) {
			FrmTestGrid frmTestGrid=new FrmTestGrid();
			frmTestGrid.ShowDialog();
		}

		private void butGridOD_Click(object sender,EventArgs e) {
			FormGridTest formGridTest=new FormGridTest();
			formGridTest.ShowDialog();
		}

		private void button1_Click(object sender,EventArgs e) {
			FormTestAllControls formTestAllControls=new FormTestAllControls();
			formTestAllControls.ShowDialog();
		}

		private void butShowFocus_Click(object sender,EventArgs e) {
			FrmTestFocusTabbing frmTestFocusTabbing=new FrmTestFocusTabbing();
			frmTestFocusTabbing.ShowDialog();
		}

		private void butFilterActs_Click(object sender,EventArgs e) {
			FrmTestFilters frmTestFilters=new FrmTestFilters();
			frmTestFilters.ShowDialog();
		}

		private void buttonSplitTests_Click(object sender,EventArgs e) {
			FormSplitContainerTests formSplitContainerTests=new FormSplitContainerTests();
			formSplitContainerTests.ShowDialog();
		}

		private void butSplitsWPF_Click(object sender,EventArgs e) {
			FrmTestSplitters frmTestSplitters=new FrmTestSplitters();
			frmTestSplitters.ShowDialog();
		}

		private void butCombosWPF_Click(object sender,EventArgs e) {
			FrmTestCombos frmTestCombos=new FrmTestCombos();
			frmTestCombos.ShowDialog();
		}

		private void butDatePickerTests_Click(object sender,EventArgs e) {
			FormDatePickerTests formDatePickerTests=new FormDatePickerTests();
			formDatePickerTests.ShowDialog();
		}

		private void butGraphics_Click(object sender,EventArgs e) {
			FormGraphicsTests formGraphicsTests=new FormGraphicsTests();
			formGraphicsTests.ShowDialog();
		}

		private void butMsgBox_Click(object sender,EventArgs e) {
			FrmMsgBoxCopyPaste frmMsgBoxCopyPaste=new FrmMsgBoxCopyPaste("This is some text to show in the box. I hope it's long enough to wrap to the next line. Maybe it won't be. This is some text to show in the box. I hope it's long enough to wrap to the next line. Maybe it won't be. This is some text to show in the box. I hope it's long enough to wrap to the next line. Maybe it won't be. This is some text to show in the box. I hope it's long enough to wrap to the next line. Maybe it won't be.");
			frmMsgBoxCopyPaste.ShowDialog();
		}

		private void butButtonTests_Click(object sender,EventArgs e) {
			FormButtonTest formButtonTest=new FormButtonTest();
			formButtonTest.ShowDialog();
		}
	}
}
