using System.Windows.Forms;
using OpenDental;

namespace UnitTests
{
	public partial class FormSandboxJordan : FormODBase
	{
		public FormSandboxJordan()
		{
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			//MsgBox.Show(DeviceDpi.ToString());
			Form4kTests form4kTests=new Form4kTests();
			form4kTests.Show();
		}

		private void FormSandboxJordan_Load(object sender, System.EventArgs e)
		{
			//this.FormBorderStyle=FormBorderStyle.None;
		}
	}
}
