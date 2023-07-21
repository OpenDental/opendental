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
			Type type = Type.GetType("OpenDental."+textName.Text+", OpenDental");
			if(type is null){
				MsgBox.Show("Type not found.  Check spelling.");
				return;
			}
			//type.GetConstructor(BindingFlags.Instance | BindingFlags.Public,null,CallingConventions.HasThis,
			ConstructorInfo[] constructorInfoArray=type.GetConstructors();
			if(constructorInfoArray.Length!=1){
				throw new Exception("More than one constructor found.  You will need to consolidate them into a single constructor, hopefully without parameters.");
			}
			ParameterInfo[] parameterInfoArray=constructorInfoArray[0].GetParameters();
			if(parameterInfoArray.Length==0){
				FormODBase formODBase=(FormODBase)Activator.CreateInstance(type);
				WpfConverter wpfConverter=new WpfConverter();
				wpfConverter.ConvertToWpf(formODBase,checkOverwrite.Checked);
				Close();
				return;
			}
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
			FormODBase formODBase2=(FormODBase)Activator.CreateInstance(type,objectArray);
			WpfConverter wpfConverter2=new WpfConverter();
			wpfConverter2.ConvertToWpf(formODBase2,checkOverwrite.Checked);
			Close();
		}

		private void butConvert2_Click(object sender,EventArgs e) {
			FormUIManagerTests formUIManagerTests=new FormUIManagerTests();
			WpfConverter wpfConverter=new WpfConverter();
			wpfConverter.ConvertToWpf(formUIManagerTests,true);
			Close();
		}

		private void butShowTest_Click(object sender,EventArgs e) {
			FrmUIManagerTests frmUIManagerTests=new FrmUIManagerTests();
			frmUIManagerTests.ShowDialog();
		}

		private void butWPFgrid_Click(object sender,EventArgs e) {
			FrmGridTest frmGridTest=new FrmGridTest();
			frmGridTest.ShowDialog();
		}

		private void butGridOD_Click(object sender,EventArgs e) {
			FormGridTest formGridTest=new FormGridTest();
			formGridTest.ShowDialog();
		}

		private void button1_Click(object sender,EventArgs e) {
			FormUIManagerTests formUIManagerTests=new FormUIManagerTests();
			formUIManagerTests.ShowDialog();
		}
	}
}
