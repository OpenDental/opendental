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

		public FormEmailCertRegister(string emailAddress) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textEmailAddress.Text=emailAddress;
		}

		private void butSendCode_Click(object sender,EventArgs e) {
			if(textEmailAddress.Text.Trim()=="") {
				MsgBox.Show(this,"Email Address is blank.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("RequestEmailVeritificationCode");
					writer.WriteElementString("RegistrationKey",PrefC.GetString(PrefName.RegistrationKey));
					writer.WriteElementString("EmailAddress",textEmailAddress.Text);
				writer.WriteEndElement();
			}
#if DEBUG
			OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
#else
			OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
			updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
				IWebProxy proxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				proxy.Credentials=cred;
				updateService.Proxy=proxy;
			}
			string xmlResponse="";
			try {
				xmlResponse=updateService.RequestEmailVerificationCode(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Error.")+"  "+ex.Message);
				return;
			}
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xmlResponse);
			string strError=WebServiceRequest.CheckForErrors(doc);
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
			byte[] arrayCertificateBytes=null;
			try {
				arrayCertificateBytes=File.ReadAllBytes(textCertFilePath.Text);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Failed to read the certificate file.")+"  "+ex.Message);
				return;
			}
			X509Certificate2 cert=null;
			try {
				cert=new X509Certificate2(arrayCertificateBytes);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Invalid certificate file.")+"  "+ex.Message);
				return;
			}
			if(EmailNameResolver.GetCertSubjectName(cert).ToLower()!=textEmailAddress.Text.ToLower()) {
				MessageBox.Show(Lan.g(this,"Email certificates are tied to specific addresses or domains.")+"  "
					+Lan.g(this,"The email address on the certificate is")+" "+EmailNameResolver.GetCertSubjectName(cert)+", "
					+Lan.g(this,"but the email address you specified is")+" "+textEmailAddress.Text);
				return;
			}
			if(cert.HasPrivateKey) {
				MsgBox.Show(this,"The specified certificate contains a private key.  For your security, please export your public key and upload that instead.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars=("    ");
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
				writer.WriteStartElement("PostEmailCertificate");
				writer.WriteElementString("RegistrationKey",PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteElementString("EmailAddress",textEmailAddress.Text);
				writer.WriteElementString("VerificationCode",textVerificationCode.Text);
				writer.WriteElementString("CertificateData",Convert.ToBase64String(arrayCertificateBytes));
				writer.WriteEndElement();
			}
#if DEBUG
			OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
#else
			OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
			updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
			if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
				IWebProxy proxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
				ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
				proxy.Credentials=cred;
				updateService.Proxy=proxy;
			}
			string xmlResponse="";
			try {
				xmlResponse=updateService.PostEmailCertificate(strbuild.ToString());
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show("Error: "+ex.Message);
				return;
			}
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xmlResponse);
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Error.")+"  "+Lan.g(this,"Email certificate was not registered.")+"  "+node.InnerText);
				return;
			}
			Cursor=Cursors.Default;
			if(doc.InnerText=="Insert") {
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