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

namespace UnitTests {
	public partial class FormValidTextTests:FormODBase {
		public FormValidTextTests() {
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=20;
		}

		private void FormValidTextTests_Load(object sender, EventArgs e){
		
		}

		private void butSetDate_Click(object sender,EventArgs e) {
			//textDate.Text=DateTime.Today.ToShortDateString();
			textDate.Value=DateTime.Today;
			textDate.Validate();
		}

		private void butSetEmpty_Click(object sender,EventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}
	}
}
