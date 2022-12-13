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
			textBox1.Text="8\u23BE8\u23BF8\u23CA8\u23CB8\u23CC8";
			string ur="\u23CC";
			string ul="\u23BF";
			string ll="\u23BE";
			string lr="\u23CB";
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
