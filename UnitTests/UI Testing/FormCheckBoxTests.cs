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
using OpenDental.UI;

namespace UnitTests{
	public partial class FormCheckBoxTests : FormODBase{
		public FormCheckBoxTests(){
			InitializeComponent();
			InitializeLayoutManager();
			LayoutManager.ZoomTest=50;
		}

		private void FormButtonTest_Load(object sender, EventArgs e){
			//checkBox21.set.ZoomTest=33;//for 133%
		}

		private void checkBox21_Click(object sender,EventArgs e) {
			
		}

		private void checkBox21_CheckedChanged(object sender,EventArgs e) {
			MsgBox.Show("New state:"+checkBox21.CheckState.ToString());
		}

		private void checkBox3_CheckedChanged(object sender,EventArgs e) {
			MsgBox.Show("New state:"+checkBox3.CheckState.ToString());
		}

		private void checkBox1_CheckStateChanged(object sender,EventArgs e) {
			MsgBox.Show("New state:"+checkBox1.CheckState.ToString());
		}

		private void checkBox6_CheckStateChanged(object sender,EventArgs e) {
			MsgBox.Show("New state:"+checkBox6.CheckState.ToString());
		}
	}
}
