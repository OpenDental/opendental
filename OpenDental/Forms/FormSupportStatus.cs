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
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			using XmlWriter xmlWriter = XmlWriter.Create(stringBuilder,xmlWriterSettings);
			xmlWriter.WriteStartElement("RegistrationKey");
			xmlWriter.WriteString(_regKey);
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
			OpenDentBusiness.localhost.Service1 updateService=CustomerUpdatesProxy.GetWebServiceInstance();
			string result="";
			try {
				if(!CodeBase.ODBuild.IsDebug()) {
					result=updateService.RequestRegKeyStatus(stringBuilder.ToString());
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
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			XmlNode xmlNode=xmlDocument.SelectSingleNode("//Error");
			if(xmlNode!=null) {
				MessageBox.Show(xmlNode.InnerText,"Error");
				return;
			}
			xmlNode=xmlDocument.SelectSingleNode("//KeyDisabled");
			if(xmlNode!=null) {
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				labelStatusValue.Text="DISABLED "+xmlNode.InnerText;
				labelStatusValue.ForeColor=Color.Red;
			}
			//Checking all three statuses in case RequestRegKeyStatus changes in the future
			xmlNode=xmlDocument.SelectSingleNode("//KeyEnabled");
			if(xmlNode!=null) {
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				labelStatusValue.Text="ENABLED";
				labelStatusValue.ForeColor=Color.Green;
			}
			xmlNode=xmlDocument.SelectSingleNode("//KeyEnded");
			if(xmlNode!=null) {
				labelStatusValue.Text="EXPIRED "+xmlNode.InnerText;
				labelStatusValue.ForeColor=Color.Red;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}





	}
}