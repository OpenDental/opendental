using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Xml;

namespace OpenDental {
	public partial class FormSupportStatus:FormODBase {
		private string _regKey;

		public FormSupportStatus() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupportStatus_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			_regKey=PrefC.GetString(PrefName.RegistrationKey);
			textRegKey.Text=_regKey;
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer = XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(_regKey);
				writer.WriteEndElement();
			}
			OpenDentBusiness.localhost.Service1 updateService=CustomerUpdatesProxy.GetWebServiceInstance();
			string result="";
			try {
				if(!CodeBase.ODBuild.IsDebug()) {
					result=updateService.RequestRegKeyStatus(strbuild.ToString());
				}
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				this.Close();
				return;
			}
			try{
				string helpKeyDecrypted=OpenDentalHelp.ODHelp.UpdateHelpKey();
				string[] arrayHelpKeyValues=helpKeyDecrypted.Split(',');
				bool onSupport=PIn.Bool(arrayHelpKeyValues[1]);
				if(onSupport){
					labelHelpKey.Text="Yes";
				}
				else{
					labelHelpKey.Text="No";
				}
			}
			catch(Exception ex){
				labelHelpKey.Text="error:"+ex;
			}
			Cursor=Cursors.Default;
			if(CodeBase.ODBuild.IsDebug()) {
				labelStatusValue.Text="debug mode";
				return;
			}
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				MessageBox.Show(node.InnerText,"Error");
				return;
			}
			node=doc.SelectSingleNode("//KeyDisabled");
			if(node!=null) {
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				labelStatusValue.Text="DISABLED "+node.InnerText;
				labelStatusValue.ForeColor=Color.Red;
			}
			//Checking all three statuses in case RequestRegKeyStatus changes in the future
			node=doc.SelectSingleNode("//KeyEnabled");
			if(node!=null) {
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				labelStatusValue.Text="ENABLED";
				labelStatusValue.ForeColor=Color.Green;
			}
			node=doc.SelectSingleNode("//KeyEnded");
			if(node!=null) {
				labelStatusValue.Text="EXPIRED "+node.InnerText;
				labelStatusValue.ForeColor=Color.Red;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}
	}
}