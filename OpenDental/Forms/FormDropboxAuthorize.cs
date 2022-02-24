using System;
using System.Windows.Forms;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDental {
	public partial class FormDropboxAuthorize:FormODBase {
		
		public ProgramProperty ProgramPropertyAccessToken;

		public FormDropboxAuthorize() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDropboxAuthorize_Load(object sender,EventArgs e) {
			try {
				string url=WebSerializer.DeserializePrimitiveOrThrow<string>(
						WebServiceMainHQProxy.GetWebServiceMainHQInstance().BuildOAuthUrl(PrefC.GetString(PrefName.RegistrationKey),OAuthApplicationNames.Dropbox.ToString()));
				System.Diagnostics.Process.Start(url);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error:")+"  "+ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			try {				
				string accessTokenFinal=WebSerializer.DeserializePrimitiveOrThrow<string>(
					WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetDropboxAccessToken(WebSerializer.SerializePrimitive<string>(textAccessToken.Text)));
				ProgramPropertyAccessToken.PropertyValue=accessTokenFinal;
				ProgramProperties.Update(ProgramPropertyAccessToken);
				DataValid.SetInvalid(InvalidType.Programs);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error:")+"  "+ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}