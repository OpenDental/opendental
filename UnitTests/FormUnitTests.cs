using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental;
using OpenDentBusiness;
using System.IO;
using System.Reflection;
using CodeBase;
using UnitTestsCore;

namespace UnitTests {
	public partial class FormUnitTests:Form {
		private bool _isOracle {
			get {
				//Only two selections, MySQL or Oracle.
				return listType.SelectedIndex!=0;
			}
		}

		public FormUnitTests() {
			InitializeComponent();
		}

		private void FormUnitTests_Load(object sender,EventArgs e) {
			listType.Items.Add("MySql");
			listType.Items.Add("Oracle");
			listType.SelectedIndex=0;
		}

		private void AddResultText(string result) {
			if(this.InvokeRequired) {
				this.BeginInvoke((Action)delegate() { AddResultText(result); });
				return;
			}
			if(string.IsNullOrWhiteSpace(result)) {
				return;
			}
			textResults.AppendText(result.Trim()+"\r\n");
		}

		private void butNewDb_Click(object sender,EventArgs e) {
			textResults.Text="";
			Application.DoEvents();
			Cursor=Cursors.WaitCursor;
			if(!_isOracle) {
				string serverAddr=textAddr.Text;
				string serverPort=textPort.Text;
				if(serverAddr=="") {
					serverAddr="localhost";
				}
				if(serverPort=="") {
					serverPort="3306";
				}
				if(!UnitTestsCore.DatabaseTools.SetDbConnection("",serverAddr,serverPort,textUserName.Text,textPassword.Text,_isOracle)) {
					OpenDental.MessageBox.Show("Could not connect");
					return;
				}
			}
			DatabaseTools.FreshFromDump(textAddr.Text,textPort.Text,textUserName.Text,textPassword.Text,_isOracle);
			textResults.Text+="Fresh database loaded from sql dump.";
			Cursor=Cursors.Default;
		}

		#region Middle Tier

		private void butWebServiceParamCheck_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(!WebServiceT.ConnectToMiddleTier(textMiddleTierURI.Text,textMiddleTierUser.Text,textMiddleTierPassword.Text,Application.ProductVersion,_isOracle)) {
				Cursor=Cursors.Default;
				textResults.Text="Error connecting to Middle Tier.";
				return;
			}
			textResults.Text="";
			Application.DoEvents();
			textResults.Text="This unit test has been deprecated.  Use Microsoft.VisualStudio.TestTools.UnitTesting instead.";
			Cursor=Cursors.Default;
		}

		private void butWebService_Click(object sender,EventArgs e) {
			textResults.Text="These unit tests have been deprecated.  Use Microsoft.VisualStudio.TestTools.UnitTesting instead.";
		}

		#endregion

		private void butSchema_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			textResults.Text="";
			Application.DoEvents();
			if(radioSchema1.Checked) {
				textResults.Text+=SchemaT.TestProposedCrud(textAddr.Text,textPort.Text,textUserName.Text,textPassword.Text,_isOracle);
			}
			else {
				textResults.Text+=SchemaT.CompareProposedToGenerated(_isOracle);
			}
			Cursor=Cursors.Default;
		}

		private void butCore_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			textResults.Text="";
			Application.DoEvents();
			textResults.Text+=CoreTypesT.CreateTempTable(textAddr.Text,textPort.Text,textUserName.Text,textPassword.Text,_isOracle);
			Application.DoEvents();
			textResults.Text+=CoreTypesT.RunAll();
			//}
			//else {
			//	textResults.Text+=CoreTypesT.RunAllMySql();
			//}
			Cursor=Cursors.Default;
		}

		private void butHL7_Click(object sender,EventArgs e) {
			textResults.Text="These unit tests have been deprecated.  Use Microsoft.VisualStudio.TestTools.UnitTesting instead.";
		}

		private void butRun_Click(object sender,EventArgs e) {
			textResults.Text="These unit tests have been deprecated.  Use Microsoft.VisualStudio.TestTools.UnitTesting instead.";
		}
	}



	
}