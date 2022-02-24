using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailDigitalSignature:FormODBase {

		private X509Certificate2 _certSig;
		private bool _isTrusted;

		public FormEmailDigitalSignature(X509Certificate2 certSig) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_certSig=certSig;
		}

		private void FormEmailDigitalSignature_Load(object sender,EventArgs e) {
			string signedByAddress=EmailNameResolver.GetCertSubjectName(_certSig);
			textSignedBy.Text=signedByAddress;
			textCertificateAuthority.Text=_certSig.IssuerName.Name;
			textValidFrom.Text=_certSig.NotBefore.ToShortDateString()+" to "+_certSig.NotAfter.ToShortDateString();
			textThumbprint.Text=_certSig.Thumbprint;
			textVersion.Text=_certSig.Version.ToString();
			_isTrusted=(EmailMessages.GetReceiverUntrustedCount(signedByAddress)==-1);
			if(_isTrusted) {
				butTrust.Visible=false;
				textTrustStatus.Text=Lan.g(this,"Trusted");
				textTrustExplanation.Text=Lan.g(this,"Encrypted email and EHR Direct messaging are currently enabled for the signer.");
			}
			else {
				butTrust.Visible=true;
				textTrustStatus.Text=Lan.g(this,"Untrusted or invalid");
				textTrustExplanation.Text=Lan.g(this,"Encrypted email and EHR Direct messaging will not work until this digital signature is trusted by you.")+"  "
					+Lan.g(this,"Click the Trust button to add trust for this digital signature.");
			}
		}

		private void butTrust_Click(object sender,EventArgs e) {
			try {
				EmailMessages.TryAddTrustForSignature(_certSig);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			MsgBox.Show(this,"Trust added for digital signature.");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}