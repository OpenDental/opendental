using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;

namespace UnitTests{
	public partial class FormUIManagerTests:FormODBase{
		public FormUIManagerTests(){
			InitializeComponent();
			//InitializeLayoutManager();
			InitializeUIManager();
		}

		private void FormUIManagerTests_Load(object sender,EventArgs e){
			//bool isHandleCreated=butDelete.IsHandleCreated;//true
			//butDelete.Text="New text";//verified that this will fail as it should
		}

		private void butDelete_Click(object sender,EventArgs e) {
			float fontSize=textBox1.Font.Size;
			OpenDental.MessageBox.Show(fontSize.ToString());
		}

		private void textBox1_FontChanged(object sender,EventArgs e) {
			float fontSize=textBox1.Font.Size;
			return;
		}

		private void button1_Click(object sender,EventArgs e) {
			FormTextBoxTests formTextBoxTests=new FormTextBoxTests();
			formTextBoxTests.ShowDialog();
		}

		private void butDelete_MouseUp(object sender,MouseEventArgs e) {
			return;
		}

		private void button1_MouseUp(object sender,MouseEventArgs e) {
			return;
		}
	}
}
