using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailCertRegister:FormODBase {

		public FormEmailCertRegister(string emailAddr) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textEmailAddress.Text=emailAddr;
		}

		private void butSendCode_Click(object sender,EventArgs e) {
			if(textEmailAddress.Text.Trim()=="") {
				MsgBox.Show(this,"Email Address is blank.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings);
			xmlWriter.WriteStartElement("RequestEmailVeritificationCode");
			xmlWriter.WriteElementString("RegistrationKey",PrefC.GetString(PrefName.RegistrationKey));
			xmlWriter.WriteElementString("EmailAddress",textEmailAddress.Text);
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
#if DEBUG
			OpenDental.localhost.Service1 service1=new OpenDental.localhost.Service1();
#else
			OpenDental.customerUpdates.Service1 service1=new OpenDental.customerUpdates.Service1();
			service1.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
				IWebProxy iWebProxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials iCredentials=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				iWebProxy.Credentials=iCredentials;
				service1.Proxy=iWebProxy;
			}
			string xmlResponse="";
			try {
				xmlResponse=service1.RequestEmailVerificationCode(stringBuilder.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Error.")+"  "+ex.Message);
				return;
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(xmlResponse);
			string strError=WebServiceRequest.CheckForErrors(xmlDocument);
			if(!string.IsNullOrEmpty(strError)) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Error.")+"  "+Lan.g(this,"Verification code was not sent.")+"  "+strError);
				return;
			}
			Cursor=Cursors.Default;
			textVerificationCode.Text="";//Clear the old verification code if there was one.
			MessageBox.Show(Lan.g(this,"Done.")+"  "+Lan.g(this,"The verification code has been sent to")+" "+textEmailAddress.Text);
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			if(openFileDialogCert.ShowDialog()==DialogResult.OK) {
				textCertFilePath.Text=openFileDialogCert.FileName;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textVerificationCode.Text.Trim()=="") {
				MsgBox.Show(this,"Verification Code is blank.");
				return;
			}
			if(!File.Exists(textCertFilePath.Text)) {
				MsgBox.Show(this,"Certificate file path is invalid.");
				return;
			}
			string ext=Path.GetExtension(textCertFilePath.Text).ToLower();
			if(ext!=".der" && ext!=".cer") {
				MsgBox.Show(this,"Certificate file path extension must be .der or .cer.");
				return;
			}
			byte[] byteArrayCertificate=null;
			try {
				byteArrayCertificate=File.ReadAllBytes(textCertFilePath.Text);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Failed to read the certificate file.")+"  "+ex.Message);
				return;
			}
			X509Certificate2 x509Certificate2=null;
			try {
				x509Certificate2=new X509Certificate2(byteArrayCertificate);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Invalid certificate file.")+"  "+ex.Message);
				return;
			}
			if(EmailNameResolver.GetCertSubjectName(x509Certificate2).ToLower()!=textEmailAddress.Text.ToLower()) {
				MessageBox.Show(Lan.g(this,"Email certificates are tied to specific addresses or domains.")+"  "
					+Lan.g(this,"The email address on the certificate is")+" "+EmailNameResolver.GetCertSubjectName(x509Certificate2)+", "
					+Lan.g(this,"but the email address you specified is")+" "+textEmailAddress.Text);
				return;
			}
			if(x509Certificate2.HasPrivateKey) {
				MsgBox.Show(this,"The specified certificate contains a private key.  For your security, please export your public key and upload that instead.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings);
			xmlWriter.WriteStartElement("PostEmailCertificate");
			xmlWriter.WriteElementString("RegistrationKey",PrefC.GetString(PrefName.RegistrationKey));
			xmlWriter.WriteElementString("EmailAddress",textEmailAddress.Text);
			xmlWriter.WriteElementString("VerificationCode",textVerificationCode.Text);
			xmlWriter.WriteElementString("CertificateData",Convert.ToBase64String(byteArrayCertificate));
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
#if DEBUG
			OpenDental.localhost.Service1 service1=new OpenDental.localhost.Service1();
#else
			OpenDental.customerUpdates.Service1 service1=new OpenDental.customerUpdates.Service1();
			service1.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
				IWebProxy iWebProxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials iCredentials=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				iWebProxy.Credentials=iCredentials;
				service1.Proxy=iWebProxy;
			}
			string xmlResponse="";
			try {
				xmlResponse=service1.PostEmailCertificate(stringBuilder.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(xmlResponse);
			XmlNode node=xmlDocument.SelectSingleNode("//Error");
			if(node!=null) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Error.")+"  "+Lan.g(this,"Email certificate was not registered.")+"  "+node.InnerText);
				return;
			}
			Cursor=Cursors.Default;
			if(xmlDocument.InnerText=="Insert") {
				MessageBox.Show(Lan.g(this,"Done.")+"  "+Lan.g(this,"The email certificate has been registered for address")+" "+textEmailAddress.Text);
			}
			else {//Updated
				MessageBox.Show(Lan.g(this,"Done.")+"  "+Lan.g(this,"The email certificate has been updated for address")+" "+textEmailAddress.Text);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}